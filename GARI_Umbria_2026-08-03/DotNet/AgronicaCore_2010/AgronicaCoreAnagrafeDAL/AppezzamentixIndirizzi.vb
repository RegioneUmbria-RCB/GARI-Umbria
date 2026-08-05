Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreEntityFramework
Imports System.Transactions
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports System.Data.Entity

'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

Public Class AppezzamentixIndirizzi_Read
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function Leggi(
        ByVal Piva As String,
        ByVal Sa_Cod As Integer,
        ByVal Appezza As Integer,
        ByVal cod_indirizzo As Integer,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable



        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.AppezzamentixIndirizzi_Read.Leggi()"


        Dim MessaggioErrore As String = ""
        Dim DT As DataTable

        Try

            Dim stb As New Text.StringBuilder

            stb.AppendLine(" SELECT ")
            stb.AppendLine(" AppezzamentixIndirizzi.*, ")
            stb.AppendLine(" '' as Tipo_Indirizzo_Des, ")
            stb.AppendLine(" Indirizzi.ind_des, ")
            stb.AppendLine(" Indirizzi.frz_des, ")
            stb.AppendLine(" ISNULL(ISTAT.COMUNI_PROV, '') As pro_cod, ")
            stb.AppendLine(" ISNULL(ISTAT.LOCALITA, '') as com_des, ")
            stb.AppendLine(" Indirizzi.com_cod_istat, ")
            stb.AppendLine(" Indirizzi.pro_cod_istat, ")
            stb.AppendLine(" Indirizzi.note, ")
            stb.AppendLine(" Indirizzi.CAP, ")
            stb.AppendLine(" Indirizzi.stato, ")
            stb.AppendLine(" ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166.descrizione as Stato_Des ")
            stb.AppendLine(" FROM Indirizzi ")
            stb.AppendLine(" JOIN AppezzamentixIndirizzi On Indirizzi.cod_indirizzo = AppezzamentixIndirizzi.cod_indirizzo ")
            stb.AppendLine(" LEFT JOIN ISTAT On Indirizzi.pro_cod_istat = ISTAT.PROV And Indirizzi.com_cod_istat = ISTAT.COM ")
            stb.AppendLine(" LEFT JOIN ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166 On Indirizzi.stato = ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166.Codice ")
            stb.AppendLine(" WHERE AppezzamentixIndirizzi.Piva = " & Agro_SQL_SaveText_NULL(Piva) & " ")
            stb.AppendLine(" And AppezzamentixIndirizzi.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            stb.AppendLine(" And AppezzamentixIndirizzi.appezza = " & Agro_SQL_SaveNum(Appezza) & " ")

            If cod_indirizzo <> 0 Then
                stb.AppendLine(" And AppezzamentixIndirizzi.cod_indirizzo = " & Agro_SQL_SaveNum(cod_indirizzo) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
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


Public Class AppezzamentixIndirizzi_Write
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(
                            ByVal Piva As String,
                            ByVal Sa_Cod As Int32,
                            ByVal Appezza As Int32,
                            ByVal Cod_Indirizzo As Integer,
                            ByVal Tipo_Indirizzo As Integer,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            , Optional ByVal Data_creazione As Date = #2/1/1900# _
                            , Optional ByVal Data_modifica As Date = #2/1/1900# _
                            , Optional ByVal username_creazione As String = "" _
                            , Optional ByVal username_modifica As String = ""
    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.AppezzamentixIndirizzi_Write.Scrivi()"

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
            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO AppezzamentixIndirizzi(       ")
            StrSQL.Append("                    PIVA, Sa_Cod, Appezza, Cod_Indirizzo, ")
            StrSQL.Append("                    Tipo_Indirizzo, ")
            StrSQL.Append("                    Inviato,            DataInvio, ")
            StrSQL.Append("                    Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("                    UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("                    Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("                    ) ")
            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(Trim(Piva)) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Appezza) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Cod_Indirizzo) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Tipo_Indirizzo) & "  ")
            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append(")")
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

    Public Function Modifica(
                            ByVal Piva As String,
                            ByVal Sa_Cod As Int32,
                            ByVal Appezza As Int32,
                            ByVal Cod_Indirizzo As Integer,
                            ByVal Tipo_Indirizzo As Integer,
                                ByVal Validita_Inizio As Date,
                                ByVal Validita_Fine As Date,
                                ByVal xFiltroAggiuntivo As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.AppezzamentixIndirizzi_Write.Modifica()"

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
            StrSQL.Append("UPDATE AppezzamentixIndirizzi SET ")
            StrSQL.Append("       Tipo_Indirizzo   =  " & Agro_SQL_SaveNum(Tipo_Indirizzo) & "   ")
            StrSQL.Append("       ,Inviato              =  0 ")
            StrSQL.Append("       ,DataInvio            =  Null ")
            StrSQL.Append("       ,Data_Modifica        =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("       ,UserName_Modifica    = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("       ,Validita_Inizio      =  " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            StrSQL.Append("       ,Validita_Fine        =  " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.Append(" WHERE    PIVA        = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            StrSQL.Append(" AND      Sa_Cod      =  " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append(" AND      Appezza     =  " & Agro_SQL_SaveNum(Appezza) & "  ")
            StrSQL.Append(" AND      Cod_Indirizzo     =  " & Agro_SQL_SaveNum(Cod_Indirizzo) & "  ")


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

    Public Function Cancella(
                            ByVal Piva As String,
                            ByVal Sa_Cod As Int32,
                            ByVal Appezza As Int32,
                            ByVal Cod_Indirizzo As Integer,
                            ByVal xFiltroAggiuntivo As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.AppezzamentixIndirizzi_Write.Cancella()"

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
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE AppezzamentixIndirizzi ")
                StrSQL.Append(" SET ")
                StrSQL.Append("       Validita_Fine = " & Agro_SQL_SaveDate(CDate("31/12/1899")) & " ")
                StrSQL.Append("      ,Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Piva = <> '0' ")
                StrSQL.Append(" AND Inviato >= 0")

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     AppezzamentixIndirizzi ")
                StrSQL.Append(" WHERE    PIVA <> '0' ")

            End If

            If Piva <> "" Then
                StrSQL.Append(" AND   Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            End If
            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If
            If Appezza <> 0 Then
                StrSQL.Append(" AND   Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If
            If Cod_Indirizzo <> 0 Then
                StrSQL.Append(" AND   Cod_Indirizzo = " & Agro_SQL_SaveNum(Cod_Indirizzo) & " ")
            End If
            '---------------------------------------------



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