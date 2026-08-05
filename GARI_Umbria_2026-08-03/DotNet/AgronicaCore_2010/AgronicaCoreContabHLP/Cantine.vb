Imports System.Data
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class Cantine

    ''########################################################################################
    'Public Function CaricaDT_xControlloCali() As DataTable

    '    Dim Dt As New DataTable

    '    '----- Definisco la struttura del DataTable

    '    Dt.Columns.Add(New DataColumn("id_agenda", GetType(Integer)))
    '    Dt.Columns.Add(New DataColumn("elem_cod", GetType(Integer)))
    '    Dt.Columns.Add(New DataColumn("mat_cod", GetType(Integer)))

    '    Return Dt

    'End Function

    ''########################################################################################
    'Public Sub Inserisci_Riga_Dt_xControlloCali(ByRef Dt As DataTable, _
    '                                            ByVal id_agenda As Integer, _
    '                                            ByVal elem_cod As Integer, _
    '                                            ByVal mat_cod As Integer)

    '    Dim Dr As DataRow

    '    Dr = Dt.NewRow

    '    Dr.Item("id_agenda") = id_agenda
    '    Dr.Item("elem_cod") = elem_cod
    '    Dr.Item("mat_cod") = mat_cod

    '    Dt.Rows.Add(Dr)

    'End Sub


    '####################################################
    'serve sia per vinificazione che per commercializzazione
    Public Function Verifica_CaloPerdita_Movimenti(ByVal Dt As DataTable, _
                                                    ByVal Id_Agenda As Integer, _
                                                    ByRef objHT_Cali As Hashtable, _
                                                    ByVal Cod_report As enum_AgroReportistica) As Boolean


        Dim AggiungiRiga As Boolean = True
        Dim Msgerrore As String = "Verifica_CaloPerdita_Movimenti: Operazione " & CStr(Id_Agenda) & " - nessuna riga"
        Dim mat_cod As Integer
        Dim Carico, Scarico, TotCarico, TotScarico As Double

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count > 0 Then

            Dim Dr() As DataRow

            'filtro per operazione
            Dr = Dt.Select(" Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda))

            If Not IsNothing(Dr) Then

                Select Case Dr.Length

                    Case 0
                        'non deve succedere, se sono qui c'è almeno la riga del calo
                        Throw New Exception(Msgerrore)

                    Case 1
                        'modifica del 31/04/2015:verifico il lav-cod,
                        'per le op di cantina faccio gli altri controlli
                        'se è di magazzino o contabile -> stampo
                        If CInt(Dr(0).Item("lav_cod")) = LAVCOD_TRASFORMAZIONI Then
                            'c'è solo il calo o solo l'altra materia prima, non devo aggiungerlo
                            AggiungiRiga = False

                            mat_cod = CInt(Dr(0).Item("mat_cod"))

                            If CInt(Dr(0).Item("udm_Cod")) = enum_UnitaMisura.Litri Then
                                Carico = CDbl(Dr(0).Item("CaricoLt"))
                                Scarico = CDbl(Dr(0).Item("ScaricoLt"))
                            Else
                                Select Case Cod_report
                                    Case enum_AgroReportistica.Vinificazione_DOC, _
                                             enum_AgroReportistica.Vinificazione_ViniTavola
                                        Carico = CDbl(Dr(0).Item("CaricoKg"))
                                        Scarico = CDbl(Dr(0).Item("ScaricoKg"))
                                    Case Else
                                        'registro commercializzazione
                                        Carico = CDbl(Dr(0).Item("CaricoLt"))
                                        Scarico = CDbl(Dr(0).Item("ScaricoLt"))
                                End Select
                            End If

                            If Not objHT_Cali.ContainsKey(mat_cod) Then
                                objHT_Cali.Add(mat_cod, CStr(Carico) & "|" & CStr(Scarico))
                            Else
                                TotCarico = CDbl(CStr(objHT_Cali(mat_cod)).Split({"|"c})(0))
                                TotScarico = CDbl(CStr(objHT_Cali(mat_cod)).Split({"|"c})(1))
                                TotCarico += Carico
                                TotScarico += Scarico
                                objHT_Cali(mat_cod) = CStr(TotCarico) & "|" & CStr(TotScarico)
                            End If

                        Else
                            'operazione di magazzino o contabile
                            'sicuramente un carico
                            AggiungiRiga = True
                        End If

                    Case Else
                        'ci sono altri dettagli

                        'modifica del 31/04/2015:verifico il lav-cod,
                        'per le op di cantina faccio gli altri controlli
                        'se è di magazzino o contabile -> stampo
                        If CInt(Dr(0).Item("lav_cod")) = LAVCOD_TRASFORMAZIONI Then

                            'modifica del 14/07/2015:
                            'verifico se ci sono solo cali e altre materia prima
                            AggiungiRiga = False

                            Dim i As Integer
                            For i = 0 To Dr.Length - 1
                                If CInt(Dr(i).Item("Elem_Cod")) <> ALTRE_MATERIE AndAlso
                                   CInt(Dr(i).Item("Elem_Cod")) <> CALI_LAVORAZIONE Then
                                    'oltre a cali e altre materie prime ci sono altre anagrafiche
                                    AggiungiRiga = True
                                    Exit For
                                End If
                            Next

                            If AggiungiRiga = False Then

                                mat_cod = CInt(Dr(0).Item("mat_cod"))

                                If CInt(Dr(0).Item("udm_Cod")) = enum_UnitaMisura.Litri Then
                                    Carico = CDbl(Dr(0).Item("CaricoLt"))
                                    Scarico = CDbl(Dr(0).Item("ScaricoLt"))
                                Else
                                    Select Case Cod_report
                                        Case enum_AgroReportistica.Vinificazione_DOC, _
                                                 enum_AgroReportistica.Vinificazione_ViniTavola
                                            Carico = CDbl(Dr(0).Item("CaricoKg"))
                                            Scarico = CDbl(Dr(0).Item("ScaricoKg"))
                                        Case Else
                                            'registro commercializzazione
                                            Carico = CDbl(Dr(0).Item("CaricoLt"))
                                            Scarico = CDbl(Dr(0).Item("ScaricoLt"))
                                    End Select
                                End If

                                If Not objHT_Cali.ContainsKey(mat_cod) Then
                                    objHT_Cali.Add(mat_cod, CStr(Carico) & "|" & CStr(Scarico))
                                Else
                                    TotCarico = CDbl(CStr(objHT_Cali(mat_cod)).Split({"|"c})(0))
                                    TotScarico = CDbl(CStr(objHT_Cali(mat_cod)).Split({"|"c})(1))
                                    TotCarico += Carico
                                    TotScarico += Scarico
                                    objHT_Cali(mat_cod) = CStr(TotCarico) & "|" & CStr(TotScarico)
                                End If
                            End If
                        Else
                            'operazione di magazzino o contabile
                            'sicuramente un carico
                            AggiungiRiga = True
                        End If 'lav_cod

                End Select

            End If
        Else
            Throw New Exception("Verifica_CaloPerdita: DT vuoto")
        End If

        Return AggiungiRiga

    End Function

    '####################################################
    'serve sia per vinificazione che per commercializzazione
    Public Sub Verifica_CaloPerdita_Riepilogo(ByVal mat_cod As Integer, _
                                                ByRef objHT_Cali As Hashtable, _
                                                ByRef CaricoLtDaNonContare As Double, _
                                                ByRef ScaricoLtDaNonContare As Double)

        ' Dim Msgerrore As String = "Verifica_CaloPerdita_Riepilogo: mat_cod " & CStr(mat_cod) & " - ht vuoto"

        If Not IsNothing(objHT_Cali) AndAlso Not IsNothing(objHT_Cali(mat_cod)) Then
            CaricoLtDaNonContare = CDbl(CStr(objHT_Cali(mat_cod)).Split({"|"c})(0)) * -1
            ScaricoLtDaNonContare = CDbl(CStr(objHT_Cali(mat_cod)).Split({"|"c})(1)) * -1
        End If

    End Sub

    '##############################################################
    Public Function Registro_from_ModelloLineaFrizSpum(ByVal Modello As String) As String

        Dim registro As String = ""

        Select Case Modello.ToUpper

            Case "JZ1", "JZ2", "JZ3", "JZ4", "JZ5", "JZ6", "JZ7", "JZ8", "JZ9", _
                    "JSP1", "JSP2", "JSP3", "JSP4", "JSP5", "JSP6", "JSP7", "JSP8"
                registro = "VINIF."

            Case "JZ1C", "JZ2C", "JZ3C", "JZ4C", "JZ5C", _
                "JSP1C", "JSP2C", "JSP3C", "JSP4C", "JSP5", "JSP6", "JSP7", "JSP8"
                registro = "COMM."

            Case Else
                registro = "n.d."

        End Select

        Return registro

    End Function


    '##############################################################
    'da usare quando si ha il dt restituito da AgronicaCoreAnagrafeDAL.Cantina_Vasche_R.Leggi()
    Public Function IdentificativoVasca_from_Codice(ByVal DT As DataTable, _
                                                    ByVal Sa_Cod As Integer, _
                                                    ByVal Id_Vasca As Integer) As String

        Dim Des As String = ""

        If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

            Dim Dr() As DataRow

            Dr = DT.Select(" Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " AND Vas_Cod = " & Agro_SQL_SaveNum(Id_Vasca))

            If Not IsNothing(Dr) AndAlso Dr.Length > 0 Then
                Des = CStr(Dr(0).Item("Identificativo"))
            End If

        End If

        Return Des

    End Function

    '##############################################################
    'da usare quando si ha il dt restituito da AgronicaCoreAnagrafeDAL.Cantina_Vasche_R.LeggiJoinUdm()
    Public Function IdentificativoCapacitaVasca_from_Codice(ByVal Flag_StampaNumeroVasca As Boolean, _
                                                            ByVal Flag_StampaCapacitaVasca As Boolean, _
                                                            ByVal DT As DataTable, _
                                                            ByVal Sa_Cod As Integer, _
                                                            ByVal Id_Vasca As Integer) As String

        Dim Des As String = ""

        If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

            Dim Dr() As DataRow

            Dr = DT.Select(" Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " AND Vas_Cod = " & Agro_SQL_SaveNum(Id_Vasca))

            If Not IsNothing(Dr) AndAlso Dr.Length > 0 Then

                Des &= " (Vasca "
                If Flag_StampaNumeroVasca Then
                    Des &= "n." & CStr(Dr(0).Item("Identificativo")) & " "
                End If
                If Flag_StampaCapacitaVasca Then
                    Des &= "Vol." & CStr(Dr(0).Item("Capacita_Nominale")) & " " & CStr(Dr(0).Item("Udm_sim"))
                End If
                Des &= ")"

            End If

        End If

        Return Des

    End Function

    '##############################################################
    'da usare quando si ha il dt restituito da AgronicaCoreAnagrafeDAL.Cantina_Vasche_R.LeggiVascaVinoSfusoMovimentato_byIdAgenda()
    Public Function IdentificativoCapacitaVasca_from_IdAgenda(ByVal Flag_StampaNumeroVasca As Boolean, _
                                                                ByVal Flag_StampaCapacitaVasca As Boolean, _
                                                                ByVal DT As DataTable, _
                                                                ByVal Id_Agenda As Integer) As String

        Dim Des As String = ""

        If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

            Dim Dr() As DataRow

            Dr = DT.Select(" Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda))

            If Not IsNothing(Dr) AndAlso Dr.Length > 0 Then

                Des &= " (Vasca "
                If Flag_StampaNumeroVasca Then
                    Des &= "n." & CStr(Dr(0).Item("Identificativo")) & " "
                End If
                If Flag_StampaCapacitaVasca Then
                    Des &= "Vol." & CStr(Dr(0).Item("Capacita_Nominale")) & " " & CStr(Dr(0).Item("Udm_sim"))
                End If
                Des &= ")"

            End If

        End If

        Return Des

    End Function

    '##############################################################
    'da usare quando si ha il dt restituito da AgronicaCoreAnagrafeDAL.Cantina_Vasche_R.LeggiVascaVinoSfusoMovimentato_byIdAgenda()
    ' a differenza di IdentificativoCapacitaVasca_from_IdAgenda
    ' non guarda i flag
    Public Function IdentificativoCapacitaVasca_from_IdAgenda_RegImbottigliamento( _
                                                ByVal Flag_StampaNumeroVasca As Boolean, _
                                                ByVal Flag_StampaCapacitaVasca As Boolean, _
                                                ByVal DT As DataTable, _
                                                ByVal Id_Agenda As Integer) As String

        Dim Des As String = ""

        If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

            Dim Dr() As DataRow

            Dr = DT.Select(" Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda))

            If Not IsNothing(Dr) AndAlso Dr.Length > 0 Then

                Des &= " (Vasca "
                'If Flag_StampaNumeroVasca = True Then
                Des &= "n." & CStr(Dr(0).Item("Identificativo")) & " "
                'End If
                'If Flag_StampaCapacitaVasca = True Then
                Des &= "Vol." & CStr(Dr(0).Item("Capacita_Nominale")) & " " & CStr(Dr(0).Item("Udm_sim"))
                'End If
                Des &= ")"

            End If

        End If

        Return Des

    End Function


    '##############################################################
    'da usare quando si ha il dt restituito da AgronicaCoreAnagrafeDAL.Lotto_Configurazione_Alias_R.Leggi()
    Public Function LottoAlias_from_LottoVal(ByVal DT As DataTable, _
                                              ByVal Piva As String, _
                                            ByVal Elem_Cod As Int32, _
                                            ByVal Lotto_Cod As Int32, _
                                            ByVal Lotto_Val As String) As String

        Dim Des As String = ""

        If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

            Dim Dr() As DataRow

            Dr = DT.Select(" Piva = '" & Agro_SQL_SaveText(Piva) & "'" & _
                            " AND Lotto_Cod = " & Agro_SQL_SaveNum(Lotto_Cod) & _
                            " AND Lotto_Val = '" & Agro_SQL_SaveNum(Lotto_Val) & "'")

            If Not IsNothing(Dr) AndAlso Dr.Length > 0 Then
                Des = CStr(Dr(0).Item("Lotto_Alias"))
            End If

        End If

        Return Des

    End Function


    '##############################################################
    'da usare quando si ha il dt restituito da AgronicaCoreContabDAL.MovimentixReport_R.LeggiJOINLineePreparazionixReport()
    Public Function DescMovXReport_from_IdAgenda(ByVal DT As DataTable, _
                                                 ByVal Id_Agenda As Integer) As String

        Dim Des As String = ""

        If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

            Dim Dr() As DataRow

            Dr = DT.Select(" Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda))

            If Not IsNothing(Dr) AndAlso Dr.Length > 0 Then
                Des = CStr(Dr(0).Item("Descrizione"))
            End If

        End If

        Return Des

    End Function


End Class
