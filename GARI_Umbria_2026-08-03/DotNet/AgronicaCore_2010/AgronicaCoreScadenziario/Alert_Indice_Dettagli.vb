Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider

'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################


Public Class Alert_Indice_Dettagli_R
    Inherits AgronicaCoreDataProvider.DataProvider
    Public Function Leggi(ByVal Piva As String,
                          ByVal Id_Indice As Integer,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Indice_Dettagli_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim PivaSuperUser = objParametri.PivaSuperUser

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT *  ")
            StrSQL.Append(" FROM  Alert_Indice_Dettagli ")
            StrSQL.Append(" WHERE 1= 1 ")

            If PivaSuperUser <> "" Then
                StrSQL.Append(" AND PivaSuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
            End If

            If Piva <> "" Then
                If DataProviderFactory.Instance.TipoProvider = enum_DataProvidersType.OleDbProvider Then
                    StrSQL.Append(" AND Piva  In ('" & Agro_SQL_Save_Clausola_IN(Piva, True) & "', '') ")
                Else
                    If DataProviderFactory.Instance.ParametrizzaQuery Then
                        StrSQL.Append(" AND Piva  In (" & Agro_SQL_Save_Clausola_IN(Piva, True) & ", '') ")
                    Else
                        StrSQL.Append(" AND Piva  In ('" & Agro_SQL_Save_Clausola_IN(Piva, True) & "', '') ")
                    End If

                End If
            End If

            If Id_Indice <> 0 Then
                StrSQL.Append(" AND Id_Indice = " & Agro_SQL_SaveNum(Id_Indice) & " ")
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

Public Class Alert_Indice_Dettagli_W

    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal piva As String,
                           ByVal id_indice As Integer,
                           ByVal id_indice_det As Integer,
                           ByVal valore As String,
                           ByVal validita_inizio As Date,
                           ByVal validita_fine As Date,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                           ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Indice_Dettagli_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False
        Dim PivaSuperUser = objParametri.PivaSuperUser

        Try

            piva = "" 'Non gestito

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO Alert_Indice_Dettagli ")
            StrSQL.Append("                   ( PivaSuperUser, Piva, Id_Indice, Id_Indice_Det, Valore, Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica, Inviato, DataInvio, Validita_Inizio, Validita_Fine ")

            StrSQL.Append(" ) ")

            StrSQL.Append("VALUES (")

            StrSQL.Append("          '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(piva) & "' ")
            StrSQL.Append("         ," & Agro_vb_SaveNum(id_indice) & " ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(id_indice_det) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(valore) & "' ")
            StrSQL.Append("		    , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("		    , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("		    ,'" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "' ")
            StrSQL.Append("		    ,'" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "' ")
            StrSQL.Append("         , 0 ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("         ," & Agro_SQL_SaveDate(validita_inizio) & " ")
            StrSQL.Append("         ," & Agro_SQL_SaveDate(validita_fine) & " ")


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

    Public Function Modifica(ByVal piva As String,
                             ByVal id_indice As Integer,
                             ByVal id_indice_det As Integer,
                             ByVal valore As String,
                             ByVal validita_inizio As Date,
                             ByVal validita_fine As Date,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Indice_Dettagli_W.Modifica()"



        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False
        Dim PivaSuperUser = objParametri.PivaSuperUser


        '------------------------------

        Try

            StrSQL.Length = 0

            StrSQL.Append(" UPDATE Alert_Indice_Dettagli SET ")
            StrSQL.Append(" valore = '" & Agro_SQL_SaveText(valore) & "' ")
            StrSQL.Append(" ,Data_Modifica = " & Agro_SQL_SaveDate(Date.Now) & " ")
            StrSQL.Append(" ,Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "' ")
            StrSQL.Append(" ,validita_inizio = " & Agro_SQL_SaveDate(validita_inizio) & " ")
            StrSQL.Append(" ,validita_fine = " & Agro_SQL_SaveDate(validita_fine) & " ")

            StrSQL.Append(" WHERE id_indice = " & Agro_SQL_SaveNum(id_indice) & " ")
            StrSQL.Append(" And id_indice_det = " & Agro_SQL_SaveNum(id_indice_det) & " ")

            If PivaSuperUser <> "" Then
                StrSQL.Append(" AND PivaSuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
            End If

            If piva <> "" Then
                StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(piva) & "' ")
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





    Public Function Cancella(ByVal piva As String,
                             ByVal id_indice As Integer,
                             ByVal id_indice_det As Integer,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Indice_W.Cancella()"



        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False
        Dim PivaSuperUser = objParametri.PivaSuperUser

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM Alert_Indice_Dettagli ")
                StrSQL.Append(" WHERE id_indice = " & Agro_SQL_SaveNum(id_indice) & " ")

                If PivaSuperUser <> "" Then
                    StrSQL.Append(" AND PivaSuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
                End If

                If piva <> "" Then
                    StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(piva) & "' ")
                End If

                If id_indice_det <> 0 Then
                    StrSQL.Append(" And id_indice_det = " & Agro_SQL_SaveNum(id_indice_det) & " ")
                End If

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
