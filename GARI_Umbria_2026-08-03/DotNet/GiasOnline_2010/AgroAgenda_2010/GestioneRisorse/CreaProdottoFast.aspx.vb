Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider
Imports System.Web.Services
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq


Public Class CreaProdottoFast
    Inherits System.Web.UI.Page

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Public objparametri_server_string, objparametri_utenti_string As String

    Public objProdotto As New JObject()
    Dim Elem_Cod As Integer
    Dim Piva As String

    Public Shadows ReadOnly Property Master() As AgroAgenda_2010.AgendaBootstrap
        Get
            Return CType(MyBase.Master, AgroAgenda_2010.AgendaBootstrap)
        End Get
    End Property


    '##############################################################################################
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Master().Lbl_Titolo.Text = "Creazione anagrafica prodotto"

        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objparametri_server_string = AgronicaCoreDataProvider.Utility.convertOBJparametritoString(objParametri_Server)
        objparametri_utenti_string = AgronicaCoreDataProvider.Utility.convertOBJparametritoString(objParametri_Utenti)

        'Controllo se l'utente ha i permessi per accedere
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim UtenteAbilitatoLettura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                            Session("ASG_Utente_Username"),
                                            Session("ASG_IdServizio"),
                                            enum_Security_Attivita.Gest_Prodotti,
                                            enum_Security_Operazione.Modifica,
                                            Date.Now,
                                            "",
                                            objParametri_Utenti)


        ''Imposto le variabili di ponte con il client
        'hf_UtenteAbilitatoLettura.Value = UtenteAbilitatoLettura

        If UtenteAbilitatoLettura = False Then
            'chiudo 
            Dim str As String = " $(document).ready(function () { "
            str &= "  window.close();"
            str &= "});"
            'ScriptManager.RegisterClientScriptBlock(Me.Master.FindControl("FORM1"), Me.Master.FindControl("FORM1").GetType(),
            '                                 String.Format("jQuery_{0}", Txt_DataMovimento.ClientID), str, True)

            Dim strClose As String = "<script language='javascript'>" & str & "</script>"
            Me.Master.FindControl("Form1").Controls.Add(New LiteralControl(strClose))
            'ClientScript.RegisterClientScriptBlock(Me.GetType(), "Close", "window.close()", True)
            Exit Sub

        End If

        Piva = Stringa_Decodifica(CStr(Request.QueryString("p")),
                                        AgroKey_EncoderDecoder,
                                        Server)

        Elem_Cod = Stringa_Decodifica(CStr(Request.QueryString("e")),
                                AgroKey_EncoderDecoder,
                                Server)

        objProdotto.Add(New JProperty("piva", Piva))
        objProdotto.Add(New JProperty("sa_cod", PRIVATO))
        objProdotto.Add(New JProperty("elem_cod", Elem_Cod))
        objProdotto.Add(New JProperty("cod_articolo", ""))
        objProdotto.Add(New JProperty("sem_cod", 0))
        objProdotto.Add(New JProperty("veg_cod", 0))
        objProdotto.Add(New JProperty("veg_des", ""))
        objProdotto.Add(New JProperty("cul_cod", 0))
        objProdotto.Add(New JProperty("cul_des", ""))
        objProdotto.Add(New JProperty("grva_cod", 0))
        objProdotto.Add(New JProperty("reg_cod", 0))

        hdElem_Cod.Value = Elem_Cod

        If Elem_Cod = 0 Then

            Dim str As String = " $(document).ready(function () { "
            str &= "  window.alert('ERRORE, categoria di magazzino non selezionata, impossibile procedere con l'inserimento del prodotto');"
            str &= "});"
            Dim strClose As String = "<script language='javascript'>" & str & "</script>"
            Me.Master.FindControl("Form1").Controls.Add(New LiteralControl(strClose))
            Exit Sub

        End If

        If IsPostBack Then
            Exit Sub
        End If

    End Sub


    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva(ByVal obj_Prodotto_str As String) As RispostaStandard

        'ByVal piva As String,
        'ByVal cod_articolo As String,
        ' ByVal SaCod_Visibilita As Integer,
        'ByVal elem_cod As Integer,
        'ByVal veg_cod As Integer,
        'ByVal cul_cod As Integer,
        'ByVal regolamento As Integer,
        'ByVal sem_cod As Integer,
        '    ByVal veg_des As String,
        'ByVal cul_des As String


        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If


        Try

            Dim obj_Prodotto = Newtonsoft.Json.JsonConvert.DeserializeObject(obj_Prodotto_str)

            Dim piva As String = obj_Prodotto.piva
            Dim SaCod_Visibilita As Integer = obj_Prodotto.sa_cod
            Dim Cod_Articolo As String = obj_Prodotto.cod_articolo
            Dim elem_cod As Integer = obj_Prodotto.elem_cod
            Dim veg_cod As Integer = obj_Prodotto.veg_cod
            Dim cul_cod As Integer = obj_Prodotto.cul_cod
            Dim sem_cod As Integer = obj_Prodotto.sem_cod
            Dim veg_des As String = obj_Prodotto.veg_des
            Dim cul_des As String = obj_Prodotto.cul_des
            Dim regolamento As Integer = obj_Prodotto.reg_cod

            Dim Mat_Des As String
            ' Dim Regolamento As enum_Cod_Regolamento
            Dim Flag_Biologico As Boolean

            Dim objCore_XML_Anagrafe As New AgronicaCoreXML.XML_Anagrafe
            Dim objCore_MP_W As New AgronicaCoreAnagrafeBIZ.Materie_Prime_W
            Dim objImportaGias As New AgronicaCoreAnagrafeBIZ.Importa_GIAS
            Dim OUTPUT_Mat_Cod As Integer = 0

            Dim Flag_Insert As Boolean
            Dim Basecode As Integer = 0
            Dim Topcode As Integer = 0

            OUTPUT_Mat_Cod = 0

            Select Case Elem_Cod

                Case SEMENTI, SEMILAVORATI_VEGETALI, TRASFORMATI_VEGETALI

                    If veg_cod <> 0 And cul_cod <> 0 Then

                        SaCod_Visibilita = PRIVATO
                        regolamento = enum_Cod_Regolamento.Regolamento_Nessuno

                        cod_articolo = Right("000" & veg_cod, 3) &
                                       "_" & Right("00000000" & CStr(cul_cod), 8)

                        Mat_Des = veg_des & " - " & cul_des

                        If regolamento = enum_Cod_Regolamento.Regolamento_bio Then
                            Flag_Biologico = True
                            Mat_Des &= " BIO"
                        Else
                            Flag_Biologico = False
                        End If

                        'flag_esiste = objCore_MP_R.Esiste_SemilavoratoVegetale(piva,
                        '                                                       veg_cod,
                        '                                                       cul_cod,
                        '                                                        " Sa_cod = -1 ",
                        '                                                        objParametri_Server)

                        '  If flag_esiste = False Then

                        Select Case Elem_Cod
                            Case SEMENTI

                                objImportaGias.Creazione_Automatica_Semente(objParametri_Server,
                                                            objCore_XML_Anagrafe,
                                                             objCore_MP_W,
                                                             Flag_Insert,
                                                             OUTPUT_Mat_Cod,
                                                             Basecode,
                                                             Topcode,
                                                             Piva,
                                                             SaCod_Visibilita,
                                                             cod_articolo,
                                                             Mat_Des,
                                                             veg_cod,
                                                             cul_cod,
                                                             sem_cod,
                                                             regolamento,
                                                             Flag_Biologico)

                            Case SEMILAVORATI_VEGETALI
                                objImportaGias.Creazione_Automatica_SemilavoratoVegetale(objParametri_Server,
                                                                 objCore_XML_Anagrafe,
                                                                  objCore_MP_W,
                                                                  Flag_Insert,
                                                                  OUTPUT_Mat_Cod,
                                                                  Basecode,
                                                                  Topcode,
                                                                  Piva,
                                                                  SaCod_Visibilita,
                                                                  cod_articolo,
                                                                  Mat_Des,
                                                                  veg_cod,
                                                                  cul_cod,
                                                                  regolamento,
                                                                  Flag_Biologico)

                            Case TRASFORMATI_VEGETALI
                                objImportaGias.Creazione_Automatica_TrasformatoVegetale(objParametri_Server,
                                                                 objCore_XML_Anagrafe,
                                                                  objCore_MP_W,
                                                                  Flag_Insert,
                                                                  OUTPUT_Mat_Cod,
                                                                  Basecode,
                                                                  Topcode,
                                                                  Piva,
                                                                  SaCod_Visibilita,
                                                                  cod_articolo,
                                                                  Mat_Des,
                                                                  veg_cod,
                                                                  cul_cod,
                                                                  regolamento,
                                                                  Flag_Biologico)

                        End Select


                        If Flag_Insert = True Then
                            r.RispostaOK = True
                        Else
                            r.RispostaOK = False
                            r.Errore = "Errore, prodotto non inserito. "
                        End If

                        'Log_Riepilogo.Append("Inserito: " & Mat_Des & " (" & Cod_Articolo & ") - mat_cod =" & CStr(OUTPUT_Mat_Cod) + vbCrLf)

                        'num_semilavorati_creati += 1

                        'Else
                        '    num_semilavorati_presenti += 1
                        '    Log_Riepilogo.Append("Già presente: " & Mat_Des + vbCrLf)
                        'End If

                    Else
                        'errore, il veg_cod e il cul_cod devono arrivare
                        r.RispostaOK = False
                        r.Errore = "Per l'inserimento del prodotto è necessario specificare specie e varietà "
                    End If

                Case Else
                    r.RispostaOK = False
                    r.Errore = "Categoria non gestita. Andare nella gestione prodotti e risorse per isnerire un prodotto di questa categoria. "
            End Select

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante il salvataggio del prodotto: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

End Class