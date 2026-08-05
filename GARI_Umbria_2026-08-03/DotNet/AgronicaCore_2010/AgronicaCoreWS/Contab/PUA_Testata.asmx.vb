Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreContabObject
Imports System.IO
Imports System.Text
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider



' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class PUA_Testata
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Pua_Per_Piva(ByVal objP_server As String, ByVal piva As String) As RispostaStandard
        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim ddl_PUA As New DropDownList
            AgronicaCoreUtility.CaricaListControl.PUA(ddl_PUA, False, "", "",
                                                      piva, "", " PUA_Anno DESC ", objParametri_Server)

            Dim JArrayListaOp As New JArray()

            For Each li As ListItem In ddl_PUA.Items
                JArrayListaOp.Add(New JObject(New JProperty("pua_cod", li.Value), New JProperty("nome", li.Text)))
            Next

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            r.RispostaStringa = JsonConvert.SerializeObject(JArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function VaiAlPianoDistribuzione(ByVal objP_server As String, ByVal objP_utenti As String, ByVal pua_cod As Integer, ByVal regolamento_cod As Integer, ByVal piva As String) As RispostaStandard
        Dim r As New RispostaStandard()

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

            'Controllo che mi sia tornato un id del piano concimazione
            If IsNothing(pua_cod) OrElse Not IsNumeric(pua_cod) OrElse pua_cod = 0 Then
                Throw New Exception("Errore. PUA corrotto")
            End If

            'Leggo i dati di testata
            Dim objTestata As New AgronicaCorePUA_DAL.PUA_Testata_R
            Dim dtTestata As DataTable = objTestata.Leggi_xGriglia(piva, pua_cod, regolamento_cod, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server)

            If IsNothing(dtTestata) OrElse dtTestata.Rows.Count <> 1 Then
                Throw New Exception("Errore. Testata PUA non trovata")
            End If

            Dim pua_tipo As String = dtTestata.Rows(0).Item("PC_Tipo")
            Dim testata_des As String = dtTestata.Rows(0).Item("PC_Testata_Des")
            Dim data_inizio As String = dtTestata.Rows(0).Item("Validita_Inizio")
            Dim data_fine As String = dtTestata.Rows(0).Item("Validita_Fine")
            Dim blocco_flag As String = dtTestata.Rows(0).Item("blocco_flag")

            Dim urlTarget As String

            Dim objRicette As New AgronicaCoreContabDAL.Ricette_R
            Dim dtPiani As DataTable = objRicette.Leggi_xGriglia(0, piva, 0, enum_TipoRicetta.PianoDistribuzionePua,
                                                                 0, pua_cod, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server)

            If Not IsNothing(dtPiani) AndAlso dtPiani.Rows.Count > 0 Then

                urlTarget = "PUA_Piano_Distribuzione.aspx?" &
                    "tipo=" & Stringa_Codifica(pua_tipo, AgroKey_EncoderDecoder, objParametri_Server) &
                    "&o=" & Stringa_Codifica(enum_TipoOperazioneDB.Modifica, AgroKey_EncoderDecoder, objParametri_Server) &
                    "&p=" & Stringa_Codifica(piva, AgroKey_EncoderDecoder, objParametri_Server) &
                    "&q=" & Stringa_Codifica(pua_cod, AgroKey_EncoderDecoder, objParametri_Server) &
                    "&r=" & Stringa_Codifica(regolamento_cod, AgroKey_EncoderDecoder, objParametri_Server) &
                    "&data_da=" & Stringa_Codifica(CStr(data_inizio), AgroKey_EncoderDecoder, objParametri_Server) &
                    "&data_a=" & Stringa_Codifica(CStr(data_fine), AgroKey_EncoderDecoder, objParametri_Server) &
                    "&blocco_flag=" & Stringa_Codifica(CStr(blocco_flag), AgroKey_EncoderDecoder, objParametri_Server) &
                    "&ric=" & Stringa_Codifica(dtPiani.Rows(0).Item("Ricetta_Cod"), AgroKey_EncoderDecoder, objParametri_Server)

            Else

                Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_CodiciGiasPRO_R
                Dim dt As DataTable = objUtenti.Leggi("", "", objParametri_Utenti)
                Dim progressivoGias As Integer = dt.Rows(0).Item("ProgressivoGIAS")

                Dim BaseCode As Integer = 0
                Dim TopCode As Integer = 0
                UtilityProvider.Calcola_BaseCode_TopCode(BaseCode, TopCode, progressivoGias)

                Dim XmlDoc As New System.Xml.XmlDocument

                Dim XmlDatiRicetta As System.Xml.XmlElement = XmlDoc.CreateElement("DatiRicetta")

                '----- < RICETTA > -----
                Dim XmlRicetta As System.Xml.XmlElement = XmlDoc.CreateElement("Ricetta")
                XmlDatiRicetta.AppendChild(XmlRicetta)

                With XmlRicetta
                    .SetAttribute("TipoOperazioneDB", enum_TipoOperazioneDB.Scrittura)
                    .SetAttribute("ricetta_cod", 0)
                    .SetAttribute("ricetta_numero", pua_cod & "_1")
                    .SetAttribute("piva", piva)
                    .SetAttribute("sa_cod", 0)
                    .SetAttribute("tipo_ricetta", enum_TipoRicetta.PianoDistribuzionePua)
                    .SetAttribute("ricetta_des", "Piano Distribuzione (" & testata_des & ")")
                    .SetAttribute("ricetta_des_long", "Piano Distribuzione (" & testata_des & ")")
                    .SetAttribute("veg_cod", 0)
                    .SetAttribute("note", "")
                    .SetAttribute("basecode", BaseCode.ToString)
                    .SetAttribute("topcode", TopCode.ToString)
                    .SetAttribute("programmazione_cod", pua_cod)
                    .SetAttribute("validita_inizio", data_inizio)
                    .SetAttribute("validita_fine", data_fine)
                End With

                XmlDoc.AppendChild(XmlDatiRicetta)

                Dim OUTPUT_Ricetta_Cod As Integer = 0
                Dim objW As New AgronicaCoreContabBIZ.Ricette_W
                Dim xRisp As Boolean = objW.Ricetta_Scrivi(XmlDoc.InnerXml, OUTPUT_Ricetta_Cod, objParametri_Server)

                If xRisp = False OrElse OUTPUT_Ricetta_Cod = 0 Then
                    Throw New Exception("Errore. Impossibile creare Piano Distribuzione PUA")
                End If

                urlTarget = "PUA_Piano_Distribuzione.aspx?" &
                    "tipo=" & Stringa_Codifica(pua_tipo, AgroKey_EncoderDecoder, objParametri_Server) &
                    "&o=" & Stringa_Codifica(enum_TipoOperazioneDB.Scrittura, AgroKey_EncoderDecoder, objParametri_Server) &
                    "&p=" & Stringa_Codifica(piva, AgroKey_EncoderDecoder, objParametri_Server) &
                    "&q=" & Stringa_Codifica(pua_cod, AgroKey_EncoderDecoder, objParametri_Server) &
                    "&r=" & Stringa_Codifica(regolamento_cod, AgroKey_EncoderDecoder, objParametri_Server) &
                    "&data_da=" & Stringa_Codifica(CStr(data_inizio), AgroKey_EncoderDecoder, objParametri_Server) &
                    "&data_a=" & Stringa_Codifica(CStr(data_fine), AgroKey_EncoderDecoder, objParametri_Server) &
                    "&blocco_flag=" & Stringa_Codifica(CStr(blocco_flag), AgroKey_EncoderDecoder, objParametri_Server) &
                    "&ric=" & Stringa_Codifica(OUTPUT_Ricetta_Cod, AgroKey_EncoderDecoder, objParametri_Server)

            End If

            r.RispostaOK = True
            r.RispostaStringa = urlTarget

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function


End Class