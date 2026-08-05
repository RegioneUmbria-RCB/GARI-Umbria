Imports System.Web.Services
Imports AgronicaCoreScadenziario_BIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreVarieBIZ

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
Public Class Alert_Avvisi_WS
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Avvisi_ToKendoGrid(objP_server As String, objP_utenti As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim xFiltroAggiuntivo As String = ""
        Dim dtRapCon As New DataTable


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

        Dim Avvisi_R As New AgronicaCoreScadenziario.Alert_Avvisi_R
        Dim dt As DataTable = Avvisi_R.Leggi(Nothing, Nothing, Nothing, "", Nothing, Nothing, "", "", objParametri_Server)


        dt.Columns.Add(New DataColumn("Filtro_RapCon_Des", GetType(String)))

        If dt.Rows.Count > 0 Then

            Dim objRapCon As New AgronicaCoreAnagrafeDAL.Rapporti_Contabili_R

            For Each dr As DataRow In dt.Rows

                dr.Item("Filtro_RapCon_Des") = "" 'Inizializzazione

                If Trim(dr.Item("Filtro_RapCon")) <> "" Then

                    xFiltroAggiuntivo = "Cod_Rapporto In (" & dr.Item("Filtro_RapCon") & ")"
                    dtRapCon = objRapCon.RapportiContabili_Leggi("", xFiltroAggiuntivo, "", objParametri_Server)

                    If dtRapCon.Rows.Count > 0 Then

                        For Each drRapCon As DataRow In dtRapCon.Rows

                            dr.Item("Filtro_RapCon_Des") = dr.Item("Filtro_RapCon_Des") & IIf(dr.Item("Filtro_RapCon_Des") = "", "", ", ") & drRapCon("Rapporto_Des")
                        Next

                    End If



                End If

            Next

        End If


        If dt.Rows.Count > 0 Then



        End If

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)
        l.Add(New ColonneNome("ID_Avviso", "ID Avviso", "number") With {._hidden = True})
        l.Add(New ColonneNome("ID_Area", "ID_Area", "number") With {._hidden = True})
        l.Add(New ColonneNome("Area", Gias.Area, "string"))
        l.Add(New ColonneNome("ID_Tipologia", "ID_Tipologia", "number") With {._hidden = True})
        l.Add(New ColonneNome("Tipologia", Gias.Tipologia, "string"))
        l.Add(New ColonneNome("Filtro_RapCon", "Filtro_RapCon", "string") With {._hidden = True})
        l.Add(New ColonneNome("Filtro_RapCon_Des", "Rapporto Contabile", "string"))
        l.Add(New ColonneNome("GGAttesa", Gias.GiorniDiAttesa, "number"))
        l.Add(New ColonneNome("GGAttesa_Old", Gias.GiorniDiAttesa, "number") With {._hidden = True})
        l.Add(New ColonneNome("MailMittente", Gias.MailMittente, "string"))
        l.Add(New ColonneNome("MailA", Gias.MailDestinatari, "string"))
        l.Add(New ColonneNome("MailCC", Gias.MailCopiaConoscenza, "string"))

        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        r.RispostaStringa = js.JSON_DataTable_Kendo(dt, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.Menu, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True)
        r.RispostaOK = True

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiElem(objP_server As String, ID_Avviso As Integer) As rispostaStandard(Of Alert_Avvisi)

        Dim r As New rispostaStandard(Of Alert_Avvisi)

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

        Dim Avvisi_R As New AgronicaCoreScadenziario_BIZ.Alert_Avvisi_R
        Dim avv As AgronicaCoreScadenziario_BIZ.Alert_Avvisi = Avvisi_R.leggi_Alert_Avvisi(objParametri_Server, ID_Avviso).First()

        r.RispostaStringa = avv
        r.RispostaOK = True

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Modifica(objP_server As String,
                             Old_ID_Avviso As Integer, New_ID_Area As Integer, New_ID_Tipologia As Integer,
                             New_Filtro_RapCon As String, New_ID_Evento As Integer,
                             New_GGAttesa As Integer, New_MailMittente As String,
                             New_MailA As String, New_MailCC As String) As RispostaStandard

        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

        Dim Alert_W As New Alert_Avvisi_W
        Dim err As String = Alert_W.modifica(objParametri_Server, Old_ID_Avviso, New_ID_Area, New_ID_Tipologia,
                                             New_Filtro_RapCon, New_ID_Evento, New_GGAttesa, New_MailMittente,
                                             New_MailA, New_MailCC)

        If err = "" Then
            r.RispostaOK = True
        Else
            r.RispostaOK = False
            r.Errore = err
        End If

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Aggiungi(objP_server As String,
                             ID_Area As Integer,
                             ID_Tipologia As Integer,
                             Filtro_RapCon As String,
                             ID_Evento As Integer,
                             GGAttesa As Integer,
                             MailMittente As String,
                             MailA As String,
                             MailCC As String) As RispostaStandard

        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

        'Salvo l'elemento
        Dim Alert_W As New Alert_Avvisi_W
        Dim err As String = Alert_W.aggiungi(objParametri_Server, ID_Area, ID_Tipologia, Filtro_RapCon, ID_Evento, GGAttesa, MailMittente, MailA, MailCC)

        If err = "" Then
            r.RispostaOK = True
        Else
            r.RispostaOK = False
            r.Errore = err
        End If

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Cancella(objP_server As String, ID_Avviso As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

        Dim Alert_W As New Alert_Avvisi_W
        Dim err As String = Alert_W.cancella(objParametri_Server, ID_Avviso)

        If err = "" Then
            r.RispostaOK = True
        Else
            r.RispostaOK = False
            r.Errore = err
        End If

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function TestMail(objP_server As String, objP_utenti As String, ID_Avviso As Integer) As RispostaStandard
        Dim r As New RispostaStandard

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

        Try

            'Leggo l'avviso
            Dim Avvisi_R As New Alert_Avvisi_R
            Dim avv As Alert_Avvisi = Avvisi_R.leggi_Alert_Avvisi(objParametri_Server, ID_Avviso).First()

            'Invio la mail
            Dim mail As New AgronicaCoreUtility.Mail
            mail.invia(objParametri_Server, avv.MailMittente, avv.MailA,
                            avv.MailCC, Nothing, Gias.MailDiProvaPerLeScadenze,
                            Gias.InvioEffettuatoIl_ & DateTime.Now.ToString(), True, Nothing)

            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r

    End Function

End Class