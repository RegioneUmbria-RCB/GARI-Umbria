Imports System.Web
Imports System.Web.Services

Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreUtility
Imports AgronicaCoreUtility.CaricaListControl

Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreUtentiDAL
Imports System.Xml
Imports AgronicaCoreVarieBIZ
Imports AgroAgenda_2010.Resources
Imports AgronicaCoreGestioneRichieste
Imports Newtonsoft.Json

Partial Class Fabbricato_Edit
    Inherits System.Web.UI.Page


    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Dim objParametriAgenda As ParametriAgenda

    Public permessi As PermessiUtente
    Public Operazione As Integer

    Dim xPiva As String
    Dim xSa_Cod As String
    Dim xFabbricato_Cod As String
    Dim xIndirizzoCod As Integer
    Dim Qs_Visibilita As Integer = 0

    Public jsCodici As String
    Public jsStalle As String

    Public jsStalleConfigurazioniBDN As String

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()

    End Sub

    '########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Riempi_Cmb_IndirizzoProduttivo(ByVal Gen_Cod As String, ByVal Spe_Cod As String) As String

        Dim cmb_ind_prod As New DropDownList

        'AgronicaCoreUtility.CaricaListControl.ImpreseConFiltroUtente(cmb_ind_prod, True, "SELEZIONA", "", _
        '                                                                 " Imprese.rag_soc LIKE '%" & codice & "%'", " ORDER BY Rag_Soc asc", _
        '                                                                 HttpContext.Current.Session("ASG_objParametri_Server"), _
        '                                                                 HttpContext.Current.Session("ASG_objParametri_Utenti"))

        Lingua.Gias_InizializzaCultura_DaSession()

        AgronicaCoreUtility.CaricaListControl.Lista_IndirizziProd_Animali(cmb_ind_prod,
                        True,
                        AgronicaAgenda_2010.Seleziona.ToUpper(),
                        "",
                        Gen_Cod,
                        Spe_Cod,
                        "",
                        "",
                        HttpContext.Current.Session("ASG_objParametri_Server")
                        )

        Dim rval As String = ""
        For Each itm As ListItem In cmb_ind_prod.Items
            rval &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
        Next


        Return rval

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Set_Comune(ByVal comune As String)
        Dim Arrayp() As String
        Dim Com As String = ""

        HttpContext.Current.Session("comune_settato") = comune
        'HttpContext.Current.Session("nome_comune_settato") = nome_comune

        If Trim(comune) <> "" Then
            Arrayp = Split(comune, "|")
            Com = Arrayp(1)
        End If
        HttpContext.Current.Session("com") = Com

    End Function


    '########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Riempi_Cmb_Idoneita() As String

        Dim rval As String = ""

        Lingua.Gias_InizializzaCultura_DaSession()

        Dim XML_Fabbricato As System.Xml.XmlElement


        XML_Fabbricato = HttpContext.Current.Session("XML_Fabbricato")

        If Not IsNothing(XML_Fabbricato) Then

            If XML_Fabbricato.GetAttribute("idoneo_costruzione") = "0" Then
                rval &= "<option value=""1"">" & DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Fabbricato_Edit.aspx", "CaratteristicheDiCostruzione"), String) & "</option>"
            Else
                rval &= "<option value=""1"" selected=""selected"">" & DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Fabbricato_Edit.aspx", "CaratteristicheDiCostruzione"), String) & "</option>"
            End If

            If XML_Fabbricato.GetAttribute("idoneo_separazambienti") = "0" Then
                rval &= "<option value=""2"">" & DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Fabbricato_Edit.aspx", "SeparazioneAmbienti"), String) & "</option>"
            Else
                rval &= "<option value=""2"" selected=""selected"">" & DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Fabbricato_Edit.aspx", "SeparazioneAmbienti"), String) & "</option>"
            End If

            If XML_Fabbricato.GetAttribute("idoneo_separazprodotti") = "0" Then
                rval &= "<option value=""3"">" & DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Fabbricato_Edit.aspx", "SeparazioneProdotti"), String) & "</option>"
            Else
                rval &= "<option value=""3"" selected=""selected"">" & DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Fabbricato_Edit.aspx", "SeparazioneProdotti"), String) & "</option>"
            End If

            If XML_Fabbricato.GetAttribute("idoneo_condigieniche") = "0" Then
                rval &= "<option value=""4"">" & DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Fabbricato_Edit.aspx", "CondizioniIgienicoSanitarie"), String) & "</option>"
            Else
                rval &= "<option value=""4"" selected=""selected"">" & DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Fabbricato_Edit.aspx", "CondizioniIgienicoSanitarie"), String) & "</option>"
            End If

            If XML_Fabbricato.GetAttribute("idoneo_autorizsanitaria") = "0" Then
                rval &= "<option value=""5"">" & DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Fabbricato_Edit.aspx", "AutorizzazioneSanitaria"), String) & "</option>"
            Else
                rval &= "<option value=""5"" selected=""selected"">" & DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Fabbricato_Edit.aspx", "AutorizzazioneSanitaria"), String) & "</option>"
            End If

            If XML_Fabbricato.GetAttribute("idoneo_haccp") = "0" Then
                rval &= "<option value=""6"">" & "HACCP" & "</option>"
            Else
                rval &= "<option value=""6"" selected=""selected"">" & "HACCP" & "</option>"
            End If

            If XML_Fabbricato.GetAttribute("idoneo_planimetria") = "0" Then
                rval &= "<option value=""7"">" & DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Fabbricato_Edit.aspx", "Planimetria"), String) & "</option>"
            Else
                rval &= "<option value=""7"" selected=""selected"">" & DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Fabbricato_Edit.aspx", "Planimetria"), String) & "</option>"
            End If

            If XML_Fabbricato.GetAttribute("idoneo_layout") = "0" Then
                rval &= "<option value=""8"">" & "Layout" & "</option>"
            Else
                rval &= "<option value=""8"" selected=""selected"">" & "Layout" & "</option>"
            End If

            If XML_Fabbricato.GetAttribute("idoneo_diagrammiflusso") = "0" Then
                rval &= "<option value=""9"">" & DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Fabbricato_Edit.aspx", "DiagrammiDiFlusso"), String) & "</option>"
            Else
                rval &= "<option value=""9"" selected=""selected"">" & DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Fabbricato_Edit.aspx", "DiagrammiDiFlusso"), String) & "</option>"
            End If

            If XML_Fabbricato.GetAttribute("idoneo_cdx_m004") = "0" Then
                rval &= "<option value=""10"">" & "CDX-M004" & "</option>"
            Else
                rval &= "<option value=""10"" selected=""selected"">" & "CDX-M004" & "</option>"
            End If

            If XML_Fabbricato.GetAttribute("idoneo_supmincoperte") = "0" Then
                rval &= "<option value=""11"">" & DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Fabbricato_Edit.aspx", "SuperficiMinimeCoperte"), String) & "</option>"
            Else
                rval &= "<option value=""11"" selected=""selected"">" & DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Fabbricato_Edit.aspx", "SuperficiMinimeCoperte"), String) & "</option>"
            End If

            If XML_Fabbricato.GetAttribute("idoneo_supminscoperte") = "0" Then
                rval &= "<option value=""12"">" & DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Fabbricato_Edit.aspx", "SuperficiMinimeScoperte"), String) & "</option>"
            Else
                rval &= "<option value=""12"" selected=""selected"">" & DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Fabbricato_Edit.aspx", "SuperficiMinimeScoperte"), String) & "</option>"
            End If

        Else
            rval &= "<option value=""1"">" & DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Fabbricato_Edit.aspx", "CaratteristicheDiCostruzione"), String) & "</option>"

            rval &= "<option value=""2"">" & DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Fabbricato_Edit.aspx", "SeparazioneAmbienti"), String) & "</option>"

            rval &= "<option value=""3"">" & DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Fabbricato_Edit.aspx", "SeparazioneProdotti"), String) & "</option>"

            rval &= "<option value=""4"">" & DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Fabbricato_Edit.aspx", "CondizioniIgienicoSanitarie"), String) & "</option>"

            rval &= "<option value=""5"">" & DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Fabbricato_Edit.aspx", "AutorizzazioneSanitaria"), String) & "</option>"

            rval &= "<option value=""6"">" & "HACCP" & "</option>"

            rval &= "<option value=""7"">" & DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Fabbricato_Edit.aspx", "Planimetria"), String) & "</option>"

            rval &= "<option value=""8"">" & "Layout" & "</option>"

            rval &= "<option value=""9"">" & DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Fabbricato_Edit.aspx", "DiagrammiDiFlusso"), String) & "</option>"

            rval &= "<option value=""10"">" & "CDX-M004" & "</option>"

            rval &= "<option value=""11"">" & DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Fabbricato_Edit.aspx", "SuperficiMinimeCoperte"), String) & "</option>"

            rval &= "<option value=""12"">" & DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Fabbricato_Edit.aspx", "SuperficiMinimeScoperte"), String) & "</option>"


        End If


        Return rval

    End Function




    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Province(ByVal stato As String) As Array

        Dim dll_Provincia As New DropDownList
        Dim rval(0) As String
        Dim options As String = ""


        AgronicaCoreUtility.CaricaListControl.Provincie(dll_Provincia,
                                                        True, "", "",
                                                        False, 1, "", "", "", "", "", "", "", HttpContext.Current.Session("ASG_objParametri_Server"),
                                                        stato)


        For Each itm As ListItem In dll_Provincia.Items
            options &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
        Next

        rval(0) = options

        Return rval

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Comuni(ByVal provincia As String) As Array


        Dim cmb_comuni2 As New DropDownList
        Dim rval(1) As String
        Dim options As String = ""

        If provincia <> "" Then

            AgronicaCoreUtility.CaricaListControl.Comuni(cmb_comuni2,
                                                             True, "", "",
                                                            provincia, False, 1, "", "", HttpContext.Current.Session("ASG_objParametri_Server"))


            For Each itm As ListItem In cmb_comuni2.Items
                options &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
            Next

            rval(0) = options

            Dim objI As New AgronicaCoreMetaSchemaDAL.Istat_R
            'Imposto la textbox del CODICE ISTAT
            rval(1) = objI.CodIstat_from_Provincia(provincia, HttpContext.Current.Session("ASG_objParametri_Server"))


        End If

        Return rval

    End Function


    '########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Riempi_Cmb_TipoRicovero(ByVal Gen_Cod As String, ByVal Spe_Cod As String, ByVal IPro_Cod As String) As String

        Dim cmb_ind_prod As New DropDownList

        'AgronicaCoreUtility.CaricaListControl.ImpreseConFiltroUtente(cmb_ind_prod, True, "SELEZIONA", "", _
        '                                                                 " Imprese.rag_soc LIKE '%" & codice & "%'", " ORDER BY Rag_Soc asc", _
        '                                                                 HttpContext.Current.Session("ASG_objParametri_Server"), _
        '                                                                 HttpContext.Current.Session("ASG_objParametri_Utenti"))
        Lingua.Gias_InizializzaCultura_DaSession()

        AgronicaCoreUtility.CaricaListControl.Lista_Tipi_Stalla(cmb_ind_prod,
                        True,
                        AgronicaAgenda_2010.Seleziona.ToUpper(),
                        "",
                        Gen_Cod,
                        Spe_Cod,
                        IPro_Cod,
                        "",
                        "",
                        HttpContext.Current.Session("ASG_objParametri_Server")
                        )

        Dim rval As String = ""
        For Each itm As ListItem In cmb_ind_prod.Items
            rval &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
        Next


        Return rval

    End Function


    '########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Riempi_Cmb_Sottotipi_Stalla(ByVal Gen_Cod As String, ByVal Spe_Cod As String, ByVal IPro_Cod As String, ByVal Cod_Fabb As String) As String

        Dim cmb_ind_sottotipi As New DropDownList

        'CaricaCombo_Lista_Sottotipi_Stalla(Me.Cmb_SottotipoRicovero, _
        '                                   Gen_Cod, _
        '                                   Spe_Cod, _
        '                                   IPro_Cod, _
        '                                   Cod_Fabb, _
        '                                   objParametri_Server)

        Lingua.Gias_InizializzaCultura_DaSession()

        AgronicaCoreUtility.CaricaListControl.Lista_Sottotipi_Stalla(cmb_ind_sottotipi,
                        True,
                        AgronicaAgenda_2010.Seleziona.ToUpper(),
                        "",
                        Gen_Cod,
                        Spe_Cod,
                        IPro_Cod,
                        Cod_Fabb,
                        "",
                        "",
                        HttpContext.Current.Session("ASG_objParametri_Server")
                        )

        Dim rval As String = ""
        For Each itm As ListItem In cmb_ind_sottotipi.Items
            rval &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
        Next


        Return rval

    End Function


    '########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Riempi_Cmb_Caratteristiche(ByVal Cod_Fabb As String) As String

        Dim cmb_caratt As New DropDownList

        'AgronicaCoreUtility.CaricaListControl.ImpreseConFiltroUtente(cmb_ind_prod, True, "SELEZIONA", "", _
        '                                                                 " Imprese.rag_soc LIKE '%" & codice & "%'", " ORDER BY Rag_Soc asc", _
        '                                                                 HttpContext.Current.Session("ASG_objParametri_Server"), _
        '                                                                 HttpContext.Current.Session("ASG_objParametri_Utenti"))

        Lingua.Gias_InizializzaCultura_DaSession()

        AgronicaCoreUtility.CaricaListControl.Stalle_Attributi(cmb_caratt,
                        True,
                        AgronicaAgenda_2010.Seleziona.ToUpper(),
                        "",
                        Cod_Fabb,
                        "",
                        "",
                        HttpContext.Current.Session("ASG_objParametri_Server")
                        )

        Dim rval As String = ""
        For Each itm As ListItem In cmb_caratt.Items
            rval &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
        Next


        Return rval

    End Function


    '########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Riempi_Cmb_SottotipoRicovero(ByVal Gen_Cod As String, ByVal Spe_Cod As String, ByVal IPro_Cod As String, ByVal Cod_Fabb As String) As String

        Dim cmb_ind_prod As New DropDownList

        'AgronicaCoreUtility.CaricaListControl.ImpreseConFiltroUtente(cmb_ind_prod, True, "SELEZIONA", "", _
        '                                                                 " Imprese.rag_soc LIKE '%" & codice & "%'", " ORDER BY Rag_Soc asc", _
        '                                                                 HttpContext.Current.Session("ASG_objParametri_Server"), _
        '                                                                 HttpContext.Current.Session("ASG_objParametri_Utenti"))

        Lingua.Gias_InizializzaCultura_DaSession()

        AgronicaCoreUtility.CaricaListControl.Lista_Sottotipi_Stalla(cmb_ind_prod,
                        True,
                        AgronicaAgenda_2010.Seleziona.ToUpper(),
                        "",
                        Gen_Cod,
                        Spe_Cod,
                        IPro_Cod,
                        Cod_Fabb,
                        "",
                        "",
                        HttpContext.Current.Session("ASG_objParametri_Server")
                        )

        Dim rval As String = ""
        For Each itm As ListItem In cmb_ind_prod.Items
            rval &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
        Next


        Return rval

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_Cmb_TipoRicovero(ByVal valore As String)

        HttpContext.Current.Session("tipo_ricorvero") = valore

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Aggiungi_Codice(ByVal codice As String, ByVal valore As String, ByVal codice_id As String, ByVal DataInizio As String, ByVal DataFine As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim Dt As New DataTable
        Dim Dr As DataRow
        'Dim Contatore As Integer
        Dim flag As Boolean = True

        Lingua.Gias_InizializzaCultura_DaSession()

        Dt = HttpContext.Current.Session("dt_Codici")

        If Not IsDate(DataInizio) Then
            DataInizio = AGRODATAINIZIO
        Else
            DataInizio = CDate(DataInizio)
        End If

        If Not IsDate(DataFine) Then
            DataFine = AGRODATAFINE
        Else
            DataFine = CDate(DataFine)
        End If


        '----- Cerco il valore massimo del contatore

        'Contatore = 0

        'For i = 0 To Dt.Rows.Count - 1
        '    If Dt.Rows(i).Item("Contatore") > Contatore Then
        '        Contatore = Dt.Rows(i).Item("Contatore")
        '    End If
        'Next

        'Contatore = Contatore + 1

        '----- Inserisco la riga nel datagrid

        'Creo una nuova riga
        Dr = Dt.NewRow

        'Definisco i valori
        'Dr.Item("Contatore") = Contatore

        Dr.Item("Id_Cod") = codice_id
        Dr.Item("Descrizione") = codice
        Dr.Item("Val_Cod") = valore

        If DataInizio = #1/1/1900# Then
            'Dr.Item("Validita_Inizio") = "..."
        Else
            ' Dr.Item("Validita_Inizio") = DataInizio.ToShortDateString
            Dr.Item("Validita_Inizio") = DataInizio
        End If

        If DataFine = #12/31/2100# Then
            'Dr.Item("Validita_Fine") = "..."
        Else
            ' Dr.Item("Validita_Fine") = DataFine.ToShortDateString
            Dr.Item("Validita_Fine") = DataFine
        End If


        'Controllo se esiste già la voce che si vuole inserire
        For i = 0 To Dt.Rows.Count - 1

            If (Dt.Rows(i).Item("Id_Cod") = codice_id) Then
                flag = False
            End If
        Next

        If (flag) Then
            'Associo alla tabella la nuova riga creata
            Dt.Rows.Add(Dr)
            HttpContext.Current.Session("dt_Codici") = Dt
            Dim str_Risposta = DT_to_Json_Codici(Dt)
            r.RispostaOK = True
            r.RispostaStringa = str_Risposta
        Else
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.CodiceGiàInserito
        End If

        Return r

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Aggiungi_Stalla(ByVal codice As String, ByVal valore As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim Dt As New DataTable
        Dim Dr As DataRow
        'Dim Contatore As Integer
        Dim flag As Boolean = True

        Lingua.Gias_InizializzaCultura_DaSession()

        Dt = HttpContext.Current.Session("dt_Stalle_car")

        codice = codice.Replace("|"c, "."c)

        '----- Cerco il valore massimo del contatore

        'Contatore = 0

        'For i = 0 To Dt.Rows.Count - 1
        '    If Dt.Rows(i).Item("Contatore") > Contatore Then
        '        Contatore = Dt.Rows(i).Item("Contatore")
        '    End If
        'Next

        'Contatore = Contatore + 1

        '----- Inserisco la riga nel datagrid

        'Creo una nuova riga
        Dr = Dt.NewRow

        'Definisco i valori
        'Dr.Item("Contatore") = Contatore

        Dr.Item("ATT_COD") = CInt(codice)
        'Dr.Item("Descrizione") = codice
        Dr.Item("VALORE") = valore


        'Controllo se esiste già la voce che si vuole inserire
        For i = 0 To Dt.Rows.Count - 1

            If (Dt.Rows(i).Item("ATT_COD") = codice) Then
                flag = False
            End If
        Next

        If (flag) Then
            'Associo alla tabella la nuova riga creata
            Dt.Rows.Add(Dr)
            HttpContext.Current.Session("dt_Stalle_car") = Dt
            Dim str_Risposta = DT_to_Json_Stalle(Dt)
            r.RispostaOK = True
            r.RispostaStringa = str_Risposta
        Else
            r.RispostaOK = False
            r.Errore = "Caratteristica Stalla già inserita"
        End If

        Return r

    End Function


    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Aggiungi_Configurazione_BDN(ByVal str As String) As RispostaStandard

        Dim r As New RispostaStandard


        Try

            Dim dt = JsonConvert.DeserializeObject(Of DataTable)(str, New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local})

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 AndAlso dt.Columns.Contains("CF_Proprietario") Then

                Dim dt_proprietari = dt.DefaultView.ToTable(True, "CF_Proprietario")

                For Each rProprietario In dt_proprietari.Rows
                    Dim proprietario As String = rProprietario(0)
                    Dim dt_conf_prop = dt.Select(" CF_Proprietario = '" & proprietario & "' ").CopyToDataTable

                    For i As Integer = 0 To dt_conf_prop.Rows.Count - 1
                        Dim iValidita_Inizio As Date = dt_conf_prop.Rows(i)("Validita_Inizio")
                        Dim iValidita_Fine As Date = dt_conf_prop.Rows(i)("Validita_Fine")
                        For j As Integer = 0 To dt_conf_prop.Rows.Count - 1

                            Dim jValidita_Inizio As Date = dt_conf_prop.Rows(j)("Validita_Inizio")
                            Dim jValidita_Fine As Date = dt_conf_prop.Rows(j)("Validita_Fine")

                            If i <> j Then

                                If (iValidita_Inizio >= jValidita_Inizio AndAlso iValidita_Inizio <= jValidita_Fine) OrElse
                                    (iValidita_Fine <= jValidita_Inizio AndAlso iValidita_Fine >= jValidita_Fine) Then
                                    Throw New Exception(" Le date di validità del C.F. Proprietario " & proprietario & " si sovrappongono ")
                                End If
                            Else
                                If (jValidita_Inizio > jValidita_Fine) Then
                                    Throw New Exception(" La data di inizio supera la data di fine per il C.F. Proprietario: " & proprietario & " ")
                                End If
                            End If

                        Next
                    Next



                Next

            End If

            HttpContext.Current.Session("dt_Stalle_conf_BDN_car") = dt
            r.RispostaOK = True
            r.RispostaStringa = ""

        Catch ex As Exception

            r.RispostaOK = False
            r.RispostaStringa = ex.Message
            r.Errore = ex.Message

        End Try

        Return r

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function DT_to_Json_Codici(ByVal dt As DataTable) As String

        Dim objParametriAgenda As Integer
        objParametriAgenda = HttpContext.Current.Session("operazione")

        Lingua.Gias_InizializzaCultura_DaSession()

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)

        'aggiungo i pulsanti per modifica ed eliminazione
        Dim c = New ColonneNome("Id_Cod", "Tool", "string")

        Dim listaBtn = New List(Of btnAzioni)

        If objParametriAgenda <> enum_TipoOperazioneDB.Lettura Then
            listaBtn.Add(New btnAzioni("fa-pencil-square-o edit_elem", "ModificaCodice(this);"))
            listaBtn.Add(New btnAzioni("fa-trash-o del_elem", "EliminaCodice(this);"))

        End If

        Dim tool As New ToolStandard(listaBtn)
        tool.cssColonna = "colmodmovimenti"
        c._FormatoParticolare = tool.toString()
        c._Filtrabile = False
        c._ColonnaDiSelezione = True
        l.Add(c)

        'Dim cn As New ColonneNome("Contatore", "Contatore", "string")
        'l.Add(cn)

        'Dim cn As New ColonneNome("Id_Cod", "Id_Cod", "string")
        'cn._hidden = True
        'l.Add(cn)

        Dim cn As New ColonneNome("descrizione", AgronicaAgenda_2010.Codice, "string")
        l.Add(cn)

        cn = New ColonneNome("Val_Cod", AgronicaAgenda_2010.Valore, "string")
        l.Add(cn)

        cn = New ColonneNome("Validita_Inizio", AgronicaAgenda_2010.Dal, "string")
        l.Add(cn)

        cn = New ColonneNome("Validita_Fine", AgronicaAgenda_2010.Al, "string")
        l.Add(cn)



        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable(dt, l)
        Return risp
    End Function


    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function DT_to_Json_Stalle(ByVal dt As DataTable) As String

        Dim objParametriAgenda As Integer
        objParametriAgenda = HttpContext.Current.Session("operazione")

        Lingua.Gias_InizializzaCultura_DaSession()

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)

        'aggiungo i pulsanti per modifica ed eliminazione
        Dim c = New ColonneNome("ATT_COD", "Tool", "string")

        Dim listaBtn = New List(Of btnAzioni)

        If objParametriAgenda <> enum_TipoOperazioneDB.Lettura Then
            'listaBtn.Add(New btnAzioni("fa-pencil-square-o edit_elem", "ModificaCodice(this);"))
            listaBtn.Add(New btnAzioni("fa-trash-o del_elem", "EliminaCodice(this);"))
        End If

        Dim tool As New ToolStandard(listaBtn)
        tool.cssColonna = "colmodmovimenti"
        c._FormatoParticolare = tool.toString()
        c._Filtrabile = False
        c._ColonnaDiSelezione = True
        l.Add(c)

        'Dim cn As New ColonneNome("Contatore", "Contatore", "string")
        'l.Add(cn)

        'Dim cn As New ColonneNome("Id_Cod", "Id_Cod", "string")
        'cn._hidden = True
        'l.Add(cn)

        Dim cn As New ColonneNome("ATT_COD", AgronicaAgenda_2010.Codice, "string")
        l.Add(cn)

        cn = New ColonneNome("VALORE", AgronicaAgenda_2010.Valore, "string")
        l.Add(cn)




        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable(dt, l)
        Return risp
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function DT_to_Json_Stalle_Conf_BDN(ByVal dt As DataTable) As String

        'Dim objParametriAgenda As Integer
        'objParametriAgenda = HttpContext.Current.Session("operazione")

        'Lingua.Gias_InizializzaCultura_DaSession()

        ''creo la lista delle colonne da visualizzare
        'Dim l As New List(Of ColonneNome)

        ''aggiungo i pulsanti per modifica ed eliminazione
        'Dim c = New ColonneNome("ATT_COD", "Tool", "string")

        'Dim listaBtn = New List(Of btnAzioni)

        'If objParametriAgenda <> enum_TipoOperazioneDB.Lettura Then
        '    'listaBtn.Add(New btnAzioni("fa-pencil-square-o edit_elem", "ModificaCodice(this);"))
        '    listaBtn.Add(New btnAzioni("fa-trash-o del_elem", "EliminaCodice(this);"))
        'End If

        'Dim tool As New ToolStandard(listaBtn)
        'tool.cssColonna = "colmodmovimenti"
        'c._FormatoParticolare = tool.toString()
        'c._Filtrabile = False
        'c._ColonnaDiSelezione = True
        'l.Add(c)

        ''Dim cn As New ColonneNome("Contatore", "Contatore", "string")
        ''l.Add(cn)

        ''Dim cn As New ColonneNome("Id_Cod", "Id_Cod", "string")
        ''cn._hidden = True
        ''l.Add(cn)

        'Dim cn As New ColonneNome("ATT_COD", AgronicaAgenda_2010.Codice, "string")
        'l.Add(cn)

        'cn = New ColonneNome("VALORE", AgronicaAgenda_2010.Valore, "string")
        'l.Add(cn)




        ''ritorno la tabella trasformata in json (aggiustando anche le colonne)
        'Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = JsonConvert.SerializeObject(dt)
        Return risp
    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_cbl_idoneita(ByVal valori As String)

        ' salvo i valori OTE in session (x salvataggio)
        'valori = valori.Replace("-"c, "|"c)
        HttpContext.Current.Session("idoneita") = valori

    End Function

    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Ripristina_cbl_idoneita()

        Dim rval As String = ""

        Lingua.Gias_InizializzaCultura_DaSession()

        If HttpContext.Current.Session("idoneita") <> Nothing Then
            Dim valori As String = HttpContext.Current.Session("idoneita")
            Dim idoneita_val As String() = valori.Split(New Char() {"-"c})


            'For Each itm As ListItem In cmb_ind_prod.Items
            '    rval &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
            'Next
            If idoneita_val(0) <> "" Then
                rval &= "<option value=""1"" selected=""selected"">" & DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Fabbricato_Edit.aspx", "CaratteristicheDiCostruzione"), String) & "</option>"
            Else
                rval &= "<option value=""1"">" & DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Fabbricato_Edit.aspx", "CaratteristicheDiCostruzione"), String) & "</option>"
            End If

            If idoneita_val(1) <> "" Then
                rval &= "<option value=""2"" selected=""selected"">" & DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Fabbricato_Edit.aspx", "SeparazioneAmbienti"), String) & "</option>"
            Else
                rval &= "<option value=""2"">" & DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Fabbricato_Edit.aspx", "SeparazioneAmbienti"), String) & "</option>"
            End If

            If idoneita_val(2) <> "" Then
                rval &= "<option value=""3"" selected=""selected"">" & DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Fabbricato_Edit.aspx", "SeparazioneProdotti"), String) & "</option>"
            Else
                rval &= "<option value=""3"">" & DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Fabbricato_Edit.aspx", "SeparazioneProdotti"), String) & "</option>"
            End If

            If idoneita_val(3) <> "" Then
                rval &= "<option value=""4"" selected=""selected"">" & DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Fabbricato_Edit.aspx", "CondizioniIgienicoSanitarie"), String) & "</option>"
            Else
                rval &= "<option value=""4"">" & DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Fabbricato_Edit.aspx", "CondizioniIgienicoSanitarie"), String) & "</option>"
            End If

            If idoneita_val(4) <> "" Then
                rval &= "<option value=""5"" selected=""selected"">" & DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Fabbricato_Edit.aspx", "AutorizzazioneSanitaria"), String) & "</option>"
            Else
                rval &= "<option value=""5"">" & DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Fabbricato_Edit.aspx", "AutorizzazioneSanitaria"), String) & "</option>"
            End If


            If idoneita_val(5) <> "" Then
                rval &= "<option value=""6"" selected=""selected"">" & "HACCP" & "</option>"
            Else
                rval &= "<option value=""6"">" & "HACCP" & "</option>"
            End If

            If idoneita_val(6) <> "" Then
                rval &= "<option value=""7"" selected=""selected"">" & DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Fabbricato_Edit.aspx", "Planimetria"), String) & "</option>"
            Else
                rval &= "<option value=""7"">" & DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Fabbricato_Edit.aspx", "Planimetria"), String) & "</option>"
            End If

            If idoneita_val(7) <> "" Then
                rval &= "<option value=""8"" selected=""selected"">" & "Layout" & "</option>"
            Else
                rval &= "<option value=""8"">" & "Layout" & "</option>"
            End If

            If idoneita_val(8) <> "" Then
                rval &= "<option value=""9"" selected=""selected"">" & DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Fabbricato_Edit.aspx", "DiagrammiDiFlusso"), String) & "</option>"
            Else
                rval &= "<option value=""9"">" & DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Fabbricato_Edit.aspx", "DiagrammiDiFlusso"), String) & "</option>"
            End If

            If idoneita_val(9) <> "" Then
                rval &= "<option value=""10"" selected=""selected"">" & "CDX-M004" & "</option>"
            Else
                rval &= "<option value=""10"">" & "CDX-M004" & "</option>"
            End If

            If idoneita_val(10) <> "" Then
                rval &= "<option value=""11"" selected=""selected"">" & DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Fabbricato_Edit.aspx", "SuperficiMinimeCoperte"), String) & "</option>"
            Else
                rval &= "<option value=""11"">" & DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Fabbricato_Edit.aspx", "SuperficiMinimeCoperte"), String) & "</option>"
            End If

            If idoneita_val(11) <> "" Then
                rval &= "<option value=""12"" selected=""selected"">" & DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Fabbricato_Edit.aspx", "SuperficiMinimeScoperte"), String) & "</option>"
            Else
                rval &= "<option value=""12"">" & DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Fabbricato_Edit.aspx", "SuperficiMinimeScoperte"), String) & "</option>"
            End If

        End If

        Return rval

    End Function


    Private Sub Fabbricato_Edit_Init(sender As Object, e As System.EventArgs) Handles Me.Init

        ' VAnni: 2/10/2017: a seguito di AgroMaseterPage
        'AddHandler CType(Page.Master.FindControl("ImgBtn_AnnullaTutto"), ImageButton).Click, AddressOf Me.AnnullaTutto
        AddHandler CType(Me.Master, AgendaBootstrap).ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto
        Master.flag_MostraBtnIndietro = True
    End Sub



    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        permessi = New PermessiUtente()

        Master.flag_pag_Anagrafica = True
        '----- Verifico che l'utente sia autenticato
        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("~/Custom500.aspx")
        End If

        'inizializzazione oggetti objParametri_Utenti e objParametri_Server
        '---
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        'objParametri_Utenti = Session("ASG_objParametri_Utenti")
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        'objParametri_Server = Session("ASG_objParametri_Server")
        '---


        If Request.QueryString("visibilita") IsNot Nothing AndAlso IsNumeric(Request.QueryString("visibilita")) Then
            Qs_Visibilita = CInt(Request.QueryString("visibilita"))
        End If

        'If Qs_Visibilita = 0 Then
        '    Master.Master_versione = VERSIONE_MASTER_DEFAULT
        '    Master.Header_versione = VERSIONE_HEADER_DEFAULT
        'End If

        objParametriAgenda = New ParametriAgenda
        'Dim xChiave As String

        Select Case objParametriAgenda.Tipo_Operazione
            Case enum_TipoOperazioneDB.Scrittura
                Master.Lbl_Titolo.Text = DirectCast(GetLocalResourceObject("CreazioneNuovoFabbricato"), String)
                'objParametriAgenda.Sa_Cod = 0
            Case enum_TipoOperazioneDB.Lettura
                Master.Lbl_Titolo.Text = DirectCast(GetLocalResourceObject("LetturaFabbricato"), String)
            Case enum_TipoOperazioneDB.Modifica
                Master.Lbl_Titolo.Text = DirectCast(GetLocalResourceObject("ModificaFabbricato"), String)
        End Select

        Operazione = objParametriAgenda.Tipo_Operazione
        HttpContext.Current.Session("operazione") = Operazione

        '==================================
        '======= VERIFICA PERMESSI ========
        '==================================

        Dim UtenteAbilitato_Lettura As Boolean = False

        'Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        'UtenteAbilitato_Lettura = objPermessi.Controlla_Permessi_Utente( _
        '                            Session("ASG_Utente_Username"), _
        '                            Session("ASG_IdServizio"), _
        '                            enum_Security_Attivita.Anagrafica_ParticellaCatastale, _
        '                            enum_Security_Operazione.Lettura, _
        '                            Date.Now, _
        '                            "", _
        '                            objParametri_Utenti)

        'Session("UtenteAbilitato_Lettura") = UtenteAbilitato_Lettura

        Dim UtenteAbilitato_Modifica As Boolean = False
        'UtenteAbilitato_Modifica = objPermessi.Controlla_Permessi_Utente( _
        '                            Session("ASG_Utente_Username"), _
        '                            Session("ASG_IdServizio"), _
        '                            enum_Security_Attivita.Anagrafica_ParticellaCatastale, _
        '                            enum_Security_Operazione.Modifica, _
        '                            Date.Now, _
        '                            "", _
        '                            objParametri_Utenti)

        'Session("UtenteAbilitato_Modifica") = UtenteAbilitato_Modifica


        UtenteAbilitato_Lettura = permessi.getPermesso(enum_Security_Attivita.Anagrafica_Fabbricato).Lettura
        UtenteAbilitato_Modifica = permessi.getPermesso(enum_Security_Attivita.Anagrafica_Fabbricato).Scrittura


        Session("UtenteAbilitato_Modifica") = UtenteAbilitato_Modifica
        Session("UtenteAbilitato_Lettura") = UtenteAbilitato_Lettura


        If Not UtenteAbilitato_Modifica Then
            Response.Redirect("../MenuAnagrafica/Menubs_anagrafica.aspx")
            Exit Sub
        End If

        If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica AndAlso objParametriAgenda.Fabbricato = 0 Then
            Throw New Exception("objParametriAgenda.Fabbricato = 0")
        End If

        xPiva = objParametriAgenda.Piva
        xSa_Cod = objParametriAgenda.Sa_Cod
        If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Lettura OrElse
        objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica Then
            xFabbricato_Cod = objParametriAgenda.Fabbricato
        End If


        If IsPostBack Then
            Exit Sub
        End If

        RipristinaDatiNeiControlli()

        If Operazione = enum_TipoOperazioneDB.Lettura Then

            TxtDenominazione.Enabled = False
            Cmb_TipoFabbricato.Enabled = False
            Txt_Via.Enabled = False
            Txt_Frazione.Enabled = False
            Cmb_Provincia.Enabled = False
            Cmb_Comune.Enabled = False
            Txt_CAP.Enabled = False
            'Txt_Stato.Enabled = False
            cmb_Stato.Enabled = False
            Txt_Note.Enabled = False
            cbl_idoneita.Disabled = True
            Cmb_TitoloPossesso.Enabled = False
            Cmb_Particella.Enabled = False
            Cmb_Regolamenti.Enabled = False
            TxtValiditaInizio.Enabled = False
            TxtValiditaFine.Enabled = False
            TxtSup_Convenzionale.Enabled = False
            TxtSup_Biologico.Enabled = False
            CmbCodice.Enabled = False
            TxtCodiceValore.Enabled = False
            TxtValiditaInizioCodice.Enabled = False
            TxtValiditaFineCodice.Enabled = False
            TxtCodiceStallaBDN.Enabled = False
            TxtIdFiscaleStallaBDN.Enabled = False
            chk_MagazzinoFarmaci.Enabled = False

        End If

    End Sub

    ' Ripristina combo stato per codice e in seconda battuta per descrizione (per retrocompatibiltà)
    Private Sub Ripristina_Cmb_Stato(ByVal stato As String)
        Dim index = cmb_Stato.Items.IndexOf(cmb_Stato.Items.FindByValue(stato))
        If index = -1 Then
            stato = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(stato.ToLower())
            index = cmb_Stato.Items.IndexOf(cmb_Stato.Items.FindByText(stato))
        End If
        cmb_Stato.SelectedIndex = If(index = -1, cmb_Stato.Items.IndexOf(cmb_Stato.Items.FindByValue("IT")), index)
    End Sub

    Private Sub RipristinaDatiNeiControlli()

        Dim StringaXML As String

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XML_DatiFabbricati As System.Xml.XmlElement
        Dim XML_Fabbricato As System.Xml.XmlElement
        Dim XML_Indirizzo As System.Xml.XmlElement
        Dim XML_Stalla As System.Xml.XmlElement
        'Dim XMLs_Codici As XmlNodeList

        Dim DTCodici As DataTable

        Dim Valore As String

        Dim StrCodice As String
        Dim BaseCode As Integer
        Dim TopCode As Integer

        Dim DTCentri As New DataTable
        Dim Stato As String = "IT"

        'HttpContext.Current.Session("XML_Fabbricato") = ""

        ' Inserimento label riferimenti
        LblRiferimenti.Text = objParametriAgenda.Piva & " " & objParametriAgenda.Sa_Cod & " " & objParametriAgenda.Fabbricato

        ' Inserimento label Centro
        Dim objCentriR As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
        DTCentri = objCentriR.Leggi(CStr(xPiva),
                                    CInt(xSa_Cod),
                                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                                    "",
                                    "",
                                    objParametri_Server)



        LblCentro.Text = DTCentri.Rows(0).Item("Sa_Nome")

        lbl_centro_data_inizio.Text = DTCentri.Rows(0).Item("Validita_inizio")
        lbl_centro_data_fine.Text = DTCentri.Rows(0).Item("Validita_fine")




        Dim objFabbricati As New AgronicaCoreAnagrafeBIZ.Fabbricato_R

        If Operazione = enum_TipoOperazioneDB.Modifica OrElse Operazione = enum_TipoOperazioneDB.Lettura Then

            xFabbricato_Cod = objParametriAgenda.Fabbricato

            'Leggo la stringa XML dell'oggetto
            StringaXML = objFabbricati.Fabbricato_Leggi(
                                            CStr(xPiva),
                                            CInt(xSa_Cod),
                                            CInt(xFabbricato_Cod),
                                            False,
                                            objParametri_Server)
        Else

            xFabbricato_Cod = 0

        End If

        objFabbricati = Nothing




        ''' Riempimento STALLE '''
        ' Riempimento select Tipo animali
        AgronicaCoreUtility.CaricaListControl.Lista_Specie_Animali(
                Cmb_Animali,
                True,
                AgronicaAgenda_2010.Seleziona.ToUpper(),
                "",
                0,
                "",
                "",
                objParametri_Server
        )


        Dim tempValidita = objParametri_Server.FinestraTemporaleInizio
        objParametri_Server.FinestraTemporaleInizio = Date.Now
        AgronicaCoreUtility.CaricaListControl.Tipo_Fabbricato_Cod(Cmb_TipoFabbricato,
                                                           False, "", "",
                                                           "", "", objParametri_Server)
        objParametri_Server.FinestraTemporaleInizio = tempValidita

        ''Inizializzo la ComboBox dei Titoli di Possesso
        AgronicaCoreUtility.CaricaListControl.TitoloPossesso(CType(Cmb_TitoloPossesso, ListControl),
                                                             False, "", "",
                                                             "", "", objParametri_Server)

        ''Inizializzo la ConboBox delle particelle catastali del Centro selezionato
        AgronicaCoreUtility.CaricaListControl.ParticelleCatastali(CType(Cmb_Particella, ListControl),
                                                             False, "", "",
                                                             xPiva, xSa_Cod, objParametri_Server)


        ''Inizializzo la ComboBox dei Regolamenti
        'Call CaricaCombo_Regolamento(Cmb_Regolamenti, objParametri_Server)
        AgronicaCoreUtility.CaricaListControl.Regolamento(CType(Cmb_Regolamenti, ListControl),
                                                             False, "", "",
                                                             "", "", objParametri_Server)


        ' Riempio la tendina delle nazioni
        Dim DT_Nazioni As DataTable
        Dim objNazioni As New AgronicaCoreMetaSchemaDAL.ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166_R
        DT_Nazioni = objNazioni.Leggi("", "", "Descrizione", objParametri_Server)

        For i = 0 To DT_Nazioni.Rows.Count - 1
            cmb_Stato.Items.Add(New ListItem(DT_Nazioni.Rows(i).Item("Descrizione"), DT_Nazioni.Rows(i).Item("Codice")))
            cmb_Stato.Items(i).Attributes.Add("Gestione_Gerarchia_Geografica", DT_Nazioni.Rows(i).Item("Gestione_Gerarchia_Geografica"))
        Next

        ' setto Italia come default
        cmb_Stato.SelectedIndex = cmb_Stato.Items.IndexOf(cmb_Stato.Items.FindByValue("IT"))

        'i18n Sigle traducibili?
        ' Riempio Tendina multipla Idoneità
        cbl_idoneita.Items.Add(New ListItem(DirectCast(GetLocalResourceObject("CaratteristicheDiCostruzione"), String), "1"))
        cbl_idoneita.Items.Add(New ListItem(DirectCast(GetLocalResourceObject("SeparazioneAmbienti"), String), "2"))
        cbl_idoneita.Items.Add(New ListItem(DirectCast(GetLocalResourceObject("SeparazioneProdotti"), String), "3"))
        cbl_idoneita.Items.Add(New ListItem(DirectCast(GetLocalResourceObject("CondizioniIgienicoSanitarie"), String), "4"))
        cbl_idoneita.Items.Add(New ListItem(DirectCast(GetLocalResourceObject("AutorizzazioneSanitaria"), String), "5"))
        cbl_idoneita.Items.Add(New ListItem("HACCP", "6"))
        cbl_idoneita.Items.Add(New ListItem(DirectCast(GetLocalResourceObject("Planimetria"), String), "7"))
        cbl_idoneita.Items.Add(New ListItem("Layout", "8"))
        cbl_idoneita.Items.Add(New ListItem(DirectCast(GetLocalResourceObject("DiagrammiDiFlusso"), String), "9"))
        cbl_idoneita.Items.Add(New ListItem("CDX-M004", "10"))
        cbl_idoneita.Items.Add(New ListItem(DirectCast(GetLocalResourceObject("SuperficiMinimeCoperte"), String), "11"))
        cbl_idoneita.Items.Add(New ListItem(DirectCast(GetLocalResourceObject("SuperficiMinimeScoperte"), String), "12"))



        If Operazione = enum_TipoOperazioneDB.Scrittura Then

            xFabbricato_Cod = "0"

            TxtSup_Conversione.Text = "0"
            Txt_Via.Text = ""
            Txt_CAP.Text = ""
            Cmb_Provincia.ClearSelection()
            Cmb_Comune.ClearSelection()

            Cmb_TipoFabbricato.SelectedIndex = Cmb_TipoFabbricato.Items.IndexOf(Cmb_TipoFabbricato.Items.FindByValue(20))

            Dim i As Integer

            Dim objIndirizzi As New AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read   'Object  Agro_Anagrafe_AD.Indirizzi_Read
            Dim DTIndirizzi As DataTable

            DTIndirizzi = objIndirizzi.Leggi(CStr(xPiva),
                                            CInt(xSa_Cod), 0, 0, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta, "",
                                            "", objParametri_Server)

            'Se il recordset non e' nullo
            If DTIndirizzi.Rows.Count > 0 Then

                For i = 0 To DTIndirizzi.Rows.Count - 1


                    'Me.Txt_CodIndirizzo_Centro.Text = DTIndirizzi.Rows(i).Item("cod_indirizzo")
                    'Creo un NUOVO cod indirizzo
                    Me.Txt_CodIndirizzo_Centro.Text = -1

                    Txt_Via.Text = DTIndirizzi.Rows(i).Item("ind_des")
                    Txt_Frazione.Text = DTIndirizzi.Rows(i).Item("frz_des")
                    Txt_CAP.Text = DTIndirizzi.Rows(i).Item("cap")

                    If Trim(DTIndirizzi.Rows(0).Item("Stato")) <> "" Then
                        Stato = UCase(Trim(DTIndirizzi.Rows(0).Item("Stato")))
                    End If

                    ' Inizializzazione combo Provincia
                    AgronicaCoreUtility.CaricaListControl.Provincie(Cmb_Provincia,
                                                            True, AgronicaAgenda_2010.Seleziona.ToUpper(), "",
                                                            False, 1, "", "", "", "", "", "", "", objParametri_Server, Stato)

                    Cmb_Comune.Enabled = False


                    Ripristina_Cmb_Stato(Stato)


                    Cmb_Provincia.SelectedIndex = Cmb_Provincia.Items.IndexOf(Cmb_Provincia.Items.FindByValue(DTIndirizzi.Rows(i).Item("pro_cod")))
                    Cmb_Comune.Enabled = True
                    AgronicaCoreUtility.CaricaListControl.Comuni(Cmb_Comune,
                                                                     True, "", "",
                                                                    DTIndirizzi.Rows(i).Item("pro_cod"), False, 1, "", "", HttpContext.Current.Session("ASG_objParametri_Server"))

                    Dim cod_tot As String = CStr(DTIndirizzi.Rows(i).Item("pro_cod_istat") & "|" & DTIndirizzi.Rows(i).Item("com_cod_istat") & "|" & DTIndirizzi.Rows(i).Item("pro_cod").ToString.ToLower)


                    Cmb_Comune.SelectedIndex = Cmb_Comune.Items.IndexOf(Cmb_Comune.Items.FindByValue(cod_tot))
                    Txt_Note.Text = DTIndirizzi.Rows(i).Item("note")

                    Me.Txt_ProvinciaSigla.Text = CStr(DTIndirizzi.Rows(i).Item("pro_cod"))

                    Me.Txt_ProCodIstat.Text = CStr(DTIndirizzi.Rows(i).Item("pro_cod_istat"))
                    Me.Txt_ComCodIstat.Text = CStr(DTIndirizzi.Rows(i).Item("com_cod_istat"))

                    'Me.Txt_Provincia.Text = Provincia_from_CodIstat(Server, Session, Page, CStr(RsIndirizzi.Fields("pro_cod_istat").Value), Nothing)
                    'Me.Txt_Provincia.Text = CStr(DTIndirizzi.Rows(i).Item("pro_des"))
                    'Me.Txt_Comune.Text = CStr(DTIndirizzi.Rows(i).Item("com_des"))




                Next

            Else

                ' Inizializzazione combo Provincia
                AgronicaCoreUtility.CaricaListControl.Provincie(Cmb_Provincia,
                                                            True, AgronicaAgenda_2010.Seleziona.ToUpper(), "",
                                                            False, 1, "", "", "", "", "", "", "", objParametri_Server, Stato)

                Cmb_Comune.Enabled = False
                ''''''
            End If


            HttpContext.Current.Session("idoneita") = "------------"

            AgronicaCoreUtility.CaricaListControl.CodiciAnagrafe_Fabbricato(CType(CmbCodice, ListControl),
                                                                          True, "", "0",
                                                                          "", "", objParametri_Server)

            Dim Dt As New DataTable

            '----- Definisco la struttura del DataTable

            Dt.Columns.Add(New DataColumn("Contatore", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("Id_Cod", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("Descrizione", GetType(String)))
            Dt.Columns.Add(New DataColumn("Val_Cod", GetType(String)))
            Dt.Columns.Add(New DataColumn("Validita_Inizio", GetType(String)))
            Dt.Columns.Add(New DataColumn("Validita_Fine", GetType(String)))

            jsCodici = DT_to_Json_Codici(Dt)

            '----- Associo il DataTable con il DataGrid
            Aggiorna_Griglia_Codici(Dt)

            chk_VisibileApp.Checked = True

        ElseIf Operazione = enum_TipoOperazioneDB.Modifica OrElse Operazione = enum_TipoOperazioneDB.Lettura Then

            xFabbricato_Cod = objParametriAgenda.Fabbricato

            '------------------------------------------
            '----- Analizzo la stringa XML
            '------------------------------------------

            'Carico la stringa nel documento XML
            XmlDoc.LoadXml(StringaXML)


            '----- Tag DatiFabbricati

            XML_DatiFabbricati = XmlDoc.SelectSingleNode("DatiFabbricati")

            '----- Tag Fabbricato

            XML_Fabbricato = XML_DatiFabbricati.SelectSingleNode("Fabbricato")

            TxtDenominazione.Text = CStr(XML_Fabbricato.GetAttribute("fabbricato_des"))
            TxtProprietarioCapi.Text = CStr(XML_Fabbricato.GetAttribute("proprietario_capi"))
            TxtCodiceBDN.Text = CStr(XML_Fabbricato.GetAttribute("codice_bdn"))

            Cmb_TipoFabbricato.SelectedIndex =
                Cmb_TipoFabbricato.Items.IndexOf(
                    Cmb_TipoFabbricato.Items.FindByValue(
                        XML_Fabbricato.GetAttribute("tipo_fabbricato_cod")))

            Cmb_TitoloPossesso.SelectedIndex =
                Cmb_TitoloPossesso.Items.IndexOf(
                    Cmb_TitoloPossesso.Items.FindByValue(
                        XML_Fabbricato.GetAttribute("titolopossesso")))

            Cmb_Regolamenti.SelectedIndex =
                Cmb_Regolamenti.Items.IndexOf(
                    Cmb_Regolamenti.Items.FindByValue(
                        XML_Fabbricato.GetAttribute("regolamento_cod")))

            'Particella Catastale

            Valore = "" & XML_Fabbricato.GetAttribute("prov") &
                    "£" & XML_Fabbricato.GetAttribute("com") &
                    "£" & XML_Fabbricato.GetAttribute("sezione") &
                    "£" & XML_Fabbricato.GetAttribute("foglio") &
                    "£" & XML_Fabbricato.GetAttribute("numero") &
                    "£" & XML_Fabbricato.GetAttribute("subalterno")

            Cmb_Particella.SelectedIndex =
                Cmb_Particella.Items.IndexOf(
                    Cmb_Particella.Items.FindByValue(
                        Valore))

            TxtValiditaInizio.Text = XML_Fabbricato.GetAttribute("validita_inizio")
            'controllo che non sia inserita la data di default..
            If TxtValiditaInizio.Text = "01/01/1900" Then
                TxtValiditaInizio.Text = ""
            End If

            TxtValiditaFine.Text = XML_Fabbricato.GetAttribute("validita_fine")
            If TxtValiditaFine.Text = "31/12/2100" Then
                TxtValiditaFine.Text = ""
            End If

            TxtSup_Convenzionale.Text = XML_Fabbricato.GetAttribute("mc_convenzionale")
            TxtSup_Conversione.Text = XML_Fabbricato.GetAttribute("mc_conversione")
            TxtSup_Biologico.Text = XML_Fabbricato.GetAttribute("mc_biologico")

            xIndirizzoCod = XML_Fabbricato.GetAttribute("indirizzo_cod")

            '-----
            ' Inserimento idoneità (combo adv.)

            HttpContext.Current.Session("XML_Fabbricato") = XML_Fabbricato

            Dim str_idoneita As String = ""


            If XML_Fabbricato.GetAttribute("idoneo_costruzione") = "1" Then
                'cbl_idoneita.SelectedIndex = cbl_idoneita.Items.IndexOf(cbl_idoneita.Items.FindByValue("1"))
                str_idoneita &= "1-"
            Else
                str_idoneita &= "-"
            End If

            If XML_Fabbricato.GetAttribute("idoneo_separazambienti") = "1" Then
                'cbl_idoneita.SelectedIndex = cbl_idoneita.Items.IndexOf(cbl_idoneita.Items.FindByValue("2"))
                str_idoneita &= "2-"
            Else
                str_idoneita &= "-"
            End If

            If XML_Fabbricato.GetAttribute("idoneo_separazprodotti") = "1" Then
                'cbl_idoneita.SelectedIndex = cbl_idoneita.Items.IndexOf(cbl_idoneita.Items.FindByValue("3"))
                str_idoneita &= "3-"
            Else
                str_idoneita &= "-"
            End If

            If XML_Fabbricato.GetAttribute("idoneo_condigieniche") = "1" Then
                'cbl_idoneita.SelectedIndex = cbl_idoneita.Items.IndexOf(cbl_idoneita.Items.FindByValue("4"))
                str_idoneita &= "4-"
            Else
                str_idoneita &= "-"
            End If

            If XML_Fabbricato.GetAttribute("idoneo_autorizsanitaria") = "1" Then
                'cbl_idoneita.SelectedIndex = cbl_idoneita.Items.IndexOf(cbl_idoneita.Items.FindByValue("5"))
                str_idoneita &= "5-"
            Else
                str_idoneita &= "-"
            End If

            If XML_Fabbricato.GetAttribute("idoneo_haccp") = "1" Then
                'cbl_idoneita.SelectedIndex = cbl_idoneita.Items.IndexOf(cbl_idoneita.Items.FindByValue("6"))
                str_idoneita &= "6-"
            Else
                str_idoneita &= "-"
            End If

            If XML_Fabbricato.GetAttribute("idoneo_planimetria") = "1" Then
                'cbl_idoneita.SelectedIndex = cbl_idoneita.Items.IndexOf(cbl_idoneita.Items.FindByValue("7"))
                str_idoneita &= "7-"
            Else
                str_idoneita &= "-"
            End If

            If XML_Fabbricato.GetAttribute("idoneo_layout") = "1" Then
                'cbl_idoneita.SelectedIndex = cbl_idoneita.Items.IndexOf(cbl_idoneita.Items.FindByValue("8"))
                str_idoneita &= "8-"
            Else
                str_idoneita &= "-"
            End If

            If XML_Fabbricato.GetAttribute("idoneo_diagrammiflusso") = "1" Then
                'cbl_idoneita.SelectedIndex = cbl_idoneita.Items.IndexOf(cbl_idoneita.Items.FindByValue("9"))
                str_idoneita &= "9-"
            Else
                str_idoneita &= "-"
            End If

            If XML_Fabbricato.GetAttribute("idoneo_cdx_m004") = "1" Then
                'cbl_idoneita.SelectedIndex = cbl_idoneita.Items.IndexOf(cbl_idoneita.Items.FindByValue("10"))
                str_idoneita &= "10-"
            Else
                str_idoneita &= "-"
            End If

            If XML_Fabbricato.GetAttribute("idoneo_supmincoperte") = "1" Then
                'cbl_idoneita.SelectedIndex = cbl_idoneita.Items.IndexOf(cbl_idoneita.Items.FindByValue("11"))
                str_idoneita &= "11-"
            Else
                str_idoneita &= "-"
            End If

            If XML_Fabbricato.GetAttribute("idoneo_supminscoperte") = "1" Then
                'cbl_idoneita.SelectedIndex = cbl_idoneita.Items.IndexOf(cbl_idoneita.Items.FindByValue("12"))
                str_idoneita &= "12-"
            Else
                str_idoneita &= "-"
            End If

            If str_idoneita.Length > 0 Then
                str_idoneita = str_idoneita.Substring(0, str_idoneita.Length - 1)
            End If

            HttpContext.Current.Session("idoneita") = str_idoneita

            '-------------------------------------------

            Dim Gen_Cod As Integer
            Dim Spe_Cod As Integer
            Dim IPro_Cod As Integer
            Dim Cod_Fabb As String
            Dim Att_Cod As Integer


            '----- Tag Indirizzo

            XML_Indirizzo = XML_Fabbricato.SelectSingleNode("Indirizzo")

            If XML_Indirizzo IsNot Nothing Then

                Me.Txt_CodIndirizzo_Centro.Text = xIndirizzoCod

                Txt_Via.Text = XML_Indirizzo.GetAttribute("ind_des")
                Txt_Frazione.Text = XML_Indirizzo.GetAttribute("frz_des")
                Txt_CAP.Text = XML_Indirizzo.GetAttribute("cap")
                Ripristina_Cmb_Stato(XML_Indirizzo.GetAttribute("stato"))
                Txt_Note.Text = XML_Indirizzo.GetAttribute("note")

                Me.Txt_ProvinciaSigla.Text = XML_Indirizzo.GetAttribute("pro_cod")

                Me.Txt_ProCodIstat.Text = XML_Indirizzo.GetAttribute("pro_cod_istat")
                Me.Txt_ComCodIstat.Text = XML_Indirizzo.GetAttribute("com_cod_istat")

                If Trim(XML_Indirizzo.GetAttribute("stato")) <> "" Then
                    Stato = UCase(Trim(XML_Indirizzo.GetAttribute("stato")))
                End If

                ' Inizializzazione combo Provincia
                AgronicaCoreUtility.CaricaListControl.Provincie(Cmb_Provincia,
                                                            True, AgronicaAgenda_2010.Seleziona.ToUpper(), "",
                                                            False, 1, "", "", "", "", "", "", "", objParametri_Server, Stato)

                Cmb_Comune.Enabled = False


                AgronicaCoreUtility.CaricaListControl.Comuni(Cmb_Comune,
                                                                 True, "", "",
                                                                XML_Indirizzo.GetAttribute("pro_cod"), False, 1, "", "", HttpContext.Current.Session("ASG_objParametri_Server"))




                Cmb_Comune.Enabled = True


                Cmb_Comune.SelectedIndex =
                    Cmb_Comune.Items.IndexOf(
                Cmb_Comune.Items.FindByValue(
                    XML_Indirizzo.GetAttribute("pro_cod_istat") & "|" & XML_Indirizzo.GetAttribute("com_cod_istat") & "|" & XML_Indirizzo.GetAttribute("pro_cod").ToLower))

                Cmb_Provincia.SelectedIndex =
                    Cmb_Provincia.Items.IndexOf(
                        Cmb_Provincia.Items.FindByValue(XML_Indirizzo.GetAttribute("pro_cod")))

            End If


            '''''''''''''''''''''''''''''''''''''''''''''''''''



            'carico denominazione
            'Dim StringaXML As String

            'Dim StrXMLCodici As String
            Dim StrXmlCodiciAttuali As String = ""

            Dim StrCaratteristiche As String = ""
            Dim StrCaratteristica As String

            Dim StrConfigurazioniBDN As String = ""
            Dim StrConfigurazioneBDN As String


            Cmb_TipoFabbricato.SelectedIndex =
                Cmb_TipoFabbricato.Items.IndexOf(
                    Cmb_TipoFabbricato.Items.FindByValue(
                        XML_Fabbricato.GetAttribute("tipo_fabbricato_cod")))


            'ALLA FINE DA TOGLIERE IL COMMENTO!!!!!!!!!!
            Cmb_TipoFabbricato.Enabled = False
            'Cmb_TipoFabbricato.BackColor = AgroColor_GrigioChiaro

            Cmb_TitoloPossesso.SelectedIndex =
                Cmb_TitoloPossesso.Items.IndexOf(
                    Cmb_TitoloPossesso.Items.FindByValue(
                        XML_Fabbricato.GetAttribute("titolopossesso")))

            Cmb_Regolamenti.SelectedIndex =
                Cmb_Regolamenti.Items.IndexOf(
                    Cmb_Regolamenti.Items.FindByValue(
                        XML_Fabbricato.GetAttribute("regolamento_cod")))




            '----- Tag Stalla

            XML_Stalla = XML_Fabbricato.SelectSingleNode("Stalla")

            If XML_Stalla IsNot Nothing Then

                'Me.ImgBtn_From1_To2.Visible = True

                Gen_Cod = XML_Stalla.GetAttribute("gen_cod")
                Spe_Cod = XML_Stalla.GetAttribute("spe_cod")
                IPro_Cod = XML_Stalla.GetAttribute("ipro_cod")
                Cod_Fabb = XML_Stalla.GetAttribute("cod_fabb")

                '--- Animali
                Cmb_Animali.SelectedIndex =
                    Cmb_Animali.Items.IndexOf(
                        Cmb_Animali.Items.FindByValue(
                            Gen_Cod & "|" & Spe_Cod))
                Cmb_Animali.Enabled = False

                'gestione codice azienda in BDN leggendo da Imprese_Codici
                Dim BDN_Allev_IdFiscale As String = XML_Stalla.GetAttribute("BDN_allev_idfiscale")
                If IsNothing(BDN_Allev_IdFiscale) OrElse BDN_Allev_IdFiscale = "" Then
                    Dim objImpreseCodici_R As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
                    Dim dt_ValCod As DataTable = objImpreseCodici_R.LeggixCodice(XML_Stalla.GetAttribute("piva"), 1010,
                                                                                 enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                                 "", "",
                                                                                 objParametri_Server)

                    If IsNothing(dt_ValCod) OrElse dt_ValCod.Rows.Count = 0 Then

                    ElseIf dt_ValCod.Rows.Count > 1 Then

                    Else
                        BDN_Allev_IdFiscale = dt_ValCod.Rows(0).Item("val_cod")
                    End If

                End If


                'If Operazione = enum_TipoOperazioneDB.Lettura Then
                '    TxtIdFiscaleStallaBDN.Text = BDN_Allev_IdFiscale
                'End If
                TxtCodiceStallaBDN.Text = XML_Stalla.GetAttribute("BDN_codice_azienda")

                If Me.Cmb_Animali.SelectedItem.Text <> "" Then

                    '--- Indirizzi Produttivi
                    'CaricaCombo_Lista_IndirizziProd_Animali( _
                    '                        Me.Cmb_IndirizzoProduttivo, _
                    '                        Gen_Cod, _
                    '                        Spe_Cod, _
                    '                        objParametri_Server)
                    AgronicaCoreUtility.CaricaListControl.Lista_IndirizziProd_Animali(Me.Cmb_IndirizzoProduttivo,
                            True,
                            "",
                            "",
                            Gen_Cod,
                            Spe_Cod,
                            "",
                            "",
                            objParametri_Server
                            )

                    Cmb_IndirizzoProduttivo.SelectedIndex =
                        Cmb_IndirizzoProduttivo.Items.IndexOf(
                            Cmb_IndirizzoProduttivo.Items.FindByValue(
                                Gen_Cod & "|" & Spe_Cod & "|" & IPro_Cod))



                    '--- Tipo Ricovero
                    'CaricaCombo_Lista_Tipi_Stalla( _
                    '                            Me.Cmb_TipoRicovero, _
                    '                            Gen_Cod, _
                    '                            Spe_Cod, _
                    '                            IPro_Cod, _
                    '                            objParametri_Server)

                    AgronicaCoreUtility.CaricaListControl.Lista_Tipi_Stalla(Cmb_TipoRicovero,
                        True,
                        AgronicaAgenda_2010.Seleziona.ToUpper(),
                        "",
                        Gen_Cod,
                        Spe_Cod,
                        IPro_Cod,
                        "",
                        "",
                        HttpContext.Current.Session("ASG_objParametri_Server")
                        )



                    Cmb_TipoRicovero.SelectedIndex =
                        Cmb_TipoRicovero.Items.IndexOf(
                        Cmb_TipoRicovero.Items.FindByValue(
                            Gen_Cod & "|" & Spe_Cod & "|" & IPro_Cod & "|" & Cod_Fabb))


                    '--- Sottotipo Ricovero



                    'CaricaCombo_Lista_Sottotipi_Stalla(Me.Cmb_SottotipoRicovero, _
                    '                                   Gen_Cod, _
                    '                                   Spe_Cod, _
                    '                                   IPro_Cod, _
                    '                                   Cod_Fabb, _
                    '                                   objParametri_Server)

                    AgronicaCoreUtility.CaricaListControl.Lista_Sottotipi_Stalla(Cmb_SottotipoRicovero,
                        True,
                                AgronicaAgenda_2010.Seleziona.ToUpper(),
                                "",
                                Gen_Cod,
                                Spe_Cod,
                                IPro_Cod,
                                Cod_Fabb,
                                "",
                                "",
                                HttpContext.Current.Session("ASG_objParametri_Server")
                                )

                    Cmb_SottotipoRicovero.SelectedIndex =
                      Cmb_SottotipoRicovero.Items.IndexOf(
                          Cmb_SottotipoRicovero.Items.FindByValue(
                              Gen_Cod & "|" & Spe_Cod & "|" & IPro_Cod & "|" & Cod_Fabb))


                    'TxtAllevamentoId_BDN.Text = XML_Stalla.GetAttribute("gen_cod")

                    'Combo Caratteristiche
                    AgronicaCoreUtility.CaricaListControl.Stalle_Attributi(Cmb_Caratteristiche, True, AgronicaAgenda_2010.Seleziona.ToUpper(), "",
                                            Cod_Fabb, "", "", objParametri_Server)





                    ''' Estrapolo le Caratteristiche delle Stalle
                    Dim objStalle_car As New AgronicaCoreAnagrafeDAL.Stalla_Caratteristiche_R
                    Dim DTStalle_car As DataTable

                    DTStalle_car = objStalle_car.Leggi(CStr(xPiva), CInt(xSa_Cod), CInt(xFabbricato_Cod), 0, 0,
                                                        AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)

                    jsStalle = DT_to_Json_Stalle(DTStalle_car)


                    HttpContext.Current.Session("dt_Stalle_car") = DTStalle_car


                    For j = 0 To DTStalle_car.Rows.Count - 1


                        'Genero(l) 'XML del singolo nodo
                        Call XML_Stalla_Caratteristica(enum_CodificaDecodifica.Codifica,
                                                        StrCaratteristica,
                                                        enum_TipoOperazioneDB.Cancellazione,
                                                        Att_Cod,
                                                        Valore,
                                                        XML_Indirizzo.GetAttribute("validita_inizio"),
                                                        XML_Indirizzo.GetAttribute("validita_fine"),
                                                        BaseCode,
                                                        TopCode)

                        'Inserisco l'XML nella stringa complessiva
                        StrCaratteristiche &= StrCaratteristica



                    Next

                    '----- Salvo la StrXmlStallaCaratteristiche 
                    Session("StrXmlStallaCaratteristicheAttuali") = StrCaratteristiche



                    ''' Estrapolo le Caratteristiche delle Stalle
                    Dim objStalleConfBDN_car As New AgronicaCoreAnagrafeDAL.Stalla_Configurazioni_BDN_R
                    Dim DTStalleConfBDN_car As DataTable

                    DTStalleConfBDN_car = objStalleConfBDN_car.Leggi(0, CStr(xPiva), CInt(xSa_Cod), CInt(xFabbricato_Cod), "", "", AGRODATAINIZIO, AGRODATAFINE,
                                                        AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)

                    jsStalleConfigurazioniBDN = DT_to_Json_Stalle_Conf_BDN(DTStalleConfBDN_car)


                    HttpContext.Current.Session("dt_Stalle_conf_BDN_car") = DTStalleConfBDN_car


                    For j = 0 To DTStalleConfBDN_car.Rows.Count - 1

                        Dim ID = IIf(IsDBNull(DTStalleConfBDN_car.Rows(j)("ID")), 0, DTStalleConfBDN_car.Rows(j)("ID"))
                        Dim CF_Detentore = IIf(IsDBNull(DTStalleConfBDN_car.Rows(j)("CF_Detentore")), "", DTStalleConfBDN_car.Rows(j)("CF_Detentore"))
                        Dim CF_Proprietario = IIf(IsDBNull(DTStalleConfBDN_car.Rows(j)("CF_Proprietario")), "", DTStalleConfBDN_car.Rows(j)("CF_Proprietario"))
                        Dim RagSoc_Detentore = IIf(IsDBNull(DTStalleConfBDN_car.Rows(j)("RagSoc_Detentore")), "", DTStalleConfBDN_car.Rows(j)("RagSoc_Detentore"))
                        Dim RagSoc_Proprietario = IIf(IsDBNull(DTStalleConfBDN_car.Rows(j)("RagSoc_Proprietario")), "", DTStalleConfBDN_car.Rows(j)("RagSoc_Proprietario"))
                        'Genero(l) 'XML del singolo nodo
                        Call XML_Stalla_Configurazione_BDN(enum_CodificaDecodifica.Codifica,
                                                        StrConfigurazioneBDN,
                                                        enum_TipoOperazioneDB.Cancellazione,
                                                        ID,
                                                        CF_Detentore,
                                                        CF_Proprietario,
                                                        RagSoc_Detentore,
                                                        RagSoc_Proprietario,
                                                        XML_Indirizzo.GetAttribute("validita_inizio"),
                                                        XML_Indirizzo.GetAttribute("validita_fine"),
                                                        BaseCode,
                                                        TopCode)

                        'Inserisco l'XML nella stringa complessiva
                        StrConfigurazioniBDN &= StrConfigurazioneBDN



                    Next

                    '----- Salvo la StrXmlStallaCaratteristiche 
                    Session("StrXmlStallaConfigurazioniBDN") = StrConfigurazioniBDN


                End If

            End If




            '''''''''''''''''''''''''''''''''''''''''''





            ''Elimino l'oggetto COM+
            'objIndirizzi = Nothing




            'INTI - Codici Anagrafici 
            AgronicaCoreUtility.CaricaListControl.CodiciAnagrafe_Fabbricato(CType(CmbCodice, ListControl),
                                                                            True, "", "0",
                                                                            "", "", objParametri_Server)
            TxtCodiceValore.Text = ""


            '----- Tag Codici 
            Dim objFabbricatiCodici As New AgronicaCoreAnagrafeDAL.Fabbricati_Codici_R


            DTCodici = objFabbricatiCodici.Leggi(CStr(xPiva), xSa_Cod, xFabbricato_Cod, 0, "",
                                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                                            " ((id_cod < 2000) OR (id_cod >= 3000)) AND (id_Cod <> " & enum_CodiciAnagrafe.Visibile_da_App & ") ",
                                            "",
                                            objParametri_Server)

            'StrXMLCodici = ""

            'XMLs_Codici = XML_Fabbricato.GetElementsByTagName("CodiceFabbricato")

            HttpContext.Current.Session("dt_Codici") = DTCodici


            Dim Dt As New DataTable
            Dim Dr As DataRow

            '----- Definisco la struttura del DataTable

            Dt.Columns.Add(New DataColumn("Contatore", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("Id_Cod", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("Descrizione", GetType(String)))
            Dt.Columns.Add(New DataColumn("Val_Cod", GetType(String)))
            Dt.Columns.Add(New DataColumn("Validita_Inizio", GetType(String)))
            Dt.Columns.Add(New DataColumn("Validita_Fine", GetType(String)))

            If (Not IsNothing(DTCodici)) AndAlso (DTCodici.Rows.Count > 0) Then

                Calcola_BaseCode_TopCode(BaseCode, TopCode, Session("ASG_ProgressivoGIAS"))

                Dim Contatore = 1
                Dim objCodiceAnagrafeR As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R

                For i = 0 To DTCodici.Rows.Count - 1

                    Select Case DTCodici.Rows(i).Item("Id_Cod")

                        Case enum_CodiciAnagrafe.CodiceCUAA, enum_CodiciAnagrafe.Tecnico

                        Case Else

                            'Creo una nuova riga
                            Dr = Dt.NewRow

                            'Definisco i valori

                            Dr.Item("Contatore") = Contatore
                            Contatore += 1

                            Dr.Item("Id_Cod") = DTCodici.Rows(i).Item("id_cod")
                            Dr.Item("Descrizione") = objCodiceAnagrafeR.CodiceAnagrafeDes_from_CodiceAnagrafeCod(CInt(DTCodici.Rows(i).Item("id_cod")), objParametri_Server)
                            Dr.Item("Val_Cod") = DTCodici.Rows(i).Item("val_cod")

                            If (CDate(DTCodici.Rows(i).Item("Validita_Inizio"))) = AGRODATAINIZIO Then
                                Dr.Item("Validita_Inizio") = "..."
                            Else
                                'Dr.Item("Validita_Inizio") = CDate(DTCodici.Rows(i).Item("Validita_Inizio")).ToShortDateString
                                Dr.Item("Validita_Inizio") = CDate(DTCodici.Rows(i).Item("Validita_Inizio")).ToString("dd/MM/yyyy")
                            End If

                            If (CDate(DTCodici.Rows(i).Item("Validita_Fine"))) = AGRODATAFINE Then
                                Dr.Item("Validita_Fine") = "..."
                            Else
                                'Dr.Item("Validita_Fine") = CDate(DTCodici.Rows(i).Item("Validita_Fine")).ToShortDateString
                                Dr.Item("Validita_Fine") = CDate(DTCodici.Rows(i).Item("Validita_Fine")).ToString("dd/MM/yyyy")
                            End If

                            Dt.Rows.Add(Dr)

                    End Select



                    'Genero l'XML del singolo nodo
                    Call XML_Codice(enum_CodificaDecodifica.Codifica,
                                    StrCodice,
                                    enum_TipoOperazioneDB.Cancellazione,
                                    DTCodici.Rows(i).Item("Id_Cod"),
                                    DTCodici.Rows(i).Item("Val_cod"),
                                    DTCodici.Rows(i).Item("xValidita_Inizio"),
                                    DTCodici.Rows(i).Item("xValidita_Fine"),
                                    BaseCode,
                                    TopCode,
                                    "Fabbricato")

                    StrXmlCodiciAttuali &= StrCodice

                Next

                '----- Salvo la StrXmlCodiciAttuali 

                Session("StrXmlCodiciAttuali") = StrXmlCodiciAttuali

                ' Controllo se non è stato aggiunto un nuovo record di Codici Anagrafici
                If (IsNothing(HttpContext.Current.Session("dt_Codici"))) Then

                    HttpContext.Current.Session("dt_Codici") = Dt
                    'GridView_Codici.DataSource = HttpContext.Current.Session("dt_Codici")
                    'GridView_Codici.DataBind()
                Else
                    'GridView_Codici.DataSource = Dt
                    'GridView_Codici.DataBind()
                End If



            End If

            jsCodici = DT_to_Json_Codici(Dt)

            '----- Salvo la StrXmlCodiciAttuali 

            Session("StrXmlCodiciAttuali") = StrXmlCodiciAttuali


            '----- Associo il DataTable con il DataGrid
            Aggiorna_Griglia_Codici(Dt)


            Dim DTCodiceVisibile = objFabbricatiCodici.Leggi(CStr(xPiva), xSa_Cod, xFabbricato_Cod, enum_CodiciAnagrafe.Visibile_da_App, "",
                                            AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                            "",
                                            "",
                                            objParametri_Server)

            chk_VisibileApp.Checked = False
            If DTCodiceVisibile.Rows.Count > 0 AndAlso IsNumeric(DTCodiceVisibile.Rows(0)("val_Cod")) AndAlso CInt(DTCodiceVisibile.Rows(0)("val_Cod")) = 1 Then
                chk_VisibileApp.Checked = True
            End If

            chk_DefaultOrtofrutta.Checked = False
            Dim OGenerazioni_Anagrafe_Log_R As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Log_R
            Dim dtDef_Ort = OGenerazioni_Anagrafe_Log_R.Leggi(CStr(xPiva), xSa_Cod, -1, 2, 15, 391, AGRODATAINIZIO, AGRODATAFINE, " Key1 = " & xFabbricato_Cod & " ", "", objParametri_Server)
            If dtDef_Ort.Rows.Count > 0 Then
                chk_DefaultOrtofrutta.Checked = True
            End If

            chk_MagazzinoFarmaci.Attributes.Add("hidden", "hidden")
            If Not IsNothing(XML_Fabbricato.GetAttribute("tipo_fabbricato_cod")) AndAlso XML_Fabbricato.GetAttribute("tipo_fabbricato_cod").Equals("20") Then
                chk_MagazzinoFarmaci.Attributes.Remove("hidden")
                chk_MagazzinoFarmaci.Checked = XML_Fabbricato.GetAttribute("chkmagazzinofarmaci")
            End If

        End If

            If Cmb_Comune.Items.Count > 0 Then
            HttpContext.Current.Session("nome_comune_settato") = Cmb_Comune.SelectedItem.Text
            HttpContext.Current.Session("comune_settato") = Cmb_Comune.SelectedValue
        End If

    End Sub


    '#####################################################################################################################################################
    Private Sub Aggiorna_Griglia_Codici(ByVal Dt As DataTable)

        ' Chiavi per recuperare le righe
        Dim DtKeys(4) As String
        DtKeys(0) = "Contatore"
        DtKeys(1) = "Id_Cod"
        DtKeys(2) = "Val_Cod"
        DtKeys(3) = "Validita_Inizio"
        DtKeys(4) = "Validita_Fine"


        '----- Associo il DataTable con la DataGrid
        'GridView_Codici.DataSource = Dt
        'GridView_Codici.DataKeyNames = DtKeys
        'GridView_Codici.DataBind()

        '----- Salvo il DataTable dentro il viewstate
        If (Not IsNothing(HttpContext.Current.Session("dt_Codici"))) Then

            ' Aggiorno il datatable di Codici Anagrafici  con quello in sessione
            'DTCodici = HttpContext.Current.Session("dt_Codici")

            'GridView_Codici.DataSource = HttpContext.Current.Session("dt_Codici")
            ''GridView_Codici.DataKeyNames = DtKeys
            'GridView_Codici.DataBind()

        Else

            '----- Associo il DataTable con la DataGrid
            'GridView_Codici.DataSource = Dt
            'GridView_Codici.DataKeyNames = DtKeys
            'GridView_Codici.DataBind()

            '----- Salvo il DataTable dentro il viewstate
            HttpContext.Current.Session("dt_Codici") = Dt

        End If

    End Sub


    '##########################################################################################################################################
    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)

        ' Pulisco la Sessione
        HttpContext.Current.Session("dt_Stalle_car") = Nothing
        HttpContext.Current.Session("dt_Codici") = Nothing

        Dim TargetUrl As String = ""
        Dim objParametriAgenda As New ParametriAgenda

        If objParametriAgenda.SitoOrigine = Enum_SiteRedirector.GiasNG Then

            MenuBS_2017_RedirectGestione.RedirectGenerico(objParametriAgenda.Piva,
                                                          Enum_SiteRedirector.GiasNG,
                                                          enum_PagineGiasNG.Pagina_Menu_Anagrafica_Fabbricati,
                                                          TargetUrl,
                                                          objParametri_Server)

        Else


            TargetUrl = Master.TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAnagrafica, objParametriAgenda, , Qs_Visibilita)



        End If

        Response.Redirect(TargetUrl)

    End Sub



    '########################################################################################
    Private Sub ImgBtn_SalvaTutto_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_SalvaTutto.Click

        Salva_Tutto()

    End Sub


    '########################################################################################
    Private Sub Salva_Tutto()

        Try

            'Generali
            'Dim Operazione As enum_TipoOperazioneDB
            'Dim Indice As Integer
            Dim StrDummy As String
            Dim Testo As String
            Dim ArrTesto() As String


            'Fabbricato
            Dim Piva As String
            Dim Sa_Cod As Integer
            Dim Fabbricato_Cod As Integer
            Dim Fabbricato_Des As String
            Dim Indirizzo_Cod As Integer
            Dim Tipo_Fabbricato_Cod As Integer
            Dim Regolamento_Cod As Integer

            Dim p_Prov As String
            Dim p_Com As String
            Dim p_Sezione As String
            Dim p_Foglio As Integer
            Dim p_Numero As Integer
            Dim p_Subalterno As String

            Dim TitoloPossesso As Integer

            Dim mc_Convenzionale As Double
            Dim mc_Conversione As Double
            Dim mc_Biologico As Double

            Dim BaseCode As Integer
            Dim TopCode As Integer

            Dim ProprietarioCapi As String
            Dim CodiceBDN As String

            'Indirizzo
            Dim Cod_Indirizzo As Integer
            Dim Ind_Des As String
            Dim Frz_Des As String
            Dim CAP As String
            Dim Com_Des As String
            Dim Pro_Cod As String
            Dim Stato As String
            Dim Note As String
            Dim Pro_Cod_Istat As String
            Dim Com_Cod_Istat As String

            Dim Conversione_Inizio As Date
            Dim Conversione_Fine As Date

            Dim Validita_Inizio As Date
            Dim Validita_Fine As Date


            'Idoneita'
            Dim Id_Costruzione As Integer
            Dim Id_SeparazAmbienti As Integer
            Dim Id_SeparazProdotti As Integer
            Dim Id_CondIgieniche As Integer
            Dim Id_AutorizSanitaria As Integer
            Dim Id_HACCP As Integer
            Dim Id_Planimetria As Integer
            Dim Id_Layout As Integer
            Dim Id_DiagrammiFlusso As Integer
            Dim Id_CdxM004 As Integer
            Dim Id_SupMinCoperte As Integer
            Dim Id_SupMinScoperte As Integer

            Dim Visibile_da_App As Integer
            Dim Default_Ortofrutta As Integer
            Dim flagMagazzinoFarmaci As Integer


            Dim FlagOK As Boolean


            '------------------------------------------------
            '----- Calcolo i valori di BaseCode e TopCode
            '------------------------------------------------

            Calcola_BaseCode_TopCode(BaseCode, TopCode, Session("ASG_ProgressivoGIAS"))


            '------------------------------------------------
            '-----  INDIRIZZO  ------------------------------
            '------------------------------------------------

            Dim StrIndirizzo As String
            Dim Tipo_Indirizzo As Integer

            'Prelevo le informazioni immediate
            Tipo_Indirizzo = 1
            'ATTENZIONE: corretto baco in data 30/06/2009: veniva salvato l'indirizzo del fabbricato con lo stesso cod_indirizzo del centro... corretto!
            Cod_Indirizzo = Txt_CodIndirizzo_Centro.Text
            Ind_Des = Txt_Via.Text
            Frz_Des = Txt_Frazione.Text
            CAP = Txt_CAP.Text
            'Com_Des = Me.Txt_Comune.Text
            Com_Des = Cmb_Comune.SelectedItem.Text
            Pro_Cod = Me.Txt_ProvinciaSigla.Text
            'Stato = Txt_Stato.Text


            'Lettura Gestione_Gerarchia_Geografica
            Dim DT_Nazioni As DataTable
            Dim objNazioni As New AgronicaCoreMetaSchemaDAL.ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166_R
            DT_Nazioni = objNazioni.Leggi(cmb_Stato.SelectedItem.Value, "", "Descrizione", objParametri_Server)

            If CInt(DT_Nazioni(0).Item("Gestione_Gerarchia_Geografica")) = 1 Then

                Frz_Des = Txt_Frazione.Text
                CAP = Txt_CAP.Text
                'Com_Des = ddl_comune.SelectedItem.Text
                'Com_Des = HttpContext.Current.Session("nome_comune_settato")
                Pro_Cod = Me.Txt_ProvinciaSigla.Text
                Pro_Cod_Istat = Me.Txt_ProCodIstat.Text

                If HttpContext.Current.Session("com") = Nothing Then
                    Set_Comune(Cmb_Comune.SelectedItem.Value)
                End If

                Dim com_codice As String
                'com_codice = ddl_comune.SelectedItem.Value
                com_codice = HttpContext.Current.Session("comune_settato")
                Com_Cod_Istat = HttpContext.Current.Session("com")
                Com_Des = Cmb_Comune.SelectedItem.Text
            Else
                Frz_Des = ""
                CAP = Txt_CAP.Text
                'Com_Des = ddl_comune.SelectedItem.Text
                Com_Des = ""
                Pro_Cod = "00"
                Pro_Cod_Istat = "000"
                Com_Cod_Istat = "000"
            End If

            If cmb_Stato.SelectedItem.Value = "IT" AndAlso CAP.Length > 5 Then
                Throw New Exception("Il CAP non può essere più lungo di 5")
            End If


            Note = Txt_Note.Text
            Stato = cmb_Stato.SelectedItem.Value

            If TxtValiditaInizio.Text = "" Then
                Validita_Inizio = CDate("01/01/1900")
            Else
                Validita_Inizio = CDate(TxtValiditaInizio.Text)
            End If

            If TxtValiditaFine.Text = "" Then
                Validita_Fine = CDate("31/12/2100")
            Else
                Validita_Fine = CDate(TxtValiditaFine.Text)
            End If



            'Genero la stringa XML
            Call XML_Indirizzo(enum_CodificaDecodifica.Codifica,
                            StrIndirizzo,
                            Operazione,
                            Tipo_Indirizzo,
                            Cod_Indirizzo,
                            Ind_Des,
                            Frz_Des,
                            CAP,
                            Com_Des,
                            Pro_Cod,
                            Stato,
                            Note,
                            Pro_Cod_Istat,
                            Com_Cod_Istat,
                            Validita_Inizio,
                            Validita_Fine,
                            BaseCode,
                            TopCode)



            '------------------------------------------------
            '-----  FABBRICATO  -----------------------------
            '------------------------------------------------

            Dim StrFabbricato As String

            'Call ChiaveAlbero_Decodifica_PartitaIVA_SaCod(Qs_Key, _
            '                                              Piva, _
            '                                              Sa_Cod)


            'Fabbricato_Cod = TxtFabbricatoCod.Text
            Fabbricato_Des = TxtDenominazione.Text
            'ATTENZIONE: corretto baco in data 30/06/2009: veniva salvato l'indirizzo del fabbricato con lo stesso cod_indirizzo del centro... corretto!
            Indirizzo_Cod = Cod_Indirizzo
            Tipo_Fabbricato_Cod = Cmb_TipoFabbricato.SelectedItem.Value
            TitoloPossesso = Cmb_TitoloPossesso.SelectedItem.Value

            If chk_MagazzinoFarmaci.Checked AndAlso Tipo_Fabbricato_Cod = MAGAZZINO Then
                flagMagazzinoFarmaci = 1
            Else
                flagMagazzinoFarmaci = 0
                TxtProprietarioCapi.Text = ""
                TxtCodiceBDN.Text = ""
            End If

            mc_Convenzionale = 0
            If IsNumeric(TxtSup_Convenzionale.Text) Then
                mc_Convenzionale = TxtSup_Convenzionale.Text
            End If

            mc_Conversione = 0
            If IsNumeric(TxtSup_Conversione.Text) Then
                mc_Conversione = TxtSup_Conversione.Text
            End If

            mc_Biologico = 0
            If IsNumeric(TxtSup_Biologico.Text) Then
                mc_Biologico = TxtSup_Biologico.Text
            End If

            Regolamento_Cod = Cmb_Regolamenti.SelectedItem.Value
            Conversione_Inizio = #1/1/1900#
            Conversione_Fine = #1/1/1900#

            If Cmb_Particella.Items.Count <> 0 Then
                If Cmb_Particella.SelectedIndex > 0 Then
                    Testo = Cmb_Particella.SelectedItem.Value
                    ArrTesto = Split(Testo, "£")
                    p_Prov = ArrTesto(0)
                    p_Com = ArrTesto(1)
                    p_Sezione = ArrTesto(2)
                    p_Foglio = ArrTesto(3)
                    p_Numero = ArrTesto(4)
                    p_Subalterno = ArrTesto(5)
                Else
                    p_Prov = "000"
                    p_Com = "000"
                    p_Sezione = ""
                    p_Foglio = 0
                    p_Numero = 0
                    p_Subalterno = ""
                End If
            Else
                p_Prov = "000"
                p_Com = "000"
                p_Sezione = ""
                p_Foglio = 0
                p_Numero = 0
                p_Subalterno = ""
            End If

            ProprietarioCapi = TxtProprietarioCapi.Text
            CodiceBDN = TxtCodiceBDN.Text

            Dim idoneita As String
            idoneita = HttpContext.Current.Session("idoneita")

            If idoneita <> "" Then
                Dim idoneita_val As String() = idoneita.Split(New Char() {"-"c})


                Id_Costruzione = IIf(idoneita_val(0) <> "", 1, 0)
                Id_SeparazAmbienti = IIf(idoneita_val(1) <> "", 1, 0)
                Id_SeparazProdotti = IIf(idoneita_val(2) <> "", 1, 0)
                Id_CondIgieniche = IIf(idoneita_val(3) <> "", 1, 0)
                Id_AutorizSanitaria = IIf(idoneita_val(4) <> "", 1, 0)
                Id_HACCP = IIf(idoneita_val(5) <> "", 1, 0)
                Id_Planimetria = IIf(idoneita_val(6) <> "", 1, 0)
                Id_Layout = IIf(idoneita_val(7) <> "", 1, 0)
                Id_DiagrammiFlusso = IIf(idoneita_val(8) <> "", 1, 0)
                Id_CdxM004 = IIf(idoneita_val(9) <> "", 1, 0)
                Id_SupMinCoperte = (IIf(idoneita_val(10) <> "", 1, 0))
                Id_SupMinScoperte = (IIf(idoneita_val(11) <> "", 1, 0))

                'Genero la stringa XML
                Call XML_Fabbricato(enum_CodificaDecodifica.Codifica,
                                    StrFabbricato,
                                    Operazione,
                                    CStr(xPiva),
                                    CInt(xSa_Cod),
                                    CInt(xFabbricato_Cod),
                                    Fabbricato_Des,
                                    Indirizzo_Cod,
                                    Tipo_Fabbricato_Cod,
                                    p_Prov,
                                    p_Com,
                                    p_Sezione,
                                    p_Foglio,
                                    p_Numero,
                                    p_Subalterno,
                                    mc_Convenzionale,
                                    mc_Conversione,
                                    mc_Biologico,
                                    Regolamento_Cod,
                                    TitoloPossesso,
                                    Id_Costruzione,
                                    Id_SeparazAmbienti,
                                    Id_SeparazProdotti,
                                    Id_CondIgieniche,
                                    Id_AutorizSanitaria,
                                    Id_HACCP,
                                    Id_Planimetria,
                                    Id_Layout,
                                    Id_DiagrammiFlusso,
                                    Id_CdxM004,
                                    Id_SupMinCoperte,
                                    Id_SupMinScoperte,
                                    Conversione_Inizio,
                                    Conversione_Fine,
                                    CStr(Session("ASG_Utente_CodFiscale").ToString),
                                    Validita_Inizio,
                                    Validita_Fine,
                                    BaseCode,
                                    TopCode,
                                    ProprietarioCapi,
                                    flagMagazzinoFarmaci,
                                    CodiceBDN)


            End If

            If chk_VisibileApp.Checked Then
                Visibile_da_App = 1
            Else
                Visibile_da_App = 0
            End If

            If chk_DefaultOrtofrutta.Checked Then
                Default_Ortofrutta = 1
            Else
                Default_Ortofrutta = 0
            End If

            '------------------------------------------------
            '-----  CODICI  ------------------------------
            '------------------------------------------------

            Dim DTCodici As DataTable
            DTCodici = HttpContext.Current.Session("dt_Codici")

            Dim StrXMLCodici, StrCodice As String
            Dim Id_Cod, Valore As String
            'Dim Start As Int16

            'StrXMLCodici = ""

            For i = 0 To DTCodici.Rows.Count - 1

                'Recupero le informazioni
                Id_Cod = DTCodici.Rows(i).Item("Id_Cod")
                Valore = DTCodici.Rows(i).Item("Val_Cod")
                Dim Validita_Inizio_Codice = AGRODATAINIZIO
                Dim Validita_Fine_Codice = AGRODATAFINE

                If Not IsDBNull(DTCodici.Rows(i).Item("Validita_Inizio")) AndAlso IsDate(DTCodici.Rows(i).Item("Validita_Inizio")) Then
                    Validita_Inizio_Codice = CDate(DTCodici.Rows(i).Item("Validita_Inizio"))
                End If

                If Not IsDBNull(DTCodici.Rows(i).Item("Validita_Fine")) AndAlso IsDate(DTCodici.Rows(i).Item("Validita_Fine")) Then
                    Validita_Fine_Codice = CDate(DTCodici.Rows(i).Item("Validita_Fine"))
                End If

                'Genero l'XML del singolo nodo
                Call XML_Codice(enum_CodificaDecodifica.Codifica,
                                                StrCodice,
                                                enum_TipoOperazioneDB.Scrittura,
                                                Id_Cod,
                                                Valore,
                                                Validita_Inizio_Codice,
                                                Validita_Fine_Codice,
                                                BaseCode,
                                                TopCode,
                                                "Fabbricato")

                'Inserisco l'XML nella stringa complessiva
                StrXMLCodici &= StrCodice

            Next

            'CODICE FABBRICATO VISIBILE DA APP
            If Operazione = enum_TipoOperazioneDB.Modifica Then

                ' la cancellazione va fatta solo sulla modifica se no cancella l'impostazione degli altri fabbricati (5608)
                Call XML_Codice(enum_CodificaDecodifica.Codifica,
                                                StrCodice,
                                                enum_TipoOperazioneDB.Cancellazione,
                                                enum_CodiciAnagrafe.Visibile_da_App,
                                                Visibile_da_App,
                                                AGRODATAINIZIO,
                                                AGRODATAFINE,
                                                BaseCode,
                                                TopCode,
                                                "Fabbricato")

                StrXMLCodici += StrCodice

            End If

            Call XML_Codice(enum_CodificaDecodifica.Codifica,
                                                StrCodice,
                                                enum_TipoOperazioneDB.Scrittura,
                                                enum_CodiciAnagrafe.Visibile_da_App,
                                                Visibile_da_App,
                                                AGRODATAINIZIO,
                                                AGRODATAFINE,
                                                BaseCode,
                                                TopCode,
                                                "Fabbricato")

            StrXMLCodici += StrCodice

            '------------------------------------------------
            '-----  STALLA  ---------------------------------
            '------------------------------------------------

            Dim StrStalla As String
            Dim StrCaratteristica As String
            Dim StrCaratteristiche As String

            Dim StrConfigurazione As String
            Dim StrConfigurazioni As String

            Dim tipo_ricovero As String

            'Dim Att_Cod As Integer
            'Dim Valore As String
            'Dim Start As Integer

            Dim ArrayAnimali() As String
            Dim Gen_Cod As Integer = 0
            Dim Spe_Cod As Integer = 0
            Dim IPro_Cod As Integer = 0
            Dim Cod_Fabb As String = ""
            Dim BDN_Allev_IdFiscale As String = ""
            Dim BDN_Codice_Azienda As String = ""

            Select Case Me.Cmb_TipoFabbricato.SelectedValue

                Case 170 To 179

                    If Me.Cmb_Animali.SelectedItem.Value <> "" Then

                        ArrayAnimali = Split(Cmb_Animali.SelectedItem.Value, "|")
                        Gen_Cod = ArrayAnimali(0)
                        Spe_Cod = ArrayAnimali(1)


                        tipo_ricovero = HttpContext.Current.Session("tipo_ricorvero")
                        BDN_Allev_IdFiscale = TxtIdFiscaleStallaBDN.Text
                        BDN_Codice_Azienda = TxtCodiceStallaBDN.Text


                        'If Not IsNothing(Me.Cmb_TipoRicovero.SelectedItem) Then

                        If tipo_ricovero <> "" Then

                            ArrayAnimali = Split(tipo_ricovero, "|")
                            Gen_Cod = ArrayAnimali(0)
                            Spe_Cod = ArrayAnimali(1)
                            IPro_Cod = ArrayAnimali(2)
                            Cod_Fabb = ArrayAnimali(3)

                        End If

                        'End If


                    End If


                    'If Me.Cmb_Animali.SelectedItem.Text <> "" Then

                    '    ArrayAnimali = Split(Cmb_TipoRicovero.SelectedItem.Value, "|")
                    '    Gen_Cod = ArrayAnimali(0)
                    '    Spe_Cod = ArrayAnimali(1)
                    '    IPro_Cod = ArrayAnimali(2)
                    '    Cod_Fabb = ArrayAnimali(3)

                    XML_Stalla(enum_CodificaDecodifica.Codifica,
                                StrStalla,
                                Operazione,
                                CStr(xPiva),
                                CInt(xSa_Cod),
                                CInt(xFabbricato_Cod),
                                Fabbricato_Des,
                                "0",
                                Validita_Inizio,
                                Validita_Fine,
                                Cod_Fabb,
                                Gen_Cod,
                                Spe_Cod,
                                IPro_Cod,
                                "0",
                                "0",
                                BDN_Codice_Azienda,
                                BDN_Allev_IdFiscale,
                                Validita_Inizio,
                                Validita_Fine,
                                BaseCode,
                                TopCode)


                    '------------------------------------------------
                    '-----  ATTRIBUTI  ------------------------------
                    '------------------------------------------------

                    StrCaratteristiche = ""

                    Dim DT_stalle_car As DataTable
                    DT_stalle_car = HttpContext.Current.Session("dt_Stalle_car")

                    If Not IsNothing(DT_stalle_car) AndAlso DT_stalle_car.Rows.Count > 0 Then


                        For i = 0 To DT_stalle_car.Rows.Count - 1



                            ''Recupero le informazioni
                            'Att_Cod = ListCaratteristiche.Items(Indice).Value
                            'Start = InStr(ListCaratteristiche.Items(Indice).Text, " =", )
                            'Valore = Right(ListCaratteristiche.Items(Indice).Text, ListCaratteristiche.Items(Indice).Text.Length - Start - 2)

                            'Genero l'XML del singolo nodo
                            Call XML_Stalla_Caratteristica(enum_CodificaDecodifica.Codifica,
                                                            StrCaratteristica,
                                                            enum_TipoOperazioneDB.Scrittura,
                                                            DT_stalle_car.Rows(i).Item("ATT_COD"),
                                                            DT_stalle_car.Rows(i).Item("VALORE"),
                                                            Validita_Inizio,
                                                            Validita_Fine,
                                                            BaseCode,
                                                            TopCode)

                            'Inserisco l'XML nella stringa complessiva
                            StrCaratteristiche &= StrCaratteristica

                        Next

                    End If


                    StrConfigurazioni = ""

                    Dim DT_stalle_conf As DataTable
                    DT_stalle_conf = HttpContext.Current.Session("dt_Stalle_conf_BDN_car")

                    If Not IsNothing(DT_stalle_conf) AndAlso DT_stalle_conf.Rows.Count > 0 Then


                        For i = 0 To DT_stalle_conf.Rows.Count - 1



                            'Recupero le informazioni
                            Dim ID = IIf(IsDBNull(DT_stalle_conf.Rows(i)("ID")), 0, DT_stalle_conf.Rows(i)("ID"))
                            Dim CF_Detentore = IIf(IsDBNull(DT_stalle_conf.Rows(i)("CF_Detentore")), "", DT_stalle_conf.Rows(i)("CF_Detentore"))
                            Dim CF_Proprietario = IIf(IsDBNull(DT_stalle_conf.Rows(i)("CF_Proprietario")), "", DT_stalle_conf.Rows(i)("CF_Proprietario"))
                            Dim RagSoc_Detentore = IIf(IsDBNull(DT_stalle_conf.Rows(i)("RagSoc_Detentore")), "", DT_stalle_conf.Rows(i)("RagSoc_Detentore"))
                            Dim RagSoc_Proprietario = IIf(IsDBNull(DT_stalle_conf.Rows(i)("RagSoc_Proprietario")), "", DT_stalle_conf.Rows(i)("RagSoc_Proprietario"))
                            Dim confValidita_Inizio = IIf(IsDBNull(DT_stalle_conf.Rows(i)("Validita_Inizio")), "", DT_stalle_conf.Rows(i)("Validita_Inizio"))
                            Dim confValidita_Fine = IIf(IsDBNull(DT_stalle_conf.Rows(i)("Validita_Fine")), "", DT_stalle_conf.Rows(i)("Validita_Fine"))
                            'Genero(l) 'XML del singolo nodo
                            Call XML_Stalla_Configurazione_BDN(enum_CodificaDecodifica.Codifica,
                                                        StrConfigurazione,
                                                        enum_TipoOperazioneDB.Scrittura,
                                                        ID,
                                                        CF_Detentore,
                                                        CF_Proprietario,
                                                        RagSoc_Detentore,
                                                        RagSoc_Proprietario,
                                                        confValidita_Inizio,
                                                        confValidita_Fine,
                                                        BaseCode,
                                                        TopCode)

                            'Inserisco l'XML nella stringa complessiva
                            StrConfigurazioni &= StrConfigurazione

                        Next

                    End If

            End Select



            '------------------------------------------------
            '----- Costruisco la stringa XML complessiva di inserimento
            '------------------------------------------------

            Dim XmlDoc As New System.Xml.XmlDocument
            Dim XmlDoc2 As New System.Xml.XmlDocument
            Dim XmlDatiFabbricati As System.Xml.XmlElement
            Dim XmlFabbricato As System.Xml.XmlElement
            Dim XmlStalla As System.Xml.XmlElement
            Dim XmlCodice As System.Xml.XmlElement

            Dim StrXmlInserisci As String
            Dim StrXmlCancella As String
            Dim StrXmlCancellaCodice As String


            'Creo il nodo "DatiFabbricati"
            XmlDatiFabbricati = XmlDoc.CreateElement("DatiFabbricati")

            'Inserisco il nodo "Fabbricato"
            XmlDatiFabbricati.InnerXml = StrFabbricato

            'Rendo l'albero figlio del documento
            XmlDoc.AppendChild(XmlDatiFabbricati)

            'Faccio una copia del documento XML
            XmlDoc2 = XmlDoc

            'Seleziono il nodo "Fabbricato"
            XmlFabbricato = XmlDoc.SelectSingleNode("//Fabbricato")

            'Inserisco gli elementi "Indirizzo", "Stalla" come figli del nodo "Fabbricato"
            XmlFabbricato.InnerXml = StrIndirizzo & StrStalla & StrXMLCodici

            'Seleziono il nodo "Stalla" (se ESISTE)
            XmlStalla = XmlDoc.SelectSingleNode("//Stalla")

            If XmlStalla IsNot Nothing Then

                XmlStalla.InnerXml = StrCaratteristiche & StrConfigurazioni

            End If

            'Estraggo la stringa XML complessiva
            StrXmlInserisci = XmlDoc.InnerXml

            '------------------------------------------------------------------------
            '----- Costruisco la stringa XML di cancellazione 
            '----- delle caratteristiche e dei codici
            '------------------------------------------------------------------------


            If Operazione = enum_TipoOperazioneDB.Modifica Then

                If (Session("StrXmlStallaCaratteristicheAttuali") IsNot Nothing AndAlso
                   Session("StrXmlStallaCaratteristicheAttuali").ToString <> "") OrElse
                   (Session("StrXmlStallaConfigurazioniBDN") IsNot Nothing AndAlso
                   Session("StrXmlStallaConfigurazioniBDN").ToString <> "") Then

                    'Seleziono il nodo "Stalla"
                    XmlStalla = XmlDoc2.SelectSingleNode("//Stalla")

                    If XmlStalla IsNot Nothing Then

                        'Rendo la stalla in lettura
                        XmlStalla.SetAttribute("TipoOperazioneDB", CStr(enum_TipoOperazioneDB.Lettura))
                        XmlStalla.InnerXml = ""
                        'Inserisco gli elementi "Stalla_Caratteristica" da cancellare
                        If Session("StrXmlStallaCaratteristicheAttuali") IsNot Nothing AndAlso
                            Session("StrXmlStallaCaratteristicheAttuali").ToString <> "" Then

                            XmlStalla.InnerXml = Session("StrXmlStallaCaratteristicheAttuali")

                        End If

                        'Inserisco gli elementi "Stalla_Caratteristica" da cancellare
                        If Session("StrXmlStallaConfigurazioniBDN") IsNot Nothing AndAlso
                            Session("StrXmlStallaConfigurazioniBDN").ToString <> "" Then

                            XmlStalla.InnerXml = XmlStalla.InnerXml & Session("StrXmlStallaConfigurazioniBDN")

                        End If


                        'Estraggo la stringa XML complessiva
                        StrXmlCancella = XmlDoc2.InnerXml

                        End If

                    End If

                'If Not Session("StrXmlCodici") Is Nothing AndAlso _
                '       Session("StrXmlCodici").ToString <> "" Then

                '    'Seleziono il nodo "Stalla"
                '    XmlCodice = XmlDoc2.SelectSingleNode("//Fabbricato")

                '    If Not XmlCodice Is Nothing Then

                '        'Rendo la stalla in lettura
                '        XmlCodice.SetAttribute("TipoOperazioneDB", CStr(enum_TipoOperazioneDB.Lettura))

                '        'Inserisco gli elementi "Stalla_Cratteristica" da cancellare
                '        XmlCodice.InnerXml = Session("StrXmlCodici")

                '        'Estraggo la stringa XML complessiva
                '        StrXmlCancellaCodice = XmlDoc2.InnerXml

                '    End If

                'End If

                If Session("StrXmlCodiciAttuali") IsNot Nothing AndAlso
                    Session("StrXmlCodiciAttuali").ToString <> "" Then

                    'Seleziono il nodo "Impresa"
                    XmlCodice = XmlDoc2.SelectSingleNode("//Fabbricato")

                    'Rendo l'impresa in lettura
                    XmlCodice.SetAttribute("TipoOperazioneDB", CStr(enum_TipoOperazioneDB.Lettura))

                    'Inserisco gli elementi "Codice" da cancellare
                    XmlCodice.InnerXml = Session("StrXmlCodiciAttuali")

                    'Estraggo la stringa XML complessiva
                    StrXmlCancellaCodice = XmlDoc2.InnerXml

                End If


            End If



            '=======================
            '===  Aggiornamento  ===
            '=======================

            Try

                Dim objFabbricato As New AgronicaCoreAnagrafeBIZ.Fabbricato_W

                Dim NoteLog As String = NOTELOG_ANAGRAFE_BOOTSTRAP


                '---------------------------------------------------------------------------
                '----- Se sono in MODIFICA cancello 
                '---- le caratteristiche della stalla attuali
                '---- e i codici del fabbricato
                '---------------------------------------------------------------------------

                If Operazione = enum_TipoOperazioneDB.Modifica Then

                    If (Session("StrXmlStallaCaratteristicheAttuali") IsNot Nothing AndAlso
                       Session("StrXmlStallaCaratteristicheAttuali").ToString <> "") OrElse
                       (Session("StrXmlStallaConfigurazioniBDN") IsNot Nothing AndAlso
                       Session("StrXmlStallaConfigurazioniBDN").ToString <> "") Then

                        StrDummy = objFabbricato.Fabbricato_Scrivi(CStr(StrXmlCancella),
                                                                    Piva,
                                                                    Sa_Cod, Fabbricato_Cod,
                                                                    objParametri_Server,
                                                                    NoteLog:=NoteLog)

                    End If

                    If Session("StrXmlCodiciAttuali") IsNot Nothing AndAlso
                       Session("StrXmlCodiciAttuali").ToString <> "" Then

                        StrDummy = objFabbricato.Fabbricato_Scrivi(CStr(StrXmlCancellaCodice),
                                                                    Piva,
                                                                    Sa_Cod, Fabbricato_Cod,
                                                                    objParametri_Server,
                                                                    NoteLog:=NoteLog)


                        'StrDummy = objFabbricato.Fabbricato_Scrivi(CStr(Session("ASG_Utente_CodFiscale")), _
                        '                                            CStr(StrXmlCancellaCodice), _
                        '                                            , _
                        '                                            , _
                        '                                            CStr(Session(ASG_.con..._server)))

                    End If

                End If



                '   *   CreateCANCELLATOObject("Agro_Contab.Fabbricati_W")

                '------------------------------------------------
                '----- Effettuo le modifiche al database
                '------------------------------------------------

                'Eseguo i comandi XML
                StrDummy = objFabbricato.Fabbricato_Scrivi(CStr(StrXmlInserisci),
                                                Piva,
                                                Sa_Cod, Fabbricato_Cod,
                                                objParametri_Server,
                                                NoteLog:=NoteLog)



                If Default_Ortofrutta Then
                    Dim OGenerazioni_Anagrafe_Moduli_Log_R As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Moduli_Log_R
                    Dim OGenerazioni_Anagrafe_Log_R As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Log_R
                    Dim OGenerazioni_Anagrafe_Log_W As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Log_W

                    Dim modulo = OGenerazioni_Anagrafe_Moduli_Log_R.Leggi(Piva, 0, 2, 0, 0, -1, -1, -1, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server)

                    If modulo.Rows.Count > 0 Then
                        Dim dtDef_Ort = OGenerazioni_Anagrafe_Log_R.Leggi(Piva, Sa_Cod, -1, 2, 15, 391, AGRODATAINIZIO, AGRODATAFINE, " Key1 = " & Fabbricato_Cod & " ", "", objParametri_Server)
                        If dtDef_Ort.Rows.Count = 0 Then

                            OGenerazioni_Anagrafe_Log_W.Cancella(Piva, Sa_Cod, -1, 2, 15, 391, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                                             0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, objParametri_Server)

                            OGenerazioni_Anagrafe_Log_W.Scrivi(Piva, Sa_Cod, -1, 2, 15, 391, 0, 0,
                                                           Fabbricato_Cod, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                                           DateTime.Now, AGRODATAINIZIO, AGRODATAFINE, 0, 0, 0, 0, objParametri_Server)

                        End If

                    End If


                End If


                'StrDummy = objFabbricato.Fabbricato_Scrivi( _
                '                            CStr(Session("ASG_Utente_CodFiscale")), _
                '                            CStr(StrXmlInserisci), _
                '                            , _
                '                            , _
                '                            CStr(Session(ASG_.con..._server)))

                'Elimino gli oggetti COM+
                objFabbricato = Nothing




                ''§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

                ''------------------------------------------------
                ''----- Validazione dati inseriti
                ''------------------------------------------------

                ''NOTA
                ''La routine seguenti verifica se l'utente dispone dei permessi
                ''di validazione dei dati.
                ''In caso negativo imposta a (-1) il flag di validazione
                ''dell'impresa e invia un messaggio all'Ufficio Segreteria Tecnica

                'Call Gestione_Validazione_Dati(Operazione, _
                '                                Piva, _
                '                                AgroLabel_Fabbricato & " : " _
                '                                & Fabbricato_Des & " ", _
                '                                1, _
                '                                "", _
                '                                Session, _
                '                                Server)

                ''§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§


                '------------------------------------------------
                '----- Conferma di aggiornamento del database
                '------------------------------------------------



                '============================
                '===  Fine Aggiornamento  ===
                '============================
                ''chiudo la transazione
                'AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server)
                ''chiudo la connessione
                'AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)

                'Recupero  la partita IVA dalla querystring
                Piva = xPiva
                'Codifico la partita IVA
                Piva = Stringa_Codifica(Piva, AgroKey_EncoderDecoder, Server)
                'Ritorno alla pagina AlberoImprese
                FlagOK = True


            Catch exc As Exception

                FlagOK = False
                '------------------------------------------------
                'Si e' verificata una eccezione !!!!!!
                '------------------------------------------------

                'chiudo la transazione con il rollback
                If objParametri_Server.objConnessione IsNot Nothing Then
                    If objParametri_Server.objTransazione IsNot Nothing Then
                        'chiudo transazione
                        ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
                    End If
                    'chiudo la connessione
                    ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)
                End If

                'Messaggio di errore
                StrDummy = exc.Message.ToString()

                'Messaggio di errore
                AgroMsgBox(AgronicaAgenda_2010.SiÈVerificatoUnErroreDuranteLaFaseDiSalvat & vbCrLf & StrDummy, Page)
                '------------------------------------------------

            End Try


            If FlagOK Then
                'Dim Piva As String

                'Ritorno alla pagina AlberoImprese
                'Response.Redirect(Qs_PaginaRitorno & "?P=" & Piva)

                Dim Messaggio2 As String
                Messaggio2 = DirectCast(GetLocalResourceObject("FABBRICATOSalvatoConSuccesso"), String) & vbCrLf & vbCrLf

                Dim TargetUrl As String = ""
                Select Case tipo_salva.Value

                    Case 1
                        Messaggio2 += AgronicaAgenda_2010.VerràRicaricataLaPaginaPerInserimento & vbCrLf
                        AgroMsgBox(Messaggio2, Page)

                        Page_Load(Nothing, EventArgs.Empty)
                        clear_form()
                    Case Else

                        Dim objParametriAgenda As New ParametriAgenda

                        If objParametriAgenda.SitoOrigine = Enum_SiteRedirector.GiasNG Then

                            MenuBS_2017_RedirectGestione.RedirectGenerico(objParametriAgenda.Piva,
                                                          Enum_SiteRedirector.GiasNG,
                                                          enum_PagineGiasNG.Pagina_Menu_Anagrafica_Fabbricati,
                                                          TargetUrl,
                                                          objParametri_Server)

                        Else


                            TargetUrl = Master.TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAnagrafica, objParametriAgenda)
                            If Qs_Visibilita <> 0 Then
                                TargetUrl &= "?visibilita=" & CStr(Qs_Visibilita)
                            End If


                        End If

                        Response.Redirect(TargetUrl)
                        AgroMsgBox(Messaggio2, Page, , , "window.location = '" & TargetUrl & "';")

                End Select
            Else
                Messaggi.AgroMsgBox(DirectCast(GetLocalResourceObject("DatiIncompleti"), String), Page, , UpdatePanel_script, , True)
                'Response.Redirect(Request.RawUrl)

            End If

            'If FlagOK = True Then


            '    Dim stringURL As String
            '    stringURL = "AlberoImprese.aspx?P=" & Piva
            '    Response.Redirect(stringURL)

            'End If

        Catch ex As Exception

            Messaggi.AgroMsgBox(ex.Message, Page, , UpdatePanel_script, , True)

        End Try


    End Sub

    Private Sub clear_form()

        TxtDenominazione.Text = ""
        Cmb_TipoFabbricato.ClearSelection()
        Txt_Via.Text = ""
        Txt_Frazione.Text = ""
        Cmb_Provincia.ClearSelection()
        Cmb_Comune.ClearSelection()
        Txt_CAP.Text = ""
        'Txt_Stato.Enabled = False
        cmb_Stato.ClearSelection()
        Txt_Note.Text = ""
        Cmb_TitoloPossesso.ClearSelection()
        Cmb_Particella.ClearSelection()
        Cmb_Regolamenti.ClearSelection()
        TxtValiditaInizio.Text = ""
        TxtValiditaFine.Text = ""
        TxtSup_Convenzionale.Text = ""
        TxtSup_Biologico.Text = ""
        CmbCodice.ClearSelection()
        TxtCodiceValore.Text = ""
        TxtValiditaInizioCodice.Text = ""
        TxtValiditaFineCodice.Text = ""
        TxtCodiceStallaBDN.Text = ""

    End Sub

End Class