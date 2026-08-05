Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider

Public Class Materie_PrimexLC_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '################################################################################
    Public Function AnnoProd_from_Lotto(ByVal Elem_Cod As Integer,
                                        ByVal Mat_cod As Integer,
                                        ByVal Lotto As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_PrimexLC_R.AnnoProd_from_Lotto"
        Dim MessaggioErrore As String = ""

        Dim Anno_Prod As String = ""
        Dim Dt_config As DataTable
        Dim Cifra_Start, Cifra_End As Integer

        Try

            Dt_config = Leggi_AnnoProduzione(Elem_Cod,
                                           Mat_cod,
                                           0, "",
                                           "", "",
                                           objParametri)

            'la query ritorna niente se è un vino da tavola o un vino senza annno configurato
            'altrimenti deve ritornare un record solo con l'anno di produzione
            If Not IsNothing(Dt_config) AndAlso Dt_config.Rows.Count > 0 Then

                Cifra_Start = Dt_config.Rows(0).Item("Cifra_Start")
                Cifra_End = Dt_config.Rows(0).Item("Cifra_End")
                'Lotto_Cod = Dt_config.Rows(0).Item("Lotto_Cod")

                If Cifra_Start = 0 And Cifra_End = 0 Then
                    Anno_Prod = ""
                Else
                    Anno_Prod = CStr(Lotto.Substring(Cifra_Start - 1, (CInt(Cifra_End) - CInt(Cifra_Start) + 1)))
                End If

            End If

        Catch ex As Exception
            Anno_Prod = ""
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Anno_Prod


    End Function


    '################################################################################
    Public Function Leggi_AnnoProduzione(ByVal Elem_Cod As Integer,
                                        ByVal Mat_Cod As Integer,
                                        ByVal Lotto_Cod As Integer,
                                        ByVal Lotto_Val As String,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_PrimexLC_R.Leggi_AnnoProduzione"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.Append("SELECT  Materie_PrimexLotto_Configurazione.Elem_Cod , Materie_PrimexLotto_Configurazione.Mat_Cod,")
            Stb.Append("         Materie_PrimexLotto_Configurazione.Cifra_Start, Materie_PrimexLotto_Configurazione.Cifra_End,Materie_PrimexLotto_Configurazione.Lotto_Cod " & vbCrLf)
            Stb.Append("         , Lotto_Des, Lotto_Des_Estesa  " & vbCrLf)
            Stb.Append("        , ISNULL(Lotto_Val,'') AS Lotto_Val, ISNULL(Lotto_alias,'') AS Lotto_alias  " & vbCrLf)

            Stb.Append(" FROM Lotto_Configurazione    " & vbCrLf)

            Stb.Append(" INNER JOIN Materie_PrimexLotto_Configurazione ON Materie_PrimexLotto_Configurazione.Piva_SuperUser = Lotto_Configurazione.Piva_SuperUser AND " & vbCrLf)
            Stb.Append("                Materie_PrimexLotto_Configurazione.Piva = Lotto_Configurazione.Piva AND " & vbCrLf)
            Stb.Append("                Materie_PrimexLotto_Configurazione.Elem_Cod = Lotto_Configurazione.Elem_Cod AND " & vbCrLf)
            Stb.Append("                 Materie_PrimexLotto_Configurazione.Lotto_Cod = Lotto_Configurazione.Lotto_Cod " & vbCrLf)

            'left join perchè non è detto che sia impostato un alias
            Stb.Append(" LEFT OUTER JOIN Lotto_Configurazione_Alias ON Lotto_Configurazione_Alias.Piva_SuperUser = Lotto_Configurazione.Piva_SuperUser AND " & vbCrLf)
            Stb.Append("                Lotto_Configurazione_Alias.Piva = Lotto_Configurazione.Piva AND " & vbCrLf)
            Stb.Append("                Lotto_Configurazione_Alias.Elem_Cod = Lotto_Configurazione.Elem_Cod AND " & vbCrLf)
            Stb.Append("                 Lotto_Configurazione_Alias.Lotto_Cod = Lotto_Configurazione.Lotto_Cod " & vbCrLf)

            'WHERE
            Stb.Append(" WHERE Lotto_Configurazione.Piva_SuperUser = '" & CStr(objParametri.PivaSuperUser) & "'" & vbCrLf)

            'importante! bisogna cercare solo gli anni di produzione
            Stb.Append(" AND (Lotto_Des like '%Anno%produz%' OR Lotto_Des like '%Anno%vendemmia%'  ) " & vbCrLf)

            'If Piva <> "" Then
            '    Stb.Append(" AND (Materie_Prime.Piva = '" & Agro_SQL_SaveText(Piva) & "' OR Materie_Prime.Sa_Cod = -1  )")
            'Else
            '    Stb.Append(" AND (Materie_Prime.Sa_Cod = -1)" & vbCrLf)
            'End If

            If Elem_Cod <> 0 Then
                Stb.Append(" AND Materie_PrimexLotto_Configurazione.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   " & vbCrLf)
            End If

            If Mat_Cod <> 0 Then
                Stb.Append(" AND Materie_PrimexLotto_Configurazione.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   " & vbCrLf)
            End If

            If Lotto_Cod <> 0 Then
                Stb.Append(" AND Materie_PrimexLotto_Configurazione.Lotto_Cod = " & Agro_SQL_SaveNum(Lotto_Cod) & "   " & vbCrLf)
            End If

            If Lotto_Val <> "" Then
                Stb.Append(" AND Lotto_Configurazione_Alias.Lotto_Val = '" & Agro_SQL_SaveText(Lotto_Val) & "'   " & vbCrLf)
            End If

            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT


    End Function


    '################################################################################
    'gestisce il lotto del prodotto (modalità normale o con configurazione)
    'questa funzione viene chiamata da tutti i documenti contabili,
    'da registro vinificazione e commercializzazione
    '(imbottigliamento e frizzanti no)
    Public Function Gestione_LottoProdotto(ByVal Piva As String,
                                           ByVal Elem_Cod As Integer,
                                           ByVal Mat_Cod As Integer,
                                           ByVal Lotto As String,
                                           ByVal moduliCliente As List(Of Integer),
                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                           Optional ByVal Mat_Cod_Principale As Integer = 0) As String

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_PrimexLC_R.Gestione_LottoProdotto"
        Dim MessaggioErrore As String

        Dim objContabHLP As New AgronicaCoreContabHLP.Contabilita
        Dim DettagliLotto As String = ""

        Try

            Select Case Elem_Cod

                'GESTIONE CATEGORIE SENSIBILI
                Case SEMENTI, ALTRE_MATERIE, MANGIMI,
                    SEMILAVORATI_VEGETALI, TRASFORMATI_VEGETALI, MATERIE_VEGETALI, BENI_CONFEZ_VEGETALE,
                    SEMILAVORATI_ANIMALI, TRASFORMATI_ANIMALI, MATERIE_ANIMALI, BENI_CONFEZ_ANIMALE

                    Dim Chk_Report As Integer = 0

                    Dim moduloCantine = moduliCliente.Contains(enum_Omni_Modulo_Generazione.Cantine)

                    'verifico se esiste la configurazione del lotto e nel caso ricavo i dettagli
                    DettagliLotto = Recupera_DettagliLotto_MateriePrime(Piva, Elem_Cod, Mat_Cod, Lotto, moduloCantine, False,
                                                                        "", "", objParametri,
                                                                        Chk_Report, Mat_Cod_Principale)

                    If DettagliLotto = "" Then

                        'caso clienti giasonline
                        'o
                        'clienti giaslan
                        'senza configurazione del lotto, 
                        'o con configurazione lotto ma senza vincoli di stampa
                        If Lotto <> "" And Lotto.ToLower <> "indefinito" Then

                            Dim Tipo_Lotto As String

                            'Tipo_Lotto = objContabHLP.TipoLotto_from_ElemCod(Elem_Cod)
                            Tipo_Lotto = "Lotto"

                            'modifica del 06/09/2012: alcune cantine han chiesto di non visualizzare il lotto
                            'usiamo Chk_Report come discriminante
                            '        DettagliLotto = " - " + Tipo_Lotto + " : " & Lotto
                            Select Case Elem_Cod
                                Case TRASFORMATI_VEGETALI, TRASFORMATI_ANIMALI
                                    If Chk_Report = 1 Then
                                        DettagliLotto = " - " + Tipo_Lotto + " : " & Lotto
                                    End If
                                Case Else
                                    'categorie gestite anche da giasonline
                                    'non verifico il Chk_Report perchè la tabella Materie_PrimexLotto_Configurazione non è gestita
                                    DettagliLotto = " - " + Tipo_Lotto + " : " & Lotto
                            End Select

                        End If

                    End If

            End Select

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DettagliLotto

    End Function



    Public Function Leggi_Proprieta_Aziende(ByVal Piva As String,
                                           ByVal Elem_Cod As Integer,
                                           ByVal Mat_Cod As Integer,
                                           ByVal Cod_Contatto As String,
                                           ByVal FinestraTemp_Inizio As Date,
                                           ByVal FinestraTemp_Fine As Date,
                                           ByRef objParametri As AgronicaCoreParametri
                                           ) As DataTable

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_Proprieta_Aziende()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.Append(" SELECT Lotto_Val1 From Materie_PrimexLotto_Proprieta, Materie_PrimexLotto_Configurazione  ")
            strSql.Append(" WHERE Materie_PrimexLotto_Proprieta.Piva_SuperUser = '" & Agro_SQL_SaveText(Trim(Piva_SuperUser)) & "' ")
            strSql.Append(" And Materie_PrimexLotto_Proprieta.Validita_Inizio <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " ")
            strSql.Append(" And Materie_PrimexLotto_Proprieta.Validita_Fine >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & "")
            strSql.Append(" And Materie_PrimexLotto_Configurazione.Piva_SuperUser = Materie_PrimexLotto_Proprieta.Piva_SuperUser")
            strSql.Append(" And Materie_PrimexLotto_Configurazione.Piva = Materie_PrimexLotto_Proprieta.Piva")
            strSql.Append(" And Materie_PrimexLotto_Configurazione.Elem_Cod = Materie_PrimexLotto_Proprieta.Elem_Cod ")
            strSql.Append(" And Materie_PrimexLotto_Configurazione.Mat_Cod = Materie_PrimexLotto_Proprieta.Mat_Cod")
            strSql.Append(" And Materie_PrimexLotto_Configurazione.ChkProprieta = 1 ")
            strSql.Append(" And Materie_PrimexLotto_Configurazione.Lotto_Cod = Materie_PrimexLotto_Proprieta.Lotto_Cod1 ")
            strSql.Append(" And Materie_PrimexLotto_Proprieta.Id_Proprieta = 4")



            If Elem_Cod <> 0 Then
                strSql.Append(" And Materie_PrimexLotto_Proprieta.Linea_Cod = " & Elem_Cod & " ")
            End If

            If Mat_Cod <> 0 Then
                strSql.Append(" And Materie_PrimexLotto_Proprieta.Mat_Cod = " & Mat_Cod & " ")
            End If

            If Cod_Contatto <> "" Then
                strSql.Append("  AND Materie_PrimexLotto_Proprieta.Proprieta_Val Like '%" & Cod_Contatto & "%'")
            End If


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------



        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("" & nomeRoutine & " :  " & messaggioErrore)
        End Try


        Return dt

    End Function




    '#############################################################################################################
    'modificata il 06/09/2012: restituito Chk_Report
    Private Function Recupera_DettagliLotto_MateriePrime(ByVal Piva As String,
                                                         ByVal Elem_Cod As Integer,
                                                         ByVal Mat_Cod As Integer,
                                                         ByVal Lotto As String,
                                                         ByVal moduloCantine As Boolean,
                                                         ByVal Flag_Mat_Des As Boolean,
                                                         ByVal xFiltroAggiuntivo As String,
                                                         ByVal xOrderBy As String,
                                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                         ByRef Chk_Report As Integer,
                                                         Optional ByVal Mat_Cod_Principale As Integer = 0) As String

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_PrimexLC_R.Recupera_DettagliLotto_MateriePrime"
        Dim MessaggioErrore As String

        Dim DT As DataTable
        Dim Dettagli_MateriaPrima As String = ""
        Dim i As Integer = 0
        Dim Lotto_Des As String = ""
        Dim Lotto_Val As String = ""
        Dim Lotto_Alias As String = ""
        Dim Cifra_Start As Integer = 0
        Dim Cifra_End As Integer = 0
        Dim Cifra_Length As Integer = 0
        Dim Selezione_Lotto As String = ""
        Dim debug As Boolean

        Try

            'Dim Lotto_Cod As Integer
            'Dim Lotto_Cod_Temp As Integer = 0
            'Dim Flag_Impostato As Boolean = False

            'se il lotto non è definito, inutile verificare la sua configurazione
            If Lotto <> "" AndAlso Lotto.ToLower <> "indefinito" Then

                'il lotto è valorizzato, verifico se esiste una configurazione
                ' Giulia: 30/8/2017: in alcuni casi il mat_cod che mi arriva è in realtà quello dell'alias, che non ha il collegamento sul lotto,
                '   che invece è collegato con il mat_cod principale (al quale è legato l'alias). Quindi se mi è arrivato un mat_cod_principale uso quello,
                '   perché sono certa che non si tratti di un alias
                DT = MateriePrime_LottoConfigurazione_Leggi(Piva, 0,
                                                            Elem_Cod,
                                                            If(Mat_Cod_Principale <> 0, Mat_Cod_Principale, Mat_Cod),
                                                            "", 0, "",
                                                            AGRODATAINIZIO, AGRODATAFINE,
                                                            "", 0, "", True,
                                                            xFiltroAggiuntivo, xOrderBy,
                                                            objParametri)

                '17/01/2023 Giulia: di default facciamo sempre stampare il lotto,
                'a meno che non esista la configurazione su tabella e questa mi dica che non lo devo stampare.
                'Escludiamo cmq le cantine
                If moduloCantine = False Then
                    Chk_Report = 1
                End If

                If Not IsNothing(DT) AndAlso DT.Rows.Count <> 0 Then

                    'se voglio che nella stringa di ritorno ci sia anche il mat_des
                    If Flag_Mat_Des = True Then
                        Dettagli_MateriaPrima = DT.Rows(0).Item("Mat_Des")
                    End If

                    'ci possono essere tante righe 
                    'tante quante le 'sezioni' di cui può essere composto il lotto di quella categoria di magazzino (uno o più lotto_cod)
                    'per i valori che ognuna di queste 'sezioni' può avere (uno o più lotto_val)
                    For i = 0 To DT.Rows.Count - 1

                        Chk_Report = DT.Rows(i).Item("ChkReport")

                        'se la 'sezione' di lotto è stata impostata per essere visibile nella stampa
                        If Chk_Report = 1 OrElse moduloCantine = False Then

                            'leggo i dati della tabella Lotto_Configurazione
                            '(se è impostata una configurazione del lotto per la categoria di magazzino selezionata,
                            'in questa tabella ci sono sempre dei valori)

                            'Lotto_Cod = DT.Rows(i).Item("Lotto_Cod")

                            'If Lotto_Cod_Temp = 0 Then
                            '    Lotto_Cod_Temp = Lotto_Cod
                            'End If

                            'If Lotto_Cod_Temp <> Lotto_Cod Then
                            '    Lotto_Cod_Temp = Lotto_Cod
                            '    If Flag_Impostato = False Then
                            '        Dettagli_MateriaPrima += " - " + Lotto_Des + ": " + Selezione_Lotto
                            '    End If
                            '    Flag_Impostato = False
                            'End If

                            Lotto_Des = DT.Rows(i).Item("Lotto_Des")
                            Cifra_Start = DT.Rows(i).Item("Cifra_Start")
                            Cifra_End = DT.Rows(i).Item("Cifra_End")

                            If Cifra_Start <> 0 AndAlso Cifra_End <> 0 Then

                                Cifra_Length = Cifra_End - Cifra_Start + 1

                                'leggo i dati della tabella Lotto_Configurazione_Alias
                                '(in questa tabella non è detto che ci siano dei valori,
                                'se non sono impostati degli alias, lotto_val e lotto_alias sono = "")
                                Lotto_Val = DT.Rows(i).Item("Lotto_Val")
                                Lotto_Alias = DT.Rows(i).Item("Lotto_Alias")

                                'estrapolo dal lotto, la selezione inerente alla 'sezione' in oggetto
                                Selezione_Lotto = Mid(Lotto, Cifra_Start, Cifra_Length)

                                'verifico se c'è un alias impostato
                                If Lotto_Val <> "" Then
                                    'è impostato un'alias 

                                    'devo visualizzare solo l'alias del valore impostato, non di tutti i possibili valori
                                    If Selezione_Lotto = Lotto_Val Then
                                        'Flag_Impostato = True
                                        Dettagli_MateriaPrima += " - " + Lotto_Des + ": " + Lotto_Alias
                                    End If
                                Else
                                    'non è impostato l'alias
                                    'Flag_Impostato = True
                                    'visualizzo la selezione del lotto estrapolata
                                    Dettagli_MateriaPrima += " - " + Lotto_Des + ": " + Selezione_Lotto
                                End If
                            Else
                                'Cifra_Start Cifra_End non valorizzati
                                debug = True
                            End If

                        End If 'visibilità del lotto nella stampa

                    Next

                Else
                    'non esiste la configurazione
                    Dettagli_MateriaPrima = ""
                End If
            Else
                'il lotto non è valorizzato -> errore
                Dettagli_MateriaPrima = ""
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


        Return Dettagli_MateriaPrima


    End Function


    '###############################################################################
    'legge la configurazione del lotto (per le categorie di magazzino 'sensibili')
    'Optional ByVal Piva As String = "", _
    'Optional ByVal Sa_Cod As Integer = 0, _
    ' Optional ByVal Elem_Cod As Integer = 0, _
    ' Optional ByVal Mat_Cod As Integer = 0, _
    ' Optional ByVal Cod_Articolo As String = "", _
    'Optional ByVal Lotto_Cod As Integer = 0, _
    ' Optional ByVal Lotto_Val As String = "", _
    ' Optional ByVal FinestraTemp_Inizio As String = "01/01/1900", _
    ' Optional ByVal FinestraTemp_Fine As String = "31/12/2100", _
    'Optional ByVal RicercaTesto As String = "", _
    ' Optional ByVal Mat_Cod_Origine As Integer = 0, _
    'Optional ByVal Piva_SuperUser_Origine As String = "", _
    'Optional ByVal Flag_AncheImportatati As Boolean = False
    Public Function MateriePrime_LottoConfigurazione_Leggi(ByVal Piva As String, _
                                                            ByVal Sa_Cod As Integer, _
                                                            ByVal Elem_Cod As Integer, _
                                                            ByVal Mat_Cod As Integer, _
                                                            ByVal Cod_Articolo As String, _
                                                            ByVal Lotto_Cod As Integer, _
                                                            ByVal Lotto_Val As String, _
                                                            ByVal FinestraTemp_Inizio As String, _
                                                            ByVal FinestraTemp_Fine As String, _
                                                            ByVal RicercaTesto As String, _
                                                            ByVal Mat_Cod_Origine As Integer, _
                                                            ByVal Piva_SuperUser_Origine As String, _
                                                            ByVal Flag_AncheImportatati As Boolean, _
                                                            ByVal xFiltroAggiuntivo As String, _
                                                            ByVal xOrderBy As String, _
                                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable


        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_PrimexLC_R.MateriePrime_LottoConfigurazione_Leggi"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '----------------------------------------------------
            '--- Preparo la Query SQL ---------------------------
            '----------------------------------------------------

            Stb.Length = 0

            Stb.Append("SELECT   Materie_Prime.*,  ")
            Stb.Append("        Materie_PrimexLotto_Configurazione.Lotto_Cod, Materie_PrimexLotto_Configurazione.ChkListini, Materie_PrimexLotto_Configurazione.ChkReport, " & vbCrLf)
            Stb.Append("         Lotto_Configurazione.Lotto_Des,  " & vbCrLf)
            Stb.Append("         Materie_PrimexLotto_Configurazione.Cifra_Start, Materie_PrimexLotto_Configurazione.Cifra_End, " & vbCrLf)
            Stb.Append("        ISNULL(Lotto_Configurazione_Alias.Lotto_Val, '') AS Lotto_Val, ISNULL(Lotto_Configurazione_Alias.Lotto_Alias, '') AS Lotto_Alias " & vbCrLf)

            Stb.Append(" FROM Materie_Prime    " & vbCrLf)

            'join con UtentiXImprese
            Stb.Append(" INNER  JOIN UtentiXImprese ON Materie_Prime.Piva = UtentiXImprese.PIVA " & vbCrLf)

            'inner join poichè voglio sapere le info sul lotto
            Stb.Append(" INNER JOIN Materie_PrimexLotto_Configurazione ON Materie_Prime.Elem_Cod = Materie_PrimexLotto_Configurazione.Elem_Cod AND Materie_Prime.Mat_Cod = Materie_PrimexLotto_Configurazione.Mat_Cod " & vbCrLf)

            Stb.Append(" INNER JOIN Lotto_Configurazione ON Materie_PrimexLotto_Configurazione.Piva_SuperUser = Lotto_Configurazione.Piva_SuperUser AND " & vbCrLf)
            Stb.Append("                Materie_PrimexLotto_Configurazione.Piva = Lotto_Configurazione.Piva AND " & vbCrLf)
            Stb.Append("                Materie_PrimexLotto_Configurazione.Elem_Cod = Lotto_Configurazione.Elem_Cod AND " & vbCrLf)
            Stb.Append("                 Materie_PrimexLotto_Configurazione.Lotto_Cod = Lotto_Configurazione.Lotto_Cod " & vbCrLf)

            'left join perchè non è detto che sia impostato un alias
            Stb.Append(" LEFT OUTER JOIN Lotto_Configurazione_Alias ON Lotto_Configurazione_Alias.Piva_SuperUser = Lotto_Configurazione.Piva_SuperUser AND " & vbCrLf)
            Stb.Append("                Lotto_Configurazione_Alias.Piva = Lotto_Configurazione.Piva AND " & vbCrLf)
            Stb.Append("                Lotto_Configurazione_Alias.Elem_Cod = Lotto_Configurazione.Elem_Cod AND " & vbCrLf)
            Stb.Append("                 Lotto_Configurazione_Alias.Lotto_Cod = Lotto_Configurazione.Lotto_Cod " & vbCrLf)

            'WHERE
            Stb.Append(" WHERE UtentiXImprese.[USER] = '" & CStr(objParametri.PivaSuperUser) & "'" & vbCrLf)

            Stb.Append(" AND Materie_Prime.Validita_inizio <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " " & vbCrLf)
            Stb.Append(" AND Materie_Prime.Validita_Fine >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " " & vbCrLf)

            If Piva <> "" Then
                Stb.Append(" AND (Materie_Prime.Piva = '" & Agro_SQL_SaveText(Piva) & "' OR Materie_Prime.Sa_Cod = -1  )")
            Else
                Stb.Append(" AND (Materie_Prime.Sa_Cod = -1)" & vbCrLf)
            End If

            Stb.Append(" AND ( Materie_PrimexLotto_Configurazione.Piva_SuperUser = UtentiXImprese.[USER] )" & vbCrLf)

            If RicercaTesto <> "" Then
                Stb.Append(" AND Materie_Prime.Mat_Des like '%" & Agro_SQL_SaveText(RicercaTesto) & "%'   " & vbCrLf)
            End If

            If Cod_Articolo <> "" Then
                Stb.Append(" AND (Materie_Prime.Cod_Articolo = '" & Agro_SQL_SaveText(Cod_Articolo) & "' " & vbCrLf)
            End If

            If Elem_Cod <> 0 Then
                Stb.Append(" AND Materie_Prime.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   " & vbCrLf)
            End If

            If Mat_Cod <> 0 Then
                Stb.Append(" AND Materie_Prime.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   " & vbCrLf)
            End If

            If Lotto_Cod <> 0 Then
                Stb.Append(" AND Materie_Prime.Lotto_Cod = " & Agro_SQL_SaveNum(Lotto_Cod) & "   " & vbCrLf)
            End If

            If Lotto_Val <> "" Then
                Stb.Append(" AND Lotto_Configurazione_Alias.Lotto_Val = '" & Agro_SQL_SaveText(Lotto_Val) & "'   " & vbCrLf)
            End If

            'GIAS 2 GIAS 
            If Mat_Cod_Origine <> 0 Then
                Stb.Append(" AND  Materie_Prime.Mat_Cod_Origine = " & Agro_SQL_SaveNum(Mat_Cod_Origine) & "   " & vbCrLf)
            End If

            If Piva_SuperUser_Origine <> "" Then
                Stb.Append(" AND  Materie_Prime.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'" & vbCrLf)
            End If

            If Flag_AncheImportatati = False Then
                Stb.Append(" AND  Materie_Prime.Mat_Cod_Origine = " & Agro_SQL_SaveNum(0) & " " & vbCrLf)
            End If
            'FINE GIAS 2 GIAS

            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    Stb.Append(" AND   Materie_PrimexLotto_Configurazione.Inviato >=0 ")
                    Stb.Append(" AND   Lotto_Configurazione.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    Stb.Append(" AND   Materie_PrimexLotto_Configurazione.Inviato =-1 ")
                    Stb.Append(" AND   Lotto_Configurazione.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                Stb.Append(" ORDER BY Materie_Prime.Mat_Des Asc ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT


    End Function



    '#####################################################################################
    Public Function Leggi_xAltoLivello( _
                         ByVal Piva As String, _
                        ByVal Sa_Cod As Integer, _
                        ByVal Elem_Cod As Integer, _
                        ByVal Pro_Cod As Integer, _
                        ByVal Mat_Cod As Integer, _
                        ByVal Lotto_Cod As Integer, _
                        ByVal ChkListini As Integer, _
                        ByVal ChkReport As Integer, _
                        ByVal ChkProprieta As Integer, _
                                 ByVal xFiltroAggiuntivo As String, _
                                 ByVal xOrderBy As String, _
                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                 ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_PrimexLC_R.Leggi_xAltoLivello()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT DISTINCT Materie_PrimexLotto_Configurazione.*, Lotto_Configurazione.Lotto_Des, Lotto_Configurazione.Lotto_Des_Estesa, Lotto_Configurazione.Tipo ")
            StrSQL.Append(" FROM  Materie_PrimexLotto_Configurazione, Lotto_Configurazione ")
            StrSQL.Append(" WHERE Materie_PrimexLotto_Configurazione.Piva_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND   Materie_PrimexLotto_Configurazione.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   Materie_PrimexLotto_Configurazione.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   Lotto_Configurazione.Piva_SuperUser = Materie_PrimexLotto_Configurazione.Piva_SuperUser ")
            StrSQL.Append(" AND   Lotto_Configurazione.Lotto_Cod = Materie_PrimexLotto_Configurazione.Lotto_Cod  ")
            StrSQL.Append(" AND   Lotto_Configurazione.Elem_Cod = Materie_PrimexLotto_Configurazione.Elem_Cod  ")

            'Nota Marco: Sa_Cod non Utilizzato (messo in interfaccia solo per standardizzazione)
            StrSQL.Append(" AND (Materie_PrimexLotto_Configurazione.Piva = '" & Agro_SQL_SaveText(Piva) & "' Or Materie_PrimexLotto_Configurazione.Sa_Cod = -1)  ")

            If Elem_Cod <> 0 Then
                StrSQL.Append(" AND Materie_PrimexLotto_Configurazione.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
            End If

            If Pro_Cod <> 0 Then
                StrSQL.Append(" AND Materie_PrimexLotto_Configurazione.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   ")
            End If

            If Mat_Cod <> 0 Then
                StrSQL.Append(" AND Materie_PrimexLotto_Configurazione.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
            End If

            If Lotto_Cod <> 0 Then
                StrSQL.Append(" AND Materie_PrimexLotto_Configurazione.Lotto_Cod = " & Agro_SQL_SaveNum(Lotto_Cod) & "   ")
            End If

            If ChkListini <> -1 Then
                StrSQL.Append(" AND Materie_PrimexLotto_Configurazione.ChkListini = " & Agro_SQL_SaveNum(ChkListini) & "   ")
            End If

            If ChkReport <> -1 Then
                StrSQL.Append(" AND Materie_PrimexLotto_Configurazione.ChkReport = " & Agro_SQL_SaveNum(ChkReport) & "   ")
            End If

            If ChkProprieta <> -1 Then
                StrSQL.Append(" AND Materie_PrimexLotto_Configurazione.ChkProprieta  = " & Agro_SQL_SaveNum(ChkProprieta) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Materie_PrimexLotto_Configurazione.Inviato >=0 ")
                    StrSQL.Append(" AND   Lotto_Configurazione.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Materie_PrimexLotto_Configurazione.Inviato =-1 ")
                    StrSQL.Append(" AND   Lotto_Configurazione.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Materie_PrimexLotto_Configurazione.Piva, Materie_PrimexLotto_Configurazione.Elem_Cod, Materie_PrimexLotto_Configurazione.Pro_Cod, Materie_PrimexLotto_Configurazione.Mat_Cod, Materie_PrimexLotto_Configurazione.Cifra_Start ASC ")
            End If



            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function


End Class


'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§


Public Class Materie_PrimexLC_W
    Inherits AgronicaCoreDataProvider.DataProvider


    '###########################################################
    Public Function Scrivi( _
                            ByVal PIVA As String, _
                            ByVal Sa_Cod As Int32, _
                            ByVal Elem_Cod As Int32, _
                            ByVal Pro_Cod As Int32, _
                            ByVal Mat_Cod As Int32, _
                            ByVal Lotto_Cod As Int32, _
                            ByVal ChkListini As Integer, _
                            ByVal ChkReport As Integer, _
                            ByVal Validita_Inizio As Date, _
                                ByVal Validita_Fine As Date, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Materie_PrimexLC_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" INSERT INTO Materie_PrimexLotto_Configurazione ")
            StrSQL.Append("         ( ")
            StrSQL.Append("          Piva_SuperUser, Piva,        Sa_Cod,        Elem_Cod,      Pro_Cod,   ")
            StrSQL.Append("          Mat_Cod,        Lotto_Cod,   ChkListini,    ChkReport,                ")

            StrSQL.Append("          Inviato,            DataInvio, ")
            StrSQL.Append("          Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("          UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("          Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("         ) ")

            StrSQL.Append(" VALUES ( ")
            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(PIVA) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Elem_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Pro_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Mat_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Lotto_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(ChkListini) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(ChkReport) & "  ")

            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            StrSQL.Append(") ")


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function


    '#########################################################
    Public Function Modifica( _
                            ByVal Piva As String, _
                            ByVal Sa_Cod As Int32, _
                            ByVal Elem_Cod As Int32, _
                            ByVal Pro_Cod As Int32, _
                            ByVal Mat_Cod As Int32, _
                            ByVal Lotto_Cod As Int32, _
                            ByVal ChkListini As Int16, _
                            ByVal ChkReport As Int16, _
                            ByVal Validita_Inizio As Date, _
                                ByVal Validita_Fine As Date, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Materie_PrimexLC_W.Modifica()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            If objParametri.PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva_SuperUser obbligatorio)")
            End If

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            '---------------------------------------------

            StrSQL.Length = 0

            StrSQL.Append(" UPDATE Materie_PrimexLotto_Configurazione SET ")
            StrSQL.Append("    ChkListini           = " & Agro_SQL_SaveNum(ChkListini) & "  ")
            StrSQL.Append("   ,ChkReport            = " & Agro_SQL_SaveNum(ChkReport) & "  ")
            StrSQL.Append("   ,UserName_Modifica    = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'  ")
            StrSQL.Append("   ,Validita_Inizio      =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("   ,Validita_Fine        =  " & Agro_SQL_SaveDate(Validita_Fine))


            StrSQL.Append(" WHERE Piva_SuperUser =  '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "'   ")
            StrSQL.Append(" AND   Piva           =  '" & Agro_SQL_SaveText(Trim(Piva)) & "'   ")

            'Sa_Cod Non Utilizzato per Evitare GUAI

            If Elem_Cod <> 0 Then
                StrSQL.Append(" AND Materie_PrimexLotto_Configurazione.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
            End If

            If Pro_Cod <> 0 Then
                StrSQL.Append(" AND Materie_PrimexLotto_Configurazione.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   ")
            End If

            If Mat_Cod <> 0 Then
                StrSQL.Append(" AND Materie_PrimexLotto_Configurazione.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
            End If

            If Lotto_Cod <> 0 Then
                StrSQL.Append(" AND Materie_PrimexLotto_Configurazione.Lotto_Cod = " & Agro_SQL_SaveNum(Lotto_Cod) & "   ")
            End If



            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp


    End Function


    '#############################################################################
    Public Function Cancella( _
                            ByVal Piva As String, _
                            ByVal Sa_Cod As Int32, _
                            ByVal Elem_Cod As Int32, _
                            ByVal Pro_Cod As Int32, _
                            ByVal Mat_Cod As Int32, _
                            ByVal Lotto_Cod As Int32, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Materie_PrimexLC_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""
        '   Sa_Cod = 0
        '   Id_Trasformazione = 0

        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If objParametri.PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva_SuperUser obbligatorio)")
            End If

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If


            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Materie_PrimexLotto_Configurazione ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0 ")

            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM Materie_PrimexLotto_Configurazione ")
                StrSQL.Append(" WHERE  1=1 ")

            End If

            StrSQL.Append(" AND     Piva_SuperUser =  '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "'   ")
            StrSQL.Append(" AND     Piva           =  '" & Agro_SQL_SaveText(Trim(Piva)) & "'   ")


            'Nota Marco: Sa_Cod non Utilizzato (messo in interfaccia solo per standardizzazione)

            If Elem_Cod <> 0 Then
                StrSQL.Append(" AND Materie_PrimexLotto_Configurazione.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
            End If

            If Pro_Cod <> 0 Then
                StrSQL.Append(" AND Materie_PrimexLotto_Configurazione.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   ")
            End If

            If Mat_Cod <> 0 Then
                StrSQL.Append(" AND Materie_PrimexLotto_Configurazione.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
            End If

            If Lotto_Cod <> 0 Then
                StrSQL.Append(" AND Materie_PrimexLotto_Configurazione.Lotto_Cod = " & Agro_SQL_SaveNum(Lotto_Cod) & "   ")
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function





End Class
