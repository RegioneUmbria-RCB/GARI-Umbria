Imports System.ComponentModel
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Imports AgronicaCoreUtility
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreDTOStd.InData.Anagrafica
Imports AgronicaCoreModelsSTD.exceptions

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
Public Class Catasto
    Inherits System.Web.Services.WebService
    'AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG
    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Leggi_Catasto_Anagrafica(InData As Object) As RispostaStandard
        Dim r As New RispostaStandard

        Dim settings As New JsonSerializerSettings()
        settings.DateTimeZoneHandling = DateTimeZoneHandling.Local

        Dim iData As CoreWS_Generic(
            Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG))(JsonConvert.SerializeObject(InData), settings)

        If iData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If iData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objParticelle As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_R
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)

            Dim objParametriAgenda = iData.InData

            Dim FinestraTemporaleInizio = objParametri_Server.FinestraTemporaleInizio
            Dim FinestraTemporaleFine = objParametri_Server.FinestraTemporaleFine

            If objParametriAgenda.Data <> AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO Then
                objParametri_Server.FinestraTemporaleInizio = objParametriAgenda.Data.Date
                objParametri_Server.FinestraTemporaleFine = objParametriAgenda.Data.Date
            End If

            Dim dt As DataTable = objParticelle.Leggi_x_anagrafica_angular(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, "", "", "", 0, 0, "", "", "", objParametri_Server)

            If objParametriAgenda.Data <> AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO Then
                objParametri_Server.FinestraTemporaleInizio = objParametriAgenda.Data.Date
                objParametri_Server.FinestraTemporaleFine = objParametriAgenda.Data.Date
            End If

            'dt.Columns.Add(New DataColumn("Macrousi"))
            'dt.Columns.Add(New DataColumn("Utilizzi"))

            'Dim prov = ""
            'Dim com = ""
            'Dim sezione = ""
            'Dim foglio = 0
            'Dim numero = 0
            'Dim subalterno = ""
            'Dim obj_Macrousi As New AgronicaCoreAnagrafeDAL.ParticelleCatastalixMacrousi_R
            'Dim obj_Utilizzo As New AgronicaCoreAnagrafeDAL.ParticelleCatastalixMacrousixUtilizzo_R
            'For Each row In dt.Rows
            '    prov = row("PROV")
            '    com = row("COM")
            '    sezione = row("Sezione")
            '    foglio = row("Foglio")
            '    numero = row("Numero")
            '    subalterno = row("Subalterno")

            'Next


            'creo la lista delle colonne da visualizzare
            Dim l As New List(Of ColonneNome)

            l.Add(New ColonneNome("Codice", "Codice", "number") With {._hidden = True})
            l.Add(New ColonneNome("chiave", "chiave", "string") With {._hidden = True})
            l.Add(New ColonneNome("part_cod", "part_cod", "number"))
            l.Add(New ColonneNome("cod_particella", "Codice Particella", "string"))
            'If objParametriAgenda.Sa_Cod = 0 Then
            l.Add(New ColonneNome("Sa_Nome", "Centro", "string"))
            l.Add(New ColonneNome("Sa_Cod", "Sa_Cod", "number"))
            'End If
            l.Add(New ColonneNome("Piva", "Piva", "string"))
            l.Add(New ColonneNome("Prov", "Prov Istat", "string"))
            l.Add(New ColonneNome("Com", "Com Istat", "string"))
            l.Add(New ColonneNome("COMUNE", "Com.", "string"))
            l.Add(New ColonneNome("PROVINCIA", "Prov.", "string"))
            l.Add(New ColonneNome("SEZIONE", "Sez.", "string"))
            l.Add(New ColonneNome("FOGLIO", "Fgl.", "number"))
            l.Add(New ColonneNome("NUMERO", "Num.", "number"))
            l.Add(New ColonneNome("SUBALTERNO", "S.", "string"))
            'l.Add(New ColonneNome("Macrousi", "Macrousi", "string"))
            'l.Add(New ColonneNome("Utilizzi", "Utilizzi", "string"))
            l.Add(New ColonneNome("Titolo_Possesso_Cod", "Titolo_Possesso_Cod", "number"))
            l.Add(New ColonneNome("Titolo_possesso", "Possesso", "string"))
            l.Add(New ColonneNome("MetodoProduzione_Cod", "MetodoProduzione_Cod", "number"))
            l.Add(New ColonneNome("MetodoProduzione_Des", "MetodoProduzione_Des", "string"))
            l.Add(New ColonneNome("ZVN", "ZVN", "string"))
            l.Add(New ColonneNome("Sup_Catastale", "Sup. Catastale [ha]", "number"))
            l.Add(New ColonneNome("Sup_Condotta", "Sup. Condotta [ha]", "number"))
            l.Add(New ColonneNome("Validita_Inizio", "Validità Inizio", "date"))
            l.Add(New ColonneNome("Validita_Fine", "Validità Fine", "date"))

            l.Add(New ColonneNome("Attivo", "Attivo", "number"))

            l.Add(New ColonneNome("Data_Creazione", "Data Creazione", "date"))
            l.Add(New ColonneNome("Utente_Creazione", "Utente Creazione", "string"))
            l.Add(New ColonneNome("Data_Modifica", "Data Modifica", "date"))
            l.Add(New ColonneNome("Utente_Modifica", "Utente Modifica", "string"))

            l.Add(New ColonneNome("Proprietario", "Proprietario", "string"))

            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            Dim risp As String = js.JSON_DataTable_Kendo(dt, l, False, False, TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa, "")

            r.RispostaOK = True
            r.RispostaStringa = risp

            'r.RispostaStringa = JsonConvert.SerializeObject(dt)


        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod()>
    Public Function Leggi_Particella_Anagrafica(InData As Object
                                                  ) As rispostaStandard(Of AgronicaCoreModelsSTD.anagrafiche.CatastoCentroAziendale)
        Dim r As New rispostaStandard(Of AgronicaCoreModelsSTD.anagrafiche.CatastoCentroAziendale)

        Dim iData As CoreWS_Generic(Of AgronicaCoreModelsSTD.anagrafiche.CatastoCentroAziendale) = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of AgronicaCoreModelsSTD.anagrafiche.CatastoCentroAziendale))(JsonConvert.SerializeObject(InData))

        If iData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If iData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objParticella As New AgronicaCoreAnagrafeBIZ.Particella_R

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)

            Dim Piva = iData.InData.centro.partitaIva
            Dim Sa_Cod = iData.InData.centro.codice
            Dim Prov = iData.InData.particella.primaryKey.Prov
            Dim Com = iData.InData.particella.primaryKey.Com
            Dim Sezione = iData.InData.particella.primaryKey.Sezione
            Dim Foglio = iData.InData.particella.primaryKey.Foglio
            Dim Numero = iData.InData.particella.primaryKey.Numero
            Dim Subalterno = iData.InData.particella.primaryKey.Subalterno

            Dim Data_Filtro = DateTime.Now

            Dim particella = objParticella.Leggi_Particella_Anagrafica(Piva:=Piva,
                                                                       Sa_Cod:=Sa_Cod,
                                                                       Prov:=Prov,
                                                                       Com:=Com,
                                                                       Sezione:=Sezione,
                                                                       Foglio:=Foglio,
                                                                       Numero:=Numero,
                                                                       Subalterno:=Subalterno,
                                                                       Leggi_Metodi_Produzione:=True,
                                                                       Leggi_Macrousi:=True,
                                                                       Leggi_Zone:=True,
                                                                       Leggi_Classamento:=True,
                                                                       objParametri_Server)

            'If particella Is Nothing Then
            '    Dim keys() As String = {Piva, Sa_Cod, Prov, Com, Sezione, Foglio, Numero, Subalterno}
            '    Throw New Exception("Errore in lettura Appezzamento with key " + String.Join("|", keys))
            'End If

            r.RispostaOK = True
            r.RispostaStringa = particella

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function


    <WebMethod()>
    Public Function Scrivi_Particella_Anagrafica(InData As Object) As rispostaStandard(Of String)

        Dim r As New rispostaStandard(Of String)

        Dim settings As New JsonSerializerSettings()
        settings.DateTimeZoneHandling = DateTimeZoneHandling.Local

        Dim str = JsonConvert.SerializeObject(InData, settings)

        Dim iData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Anagrafica.ScriviCatasto) =
            JsonConvert.DeserializeObject(
            Of CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Anagrafica.ScriviCatasto))(JsonConvert.SerializeObject(InData, settings),
                                                                                      settings)

        If iData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If iData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objParticella As New AgronicaCoreAnagrafeBIZ.Particella_W

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)

            Dim oldValue = iData.InData.oldValue
            Dim newValue = iData.InData.newValue

            If newValue IsNot Nothing Then
                objParticella.ParticellaCatasto_Scrivi(newValue, oldValue, objParametri_Server, objParametri_Utenti, False, NoteLog:=NOTELOG_ANAGRAFE_NG)
            End If

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(iData.InData.newValue, Formatting.None)

        Catch ex As Exception

            r.RispostaOK = False
            r.RispostaStringa = JsonConvert.SerializeObject(iData.InData.newValue, Formatting.None)
            'uso questa funzione per ottenere il Messaggio..:
            r.ErroriGias = New List(Of ErroreGias) From {AgronicaCoreDataProvider.Gestione_Eccezioni_2015.ErrorHandler(ex)}
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod()>
    Public Function Scrivi_ParticelleCatastali_Anagrafica(InData As Object) As RispostaStandard
        Dim r As New RispostaStandard

        Dim settings As New JsonSerializerSettings()
        settings.DateTimeZoneHandling = DateTimeZoneHandling.Local

        Dim str = JsonConvert.SerializeObject(InData, settings)

        Dim iData As CoreWS_Generic(Of List(Of AgronicaCoreDTOStd.InData.Anagrafica.ScriviCatasto)) =
            JsonConvert.DeserializeObject(
            Of CoreWS_Generic(Of List(Of AgronicaCoreDTOStd.InData.Anagrafica.ScriviCatasto)))(JsonConvert.SerializeObject(InData, settings),
                                                                                      settings)

        If iData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If iData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objParticella As New AgronicaCoreAnagrafeBIZ.Particella_W

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)

            r.RispostaOK = objParticella.ParticelleCatastali_Scrivi(iData.InData, objParametri_Server, objParametri_Utenti, False, NoteLog:=NOTELOG_ANAGRAFE_NG)
            If r.RispostaOK Then
                r.RispostaStringa = "Scrittura catasto eseguita correttamente"
            End If

        Catch ex As Exception

            r.RispostaOK = False
            'uso questa funzione per ottenere il Messaggio..:
            r.ErroriGias = New List(Of ErroreGias) From {AgronicaCoreDataProvider.Gestione_Eccezioni_2015.ErrorHandler(ex)}
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r

    End Function


    <WebMethod()>
    Public Function Leggi_Investimento_Catastale(InData As Object
                                                  ) As rispostaStandard(Of String)
        Dim r As New rispostaStandard(Of String)

        Dim settings As New JsonSerializerSettings()
        settings.DateTimeZoneHandling = DateTimeZoneHandling.Local

        Dim str = JsonConvert.SerializeObject(InData, settings)

        Dim iData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Metaschema.LeggiInvestimentoCatastale) =
            JsonConvert.DeserializeObject(
            Of CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Metaschema.LeggiInvestimentoCatastale))(JsonConvert.SerializeObject(InData, settings),
                                                                                      settings)

        If iData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If iData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objParticella As New AgronicaCoreAnagrafeDAL.AppezzaxParticelle_R

            Dim dt As DataTable
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)

            Dim Piva As String = ""
            Dim sa_Cod As Long = 0
            Dim Appezza As Long = 0
            Dim Id_Reg As Long = 0
            Dim Prov As String = ""
            Dim Com As String = ""
            Dim sezione As String = ""
            Dim foglio As Integer = 0
            Dim numero As Integer = 0
            Dim subalterno As String = ""

            If iData.InData.impresa IsNot Nothing AndAlso iData.InData.impresa.partitaIva <> "" Then
                Piva = iData.InData.impresa.partitaIva
            End If

            If iData.InData.centro IsNot Nothing AndAlso
                iData.InData.centro.primaryKey IsNot Nothing AndAlso
                iData.InData.centro.primaryKey.codice > 0 Then

                Piva = iData.InData.centro.primaryKey.partitaIva
                sa_Cod = iData.InData.centro.primaryKey.codice

            End If

            If iData.InData.particella IsNot Nothing AndAlso
                iData.InData.particella.primaryKey IsNot Nothing Then

                Prov = iData.InData.particella.primaryKey.Prov
                Com = iData.InData.particella.primaryKey.Com
                sezione = iData.InData.particella.primaryKey.Sezione
                foglio = iData.InData.particella.primaryKey.Foglio
                numero = iData.InData.particella.primaryKey.Numero
                subalterno = iData.InData.particella.primaryKey.Subalterno

            End If

            If iData.InData.impianto IsNot Nothing AndAlso
                iData.InData.impianto.primaryKey IsNot Nothing Then
                Piva = iData.InData.impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva
                sa_Cod = iData.InData.impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice
                Appezza = iData.InData.impianto.primaryKey.appezzamentoPK.codice
                Id_Reg = iData.InData.impianto.primaryKey.codice
            End If

            dt = objParticella.LeggiAppezzamenti_Da_Particella(Piva, sa_Cod, Appezza, Id_Reg, Prov, Com, sezione, foglio, numero, subalterno, "", "", objParametri_Server)

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None)

        Catch ex As Exception

            r.RispostaOK = False
            r.RispostaStringa = ex.Message
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod()>
    Public Function Leggi_Investimento_Catastale_Campo(InData As Object
                                                  ) As rispostaStandard(Of String)
        Dim r As New rispostaStandard(Of String)

        Dim settings As New JsonSerializerSettings()
        settings.DateTimeZoneHandling = DateTimeZoneHandling.Local

        Dim str = JsonConvert.SerializeObject(InData, settings)

        Dim iData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Metaschema.LeggiInvestimentoCatastaleCampo) =
            JsonConvert.DeserializeObject(
            Of CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Metaschema.LeggiInvestimentoCatastaleCampo))(JsonConvert.SerializeObject(InData, settings),
                                                                                      settings)

        If iData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If iData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objCampo As New AgronicaCoreAnagrafeDAL.Campi_R

            Dim dt As DataTable
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)

            Dim Piva As String = ""
            Dim sa_Cod As Long = 0
            Dim Campo_Cod As Long = 0
            Dim Prov As String = ""
            Dim Com As String = ""
            Dim sezione As String = ""
            Dim foglio As Integer = 0
            Dim numero As Integer = 0
            Dim subalterno As String = ""

            If iData.InData.impresa IsNot Nothing AndAlso iData.InData.impresa.partitaIva <> "" Then
                Piva = iData.InData.impresa.partitaIva
            End If

            If iData.InData.centro IsNot Nothing AndAlso
                iData.InData.centro.primaryKey IsNot Nothing AndAlso
                iData.InData.centro.primaryKey.codice > 0 Then

                Piva = iData.InData.centro.primaryKey.partitaIva
                sa_Cod = iData.InData.centro.primaryKey.codice

            End If

            If iData.InData.particella IsNot Nothing AndAlso
                iData.InData.particella.primaryKey IsNot Nothing Then

                Prov = iData.InData.particella.primaryKey.Prov
                Com = iData.InData.particella.primaryKey.Com
                sezione = iData.InData.particella.primaryKey.Sezione
                foglio = iData.InData.particella.primaryKey.Foglio
                numero = iData.InData.particella.primaryKey.Numero
                subalterno = iData.InData.particella.primaryKey.Subalterno

            End If

            If iData.InData.campo IsNot Nothing AndAlso
                iData.InData.campo.primaryKey IsNot Nothing Then
                Piva = iData.InData.campo.primaryKey.centroAziendalePK.partitaIva
                sa_Cod = iData.InData.campo.primaryKey.centroAziendalePK.codice
                Campo_Cod = iData.InData.campo.primaryKey.codice
            End If

            dt = objCampo.LeggiCampixParticella(Piva, sa_Cod, Campo_Cod, Prov, Com, sezione, foglio, numero, subalterno, "", "", objParametri_Server)

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None)

        Catch ex As Exception

            r.RispostaOK = False
            r.RispostaStringa = ex.Message
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function



    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Test_Catasto_Archivio_Lettura(ByVal InData As CoreWS_Generic(Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG)) As rispostaStandard(Of Boolean)

        Dim r As New rispostaStandard(Of Boolean)

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim objCatastoDAL_R As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_R
            Dim objCatastoBIZ_R As New AgronicaCoreAnagrafeBIZ.Particella_R

            Dim dt = objCatastoDAL_R.Leggi_x_anagrafica_angular(InData.InData.Piva, 0, "", "", "", 0, 0, "", "", "", objParametri_Server)

            Dim listCatasto As New List(Of String)

            r.ErroriGias = New List(Of ErroreGias)
            Dim i = 0
            For Each row In dt.Rows
                i += 1
                Try
                    Dim catastoVal = row("Prov") & "_" & row("Com") & "_" & row("Sezione") & "_" & row("Foglio") & "_" & row("Numero") & "_" & row("Subalterno")

                    If Not listCatasto.Contains(catastoVal) Then
                        Dim m = objCatastoBIZ_R.Leggi_Particella_Anagrafica(row("Piva"),
                                                                row("Sa_Cod"),
                                                                row("Prov"),
                                                                row("Com"),
                                                                row("Sezione"),
                                                                row("Foglio"),
                                                                row("Numero"),
                                                                row("Subalterno"),
                                                                True,
                                                                True,
                                                                True,
                                                                True,
                                                                objParametri_Server)
                        listCatasto.Add(catastoVal)
                    End If

                Catch ex As Exception

                    r.ErroriGias.Add(New ErroreGias() With {
                                     .ex = ex.Message,
                                     .messaggio = "Errore Catasto " & row("Prov") & "_" & row("Com") & "_" & row("Sezione") & "_" & row("Foglio") & "_" & row("Numero") & "_" & row("Subalterno")
                                     })

                End Try
            Next

            r.RispostaOK = True
            r.RispostaStringa = True

            If r.ErroriGias.Count > 0 Then
                r.RispostaOK = False
                r.RispostaStringa = False
            End If

        Catch ex As Exception

            r.RispostaOK = False
            r.RispostaStringa = False
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Test_Catasto_Archivio_Scrittura(ByVal InData As CoreWS_Generic(Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG)) As rispostaStandard(Of Boolean)

        Dim r As New rispostaStandard(Of Boolean)

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim objCatastoDAL_R As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_R
            Dim objCatastoBIZ_R As New AgronicaCoreAnagrafeBIZ.Particella_R
            Dim objCatastoBIZ_W As New AgronicaCoreAnagrafeBIZ.Particella_W

            Dim dt = objCatastoDAL_R.Leggi_x_anagrafica_angular(InData.InData.Piva, 0, "", "", "", 0, 0, "", "", "", objParametri_Server)

            Dim listCatasto As New List(Of String)

            r.ErroriGias = New List(Of ErroreGias)
            Dim i = 0
            For Each row In dt.Rows
                i += 1
                Dim catastoVal = row("Prov") & "_" & row("Com") & "_" & row("Sezione") & "_" & row("Foglio") & "_" & row("Numero") & "_" & row("Subalterno")

                If Not listCatasto.Contains(catastoVal) Then

                    Dim m As AgronicaCoreModelsSTD.anagrafiche.CatastoCentroAziendale

                    Try
                        m = objCatastoBIZ_R.Leggi_Particella_Anagrafica(row("Piva"),
                                                                row("Sa_Cod"),
                                                                row("Prov"),
                                                                row("Com"),
                                                                row("Sezione"),
                                                                row("Foglio"),
                                                                row("Numero"),
                                                                row("Subalterno"),
                                                                True,
                                                                True,
                                                                True,
                                                                True,
                                                                objParametri_Server)
                        listCatasto.Add(catastoVal)
                    Catch ex As Exception

                        r.ErroriGias.Add(New ErroreGias() With {
                                         .ex = ex.Message,
                                         .messaggio = "Errore Lettura Campo " & row("Piva") & "_" & row("Sa_Cod") & "_" & row("Campo_Cod")}
                                         )

                    End Try

                    Try
                        If m IsNot Nothing Then
                            objCatastoBIZ_W.ParticellaCatasto_Scrivi(m, m, objParametri_Server, objParametri_Utenti, False)
                        End If

                    Catch ex As Exception

                        r.ErroriGias.Add(New ErroreGias() With {
                                         .ex = ex.Message,
                                         .messaggio = "Errore Scrittura Campo " & row("Piva") & "_" & row("Sa_Cod") & "_" & row("Campo_Cod")}
                                         )

                    End Try


                End If

            Next

            r.RispostaOK = True
            r.RispostaStringa = True

            If r.ErroriGias.Count > 0 Then
                r.RispostaOK = False
                r.RispostaStringa = False
            End If



        Catch ex As Exception

            r.RispostaOK = False
            r.RispostaStringa = False
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod()>
    Public Function CheckPossessi(InData As Object) As rispostaStandard(Of Boolean)

        Dim r As New rispostaStandard(Of Boolean)

        Dim settings As New JsonSerializerSettings()
        settings.DateTimeZoneHandling = DateTimeZoneHandling.Local

        Dim str = JsonConvert.SerializeObject(InData, settings)

        Dim iData As CoreWS_Generic(Of AgronicaCoreModelsSTD.anagrafiche.CatastoCentroAziendale) =
            JsonConvert.DeserializeObject(
            Of CoreWS_Generic(Of AgronicaCoreModelsSTD.anagrafiche.CatastoCentroAziendale))(JsonConvert.SerializeObject(InData, settings),
                                                                                      settings)
        If iData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If iData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)

        Try
            Dim obj_Particelle As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_R

            r.RispostaOK = True

            r.RispostaStringa = obj_Particelle.CheckEsistenzaPossessi(iData.InData.centro.partitaIva,
                                                     iData.InData.centro.codice,
                                                     iData.InData.particella.primaryKey.Prov,
                                                     iData.InData.particella.primaryKey.Com,
                                                     IIf(iData.InData.particella.primaryKey.Sezione = "", "0", iData.InData.particella.primaryKey.Sezione),
                                                     iData.InData.particella.primaryKey.Foglio,
                                                     iData.InData.particella.primaryKey.Numero,
                                                     IIf(iData.InData.particella.primaryKey.Subalterno = "", "0", iData.InData.particella.primaryKey.Subalterno),
                                                     iData.InData.flag_cancellazione,
                                                     objParametri_Server)

        Catch gex As GiasException
            r.RispostaOK = False
            r.RispostaStringa = True
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(New Exception(gex.Message), False, source:=True)
        Catch ex As Exception

            r.RispostaOK = False
            r.RispostaStringa = True
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r

    End Function

#Region "lettura Particelle Catastali per Documentale"
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiParticelleCatastali_toKendoGrid(ByVal Piva As String,
                                                         ByVal Id_ImpresexParticelle As Integer,
                                                         ByVal objP_server As String
                                                         ) As rispostaStandard(Of Object)

        Dim NomeRoutine As String = "Catasto.asmx/LeggiParticelleCatastali_toKendoGrid()"

        Dim r As New rispostaStandard(Of Object)
        Dim xFiltroAggiuntivo As String = ""

        xFiltroAggiuntivo += " i.Validita_inizio <= " & Agro_SQL_SaveDate(Date.Now) + vbCrLf

        xFiltroAggiuntivo += " AND i.Validita_fine  >= " & Agro_SQL_SaveDate(Date.Now)

        If Id_ImpresexParticelle <> 0 Then
            xFiltroAggiuntivo += "AND i.ID  = '" & Id_ImpresexParticelle & "'"
        End If

        Try
            Dim objParametri_Server = Utility.convertStringtoOBJparametri(objP_server)

            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod,
                                                                  AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                  "", "", objParametri_Server)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)


            Dim objParticelle As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_R
            Dim dt As DataTable = objParticelle.Leggi_x_anagrafica_desc(Piva, 0, "", "", "", 0, 0, "", xFiltroAggiuntivo, "", objParametri_Server)


            Dim l As New List(Of ColonneNome)

            l.Add(New ColonneNome("chiave", "chiave", "string") With {._hidden = True})

            l.Add(New ColonneNome("Id_ImpresexParticelle", "Id_ImpresexParticelle", "number") With {._hidden = True})
            l.Add(New ColonneNome("sa_Cod", "sa_cod", "number") With {._hidden = True})
            l.Add(New ColonneNome("part_cod", "part_cod", "number") With {._hidden = True})

            l.Add(New ColonneNome("Sa_Nome", Gias.Centro, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
            l.Add(New ColonneNome("cod_particella", Gias.CodiceParticella, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})

            l.Add(New ColonneNome("Prov", "Prov Istat", "string") With {._Filtrabile = True, ._FiltrabileConCheck = True, ._Display = False})
            l.Add(New ColonneNome("COMUNE", Gias.Comune, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})

            l.Add(New ColonneNome("Com", "Com Istat", "string") With {._Filtrabile = True, ._FiltrabileConCheck = True, ._Display = False})
            l.Add(New ColonneNome("PROVINCIA", Gias.Provincia, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})

            l.Add(New ColonneNome("SEZIONE", Gias.Sezione, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
            l.Add(New ColonneNome("FOGLIO", Gias.Foglio, "number") With {._Filtrabile = True, ._FiltrabileConCheck = True})
            l.Add(New ColonneNome("NUMERO", Gias.Numero, "number") With {._Filtrabile = True, ._FiltrabileConCheck = True})
            l.Add(New ColonneNome("SUBALTERNO", Gias.Subalterno, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})

            l.Add(New ColonneNome("Titolo_possesso", Gias.Possesso, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
            l.Add(New ColonneNome("Sup_Catastale", Gias.SuperficieCatastaleAbbr + " [ha]", "number") With {._Filtrabile = True, ._FiltrabileConCheck = True})
            l.Add(New ColonneNome("sup_Condotta", Gias.SuperficieCondottaAbbr + " [ha]", "number") With {._Filtrabile = True, ._FiltrabileConCheck = True})
            l.Add(New ColonneNome("Validita_Inizio", Gias.ValiditaInizio, "date") With {._Filtrabile = True, ._FiltrabileConCheck = True})
            l.Add(New ColonneNome("Validita_Fine", Gias.ValiditaFine, "date") With {._Filtrabile = True, ._FiltrabileConCheck = True})

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            r.RispostaStringa = js.JSON_DataTable_Kendo(dt, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True)
            r.RispostaOK = True

        Catch ex As Exception
            r.ErroriGias = New List(Of ErroreGias) From {Gestione_Eccezioni_2015.ErrorHandler(ex)}
            r.RispostaOK = False
            r.Errore = NomeRoutine & ": " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiParticelleCatastali_toKendoGrid_NG(InData As CoreWS_Generic(Of LeggiParticelleCatastali_toKendoGrid)) As rispostaStandard(Of Object)

        Dim NomeRoutine As String = "Catasto.asmx/LeggiParticelleCatastali_toKendoGrid_NG()"

        Dim r As New rispostaStandard(Of Object)
        Dim xFiltroAggiuntivo As String = ""

        xFiltroAggiuntivo += " i.Validita_inizio <= " & Agro_SQL_SaveDate(Date.Now) + vbCrLf

        xFiltroAggiuntivo += " AND i.Validita_fine  >= " & Agro_SQL_SaveDate(Date.Now)

        If InData.InData.Id_ImpresexParticelle <> 0 Then
            xFiltroAggiuntivo += "AND i.ID  = '" & InData.InData.Id_ImpresexParticelle & "'"
        End If

        Try
            Dim objParametri_Server = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod,
                                                                  AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                  "", "", objParametri_Server)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)


            Dim objParticelle As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_R
            Dim dt As DataTable = objParticelle.Leggi_x_anagrafica_desc(InData.InData.Piva, 0, "", "", "", 0, 0, "", xFiltroAggiuntivo, "", objParametri_Server)


            Dim l As New List(Of ColonneNome)

            l.Add(New ColonneNome("chiave", "chiave", "string") With {._hidden = True})

            l.Add(New ColonneNome("Id_ImpresexParticelle", "Id_ImpresexParticelle", "number") With {._hidden = True})
            l.Add(New ColonneNome("sa_Cod", "sa_cod", "number") With {._hidden = True})
            l.Add(New ColonneNome("part_cod", "part_cod", "number") With {._hidden = True})

            l.Add(New ColonneNome("Sa_Nome", Gias.Centro, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
            l.Add(New ColonneNome("cod_particella", Gias.CodiceParticella, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})

            l.Add(New ColonneNome("Prov", "Prov Istat", "string") With {._Filtrabile = True, ._FiltrabileConCheck = True, ._Display = False})
            l.Add(New ColonneNome("COMUNE", Gias.Comune, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})

            l.Add(New ColonneNome("Com", "Com Istat", "string") With {._Filtrabile = True, ._FiltrabileConCheck = True, ._Display = False})
            l.Add(New ColonneNome("PROVINCIA", Gias.Provincia, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})

            l.Add(New ColonneNome("SEZIONE", Gias.Sezione, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
            l.Add(New ColonneNome("FOGLIO", Gias.Foglio, "number") With {._Filtrabile = True, ._FiltrabileConCheck = True})
            l.Add(New ColonneNome("NUMERO", Gias.Numero, "number") With {._Filtrabile = True, ._FiltrabileConCheck = True})
            l.Add(New ColonneNome("SUBALTERNO", Gias.Subalterno, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})

            l.Add(New ColonneNome("Titolo_possesso", Gias.Possesso, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
            l.Add(New ColonneNome("Sup_Catastale", Gias.SuperficieCatastaleAbbr + " [ha]", "number") With {._Filtrabile = True, ._FiltrabileConCheck = True})
            l.Add(New ColonneNome("sup_Condotta", Gias.SuperficieCondottaAbbr + " [ha]", "number") With {._Filtrabile = True, ._FiltrabileConCheck = True})
            l.Add(New ColonneNome("Validita_Inizio", Gias.ValiditaInizio, "date") With {._Filtrabile = True, ._FiltrabileConCheck = True})
            l.Add(New ColonneNome("Validita_Fine", Gias.ValiditaFine, "date") With {._Filtrabile = True, ._FiltrabileConCheck = True})

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            r.RispostaStringa = js.JSON_DataTable_Kendo(dt, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True)
            r.RispostaOK = True

        Catch ex As Exception
            r.ErroriGias = New List(Of ErroreGias) From {Gestione_Eccezioni_2015.ErrorHandler(ex)}
            r.RispostaOK = False
            r.Errore = NomeRoutine & ": " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r
    End Function
#End Region


End Class