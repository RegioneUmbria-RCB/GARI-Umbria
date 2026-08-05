Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider

Namespace OperazioneAgenda_Temp

    Public Class Movimento_Dettaglio_Tecnico

        Sub New(ByVal pivaInput As String, ByVal dataOperazione As Date)

            Piva = pivaInput
            Sa_Cod = 0
            Id_Agenda = 0
            Id_Mov = 0
            Id_Mov_Det = 0
            Id_Reg_Dettaglio = 0
            Data = dataOperazione
            Qta_Ril = 0
            Av_Cod = 0
            Av_Gru = 0
            N = 0
            P = 0
            K = 0
            M = 0
            Efficienza = 0
            Cu = 0

            Soglia_Cod = 0
            Soglia_Quantita = 0
            Soglia_Des = ""

            ff_classe = 0
            Ditta_cod = 0
            Dose = 0
            Freatimetro = 0
            Trap_num = 0
            Inn1_data = DateTime.Now
            Inn2_data = New Date
            Sigla_av = ""
            ExtraStr = ""
            Extra_Int = 0
            Extra_Date = New Date
            dett_cod = 0
            Data_Ril = New Date
            Id_Insetto = 0

            Nitrati = 0
            Parziale = 0

            Piezo1 = 0
            Piezo2 = 0
            Piezo3 = 0
            Piezo4 = 0
            
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
            Id_Reg_Dettaglio = 0
            Data = AGRODATAINIZIO
            Qta_Ril = 0
            Av_Cod = 0
            Av_Gru = 0
            N = 0
            P = 0
            K = 0
            M = 0
            Efficienza = 0
            Cu = 0

            Soglia_Cod = 0
            Soglia_Quantita = 0
            Soglia_Des = ""

            ff_classe = 0
            Ditta_cod = 0
            Dose = 0
            Freatimetro = 0
            Trap_num = 0
            Inn1_data = DateTime.Now
            Inn2_data = New Date
            Sigla_av = ""
            ExtraStr = ""
            Extra_Int = 0
            Extra_Date = New Date
            dett_cod = 0
            Data_Ril = New Date
            Id_Insetto = 0

            Nitrati = 0
            Parziale = 0

            Piezo1 = 0
            Piezo2 = 0
            Piezo3 = 0
            Piezo4 = 0

            'valori che poi il DAL in fase di scrittura sostituirà con quelli reali
            Data_Creazione = #2/1/1900#
            Data_Modifica = #2/1/1900#
            Username_Creazione = ""
            Username_Modifica = ""

            BaseCode = 0
            TopCode = 200000000

        End Sub




        Public Property Inn2_data As String

        Public Property Nitrati As Integer

        Public Property Parziale As Integer

        Public Property Piezo1 As Integer

        Public Property Piezo2 As Integer

        Public Property Piezo3 As Integer

        Public Property Piezo4 As Integer

        Public Property Soglia_Cod As Integer

        Public Property Soglia_Quantita As Decimal

        Public Property Soglia_Des As String

        Public Property ff_classe As Integer

        Public Property Piva As String

        Public Property Sa_Cod As Integer

        Public Property Id_Agenda As Integer

        Public Property Id_Mov As Integer

        Public Property Id_Mov_Det As Integer

        Public Property Id_Reg_Dettaglio As Integer

        Public Property Data As Date

        Public Property Qta_Ril As Decimal

        Public Property Id_Insetto As Integer

        Public Property Av_Cod As Integer

        Public Property Av_Gru As Integer

        Public Property N As Decimal

        Public Property P As Decimal

        Public Property K As Decimal

        Public Property M As Decimal

        Public Property Cu As Decimal

        Public Property Efficienza As Decimal

        Public Property TopCode As Integer

        Public Property BaseCode As Integer

        Public Property Trap_num As Integer

        Public Property Ditta_cod As Integer

        Public Property Dose As Decimal

        Public Property Freatimetro As Decimal

        Public Property Inn1_data As Date

        Public Property Sigla_av As String

        Public Property ExtraStr As String
        Public Property Extra_Int As Integer
        Public Property Extra_Date As DateTime

        Public Property dett_cod As Integer

        Public Property Data_Ril As String

        Public Property Data_Creazione As DateTime
        
        Public Property Data_Modifica As DateTime

        Public Property Username_Creazione As String

        Public Property Username_Modifica As String

    End Class

    Public Class Agenda_Movimenti_Dettagli_Tecnici_Helper

        Public Function Scrivi(ByVal Movimento_Dettaglio_Tecnico As Movimento_Dettaglio_Tecnico,
                               ByVal objParametri As AgronicaCoreParametri
                               ) As Boolean

            Dim flagConnessione As Boolean = False
            Dim flagTransazione As Boolean = False

            Dim idRegDettaglio As Integer

            Try

                Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

                '---------------------------
                'Movimento_Dettaglio_Tecnico
                '---------------------------

                idRegDettaglio = Movimento_Dettaglio_Tecnico.Id_Reg_Dettaglio

                If idRegDettaglio <= 0 Then

                    Dim objSequenze As New Agro_Sequenze

                    idRegDettaglio = objSequenze.NuovoId_Tabella("Movimenti_Dettagli_Tecnici",
                                                                 Movimento_Dettaglio_Tecnico.BaseCode,
                                                                 Movimento_Dettaglio_Tecnico.TopCode,
                                                                 objParametri)

                    objSequenze = Nothing

                End If

                Dim objMovimentiDettagliTecnici As New AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_W

                objMovimentiDettagliTecnici.Scrivi(Movimento_Dettaglio_Tecnico.Piva,
                                                   Movimento_Dettaglio_Tecnico.Sa_Cod,
                                                   Movimento_Dettaglio_Tecnico.Id_Agenda,
                                                   Movimento_Dettaglio_Tecnico.Id_Mov,
                                                   Movimento_Dettaglio_Tecnico.Id_Mov_Det,
                                                   idRegDettaglio,
                                                   Movimento_Dettaglio_Tecnico.Av_Cod,
                                                   Movimento_Dettaglio_Tecnico.Av_Gru,
                                                   Movimento_Dettaglio_Tecnico.Sigla_av,
                                                   Movimento_Dettaglio_Tecnico.Data_Ril,
                                                   Movimento_Dettaglio_Tecnico.Qta_Ril,
                                                   Movimento_Dettaglio_Tecnico.Dose,
                                                   Movimento_Dettaglio_Tecnico.Ditta_cod,
                                                   Movimento_Dettaglio_Tecnico.dett_cod,
                                                   Movimento_Dettaglio_Tecnico.Id_Insetto,
                                                   Movimento_Dettaglio_Tecnico.ff_classe,
                                                   Movimento_Dettaglio_Tecnico.M,
                                                   Movimento_Dettaglio_Tecnico.N,
                                                   Movimento_Dettaglio_Tecnico.K,
                                                   Movimento_Dettaglio_Tecnico.P,
                                                   Movimento_Dettaglio_Tecnico.Parziale,
                                                   Movimento_Dettaglio_Tecnico.Nitrati,
                                                   Movimento_Dettaglio_Tecnico.Freatimetro,
                                                   Movimento_Dettaglio_Tecnico.Piezo1,
                                                   Movimento_Dettaglio_Tecnico.Piezo2,
                                                   Movimento_Dettaglio_Tecnico.Piezo3,
                                                   Movimento_Dettaglio_Tecnico.Piezo4,
                                                   Movimento_Dettaglio_Tecnico.Trap_num,
                                                   Movimento_Dettaglio_Tecnico.Inn1_data,
                                                   Movimento_Dettaglio_Tecnico.Inn2_data,
                                                   New Date,
                                                   New Date,
                                                   "",
                                                   Movimento_Dettaglio_Tecnico.Extra_Int,
                                                   Movimento_Dettaglio_Tecnico.ExtraStr,
                                                   Movimento_Dettaglio_Tecnico.Extra_Date,
                                                   Movimento_Dettaglio_Tecnico.Soglia_Cod,
                                                   Movimento_Dettaglio_Tecnico.Soglia_Quantita,
                                                   Movimento_Dettaglio_Tecnico.Soglia_Des,
                                                   Movimento_Dettaglio_Tecnico.Efficienza,
                                                   Movimento_Dettaglio_Tecnico.Cu,
                                                   Movimento_Dettaglio_Tecnico.Data,
                                                   AGRODATAFINE,
                                                   objParametri,
                                                   Data_creazione:=Movimento_Dettaglio_Tecnico.Data_Creazione,
                                                   username_creazione:=Movimento_Dettaglio_Tecnico.Username_Creazione)

                objMovimentiDettagliTecnici = Nothing

                Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

            Catch ex As Exception

                Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)
                Throw New Exception("[ Agenda_Movimenti_Dettagli_Tecnici_Helper.scrivi() ] : " & ex.Message)
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
                'Movimento_Dettaglio_Tecnico
                '---------------------------

                Dim objMovimentiDettagliTecnici As New AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_W

                objMovimentiDettagliTecnici.Cancella(piva,
                                                     saCod,
                                                     idAgenda,
                                                     idMov,
                                                     idMovDet,
                                                     idRegDettaglio,
                                                     "",
                                                     objParametri)

                objMovimentiDettagliTecnici = Nothing

                Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

            Catch ex As Exception

                Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)
                Throw New Exception("[ Agenda_Movimenti_Dettagli_Tecnici_Helper.cancella() ] : " & ex.Message)

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
                              ByVal objParametri As AgronicaCoreParametri
                              ) As List(Of Movimento_Dettaglio_Tecnico)

            Dim flagConnessione As Boolean = False

            Dim listaMovimentiDettagliTecnici As New List(Of Movimento_Dettaglio_Tecnico)
            Dim Movimento_Dettaglio_Tecnico As Movimento_Dettaglio_Tecnico

            Try

                Utility.VerificaApriConnessione(objParametri, flagConnessione)

                '--------------------------------------------------------
                '-------- MOVIMENTI_DETTAGLI_TECNICI --------------------
                '--------------------------------------------------------
                Dim objMovimentiDettagliTecnici = New AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_R
                Dim Dt_MovimentiDettagliTecnici As DataTable

                Dt_MovimentiDettagliTecnici = objMovimentiDettagliTecnici.Leggi(
                                        CStr(piva),
                                        CInt(saCod),
                                        CInt(idAgenda),
                                        idMov,
                                        idMovDet,
                                        0,
                                        "",
                                        enumSelezioneVariabile.Selezione_TabellaCompleta,
                                        "",
                                        "",
                                        objParametri)

                objMovimentiDettagliTecnici = Nothing

                If Dt_MovimentiDettagliTecnici.Rows.Count > 0 Then

                    For j = 0 To Dt_MovimentiDettagliTecnici.Rows.Count - 1

                        Movimento_Dettaglio_Tecnico = New Movimento_Dettaglio_Tecnico With {
                            .Piva = Dt_MovimentiDettagliTecnici.Rows(j).Item("Piva"),
                            .Sa_Cod = Dt_MovimentiDettagliTecnici.Rows(j).Item("Sa_Cod"),
                            .Id_Agenda = Dt_MovimentiDettagliTecnici.Rows(j).Item("Id_Agenda"),
                            .Id_Mov = Dt_MovimentiDettagliTecnici.Rows(j).Item("Id_Mov"),
                            .Id_Mov_Det = Dt_MovimentiDettagliTecnici.Rows(j).Item("Id_Mov_Det"),
                            .Id_Reg_Dettaglio = Dt_MovimentiDettagliTecnici.Rows(j).Item("Id_Reg_Dettaglio"),
                            .Data = Dt_MovimentiDettagliTecnici.Rows(j).Item("Validita_Inizio"),
                            .Qta_Ril = Dt_MovimentiDettagliTecnici.Rows(j).Item("Qta_Ril"),
                            .Av_Cod = Dt_MovimentiDettagliTecnici.Rows(j).Item("Av_Cod"),
                            .Av_Gru = Dt_MovimentiDettagliTecnici.Rows(j).Item("Av_Gru"),
                            .N = Dt_MovimentiDettagliTecnici.Rows(j).Item("N"),
                            .P = Dt_MovimentiDettagliTecnici.Rows(j).Item("P"),
                            .K = Dt_MovimentiDettagliTecnici.Rows(j).Item("K"),
                            .M = Dt_MovimentiDettagliTecnici.Rows(j).Item("Mg"),
                            .Data_Creazione = CDate(Dt_MovimentiDettagliTecnici.Rows(j).Item("Data_Creazione")),
                            .Data_Modifica = CDate(Dt_MovimentiDettagliTecnici.Rows(j).Item("Data_Modifica")),
                            .Username_Creazione = Dt_MovimentiDettagliTecnici.Rows(j).Item("Username_Creazione"),
                            .Username_Modifica = Dt_MovimentiDettagliTecnici.Rows(j).Item("Username_Modifica")
                        }

                        If Not IsNothing(Dt_MovimentiDettagliTecnici.Rows(j).Item("Efficienza")) AndAlso
                           IsNumeric(Dt_MovimentiDettagliTecnici.Rows(j).Item("Efficienza")) Then
                            Movimento_Dettaglio_Tecnico.Efficienza = CDec(Dt_MovimentiDettagliTecnici.Rows(j).Item("Efficienza"))
                        End If

                        If Not IsNothing(Dt_MovimentiDettagliTecnici.Rows(j).Item("Cu")) AndAlso
                           IsNumeric(Dt_MovimentiDettagliTecnici.Rows(j).Item("Cu")) Then
                            Movimento_Dettaglio_Tecnico.Cu = CDec(Dt_MovimentiDettagliTecnici.Rows(j).Item("Cu"))
                        End If

                        Movimento_Dettaglio_Tecnico.ff_classe = Dt_MovimentiDettagliTecnici.Rows(j).Item("ff_classe")

                        Movimento_Dettaglio_Tecnico.Ditta_cod = Dt_MovimentiDettagliTecnici.Rows(j).Item("Ditta_cod")

                        Movimento_Dettaglio_Tecnico.Dose = Dt_MovimentiDettagliTecnici.Rows(j).Item("Dose")

                        Movimento_Dettaglio_Tecnico.Freatimetro = Dt_MovimentiDettagliTecnici.Rows(j).Item("Freatimetro")

                        Movimento_Dettaglio_Tecnico.Trap_num = Dt_MovimentiDettagliTecnici.Rows(j).Item("Trap_num")


                        If Not IsNothing(Dt_MovimentiDettagliTecnici.Rows(j).Item("Inn1_data")) AndAlso
                            IsDate(Dt_MovimentiDettagliTecnici.Rows(j).Item("Inn1_data")) Then
                            Movimento_Dettaglio_Tecnico.Inn1_data = Dt_MovimentiDettagliTecnici.Rows(j).Item("Inn1_data")
                        End If

                        If Not IsNothing(Dt_MovimentiDettagliTecnici.Rows(j).Item("Inn2_data")) AndAlso
                            IsDate(Dt_MovimentiDettagliTecnici.Rows(j).Item("Inn2_data")) Then
                            Movimento_Dettaglio_Tecnico.Inn2_data = Dt_MovimentiDettagliTecnici.Rows(j).Item("Inn2_data")
                        End If

                        Movimento_Dettaglio_Tecnico.Sigla_av = Dt_MovimentiDettagliTecnici.Rows(j).Item("Sigla_AV")

                        'non usata attualmente, si può usare per un codice personalizzato in forma di stringa e 
                        'non Decimal come con freatimetro
                        Movimento_Dettaglio_Tecnico.ExtraStr = Dt_MovimentiDettagliTecnici.Rows(j).Item("Extra_Str")

                        'Viene salvato il regolamento cod nel carico di magazzino del fertilizzante
                        If Not IsNothing(Dt_MovimentiDettagliTecnici.Rows(j).Item("Extra_Int")) AndAlso
                           IsNumeric(Dt_MovimentiDettagliTecnici.Rows(j).Item("Extra_Int")) Then
                            Movimento_Dettaglio_Tecnico.Extra_Int = CInt(Dt_MovimentiDettagliTecnici.Rows(j).Item("Extra_Int"))
                        End If

                        If Not IsNothing(Dt_MovimentiDettagliTecnici.Rows(j).Item("Extra_Date")) AndAlso
                           IsDate(Dt_MovimentiDettagliTecnici.Rows(j).Item("Extra_Date")) Then
                            Movimento_Dettaglio_Tecnico.Extra_Date = Dt_MovimentiDettagliTecnici.Rows(j).Item("Extra_Date")
                        End If


                        If Not IsNothing(Dt_MovimentiDettagliTecnici.Rows(j).Item("dett_cod")) AndAlso
                            IsNumeric(Dt_MovimentiDettagliTecnici.Rows(j).Item("dett_cod")) Then
                            Movimento_Dettaglio_Tecnico.dett_cod = CInt(Dt_MovimentiDettagliTecnici.Rows(j).Item("dett_cod"))
                        End If


                        If Not IsNothing(Dt_MovimentiDettagliTecnici.Rows(j).Item("Data_Ril")) AndAlso
                            IsDate(Dt_MovimentiDettagliTecnici.Rows(j).Item("Data_Ril")) Then
                            Movimento_Dettaglio_Tecnico.Data_Ril = CDate(Dt_MovimentiDettagliTecnici.Rows(j).Item("Data_Ril"))
                        End If


                        If Not IsNothing(Dt_MovimentiDettagliTecnici.Rows(j).Item("Id_Insetto")) AndAlso
                            IsNumeric(Dt_MovimentiDettagliTecnici.Rows(j).Item("Id_Insetto")) Then
                            Movimento_Dettaglio_Tecnico.Id_Insetto = Dt_MovimentiDettagliTecnici.Rows(j).Item("Id_Insetto")
                        End If

                        'If Not IsNothing(Dt_MovimentiDettagliTecnici.Rows(j).Item("BaseCode")) AndAlso
                        '    IsNumeric(Dt_MovimentiDettagliTecnici.Rows(j).Item("BaseCode")) Then
                        '    Movimento_Dettaglio_Tecnico.BaseCode = Dt_MovimentiDettagliTecnici.Rows(j).Item("BaseCode")
                        'End If


                        'If Not IsNothing(Dt_MovimentiDettagliTecnici.Rows(j).Item("TopCode")) AndAlso
                        '    IsNumeric(Dt_MovimentiDettagliTecnici.Rows(j).Item("TopCode")) Then
                        '    Movimento_Dettaglio_Tecnico.TopCode = Dt_MovimentiDettagliTecnici.Rows(j).Item("TopCode")
                        'End If


                        If Not IsNothing(Dt_MovimentiDettagliTecnici.Rows(j).Item("Soglia_Cod")) AndAlso
                            IsNumeric(Dt_MovimentiDettagliTecnici.Rows(j).Item("Soglia_Cod")) Then
                            Movimento_Dettaglio_Tecnico.Soglia_Cod = Dt_MovimentiDettagliTecnici.Rows(j).Item("Soglia_Cod")
                        End If
                        If Not IsNothing(Dt_MovimentiDettagliTecnici.Rows(j).Item("Soglia_Quantita")) AndAlso
                            IsNumeric(Dt_MovimentiDettagliTecnici.Rows(j).Item("Soglia_Quantita")) Then
                            Movimento_Dettaglio_Tecnico.Soglia_Quantita = Dt_MovimentiDettagliTecnici.Rows(j).Item("Soglia_Quantita")
                        End If
                        If Not IsNothing(Dt_MovimentiDettagliTecnici.Rows(j).Item("Soglia_Des")) AndAlso
                            Not IsDBNull(Dt_MovimentiDettagliTecnici.Rows(j).Item("Soglia_Des")) Then
                            Movimento_Dettaglio_Tecnico.Soglia_Des = Dt_MovimentiDettagliTecnici.Rows(j).Item("Soglia_Des")
                        End If

                        If Not IsNothing(Dt_MovimentiDettagliTecnici.Rows(j).Item("Parziale")) Then
                            Movimento_Dettaglio_Tecnico.Parziale = Dt_MovimentiDettagliTecnici.Rows(j).Item("Parziale")
                        End If
                        If Not IsNothing(Dt_MovimentiDettagliTecnici.Rows(j).Item("Nitrati")) Then
                            Movimento_Dettaglio_Tecnico.Nitrati = Dt_MovimentiDettagliTecnici.Rows(j).Item("Nitrati")
                        End If

                        If Not IsNothing(Dt_MovimentiDettagliTecnici.Rows(j).Item("Piezo1")) Then
                            Movimento_Dettaglio_Tecnico.Piezo1 = Dt_MovimentiDettagliTecnici.Rows(j).Item("Piezo1")
                        End If

                        If Not IsNothing(Dt_MovimentiDettagliTecnici.Rows(j).Item("Piezo2")) Then
                            Movimento_Dettaglio_Tecnico.Piezo2 = Dt_MovimentiDettagliTecnici.Rows(j).Item("Piezo2")
                        End If

                        If Not IsNothing(Dt_MovimentiDettagliTecnici.Rows(j).Item("Piezo3")) Then
                            Movimento_Dettaglio_Tecnico.Piezo3 = Dt_MovimentiDettagliTecnici.Rows(j).Item("Piezo3")
                        End If

                        If Not IsNothing(Dt_MovimentiDettagliTecnici.Rows(j).Item("Piezo4")) Then
                            Movimento_Dettaglio_Tecnico.Piezo4 = Dt_MovimentiDettagliTecnici.Rows(j).Item("Piezo4")
                        End If

                        listaMovimentiDettagliTecnici.Add(Movimento_Dettaglio_Tecnico)

                    Next

                End If

            Catch ex As Exception

                Throw New Exception("[ Agenda_Movimenti_Dettagli_Tecnici_Helper.leggi() ] : " & ex.Message)

            Finally

                Utility.VerificaChiudiConnessione(objParametri, flagConnessione)
            End Try

            Return listaMovimentiDettagliTecnici

        End Function

    End Class

End Namespace
