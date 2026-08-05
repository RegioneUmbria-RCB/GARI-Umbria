Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider.TipiEnumerativi


Public Class Categorie_Magazzino_R
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function NomeComune_from_ElemCod(ByVal Elem_Cod As Integer,
                                            ByRef objParametri As AgronicaCoreParametri
                                            ) As String

        Dim dt As DataTable

        dt = Leggi(CInt(Elem_Cod), "", False,
                   "", "", objParametri)

        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            Return dt.Rows(0).Item("NomeComune")
        End If

        Return ""

    End Function

    '##############################################################################################
    'default Flag_Cantina As Boolean = False
    'visto che la query filtra sulle data inizio e fine in base alla finestra temporale dell'utente
    'per fare in modo di scartare la categoria coadiuvanti bisogna fare due operazioni sull'objParametri:
    'PRIMA DELLA QUERY: objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(Date.Today, Date.Today)
    'DOPO LA QUERY: objParametri_Server.ResettaFinestra()
    Public Function Leggi(ByVal Elem_Cod As Integer,
                          ByVal Cau_Mov As String,
                          ByVal Flag_Cantina As Boolean,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreMetaschemaDAL.Categorie_Magazzino_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  CategorieMagazzino ")
            StrSQL.Append(" WHERE CategorieMagazzino.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   CategorieMagazzino.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Not Flag_Cantina Then
                StrSQL.Append(" AND  ( Elem_Cod > 0 ) " & vbCrLf)
            End If

            If Elem_Cod <> 0 Then
                StrSQL.Append(" AND   (Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & ")   " & vbCrLf)
            End If

            If Cau_Mov <> "" AndAlso Cau_Mov <> CAU_ANIMALE Then
                StrSQL.Append(" AND   (Elem_Cod <> " & CStr(ZOO_CONSISTENZA) & ")   " & vbCrLf)
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND     Inviato >= 0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND     Inviato = -1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY NomeComune ASC")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function



    '###############################################################################
    Public Function LeggiTabella_da_CategorieMagazzino(ByVal NomeTabella As String,
                                                       ByVal NomeCodice As String,
                                                       ByVal NomeDescrizione As String,
                                                       ByVal TestoRicerca As String,
                                                       ByVal Codice As Integer,
                                                       ByVal xFiltroAggiuntivo As String,
                                                       ByVal xOrderBy As String,
                                                       ByRef objParametri As AgronicaCoreParametri
                                                       ) As DataTable

        Const nomeRoutine = "AgronicaCoreMetaschemaDAL.Categorie_Magazzino_R.LeggiTabella_da_CategorieMagazzino()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            '---------------------------------------------

            StrSQL.Length = 0
            StrSQL.Append(" SELECT  * " & vbCrLf)
            StrSQL.Append(" FROM " & NomeTabella & vbCrLf)
            StrSQL.Append(" WHERE " & NomeDescrizione & " LIKE '%" & Agro_SQL_SaveText(TestoRicerca) & "%' " & vbCrLf)

            If NomeCodice <> "" AndAlso Codice <> 0 Then
                StrSQL.Append(" AND " & NomeCodice & " = " & Agro_SQL_SaveNum(Codice) & " " & vbCrLf)
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND     Inviato >= 0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND     Inviato = -1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function



    '##############################################################################################
    Public Function ProDes_from_ProCod(ByVal Elem_Cod As Integer,
                                       ByVal Pro_Cod As Integer,
                                       ByRef objParametri As AgronicaCoreParametri
                                       ) As String

        Dim nomeTabella As String
        Dim nomeCodice As String
        Dim dt As DataTable
        Dim dtProdotti As DataTable

        Dim objProdotti As New AgronicaCoreAnagrafeDAL.Prodotti_R

        dt = Leggi(CInt(Elem_Cod), "", False,
                   "", "", objParametri)

        If dt.Rows.Count > 0 Then
            nomeTabella = dt.Rows(0).Item("Tabella")
            nomeCodice = dt.Rows(0).Item("Tabella_Cod")

            dtProdotti = objProdotti.Leggi(CStr(nomeTabella),
                                           CStr(nomeCodice),
                                           CInt(Pro_Cod),
                                           enumSelezioneVariabile.Selezione_TabellaCompleta,
                                           "",
                                           "",
                                           objParametri)

            If dtProdotti.Rows.Count > 0 Then
                Return dtProdotti.Rows(0).Item(dt.Rows(0).Item("Tabella_Des"))
            End If

            dtProdotti = Nothing

        End If

        dt = Nothing

    End Function


    '######################################################################################
    ' Legge le tabelle dei prodotti della banca dati (per i fertilizzanti legge anche la tabella materie_prime)
    '--- Obbligatori:
    'Elem_cod da specificare
    '--- Facoltativi:
    'Piva: serve per la tabella materie prime dei fert aziendali
    'pro_cod e mat_cod (se=0 vengono letti tutti i prodotti)
    'Ricerca_Des = filtro sulla descrizione dei prodotti
    'Flag_FiltraFormulati = nel caso di debbano leggere tutti i fito, si può scegliere se filtrare i revocati
    'DataFiltroFormulati = data per il filtro formulati
    '--- Riferimento:
    'Categoria = descrizione categoria magazzino
    'NomeCodice = nome del codice della tabella (fr_cod, fer_cod, ecc)
    'NomeDescrizione = nome della descrizione (fr_des, fer_des, ecc)
    Public Function Leggi_Prodotti_Parametrizzata(ByVal Piva As String,
                                                  ByVal Elem_Cod As Integer,
                                                  ByVal Pro_Cod As Integer,
                                                  ByVal Mat_Cod As Integer,
                                                  ByVal Ricerca_Des As String,
                                                  ByVal Flag_FiltraFormulati As Boolean,
                                                  ByVal DataFiltroFormulati As Date,
                                                  ByRef Categoria As String,
                                                  ByRef NomeCodice As String,
                                                  ByRef NomeDescrizione As String,
                                                  ByRef objParametri As AgronicaCoreParametri
                                                  ) As DataTable

        Dim dtCategorie As DataTable
        Dim dtProdotti As DataTable
        Dim i As Integer
        'Dim j As Integer
        'Dim x_Pro_Cod As Integer
        'Dim x_Pro_Des As String
        Dim NomeTabella As String

        'legge le categorie di magazzino
        dtCategorie = Leggi(Elem_Cod, CAU_MAGAZZINO, False,
                            "", "", objParametri)


        'la categoria di magazzino è 1
        For i = 0 To dtCategorie.Rows.Count - 1

            NomeTabella = dtCategorie.Rows(i).Item("Tabella")
            NomeCodice = dtCategorie.Rows(i).Item("Tabella_Cod")
            NomeDescrizione = dtCategorie.Rows(i).Item("Tabella_Des")
            Categoria = dtCategorie.Rows(i).Item("NomeComune")

            Select Case Elem_Cod

                Case FERTILIZZANTI

                    'per i fertilizzanti non uso più la funzione NewCom_LeggiTabella_da_CategorieMagazzino
                    'perché oltre alla tabella fertilizzanti, devo leggere anche la tabella materie_prime 
                    '(per i fertilizzanti aziendali)
                    Dim objFert As New AgronicaCoreMetaSchemaDAL.Fertilizzanti_R

                    dtProdotti = objFert.Leggi_Completa(Pro_Cod,
                                                        Ricerca_Des,
                                                        0,
                                                        True,
                                                        Piva,
                                                        Mat_Cod,
                                                        objParametri.FinestraTemporaleInizio,
                                                        objParametri.FinestraTemporaleFine,
                                                        "", "",
                                                        objParametri)

                Case Else

                    Select Case NomeCodice
                        Case "Mat_Cod"
                            dtProdotti = LeggiTabella_da_CategorieMagazzino(NomeTabella,
                                                                            NomeCodice,
                                                                            NomeDescrizione,
                                                                            Ricerca_Des,
                                                                            Mat_Cod,
                                                                            "", "",
                                                                            objParametri)
                        Case Else
                            dtProdotti = LeggiTabella_da_CategorieMagazzino(NomeTabella,
                                                                            NomeCodice,
                                                                            NomeDescrizione,
                                                                            Ricerca_Des,
                                                                            Pro_Cod,
                                                                            "", "",
                                                                            objParametri)
                    End Select

            End Select


            If Not IsNothing(dtProdotti) Then

                '(03/08/2015) commentato perché i dati delle revoca etc non sono piu in locale
                'Select Case Elem_Cod
                '    Case FORMULATI
                '        If Flag_FiltraFormulati = True Then
                '            Dim objFito As New AgronicaCoreMetaSchemaDAL.Formulati_R
                '            'nel caso dei formulati devo filtrare per evitare di visualizzare prodotti revocati, ecc
                '            Dt_Prodotti = objFito.Filtra_Formulati(Dt_Prodotti, DataFiltroFormulati, objParametri)
                '        End If
                'End Select

                'For j = 0 To Dt_Prodotti.Rows.Count - 1

                '    Select Case Elem_Cod

                '        Case FERTILIZZANTI
                '            'value salvato nella modalità gestita dalla FormProdotto
                '            If Dt_Prodotti.Rows(j).Item("Fer_Cod") <> 0 Then
                '                x_Pro_Cod = Dt_Prodotti.Rows(j).Item("Fer_Cod")
                '            ElseIf Dt_Prodotti.Rows(j).Item("Mat_Cod") <> 0 Then
                '                x_Pro_Cod = -Dt_Prodotti.Rows(j).Item("Mat_Cod")
                '            Else
                '                'errore, qui non dovrebbe mai entrare
                '                x_Pro_Cod = 0
                '            End If

                '        Case Else
                '            x_Pro_Cod = Dt_Prodotti.Rows(j).Item(NomeCodice)

                '    End Select

                '    If Not IsDBNull(x_Pro_Cod) Then

                '        If Flag_VisualizzaProCod = True Then
                '            x_Pro_Des = CStr(Dt_Prodotti.Rows(j).Item(NomeDescrizione)) & _
                '                         " (" & CStr(Math.Abs(x_Pro_Cod)) + ")"
                '        Else
                '            x_Pro_Des = Dt_Prodotti.Rows(j).Item(NomeDescrizione)
                '        End If

                '    End If

                'Next

            End If

        Next

        Return dtProdotti

    End Function



    '######################################################################################
    'Recupera la descrizione del prodotto
    '--- Obbligatori:
    'Elem_cod da specificare
    'Pro_cod (x i prod da banca dati)
    'Mat_Cod (x i fert azi)
    'piva (x i fert azi)
    '--- Facoltativi:
    'Flag_VisualizzaProCod (x visualizzare il codice nella descrizione)
    '--- Riferimento:
    'Categoria = descrizione categoria magazzino
    Public Function Prodotto_Des(ByVal Piva As String,
                                 ByVal Elem_Cod As Integer,
                                 ByVal Pro_Cod As Integer,
                                 ByVal Mat_Cod As Integer,
                                 ByVal Flag_VisualizzaProCod As Boolean,
                                 ByRef Categoria As String,
                                 ByRef objParametri As AgronicaCoreParametri
                                 ) As String

        Dim dtProd As DataTable
        Dim x_Pro_Des As String = ""
        Dim x_Pro_Cod As Integer = 0
        Dim nomeCodice As String = ""
        Dim nomeDescrizione As String = ""

        dtProd = Leggi_Prodotti_Parametrizzata(Piva,
                                               Elem_Cod, Pro_Cod, Mat_Cod,
                                               "",
                                               False,
                                               AGRODATAINIZIO,
                                               Categoria,
                                               nomeCodice,
                                               nomeDescrizione,
                                               objParametri)


        If Not IsNothing(dtProd) AndAlso dtProd.Rows.Count > 0 Then

            x_Pro_Cod = dtProd.Rows(0).Item(nomeCodice)

            If Flag_VisualizzaProCod Then
                x_Pro_Des = CStr(dtProd.Rows(0).Item(nomeDescrizione)) &
                             " (" & CStr(Math.Abs(x_Pro_Cod)) & ")"
            Else
                x_Pro_Des = dtProd.Rows(0).Item(nomeDescrizione)
            End If

        End If

        Return x_Pro_Des

    End Function


    '################################################################################
    Public Function CategoriaMagazzino_from_Elem_Cod(ByVal Elem_Cod As Integer,
                                                     ByRef objParametri As AgronicaCoreParametri
                                                     ) As String

        Dim dt As DataTable
        dt = Leggi(Elem_Cod, "", True,
                   "", "", objParametri)

        If Not IsNothing(dt) AndAlso dt.Rows.Count <> 0 Then
            Return dt.Rows(0).Item("NomeComune")
        Else
            Return ""
        End If

    End Function


    '################################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Elem_Cod"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[magnani]	28/04/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function Numero_Prodotti_xCategoria(ByVal Elem_Cod As Integer,
                                               ByRef objParametri As AgronicaCoreParametri
                                               ) As Integer

        Const nomeRoutine = "AgronicaCoreMetaschemaDAL.Categorie_Magazzino_R.Numero_Prodotti_xCategoria()"
    
        Dim messaggioErrore As String
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Dim tabella As String = ""
        Dim numProdotti As Integer

        Dim dtTabella As DataTable

        dtTabella = Leggi(Elem_Cod, "", False, "", "", objParametri)

        If Not IsNothing(dtTabella) AndAlso dtTabella.Rows.Count <> 0 Then
            tabella = dtTabella.Rows(0).Item("tabella")
        End If

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT  COUNT(*) as Num_Prodotti ")
            StrSQL.Append(" FROM " & tabella)
            
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            
            If Not IsNothing(dt) AndAlso dt.Rows.Count <> 0 Then
                numProdotti = dt.Rows(0).Item("Num_Prodotti")
            End If

        Catch ex As Exception
            Return 0
        End Try

        Return numProdotti

    End Function

    Public Function Leggi_ElemCod_Da_IdAgenda(Id_Agenda As String,
                                              ByRef objParametri As AgronicaCoreParametri
                                              ) As List(Of Integer)

        Const nomeRoutine = "AgronicaCoreMetaschemaDAL.Categorie_Magazzino_R.Leggi_ElemCod_Da_IdAgenda()"

        Dim messaggioErrore As String
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable
        
        Dim elemCod As New List(Of Integer)

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT DISTINCT Elem_Cod ")
            StrSQL.AppendLine(" FROM Movimenti_dettagli")
            StrSQL.AppendLine(" WHERE Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")
            
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)

            If Not IsNothing(dt) AndAlso dt.Rows.Count <> 0 Then
                For Each row In dt.Rows
                    elemCod.Add(row.Item("Elem_Cod"))
                Next
            End If

        Catch ex As Exception
            Return New List(Of Integer)
        End Try

        Return elemCod

    End Function

End Class
