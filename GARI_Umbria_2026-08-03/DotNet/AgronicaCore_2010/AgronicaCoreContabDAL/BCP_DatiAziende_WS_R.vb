Imports System.Data.Common
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class BCP_DatiAziende_WS_R_OLD
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Leggi(
                            ByVal SessionID As String,
                            ByVal Azienda As String,
                            ByVal Anno As String,
                            ByVal Specie As String,
                            ByVal Operazione As String,
                            ByVal ElemLiv1 As String,
                            ByVal ElemLiv2 As String,
                            ByVal ElemLiv3 As String,
                            ByVal ElemLiv4 As String,
                            ByVal FinestraTemp_Inizio As Date,
                            ByVal FinestraTemp_Fine As Date,
                            ByRef objConnessione As DbConnection,
                            ByVal StringaConnessione As String,
                            ByVal FlagVisibilita As Int32,
                            ByVal DirectoryLOG As String,
                            ByVal FileLOG As String,
                            ByVal IdentificatoreUtente As String
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.BCP_DatiAziende_WS_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :     
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" SELECT *  ")
            StrSQL.Append(" FROM _BCP_DatiAziende_WS ")

            StrSQL.Append(" WHERE 1=1 ")

            StrSQL.Append(" AND   Session_ID = '" & Agro_SQL_SaveText(SessionID) & "' ")
            StrSQL.Append(" AND   ID_Azienda = '" & Agro_SQL_SaveText(Azienda) & "' ")
            StrSQL.Append(" AND   ANNO_AGRARIO = '" & Agro_SQL_SaveText(Anno) & "' ")

            If Specie <> "" Then
                StrSQL.Append(" AND   Specie = '" & Agro_SQL_SaveText(Specie) & "' ")
            End If

            If Operazione <> "" Then
                StrSQL.Append(" AND   Operazione = '" & Agro_SQL_SaveText(Operazione) & "' ")
            End If

            If ElemLiv1 <> "" Then
                StrSQL.Append(" AND   ELEM_LIV1 = '" & Agro_SQL_SaveText(ElemLiv1) & "' ")
            End If

            If ElemLiv2 <> "" Then
                StrSQL.Append(" AND   ELEM_LIV2 = '" & Agro_SQL_SaveText(ElemLiv2) & "' ")
            End If

            If ElemLiv3 <> "" Then
                StrSQL.Append(" AND   ELEM_LIV3 = '" & Agro_SQL_SaveText(ElemLiv3) & "' ")
            End If

            If ElemLiv4 <> "" Then
                StrSQL.Append(" AND   ELEM_LIV4 = '" & Agro_SQL_SaveText(ElemLiv4) & "' ")
            End If

            StrSQL.Append(" ORDER BY Specie, OPERAZIONE, ELEM_LIV1, ELEM_LIV2, ELEM_LIV3, ELEM_LIV4 ")

            '---------------------------------------------

            DT = EseguiQuery_Lettura(objConnessione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function


    '##############################################################################################
    Public Function Leggi_SE_Specie(
                                    ByVal SessionID As String,
                                    ByVal Azienda As String,
                                    ByVal Anno As String,
                                    ByVal FinestraTemp_Inizio As Date,
                                    ByVal FinestraTemp_Fine As Date,
                                    ByRef objConnessione As DbConnection,
                                    ByVal StringaConnessione As String,
                                    ByVal FlagVisibilita As Int32,
                                    ByVal DirectoryLOG As String,
                                    ByVal FileLOG As String,
                                    ByVal IdentificatoreUtente As String
                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.BCP_DatiAziende_WS_R.Leggi_SE_Macrovoci()"

        '====================================================================================
        'Parametri opzionali :     
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" SELECT DISTINCT PIVA, Specie, ANNO_AGRARIO ")
            StrSQL.Append(" FROM _BCP_DatiAziende_WS ")

            StrSQL.Append(" WHERE 1=1 ")

            StrSQL.Append(" AND   Session_ID = '" & Agro_SQL_SaveText(SessionID) & "' ")
            StrSQL.Append(" AND   ID_Azienda = '" & Agro_SQL_SaveText(Azienda) & "' ")
            StrSQL.Append(" AND   ANNO_AGRARIO = '" & Agro_SQL_SaveText(Anno) & "' ")


            '---------------------------------------------

            DT = EseguiQuery_Lettura(objConnessione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function



    '##############################################################################################
    Public Function Leggi_SE_Operazioni(
                                    ByVal SessionID As String,
                                    ByVal Azienda As String,
                                    ByVal Anno As String,
                                    ByVal Specie As String,
                                    ByVal FinestraTemp_Inizio As Date,
                                    ByVal FinestraTemp_Fine As Date,
                                    ByRef objConnessione As DbConnection,
                                    ByVal StringaConnessione As String,
                                    ByVal FlagVisibilita As Int32,
                                    ByVal DirectoryLOG As String,
                                    ByVal FileLOG As String,
                                    ByVal IdentificatoreUtente As String
                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.BCP_DatiAziende_WS_R.Leggi_SE_Operazioni()"

        '====================================================================================
        'Parametri opzionali :     
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" SELECT DISTINCT PIVA, Specie, ANNO_AGRARIO, Operazione ")
            StrSQL.Append(" FROM _BCP_DatiAziende_WS ")

            StrSQL.Append(" WHERE 1=1 ")

            StrSQL.Append(" AND   Session_ID = '" & Agro_SQL_SaveText(SessionID) & "' ")
            StrSQL.Append(" AND   ID_Azienda = '" & Agro_SQL_SaveText(Azienda) & "' ")
            StrSQL.Append(" AND   ANNO_AGRARIO = '" & Agro_SQL_SaveText(Anno) & "' ")
            StrSQL.Append(" AND   Specie = '" & Agro_SQL_SaveText(Specie) & "' ")

            '---------------------------------------------

            DT = EseguiQuery_Lettura(objConnessione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function



End Class
