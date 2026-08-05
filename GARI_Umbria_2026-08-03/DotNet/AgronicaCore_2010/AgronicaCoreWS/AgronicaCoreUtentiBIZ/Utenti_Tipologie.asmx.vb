Imports System.Web.Services
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDTOStd.InData.Agenda
Imports AgronicaCoreModelsSTD.baseClass
Imports AgronicaCoreWS.My.Resources
Imports AgronicaCoreModelsSTD.profilazione
Imports AgronicaCoreDTOStd.InData.Utility

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
Public Class Utenti_Tipologie
    Inherits WebService

    Class JSON_Result
        Public Operazione As enum_TipoOperazioneDB
        Public Tipologie As BaseCodeDescr()
    End Class

    Class JSON_PermessoTipologia_Result
        Public Operazione As enum_TipoOperazioneDB
        Public Tipologia As BaseCodeDescr
        Public Attivita As BaseCodeDescr()
    End Class

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function ListaTipologie(InData As Object) As rispostaStandard(Of IEnumerable(Of TipologiaUtente))

        Dim r As New rispostaStandard(Of IEnumerable(Of TipologiaUtente))
        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
        Dim iData As CoreWS_Generic(Of String) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData), a)

        If iData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If
        If iData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)
            Dim objUtentiBIZ As New AgronicaCoreUtentiBIZ.Tipologie
            Dim tipologie = objUtentiBIZ.Carica_Tipologie(objParametri_Utenti)

            r.RispostaStringa = tipologie
            r.RispostaOK = True
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function ListaTipologiexPermessi_NG(InData As Object) As RispostaStandard
        Dim res As New RispostaStandard
        InData = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData))

        Dim obj_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(InData.objP.objP_server)
        Dim obj_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
        Dim objUtentiBIZ As New AgronicaCoreUtentiBIZ.Tipologie

        Try
            Dim txp = objUtentiBIZ.CaricaPermessiTipologie(New ObjParams With {
                .ObjParametri_Utenti = obj_Utenti,
                .ObjParametri_Server = obj_Server
            }).ToList()
            res.RispostaStringa = JsonConvert.SerializeObject(txp, Formatting.None)
            res.RispostaOK = True
        Catch ex As Exception
            res.RispostaOK = False
            res.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return res
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function ScriviTipologie_NG(InData As CoreWS_Generic(Of TipologiaUtente())) As RispostaStandard
        Dim res As New RispostaStandard
        Dim objUtentiBIZ = New AgronicaCoreUtentiBIZ.Tipologie
        Dim params = New ObjParams With {
            .ObjParametri_Server = Utility.convertStringtoOBJparametri(InData.objP.objP_server),
            .ObjParametri_Utenti = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
        }
        Try
            objUtentiBIZ.Scrivi_Tipologie(InData.InData, params)
            res.RispostaOK = True
        Catch ex As Exception
            res.RispostaOK = False
            res.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try
        Return res
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function CopiaTipologia_NG(InData As CoreWS_Generic(Of CopyProfileObj)) As RispostaStandard
        Dim res As New RispostaStandard
        Dim profilesBIZ = New AgronicaCoreUtentiBIZ.Tipologie
        Dim params = New ObjParams With {
            .ObjParametri_Server = Utility.convertStringtoOBJparametri(InData.objP.objP_server),
            .ObjParametri_Utenti = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
        }
        Try
            Dim original As BaseCodeDescr = InData.InData.Original
            Dim newProfile As BaseCodeDescr = InData.InData.CopyTemplate
            profilesBIZ.CopiaTipologia(original, newProfile, InData.InData.AlsoCopySettings, params)
            res.RispostaOK = True
        Catch ex As Exception
            res.RispostaOK = False
            res.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try
        Return res
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function CancellaTipologia_NG(InData As CoreWS_Generic(Of TipologiaUtente)) As RispostaStandard
        Dim res As New RispostaStandard

        Dim obj_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(InData.objP.objP_server)
        Dim obj_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

        Dim objUtentiBIZ = New AgronicaCoreUtentiBIZ.Tipologie

        Try
            objUtentiBIZ.Elimina_Tipologia(InData.InData.codice, obj_Server, obj_Utenti)

            res.RispostaOK = True
        Catch ex As Exception
            res.RispostaOK = False
            res.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return res

    End Function
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function CancellaTipologia(data As Object, objParametri_Utenti As String, objParametri_Server As String) As RispostaStandard
        Dim res As New RispostaStandard

        Dim obj_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objParametri_Server)
        Dim obj_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objParametri_Utenti)

        Dim objUtentiBIZ = New AgronicaCoreUtentiBIZ.Tipologie

        Try
            Dim params = JsonConvert.DeserializeObject(Of BaseCodeDescr)(data)

            objUtentiBIZ.Elimina_Tipologia(params.codice, obj_Server, obj_Utenti)

            res.RispostaOK = True
        Catch ex As Exception
            res.RispostaOK = False
            res.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return res

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function AssegnaPermesso_NG(InData As CoreWS_Generic(Of TipologiaUtente)) As RispostaStandard
        Dim res As New RispostaStandard
        Dim params As New ObjParams With {
            .ObjParametri_Server = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(InData.objP.objP_server),
            .ObjParametri_Utenti = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
        }
        Dim objUtentiBIZ As New AgronicaCoreUtentiBIZ.Tipologie
        Dim tipologia = InData.InData
        Dim optimizeMassive = False
        Try
            If tipologia.codice < 0 Then
                tipologia.codice = -tipologia.codice
                optimizeMassive = True
            End If
            objUtentiBIZ.Aggiorna_Permessi_Tipologia(
                tipologia.codice,
                tipologia.Permessi.Select(Function(p) New BaseCodeDescr(p.Permesso_ID, "")).ToArray(),
                tipologia.Permessi.FirstOrDefault().Permesso_Tipo,
                params
            )
            res.RispostaOK = True
        Catch ex As Exception
            res.RispostaOK = False
            res.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try
        Return res
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function AggiornaPermessiUtentiTipologia(InData As CoreWS_Generic(Of TipologiaUtente)) As RispostaStandard
        Dim res As New RispostaStandard
        Dim getsoreTipologie As New AgronicaCoreUtentiBIZ.Tipologie
        Dim params As New ObjParams With {
            .ObjParametri_Utenti = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(InData.objP.objP_utenti),
            .ObjParametri_Server = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(InData.objP.objP_server)
        }
        Try
            getsoreTipologie.AggiornaPermessiUtentiCollegati(InData.InData, params)
            res.RispostaOK = True
        Catch ex As Exception
            res.RispostaOK = False
            res.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try
        Return res
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function AggiornaImpostazioniUtentiTipologia(InData As CoreWS_Generic(Of AssociaProfiloObj)) As RispostaStandard
        Dim res As New RispostaStandard
        Dim getsoreTipologie As New AgronicaCoreUtentiBIZ.Tipologie
        Dim params As New ObjParams With {
            .ObjParametri_Utenti = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(InData.objP.objP_utenti),
            .ObjParametri_Server = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(InData.objP.objP_server)
        }
        Try
            getsoreTipologie.AggiornaImpostzioniUtentiCollegati(InData.InData, params)
            res.RispostaOK = True
        Catch ex As Exception
            res.RispostaOK = False
            res.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try
        Return res
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function HaImpostazioniPermessiCollegati(InData As CoreWS_Generic(Of TipologiaUtente)) As RispostaStandard
        Dim res As New RispostaStandard
        Dim obj_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
        Dim objTipologie = New AgronicaCoreUtentiBIZ.Tipologie
        Try
            Dim objRisposta = New With {
                .haImpostazioni = objTipologie.HaImpostazioniCollegate(InData.InData.codice, obj_Utenti),
                .haPermessi = objTipologie.HaPermessiCollegati(InData.InData.codice, obj_Utenti)
            }
            res.RispostaStringa = JsonConvert.SerializeObject(objRisposta)
            res.RispostaOK = True
        Catch ex As Exception
            res.RispostaOK = False
            res.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try
        Return res
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function LeggiUtentiCollegati(InData As CoreWS_Generic(Of TipologiaUtente)) As RispostaStandard
        Dim res As New RispostaStandard
        Dim obj_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
        Dim objTipologie = New AgronicaCoreUtentiBIZ.Tipologie
        Try
            Dim objRisposta = objTipologie.getUtentiCollegati(InData.InData, obj_Utenti)
            res.RispostaStringa = JsonConvert.SerializeObject(objRisposta)
            res.RispostaOK = True
        Catch ex As Exception
            res.RispostaOK = False
            res.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try
        Return res
    End Function

End Class

