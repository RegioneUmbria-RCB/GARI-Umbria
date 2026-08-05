Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreScadenziario
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreUtentiDAL
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class Alert_Tipologia
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Tipologie(ByVal objP_server As String,
                                    ByVal id_area As Integer,
                                    ByVal piva As String,
                                    ByVal soloPrivate As Boolean,
                                    ByVal controllaSeUtenteAutorizzato As Boolean,
                                    ByVal tipoPermessoDaControllare As Integer,
                                    ByVal listaIndici As String,
                                    ByVal xMultiSelect As Boolean,
                                    ByVal id_tipologia As Integer
                                    ) As RispostaStandard
        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim objtipologia As New AgronicaCoreScadenziario.Alert_Tipologia_R
            Dim dt As DataTable = objtipologia.Leggi(id_area, id_tipologia, piva, soloPrivate, objParametri_Server, controllaSeUtenteAutorizzato, tipoPermessoDaControllare,
                                                     listaIndici:=listaIndici, xMultiSelect:=xMultiSelect)

            Dim JArrayListaOp As New JArray()


            If dt.Rows.Count > 0 Then

                dt.DefaultView.Sort = "Nome ASC"

                For Each dr In dt.DefaultView
                    Dim data As String = ""
                    If Not IsDBNull(dr.Item("DataDefault")) Then
                        data = dr.Item("DataDefault")
                        'giornoMese = giornoMese.Substring(0, giornoMese.Length - 5)
                    End If

                    JArrayListaOp.Add(New JObject(New JProperty("id_tipologia", dr.Item("id_tipologia")),
                                                  New JProperty("nome", dr.Item("nome")),
                                                  New JProperty("id_area", dr.Item("id_area")),
                                                  New JProperty("utilizzo_giasapp", dr.Item("utilizzo_giasapp")),
                                                  New JProperty("FlagDataScadenzaObbligatoria", dr.Item("FlagDataScadenzaObbligatoria")),
                                                  New JProperty("DataDefault", data)))
                Next

            End If

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            r.RispostaStringa = JsonConvert.SerializeObject(JArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function




    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiTipologie_APP(ByVal objP_server As String) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim objtipologia As New AgronicaCoreScadenziario.Alert_Tipologia_R
            Dim dt As DataTable = objtipologia.Leggi_AreaETipologia("", 0, 0, objParametri_Server, 1)

            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Azienda_Per_id_tipologia(ByVal objP_server As String, ByVal objP_utenti As String, ByVal id_tipologia As Integer) As RispostaStandard
        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim dt As DataTable

            Dim ddl_Aziende As New DropDownList
            AgronicaCoreUtility.CaricaListControl.ImpreseConFiltroUtente(ddl_Aziende, True,
                                                "Tutte le Aziende", "-1", "", " ORDER BY Rag_Soc asc",
                                                objParametri_Server, objParametri_Utenti)

            Select Case id_tipologia
                Case enum_ID_Area_Tipologia.Patentino_trattamenti, enum_ID_Area_Tipologia.Carta_Identita
                    Dim obj_Read As New AgronicaCoreAnagrafeDAL.Contatti_R
                    dt = obj_Read.Leggi_Tutte_Piva_Che_Hanno_Contatto(objParametri_Server)
                Case enum_ID_Area_Tipologia.Taratura_ugelli
                    Dim obj_Read As New AgronicaCoreContabDAL.Parco_Macchine_R
                    dt = obj_Read.Leggi_Tutte_Piva_Che_Hanno_Macchine(objParametri_Server)
                Case enum_ID_Area_Tipologia.Analisi_terreno
                    Dim obj_Read As New AgronicaCoreAnagrafeDAL.Analisi_EntitaxTestata_R
                    dt = obj_Read.Leggi_Tutte_Piva_Che_Hanno_Analisi(objParametri_Server)
                Case enum_ID_Area_Tipologia.Piano_Concimazione
                    Dim obj_Read As New AgronicaCoreAnagrafeDAL.PianoConcimazione_EntitaxTestata_R
                    dt = obj_Read.Leggi_Tutte_Piva_Che_Hanno_PianoConcimazione(objParametri_Server)
                Case enum_ID_Area_Tipologia.PUA
                    Dim obj_Read As New AgronicaCorePUA_DAL.PUA_Testata_R
                    dt = obj_Read.Leggi_Tutte_Piva_Che_Hanno_PUA(objParametri_Server)
                Case Else
                    r.Errore = "la tipologia selezionata non richiede l'azienda"
                    Return r
            End Select

            Dim h As New Hashtable()
            h.Add("-1", "-1")
            For i = 0 To dt.Rows.Count - 1
                h.Add(dt.Rows(i).Item("Piva"), dt.Rows(i).Item("Piva"))
            Next

            For i = ddl_Aziende.Items.Count - 1 To 0 Step -1
                If h.ContainsKey(ddl_Aziende.Items(i).Value) = False Then
                    ddl_Aziende.Items.RemoveAt(i)
                End If
            Next

            Dim JArrayListaOp As New JArray()
            For Each i As ListItem In ddl_Aziende.Items
                JArrayListaOp.Add(New JObject(New JProperty("rag_soc", i.Text), New JProperty("piva", i.Value)))
            Next

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            r.RispostaStringa = JsonConvert.SerializeObject(JArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Aggiungi(ByVal objP_server As String,
                             ByVal objP_utenti As String,
                             ByVal Piva As String,
                             ByVal IDArea As Integer,
                             ByVal NomeTipologia As String,
                             ByVal Utilizzo_GiasAPP As Integer,
                             ByVal DataDefault As String,
                             ByVal FlagDataScadenzaObbligatoria As Integer
                             ) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If
        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

        Dim leggiLingua As New Lingue_Read
        Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod,
                                                                  AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                  "", "", objParametri_Utenti)
        Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
        Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

        Try
            'apro una transazione
            ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)

            'Verifico se esiste e nel caso estraggo l'id della tipologia
            Dim t_R As New AgronicaCoreScadenziario.Alert_Tipologia_R()
            Dim idTipologia As Integer? = t_R.LeggiIdDaNomeTipologia(IDArea, NomeTipologia, Piva, objParametri_Server)

            If Not IsNothing(idTipologia) Then
                r.Errore = Gias.ErroreCategoriaEsistentePerAzienda
                Return r
            End If

            'Salvo la nuova categoria
            Dim seq As New Agro_Sequenze()
            idTipologia = seq.NuovoId_Tabella("Alert_Tipologia", 0, 2000000000, objParametri_Server)

            Dim t_W As New AgronicaCoreScadenziario.Alert_Tipologia_W()
            Dim res As Boolean = t_W.Scrivi(IDArea, idTipologia, Piva, NomeTipologia, "", 0, 0, "", DataDefault, FlagDataScadenzaObbligatoria, objParametri_Server, Utilizzo_GiasAPP)

            If res = False Then
                r.Errore = Gias.ErroreSalvataggioNuovaCategoria
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server) 'Flag_Commit1_Rollback2
                Return r
            End If

            r.RispostaOK = True
            r.RispostaStringa = Gias.CategoriaSalvataCorrettamente

            'Se è andato tutto bene
            ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server) 'Flag_Commit1_Rollback2

        Catch ex As Exception
            ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server) 'Flag_Commit1_Rollback2
            r.Errore = Gias.ErroreDuePunti_ + ex.Message
            Return r
        End Try

        Return r


    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Modifica(ByVal objP_server As String,
                             ByVal objP_utenti As String,
                             ByVal ID_Tipologia As Integer,
                             ByVal Nome_Tipologia As String,
                             ByVal Piva As String,
                             ByVal Utilizzo_GiasAPP As Integer,
                             ByVal DataDefault As String,
                             ByVal FlagDataScadenzaObbligatoria As Integer
                             ) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If
        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        'If ID_Tipologia < 0 Then
        '    r.Errore = Gias.ImpossibileModificareUnaCategoriaDiSistema
        '    Return r
        'End If

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

        Dim leggiLingua As New Lingue_Read
        Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod,
                                                                  AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                  "", "", objParametri_Utenti)
        Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
        Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)


        Dim Colore As String = ""
        Dim Preavviso As Integer = 0
        Dim Cat_Cod As Integer = 0

        Try
            'Ora le tipologie sono per installazione non più per singola impresa
            'Dim t_R As New AgronicaCoreScadenziario.Alert_Tipologia_R()
            'Dim dt As DataTable = t_R.Leggi(0, ID_Tipologia, "", False, objParametri_Server)
            'If dt.Rows(0).Item("Piva") <> Piva Then

            '    Dim a_R As New AgronicaCoreAnagrafeDAL.Imprese_Read()
            '    Dim rag_soc As String = a_R.RagSoc_from_Piva(dt.Rows(0).Item("Piva"), objParametri_Server)
            '    r.Errore = Gias.ImpossibileModificareCategoriaCreataDaAltraAzienda & ": " & rag_soc
            '    Return r
            'End If

            Dim t As New AgronicaCoreScadenziario.Alert_Tipologia_W()
            Dim res As Boolean = t.Modifica(ID_Tipologia, Piva, Nome_Tipologia, Colore, Preavviso, Cat_Cod, DataDefault, FlagDataScadenzaObbligatoria, objParametri_Server, Utilizzo_GiasAPP)

            If res = False Then
                r.Errore = Gias.ErroreDuranteModificaDellaTipologia
                Return r
            End If

            r.RispostaOK = True
            r.RispostaStringa = Gias.CategoriaModificataCorrettamente

        Catch ex As Exception
            r.Errore = Gias.ErroreDuePunti_ + ex.Message
            Return r
        End Try

        Return r

    End Function


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Cancella(ByVal objP_server As String,
                             ByVal objP_utenti As String,
                             ByVal ID_Tipologia As Integer,
                             ByVal Piva As String
                             ) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If
        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

        Dim leggiLingua As New Lingue_Read
        Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, 
                                                                  AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                  "", "", objParametri_Utenti)
        Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
        Threading.Thread.CurrentThread.CurrentUICulture = new Globalization.CultureInfo(linguaCodiceISO)

        Dim t_R As New AgronicaCoreScadenziario.Alert_Tipologia_R()

        'Ora le tipologie sono per installazione non più per singola impresa
        'Dim dt As DataTable = t_R.Leggi(0, ID_Tipologia, "", False, objParametri_Server)
        'If dt.Rows(0).Item("Piva") <> Piva Then

        '    Dim a_R As New AgronicaCoreAnagrafeDAL.Imprese_Read()
        '    Dim rag_soc As String = a_R.RagSoc_from_Piva(dt.Rows(0).Item("Piva"), objParametri_Server)
        '    r.Errore = Gias.ImpossibileCancellareCategoriaCreataDaAltraAzienda_ & rag_soc
        '    Return r
        'End If

        Dim inUso As Boolean = t_R.TestSeTipologiaInUso(ID_Tipologia, objParametri_Server)
        If inUso = True Then
            r.Errore = Gias.CategoriaInUsoPerScadenzeImpossibileCancellare
            Return r
        Else
            'Cerco se esiste uno Schema_Documenti per la Tipologia da eliminare, se esiste non permetto la cancellazione della Tipologia
            Dim obj_SchemaDocumenti As New AgronicaCoreScadenziario.SchemaDocumenti_R

            Dim DT_SchemaDocumenti = obj_SchemaDocumenti.Leggi("", 0, 0, ID_Tipologia, "", 0, New Date(1900, 1, 1), New Date(2100, 12, 31),
                                                               AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                               objParametri_Server, objParametri_Utenti)


            If Not IsNothing(DT_SchemaDocumenti) AndAlso DT_SchemaDocumenti.Rows.Count > 0 Then
                r.Errore = "La tipologia è in uso per qualche schema documenti, impossibile cancellare."
                Return r
            End If
        End If

        'apro una transazione per la cancellazione
        ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)

        Try
            Dim res As Boolean

            Dim t_W As New AgronicaCoreScadenziario.Alert_Tipologia_W()
            res = t_W.Cancella(ID_Tipologia, Piva, objParametri_Server)

            If res = False Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
                r.Errore = Gias.ErroreCancellazioneCategoria
                Return r
            Else

                'Cancello anche Alert_IndicexTipologia (Associazione tra Indice e Tipologia)
                Dim Indice_W As New AgronicaCoreScadenziario.Alert_Indice_W
                res = Indice_W.Cancella_Alert_IndicexTipologie("", 0, ID_Tipologia, 0, objParametri_Server)

                If res = False Then
                    ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
                    r.Errore = Gias.ErroreCancellazioneCategoria
                    Return r
                Else

                    'Cancello anche CategTipologiaDocumentiXUtenti (Autorizzazione  Utenti ,Tipologia e Categoria)
                    Dim CategTipoxUtenti_W As New AgronicaCoreScadenziario.CategTipologiaDocumentiXUtenti_W
                    res = CategTipoxUtenti_W.Cancella("", "", 0, ID_Tipologia, objParametri_Server)

                    If res = False Then
                        ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
                        r.Errore = Gias.ErroreCancellazioneCategoria
                        Return r
                    End If

                End If

            End If

            ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server)
            r.RispostaOK = True
            r.RispostaStringa = Gias.CategoriaCancellataCorrettamente

        Catch ex As Exception
            ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            r.Errore = ex.Message
        Finally
            'Se è andato tutto bene
            ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)
        End Try

        Return r

    End Function


End Class