Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class PS_Piano_Date_Raccolte_Semina_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '#########################################################################
    Public Function Scrivi( _
                                ByVal ID_PianoSeminaTestata As Integer, _
                                ByVal ID_Piano_Date_Raccolte_Semina As Integer, _
                                ByVal Cul_Cod As Integer, _
                                ByVal HA As Decimal, _
                                ByVal Nettissimo As Decimal, _
                                ByVal Raccolta_1 As Date, _
                                ByVal Lock_Raccolta_1 As Integer, _
                                ByVal Semina As Date, _
                                ByVal Lock_Semina As Integer, _
                                ByVal Raccolta_2 As Date, _
                                ByVal Lock_Raccolta_2 As Integer, _
                                    ByVal Validita_Inizio As Date, _
                                    ByVal Validita_Fine As Date, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PS_Piano_Date_Raccolte_Semina_W.Scrivi()"

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

            StrSQL.Append("INSERT INTO PS_Piano_Date_Raccolte_Semina( ")
            StrSQL.Append("            PivaSuperUser,        ID_PianoSeminaTestata,  ID_Piano_Date_Raccolte_Semina,           ")
            StrSQL.Append("            Cul_Cod,    ")
            StrSQL.Append("            Ha, Nettissimo,  ")
            StrSQL.Append("            Raccolta_1, lock_raccolta_1,        ")
            If Not IsNothing(Semina) AndAlso Semina <> New Date Then
                StrSQL.Append("            Semina, lock_semina,        ")
            End If
            If Not IsNothing(Raccolta_2) AndAlso Raccolta_2 <> New Date Then
                StrSQL.Append("            Raccolta_2, lock_raccolta_2,        ")
            End If

            StrSQL.Append("            Inviato, DataInvio, ")
            StrSQL.Append("            Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("            UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("            Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("            ) ")


            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(ID_PianoSeminaTestata) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(ID_Piano_Date_Raccolte_Semina) & "  ")

            StrSQL.Append("         , " & Agro_SQL_SaveNum(Cul_Cod) & " ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(HA) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Nettissimo) & " ")

            StrSQL.Append("         , " & Agro_SQL_SaveDate(Raccolta_1) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Lock_Raccolta_1) & " ")
            If Not IsNothing(Semina) AndAlso Semina <> New Date Then
                StrSQL.Append("         , " & Agro_SQL_SaveDate(Semina) & "  ")
                StrSQL.Append("         , " & Agro_SQL_SaveNum(Lock_Semina) & " ")
            End If
            If Not IsNothing(Raccolta_2) AndAlso Raccolta_2 <> New Date Then
                StrSQL.Append("         , " & Agro_SQL_SaveDate(Raccolta_2) & "  ")
                StrSQL.Append("         , " & Agro_SQL_SaveNum(Lock_Raccolta_2) & " ")
            End If


            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append(")")

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
    Public Function Modifica_Data_Semina( _
                                ByVal ID_PianoSeminaTestata As Integer, _
                                ByVal ID_Piano_Date_Raccolte_Semina As Integer, _
                                 ByVal Semina As Date, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PS_Piano_Date_Raccolte_Semina_W.Modifica_Data_Semina()"

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


            StrSQL.Append("UPDATE PS_Piano_Date_Raccolte_Semina SET ")
            StrSQL.Append("    Semina            = " & Agro_SQL_SaveDate(Semina))

            StrSQL.Append(" WHERE PivaSuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")
            StrSQL.Append(" AND   ID_PianoSeminaTestata = " & ID_PianoSeminaTestata & " ")
            StrSQL.Append(" AND   ID_Piano_Date_Raccolte_Semina = " & ID_Piano_Date_Raccolte_Semina & " ")


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
    Public Function SetNull_Data_Semina( _
                                ByVal ID_PianoSeminaTestata As Integer, _
                                ByVal ID_Piano_Date_Raccolte_Semina As Integer, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PS_Piano_Date_Raccolte_Semina_W.SetNull_Data_Semina()"

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


            StrSQL.Append("UPDATE PS_Piano_Date_Raccolte_Semina SET ")
            StrSQL.Append("    Semina            = NULL ")

            StrSQL.Append(" WHERE PivaSuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")
            StrSQL.Append(" AND   ID_PianoSeminaTestata = " & ID_PianoSeminaTestata & " ")
            StrSQL.Append(" AND   ID_Piano_Date_Raccolte_Semina = " & ID_Piano_Date_Raccolte_Semina & " ")


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
    Public Function Modifica_Data_Raccolta_1( _
                                ByVal ID_PianoSeminaTestata As Integer, _
                                ByVal ID_Piano_Date_Raccolte_Semina As Integer, _
                                 ByVal raccolta_1 As Date, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PS_Piano_Date_Raccolte_Semina_W.Modifica_Data_Raccolta_1()"

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


            StrSQL.Append("UPDATE PS_Piano_Date_Raccolte_Semina SET ")
            StrSQL.Append("    raccolta_1            = " & Agro_SQL_SaveDate(raccolta_1))

            StrSQL.Append(" WHERE PivaSuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")
            StrSQL.Append(" AND   ID_PianoSeminaTestata = " & ID_PianoSeminaTestata & " ")
            StrSQL.Append(" AND   ID_Piano_Date_Raccolte_Semina = " & ID_Piano_Date_Raccolte_Semina & " ")


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
    Public Function Modifica_Data_Raccolta_2( _
                                ByVal ID_PianoSeminaTestata As Integer, _
                                ByVal ID_Piano_Date_Raccolte_Semina As Integer, _
                                 ByVal raccolta_2 As Date, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PS_Piano_Date_Raccolte_Semina_W.Modifica_Data_Raccolta_1()"

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


            StrSQL.Append("UPDATE PS_Piano_Date_Raccolte_Semina SET ")
            StrSQL.Append("    raccolta_2            = " & Agro_SQL_SaveDate(raccolta_2))

            StrSQL.Append(" WHERE PivaSuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")
            StrSQL.Append(" AND   ID_PianoSeminaTestata = " & ID_PianoSeminaTestata & " ")
            StrSQL.Append(" AND   ID_Piano_Date_Raccolte_Semina = " & ID_Piano_Date_Raccolte_Semina & " ")


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
    Public Function Cancella( _
                            ByVal ID_PianoSeminaTestata As Integer, _
                            ByVal Cul_Cod As Integer, _
                            ByVal Data_Raccolta_1 As Date, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PS_PianoVarieta_HA_W.Cancella()"

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

                StrSQL.Append(" UPDATE  PS_Piano_Date_Raccolte_Semina ")
                StrSQL.Append(" SET ")
                StrSQL.Append("          Username_Modifica = '" & objParametri.UsernameOperazione & "' ")
                StrSQL.Append("         ,Data_Modifica = " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE PivaSuperUser = '" & Trim(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND Inviato >= 0 ")
            Else

                StrSQL.Length = 0

                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     PS_Piano_Date_Raccolte_Semina ")
                StrSQL.Append(" WHERE    PivaSuperUser = '" & Trim(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND Inviato = 0 ")

            End If

            If ID_PianoSeminaTestata <> 0 Then
                StrSQL.Append(" AND PS_Piano_Date_Raccolte_Semina.ID_PianoSeminaTestata = " & ID_PianoSeminaTestata & " ")
            End If
            If Cul_Cod <> 0 Then
                StrSQL.Append(" AND PS_Piano_Date_Raccolte_Semina.Cul_Cod = " & Cul_Cod & " ")
            End If
            If Not IsNothing(Data_Raccolta_1) AndAlso Data_Raccolta_1 <> New Date Then
                StrSQL.Append(" AND PS_Piano_Date_Raccolte_Semina.Data_Raccolta_1 = " & Agro_SQL_SaveDate(Data_Raccolta_1) & " ")
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

'########################################################################
'########################################################################
'########################################################################
'########################################################################
'########################################################################
'########################################################################

Public Class PS_Piano_Date_Raccolte_Semina_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Leggi(ByVal ID_PianoSeminaTestata As Integer, _
                          ByVal Cul_Cod As Integer, _
                          ByVal Data_Raccolta_1 As Date, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PS_Piano_Date_Raccolte_Semina_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM   PS_Piano_Date_Raccolte_Semina ")
            StrSQL.Append(" WHERE  PS_Piano_Date_Raccolte_Semina.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND    PS_Piano_Date_Raccolte_Semina.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.Append(" AND    PS_Piano_Date_Raccolte_Semina.PivaSuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")

            If ID_PianoSeminaTestata <> 0 Then
                StrSQL.Append(" AND PS_Piano_Date_Raccolte_Semina.ID_PianoSeminaTestata = " & ID_PianoSeminaTestata & " ")
            End If

            If Cul_Cod <> 0 Then
                StrSQL.Append(" AND PS_Piano_Date_Raccolte_Semina.Cul_Cod = " & Cul_Cod)
            End If
            If Not IsNothing(Data_Raccolta_1) AndAlso Data_Raccolta_1 <> New Date Then
                StrSQL.Append(" AND PS_Piano_Date_Raccolte_Semina.Data_Raccolta_1 = " & Agro_SQL_SaveDate(Data_Raccolta_1))
            End If


            '-------------------------------------------------------------------------- 
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   PS_Piano_Date_Raccolte_Semina.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   PS_Piano_Date_Raccolte_Semina.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append("   order by raccolta_1 ")
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
    Public Function Somma_SemineConfermate(ByVal ID_PianoSeminaTestata As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PS_Piano_Date_Raccolte_Semina_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT    sum(ha) as Somma, cul_cod from PS_Piano_Date_Raccolte_Semina ")

            StrSQL.Append(" where(lock_semina = -1) ")
            StrSQL.Append(" AND  PS_Piano_Date_Raccolte_Semina.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND    PS_Piano_Date_Raccolte_Semina.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.Append(" AND    PS_Piano_Date_Raccolte_Semina.PivaSuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")

            If ID_PianoSeminaTestata <> 0 Then
                StrSQL.Append(" AND PS_Piano_Date_Raccolte_Semina.ID_PianoSeminaTestata = " & ID_PianoSeminaTestata & " ")
            End If

            '-------------------------------------------------------------------------- 
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   PS_Piano_Date_Raccolte_Semina.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   PS_Piano_Date_Raccolte_Semina.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            StrSQL.Append(" GROUP BY cul_cod ")

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
    Public Function Somma_x_Raccolta_Teorica(ByVal ID_PianoSeminaTestata As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PS_Piano_Date_Raccolte_Semina_R.Somma_x_Raccolta_Teorica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" select SUM(somma) as Somma, Data ")

            StrSQL.Append(" FROM ( ")
            StrSQL.Append(" SELECT    ha  * ( ")
            StrSQL.Append("     (SELECT     top 1 CONVERT( float ,  replace(PS_Zone_Specie_Varieta_Default.Valore, ',','.')) FROM PS_Zone_Specie_Varieta_Default INNER JOIN                      PS_PianoSeminaTestata ON PS_Zone_Specie_Varieta_Default.PivaSuperUser = PS_PianoSeminaTestata.PivaSuperUser AND PS_Zone_Specie_Varieta_Default.Veg_Cod = PS_PianoSeminaTestata.Veg_Cod AND PS_Zone_Specie_Varieta_Default.ID_Zona = PS_PianoSeminaTestata.ID_Zona ")
            StrSQL.Append("         WHERE     (PS_Zone_Specie_Varieta_Default.Codice = 4) AND (Cul_Cod = PS_Piano_Date_Raccolte_Semina.Cul_Cod))) as Somma ")
            StrSQL.Append(" , raccolta_2 as Data ")

            StrSQL.Append(" FROM PS_Piano_Date_Raccolte_Semina ")

            StrSQL.Append(" where(lock_semina = -1) ")
            StrSQL.Append(" AND  PS_Piano_Date_Raccolte_Semina.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND    PS_Piano_Date_Raccolte_Semina.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.Append(" AND    PS_Piano_Date_Raccolte_Semina.PivaSuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")

            If ID_PianoSeminaTestata <> 0 Then
                StrSQL.Append(" AND PS_Piano_Date_Raccolte_Semina.ID_PianoSeminaTestata = " & ID_PianoSeminaTestata & " ")
            End If


            '-------------------------------------------------------------------------- 
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   PS_Piano_Date_Raccolte_Semina.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   PS_Piano_Date_Raccolte_Semina.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------


            StrSQL.Append(" ) DATI ")

            StrSQL.Append(" GROUP BY Data ")

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
    Public Function Distinct_Cul_Cod(ByVal ID_PianoSeminaTestata As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PS_Piano_Date_Raccolte_Semina_R.Distinct_Cul_Cod()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT     PS_Piano_Date_Raccolte_Semina.Cul_Cod, SUM(PS_Piano_Date_Raccolte_Semina.ha) AS HA_Sum, Cultivar.Cul_Des ")
            StrSQL.Append(" ,Convert(real,(SELECT   PS_Zone_Specie_Varieta_Default.Valore FROM   PS_Zone_Specie_Varieta_Default  WHERE      (PS_Zone_Specie_Varieta_Default.ID_Zona = PS_PianoSeminaTestata.ID_Zona) AND (PS_Zone_Specie_Varieta_Default.Veg_Cod = PS_PianoSeminaTestata.Veg_Cod) AND (PS_Zone_Specie_Varieta_Default.Cul_Cod = PS_Piano_Date_Raccolte_Semina.Cul_Cod) AND (PS_Zone_Specie_Varieta_Default.Codice = 5))) as Unita_Calore")

            StrSQL.Append(" FROM         PS_Piano_Date_Raccolte_Semina INNER JOIN ")
            StrSQL.Append("       PS_PianoSeminaTestata ON PS_Piano_Date_Raccolte_Semina.PivaSuperUser = PS_PianoSeminaTestata.PivaSuperUser AND  ")
            StrSQL.Append("       PS_Piano_Date_Raccolte_Semina.ID_PianoSeminaTestata = PS_PianoSeminaTestata.ID_PianoSeminaTestata INNER JOIN ")
            StrSQL.Append("       Cultivar ON PS_Piano_Date_Raccolte_Semina.Cul_Cod = Cultivar.Cul_Cod AND PS_PianoSeminaTestata.Veg_Cod = Cultivar.Veg_Cod ")


            StrSQL.Append(" WHERE  PS_Piano_Date_Raccolte_Semina.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND    PS_Piano_Date_Raccolte_Semina.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.Append(" AND    PS_Piano_Date_Raccolte_Semina.PivaSuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")

            If ID_PianoSeminaTestata <> 0 Then
                StrSQL.Append(" AND PS_Piano_Date_Raccolte_Semina.ID_PianoSeminaTestata = " & ID_PianoSeminaTestata & " ")
            End If


            '-------------------------------------------------------------------------- 
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   PS_Piano_Date_Raccolte_Semina.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   PS_Piano_Date_Raccolte_Semina.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            StrSQL.Append(" GROUP BY PS_Piano_Date_Raccolte_Semina.Cul_Cod, Cultivar.Cul_Des, PS_PianoSeminaTestata.ID_Zona, PS_PianoSeminaTestata.veg_cod, PS_Piano_Date_Raccolte_Semina.Cul_Cod ")

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY unita_calore")
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
    Public Function Leggi_x_Grid_View(ByVal ID_PianoSeminaTestata As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PS_Piano_Date_Raccolte_Semina_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT     PS_Piano_Date_Raccolte_Semina.Cul_Cod, PS_Piano_Date_Raccolte_Semina.ID_Piano_Date_Raccolte_Semina, PS_Piano_Date_Raccolte_Semina.ha, PS_Piano_Date_Raccolte_Semina.nettissimo, ")
            StrSQL.Append("     (SELECT     Valore FROM          PS_Zone_Specie_Varieta_Default AS ps WHERE      (ID_Zona = PS_PianoSeminaTestata.ID_Zona) AND (Veg_Cod = PS_PianoSeminaTestata.Veg_Cod) AND (Cul_Cod = PS_Piano_Date_Raccolte_Semina.Cul_Cod) AND (Codice = 4)) AS Resa , ")
            StrSQL.Append("     (SELECT     Valore FROM          PS_Zone_Specie_Varieta_Default AS ps WHERE      (ID_Zona = PS_PianoSeminaTestata.ID_Zona) AND (Veg_Cod = PS_PianoSeminaTestata.Veg_Cod) AND (Cul_Cod = PS_Piano_Date_Raccolte_Semina.Cul_Cod) AND (Codice = 5)) AS Unita_Calore, ")
            StrSQL.Append("     (SELECT     Valore FROM          PS_Zone_Specie_Varieta_Default AS ps WHERE      (ID_Zona = PS_PianoSeminaTestata.ID_Zona) AND (Veg_Cod = PS_PianoSeminaTestata.Veg_Cod) AND (Cul_Cod = 0) AND (Codice = 14)) AS Soglia_Germinazione, ")

            StrSQL.Append("     PS_Piano_Date_Raccolte_Semina.raccolta_1, PS_Piano_Date_Raccolte_Semina.lock_raccolta_1, PS_Piano_Date_Raccolte_Semina.semina, ")
            StrSQL.Append("     PS_Piano_Date_Raccolte_Semina.lock_semina, PS_Piano_Date_Raccolte_Semina.raccolta_2, PS_Piano_Date_Raccolte_Semina.lock_raccolta_2, Cultivar.Cul_Des ")

            StrSQL.Append(" FROM         PS_Piano_Date_Raccolte_Semina INNER JOIN ")
            StrSQL.Append("         PS_PianoSeminaTestata ON PS_Piano_Date_Raccolte_Semina.PivaSuperUser = PS_PianoSeminaTestata.PivaSuperUser AND ")
            StrSQL.Append("         PS_Piano_Date_Raccolte_Semina.ID_PianoSeminaTestata = PS_PianoSeminaTestata.ID_PianoSeminaTestata INNER JOIN ")
            StrSQL.Append("         Cultivar ON PS_Piano_Date_Raccolte_Semina.Cul_Cod = Cultivar.Cul_Cod AND PS_PianoSeminaTestata.Veg_Cod = Cultivar.Veg_Cod ")

            StrSQL.Append(" WHERE  PS_Piano_Date_Raccolte_Semina.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND    PS_Piano_Date_Raccolte_Semina.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.Append(" AND    PS_Piano_Date_Raccolte_Semina.PivaSuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")

            If ID_PianoSeminaTestata <> 0 Then
                StrSQL.Append(" AND PS_Piano_Date_Raccolte_Semina.ID_PianoSeminaTestata = " & ID_PianoSeminaTestata & " ")
            End If

            '-------------------------------------------------------------------------- 
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   PS_Piano_Date_Raccolte_Semina.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   PS_Piano_Date_Raccolte_Semina.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append("   order by raccolta_1 ")
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