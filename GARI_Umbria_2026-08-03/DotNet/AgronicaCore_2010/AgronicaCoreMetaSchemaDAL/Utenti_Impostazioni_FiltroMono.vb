Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider







'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

Public Class Utenti_Impostazioni_FiltroMono_R
    Inherits AgronicaCoreDataProvider.DataProvider




    '##############################################################################################
    Public Function Leggi( _
                        ByVal Impostazione_Cod As Integer, _
                        ByVal ID_0 As Integer, _
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Utenti_Impostazioni_FiltroMono_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Veg_Cod = 0           =>  tutti i record
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    '//////////////////////////////////////////////////////////////////////
                    '//////////////////////////////////////////////////////////////////////


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta
                    '//////////////////////////////////////////////////////////////////////
                    '//////////////////////////////////////////////////////////////////////

                    '//////////////////////////////////////////////////////////////////////
                    '//////////////////////////////////////////////////////////////////////



                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni
                    '
                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT      Utenti_Impostazioni_FiltroMono.Piva_SuperUser, Utenti_Impostazioni_FiltroMono.UserName, Utenti_Impostazioni_FiltroMono.Impostazione_Cod,   ")
                    StrSQL.Append("             Utenti_Impostazioni_FiltroMono.ID_0, Utenti_Impostazioni.Impostazione_Valore_1, Utenti_Impostazioni.Impostazione_Valore_2, ")
                    StrSQL.Append("             Utenti_Impostazioni.Impostazione_Valore_3, Utenti_Impostazioni.Impostazione_Valore_4 ")
                    StrSQL.Append(" FROM        Utenti_Impostazioni_FiltroMono ")
                    StrSQL.Append(" INNER JOIN  Utenti_Impostazioni ON ")
                    StrSQL.Append("             Utenti_Impostazioni.Piva_SuperUser = Utenti_Impostazioni_FiltroMono.Piva_SuperUser ")
                    StrSQL.Append("             AND  Utenti_Impostazioni.UserName = Utenti_Impostazioni_FiltroMono.UserName ")
                    StrSQL.Append("             AND Utenti_Impostazioni.Impostazione_Cod = Utenti_Impostazioni_FiltroMono.Impostazione_Cod ")
                    StrSQL.Append(" WHERE     1=1  ")
                    If objParametri.PivaSuperUser <> "" Then
                        StrSQL.Append(" AND     (Utenti_Impostazioni_FiltroMono.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "')   ")
                    End If

                    If objParametri.UsernameOperazione <> "" Then
                        StrSQL.Append(" AND     (Utenti_Impostazioni_FiltroMono.Username = '" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "')   ")

                    End If

                    If Impostazione_Cod <> 0 Then
                        StrSQL.Append(" AND     (Utenti_Impostazioni_FiltroMono.Impostazione_Cod = " & Agro_SQL_SaveNum(Impostazione_Cod) & ")   ")
                    End If

                    If ID_0 <> 0 Then

                        StrSQL.Append(" AND     (Utenti_Impostazioni_FiltroMono.ID_0 = " & Agro_SQL_SaveNum(ID_0) & ")   ")

                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Utenti_Impostazioni_FiltroMono.Piva_SuperUser, Utenti_Impostazioni_FiltroMono.Username, Utenti_Impostazioni_FiltroMono.Impostazione_Cod ")

                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
                    '
                    '
                    '
                    '


            End Select



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


'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
