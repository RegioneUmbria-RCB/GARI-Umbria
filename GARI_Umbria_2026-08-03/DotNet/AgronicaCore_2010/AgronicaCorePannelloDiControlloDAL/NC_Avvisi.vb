Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class NC_Avvisi_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi( _
                                ByVal ID_Avviso As Integer?, _
                                ByVal Area As String, _
                                ByVal ID_Evento As Integer?, _
                                ByVal GGAttesa As Integer?, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCorePannelloDiControlloDAL.NC_Avvisi_R.Leggi"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.AppendLine(" SELECT ID_Avviso, Area, ID_Evento, GGAttesa, MailMittente, MailA, MailCC, MailA_IncludiResponsabile, MailCC_IncludiResponsabile ")
            strSQL.AppendLine(" FROM NC_Avvisi ")

            strSQL.AppendLine(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))

            If Not IsNothing(ID_Avviso) Then
                strSQL.AppendLine(" AND ID_Avviso = " & Agro_SQL_SaveNum_NULL(ID_Avviso))
            End If

            If Not IsNothing(Area) Then
                strSQL.AppendLine(" AND Area = " & Agro_SQL_SaveText_NULL(Area))
            End If

            If Not IsNothing(ID_Evento) Then
                strSQL.AppendLine(" AND ID_Evento = " & Agro_SQL_SaveNum_NULL(ID_Evento))
            End If

            If Not IsNothing(GGAttesa) Then
                strSQL.AppendLine(" AND GGAttesa = " & Agro_SQL_SaveNum_NULL(GGAttesa))
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
End Class

Public Class NC_Avvisi_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Modifica(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            ByVal Old_ID_Avviso As Integer,
                            ByVal New_Area As String,
                            ByVal New_ID_Evento As Integer,
                            ByVal New_GGAttesa As Integer,
                            ByVal New_MailMittente As String,
                            ByVal New_MailA As String,
                            ByVal New_MailCC As String,
                            ByVal New_MailA_IncludiResponsabile As Boolean,
                            ByVal New_MailCC_IncludiResponsabile As Boolean,
                            Optional ByVal Data_modifica As Date = #2/1/1900#,
                            Optional ByVal username_modifica As String = ""
                            ) As Boolean


        Dim NomeRoutine As String = "AgronicaCorePannelloDiControlloDAL.NC_Avvisi_W.Modifica()"

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
            StrSQL.AppendLine(" UPDATE NC_Avvisi SET")

            StrSQL.AppendLine("  Area = " & Agro_SQL_SaveText_NULL(New_Area))
            StrSQL.AppendLine(", ID_Evento = " & Agro_SQL_SaveNum_NULL(New_ID_Evento))
            StrSQL.AppendLine(", GGAttesa = " & Agro_SQL_SaveNum_NULL(New_GGAttesa))
            StrSQL.AppendLine(", MailMittente = " & Agro_SQL_SaveText_NULL(New_MailMittente))
            StrSQL.AppendLine(", MailA = " & Agro_SQL_SaveText_NULL(New_MailA))
            StrSQL.AppendLine(", MailCC = " & Agro_SQL_SaveText_NULL(New_MailCC))
            StrSQL.AppendLine(", MailA_IncludiResponsabile = " & Agro_SQL_SaveNum_NULL(New_MailA_IncludiResponsabile))
            StrSQL.AppendLine(", MailCC_IncludiResponsabile = " & Agro_SQL_SaveNum_NULL(New_MailCC_IncludiResponsabile))


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
                            ByVal Area As String,
                            ByVal ID_Evento As Integer,
                            ByVal GGAttesa As Integer,
                            ByVal MailMittente As String,
                            ByVal MailA As String,
                            ByVal MailCC As String,
                            ByVal New_MailA_IncludiResponsabile As Boolean,
                            ByVal New_MailCC_IncludiResponsabile As Boolean,
                            Optional ByVal Data_creazione As Date = #2/1/1900#,
                            Optional ByVal Data_modifica As Date = #2/1/1900#,
                            Optional ByVal username_creazione As String = "",
                            Optional ByVal username_modifica As String = ""
                            ) As Boolean


        Dim NomeRoutine As String = "AgronicaCorePannelloDiControlloDAL.NC_Avvisi_W.Scrivi()"

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
            StrSQL.AppendLine(" INSERT INTO  NC_Avvisi")

            StrSQL.AppendLine("              (")
            StrSQL.AppendLine("              PivaSuperUser,      ")
            StrSQL.AppendLine("              ID_Avviso,           Area, ")
            StrSQL.AppendLine("              ID_Evento,           GGAttesa, ")
            StrSQL.AppendLine("              MailMittente,        MailA, ")
            StrSQL.AppendLine("              MailCC,              MailA_IncludiResponsabile, ")
            StrSQL.AppendLine("              MailCC_IncludiResponsabile, ")

            StrSQL.AppendLine("              Inviato,            datainvio, ")
            StrSQL.AppendLine("              Data_Creazione,     Data_Modifica, ")
            StrSQL.AppendLine("              UserName_Creazione, UserName_Modifica, ")
            StrSQL.AppendLine("              Validita_Inizio,    Validita_Fine ")
            StrSQL.AppendLine("              ) ")

            StrSQL.AppendLine(" VALUES ( ")

            StrSQL.AppendLine("			 " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.AppendLine("			," & Agro_SQL_SaveNum_NULL(ID_Avviso))
            StrSQL.AppendLine("			," & Agro_SQL_SaveText_NULL(Area))
            StrSQL.AppendLine("			," & Agro_SQL_SaveNum_NULL(ID_Evento))
            StrSQL.AppendLine("			," & Agro_SQL_SaveNum_NULL(GGAttesa))
            StrSQL.AppendLine("			," & Agro_SQL_SaveText_NULL(MailMittente))
            StrSQL.AppendLine("			," & Agro_SQL_SaveText_NULL(MailA))
            StrSQL.AppendLine("			," & Agro_SQL_SaveText_NULL(MailCC))
            StrSQL.AppendLine("			," & Agro_SQL_SaveNum_NULL(New_MailA_IncludiResponsabile))
            StrSQL.AppendLine("			," & Agro_SQL_SaveNum_NULL(New_MailCC_IncludiResponsabile))

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
        Dim NomeRoutine As String = "AgronicaCorePannelloDiControlloDAL.NC_Avvisi_W.Cancella()"

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
                StrSQL.AppendLine(" DELETE FROM NC_Avvisi ")
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