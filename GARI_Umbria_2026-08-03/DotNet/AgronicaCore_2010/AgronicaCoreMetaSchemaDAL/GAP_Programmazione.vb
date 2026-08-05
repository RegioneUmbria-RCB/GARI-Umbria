
Imports System.Data
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class GAP_Programmazione_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '-----------------------------------------------------------
    '--------------------------COLLEZIONI-----------------------
    '-----------------------------------------------------------

    'per la lettura delle collezioni - IN USO
    'se in FILTROULTIMO passo ATTIVI vado a pescare solo le collezioni ATTIVE, valide in termini temporali
    'se in FILTROULTIMO non passo NULLA vado a prendere tutte le collezioni perchè devo popolare l'ARCHIVIO
    Public Function Leggi_Collezioni(
                                ByVal IDCollezione As Integer?,
                                ByVal filtroUltimo As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "GAP_Programmazione_R.Leggi_Collezioni"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.AppendLine(" SELECT IDCollezione, Coll_Titolo, Coll_Des, inviato, datainvio, Data_Creazione, ")
            strSQL.AppendLine("Data_Modifica, Username_Creazione, Username_Modifica, Validita_inizio, Validita_fine")
            strSQL.AppendLine(" FROM DOC_Collezioni ")

            'strSQL.AppendLine(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            strSQL.AppendLine(" WHERE Validita_Inizio < GETDATE()")

            'se devo ricercare per collezione
            If Not IsNothing(IDCollezione) Then
                strSQL.AppendLine(" AND IDCollezione = " & Agro_SQL_SaveNum_NULL(IDCollezione))
            End If

            If xFiltroAggiuntivo <> "" Then
                strSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            'quando voglio la collezione PIU RECENTE
            If filtroUltimo.Equals("ultimo") Then
                strSQL.AppendLine(" AND Validita_Inizio = (SELECT MAX (Validita_Inizio) FROM DOC_Collezioni WHERE GETDATE() < Validita_fine AND Validita_Inizio < GETDATE())")
                'se voglio l'archivio voglio tutte le date
            ElseIf filtroUltimo.Equals("archivio") Then
                'se voglio l'ARCHIVIO prendo tutto
            Else
                'per le collezioni ATTIVE
                strSQL.AppendLine(" AND GETDATE() < Validita_fine")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message & " QUERY: " & strSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    '----- cerco le collezioni per titolo o descrizione
    Public Function Cerco_Collezioni(ByVal titoloCollezione As String,
                                     ByVal desCollezione As String,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                     ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "GAP_Programmazione_R.Cerca_Collezioni"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.AppendLine(" SELECT IDCollezione, Coll_Titolo, Coll_Des, inviato, datainvio, Data_Creazione, ")
            strSQL.AppendLine("Data_Modifica, Username_Creazione, Username_Modifica, Validita_fine")
            strSQL.AppendLine(" FROM DOC_Collezioni ")

            'strSQL.AppendLine(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            strSQL.AppendLine(" WHERE GETDATE() < Validita_fine")

            'se devo ricercare per titolo
            If Not IsNothing(titoloCollezione) Then
                strSQL.AppendLine(" AND Coll_Titolo LIKE %" & Agro_SQL_SaveText_NULL(titoloCollezione) & "%")
            End If

            'se devo ricercare per descrizoone
            If Not IsNothing(desCollezione) Then
                strSQL.AppendLine(" AND Coll_Des LIKE %" & Agro_SQL_SaveText_NULL(titoloCollezione) & "%")
            End If

            strSQL.AppendLine(" ORDER BY Data_Creazione")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message & " QUERY: " & strSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    '-----------------------------------------------------------
    '--------------------------PUBBLICAZIONI--------------------
    '-----------------------------------------------------------

    'per andare a leggere le pubblicazioni - IN USO
    'se a FILTROULTIMO passo VALIDE vado a leggere solamente le pubblicazioni ATTIVE in termini temporali
    Public Function Leggi_Pubblicazioni(
                                ByVal IDPubblicazione As Integer?,
                                ByVal IDCollezione As Integer?,
                                ByVal Titolo As String,
                                ByVal IDLingua As Integer?,
                                ByVal filtroUltimo As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "GAP_Programmazione_R.Leggi_Pubblicazioni"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.AppendLine(" SELECT IDPubblicazione, IDCollezione, Titolo, Des, N_Ordine, inviato, datainvio, ")
            strSQL.AppendLine("Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica, ")
            strSQL.AppendLine("Validita_Inizio, Validita_fine, Pubblica, IDLingua, Copertina, DocumentoPDF")
            strSQL.AppendLine(" FROM DOC_Pubblicazioni ")

            'strSQL.AppendLine(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))       

            'mostro solamente le cpse pubblicate'
            strSQL.Append("WHERE PUBBLICA = 1")

            'se devo ridercare per pubblicazione
            If Not IsNothing(IDPubblicazione) Then
                strSQL.AppendLine(" AND IDPubblicazione = " & Agro_SQL_SaveNum_NULL(IDPubblicazione))
            End If

            'se devo ricercare per collezione
            If Not IsNothing(IDCollezione) Then
                strSQL.AppendLine(" AND IDCollezione = " & Agro_SQL_SaveNum_NULL(IDCollezione))
            End If

            'se devo ricercare per titolo
            If Not String.IsNullOrEmpty(Titolo) Then
                strSQL.AppendLine(" AND Titolo LIKE %" & Agro_SQL_SaveText_NULL(Titolo) & "%")
            End If

            'se devo specificare la lingua
            If Not IsNothing(IDLingua) Then
                strSQL.AppendLine(" AND IDLingua = " & Agro_SQL_SaveNum_NULL(IDLingua))
            End If

            If xFiltroAggiuntivo <> "" Then
                strSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            'nel caso in cui voglia solo LA PIU' RECENTE 
            If filtroUltimo.Equals("ultima_valida") Then
                strSQL.AppendLine(" AND Validita_Inizio = (SELECT MAX (Validita_Inizio) FROM DOC_Pubblicazioni WHERE GETDATE() < Validita_fine AND Validita_Inizio < GETDATE() AND PUBBLICA = 1)")
                'nel caso in cui voglia tutte quelle VALIDE
            ElseIf filtroUltimo.Equals("valide") Then
                strSQL.AppendLine(" AND Validita_Inizio < GETDATE()")
                strSQL.AppendLine(" AND Validita_Fine > GETDATE()")
                'nel caso in cui voglia le pubblicazioni da ARCHIVIO
            ElseIf filtroUltimo.Equals("archivio") Then
                strSQL.AppendLine(" AND Validita_Inizio < GETDATE()")
                strSQL.AppendLine(" AND Validita_Fine < GETDATE()")
            End If

            'ordino per numero di ordine come al solito
            strSQL.AppendLine(" ORDER BY N_Ordine DESC")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message & " QUERY: " & strSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    '----- cerco le pubblicazioni per titolo o descrizione
    Public Function Cerco_Pubblicazioni(ByVal titoloPubblicazione As String,
                                        ByVal descPubblicazione As String,
                                        ByVal IDLingua As Integer,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "GAP_Programmazione_R.Cerca_Pubblicazioni"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.AppendLine(" SELECT IDPubblicazione, IDCollezione, Titolo, Des, N_Ordine, inviato, datainvio, Data_Creazione, ")
            strSQL.AppendLine("Data_Modifica, Username_Creazione, Username_Modifica, Validita_Inizio, Validita_fine, IDLingua")
            strSQL.AppendLine(" FROM DOC_Collezioni ")

            'strSQL.AppendLine(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            strSQL.AppendLine(" WHERE GETDATE() < Validita_fine")

            'se devo ricercare per titolo
            If Not IsNothing(titoloPubblicazione) Then
                strSQL.AppendLine(" AND Titolo LIKE %" & Agro_SQL_SaveText_NULL(titoloPubblicazione) & "%")
            End If

            'se devo ricercare per descrizoone
            If Not IsNothing(descPubblicazione) Then
                strSQL.AppendLine(" AND Des LIKE %" & Agro_SQL_SaveText_NULL(descPubblicazione) & "%")
            End If

            'se devo specificare la lingua
            If Not IsNothing(IDLingua) Then
                strSQL.AppendLine(" AND IDLingua = " & Agro_SQL_SaveNum_NULL(IDLingua))
            End If

            'mostro solamente le cpse pubblicate'
            strSQL.Append(" AND PUBBLICA == 1")

            strSQL.AppendLine(" ORDER BY N_Ordine")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message & " QUERY: " & strSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    '-----------------------------------------------------------
    '--------------------------SEZIONI--------------------------
    '-----------------------------------------------------------

    '----- vado a leggere tutte le sezioni presenti: 
    'posso ricercarle per IDSezione IDPubblicazione IDCollezione o per Titolo o Sottotitolo
    'cosi posso intercettare la singola sezione, tutte le sezioni di una pubblicazione o di una collezione
    'oppure le sezioni che, in titolo e sottotitolo, contengono parole chiave utilizzate come chiave di ricerca
    'altrimenti me le restituisce tutte
    'posso anche specificare la lingua del documento
    Public Function Leggi_Sezioni(
                                ByVal IDSezione As Integer?,
                                ByVal IDPubblicazione As Integer?,
                                ByVal IDCollezione As Integer?,
                                ByVal Titolo As String,
                                ByVal Sottotitolo As String,
                                ByVal IDLingua As Integer?,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "GAP_Programmazione_R.Leggi_Sezioni"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.AppendLine(" SELECT IDSezione, IDPubblicazione, IDCollezione, N_Ordine, Titolo, Sottotitolo, ")
            strSQL.AppendLine("Des_Interna, AutoreEdizione, Gru_Cod, Veg_Cod, Cul_Cod, FF_Cod, AV_Cod, inviato, ")
            strSQL.AppendLine("datainvio, Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica, ")
            strSQL.AppendLine("Validita_Inizio, Validita_fine, Pubblica, IDLingua")
            strSQL.AppendLine(" FROM DOC_Sezioni ")

            'strSQL.AppendLine(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            'strSQL.AppendLine(" WHERE GETDATE() < Validita_fine")

            'mostro solamente le cpse pubblicate'
            strSQL.Append(" WHERE PUBBLICA = 1")

            'se devo ridercare per sezione
            If Not IsNothing(IDSezione) Then
                strSQL.AppendLine(" AND IDSezione = " & Agro_SQL_SaveNum_NULL(IDSezione))
            End If

            'se devo ridercare per pubblicazione
            If Not IsNothing(IDPubblicazione) Then
                strSQL.AppendLine(" AND IDPubblicazione = " & Agro_SQL_SaveNum_NULL(IDPubblicazione))
            End If

            'se devo ricercare per collezione
            If Not IsNothing(IDCollezione) Then
                strSQL.AppendLine(" AND IDCollezione = " & Agro_SQL_SaveNum_NULL(IDCollezione))
            End If

            'se devo ricercare per titolo
            If Not String.IsNullOrEmpty(Titolo) Then
                strSQL.AppendLine(" AND Titolo LIKE %" & Agro_SQL_SaveText_NULL(Titolo) & "%")
            End If

            'se devo ricercare per sottotitolo
            If Not String.IsNullOrEmpty(Sottotitolo) Then
                strSQL.AppendLine(" AND Sottotitolo LIKE %" & Agro_SQL_SaveText_NULL(Sottotitolo) & "%")
            End If

            ''se devo specificare la lingua
            'If Not IsNothing(IDLingua) Then
            '    strSQL.AppendLine(" AND IDLingua = " & Agro_SQL_SaveNum_NULL(IDLingua))
            'End If

            If xFiltroAggiuntivo <> "" Then
                strSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            'ordino le sezioni per numero ordine'
            strSQL.AppendLine(" ORDER BY N_Ordine")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message & " QUERY: " & strSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    '----- vado a leggere i testi delle sezioni: 
    'posso ricercarle per IDTesto IDSezione IDPubblicazione IDCollezione 
    'cosi posso intercettare il testo singolo, tutti i testi di una sezione, di una pubblicazione o di una collezione
    'e posso specificare la lingua del documento
    Public Function Leggi_Sezioni_Testi(
                                ByVal IDTesto As Integer?,
                                ByVal IDSezione As Integer?,
                                ByVal IDPubblicazione As Integer?,
                                ByVal IDCollezione As Integer?,
                                ByVal IDLingua As Integer?,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "GAP_Programmazione_R.Leggi_Sezioni_Testi"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.AppendLine(" SELECT IDTesto, IDSezione, IDPubblicazione, IDCollezione, Testo, ImmagineAllegato, ")
            strSQL.AppendLine("N_Ordine, inviato, datainvio, Data_Creazione, Data_Modifica, Username_Creazione, ")
            strSQL.AppendLine("Username_Modifica, Validita_Inizio, Validita_fine, Pubblica, IDLingua")
            strSQL.AppendLine(" FROM DOC_SezioniTesti ")

            'strSQL.AppendLine(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            'strSQL.AppendLine(" WHERE GETDATE() < Validita_fine")

            'mostro solamente le cpse pubblicate'
            strSQL.Append(" WHERE PUBBLICA = 1")

            'se devo ridercare per testo
            If Not IsNothing(IDTesto) Then
                strSQL.AppendLine(" AND IDTesto = " & Agro_SQL_SaveNum_NULL(IDTesto))
            End If

            'se devo ridercare per sezione
            If Not IsNothing(IDSezione) Then
                strSQL.AppendLine(" AND IDSezione = " & Agro_SQL_SaveNum_NULL(IDSezione))
            End If

            'se devo ridercare per pubblicazione
            If Not IsNothing(IDPubblicazione) Then
                strSQL.AppendLine(" AND IDPubblicazione = " & Agro_SQL_SaveNum_NULL(IDPubblicazione))
            End If

            'se devo ricercare per collezione
            If Not IsNothing(IDCollezione) Then
                strSQL.AppendLine(" AND IDCollezione = " & Agro_SQL_SaveNum_NULL(IDCollezione))
            End If

            ''se devo specificare la lingua
            'If Not IsNothing(IDLingua) Then
            '    strSQL.AppendLine(" AND IDLingua = " & Agro_SQL_SaveNum_NULL(IDLingua))
            'End If

            If xFiltroAggiuntivo <> "" Then
                strSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            'ordino i testi delle sezioni per numero ordine'
            strSQL.AppendLine(" ORDER BY N_Ordine")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message & " QUERY: " & strSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    '----- cerco le sezioni per titolo o sottotitolo
    Public Function Cerco_Sezioni(ByVal titoloSezione As String,
                                  ByVal sottotitoloSezione As String,
                                  ByVal IDLingua As Integer,
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                  ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "GAP_Programmazione_R.Cerca_Sezioni"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.AppendLine(" SELECT IDSezione, IDPubblicazione, IDCollezione, N_Ordine, Titolo, Sottotitolo, ")
            strSQL.AppendLine("Des_Interna, AutoreEdizione, Gru_Cod, Veg_Cod, Cul_Cod, FF_Cod, AV_Cod, inviato, ")
            strSQL.AppendLine("datainvio, Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica, ")
            strSQL.AppendLine("Validita_Inizio, Validita_fine, Pubblica, IDLingua")
            strSQL.AppendLine(" FROM DOC_Sezioni ")

            'strSQL.AppendLine(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            'strSQL.AppendLine(" WHERE GETDATE() < Validita_fine")

            'mostro solamente le cpse pubblicate'
            strSQL.Append(" WHERE PUBBLICA = 1")

            'se devo ricercare per titolo
            If Not IsNothing(titoloSezione) Then
                strSQL.AppendLine(" AND Titolo LIKE %" & Agro_SQL_SaveText_NULL(titoloSezione) & "%")
            End If

            'se devo ricercare per sottotitolo
            If Not IsNothing(sottotitoloSezione) Then
                strSQL.AppendLine(" AND Sottotitolo LIKE %" & Agro_SQL_SaveText_NULL(sottotitoloSezione) & "%")
            End If

            ''se devo specificare la lingua
            'If Not IsNothing(IDLingua) Then
            '    strSQL.AppendLine(" AND IDLingua = " & Agro_SQL_SaveNum_NULL(IDLingua))
            'End If

            strSQL.AppendLine(" ORDER BY N_Ordine")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message & " QUERY: " & strSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    '-----------------------------------------------------------
    '--------------------------CAPITOLI-------------------------
    '-----------------------------------------------------------

    '----- vado a leggere tutte le sezioni presenti: 
    'posso ricercarle per IDSezione IDPubblicazione IDCollezione o per Titolo o Sottotitolo
    'cosi posso intercettare il singolo capitolo, i capitoli di una sezione, di una pubblicazione o di una collezione
    'oppure andare a prendere tutti i capitoli che, in titolo ed in sottotitolo, contiene le parole che inserisci come chiave di ricerca
    'altrimenti me le restituisce tutte
    'posso anche specificare la lingua del documento
    Public Function Leggi_Capitoli(
                                ByVal IDCapitolo As Integer?,
                                ByVal IDSezione As Integer?,
                                ByVal IDPubblicazione As Integer?,
                                ByVal IDCollezione As Integer?,
                                ByVal Titolo As String,
                                ByVal Sottotitolo As String,
                                ByVal IDLingua As Integer?,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "GAP_Programmazione_R.Leggi_Capitoli"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.AppendLine("SELECT IDCapitolo, ")
            strSQL.AppendLine("IDSezione, ")
            strSQL.AppendLine("IDPubblicazione, ")
            strSQL.AppendLine("IDCollezione,  ")
            strSQL.AppendLine("N_Ordine, ")
            strSQL.AppendLine("Titolo,  ")
            strSQL.AppendLine("Sottotitolo, ")
            strSQL.AppendLine("Des_Interna, ")
            strSQL.AppendLine("AutoreEdizione, ")
            strSQL.AppendLine("Gru_Cod, ")
            strSQL.AppendLine("Veg_Cod, ")
            strSQL.AppendLine("Cul_Cod, ")
            strSQL.AppendLine("FF_Cod, ")
            strSQL.AppendLine("AV_Cod, ")
            strSQL.AppendLine("inviato,  ")
            strSQL.AppendLine("datainvio,  ")
            strSQL.AppendLine("Data_Creazione, ")
            strSQL.AppendLine("Data_Modifica, ")
            strSQL.AppendLine("Username_Creazione, ")
            strSQL.AppendLine("Username_Modifica, ")
            strSQL.AppendLine("Validita_Inizio, ")
            strSQL.AppendLine("Validita_fine, ")
            strSQL.AppendLine("Pubblica, ")
            strSQL.AppendLine("IDLingua")
            strSQL.AppendLine(" FROM DOC_Capitoli ")

            'strSQL.AppendLine(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            'strSQL.AppendLine(" WHERE GETDATE() < Validita_fine")

            'mostro solamente le cpse pubblicate'
            strSQL.Append(" WHERE PUBBLICA = 1")

            'se devo ridercare per capitolo
            If Not IsNothing(IDCapitolo) Then
                strSQL.AppendLine(" AND IDCapitolo = " & Agro_SQL_SaveNum_NULL(IDCapitolo))
            End If

            'se devo ridercare per sezione
            If Not IsNothing(IDSezione) Then
                strSQL.AppendLine(" AND IDSezione = " & Agro_SQL_SaveNum_NULL(IDSezione))
            End If

            'se devo ridercare per pubblicazione
            If Not IsNothing(IDPubblicazione) Then
                strSQL.AppendLine(" AND IDPubblicazione = " & Agro_SQL_SaveNum_NULL(IDPubblicazione))
            End If

            'se devo ricercare per collezione
            If Not IsNothing(IDCollezione) Then
                strSQL.AppendLine(" AND IDCollezione = " & Agro_SQL_SaveNum_NULL(IDCollezione))
            End If

            'se devo ricercare per titolo
            If Not String.IsNullOrEmpty(Titolo) Then
                strSQL.AppendLine(" AND Titolo LIKE %" & Agro_SQL_SaveText_NULL(Titolo) & "%")
            End If

            'se devo ricercare per sottotitolo
            If Not String.IsNullOrEmpty(Sottotitolo) Then
                strSQL.AppendLine(" AND Sottotitolo LIKE %" & Agro_SQL_SaveText_NULL(Sottotitolo) & "%")
            End If

            ''se devo specificare la lingua
            'If Not IsNothing(IDLingua) Then
            '    strSQL.AppendLine(" AND IDLingua = " & Agro_SQL_SaveNum_NULL(IDLingua))
            'End If

            If xFiltroAggiuntivo <> "" Then
                strSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            'ordino i capitoli per numero ordine'
            strSQL.AppendLine(" ORDER BY N_Ordine")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message & " QUERY: " & strSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    '----- vado a leggere i testi di un capitolo: 
    'posso ricercarle per IDTesto IDCAPITOLO IDSezione IDPubblicazione IDCollezione 
    'cosi posso intercettare il testo singolo, tutti i testi di un capitolo, di una sezione, di una pubblicazione o di una collezione
    'posso scegliere anche la lingua del documento
    Public Function Leggi_Capitoli_Testi(
                                ByVal IDTesto As Integer?,
                                ByVal IDCapitolo As Integer?,
                                ByVal IDSezione As Integer?,
                                ByVal IDPubblicazione As Integer?,
                                ByVal IDCollezione As Integer?,
                                ByVal IDLingua As Integer?,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "GAP_Programmazione_R.Leggi_Capitoli_Testi"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.AppendLine(" SELECT IDTesto, IDCapitolo, IDSezione, IDPubblicazione, IDCollezione, ")
            strSQL.AppendLine("Testo, ImmagineAllegato, N_Ordine, inviato, datainvio, Data_Creazione, ")
            strSQL.AppendLine("Data_Modifica, Username_Creazione, Username_Modifica, Validita_Inizio, ")
            strSQL.AppendLine("Validita_fine, Pubblica, IDLingua")
            strSQL.AppendLine(" FROM DOC_CapitoliTesti ")

            'strSQL.AppendLine(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            'strSQL.AppendLine(" WHERE GETDATE() < Validita_fine")

            'mostro solamente le cpse pubblicate'
            strSQL.Append(" WHERE PUBBLICA = 1")

            'se devo ridercare per testo
            If Not IsNothing(IDTesto) Then
                strSQL.AppendLine(" AND IDTesto = " & Agro_SQL_SaveNum_NULL(IDTesto))
            End If

            'se devo ridercare per capitolo
            If Not IsNothing(IDCapitolo) Then
                strSQL.AppendLine(" AND IDCapitolo = " & Agro_SQL_SaveNum_NULL(IDCapitolo))
            End If

            'se devo ridercare per sezione
            If Not IsNothing(IDSezione) Then
                strSQL.AppendLine(" AND IDSezione = " & Agro_SQL_SaveNum_NULL(IDSezione))
            End If

            'se devo ridercare per pubblicazione
            If Not IsNothing(IDPubblicazione) Then
                strSQL.AppendLine(" AND IDPubblicazione = " & Agro_SQL_SaveNum_NULL(IDPubblicazione))
            End If

            'se devo ricercare per collezione
            If Not IsNothing(IDCollezione) Then
                strSQL.AppendLine(" AND IDCollezione = " & Agro_SQL_SaveNum_NULL(IDCollezione))
            End If

            ''se devo specificare la lingua
            'If Not IsNothing(IDLingua) Then
            '    strSQL.AppendLine(" AND IDLingua = " & Agro_SQL_SaveNum_NULL(IDLingua))
            'End If

            If xFiltroAggiuntivo <> "" Then
                strSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            'ordino i testi dei capitoli per numero ordine'
            strSQL.AppendLine(" ORDER BY N_Ordine")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message & " QUERY: " & strSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    '----- cerco i capitoli per titolo o sottotitolo
    Public Function Cerco_Capitoli(ByVal titoloCapitolo As String,
                                    ByVal sottotitoloCapitolo As String,
                                    ByVal IDLingua As Integer,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "GAP_Programmazione_R.Cerca_Capitoli"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.AppendLine("SELECT IDCapitolo, ")
            strSQL.AppendLine("IDSezione, ")
            strSQL.AppendLine("IDPubblicazione, ")
            strSQL.AppendLine("IDCollezione,  ")
            strSQL.AppendLine("N_Ordine, ")
            strSQL.AppendLine("Titolo,  ")
            strSQL.AppendLine("Sottotitolo, ")
            strSQL.AppendLine("Des_Interna, ")
            strSQL.AppendLine("AutoreEdizione, ")
            strSQL.AppendLine("Gru_Cod, ")
            strSQL.AppendLine("Veg_Cod, ")
            strSQL.AppendLine("Cul_Cod, ")
            strSQL.AppendLine("FF_Cod, ")
            strSQL.AppendLine("AV_Cod, ")
            strSQL.AppendLine("inviato,  ")
            strSQL.AppendLine("datainvio,  ")
            strSQL.AppendLine("Data_Creazione, ")
            strSQL.AppendLine("Data_Modifica, ")
            strSQL.AppendLine("Username_Creazione, ")
            strSQL.AppendLine("Username_Modifica, ")
            strSQL.AppendLine("Validita_Inizio, ")
            strSQL.AppendLine("Validita_fine, ")
            strSQL.AppendLine("Pubblica, ")
            strSQL.AppendLine("IDLingua")
            strSQL.AppendLine(" FROM DOC_Capitoli ")

            'strSQL.AppendLine(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            'strSQL.AppendLine(" WHERE GETDATE() < Validita_fine")

            'mostro solamente le cpse pubblicate'
            strSQL.Append(" WHERE PUBBLICA = 1")

            'se devo ricercare per titolo
            If Not IsNothing(titoloCapitolo) Then
                strSQL.AppendLine(" AND Titolo LIKE %" & Agro_SQL_SaveText_NULL(titoloCapitolo) & "%")
            End If

            'se devo ricercare per sottotitolo
            If Not IsNothing(sottotitoloCapitolo) Then
                strSQL.AppendLine(" AND Sottotitolo LIKE %" & Agro_SQL_SaveText_NULL(sottotitoloCapitolo) & "%")
            End If

            ''se devo specificare la lingua
            'If Not IsNothing(IDLingua) Then
            '    strSQL.AppendLine(" AND IDLingua = " & Agro_SQL_SaveNum_NULL(IDLingua))
            'End If

            strSQL.AppendLine(" ORDER BY N_Ordine")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message & " QUERY: " & strSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    '-----------------------------------------------------------
    '--------------------------PARAGRAFI------------------------
    '-----------------------------------------------------------

    '----- vado a leggere tutte i paragrafi presenti: 
    'posso ricercarle per IDParagrafo IDSezione IDPubblicazione IDCollezione o per Titolo o Sottotitolo
    'cosi posso intercettare il singolo capitolo, i capitoli di una sezione, di una pubblicazione o di una collezione
    'oppure andare a prendere tutti i capitoli che, in titolo ed in sottotitolo, contiene le parole che inserisci come chiave di ricerca
    'altrimenti me le restituisce tutte
    'posso anche specificare la lingua del documento
    Public Function Leggi_Paragrafi(
                                ByVal IDParagrafo As Integer?,
                                ByVal IDCapitolo As Integer?,
                                ByVal IDSezione As Integer?,
                                ByVal IDPubblicazione As Integer?,
                                ByVal IDCollezione As Integer?,
                                ByVal Titolo As String,
                                ByVal Sottotitolo As String,
                                ByVal IDLingua As Integer?,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "GAP_Programmazione_R.Leggi_Paragrafi"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.AppendLine(" SELECT IDParagrafo, IDCapitolo, IDSezione, IDPubblicazione, IDCollezione, ")
            strSQL.AppendLine("N_Ordine, Titolo, Sottotitolo, Des_Interna, AutoreEdizione, Gru_Cod, ")
            strSQL.AppendLine("Veg_Cod, Cul_Cod, FF_Cod, AV_Cod, inviato, datainvio, Data_Creazione, ")
            strSQL.AppendLine("Data_Modifica, Username_Creazione, Username_Modifica, Validita_Inizio, ")
            strSQL.AppendLine("Validita_fine, Pubblica, IDLingua")
            strSQL.AppendLine(" FROM DOC_Paragrafi ")

            'strSQL.AppendLine(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            'strSQL.AppendLine(" WHERE GETDATE() < Validita_fine")

            'mostro solamente le cpse pubblicate'
            strSQL.Append(" WHERE PUBBLICA = 1")

            'se devo ridercare per paragrafo
            If Not IsNothing(IDParagrafo) Then
                strSQL.AppendLine(" AND IDParagrafo = " & Agro_SQL_SaveNum_NULL(IDParagrafo))
            End If

            'se devo ridercare per capitolo
            If Not IsNothing(IDCapitolo) Then
                strSQL.AppendLine(" AND IDCapitolo = " & Agro_SQL_SaveNum_NULL(IDCapitolo))
            End If

            'se devo ridercare per sezione
            If Not IsNothing(IDSezione) Then
                strSQL.AppendLine(" AND IDSezione = " & Agro_SQL_SaveNum_NULL(IDSezione))
            End If

            'se devo ridercare per pubblicazione
            If Not IsNothing(IDPubblicazione) Then
                strSQL.AppendLine(" AND IDPubblicazione = " & Agro_SQL_SaveNum_NULL(IDPubblicazione))
            End If

            'se devo ricercare per collezione
            If Not IsNothing(IDCollezione) Then
                strSQL.AppendLine(" AND IDCollezione = " & Agro_SQL_SaveNum_NULL(IDCollezione))
            End If

            'se devo ricercare per titolo
            If Not String.IsNullOrEmpty(Titolo) Then
                strSQL.AppendLine(" AND Titolo LIKE %" & Agro_SQL_SaveText_NULL(Titolo) & "%")
            End If

            'se devo ricercare per sottotitolo
            If Not String.IsNullOrEmpty(Sottotitolo) Then
                strSQL.AppendLine(" AND Sottotitolo LIKE %" & Agro_SQL_SaveText_NULL(Sottotitolo) & "%")
            End If

            ''se devo specificare la lingua
            'If Not IsNothing(IDLingua) Then
            '    strSQL.AppendLine(" AND IDLingua = " & Agro_SQL_SaveNum_NULL(IDLingua))
            'End If

            If xFiltroAggiuntivo <> "" Then
                strSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            'ordino i paragrafi per numero ordine'
            strSQL.AppendLine(" ORDER BY N_Ordine")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message & " QUERY: " & strSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    '----- vado a leggere i testi di un paragrafo: 
    'posso ricercarle per IDTesto IDParagrafo IDCAPITOLO IDSezione IDPubblicazione IDCollezione 
    'cosi posso intercettare il testo singolo, tutti i testi di un paragrafo, di un capitolo, di una sezione, di una pubblicazione o di una collezione
    'posso scegliere anche la lingua del documento
    Public Function Leggi_Paragrafi_Testi(
                                ByVal IDTesto As Integer?,
                                ByVal IDParagrafo As Integer?,
                                ByVal IDCapitolo As Integer?,
                                ByVal IDSezione As Integer?,
                                ByVal IDPubblicazione As Integer?,
                                ByVal IDCollezione As Integer?,
                                ByVal IDLingua As Integer?,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "GAP_Programmazione_R.Leggi_Paragrafo_Testi"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.AppendLine(" SELECT IDTesto, IDParagrafo, IDCapitolo, IDSezione, IDPubblicazione, IDCollezione, ")
            strSQL.AppendLine("Testo, ImmagineAllegato, N_Ordine, inviato, datainvio, Data_Creazione, Data_Modifica, ")
            strSQL.AppendLine("Username_Creazione, Username_Modifica, Validita_Inizio, Validita_fine, ")
            strSQL.AppendLine("Pubblica, IDLingua")
            strSQL.AppendLine(" FROM DOC_ParagrafiTesti ")

            'strSQL.AppendLine(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            'strSQL.AppendLine(" WHERE GETDATE() < Validita_fine")

            'mostro solamente le cpse pubblicate'
            strSQL.Append(" WHERE PUBBLICA = 1")

            'se devo ridercare per testo
            If Not IsNothing(IDTesto) Then
                strSQL.AppendLine(" AND IDTesto = " & Agro_SQL_SaveNum_NULL(IDTesto))
            End If

            'se devo ridercare per paragrafo
            If Not IsNothing(IDParagrafo) Then
                strSQL.AppendLine(" AND IDParagrafo = " & Agro_SQL_SaveNum_NULL(IDParagrafo))
            End If

            'se devo ridercare per capitolo
            If Not IsNothing(IDCapitolo) Then
                strSQL.AppendLine(" AND IDCapitolo = " & Agro_SQL_SaveNum_NULL(IDCapitolo))
            End If

            'se devo ridercare per sezione
            If Not IsNothing(IDSezione) Then
                strSQL.AppendLine(" AND IDSezione = " & Agro_SQL_SaveNum_NULL(IDSezione))
            End If

            'se devo ridercare per pubblicazione
            If Not IsNothing(IDPubblicazione) Then
                strSQL.AppendLine(" AND IDPubblicazione = " & Agro_SQL_SaveNum_NULL(IDPubblicazione))
            End If

            'se devo ricercare per collezione
            If Not IsNothing(IDCollezione) Then
                strSQL.AppendLine(" AND IDCollezione = " & Agro_SQL_SaveNum_NULL(IDCollezione))
            End If

            ''se devo specificare la lingua
            'If Not IsNothing(IDLingua) Then
            '    strSQL.AppendLine(" AND IDLingua = " & Agro_SQL_SaveNum_NULL(IDLingua))
            'End If

            If xFiltroAggiuntivo <> "" Then
                strSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            'ordino i testi dei capitoli per numero ordine'
            strSQL.AppendLine(" ORDER BY N_Ordine")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message & " QUERY: " & strSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    '----- cerco i capitoli per titolo o sottotitolo
    Public Function Cerco_Paragrafi(ByVal titoloParagrafo As String,
                                    ByVal sottotitoloParagrafo As String,
                                    ByVal IDLingua As Integer,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "GAP_Programmazione_R.Cerca_Paragrafi"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.AppendLine(" SELECT IDParagrafo, IDCapitolo, IDSezione, IDPubblicazione, IDCollezione, ")
            strSQL.AppendLine("N_Ordine, Titolo, Sottotitolo, Des_Interna, AutoreEdizione, Gru_Cod, ")
            strSQL.AppendLine("Veg_Cod, Cul_Cod, FF_Cod, AV_Cod, inviato, datainvio, Data_Creazione, ")
            strSQL.AppendLine("Data_Modifica, Username_Creazione, Username_Modifica, Validita_Inizio, ")
            strSQL.AppendLine("Validita_fine, Pubblica, IDLingua")
            strSQL.AppendLine(" FROM DOC_Paragrafi ")

            'strSQL.AppendLine(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            'strSQL.AppendLine(" WHERE GETDATE() < Validita_fine")

            'mostro solamente le cpse pubblicate'
            strSQL.Append(" WHERE PUBBLICA = 1")

            'se devo ricercare per titolo
            If Not IsNothing(titoloParagrafo) Then
                strSQL.AppendLine(" AND Titolo LIKE %" & Agro_SQL_SaveText_NULL(titoloParagrafo) & "%")
            End If

            'se devo ricercare per sottotitolo
            If Not IsNothing(sottotitoloParagrafo) Then
                strSQL.AppendLine(" AND Sottotitolo LIKE %" & Agro_SQL_SaveText_NULL(sottotitoloParagrafo) & "%")
            End If

            ''se devo specificare la lingua
            'If Not IsNothing(IDLingua) Then
            '    strSQL.AppendLine(" AND IDLingua = " & Agro_SQL_SaveNum_NULL(IDLingua))
            'End If

            strSQL.AppendLine(" ORDER BY N_Ordine")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message & " QUERY: " & strSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

End Class