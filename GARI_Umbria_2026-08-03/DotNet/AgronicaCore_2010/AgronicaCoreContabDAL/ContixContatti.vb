Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreEntityFramework
Imports System.Xml

Public Class ContixContatti_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi_Estesa(
            ByVal PivaSuperUser As String,
            ByVal Cod_Contatto As String,
            ByVal Piva As String,
            ByVal xFiltroAggiuntivo As String,
            ByVal xOrderBy As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
            Optional ByVal Ric_Cod As Long = 0,
            Optional ByVal Anno As Long = 0,
            Optional ByVal Cod_Conto As Long = 0,
            Optional ByVal Id_Riclassificazione As String = ""
        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreContabDAL.ContixContatti_W.Leggi_Estesa()"
        Dim StbSQL As New System.Text.StringBuilder
        Dim MessaggioErrore As String = ""
        Dim dtConti As DataTable

        Try

            StbSQL.Append(" Select DISTINCT" + vbCrLf)
            StbSQL.Append(" RicXConti.*, Conti.Conto_Descr, Conti.Flag_UE," + vbCrLf)
            StbSQL.Append(" ContixContatti.Cod_Contatto, Conti.Extra_Int, Conti.Extra_Str, Conti.Extra_Date, " + vbCrLf)
            StbSQL.Append(" ContixContatti.Validita_Inizio, ContixContatti.Validita_Fine " + vbCrLf)
            StbSQL.Append(" FROM  Conti, ")
            StbSQL.Append(" RicXConti, Riclassificazioni , ContixContatti " + vbCrLf)
            StbSQL.Append(" WHERE " + vbCrLf)
            StbSQL.Append(" Conti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " + vbCrLf)
            StbSQL.Append(" AND Conti.Validita_fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " " + vbCrLf)
            StbSQL.Append(" AND RicXConti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " + vbCrLf)
            StbSQL.Append(" AND RicXConti.Validita_fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " " + vbCrLf)
            StbSQL.Append(" AND Riclassificazioni.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " + vbCrLf)
            StbSQL.Append(" AND Riclassificazioni.Validita_fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " " + vbCrLf)
            StbSQL.Append(" AND Conti.Cod_Conto = RicXConti.Cod_Conto " + vbCrLf)
            StbSQL.Append(" AND (Riclassificazioni.Ric_Cod = RicXConti.Ric_Cod  or Riclassificazioni.Ric_Cod = 1 ) " + vbCrLf)

            StbSQL.Append(" AND (Riclassificazioni.Piva = RicXConti.Piva ")
            StbSQL.Append(" OR Riclassificazioni.Piva_Riferimento = RicXConti.Piva ")
            StbSQL.Append(" OR Riclassificazioni.Piva = 'AAAAAAAAAAA') " + vbCrLf)
            StbSQL.Append(" AND (Riclassificazioni.Piva   = Conti.Piva ")
            StbSQL.Append(" OR Riclassificazioni.Piva_Riferimento   = Conti.Piva ")
            StbSQL.Append(" OR Riclassificazioni.Piva = 'AAAAAAAAAAA' ) " + vbCrLf)
            StbSQL.Append(" AND RicXConti.Piva = '" & Agro_SQL_SaveText(Piva) & "' " + vbCrLf)
            StbSQL.Append(" and ContixContatti.Cod_Conto = RicXConti.Cod_Conto " + vbCrLf)
            StbSQL.Append(" and ContixContatti.PIVA = RicXConti.Piva" + vbCrLf)
            'StbSQL.Append(" AND ( Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' OR  Contatti.Sa_Cod = -1 ) " + vbCrLf)

            If Ric_Cod <> 0 Then
                StbSQL.Append(" and RicXConti.Ric_Cod = " & Agro_SQL_SaveNum(Ric_Cod) + vbCrLf)
            End If

            If Not String.IsNullOrEmpty(Cod_Contatto) Then
                StbSQL.Append(" and ContixContatti.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "' " + vbCrLf)
            End If

            If Anno <> 0 Then
                StbSQL.Append(" AND RicXConti.Anno = " & Agro_SQL_SaveNum(Anno) + vbCr)
            End If

            If Cod_Conto <> 0 Then
                StbSQL.Append(" AND RicXConti.Cod_Conto = " & Agro_SQL_SaveNum(Cod_Conto) + vbCr)
            End If

            If Not String.IsNullOrEmpty(Id_Riclassificazione) Then
                StbSQL.Append(" And RicXConti.Id_Riclassificazione = '" & Agro_SQL_SaveText(Id_Riclassificazione) & "' " + vbCrLf)
            End If

            If Not String.IsNullOrEmpty(xFiltroAggiuntivo) Then
                StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) + vbCrLf)
            End If

            If xOrderBy <> "" Then
                StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.Append(" ORDER BY RicXConti.Piva ASC, RicXConti.Ric_Cod ASC, RicXConti.Cod_Conto ASC ")
            End If

            '--------------------------------------------------------------------------
            dtConti = EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)
            '-------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return dtConti

    End Function

End Class

Public Class ContixContatti_W
    Inherits AgronicaCoreDataProvider.DataProvider
    Public Function Scrivi(ByVal PIVA As String,
                             ByVal Cod_Conto As Integer,
                             ByVal Cod_Contatto As String,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                             , Optional ByVal username_creazione As String = "" _
                             , Optional ByVal username_modifica As String = ""
                             ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.ContixContatti_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If


            '---------------------------------------------
            'Query per l'inserimento dei dati                 ' #### CLASSE ####
            StrSQL.Length = 0

            StrSQL.Append("INSERT INTO ContixContatti ")
            StrSQL.Append(" (PIVA_SuperUser,    Piva,   Cod_Conto,  Cod_Contatto, ")
            StrSQL.Append(" Inviato,            DataInvio, ")
            StrSQL.Append(" UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append(" Data_Creazione, Data_Modifica, ")
            StrSQL.Append(" Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("    ) ")

            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(PIVA) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Cod_Conto))
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Cod_Contatto) & "' ")
            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append(")")

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

    Public Function Cancella(ByVal PIVA As String,
                             ByVal Cod_Conto As Integer,
                             ByVal Cod_Contatto As String,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.ContixContatti_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            '---------------------------------------------
            StrSQL.Length = 0

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Append(" UPDATE  ContixContatti ")
                StrSQL.Append(" SET ")
                StrSQL.Append("          Username_Modifica = '" & objParametri.UsernameOperazione & "' ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE    Inviato > 0 ")
            Else

                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM  ContixContatti ")
                StrSQL.Append(" WHERE   Inviato = 0 ")

            End If

            StrSQL.Append(" AND PIVA_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")

            If PIVA <> "" Then
                StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(PIVA) & "'   ")
            End If

            If Cod_Contatto <> "" Then
                StrSQL.Append(" AND Cod_Contatto = '" & Agro_SQL_SaveText(Trim(Cod_Contatto)) & "'   ")
            End If

            If Cod_Conto <> 0 Then
                StrSQL.Append(" AND Cod_Conto = " & Agro_SQL_SaveNum(Cod_Conto) & " ")
            End If

            '----------------------------------------------------------------------
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