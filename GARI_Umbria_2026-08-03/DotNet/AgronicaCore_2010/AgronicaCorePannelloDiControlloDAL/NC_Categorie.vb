Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class NC_Categorie_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi( _
                            ByVal ID_Categoria As Integer?, _
                            ByVal Area As String, _
                            ByVal Tipologia As String, _
                            ByVal xFiltroAggiuntivo As String, _
                            ByVal xOrderBy As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCorePannelloDiControlloDAL.NC_Categorie_R.Leggi"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.AppendLine(" SELECT ID_Categoria, Area, Tipologia ")
            strSQL.AppendLine(" FROM NC_Categorie ")

            strSQL.AppendLine(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))

            If Not IsNothing(ID_Categoria) Then
                strSQL.AppendLine(" AND ID_Categoria = " & Agro_SQL_SaveNum_NULL(ID_Categoria))
            End If

            If Not IsNothing(Area) Then
                strSQL.AppendLine(" AND Area = " & Agro_SQL_SaveText_NULL(Area))
            End If

            If Not IsNothing(Tipologia) Then
                strSQL.AppendLine(" AND Tipologia = " & Agro_SQL_SaveText_NULL(Tipologia))
            End If

            If xFiltroAggiuntivo <> "" Then
                strSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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
            Else
                strSQL.AppendLine(" ORDER BY Area, Tipologia ")
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

Public Class NC_Categorie_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Modifica(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            ByVal Old_ID_Categoria As Integer,
                            ByVal New_Area As String,
                            ByVal New_Tipologia As String,
                            Optional ByVal Data_modifica As Date = #2/1/1900#,
                            Optional ByVal username_modifica As String = ""
                            ) As Boolean


        Dim NomeRoutine As String = "AgronicaCorePannelloDiControlloDAL.NC_Categorie_W.Modifica"

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
            StrSQL.AppendLine(" UPDATE NC_Categorie SET")

            StrSQL.AppendLine("  Area = " & Agro_SQL_SaveText_NULL(New_Area))
            StrSQL.AppendLine(", Tipologia = " & Agro_SQL_SaveText_NULL(New_Tipologia))

            StrSQL.AppendLine(", Data_Modifica = " & Agro_SQL_SaveDateTime(Data_modifica))
            StrSQL.AppendLine(", UserName_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "' ")

            StrSQL.AppendLine("	WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.AppendLine("	AND ID_Categoria  =		" & Agro_SQL_SaveNum_NULL(Old_ID_Categoria))


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
                            ByVal ID_Categoria As Integer,
                            ByVal Area As String,
                            ByVal Tipologia As String,
                            Optional ByVal Data_creazione As Date = #2/1/1900#,
                            Optional ByVal Data_modifica As Date = #2/1/1900#,
                            Optional ByVal username_creazione As String = "",
                            Optional ByVal username_modifica As String = ""
                            ) As Boolean


        Dim NomeRoutine As String = "AgronicaCorePannelloDiControlloDAL.NC_Categorie_W.Scrivi"

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
            StrSQL.AppendLine(" INSERT INTO  NC_Categorie" + vbCrLf)

            StrSQL.AppendLine("              (")
            StrSQL.AppendLine("              PivaSuperUser,                 ")
            StrSQL.AppendLine("              ID_Categoria,                  ")
            StrSQL.AppendLine("              Area,                          Tipologia, ")

            StrSQL.AppendLine("              Inviato,            datainvio, ")
            StrSQL.AppendLine("              Data_Creazione,     Data_Modifica, ")
            StrSQL.AppendLine("              UserName_Creazione, UserName_Modifica, ")
            StrSQL.AppendLine("              Validita_Inizio,    Validita_Fine ")
            StrSQL.AppendLine("              ) ")

            StrSQL.AppendLine(" VALUES ( ")

            StrSQL.AppendLine("			 " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.AppendLine("			," & Agro_SQL_SaveNum_NULL(ID_Categoria))
            StrSQL.AppendLine("			," & Agro_SQL_SaveText_NULL(Area))
            StrSQL.AppendLine("			," & Agro_SQL_SaveText_NULL(Tipologia))

            StrSQL.AppendLine("         , 0  ")
            StrSQL.AppendLine("         , Null  ")
            StrSQL.AppendLine("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            StrSQL.AppendLine("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            StrSQL.AppendLine("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.AppendLine("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.AppendLine("			, " & Agro_SQL_SaveDate(AGRODATAINIZIO) & "  ")
            StrSQL.AppendLine("			, " & Agro_SQL_SaveDate(AGRODATAFINE) & "  ")

            StrSQL.Append(") ")

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
                             ByVal ID_Categoria As Integer,
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                              ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCorePannelloDiControlloDAL.PnlCtrl_Elementi_W.Cancella()"

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
                StrSQL.AppendLine(" DELETE FROM NC_Categorie ")
                StrSQL.AppendLine(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
                StrSQL.AppendLine("	AND ID_Categoria =		" & Agro_SQL_SaveNum_NULL(ID_Categoria))
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