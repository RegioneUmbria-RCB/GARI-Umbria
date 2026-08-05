Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class Alert_Avvisi_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(
                                ByVal ID_Avviso As Integer?,
                                ByVal ID_Area As Integer?,
                                ByVal ID_Tipologia As Integer?,
                                ByVal Filtro_RapCon As String,
                                ByVal ID_Evento As Integer?,
                                ByVal GGAttesa As Integer?,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCorePannelloDiControlloDAL.Alert_Avvisi_R.Leggi"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.AppendLine(" SELECT avv.ID_Avviso, avv.ID_Area, avv.ID_Evento, avv.GGAttesa, avv.GGAttesa as GGAttesa_Old, avv.MailMittente, avv.MailA, avv.MailCC, ar.Nome as Area, isNull(avv.ID_Tipologia, 0) as ID_Tipologia, isNull(ti.Nome, '') as Tipologia, avv.Filtro_RapCon ")
            strSQL.AppendLine(" FROM Alert_Avvisi avv")
            strSQL.AppendLine(" INNER JOIN Alert_Area ar ON ar.ID_Area=avv.ID_Area")
            strSQL.AppendLine(" Left Outer JOIN Alert_Tipologia ti ON ti.ID_Tipologia=avv.ID_Tipologia")

            strSQL.AppendLine(" WHERE avv.PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))

            If Not IsNothing(ID_Avviso) Then
                strSQL.AppendLine(" AND avv.ID_Avviso = " & Agro_SQL_SaveNum_NULL(ID_Avviso))
            End If

            If Not IsNothing(ID_Area) Then
                strSQL.AppendLine(" AND avv.ID_Area = " & Agro_SQL_SaveNum_NULL(ID_Area))
            End If

            If Not IsNothing(ID_Tipologia) Then
                strSQL.AppendLine(" AND avv.ID_Tipologia In (0, " & Agro_SQL_SaveNum_NULL(ID_Tipologia) & ")")
            End If

            If Trim(Filtro_RapCon) <> "" Then
                strSQL.AppendLine(" AND avv.Filtro_RapCon = '" & Agro_SQL_SaveText_NULL(Filtro_RapCon) & "' ")
            End If

            If Not IsNothing(ID_Evento) Then
                strSQL.AppendLine(" AND avv.ID_Evento = " & Agro_SQL_SaveNum_NULL(ID_Evento))
            End If

            If Not IsNothing(GGAttesa) Then
                strSQL.AppendLine(" AND avv.GGAttesa = " & Agro_SQL_SaveNum_NULL(GGAttesa))
            End If


            If xFiltroAggiuntivo <> "" Then
                strSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    strSQL.AppendLine(" AND   avv.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    strSQL.AppendLine(" AND   avv.Inviato =-1 ")
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
End Class

Public Class Alert_Avvisi_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Modifica(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            ByVal Old_ID_Avviso As Integer,
                            ByVal New_ID_Area As Integer,
                            ByVal New_ID_Tipologia As Integer,
                            ByVal New_Filtro_RapCon As String,
                            ByVal New_ID_Evento As Integer,
                            ByVal New_GGAttesa As Integer,
                            ByVal New_MailMittente As String,
                            ByVal New_MailA As String,
                            ByVal New_MailCC As String,
                            Optional ByVal Data_modifica As Date = #2/1/1900#,
                            Optional ByVal username_modifica As String = ""
                            ) As Boolean


        Dim NomeRoutine As String = "AgronicaCorePannelloDiControlloDAL.Alert_Avvisi_W.Modifica()"

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
            StrSQL.AppendLine(" UPDATE Alert_Avvisi SET")

            StrSQL.AppendLine("  ID_Area = " & Agro_SQL_SaveNum_NULL(New_ID_Area))
            StrSQL.AppendLine(", ID_Tipologia = " & Agro_SQL_SaveNum_NULL(New_ID_Tipologia))
            StrSQL.AppendLine(", Filtro_RapCon = " & Agro_SQL_SaveText_NULL(New_Filtro_RapCon))
            StrSQL.AppendLine(", ID_Evento = " & Agro_SQL_SaveNum_NULL(New_ID_Evento))
            StrSQL.AppendLine(", GGAttesa = " & Agro_SQL_SaveNum_NULL(New_GGAttesa))
            StrSQL.AppendLine(", MailMittente = " & Agro_SQL_SaveText_NULL(New_MailMittente))
            StrSQL.AppendLine(", MailA = " & Agro_SQL_SaveText_NULL(New_MailA))
            StrSQL.AppendLine(", MailCC = " & Agro_SQL_SaveText_NULL(New_MailCC))

            StrSQL.AppendLine(", Data_Modifica = " & Agro_SQL_SaveDateTime(Data_modifica))
            StrSQL.AppendLine(", UserName_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "' ")

            StrSQL.AppendLine("	WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.AppendLine("	AND ID_Avviso  =		" & Agro_SQL_SaveNum_NULL(Old_ID_Avviso))


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
                            ByVal ID_Avviso As Integer,
                            ByVal ID_Area As Integer,
                            ByVal ID_Tipologia As Integer,
                            ByVal Filtro_RapCon As String,
                            ByVal ID_Evento As Integer,
                            ByVal GGAttesa As Integer,
                            ByVal MailMittente As String,
                            ByVal MailA As String,
                            ByVal MailCC As String,
                            Optional ByVal Data_creazione As Date = #2/1/1900#,
                            Optional ByVal Data_modifica As Date = #2/1/1900#,
                            Optional ByVal username_creazione As String = "",
                            Optional ByVal username_modifica As String = ""
                            ) As Boolean


        Dim NomeRoutine As String = "AgronicaCorePannelloDiControlloDAL.Alert_Avvisi_W.Scrivi()"

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
            StrSQL.AppendLine(" INSERT INTO  Alert_Avvisi")

            StrSQL.AppendLine("              (")
            StrSQL.AppendLine("              PivaSuperUser,      ")
            StrSQL.AppendLine("              ID_Avviso,           ID_Area,     ID_Tipologia,   ")
            StrSQL.AppendLine("              Filtro_RapCon,       ID_Evento,   GGAttesa, ")
            StrSQL.AppendLine("              MailMittente,        MailA, ")
            StrSQL.AppendLine("              MailCC,              ")

            StrSQL.AppendLine("              Inviato,            datainvio, ")
            StrSQL.AppendLine("              Data_Creazione,     Data_Modifica, ")
            StrSQL.AppendLine("              UserName_Creazione, UserName_Modifica, ")
            StrSQL.AppendLine("              Validita_Inizio,    Validita_Fine ")
            StrSQL.AppendLine("              ) ")

            StrSQL.AppendLine(" VALUES ( ")

            StrSQL.AppendLine("			 " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.AppendLine("			," & Agro_SQL_SaveNum_NULL(ID_Avviso))
            StrSQL.AppendLine("			," & Agro_SQL_SaveNum_NULL(ID_Area))
            StrSQL.AppendLine("			," & Agro_SQL_SaveNum_NULL(ID_Tipologia))
            StrSQL.AppendLine("			," & Agro_SQL_SaveText_NULL(Filtro_RapCon))
            StrSQL.AppendLine("			," & Agro_SQL_SaveNum_NULL(ID_Evento))
            StrSQL.AppendLine("			," & Agro_SQL_SaveNum_NULL(GGAttesa))
            StrSQL.AppendLine("			," & Agro_SQL_SaveText_NULL(MailMittente))
            StrSQL.AppendLine("			," & Agro_SQL_SaveText_NULL(MailA))
            StrSQL.AppendLine("			," & Agro_SQL_SaveText_NULL(MailCC))

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
                             ByVal ID_Avviso As Integer,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCorePannelloDiControlloDAL.Alert_Avvisi_W.Cancella()"

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
                StrSQL.AppendLine(" DELETE FROM Alert_Avvisi ")
                StrSQL.AppendLine(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
                StrSQL.AppendLine("	AND ID_Avviso =		" & Agro_SQL_SaveNum_NULL(ID_Avviso))
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