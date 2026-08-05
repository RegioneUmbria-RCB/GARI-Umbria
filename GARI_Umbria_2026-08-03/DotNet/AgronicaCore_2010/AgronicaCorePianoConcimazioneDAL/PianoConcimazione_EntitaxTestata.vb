Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class PianoConcimazione_EntitaxTestata_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '  Galassi, 24/02/2017 11.48.59: Copiato dai Core dell'anagrafe
    '##############################################################################################
    Public Function Leggi_Default(ByVal PC_Testata_Cod As Integer, _
                            ByVal PC_Entita_Cod As Integer, _
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
                            ByVal Progetto_Cod As Integer, _
                            ByVal Id_Oggetto_GRafico As String, _
                            ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                            ByVal xFiltroAggiuntivo As String, _
                            ByVal xOrderBy As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PianoConcimazione_EntitaxTestata_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT * " & vbCrLf)
                    StrSQL.Append(" FROM    PianoConcimazione_EntitaxTestata " & vbCrLf)
                    StrSQL.Append(" WHERE   Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " & vbCrLf)
                    StrSQL.Append(" AND     Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " " & vbCrLf)
                    StrSQL.Append(" AND     PC_SuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser) & " " & vbCrLf)

                    If PC_Testata_Cod <> 0 Then
                        StrSQL.Append(" AND PC_Testata_Cod = " & Agro_SQL_SaveNum(PC_Testata_Cod) & " " & vbCrLf)
                    End If

                    If PC_Entita_Cod <> 0 Then
                        StrSQL.Append(" AND PC_Entita_Cod = " & Agro_SQL_SaveNum(PC_Entita_Cod) & " " & vbCrLf)
                    End If

                    If Piva <> "" Then
                        StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " " & vbCrLf)
                    End If

                    If Campo_Cod <> 0 Then
                        StrSQL.Append(" AND Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod) & " " & vbCrLf)
                    End If

                    If Appezza <> 0 Then
                        StrSQL.Append(" AND Appezza = " & Agro_SQL_SaveNum(Appezza) & " " & vbCrLf)
                    End If

                    If Id_Imp <> 0 Then
                        StrSQL.Append(" AND Id_Imp = " & Agro_SQL_SaveNum(Id_Imp) & " " & vbCrLf)
                    End If

                    If Fabbricato_Cod <> 0 Then
                        StrSQL.Append(" AND Fabbricato_Cod = " & Agro_SQL_SaveNum(Fabbricato_Cod) & " " & vbCrLf)
                    End If

                    If Prov <> "" Then
                        StrSQL.Append(" AND Prov = '" & Agro_SQL_SaveText(Prov) & "' " & vbCrLf)
                    End If

                    If Com <> "" Then
                        StrSQL.Append(" AND Com = '" & Agro_SQL_SaveText(Com) & "' " & vbCrLf)
                    End If

                    If Sezione <> "" Then
                        StrSQL.Append(" AND Sezione = '" & Agro_SQL_SaveText(Sezione) & "' " & vbCrLf)
                    End If

                    If Foglio <> 0 Then
                        StrSQL.Append(" AND Foglio = " & Agro_SQL_SaveNum(Foglio) & " " & vbCrLf)
                    End If

                    If Numero <> 0 Then
                        StrSQL.Append(" AND Numero = " & Agro_SQL_SaveNum(Numero) & " " & vbCrLf)
                    End If

                    If Subalterno <> "" Then
                        StrSQL.Append(" AND Subalterno = '" & Agro_SQL_SaveText(Subalterno) & "' " & vbCrLf)
                    End If

                    If Id_Oggetto_GRafico <> "" Then
                        StrSQL.Append(" AND Id_Oggetto_GRafico = '" & Agro_SQL_SaveText(Id_Oggetto_GRafico) & "' " & vbCrLf)
                    End If

                    If Progetto_Cod <> 0 Then
                        StrSQL.Append(" AND Progetto_Cod = " & Agro_SQL_SaveNum(Progetto_Cod) & " " & vbCrLf)
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & vbCrLf)
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
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)" & vbCrLf)
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT *, (select [PC_Testata_Des] from PianoConcimazione_Testata where PC_Testata_Cod = PianoConcimazione_EntitaxTestata.PC_Testata_Cod) as PC_Testata_Des " & vbCrLf)
                    StrSQL.Append(" FROM    PianoConcimazione_EntitaxTestata " & vbCrLf)
                    StrSQL.Append(" WHERE   Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " & vbCrLf)
                    StrSQL.Append(" AND     Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " " & vbCrLf)
                    StrSQL.Append(" AND     PC_SuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser) & " " & vbCrLf)

                    If PC_Testata_Cod <> 0 Then
                        StrSQL.Append(" AND PC_Testata_Cod = " & Agro_SQL_SaveNum(PC_Testata_Cod) & " " & vbCrLf)
                    End If

                    If PC_Entita_Cod <> 0 Then
                        StrSQL.Append(" AND PC_Entita_Cod = " & Agro_SQL_SaveNum(PC_Entita_Cod) & " " & vbCrLf)
                    End If

                    If Piva <> "" Then
                        StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " " & vbCrLf)
                    End If

                    If Campo_Cod <> 0 Then
                        StrSQL.Append(" AND Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod) & " " & vbCrLf)
                    End If

                    If Appezza <> 0 Then
                        StrSQL.Append(" AND Appezza = " & Agro_SQL_SaveNum(Appezza) & " " & vbCrLf)
                    End If

                    If Id_Imp <> 0 Then
                        StrSQL.Append(" AND Id_Imp = " & Agro_SQL_SaveNum(Id_Imp) & " " & vbCrLf)
                    End If

                    If Fabbricato_Cod <> 0 Then
                        StrSQL.Append(" AND Fabbricato_Cod = " & Agro_SQL_SaveNum(Fabbricato_Cod) & " " & vbCrLf)
                    End If

                    If Prov <> "" Then
                        StrSQL.Append(" AND Prov = '" & Agro_SQL_SaveText(Prov) & "' " & vbCrLf)
                    End If

                    If Com <> "" Then
                        StrSQL.Append(" AND Com = '" & Agro_SQL_SaveText(Com) & "' " & vbCrLf)
                    End If

                    If Sezione <> "" Then
                        StrSQL.Append(" AND Sezione = '" & Agro_SQL_SaveText(Sezione) & "' " & vbCrLf)
                    End If

                    If Foglio <> 0 Then
                        StrSQL.Append(" AND Foglio = " & Agro_SQL_SaveNum(Foglio) & " " & vbCrLf)
                    End If

                    If Numero <> 0 Then
                        StrSQL.Append(" AND Numero = " & Agro_SQL_SaveNum(Numero) & " " & vbCrLf)
                    End If

                    If Subalterno <> "" Then
                        StrSQL.Append(" AND Subalterno = '" & Agro_SQL_SaveText(Subalterno) & "' " & vbCrLf)
                    End If

                    If Id_Oggetto_GRafico <> "" Then
                        StrSQL.Append(" AND Id_Oggetto_GRafico = '" & Agro_SQL_SaveText(Id_Oggetto_GRafico) & "' " & vbCrLf)
                    End If

                    If Progetto_Cod <> 0 Then
                        StrSQL.Append(" AND Progetto_Cod = " & Agro_SQL_SaveNum(Progetto_Cod) & " " & vbCrLf)
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & vbCrLf)
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
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)" & vbCrLf)
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


    '===========================================================================
    Public Function Distinct_TestataCod_Piva(ByVal xFiltroAggiuntivo As String, _
                                            ByVal xOrderBy As String, _
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                            ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PianoConcimazione_EntitaxTestata_R.Distinct_TestataCod_Piva()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" SELECT DISTINCT PC_SuperUser, PC_Testata_Cod, Piva")
            StrSQL.Append(" FROM   PianoConcimazione_EntitaxTestata ")
            StrSQL.Append(" WHERE  Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND    Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.Append(" AND    PC_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")

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
                StrSQL.Append(" ORDER BY PC_SuperUser, PC_Testata_Cod, Piva")
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
    Public Function Leggi_Tutte_Piva_Che_Hanno_PianoConcimazione(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                                 ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PianoConcimazione_EntitaxTestata.Leggi_Tutte_Piva_Che_Hanno_PianoConcimazione()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("  select distinct piva from PianoConcimazione_EntitaxTestata ")

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

    '  Monti, 24/02/2017 11.49.45: Creato nuovo
    '##########################################################################################################
    Public Function Leggi( _
            ByVal PC_Testata_Cod As Int32, _
            ByVal xFiltroAggiuntivo As String, _
            ByVal xOrderBy As String, _
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCorePianoConcimazioneDAL.PianoConcimazione_EntitaxTestata_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            If objParametri.PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva_Superuser obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  PianoConcimazione_EntitaxTestata ")
            StrSQL.Append(" WHERE   (PC_SuperUser = '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "')  ")

            If PC_Testata_Cod <> 0 Then
                StrSQL.Append(" AND     PC_Testata_Cod = " & Agro_SQL_SaveNum(PC_Testata_Cod) & " ")
            End If

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

'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################

'  Galassi, 24/02/2017 11.51.25: Copiato dai core dell'aANAgrafe
Public Class PianoConcimazione_EntitaxTestata_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    'i default per le chiavi degli elementi anagrafici sono: -99999
    Public Function Cancella(ByVal PC_Testata_Cod As Integer, _
                                ByVal PC_Entita_Cod As Integer, _
                                ByVal Piva As String, _
                                ByVal Sa_Cod As Integer, _
                                ByVal Appezza As Integer, _
                                ByVal Id_Imp As Integer, _
                                ByVal Fabbricato_Cod As Integer, _
                                ByVal Campo_Cod As Integer, _
                                ByVal Prov As String, _
                                ByVal Com As String, _
                                ByVal Sezione As String, _
                                ByVal Foglio As Integer, _
                                ByVal Numero As Integer, _
                                ByVal Subalterno As String, _
                                ByVal Progetto_Cod As Integer, _
                                ByVal ID_Oggetto_Grafico As String, _
                                ByVal xFiltroAggiuntivo As String, _
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                   ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PianoConcimazione_EntitaxTestata_W.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If PC_Testata_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (PC_Testata_Cod obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE PianoConcimazione_EntitaxTestata ")
                StrSQL.Append(" SET ")
                StrSQL.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE   PC_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND     PC_Testata_Cod = " & Agro_SQL_SaveNum(PC_Testata_Cod) & " ")
                StrSQL.Append(" AND     Inviato >= 0 ")

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM    PianoConcimazione_EntitaxTestata ")
                StrSQL.Append(" WHERE   PC_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND     PC_Testata_Cod = " & Agro_SQL_SaveNum(PC_Testata_Cod) & " ")

            End If

            If PC_Entita_Cod <> 0 Then
                StrSQL.Append(" AND     (PianoConcimazione_EntitaxTestata.PC_Entita_Cod = " & Agro_SQL_SaveNum(PC_Entita_Cod) & ")  " & vbCrLf)
            End If

            If Piva <> "" Then
                StrSQL.Append(" AND     (PianoConcimazione_EntitaxTestata.Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "')  " & vbCrLf)
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND     (PianoConcimazione_EntitaxTestata.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & ")  " & vbCrLf)
            End If

            If Campo_Cod <> 0 Then
                StrSQL.Append(" AND     (PianoConcimazione_EntitaxTestata.Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod) & ")  " & vbCrLf)
            End If

            If Appezza <> 0 Then
                StrSQL.Append(" AND     (PianoConcimazione_EntitaxTestata.Appezza = " & Agro_SQL_SaveNum(Appezza) & ")  " & vbCrLf)
            End If

            If Id_Imp <> 0 Then
                StrSQL.Append(" AND     (PianoConcimazione_EntitaxTestata.Id_Imp = " & Agro_SQL_SaveNum(Id_Imp) & ")  " & vbCrLf)
            End If

            If Fabbricato_Cod <> 0 Then
                StrSQL.Append(" AND     (PianoConcimazione_EntitaxTestata.Fabbricato_Cod = " & Agro_SQL_SaveNum(Fabbricato_Cod) & ")  " & vbCrLf)
            End If

            If Prov <> "" Then
                StrSQL.Append(" AND     (PianoConcimazione_EntitaxTestata.Prov = '" & Agro_SQL_SaveText(Prov) & "') " & vbCrLf)
            End If

            If Com <> "" Then
                StrSQL.Append(" AND     (PianoConcimazione_EntitaxTestata.Com = '" & Agro_SQL_SaveText(Com) & "')  " & vbCrLf)
            End If

            If Sezione <> "" Then
                StrSQL.Append(" AND     (PianoConcimazione_EntitaxTestata.Sezione = '" & Agro_SQL_SaveText(Sezione) & "')  " & vbCrLf)
            End If

            If Foglio <> 0 Then
                StrSQL.Append(" AND     (PianoConcimazione_EntitaxTestata.Foglio = " & Agro_SQL_SaveNum(Foglio) & ")  " & vbCrLf)
            End If

            If Numero <> 0 Then
                StrSQL.Append(" AND     (PianoConcimazione_EntitaxTestata.Numero = " & Agro_SQL_SaveNum(Numero) & ")  " & vbCrLf)
            End If

            If Subalterno <> "" Then
                StrSQL.Append(" AND     (PianoConcimazione_EntitaxTestata.Subalterno = '" & Agro_SQL_SaveText(Subalterno) & "')  " & vbCrLf)
            End If

            If ID_Oggetto_Grafico <> "" Then
                StrSQL.Append(" AND     (PianoConcimazione_EntitaxTestata.ID_Oggetto_Grafico = '" & Agro_SQL_SaveText(ID_Oggetto_Grafico) & "')" & vbCrLf)
            End If

            If Progetto_Cod <> 0 Then
                StrSQL.Append(" AND     (PianoConcimazione_EntitaxTestata.Progetto_Cod = " & Agro_SQL_SaveNum(Progetto_Cod) & ")  " & vbCrLf)
            End If

            '---------------------------------------------
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


    '##############################################################################################
    Public Function Scrivi(ByVal PC_Testata_Cod As Integer, _
                            ByVal PC_Entita_Cod As enum_Entita_PianoConcimazione, _
                            ByVal Piva As String, _
                            ByVal Sa_Cod As Integer, _
                            ByVal Appezza As Integer, _
                            ByVal Id_Imp As Integer, _
                            ByVal Fabbricato_Cod As Integer, _
                            ByVal Campo_Cod As Integer, _
                            ByVal Prov As String, _
                            ByVal Com As String, _
                            ByVal Sezione As String, _
                            ByVal Foglio As Integer, _
                            ByVal Numero As Integer, _
                            ByVal Subalterno As String, _
                            ByVal Progetto_Cod As Integer, _
                            ByVal ID_Oggetto_Grafico As String, _
                            ByVal Chiave_Albero_Imprese As String, _
                            ByVal QtaMaxN As Decimal, _
                            ByVal QtaMaxP2O5 As Decimal, _
                            ByVal QtaMaxK2O As Decimal, _
                            ByVal Validita_Inizio As Date, _
                            ByVal Validita_Fine As Date, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PianoConcimazione_EntitaxTestata_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If PC_Entita_Cod = enum_Entita_PianoConcimazione.NonDefinito Then
                PC_Entita_Cod = AgronicaCoreDataProvider.Albero.PCEntitaCod_from_ChiaveAlberoImprese(Chiave_Albero_Imprese)
            End If

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" INSERT INTO PianoConcimazione_EntitaxTestata " & vbCrLf)

            StrSQL.Append(" (PC_SuperUser, PC_Testata_Cod, PC_Entita_Cod, " & vbCrLf)
            StrSQL.Append(" Piva, Sa_Cod, Appezza, Id_Imp, Fabbricato_Cod, Campo_Cod, " & vbCrLf)
            StrSQL.Append(" Prov, Com, Sezione, Foglio, Numero, Subalterno, " & vbCrLf)
            StrSQL.Append(" ID_Oggetto_Grafico, Chiave_Albero_Imprese, Progetto_Cod, " & vbCrLf)
            StrSQL.Append(" QtaMaxN, QtaMaxP2O5, QtaMaxK2O, " & vbCrLf)

            StrSQL.Append("              Inviato,            datainvio, " & vbCrLf)
            StrSQL.Append("              Data_Creazione,     Data_Modifica, " & vbCrLf)
            StrSQL.Append("              UserName_Creazione, UserName_Modifica, " & vbCrLf)
            StrSQL.Append("              Validita_Inizio,    Validita_Fine  , DataLock " & vbCrLf)
            StrSQL.Append("              ) " & vbCrLf)

            StrSQL.Append(" VALUES (" & vbCrLf)

            StrSQL.Append(" '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "', " & vbCrLf)
            StrSQL.Append("" & Agro_SQL_SaveNum(Trim(PC_Testata_Cod)) & ", " & vbCrLf)
            StrSQL.Append("" & Agro_SQL_SaveNum(Trim(PC_Entita_Cod)) & ", " & vbCrLf)
            StrSQL.Append("'" & Agro_SQL_SaveText(Trim(Piva)) & "', " & vbCrLf)
            StrSQL.Append("" & Agro_SQL_SaveNum(Trim(Sa_Cod)) & ", " & vbCrLf)
            StrSQL.Append("" & Agro_SQL_SaveNum(Trim(Appezza)) & ", " & vbCrLf)
            StrSQL.Append("" & Agro_SQL_SaveNum(Trim(Id_Imp)) & ", " & vbCrLf)
            StrSQL.Append("" & Agro_SQL_SaveNum(Trim(Fabbricato_Cod)) & ", " & vbCrLf)
            StrSQL.Append("" & Agro_SQL_SaveNum(Trim(Campo_Cod)) & ", " & vbCrLf)
            StrSQL.Append("'" & Agro_SQL_SaveText(Trim(Prov)) & "', " & vbCrLf)
            StrSQL.Append("'" & Agro_SQL_SaveText(Trim(Com)) & "', " & vbCrLf)
            StrSQL.Append("'" & Agro_SQL_SaveText(Trim(Sezione)) & "', " & vbCrLf)
            StrSQL.Append("" & Agro_SQL_SaveNum(Trim(Foglio)) & ", " & vbCrLf)
            StrSQL.Append("" & Agro_SQL_SaveNum(Trim(Numero)) & ", " & vbCrLf)
            StrSQL.Append("'" & Agro_SQL_SaveText(Trim(Subalterno)) & "', " & vbCrLf)
            StrSQL.Append("'" & Agro_SQL_SaveText(Trim(ID_Oggetto_Grafico)) & "', " & vbCrLf)
            StrSQL.Append("'" & Agro_SQL_SaveText(Trim(Chiave_Albero_Imprese)) & "', " & vbCrLf)
            StrSQL.Append("" & Agro_SQL_SaveNum(Progetto_Cod) & ", " & vbCrLf)
            StrSQL.Append("" & Agro_SQL_SaveNum(QtaMaxN) & ", " & vbCrLf)
            StrSQL.Append("" & Agro_SQL_SaveNum(QtaMaxP2O5) & ", " & vbCrLf)
            StrSQL.Append("" & Agro_SQL_SaveNum(QtaMaxK2O) & " " & vbCrLf)

            StrSQL.Append("         , 0  " & vbCrLf)
            StrSQL.Append("         , Null  " & vbCrLf)
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  " & vbCrLf)
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  " & vbCrLf)
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' " & vbCrLf)
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' " & vbCrLf)
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  " & vbCrLf)
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  " & vbCrLf)
            StrSQL.Append("         , 0  ")
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

    '##############################################################################################
    Public Function AggiornaQtaMax(ByVal PC_Testata_Cod As Integer, _
                                   ByVal PC_Entita_Cod As enum_Entita_PianoConcimazione, _
                                   ByVal Piva As String, _
                                   ByVal Sa_Cod As Integer, _
                                   ByVal Appezza As Integer, _
                                   ByVal Id_Imp As Integer, _
                                   ByVal Fabbricato_Cod As Integer, _
                                   ByVal Campo_Cod As Integer, _
                                   ByVal Prov As String, _
                                   ByVal Com As String, _
                                   ByVal Sezione As String, _
                                   ByVal Foglio As Integer, _
                                   ByVal Numero As Integer, _
                                   ByVal Subalterno As String, _
                                   ByVal Progetto_Cod As Integer, _
                                   ByVal ID_Oggetto_Grafico As String, _
                                   ByVal Chiave_Albero_Imprese As String, _
                                   ByVal QtaMaxN As Decimal, _
                                   ByVal QtaMaxP2O5 As Decimal, _
                                   ByVal QtaMaxK2O As Decimal, _
                                        ByVal xFiltroAggiuntivo As String, _
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PianoConcimazione_EntitaxTestata_W.AggiornaQtaMax()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If PC_Testata_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (PC_Testata_Cod obbligatorio)")
            End If


            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Length = 0
            StrSQL.Append(" UPDATE PianoConcimazione_EntitaxTestata ")
            StrSQL.Append(" SET ")
            StrSQL.Append("          QtaMaxN =    " & Agro_SQL_SaveNum(QtaMaxN) & " ")
            StrSQL.Append("         ,QtaMaxP2O5 = " & Agro_SQL_SaveNum(QtaMaxP2O5) & " ")
            StrSQL.Append("         ,QtaMaxK2O =  " & Agro_SQL_SaveNum(QtaMaxK2O) & " ")
            StrSQL.Append(" WHERE    PC_Testata_Cod = " & Agro_SQL_SaveNum(PC_Testata_Cod) & " ")
            StrSQL.Append(" AND      PC_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If PC_Entita_Cod <> 0 Then
                StrSQL.Append(" AND     (PianoConcimazione_EntitaxTestata.PC_Entita_Cod = " & Agro_SQL_SaveNum(PC_Entita_Cod) & ")  " & vbCrLf)
            End If

            If Piva <> "" Then
                StrSQL.Append(" AND     (PianoConcimazione_EntitaxTestata.Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "')  " & vbCrLf)
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND     (PianoConcimazione_EntitaxTestata.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & ")  " & vbCrLf)
            End If

            If Campo_Cod <> 0 Then
                StrSQL.Append(" AND     (PianoConcimazione_EntitaxTestata.Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod) & ")  " & vbCrLf)
            End If

            If Appezza <> 0 Then
                StrSQL.Append(" AND     (PianoConcimazione_EntitaxTestata.Appezza = " & Agro_SQL_SaveNum(Appezza) & ")  " & vbCrLf)
            End If

            If Id_Imp <> 0 Then
                StrSQL.Append(" AND     (PianoConcimazione_EntitaxTestata.Id_Imp = " & Agro_SQL_SaveNum(Id_Imp) & ")  " & vbCrLf)
            End If

            If Fabbricato_Cod <> 0 Then
                StrSQL.Append(" AND     (PianoConcimazione_EntitaxTestata.Fabbricato_Cod = " & Agro_SQL_SaveNum(Fabbricato_Cod) & ")  " & vbCrLf)
            End If

            If Prov <> "" Then
                StrSQL.Append(" AND     (PianoConcimazione_EntitaxTestata.Prov = '" & Agro_SQL_SaveText(Prov) & "') " & vbCrLf)
            End If

            If Com <> "" Then
                StrSQL.Append(" AND     (PianoConcimazione_EntitaxTestata.Com = '" & Agro_SQL_SaveText(Com) & "')  " & vbCrLf)
            End If

            If Sezione <> "" Then
                StrSQL.Append(" AND     (PianoConcimazione_EntitaxTestata.Sezione = '" & Agro_SQL_SaveText(Sezione) & "')  " & vbCrLf)
            End If

            If Foglio <> 0 Then
                StrSQL.Append(" AND     (PianoConcimazione_EntitaxTestata.Foglio = " & Agro_SQL_SaveNum(Foglio) & ")  " & vbCrLf)
            End If

            If Numero <> 0 Then
                StrSQL.Append(" AND     (PianoConcimazione_EntitaxTestata.Numero = " & Agro_SQL_SaveNum(Numero) & ")  " & vbCrLf)
            End If

            If Subalterno <> "" Then
                StrSQL.Append(" AND     (PianoConcimazione_EntitaxTestata.Subalterno = '" & Agro_SQL_SaveText(Subalterno) & "')  " & vbCrLf)
            End If

            If ID_Oggetto_Grafico <> "" Then
                StrSQL.Append(" AND     (PianoConcimazione_EntitaxTestata.ID_Oggetto_Grafico = '" & Agro_SQL_SaveText(ID_Oggetto_Grafico) & "')" & vbCrLf)
            End If

            If Progetto_Cod <> 0 Then
                StrSQL.Append(" AND     (PianoConcimazione_EntitaxTestata.Progetto_Cod = " & Agro_SQL_SaveNum(Progetto_Cod) & ")  " & vbCrLf)
            End If

            '---------------------------------------------
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
