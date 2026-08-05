Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.DataProviderExtensions

Public Class Allegati_EntitaxDocumenti_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '#########################################################################
    Public Function Scrivi(
                            ByVal Allegati_Documenti_Cod As Integer,
                            ByVal Allegati_Entita_Cod As Integer,
                            ByVal Piva As String,
                            ByVal Sa_Cod As Integer,
                            ByVal Campo_Cod As Integer,
                            ByVal Appezza As Integer,
                            ByVal Id_Imp As Integer,
                            ByVal Fabbricato_Cod As Integer,
                            ByVal Prov As String,
                            ByVal Com As String,
                            ByVal Sezione As String,
                            ByVal Foglio As Integer,
                            ByVal Numero As Integer,
                            ByVal Subalterno As String,
                            ByVal ID_Oggetto_Grafico As String,
                            ByVal Chiave_Albero_Imprese As String,
                            ByVal Analisi_Testata_Cod As String,
                            ByVal Programmazione_Cod As Integer,
                            ByVal Programmazione_Entita_Cod As Integer,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            , Optional ByVal Data_creazione As DateTime = #2/1/1900# _
                            , Optional ByVal Data_modifica As DateTime = #2/1/1900# _
                            , Optional ByVal username_creazione As String = "" _
                            , Optional ByVal username_modifica As String = ""
                                ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Allegati_EntitaxDocumenti_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False


        If Data_creazione = #2/1/1900# Then
            Data_creazione = Date.Now
        End If

        If Data_modifica = #2/1/1900# Then
            Data_modifica = Date.Now
        End If

        If username_creazione = "" Then
            username_creazione = objParametri.UsernameOperazione
        End If

        If username_modifica = "" Then
            username_modifica = objParametri.UsernameOperazione
        End If



        Try

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append("INSERT INTO Allegati_EntitaxDocumenti( ")
            StrSQL.Append("            Allegati_Documenti_SuperUser, Allegati_Documenti_Cod,  Allegati_Entita_Cod, ")
            StrSQL.Append("            Piva, Sa_Cod, Campo_Cod, Appezza, Id_Imp, Fabbricato_Cod, ")
            StrSQL.Append("            Prov, Com, Sezione, Foglio, Numero, Subalterno, ")
            StrSQL.Append("            ID_Oggetto_Grafico, Chiave_Albero_Imprese, ")
            StrSQL.Append("            Analisi_Testata_Cod, ")
            StrSQL.Append("            Programmazione_Cod, Programmazione_Entita_Cod, ")
            StrSQL.Append("            Inviato, DataInvio, ")
            StrSQL.Append("            Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("            UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("            Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("            ) ")

            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Allegati_Documenti_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Allegati_Entita_Cod) & "  ")

            StrSQL.Append("         , '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Campo_Cod) & " ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Appezza) & " ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Imp) & " ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Fabbricato_Cod) & " ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(Prov) & "' ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(Com) & "' ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(Sezione) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Foglio) & " ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Numero) & " ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(Subalterno) & "' ")

            StrSQL.Append("         , '" & Agro_SQL_SaveText(ID_Oggetto_Grafico) & "' ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(Chiave_Albero_Imprese) & "' ")

            StrSQL.Append("         , " & Agro_SQL_SaveNum(Analisi_Testata_Cod) & " ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Programmazione_Cod) & " ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Programmazione_Entita_Cod) & " ")

            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append(")")

            '---------------------------------------------


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


    '#########################################################################
    Public Function Scrivi_Analisi( _
                                ByVal Allegati_Documenti_Cod As Integer, _
                                ByVal Allegati_Entita_Cod As Integer, _
                                ByVal Analisi_Testata_Cod As String, _
                                ByVal Validita_Inizio As Date, _
                                ByVal Validita_Fine As Date, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Allegati_EntitaxDocumenti_W.Scrivi_Analisi()"

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

            StrSQL.Append("INSERT INTO Allegati_EntitaxDocumenti( ")
            StrSQL.Append("            Allegati_Documenti_SuperUser, Allegati_Documenti_Cod,  Allegati_Entita_Cod, ")
            StrSQL.Append("            Analisi_Testata_Cod, ")
            StrSQL.Append("            Inviato, DataInvio, ")
            StrSQL.Append("            Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("            UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("            Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("            ) ")

            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Allegati_Documenti_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Allegati_Entita_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Analisi_Testata_Cod) & " ")



            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append(")")

            '---------------------------------------------


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


    '#########################################################################
    Public Function Scrivi_CampionePDC( _
                                ByVal Allegati_Documenti_Cod As String, _
                                ByVal Allegati_Entita_Cod As Integer, _
                                ByVal ID_PDC_Testata As Integer, _
                                ByVal ID_PDC_Dettagli As Integer, _
                                ByVal Validita_Inizio As Date, _
                                ByVal Validita_Fine As Date, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Allegati_EntitaxDocumenti_W.Scrivi_CampionePDC()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append("INSERT INTO Allegati_EntitaxDocumenti( ")
            StrSQL.Append("            Allegati_Documenti_SuperUser, Allegati_Documenti_Cod,  Allegati_Entita_Cod, ")
            StrSQL.Append("            ID_PDC_Testata, ID_PDC_Dettagli,  ")
            StrSQL.Append("            Inviato, DataInvio, ")
            StrSQL.Append("            Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("            UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("            Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("            ) ")

            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Allegati_Documenti_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Allegati_Entita_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(ID_PDC_Testata) & " ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(ID_PDC_Dettagli) & " ")


            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append(")")

            '---------------------------------------------


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


    '#########################################################################
    Public Function Cancella_Programmazione_cod( _
                            ByVal Programmazione_cod As Integer, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Allegati_EntitaxDocumenti_W.Cancella_Analisi()"

        '====================================================================================
        'Parametri opzionali :
        '   Sa_Cod = 0           =>  si cancellano tutti gli appezzamenti dell'impresa
        '   Appezza = 0          =>  si cancellano tutti gli appezzamenti del centro aziendale

        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0

                StrSQL.Append(" UPDATE  Allegati_EntitaxDocumenti ")
                StrSQL.Append(" SET ")
                StrSQL.Append("          Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         ,Data_Modifica = " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE Allegati_Documenti_SuperUser = '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "' ")
                StrSQL.Append("   AND    Inviato >= 0 ")
            Else

                StrSQL.Length = 0

                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     Allegati_EntitaxDocumenti ")
                'StrSQL.Append(" WHERE    Allegati_Documenti_SuperUser = '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "' ")
                StrSQL.Append(" WHERE    1=1 ")
                StrSQL.Append(" AND      Inviato >= 0 ")

            End If

            StrSQL.Append(" AND Programmazione_cod = " & Agro_SQL_SaveNum(Programmazione_cod) & " ")


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

    Public Function Cancella_Fabbricato(
                            ByVal Piva As String,
                            ByVal sa_cod As Integer,
                            ByVal fabbricato_cod As Integer,
                            ByVal xFiltroAggiuntivo As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Allegati_EntitaxDocumenti_W.Cancella_Fabbricato()"

        '====================================================================================
        'Parametri opzionali :
        '   Sa_Cod = 0           =>  si cancellano tutti gli appezzamenti dell'impresa
        '   Appezza = 0          =>  si cancellano tutti gli appezzamenti del centro aziendale

        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0

                StrSQL.AppendLine(" UPDATE  Allegati_EntitaxDocumenti ")
                StrSQL.AppendLine(" SET ")
                StrSQL.AppendLine("          Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.AppendLine("         ,Data_Modifica = " & Agro_SQL_SaveDate(Date.Now))
                StrSQL.AppendLine("         ,Inviato = -1 ")
                StrSQL.AppendLine(" WHERE Allegati_Documenti_SuperUser = '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "' ")
                StrSQL.AppendLine("   AND    Inviato >= 0 ")
            Else

                StrSQL.Length = 0

                StrSQL.AppendLine(" DELETE ")
                StrSQL.AppendLine(" FROM     Allegati_EntitaxDocumenti ")
                'StrSQL.AppendLine(" WHERE    Allegati_Documenti_SuperUser = '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "' ")
                StrSQL.AppendLine(" WHERE    1=1 ")
                StrSQL.AppendLine(" AND      Inviato >= 0 ")

            End If

            StrSQL.AppendLine(" AND piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.AppendLine(" AND sa_cod = " & Agro_SQL_SaveNum(sa_cod))
            StrSQL.AppendLine(" AND fabbricato_cod = " & Agro_SQL_SaveNum(fabbricato_cod))

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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

    '#########################################################################
    Public Function Cancella_Analisi( _
                            ByVal Analisi_Testata_Cod As Integer, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Allegati_EntitaxDocumenti_W.Cancella_Analisi()"

        '====================================================================================
        'Parametri opzionali :
        '   Sa_Cod = 0           =>  si cancellano tutti gli appezzamenti dell'impresa
        '   Appezza = 0          =>  si cancellano tutti gli appezzamenti del centro aziendale

        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False
        If Analisi_Testata_Cod = 0 Then
            Throw New Exception(" errore analisi_testata_cod = 0 ")
        End If


        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0

                StrSQL.Append(" UPDATE  Allegati_EntitaxDocumenti ")
                StrSQL.Append(" SET ")
                StrSQL.Append("          Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         ,Data_Modifica = " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE Allegati_Documenti_SuperUser = '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "' ")
                StrSQL.Append("   AND    Inviato >= 0 ")
            Else

                StrSQL.Length = 0

                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     Allegati_EntitaxDocumenti ")
                StrSQL.Append(" WHERE    Allegati_Documenti_SuperUser = '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "' ")
                StrSQL.Append(" AND      Inviato >= 0 ")

            End If

            StrSQL.Append(" AND Analisi_Testata_Cod = " & Agro_SQL_SaveNum(Analisi_Testata_Cod) & " ")


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


    '#########################################################################
    Public Function Cancella_x_Documento( _
                            ByVal Allegati_Documenti_Cod As Integer, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Allegati_EntitaxDocumenti_W.Cancella_x_Documento()"

        '====================================================================================
        'Parametri opzionali :
        '   Sa_Cod = 0           =>  si cancellano tutti gli appezzamenti dell'impresa
        '   Appezza = 0          =>  si cancellano tutti gli appezzamenti del centro aziendale

        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0

                StrSQL.Append(" UPDATE  Allegati_EntitaxDocumenti ")
                StrSQL.Append(" SET ")
                StrSQL.Append("          Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         ,Data_Modifica = " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE Allegati_Documenti_SuperUser = '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "' ")
                StrSQL.Append("   AND    Inviato >= 0 ")
            Else

                StrSQL.Length = 0

                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     Allegati_EntitaxDocumenti ")
                StrSQL.Append(" WHERE    Allegati_Documenti_SuperUser = '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "' ")
                StrSQL.Append(" AND      Inviato >= 0 ")

            End If

            StrSQL.Append(" AND Allegati_Documenti_Cod = " & Agro_SQL_SaveNum(Allegati_Documenti_Cod) & " ")


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



    '#########################################################################
    Public Function Cancella_x_Piva( _
                            ByVal Piva As String, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Allegati_EntitaxDocumenti_W.Cancella_x_Piva()"

        '====================================================================================
        'Parametri opzionali :
        '   Sa_Cod = 0           =>  si cancellano tutti gli appezzamenti dell'impresa
        '   Appezza = 0          =>  si cancellano tutti gli appezzamenti del centro aziendale

        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0

                StrSQL.Append(" UPDATE  Allegati_EntitaxDocumenti ")
                StrSQL.Append(" SET ")
                StrSQL.Append("          Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         ,Data_Modifica = " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE Allegati_Documenti_SuperUser = '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "' ")
                StrSQL.Append("   AND    Inviato >= 0 ")
            Else

                StrSQL.Length = 0

                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     Allegati_EntitaxDocumenti ")
                StrSQL.Append(" WHERE    Allegati_Documenti_SuperUser = '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "' ")
                StrSQL.Append(" AND      Inviato >= 0 ")

            End If

            StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")


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

    '#########################################################################
    Public Function Cancella_Programmazione_Entita_cod(
                            ByVal Programmazione_Entita_cod As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Allegati_EntitaxDocumenti_W.Cancella_Analisi()"

        '====================================================================================
        'Parametri opzionali :
        '   Sa_Cod = 0           =>  si cancellano tutti gli appezzamenti dell'impresa
        '   Appezza = 0          =>  si cancellano tutti gli appezzamenti del centro aziendale

        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0

                StrSQL.Append(" UPDATE  Allegati_EntitaxDocumenti ")
                StrSQL.Append(" SET ")
                StrSQL.Append("          Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         ,Data_Modifica = " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE Allegati_Documenti_SuperUser = '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "' ")
                StrSQL.Append("   AND    Inviato >= 0 ")
            Else

                StrSQL.Length = 0

                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     Allegati_EntitaxDocumenti ")
                'StrSQL.Append(" WHERE    Allegati_Documenti_SuperUser = '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "' ")
                StrSQL.Append(" WHERE    1=1 ")
                StrSQL.Append(" AND      Inviato >= 0 ")

            End If

            StrSQL.Append(" AND Programmazione_Entita_cod = " & Agro_SQL_SaveNum(Programmazione_Entita_cod) & " ")


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

'#######################################################################
'#######################################################################
'#######################################################################

Public Class Allegati_EntitaxDocumenti_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Leggi_Analisi(ByVal Analisi_Testata_Cod As Integer, _
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Allegati_EntitaxDocumenti_R.Leggi_Analisi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta
                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT    Allegati_Documenti.Allegati_Documenti_Cod, Allegati_EntitaxDocumenti.Analisi_Testata_Cod, Allegati_Documenti.Allegati_Documenti_Des, Allegati_Documenti.Allegati_Documenti_NomeFile ")
                    StrSQL.Append(" FROM        Allegati_EntitaxDocumenti INNER JOIN ")
                    StrSQL.Append("             Allegati_Documenti ON Allegati_EntitaxDocumenti.Allegati_Documenti_SuperUser = Allegati_Documenti.Allegati_Documenti_SuperUser AND  ")
                    StrSQL.Append("             Allegati_EntitaxDocumenti.Allegati_Documenti_Cod = Allegati_Documenti.Allegati_Documenti_Cod ")
                    StrSQL.Append(" WHERE  Allegati_EntitaxDocumenti.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND    Allegati_EntitaxDocumenti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND    Allegati_EntitaxDocumenti.Allegati_Documenti_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")

                    If Analisi_Testata_Cod <> 0 Then
                        StrSQL.Append(" AND Analisi_Testata_Cod = " & Agro_SQL_SaveNum(Analisi_Testata_Cod) & " ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Allegati_EntitaxDocumenti.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Allegati_EntitaxDocumenti.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
                    '
                    '
                    '
                    '

            End Select

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






    '##############################################################################################
    Public Function Leggi_x_PDC(ByVal Allegati_Documenti_Cod As String, _
                                ByVal Allegati_Entita_Cod As Integer, _
                                ByVal ID_PDC_Testata As Integer, _
                                ByVal ID_PDC_Dettagli As Integer, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Allegati_EntitaxDocumenti_R.Leggi_Analisi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            
            StrSQL.Length = 0
            StrSQL.Append(" SELECT    Allegati_Documenti.Allegati_Documenti_Cod, Allegati_EntitaxDocumenti.Analisi_Testata_Cod, Allegati_Documenti.Allegati_Documenti_Des, Allegati_Documenti.Allegati_Documenti_NomeFile ")
            StrSQL.Append("         , Allegati_EntitaxDocumenti.ID_PDC_Testata, Allegati_EntitaxDocumenti.ID_PDC_Dettagli ")


            StrSQL.Append(" FROM         Allegati_EntitaxDocumenti INNER JOIN ")
            StrSQL.Append("       Allegati_Documenti ON Allegati_EntitaxDocumenti.Allegati_Documenti_SuperUser = Allegati_Documenti.Allegati_Documenti_SuperUser AND ")
            StrSQL.Append("       Allegati_EntitaxDocumenti.Allegati_Documenti_Cod = Allegati_Documenti.Allegati_Documenti_Cod ")
            StrSQL.Append(" WHERE  1=1 ")

            If Allegati_Documenti_Cod <> 0 Then
                StrSQL.Append(" AND    Allegati_EntitaxDocumenti.Allegati_Documenti_Cod = " & Agro_SQL_SaveNum(Allegati_Documenti_Cod) & " ")
            End If
            If Allegati_Entita_Cod <> 0 Then
                StrSQL.Append(" AND    Allegati_EntitaxDocumenti.Allegati_Entita_Cod = " & Agro_SQL_SaveNum(Allegati_Entita_Cod) & " ")
            End If

            If ID_PDC_Testata <> 0 Then
                StrSQL.Append(" AND    Allegati_EntitaxDocumenti.ID_PDC_Testata = " & Agro_SQL_SaveNum(ID_PDC_Testata) & " ")
            End If

            If ID_PDC_Dettagli <> 0 Then
                StrSQL.Append(" AND    Allegati_EntitaxDocumenti.ID_PDC_Dettagli = " & Agro_SQL_SaveNum(ID_PDC_Dettagli) & " ")
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



    '##############################################################################################
    Public Function Leggi(ByVal Allegati_Documenti_Cod As Integer,
                          ByVal Piva As String,
                          ByVal Sa_Cod As Integer,
                          ByVal Campo_Cod As Integer,
                          ByVal Appezza As Integer,
                          ByVal Id_Imp As Integer,
                          ByVal Fabbricato_Cod As Integer,
                          ByVal Prov As String,
                          ByVal Com As String,
                          ByVal Sezione As String,
                          ByVal Foglio As Integer,
                          ByVal Numero As Integer,
                          ByVal Subalterno As String,
                          ByVal ID_Oggetto_Grafico As String,
                          ByVal Chiave_Albero_Imprese As String,
                          ByVal Analisi_Testata_Cod As Integer,
                          ByVal Programmazione_Cod As Integer,
                          ByVal Programmazione_Entita_Cod As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Allegati_EntitaxDocumenti_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT Allegati_EntitaxDocumenti.*, Allegati_Documenti.Allegati_Documenti_Numero ")

            StrSQL.Append(" FROM         Allegati_EntitaxDocumenti INNER JOIN ")
            StrSQL.Append("       Allegati_Documenti ON Allegati_EntitaxDocumenti.Allegati_Documenti_SuperUser = Allegati_Documenti.Allegati_Documenti_SuperUser AND ")
            StrSQL.Append("       Allegati_EntitaxDocumenti.Allegati_Documenti_Cod = Allegati_Documenti.Allegati_Documenti_Cod ")
            StrSQL.Append(" WHERE  Allegati_EntitaxDocumenti.Allegati_Documenti_SuperUser='" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")

            If Allegati_Documenti_Cod <> 0 Then
                StrSQL.Append(" AND    Allegati_EntitaxDocumenti.Allegati_Documenti_Cod = " & Agro_SQL_SaveNum(Allegati_Documenti_Cod) & " ")
            End If

            If Piva <> "" Then
                StrSQL.Append(" AND    Allegati_EntitaxDocumenti.piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If
            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND    Allegati_EntitaxDocumenti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If
            If Campo_Cod <> 0 Then
                StrSQL.Append(" AND    Allegati_EntitaxDocumenti.Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod) & " ")
            End If
            If Appezza <> 0 Then
                StrSQL.Append(" AND    Allegati_EntitaxDocumenti.Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If
            If Id_Imp <> 0 Then
                StrSQL.Append(" AND    Allegati_EntitaxDocumenti.Id_Imp = " & Agro_SQL_SaveNum(Id_Imp) & " ")
            End If
            If Fabbricato_Cod <> 0 Then
                StrSQL.Append(" AND    Allegati_EntitaxDocumenti.Fabbricato_Cod = " & Agro_SQL_SaveNum(Fabbricato_Cod) & " ")
            End If
            If Prov <> "" Then
                StrSQL.Append(" AND    Allegati_EntitaxDocumenti.Prov = '" & Agro_SQL_SaveText(Prov) & "' ")
            End If
            If Com <> "" Then
                StrSQL.Append(" AND    Allegati_EntitaxDocumenti.com = '" & Agro_SQL_SaveText(Com) & "' ")
            End If
            If Sezione <> "" Then
                StrSQL.Append(" AND    Allegati_EntitaxDocumenti.Sezione = '" & Agro_SQL_SaveText(Sezione) & "' ")
            End If
            If Foglio <> 0 Then
                StrSQL.Append(" AND    Allegati_EntitaxDocumenti.Foglio = " & Agro_SQL_SaveNum(Foglio) & " ")
            End If
            If Numero <> 0 Then
                StrSQL.Append(" AND    Allegati_EntitaxDocumenti.Numero = " & Agro_SQL_SaveNum(Numero) & " ")
            End If
            If Subalterno <> "" Then
                StrSQL.Append(" AND    Allegati_EntitaxDocumenti.Subalterno = '" & Agro_SQL_SaveText(Subalterno) & "' ")
            End If
            If ID_Oggetto_Grafico <> "" Then
                StrSQL.Append(" AND    Allegati_EntitaxDocumenti.ID_Oggetto_Grafico = '" & Agro_SQL_SaveText(ID_Oggetto_Grafico) & "' ")
            End If
            If Chiave_Albero_Imprese <> "" Then
                StrSQL.Append(" AND    Allegati_EntitaxDocumenti.Chiave_Albero_Imprese = '" & Agro_SQL_SaveText(Chiave_Albero_Imprese) & "' ")
            End If
            If Analisi_Testata_Cod <> 0 Then
                StrSQL.Append(" AND    Allegati_EntitaxDocumenti.Analisi_Testata_Cod = " & Agro_SQL_SaveNum(Analisi_Testata_Cod) & " ")
            End If

            If Programmazione_Cod <> 0 Then
                StrSQL.Append(" AND    Allegati_EntitaxDocumenti.Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod) & " ")
            End If
            If Programmazione_Entita_Cod <> 0 Then
                StrSQL.Append(" AND    Allegati_EntitaxDocumenti.Programmazione_Entita_Cod = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod) & " ")
            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Allegati_EntitaxDocumenti.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Allegati_EntitaxDocumenti.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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



    '##############################################################################################
    Public Function Leggi_con_Ente(ByVal Allegati_Documenti_Cod As Integer, _
                          ByVal Piva As String, _
                          ByVal Sa_Cod As Integer, _
                          ByVal Campo_Cod As Integer, _
                          ByVal Appezza As Integer, _
                          ByVal Id_Imp As Integer, _
                          ByVal Fabbricato_Cod As Integer, _
                          ByVal Prov As String, _
                          ByVal Com As String, _
                          ByVal Sezione As String, _
                          ByVal Foglio As Integer, _
                          ByVal Numero As Integer, _
                          ByVal Subalterno As String, _
                          ByVal ID_Oggetto_Grafico As String, _
                          ByVal Chiave_Albero_Imprese As String, _
                          ByVal Analisi_Testata_Cod As Integer, _
                          ByVal Programmazione_Cod As Integer, _
                          ByVal Programmazione_Entita_Cod As Integer, _
                          ByVal xFiltroAggiuntivo As String, _
                          ByVal xOrderBy As String, _
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                          ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Allegati_EntitaxDocumenti_R.Leggi_con_Ente()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT Allegati_EntitaxDocumenti.*, Allegati_Documenti.*, EnteTecnico.Ente_Cod, EnteTecnico.ente_codifica ")

            StrSQL.Append(" FROM         Allegati_EntitaxDocumenti INNER JOIN ")
            StrSQL.Append("       Allegati_Documenti ON Allegati_EntitaxDocumenti.Allegati_Documenti_SuperUser = Allegati_Documenti.Allegati_Documenti_SuperUser AND ")
            StrSQL.Append("       Allegati_EntitaxDocumenti.Allegati_Documenti_Cod = Allegati_Documenti.Allegati_Documenti_Cod LEFT JOIN EnteTecnico ON Allegati_Documenti.Allegati_Documenti_Ente_Cod=EnteTecnico.Ente_Cod ")
            StrSQL.Append(" WHERE  Allegati_EntitaxDocumenti.Allegati_Documenti_SuperUser='" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")

            If Allegati_Documenti_Cod <> 0 Then
                StrSQL.Append(" AND    Allegati_EntitaxDocumenti.Allegati_Documenti_Cod = " & Agro_SQL_SaveNum(Allegati_Documenti_Cod) & " ")
            End If

            If Piva <> "" Then
                StrSQL.Append(" AND    Allegati_EntitaxDocumenti.piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If
            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND    Allegati_EntitaxDocumenti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If
            If Campo_Cod <> 0 Then
                StrSQL.Append(" AND    Allegati_EntitaxDocumenti.Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod) & " ")
            End If
            If Appezza <> 0 Then
                StrSQL.Append(" AND    Allegati_EntitaxDocumenti.Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If
            If Id_Imp <> 0 Then
                StrSQL.Append(" AND    Allegati_EntitaxDocumenti.Id_Imp = " & Agro_SQL_SaveNum(Id_Imp) & " ")
            End If
            If Fabbricato_Cod <> 0 Then
                StrSQL.Append(" AND    Allegati_EntitaxDocumenti.Fabbricato_Cod = " & Agro_SQL_SaveNum(Fabbricato_Cod) & " ")
            End If
            If Prov <> "" Then
                StrSQL.Append(" AND    Allegati_EntitaxDocumenti.Prov = '" & Agro_SQL_SaveText(Prov) & "' ")
            End If
            If Com <> "" Then
                StrSQL.Append(" AND    Allegati_EntitaxDocumenti.com = '" & Agro_SQL_SaveText(Com) & "' ")
            End If
            If Sezione <> "" Then
                StrSQL.Append(" AND    Allegati_EntitaxDocumenti.Sezione = '" & Agro_SQL_SaveText(Sezione) & "' ")
            End If
            If Foglio <> 0 Then
                StrSQL.Append(" AND    Allegati_EntitaxDocumenti.Foglio = " & Agro_SQL_SaveNum(Foglio) & " ")
            End If
            If Numero <> 0 Then
                StrSQL.Append(" AND    Allegati_EntitaxDocumenti.Numero = " & Agro_SQL_SaveNum(Numero) & " ")
            End If
            If Subalterno <> "" Then
                StrSQL.Append(" AND    Allegati_EntitaxDocumenti.Subalterno = '" & Agro_SQL_SaveText(Subalterno) & "' ")
            End If
            If ID_Oggetto_Grafico <> "" Then
                StrSQL.Append(" AND    Allegati_EntitaxDocumenti.ID_Oggetto_Grafico = '" & Agro_SQL_SaveText(ID_Oggetto_Grafico) & "' ")
            End If
            If Chiave_Albero_Imprese <> "" Then
                StrSQL.Append(" AND    Allegati_EntitaxDocumenti.Chiave_Albero_Imprese = '" & Agro_SQL_SaveText(Chiave_Albero_Imprese) & "' ")
            End If
            If Analisi_Testata_Cod <> 0 Then
                StrSQL.Append(" AND    Allegati_EntitaxDocumenti.Analisi_Testata_Cod = " & Agro_SQL_SaveNum(Analisi_Testata_Cod) & " ")
            End If

            If Programmazione_Cod <> 0 Then
                StrSQL.Append(" AND    Allegati_EntitaxDocumenti.Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod) & " ")
            End If
            If Programmazione_Entita_Cod <> 0 Then
                StrSQL.Append(" AND    Allegati_EntitaxDocumenti.Programmazione_Entita_Cod = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod) & " ")
            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Allegati_EntitaxDocumenti.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Allegati_EntitaxDocumenti.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    ' legge gli allegati entita
    '##############################################################################################
    Public Function Leggi_Allegati_Entita(ByVal Piva As String,
                                            ByVal Cod_Contatto As String,
                                            ByVal Analisi_Testata_Cod As Integer,
                                            ByVal ID_Elenco As Integer,
                                            ByVal ID_Alert_Entita As Integer,
                                            ByVal TipoEntita_Cod As Integer,
                                            ByVal TipoDocumento_Cod As Integer,
                                            ByVal Allegati_Documenti_Cod As Integer,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Allegati_EntitaxDocumenti_R.Leggi_Allegati_Contatto()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.AppendLine(" SELECT Alert_Entita.ID_Alert_Entita, Alert_Entita.PivaSuperUser, Alert_Entita.Piva, Alert_Entita.Cod_Contatto, Alert_Entita.TipoEntita_Cod, ")
            StrSQL.AppendLine("   Allegati_Documenti.Allegati_Documenti_Cod, Allegati_Documenti.Allegati_Documenti_Numero, Allegati_Documenti.Allegati_Documenti_Des, ")
            StrSQL.AppendLine("   Allegati_Documenti.Sottocartella, Allegati_Documenti.Allegati_Documenti_NomeFile, Allegati_Documenti.Allegati_Documenti_Estensione, Allegati_Documenti.Allegati_Documenti_CatCod, ")
            StrSQL.AppendLine("   Allegati_Documenti.Allegati_Documenti_Ente_Cod, Allegati_Documenti.Allegati_Documenti_Ente_Des, Allegati_Documenti.Validazione_Data, ")
            StrSQL.AppendLine("   Alert_Elenco.ID_Elenco, Alert_Elenco.Data_Scadenza, Alert_Elenco.Descrizione_Scadenza, Alert_Elenco.ID_Tipologia, Alert_Tipologia.Nome AS Tipologia, ")
            StrSQL.AppendLine("   Alert_Tipologia.ID_Area, Alert_Area.Nome AS Area, ")

            StrSQL.AppendLine(" Allegati_Documenti.Validazione_Flag, Allegati_Documenti.UserName_Upload, Allegati_Documenti.Data_Upload, Alert_Elenco.Note ")

            If ID_Elenco <> 0 Then
                StrSQL.AppendLine(" , File_Allegato_DB ")
            End If

            StrSQL.AppendLine(" FROM Alert_Entita (nolock) ")
            StrSQL.AppendLine(" INNER JOIN Allegati_Documenti (nolock) ON Alert_Entita.PivaSuperUser = Allegati_Documenti.Allegati_Documenti_SuperUser AND Alert_Entita.Allegati_Documenti_Cod = Allegati_Documenti.Allegati_Documenti_Cod ")
            StrSQL.AppendLine(" INNER JOIN Alert_Elenco (nolock) ON Alert_Entita.PivaSuperUser = Alert_Elenco.PivaSuperUser AND Alert_Entita.ID_Alert_Entita = Alert_Elenco.ID_Alert_Entita ")
            StrSQL.AppendLine(" LEFT JOIN Alert_Tipologia (nolock) ON Alert_Elenco.ID_Tipologia = Alert_Tipologia.ID_Tipologia ")
            StrSQL.AppendLine(" LEFT JOIN Alert_Area (nolock) ON Alert_Tipologia.ID_Area = Alert_Area.ID_Area ")
            StrSQL.AppendLine(" WHERE Alert_Entita.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine("   AND Alert_Entita.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.AppendLine("   AND Alert_Entita.PivaSuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")

            If Not String.IsNullOrEmpty(Piva) Then
                StrSQL.AppendLine(" AND Alert_Entita.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Not String.IsNullOrEmpty(Cod_Contatto) Then
                StrSQL.AppendLine(" AND Alert_Entita.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto.Trim()) & "' ")
            End If

            If Analisi_Testata_Cod <> 0 Then
                StrSQL.Append(" AND Alert_Entita.Analisi_Testata_Cod = " & Agro_SQL_SaveNum(Analisi_Testata_Cod) & " ")
            End If

            If ID_Elenco <> 0 Then
                StrSQL.AppendLine(" AND Alert_Elenco.ID_Elenco = " & Agro_SQL_SaveNum(ID_Elenco) & " ")
            End If

            If ID_Alert_Entita <> 0 Then
                StrSQL.AppendLine(" AND Alert_Entita.ID_Alert_Entita = " & Agro_SQL_SaveNum(ID_Alert_Entita) & " ")
            End If

            If TipoEntita_Cod <> 0 Then
                StrSQL.AppendLine(" AND Alert_Entita.TipoEntita_Cod = " & Agro_SQL_SaveNum(TipoEntita_Cod) & " ")
            End If

            If TipoDocumento_Cod <> 0 Then
                StrSQL.AppendLine(" AND Allegati_Documenti.Allegati_Documenti_CatCod = " & Agro_SQL_SaveNum(TipoDocumento_Cod) & " ")
            End If

            If Allegati_Documenti_Cod <> 0 Then
                StrSQL.AppendLine(" AND Alert_Entita.Allegati_Documenti_Cod = " & Agro_SQL_SaveNum(Allegati_Documenti_Cod) & " ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   Alert_Entita.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   Alert_Entita.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select


            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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


    ' legge gli allegati di un'analisi_testata_cod (union per leggere sia vecchia che nuova gestione)
    '##############################################################################################
    Public Function Leggi_AllegatiAnalisi(ByVal Analisi_Testata_Cod As Integer, _
                                          ByVal xFiltroAggiuntivoAllegati As String, _
                                          ByVal xFiltroAggiuntivoAlert As String, _
                                          ByVal xOrderBy As String, _
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                          ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Allegati_EntitaxDocumenti_R.Leggi_Analisi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
          
            StrSQL.Length = 0
            StrSQL.Append(" SELECT    Allegati_Documenti.Allegati_Documenti_Cod, Allegati_EntitaxDocumenti.Analisi_Testata_Cod, Allegati_Documenti.Allegati_Documenti_Des, Allegati_Documenti.Allegati_Documenti_NomeFile ")
            StrSQL.Append(" FROM        Allegati_EntitaxDocumenti INNER JOIN ")
            StrSQL.Append("             Allegati_Documenti ON Allegati_EntitaxDocumenti.Allegati_Documenti_SuperUser = Allegati_Documenti.Allegati_Documenti_SuperUser AND  ")
            StrSQL.Append("             Allegati_EntitaxDocumenti.Allegati_Documenti_Cod = Allegati_Documenti.Allegati_Documenti_Cod ")
            StrSQL.Append(" WHERE  Allegati_EntitaxDocumenti.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND    Allegati_EntitaxDocumenti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.Append(" AND    Allegati_EntitaxDocumenti.Allegati_Documenti_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")

            If Analisi_Testata_Cod <> 0 Then
                StrSQL.Append(" AND Analisi_Testata_Cod = " & Agro_SQL_SaveNum(Analisi_Testata_Cod) & " ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivoAllegati <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivoAllegati, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Allegati_EntitaxDocumenti.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Allegati_EntitaxDocumenti.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------

            StrSQL.Append(" UNION ")

            StrSQL.Append(" SELECT  Allegati_Documenti.Allegati_Documenti_Cod, Alert_Entita.Analisi_Testata_Cod, Allegati_Documenti.Allegati_Documenti_Des, Allegati_Documenti.Allegati_Documenti_NomeFile ")
            StrSQL.Append(" FROM    Alert_Entita INNER JOIN  ")
            StrSQL.Append("         Allegati_Documenti ON Alert_Entita.PivaSuperUser = Allegati_Documenti.Allegati_Documenti_SuperUser AND ")
            StrSQL.Append("         Alert_Entita.Allegati_Documenti_Cod = Allegati_Documenti.Allegati_Documenti_Cod ")
            StrSQL.Append(" WHERE   Alert_Entita.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND     Alert_Entita.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.Append(" AND     Alert_Entita.PivaSuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")

            If Analisi_Testata_Cod <> 0 Then
                StrSQL.Append(" AND Analisi_Testata_Cod = " & Analisi_Testata_Cod & " ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivoAlert <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivoAlert, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Alert_Entita.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Alert_Entita.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select


            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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


    Public Function Leggi_Planning(ByVal Piva As String, ByVal xFiltroAggiuntivo As String, ByVal xOrderBy As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Allegati_EntitaxDocumenti_R.Leggi_Planning_Precedente_Da_Validazione_Data()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT Programmazione_Cod,Validazione_Data, Allegati_Documenti_Numero ")

            StrSQL.Append(" FROM   Allegati_Documenti INNER JOIN ")
            StrSQL.Append("        Allegati_EntitaxDocumenti ON Allegati_Documenti.Allegati_Documenti_SuperUser = Allegati_EntitaxDocumenti.Allegati_Documenti_SuperUser AND  ")
            StrSQL.Append("        Allegati_Documenti.Allegati_Documenti_Cod = Allegati_EntitaxDocumenti.Allegati_Documenti_Cod ")
            StrSQL.Append(" WHERE  Allegati_EntitaxDocumenti.Allegati_Documenti_SuperUser='" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
            StrSQL.Append(" AND    Programmazione_Cod<>0 AND Programmazione_Entita_Cod=0 ")
            StrSQL.Append(" AND    Allegati_Documenti_CatCod=9 ")

            If Piva <> "" Then
                StrSQL.Append(" AND   Allegati_Documenti.Allegati_Documenti_Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Allegati_EntitaxDocumenti.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Allegati_EntitaxDocumenti.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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