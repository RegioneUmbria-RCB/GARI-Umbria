Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider


'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
Public Class Analisi_EntitaxCertificato_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '============================================================================
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' da utilizzare con Selezione_TabellaCompleta
    ''' </summary>
    ''' <param name="Analisi_Certificato_Cod">/param>
    ''' <param name="Analisi_Entita_Cod"></param>
    ''' <param name="Piva"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="Campo_Cod"></param>
    ''' <param name="Appezza"></param>
    ''' <param name="Id_Imp"></param>
    ''' <param name="Fabbricato_Cod"></param>
    ''' <param name="Prov"></param>
    ''' <param name="Com"></param>
    ''' <param name="Sezione"></param>
    ''' <param name="Foglio"></param>
    ''' <param name="Numero"></param>
    ''' <param name="Subalterno"></param>
    ''' <param name="Id_Oggetto_Grafico"></param>
    ''' <param name="xSelezioneVariabile"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni] 05/01/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function Leggi(ByVal Analisi_Certificato_Cod As Integer, _
                            ByVal Analisi_Entita_Cod As Integer, _
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
                            ByVal Id_Oggetto_Grafico As String, _
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable



        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Analisi_EntitaxCertificato_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    '------------------------------

                    'Query per il prelievo dei dati                 ' #### CLASSE ####

                    StrSQL.Append(" SELECT Analisi_EntitaxCertificato.* ")
                    StrSQL.Append(" FROM   Analisi_EntitaxCertificato ")
                    StrSQL.Append(" WHERE  Analisi_EntitaxCertificato.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND    Analisi_EntitaxCertificato.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND    Analisi_EntitaxCertificato.Analisi_EntitaxCertificato_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")

                    If Analisi_Certificato_Cod <> 0 Then
                        StrSQL.Append(" AND Analisi_EntitaxCertificato.Analisi_Certificato_Cod = " & Analisi_Certificato_Cod & " ")
                    End If

                    If Analisi_Entita_Cod <> 0 Then
                        StrSQL.Append(" AND Analisi_EntitaxCertificato.Analisi_Entita_Cod = " & Analisi_Entita_Cod & " ")
                    End If

                    If Trim(Piva) <> "" Then
                        StrSQL.Append(" AND Analisi_EntitaxCertificato.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Analisi_EntitaxCertificato.Sa_Cod = " & Sa_Cod & " ")
                    End If

                    If Campo_Cod <> 0 Then
                        StrSQL.Append(" AND Analisi_EntitaxCertificato.Campo_Cod = " & Campo_Cod & " ")
                    End If

                    If Appezza <> 0 Then
                        StrSQL.Append(" AND Analisi_EntitaxCertificato.Appezza = " & Appezza & " ")
                    End If

                    If Id_Imp <> 0 Then
                        StrSQL.Append(" AND Analisi_EntitaxCertificato.Id_Imp = " & Id_Imp & " ")
                    End If

                    If Fabbricato_Cod <> 0 Then
                        StrSQL.Append(" AND Analisi_EntitaxCertificato.Fabbricato_Cod = " & Fabbricato_Cod & " ")
                    End If

                    If Trim(Prov) <> "" Then
                        StrSQL.Append(" AND Analisi_EntitaxCertificato.Prov = '" & Agro_SQL_SaveText(Prov) & "'")
                    End If

                    If Trim(Com) <> "" Then
                        StrSQL.Append(" AND Analisi_EntitaxCertificato.Com = '" & Agro_SQL_SaveText(Com) & "'")
                    End If

                    If Trim(Sezione) <> "" Then
                        StrSQL.Append(" AND Analisi_EntitaxCertificato.Sezione = '" & Agro_SQL_SaveText(Sezione) & "'")
                    End If

                    If Numero <> 0 Then
                        StrSQL.Append(" AND Analisi_EntitaxCertificato.Numero = " & Numero & " ")
                    End If

                    If Foglio <> 0 Then
                        StrSQL.Append(" AND Analisi_EntitaxCertificato.Foglio = " & Foglio & " ")
                    End If

                    If Trim(Subalterno) <> "" Then
                        StrSQL.Append(" AND Analisi_EntitaxCertificato.Subalterno = '" & Agro_SQL_SaveText(Subalterno) & "'")
                    End If

                    If Trim(Id_Oggetto_Grafico) <> "" Then
                        StrSQL.Append(" AND Analisi_EntitaxCertificato.Id_Oggetto_Grafico = '" & Agro_SQL_SaveText(Id_Oggetto_Grafico) & "'")
                    End If

                    '------------------------------


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Analisi_EntitaxTestata.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Analisi_EntitaxTestata.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta


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


End Class



'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

Public Class Analisi_EntitaxCertificato_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '#########################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Analisi_Certificato_Cod">/param>
    ''' <param name="Analisi_Entita_Cod"></param>
    ''' <param name="Piva"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="Campo_Cod"></param>
    ''' <param name="Appezza"></param>
    ''' <param name="Id_Imp"></param>
    ''' <param name="Fabbricato_Cod"></param>
    ''' <param name="Prov"></param>
    ''' <param name="Com"></param>
    ''' <param name="Sezione"></param>
    ''' <param name="Foglio"></param>
    ''' <param name="Numero"></param>
    ''' <param name="Subalterno"></param>
    ''' <param name="Id_Oggetto_Grafico"></param>
    ''' <param name="DataLock"></param>
    ''' <param name="Validita_Inizio"></param>
    ''' <param name="Validita_Fine"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni] 05/01/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function Scrivi( _
                                ByVal Analisi_Certificato_Cod As Integer, _
                                ByVal Analisi_Entita_Cod As Integer, _
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
                                ByVal Id_Oggetto_Grafico As String, _
                                ByVal DataLock As Integer, _
                                ByVal Validita_Inizio As Date, _
                                ByVal Validita_Fine As Date, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Analisi_EntitaxCertificato_W.Scrivi()"

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

            '------------------------------

            'Query per l'inserimento dei dati                 ' #### CLASSE ####

            StrSQL.Append("INSERT INTO Analisi_EntitaxCertificato( ")
            StrSQL.Append("            Analisi_SuperUser,        Analisi_Certificato_Cod,   ")
            StrSQL.Append("            Analisi_Entita_Cod,    ")
            StrSQL.Append("            Piva,        Sa_Cod,         Campo_Cod,      Appezza,    ")
            StrSQL.Append("            Id_Imp,      Fabbricato_Cod, Prov,           Com,        ")
            StrSQL.Append("            Sezione,     Foglio,         Numero,         Subalterno, ")
            StrSQL.Append("            Id_Oggetto_Grafico,          DataLock,       Data_Agg,    ")
            StrSQL.Append("            Inviato, DataInvio, ")
            StrSQL.Append("            Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("            UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("            Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("            ) ")

            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Analisi_Certificato_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Analisi_Entita_Cod) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Campo_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Appezza) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Imp) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Fabbricato_Cod) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Prov) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Com) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Sezione) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Foglio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Numero) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Subalterno) & "' ")

            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Id_Oggetto_Grafico) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(DataLock) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append(")")

            '------------------------------


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
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Analisi_Certificato_Cod">/param>
    ''' <param name="Analisi_Entita_Cod"></param>
    ''' <param name="Piva"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="Campo_Cod"></param>
    ''' <param name="Appezza"></param>
    ''' <param name="Id_Imp"></param>
    ''' <param name="Fabbricato_Cod"></param>
    ''' <param name="Prov"></param>
    ''' <param name="Com"></param>
    ''' <param name="Sezione"></param>
    ''' <param name="Foglio"></param>
    ''' <param name="Numero"></param>
    ''' <param name="Subalterno"></param>
    ''' <param name="Id_Oggetto_Grafico"></param>
    ''' <param name="DataLock"></param>
    ''' <param name="Validita_Inizio"></param>
    ''' <param name="Validita_Fine"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni] 05/01/2011	Created
    ''' </history>
    Public Function Modifica( _
                                ByVal Analisi_Certificato_Cod As Integer, _
                                ByVal Analisi_Entita_Cod As Integer, _
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
                                ByVal Id_Oggetto_Grafico As String, _
                                ByVal DataLock As Integer, _
                                ByVal Validita_Inizio As Date, _
                                ByVal Validita_Fine As Date, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Analisi_EntitaxCertificato_W.Modifica()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        If Analisi_Entita_Cod = 0 Then
            Throw New Exception("Analisi_Entita_Cod = 0 ")
        End If

        Try

            '------------------------------
            'Query per la modifica dei dati                 ' #### CLASSE ####

            StrSQL.Length = 0

            StrSQL.Append("UPDATE Analisi_EntitaxCertificato SET ")
            StrSQL.Append("    Data_Agg          = " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("   ,DataLock          = " & Agro_SQL_SaveNum(DataLock) & "  ")
            StrSQL.Append("   ,Inviato           =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append(" WHERE Analisi_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")


            If Analisi_Certificato_Cod <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxCertificato.Analisi_Certificato_Cod = " & Analisi_Certificato_Cod & " ")
            End If

            If Analisi_Entita_Cod <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxCertificato.Analisi_Entita_Cod = " & Analisi_Entita_Cod & " ")
            End If

            If Trim(Piva) <> "" Then
                StrSQL.Append(" AND Analisi_EntitaxCertificato.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxCertificato.Sa_Cod = " & Sa_Cod & " ")
            End If

            If Campo_Cod <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxCertificato.Campo_Cod = " & Campo_Cod & " ")
            End If

            If Appezza <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxCertificato.Appezza = " & Appezza & " ")
            End If

            If Id_Imp <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxCertificato.Id_Imp = " & Id_Imp & " ")
            End If

            If Fabbricato_Cod <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxCertificato.Fabbricato_Cod = " & Fabbricato_Cod & " ")
            End If

            If Trim(Prov) <> "" Then
                StrSQL.Append(" AND Analisi_EntitaxCertificato.Prov = '" & Agro_SQL_SaveText(Prov) & "'")
            End If

            If Trim(Com) <> "" Then
                StrSQL.Append(" AND Analisi_EntitaxCertificato.Com = '" & Agro_SQL_SaveText(Com) & "'")
            End If

            If Trim(Sezione) <> "" Then
                StrSQL.Append(" AND Analisi_EntitaxCertificato.Sezione = '" & Agro_SQL_SaveText(Sezione) & "'")
            End If

            If Numero <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxCertificato.Numero = " & Numero & " ")
            End If

            If Foglio <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxCertificato.Foglio = " & Foglio & " ")
            End If

            If Trim(Subalterno) <> "" Then
                StrSQL.Append(" AND Analisi_EntitaxCertificato.Subalterno = '" & Agro_SQL_SaveText(Subalterno) & "'")
            End If

            If Trim(Id_Oggetto_Grafico) <> "" Then
                StrSQL.Append(" AND Analisi_EntitaxCertificato.Id_Oggetto_Grafico = '" & Agro_SQL_SaveText(Id_Oggetto_Grafico) & "'")
            End If


            '------------------------------

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
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Analisi_Certificato_Cod">/param>
    ''' <param name="Analisi_Entita_Cod"></param>
    ''' <param name="Piva"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="Campo_Cod"></param>
    ''' <param name="Appezza"></param>
    ''' <param name="Id_Imp"></param>
    ''' <param name="Fabbricato_Cod"></param>
    ''' <param name="Prov"></param>
    ''' <param name="Com"></param>
    ''' <param name="Sezione"></param>
    ''' <param name="Foglio"></param>
    ''' <param name="Numero"></param>
    ''' <param name="Subalterno"></param>
    ''' <param name="Id_Oggetto_Grafico"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni] 05/01/2011	Created
    ''' </history>
    Public Function Cancella( _
                                ByVal Analisi_Certificato_Cod As Integer, _
                                ByVal Analisi_Entita_Cod As Integer, _
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
                                ByVal Id_Oggetto_Grafico As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Analisi_EntitaxCertificato_W.Cancella()"

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

        If Analisi_Entita_Cod = 0 Then
            Throw New Exception("Analisi_Entita_Cod = 0 ")
        End If

        Try

            '------------------------------

            'Query per la cancellazione dei dati                 ' #### CLASSE ####
            '
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0

                StrSQL.Append(" UPDATE  Analisi_EntitaxCertificato ")
                StrSQL.Append(" SET ")
                StrSQL.Append("          Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         ,Data_Modifica = '" & Agro_SQL_SaveDate(Date.Now) & "' ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE Analisi_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append("   AND    Inviato > 0 ")

            Else

                StrSQL.Length = 0

                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     Analisi_EntitaxCertificato ")
                StrSQL.Append(" WHERE    Analisi_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append("   AND    Inviato = 0 ")

            End If


            If Analisi_Certificato_Cod <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxCertificato.Analisi_Certificato_Cod = " & Analisi_Certificato_Cod & " ")
            End If

            If Analisi_Entita_Cod <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxCertificato.Analisi_Entita_Cod = " & Analisi_Entita_Cod & " ")
            End If

            If Trim(Piva) <> "" Then
                StrSQL.Append(" AND Analisi_EntitaxCertificato.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxCertificato.Sa_Cod = " & Sa_Cod & " ")
            End If

            If Campo_Cod <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxCertificato.Campo_Cod = " & Campo_Cod & " ")
            End If

            If Appezza <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxCertificato.Appezza = " & Appezza & " ")
            End If

            If Id_Imp <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxCertificato.Id_Imp = " & Id_Imp & " ")
            End If

            If Fabbricato_Cod <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxCertificato.Fabbricato_Cod = " & Fabbricato_Cod & " ")
            End If

            If Trim(Prov) <> "" Then
                StrSQL.Append(" AND Analisi_EntitaxCertificato.Prov = '" & Agro_SQL_SaveText(Prov) & "'")
            End If

            If Trim(Com) <> "" Then
                StrSQL.Append(" AND Analisi_EntitaxCertificato.Com = '" & Agro_SQL_SaveText(Com) & "'")
            End If

            If Trim(Sezione) <> "" Then
                StrSQL.Append(" AND Analisi_EntitaxCertificato.Sezione = '" & Agro_SQL_SaveText(Sezione) & "'")
            End If

            If Numero <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxCertificato.Numero = " & Numero & " ")
            End If

            If Foglio <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxCertificato.Foglio = " & Foglio & " ")
            End If

            If Trim(Subalterno) <> "" Then
                StrSQL.Append(" AND Analisi_EntitaxCertificato.Subalterno = '" & Agro_SQL_SaveText(Subalterno) & "'")
            End If

            If Trim(Id_Oggetto_Grafico) <> "" Then
                StrSQL.Append(" AND Analisi_EntitaxCertificato.Id_Oggetto_Grafico = '" & Agro_SQL_SaveText(Id_Oggetto_Grafico) & "'")
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
