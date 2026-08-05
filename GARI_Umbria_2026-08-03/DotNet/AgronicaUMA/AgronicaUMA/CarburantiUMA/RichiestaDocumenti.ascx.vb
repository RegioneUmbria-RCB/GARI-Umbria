Imports AgronicaUMA.Resources
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
Imports AgronicaCoreAnagrafeBIZ
Imports System.Web

Public Class RichiestaDocumenti
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


    '<WebMethod(EnableSession:=True)>
    'Public Shared Function RichiestaDocumenti_SalvaGriglia(ByVal righeCancellate As String,
    '                                                    ByVal righeInserite As String,
    '                                                    ByVal righeModificate As String) As RispostaStandard

    '    Dim r As New RispostaStandard

    '    Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
    '    If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
    '        r.Sessione = False
    '        Return r
    '    End If

    '    Try
    '        ''' Salva le righe nel DB '''
    '        Dim biz As New AgronicaCoreAnagrafeBIZ.RichiestaDocumenti
    '        Return biz.RichiestaDocumenti_SalvaGriglia(objParametriServer, righeInserite, righeModificate, righeCancellate)
    '    Catch ex As Exception
    '        r.RispostaOK = False
    '        r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
    '        Return r
    '    End Try
    'End Function


    <WebMethod(EnableSession:=True)>
    Protected Sub LeggiDataRichiestaDocumenti()
        Dim documentiBiz As New AgronicaCoreScadenziario_BIZ.RichiestaDocumenti


        If Not Page.IsPostBack Then
            Dim dt = documentiBiz.AgronicaCoreDataProvider_Leggi(objParametri_Server)
        End If
    End Sub



    <WebMethod(EnableSession:=True)>
    Public Shared Function leggiConfigurazione() As RispostaStandard

        Dim r As New RispostaStandard
        Dim Dt As New DataTable

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        Dim ricette_operazioni_W As New AgronicaCoreContabBIZ.Ricette_Operazioni_W

        'ricette_operazioni_W.Test("02961820541", objParametri_Server, objParametri_Utenti)

        r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)
        r.RispostaOK = True
        Return r


    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function RichiestaDocumenti_CaricaElenco(ByVal piva As String, ByVal Richiesta_Cod As Integer, ByVal permesso_richiesta As Boolean, ByVal permesso_rendicontazione As Boolean, ByVal anno As Integer, ByVal QS_Avanzamento As Integer, ByVal QS_Type As Integer, ByVal permesso_approvazione_richiesta As Boolean, ByVal permesso_approvazione_rendicontazione As Boolean) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Try
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim biz As New AgronicaCoreScadenziario_BIZ.RichiestaDocumenti
            Dim richiestaDocumenti As DataTable = biz.RichiestaDocumenti_LeggiTabella(objParametri_Server, piva, Richiesta_Cod, objParametri_Utenti)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(richiestaDocumenti, Formatting.None, serializerSettings)
            r.RispostaOK = True
            Return r
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return r
        End Try
    End Function

    Public Shared Function CaricaConfigurazioniRichiestaDocumenti(ByVal piva As String, ByVal Richiesta_Cod As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Dim biz As New AgronicaCoreScadenziario_BIZ.RichiestaDocumenti
        Dim dal As New AgronicaCoreScadenziario.RichiestaDocumenti_R

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore


        Dim richiestaDocumenti = dal.LeggiRichiestaDocumenti(objParametri_Server, piva, Richiesta_Cod, objParametri_Utenti)

        Dim Dt As New DataTable



        Dim x As New List(Of Object)

        Dim configurazionSD = (From row In richiestaDocumenti.AsEnumerable() Select New With
                                                             {
                                                               .Id_Schema_Template = CInt(row.Item("Id_Schema_Template")),
                                                               .Nr_Documenti_Presenti = If(row.Item("Nr_Documenti_Presenti") Is DBNull.Value, 0, CInt(row.Item("Nr_Documenti_Presenti"))),
                                                               .Richiesta_Cod = If(row.Item("Richiesta_Cod") Is DBNull.Value, 0, CInt(row.Item("Richiesta_Cod"))),
                                                               .Pratica_Cod = If(row.Item("Pratica_Cod") Is DBNull.Value, 0, CInt(row.Item("Pratica_Cod"))),
                                                               .Tipologia_Des = If(row.Item("Tipologia_Des") Is DBNull.Value, "", row.Item("Tipologia_Des")),
                                                               .Ambito_Des = If(row.Item("Ambito_Des") Is DBNull.Value, "", row.Item("Ambito_Des")),
                                                               .Fase_Des = If(row.Item("Fase_Des") Is DBNull.Value, "", row.Item("Fase_Des")),
                                                               .Firmato = If(row.Item("Firmato") Is DBNull.Value, "", row.Item("Firmato")),
                                                               .Obbligatorio = If(row.Item("Obbligatorio") Is DBNull.Value, "", row.Item("Obbligatorio")),
                                                               .Ambito = If(row.Item("Ambito") Is DBNull.Value, 0, CInt(row.Item("Ambito"))),
                                                               .Servizio_Cod = If(row.Item("Servizio_Cod") Is DBNull.Value, 0, CInt(row.Item("Servizio_Cod"))),
                                                               .Ordine = If(row.Item("Ordine") Is DBNull.Value, 0, CInt(row.Item("Ordine"))),
                                                               .Tipologia = If(row.Item("ID_Tipologia") Is DBNull.Value, 0, CInt(row.Item("ID_Tipologia"))),
                                                               .Suffisso_File = If(row.Item("Suffisso_File") Is DBNull.Value, "", row.Item("Suffisso_File")),
                                                               .Descrizione = If(row.Item("Descrizione") Is DBNull.Value, "", row.Item("Descrizione")),
                                                               .Flag_Obbligatorio = If(row.Item("Flag_Obbligatorio") Is DBNull.Value, 0, CInt(row.Item("Flag_Obbligatorio"))),
                                                               .Flag_Firmato_Digit = If(row.Item("Flag_Firmato_Digit") Is DBNull.Value, 0, CInt(row.Item("Flag_Firmato_Digit"))),
                                                               .Nr_Documenti = If(row.Item("Nr_Documenti") Is DBNull.Value, 0, CInt(row.Item("Nr_Documenti"))),
                                                               .Autorizzato = If(row.Item("Autorizzato") Is DBNull.Value, 0, CInt(row.Item("Autorizzato")))
                                                             }).ToList()

        r.RispostaStringa = JsonConvert.SerializeObject(configurazionSD, Formatting.None, serializerSettings)
        r.RispostaOK = True

        'Else
        'r.RispostaStringa = JsonConvert.SerializeObject(New DataTable, Formatting.None, serializerSettings)
        'r.RispostaOK = True
        'End If

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function RichiestaDocumenti_CaricaStato(Servizio_Cod As Integer, elencoRichiesto As String) As RispostaStandard
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
        Dim dt As New DataTable
        Try
            Dim biz As New RichiestaDocumenti
            r.RispostaOK = True ' Assumiamo argomento valido

            'Select Case elencoRichiesto
            '    Case "Stato_Da_Des"
            '        dt = biz.Dropdown_Stato_Da(Servizio_Cod, objParametri_Server)
            '    Case "Stato_A_Des"
            '        dt = biz.Dropdown_Stato_A(Servizio_Cod, objParametri_Server)
            '    Case Else
            '        r.RispostaOK = False
            '        r.Errore = "Argomente non valido: " & elencoRichiesto
            'End Select

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
    Protected Sub RichiestaDocumenti_LeggiTabella(ByVal piva As String, ByVal Richiesta_Cod As Integer)
        Dim richiestaDocumenti As New AgronicaCoreScadenziario_BIZ.RichiestaDocumenti

        If Not Page.IsPostBack Then
            Dim dt = richiestaDocumenti.RichiestaDocumenti_LeggiTabella(objParametri_Server, piva, Richiesta_Cod, objParametri_Utenti)
        End If
    End Sub

    <WebMethod(EnableSession:=True)>
    Protected Sub RichiestaDocumenti_EstraiFase(ByVal piva As String, ByVal Richiesta_Cod As Integer)
        Dim richiestaDocumenti As New AgronicaCoreScadenziario_BIZ.RichiestaDocumenti

        If Not Page.IsPostBack Then
            Dim fase = richiestaDocumenti.RichiestaDocumenti_EstraiFase(objParametri_Server, piva, Richiesta_Cod, objParametri_Utenti)
        End If
    End Sub

End Class