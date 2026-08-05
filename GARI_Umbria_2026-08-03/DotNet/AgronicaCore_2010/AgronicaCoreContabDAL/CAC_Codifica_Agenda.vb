
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class CAC_Codifica_Agenda_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function LeggiChiaveSistemaEsterno(
        byval CodiceSistemaEsterno As String,
        byval Cod_agenda_Cliente As String,
        ByVal xFiltroAggiuntivo As String, _
        ByVal xOrderBy As String, _
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.AppendLine(" select * ") 
            Stb.AppendLine(" from CAC_Codifica_Agenda ")
            Stb.AppendLine(" where CodiceSistemaEsterno = '" & Agro_SQL_SaveText(CodiceSistemaEsterno) & "' ")
            Stb.AppendLine(" and Cod_agenda_Cliente = '" & Agro_SQL_SaveText(Cod_agenda_Cliente) & "'")



            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    Stb.Append(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    Stb.Append(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
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


'#################################################################
'#################################################################
'#################################################################

Public Class CAC_Codifica_Agenda_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################
    Public Function Scrivi(
          ByVal Piva_SuperUser As String _
        , ByVal piva As String _
        , ByVal sa_cod As Integer _
        , ByVal id_agenda As Integer _
        , ByVal Cod_agenda_Cliente As String _
        , ByVal CodiceSistemaEsterno As String _
        , ByVal DataOra_Modifica_SistemaEsterno As DateTime _
        , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
        , Optional ByVal Data_creazione As Date = #2/1/1900# _
        , Optional ByVal Data_modifica As Date = #2/1/1900# _
        , Optional ByVal username_creazione As String = "" _
        , Optional ByVal username_modifica As String = ""
        ) As Boolean


        Dim NomeRoutine As String = "Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_creazione = #2/1/1900# Then
                Data_creazione = Date.Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If



            '---------------------------------------------
            stb.Length = 0
            stb.Append(" INSERT CAC_Codifica_Agenda " + vbCrLf)

            stb.Append("              (")
            stb.Append("   [Piva_SuperUser] " & vbCrLf)
            stb.Append("  ,[piva] " & vbCrLf)
            stb.Append("  ,[sa_cod] " & vbCrLf)
            stb.Append("  ,[id_agenda] " & vbCrLf)
            stb.Append("  ,[Cod_agenda_Cliente] " & vbCrLf)
            stb.Append("  ,[CodiceSistemaEsterno] " & vbCrLf)
            stb.Append("  ,[DataOra_Modifica_SistemaEsterno] " & vbCrLf)


            stb.Append("              , Inviato,            datainvio, ")
            stb.Append("              Data_Creazione,     Data_Modifica, ")
            stb.Append("              UserName_Creazione, UserName_Modifica ")
            stb.Append("              ) ")

            stb.Append(" VALUES ( ")

            stb.Append(" '" & Agro_SQL_SaveText(Piva_SuperUser) & "'" & vbCrLf)
            stb.Append(",'" & Agro_SQL_SaveText(piva) & "'" & vbCrLf)
            stb.Append(", " & Agro_SQL_SaveNum(sa_cod) & " " & vbCrLf)
            stb.Append(", " & Agro_SQL_SaveNum(id_agenda) & " " & vbCrLf)
            stb.Append(",'" & Agro_SQL_SaveText(Cod_agenda_Cliente) & "'" & vbCrLf)
            stb.Append(",'" & Agro_SQL_SaveText(CodiceSistemaEsterno) & "'" & vbCrLf)
            stb.Append(", " & Agro_SQL_SaveDateTime(DataOra_Modifica_SistemaEsterno) & " " & vbCrLf)


            stb.Append("         , 0  " + vbCrLf)
            stb.Append("         , Null  " + vbCrLf)

            stb.Append("			, " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            stb.Append("			, " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            stb.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            stb.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")



            stb.Append(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
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
    Public Function ModificaDataSistemaEsterno(
          ByVal Piva_SuperUser As String _
        , ByVal piva As String _
        , ByVal sa_cod As Integer _
        , ByVal id_agenda As Integer _
        , ByVal Cod_agenda_Cliente As String _
        , ByVal CodiceSistemaEsterno As String _
        , ByVal DataOra_Modifica_SistemaEsterno As DateTime _
        , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
        , Optional ByVal Data_creazione As Date = #2/1/1900# _
        , Optional ByVal Data_modifica As Date = #2/1/1900# _
        , Optional ByVal username_creazione As String = "" _
        , Optional ByVal username_modifica As String = ""
        ) As Boolean


        Dim NomeRoutine As String = "Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_creazione = #2/1/1900# Then
                Data_creazione = Date.Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If



            '---------------------------------------------
            Stb.Length = 0
            Stb.AppendLine(" update cac_codifica_agenda ")
            Stb.AppendLine(" set DataOra_Modifica_SistemaEsterno = " & Agro_SQL_SaveDateTime(DataOra_Modifica_SistemaEsterno) & " ")
            Stb.AppendLine(" where Piva_SuperUser  = '" & Piva_SuperUser & "' ")
            Stb.AppendLine(" and piva  = '" & Agro_SQL_SaveText(piva) & "' ")
            Stb.AppendLine(" and sa_cod  =  " & sa_cod)
            Stb.AppendLine(" and id_agenda  =  " & id_agenda)
            Stb.AppendLine(" and Cod_agenda_Cliente  =  '" & Agro_SQL_SaveText(Cod_agenda_Cliente) & "'")
            Stb.AppendLine(" and CodiceSistemaEsterno  = '" & Agro_SQL_SaveText(CodiceSistemaEsterno) & "' ")


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function





    '#################################################################
    Public Function Cancella(
          ByVal name As String _
        , ByVal Piva_SuperUser As String _
        , ByVal piva As String _
        , ByVal sa_cod As Integer _
        , ByVal id_agenda As Integer _
        , ByVal Cod_agenda_Cliente As String _
        , ByVal CodiceSistemaEsterno As String _
        , ByVal DataOra_Modifica_SistemaEsterno As DateTime _
        , ByVal xFiltroAggiuntivo As String _
        , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
        , Optional ByVal Data_creazione As Date = #2/1/1900# _
        , Optional ByVal Data_modifica As Date = #2/1/1900# _
        , Optional ByVal username_creazione As String = "" _
        , Optional ByVal username_modifica As String = ""
        ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "Cancella()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            Stb.Length = 0

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                Stb.Append(" UPDATE ... ")
                Stb.Append(" SET ")
                Stb.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                Stb.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                Stb.Append("         ,Inviato = -1 ")
                Stb.Append(" WHERE   1=1 ")
                Stb.Append(" AND     Inviato >= 0 ")
            Else
                Stb.Append(" DELETE FROM ... ")
                Stb.Append(" WHERE 1=1 ")
            End If
            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
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

