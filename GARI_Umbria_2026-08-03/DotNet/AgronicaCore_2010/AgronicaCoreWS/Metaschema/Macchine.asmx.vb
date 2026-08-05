Imports System.ComponentModel
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class Macchine1
    Inherits System.Web.Services.WebService

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Leggi_Ditte(ByVal objP_super_server As String,
                                         ByVal objP_server As String,
                                         ByVal objP_utenti As String) As RispostaStandard
        Dim r As New RispostaStandard

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

            Dim objCodiceAnagrafe As New AgronicaCoreMetaSchemaDAL.Ditte_R

            Dim DT = objCodiceAnagrafe.Leggi(0,
                                     "",
                                     "",
                                     "",
                                     objParametri_Server)

            DT = DT.DefaultView.ToTable(True, "Ditta_cod", "Ditta_des")

            DT.Columns("Ditta_cod").ColumnName = "codice"
            DT.Columns("Ditta_des").ColumnName = "descrizione"

            Dim serializerSettings As New JsonSerializerSettings()

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(DT, Formatting.None, serializerSettings)
        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Leggi_Ditte_NG(ByVal InData As CoreWS_Generic(Of String)) As RispostaStandard
        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim objCodiceAnagrafe As New AgronicaCoreMetaSchemaDAL.Ditte_R

            Dim DT = objCodiceAnagrafe.Leggi(0,
                                     "",
                                     "",
                                     "",
                                     objParametri_Server)

            DT = DT.DefaultView.ToTable(True, "Ditta_cod", "Ditta_des")

            DT.Columns("Ditta_cod").ColumnName = "codice"
            DT.Columns("Ditta_des").ColumnName = "descrizione"

            Dim serializerSettings As New JsonSerializerSettings()

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(DT, Formatting.None, serializerSettings)
        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Leggi_Tipo(ByVal objP_super_server As String,
                               ByVal objP_server As String,
                               ByVal objP_utenti As String,
                               ByVal cmbTipo As String,
                               ByVal cmbDettaglio1 As String,
                               ByVal cmbDettaglio2 As String,
                               ByVal livello As Integer) As RispostaStandard
        Dim r As New RispostaStandard
        Dim Controllo As New Global.System.Web.UI.WebControls.DropDownList

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

            Dim objCodiceAnagrafe As New AgronicaCoreUtility.CaricaListControl

            Dim StrLivello1 = If(cmbTipo = "", "", cmbTipo)
            Dim StrLivello2 = If(cmbDettaglio1 = "", "", cmbDettaglio1)

            objCodiceAnagrafe.TipoMacchine(Controllo,
                                                    livello,
                                                    StrLivello1,
                                                    StrLivello2,
                                                    objParametri_Server)

            Dim DT As New DataTable
            Dim row As DataRow
            DT.Columns.Add("codice", GetType(String))
            DT.Columns.Add("descrizione", GetType(String))

            If (Controllo.Items.Count > 0) Then
                For Each li As ListItem In Controllo.Items

                    row = DT.NewRow()
                    row("codice") = li.Value
                    row("descrizione") = li.Text

                    DT.Rows.Add(row)
                Next
            End If

            Dim serializerSettings As New JsonSerializerSettings()

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(DT, Formatting.None, serializerSettings)
        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    'Public Function Leggi_Tipo(ByVal objP_super_server As String,
    '                           ByVal objP_server As String,
    '                           ByVal objP_utenti As String,
    '                           ByVal cmbTipo As String,
    '                           ByVal cmbDettaglio1 As String,
    '                           ByVal cmbDettaglio2 As String,
    '                           ByVal livello As Integer) As RispostaStandard
    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Leggi_Tipo_NG(ByVal InData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Metaschema.LeggiTipo)) As RispostaStandard
        Dim r As New RispostaStandard
        Dim Controllo As New Global.System.Web.UI.WebControls.DropDownList

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim objCodiceAnagrafe As New AgronicaCoreUtility.CaricaListControl

            Dim StrLivello1 = If(InData.InData.cmbTipo = "", "", InData.InData.cmbTipo)
            Dim StrLivello2 = If(InData.InData.cmbDettaglio1 = "", "", InData.InData.cmbDettaglio1)

            objCodiceAnagrafe.TipoMacchine(Controllo,
                                                    InData.InData.livello,
                                                    StrLivello1,
                                                    StrLivello2,
                                                    objParametri_Server)

            Dim DT As New DataTable
            Dim row As DataRow
            DT.Columns.Add("codice", GetType(String))
            DT.Columns.Add("descrizione", GetType(String))

            If (Controllo.Items.Count > 0) Then
                For Each li As ListItem In Controllo.Items

                    row = DT.NewRow()
                    row("codice") = li.Value
                    row("descrizione") = li.Text

                    DT.Rows.Add(row)
                Next
            End If

            Dim serializerSettings As New JsonSerializerSettings()

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(DT, Formatting.None, serializerSettings)
        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Leggi_TabellaCostoUnitario_NG(ByVal InData As CoreWS_Generic(Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG)) As RispostaStandard

        Dim r As New RispostaStandard
        Dim Controllo As New Global.System.Web.UI.WebControls.DropDownList

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim objParametriAgenda = InData.InData

            Dim DT As DataTable
            Dim objCodiceAnagrafe As New AgronicaCoreContabDAL.Prodotti_Costi_R

            If objParametriAgenda.Mac_Cod <> 0 Then
                DT = objCodiceAnagrafe.Leggi_Macchine(objParametriAgenda.Piva,
                                             objParametriAgenda.Mac_Cod,
                                             AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                             "", "",
                                             objParametri_Server)

                'creo la lista delle colonne da visualizzare
                Dim l As New List(Of ColonneNome)

                l.Add(New ColonneNome("ID", "ID", "string"))
                l.Add(New ColonneNome("Prezzo_Unitario", "Prezzo_Unitario", "string"))
                l.Add(New ColonneNome("Piva", "Piva", "string"))

                Dim js As New AgronicaCoreDataProvider.JSON_DataTable
                Dim risp As String = js.JSON_DataTable_Kendo(DT, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa)

                r.RispostaOK = True
                r.RispostaStringa = risp
            Else
                r.RispostaStringa = JsonConvert.SerializeObject(New List(Of Object), Newtonsoft.Json.Formatting.None)
                r.RispostaOK = True
            End If

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function
    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Leggi_TabellaCostoUnitario(
                                                ByVal InData As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG,
                                                ByVal objP_super_server As String,
                                                ByVal objP_server As String,
                                                ByVal objP_utenti As String
                                              ) As RispostaStandard

        Dim r As New RispostaStandard
        Dim Controllo As New Global.System.Web.UI.WebControls.DropDownList

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

            Dim objParametriAgenda = InData

            Dim DT As DataTable
            Dim objCodiceAnagrafe As New AgronicaCoreContabDAL.Prodotti_Costi_R

            If objParametriAgenda.Mac_Cod <> 0 Then
                DT = objCodiceAnagrafe.Leggi_Macchine(objParametriAgenda.Piva,
                                             objParametriAgenda.Mac_Cod,
                                             AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                             "", "",
                                             objParametri_Server)

                'creo la lista delle colonne da visualizzare
                Dim l As New List(Of ColonneNome)

                l.Add(New ColonneNome("ID", "ID", "string"))
                l.Add(New ColonneNome("Prezzo_Unitario", "Prezzo_Unitario", "string"))
                l.Add(New ColonneNome("Piva", "Piva", "string"))

                Dim js As New AgronicaCoreDataProvider.JSON_DataTable
                Dim risp As String = js.JSON_DataTable_Kendo(DT, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa)

                r.RispostaOK = True
                r.RispostaStringa = risp
            Else
                r.RispostaStringa = JsonConvert.SerializeObject(New List(Of Object), Newtonsoft.Json.Formatting.None)
                r.RispostaOK = True
            End If

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function
    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Leggi_TutteMacchine_NG(InData As CoreWS_Generic(Of Object)) As RispostaStandard

        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim coreM As New AgronicaCoreMetaSchemaDAL.Macchine_R
            'le leggo tutte..
            Dim dtMac As DataTable = coreM.Leggi("", objParametri_Server)

            Dim view As New DataView(dtMac)

            ' Sort by CLASS_CODE column 
            view.Sort = "CLASS_CODE ASC"

            dtMac = view.ToTable()

            'creo la lista delle colonne da visualizzare
            Dim l As New List(Of ColonneNome)

            l.Add(New ColonneNome("CLASS_DESC", "CLASS_DESC", "string"))
            l.Add(New ColonneNome("CLASS_CODE", "CLASS_CODE", "string"))

            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            Dim risp As String = js.JSON_DataTable(dtMac, l)

            Dim serializerSettings As New JsonSerializerSettings()

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(dtMac, Formatting.None, serializerSettings)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Leggi_TutteMacchine(ByVal objP_super_server As String,
                                               ByVal objP_server As String,
                                               ByVal objP_utenti As String) As RispostaStandard

        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim coreM As New AgronicaCoreMetaSchemaDAL.Macchine_R
            'le leggo tutte..
            Dim dtMac As DataTable = coreM.Leggi("", objParametri_Server)

            Dim view As New DataView(dtMac)

            ' Sort by CLASS_CODE column 
            view.Sort = "CLASS_CODE ASC"

            dtMac = view.ToTable()

            'creo la lista delle colonne da visualizzare
            Dim l As New List(Of ColonneNome)

            l.Add(New ColonneNome("CLASS_DESC", "CLASS_DESC", "string"))
            l.Add(New ColonneNome("CLASS_CODE", "CLASS_CODE", "string"))

            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            Dim risp As String = js.JSON_DataTable(dtMac, l)

            Dim serializerSettings As New JsonSerializerSettings()

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(dtMac, Formatting.None, serializerSettings)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function


End Class