Imports System.Data.OleDb
Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider


'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
Public Class Analisi_EntitaxTestata_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '===========================================================================
    Public Function Leggi(ByVal Analisi_Testata_Cod As Integer,
                            ByVal Analisi_Entita_Cod As Integer,
                            ByVal Piva As String,
                            ByVal Sa_Cod As Integer,
                            ByVal Campo_Cod As Integer,
                            ByVal Appezza As Integer,
                            ByVal Id_Imp As Integer,
                            ByVal Fabbricato_Cod As Integer,
                            ByVal PROV As String,
                            ByVal COM As String,
                            ByVal SEZIONE As String,
                            ByVal FOGLIO As Integer,
                            ByVal NUMERO As Integer,
                            ByVal SUBALTERNO As String,
                            ByVal Id_Oggetto_Grafico As String,
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable



        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Analisi_EntitaxTestata_R.Leggi()"

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
                    StrSQL.Append(" SELECT Analisi_EntitaxTestata.* ")
                    StrSQL.Append(" FROM   Analisi_EntitaxTestata ")
                    StrSQL.Append(" WHERE  Analisi_EntitaxTestata.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND    Analisi_EntitaxTestata.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND    Analisi_EntitaxTestata.Analisi_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")

                    If Analisi_Testata_Cod <> 0 Then
                        StrSQL.Append(" AND Analisi_EntitaxTestata.Analisi_Testata_Cod = " & Analisi_Testata_Cod & " ")
                    End If

                    If Analisi_Entita_Cod <> 0 Then
                        StrSQL.Append(" AND Analisi_EntitaxTestata.Analisi_Entita_Cod = " & Analisi_Entita_Cod & " ")
                    End If

                    If Trim(Piva) <> "" Then
                        StrSQL.Append(" AND Analisi_EntitaxTestata.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Analisi_EntitaxTestata.Sa_Cod = " & Sa_Cod & " ")
                    End If

                    If Campo_Cod <> 0 Then
                        StrSQL.Append(" AND Analisi_EntitaxTestata.Campo_Cod = " & Campo_Cod & " ")
                    End If

                    If Appezza <> 0 Then
                        StrSQL.Append(" AND Analisi_EntitaxTestata.Appezza = " & Appezza & " ")
                    End If

                    If Id_Imp <> 0 Then
                        StrSQL.Append(" AND Analisi_EntitaxTestata.Id_Imp = " & Id_Imp & " ")
                    End If

                    If Fabbricato_Cod <> 0 Then
                        StrSQL.Append(" AND Analisi_EntitaxTestata.Fabbricato_Cod = " & Fabbricato_Cod & " ")
                    End If

                    If Trim(PROV) <> "" Then
                        StrSQL.Append(" AND Analisi_EntitaxTestata.Prov = '" & Agro_SQL_SaveText(PROV) & "'")
                    End If

                    If Trim(COM) <> "" Then
                        StrSQL.Append(" AND Analisi_EntitaxTestata.Com = '" & Agro_SQL_SaveText(COM) & "'")
                    End If

                    If Trim(SEZIONE) <> "" Then
                        StrSQL.Append(" AND Analisi_EntitaxTestata.Sezione = '" & Agro_SQL_SaveText(SEZIONE) & "'")
                    End If

                    If NUMERO <> 0 Then
                        StrSQL.Append(" AND Analisi_EntitaxTestata.Numero = " & NUMERO & " ")
                    End If

                    If FOGLIO <> 0 Then
                        StrSQL.Append(" AND Analisi_EntitaxTestata.Foglio = " & FOGLIO & " ")
                    End If

                    If Trim(SUBALTERNO) <> "" Then
                        StrSQL.Append(" AND Analisi_EntitaxTestata.Subalterno = '" & Agro_SQL_SaveText(SUBALTERNO) & "'")
                    End If

                    If Trim(Id_Oggetto_Grafico) <> "" Then
                        StrSQL.Append(" AND Analisi_EntitaxTestata.Id_Oggetto_Grafico = '" & Agro_SQL_SaveText(Id_Oggetto_Grafico) & "'")
                    End If


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
                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT  * ")
                    StrSQL.Append(" FROM    Analisi_EntitaxTestata INNER JOIN ")
                    StrSQL.Append("         Analisi_Testata ON Analisi_EntitaxTestata.Analisi_SuperUser = Analisi_Testata.Analisi_SuperUser AND  ")
                    StrSQL.Append("         Analisi_EntitaxTestata.Analisi_Testata_Cod = Analisi_Testata.Analisi_Testata_Cod ")

                    StrSQL.Append(" WHERE   (Analisi_EntitaxTestata.Analisi_SuperUser = '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "')  ")

                    '----- Condizioni

                    If Analisi_Entita_Cod <> 0 Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Analisi_Entita_Cod = " & Agro_SQL_SaveNum(Analisi_Entita_Cod) & ")  ")
                    End If

                    If Piva <> "" Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "')  ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & ")  ")
                    End If

                    If Campo_Cod <> 0 Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod) & ")  ")
                    End If

                    If Appezza <> 0 Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Appezza = " & Agro_SQL_SaveNum(Appezza) & ")  ")
                    End If

                    If Id_Imp <> 0 Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Id_Imp = " & Agro_SQL_SaveNum(Id_Imp) & ")  ")
                    End If

                    If Fabbricato_Cod <> 0 Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Fabbricato_Cod = " & Agro_SQL_SaveNum(Fabbricato_Cod) & ")  ")
                    End If

                    If PROV <> "" Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Prov = '" & Agro_SQL_SaveText(PROV) & "') ")
                    End If

                    If COM <> "" Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Com = '" & Agro_SQL_SaveText(COM) & "')  ")
                    End If

                    If SEZIONE <> "" Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Sezione = '" & Agro_SQL_SaveText(SEZIONE) & "')  ")
                    End If

                    If FOGLIO <> 0 Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Foglio = " & Agro_SQL_SaveNum(FOGLIO) & ")  ")
                    End If

                    If NUMERO <> 0 Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Numero = " & Agro_SQL_SaveNum(NUMERO) & ")  ")
                    End If

                    If SUBALTERNO <> "" Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Subalterno = '" & Agro_SQL_SaveText(SUBALTERNO) & "')  ")
                    End If

                    If Id_Oggetto_Grafico <> "" Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.ID_Oggetto_Grafico = '" & Agro_SQL_SaveText(Id_Oggetto_Grafico) & "')")
                    End If

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

    '===========================================================================
    Public Function Distinct_TestataCod_Piva(ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Analisi_EntitaxTestata_R.Distinct_TestataCod_Piva()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" SELECT DISTINCT Analisi_SuperUser, Analisi_Testata_Cod, Piva")
            StrSQL.Append(" FROM   Analisi_EntitaxTestata ")
            StrSQL.Append(" WHERE  Analisi_EntitaxTestata.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND    Analisi_EntitaxTestata.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.Append(" AND    Analisi_EntitaxTestata.Analisi_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")

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


    '===========================================================================
    ''' <summary>
    ''' Da utilizzare con Selezione_TabellaDatiMinimi o Selezione_TabellaCompleta
    ''' </summary>
    ''' <param name="Analisi_Entita_Cod">Per non essere considerato 0</param>
    ''' <param name="Piva">Per non essere considerato ""</param>
    ''' <param name="Sa_Cod">Per non essere considerato 0</param>
    ''' <param name="Campo_Cod">Per non essere considerato 0</param>
    ''' <param name="Appezza">Per non essere considerato 0</param>
    ''' <param name="Id_Imp">Per non essere considerato 0</param>
    ''' <param name="Fabbricato_Cod">Per non essere considerato 0</param>
    ''' <param name="Prov">Per non essere considerato ""</param>
    ''' <param name="Com">Per non essere considerato ""</param>
    ''' <param name="Sezione">Per non essere considerato ""</param>
    ''' <param name="Foglio">Per non essere considerato 0</param>
    ''' <param name="Numero">Per non essere considerato 0</param>
    ''' <param name="Subalterno">Per non essere considerato ""</param>
    ''' <param name="ID_oggetto_Grafico">Per non essere considerato "0"</param>
    ''' <param name="Vas_Cod">Per non essere considerato 0</param>
    ''' <param name="Validita_Inizio">Per non essere considerato AGRODATAINIZIO</param>
    ''' <param name="Validita_Fine">Per non essere considerato AGRODATAFINE</param>
    ''' <param name="xSelezioneVariabile">Da utilizzare con Selezione_TabellaDatiMinimi o Selezione_TabellaCompleta</param>
    ''' <param name="xFiltroAggiuntivo">Per non essere considerato ""</param>
    ''' <param name="xOrderBy">Per non essere considerato ""</param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Leggi3(ByVal Analisi_Entita_Cod As Integer,
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
                                 ByVal ID_oggetto_Grafico As String,
                                 ByVal Vas_Cod As Integer,
                                 ByVal Validita_Inizio As Date,
                                 ByVal Validita_Fine As Date,
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable



        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Analisi_EntitaxTestata_R.Leggi3()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT  DISTINCT Analisi_Testata.* ")
                    StrSQL.Append(" FROM    Analisi_EntitaxTestata INNER JOIN ")
                    StrSQL.Append("         Analisi_Testata ON Analisi_EntitaxTestata.Analisi_SuperUser = Analisi_Testata.Analisi_SuperUser AND  ")
                    StrSQL.Append("         Analisi_EntitaxTestata.Analisi_Testata_Cod = Analisi_Testata.Analisi_Testata_Cod ")

                    StrSQL.Append(" WHERE   (Analisi_EntitaxTestata.Analisi_SuperUser = '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "')  ")

                    '----- Condizioni

                    If Analisi_Entita_Cod <> 0 Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Analisi_Entita_Cod = " & Agro_SQL_SaveNum(Analisi_Entita_Cod) & ")  ")
                    End If

                    If Piva <> "" Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "')  ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & ")  ")
                    End If

                    If Campo_Cod <> 0 Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod) & ")  ")
                    End If

                    If Appezza <> 0 Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Appezza = " & Agro_SQL_SaveNum(Appezza) & ")  ")
                    End If

                    If Id_Imp <> 0 Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Id_Imp = " & Agro_SQL_SaveNum(Id_Imp) & ")  ")
                    End If

                    If Fabbricato_Cod <> 0 Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Fabbricato_Cod = " & Agro_SQL_SaveNum(Fabbricato_Cod) & ")  ")
                    End If

                    If Prov <> "" Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Prov = '" & Agro_SQL_SaveText(Prov) & "') ")
                    End If

                    If Com <> "" Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Com = '" & Agro_SQL_SaveText(Com) & "')  ")
                    End If

                    If Sezione <> "" And Sezione <> "0" Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Sezione = '" & Agro_SQL_SaveText(Sezione) & "')  ")
                    End If

                    If Foglio <> 0 Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Foglio = " & Agro_SQL_SaveNum(Foglio) & ")  ")
                    End If

                    If Numero <> 0 Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Numero = " & Agro_SQL_SaveNum(Numero) & ")  ")
                    End If

                    If Subalterno <> "" And Subalterno <> "0" Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Subalterno = '" & Agro_SQL_SaveText(Subalterno) & "')  ")
                    End If

                    If ID_oggetto_Grafico <> "0" Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.ID_Oggetto_Grafico = '" & Agro_SQL_SaveText(ID_oggetto_Grafico) & "')")
                    End If

                    If Vas_Cod <> 0 Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Vas_Cod = " & Agro_SQL_SaveNum(Vas_Cod) & ")  ")
                    End If

                    StrSQL.Append(" AND (Analisi_Testata.Analisi_Testata_Data_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & ") ")
                    StrSQL.Append(" AND (Analisi_Testata.Analisi_Testata_Data_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & ") ")

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Analisi_EntitaxTestata.Inviato >=0 ")
                            StrSQL.Append(" AND   Analisi_Testata.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Analisi_EntitaxTestata.Inviato =-1 ")
                            StrSQL.Append(" AND   Analisi_Testata.Inviato =-1 ")
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

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT  *  ")
                    StrSQL.Append(" FROM    Analisi_EntitaxTestata INNER JOIN ")
                    StrSQL.Append("         Analisi_Testata ON Analisi_EntitaxTestata.Analisi_SuperUser = Analisi_Testata.Analisi_SuperUser AND  ")
                    StrSQL.Append("         Analisi_EntitaxTestata.Analisi_Testata_Cod = Analisi_Testata.Analisi_Testata_Cod ")

                    StrSQL.Append(" WHERE   (Analisi_EntitaxTestata.Analisi_SuperUser = '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "')  ")

                    '----- Condizioni

                    If Analisi_Entita_Cod <> 0 Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Analisi_Entita_Cod = " & Agro_SQL_SaveNum(Analisi_Entita_Cod) & ")  ")
                    End If

                    If Piva <> "" Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "')  ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & ")  ")
                    End If

                    If Campo_Cod <> 0 Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod) & ")  ")
                    End If

                    If Appezza <> 0 Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Appezza = " & Agro_SQL_SaveNum(Appezza) & ")  ")
                    End If

                    If Id_Imp <> 0 Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Id_Imp = " & Agro_SQL_SaveNum(Id_Imp) & ")  ")
                    End If

                    If Fabbricato_Cod <> 0 Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Fabbricato_Cod = " & Agro_SQL_SaveNum(Fabbricato_Cod) & ")  ")
                    End If

                    If Prov <> "" Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Prov = '" & Agro_SQL_SaveText(Prov) & "') ")
                    End If

                    If Com <> "" Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Com = '" & Agro_SQL_SaveText(Com) & "')  ")
                    End If

                    If Sezione <> "" And Sezione <> "0" Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Sezione = '" & Agro_SQL_SaveText(Sezione) & "')  ")
                    End If

                    If Foglio <> 0 Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Foglio = " & Agro_SQL_SaveNum(Foglio) & ")  ")
                    End If

                    If Numero <> 0 Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Numero = " & Agro_SQL_SaveNum(Numero) & ")  ")
                    End If

                    If Subalterno <> "" And Subalterno <> "0" Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Subalterno = '" & Agro_SQL_SaveText(Subalterno) & "')  ")
                    End If

                    If ID_oggetto_Grafico <> "0" Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.ID_Oggetto_Grafico = '" & Agro_SQL_SaveText(ID_oggetto_Grafico) & "')")
                    End If

                    If Vas_Cod <> 0 Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Vas_Cod = " & Agro_SQL_SaveNum(Vas_Cod) & ")  ")
                    End If

                    StrSQL.Append(" AND (Analisi_Testata.Analisi_Testata_Data_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & ") ")
                    StrSQL.Append(" AND (Analisi_Testata.Analisi_Testata_Data_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & ") ")

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Analisi_EntitaxTestata.Inviato >=0 ")
                            StrSQL.Append(" AND   Analisi_Testata.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Analisi_EntitaxTestata.Inviato =-1 ")
                            StrSQL.Append(" AND   Analisi_Testata.Inviato =-1 ")
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

    ''' <summary>
    ''' Da utilizzare con Selezione_TabellaDatiMinimi o Selezione_TabellaCompleta
    ''' </summary>
    ''' <param name="Analisi_Testata_Tipo">Per non essere considerato 0</param>
    ''' <param name="Analisi_Testata_Cod">Per non essere considerato 0</param>
    ''' <param name="Analisi_Entita_Cod">Per non essere considerato 0</param>
    ''' <param name="Piva">Per non essere considerato ""</param>
    ''' <param name="Sa_Cod">Per non essere considerato 0</param>
    ''' <param name="Campo_Cod">Per non essere considerato 0</param>
    ''' <param name="Appezza">Per non essere considerato 0</param>
    ''' <param name="Id_Imp">Per non essere considerato 0</param>
    ''' <param name="Fabbricato_Cod">Per non essere considerato 0</param>
    ''' <param name="Prov">Per non essere considerato ""</param>
    ''' <param name="Com">Per non essere considerato ""</param>
    ''' <param name="Sezione">Per non essere considerato ""</param>
    ''' <param name="Foglio">Per non essere considerato 0</param>
    ''' <param name="Numero">Per non essere considerato 0</param>
    ''' <param name="Subalterno">Per non essere considerato ""</param>
    ''' <param name="ID_oggetto_Grafico">Per non essere considerato "0"</param>
    ''' <param name="Vas_Cod">Per non essere considerato 0</param>
    ''' <param name="Validita_Inizio">Per non essere considerato AGRODATAINIZIO</param>
    ''' <param name="Validita_Fine">Per non essere considerato AGRODATAFINE</param>
    ''' <param name="xSelezioneVariabile">Da utilizzare con Selezione_TabellaDatiMinimi o Selezione_TabellaCompleta</param>
    ''' <param name="xFiltroAggiuntivo">Per non essere considerato ""</param>
    ''' <param name="xOrderBy">Per non essere considerato ""</param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Leggi4(ByVal Analisi_Testata_Tipo As Integer,
                                 ByVal Analisi_Testata_Cod As Integer,
                                 ByVal Analisi_Entita_Cod As Integer,
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
                                 ByVal ID_oggetto_Grafico As String,
                                 ByVal Vas_Cod As Integer,
                                 ByVal Validita_Inizio As Date,
                                 ByVal Validita_Fine As Date,
                                    ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable



        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Analisi_EntitaxTestata_R.Leggi4()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT  DISTINCT Analisi_Testata.* ")
                    StrSQL.Append(", ISNULL((SELECT Identificativo FROM Cantina_Vasche ")
                    StrSQL.Append(" where (Analisi_EntitaxTestata.vas_cod = Cantina_Vasche.Vas_cod) ")
                    StrSQL.Append(" ), '') AS Identificativo ")

                    StrSQL.Append(" FROM    Analisi_EntitaxTestata INNER JOIN ")
                    StrSQL.Append("         Analisi_Testata ON Analisi_EntitaxTestata.Analisi_SuperUser = Analisi_Testata.Analisi_SuperUser AND  ")
                    StrSQL.Append("         Analisi_EntitaxTestata.Analisi_Testata_Cod = Analisi_Testata.Analisi_Testata_Cod ")
                    StrSQL.Append("         INNER JOIN Analisi_Dettagli ")
                    StrSQL.Append("         ON Analisi_EntitaxTestata.Analisi_SuperUser = Analisi_Dettagli.Analisi_SuperUser AND Analisi_EntitaxTestata.Analisi_Testata_Cod = Analisi_Dettagli.Analisi_Testata_Cod ")
                    StrSQL.Append("         INNER JOIN Analisi_Parametri ")
                    StrSQL.Append("         ON Analisi_Dettagli.Analisi_Parametro_Cod = Analisi_Parametri.Analisi_Parametro_Cod ")
                    StrSQL.Append("         ")

                    StrSQL.Append(" WHERE   (Analisi_EntitaxTestata.Analisi_SuperUser = '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "')  ")
                    StrSQL.Append(" AND     (Analisi_Dettagli.Analisi_Parametro_Cod <> " & Agro_SQL_SaveNum(1) & ")  ")



                    '----- Condizioni

                    If Analisi_Testata_Tipo <> 0 Then
                        StrSQL.Append(" AND     (Analisi_Testata.Analisi_Testata_Tipo = " & Agro_SQL_SaveNum(Analisi_Testata_Tipo) & ")  ")
                    End If

                    If Analisi_Testata_Cod <> 0 Then
                        StrSQL.Append(" AND     (Analisi_Testata.Analisi_Testata_Cod = " & Agro_SQL_SaveNum(Analisi_Testata_Cod) & ")  ")
                    End If

                    If Analisi_Entita_Cod <> 0 Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Analisi_Entita_Cod = " & Agro_SQL_SaveNum(Analisi_Entita_Cod) & ")  ")
                    End If

                    If Piva <> "" Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "')  ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & ")  ")
                    End If

                    If Campo_Cod <> 0 Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod) & ")  ")
                    End If

                    If Appezza <> 0 Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Appezza = " & Agro_SQL_SaveNum(Appezza) & ")  ")
                    End If

                    If Id_Imp <> 0 Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Id_Imp = " & Agro_SQL_SaveNum(Id_Imp) & ")  ")
                    End If

                    If Fabbricato_Cod <> 0 Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Fabbricato_Cod = " & Agro_SQL_SaveNum(Fabbricato_Cod) & ")  ")
                    End If

                    If Prov <> "" Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Prov = '" & Agro_SQL_SaveText(Prov) & "') ")
                    End If

                    If Com <> "" Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Com = '" & Agro_SQL_SaveText(Com) & "')  ")
                    End If

                    If Sezione <> "" And Sezione <> "0" Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Sezione = '" & Agro_SQL_SaveText(Sezione) & "')  ")
                    End If

                    If Foglio <> 0 Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Foglio = " & Agro_SQL_SaveNum(Foglio) & ")  ")
                    End If

                    If Numero <> 0 Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Numero = " & Agro_SQL_SaveNum(Numero) & ")  ")
                    End If

                    If Subalterno <> "" And Subalterno <> "0" Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Subalterno = '" & Agro_SQL_SaveText(Subalterno) & "')  ")
                    End If

                    If ID_oggetto_Grafico <> "0" Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.ID_Oggetto_Grafico = '" & Agro_SQL_SaveText(ID_oggetto_Grafico) & "')")
                    End If

                    If Vas_Cod <> 0 Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Vas_Cod = " & Agro_SQL_SaveNum(Vas_Cod) & ")  ")
                    End If

                    StrSQL.Append(" AND (Analisi_Testata.Analisi_Testata_Data_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & ") ")
                    StrSQL.Append(" AND (Analisi_Testata.Analisi_Testata_Data_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & ") ")

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Analisi_EntitaxTestata.Inviato >=0 ")
                            StrSQL.Append(" AND   Analisi_Testata.Inviato >=0 ")
                            StrSQL.Append(" AND   Analisi_Dettagli.Inviato >=0 ")
                            StrSQL.Append(" AND   Analisi_Parametri.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Analisi_EntitaxTestata.Inviato =-1 ")
                            StrSQL.Append(" AND   Analisi_Testata.Inviato =-1 ")
                            StrSQL.Append(" AND   Analisi_Dettagli.Inviato =-1 ")
                            StrSQL.Append(" AND   Analisi_Parametri.Inviato =-1 ")
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

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT  * ")
                    StrSQL.Append(", ISNULL((SELECT Identificativo FROM Cantina_Vasche ")
                    StrSQL.Append(" where (Analisi_EntitaxTestata.vas_cod = Cantina_Vasche.Vas_cod) ")
                    StrSQL.Append(" ), '') AS Identificativo ")

                    StrSQL.Append(" FROM    Analisi_EntitaxTestata INNER JOIN ")
                    StrSQL.Append("         Analisi_Testata ON Analisi_EntitaxTestata.Analisi_SuperUser = Analisi_Testata.Analisi_SuperUser AND  ")
                    StrSQL.Append("         Analisi_EntitaxTestata.Analisi_Testata_Cod = Analisi_Testata.Analisi_Testata_Cod ")
                    StrSQL.Append("         INNER JOIN Analisi_Dettagli ")
                    StrSQL.Append("         ON Analisi_EntitaxTestata.Analisi_SuperUser = Analisi_Dettagli.Analisi_SuperUser AND Analisi_EntitaxTestata.Analisi_Testata_Cod = Analisi_Dettagli.Analisi_Testata_Cod ")
                    StrSQL.Append("         INNER JOIN Analisi_Parametri ")
                    StrSQL.Append("         ON Analisi_Dettagli.Analisi_Parametro_Cod = Analisi_Parametri.Analisi_Parametro_Cod ")
                    StrSQL.Append("         ")

                    StrSQL.Append(" WHERE   (Analisi_EntitaxTestata.Analisi_SuperUser = '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "')  ")
                    StrSQL.Append(" AND     (Analisi_Dettagli.Analisi_Parametro_Cod <> " & Agro_SQL_SaveNum(1) & ")  ")



                    '----- Condizioni

                    If Analisi_Testata_Tipo <> 0 Then
                        StrSQL.Append(" AND     (Analisi_Testata.Analisi_Testata_Tipo = " & Agro_SQL_SaveNum(Analisi_Testata_Tipo) & ")  ")
                    End If

                    If Analisi_Testata_Cod <> 0 Then
                        StrSQL.Append(" AND     (Analisi_Testata.Analisi_Testata_Cod = " & Agro_SQL_SaveNum(Analisi_Testata_Cod) & ")  ")
                    End If

                    If Analisi_Entita_Cod <> 0 Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Analisi_Entita_Cod = " & Agro_SQL_SaveNum(Analisi_Entita_Cod) & ")  ")
                    End If

                    If Piva <> "" Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "')  ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & ")  ")
                    End If

                    If Campo_Cod <> 0 Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod) & ")  ")
                    End If

                    If Appezza <> 0 Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Appezza = " & Agro_SQL_SaveNum(Appezza) & ")  ")
                    End If

                    If Id_Imp <> 0 Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Id_Imp = " & Agro_SQL_SaveNum(Id_Imp) & ")  ")
                    End If

                    If Fabbricato_Cod <> 0 Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Fabbricato_Cod = " & Agro_SQL_SaveNum(Fabbricato_Cod) & ")  ")
                    End If

                    If Prov <> "" Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Prov = '" & Agro_SQL_SaveText(Prov) & "') ")
                    End If

                    If Com <> "" Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Com = '" & Agro_SQL_SaveText(Com) & "')  ")
                    End If

                    If Sezione <> "" And Sezione <> "0" Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Sezione = '" & Agro_SQL_SaveText(Sezione) & "')  ")
                    End If

                    If Foglio <> 0 Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Foglio = " & Agro_SQL_SaveNum(Foglio) & ")  ")
                    End If

                    If Numero <> 0 Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Numero = " & Agro_SQL_SaveNum(Numero) & ")  ")
                    End If

                    If Subalterno <> "" And Subalterno <> "0" Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Subalterno = '" & Agro_SQL_SaveText(Subalterno) & "')  ")
                    End If

                    If ID_oggetto_Grafico <> "0" Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.ID_Oggetto_Grafico = '" & Agro_SQL_SaveText(ID_oggetto_Grafico) & "')")
                    End If

                    If Vas_Cod <> 0 Then
                        StrSQL.Append(" AND     (Analisi_EntitaxTestata.Vas_Cod = " & Agro_SQL_SaveNum(Vas_Cod) & ")  ")
                    End If

                    StrSQL.Append(" AND (Analisi_Testata.Analisi_Testata_Data_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & ") ")
                    StrSQL.Append(" AND (Analisi_Testata.Analisi_Testata_Data_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & ") ")

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Analisi_EntitaxTestata.Inviato >=0 ")
                            StrSQL.Append(" AND   Analisi_Testata.Inviato >=0 ")
                            StrSQL.Append(" AND   Analisi_Dettagli.Inviato >=0 ")
                            StrSQL.Append(" AND   Analisi_Parametri.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Analisi_EntitaxTestata.Inviato =-1 ")
                            StrSQL.Append(" AND   Analisi_Testata.Inviato =-1 ")
                            StrSQL.Append(" AND   Analisi_Dettagli.Inviato =-1 ")
                            StrSQL.Append(" AND   Analisi_Parametri.Inviato =-1 ")
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


    Public Function LeggixPrecaricaAlbero(ByVal Piva As String,
                               ByVal xFiltroAggiuntivo As String,
                               ByVal xOrderBy As String,
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                               ) As DataTable



        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Analisi_EntitaxTestata_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------

            StrSQL.Length = 0
            StrSQL.Append(" SELECT Analisi_EntitaxTestata.Analisi_Entita_Cod, Analisi_EntitaxTestata.Piva as Piva, Analisi_EntitaxTestata.Sa_Cod as Sa_Cod , ")
            StrSQL.Append(" Analisi_EntitaxTestata.Campo_Cod as Campo_Cod, Analisi_EntitaxTestata.Appezza as Appezza , ")
            StrSQL.Append(" Analisi_EntitaxTestata.Id_Imp as Id_Imp , ")
            StrSQL.Append(" Analisi_EntitaxTestata.Fabbricato_Cod as Fabbricato_Cod, Analisi_EntitaxTestata.Sa_Cod as Sa_Cod , ")
            StrSQL.Append(" Analisi_EntitaxTestata.Prov as Prov, Analisi_EntitaxTestata.Com as Com , ")

            StrSQL.Append(" Analisi_Testata.Analisi_Testata_Data_Inizio, ") ' Nico 29/01/2014


            StrSQL.Append(" Analisi_EntitaxTestata.Sezione as Sezione, Analisi_EntitaxTestata.Foglio as Foglio , ")
            StrSQL.Append(" Analisi_EntitaxTestata.Numero as Numero, Analisi_EntitaxTestata.Subalterno as Subalterno , ")
            StrSQL.Append(" Analisi_EntitaxTestata.ID_Oggetto_Grafico as ID_Oggetto_Grafico, Analisi_EntitaxTestata.Vas_Cod as Vas_Cod , ")

            StrSQL.Append(" Analisi_Testata.Analisi_Testata_Cod as Analisi_Testata_Cod, Analisi_Testata.Analisi_Testata_Des as Analisi_Testata_Des ")

            StrSQL.Append(" FROM    Analisi_EntitaxTestata INNER JOIN ")
            StrSQL.Append("         Analisi_Testata ON Analisi_EntitaxTestata.Analisi_SuperUser = Analisi_Testata.Analisi_SuperUser AND  ")
            StrSQL.Append("         Analisi_EntitaxTestata.Analisi_Testata_Cod = Analisi_Testata.Analisi_Testata_Cod ")

            StrSQL.Append(" WHERE   (Analisi_EntitaxTestata.Analisi_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "')  ")

            StrSQL.Append(" AND (Analisi_Testata.Analisi_Testata_Data_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ") ")
            StrSQL.Append(" AND (Analisi_Testata.Analisi_Testata_Data_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")

            If Piva <> "" Then
                StrSQL.Append(" AND     (Analisi_EntitaxTestata.Piva = '" & Agro_SQL_SaveText(Piva) & "')  ")
            End If



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
    Public Function Leggi_Tutte_Piva_Che_Hanno_Analisi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                       ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Analisi_EntitaxTestata.Leggi_Tutte_Piva_Che_Hanno_Analisi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim NumContatti As Integer = 0

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("  select distinct piva from Analisi_EntitaxTestata ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '-------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function Leggi_Tessitura_Scalare_Lista_Appezzamenti(listChiavi As List(Of (String, Integer, Integer, Integer)),
                                                               richiesta_cod As Integer,
                                                               objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                               ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Analisi_EntitaxTestata.Leggi_Tessitura_Scalare_Lista_Appezzamenti()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim StrJoinAnalisiParam As New System.Text.StringBuilder
        Dim StrSelectAnalisiParam As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim NumContatti As Integer = 0

        Try

            'AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, objParametri)
            Dim FlagConnessioneLocale = False
            Dim FlagTransazioneLocale = False
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale, objParametri)

            TempChiaviMassivo.CreaTabellaTemp_FiltroImpianti(listChiavi, NomeRoutine, objParametri)

            StrJoinAnalisiParam.Length = 0
            StrJoinAnalisiParam.AppendLine(" LEFT JOIN Analisi_Dettagli sabbia ON ")
            StrJoinAnalisiParam.AppendLine("     sabbia.Analisi_Testata_Cod = at.Analisi_Testata_Cod ")
            StrJoinAnalisiParam.AppendLine($" AND sabbia.Analisi_Parametro_Cod = {CInt(enum_AnalisiParametri.AnalisiParametri_Sabbia)} ")
            StrJoinAnalisiParam.AppendLine(" LEFT JOIN Analisi_Dettagli limo ON	 ")
            StrJoinAnalisiParam.AppendLine("     limo.Analisi_Testata_Cod = at.Analisi_Testata_Cod ")
            StrJoinAnalisiParam.AppendLine($" AND limo.Analisi_Parametro_Cod = {CInt(enum_AnalisiParametri.AnalisiParametri_Limo)} ")
            StrJoinAnalisiParam.AppendLine(" LEFT JOIN Analisi_Dettagli argilla ON ")
            StrJoinAnalisiParam.AppendLine("     argilla.Analisi_Testata_Cod = at.Analisi_Testata_Cod ")
            StrJoinAnalisiParam.AppendLine($" AND argilla.Analisi_Parametro_Cod = {CInt(enum_AnalisiParametri.AnalisiParametri_Argilla)} ")
            StrJoinAnalisiParam.AppendLine(" -- Mi interessano solo le analisi dove è indicato almeno uno di questi parametri sabbia-limo-argilla ")
            StrJoinAnalisiParam.AppendLine(" WHERE (sabbia.Analisi_Dettaglio_Valore_1 IS NOT NULL ")
            'Il limo non è significativo nel calcolo della tessitura
            'StrJoinAnalisiParam.AppendLine(" OR limo.Analisi_Dettaglio_Valore_1 IS NOT NULL  ")
            StrJoinAnalisiParam.AppendLine(" OR argilla.Analisi_Dettaglio_Valore_1 IS NOT NULL) ")

            StrSelectAnalisiParam.Length = 0
            StrSelectAnalisiParam.AppendLine("   , sabbia.Analisi_Dettaglio_Valore_1 AS sabbia")
            StrSelectAnalisiParam.AppendLine("   , limo.Analisi_Dettaglio_Valore_1 AS limo ")
            StrSelectAnalisiParam.AppendLine("   , argilla.Analisi_Dettaglio_Valore_1 AS argilla ")

            StrSQL.Length = 0
            StrSQL.AppendLine($" DECLARE @annoPratica AS INT = (SELECT anno FROM Pratiche p INNER JOIN uma_richieste_Testata uma ON uma.Pratica_Cod = p.Pratica_Cod WHERE Richiesta_Cod = {Agro_SQL_SaveNum(richiesta_cod)}) ")

            StrSQL.AppendLine(" -- I valori 'vuoti' salvati nelle stringhe chiave su AppezzamentixParticelle e Analisi_EntitaxTestata sono diversi ")
            StrSQL.AppendLine(" -- AppezzamentixParticelle.Sezione e Subalterno DEFAULT = '0' ")
            StrSQL.AppendLine(" -- Analisi_EntitaxTestata.Sezione e Subalterno DEFAULT = '' ")
            StrSQL.AppendLine(" -- Allineo AppezzamentixParticelle a Analisi_EntitaxTestata ")
            StrSQL.AppendLine(" -- Filtro sugli impianti del PCG ")
            StrSQL.AppendLine("  SELECT ")
            StrSQL.AppendLine("      ap.Piva, ap.SA_COD, ap.Appezza, ri.Id_Reg, Prov, Com, Foglio, Numero ")
            StrSQL.AppendLine("    , CASE WHEN Sezione = '0' THEN '' ELSE Sezione END AS Sezione ")
            StrSQL.AppendLine("    , CASE WHEN SUBALTERNO = '0' THEN '' ELSE SUBALTERNO END AS SUBALTERNO ")
            StrSQL.AppendLine("    , AREA ")
            StrSQL.AppendLine(" INTO #AppezzamentixParticelle ")
            StrSQL.AppendLine(" FROM AppezzamentiXParticelle ap ")
            StrSQL.AppendLine(" JOIN #TempImpianto tmp ON ")
            StrSQL.AppendLine("     tmp.PIVA = ap.PIVA COLLATE DATABASE_DEFAULT ")
            StrSQL.AppendLine(" AND tmp.SA_COD = ap.SA_COD ")
            StrSQL.AppendLine(" AND tmp.APPEZZA = ap.APPEZZA ")
            StrSQL.AppendLine(" JOIN Appezzamento a ON ")
            StrSQL.AppendLine("     a.piva = tmp.piva COLLATE DATABASE_DEFAULT ")
            StrSQL.AppendLine(" AND a.SA_COD = tmp.SA_COD ")
            StrSQL.AppendLine(" AND a.APPEZZA = tmp.APPEZZA ")
            StrSQL.AppendLine(" JOIN Reg_Impianti ri ON ")
            StrSQL.AppendLine("     ri.piva = tmp.piva COLLATE DATABASE_DEFAULT ")
            StrSQL.AppendLine(" AND ri.SA_COD = tmp.SA_COD ")
            StrSQL.AppendLine(" AND ri.APPEZZA = tmp.APPEZZA ")
            StrSQL.AppendLine(" AND ri.Id_reg = tmp.Id_reg ")

            StrSQL.AppendLine("")
            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine("      Piva, Sa_Cod, Appezza, Id_Reg, Sup_TotaleImpianto, AreaTessitura, TipoCod, TipoDes, Id_ClasseTessitura, sabbia, limo, argilla  ")
            'StrSQL.AppendLine(" INTO #AnalisiResult ")
            StrSQL.AppendLine(" INTO #AnalisiResult ")
            StrSQL.AppendLine(" FROM ( ")
#Region "1 - ANALISI SU APPEZZAMENTO"
            StrSQL.AppendLine(" --Estraggo le analisi associate agli appezzamenti ")
            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine("     1 AS TipoCod, '1 - ANALISI SU APPEZZAMENTO' AS TipoDes ")
            StrSQL.AppendLine("   , Id_ClasseTessitura, ri.Piva, ri.Sa_Cod, ri.Appezza, ri.Id_Reg, ri.Sup_Imp AS AreaTessitura, ri.Sup_Imp AS Sup_TotaleImpianto ")
            StrSQL.AppendLine(StrSelectAnalisiParam.ToString())
            StrSQL.AppendLine(" FROM Analisi_Testata [at] ")
            StrSQL.AppendLine(" JOIN Analisi_EntitaxTestata aet ON ")
            StrSQL.AppendLine("     aet.Analisi_Testata_Cod = at.Analisi_Testata_Cod ")
            StrSQL.AppendLine($" AND aet.Analisi_Entita_Cod = {CInt(enum_Entita_Analisi.Appezzamento)} -- APPEZZAMENTO ")
            StrSQL.AppendLine(" -- Filtro sugli Appezzamenti passati come parametri ")
            StrSQL.AppendLine(" JOIN #TempImpianto tmp ON ")
            StrSQL.AppendLine("     tmp.PIVA = aet.PIVA COLLATE DATABASE_DEFAULT  ")
            StrSQL.AppendLine(" AND tmp.SA_COD = aet.SA_COD ")
            StrSQL.AppendLine(" AND tmp.APPEZZA = aet.APPEZZA ")
            StrSQL.AppendLine(" JOIN Reg_Impianti ri ON ")
            StrSQL.AppendLine("     ri.PIVA = tmp.PIVA COLLATE DATABASE_DEFAULT ")
            StrSQL.AppendLine(" AND ri.SA_COD = tmp.SA_COD ")
            StrSQL.AppendLine(" AND ri.APPEZZA = tmp.APPEZZA ")
            StrSQL.AppendLine(" AND ri.Id_reg = tmp.Id_reg ")

            StrSQL.AppendLine(StrJoinAnalisiParam.ToString())

            StrSQL.AppendLine($" AND YEAR([at].Analisi_Testata_Data_Inizio) <= @annoPratica AND YEAR([at].Analisi_Testata_Data_Fine) >= @annoPratica ")

#End Region

            StrSQL.AppendLine(" ")
            StrSQL.AppendLine(" UNION ")
            StrSQL.AppendLine(" ")

#Region "2 - ANALISI SU CAMPO"
            StrSQL.AppendLine(" --Estraggo le analisi associate ai campi x appezzamenti ")
            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine("   2 AS TipoCod, '2 - ANALISI SU CAMPO' AS TipoDes  ")
            StrSQL.AppendLine(" , Id_ClasseTessitura, a.Piva, a.Sa_Cod, a.Appezza, ri.id_Reg, ri.Sup_Imp AS AreaTessitura, ri.Sup_Imp AS Sup_TotaleImpianto ")
            StrSQL.AppendLine(StrSelectAnalisiParam.ToString())
            StrSQL.AppendLine(" FROM Analisi_Testata [at] ")
            StrSQL.AppendLine(" JOIN Analisi_EntitaxTestata aet ON ")
            StrSQL.AppendLine("     aet.Analisi_Testata_Cod = at.Analisi_Testata_Cod ")
            StrSQL.AppendLine($" AND aet.Analisi_Entita_Cod = {CInt(enum_Entita_Analisi.Campo)} -- CAMPO ")
            StrSQL.AppendLine(" -- Join sull'appezzamento per estrarre l'area ")
            StrSQL.AppendLine(" JOIN Appezzamento a ON ")
            StrSQL.AppendLine("     a.piva = aet.piva ")
            StrSQL.AppendLine(" AND a.SA_COD = aet.SA_COD ")
            StrSQL.AppendLine(" AND a.Campo_Cod = aet.Campo_Cod ")
            StrSQL.AppendLine(" -- Filtro sugli Appezzamenti passati come parametri ")
            StrSQL.AppendLine(" JOIN #TempImpianto tmp ON ")
            StrSQL.AppendLine("     tmp.PIVA = a.PIVA COLLATE DATABASE_DEFAULT ")
            StrSQL.AppendLine(" AND tmp.SA_COD = a.SA_COD ")
            StrSQL.AppendLine(" AND tmp.APPEZZA = a.APPEZZA ")
            StrSQL.AppendLine(" JOIN Reg_Impianti ri ON ")
            StrSQL.AppendLine("     ri.PIVA = tmp.PIVA COLLATE DATABASE_DEFAULT ")
            StrSQL.AppendLine(" AND ri.SA_COD = tmp.SA_COD ")
            StrSQL.AppendLine(" AND ri.APPEZZA = tmp.APPEZZA ")
            StrSQL.AppendLine(" AND ri.Id_reg = tmp.Id_reg ")

            StrSQL.AppendLine(StrJoinAnalisiParam.ToString())

            StrSQL.AppendLine($" AND YEAR([at].Analisi_Testata_Data_Inizio) <= @annoPratica AND YEAR([at].Analisi_Testata_Data_Fine) >= @annoPratica ")

#End Region

            StrSQL.AppendLine(" ")
            StrSQL.AppendLine(" UNION ")
            StrSQL.AppendLine(" ")

#Region "3 - ANALISI SU CATASTO"
            StrSQL.AppendLine(" --Estraggo le analisi associate alle particelle degli appezzamenti ")
            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine("   3 AS TipoCod, '3 - ANALISI SU CATASTO' AS TipoDes  ")
            StrSQL.AppendLine(" , Id_ClasseTessitura, ap.Piva, ap.Sa_Cod, ap.Appezza, ri.id_Reg, AREA AS AreaTessitura, ri.Sup_Imp AS Sup_TotaleImpianto ")
            StrSQL.AppendLine(StrSelectAnalisiParam.ToString())
            StrSQL.AppendLine(" FROM Analisi_Testata [at] ")
            StrSQL.AppendLine(" JOIN Analisi_EntitaxTestata aet ON ")
            StrSQL.AppendLine("     aet.Analisi_Testata_Cod = at.Analisi_Testata_Cod ")
            StrSQL.AppendLine($" AND aet.Analisi_Entita_Cod = {CInt(enum_Entita_Analisi.Particella)} -- PARTICELLE ")
            StrSQL.AppendLine(" JOIN #AppezzamentixParticelle ap ON ")
            StrSQL.AppendLine("     ap.piva = aet.Piva ")
            StrSQL.AppendLine(" AND ap.SA_COD = aet.Sa_Cod ")
            StrSQL.AppendLine(" AND ap.prov = aet.prov ")
            StrSQL.AppendLine(" AND ap.com = aet.com ")
            StrSQL.AppendLine(" AND ap.sezione = aet.sezione ")
            StrSQL.AppendLine(" AND ap.FOGLIO = aet.FOGLIO ")
            StrSQL.AppendLine(" AND ap.NUMERO = aet.NUMERO ")
            StrSQL.AppendLine(" AND ap.SUBALTERNO = aet.SUBALTERNO ")
            StrSQL.AppendLine(" JOIN Reg_Impianti ri ON ")
            StrSQL.AppendLine(" ri.Piva = ap.Piva ")
            StrSQL.AppendLine(" AND ri.Piva = ap.Piva ")
            StrSQL.AppendLine(" AND ri.Sa_Cod = ap.Sa_Cod ")
            StrSQL.AppendLine(" AND ri.Appezza = ap.Appezza ")
            StrSQL.AppendLine(" AND ri.ID_Reg = ap.ID_Reg ")

            StrSQL.AppendLine(StrJoinAnalisiParam.ToString())

            StrSQL.AppendLine($" AND YEAR([at].Analisi_Testata_Data_Inizio) <= @annoPratica AND YEAR([at].Analisi_Testata_Data_Fine) >= @annoPratica ")

#End Region

            StrSQL.AppendLine(" ")
            StrSQL.AppendLine(" UNION ")
            StrSQL.AppendLine(" ")

#Region "4 - VALORI APPEZZAMENTO"
            StrSQL.AppendLine(" --Estraggo i valori salvati in tabella appezzamento ")
            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine("     4 AS TipoCod, '4 - VALORI APPEZZAMENTO' AS TipoDes ")
            StrSQL.AppendLine("   , Clas, a.Piva, a.Sa_Cod, a.Appezza, ri.id_Reg, ri.Sup_Imp AS AreaTessitura, ri.Sup_Imp AS Sup_TotaleImpianto ")
            StrSQL.AppendLine("   , sabbia, limo, argilla ")
            StrSQL.AppendLine(" FROM Appezzamento a ")
            StrSQL.AppendLine(" -- Filtro sugli Appezzamenti passati come parametri ")
            StrSQL.AppendLine(" JOIN #TempImpianto tmp ON ")
            StrSQL.AppendLine("     tmp.PIVA = a.PIVA COLLATE DATABASE_DEFAULT ")
            StrSQL.AppendLine(" AND tmp.SA_COD = a.SA_COD ")
            StrSQL.AppendLine(" AND tmp.APPEZZA = a.APPEZZA ")
            StrSQL.AppendLine(" JOIN Reg_Impianti ri ON ")
            StrSQL.AppendLine("     ri.PIVA = tmp.PIVA COLLATE DATABASE_DEFAULT ")
            StrSQL.AppendLine(" AND ri.SA_COD = tmp.SA_COD ")
            StrSQL.AppendLine(" AND ri.APPEZZA = tmp.APPEZZA ")
            StrSQL.AppendLine(" AND ri.ID_Reg = tmp.ID_Reg ")

            StrSQL.AppendLine(" WHERE SABBIA IS NOT NULL OR ARGILLA IS NOT NULL ")
#End Region

            StrSQL.AppendLine(" ")
            StrSQL.AppendLine(" UNION ")
            StrSQL.AppendLine(" ")

#Region "5 - ANALISI SU CENTRO"
            StrSQL.AppendLine(" --Estraggo le analisi associate ai campi x appezzamenti ")
            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine("   5 AS TipoCod, '5 - ANALISI SU CENTRO' AS TipoDes  ")
            StrSQL.AppendLine(" , Id_ClasseTessitura, a.Piva, a.Sa_Cod, a.Appezza, ri.id_Reg, ri.Sup_Imp AS AreaTessitura, ri.Sup_Imp AS Sup_TotaleImpianto ")
            StrSQL.AppendLine(StrSelectAnalisiParam.ToString())
            StrSQL.AppendLine(" FROM Analisi_Testata [at] ")
            StrSQL.AppendLine(" JOIN Analisi_EntitaxTestata aet ON ")
            StrSQL.AppendLine("     aet.Analisi_Testata_Cod = at.Analisi_Testata_Cod ")
            StrSQL.AppendLine($" AND aet.Analisi_Entita_Cod = {CInt(enum_Entita_Analisi.Centro)} -- CENTRO ")
            StrSQL.AppendLine(" -- Join sull'appezzamento per estrarre l'area ")
            StrSQL.AppendLine(" JOIN Appezzamento a ON ")
            StrSQL.AppendLine("     a.piva = aet.piva ")
            StrSQL.AppendLine(" AND a.SA_COD = aet.SA_COD ")
            StrSQL.AppendLine(" -- Filtro sugli Appezzamenti passati come parametri ")
            StrSQL.AppendLine(" JOIN #TempImpianto tmp ON ")
            StrSQL.AppendLine("     tmp.PIVA = a.PIVA  COLLATE DATABASE_DEFAULT ")
            StrSQL.AppendLine(" AND tmp.SA_COD = a.SA_COD ")
            StrSQL.AppendLine(" AND tmp.APPEZZA = a.APPEZZA ")
            StrSQL.AppendLine(" JOIN Reg_Impianti ri ON ")
            StrSQL.AppendLine("     ri.PIVA = tmp.PIVA COLLATE DATABASE_DEFAULT ")
            StrSQL.AppendLine(" AND ri.SA_COD = tmp.SA_COD ")
            StrSQL.AppendLine(" AND ri.APPEZZA = tmp.APPEZZA ")
            StrSQL.AppendLine(" AND ri.ID_Reg = tmp.ID_Reg ")

            StrSQL.AppendLine(StrJoinAnalisiParam.ToString())

            StrSQL.AppendLine($" AND YEAR([at].Analisi_Testata_Data_Inizio) <= @annoPratica AND YEAR([at].Analisi_Testata_Data_Fine) >= @annoPratica ")

#End Region

            StrSQL.AppendLine(" ")
            StrSQL.AppendLine(" UNION ")
            StrSQL.AppendLine(" ")

#Region "99 - No Tessitura"
            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine("     99 AS TipoCod, '99 - No Tessitura' AS TipoDes ")
            StrSQL.AppendLine("   , 0 AS Id_ClasseTessitura, a.Piva, a.Sa_Cod, a.Appezza, ri.id_Reg, ri.Sup_Imp AS AreaTessitura, ri.Sup_Imp AS Sup_TotaleImpianto ")
            StrSQL.AppendLine("   , NULL AS sabbia, NULL AS limo, NULL AS argilla ")
            StrSQL.AppendLine(" FROM Appezzamento a ")
            StrSQL.AppendLine(" JOIN #TempImpianto tmp ON ")
            StrSQL.AppendLine("     tmp.PIVA = a.PIVA COLLATE DATABASE_DEFAULT ")
            StrSQL.AppendLine(" AND tmp.SA_COD = a.SA_COD ")
            StrSQL.AppendLine(" AND tmp.APPEZZA = a.APPEZZA ")
            StrSQL.AppendLine(" JOIN Reg_Impianti ri ON ")
            StrSQL.AppendLine("     ri.PIVA = tmp.PIVA COLLATE DATABASE_DEFAULT ")
            StrSQL.AppendLine(" AND ri.SA_COD = tmp.SA_COD ")
            StrSQL.AppendLine(" AND ri.APPEZZA = tmp.APPEZZA ")
            StrSQL.AppendLine(" AND ri.ID_Reg = tmp.ID_Reg ")

#End Region
            StrSQL.AppendLine(" ) AS AnalisiResult ")

            StrSQL.AppendLine(" --Prendo una sola analisi per appezzamento assieme a tutte le analisi sulle particelle associate ")
            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine("    Piva, SA_COD, Appezza, Id_Reg, MIN(TipoCod) AS TipoCod")
            'StrSQL.AppendLine(" INTO #AnalisiRilevanti ")
            StrSQL.AppendLine(" INTO #AnalisiRilevanti ")
            StrSQL.AppendLine(" FROM #AnalisiResult ")
            StrSQL.AppendLine(" GROUP BY piva, SA_COD, appezza, id_Reg, Sup_TotaleImpianto, AreaTessitura ")

            StrSQL.AppendLine(" --Ritorno la media dei valori delle analisi per analisi, per tipo ")
            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine("   a.Piva, a.Sa_Cod, a.Appezza, a.Id_Reg, AreaTessitura, Sup_TotaleImpianto ")
            StrSQL.AppendLine(" , a.TipoCod, TipoDes")
            StrSQL.AppendLine(" , CASE WHEN MIN(Id_ClasseTessitura) <> MAX(Id_ClasseTessitura) THEN NULL ELSE MIN(Id_ClasseTessitura) END AS Id_ClasseTessitura")
            StrSQL.AppendLine(" , AVG(sabbia) AS sabbiaAvg, AVG(limo) AS limoAvg, AVG(argilla) AS ArgillaAvg ")
            StrSQL.AppendLine(" FROM #AnalisiResult a ")
            StrSQL.AppendLine(" JOIN #AnalisiRilevanti ar ON ")
            StrSQL.AppendLine("     ar.PIVA = a.piva ")
            StrSQL.AppendLine(" AND ar.SA_COD = a.SA_COD  ")
            StrSQL.AppendLine(" AND ar.APPEZZA = a.APPEZZA  ")
            StrSQL.AppendLine(" AND ar.ID_REG = a.ID_REG   ")
            StrSQL.AppendLine(" AND ar.TipoCod = a.TipoCod ")
            StrSQL.AppendLine(" GROUP BY a.TipoCod, TipoDes, a.PIVA, a.Sa_Cod, a.Appezza, a.Id_Reg, AreaTessitura, Sup_TotaleImpianto ")
            StrSQL.AppendLine(" ORDER BY a.piva, a.Sa_Cod, a.Appezza, a.Id_Reg, a.TipoCod ")


            StrSQL.AppendLine(" DROP TABLE #AppezzamentixParticelle ")
            StrSQL.AppendLine(" DROP TABLE #AnalisiResult ")
            StrSQL.AppendLine(" DROP TABLE #AnalisiRilevanti ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '-------------------------------------------------------------------------

            ' Eliminazione tabella temporanea
            TempChiaviMassivo.EliminaTabellaTemp_FiltroImpianti(NomeRoutine, objParametri)

            'commit transazione
            ConnessioniTransazioni.ChiudiTransazione(1, objParametri)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            'rollback transazione
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        Finally
            ConnessioniTransazioni.ChiudiConnessione(objParametri)
        End Try

        Return DT

    End Function

End Class



'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

Public Class Analisi_EntitaxTestata_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '#########################################################################
    Public Function Scrivi(
                                ByVal Analisi_Testata_Cod As Integer,
                                ByVal Analisi_Entita_Cod As Integer,
                                ByVal Piva As String, ByVal Sa_Cod As Integer,
                                ByVal Campo_Cod As Integer, ByVal Appezza As Integer,
                                ByVal Id_Imp As Integer, ByVal Fabbricato_Cod As Integer,
                                ByVal PROV As String, ByVal COM As String,
                                ByVal SEZIONE As String, ByVal FOGLIO As Integer,
                                ByVal NUMERO As Integer, ByVal SUBALTERNO As String,
                                ByVal Id_Oggetto_Grafico As String,
                                ByVal DataLock As Integer,
                                ByVal UserName_Creazione As String,
                                ByVal Data_Creazione As Date,
                                ByVal Data_Agg As Date,
                                ByVal Validita_Inizio As Date,
                                ByVal Validita_Fine As Date,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Analisi_EntitaxTestata_W.Scrivi()"

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



            StrSQL.Append("INSERT INTO Analisi_EntitaxTestata( ")
            StrSQL.Append("            Analisi_SuperUser,        Analisi_Testata_Cod,   ")
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
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Analisi_Testata_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Analisi_Entita_Cod) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Campo_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Appezza) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Imp) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Fabbricato_Cod) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(PROV) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(COM) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(SEZIONE) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(FOGLIO) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(NUMERO) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(SUBALTERNO) & "' ")

            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Id_Oggetto_Grafico) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(DataLock) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_Agg) & "  ")
            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(CDate(Now)))
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_Creazione) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_Creazione) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(UserName_Creazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(UserName_Creazione) & "' ")
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
    Public Function Scrivi2(
                                ByVal Analisi_Testata_Cod As Integer,
                                ByVal Analisi_Entita_Cod As Integer,
                                ByVal Piva As String, ByVal Sa_Cod As Integer,
                                ByVal Campo_Cod As Integer, ByVal Appezza As Integer,
                                ByVal Id_Imp As Integer, ByVal Fabbricato_Cod As Integer,
                                ByVal PROV As String, ByVal COM As String,
                                ByVal SEZIONE As String, ByVal FOGLIO As Integer,
                                ByVal NUMERO As Integer, ByVal SUBALTERNO As String,
                                ByVal Id_Oggetto_Grafico As String,
                                ByVal Vas_Cod As Integer,
                                ByVal DataLock As Integer,
                                    ByVal Validita_Inizio As Date,
                                    ByVal Validita_Fine As Date,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Analisi_EntitaxTestata_W.Scrivi2()"

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


            StrSQL.Append("INSERT INTO Analisi_EntitaxTestata( ")
            StrSQL.Append("            Analisi_SuperUser,        Analisi_Testata_Cod,   ")
            StrSQL.Append("            Analisi_Entita_Cod,    ")
            StrSQL.Append("            Piva,        Sa_Cod,         Campo_Cod,      Appezza,    ")
            StrSQL.Append("            Id_Imp,      Fabbricato_Cod, Prov,           Com,        ")
            StrSQL.Append("            Sezione,     Foglio,         Numero,         Subalterno, ")
            StrSQL.Append("            Id_Oggetto_Grafico,          Vas_Cod,        DataLock,       Data_Agg,    ")


            StrSQL.Append("            Inviato, DataInvio, ")
            StrSQL.Append("            Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("            UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("            Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("            ) ")

            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Analisi_Testata_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Analisi_Entita_Cod) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Campo_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Appezza) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Imp) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Fabbricato_Cod) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(PROV) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(COM) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(SEZIONE) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(FOGLIO) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(NUMERO) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(SUBALTERNO) & "' ")

            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Id_Oggetto_Grafico) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Vas_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(DataLock) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , NULL ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
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
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Analisi_Testata_Cod">/param>
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
    ''' <param name="DataLock"></param>
    ''' <param name="Validita_Inizio"></param>
    ''' <param name="Validita_Fine"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni] 05/01/2011	Created
    ''' </history>
    Public Function Modifica(
                                ByVal Analisi_Testata_Cod As Integer,
                                ByVal Analisi_Entita_Cod As Integer,
                                ByVal Piva As String,
                                ByVal Sa_Cod As Integer,
                                ByVal Campo_Cod As Integer,
                                ByVal Appezza As Integer,
                                ByVal Id_Imp As Integer,
                                ByVal Fabbricato_Cod As Integer,
                                ByVal PROV As String,
                                ByVal COM As String,
                                ByVal SEZIONE As String,
                                ByVal FOGLIO As Integer,
                                ByVal NUMERO As Integer,
                                ByVal SUBALTERNO As String,
                                ByVal Id_Oggetto_Grafico As String,
                                ByVal DataLock As Integer,
                                ByVal Validita_Inizio As Date,
                                ByVal Validita_Fine As Date,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Analisi_EntitaxTestata_W.Modifica()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        If Analisi_Testata_Cod = 0 Then
            Throw New Exception("Analisi_Testata_Cod = 0 ")
        End If
        Try
            '------------------------------
            'Query per la modifica dei dati                 ' #### CLASSE ####
            StrSQL.Length = 0

            StrSQL.Append("UPDATE Analisi_EntitaxTestata SET ")
            StrSQL.Append("    Data_Agg          = " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("   ,DataLock          = " & Agro_SQL_SaveNum(DataLock) & "  ")
            StrSQL.Append("   ,Inviato           =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append(" WHERE Analisi_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")


            If Analisi_Testata_Cod <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Analisi_Testata_Cod = " & Analisi_Testata_Cod & " ")
            End If

            If Analisi_Entita_Cod <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Analisi_Entita_Cod = " & Analisi_Entita_Cod & " ")
            End If

            If Trim(Piva) <> "" Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Sa_Cod = " & Sa_Cod & " ")
            End If

            If Campo_Cod <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Campo_Cod = " & Campo_Cod & " ")
            End If

            If Appezza <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Appezza = " & Appezza & " ")
            End If

            If Id_Imp <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Id_Imp = " & Id_Imp & " ")
            End If

            If Fabbricato_Cod <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Fabbricato_Cod = " & Fabbricato_Cod & " ")
            End If

            If Trim(PROV) <> "" Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Prov = '" & Agro_SQL_SaveText(PROV) & "'")
            End If

            If Trim(COM) <> "" Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Com = '" & Agro_SQL_SaveText(COM) & "'")
            End If

            If Trim(SEZIONE) <> "" Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Sezione = '" & Agro_SQL_SaveText(SEZIONE) & "'")
            End If

            If NUMERO <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Numero = " & NUMERO & " ")
            End If

            If FOGLIO <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Foglio = " & FOGLIO & " ")
            End If

            If Trim(SUBALTERNO) <> "" Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Subalterno = '" & Agro_SQL_SaveText(SUBALTERNO) & "'")
            End If

            If Trim(Id_Oggetto_Grafico) <> "" Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Id_Oggetto_Grafico = '" & Agro_SQL_SaveText(Id_Oggetto_Grafico) & "'")
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
    Public Function Modifica2(
                                ByVal Analisi_Testata_Cod As Integer,
                                ByVal Analisi_Entita_Cod As Integer,
                                ByVal Piva As String,
                                ByVal Sa_Cod As Integer,
                                ByVal Campo_Cod As Integer,
                                ByVal Appezza As Integer,
                                ByVal Id_Imp As Integer,
                                ByVal Fabbricato_Cod As Integer,
                                ByVal PROV As String,
                                ByVal COM As String,
                                ByVal SEZIONE As String,
                                ByVal FOGLIO As Integer,
                                ByVal NUMERO As Integer,
                                ByVal SUBALTERNO As String,
                                ByVal Id_Oggetto_Grafico As String,
                                ByVal Vas_Cod As Integer,
                                ByVal DataLock As Integer,
                                ByVal Validita_Inizio As Date,
                                ByVal Validita_Fine As Date,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Analisi_EntitaxTestata_W.Modifica2()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        If Analisi_Testata_Cod = 0 Then
            Throw New Exception("Analisi_Testata_Cod = 0 ")
        End If
        Try

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append("UPDATE Analisi_EntitaxTestata SET ")

            StrSQL.Append("     Data_Agg          = " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.Append("   , DataLock          = " & Agro_SQL_SaveNum(DataLock) & "  ")
            StrSQL.Append("   , Inviato           =  0 ")
            StrSQL.Append("   , DataInvio         =  Null ")

            StrSQL.Append("   , Data_Modifica     =  " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.Append("   , UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   , Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("   , Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            StrSQL.Append(" WHERE Analisi_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")

            If Analisi_Testata_Cod <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Analisi_Testata_Cod = " & Analisi_Testata_Cod & " ")
            End If

            If Analisi_Entita_Cod <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Analisi_Entita_Cod = " & Analisi_Entita_Cod & " ")
            End If

            If Trim(Piva) <> "" Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Sa_Cod = " & Sa_Cod & " ")
            End If

            If Campo_Cod <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Campo_Cod = " & Campo_Cod & " ")
            End If

            If Appezza <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Appezza = " & Appezza & " ")
            End If

            If Id_Imp <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Id_Imp = " & Id_Imp & " ")
            End If

            If Fabbricato_Cod <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Fabbricato_Cod = " & Fabbricato_Cod & " ")
            End If

            If Trim(PROV) <> "" Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Prov = '" & Agro_SQL_SaveText(PROV) & "'")
            End If

            If Trim(COM) <> "" Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Com = '" & Agro_SQL_SaveText(COM) & "'")
            End If

            If Trim(SEZIONE) <> "" Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Sezione = '" & Agro_SQL_SaveText(SEZIONE) & "'")
            End If

            If NUMERO <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Numero = " & NUMERO & " ")
            End If

            If FOGLIO <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Foglio = " & FOGLIO & " ")
            End If

            If Trim(SUBALTERNO) <> "" Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Subalterno = '" & Agro_SQL_SaveText(SUBALTERNO) & "'")
            End If

            If Trim(Id_Oggetto_Grafico) <> "" Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Id_Oggetto_Grafico = '" & Agro_SQL_SaveText(Id_Oggetto_Grafico) & "'")
            End If

            If Vas_Cod <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Vas_Cod = " & Vas_Cod & " ")
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
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Analisi_Testata_Cod">/param>
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
    Public Function Cancella(
                                ByVal Analisi_Testata_Cod As Integer,
                                ByVal Analisi_Entita_Cod As Integer,
                                ByVal Piva As String,
                                ByVal Sa_Cod As Integer,
                                ByVal Campo_Cod As Integer,
                                ByVal Appezza As Integer,
                                ByVal Id_Imp As Integer,
                                ByVal Fabbricato_Cod As Integer,
                                ByVal PROV As String,
                                ByVal COM As String,
                                ByVal SEZIONE As String,
                                ByVal FOGLIO As Integer,
                                ByVal NUMERO As Integer,
                                ByVal SUBALTERNO As String,
                                ByVal Id_Oggetto_Grafico As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Analisi_EntitaxTestata_W.Cancella()"

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
            Throw New Exception("Analisi_Testata_Cod = 0 ")
        End If
        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0

                StrSQL.Append(" UPDATE  Analisi_EntitaxTestata ")
                StrSQL.Append(" SET ")
                StrSQL.Append("          Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         ,Data_Modifica = " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE Analisi_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append("   AND    Inviato > 0 ")

            Else

                StrSQL.Length = 0

                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     Analisi_EntitaxTestata ")
                StrSQL.Append(" WHERE    Analisi_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append("   AND    Inviato = 0 ")

            End If


            If Analisi_Testata_Cod <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Analisi_Testata_Cod = " & Analisi_Testata_Cod & " ")
            End If

            If Analisi_Entita_Cod <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Analisi_Entita_Cod = " & Analisi_Entita_Cod & " ")
            End If

            If Trim(Piva) <> "" Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Sa_Cod = " & Sa_Cod & " ")
            End If

            If Campo_Cod <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Campo_Cod = " & Campo_Cod & " ")
            End If

            If Appezza <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Appezza = " & Appezza & " ")
            End If

            If Id_Imp <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Id_Imp = " & Id_Imp & " ")
            End If

            If Fabbricato_Cod <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Fabbricato_Cod = " & Fabbricato_Cod & " ")
            End If

            If Trim(PROV) <> "" Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Prov = '" & Agro_SQL_SaveText(PROV) & "'")
            End If

            If Trim(COM) <> "" Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Com = '" & Agro_SQL_SaveText(COM) & "'")
            End If

            If Trim(SEZIONE) <> "" Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Sezione = '" & Agro_SQL_SaveText(SEZIONE) & "'")
            End If

            If NUMERO <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Numero = " & NUMERO & " ")
            End If

            If FOGLIO <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Foglio = " & FOGLIO & " ")
            End If

            If Trim(SUBALTERNO) <> "" Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Subalterno = '" & Agro_SQL_SaveText(SUBALTERNO) & "'")
            End If

            If Trim(Id_Oggetto_Grafico) <> "" Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Id_Oggetto_Grafico = '" & Agro_SQL_SaveText(Id_Oggetto_Grafico) & "'")
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
    Public Function Cancella2(
                                ByVal Analisi_Testata_Cod As Integer,
                                ByVal Analisi_Entita_Cod As Integer,
                                ByVal Piva As String,
                                ByVal Sa_Cod As Integer,
                                ByVal Campo_Cod As Integer,
                                ByVal Appezza As Integer,
                                ByVal Id_Imp As Integer,
                                ByVal Fabbricato_Cod As Integer,
                                ByVal PROV As String,
                                ByVal COM As String,
                                ByVal SEZIONE As String,
                                ByVal FOGLIO As Integer,
                                ByVal NUMERO As Integer,
                                ByVal SUBALTERNO As String,
                                ByVal Id_Oggetto_Grafico As String,
                                ByVal Vas_Cod As Integer,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Analisi_EntitaxTestata_W.Cancella2()"

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
            Throw New Exception("Analisi_Testata_Cod = 0 ")
        End If
        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0

                StrSQL.Append(" UPDATE  Analisi_EntitaxTestata ")
                StrSQL.Append(" SET ")
                StrSQL.Append("          Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         ,Data_Modifica = " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE Analisi_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append("   AND    Inviato > 0 ")

            Else

                StrSQL.Length = 0

                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     Analisi_EntitaxTestata ")
                StrSQL.Append(" WHERE    Analisi_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append("   AND    Inviato = 0 ")

            End If


            If Analisi_Testata_Cod <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Analisi_Testata_Cod = " & Analisi_Testata_Cod & " ")
            End If

            If Analisi_Entita_Cod <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Analisi_Entita_Cod = " & Analisi_Entita_Cod & " ")
            End If

            If Trim(Piva) <> "" Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Sa_Cod = " & Sa_Cod & " ")
            End If

            If Campo_Cod <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Campo_Cod = " & Campo_Cod & " ")
            End If

            If Appezza <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Appezza = " & Appezza & " ")
            End If

            If Id_Imp <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Id_Imp = " & Id_Imp & " ")
            End If

            If Fabbricato_Cod <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Fabbricato_Cod = " & Fabbricato_Cod & " ")
            End If

            If Trim(PROV) <> "" Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Prov = '" & Agro_SQL_SaveText(PROV) & "'")
            End If

            If Trim(COM) <> "" Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Com = '" & Agro_SQL_SaveText(COM) & "'")
            End If

            If Trim(SEZIONE) <> "" Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Sezione = '" & Agro_SQL_SaveText(SEZIONE) & "'")
            End If

            If NUMERO <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Numero = " & NUMERO & " ")
            End If

            If FOGLIO <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Foglio = " & FOGLIO & " ")
            End If

            If Trim(SUBALTERNO) <> "" Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Subalterno = '" & Agro_SQL_SaveText(SUBALTERNO) & "'")
            End If

            If Trim(Id_Oggetto_Grafico) <> "" Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Id_Oggetto_Grafico = '" & Agro_SQL_SaveText(Id_Oggetto_Grafico) & "'")
            End If

            If Vas_Cod <> 0 Then
                StrSQL.Append(" AND Analisi_EntitaxTestata.Vas_Cod = " & Vas_Cod & " ")
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

    Public Function CancellaEntitaXTestata(ByVal Analisi_Testata_Cod As Integer,
                                           ByVal Analisi_Entita_Cod As Integer,
                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                           ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Analisi_EntitaxTestata_W.CancellaEntitaXTestata()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        If Analisi_Testata_Cod = 0 Then
            Throw New Exception("Analisi_Testata_Cod = 0 ")
        End If
        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" DELETE ")
            StrSQL.AppendLine(" FROM Analisi_EntitaxTestata ")
            StrSQL.AppendLine(" WHERE Analisi_SuperUser = '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "' ")

            If Analisi_Testata_Cod <> 0 Then
                StrSQL.AppendLine(" AND Analisi_EntitaxTestata.Analisi_Testata_Cod = " & Agro_SQL_SaveNum(Analisi_Testata_Cod) & " ")
            End If

            If Analisi_Entita_Cod <> 0 Then
                StrSQL.AppendLine(" AND Analisi_EntitaxTestata.Analisi_Entita_Cod = " & Agro_SQL_SaveNum(Analisi_Entita_Cod) & " ")
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
