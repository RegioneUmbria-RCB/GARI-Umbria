Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDTOStd.InData.Agenda
Imports AgronicaCoreGHGBIZ
Imports AgronicaCoreModelsSTD.attivita
Imports System.Globalization
Imports InData.Anagrafica

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class GHG_WS
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function OttieniParamQualDaRaccolte(
                                            agendas As List(Of RifAttivita),
                                            ByVal objP_super_server As String,
                                            ByVal objP_server As String,
                                            ByVal objP_utenti As String
    )
        Dim ris = New RispostaStandard()

        If objP_super_server = "" Then
            ris.Errore = "objP_super_server non valorizzato"
            Return ris
        End If

        If objP_server = "" Then
            ris.Errore = "objP_server non valorizzato"
            Return ris
        End If

        If objP_utenti = "" Then
            ris.Errore = "objP_utenti non valorizzato"
            Return ris
        End If

        Try
            Dim objParametri_SuperServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)
            Dim GHG = New AgronicaCoreGHGBIZ.GHG()
            ris.RispostaStringa = JsonConvert.SerializeObject(GHG.Ottieni_ParamQual_GHG(agendas, objParametri_SuperServer, objParametri_Server, objParametri_Utenti), Formatting.None)
            ris.RispostaOK = True
        Catch ex As Exception
            ris.Errore = $"Errore durante l'operazione: {vbCrLf}{Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)}"
            ris.RispostaOK = False
        End Try
        Return ris
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function OttieniEECPresunto(
                                piva As String,
                                elemCod As Integer,
                                matCod As Integer,
                                ByVal data As DateTime,
                                ByVal objP_server As String
    ) As RispostaStandard
        Dim ris = New RispostaStandard()

        If objP_server = "" Then
            ris.Errore = "objP_server non valorizzato"
            Return ris
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim GHG = New AgronicaCoreGHGBIZ.GHG()
            ris.RispostaStringa = GHG.Ottieni_EEC_Presunto(piva, elemCod, matCod, data, objParametri_Server).ToString("G", CultureInfo.InvariantCulture)
            ris.RispostaOK = True
        Catch ex As Exception
            ris.Errore = $"Errore durante l'operazione: {vbCrLf}{Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)}"
            ris.RispostaOK = False
        End Try
        Return ris
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiImpreseParametri_NG(InData As CoreWS_Generic(Of LeggiImpreseParametri)
    ) As rispostaStandard(Of List(Of ImpresaParametri))
        Dim ris = New rispostaStandard(Of List(Of ImpresaParametri))

        If InData.objP.objP_server = "" Then
            ris.Errore = "objP_server non valorizzato"
            Return ris
        End If

        Try
            Dim objParametri_Utente As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim GHG = New AgronicaCoreGHGBIZ.GHG()
            Dim dt As DataTable = GHG.leggi_ImpreseParametri(InData.InData.piva, InData.InData.specie, InData.InData.culCod, InData.InData.regolamentoCod, InData.InData.dataValiditaInizio, InData.InData.dataValiditaFine, objParametri_Utente, objParametri_Server)
            Dim result As List(Of ImpresaParametri) = New List(Of ImpresaParametri)()
            For Each row As DataRow In dt.Rows()
                result.Add(New ImpresaParametri With {
                           .ID = If(IsDBNull(row.Item("ID")), -1, row.Item("ID")),
                           .Piva = If(IsDBNull(row.Item("PIVA")), "", row.Item("PIVA")),
                           .Rag_Soc = If(IsDBNull(row.Item("Rag_Soc")), "", row.Item("Rag_Soc")),
                           .Specie = If(IsDBNull(row.Item("Veg_Cod")), 0, row.Item("Veg_Cod")),
                           .Veg_Des = If(IsDBNull(row.Item("Veg_Des")), "", row.Item("Veg_Des")),
                           .Varieta = If(IsDBNull(row.Item("Cul_Cod")), 0, row.Item("Cul_Cod")),
                           .Cul_Des = If(IsDBNull(row.Item("Cul_Des")), "", row.Item("Cul_Des")),
                           .Regolamento = If(IsDBNull(row.Item("Regolamento_Cod")), 0, row.Item("Regolamento_Cod")),
                           .Reg_Des = If(IsDBNull(row.Item("Reg_Des")), "", row.Item("Reg_Des")),
                           .Validita_Inizio = If(IsDBNull(row.Item("Validita_Inizio")), AGRODATAINIZIO, row.Item("Validita_Inizio")),
                           .Validita_Fine = If(IsDBNull(row.Item("Validita_Fine")), AGRODATAFINE, row.Item("Validita_Fine")),
                           .EEC = If(IsDBNull(row.Item("EEC")), 0, row.Item("EEC"))
                           })
            Next
            ris.RispostaOK = True
            ris.RispostaStringa = result
        Catch ex As Exception
            ris.Errore = $"Errore durante l'operazione: {vbCrLf}{Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)}"
            ris.RispostaOK = False
        End Try
        Return ris
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ScriviImpreseParametri_NG(InData As CoreWS_Generic(Of ScriviModificaImpreseParametri)
    ) As rispostaStandard(Of Boolean)
        Dim ris = New rispostaStandard(Of Boolean)

        If InData.objP.objP_server = "" Then
            ris.Errore = "objP_server non valorizzato"
            Return ris
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim GHG = New AgronicaCoreGHGBIZ.GHG()
            Dim result As Boolean = GHG.scrivi_ImpreseParametri(InData.InData.Piva, InData.InData.Veg_Cod, InData.InData.Cul_Cod, InData.InData.Regolamento_Cod, InData.InData.EEC, InData.InData.Validita_Inizio, InData.InData.Validita_Fine, objParametri_Server)

            ris.RispostaOK = True
            ris.RispostaStringa = result
        Catch ex As Exception
            ris.Errore = $"Errore durante l'operazione: {vbCrLf}{Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)}"
            ris.RispostaOK = False
        End Try
        Return ris
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ModificaImpreseParametri_NG(InData As CoreWS_Generic(Of ScriviModificaImpreseParametri)
    ) As rispostaStandard(Of Boolean)
        Dim ris = New rispostaStandard(Of Boolean)

        If InData.objP.objP_server = "" Then
            ris.Errore = "objP_server non valorizzato"
            Return ris
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim GHG = New AgronicaCoreGHGBIZ.GHG()
            Dim result As Boolean = GHG.modifica_ImpreseParametri(InData.InData.ID, InData.InData.Piva, InData.InData.Veg_Cod, InData.InData.Cul_Cod, InData.InData.Regolamento_Cod, InData.InData.EEC, InData.InData.Validita_Inizio, InData.InData.Validita_Fine, objParametri_Server)

            ris.RispostaOK = True
            ris.RispostaStringa = result
        Catch ex As Exception
            ris.Errore = $"Errore durante l'operazione: {vbCrLf}{Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)}"
            ris.RispostaOK = False
        End Try
        Return ris
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CancellaImpreseParametri_NG(InData As CoreWS_Generic(Of ScriviModificaImpreseParametri)
    ) As rispostaStandard(Of Boolean)
        Dim ris = New rispostaStandard(Of Boolean)

        If InData.objP.objP_server = "" Then
            ris.Errore = "objP_server non valorizzato"
            Return ris
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim GHG = New AgronicaCoreGHGBIZ.GHG()
            Dim result As Boolean = GHG.cancella_ImpreseParametri(InData.InData.ID, objParametri_Server)

            ris.RispostaOK = True
            ris.RispostaStringa = result
        Catch ex As Exception
            ris.Errore = $"Errore durante l'operazione: {vbCrLf}{Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)}"
            ris.RispostaOK = False
        End Try
        Return ris
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CalcolaGHGDaRaccolte(
                        agendas As List(Of RifAttivita),
                        ByVal objP_super_server As String,
                        ByVal objP_server As String,
                        ByVal objP_utenti As String
    ) As RispostaStandard
        Dim ris As New RispostaStandard()

        If objP_super_server = "" Then
            ris.Errore = "objP_super_server non valorizzato"
            Return ris
        End If

        If objP_server = "" Then
            ris.Errore = "objP_server non valorizzato"
            Return ris
        End If

        If objP_utenti = "" Then
            ris.Errore = "objP_utenti non valorizzato"
            Return ris
        End If

        Try
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)
            Dim GHG = New AgronicaCoreGHGBIZ.GHG()
            Dim ghgResult = GHG.Calcola_GHG_DaRaccolte(agendas, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)
            Dim ghgResponse = New With {
                Key .FF_ghgforec_Val_Cod = ghgResult.eec,
                Key .FF_ghgforel_Val_Cod = ghgResult.el,
                Key .FF_ghgforesca_Val_Cod = ghgResult.esca,
                Key .FF_ghgforetd_Val_Cod = ghgResult.etd,
                Key .FF_ghgtotal_Val_Cod = ghgResult.getTotalEmissions()
            }
            ris.RispostaStringa = JsonConvert.SerializeObject(ghgResponse, Formatting.None)
            ris.RispostaOK = True
        Catch ex As Exception
            ris.Errore = $"Errore durante l'operazione: {vbCrLf}{Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)}"
            ris.RispostaOK = False
        End Try
        Return ris
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiStandard_Factor(ByVal carCod As Integer,
                                         ByVal udmCod As Integer,
                                         ByVal direttivaCod As Integer,
                                         ByVal codiceStato As String,
                                         ByVal xfiltroaggiuntivo As String,
                                         ByVal objP_server As String) As RispostaStandard

        Dim r As New RispostaStandard()

        Try


            Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            If objP_server = "" Then
                r.Errore = "objP_server non valorizzato"
                Return r
            End If

            Dim leggi As New AgronicaCoreContabDAL.GHG_Registrazioni_R

            Dim Dt As DataTable = leggi.LeggiStandard_Factor(carCod, udmCod, direttivaCod, codiceStato, xfiltroaggiuntivo, objParametri_Server)

            r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r


    End Function



    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CalcoloStandard_Factor(ByVal qty_trasporto_attuale As String,
                                           ByVal carCod As Integer,
                                           ByVal udmCod As Integer,
                                           ByVal direttivaCod As Integer,
                                           ByVal codiceStato As String,
                                           ByVal xfiltroaggiuntivo As String,
                                           ByVal objP_server As String) As RispostaStandard

        Dim r As New RispostaStandard()
        Dim Standard_Factor As Decimal = 0

        Try
            Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim leggi As New AgronicaCoreContabDAL.GHG_Registrazioni_R

            Standard_Factor = leggi.CalcoloStandard_Factor(qty_trasporto_attuale,
                                                           carCod,
                                                           udmCod,
                                                           direttivaCod,
                                                           codiceStato,
                                                           xfiltroaggiuntivo,
                                                           objParametri_Server)


            r.RispostaStringa = Standard_Factor
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r


    End Function


End Class