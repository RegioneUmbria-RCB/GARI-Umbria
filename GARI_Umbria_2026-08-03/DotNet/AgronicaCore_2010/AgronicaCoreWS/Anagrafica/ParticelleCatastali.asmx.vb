Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDTOStd.InData.Anagrafica

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class ParticelleCatastali
    Inherits System.Web.Services.WebService

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function GetImpresexParticelle_NG(ByVal InData As CoreWS_Generic(Of GetImpresexParticelle)) As RispostaStandard

        Dim r As New RispostaStandard

        Try
            Dim objParametri_server As AgronicaCoreParametri
            If HttpContext.Current.Session("ASG_objParametri_Server") IsNot Nothing Then
                objParametri_server = HttpContext.Current.Session("ASG_objParametri_Server")
            Else
                objParametri_server = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            End If


            Dim objR As New AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R

            Dim dt As DataTable = objR.Leggi_Superfici(InData.InData.id, InData.InData.piva, InData.InData.sa_cod, 0, InData.InData.prov, InData.InData.com, InData.InData.sezione, InData.InData.foglio, InData.InData.numero, InData.InData.subalterno, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_server)



            Dim lista As New List(Of String)
            Dim supCatastale As Double = 0
            Dim jarr = New JArray()
            For Each dr As DataRow In dt.Rows
                Dim jobj As JObject = New JObject
                jobj.Add(New JProperty("ID", dr.Item("ID")))
                jobj.Add(New JProperty("Piva", dr.Item("Piva")))
                jobj.Add(New JProperty("Sa_cod", dr.Item("Sa_cod")))
                jobj.Add(New JProperty("PROV", dr.Item("PROV")))
                jobj.Add(New JProperty("COM", dr.Item("COM")))
                jobj.Add(New JProperty("SEZIONE", dr.Item("SEZIONE")))
                jobj.Add(New JProperty("FOGLIO", dr.Item("FOGLIO")))
                jobj.Add(New JProperty("NUMERO", dr.Item("NUMERO")))
                jobj.Add(New JProperty("SUBALTERNO", dr.Item("SUBALTERNO")))
                jobj.Add(New JProperty("TitoloPossesso", dr.Item("TitoloPossesso")))
                jobj.Add(New JProperty("Sup_Condotta", dr.Item("Sup_Condotta")))
                jobj.Add(New JProperty("LOCALITA", dr.Item("LOCALITA")))
                jobj.Add(New JProperty("COMUNI_PROV", dr.Item("COMUNI_PROV")))
                jobj.Add(New JProperty("TitoloPossesso_Des", dr.Item("TitoloPossesso_Des")))
                jobj.Add(New JProperty("Ettari", dr.Item("Ettari")))
                jobj.Add(New JProperty("Are", dr.Item("Are")))
                jobj.Add(New JProperty("Centiare", dr.Item("Centiare")))
                supCatastale = AgronicaCoreDataProvider.UtilityProvider.Ettari_from_EttariAreCentiare(dr.Item("Ettari"), dr.Item("Are"), dr.Item("Centiare"))
                jobj.Add(New JProperty("Sup_Catastale", supCatastale))
                'lista.Add("{""id"":""" & jSon.Escape(dr.Item("id")) & """, ""sa_cod"":""" & dr.Item("sa_cod") & """}")
                jarr.Add(jobj)
            Next


            'Dim strRisp As String = "[" & String.Join(",", lista.Distinct.ToList) & "]"
            Dim strRisp As String = jarr.ToString

            r.RispostaOK = True
            r.RispostaStringa = strRisp

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            Dim MessaggioErrore As String = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


            r.Errore = MessaggioErrore


        End Try

        Return r

    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function GetImpresexParticelle(id As Integer,
                                          piva As String,
                                          sa_cod As Integer,
                                          prov As String,
                                          com As String,
                                          sezione As String,
                                          foglio As String,
                                          numero As String,
                                          subalterno As String,
                                          ByVal objP_server As String) As RispostaStandard

        Dim r As New RispostaStandard

        Try
            Dim objParametri_server As AgronicaCoreParametri
            If HttpContext.Current.Session("ASG_objParametri_Server") IsNot Nothing Then
                objParametri_server = HttpContext.Current.Session("ASG_objParametri_Server")
            Else
                objParametri_server = Utility.convertStringtoOBJparametri(objP_server)
            End If


            Dim objR As New AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R

            Dim dt As DataTable = objR.Leggi_Superfici(id, piva, sa_cod, 0, prov, com, sezione, foglio, numero, subalterno, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_server)



            Dim lista As New List(Of String)
            Dim supCatastale As Double = 0
            Dim jarr = New JArray()
            For Each dr As DataRow In dt.Rows
                Dim jobj As JObject = New JObject
                jobj.Add(New JProperty("ID", dr.Item("ID")))
                jobj.Add(New JProperty("Piva", dr.Item("Piva")))
                jobj.Add(New JProperty("Sa_cod", dr.Item("Sa_cod")))
                jobj.Add(New JProperty("PROV", dr.Item("PROV")))
                jobj.Add(New JProperty("COM", dr.Item("COM")))
                jobj.Add(New JProperty("SEZIONE", dr.Item("SEZIONE")))
                jobj.Add(New JProperty("FOGLIO", dr.Item("FOGLIO")))
                jobj.Add(New JProperty("NUMERO", dr.Item("NUMERO")))
                jobj.Add(New JProperty("SUBALTERNO", dr.Item("SUBALTERNO")))
                jobj.Add(New JProperty("TitoloPossesso", dr.Item("TitoloPossesso")))
                jobj.Add(New JProperty("Sup_Condotta", dr.Item("Sup_Condotta")))
                jobj.Add(New JProperty("LOCALITA", dr.Item("LOCALITA")))
                jobj.Add(New JProperty("COMUNI_PROV", dr.Item("COMUNI_PROV")))
                jobj.Add(New JProperty("TitoloPossesso_Des", dr.Item("TitoloPossesso_Des")))
                jobj.Add(New JProperty("Ettari", dr.Item("Ettari")))
                jobj.Add(New JProperty("Are", dr.Item("Are")))
                jobj.Add(New JProperty("Centiare", dr.Item("Centiare")))
                supCatastale = AgronicaCoreDataProvider.UtilityProvider.Ettari_from_EttariAreCentiare(dr.Item("Ettari"), dr.Item("Are"), dr.Item("Centiare"))
                jobj.Add(New JProperty("Sup_Catastale", supCatastale))
                'lista.Add("{""id"":""" & jSon.Escape(dr.Item("id")) & """, ""sa_cod"":""" & dr.Item("sa_cod") & """}")
                jarr.Add(jobj)
            Next


            'Dim strRisp As String = "[" & String.Join(",", lista.Distinct.ToList) & "]"
            Dim strRisp As String = jarr.ToString

            r.RispostaOK = True
            r.RispostaStringa = strRisp

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            Dim MessaggioErrore As String = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


            r.Errore = MessaggioErrore


        End Try

        Return r

    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function GetImpresexParticelle_Movimentate_NG(ByVal InData As CoreWS_Generic(Of GetImpresexParticelle)) As RispostaStandard

        Dim r As New RispostaStandard

        Try
            Dim objParametri_server As AgronicaCoreParametri
            If HttpContext.Current.Session("ASG_objParametri_Server") IsNot Nothing Then
                objParametri_server = HttpContext.Current.Session("ASG_objParametri_Server")
            Else
                objParametri_server = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            End If


            Dim objR As New AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R

            Dim objAppezza As New AgronicaCoreAnagrafeDAL.AppezzaxParticelle_R

            Dim dt As DataTable = objR.Leggi_Superfici(InData.InData.id, InData.InData.piva, InData.InData.sa_cod, 0, InData.InData.prov, InData.InData.com, InData.InData.sezione, InData.InData.foglio, InData.InData.numero, InData.InData.subalterno, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_server)



            Dim lista As New List(Of String)
            Dim supCatastale As Double = 0
            Dim jarr = New JArray()
            For Each dr As DataRow In dt.Rows
                Dim jobj As JObject = New JObject
                jobj.Add(New JProperty("ID", dr.Item("ID")))
                jobj.Add(New JProperty("Piva", dr.Item("Piva")))
                jobj.Add(New JProperty("Sa_cod", dr.Item("Sa_cod")))
                jobj.Add(New JProperty("PROV", dr.Item("PROV")))
                jobj.Add(New JProperty("COM", dr.Item("COM")))
                jobj.Add(New JProperty("SEZIONE", dr.Item("SEZIONE")))
                jobj.Add(New JProperty("FOGLIO", dr.Item("FOGLIO")))
                jobj.Add(New JProperty("NUMERO", dr.Item("NUMERO")))
                jobj.Add(New JProperty("SUBALTERNO", dr.Item("SUBALTERNO")))
                jobj.Add(New JProperty("TitoloPossesso", dr.Item("TitoloPossesso")))
                jobj.Add(New JProperty("Sup_Condotta", dr.Item("Sup_Condotta")))
                jobj.Add(New JProperty("LOCALITA", dr.Item("LOCALITA")))
                jobj.Add(New JProperty("COMUNI_PROV", dr.Item("COMUNI_PROV")))
                jobj.Add(New JProperty("TitoloPossesso_Des", dr.Item("TitoloPossesso_Des")))
                jobj.Add(New JProperty("Ettari", dr.Item("Ettari")))
                jobj.Add(New JProperty("Are", dr.Item("Are")))
                jobj.Add(New JProperty("Centiare", dr.Item("Centiare")))
                supCatastale = AgronicaCoreDataProvider.UtilityProvider.Ettari_from_EttariAreCentiare(dr.Item("Ettari"), dr.Item("Are"), dr.Item("Centiare"))
                jobj.Add(New JProperty("Sup_Catastale", supCatastale))
                jobj.Add(New JProperty("movimentata", ParticellaMovimentata(dr.Item("Piva"),
                                                                            dr.Item("sa_cod"),
                                                                            dr.Item("Prov"),
                                                                            dr.Item("Com"),
                                                                            dr.Item("Sezione"),
                                                                            dr.Item("foglio"),
                                                                            dr.Item("numero"),
                                                                            dr.Item("subalterno"),
                                                                            objParametri_server,
                                                                            objAppezza)))
                'lista.Add("{""id"":""" & jSon.Escape(dr.Item("id")) & """, ""sa_cod"":""" & dr.Item("sa_cod") & """}")
                jarr.Add(jobj)
            Next


            'Dim strRisp As String = "[" & String.Join(",", lista.Distinct.ToList) & "]"
            Dim strRisp As String = jarr.ToString

            r.RispostaOK = True
            r.RispostaStringa = strRisp

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            Dim MessaggioErrore As String = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


            r.Errore = MessaggioErrore


        End Try

        Return r

    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function GetImpresexParticelle_Movimentate(id As Integer,
                                          piva As String,
                                          sa_cod As Integer,
                                          prov As String,
                                          com As String,
                                          sezione As String,
                                          foglio As String,
                                          numero As String,
                                          subalterno As String,
                                          ByVal objP_server As String) As RispostaStandard

        Dim r As New RispostaStandard

        Try
            Dim objParametri_server As AgronicaCoreParametri
            If HttpContext.Current.Session("ASG_objParametri_Server") IsNot Nothing Then
                objParametri_server = HttpContext.Current.Session("ASG_objParametri_Server")
            Else
                objParametri_server = Utility.convertStringtoOBJparametri(objP_server)
            End If


            Dim objR As New AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R

            Dim objAppezza As New AgronicaCoreAnagrafeDAL.AppezzaxParticelle_R

            Dim dt As DataTable = objR.Leggi_Superfici(id, piva, sa_cod, 0, prov, com, sezione, foglio, numero, subalterno, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_server)



            Dim lista As New List(Of String)
            Dim supCatastale As Double = 0
            Dim jarr = New JArray()
            For Each dr As DataRow In dt.Rows
                Dim jobj As JObject = New JObject
                jobj.Add(New JProperty("ID", dr.Item("ID")))
                jobj.Add(New JProperty("Piva", dr.Item("Piva")))
                jobj.Add(New JProperty("Sa_cod", dr.Item("Sa_cod")))
                jobj.Add(New JProperty("PROV", dr.Item("PROV")))
                jobj.Add(New JProperty("COM", dr.Item("COM")))
                jobj.Add(New JProperty("SEZIONE", dr.Item("SEZIONE")))
                jobj.Add(New JProperty("FOGLIO", dr.Item("FOGLIO")))
                jobj.Add(New JProperty("NUMERO", dr.Item("NUMERO")))
                jobj.Add(New JProperty("SUBALTERNO", dr.Item("SUBALTERNO")))
                jobj.Add(New JProperty("TitoloPossesso", dr.Item("TitoloPossesso")))
                jobj.Add(New JProperty("Sup_Condotta", dr.Item("Sup_Condotta")))
                jobj.Add(New JProperty("LOCALITA", dr.Item("LOCALITA")))
                jobj.Add(New JProperty("COMUNI_PROV", dr.Item("COMUNI_PROV")))
                jobj.Add(New JProperty("TitoloPossesso_Des", dr.Item("TitoloPossesso_Des")))
                jobj.Add(New JProperty("Ettari", dr.Item("Ettari")))
                jobj.Add(New JProperty("Are", dr.Item("Are")))
                jobj.Add(New JProperty("Centiare", dr.Item("Centiare")))
                supCatastale = AgronicaCoreDataProvider.UtilityProvider.Ettari_from_EttariAreCentiare(dr.Item("Ettari"), dr.Item("Are"), dr.Item("Centiare"))
                jobj.Add(New JProperty("Sup_Catastale", supCatastale))
                jobj.Add(New JProperty("movimentata", ParticellaMovimentata(dr.Item("Piva"),
                                                                            dr.Item("sa_cod"),
                                                                            dr.Item("Prov"),
                                                                            dr.Item("Com"),
                                                                            dr.Item("Sezione"),
                                                                            dr.Item("foglio"),
                                                                            dr.Item("numero"),
                                                                            dr.Item("subalterno"),
                                                                            objParametri_server,
                                                                            objAppezza)))
                'lista.Add("{""id"":""" & jSon.Escape(dr.Item("id")) & """, ""sa_cod"":""" & dr.Item("sa_cod") & """}")
                jarr.Add(jobj)
            Next


            'Dim strRisp As String = "[" & String.Join(",", lista.Distinct.ToList) & "]"
            Dim strRisp As String = jarr.ToString

            r.RispostaOK = True
            r.RispostaStringa = strRisp

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            Dim MessaggioErrore As String = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


            r.Errore = MessaggioErrore


        End Try

        Return r

    End Function
    Public Function ParticellaMovimentata(Piva As String,
                                          Sa_Cod As Integer,
                                          PROV As String,
                                          COM As String,
                                          Sezione As String,
                                          Foglio As Integer,
                                          Numero As Integer,
                                          Subalterno As String,
                                          objParametri_server As AgronicaCoreParametri,
                                          objAppezza As AgronicaCoreAnagrafeDAL.AppezzaxParticelle_R) As Boolean

        Dim movimentata As Boolean
        Dim dtAppezza = objAppezza.LeggiAppezzamentiImpresa_Da_Particella(Piva, Sa_Cod, PROV, COM, Sezione, Foglio, Numero, Subalterno, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_server)
        If dtAppezza IsNot Nothing AndAlso dtAppezza.Rows.Count > 0 Then
            For Each appezzamento In dtAppezza.Rows
                movimentata = AppezzaMovimentato(Piva, Sa_Cod, appezzamento("appezza"), objParametri_server)
                If movimentata Then
                    Exit For
                End If
            Next
        Else
            movimentata = False
        End If
        Return movimentata
    End Function

    Public Function AppezzaMovimentato(piva As String, sa_cod As Integer, appezza As Integer, objParametri_server As AgronicaCoreParametri) As Boolean
        Dim movimentato As Boolean = False
        Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        Dim dt = objImpianti.Leggi_Operazioni_Impianti(piva, sa_cod, appezza, 0, "", "", objParametri_server)
        If dt IsNot Nothing And dt.Rows.Count > 0 Then
            movimentato = True
        End If
        Return movimentato

    End Function

End Class