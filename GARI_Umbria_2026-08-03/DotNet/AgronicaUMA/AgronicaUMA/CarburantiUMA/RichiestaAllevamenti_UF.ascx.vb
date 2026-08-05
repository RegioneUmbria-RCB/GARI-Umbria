Imports System.Net.NetworkInformation
Imports System.Web
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreUmaBiz
Imports AgronicaCoreUmaDal
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class RichiestaAllevamenti_UF
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Shared Function LeggiColtureUF(ByVal piva As String, ByVal richiestaCod As Integer) As RispostaStandard
        Dim r As New RispostaStandard()
        r.RispostaOK = True

        Try
            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Dim handleAllevamenti As New UMA_Richieste_Allevamenti_UF_R()
            Dim leggiUFColture As New UMA_UF_Colture_R
            Dim leggitestata As New UMA_Richieste_Testata_R
            Dim leggiLavorazioni As New UMA_Richieste_Lavorazioni_R
            Dim irrigua As Boolean

            Dim dtRichiesta = leggitestata.LeggiDaRichiestaCod(richiestaCod, objParametri_Server)

            Dim dtAll = handleAllevamenti.LeggiColtureUF(piva, richiestaCod, 0,
                                                         0, "", "",
                                                         "", "", "", "",
                                                         objParametri_Server)

            Dim xFiltroAggiuntivo As String = ""

            leggiUFColture.ComponiFiltroAggiuntivoValidita(xFiltroAggiuntivo, CDate(dtRichiesta.Rows.Item(0).Item("Validita_Inizio")).ToShortDateString, CDate(dtRichiesta.Rows.Item(0).Item("Validita_Fine")).ToShortDateString)

            For Each ufprod As DataRow In dtAll.Rows

                If ufprod.Item("Tipo_Territorio") = 1 Then
                    Dim Dtlav = leggiLavorazioni.Leggi(piva,
                                                   ufprod.Item("macrouso_Uma_Cod"),
                                                   ufprod.Item("programmazione_cod"),
                                                   richiestaCod,
                                                   0,
                                                   enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                   objParametri_Server)
                    If Dtlav.Select("Lavorazione_GIAS = 1").Count > 0 Then
                        irrigua = True
                    Else
                        irrigua = False
                    End If
                Else
                    irrigua = True
                End If

                Dim dtUFProd = leggiUFColture.LeggiUF(ufprod.Item("Occupazione_Cod"),
                                                  ufprod.Item("Destinazione_Cod"),
                                                  ufprod.Item("Uso_Cod"),
                                                  ufprod.Item("Qualita_Cod"),
                                                  irrigua,
                                                  xFiltroAggiuntivo,
                                                  "",
                                                  objParametri_Server)

                If dtUFProd.Rows.Count > 0 Then
                    If dtUFProd.Rows.Item(0).Item("UF") > 0 OrElse dtUFProd.Rows.Item(0).Item("UFC") > 0 OrElse dtUFProd.Rows.Item(0).Item("UFL") > 0 Then
                        ufprod.Item("Produce_UF") = "SI"
                    End If
                End If

                If IsDBNull(ufprod.Item("Programmazione_Des")) Then
                    ufprod.Item("Programmazione_Des") = "Piano Colturale"
                End If

            Next


            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(dtAll, Formatting.None, serializerSettings)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r
    End Function

    Public Shared Function AggiungiRigheFascicolo_UF(ByVal piva As String,
                                                     ByVal richiestaCod As Integer,
                                                     ByVal programmazione_cod As Integer) As RispostaStandard

        Dim richieste = New UMA_Richieste_R
        Dim testata = New UMA_Richieste_Testata_R
        Dim scrivi = New UMA_Richieste_Allevamenti_UF_W
        Dim leggi = New UMA_Richieste_Allevamenti_UF_R
        Dim r As New RispostaStandard()

        r.RispostaOK = True

        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Dim datiRichiestaFascicoloSelezionato = richieste.Leggi(piva,
                                                                    richiestaCod,
                                                                    "",
                                                                    programmazione_cod,
                                                                    enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                    objParametri_Server,
                                                                    EscludiFascicoliFittizi:=True)

            If IsNothing(datiRichiestaFascicoloSelezionato) OrElse datiRichiestaFascicoloSelezionato.Rows.Count = 0 Then
                r.Errore = "Fascicolo non utilizzato nella pratica corrente"
                r.RispostaOK = False
            End If

            If r.RispostaOK Then

                Dim colturePresenti = leggi.LeggiColtureUF(piva, richiestaCod, 0, 0, "", "", "", "", "", "", objParametri_Server)
                Dim fascicoli = colturePresenti.DefaultView.ToTable(True, "Programmazione_Cod")

                If fascicoli.Select("Programmazione_Cod = " & programmazione_cod & "").Count > 0 Then
                    r.Errore = "Fascicolo già inserito. Per inserirlo nuovamente è necessario eliminare ogni coltura di quel fascicolo"
                    r.RispostaOK = False
                End If

            End If
            If r.RispostaOK Then

                Dim anno = testata.RecuperaAnnoRichiesta(richiestaCod, objParametri_Server)
                Dim terreniINUmbria As Dictionary(Of Tuple(Of String, String, String, String, String), Decimal)
                Dim terreniFuoriUmbria As Dictionary(Of Tuple(Of String, String, String, String, String), Decimal)

                If anno >= 2025 Then
                    terreniINUmbria = TrovaColture2025(anno, piva, True, objParametri_Server)
                    terreniFuoriUmbria = TrovaColture2025(anno, piva, False, objParametri_Server)
                Else
                    terreniINUmbria = TrovaColture(programmazione_cod, piva, True, objParametri_Server)
                    terreniFuoriUmbria = TrovaColture(programmazione_cod, piva, False, objParametri_Server)
                End If

                For Each x In terreniInUmbria
                    scrivi.InserisciRiga(piva, richiestaCod, programmazione_cod, 1,
                                            x.Key.Item2, x.Key.Item3, x.Key.Item4, x.Key.Item5, x.Key.Item1,
                                            x.Value, objParametri_Server)
                Next

                For Each x In terreniFuoriUmbria
                    scrivi.InserisciRiga(piva, richiestaCod, programmazione_cod, 2,
                                            x.Key.Item2, x.Key.Item3, x.Key.Item4, x.Key.Item5, x.Key.Item1,
                                            x.Value, objParametri_Server)
                Next

                Dim serializerSettings As New JsonSerializerSettings()
                serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                r.RispostaStringa = JsonConvert.SerializeObject("", Formatting.None, serializerSettings)

            End If

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    Public Shared Function UC_Allevamenti_UF_EliminaModificaRighe(ByVal piva As String, ByVal richiestaCod As Integer,
                                                          ByVal righeCancellate As String, ByVal righeModificate As String) As RispostaStandard
        Dim r As New RispostaStandard
        Dim Dt As New DataTable
        Dim Dtres As New DataTable

        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim cancellate_Arr = JArray.Parse(righeCancellate)
            Dim modificate_Arr = JArray.Parse(righeModificate)

            Dim modAllevamentiUF As New AgronicaCoreUmaDal.UMA_Richieste_Allevamenti_UF_W

            For Each rigaCancellata As JObject In cancellate_Arr
                modAllevamentiUF.CancellaRiga(piva, richiestaCod, CInt(rigaCancellata.Item("Programmazione_Cod")),
                                              CInt(rigaCancellata.Item("Tipo_Territorio")),
                                              CStr(rigaCancellata.Item("Occupazione_Cod")),
                                              CStr(rigaCancellata.Item("Destinazione_Cod")),
                                              CStr(rigaCancellata.Item("Uso_Cod")),
                                              CStr(rigaCancellata.Item("Qualita_Cod")), objParametri_Server)
            Next

            For Each rigaModificata As JObject In modificate_Arr
                modAllevamentiUF.ModificaRiga(piva, richiestaCod, CInt(rigaModificata.Item("Programmazione_Cod")),
                                              CInt(rigaModificata.Item("Tipo_Territorio")),
                                              CStr(rigaModificata.Item("Occupazione_Cod")),
                                              CStr(rigaModificata.Item("Destinazione_Cod")),
                                              CStr(rigaModificata.Item("Uso_Cod")),
                                              CStr(rigaModificata.Item("Qualita_Cod")),
                                              CDbl(rigaModificata.Item("Superf_Calcolo")),
                                              objParametri_Server)
            Next

            r.RispostaStringa = "Fatto"
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    Public Shared Function UC_Allevamenti_UF_ControlloCapiAllevabili(ByVal piva As String, ByVal richiestaCod As Integer) As RispostaStandard
        Dim r As New RispostaStandard
        Dim umaUF As New UMA_UF
        Dim strCapiInEccesso As String

        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            strCapiInEccesso = umaUF.UF_ControlloCapiAllevabili(piva, richiestaCod, objParametri_Server)

            r.RispostaStringa = ""
            r.Errore = strCapiInEccesso
            r.RispostaOK = IIf(strCapiInEccesso = "", True, False)

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    Public Shared Function UC_Allevamenti_UF_SalvaAllevatiInMontagna(ByVal richiestaCod As Integer, ByVal allevatoMontagna As Boolean) As RispostaStandard
        Dim r As New RispostaStandard
        Dim umaTesta As New UMA_Richieste_Testata_W
        Dim res As Boolean

        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            res = umaTesta.Modifica_Allevati_In_Montagna(allevatoMontagna, richiestaCod, objParametri_Server)

            r.RispostaStringa = "" + res.ToString + ""
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    Private Shared Function TrovaColture(ByVal programmazione_Cod As Integer,
                                  ByVal piva As String,
                                  ByVal terreniInUmbria As Boolean,
                                  ByRef objParametri_Server As AgronicaCoreParametri) As Dictionary(Of Tuple(Of String, String, String, String, String), Decimal)

        Dim entDictionary As New Dictionary(Of Tuple(Of String, String, String, String, String), Decimal) 'MacrousoUMA, occupazione -> destinazione, uso, qualita, superficie
        Dim DtEntita As DataTable
        Dim ageaCodifiche As New UMA_Richieste_Allevamenti_UF_R
        Dim entita = New Programmazione_Entita_R
        Dim dtCodifiche As New DataTable
        Dim macrouso_UMA_Temp As String
        Dim filtro_aggiuntivo As String = "EXISTS (SELECT TOP 1 * FROM Programmazione_Particelle WHERE Programmazione_Particelle.Programmazione_Entita_Cod = Programmazione_entita.Programmazione_Entita_Cod AND Programmazione_Particelle.prov in "

        If terreniInUmbria Then
            filtro_aggiuntivo += " ('054', '055')) "
        Else
            filtro_aggiuntivo += " ('041', '042', '043', '044', '045', '046','047','048','049','050','051','052','053','056','057','058','059', '060')) "
        End If

        DtEntita = entita.Programmazione_Entita_Leggi(programmazione_Cod, "", 0, "", piva, 0, 0, 0, 0, 0,
                                                      #1/1/2000 12:00 PM#, DateTime.Now, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                      filtro_aggiuntivo,
                                                      "Macrouso_Cod, Occupazione_Cod_Agea, Cul_Cod_Agea", objParametri_Server, Lettura_Per_UMA:=True)

        Dim DtEntitaDistinct = DtEntita.DefaultView.ToTable(False, "Macrouso_Cod", "Occupazione_Cod_Agea", "Cul_Cod_Agea", "Veg_Cod", "Id_Cod", "Destinazione_Cod_Agea", "Uso_Cod_Agea", "Qualita_Cod_Agea", "Superficie")

        For Each row As DataRow In DtEntitaDistinct.Rows

            dtCodifiche = ageaCodifiche.EstraiMacrousoUmaDaAgea(CStr(row.Item("Occupazione_Cod_Agea")),
                                                                CStr(row.Item("Destinazione_Cod_Agea")),
                                                                CStr(row.Item("Uso_Cod_Agea")),
                                                                CStr(row.Item("Qualita_Cod_Agea")), objParametri_Server)

            If dtCodifiche.Rows.Count > 0 Then
                macrouso_UMA_Temp = CStr(dtCodifiche.Rows.Item(0).Item("Macrouso_Uma_Cod"))
            Else
                macrouso_UMA_Temp = CStr(row.Item("Macrouso_Cod"))
            End If

            Dim tempKey = Tuple.Create(macrouso_UMA_Temp, CStr(row.Item("Occupazione_Cod_Agea")), CStr(row.Item("Destinazione_Cod_Agea")),
                                               CStr(row.Item("Uso_Cod_Agea")),
                                               CStr(row.Item("Qualita_Cod_Agea")))

            If entDictionary.ContainsKey(tempKey) Then
                Dim temp = entDictionary.Item(tempKey)
                Dim supTot = temp + CDec(row.Item("Superficie"))
                entDictionary.Remove(tempKey)
                entDictionary.Add(tempKey,
                                  supTot)
            Else
                entDictionary.Add(tempKey, CDec(row.Item("Superficie")))
            End If

        Next

        Return entDictionary

    End Function

    Private Shared Function TrovaColture2025(ByVal anno As Integer,
                                  ByVal piva As String,
                                  ByVal terreniInUmbria As Boolean,
                                  ByRef objParametri_Server As AgronicaCoreParametri) As Dictionary(Of Tuple(Of String, String, String, String, String), Decimal)

        Dim entDictionary As New Dictionary(Of Tuple(Of String, String, String, String, String), Decimal) 'MacrousoUMA, occupazione -> destinazione, uso, qualita, superficie
        Dim DtEntita As DataTable
        Dim umaRichiesta As New UMA_Richieste_R

        DtEntita = umaRichiesta.TrovaColtureAppezzamentiSenzaFascicolo(piva, 0, objParametri_Server, terreniInUmbria:=terreniInUmbria, anno:=anno)

        Dim DtEntitaDistinct = DtEntita.DefaultView.ToTable(False, "Macrouso_Uma_Cod", "Occupazione_Agea", "Destinazione_Agea", "Uso_Agea", "Qualita_Agea", "SUP_APP")

        For Each row As DataRow In DtEntitaDistinct.Rows

            Dim tempKey = Tuple.Create(CStr(row.Item("Macrouso_Uma_Cod")), CStr(row.Item("Occupazione_Agea")), CStr(row.Item("Destinazione_Agea")),
                                               CStr(row.Item("Uso_Agea")),
                                               CStr(row.Item("Qualita_Agea")))

            If entDictionary.ContainsKey(tempKey) Then
                Dim temp = entDictionary.Item(tempKey)
                Dim supTot = temp + CDec(row.Item("SUP_APP"))
                entDictionary.Remove(tempKey)
                entDictionary.Add(tempKey,
                                  supTot)
            Else
                entDictionary.Add(tempKey, CDec(row.Item("SUP_APP")))
            End If

        Next

        Return entDictionary

    End Function

End Class