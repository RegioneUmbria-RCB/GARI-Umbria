Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCorePannelloDiControlloDAL
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class NC_Testata_R

    Public Function leggi( _
                                ByRef objParametri As AgronicaCoreParametri, _
                                ByVal ID_NC As Integer _
                                ) As List(Of NC_Testata)

        Return leggi(objParametri, ID_NC, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing)

    End Function

    Public Function leggi( _
                                ByRef objParametri As AgronicaCoreParametri, _
                                ByVal ID_NC As Integer?, _
                                ByVal ID_Categoria As Integer?, _
                                ByVal Piva As String, _
                                ByVal ID_Gravita As Integer?, _
                                ByVal CodiceNC As String, _
                                ByVal Data_Apertura As DateTime?, _
                                ByVal Data_Chiusura As DateTime?, _
                                ByVal ID_CausaChiusura As Integer?, _
                                ByVal ID_Rischio As Integer?, _
                                ByVal ID_LineaImpianto As Integer? _
                              ) As List(Of NC_Testata)

        Dim NC_R As New AgronicaCorePannelloDiControlloDAL.NC_Testata_R
        Dim NC_Det_R As New AgronicaCorePannelloDiControlloBIZ.NC_Dettagli_R
        Dim listaObjPnlCtrl As New List(Of NC_Testata)

        'leggo le NC che rispondono ai parametri passati
        Dim dt_NC As DataTable = NC_R.Leggi(ID_NC, ID_Categoria, Piva, ID_Gravita, CodiceNC, Data_Apertura, _
                                            Data_Chiusura, ID_CausaChiusura, ID_Rischio, ID_LineaImpianto, _
                                            "", "", objParametri)

        For Each dRow As DataRow In dt_NC.Rows

            'Leggo i dettagli della NC e se ci sono, li aggiungo
            Dim lista_NC_Det As List(Of NC_Dettagli) = Nothing
            If Not IsDBNull(dRow("ID_NC")) Then
                lista_NC_Det = NC_Det_R.leggi_NonConformita_Dettagli(objParametri, ID_NC)
            End If

            Dim elem As New NC_Testata( _
                                          UtilityProvider.DBNullToNothing(dRow("ID_NC")), _
                                          UtilityProvider.DBNullToNothing(dRow("ID_Categoria")), _
                                          UtilityProvider.DBNullToNothing(dRow("Piva")), _
                                          UtilityProvider.DBNullToNothing(dRow("ID_Gravita")), _
                                          lista_NC_Det, _
                                          UtilityProvider.DBNullToNothing(dRow("CodiceNC")), _
                                          UtilityProvider.DBNullToNothing(dRow("Descrizione")), _
                                          UtilityProvider.DBNullToNothing(dRow("Note")), _
                                          UtilityProvider.DBNullToNothing(dRow("Data_Apertura")), _
                                          UtilityProvider.DBNullToNothing(dRow("Data_Chiusura")), _
                                          UtilityProvider.DBNullToNothing(dRow("ID_CausaChiusura")), _
                                          UtilityProvider.DBNullToNothing(dRow("ID_Rischio")), _
                                          UtilityProvider.DBNullToNothing(dRow("CodiceProdotto")), _
                                          UtilityProvider.DBNullToNothing(dRow("DescrizioneProdotto")), _
                                          UtilityProvider.DBNullToNothing(dRow("LineaProdotti")), _
                                          UtilityProvider.DBNullToNothing(dRow("LottoJDE")), _
                                          UtilityProvider.DBNullToNothing(dRow("DataArrivoProdotto")), _
                                          UtilityProvider.DBNullToNothing(dRow("LottoFruttagel")), _
                                          UtilityProvider.DBNullToNothing(dRow("LottoFornitore")), _
                                          UtilityProvider.DBNullToNothing(dRow("Bloccato")), _
                                          UtilityProvider.DBNullToNothing(dRow("QtaBloccati_UdM")), _
                                          UtilityProvider.DBNullToNothing(dRow("QtaBloccati_Qta")), _
                                          UtilityProvider.DBNullToNothing(dRow("QtaBloccati_Progressivi")), _
                                          UtilityProvider.DBNullToNothing(dRow("Fornitore")), _
                                          UtilityProvider.DBNullToNothing(dRow("ID_LineaImpianto")), _
                                          UtilityProvider.DBNullToNothing(dRow("Utente")), _
                                          UtilityProvider.DBNullToNothing(dRow("TipoNC_Codice")), _
                                          UtilityProvider.DBNullToNothing(dRow("TipoNC_Chiave")) _
                                          )
            listaObjPnlCtrl.Add(elem)
        Next

        Return listaObjPnlCtrl

    End Function

End Class

Public Class NC_testata_W

    Public Function aggiungi(ByRef objParametri_server As AgronicaCoreParametri,
                             ByRef objParametri_utenti As AgronicaCoreParametri,
                            ByVal nc As NC_Testata,
                            ByRef CodiceNC_out As String
                            ) As String

        Dim res As String = ""

        Try

            'apro una transazione
            ConnessioniTransazioni.ApriConnessione(True, objParametri_server)

            'Creo il nuovo ID_Dettaglio
            Dim seq As New Agro_Sequenze()
            Dim ID_NC As Integer = seq.NuovoId_Tabella("NC_ID_NC", 0, 2000000000, objParametri_server)

            'Creo il nuovo CodiceNC
            Dim CodiceNC As String = nc.CodiceNC
            If IsNothing(CodiceNC) OrElse CodiceNC = "" Then
                CodiceNC = calcolaNuovoCodiceNC(nc, objParametri_server, objParametri_utenti)
            End If

            'controllo che non si salvi una NC con codice vuoto
            If CodiceNC.Trim = "" Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri_server) 'Flag_Commit1_Rollback2
                res = "Errore durante la Modifica della Testata della NC: il codice NC è vuoto. Contattare l'amministratore o chiudere e riaprire Chrome."
                Exit Try
            End If

            'controllo che non esista già una NC con lo stesso codice (per evitare il problema della sovrascrittura delle NC)
            Dim dal_r As New AgronicaCorePannelloDiControlloDAL.NC_Testata_R
            Dim esisteGiaCodiceNC As Boolean = dal_r.VerificaEsisteGiaCodiceNC(CodiceNC, ID_NC, objParametri_server)
            If esisteGiaCodiceNC = True Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri_server) 'Flag_Commit1_Rollback2
                res = "Errore durante la Modifica della Testata della NC: il codice NC è già stato assegnato. Contattare l'amministratore o chiudere e riaprire Chrome."
                Exit Try
            End If

            Dim esito As Boolean = aggiungi(objParametri_server, ID_NC, nc.ID_Categoria, nc.Piva, nc.ID_Gravita, CodiceNC,
                                          nc.Descrizione, nc.Note, nc.Data_Apertura, nc.Data_Chiusura, nc.ID_CausaChiusura,
                                          nc.ID_Rischio, nc.CodiceProdotto, nc.DescrizioneProdotto, nc.LineaProdotti,
                                          nc.LottoJDE, nc.DataArrivoProdotto, nc.LottoFruttagel, nc.LottoFornitore, nc.Bloccato,
                                          nc.QtaBloccati_UdM, nc.QtaBloccati_Qta, nc.QtaBloccati_Progressivi, nc.Fornitore,
                                          nc.ID_LineaImpianto, nc.Utente, nc.TipoNC_Codice, nc.TipoNC_Chiave)

            If Not esito Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri_server) 'Flag_Commit1_Rollback2
                res = "Errore: Impossibile creare la testata della NC"
                Exit Try
            End If


            For Each det As NC_Dettagli In nc.Dettagli
                'Imposto l'id_nc nuovo su tutti i dettagli
                det.ID_NC = ID_NC

                ''Creo il nuovo CodiceDettaglio
                'Dim CodiceDettaglio As String = det.CodiceDettaglio
                'If IsNothing(CodiceDettaglio) OrElse CodiceDettaglio = 0 Then
                '    CodiceDettaglio = calcolaNuovoCodiceDettaglio(nc, det, objParametri)

                'End If

            Next

            'Aggiungo i dettagli
            If Not IsNothing(nc.Dettagli) Then
                Dim det_W As New NC_Dettagli_W
                esito = det_W.aggiungi(objParametri_server, nc.Dettagli)

                If Not esito Then
                    ConnessioniTransazioni.ChiudiTransazione(2, objParametri_server) 'Flag_Commit1_Rollback2
                    res = "Errore: Impossibile creare i dettagli della NC"
                    Exit Try
                End If
            End If

            'Se Necessario salvo l'ID_NC anche nella tabella della sorgente, marcandole come collegate.
            Dim fnc As New FontiNC()
            esito = fnc.assegnaID_NCaFonte(ID_NC, nc.TipoNC_Codice, nc.TipoNC_Chiave, objParametri_server)
            If Not esito Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri_server) 'Flag_Commit1_Rollback2
                res = "Errore: Impossibile marcare la fonte della NC come già aggiunta all'elenco delle NC"
                Exit Try
            End If

            res = ID_NC
            CodiceNC_out = CodiceNC

            'Se è andato tutto bene
            ConnessioniTransazioni.ChiudiTransazione(1, objParametri_server) 'Flag_Commit1_Rollback2

        Catch ex As Exception
            ConnessioniTransazioni.ChiudiTransazione(2, objParametri_server) 'Flag_Commit1_Rollback2
            res = "Errore: " + ex.Message
        End Try

        Return res

    End Function

    Private Function aggiungi(ByRef objParametri As AgronicaCoreParametri,
                                ByVal ID_NC As Integer,
                                ByVal ID_Categoria As Integer?,
                                ByVal Piva As String,
                                ByVal ID_Gravita As Integer?,
                                ByVal CodiceNC As String,
                                ByVal Descrizione As String,
                                ByVal Note As String,
                                ByVal Data_Apertura As DateTime?,
                                ByVal Data_Chiusura As DateTime?,
                                ByVal ID_CausaChiusura As Integer?,
                                ByVal ID_Rischio As Integer?,
                                ByVal CodiceProdotto As String,
                                ByVal DescrizioneProdotto As String,
                                ByVal LineaProdotti As String,
                                ByVal LottoJDE As String,
                                ByVal DataArrivoProdotto As DateTime?,
                                ByVal LottoFruttagel As String,
                                ByVal LottoFornitore As String,
                                ByVal Bloccato As Integer?,
                                ByVal QtaBloccati_UdM As String,
                                ByVal QtaBloccati_Qta As Decimal?,
                                ByVal QtaBloccati_Progressivi As String,
                                ByVal Fornitore As String,
                                ByVal ID_LineaImpianto As Integer?,
                                ByVal Utente As String,
                                ByVal TipoNC_Codice As Integer?,
                                ByVal TipoNC_Chiave As String
                              ) As Boolean

        Dim w As New AgronicaCorePannelloDiControlloDAL.NC_Testata_W
        Dim res As Boolean = False

        res = w.Scrivi(objParametri, ID_NC, ID_Categoria, Piva, ID_Gravita, CodiceNC,
                        Descrizione, Note, Data_Apertura, Data_Chiusura, ID_CausaChiusura,
                        ID_Rischio, CodiceProdotto, DescrizioneProdotto, LineaProdotti,
                        LottoJDE, DataArrivoProdotto, LottoFruttagel, LottoFornitore, Bloccato,
                        QtaBloccati_UdM, QtaBloccati_Qta, QtaBloccati_Progressivi, Fornitore,
                        ID_LineaImpianto, Utente, TipoNC_Codice, TipoNC_Chiave)

        Return res

    End Function

    Private Function modifica(ByRef objParametri As AgronicaCoreParametri,
                                ByVal Old_ID_NC As Integer,
                                ByVal New_ID_Categoria As Integer?,
                                ByVal New_Piva As String,
                                ByVal New_ID_Gravita As Integer?,
                                ByVal New_CodiceNC As String,
                                ByVal New_Descrizione As String,
                                ByVal New_Note As String,
                                ByVal New_Data_Apertura As DateTime?,
                                ByVal New_Data_Chiusura As DateTime?,
                                ByVal New_ID_CausaChiusura As Integer?,
                                ByVal New_ID_Rischio As Integer?,
                                ByVal New_CodiceProdotto As String,
                                ByVal New_DescrizioneProdotto As String,
                                ByVal New_LineaProdotti As String,
                                ByVal New_LottoJDE As String,
                                ByVal New_DataArrivoProdotto As DateTime?,
                                ByVal New_LottoFruttagel As String,
                                ByVal New_LottoFornitore As String,
                                ByVal New_Bloccato As Integer?,
                                ByVal New_QtaBloccati_UdM As String,
                                ByVal New_QtaBloccati_Qta As Decimal?,
                                ByVal New_QtaBloccati_Progressivi As String,
                                ByVal New_Fornitore As String,
                                ByVal New_ID_LineaImpianto As Integer?,
                                ByVal New_Utente As String,
                                ByVal New_TipoNC_Codice As Integer?,
                                ByVal New_TipoNC_Chiave As String
                              ) As Boolean

        Dim w As New AgronicaCorePannelloDiControlloDAL.NC_Testata_W
        Dim res As Boolean = w.Modifica(objParametri, Old_ID_NC, New_ID_Categoria, New_Piva, New_ID_Gravita,
                                        New_CodiceNC, New_Descrizione, New_Note, New_Data_Apertura, New_Data_Chiusura,
                                        New_ID_CausaChiusura, New_ID_Rischio, New_CodiceProdotto, New_DescrizioneProdotto,
                                        New_LineaProdotti, New_LottoJDE, New_DataArrivoProdotto, New_LottoFruttagel,
                                        New_LottoFornitore, New_Bloccato, New_QtaBloccati_UdM, New_QtaBloccati_Qta,
                                        New_QtaBloccati_Progressivi, New_Fornitore, New_ID_LineaImpianto, New_Utente,
                                        New_TipoNC_Codice, New_TipoNC_Chiave)

        Return res

    End Function

    Public Function modifica(ByRef objParametri As AgronicaCoreParametri,
                                ByVal e As NC_Testata
                              ) As String

        Dim res As String = ""

        Try

            'apro una transazione
            ConnessioniTransazioni.ApriConnessione(True, objParametri)

            'controllo che non si salvi una NC con codice vuoto
            If e.CodiceNC.Trim = "" Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri) 'Flag_Commit1_Rollback2
                res = "Errore durante la Modifica della Testata della NC: il codice NC è vuoto. Contattare l'amministratore o chiudere e riaprire Chrome."
                Exit Try
            End If

            'controllo che non esista già una NC con lo stesso codice (per evitare il problema della sovrascrittura delle NC)
            Dim dal_r As New AgronicaCorePannelloDiControlloDAL.NC_Testata_R
            Dim esisteGiaCodiceNC As Boolean = dal_r.VerificaEsisteGiaCodiceNC(e.CodiceNC, e.ID_NC, objParametri)
            If esisteGiaCodiceNC = True Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri) 'Flag_Commit1_Rollback2
                res = "Errore durante la Modifica della Testata della NC: il codice NC è già stato assegnato. Contattare l'amministratore o chiudere e riaprire Chrome."
                Exit Try
            End If

            'modifico la testata
            Dim esito As Boolean = modifica(objParametri, e.ID_NC, e.ID_Categoria, e.Piva, e.ID_Gravita,
                                            e.CodiceNC, e.Descrizione, e.Note, e.Data_Apertura, e.Data_Chiusura,
                                            e.ID_CausaChiusura, e.ID_Rischio, e.CodiceProdotto, e.DescrizioneProdotto,
                                            e.LineaProdotti, e.LottoJDE, e.DataArrivoProdotto, e.LottoFruttagel,
                                            e.LottoFornitore, e.Bloccato, e.QtaBloccati_UdM, e.QtaBloccati_Qta,
                                            e.QtaBloccati_Progressivi, e.Fornitore, e.ID_LineaImpianto, e.Utente,
                                            e.TipoNC_Codice, e.TipoNC_Chiave)

            If Not esito Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri) 'Flag_Commit1_Rollback2
                res = "Errore durante la Modifica della Testata della NC"
                Exit Try
            End If

            'modifico i dettagli
            Dim det_W As New NC_Dettagli_W
            esito = det_W.modifica(objParametri, e.Dettagli)

            If Not esito Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri) 'Flag_Commit1_Rollback2
                res = "Errore durante la Modifica dei dettagli della NC"
                Exit Try
            End If

            'Se è andato tutto bene
            ConnessioniTransazioni.ChiudiTransazione(1, objParametri) 'Flag_Commit1_Rollback2

        Catch ex As Exception
            ConnessioniTransazioni.ChiudiTransazione(2, objParametri) 'Flag_Commit1_Rollback2
            res = "Errore: " + ex.Message
        End Try

        Return res

    End Function

    Public Function cancellaInteraNC(ByRef objParametri As AgronicaCoreParametri,
                                            ByVal ID_NC As Integer
                                           ) As String

        Dim res As String = ""

        Try
            'apro una transazione
            ConnessioniTransazioni.ApriConnessione(True, objParametri)

            'cancello la testata
            Dim elemW As New AgronicaCorePannelloDiControlloDAL.NC_Testata_W
            Dim esito = elemW.CancellaNonConformita("", ID_NC, objParametri)

            If Not esito Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri) 'Flag_Commit1_Rollback2
                Exit Try
            End If

            'cancello i dettagli
            Dim det_W As New NC_Dettagli_W
            esito = det_W.CancellaTuttiIDettagliDiNC(objParametri, ID_NC)

            If Not esito Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri) 'Flag_Commit1_Rollback2
                Exit Try
            End If

            'Se è andato tutto bene
            ConnessioniTransazioni.ChiudiTransazione(1, objParametri) 'Flag_Commit1_Rollback2

        Catch ex As Exception
            ConnessioniTransazioni.ChiudiTransazione(2, objParametri) 'Flag_Commit1_Rollback2
            res = "Errore: " + ex.Message
        End Try

        Return res

    End Function

    Private Function calcolaNuovoCodiceNC(ByRef nc As NC_Testata,
                                          ByRef objParametri_Server As AgronicaCoreParametri,
                                          ByRef objParametri_Utenti As AgronicaCoreParametri) As String
        Dim CodiceNC As String = ""

        Select Case objParametri_Server.PivaSuperUser

            Case "01271980391" 'FRUTTAGEL

                Dim CodiceCiclo As String = "XXXX"
                Dim Anno As String = CDate(nc.Data_Apertura).Year
                Dim CodiceFruttagel As String = nc.CodiceProdotto

                Dim articoli_R As New AgronicaCoreLabControlloQualitaBIZ.Articoli_R
                Dim catMer As String = articoli_R.leggi_Articoli(AGRODATAINIZIO,
                                                                 AGRODATAFINE,
                                                                 objParametri_Server, objParametri_Utenti,
                                                                 CodiceFruttagel)(0).CatMerceologica.ToLower()

                'campi di Materie_Prime_Dettagli
                'Extra_Str1 as Marchio,
                'Extra_Str2 As MatPrima_Nome, 
                'Extra_Str3 as Formato,
                'Extra_Str4 As CatMerceologica, 
                'Extra_Str5 as TipoProduzione,
                'Extra_Str6 As RegolaScadenza_Tipo,
                'Extra_Str7 as RegolaScadenza_Qta,
                'Extra_Str8 As Stato, 
                'Extra_Str9 as LineaPDZ


                'le regole condivise:
                'PROC(produzione caldo)
                '-> codici che iniziano con 1 o 3 e con categoria merceologica 30 (Prodotto Finito) o 20 (Semilavorato)
                'ACQC(acquisti caldo)
                '-> codici che iniziano con 1 o 3 e con categoria merceologica 32 (Prodotto Finito) o 19 (Semilavorato) OPPURE codici che iniziano con 610 o con 250  
                'PROF(produzione freddo)
                '-> codici che iniziano con 4 e con categoria merceologica 30 (Prodotto Finito) o 20 (Semilavorato)
                'ACQF(acquisti freddo)
                '-> codici che iniziano con 4 e con categoria merceologica 32 (Prodotto Finito) o 19 (Semilavorato)
                'AGRO(agronomico)
                '-> codici che iniziano con 600
                'IMBL(imballi)
                '-> codici che iniziano con 5  
                'Dal 23 / 3 / 2021
                'SCOF(scondizionati freddo)
                '-> codici che iniziano con 4 e con categoria merceologica 31 (prodotti scondizionati)
                'Dal 13 / 5 / 2022
                'LAVT(lavorazioni di terzi)
                '-> codici che iniziano con 1, 3 o 4 e con categoria merceologica 80 (merce di terzi)


                '1=Bevande - 3=Pomodoro - 4=Surgelati - 5=Imballaggi - 600=MatPrimaAgricola -  250= SL acquistati - 610=
                If (CodiceFruttagel.StartsWith("1") OrElse CodiceFruttagel.StartsWith("3")) AndAlso catMer.Contains("ns. produz") Then
                    CodiceCiclo = "PROC" ' Produzione Caldo
                ElseIf ((CodiceFruttagel.StartsWith("1") OrElse CodiceFruttagel.StartsWith("3")) AndAlso catMer.Contains("acquistati")) _
                    OrElse CodiceFruttagel.StartsWith("610") OrElse CodiceFruttagel.StartsWith("250") Then
                    '05/10/2020: chiamata 8345 (Elisa Rossi: vi chiediamo di far rientrare gli articoli che iniziano con il 250*** negli ACQC anzichè negli ACQF)
                    CodiceCiclo = "ACQC" ' Acquisti Caldo
                ElseIf ((CodiceFruttagel.StartsWith("1") OrElse CodiceFruttagel.StartsWith("3") OrElse CodiceFruttagel.StartsWith("4")) AndAlso catMer.Contains("merce di terzi")) Then
                    '13/05/2022: chiamata 18104 (Elisa Rossi: gestire sigla LAVT (lavorazioni di terzi) per codici che iniziano con 1, 3 o 4 con categoria merceologica 80 (Merce di terzi))
                    CodiceCiclo = "LAVT" ' Lavorazioni di terzi
                ElseIf CodiceFruttagel.StartsWith("4") AndAlso catMer.Contains("ns. produz") Then
                    CodiceCiclo = "PROF" ' Produzione Freddo
                ElseIf CodiceFruttagel.StartsWith("4") AndAlso catMer.Contains("acquistati") Then
                    '19/01/2021: patch (chiamata 10368 di elisa rossi): RNC aperta sul codice 450663 BASIL.SUR.C/OL.X AS.15X1 PI.LI perchè inizia con il 4* ma il sistema ha attribuito la RNC all'area ACQC (acquisti caldo)? 
                    CodiceCiclo = "ACQF" ' Acquisti Freddo
                ElseIf CodiceFruttagel.StartsWith("4") AndAlso catMer.Contains("scondizionati") Then
                    '23/03/2021 richiesta nuovo codice
                    CodiceCiclo = "SCOF" 'SCOF(scondizionati freddo)
                ElseIf CodiceFruttagel.StartsWith("600") Then
                    CodiceCiclo = "AGRO" ' Agronomico
                ElseIf CodiceFruttagel.StartsWith("5") Then
                    CodiceCiclo = "IMBL" ' Imballaggi
                Else
                    'ERRORE
                End If

                'Utilizzo questa versione del nuovo id, per non perdere valori nel contatore in caso di errore nella transazione
                Dim seq As New Agro_Sequenze()
                Dim seqCodiceNC As Integer = seq.NuovoId_Tabella("NC_CodiceNC_" + Anno + "_" + CodiceCiclo, objParametri_Server.UsernameOperazione, 0, 2000000000, objParametri_Server.objConnessione, objParametri_Server.objTransazione,
                                                                 objParametri_Server.StringaConnessione, objParametri_Server.FlagVisibilita, objParametri_Server.LogDirectory, objParametri_Server.LogFileName, objParametri_Server.UsernameOperazione)

                CodiceNC = Anno + CodiceCiclo + seqCodiceNC.ToString("0000")

            Case Else

                Dim anno As String = CDate(nc.Data_Apertura).Year

                'Utilizzo questa versione del nuovo id, per non perdere valori nel contatore in caso di errore nella transazione
                Dim seq As New Agro_Sequenze()
                Dim seqCodiceNC As Integer = seq.NuovoId_Tabella("NC_CodiceNC_" + anno, objParametri_Server.UsernameOperazione, 0, 2000000000, objParametri_Server.objConnessione, objParametri_Server.objTransazione,
                                                                 objParametri_Server.StringaConnessione, objParametri_Server.FlagVisibilita, objParametri_Server.LogDirectory, objParametri_Server.LogFileName, objParametri_Server.UsernameOperazione)

                CodiceNC = anno + "-" + seqCodiceNC.ToString("0000")
        End Select

        Return CodiceNC

    End Function

    Private Function calcolaNuovoCodiceDettaglio(ByRef nc As NC_Testata, ByRef det As NC_Dettagli, ByRef objParametri As AgronicaCoreParametri) As Integer
        Dim maxCodiceDettaglio As Integer = 0

        For Each d As NC_Dettagli In nc.Dettagli
            If Not IsNothing(d.CodiceDettaglio) AndAlso d.CodiceDettaglio > maxCodiceDettaglio Then
                maxCodiceDettaglio = d.CodiceDettaglio
            End If
        Next

        Return maxCodiceDettaglio + 1
    End Function

End Class

<Serializable()> _
Public Class NC_Testata
    Implements ICloneable

    Public Property ID_NC As Integer?
    Public Property ID_Categoria As Integer?
    Public Property Piva As String
    Public Property ID_Gravita As Integer?
    Public Property Dettagli As List(Of NC_Dettagli)
    Public Property CodiceNC As String
    Public Property Descrizione As String
    Public Property Note As String
    Public Property Data_Apertura As DateTime?
    Public Property Data_Chiusura As DateTime?
    Public Property ID_CausaChiusura As Integer?
    Public Property ID_Rischio As Integer?
    Public Property CodiceProdotto As String
    Public Property DescrizioneProdotto As String
    Public Property LineaProdotti As String
    Public Property LottoJDE As String
    Public Property DataArrivoProdotto As DateTime?
    Public Property LottoFruttagel As String
    Public Property LottoFornitore As String
    Public Property Bloccato As Short?
    Public Property QtaBloccati_UdM As String
    Public Property QtaBloccati_Qta As Decimal?
    Public Property QtaBloccati_Progressivi As String
    Public Property Fornitore As String
    Public Property ID_LineaImpianto As Integer?
    Public Property Utente As String
    Public Property TipoNC_Codice As Integer?
    Public Property TipoNC_Chiave As String

    Public Property Area As String

    Public Sub New( _
                    ID_NC As Integer?, _
                    ID_Categoria As Integer?, _
                    Piva As String, _
                    ID_Gravita As Integer?, _
                    Dettagli As List(Of NC_Dettagli), _
                    CodiceNC As String, _
                    Descrizione As String, _
                    Note As String, _
                    Data_Apertura As DateTime?, _
                    Data_Chiusura As DateTime?, _
                    ID_CausaChiusura As Integer?, _
                    ID_Rischio As Integer?, _
                    CodiceProdotto As String, _
                    DescrizioneProdotto As String, _
                    LineaProdotti As String, _
                    LottoJDE As String, _
                    DataArrivoProdotto As DateTime?, _
                    LottoFruttagel As String, _
                    LottoFornitore As String, _
                    Bloccato As Short?, _
                    QtaBloccati_UdM As String, _
                    QtaBloccati_Qta As Decimal?, _
                    QtaBloccati_Progressivi As String, _
                    Fornitore As String, _
                    ID_LineaImpianto As Integer?, _
                    Utente As String, _
                    TipoNC_Codice As Integer?, _
                    TipoNC_Chiave As String)

        _ID_NC = ID_NC
        _ID_Categoria = ID_Categoria
        _Piva = Piva
        _ID_Gravita = ID_Gravita
        _Dettagli = Dettagli
        _CodiceNC = CodiceNC
        _Descrizione = Descrizione
        _Note = Note
        _Data_Apertura = Data_Apertura
        _Data_Chiusura = Data_Chiusura
        _ID_CausaChiusura = ID_CausaChiusura
        _ID_Rischio = ID_Rischio
        _CodiceProdotto = CodiceProdotto
        _DescrizioneProdotto = DescrizioneProdotto
        _LineaProdotti = LineaProdotti
        _LottoJDE = LottoJDE
        _DataArrivoProdotto = DataArrivoProdotto
        _LottoFruttagel = LottoFruttagel
        _LottoFornitore = LottoFornitore
        _Bloccato = Bloccato
        _QtaBloccati_UdM = QtaBloccati_UdM
        _QtaBloccati_Qta = QtaBloccati_Qta
        _QtaBloccati_Progressivi = QtaBloccati_Progressivi
        _Fornitore = Fornitore
        _ID_LineaImpianto = ID_LineaImpianto
        _Utente = Utente
        _TipoNC_Codice = TipoNC_Codice
        _TipoNC_Chiave = TipoNC_Chiave

    End Sub

    Public Sub New()
        _ID_NC = Nothing
        _ID_Categoria = Nothing
        _Piva = ""
        _ID_Gravita = Nothing
        _Dettagli = New List(Of NC_Dettagli)
        _CodiceNC = ""
        _Descrizione = ""
        _Note = ""
        _Data_Apertura = Nothing
        _Data_Chiusura = Nothing
        _ID_CausaChiusura = Nothing
        _ID_Rischio = Nothing
        _CodiceProdotto = ""
        _DescrizioneProdotto = ""
        _LineaProdotti = ""
        _LottoJDE = ""
        _DataArrivoProdotto = Nothing
        _LottoFruttagel = ""
        _LottoFornitore = ""
        _Bloccato = Nothing
        _QtaBloccati_UdM = ""
        _QtaBloccati_Qta = Nothing
        _QtaBloccati_Progressivi = ""
        _Fornitore = ""
        _ID_LineaImpianto = Nothing
        _Utente = ""
        _ID_LineaImpianto = TipoNC_Codice
        _Utente = TipoNC_Chiave
    End Sub

    Public Sub New(obj As JObject)
        'JObject jObject = JObject.Parse(json);
        'JToken jUser = jObject["user"];
        'name = (string) jUser["name"];
        'teamname = (string) jUser["teamname"];
        'email = (string) jUser["email"];
        'players = jUser["players"].ToArray();

        _ID_NC = assegnaValoreNullableDaJSON_Integer(obj("ID_NC"), _ID_NC)
        _ID_Categoria = assegnaValoreNullableDaJSON_Integer(obj("ID_Categoria"), _ID_Categoria)
        _Piva = assegnaValoreNullableDaJSON_String(obj("Piva"), _Piva)
        _ID_Gravita = assegnaValoreNullableDaJSON_Integer(obj("ID_Gravita"), _ID_Gravita)
        _CodiceNC = assegnaValoreNullableDaJSON_String(obj("CodiceNC"), _CodiceNC)
        _Descrizione = assegnaValoreNullableDaJSON_String(obj("Descrizione"), _Descrizione)
        _Note = assegnaValoreNullableDaJSON_String(obj("Note"), _Note)
        _Data_Apertura = assegnaValoreNullableDaJSON_Date(obj("Data_Apertura"), _Data_Apertura)
        _Data_Chiusura = assegnaValoreNullableDaJSON_Date(obj("Data_Chiusura"), _Data_Chiusura)
        _ID_CausaChiusura = assegnaValoreNullableDaJSON_Integer(obj("ID_CausaChiusura"), _ID_CausaChiusura)
        _ID_Rischio = assegnaValoreNullableDaJSON_Integer(obj("ID_Rischio"), _ID_Rischio)
        _CodiceProdotto = assegnaValoreNullableDaJSON_String(obj("CodiceProdotto"), _CodiceProdotto)
        _DescrizioneProdotto = assegnaValoreNullableDaJSON_String(obj("DescrizioneProdotto"), _DescrizioneProdotto)
        _LineaProdotti = assegnaValoreNullableDaJSON_String(obj("LineaProdotti"), _LineaProdotti)
        _LottoJDE = assegnaValoreNullableDaJSON_String(obj("LottoJDE"), _LottoJDE)
        _DataArrivoProdotto = assegnaValoreNullableDaJSON_Date(obj("DataArrivoProdotto"), _DataArrivoProdotto)
        _LottoFruttagel = assegnaValoreNullableDaJSON_String(obj("LottoFruttagel"), _LottoFruttagel)
        _LottoFornitore = assegnaValoreNullableDaJSON_String(obj("LottoFornitore"), _LottoFornitore)
        _Bloccato = assegnaValoreNullableDaJSON_Integer(obj("Bloccato"), _Bloccato)
        _QtaBloccati_UdM = assegnaValoreNullableDaJSON_String(obj("QtaBloccati_UdM"), _QtaBloccati_UdM)
        _QtaBloccati_Qta = assegnaValoreNullableDaJSON_Decimal(obj("QtaBloccati_Qta"), _QtaBloccati_Qta)
        _QtaBloccati_Progressivi = assegnaValoreNullableDaJSON_String(obj("QtaBloccati_Progressivi"), _QtaBloccati_Progressivi)
        _Fornitore = assegnaValoreNullableDaJSON_String(obj("Fornitore"), _Fornitore)
        _ID_LineaImpianto = assegnaValoreNullableDaJSON_Integer(obj("ID_LineaImpianto"), _ID_LineaImpianto)
        _Utente = assegnaValoreNullableDaJSON_String(obj("Utente"), _Utente)
        _TipoNC_Codice = assegnaValoreNullableDaJSON_Integer(obj("TipoNC_Codice"), _TipoNC_Codice)
        _TipoNC_Chiave = assegnaValoreNullableDaJSON_String(obj("TipoNC_Chiave"), _TipoNC_Chiave)

        If Not IsNothing(obj("Dettagli")) AndAlso obj("Dettagli").Type <> JTokenType.Null AndAlso obj("Dettagli").HasValues Then
            Dim objDet As JArray = obj("Dettagli")
            Dim listaDet As New List(Of NC_Dettagli)
            For Each det As JObject In objDet.Children
                listaDet.Add(New NC_Dettagli(det))
            Next
            _Dettagli = listaDet
        Else
            _Dettagli = New List(Of NC_Dettagli)
        End If

    End Sub

    Private Function assegnaValoreNullableDaJSON_String(objJSON As JValue, objVB As String) As String
        Return If(Not IsNothing(objJSON) AndAlso objJSON.Type <> JTokenType.Null, CStr(objJSON), objVB)
    End Function

    Private Function assegnaValoreNullableDaJSON_Integer(objJSON As JValue, objVB As Integer?) As Integer?
        Return If(Not IsNothing(objJSON) AndAlso objJSON.Type <> JTokenType.Null, CInt(objJSON), objVB)
    End Function

    Private Function assegnaValoreNullableDaJSON_Date(objJSON As JValue, objVB As DateTime?) As DateTime?
        Return If(Not IsNothing(objJSON) AndAlso objJSON.Type <> JTokenType.Null, CDate(objJSON), objVB)
    End Function

    Private Function assegnaValoreNullableDaJSON_Decimal(objJSON As JValue, objVB As Decimal?) As Decimal?
        Return If(Not IsNothing(objJSON) AndAlso objJSON.Type <> JTokenType.Null, CDec(objJSON), objVB)
    End Function

    Public Function Clone() As Object Implements System.ICloneable.Clone
        Dim m As New System.IO.MemoryStream()
        Dim f As New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter()
        f.Serialize(m, Me)
        m.Seek(0, System.IO.SeekOrigin.Begin)
        Return f.Deserialize(m)
    End Function

    Public Shared Function TestSeTestateSonoUguali(ByVal ncTestata1 As NC_Testata, ByVal ncTestata2 As NC_Testata) As Boolean

        If IsNothing(ncTestata1) AndAlso IsNothing(ncTestata2) Then
            Return True
        ElseIf (IsNothing(ncTestata1) AndAlso Not IsNothing(ncTestata2)) OrElse (Not IsNothing(ncTestata1) AndAlso IsNothing(ncTestata2)) Then
            Return False
        Else
            'Pulisco le testate dai dettagli
            'ncTestata1.Dettagli = Nothing
            'ncTestata2.Dettagli = Nothing

            'Return ncTestata1.Equals(ncTestata2)

            If ((ncTestata1.Bloccato = ncTestata2.Bloccato) OrElse (IsNothing(ncTestata1.Bloccato) AndAlso IsNothing(ncTestata2.Bloccato))) AndAlso _
                ((ncTestata1.CodiceNC = ncTestata2.CodiceNC) OrElse (IsNothing(ncTestata1.CodiceNC) AndAlso IsNothing(ncTestata2.CodiceNC))) AndAlso _
                ((ncTestata1.CodiceProdotto = ncTestata2.CodiceProdotto) OrElse (IsNothing(ncTestata1.CodiceProdotto) AndAlso IsNothing(ncTestata2.CodiceProdotto))) AndAlso _
                ((ncTestata1.Data_Apertura = ncTestata2.Data_Apertura) OrElse (IsNothing(ncTestata1.Data_Apertura) AndAlso IsNothing(ncTestata2.Data_Apertura))) AndAlso _
                ((ncTestata1.Data_Chiusura = ncTestata2.Data_Chiusura) OrElse (IsNothing(ncTestata1.Data_Chiusura) AndAlso IsNothing(ncTestata2.Data_Chiusura))) AndAlso _
                ((ncTestata1.DataArrivoProdotto = ncTestata2.DataArrivoProdotto) OrElse (IsNothing(ncTestata1.DataArrivoProdotto) AndAlso IsNothing(ncTestata2.DataArrivoProdotto))) AndAlso _
                ((ncTestata1.Descrizione = ncTestata2.Descrizione) OrElse (IsNothing(ncTestata1.Descrizione) AndAlso IsNothing(ncTestata2.Descrizione))) AndAlso _
                ((ncTestata1.DescrizioneProdotto = ncTestata2.DescrizioneProdotto) OrElse (IsNothing(ncTestata1.DescrizioneProdotto) AndAlso IsNothing(ncTestata2.DescrizioneProdotto))) AndAlso _
                ((ncTestata1.Fornitore = ncTestata2.Fornitore) OrElse (IsNothing(ncTestata1.Fornitore) AndAlso IsNothing(ncTestata2.Fornitore))) AndAlso _
                ((ncTestata1.ID_Categoria = ncTestata2.ID_Categoria) OrElse (IsNothing(ncTestata1.ID_Categoria) AndAlso IsNothing(ncTestata2.ID_Categoria))) AndAlso _
                ((ncTestata1.ID_CausaChiusura = ncTestata2.ID_CausaChiusura) OrElse (IsNothing(ncTestata1.ID_CausaChiusura) AndAlso IsNothing(ncTestata2.ID_CausaChiusura))) AndAlso _
                ((ncTestata1.ID_Gravita = ncTestata2.ID_Gravita) OrElse (IsNothing(ncTestata1.ID_Gravita) AndAlso IsNothing(ncTestata2.ID_Gravita))) AndAlso _
                ((ncTestata1.ID_LineaImpianto = ncTestata2.ID_LineaImpianto) OrElse (IsNothing(ncTestata1.ID_LineaImpianto) AndAlso IsNothing(ncTestata2.ID_LineaImpianto))) AndAlso _
                ((ncTestata1.ID_NC = ncTestata2.ID_NC) OrElse (IsNothing(ncTestata1.ID_NC) AndAlso IsNothing(ncTestata2.ID_NC))) AndAlso _
                ((ncTestata1.ID_Rischio = ncTestata2.ID_Rischio) OrElse (IsNothing(ncTestata1.ID_Rischio) AndAlso IsNothing(ncTestata2.ID_Rischio))) AndAlso _
                ((ncTestata1.LineaProdotti = ncTestata2.LineaProdotti) OrElse (IsNothing(ncTestata1.LineaProdotti) AndAlso IsNothing(ncTestata2.LineaProdotti))) AndAlso _
                ((ncTestata1.LottoFornitore = ncTestata2.LottoFornitore) OrElse (IsNothing(ncTestata1.LottoFornitore) AndAlso IsNothing(ncTestata2.LottoFornitore))) AndAlso _
                ((ncTestata1.LottoFruttagel = ncTestata2.LottoFruttagel) OrElse (IsNothing(ncTestata1.LottoFruttagel) AndAlso IsNothing(ncTestata2.LottoFruttagel))) AndAlso _
                ((ncTestata1.LottoJDE = ncTestata2.LottoJDE) OrElse (IsNothing(ncTestata1.LottoJDE) AndAlso IsNothing(ncTestata2.LottoJDE))) AndAlso _
                ((ncTestata1.Note = ncTestata2.Note) OrElse (IsNothing(ncTestata1.Note) AndAlso IsNothing(ncTestata2.Note))) AndAlso _
                ((ncTestata1.Piva = ncTestata2.Piva) OrElse (IsNothing(ncTestata1.Piva) AndAlso IsNothing(ncTestata2.Piva))) AndAlso _
                ((ncTestata1.QtaBloccati_Progressivi = ncTestata2.QtaBloccati_Progressivi) OrElse (IsNothing(ncTestata1.QtaBloccati_Progressivi) AndAlso IsNothing(ncTestata2.QtaBloccati_Progressivi))) AndAlso _
                ((ncTestata1.QtaBloccati_Qta = ncTestata2.QtaBloccati_Qta) OrElse (IsNothing(ncTestata1.QtaBloccati_Qta) AndAlso IsNothing(ncTestata2.QtaBloccati_Qta))) AndAlso _
                ((ncTestata1.QtaBloccati_UdM = ncTestata2.QtaBloccati_UdM) OrElse (IsNothing(ncTestata1.QtaBloccati_UdM) AndAlso IsNothing(ncTestata2.QtaBloccati_UdM))) AndAlso _
                ((ncTestata1.Utente = ncTestata2.Utente) OrElse (IsNothing(ncTestata1.Utente) AndAlso IsNothing(ncTestata2.Utente))) AndAlso _
                ((ncTestata1.TipoNC_Codice = ncTestata2.TipoNC_Codice) OrElse (IsNothing(ncTestata1.TipoNC_Codice) AndAlso IsNothing(ncTestata2.TipoNC_Codice))) AndAlso _
                ((ncTestata1.TipoNC_Chiave = ncTestata2.TipoNC_Chiave) OrElse (IsNothing(ncTestata1.TipoNC_Chiave) AndAlso IsNothing(ncTestata2.TipoNC_Chiave))) Then

                Return True
            Else
                Return False
            End If

        End If

    End Function

End Class
