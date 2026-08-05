
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Riclassificazioni_W
    Inherits AgronicaCoreDataProvider.DataProvider




    '##############################################################################################
    Public Function Scrivi_New( _
                                ByVal Piva As String, _
                                ByVal Ric_Cod As Int32, _
                                ByVal Ric_Des As String, _
                                    ByVal Validita_Inizio As Date, _
                                    ByVal Validita_Fine As Date, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Riclassificazioni_W.Scrivi_New()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = True

        Try

            '---------------------------------------------

            StrSQL.Length = 0

            StrSQL.Append(" INSERT INTO Riclassificazioni ")
            StrSQL.Append("                    (Piva, Ric_Cod,  Ric_Des, ")
            StrSQL.Append("                    Inviato,            DataInvio, ")
            StrSQL.Append("                    Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("                    UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("                    Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("                    ) ")

            StrSQL.Append(" VALUES (")


            StrSQL.Append("          '" & Agro_SQL_SaveText(Piva) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Ric_Cod) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Ric_Des) & "'  ")

            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
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




    '##############################################################################################
    Public Function Modifica_New(ByVal Piva As String, _
                    ByVal Ric_Cod As Long, _
                    ByVal Ric_Des As String, _
                                ByVal Validita_Inizio As Date, _
                                ByVal Validita_Fine As Date, _
                                    ByVal xFiltroAggiuntivo As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Riclassificazioni_W.Modifica_New()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = True

        '------------------------------

        Try


            If objParametri.PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (Ricetta_SuperUser obbligatorio)")
            End If

            '---------------------------------------------

            StrSQL.Length = 0


            StrSQL.Append(" UPDATE Riclassificazioni SET ")


            StrSQL.Append("         Ric_Des           = '" & Agro_SQL_SaveText(Ric_Des) & "'  ")


            StrSQL.Append("   ,Inviato           = 0 ")
            StrSQL.Append("   ,DataInvio         = Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'  ")
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append(" WHERE  1=1 ")

            If (Ric_Cod <> 0) Then
                StrSQL.Append(" AND Ric_Cod   =  " & Agro_SQL_SaveNum(Ric_Cod) & "    ")
            End If
            If (Piva <> "") Then
                StrSQL.Append(" AND Piva   =  '" & Agro_SQL_SaveText(Piva) & "'")

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
