Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class Alert_Elenco_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(
                                ByVal ID_Elenco As Integer,
                                ByVal ID_Tipologia As Integer,
                                ByVal ID_Alert_Entita As Integer,
                                ByVal Data_Scadenza As Date,
                                ByVal Descrizione_Scadenza As String,
                                ByVal Gia_Passato As Integer,
                                ByVal Validita_Inizio As Date,
                                ByVal Validita_Fine As Date,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                Optional ByVal note As String = "",
                                Optional ByVal ID_App As String = ""
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Elenco_W.Scrivi()"

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
            StrSQL.Append("INSERT INTO Alert_Elenco ")
            StrSQL.Append("                   ( PivaSuperUser    ,ID_Elenco  ,ID_Tipologia , ID_Alert_Entita      ,Data_Scadenza       ,Gia_Passato , Descrizione_Scadenza, note, ID_App   ")


            StrSQL.Append("                    ,Inviato,            DataInvio, ")
            StrSQL.Append("                    Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("                    UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("                    Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("                   ) ")

            StrSQL.Append("VALUES (")


            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(ID_Elenco) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(ID_Tipologia) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(ID_Alert_Entita) & "  ")

            StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_Scadenza) & " ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Gia_Passato) & "  ")

            StrSQL.Append("         , '" & Agro_SQL_SaveText(Descrizione_Scadenza) & "' ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(note) & "' ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(ID_App) & "'  ")

            StrSQL.Append("         , 0 ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ," & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            StrSQL.Append("         ," & Agro_SQL_SaveDate(Validita_Fine) & " ")
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


    Public Function Modifica(
                                ByVal ID_Elenco As Integer,
                                ByVal ID_Tipologia As Integer,
                                ByVal ID_Alert_Entita As Integer,
                                ByVal Data_Scadenza As Date,
                                ByVal Gia_Passato As Integer,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Elenco_W.Modifica()"

        '====================================================================================
        'Parametri opzionali :
        '   Latitudine = 0
        '   Longitudine = 0
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try


            StrSQL.Append(" UPDATE Alert_Elenco SET ")
            StrSQL.Append("    ID_Tipologia           = " & Agro_SQL_SaveNum(ID_Tipologia) & " ")
            StrSQL.Append("    ID_Alert_Entita           = " & Agro_SQL_SaveNum(ID_Alert_Entita) & " ")
            StrSQL.Append("    Data_Scadenza           = " & Agro_SQL_SaveDate(Data_Scadenza) & " ")
            StrSQL.Append("   ,Gia_Passato        = " & Agro_SQL_SaveNum(Gia_Passato) & " ")

            StrSQL.Append("   ,Inviato           =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(DateTime.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            StrSQL.Append(" WHERE   ID_Elenco        =" & Agro_SQL_SaveNum(ID_Elenco) & " ")
            StrSQL.Append(" AND     PivaSuperUser      = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")


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

    Public Function Modifica(
                                ByVal ID_Elenco As Integer,
                                ByVal Id_Tipologia As Integer,
                                ByVal Data_Scadenza As Date,
                                ByVal Descrizione_Scadenza As String,
                                ByVal Note As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Elenco_W.Modifica()"

        '====================================================================================
        'Parametri opzionali :
        '   Latitudine = 0
        '   Longitudine = 0
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            StrSQL.Append(" UPDATE Alert_Elenco SET ")
            StrSQL.Append("    Id_Tipologia           = " & Agro_SQL_SaveNum(Id_Tipologia) & " ")
            StrSQL.Append("   ,Data_Scadenza           = " & Agro_SQL_SaveDate(Data_Scadenza) & " ")
            StrSQL.Append("   ,Descrizione_Scadenza        = '" & Agro_SQL_SaveText(Descrizione_Scadenza) & "' ")
            StrSQL.Append("   ,Note        = '" & Agro_SQL_SaveText(Note) & "' ")

            StrSQL.Append("   ,Inviato           =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(DateTime.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            StrSQL.Append(" WHERE   ID_Elenco        =" & Agro_SQL_SaveNum(ID_Elenco) & " ")
            StrSQL.Append(" AND     PivaSuperUser      = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")

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

    Public Function ModificaAutomatica(ByVal ID_Elenco As Integer,
                                       ByVal Data_Scadenza As Date,
                                       ByVal Descrizione_Scadenza As String,
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                       ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Elenco_W.ModificaAutomatica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            StrSQL.Append(" UPDATE Alert_Elenco SET ")
            StrSQL.Append("     Data_Scadenza = " & Agro_SQL_SaveDate(Data_Scadenza) & " ")
            StrSQL.Append("   , Descrizione_Scadenza = '" & Agro_SQL_SaveText(Descrizione_Scadenza) & "' ")

            StrSQL.Append("   , Inviato =  0 ")
            StrSQL.Append("   , DataInvio =  NULL ")
            StrSQL.Append("   , Data_Modifica =  " & Agro_SQL_SaveDateTime(DateTime.Now))
            StrSQL.Append("   , UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            StrSQL.Append(" WHERE ID_Elenco = " & Agro_SQL_SaveNum(ID_Elenco) & " ")
            StrSQL.Append(" AND PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")

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


    Public Function Cancella(ByVal ID_Elenco As Integer,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Elenco_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""
        '   Sa_Cod = 0
        '   Id_Trasformazione = 0

        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Alert_Elenco ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0 ")

            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM Alert_Elenco ")
                StrSQL.Append(" WHERE  1=1 ")

            End If

            StrSQL.Append(" AND ID_Elenco = " & Agro_SQL_SaveNum(ID_Elenco) & " ")
            StrSQL.Append(" AND pivasuperuser =    '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

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

'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################


Public Class Alert_Elenco_R
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function Leggi(ByVal ID_Elenco As Integer,
                          ByVal ID_Tipologia As Integer,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          Optional ByVal xFiltroAggiuntivo As String = "",
                          Optional ByVal xOrderBy As String = ""
                          ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Elenco_r.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT *  ")
            StrSQL.Append(" FROM  Alert_Elenco ")
            StrSQL.Append(" WHERE 1= 1 ")


            If ID_Elenco <> 0 Then
                StrSQL.Append(" AND ID_Elenco = " & Agro_SQL_SaveNum(ID_Elenco) & " ")
            End If

            If ID_Tipologia <> 0 Then
                StrSQL.Append(" AND ID_Tipologia = " & Agro_SQL_SaveNum(ID_Tipologia) & " ")
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------


            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

#Region "Non Usata"
    'Anna 29/04/22: aggiunti campi al filtro di ricerca
    Public Function Leggi_UtentiUpload(ByVal piva As String,
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Elenco_r.Leggi_Allegato()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" Select UserName_Upload as Nome")
            StrSQL.Append(" FROM  Allegati_Documenti ")


            StrSQL.Append(" WHERE 1=1 ")
            StrSQL.Append(" AND UserName_Upload <> '' ")


            If piva <> "" Then
                StrSQL.Append(" AND Allegati_Documenti_Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            StrSQL.Append("GROUP BY UserName_Upload ")
            StrSQL.Append("ORDER BY Username_Upload")

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
#End Region
    Public Function Leggi_Allegato(ByVal Piva As String,
                                   ByVal Allegati_Documenti_Cod As Integer,
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                   Optional xFiltro_Aggiuntivo As String = "",
                                   Optional getInfoDocumento As Boolean = False,
                                   Optional workflowAbilitato As Boolean = False) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Elenco_r.Leggi_Allegato()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT ad.*  ")

            If getInfoDocumento Then
                StrSQL.AppendLine(", ('<b>' + ad.Allegati_Documenti_NomeFile + '</b> di ' + i.rag_soc + ' (' + CASE WHEN aa.Nome like '%COPROB%' THEN at.Nome ELSE aa.Nome + ', ' + at.Nome END  + ')') AS infoDocumento")
                StrSQL.AppendLine(", ('<b>' + ad.Allegati_Documenti_NomeFile + '</b>  (' + at.Nome + ')') AS infoDocumentoAudit")
                StrSQL.AppendLine(", ad.Validazione_Flag, a.ChkStorico")
            End If

            If workflowAbilitato Then
                StrSQL.AppendLine(" , psa.stato_cod As Stato_Attuale ")
            End If

            StrSQL.AppendLine(" FROM  Allegati_Documenti ad ")

            If getInfoDocumento Then
                StrSQL.AppendLine(" LEFT JOIN Alert_Entita a on ad.Allegati_Documenti_Cod = a.Allegati_Documenti_Cod")
                StrSQL.AppendLine(" LEFT JOIN Alert_Elenco ae on a.ID_Alert_Entita = ae.ID_Alert_Entita")
                StrSQL.AppendLine(" LEFT JOIN Imprese i on ad.Allegati_Documenti_Piva = i.PIVA")
                StrSQL.AppendLine(" LEFT JOIN Alert_Tipologia at on at.ID_Tipologia = ae.ID_Tipologia")
                StrSQL.AppendLine(" LEFT JOIN Alert_Area aa on aa.ID_Area = at.ID_Area")
            End If

            If workflowAbilitato Then
                StrSQL.AppendLine(" LEFT JOIN pratiche_Stati_Attuali psa ON ad.pratica_Cod = psa.pratica_Cod")
            End If

            StrSQL.AppendLine(" WHERE ad.Allegati_Documenti_SuperUser = '" & objParametri.PivaSuperUser & "' ")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND ad.Allegati_Documenti_Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Allegati_Documenti_Cod <> 0 Then
                StrSQL.AppendLine(" AND ad.Allegati_Documenti_Cod = " & Agro_SQL_SaveNum(Allegati_Documenti_Cod) & " ")
            End If

            If xFiltro_Aggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltro_Aggiuntivo,, objParametri) & " ")
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   ad.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   ad.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

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



    Public Function Leggi_Elenco(ByVal SelectTOP As Integer,
                                 ByVal Filtro_Area As String,
                                 ByVal Filtro_Tipologia As String,
                                 ByVal Piva As String,
                                 ByVal Attivo As Integer,
                                 ByVal Solo_Allegati As Integer,
                                 ByVal Tutti1_soloInScadenza2 As Integer,
                                 ByVal xFiltroAggiuntivo As String,
                                 ByVal xOrderBy As String,
                                 ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                 ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                 Optional ByVal ChkDocumento As Integer = -1,
                                 Optional ByVal ChkSoloAttive As Integer = 0,
                                 Optional ByVal ChkSoloNonStorico As Integer = 0,
                                 Optional ByVal Validita_Inizio As String = "",
                                 Optional ByVal Validita_Fine As String = "",
                                 Optional ByVal BypassControlloPermessi As Boolean = False,
                                 Optional ByVal Controlla_Solo_Utenti_Visibilita As Boolean = False,
                                 Optional ByVal Controlla_Solo_Utenti_Visibilita_Appoggio As Boolean = False,
                                 Optional Stato_Validazione As Integer = -10,
                                 Optional Utente_Upload As Integer = 0,
                                 Optional Inizio_Upload As String = "",
                                 Optional Fine_Upload As String = "",
                                 Optional workflow_Documentale As Boolean = False,
                                 Optional estraiFileAllegatoDB As Boolean = True,
                                 Optional Indici As String = "") As DataTable

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Elenco_r.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Dim PivaSuperUser As String = objParametri_Server.PivaSuperUser
            Dim UtenteUsername As String = objParametri_Server.UtenteUsername
            Dim DT_Permessi As New DataTable
            Dim DT_Gerarchia As New DataTable

            Dim bPermessixUtente As Boolean = False 'Booleano per la presenza di record in tabella obj_CategTipologiaDocumentiXUtenti
            'In caso non esistano record viene bypassato il filtro

            Dim iLivelloGerarchia As Integer = 0 'Livello Max della tabella GerarchiaImprese (in modo da costruire un filtro dinamico)

            'Il superuser vede tutto
            Dim Flag_SuperUSer As Boolean = IIf(objParametri_Server.UtenteUsername.ToLower() <> objParametri_Server.SuperUserUsername.ToLower(), False, True)

            If Not Flag_SuperUSer And Not BypassControlloPermessi Then

                'Controllo se sono stati inseriti dei Permessi
                Dim obj_CategTipologiaDocumentiXUtenti As New AgronicaCoreScadenziario.CategTipologiaDocumentiXUtenti_R

                DT_Permessi = obj_CategTipologiaDocumentiXUtenti.Leggi("", 0, Nothing, enumSelezioneVariabile.Selezione_TabellaCompleta, objParametri_Server, Nothing, Nothing)

                If Not IsNothing(DT_Permessi) AndAlso DT_Permessi.Rows.Count > 0 Then

                    bPermessixUtente = True

                    ''Lettura della tabella gerarchia imprese per determinarne il livello max                
                    Dim GI As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R
                    DT_Gerarchia = GI.LeggixFiglio("", enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                   "", "Livello Desc",
                                                   objParametri_Server)

                    If Not IsNothing(DT_Gerarchia) AndAlso DT_Gerarchia.Rows.Count > 0 Then

                        iLivelloGerarchia = DT_Gerarchia(0)("Livello")

                    End If

                End If

            End If

            If Not Controlla_Solo_Utenti_Visibilita Then
                Componi_Leggi_Elenco(-1,
                                     StrSQL,
                                     PivaSuperUser,
                                     UtenteUsername,
                                     bPermessixUtente,
                                     iLivelloGerarchia,
                                     Flag_SuperUSer,
                                     SelectTOP,
                                     Filtro_Area,
                                     Filtro_Tipologia,
                                     Piva,
                                     Attivo,
                                     Solo_Allegati,
                                     Tutti1_soloInScadenza2,
                                     xFiltroAggiuntivo,
                                     xOrderBy,
                                     objParametri_Server,
                                     objParametri_Utenti,
                                     ChkDocumento,
                                     ChkSoloAttive,
                                     ChkSoloNonStorico,
                                     Validita_Inizio,
                                     Validita_Fine,
                                     Stato_Validazione:=Stato_Validazione,
                                     Utente_Upload:=Utente_Upload,
                                     Inizio_Upload:=Inizio_Upload,
                                     Fine_Upload:=Fine_Upload,
                                     Workflow_Documentale:=workflow_Documentale,
                                     estraiFileAllegatoDB:=estraiFileAllegatoDB,
                                     Indici:=Indici)

                If Not Flag_SuperUSer AndAlso Not Controlla_Solo_Utenti_Visibilita_Appoggio Then

                    'Entra in gioco la seconda UNION solo se viene gestita la tabella Utenti_Visibilita per l'area UMA
                    Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                    Dim objUtentixGruppiUtente As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_R
                    Dim DT_UtentixGruppiUtente = objUtentixGruppiUtente.Leggi(UtenteUsername, 0, "", "", objParametri_Utenti)

                    Dim lista_gruppi As New List(Of Integer)
                    If DT_UtentixGruppiUtente IsNot Nothing AndAlso DT_UtentixGruppiUtente.Rows.Count > 0 Then
                        For Each dr In DT_UtentixGruppiUtente.Rows
                            lista_gruppi.Add(dr("Gruppi_Utente_cod"))
                        Next
                    End If

                    Dim xFiltroAggiuntivo_UtentiVisibilita = "Utenti_Visibilita.Area = '" & enum_Area_Visibilita.UMA & "' AND (Utenti_Visibilita.UserName = '" & UtenteUsername & "'"

                    If lista_gruppi.Count > 0 Then
                        xFiltroAggiuntivo_UtentiVisibilita += " OR Utenti_Visibilita.Gruppo IN (" & Agro_SQL_Save_Clausola_IN(String.Join(",", lista_gruppi.ToArray)) & "))"
                    Else
                        xFiltroAggiuntivo_UtentiVisibilita += ")"
                    End If

                    Dim DtUtentiVisibilita As DataTable = objUtentiVisibilita.LeggiAree(xFiltroAggiuntivo_UtentiVisibilita, "", objParametri_Utenti)


                    If Not IsNothing(DtUtentiVisibilita) AndAlso DtUtentiVisibilita.Rows.Count > 0 Then


                        StrSQL.AppendLine("UNION")

                        Componi_Leggi_Elenco(enum_Area_Visibilita.UMA,
                                             StrSQL,
                                             PivaSuperUser,
                                             UtenteUsername,
                                             bPermessixUtente,
                                             iLivelloGerarchia,
                                             Flag_SuperUSer,
                                             SelectTOP,
                                             Filtro_Area,
                                             Filtro_Tipologia,
                                             Piva,
                                             Attivo,
                                             Solo_Allegati,
                                             Tutti1_soloInScadenza2,
                                             xFiltroAggiuntivo,
                                             xOrderBy,
                                             objParametri_Server,
                                             objParametri_Utenti,
                                             ChkDocumento,
                                             ChkSoloAttive,
                                             ChkSoloNonStorico,
                                             Validita_Inizio,
                                             Validita_Fine,
                                             Stato_Validazione:=Stato_Validazione,
                                             Utente_Upload:=Utente_Upload,
                                             Inizio_Upload:=Inizio_Upload,
                                             Fine_Upload:=Fine_Upload,
                                             Workflow_Documentale:=workflow_Documentale,
                                             estraiFileAllegatoDB:=estraiFileAllegatoDB,
                                             Indici:=Indici)

                    End If

                End If

            Else

                Componi_Leggi_Elenco(enum_Area_Visibilita.UMA,
                                     StrSQL,
                                     PivaSuperUser,
                                     UtenteUsername,
                                     bPermessixUtente,
                                     iLivelloGerarchia,
                                     Flag_SuperUSer,
                                     SelectTOP,
                                     Filtro_Area,
                                     Filtro_Tipologia,
                                     Piva,
                                     Attivo,
                                     Solo_Allegati,
                                     Tutti1_soloInScadenza2,
                                     xFiltroAggiuntivo,
                                     xOrderBy,
                                     objParametri_Server,
                                     objParametri_Utenti,
                                     ChkDocumento,
                                     ChkSoloAttive,
                                     ChkSoloNonStorico,
                                     Validita_Inizio,
                                     Validita_Fine,
                                     Stato_Validazione:=Stato_Validazione,
                                     Utente_Upload:=Utente_Upload,
                                     Inizio_Upload:=Inizio_Upload,
                                     Fine_Upload:=Fine_Upload,
                                     Workflow_Documentale:=workflow_Documentale,
                                     estraiFileAllegatoDB:=estraiFileAllegatoDB,
                                     Indici:=Indici)

            End If

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Server))
            Else
                StrSQL.AppendLine(" ORDER BY Data_Scadenza desc ")
            End If


            If LivelloCompatibilita(objParametri_Server) >= 150 Then
                StrSQL.AppendLine(" OPTION (USE HINT ('FORCE_LEGACY_CARDINALITY_ESTIMATION')) ")
            End If

            '-------------------------------------------------------------------------- 
            DT = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)
            '-------------------------------------------------------------------------- 

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function


    Private Sub Componi_Leggi_Elenco(ByVal FiltroVisibilitaUtentiDaUtilizzare As Integer,
                                     ByRef StrSQL As System.Text.StringBuilder,
                                     ByVal PivaSuperUser As String,
                                     ByVal UtenteUsername As String,
                                     ByVal bPermessixUtente As Boolean,
                                     ByVal iLivelloGerarchia As Integer,
                                     ByVal Flag_SuperUSer As Boolean,
                                     ByVal SelectTOP As Integer,
                                     ByVal Filtro_Area As String,
                                     ByVal Filtro_Tipologia As String,
                                     ByVal Piva As String,
                                     ByVal Attivo As Integer,
                                     ByVal Solo_Allegati As Integer,
                                     ByVal Tutti1_soloInScadenza2 As Integer,
                                     ByVal xFiltroAggiuntivo As String,
                                     ByVal xOrderBy As String,
                                     ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                     ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                     ByVal ChkDocumento As Integer,
                                     ByVal ChkSoloAttive As Integer,
                                     ByVal ChkSoloNonStorico As Integer,
                                     ByVal Validita_Inizio As String,
                                     ByVal Validita_Fine As String,
                                     Optional Stato_Validazione As Integer = -10,
                                     Optional Utente_Upload As Integer = 0,
                                     Optional Inizio_Upload As String = "",
                                     Optional Fine_Upload As String = "",
                                     Optional Workflow_Documentale As Boolean = False,
                                     Optional estraiFileAllegatoDB As Boolean = True,
                                     Optional Indici As String = "")

        Dim FiltroUtente As Boolean = False
        Dim FiltroGruppoUtente As Boolean = False

        Dim VisibilitaTotale As Boolean = False

        StrSQL.AppendLine(" SELECT ")
        If SelectTOP <> 0 Then
            StrSQL.AppendLine(" TOP " & SelectTOP)
        End If
        StrSQL.AppendLine(" Alert_Elenco.ID_Tipologia, Alert_Tipologia.ID_Area AS ID_Categoria, Alert_Elenco.ID_Alert_Entita, ISNULL(Allegati_Documenti.Validazione_Flag, 0) AS Validazione_Flag, ")
        StrSQL.AppendLine(" ISNULL(Allegati_Documenti.Allegati_Documenti_Cod, 0) As Allegati_Documenti_Cod, ISNULL(Allegati_Documenti.allegati_documenti_estensione, '') As allegati_documenti_estensione, ")
        StrSQL.AppendLine(" CASE WHEN CONVERT(VARCHAR(10), Alert_Elenco.Data_Scadenza , 103) = '31/12/2100' THEN '' ELSE CONVERT(VARCHAR(10), Alert_Elenco.Data_Scadenza , 103) END AS Data_Scadenza, Alert_Elenco.Data_Scadenza As Data_Scadenza_Data, ")
        StrSQL.AppendLine(" Alert_Elenco.Gia_Passato, Alert_Elenco.ID_Elenco, Alert_Elenco.Descrizione_Scadenza, Alert_Elenco.Note, ")
        If estraiFileAllegatoDB Then
            StrSQL.AppendLine(" Allegati_Documenti.File_Allegato_DB, ")
        Else
            StrSQL.AppendLine(" '' File_Allegato_DB, ")
        End If
        StrSQL.AppendLine(" Allegati_Documenti.Allegati_Documenti_Piva, Alert_Entita.Piva, ")
        StrSQL.AppendLine(" CASE WHEN ISNULL(Imprese.partitaIvaReale, '') = '' THEN Alert_Entita.PIVA ELSE Imprese.partitaIvaReale END PivaReale, ")

        'StrSQL.AppendLine(" Alert_Elenco.Gia_Passato, Alert_Elenco.ID_Elenco, Alert_Elenco.Descrizione_Scadenza, Alert_Elenco.Note, Allegati_Documenti.File_Allegato_DB,Allegati_Documenti.Allegati_Documenti_Piva, Alert_Entita.Piva, ")

        StrSQL.AppendLine(" Alert_Tipologia.Nome As Categoria_2 ,Alert_Area.Nome As Categoria_1, ")
        StrSQL.AppendLine(" Alert_Entita.Cod_Contatto, Alert_Entita.Mac_Cod, Alert_Entita.ChkDocumento, ")
        StrSQL.AppendLine(" Allegati_Documenti.sottocartella, ISNULL(Allegati_Documenti.Allegati_documenti_nomefile, '') AS Allegati_documenti_nomefile , Imprese.rag_soc As 'Azienda', ")
        StrSQL.AppendLine(" (Dettagli.Cognome + ' ' + Dettagli.Nome + ' ' + Dettagli.Rag_soc ) AS Username_Upload, ISNULL(Allegati_Documenti.Data_Upload, '') AS Data_Upload, ISNULL(Allegati_Documenti.Allegati_Documenti_Numero, '') AS Allegati_Documenti_Numero, ")
        StrSQL.AppendLine(" (ISNULL(indirizzi.Ind_Des, '') + ' - ' + ISNULL(indirizzi.Cap, '') + ' - ' + ISNULL(indirizzi.Com_Des, '')) AS Indirizzo, ISNULL(Lista_Province.Provincia, '') AS Provincia, ISNULL(Lista_Regioni.Regione_Des, '') AS Regione, ISNULL(indirizzi.Stato, '') AS Stato, ")
        StrSQL.AppendLine(" ISNULL(Imprese_Codici.Val_Cod, '') AS Codice_Socio, ")
        StrSQL.AppendLine(" ISNULL(UMA_Richieste_Testata.Richiesta_Cod, 0) AS Richiesta_Cod, ISNULL(Pratiche.Numero, '') AS Pratica, ISNULL(UMA_Richieste_Testata.Avanzamento_Richiesta, 0) AS Avanzamento_Richiesta,")
        StrSQL.AppendLine(" ISNULL(Pratiche_Stati_Attuali.Stato_Cod, 0) AS Pratica_Stato_Cod, ISNULL(Alert_Entita.ChkStorico, 0) AS ChkStorico, ")
        StrSQL.AppendLine(" ISNULL(Analisi_Testata.Analisi_Testata_Cod, 0) AS Analisi_Testata_Cod, ISNULL(Analisi_Testata.Analisi_Testata_Des,'') AS Analisi,")
        'StrSQL.AppendLine(" ISNULL(Reg_Impianti_Codici.Val_Cod, '') AS Grower,")

        'Anna: aggiunti nuovi stati di validazione documento
        'StrSQL.AppendLine(" (CASE When Allegati_Documenti.Validazione_Flag = -1 THEN '" & Gias.DocumentoNonValido & "' When Allegati_Documenti.Validazione_Flag = 1 THEN '" & Gias.DocumentoValido & "' Else '" & Gias.DocumentoDaValidare & "' End) AS Validazione_Des, ")
        StrSQL.AppendLine(" CASE WHEN Allegati_Documenti.Validazione_Flag = -1 THEN '" & Gias.DocumentoNonValidoUfficio & "'")
        StrSQL.AppendLine(" WHEN Allegati_Documenti.Validazione_Flag = -2 THEN '" & Gias.DocumentoNonvalidoAutocontrollo & "'")
        StrSQL.AppendLine(" WHEN Allegati_Documenti.Validazione_Flag = 0 THEN '" & Gias.DocumentoDaValidare & "'")
        StrSQL.AppendLine(" WHEN Allegati_Documenti.Validazione_Flag = 1 THEN '" & Gias.DocumentoValidoUfficio & "'")
        StrSQL.AppendLine(" WHEN Allegati_Documenti.Validazione_Flag = 2 THEN '" & Gias.DocumentoValidoAutocontrollo & "'")
        StrSQL.AppendLine(" END AS Validazione_Des, Allegati_Documenti.Validazione_Flag AS Validazione_Flag, ")

        StrSQL.AppendLine(" CASE WHEN Alert_Entita.ChkStorico = 0 THEN '" & Gias.No & "'")
        StrSQL.AppendLine(" WHEN Alert_Entita.ChkStorico = 1 THEN '" & Gias.Si & "'")
        StrSQL.AppendLine(" ELSE ''")
        StrSQL.AppendLine(" END AS Storico, ")

        'Anna: aggiunto componente kendoUpload per la gestione multi allegato
        StrSQL.AppendLine(" Allegati_Documenti.CompressoDaGIAS,")

        'Anna 06/06/22: aggiunta colonne Agenda e ricette
        StrSQL.AppendLine(" ISNULL(Agenda.ID_Agenda, 0) AS ID_Agenda, ISNULL(Agenda.des_lib,'')  AS Riferimento_Agenda,")
        StrSQL.AppendLine(" ISNULL(Operazioni.LAV_DES, '') AS Descrizione_Operazione, ")

        StrSQL.AppendLine(" --DOC CONTABILI")
        StrSQL.AppendLine(" ISNULL(Agenda.Lav_Cod, 0) AS Lav_Cod_DocContabili,  ")
        'StrSQL.AppendLine(" ISNULL(Data_Movimento, '') as Data_Movimento_DocContabili,  ")
        StrSQL.AppendLine(" CASE WHEN Data_Movimento = '1900-01-01 00:00:00.000' THEN '' ELSE Data_Movimento END as Data_Movimento_DocContabili,    ")
        StrSQL.AppendLine(" ISNULL(Contatti.Rag_Soc, '') AS Rag_Soc_Contatto_DocContabili, ")
        StrSQL.AppendLine(" ISNULL(Contatti.Cod_Contatto, '') as Cod_Contatto_DocContabili,  ")
        StrSQL.AppendLine(" ISNULL(Doc_Numero_Visualizzato, '') AS Doc_Numero_Visualizzato_DocContabili, ")

        StrSQL.AppendLine(" --QDC ")
        StrSQL.AppendLine(" ISNULL(Agenda.Validita_Inizio, '') as Data_Agenda, ")

        StrSQL.AppendLine(" --RICETTE ")
        StrSQL.AppendLine(" ISNULL(Ricette.Ricetta_Cod, 0) AS Ricetta_Cod, ISNULL(Ricette_Operazioni.Ricetta_Operazione_Cod, 0) AS Ricetta_Operazione_Cod, ")
        StrSQL.AppendLine(" ISNULL(Ricette_Operazioni.W_Anagrafica_Stati_Cod, 0) AS W_Anagrafica_Stati_Cod, ")
        StrSQL.AppendLine(" CASE WHEN Ricette_Operazioni.W_Anagrafica_Stati_Cod = 300 THEN 'Ricetta' WHEN Ricette_Operazioni.W_Anagrafica_Stati_Cod = 301 THEN 'Brogliaccio' END AS RicettaBrogliaccio, ")
        StrSQL.AppendLine(" ISNULL(Ricette_Operazioni.Ricetta_Operazione_Des, '') AS Ricetta, ")
        StrSQL.AppendLine(" ISNULL(Ricette.Ricetta_Numero, '') AS Codice_Ricetta, ")
        If Workflow_Documentale Then
            StrSQL.AppendLine(" psa.stato_cod As Stato_Attuale, ")
            StrSQL.AppendLine(" wa.WAnagraficaStati_Des As Descrizione_Stato, ")
        End If


        Select Case bPermessixUtente

            Case True


                StrSQL.AppendLine(" ISNULL(permessi.Autorizzato, 0) AS Autorizzato, ")
                StrSQL.AppendLine(" Abs(ISNULL(permessi.Id_Tipologia, 0)) AS Autorizzato_Tipologia, ")

                If iLivelloGerarchia > 1 Then
                    StrSQL.AppendLine(" ISNULL(permessi_padre.Autorizzato, 0) AS Autorizzato_Padre, ")
                    StrSQL.AppendLine(" Abs(ISNULL(permessi_padre.Id_Tipologia, 0)) AS Autorizzato_Padre_Tipologia, ")
                Else
                    StrSQL.AppendLine(" 0 AS Autorizzato_Padre, ")
                    StrSQL.AppendLine(" 0 AS Autorizzato_Padre_Tipologia, ")
                End If

                If iLivelloGerarchia > 2 Then
                    StrSQL.AppendLine(" ISNULL(permessi_nonno.Autorizzato, 0) AS Autorizzato_Nonno, ")
                    StrSQL.AppendLine(" Abs(ISNULL(permessi_nonno.Id_Tipologia, 0)) AS Autorizzato_Nonno_Tipologia, ")
                Else
                    StrSQL.AppendLine(" 0 AS Autorizzato_Nonno, ")
                    StrSQL.AppendLine(" 0 AS Autorizzato_Nonno_Tipologia, ")
                End If

                If iLivelloGerarchia > 3 Then
                    StrSQL.AppendLine(" ISNULL(permessi_bis_nonno.Autorizzato, 0) AS Autorizzato_Bis_Nonno, ")
                    StrSQL.AppendLine(" Abs(ISNULL(permessi_bis_nonno.Id_Tipologia, 0)) AS Autorizzato_Bis_Nonno_Tipologia, ")
                Else
                    StrSQL.AppendLine(" 0 AS Autorizzato_Bis_Nonno, ")
                    StrSQL.AppendLine(" 0 AS Autorizzato_Bis_Nonno_Tipologia, ")
                End If

                If iLivelloGerarchia > 4 Then
                    StrSQL.AppendLine(" ISNULL(permessi_tris_nonno.Autorizzato, 0) AS Autorizzato_Tris_Nonno, ")
                    StrSQL.AppendLine(" Abs(ISNULL(permessi_tris_nonno.Id_Tipologia, 0)) AS Autorizzato_Tris_Nonno_Tipologia, ")
                Else
                    StrSQL.AppendLine(" 0 AS Autorizzato_Tris_Nonno, ")
                    StrSQL.AppendLine(" 0 AS Autorizzato_Tris_Nonno_Tipologia, ")
                End If

                If iLivelloGerarchia > 5 Then
                    StrSQL.AppendLine(" ISNULL(permessi_quad_nonno.Autorizzato, 0) AS Autorizzato_Quad_Nonno, ")
                    StrSQL.AppendLine(" Abs(ISNULL(permessi_quad_nonno.Id_Tipologia, 0)) AS Autorizzato_Quad_Nonno_Tipologia ")
                Else
                    StrSQL.AppendLine(" 0 AS Autorizzato_Quad_Nonno, ")
                    StrSQL.AppendLine(" 0 AS Autorizzato_Quad_Nonno_Tipologia ")
                End If


            Case False

                'Tutti permessi
                StrSQL.AppendLine(" 1 AS Autorizzato, ")
                StrSQL.AppendLine(" 0 AS Autorizzato_Tipologia, ")
                StrSQL.AppendLine(" 1 AS Autorizzato_Padre, ")
                StrSQL.AppendLine(" 0 AS Autorizzato_Padre_Tipologia, ")
                StrSQL.AppendLine(" 1 AS Autorizzato_Nonno, ")
                StrSQL.AppendLine(" 0 AS Autorizzato_Nonno_Tipologia, ")
                StrSQL.AppendLine(" 1 AS Autorizzato_Bis_Nonno, ")
                StrSQL.AppendLine(" 0 AS Autorizzato_Bis_Nonno_Tipologia, ")
                StrSQL.AppendLine(" 1 AS Autorizzato_Tris_Nonno, ")
                StrSQL.AppendLine(" 0 AS Autorizzato_Tris_Nonno_Tipologia, ")
                StrSQL.AppendLine(" 1 AS Autorizzato_Quad_Nonno, ")
                StrSQL.AppendLine(" 0 AS Autorizzato_Quad_Nonno_Tipologia ")

        End Select



        StrSQL.AppendLine(" FROM Alert_Elenco ")
        StrSQL.AppendLine(" INNER JOIN Alert_Tipologia ON Alert_Elenco.ID_Tipologia = Alert_Tipologia.ID_Tipologia ")
        StrSQL.AppendLine(" INNER JOIN Alert_Area ON Alert_Tipologia.ID_Area = Alert_Area.ID_Area ")
        'StrSQL.AppendLine(" LEFT JOIN Alert_Tipologia_X_Utente ON Alert_Tipologia.ID_Tipologia = Alert_Tipologia_X_Utente.ID_Tipologia ")
        StrSQL.AppendLine(" INNER JOIN Alert_Entita ON Alert_Elenco.PivaSuperUser = Alert_Entita.PivaSuperUser AND Alert_Elenco.ID_Alert_Entita = Alert_Entita.ID_Alert_Entita ")
        StrSQL.AppendLine(" LEFT JOIN Allegati_Documenti ON Alert_Entita.allegati_documenti_cod = Allegati_Documenti.allegati_documenti_cod ")
        If Workflow_Documentale Then
            StrSQL.AppendLine(" LEFT JOIN pratiche_Stati_Attuali psa ON Allegati_Documenti.pratica_Cod= psa.pratica_Cod")
            StrSQL.AppendLine(" LEFT JOIN WAnagraficaStati wa ON wa.WAnagraficaStati_Cod = psa.Stato_Cod ")
        End If
        StrSQL.AppendLine(" INNER JOIN Imprese ON Alert_Entita.Piva = Imprese.Piva ")

        If Not Flag_SuperUSer Then

            Select Case FiltroVisibilitaUtentiDaUtilizzare

                Case enum_Area_Visibilita.UMA


                    Dim Gruppo As Integer = 0

                    Dim objUtenti_Visibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                    objUtenti_Visibilita.OttieniTipoFiltroVisibilita(objParametri_Utenti, 0, FiltroUtente, FiltroGruppoUtente, Gruppo, VisibilitaTotale)

                    If Not VisibilitaTotale Then
                        If FiltroUtente And Not FiltroGruppoUtente Then
                            StrSQL.AppendLine(" LEFT JOIN " & objParametri_Utenti.Recupera_NomeDB & ".dbo.Utenti_Visibilita visibilita ON Imprese.Piva = visibilita.Piva_Azienda AND visibilita.Username = '" & objParametri_Utenti.UtenteUsername & "' AND Alert_Area.ID_Area = " & enum_ID_Area_Alert.UMA_Carburanti)
                        End If
                        If Not FiltroUtente And FiltroGruppoUtente Then
                            StrSQL.AppendLine(" LEFT JOIN " & objParametri_Utenti.Recupera_NomeDB & ".dbo.Utenti_Visibilita visibilita ON Imprese.Piva = visibilita.Piva_Azienda AND visibilita.Gruppo = " & Gruppo & " AND Alert_Area.ID_Area = " & enum_ID_Area_Alert.UMA_Carburanti)
                        End If
                    End If


            End Select

        End If



        StrSQL.AppendLine(" LEFT OUTER JOIN ImpresexIndirizzi ON Alert_Entita.PIVA = ImpresexIndirizzi.PIVA ")
        StrSQL.AppendLine(" LEFT OUTER JOIN Indirizzi ON ImpresexIndirizzi.cod_indirizzo = Indirizzi.cod_indirizzo AND Tipo_indirizzo = 1 ")
        StrSQL.AppendLine(" LEFT OUTER JOIN ISTAT ON (Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM) ")
        StrSQL.AppendLine(" LEFT OUTER JOIN Lista_Province ON Lista_Province.Sigla = ISTAT.COMUNI_PROV ")
        StrSQL.AppendLine(" LEFT OUTER JOIN Lista_Regioni ON Lista_Regioni.REG = Lista_Province.REG ")
        StrSQL.AppendLine(" LEFT OUTER JOIN Imprese_Codici ON (Imprese_Codici.Piva = Alert_Entita.PIVA AND Id_Cod = 1033) ")

        StrSQL.AppendLine(" LEFT OUTER JOIN UMA_Richieste_Testata ON (UMA_Richieste_Testata.Richiesta_Cod = Alert_Entita.Richiesta_Cod) ")
        StrSQL.AppendLine(" LEFT OUTER JOIN Pratiche ON (Pratiche.Piva_SuperUser = UMA_Richieste_Testata.Piva_SuperUser AND Pratiche.Piva = UMA_Richieste_Testata.Piva AND Pratiche.Pratica_Cod = UMA_Richieste_Testata.Pratica_Cod) ")
        StrSQL.AppendLine(" LEFT OUTER JOIN Pratiche_Stati_Attuali ON (Pratiche.Piva_SuperUser = Pratiche_Stati_Attuali.Piva_SuperUser AND Pratiche.Pratica_Cod = Pratiche_Stati_Attuali.Pratica_Cod) ")

        StrSQL.AppendLine(" LEFT OUTER JOIN Analisi_Testata ON (Analisi_Testata.Analisi_SuperUser = Alert_Entita.PivaSuperUser AND Analisi_Testata.Analisi_Testata_Cod = Alert_Entita.Analisi_Testata_Cod)")

        'Anna 06/06/22: aggiunta colonna Riferimenti e Ricette alla griglia
        StrSQL.AppendLine(" LEFT OUTER JOIN Agenda ON (Agenda.PIVA = Alert_Entita.Piva AND Agenda.Id_Agenda = Alert_Entita.ID_Agenda)")
        StrSQL.AppendLine(" LEFT OUTER JOIN Ricette_Operazioni ON (Ricette_Operazioni.Ricetta_SuperUser = Alert_Entita.PivaSuperUser AND Ricette_Operazioni.Ricetta_Operazione_Cod = Alert_Entita.Ricetta_Operazione_cod)")
        StrSQL.AppendLine(" LEFT OUTER JOIN Ricette ON (Ricette.Ricetta_SuperUser = Alert_Entita.PivaSuperUser AND Ricette.Ricetta_Cod = Ricette_Operazioni.Ricetta_Cod)")

        StrSQL.AppendLine(" LEFT JOIN Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda")
        StrSQL.AppendLine(" LEFT JOIN  Risorse_Umane ON Movimenti.Cod_RisUm = Risorse_Umane.Cod_RisUm ")
        StrSQL.AppendLine(" LEFT JOIN Contatti ON Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto AND Risorse_Umane.Piva = Contatti.Piva ")
        StrSQL.AppendLine(" LEFT JOIN Operazioni ON Operazioni.LAV_COD = Agenda.Lav_Cod")



        'StrSQL.AppendLine(" LEFT OUTER JOIN Reg_Impianti_Codici ON (Reg_Impianti_Codici.Id_Cod = 1317 AND Exists (Select * From Imprese_Progetti ")
        'StrSQL.AppendLine("         WHERE Imprese_Progetti.Progetto_Cod = Reg_Impianti_Codici.Progetto_Cod ")
        'StrSQL.Append("            AND Imprese_Progetti.Validita_Inizio <= Allegati_Documenti.Data_Upload ")
        'StrSQL.Append("            AND Imprese_Progetti.Validita_Fine >= Allegati_Documenti.Data_Upload)) ")


        Dim NomeDB_Utenti As String = objParametri_Utenti.StringaConnessione.Split(";")(2).Split("=")(1)
        StrSQL.AppendLine(" LEFT JOIN " & NomeDB_Utenti & ".dbo.Utenti_Dettagli Dettagli ON Dettagli.UserName = Allegati_Documenti.Username_Upload")

        If bPermessixUtente Then

            'Ora se sono stati inseriti dei record nella tabella CategTipologiaDocumentiXUtenti,
            'l 'utente deve per forza essere autorizzato ad utilizzare quella Categoria oppure quella specifica Tipologia 

            '1. Controllo per Piva puntuale
            StrSQL.AppendLine(" Left Outer Join CategTipologiaDocumentiXUtenti permessi ON ( permessi.PivaSuperUser = Alert_Tipologia.PivaSuperUser AND permessi.ID_Categoria = Alert_Tipologia.ID_area AND ( permessi.ID_Tipologia = Alert_Tipologia.ID_Tipologia Or permessi.ID_Tipologia = 0 ) AND permessi.Autorizzato IN (1,2) AND permessi.Username = '" & Agro_SQL_SaveText(UtenteUsername) & "' AND permessi.Piva = Alert_Entita.Piva) ")

            If iLivelloGerarchia > 1 Then
                '2. Controllo per Piva Padre
                StrSQL.AppendLine(" Left Outer Join CategTipologiaDocumentiXUtenti permessi_padre ON ( permessi_padre.PivaSuperUser = Alert_Tipologia.PivaSuperUser AND permessi_padre.ID_Categoria = Alert_Tipologia.ID_area AND ( permessi_padre.ID_Tipologia = Alert_Tipologia.ID_Tipologia Or permessi_padre.ID_Tipologia = 0 ) AND permessi_padre.Autorizzato IN (1,2) AND permessi_padre.Username = '" & Agro_SQL_SaveText(UtenteUsername) & "' ")
                StrSQL.AppendLine(" and permessi_padre.Piva IN (Select Padre from GerarchiaImprese WHERE Figlio = alert_entita.Piva)) ")

            End If

            If iLivelloGerarchia > 2 Then
                '2. Controllo per Piva Nonno
                StrSQL.AppendLine(" Left Outer Join CategTipologiaDocumentiXUtenti permessi_nonno ON ( permessi_nonno.PivaSuperUser = Alert_Tipologia.PivaSuperUser AND permessi_nonno.ID_Categoria = Alert_Tipologia.ID_area AND ( permessi_nonno.ID_Tipologia = Alert_Tipologia.ID_Tipologia Or permessi_nonno.ID_Tipologia = 0 ) AND permessi_nonno.Autorizzato IN (1,2) AND permessi_nonno.Username = '" & Agro_SQL_SaveText(UtenteUsername) & "' ")
                StrSQL.AppendLine(" and permessi_nonno.Piva IN (Select Padre from GerarchiaImprese GerarchiaImpreseNonno WHERE Figlio IN (Select Padre from GerarchiaImprese GerarchiaImpresePadre WHERE Figlio = alert_entita.Piva))) ")
            End If

            If iLivelloGerarchia > 3 Then
                '3. Controllo per Piva Bis-Nonno
                StrSQL.AppendLine(" Left Outer Join CategTipologiaDocumentiXUtenti permessi_bis_nonno ON ( permessi_bis_nonno.PivaSuperUser = Alert_Tipologia.PivaSuperUser AND permessi_bis_nonno.ID_Categoria = Alert_Tipologia.ID_area AND ( permessi_bis_nonno.ID_Tipologia = Alert_Tipologia.ID_Tipologia Or permessi_bis_nonno.ID_Tipologia = 0 ) AND permessi_bis_nonno.Autorizzato IN (1,2) AND permessi_bis_nonno.Username = '" & Agro_SQL_SaveText(UtenteUsername) & "' ")
                StrSQL.AppendLine(" and permessi_bis_nonno.Piva IN ")
                StrSQL.AppendLine("    (Select Padre from GerarchiaImprese GerarchiaImpreseBisNonno WHERE Figlio IN ")
                StrSQL.AppendLine("    (Select Padre from GerarchiaImprese GerarchiaImpreseNonno2 WHERE Figlio IN (Select Padre from GerarchiaImprese GerarchiaImpresePadre2 WHERE Figlio = alert_entita.Piva))) ")
                StrSQL.AppendLine(" )")
            End If

            If iLivelloGerarchia > 4 Then
                '4. Controllo per Piva Tris-Nonno
                StrSQL.AppendLine(" Left Outer Join CategTipologiaDocumentiXUtenti permessi_tris_nonno ON ( permessi_tris_nonno.PivaSuperUser = Alert_Tipologia.PivaSuperUser AND permessi_tris_nonno.ID_Categoria = Alert_Tipologia.ID_area AND ( permessi_tris_nonno.ID_Tipologia = Alert_Tipologia.ID_Tipologia Or permessi_tris_nonno.ID_Tipologia = 0 ) AND permessi_tris_nonno.Autorizzato IN (1,2) AND permessi_tris_nonno.Username = '" & Agro_SQL_SaveText(UtenteUsername) & "' ")
                StrSQL.AppendLine(" and permessi_tris_nonno.Piva IN ")
                StrSQL.AppendLine("    (Select Padre from GerarchiaImprese GerarchiaImpreseTrisNonno WHERE Figlio IN ")
                StrSQL.AppendLine("    (Select Padre from GerarchiaImprese GerarchiaImpreseBisNonno3 WHERE Figlio IN ")
                StrSQL.AppendLine("    (Select Padre from GerarchiaImprese GerarchiaImpreseNonno3 WHERE Figlio IN (Select Padre from GerarchiaImprese GerarchiaImpresePadre3 WHERE Figlio = alert_entita.Piva))) ")
                StrSQL.AppendLine(" ))")
            End If

            If iLivelloGerarchia > 5 Then
                '5. Controllo per Piva Quad-Nonno
                StrSQL.AppendLine(" Left Outer Join CategTipologiaDocumentiXUtenti permessi_Quad_nonno ON ( permessi_Quad_nonno.PivaSuperUser = Alert_Tipologia.PivaSuperUser AND permessi_Quad_nonno.ID_Categoria = Alert_Tipologia.ID_area AND ( permessi_Quad_nonno.ID_Tipologia = Alert_Tipologia.ID_Tipologia Or permessi_Quad_nonno.ID_Tipologia = 0 ) AND permessi_Quad_nonno.Autorizzato IN (1,2) AND permessi_Quad_nonno.Username = '" & UtenteUsername & "' ")
                StrSQL.AppendLine(" and permessi_Quad_nonno.Piva IN ")
                StrSQL.AppendLine("    (Select Padre from GerarchiaImprese GerarchiaImpreseQuadNonno WHERE Figlio IN ")
                StrSQL.AppendLine("    (Select Padre from GerarchiaImprese GerarchiaImpreseTrisNonno4 WHERE Figlio IN ")
                StrSQL.AppendLine("    (Select Padre from GerarchiaImprese GerarchiaImpreseBisNonno4 WHERE Figlio IN ")
                StrSQL.AppendLine("    (Select Padre from GerarchiaImprese GerarchiaImpreseNonno4 WHERE Figlio IN (Select Padre from GerarchiaImprese GerarchiaImpresePadre4 WHERE Figlio = alert_entita.Piva))) ")
                StrSQL.AppendLine(" )))")
            End If

        End If





        StrSQL.AppendLine(" WHERE Alert_Tipologia.PivaSuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")

        If bPermessixUtente Then
            'Condizione almeno un autorizzazione valida
            StrSQL.AppendLine(" AND ((isnull(permessi.Autorizzato, 0)  ")

            If iLivelloGerarchia > 1 Then
                StrSQL.AppendLine(" + isnull(permessi_padre.Autorizzato, 0) ")
            End If

            If iLivelloGerarchia > 2 Then
                StrSQL.AppendLine(" + isnull(permessi_nonno.Autorizzato, 0) ")
            End If

            If iLivelloGerarchia > 3 Then
                StrSQL.AppendLine(" + isnull(permessi_bis_nonno.Autorizzato, 0) ")
            End If

            If iLivelloGerarchia > 4 Then
                StrSQL.AppendLine(" + isnull(permessi_tris_nonno.Autorizzato, 0) ")
            End If

            If iLivelloGerarchia > 5 Then
                StrSQL.AppendLine(" + isnull(permessi_quad_nonno.Autorizzato, 0) ")
            End If

            StrSQL.AppendLine(" ) > 0) ")
        End If





        If Tutti1_soloInScadenza2 = 2 Then
            StrSQL.AppendLine(" AND Alert_Elenco.Data_Scadenza- GETDATE() < Alert_Tipologia_X_Utente.preavviso ")
        End If

        If Trim(Filtro_Area) <> "" Then
            StrSQL.AppendLine(" AND Alert_Area.ID_Area IN (" & Agro_SQL_Save_Clausola_IN(Filtro_Area) & ") ")
        End If

        If Trim(Filtro_Tipologia) <> "" AndAlso Trim(Filtro_Tipologia) <> "0" Then
            StrSQL.AppendLine(" AND Alert_Elenco.ID_Tipologia IN (" & Agro_SQL_Save_Clausola_IN(Filtro_Tipologia) & ") ")
        End If

        If Piva <> "" Then
            StrSQL.AppendLine(" AND Alert_Entita.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
        End If

        'Anna aggiunti nuovi componenti al filtro ricerca
        If Stato_Validazione <> -10 Then
            StrSQL.AppendLine(" AND Allegati_Documenti.Validazione_Flag = " & Agro_SQL_SaveNum(Stato_Validazione) & " ")
        End If

        If Inizio_Upload <> "" And IsDate(Inizio_Upload) Then
            StrSQL.AppendLine(" AND CAST(Allegati_Documenti.Data_Upload AS DATETIME) >= CAST( '" & Agro_SQL_SaveText(Inizio_Upload) & "' AS DATETIME) ")
        End If

        If Fine_Upload <> "" And IsDate(Fine_Upload) Then
            StrSQL.AppendLine(" AND CAST(Allegati_Documenti.Data_Upload AS DATETIME) <= CAST( '" & Agro_SQL_SaveText(Fine_Upload) & "' AS DATETIME) ")
        End If

        If Utente_Upload <> 0 Then
            StrSQL.AppendLine(" AND Allegati_Documenti.Username_Upload = '" & Agro_SQL_SaveText(objParametri_Utenti.UtenteUsername) & "' ")
        End If



        If Not Flag_SuperUSer AndAlso FiltroVisibilitaUtentiDaUtilizzare = -1 Then

            '----------------------------------------------------------------
            '--- Filtro associato all'utente 
            '----------------------------------------------------------------
            Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
            Dim UtenteProfiloImpreseSql As String = ""

            Dim DtImpreseVisibili As DataTable = objUtentiVisibilita.Leggi(enum_TipoEntita.Impresa, "", "", objParametri_Server)
            If Not IsNothing(DtImpreseVisibili) AndAlso DtImpreseVisibili.Rows.Count > 0 Then
                For Each dr In DtImpreseVisibili.Rows
                    UtenteProfiloImpreseSql &= "'" & dr.Item("Piva") & "',"
                Next
                StrSQL.AppendLine(" AND Imprese.piva IN (" & Agro_SQL_Save_Clausola_IN(UtenteProfiloImpreseSql.TrimEnd(","), True) & ") ")
            End If
            '  End If

        End If

        If Not Flag_SuperUSer AndAlso FiltroVisibilitaUtentiDaUtilizzare = enum_Area_Visibilita.UMA AndAlso Not VisibilitaTotale AndAlso (FiltroUtente OrElse FiltroGruppoUtente) Then
            StrSQL.AppendLine(" AND (NOT visibilita.Piva_Azienda IS NULL) ")
        End If


        If Attivo = True Then
            'StrSQL.AppendLine(" AND (Alert_Tipologia_X_Utente.Attivo= " & Agro_SQL_SaveNum(Attivo) & " OR Alert_Tipologia_X_Utente.Attivo IS NULL) ")
            StrSQL.AppendLine(" AND ID_Elenco not IN (Select ID_Elenco from Alert_Elenco_x_Utente where Alert_Elenco_x_Utente.ID_Elenco = ID_Elenco and non_mostrare = -1 AND Alert_Elenco_x_Utente.Username ='" & objParametri_Server.UsernameOperazione & "') ")
        End If

        If ChkDocumento <> -1 Then

            Select Case ChkDocumento

                Case 1 'Documenti

                    StrSQL.AppendLine(" AND Alert_Entita.ChkDocumento IN (1,2) ")

                Case 0 'Scadenza 

                    StrSQL.AppendLine(" AND Alert_Entita.ChkDocumento IN (0,2) ")

                    'StrSQL.AppendLine(" AND ( Alert_Entita.ChkDocumento = " & Agro_SQL_SaveNum(ChkDocumento) & " Or Convert(datetime, Alert_Elenco.Data_Scadenza, 120) < " & Agro_SQL_SaveDate("31/12/2100") & ") ")

                    'Anna 27/04/22: filtro Validità spostato fuori dal select, serve anche per i documenti
                    'If IsDate(Validita_Inizio) Then
                    '  StrSQL.AppendLine(" AND Convert(datetime, Alert_Elenco.Data_Scadenza, 120) >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
                    'End If

                    'If IsDate(Validita_Fine) Then
                    '  StrSQL.AppendLine(" AND Convert(datetime, Alert_Elenco.Data_Scadenza, 120) <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                    'End If

            End Select

            'Anna 27/04/22: filtro Validità spostato fuori dal select, serve anche per i documenti
            If IsDate(Validita_Inizio) Then
                StrSQL.AppendLine(" AND Convert(datetime, Alert_Elenco.Data_Scadenza, 120) >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            End If

            If IsDate(Validita_Fine) Then
                StrSQL.AppendLine(" AND Convert(datetime, Alert_Elenco.Data_Scadenza, 120) <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            End If

        End If

        If ChkSoloAttive = 1 Then

            StrSQL.Append(" AND Exists (Select * From Reg_Impianti WHERE Reg_Impianti.Piva = Alert_Entita.Piva ")
            StrSQL.Append("                    AND Reg_Impianti.Validita_Inizio <= " & Agro_SQL_SaveDate(Now) & " ")
            StrSQL.Append("                    AND Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(Now) & ") ")

        End If



        If Trim(Indici) <> "" Then
            If IsNumeric(Filtro_Area) Then
                Select Case CInt(Filtro_Area)
                    Case 110 'Trasporti Coprob

                        Dim Arrayp = Split(Indici & "|", "|")
                        If UBound(Arrayp) > 0 Then
                            For i = 0 To UBound(Arrayp) - 1
                                Dim Arrayk = Split(Arrayp(i), "-")
                                If Trim(Arrayk(1)) <> "" Then
                                    StrSQL.AppendLine(" AND Alert_Entita.Id_Alert_Entita In (Select Id_Alert_Entita From Alert_EntitaxIndici Where Id_Indice = " & Arrayk(0) & " And (Elenco_Val = '" & Agro_SQL_SaveText(Arrayk(1)) & "' Or Upper(Valore_Des) = '" & Agro_SQL_SaveText(UCase(Arrayk(1))) & "'))")
                                End If

                            Next
                        End If
                End Select

            End If

        End If



        If ChkSoloNonStorico = 0 Then
            StrSQL.AppendLine(" AND Alert_Entita.ChkStorico = 0 ")
        ElseIf ChkSoloNonStorico = 1 Then
            StrSQL.AppendLine(" AND Alert_Entita.ChkStorico = 1 ")
        End If

        '--------------------------------------------------------------------------
        If xFiltroAggiuntivo <> "" Then
            StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri_Server))
        End If
        '--------------------------------------------------------------------------
        Select Case objParametri_Server.FlagVisibilita
            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                StrSQL.AppendLine(" AND  Alert_Elenco.Inviato >=0 ")
            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                StrSQL.AppendLine(" AND  Alert_Elenco.Inviato = -1 ")
            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                '...................................
            Case Else
                Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
        End Select
        '--------------------------------------------------------------------------

    End Sub

    Public Function Controlla_Se_Utilizzato(ByVal ID_Tipologia As Integer,
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                   ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Elenco_r.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT *  ")
            StrSQL.Append(" FROM  Alert_Elenco ")
            StrSQL.Append(" WHERE 1= 1 ")

            StrSQL.Append(" AND ID_Tipologia = " & Agro_SQL_SaveNum(ID_Tipologia) & " ")

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        If DT.Rows.Count = 0 Then
            Return False
        Else
            Return True
        End If

    End Function





    Public Function Leggi_x_Passaggio(ByVal ID_Elenco As Integer,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                     ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Elenco_r.Leggi_x_Passaggio()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT Alert_Elenco.ID_Elenco, Alert_Elenco.ID_Tipologia, Alert_Tipologia.ID_Area, Alert_Entita.*  ")
            StrSQL.Append(" FROM    Alert_Elenco ")
            StrSQL.Append("         INNER JOIN Alert_Tipologia ON Alert_Elenco.ID_Tipologia = Alert_Tipologia.ID_Tipologia ")
            StrSQL.Append("         INNER JOIN Alert_Entita ON Alert_Elenco.PivaSuperUser = Alert_Entita.PivaSuperUser AND Alert_Elenco.ID_Alert_Entita = Alert_Entita.ID_Alert_Entita ")

            StrSQL.Append(" WHERE 1= 1 ")


            If ID_Elenco <> 0 Then
                StrSQL.Append(" AND ID_Elenco = " & Agro_SQL_SaveNum(ID_Elenco) & " ")
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




    Public Function Leggi_x_QDC(ByVal Piva As String,
                                ByVal Id_Tipologia As Integer,
                                ByVal Id_Indice_Veg_Cod As Integer,
                                ByVal Veg_Cod As Integer,
                                ByVal Id_Indice_Anno As Integer,
                                ByVal Anno As Integer,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Elenco_r.Leggi_x_QDC()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT alert_entita.ID_Alert_Entita, Alert_Elenco.Id_Elenco, alert_entita.Allegati_Documenti_Cod, Allegati_Documenti.Data_Upload from alert_entita, alert_elenco, Allegati_Documenti  ")
            StrSQL.Append(" WHere  Alert_Entita.PivaSuperUser      = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
            StrSQL.Append(" And    Alert_Elenco.PivaSuperUser = Alert_Entita.PivaSuperUser ")
            StrSQL.Append(" And    Alert_Entita.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            StrSQL.Append(" And    Alert_Entita.ID_Alert_Entita = alert_elenco.id_alert_entita ")
            StrSQL.Append(" And    Alert_Elenco.ID_Tipologia = " & Agro_SQL_SaveNum(Id_Tipologia) & " ")
            StrSQL.Append(" And    Alert_entita.Allegati_Documenti_Cod = Allegati_Documenti.Allegati_Documenti_Cod ")

            StrSQL.Append(" And alert_entita.id_alert_entita in (select id_alert_entita from alert_entitaxindici where id_indice = " & Id_Indice_Veg_Cod & " And Elenco_Val = '" & Agro_SQL_SaveText(Veg_Cod) & "') ")
            StrSQL.Append(" And alert_entita.id_alert_entita in (select id_alert_entita from alert_entitaxindici where  id_indice = " & Id_Indice_Anno & " And Valore_Des = '" & Agro_SQL_SaveText(Anno) & "') ")




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





    Public Function Leggi_x_GlobalGAP(ByVal Piva As String,
                                      ByVal Id_Tipologia As Integer,
                                      ByVal Id_Indice_Codice As Integer,
                                      ByVal Codice As String,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                      ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Elenco_r.Leggi_x_GlobalGAP()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT alert_entita.ID_Alert_Entita, Alert_Elenco.Id_Elenco, alert_entita.Allegati_Documenti_Cod, Allegati_Documenti.Data_Upload, allegati_documenti.Allegati_Documenti_NomeFile from alert_entita, alert_elenco, Allegati_Documenti  ")
            StrSQL.Append(" WHere  Alert_Entita.PivaSuperUser      = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
            StrSQL.Append(" And    Alert_Elenco.PivaSuperUser = Alert_Entita.PivaSuperUser ")
            StrSQL.Append(" And    Alert_Entita.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            StrSQL.Append(" And    Alert_Entita.ID_Alert_Entita = alert_elenco.id_alert_entita ")
            StrSQL.Append(" And    Alert_Elenco.ID_Tipologia = " & Agro_SQL_SaveNum(Id_Tipologia) & " ")
            StrSQL.Append(" And    Alert_entita.Allegati_Documenti_Cod = Allegati_Documenti.Allegati_Documenti_Cod ")

            StrSQL.Append(" And alert_entita.id_alert_entita in (select id_alert_entita from alert_entitaxindici where  id_indice = " & Id_Indice_Codice & " And Valore_Des = '" & Agro_SQL_SaveText(Codice) & "') ")

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




    Public Function Leggi_x_GlobalGAP_VerificaCancellazione(ByVal Piva As String,
                                                            ByVal Id_Tipologia As Integer,
                                                            ByVal Id_Indice_Codice As Integer,
                                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Elenco_r.Leggi_x_GlobalGAP_VerificaCancellazione()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT Distinct Alert_Elenco.Id_Elenco From alert_entita, alert_entitaxindici, alert_elenco, Allegati_Documenti  ")
            StrSQL.Append(" WHere  Alert_Entita.PivaSuperUser      = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
            StrSQL.Append(" And    Alert_Elenco.PivaSuperUser = Alert_Entita.PivaSuperUser ")
            StrSQL.Append(" And    Alert_Entita.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            StrSQL.Append(" And    Alert_Entita.ID_Alert_Entita = alert_elenco.id_alert_entita ")
            StrSQL.Append(" And    Alert_Elenco.ID_Tipologia = " & Agro_SQL_SaveNum(Id_Tipologia) & " ")
            StrSQL.Append(" And    Alert_entita.Allegati_Documenti_Cod = Allegati_Documenti.Allegati_Documenti_Cod ")

            StrSQL.Append(" And     alert_entitaxindici.id_alert_entita = alert_entita.id_alert_entita ")
            StrSQL.Append(" And     alert_entitaxindici.id_indice = " & Id_Indice_Codice & " ")

            StrSQL.Append(" And     Not Exists (Select * From  [Audit] Where alert_entitaxindici.Valore_Des = convert(varchar(50) , [Audit].Audit_Cod) And  [Audit].Piva = '" & Agro_SQL_SaveText(Piva) & "') ")

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


    Public Function Leggi_ElencoAllegati(ByVal CatCod As Integer,
                                            ByVal xFiltroExistsEntita As String,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Elenco_r.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM Allegati_Documenti  ")
            StrSQL.Append(" WHERE 1=1 ")

            If CatCod <> 0 Then
                StrSQL.Append(" AND Allegati_Documenti_CatCod = " & Agro_SQL_SaveNum(CatCod))
            End If

            '--------------------------------------------------------------------------
            If xFiltroExistsEntita <> "" Then
                StrSQL.Append(" AND EXISTS  ")
                StrSQL.Append(" (SELECT * from Alert_Entita ")
                StrSQL.Append(" WHERE Allegati_Documenti.Allegati_Documenti_Cod = Alert_Entita.Allegati_Documenti_Cod  ")
                StrSQL.Append(" AND " & xFiltroExistsEntita)
                StrSQL.Append(" )")

            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Allegati_Documenti.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Allegati_Documenti.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Validita_Inizio DESC ")
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

    Public Function HasAttachment(
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
        Optional xFiltroAggiuntivo As String = "",
        optional piva As String = "",
        optional idAgenda As Integer = 0,
        optional macCod As Integer = 0,
        optional ricettaCod As Integer = 0,
        optional analisiTestataCod As Integer = 0
    ) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Elenco_r.Leggi()"
        Dim StrSQL As New Text.StringBuilder With {.Length = 0}
        Try
            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM Alert_Elenco ")
            StrSQL.AppendLine(" INNER JOIN Alert_Tipologia ON Alert_Elenco.ID_Tipologia = Alert_Tipologia.ID_Tipologia ")
            StrSQL.AppendLine(" INNER JOIN Alert_Area ON Alert_Tipologia.ID_Area = Alert_Area.ID_Area ")
            StrSQL.AppendLine(" INNER JOIN Alert_Entita ON Alert_Elenco.PivaSuperUser = Alert_Entita.PivaSuperUser AND Alert_Elenco.ID_Alert_Entita = Alert_Entita.ID_Alert_Entita ")
            StrSQL.AppendLine(" LEFT JOIN Allegati_Documenti ON Alert_Entita.allegati_documenti_cod = Allegati_Documenti.allegati_documenti_cod ")

            StrSQL.AppendLine(" WHERE Alert_Tipologia.PivaSuperUser = '" & objParametri.PivaSuperUser & "'")
            StrSQL.AppendLine(" AND ID_Elenco NOT IN (
                SELECT ID_Elenco FROM Alert_Elenco_x_Utente
                WHERE Alert_Elenco_x_Utente.ID_Elenco = ID_Elenco AND non_mostrare = -1 AND Alert_Elenco_x_Utente.Username ='" & objParametri.UsernameOperazione & "') ")
            StrSQL.AppendLine(" AND Alert_Entita.ChkDocumento IN (1,2) ")
            StrSQL.AppendLine(" AND Alert_Entita.ChkStorico = 0 ")
            StrSQL.AppendLine(" AND Alert_Elenco.Inviato >=0 ")

            If Not String.IsNullOrEmpty(piva) Then
                StrSQL.AppendLine(" AND Alert_Entita.Piva = '" & Agro_SQL_SaveText(piva) & "'  ")
            End If
            If idAgenda <> 0 Then
                StrSQL.AppendLine(" AND Alert_Area.ID_Area IN (" & Agro_SQL_SaveNum(enum_ID_Area_Alert.Operazioni_Campagna_QDC) & ")")
                StrSQL.AppendLine(" AND Alert_Entita.TipoEntita_Cod = " & Agro_SQL_SaveNum(enum_TipoEntita.OperazioneDiAgenda))
                StrSQL.AppendLine(" AND Alert_Entita.ID_Agenda = " & Agro_SQL_SaveNum(idAgenda) & " ")
            End If
            If ricettaCod <> 0 Then
                StrSQL.AppendLine(" And Alert_Area.ID_Area IN (" & Agro_SQL_SaveNum(enum_ID_Area_Alert.Operazioni_Campagna_QDC) & ")")
                StrSQL.AppendLine(" AND Alert_Entita.TipoEntita_Cod = " & Agro_SQL_SaveNum(enum_TipoEntita.Ricetta))
                StrSQL.appendline("And Alert_Entita.Ricetta_Operazione_cod = " & Agro_SQL_SaveNum(ricettaCod) & " ")
            End If
            If macCod <> 0 Then
                StrSQL.AppendLine(" And Alert_Area.ID_Area IN (" & Agro_SQL_SaveNum(enum_ID_Area_Alert.Macchine) & ")")
                StrSQL.AppendLine(" And Alert_Entita.TipoEntita_Cod = " & Agro_SQL_SaveNum(enum_TipoEntita.Macchina))
                StrSQL.appendline("AND Alert_Entita.Mac_Cod = " & Agro_SQL_SaveNum(macCod))
            End If
            If analisiTestataCod <> 0 Then
                StrSQL.AppendLine(" AND Alert_Area.ID_Area IN (" & Agro_SQL_SaveNum(enum_ID_Area_Alert.Analisi) & ")")
                StrSQL.AppendLine(" AND Alert_Entita.TipoEntita_Cod = " & Agro_SQL_SaveNum(enum_TipoEntita.AnalisiTerreno))
                StrSQL.appendline("AND Alert_Entita.Analisi_Testata_Cod = " & Agro_SQL_SaveNum(analisiTestataCod))
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Allegati_Documenti.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Allegati_Documenti.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '-------------------------------------------------------------------------- 
            Dim DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            Return DT.Rows.Count > 0
            '-------------------------------------------------------------------------- 
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
        Return False
    End Function

End Class
