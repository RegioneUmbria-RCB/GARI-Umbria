Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider



''' <summary>
''' OK
''' </summary>
''' <remarks></remarks>
Public Class Profilazione_R
    Inherits AgronicaCoreDataProvider.DataProvider



    'costruttore
    Sub New()

    End Sub


    Public Function Leggi(ByVal PIVA_isK As String, _
                          ByVal IdProfiloDati_isK As Integer, _
                            ByVal Codice_Chiave_isK As String, _
                            ByVal IdGruppo_isK As String, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByVal Lav_Cod As Integer, _
                                ByVal Veg_Cod As Integer, _
                                ByVal LeggiSoloNoteConLavorazioneNullSeLAv_CodZero As Boolean, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreProfilazioneDAL.Profilazione.LeggiProfilazioneNote()"



        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------



            StrSQL.Length = 0
            StrSQL.Append(" SELECT PIVA,Id_Profilo_Dati,Codice_Chiave,Quesito,Id_Gruppo,Valore_salvato, Veg_Cod,PivaSuperUser, ISNULL(Lav_Cod, 0) AS Lav_Cod," + vbCrLf)
            StrSQL.Append("inviato,datainvio,Data_Creazione,Data_Modifica,Username_Creazione,Username_Modifica,Validita_Inizio,Validita_Fine " + vbCrLf)

            StrSQL.Append(" FROM Profilazione_Dati (NOLOCK)")
            StrSQL.Append(" WHERE 1=1 ")

            If PIVA_isK <> "0" Then
                StrSQL.Append(" AND PIVA='" & Agro_SQL_SaveText(PIVA_isK) & "'  ")
            End If

            If IdProfiloDati_isK <> 0 Then
                StrSQL.Append(" AND Id_Profilo_Dati= " & Agro_SQL_SaveNum(IdProfiloDati_isK) & "  ")
            End If
            If (Not Codice_Chiave_isK.Equals("")) And (Not Codice_Chiave_isK.Equals("0")) Then
                StrSQL.Append(" AND Codice_chiave='" + Agro_SQL_SaveText(Codice_Chiave_isK) + "' ")
            End If

            If IdGruppo_isK <> "" Then
                StrSQL.Append(" AND Id_Gruppo='" + Agro_SQL_SaveText(IdGruppo_isK) + "'")
            End If

            If LeggiSoloNoteConLavorazioneNullSeLAv_CodZero Then
                If Lav_Cod <> 0 Then
                    StrSQL.Append(" AND Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & " ")
                Else
                    StrSQL.Append(" AND Lav_Cod  is null  ")
                End If
                If Veg_Cod <> 0 Then
                    StrSQL.Append(" AND Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
                End If
            Else
                If Lav_Cod <> 0 Then
                    StrSQL.Append(" AND Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & " ")
                End If
                If Veg_Cod <> 0 Then
                    StrSQL.Append(" AND Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
                End If
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Profilazione_Dati.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Profilazione_Dati.Inviato =-1 ")
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


    Public Function LeggiAziendexUtenti(
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreProfilazioneDAL.Profilazione.LeggiAziendexUtenti()"



        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------



            StrSQL.Length = 0
            StrSQL.Append(" SELECT PIVA,Id_Profilo_Dati,Codice_Chiave,Quesito,Id_Gruppo,Valore_salvato, Veg_Cod,PivaSuperUser, ISNULL(Lav_Cod, 0) AS Lav_Cod," + vbCrLf)
            StrSQL.Append("inviato,datainvio,Data_Creazione,Data_Modifica,Username_Creazione,Username_Modifica,Validita_Inizio,Validita_Fine " + vbCrLf)

            StrSQL.Append(" FROM Profilazione_Dati ")
            StrSQL.Append(" WHERE 1=1 ")

            'If PIVA_isK <> "0" Then
            '    StrSQL.Append(" AND PIVA='" & Agro_SQL_SaveText(PIVA_isK) & "'  ")
            'End If

            'If IdProfiloDati_isK <> 0 Then
            '    StrSQL.Append(" AND Id_Profilo_Dati= " & Agro_SQL_SaveNum(IdProfiloDati_isK) & "  ")
            'End If
            'If (Not Codice_Chiave_isK.Equals("")) And (Not Codice_Chiave_isK.Equals("0")) Then
            '    StrSQL.Append(" AND Codice_chiave='" + Agro_SQL_SaveText(Codice_Chiave_isK) + "' ")
            'End If

            'If IdGruppo_isK <> "" Then
            '    StrSQL.Append(" AND Id_Gruppo='" + Agro_SQL_SaveText(IdGruppo_isK) + "'")
            'End If

            'If LeggiSoloNoteConLavorazioneNullSeLAv_CodZero Then
            '    If Lav_Cod <> 0 Then
            '        StrSQL.Append(" AND Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & " ")
            '    Else
            '        StrSQL.Append(" AND Lav_Cod  is null  ")
            '    End If
            '    If Veg_Cod <> 0 Then
            '        StrSQL.Append(" AND Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
            '    End If
            'Else
            '    If Lav_Cod <> 0 Then
            '        StrSQL.Append(" AND Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & " ")
            '    End If
            '    If Veg_Cod <> 0 Then
            '        StrSQL.Append(" AND Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
            '    End If
            'End If


            'If xFiltroAggiuntivo <> "" Then
            '    StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            'End If
            ''--------------------------------------------------------------------------
            'Select Case objParametri.FlagVisibilita
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
            '        StrSQL.Append(" AND   Profilazione_Dati.Inviato >=0 ")
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
            '        StrSQL.Append(" AND   Profilazione_Dati.Inviato =-1 ")
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
            '        '...................................
            '    Case Else
            '        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            'End Select
            ''--------------------------------------------------------------------------
            'If xOrderBy <> "" Then
            '    StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))

            'End If



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



Public Class Profilazione_W
    Inherits AgronicaCoreDataProvider.DataProvider



    Public Function Scrivi( _
                          ByVal PIVA_isK As String, _
                          ByVal IdProfiloDati_isK As Integer, _
                          ByVal Codice_Chiave_isK As String, _
                          ByVal Quesito As String, _
                          ByVal IdGruppo_isK As String, _
                          ByVal ValoreSalvato As String, _
                          ByVal Validita_Inizio As Date, _
                          ByVal Validita_Fine As Date, _
                          ByVal Lav_Cod As Integer, _
                          ByVal Veg_Cod As Integer, _
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastali_W.Scrivi()"

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
            StrSQL.Append("INSERT INTO Profilazione_Dati(       ")
            StrSQL.Append("  PIVA, Id_Profilo_Dati, Codice_chiave, Quesito, Id_Gruppo, Valore_salvato, PivaSuperUser,")
            StrSQL.Append("  inviato, datainvio, Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica, ")
            StrSQL.Append("  Validita_Inizio, Validita_Fine, Lav_Cod, Veg_Cod) ")
            StrSQL.Append(" VALUES (")
            StrSQL.Append("'" + Agro_SQL_SaveText(PIVA_isK) + "',")
            StrSQL.Append(Agro_SQL_SaveNum(IdProfiloDati_isK) + ",")

            StrSQL.Append("'" + Agro_SQL_SaveText(Codice_Chiave_isK) + "',")
            StrSQL.Append("'" + Agro_SQL_SaveText(Quesito) + "',")
            StrSQL.Append("'" + Agro_SQL_SaveText(IdGruppo_isK) + "',")
            StrSQL.Append("'" + Agro_SQL_SaveText(ValoreSalvato) + "',")
            StrSQL.Append("'" + Agro_SQL_SaveText(objParametri.PivaSuperUser) + "',")

            StrSQL.Append("0,") 'inviato
            StrSQL.Append("null,") ' data invio

            StrSQL.Append(Agro_SQL_SaveDate(Date.Now) + ",") 'data creazione
            StrSQL.Append(Agro_SQL_SaveDate(Date.Now) + ",") 'data modifica

            StrSQL.Append("'" + Agro_SQL_SaveText(objParametri.UsernameOperazione) + "',")
            StrSQL.Append("'" + Agro_SQL_SaveText(objParametri.UsernameOperazione) + "',")
            StrSQL.Append(Agro_SQL_SaveDate(Validita_Inizio) + ",")
            StrSQL.Append(Agro_SQL_SaveDate(Validita_Fine) + ",")

            If Lav_Cod <> 0 Then
                StrSQL.Append(" " & Agro_SQL_SaveNum(Lav_Cod) & ", ")
            Else
                StrSQL.Append("null ,")
            End If
            StrSQL.Append(" " & Agro_SQL_SaveNum(Veg_Cod) & " ")

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


    Public Function Modifica( _
                               ByVal Quesito As String, _
                               ByVal ValoreSalvato As String, _
                               ByVal PIVA_isK As String, _
                               ByVal IdProfiloDati_isK As Integer, _
                               ByVal Codice_Chiave_isK As String, _
                               ByVal IdGruppo_isK As String, _
                               ByVal Validita_Inizio As String, _
                               ByVal Validita_Fine As String, _
                                    ByVal xFiltroAggiuntivo As String, _
                                    ByVal Lav_Cod As Integer, _
                                    ByVal Veg_Cod As Integer, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean




        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Profilazione_W.Modifica"

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
            StrSQL.Append("UPDATE Profilazione_Dati SET ")
            StrSQL.Append("        Quesito             =  '" & Agro_SQL_SaveText(Quesito) & "'  ")
            StrSQL.Append("       ,Valore_salvato      =  '" & Agro_SQL_SaveText(ValoreSalvato) & "'  ")
            StrSQL.Append("       ,Data_Modifica      =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("       ,UserName_Modifica  = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("       ,Validita_Inizio    =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("       ,Validita_Fine      =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append("       ,inviato = 0  ")
            If Lav_Cod <> 0 Then
                StrSQL.Append("   ,Lav_Cod             = " & Agro_SQL_SaveNum(Lav_Cod) & " ")
            Else
                StrSQL.Append("   ,Lav_Cod             = null ")
            End If
            StrSQL.Append("   ,Veg_Cod             = " & Agro_SQL_SaveNum(Veg_Cod) & " ")

            StrSQL.Append(" WHERE    PIVA               = '" & Agro_SQL_SaveText(PIVA_isK) & "' ")
            StrSQL.Append(" AND      Id_Profilo_Dati    = " & Agro_SQL_SaveNum(IdProfiloDati_isK) & " ")
            StrSQL.Append(" AND      Codice_chiave      = '" & Agro_SQL_SaveText(Codice_Chiave_isK) & "' ")
            StrSQL.Append(" AND      Id_Gruppo          = '" & Agro_SQL_SaveText(IdGruppo_isK) & "' ")

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


    Public Function Cancella( _
                            ByVal PIVA_isK As String, _
                            ByVal Codice_Chiave_isK As String, _
                            ByVal IdGruppo_isK As String, _
                            ByVal Lav_Cod As Integer, _
                            ByVal Veg_Cod As Integer, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Profilazione_W.Cancella()"

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
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Profilazione_Dati ")
                StrSQL.Append(" SET ")
                StrSQL.Append("          Validita_Fine = " & Agro_SQL_SaveDate(CDate("01/01/1900")) & " ")
                StrSQL.Append("         ,Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append("WHERE PIVA='" + Agro_SQL_SaveText(PIVA_isK) + "'")
                StrSQL.Append(" AND Id_Gruppo='" + Agro_SQL_SaveText(IdGruppo_isK) + "'")
                StrSQL.Append(" AND     Inviato >= 0")

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM    Profilazione_Dati ")
                StrSQL.Append("WHERE PIVA='" + Agro_SQL_SaveText(PIVA_isK) + "'")
                StrSQL.Append(" AND Id_Gruppo='" + Agro_SQL_SaveText(IdGruppo_isK) + "'")

            End If

            If Codice_Chiave_isK <> "" Then
                StrSQL.Append(" AND Codice_chiave='" + Agro_SQL_SaveText(Codice_Chiave_isK) + "' ")
            End If


            If Lav_Cod <> 0 Then
                StrSQL.Append(" AND Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & " ")
            Else
                StrSQL.Append(" AND Lav_Cod  is null  ")
            End If

            StrSQL.Append(" AND Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
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



    Public Function Cancella_Tutte_lavorazioni( _
                            ByVal PIVA_isK As String, _
                            ByVal IdGruppo_isK As String, _
                            ByVal Veg_Cod As Integer, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Profilazione_W.Cancella_Tutte_lavorazioni()"

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
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Profilazione_Dati ")
                StrSQL.Append(" SET ")
                StrSQL.Append("          Validita_Fine = " & Agro_SQL_SaveDate(CDate("01/01/1900")) & " ")
                StrSQL.Append("         ,Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append("WHERE PIVA='" + Agro_SQL_SaveText(PIVA_isK) + "'")
                StrSQL.Append(" AND Id_Gruppo='" + Agro_SQL_SaveText(IdGruppo_isK) + "'")
                StrSQL.Append(" AND     Inviato >= 0")
            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM    Profilazione_Dati ")
                StrSQL.Append("WHERE PIVA='" + Agro_SQL_SaveText(PIVA_isK) + "'")
                StrSQL.Append(" AND Id_Gruppo='" + Agro_SQL_SaveText(IdGruppo_isK) + "'")

            End If

            StrSQL.Append(" AND Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
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
