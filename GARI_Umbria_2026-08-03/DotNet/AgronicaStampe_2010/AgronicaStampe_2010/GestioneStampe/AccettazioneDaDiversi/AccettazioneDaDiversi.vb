Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Agro_Math

Public Module AccettazioneDaDiversi_funzioniXStampa

    'LEGGIMI!!!!
    'tutte le funzioni sono state spostate nel core AgronicaCoreStampeDAL.AccettazioneDaDiversi_Funzioni.vb
    'qui rimangono solo le funzioni specifiche per i report

    '##################################################################################
    Public Sub Carica_DSImballaggi_StampaBollaECertificato(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                                            ByRef Log_Errori As String, _
                                                            ByRef DSRiepilogoImballiEntrata As DS_Iva, _
                                                            ByRef DSImballiUscita As DS_ImballiUscita, _
                                                            ByRef Num_Colli As Integer, _
                                                            ByRef Tara_Imballi As Decimal, _
                                                            ByVal Flag_ImballiEntrata As Boolean, _
                                                            ByVal Flag_ImballiUscita As Boolean, _
                                                            ByVal Piva As String, _
                                                            ByVal Id_Agenda As Integer, _
                                                            ByVal Lav_Cod As Integer)

        '====================================================================================
        'RIEPILOGO IMBALLAGGI

        Dim ADD As New AgronicaCoreStampeDAL.AccettazioneDaDiversi

        '----------------------
        'IMBALLAGGI IN ENTRATA
        '----------------------

        If Flag_ImballiEntrata = True Then

            Dim Dt_ImballiEntrata As DataTable

            Try

                Dt_ImballiEntrata = ADD.ImballiEntrata_Leggi(Piva, _
                                                            Id_Agenda, _
                                                            False, _
                                                            False, _
                                                             "", "", _
                                                             objParametri_Server)

            Catch ex As Exception
                Log_Errori += "- Lettura degli imballaggi in entrata: " + vbCrLf + ex.Message + vbCrLf
            End Try



            Try

                If Not IsNothing(Dt_ImballiEntrata) AndAlso Dt_ImballiEntrata.Rows.Count <> 0 Then

                    Dim DR_ImballoEntrata As DS_Iva.DS_IvaRow
                    Dim Mat_Des_Imballo As String
                    Dim Num_Imballo As Integer
                    Dim Qta_Extra_Imballo As Decimal
                    Dim Qta_Extra_Totale_Imballo As Decimal
                    'Dim Peso_Imballaggi As Double = 0
                    Dim i As Integer

                    Num_Colli = 0
                    Tara_Imballi = 0

                    For i = 0 To Dt_ImballiEntrata.Rows.Count - 1

                        Mat_Des_Imballo = Dt_ImballiEntrata.Rows(i).Item("Mat_Des_ImballiEntrata")
                        Num_Imballo = Dt_ImballiEntrata.Rows(i).Item("Qta_ImballiEntrata")
                        Qta_Extra_Imballo = Dt_ImballiEntrata.Rows(i).Item("Qta_Extra_ImballiEntrata")
                        Qta_Extra_Totale_Imballo = Dt_ImballiEntrata.Rows(i).Item("Qta_Extra_Totale_ImballiEntrata")

                        Num_Colli += Num_Imballo
                        Tara_Imballi += Qta_Extra_Totale_Imballo

                        'Peso_Imballaggi += Qta_Extra_Imballo

                        '===================================================================

                        DR_ImballoEntrata = DSRiepilogoImballiEntrata.DS_Iva.NewDS_IvaRow

                        DR_ImballoEntrata.Imponibile2 = Mat_Des_Imballo
                        DR_ImballoEntrata.Cod_Iva_2 = CStr(Num_Imballo)
                        DR_ImballoEntrata.Imposta2 = CStr(Qta_Extra_Imballo)

                        DSRiepilogoImballiEntrata.DS_Iva.Rows.Add(DR_ImballoEntrata)

                        '===================================================================

                    Next 'dettagli

                End If

            Catch ex As Exception
                Log_Errori += "- Riepilogo Imballaggi in Entrata: " + vbCrLf + ex.Message + vbCrLf
            End Try

        End If 'Flag_ImballiEntrata


        '----------------------
        'IMBALLAGGI IN USCITA
        '----------------------

        If Flag_ImballiUscita = True Then

            Dim Dt_ImballiUscita As DataTable

            Try

                'Dt_ImballiUscita = NewCom_AccettazioneBeniDaDiversi_RifBollaImballiUscita_Leggi(objParametri_Server, _
                '                                                                                Piva, _
                '                                                                                0, _
                '                                                                                Id_Agenda, _
                '                                                                                Lav_Cod, _
                '                                                                                True, _
                '                                                                                , , , )

                Dt_ImballiUscita = ADD.ImballiUscita_Leggi(Piva,
                                                            0,
                                                            Id_Agenda,
                                                            Lav_Cod,
                                                            False,
                                                            "", "",
                                                            objParametri_Server)

            Catch ex As Exception
                Log_Errori += "- Lettura degli imballaggi in uscita: " + vbCrLf + ex.Message + vbCrLf
            End Try

            Try

                If Not IsNothing(Dt_ImballiUscita) AndAlso Dt_ImballiUscita.Rows.Count <> 0 Then

                    Dim i As Integer
                    Dim DR_ImballoUscita As DS_ImballiUscita.DS_ImballiUscitaRow
                    Dim Mat_Des_Imballo As String
                    Dim Num_Imballo As Integer
                    Dim Rif_DDTImballo As String
                    Dim Doc_Numero_Sin_Imballo As String
                    Dim Doc_Numero_Imballo As Integer
                    Dim Doc_Numero_Des_Imballo As String
                    Dim Data_Imballo As String
                    'Dim Lunghezza_Sin_NumDDT As Integer
                    'Dim Lunghezza_Centro_NumDDT As Integer
                    'Dim Lunghezza_Des_NumDDT As Integer
                    'Dim Carattere_Format_NumDDT As String

                    For i = 0 To Dt_ImballiUscita.Rows.Count - 1

                        Mat_Des_Imballo = Dt_ImballiUscita.Rows(i).Item("Mat_Des")
                        Num_Imballo = Dt_ImballiUscita.Rows(i).Item("Qta")

                        Doc_Numero_Sin_Imballo = Dt_ImballiUscita.Rows(i).Item("Doc_Numero_Sin")
                        Doc_Numero_Imballo = Dt_ImballiUscita.Rows(i).Item("Doc_Numero")
                        Doc_Numero_Des_Imballo = Dt_ImballiUscita.Rows(i).Item("Doc_Numero_Des")
                        Data_Imballo = CDate(Dt_ImballiUscita.Rows(i).Item("Data_Movimento")).ToShortDateString
                        'Lunghezza_Sin_NumDDT = Dt_ImballiUscita.Rows(i).Item("Lunghezza_Sin")
                        'Lunghezza_Centro_NumDDT = Dt_ImballiUscita.Rows(i).Item("Lunghezza_Centro")
                        'Lunghezza_Des_NumDDT = Dt_ImballiUscita.Rows(i).Item("Lunghezza_Des")
                        'Carattere_Format_NumDDT = Dt_ImballiUscita.Rows(i).Item("CarattereFormattazione")

                        'Rif_DDTImballo = Ricava_NumeroDocumento_Con_Sequenza( _
                        '                            Doc_Numero_Sin_Imballo, _
                        '                            CStr(Doc_Numero_Imballo), _
                        '                            Doc_Numero_Des_Imballo, _
                        '                            Lunghezza_Sin_NumDDT, _
                        '                            Lunghezza_Centro_NumDDT, _
                        '                            Lunghezza_Des_NumDDT, _
                        '                            Carattere_Format_NumDDT) + " del " + Data_Imballo

                        Rif_DDTImballo = Ricava_NumeroDocumento_Senza_Sequenza(
                                                    Doc_Numero_Sin_Imballo,
                                                    CStr(Doc_Numero_Imballo),
                                                    Doc_Numero_Des_Imballo) + " del " + Data_Imballo

                        '===================================================================

                        DR_ImballoUscita = DSImballiUscita.DS_ImballiUscita.NewDS_ImballiUscitaRow

                        DR_ImballoUscita.MatDes_Imballo = Mat_Des_Imballo
                        DR_ImballoUscita.Num_Imballo = CStr(Num_Imballo)
                        DR_ImballoUscita.RifDDT_Imballo = CStr(Rif_DDTImballo)

                        DSImballiUscita.DS_ImballiUscita.Rows.Add(DR_ImballoUscita)

                        '===================================================================

                    Next 'dettagli

                End If

            Catch ex As Exception
                Log_Errori += "- Riepilogo Imballaggi Uscita: " + vbCrLf + ex.Message + vbCrLf
            End Try

        End If 'Flag_ImballiUscita



    End Sub


    ''##################################################################################
    ''Tipo_Report serve per indicare se si tratta di bolle di accettazione o di certificati
    ''----------
    ''Viene utilizzata da Export_BolleAccettazione2AltroCliente (export Fruttagel -> Terremerse)
    ''e dalla pagina di filtro (Coltrolla_RecuperaFiltri e StampaMassivaBolle)
    'Public Function Verifica_Filtro_Bolle(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, _
    '                                        ByRef objServer As System.Web.HttpServerUtility, _
    '                                        ByRef objSession As System.Web.SessionState.HttpSessionState, _
    '                                        ByRef objPage As System.Web.UI.Page, _
    '                                        ByRef Messaggio As String, _
    '                                        ByVal Flag_FiltroSQLIdAgenda As Boolean, _
    '                                        ByVal Piva As String, _
    '                                        ByVal Tipo_Report As Integer, _
    '                                        ByVal PrefissoDaNumeroBolla As String, _
    '                                        ByVal DaNumeroBolla As String, _
    '                                        ByVal SuffissoDaNumeroBolla As String, _
    '                                        ByVal PrefissoANumeroBolla As String, _
    '                                        ByVal ANumeroBolla As String, _
    '                                        ByVal SuffissoANumeroBolla As String, _
    '                                        ByVal Validita_Inizio As String, _
    '                                        ByVal Validita_Fine As String, _
    '                                        ByVal Codice_Specie As Integer, _
    '                                        ByVal Str_Codici_Specie As String, _
    '                                        ByVal Codice_Conferente As Integer, _
    '                                        ByVal Str_Codici_Conferenti As String, _
    '                                        ByVal Piva_Produttore As String, _
    '                                        ByVal Piva_Coop1 As String, _
    '                                        ByVal Piva_Coop2 As String) As String


    '    Dim SringaFiltroBolle As String = ""
    '    Dim label, label2 As String

    '    Select Case Tipo_Report

    '        Case enum_CodificaStampe.Buono_Accettazione_Diversi
    '            label = "Bolla"
    '            label2 = "delle Bolle"

    '        Case enum_CodificaStampe.Certificato_Pomodoro
    '            'label = "Certificato"
    '            'label = "dei Certificati"
    '            label = "Bolla"
    '            label2 = "delle Bolle"

    '    End Select

    '    Try

    '        If DaNumeroBolla <> "" And ANumeroBolla <> "" Then

    '            If Not IsNumeric(DaNumeroBolla) Or Not IsNumeric(ANumeroBolla) Then
    '                Messaggio = "I numeri " + label + " devono essere numerici."
    '                Exit Function
    '            End If

    '            ''se sono uguali, si filtra solo una specie
    '            'If DaNumeroBolla <> ANumeroBolla Then

    '            If CInt(ANumeroBolla) < CInt(DaNumeroBolla) Then
    '                Messaggio = "Il Numero " + label + " A deve essere maggiore rispetto al Numero " + label + " DA"
    '                Exit Function
    '            End If

    '            Prepara_StrFiltroBolle(objParametri_Server, _
    '                                    objServer, objSession, objPage, _
    '                                    SringaFiltroBolle, _
    '                                    Messaggio, _
    '                                    Flag_FiltroSQLIdAgenda, _
    '                                    Piva, _
    '                                    Tipo_Report, _
    '                                    True, _
    '                                    PrefissoDaNumeroBolla, DaNumeroBolla, SuffissoDaNumeroBolla, _
    '                                    PrefissoANumeroBolla, ANumeroBolla, SuffissoANumeroBolla, _
    '                                    Validita_Inizio, Validita_Fine, _
    '                                    Codice_Specie, _
    '                                    Str_Codici_Specie, _
    '                                    Codice_Conferente, _
    '                                    Str_Codici_Conferenti, _
    '                                    Piva_Produttore, _
    '                                    Piva_Coop1, _
    '                                    Piva_Coop2)


    '            'Else
    '            '    'se sono uguali, si filtra solo una specie
    '            '    Me.Txt_SringaFiltroBolle.Text = Str_NumBolla
    '            'End If

    '        Else
    '            If Validita_Inizio <> "" And Validita_Fine <> "" Then
    '                'ok, non si filtra per numero, ma solo per date
    '                Prepara_StrFiltroBolle(objParametri_Server, _
    '                                        objServer, objSession, objPage, _
    '                                        SringaFiltroBolle, _
    '                                        Messaggio, _
    '                                        Flag_FiltroSQLIdAgenda, _
    '                                        Piva, _
    '                                        Tipo_Report, _
    '                                        False, _
    '                                        PrefissoDaNumeroBolla, DaNumeroBolla, SuffissoDaNumeroBolla, _
    '                                        PrefissoANumeroBolla, ANumeroBolla, SuffissoANumeroBolla, _
    '                                        Validita_Inizio, Validita_Fine, _
    '                                        Codice_Specie, _
    '                                        Str_Codici_Specie, _
    '                                        Codice_Conferente, _
    '                                        Str_Codici_Conferenti, _
    '                                        Piva_Produttore, _
    '                                        Piva_Coop1, _
    '                                        Piva_Coop2)

    '            Else
    '                'è necessario impostare almeno un tipo di filtro
    '                Messaggio = "E' necessario impostare almeno un criterio di filtro di stampa " + label2 + " (per intervallo temporale o per numero " + label + ")."
    '            End If
    '        End If

    '    Catch ex As Exception
    '        Messaggio += "Si è verificato il seguente errore: " + ex.Message
    '    End Try

    '    Return SringaFiltroBolle


    'End Function


    ''##################################################################################
    'Private Sub Prepara_StrFiltroBolle(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, _
    '                                    ByRef objServer As System.Web.HttpServerUtility, _
    '                                    ByRef objSession As System.Web.SessionState.HttpSessionState, _
    '                                    ByRef objPage As System.Web.UI.Page, _
    '                                    ByRef SringaFiltroBolle As String, _
    '                                    ByRef Messaggio As String, _
    '                                    ByVal Flag_FiltroSQLIdAgenda As Boolean, _
    '                                    ByVal Piva As String, _
    '                                    ByVal Tipo_Report As Integer, _
    '                                    ByVal Flag_FiltraNumBolla As Boolean, _
    '                                    ByVal PrefissoDaNumeroBolla As String, _
    '                                    ByVal DaNumeroBolla As String, _
    '                                    ByVal SuffissoDaNumeroBolla As String, _
    '                                    ByVal PrefissoANumeroBolla As String, _
    '                                    ByVal ANumeroBolla As String, _
    '                                    ByVal SuffissoANumeroBolla As String, _
    '                                    ByVal Validita_Inizio As String, _
    '                                    ByVal Validita_Fine As String, _
    '                                    ByVal Codice_Specie As Integer, _
    '                                    ByVal Str_Codici_Specie As String, _
    '                                    ByVal Codice_Conferente As Integer, _
    '                                    ByVal Str_Codici_Conferenti As String, _
    '                                    ByVal Piva_Produttore As String, _
    '                                    ByVal Piva_Coop1 As String, _
    '                                    ByVal Piva_Coop2 As String)


    '    'compongo la stringa dei progressivi da filtrare
    '    Dim DT_NumBolla As DataTable
    '    Dim i As Integer
    '    Dim Str_NumBolla As String = ""

    '    Dim label As String

    '    Select Case Tipo_Report
    '        Case enum_CodificaStampe.Buono_Accettazione_Diversi
    '            label = "Non è stata trovata alcuna Bolla"
    '        Case enum_CodificaStampe.Certificato_Pomodoro
    '            label = "Non è stato trovato alcun Certificato"
    '    End Select

    '    'modifico la data
    '    If Validita_Inizio = "" Or Validita_Inizio = Nothing Then
    '        Validita_Inizio = AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO.ToShortDateString
    '    End If

    '    If Validita_Fine = "" Or Validita_Fine = Nothing Then
    '        Validita_Fine = AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAFINE.ToShortDateString

    '    End If


    '    DT_NumBolla = Leggi_Range_IdAgenda_Bolle(objParametri_Server, _
    '                                        objSession, _
    '                                        Piva, _
    '                                        Tipo_Report, _
    '                                        Flag_FiltraNumBolla, _
    '                                        PrefissoDaNumeroBolla, _
    '                                        DaNumeroBolla, _
    '                                        SuffissoDaNumeroBolla, _
    '                                        PrefissoANumeroBolla, _
    '                                        ANumeroBolla, _
    '                                        SuffissoANumeroBolla, _
    '                                        Validita_Inizio, _
    '                                        Validita_Fine, _
    '                                        Codice_Specie, _
    '                                        Str_Codici_Specie, _
    '                                        Codice_Conferente, _
    '                                        Str_Codici_Conferenti, _
    '                                        Piva_Produttore, _
    '                                        Piva_Coop1, _
    '                                        Piva_Coop2)


    '    If Not IsNothing(DT_NumBolla) Then

    '        If DT_NumBolla.Rows.Count <> 0 Then

    '            For i = 0 To DT_NumBolla.Rows.Count - 1

    '                If Flag_FiltroSQLIdAgenda = True Then
    '                    If i <> DT_NumBolla.Rows.Count - 1 Then
    '                        Str_NumBolla += CStr(DT_NumBolla.Rows(i).Item("Id_Agenda")) + ","
    '                    Else
    '                        Str_NumBolla += CStr(DT_NumBolla.Rows(i).Item("Id_Agenda"))
    '                    End If
    '                Else
    '                    If i <> DT_NumBolla.Rows.Count - 1 Then
    '                        Str_NumBolla += CStr(DT_NumBolla.Rows(i).Item("Id_Agenda")) + "|"
    '                    Else
    '                        Str_NumBolla += CStr(DT_NumBolla.Rows(i).Item("Id_Agenda"))
    '                    End If
    '                End If

    '            Next

    '            SringaFiltroBolle = Str_NumBolla

    '        Else
    '            Messaggio = label + " che soddisfi il criterio di filtro impostato."
    '        End If

    '    End If

    'End Sub


    ''##################################################################################
    'Private Function Leggi_Range_IdAgenda_Bolle(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, _
    '                                            ByRef objSession As System.Web.SessionState.HttpSessionState, _
    '                                            ByVal Piva As String, _
    '                                            ByVal Tipo_Report As Integer, _
    '                                            ByVal Flag_FiltraNumBolla As Boolean, _
    '                                            ByVal Prefisso_Num_Bolla_DA As String, _
    '                                            ByVal Num_Bolla_DA As String, _
    '                                            ByVal Suffisso_Num_Bolla_DA As String, _
    '                                            ByVal Prefisso_Num_Bolla_A As String, _
    '                                            ByVal Num_Bolla_A As String, _
    '                                            ByVal Suffisso_Num_Bolla_A As String, _
    '                                            ByVal Validita_Inizio As String, _
    '                                            ByVal Validita_Fine As String, _
    '                                            ByVal Codice_Specie As String, _
    '                                            ByVal Str_Codici_Specie As String, _
    '                                            ByVal Codice_Conferente As String, _
    '                                            ByVal Str_Codici_Conferenti As String, _
    '                                            ByVal Piva_Produttore As String, _
    '                                            ByVal Piva_Coop1 As String, _
    '                                            ByVal Piva_Coop2 As String) As DataTable


    '    Dim DT_NumBolla As DataTable
    '    Dim FiltroAggiuntivo As String = ""
    '    Dim Str_FiltroConf As String = ""
    '    Dim Str_FiltroSpecie As String = ""
    '    Dim Ordinamento As String


    '    If Flag_FiltraNumBolla = True Then

    '        Dim j As Integer
    '        Dim ElencoNumeriBolla As String = ""
    '        Dim ChiaveBolla As String = ""

    '        For j = CInt(Num_Bolla_DA) To CInt(Num_Bolla_A)

    '            ChiaveBolla = Prefisso_Num_Bolla_DA & "_" & CStr(j) & "_" & Suffisso_Num_Bolla_DA

    '            ElencoNumeriBolla += ",'" & ChiaveBolla & "'"

    '        Next

    '        'Formatto correttamente l'elenco (tolgo la virgola iniziale ...)
    '        ElencoNumeriBolla = Mid(ElencoNumeriBolla, 2)

    '        Select Case Tipo_Report

    '            Case enum_CodificaStampe.Buono_Accettazione_Diversi
    '                'FiltroAggiuntivo += " AND               ( ( Mov_Accett.Doc_Numero_Sin  "
    '                'per il core non serve più l'and
    '                FiltroAggiuntivo += "                ( ( Mov_Accett.Doc_Numero_Sin  "
    '                FiltroAggiuntivo += "                     + '_' + CONVERT(varchar(10), Mov_Accett.Doc_Numero)  "
    '                FiltroAggiuntivo += "                     + '_' + Mov_Accett.Doc_Numero_Des )  "
    '                FiltroAggiuntivo += "                     IN (" & ElencoNumeriBolla & ")    )  "

    '            Case enum_CodificaStampe.Certificato_Pomodoro
    '                'MODIFICA IN DATA 26/07/2010:
    '                'si filtra per numero bolla, ma la query deve essere quella del certificato
    '                'FiltroAggiuntivo += " AND               ( ( Mov_Certificato.Doc_Numero_Sin  "
    '                'FiltroAggiuntivo += "                     + '_' + CONVERT(varchar(10), Mov_Certificato.Doc_Numero)  "
    '                'FiltroAggiuntivo += "                     + '_' + Mov_Certificato.Doc_Numero_Des )  "
    '                'FiltroAggiuntivo += "                     IN (" & ElencoNumeriBolla & ")    )  "

    '                'per il core non si usa più l'and
    '                'FiltroAggiuntivo += " AND               ( ( Mov_Accett.Doc_Numero_Sin  "
    '                FiltroAggiuntivo += "                ( ( Mov_Accett.Doc_Numero_Sin  "
    '                FiltroAggiuntivo += "                     + '_' + CONVERT(varchar(10), Mov_Accett.Doc_Numero)  "
    '                FiltroAggiuntivo += "                     + '_' + Mov_Accett.Doc_Numero_Des )  "
    '                FiltroAggiuntivo += "                     IN (" & ElencoNumeriBolla & ")    )  "

    '        End Select



    '    End If


    '    If Codice_Specie = "0" Then
    '        Codice_Specie = ""
    '        'il codice non è stato passato, 
    '        'perchè si vogliono cercare tutte le specie
    '        'o il range di specie selezionato
    '    End If

    '    If Str_Codici_Specie <> "" Then
    '        'è stato selezionato un range di codici
    '        Str_FiltroSpecie = " AND MP_Raccolta.Cod_Articolo IN " & Str_Codici_Specie
    '    Else
    '        Str_FiltroSpecie = ""
    '    End If

    '    If Codice_Conferente = "0" Then
    '        Codice_Conferente = ""
    '        'il codice non è stato passato, 
    '        'perchè si vogliono cercare tutti i conferenti
    '        'o il range di conferenti selezionato
    '    Else
    '        Codice_Conferente = CStr(Codice_Conferente)
    '    End If

    '    If Str_Codici_Conferenti <> "" Then
    '        'è stato selezionato un range di codici
    '        Str_FiltroConf = " AND Risorse_Umane_Conferenti.settore_Des IN " & Str_Codici_Conferenti
    '    Else
    '        Str_FiltroConf = ""
    '    End If

    '    Dim ADD As New AgronicaCoreStampeDAL.AccettazioneDaDiversi


    '    Select Case Tipo_Report

    '        Case enum_CodificaStampe.Buono_Accettazione_Diversi

    '            'x il core
    '            'Ordinamento = " ORDER BY Mov_Accett.Doc_Numero, Mov_Accett.Doc_Numero_Sin, Mov_Accett.Doc_Numero_Des "
    '            Ordinamento = " Mov_Accett.Doc_Numero, Mov_Accett.Doc_Numero_Sin, Mov_Accett.Doc_Numero_Des "


    '            'DT_NumBolla = NewCom_ADD_IdAgenda_DocNumero_Leggi(objServer, objSession, objPage, _
    '            '                                                    Piva, _
    '            '                                                    0, _
    '            '                                                    True, _
    '            '                                                    Prefisso_Num_Bolla_DA, _
    '            '                                                    FiltroAggiuntivo, _
    '            '                                                    Ordinamento, _
    '            '                                                    Validita_Inizio, _
    '            '                                                    Validita_Fine, _
    '            '                                                    Codice_Specie, _
    '            '                                                    Str_FiltroSpecie, _
    '            '                                                    Codice_Conferente, _
    '            '                                                    Str_FiltroConf, _
    '            '                                                    Piva_Produttore)

    '            DT_NumBolla = ADD.Bolle_IdAgenda_DocNumero_Leggi(Piva, _
    '                                                        0, _
    '                                                        True, _
    '                                                        Prefisso_Num_Bolla_DA, _
    '                                                        Validita_Inizio, _
    '                                                        Validita_Fine, _
    '                                                        Codice_Specie, _
    '                                                        Str_FiltroSpecie, _
    '                                                        Codice_Conferente, _
    '                                                        Str_FiltroConf, _
    '                                                        Piva_Produttore, _
    '                                                        Piva_Coop1, _
    '                                                        Piva_Coop2, _
    '                                                        0, _
    '                                                        FiltroAggiuntivo, _
    '                                                        Ordinamento, _
    '                                                        objParametri_Server)

    '        Case enum_CodificaStampe.Certificato_Pomodoro

    '            ''Ordinamento = " ORDER BY Mov_Certificato.Doc_Numero, Mov_Certificato.Doc_Numero_Sin, Mov_Certificato.Doc_Numero_Des "
    '            'Ordinamento = " ORDER BY Mov_Accett.Doc_Numero, Mov_Accett.Doc_Numero_Sin, Mov_Accett.Doc_Numero_Des "
    '            'per,il core
    '            Ordinamento = " Mov_Accett.Doc_Numero, Mov_Accett.Doc_Numero_Sin, Mov_Accett.Doc_Numero_Des "

    '            Dim Tipo_Certificato As enum_CodificaStampe

    '            If IsNumeric(objSession("ReportSelezionato")) Then
    '                Tipo_Certificato = objSession("ReportSelezionato")
    '            Else
    '                'non è valorizzato il report nella sessione
    '                'imposto come default il cert. interno
    '                Tipo_Certificato = enum_CodificaStampe.Certificato_Pomodoro_Interno
    '            End If

    '            'DT_NumBolla = NewCom_ADD_CertificatiPomodoro_IdAgenda_Leggi(objServer, objSession, objPage, _
    '            '                                                            Piva, _
    '            '                                                            Tipo_Certificato, _
    '            '                                                            FiltroAggiuntivo, _
    '            '                                                            Ordinamento, _
    '            '                                                            Validita_Inizio, _
    '            '                                                            Validita_Fine, _
    '            '                                                            Codice_Specie, _
    '            '                                                            Str_FiltroSpecie, _
    '            '                                                            Codice_Conferente, _
    '            '                                                            Str_FiltroConf, _
    '            '                                                            Piva_Produttore)

    '            DT_NumBolla = ADD.CertificatiPomodoro_IdAgenda_Leggi(Piva, _
    '                                                                    Tipo_Certificato, _
    '                                                                    Validita_Inizio, _
    '                                                                    Validita_Fine, _
    '                                                                    Codice_Specie, _
    '                                                                    Str_FiltroSpecie, _
    '                                                                    Codice_Conferente, _
    '                                                                    Str_FiltroConf, _
    '                                                                    Piva_Produttore, _
    '                                                                    Piva_Coop1, _
    '                                                                    Piva_Coop2, _
    '                                                                    FiltroAggiuntivo, _
    '                                                                    Ordinamento, _
    '                                                                    objParametri_Server)


    '    End Select


    '    Return DT_NumBolla


    'End Function


    ''##################################################################################
    'Public Sub Leggi_MinMaxNumeroBolla_byFiltro(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, _
    '                                                ByRef Doc_Numero_Max As Double, _
    '                                                ByRef Doc_Numero_Min As Double, _
    '                                                ByVal Piva As String, _
    '                                                ByVal Prefisso_Num_Bolla_DA As String, _
    '                                                ByVal Validita_Inizio As String, _
    '                                                ByVal Validita_Fine As String, _
    '                                                ByVal Codice_Specie As String, _
    '                                                ByVal Str_Codici_Specie As String, _
    '                                                ByVal Codice_Conferente As String, _
    '                                                ByVal Str_Codici_Conferenti As String, _
    '                                                ByVal Piva_Produttore As String)


    '    Dim DT_NumBolla As DataTable
    '    Dim Str_FiltroConf As String = ""
    '    Dim Str_FiltroSpecie As String = ""

    '    If Codice_Specie = "0" Then
    '        Codice_Specie = ""
    '        'il codice non è stato passato, 
    '        'perchè si vogliono cercare tutte le specie
    '        'o il range di specie selezionato
    '    End If

    '    If Str_Codici_Specie <> "" Then
    '        'è stato selezionato un range di codici
    '        Str_FiltroSpecie = " AND MP_Raccolta.Cod_Articolo IN " & Str_Codici_Specie
    '    Else
    '        Str_FiltroSpecie = ""
    '    End If

    '    If Codice_Conferente = "0" Then
    '        Codice_Conferente = ""
    '        'il codice non è stato passato, 
    '        'perchè si vogliono cercare tutti i conferenti
    '        'o il range di conferenti selezionato
    '    Else
    '        Codice_Conferente = CStr(Codice_Conferente)
    '    End If

    '    If Str_Codici_Conferenti <> "" Then
    '        'è stato selezionato un range di codici
    '        Str_FiltroConf = " AND Risorse_Umane_Conferenti.settore_Des IN " & Str_Codici_Conferenti
    '    Else
    '        Str_FiltroConf = ""
    '    End If

    '    'ATTENZIONE! Passare TipoSelect=1 e Flag_FiltraPomodoro= false
    '    'DT_NumBolla = NewCom_ADD_IdAgenda_DocNumero_Leggi(objServer, objSession, objPage, _
    '    '                                                    Piva, _
    '    '                                                    0, _
    '    '                                                    False, _
    '    '                                                    Prefisso_Num_Bolla_DA, _
    '    '                                                    "", _
    '    '                                                    "", _
    '    '                                                    Validita_Inizio, _
    '    '                                                    Validita_Fine, _
    '    '                                                    Codice_Specie, _
    '    '                                                    Str_FiltroSpecie, _
    '    '                                                    Codice_Conferente, _
    '    '                                                    Str_FiltroConf, _
    '    '                                                    Piva_Produttore, _
    '    '                                                    1)

    '    Dim ADD As New AgronicaCoreStampeDAL.AccettazioneDaDiversi

    '    DT_NumBolla = ADD.Bolle_IdAgenda_DocNumero_Leggi(Piva, _
    '                                                        0, _
    '                                                        False, _
    '                                                        Prefisso_Num_Bolla_DA, _
    '                                                        Validita_Inizio, _
    '                                                        Validita_Fine, _
    '                                                        Codice_Specie, _
    '                                                        Str_FiltroSpecie, _
    '                                                        Codice_Conferente, _
    '                                                        Str_FiltroConf, _
    '                                                        Piva_Produttore, _
    '                                                        "", "", _
    '                                                        1, _
    '                                                        "", "", _
    '                                                        objParametri_Server)


    '    If Not IsNothing(DT_NumBolla) AndAlso DT_NumBolla.Rows.Count > 0 Then
    '        Doc_Numero_Max = DT_NumBolla.Rows(0).Item("Doc_Numero_Max")
    '        Doc_Numero_Min = DT_NumBolla.Rows(0).Item("Doc_Numero_Min")
    '    End If


    'End Sub


    ''##################################################################################
    'Public Sub NumeroBolla_from_IdAgenda(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, _
    '                                            ByRef Doc_Numero_Sin As String, _
    '                                            ByRef Doc_Numero As Double, _
    '                                            ByRef Doc_Numero_Des As String, _
    '                                            ByVal Piva As String, _
    '                                            ByVal Id_Agenda As Integer)

    '    Dim DT_NumBolla As DataTable
    '    Dim FiltroAggiuntivo As String = ""
    '    Dim Ordinamento As String = ""

    '    Dim ADD As New AgronicaCoreStampeDAL.AccettazioneDaDiversi

    '    DT_NumBolla = ADD.Bolle_IdAgenda_DocNumero_Leggi(Piva, _
    '                                                        Id_Agenda, _
    '                                                        False, _
    '                                                        "-999", _
    '                                                        AGRODATAINIZIO, _
    '                                                        AGRODATAFINE, _
    '                                                        "", "", "", "", "", "", "", _
    '                                                        0, _
    '                                                        FiltroAggiuntivo, _
    '                                                        Ordinamento, _
    '                                                        objParametri_Server)


    '    'DT_NumBolla = NewCom_ADD_IdAgenda_DocNumero_Leggi(objServer, objSession, objPage, _
    '    '                                                            Piva, _
    '    '                                                            Id_Agenda, _
    '    '                                                            False, _
    '    '                                                            , _
    '    '                                                            FiltroAggiuntivo, _
    '    '                                                            Ordinamento, _
    '    '                                                            , )


    '    If Not IsNothing(DT_NumBolla) AndAlso DT_NumBolla.Rows.Count > 0 Then
    '        Doc_Numero_Sin = DT_NumBolla.Rows(0).Item("Doc_Numero_Sin")
    '        Doc_Numero = DT_NumBolla.Rows(0).Item("Doc_Numero")
    '        Doc_Numero_Des = DT_NumBolla.Rows(0).Item("Doc_Numero_Des")
    '    End If

    'End Sub


    ''##################################################################################
    'Public Function UdmDesParamQualitativo_from_TipoCodParamQualitativo(ByVal Tipo_Cod As Integer) As String

    '    Select Case Tipo_Cod

    '        Case FRUTTAGEL_INDMATCOD_PUNTEGGIO
    '            Return "n"

    '        Case FRUTTAGEL_INDMATCOD_GRADOTEND
    '            Return "psi"

    '        Case FRUTTAGEL_INDMATCOD_GRADOBRIX
    '            Return ""

    '        Case Else
    '            Return ""

    '    End Select


    'End Function


    ''##################################################################################
    'Public Function TipologiaPomodoro_from_GrvaCod(ByVal Grva_Cod As Integer) As String

    '    Select Case Grva_Cod

    '        Case 84
    '            Return "Ovale"

    '        Case 85
    '            Return "Ciliegino"

    '        Case 86
    '            Return "Verde (Insalataro)"

    '        Case 87
    '            Return "Tondo liscio a grappolo rosso"

    '        Case 88
    '            Return "Ovale liscio a grappolo rosso"

    '        Case 89
    '            Return "Lungo (S.Marzano)"

    '        Case 90
    '            Return "Datterino"

    '        Case 151
    '            Return "Tondo"

    '        Case 152
    '            Return "Lungo"

    '        Case 153
    '            Return "Alto licopene (per industria)"

    '        Case Else
    '            Return ""

    '    End Select


    'End Function

    ''##################################################################################
    'Public Sub TondoLungo_from_GrvaCod(ByVal Grva_Cod As Integer, _
    '                                    ByRef Tondo As String, _
    '                                    ByRef Lungo As String)

    '    Select Case Grva_Cod

    '        Case 151
    '            Tondo = "X"
    '            Lungo = ""

    '        Case 152
    '            Tondo = ""
    '            Lungo = "X"

    '        Case Else
    '            Tondo = ""
    '            Lungo = ""

    '    End Select


    'End Sub



    ''#####################################################################################################
    'Public Sub Calcola_Dati_Certificato_Pomodoro(ByVal Peso_Lordo As Double, _
    '                                                ByVal Tara_Veicolo As Double, _
    '                                                ByVal Tara_Imballi As Double, _
    '                                                ByVal Peso_Netto As Double, _
    '                                                ByVal Perc_Marcio As Double, _
    '                                                ByVal Perc_Verde As Double, _
    '                                                ByVal Perc_Inerti As Double, _
    '                                                ByVal Perc_Schiacciati As Double, _
    '                                                ByVal Perc_Immaturi As Double, _
    '                                                ByVal Perc_Scottature As Double, _
    '                                                ByVal Perc_Lesioni As Double, _
    '                                                ByVal Grado_Brix As Double, _
    '                                                ByVal Indice_Prezzo_Grado_Brix As Double, _
    '                                                ByVal Coefficiente As Double, _
    '                                                ByVal Franchigia As Double, _
    '                                                ByVal Premio_Pomo_Tardivo As Double, _
    '                                                ByVal Prezzo_Unitario_Contratto As Double, _
    '                                                 ByRef Perc_Tot_Dif_Magg As Double, _
    '                                                 ByRef Perc_Tot_Dif_Magg_Round As Double, _
    '                                                 ByRef Netto_Pag As Double, _
    '                                                ByRef Scarto As Double, _
    '                                                 ByRef Perc_Tot_Dif_Minori As Double, _
    '                                                 ByRef Dif_MinoriXCoeff As Double, _
    '                                                ByRef Dif_Magg_Franchigia As Double, _
    '                                                ByRef Indice_Variazione_Prezzo As Double, _
    '                                                ByRef Prezzo_Unitario_Finale As Double, _
    '                                                ByRef Importo_Totale_Pag As Double)

    '    Dim Prezzo_Unitario_Pomo_Tardivo As Double


    '    Perc_Tot_Dif_Magg = 0
    '    Perc_Tot_Dif_Magg += Perc_Marcio
    '    Perc_Tot_Dif_Magg += Perc_Verde
    '    Perc_Tot_Dif_Magg += Perc_Inerti

    '    'per calcolare lo scarto, la somma dei difetti maggiori va arrotondata:
    '    'tipo di arrotondamento: 4,5 -> 4, 4,51 -> 5
    '    'questo arrotondamento lo faccio con una funzione ad hoc
    '    Perc_Tot_Dif_Magg_Round = Arrotonda_Unita_DifettiMaggiori(Perc_Tot_Dif_Magg)

    '    Scarto = (Peso_Netto * Perc_Tot_Dif_Magg_Round) / 100

    '    Scarto = RoundNumber_ParteIntera(Scarto)

    '    Netto_Pag = Peso_Netto - Scarto

    '    Netto_Pag = RoundNumber_ParteIntera(Netto_Pag)

    '    Perc_Tot_Dif_Minori = 0
    '    Perc_Tot_Dif_Minori += Perc_Schiacciati
    '    Perc_Tot_Dif_Minori += Perc_Immaturi
    '    Perc_Tot_Dif_Minori += Perc_Scottature
    '    Perc_Tot_Dif_Minori += Perc_Lesioni

    '    Dif_MinoriXCoeff = Perc_Tot_Dif_Minori * Coefficiente

    '    Dif_Magg_Franchigia = Perc_Tot_Dif_Magg - Franchigia
    '    If Dif_Magg_Franchigia < 0 Then
    '        Dif_Magg_Franchigia = 0
    '    End If

    '    Indice_Variazione_Prezzo = Indice_Prezzo_Grado_Brix - Dif_MinoriXCoeff - Dif_Magg_Franchigia

    '    Prezzo_Unitario_Finale = (Prezzo_Unitario_Contratto * Indice_Variazione_Prezzo) / 100

    '    'Dim Prezzo_Unitario_Finale_Round As Double
    '    'Prezzo_Unitario_Finale_Round = Arrotonda_2Decimali(Prezzo_Unitario_Finale)
    '    'Importo_Totale_Pag = (Netto_Pag * Prezzo_Unitario_Finale_Round) / 1000

    '    Prezzo_Unitario_Finale = Arrotonda_2Decimali(Prezzo_Unitario_Finale)

    '    'modifica del 23/09/2010
    '    'nel caso di premio pomodoro tardivo
    '    If Premio_Pomo_Tardivo <> 0 Then
    '        'il premio è euro/tonnellata

    '        'divido il premio (euro/t) per il netto pagamento (t)
    '        Prezzo_Unitario_Pomo_Tardivo = Premio_Pomo_Tardivo / (Netto_Pag / 1000)

    '        Prezzo_Unitario_Pomo_Tardivo = Arrotonda_2Decimali(Prezzo_Unitario_Pomo_Tardivo)

    '        'sommo il prezzo unitario del pomo tardivo al prezzo unitario
    '        Prezzo_Unitario_Finale += Prezzo_Unitario_Pomo_Tardivo

    '    End If

    '    Importo_Totale_Pag = (Netto_Pag * Prezzo_Unitario_Finale) / 1000

    '    Importo_Totale_Pag = Arrotonda_2Decimali(Importo_Totale_Pag)

    '    'modifica del 23/09/2010
    '    'il premio non va sommato all'importo, va sommato il suo prezzo unitario al prezzo unitario finale
    '    'Importo_Totale_Pag += Premio_Pomo_Tardivo

    '    Importo_Totale_Pag = Arrotonda_2Decimali(Importo_Totale_Pag)


    'End Sub

    ''#####################################################################################################
    ''converte la qta da kg a quintali e arrotonda a 2 decimali
    'Public Function Qta_Ql_2Dec(ByVal Qta As Double) As Double

    '    Qta = Qta / 100

    '    Qta = Arrotonda_2Decimali(Qta)

    '    Return Qta


    'End Function

    ''#####################################################################################################
    ''funzione usata per la gestione del 2011
    ''i valori sono salvati in kg e vanno convertiti in quintali solo alla fine, dopo i calcoli
    'Public Sub Calcola_Dati_Certificato_Pomodoro_2011(ByVal Peso_Netto_Kg As Double, _
    '                                                    ByVal Perc_Marcio As Double, _
    '                                                    ByVal Perc_Verde As Double, _
    '                                                    ByVal Perc_Inerti As Double, _
    '                                                    ByVal Degrado_Perc As Double, _
    '                                                    ByVal Perc_Schiacciati As Double, _
    '                                                    ByVal Perc_Immaturi As Double, _
    '                                                    ByVal Perc_Scottature As Double, _
    '                                                    ByVal Perc_Lesioni As Double, _
    '                                                    ByVal Indice_Prezzo_Grado_Brix As Double, _
    '                                                    ByVal Coefficiente As Double, _
    '                                                    ByVal Franchigia As Double, _
    '                                                    ByVal Prezzo_Unitario_Finale_Kg As Double, _
    '                                                    ByRef Scarto_Q As Double, _
    '                                                     ByRef Netto_Pag_Q As Double, _
    '                                                    ByRef Perc_Tot_Dif_Maggiori As Double, _
    '                                                    ByRef Perc_Tot_Dif_Minori As Double, _
    '                                                    ByRef Dif_MinoriXCoeff As Double, _
    '                                                    ByRef Magg_Rid_DifMaggiori As Double, _
    '                                                    ByRef Indice_Variazione_Prezzo As Double, _
    '                                                    ByRef Prezzo_Unitario_Finale_Tn As Double, _
    '                                                    ByRef Importo_Totale_Pag As Double)

    '    Dim Scarto_Kg As Double
    '    Dim Netto_Pag_Kg As Double

    '    'ByVal Premio_EuroTon_PomoBio As Double, _
    '    'ByRef Premio_Pomo_Biologico As Double, _
    '    'ByRef Premio_Pomo_Tardivo As Double, _
    '    'Dim Prezzo_Unitario_Pomo_Tardivo As Double
    '    'Dim Prezzo_Unitario_Pomo_Bio As Double

    '    'Degrado_Perc è la somma dei difetti maggiori arrotondati
    '    Scarto_Kg = (Peso_Netto_Kg * Degrado_Perc) / 100
    '    Scarto_Kg = RoundNumber_ParteIntera(Scarto_Kg)

    '    Netto_Pag_Kg = Peso_Netto_Kg - Scarto_Kg
    '    Netto_Pag_Kg = RoundNumber_ParteIntera(Netto_Pag_Kg)

    '    Scarto_Q = Qta_Ql_2Dec(Scarto_Kg)
    '    Netto_Pag_Q = Qta_Ql_2Dec(Netto_Pag_Kg)

    '    Perc_Tot_Dif_Maggiori = 0
    '    Perc_Tot_Dif_Maggiori += Perc_Marcio
    '    Perc_Tot_Dif_Maggiori += Perc_Verde
    '    Perc_Tot_Dif_Maggiori += Perc_Inerti
    '    Perc_Tot_Dif_Maggiori = Arrotonda_2Decimali(Perc_Tot_Dif_Maggiori)

    '    Select Case Perc_Tot_Dif_Maggiori
    '        Case Is < 3
    '            'risulta un valore positivo
    '            Magg_Rid_DifMaggiori = Franchigia - Perc_Tot_Dif_Maggiori
    '            '---------
    '        Case Is >= 4
    '            'risulta un valore negativo
    '            Magg_Rid_DifMaggiori = Franchigia - Perc_Tot_Dif_Maggiori
    '            '---------
    '        Case Else
    '            'tra 3 e 3,99
    '            Magg_Rid_DifMaggiori = 0
    '    End Select
    '    Magg_Rid_DifMaggiori = Arrotonda_2Decimali(Magg_Rid_DifMaggiori)

    '    Perc_Tot_Dif_Minori = 0
    '    Perc_Tot_Dif_Minori += Perc_Schiacciati
    '    Perc_Tot_Dif_Minori += Perc_Immaturi
    '    Perc_Tot_Dif_Minori += Perc_Scottature
    '    Perc_Tot_Dif_Minori += Perc_Lesioni
    '    Perc_Tot_Dif_Minori = Arrotonda_2Decimali(Perc_Tot_Dif_Minori)

    '    Dif_MinoriXCoeff = Perc_Tot_Dif_Minori * Coefficiente
    '    Dif_MinoriXCoeff = Arrotonda_2Decimali(Dif_MinoriXCoeff)

    '    Indice_Variazione_Prezzo = Indice_Prezzo_Grado_Brix - Dif_MinoriXCoeff + Magg_Rid_DifMaggiori
    '    Indice_Variazione_Prezzo = RoundNumber_3Decimali(Indice_Variazione_Prezzo)

    '    'Prezzo_Unitario_Finale = (Prezzo_Unitario_Contratto * Indice_Variazione_Prezzo) / 100
    '    'Prezzo_Unitario_Finale = Arrotonda_2Decimali(Prezzo_Unitario_Finale)

    '    'If Premio_Pomo_Biologico <> 0 Then
    '    '    'è salvato in euro/tonnellata
    '    '    'sommo il prezzo unitario del pomo bio al prezzo unitario
    '    '    Prezzo_Unitario_Finale += Premio_EuroTon_PomoBio
    '    'End If
    '    'Prezzo_Unitario_Finale = Arrotonda_2Decimali(Prezzo_Unitario_Finale)

    '    'If Premio_Pomo_Tardivo <> 0 Then
    '    '    'il premio è euro/tonnellata
    '    '    'divido il premio (euro/t) per il netto pagamento (t)
    '    '    Prezzo_Unitario_Pomo_Tardivo = Premio_Pomo_Tardivo / (Netto_Pag / 10)
    '    '    Prezzo_Unitario_Pomo_Tardivo = Arrotonda_2Decimali(Prezzo_Unitario_Pomo_Tardivo)
    '    '    'sommo il prezzo unitario del pomo tardivo al prezzo unitario
    '    '    Prezzo_Unitario_Finale += Prezzo_Unitario_Pomo_Tardivo
    '    'End If
    '    'Prezzo_Unitario_Finale = Arrotonda_2Decimali(Prezzo_Unitario_Finale)

    '    Prezzo_Unitario_Finale_Tn = Prezzo_Unitario_Finale_Kg * 1000
    '    Prezzo_Unitario_Finale_Tn = Arrotonda_2Decimali(Prezzo_Unitario_Finale_Tn)

    '    ''divido per 10 perchè il netto a pagamento è in quintali
    '    'Importo_Totale_Pag = Netto_Pag * Prezzo_Unitario_Finale_Tn / 10

    '    'divido per 1000 perchè il netto a pagamento è in kg
    '    Importo_Totale_Pag = Netto_Pag_Kg * Prezzo_Unitario_Finale_Tn / 1000

    '    Importo_Totale_Pag = Arrotonda_2Decimali(Importo_Totale_Pag)

    'End Sub

    ''#####################################################################################################
    ''funzione usata per la gestione del 2012
    ''i valori sono salvati in kg e vanno convertiti in quintali solo alla fine, dopo i calcoli
    'Public Sub Calcola_Dati_Certificato_Pomodoro_2012(ByVal Peso_Netto_Kg As Double, _
    '                                                    ByVal Perc_Marcio As Double, _
    '                                                    ByVal Perc_Verde As Double, _
    '                                                    ByVal Perc_Inerti As Double, _
    '                                                    ByVal Degrado_Perc As Double, _
    '                                                    ByVal Perc_Schiacciati As Double, _
    '                                                    ByVal Perc_Immaturi As Double, _
    '                                                    ByVal Perc_Scottature As Double, _
    '                                                    ByVal Perc_Lesioni As Double, _
    '                                                    ByVal Indice_Prezzo_Grado_Brix As Double, _
    '                                                    ByVal Coefficiente As Double, _
    '                                                    ByVal Franchigia As Double, _
    '                                                    ByVal Prezzo_Unitario_Finale_Kg As Double, _
    '                                                    ByRef Scarto_Q As Double, _
    '                                                     ByRef Netto_Pag_Q As Double, _
    '                                                    ByRef Perc_Tot_Dif_Maggiori As Double, _
    '                                                    ByRef Perc_Tot_Dif_Minori As Double, _
    '                                                    ByRef Dif_MinoriXCoeff As Double, _
    '                                                    ByRef Magg_Rid_DifMaggiori As Double, _
    '                                                    ByRef Indice_Variazione_Prezzo As Double, _
    '                                                    ByRef Prezzo_Unitario_Finale_Tn As Double, _
    '                                                    ByRef Importo_Totale_Pag As Double)

    '    Dim Scarto_Kg As Double
    '    Dim Netto_Pag_Kg As Double

    '    'ByVal Premio_EuroTon_PomoBio As Double, _
    '    'ByRef Premio_Pomo_Biologico As Double, _
    '    'ByRef Premio_Pomo_Tardivo As Double, _
    '    'Dim Prezzo_Unitario_Pomo_Tardivo As Double
    '    'Dim Prezzo_Unitario_Pomo_Bio As Double

    '    'Degrado_Perc è la somma dei difetti maggiori arrotondati
    '    Scarto_Kg = (Peso_Netto_Kg * Degrado_Perc) / 100
    '    Scarto_Kg = RoundNumber_ParteIntera(Scarto_Kg)

    '    Netto_Pag_Kg = Peso_Netto_Kg - Scarto_Kg
    '    Netto_Pag_Kg = RoundNumber_ParteIntera(Netto_Pag_Kg)

    '    Scarto_Q = Qta_Ql_2Dec(Scarto_Kg)
    '    Netto_Pag_Q = Qta_Ql_2Dec(Netto_Pag_Kg)

    '    Perc_Tot_Dif_Maggiori = 0
    '    Perc_Tot_Dif_Maggiori += Perc_Marcio
    '    Perc_Tot_Dif_Maggiori += Perc_Verde
    '    Perc_Tot_Dif_Maggiori += Perc_Inerti
    '    Perc_Tot_Dif_Maggiori = Arrotonda_2Decimali(Perc_Tot_Dif_Maggiori)

    '    Select Case Perc_Tot_Dif_Maggiori
    '        Case Is < 3
    '            'risulta un valore positivo
    '            Magg_Rid_DifMaggiori = Franchigia - Perc_Tot_Dif_Maggiori
    '            '---------
    '        Case Is = 4
    '            Magg_Rid_DifMaggiori = -1.5
    '            '---------
    '        Case Is > 4
    '            'risulta un valore negativo
    '            'MODIFICA 2012 RISPETTO A 2011
    '            Magg_Rid_DifMaggiori = 4 - Perc_Tot_Dif_Maggiori
    '            Magg_Rid_DifMaggiori = Arrotonda_2Decimali(Magg_Rid_DifMaggiori)
    '            Magg_Rid_DifMaggiori = Magg_Rid_DifMaggiori * 100
    '            Magg_Rid_DifMaggiori = Arrotonda_2Decimali(Magg_Rid_DifMaggiori)
    '            Magg_Rid_DifMaggiori = Magg_Rid_DifMaggiori * 0.014999999999999999
    '            Magg_Rid_DifMaggiori = Arrotonda_2Decimali(Magg_Rid_DifMaggiori)
    '            Magg_Rid_DifMaggiori = -1.5 + Magg_Rid_DifMaggiori
    '            '---------
    '        Case Else
    '            'tra 3 e 3,99
    '            Magg_Rid_DifMaggiori = 0
    '    End Select
    '    Magg_Rid_DifMaggiori = Arrotonda_2Decimali(Magg_Rid_DifMaggiori)

    '    Perc_Tot_Dif_Minori = 0
    '    Perc_Tot_Dif_Minori += Perc_Schiacciati
    '    Perc_Tot_Dif_Minori += Perc_Immaturi
    '    Perc_Tot_Dif_Minori += Perc_Scottature
    '    Perc_Tot_Dif_Minori += Perc_Lesioni
    '    Perc_Tot_Dif_Minori = Arrotonda_2Decimali(Perc_Tot_Dif_Minori)

    '    Dif_MinoriXCoeff = Perc_Tot_Dif_Minori * Coefficiente
    '    Dif_MinoriXCoeff = Arrotonda_2Decimali(Dif_MinoriXCoeff)

    '    Indice_Variazione_Prezzo = Indice_Prezzo_Grado_Brix - Dif_MinoriXCoeff + Magg_Rid_DifMaggiori
    '    Indice_Variazione_Prezzo = RoundNumber_3Decimali(Indice_Variazione_Prezzo)

    '    'Prezzo_Unitario_Finale = (Prezzo_Unitario_Contratto * Indice_Variazione_Prezzo) / 100
    '    'Prezzo_Unitario_Finale = Arrotonda_2Decimali(Prezzo_Unitario_Finale)

    '    'If Premio_Pomo_Biologico <> 0 Then
    '    '    'è salvato in euro/tonnellata
    '    '    'sommo il prezzo unitario del pomo bio al prezzo unitario
    '    '    Prezzo_Unitario_Finale += Premio_EuroTon_PomoBio
    '    'End If
    '    'Prezzo_Unitario_Finale = Arrotonda_2Decimali(Prezzo_Unitario_Finale)

    '    'If Premio_Pomo_Tardivo <> 0 Then
    '    '    'il premio è euro/tonnellata
    '    '    'divido il premio (euro/t) per il netto pagamento (t)
    '    '    Prezzo_Unitario_Pomo_Tardivo = Premio_Pomo_Tardivo / (Netto_Pag / 10)
    '    '    Prezzo_Unitario_Pomo_Tardivo = Arrotonda_2Decimali(Prezzo_Unitario_Pomo_Tardivo)
    '    '    'sommo il prezzo unitario del pomo tardivo al prezzo unitario
    '    '    Prezzo_Unitario_Finale += Prezzo_Unitario_Pomo_Tardivo
    '    'End If
    '    'Prezzo_Unitario_Finale = Arrotonda_2Decimali(Prezzo_Unitario_Finale)

    '    Prezzo_Unitario_Finale_Tn = Prezzo_Unitario_Finale_Kg * 1000
    '    Prezzo_Unitario_Finale_Tn = Arrotonda_2Decimali(Prezzo_Unitario_Finale_Tn)

    '    ''divido per 10 perchè il netto a pagamento è in quintali
    '    'Importo_Totale_Pag = Netto_Pag * Prezzo_Unitario_Finale_Tn / 10

    '    'divido per 1000 perchè il netto a pagamento è in kg
    '    Importo_Totale_Pag = Netto_Pag_Kg * Prezzo_Unitario_Finale_Tn / 1000

    '    Importo_Totale_Pag = Arrotonda_2Decimali(Importo_Totale_Pag)

    'End Sub


    ''#####################################################################################################
    ''funzione usata per la gestione del 2013
    ''i valori sono salvati in kg e vanno convertiti in quintali solo alla fine, dopo i calcoli
    ''modifiche rispetto al 2012:
    ''Indice_Prezzo_Grado_Brix byref, lo calcolo qui dentro
    ''passo anche Grado_Brix
    'Public Sub Calcola_Dati_Certificato_Pomodoro_2013(ByVal Peso_Netto_Kg As Double, _
    '                                                    ByVal Perc_Marcio As Double, _
    '                                                    ByVal Perc_Verde As Double, _
    '                                                    ByVal Perc_Inerti As Double, _
    '                                                    ByVal Degrado_Perc As Double, _
    '                                                    ByVal Perc_Schiacciati As Double, _
    '                                                    ByVal Perc_Immaturi As Double, _
    '                                                    ByVal Perc_Scottature As Double, _
    '                                                    ByVal Perc_Lesioni As Double, _
    '                                                    ByVal Grado_Brix As Double, _
    '                                                    ByRef Indice_Prezzo_Grado_Brix As Double, _
    '                                                    ByVal Coefficiente As Double, _
    '                                                    ByVal Franchigia As Double, _
    '                                                    ByVal Prezzo_Unitario_Finale_Kg As Double, _
    '                                                    ByRef Scarto_Q As Double, _
    '                                                     ByRef Netto_Pag_Q As Double, _
    '                                                    ByRef Perc_Tot_Dif_Maggiori As Double, _
    '                                                    ByRef Perc_Tot_Dif_Minori As Double, _
    '                                                    ByRef Dif_MinoriXCoeff As Double, _
    '                                                    ByRef Magg_Rid_DifMaggiori As Double, _
    '                                                    ByRef Indice_Variazione_Prezzo As Double, _
    '                                                    ByRef Prezzo_Unitario_Finale_Tn As Double, _
    '                                                    ByRef Importo_Totale_Pag As Double)

    '    Dim Scarto_Kg As Double
    '    Dim Netto_Pag_Kg As Double

    '    'ByVal Premio_EuroTon_PomoBio As Double, _
    '    'ByRef Premio_Pomo_Biologico As Double, _
    '    'ByRef Premio_Pomo_Tardivo As Double, _
    '    'Dim Prezzo_Unitario_Pomo_Tardivo As Double
    '    'Dim Prezzo_Unitario_Pomo_Bio As Double

    '    'Degrado_Perc è la somma dei difetti maggiori arrotondati
    '    Scarto_Kg = (Peso_Netto_Kg * Degrado_Perc) / 100
    '    'Scarto_Kg = Arrotonda_Unita(Scarto_Kg)
    '    Scarto_Kg = AgronicaCoreDataProvider.Agro_Math.RoundNumber_ParteIntera(Scarto_Kg)

    '    Netto_Pag_Kg = Peso_Netto_Kg - Scarto_Kg
    '    '  Netto_Pag_Kg = Arrotonda_Unita(Netto_Pag_Kg)
    '    Netto_Pag_Kg = AgronicaCoreDataProvider.Agro_Math.RoundNumber_ParteIntera(Netto_Pag_Kg)

    '    Scarto_Q = Qta_Ql_2Dec(Scarto_Kg)
    '    Netto_Pag_Q = Qta_Ql_2Dec(Netto_Pag_Kg)

    '    Perc_Tot_Dif_Maggiori = 0
    '    Perc_Tot_Dif_Maggiori += Perc_Marcio
    '    Perc_Tot_Dif_Maggiori += Perc_Verde
    '    Perc_Tot_Dif_Maggiori += Perc_Inerti
    '    '  Perc_Tot_Dif_Maggiori = Arrotonda_2Decimali(Perc_Tot_Dif_Maggiori)
    '    Perc_Tot_Dif_Maggiori = AgronicaCoreDataProvider.Agro_Math.RoundNumber_2Decimali(Perc_Tot_Dif_Maggiori)

    '    Select Case Perc_Tot_Dif_Maggiori
    '        Case Is < 3
    '            'risulta un valore positivo
    '            '= 2012
    '            Magg_Rid_DifMaggiori = Franchigia - Perc_Tot_Dif_Maggiori
    '            '---------
    '        Case Is > 5
    '            'risulta un valore negativo
    '            'MODIFICA 2013 RISPETTO A 2012
    '            Magg_Rid_DifMaggiori = 5 - Perc_Tot_Dif_Maggiori
    '            '---------
    '        Case Else
    '            'tra 3,00 e 5,00
    '            Magg_Rid_DifMaggiori = 0
    '    End Select
    '    ' Magg_Rid_DifMaggiori = Arrotonda_2Decimali(Magg_Rid_DifMaggiori)
    '    Magg_Rid_DifMaggiori = AgronicaCoreDataProvider.Agro_Math.RoundNumber_2Decimali(Magg_Rid_DifMaggiori)

    '    Perc_Tot_Dif_Minori = 0
    '    Perc_Tot_Dif_Minori += Perc_Schiacciati
    '    Perc_Tot_Dif_Minori += Perc_Immaturi
    '    Perc_Tot_Dif_Minori += Perc_Scottature
    '    Perc_Tot_Dif_Minori += Perc_Lesioni
    '    ' Perc_Tot_Dif_Minori = Arrotonda_2Decimali(Perc_Tot_Dif_Minori)
    '    Perc_Tot_Dif_Minori = AgronicaCoreDataProvider.Agro_Math.RoundNumber_2Decimali(Perc_Tot_Dif_Minori)

    '    Dif_MinoriXCoeff = Perc_Tot_Dif_Minori * Coefficiente
    '    ' Dif_MinoriXCoeff = Arrotonda_2Decimali(Dif_MinoriXCoeff)
    '    Dif_MinoriXCoeff = AgronicaCoreDataProvider.Agro_Math.RoundNumber_2Decimali(Dif_MinoriXCoeff)

    '    'calcolo Indice_Prezzo_Grado_Brix (nel 2013 c'è la formula, non la tabella):
    '    Select Case Grado_Brix
    '        Case Is <= 4.3499999999999996
    '            Indice_Prezzo_Grado_Brix = 82.5
    '        Case Is >= 5.75
    '            Indice_Prezzo_Grado_Brix = 117.5
    '        Case Else
    '            Indice_Prezzo_Grado_Brix = Grado_Brix - 5.0499999999999998
    '            Indice_Prezzo_Grado_Brix = Indice_Prezzo_Grado_Brix * 25
    '            Indice_Prezzo_Grado_Brix = Indice_Prezzo_Grado_Brix + 100
    '    End Select

    '    Indice_Variazione_Prezzo = Indice_Prezzo_Grado_Brix - Dif_MinoriXCoeff + Magg_Rid_DifMaggiori
    '    ' Indice_Variazione_Prezzo = Arrotonda_3Decimali(Indice_Variazione_Prezzo)
    '    Indice_Variazione_Prezzo = AgronicaCoreDataProvider.Agro_Math.RoundNumber_3Decimali(Indice_Variazione_Prezzo)

    '    'Prezzo_Unitario_Finale = (Prezzo_Unitario_Contratto * Indice_Variazione_Prezzo) / 100
    '    'Prezzo_Unitario_Finale = Arrotonda_2Decimali(Prezzo_Unitario_Finale)

    '    'If Premio_Pomo_Biologico <> 0 Then
    '    '    'è salvato in euro/tonnellata
    '    '    'sommo il prezzo unitario del pomo bio al prezzo unitario
    '    '    Prezzo_Unitario_Finale += Premio_EuroTon_PomoBio
    '    'End If
    '    'Prezzo_Unitario_Finale = Arrotonda_2Decimali(Prezzo_Unitario_Finale)

    '    'If Premio_Pomo_Tardivo <> 0 Then
    '    '    'il premio è euro/tonnellata
    '    '    'divido il premio (euro/t) per il netto pagamento (t)
    '    '    Prezzo_Unitario_Pomo_Tardivo = Premio_Pomo_Tardivo / (Netto_Pag / 10)
    '    '    Prezzo_Unitario_Pomo_Tardivo = Arrotonda_2Decimali(Prezzo_Unitario_Pomo_Tardivo)
    '    '    'sommo il prezzo unitario del pomo tardivo al prezzo unitario
    '    '    Prezzo_Unitario_Finale += Prezzo_Unitario_Pomo_Tardivo
    '    'End If
    '    'Prezzo_Unitario_Finale = Arrotonda_2Decimali(Prezzo_Unitario_Finale)

    '    Prezzo_Unitario_Finale_Tn = Prezzo_Unitario_Finale_Kg * 1000
    '    ' Prezzo_Unitario_Finale_Tn = Arrotonda_2Decimali(Prezzo_Unitario_Finale_Tn)
    '    Prezzo_Unitario_Finale_Tn = AgronicaCoreDataProvider.Agro_Math.RoundNumber_2Decimali(Prezzo_Unitario_Finale_Tn)

    '    ''divido per 10 perchè il netto a pagamento è in quintali
    '    'Importo_Totale_Pag = Netto_Pag * Prezzo_Unitario_Finale_Tn / 10

    '    'divido per 1000 perchè il netto a pagamento è in kg
    '    Importo_Totale_Pag = Netto_Pag_Kg * Prezzo_Unitario_Finale_Tn / 1000

    '    ' Importo_Totale_Pag = Arrotonda_2Decimali(Importo_Totale_Pag)
    '    Importo_Totale_Pag = AgronicaCoreDataProvider.Agro_Math.RoundNumber_2Decimali(Importo_Totale_Pag)

    'End Sub


    ''#####################################################################################################
    ''funzione usata per la gestione del 2014
    ''i valori sono salvati in kg e vanno convertiti in quintali solo alla fine, dopo i calcoli
    ''modifica rispetto al 2013: formula per calcolo indice prezzo
    'Public Sub Calcola_Dati_Certificato_Pomodoro_2014(ByVal Peso_Netto_Kg As Double, _
    '                                                    ByVal Perc_Marcio As Double, _
    '                                                    ByVal Perc_Verde As Double, _
    '                                                    ByVal Perc_Inerti As Double, _
    '                                                    ByVal Degrado_Perc As Double, _
    '                                                    ByVal Perc_Schiacciati As Double, _
    '                                                    ByVal Perc_Immaturi As Double, _
    '                                                    ByVal Perc_Scottature As Double, _
    '                                                    ByVal Perc_Lesioni As Double, _
    '                                                    ByVal Grado_Brix As Double, _
    '                                                    ByRef Indice_Prezzo_Grado_Brix As Double, _
    '                                                    ByVal Coefficiente As Double, _
    '                                                    ByVal Franchigia As Double, _
    '                                                    ByVal Prezzo_Unitario_Finale_Kg As Double, _
    '                                                    ByRef Scarto_Q As Double, _
    '                                                     ByRef Netto_Pag_Q As Double, _
    '                                                    ByRef Perc_Tot_Dif_Maggiori As Double, _
    '                                                    ByRef Perc_Tot_Dif_Minori As Double, _
    '                                                    ByRef Dif_MinoriXCoeff As Double, _
    '                                                    ByRef Magg_Rid_DifMaggiori As Double, _
    '                                                    ByRef Indice_Variazione_Prezzo As Double, _
    '                                                    ByRef Prezzo_Unitario_Finale_Tn As Double, _
    '                                                    ByRef Importo_Totale_Pag As Double)

    '    Dim Scarto_Kg As Double
    '    Dim Netto_Pag_Kg As Double

    '    'ByVal Premio_EuroTon_PomoBio As Double, _
    '    'ByRef Premio_Pomo_Biologico As Double, _
    '    'ByRef Premio_Pomo_Tardivo As Double, _
    '    'Dim Prezzo_Unitario_Pomo_Tardivo As Double
    '    'Dim Prezzo_Unitario_Pomo_Bio As Double

    '    'Degrado_Perc è la somma dei difetti maggiori arrotondati
    '    Scarto_Kg = (Peso_Netto_Kg * Degrado_Perc) / 100
    '    'Scarto_Kg = Arrotonda_Unita(Scarto_Kg)
    '    Scarto_Kg = AgronicaCoreDataProvider.Agro_Math.RoundNumber_ParteIntera(Scarto_Kg)

    '    Netto_Pag_Kg = Peso_Netto_Kg - Scarto_Kg
    '    '  Netto_Pag_Kg = Arrotonda_Unita(Netto_Pag_Kg)
    '    Netto_Pag_Kg = AgronicaCoreDataProvider.Agro_Math.RoundNumber_ParteIntera(Netto_Pag_Kg)

    '    Scarto_Q = Qta_Ql_2Dec(Scarto_Kg)
    '    Netto_Pag_Q = Qta_Ql_2Dec(Netto_Pag_Kg)

    '    Perc_Tot_Dif_Maggiori = 0
    '    Perc_Tot_Dif_Maggiori += Perc_Marcio
    '    Perc_Tot_Dif_Maggiori += Perc_Verde
    '    Perc_Tot_Dif_Maggiori += Perc_Inerti
    '    '  Perc_Tot_Dif_Maggiori = Arrotonda_2Decimali(Perc_Tot_Dif_Maggiori)
    '    Perc_Tot_Dif_Maggiori = AgronicaCoreDataProvider.Agro_Math.RoundNumber_2Decimali(Perc_Tot_Dif_Maggiori)

    '    Select Case Perc_Tot_Dif_Maggiori
    '        Case Is < 3
    '            'risulta un valore positivo
    '            '= 2012
    '            Magg_Rid_DifMaggiori = Franchigia - Perc_Tot_Dif_Maggiori
    '            '---------
    '        Case Is > 5
    '            'risulta un valore negativo
    '            'MODIFICA 2013 RISPETTO A 2012
    '            Magg_Rid_DifMaggiori = 5 - Perc_Tot_Dif_Maggiori
    '            '---------
    '        Case Else
    '            'tra 3,00 e 5,00
    '            Magg_Rid_DifMaggiori = 0
    '    End Select
    '    ' Magg_Rid_DifMaggiori = Arrotonda_2Decimali(Magg_Rid_DifMaggiori)
    '    Magg_Rid_DifMaggiori = AgronicaCoreDataProvider.Agro_Math.RoundNumber_2Decimali(Magg_Rid_DifMaggiori)

    '    Perc_Tot_Dif_Minori = 0
    '    Perc_Tot_Dif_Minori += Perc_Schiacciati
    '    Perc_Tot_Dif_Minori += Perc_Immaturi
    '    Perc_Tot_Dif_Minori += Perc_Scottature
    '    Perc_Tot_Dif_Minori += Perc_Lesioni
    '    ' Perc_Tot_Dif_Minori = Arrotonda_2Decimali(Perc_Tot_Dif_Minori)
    '    Perc_Tot_Dif_Minori = AgronicaCoreDataProvider.Agro_Math.RoundNumber_2Decimali(Perc_Tot_Dif_Minori)

    '    Dif_MinoriXCoeff = Perc_Tot_Dif_Minori * Coefficiente
    '    ' Dif_MinoriXCoeff = Arrotonda_2Decimali(Dif_MinoriXCoeff)
    '    Dif_MinoriXCoeff = AgronicaCoreDataProvider.Agro_Math.RoundNumber_2Decimali(Dif_MinoriXCoeff)

    '    'calcolo Indice_Prezzo_Grado_Brix (nel 2013 c'è la formula, non la tabella):
    '    Select Case Grado_Brix
    '        Case Is <= 4.2999999999999998
    '            Indice_Prezzo_Grado_Brix = 82.5
    '        Case Is >= 5.7000000000000002
    '            Indice_Prezzo_Grado_Brix = 117.5
    '        Case Else
    '            Indice_Prezzo_Grado_Brix = Grado_Brix - 5.0
    '            Indice_Prezzo_Grado_Brix = Indice_Prezzo_Grado_Brix * 25
    '            Indice_Prezzo_Grado_Brix = Indice_Prezzo_Grado_Brix + 100
    '    End Select

    '    Indice_Variazione_Prezzo = Indice_Prezzo_Grado_Brix - Dif_MinoriXCoeff + Magg_Rid_DifMaggiori
    '    ' Indice_Variazione_Prezzo = Arrotonda_3Decimali(Indice_Variazione_Prezzo)
    '    Indice_Variazione_Prezzo = AgronicaCoreDataProvider.Agro_Math.RoundNumber_3Decimali(Indice_Variazione_Prezzo)

    '    'Prezzo_Unitario_Finale = (Prezzo_Unitario_Contratto * Indice_Variazione_Prezzo) / 100
    '    'Prezzo_Unitario_Finale = Arrotonda_2Decimali(Prezzo_Unitario_Finale)

    '    'If Premio_Pomo_Biologico <> 0 Then
    '    '    'è salvato in euro/tonnellata
    '    '    'sommo il prezzo unitario del pomo bio al prezzo unitario
    '    '    Prezzo_Unitario_Finale += Premio_EuroTon_PomoBio
    '    'End If
    '    'Prezzo_Unitario_Finale = Arrotonda_2Decimali(Prezzo_Unitario_Finale)

    '    'If Premio_Pomo_Tardivo <> 0 Then
    '    '    'il premio è euro/tonnellata
    '    '    'divido il premio (euro/t) per il netto pagamento (t)
    '    '    Prezzo_Unitario_Pomo_Tardivo = Premio_Pomo_Tardivo / (Netto_Pag / 10)
    '    '    Prezzo_Unitario_Pomo_Tardivo = Arrotonda_2Decimali(Prezzo_Unitario_Pomo_Tardivo)
    '    '    'sommo il prezzo unitario del pomo tardivo al prezzo unitario
    '    '    Prezzo_Unitario_Finale += Prezzo_Unitario_Pomo_Tardivo
    '    'End If
    '    'Prezzo_Unitario_Finale = Arrotonda_2Decimali(Prezzo_Unitario_Finale)

    '    Prezzo_Unitario_Finale_Tn = Prezzo_Unitario_Finale_Kg * 1000
    '    ' Prezzo_Unitario_Finale_Tn = Arrotonda_2Decimali(Prezzo_Unitario_Finale_Tn)
    '    Prezzo_Unitario_Finale_Tn = AgronicaCoreDataProvider.Agro_Math.RoundNumber_2Decimali(Prezzo_Unitario_Finale_Tn)

    '    ''divido per 10 perchè il netto a pagamento è in quintali
    '    'Importo_Totale_Pag = Netto_Pag * Prezzo_Unitario_Finale_Tn / 10

    '    'divido per 1000 perchè il netto a pagamento è in kg
    '    Importo_Totale_Pag = Netto_Pag_Kg * Prezzo_Unitario_Finale_Tn / 1000

    '    ' Importo_Totale_Pag = Arrotonda_2Decimali(Importo_Totale_Pag)
    '    Importo_Totale_Pag = AgronicaCoreDataProvider.Agro_Math.RoundNumber_2Decimali(Importo_Totale_Pag)

    'End Sub

    ''#####################################################################################################
    ''funzione usata per la gestione del 2015
    ''i valori sono salvati in kg e vanno convertiti in quintali solo alla fine, dopo i calcoli
    ''modifica rispetto al 2013: formula per calcolo indice prezzo
    'Public Sub Calcola_Dati_Certificato_Pomodoro_2015(ByVal Flag_Biologico As Boolean, _
    '                                                    ByVal Prezzo_Unitario_Contratto As Double, _
    '                                                    ByVal Peso_Netto_Kg As Double, _
    '                                                    ByVal Perc_Marcio As Double, _
    '                                                    ByVal Perc_Verde As Double, _
    '                                                    ByVal Perc_Inerti As Double, _
    '                                                    ByVal Degrado_Perc As Double, _
    '                                                    ByVal Perc_Schiacciati As Double, _
    '                                                    ByVal Perc_Immaturi As Double, _
    '                                                    ByVal Perc_Scottature As Double, _
    '                                                    ByVal Perc_Lesioni As Double, _
    '                                                    ByVal Grado_Brix As Double, _
    '                                                    ByRef Indice_Prezzo_Grado_Brix As Double, _
    '                                                    ByVal Coefficiente As Double, _
    '                                                    ByVal Franchigia As Double, _
    '                                                    ByVal Prezzo_Unitario_Finale_Kg As Double, _
    '                                                    ByRef Scarto_Q As Double, _
    '                                                     ByRef Netto_Pag_Q As Double, _
    '                                                    ByRef Perc_Tot_Dif_Maggiori As Double, _
    '                                                    ByRef Perc_Tot_Dif_Minori As Double, _
    '                                                    ByRef Dif_MinoriXCoeff As Double, _
    '                                                    ByRef Magg_Rid_DifMaggiori As Double, _
    '                                                    ByRef Indice_Variazione_Prezzo As Double, _
    '                                                    ByRef Prezzo_Unitario_Finale_Tn As Double, _
    '                                                    ByRef Importo_Totale_Pag As Double)

    '    Dim Scarto_Kg As Double
    '    Dim Netto_Pag_Kg As Double

    '    'ByVal Premio_EuroTon_PomoBio As Double, _
    '    'ByRef Premio_Pomo_Biologico As Double, _
    '    'ByRef Premio_Pomo_Tardivo As Double, _
    '    'Dim Prezzo_Unitario_Pomo_Tardivo As Double
    '    'Dim Prezzo_Unitario_Pomo_Bio As Double

    '    'Degrado_Perc è la somma dei difetti maggiori arrotondati
    '    Scarto_Kg = (Peso_Netto_Kg * Degrado_Perc) / 100
    '    'Scarto_Kg = Arrotonda_Unita(Scarto_Kg)
    '    Scarto_Kg = AgronicaCoreDataProvider.Agro_Math.RoundNumber_ParteIntera(Scarto_Kg)

    '    Netto_Pag_Kg = Peso_Netto_Kg - Scarto_Kg
    '    '  Netto_Pag_Kg = Arrotonda_Unita(Netto_Pag_Kg)
    '    Netto_Pag_Kg = AgronicaCoreDataProvider.Agro_Math.RoundNumber_ParteIntera(Netto_Pag_Kg)

    '    Scarto_Q = Qta_Ql_2Dec(Scarto_Kg)
    '    Netto_Pag_Q = Qta_Ql_2Dec(Netto_Pag_Kg)

    '    Perc_Tot_Dif_Maggiori = 0
    '    Perc_Tot_Dif_Maggiori += Perc_Marcio
    '    Perc_Tot_Dif_Maggiori += Perc_Verde
    '    Perc_Tot_Dif_Maggiori += Perc_Inerti
    '    '  Perc_Tot_Dif_Maggiori = Arrotonda_2Decimali(Perc_Tot_Dif_Maggiori)
    '    Perc_Tot_Dif_Maggiori = AgronicaCoreDataProvider.Agro_Math.RoundNumber_2Decimali(Perc_Tot_Dif_Maggiori)

    '    Perc_Tot_Dif_Minori = 0
    '    Perc_Tot_Dif_Minori += Perc_Schiacciati
    '    Perc_Tot_Dif_Minori += Perc_Immaturi
    '    Perc_Tot_Dif_Minori += Perc_Scottature
    '    Perc_Tot_Dif_Minori += Perc_Lesioni
    '    ' Perc_Tot_Dif_Minori = Arrotonda_2Decimali(Perc_Tot_Dif_Minori)
    '    Perc_Tot_Dif_Minori = AgronicaCoreDataProvider.Agro_Math.RoundNumber_2Decimali(Perc_Tot_Dif_Minori)

    '    'per il 2015, per il pomo Biologico non c'è diminuzione di prezzo
    '    If Flag_Biologico = False Then

    '        Select Case Perc_Tot_Dif_Maggiori
    '            Case Is < 3
    '                'risulta un valore positivo
    '                '= 2012
    '                Magg_Rid_DifMaggiori = Franchigia - Perc_Tot_Dif_Maggiori
    '                '---------
    '            Case Is > 5
    '                'risulta un valore negativo
    '                'MODIFICA 2013 RISPETTO A 2012
    '                Magg_Rid_DifMaggiori = 5 - Perc_Tot_Dif_Maggiori
    '                '---------
    '            Case Else
    '                'tra 3,00 e 5,00
    '                Magg_Rid_DifMaggiori = 0
    '        End Select
    '        ' Magg_Rid_DifMaggiori = Arrotonda_2Decimali(Magg_Rid_DifMaggiori)
    '        Magg_Rid_DifMaggiori = AgronicaCoreDataProvider.Agro_Math.RoundNumber_2Decimali(Magg_Rid_DifMaggiori)


    '        Dif_MinoriXCoeff = Perc_Tot_Dif_Minori * Coefficiente
    '        ' Dif_MinoriXCoeff = Arrotonda_2Decimali(Dif_MinoriXCoeff)
    '        Dif_MinoriXCoeff = AgronicaCoreDataProvider.Agro_Math.RoundNumber_2Decimali(Dif_MinoriXCoeff)

    '        'calcolo Indice_Prezzo_Grado_Brix (nel 2013 c'è la formula, non la tabella):
    '        Select Case Grado_Brix
    '            Case Is <= 4.2999999999999998
    '                Indice_Prezzo_Grado_Brix = 82.5
    '            Case Is >= 5.7000000000000002
    '                Indice_Prezzo_Grado_Brix = 117.5
    '            Case Else
    '                Indice_Prezzo_Grado_Brix = Grado_Brix - 5.0
    '                Indice_Prezzo_Grado_Brix = Indice_Prezzo_Grado_Brix * 25
    '                Indice_Prezzo_Grado_Brix = Indice_Prezzo_Grado_Brix + 100
    '        End Select

    '        Indice_Variazione_Prezzo = Indice_Prezzo_Grado_Brix - Dif_MinoriXCoeff + Magg_Rid_DifMaggiori
    '        ' Indice_Variazione_Prezzo = Arrotonda_3Decimali(Indice_Variazione_Prezzo)
    '        Indice_Variazione_Prezzo = AgronicaCoreDataProvider.Agro_Math.RoundNumber_3Decimali(Indice_Variazione_Prezzo)

    '    Else
    '        'segnalato il 04/08/2015
    '        'Indice_Prezzo_Grado_Brix = Prezzo_Unitario_Contratto
    '        Indice_Prezzo_Grado_Brix = 100
    '        Indice_Variazione_Prezzo = 100
    '    End If 'Flag_Bio


    '    'Prezzo_Unitario_Finale = (Prezzo_Unitario_Contratto * Indice_Variazione_Prezzo) / 100
    '    'Prezzo_Unitario_Finale = Arrotonda_2Decimali(Prezzo_Unitario_Finale)

    '    'If Premio_Pomo_Biologico <> 0 Then
    '    '    'è salvato in euro/tonnellata
    '    '    'sommo il prezzo unitario del pomo bio al prezzo unitario
    '    '    Prezzo_Unitario_Finale += Premio_EuroTon_PomoBio
    '    'End If
    '    'Prezzo_Unitario_Finale = Arrotonda_2Decimali(Prezzo_Unitario_Finale)

    '    'If Premio_Pomo_Tardivo <> 0 Then
    '    '    'il premio è euro/tonnellata
    '    '    'divido il premio (euro/t) per il netto pagamento (t)
    '    '    Prezzo_Unitario_Pomo_Tardivo = Premio_Pomo_Tardivo / (Netto_Pag / 10)
    '    '    Prezzo_Unitario_Pomo_Tardivo = Arrotonda_2Decimali(Prezzo_Unitario_Pomo_Tardivo)
    '    '    'sommo il prezzo unitario del pomo tardivo al prezzo unitario
    '    '    Prezzo_Unitario_Finale += Prezzo_Unitario_Pomo_Tardivo
    '    'End If
    '    'Prezzo_Unitario_Finale = Arrotonda_2Decimali(Prezzo_Unitario_Finale)

    '    Prezzo_Unitario_Finale_Tn = Prezzo_Unitario_Finale_Kg * 1000
    '    ' Prezzo_Unitario_Finale_Tn = Arrotonda_2Decimali(Prezzo_Unitario_Finale_Tn)
    '    Prezzo_Unitario_Finale_Tn = AgronicaCoreDataProvider.Agro_Math.RoundNumber_2Decimali(Prezzo_Unitario_Finale_Tn)

    '    ''divido per 10 perchè il netto a pagamento è in quintali
    '    'Importo_Totale_Pag = Netto_Pag * Prezzo_Unitario_Finale_Tn / 10

    '    'divido per 1000 perchè il netto a pagamento è in kg
    '    Importo_Totale_Pag = Netto_Pag_Kg * Prezzo_Unitario_Finale_Tn / 1000

    '    ' Importo_Totale_Pag = Arrotonda_2Decimali(Importo_Totale_Pag)
    '    Importo_Totale_Pag = AgronicaCoreDataProvider.Agro_Math.RoundNumber_2Decimali(Importo_Totale_Pag)

    'End Sub





    ''#####################################################################################################
    ''funzione usata per la gestione del 2011
    ''i valori sono salvati in kg e vanno convertiti in quintali
    'Public Sub Calcola_Dati_Certificato_Pomodoro_2011_TEMP2(ByVal Peso_Netto_Q As Double, _
    '                                                        ByVal Perc_Marcio As Double, _
    '                                                        ByVal Perc_Verde As Double, _
    '                                                        ByVal Perc_Inerti As Double, _
    '                                                        ByVal Degrado_Perc As Double, _
    '                                                        ByVal Perc_Schiacciati As Double, _
    '                                                        ByVal Perc_Immaturi As Double, _
    '                                                        ByVal Perc_Scottature As Double, _
    '                                                        ByVal Perc_Lesioni As Double, _
    '                                                        ByVal Indice_Prezzo_Grado_Brix As Double, _
    '                                                        ByVal Coefficiente As Double, _
    '                                                        ByVal Franchigia As Double, _
    '                                                        ByVal Prezzo_Unitario_Finale_Kg As Double, _
    '                                                        ByRef Scarto As Double, _
    '                                                        ByRef Netto_Pag As Double, _
    '                                                        ByRef Perc_Tot_Dif_Maggiori As Double, _
    '                                                        ByRef Perc_Tot_Dif_Minori As Double, _
    '                                                        ByRef Dif_MinoriXCoeff As Double, _
    '                                                        ByRef Magg_Rid_DifMaggiori As Double, _
    '                                                        ByRef Indice_Variazione_Prezzo As Double, _
    '                                                        ByRef Prezzo_Unitario_Finale_Tn As Double, _
    '                                                        ByRef Importo_Totale_Pag As Double)

    '    'ByVal Premio_EuroTon_PomoBio As Double, _
    '    'ByRef Premio_Pomo_Biologico As Double, _
    '    'ByRef Premio_Pomo_Tardivo As Double, _
    '    'Dim Prezzo_Unitario_Pomo_Tardivo As Double
    '    'Dim Prezzo_Unitario_Pomo_Bio As Double

    '    'Degrado_Perc è la somma dei difetti maggiori arrotondati
    '    Scarto = (Peso_Netto_Q * Degrado_Perc) / 100
    '    Scarto = Arrotonda_2Decimali(Scarto)

    '    Netto_Pag = Peso_Netto_Q - Scarto
    '    Netto_Pag = Arrotonda_2Decimali(Netto_Pag)

    '    Perc_Tot_Dif_Maggiori = 0
    '    Perc_Tot_Dif_Maggiori += Perc_Marcio
    '    Perc_Tot_Dif_Maggiori += Perc_Verde
    '    Perc_Tot_Dif_Maggiori += Perc_Inerti
    '    Perc_Tot_Dif_Maggiori = Arrotonda_2Decimali(Perc_Tot_Dif_Maggiori)

    '    Select Case Perc_Tot_Dif_Maggiori
    '        Case Is < 3
    '            'risulta un valore positivo
    '            Magg_Rid_DifMaggiori = Franchigia - Perc_Tot_Dif_Maggiori
    '            '---------
    '        Case Is >= 4
    '            'risulta un valore negativo
    '            Magg_Rid_DifMaggiori = Franchigia - Perc_Tot_Dif_Maggiori
    '            '---------
    '        Case Else
    '            'tra 3 e 3,99
    '            Magg_Rid_DifMaggiori = 0
    '    End Select
    '    Magg_Rid_DifMaggiori = Arrotonda_2Decimali(Magg_Rid_DifMaggiori)

    '    Perc_Tot_Dif_Minori = 0
    '    Perc_Tot_Dif_Minori += Perc_Schiacciati
    '    Perc_Tot_Dif_Minori += Perc_Immaturi
    '    Perc_Tot_Dif_Minori += Perc_Scottature
    '    Perc_Tot_Dif_Minori += Perc_Lesioni
    '    Perc_Tot_Dif_Minori = Arrotonda_2Decimali(Perc_Tot_Dif_Minori)

    '    Dif_MinoriXCoeff = Perc_Tot_Dif_Minori * Coefficiente
    '    Dif_MinoriXCoeff = Arrotonda_2Decimali(Dif_MinoriXCoeff)

    '    Indice_Variazione_Prezzo = Indice_Prezzo_Grado_Brix - Dif_MinoriXCoeff + Magg_Rid_DifMaggiori
    '    Indice_Variazione_Prezzo = RoundNumber_3Decimali(Indice_Variazione_Prezzo)

    '    'Prezzo_Unitario_Finale = (Prezzo_Unitario_Contratto * Indice_Variazione_Prezzo) / 100
    '    'Prezzo_Unitario_Finale = Arrotonda_2Decimali(Prezzo_Unitario_Finale)

    '    'If Premio_Pomo_Biologico <> 0 Then
    '    '    'è salvato in euro/tonnellata
    '    '    'sommo il prezzo unitario del pomo bio al prezzo unitario
    '    '    Prezzo_Unitario_Finale += Premio_EuroTon_PomoBio
    '    'End If
    '    'Prezzo_Unitario_Finale = Arrotonda_2Decimali(Prezzo_Unitario_Finale)

    '    'If Premio_Pomo_Tardivo <> 0 Then
    '    '    'il premio è euro/tonnellata
    '    '    'divido il premio (euro/t) per il netto pagamento (t)
    '    '    Prezzo_Unitario_Pomo_Tardivo = Premio_Pomo_Tardivo / (Netto_Pag / 10)
    '    '    Prezzo_Unitario_Pomo_Tardivo = Arrotonda_2Decimali(Prezzo_Unitario_Pomo_Tardivo)
    '    '    'sommo il prezzo unitario del pomo tardivo al prezzo unitario
    '    '    Prezzo_Unitario_Finale += Prezzo_Unitario_Pomo_Tardivo
    '    'End If
    '    'Prezzo_Unitario_Finale = Arrotonda_2Decimali(Prezzo_Unitario_Finale)

    '    Prezzo_Unitario_Finale_Tn = Prezzo_Unitario_Finale_Kg * 1000
    '    Prezzo_Unitario_Finale_Tn = Arrotonda_2Decimali(Prezzo_Unitario_Finale_Tn)

    '    'divido per 10 perchè il netto a pagamento è in quintali
    '    Importo_Totale_Pag = Netto_Pag * Prezzo_Unitario_Finale_Tn / 10
    '    Importo_Totale_Pag = Arrotonda_2Decimali(Importo_Totale_Pag)

    'End Sub


    ''#####################################################################################################
    ''funzione usata per la gestione del 2011
    ''i valori sono salvati in kg e vanno convertiti in quintali
    'Public Sub Calcola_Dati_Certificato_Pomodoro_2011_TEMP(ByVal Peso_Lordo As Double, _
    '                                                ByVal Tara_Veicolo As Double, _
    '                                                ByVal Tara_Imballi As Double, _
    '                                                ByVal Peso_Netto As Double, _
    '                                                ByVal Perc_Marcio As Double, _
    '                                                ByVal Perc_Verde As Double, _
    '                                                ByVal Perc_Inerti As Double, _
    '                                                ByVal Perc_Schiacciati As Double, _
    '                                                ByVal Perc_Immaturi As Double, _
    '                                                ByVal Perc_Scottature As Double, _
    '                                                ByVal Perc_Lesioni As Double, _
    '                                                ByVal Grado_Brix As Double, _
    '                                                ByVal Indice_Prezzo_Grado_Brix As Double, _
    '                                                ByVal Coefficiente As Double, _
    '                                                ByVal Franchigia As Double, _
    '                                                ByVal Premio_EuroTon_PomoBio As Double, _
    '                                                ByRef Premio_Pomo_Biologico As Double, _
    '                                                ByRef Premio_Pomo_Tardivo As Double, _
    '                                                ByVal Prezzo_Unitario_Contratto As Double, _
    '                                                 ByRef Perc_Tot_Dif_Magg As Double, _
    '                                                 ByRef Perc_Tot_Dif_Magg_Round As Double, _
    '                                                 ByRef Netto_Pag As Double, _
    '                                                ByRef Scarto As Double, _
    '                                                 ByRef Perc_Tot_Dif_Minori As Double, _
    '                                                 ByRef Dif_MinoriXCoeff As Double, _
    '                                                ByRef Dif_Magg_Franchigia As Double, _
    '                                                ByRef Indice_Variazione_Prezzo As Double, _
    '                                                ByRef Prezzo_Unitario_Finale As Double, _
    '                                                ByRef Importo_Totale_Pag As Double)

    '    'sono già a quintali (convertiti da altra funzione)
    '    'Peso_Lordo -  Tara_Veicolo - Tara_Imballi - Peso_Netto 

    '    Dim Prezzo_Unitario_Pomo_Tardivo As Double
    '    Dim Prezzo_Unitario_Pomo_Bio As Double

    '    Perc_Tot_Dif_Magg = 0
    '    Perc_Tot_Dif_Magg += Perc_Marcio
    '    Perc_Tot_Dif_Magg += Perc_Verde
    '    Perc_Tot_Dif_Magg += Perc_Inerti

    '    'per calcolare lo scarto, la somma dei difetti maggiori va arrotondata:
    '    'tipo di arrotondamento: 4,5 -> 4, 4,51 -> 5
    '    'questo arrotondamento lo faccio con una funzione ad hoc
    '    Perc_Tot_Dif_Magg_Round = Arrotonda_Unita_DifettiMaggiori(Perc_Tot_Dif_Magg)

    '    Scarto = (Peso_Netto * Perc_Tot_Dif_Magg_Round) / 100

    '    ' Scarto = Arrotonda_Unita(Scarto)
    '    Scarto = Arrotonda_2Decimali(Scarto)

    '    Netto_Pag = Peso_Netto - Scarto

    '    '  Netto_Pag = Arrotonda_Unita(Netto_Pag)
    '    Netto_Pag = Arrotonda_2Decimali(Netto_Pag)

    '    Perc_Tot_Dif_Minori = 0
    '    Perc_Tot_Dif_Minori += Perc_Schiacciati
    '    Perc_Tot_Dif_Minori += Perc_Immaturi
    '    Perc_Tot_Dif_Minori += Perc_Scottature
    '    Perc_Tot_Dif_Minori += Perc_Lesioni

    '    Dif_MinoriXCoeff = Perc_Tot_Dif_Minori * Coefficiente

    '    Select Case Perc_Tot_Dif_Magg
    '        Case Is < 3
    '            'risulta un valore positivo
    '            Dif_Magg_Franchigia = Franchigia - Perc_Tot_Dif_Magg
    '            '---------
    '        Case Is >= 4
    '            'risulta un valore negativo
    '            Dif_Magg_Franchigia = Franchigia - Perc_Tot_Dif_Magg
    '            '---------
    '        Case Else
    '            'tra 3 e 3,99
    '            Dif_Magg_Franchigia = 0
    '    End Select

    '    Indice_Variazione_Prezzo = Indice_Prezzo_Grado_Brix - Dif_MinoriXCoeff + Dif_Magg_Franchigia

    '    Prezzo_Unitario_Finale = (Prezzo_Unitario_Contratto * Indice_Variazione_Prezzo) / 100

    '    'Dim Prezzo_Unitario_Finale_Round As Double
    '    'Prezzo_Unitario_Finale_Round = Arrotonda_2Decimali(Prezzo_Unitario_Finale)
    '    'Importo_Totale_Pag = (Netto_Pag * Prezzo_Unitario_Finale_Round) / 1000

    '    Prezzo_Unitario_Finale = Arrotonda_2Decimali(Prezzo_Unitario_Finale)

    '    If Premio_Pomo_Biologico <> 0 Then
    '        'è salvato in euro/tonnellata

    '        'il prezzo unitario del pomo bio è di 25 euro/ton
    '        Prezzo_Unitario_Pomo_Bio = Premio_EuroTon_PomoBio

    '        ''divido il premio (euro/t) per il netto pagamento (t)
    '        'Prezzo_Unitario_Pomo_Bio = Premio_Pomo_Biologico / (Netto_Pag / 10)

    '        'Prezzo_Unitario_Pomo_Bio = Arrotonda_2Decimali(Prezzo_Unitario_Pomo_Bio)

    '        ''converto in quintali
    '        'Premio_Pomo_Biologico = Qta_Ql_2Dec(Premio_Pomo_Biologico)

    '        'sommo il prezzo unitario del pomo bio al prezzo unitario
    '        Prezzo_Unitario_Finale += Prezzo_Unitario_Pomo_Bio

    '    End If

    '    Prezzo_Unitario_Finale = Arrotonda_2Decimali(Prezzo_Unitario_Finale)

    '    If Premio_Pomo_Tardivo <> 0 Then
    '        'il premio è euro/tonnellata

    '        'divido il premio (euro/t) per il netto pagamento (t)
    '        Prezzo_Unitario_Pomo_Tardivo = Premio_Pomo_Tardivo / (Netto_Pag / 10)

    '        Prezzo_Unitario_Pomo_Tardivo = Arrotonda_2Decimali(Prezzo_Unitario_Pomo_Tardivo)

    '        'sommo il prezzo unitario del pomo tardivo al prezzo unitario
    '        Prezzo_Unitario_Finale += Prezzo_Unitario_Pomo_Tardivo

    '        ''visualizzo poi il premio pomo tardivo in quintali
    '        'Premio_Pomo_Tardivo = Premio_Pomo_Tardivo / 10

    '    End If

    '    Prezzo_Unitario_Finale = Arrotonda_2Decimali(Prezzo_Unitario_Finale)

    '    ' Importo_Totale_Pag = (Netto_Pag * Prezzo_Unitario_Finale) / 1000 da tn a kg
    '    Importo_Totale_Pag = (Netto_Pag * Prezzo_Unitario_Finale) / 10 'da tn a q

    '    Importo_Totale_Pag = Arrotonda_2Decimali(Importo_Totale_Pag)

    '    'modifica del 23/09/2010
    '    'il premio non va sommato all'importo, va sommato il suo prezzo unitario al prezzo unitario finale
    '    'Importo_Totale_Pag += Premio_Pomo_Tardivo
    '    'Importo_Totale_Pag = Arrotonda_2Decimali(Importo_Totale_Pag)


    'End Sub





    ''#####################################################################################################
    'Public Sub Calcola_NettoPagamento(ByVal Peso_Netto As Double, _
    '                                    ByVal Degrado_Perc As Double, _
    '                                    ByVal Udm_Cod As Integer, _
    '                                    ByRef Degrado As Double, _
    '                                    ByRef Netto_Pagamento As Double)

    '    'se ,5 il Math.Round arrotonda x difetto
    '    'allora uso la formula manuale:
    '    'Degrado_Dbl = Degrado_Dbl + 0.5
    '    'Degrado_Dbl = Int(Degrado_Dbl)
    '    Select Case Udm_Cod

    '        Case 2 'kg

    '            Peso_Netto = RoundNumber_ParteIntera(Peso_Netto)

    '            Degrado = (Peso_Netto * Degrado_Perc) / 100

    '            'nel caso dei kg, non ci devono essere decimali
    '            Degrado = RoundNumber_ParteIntera(Degrado)

    '            Netto_Pagamento = (Peso_Netto - Degrado)

    '        Case 4 'q

    '            Peso_Netto = Arrotonda_2Decimali(Peso_Netto)

    '            Degrado = (Peso_Netto * Degrado_Perc) / 100

    '            'nel caso dei quintali, ci devono essere 2 decimali
    '            Degrado = Arrotonda_2Decimali(Degrado)

    '            Netto_Pagamento = (Peso_Netto - Degrado)

    '        Case Else
    '            'errore

    '            Peso_Netto = RoundNumber_ParteIntera(Peso_Netto)

    '            Degrado = (Peso_Netto * Degrado_Perc) / 100

    '            Degrado = RoundNumber_ParteIntera(Degrado)

    '            Netto_Pagamento = (Peso_Netto - Degrado)

    '    End Select


    'End Sub

    ''#####################################################################################################
    'Public Function Calcola_PesoLordo2(ByVal Flag_1Kg_2Qli As Integer, _
    '                                    ByVal Tipo_Peso As Integer, _
    '                                    ByVal Peso As Double, _
    '                                    ByVal Tara_Imballi As Double) As Double

    '    Dim Peso_Lordo As Double

    '    Select Case Tipo_Peso
    '        Case 0 'lordo
    '            Peso_Lordo = Peso
    '        Case 1 'netto
    '            Peso_Lordo = Peso + Tara_Imballi
    '    End Select

    '    Select Case Flag_1Kg_2Qli
    '        Case 2 'quintali
    '            Peso_Lordo = Qta_Ql_2Dec(Peso_Lordo)
    '    End Select

    '    Return Peso_Lordo

    'End Function

    ''#####################################################################################################
    'Public Function Calcola_PesoNetto2(ByVal Flag_1Kg_2Qli As Integer, _
    '                                    ByVal Tipo_Peso As Integer, _
    '                                    ByVal Peso As Double, _
    '                                    ByVal Tara_Imballi As Double) As Double

    '    Dim Peso_Netto As Double

    '    Select Case Tipo_Peso
    '        Case 0 'lordo
    '            Peso_Netto = Peso - Tara_Imballi
    '        Case 1 'netto
    '            Peso_Netto = Peso
    '    End Select

    '    'non è udm_cod
    '    Select Case Flag_1Kg_2Qli
    '        Case 2 'quintali
    '            Peso_Netto = Qta_Ql_2Dec(Peso_Netto)
    '    End Select


    '    Return Peso_Netto

    'End Function

    ''#####################################################################################################
    'Public Function Calcola_PesoTotale(ByVal Flag_1Kg_2Qli As Integer, _
    '                                    ByVal Peso_Lordo_Kg As Double, _
    '                                    ByVal Tara_Veicolo_Kg As Double) As Double

    '    Dim Peso_Totale As Double

    '    Peso_Totale = Peso_Lordo_Kg + Tara_Veicolo_Kg

    '    Peso_Totale = RoundNumber_ParteIntera(Peso_Totale)

    '    'non è udm_cod
    '    Select Case Flag_1Kg_2Qli
    '        Case 2 'quintali
    '            Peso_Totale = Qta_Ql_2Dec(Peso_Totale)
    '    End Select


    '    Return Peso_Totale

    'End Function

    '''#####################################################################################################
    '''converte a quintali le tare e il peso netto (se richiesto)
    '''e calcola il peso totale
    ''Public Sub Calcola_PesoTotale(ByVal Flag_1Kg_2Qli As Integer, _
    ''                                ByVal Tara_Veicolo_IN As Double, _
    ''                                ByVal Tara_Imballi_IN As Double, _
    ''                                ByVal Peso_Netto_IN As Double, _
    ''                                 ByRef Tara_Veicolo_OUT As Double, _
    ''                                ByRef Tara_Imballi_OUT As Double, _
    ''                                ByRef Peso_Netto_OUT As Double, _
    ''                                ByRef Peso_Totale_OUT As Double)

    ''    Select Case Flag_1Kg_2Qli

    ''        Case 2 'quintali
    ''            Tara_Veicolo_OUT = Qta_Ql_2Dec(Tara_Veicolo_IN)
    ''            Tara_Imballi_OUT = Qta_Ql_2Dec(Tara_Imballi_IN)
    ''            Peso_Netto_OUT = Qta_Ql_2Dec(Peso_Netto_IN)

    ''        Case Else 'kg
    ''            Tara_Veicolo_OUT = Tara_Veicolo_IN
    ''            Tara_Imballi_OUT = Tara_Imballi_IN
    ''            Peso_Netto_OUT = Peso_Netto_IN

    ''    End Select

    ''    Peso_Totale_OUT = Peso_Netto_OUT + Tara_Veicolo_OUT + Tara_Imballi_OUT


    ''End Sub


    ''################################################################
    'Public Function Arrotonda_Unita_DifettiMaggiori(ByVal Valore As Double) As Double

    '    Dim Str_Valore As String
    '    Dim Str_ParteIntera As String
    '    Dim Str_ParteDecimale As String

    '    Str_Valore = CStr(Valore)

    '    If InStr(Str_Valore, ",") > 0 Then
    '        'il numero ha una parte decimale

    '        'prendo la parte intera
    '        Str_ParteIntera = Str_Valore.Split(",")(0)

    '        'prendo la parte decimale
    '        Str_ParteDecimale = Str_Valore.Split(",")(1)

    '        'prendo le prime due cifre decimali
    '        'se ho una cifra sola occorre aggiugnere uno 0 a destra
    '        Str_ParteDecimale = Left(Mid(Str_ParteDecimale, 1, 2) + "00", 2)

    '        'se l'intero costituito dalle 2 cifre è <= 50
    '        'faccio l'arrotondamento per difetto
    '        If CInt(Str_ParteDecimale) <= 50 Then
    '            Valore = Math.Floor(Valore)
    '        Else
    '            'se l'intero costituito dalle 2 cifre è > 50
    '            'faccio l'arrotondamento per eccesso
    '            Valore = Math.Ceiling(Valore)
    '        End If
    '        Return Valore

    '    Else
    '        'il numero è intero
    '        Return Valore
    '    End If



    'End Function


    ''#####################################################################################################
    'Public Sub Ricava_Specie_Varieta_FRG(ByVal Mat_Des As String, _
    '                                        ByRef Descr_Specie As String, _
    '                                        ByRef Descr_Varieta As String)

    '    If InStr(Mat_Des, "-") Then
    '        Descr_Specie = Mat_Des.Split("-")(0)
    '        Descr_Varieta = Mat_Des.Split("-")(1)
    '    Else
    '        Descr_Specie = Mat_Des
    '        Descr_Varieta = ""
    '    End If

    'End Sub


    ''##################################################################################
    'Public Sub Calcola_TotaliPrecedenti_RegCaricoScaricoPomodoro(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, _
    '                                                            ByVal Tipo_Registro As enum_TipoRegistroCaricoScaricoPomodoro, _
    '                                                            ByVal Piva As String, _
    '                                                            ByVal Validita_Inizio As String, _
    '                                                            ByVal Validita_Fine As String, _
    '                                                            ByRef TOT_PREC_Peso_Netto As Double, _
    '                                                            ByRef TOT_PREC_Degrado As Double, _
    '                                                            ByRef TOT_PREC_Netto_Pagamento As Double)

    '    Dim DT As DataTable
    '    Dim i As Integer

    '    Dim ADD As New AgronicaCoreStampeDAL.AccettazioneDaDiversi

    '    DT = ADD.CertificatiPomodoro_NettoDegrado_Leggi(Tipo_Registro, _
    '                                                          Piva, _
    '                                                          Validita_Inizio, _
    '                                                          Validita_Fine, _
    '                                                          "", "", _
    '                                                          objParametri_Server)

    '    'DT = NewCom_ADD_CertificatiPomodoro_NettoDegrado_Leggi(objServer, objSession, objPage, _
    '    '                                                            Tipo_Registro, _
    '    '                                                            Piva, _
    '    '                                                            Validita_Inizio, _
    '    '                                                            Validita_Fine)

    '    If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

    '        Dim Udm_Cod As Integer
    '        Dim Peso_Netto As Double
    '        Dim Degrado_Perc As Double
    '        Dim Degrado As Double
    '        Dim Netto_Pagamento As Double

    '        For i = 0 To DT.Rows.Count - 1

    '            Udm_Cod = DT.Rows(i).Item("Udm_Cod")
    '            Peso_Netto = RoundNumber_ParteIntera(DT.Rows(i).Item("Peso_Netto"))
    '            Degrado_Perc = DT.Rows(i).Item("Degrado_Perc")

    '            Calcola_NettoPagamento(Peso_Netto, Degrado_Perc, Udm_Cod, Degrado, Netto_Pagamento)

    '            TOT_PREC_Peso_Netto += Peso_Netto
    '            TOT_PREC_Degrado += Degrado
    '            TOT_PREC_Netto_Pagamento += Netto_Pagamento

    '        Next

    '    End If

    'End Sub

    ''##################################################################################
    'Public Function PaginaCertificatoPomodoro_from_IdAgenda(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, _
    '                                                        ByVal Piva As String, _
    '                                                        ByVal Id_Agenda As Integer) As String

    '    Dim Pagina_CertificatoPomodoro As String
    '    Dim objADD As New AgronicaCoreContabDAL.AccettazioneDaDiversi_R
    '    Dim Anno As Integer

    '    If Id_Agenda = 0 Then
    '        Throw New Exception("Id_Agenda = 0")
    '    End If

    '    Anno = objADD.AnnoCertificatoPomodoro_from_IdAgenda(Piva, Id_Agenda, _
    '                                                        objParametri_Server)

    '    Select Case Anno
    '        Case 0
    '            Throw New Exception("Anno non riconosciuto!")
    '        Case 2010
    '            Pagina_CertificatoPomodoro = "CertificatiPomodoro/CertificatiPomodoro.aspx"
    '        Case Else
    '            Pagina_CertificatoPomodoro = "CertificatiPomodoro2011/CertificatiPomodoro2011.aspx"
    '    End Select

    '    Return Pagina_CertificatoPomodoro

    'End Function

    ''##################################################################################
    'Public Function PaginaCertificatoPomodoro_from_Str_Id_Agenda(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, _
    '                                                                ByVal Piva As String, _
    '                                                                ByVal Str_Id_Agenda_Cert As String) As String

    '    Dim Pagina_CertificatoPomodoro As String = ""
    '    Dim objADD As New AgronicaCoreContabDAL.AccettazioneDaDiversi_R
    '    Dim Anno As Integer
    '    Dim MessaggioErrore As String

    '    If Str_Id_Agenda_Cert = "" Then
    '        Throw New Exception("Str_Id_Agenda_Cert = ''")
    '    End If

    '    Anno = objADD.AnnoCertificatiPomodoro_from_Str_Id_Agenda(Piva, _
    '                                                                Str_Id_Agenda_Cert, _
    '                                                                MessaggioErrore, _
    '                                                                objParametri_Server)

    '    If MessaggioErrore = "" Then

    '        Select Case Anno
    '            Case 0
    '                Throw New Exception("Anno non riconosciuto!")
    '            Case 2010
    '                Pagina_CertificatoPomodoro = "CertificatiPomodoro/CertificatiPomodoro.aspx"
    '            Case Else
    '                Pagina_CertificatoPomodoro = "CertificatiPomodoro2011/CertificatiPomodoro2011.aspx"
    '        End Select
    '    Else
    '        Throw New Exception(MessaggioErrore)
    '    End If

    '    Return Pagina_CertificatoPomodoro

    'End Function



    ''##################################################################################
    'Public Function PaginaExportCertificatiPomodoro_from_AnnoDataSelezionata(ByVal Data_Inizio As String) As String

    '    Dim PaginaExportCertificatiPomodoro As String
    '    'Dim objADD As New AgronicaCoreContabDAL.AccettazioneDaDiversi_R
    '    Dim Anno As Integer

    '    If IsDate(Data_Inizio) Then
    '        Anno = CDate(Data_Inizio).Year
    '    Else
    '        Throw New Exception("E' necessario impostare la data di inizio del filtro temporale!")
    '    End If

    '    'Anno = objADD.AnnoCertificatoPomodoro_from_IdAgenda(Qs_Piva, Qs_IdAgenda, _
    '    '                                                    objParametri_Server)

    '    Select Case Anno
    '        Case 2010
    '            PaginaExportCertificatiPomodoro = "ExportCertificatiPomodoro/CertificatiPomodoro_XLS.aspx"
    '        Case Else
    '            PaginaExportCertificatiPomodoro = "ExportCertificatiPomodoro2011/CertificatiPomodoro2011_XLS.aspx"
    '    End Select

    '    Return PaginaExportCertificatiPomodoro

    'End Function


    ''##################################################################################
    'Public Function PaginaExportAgreaCertificatiPomodoro_from_AnnoDataOdierna() As String

    '    Dim PaginaExportAgreaCertificatiPomodoro As String
    '    Dim Anno As Integer = Date.Today.Year

    '    Select Case Anno
    '        Case 2010
    '            PaginaExportAgreaCertificatiPomodoro = "GestioneEsportazioni/Esportazione_ConferimentiPomodoro_Agrea/Export_ConferimentiPomodoro_Agrea.aspx"
    '        Case Else
    '            PaginaExportAgreaCertificatiPomodoro = "GestioneEsportazioni/Esportazione_ConferimentiPomodoro_Agrea/Export_ConferimentiPomodoro_Agrea_2011.aspx"
    '    End Select

    '    Return PaginaExportAgreaCertificatiPomodoro

    'End Function



    '''##################################################################################
    ''Public Function Verifica_Filtro_Certificati(ByRef objServer As System.Web.HttpServerUtility, _
    ''                                            ByRef objSession As System.Web.SessionState.HttpSessionState, _
    ''                                            ByRef objPage As System.Web.UI.Page, _
    ''                                            ByRef Messaggio As String, _
    ''                                            ByVal Flag_FiltroSQLIdAgenda As Boolean, _
    ''                                            ByVal Piva As String, _
    ''                                            ByVal PrefissoDaNumeroCert As String, _
    ''                                            ByVal DaNumeroCert As String, _
    ''                                            ByVal SuffissoDaNumeroCert As String, _
    ''                                            ByVal PrefissoANumeroCert As String, _
    ''                                            ByVal ANumeroCert As String, _
    ''                                            ByVal SuffissoANumeroCert As String, _
    ''                                            ByVal Validita_Inizio As String, _
    ''                                            ByVal Validita_Fine As String) As String


    ''    Dim SringaFiltroCertificati As String = ""

    ''    Try

    ''        If DaNumeroCert <> "" And ANumeroCert <> "" Then

    ''            If Not IsNumeric(DaNumeroCert) Or Not IsNumeric(ANumeroCert) Then
    ''                Messaggio = "I numeri certificato devono essere numerici."
    ''                Exit Function
    ''            End If

    ''            ''se sono uguali, si filtra solo una specie
    ''            'If DaNumeroBolla <> ANumeroBolla Then

    ''            If CInt(ANumeroCert) < CInt(DaNumeroCert) Then
    ''                Messaggio = "Il Numero Certificato A deve essere maggiore rispetto al Numero Certificato DA"
    ''                Exit Function
    ''            End If

    ''            Prepara_StrFiltroCertificati(objServer, objSession, objPage, SringaFiltroCertificati, Messaggio, Flag_FiltroSQLIdAgenda, Piva, True, PrefissoDaNumeroCert, DaNumeroCert, SuffissoDaNumeroCert, PrefissoANumeroCert, ANumeroCert, SuffissoANumeroCert, Validita_Inizio, Validita_Fine)

    ''            'Else
    ''            '    'se sono uguali, si filtra solo una specie
    ''            '    Me.Txt_SringaFiltroBolle.Text = Str_NumBolla
    ''            'End If

    ''        Else
    ''            If Validita_Inizio <> "" And Validita_Fine <> "" Then
    ''                'ok, non si filtra per numero, ma solo per date
    ''                Prepara_StrFiltroCertificati(objServer, objSession, objPage, SringaFiltroCertificati, Messaggio, Flag_FiltroSQLIdAgenda, Piva, False, PrefissoDaNumeroCert, DaNumeroCert, SuffissoDaNumeroCert, PrefissoANumeroCert, ANumeroCert, SuffissoANumeroCert, Validita_Inizio, Validita_Fine)
    ''            Else
    ''                'è necessario impostare almeno un tipo di filtro
    ''                Messaggio = "E' necessario impostare almeno un criterio di filtro di stampa dei certificati (per intervallo temporale o per numero certificato)."
    ''            End If
    ''        End If

    ''    Catch ex As Exception
    ''        Messaggio += "Si è verificato il seguente errore: " + ex.Message
    ''    End Try

    ''    Return SringaFiltroCertificati


    ''End Function

    '''##################################################################################
    ''Private Sub Prepara_StrFiltroCertificati(ByRef objServer As System.Web.HttpServerUtility, _
    ''                                            ByRef objSession As System.Web.SessionState.HttpSessionState, _
    ''                                            ByRef objPage As System.Web.UI.Page, _
    ''                                            ByRef SringaFiltroCertificati As String, _
    ''                                            ByRef Messaggio As String, _
    ''                                            ByVal Flag_FiltroSQLIdAgenda As Boolean, _
    ''                                            ByVal Piva As String, _
    ''                                            ByVal Flag_FiltraNumCertificato As Boolean, _
    ''                                            ByVal PrefissoDaNumeroCert As String, _
    ''                                            ByVal DaNumeroCert As String, _
    ''                                            ByVal SuffissoDaNumeroCert As String, _
    ''                                            ByVal PrefissoANumeroCert As String, _
    ''                                            ByVal ANumeroCert As String, _
    ''                                            ByVal SuffissoANumeroCert As String, _
    ''                                            ByVal Validita_Inizio As String, _
    ''                                            ByVal Validita_Fine As String)


    ''    'compongo la stringa dei progressivi da filtrare
    ''    Dim DT_NumCert As DataTable
    ''    Dim i As Integer
    ''    Dim Str_NumCert As String = ""

    ''    'modifico la data
    ''    If Validita_Inizio = "" Or Validita_Inizio = Nothing Then
    ''        Validita_Inizio = AgronicaCoreDataProvider.CostantiPersonalizzate.STR_DATAINIZIO
    ''    End If

    ''    If Validita_Fine = "" Or Validita_Fine = Nothing Then
    ''        Validita_Fine = AgronicaCoreDataProvider.CostantiPersonalizzate.STR_DATAFINE

    ''    End If

    ''    DT_NumCert = Leggi_Range_IdAgenda_Certificati(objServer, objSession, objPage, _
    ''                                                    Piva, _
    ''                                                    Flag_FiltraNumCertificato, _
    ''                                                    PrefissoDaNumeroCert, _
    ''                                                    DaNumeroCert, _
    ''                                                    SuffissoDaNumeroCert, _
    ''                                                    PrefissoANumeroCert, _
    ''                                                    ANumeroCert, _
    ''                                                    SuffissoANumeroCert, _
    ''                                                    Validita_Inizio, _
    ''                                                    Validita_Fine)


    ''    If Not IsNothing(DT_NumCert) Then

    ''        If DT_NumCert.Rows.Count <> 0 Then

    ''            For i = 0 To DT_NumCert.Rows.Count - 1

    ''                If Flag_FiltroSQLIdAgenda = True Then
    ''                    If i <> DT_NumCert.Rows.Count - 1 Then
    ''                        Str_NumCert += CStr(DT_NumCert.Rows(i).Item("Id_Agenda")) + ","
    ''                    Else
    ''                        Str_NumCert += CStr(DT_NumCert.Rows(i).Item("Id_Agenda"))
    ''                    End If
    ''                Else
    ''                    If i <> DT_NumCert.Rows.Count - 1 Then
    ''                        Str_NumCert += CStr(DT_NumCert.Rows(i).Item("Id_Agenda")) + "|"
    ''                    Else
    ''                        Str_NumCert += CStr(DT_NumCert.Rows(i).Item("Id_Agenda"))
    ''                    End If
    ''                End If

    ''            Next

    ''            SringaFiltroCertificati = Str_NumCert

    ''        Else
    ''            Messaggio = "Non è stata trovata alcun certificato che soddisfi il criterio di filtro impostato."
    ''        End If

    ''    End If

    ''End Sub

    '''##################################################################################
    ''Private Function Leggi_Range_IdAgenda_Certificati(ByRef objServer As System.Web.HttpServerUtility, _
    ''                                                    ByRef objSession As System.Web.SessionState.HttpSessionState, _
    ''                                                    ByRef objPage As System.Web.UI.Page, _
    ''                                                    ByVal Piva As String, _
    ''                                                    ByVal Flag_FiltraNumCertificato As Boolean, _
    ''                                                    ByVal PrefissoDaNumeroCert As String, _
    ''                                                    ByVal DaNumeroCert As String, _
    ''                                                    ByVal SuffissoDaNumeroCert As String, _
    ''                                                    ByVal PrefissoANumeroCert As String, _
    ''                                                    ByVal ANumeroCert As String, _
    ''                                                    ByVal SuffissoANumeroCert As String, _
    ''                                                    ByVal Validita_Inizio As String, _
    ''                                                    ByVal Validita_Fine As String) As DataTable


    ''    Dim DT_NumCert As DataTable
    ''    Dim FiltroAggiuntivo As String = ""
    ''    Dim Ordinamento As String


    ''    If Flag_FiltraNumCertificato = True Then

    ''        Dim j As Integer
    ''        Dim ElencoNumeriBolla As String = ""
    ''        Dim ChiaveBolla As String = ""

    ''        For j = CInt(Num_Bolla_DA) To CInt(Num_Bolla_A)

    ''            ChiaveBolla = Prefisso_Num_Bolla_DA & "_" & CStr(j) & "_" & Suffisso_Num_Bolla_DA

    ''            ElencoNumeriBolla += ",'" & ChiaveBolla & "'"

    ''        Next

    ''        'Formatto correttamente l'elenco (tolgo la virgola iniziale ...)
    ''        ElencoNumeriBolla = Mid(ElencoNumeriBolla, 2)

    ''        FiltroAggiuntivo += " AND               ( ( Mov_Accett.Doc_Numero_Sin  "
    ''        FiltroAggiuntivo += "                     + '_' + CONVERT(varchar(10), Mov_Accett.Doc_Numero)  "
    ''        FiltroAggiuntivo += "                     + '_' + Mov_Accett.Doc_Numero_Des )  "
    ''        FiltroAggiuntivo += "                     IN (" & ElencoNumeriBolla & ")    )  "

    ''    End If

    ''    Ordinamento = " ORDER BY Mov_Accett.Doc_Numero, Mov_Accett.Doc_Numero_Sin, Mov_Accett.Doc_Numero_Des "


    ''    DT_NumBolla = NewCom_Accettazione_DaDiversi_IdAgenda_Leggi(objServer, objSession, objPage, _
    ''                                                                Piva, _
    ''                                                                Prefisso_Num_Bolla_DA, _
    ''                                                                FiltroAggiuntivo, _
    ''                                                                Ordinamento, _
    ''                                                                Validita_Inizio, _
    ''                                                                Validita_Fine)


    ''    Return DT_NumBolla


    ''End Function



End Module
