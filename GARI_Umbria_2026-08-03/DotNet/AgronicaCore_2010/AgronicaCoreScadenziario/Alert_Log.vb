Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi

'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################

Public Class Alert_Log_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub


End Class


Public Class Alert_Log_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    Public Function Leggi(ByVal ID_Alert_Log As Int32,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           Optional ByVal xFiltroAggiuntivo As String = "",
                           Optional ByVal xOrderBy As String = ""
                           ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Log_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT *  ")
            StrSQL.Append(" FROM  Alert_Log ")
            StrSQL.Append(" WHERE 1 = 1 ")

            If ID_Alert_Log <> 0 Then
                StrSQL.Append(" AND ID_Alert_Log = " & Agro_SQL_SaveNum(ID_Alert_Log) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

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



    Public Function Scrivi(ByVal ID_Alert_Log As Int32,
                           ByVal Piva As String,
                           ByVal ID_Elenco As Integer,
                           ByVal ID_Tipologia As Integer,
                           ByVal Descrizione As String,
                           ByVal Tipo_Operazione As enum_TipoOperazioneDB,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                           ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Log_W.Scrivi()"

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
            StrSQL.Append("INSERT INTO Alert_Log ")
            StrSQL.Append("                   ( PivaSuperUser, Piva, ID_Alert_Log, Descrizione, ID_Elenco, ID_Tipologia, Tipo_Operazione  ")

            StrSQL.Append("                    ,Inviato,            DataInvio, ")
            StrSQL.Append("                    Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("                    UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("                    Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("                   ) ")

            StrSQL.Append("VALUES (")


            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(ID_Alert_Log) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Descrizione) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(ID_Elenco) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(ID_Tipologia) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Tipo_Operazione) & "  ")

            StrSQL.Append("         , 0 ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ," & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            StrSQL.Append("         ," & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.Append(" )")
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


End Class
