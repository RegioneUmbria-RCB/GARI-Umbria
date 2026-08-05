Imports AgroAgenda_2010.Resources
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class criteri_AggregazioneUC
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Shared Function Carica_Imprese_Idonee(ByVal objP_server As String, ByVal objP_utenti As String) As RispostaStandard
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

            Dim objAnagrafeBIZ As New AgronicaCoreAnagrafeBIZ.Impresa_R

            Dim dt_imprese = objAnagrafeBIZ.Imprese_Leggi_VisibilitaUtente_CUAA(objParametri_Server, objParametri_Utenti,
                                                                                New List(Of Integer)({TipiEnumerativi.enum_Omni_Modulo_Generazione.FreshFood,
                                                                                TipiEnumerativi.enum_Omni_Modulo_Generazione.Zoo,
                                                                                TipiEnumerativi.enum_Omni_Modulo_Generazione.Tabacco}))

            Dim JArrayLista As New JArray()
            For Each dr In dt_imprese.Rows
                JArrayLista.Add(New JObject(New JProperty("piva", dr.Item("piva")), New JProperty("rag_soc", dr.Item("rag_soc"))))
            Next

            r.RispostaOK = True
            r.RispostaStringa = JArrayLista.ToString
        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function

    Public Shared Function Leggi_CriteriAggregazione(ByVal piva As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objCriteriAggregazioneR As New AgronicaCoreContabDAL.Lavorazioni_CriteriAggrega_R
        Dim objOTabelleR As New AgronicaCoreAnagrafeDAL.OTabelle_R

        Dim serializerSettings As New JsonSerializerSettings With {
            .ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        }

        Dim aggregazione As String = ""

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim modificabile = " (" & AgronicaAgenda_2010.Modificabile & ") "

            Dim soloUguali As String = DirectCast(HttpContext.GetLocalResourceObject("~/GestioneLavorazioni/criteri_AggregazioneUC.ascx", "SoloUguali"), String)

            Dim dt = objCriteriAggregazioneR.LeggiPerGriglia(piva, "", "", objParametri_Server)

            For Each row As DataRow In dt.Rows

                Dim params = JsonConvert.DeserializeObject(row.Item("Criteri_Aggiuntivi"))
                Dim qualitativi As String = ""

                If row.Item("Aggrega_Fornitore_Cod") = 0 Then
                    row.Item("Aggrega_Fornitore") = AgronicaAgenda_2010.Tutti
                Else
                    row.Item("Aggrega_Fornitore") = soloUguali
                End If

                If row.Item("Aggrega_Specie_Cod") = 0 Then
                    row.Item("Aggrega_Specie") = AgronicaAgenda_2010.Tutti
                Else
                    row.Item("Aggrega_Specie") = soloUguali
                End If

                If row.Item("Aggrega_Varieta_Cod") = 0 Then
                    row.Item("Aggrega_Varieta") = AgronicaAgenda_2010.Tutti
                Else
                    row.Item("Aggrega_Varieta") = soloUguali
                End If

                If row.Item("Aggrega_Regolamento_Cod") = 0 Then
                    row.Item("Aggrega_Regolamento") = AgronicaAgenda_2010.Tutti
                Else
                    row.Item("Aggrega_Regolamento") = soloUguali
                End If

                If row.Item("Aggrega_Lotto_Cod") = 0 Then
                    row.Item("Aggrega_Lotto") = AgronicaAgenda_2010.Tutti
                Else
                    row.Item("Aggrega_Lotto") = soloUguali
                End If

                If row.Item("Lotto_Modifica_Uscita_Cod") = 1 Then
                    row.Item("Lotto_Modifica_Uscita") = AgronicaAgenda_2010.Si
                Else
                    row.Item("Lotto_Modifica_Uscita") = AgronicaAgenda_2010.No
                End If

                If row.Item("Prodotto_Modifica_Uscita_Cod") = 1 Then
                    row.Item("Prodotto_Modifica_Uscita") = AgronicaAgenda_2010.Si
                Else
                    row.Item("Prodotto_Modifica_Uscita") = AgronicaAgenda_2010.No
                End If

                If row.Item("Aggrega_Prodotto_Cod") = 0 Then
                    row.Item("Aggrega_Prodotto") = AgronicaAgenda_2010.Tutti
                Else
                    row.Item("Aggrega_Prodotto") = soloUguali
                End If

                If row.Item("Cella_Modifica_Uscita_Cod") = 1 Then
                    row.Item("Cella_Modifica_Uscita") = AgronicaAgenda_2010.Si
                Else
                    row.Item("Cella_Modifica_Uscita") = AgronicaAgenda_2010.No
                End If

                If row.Item("Aggrega_Cella_Cod") = 0 Then
                    row.Item("Aggrega_Cella") = AgronicaAgenda_2010.Tutti
                Else
                    row.Item("Aggrega_Cella") = soloUguali
                End If

                If row.Item("Aggrega_Unita_Misura_Cod") = 0 Then
                    row.Item("Aggrega_Unita_Misura") = AgronicaAgenda_2010.Tutti
                Else
                    row.Item("Aggrega_Unita_Misura") = soloUguali
                End If

                For Each param As JToken In params

                    Select Case CInt(param.Item("AggregaParam"))
                        Case 1
                            aggregazione = soloUguali
                        Case 10
                            aggregazione = Gias.Media
                        Case 11
                            aggregazione = Gias.Minimo
                        Case 12
                            aggregazione = Gias.Massimo
                        Case Else
                            aggregazione = AgronicaAgenda_2010.Tutti
                    End Select

                    Dim p = objOTabelleR.Leggi("", 0, 0, param.Item("CodParam"), objParametri_Server)
                    qualitativi = "<p>" & qualitativi & p.Rows.Item(0).Item("Tabella_Des") & ": " & aggregazione & IIf(CInt(param.Item("ModifUscita")) > 0, modificabile, "") & "</p>"

                Next

                row.Item("Criteri_Aggiuntivi") = qualitativi
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    Public Shared Function LeggiPreparazioniGeneriche(ByVal piva As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim DT As New DataTable

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try


            Dim reader As New AgronicaCoreContabDAL.Linee_Preparazioni_R
            DT = reader.Leggi(piva, 0, 0, 0,
                                              "Preparazione_cod != -208",
                                              "Preparazione_Des", objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(DT, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    Public Shared Function LeggiLineePreparazione(ByVal piva As String, ByVal codGenerazione As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim DT As New DataTable

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim reader As New AgronicaCoreContabDAL.Linee_ProduzionixPreparazioni_R
            DT = reader.LeggiDaAziendaECodGenerazione(piva, codGenerazione, "", "", objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(DT, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    Public Shared Function Aggiorna_CriteriAggregazione(ByVal piva As String, ByVal tipoLav As Integer, ByVal tipoLavLinea As Integer, ByVal aggregaFornitore As Integer,
                                                     ByVal aggregaSpecie As Integer, ByVal aggregaVarieta As Integer, ByVal aggregaRegolamento As Integer,
                                                     ByVal aggregaLotto As Integer, ByVal aggregaLottoUscita As Integer, ByVal aggregaProdotto As Integer, ByVal aggregaProdottoUscita As Integer,
                                                     ByVal aggregaCella As Integer, ByVal aggregaCellaUscita As Integer, ByVal aggregaUnitaMisura As Integer,
                                                     ByVal paramQual As String, ByVal azione As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objCriteriAggregazioneR As New AgronicaCoreContabDAL.Lavorazioni_CriteriAggrega_R
        Dim objCriteriAggregazioneW As New AgronicaCoreContabDAL.Lavorazioni_CriteriAggrega_W

        Dim serializerSettings As New JsonSerializerSettings With {
            .ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        }

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            If azione = 0 Then
                Dim dtCriteri = objCriteriAggregazioneR.Leggi(piva, tipoLav, tipoLavLinea, "", "", objParametri_Server)

                If dtCriteri.Rows.Count = 0 Then

                    Dim res = objCriteriAggregazioneW.Nuovo_CriteriAggregazione(piva, tipoLav, tipoLavLinea, aggregaFornitore, aggregaSpecie, aggregaVarieta, aggregaRegolamento,
                                                    aggregaLotto, aggregaLottoUscita, aggregaProdotto, aggregaProdottoUscita, aggregaCella, aggregaCellaUscita, aggregaUnitaMisura,
                                                    paramQual, objParametri_Server)

                    r.RispostaStringa = JsonConvert.SerializeObject(res, Formatting.None, serializerSettings)

                    r.RispostaOK = True
                Else

                    r.RispostaOK = False

                    r.Errore = "Esiste gia un criterio legato a Piva, Lavorazione e Linea Lavorazione specificati"

                End If

            Else
                Dim res = objCriteriAggregazioneW.Modifica_CriteriAggregazione(piva, tipoLav, tipoLavLinea, aggregaFornitore, aggregaSpecie, aggregaVarieta, aggregaRegolamento,
                                                    aggregaLotto, aggregaLottoUscita, aggregaProdotto, aggregaProdottoUscita, aggregaCella, aggregaCellaUscita, aggregaUnitaMisura,
                                                    paramQual, objParametri_Server)

                r.RispostaStringa = JsonConvert.SerializeObject(res, Formatting.None, serializerSettings)

                r.RispostaOK = True
            End If

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    Public Shared Function CancellaCriterioAggregazione(ByVal cancellate As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objCriteriAggregazioneW As New AgronicaCoreContabDAL.Lavorazioni_CriteriAggrega_W
        Dim res As Boolean = True

        Dim serializerSettings As New JsonSerializerSettings With {
            .ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        }

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim righeCancellate = JArray.Parse(cancellate)

            For Each riga As JObject In righeCancellate

                res = res AndAlso objCriteriAggregazioneW.CancellaCriterioAggregazione(riga.Item("Piva"), riga.Item("Tipologia_Lavorazione_Cod"),
                                                                                       riga.Item("Tipologia_Lavorazione_Linea_Cod"), "", objParametri_Server)

            Next
            If res Then

                r.RispostaStringa = JsonConvert.SerializeObject(res, Formatting.None, serializerSettings)

                r.RispostaOK = True
            Else

                r.RispostaOK = False

                r.Errore = "Esiste gia un criterio legato a Piva, Lavorazione e Linea Lavorazione specificati"

            End If

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

End Class


