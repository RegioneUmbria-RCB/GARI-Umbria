Imports System.Data
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class SMS_Programmazione_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(
                                ByVal ID_SMS As Integer?,
                                ByVal TipoSMS_ID As Integer?,
                                ByVal TipoSMS_Chiave As String,
                                ByVal DataOraDaCuiInviare_Inizio As DateTime?,
                                ByVal DataOraDaCuiInviare_Fine As DateTime?,
                                ByVal Spedito As Boolean?,
                                ByVal AnnullatoInvio As Boolean?,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreMailDAL.SMS_Programmazione_R.Leggi"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.AppendLine(" SELECT ID_SMS, PivaSuperUser, TipoSMS_ID, TipoSMS_Chiave, Mittente, Destinatari,")
            strSQL.AppendLine(" Testo, DataOraDaCuiInviare, Spedito, Spedizione_DataOra, Spedizione_Risultato, ")
            strSQL.AppendLine(" AnnullatoInvio, AnnullatoInvio_DataOra, AnnullatoInvio_User ")
            strSQL.AppendLine(" FROM SMS_Programmazione ")

            strSQL.AppendLine(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))

            If Not IsNothing(ID_SMS) Then
                strSQL.AppendLine(" AND ID_SMS = " & Agro_SQL_SaveNum_NULL(ID_SMS))
            End If

            If Not IsNothing(TipoSMS_ID) Then
                strSQL.AppendLine(" AND TipoSMS_ID = " & Agro_SQL_SaveNum_NULL(TipoSMS_ID))
            End If

            If Not IsNothing(TipoSMS_Chiave) Then
                strSQL.AppendLine(" AND TipoSMS_Chiave = " & Agro_SQL_SaveText_NULL(TipoSMS_Chiave))
            End If

            If Not IsNothing(Spedito) Then
                strSQL.AppendLine(" AND Spedito = " & Agro_SQL_SaveBoolStrToInt_NULL(Spedito))
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
            'Select Case objParametri.FlagVisibilita
            'Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
            'strSQL.AppendLine(" AND   Inviato >=0 ")
            'Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
            'strSQL.AppendLine(" AND   Inviato =-1 ")
            'Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
            '...................................
            'Case Else
            'Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            'End Select
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

End Class

Public Class SMS_Programmazione_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function MarcaSpedizioneSMS(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByVal ID_SMS As Integer,
                                        ByVal Spedito As Boolean,
                                        ByVal Spedizione_DataOra As DateTime,
                                        ByVal Spedizione_Risultato As String,
                                        Optional ByVal Data_modifica As Date = #2/1/1900#,
                                        Optional ByVal username_modifica As String = ""
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreMailDAL.SMS_Programmazione_W.MarcaSpedizioneSMS()"

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
            StrSQL.AppendLine(" UPDATE SMS_Programmazione SET")
            StrSQL.AppendLine("  Spedito = " & Agro_SQL_SaveBoolStrToInt_NULL(Spedito))
            StrSQL.AppendLine(", Spedizione_DataOra = " & Agro_SQL_SaveDateTime_NULL(Spedizione_DataOra))
            StrSQL.AppendLine(", Spedizione_Risultato = " & Agro_SQL_SaveText_NULL(Spedizione_Risultato))
            StrSQL.AppendLine(", Data_Modifica = " & Agro_SQL_SaveDateTime(Data_modifica))
            StrSQL.AppendLine(", UserName_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "' ")

            StrSQL.AppendLine("	WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.AppendLine("	AND ID_SMS  =		" & Agro_SQL_SaveNum_NULL(ID_SMS))


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
                            ByVal Old_ID_SMS As Integer,
                            ByVal New_TipoSMS_ID As Integer,
                            ByVal New_TipoSMS_Chiave As String,
                            ByVal New_Mittente As String,
                            ByVal New_Destinatari As String,
                            ByVal New_Testo As String,
                            ByVal New_DataOraDaCuiInviare As DateTime,
                            ByVal New_Spedito As Boolean?,
                            ByVal New_Spedizione_DataOra As DateTime?,
                            ByVal New_Spedizione_Risultato As String,
                            ByVal New_AnnullatoInvio As Boolean?,
                            ByVal New_AnnullatoInvio_DataOra As DateTime?,
                            ByVal New_AnnullatoInvio_User As String,
                            Optional ByVal Data_modifica As Date = #2/1/1900#,
                            Optional ByVal username_modifica As String = ""
                            ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreMailDAL.SMS_Programmazione_W.Modifica()"

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
            StrSQL.AppendLine(" UPDATE SMS_Programmazione SET")

            StrSQL.AppendLine("  TipoSMS_ID = " & Agro_SQL_SaveNum_NULL(New_TipoSMS_ID))
            StrSQL.AppendLine(", TipoSMS_Chiave = " & Agro_SQL_SaveText_NULL(New_TipoSMS_Chiave))
            StrSQL.AppendLine(", Mittente = " & Agro_SQL_SaveText_NULL(New_Mittente))
            StrSQL.AppendLine(", Destinatari = " & Agro_SQL_SaveText_NULL(New_Destinatari))
            StrSQL.AppendLine(", Testo = " & Agro_SQL_SaveText_NULL(New_Testo))
            StrSQL.AppendLine(", DataOraDaCuiInviare = " & Agro_SQL_SaveDateTime_NULL(New_DataOraDaCuiInviare))
            StrSQL.AppendLine(", Spedito = " & Agro_SQL_SaveNum_NULL(New_Spedito))
            StrSQL.AppendLine(", Spedizione_DataOra = " & Agro_SQL_SaveDateTime_NULL(New_Spedizione_DataOra))
            StrSQL.AppendLine(", Spedizione_Risultato = " & Agro_SQL_SaveText_NULL(New_Spedizione_Risultato))
            StrSQL.AppendLine(", AnnullatoInvio = " & Agro_SQL_SaveNum_NULL(New_AnnullatoInvio))
            StrSQL.AppendLine(", AnnullatoInvio_DataOra = " & Agro_SQL_SaveDateTime_NULL(New_AnnullatoInvio_DataOra))
            StrSQL.AppendLine(", AnnullatoInvio_User = " & Agro_SQL_SaveText_NULL(New_AnnullatoInvio_User))

            StrSQL.AppendLine(", Data_Modifica = " & Agro_SQL_SaveDateTime(Data_modifica))
            StrSQL.AppendLine(", UserName_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "' ")

            StrSQL.AppendLine("	WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.AppendLine("	AND ID_SMS  =		" & Agro_SQL_SaveNum_NULL(Old_ID_SMS))


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
    Public Function Modifica_AnnullaInvioFromChiaveTestataFase(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            ByVal Old_TipoSMS_ID As Integer,
                            ByVal Old_TipoSMS_Chiave As String,
                            ByVal New_AnnullatoInvio As Boolean,
                            ByVal New_AnnullatoInvio_DataOra As DateTime?,
                            ByVal New_AnnullatoInvio_User As String,
                            Optional ByVal Data_modifica As Date = #2/1/1900#,
                            Optional ByVal username_modifica As String = ""
                            ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreMailDAL.SMS_Programmazione_W.Modifica_AnnullaInvioFromChiaveTestataFase()"

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
            StrSQL.AppendLine(" UPDATE SMS_Programmazione SET")

            StrSQL.AppendLine("  AnnullatoInvio = " & Agro_SQL_SaveBoolStrToInt_NULL(New_AnnullatoInvio))
            StrSQL.AppendLine(", AnnullatoInvio_DataOra = " & Agro_SQL_SaveDateTime_NULL(New_AnnullatoInvio_DataOra))
            StrSQL.AppendLine(", AnnullatoInvio_User = " & Agro_SQL_SaveText_NULL(New_AnnullatoInvio_User))

            StrSQL.AppendLine(", Data_Modifica = " & Agro_SQL_SaveDateTime(Data_modifica))
            StrSQL.AppendLine(", UserName_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "' ")

            StrSQL.AppendLine("	WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.AppendLine("	AND TipoSMS_ID  =		" & Agro_SQL_SaveNum_NULL(Old_TipoSMS_ID))
            StrSQL.AppendLine("	AND TipoSMS_Chiave  =		" & Agro_SQL_SaveText_NULL(Old_TipoSMS_Chiave))
            StrSQL.AppendLine("	AND Spedito  =	0	")
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
    Public Function Modifica_AnnullaInvioFromChiaveTestata(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            ByVal Old_TipoSMS_ID As Integer,
                            ByVal Old_TipoSMS_ChiaveLike As String,
                            ByVal New_AnnullatoInvio As Boolean,
                            ByVal New_AnnullatoInvio_DataOra As DateTime?,
                            ByVal New_AnnullatoInvio_User As String,
                            Optional ByVal Data_modifica As Date = #2/1/1900#,
                            Optional ByVal username_modifica As String = ""
                            ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreMailDAL.SMS_Programmazione_W.Modifica_AnnullaInvioFromChiaveTestata()"

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
            StrSQL.AppendLine(" UPDATE SMS_Programmazione SET")

            StrSQL.AppendLine("  AnnullatoInvio = " & Agro_SQL_SaveBoolStrToInt_NULL(New_AnnullatoInvio))
            StrSQL.AppendLine(", AnnullatoInvio_DataOra = " & Agro_SQL_SaveDateTime_NULL(New_AnnullatoInvio_DataOra))
            StrSQL.AppendLine(", AnnullatoInvio_User = " & Agro_SQL_SaveText_NULL(New_AnnullatoInvio_User))

            StrSQL.AppendLine(", Data_Modifica = " & Agro_SQL_SaveDateTime(Data_modifica))
            StrSQL.AppendLine(", UserName_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "' ")

            StrSQL.AppendLine("	WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.AppendLine("	AND TipoSMS_ID  =		" & Agro_SQL_SaveNum_NULL(Old_TipoSMS_ID))
            StrSQL.AppendLine("	AND TipoSMS_Chiave LIKE '" & Old_TipoSMS_ChiaveLike & "%'")
            StrSQL.AppendLine("	AND Spedito = 0 ")
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


    Public Function Modifica_AnnullaInvioFromIDSMS(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                        ByVal Old_ID_SMS As Integer,
                        ByVal New_AnnullatoInvio As Boolean,
                        ByVal New_AnnullatoInvio_DataOra As DateTime?,
                        ByVal New_AnnullatoInvio_User As String,
                        Optional ByVal Data_modifica As Date = #2/1/1900#,
                        Optional ByVal username_modifica As String = ""
                        ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreMailDAL.SMS_Programmazione_W.Modifica_AnnullaInvioFromIDSMS()"

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
            StrSQL.AppendLine(" UPDATE SMS_Programmazione SET")

            StrSQL.AppendLine("  AnnullatoInvio = " & Agro_SQL_SaveBoolStrToInt_NULL(New_AnnullatoInvio))
            StrSQL.AppendLine(", AnnullatoInvio_DataOra = " & Agro_SQL_SaveDateTime_NULL(New_AnnullatoInvio_DataOra))
            StrSQL.AppendLine(", AnnullatoInvio_User = " & Agro_SQL_SaveText_NULL(New_AnnullatoInvio_User))

            StrSQL.AppendLine(", Data_Modifica = " & Agro_SQL_SaveDateTime(Data_modifica))
            StrSQL.AppendLine(", UserName_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "' ")

            StrSQL.AppendLine("	WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.AppendLine("	AND ID_SMS  =		" & Agro_SQL_SaveNum_NULL(Old_ID_SMS))


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
                            ByVal ID_SMS As Integer,
                            ByVal TipoSMS_ID As Integer,
                            ByVal TipoSMS_Chiave As String,
                            ByVal Mittente As String,
                            ByVal Destinatari As String,
                            ByVal Testo As String,
                            ByVal DataOraDaCuiInviare As DateTime?,
                            ByVal Spedito As Boolean,
                            ByVal Spedizione_DataOra As DateTime?,
                            ByVal Spedizione_Risultato As String,
                            ByVal AnnullatoInvio As Boolean,
                            ByVal AnnullatoInvio_DataOra As DateTime?,
                            ByVal AnnullatoInvio_User As String,
                            Optional ByVal Data_creazione As Date = #2/1/1900#,
                            Optional ByVal Data_modifica As Date = #2/1/1900#,
                            Optional ByVal username_creazione As String = "",
                            Optional ByVal username_modifica As String = ""
                            ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreMailDAL.SMS_Programmazione_W.Scrivi()"

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
            StrSQL.AppendLine(" INSERT INTO  SMS_Programmazione ")

            StrSQL.AppendLine("              (")
            StrSQL.AppendLine("              PivaSuperUser,      ")
            StrSQL.AppendLine("              ID_SMS,                   TipoSMS_ID, ")
            StrSQL.AppendLine("              TipoSMS_Chiave,           Mittente, ")
            StrSQL.AppendLine("              Destinatari,              ")
            StrSQL.AppendLine("              Testo,                    ")
            StrSQL.AppendLine("              DataOraDaCuiInviare, ")
            StrSQL.AppendLine("              Spedito,                   Spedizione_DataOra, ")
            StrSQL.AppendLine("              Spedizione_Risultato,      AnnullatoInvio, ")
            StrSQL.AppendLine("              AnnullatoInvio_DataOra,    AnnullatoInvio_User, ")

            StrSQL.AppendLine("              Inviato,            datainvio, ")
            StrSQL.AppendLine("              Data_Creazione,     Data_Modifica, ")
            StrSQL.AppendLine("              UserName_Creazione, UserName_Modifica, ")
            StrSQL.AppendLine("              Validita_Inizio,    Validita_Fine ")
            StrSQL.AppendLine("              ) ")

            StrSQL.AppendLine(" VALUES ( ")

            StrSQL.AppendLine("			 " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.AppendLine("			," & Agro_SQL_SaveNum_NULL(ID_SMS))
            StrSQL.AppendLine("			," & Agro_SQL_SaveNum_NULL(TipoSMS_ID))
            StrSQL.AppendLine("			," & Agro_SQL_SaveText_NULL(TipoSMS_Chiave))
            StrSQL.AppendLine("			," & Agro_SQL_SaveText_NULL(Mittente))
            StrSQL.AppendLine("			," & Agro_SQL_SaveText_NULL(Destinatari))
            StrSQL.AppendLine("			," & Agro_SQL_SaveText_NULL(Testo))
            StrSQL.AppendLine("			," & Agro_SQL_SaveDateTime_NULL(DataOraDaCuiInviare))
            StrSQL.AppendLine("			," & Agro_SQL_SaveBoolStrToInt_NULL(Spedito))
            StrSQL.AppendLine("			," & Agro_SQL_SaveDateTime_NULL(Spedizione_DataOra))
            StrSQL.AppendLine("			," & Agro_SQL_SaveText_NULL(Spedizione_Risultato))
            StrSQL.AppendLine("			," & Agro_SQL_SaveBoolStrToInt_NULL(AnnullatoInvio))
            StrSQL.AppendLine("			," & Agro_SQL_SaveDateTime_NULL(AnnullatoInvio_DataOra))
            StrSQL.AppendLine("			," & Agro_SQL_SaveText_NULL(AnnullatoInvio_User))

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
                             ByVal ID_SMS As Integer,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreMailDAL.SMS_Programmazione_W.Cancella()"

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
                StrSQL.AppendLine(" DELETE FROM SMS_Programmazione ")
                StrSQL.AppendLine(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
                StrSQL.AppendLine("	AND ID_SMS =		" & Agro_SQL_SaveNum_NULL(ID_SMS))
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
