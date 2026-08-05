Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class Conti
    Inherits System.Web.Services.WebService

    <WebMethod()>
    Public Function WS_LeggiSaldiEC(ByVal Data_Inizio_Saldo As Date,
                                ByVal Data_Saldo As Date,
                                ByVal Flag_SOLOSaldiIniziali As Boolean,
                                ByVal GestCont_Flag_ConsideraSaldiIniziali As Boolean,
                                ByVal GestCont_DataInizio As Date,
                                ByVal Piva As String,
                                ByVal Anno As Integer,
                                ByVal Ric_Cod As Integer,
                                ByVal Id_Riclassificazione As String,
                                ByVal Dare_Avere As String,
                                ByVal Imputabile As Integer,
                                ByVal Cod_Conto As Integer,
                                ByVal Flag_UE As Integer,
                                ByVal Cod_Contatto As String,
                                ByVal Sezionale_Cod As Integer,
                                ByVal Sezionale_ChkDefault As Integer,
                                ByVal Cod_RisUm As Integer,
                                ByVal Flag_ContiSaldo0 As Boolean,
                                ByVal xFiltroAggiuntivo1 As String,
                                ByVal xFiltroAggiuntivo2 As String,
                                ByVal xFiltroAggiuntivo3 As String,
                                ByVal xFiltroAggiuntivo4 As String,
                                ByVal Flag_LayoutCostiRicavi As Boolean,
                                ByVal objP_server As String
                                ) As String

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

        Try

            Dim strRisposta As String = ""
            'Dim objCoreDAL As New AgronicaCoreContabDAL.Cespiti_R
            Dim objCoreStampeDAL As New AgronicaCoreStampeDAL.PianoConti
            Dim dt As DataTable
            Dim DT_Codifiche As DataTable


            Dim objRicXConti As New AgronicaCoreContabDAL.RicxConti_R
            'leggi DT con codifiche conti personalizzati-gias (x chi ha i conti gias, i codici coincidono)
            DT_Codifiche = objRicXConti.Leggi_Codifica_ContiEconomici(Piva, 2, Data_Saldo.Year, GestCont_DataInizio.Year, "", objParametri_Server)


            dt = objCoreStampeDAL.Saldo_Conti_Economici(Data_Inizio_Saldo,
                                 Data_Saldo,
                                 Flag_SOLOSaldiIniziali,
                                 GestCont_Flag_ConsideraSaldiIniziali,
                                 GestCont_DataInizio,
                                 Piva,
                                 Anno,
                                 Ric_Cod,
                                 Id_Riclassificazione,
                                 Dare_Avere,
                                 Imputabile,
                                 Cod_Conto,
                                 Flag_UE,
                                 Cod_Contatto,
                                 Sezionale_Cod,
                                 Sezionale_ChkDefault,
                                 Cod_RisUm,
                                 Flag_ContiSaldo0,
                                False,
                                 xFiltroAggiuntivo1,
                                 xFiltroAggiuntivo2,
                                 xFiltroAggiuntivo3,
                                 xFiltroAggiuntivo4,
                                 DT_Codifiche,
                                 Flag_LayoutCostiRicavi,
                                 objParametri_Server,
                                 strRisposta)


            r.RispostaOK = True
            r.RispostaStringa = strRisposta

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r.RispostaStringa
    End Function

    <WebMethod()>
    Public Function WS_LeggiSaldiPAT(ByVal Data_Inizio_Saldo As Date,
                                    ByVal Data_Saldo As Date,
                                    ByVal Flag_SOLOSaldiIniziali As Boolean,
                                    ByVal GestCont_Flag_ConsideraSaldiIniziali As Boolean,
                                    ByVal GestCont_DataInizio As Date,
                                    ByVal Piva As String,
                                    ByVal Anno As Integer,
                                    ByVal Ric_Cod As Integer,
                                    ByVal Id_Riclassificazione As String,
                                    ByVal Dare_Avere As String,
                                    ByVal Imputabile As Integer,
                                    ByVal Cod_Conto As Integer,
                                    ByVal Flag_UE As Integer,
                                    ByVal Cod_Contatto As String,
                                    ByVal Sezionale_Cod As Integer,
                                    ByVal Sezionale_ChkDefault As Integer,
                                    ByVal Cod_RisUm As Integer,
                                    ByVal Lista_CodRisUm As String,
                                    ByVal Cod_Liquidita As Integer,
                                    ByVal Flag_ContiSaldo0 As Boolean,
                                    ByVal Flag_AggiungiContoPadre_CreditiDebitiBanche As Boolean,
                                    ByVal xFiltroAggiuntivo1 As String,
                                    ByVal xFiltroAggiuntivo2 As String,
                                    ByVal xFiltroAggiuntivo3 As String,
                                    ByVal xFiltroAggiuntivo4 As String,
                                    ByVal objP_server As String
                                    ) As String

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

        Try

            Dim strRisposta As String = ""
            'Dim objCoreDAL As New AgronicaCoreContabDAL.Cespiti_R
            Dim objCoreStampeDAL As New AgronicaCoreStampeDAL.PianoConti
            Dim dt As DataTable
            Dim DT_Codifiche As DataTable

            Dim objRicXConti As New AgronicaCoreContabDAL.RicxConti_Patrimonio_R
            'leggi DT con codifiche conti personalizzati-gias (x chi ha i conti gias, i codici coincidono)
            DT_Codifiche = objRicXConti.Leggi_Codifica_ContiPatrimoniali(Piva, 2, Data_Saldo.Year, GestCont_DataInizio.Year, "", objParametri_Server)


            dt = objCoreStampeDAL.Saldo_Conti_Patrimoniali(Data_Inizio_Saldo,
                                     Data_Saldo,
                                     Flag_SOLOSaldiIniziali,
                                     GestCont_Flag_ConsideraSaldiIniziali,
                                     GestCont_DataInizio,
                                     Piva,
                                     Anno,
                                     Ric_Cod,
                                     Id_Riclassificazione,
                                     Dare_Avere,
                                     Imputabile,
                                     Cod_Conto,
                                     Flag_UE,
                                     Cod_Contatto,
                                     Sezionale_Cod,
                                     Sezionale_ChkDefault,
                                     Cod_RisUm,
                                     Lista_CodRisUm,
                                     Cod_Liquidita,
                                     Flag_ContiSaldo0,
                                     Flag_AggiungiContoPadre_CreditiDebitiBanche,
                                     xFiltroAggiuntivo1,
                                     xFiltroAggiuntivo2,
                                     xFiltroAggiuntivo3,
                                     xFiltroAggiuntivo4,
                                     DT_Codifiche,
                                     objParametri_Server,
                                     strRisposta)


            r.RispostaOK = True
            r.RispostaStringa = strRisposta

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r.RispostaStringa
    End Function


    <WebMethod()>
    Public Function WS_Update_SaldiIniziali(ByVal Piva As String,
                                            ByVal SaCod As Integer,
                                            ByVal TipoConto As String,
                                            ByVal CodConto As Integer,
                                            ByVal CodContatto As Integer,
                                            ByVal TipoContatto As Integer,
                                            ByVal CodBanca As Integer,
                                            ByVal ValSaldo As Double,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal objP_server As String
                                            ) As Boolean

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

        Try

            Dim objCoreStampeDAL As New AgronicaCoreStampeDAL.PianoConti

            r.RispostaOK = objCoreStampeDAL.Update_SaldiIniziali(Piva,
                                                                 SaCod,
                                                                 TipoConto,
                                                                 CodConto,
                                                                 CodContatto,
                                                                 TipoContatto,
                                                                 CodBanca,
                                                                 ValSaldo,
                                                                 xFiltroAggiuntivo,
                                                                 objParametri_Server)
            
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r.RispostaOK
    End Function


    <WebMethod()>
    Public Function WS_Azzera_SaldiIniziali(ByVal Piva As String,
                                            ByVal SaCod As Integer,
                                            ByVal objP_server As String
                                            ) As Boolean
        
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

        Try


            Dim objCoreStampeDAL As New AgronicaCoreStampeDAL.PianoConti

            r.RispostaOK = objCoreStampeDAL.Azzera_SaldiIniziali(Piva,
                                                                 SaCod,
                                                                 objParametri_Server)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r.RispostaOK
    End Function

    <WebMethod()>
    Public Function WS_Leggi_ContiEconomici(ByVal objP_server As String,
                                            ByVal objP_utenti As String,
                                            ByVal piva As String,
                                            ByVal anno As Integer,
                                            ByVal ricCod As Integer,
                                            ByVal tipoDareAvere As String,
                                            ByVal codConto As Integer,
                                            ByVal imputabile As Integer
                                            ) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

        Try
            Dim objRicxCod As New AgronicaCoreContabDAL.RicxConti_R

            Dim xFiltroAggiuntivo As String = ""
            If imputabile <> 0 Then
                xFiltroAggiuntivo = " RicXConti.Imputabile = " & Agro_SQL_SaveNum(imputabile) & " "
            End If

            Dim xOrderBy As String = " RicXConti.Id_Riclassificazione, Conti.Conto_Descr"

            Dim dtRicxCod = objRicxCod.Leggi_New(piva,
                                                 ricCod,
                                                 anno,
                                                 codConto,
                                                 "",
                                                 tipoDareAvere,
                                                 xFiltroAggiuntivo,
                                                 xOrderBy,
                                                 objParametri_Server)

            Dim dtConti As New DataTable
            dtConti.Columns.Add("Cod_Conto", GetType(Integer))
            dtConti.Columns.Add("Anno", GetType(Integer))
            dtConti.Columns.Add("Descr_Conto", GetType(String))
            For Each row In dtRicxCod.Rows
                Dim dr = dtConti.NewRow
                dr("Anno") = row.Item("Anno")
                dr("Cod_Conto") = row.Item("Cod_Conto")
                dr("Descr_Conto") = row.Item("Id_Riclassificazione") & " - " & row.Item("Conto_Descr")
                dtConti.Rows.Add(dr)
            Next

            Dim serializerSettings As New JsonSerializerSettings With { .ReferenceLoopHandling = ReferenceLoopHandling.Ignore }
            r.RispostaStringa = JsonConvert.SerializeObject(dtConti, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod()>
    Public Function WS_Leggi_ContiPatrimoniali(ByVal objP_server As String,
                                               ByVal objP_utenti As String,
                                               ByVal piva As String,
                                               ByVal anno As Integer,
                                               ByVal ricCodPat As Integer,
                                               ByVal tipoDareAvere As String,
                                               ByVal codContoPat As Integer,
                                               ByVal imputabile As Integer
                                               ) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

        Try
            Dim objRicxConti_Pat As New AgronicaCoreContabDAL.RicxConti_Patrimonio_R

            Dim xFiltroAggiuntivo As String = ""
            If imputabile <> 0 Then
                xFiltroAggiuntivo = " RicxConti_Patrimonio.Imputabile = " & Agro_SQL_SaveNum(imputabile) & " "
            End If

            Dim xOrderBy As String = " RicxConti_Patrimonio.Id_Riclassificazione, Conti_Patrimonio.Conto_Pat_Descr"

            Dim dtRicxCod_Pat = objRicxConti_Pat.LeggiEstesa(piva,
                                                             ricCodPat,
                                                             anno,
                                                             codContoPat,
                                                             "",
                                                             tipoDareAvere,
                                                             xFiltroAggiuntivo,
                                                             xOrderBy,
                                                             objParametri_Server)

            Dim dtContiPat As New DataTable
            dtContiPat.Columns.Add("Cod_Conto_Pat", GetType(Integer))
            dtContiPat.Columns.Add("Anno", GetType(Integer))
            dtContiPat.Columns.Add("Descr_Conto_Pat", GetType(String))
            For Each row In dtRicxCod_Pat.Rows
                Dim dr = dtContiPat.NewRow
                dr("Anno") = row.Item("Anno")
                dr("Cod_Conto_Pat") = row.Item("Cod_Conto_Pat")
                dr("Descr_Conto_Pat") = row.Item("Id_Riclassificazione") & " - " & row.Item("Conto_Pat_Descr")
                dtContiPat.Rows.Add(dr)
            Next

            Dim serializerSettings As New JsonSerializerSettings With { .ReferenceLoopHandling = ReferenceLoopHandling.Ignore }
            r.RispostaStringa = JsonConvert.SerializeObject(dtContiPat, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod()>
    Public Function WS_Leggi_AnniAperti(ByVal objP_server As String,
                                        ByVal objP_utenti As String,
                                        ByVal piva As String
                                        ) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

        Try
            Dim objRicxCod As New AgronicaCoreContabDAL.RicxConti_R
            Dim objRicxConti_Pat As New AgronicaCoreContabDAL.RicxConti_Patrimonio_R
            Dim anniList As New List(Of Integer)
            Dim xFiltroAggiuntivo As String = " RicXConti.Imputabile = 1"
            Dim DTRicxCod = objRicxCod.LeggiDistinctAnno(piva,
                                                         2,
                                                         xFiltroAggiuntivo,
                                                         objParametri_Server)

            For Each row In DTRicxCod.Rows
                If Not anniList.Contains(row.Item("Anno")) Then
                    anniList.Add(row.Item("Anno"))
                End If
            Next

            xFiltroAggiuntivo = " RicxConti_Patrimonio.Imputabile = 1"
            Dim DTRicxCod_Pat = objRicxConti_Pat.LeggiDistinctAnno(piva,
                                                                   2,
                                                                   xFiltroAggiuntivo,
                                                                   objParametri_Server)

            For Each row In DTRicxCod_Pat.Rows
                If Not anniList.Contains(row.Item("Anno")) Then
                    anniList.Add(row.Item("Anno"))
                End If
            Next

            Dim dtDatiAggregati As New DataTable
            dtDatiAggregati.Columns.Add(New DataColumn("Anno", GetType(Integer)))
            dtDatiAggregati.Columns.Add(New DataColumn("Anno_Descr", GetType(Integer)))
            Dim d0 As DataRow
            For i As Integer = 0 To anniList.Count - 1
                d0 = dtDatiAggregati.NewRow
                d0("Anno") = anniList(i)
                d0("Anno_Descr") = anniList(i)
                dtDatiAggregati.Rows.Add(d0)
            Next

            Dim serializerSettings As New JsonSerializerSettings With { .ReferenceLoopHandling = ReferenceLoopHandling.Ignore }
            r.RispostaStringa = JsonConvert.SerializeObject(dtDatiAggregati, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

End Class
