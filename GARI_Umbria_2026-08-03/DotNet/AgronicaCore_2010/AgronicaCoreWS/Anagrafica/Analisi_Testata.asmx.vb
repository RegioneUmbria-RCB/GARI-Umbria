Imports System.Web.Services
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDTOStd.InData.Anagrafica
Imports AgronicaCorePianoConcimazioneBIZ
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq


' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class Analisi_Testata
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Analisi_Testata_Per_Piva(ByVal objP_server As String, ByVal piva As String) As RispostaStandard
        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim ddl_Analisi As New DropDownList
            AgronicaCoreUtility.CaricaListControl.Analisi_Terreno(ddl_Analisi, False, "", "",
                                                             piva, "", "", objParametri_Server)

            Dim JArrayListaOp As New JArray()

            For Each li As ListItem In ddl_Analisi.Items
                JArrayListaOp.Add(New JObject(New JProperty("analisi_testata_cod", li.Value), New JProperty("nome", li.Text)))
            Next

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
    Public Function Leggi_Analisi_Testata_Per_Piva_NG(InData As Object) As RispostaStandard
        Dim r As New RispostaStandard()

        InData = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData))

        Dim piva As String = InData.InData

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If


        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim ddl_Analisi As New DropDownList
            AgronicaCoreUtility.CaricaListControl.Analisi_Terreno(ddl_Analisi, False, "", "",
                                                             piva, "", "", objParametri_Server)

            Dim JArrayListaOp As New JArray()

            For Each li As ListItem In ddl_Analisi.Items
                JArrayListaOp.Add(New JObject(New JProperty("analisi_testata_cod", li.Value), New JProperty("nome", li.Text)))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(JArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function
    ''' <summary>
    ''' filtroParticelle = jSon di List(Of AgronicaCorePianoConcimazioneBIZ.PUA_ParticellaVincoloAgronomico)
    ''' </summary>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Analisi_Testata_Filtrata_NG(ByVal InData As CoreWS_Generic(Of Leggi_Analisi_Testata_Filtrata)) As RispostaStandard

        Dim r As New RispostaStandard()

        Dim objAnalisi As New AgronicaCoreAnagrafeDAL.Analisi_Testata_R
        Dim filtroStr As String = ""

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim strAnalisiCatasto As String = ""
            If Not InData.InData.filtroParticelle Is Nothing AndAlso InData.InData.filtroParticelle <> "" Then

                Dim listPart As List(Of PUA_ParticellaVincoloAgronomico) = JsonConvert.DeserializeObject(InData.InData.filtroParticelle, GetType(List(Of PUA_ParticellaVincoloAgronomico)))
                strAnalisiCatasto = PUA_ParticellaVincoloAgronomico.EstrapolaStringaFiltroAnalisiCatasto(InData.InData.piva, listPart)

            End If


            Dim strAnalisi As String = ""
            Dim numAnalisi As Integer = 0
            Dim AnalisiCodPiuRecente As Integer = 0
            strAnalisi = objAnalisi.OttieniFiltroAnalisi(InData.InData.piva, InData.InData.saCod, InData.InData.campoCod,
                                                         InData.InData.appezza, InData.InData.idReg,
                                                         strAnalisiCatasto,
                                                         numAnalisi, AnalisiCodPiuRecente,
                                                         objParametri_Server)


            If strAnalisi <> "" Then
                filtroStr = " Analisi_Testata.Analisi_Testata_Cod IN (" & strAnalisi & ") "
            End If

            Dim ddl_Analisi As New DropDownList
            AgronicaCoreUtility.CaricaListControl.Analisi_Terreno(ddl_Analisi,
                                                                  InData.InData.PrimaRiga_Flag, InData.InData.PrimaRiga_Text, InData.InData.PrimaRiga_Value,
                                                                  InData.InData.piva,
                                                                  filtroStr,
                                                                  "Analisi_Testata_Data_Inizio DESC",
                                                                  objParametri_Server)

            Dim JArrayListaOp As New JArray()

            For Each li As ListItem In ddl_Analisi.Items

                Dim Jobj As New JObject(New JProperty("analisi_testata_cod", li.Value),
                                        New JProperty("nome", li.Text))

                If InData.InData.Recupera_Dettagli = True Then

                    Dim objAnalisiD As New Analisi_Dettagli_R

                    Dim Sabbia As Decimal = 0
                    Dim Argilla As Decimal = 0
                    Dim SostanzaOrganica As Decimal = 0

                    If InData.InData.PrimaRiga_Flag = False OrElse li.Value <> InData.InData.PrimaRiga_Value Then
                        objAnalisiD.ValorizzaDettagliAnalisiTerreno(CInt(li.Value),
                                                                    Sabbia, Argilla, SostanzaOrganica,
                                                                    objParametri_Server)
                    End If

                    Jobj.Add(New JProperty("Sabbia", Sabbia))
                    Jobj.Add(New JProperty("Argilla", Argilla))
                    Jobj.Add(New JProperty("SostanzaOrganica", SostanzaOrganica))

                End If

                JArrayListaOp.Add(Jobj)
            Next

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
    Public Function Leggi_Analisi_Testata_Filtrata(ByVal objP_server As String,
                                                   ByVal piva As String,
                                                   ByVal saCod As Integer,
                                                   ByVal campoCod As Integer,
                                                   ByVal appezza As Integer,
                                                   ByVal idReg As Integer,
                                                   ByVal filtroParticelle As String,
                                                   ByVal PrimaRiga_Flag As Boolean,
                                                   ByVal PrimaRiga_Text As String,
                                                   ByVal PrimaRiga_Value As String,
                                                   ByVal Recupera_Dettagli As Boolean
                                                   ) As RispostaStandard

        Dim r As New RispostaStandard()

        Dim objAnalisi As New AgronicaCoreAnagrafeDAL.Analisi_Testata_R
        Dim filtroStr As String = ""

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim strAnalisiCatasto As String = ""
            If Not filtroParticelle Is Nothing AndAlso filtroParticelle <> "" Then

                Dim listPart As List(Of PUA_ParticellaVincoloAgronomico) = JsonConvert.DeserializeObject(filtroParticelle, GetType(List(Of PUA_ParticellaVincoloAgronomico)))
                strAnalisiCatasto = PUA_ParticellaVincoloAgronomico.EstrapolaStringaFiltroAnalisiCatasto(piva, listPart)

            End If


            Dim strAnalisi As String = ""
            Dim numAnalisi As Integer = 0
            Dim AnalisiCodPiuRecente As Integer = 0
            strAnalisi = objAnalisi.OttieniFiltroAnalisi(piva, saCod, campoCod,
                                                         appezza, idReg,
                                                         strAnalisiCatasto,
                                                         numAnalisi, AnalisiCodPiuRecente,
                                                         objParametri_Server)


            If strAnalisi <> "" Then
                filtroStr = " Analisi_Testata.Analisi_Testata_Cod IN (" & strAnalisi & ") "
            End If

            Dim ddl_Analisi As New DropDownList
            AgronicaCoreUtility.CaricaListControl.Analisi_Terreno(ddl_Analisi,
                                                                  PrimaRiga_Flag, PrimaRiga_Text, PrimaRiga_Value,
                                                                  piva,
                                                                  filtroStr,
                                                                  "Analisi_Testata_Data_Inizio DESC",
                                                                  objParametri_Server)

            Dim JArrayListaOp As New JArray()

            For Each li As ListItem In ddl_Analisi.Items

                Dim Jobj As New JObject(New JProperty("analisi_testata_cod", li.Value),
                                        New JProperty("nome", li.Text))

                If Recupera_Dettagli = True Then

                    Dim objAnalisiD As New Analisi_Dettagli_R

                    Dim Sabbia As Decimal = 0
                    Dim Argilla As Decimal = 0
                    Dim SostanzaOrganica As Decimal = 0

                    If PrimaRiga_Flag = False OrElse li.Value <> PrimaRiga_Value Then
                        objAnalisiD.ValorizzaDettagliAnalisiTerreno(CInt(li.Value),
                                                                    Sabbia, Argilla, SostanzaOrganica,
                                                                    objParametri_Server)
                    End If

                    Jobj.Add(New JProperty("Sabbia", Sabbia))
                    Jobj.Add(New JProperty("Argilla", Argilla))
                    Jobj.Add(New JProperty("SostanzaOrganica", SostanzaOrganica))

                End If

                JArrayListaOp.Add(Jobj)
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(JArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function
End Class