Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Audit_Dettagli_W
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function ModificaID_NC_AuditMacchine( _
                      ByVal Audit_Cod As Integer _
                    , ByVal Audit_Tipo As Integer _
                    , ByVal Regolamento_Cod As Integer _
                    , ByVal Mac_Cod As Integer _
                    , ByVal ID_Dettaglio As Integer _
                    , ByVal ID_Dettaglio2 As Integer? _
                    , ByVal ID_NC As Integer _
                    , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Dettagli_W.ModificaID_NC_AuditMacchine()"

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

            If IsNothing(ID_Dettaglio2) OrElse ID_Dettaglio2 = -1 Then
                StrSQL.AppendLine(" UPDATE AUDIT_MacchineXDettagli ")
            Else
                StrSQL.AppendLine(" UPDATE AUDIT_MacchineXDettagli2 ")
            End If

            StrSQL.AppendLine(" SET ID_NC = " & Agro_SQL_SaveNum(ID_NC) & " ")
            StrSQL.AppendLine(" WHERE 1=1 ")
            StrSQL.AppendLine(" AND Audit_Cod = " & Agro_SQL_SaveNum(Audit_Cod))
            StrSQL.AppendLine(" AND Audit_Tipo = " & Agro_SQL_SaveNum(Audit_Tipo))
            StrSQL.AppendLine(" AND Audit_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine(" AND Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod))
            StrSQL.AppendLine(" AND Mac_Cod = " & Agro_SQL_SaveNum(Mac_Cod))
            StrSQL.AppendLine(" AND ID_Dettaglio = " & Agro_SQL_SaveNum(ID_Dettaglio))

            If Not IsNothing(ID_Dettaglio2) AndAlso ID_Dettaglio2 <> -1 Then
                StrSQL.AppendLine(" AND ID_Dettaglio2 = " & Agro_SQL_SaveNum(ID_Dettaglio2))
            End If
            '--------------------------------------------


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

    Public Function ModificaID_NC_AuditFabbricati( _
                  ByVal Audit_Cod As Integer _
                , ByVal Audit_Tipo As Integer _
                , ByVal Regolamento_Cod As Integer _
                , ByVal Piva As String _
                , ByVal sa_cod As Integer _
                , ByVal fabbricato_cod As Integer _
                , ByVal ID_Dettaglio As Integer _
                , ByVal ID_Dettaglio2 As Integer? _
                , ByVal ID_NC As Integer _
                , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
            ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Dettagli_W.ModificaID_NC_AuditFabbricati()"

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

            If IsNothing(ID_Dettaglio2) OrElse ID_Dettaglio2 = -1 Then
                StrSQL.AppendLine(" UPDATE AUDIT_FabbricatiXDettagli ")
            Else
                StrSQL.AppendLine(" UPDATE AUDIT_FabbricatiXDettagli2 ")
            End If

            StrSQL.AppendLine(" SET ID_NC = " & Agro_SQL_SaveNum(ID_NC) & " ")
            StrSQL.AppendLine(" WHERE 1=1 ")
            StrSQL.AppendLine(" AND Audit_Cod = " & Agro_SQL_SaveNum(Audit_Cod))
            StrSQL.AppendLine(" AND Audit_Tipo = " & Agro_SQL_SaveNum(Audit_Tipo))
            StrSQL.AppendLine(" AND Audit_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine(" AND Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod))
            StrSQL.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.AppendLine(" AND sa_Cod = " & Agro_SQL_SaveNum(sa_cod))
            StrSQL.AppendLine(" AND fabbricato_cod = " & Agro_SQL_SaveNum(fabbricato_cod))
            StrSQL.AppendLine(" AND ID_Dettaglio = " & Agro_SQL_SaveNum(ID_Dettaglio))

            If Not IsNothing(ID_Dettaglio2) AndAlso ID_Dettaglio2 <> -1 Then
                StrSQL.AppendLine(" AND ID_Dettaglio2 = " & Agro_SQL_SaveNum(ID_Dettaglio2))
            End If
            '--------------------------------------------


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


    Public Function ModificaID_NC_AuditElementi( _
                  ByVal Audit_Cod As Integer _
                , ByVal Audit_Tipo As Integer _
                , ByVal Regolamento_Cod As Integer _
                , ByVal Elemento_Cod As Integer _
                , ByVal ID_Dettaglio As Integer _
                , ByVal ID_Dettaglio2 As Integer? _
                , ByVal ID_NC As Integer _
                , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
            ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Dettagli_W.ModificaID_NC_AuditElementi()"

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

            If IsNothing(ID_Dettaglio2) OrElse ID_Dettaglio2 = -1 Then
                StrSQL.AppendLine(" UPDATE AUDIT_ElementiXDettagli ")
            Else
                StrSQL.AppendLine(" UPDATE AUDIT_ElementiXDettagli2 ")
            End If

            StrSQL.AppendLine(" SET ID_NC = " & Agro_SQL_SaveNum(ID_NC) & " ")
            StrSQL.AppendLine(" WHERE 1=1 ")
            StrSQL.AppendLine(" AND Audit_Cod = " & Agro_SQL_SaveNum(Audit_Cod))
            StrSQL.AppendLine(" AND Audit_Tipo = " & Agro_SQL_SaveNum(Audit_Tipo))
            StrSQL.AppendLine(" AND Audit_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine(" AND Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod))
            StrSQL.AppendLine(" AND Elemento_Cod = " & Agro_SQL_SaveNum(Elemento_Cod))
            StrSQL.AppendLine(" AND ID_Dettaglio = " & Agro_SQL_SaveNum(ID_Dettaglio))

            If Not IsNothing(ID_Dettaglio2) AndAlso ID_Dettaglio2 <> -1 Then
                StrSQL.AppendLine(" AND ID_Dettaglio2 = " & Agro_SQL_SaveNum(ID_Dettaglio2))
            End If
            '--------------------------------------------


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
