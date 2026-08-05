Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreVarieBIZ
Imports System.Web.Services
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCorePianoConcimazioneBIZ
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.UtilityProvider
Imports Newtonsoft.Json
Imports ClosedXML.Excel
'Imports AgronicaCoreDataProvider.LogProvider
Public Class PC_PUA_FiltraEsporta

    Inherits System.Web.UI.Page

    Dim objParametri_Utenti As AgronicaCoreParametri
    Dim objParametri_Server As AgronicaCoreParametri

    Public permessi As PermessiUtente


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        '##############################################################
        '#####  Verifico Credenziali di Accesso  ######################
        '##############################################################

        '----- Verifico che l'utente sia autenticato
        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("../Autenticazione/Autenticazione.aspx")
        End If

        permessi = New PermessiUtente()

        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        'AddHandler CType(Page.Master.FindControl("ImgBtn_AnnullaTutto"), ImageButton).Click, AddressOf Me.AnnullaTutto

        caricaGerarchiaImpresa()
        ' caricaSpecieVegetali()

        If Not Page.IsPostBack Then
            HttpContext.Current.Session("dtCuaa") = Nothing
        End If

    End Sub

    Protected Sub caricaGerarchiaImpresa()
        Dim objCombo As New AgronicaCoreUtility.CaricaListControl
        Dim livelli As New List(Of Integer)
        livelli.Add(1)
        livelli.Add(2)
        livelli.Add(3)
        livelli.Add(4)
        livelli.Add(5)
        ddl_Gerarchia_Impresa.Items.Add(New ListItem("Nessuno", "0"))
        objCombo.Gerarchia_Imprese(ddl_Gerarchia_Impresa, True, "{Tutte}", "-1", "", "", livelli, 0, "", "", objParametri_Server)
        'ddl_Gerarchia_Impresa.SelectedIndex = ddl_Gerarchia_Impresa.Items.IndexOf(ddl_Gerarchia_Impresa.Items.FindByValue("0"))
    End Sub

    Protected Sub caricaSpecieVegetali()
        Dim objCombo As New AgronicaCoreUtility.CaricaListControl
        objCombo.TutteSpecieColtivate_3(ddl_SpecieVegetali, True, "", "", "", 0, AGRODATAINIZIO, AGRODATAFINE, True, "", "", objParametri_Server, True)
        'objCombo.TutteSpecieColtivate_4(ddl_SpecieVegetali, True, "", "", "", 0, False, "", "", objParametri_Server, False) 
    End Sub


    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiPUA_Testata_OLD(imprese As String, dataInizio As String, dataFine As String, filtroUltima As Boolean) As rispostaStandard(Of Byte())

        Dim r As New rispostaStandard(Of Byte())

        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) OrElse IsNothing(objParametri_Utenti) OrElse IsNothing(objParametri_Super_Server) Then
            r.Sessione = False
            Return r
        End If


        Try

            'Inserire il codice QUI..

            r.RispostaOK = True

            '----------------------------------------------------------------
            '--- Filtro associato all'utente 
            '----------------------------------------------------------------
            Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
            Dim UtenteProfiloImpreseSql As String = ""
            Dim UtenteProfiloCentriSql As String = ""
            Dim DtImpreseVisibili As DataTable
            Dim i As Integer

            DtImpreseVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Impresa, "", "", objParametri_Server)
            If Not DtImpreseVisibili Is Nothing Then
                For i = 0 To DtImpreseVisibili.Rows.Count - 1
                    UtenteProfiloImpreseSql &= "'" & DtImpreseVisibili.Rows(i).Item("Piva") & "',"
                Next
                If UtenteProfiloImpreseSql <> "" Then
                    UtenteProfiloImpreseSql = " IMP.piva IN (" & Left(UtenteProfiloImpreseSql, UtenteProfiloImpreseSql.Length - 1) & ") "
                End If
            End If

            Dim tipoImpresaGerarchia As Integer = tipoImpresaGerarchia


            Dim impreseList As New List(Of String)
            Dim imp = imprese.Split("|").ToList
            If imp.Contains("-1") Then
                impreseList.Add("-1")
            Else
                For Each value In imp
                    If value <> "" Then
                        impreseList.Add(value)
                    End If
                Next
            End If

            Dim data_Inizio As Date
            Dim data_Fine As Date
            If dataInizio IsNot Nothing AndAlso dataInizio <> "" Then
                If dataInizio.Split("/").Length = 3 Then
                    data_Inizio = CDate(dataInizio)
                Else
                    data_Inizio = New Date(CInt(dataInizio), 1, 1)
                End If
            Else
                data_Inizio = New Date(1900, 1, 1)
            End If

            If dataFine IsNot Nothing AndAlso dataFine <> "" Then
                If dataFine.Split("/").Length = 3 Then
                    data_Fine = CDate(dataFine)
                Else
                    data_Fine = New Date(CInt(dataFine), 12, 31)
                End If
            Else
                data_Fine = New Date(2100, 12, 31)
            End If

            Dim regolamentoCod As Integer = 125 '78

            Dim objEffluentiInput As New PUA_Effluenti_input With {
                    .Regolamento_Cod = regolamentoCod
                }
            Dim objPC As New AgronicaCoreWebService.PianoConcimazione_WS
            Dim objEffluentiOutput As New PUA_Effluenti_output
            objEffluentiOutput = objPC.Effluenti(objEffluentiInput, objParametri_Server, objParametri_Super_Server)

            Dim listaCoefficienteB As List(Of PUA_CoefficienteB)
            listaCoefficienteB = GetListaCoefficienteB(regolamentoCod)

            Dim dtZvnServer As DataTable = Nothing
            Dim dtZvnMetaschema As DataTable = Nothing
            LeggiZoneVulnerabili(regolamentoCod, AGRODATAINIZIO, AGRODATAFINE, dtZvnServer, dtZvnMetaschema, objParametri_Server)

            Dim dtPua As DataTable
            Dim objP As New AgronicaCorePUA_DAL.PUA_Testata_R

            Dim Piva As String = "" '"03566410548" '"01704430519" '"01631160544"
            dtPua = objP.LeggiTestata_Da_GerarchiaImpresa(impreseList, Piva, data_Inizio, data_Fine, " pt.regolamento_cod=" & regolamentoCod, "rag_soc", objParametri_Server, objParametri_Utenti, filtroUltima)

            Dim listPiani As New List(Of PUA_Appezzamento)
            Dim listPiano As New List(Of PUA_Appezzamento)

            Dim DtRis As New DataTable
            Dim Dr As DataRow

            DtRis.Columns.Add(New DataColumn("pua_cod", GetType(Integer)))
            DtRis.Columns.Add(New DataColumn("regolamento_cod", GetType(Integer)))
            DtRis.Columns.Add(New DataColumn("cuaa", GetType(String)))
            DtRis.Columns.Add(New DataColumn("piva", GetType(String)))
            DtRis.Columns.Add(New DataColumn("ragione_sociale", GetType(String)))
            DtRis.Columns.Add(New DataColumn("app", GetType(String)))
            DtRis.Columns.Add(New DataColumn("sup_appezzamento", GetType(Decimal)))
            DtRis.Columns.Add(New DataColumn("specie", GetType(String)))
            'DtRis.Columns.Add(New DataColumn("catasto", GetType(String)))
            DtRis.Columns.Add(New DataColumn("provincia", GetType(String)))
            DtRis.Columns.Add(New DataColumn("comune", GetType(String)))
            DtRis.Columns.Add(New DataColumn("istat_provincia", GetType(String)))
            DtRis.Columns.Add(New DataColumn("istat_comune", GetType(String)))
            DtRis.Columns.Add(New DataColumn("sezione", GetType(String)))
            DtRis.Columns.Add(New DataColumn("foglio", GetType(Integer)))
            DtRis.Columns.Add(New DataColumn("numero", GetType(Integer)))
            DtRis.Columns.Add(New DataColumn("subalterno", GetType(String)))
            DtRis.Columns.Add(New DataColumn("sup_particella", GetType(Decimal)))
            DtRis.Columns.Add(New DataColumn("zvn_si", GetType(String)))
            DtRis.Columns.Add(New DataColumn("zvn_no", GetType(String)))
            DtRis.Columns.Add(New DataColumn("conforme_si", GetType(String)))
            DtRis.Columns.Add(New DataColumn("conforme_no", GetType(String)))
            DtRis.Columns.Add(New DataColumn("compilatore", GetType(String)))

            Dim r_pua As RispostaStandard

            Dim objPW As New AgronicaCorePUA_DAL.Pua_Elaborazione_W

            For Each row In dtPua.Rows

                r_pua = CaricaGrigliaPianoDistribuzione(row.Item("piva"), row.Item("regolamento_cod"), row.Item("pua_cod"), row.Item("pua_tipo"),
                                                row.Item("Validita_inizio"), row.Item("Validita_Fine"),
                                                enum_PUA_Modalita.Modalita_Verifica, 100,
                                                objEffluentiOutput, listaCoefficienteB,
                                                dtZvnServer, dtZvnMetaschema)

                If r_pua.RispostaOK = True Then

                    Dim obj_Dati As PUA_Appezzamento()
                    obj_Dati = JsonConvert.DeserializeObject(Of PUA_Appezzamento())(r_pua.RispostaStringa)

                    For Each a As PUA_Appezzamento In obj_Dati

                        For Each p As PUA_ParticellaVincoloAgronomico In a.ParticelleVincoli

                            Dr = DtRis.NewRow

                            Dr.Item("pua_cod") = row.Item("pua_cod")
                            Dr.Item("regolamento_cod") = row.Item("regolamento_cod")

                            Dr.Item("cuaa") = row.Item("cuaa")
                            Dr.Item("piva") = row.Item("piva")
                            Dr.Item("ragione_sociale") = row.Item("rag_soc")

                            Dr.Item("app") = Split(a.Appezzamento, " - ")(0)
                            Dr.Item("sup_appezzamento") = a.Superficie
                            Dr.Item("specie") = Split(a.Appezzamento, " - ")(1)

                            'Dr.Item("catasto") = a.Catasto
                            Dr.Item("istat_provincia") = p.Part_PROV
                            Dr.Item("istat_comune") = p.Part_COM
                            Dr.Item("sezione") = p.Part_SEZIONE
                            Dr.Item("foglio") = p.Part_FOGLIO
                            Dr.Item("numero") = p.Part_NUMERO
                            Dr.Item("subalterno") = p.Part_SUBALTERNO
                            Dr.Item("provincia") = p.Part_PROVINCIA
                            Dr.Item("comune") = p.Part_COMUNE
                            Dr.Item("sup_particella") = p.Sup_Condotta

                            If a.ZVN = True Then
                                Dr.Item("zvn_si") = "X"
                                Dr.Item("zvn_no") = ""
                            Else
                                Dr.Item("zvn_si") = ""
                                Dr.Item("zvn_no") = "X"
                            End If

                            'se almeno un valore non è conforme
                            If a.Valutazione_NUtile = 1 Or a.Valutazione_NTotale = 1 Or a.Valutazione_Efficienza = 1 Then
                                Dr.Item("conforme_si") = ""
                                Dr.Item("conforme_no") = "X"
                            Else
                                Dr.Item("conforme_no") = ""
                                Dr.Item("conforme_si") = "X"
                            End If

                            Dr.Item("compilatore") = row.Item("compilatore")



                            If a.Id_AnagrafeVincoli > 0 Then

                                objPW.Scrivi(row.Item("pua_cod"), row.Item("regolamento_cod"), row.Item("pua_tipo"),
                                                  a.Id_AnagrafeVincoli,
                                                  row.Item("cuaa"), row.Item("piva"), a.Sa_Cod,
                                                    a.Campo_Cod, a.appezza, a.id_reg, a.Progetto_Cod,
                                                    IIf(a.Veg_Cod < 0, 0, a.Veg_Cod), 0, IIf(a.Veg_Cod < 0, Math.Abs(a.Veg_Cod), 0),
                                                    a.Grfi_Cod, a.Grfi_Cod_Concimazione, a.StatoImpiantoCod, a.Ciclo,
                                                    a.Resa, a.Resa_Rif, a.FattoreCorrettivo_N, a.Superficie, IIf(a.ZVN = True, 1, 0),
                                                    a.N_Fabbisogno_Database, a.N_Fabbisogno, a.LimiteMas,
                                                    a.N_FabbisognoSoddisfatto, a.N_TotaleSoddisfatto, a.N_Zootecnico, a.N_Zootecnico_Letame, a.N_Zootecnico_Liquame,
                                                    a.N_BilancioAzotato_Utile, a.N_BilancioAzotato_Totale, a.Indice_Efficienza_Azotata,
                                                    a.Valutazione_NUtile, a.Valutazione_NTotale, a.Valutazione_Efficienza,
                                                    IIf(a.Valutazione_NUtile = 1 Or a.Valutazione_NTotale = 1 Or a.Valutazione_Efficienza = 1, 0, 1),
                                                    row.Item("Validita_inizio"), row.Item("Validita_Fine"), objParametri_Server)

                            End If



                            'objPW.Scrivi_old(row.Item("pua_cod"), row.Item("regolamento_cod"),
                            '                              row.Item("piva"), row.Item("cuaa"), row.Item("rag_soc"),
                            '                              Split(a.Appezzamento, " - ")(0), a.Superficie, Split(a.Appezzamento, " - ")(1),
                            '                              p.Part_PROVINCIA, p.Part_COMUNE, p.Part_PROV, p.Part_COM, p.Part_SEZIONE, p.Part_FOGLIO, p.Part_NUMERO, p.Part_SUBALTERNO, p.Sup_Condotta,
                            '                              Dr.Item("zvn_si"), Dr.Item("zvn_no"), Dr.Item("conforme_si"), Dr.Item("conforme_no"), row.Item("compilatore"),
                            '                                objParametri_Server)



                            DtRis.Rows.Add(Dr)

                        Next

                    Next

                End If

            Next

            'percorso e nome del file da esportare
            Dim nomeFileUnivoco As String = "Estrazione_PUA_" & AgronicaCoreUtility.FileSystemHelper.NomeFileUnivoco(".xlsx")
            Dim _AgroWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
            Dim path_file As String = _AgroWebConfig.GestioneAllegati_Repository & "\" & nomeFileUnivoco

            DtRis.TableName = "Esportazione"

            'Creo il file xlsx a partire dal DT
            Dim workbook = New ClosedXML.Excel.XLWorkbook()
            workbook.Worksheets.Add(DtRis)
            workbook.SaveAs(path_file)

            'creo l'url dove si deve andare a prendere il file
            Dim url As String = _AgroWebConfig.LinkAgronicaStampe.ToLower().Replace("gestionerichieste.aspx", "") & "File_Allegati/" & nomeFileUnivoco

            r.RispostaOK = True
            'r.RispostaStringa = url

            Dim binReader As New System.IO.BinaryReader(System.IO.File.Open(path_file, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            binReader.BaseStream.Position = 0
            Dim binFile As Byte() = binReader.ReadBytes(Convert.ToInt32(binReader.BaseStream.Length))
            binReader.Close()
            r.RispostaStringa = binFile
            r.opzioniWatable.PrefissoNomeFileExport = nomeFileUnivoco


        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaGrigliaPianoDistribuzione(ByVal piva As String,
                                                           ByVal regolamentoCod As Integer,
                                                           ByVal puaCod As Integer,
                                                           ByVal puaTipo As Integer,
                                                           ByVal dataInizio As Date, ByVal dataFine As Date,
                                                           ByVal modalita As Integer, ByVal num_blocco As Integer,
                                                           ByVal objEffluentiOutput As PUA_Effluenti_output, ByVal listaCoefficienteB As List(Of PUA_CoefficienteB),
                                                           ByVal dtZvnServer As DataTable, ByVal dtZvnMetaschema As DataTable
                                                           ) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) OrElse IsNothing(objParametriUtenti) Then
            r.Sessione = False
            Return r
        End If

        Dim SeparatoreChiave As Char = "_"
        Dim VulnerabileStr As String = " <b>(V)</b>"

        Try



            'recupero effluenti e relativi fertilizzanti per l'analisi dell'azoto organico
            Dim HashFerCodOrganici As New Hashtable
            Dim HashFerCodDigestati As New Hashtable
            Dim HashFerCodLetami As New Hashtable
            Dim HashFerCodLiquami As New Hashtable

            Dim objEff As New AgronicaCorePUA_DAL.Pua_Effluente_R
            Dim DtEffluenti As DataTable = objEff.Leggi(regolamentoCod, puaCod, 0, " azoto_qta >0 ", "", objParametriServer)

            'spostata fuori
            'Dim objEffluentiInput As New PUA_Effluenti_input With {
            '        .Regolamento_Cod = regolamentoCod
            '    }
            'Dim objPC As New AgronicaCoreWebService.PianoConcimazione_WS
            'Dim objEffluentiOutput As New PUA_Effluenti_output
            'objEffluentiOutput = objPC.Effluenti(objEffluentiInput)

            Dim Perc_Zootecnico As Decimal = 100
            Dim str_FerCod_Org As String = ""

            If Not objEffluentiOutput Is Nothing Then
                For Each eff As PUA_Effluente In objEffluentiOutput.ListaEffluenti

                    Perc_Zootecnico = 100
                    If eff.MatricePrevalente = 1 AndAlso Not DtEffluenti Is Nothing AndAlso DtEffluenti.Select("eff_cod=" & eff.Eff_Cod).Length > 0 Then
                        Perc_Zootecnico = DtEffluenti.Select("eff_cod=" & eff.Eff_Cod)(0).Item("perc_zootecnico")
                    End If

                    If eff.SpecieAllevamento = 1 Then
                        HashFerCodOrganici.Add(eff.Fer_Cod, Perc_Zootecnico)
                        str_FerCod_Org &= eff.Fer_Cod & ","
                    End If
                    If eff.MatricePrevalente = 1 Then
                        HashFerCodDigestati.Add(eff.Fer_Cod, Perc_Zootecnico)
                    End If

                    Select Case eff.Id_tp_fer
                        Case enum_PUA_TipoFertilizzante.Ammendante
                            HashFerCodLetami.Add(eff.Fer_Cod, Perc_Zootecnico)
                        Case enum_PUA_TipoFertilizzante.Liquame
                            HashFerCodLiquami.Add(eff.Fer_Cod, Perc_Zootecnico)
                    End Select

                Next
            End If

            If str_FerCod_Org <> "" Then
                str_FerCod_Org = "(" & Left(str_FerCod_Org, str_FerCod_Org.Length - 1) & ")"
            End If

            '"(" & fer_cod_d & ")") * HashFerCodDigestati(fer_cod_d) / 100

            Dim objImpiantiR As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
            Dim objSequenze As New Agro_Sequenze
            Dim objAnaVincoliW As New AgronicaCoreAnagrafeDAL.Anagrafe_VincoliAgronomici_W

            Dim Filtro As String = "" ' s.veg_cod=38 "

            Dim dt As DataTable = objImpiantiR.Leggi_Dati_Impianti_per_PUA_NEW(piva, 0, puaCod, regolamentoCod,
                                                                                    dataInizio, dataFine,
                                                                                       1,
                                                                                       str_FerCod_Org, HashFerCodDigestati,
                                                                                       modalita, HashFerCodLetami, HashFerCodLiquami,
                                                                                       Filtro, " App_Nome ASC, av.id desc ", objParametriServer, True)

            Dim dtTN As DataTable = objImpiantiR.Leggi_Dati_Impianti_per_PUA_NEW(piva, 0, puaCod, regolamentoCod,
                                                                                    dataInizio, dataFine,
                                                                                       2,
                                                                                       "", Nothing,
                                                                                       modalita, Nothing, Nothing,
                                                                                       "", " App_Nome ASC, av.id desc ", objParametriServer, True)

            'Dim dictStatoImpianto As New Dictionary(Of Integer, List(Of Fase))

            'Dim listaPrecessioni As List(Of Precessione) = GetListaPrecessioniDes(regolamentoCod)
            'Dim listaUbicazioni As List(Of Ubicazione) = GetListaUbicazioniDes(regolamentoCod)
            'Dim listaTipiAcqua As List(Of TipoAcqua) = GetListaTipiAcquaDes(regolamentoCod)

            'estraggo i singoli veg_cod
            'Dim listaReseMas As List(Of PianoConcimazione_LimiteMAS_output)
            'Dim listaFinalita As List(Of Finalita)
            'Dim listaCoefficienteB As List(Of PUA_CoefficienteB)
            'Dim HashVegCod As New Hashtable
            'Dim veg_cod_elenco As String
            'If Not dt Is Nothing Then
            '    For Each row In dt.Rows
            '        If Not HashVegCod.ContainsKey(row.Item("veg_cod")) Then
            '            HashVegCod.Add(row.Item("veg_cod"), "")
            '            veg_cod_elenco &= row.Item("veg_cod") & ","
            '        End If
            '    Next
            '    If veg_cod_elenco <> "" Then
            '        'listaReseMas = GetListaReseMas(regolamentoCod, Left(veg_cod_elenco, veg_cod_elenco.Length - 1))
            '        'listaFinalita = GetListaFinalitaRer(regolamentoCod, Left(veg_cod_elenco, veg_cod_elenco.Length - 1))
            '        'listaCoefficienteB = GetListaCoefficienteB(regolamentoCod, Left(veg_cod_elenco, veg_cod_elenco.Length - 1))
            '    End If
            'End If


            'Dim dtZvnServer As DataTable = Nothing
            'Dim dtZvnMetaschema As DataTable = Nothing
            'LeggiZoneVulnerabili(regolamentoCod, dtZvnServer, dtZvnMetaschema, objParametriServer)


            'TODO: verifico se nelle liste c'è un solo elemento possibile, quindi lo scriverò subito

            'Dim preimpostaPrecessione As Integer? = Nothing
            'If Not listaPrecessioni Is Nothing AndAlso listaPrecessioni.Count = 1 Then
            '    preimpostaPrecessione = listaPrecessioni(0).Codice
            'End If

            'Dim preimpostaUbicazione As Integer? = Nothing
            'If Not listaUbicazioni Is Nothing AndAlso listaUbicazioni.Count = 1 Then
            '    preimpostaUbicazione = listaUbicazioni(0).Codice
            'End If

            Dim listPiano As New List(Of PUA_Appezzamento)

            'Chiave = keyImpianto, Value = keyParticella
            Dim dict As New Dictionary(Of String, List(Of String))
            'Dim dictAnalisi As New Dictionary(Of Integer, AnalisiDettaglio)

            Dim count As Integer = 1

            Dim ChiamaWS As Boolean = True

            If Not dt Is Nothing Then

                For Each row In dt.Rows

                    Dim chiaveDistinta As String = row.Item("Piva") & SeparatoreChiave &
                                                   row.Item("Sa_Cod") & SeparatoreChiave &
                                                   row.Item("Appezza") & SeparatoreChiave &
                                                   row.Item("Id_Reg") & SeparatoreChiave &
                                                   row.Item("Progetto_Cod")
                    Dim chiavePart As String = row.Item("PROV") & ":" & row.Item("COM") & ":_" & row.Item("SEZIONE") & ":_" & row.Item("FOGLIO") & ":_" & row.Item("NUMERO") & ":_" & row.Item("SUBALTERNO")
                    Dim objPiano As PUA_Appezzamento

                    If Not dict.ContainsKey(chiaveDistinta) Then

                        Dim nFabbDatabaseStr As String = row.Item("N_Fabbisogno")
                        Dim nFabbDatabase As Decimal = -1
                        If IsNumeric(nFabbDatabaseStr) Then
                            nFabbDatabase = CDec(nFabbDatabaseStr)
                        End If

                        Dim nFabbDatabaseOrgStr As String = row.Item("N_Fabbisogno_Organico")
                        Dim nFabbDatabaseOrg As Decimal = -1
                        If IsNumeric(nFabbDatabaseOrgStr) Then
                            nFabbDatabaseOrg = CDec(nFabbDatabaseOrgStr)
                        End If

                        'è la prima volta dell'impianto, quindi devo creare tutto
                        objPiano = New PUA_Appezzamento With {
                            .Chiave = chiaveDistinta,
                            .Piva = row.Item("Piva"),
                            .Sa_Cod = row.Item("Sa_Cod"),
                            .Campo_Cod = row.Item("Campo_Cod"),
                            .appezza = row.Item("Appezza"),
                            .id_reg = row.Item("Id_Reg"),
                            .Progetto_Cod = row.Item("Progetto_Cod"),
                            .Veg_Cod = row.Item("Veg_Cod"),
                            .Grfi_Cod = row.Item("Grfi_Cod"),
                            .Grfi_Cod_Concimazione = row.Item("Grfi_Cod_Concimazione"),
                            .B_Perc = 0,
                            .Appezzamento = row.Item("App_Nome") & " - " & row.Item("veg_des"),
                            .Superficie = row.Item("Sup_Imp"),
                            .ValiditaInizio = row.Item("Validita_Inizio_Impianto"),
                            .ValiditaFine = row.Item("Validita_Fine_Impianto"),
                            .DurataColtura = row.Item("Validita_Inizio_Impianto") & "|" & row.Item("Validita_Fine_Impianto"),
                            .StatoImpiantoCod = row.Item("Stato_Impianto"),
                            .Ciclo = row.Item("Ciclo_Cod"),
                            .CicloDes = row.Item("Ciclo_Des"),
                            .Resa = row.Item("Resa"),
                            .Id_AnagrafeVincoli = row.Item("Id_AnaVincoli"),
                            .Pua_Cod = row.Item("Pua_Cod"),
                            .Regolamento_Cod = row.Item("PUA_Regolamento_Cod"),
                            .Data_Pua = row.Item("DataPua"),
                            .AnalisiTestataCod = row.Item("Analisi_Testata_Cod"),
                            .AnalisiTestataDes = row.Item("Analisi_Testata_Des"),
                            .Sabbia = row.Item("sabbia"),
                            .Argilla = row.Item("argilla"),
                            .So = row.Item("So"),
                            .PrecessioneCod = row.Item("Veg_Cod_Prec"),
                            .UbicazioneCod = row.Item("Ubicazione_Cod"),
                            .TipoAcquaCod = row.Item("TipoAcqua_Cod"),
                            .N_FertilizzazioniPrecedenti = row.Item("N_FertilizzazioniPrecedenti"),
                            .N_Fabbisogno_Database = nFabbDatabase,
                            .N_Fabbisogno = nFabbDatabaseOrg,
                            .N_FabbisognoComplessivo = If(nFabbDatabase >= 0, nFabbDatabase, 0) * row.Item("Sup_Imp"),
                            .N_FabbisognoSoddisfatto = CDec(row.Item("N_FabbisognoSoddisfatto")),
                            .N_Zootecnico = CDec(row.Item("N_FabbisognoSoddisfattoOrganico")) + CDec(row.Item("N_SoddisfattoDigestato")),
                            .LimiteMas = 0,
                            .N_TotaleSoddisfatto = CDec(row.Item("N_TotaleSoddisfatto")),
                            .N_Zootecnico_Letame = CDec(row.Item("N_Zootecnico_Letame")),
                            .N_Zootecnico_Liquame = CDec(row.Item("N_Zootecnico_Liquame"))
                        }





                        Dim objParticella As New PUA_ParticellaVincoloAgronomico With {
                            .Part_PROV = row.Item("PROV"),
                            .Part_COM = row.Item("COM"),
                            .Part_SEZIONE = row.Item("SEZIONE"),
                            .Part_FOGLIO = row.Item("FOGLIO"),
                            .Part_NUMERO = row.Item("NUMERO"),
                            .Part_SUBALTERNO = row.Item("SUBALTERNO"),
                            .Part_PROVINCIA = row.Item("provincia"),
                            .Part_COMUNE = row.Item("comune"),
                            .Sup_Condotta = row.item("sup_condotta")
                        }

                        '------ Zone Vulnerabili (ZVN)
                        Dim flagVulnerabile As Boolean = IsZonaVulnerabile(dataInizio, dataFine,
                                                                           objParticella.Part_PROV, objParticella.Part_COM,
                                                                           objParticella.Part_SEZIONE, objParticella.Part_FOGLIO,
                                                                           objParticella.Part_NUMERO, objParticella.Part_SUBALTERNO,
                                                                           dtZvnServer, dtZvnMetaschema)

                        objParticella.ZVN = flagVulnerabile

                        'C'è almeno una particella vulnerabile, quindi tutto l'appezzamento viene considerato vulnerabile
                        If objParticella.ZVN = True Then
                            objPiano.ZVN = True
                        End If


                        objPiano.ParticelleVincoli.Add(objParticella)


                        '------ Catasto
                        objPiano.Catasto = chiavePart & If(objParticella.ZVN = True, VulnerabileStr, "")

                        ''------ StatoImpiantoDes
                        'objPiano.StatoImpiantoDes = GetStatoImpiantoDes(dictStatoImpianto,
                        '                                                regolamentoCod, objPiano.Veg_Cod,
                        '                                                objPiano.StatoImpiantoCod)

                        ''------ PrecessioneDes
                        'objPiano.PrecessioneDes = GetPrecessioneDes(listaPrecessioni, objPiano.PrecessioneCod)

                        ''------ UbicazioneDes
                        'objPiano.UbicazioneDes = GetUbicazioneDes(listaUbicazioni, objPiano.UbicazioneCod)

                        ''------ TipoAcquaDes
                        'objPiano.TipoAcquaDes = GetTipoAcquaDes(listaTipiAcqua, objPiano.TipoAcquaCod)

                        Dim ResaDB As Decimal = -1
                        Dim MasDB As Decimal = -1
                        Dim Grfi_Cod_ConcimazioneDB As Integer = -1
                        'GetResaMas(listaReseMas, listaFinalita, listaCoefficienteB,
                        '           objPiano.Veg_Cod, objPiano.Grfi_Cod, objPiano.Grfi_Cod_Concimazione,
                        '           objPiano.StatoImpiantoCod, ResaDB, MasDB, Grfi_Cod_ConcimazioneDB)

                        ''------ Resa
                        'If objPiano.Resa <= 0 And ResaDB > 0 Then
                        '    objPiano.Resa = ResaDB
                        '    ValorizzaResaDaMas(objPiano, objParametriServer)
                        'End If

                        ''----- Mas
                        'If objPiano.LimiteMas <= 0 And MasDB > 0 Then
                        '    objPiano.LimiteMas = MasDB
                        'End If

                        ''-- finalita piano conc
                        'If objPiano.Grfi_Cod_Concimazione <= 0 And Grfi_Cod_ConcimazioneDB > 0 Then
                        '    objPiano.Grfi_Cod_Concimazione = Grfi_Cod_ConcimazioneDB
                        '    ValorizzaFinalitaRer(objPiano, objParametriServer)
                        'End If

                        If objPiano.Grfi_Cod_Concimazione > 0 Then
                            'objPiano.Grfi_Des_Concimazione = GetFinalitaRerDes(listaFinalita, objPiano.Grfi_Cod_Concimazione)
                            Dim Base As Decimal = 0
                            Dim Coeff_Incr As Decimal = 1
                            objPiano.B_Perc = GetCoefficienteB(listaCoefficienteB, objPiano.Veg_Cod, objPiano.Grfi_Cod_Concimazione, Base, Coeff_Incr)
                            If objPiano.Resa > 0.0 Then
                                objPiano.Assorbimento = ((objPiano.B_Perc * 10 * objPiano.Resa) + Base) * Coeff_Incr
                            Else
                                objPiano.Assorbimento = 0
                            End If
                        Else
                            ChiamaWS = True
                        End If

                        listPiano.Add(objPiano)

                        dict.Add(chiaveDistinta, New List(Of String) From {chiavePart})

                    Else

                        'devo solo prendere la parte della particella
                        Dim idx As Integer = listPiano.FindIndex(Function(x) x.Chiave = chiaveDistinta)

                        Dim ParticellaEsistente = (From part In listPiano(idx).ParticelleVincoli Where part.Part_PROV = row.Item("PROV") And part.Part_COM = row.Item("COM") And part.Part_SEZIONE = row.Item("SEZIONE") And part.Part_FOGLIO = row.Item("FOGLIO") And part.Part_NUMERO = row.Item("NUMERO") And part.Part_SUBALTERNO = row.Item("SUBALTERNO") Select part).FirstOrDefault

                        If ParticellaEsistente Is Nothing Then

                            Dim objParticella As New PUA_ParticellaVincoloAgronomico With {
                         .Part_PROV = row.Item("PROV"),
                         .Part_COM = row.Item("COM"),
                         .Part_SEZIONE = row.Item("SEZIONE"),
                         .Part_FOGLIO = row.Item("FOGLIO"),
                         .Part_NUMERO = row.Item("NUMERO"),
                         .Part_SUBALTERNO = row.Item("SUBALTERNO"),
                         .Part_PROVINCIA = row.Item("provincia"),
                         .Part_COMUNE = row.Item("comune"),
                         .Sup_Condotta = row.item("sup_condotta")
                     }

                            '------ Zone Vulnerabili (ZVN)
                            Dim flagVulnerabile As Boolean = IsZonaVulnerabile(dataInizio, dataFine,
                                                                               objParticella.Part_PROV, objParticella.Part_COM,
                                                                               objParticella.Part_SEZIONE, objParticella.Part_FOGLIO,
                                                                               objParticella.Part_NUMERO, objParticella.Part_SUBALTERNO,
                                                                               dtZvnServer, dtZvnMetaschema)

                            objParticella.ZVN = flagVulnerabile

                            'C'è almeno una particella vulnerabile, quindi tutto l'appezzamento viene considerato vulnerabile
                            If objParticella.ZVN = True Then
                                listPiano(idx).ZVN = True
                            End If

                            listPiano(idx).Catasto = listPiano(idx).Catasto & "|" & chiavePart & If(objParticella.ZVN = True, VulnerabileStr, "")
                            listPiano(idx).ParticelleVincoli.Add(objParticella)

                        End If



                    End If

                    count += 1
                Next

            End If



            'ricavo N_Fabbisogno 

            '(09/10/2019 fede) spezzo le chiamate al ws per ridurre la dimensione della richiesta
            Dim listPianoTmp As New List(Of PUA_Appezzamento)
            Dim n As Integer = 1
            Dim listPianoTotale As New List(Of PUA_Appezzamento)

            'num_blocco = 100

            '(fede) settare a false quando non richiesta l'elaborazione della conformità
            ChiamaWS = False

            If ChiamaWS = True Then

                For l = 0 To listPiano.Count - 1

                    listPianoTmp.Add(listPiano(l))

                    If (listPiano.Count - 1) = l Then
                        Dim Fabb_InputTmp As New PUA_Fabbisogni_input With {
                            .Regolamento_Cod = regolamentoCod,
                            .PUA_Tipo = puaTipo,
                            .PUA_ListaAppezzamenti = listPianoTmp
                        }
                        Dim Fabb_outTmp As New PUA_Fabbisogni_output
                        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
                        'Fabb_outTmp = objPC_WS.PUAFabbisogni(Fabb_InputTmp)
                        Fabb_outTmp = objPC_WS.PUAFabbisogniCompresso(Fabb_InputTmp)

                        listPianoTotale.AddRange(Fabb_outTmp.PUA_ListaAppezzamenti)
                        n = 1
                        listPianoTmp.Clear()
                    ElseIf n < num_blocco Then
                        n += 1
                    Else
                        Dim Fabb_InputTmp As New PUA_Fabbisogni_input With {
                            .Regolamento_Cod = regolamentoCod,
                            .PUA_Tipo = puaTipo,
                            .PUA_ListaAppezzamenti = listPianoTmp
                        }
                        Dim Fabb_outTmp As New PUA_Fabbisogni_output
                        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
                        'Fabb_outTmp = objPC_WS.PUAFabbisogni(Fabb_InputTmp)
                        Fabb_outTmp = objPC_WS.PUAFabbisogniCompresso(Fabb_InputTmp)

                        listPianoTotale.AddRange(Fabb_outTmp.PUA_ListaAppezzamenti)
                        n = 1
                        listPianoTmp.Clear()
                    End If

                Next

                listPiano = listPianoTotale

            End If
            '----------------------------------------------------------------------------------------------------

            'aggiungo i terreni nudi
            If Not dtTN Is Nothing Then

                For Each row In dtTN.Rows

                    Dim chiaveDistinta As String = row.Item("Piva") & SeparatoreChiave &
                                                   row.Item("Sa_Cod") & SeparatoreChiave &
                                                   row.Item("Appezza") & SeparatoreChiave &
                                                   row.Item("Id_Reg") & SeparatoreChiave &
                                                   row.Item("Progetto_Cod")
                    Dim chiavePart As String = row.Item("PROV") & ":" & row.Item("COM") & ":_" & row.Item("SEZIONE") & ":_" & row.Item("FOGLIO") & ":_" & row.Item("NUMERO") & ":_" & row.Item("SUBALTERNO")
                    Dim objPiano As PUA_Appezzamento

                    If Not dict.ContainsKey(chiaveDistinta) Then

                        Dim nFabbDatabaseStr As String = row.Item("N_Fabbisogno")
                        Dim nFabbDatabase As Decimal = -1
                        If IsNumeric(nFabbDatabaseStr) Then
                            nFabbDatabase = CDec(nFabbDatabaseStr)
                        End If

                        Dim nFabbDatabaseOrgStr As String = row.Item("N_Fabbisogno_Organico")
                        Dim nFabbDatabaseOrg As Decimal = -1
                        If IsNumeric(nFabbDatabaseOrgStr) Then
                            nFabbDatabaseOrg = CDec(nFabbDatabaseOrgStr)
                        End If

                        ' Giulia: 9/1/2020: Quando passava da web service, N_fabbisogno, veniva impostato a 0, ora viene mostrato -1
                        'quindi se su db è "impostato" -1 (vuol dire che non l'avevo ancora associato all'impianto), mostro 0; se poi si sceglie di far ereditare all'impianto il valore, sarà salvato 0

                        'è la prima volta dell'impianto, quindi devo creare tutto
                        objPiano = New PUA_Appezzamento With {
                            .Chiave = chiaveDistinta,
                            .Piva = row.Item("Piva"),
                            .Sa_Cod = row.Item("Sa_Cod"),
                            .Campo_Cod = row.Item("Campo_Cod"),
                            .appezza = row.Item("Appezza"),
                            .id_reg = row.Item("Id_Reg"),
                            .Progetto_Cod = row.Item("Progetto_Cod"),
                            .Veg_Cod = row.Item("Veg_Cod"),
                            .Grfi_Cod = row.Item("Grfi_Cod"),
                            .Grfi_Cod_Concimazione = row.Item("Grfi_Cod_Concimazione"),
                            .B_Perc = 0,
                            .Appezzamento = row.Item("App_Nome") & " - " & row.Item("veg_des"),
                            .Superficie = row.Item("Sup_Imp"),
                            .ValiditaInizio = row.Item("Validita_Inizio_Impianto"),
                            .ValiditaFine = row.Item("Validita_Fine_Impianto"),
                            .DurataColtura = row.Item("Validita_Inizio_Impianto") & "|" & row.Item("Validita_Fine_Impianto"),
                            .StatoImpiantoCod = row.Item("Stato_Impianto"),
                            .Ciclo = row.Item("Ciclo_Cod"),
                            .CicloDes = row.Item("Ciclo_Des"),
                            .Resa = row.Item("Resa"),
                            .Id_AnagrafeVincoli = row.Item("Id_AnaVincoli"),
                            .Pua_Cod = row.Item("Pua_Cod"),
                            .Regolamento_Cod = row.Item("PUA_Regolamento_Cod"),
                            .Data_Pua = row.Item("DataPua"),
                            .AnalisiTestataCod = row.Item("Analisi_Testata_Cod"),
                            .AnalisiTestataDes = row.Item("Analisi_Testata_Des"),
                            .Sabbia = row.Item("sabbia"),
                            .Argilla = row.Item("argilla"),
                            .So = row.Item("So"),
                            .PrecessioneCod = row.Item("Veg_Cod_Prec"),
                            .UbicazioneCod = row.Item("Ubicazione_Cod"),
                            .TipoAcquaCod = row.Item("TipoAcqua_Cod"),
                            .N_FertilizzazioniPrecedenti = row.Item("N_FertilizzazioniPrecedenti"),
                            .N_Fabbisogno_Database = nFabbDatabase,
                            .N_Fabbisogno = If(nFabbDatabaseOrg >= 0, nFabbDatabaseOrg, 0),
                            .N_FabbisognoComplessivo = If(nFabbDatabase >= 0, nFabbDatabase, 0) * row.Item("Sup_Imp"),
                            .N_FabbisognoSoddisfatto = 0,
                            .N_Zootecnico = 0,
                            .LimiteMas = 0
                        }

                        Dim objParticella As New PUA_ParticellaVincoloAgronomico With {
                            .Part_PROV = row.Item("PROV"),
                            .Part_COM = row.Item("COM"),
                            .Part_SEZIONE = row.Item("SEZIONE"),
                            .Part_FOGLIO = row.Item("FOGLIO"),
                            .Part_NUMERO = row.Item("NUMERO"),
                            .Part_SUBALTERNO = row.Item("SUBALTERNO"),
                            .Part_PROVINCIA = row.Item("provincia"),
                            .Part_COMUNE = row.Item("comune"),
                            .Sup_Condotta = row.item("sup_condotta")
                        }

                        '------ Zone Vulnerabili (ZVN)
                        Dim flagVulnerabile As Boolean = IsZonaVulnerabile(dataInizio, dataFine,
                                                                           objParticella.Part_PROV, objParticella.Part_COM,
                                                                           objParticella.Part_SEZIONE, objParticella.Part_FOGLIO,
                                                                           objParticella.Part_NUMERO, objParticella.Part_SUBALTERNO,
                                                                           dtZvnServer, dtZvnMetaschema)

                        objParticella.ZVN = flagVulnerabile

                        'C'è almeno una particella vulnerabile, quindi tutto l'appezzamento viene considerato vulnerabile
                        If objParticella.ZVN = True Then
                            objPiano.ZVN = True
                        End If


                        objPiano.ParticelleVincoli.Add(objParticella)


                        '------ Catasto
                        objPiano.Catasto = chiavePart & If(objParticella.ZVN = True, VulnerabileStr, "")

                        listPiano.Add(objPiano)

                        dict.Add(chiaveDistinta, New List(Of String) From {chiavePart})

                    Else

                        'devo solo prendere la parte della particella
                        Dim idx As Integer = listPiano.FindIndex(Function(x) x.Chiave = chiaveDistinta)

                        Dim ParticellaEsistente = (From part In listPiano(idx).ParticelleVincoli Where part.Part_PROV = row.Item("PROV") And part.Part_COM = row.Item("COM") And part.Part_SEZIONE = row.Item("SEZIONE") And part.Part_FOGLIO = row.Item("FOGLIO") And part.Part_NUMERO = row.Item("NUMERO") And part.Part_SUBALTERNO = row.Item("SUBALTERNO") Select part).FirstOrDefault

                        If ParticellaEsistente Is Nothing Then

                            Dim objParticella As New PUA_ParticellaVincoloAgronomico With {
                            .Part_PROV = row.Item("PROV"),
                            .Part_COM = row.Item("COM"),
                            .Part_SEZIONE = row.Item("SEZIONE"),
                            .Part_FOGLIO = row.Item("FOGLIO"),
                            .Part_NUMERO = row.Item("NUMERO"),
                            .Part_SUBALTERNO = row.Item("SUBALTERNO"),
                            .Part_PROVINCIA = row.Item("provincia"),
                            .Part_COMUNE = row.Item("comune"),
                            .Sup_Condotta = row.item("sup_condotta")
                        }

                            '------ Zone Vulnerabili (ZVN)
                            Dim flagVulnerabile As Boolean = IsZonaVulnerabile(dataInizio, dataFine,
                                                                           objParticella.Part_PROV, objParticella.Part_COM,
                                                                           objParticella.Part_SEZIONE, objParticella.Part_FOGLIO,
                                                                           objParticella.Part_NUMERO, objParticella.Part_SUBALTERNO,
                                                                           dtZvnServer, dtZvnMetaschema)

                            objParticella.ZVN = flagVulnerabile

                            'C'è almeno una particella vulnerabile, quindi tutto l'appezzamento viene considerato vulnerabile
                            If objParticella.ZVN = True Then
                                listPiano(idx).ZVN = True
                            End If

                            listPiano(idx).Catasto = listPiano(idx).Catasto & "|" & chiavePart & If(objParticella.ZVN = True, VulnerabileStr, "")
                            listPiano(idx).ParticelleVincoli.Add(objParticella)

                        End If

                    End If

                    count += 1

                Next

            End If




            '----------------------------------------------------------------------------------------------------
            If ChiamaWS = True Then
                If modalita = enum_PUA_Modalita.Modalita_Verifica Then

                    For Each item As PUA_Appezzamento In listPiano
                        AssegnaSemafori(item)
                    Next

                End If
            End If

            If dt.Rows.Count = 0 And dtTN.Rows.Count = 0 Then
                Scrivi_LOG(objParametriServer, "CaricaPiano", "Pua NON importato - Piva " & piva & " - pua_cod " & puaCod)
            End If

            '----------------------------------------------------------------------------------------------------

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            r.RispostaStringa = JsonConvert.SerializeObject(listPiano, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    Private Shared Sub AssegnaSemafori(ByRef objPiano As PUA_Appezzamento)

        ' Bilancio N Utile 
        objPiano.N_BilancioAzotato_Utile = objPiano.N_FabbisognoSoddisfatto - objPiano.N_Fabbisogno

        Select Case objPiano.N_BilancioAzotato_Utile
            Case <= 0
                objPiano.Valutazione_NUtile = 0' SemaforoValutazione.Verde
            Case 0 To 30
                objPiano.Valutazione_NUtile = 0 ' SemaforoValutazione.Arancione
            Case Else
                objPiano.Valutazione_NUtile = 1 ' SemaforoValutazione.Rosso
        End Select


        ' Bilancio N Totale
        objPiano.N_BilancioAzotato_Totale = objPiano.N_TotaleSoddisfatto - objPiano.N_Fabbisogno

        Select Case objPiano.N_BilancioAzotato_Totale
            Case <= 0
                objPiano.Valutazione_NTotale = 0' SemaforoValutazione.Verde
            Case 0 To 50
                objPiano.Valutazione_NTotale = 0 ' SemaforoValutazione.Arancione
            Case Else
                objPiano.Valutazione_NTotale = 1 ' SemaforoValutazione.Rosso
        End Select

        'valutazione efficienza
        If objPiano.N_TotaleSoddisfatto <> 0 Then
            objPiano.Indice_Efficienza_Azotata = objPiano.Assorbimento / objPiano.N_TotaleSoddisfatto * 100
        End If

        Select Case objPiano.Indice_Efficienza_Azotata
            Case 0
                objPiano.Valutazione_Efficienza = 0' SemaforoValutazione.Verde
            Case >= 50
                objPiano.Valutazione_Efficienza = 0 ' SemaforoValutazione.Verde
            Case Else
                objPiano.Valutazione_Efficienza = 1 ' SemaforoValutazione.Rosso
        End Select

    End Sub

    Private Shared Sub LeggiZoneVulnerabili(ByVal regCod As Integer, ByVal dataInizio As Date, ByVal dataFine As Date,
                                            ByRef dtZvnServer As DataTable,
                                            ByRef dtZvnMetaschema As DataTable,
                                            ByRef objParametriServer As AgronicaCoreParametri)

        Dim filtroDate As String = " (Validita_inizio <= " & Agro_SQL_SaveDate(dataFine) & ")  AND     (Validita_Fine >= " & Agro_SQL_SaveDate(dataInizio) & ") "

        'lettura zone vulnerabili
        Dim objPV As New AgronicaCoreAnagrafeDAL.ZonexParticelle_R
        dtZvnServer = objPV.Leggi(-17,
                                  "", "", "", 0, 0, "",
                                  enumSelezioneVariabile.Selezione_TabellaCompleta,
                                  filtroDate,
                                  "", objParametriServer)


        '----------------------------------------
        'lettura fasce (la metto in un try catch in caso non esista la tabella nel DB PianoConcimazione_Pua e di conseguenza la vista)
        Try
            'TODO: introdurre lettura zone vulnerabili PUA tramite WebService e non in locale?!?
            Dim objPVF As New AgronicaCoreMetaSchemaDAL.ParticelleCatastali_Vulnerabili_R
            dtZvnMetaschema = objPVF.Leggi("", "", "", 0, 0, "",
                                           regCod,
                                           " Fascia_Cod <>0 AND " & filtroDate,
                                           "", objParametriServer)
        Catch ex As Exception

        End Try

    End Sub

    Private Shared Function IsZonaVulnerabile(ByVal dataInizio As Date, ByVal dataFine As Date,
                                              ByVal prov As String, ByVal com As String,
                                              ByVal sezione As String, ByVal foglio As Integer,
                                              ByVal numero As Integer, ByVal subalterno As String,
                                              ByRef dtZvnServer As DataTable, ByRef dtZvnMetaschema As DataTable
                                              ) As Boolean

        Dim flagVulnerabile As Boolean = False

        If Not dtZvnServer Is Nothing AndAlso dtZvnServer.Rows.Count > 0 Then

            flagVulnerabile = dtZvnServer.AsEnumerable().Any(Function(x) x.Item("PROV") = prov AndAlso
                                                                         x.Item("COM") = com AndAlso
                                                                         x.Item("SEZIONE") = sezione AndAlso
                                                                         x.Item("FOGLIO") = foglio AndAlso
                                                                         x.Item("NUMERO") = numero AndAlso
                                                                         x.Item("SUBALTERNO") = subalterno AndAlso
                                                                         x.Item("validita_inizio") <= dataFine AndAlso
                                                                         x.Item("validita_fine") >= dataInizio)

        End If

        If flagVulnerabile = False AndAlso
           Not dtZvnMetaschema Is Nothing AndAlso dtZvnMetaschema.Rows.Count > 0 Then

            'Se non l'ho trovato nel server lo cerco nel metaschema

            flagVulnerabile = dtZvnMetaschema.AsEnumerable().Any(Function(x) x.Item("PROV") = prov AndAlso
                                                                             x.Item("COM") = com AndAlso
                                                                             x.Item("SEZIONE") = sezione AndAlso
                                                                             x.Item("FOGLIO") = foglio AndAlso
                                                                             x.Item("NUMERO") = numero AndAlso
                                                                             x.Item("SUBALTERNO") = subalterno AndAlso
                                                                             x.Item("validita_inizio") <= dataFine AndAlso
                                                                             x.Item("validita_fine") >= dataInizio)

        End If

        Return flagVulnerabile

    End Function

    Private Shared Function GetListaCoefficienteB(ByVal regolamentoCod As Integer) As List(Of PUA_CoefficienteB)

        Dim objParametriIngresso As New PUA_CoefficienteB_Coltura_input With {
            .Regolamento_Cod = regolamentoCod
        }

        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        Dim objParametriUscita As PUA_CoefficienteB_Coltura_output = objPC_WS.CoefficienteB_Coltura(objParametriIngresso)

        Return objParametriUscita.ListaCoefficienteB

    End Function

    Private Shared Function GetCoefficienteB(ByVal listaCoefficienteB As List(Of PUA_CoefficienteB),
                                             ByVal Veg_Cod As Integer, ByVal Grfi_Cod_RER As Integer,
                                             ByRef Base As Decimal, ByRef Coeff_Incr As Decimal
                                             ) As Decimal

        Dim B_Perc As Decimal = 0

        If Not listaCoefficienteB Is Nothing AndAlso listaCoefficienteB.Count > 0 Then
            Dim obj = (From l In listaCoefficienteB Where l.Veg_Cod = Veg_Cod And l.Grfi_Cod_RER = Grfi_Cod_RER Select l).FirstOrDefault()
            If Not obj Is Nothing Then
                B_Perc = obj.B_Perc
                Base = obj.Base
                Coeff_Incr = obj.Coeff_Incr
            End If
        End If

        Return B_Perc

    End Function
    Private Shared Sub Scrivi_LOG(
                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                        ByVal NomeRoutine As String,
                        ByVal MessaggioErrore As String)

        Dim DefaultDirectoryLOG As String = "C:\Agronica_LOG"
        Dim DefaultFileLOG As String = "AgronicaCoreLOG.txt"

        Dim NomeCompletoFileLOG As String = ""
        Dim xStreamWriter As System.IO.StreamWriter = Nothing
        Dim Testo As String = ""

        Try

            'Verifico se e' stata indicata una directory di LOG
            If objParametri.LogDirectory = "" Then
                objParametri.LogDirectory = DefaultDirectoryLOG
            End If

            'Verifico se e' stato indicato un file di LOG
            If objParametri.LogFileName = "" Then
                objParametri.LogFileName = DefaultFileLOG
            End If

            'Verifico se esiste la DIRECTORY di LOG indicata ... altrimenti la creo
            If System.IO.Directory.Exists(objParametri.LogDirectory) = False Then
                System.IO.Directory.CreateDirectory(objParametri.LogDirectory)
            End If

            'Costruisco il nome completo del file di LOG
            NomeCompletoFileLOG = (objParametri.LogDirectory & "\" & objParametri.LogFileName).Replace("\\", "\")

            'Verifico se esiste il FILE di LOG indicato ... altrimenti lo creo
            If System.IO.File.Exists(NomeCompletoFileLOG) = False Then

                'Creo il file di LOG nuovo
                xStreamWriter = System.IO.File.CreateText(NomeCompletoFileLOG)
                xStreamWriter.WriteLine("Inizializzazione file di LOG ... " &
                                    Date.Now.ToShortDateString & " " &
                                    Date.Now.ToLongTimeString)
                xStreamWriter.Flush()
                xStreamWriter.Close()

            End If

            'Costruisco la stringa di testo da scrivere
            Testo = Date.Now.ToShortDateString &
                    " " &
                    Date.Now.ToLongTimeString &
                    " {" &
                    objParametri.LogDescrizioneUtente &
                    "} : [" &
                    NomeRoutine &
                    "] : " &
                    MessaggioErrore


            'Apro il file di LOG
            xStreamWriter = System.IO.File.AppendText(NomeCompletoFileLOG)

            'Scrivo la stringa
            xStreamWriter.WriteLine(Testo)
            xStreamWriter.Flush()
            'xStreamWriter.Close()

        Catch ex As Exception

            Throw New Exception("LOG : " & ex.Message)

        Finally

            If Not IsNothing(xStreamWriter) Then
                xStreamWriter.Close()
            End If

        End Try

    End Sub

    ' a differenza della precedente, se il pua è presente su pua_elaborazione, prendo li i dati
    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiPUA_Testata(imprese As String, dataInizio As String, dataFine As String, filtroUltima As Boolean) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")


        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) OrElse IsNothing(objParametri_Utenti) OrElse IsNothing(objParametri_Super_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim strPua As String = ""

        Try



            '----------------------------------------------------------------
            '--- Filtro associato all'utente 
            '----------------------------------------------------------------
            Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
            Dim UtenteProfiloImpreseSql As String = ""
            Dim UtenteProfiloCentriSql As String = ""
            Dim DtImpreseVisibili As DataTable
            Dim i As Integer

            DtImpreseVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Impresa, "", "", objParametri_Server)
            If Not DtImpreseVisibili Is Nothing Then
                For i = 0 To DtImpreseVisibili.Rows.Count - 1
                    UtenteProfiloImpreseSql &= "'" & DtImpreseVisibili.Rows(i).Item("Piva") & "',"
                Next
                If UtenteProfiloImpreseSql <> "" Then
                    UtenteProfiloImpreseSql = " IMP.piva IN (" & Left(UtenteProfiloImpreseSql, UtenteProfiloImpreseSql.Length - 1) & ") "
                End If
            End If

            Dim tipoImpresaGerarchia As Integer = tipoImpresaGerarchia


            Dim impreseList As New List(Of String)
            Dim imp = imprese.Split("|").ToList
            If imp.Contains("-1") Then
                impreseList.Add("-1")
            Else
                For Each value In imp
                    If value <> "" Then
                        impreseList.Add(value)
                    End If
                Next
            End If

            Dim data_Inizio As Date
            Dim data_Fine As Date
            If dataInizio IsNot Nothing AndAlso dataInizio <> "" Then
                If dataInizio.Split("/").Length = 3 Then
                    data_Inizio = CDate(dataInizio)
                Else
                    data_Inizio = New Date(CInt(dataInizio), 1, 1)
                End If
            Else
                data_Inizio = New Date(1900, 1, 1)
            End If

            If dataFine IsNot Nothing AndAlso dataFine <> "" Then
                If dataFine.Split("/").Length = 3 Then
                    data_Fine = CDate(dataFine)
                Else
                    data_Fine = New Date(CInt(dataFine), 12, 31)
                End If
            Else
                data_Fine = New Date(2100, 12, 31)
            End If

            Dim regolamentoCod As Integer = 125

            Dim objEffluentiInput As New PUA_Effluenti_input With {
                    .Regolamento_Cod = regolamentoCod
                }
            Dim objPC As New AgronicaCoreWebService.PianoConcimazione_WS
            Dim objEffluentiOutput As New PUA_Effluenti_output
            objEffluentiOutput = objPC.Effluenti(objEffluentiInput, objParametri_Server, objParametri_Super_Server)

            Dim listaCoefficienteB As List(Of PUA_CoefficienteB)
            listaCoefficienteB = GetListaCoefficienteB(regolamentoCod)

            Dim dtZvnServer As DataTable = Nothing
            Dim dtZvnMetaschema As DataTable = Nothing
            LeggiZoneVulnerabili(regolamentoCod, AGRODATAINIZIO, AGRODATAFINE, dtZvnServer, dtZvnMetaschema, objParametri_Server)

            Dim dtPua As DataTable
            Dim objP As New AgronicaCorePUA_DAL.PUA_Testata_R

            Dim Piva As String = "" ' "01704430519" '"01631160544"
            dtPua = objP.LeggiTestata_Da_GerarchiaImpresa(impreseList, Piva, data_Inizio, data_Fine, " pt.regolamento_cod=" & regolamentoCod, "rag_soc", objParametri_Server, objParametri_Utenti, filtroUltima)


            Dim dtPuaEleborazione As DataTable
            Dim objPuaEleborazione As New AgronicaCorePUA_DAL.Pua_Elaborazione_R
            dtPuaEleborazione = objPuaEleborazione.Leggi_Da_GerarchiaImpresa(impreseList, Piva, data_Inizio, data_Fine, " pt.regolamento_cod=" & regolamentoCod, "rag_soc", objParametri_Server, objParametri_Utenti, filtroUltima)


            Dim listPiani As New List(Of PUA_Appezzamento)
            Dim listPiano As New List(Of PUA_Appezzamento)

            Dim DtRis As New DataTable
            Dim Dr As DataRow

            DtRis.Columns.Add(New DataColumn("id", GetType(Integer)))
            DtRis.Columns.Add(New DataColumn("pua_cod", GetType(Integer)))
            DtRis.Columns.Add(New DataColumn("regolamento_cod", GetType(Integer)))

            DtRis.Columns.Add(New DataColumn("validita_inizio", GetType(Date)))
            DtRis.Columns.Add(New DataColumn("validita_fine", GetType(Date)))

            DtRis.Columns.Add(New DataColumn("cuaa", GetType(String)))
            DtRis.Columns.Add(New DataColumn("piva", GetType(String)))
            DtRis.Columns.Add(New DataColumn("ragione_sociale", GetType(String)))

            DtRis.Columns.Add(New DataColumn("appezzamento", GetType(String)))
            DtRis.Columns.Add(New DataColumn("superficie", GetType(Decimal)))
            DtRis.Columns.Add(New DataColumn("specie", GetType(String)))

            DtRis.Columns.Add(New DataColumn("provincia", GetType(String)))
            DtRis.Columns.Add(New DataColumn("comune", GetType(String)))
            DtRis.Columns.Add(New DataColumn("istat_provincia", GetType(String)))
            DtRis.Columns.Add(New DataColumn("istat_comune", GetType(String)))
            DtRis.Columns.Add(New DataColumn("sezione", GetType(String)))
            DtRis.Columns.Add(New DataColumn("foglio", GetType(Integer)))
            DtRis.Columns.Add(New DataColumn("numero", GetType(Integer)))
            DtRis.Columns.Add(New DataColumn("subalterno", GetType(String)))
            DtRis.Columns.Add(New DataColumn("sup_particella", GetType(Decimal)))

            DtRis.Columns.Add(New DataColumn("zvn_si", GetType(String)))
            DtRis.Columns.Add(New DataColumn("zvn_no", GetType(String)))

            DtRis.Columns.Add(New DataColumn("conforme_si", GetType(String)))
            DtRis.Columns.Add(New DataColumn("conforme_no", GetType(String)))
            DtRis.Columns.Add(New DataColumn("compilatore", GetType(String)))

            Dim r_pua As RispostaStandard

            Dim id As Integer = 0

            Dim objPW As New AgronicaCorePUA_DAL.Pua_Elaborazione_W

            For Each row In dtPua.Rows

                'verifico se già loggata la verifica bilancio
                Dim DrPua() As DataRow
                DrPua = dtPuaEleborazione.Select("pua_cod=" & row.Item("pua_cod") & " and regolamento_cod=" & row.Item("regolamento_cod") & " and piva='" & row.Item("piva") & "'")

                If Not DrPua Is Nothing AndAlso DrPua.Length > 0 Then

                    For Each Drp In DrPua

                        Dr = DtRis.NewRow

                        Dr.Item("id") = id

                        Dr.Item("pua_cod") = Drp("pua_cod")
                        Dr.Item("regolamento_cod") = Drp("regolamento_cod")

                        Dr.Item("validita_inizio") = Drp("validita_inizio")
                        Dr.Item("validita_fine") = Drp("validita_fine")

                        Dr.Item("cuaa") = Drp("cuaa")
                        Dr.Item("piva") = Drp("piva")
                        Dr.Item("ragione_sociale") = Drp("rag_soc")

                        Dr.Item("appezzamento") = Drp("app_nome")
                        Dr.Item("superficie") = Drp("sup_imp")
                        Dr.Item("specie") = Drp("veg_des")

                        Dr.Item("istat_provincia") = Drp("prov")
                        Dr.Item("istat_comune") = Drp("com")
                        Dr.Item("sezione") = Drp("sezione")
                        Dr.Item("foglio") = Drp("foglio")
                        Dr.Item("numero") = Drp("numero")
                        Dr.Item("subalterno") = Drp("subalterno")
                        Dr.Item("provincia") = Drp("provincia")
                        Dr.Item("comune") = Drp("comune")
                        Dr.Item("sup_particella") = Drp("Sup_Condotta")

                        If Drp("zvn") = 1 Then
                            Dr.Item("zvn_si") = "X"
                            Dr.Item("zvn_no") = ""
                        Else
                            Dr.Item("zvn_si") = ""
                            Dr.Item("zvn_no") = "X"
                        End If

                        'se almeno un valore non è conforme
                        If Drp("Valutazione_N_Utile") = 1 Or Drp("Valutazione_N_Totale") = 1 Or Drp("Valutazione_Efficienza_Azotata") = 1 Then
                            Dr.Item("conforme_si") = ""
                            Dr.Item("conforme_no") = "X"
                        Else
                            Dr.Item("conforme_no") = ""
                            Dr.Item("conforme_si") = "X"
                        End If

                        Dr.Item("compilatore") = Drp("compilatore")

                        DtRis.Rows.Add(Dr)

                        id += 1

                    Next


                Else

                    r_pua = CaricaGrigliaPianoDistribuzione(row.Item("piva"), row.Item("regolamento_cod"), row.Item("pua_cod"), row.Item("pua_tipo"),
                                                    row.Item("Validita_inizio"), row.Item("Validita_Fine"),
                                                    enum_PUA_Modalita.Modalita_Verifica, 100,
                                                    objEffluentiOutput, listaCoefficienteB,
                                                    dtZvnServer, dtZvnMetaschema)

                    If r_pua.RispostaOK = True Then

                        Dim obj_Dati As PUA_Appezzamento()
                        obj_Dati = JsonConvert.DeserializeObject(Of PUA_Appezzamento())(r_pua.RispostaStringa)

                        For Each a As PUA_Appezzamento In obj_Dati

                            For Each p As PUA_ParticellaVincoloAgronomico In a.ParticelleVincoli

                                Dr = DtRis.NewRow

                                Dr.Item("id") = id

                                Dr.Item("pua_cod") = row.Item("pua_cod")
                                Dr.Item("regolamento_cod") = row.Item("regolamento_cod")

                                Dr.Item("validita_inizio") = row.item("validita_inizio")
                                Dr.Item("validita_fine") = row.item("validita_fine")

                                Dr.Item("cuaa") = row.Item("cuaa")
                                Dr.Item("piva") = row.Item("piva")
                                Dr.Item("ragione_sociale") = row.Item("rag_soc")

                                Dr.Item("appezzamento") = Split(a.Appezzamento, " - ")(0)
                                Dr.Item("superficie") = a.Superficie
                                Dr.Item("specie") = Split(a.Appezzamento, " - ")(1)

                                'Dr.Item("catasto") = a.Catasto
                                Dr.Item("istat_provincia") = p.Part_PROV
                                Dr.Item("istat_comune") = p.Part_COM
                                Dr.Item("sezione") = p.Part_SEZIONE
                                Dr.Item("foglio") = p.Part_FOGLIO
                                Dr.Item("numero") = p.Part_NUMERO
                                Dr.Item("subalterno") = p.Part_SUBALTERNO
                                Dr.Item("provincia") = p.Part_PROVINCIA
                                Dr.Item("comune") = p.Part_COMUNE
                                Dr.Item("sup_particella") = p.Sup_Condotta

                                If a.ZVN = True Then
                                    Dr.Item("zvn_si") = "X"
                                    Dr.Item("zvn_no") = ""
                                Else
                                    Dr.Item("zvn_si") = ""
                                    Dr.Item("zvn_no") = "X"
                                End If

                                'se almeno un valore non è conforme
                                If a.Valutazione_NUtile = 1 Or a.Valutazione_NTotale = 1 Or a.Valutazione_Efficienza = 1 Then
                                    Dr.Item("conforme_si") = ""
                                    Dr.Item("conforme_no") = "X"
                                Else
                                    Dr.Item("conforme_no") = ""
                                    Dr.Item("conforme_si") = "X"
                                End If

                                Dr.Item("compilatore") = row.Item("compilatore")


                                DtRis.Rows.Add(Dr)

                                id += 1

                            Next

                            If a.Id_AnagrafeVincoli > 0 Then

                                objPW.Scrivi(row.Item("pua_cod"), row.Item("regolamento_cod"), row.Item("pua_tipo"),
                                                  a.Id_AnagrafeVincoli,
                                                  row.Item("cuaa"), row.Item("piva"), a.Sa_Cod,
                                                    a.Campo_Cod, a.appezza, a.id_reg, a.Progetto_Cod,
                                                    IIf(a.Veg_Cod < 0, 0, a.Veg_Cod), 0, IIf(a.Veg_Cod < 0, Math.Abs(a.Veg_Cod), 0),
                                                    a.Grfi_Cod, a.Grfi_Cod_Concimazione, a.StatoImpiantoCod, a.Ciclo,
                                                    a.Resa, a.Resa_Rif, a.FattoreCorrettivo_N, a.Superficie, IIf(a.ZVN = True, 1, 0),
                                                    a.N_Fabbisogno_Database, a.N_Fabbisogno, a.LimiteMas,
                                                    a.N_FabbisognoSoddisfatto, a.N_TotaleSoddisfatto, a.N_Zootecnico, a.N_Zootecnico_Letame, a.N_Zootecnico_Liquame,
                                                    a.N_BilancioAzotato_Utile, a.N_BilancioAzotato_Totale, a.Indice_Efficienza_Azotata,
                                                    a.Valutazione_NUtile, a.Valutazione_NTotale, a.Valutazione_Efficienza,
                                                    IIf(a.Valutazione_NUtile = 1 Or a.Valutazione_NTotale = 1 Or a.Valutazione_Efficienza = 1, 0, 1),
                                                    row.Item("Validita_inizio"), row.Item("Validita_Fine"), objParametri_Server)

                            End If

                        Next

                    End If


                End If


            Next

            ''percorso e nome del file da esportare
            'Dim nomeFileUnivoco As String = "Estrazione_PUA_" & AgronicaCoreUtility.FileSystemHelper.NomeFileUnivoco(".xlsx")
            'Dim _AgroWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig

            'Dim path_file As String = _AgroWebConfig.GestioneAllegati_Repository
            'If Not path_file.EndsWith("\") Then
            '    path_file &= "\"
            'End If
            'path_file &= nomeFileUnivoco

            DtRis.TableName = "Esportazione"

            strPua = JSON_DataTable_Pua(DtRis)

            r.RispostaOK = True
            r.RispostaStringa = strPua

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r

    End Function

    Private Shared Function JSON_DataTable_Pua(ByRef DT As DataTable,
                                                               Optional ByVal stringaKendoRow As String = "") As String

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)

        l.Add(New ColonneNome("id", "id", "number") With {._hidden = True})
        l.Add(New ColonneNome("pua_cod", "pua_Cod", "number") With {._hidden = True})
        l.Add(New ColonneNome("regolamento_cod", "regolamento_cod", "number") With {._hidden = True})
        l.Add(New ColonneNome("validita_inizio", "Data Inizio", "date"))
        l.Add(New ColonneNome("validita_fine", "Data Fine", "date"))
        l.Add(New ColonneNome("piva", "Partita Iva", "string"))
        l.Add(New ColonneNome("cuaa", "Cuaa", "string"))
        l.Add(New ColonneNome("ragione_sociale", "Ragione Sociale", "string"))
        l.Add(New ColonneNome("appezzamento", "Appezzamento", "string"))
        l.Add(New ColonneNome("superficie", "Sup. [ha]", "number") With {._formatNr = "n4", ._css = "allineadestra", ._cssHeader = "allineadestra"})
        l.Add(New ColonneNome("specie", "Specie", "string"))

        l.Add(New ColonneNome("provincia", "Provincia", "string"))
        l.Add(New ColonneNome("comune", "Comune", "string"))
        l.Add(New ColonneNome("sezione", "sezione", "string"))
        l.Add(New ColonneNome("foglio", "foglio", "number"))
        l.Add(New ColonneNome("numero", "numero", "number"))
        l.Add(New ColonneNome("subalterno", "subalterno", "string"))
        l.Add(New ColonneNome("sup_particella", "Sup. Particella [Ha]", "number") With {._formatNr = "n4", ._css = "allineadestra", ._cssHeader = "allineadestra"})

        l.Add(New ColonneNome("zvn_si", "zvn_si", "string"))
        l.Add(New ColonneNome("zvn_no", "zvn_no", "string"))

        l.Add(New ColonneNome("conforme_si", "conforme_si", "string"))
        l.Add(New ColonneNome("conforme_no", "conforme_no", "string"))

        l.Add(New ColonneNome("compilatore", "compilatore", "string"))


        'l.Add(New ColonneNome("zvn", "zvn", "number") With {._hidden = True})
        'l.Add(New ColonneNome("zvn_des", "zvn_des", "string") With {._hidden = True})
        'l.Add(New ColonneNome("conforme", "conforme", "number") With {._hidden = True})
        'l.Add(New ColonneNome("conforme_des", "conforme_des", "string") With {._hidden = True})

        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        js.Editabile_Deafault = True

        Dim risp As String = js.JSON_DataTable_Kendo(DT, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True, tipoFiltroKendo:=TipoFiltroKendo_colonne.Menu,
                                           stringaKendoRow:=stringaKendoRow) 'True, True, TipoFiltroKendo_colonne.CasellaTesto) '

        Return risp

    End Function


End Class