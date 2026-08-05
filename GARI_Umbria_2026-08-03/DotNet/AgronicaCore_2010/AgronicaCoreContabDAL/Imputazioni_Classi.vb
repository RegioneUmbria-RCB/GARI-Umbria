Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class Imputazioni_Classi_R
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function Leggi(ByVal piva As String,
                          ByVal Imputazione_Classe_Cod As Integer,
                          ByVal Imputazione_Classe_Des As String,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Imputazioni_Classi_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Select Case xSelezioneVariabile
                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT * ")
                    StrSQL.AppendLine(" FROM Imputazioni_Classi ")
                    StrSQL.AppendLine(" WHERE  Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
                    StrSQL.AppendLine(" AND    Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
                    StrSQL.AppendLine(" AND   Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                    StrSQL.AppendLine(" AND   Piva = '" & Agro_SQL_SaveText(piva) & "' ")


                    If Imputazione_Classe_Cod <> 0 Then
                        StrSQL.AppendLine(" AND   Imputazione_Classe_Cod = " & Agro_SQL_SaveNum(Imputazione_Classe_Cod) & " ")
                    End If


                    If Imputazione_Classe_Des <> "" Then
                        StrSQL.AppendLine(" AND   Imputazione_Classe_Des = '" & Agro_SQL_SaveText(Imputazione_Classe_Des) & "' ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query ")
                    End Select
                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT  DISTINCT i_c.Imputazione_Classe_Cod, i_c.Imputazione_Classe_Des,i_c.Imputazione_Classe_Padre_Cod, ")
                    StrSQL.AppendLine(" CASE  ")
                    StrSQL.AppendLine("     WHEN i_c.Imputazione_Classe_Padre_Cod = -1 THEN 'Nessuna Classe Progetto' ")
                    StrSQL.AppendLine("     ELSE '' ")
                    StrSQL.AppendLine(" END AS Imputazione_Classe_Padre_Des,")
                    StrSQL.AppendLine(" i_c.Validita_Inizio, i_c.Validita_Fine ,i_c.Tipo_Classe ")
                    StrSQL.AppendLine(" FROM Imputazioni_Classi i_c")
                    StrSQL.AppendLine(" WHERE  i_c.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
                    StrSQL.AppendLine(" AND    i_c.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
                    StrSQL.AppendLine(" AND   i_c.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                    StrSQL.AppendLine(" AND   i_c.Piva = '" & Agro_SQL_SaveText(piva) & "' ")


                    If Imputazione_Classe_Cod <> 0 Then
                        StrSQL.AppendLine(" AND   i_c.Imputazione_Classe_Cod = " & Agro_SQL_SaveNum(Imputazione_Classe_Cod) & " ")
                    End If


                    If Imputazione_Classe_Des <> "" Then
                        StrSQL.AppendLine(" AND   i_c.Imputazione_Classe_Des = '" & Agro_SQL_SaveText(Imputazione_Classe_Des) & "' ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   i_c.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   i_c.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query ")
                    End Select
                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If
            End Select

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] :   " & MessaggioErrore)
        End Try


        Return DT

    End Function

End Class


Public Class Imputazioni_Classi_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal Piva As String,
                           ByVal Imputazione_Classe_Cod As Integer,
                           ByVal Imputazione_Classe_Des As String,
                           ByVal Imputazione_Classe_Padre_Cod As Integer,
                           ByVal Tipo_Classe As Integer,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                           ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Imputazioni_Classi_W.Scrivi()"

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
            StrSQL.AppendLine("INSERT INTO Imputazioni_Classi( ")
            StrSQL.AppendLine("                            Piva,   Piva_SuperUser,Imputazione_Classe_Cod,")
            StrSQL.AppendLine("                            Imputazione_Classe_Des, Imputazione_Classe_Padre_Cod, Tipo_Classe, ")
            StrSQL.AppendLine("                            Inviato,            DataInvio, ")
            StrSQL.AppendLine("                            Data_Creazione,     Data_Modifica, ")
            StrSQL.AppendLine("                            UserName_Creazione, UserName_Modifica, ")
            StrSQL.AppendLine("                            Validita_Inizio,    Validita_Fine ")
            StrSQL.AppendLine("                            ) ")


            StrSQL.AppendLine("VALUES ( ")
            StrSQL.AppendLine("          '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Imputazione_Classe_Cod) & " ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Imputazione_Classe_Des) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Imputazione_Classe_Padre_Cod) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Tipo_Classe) & " ")

            StrSQL.AppendLine("         , 0  ")
            StrSQL.AppendLine("         , Null  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.AppendLine(" )")
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


    Public Function Modifica(ByVal Piva As String,
                           ByVal Imputazione_Classe_Cod As Integer,
                           ByVal Imputazione_Classe_Des As String,
                           ByVal Imputazione_Classe_Padre_Cod As Integer,
                           ByVal Tipo_Classe As Integer,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Imputazioni_Classi_W.Modifica()"

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


            StrSQL.AppendLine("UPDATE Imputazioni_Classi SET ")
            StrSQL.AppendLine("     Imputazione_Classe_Des = '" & Agro_SQL_SaveText(Imputazione_Classe_Des) & "'")
            StrSQL.AppendLine("     ,Imputazione_Classe_Padre_Cod = " & Agro_SQL_SaveNum(Imputazione_Classe_Padre_Cod) & " ")
            StrSQL.AppendLine("     ,Tipo_Classe = " & Agro_SQL_SaveNum(Tipo_Classe) & " ")


            StrSQL.AppendLine("   ,Inviato           =  0 ")
            StrSQL.AppendLine("   ,DataInvio         =  Null ")
            StrSQL.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.AppendLine("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.AppendLine("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            StrSQL.AppendLine(" WHERE Piva_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")
            StrSQL.AppendLine(" AND   Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.AppendLine(" AND   Imputazione_Classe_Cod = " & Agro_SQL_SaveNum(Imputazione_Classe_Cod) & " ")

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

    Public Function Cancella(ByVal Piva As String,
                             ByVal Imputazione_Classe_Cod As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Imputazioni_Classi_W.Cancella()"

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
            StrSQL.AppendLine(" DELETE ")
            StrSQL.AppendLine(" FROM  Imputazioni_Classi ")
            StrSQL.AppendLine(" WHERE  Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "'  ")
            StrSQL.AppendLine(" AND Imputazione_Classe_Cod = " & Agro_SQL_SaveNum(Imputazione_Classe_Cod) & "  ")

            '----------------------------------------------------------------------
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
End Class
