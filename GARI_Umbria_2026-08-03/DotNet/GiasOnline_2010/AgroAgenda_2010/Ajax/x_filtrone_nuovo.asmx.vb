Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility
Imports AgronicaCoreUtility.CaricaListControl
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json
Imports AgronicaCoreDataProvider.TipiEnumerativi

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
Public Class x_filtrone_nuovo
    Inherits System.Web.Services.WebService


    <Script.Services.ScriptMethod()> <WebMethod(EnableSession:=True)>
    Public Function DropDownG2GInfoConfigSelezionata(G2GLocalConfigurazioni_COD As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim SonoLoggatoComeSuperUser As Boolean =
                (objParametri_Server.SuperUserUsername = objParametri_Server.UtenteUsername)

            Dim objR As New AgronicaCoreG2GLocalDal.G2GLocal_R
            Dim dt As DataTable = objR.LeggiConfigurazioni(G2GLocalConfigurazioni_COD, objParametri_Server)

            Dim conteggioImpianti As Integer = 0
            Dim conteggioImprese As Integer = 0

            Dim xDoc As XDocument = XDocument.Parse(dt(0)("G2GLocalConfigurazioni_CFG"))
            Dim ns1 As XNamespace = "http://G2G"
            Dim jSonListaImpianti As String = xDoc.Element(ns1 + "dati").Element(ns1 + "imprese").Element(ns1 + "filtronerisultato").Value()
            Dim jSonListaImprese As String = xDoc.Element(ns1 + "dati").Element(ns1 + "imprese").Element(ns1 + "filtronerisultato_azienda").Value()

            Dim msgAdd As String = ""


            If Not String.IsNullOrEmpty(jSonListaImpianti) Then
                Dim o As JObject = JObject.Parse(jSonListaImpianti)
                conteggioImpianti = o("chiavi").Count()
                Dim tipoElemento As String = o("tipo")
                msgAdd &= conteggioImpianti & " elementi di tipo " & tipoElemento & "."
            End If

            If Not String.IsNullOrEmpty(jSonListaImprese) Then
                Dim o As JObject = JObject.Parse(jSonListaImprese)
                conteggioImprese = o("chiavi").Count()
                msgAdd &= conteggioImprese & " imprese."
            End If

            If SonoLoggatoComeSuperUser Then
                Dim oUtenti As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R
                Dim uMod As String = dt(0)("Username_Modifica").ToString()
                Dim dtU As DataTable =
                    oUtenti.Leggi("", 0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, " codFisc = '" & uMod & "'", "", objParametri_Utenti)

                Dim utenteModificatore As String = ""

                If dtU.Rows.Count > 0 Then
                    If (dtU(0)("username") = objParametri_Server.SuperUserUsername) Then
                        utenteModificatore &= " dal Super-User"
                    Else
                        utenteModificatore &= dtU(0)("Rag_Soc") & " " & dtU(0)("Nome") & " " & dtU(0)("Cognome")
                    End If
                Else
                    If (uMod.ToString().ToLower() = "agronica") Then
                        utenteModificatore &= " dal Super-User"
                    End If
                End If


                msgAdd &= " Ultima modifica configurazione il " & CType(dt(0)("Data_Modifica"), Date).ToShortDateString() & " effettuta da " & utenteModificatore
            End If


            r.RispostaOK = True
            r.RispostaStringa = "La configurazione """ & dt(0)("G2GLocalConfigurazioni_Des") & """ gestisce il trasferimento di " & msgAdd

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r
    End Function


    <Script.Services.ScriptMethod()> <WebMethod(EnableSession:=True)>
    Public Function GetG2G() As String

        If IsNothing(System.Web.HttpContext.Current.Cache("GetG2G")) Then

            Dim objParametri_server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

            Dim objR As New AgronicaCoreG2GLocalDal.G2GLocal_R
            Dim dt As DataTable = objR.LeggiConfigurazioni(0, objParametri_server)

            Dim lista As New List(Of String)
            For Each dr As DataRow In dt.Rows
                lista.Add("{""G2GLocalConfigurazioni_DES"":""" & jSon.Escape(dr.Item("G2GLocalConfigurazioni_DES")) & """, ""G2GLocalConfigurazioni_COD"":""" & dr.Item("G2GLocalConfigurazioni_COD") & """}")
            Next

            Dim strRisp As String = "[" & String.Join(",", lista) & "]"

            System.Web.HttpContext.Current.Cache("ListaRegioni") = strRisp

            Return strRisp
        Else
            Return System.Web.HttpContext.Current.Cache("ListaRegioni")
        End If

    End Function

    <Script.Services.ScriptMethod()> <WebMethod(EnableSession:=True)>
    Public Function GetStati() As String

        'If IsNothing(System.Web.HttpContext.Current.Cache("GetStati")) Then

        Dim objParametri_server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim objR As New AgronicaCoreMetaSchemaDAL.ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166_R
        Dim dt As DataTable = objR.Leggi("", "", "descrizione", objParametri_server)

        Dim lista As New List(Of String)
        For Each dr As DataRow In dt.Rows
            lista.Add("{""des"":""" & jSon.Escape(dr.Item("descrizione")) & """, ""val"":""" & dr.Item("codice") & """}")
        Next

        Dim strRisp As String = "[" & String.Join(",", lista) & "]"

        'System.Web.HttpContext.Current.Cache("GetStati") = strRisp

        Return strRisp

        'Else

        'Return System.Web.HttpContext.Current.Cache("GetStati")

        'End If

    End Function

    <Script.Services.ScriptMethod()> <WebMethod(EnableSession:=True)>
    Public Function GetRegioni(ByVal valori_stati As String) As String

        If IsNothing(System.Web.HttpContext.Current.Cache("GetRegioni")) Then

            Dim objParametri_server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

            Dim objR As New AgronicaCoreMetaSchemaDAL.Lista_Regioni_R
            Dim FiltroAggiuntivo As String = ""

            If Trim(valori_stati) <> "" Then
                'Formattazione
                valori_stati = "'" & Replace(valori_stati, ",", "','") & "'"
                FiltroAggiuntivo = "Stato_Country In (" & valori_stati & ")"
            End If


            Dim dt As DataTable = objR.Leggi("", FiltroAggiuntivo, "", objParametri_server, "")

            Dim lista As New List(Of String)
            For Each dr As DataRow In dt.Rows
                lista.Add("{""des"":""" & jSon.Escape(dr.Item("regione_des")) & """, ""val"":""" & dr.Item("reg") & """}")
            Next

            Dim strRisp As String = "[" & String.Join(",", lista) & "]"

            System.Web.HttpContext.Current.Cache("ListaRegioni") = strRisp

            Return strRisp
        Else
            Return System.Web.HttpContext.Current.Cache("ListaRegioni")
        End If

    End Function

    <Script.Services.ScriptMethod()> <WebMethod(EnableSession:=True)>
    Public Function GetZone() As String

        If IsNothing(System.Web.HttpContext.Current.Cache("GetZone")) Then

            Dim objParametri_server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

            Dim objR As New AgronicaCoreAnagrafeDAL.Zone_R
            Dim dt As DataTable = objR.Leggi(0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_server)

            Dim lista As New List(Of String)
            For Each dr As DataRow In dt.Rows
                lista.Add("{""des"":""" & jSon.Escape(dr.Item("Descrizione")) & """, ""val"":""" & dr.Item("Zona_Cod") & """}")
            Next

            Dim strRisp As String = "[" & String.Join(",", lista) & "]"

            System.Web.HttpContext.Current.Cache("GetZone") = strRisp

            Return strRisp
        Else
            Return System.Web.HttpContext.Current.Cache("GetZone")
        End If

    End Function


    <Script.Services.ScriptMethod()> <WebMethod(EnableSession:=True)>
    Public Function GetProvincie(ByVal valori_regioni As String) As String

        Dim strP As String = "GetProvincie_" & valori_regioni
        If IsNothing(System.Web.HttpContext.Current.Cache(strP)) Then

            Dim objParametri_server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

            Dim objR As New AgronicaCoreMetaSchemaDAL.Lista_Province_R
            Dim FiltroAggiuntivo As String = ""

            If Trim(valori_regioni) <> "" Then
                'Formattazione
                valori_regioni = "'" & Replace(valori_regioni, ",", "','") & "'"
                FiltroAggiuntivo = "Lista_Province.REG In (" & valori_regioni & ")"
            End If

            Dim dt As DataTable = objR.Leggi("", "", AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                             FiltroAggiuntivo, "", objParametri_server, "")

            Dim lista As New List(Of String)
            For Each dr As DataRow In dt.Rows
                lista.Add("{""regione_des"":""" & jSon.Escape(dr.Item("regione_des")) & """, ""provincia"":""" & jSon.Escape(dr.Item("provincia")) & """, ""prov"":""" & dr.Item("prov") & """}")
            Next

            Dim strRisp As String = "[" & String.Join(",", lista) & "]"

            System.Web.HttpContext.Current.Cache(strP) = strRisp

            Return strRisp
        Else
            Return System.Web.HttpContext.Current.Cache(strP)
        End If

    End Function


    <Script.Services.ScriptMethod()> <WebMethod(EnableSession:=True)>
    Public Function GetComuni(ByVal valori_provincia As String) As String

        Dim objParametri_server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        'Dim listaProv As String() = valori_provincia.Split(",")
        'For i = 0 To listaProv.Length - 1
        '    listaProv(i) = "'" & listaProv(i) & "'"
        'Next
        'valori_provincia = String.Join(",", listaProv)

        Dim objR As New AgronicaCoreMetaSchemaDAL.Istat_R
        'Dim dt As DataTable = objR.Leggi("", "", "", "", "", AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, " prov in(" & valori_provincia & ")", "", objParametri_server)

        Dim FiltroAggiuntivo As String = ""
        If Trim(valori_provincia) <> "" Then
            'Formattazione
            valori_provincia = "'" & Replace(valori_provincia, ",", "','") & "'"
            FiltroAggiuntivo = "Lista_Province.prov In(" & valori_provincia & ")"
        End If

        Dim dt As DataTable = objR.ISTATXListaProvince("", "", "", "", "", "", "", "", FiltroAggiuntivo, "", objParametri_server)

        Dim lista As New List(Of String)
        'lista.Add("{""provincia_des"":""" & "nessuno" & """, ""descrizione"":""" & "nessuno" & """, ""cod_istat"":""" & "" & """}")
        For Each dr As DataRow In dt.Rows
            lista.Add("{""provincia_des"":""" & jSon.Escape(dr.Item("provincia")) & """, ""descrizione"":""" & jSon.Escape(dr.Item("localita")) & """, ""cod_istat"":""" & dr.Item("PROV") & "_" & dr.Item("com_localita") & """}")
        Next

        Dim strRisp As String = "[" & String.Join(",", lista) & "]"

        Return strRisp

    End Function

    <Script.Services.ScriptMethod()> <WebMethod(EnableSession:=True)>
    Public Function GetSpecie(ByVal Gru_Cod As String) As String
        'Dim strP As String = "GetSpecie" & Gru_Cod
        'If IsNothing(System.Web.HttpContext.Current.Cache(strP)) Then

        Dim objParametri_server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Dim str_filtoro As String = If(Gru_Cod = "0", "", " SpecieVegetali.Gru_Cod in (" & Gru_Cod & ") ")

        Dim cblSpecie As New DropDownList
        CaricaCheckBoxList_SpecieVegetale_Optimize(cblSpecie, 0, True, str_filtoro, "", 0, 0, objParametri_server, objParametri_Utenti)

        Dim lista As New List(Of String)
        For Each item As ListItem In cblSpecie.Items
            lista.Add("{""codice"":""" & item.Value & """, ""descrizione"":""" & jSon.Escape(item.Text) & """}")
        Next

        Dim strRisp As String = "[" & String.Join(",", lista) & "]"

        'System.Web.HttpContext.Current.Cache(strP) = strRisp
        Return strRisp
        'Else
        'Return System.Web.HttpContext.Current.Cache(strP)
        'End If
    End Function


    <Script.Services.ScriptMethod()> <WebMethod(EnableSession:=True)>
    Public Function GetCultivar(ByVal Veg_Cod As String) As String
        'Dim strP As String = "GetCultivar" & Veg_Cod
        'If IsNothing(System.Web.HttpContext.Current.Cache(strP)) Then

        Dim objParametri_server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Dim CBL_Cultivar As New DropDownList
        Dim strFiltro As String = " SpecieVegetali.Veg_cod IN (" & Veg_Cod & ") "
        'Cultivar(CBL_Cultivar, False, "", "", Veg_Cod, 0, "", True, 0, 0, "", "", objParametri_server, objParametri_Utenti)
        Cultivar(CBL_Cultivar, False, "", "", 0, 0, "", True, 0, 0, strFiltro, "", objParametri_server, objParametri_Utenti)

        Dim lista As New List(Of String)
        'For Each item As ListItem In CBL_Cultivar.Items
        '    lista.Add("{""codice"":""" & item.Value & """, ""descrizione"":""" & jSon.Escape(item.Text) & """}")
        'Next

        Dim objMatrice As New AgronicaCoreMetaSchemaDAL.Cultivar_R
        Dim Dt As DataTable = objMatrice.GestioneFiltroUtente_Leggi(0, 0, "", AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                                    strFiltro, "", objParametri_Utenti)

        For Each dr As DataRow In Dt.Rows
            lista.Add("{""codice"":""" & dr.Item("Cul_Cod") & """, ""descrizione"":""" & jSon.Escape(dr.Item("Cul_Des")) & """, ""veg_des"":""" & jSon.Escape(dr.Item("Veg_Des")) & """}")
        Next

        Dim strRisp As String = "[" & String.Join(",", lista) & "]"

        'System.Web.HttpContext.Current.Cache(strP) = strRisp
        Return strRisp
        'Else
        'Return System.Web.HttpContext.Current.Cache(strP)
        'End If
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function GetGruppoOperazioni() As String
        'Dim strP As String = "GetGruppoOperazioni"
        'If IsNothing(System.Web.HttpContext.Current.Cache(strP)) Then

        Dim objParametri_server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim objGrOp As New AgronicaCoreMetaSchemaDAL.GruppoOperazioni_R
        Dim Dt As DataTable = objGrOp.Leggi(0, "",
                                0, "", True, False, False, False, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                "(GOper.GRU_COD = 1 OR GOper.GRU_COD = 2 OR GOper.GRU_COD = 3 OR GOper.GRU_COD = 4 ) ", "", objParametri_server)

        Dim lista As New List(Of String)
        For Each dr As DataRow In Dt.Rows
            lista.Add("{""codice"":""" & dr.Item("Gru_Cod") & """, ""descrizione"":""" & jSon.Escape(dr.Item("Gru_des")) & """}")
        Next

        Dim strRisp As String = "[" & String.Join(",", lista) & "]"

        Return strRisp
        'Else
        'Return System.Web.HttpContext.Current.Cache(strP)
        'End If
    End Function


    <Script.Services.ScriptMethod()> <WebMethod(EnableSession:=True)>
    Public Function GetOperazioni(ByVal Gruppo_Operazioni As String) As String
        'Dim strP As String = "GetOperazioni" & Gruppo_Operazioni
        'If IsNothing(System.Web.HttpContext.Current.Cache(strP)) Then

        Dim objParametri_server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim objOperazioni As New AgronicaCoreMetaSchemaDAL.GruppoOperazioni_R
        Dim listaPerGruCod As New List(Of String)
        Dim listaGenerale As New List(Of String)

        Dim listaGruCod As String() = Gruppo_Operazioni.Split(",")
        For Each gru_cod As String In listaGruCod

            Dim gru_des = objOperazioni.GruOper_Des_from_GruOper_Cod(gru_cod, objParametri_server)
            Dim ddl As New DropDownList
            Operazioni(ddl, False, "", "", 0, 0, gru_cod, "", "", objParametri_server)

            For Each item As ListItem In ddl.Items
                listaPerGruCod.Add("{""gruppo"":""" & jSon.Escape(gru_des) & """, ""codice"":""" & item.Value & """, ""descrizione"":""" & jSon.Escape(item.Text) & """}")
            Next

            listaGenerale.Add(String.Join(",", listaPerGruCod))

        Next

        Dim strRisp As String = "[" & String.Join(",", listaGenerale) & "]"

        'System.Web.HttpContext.Current.Cache(strP) = strRisp
        Return strRisp
        'Else
        'Return System.Web.HttpContext.Current.Cache(strP)
        'End If

    End Function



    <Script.Services.ScriptMethod()> <WebMethod(EnableSession:=True)>
    Public Function GetCategoriaProdotto() As String
        'Dim strP As String = "GetCategoriaProdotto"
        'If IsNothing(System.Web.HttpContext.Current.Cache(strP)) Then

        Dim objParametri_server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim CBL_prodotto As New DropDownList
        CategorieMagazzino(CBL_prodotto, True, "", "", 0, CAU_SCARICO, 0, False, False, "", "", objParametri_server)

        Dim lista As New List(Of String)
        For Each item As ListItem In CBL_prodotto.Items
            lista.Add("{""codice"":""" & item.Value & """, ""descrizione"":""" & jSon.Escape(item.Text) & """}")
        Next

        Dim strRisp As String = "[" & String.Join(",", lista) & "]"

        'System.Web.HttpContext.Current.Cache(strP) = strRisp
        Return strRisp
        'Else
        'Return System.Web.HttpContext.Current.Cache(strP)
        'End If

    End Function

    <Script.Services.ScriptMethod()> <WebMethod(EnableSession:=True)>
    Public Function GetRicercaProdotti(ByVal tipologia As String, ByVal str_ricerca As String) As String


        Dim objParametri_server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim CBL_prodotto As New DropDownList

        ProdottiAnagrafica(CBL_prodotto,
                                        False, "", "",
                                        AGRODATAFINE,
                                        tipologia,
                                        str_ricerca,
                                        False,
                                        False,
                                        "",
                                        objParametri_server)

        Dim lista As New List(Of String)
        For Each item As ListItem In CBL_prodotto.Items
            lista.Add("{""codice"":""" & item.Value & """, ""descrizione"":""" & AgronicaCoreUtility.jSon.Escape(item.Text) & """}")
        Next

        Dim strRisp As String = "[" & String.Join(",", lista) & "]"

        Return strRisp
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function GetValoriIdCod(ByVal IdCod As Integer, TipoEntita As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim objCodiceAn As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R
            Dim Dt_Padri As DataTable =
                objCodiceAn.LeggiValCodDatoIdCod(IdCod, TipoEntita, False, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server)

            Dim jArrayListaOp As New JArray()

            For Each dr As DataRow In Dt_Padri.Rows

                jArrayListaOp.Add(New JObject(New JProperty("text", dr.Item("val_Cod")),
                                              New JProperty("value", dr.Item("val_cod"))))


            Next

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(jArrayListaOp, Formatting.None)

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r
    End Function


    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function GetCodici(ByVal tipo As String) As String

        Dim objParametri_server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objCodiceAn As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R
        Dim strFiltro As String = ""

        If tipo = "azienda" Then
            strFiltro = "("
            strFiltro += objCodiceAn.Filtro_Codici_Anagrafe(1, 3, 2, objParametri_server)
            strFiltro = Replace(strFiltro, " Or Codice = 1016", "")
            strFiltro = Replace(strFiltro, "Codice = 1016 Or ", "")
            strFiltro = Replace(strFiltro, " Or Codice = 1088", "")
            strFiltro = Replace(strFiltro, " Codice = 1088", "")
            strFiltro += ") OR ( UPPER(gruppo) = 'CENTRO')"
        End If

        'If tipo = "centro" Then
        '    strFiltro = " UPPER(gruppo) = 'CENTRO' "
        'End If

        If tipo = "impianto" Then
            strFiltro = "("
            strFiltro += objCodiceAn.Filtro_Codici_Anagrafe(3, 3, 2, objParametri_server)
            strFiltro += ") OR ( "
            strFiltro += objCodiceAn.Filtro_Codici_Anagrafe(4, 3, 2, objParametri_server)
            strFiltro = Replace(strFiltro, " Or Codice = 1016", "")
            strFiltro = Replace(strFiltro, "Codice = 1016 Or ", "")
            strFiltro = Replace(strFiltro, " Or Codice = 1018", "")
            strFiltro = Replace(strFiltro, "Codice = 1018 Or ", "")
            strFiltro = Replace(strFiltro, " Or Codice = 1108", "")
            strFiltro = Replace(strFiltro, "Codice = 1108 Or ", "")
            strFiltro += ") "
        End If

        'If tipo = "impianto" Then
        '    strFiltro = objCodiceAn.Filtro_Codici_Anagrafe(4, 3, 2, objParametri_server)
        '    strFiltro = Replace(strFiltro, " Or Codice = 1016", "")
        '    strFiltro = Replace(strFiltro, "Codice = 1016 Or ", "")
        '    strFiltro = Replace(strFiltro, " Or Codice = 1018", "")
        '    strFiltro = Replace(strFiltro, "Codice = 1018 Or ", "")
        '    strFiltro = Replace(strFiltro, " Or Codice = 1108", "")
        '    strFiltro = Replace(strFiltro, "Codice = 1108 Or ", "")
        'End If





        Dim DT As DataTable = objCodiceAn.Leggi(0, "", AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, strFiltro, "", objParametri_server)

        Dim lista As New List(Of String)
        For Each dr As DataRow In DT.Rows
            Dim gruppo As String = ""

            If tipo = "azienda" Then
                gruppo = If(dr.Item("gruppo") = "CENTRO", "CENTRO", "IMPRESA")
            End If
            If tipo = "impianto" Then
                Select Case dr.Item("codice")
                    Case enum_CodiciAnagrafe.Appezzamento_CodiceContratto, 1132
                        gruppo = "APPEZZA"
                    Case Else
                        gruppo = "IMPIANTO"
                End Select
            End If

            lista.Add("{""codice"":""" & dr.Item("codice") & """, ""descrizione"":""" & dr.Item("descrizione") & """,""gruppo"":""" & gruppo & """}")
        Next

        Dim strRisp As String = "[" & String.Join(", ", lista).Replace("'", "'") & "]"

        Return strRisp

    End Function

    <Script.Services.ScriptMethod()> <WebMethod(EnableSession:=True)>
    Public Function GetOrganismiReferenti() As String

        Dim objParametri_server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim objR As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R
        Dim dt As DataTable = objR.Leggi_OrgReferente_SuImpianto_SenzaDistinta("", 0, 0, 0, 0, "", AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_server)

        Dim lista As New List(Of String)
        For Each dr As DataRow In dt.Rows
            lista.Add("{""des"":""" & jSon.Escape(dr.Item("rag_soc")) & """, ""val"":""" & dr.Item("val_cod") & """}")
        Next

        lista = lista.Distinct().ToList

        Dim strRisp As String = "[" & String.Join(",", lista) & "]"

        Return strRisp

    End Function

    <Script.Services.ScriptMethod()> <WebMethod(EnableSession:=True)>
    Public Function GetRiferimentoTrasferimentoDati() As String

        Dim objParametri_server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim objR As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R
        Dim dt As DataTable = objR.Leggi_Contatti_SuImpianto_SenzaDistinta(TipiEnumerativi.enum_CodiciAnagrafe.Riferimento_Trasferimento_Dati,
                                                                           "", 0, 0, 0, 0, "", AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_server)

        Dim lista As New List(Of String)
        For Each dr As DataRow In dt.Rows
            lista.Add("{""des"":""" & jSon.Escape(dr.Item("rag_soc")) & """, ""val"":""" & dr.Item("val_cod") & """}")
        Next

        lista = lista.Distinct().ToList

        Dim strRisp As String = "[" & String.Join(",", lista) & "]"

        Return strRisp

    End Function


    <Script.Services.ScriptMethod()> <WebMethod(EnableSession:=True)>
    Public Function FiltrinoProsegui(ByVal piva As String, ByVal parametri As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        Dim objParametriAgenda As New AgronicaCoreModello.ParametriAgenda_Temp.ParametriAgenda()

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim linguaCorrente As Lingua = CType(HttpContext.Current.Session("LinguaCorrente"), Lingua)

        Dim redirectManager As New AgroRedirectManager(linguaCorrente, objParametri_Server, HttpContext.Current.Session)

        Dim parametriIniziali = JObject.Parse(parametri)
        Dim outLinkRedirect As String = ""
        Dim outOpenScript As String = ""
        Dim outDtSelected As DataTable = Nothing

        Try
            If parametriIniziali.Value(Of Integer)("Sito_Destinazione") = TipiEnumerativi.Enum_SiteRedirector.Sito_AgronicaAgenda_2010 Then

                redirectManager.RedirectStessoSito(piva, parametriIniziali.Value(Of String)("Pagina_Destinazione"), outLinkRedirect)

            Else
                redirectManager.aprisitoversione2013(
                    piva,
                    parametriIniziali.Value(Of Integer)("Sito_Destinazione"),
                    parametriIniziali.Value(Of String)("Pagina_Destinazione"), 'TODO
                    parametriIniziali.Value(Of String)("Pagina_Destinazione"),
                    outLinkRedirect,
                    outOpenScript,
                    outDtSelected
                )
                'La funzione restituisce uno script js, facendo così ne ricavo la solo URL
                Dim linkSegments = outLinkRedirect.Split("'")
                If linkSegments.Length > 1 Then
                    outLinkRedirect = outLinkRedirect.Split("'")(3)
                End If
            End If

            'If r.RispostaOK = True Then
            r.RispostaOK = True
            r.RispostaStringa = outLinkRedirect
            objParametriAgenda.Piva = piva 'In questo modo imposto il valore in sessione
            objParametriAgenda.RagSoc = "" 'Forzo la rilettura della ragione sociale per la nuova azienda
            ' VAnni: 6/2/2019: imposto la p.iva per il GIS (in questo modo anche da nuovo menu viene mantenuta la selezione).
            HttpContext.Current.Session("_Piva") = piva
            'Else
            '    r.RispostaStringa = "Problema nel salvataggio dei parametri filtro"
            'End If

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, False, source:=True)

        End Try

        Return r

    End Function

End Class