Imports System.Web.Services
Imports Newtonsoft.Json
Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDTOStd.InData.Widgets
Imports AgronicaCoreModelsSTD.Widgets
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreMapper
Imports System.Threading.Tasks
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreUtentiDAL
Imports System.Globalization
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreModelsSTD
Imports AgronicaCoreModelsSTD.attivita.risorse
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class Widgets_Dati
    Inherits System.Web.Services.WebService

    Private Const NOME_GRUPPO_CACHE As String = "WIDGETS_CAHCE"
    Private Const CACHE_KEY_PREFIX As String = "WIDGETS"

    <WebMethod(EnableSession:=True)>
    Public Function LeggiMovimentiMagazzino(ByVal InData As Object) As rispostaStandard(Of List(Of Widget_MovimentoMagazzino))

        Dim r As New rispostaStandard(Of List(Of Widget_MovimentoMagazzino))

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
        Dim params As CoreWS_Generic(Of Widget_MovimentiMagazziono_In) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of Widget_MovimentiMagazziono_In))(JsonConvert.SerializeObject(InData), a)

        If params.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If params.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Dim objParametri_Super_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_super_server)
        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_utenti)

        ' Servono per istanziare AgroWebConfig
        HttpContext.Current.Session("ASG_objParametri_Super_Server") = objParametri_Super_Server
        HttpContext.Current.Session("ASG_objParametri_Server") = objParametri_Server
        HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametri_Utenti

        Dim topNRows As Integer = 5
        If params.InData.NumeroMovimenti > 0 Then
            topNRows = params.InData.NumeroMovimenti
        End If

        Dim Piva = params.InData.Piva
        Dim objMovDet As New AgronicaCoreContabDAL.Movimenti_Dettagli_R()
        Dim dt As DataTable = objMovDet.Leggi(Piva, 0, 0, 0, 0,
                                              0, 0, 0, CAU_SCARICO, 0, 0, 0, 0, 0, 0,
                                              AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta, " Agenda.LAV_COD < 1000 ", " Movimenti.Data_Movimento DESC,  Id_agenda desc ", objParametri_Server, topNRows)

        Dim listaMovimenti As New List(Of Widget_MovimentoMagazzino)

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then

            Dim leggiAnagrafeLog As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Moduli_Log_R
            Dim objMovimenti As New AgronicaCoreContabDAL.Movimenti_R
            Dim objGruppiMerce As New Gruppi_Merce_R
            Dim ListaGruppiMercePerCategoria As List(Of ImpostazioneDefault_GruppiMerce) = objGruppiMerce.GetListGruppiMerceDefault(Piva, SACOD_NOFILTRO, objParametri_Utenti, objParametri_Server)
            Dim gestioneGruppiMerce As Boolean = ListaGruppiMercePerCategoria.Count > 0

            For Each row As DataRow In dt.Rows

                Dim DataMov As DateTime = IIf(row.Item("Data_Movimento") Is DBNull.Value, DateTime.Now, Convert.ToDateTime(row.Item("Data_Movimento")))
                Dim Sa_Cod As Integer = IIf(row.Item("Sa_Cod") Is DBNull.Value, 0, Convert.ToInt32(row.Item("Sa_Cod")))
                Dim Elem_Cod As Integer = IIf(row.Item("Elem_Cod") Is DBNull.Value, 0, Convert.ToInt32(row.Item("Elem_Cod")))
                Dim Pro_Cod As Integer = IIf(row.Item("Pro_Cod") Is DBNull.Value, 0, Convert.ToInt32(row.Item("Pro_Cod")))
                Dim Mat_Cod As Integer = IIf(row.Item("Mat_Cod") Is DBNull.Value, 0, Convert.ToInt32(row.Item("Mat_Cod")))
                Dim Udm_Cod As Integer = IIf(row.Item("Udm_Cod") Is DBNull.Value, 0, Convert.ToInt32(row.Item("Udm_Cod")))
                Dim Id_Agenda As Integer = Convert.ToInt32(row.Item("Id_Agenda"))
                Dim Lotto As String = IIf(row.Item("Lotto") Is DBNull.Value, "", row.Item("Lotto").ToString)
                Dim isFreshAndFood As Boolean = leggiAnagrafeLog.IsFreshAndFood(Piva, Elem_Cod, objParametri_Server)
                Dim stringaFilter As String = " AND Agenda.Id_Agenda = " + Id_Agenda.ToString

                Dim dtScheda As DataTable = objMovimenti.SchedaMovimentiMagazzino(DataMov,
                                                                                 DataMov,
                                                                                 Piva,
                                                                                 0,
                                                                                 0,
                                                                                 Elem_Cod,
                                                                                 Pro_Cod,
                                                                                 Mat_Cod,
                                                                                 0,
                                                                                 0,
                                                                                 0,
                                                                                 Udm_Cod,
                                                                                 Lotto,
                                                                                 stringaFilter, "", "", "", "", "", "", "", "", "", "",
                                                                                 " Movimenti.Data_Movimento DESC ",
                                                                                 objParametri_Server, objParametri_Utenti,
                                                                                 flagRecuperaCodArticolo:=True,
                                                                                 codArticolo:="",
                                                                                 cercaCodArticoloPerLike:=True,
                                                                                 isFreshAndFood:=isFreshAndFood,
                                                                                 gruppiMerceDefaultPerCategoria:=ListaGruppiMercePerCategoria)

                If Not IsNothing(dtScheda) AndAlso dtScheda.Rows.Count > 0 Then
                    For Each rowScheda As DataRow In dtScheda.Rows
                        Dim mm As New Widget_MovimentoMagazzino With
                        {
                            .Piva = Piva,
                            .Id_Agenda = Id_Agenda,
                            .Sa_cod = .Udm = IIf(rowScheda.Item("Sa_cod") Is DBNull.Value, 0, Convert.ToInt32(rowScheda.Item("Sa_cod"))),
                            .Data_Movimento = Convert.ToDateTime(rowScheda.Item("Data_Movimento")),
                            .Des_Lib = rowScheda.Item("Des_Lib").ToString,
                            .Qta = Convert.ToDecimal(rowScheda.Item("Qta")) * -1,
                            .Udm = IIf(rowScheda.Item("Udm_Cod") Is DBNull.Value, 0, Convert.ToInt32(rowScheda.Item("Udm_Cod"))),
                            .Udm_Des = IIf(rowScheda.Item("Udm_Des") Is DBNull.Value, "", rowScheda.Item("Udm_Des").ToString),
                            .Udm_Sim = IIf(rowScheda.Item("Udm_Sim") Is DBNull.Value, "", rowScheda.Item("Udm_Sim").ToString),
                            .Id_Destinazione = IIf(rowScheda.Item("Id_Destinazione") Is DBNull.Value, 0, Convert.ToInt32(rowScheda.Item("Id_Destinazione"))),
                            .Fabbricato_Des = IIf(rowScheda.Item("Fabbricato_Des") Is DBNull.Value, "", rowScheda.Item("Fabbricato_Des").ToString),
                            .Cod_Articolo = IIf(rowScheda.Item("Cod_Articolo") Is DBNull.Value, "", rowScheda.Item("Cod_Articolo").ToString),
                            .Descrizione_Prodotto = IIf(rowScheda.Item("Descrizione_Prodotto") Is DBNull.Value, "", rowScheda.Item("Descrizione_Prodotto").ToString),
                            .Lotto = IIf(rowScheda.Item("Lotto") Is DBNull.Value, "", rowScheda.Item("Lotto").ToString),
                            .Elem_Cod = IIf(rowScheda.Item("Elem_Cod") Is DBNull.Value, 0, Convert.ToInt32(rowScheda.Item("Elem_Cod"))),
                            .Pro_Cod = IIf(rowScheda.Item("Pro_Cod") Is DBNull.Value, 0, Convert.ToInt32(rowScheda.Item("Pro_Cod"))),
                            .Mat_Cod = IIf(rowScheda.Item("Mat_Cod") Is DBNull.Value, 0, Convert.ToInt32(rowScheda.Item("Mat_Cod")))
                        }
                        listaMovimenti.Add(mm)

                    Next
                End If

            Next

            If listaMovimenti.Any Then
                Dim objGiacenze As New AgronicaCoreContabDAL.Giacenze_R

                For Each mov In listaMovimenti
                    Dim idDest As Integer = mov.Id_Destinazione
                    Dim Pro_Cod As Integer = mov.Pro_Cod
                    Dim Mat_Cod As Integer = mov.Mat_Cod
                    Dim Elem_Cod As Integer = mov.Elem_Cod

                    Dim dtGiacenze As DataTable
                    dtGiacenze = objGiacenze.SchedaGiacenzeMagazzino(DateTime.Now,
                                                                        Piva,
                                                                        0,
                                                                        idDest,
                                                                        Elem_Cod, Pro_Cod, Mat_Cod, 0, 0, 0, 0, LOTTO_NONDEFINITO,
                                                                        Flag_QtaNoZero:=False,
                                                                        "", "", "", "", "", "", "", "", "",
                                                                        "", "",
                                                                        "",
                                                                        objParametri_Server, objParametri_Utenti,
                                                                        flagRecuperaCodArticolo:=False,
                                                                        codArticolo:="",
                                                                        cercaCodArticoloPerLike:=False)

                    If Not dtGiacenze Is Nothing AndAlso dtGiacenze.Rows.Count > 0 Then
                        mov.Giacenza = Convert.ToDecimal(dtGiacenze.Rows(0).Item("Giacenza"))
                    End If
                Next


            End If

        End If

        r.RispostaOK = True
        r.RispostaStringa = listaMovimenti

        Return r

    End Function

    <WebMethod()>
    Public Function LeggiUltimiRilievi(ByVal InData As Object) As rispostaStandard(Of List(Of Widget_Operazione))

        Dim r As New rispostaStandard(Of List(Of Widget_Operazione))

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim params As CoreWS_Generic(Of Widget_Operazioni_IN) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of Widget_Operazioni_IN))(JsonConvert.SerializeObject(InData), a)

        If params.objP.objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If
        If params.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If
        If params.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Dim operazione As New List(Of Widget_Operazione)


        Dim widgetCache = MemoryCacheFactory.Instance.WIDGET_CACHE

        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_utenti)
            Dim topNRows As Integer = 5
            If Not IsNothing(params.InData.NumeroMovimenti) Then
                topNRows = params.InData.NumeroMovimenti
            End If

            Dim Piva As String = params.InData.Piva
            Dim filtroAttivita = "Rilievi"

            Dim objMov As New AgronicaCoreContabDAL.Movimenti_R
            Dim xFiltroMovimenti As String = " Agenda.Lav_Cod IN (" + params.InData.Filtro_Lav_Cod + ")"
            Dim xOrderBY As String = " Movimenti.Data_Movimento DESC, Id_Agenda DESC "
            Dim dt As DataTable = objMov.Leggi(Piva, 0, 0, 0,
                                               0, "", AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                xFiltroMovimenti, xOrderBY, objParametri_Server, topNRows)

            Dim agende = dt.ToExpandoObject()

            If agende.Any Then

                Dim listaAgendeNew As New List(Of OggettoConfronto)
                For Each agenda In agende
                    listaAgendeNew.Add(New OggettoConfronto With
                                    {
                                        .Id_agenda = agenda("Id_Agenda"),
                                        .Data_Modifica = agenda("Data_Modifica")
                                    })
                Next
                Dim cacheKey As String = Get_Cache_Key_Attivita(filtroAttivita, objParametri_Server.StringaConnessione, objParametri_Utenti.UtenteUsername, Piva)
                Dim ListaAttivitaCached As CacheAttivita = widgetCache.Get(cacheKey)

                Dim ricarica As Boolean = False

                If IsNothing(ListaAttivitaCached) Then
                    ricarica = True
                Else
                    If Not listaAgendeNew.SequenceEqual(ListaAttivitaCached.ListaConfronto) Then
                        ricarica = True
                    End If
                End If

                '' TODO (Check
                ricarica = True


                If ricarica Then

                    Dim listaAttivita As New List(Of AgronicaCoreModelsSTD.attivita.Attivita)
                    If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                        Dim objAgenda As New Agenda_Operazione_Helper
                        Dim agendaMapper As New AgendaToAttivita

                        Parallel.ForEach(dt.AsEnumerable, Sub(row As DataRow)

                                                              Dim pSServer = objParametri_Super_Server.CreateDeepCopy(objParametri_Super_Server)
                                                              Dim pServer = objParametri_Server.CreateDeepCopy(objParametri_Server)
                                                              Dim pUtenti = objParametri_Utenti.CreateDeepCopy(objParametri_Utenti)

                                                              Dim id_agenda As Integer = Convert.ToInt32(row.Item("Id_Agenda"))
                                                              Dim agenda As Operazione_Agenda = objAgenda.Leggi(params.InData.Piva, 0, id_agenda, 0, pServer)
                                                              Dim attivita As AgronicaCoreModelsSTD.attivita.Attivita = Nothing
                                                              If Not IsNothing(agenda) Then
                                                                  Try
                                                                      attivita = agendaMapper.AgendaSuAttivita(agenda, True, pSServer, pServer, pUtenti)
                                                                  Catch ex As Exception
                                                                  End Try
                                                              End If
                                                              If Not IsNothing(attivita) Then
                                                                  SyncLock (listaAttivita)
                                                                      listaAttivita.Add(attivita)
                                                                  End SyncLock
                                                              End If

                                                          End Sub)

                    End If

                    If Not IsNothing(listaAttivita) AndAlso listaAttivita.Any Then

                        Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
                        Dim permessoScrittura As Boolean = ObjUtenti.Controlla_Permessi_Utente(
                                objParametri_Utenti.UtenteUsername, 5,
                                enum_Security_Attivita.Agenda_AccessoMenu, enum_Security_Operazione.Modifica, Date.Now, "", objParametri_Utenti)


                        Dim terrenoConerter = New AgronicaCoreModelsSTD.metaschema.utilizzi.UtilizzoTerrenoClassConverter()
                        For Each att As AgronicaCoreModelsSTD.attivita.Attivita In listaAttivita

                            Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read

                            Dim persone As New List(Of String)
                            Dim macchine As New List(Of String)
                            Dim impianti As New List(Of String)
                            Dim impiantiMappe As New List(Of ImpiantiMappe)
                            Dim filtroImpiantiPerStaticMap = New List(Of Dictionary(Of String, Object))

                            Dim op = New Widget_Operazione
                            op.IdAgenda = CInt(att.codice)
                            op.LavCod = 0
                            If Not IsNothing(att.job) AndAlso Not IsNothing(att.job.primaryKey) Then
                                op.LavCod = If(String.IsNullOrEmpty(att.job.primaryKey.codice), 0, CInt(att.job.primaryKey.codice))
                            End If

                            op.Data_Operazione = att.inizio
                            op.Descrizione_Operazione = att.descrizione
                            op.Piva = params.InData.Piva
                            op.Specie = ""
                            If Not IsNothing(att.centroAziendale) Then
                                op.Rag_Soc = att.centroAziendale.nome
                            End If
                            If Not IsNothing(att.utilizzoTerreno) Then
                                Select Case att.utilizzoTerreno.classType.ToLowerInvariant
                                    Case "varieta"
                                        Dim varieta = DirectCast(att.utilizzoTerreno, AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta)
                                        If Not IsNothing(varieta) AndAlso Not IsNothing(varieta.specie) Then
                                            op.Specie = varieta.specie.descrizione
                                        End If
                                End Select
                            End If
                            If Not IsNothing(att.risorse) AndAlso att.risorse.Any Then
                                For Each ris As AgronicaCoreModelsSTD.attivita.risorse.Risorsa In att.risorse
                                    Select Case ris.classType.ToLowerInvariant()
                                        Case "risorsamacchina"
                                            Dim macchina As AgronicaCoreModelsSTD.attivita.risorse.RisorsaMacchina = DirectCast(ris, AgronicaCoreModelsSTD.attivita.risorse.RisorsaMacchina)
                                            If Not IsNothing(macchina) AndAlso Not IsNothing(macchina.macchina) Then
                                                macchine.Add(macchina.macchina.descrizione)
                                            End If
                                        Case "risorsapersona"
                                            Dim persona As AgronicaCoreModelsSTD.attivita.risorse.RisorsaPersona = DirectCast(ris, AgronicaCoreModelsSTD.attivita.risorse.RisorsaPersona)
                                            If Not IsNothing(persona) AndAlso Not IsNothing(persona.risorsaUmana) AndAlso Not IsNothing(persona.risorsaUmana.contatto) Then
                                                If Not String.IsNullOrEmpty(persona.risorsaUmana.contatto.ragione_Sociale) Then
                                                    persone.Add(persona.risorsaUmana.contatto.ragione_Sociale)
                                                Else
                                                    persone.Add(String.Format("{0} {1}", persona.risorsaUmana.contatto.nome, persona.risorsaUmana.contatto.cognome))
                                                End If
                                            End If
                                    End Select
                                Next
                            End If
                            If Not IsNothing(att.centriDiCosto) AndAlso att.centriDiCosto.Any Then
                                For Each c In att.centriDiCosto
                                    Dim cdc = DirectCast(c, AgronicaCoreModelsSTD.attivita.centri_di_costo.EsercizioCDC)
                                    If Not IsNothing(cdc) AndAlso Not IsNothing(cdc.esercizio) Then
                                        Dim Piva_Cdc = cdc.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva
                                        Dim Sa_Cod_Cdc = cdc.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice
                                        Dim Appezza_Cdc = cdc.esercizio.impiantoPK.appezzamentoPK.codice
                                        Dim Id_Reg_Cdc = cdc.esercizio.impiantoPK.codice

                                        filtroImpiantiPerStaticMap.Add(
                                            New Dictionary(Of String, Object) From
                                            {
                                                {"piva", Piva_Cdc}, {"sa_cod", Sa_Cod_Cdc}, {"appezza", Appezza_Cdc}, {"id_reg", Id_Reg_Cdc}
                                            }
                                        )

                                        Dim DTImpianti = objImpianti.Leggi_x_anagrafica2(Piva_Cdc,
                                                                 Sa_Cod_Cdc, Appezza_Cdc, Id_Reg_Cdc, 0,
                                                                "", "", objParametri_Server, Date.Now)
                                        If Not IsNothing(DTImpianti) AndAlso DTImpianti.Rows.Count > 0 Then
                                            impianti.Add(DTImpianti.Rows(0).Item("app_nome"))
                                        End If

                                    End If
                                Next

                            End If
                            If filtroImpiantiPerStaticMap.Any AndAlso filtroImpiantiPerStaticMap.Count <= 3 Then
                                Dim DtImpiantiMappe = objImpianti.Leggi_StaticMap(filtroImpiantiPerStaticMap, "", "", objParametri_Server)
                                If Not IsNothing(DtImpiantiMappe) AndAlso DtImpiantiMappe.Rows.Count > 0 Then
                                    Dim dicMappe = DtImpiantiMappe.ToExpandoObject
                                    For Each mappa In dicMappe
                                        impiantiMappe.Add(New AgronicaCoreModelsSTD.Widgets.ImpiantiMappe With
                                                           {
                                                                .APP_Nome = mappa("APP_NOME"),
                                                                .StaticMap = mappa("StaticMapBase64String")
                                                           }
                                                        )
                                    Next
                                End If
                            End If


                            op.Impianti = impianti.OrderBy(Function(i) i).ToList
                            op.ImpiantiMappe = impiantiMappe.OrderBy(Function(i) i.APP_Nome).ToList
                            op.Macchine = macchine
                            op.Personale = persone
                            op.Operazione = New Operazione With {.Modifica = "", .Visualizza = "", .PermessoScrittura = permessoScrittura}
                            op.AttivitaSvolte = New List(Of String)
                            operazione.Add(op)
                        Next

                    End If

                    widgetCache.Set(cacheKey,
                                    New CacheAttivita With
                                    {
                                        .ListaConfronto = listaAgendeNew,
                                        .Attivita = operazione
                                    },
                                    MemoryCacheFactory.Instance.CachePolicy)



                Else
                    operazione.AddRange(ListaAttivitaCached.Attivita)
                End If

            End If

            r.RispostaStringa = operazione
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r


    End Function

    <WebMethod()>
    Public Function LeggiUltimeVisite(ByVal InData As Object) As rispostaStandard(Of List(Of Widget_Operazione))

        Dim r As New rispostaStandard(Of List(Of Widget_Operazione))

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim params As CoreWS_Generic(Of Widget_Operazioni_IN) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of Widget_Operazioni_IN))(JsonConvert.SerializeObject(InData), a)

        If params.objP.objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If
        If params.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If
        If params.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Dim operazioni As New List(Of Widget_Operazione)
        Dim widgetCache = MemoryCacheFactory.Instance.WIDGET_CACHE

        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_utenti)

            Dim Piva As String = params.InData.Piva
            Dim topNRows As Integer = 5
            If Not IsNothing(params.InData.NumeroMovimenti) Then
                topNRows = params.InData.NumeroMovimenti
            End If

            Dim filtroVisite As String = "5007"
            If Not IsNothing(params.InData.Filtro_Lav_Cod) Then
                filtroVisite = params.InData.Filtro_Lav_Cod
            End If

            Dim objMov As New AgronicaCoreContabDAL.Movimenti_R
            Dim xFiltroMovimenti As String = " AND a.Lav_Cod IN (" + filtroVisite + ")"
            Dim xOrderBY As String = " ORDER BY m.Data_Movimento DESC, a.Id_Agenda DESC "

            Dim v_R As New AgronicaCoreVisiteBIZ.Visite_R
            Dim dt As DataTable = v_R.LeggiDettagliVisiteNew(-1, -1, "", "", 0, AGRODATAINIZIO, AGRODATAFINE, "", "", Nothing, Nothing, -1, -1, -1, xFiltroMovimenti, xOrderBY, objParametri_Server, objParametri_Utenti, topNRows)

            If dt IsNot Nothing Then
                dt = dt.DefaultView.ToTable(True, "Id_Agenda", "Data_Movimento")
            End If

            Dim agende = dt.ToExpandoObject

            If agende.Any Then
                Dim listaAgendeNew As New List(Of OggettoConfronto)
                For Each agenda In agende
                    listaAgendeNew.Add(New OggettoConfronto With
                                    {
                                        .Id_agenda = agenda("Id_Agenda"),
                                        .Data_Modifica = agenda("Data_Movimento")
                                    })
                Next
                Dim cacheKey As String = Get_Cache_Key_Attivita("Visite_" + filtroVisite, objParametri_Server.StringaConnessione, objParametri_Utenti.UtenteUsername, Piva)
                Dim ListaAttivitaCached As CacheAttivita = widgetCache.Get(cacheKey)

                Dim ricarica As Boolean = False

                If IsNothing(ListaAttivitaCached) Then
                    ricarica = True
                Else
                    If Not listaAgendeNew.SequenceEqual(ListaAttivitaCached.ListaConfronto) Then
                        ricarica = True
                    End If
                End If

                '' TODO (Check
                ricarica = True


                If ricarica Then

                    Dim listaAttivita As New List(Of AgronicaCoreModelsSTD.attivita.Attivita)

                    If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                        Dim objAgenda As New Agenda_Operazione_Helper
                        Dim agendaMapper As New AgendaToAttivita
                        Parallel.ForEach(dt.AsEnumerable, Sub(row As DataRow)

                                                              Dim pSServer = objParametri_Super_Server.CreateDeepCopy(objParametri_Super_Server)
                                                              Dim pServer = objParametri_Server.CreateDeepCopy(objParametri_Server)
                                                              Dim pUtenti = objParametri_Utenti.CreateDeepCopy(objParametri_Utenti)

                                                              Dim id_agenda As Integer = Convert.ToInt32(row.Item("Id_Agenda"))
                                                              Dim agenda As Operazione_Agenda = objAgenda.Leggi(params.InData.Piva, 0, id_agenda, 0, pServer)
                                                              Dim attivita As AgronicaCoreModelsSTD.attivita.Attivita = Nothing
                                                              If Not IsNothing(agenda) Then
                                                                  Try
                                                                      attivita = agendaMapper.AgendaSuAttivita(agenda, True, pSServer, pServer, pUtenti)
                                                                  Catch ex As Exception
                                                                  End Try
                                                              End If
                                                              If Not IsNothing(attivita) Then
                                                                  SyncLock (listaAttivita)
                                                                      listaAttivita.Add(attivita)
                                                                  End SyncLock
                                                              End If

                                                          End Sub)

                    End If

                    If Not IsNothing(listaAttivita) AndAlso listaAttivita.Any Then

                        Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
                        Dim permessoScrittura As Boolean = ObjUtenti.Controlla_Permessi_Utente(
                            objParametri_Utenti.UtenteUsername, 5,
                            enum_Security_Attivita.Visite_Lista, enum_Security_Operazione.Modifica, Date.Now, "", objParametri_Utenti)

                        Dim leggi_Attivita_R As New AgronicaCoreContabDAL.Attivita_R

                        Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                        For Each att As AgronicaCoreModelsSTD.attivita.Attivita In listaAttivita

                            Dim centriAziendali As New List(Of String)
                            Dim impianti As New List(Of String)
                            Dim impiantiMappe As New List(Of ImpiantiMappe)
                            Dim filtroImpiantiPerStaticMap = New List(Of Dictionary(Of String, Object))
                            Dim attivitaSvolte As New List(Of String)

                            Dim op = New Widget_Operazione
                            op.IdAgenda = CInt(att.codice)
                            op.LavCod = 0

                            Dim lavDes = ""
                            If Not IsNothing(att.job) AndAlso Not IsNothing(att.job.primaryKey) Then
                                op.LavCod = If(String.IsNullOrEmpty(att.job.primaryKey.codice), 0, CInt(att.job.primaryKey.codice))
                                lavDes = att.job.descrizione
                            End If

                            op.Data_Operazione = att.inizio

                            If att.attivitaPersonalizzata IsNot Nothing Then
                                op.Descrizione_Operazione = leggi_Attivita_R.AttivitaDes_From_AttivitaCod(att.attivitaPersonalizzata.codice, objParametri_Server)
                            Else
                                op.Descrizione_Operazione = lavDes
                            End If

                            op.Piva = params.InData.Piva

                            Dim risorsaAttivita = Nothing

                            op.Specie = ""
                            Dim attRisorse = att
                            If att.attivitaCollegate IsNot Nothing AndAlso att.attivitaCollegate.Count > 0 Then
                                attRisorse = att.attivitaCollegate(0)
                            End If

                            If Not IsNothing(attRisorse) Then
                                If Not IsNothing(attRisorse.utilizzoTerreno) Then
                                    Select Case attRisorse.utilizzoTerreno.classType
                                        Case costanti.ClassType.Varieta
                                            Dim varieta = DirectCast(attRisorse.utilizzoTerreno, AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta)
                                            If Not IsNothing(varieta) AndAlso Not IsNothing(varieta.specie) Then
                                                op.Specie = varieta.specie.descrizione
                                            End If

                                        Case costanti.ClassType.DestinazioneUso
                                            Dim destinazioneUso = DirectCast(attRisorse.utilizzoTerreno, AgronicaCoreModelsSTD.metaschema.utilizzi.DestinazioneUso)
                                            If Not IsNothing(destinazioneUso) Then
                                                op.Specie = destinazioneUso.descrizione
                                            End If
                                    End Select
                                End If

                                'Centro Aziendale. Li prendo tutti poi alla fine distinct
                                If Not IsNothing(attRisorse.centroAziendale) Then
                                    centriAziendali.Add(attRisorse.centroAziendale.nome)
                                End If

                                ' Risorse (sono se non esiste l'utilizzo terreno, e solo una (potrebbe esserci specie vegetale + animale, ne prendiamo solo una)
                                If Not IsNothing(attRisorse.risorse) AndAlso attRisorse.risorse.Any Then
                                    For Each ris As AgronicaCoreModelsSTD.attivita.risorse.Risorsa In attRisorse.risorse
                                        Select Case ris.classType
                                            Case costanti.ClassType.RisorsaSpecie
                                                Dim risorsaSpecie = DirectCast(ris, AgronicaCoreModelsSTD.attivita.risorse.RisorsaSpecie)
                                                If Not IsNothing(risorsaSpecie) AndAlso Not IsNothing(risorsaSpecie.specie) Then
                                                    If op.Specie.Length = 0 Then
                                                        op.Specie += risorsaSpecie.specie.descrizione
                                                    End If
                                                End If

                                            Case costanti.ClassType.RisorsaDestinazioneUso
                                                Dim risorsaDestinazioneUso = DirectCast(ris, AgronicaCoreModelsSTD.attivita.risorse.RisorsaDestinazioneUso)
                                                If Not IsNothing(risorsaDestinazioneUso) AndAlso Not IsNothing(risorsaDestinazioneUso.destinazioneUso) Then
                                                    If op.Specie.Length = 0 Then
                                                        op.Specie += risorsaDestinazioneUso.destinazioneUso.descrizione
                                                    End If
                                                End If

                                            Case costanti.ClassType.RisorsaZootecnica
                                                Dim risorsaZootecnica = DirectCast(ris, AgronicaCoreModelsSTD.attivita.risorse.RisorsaZootecnica)
                                                If Not IsNothing(risorsaZootecnica) Then
                                                    If op.Specie.Length = 0 Then
                                                        op.Specie += risorsaZootecnica.descrizione
                                                    End If
                                                End If

                                        End Select
                                    Next
                                End If

                                ' Impianti
                                If Not IsNothing(attRisorse.centriDiCosto) AndAlso attRisorse.centriDiCosto.Any Then
                                    For Each c In attRisorse.centriDiCosto
                                        Dim cdc = DirectCast(c, AgronicaCoreModelsSTD.attivita.centri_di_costo.EsercizioCDC)
                                        If Not IsNothing(cdc) AndAlso Not IsNothing(cdc.esercizio) Then
                                            Dim Piva_Cdc = cdc.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva
                                            Dim Sa_Cod_Cdc = cdc.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice
                                            Dim Appezza_Cdc = cdc.esercizio.impiantoPK.appezzamentoPK.codice
                                            Dim Id_Reg_Cdc = cdc.esercizio.impiantoPK.codice

                                            filtroImpiantiPerStaticMap.Add(
                                            New Dictionary(Of String, Object) From
                                            {
                                                {"piva", Piva_Cdc}, {"sa_cod", Sa_Cod_Cdc}, {"appezza", Appezza_Cdc}, {"id_reg", Id_Reg_Cdc}
                                            }
                                        )

                                            Dim DTImpianti = objImpianti.Leggi_x_anagrafica2(Piva_Cdc,
                                                                    Sa_Cod_Cdc, Appezza_Cdc, Id_Reg_Cdc, 0,
                                                                "", "", objParametri_Server, Date.Now)
                                            If Not IsNothing(DTImpianti) AndAlso DTImpianti.Rows.Count > 0 Then
                                                impianti.Add(DTImpianti.Rows(0).Item("app_nome"))
                                            End If

                                        End If
                                    Next

                                End If

                            End If

                            If filtroImpiantiPerStaticMap.Any AndAlso filtroImpiantiPerStaticMap.Count <= 3 Then
                                Dim DtImpiantiMappe = objImpianti.Leggi_StaticMap(filtroImpiantiPerStaticMap, "", "", objParametri_Server)
                                If Not IsNothing(DtImpiantiMappe) AndAlso DtImpiantiMappe.Rows.Count > 0 Then
                                    Dim dicMappe = DtImpiantiMappe.ToExpandoObject
                                    For Each mappa In dicMappe
                                        impiantiMappe.Add(New AgronicaCoreModelsSTD.Widgets.ImpiantiMappe With
                                                       {
                                                            .APP_Nome = mappa("APP_NOME"),
                                                            .StaticMap = mappa("StaticMapBase64String")
                                                       }
                                                    )
                                    Next
                                End If
                            End If

                            ' attivita svolte, le prende tutti poi alla fine distinct
                            attivitaSvolte.Add(att.descrizione)
                            op.AttivitaSvolte = attivitaSvolte.Distinct.ToList

                            op.Rag_Soc = String.Join(", ", centriAziendali.Distinct())
                            op.Impianti = impianti.Distinct.OrderBy(Function(i) i).ToList()
                            op.ImpiantiMappe = impiantiMappe.OrderBy(Function(i) i.APP_Nome).ToList
                            op.Operazione = New Operazione With {.Modifica = "", .Visualizza = "", .PermessoScrittura = permessoScrittura}
                            operazioni.Add(op)
                        Next

                    End If

                    operazioni = operazioni.OrderByDescending(Function(op) op.Data_Operazione).ThenByDescending(Function(op) op.IdAgenda).ToList

                    widgetCache.Set(cacheKey,
                                New CacheAttivita With
                                {
                                    .ListaConfronto = listaAgendeNew,
                                    .Attivita = operazioni
                                },
                                MemoryCacheFactory.Instance.CachePolicy)


                Else
                    operazioni.AddRange(ListaAttivitaCached.Attivita)
                End If

            End If


            r.RispostaStringa = operazioni
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r


    End Function

    <WebMethod(EnableSession:=True)>
    Public Function LeggiUltimeAttivita(ByVal InData As Object) As rispostaStandard(Of List(Of Widget_Operazione))

        Dim r As New rispostaStandard(Of List(Of Widget_Operazione))

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim params As CoreWS_Generic(Of Widget_Operazioni_IN) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of Widget_Operazioni_IN))(JsonConvert.SerializeObject(InData), a)

        If params.objP.objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If
        If params.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If
        If params.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Dim widgetCache = MemoryCacheFactory.Instance.WIDGET_CACHE
        Dim listaAttivita As New List(Of Widget_Operazione)

        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_utenti)

            ' Servono per istanziare AgroWebConfig
            HttpContext.Current.Session("ASG_objParametri_Super_Server") = objParametri_Super_Server
            HttpContext.Current.Session("ASG_objParametri_Server") = objParametri_Server
            HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametri_Utenti

            Dim Piva As String = params.InData.Piva

            Dim filtroAttivita As String = "30, 79, 108, 109, 110, 113, 114, 119, 125, 126, 169"
            Dim topNRows As Integer = 5
            If Not IsNothing(params.InData.NumeroMovimenti) Then
                topNRows = params.InData.NumeroMovimenti
            End If

            Dim objMov As New AgronicaCoreContabDAL.Movimenti_R
            Dim xFiltroMovimenti As String = " Agenda.Lav_Cod <1000 AND Agenda.Lav_Cod NOT IN (" + filtroAttivita + ")"
            Dim xOrderBY As String = " Movimenti.Data_Movimento DESC, Id_Agenda DESC "
            Dim dt As DataTable = objMov.Leggi(Piva, 0, 0, 0,
                                               0, "", AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                xFiltroMovimenti, xOrderBY, objParametri_Server, topNRows)

            Dim agende = dt.ToExpandoObject()
            ' Se non ci sono movimenti esco direttamente
            If agende.Any Then

                Dim listaAgendeNew As New List(Of OggettoConfronto)
                For Each agenda In agende
                    listaAgendeNew.Add(New OggettoConfronto With
                                    {
                                        .Id_agenda = agenda("Id_Agenda"),
                                        .Data_Modifica = agenda("Data_Modifica")
                                    })
                Next
                Dim filtroAgende As List(Of Integer) = listaAgendeNew.Select(Function(ag)
                                                                                 Return ag.Id_agenda
                                                                             End Function).Distinct.ToList()

                Dim cacheKey As String = Get_Cache_Key_Attivita("Attivita", objParametri_Server.StringaConnessione, objParametri_Utenti.UtenteUsername, Piva)
                Dim ListaAttivitaCached As CacheAttivita = widgetCache.Get(cacheKey)

                Dim ricarica As Boolean = False

                If IsNothing(ListaAttivitaCached) Then
                    ricarica = True
                Else
                    If Not listaAgendeNew.SequenceEqual(ListaAttivitaCached.ListaConfronto) Then
                        ricarica = True
                    End If
                End If

                '' TODO (Check
                ricarica = True

                If ricarica Then

                    Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                    Dim objLav As New AgronicaCoreContabBIZ.Movimenti_R
                    Dim dtAgenda As DataTable = objLav.Carica_LavorazioniParallel(
                                                                Piva:=Piva,
                                                                Sa_Cod:=0,
                                                                DataDa:=AGRODATAINIZIO,
                                                                DataA:=AGRODATAFINE,
                                                                Veg_Cod:=0,
                                                                id_cod:=0,
                                                                Cul_Cod:=0,
                                                                Tipo:="",
                                                                Gru_Cod:=0,
                                                                Lav_Cod:=0,
                                                                Flag_TerrenoNudo:=False,
                                                                xFiltroAggiuntivo_colturali:="",
                                                                xFiltroAggiuntivo_postRaccolta:="",
                                                                xFiltroAggiuntivo_contabili:="",
                                                                xFiltroAggiuntivo_contabili_Macchine:="",
                                                                xFiltroAggiuntivo_contabili_Audit:="",
                                                                xOrderBy:="",
                                                                objparametri_Server:=objParametri_Server,
                                                                objparametri_Utenti:=objParametri_Utenti,
                                                                FF_TrackedData_Cod:=-1, filtroAgende:=filtroAgende,
                                                                numeroDiRigheDaEstrarre:=topNRows,
                                                                TipoOperazioni:=New List(Of Integer),
                                                                estraiPkImpianti:=True)

                    If Not IsNothing(dtAgenda) AndAlso dtAgenda.Rows.Count > 0 Then

                        Dim BtnModifica As String = "1"
                        Dim BtnVisualizza As String = "1"

                        Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
                        Dim permessoScrittura As Boolean = ObjUtenti.Controlla_Permessi_Utente(
                            objParametri_Utenti.UtenteUsername, 5,
                            enum_Security_Attivita.Agenda_AccessoMenu, enum_Security_Operazione.Modifica, Date.Now, "", objParametri_Utenti)

                        Dim expandoAgende = dtAgenda.ToExpandoObject
                        For Each ag In expandoAgende


                            Dim filtroImpiantiPerStaticMap = New List(Of Dictionary(Of String, Object))

                            Dim impianti As New List(Of String)
                            Dim impiantiMappe As New List(Of ImpiantiMappe)
                            Dim macchine As New List(Of String)
                            Dim personale As New List(Of String)

                            Dim attivita As New Widget_Operazione
                            attivita.IdAgenda = ag("Id_Agenda")
                            attivita.Specie = ag("Specie")
                            attivita.Piva = Piva
                            attivita.Rag_Soc = ag("Rag_Soc")
                            attivita.Data_Operazione = ag("Data2")
                            attivita.LavCod = ag("Lav_Cod")
                            Dim opDes As String = ag("Operazione_DES")
                            If IsNothing(opDes) Then
                                attivita.Descrizione_Operazione = ag("Lav_Des")
                            Else
                                attivita.Descrizione_Operazione = String.Format("{0} - {1}", opDes, ag("Lav_Des"))
                            End If
                            If Not String.IsNullOrEmpty(ag("Appezzamenti_Coinvolti")) Then
                                impianti = ag("Appezzamenti_Coinvolti").ToString.Split(",").ToList
                            End If
                            If Not String.IsNullOrEmpty(ag("Costi_Macchine")) Then
                                macchine = ag("Costi_Macchine").ToString.Split(",").ToList
                            End If
                            If Not String.IsNullOrEmpty(ag("Costi_Operatori")) Then
                                personale = ag("Costi_Operatori").ToString.Split(",").ToList
                            End If

                            If Not IsNothing(ag("PK_Impianti")) AndAlso Not String.IsNullOrEmpty(ag("PK_Impianti")) Then
                                Dim arrayImpPk = ag("PK_Impianti").ToString.Split("|")
                                For Each pk In arrayImpPk
                                    Dim chiavi = pk.Split(",")
                                    Dim Piva_Cdc = chiavi(0).ToString.Trim
                                    Dim Sa_Cod_Cdc = CInt(chiavi(1))
                                    Dim Appezza_Cdc = CInt(chiavi(2))
                                    Dim Id_Reg_Cdc = CInt(chiavi(3))
                                    filtroImpiantiPerStaticMap.Add(
                                        New Dictionary(Of String, Object) From
                                        {
                                            {"piva", Piva_Cdc}, {"sa_cod", Sa_Cod_Cdc}, {"appezza", Appezza_Cdc}, {"id_reg", Id_Reg_Cdc}
                                        }
                                    )
                                Next
                            End If

                            If filtroImpiantiPerStaticMap.Any AndAlso filtroImpiantiPerStaticMap.Count <= 3 Then
                                Dim DtImpiantiMappe = objImpianti.Leggi_StaticMap(filtroImpiantiPerStaticMap, "", "", objParametri_Server)
                                If Not IsNothing(DtImpiantiMappe) AndAlso DtImpiantiMappe.Rows.Count > 0 Then
                                    Dim dicMappe = DtImpiantiMappe.ToExpandoObject
                                    For Each mappa In dicMappe
                                        impiantiMappe.Add(New AgronicaCoreModelsSTD.Widgets.ImpiantiMappe With
                                                       {
                                                            .APP_Nome = mappa("APP_NOME"),
                                                            .StaticMap = mappa("StaticMapBase64String")
                                                       }
                                                    )
                                    Next
                                End If
                            End If

                            attivita.Impianti = impianti.OrderBy(Function(i) i).ToList
                            attivita.ImpiantiMappe = impiantiMappe.OrderBy(Function(i) i.APP_Nome).ToList
                            attivita.Macchine = macchine
                            attivita.Personale = personale
                            attivita.AttivitaSvolte = New List(Of String)

                            'Nascondo il pulsante di Modifica se: 
                            '- L'installazione della trappola è stata reinnescata
                            '- Ho un Reinnesco Trappole nuovo con la categoria di Magazzino Formulati
                            If permessoScrittura AndAlso
                              (CInt(ag("Installazione_Trappola_Reinnescata")) = 1 OrElse (CInt(ag("Lav_Cod")) = LAVCOD_REINNESCO_TRAPPOLE AndAlso ag("Cau_Mov").ToString() = CAU_TRATTAMENTO)) Then
                                BtnModifica = "0"
                            End If

                            attivita.Operazione = New Operazione With {.Modifica = BtnModifica, .Visualizza = BtnVisualizza, .PermessoScrittura = permessoScrittura}

                            listaAttivita.Add(attivita)
                        Next

                    End If

                    listaAttivita = listaAttivita.OrderByDescending(Function(op) op.Data_Operazione).ThenByDescending(Function(op) op.IdAgenda).ToList

                    widgetCache.Set(cacheKey,
                               New CacheAttivita With
                               {
                                   .ListaConfronto = listaAgendeNew,
                                   .Attivita = listaAttivita
                               },
                               MemoryCacheFactory.Instance.CachePolicy)
                Else

                    listaAttivita.AddRange(ListaAttivitaCached.Attivita)
                End If

            End If


            r.RispostaStringa = listaAttivita
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r


    End Function

    <WebMethod()>
    Public Function ConfigurazioneAgroMeteo(ByVal InData As Object) As rispostaStandard(Of Widget_AgroMeteo)

        Dim r As New rispostaStandard(Of Widget_AgroMeteo)

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim params As CoreWS_Generic(Of String) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData), a)

        If params.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Dim config As New Widget_AgroMeteo With {.Piva = params.InData}

        Try

            ' se non è selezionata l'azienda uso l'azienda superuser
            Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_server)
            Dim PivaAzienda = IIf(params.InData <> "", params.InData, objParametri_Server.PivaSuperUser)

            If PivaAzienda <> "" Then

                Dim CentriAziendaliLeggi As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
                Dim dtCentriAziendali As DataTable = CentriAziendaliLeggi.CentroAziendaliLatLongDescrizione(PivaAzienda, 0, objParametri_Server)

                If dtCentriAziendali.Rows.Count > 0 Then
                    config.AgroMeteoLatitudine = dtCentriAziendali(0)("lat")
                    config.AgroMeteoLongitudine = dtCentriAziendali(0)("long")
                End If

                If config.AgroMeteoLatitudine = 0 AndAlso config.AgroMeteoLongitudine = 0 Then

                    Dim objImpreseInd As New AgronicaCoreAnagrafeDAL.ImpresexIndirizzi_R
                    Dim dtIndirizzo As DataTable = objImpreseInd.Leggi2(PivaAzienda, True, 0, 1, "", "", objParametri_Server)

                    If dtIndirizzo.Rows.Count > 0 Then
                        Dim citta As String = dtIndirizzo(0)("com_des")
                        Dim frazione As String = dtIndirizzo(0)("frz_des")
                        Dim stato As String = dtIndirizzo(0)("stato")
                        If String.IsNullOrEmpty(stato) OrElse stato.ToUpper = "ITA" OrElse stato.ToUpper = "ITALIA" Then
                            stato = "IT"
                        End If

                        If (citta = "" OrElse citta = "Non Definita") AndAlso frazione <> "" Then
                            config.AgroMeteoDescrizione = frazione
                        Else
                            config.AgroMeteoDescrizione = citta
                        End If

                        config.AgroMeteoDescrizione &= "," + stato
                    End If

                End If

            End If

            r.RispostaStringa = config

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod()>
    Public Function WeatherLatLng(ByVal InData As Object) As rispostaStandard(Of Widget_MeteoImpresaLatLng)

        Dim response As New rispostaStandard(Of Widget_MeteoImpresaLatLng)
        Dim jsonSettings As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
        Dim params As CoreWS_Generic(Of String) = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData), jsonSettings)

        If params.objP.objP_server = "" Then
            response.Errore = "objP_server non valorizzato"
            Return response
        End If

        Try
            ' se non è selezionata l'azienda uso l'azienda superuser
            Dim objServer As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_server)
            Dim objWeather As New AgronicaCoreAnagrafeBIZ.WeatherWidget

            response.RispostaStringa = objWeather.ReadLatLng(objParams:=objServer, piva:=params.InData)

        Catch ex As Exception
            response.RispostaOK = False
            response.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return response

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiColture(ByVal InData As Object) As rispostaStandard(Of List(Of Widget_Coltura))

        Dim r As New rispostaStandard(Of List(Of Widget_Coltura))

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim params As CoreWS_Generic(Of Widget_Culture_IN) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of Widget_Culture_IN))(JsonConvert.SerializeObject(InData), a)

        If params.objP.objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If
        If params.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If
        If params.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Dim colture As New List(Of Widget_Coltura)

        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_utenti)



            Dim linguaCodiceISO As String = "it"
            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            If Not IsNothing(dtLingua) AndAlso dtLingua.Rows.Count > 0 Then
                linguaCodiceISO = dtLingua.Rows(0)("CodiceISO")
            End If

            Dim ci As CultureInfo = New CultureInfo(linguaCodiceISO)

            'Considero gli impianti validi alla data e quelli che sono stati chiusi durante l'anno solare corrente
            Dim Inizio_Anno_Corrente = "01/01/" & Now.Year
            Dim Fine_Anno_Corrente = "31/12/" & Now.Year

            Dim Piva As String = params.InData.Piva
            Dim xFiltroAggiuntivo As String = " (( Reg_Impianti.Validita_inizio <=" + UtilityProvider.Agro_SQL_SaveDate(DateTime.Now)
            xFiltroAggiuntivo = xFiltroAggiuntivo + " AND " + UtilityProvider.Agro_SQL_SaveDate(DateTime.Now) + " <= Reg_Impianti.Validita_Fine ) OR "
            xFiltroAggiuntivo = xFiltroAggiuntivo + " ( Reg_Impianti.Validita_Fine >= " + UtilityProvider.Agro_SQL_SaveDate(Inizio_Anno_Corrente)
            xFiltroAggiuntivo = xFiltroAggiuntivo + " AND " + UtilityProvider.Agro_SQL_SaveDate(Fine_Anno_Corrente) + " >= Reg_Impianti.Validita_Fine ))"

            Dim xOrderBY As String = " sum(Sup_Imp) DESC "

            Dim numeroDiRighe = 5
            If Not IsNothing(params.InData.NumeroMovimenti) Then
                numeroDiRighe = params.InData.NumeroMovimenti
            End If

            Dim ir As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
            Dim dt As DataTable = ir.LeggiSuperficiXWidget(Piva, 0, 0, 0, 0, 0,
                                                     AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, xFiltroAggiuntivo, xOrderBY, objParametri_Server, numeroDiRighe)

            If IsNothing(dt) Then
                r.RispostaStringa = colture
                Return r
            End If

            Dim expandoObj = dt.ToExpandoObject
            colture = expandoObj.Where(Function(colt) colt("Superficie") > 0).Select(Function(colt) New Widget_Coltura With {
                        .Veg_Cod = colt("veg_cod"),
                        .Veg_Des = colt("Veg_Des"),
                        .Superficie = colt("Superficie")
                    }).ToList()


            Dim listaVeg_COd As List(Of Integer) = colture.Select(Function(c)
                                                                      Return c.Veg_Cod
                                                                  End Function).Distinct.ToList


            If listaVeg_COd.Count > 0 Then
                Dim coltureDaNonContare = String.Join(",", listaVeg_COd)
                Dim xFiltroSpecie = " SpecieVegetali.veg_cod NOT IN (" + coltureDaNonContare + ")"
                xFiltroAggiuntivo = xFiltroAggiuntivo + " AND " + xFiltroSpecie
            End If
            Dim supTotale = ir.LeggiSuperfici(Piva, 0, 0, 0, 0, 0,
                                                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, xFiltroAggiuntivo, "", objParametri_Server)

            If (supTotale > 0) Then
                colture.Add(New Widget_Coltura With {
                    .Veg_Cod = 0,
                    .Veg_Des = "Rimanenti colture",
                    .Superficie = supTotale
                })
            End If

            r.RispostaStringa = colture
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r


    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_GHGColture(ByVal InData As Object) As rispostaStandard(Of List(Of Widget_GHGColture))

        Dim r As New rispostaStandard(Of List(Of Widget_GHGColture))

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim params As CoreWS_Generic(Of Widget_Culture_IN) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of Widget_Culture_IN))(JsonConvert.SerializeObject(InData), a)

        If params.objP.objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If
        If params.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If
        If params.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Dim colture As New List(Of Widget_Coltura)
        Dim colture_GHG As New List(Of Widget_GHGColture)

        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_utenti)



            Dim linguaCodiceISO As String = "it"
            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            If Not IsNothing(dtLingua) AndAlso dtLingua.Rows.Count > 0 Then
                linguaCodiceISO = dtLingua.Rows(0)("CodiceISO")
            End If

            Dim ci As CultureInfo = New CultureInfo(linguaCodiceISO)


            Dim Piva As String = params.InData.Piva

            'Considero gli impianti validi alla data e quelli che sono stati chiusi durante l'anno solare corrente
            Dim Inizio_Anno_Corrente = "01/01/" & Now.Year
            Dim Fine_Anno_Corrente = "31/12/" & Now.Year
            'Non estraggi il terreno nudo
            Dim xFiltroAggiuntivo As String = ""
            xFiltroAggiuntivo = xFiltroAggiuntivo + " Reg_Impianti.Cul_Cod != 0 "
            xFiltroAggiuntivo = xFiltroAggiuntivo + " AND ( ((Reg_Impianti.Validita_inizio <= " + UtilityProvider.Agro_SQL_SaveDate(DateTime.Now)
            xFiltroAggiuntivo = xFiltroAggiuntivo + " AND " + UtilityProvider.Agro_SQL_SaveDate(DateTime.Now) + " <= Reg_Impianti.Validita_Fine)) OR  "
            xFiltroAggiuntivo = xFiltroAggiuntivo + "   ((Reg_Impianti.Validita_Fine >= " + UtilityProvider.Agro_SQL_SaveDate(Inizio_Anno_Corrente)
            xFiltroAggiuntivo = xFiltroAggiuntivo + " AND " + UtilityProvider.Agro_SQL_SaveDate(Fine_Anno_Corrente) + " >= Reg_Impianti.Validita_Fine) ) ) "

            Dim xOrderBY As String = " sum(Sup_Imp) DESC "

            Dim numeroDiRighe = 5
            If Not IsNothing(params.InData.NumeroMovimenti) Then
                numeroDiRighe = params.InData.NumeroMovimenti
            End If

            'Estraggo il GHG solo per le specie con superficie più grande
            Dim ir As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
            Dim dt As DataTable = ir.LeggiSuperficiXWidget(Piva, 0, 0, 0, 0, 0,
                                                     AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, xFiltroAggiuntivo, xOrderBY, objParametri_Server, numeroDiRighe)

            If IsNothing(dt) Then
                r.RispostaStringa = colture_GHG
                Return r
            End If

            ''''Dim expandoObj = dt.ToExpandoObject
            ''''colture = expandoObj.Where(Function(colt) colt("Superficie") > 0).Select(Function(colt) New Widget_Coltura With {
            ''''            .Veg_Cod = colt("veg_cod"),
            ''''            .Veg_Des = colt("Veg_Des"),
            ''''            .Superficie = colt("Superficie")
            ''''        }).ToList()


            ''''Dim listaVeg_COd As List(Of Integer) = colture.Select(Function(c)
            ''''                                                          Return c.Veg_Cod
            ''''                                                      End Function).Distinct.ToList
            ''''Dim coltureTrovate = String.Join(",", listaVeg_COd)

            'Lettura dalla tabella Imprese_Parametri_GHG
            Dim ObjImprese_Par_GHG As New AgronicaCoreAnagrafeDAL.Imprese_Parametri_GHG_R
            Dim dtPat_GHG As DataTable = ObjImprese_Par_GHG.Leggi("", 0, 0, 0, AGRODATAINIZIO, "Piva In ('" & Piva & "','" & objParametri_Server.PivaSuperUser & "')", "", objParametri_Server)
            Dim dr_search() As DataRow

            Dim rese_dal As New Impresa_Progetti_R
            Dim ghg_dal As New AgronicaCoreContabDAL.GHG_Registrazioni_R
            Dim dt_rese_colture As DataTable
            Dim bRese As Boolean = False
            For Each drColture In dt.Rows

                bRese = False

                '=======================================================================================================================================
                '1. Controllo Utilizzando le rese
                '---------------------------------------------------------------------------------------------------------------------------------------

                dt_rese_colture = rese_dal.CalcolaResaHaPerImpianto(Piva, drColture("veg_cod"), 0, 0, 0, 0, 0,
                                                      objParametri_Server)
                If (dt_rese_colture.Rows.Count > 0) Then
                    Dim dt_GHG_colture As DataTable = ghg_dal.LeggiGHG_Colture(Piva, drColture("veg_cod"), objParametri_Server)
                    'TODO Eec_Totale controllare se è corretto rapportarsi agli HA impianti e non a quelli lavorati
                    If dt_rese_colture.Rows(0).Item("HA") > 0 AndAlso
                        dt_rese_colture.Rows(0).Item("RESA_HA") > 0 Then
                        If dt_GHG_colture.Rows.Count > 0 Then
                            colture_GHG.Add(New Widget_GHGColture With {
                                .Veg_Cod = drColture("veg_cod"),
                                .Veg_Des = drColture("veg_des"),
                                .Eec_Ha = dt_GHG_colture.Rows(0).Item("EEC_HA") / dt_rese_colture.Rows(0).Item("RESA_HA"),
                                .Eec_Totale = dt_GHG_colture.Rows(0).Item("EEC_HA") / dt_rese_colture.Rows(0).Item("RESA_HA") * dt_rese_colture.Rows(0).Item("HA")
                            })
                            'Else
                            '    colture_GHG.Add(New Widget_GHGColture With {
                            '        .Veg_Cod = drColture("veg_cod"),
                            '        .Veg_Des = drColture("veg_des"),
                            '        .Eec_Ha = 0,
                            '        .Eec_Totale = 0
                            '    })


                            bRese = True

                        End If

                    End If

                End If


                If Not bRese And dtPat_GHG.Rows.Count > 0 Then

                    '=======================================================================================================================================
                    '2. Controllo Utilizzando la tabella Imprese_Parametri_GHG
                    '---------------------------------------------------------------------------------------------------------------------------------------

                    Dim Eec_Totale As Double = 0
                    Dim HA_Totale As Double = 0
                    Dim Veg_Cod As Integer = 0
                    Dim Veg_Des As String = ""
                    Dim Data_Riferimento As String = ""
                    Dim iCount_Istanze As Integer = 0

                    'Non esiste resa --> Leggo tutti gli impianti della specie vegetale nella terna veg_cod-cul_cod-reg_cod
                    Dim dt_impianti As DataTable = ir.LeggiSuperficiXWidget(Piva, 0, 0, drColture("veg_cod"), 0, 0,
                                                     AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, xFiltroAggiuntivo, xOrderBY, objParametri_Server, numeroDiRighe, True)
                    For Each drColture2 In dt_impianti.Rows

                        Dim bOk As Boolean = False

                        Dim iCount As Integer = 1
                        Dim xFiltro = ""

                        Veg_Cod = drColture2("Veg_Cod")
                        Veg_Des = drColture2("Veg_Des")
                        Data_Riferimento = Format(drColture2("Validita_Inizio"), "MM/dd/yyyy")

                        Do While Not bOk AndAlso iCount <= 6

                            Select Case iCount

                                Case 1 'PIVA corrente + spe + var + reg
                                    xFiltro = "Piva = '" & Piva & "' And Veg_Cod = " & drColture2("Veg_Cod") & " And Cul_Cod = " & drColture2("Cul_Cod") & " And Regolamento_Cod = " & drColture2("Regolamento") & " And Validita_Inizio <= #" & Data_Riferimento & "# And Validita_Fine >= #" & Data_Riferimento & "#"
                                Case 2 'PIVA corrente + spe + var
                                    xFiltro = "Piva = '" & Piva & "' And Veg_Cod = " & drColture2("Veg_Cod") & " And Cul_Cod = " & drColture2("Cul_Cod") & " And Validita_Inizio <= #" & Data_Riferimento & "# And Validita_Fine >= #" & Data_Riferimento & "#"
                                Case 3 'PIVA corrente + spe
                                    xFiltro = "Piva = '" & Piva & "' And Veg_Cod = " & drColture2("Veg_Cod") & " And Validita_Inizio <= #" & Data_Riferimento & "# And Validita_Fine >= #" & Data_Riferimento & "#"
                                Case 4 'PIVA SU + spe + var + reg
                                    xFiltro = "Piva = '" & objParametri_Server.PivaSuperUser & "' And Veg_Cod = " & drColture2("Veg_Cod") & " And Cul_Cod = " & drColture2("Cul_Cod") & " And Regolamento_Cod = " & drColture2("Regolamento") & " And Validita_Inizio <= #" & Data_Riferimento & "# And Validita_Fine >= #" & Data_Riferimento & "#"
                                Case 5 'PIVA SU + spe + var
                                    xFiltro = "Piva = '" & objParametri_Server.PivaSuperUser & "' And Veg_Cod = " & drColture2("Veg_Cod") & " And Cul_Cod = " & drColture2("Cul_Cod") & " And Validita_Inizio <= #" & Data_Riferimento & "# And Validita_Fine >= #" & Data_Riferimento & "#"
                                Case 6 'PIVA SU + spe
                                    xFiltro = "Piva = '" & objParametri_Server.PivaSuperUser & "' And Veg_Cod = " & drColture2("Veg_Cod") & " And Validita_Inizio <= #" & Data_Riferimento & "# And Validita_Fine >= #" & Data_Riferimento & "#"

                            End Select

                            dr_search = dtPat_GHG.Select(xFiltro)
                            If dr_search.Length > 0 Then

                                HA_Totale = HA_Totale + drColture2.Item("Superficie")
                                Eec_Totale = Eec_Totale + (dr_search(0)("EEC") * drColture2.Item("Superficie"))

                                iCount_Istanze = iCount_Istanze + 1
                                bOk = True

                            End If

                            iCount = iCount + 1

                        Loop

                    Next

                    If iCount_Istanze > 0 Then

                        Dim Eec_Ha As Double = Eec_Totale / HA_Totale

                        colture_GHG.Add(New Widget_GHGColture With {
                            .Veg_Cod = Veg_Cod,
                            .Veg_Des = Veg_Des,
                            .Eec_Ha = Eec_Ha,
                            .Eec_Totale = Eec_Totale
                        })

                    End If


                End If

            Next
            r.RispostaStringa = colture_GHG
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r


    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_StimeProduzioneColture(ByVal InData As Object) As rispostaStandard(Of List(Of Widget_StimeProduzioneColture))

        Dim r As New rispostaStandard(Of List(Of Widget_StimeProduzioneColture))

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim params As CoreWS_Generic(Of Widget_StimeProduzioneColture_IN) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of Widget_StimeProduzioneColture_IN))(JsonConvert.SerializeObject(InData), a)

        If params.objP.objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If
        If params.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If
        If params.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Dim colture As New List(Of Widget_Coltura)
        Dim colture_StimeProduzione As New List(Of Widget_StimeProduzioneColture)

        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_utenti)


            Dim linguaCodiceISO As String = "it"
            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            If Not IsNothing(dtLingua) AndAlso dtLingua.Rows.Count > 0 Then
                linguaCodiceISO = dtLingua.Rows(0)("CodiceISO")
            End If

            Dim ci As CultureInfo = New CultureInfo(linguaCodiceISO)


            Dim Piva As String = params.InData.Piva

            'Consiero gli impianti validi alla data e quelli che sono stati chiusi durante l'anno solare corrente
            Dim Inizio_Anno_Corrente = "01/01/" & Now.Year
            Dim Fine_Anno_Corrente = "31/12/" & Now.Year
            'Non estraggi il terreno nudo
            Dim xFiltroAggiuntivo As String = ""
            xFiltroAggiuntivo = xFiltroAggiuntivo + " Reg_Impianti.Cul_Cod != 0 "
            xFiltroAggiuntivo = xFiltroAggiuntivo + " AND ( ((Reg_Impianti.Validita_inizio <= " + UtilityProvider.Agro_SQL_SaveDate(DateTime.Now)
            xFiltroAggiuntivo = xFiltroAggiuntivo + " AND " + UtilityProvider.Agro_SQL_SaveDate(DateTime.Now) + " <= Reg_Impianti.Validita_Fine)) OR  "
            xFiltroAggiuntivo = xFiltroAggiuntivo + "   ((Reg_Impianti.Validita_Fine >= " + UtilityProvider.Agro_SQL_SaveDate(Inizio_Anno_Corrente)
            xFiltroAggiuntivo = xFiltroAggiuntivo + " AND " + UtilityProvider.Agro_SQL_SaveDate(Fine_Anno_Corrente) + " >= Reg_Impianti.Validita_Fine) ) ) "

            Dim xOrderBY As String = " sum(Sup_Imp) DESC "

            Dim numeroDiRighe = 5
            If Not IsNothing(params.InData.NumeroMovimenti) Then
                numeroDiRighe = params.InData.NumeroMovimenti
            End If

            'Estraggo le stime di produzione solo per le specie con superficie più grande
            Dim ir As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
            Dim dt As DataTable = ir.LeggiSuperficiXWidget(Piva, 0, 0, 0, 0, 0,
                                                     AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, xFiltroAggiuntivo, xOrderBY, objParametri_Server, numeroDiRighe)

            If IsNothing(dt) Then
                r.RispostaStringa = colture_StimeProduzione
                Return r
            End If

            ''''Dim expandoObj = dt.ToExpandoObject
            ''''colture = expandoObj.Where(Function(colt) colt("Superficie") > 0).Select(Function(colt) New Widget_Coltura With {
            ''''            .Veg_Cod = colt("veg_cod"),
            ''''            .Veg_Des = colt("Veg_Des"),
            ''''            .Superficie = colt("Superficie")
            ''''        }).ToList()


            ''''Dim listaVeg_COd As List(Of Integer) = colture.Select(Function(c)
            ''''                                                          Return c.Veg_Cod
            ''''                                                      End Function).Distinct.ToList
            ''''Dim coltureTrovate = String.Join(",", listaVeg_COd)
            Dim stimeproduzione_dal As New Impresa_Progetti_R
            Dim dt_stimeproduzione_colture As DataTable
            For Each drColture In dt.Rows
                dt_stimeproduzione_colture = stimeproduzione_dal.Leggi_StimeProduzione_ColturaOrEsercizi(Piva, drColture("veg_cod"), Nothing,
                                                      objParametri_Server)

                If dt_stimeproduzione_colture.Rows.Count > 0 AndAlso
                        dt_stimeproduzione_colture.Rows(0).Item("HA_TOT") > 0 AndAlso
                        dt_stimeproduzione_colture.Rows(0).Item("STIMA_PRODUZIONE_TOT") > 0 Then
                    colture_StimeProduzione.Add(New Widget_StimeProduzioneColture With {
                            .Veg_Cod = drColture("veg_cod"),
                            .Veg_Des = drColture("veg_des"),
                            .StimaProduzione_Ha = CInt(dt_stimeproduzione_colture.Rows(0).Item("STIMA_PRODUZIONE_TOT") / dt_stimeproduzione_colture.Rows(0).Item("HA_TOT")),
                            .StimaProduzione_Totale = CInt(dt_stimeproduzione_colture.Rows(0).Item("STIMA_PRODUZIONE_TOT"))
                        })
                End If

            Next

            r.RispostaStringa = colture_StimeProduzione
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r


    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiProduzioneColture(ByVal InData As Object) As rispostaStandard(Of List(Of Widget_ProduzioneColtura))

        Dim r As New rispostaStandard(Of List(Of Widget_ProduzioneColtura))

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim params As CoreWS_Generic(Of Widget_Culture_IN) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of Widget_Culture_IN))(JsonConvert.SerializeObject(InData), a)

        If params.objP.objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If
        If params.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If
        If params.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Dim prodColture As New List(Of Widget_ProduzioneColtura)

        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_utenti)

            Dim Piva As String = params.InData.Piva
            'Considero gli impianti validi alla data e quelli che sono stati chiusi durante l'anno solare corrente
            Dim Inizio_Anno_Corrente = "01/01/" & Now.Year
            Dim Fine_Anno_Corrente = "31/12/" & Now.Year

            Dim xFiltroAggiuntivo As String = " Reg_Impianti.Cul_Cod != 0 "
            xFiltroAggiuntivo = xFiltroAggiuntivo + " AND ( ((Reg_Impianti.Validita_inizio <= " + UtilityProvider.Agro_SQL_SaveDate(DateTime.Now)
            xFiltroAggiuntivo = xFiltroAggiuntivo + " AND " + UtilityProvider.Agro_SQL_SaveDate(DateTime.Now) + " <= Reg_Impianti.Validita_Fine)) OR  "
            xFiltroAggiuntivo = xFiltroAggiuntivo + "   ((Reg_Impianti.Validita_Fine >= " + UtilityProvider.Agro_SQL_SaveDate(Inizio_Anno_Corrente)
            xFiltroAggiuntivo = xFiltroAggiuntivo + " AND " + UtilityProvider.Agro_SQL_SaveDate(Fine_Anno_Corrente) + " >= Reg_Impianti.Validita_Fine) ) ) "

            Dim xOrderBY As String = " sum(Sup_Imp) DESC "

            Dim numeroDiRighe = 5
            If Not IsNothing(params.InData.NumeroMovimenti) Then
                numeroDiRighe = params.InData.NumeroMovimenti
            End If

            Dim ir As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
            Dim dt As DataTable = ir.LeggiSuperficiXWidget(Piva, 0, 0, 0, 0, 0,
                                                     AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, xFiltroAggiuntivo, xOrderBY, objParametri_Server, numeroDiRighe)

            If IsNothing(dt) Then
                r.RispostaStringa = prodColture
                Return r
            End If

            Dim expandoObj = dt.ToExpandoObject
            For Each colt In expandoObj
                Dim dtSemina As DataTable = ir.LeggiSuperficiXWidget2(Piva, colt("veg_cod"), LAVCOD_SEMINA, colt("Superficie"), xFiltroAggiuntivo, objParametri_Server)
                Dim dtRaccolta As DataTable = ir.LeggiSuperficiXWidget2(Piva, colt("veg_cod"), LAVCOD_RACCOLTA, colt("Superficie"), xFiltroAggiuntivo, objParametri_Server)

                Dim percentualeRaccolta = 0
                Dim percentualeSemina = 0
                If dtSemina.Rows.Count > 0 Then
                    percentualeSemina = Math.Round(dtSemina(0)("PercentualeSuperficie"), 0)
                    percentualeSemina = If(percentualeSemina > 100, 100, percentualeSemina)
                End If
                If dtRaccolta.Rows.Count > 0 Then
                    percentualeRaccolta = Math.Round(dtRaccolta(0)("PercentualeSuperficie"), 0)
                    percentualeRaccolta = If(percentualeRaccolta > 100, 100, percentualeRaccolta)
                End If


                Dim coltura As New Widget_ProduzioneColtura With
                    {
                        .Veg_Cod = colt("veg_cod"),
                        .Veg_Des = colt("Veg_Des"),
                        .Superficie = colt("Superficie"),
                        .PercentualeRaccolta = percentualeRaccolta,
                        .PercentualeSeminata = percentualeSemina
                    }
                prodColture.Add(coltura)
            Next

            r.RispostaStringa = prodColture
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r


    End Function

    <WebMethod(EnableSession:=True)>
    Public Function LinkGestioneCompleta(ByVal InData As Object) As rispostaStandard(Of String)


        Dim urlCompleto As String = String.Empty
        Dim r As New rispostaStandard(Of String)

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
        Dim params As CoreWS_Generic(Of Widget_LinkGestione_IN) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of Widget_LinkGestione_IN))(JsonConvert.SerializeObject(InData), a)

        If params.objP.objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If

        If params.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If params.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Dim objParametri_Super_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_super_server)
        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_utenti)

        ' Servono per istanziare AgroWebConfig
        HttpContext.Current.Session("ASG_objParametri_Super_Server") = objParametri_Super_Server
        HttpContext.Current.Session("ASG_objParametri_Server") = objParametri_Server
        HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametri_Utenti

        Try

            Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
            Dim hrefsito As String = ""
            Dim qs As New List(Of String)

            Select Case params.InData.CodiceWidget
                Case Enum_Codice_Widget.ProdottiMovimentatiGiacenze
                    hrefsito = objWebConfig.LinkAgronicaAgenda2010
                    hrefsito = hrefsito.Replace("GestioneRichieste.aspx", "GestioneMagazzini/GestioneMagazziniBS.aspx")
                    qs.Add(String.Format("piva={0}", Stringa_Codifica(params.InData.Piva, AgroKey_EncoderDecoder)))
                    qs.Add(String.Format("tab_richiesto={0}", Stringa_Codifica(0, AgroKey_EncoderDecoder)))
                    qs.Add(String.Format("visualizzazione_mode={0}", Stringa_Codifica(0, AgroKey_EncoderDecoder)))
                    qs.Add(String.Format("idBC={0}", Stringa_Codifica(46, AgroKey_EncoderDecoder)))

                Case Else

            End Select

            If qs.Any Then
                urlCompleto = String.Format("{0}?{1}", hrefsito, String.Join("&", qs))
            Else
                urlCompleto = hrefsito
            End If

            r.RispostaOK = True
            r.RispostaStringa = urlCompleto

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiStatistichePrevisioniAI(ByVal InData As Object) As rispostaStandard(Of List(Of Widget_PrevisioniAI))

        Dim r As New rispostaStandard(Of List(Of Widget_PrevisioniAI))

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
        Dim params As CoreWS_Generic(Of Widget_PrevisioniAI_IN) =
                       JsonConvert.DeserializeObject(Of CoreWS_Generic(Of Widget_PrevisioniAI_IN))(JsonConvert.SerializeObject(InData), a)

        If params.objP.objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If

        If params.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If params.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Dim statistichePrevisioniAI As New List(Of Widget_PrevisioniAI)

        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_utenti)

            Dim Piva As String = params.InData.Piva

            Dim statsBIZ As New AgronicaCoreContabBIZ.Statistiche_R
            Dim dt As DataTable = statsBIZ.getStatistiche_PrevisioniAI(params.InData.Piva, params.InData.DataStats, objParametri_Server)

            If IsNothing(dt) Then
                r.RispostaStringa = statistichePrevisioniAI
                Return r
            End If

            Dim expandoObj = dt.ToExpandoObject
            For Each statRow In expandoObj
                Dim stat As New Widget_PrevisioniAI With
                    {
                        .Localita = statRow("LOCALITA"),
                        .Stato = statRow("Stato_Country"),
                        .SpecieVegetale = statRow("Veg_Des"),
                        .Cultivar = statRow("Cul_Des"),
                        .NomeApp = statRow("APP_NOME"),
                        .SuperficieApp = statRow("SUP_APP"),
                        .AgricolturaBio = If(statRow("AgrBio") = 1, "true", "false")
                    }
                statistichePrevisioniAI.Add(stat)
            Next

            r.RispostaStringa = statistichePrevisioniAI
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    Private Function Get_Cache_Key_Attivita(ByVal tipo As String, ByVal stringaConnessione As String, ByVal userName As String, ByVal piva As String) As String

        Dim nomeIstanza As String = DataProviderFactory.Instance.Provider.NomeIstanza_FromStringaConnessione(stringaConnessione)
        Dim nomeServer As String = DataProviderFactory.Instance.Provider.NomeDataBase_FromStringaConnessione(stringaConnessione)
        Dim nomeDBCompleto As String = String.Format("{0}§{1}", nomeIstanza, nomeServer)

        Return String.Format("{0}§{1}§{2}§{3}§{4}§{5}", CACHE_KEY_PREFIX, nomeDBCompleto, NOME_GRUPPO_CACHE, userName, piva, tipo)

    End Function

    Private Class CacheAttivita

        Public ListaConfronto As New List(Of OggettoConfronto)
        Public Attivita As List(Of Widget_Operazione)

    End Class

    Private Class OggettoConfronto : Implements IEquatable(Of OggettoConfronto)

        Public Id_agenda As Integer
        Public Data_Modifica As DateTime

        Public Function Compare(ByVal other As OggettoConfronto) As Boolean
            If other Is Nothing Then Return False
            Return Me.Id_agenda = other.Id_agenda AndAlso Me.Data_Modifica = other.Data_Modifica
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return (Id_agenda, Data_Modifica).GetHashCode()
        End Function

        Public Function Equals(other As OggettoConfronto) As Boolean Implements IEquatable(Of OggettoConfronto).Equals
            Return Compare(TryCast(other, OggettoConfronto))
        End Function
    End Class

End Class