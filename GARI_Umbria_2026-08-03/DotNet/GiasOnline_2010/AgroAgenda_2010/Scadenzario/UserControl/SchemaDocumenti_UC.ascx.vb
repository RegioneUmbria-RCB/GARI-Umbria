Imports AgroAgenda_2010.Resources
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports System.Web.Services
Imports AgronicaCoreScadenziario_BIZ

Public Class SchemaDocumenti_UC
    Inherits System.Web.UI.UserControl
    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Public objparametri_server_string, objparametri_utenti_string As String

    Private Sub inizializzoObjParametri()
        '---
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objparametri_server_string = AgronicaCoreDataProvider.Utility.convertOBJparametritoString(objParametri_Server)
        '---
    End Sub


    <WebMethod(EnableSession:=True)>
    Public Shared Function SchemaDocumenti_SalvaGriglia(ByVal righeCancellate As String,
                                                        ByVal righeInserite As String,
                                                        ByVal righeModificate As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            ''' Salva le righe nel DB '''
            Dim biz As New AgronicaCoreScadenziario_BIZ.SchemaDocumenti
            Return biz.SchemaDocumenti_SalvaGriglia(objParametriServer, righeInserite, righeModificate, righeCancellate)
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return r
        End Try
    End Function


    <WebMethod(EnableSession:=True)>
    Protected Sub LeggiDataSchemaDocumenti()
        Dim documentiBiz As New AgronicaCoreScadenziario_BIZ.SchemaDocumenti


        If Not Page.IsPostBack Then
            Dim dt = documentiBiz.AgronicaCoreDataProvider_Leggi(objParametri_Server)
        End If
    End Sub






    <WebMethod(EnableSession:=True)>
    Public Shared Function leggiConfigurazione() As RispostaStandard

        Dim r As New RispostaStandard
        Dim Dt As New DataTable
        Lingua.Gias_InizializzaCultura_DaSession()

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("objParametri_Utenti")

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        Dim ricette_operazioni_W As New AgronicaCoreContabBIZ.Ricette_Operazioni_W

        'ricette_operazioni_W.Test("02961820541", objParametri_Server, objParametri_Utenti)

        r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)
        r.RispostaOK = True
        Return r


    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function SchemaDocumenti_CaricaElenco() As RispostaStandard
        Dim r As New RispostaStandard
        Lingua.Gias_InizializzaCultura_DaSession()

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("objParametri_Utenti")

        Try
            Dim biz As New SchemaDocumenti
            Dim schemaDocumenti As DataTable = biz.SchemaDocumenti_LeggiTabella(objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(schemaDocumenti, Formatting.None, serializerSettings)
            r.RispostaOK = True
            Return r
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return r
        End Try
    End Function

    Public Shared Function CaricaConfigurazioniSchemaDocumenti() As RispostaStandard
        Dim r As New RispostaStandard
        Lingua.Gias_InizializzaCultura_DaSession()

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("objParametri_Utenti")

        Dim biz As New AgronicaCoreScadenziario_BIZ.SchemaDocumenti
        Dim dal As New AgronicaCoreScadenziario.SchemaDocumenti_R

        Dim schemaDocumenti = dal.LeggiSchemaDocumenti(objParametri_Server)

        Dim Dt As New DataTable
        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore



        Dim x As New List(Of Object)

        Dim configurazionSD = (From row In schemaDocumenti.AsEnumerable() Select New With
                                                         {
                                                           .Id_Schema_Template = CInt(row.Item("Id_Schema_Template")),
                                                           .Servizio_Des = If(row.Item("Servizio_Des") Is DBNull.Value, "", row.Item("Servizio_Des")),
                                                           .Stato_Da_Des = If(row.Item("Stato_Da_Des") Is DBNull.Value, "", row.Item("Stato_Da_Des")),
                                                           .Stato_A_Des = If(row.Item("Stato_A_Des") Is DBNull.Value, "", row.Item("Stato_A_Des")),
                                                           .Tipologia_Des = If(row.Item("Tipologia_Des") Is DBNull.Value, "", row.Item("Tipologia_Des")),
                                                           .Ambito_Des = If(row.Item("Ambito_Des") Is DBNull.Value, "", row.Item("Ambito_Des")),
                                                           .Fase_Des = If(row.Item("Fase_Des") Is DBNull.Value, "", row.Item("Fase_Des")),
                                                           .Firmato = If(row.Item("Firmato") Is DBNull.Value, "", row.Item("Firmato")),
                                                           .Obbligatorio = If(row.Item("Obbligatorio") Is DBNull.Value, "", row.Item("Obbligatorio")),
                                                           .Ambito = If(row.Item("Ambito") Is DBNull.Value, 0, CInt(row.Item("Ambito"))),
                                                           .Servizio_Cod = If(row.Item("Servizio_Cod") Is DBNull.Value, 0, CInt(row.Item("Servizio_Cod"))),
                                                           .Stato_Da = If(row.Item("Stato_Da") Is DBNull.Value, 0, CInt(row.Item("Stato_Da"))),
                                                           .Stato_A = If(row.Item("Stato_A") Is DBNull.Value, 0, CInt(row.Item("Stato_A"))),
                                                           .Fase = If(row.Item("Fase") Is DBNull.Value, 0, CInt(row.Item("Fase"))),
                                                           .Ordine = If(row.Item("Ordine") Is DBNull.Value, 0, CInt(row.Item("Ordine"))),
                                                           .Tipologia = If(row.Item("Tipologia") Is DBNull.Value, 0, CInt(row.Item("Tipologia"))),
                                                           .Suffisso_File = If(row.Item("Suffisso_File") Is DBNull.Value, "", row.Item("Suffisso_File")),
                                                           .Descrizione = If(row.Item("Descrizione") Is DBNull.Value, "", row.Item("Descrizione")),
                                                           .Flag_Obbligatorio = If(row.Item("Flag_Obbligatorio") Is DBNull.Value, 0, CInt(row.Item("Flag_Obbligatorio"))),
                                                           .Flag_Firmato_Digit = If(row.Item("Flag_Firmato_Digit") Is DBNull.Value, 0, CInt(row.Item("Flag_Firmato_Digit"))),
                                                           .Nome_Modello = If(row.Item("Nome_Modello") Is DBNull.Value, "", row.Item("Nome_Modello")),
                                                           .Nr_Documenti = If(row.Item("Nr_Documenti") Is DBNull.Value, 0, CInt(row.Item("Nr_Documenti"))),
                                                           .inviato = If(row.Item("inviato") Is DBNull.Value, 0, CInt(row.Item("inviato"))),
                                                           .datainvio = If(row.Item("datainvio") Is DBNull.Value, AGRODATAINIZIO, CDate(row.Item("datainvio"))),
                                                           .Data_Creazione = If(row.Item("Data_Creazione") Is DBNull.Value, DateTime.Now, CDate(row.Item("Data_Creazione"))),
                                                           .Data_Modifica = If(row.Item("Data_Modifica") Is DBNull.Value, DateTime.Now, CDate(row.Item("Data_Modifica"))),
                                                           .Username_Creazione = If(row.Item("Username_Creazione") Is DBNull.Value, objParametri_Server.UtenteUsername, row.Item("Username_Creazione")),
                                                           .Username_modifica = If(row.Item("Username_Modifica") Is DBNull.Value, objParametri_Server.UtenteUsername, row.Item("Username_Modifica")),
                                                           .Validita_Inizio = If(row.Item("Validita_Inizio") Is DBNull.Value, AGRODATAINIZIO, CDate(row.Item("Validita_Inizio"))),
                                                           .Validita_Fine = If(row.Item("Validita_Fine") Is DBNull.Value, AGRODATAFINE, CDate(row.Item("Validita_Fine")))
                                                         }).ToList()


        r.RispostaStringa = JsonConvert.SerializeObject(configurazionSD, Formatting.None, serializerSettings)
        r.RispostaOK = True
        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function SchemaDocumenti_CaricaDropdown(elencoRichiesto As String) As RispostaStandard
        Dim r As New RispostaStandard

        ' ***************************************** Verifiche *****************************************
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        ' ****************************************** Carica l'elenco ******************************************
        Dim dt As DataTable
        Try
            Dim biz As New SchemaDocumenti
            r.RispostaOK = True ' Assumiamo argomento valido

            Select Case elencoRichiesto
                Case "Servizio_Des"
                    dt = biz.Dropdown_Servizi(objParametri_Server)
                Case "Tipologia"
                    dt = biz.Dropdown_Tipologia(objParametri_Server)
                Case "Tipologia"
                    dt = biz.Dropdown_Ambito(objParametri_Server)
                Case "Fase"
                    dt = biz.Dropdown_Fase(objParametri_Server)
                Case "Firmato"
                    dt = biz.Dropdown_Firmato(objParametri_Server)
                Case "Obbligatorio"
                    dt = biz.Dropdown_Obbligatorio(objParametri_Server)
                Case Else
                    r.RispostaOK = False
                    r.Errore = "Argomente non valido: " & elencoRichiesto
            End Select

            ' ****************************************** tutto ok, risposta ******************************************
            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)
            Return r
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return r
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function SchemaDocumenti_CaricaStato(Servizio_Cod As Integer, elencoRichiesto As String) As RispostaStandard
        Dim r As New RispostaStandard

        ' ***************************************** Verifiche *****************************************
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        ' ****************************************** Carica l'elenco ******************************************
        Dim dt As DataTable
        Try
            Dim biz As New SchemaDocumenti
            r.RispostaOK = True ' Assumiamo argomento valido

            Select Case elencoRichiesto
                Case "Stato_Da_Des"
                    dt = biz.Dropdown_Stato_Da(Servizio_Cod, objParametri_Server)
                Case "Stato_A_Des"
                    dt = biz.Dropdown_Stato_A(Servizio_Cod, objParametri_Server)
                Case Else
                    r.RispostaOK = False
                    r.Errore = "Argomente non valido: " & elencoRichiesto
            End Select

            ' ****************************************** tutto ok, risposta ******************************************
            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)
            Return r
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return r
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Servizi_LeggiElencoDropdown() As RispostaStandard
        Dim r As New RispostaStandard

        ' ***************************************** Verifiche *****************************************
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        ' ****************************************** Carica l'elenco ******************************************
        Dim dt As DataTable
        Try
            Dim biz As New SchemaDocumenti

            r.RispostaOK = True ' Assumiamo argomento valido
            dt = biz.Dropdown_Servizi(objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return r
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Stato_Da_LeggiElencoDropdown(Servizio_Cod As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        ' ***************************************** Verifiche *****************************************
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        ' ****************************************** Carica l'elenco ******************************************
        Dim dt As DataTable
        Try
            Dim biz As New SchemaDocumenti

            r.RispostaOK = True ' Assumiamo argomento valido
            dt = biz.Dropdown_Stato_Da(Servizio_Cod, objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return r
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Stato_A_LeggiElencoDropdown(Servizio_Cod As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        ' ***************************************** Verifiche *****************************************
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        ' ****************************************** Carica l'elenco ******************************************
        Dim dt As DataTable
        Try
            Dim biz As New SchemaDocumenti

            r.RispostaOK = True ' Assumiamo argomento valido
            dt = biz.Dropdown_Stato_A(Servizio_Cod, objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return r
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Tipologia_LeggiElencoDropdown() As RispostaStandard
        Dim r As New RispostaStandard

        ' ***************************************** Verifiche *****************************************
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        ' ****************************************** Carica l'elenco ******************************************
        Dim dt As DataTable
        Try
            Dim biz As New SchemaDocumenti

            r.RispostaOK = True ' Assumiamo argomento valido
            dt = biz.Dropdown_Tipologia(objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return r
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Ambito_LeggiElencoDropdown() As RispostaStandard
        Dim r As New RispostaStandard

        ' ***************************************** Verifiche *****************************************
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        ' ****************************************** Carica l'elenco ******************************************
        Dim dt As DataTable
        Try
            Dim biz As New SchemaDocumenti

            r.RispostaOK = True ' Assumiamo argomento valido
            dt = biz.Dropdown_Ambito(objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return r
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Fase_LeggiElencoDropdown() As RispostaStandard
        Dim r As New RispostaStandard

        ' ***************************************** Verifiche *****************************************
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        ' ****************************************** Carica l'elenco ******************************************
        Dim dt As DataTable
        Try
            Dim biz As New SchemaDocumenti

            r.RispostaOK = True ' Assumiamo argomento valido
            dt = biz.Dropdown_Fase(objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return r
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Firmato_LeggiElencoDropdown() As RispostaStandard
        Dim r As New RispostaStandard

        ' ***************************************** Verifiche *****************************************
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        ' ****************************************** Carica l'elenco ******************************************
        Dim dt As DataTable
        Try
            Dim biz As New SchemaDocumenti

            r.RispostaOK = True ' Assumiamo argomento valido
            dt = biz.Dropdown_Firmato(objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return r
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Obbligatorio_LeggiElencoDropdown() As RispostaStandard
        Dim r As New RispostaStandard

        ' ***************************************** Verifiche *****************************************
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        ' ****************************************** Carica l'elenco ******************************************
        Dim dt As DataTable
        Try
            Dim biz As New SchemaDocumenti

            r.RispostaOK = True ' Assumiamo argomento valido
            dt = biz.Dropdown_Obbligatorio(objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return r
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Protected Sub SchemaDocumenti_LeggiTabella()
        Dim schemaDocumenti As New SchemaDocumenti

        If Not Page.IsPostBack Then
            Dim dt = schemaDocumenti.SchemaDocumenti_LeggiTabella(objParametri_Server)
        End If
    End Sub

End Class