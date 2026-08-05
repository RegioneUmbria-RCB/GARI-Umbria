Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreVarieBIZ

Public Class cfgRilievi_WS
    Inherits System.Web.UI.Page

    <WebMethod(EnableSession:=True)>
    Public Shared Function cfgRilieviAnagAggiornaDatiSrv(ByVal righeInserite As String, ByVal righeModificate As String, ByVal righeCancellate As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If


        Try

            'Inserire il codice QUI..


            Dim xAggiorna As New AgronicaCoreMetaSchemaBIZ.MisuraXAvversita_Anagrafiche_W
            Dim rval As String = xAggiorna.Aggiorna_MisuraXAvversita_Anagrafiche(righeInserite, righeModificate, righeCancellate, objParametri_Server)



            r.RispostaOK = True
            r.RispostaStringa = rval

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function kReadCfgRilievi_dati(ByVal righeInserite As String, ByVal righeModificate As String, ByVal righeCancellate As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If


        Try

            'Inserire il codice QUI..


            Dim xAggiorna As New AgronicaCoreMetaSchemaBIZ.MisuraXAvversita_W
            Dim rval As String = xAggiorna.Aggiorna_MisuraXAvversita(righeInserite, righeModificate, righeCancellate, objParametri_Server)



            r.RispostaOK = True
            r.RispostaStringa = rval

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LetturaDatiCfgRilieviKendo() As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If


        Try

            'Inserire il codice QUI..

            Dim dt As DataTable

            Dim misuraLeggi As New AgronicaCoreMetaSchemaDAL.MisuraXAvversita_R
            dt = misuraLeggi.Leggi_Misura_xPaginaEdit(0, 0, "", "", "", objParametri_Server)

            r.RispostaOK = True
            r.RispostaStringa = JSON_Datatable_Rilievi(dt)

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r
    End Function



    <WebMethod(EnableSession:=True)>
    Public Shared Function letturaDaticfgRilieviAnagKendo(cod) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If


        Try

            'Inserire il codice QUI..

            Dim dt As DataTable

            Dim misuraLeggi As New AgronicaCoreMetaSchemaDAL.MisuraxAvversita_Anagrafiche_R
            dt = misuraLeggi.Leggi_Misura_xPaginaEdit(cod, "", "", objParametri_Server)

            r.RispostaOK = True
            r.RispostaStringa = JSON_Datatable_RilieviAnagrafica(dt)

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LetturaDatiCfgAnagraficheVarie() As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If


        Try

            'Inserire il codice QUI..

            Dim dt As DataTable

            Dim avvLeggi As New AgronicaCoreMetaSchemaDAL.Avversita_R
            dt = avvLeggi.Leggi(0, "", AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)

            Dim sJsonAvversita As String =
                jsonArrayEstrai(dt, "Av_Cod", "Av_Des_Vol")

            Dim vegLeggi As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
            dt = vegLeggi.Leggi(0, 0, "", "", AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)

            Dim sJsonSpecie As String =
                jsonArrayEstrai(dt, "Veg_Cod", "Veg_Des")

            Dim udmLeggi As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R
            dt = udmLeggi.Leggi(0, 0, "", "", AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)

            Dim sJsonUnitaMisura As String =
                jsonArrayEstrai(dt, "Udm_Cod", "Udm_Des")

            Dim ffLeggi As New AgronicaCoreMetaSchemaDAL.FasiFenologichexSpecie_R
            dt = ffLeggi.Leggi(0, 0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta, "", "", objParametri_Server)

            Dim ff1 As New List(Of String)
            For Each ffDT In dt.Rows
                ff1.Add("{ ""FF_Cod"": " & ffDT("FF_Cod") & ", " &
                        "  ""FF_Des"": """ & AgronicaCoreUtility.jSon.Escape(ffDT("FF_Des")) & """, " &
                        "  ""Veg_Cod"": " & ffDT("Veg_Cod") & " } ")
            Next

            Dim ff As New StringBuilder
            ff.Append("[")
            ff.Append(String.Join(",", ff1.ToArray))
            ff.Append("]")

            Dim sJsonFasiFenologiche As String = ff.ToString

            Dim sJsonFinale As New StringBuilder
            sJsonFinale.Append("{ ""Avversita"": " & sJsonAvversita & ", ")
            sJsonFinale.Append("  ""Specie"": " & sJsonSpecie & ", ")
            sJsonFinale.Append("  ""FasiFenologiche"": " & sJsonFasiFenologiche & ", ")
            sJsonFinale.Append("  ""UnitaMisura"": " & sJsonUnitaMisura & "}")

            r.RispostaOK = True
            r.RispostaStringa = sJsonFinale.ToString

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r
    End Function

    Private Shared Function jsonArrayEstrai(ByVal dt As DataTable, ByVal sCodice As String, ByVal sValore As String) As String

        Dim dll As New DropDownList
        dll.DataSource = dt
        dll.DataTextField = sValore

        dll.DataValueField = sCodice
        dll.DataBind()

        Dim rval As String =
            AgronicaCoreUtility.CaricaListControl.itemsCollection_AsJsonArray(dll.Items, sCodice, sValore)

        Return rval
    End Function

    Private Shared Function JSON_Datatable_Rilievi(ByVal dt As DataTable) As String

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)
        Dim c As ColonneNome

        'StrSQL.Append("     MisuraxAvversita.VEG_COD,  ")
        'StrSQL.Append("     Avversita.Av_Cod, ")
        'StrSQL.Append("     Avversita.Av_Des_Vol, ")
        'StrSQL.Append("     UnitaMisura.UDM_COD, ")
        'StrSQL.Append("     UnitaMisura.UDM_SIM, ")
        'StrSQL.Append("     UnitaMisura.UDM_DES ")

        c = New ColonneNome("COD", "COD", "string")
        c._Display = False
        l.Add(c)



        c = New ColonneNome("Veg_Cod", "Veg_Cod", "number")
        c._hidden = True
        c._Editabile = True
        l.Add(c)

        c = New ColonneNome("Veg_Des", "Specie", "string")
        c._Filtrabile = True
        c._hidden = True
        c._Editabile = True
        l.Add(c)


        c = New ColonneNome("Av_Cod", "Av_Cod", "number")
        c._hidden = True
        c._Editabile = True
        l.Add(c)

        c = New ColonneNome("Av_Des_Vol", "Avversità", "string")
        c._Filtrabile = True
        c._hidden = True
        c._Editabile = True
        l.Add(c)


        c = New ColonneNome("Udm_Cod", "Udm_Cod", "number")
        c._hidden = True
        c._Editabile = True
        l.Add(c)

        c = New ColonneNome("Udm_Des", "Unità di Misura", "string")
        c._Filtrabile = True
        c._hidden = True
        c._Editabile = True
        l.Add(c)

        c = New ColonneNome("FF_Cod", "FF_Cod", "number")
        c._hidden = True
        c._Editabile = True
        l.Add(c)

        c = New ColonneNome("FF_Des", "Fase Fenologica", "string")
        c._Filtrabile = True
        c._hidden = True
        c._Editabile = True
        l.Add(c)

        c = New ColonneNome("Ordine", "Ordine", "number")
        c._Editabile = True
        l.Add(c)

        c = New ColonneNome("Fondamentale", "Fondamentale", "number")
        c._Editabile = True
        l.Add(c)

        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable

        Return js.JSON_DataTable_Kendo(dt, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaDiscesa)

    End Function

    Private Shared Function JSON_Datatable_RilieviAnagrafica(ByVal dt As DataTable) As String

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)
        Dim c As ColonneNome

        'StrSQL.Append("     MisuraxAvversita.VEG_COD,  ")
        'StrSQL.Append("     Avversita.Av_Cod, ")
        'StrSQL.Append("     Avversita.Av_Des_Vol, ")
        'StrSQL.Append("     UnitaMisura.UDM_COD, ")
        'StrSQL.Append("     UnitaMisura.UDM_SIM, ")
        'StrSQL.Append("     UnitaMisura.UDM_DES ")

        c = New ColonneNome("kendoKey", "kendoKey", "string")
        c._Display = False
        l.Add(c)

        c = New ColonneNome("Anag_Des", "Descrizione Anagrafica", "string")
        c._Editabile = True
        l.Add(c)

        c = New ColonneNome("Anag_Valore", "Anag_Valore", "number")
        c._Editabile = True
        l.Add(c)




        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable

        Return js.JSON_DataTable_Kendo(dt, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaDiscesa)

    End Function
End Class