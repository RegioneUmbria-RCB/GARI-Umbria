Imports System.Globalization
Imports System.IO
Imports System.Web.Services
Imports AgronicaCoreAnagrafeBIZ
Imports AgronicaCoreContabObject
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDTOStd.InData.Agenda
Imports AgronicaCoreGisBIZ
Imports AgronicaCoreModelsSTD.metaschema.utilizzi
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _

<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class Impianti
    Inherits System.Web.Services.WebService

    ' TODO Andrea: valutare come centralizzare con medesima classe in GisWS.asmx.vb

    Private Class ObjParametri(Of T)
        Public Super_Server As AgronicaCoreParametri
        Public Server As AgronicaCoreParametri
        Public Utenti As AgronicaCoreParametri
        Public InData As T
    End Class

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function getJSON_Agenda_js() As String
        Dim dd = File.ReadAllText("C:\AgroSorgenti\AgronicaCore_2010\AgronicaCoreWS\Agenda\JSON_Agenda.json")
        Return dd
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function getListaImpianti(ByVal Piva As String, ByVal Sa_Cod As Integer,
                                     ByVal Disciplinare_Cod As String,
                                     ByVal Veg_Cod As Integer, ByVal ID_Cod As Integer,
                                     ByVal Data As String,
                                ByVal objParametri As String) As List(Of Impianto)
        Dim risp As New List(Of Impianto)


        Dim obJP As AgronicaCoreDataProvider.AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objParametri)


        Dim Dt As New DataTable

        Dim Array() As String
        Array = Split(Disciplinare_Cod, "/")

        Dim Grfi_Cod As Integer
        Dim Flag_Protetto As Integer
        Dim Flag_Disciplinare As Boolean
        Dim Flag_PubblicoPrivato As Integer = 0
        If Not IsNothing(Array) And Array.Length > 1 Then
            Grfi_Cod = Array(2)
            Flag_Protetto = Array(3)
            Flag_PubblicoPrivato = Array(4)
            Flag_Disciplinare = True
        Else
            Grfi_Cod = 0
            Flag_Protetto = 0
            Flag_PubblicoPrivato = 0
            Flag_Disciplinare = False
        End If


        Dim LeggiAncheBloccati As Boolean = False
        Dim FiltroAggiuntivo As String = ""

        risp = Impianto.getListaImpianti(Piva, Sa_Cod, Veg_Cod, ID_Cod, Data, Grfi_Cod, Flag_Protetto, Flag_Disciplinare, Flag_PubblicoPrivato, LeggiAncheBloccati, FiltroAggiuntivo, obJP)

        Return risp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiImpianti_NG(InData As CoreWS_Generic(Of LeggiImpianti)) As RispostaStandard

        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try


            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            'Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            'If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            '    r.Sessione = False
            '    Return r
            'End If

            Dim objReg_Impianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
            Dim DT As DataTable
            Dim DataInizio = CostantiPersonalizzate.AGRODATAINIZIO
            Dim DataFine = CostantiPersonalizzate.AGRODATAFINE
            If IsNumeric(InData.InData.Veg_Cod) Then
                DT = objReg_Impianti.Leggi_ImpiantiMenuAgenda(InData.InData.piva, InData.InData.Sa_Cod, 0, 0, InData.InData.Veg_Cod, -1, DataInizio, DataFine, "", "", objParametri_Server)
            Else
                DT = objReg_Impianti.Leggi_ImpiantiMenuAgenda(InData.InData.piva, InData.InData.Sa_Cod, 0, 0, -1, InData.InData.Veg_Cod.Split("/")(1), DataInizio, DataFine, "", "", objParametri_Server)
            End If
            '

            Dim returnArray As New JArray

            For Each row In DT.Rows
                Dim objRet As New JObject
                objRet("chiave") = CStr(row("piva") & "_" & row("sa_cod") & "_" & row("appezza") & "_" & row("id_reg"))
                Dim des As String = ""
                If (row("Campo_Des") <> "") Then
                    des &= row("Campo_Des") & " - "
                End If
                des &= row("App_Nome") & " - " & row("Cul_Des") & " - " & row("Sup_Imp")
                objRet("des") = des
                returnArray.Add(objRet)
            Next

            r.RispostaOK = True
            r.RispostaStringa = returnArray.ToString
            Return r
        Catch ex As Exception
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiImpianti(objP_server As String, piva As String, Sa_Cod As Integer, Veg_Cod As String, Data_Inizio As Date, Data_Fine As Date) As RispostaStandard

        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try


            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            'Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            'If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            '    r.Sessione = False
            '    Return r
            'End If

            Dim objReg_Impianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
            Dim DT As DataTable
            Dim DataInizio = CostantiPersonalizzate.AGRODATAINIZIO
            Dim DataFine = CostantiPersonalizzate.AGRODATAFINE
            If IsNumeric(Veg_Cod) Then
                DT = objReg_Impianti.Leggi_ImpiantiMenuAgenda(piva, Sa_Cod, 0, 0, Veg_Cod, -1, DataInizio, DataFine, "", "", objParametri_Server)
            Else
                DT = objReg_Impianti.Leggi_ImpiantiMenuAgenda(piva, Sa_Cod, 0, 0, -1, Veg_Cod.Split("/")(1), DataInizio, DataFine, "", "", objParametri_Server)
            End If
            '

            Dim returnArray As New JArray

            For Each row In DT.Rows
                Dim objRet As New JObject
                objRet("chiave") = CStr(row("piva") & "_" & row("sa_cod") & "_" & row("appezza") & "_" & row("id_reg"))
                Dim des As String = ""
                If (row("Campo_Des") <> "") Then
                    des &= row("Campo_Des") & " - "
                End If
                des &= row("App_Nome") & " - " & row("Cul_Des") & " - " & row("Sup_Imp")
                objRet("des") = des
                returnArray.Add(objRet)
            Next

            r.RispostaOK = True
            r.RispostaStringa = returnArray.ToString
            Return r
        Catch ex As Exception
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function getListaImpianti_APP(ByVal piva As String,
                                         ByVal Data As String,
                                         ByVal objP_super_server As String,
                                         ByVal objP_server As String,
                                         ByVal objP_utenti As String
                                         ) As RispostaStandard

        Dim r As New RispostaStandard

        Try

            'TODO: fare fix definitivo per data
            Dim data_Date As Date

            If Date.TryParseExact(Data, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, Nothing) Then
                data_Date = Date.ParseExact(Data, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal)
            ElseIf Not IsDate(Data) Then
                data_Date = Today
            End If

            Dim objParametri_SuperServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim objImpiantiBIZ_R As New Reg_Impianto_R
            Dim objImpiantiBIZ_W As New Reg_Impianto_W
            Dim LeggiAncheBloccati As Boolean = objImpiantiBIZ_R.BlockedImplantsReadSetting(objParametri_Utenti)

            Dim StaticMapCFG As New GeneraMappaStaticaInData
            Dim dtImpianti As DataTable
            Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read

            Dim LetturaCFG As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim jSonStaticMapCFG As String = LetturaCFG.Leggi_Valore(0, "GIS_StaticMapCFG", "", "", objParametri_Server)
            Dim leggiStaticMap As Boolean = False
            Dim GeneraStaticMapDaSincroAPP As Boolean = False
            If jSonStaticMapCFG <> "" Then
                StaticMapCFG = JsonConvert.DeserializeObject(Of GeneraMappaStaticaInData)(jSonStaticMapCFG)
                If StaticMapCFG.StaticMapAttive Then
                    leggiStaticMap = True
                End If
                If StaticMapCFG.GeneraStaticMapDaSincroAPP Then
                    GeneraStaticMapDaSincroAPP = True
                End If
            End If

            dtImpianti = objImpianti.Leggi_Impianti_APP(
                piva,
                0,
                0,
                0,
                "",
                data_Date,
                leggiStaticMap OrElse GeneraStaticMapDaSincroAPP,
                "",
                xOrderBy:=" Cul_Des, App_Nome ",
                objParametri_Server,
                LeggiAncheBloccati
                )

            'Troviamo tutti gli impianti che non hanno la StaticMap e la generiamo
            If GeneraStaticMapDaSincroAPP Then
                objImpiantiBIZ_W.GeneraStaticMapDaSincroAPP(dtImpianti, StaticMapCFG, objParametri_Server)
            End If

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(dtImpianti, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiUtilizzoTerrenoImpianto(ByVal InData As Object) As rispostaStandard(Of UtilizzoTerreno)

        Dim r As New rispostaStandard(Of UtilizzoTerreno)

        Dim objParametri = DeserializzaInData(Of ChiaveImpianto)(InData)

        Dim chiaveImpianto = objParametri.InData

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try

            Dim objAppezzamenti As New Appezzamento_R

            r.RispostaStringa = objAppezzamenti.Leggi_Appezzamento_UtilizzoTerreno(chiaveImpianto.piva,
                                                                                   chiaveImpianto.saCod,
                                                                                   chiaveImpianto.appezza,
                                                                                   chiaveImpianto.idReg,
                                                                                   objParametri.Server)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    ' TODO Andrea: valutare come centralizzare con medesime funzioni in GisWS.asmx.vb

    Private Function DeserializzaInData(Of T)(ByVal InData As Object) As ObjParametri(Of T)

        Dim JsonSettings As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim objInData As CoreWS_Generic(Of T) = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of T))(JsonConvert.SerializeObject(InData), JsonSettings)

        Dim objParametri As New ObjParametri(Of T)

        objParametri.Super_Server = Utility.convertStringtoOBJparametri(objInData.objP.objP_super_server)
        objParametri.Server = Utility.convertStringtoOBJparametri(objInData.objP.objP_server)
        objParametri.Utenti = Utility.convertStringtoOBJparametri(objInData.objP.objP_utenti)

        objParametri.InData = objInData.InData

        Return objParametri

    End Function

    Private Shared Sub Gias_InizializzaCultura_DaParams(ByRef objParametri_Server As AgronicaCoreParametri,
                                                        ByRef objParametri_Utenti As AgronicaCoreParametri)

        Dim leggiLingua As New Lingue_Read

        Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod,
                                                                  AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                  "",
                                                                  "",
                                                                  objParametri_Utenti)

        Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")

        Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

    End Sub

End Class