Imports System.Data.Common
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider
Imports System.Text

Public Class Sementieri_EstrazioneVariazioni_R
    Inherits AgronicaCoreDataProvider.DataProvider
    '##############################################################################################
    Public Function Leggi(
                                    ByVal Estrazione_Id As Integer,
                                    ByVal Sementieri_Sportello_Configurazione_cod As Integer,
                                    ByVal Regione As String,
                                    ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                   ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreSementieriDAL.Sementieri_EstrazioneVariazioni_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    Stb.Length = 0

                    Stb.Append("select SEV.Progressivo " & vbCrLf)
                    Stb.Append(", SEV.Ditta_sementiera " & vbCrLf)
                    Stb.Append(", SEV.Id_Appezzamento " & vbCrLf)
                    Stb.Append(", SEV.Specie_NC " & vbCrLf)
                    Stb.Append(", SEV.Specie_NS " & vbCrLf)
                    Stb.Append(", SEV.Tipologia " & vbCrLf)
                    Stb.Append(", SEV.Provincia " & vbCrLf)
                    Stb.Append(", SEV.Comune " & vbCrLf)
                    Stb.Append(", SEV.Azienda_Agricola " & vbCrLf)
                    Stb.Append(", SEV.Indirizzo_Appezzamento " & vbCrLf)
                    Stb.Append(", SEV.Latitudine " & vbCrLf)
                    Stb.Append(", SEV.Longitudine " & vbCrLf)
                    Stb.Append(", SEV.Superficie " & vbCrLf)
                    Stb.Append(", SEV.Motivo_Variazione " & vbCrLf)
                    Stb.Append(", SEV.Data_Variazione " & vbCrLf)
                    Stb.Append("FROM [dbo].[Sementieri_Estrazione_Variazioni] SEV " & vbCrLf)

                    Stb.Append(" WHERE 1=1 " & vbCrLf)
                    If Estrazione_Id <> 0 Then
                        Stb.Append(String.Format(" And SEV.Progressivo = {0} ", Agro_SQL_SaveNum(Estrazione_Id)) & vbCrLf)
                    End If

                    If Sementieri_Sportello_Configurazione_cod <> 0 Then
                        Stb.Append(String.Format(" And SEV.Sementieri_Sportello_Configurazione_cod = {0} ", Agro_SQL_SaveNum(Sementieri_Sportello_Configurazione_cod)) & vbCrLf)
                    End If

                    If Not Regione.Equals("") Then
                        Stb.Append(String.Format(" And UPPER(SEV.Regione) = '{0}' ", Agro_SQL_SaveText(Regione.ToUpper.Trim)) & vbCrLf)
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------

                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
                    '
                    '
                    '
                    '

            End Select

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
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
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################

Public Class Sementieri_EstrazioneVariazioni_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Scrivi(ByVal Sementieri_Sportello_Configurazione_cod As Integer,
                           ByVal Regione As String,
                            ByVal Ditta_Sementiera As String,
                            ByVal Id_Appezzamento As String,
                            ByVal Specie_NC As String,
                            ByVal Specie_NS As String,
                            ByVal Tipologia As String,
                            ByVal Provincia As String,
                            ByVal Comune As String,
                            ByVal Azienda_Agricola As String,
                            ByVal Indirizzo_Appezzamento As String,
                            ByVal Latitudine As Decimal,
                            ByVal Longitudine As Decimal,
                            ByVal Superficie As Decimal,
                            ByVal Motivo_Variazione As String,
                            ByVal Data_Variazione As DateTime,
                            ByVal Inviato As Int32,
                            ByVal DataInvio As DateTime,
                            ByRef objParametri As AgronicaCoreParametri) As Boolean

        Const nomeRoutine = "AgronicaCoreSementieriDAL.Sementieri_EstrazioneVariazioni_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            strSql.Length = 0
            strSql.AppendLine(" INSERT INTO Sementieri_Estrazione_Variazioni ")
            strSql.AppendLine("( ")
            strSql.AppendLine(" [Sementieri_Sportello_Configurazione_cod] ")
            strSql.AppendLine(" ,[Regione] ")
            strSql.AppendLine(" ,[Ditta_Sementiera] ")
            strSql.AppendLine(" ,[Id_Appezzamento] ")
            strSql.AppendLine(" ,[Specie_NC] ")
            strSql.AppendLine(" ,[Specie_NS] ")
            strSql.AppendLine(" ,[Tipologia] ")
            strSql.AppendLine(" ,[Provincia] ")
            strSql.AppendLine(" ,[Comune] ")
            strSql.AppendLine(" ,[Azienda_Agricola] ")
            strSql.AppendLine(" ,[Indirizzo_Appezzamento] ")
            strSql.AppendLine(" ,[Latitudine] ")
            strSql.AppendLine(" ,[Longitudine] ")
            strSql.AppendLine(" ,[Superficie] ")
            strSql.AppendLine(" ,[Motivo_Variazione] ")
            strSql.AppendLine(" ,[Data_Variazione] ")
            strSql.AppendLine(" ,[inviato] ")
            strSql.AppendLine(" ,[datainvio] ")
            strSql.AppendLine(" ,[Data_Creazione] ")
            strSql.AppendLine(" ,[Data_Modifica] ")
            strSql.AppendLine(" ,[Username_Creazione] ")
            strSql.AppendLine(" ,[Username_Modifica] ")
            strSql.AppendLine(" ,[Validita_Inizio] ")
            strSql.AppendLine(" ,[Validita_Fine] ")
            strSql.AppendLine(" ) ")
            strSql.AppendLine(" VALUES ")
            strSql.AppendLine("( ")
            strSql.AppendLine(" " & Agro_SQL_SaveNum(Sementieri_Sportello_Configurazione_cod) & " ")
            strSql.AppendLine(", '" & Agro_SQL_SaveText(Regione) & "' ")
            strSql.AppendLine(", '" & Agro_SQL_SaveText(Ditta_Sementiera) & "' ")
            strSql.AppendLine(", '" & Agro_SQL_SaveText(Id_Appezzamento) & "' ")
            strSql.AppendLine(", '" & Agro_SQL_SaveText(Specie_NC) & "' ")
            strSql.AppendLine(", '" & Agro_SQL_SaveText(Specie_NS) & "' ")
            strSql.AppendLine(", '" & Agro_SQL_SaveText(Tipologia) & "' ")
            strSql.AppendLine(", '" & Agro_SQL_SaveText(Provincia) & "' ")
            strSql.AppendLine(", '" & Agro_SQL_SaveText(Comune) & "' ")
            strSql.AppendLine(", '" & Agro_SQL_SaveText(Azienda_Agricola) & "' ")
            strSql.AppendLine(", '" & Agro_SQL_SaveText(Indirizzo_Appezzamento) & "' ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Latitudine) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Longitudine) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Superficie) & " ")
            strSql.AppendLine(", '" & Agro_SQL_SaveText(Motivo_Variazione) & "' ")
            strSql.AppendLine(", " & Agro_SQL_SaveDate(Data_Variazione) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Inviato) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveDate(DataInvio) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveDate(DateTime.Now) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveDate(DateTime.Now) & " ")
            strSql.AppendLine(", '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            strSql.AppendLine(", '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            strSql.AppendLine(", " & Agro_SQL_SaveDate(CostantiPersonalizzate.AGRODATAINIZIO) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveDate(CostantiPersonalizzate.AGRODATAFINE) & " ")
            strSql.AppendLine(" ) ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp
    End Function

    Public Function Elimina(ByVal Sementieri_Sportello_Configurazione_cod As Integer,
                            ByVal Progressivo As Integer,
                            ByRef objParametri As AgronicaCoreParametri) As Boolean

        Const nomeRoutine = "AgronicaCoreSementieriDAL.Sementieri_EstrazioneVariazioni_W.Elimina()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            strSql.Length = 0
            strSql.AppendLine(" DELETE from Sementieri_Estrazione_Variazioni ")
            strSql.AppendLine(" WHERE 1=1 ")

            If Sementieri_Sportello_Configurazione_cod > 0 Then
                strSql.AppendLine(" AND Sementieri_Sportello_Configurazione_cod = " & Agro_SQL_SaveNum(Sementieri_Sportello_Configurazione_cod) & " ")
            End If

            If Progressivo > 0 Then
                strSql.AppendLine(" AND Progressivo = " & Agro_SQL_SaveNum(Progressivo) & " ")
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp
    End Function
End Class
