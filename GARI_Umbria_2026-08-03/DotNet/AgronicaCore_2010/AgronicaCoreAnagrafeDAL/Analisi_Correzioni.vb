Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.DataProviderExtensions

Public Class Analisi_Correzioni_Testate_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '#########################################################################
    Public Function Scrivi(ByVal piva As String,
                           ByVal sa_cod As Integer,
                           ByVal id_correzione_testata As Integer,
                           ByVal nome As String,
                           ByVal analisi_parametro_cod As Integer,
                           ByVal scala_definizione As String,
                           ByVal scala_inizio As String,
                           ByVal scala_fine As String,
                           ByVal note As String,
                           ByVal validita_inizio As DateTime,
                           ByVal validita_fine As DateTime,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                           , Optional ByVal Data_creazione As DateTime = #2/1/1900# _
                           , Optional ByVal Data_modifica As DateTime = #2/1/1900# _
                           , Optional ByVal username_creazione As String = "" _
                           , Optional ByVal username_modifica As String = ""
                           ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Analisi_Correzioni_Testate_W.Scrivi()"

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



            '---------------------------------------------
            StrSQL.Length = 0


            StrSQL.Append("INSERT INTO Analisi_Correzioni_Testata( ")
            StrSQL.Append("            PivaSuperUser, Piva,  Sa_Cod, ")
            StrSQL.Append("            ID_Correzione_Testata,  Nome,")
            StrSQL.Append("            Analisi_Parametro_Cod,  ")
            StrSQL.Append("            Scala_Definizione, Scala_Inizio, Scala_Fine, Note,  ")
            StrSQL.Append("            Inviato,  ")
            StrSQL.Append("            Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("            UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("            Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("            ) ")


            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(piva) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(sa_cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(id_correzione_testata) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(nome) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(analisi_parametro_cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(scala_definizione) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(scala_inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(scala_fine) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(note) & "'  ")
            StrSQL.Append("         , 0  ")

            StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_creazione))
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(validita_inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(validita_fine) & "  ")
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
    Public Function Modifica(ByVal piva As String,
                             ByVal sa_cod As Integer,
                             ByVal id_correzione_testata As Integer,
                             ByVal nome As String,
                             ByVal analisi_parametro_cod As Integer,
                             ByVal scala_definizione As Integer,
                             ByVal scala_inizio As Decimal,
                             ByVal scala_fine As Decimal,
                             ByVal note As String,
                             ByVal validita_inizio As DateTime,
                             ByVal validita_fine As DateTime,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Analisi_Correzione_Testate_W.Modifica()"

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

            StrSQL.Append("UPDATE Analisi_Correzioni_Testata SET ")
            StrSQL.Append("   Nome = '" & Agro_SQL_SaveText(nome) & "'")
            StrSQL.Append("   ,Note = '" & Agro_SQL_SaveText(note) & "'")
            StrSQL.Append("   ,Sa_Cod   =  " & Agro_SQL_SaveNum(sa_cod))
            StrSQL.Append("   ,Analisi_Parametro_Cod   =  " & Agro_SQL_SaveNum(analisi_parametro_cod))
            StrSQL.Append("   ,Scala_Definizione   =  " & Agro_SQL_SaveNum(scala_definizione))
            StrSQL.Append("   ,Scala_Inizio   =  " & Agro_SQL_SaveNum(scala_inizio))
            StrSQL.Append("   ,Scala_Fine   =  " & Agro_SQL_SaveNum(scala_fine))

            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(validita_inizio))
            StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(validita_fine))
            StrSQL.Append(" WHERE PivaSuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")
            StrSQL.Append(" AND   (Piva =  '" & Agro_SQL_SaveText(piva) & "' Or Sa_Cod = -1) ")
            StrSQL.Append(" AND   ID_Correzione_Testata = " & Agro_SQL_SaveNum(id_correzione_testata))

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
    Public Function Cancella(ByVal piva As String,
                             ByVal id_correzione_testata As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Analisi_Correzioni_Testate_W.Cancella()"

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

                StrSQL.Append(" UPDATE  Analisi_Correzioni_Testata ")
                StrSQL.Append(" SET ")
                StrSQL.Append("          Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         ,Data_Modifica = " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE PivaSuperUser = '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "' ")
                StrSQL.Append(" AND   (Piva =  '" & Agro_SQL_SaveText(piva) & "' Or Sa_Cod = -1) ")
                StrSQL.Append(" AND   ID_Correzione_Testata = " & Agro_SQL_SaveNum(id_correzione_testata))
                StrSQL.Append("   AND    Inviato > 0 ")
            Else

                StrSQL.Length = 0

                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     Analisi_Correzioni_Testata ")
                StrSQL.Append(" WHERE    PivaSuperUser = '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "' ")
                StrSQL.Append(" AND   (Piva =  '" & Agro_SQL_SaveText(piva) & "' Or Sa_Cod = -1) ")
                StrSQL.Append(" AND   ID_Correzione_Testata = " & Agro_SQL_SaveNum(id_correzione_testata))
                StrSQL.Append(" AND      Inviato = 0 ")

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

'#######################################################################
'#######################################################################
'#######################################################################

Public Class Analisi_Correzioni_Testate_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Leggi(ByVal Piva As String,
                          ByVal ID_Correzione_Testata As Integer,
                          ByVal Analisi_Parametro_Cod As Integer,
                          ByVal ID_PDC_Testata As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Analisi_Correzioni_Testate_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT DISTINCT  ")
            StrSQL.AppendLine("         Imprese.Rag_Soc, Analisi_Correzioni_Testata.Piva, Analisi_Correzioni_Testata.ID_Correzione_Testata, Analisi_Correzioni_Testata.Sa_Cod ")
            StrSQL.AppendLine("       , Analisi_Correzioni_Testata.Nome, Analisi_Correzioni_Testata.Note, Analisi_Correzioni_Testata.Analisi_Parametro_Cod  ")
            StrSQL.AppendLine("       , Analisi_Correzioni_Testata.Scala_Definizione, Analisi_Correzioni_Testata.Scala_Inizio, Analisi_Correzioni_Testata.Scala_Fine  ")
            StrSQL.AppendLine("       , Analisi_Correzioni_Testata.Data_Creazione, Analisi_Correzioni_Testata.Validita_Inizio, Analisi_Correzioni_Testata.Validita_Fine ")
            StrSQL.AppendLine("       , Analisi_Parametri.Analisi_Parametro_Des, Analisi_Parametri.Analisi_Parametro_Simbolo ")
            StrSQL.AppendLine(" FROM Analisi_Correzioni_Testata ")
            StrSQL.AppendLine(" INNER JOIN Analisi_Parametri ON (Analisi_Correzioni_Testata.Analisi_Parametro_Cod = Analisi_Parametri.Analisi_Parametro_Cod) ")
            StrSQL.AppendLine(" INNER JOIN Imprese ON Analisi_Correzioni_Testata.Piva = Imprese.Piva ")

            If ID_PDC_Testata <> 0 Then
                'Controllo Piva su ID_PDC_Testata
                StrSQL.AppendLine(" INNER JOIN PDC_Testata ON ((PDC_Testata.PivaOwner = Analisi_Correzioni_Testata.Piva AND PDC_Testata.Id_PDC_Testata = " & Agro_SQL_SaveNum(ID_PDC_Testata) & " AND Analisi_Correzioni_Testata.Sa_Cod = 0) OR (PDC_Testata.Id_PDC_Testata = " & Agro_SQL_SaveNum(ID_PDC_Testata) & " AND Analisi_Correzioni_Testata.Sa_Cod = -1)) ")
            End If

            StrSQL.AppendLine(" WHERE Analisi_Correzioni_Testata.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND Analisi_Correzioni_Testata.Validita_Fine>= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.AppendLine(" AND Analisi_Correzioni_Testata.PivaSuperUser= '" & Trim(objParametri.PivaSuperUser) & "'")

            If ID_PDC_Testata <> 0 Then
                StrSQL.AppendLine(" AND PDC_Testata.PDC_Data_Istantanea >= Analisi_Correzioni_Testata.Validita_Inizio ")
                StrSQL.AppendLine(" AND PDC_Testata.PDC_Data_Istantanea <= Analisi_Correzioni_Testata.Validita_Fine ")
            End If

            If Trim(Piva) <> "" Then
                StrSQL.AppendLine(" AND (Analisi_Correzioni_Testata.Piva = '" & Agro_SQL_SaveText(Piva) & "' OR Analisi_Correzioni_Testata.Sa_Cod = -1) ")
            End If

            If ID_Correzione_Testata <> 0 Then
                StrSQL.AppendLine(" AND Analisi_Correzioni_Testata.ID_Correzione_Testata = " & ID_Correzione_Testata & " ")
            End If

            If Analisi_Parametro_Cod <> 0 Then
                StrSQL.AppendLine(" AND Analisi_Correzioni_Testata.Analisi_Parametro_Cod = " & Analisi_Parametro_Cod & " ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   Analisi_Correzioni_Testata.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   Analisi_Correzioni_Testata.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Nome, Analisi_Parametro_Des ")
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
    Public Function Leggi_Parametri_PDC(ByVal Id_PDC_Testata As Integer,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        Optional ByVal xFiltroAggiuntivo As String = ""
                                        ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Analisi_Correzioni_Testate_R.Leggi_Parametri_PDC()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" WITH AnalisiParametriTestata as ( ")
            StrSQL.Append(" SELECT DISTINCT PDC_Testata.PivaSuperUser, PDC_Testata.PivaOwner, Analisi_Dettagli.Analisi_Parametro_Cod ")
            StrSQL.Append(" FROM Analisi_Dettagli  ")
            StrSQL.Append(" Inner Join PDC_Analisi On Analisi_Dettagli.Analisi_Testata_Cod = PDC_Analisi.Analisi_Testata_Cod ")
            StrSQL.Append(" Inner Join PDC_Testata On PDC_Analisi.ID_PDC_Testata = PDC_Testata.ID_PDC_Testata  ")
            StrSQL.Append(" Where PDC_Testata.Id_PDC_Testata = " & Id_PDC_Testata & ")")

            StrSQL.Append(" select  Distinct ")
            StrSQL.Append(" Analisi_Parametri.Analisi_Parametro_Cod, Analisi_Parametri.Analisi_Parametro_Des, Analisi_Parametri.Analisi_Parametro_Simbolo  ")
            StrSQL.Append(" from  Analisi_Parametri ")
            StrSQL.Append(" Inner Join AnalisiParametriTestata ON Analisi_Parametri.Analisi_Parametro_Cod = AnalisiParametriTestata.Analisi_Parametro_Cod ")
            StrSQL.Append(" Inner Join Analisi_Correzioni_Testata On Analisi_Correzioni_Testata.Analisi_Parametro_Cod = Analisi_Parametri.Analisi_Parametro_Cod ")
            StrSQL.Append("     And Analisi_Correzioni_Testata.PivaSuperUser = AnalisiParametriTestata.PivaSuperUser ")
            StrSQL.Append("      And (Analisi_Correzioni_Testata.Piva = AnalisiParametriTestata.PivaOwner Or Analisi_Correzioni_Testata.Sa_Cod = -1) ")
            StrSQL.Append("      And  Analisi_Correzioni_Testata.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append("      And  Analisi_Correzioni_Testata.Validita_Fine>= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.Append("      And  Analisi_Correzioni_Testata.Inviato >=0 ")


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" And " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If


            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Analisi_Parametro_Des ")
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


'#######################################################################
'#######################################################################
'#######################################################################
'#######################################################################
'#######################################################################
'#######################################################################
'#######################################################################
'#######################################################################
'#######################################################################

Public Class Analisi_Correzioni_Dettagli_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '#########################################################################
    Public Function Scrivi(ByVal piva As String,
                           ByVal id_correzione_testata As Integer,
                           ByVal valore_sangue As Decimal,
                           ByVal valore_correzione As Decimal,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            , Optional ByVal Data_creazione As DateTime = #2/1/1900# _
                            , Optional ByVal Data_modifica As DateTime = #2/1/1900# _
                            , Optional ByVal username_creazione As String = "" _
                            , Optional ByVal username_modifica As String = ""
                           ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Analisi_Correzioni_Dettagli_W.Scrivi()"

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



            '---------------------------------------------
            StrSQL.Length = 0


            StrSQL.Append("INSERT INTO Analisi_Correzioni_Dettagli( ")
            StrSQL.Append("            PivaSuperUser, Piva, ")
            StrSQL.Append("            ID_Correzione_Testata, valore_sangue, valore_correzione, ")
            StrSQL.Append("            Inviato,  ")
            StrSQL.Append("            Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("            UserName_Creazione, UserName_Modifica ")
            StrSQL.Append("            ) ")

            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(piva) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(id_correzione_testata) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(valore_sangue) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(valore_correzione) & " ")

            StrSQL.Append("         , 0  ")

            StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_creazione))
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(username_modifica) & "' ")

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
    Public Function Cancella(ByVal piva As String,
                             ByVal id_correzione_testata As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Analisi_Correzioni_Dettagli_W.Cancella()"

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

                StrSQL.Append(" UPDATE  Analisi_Correzioni_Dettagli ")
                StrSQL.Append(" SET ")
                StrSQL.Append("          Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         ,Data_Modifica = " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE PivaSuperUser = '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "' ")
                StrSQL.Append(" AND   Piva =  '" & Agro_SQL_SaveText(piva) & "' ")
                StrSQL.Append(" And   ID_Correzione_Testata = " & Agro_SQL_SaveNum(id_correzione_testata))
                StrSQL.Append(" And    Inviato > 0 ")
            Else

                StrSQL.Length = 0

                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     Analisi_Correzioni_Dettagli ")
                StrSQL.Append(" WHERE    PivaSuperUser = '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "' ")
                StrSQL.Append(" AND   Piva =  '" & Agro_SQL_SaveText(piva) & "' ")
                StrSQL.Append(" And   ID_Correzione_Testata = " & Agro_SQL_SaveNum(id_correzione_testata))
                StrSQL.Append(" And      Inviato = 0 ")

            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" And " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] :  " & MessaggioErrore)
        End Try

        Return xRisp

    End Function




End Class

'#######################################################################
'#######################################################################
'#######################################################################

Public Class Analisi_Correzioni_Dettagli_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Leggi(ByVal Piva As String,
                          ByVal Id_Correzione_Testata As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Analisi_Correzione_Dettagli_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM   Analisi_Correzioni_Dettagli ")
            StrSQL.Append(" Where  PivaSuperUser= '" & Trim(objParametri.PivaSuperUser) & "'")

            If Trim(Piva) <> "" Then
                StrSQL.Append(" And    Piva= '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If Id_Correzione_Testata <> 0 Then
                StrSQL.Append(" AND Id_Correzione_Testata = " & Id_Correzione_Testata & " ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Valore_Sangue ")
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
