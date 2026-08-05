Imports System.Data
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreModelsSTD
Public Class STD_Limitazioni

    Public Function LeggiLocalizzazioni(ByVal Disciplinare_Cod As Integer,
                                        ByVal Id_RcDpi As Integer,
                                        ByVal Fr_Cod As Integer,
                                        ByVal strPa_Cod As String,
                                        ByVal TipoTestata As Integer,
                                        ByVal Veg_Cod As Integer,
                                        ByVal Av_Cod As Integer,
                                        ByVal Av_Gru As Integer,
                                        ByVal Avversita_Filtro As String,
                                        ByVal Data As Date,
                                        ByVal NomeUtente As String,
                                        ByRef strErr As String,
                                        objParametri_Super_Server As AgronicaCoreParametri,
                                        objParametri_Server As AgronicaCoreParametri,
                                        objParametri_Utenti As AgronicaCoreParametri) As DataTable

        Dim DTLocalizzazioni As New DataTable

        'Dim DtRisultati As DataTable

        'Richiamo caricamento dosi etichetta da banche dati
        DTLocalizzazioni = AgronicaCoreWebService.Limitazioni_WS.Localizzazioni_Elenco(Disciplinare_Cod,
                                                                                        Id_RcDpi,
                                                                                        Fr_Cod,
                                                                                        strPa_Cod,
                                                                                        TipoTestata,
                                                                                        Veg_Cod,
                                                                                        Av_Cod,
                                                                                        Av_Gru,
                                                                                        Avversita_Filtro,
                                                                                        Data,
                                                                                        NomeUtente,
                                                                                        objParametri_Super_Server,
                                                                                        objParametri_Server,
                                                                                        objParametri_Utenti,
                                                                                        strErr)



        Return DTLocalizzazioni

    End Function


    Public Function LeggiLocalizzazioni_Modello(ByVal lavorazione As attivita.Lavorazione,
                                                ByVal dettaglioTrattamento As attivita.dettagli.DettaglioTrattamento,
                                                ByVal disciplinare As metaschema.Disciplinare,
                                                ByVal avversitaGruppo As metaschema.avversita.AvversitaGruppo,
                                                ByVal specie As metaschema.utilizzi.Specie,
                                                ByRef strErr As String,
                                                ByVal objParametri_Super_Server As AgronicaCoreParametri,
                                                ByVal objParametri_Server As AgronicaCoreParametri,
                                                ByVal objParametri_Utenti As AgronicaCoreParametri) As List(Of metaschema.Localizzazione)

        Dim localizzazioniList As New List(Of metaschema.Localizzazione)

        Dim Dpi_Cod As Integer = 0
        Dim IdRcdpi As Integer = 0
        Dim Flag_PubblicoPrivato As Integer = 0
        Dim strAvversita As String = ""
        Dim Av_Cod As Integer = 0
        Dim Av_Gru As Integer = 0

        Dim Lav_Cod As Integer = CInt(lavorazione.primaryKey.codice)

        Dim Fr_Cod As Integer = dettaglioTrattamento.prodotto.codice

        Dim Veg_Cod As Integer = specie.codice

        If Not IsNothing(disciplinare) Then
            Dpi_Cod = disciplinare.codice

            'TODO Chiedere se va bene così
            IdRcdpi = disciplinare.raggruppamentiColturaliDPI.codice
            Flag_PubblicoPrivato = disciplinare.disciplinarePubblicoPrivato

            If Flag_PubblicoPrivato = 2 Then
                Dpi_Cod = -Dpi_Cod
            End If
        End If


        Dim TipoTestata As Integer = 0
        Select Case Lav_Cod
            Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO,
                 LAVCOD_TRATTAMENTO_FITOREGOLATORE,
                 LAVCOD_CONCIA_SEME,
                 LAVCOD_GEODISINFESTAZIONE,
                 LAVCOD_CONFUSIONE_SESSUALE,
                 LAVCOD_DISORIENTAMENTO_SESSUALE
                TipoTestata = 0
            Case LAVCOD_DISERBO
                TipoTestata = 1
            Case LAVCOD_DISSECCAMENTO
                TipoTestata = 0
        End Select

        strAvversita = ""

        'If ddl_avversita.SelectedValue = "" Then

        'Else
        '    If Split(ddl_avversita.SelectedValue, "|")(0) <> "0" Then
        '        Av_Cod = Split(ddl_avversita.SelectedValue, "|")(0)
        '    Else
        '        Av_Gru = Split(ddl_avversita.SelectedValue, "|")(1)
        '    End If

        'End If

        If Not IsNothing(avversitaGruppo) Then

            If avversitaGruppo.classType.Equals(costanti.ClassType.Avversita) Then
                Av_Cod = avversitaGruppo.codice
            ElseIf avversitaGruppo.classType.Equals(costanti.ClassType.GruppoAvversita) Then
                Av_Gru = avversitaGruppo.codice
            End If

            ''------------------------------------------------------------------------------
            ''AVVERSITA' SINGOLE
            ''------------------------------------------------------------------------------

            strAvversita = strAvversita & " Av_Cod=" & avversitaGruppo.codice

        End If


        Dim DTLocalizzazioni As DataTable = AgronicaCoreWebService.Limitazioni_WS.Localizzazioni_Elenco(CInt(Dpi_Cod),
                                                                                                         CInt(IdRcdpi),
                                                                                                         CInt(Fr_Cod),
                                                                                                         "",
                                                                                                         CInt(TipoTestata),
                                                                                                         CInt(Veg_Cod),
                                                                                                         CInt(Av_Cod),
                                                                                                         CInt(Av_Gru),
                                                                                                         CStr(strAvversita),
                                                                                                         CDate(Now),
                                                                                                         objParametri_Utenti.UtenteUsername,
                                                                                                         objParametri_Super_Server,
                                                                                                         objParametri_Server,
                                                                                                         objParametri_Utenti,
                                                                                                         strErr)


        If Not IsNothing(DTLocalizzazioni) AndAlso DTLocalizzazioni.Rows.Count > 0 Then
            For Each r As DataRow In DTLocalizzazioni.Rows
                localizzazioniList.Add(New metaschema.Localizzazione(r("TL_COD")) With
                                       {
                                            .descrizione = r("TL_DES")
                                       })
            Next
        End If


        Return localizzazioniList

    End Function

End Class
