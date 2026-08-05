Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.DataProviderExtensions

Public Class Imputazioni_R
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function Leggi(ByVal piva As String,
                          ByVal Imputazione_Cod As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine As String = "AgronicaCoreContabDAL.Imputazioni_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT i.*, ic.*, it.*  ")

            StrSQL.Append(" From imputazioni i  ")
            StrSQL.Append(" Join Imputazioni_Tipi it on i.Piva_SuperUser =  it.Piva_SuperUser And i.Piva = it.Piva And i.Tipo_Imputazione = it.Tipo_Imputazione ")
            StrSQL.Append(" Join Imputazioni_Classi ic on i.Piva_SuperUser =  ic.Piva_SuperUser And i.Piva = ic.Piva And i.Imputazione_Classe_Cod = ic.Imputazione_Classe_Cod  ")
            StrSQL.Append(" WHERE i.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
            StrSQL.Append(" AND   i.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
            StrSQL.Append(" AND   i.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Trim(piva) <> "" Then
                StrSQL.Append(" AND   i.Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            If Imputazione_Cod <> 0 Then
                StrSQL.Append(" AND   i.Imputazione_Cod = " & Imputazione_Cod)
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" And " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" And   i.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" And   i.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query ")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] :  " & messaggioErrore)
        End Try

        Return dt

    End Function


    Public Function LeggiXGrigliaProgetti_AnagraficheCdG(ByVal piva As String,
                                                         ByVal Imputazione_Cod As Integer,
                                                         ByVal xFiltroAggiuntivo As String,
                                                         ByVal xOrderBy As String,
                                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                         ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Imputazioni_R.LeggiXGrigliaProgetti_AnagraficheCdG()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT i.*, ic.Imputazione_Classe_Des, ")
            StrSQL.AppendLine(" it.Tipo_Imputazione_Des, i.Settore, ")
            StrSQL.AppendLine(" CASE WHEN i.ChkImputazione = 1 THEN 'Sì' WHEN i.ChkImputazione = 0 THEN 'No' END AS ChkImputazione_Des,")
            StrSQL.AppendLine(" CASE WHEN i.ChkUtilizzaInReport = 1 THEN 'Sì' WHEN i.ChkUtilizzaInReport = 0 THEN 'No' END AS ChkUtilizzaInReport_Des,")
            StrSQL.AppendLine(" CASE ")
            StrSQL.AppendLine("     WHEN i.Analisi = '|0|' THEN 'Costi e Ricavi' ")
            StrSQL.AppendLine("     WHEN i.Analisi = '|1|' THEN 'Costi' ")
            StrSQL.AppendLine("     WHEN i.Analisi = '|2|' THEN 'Ricavi' ")
            StrSQL.AppendLine(" END AS Analisi_Des,")
            StrSQL.AppendLine(" i.Imputazione_Interna as Progetto_Interno,")
            StrSQL.AppendLine(" CASE WHEN i.Imputazione_Interna = 1 THEN 'Sì' WHEN i.Imputazione_Interna = 0 THEN 'No' END AS Progetto_Interno_Des")

            StrSQL.AppendLine(" FROM imputazioni i  ")
            StrSQL.AppendLine(" JOIN Imputazioni_Tipi it on i.Piva_SuperUser =  it.Piva_SuperUser AND i.Piva = it.Piva AND i.Tipo_Imputazione = it.Tipo_Imputazione ")
            StrSQL.AppendLine(" JOIN Imputazioni_Classi ic on i.Piva_SuperUser =  ic.Piva_SuperUser AND i.Piva = ic.Piva AND i.Imputazione_Classe_Cod = ic.Imputazione_Classe_Cod  ")
            StrSQL.AppendLine(" WHERE i.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
            StrSQL.AppendLine(" AND   i.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
            StrSQL.AppendLine(" AND   i.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine(" AND   i.Piva = '" & Agro_SQL_SaveText(piva) & "' ")

            If Imputazione_Cod <> 0 Then
                StrSQL.AppendLine(" AND   i.Imputazione_Cod = " & Imputazione_Cod)
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   i.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   i.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query ")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] :  " & messaggioErrore)
        End Try


        Return dt

    End Function
End Class


Public Class Imputazioni_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal Piva As String,
                           ByVal Imputazione_Cod As Integer,
                           ByVal Imputazione_Classe_Cod As Integer,
                           ByVal Imputazione_Nome As String,
                           ByVal Imputazione_Cod_Des As String,
                           ByVal Imputazione_Des As String,
                           ByVal Tipo_Imputazione As Integer,
                           ByVal Settore As String,
                           ByVal Modulo As String,
                           ByVal Superficie As Decimal,
                           ByVal Superficie_Consuntivo As Decimal,
                           ByVal Imputazione_Preset_Cod As Integer,
                           ByVal Analisi As String,
                           ByVal ChkImputazione As Integer,
                           ByVal ChkUtilizzaInReport As Integer,
                           ByVal ChkRisorse_Umane As Integer,
                           ByVal ChkMacchine As Integer,
                           ByVal ChkProdotti As Integer,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           Optional Imputazione_Interna As Integer = 0) As Boolean

        Const nomeRoutine As String = "AgronicaCoreContabDAL.Imputazioni_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '------------------------------

            StrSQL.Length = 0
            StrSQL.AppendLine("INSERT INTO Imputazioni( ")
            StrSQL.AppendLine("                     Piva_SuperUser,           Piva,                        ")
            StrSQL.AppendLine("                     Imputazione_Cod,          Imputazione_Classe_Cod,        ")
            StrSQL.AppendLine("                     Imputazione_Nome,         Imputazione_Cod_Des,       Imputazione_Des,     ")
            StrSQL.AppendLine("                     Tipo_Imputazione,         Settore,                   Modulo,                  Superficie,   ")
            StrSQL.AppendLine("                     Analisi,                  ChkImputazione,            ChkUtilizzaInReport,     ChkRisorse_Umane,   ChkMacchine, ")
            StrSQL.AppendLine("                     ChkProdotti,              Superficie_Consuntivo,     Imputazione_Preset_Cod,  Imputazione_Interna, ")
            StrSQL.AppendLine("                     Inviato,                  DataInvio,  ")
            StrSQL.AppendLine("                     Data_Creazione,           Data_Modifica, ")
            StrSQL.AppendLine("                     UserName_Creazione,       UserName_Modifica, ")
            StrSQL.AppendLine("                     Validita_Inizio,          Validita_Fine  ")
            StrSQL.AppendLine("                     ) ")


            StrSQL.AppendLine("VALUES ( ")
            StrSQL.AppendLine("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Imputazione_Cod) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Imputazione_Classe_Cod) & " ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Imputazione_Nome) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Imputazione_Cod_Des) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Imputazione_Des) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Tipo_Imputazione) & " ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Settore) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Modulo) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Superficie) & " ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Analisi) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(ChkImputazione) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(ChkUtilizzaInReport) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(ChkRisorse_Umane) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(ChkMacchine) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(ChkProdotti) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Superficie_Consuntivo) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Imputazione_Preset_Cod) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Imputazione_Interna) & " ")

            StrSQL.AppendLine("         , 0  ")
            StrSQL.AppendLine("         , Null  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(Now) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(Now) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.AppendLine(" )")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function


    Public Function Modifica(ByVal Piva As String,
                             ByVal Imputazione_Cod As Integer,
                             ByVal Imputazione_Classe_Cod As Integer,
                             ByVal Imputazione_Nome As String,
                             ByVal Imputazione_Cod_Des As String,
                             ByVal Imputazione_Des As String,
                             ByVal Tipo_Imputazione As Integer,
                             ByVal Settore As String,
                             ByVal Modulo As String,
                             ByVal Superficie As Decimal,
                             ByVal Superficie_Consuntivo As Decimal,
                             ByVal Imputazione_Preset_Cod As Integer,
                             ByVal Analisi As String,
                             ByVal ChkImputazione As Integer,
                             ByVal ChkUtilizzaInReport As Integer,
                             ByVal ChkRisorse_Umane As Integer,
                             ByVal ChkMacchine As Integer,
                             ByVal ChkProdotti As Integer,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                             Optional Imputazione_Interna As Integer = 0
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Imputazioni_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine("UPDATE Imputazioni SET ")
            StrSQL.AppendLine("     Imputazione_Nome                 = '" & Agro_SQL_SaveText(Imputazione_Nome) & "'  ")
            StrSQL.AppendLine("     ,Imputazione_Cod_Des              = '" & Agro_SQL_SaveText(Imputazione_Cod_Des) & "'  ")
            StrSQL.AppendLine("     ,Imputazione_Des                  = '" & Agro_SQL_SaveText(Imputazione_Des) & "'  ")
            StrSQL.AppendLine("     ,Imputazione_Classe_Cod           =  " & Agro_SQL_SaveNum(Imputazione_Classe_Cod) & "   ")
            StrSQL.AppendLine("     ,Tipo_Imputazione                 =  " & Agro_SQL_SaveNum(Tipo_Imputazione) & "   ")
            StrSQL.AppendLine("     ,Settore                       = '" & Agro_SQL_SaveText(Settore) & "'  ")
            StrSQL.AppendLine("     ,Modulo                        = '" & Agro_SQL_SaveText(Modulo) & "'  ")
            StrSQL.AppendLine("     ,Superficie                    =  " & Agro_SQL_SaveNum(Superficie) & "   ")
            StrSQL.AppendLine("     ,Analisi                       = '" & Agro_SQL_SaveText(Analisi) & "'  ")
            StrSQL.AppendLine("     ,ChkImputazione                =  " & Agro_SQL_SaveNum(ChkImputazione) & "   ")
            StrSQL.AppendLine("     ,ChkUtilizzaInReport           =  " & Agro_SQL_SaveNum(ChkUtilizzaInReport) & "   ")
            StrSQL.AppendLine("     ,ChkRisorse_Umane              =  " & Agro_SQL_SaveNum(ChkRisorse_Umane) & "   ")
            StrSQL.AppendLine("     ,ChkMacchine                   =  " & Agro_SQL_SaveNum(ChkMacchine) & "   ")
            StrSQL.AppendLine("     ,Superficie_Consuntivo         =  " & Agro_SQL_SaveNum(Superficie_Consuntivo) & "   ")
            StrSQL.AppendLine("     ,Imputazione_Preset_Cod        =  " & Agro_SQL_SaveNum(Imputazione_Preset_Cod) & "   ")
            StrSQL.AppendLine("     ,Imputazione_Interna        =  " & Agro_SQL_SaveNum(Imputazione_Interna) & "   ")

            StrSQL.AppendLine("      ,Inviato           =  0 ")
            StrSQL.AppendLine("      ,DataInvio         =  Null ")
            StrSQL.AppendLine("      ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now))
            StrSQL.AppendLine("      ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.AppendLine("      ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.AppendLine("      ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            StrSQL.AppendLine(" WHERE Piva_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")
            StrSQL.AppendLine(" AND   Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.AppendLine(" AND   Imputazione_Cod = " & Agro_SQL_SaveNum(Imputazione_Cod) & " ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function


    Public Function Cancella(ByVal Piva As String,
                             ByVal Imputazione_Cod As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Imputazioni_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" DELETE ")
            StrSQL.AppendLine(" FROM     Imputazioni ")
            StrSQL.AppendLine(" WHERE  Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.AppendLine(" AND Imputazione_Cod = " & Agro_SQL_SaveNum(Imputazione_Cod) & "  ")

            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
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
