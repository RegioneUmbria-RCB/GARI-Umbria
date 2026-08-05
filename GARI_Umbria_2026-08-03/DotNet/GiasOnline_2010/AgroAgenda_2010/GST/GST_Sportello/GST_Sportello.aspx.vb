

Imports System.Web.Services
Imports Newtonsoft.Json.Linq

Public Class GST_Sportello
    Inherits System.Web.UI.Page

    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        '----- Dimensiono le variabili
        objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")
        objParametri_Utenti = HttpContext.Current.Session("ASG_objParametri_Utenti")

        'parametri master page.
        Me.Master.flag_MostraBtnIndietro = True

        '----- Verifico se l'utente dispone dei permessi per la visualizzazione della pagina

        Dim UtenteAbilitato As Boolean = False


        If objParametri_Utenti.UtenteUsername <> objParametri_Server.SuperUserUsername Then
            UtenteAbilitato = False
        Else
            UtenteAbilitato = True
        End If

        If (UtenteAbilitato = False) Then
            Response.Redirect("../GST_Menu/Menu.aspx")
        End If

        '##############################################################
        '#####  Verifico se sono in Post-Back  ########################
        '##############################################################

        If Not IsPostBack Then
            'output.Write("Page has just been loaded")

        Else
            'output.Write("Postback has occured")
            Exit Sub
        End If

        '##############################################################
        '#####  Inizializzo la Pagina  ################################
        '##############################################################

    End Sub

    Private Sub GST_Sportello_Init(sender As Object, e As EventArgs) Handles Me.Init

        AddHandler CType(Me.Master, AgendaBootstrap).ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto
    End Sub


    Private Sub AnnullaTutto()
        Response.Redirect("../GST_Menu/GST_Menu.aspx")
    End Sub

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaElencoSpecie() As String

        Dim objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")

        'Dim leggiSementi As New AgronicaCoreSementieriDAL.ClassiDiSpecie_R
        'Dim dt As DataTable = leggiSementi.distinct_sementieri_classidispecievegetali_des(objParametri_Server)

        Dim obj_R As New AgronicaCoreSementieriDAL.Sportello_R()
        Dim dt = obj_R.LeggiClassiDiSpecieXSportello(0, objParametri_Server)

        Dim arrClassi As New JArray

        For Each dr In dt.Rows
            Dim objClasse = New JObject

            objClasse("IdSpecie") = CInt(dr("IdSpecie"))
            objClasse("DesSpecie") = dr("DesSpecie").ToString
            'objClasse("IdSpecie") = CInt(dr("id_specie"))
            'objClasse("DesSpecie") = dr("sementieri_classidispecievegetali_des").ToString

            arrClassi.Add(objClasse)
        Next

        Return arrClassi.ToString
    End Function


    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiPassaggi() As String

        Dim objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim obj_R As New AgronicaCoreSementieriDAL.Sportello_R()
        Dim dt = obj_R.Leggi_Passaggi(0, "", "", objParametri_Server)

        Dim arrPassaggi As New JArray

        For Each dr In dt.Rows

            Dim objPassaggio As New JObject

            objPassaggio("IdPassaggio") = CInt(dr("Sementieri_Sportello_Passaggi_cod"))
            objPassaggio("DesPassaggio") = dr("Sementieri_Sportello_Passaggi_des").ToString
            objPassaggio("VisImpianti") = CInt(dr("Visibilita_Impianti")) = 1
            objPassaggio("OpPermesse") = CInt(dr("DestinazioneSalvataggio")) >= 0
            objPassaggio("LogOperazioni") = CInt(dr("LoggaOperazioni")) = 1
            objPassaggio("Notifiche") = CInt(dr("RichiediConfermaSuInterferenze")) = 1

            arrPassaggi.Add(objPassaggio)

        Next

        Return arrPassaggi.ToString()

    End Function


    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaGrigliaSportelli() As String

        Dim objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim obj_R As New AgronicaCoreSementieriDAL.Sportello_R()
        Dim DT = obj_R.Leggi_Sementieri_Sportello_Configurazione(0,
                                                                 AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO,
                                                                 "Validita_Inizio DESC, Sementieri_Sportello_Configurazione_cod DESC",
                                                                 objParametri_Server)

        Dim elencoSportelli As New JArray

        Dim specie, sep As String
        For Each dr In DT.Rows

            Dim dtSpecie = obj_R.Leggi_SportelloxSpecie(dr("Sementieri_Sportello_Configurazione_cod"), 0, 0, 0, 0, 0, "", objParametri_Server)

            specie = ""
            sep = ""
            For Each sp In dtSpecie.Rows

                specie &= sep & sp("Veg_Des") & " - " & sp("Grva_Des")
                If sp("Hybrid") = 1 Then
                    specie &= " (Hybrid)"
                End If
                sep = ", "

            Next

            Dim objSportello As New JObject

            objSportello("id_sportello") = CInt(dr("Sementieri_Sportello_Configurazione_cod"))
            objSportello("sportello") = dr("Sementieri_Sportello_Configurazione_des").ToString
            objSportello("specie") = specie
            objSportello("validitaInizio") = CDate(dr("Validita_Inizio"))
            objSportello("validitaFine") = CDate(dr("Validita_Fine"))

            elencoSportelli.Add(objSportello)

        Next

        Return elencoSportelli.ToString
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiSportello(ByVal id_sportello As Integer) As String

        Dim objSportello As New JObject

        If id_sportello = 0 Then

            objSportello("descrizione") = ""
            objSportello("validitaInizio") = DateTime.Now()
            objSportello("validitaFine") = DateTime.Now()
            objSportello("classi") = New JArray
            objSportello("passaggi") = New JArray

        Else

            Dim objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")

            Dim obj_R As New AgronicaCoreSementieriDAL.Sportello_R()
            Dim DT = obj_R.Leggi_Sementieri_Sportello_Configurazione(id_sportello,
                                                                 AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO,
                                                                 "",
                                                                 objParametri_Server)
            If DT.Rows.Count = 0 Then
                Return ""
            End If

            Dim arrClassi As New JArray
            Dim dtClassi = obj_R.LeggiClassiDiSpecieXSportello(id_sportello, objParametri_Server)
            For Each cl In dtClassi.Rows
                Dim objClasse = New JObject
                objClasse("IdSpecie") = CInt(cl("IdSpecie"))
                objClasse("DesSpecie") = cl("DesSpecie").ToString
                arrClassi.Add(objClasse)
            Next

            Dim arrPassaggi As New JArray
            Dim dtPassaggi = obj_R.Leggi_SportelloxPassaggi(id_sportello, 0, 0, "ORDER BY Data_Inizio", objParametri_Server)
            For Each pass In dtPassaggi.Rows
                Dim objPass As New JObject
                objPass("IdPassaggio") = CInt(pass("Sementieri_Sportello_Passaggi_cod"))
                objPass("DesPassaggio") = pass("Sementieri_Sportello_Passaggi_des").ToString
                objPass("DataInizio") = CDate(pass("Data_Inizio"))
                objPass("DataFine") = CDate(pass("Data_Fine"))
                objPass("VisImpianti") = CInt(pass("Visibilita_Impianti")) = 1
                objPass("OpPermesse") = CInt(pass("DestinazioneSalvataggio")) >= 0
                objPass("LogOperazioni") = CInt(pass("LoggaOperazioni")) = 1
                objPass("Notifiche") = CInt(pass("RichiediConfermaSuInterferenze")) = 1
                arrPassaggi.Add(objPass)
            Next

            Dim dr = DT.Rows(0)

            objSportello("descrizione") = dr("Sementieri_Sportello_Configurazione_des").ToString
            objSportello("validitaInizio") = CDate(dr("Validita_Inizio"))
            objSportello("validitaFine") = CDate(dr("Validita_Fine"))
            objSportello("classi") = arrClassi
            objSportello("passaggi") = arrPassaggi

        End If

        Return objSportello.ToString

    End Function

    Private Structure Passaggio
        Public Id As Integer
        Public DataInizio As DateTime
        Public DataFine As DateTime
    End Structure
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function SalvaSportello(ByVal id_sportello As Integer, ByVal descrizione As String, ByVal dataInizio As String, ByVal dataFine As String, ByVal specie As String, ByVal passaggi As String) As String

        Dim result As New JObject

        Dim dtInizio As DateTime = dataInizio
        Dim dtFine As DateTime = dataFine

        If String.IsNullOrEmpty(specie) Then

            result("result") = False
            result("error") = "Elenco specie vegetali associate vuoto..."

            Return result.ToString()
        End If

        Dim id_specie As List(Of Integer) = specie.Split("|").ToList().ConvertAll(Function(str) Int32.Parse(str))
        Dim arrPassaggi = JArray.Parse(passaggi)
        Dim listPassaggi As New List(Of Passaggio)
        Dim objChecker As New JObject

        Dim isDupl As Boolean = False
        Dim errDate As Integer = 0

        For Each obj In arrPassaggi

            Dim pass As New Passaggio

            If errDate = 0 AndAlso Not isDupl Then

                Try

                    pass.DataInizio = CDate(obj("DataInizio")).ToLocalTime

                    Try
                        pass.DataFine = CDate(obj("DataFine")).ToLocalTime

                        If pass.DataFine < pass.DataInizio Then

                            errDate = 3

                        Else

                            pass.Id = CInt(obj("IdPassaggio"))

                            If objChecker("ID_" & pass.Id.ToString) IsNot Nothing Then
                                isDupl = True
                            Else
                                objChecker("ID_" & pass.Id.ToString) = True
                                listPassaggi.Add(pass)
                            End If

                        End If

                    Catch ex As Exception

                        errDate = 2

                    End Try

                Catch ex As Exception

                    errDate = 1

                End Try

            End If

        Next

        If isDupl Then

            result("result") = False
            result("error") = "Fase sportello duplicata non ammessa."

            Return result.ToString()
        End If

        If errDate <> 0 Then
            Dim errmsg As String = "Fase sportello con <b>Data inizio</b> maggiore di <b>Data fine</b> non permessa."
            If errDate = 1 Then
                errmsg = "Fase sportello con <b>Data inizio</b> non valida."
            ElseIf errDate = 2 Then
                errmsg = "Fase sportello con <b>Data fine</b> non valida."
            End If

            result("result") = False
            result("error") = errmsg

            Return result.ToString()
        End If

        'Controllare che non ci siano sovrapposizioni nei periodi di validità dei passaggi
        Dim sovrapposizione As Boolean = False
        Dim p0 As Integer = 0
        While p0 < listPassaggi.Count AndAlso Not sovrapposizione

            Dim pass0 = listPassaggi(p0)
            Dim p1 As Integer = p0 + 1

            While p1 < listPassaggi.Count AndAlso Not sovrapposizione

                Dim pass1 = listPassaggi(p1)

                sovrapposizione = pass0.DataFine >= pass1.DataInizio And pass1.DataFine >= pass0.DataInizio

                p1 += 1
            End While

            p0 += 1
        End While

        If sovrapposizione Then

            result("result") = False
            result("error") = "Ci sono due fasi che si sovrappongono. Controllare le date di inizio/fine."

            Return result.ToString()
        End If

        Dim objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objSportello_W As New AgronicaCoreSementieriDAL.Sportello_W

        If id_sportello = 0 Then

            'nuovo
            Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

            Dim obj As New AgronicaCoreUtentiDAL.Utenti_CodiciGiasPRO_R
            Dim dt As DataTable = obj.Leggi("", "", objParametri_Utenti)
            Dim progressivo As Integer = dt.Rows(0).Item("progressivogias")

            'ricavo base e top 
            Dim objSeq As New AgronicaCoreDataProvider.Agro_Sequenze

            Dim basecode, topcode As Integer
            objSeq.Calcola_BaseCode(progressivo, topcode, basecode, objParametri_Utenti)

            id_sportello = objSeq.NuovoId_Tabella("Sementieri_Sportello_Configurazione", basecode, topcode, objParametri_Server)

            objSportello_W.Scrivi_Sementieri_Sportello_Configurazione(id_sportello, descrizione, dtInizio, dtFine, objParametri_Server)

        Else

            'modifica... cancello le tre tabelle e poi le riscrivo

            objSportello_W.Cancella_Sementieri_Sportello_ConfigurazioneXmappatura_specie(id_sportello, 0, 0, 0, 0, objParametri_Server)
            objSportello_W.Cancella_Sementieri_Sportello_ConfigurazioneXpassaggi(id_sportello, 0, 0, objParametri_Server)
            'objSportello_W.Cancella_Sementieri_Sportello_Configurazione(id_sportello, objParametri_Server)
            objSportello_W.MODIFICA_Sementieri_Sportello_Configurazione(id_sportello, descrizione, dtInizio, dtFine, objParametri_Server)

        End If

        For Each ids In id_specie

            objSportello_W.Scrivi_Sementieri_Sportello_ConfigurazioneXMappatura_Specie_(id_sportello, ids, dtInizio, dtFine, objParametri_Server)

        Next

        listPassaggi.Sort(Function(pass1 As Passaggio, pass2 As Passaggio)
                              If pass1.DataInizio < pass2.DataInizio Then
                                  Return -1
                              ElseIf pass1.DataInizio > pass2.DataInizio Then
                                  Return 1
                              End If
                              Return 0
                          End Function)

        Dim ordine As Integer = 1
        For Each pass In listPassaggi

            objSportello_W.Scrivi_Sementieri_Sportello_ConfigurazioneXpassaggi(id_sportello, pass.Id, ordine, pass.DataInizio.ToString, pass.DataFine.ToString, objParametri_Server)

            ordine += 1
        Next

        result("result") = True
        result("error") = ""
        result("id") = id_sportello

        Return result.ToString()
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function EliminaSportello(ByVal id_sportello As Integer) As String

        Dim objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim result As New JObject

        Dim objSportello_R As New AgronicaCoreSementieriDAL.Sportello_R
        If Not objSportello_R.PossoCancellare(id_sportello, objParametri_Server) Then

            result("result") = False
            result("msg") = "Esistono operazioni collegate allo sportello. Eliminazione non permessa."

            Return result.ToString
        End If

        Dim objSportello_W As New AgronicaCoreSementieriDAL.Sportello_W

        objSportello_W.Cancella_Sementieri_Sportello_ConfigurazioneXpassaggi(id_sportello, 0, 0, objParametri_Server)
        objSportello_W.Cancella_Sementieri_Sportello_ConfigurazioneXmappatura_specie(id_sportello, 0, 0, 0, 0, objParametri_Server)
        objSportello_W.Cancella_Sementieri_Sportello_Configurazione(id_sportello, objParametri_Server)

        result("result") = True
        result("msg") = ""

        Return result.ToString
    End Function


End Class