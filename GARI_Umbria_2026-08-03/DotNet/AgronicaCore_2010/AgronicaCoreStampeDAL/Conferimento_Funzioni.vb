Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class Conferimento_Funzioni

    '#####################################################################################################
    'StrCodiciProdotto è il contenuto della var Session("Str_Codici_Prodotto") che la pagina di filtro stampe conferimento passa ai report
    Public Function Ricava_FiltroMatCod_da_StrCodiciProdotto(ByVal StrCodiciProdotto As String) As String

        Dim StrFiltroSQL As String = ""
        If StrCodiciProdotto <> "" Then
            If StrCodiciProdotto.Contains("|") Then
                Dim VetCod As String() = StrCodiciProdotto.Split("|")
                For i = 0 To VetCod.Length - 1
                    If IsNumeric(VetCod(i)) Then
                        StrFiltroSQL &= VetCod(i)
                        If i <> VetCod.Length - 1 Then
                            StrFiltroSQL &= ", "
                        End If
                    End If
                Next
                If StrFiltroSQL <> "" Then
                    StrFiltroSQL = " AND MP_Raccolta.Mat_Cod IN (" & StrFiltroSQL & ")"
                End If
            End If
        End If
        Return StrFiltroSQL

    End Function

    '#####################################################################################################
    'StrCodiciContatto è il contenuto della var Session("Str_CodRisUm_Conferenti") che la pagina di filtro stampe conferimento passa ai report
    Public Function Ricava_FiltroCodRisUm_da_StrCodiciContatto(ByVal StrCodiciContatto As String) As String

        Dim StrFiltroSQL As String = ""
        If StrCodiciContatto <> "" Then
            If StrCodiciContatto.Contains("|") Then
                Dim VetCod As String() = StrCodiciContatto.Split("|")
                For i = 0 To VetCod.Length - 1
                    If IsNumeric(VetCod(i)) Then
                        StrFiltroSQL &= VetCod(i)
                        If i <> VetCod.Length - 1 Then
                            StrFiltroSQL &= ", "
                        End If
                    End If
                Next
                If StrFiltroSQL <> "" Then
                    StrFiltroSQL = " AND Mov_Accett.Cod_RisUm IN (" & StrFiltroSQL & ")"
                End If
            End If
        End If
        Return StrFiltroSQL

    End Function


    '#####################################################################################################
    Public Sub Ricava_Specie_Varieta_FRG(ByVal Mat_Des As String,
                                            ByRef Descr_Specie As String,
                                            ByRef Descr_Varieta As String)

        If InStr(Mat_Des, "-") Then
            Descr_Specie = Mat_Des.Split("-")(0)
            Descr_Varieta = Mat_Des.Substring(Mat_Des.IndexOf("-") + 2)
        Else
            Descr_Specie = Mat_Des
            Descr_Varieta = ""
        End If

    End Sub

    '#####################################################################################################
    Public Function Calcola_TaraImballi(ByVal Tara_Imballi_Vuoti As Decimal,
                                        ByVal Tara_Imballi_Riga As Decimal,
                                        ByVal Num_Righe_Bolla As Integer) As Decimal

        'rif mail "R: lamentele da Fruttagel -> test cambio modalità insert imballi" - lunedì 18/01/2021 10:58
        'Sui report di riepilogo posso anche fare che se: 
        '-	Tara <> 0 sul dettaglio -> stampa quella tara
        '-	Tara = 0 allora per quella bolla (id_agenda) visualizza la tara ricavata dalla SOMMA delle tare degli imballi x numero imballi per ogni imballo movimentato in ENTRATA su quella bolla.
        'Potresti avere anche il caso misto, ossia In presenza di più tipi imballo mettono il primo tipo sulla riga prodotto e gli altri tipi li caricano come vuoti, questo a meno che non andiamo a mettere dei vincoli per bloccare questa possibilità
        'In presenza di una sola riga prodotto, non puoi semplicemente sommare la tara del dettaglio con la tara di quelli vuoti?
        'Invece in presenza di più prodotti secondo me ti tocca non considerare le tare degli imballi vuoti

        Dim TaraImballi As Decimal = 0

        If Num_Righe_Bolla = 1 Then
            'se c'è una sola riga prodotto -> sommare la tara del dettaglio con le tare imballi vuoti
            TaraImballi = Tara_Imballi_Riga + Tara_Imballi_Vuoti
        Else
            'se ci sono più prodotti non bisogna considerare le tare degli imballi vuoti
            TaraImballi = Tara_Imballi_Riga
        End If

        TaraImballi = AgronicaCoreDataProvider.Agro_Math.ArrotondaVal_0(TaraImballi)

        Return TaraImballi

    End Function

    '##################################################################################
    'Viene utilizzata dalla pagina di filtro Filtro_Stampe_Conf.aspx
    'probabilmente inutile se questi controlli li fa il JS (nel caso basta chiamare la Prepara_StrFiltroBolle)
    '----------
    'il commento sotto è quello della funzione dalla quale è stata copiata:
    'nota del 02/02/2015: aggiunto parametro ReportSelezionato (prima lo leggeva dalla sessione)
    'Tipo_Report serve per indicare se si tratta di bolle di accettazione o di certificati
    Public Function Verifica_Filtro_Bolle(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                            ByRef Messaggio As String,
                                            ByVal ReportSelezionato As Integer,
                                            ByVal Flag_FiltroSQLIdAgenda As Boolean,
                                            ByVal Piva As String,
                                            ByVal Centro_Aziendale As Integer,
                                            ByVal PrefissoNumeroBolla As String,
                                            ByVal SuffissoNumeroBolla As String,
                                            ByVal DaNumeroBolla As String,
                                            ByVal ANumeroBolla As String,
                                            ByVal Validita_Inizio As String,
                                            ByVal Validita_Fine As String,
                                            ByVal Codice_Conferente As Integer,
                                            ByVal Prodotto_Cod As Integer,
                                            ByVal Codice_Specie As Integer,
                                            ByVal Codice_Varieta As Integer,
                                            ByVal FiltroMultipliProdotti As String,
                                            ByVal FiltroMultipliConferenti As String,
                                            ByVal Piva_Produttore As String,
                                            ByVal Piva_Coop1 As String,
                                            ByVal Piva_Coop2 As String) As String


        Dim StringaFiltroBolle As String = ""
        Dim label As String = ""
        Dim label2 As String = ""

        Select Case ReportSelezionato
            Case enum_CodificaStampe.FreshFood_BollaAccettazione
                label = "Bolla"
                label2 = "delle Bolle"
            Case enum_CodificaStampe.Conf_Certificato_Pomodoro
                label = "Certificato"
                label2 = "dei Certificati"
        End Select

        Try

            If DaNumeroBolla <> "" AndAlso ANumeroBolla <> "" Then

                If Not IsNumeric(DaNumeroBolla) OrElse Not IsNumeric(ANumeroBolla) Then
                    Messaggio = "I numeri " & label & " devono essere numerici."
                    Exit Function
                End If

                ''se sono uguali, si filtra solo una specie
                'If DaNumeroBolla <> ANumeroBolla Then

                If CInt(ANumeroBolla) < CInt(DaNumeroBolla) Then
                    Messaggio = "Il Numero " & label & " A deve essere maggiore rispetto al Numero " & label & " DA"
                    Exit Function
                End If

                Prepara_StrFiltroBolle(objParametri_Server,
                                        StringaFiltroBolle,
                                        Messaggio,
                                        ReportSelezionato,
                                        Flag_FiltroSQLIdAgenda,
                                        Piva,
                                        Centro_Aziendale,
                                        True,
                                        PrefissoNumeroBolla,
                                        SuffissoNumeroBolla,
                                        DaNumeroBolla,
                                        ANumeroBolla,
                                        Validita_Inizio, Validita_Fine,
                                        Codice_Conferente,
                                        Prodotto_Cod,
                                        Codice_Specie,
                                        Codice_Varieta,
                                        FiltroMultipliProdotti,
                                        FiltroMultipliConferenti,
                                        Piva_Produttore,
                                        Piva_Coop1,
                                        Piva_Coop2)


                'Else
                '    'se sono uguali, si filtra solo una specie
                '    Me.Txt_SringaFiltroBolle.Text = Str_NumBolla
                'End If

            Else
                If Validita_Inizio <> "" AndAlso Validita_Fine <> "" Then
                    'ok, non si filtra per numero, ma solo per date
                    Prepara_StrFiltroBolle(objParametri_Server,
                                        StringaFiltroBolle,
                                        Messaggio,
                                        ReportSelezionato,
                                        Flag_FiltroSQLIdAgenda,
                                        Piva,
                                        Centro_Aziendale,
                                        False,
                                        PrefissoNumeroBolla,
                                        SuffissoNumeroBolla,
                                        DaNumeroBolla,
                                        ANumeroBolla,
                                        Validita_Inizio, Validita_Fine,
                                        Codice_Conferente,
                                        Prodotto_Cod,
                                        Codice_Specie,
                                        Codice_Varieta,
                                        FiltroMultipliProdotti,
                                        FiltroMultipliConferenti,
                                        Piva_Produttore,
                                        Piva_Coop1,
                                        Piva_Coop2)

                Else
                    'è necessario impostare almeno un tipo di filtro
                    Messaggio = "E' necessario impostare almeno un criterio di filtro di stampa " & label2 & " (per intervallo temporale o per numero " & label & ")."
                End If
            End If

        Catch ex As Exception
            Messaggio &= "Si è verificato il seguente errore: " & ex.Message
        End Try

        Return StringaFiltroBolle

    End Function

    '##################################################################################
    Public Sub Prepara_StrFiltroBolle(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef StringaFiltroBolle As String,
                                        ByRef Messaggio As String,
                                        ByVal ReportSelezionato As Integer,
                                        ByVal Flag_FiltroSQLIdAgenda As Boolean,
                                        ByVal Piva As String,
                                        ByVal Centro_Aziendale As Integer,
                                        ByVal Flag_FiltraNumBolla As Boolean,
                                        ByVal PrefissoNumeroBolla As String,
                                        ByVal SuffissoNumeroBolla As String,
                                        ByVal DaNumeroBolla As String,
                                        ByVal ANumeroBolla As String,
                                        ByVal Validita_Inizio As String,
                                        ByVal Validita_Fine As String,
                                        ByVal Codice_Conferente As Integer,
                                        ByVal Prodotto_Cod As Integer,
                                        ByVal Codice_Specie As Integer,
                                        ByVal Codice_Varieta As Integer,
                                        ByVal FiltroMultipliProdotti As String,
                                        ByVal FiltroMultipliConferenti As String,
                                        ByVal Piva_Produttore As String,
                                        ByVal Piva_Coop1 As String,
                                        ByVal Piva_Coop2 As String)


        'compongo la stringa dei progressivi da filtrare
        Dim DT_NumBolla As DataTable
        Dim i As Integer
        Dim Str_NumBolla As String = ""

        Dim label As String = ""

        Select Case ReportSelezionato
            Case enum_CodificaStampe.FreshFood_BollaAccettazione
                label = "Non è stata trovata alcuna Bolla"
            Case enum_CodificaStampe.Conf_Certificato_Pomodoro
                label = "Non è stato trovato alcun Certificato"
        End Select

        'modifico la data
        If Validita_Inizio = "" OrElse Validita_Inizio = Nothing Then
            Validita_Inizio = AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO.ToShortDateString
        End If

        If Validita_Fine = "" OrElse Validita_Fine = Nothing Then
            Validita_Fine = AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAFINE.ToShortDateString

        End If

        DT_NumBolla = Leggi_Range_IdAgenda_Bolle(objParametri_Server,
                                            ReportSelezionato,
                                            Piva,
                                            Centro_Aziendale,
                                            Flag_FiltraNumBolla,
                                            PrefissoNumeroBolla,
                                            SuffissoNumeroBolla,
                                            DaNumeroBolla,
                                            ANumeroBolla,
                                            Validita_Inizio,
                                            Validita_Fine,
                                            Codice_Conferente,
                                            Prodotto_Cod,
                                            Codice_Specie,
                                            Codice_Varieta,
                                            FiltroMultipliProdotti,
                                            FiltroMultipliConferenti,
                                            Piva_Produttore,
                                            Piva_Coop1,
                                            Piva_Coop2)


        If Not IsNothing(DT_NumBolla) Then

            If DT_NumBolla.Rows.Count <> 0 Then

                For i = 0 To DT_NumBolla.Rows.Count - 1

                    If Flag_FiltroSQLIdAgenda Then
                        If i <> DT_NumBolla.Rows.Count - 1 Then
                            Str_NumBolla &= CStr(DT_NumBolla.Rows(i).Item("Id_Agenda")) & ","
                        Else
                            Str_NumBolla &= CStr(DT_NumBolla.Rows(i).Item("Id_Agenda"))
                        End If
                    Else
                        If i <> DT_NumBolla.Rows.Count - 1 Then
                            Str_NumBolla &= CStr(DT_NumBolla.Rows(i).Item("Id_Agenda")) & "|"
                        Else
                            Str_NumBolla &= CStr(DT_NumBolla.Rows(i).Item("Id_Agenda"))
                        End If
                    End If

                Next

                StringaFiltroBolle = Str_NumBolla

            Else
                Messaggio = label & " che soddisfi il criterio di filtro impostato."
            End If

        End If

    End Sub

    '##################################################################################
    Private Function Leggi_Range_IdAgenda_Bolle(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                ByVal ReportSelezionato As Integer,
                                                ByVal Piva As String,
                                                ByVal Sa_Cod As Integer,
                                                ByVal Flag_FiltraNumBolla As Boolean,
                                                 ByVal Prefisso_Num_Bolla As String,
                                                ByVal Suffisso_Num_Bolla As String,
                                                ByVal Num_Bolla_DA As String,
                                                ByVal Num_Bolla_A As String,
                                                ByVal Data_Inizio As String,
                                                ByVal Data_Fine As String,
                                                ByVal Cod_RisUm As Integer,
                                                ByVal Mat_Cod As Integer,
                                                ByVal Veg_Cod As Integer,
                                                ByVal Cul_Cod As Integer,
                                                ByVal FiltroAggArticoli As String,
                                                ByVal FiltroAggConferenti As String,
                                                ByVal Piva_Produttore As String,
                                                ByVal Piva_Coop1 As String,
                                                ByVal Piva_Coop2 As String) As DataTable


        Dim DT_NumBolla As New DataTable
        Dim xOrderBy As String
        Dim ElencoNumeriBolla As String = ""

        If Flag_FiltraNumBolla Then

            For j = CInt(Num_Bolla_DA) To CInt(Num_Bolla_A)
                ElencoNumeriBolla &= "," & CStr(j)
            Next

            'Formatto correttamente l'elenco (tolgo la virgola iniziale ...)
            ElencoNumeriBolla = Mid(ElencoNumeriBolla, 2)

        End If


        Select Case ReportSelezionato

            Case enum_CodificaStampe.FreshFood_BollaAccettazione

                'x il core
                xOrderBy = " Mov_Accett.Doc_Numero, Mov_Accett.Doc_Numero_Sin, Mov_Accett.Doc_Numero_Des "

                Dim objFF As New AgronicaCoreStampeDAL.FreshAndFood
                DT_NumBolla = objFF.Bolle_IdAgenda_DocNumero_Leggi(Piva,
                                                                   Sa_Cod,
                                                                    0,
                                                                    0,
                                                                    True,
                                                                    Prefisso_Num_Bolla,
                                                                    Suffisso_Num_Bolla,
                                                                    ElencoNumeriBolla,
                                                                    Data_Inizio,
                                                                    Data_Fine,
                                                                   Cod_RisUm,
                                                                     Mat_Cod,
                                                                     Veg_Cod,
                                                                     Cul_Cod,
                                                                     FiltroAggArticoli,
                                                                     FiltroAggConferenti,
                                                                    Piva_Produttore,
                                                                    Piva_Coop1,
                                                                    Piva_Coop2,
                                                                    xOrderBy,
                                                                     objParametri_Server)

            Case enum_CodificaStampe.Conf_Certificato_Pomodoro

                '''Ordinamento = " ORDER BY Mov_Certificato.Doc_Numero, Mov_Certificato.Doc_Numero_Sin, Mov_Certificato.Doc_Numero_Des "
                ''Ordinamento = " ORDER BY Mov_Accett.Doc_Numero, Mov_Accett.Doc_Numero_Sin, Mov_Accett.Doc_Numero_Des "

                xOrderBy = " Numero_Bolla "

                'Dim Tipo_Certificato As enum_CodificaStampe
                'If IsNumeric(ReportSelezionato) Then
                '    Tipo_Certificato = ReportSelezionato.ToString
                'Else
                '    'non è valorizzato il report nella sessione
                '    'imposto come default il cert. interno
                '    Tipo_Certificato = enum_CodificaStampe.Certificato_Pomodoro_Interno
                'End If

                Dim objConfPomo As New AgronicaCoreStampeDAL.Conferimento_Pomodoro
                DT_NumBolla = objConfPomo.CertificatiPomodoro_IdAgenda_Leggi(Piva,
                                                                            Sa_Cod,
                                                                            Prefisso_Num_Bolla,
                                                                            Suffisso_Num_Bolla,
                                                                            ElencoNumeriBolla,
                                                                            Data_Inizio,
                                                                            Data_Fine,
                                                                            Cod_RisUm,
                                                                            Piva_Produttore,
                                                                            Piva_Coop1,
                                                                            Mat_Cod,
                                                                            Veg_Cod,
                                                                            Cul_Cod,
                                                                            FiltroAggArticoli,
                                                                            FiltroAggConferenti,
                                                                            "", xOrderBy,
                                                                            objParametri_Server)

        End Select

        Return DT_NumBolla


    End Function

End Class
