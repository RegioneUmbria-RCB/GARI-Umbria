Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreEntityFramework
Imports System.Transactions
Imports System.Data.Common
Imports System.Data.Entity
Imports System.Configuration

Public Class Agro_Sequenze
    Inherits AgronicaCoreDataProvider.DataProvider

    'NB: I nomi delle tabelle devono essere tutti TOLOWER
    Public SequenceTable As New List(Of String) From {
        "agenda",
        "movimenti",
        "movimenti_dettagli",
        "movimenti_dettagli_tecnici",
        "movimenti_dettagli_tecnici_extra",
        "idtestatatemp",
        "ricette",
        "ricette_operazioni",
        "ricette_dettaglio_tecnico",
        "ricette_dettagli",
        "ricette_destinazioni",
        "raccoglitore",
        "gis_entita",
        "gis_elementigrafici",
        "particellecatastali",
        "impresa_progetto",
        "agronica_log_invio_chiamate",
        "indirizzi",
        "uma_lavorazioni_parziali",
        "uma_richieste_lavorazioni",
        "uma_richieste_testata",
        "uma_vendite",
        "gis_layeranalysisconfig_exec_log",
        "passaggiodistato_cod",
        "ricette_zoo",
        "ricette_zoo_agenda",
        "ricette_zoo_movimenti",
        "ricette_zoo_dettagli",
        "ricette_zoo_destinazioni",
        "ricette_zoo_dettaglio_tecnico"
    }

    Public SequenceTableExceptions As New List(Of String) From {
        "jDeereDataModel_EntitaGIAS",
        "materie_prime_campionature",
        "ws_vivaiPassaporti_operazioni",
        "regolamentiRMA",
        "PrincipiAttiviRMA",
        "GIS_LayerTilesDescrizione",
        "lineaproduzione",
        "linea_classe_produzione",
        "preparazione",
        "pagamenti_causali",
        "imprese_sezionali",
        "causaletrasporto",
        "liquidita",
        "ist_credito",
        "TabellaRMA",
        "DerrateCodifica"
    }
    '"particellecatastali",
    '"Gis_entita",
    '"Gis_elementiGrafici",

    '##############################################################################################
    'Public Function Calcola_BaseCode(ByVal ProgressivoGias As Long,
    '                                 ByRef TopCode As Long,
    '                                 ByRef BaseCode As Long,
    '                                 ByVal DirectoryLOG As String,
    '                                 ByVal FileLOG As String,
    '                                 ByVal IdentificatoreUtente As String
    '                                 ) As Boolean

    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Agro_Sequenze.Calcola_BaseCode()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    Dim costante As Long

    '    Try

    '        costante = (2 ^ 17)
    '        '------------------------------------------
    '        BaseCode = ProgressivoGias * costante
    '        TopCode = BaseCode + (costante - 1)
    '        '------------------------------------------

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & NomeRoutine & "] :   " & MessaggioErrore)

    '    Finally

    '        StrSQL = Nothing

    '    End Try

    '    Return xRisp

    'End Function

    Public Function Calcola_BaseCode(ByVal ProgressivoGias As Long,
                                     ByRef TopCode As Long,
                                     ByRef BaseCode As Long,
                                     ByRef objParametri As AgronicaCoreParametri
                                     ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Agro_Sequenze.Calcola_BaseCode()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Dim costante As Long

        Try

            costante = (2 ^ 17)
            '------------------------------------------
            BaseCode = ProgressivoGias * costante
            TopCode = BaseCode + (costante - 1)
            '------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        Finally

            StrSQL = Nothing

        End Try

        Return xRisp

    End Function

    '############################################################################
    Public Function NuovoId_Campi(ByVal Piva As String,
                                  ByVal Sa_Cod As Long,
                                  ByVal Base As Long,
                                  ByVal Fine As Long,
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                  Optional ByVal UtilizzaTransazione As Boolean = True
                                                ) As Long


        Dim NomeRoutine As String = "AgronicaCoreDataProvider.Agro_Sequenze.NuovoId_Campi()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim xRisp As Boolean = False
        Dim NuovoValore As Int32

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Sa_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            End If


            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" SELECT   * ")
            StrSQL.Append(" FROM     SeqCampi ")
            StrSQL.Append(" WHERE    PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" AND      Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")


            '---------------------------------------------

            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)

            If DT.Rows.Count <> 0 Then

                NuovoValore = CInt(DT.Rows(0).Item("Campo_Cod")) + 1

                'Se il nuovo valore supera l'ultimo valore valido ho un errore ...
                If NuovoValore > CInt(DT.Rows(0).Item("End")) Then

                    'Imposto un valore dummy
                    NuovoValore = -1

                    'Genero un errore
                    Throw New Exception("L'indice ha raggiunto il limite superiore")

                Else

                    StrSQL.Length = 0
                    StrSQL.Append(" UPDATE SeqCampi ")
                    StrSQL.Append(" SET Campo_Cod = " & Agro_SQL_SaveNum(NuovoValore) & " ")

                    StrSQL.Append(" WHERE    PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
                    StrSQL.Append(" AND      Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")

                    'xRisp = EseguiQuery_Scrittura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)
                    '--------------------------------------------------------------------------
                    xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
                    '--------------------------------------------------------------------------

                    If Not xRisp Then

                        'Imposto un valore dummy
                        NuovoValore = -1

                        'Genero un errore
                        Throw New Exception("Query di aggiornamento non riuscita.")

                    End If

                End If
            Else

                NuovoValore = Base + 1

                StrSQL.Length = 0
                StrSQL.Append(" INSERT INTO SeqCampi ( ")
                StrSQL.Append(" PIVA, Sa_Cod, Campo_Cod, Base, [END], inviato, Data_Creazione, Data_Modifica, UserName_Creazione, UserName_Modifica ")
                StrSQL.Append(" )")
                StrSQL.Append(" VALUES( ")
                StrSQL.Append(" '" & Agro_SQL_SaveText(Piva) & "' ")
                StrSQL.Append(" ," & Agro_SQL_SaveNum(Sa_Cod) & " ")
                StrSQL.Append(" ," & Agro_SQL_SaveNum(NuovoValore) & " ")
                StrSQL.Append(" ," & Agro_SQL_SaveNum(Base) & " ")
                StrSQL.Append(" ," & Agro_SQL_SaveNum(Fine) & " ")
                StrSQL.Append(" , 0  ")
                StrSQL.Append(" , " & Agro_SQL_SaveDate(Date.Today) & "  ")
                StrSQL.Append(" , " & Agro_SQL_SaveDate(Date.Today) & "  ")
                StrSQL.Append(" ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append(" ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append(" )")

                '--------------------------------------------------------------------------
                xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
                '--------------------------------------------------------------------------

                If Not xRisp Then

                    'Imposto un valore dummy
                    NuovoValore = -1

                    'Genero un errore
                    Throw New Exception("Query di inserimento non riuscita.")

                End If

            End If

            DT = Nothing

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return NuovoValore


    End Function

    '############################################################################
    Public Function Seq_SaCod_Appezza_IdReg_Fittizi_per_AcquistiPDC(ByRef objParametri As AgronicaCoreParametri) As Integer


        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreDataProvider.Agro_Sequenze.Seq_SaCod_Appezza_IdReg_Fittizi_per_AcquistiPDC()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim xRisp As Boolean = False


        'Genero la query SQL

        StrSQL.Length = 0
        StrSQL.Append(" SELECT  * ")
        StrSQL.Append(" FROM    Sequenza_Tabelle ")
        StrSQL.Append(" WHERE   (Nome_Tabella = 'Seq_Acquisti_PDC')")

        Try
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Dim NuovoID As Integer

        If (DT.Rows.Count > 0) Then

            'Calcolo il nuovo valore
            NuovoID = CInt(DT.Rows(0).Item("Ultimo_Valore")) - 1

            'Se il nuovo valore supera l'ultimo valore valido ho un errore ...
            If NuovoID < CInt(DT.Rows(0).Item("End")) Then
                'Genero un errore
                Throw New Exception("L'indice ha raggiunto il limite superiore")
                'Restituisco il risultato
                Return -1
            Else

                'Genero la query SQL di modifica
                Try
                    StrSQL.Length = 0
                    StrSQL.Append(" UPDATE  Sequenza_Tabelle ")
                    StrSQL.Append(" SET ")
                    StrSQL.Append(" Ultimo_Valore = " & NuovoID & "  ")
                    StrSQL.Append(" WHERE   (Nome_Tabella = 'Seq_Acquisti_PDC')")

                    '--------------------------------------------------------------------------
                    xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
                    '--------------------------------------------------------------------------

                Catch ex As Exception

                    MessaggioErrore = ex.Message
                    Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
                    DT = Nothing
                    Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

                End Try

                'Restituisco il nuovo valore
                Return NuovoID

            End If

        Else
            'Creo il record
            StrSQL.Length = 0
            StrSQL.Append(" INSERT INTO Sequenza_Tabelle ")
            StrSQL.Append("     (Nome_Tabella, Ultimo_Valore, Base, [End],  ")
            StrSQL.Append("      Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica ) ")
            StrSQL.Append(" VALUES     ( ")
            StrSQL.Append(" 'Seq_Acquisti_PDC',  ")
            StrSQL.Append(" -1, -1, -2000000000,   ")
            StrSQL.Append(" " & Agro_SQL_SaveDate(Date.Today) & ",  ")
            StrSQL.Append(" " & Agro_SQL_SaveDate(Date.Today) & ",  ")
            StrSQL.Append(" '" & Agro_SQL_SaveText(objParametri.LogDescrizioneUtente) & "',  ")
            StrSQL.Append(" '" & Agro_SQL_SaveText(objParametri.LogDescrizioneUtente) & "'  ")
            StrSQL.Append(" )   ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            'Verifico la presenza di errori
            If xRisp = False Then

                Return -1

                'ERRORE
                Throw New Exception("errore nell inserimento di una nuova sequenza tabella")

            End If

            'Restituisco il risultato
            Return -1

        End If

    End Function

    '############################################################################

    'Lavez - 12/07/2024 - commentato per normalizzazione chiamate. (Approvata da Scatto\Vanni\Drudi\Lavez)
    '''' <summary>
    '''' Restituisce il prossimo id disponibile nella tabella indicata.
    '''' Il valore viene letto dalla tabella Sequenza_Tabelle e aggiornato
    '''' automaticamente in modo che quando verrà nuovamente chiamata questa
    '''' funzione il valore sarà incrementato di 1.
    '''' </summary>
    '''' <param name="NomeTabella">Tabella di cui si vuole leggere il prossimo id</param>
    '''' <param name="objParametri">DB su cui eseguire la query (solitamente server)</param>
    '''' <returns>Prossimo id disponibile nella tabella indicata</returns>
    '<Obsolete("Usare la funzione NuovoId_Tabella")>
    'Public Function Agronica_SequenzaTabelle_NuovoID(ByVal NomeTabella As String,
    '                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    '                                    ) _
    '                                    As Integer


    '    '----- Descrizione
    '    Dim NomeRoutine As String = "AgronicaCoreDataProvider.Agro_Sequenze.Agronica_SequenzaTabelle_NuovoID()"


    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim DT As DataTable

    '    Dim xRisp As Boolean = False


    '    'Genero la query SQL

    '    StrSQL.Length = 0
    '    StrSQL.Append(" SELECT  * ")
    '    StrSQL.Append(" FROM    Sequenza_Tabelle ")
    '    StrSQL.Append(" WHERE   (Nome_Tabella = '" & NomeTabella & "')")

    '    Try
    '        DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '        DT = Nothing
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Dim NuovoID As Integer

    '    If (DT.Rows.Count > 0) Then

    '        'Calcolo il nuovo valore
    '        NuovoID = CInt(DT.Rows(0).Item("Ultimo_Valore")) + 1

    '        'Se il nuovo valore supera l'ultimo valore valido ho un errore ...
    '        If NuovoID > CInt(DT.Rows(0).Item("End")) Then

    '            'Genero un errore
    '            Throw New Exception("L'indice ha raggiunto il limite superiore")
    '            'Restituisco il risultato
    '            Return -1
    '        Else

    '            'Genero la query SQL di modifica
    '            Try
    '                StrSQL.Length = 0
    '                StrSQL.Append(" UPDATE  Sequenza_Tabelle ")
    '                StrSQL.Append(" SET ")
    '                StrSQL.Append(" Ultimo_Valore = " & NuovoID & "  ")
    '                StrSQL.Append(" WHERE   (Nome_Tabella = '" & NomeTabella & "')")

    '                '--------------------------------------------------------------------------
    '                xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
    '                '--------------------------------------------------------------------------

    '            Catch ex As Exception

    '                MessaggioErrore = ex.Message
    '                Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '                DT = Nothing
    '                Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '            End Try

    '            'Restituisco il nuovo valore
    '            Return NuovoID

    '        End If

    '    Else
    '        'Creo il record
    '        StrSQL.Length = 0
    '        StrSQL.Append(" INSERT INTO Sequenza_Tabelle ")
    '        StrSQL.Append("     (Nome_Tabella, Ultimo_Valore, Base, [End],  ")
    '        StrSQL.Append("      Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica ) ")
    '        StrSQL.Append(" VALUES     ( ")
    '        StrSQL.Append(" '" & Agro_SQL_SaveText(LCase(NomeTabella)) & "',  ")
    '        StrSQL.Append(" 1, 0, 2000000000,   ")
    '        StrSQL.Append(" " & Agro_SQL_SaveDate(Date.Today) & ",  ")
    '        StrSQL.Append(" " & Agro_SQL_SaveDate(Date.Today) & ",  ")
    '        StrSQL.Append(" '" & Agro_SQL_SaveText(objParametri.LogDescrizioneUtente) & "',  ")
    '        StrSQL.Append(" '" & Agro_SQL_SaveText(objParametri.LogDescrizioneUtente) & "'  ")
    '        StrSQL.Append(" )   ")

    '        '--------------------------------------------------------------------------
    '        xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
    '        '--------------------------------------------------------------------------

    '        'Verifico la presenza di errori
    '        If xRisp = False Then

    '            Return -1

    '            'ERRORE
    '            Throw New Exception("errore nell inserimento di una nuova sequenza tabella")

    '        End If

    '        'Restituisco il risultato
    '        Return 1

    '    End If


    'End Function

    '############################################################################

    Public Function AppezzaCodNumSuccessivo_from_PivaSacod(
                                                        ByVal Piva As String,
                                                        ByVal Sa_Cod As Integer,
                                                        ByVal basecode As Integer,
                                                        ByRef Nr_Appezza As Integer,
                                                        ByRef objParametri As AgronicaCoreParametri
                                                            ) As String
        Const nomeRoutine = "AgronicaCoreDataProvider.Agro_Sequenze.AppezzaCodNumSuccessivo_from_PivaSacod()"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Dim xRisp As Boolean = False

        Dim Appezza As Integer

        Try

            StrSQL.Append(" ")
            StrSQL.Append(" SELECT MAX(Appezza) as Appezza ")
            StrSQL.Append(" FROM SeqAppezzamento ")
            StrSQL.Append(" WHERE (Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "') ")
            StrSQL.Append(" and sa_cod = " & Agro_SQL_SaveNum(Trim(Sa_Cod)))

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try



        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then

            'se l'operazione è stata fatta su un solo appezzamento restituisco il nome..
            If dt.Rows.Count = 1 Then

                If Not IsDBNull(dt.Rows(0).Item("Appezza")) Then
                    Appezza = CInt(dt.Rows(0).Item("Appezza"))
                    Nr_Appezza = Appezza - basecode + 1
                    Return Format(Nr_Appezza, "000")

                Else
                    Nr_Appezza = 1
                    Return "001"
                End If

            Else
                Nr_Appezza = 1
                Return "001"
            End If

        End If

    End Function

    '############################################################################
    '############################################################################
    '##########  Campi  ##################################################
    '############################################################################
    '############################################################################

    '############################################################################
    Public Function NuovoId_Grafica(ByVal Piva As String,
                                    ByVal Sa_cod As Long,
                                    ByVal Layer As String,
                                    ByVal Base As Long,
                                    ByVal Fine As Long,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As Long


        Dim NomeRoutine As String = "AgronicaCoreDataProvider.Agro_Sequenze.NuovoId_Grafica()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim xRisp As Boolean = False
        Dim NuovoValore As Int32

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Sa_cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            End If


            If Layer = "" Then
                Throw New Exception("Parametro non corretto nella query (Layer obbligatorio)")
            End If


            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" SELECT   * ")
            StrSQL.Append(" FROM     SeqGrafica ")
            StrSQL.Append(" WHERE    PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" AND      Sa_Cod = " & Agro_SQL_SaveNum(Sa_cod) & " ")
            StrSQL.Append(" AND      Layer = '" & Agro_SQL_SaveText(Layer) & "' ")
            '---------------------------------------------

            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)

            If DT.Rows.Count <> 0 Then

                NuovoValore = CInt(DT.Rows(0).Item("Codice")) + 1

                'Se il nuovo valore supera l'ultimo valore valido ho un errore ...
                If NuovoValore > CInt(DT.Rows(0).Item("End")) Then

                    'Imposto un valore dummy
                    NuovoValore = -1

                    'Genero un errore
                    Throw New Exception("L'indice ha raggiunto il limite superiore")

                Else

                    StrSQL.Length = 0
                    StrSQL.Append(" UPDATE SeqGrafica ")
                    StrSQL.Append(" SET Codice = " & Agro_SQL_SaveNum(NuovoValore) & " ")

                    StrSQL.Append(" WHERE    PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
                    StrSQL.Append(" AND      Sa_Cod = " & Agro_SQL_SaveNum(Sa_cod) & " ")
                    StrSQL.Append(" AND      Layer = '" & Agro_SQL_SaveText(Layer) & "' ")

                    'xRisp = EseguiQuery_Scrittura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)
                    '--------------------------------------------------------------------------
                    xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
                    '--------------------------------------------------------------------------

                    If Not xRisp Then

                        'Imposto un valore dummy
                        NuovoValore = -1

                        'Genero un errore
                        Throw New Exception("Query di aggiornamento non riuscita.")

                    End If

                End If
            Else

                NuovoValore = Base + 1

                StrSQL.Length = 0
                StrSQL.Append(" INSERT INTO SeqGrafica ( ")
                StrSQL.Append(" PIVA, Sa_Cod, Layer , Codice, Base, [END], inviato, Data_Creazione, Data_Modifica, UserName_Creazione, UserName_Modifica ")
                StrSQL.Append(" )")
                StrSQL.Append(" VALUES( ")
                StrSQL.Append(" '" & Agro_SQL_SaveText(Piva) & "' ")
                StrSQL.Append(" ," & Agro_SQL_SaveNum(Sa_cod) & " ")
                StrSQL.Append(" ," & Agro_SQL_SaveText(Layer) & " ")
                StrSQL.Append(" ," & Agro_SQL_SaveNum(NuovoValore) & " ")
                StrSQL.Append(" ," & Agro_SQL_SaveNum(Base) & " ")
                StrSQL.Append(" ," & Agro_SQL_SaveNum(Fine) & " ")
                StrSQL.Append(" , 0  ")
                StrSQL.Append(" , " & Agro_SQL_SaveDate(Date.Today) & "  ")
                StrSQL.Append(" , " & Agro_SQL_SaveDate(Date.Today) & "  ")
                StrSQL.Append(" ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append(" ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append(" )")

                '--------------------------------------------------------------------------
                xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
                '--------------------------------------------------------------------------

                If Not xRisp Then

                    'Imposto un valore dummy
                    NuovoValore = -1

                    'Genero un errore
                    Throw New Exception("Query di inserimento non riuscita.")

                End If

            End If

            DT = Nothing

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return NuovoValore

    End Function

    '############################################################################
    '############################################################################
    '##########  APPEZZAMENTO  ##################################################
    '############################################################################
    '############################################################################

    '############################################################################
    'Public Function NuovoId_Appezzamento(ByVal Piva As String,
    '                                     ByVal Sa_Cod As Integer,
    '                                     ByVal UserNameUtente As String,
    '                                     ByVal Base As Integer,
    '                                     ByVal Fine As Integer,
    '                                     ByRef objConnessione As DbConnection,
    '                                     ByRef objTransazione As DbTransaction,
    '                                     ByVal StringaConnessione As String,
    '                                     ByVal FlagVisibilita As Integer,
    '                                     ByVal DirectoryLOG As String,
    '                                     ByVal FileLOG As String,
    '                                     ByVal IdentificatoreUtente As String
    '                                     ) As Integer


    '    Const nomeRoutine = "AgronicaCoreDataProvider.Agro_Sequenze.NuovoId_Appezzamento()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim DT As DataTable

    '    Dim xRisp As Boolean = False
    '    Dim NuovoValore As Int32

    '    Try

    '        If Piva = "" Then
    '            Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
    '        End If

    '        If Sa_Cod = 0 Then
    '            Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
    '        End If


    '        '---------------------------------------------
    '        StrSQL.Length = 0
    '        StrSQL.Append(" SELECT   * ")
    '        StrSQL.Append(" FROM     SeqAppezzamento ")
    '        StrSQL.Append(" WHERE    PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
    '        StrSQL.Append(" AND      Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")

    '        '---------------------------------------------


    '        If Not IsNothing(objTransazione) Then
    '            DT = EseguiQuery_Lettura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, nomeRoutine)
    '        Else
    '            DT = EseguiQuery_Lettura(objConnessione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, nomeRoutine)
    '        End If


    '        If DT.Rows.Count <> 0 Then

    '            NuovoValore = DT.Rows(0).Item("Appezza") + 1

    '            'Se il nuovo valore supera l'ultimo valore valido ho un errore ...
    '            If NuovoValore > DT.Rows(0).Item("End") Then

    '                'Imposto un valore dummy
    '                NuovoValore = -1

    '                'Genero un errore
    '                Throw New Exception("L'indice ha raggiunto il limite superiore")

    '            Else

    '                StrSQL.Length = 0
    '                StrSQL.Append(" UPDATE SeqAppezzamento ")
    '                StrSQL.Append(" SET Appezza = " & Agro_SQL_SaveNum(NuovoValore) & " ")

    '                StrSQL.Append(" WHERE    PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
    '                StrSQL.Append(" AND      Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")

    '                'xRisp = EseguiQuery_Scrittura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)
    '                xRisp = EseguiQuery_Scrittura(objTransazione.Connection, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, nomeRoutine)

    '                If Not xRisp Then

    '                    'Imposto un valore dummy
    '                    NuovoValore = -1

    '                    'Genero un errore
    '                    Throw New Exception("Query di aggiornamento non riuscita.")

    '                End If

    '            End If
    '        Else

    '            NuovoValore = Base + 1

    '            StrSQL.Length = 0
    '            StrSQL.Append(" INSERT INTO SeqAppezzamento ( ")
    '            StrSQL.Append(" PIVA, Sa_Cod, Appezza, Base, [END], inviato, Data_Creazione, Data_Modifica, UserName_Creazione, UserName_Modifica ")
    '            StrSQL.Append(" )")
    '            StrSQL.Append(" VALUES( ")
    '            StrSQL.Append(" '" & Agro_SQL_SaveText(Piva) & "' ")
    '            StrSQL.Append(" ," & Agro_SQL_SaveNum(Sa_Cod) & " ")
    '            StrSQL.Append(" ," & Agro_SQL_SaveNum(NuovoValore) & " ")
    '            StrSQL.Append(" ," & Agro_SQL_SaveNum(Base) & " ")
    '            StrSQL.Append(" ," & Agro_SQL_SaveNum(Fine) & " ")
    '            StrSQL.Append(" , 0  ")
    '            StrSQL.Append(" , " & Agro_SQL_SaveDate(Date.Today) & "  ")
    '            StrSQL.Append(" , " & Agro_SQL_SaveDate(Date.Today) & "  ")
    '            StrSQL.Append(" ,'" & Agro_SQL_SaveText(UserNameUtente) & "' ")
    '            StrSQL.Append(" ,'" & Agro_SQL_SaveText(UserNameUtente) & "' ")
    '            StrSQL.Append(" )")

    '            xRisp = EseguiQuery_Scrittura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, nomeRoutine)

    '            If Not xRisp Then

    '                'Imposto un valore dummy
    '                NuovoValore = -1

    '                'Genero un errore
    '                Throw New Exception("Query di inserimento non riuscita.")

    '            End If

    '        End If

    '        DT = Nothing

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, nomeRoutine, MessaggioErrore)
    '        DT = Nothing
    '        Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return NuovoValore


    'End Function

    Public Function NuovoId_Appezzamento(ByVal Piva As String,
                                         ByVal Sa_Cod As Integer,
                                         ByVal Base As Integer,
                                         ByVal Fine As Integer,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As Integer

        Const nomeRoutine = "AgronicaCoreDataProvider.Agro_Sequenze.NuovoId_Appezzamento()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Dim xRisp As Boolean = False
        Dim NuovoValore As Int32

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Sa_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" SELECT   * ")
            StrSQL.Append(" FROM     SeqAppezzamento ")
            StrSQL.Append(" WHERE    PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" AND      Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")

            '---------------------------------------------

            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)

            If dt.Rows.Count <> 0 Then

                NuovoValore = CInt(dt.Rows(0).Item("Appezza")) + 1

                'Se il nuovo valore supera l'ultimo valore valido ho un errore ...
                If NuovoValore > CInt(dt.Rows(0).Item("End")) Then

                    'Imposto un valore dummy
                    NuovoValore = -1

                    'Genero un errore
                    Throw New Exception("L'indice ha raggiunto il limite superiore")

                Else

                    StrSQL.Length = 0
                    StrSQL.Append(" UPDATE SeqAppezzamento ")
                    StrSQL.Append(" SET Appezza = " & Agro_SQL_SaveNum(NuovoValore) & " ")

                    StrSQL.Append(" WHERE    PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
                    StrSQL.Append(" AND      Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")

                    'xRisp = EseguiQuery_Scrittura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)
                    '--------------------------------------------------------------------------
                    xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
                    '--------------------------------------------------------------------------

                    If Not xRisp Then

                        'Imposto un valore dummy
                        NuovoValore = -1

                        'Genero un errore
                        Throw New Exception("Query di aggiornamento non riuscita.")

                    End If

                End If
            Else

                NuovoValore = Base + 1

                StrSQL.Length = 0
                StrSQL.Append(" INSERT INTO SeqAppezzamento ( ")
                StrSQL.Append(" PIVA, Sa_Cod, Appezza, Base, [END], inviato, Data_Creazione, Data_Modifica, UserName_Creazione, UserName_Modifica ")
                StrSQL.Append(" )")
                StrSQL.Append(" VALUES( ")
                StrSQL.Append(" '" & Agro_SQL_SaveText(Piva) & "' ")
                StrSQL.Append(" ," & Agro_SQL_SaveNum(Sa_Cod) & " ")
                StrSQL.Append(" ," & Agro_SQL_SaveNum(NuovoValore) & " ")
                StrSQL.Append(" ," & Agro_SQL_SaveNum(Base) & " ")
                StrSQL.Append(" ," & Agro_SQL_SaveNum(Fine) & " ")
                StrSQL.Append(" , 0  ")
                StrSQL.Append(" , " & Agro_SQL_SaveDate(Date.Today) & "  ")
                StrSQL.Append(" , " & Agro_SQL_SaveDate(Date.Today) & "  ")
                StrSQL.Append(" ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append(" ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append(" )")

                '--------------------------------------------------------------------------
                xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
                '--------------------------------------------------------------------------

                If Not xRisp Then

                    'Imposto un valore dummy
                    NuovoValore = -1

                    'Genero un errore
                    Throw New Exception("Query di inserimento non riuscita.")

                End If

            End If

            dt = Nothing

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return NuovoValore

    End Function

    '############################################################################
    Public Function NuovoId_Tabella(ByVal NomeTabella As String,
                                    ByVal UserNameUtente As String,
                                    ByVal Base As Int32,
                                    ByVal Fine As Int32,
                                    ByRef objConnessione As DbConnection,
                                    ByRef objTransazione As DbTransaction,
                                    ByVal StringaConnessione As String,
                                    ByVal FlagVisibilita As Int32,
                                    ByVal DirectoryLOG As String,
                                    ByVal FileLOG As String,
                                    ByVal IdentificatoreUtente As String
                                         ) As Int32

        'Lavez - 15/07/2024 - aggiunto costruttore ObjParametri per poter leggere la versione di SQLServer e fare il check di versione che deve essere > 2008 per poter usare le sequence
        Dim objPHelper = New AgronicaCoreParametri_Helper
        Dim objParametri As AgronicaCoreParametri = objPHelper.Crea_ObjParametri(CostantiPersonalizzate.AGRODATAINIZIO,
                                                        CostantiPersonalizzate.AGRODATAFINE,
                                                        enumCancellazioneLogica.CancellazioneFisica,
                                                        enumVisibilita.Visibilita_Tutti,
                                                        DirectoryLOG, FileLOG, "", "", IdentificatoreUtente, "",
                                                        StringaConnessione)


        If gestioneSequencexTabella(NomeTabella, objParametri) Then
            Return Internal_NuovoId_Tabella_DaSequenza(NomeTabella, Base, Fine,
                                                       StringaConnessione,
                                                       DirectoryLOG,
                                                       FileLOG,
                                                       IdentificatoreUtente,
                                                       objParametri)
        End If
        Dim NomeRoutine As String = "AgronicaCoreDataProvider.Agro_Sequenze.NuovoId_Tabella()"
        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim xRisp As Boolean = False
        Dim NuovoValore As Int32

        Try


            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM   Sequenza_Tabelle ")
            StrSQL.Append(" WHERE  Nome_Tabella = '" & LCase(NomeTabella) & "' ")

            '---------------------------------------------
            'DT = EseguiQuery_Lettura(objConnessione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)
            DT = EseguiQuery_Lettura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

            If DT.Rows.Count <> 0 Then

                NuovoValore = CInt(DT.Rows(0).Item("Ultimo_Valore")) + 1

                'Se il nuovo valore supera l'ultimo valore valido ho un errore ...
                If NuovoValore > CInt(DT.Rows(0).Item("End")) Then

                    'Imposto un valore dummy
                    NuovoValore = -1

                    'Genero un errore
                    Throw New Exception("L'indice ha raggiunto il limite superiore")

                Else

                    StrSQL.Length = 0
                    StrSQL.Append(" UPDATE Sequenza_Tabelle ")
                    StrSQL.Append(" SET Ultimo_Valore = " & Agro_SQL_SaveNum(NuovoValore) & " ")
                    StrSQL.Append(" WHERE  Nome_Tabella = '" & LCase(NomeTabella) & "' ")


                    xRisp = EseguiQuery_Scrittura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

                    If Not xRisp Then

                        'Imposto un valore dummy
                        NuovoValore = -1

                        'Genero un errore
                        Throw New Exception("Query di aggiornamento non riuscita.")

                    End If

                End If

            Else 'Non esiste il record nella tabella

                If NomeTabella.ToLower <> "contatti" Then
                    NuovoValore = Base + 1
                Else
                    Base = -2000000000
                    Fine = 0
                    NuovoValore = -2000000000
                End If

                StrSQL.Length = 0
                StrSQL.Append(" INSERT INTO Sequenza_Tabelle ( ")
                StrSQL.Append(" Nome_Tabella, Ultimo_Valore, UserName_Creazione, UserName_Modifica, Base, [End] ")
                StrSQL.Append(" )")
                StrSQL.Append(" VALUES( ")
                StrSQL.Append(" '" & Agro_SQL_SaveText(LCase(NomeTabella)) & "' ")
                StrSQL.Append(" ," & Agro_SQL_SaveNum(NuovoValore) & " ")
                StrSQL.Append(" ,'" & Agro_SQL_SaveText(UserNameUtente) & "' ")
                StrSQL.Append(" ,'" & Agro_SQL_SaveText(UserNameUtente) & "' ")
                StrSQL.Append(" ," & Agro_SQL_SaveNum(Base) & " ")
                StrSQL.Append(" ," & Agro_SQL_SaveNum(Fine) & " ")
                StrSQL.Append(" )")

                xRisp = EseguiQuery_Scrittura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

                If Not xRisp Then

                    'Imposto un valore dummy
                    NuovoValore = -1

                    'Genero un errore
                    Throw New Exception("Query di inserimento non riuscita.")

                End If

            End If

            DT = Nothing

        Catch ex As Exception

            MessaggioErrore = ex.Message + " " + "[NomeTabella = " + NomeTabella + "]"

            If ex.InnerException IsNot Nothing Then
                MessaggioErrore &= " - Inner exception:" & ex.InnerException.Message
            End If
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return NuovoValore


    End Function

    ''' <summary>
    ''' Allinea il contatore in sequenza tabella ponendolo uguale a valore massimo letto dalla colonna per la tabella passata come parametro
    ''' </summary>
    ''' <param name="NomeTabella">esempio: Agenda</param>
    ''' <param name="NomeCampoChiave">esempio: id_agenda</param>
    ''' <param name="objParametri"></param>
    ''' <param name="UtilizzaTransazione"></param>
    ''' <returns></returns>
    Public Function AllineaUltimoValoreDataTabella(
        ByVal NomeTabella As String,
        ByVal NomeCampoChiave As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
        Optional ByVal UtilizzaTransazione As Boolean = True
     ) As Integer

        Const nomeRoutine = "AgronicaCoreDataProvider.Agro_Sequenze.AllineaUltimoValoreDataTabella()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Dim xRisp As Boolean = False
        Dim NuovoValore As Int32 = 0

        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False

        '''''''DRUDI 21/11/2018
        ' Creo la connessione e la transazione manualmente
        ' In modo da non fare il rollback in caso di errore
        Dim connection = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
        connection.Open()

        Dim transaction As DbTransaction = Nothing
        If UtilizzaTransazione = True Then
            transaction = connection.BeginTransaction()
        End If


        Try

            'Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" SELECT ISNULL(MAX(" & NomeCampoChiave & "), 0) ")
            StrSQL.Append(" FROM    " & NomeTabella)
            StrSQL.Append(" WITH(XLOCK) ") 'vanni Blocco operazioni in insert per la tabella


            '--------------------------------------------------------------------------
            'DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            Dim DTUltimoValore As DataTable =
                    EseguiQuery_Lettura(connection, objParametri.StringaConnessione, StrSQL.ToString, objParametri.LogDirectory, objParametri.LogFileName, objParametri.UsernameOperazione, nomeRoutine, transaction)
            '--------------------------------------------------------------------------


            NuovoValore = CInt(DTUltimoValore.Rows(0)(0))

            If NuovoValore > 0 Then


                '---------------------------------------------
                StrSQL.Length = 0
                StrSQL.Append(" SELECT * ")
                StrSQL.Append(" FROM   Sequenza_Tabelle ")
                StrSQL.Append(" WITH(XLOCK, ROWLOCK) ") 'DRUDI 21/11/2018 Blocco solo la riga e non tutta la tabella
                StrSQL.Append(" WHERE  Nome_Tabella = '" & LCase(NomeTabella) & "' ")

                '--------------------------------------------------------------------------
                'DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
                dt = EseguiQuery_Lettura(connection, objParametri.StringaConnessione, StrSQL.ToString, objParametri.LogDirectory, objParametri.LogFileName, objParametri.UsernameOperazione, nomeRoutine, transaction)
                '--------------------------------------------------------------------------


                If dt.Rows.Count <> 0 Then



                    'Se il nuovo valore supera l'ultimo valore valido ho un errore ...
                    If NuovoValore > CInt(dt.Rows(0).Item("End")) Then

                        'Imposto un valore dummy
                        NuovoValore = -1

                        'Genero un errore
                        Throw New Exception("L'indice ha raggiunto il limite superiore")

                    Else

                        StrSQL.Length = 0
                        StrSQL.Append(" UPDATE Sequenza_Tabelle ")
                        StrSQL.Append(" SET Ultimo_Valore = " & Agro_SQL_SaveNum(NuovoValore) & " ")
                        StrSQL.Append(" WHERE  Nome_Tabella = '" & LCase(NomeTabella) & "' ")


                        '--------------------------------------------------------------------------
                        'xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
                        xRisp = EseguiQuery_Scrittura(connection, transaction, objParametri.StringaConnessione, StrSQL.ToString, objParametri.LogDirectory, objParametri.LogFileName, objParametri.UsernameOperazione, nomeRoutine)
                        '--------------------------------------------------------------------------

                        If Not xRisp Then

                            'Imposto un valore dummy
                            NuovoValore = -1

                            'Genero un errore
                            Throw New Exception("Query di aggiornamento non riuscita.")

                        End If

                    End If

                Else 'Non esiste il record in Sequenza_Tabelle

                    Dim base As Integer
                    Dim fine As Integer = 2000000000

                    Select Case NomeTabella.ToLower
                        Case "contatti"
                            base = -2000000000
                            fine = 0
                            NuovoValore = -2000000000
                        Case "impresa"
                            base = -2000000000
                            fine = 0
                    End Select


                    StrSQL.Length = 0
                    StrSQL.Append(" INSERT INTO Sequenza_Tabelle ( ")
                    StrSQL.Append(" Nome_Tabella, Ultimo_Valore, UserName_Creazione, UserName_Modifica, Base, [End] ")
                    StrSQL.Append(" )")
                    StrSQL.Append(" VALUES( ")
                    StrSQL.Append(" '" & Agro_SQL_SaveText(LCase(NomeTabella)) & "' ")
                    StrSQL.Append(" ," & Agro_SQL_SaveNum(NuovoValore) & " ")
                    StrSQL.Append(" ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                    StrSQL.Append(" ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                    StrSQL.Append(" ," & Agro_SQL_SaveNum(base) & " ")
                    StrSQL.Append(" ," & Agro_SQL_SaveNum(fine) & " ")
                    StrSQL.Append(" )")

                    '--------------------------------------------------------------------------
                    'xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
                    xRisp = EseguiQuery_Scrittura(connection, transaction, objParametri.StringaConnessione, StrSQL.ToString, objParametri.LogDirectory, objParametri.LogFileName, objParametri.UsernameOperazione, nomeRoutine)
                    '--------------------------------------------------------------------------

                    If Not xRisp Then

                        'Imposto un valore dummy
                        NuovoValore = -1

                        'Genero un errore
                        Throw New Exception("Query di inserimento non riuscita.")

                    End If

                End If

                'Utility.VerificaChiudiTransazione(objParametri, flagTransazione)
                If UtilizzaTransazione = True Then
                    transaction.Commit()
                End If

            End If
            'Esiste un valore da aggiornare
            dt = Nothing

        Catch ex As Exception

            'Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)

            If UtilizzaTransazione = True Then
                transaction.Rollback()
            End If

            messaggioErrore = ex.Message + " " + "[NomeTabella = " + NomeTabella + "]"

            If ex.InnerException IsNot Nothing Then
                messaggioErrore &= " - Inner exception:" & ex.InnerException.Message
            End If

            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)


        Finally

            'Utility.VerificaChiudiConnessione(objParametri, flagConnessione)
            connection.Close()
            connection.Dispose()

        End Try

        Return NuovoValore


    End Function

    Private Function Internal_NuovoId_Tabella(ByVal NomeTabella As String,
                                         ByVal Base As Int32,
                                         ByVal Fine As Int32,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                         Optional ByVal UtilizzaTransazione As Boolean = False
                                         ) As Int32

        If gestioneSequencexTabella(NomeTabella, objParametri) Then
            Return Internal_NuovoId_Tabella_DaSequenza(NomeTabella, Base, Fine,
                                                       objParametri.StringaConnessione,
                                                       objParametri.LogDirectory,
                                                       objParametri.LogFileName,
                                                       objParametri.UsernameOperazione,
                                                       objParametri)
        End If

        Dim NomeRoutine As String = "AgronicaCoreDataProvider.Agro_Sequenze.Internal_NuovoId_Tabella()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim NuovoValore As Int32

        Dim xRisp As Boolean


        Dim connection = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
        connection.Open()
        Dim isolationLevel As System.Data.IsolationLevel = System.Data.IsolationLevel.ReadUncommitted
        Dim transaction As DbTransaction = Nothing
        If UtilizzaTransazione = True Then
            transaction = connection.BeginTransaction(isolationLevel)
        End If

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")

            StrSQL.AppendLine(" UPDATE ")
            StrSQL.AppendLine(" Sequenza_Tabelle ")
            StrSQL.AppendLine(" SET Ultimo_Valore = (SELECT Ultimo_Valore + 1 FROM Sequenza_Tabelle (NOLOCK) WHERE Nome_Tabella = '" & LCase(NomeTabella) & "') ")
            StrSQL.AppendLine("     , Data_Modifica = GETDATE() ")
            StrSQL.AppendLine("     , Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine(" OUTPUT inserted.Ultimo_Valore ")
            StrSQL.AppendLine(" where Nome_Tabella = '" & LCase(NomeTabella) & "' ")

            '--------------------------------------------------------------------------
            'xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            DT = EseguiQuery_Lettura(connection, transaction, objParametri.StringaConnessione, StrSQL.ToString, objParametri.LogDirectory, objParametri.LogFileName, objParametri.UsernameOperazione, NomeRoutine)
            '--------------------------------------------------------------------------

            If DT IsNot Nothing AndAlso DT.Rows.Count > 0 Then
                NuovoValore = DT.Rows(0)(0)
            Else
                NuovoValore = 1

                Select Case NomeTabella.ToLower
                    Case "contatti"
                        Base = -2000000000
                        Fine = 0
                        NuovoValore = -2000000000
                    Case "impresa"
                        Base = -2000000000
                        Fine = 0
                        NuovoValore = -2000000000
                    Case Else
                        NuovoValore = Base + 1
                End Select


                StrSQL.Length = 0
                StrSQL.Append(" INSERT INTO Sequenza_Tabelle ( ")
                StrSQL.Append(" Nome_Tabella, Ultimo_Valore, UserName_Creazione, UserName_Modifica, Base, [End] ")
                StrSQL.Append(" )")
                StrSQL.Append(" VALUES( ")
                StrSQL.Append(" '" & Agro_SQL_SaveText(LCase(NomeTabella)) & "' ")
                StrSQL.Append(" ," & Agro_SQL_SaveNum(NuovoValore) & " ")
                StrSQL.Append(" ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append(" ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append(" ," & Agro_SQL_SaveNum(Base) & " ")
                StrSQL.Append(" ," & Agro_SQL_SaveNum(Fine) & " ")
                StrSQL.Append(" )")

                '--------------------------------------------------------------------------
                'xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
                xRisp = EseguiQuery_Scrittura(connection, transaction, objParametri.StringaConnessione, StrSQL.ToString, objParametri.LogDirectory, objParametri.LogFileName, objParametri.UsernameOperazione, NomeRoutine)
                '--------------------------------------------------------------------------

                If Not xRisp Then

                    'Imposto un valore dummy
                    NuovoValore = -1

                    'Genero un errore
                    Throw New Exception("Query di inserimento non riuscita.")

                End If

                'Utility.VerificaChiudiTransazione(objParametri, flagTransazione)
                If UtilizzaTransazione = True Then
                    transaction.Commit()
                End If

                DT = Nothing

            End If

        Catch ex As Exception

            'Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)

            If UtilizzaTransazione = True Then
                transaction.Rollback()
            End If

            MessaggioErrore = ex.Message + " " + "[NomeTabella = " + NomeTabella + "]"

            If ex.InnerException IsNot Nothing Then
                MessaggioErrore &= " - Inner exception:" & ex.InnerException.Message
            End If

            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)


        Finally
            Threading.Thread.Sleep(200)
            'Utility.VerificaChiudiConnessione(objParametri, flagConnessione)
            If transaction IsNot Nothing Then
                transaction.Dispose()
            End If

            If connection IsNot Nothing Then
                connection.Close()
                connection.Dispose()
            End If

        End Try
        Return NuovoValore


    End Function

    Public Shared Function CheckAllowAppSettingsFlagUseSequence() As Boolean
        Dim ret = True
        Dim chk = ConfigurationManager.AppSettings("Allow_Sql_Sequence")
        Try
            If chk IsNot Nothing AndAlso chk <> "" Then
                ret = Boolean.Parse(chk)
            End If
        Catch ex As Exception
            ret = True
        End Try
        Return ret
    End Function

    Private Function gestioneSequencexTabella(ByVal Nome_Tabella As String,
                                              ByRef ObjParametri As AgronicaCoreParametri) As Boolean

        'check appSettings se è specificato false
        If Not CheckAllowAppSettingsFlagUseSequence() Then
            Return False
        End If

        'non è più necessario controllare la versione di sql, commanda il valore nel file appsettings
        ''sql Versione 2008 non gestisce le sequence
        'Dim major As Integer = VersioneSqlServer_Major(ObjParametri)
        'If (major <= 10) Then 'major 10 corrisponde a sql server 2008 https://learn.microsoft.com/en-us/troubleshoot/sql/releases/download-and-install-latest-updates
        '    Return False
        'End If

        'Tabelle speciali che non devono essere MAI gestite con sequence
        If SequenceTableExceptions.Select(Function(x) x.ToLower).ToList.Contains(Nome_Tabella.ToLower) Then
            Return False
        End If

        'Caso speciale di sequence utilizzata per progressivo annuale, pdc_campioni_2024, pdc_campioni_2025 etc etc
        If Nome_Tabella.ToLower().Contains("pdc_campioni_") Then
            Return False
        End If

        'Default gestione con sequence
        Return True
    End Function

    Private Function Internal_NuovoId_Tabella_DaSequenza(ByVal NomeTabella As String,
                                                         ByVal Base As Int32,
                                                         ByVal Fine As Int32,
                                                         ByVal StringaConnessione As String,
                                                         ByVal LogDirectory As String,
                                                         ByVal LogFileName As String,
                                                         ByVal UsernameOperazione As String,
                                                         objParametri As AgronicaCoreParametri) As Int32
        Dim nomeRoutine = "AgronicaCoreDataProvider.Agro_Sequenze.Internal_NuovoId_Tabella_DaSequenza()"

        Dim nuovoValore = 1
        Dim connection = DataProviderFactory.Instance.CreaNuovaConnessione(StringaConnessione)
        connection.Open()

        Try
            If Not esisteSequenza(NomeTabella, connection, StringaConnessione, LogDirectory, LogFileName, UsernameOperazione, objParametri) Then
                Dim ultimoValore = UltimoSequenzaTabelle(NomeTabella, connection, StringaConnessione, LogDirectory, LogFileName, UsernameOperazione, objParametri)
                'Lavez - 15/01/2025 - Fix per gestire i contatori che partono con un valore maggiore di 1 (vedi i layer personalizzati del GIS)
                If ultimoValore = 0 AndAlso Base > 1 Then
                    ultimoValore = Base
                End If
                creaSequenza(NomeTabella, ultimoValore, connection, StringaConnessione, LogDirectory, LogFileName, UsernameOperazione, objParametri)
            End If

            nuovoValore = nuovoValoreSequenza(NomeTabella, connection, StringaConnessione, LogDirectory, LogFileName, UsernameOperazione, objParametri)
        Catch ex As Exception

        Finally
            If connection IsNot Nothing Then
                connection.Close()
                connection.Dispose()
            End If
        End Try

        Return nuovoValore

    End Function

    Public Shared Function InizializzaSequence(ByVal NomeTabella As String,
                                         ByVal Base As Int32,
                                         ByVal Fine As Int32,
                                         ByVal ObjParametri As AgronicaCoreParametri) As Boolean

        Dim ret As Boolean = False
        Dim connection = DataProviderFactory.Instance.CreaNuovaConnessione(ObjParametri.StringaConnessione)
        connection.Open()

        Dim dymmy As New Agro_Sequenze

        Try
            If Not dymmy.esisteSequenza(NomeTabella, connection, ObjParametri.StringaConnessione, ObjParametri.LogDirectory, ObjParametri.LogFileName, ObjParametri.UsernameOperazione, ObjParametri) Then
                Dim ultimoValore = dymmy.UltimoSequenzaTabelle(NomeTabella, connection, ObjParametri.StringaConnessione, ObjParametri.LogDirectory, ObjParametri.LogFileName, ObjParametri.UsernameOperazione, ObjParametri)
                ret = dymmy.creaSequenza(NomeTabella, ultimoValore, connection, ObjParametri.StringaConnessione, ObjParametri.LogDirectory, ObjParametri.LogFileName, ObjParametri.UsernameOperazione, ObjParametri)
            Else
                'la sequence esiste già per cui l'inizializzazione non serve
                ret = True
            End If

        Catch ex As Exception

        Finally
            If connection IsNot Nothing Then
                connection.Close()
                connection.Dispose()
            End If
        End Try
        Return ret
    End Function

    Private Function esisteSequenza(ByVal NomeTabella As String,
                                    ByRef connection As DbConnection,
                                    ByVal StringaConnessione As String,
                                    ByVal LogDirectory As String,
                                    ByVal LogFileName As String,
                                    ByVal UsernameOperazione As String,
                                    objParametri As AgronicaCoreParametri) As Boolean
        Const nomeRoutine = "AgronicaCoreDataProvider.Agro_Sequenze.esisteSequenza()"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable
        Dim esiste As Boolean = False

        Try
            Dim nomeSequenza = "Sequence_" & NomeTabella
            StrSQL.Append(" SELECT   * ")
            StrSQL.Append(" FROM     sys.sequences ")
            StrSQL.Append(" WHERE    [name] = '" & Agro_SQL_SaveText(nomeSequenza) & "' ")

            '-------------------------------------------------------------------------- 
            dt = EseguiQuery_Lettura(connection, Nothing, StringaConnessione, StrSQL.ToString, LogDirectory, LogFileName, UsernameOperazione, nomeRoutine)
            '--------------------------------------------------------------------------

            If dt.Rows.Count > 0 Then
                esiste = True
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message + " " + "[NomeTabella = " + NomeTabella + "]"

            If ex.InnerException IsNot Nothing Then
                messaggioErrore &= " - Inner exception:" & ex.InnerException.Message
            End If

            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            esiste = False
            Return esiste
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return esiste
    End Function

    Private Function creaSequenza(ByVal NomeTabella As String,
                                  ByVal startValue As Integer,
                                  ByRef connection As DbConnection,
                                  ByVal StringaConnessione As String,
                                  ByVal LogDirectory As String,
                                  ByVal LogFileName As String,
                                  ByVal UsernameOperazione As String,
                                  objParametri As AgronicaCoreParametri) As Boolean
        Const nomeRoutine = "AgronicaCoreDataProvider.Agro_Sequenze.creaSequenza()"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim risp As Boolean = False
        Try

            If startValue = 0 Then
                Select Case NomeTabella.ToLower
                    Case "contatti"
                        startValue = -2000000000
                    Case "impresa"
                        startValue = -2000000000
                End Select
                'Else
                '    startValue = startValue + 1
            End If

            startValue = startValue + 1

            Dim nomeSequenza As String = "Sequence_" & NomeTabella
            StrSQL.Append(" CREATE SEQUENCE " & nomeSequenza & " ")
            StrSQL.Append(" START WITH " & startValue.ToString() & " ")
            StrSQL.Append(" INCREMENT BY 1 ")

            '-------------------------------------------------------------------------- 
            risp = EseguiQuery_Scrittura(connection, Nothing, StringaConnessione, StrSQL.ToString, LogDirectory, LogFileName, UsernameOperazione, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message + " " + "[NomeTabella = " + NomeTabella + "]"

            If ex.InnerException IsNot Nothing Then
                messaggioErrore &= " - Inner exception:" & ex.InnerException.Message
            End If

            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Return False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return risp
    End Function

    Private Function nuovoValoreSequenza(ByVal NomeTabella As String,
                                         ByRef connection As DbConnection,
                                         ByVal StringaConnessione As String,
                                         ByVal LogDirectory As String,
                                         ByVal LogFileName As String,
                                         ByVal UsernameOperazione As String,
                                         objParametri As AgronicaCoreParametri) As Integer
        Const nomeRoutine = "AgronicaCoreDataProvider.Agro_Sequenze.nuovoValoreSequenza()"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Dim ultimoValore As Integer = 0

        Try
            Dim nomeSequenza As String = "Sequence_" & NomeTabella

            StrSQL.Append(" SELECT NEXT VALUE FOR " & nomeSequenza & " ")

            '-------------------------------------------------------------------------- 
            dt = EseguiQuery_Lettura(connection, Nothing, StringaConnessione, StrSQL.ToString, LogDirectory, LogFileName, UsernameOperazione, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message + " " + "[NomeTabella = " + NomeTabella + "]"

            If ex.InnerException IsNot Nothing Then
                messaggioErrore &= " - Inner exception:" & ex.InnerException.Message
            End If

            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Return ultimoValore
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If (Not IsNothing(dt)) AndAlso
            (dt.Rows.Count > 0) Then
            ultimoValore = CInt(dt.Rows(0).Item(0))
        End If

        Return ultimoValore
    End Function

    Public Function NuovoId_Tabella(ByVal NomeTabella As String,
                                         ByVal Base As Int32,
                                         ByVal Fine As Int32,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                         Optional ByVal UtilizzaTransazione As Boolean = True
                                         ) As Int32


        Dim NomeRoutine As String = "AgronicaCoreDataProvider.Agro_Sequenze.NuovoId_Tabella()"
        Return Internal_NuovoId_Tabella(NomeTabella, Base, Fine, objParametri, False)

        #Region "unreachable code :("
        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        'Dim MessaggioErrore As String = ""
        'Dim StrSQL As New System.Text.StringBuilder
        'Dim DT As DataTable
        'Dim xRisp As Boolean = False
        'Dim NuovoValore As Int32

        'Dim flagConnessione As Boolean = False
        'Dim flagTransazione As Boolean = False
        ''Dim threadString = "Thread " & System.Environment.CurrentManagedThreadId.ToString() & " "
        ''Scrivi_LOG(objParametri, NomeRoutine, threadString & "Inizio Lock " & NomeTabella & ": " & DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))

        ''''''''DRUDI 21/11/2018
        '' Creo la connessione e la transazione manualmente
        '' In modo da non fare il rollback in caso di errore
        'Dim connection = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
        'connection.Open()
        'Dim isolationLevel As System.Data.IsolationLevel = System.Data.IsolationLevel.ReadUncommitted
        'Dim transaction As DbTransaction = Nothing
        'If UtilizzaTransazione = True Then
        '    transaction = connection.BeginTransaction(isolationLevel)
        'End If

        'Try
        '    'Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)
        '    '---------------------------------------------
        '    StrSQL.Length = 0
        '    StrSQL.Append(" SELECT * ")
        '    StrSQL.Append(" FROM   Sequenza_Tabelle ")
        '    StrSQL.Append(" WITH(XLOCK, ROWLOCK) ") 'DRUDI 21/11/2018 Blocco solo la riga e non tutta la tabella
        '    StrSQL.Append(" WHERE  Nome_Tabella = '" & LCase(NomeTabella) & "' ")

        '    Dim i = 0
        '    For i = 0 To 4
        '        Dim err = False
        '        Try
        '            MessaggioErrore = ""
        '            'start = DateTime.Now
        '            '--------------------------------------------------------------------------
        '            'DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
        '            DT = EseguiQuery_Lettura(connection, objParametri.StringaConnessione, StrSQL.ToString, objParametri.LogDirectory, objParametri.LogFileName, objParametri.UsernameOperazione, NomeRoutine, transaction)
        '            'Scrivi_LOG(objParametri, NomeRoutine, threadString & " " & NomeTabella & ": " & "Err: " & IIf(err, "True", "False") & " T: (ms) " & STime2.TotalMilliseconds.ToString)
        '            Exit For
        '        Catch ex As Exception
        '            MessaggioErrore = "Errore NuovoId_Tabella i=" & i.ToString() & " " & vbCrLf
        '            MessaggioErrore &= AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        '            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
        '            err = True
        '        End Try
        '        'Scrivi_LOG(objParametri, NomeRoutine, threadString & " " & NomeTabella & ": " & "Err: " & IIf(err, "True", "False") & " T: (ms) " & STime.TotalMilliseconds.ToString)
        '        Threading.Thread.Sleep(200)
        '        i = i + 1
        '    Next
        '    MessaggioErrore = ""

        '    If DT.Rows.Count <> 0 AndAlso DT IsNot Nothing Then
        '        NuovoValore = CInt(DT.Rows(0).Item("Ultimo_Valore")) + 1
        '        'Se il nuovo valore supera l'ultimo valore valido ho un errore ...
        '        If NuovoValore > CInt(DT.Rows(0).Item("End")) Then
        '            'Imposto un valore dummy
        '            NuovoValore = -1
        '            'Genero un errore
        '            Throw New Exception("L'indice ha raggiunto il limite superiore")
        '        Else
        '            StrSQL.Length = 0
        '            StrSQL.Append(" UPDATE Sequenza_Tabelle ")
        '            StrSQL.Append(" SET Ultimo_Valore = " & Agro_SQL_SaveNum(NuovoValore) & " ")
        '            StrSQL.Append(" WHERE  Nome_Tabella = '" & LCase(NomeTabella) & "' ")
        '            '--------------------------------------------------------------------------
        '            'xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
        '            xRisp = EseguiQuery_Scrittura(connection, transaction, objParametri.StringaConnessione, StrSQL.ToString, objParametri.LogDirectory, objParametri.LogFileName, objParametri.UsernameOperazione, NomeRoutine)
        '            '--------------------------------------------------------------------------
        '            If Not xRisp Then
        '                'Imposto un valore dummy
        '                NuovoValore = -1
        '                'Genero un errore
        '                Throw New Exception("Query di aggiornamento non riuscita.")
        '            End If
        '        End If
        '    Else 'Non esiste il record in Sequenza_Tabelle
        '        Select Case NomeTabella.ToLower
        '            Case "contatti"
        '                Base = -2000000000
        '                Fine = 0
        '                NuovoValore = -2000000000
        '            Case "impresa"
        '                Base = -2000000000
        '                Fine = 0
        '                NuovoValore = -2000000000
        '            Case Else
        '                NuovoValore = Base + 1
        '        End Select
        '        StrSQL.Length = 0
        '        StrSQL.Append(" INSERT INTO Sequenza_Tabelle ( ")
        '        StrSQL.Append(" Nome_Tabella, Ultimo_Valore, UserName_Creazione, UserName_Modifica, Base, [End] ")
        '        StrSQL.Append(" )")
        '        StrSQL.Append(" VALUES( ")
        '        StrSQL.Append(" '" & Agro_SQL_SaveText(LCase(NomeTabella)) & "' ")
        '        StrSQL.Append(" ," & Agro_SQL_SaveNum(NuovoValore) & " ")
        '        StrSQL.Append(" ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
        '        StrSQL.Append(" ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
        '        StrSQL.Append(" ," & Agro_SQL_SaveNum(Base) & " ")
        '        StrSQL.Append(" ," & Agro_SQL_SaveNum(Fine) & " ")
        '        StrSQL.Append(" )")

        '        '--------------------------------------------------------------------------
        '        'xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
        '        xRisp = EseguiQuery_Scrittura(connection, transaction, objParametri.StringaConnessione, StrSQL.ToString, objParametri.LogDirectory, objParametri.LogFileName, objParametri.UsernameOperazione, NomeRoutine)
        '        '--------------------------------------------------------------------------

        '        If Not xRisp Then
        '            'Imposto un valore dummy
        '            NuovoValore = -1
        '            'Genero un errore
        '            Throw New Exception("Query di inserimento non riuscita.")
        '        End If
        '    End If

        '    'Utility.VerificaChiudiTransazione(objParametri, flagTransazione)
        '    If UtilizzaTransazione = True Then
        '        transaction.Commit()
        '    End If
        '    DT = Nothing

        'Catch ex As Exception

        '    'Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)
        '    If UtilizzaTransazione = True Then
        '        transaction.Rollback()
        '    End If
        '    MessaggioErrore = ex.Message + " " + "[NomeTabella = " + NomeTabella + "]"
        '    If ex.InnerException IsNot Nothing Then
        '        MessaggioErrore &= " - Inner exception:" & ex.InnerException.Message
        '    End If
        '    Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
        '    DT = Nothing
        '    Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        'Finally
        '    Threading.Thread.Sleep(200)
        '    'Utility.VerificaChiudiConnessione(objParametri, flagConnessione)
        '    transaction.Dispose()
        '    connection.Close()
        '    connection.Dispose()
        'End Try
        'Return NuovoValore
        #End Region
    End Function

    Public Sub Apertura_Sequenza_Tabella(ByVal NomeTabella As String,
                                         ByVal Base As Int32,
                                         ByVal Fine As Int32,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim NomeRoutine As String = "AgronicaCoreDataProvider.Agro_Sequenze.Apertura_Sequenza_Tabella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim xRisp As Boolean = False
        Dim NuovoValore As Int32

        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False

        Dim connection = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
        connection.Open()

        Dim transaction As DbTransaction = Nothing

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM   Sequenza_Tabelle ")
            StrSQL.Append(" WITH(XLOCK, ROWLOCK) ") 'DRUDI 21/11/2018 Blocco solo la riga e non tutta la tabella
            StrSQL.Append(" WHERE  Nome_Tabella = '" & LCase(NomeTabella) & "' ")

            DT = EseguiQuery_Lettura(connection,
                                     objParametri.StringaConnessione,
                                     StrSQL.ToString,
                                     objParametri.LogDirectory,
                                     objParametri.LogFileName,
                                     objParametri.UsernameOperazione,
                                     NomeRoutine,
                                     transaction)

            If DT.Rows.Count = 0 Then

                Select Case NomeTabella.ToLower
                    Case "contatti"
                        Base = -2000000000
                        Fine = 0
                        NuovoValore = -2000000000
                    Case "impresa"
                        Base = -2000000000
                        Fine = 0
                        NuovoValore = -2000000000
                    Case Else
                        NuovoValore = 0
                End Select

                StrSQL.Length = 0
                StrSQL.Append(" INSERT INTO Sequenza_Tabelle ( ")
                StrSQL.Append(" Nome_Tabella, Ultimo_Valore, UserName_Creazione, UserName_Modifica, Base, [End] ")
                StrSQL.Append(" )")
                StrSQL.Append(" VALUES( ")
                StrSQL.Append(" '" & Agro_SQL_SaveText(LCase(NomeTabella)) & "' ")
                StrSQL.Append(" ," & Agro_SQL_SaveNum(NuovoValore) & " ")
                StrSQL.Append(" ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append(" ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append(" ," & Agro_SQL_SaveNum(Base) & " ")
                StrSQL.Append(" ," & Agro_SQL_SaveNum(Fine) & " ")
                StrSQL.Append(" )")

                xRisp = EseguiQuery_Scrittura(connection,
                                              transaction,
                                              objParametri.StringaConnessione,
                                              StrSQL.ToString,
                                              objParametri.LogDirectory,
                                              objParametri.LogFileName,
                                              objParametri.UsernameOperazione,
                                              NomeRoutine)

                If Not xRisp Then

                    'Imposto un valore dummy
                    NuovoValore = -1

                    'Genero un errore
                    Throw New Exception("Query di inserimento non riuscita.")

                End If

            End If

            DT = Nothing

        Catch ex As Exception

            MessaggioErrore = ex.Message + " " + "[NomeTabella = " + NomeTabella + "]"

            If ex.InnerException IsNot Nothing Then
                MessaggioErrore &= " - Inner exception:" & ex.InnerException.Message
            End If

            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        Finally

            connection.Close()
            connection.Dispose()

        End Try

    End Sub

    Public Function NuovoId_Tabella_EF(ByRef EFContext As Gias_DeveloperServer_Entities,
                                       ByVal NomeTabella As String,
                                       ByVal Base As Int32,
                                       ByVal Fine As Int32,
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Int32

        If gestioneSequencexTabella(NomeTabella, objParametri) Then
            Return Internal_NuovoId_Tabella_DaSequenza(NomeTabella, Base, Fine,
                                                       objParametri.StringaConnessione,
                                                       objParametri.LogDirectory,
                                                       objParametri.LogFileName,
                                                       objParametri.UsernameOperazione,
                                                       objParametri)
        End If
        Dim saveChanges As Boolean = True
        Dim NomeRoutine As String = "AgronicaCoreDataProvider.Agro_Sequenze.NuovoId_Tabella_EF()"

        Dim MessaggioErrore As String = ""

        Dim NuovoValore As Int32


        Try
            Dim sequenza_Tabelle As DbSet(Of Sequenza_Tabelle) = EFContext.Sequenza_Tabelle

            Dim seq = (From sequenza_Tabella In sequenza_Tabelle
                       Where sequenza_Tabella.Nome_Tabella = NomeTabella).FirstOrDefault()

            If seq IsNot Nothing Then

                NuovoValore = seq.Ultimo_Valore + 1

                'Se il nuovo valore supera l'ultimo valore valido ho un errore ...
                If NuovoValore > seq.End Then

                    'Imposto un valore dummy
                    NuovoValore = -1

                    'Genero un errore
                    Throw New Exception("L'indice ha raggiunto il limite superiore")
                Else

                    seq.Ultimo_Valore += 1

                End If

                EFContext.Entry(seq).State = EntityState.Modified

            Else 'Non esiste il record in Sequenza_Tabelle

                Select Case NomeTabella.ToLower
                    Case "contatti"
                        Base = -2000000000
                        Fine = 0
                        NuovoValore = -2000000000
                    Case "impresa"
                        Base = -2000000000
                        Fine = 0
                        NuovoValore = -2000000000
                    Case Else
                        NuovoValore = Base + 1
                End Select

                Dim newSeq As New Sequenza_Tabelle

                newSeq.Nome_Tabella = UtilityProvider.Agro_SQL_SaveText(LCase(NomeTabella))
                newSeq.Ultimo_Valore = UtilityProvider.Agro_SQL_SaveNum(NuovoValore)
                newSeq.Data_Creazione = Date.Now
                newSeq.Data_Modifica = Date.Now
                newSeq.Username_Creazione = UtilityProvider.Agro_SQL_SaveText(objParametri.UsernameOperazione)
                newSeq.Username_Modifica = UtilityProvider.Agro_SQL_SaveText(objParametri.UsernameOperazione)
                newSeq.Base = UtilityProvider.Agro_SQL_SaveNum(Base)
                newSeq.End = UtilityProvider.Agro_SQL_SaveNum(Fine)

                EFContext.Sequenza_Tabelle.Add(newSeq)

            End If

            If saveChanges Then
                EFContext.SaveChanges()
            End If

        Catch ex As Exception

            MessaggioErrore = ex.Message + " " + "[NomeTabella = " + NomeTabella + "]"

            If ex.InnerException IsNot Nothing Then
                MessaggioErrore &= " - Inner exception:" & ex.InnerException.Message
            End If

            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return NuovoValore

    End Function

    '############################################################################
    '############################################################################
    '################  REG_IMPIANTI      ########################################
    '############################################################################
    '############################################################################

    ''############################################################################
    'Public Function NuovoId_Reg_Impianti(ByVal Piva As String,
    '                                     ByVal Sa_Cod As Int32,
    '                                     ByVal Appezza As Int32,
    '                                     ByVal UserNameUtente As String,
    '                                     ByVal Base As Int32,
    '                                     ByVal Fine As Int32,
    '                                     ByRef objConnessione As DbConnection,
    '                                     ByRef objTransazione As DbTransaction,
    '                                     ByVal StringaConnessione As String,
    '                                     ByVal FlagVisibilita As Int32,
    '                                     ByVal DirectoryLOG As String,
    '                                     ByVal FileLOG As String,
    '                                     ByVal IdentificatoreUtente As String
    '                                     ) As Int32


    '    Dim NomeRoutine As String = "AgronicaCoreDataProvider.Agro_Sequenze.NuovoId_Reg_Impianti()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim DT As DataTable

    '    Dim xRisp As Boolean = False
    '    Dim NuovoValore As Int32

    '    Try

    '        If Piva = "" Then
    '            Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
    '        End If

    '        If Sa_Cod = 0 Then
    '            Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
    '        End If

    '        If Appezza = 0 Then
    '            Throw New Exception("Parametro non corretto nella query (Appezza obbligatorio)")
    '        End If


    '        '---------------------------------------------
    '        StrSQL.Length = 0
    '        StrSQL.Append(" SELECT   * ")
    '        StrSQL.Append(" FROM     SeqReg_Impianti ")
    '        StrSQL.Append(" WHERE    PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
    '        StrSQL.Append(" AND      Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
    '        StrSQL.Append(" AND      Appezza =  " & Agro_SQL_SaveNum(Appezza))

    '        '---------------------------------------------
    '        'DT = EseguiQuery_Lettura(objConnessione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

    '        If Not IsNothing(objTransazione) Then
    '            DT = EseguiQuery_Lettura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)
    '        Else
    '            DT = EseguiQuery_Lettura(objConnessione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)
    '        End If

    '        If DT.Rows.Count <> 0 Then

    '            NuovoValore = DT.Rows(0).Item("Id_Reg") + 1

    '            'Se il nuovo valore supera l'ultimo valore valido ho un errore ...
    '            If NuovoValore > DT.Rows(0).Item("End") Then

    '                'Imposto un valore dummy
    '                NuovoValore = -1

    '                'Genero un errore
    '                Throw New Exception("L'indice ha raggiunto il limite superiore")

    '            Else

    '                StrSQL.Length = 0
    '                StrSQL.Append(" UPDATE SeqReg_Impianti ")
    '                StrSQL.Append(" SET Id_Reg = " & Agro_SQL_SaveNum(NuovoValore) & " ")

    '                StrSQL.Append(" WHERE    PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
    '                StrSQL.Append(" AND      Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
    '                StrSQL.Append(" AND      Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")

    '                xRisp = EseguiQuery_Scrittura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

    '                If Not xRisp Then

    '                    'Imposto un valore dummy
    '                    NuovoValore = -1

    '                    'Genero un errore
    '                    Throw New Exception("Query di aggiornamento non riuscita.")

    '                End If

    '            End If
    '        Else

    '            NuovoValore = Base + 1

    '            StrSQL.Length = 0
    '            StrSQL.Append(" INSERT INTO SeqReg_Impianti ( ")
    '            StrSQL.Append(" PIVA, Sa_Cod, Appezza, Id_Reg, Base, [END], inviato, Data_Creazione, Data_Modifica, UserName_Creazione, UserName_Modifica ")
    '            StrSQL.Append(" )")
    '            StrSQL.Append(" VALUES( ")
    '            StrSQL.Append(" '" & Agro_SQL_SaveText(Piva) & "' ")
    '            StrSQL.Append(" ," & Agro_SQL_SaveNum(Sa_Cod) & " ")
    '            StrSQL.Append(" ," & Agro_SQL_SaveNum(Appezza) & " ")
    '            StrSQL.Append(" ," & Agro_SQL_SaveNum(NuovoValore) & " ")
    '            StrSQL.Append(" ," & Agro_SQL_SaveNum(Base) & " ")
    '            StrSQL.Append(" ," & Agro_SQL_SaveNum(Fine) & " ")
    '            StrSQL.Append(" , 0  ")
    '            StrSQL.Append(" , " & Agro_SQL_SaveDate(Date.Today) & "  ")
    '            StrSQL.Append(" , " & Agro_SQL_SaveDate(Date.Today) & "  ")
    '            StrSQL.Append(" ,'" & Agro_SQL_SaveText(UserNameUtente) & "' ")
    '            StrSQL.Append(" ,'" & Agro_SQL_SaveText(UserNameUtente) & "' ")
    '            StrSQL.Append(" )")

    '            xRisp = EseguiQuery_Scrittura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

    '            If Not xRisp Then

    '                'Imposto un valore dummy
    '                NuovoValore = -1

    '                'Genero un errore
    '                Throw New Exception("Query di inserimento non riuscita.")

    '            End If

    '        End If

    '        DT = Nothing

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        DT = Nothing
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return NuovoValore


    'End Function

    Public Function NuovoId_Reg_Impianti(ByVal Piva As String,
                                         ByVal Sa_Cod As Int32,
                                         ByVal Appezza As Int32,
                                         ByVal Base As Int32,
                                         ByVal Fine As Int32,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As Int32

        Dim NomeRoutine As String = "AgronicaCoreDataProvider.Agro_Sequenze.NuovoId_Reg_Impianti()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim xRisp As Boolean = False
        Dim NuovoValore As Int32

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Sa_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            End If

            If Appezza = 0 Then
                Throw New Exception("Parametro non corretto nella query (Appezza obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" SELECT   * ")
            StrSQL.Append(" FROM     SeqReg_Impianti ")
            StrSQL.Append(" WHERE    PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" AND      Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            StrSQL.Append(" AND      Appezza =  " & Agro_SQL_SaveNum(Appezza))

            'DT = EseguiQuery_Lettura(objConnessione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)
            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If DT.Rows.Count <> 0 Then

                NuovoValore = CInt(DT.Rows(0).Item("Id_Reg")) + 1

                'Se il nuovo valore supera l'ultimo valore valido ho un errore ...
                If NuovoValore > CInt(DT.Rows(0).Item("End")) Then

                    'Imposto un valore dummy
                    NuovoValore = -1

                    'Genero un errore
                    Throw New Exception("L'indice ha raggiunto il limite superiore")

                Else

                    StrSQL.Length = 0
                    StrSQL.Append(" UPDATE SeqReg_Impianti ")
                    StrSQL.Append(" SET Id_Reg = " & Agro_SQL_SaveNum(NuovoValore) & " ")

                    StrSQL.Append(" WHERE    PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
                    StrSQL.Append(" AND      Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    StrSQL.Append(" AND      Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")

                    '--------------------------------------------------------------------------
                    xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
                    '--------------------------------------------------------------------------

                    If Not xRisp Then

                        'Imposto un valore dummy
                        NuovoValore = -1

                        'Genero un errore
                        Throw New Exception("Query di aggiornamento non riuscita.")

                    End If

                End If
            Else

                NuovoValore = Base + 1

                StrSQL.Length = 0
                StrSQL.Append(" INSERT INTO SeqReg_Impianti ( ")
                StrSQL.Append(" PIVA, Sa_Cod, Appezza, Id_Reg, Base, [END], inviato, Data_Creazione, Data_Modifica, UserName_Creazione, UserName_Modifica ")
                StrSQL.Append(" )")
                StrSQL.Append(" VALUES( ")
                StrSQL.Append(" '" & Agro_SQL_SaveText(Piva) & "' ")
                StrSQL.Append(" ," & Agro_SQL_SaveNum(Sa_Cod) & " ")
                StrSQL.Append(" ," & Agro_SQL_SaveNum(Appezza) & " ")
                StrSQL.Append(" ," & Agro_SQL_SaveNum(NuovoValore) & " ")
                StrSQL.Append(" ," & Agro_SQL_SaveNum(Base) & " ")
                StrSQL.Append(" ," & Agro_SQL_SaveNum(Fine) & " ")
                StrSQL.Append(" , 0  ")
                StrSQL.Append(" , " & Agro_SQL_SaveDate(Date.Today) & "  ")
                StrSQL.Append(" , " & Agro_SQL_SaveDate(Date.Today) & "  ")
                StrSQL.Append(" ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append(" ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append(" )")

                '--------------------------------------------------------------------------
                xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
                '--------------------------------------------------------------------------


                If Not xRisp Then

                    'Imposto un valore dummy
                    NuovoValore = -1

                    'Genero un errore
                    Throw New Exception("Query di inserimento non riuscita.")

                End If

            End If

            DT = Nothing

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return NuovoValore

    End Function

    Public Function NuovoId_SeqMagazzino(ByVal Piva As String,
                                         ByVal Sa_Cod As Int32,
                                         ByVal Base As Int32,
                                         ByVal Fine As Int32,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As Int32


        Dim NomeRoutine As String = "AgronicaCoreDataProvider.Agro_Sequenze.NuovoId_SeqMagazzino()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim xRisp As Boolean = False
        Dim NuovoValore As Int32

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Sa_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            End If


            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" SELECT   * ")
            StrSQL.Append(" FROM     SeqMagazzino ")
            StrSQL.Append(" WHERE    PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" AND      Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")

            'DT = EseguiQuery_Lettura(objConnessione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)
            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If DT.Rows.Count <> 0 Then

                NuovoValore = CInt(DT.Rows(0).Item("Mag_Cod")) + 1

                'Se il nuovo valore supera l'ultimo valore valido ho un errore ...
                If NuovoValore > CInt(DT.Rows(0).Item("End")) Then

                    'Imposto un valore dummy
                    NuovoValore = -1

                    'Genero un errore
                    Throw New Exception("L'indice ha raggiunto il limite superiore")

                Else

                    StrSQL.Length = 0
                    StrSQL.Append(" UPDATE SeqMagazzino ")
                    StrSQL.Append(" SET Mag_Cod = " & Agro_SQL_SaveNum(NuovoValore) & " ")

                    StrSQL.Append(" WHERE    PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
                    StrSQL.Append(" AND      Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")

                    '--------------------------------------------------------------------------
                    xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
                    '--------------------------------------------------------------------------


                    If Not xRisp Then

                        'Imposto un valore dummy
                        NuovoValore = -1

                        'Genero un errore
                        Throw New Exception("Query di aggiornamento non riuscita.")

                    End If

                End If
            Else

                NuovoValore = Base + 1

                StrSQL.Length = 0
                StrSQL.Append(" INSERT INTO SeqMagazzino ( ")
                StrSQL.Append(" PIVA, Sa_Cod, Mag_Cod, Base, [END], inviato, Data_Creazione, Data_Modifica, UserName_Creazione, UserName_Modifica ")
                StrSQL.Append(" )")
                StrSQL.Append(" VALUES( ")
                StrSQL.Append(" '" & Agro_SQL_SaveText(Piva) & "' ")
                StrSQL.Append(" ," & Agro_SQL_SaveNum(Sa_Cod) & " ")
                StrSQL.Append(" ," & Agro_SQL_SaveNum(NuovoValore) & " ")
                StrSQL.Append(" ," & Agro_SQL_SaveNum(Base) & " ")
                StrSQL.Append(" ," & Agro_SQL_SaveNum(Fine) & " ")
                StrSQL.Append(" , 0  ")
                StrSQL.Append(" , " & Agro_SQL_SaveDate(Date.Today) & "  ")
                StrSQL.Append(" , " & Agro_SQL_SaveDate(Date.Today) & "  ")
                StrSQL.Append(" ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append(" ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append(" )")

                '--------------------------------------------------------------------------
                xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
                '--------------------------------------------------------------------------


                If Not xRisp Then

                    'Imposto un valore dummy
                    NuovoValore = -1

                    'Genero un errore
                    Throw New Exception("Query di inserimento non riuscita.")

                End If

            End If

            DT = Nothing

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return NuovoValore


    End Function

    '############################################################################
    '############################################################################
    '########## Funzioni per il calcolo Id_reg e Appezza senza BaseCode #########
    '############################################################################
    '############################################################################
    'Queste funzioni calcolano Id_reg e Appezza senza il BaseCode e viceversa
    'Servono per la tabella Grafica dove si è scelto di utilizzare ID per contenere
    'sia Appezza che Id_Reg
    Public Sub AppezzaIdReg_Comprimi(ByRef ID As String,
                                     ByVal Prefisso As String,
                                     ByVal Appezza As Integer,
                                     ByVal Id_Reg As Integer,
                                     ByVal BaseCode As Integer,
                                     ByRef objParametri As AgronicaCoreParametri)

        'Dato: Appezza e Id_Reg --> calcola --> ID

        Const nomeRoutine = "AgronicaCoreDataProvider.Agro_Sequenze.AppezzaIdReg_Comprimi()"

        Dim messaggioErrore As String = ""

        Dim AppezzaCompresso As Integer
        Dim AppezzaPulito As Integer
        Dim Id_RegCompresso As Integer
        Dim Id_RegPulito As Integer

        Dim Segno As String
        Dim Lung As Integer

        Try
            '------------------------------------------

            If Appezza >= 0 Then
                Lung = 8
                Segno = ""

                'modifica del 17/02/2011: problema sui dati importati con gias2gias
                'capitava che l'appezza  fosse più piccolo del basecode, di conseguenza veniva utilizzato l'appezza
                'e il suo esadecimale era > di 4 caratteri
                'AppezzaCompresso = IIf(Appezza > BaseCode, Appezza - BaseCode, Appezza)
                If Appezza > 2 ^ 17 Then
                    AppezzaPulito = Appezza - ((Appezza \ (2 ^ 17)) * (2 ^ 17))
                Else
                    AppezzaPulito = Appezza
                End If

                AppezzaCompresso = AppezzaPulito

                ID = Prefisso & Left("0000", 4 - Len(CStr(Hex(AppezzaCompresso)))) & CStr(Hex(AppezzaCompresso))

                If Id_Reg >= 0 Then

                    'stessa modifica dell'appezza
                    'Id_RegCompresso = IIf(Id_Reg > BaseCode, Id_Reg - BaseCode, Id_Reg)
                    If Id_Reg > 2 ^ 17 Then
                        AppezzaPulito = Id_Reg - ((Id_Reg \ (2 ^ 17)) * (2 ^ 17))
                    Else
                        AppezzaPulito = Id_Reg
                    End If
                    Id_RegCompresso = Id_RegPulito

                    ID = ID & Left("0000", 4 - Len(CStr(Hex(Id_RegCompresso)))) & CStr(Hex(Id_RegCompresso))

                Else
                    Lung = 3
                    Segno = "-"
                    ID = ID & Prefisso & Segno & Right("0000" & Hex(Math.Abs(Id_Reg)), Lung)

                End If
            Else
                Lung = 3
                Segno = "-"
                ID = Prefisso & Segno & Right("0000" & Hex(Math.Abs(Appezza)), Lung)
                ID = ID & Segno & Right("0000" & Hex(Math.Abs(Id_Reg)), Lung)

            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

    ''Queste funzioni calcolano Id_reg e Appezza senza il BaseCode e viceversa
    ''Servono per la tabella Grafica dove si è scelto di utilizzare ID per contenere
    ''sia Appezza che Id_Reg
    'Public Sub AppezzaIdReg_Comprimi(ByRef ID As String, _
    '                                 ByVal Prefisso As String, _
    '                                 ByVal Appezza As Int32, _
    '                                 ByVal Id_Reg As Int32, _
    '                                 ByVal BaseCode As Int32, _
    '                                 ByVal DirectoryLOG As String, _
    '                                 ByVal FileLOG As String, _
    '                                 ByVal IdentificatoreUtente As String)

    '    'Dato: Appezza e Id_Reg --> calcola --> ID

    '    Dim NomeRoutine As String = "AgronicaCoreDataProvider.Agro_Sequenze.AppezzaIdReg_Comprimi()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder


    '    Dim AppezzaCompresso As Long
    '    Dim Id_RegCompresso As Long

    '    Dim Segno As String
    '    Dim Lung As Integer

    '    Try
    '        '------------------------------------------

    '        If Appezza >= 0 Then
    '            Lung = 8
    '            Segno = ""

    '            AppezzaCompresso = IIf(Appezza > BaseCode, Appezza - BaseCode, Appezza)
    '            ID = Prefisso & Left("0000", 4 - Len(CStr(Hex(AppezzaCompresso)))) & CStr(Hex(AppezzaCompresso))

    '            If Id_Reg >= 0 Then

    '                Id_RegCompresso = IIf(Id_Reg > BaseCode, Id_Reg - BaseCode, Id_Reg)
    '                ID = ID & Left("0000", 4 - Len(CStr(Hex(Id_RegCompresso)))) & CStr(Hex(Id_RegCompresso))

    '            Else
    '                Lung = 3
    '                Segno = "-"
    '                ID = ID & Prefisso & Segno & Right("0000" & Hex(Math.Abs(Id_Reg)), Lung)

    '            End If
    '        Else
    '            Lung = 3
    '            Segno = "-"
    '            ID = Prefisso & Segno & Right("0000" & Hex(Math.Abs(Appezza)), Lung)
    '            ID = ID & Segno & Right("0000" & Hex(Math.Abs(Id_Reg)), Lung)

    '        End If

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try


    'End Sub




    ''############################################################################
    'Public Function NuovoId_CentriAziendali(ByVal Piva As String,
    '                                        ByVal UserNameUtente As String,
    '                                        ByVal Base As Int32,
    '                                        ByVal Fine As Int32,
    '                                        ByRef objConnessione As DbConnection,
    '                                        ByRef objTransazione As DbTransaction,
    '                                        ByVal StringaConnessione As String,
    '                                        ByVal FlagVisibilita As Int32,
    '                                        ByVal DirectoryLOG As String,
    '                                        ByVal FileLOG As String,
    '                                        ByVal IdentificatoreUtente As String
    '                                        ) As Int32
    '    Dim NomeRoutine As String = "AgronicaCoreDataProvider.Agro_Sequenze.NuovoId_CentriAziendali()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim DT As DataTable

    '    Dim xRisp As Boolean = False
    '    Dim NuovoValore As Int32

    '    Try

    '        If Piva = "" Then
    '            Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
    '        End If

    '        '---------------------------------------------
    '        StrSQL.Length = 0

    '        '------------------------------

    '        'Query per il prelievo dei dati

    '        StrSQL.Append(" SELECT   * ")
    '        StrSQL.Append(" FROM     SeqCentri_Aziendali ")
    '        StrSQL.Append(" WHERE    PIVA = '" & Piva & "' ")

    '        '---------------------------------------------


    '        If Not IsNothing(objTransazione) Then
    '            DT = EseguiQuery_Lettura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)
    '        Else
    '            DT = EseguiQuery_Lettura(objConnessione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)
    '        End If

    '        If DT.Rows.Count <> 0 Then

    '            NuovoValore = DT.Rows(0).Item("Sa_Cod") + 1

    '            'Se il nuovo valore supera l'ultimo valore valido ho un errore ...
    '            If NuovoValore > DT.Rows(0).Item("End") Then

    '                'Imposto un valore dummy
    '                NuovoValore = -1

    '                'Genero un errore
    '                Throw New Exception("L'indice ha raggiunto il limite superiore")

    '            Else

    '                StrSQL.Length = 0
    '                StrSQL.Append(" UPDATE SeqCentri_Aziendali ")
    '                StrSQL.Append(" SET Sa_Cod = " & Agro_SQL_SaveNum(NuovoValore) & " ")

    '                StrSQL.Append(" WHERE    PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")

    '                xRisp = EseguiQuery_Scrittura(objTransazione.Connection, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

    '                If Not xRisp Then

    '                    'Imposto un valore dummy
    '                    NuovoValore = -1

    '                    'Genero un errore
    '                    Throw New Exception("Query di aggiornamento non riuscita.")

    '                End If

    '            End If
    '        Else

    '            NuovoValore = Base + 1

    '            StrSQL.Length = 0
    '            StrSQL.Append(" INSERT INTO SeqCentri_Aziendali ( ")
    '            StrSQL.Append(" PIVA, Sa_Cod, Base, [END], inviato, Data_Creazione, Data_Modifica, UserName_Creazione, UserName_Modifica ")
    '            StrSQL.Append(" )")
    '            StrSQL.Append(" VALUES( ")
    '            StrSQL.Append(" '" & Agro_SQL_SaveText(Piva) & "' ")
    '            StrSQL.Append(" ," & Agro_SQL_SaveNum(NuovoValore) & " ")
    '            StrSQL.Append(" ," & Agro_SQL_SaveNum(Base) & " ")
    '            StrSQL.Append(" ," & Agro_SQL_SaveNum(Fine) & " ")
    '            StrSQL.Append(" , 0  ")
    '            StrSQL.Append(" , " & Agro_SQL_SaveDate(Date.Today) & "  ")
    '            StrSQL.Append(" , " & Agro_SQL_SaveDate(Date.Today) & "  ")
    '            StrSQL.Append(" ,'" & Agro_SQL_SaveText(UserNameUtente) & "' ")
    '            StrSQL.Append(" ,'" & Agro_SQL_SaveText(UserNameUtente) & "' ")
    '            StrSQL.Append(" )")

    '            xRisp = EseguiQuery_Scrittura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

    '            If Not xRisp Then

    '                'Imposto un valore dummy
    '                NuovoValore = -1

    '                'Genero un errore
    '                Throw New Exception("Query di inserimento non riuscita.")

    '            End If

    '        End If

    '        DT = Nothing

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        DT = Nothing
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return NuovoValore

    'End Function







    '############################################################################

    Public Function NuovoId_CentriAziendali(ByVal Piva As String,
                                            ByVal Base As Int32,
                                            ByVal Fine As Int32,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As Int32
        Dim NomeRoutine As String = "AgronicaCoreDataProvider.Agro_Sequenze.NuovoId_CentriAziendali()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim xRisp As Boolean = False
        Dim NuovoValore As Int32

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0

            '------------------------------

            'Query per il prelievo dei dati

            StrSQL.Append(" SELECT   * ")
            StrSQL.Append(" FROM     SeqCentri_Aziendali ")
            StrSQL.Append(" WHERE    PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")

            '---------------------------------------------


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If DT.Rows.Count <> 0 Then

                NuovoValore = CInt(DT.Rows(0).Item("Sa_Cod")) + 1

                'Se il nuovo valore supera l'ultimo valore valido ho un errore ...
                If NuovoValore > CInt(DT.Rows(0).Item("End")) Then

                    'Imposto un valore dummy
                    NuovoValore = -1

                    'Genero un errore
                    Throw New Exception("L'indice ha raggiunto il limite superiore")

                Else

                    StrSQL.Length = 0
                    StrSQL.Append(" UPDATE SeqCentri_Aziendali ")
                    StrSQL.Append(" SET Sa_Cod = " & Agro_SQL_SaveNum(NuovoValore) & " ")

                    StrSQL.Append(" WHERE    PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")

                    '--------------------------------------------------------------------------
                    xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
                    '--------------------------------------------------------------------------

                    If Not xRisp Then

                        'Imposto un valore dummy
                        NuovoValore = -1

                        'Genero un errore
                        Throw New Exception("Query di aggiornamento non riuscita.")

                    End If

                End If
            Else

                NuovoValore = Base + 1

                StrSQL.Length = 0
                StrSQL.Append(" INSERT INTO SeqCentri_Aziendali ( ")
                StrSQL.Append(" PIVA, Sa_Cod, Base, [END], inviato, Data_Creazione, Data_Modifica, UserName_Creazione, UserName_Modifica ")
                StrSQL.Append(" )")
                StrSQL.Append(" VALUES( ")
                StrSQL.Append(" '" & Agro_SQL_SaveText(Piva) & "' ")
                StrSQL.Append(" ," & Agro_SQL_SaveNum(NuovoValore) & " ")
                StrSQL.Append(" ," & Agro_SQL_SaveNum(Base) & " ")
                StrSQL.Append(" ," & Agro_SQL_SaveNum(Fine) & " ")
                StrSQL.Append(" , 0  ")
                StrSQL.Append(" , " & Agro_SQL_SaveDate(Date.Today) & "  ")
                StrSQL.Append(" , " & Agro_SQL_SaveDate(Date.Today) & "  ")
                StrSQL.Append(" ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append(" ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append(" )")

                '--------------------------------------------------------------------------
                xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
                '--------------------------------------------------------------------------

                If Not xRisp Then

                    'Imposto un valore dummy
                    NuovoValore = -1

                    'Genero un errore
                    Throw New Exception("Query di inserimento non riuscita.")

                End If

            End If

            DT = Nothing

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return NuovoValore

    End Function

    ''############################################################################
    'Public Function NuovoId_xPiva_xSaCod(
    '                                ByVal NomeTabella As String,
    '                                ByVal NomeCampoContatore As String,
    '                                ByVal Piva As String,
    '                                ByVal Sa_Cod As Int32,
    '                                    ByVal UserNameUtente As String,
    '                                    ByVal Base As Int32,
    '                                    ByVal Fine As Int32,
    '                                    ByRef objConnessione As DbConnection,
    '                                    ByRef objTransazione As DbTransaction,
    '                                    ByVal StringaConnessione As String,
    '                                    ByVal FlagVisibilita As Int32,
    '                                    ByVal DirectoryLOG As String,
    '                                    ByVal FileLOG As String,
    '                                    ByVal IdentificatoreUtente As String) As _
    '                                        Int32

    '    Dim NomeRoutine As String = "AgronicaCoreDataProvider.Agro_Sequenze.NuovoId_xPiva_xSaCod()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim DT As DataTable

    '    Dim xRisp As Boolean = False
    '    Dim NuovoValore As Int32

    '    Try

    '        If Piva = "" Then
    '            Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
    '        End If

    '        If Sa_Cod = 0 Then
    '            Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
    '        End If

    '        '---------------------------------------------
    '        'Query per il prelievo dei dati
    '        StrSQL.Length = 0
    '        StrSQL.Append(" SELECT   * ")
    '        StrSQL.Append(" FROM     " & NomeTabella & " ")
    '        StrSQL.Append(" WHERE    PIVA =  '" & Agro_SQL_SaveText(Piva) & "' ")
    '        StrSQL.Append(" AND      Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
    '        '---------------------------------------------


    '        If Not IsNothing(objTransazione) Then
    '            DT = EseguiQuery_Lettura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)
    '        Else
    '            DT = EseguiQuery_Lettura(objConnessione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)
    '        End If



    '        If DT.Rows.Count <> 0 Then

    '            NuovoValore = DT.Rows(0).Item(NomeCampoContatore) + 1

    '            'Se il nuovo valore supera l'ultimo valore valido ho un errore ...
    '            If NuovoValore > DT.Rows(0).Item("End") Then

    '                'Imposto un valore dummy
    '                NuovoValore = -1

    '                'Genero un errore
    '                Throw New Exception("L'indice ha raggiunto il limite superiore")

    '            Else

    '                '---------------------------------------------
    '                StrSQL.Length = 0
    '                StrSQL.Append(" UPDATE  " & NomeTabella & " ")
    '                StrSQL.Append(" SET     " & NomeCampoContatore & " = " & Agro_SQL_SaveNum(NuovoValore) & " ")
    '                StrSQL.Append(" WHERE   PIVA =  '" & Agro_SQL_SaveText(Piva) & "' ")
    '                StrSQL.Append(" AND     Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
    '                '---------------------------------------------

    '                xRisp = EseguiQuery_Scrittura(objTransazione.Connection, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

    '                If xRisp = False Then

    '                    'Imposto un valore dummy
    '                    NuovoValore = -1

    '                    'Genero un errore
    '                    Throw New Exception("Query di aggiornamento di (" & NomeTabella & ") non riuscita.")

    '                End If

    '            End If
    '        Else

    '            NuovoValore = Base + 1

    '            StrSQL.Length = 0
    '            StrSQL.Append(" INSERT INTO " & NomeTabella & " ( ")
    '            StrSQL.Append("             " & NomeCampoContatore & ", ")
    '            StrSQL.Append("             PIVA, Sa_Cod, Base, [END], ")
    '            StrSQL.Append("             inviato, Data_Creazione, Data_Modifica, ")
    '            StrSQL.Append("             UserName_Creazione, UserName_Modifica ")
    '            StrSQL.Append("             )")
    '            StrSQL.Append(" VALUES( ")
    '            StrSQL.Append("              " & Agro_SQL_SaveNum(NuovoValore) & " ")
    '            StrSQL.Append("             ,'" & Agro_SQL_SaveText(Piva) & "' ")
    '            StrSQL.Append("             ," & Agro_SQL_SaveNum(Sa_Cod) & " ")
    '            StrSQL.Append("             ," & Agro_SQL_SaveNum(Base) & " ")
    '            StrSQL.Append("             ," & Agro_SQL_SaveNum(Fine) & " ")
    '            StrSQL.Append("             , 0  ")
    '            StrSQL.Append("             , " & Agro_SQL_SaveDate(Date.Today) & "  ")
    '            StrSQL.Append("             , " & Agro_SQL_SaveDate(Date.Today) & "  ")
    '            StrSQL.Append("             ,'" & Agro_SQL_SaveText(UserNameUtente) & "' ")
    '            StrSQL.Append("             ,'" & Agro_SQL_SaveText(UserNameUtente) & "' ")
    '            StrSQL.Append("         )")

    '            xRisp = EseguiQuery_Scrittura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

    '            If Not xRisp Then

    '                'Imposto un valore dummy
    '                NuovoValore = -1

    '                'Genero un errore
    '                Throw New Exception("Query di inserimento in (" & NomeTabella & ") non riuscita.")

    '            End If

    '        End If

    '        DT = Nothing

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        DT = Nothing
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return NuovoValore

    'End Function

    Public Function NuovoId_xPiva_xSaCod(
                                        ByVal NomeTabella As String,
                                        ByVal NomeCampoContatore As String,
                                        ByVal Piva As String,
                                        ByVal Sa_Cod As Int32,
                                        ByVal Base As Int32,
                                        ByVal Fine As Int32,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As Int32

        Dim NomeRoutine As String = "AgronicaCoreDataProvider.Agro_Sequenze.NuovoId_xPiva_xSaCod()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim xRisp As Boolean = False
        Dim NuovoValore As Int32

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Sa_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            End If

            '---------------------------------------------
            'Query per il prelievo dei dati
            StrSQL.Length = 0
            StrSQL.Append(" SELECT   * ")
            StrSQL.Append(" FROM     " & NomeTabella & " ")
            StrSQL.Append(" WHERE    PIVA =  '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" AND      Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
            '---------------------------------------------


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


            If DT.Rows.Count <> 0 Then

                NuovoValore = CInt(DT.Rows(0).Item(NomeCampoContatore)) + 1

                'Se il nuovo valore supera l'ultimo valore valido ho un errore ...
                If NuovoValore > CInt(DT.Rows(0).Item("End")) Then

                    'Imposto un valore dummy
                    NuovoValore = -1

                    'Genero un errore
                    Throw New Exception("L'indice ha raggiunto il limite superiore")

                Else

                    '---------------------------------------------
                    StrSQL.Length = 0
                    StrSQL.Append(" UPDATE  " & NomeTabella & " ")
                    StrSQL.Append(" SET     " & NomeCampoContatore & " = " & Agro_SQL_SaveNum(NuovoValore) & " ")
                    StrSQL.Append(" WHERE   PIVA =  '" & Agro_SQL_SaveText(Piva) & "' ")
                    StrSQL.Append(" AND     Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
                    '---------------------------------------------

                    '--------------------------------------------------------------------------
                    xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
                    '--------------------------------------------------------------------------

                    If xRisp = False Then

                        'Imposto un valore dummy
                        NuovoValore = -1

                        'Genero un errore
                        Throw New Exception("Query di aggiornamento di (" & NomeTabella & ") non riuscita.")

                    End If

                End If
            Else

                NuovoValore = Base + 1

                StrSQL.Length = 0
                StrSQL.Append(" INSERT INTO " & NomeTabella & " ( ")
                StrSQL.Append("             " & NomeCampoContatore & ", ")
                StrSQL.Append("             PIVA, Sa_Cod, Base, [END], ")
                StrSQL.Append("             inviato, Data_Creazione, Data_Modifica, ")
                StrSQL.Append("             UserName_Creazione, UserName_Modifica ")
                StrSQL.Append("             )")
                StrSQL.Append(" VALUES( ")
                StrSQL.Append("              " & Agro_SQL_SaveNum(NuovoValore) & " ")
                StrSQL.Append("             ,'" & Agro_SQL_SaveText(Piva) & "' ")
                StrSQL.Append("             ," & Agro_SQL_SaveNum(Sa_Cod) & " ")
                StrSQL.Append("             ," & Agro_SQL_SaveNum(Base) & " ")
                StrSQL.Append("             ," & Agro_SQL_SaveNum(Fine) & " ")
                StrSQL.Append("             , 0  ")
                StrSQL.Append("             , " & Agro_SQL_SaveDate(Date.Today) & "  ")
                StrSQL.Append("             , " & Agro_SQL_SaveDate(Date.Today) & "  ")
                StrSQL.Append("             ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("             ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         )")

                '--------------------------------------------------------------------------
                xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
                '--------------------------------------------------------------------------

                If Not xRisp Then

                    'Imposto un valore dummy
                    NuovoValore = -1

                    'Genero un errore
                    Throw New Exception("Query di inserimento in (" & NomeTabella & ") non riuscita.")

                End If

            End If

            DT = Nothing

        Catch ex As Exception

            MessaggioErrore = ex.Message + " " + "[NomeTabella = " + NomeTabella + "]"

            If ex.InnerException IsNot Nothing Then
                MessaggioErrore &= " - Inner exception:" & ex.InnerException.Message
            End If

            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return NuovoValore

    End Function

    '#####################################################################
    Public Function CancellaSeqCentri_Aziendali(ByVal Piva As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreDataProvider.Agro_Sequenze.CancellaSeqCentri_Aziendali()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE SeqCentri_Aziendali ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0 ")

            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM SeqCentri_Aziendali ")

                'StrSQL.Append(" WHERE  Inviato = 0 ")
                StrSQL.Append(" WHERE  1=1 ")

            End If

            StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            xRisp = False
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp


    End Function

    '#####################################################################
    Public Function CancellaSeqMagazzino(ByVal Piva As String,
                                         ByVal Sa_Cod As Int32,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                         ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreDataProvider.Agro_Sequenze.CancellaSeqMagazzino()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Sa_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            End If

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE SeqMagazzino ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0 ")

            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM SeqMagazzino ")

                'StrSQL.Append(" WHERE  Inviato = 0 ")
                StrSQL.Append(" WHERE  1=1 ")

            End If

            StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            xRisp = False
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp


    End Function

    '#####################################################################
    Public Function CancellaSeqCampi(ByVal Piva As String,
                                     ByVal Sa_Cod As Int32,
                                     ByRef objParametri As AgronicaCoreParametri
                                     ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreDataProvider.Agro_Sequenze.CancellaSeqCampi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Sa_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            End If

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE SeqCampi ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0 ")

            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM SeqCampi ")

                'StrSQL.Append(" WHERE  Inviato = 0 ")
                StrSQL.Append(" WHERE  1=1 ")

            End If

            StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            xRisp = False
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp


    End Function

    '#####################################################################
    Public Function CancellaSeqAppezzamento(ByVal Piva As String,
                                            ByVal Sa_Cod As Int32,
                                            ByRef objParametri As AgronicaCoreParametri
                                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreDataProvider.Agro_Sequenze.CancellaSeqCampi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Sa_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            End If

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE SeqAppezzamento ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0 ")

            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM SeqAppezzamento ")

                'StrSQL.Append(" WHERE  Inviato = 0 ")
                StrSQL.Append(" WHERE  1=1 ")

            End If

            StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            xRisp = False
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp


    End Function

    '#####################################################################
    Public Function CancellaSeqReg_Impianti(ByVal Piva As String,
                                            ByVal Sa_Cod As Int32,
                                            ByVal Appezza As Int32,
                                            ByRef objParametri As AgronicaCoreParametri
                                            ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreDataProvider.Agro_Sequenze.CancellaSeqCampi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Sa_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            End If

            If Appezza = 0 Then
                Throw New Exception("Parametro non corretto nella query (Appezza obbligatorio)")
            End If

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE SeqReg_Impianti ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0 ")

            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM SeqReg_Impianti ")

                'StrSQL.Append(" WHERE  Inviato = 0 ")
                StrSQL.Append(" WHERE  1=1 ")

            End If

            StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")

            StrSQL.Append(" AND Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            xRisp = False
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp


    End Function

    Public Function UltimoSaCod(ByVal Piva As String,
                                ByRef objParametri As AgronicaCoreParametri
                                ) As Integer

        Const nomeRoutine = "AgronicaCoreDataProvider.Agro_Sequenze.UltimoSaCod()"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Dim sa_cod As Integer = 0

        Try
            StrSQL.Append(" SELECT   sa_cod ")
            StrSQL.Append(" FROM     SeqCentri_Aziendali ")
            StrSQL.Append(" WHERE    PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")

            '-------------------------------------------------------------------------- 
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Return sa_cod
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If (Not IsNothing(dt)) AndAlso
            (dt.Rows.Count > 0) Then
            sa_cod = CInt(dt.Rows(0).Item("sa_cod"))
        End If

        Return sa_cod

    End Function

    Private Function UltimoSequenzaTabelle(ByVal NomeTabella As String,
                                           ByRef connection As DbConnection,
                                           ByVal StringaConnessione As String,
                                           ByVal LogDirectory As String,
                                           ByVal LogFileName As String,
                                           ByVal UsernameOperazione As String,
                                           objParametri As AgronicaCoreParametri) As Integer

        Const nomeRoutine = "AgronicaCoreDataProvider.Agro_Sequenze.UltimoSequenzaTabelle()"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Dim ultimoValore As Integer = 0

        Try

            StrSQL.Append(" SELECT   Ultimo_Valore ")
            StrSQL.Append(" FROM     Sequenza_Tabelle ")
            StrSQL.Append(" WHERE    Nome_Tabella = '" & Agro_SQL_SaveText(NomeTabella) & "' ")

            '-------------------------------------------------------------------------- 
            dt = EseguiQuery_Lettura(connection, Nothing, StringaConnessione, StrSQL.ToString, LogDirectory, LogFileName, UsernameOperazione, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message + " " + "[NomeTabella = " + NomeTabella + "]"

            If ex.InnerException IsNot Nothing Then
                messaggioErrore &= " - Inner exception:" & ex.InnerException.Message
            End If

            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Return ultimoValore
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        If (Not IsNothing(dt)) AndAlso
            (dt.Rows.Count > 0) Then
            ultimoValore = CInt(dt.Rows(0).Item("Ultimo_Valore"))
        End If

        Return ultimoValore

    End Function

End Class
