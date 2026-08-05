
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class Pratiche_Stati_ModelliPrevisionali_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(
        ByVal Pratica_Cod As Int32,
        ByVal Stato_Cod As Int32,
        ByVal Passaggio_Di_Stato_Cod As Int32,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT *, '' as Pacchetto_Des " & vbCrLf)
            StrSQL.Append(" FROM Pratiche_Stati_ModelliPrevisionali " & vbCrLf)
            StrSQL.Append(" WHERE 1=1 " & vbCrLf)

            If Pratica_Cod <> 0 Then
                StrSQL.Append(" AND Pratica_Cod = " & Agro_SQL_SaveNum(Pratica_Cod) & " ")
            End If

            If Stato_Cod <> 0 Then
                StrSQL.Append(" AND Stato_Cod = " & Agro_SQL_SaveNum(Stato_Cod) & " ")
            End If

            If Passaggio_Di_Stato_Cod <> 0 Then
                StrSQL.Append(" AND PassaggioDiStato_cod = " & Agro_SQL_SaveNum(Passaggio_Di_Stato_Cod) & " ")
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


'#################################################################
'#################################################################
'#################################################################

Public Class Pratiche_Stati_ModelliPrevisionali_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################
    Public Function Scrivi(
                  ByVal Piva_SuperUser As String _
                , ByVal Piva As String _
                , ByVal PassaggioDiStato_cod As Integer _
                , ByVal Pratica_Cod As Integer _
                , ByVal Stato_Cod As Integer _
                , ByVal Mod_Cod As Integer _
                , ByVal Stato_Origine_Cod As Integer _
                , ByVal ModelliPrevisionali_Gruppi_COD As Integer _
                , ByVal Scadenza As DateTime _
                , ByVal Note As String _
                , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                ByVal Data_creazione As Date,
                ByVal Data_modifica As Date,
                ByVal username_creazione As String,
                ByVal username_modifica As String
    ) As Boolean


        Dim NomeRoutine As String = "Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
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
            strSQL.Length = 0
            strSQL.Append(" INSERT Pratiche_Stati_ModelliPrevisionali  " & vbCrLf)

            strSQL.Append("              (")

            strSQL.Append("   [Piva_SuperUser] " & vbCrLf)
            strSQL.Append("  ,[Piva] " & vbCrLf)
            strSQL.Append("  ,[PassaggioDiStato_cod] " & vbCrLf)
            strSQL.Append("  ,[Pratica_Cod] " & vbCrLf)
            strSQL.Append("  ,[Stato_Cod] " & vbCrLf)
            strSQL.Append("  ,[Mod_Cod] " & vbCrLf)
            strSQL.Append("  ,[Stato_Origine_Cod] " & vbCrLf)
            strSQL.Append("  ,[ModelliPrevisionali_Gruppi_COD] " & vbCrLf)
            strSQL.Append("  ,[Scadenza] " & vbCrLf)
            strSQL.Append("  ,[Note] " & vbCrLf)


            strSQL.Append("              , Inviato,            datainvio, ")
            strSQL.Append("              Data_Creazione,     Data_Modifica, ")
            strSQL.Append("              UserName_Creazione, UserName_Modifica ")
            strSQL.Append("              ) ")

            strSQL.Append(" VALUES ( ")

            strSQL.Append(" '" & Agro_SQL_SaveText(Piva_SuperUser) & "'" & vbCrLf)
            strSQL.Append(",'" & Agro_SQL_SaveText(Piva) & "'" & vbCrLf)
            strSQL.Append(", " & Agro_SQL_SaveNum(PassaggioDiStato_cod) & " " & vbCrLf)
            strSQL.Append(", " & Agro_SQL_SaveNum(Pratica_Cod) & " " & vbCrLf)
            strSQL.Append(", " & Agro_SQL_SaveNum(Stato_Cod) & " " & vbCrLf)
            strSQL.Append(", " & Agro_SQL_SaveNum(Mod_Cod) & " " & vbCrLf)
            strSQL.Append(", " & Agro_SQL_SaveNum(Stato_Origine_Cod) & " " & vbCrLf)
            strSQL.Append(", " & Agro_SQL_SaveNum(ModelliPrevisionali_Gruppi_COD) & " " & vbCrLf)
            strSQL.Append(",'" & Agro_SQL_SaveText(Scadenza) & "'" & vbCrLf)
            strSQL.Append(",'" & Agro_SQL_SaveText(Note) & "'" & vbCrLf)


            strSQL.Append("         , 0  " + vbCrLf)
            strSQL.Append("         , Null  " + vbCrLf)

            strSQL.Append("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            strSQL.Append("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            strSQL.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            strSQL.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")



            strSQL.Append(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSQL.ToString, NomeRoutine)
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
    Public Function CancellaPerPassaggioDiStato_Cod(
                              ByVal PassaggioDiStato_Cod As Integer,
                              ByVal xFiltroAggiuntivo As String,
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
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
                Stb.Append("         ,Data_Modifica= " & Agro_SQL_SaveDateTime(Date.Now) & " ")
                Stb.Append("         ,Inviato = -1 ")
                Stb.Append(" WHERE   1=1 ")
                Stb.Append(" AND     Inviato >= 0 ")
            Else
                Stb.Append(" DELETE FROM Pratiche_Stati_ModelliPrevisionali ")
                Stb.Append(" WHERE 1=1 ")
                Stb.Append(" AND PassaggioDiStato_Cod = " & PassaggioDiStato_Cod & " ")
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

