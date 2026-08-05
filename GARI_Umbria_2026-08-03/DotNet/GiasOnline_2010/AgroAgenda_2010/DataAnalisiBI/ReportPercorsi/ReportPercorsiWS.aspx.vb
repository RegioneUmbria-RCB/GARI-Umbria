Imports System.Web.Services
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreGisDAL
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreUtentiDAL

Public Class ReportPercorsiWS
    Inherits System.Web.UI.Page

    Friend Class APP_GIS_1
        Public Property N_Telefono As String
        Public Property Descrizione As String
        Public Property DataOra As Date
        Public Property GisTxt As String

    End Class

    <WebMethod(EnableSession:=True)>
    Public Shared Function RedirGIS(ByVal ParametriFiltroXSessione As String, ByVal tipo As Integer, DataInizio As Date, DataFine As Date) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If


        Try

            Dim permessi As New PermessiUtente
            dim visibilitaCompleta As Boolean = 
                permessi.getPermesso(enum_Security_Attivita.ReportPercorsi_Amministrtore).Scrittura

            Dim xLeggi As New AgronicaCoreGisDAL.APP_EntrateUsciteCoordinate_R
            Dim dtRep As DataTable
            Select Case tipo
                Case 2 'ultima posizione nuova versione
                    dtRep = xLeggi.LeggiXPuntiGpsUltimaPosizione(Not visibilitaCompleta,
                        AGRODATAINIZIO, AGRODATAFINE, " IDTransazione in ( " & ParametriFiltroXSessione & " )", "", objParametri_Server, objParametri_Utenti)

                Case 3 ' posizioni rilevate
                    dtRep = xLeggi.LeggiXPuntiGPS(Not visibilitaCompleta,
                        AGRODATAINIZIO, AGRODATAFINE, " IDTransazione in ( " & ParametriFiltroXSessione & " )", "", objParametri_Server, objParametri_Utenti)
                Case Else

            End Select

            If tipo = 2 Or tipo = 3 Then


                Dim listaRep As List(Of APP_GIS_1) = (
                    From a In dtRep.AsEnumerable
                    Select New APP_GIS_1 With {
                        .N_Telefono = a("N_Telefono"),
                        .Descrizione = a("Descrizione"),
                        .DataOra = a("DataOra"),
                        .GisTxt = "POINT (" & a("longitudine").ToString.Replace(",", ".") & " " & a("latitudine").ToString.Replace(",", ".") & ")"
                        }).ToList()

                Dim rSer As String =
                    Newtonsoft.Json.JsonConvert.SerializeObject(listaRep)

                'serializzare...
                HttpContext.Current.Session("ParametriFiltroPercorsiGiasAppXSessione") = rSer


            Else

                HttpContext.Current.Session("ParametriFiltroPercorsiXSessione") = ParametriFiltroXSessione

            End If

            HttpContext.Current.Session("ParametriFiltroPercorsiXSessione_tipo") = tipo
            HttpContext.Current.Session("ParametriFiltroPercorsiXSessione_DataInizio") = DataInizio
            HttpContext.Current.Session("ParametriFiltroPercorsiXSessione_DataFine") = DataFine

            r.RispostaOK = True
            r.RispostaStringa = "ok"

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function ReportGPFDettaglio(ByVal FiltroTestata As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If


        Try

            'Inserire il codice QUI..
            Dim xLeggi As New PrecisionFarming
            Dim dtRep As DataTable =
                xLeggi.ReportGiasPathFinderDettaglio(AGRODATAINIZIO, AGRODATAFINE, FiltroTestata, "", objParametri_Server, objParametri_Utenti)


            r.RispostaOK = True
            r.RispostaStringa = getJsonKendoDettaglio(dtRep)

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function ReportGPF(ByVal DataInizio As Date, ByVal DataFine As Date, ByVal UtenteCorrente As Boolean) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If


        Try

            'Decidere su chiamata per vedere solo i percorsi dell'utente loggato ..: " & objParametri_Server.UtenteUsername & "
            Dim xLeggi As New PrecisionFarming
            Dim dtRep As DataTable =
                xLeggi.ReportGiasPathFinder(DataInizio, DataFine, "", objParametri_Server, objParametri_Utenti)


            r.RispostaOK = True
            r.RispostaStringa = getJsonKendo(dtRep, enum_iMotion_tipoPercorsoGIS.percorso)

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function PosizioniRilevate(ByVal DataInizio As Date, ByVal DataFine As Date, ByVal UtenteCorrente As Boolean) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            'leggere i permessi ed ottenere posizione solo su utente loggato
            'o su tutti gli utenti (parametro " & objParametri_Server.UtenteUsername & ")
            
            Dim permessi As New PermessiUtente
            dim visibilitaCompleta As Boolean = 
                permessi.getPermesso(enum_Security_Attivita.ReportPercorsi_Amministrtore).Scrittura

            'anche se vengono passate date, la funzione si chiama su agrodatainizio,fine
            Dim xLeggi As New AgronicaCoreGisDAL.APP_EntrateUsciteCoordinate_R
            Dim dtRep As DataTable =
                xLeggi.LeggiXPuntiGPS(Not visibilitaCompleta,
                    DataInizio, DataFine, "", " g.DataOraRilevata desc", objParametri_Server, objParametri_Utenti)


            r.RispostaOK = True
            r.RispostaStringa = getJsonKendo(dtRep, enum_iMotion_tipoPercorsoGIS.UltimaPosizione)


        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function UltimaPosizione(ByVal DataInizio As Date, ByVal DataFine As Date, ByVal UtenteCorrente As Boolean) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If


        Try

            'leggere i permessi ed ottenere posizione solo su utente loggato
            'o su tutti gli utenti (parametro " & objParametri_Server.UtenteUsername & ")

            Dim permessi As New PermessiUtente
            dim visibilitaCompleta As Boolean = 
                permessi.getPermesso(enum_Security_Attivita.ReportPercorsi_Amministrtore).Scrittura

            
            'anche se vengono passate date, la funzione si chiama su agrodatainizio,fine
            Dim xLeggi As New APP_EntrateUsciteCoordinate_R
            Dim dtRep As DataTable =
                xLeggi.LeggiXPuntiGpsUltimaPosizione( not visibilitaCompleta,
                    DataInizio, DataFine, "", "", objParametri_Server, objParametri_Utenti)


            r.RispostaOK = True
            r.RispostaStringa = getJsonKendo(dtRep, enum_iMotion_tipoPercorsoGIS.UltimaPosizione)

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function ReportPercorsi() As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If


        Try

            'Inserire il codice QUI..
            Dim xLeggi As New PrecisionFarming
            Dim dtRep As DataTable =
                xLeggi.ReportPercorsi_Raccolte(objParametri_Server)


            r.RispostaOK = True
            r.RispostaStringa = getJsonWATable(dtRep)

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r
    End Function
    Private Shared Function getJsonKendoDettaglio(ByVal dt As DataTable) As String
        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)
        Dim c As ColonneNome


        c = New ColonneNome("id", "id", "string")
        c._hidden = True
        l.Add(c)


        c = New ColonneNome("N_telefono", "N_telefono", "string")
        l.Add(c)

        c = New ColonneNome("Descrizione", "Descrizione", "string")
        l.Add(c)

        c = New ColonneNome("Data", "Data", "date")
        l.Add(c)

        c = New ColonneNome("DataOra", "Data/Ora", "date")
        c._FormatoParticolare = "#=kendo.toString(DataOra, 'dd/MM/yyyy HH:mm')#"
        l.Add(c)

        c = New ColonneNome("Velocita", "Velocita (km/h)", "number")
        c._formatNr = "n0"
        l.Add(c)


        c = New ColonneNome("Quota", "Quota (m)", "number")
        c._formatNr = "n0"
        l.Add(c)


        c = New ColonneNome("Direzione", "Direzione (gradi)", "number")
        c._formatNr = "n0"
        l.Add(c)

        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        js.Editabile_Deafault = True
        Dim risp As String =
            js.JSON_DataTable_Kendo(
                dt,
                l,
                AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True,
                tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaDiscesa
           )


        Return risp

    End Function

    Private Shared Function getJsonKendo(ByVal dt As DataTable, enumTipo As enum_iMotion_tipoPercorsoGIS) As String
        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)
        Dim c As ColonneNome


        c = New ColonneNome("id", "id", "string")
        c._hidden = True
        l.Add(c)


        c = New ColonneNome("N_telefono", "N_telefono", "string")
        l.Add(c)

        c = New ColonneNome("Descrizione", "Tecnico", "string")
        l.Add(c)


        Select Case enumTipo
            Case enum_iMotion_tipoPercorsoGIS.percorso

                c = New ColonneNome("Data", "Data", "date")
                l.Add(c)

                c = New ColonneNome("DataOraPartenza", "Partenza", "date")
                c._FormatoParticolare = "#=kendo.toString(DataOraPartenza, 'dd/MM/yyyy HH:mm')#"
                l.Add(c)

                c = New ColonneNome("DataOraArrivo", "Arrivo", "date")
                c._FormatoParticolare = "#=kendo.toString(DataOraArrivo, 'dd/MM/yyyy HH:mm')#"
                l.Add(c)

                c = New ColonneNome("Durata", "Durata (Minuti)", "number")
                c._formatNr = "n0"
                l.Add(c)

            Case enum_iMotion_tipoPercorsoGIS.UltimaPosizione

                c = New ColonneNome("DataOra", "Data/Ora", "date")
                c._FormatoParticolare = "#=kendo.toString(DataOra, 'dd/MM/yyyy HH:mm')#"
                l.Add(c)


                c = New ColonneNome("Latitudine", "Latitudine", "string")
                l.Add(c)

                c = New ColonneNome("Longitudine", "Longitudine", "string")
                l.Add(c)


                c = New ColonneNome("Velocita", "Velocita (km/h)", "number")
                c._formatNr = "n0"
                l.Add(c)


                c = New ColonneNome("Quota", "Quota (m)", "number")
                c._formatNr = "n0"
                l.Add(c)


                c = New ColonneNome("Direzione", "Direzione (gradi)", "number")
                c._formatNr = "n0"
                l.Add(c)


        End Select



        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        js.Editabile_Deafault = True
        Dim risp As String =
            js.JSON_DataTable_Kendo(
                dt,
                l,
                AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True,
                tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaDiscesa
           )


        Return risp

    End Function

    Private Shared Function getJsonWATable(ByVal dt As DataTable) As String

        Dim l As New List(Of ColonneNome)

        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        l = JSON_DataTable.getListaColonneFromDT(dt)

        Dim rval As String =
        js.JSON_DataTable(dt, l, True)

        Return rval

    End Function

End Class