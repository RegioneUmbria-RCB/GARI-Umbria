Imports System.Transactions
Imports AgronicaCoreAnagrafeBIZ
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreBudgetDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModelsSTD.exceptions
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class Budget_Testata_R

End Class

Public Class Budget_Testata_W
#Region "RIBALTAMENTO BUDGET"
    Public Shared Function Ribalta_Budget_Da_Testata(Id_Budget As Integer,
                                                     piva_list As List(Of String),
                                                     specie_utilizzo_list As List(Of String),
                                                     objParametri_Server As AgronicaCoreParametri,
                                                     objParametri_Utenti As AgronicaCoreParametri
                                                     ) As RispostaStandard

        'RIBALTO I PIANI COLTURALI DIVISI PER PIVA
        'UNA PIVA = UNA TRANSAZIONE

        Dim r As New RispostaStandard

        Try

            Dim list_msg_non_ribaltati As New List(Of String)
            Dim xFiltroAggiuntivo As String = ""

            Dim specie_list As New List(Of String)
            Dim utilizzo_list As New List(Of String)

            For Each elem In specie_utilizzo_list
                Dim dummy = elem.Split("/")
                If dummy(0) = "0" Then
                    utilizzo_list.Add(dummy(1))
                Else
                    specie_list.Add(dummy(0))
                End If
            Next
            If specie_list.Count > 0 OrElse utilizzo_list.Count > 0 Then
                xFiltroAggiuntivo += "("
            End If
            If specie_list.Count > 0 Then
                xFiltroAggiuntivo += " s.Veg_Cod IN (" & String.Join(", ", specie_list) & ")"
            End If
            If utilizzo_list.Count > 0 Then
                If xFiltroAggiuntivo <> "" Then
                    xFiltroAggiuntivo += " OR "
                End If
                xFiltroAggiuntivo += " Codici_Anagrafe.Codice IN (" & String.Join(", ", utilizzo_list) & ")"
            End If
            If xFiltroAggiuntivo <> "" Then
                xFiltroAggiuntivo += ")"
            End If

            Dim impianto As String = ""
            For Each _piva In piva_list

                Using scope As New TransactionScope()
                    Using GiasContext As Gias_DeveloperServer_Entities = Gias_EF_Utility.CreateGiasContextConnection(objParametri_Server.StringaConnessione)

                        Dim _rag_soc As String = ""

                        Try

                            Dim reg_impianti_dest = (From c In GiasContext.Budget_Reg_Impianti_Codici
                                                     Where c.PIVA = _piva And
                                                         c.Id_Budget = Id_Budget And
                                                         c.id_cod >= 3000 And c.id_cod < 4000).ToList

                            Dim objImprese_Progetti As New AgronicaCoreBudgetDAL.Budget_Impresa_Progetti_R
                            Dim dt = objImprese_Progetti.Leggi_x_anagraficaNG(Id_Budget, _piva, 0, 0, 0, 0,
                                                                              xFiltroAggiuntivo,
                                                                              "", objParametri_Server)

                            Dim primo_elem = dt.Rows(0)
                            Dim ultimo_elem = dt.Rows(dt.Rows.Count - 1)

                            Dim ribaltati_con_successo As Integer = 0

                            Dim list_msg_gestiti As New List(Of String)
                            Dim list_msg_non_gestiti As New List(Of String)

                            Dim dicAnagrafica_Ribaltata As New AgronicaCoreAnagrafeBIZ.Anagrafica_Ribaltata

                            _rag_soc = (From i In GiasContext.Imprese
                                        Where i.PIVA = _piva
                                        Select i.rag_soc).FirstOrDefault()

                            For Each app As DataRow In dt.Rows
                                Dim APPEZZA = app.Item("APPEZZA")
                                Dim SA_COD = app.Item("SA_COD")

                                impianto = app.Item("APP_NOME") & " (" & app.Item("Utilizzo") & ")"

                                Dim isFirst As Boolean = app.Equals(primo_elem)
                                Dim isLast As Boolean = app.Equals(ultimo_elem)

                                Dim objApp As New AgronicaCoreModelsSTD.anagrafiche.Appezzamento With {
                                    .primaryKey = New AgronicaCoreModelsSTD.anagrafiche.Appezzamento.PK(APPEZZA, New AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK(SA_COD, _piva))
                                }

                                Dim msg_errore_gestito_movimenti As String = ""
                                RibaltamentoAnagraficheColturali_W.Ribalta_BudgetReale(Id_Budget, objApp,
                                                                                       dicAnagrafica_Ribaltata, ribaltati_con_successo,
                                                                                       msg_errore_gestito_movimenti, isFirst, isLast,
                                                                                       objParametri_Server, objParametri_Utenti, GiasContext)

                                If msg_errore_gestito_movimenti <> "" Then
                                    Throw New Exception(Gias.ImpossibileRibaltareImpiantoSuPianoColturaleEffettivoMovimentiAssociati)
                                End If

                                GiasContext.SaveChanges()
                            Next

                            scope.Complete()

                        Catch ex As Exception
                            list_msg_non_ribaltati.Add(String.Format(Gias.ImpossibileRibaltareImpresaXSuPianoColturaleEffettivoImpiantoYGeneratoErrore, "<b>" & _rag_soc & "</b>", impianto) & ": " & ex.Message)
                        Finally
                            scope.Dispose()
                        End Try

                    End Using
                End Using
            Next

            Dim messaggio As String = ""
            If list_msg_non_ribaltati.Count > 0 Then
                Dim ErroreGias As New ErroreGias
                messaggio += String.Join(NEWLINE & NEWLINE, list_msg_non_ribaltati)

                ErroreGias.messaggio = messaggio
                ErroreGias.severity = ErroreGias_Severity.Info
                r.ErroriGias.Add(ErroreGias)
            Else
                r.RispostaStringa = "Ribaltamento budget effettuato correttamente"
            End If

            r.RispostaOK = True
            r.ParametroDue = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r
    End Function
#End Region

#Region "COPIA BUDGET"
    Public Shared chiavi_da_escludere As String() = {"id_budget", "appezza", "id_reg", "progetto_cod", "campo_cod", "id", "cod_indirizzo"}

    Public Shared Function Copia_Budget(piva As String, IdBudget As Int32,
                                        piano As Boolean,
                                        costi As Boolean, ricavi As Boolean,
                                        specie As String,
                                        tariffe As Boolean,
                                        macchine As Boolean, prodotti As Boolean,
                                        objParametri_Server As AgronicaCoreParametri
                                        ) As RispostaStandard

        Dim r As New RispostaStandard
        Dim dtBudget As DataTable
        Dim dtQxT As DataTable
        Dim dtMac As DataTable
        Dim dtCDG As DataTable
        Dim dtAgenda As DataTable
        Dim dtCDGMano As New DataTable
        Dim dtCDGLibera As New DataTable
        Dim dtCDGMagaz As New DataTable
        Dim dtCDGEredita As New DataTable
        Dim dtCDGMacchine As New DataTable
        Dim lastAgenda As Int32 = 0

        Dim val_Inizio As DateTime
        Dim val_Fine As DateTime
        Dim a = New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim specieList As New List(Of Integer)
        If specie <> "[]" Then
            For Each specie In specie.Substring(1, specie.Length - 1).Substring(0, specie.Length - 2).Split(",")
                specieList.Add(CInt(specie))
            Next
        End If

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
        Dim GiasContext As Gias_DeveloperServer_Entities = Nothing


        'Istanzio la transazione forzando l'uso di una nuova transazione
        Using ts As New TransactionScope(TransactionScopeOption.RequiresNew, New TimeSpan(0, 10, 0))
            Dim OpenNewTransaction As Boolean = False
            Dim Flag_ConnessioneLocale As Boolean = True

            Try
                GiasContext = New Gias_DeveloperServer_Entities(EFConnString)
                'Open the contextObject connection state explicitly
                GiasContext.Database.Connection.Open()

                Utility.VerificaApriConnessione(objParametri_Server, Flag_ConnessioneLocale)

                Dim objBudget_R As New AgronicaCoreBudgetDAL.Budget_Testata_R
                Dim objBudget_W As New AgronicaCoreBudgetDAL.Budget_Testata_W
                Dim ObjQualificheXTariffe_R As New AgronicaCoreContabDAL.QualificheXTariffe_R
                Dim ObjQualificheXTariffe_W As New AgronicaCoreContabDAL.QualificheXTariffe_W
                Dim ObjProdottiCosti_R As New AgronicaCoreContabDAL.Prodotti_Costi_R
                Dim ObjProdottiCosti_W As New AgronicaCoreContabDAL.Prodotti_Costi_W
                Dim ObjCDG_DAL_R As New AgronicaCoreContabDAL.CDG_DAL_R
                Dim ObjCDG_DAL_W As New AgronicaCoreContabDAL.CDG_DAL_W
                Dim scrivi As New AgronicaCoreContabBIZ.CDG_BIZ_W

                Dim IdsCopie As Tuple(Of List(Of Tuple(Of Tuple(Of String, Integer, Integer), Integer)),
                                         List(Of Tuple(Of Tuple(Of String, Integer, Integer, Integer), Integer)),
                                         List(Of Tuple(Of Integer, Integer)),
                                         List(Of Tuple(Of Tuple(Of String, Integer, Integer), Integer)))

                Dim objSequenze_R = New Agro_Sequenze

                dtBudget = objBudget_R.Leggi(piva, " Id_Budget = " + IdBudget.ToString + " ", "", objParametri_Server)

                Dim Id_BudgetNuovo As Integer = objSequenze_R.NuovoId_Tabella("Budget_Testata", 1, 2000000000, objParametri_Server)
                Dim budgetRow = dtBudget.Rows(0)

                val_Inizio = JsonConvert.DeserializeObject(Of Date)(JsonConvert.SerializeObject(budgetRow.Item("Validita_Inizio")), a)
                val_Fine = JsonConvert.DeserializeObject(Of Date)(JsonConvert.SerializeObject(budgetRow.Item("Validita_Fine")), a)

                objBudget_W.Scrivi(piva, Id_BudgetNuovo, CStr(budgetRow.Item("Nome_Budget")) & " - " & Gias.Copia, CInt(objBudget_R.Leggi(piva, " Nome_Budget = '" + budgetRow.Item("Nome_Budget") + "' ", " Revisione DESC", objParametri_Server).Rows(0).Item("Revisione")) + 1,
                                   CInt(0), budgetRow.Item("Tipo_Budget"), budgetRow.Item("Sa_Cod"), CInt(budgetRow.Item("Listino_Cod_Costi")),
                                   CInt(budgetRow.Item("Listino_Cod_Ricavi")), val_Inizio, val_Fine, objParametri_Server)

                If piano Then

                    IdsCopie = Copia_Piano_Colturale(IdBudget, objParametri_Server, objSequenze_R, Id_BudgetNuovo, specieList, GiasContext)

                End If

                dtCDG = ObjCDG_DAL_R.Leggi_CDG_Testata_Da_Budget(IdBudget, "", objParametri_Server)

                If costi Then

                    For Each CDGRow As DataRow In dtCDG.Select("Costi_Ricavi = 0")

                        Dim dtx = ObjCDG_DAL_R.Leggi_CDG(CDGRow.Item("piva"), 0, CDGRow.Item("Id_Mov_Det"), CDGRow.Item("Id_Agenda"),
                                                         Date.Now.Date.ToShortDateString, 0, 0, "", 0, objParametri_Server, Budget:=2)
                        Dim dt = JsonConvert.DeserializeObject(dtx)

                        If (CDGRow.Item("Id_Agenda") <> lastAgenda) Then
                            Dim FiltroImpu As String = ""

                            If Not piano Then
                                Dim listImp As New List(Of Integer)
                                For Each row As DataRow In ObjCDG_DAL_R.Leggi_Dettagli(CDGRow.Item("piva"), 0, 0, 0, 0, CDGRow.Item("Id_CDG"), objParametri_Server).Rows
                                    If row.Item("Id_Imputazione") > 0 Then
                                        listImp.Add(row.Item("ID_CDG"))
                                    End If
                                Next
                                If listImp.Count = 0 Then
                                    Continue For
                                Else
                                    FiltroImpu = " AND ID_CDG " + QueryBuilderUtility.GeneraClausolaINDaList(listImp)
                                End If
                            End If

                            dtAgenda = JsonConvert.DeserializeObject(Of DataTable)(ObjCDG_DAL_R.Leggi_Agenda(CDGRow.Item("piva"), CDGRow.Item("Id_Agenda"), objParametri_Server, IdBudget))

                            scrivi.AggiornaCDG(CDGRow.Item("Piva"),
                                               0, 0, 0, dtAgenda.Rows(0).Item("des_lib"), CDate(CDGRow.Item("Data_Inserimento")), CDGRow.Item("Modalita_Imputazione"),
                                               0, Id_BudgetNuovo, 0, CDGRow.Item("Modalita_Ripartizione"), 0, 0, False, 0,
                                               IIf(IsNothing(dt.item("kendo_Manodopera")), "", JsonConvert.SerializeObject(dt.item("kendo_Manodopera"), Formatting.None)), "",
                                               IIf(IsNothing(dt.item("kendo_Terzisti")), "", JsonConvert.SerializeObject(dt.item("kendo_Terzisti"), Formatting.None)), "",
                                               IIf(IsNothing(dt.item("kendo_Macchine")), "", JsonConvert.SerializeObject(dt.item("kendo_Macchine"), Formatting.None)), "",
                                               IIf(IsNothing(dt.item("kendo_Magazzino")), "", JsonConvert.SerializeObject(dt.item("kendo_Magazzino"), Formatting.None)), "",
                                               IIf(IsNothing(dt.item("kendo_Libera")), "", JsonConvert.SerializeObject(dt.item("kendo_Libera"), Formatting.None)), "",
                                               IIf(IsNothing(dt.item("kendo_Eredita")), "", JsonConvert.SerializeObject(dt.item("kendo_Eredita"), Formatting.None)), "",
                                               IIf(IsNothing(dt.item("kendo_Impianti_Dettagli")), "", JsonConvert.SerializeObject(dt.item("kendo_Impianti_Dettagli"), Formatting.None)), "",
                                               IIf(IsNothing(dt.item("kendo_Progetti_Dettagli")), "", JsonConvert.SerializeObject(dt.item("kendo_Progetti_Dettagli"), Formatting.None)), "",
                                               IIf(IsNothing(dt.item("kendo_Macchine_Dettagli")), "", JsonConvert.SerializeObject(dt.item("kendo_Macchine_Dettagli"), Formatting.None)), "",
                                               IIf(IsNothing(dt.item("kendo_Linee_Dettagli")), "", JsonConvert.SerializeObject(dt.item("kendo_Linee_Dettagli"), Formatting.None)), "",
                                               IIf(IsNothing(dt.item("kendo_Zoo_Dettagli")), "", JsonConvert.SerializeObject(dt.item("kendo_Zoo_Dettagli"), Formatting.None)), "",
                                               objParametri_Server,
                                               GiasContext:=GiasContext, OpenNewTransaction:=OpenNewTransaction)

                            lastAgenda = CDGRow.Item("Id_Agenda")
                        End If
                    Next
                End If

                lastAgenda = 0
                If ricavi AndAlso piano Then

                    For Each CDGRow As DataRow In dtCDG.Select("Costi_Ricavi = 1")

                        Dim dtx = ObjCDG_DAL_R.Leggi_CDG(CDGRow.Item("piva"), 0, CDGRow.Item("Id_Mov_Det"), CDGRow.Item("Id_Agenda"),
                                                         Date.Now.Date.ToShortDateString, 0, 0, "", 0, objParametri_Server, Budget:=2)
                        Dim dt = JsonConvert.DeserializeObject(dtx)

                        If (CDGRow.Item("Id_Agenda") <> lastAgenda) Then

                            dtAgenda = JsonConvert.DeserializeObject(Of DataTable)(ObjCDG_DAL_R.Leggi_Agenda(CDGRow.Item("piva"), CDGRow.Item("Id_Agenda"), objParametri_Server, IdBudget))

                            scrivi.AggiornaCDG(CDGRow.Item("Piva"),
                                               0, 0, 0, dtAgenda.Rows(0).Item("des_lib"), CDate(CDGRow.Item("Data_Inserimento")), CDGRow.Item("Modalita_Imputazione"),
                                               0, Id_BudgetNuovo, 1, CDGRow.Item("Modalita_Ripartizione"), 0, 0, False, 0,
                                               IIf(IsNothing(dt.item("kendo_Manodopera")), "", JsonConvert.SerializeObject(dt.item("kendo_Manodopera"), Formatting.None)), "",
                                               IIf(IsNothing(dt.item("kendo_Terzisti")), "", JsonConvert.SerializeObject(dt.item("kendo_Terzisti"), Formatting.None)), "",
                                               IIf(IsNothing(dt.item("kendo_Macchine")), "", JsonConvert.SerializeObject(dt.item("kendo_Macchine"), Formatting.None)), "",
                                               IIf(IsNothing(dt.item("kendo_Magazzino")), "", JsonConvert.SerializeObject(dt.item("kendo_Magazzino"), Formatting.None)), "",
                                               IIf(IsNothing(dt.item("kendo_Libera")), "", JsonConvert.SerializeObject(dt.item("kendo_Libera"), Formatting.None)), "",
                                               IIf(IsNothing(dt.item("kendo_Eredita")), "", JsonConvert.SerializeObject(dt.item("kendo_Eredita"), Formatting.None)), "",
                                               IIf(IsNothing(dt.item("kendo_Impianti_Dettagli")), "", JsonConvert.SerializeObject(dt.item("kendo_Impianti_Dettagli"), Formatting.None)), "",
                                               IIf(IsNothing(dt.item("kendo_Progetti_Dettagli")), "", JsonConvert.SerializeObject(dt.item("kendo_Progetti_Dettagli"), Formatting.None)), "",
                                               IIf(IsNothing(dt.item("kendo_Macchine_Dettagli")), "", JsonConvert.SerializeObject(dt.item("kendo_Macchine_Dettagli"), Formatting.None)), "",
                                               IIf(IsNothing(dt.item("kendo_Linee_Dettagli")), "", JsonConvert.SerializeObject(dt.item("kendo_Linee_Dettagli"), Formatting.None)), "",
                                               IIf(IsNothing(dt.item("kendo_Zoo_Dettagli")), "", JsonConvert.SerializeObject(dt.item("kendo_Zoo_Dettagli"), Formatting.None)), "",
                                               objParametri_Server,
                                               GiasContext:=GiasContext, OpenNewTransaction:=OpenNewTransaction)

                            lastAgenda = CDGRow.Item("Id_Agenda")

                        End If
                    Next

                End If

                If piano And (costi Or ricavi) Then
                    ObjCDG_DAL_W.Modifica_Dettaglio_Per_Copia_Budget(Id_BudgetNuovo, IdsCopie.Item1, IdsCopie.Item2, IdsCopie.Item3, IdsCopie.Item4, objParametri_Server, GiasContext, OpenNewTransaction)
                End If

                If tariffe Then
                    dtQxT = ObjQualificheXTariffe_R.Leggi("", 0, 0, 0, "", "", objParametri_Server, IdBudget:=IdBudget)

                    For Each QxTRow As DataRow In dtQxT.Rows

                        ObjQualificheXTariffe_W.Scrivi(QxTRow.Item("Qualifica_Cod"), QxTRow.Item("Tariffa_Cod"), QxTRow.Item("Tipo"), QxTRow.Item("Valore"),
                                                       QxTRow.Item("Validita_Inizio"), QxTRow.Item("Validita_Fine"), objParametri_Server, Id_BudgetNuovo)

                    Next
                End If

                If macchine Then
                    dtMac = ObjProdottiCosti_R.Leggi_Macchine(piva, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server, IdBudget)

                    For Each MacRow As DataRow In dtMac.Rows
                        Dim Id_ProdottiCosti As Integer = objSequenze_R.NuovoId_Tabella("Prodotti_Costi",
                                                                                        1, 2000000000,
                                                                                        objParametri_Server)

                        ObjProdottiCosti_W.Scrivi_Completa(Id_ProdottiCosti,
                                                           MacRow.Item("piva"),
                                                           MacRow.Item("Riferimento"),
                                                           CInt(MacRow.Item("Elem_Cod")), CInt(MacRow.Item("pro_Cod")), CInt(MacRow.Item("Mat_Cod")),
                                                           CInt(MacRow.Item("Udm_Cod")), CInt(MacRow.Item("Mezzo")),
                                                           CDbl(MacRow.Item("Prezzo_Unitario")),
                                                           CInt(MacRow.Item("Veg_Cod")), CInt(MacRow.Item("Cul_Cod")),
                                                           CDate(MacRow.Item("Validita_Inizio")), CDate(MacRow.Item("Validita_Fine")),
                                                           enum_TipoOperazioneDB.Scrittura,
                                                           objParametri_Server,
                                                           Id_Budget:=Id_BudgetNuovo)

                    Next
                End If

                If prodotti Then
                    dtMac = ObjProdottiCosti_R.Leggi_Prodotti(piva, "", objParametri_Server, IdBudget)

                    For Each MacProd As DataRow In dtMac.Rows
                        Dim Id_ProdottiCosti As Integer = objSequenze_R.NuovoId_Tabella("Prodotti_Costi",
                                                                                        1, 2000000000,
                                                                                        objParametri_Server)

                        ObjProdottiCosti_W.Scrivi_Completa(Id_ProdottiCosti,
                                                           MacProd.Item("piva"),
                                                           MacProd.Item("Riferimento"),
                                                           CInt(MacProd.Item("Elem_Cod")), CInt(MacProd.Item("Pro_Cod")), CInt(MacProd.Item("Mat_Cod")),
                                                           CInt(MacProd.Item("Udm_Cod")), CInt(MacProd.Item("Mezzo")),
                                                           CDbl(MacProd.Item("Prezzo_Unitario")),
                                                           CInt(MacProd.Item("Veg_Cod")), CInt(MacProd.Item("Cul_Cod")),
                                                           CDate(MacProd.Item("Validita_Inizio")), CDate(MacProd.Item("Validita_Fine")),
                                                           enum_TipoOperazioneDB.Scrittura,
                                                           objParametri_Server,
                                                           Id_Budget:=Id_BudgetNuovo)

                    Next
                End If

                r.RispostaStringa = "Copia effettuata correttamente"
                r.RispostaOK = True
                r.ParametroDue = True

                ts.Complete()

            Catch ex As Exception
                r.RispostaOK = False
                r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
                r.ParametroDue = False

            Finally

                Utility.VerificaChiudiConnessione(objParametri_Server, Flag_ConnessioneLocale)

                ts.Dispose()

                If GiasContext IsNot Nothing AndAlso GiasContext.Database.Connection.State = ConnectionState.Open Then
                    GiasContext.Database.Connection.Close()
                End If

            End Try

        End Using

        Return r

    End Function

    Private Shared Function Copia_Piano_Colturale(ByVal Id_Budget As Integer,
                                                  ByRef objParametri_Server As AgronicaCoreParametri,
                                                  ByRef objSequenze_R As Agro_Sequenze,
                                                  ByVal Id_BudgetNuovo As Integer,
                                                  ByVal specie As List(Of Integer),
                                                  GiasContext As Gias_DeveloperServer_Entities) As Tuple(Of
                                                                                           List(Of Tuple(Of Tuple(Of String, Integer, Integer), Integer)),
                                                                                           List(Of Tuple(Of Tuple(Of String, Integer, Integer, Integer), Integer)),
                                                                                           List(Of Tuple(Of Integer, Integer)),
                                                                                           List(Of Tuple(Of Tuple(Of String, Integer, Integer), Integer)))

        Dim username As String = objParametri_Server.UsernameOperazione

        Dim ObjCampoR As New Budget_Campi_R
        Dim ObjCampoCodR As New Budget_Campi_codici_R
        Dim ObjCampoCodW As New Budget_Campi_codici_W
        Dim ObjCampiXPartR As New Budget_CampixParticelle_R
        Dim ObjCampiXPartW As New Budget_CampixParticelle_W
        Dim ObjAppezzamentiR As New AgronicaCoreBudgetDAL.Budget_Appezzamento_R
        Dim ObjRegImpiantiR As New Budget_Reg_Impianti_Read
        Dim ObjImpreseProgettiR As New Budget_Impresa_Progetti_R

        Dim t_campo_cod As New List(Of Tuple(Of Tuple(Of String, Integer, Integer), Integer))
        Dim t_appezza As New List(Of Tuple(Of Tuple(Of String, Integer, Integer), Integer))
        Dim t_id_reg As New List(Of Tuple(Of Tuple(Of String, Integer, Integer, Integer), Integer))
        Dim t_progetto_cod As New List(Of Tuple(Of Integer, Integer))

        Dim campiCod As New List(Of Tuple(Of String, Integer, Integer))
        Dim appezzaCod As New List(Of Tuple(Of String, Integer, Integer))
        Dim idRegCod As New List(Of Tuple(Of String, Integer, Integer, Integer))

        Dim nuovo_campo_cod, nuovo_appezza, nuovo_id_reg, nuovo_progetto_cod As Integer

        t_campo_cod.Add(Tuple.Create(Tuple.Create("", 0, 0), 0))
        t_appezza.Add(Tuple.Create(Tuple.Create("", 0, 0), 0))
        t_id_reg.Add(Tuple.Create(Tuple.Create("", 0, 0, 0), 0))
        t_progetto_cod.Add(Tuple.Create(CInt(0), 0))


        Dim tutte = specie.Count = ObjRegImpiantiR.Leggi_Specie_Da_Budget(Id_Budget, "", objParametri_Server).Rows.Count

        Dim dtCampi = ObjCampoR.Leggi(Id_Budget, "", 0, 0, enumSelezioneVariabile.Selezione_TabellaCompleta,
                                      IIf(tutte, "", " Budget_Campi.Veg_Cod " & QueryBuilderUtility.GeneraClausolaINDaList(specie)), "", objParametri_Server)

        For Each rowCampi As DataRow In dtCampi.Rows
            Dim piva_campo As String = rowCampi.Item("Piva")
            Dim sa_cod_campo As Integer = rowCampi.Item("Sa_Cod")
            Dim original_campo_cod As Integer = rowCampi.Item("Campo_Cod")

            nuovo_campo_cod = objSequenze_R.NuovoId_Campi(piva_campo, sa_cod_campo, 1, 2000000000, objParametri_Server)

            '-----------------------
            '   CAMPI
            '-----------------------
            Dim CampoOrigineEF As Budget_Campi = (From campo In GiasContext.Budget_Campi
                                                  Where campo.Id_Budget = Id_Budget AndAlso
                                                      campo.Piva = piva_campo AndAlso
                                                      campo.Sa_Cod = sa_cod_campo AndAlso
                                                      campo.Campo_Cod = original_campo_cod).FirstOrDefault()

            Dim CampoDestinazioneEF As New Budget_Campi With {
                .Id_Budget = Id_BudgetNuovo,
                .Piva = piva_campo,
                .Sa_Cod = sa_cod_campo,
                .Campo_Cod = nuovo_campo_cod
            }

            CampoDestinazioneEF = CampoOrigineEF.PropertyCopier(CampoDestinazioneEF, chiavi_da_escludere)
            CampoDestinazioneEF.RiempiCampi_StandardGias(username)

            GiasContext.Budget_Campi.Add(CampoDestinazioneEF)
            GiasContext.SaveChanges()


            campiCod.Add(Tuple.Create(CStr(piva_campo), CInt(sa_cod_campo), CInt(original_campo_cod)))
            t_campo_cod.Add(Tuple.Create(Tuple.Create(CStr(piva_campo), CInt(sa_cod_campo), CInt(original_campo_cod)), nuovo_campo_cod))


            '-----------------------
            '   CODICI
            '-----------------------
            Dim CampiCodiciOrigine_List = (From codici In GiasContext.Budget_Campi_Codici
                                           Where codici.Id_Budget = Id_Budget AndAlso
                                               codici.PIVA = piva_campo AndAlso
                                               codici.sa_cod = sa_cod_campo AndAlso
                                               codici.campo_cod = original_campo_cod).ToList()

            For Each CodiceCampoOrigineEF In CampiCodiciOrigine_List
                Dim CodiceCampoDestinazioneEF As New Budget_Campi_Codici With {
                    .Id_Budget = Id_BudgetNuovo,
                    .PIVA = piva_campo,
                    .sa_cod = sa_cod_campo,
                    .campo_cod = nuovo_campo_cod
                }

                CodiceCampoDestinazioneEF = CodiceCampoOrigineEF.PropertyCopier(CodiceCampoDestinazioneEF, chiavi_da_escludere)
                CodiceCampoDestinazioneEF.RiempiCampi_StandardGias(username)

                GiasContext.Budget_Campi_Codici.Add(CodiceCampoDestinazioneEF)
                GiasContext.SaveChanges()
            Next


            '-----------------------
            '   PARTICELLE 
            '-----------------------
            Dim CampoParticelleOrigine_List = (From p In GiasContext.Budget_CampiXParticelle
                                               Where p.Id_Budget = Id_Budget AndAlso
                                                   p.PIVA = piva_campo AndAlso
                                                   p.SA_COD = sa_cod_campo AndAlso
                                                   p.CAMPO_COD = original_campo_cod).ToList()

            For Each ParticellaCampoOrigineEF In CampoParticelleOrigine_List
                Dim ParticellaCampoDestinazioneEF As New Budget_CampiXParticelle With {
                    .Id_Budget = Id_BudgetNuovo,
                    .PIVA = piva_campo,
                    .SA_COD = sa_cod_campo,
                    .CAMPO_COD = nuovo_campo_cod
                }

                ParticellaCampoDestinazioneEF = ParticellaCampoOrigineEF.PropertyCopier(ParticellaCampoDestinazioneEF, chiavi_da_escludere)
                ParticellaCampoDestinazioneEF.RiempiCampi_StandardGias(username)

                GiasContext.Budget_CampiXParticelle.Add(ParticellaCampoDestinazioneEF)
                GiasContext.SaveChanges()
            Next


            '-------------------------------
            '   LOG 
            '-------------------------------
            Scrivi_Log(Id_BudgetNuovo,
                       CampoDestinazioneEF, enum_TipoEntita_Des.Campi, enum_TipoOperazioneDB.Scrittura,
                       piva_campo, sa_cod_campo, nuovo_campo_cod, Nothing, Nothing,
                       objParametri_Server, GiasContext,
                       "Copia Budget (Id_Budget " & Id_Budget.ToString() & ")")
        Next

        '-----------------------
        '   IMPIANTI 
        '-----------------------
        Dim dtRegImpianti = ObjRegImpiantiR.Leggi(Id_Budget, "", 0, 0, 0, enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                  "", "", objParametri_Server, IIf(tutte, "", "s.Veg_Cod " & QueryBuilderUtility.GeneraClausolaINDaList(specie)))

        For Each rowRegImpianti As DataRow In dtRegImpianti.Rows
            Dim piva_impianto As String = rowRegImpianti.Item("Piva")
            Dim sa_cod_impianto As Integer = rowRegImpianti.Item("Sa_Cod")
            Dim original_appezza_impianto As Integer = rowRegImpianti.Item("Appezza")
            Dim original_id_reg_impianto As Integer = rowRegImpianti.Item("Id_Reg")
            Dim original_campo_cod_impianto As Integer = rowRegImpianti.Item("ID_Campo")


            nuovo_campo_cod = t_campo_cod.Find(Function(x) CInt(original_campo_cod_impianto) = 0 OrElse (x.Item1.Item3 = CInt(original_campo_cod_impianto) AndAlso x.Item1.Item1 = CStr(piva_impianto) AndAlso x.Item1.Item2 = CInt(sa_cod_impianto))).Item2
            nuovo_appezza = objSequenze_R.NuovoId_Appezzamento(piva_impianto, sa_cod_impianto, 0, 2000000000, objParametri_Server)
            nuovo_id_reg = objSequenze_R.NuovoId_Reg_Impianti(piva_impianto, sa_cod_impianto, original_appezza_impianto, 0, 2000000000, objParametri_Server)

            '-----------------------
            '   IMPIANTO
            '-----------------------
            Dim ImpiantoOrigineEF = (From imp In GiasContext.Budget_Reg_Impianti
                                     Where imp.Id_Budget = Id_Budget AndAlso
                                         imp.PIVA = piva_impianto AndAlso
                                         imp.SA_COD = sa_cod_impianto AndAlso
                                         imp.APPEZZA = original_appezza_impianto).FirstOrDefault()

            Dim ImpiantoDestinazioneEF As New Budget_Reg_Impianti With {
                .Id_Budget = Id_BudgetNuovo,
                .PIVA = piva_impianto,
                .SA_COD = sa_cod_impianto,
                .APPEZZA = nuovo_appezza,
                .ID_REG = nuovo_id_reg,
                .ID_CAMPO = nuovo_campo_cod
            }


            ImpiantoDestinazioneEF = ImpiantoOrigineEF.PropertyCopier(ImpiantoDestinazioneEF, chiavi_da_escludere)
            ImpiantoDestinazioneEF.RiempiCampi_StandardGias(username)

            GiasContext.Budget_Reg_Impianti.Add(ImpiantoDestinazioneEF)
            GiasContext.SaveChanges()

            appezzaCod.Add(Tuple.Create(CStr(piva_impianto), CInt(sa_cod_impianto), CInt(original_appezza_impianto)))
            t_appezza.Add(Tuple.Create(Tuple.Create(CStr(piva_impianto), CInt(sa_cod_impianto), CInt(original_appezza_impianto)), nuovo_appezza))

            idRegCod.Add(Tuple.Create(CStr(piva_impianto), CInt(sa_cod_impianto), CInt(original_appezza_impianto), CInt(original_id_reg_impianto)))
            t_id_reg.Add(Tuple.Create(Tuple.Create(CStr(piva_impianto), CInt(sa_cod_impianto), CInt(original_appezza_impianto), CInt(original_id_reg_impianto)), nuovo_id_reg))


            '-----------------------
            '   CODICI
            '-----------------------
            Dim ImpiantiCodiciOrigine_List = (From codici In GiasContext.Budget_Reg_Impianti_Codici
                                              Where codici.Id_Budget = Id_Budget AndAlso
                                                  codici.PIVA = piva_impianto AndAlso
                                                  codici.sa_cod = sa_cod_impianto AndAlso
                                                  codici.appezza = original_appezza_impianto AndAlso
                                                  codici.Id_Reg = original_id_reg_impianto AndAlso
                                                  codici.Progetto_Cod = 0).ToList()

            For Each CodiceImpiantoOrigineEF In ImpiantiCodiciOrigine_List
                Dim CodiceImpiantoDestinazioneEF As New Budget_Reg_Impianti_Codici With {
                    .Id_Budget = Id_BudgetNuovo,
                    .PIVA = piva_impianto,
                    .sa_cod = sa_cod_impianto,
                    .appezza = nuovo_appezza,
                    .Id_Reg = nuovo_id_reg,
                    .Progetto_Cod = 0
                }

                CodiceImpiantoDestinazioneEF = CodiceImpiantoOrigineEF.PropertyCopier(CodiceImpiantoDestinazioneEF, chiavi_da_escludere)
                CodiceImpiantoDestinazioneEF.RiempiCampi_StandardGias(username)

                GiasContext.Budget_Reg_Impianti_Codici.Add(CodiceImpiantoDestinazioneEF)
                GiasContext.SaveChanges()
            Next


            '-------------------------------
            '   LOG 
            '-------------------------------
            Scrivi_Log(Id_BudgetNuovo,
                       ImpiantoDestinazioneEF, enum_TipoEntita_Des.Impianti, enum_TipoOperazioneDB.Scrittura,
                       piva_impianto, sa_cod_impianto, nuovo_appezza, nuovo_id_reg, Nothing,
                       objParametri_Server, GiasContext,
                       "Copia Budget (Id_Budget " & Id_Budget.ToString() & ")")
        Next


        If (appezzaCod.Count > 0) Then

            Dim dtAppezza = ObjAppezzamentiR.Leggi(Id_Budget, "", 0, 0, enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                   IIf(tutte, "", CreaFiltroDaPiva_Sacod_Appezza(appezzaCod, "Budget_Appezzamento", False)), "", objParametri_Server)

            For Each rowAppezzamento As DataRow In dtAppezza.Rows
                Dim piva_appezzamento As String = rowAppezzamento.Item("Piva")
                Dim sa_cod_appezzamento As Integer = rowAppezzamento.Item("Sa_Cod")
                Dim original_appezza_appezzamento As Integer = rowAppezzamento.Item("Appezza")
                Dim original_campo_cod_appezzamento As Integer = rowAppezzamento.Item("Campo_Cod")


                nuovo_campo_cod = t_campo_cod.Find(Function(x) CInt(original_campo_cod_appezzamento) = 0 OrElse (x.Item1.Item3 = CInt(original_campo_cod_appezzamento) AndAlso x.Item1.Item1 = CStr(piva_appezzamento) AndAlso x.Item1.Item2 = CInt(sa_cod_appezzamento))).Item2
                nuovo_appezza = t_appezza.Find(Function(x) x.Item1.Item3 = CInt(original_appezza_appezzamento) AndAlso x.Item1.Item1 = CStr(piva_appezzamento) AndAlso x.Item1.Item2 = CInt(sa_cod_appezzamento)).Item2

                Dim AppezzamentoOrigineEF = (From app In GiasContext.Budget_Appezzamento
                                             Where app.Id_Budget = Id_Budget AndAlso
                                                 app.PIVA = piva_appezzamento AndAlso
                                                 app.SA_COD = sa_cod_appezzamento AndAlso
                                                 app.APPEZZA = original_appezza_appezzamento).FirstOrDefault()

                Dim AppezzamentoDestinazioneEF As New Budget_Appezzamento With {
                    .Id_Budget = Id_BudgetNuovo,
                    .PIVA = piva_appezzamento,
                    .SA_COD = sa_cod_appezzamento,
                    .APPEZZA = nuovo_appezza,
                    .Campo_Cod = nuovo_campo_cod
                }

                AppezzamentoDestinazioneEF = AppezzamentoOrigineEF.PropertyCopier(AppezzamentoDestinazioneEF, chiavi_da_escludere)
                AppezzamentoDestinazioneEF.RiempiCampi_StandardGias(username)

                GiasContext.Budget_Appezzamento.Add(AppezzamentoDestinazioneEF)
                GiasContext.SaveChanges()


                '-----------------------
                '   CODICI
                '-----------------------
                Dim AppezzamentiCodiciOrigine_List = (From codici In GiasContext.Budget_Appezzamento_Codici
                                                      Where codici.Id_Budget = Id_Budget AndAlso
                                                          codici.PIVA = piva_appezzamento AndAlso
                                                          codici.sa_cod = sa_cod_appezzamento AndAlso
                                                          codici.appezza = original_appezza_appezzamento).ToList()

                For Each CodiceAppezzamentoOrigineEF In AppezzamentiCodiciOrigine_List
                    Dim CodiceAppezzamentoDestinazioneEF As New Budget_Appezzamento_Codici With {
                        .Id_Budget = Id_BudgetNuovo,
                        .PIVA = piva_appezzamento,
                        .sa_cod = sa_cod_appezzamento,
                        .appezza = nuovo_appezza
                    }

                    CodiceAppezzamentoDestinazioneEF = CodiceAppezzamentoOrigineEF.PropertyCopier(CodiceAppezzamentoDestinazioneEF, chiavi_da_escludere)
                    CodiceAppezzamentoDestinazioneEF.RiempiCampi_StandardGias(username)

                    GiasContext.Budget_Appezzamento_Codici.Add(CodiceAppezzamentoDestinazioneEF)
                    GiasContext.SaveChanges()
                Next


                '-----------------------
                '   PARTICELLE 
                '-----------------------
                Dim AppezzamentoParticelle_List = (From p In GiasContext.Budget_AppezzamentiXParticelle
                                                   Where p.Id_Budget = Id_Budget AndAlso
                                                       p.PIVA = piva_appezzamento AndAlso
                                                       p.SA_COD = sa_cod_appezzamento AndAlso
                                                       p.APPEZZA = original_appezza_appezzamento).ToList()

                For Each ParticellaAppezzamentoOrigineEF In AppezzamentoParticelle_List
                    Dim ParticellaAppezzamentoDestinazioneEF As New Budget_AppezzamentiXParticelle With {
                        .Id_Budget = Id_BudgetNuovo,
                        .PIVA = piva_appezzamento,
                        .SA_COD = sa_cod_appezzamento,
                        .APPEZZA = nuovo_appezza
                    }

                    ParticellaAppezzamentoDestinazioneEF = ParticellaAppezzamentoOrigineEF.PropertyCopier(ParticellaAppezzamentoDestinazioneEF, chiavi_da_escludere)
                    ParticellaAppezzamentoDestinazioneEF.RiempiCampi_StandardGias(username)

                    GiasContext.Budget_AppezzamentiXParticelle.Add(ParticellaAppezzamentoDestinazioneEF)
                    GiasContext.SaveChanges()
                Next


                '-----------------------
                '    APPEZZAMENTI X INDIRIZZI
                '-----------------------
                Dim AppezzamentixIndirizziOrigine_List = (From ind In GiasContext.Budget_AppezzamentixIndirizzi
                                                          Where ind.Id_Budget = Id_Budget AndAlso
                                                              ind.PIVA = piva_appezzamento AndAlso
                                                              ind.sa_cod = sa_cod_appezzamento AndAlso
                                                              ind.appezza = original_appezza_appezzamento).ToList()

                For Each AppezzamentixIndirizziOrigineEF In AppezzamentixIndirizziOrigine_List
                    Dim idGen As New Agro_Sequenze
                    Dim AppezzamentixIndirizziDestinazioneEF As New Budget_AppezzamentixIndirizzi With {
                        .Id_Budget = Id_BudgetNuovo,
                        .PIVA = piva_appezzamento,
                        .sa_cod = sa_cod_appezzamento,
                        .appezza = t_appezza.Find(Function(x) x.Item1.Item3 = CInt(original_appezza_appezzamento) AndAlso x.Item1.Item1 = CStr(piva_appezzamento) AndAlso x.Item1.Item2 = CInt(sa_cod_appezzamento)).Item2,
                        .cod_indirizzo = idGen.NuovoId_Tabella_EF(GiasContext, "Indirizzi", 0, 2000000, objParametri_Server)
                    }

                    AppezzamentixIndirizziDestinazioneEF = AppezzamentixIndirizziOrigineEF.PropertyCopier(AppezzamentixIndirizziDestinazioneEF, chiavi_da_escludere)
                    AppezzamentixIndirizziDestinazioneEF.RiempiCampi_StandardGias(username)

                    GiasContext.Budget_AppezzamentixIndirizzi.Add(AppezzamentixIndirizziDestinazioneEF)
                    GiasContext.SaveChanges()


                    '-------------------------------
                    '    INDIRIZZO
                    '-------------------------------
                    Dim IndirizzoOrigineEF = (From ind In GiasContext.Indirizzi
                                              Where ind.cod_indirizzo = AppezzamentixIndirizziOrigineEF.cod_indirizzo).FirstOrDefault()

                    Dim IndirizziDestinazioneEF As New Indirizzi With {
                        .cod_indirizzo = AppezzamentixIndirizziDestinazioneEF.cod_indirizzo
                    }

                    IndirizziDestinazioneEF = IndirizzoOrigineEF.PropertyCopier(IndirizziDestinazioneEF, chiavi_da_escludere)
                    IndirizziDestinazioneEF.RiempiCampi_StandardGias(username)

                    GiasContext.Indirizzi.Add(IndirizziDestinazioneEF)
                    GiasContext.SaveChanges()
                Next


                '-------------------------------
                '   LOG 
                '-------------------------------
                Scrivi_Log(Id_BudgetNuovo,
                           AppezzamentoDestinazioneEF, enum_TipoEntita_Des.Appezza, enum_TipoOperazioneDB.Scrittura,
                           piva_appezzamento, sa_cod_appezzamento, nuovo_appezza, Nothing, Nothing,
                           objParametri_Server, GiasContext,
                           "Copia Budget (Id_Budget " & Id_Budget.ToString() & ")")

            Next


            '-----------------------
            '   ESERCIZI 
            '-----------------------
            Dim dtImpreseProgetti = ObjImpreseProgettiR.Leggi(Id_Budget, "", 0, "", 0, 0, 0, 0, 0, 0,
                                                              enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                              CreaFiltroDaId_Reg(idRegCod, "Budget_Imprese_Progetti"),
                                                              "", objParametri_Server)

            For Each rowImpreseProgetti As DataRow In dtImpreseProgetti.Rows
                Dim piva_esercizio As String = rowImpreseProgetti.Item("Piva")
                Dim sa_cod_esercizio As Integer = rowImpreseProgetti.Item("Sa_Cod")
                Dim original_appezza_esercizio As Integer = rowImpreseProgetti.Item("Appezza")
                Dim original_id_reg_esercizio As Integer = rowImpreseProgetti.Item("Id_Reg")
                Dim original_progetto_cod_esercizio As Integer = rowImpreseProgetti.Item("Progetto_Cod")


                nuovo_appezza = t_appezza.Find(Function(x) x.Item1.Item3 = CInt(original_appezza_esercizio) AndAlso x.Item1.Item1 = CStr(piva_esercizio) AndAlso x.Item1.Item2 = CInt(sa_cod_esercizio)).Item2
                nuovo_id_reg = t_id_reg.Find(Function(x) x.Item1.Item3 = CInt(original_appezza_esercizio) AndAlso x.Item1.Item1 = CStr(piva_esercizio) AndAlso x.Item1.Item2 = CInt(sa_cod_esercizio) AndAlso x.Item1.Item4 = CInt(original_id_reg_esercizio)).Item2
                nuovo_progetto_cod = objSequenze_R.NuovoId_Tabella("Imprese_Progetti", 1, 2000000000, objParametri_Server)

                Dim EsercizioOrigineEF = (From ese In GiasContext.Budget_Imprese_Progetti
                                          Where ese.Id_Budget = Id_Budget AndAlso
                                              ese.Piva = piva_esercizio AndAlso
                                              ese.Sa_Cod = sa_cod_esercizio AndAlso
                                              ese.Appezza = original_appezza_esercizio AndAlso
                                              ese.Id_Reg = original_id_reg_esercizio AndAlso
                                              ese.Progetto_Cod = original_progetto_cod_esercizio).FirstOrDefault()

                Dim EsercizioDestinazioneEF As New Budget_Imprese_Progetti With {
                    .Id_Budget = Id_BudgetNuovo,
                    .Piva = piva_esercizio,
                    .Sa_Cod = sa_cod_esercizio,
                    .Appezza = nuovo_appezza,
                    .Id_Reg = nuovo_id_reg,
                    .Progetto_Cod = nuovo_progetto_cod
                }

                EsercizioDestinazioneEF = EsercizioOrigineEF.PropertyCopier(EsercizioDestinazioneEF, chiavi_da_escludere)
                EsercizioDestinazioneEF.RiempiCampi_StandardGias(username)

                GiasContext.Budget_Imprese_Progetti.Add(EsercizioDestinazioneEF)
                GiasContext.SaveChanges()

                t_progetto_cod.Add(Tuple.Create(CInt(original_progetto_cod_esercizio), nuovo_progetto_cod))


                '-----------------------
                '   CODICI
                '-----------------------
                Dim EserciziCodiciOrigine_List = (From codici In GiasContext.Budget_Reg_Impianti_Codici
                                                  Where codici.Id_Budget = Id_Budget AndAlso
                                                      codici.PIVA = piva_esercizio AndAlso
                                                      codici.sa_cod = sa_cod_esercizio AndAlso
                                                      codici.appezza = original_appezza_esercizio AndAlso
                                                      codici.Id_Reg = original_id_reg_esercizio AndAlso
                                                      codici.Progetto_Cod = original_progetto_cod_esercizio).ToList()

                For Each CodiceEsercizioOrigineEF In EserciziCodiciOrigine_List
                    Dim CodiceEsercizioDestinazioneEF As New Budget_Reg_Impianti_Codici With {
                        .Id_Budget = Id_BudgetNuovo,
                        .PIVA = piva_esercizio,
                        .sa_cod = sa_cod_esercizio,
                        .appezza = nuovo_appezza,
                        .Id_Reg = nuovo_id_reg,
                        .Progetto_Cod = nuovo_progetto_cod
                    }

                    CodiceEsercizioDestinazioneEF = CodiceEsercizioOrigineEF.PropertyCopier(CodiceEsercizioDestinazioneEF, chiavi_da_escludere)
                    CodiceEsercizioDestinazioneEF.RiempiCampi_StandardGias(username)

                    GiasContext.Budget_Reg_Impianti_Codici.Add(CodiceEsercizioDestinazioneEF)
                    GiasContext.SaveChanges()
                Next

                '-------------------------------
                '   LOG 
                '-------------------------------
                Scrivi_Log(Id_BudgetNuovo,
                           EsercizioDestinazioneEF, enum_TipoEntita_Des.Progetti, enum_TipoOperazioneDB.Scrittura,
                           piva_esercizio, sa_cod_esercizio, nuovo_appezza, nuovo_id_reg, nuovo_progetto_cod,
                           objParametri_Server, GiasContext,
                           "Copia Budget (Id_Budget " & Id_Budget.ToString() & ")")

            Next
        End If

        Dim res = Tuple.Create(t_appezza, t_id_reg, t_progetto_cod, t_campo_cod)

        Return res

    End Function

    Private Shared Function CreaFiltroDaPiva_Sacod_Appezza(ByVal listona As List(Of Tuple(Of String, Integer, Integer)), ByVal nomeTabella As String, Optional ByVal campo As Boolean = True) As String

        Dim filtro As String = ""
        Dim primoElemento As Boolean = True

        If listona.Count > 1 Then
            filtro += " ( "
        End If

        For Each x In listona
            If Not primoElemento Then
                filtro += " Or "
            Else
                primoElemento = False
            End If

            filtro += " (" & nomeTabella & ".Piva = '" & x.Item1 & "' AND " & nomeTabella & ".Sa_Cod = " & x.Item2 & " AND " & nomeTabella & "." & IIf(campo, "Campo_Cod", "Appezza") & " = " & x.Item3 & ") "
        Next

        If listona.Count > 1 Then
            filtro += " ) "
        End If

        Return filtro

    End Function

    Private Shared Function CreaFiltroDaId_Reg(ByVal listona As List(Of Tuple(Of String, Integer, Integer, Integer)), ByVal nomeTabella As String) As String

        Dim filtro As String = ""
        Dim primoElemento As Boolean = True

        If listona.Count > 1 Then
            filtro += " ( "
        End If

        For Each x In listona
            If Not primoElemento Then
                filtro += " OR "
            Else
                primoElemento = False
            End If

            filtro += " (" & nomeTabella & ".Piva = '" & x.Item1 & "' AND " & nomeTabella & ".Sa_Cod = " & x.Item2 & " AND " & nomeTabella & "." & "Appezza = " & x.Item3 & " AND " & nomeTabella & "." & "Id_Reg = " & x.Item4 & ")"
        Next

        If listona.Count > 1 Then
            filtro += " ) "
        End If

        Return filtro

    End Function

    Private Shared Sub Scrivi_Log(Id_Budget As Integer,
                                  objDestinazioneEF As Object,
                                  TipoEntita As String,
                                  enum_TipoOperazioneDB As enum_TipoOperazioneDB,
                                  param1 As String, param2 As String, param3 As String, param4 As String, param5 As String,
                                  objParametri_Server As AgronicaCoreParametri,
                                  GiasContext As Gias_DeveloperServer_Entities,
                                  NoteLog As String)

        Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        'Scrittura tabella Agronica_Log_Anagrafe
        Dim DatiLogStr = JsonConvert.SerializeObject(objDestinazioneEF, a)
        Dim Log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(TipoEntita,
                                                                             param1, param2, param3, param4, param5, Nothing,
                                                                             enum_TipoOperazioneDB, objParametri_Server,
                                                                             enum_Id_Servizio.GiasOnline,
                                                                             NoteLog, DatiLogStr,
                                                                             Id_Budget)

        GiasContext.Agronica_Log_Anagrafe.Add(Log)
        GiasContext.SaveChanges()
    End Sub
#End Region

#Region "SCRITTURA TESTATA"
    Public Shared Sub Scrivi_Modifica_Testata_Budget(objCurrentTestata As JObject,
                                                     list_TestateBudget As JArray,
                                                     piva As String,
                                                     Tipo_Operazione As enum_TipoOperazioneDB,
                                                     objParametri_Server As AgronicaCoreParametri)

        Dim a = New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim Validita_Inizio As Date = JsonConvert.DeserializeObject(Of Date)(JsonConvert.SerializeObject(objCurrentTestata.Item("Validita_Inizio")), a)
        Dim Validita_Fine As Date = JsonConvert.DeserializeObject(Of Date)(JsonConvert.SerializeObject(objCurrentTestata.Item("Validita_Fine")), a)

        Dim objBudget As New AgronicaCoreBudgetDAL.Budget_Testata_W

        Dim Id_Budget_CheckInUso As Integer = 0

        Select Case Tipo_Operazione
            Case enum_TipoOperazioneDB.Scrittura
                Dim objSequenze_R As New Agro_Sequenze
                Dim Id_BudgetNuovo As Integer = objSequenze_R.NuovoId_Tabella("Budget_Testata", 1, 2000000000, objParametri_Server)

                objBudget.Scrivi(piva, Id_BudgetNuovo, CStr(objCurrentTestata.Item("Nome_Budget")), CInt(objCurrentTestata.Item("Revisione")),
                                 CInt(objCurrentTestata.Item("In_Uso")), CInt(objCurrentTestata.Item("Tipo_Budget")),
                                 CInt(objCurrentTestata.Item("Sa_Cod")), CInt(objCurrentTestata.Item("Listino_Cod_Costi")),
                                 CInt(objCurrentTestata.Item("Listino_Cod_Ricavi")), Validita_Inizio, Validita_Fine, objParametri_Server)

                Id_Budget_CheckInUso = Id_BudgetNuovo

            Case enum_TipoOperazioneDB.Modifica
                objBudget.Modifica(CInt(objCurrentTestata.Item("Id_Budget")), objParametri_Server, Nome_Budget:=CStr(objCurrentTestata.Item("Nome_Budget")),
                                   Sa_Cod:=CInt(objCurrentTestata.Item("Sa_Cod")), Revisione:=CInt(objCurrentTestata.Item("Revisione")),
                                   In_Uso:=CInt(objCurrentTestata.Item("In_Uso")), Tipo_Budget:=CInt(objCurrentTestata.Item("Tipo_Budget")),
                                   Listino_Cod_Costi:=CInt(objCurrentTestata.Item("Listino_Cod_Costi")), Listino_Cod_Ricavi:=CInt(objCurrentTestata.Item("Listino_Cod_Ricavi")),
                                   Validita_Inizio:=Validita_Inizio, Validita_Fine:=Validita_Fine)

                Id_Budget_CheckInUso = CInt(objCurrentTestata.Item("Id_Budget"))
        End Select

        'AGGIORNO LE ALTRE TESTATE SE LA CORRENTE è IN USO
        If CInt(objCurrentTestata.Item("In_Uso")) = 1 Then
            For Each objOtherTestata As JObject In list_TestateBudget

                If (CInt(objOtherTestata.Item("Id_Budget")) <> 0 AndAlso CInt(objOtherTestata.Item("Id_Budget")) <> Id_Budget_CheckInUso AndAlso
                    CInt(objOtherTestata.Item("In_Uso")) = 1 AndAlso IIf(CInt(objCurrentTestata.Item("Sa_Cod")) = -1, True, objOtherTestata.Item("Piva").ToString = piva)) Then

                    objBudget.Modifica(CInt(objOtherTestata.Item("Id_Budget")), objParametri_Server, In_Uso:=0)

                End If
            Next
        End If

    End Sub

    Public Shared Sub Cancella_Testata_Budget(objCurrentTestata As JObject, piva As String,
                                              objParametri_Server As AgronicaCoreParametri,
                                              objParametri_Utenti As AgronicaCoreParametri,
                                                Optional listErroriGias As List(Of ErroreGias) = Nothing)

        Dim objBudget As New AgronicaCoreBudgetDAL.Budget_Testata_W

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
        Dim GiasContext As Gias_DeveloperServer_Entities = Nothing

        Dim Id_Budget As Integer = CInt(objCurrentTestata.Item("Id_Budget"))
        Dim Delete_Reale_Da_Ribaltamento As Integer = CInt(objCurrentTestata.Item("Delete_Reale_Da_Ribaltamento"))

        Dim listRibaltamento_Appezzamento As New List(Of AgronicaCoreEntityFramework_POCO.Ribaltamento_Appezzamento)
        Dim listRibaltamento_Campi As New List(Of AgronicaCoreEntityFramework_POCO.Ribaltamento_Campi)

        'Istanzio la transazione forzando l'uso di una nuova transazione
        Using ts As New TransactionScope(TransactionScopeOption.RequiresNew, New TimeSpan(0, 10, 0))
            Dim OpenNewTransaction As Boolean = False
            Dim Flag_ConnessioneLocale As Boolean = True

            Dim ObjQualificheXTariffe_R As New AgronicaCoreContabDAL.QualificheXTariffe_R
            Dim ObjQualificheXTariffe_W As New AgronicaCoreContabDAL.QualificheXTariffe_W
            Dim ObjProdottiCosti_W As New AgronicaCoreContabDAL.Prodotti_Costi_W
            Dim ObjCDG_DAL_R As New AgronicaCoreContabDAL.CDG_DAL_R
            Dim ObjCDG_DAL_W As New AgronicaCoreContabDAL.CDG_DAL_W
            Dim ObjDW_W As New AgronicaCoreContabDAL.DW_CDG_Costi_Ricavi_DAL_W
            Dim ObjMovDetRif_W As New AgronicaCoreContabDAL.Mov_Det_Riferimenti_W
            Dim ObjMovDet_W As New AgronicaCoreContabDAL.Movimenti_Dettagli_W
            Dim ObjMovDest_W As New AgronicaCoreContabDAL.Mov_Destinazioni_W
            Dim ObjMov_R As New AgronicaCoreContabDAL.Movimenti_R
            Dim ObjMov_W As New AgronicaCoreContabDAL.Movimenti_W
            Dim ObjAgenda As New AgronicaCoreContabDAL.Agenda_W

            Try
                GiasContext = New Gias_DeveloperServer_Entities(EFConnString)
                'Open the contextObject connection state explicitly
                GiasContext.Database.Connection.Open()

                Utility.VerificaApriConnessione(objParametri_Server, Flag_ConnessioneLocale)

                '------------------------
                '   TESTATA
                '------------------------
                objBudget.Cancella(Id_Budget, "", objParametri_Server)


                '------------------------
                '   QUALIFICHE X TARIFFE
                '------------------------
                For Each QxTRow As DataRow In ObjQualificheXTariffe_R.Leggi(piva, 0, 0, 0, "", "", objParametri_Server, IdBudget:=Id_Budget).Rows
                    ObjQualificheXTariffe_W.Cancella(CInt(QxTRow("ID")), "", objParametri_Server)
                Next

                '------------------------
                '   PRODOTTI COSTI
                '------------------------
                ObjProdottiCosti_W.CancellaDaBudget(Id_Budget, "", objParametri_Server)

                '------------------------
                '   DW CDG
                '------------------------
                ObjDW_W.DW_Delete_DaBudget(Id_Budget, objParametri_Server)

                '------------------------
                '   CDG
                '------------------------
                For Each CDGRow As DataRow In ObjCDG_DAL_R.Leggi_CDG_Testata_Da_Budget(Id_Budget, "", objParametri_Server).Rows

                    For Each MovRow As DataRow In ObjMov_R.Leggi("", 0, CDGRow.Item("Id_Agenda"), 0, 0, "",
                                                                 enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server).Rows

                        ObjMovDetRif_W.Cancella("", MovRow.Item("Sa_Cod"), MovRow.Item("Id_Agenda"), MovRow.Item("Id_Mov"), 0, " lav_cod_rif = 4500 AND Id_Agenda_Rif = " + CDGRow.Item("Id_Agenda").ToString + " ", objParametri_Server)
                        ObjMovDet_W.Cancella("", MovRow.Item("Sa_Cod"), MovRow.Item("Id_Agenda"), MovRow.Item("Id_Mov"), 0, "", objParametri_Server)
                        ObjMovDest_W.Cancella("", MovRow.Item("Sa_Cod"), MovRow.Item("Id_Agenda"), MovRow.Item("Id_Mov"), 0, 0, 0, "", objParametri_Server)
                        ObjMov_W.Cancella("", MovRow.Item("Sa_Cod"), MovRow.Item("Id_Agenda"), MovRow.Item("Id_Mov"), "", objParametri_Server)
                    Next

                    ObjAgenda.Cancella(CDGRow.Item("Piva"), 0, CDGRow.Item("Id_Agenda"), "", objParametri_Server)
                    ObjCDG_DAL_W.CancellaCDG_Dettagli("", CDGRow.Item("Id_CDG"), objParametri_Server)
                    ObjCDG_DAL_W.CancellaCDG_Testata("", CDGRow.Item("Id_CDG"), objParametri_Server)

                Next

                '------------------------
                '   PIANO COLTURALE
                '------------------------
                Elimina_PianoColturale_da_Testata_Budget(Id_Budget, objParametri_Server, listRibaltamento_Appezzamento, listRibaltamento_Campi, GiasContext)


                ts.Complete()

            Catch ex As GiasException
                Throw New GiasException(ex.Message)
            Catch ex As Exception
                Throw New Exception(ex.Message)
            Finally

                Utility.VerificaChiudiConnessione(objParametri_Server, Flag_ConnessioneLocale)

                ts.Dispose()

                If GiasContext IsNot Nothing AndAlso GiasContext.Database.Connection.State = ConnectionState.Open Then
                    GiasContext.Database.Connection.Close()
                End If
            End Try
        End Using

        If Delete_Reale_Da_Ribaltamento = 1 AndAlso listErroriGias.Count = 0 Then
            Dim listErrori As New List(Of String)

            Dim deleteCampo As Boolean = True

            For Each Ribaltamento_Appezzamento In listRibaltamento_Appezzamento

                'Istanzio la transazione forzando l'uso di una nuova transazione
                Using ts As New TransactionScope(TransactionScopeOption.RequiresNew, New TimeSpan(0, 10, 0))
                    Dim _app_nome As String = ""

                    Try
                        GiasContext = New Gias_DeveloperServer_Entities(EFConnString)
                        'Open the contextObject connection state explicitly
                        GiasContext.Database.Connection.Open()

                        _app_nome = (From a In GiasContext.Appezzamento
                                     Where a.PIVA = Ribaltamento_Appezzamento.Reale_Piva AndAlso
                                         a.SA_COD = Ribaltamento_Appezzamento.Reale_Sa_Cod AndAlso
                                         a.APPEZZA = Ribaltamento_Appezzamento.Reale_Appezza
                                     Select a.APP_NOME).FirstOrDefault()

                        Dim msgCancellazioneReale = AgronicaCoreAnagrafeBIZ.Appezzamento_W.Internal_Delete_Appezzamento_Reale_Da_Ribaltamento(Ribaltamento_Appezzamento, objParametri_Server, objParametri_Utenti,
                                                                                                                                              GiasContext, False,
                                                                                                                                              NoteLog:="Cancellato da Eliminazione Testata Budget, Id_Budget: " & Id_Budget)
                        If msgCancellazioneReale <> "" Then
                            deleteCampo = False
                            listErrori.Add(_app_nome & ": " & msgCancellazioneReale)
                        End If

                        ts.Complete()

                    Catch ex As Exception
                        ts.Dispose()

                        deleteCampo = False
                        listErrori.Add(_app_nome & ": " & ex.Message)

                    Finally
                        ts.Dispose()

                        If GiasContext IsNot Nothing AndAlso GiasContext.Database.Connection.State = ConnectionState.Open Then
                            GiasContext.Database.Connection.Close()
                        End If
                    End Try
                End Using
            Next

            If deleteCampo Then
                For Each Ribaltamento_Campo In listRibaltamento_Campi

                    Try

                        Dim msgCancellazioneReale = Budget_Campo_W.Delete_Campo_Reale_Da_Ribaltamento(Ribaltamento_Campo, objParametri_Server, objParametri_Utenti,
                                                                                                      NoteLog:="Cancellato da Eliminazione Testata Budget, Id_Budget: " & Id_Budget)

                        If msgCancellazioneReale <> "" Then
                            listErrori.Add(msgCancellazioneReale)
                        End If

                    Catch ex As Exception
                        listErrori.Add(ex.Message)
                    End Try
                Next
            End If

            If listErrori.Count > 0 Then
                listErroriGias.Add(New ErroreGias With {
                                   .messaggio = String.Join(NEWLINE, listErrori),
                                   .severity = ErroreGias_Severity.Info})
            End If
        End If
    End Sub

    Private Shared Sub Elimina_PianoColturale_da_Testata_Budget(ByVal Id_Budget As Integer,
                                                                ByRef objParametri_Server As AgronicaCoreParametri,
                                                                ByRef listRibaltamento_Appezzamento As List(Of AgronicaCoreEntityFramework_POCO.Ribaltamento_Appezzamento),
                                                                ByRef listRibaltamento_Campi As List(Of AgronicaCoreEntityFramework_POCO.Ribaltamento_Campi),
                                                                GiasContext As Gias_DeveloperServer_Entities)
        Dim list_msg_gestiti As New List(Of String)
        Dim appezzamenti_to_delete = (From x In GiasContext.Budget_Appezzamento
                                      Where x.Id_Budget = Id_Budget).ToList()

        For Each appezzamento_to_delete In appezzamenti_to_delete
            Dim msg = Esistono_Movimenti_Impianto_Budget(Id_Budget, appezzamento_to_delete, objParametri_Server, GiasContext)
            If msg <> "" Then
                list_msg_gestiti.Add(msg)
            End If
        Next

        If list_msg_gestiti.Count > 0 Then
            Throw New GiasException(Gias.ImpossibileEliminareTestataBudgetEsistonoRichiesteMaterialeVivaisticoAssociate & ": " & NEWLINE & String.Join(NEWLINE, list_msg_gestiti))
        Else

#Region "APPEZZAMENTI"
            For Each appezzamento_to_delete In appezzamenti_to_delete
                '-------------------------------
                '   APPEZZAMENTO
                '-------------------------------
                GiasContext.Budget_Appezzamento.Remove(appezzamento_to_delete)
                GiasContext.SaveChanges()

                '-------------------------------
                '   LOG APPEZZAMENTO
                '-------------------------------
                Scrivi_Log(Id_Budget,
                           appezzamento_to_delete, enum_TipoEntita_Des.Appezza, enum_TipoOperazioneDB.Cancellazione,
                           appezzamento_to_delete.PIVA, appezzamento_to_delete.SA_COD, appezzamento_to_delete.APPEZZA, Nothing, Nothing,
                           objParametri_Server, GiasContext,
                           "Cancellazione Testata Budget (Id_Budget " & Id_Budget.ToString() & ")")

                RibaltamentoAnagraficheColturali_W.Clean_Tabelle_Ribaltamento_daDeleteElemento(Id_Budget, appezzamento_to_delete.PIVA, appezzamento_to_delete.SA_COD, appezzamento_to_delete.APPEZZA, GiasContext,
                                                                                               listRibaltamento_Appezzamento:=listRibaltamento_Appezzamento)
            Next

            '-------------------------------
            '   APPEZZAMENTI CODICI
            '-------------------------------
            Dim AppezzamentiCodici_List = (From del In GiasContext.Budget_Appezzamento_Codici
                                           Where del.Id_Budget = Id_Budget).ToList()
            GiasContext.Budget_Appezzamento_Codici.RemoveRange(AppezzamentiCodici_List)
            '-------------------------------
            '   APPEZZAMENTI X PARTICELLE
            '-------------------------------
            Dim AppezzamentoParticelle_List = (From del In GiasContext.Budget_AppezzamentiXParticelle
                                               Where del.Id_Budget = Id_Budget).ToList()
            GiasContext.Budget_AppezzamentiXParticelle.RemoveRange(AppezzamentoParticelle_List)
            GiasContext.SaveChanges()


            '-------------------------------
            '   APPEZZAMENTO X INDIRIZZI
            '-------------------------------
            Dim appxind_to_delete = (From del In GiasContext.Budget_AppezzamentixIndirizzi
                                     Where del.Id_Budget = Id_Budget).ToList()

            For Each indirizzo In appxind_to_delete
                '-------------------------------
                '    INDIRIZZO
                '-------------------------------
                Dim indirizzi_to_delete = (From ind In GiasContext.Indirizzi
                                           Where ind.cod_indirizzo = indirizzo.cod_indirizzo).FirstOrDefault()

                GiasContext.Indirizzi.Remove(indirizzi_to_delete)
                GiasContext.SaveChanges()
            Next
            GiasContext.Budget_AppezzamentixIndirizzi.RemoveRange(appxind_to_delete)
            GiasContext.SaveChanges()
#End Region

#Region "CAMPI"
            '-------------------------------
            '   CAMPI
            '-------------------------------
            Dim campi_to_delete = (From x In GiasContext.Budget_Campi
                                   Where x.Id_Budget = Id_Budget).ToList()
            For Each campo_to_delete In campi_to_delete
                '-------------------------------
                '   CAMPO
                '-------------------------------
                GiasContext.Budget_Campi.Remove(campo_to_delete)
                GiasContext.SaveChanges()

                '-------------------------------
                '   LOG CAMPO
                '-------------------------------
                Scrivi_Log(Id_Budget,
                           campo_to_delete, enum_TipoEntita_Des.Campi, enum_TipoOperazioneDB.Cancellazione,
                           campo_to_delete.Piva, campo_to_delete.Sa_Cod, campo_to_delete.Campo_Cod, Nothing, Nothing,
                           objParametri_Server, GiasContext,
                           "Cancellazione Testata Budget (Id_Budget " & Id_Budget.ToString() & ")")

                RibaltamentoAnagraficheColturali_W.Clean_Tabelle_Ribaltamento_daDeleteElemento(Id_Budget, campo_to_delete.Piva, campo_to_delete.Sa_Cod, 0, GiasContext, campo_to_delete.Campo_Cod,
                                                                                               listRibaltamento_Campi:=listRibaltamento_Campi)
            Next

            '-------------------------------
            '   CAMPI CODICI
            '-------------------------------
            Dim CampiCodici_List = (From del In GiasContext.Budget_Campi_Codici
                                    Where del.Id_Budget = Id_Budget).ToList()
            GiasContext.Budget_Campi_Codici.RemoveRange(CampiCodici_List)
            '-------------------------------
            '   CAMPI X PARTICELLE
            '-------------------------------
            Dim CampiParticelle_List = (From del In GiasContext.Budget_CampiXParticelle
                                        Where del.Id_Budget = Id_Budget).ToList()
            GiasContext.Budget_CampiXParticelle.RemoveRange(CampiParticelle_List)
            GiasContext.SaveChanges()
#End Region

#Region "IMPIANTI"
            '-------------------------------
            '   IMPIANTI
            '-------------------------------
            Dim impianti_to_delete = (From x In GiasContext.Budget_Reg_Impianti
                                      Where x.Id_Budget = Id_Budget).ToList()
            For Each impianto_to_delete In impianti_to_delete
                '-------------------------------
                '   IMPIANTO
                '-------------------------------
                GiasContext.Budget_Reg_Impianti.Remove(impianto_to_delete)
                GiasContext.SaveChanges()

                '-------------------------------
                '   LOG IMPIANTO
                '-------------------------------
                Scrivi_Log(Id_Budget,
                           impianto_to_delete, enum_TipoEntita_Des.Impianti, enum_TipoOperazioneDB.Cancellazione,
                           impianto_to_delete.PIVA, impianto_to_delete.SA_COD, impianto_to_delete.APPEZZA, impianto_to_delete.ID_REG, Nothing,
                           objParametri_Server, GiasContext,
                           "Cancellazione Testata Budget (Id_Budget " & Id_Budget.ToString() & ")")
            Next
#End Region

#Region "ESERCIZI"
            '-------------------------------
            '   ESERCIZI
            '-------------------------------
            Dim esercizi_to_delete = (From x In GiasContext.Budget_Imprese_Progetti
                                      Where x.Id_Budget = Id_Budget).ToList()
            For Each esercizio_to_delete In esercizi_to_delete
                '-------------------------------
                '   ESERCIZIO
                '-------------------------------
                GiasContext.Budget_Imprese_Progetti.Remove(esercizio_to_delete)
                GiasContext.SaveChanges()

                '-------------------------------
                '   LOG ESERCIZIO
                '-------------------------------
                Scrivi_Log(Id_Budget,
                           esercizio_to_delete, enum_TipoEntita_Des.Progetti, enum_TipoOperazioneDB.Cancellazione,
                           esercizio_to_delete.Piva, esercizio_to_delete.Sa_Cod, esercizio_to_delete.Appezza, esercizio_to_delete.Id_Reg, esercizio_to_delete.Progetto_Cod,
                           objParametri_Server, GiasContext,
                           "Cancellazione Testata Budget (Id_Budget " & Id_Budget.ToString() & ")")
            Next


            '-------------------------------
            '   REG_IMPIANTI_CODICI
            '-------------------------------
            Dim Reg_Impianti_Codici_List = (From del In GiasContext.Budget_Reg_Impianti_Codici
                                            Where del.Id_Budget = Id_Budget).ToList()
            GiasContext.Budget_Reg_Impianti_Codici.RemoveRange(Reg_Impianti_Codici_List)


        End If
#End Region

    End Sub

    Private Shared Function Esistono_Movimenti_Impianto_Budget(Id_Budget As Integer,
                                                               Appezzamento As Budget_Appezzamento,
                                                               objParametri_Server As AgronicaCoreParametri,
                                                               GiasContext As Gias_DeveloperServer_Entities) As String

        Dim objControlloPrenotazioni As New AgronicaCoreBudgetBIZ.Budget_Reg_Impianto_R

        If objControlloPrenotazioni.controllo_MovimentiProgrammazionexEliminazione(Id_Budget, Appezzamento.PIVA, Appezzamento.SA_COD, Appezzamento.APPEZZA, 0, objParametri_Server) Then
            Dim _rag_soc As String = (From i In GiasContext.Imprese
                                      Where i.PIVA = Appezzamento.PIVA
                                      Select i.rag_soc).FirstOrDefault()

            Return "- " & _rag_soc & ", " & Appezzamento.APP_NOME
        End If

        Return ""

    End Function
#End Region
End Class
