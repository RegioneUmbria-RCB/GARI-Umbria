Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Visite_Categoria2_R
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function Leggi(ByRef objParametri As AgronicaCoreParametri) As DataTable
        Return Leggi(0, 0, "", objParametri)
    End Function

    Public Function Leggi(ByVal ID_Categoria2 As Integer,
                          ByVal ID_Categoria1 As Integer,
                          ByVal Nome As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreVisite_DAL.Visite_Categoria2_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT *  ")
            StrSQL.AppendLine(" FROM  Visite_Categoria2 ")
            StrSQL.AppendLine(" WHERE 1=1 ")

            If ID_Categoria2 <> 0 Then
                StrSQL.AppendLine(" AND ID_Categoria2 = " & Agro_SQL_SaveNum_NULL(ID_Categoria2) & " ")
            End If

            If ID_Categoria1 <> 0 Then
                StrSQL.AppendLine(" AND ID_Categoria1 = " & Agro_SQL_SaveNum_NULL(ID_Categoria1) & " ")
            End If

            If Nome <> "" Then
                StrSQL.AppendLine(" AND Nome = " & Agro_SQL_SaveText_NULL(Nome) & " ")
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


'##############################################################################################


Public Class Visite_Categoria2_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal ID_Categoria1 As Integer,
                           ByVal ID_Categoria2 As Integer,
                           ByVal Nome As String,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                           ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreVisite_DAL.Visite_Categoria2_W.Scrivi()"

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
            StrSQL.AppendLine("INSERT INTO Visite_Categoria2 ")
            StrSQL.AppendLine(" (ID_Categoria2, ID_Categoria1, Nome ")

            StrSQL.AppendLine(", username_creazione, data_creazione, username_modifica, data_modifica) ")

            StrSQL.AppendLine("VALUES (")

            StrSQL.AppendLine("           " & Agro_vb_SaveNum(ID_Categoria2.ToString()) & " ")
            StrSQL.AppendLine("         , " & Agro_vb_SaveNum(ID_Categoria1.ToString()) & " ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Nome) & "' ")

            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(DateTime.Now) & " ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(DateTime.Now) & " ")

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


    Public Function Modifica(ByVal ID_Categoria2 As Integer,
                            ByVal Nome As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreVisite_DAL.Visite_Categoria2_W.Modifica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" UPDATE Visite_Categoria2 SET ")

            StrSQL.AppendLine("    Data_Modifica     =  " & Agro_SQL_SaveDateTime(DateTime.Now))
            StrSQL.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.AppendLine("   ,Nome           = '" & Agro_SQL_SaveText(Nome) & "' ")

            StrSQL.AppendLine(" WHERE   ID_Categoria2        =" & Agro_SQL_SaveNum(ID_Categoria2.ToString()) & " ")

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





    Public Function Cancella(ByVal ID_Categoria2 As Integer,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreVisite_DAL.Visite_Categoria2_W.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.AppendLine(" UPDATE Visite_Categoria2 ")
                StrSQL.AppendLine(" SET ")
                StrSQL.AppendLine("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.AppendLine("      ,Inviato = -1 ")
                StrSQL.AppendLine(" WHERE  Inviato >= 0 ")

            Else
                StrSQL.Length = 0
                StrSQL.AppendLine(" DELETE ")
                StrSQL.AppendLine(" FROM Visite_Categoria2 ")
                StrSQL.AppendLine(" WHERE  1=1 ")

            End If

            StrSQL.Append(" AND ID_Categoria2 = " & Agro_SQL_SaveNum(ID_Categoria2.ToString()) & " ")

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
