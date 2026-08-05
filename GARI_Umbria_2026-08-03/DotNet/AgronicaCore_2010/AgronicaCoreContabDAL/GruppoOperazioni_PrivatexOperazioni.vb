Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class GruppoOperazioni_PrivatexOperazioni_R
    Inherits AgronicaCoreDataProvider.DataProvider

    'Versione che utilizza AgronicaCoreParametri
    '##############################################################################################
    Public Function Leggi( _
                            ByVal Gru_Cod_Privato As Integer, _
                            ByVal Lav_Cod As Integer, _
                                ByVal xSelectInfoMinime As Boolean, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.GruppoOperazioni_Private_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Gru_Cod_Privato = 0           =>  tutti i record
        '   Lav_Cod = 0                   =>  tutti i record
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '//////////////////////////////////////////////////////////////////////
            '//////////////////////////////////////////////////////////////////////
            StrSQL.Length = 0

            If xSelectInfoMinime = True Then
                StrSQL.Append(" SELECT Gru_Cod_Privato, Lav_Cod ")
            Else
                StrSQL.Append(" SELECT * ")
            End If

            StrSQL.Append(" FROM  GruppoOperazioni_PrivatexOperazioni ")

            StrSQL.Append(" WHERE   GruppoOperazioni_PrivatexOperazioni.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND     GruppoOperazioni_PrivatexOperazioni.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            StrSQL.Append(" AND     GruppoOperazioni_PrivatexOperazioni.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Gru_Cod_Privato <> 0 Then
                StrSQL.Append(" AND GruppoOperazioni_PrivatexOperazioni.Gru_Cod_Privato = " & Agro_SQL_SaveNum(Gru_Cod_Privato) & " ")
            End If

            If Lav_Cod <> 0 Then
                StrSQL.Append(" AND GruppoOperazioni_PrivatexOperazioni.Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & " ")
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
                    StrSQL.Append(" AND   GruppoOperazioni_PrivatexOperazioni.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   GruppoOperazioni_PrivatexOperazioni.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Gru_Cod_Privato, Lav_Cod ")
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
