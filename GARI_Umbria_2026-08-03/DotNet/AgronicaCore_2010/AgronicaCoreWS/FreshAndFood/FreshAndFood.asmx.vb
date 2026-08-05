Imports System.Web.Services
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreWinsortDAL
Imports AgronicaCoreWinsortBIZ
Imports AgronicaCoreContabDAL
Imports AgronicaCoreContabBIZ
Imports AgronicaCoreAnagrafeDAL
Imports Newtonsoft.Json
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreModelsSTD.baseClass
Imports Newtonsoft.Json.Linq


' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
Public Class FreshAndFood
    Inherits System.Web.Services.WebService

    <WebMethod()>
    Public Function LeggiCalibrature(ByVal objP_server As String, ByVal idCalibro As Integer) As String

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)

        Try
            Dim strRisposta = ""

            Dim DbAccess As New DBCalibrature_R(objParametri_Server)
            If idCalibro = 0 Then
                strRisposta = DammiWaTableCalibrature(DbAccess.leggiCalibrature())
            Else
                strRisposta = DammiWaTableCalibrature(DbAccess.leggiCalibratura(idCalibro))
            End If


            r.RispostaOK = True
            r.RispostaStringa = strRisposta

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r.RispostaStringa
    End Function

    <WebMethod()>
    Public Function LeggiSpecie(ByVal objP_server As String, ByVal objP_utenti As String) As String

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_utenti)

        Try
            Dim strRisposta = ""

            Dim DbAccess As New DBCalibrature_R(objParametri_Server)


            Dim ddlApp As New DropDownList
            AgronicaCoreUtility.CaricaListControl.SpecieVegetale_Optimize(
                ddlApp,
                True,
                 "",
                 "",
                 0,
                 "",
                 False,
                 0,
                 0,
                 0,
                 "",
                 "",
                 objParametri_Server,
                  objParametri_Utenti)

            Dim rval1 As New List(Of String)
            Dim rval As String = "{""op"":["
            For Each elem As ListItem In ddlApp.Items

                rval1.Add("{""value"": """ & elem.Value & """, ""text"":""" & elem.Text & """} ")

            Next

            rval &= String.Join("," & vbCrLf, rval1.ToArray)


            rval &= "]}"

            r.RispostaOK = True
            r.RispostaStringa = rval

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r.RispostaStringa
    End Function

    <WebMethod()>
    Public Function LeggiVarieta(ByVal objP_server As String, ByVal objP_utenti As String, ByVal Veg_Cod As Integer) As String

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_utenti)

        Try
            Dim strRisposta = ""

            Dim DbAccess As New DBCalibrature_R(objParametri_Server)


            Dim ddlApp As New DropDownList
            'AgronicaCoreUtility.CaricaListControl.SpecieVegetale_Optimize(
            '    ddlApp,
            '    True,
            '     "Seleziona ... ",
            '     "0",
            '     0,
            '     "",
            '     False,
            '     0,
            '     0,
            '     0,
            '     "",
            '     "",
            '     objParametri_Server, _
            '      objParametri_Utenti)

            AgronicaCoreUtility.CaricaListControl.Cultivar(
                ddlApp,
                True,
                "",
                "",
                Veg_Cod,
                0,
                "",
                False, 0, 0, "", "", objParametri_Server, objParametri_Utenti)



            Dim rval1 As New List(Of String)
            Dim rval As String = "{""op"":["
            For Each elem As ListItem In ddlApp.Items

                rval1.Add("{""value"": """ & elem.Value & """, ""text"":""" & elem.Text & """} ")

            Next

            rval &= String.Join("," & vbCrLf, rval1.ToArray)


            rval &= "]}"

            r.RispostaOK = True
            r.RispostaStringa = rval

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r.RispostaStringa
    End Function

    <WebMethod()>
    Public Function LeggiCalibri(ByVal objP_server As String, ByVal objP_utenti As String, ByVal Veg_Cod As Integer) As String
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_utenti)

        Try
            Dim strRisposta = ""

            Dim DbAccess As New DBCalibrature_R(objParametri_Server)


            Dim ddlApp As New DropDownList

            AgronicaCoreUtility.CaricaListControl.Calibri(
                ddlApp,
                True,
                "",
                "",
                "",
                "",
                objParametri_Server)


            Dim rval1 As New List(Of String)
            Dim rval As String = "{""op"":["
            For Each elem As ListItem In ddlApp.Items

                rval1.Add("{""value"": """ & elem.Value & """, ""text"":""" & elem.Text & """} ")

            Next

            rval &= String.Join("," & vbCrLf, rval1.ToArray)


            rval &= "]}"

            r.RispostaOK = True
            r.RispostaStringa = rval

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r.RispostaStringa
    End Function

    <WebMethod()>
    Public Function LeggiCalibriXCalibrature(ByVal objP_server As String,
                                             ByVal idCalibro As Integer
                                             ) As String

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)

        Try
            Dim strRisposta = ""

            Dim DbAccess As New DBCalibrature_R(objParametri_Server)

            strRisposta = DammiWaTableCalibratureXCalibri(DbAccess.leggiCalibriXCalibratura(idCalibro))

            r.RispostaOK = True
            r.RispostaStringa = strRisposta

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r.RispostaStringa
    End Function

    <WebMethod()>
    Public Function LeggiParametriQualitativi(ByVal objP_server As String, ByVal objP_utenti As String, ByVal piva As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_utenti)

        Try
            Dim leggi_R As New OModuli_Referenze_Config_Dettagli_R
            r.RispostaStringa = leggi_R.LeggiParametriQualitativi(piva, objParametri_Server)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod()>
    Public Function LeggiValoriParametriQualitativi(ByVal objP_server As String, ByVal objP_utenti As String, ByVal piva As String, Tabella_ID As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_utenti)

        Try
            Dim leggi_R As New OTabelle_Parametri_R
            r.RispostaStringa = leggi_R.LeggiValoriParametriQualitativi(piva, Tabella_ID, objParametri_Server)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod()>
    Public Function LeggiValoriParametriQualitativi_Modello(InData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Anagrafica.FiltroValoriParametriQualitativi)) As rispostaStandard(Of List(Of BaseCodeDescr))

        Dim r As New rispostaStandard(Of List(Of BaseCodeDescr))

        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(InData.objP.objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

        Try
            Dim leggi_R As New OTabelle_Parametri_R
            Dim strVal = leggi_R.LeggiValoriParametriQualitativi(InData.InData.impresa.partitaIva, InData.InData.tabella, objParametri_Server)

            Dim jArr = JArray.Parse(strVal)
            Dim resArray = New List(Of BaseCodeDescr)
            For index As Integer = 0 To jArr.Count - 1
                resArray.Add(New BaseCodeDescr(CInt(jArr(index)("val_cod").ToString), jArr(index)("val_des").ToString))
            Next

            r.RispostaStringa = resArray
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function


    <WebMethod()>
    Public Function LeggiParametriQualitativiFiltroSpecieVarieta(ByVal objP_server As String, ByVal objP_utenti As String, ByVal piva As String, ByVal modulo_generazione As Integer, ByVal veg_cod As Integer, ByVal cul_cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_utenti)

        Try
            Dim leggi_R As New OModuli_Referenze_Config_Dettagli_R
            r.RispostaStringa = leggi_R.LeggiParametriQualitativiFiltroSpecieVarieta(piva, modulo_generazione, veg_cod, cul_cod, objParametri_Server)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function


    <WebMethod()>
    Public Function LeggiParametriIndici(ByVal objP_server As String, ByVal objP_utenti As String, ByVal piva As String, id_indice As Integer, tipoindice As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_utenti)

        Try
            Dim leggi_R As New Parametri_Indice_R


            Dim objLetturaImpreseImpostazioni = New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_R

            Dim listaCentriAziendali As New List(Of Integer)

            listaCentriAziendali.Add(0)

            'Controllo Abilitazione GHG
            Dim impostazioneGestione_GHG =
            objLetturaImpreseImpostazioni.LeggiScalareMulticentroAziendaSuperUser(
            piva,
            listaCentriAziendali,
            enum_Impostazioni_Utenti.SUPERUSER_Gestione_GHG,
            0,
            objParametri_Utenti,
            objParametri_Server)

            If CBool(impostazioneGestione_GHG) Then

                Dim dtParametri = leggi_R.Leggi(piva, id_indice, tipoindice, objParametri_Server, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta)

                Dim serializerSettings As New JsonSerializerSettings()
                serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                r.RispostaStringa = JsonConvert.SerializeObject(dtParametri, Formatting.None, serializerSettings)

            Else

                r.RispostaStringa = ""

            End If


            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function



    <WebMethod()>
    Public Function LeggiParametriIndiciDettagli(ByVal objP_server As String, ByVal objP_utenti As String, ByVal piva As String, id_indice As Integer, tipocampo As String, elenco_tipo As Integer, elenco_cod As Integer, data As String, filtroaggiuntivo As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_utenti)

        Try
            Dim leggi_R As New Parametri_Indice_R

            Dim dtParametri = leggi_R.LeggiDettagli(piva, id_indice, tipocampo, elenco_tipo, elenco_cod, data, filtroaggiuntivo, objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(dtParametri, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function



    <WebMethod()>
    Public Function LeggiMateriePrimeCampionature(ByVal objP_server As String, ByVal objP_utenti As String, ByVal piva As String, progressivo As Integer, tipo As String, tipo_cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_utenti)

        Try
            Dim leggi_R As New Materie_Prime_Campionature_R

            Dim dtMPC = leggi_R.Leggi(progressivo, tipo, tipo_cod, 0, 0, "", True, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(dtMPC, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod()>
    Public Function LeggiConfigImballiProdotto(ByVal objP_server As String, ByVal objP_utenti As String, ByVal piva As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_utenti)

        Try
            Dim leggi_R As New Configurazione_Imballaggi_R
            r.RispostaStringa = leggi_R.Leggi_Configurazione_Imballaggi(piva, objParametri_Server)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod()>
    Public Function LeggiImballaggi(ByVal objP_server As String, ByVal objP_utenti As String, ByVal piva As String, Tabella_Cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_utenti)

        Try
            Dim leggi_R As New OTabelle_Parametri_R
            r.RispostaStringa = leggi_R.LeggiValoriParametriQualitativi(piva, Tabella_Cod, objParametri_Server)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod()>
    Public Function proponiSpec(ByVal objP_server As String, ByVal varieta As String) As String

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)

        Try
            Dim strRisposta = ""

            Dim dbAccess As New DBCalibrature_R(objParametri_Server)
            Dim res = dbAccess.leggiSpecieEVarietaDataStringa(varieta)
            If res IsNot Nothing Then
                strRisposta = res(0)
            End If


            r.RispostaOK = True
            r.RispostaStringa = strRisposta

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r.RispostaStringa
    End Function




#Region "script services Specie Varieta"

    <WebMethod()>
    Public Function LeggiSpecieFF(ByVal objP_server As String, ByVal objP_utenti As String, ByVal piva As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_utenti)

        Try

            r.RispostaOK = True

            Dim d1 As New AgronicaControlli_2010.ComboSpecie
            d1.ddl_Specie = New DropDownList
            d1.Piva = piva
            'd1.Sa_Cod = objParametriAgenda.Sa_Cod
            'd1.Data = objParametriAgenda.Data
            d1.ConsideraTerrenoNudo = False
            d1.DettagliTerrenoNudo = False
            d1.leggiAncheImpiantiBloccati = False
            d1.PrimaRiga_Flag = True
            d1.PrimaRiga_Value = "-1"
            d1.PrimaRiga_Text = ""
            d1.Carica_Tutte_Specie_Esistenti = True
            d1.Veg_Cod_ModificaLettura = 0
            d1.Usa_Filtro_Utente = True
            d1.CaricaxPDC = False
            'Select Case objParametriAgenda.Tipo_Operazione
            '    Case TipiEnumerativi.enum_TipoOperazioneDB.Lettura, TipiEnumerativi.enum_TipoOperazioneDB.Modifica
            ''se sono in modifica o lettura imposto la Veg_Cod_ModificaLettura in modo che la specie sia comunque inserita nella combo anche se filtrata
            'ComboSpecie.Veg_Cod_ModificaLettura = objParametriAgenda.Veg_Cod.Split("/")(0)
            '    Case TipiEnumerativi.enum_TipoOperazioneDB.Scrittura
            'ComboSpecie.Veg_Cod_ModificaLettura = 0
            'End Select
            d1.FiltroAggiuntivo = ""

            d1.CaricaComboSpecie(objParametri_Server, objParametri_Utenti)

            r.RispostaStringa = AgronicaCoreUtility.CaricaListControl.itemsCollection_AsJsonArray(d1.ddl_Specie.Items, "Veg_Cod", "Veg_Des")

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                        AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod()>
    Public Function LeggiVarietaFF(ByVal objP_server As String, ByVal objP_utenti As String, ByVal piva As String, ByVal Veg_Cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_utenti)

        Try

            r.RispostaOK = True

            Dim d1 As New AgronicaControlli_2010.ComboVarieta
            d1.ddl_Varieta = New DropDownList
            d1.Piva = piva
            'd1.Campi = Valori_Combo
            'd1.Sa_Cod = objParametriAgenda.Sa_Cod
            ''d1.Campo_Cod = CampoCod
            d1.Veg_Cod = Veg_Cod
            d1.SoloAziendali = False
            d1.PrimaRiga_Flag = True
            d1.PrimaRiga_Text = ""
            d1.PrimaRiga_Value = "0"

            d1.CaricaComboVarieta(objParametri_Server, objParametri_Utenti)

            r.RispostaStringa = AgronicaCoreUtility.CaricaListControl.itemsCollection_AsJsonArray(d1.ddl_Varieta.Items, "Cul_Cod", "Cul_Des")

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                        AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

#End Region

    <WebMethod()>
    Public Function proponiVar(ByVal objP_server As String, ByVal varieta As String) As String
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)

        Try
            Dim strRisposta = ""

            Dim dbAccess As New DBCalibrature_R(objParametri_Server)

            Dim res = dbAccess.leggiSpecieEVarietaDataStringa(varieta)
            If res IsNot Nothing Then
                strRisposta = res(1)
            End If

            r.RispostaOK = True
            r.RispostaStringa = strRisposta

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r.RispostaStringa
    End Function

    <WebMethod()>
    Public Function proponiCalibro(ByVal objP_server As String, ByVal calibro As String, veg_cod As Integer) As String
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)

        Try
            Dim strRisposta = ""

            Dim dbAccess As New DBCalibrature_R(objParametri_Server)

            Dim res = dbAccess.leggiCalibroDataStreVegCod(calibro, veg_cod)
            If res IsNot Nothing Then
                strRisposta = res
            End If

            r.RispostaOK = True
            r.RispostaStringa = strRisposta

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r.RispostaStringa
    End Function

    <WebMethod()>
    Public Function ProponiContatto(ByVal objP_server As String, ByVal objP_utenti As String, contattoCalibratura As String) As String
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_utenti)

        Try
            Dim strRisposta = ""

            Dim dbAccess As New DBCalibrature_R(objParametri_Server)

            Dim res = dbAccess.leggiContattoDataStr(contattoCalibratura)
            If res IsNot Nothing Then
                strRisposta = res
            End If

            r.RispostaOK = True
            r.RispostaStringa = strRisposta

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r.RispostaStringa
    End Function



#Region "Script services lettura causali"

    <WebMethod()>
    Public Function LeggiCausaliEntrataProdotto(ByVal objP_server As String, ByVal objP_utenti As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_utenti)

        Try
            Dim leggi As New Operazioni_R
            r.RispostaStringa = leggi.Leggi_Causali_Entrata_Prodotto(objParametri_Server)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                        AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

#End Region


    <WebMethod()>
    Public Function leggiConferitori(ByVal objP_server As String, ByVal objP_utenti As String) As String
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_utenti)

        Try
            Dim strRisposta = ""

            Dim ddlApp As New DropDownList

            Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R

            ddlApp.Items.Clear()
            Dim dtContatti As DataTable
            dtContatti = objContatti.Contatti_Contatto_Leggi("",
                                                             "",
                                                             0,
                                                             -3,
                                                             True,
                                                             False,
                                                             0,
                                                             0,
                                                             False,
                                                             0,
                                                             0,
                                                             0,
                                                             "",
                                                             False,
                                                             0, 0, 0, 0, 0,
                                                             0,
                                                             "", "", objParametri_Server)

            For i = 0 To dtContatti.Rows.Count - 1

                Dim ragSoc = CStr(dtContatti.Rows(i).Item("Rag_Soc")) +
                             CStr(dtContatti.Rows(i).Item("Cognome")) + " " +
                             CStr(dtContatti.Rows(i).Item("Nome"))

                ddlApp.Items.Add(New ListItem(ragSoc + " (" + dtContatti.Rows(i).Item("Rapporto_Des") + ")",
                                              dtContatti.Rows(i).Item("Cod_RisUm")))


            Next

            Dim rval1 As New List(Of String)
            Dim rval As String = "{""op"":["
            For Each elem As ListItem In ddlApp.Items

                rval1.Add("{""value"": """ & elem.Value & """, ""text"":""" & elem.Text & """} ")

            Next

            rval &= String.Join("," & vbCrLf, rval1.ToArray)


            rval &= "]}"

            r.RispostaOK = True
            r.RispostaStringa = rval

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r.RispostaStringa
    End Function

    <WebMethod()>
    Public Function TranscodificaSpecieVarieta(ByVal objP_server As String, ByVal objP_utenti As String, cal As String, specie As Long, varieta As Long) As String
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_utenti)

        Try
            Dim strRisposta = ""

            Dim writer = New DBCalibrature_W(objParametri_Server)

            writer.inseriscriTranscodificaSpecieXCultivar(cal, specie, varieta)

            strRisposta = "OK"

            r.RispostaStringa = strRisposta
        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
            r.RispostaStringa = r.Errore
        End Try

        Return r.RispostaStringa
    End Function

    <WebMethod()>
    Public Function TranscodificaContatto(ByVal objP_server As String, ByVal objP_utenti As String, contatto As String, contattoCod As Long) As String
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_utenti)

        Try
            Dim strRisposta = ""

            Dim writer = New DBCalibrature_W(objParametri_Server)

            writer.inserisciTranscodificaContatto(contatto, contattoCod)

            strRisposta = "OK"

            r.RispostaStringa = strRisposta
        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
            r.RispostaStringa = r.Errore
        End Try

        Return r.RispostaStringa
    End Function

    <WebMethod()>
    Public Function TranscodificaCalibri(ByVal objP_server As String, ByVal objP_utenti As String, calibroCal As String, tabella_par_cod As Long, veg_cod As Long) As String
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_utenti)

        Try
            Dim strRisposta = ""

            Dim writer = New DBCalibrature_W(objParametri_Server)

            writer.inserisciTranscodificaCalibri(calibroCal, tabella_par_cod, veg_cod)

            strRisposta = "OK"

            r.RispostaStringa = strRisposta
        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
            r.RispostaStringa = r.Errore
        End Try

        Return r.RispostaStringa
    End Function

    <WebMethod()>
    Public Function SalvaSpecieVarietaPerCalibratura(ByVal objP_server As String, ByVal objP_utenti As String, idCal As Integer, specie As Long, varieta As Long) As String
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_utenti)

        Try
            Dim strRisposta = ""

            Dim writer = New DBCalibrature_W(objParametri_Server)

            writer.SalvaSpecieVarietaPerCalibratura(idCal, specie, varieta)

            strRisposta = "OK"

            r.RispostaStringa = strRisposta
        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
            r.RispostaStringa = r.Errore
        End Try

        Return r.RispostaStringa
    End Function

    <WebMethod()>
    Public Function SalvaConferitorePerCalibratura(ByVal objP_server As String, ByVal objP_utenti As String, idCal As Integer, contatto As Long) As String
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_utenti)

        Try
            Dim strRisposta = ""

            Dim writer = New DBCalibrature_W(objParametri_Server)

            writer.SalvaConferitorePerCalibratura(idCal, contatto)

            strRisposta = "OK"

            r.RispostaStringa = strRisposta
        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
            r.RispostaStringa = r.Errore
        End Try

        Return r.RispostaStringa
    End Function

    <WebMethod()>
    Public Function SalvaCalibroPerCalibroGIAS(ByVal objP_server As String, ByVal objP_utenti As String, idCal As Integer, nome As String, par_cod_GIAS As Long) As String
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_utenti)

        Try
            Dim strRisposta = ""

            Dim writer = New DBCalibrature_W(objParametri_Server)

            writer.SalvaCalibroPerCalibroGIAS(idCal, nome, par_cod_GIAS)

            strRisposta = "OK"

            r.RispostaStringa = strRisposta
        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
            r.RispostaStringa = r.Errore
        End Try

        Return r.RispostaStringa
    End Function

    <WebMethod()>
    Public Function VerificaSelezionati(ByVal objP_server As String, ByVal objP_utenti As String, ids As String) As String
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_utenti)

        Dim importatore As ImportaCalibratureGIAS
        importatore = New ImportaCalibratureGIAS(objParametri_Server)
        Try
            Dim strRisposta = ""
            Dim idValido = False
            Dim id As Integer
            For Each idString As String In ids.Split("|")

                Try
                    If idString <> "" Then
                        id = CInt(idString)
                        idValido = True
                    End If

                Catch ex As Exception
                    idValido = False
                End Try

                If idValido Then
                    importatore.verificaCalibratura(id)
                End If
                idValido = False
            Next

            strRisposta = "OK"

            r.RispostaStringa = strRisposta
        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
            r.RispostaStringa = r.Errore
        End Try

        Return r.RispostaStringa
    End Function

    <WebMethod()>
    Public Function getStato(ByVal objP_server As String, ByVal objP_utenti As String, idCal As Integer) As String
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_utenti)

        Dim importatore As ImportaCalibratureGIAS
        importatore = New ImportaCalibratureGIAS(objParametri_Server)
        Try
            Dim strRisposta As String

            Dim reader As New DBCalibrature_R(objParametri_Server)

            strRisposta = CStr(reader.getStatoCalibratura(idCal))

            r.RispostaStringa = strRisposta
        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
            r.RispostaStringa = r.Errore
        End Try

        Return r.RispostaStringa
    End Function

    <WebMethod()>
    Public Function AggiornaStato(ByVal objP_server As String, ByVal objP_utenti As String, idCal As Integer, stato As Integer) As String
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_utenti)

        Dim importatore As ImportaCalibratureGIAS
        importatore = New ImportaCalibratureGIAS(objParametri_Server)
        Try
            Dim strRisposta As String

            Dim writer As New DBCalibrature_W(objParametri_Server)

            writer.aggiornaStatoCalibratura(idCal, stato)

            strRisposta = "OK"

            r.RispostaStringa = strRisposta
        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
            r.RispostaStringa = r.Errore
        End Try

        Return r.RispostaStringa
    End Function

    <WebMethod()>
    Public Function LeggiPreparazioni(ByVal objP_server As String, ByVal objP_utenti As String, ByVal piva As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_utenti)

        Try

            Dim reader As New Linee_Preparazioni_R
            Dim dt = reader.Leggi(piva, 0, 0, 0, "Preparazione_cod != -208", "Preparazione_Des", objParametri_Server)

            Dim neededCols As New List(Of String) From {"Preparazione_Des", "Preparazione_Cod", "Preparazione_Sigla"}

            Dim rows As New List(Of Dictionary(Of String, Object))
            Dim row As Dictionary(Of String, Object)

            For Each dr As DataRow In dt.Rows

                row = New Dictionary(Of String, Object)
                For Each col As DataColumn In dt.Columns
                    If neededCols.Contains(col.ColumnName) Then
                        row.Add(col.ColumnName, dr(col))
                    End If
                Next
                rows.Add(row)

            Next

            Dim serializerSettings As New JsonSerializerSettings With {
                .ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }
            r.RispostaStringa = JsonConvert.SerializeObject(rows, Formatting.None, serializerSettings)
            'r.RispostaStringa = "[{""Preparazione_Des"":""Calibratura"", ""Preparazione_Cod"":-133, ""Codice_Generazione"":395}, " +
            '                "{""Preparazione_Des"":""Confezionamento"", ""Preparazione_Cod"":-137, ""Codice_Generazione"":396}]"
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod()>
    Public Function LeggiPreparazioniProdottiPresenti(ByVal objP_server As String, ByVal objP_utenti As String, ByVal piva As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_utenti)

        Try

            Dim reader As New Linee_Preparazioni_R
            Dim dt = reader.LeggiProdottiPresenti(piva, 0, 0, 0, "Preparazione_cod != -208", "Preparazione_Des", objParametri_Server)

            Dim neededCols As New List(Of String) From {"Preparazione_Des", "Preparazione_Cod", "Preparazione_Sigla"}

            Dim rows As New List(Of Dictionary(Of String, Object))
            Dim row As Dictionary(Of String, Object)

            For Each dr As DataRow In dt.Rows

                row = New Dictionary(Of String, Object)
                For Each col As DataColumn In dt.Columns
                    If neededCols.Contains(col.ColumnName) Then
                        row.Add(col.ColumnName, dr(col))
                    End If
                Next
                rows.Add(row)

            Next

            Dim serializerSettings As New JsonSerializerSettings With {
                .ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }
            r.RispostaStringa = JsonConvert.SerializeObject(rows, Formatting.None, serializerSettings)
            'r.RispostaStringa = "[{""Preparazione_Des"":""Calibratura"", ""Preparazione_Cod"":-133, ""Codice_Generazione"":395}, " +
            '                "{""Preparazione_Des"":""Confezionamento"", ""Preparazione_Cod"":-137, ""Codice_Generazione"":396}]"
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod()>
    Public Function LeggiProdotti_FF(ByVal objP_server As String, ByVal objP_utenti As String, ByVal piva As String,
                                  ByVal Mat_Cod As Integer, ByVal Elem_Cod As String,
                                  ByVal soloMovimentato As Boolean, ByVal filters As String, ByVal soloLegatiALinea As Boolean) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_utenti)

        Try
            Dim leggi_mp As New Materie_Prime_R
            r.RispostaStringa =
                leggi_mp.Leggi_MateriePrime_Conferimento_ConControlloMovimentato(piva, Elem_Cod, Mat_Cod, soloMovimentato, filters, soloLegatiALinea, objParametri_Server, objParametri_Utenti)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    <WebMethod()>
    Public Function LeggiLineeProduzione(ByVal objP_server As String, ByVal objP_utenti As String, ByVal piva As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_utenti)

        Try

            Dim reader As New Linee_Produzioni_R
            Dim dt = reader.LeggiLineeProduzione(piva, "", "Linea_Des", objParametri_Server)

            Dim neededCols As New List(Of String) From {"Linea_Des", "Linea_Cod"}

            Dim rows As New List(Of Dictionary(Of String, Object))
            Dim row As Dictionary(Of String, Object)

            For Each dr As DataRow In dt.Rows

                row = New Dictionary(Of String, Object)
                For Each col As DataColumn In dt.Columns
                    If neededCols.Contains(col.ColumnName) Then
                        row.Add(col.ColumnName, dr(col))
                    End If
                Next
                rows.Add(row)

            Next

            Dim serializerSettings As New JsonSerializerSettings With {
                .ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }
            r.RispostaStringa = JsonConvert.SerializeObject(rows, Formatting.None, serializerSettings)

            r.RispostaOK = True


        Catch ex As Exception

            r.RispostaOK = False
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function

    <WebMethod()>
    Public Function LeggiPreparazioneDaLinea(ByVal objP_server As String, ByVal objP_utenti As String, ByVal piva As String,
                                             ByVal codice_generazione As Integer,
                                             ByVal preparazione_cod As Integer,
                                             ByVal linea_cod As Integer)

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_utenti)

        Try

            Dim leggi As New Linee_Preparazioni_R
            r.RispostaStringa = leggi.LeggiPreparazioneDaLinea(piva, codice_generazione, preparazione_cod, linea_cod, objParametri_Server)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function

    <WebMethod()>
    Public Function LeggiProdottiPerSpecificaLavorazione(ByVal objP_server As String, ByVal objP_utenti As String, ByVal piva As String,
                                    ByVal linee_Preparazioni_CodiceGenerazione As Integer,
                                    ByVal oGenerazioni_Anagrafe_Log_CodiceGenerazione As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_utenti)

        Try

            Dim leggi As New OGenerazioni_Anagrafe_Log_R
            r.RispostaStringa = leggi.LeggiProdottiPerSpecificaLavorazione(piva, linee_Preparazioni_CodiceGenerazione, oGenerazioni_Anagrafe_Log_CodiceGenerazione, objParametri_Server)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function LeggiGruppiFatturazione(ByVal objP_server As String, ByVal objP_utenti As String, ByVal piva As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_utenti)

        Try
            Dim leggi As New FF_CampionamentoConferimento_R
            r.RispostaStringa = leggi.LeggiTabelleConsultazione(piva, TipiEnumerativi.enum_OTabelle.GruppoFatturazione, objParametri_Server)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                        AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    <WebMethod()>
    Public Function LeggiDaBarcode(ByVal objP_server As String, ByVal objP_utenti As String, ByVal piva As String, ByVal barcode As String, ByVal gen_cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_utenti)

        Try

            Dim dt As New DataTable
            Dim reader As New FF_MagazzinoBIZ
            Dim reader2 As New FF_CampionamentoConferimento_R

            Dim str = reader.Leggi_Giacenza(piva, 0, -1, {}, 0, 0, "210", "", "0", barcode, 0, 0, 0, 0, 0, 0, 0, 0, "", "0", "5", False, False, False, True, "", "", dt, objParametri_Server, objParametri_Utenti)

            Dim rows As New List(Of Dictionary(Of String, Object))
            Dim row As Dictionary(Of String, Object)

            For Each dr As DataRow In dt.Rows

                row = New Dictionary(Of String, Object)
                For Each col As DataColumn In dt.Columns
                    row.Add(col.ColumnName, dr(col))
                Next

                Dim cal_mat_cod = 0
                If gen_cod <> 0 Then

                    Dim cal_mat_str = reader2.LeggiLavoratoDaProdotto(piva, dr("Mat_Cod"), gen_cod, objParametri_Server)
                    Dim cal_mat_obj = JsonConvert.DeserializeObject(cal_mat_str)
                    If cal_mat_obj.Count > 0 Then
                        cal_mat_cod = cal_mat_obj(0)("mat_cod")
                    End If

                End If

                row.Add("Cal_Mat_Cod", cal_mat_cod)

                rows.Add(row)

            Next


            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(rows, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                        AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod()>
    Public Function LeggiLiquidazioni(ByVal objP_server As String, ByVal objP_utenti As String, ByVal piva As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_utenti)

        Try
            Dim leggi As New FF_CampionamentoConferimento_R
            r.RispostaStringa = leggi.Leggi_LiquidazioniPerLancio(piva, objParametri_Server)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function



    <WebMethod()>
    Public Function LeggiDestinazioneAccettazioneDefault(ByVal objP_server As String, ByVal objP_utenti As String, ByVal piva As String, ByVal sa_cod As Long, ByVal modulo_generazione As Integer, ByVal mat_cod As Long) As RispostaStandard

        Dim r As New RispostaStandard
        Dim DT As New DataTable
        Dim DT_MP As New DataTable

        Try

            Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_utenti)


            Dim leggi As New Magazzino_DAL_R
            Dim leggi_MP As New Materie_Prime_R

            'Lettura del prodotto
            DT_MP = leggi_MP.Leggi(piva, 0, 0, mat_cod, "", 0, 0, 0, 0, 0, 0, 0, "", 0, "", True, False, "", enumSelezioneVariabile.Selezione_JoinCompleta, "", "", objParametri_Server)

            If DT_MP.Rows.Count > 0 Then

                Select Case DT_MP(0).Item("ChkReferenza")

                    Case 0

                        'Referenza --> Aggancio ad anagrafica Omni
                        mat_cod = DT_MP(0).Item("Mat_Cod_Referenza")

                    Case 1

                        'Omni

                End Select

            End If

            DT = leggi.LeggiDestinazioneAccettazioneDefault(piva, sa_cod, modulo_generazione, mat_cod, objParametri_Server)


            If DT.Rows.Count > 0 And mat_cod <> 0 Then

                r.RispostaStringa = DT(0).Item("Tipo_Destinazione") & "_" & DT(0).Item("Sa_Cod_Destinazione") & "_" & DT(0).Item("Id_Destinazione")
                r.RispostaOK = True


            Else

                r.RispostaStringa = "Destinazione Indefinita"
                r.RispostaOK = False


            End If


        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function







    '######################################################################################################################
#Region "Metodi normali"

    Function DammiWaTableCalibrature(ByVal dt As DataTable) As String
        Dim l As New List(Of ColonneNome)

        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        l = JSON_DataTable.getListaColonneFromDT(dt)

        l.Item(UtilityFreshAndFood.ColCalibri.Check)._FormatoParticolare = "300"
        l.Item(UtilityFreshAndFood.ColCalibri.Check)._Nome_colonna_Json = "S"

        l.Item(UtilityFreshAndFood.ColCalibri.id)._FormatoParticolare = "300"

        l.Item(UtilityFreshAndFood.ColCalibri.Conferitore_codice)._FormatoParticolare = "400"
        l.Item(UtilityFreshAndFood.ColCalibri.Conferitore_codice)._Nome_colonna_Json = "cod"

        l.Item(UtilityFreshAndFood.ColCalibri.Conferitore_nome)._FormatoParticolare = "2000"
        l.Item(UtilityFreshAndFood.ColCalibri.Conferitore_nome)._Nome_colonna_Json = "Conferitore"

        l.Item(UtilityFreshAndFood.ColCalibri.Contatto_GIAS)._FormatoParticolare = "6000"
        l.Item(UtilityFreshAndFood.ColCalibri.Contatto_GIAS)._Nome_colonna_Json = "Contatto GIAS"

        l.Item(UtilityFreshAndFood.ColCalibri.Data_inizio)._FormatoParticolare = "2000"
        l.Item(UtilityFreshAndFood.ColCalibri.Data_inizio)._Nome_colonna_Json = "Data Inizio"

        l.Item(UtilityFreshAndFood.ColCalibri.Data_Fine)._FormatoParticolare = "2000"
        l.Item(UtilityFreshAndFood.ColCalibri.Data_Fine)._Nome_colonna_Json = "Data Fine"

        l.Item(UtilityFreshAndFood.ColCalibri.Lotto)._FormatoParticolare = "1000"

        l.Item(UtilityFreshAndFood.ColCalibri.Varieta)._FormatoParticolare = "1500"

        l.Item(UtilityFreshAndFood.ColCalibri.SpecieVegetale_GIAS)._FormatoParticolare = "2000"
        l.Item(UtilityFreshAndFood.ColCalibri.SpecieVegetale_GIAS)._Nome_colonna_Json = "Specie Vegetale GIAS"

        l.Item(UtilityFreshAndFood.ColCalibri.Varieta_GIAS)._FormatoParticolare = "2000"
        l.Item(UtilityFreshAndFood.ColCalibri.Varieta_GIAS)._Nome_colonna_Json = "Varieta GIAS"

        l.Item(UtilityFreshAndFood.ColCalibri.Stato)._FormatoParticolare = "4000"
        l.Item(UtilityFreshAndFood.ColCalibri.Stato)._Nome_colonna_Json = "Stato"

        l.Item(UtilityFreshAndFood.ColCalibri.Cod_ContattoGIAS)._FormatoParticolare = "0"
        l.Item(UtilityFreshAndFood.ColCalibri.Cod_ContattoGIAS)._Nome_colonna_Json = "Cod_ContattoGIAS"

        l.Item(UtilityFreshAndFood.ColCalibri.Cod_SpecieVegetaleGIAS)._FormatoParticolare = "0"
        l.Item(UtilityFreshAndFood.ColCalibri.Cod_SpecieVegetaleGIAS)._Nome_colonna_Json = "Cod_SpecieVegetaleGIAS"

        l.Item(UtilityFreshAndFood.ColCalibri.Cod_VarietaGIAS)._FormatoParticolare = "0"
        l.Item(UtilityFreshAndFood.ColCalibri.Cod_VarietaGIAS)._Nome_colonna_Json = "Cod_VarietaGIAS"

        l.Item(UtilityFreshAndFood.ColCalibri.Cod_Stato)._FormatoParticolare = "0"
        l.Item(UtilityFreshAndFood.ColCalibri.Cod_Stato)._Nome_colonna_Json = "Cod_Stato"

        Dim risp As String = js.JSON_DataTable(dt, l)
        Return risp
    End Function

    Function DammiWaTableCalibratureXCalibri(ByVal dt As DataTable) As String
        Dim l As New List(Of ColonneNome)

        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        l = JSON_DataTable.getListaColonneFromDT(dt)

        l.Item(UtilityFreshAndFood.CalibratureXCalibri.ID_Calibro)._FormatoParticolare = "300"
        l.Item(UtilityFreshAndFood.CalibratureXCalibri.ID_Calibro)._Nome_colonna_Json = "ID"

        l.Item(UtilityFreshAndFood.CalibratureXCalibri.Nome)._FormatoParticolare = "1000"

        l.Item(UtilityFreshAndFood.CalibratureXCalibri.Calibro_GIAS)._FormatoParticolare = "2000"
        l.Item(UtilityFreshAndFood.CalibratureXCalibri.Calibro_GIAS)._Nome_colonna_Json = "Calibro GIAS"

        l.Item(UtilityFreshAndFood.CalibratureXCalibri.Numero)._FormatoParticolare = "1000"

        l.Item(UtilityFreshAndFood.CalibratureXCalibri.Peso)._FormatoParticolare = "1000"

        l.Item(UtilityFreshAndFood.CalibratureXCalibri.Cod_CalibroGIAS)._FormatoParticolare = "0"

        Dim risp As String = js.JSON_DataTable(dt, l)
        Return risp

    End Function

#End Region

End Class