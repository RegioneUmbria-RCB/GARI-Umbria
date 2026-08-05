Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class UtentiGias_Write
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi( _
                            ByVal UserName As String, _
                            ByVal Nome_Resp As String, _
                            ByVal Ente_Resp As Integer, _
                            ByVal Inizio_Attivita As Date, _
                            ByVal Attivita As Integer, _
                            ByVal Calcolatore As Integer, _
                            ByVal Password As String, _
                            ByVal CriptaPassword As Boolean, _
                            ByVal Tipo As Integer, _
                            ByVal Codice_Fiscale As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.UtentiGias_Write.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Dim Encrypted_Pwd As String
        Dim Flag_Encrypted As Long

        'If CriptaPassword = True Then
        '    Encrypted_Pwd = AGRO_Encrypt(Password, LCase(UserName), True)
        '    Flag_Encrypted = 1
        'Else
        '    Encrypted_Pwd = Password
        '    Flag_Encrypted = 0
        'End If

        Encrypted_Pwd = Password
        Flag_Encrypted = 0

        Try

            '---------------------------------------------
            StrSQL.Length = 0


            StrSQL.Append("INSERT INTO Utenti ([User],Nome_Resp,Ente_Resp,Inizio_Attivita, ")
            StrSQL.Append("                    Attivita,Calcolatore,Password,Tipo,Codice_Fiscale,Inviato,DataInvio,Data_Creazione,Data_Modifica, ")
            StrSQL.Append("                    Username_Creazione,Username_Modifica,FinestraTemp_Inizio,FinestraTemp_Fine, ")
            StrSQL.Append("                    Validita_Inizio,Validita_Fine ")
            StrSQL.Append("                    ) ")
            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(LCase(UserName)) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Nome_Resp) & "' ")
            StrSQL.Append("          ," & Ente_Resp)
            StrSQL.Append("          ," & Agro_SQL_SaveDate(Inizio_Attivita) & "  ")
            StrSQL.Append("          ," & Attivita)
            StrSQL.Append("          ," & Calcolatore)
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Encrypted_Pwd) & "' ")
            StrSQL.Append("          ," & Tipo)
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Codice_Fiscale) & "' ")
            StrSQL.Append("         , 0 ,NULL ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
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

    Public Function Scrivi_2(
            ByVal UserName As String,
            ByVal Password As String,
            ByVal Lingua_Cod As Integer,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
            Optional ByVal Flag As String = "0"
        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.UtentiGias_Write.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Dim Encrypted_Pwd As String
        Dim Flag_Encrypted As Long

        'If CriptaPassword = True Then
        '    Encrypted_Pwd = AGRO_Encrypt(Password, LCase(UserName), True)
        '    Flag_Encrypted = 1
        'Else
        '    Encrypted_Pwd = Password
        '    Flag_Encrypted = 0
        'End If

        Encrypted_Pwd = Password
        Flag_Encrypted = 0

        Try

            '---------------------------------------------
            StrSQL.Length = 0


            StrSQL.Append("INSERT INTO Utenti (UserName, Password,Flag_Encrypted, Flag, Data_Creazione,Data_Modifica, Lingua_Cod )")

            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(LCase(UserName)) & "' ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(LCase(Password)) & "' ")
            StrSQL.Append("         , 0 ")
            StrSQL.Append("         ,  " & Flag)

            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Lingua_Cod) & "  ")
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

End Class
