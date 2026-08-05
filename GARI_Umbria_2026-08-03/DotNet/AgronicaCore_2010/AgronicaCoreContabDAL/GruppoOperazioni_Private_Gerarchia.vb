Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class GruppoOperazioni_Private_Gerarchia_R
    Inherits AgronicaCoreDataProvider.DataProvider

    'Versione che utilizza AgronicaCoreParametri
    '##############################################################################################
    Public Function Leggi( _
                            ByVal Gru_Cod_Privato_Padre As Integer, _
                            ByVal Gru_Cod_Privato_Figlio As Integer, _
                                ByVal xSelectInfoMinime As Boolean, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.GruppoOperazioni_Private_Gerarchia_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Gru_Cod_Privato_Padre = 0           =>  tutti i record
        '   Gru_Cod_Privato_Figlio = 0          =>  tutti i record
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '//////////////////////////////////////////////////////////////////////
            '//////////////////////////////////////////////////////////////////////
            StrSQL.Length = 0

            If xSelectInfoMinime = True Then
                StrSQL.Append(" SELECT GruppoOperazioni_Private_Gerarchia.Gru_Cod_Privato_Padre,  ")
                StrSQL.Append("        GruppoOperazioni_Private_Gerarchia.Gru_Cod_Privato_Figlio,  ")
                StrSQL.Append("        GruppoOperazioni_Private.Gru_Des_Privato AS Gru_Des_Privato_Padre, ")
                StrSQL.Append("        GruppoOperazioni_Private_1.Gru_Des_Privato AS Gru_Des_Privato_Figlio ")
            Else
                StrSQL.Append(" SELECT * ")
            End If

            StrSQL.Append(" FROM  GruppoOperazioni_Private_Gerarchia INNER JOIN ")
            StrSQL.Append(" GruppoOperazioni_Private AS GruppoOperazioni_Private_1 ON  ")
            StrSQL.Append(" GruppoOperazioni_Private_Gerarchia.Piva_SuperUser = GruppoOperazioni_Private_1.Piva_SuperUser AND  ")
            StrSQL.Append(" GruppoOperazioni_Private_Gerarchia.Gru_Cod_Privato_Figlio = GruppoOperazioni_Private_1.Gru_Cod_Privato LEFT OUTER JOIN  ")
            StrSQL.Append(" GruppoOperazioni_Private ON GruppoOperazioni_Private_Gerarchia.Piva_SuperUser = GruppoOperazioni_Private.Piva_SuperUser AND  ")
            StrSQL.Append(" GruppoOperazioni_Private_Gerarchia.Gru_Cod_Privato_Padre = GruppoOperazioni_Private.Gru_Cod_Privato  ")

            StrSQL.Append(" WHERE   GruppoOperazioni_Private_Gerarchia.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND     GruppoOperazioni_Private_Gerarchia.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            StrSQL.Append(" AND     GruppoOperazioni_Private_Gerarchia.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Gru_Cod_Privato_Padre <> 0 Then
                StrSQL.Append(" AND GruppoOperazioni_Private_Gerarchia.Gru_Cod_Privato_Padre = " & Agro_SQL_SaveNum(Gru_Cod_Privato_Padre) & " ")
            End If
            If Gru_Cod_Privato_Figlio <> 0 Then
                StrSQL.Append(" AND GruppoOperazioni_Private_Gerarchia.Gru_Cod_Privato_Figlio = " & Agro_SQL_SaveNum(Gru_Cod_Privato_Figlio) & " ")
            End If


            '//////////////////////////////////////////////////////////////////////
            '//////////////////////////////////////////////////////////////////////


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   GruppoOperazioni_Private_Gerarchia.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   GruppoOperazioni_Private_Gerarchia.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY GruppoOperazioni_Private.Gru_Des_Privato, GruppoOperazioni_Private_1.Gru_Des_Privato ASC ")
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
