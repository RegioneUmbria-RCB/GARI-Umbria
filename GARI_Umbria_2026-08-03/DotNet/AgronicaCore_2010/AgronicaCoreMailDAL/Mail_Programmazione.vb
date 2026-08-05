Imports System.Data
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class Mail_Programmazione_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(
                                ByVal ID_Mail As Integer?,
                                ByVal TipoMail_ID As Integer?,
                                ByVal TipoMail_Chiave As String,
                                ByVal DataOraDaCuiInviare_Inizio As DateTime?,
                                ByVal DataOraDaCuiInviare_Fine As DateTime?,
                                ByVal Spedita As Boolean?,
                                ByVal AnnullatoInvio As Boolean?,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreMailDAL.Mail_Programmazione_R.Leggi"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.AppendLine(" SELECT ID_Mail, PivaSuperUser, TipoMail_ID, TipoMail_Chiave, Mittente, DestinatariA, DestinatariCC, DestinatariCCN, ")
            strSQL.AppendLine(" Oggetto, Body, IsBodyHTML, Allegati, DataOraDaCuiInviare, Spedita, Spedizione_DataOra, Spedizione_Risultato, ")
            strSQL.AppendLine(" AnnullatoInvio, AnnullatoInvio_DataOra, AnnullatoInvio_User, Filtro_RapConMail ")
            strSQL.AppendLine(" FROM Mail_Programmazione ")

            strSQL.AppendLine(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))

            If Not IsNothing(ID_Mail) Then
                strSQL.AppendLine(" AND ID_Mail = " & Agro_SQL_SaveNum_NULL(ID_Mail))
            End If

            If Not IsNothing(TipoMail_ID) Then
                strSQL.AppendLine(" AND TipoMail_ID = " & Agro_SQL_SaveNum_NULL(TipoMail_ID))
            End If

            If Not IsNothing(TipoMail_Chiave) Then
                strSQL.AppendLine(" AND TipoMail_Chiave = " & Agro_SQL_SaveText_NULL(TipoMail_Chiave))
            End If

            If Not IsNothing(Spedita) Then
                strSQL.AppendLine(" AND Spedita = " & Agro_SQL_SaveBoolStrToInt_NULL(Spedita))
            End If

            If Not IsNothing(AnnullatoInvio) Then
                strSQL.AppendLine(" AND AnnullatoInvio = " & Agro_SQL_SaveBoolStrToInt_NULL(AnnullatoInvio))
            End If

            If Not IsNothing(DataOraDaCuiInviare_Inizio) Then
                strSQL.AppendLine(" AND DataOraDaCuiInviare >= " & Agro_SQL_SaveDateTime_NULL(DataOraDaCuiInviare_Inizio))
            End If

            If Not IsNothing(DataOraDaCuiInviare_Fine) Then
                strSQL.AppendLine(" AND DataOraDaCuiInviare <= " & Agro_SQL_SaveDateTime_NULL(DataOraDaCuiInviare_Fine))
            End If


            If xFiltroAggiuntivo <> "" Then
                strSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    strSQL.AppendLine(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    strSQL.AppendLine(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                strSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message & " QUERY: " & strSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function





    '##############################################################################################
    ''Lettura degli id_mail da aggiornare dopo la modifica della configurazione di avviso
    Public Function LeggixUpdateAvviso(
                                ByVal ID_Area As Integer,
                                ByVal ID_Tipologia As Integer,
                                ByVal Filtro_RapCon As String,
                                ByVal ID_Avviso As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreMailDAL.Mail_Programmazione_R.LeggixUpdateAvviso"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.AppendLine(" Select Distinct TipoMail_Chiave, data_scadenza, ggattesa From Mail_Programmazione, Alert_Elenco, Alert_Tipologia, Alert_Avvisi  ")
            strSQL.AppendLine(" where Mail_Programmazione.PivaSuperUser = Alert_Elenco.PivaSuperUser     ")
            strSQL.AppendLine(" And Mail_Programmazione.Id_Mail = Mail_Programmazione.id_Mail ")
            strSQL.AppendLine(" And  Alert_Avvisi.PivaSuperUser = Alert_Elenco.PivaSuperUser  ")
            strSQL.AppendLine(" And  Mail_Programmazione.spedita = 0  ")
            strSQL.AppendLine(" And  Mail_Programmazione.tipomail_Id = 7  ")
            strSQL.AppendLine(" And  Mail_Programmazione.tipomail_chiave = alert_Elenco.Id_Elenco  ")
            strSQL.AppendLine(" And  Alert_Elenco.ID_Tipologia = Alert_Tipologia.ID_Tipologia  ")
            strSQL.AppendLine(" And  Alert_Avvisi.ID_Area = Alert_Tipologia.Id_Area  ")
            strSQL.AppendLine(" And  Alert_Avvisi.ID_Tipologia In (0, Alert_Tipologia.Id_Tipologia)  ")
            strSQL.AppendLine(" And  Alert_Elenco.Data_Scadenza >= GETDATE()  ")
            strSQL.AppendLine(" And  Mail_Programmazione.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")

            If ID_Avviso <> 0 Then
                strSQL.AppendLine(" And  Alert_Avvisi.ID_Avviso = " & Agro_SQL_SaveNum(ID_Avviso) & " ")
            End If

            If ID_Area <> 0 Then
                strSQL.AppendLine(" And  Alert_Tipologia.Id_Area = " & ID_Area & "  ")
            End If

            If ID_Tipologia <> 0 Then
                strSQL.AppendLine(" And  Alert_Tipologia.ID_Tipologia = " & ID_Tipologia & "  ")
            End If

            If Trim(Filtro_RapCon) <> "" Then
                strSQL.AppendLine(" And  Alert_Avvisi.Filtro_RapCon = '" & Agro_SQL_SaveText(Filtro_RapCon) & "'  ")
            End If

            If ID_Area = 0 And ID_Avviso = 0 Then

                'Dummy per coerenza dati --> non ritorna record
                strSQL.AppendLine(" And  Alert_Tipologia.Id_Area = 10000")

            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    strSQL.AppendLine(" AND   Mail_Programmazione.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    strSQL.AppendLine(" AND   Mail_Programmazione.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message & " QUERY: " & strSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function


End Class

Public Class Mail_Programmazione_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function MarcaSpedizioneMail(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByVal ID_Mail As Integer,
                                        ByVal Spedita As Boolean,
                                        ByVal Spedizione_DataOra As DateTime,
                                        ByVal Spedizione_Risultato As String,
                                        Optional ByVal Data_modifica As Date = #2/1/1900#,
                                        Optional ByVal username_modifica As String = ""
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreMailDAL.Mail_Programmazione_W.MarcaSpedizioneMail()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_modifica = #2/1/1900# Then
                Data_modifica = DateTime.Now
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine(" UPDATE Mail_Programmazione SET")
            StrSQL.AppendLine("  Spedita = " & Agro_SQL_SaveBoolStrToInt_NULL(Spedita))
            StrSQL.AppendLine(", Spedizione_DataOra = " & Agro_SQL_SaveDateTime_NULL(Spedizione_DataOra))
            StrSQL.AppendLine(", Spedizione_Risultato = " & Agro_SQL_SaveText_NULL(Spedizione_Risultato))
            StrSQL.AppendLine(", Data_Modifica = " & Agro_SQL_SaveDateTime(Data_modifica))
            StrSQL.AppendLine(", UserName_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "' ")

            StrSQL.AppendLine("	WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.AppendLine("	AND ID_Mail  =		" & Agro_SQL_SaveNum_NULL(ID_Mail))


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message & " QUERY: " & StrSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp


    End Function


    Public Function AggiornaDataInvio(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                      ByVal TipoMail_Chiave As Integer,
                                      ByVal New_MailA As String,
                                      ByVal DataOraDaCuiInviare As DateTime
                                      ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreMailDAL.Mail_Programmazione_W.AggiornaDataInvio()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try


            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine(" UPDATE Mail_Programmazione SET")
            StrSQL.AppendLine(" DataOraDaCuiInviare = " & Agro_SQL_SaveDateTime_NULL(DataOraDaCuiInviare))
            StrSQL.AppendLine("	WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.AppendLine("	AND TipoMail_Chiave  =		" & Agro_SQL_SaveNum_NULL(TipoMail_Chiave))

            If New_MailA <> "" Then
                StrSQL.AppendLine(" And  Mail_Programmazione.DestinatariA = '" & Agro_SQL_SaveText(New_MailA) & "'")
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message & " QUERY: " & StrSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp


    End Function

    '##############################################################################################
    Public Function Modifica(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            ByVal Old_ID_Mail As Integer,
                            ByVal New_TipoMail_ID As Integer,
                            ByVal New_TipoMail_Chiave As String,
                            ByVal New_Mittente As String,
                            ByVal New_DestinatariA As String,
                            ByVal New_DestinatariCC As String,
                            ByVal New_DestinatariCCN As String,
                            ByVal New_Oggetto As String,
                            ByVal New_Body As String,
                            ByVal New_IsBodyHTML As Boolean,
                            ByVal New_Allegati As String,
                            ByVal New_DataOraDaCuiInviare As DateTime,
                            ByVal New_Spedita As Boolean?,
                            ByVal New_Spedizione_DataOra As DateTime?,
                            ByVal New_Spedizione_Risultato As String,
                            ByVal New_AnnullatoInvio As Boolean?,
                            ByVal New_AnnullatoInvio_DataOra As DateTime?,
                            ByVal New_AnnullatoInvio_User As String,
                            Optional ByVal Data_modifica As Date = #2/1/1900#,
                            Optional ByVal username_modifica As String = ""
                            ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreMailDAL.Mail_Programmazione_W.Modifica()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_modifica = #2/1/1900# Then
                Data_modifica = DateTime.Now
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine(" UPDATE Mail_Programmazione SET")

            StrSQL.AppendLine("  TipoMail_ID = " & Agro_SQL_SaveNum_NULL(New_TipoMail_ID))
            StrSQL.AppendLine(", TipoMail_Chiave = " & Agro_SQL_SaveText_NULL(New_TipoMail_Chiave))
            StrSQL.AppendLine(", Mittente = " & Agro_SQL_SaveText_NULL(New_Mittente))
            StrSQL.AppendLine(", DestinatariA = " & Agro_SQL_SaveText_NULL(New_DestinatariA))
            StrSQL.AppendLine(", DestinatariCC = " & Agro_SQL_SaveText_NULL(New_DestinatariCC))
            StrSQL.AppendLine(", DestinatariCCN = " & Agro_SQL_SaveText_NULL(New_DestinatariCCN))
            StrSQL.AppendLine(", Oggetto = " & Agro_SQL_SaveText_NULL(New_Oggetto))
            StrSQL.AppendLine(", Body = " & Agro_SQL_SaveText_NULL(New_Body))
            StrSQL.AppendLine(", IsBodyHTML = " & Agro_SQL_SaveNum_NULL(New_IsBodyHTML))
            StrSQL.AppendLine(", Allegati = " & Agro_SQL_SaveText_NULL(New_Allegati))
            StrSQL.AppendLine(", DataOraDaCuiInviare = " & Agro_SQL_SaveDateTime_NULL(New_DataOraDaCuiInviare))
            StrSQL.AppendLine(", Spedita = " & Agro_SQL_SaveNum_NULL(New_Spedita))
            StrSQL.AppendLine(", Spedizione_DataOra = " & Agro_SQL_SaveDateTime_NULL(New_Spedizione_DataOra))
            StrSQL.AppendLine(", Spedizione_Risultato = " & Agro_SQL_SaveText_NULL(New_Spedizione_Risultato))
            StrSQL.AppendLine(", AnnullatoInvio = " & Agro_SQL_SaveNum_NULL(New_AnnullatoInvio))
            StrSQL.AppendLine(", AnnullatoInvio_DataOra = " & Agro_SQL_SaveDateTime_NULL(New_AnnullatoInvio_DataOra))
            StrSQL.AppendLine(", AnnullatoInvio_User = " & Agro_SQL_SaveText_NULL(New_AnnullatoInvio_User))

            StrSQL.AppendLine(", Data_Modifica = " & Agro_SQL_SaveDateTime(Data_modifica))
            StrSQL.AppendLine(", UserName_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "' ")

            StrSQL.AppendLine("	WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.AppendLine("	AND ID_Mail  =		" & Agro_SQL_SaveNum_NULL(Old_ID_Mail))


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message & " QUERY: " & StrSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function

    '##############################################################################################
    Public Function Modifica_AnnullaInvioFromChiave(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            ByVal Old_TipoMail_ID As Integer,
                            ByVal Old_TipoMail_Chiave As String,
                            ByVal New_AnnullatoInvio As Boolean,
                            ByVal New_AnnullatoInvio_DataOra As DateTime?,
                            ByVal New_AnnullatoInvio_User As String,
                            Optional ByVal Data_modifica As Date = #2/1/1900#,
                            Optional ByVal username_modifica As String = ""
                            ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreMailDAL.Mail_Programmazione_W.Modifica_AnnullaInvioFromChiave()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_modifica = #2/1/1900# Then
                Data_modifica = DateTime.Now
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine(" UPDATE Mail_Programmazione SET")

            StrSQL.AppendLine("  AnnullatoInvio = " & Agro_SQL_SaveBoolStrToInt_NULL(New_AnnullatoInvio))
            StrSQL.AppendLine(", AnnullatoInvio_DataOra = " & Agro_SQL_SaveDateTime_NULL(New_AnnullatoInvio_DataOra))
            StrSQL.AppendLine(", AnnullatoInvio_User = " & Agro_SQL_SaveText_NULL(New_AnnullatoInvio_User))

            StrSQL.AppendLine(", Data_Modifica = " & Agro_SQL_SaveDateTime(Data_modifica))
            StrSQL.AppendLine(", UserName_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "' ")

            StrSQL.AppendLine("	WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.AppendLine("	AND TipoMail_ID  =		" & Agro_SQL_SaveNum_NULL(Old_TipoMail_ID))
            StrSQL.AppendLine("	AND TipoMail_Chiave  =		" & Agro_SQL_SaveText_NULL(Old_TipoMail_Chiave))
            StrSQL.AppendLine("	AND Spedita  =	0	")
            StrSQL.AppendLine("	AND AnnullatoInvio  =	0	")



            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message & " QUERY: " & StrSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function

    '##############################################################################################
    Public Function Modifica_AnnullaInvioFromChiaveLike(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            ByVal Old_TipoMail_ID As Integer,
                            ByVal Old_TipoMail_ChiaveLike As String,
                            ByVal New_AnnullatoInvio As Boolean,
                            ByVal New_AnnullatoInvio_DataOra As DateTime?,
                            ByVal New_AnnullatoInvio_User As String,
                            Optional ByVal Data_modifica As Date = #2/1/1900#,
                            Optional ByVal username_modifica As String = ""
                            ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreMailDAL.Mail_Programmazione_W.Modifica_AnnullaInvioFromChiaveLike()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_modifica = #2/1/1900# Then
                Data_modifica = DateTime.Now
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine(" UPDATE Mail_Programmazione SET")

            StrSQL.AppendLine("  AnnullatoInvio = " & Agro_SQL_SaveBoolStrToInt_NULL(New_AnnullatoInvio))
            StrSQL.AppendLine(", AnnullatoInvio_DataOra = " & Agro_SQL_SaveDateTime_NULL(New_AnnullatoInvio_DataOra))
            StrSQL.AppendLine(", AnnullatoInvio_User = " & Agro_SQL_SaveText_NULL(New_AnnullatoInvio_User))

            StrSQL.AppendLine(", Data_Modifica = " & Agro_SQL_SaveDateTime(Data_modifica))
            StrSQL.AppendLine(", UserName_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "' ")

            StrSQL.AppendLine("	WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.AppendLine("	AND TipoMail_ID  =		" & Agro_SQL_SaveNum_NULL(Old_TipoMail_ID))
            StrSQL.AppendLine("	AND TipoMail_Chiave LIKE '" & Agro_SQL_SaveText(Old_TipoMail_ChiaveLike) & "'")
            StrSQL.AppendLine("	AND Spedita = 0 ")
            StrSQL.AppendLine("	AND AnnullatoInvio  = 0 ")



            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message & " QUERY: " & StrSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function


    Public Function Modifica_AnnullaInvioFromIDMail(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                        ByVal Old_ID_Mail As Integer,
                        ByVal New_AnnullatoInvio As Boolean,
                        ByVal New_AnnullatoInvio_DataOra As DateTime?,
                        ByVal New_AnnullatoInvio_User As String,
                        Optional ByVal Data_modifica As Date = #2/1/1900#,
                        Optional ByVal username_modifica As String = ""
                        ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreMailDAL.Mail_Programmazione_W.Modifica_AnnullaInvioFromIDMail()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_modifica = #2/1/1900# Then
                Data_modifica = DateTime.Now
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine(" UPDATE Mail_Programmazione SET")

            StrSQL.AppendLine("  AnnullatoInvio = " & Agro_SQL_SaveBoolStrToInt_NULL(New_AnnullatoInvio))
            StrSQL.AppendLine(", AnnullatoInvio_DataOra = " & Agro_SQL_SaveDateTime_NULL(New_AnnullatoInvio_DataOra))
            StrSQL.AppendLine(", AnnullatoInvio_User = " & Agro_SQL_SaveText_NULL(New_AnnullatoInvio_User))

            StrSQL.AppendLine(", Data_Modifica = " & Agro_SQL_SaveDateTime(Data_modifica))
            StrSQL.AppendLine(", UserName_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "' ")

            StrSQL.AppendLine("	WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.AppendLine("	AND ID_Mail  =		" & Agro_SQL_SaveNum_NULL(Old_ID_Mail))


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message & " QUERY: " & StrSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function


    '##############################################################################################
    Public Function Scrivi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            ByVal ID_Mail As Integer,
                            ByVal TipoMail_ID As Integer,
                            ByVal TipoMail_Chiave As String,
                            ByVal Mittente As String,
                            ByVal DestinatariA As String,
                            ByVal DestinatariCC As String,
                            ByVal DestinatariCCN As String,
                            ByVal Oggetto As String,
                            ByVal Body As String,
                            ByVal IsBodyHTML As Boolean,
                            ByVal Allegati As String,
                            ByVal DataOraDaCuiInviare As DateTime,
                            ByVal Spedita As Boolean,
                            ByVal Spedizione_DataOra As DateTime?,
                            ByVal Spedizione_Risultato As String,
                            ByVal AnnullatoInvio As Boolean,
                            ByVal AnnullatoInvio_DataOra As DateTime?,
                            ByVal AnnullatoInvio_User As String,
                            ByVal Filtro_RapConMail As String,
                            Optional ByVal Data_creazione As Date = #2/1/1900#,
                            Optional ByVal Data_modifica As Date = #2/1/1900#,
                            Optional ByVal username_creazione As String = "",
                            Optional ByVal username_modifica As String = ""
                            ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreMailDAL.Mail_Programmazione_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_creazione = #2/1/1900# Then
                Data_creazione = DateTime.Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = DateTime.Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If



            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine(" INSERT INTO  Mail_Programmazione ")

            StrSQL.AppendLine("              (")
            StrSQL.AppendLine("              PivaSuperUser,      ")
            StrSQL.AppendLine("              ID_Mail,                   TipoMail_ID, ")
            StrSQL.AppendLine("              TipoMail_Chiave,           Mittente, ")
            StrSQL.AppendLine("              DestinatariA,              DestinatariCC, ")
            StrSQL.AppendLine("              DestinatariCCN,            Oggetto, ")
            StrSQL.AppendLine("              Body,                      IsBodyHTML, ")
            StrSQL.AppendLine("              Allegati,                  DataOraDaCuiInviare, ")
            StrSQL.AppendLine("              Spedita,                   Spedizione_DataOra, ")
            StrSQL.AppendLine("              Spedizione_Risultato,      AnnullatoInvio, ")
            StrSQL.AppendLine("              AnnullatoInvio_DataOra,    AnnullatoInvio_User, Filtro_RapConMail, ")

            StrSQL.AppendLine("              Inviato,            datainvio, ")
            StrSQL.AppendLine("              Data_Creazione,     Data_Modifica, ")
            StrSQL.AppendLine("              UserName_Creazione, UserName_Modifica, ")
            StrSQL.AppendLine("              Validita_Inizio,    Validita_Fine ")
            StrSQL.AppendLine("              ) ")

            StrSQL.AppendLine(" VALUES ( ")

            StrSQL.AppendLine("			 " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.AppendLine("			," & Agro_SQL_SaveNum_NULL(ID_Mail))
            StrSQL.AppendLine("			," & Agro_SQL_SaveNum_NULL(TipoMail_ID))
            StrSQL.AppendLine("			," & Agro_SQL_SaveText_NULL(TipoMail_Chiave))
            StrSQL.AppendLine("			," & Agro_SQL_SaveText_NULL(Mittente))
            StrSQL.AppendLine("			," & Agro_SQL_SaveText_NULL(DestinatariA))
            StrSQL.AppendLine("			," & Agro_SQL_SaveText_NULL(DestinatariCC))
            StrSQL.AppendLine("			," & Agro_SQL_SaveText_NULL(DestinatariCCN))
            StrSQL.AppendLine("			," & Agro_SQL_SaveText_NULL(Oggetto))
            StrSQL.AppendLine("			," & Agro_SQL_SaveText_NULL(Body))
            StrSQL.AppendLine("			," & Agro_SQL_SaveBoolStrToInt_NULL(IsBodyHTML))
            StrSQL.AppendLine("			," & Agro_SQL_SaveText_NULL(Allegati))
            StrSQL.AppendLine("			," & Agro_SQL_SaveDateTime_NULL(DataOraDaCuiInviare))
            StrSQL.AppendLine("			," & Agro_SQL_SaveBoolStrToInt_NULL(Spedita))
            StrSQL.AppendLine("			," & Agro_SQL_SaveDateTime_NULL(Spedizione_DataOra))
            StrSQL.AppendLine("			," & Agro_SQL_SaveText_NULL(Spedizione_Risultato))
            StrSQL.AppendLine("			," & Agro_SQL_SaveBoolStrToInt_NULL(AnnullatoInvio))
            StrSQL.AppendLine("			," & Agro_SQL_SaveDateTime_NULL(AnnullatoInvio_DataOra))
            StrSQL.AppendLine("			," & Agro_SQL_SaveText_NULL(AnnullatoInvio_User))
            StrSQL.AppendLine("			," & Agro_SQL_SaveText_NULL(Filtro_RapConMail))

            StrSQL.AppendLine("         , 0  ")
            StrSQL.AppendLine("         , Null  ")
            StrSQL.AppendLine("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            StrSQL.AppendLine("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            StrSQL.AppendLine("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.AppendLine("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.AppendLine("			, " & Agro_SQL_SaveDate(AGRODATAINIZIO) & "  ")
            StrSQL.AppendLine("			, " & Agro_SQL_SaveDate(AGRODATAFINE) & "  ")

            StrSQL.AppendLine(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message & " QUERY: " & StrSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function

    '#################################################################
    Public Function Cancella(ByVal xFiltroAggiuntivo As String,
                             ByVal ID_Mail As Integer,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreMailDAL.Mail_Programmazione_W.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                'StrSQL.Append(" UPDATE ... ")
                'StrSQL.Append(" SET ")
                'StrSQL.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                'StrSQL.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                'StrSQL.Append("         ,Inviato = -1 ")
                'StrSQL.Append(" WHERE   1=1 ")
                'StrSQL.Append(" AND     Inviato >= 0 ")
            Else
                StrSQL.AppendLine(" DELETE FROM Mail_Programmazione ")
                StrSQL.AppendLine(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
                StrSQL.AppendLine("	AND ID_Mail =		" & Agro_SQL_SaveNum_NULL(ID_Mail))
            End If
            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message & " QUERY: " & StrSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function


End Class
