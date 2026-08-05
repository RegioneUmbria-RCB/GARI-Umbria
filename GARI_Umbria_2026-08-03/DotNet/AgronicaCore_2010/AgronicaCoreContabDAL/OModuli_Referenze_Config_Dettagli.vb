Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Data.Entity
Imports System.Text

Public Class OModuli_Referenze_Config_Dettagli_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function LeggiParametriQualitativi(ByVal piva As String,
                                              ByRef objParametri As AgronicaCoreParametri
                                              ) As String

        Const nomeRoutine = "AgronicaCoreContabDAL.OModuli_Referenze_Config_Dettagli_R.LeggiParametriQualitativi()"
        Dim risposta As String = ""

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim ParamQual =
             From parametri_qualitativi In GiasContext.OModuli_Referenze_Config_Dettagli
             Join otab In GiasContext.OTabelle
                 On CStr(otab.Tabella_Cod) Equals parametri_qualitativi.Tabella_ID
             Where parametri_qualitativi.Piva.Equals(piva) And
                   parametri_qualitativi.Tipo = 1 And
                   parametri_qualitativi.Tabella_ID <> "F"
             Order By
                  parametri_qualitativi.Ordine
             Select New With {
                 .Tabella_ID = parametri_qualitativi.Tabella_ID,
                 .Tabella_Des = otab.Tabella_Des,
                 .Tabella_Cod_Des = otab.Tabella_Cod_Des.ToLower,
                 .Tipo = otab.Tipo,
                 .Valore_Minimo = otab.Valore_Minimo,
                 .Valore_Maximo = otab.Valore_Maximo,
                 .NumDecimali_Maximo = otab.NumDecimali_Maximo
             }

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            risposta = JsonConvert.SerializeObject(ParamQual.Distinct().ToList(), Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function

    Public Function LeggiParametriQualitativiXConfigurazione(ByVal piva As String,
                                              ByRef objParametri As AgronicaCoreParametri
                                              ) As String

        Const nomeRoutine = "AgronicaCoreContabDAL.OModuli_Referenze_Config_Dettagli_R.LeggiParametriQualitativiXConfigurazione()"
        Dim risposta As String = ""

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim ParamQual =
             From parametri_qualitativi In GiasContext.OModuli_Referenze_Config_Dettagli
             Join otab In GiasContext.OTabelle
                 On CStr(otab.Tabella_Cod) Equals parametri_qualitativi.Tabella_ID
             Group Join testate In GiasContext.OModuli_Referenze_Config_Testata
                 On parametri_qualitativi.Id_Testata Equals testate.Id_Testata
                 Into testate_parametri = Group From tp In testate_parametri.DefaultIfEmpty
             Where parametri_qualitativi.Piva.Equals(piva) And
                 parametri_qualitativi.Tipo = 1 'And Not (New String() {"C", "F"}).Contains(parametri_qualitativi.Tabella_ID) ' Tipo parametro di OTabelle = "Scelta da lista"
             Select New With {
                 .IdParam = parametri_qualitativi.Piva & "_" & parametri_qualitativi.Id_Testata & "_" & parametri_qualitativi.Tipo & "_" & parametri_qualitativi.Tabella_ID,
                 .Piva = parametri_qualitativi.Piva,
                 .Id_Testata = parametri_qualitativi.Id_Testata,
                 .Id_Testata_Des = If(tp Is Nothing, "", tp.Descrizione),
                 .Tipo = otab.Tipo,
                 .Tabella_ID = parametri_qualitativi.Tabella_ID,
                 .Tabella_Key = otab.Tabella_Cod_Des.ToLower(),
                 .Tabella_Cod_Des = otab.Tabella_Cod_Des.ToLower(),
                 .Tabella_Des = otab.Tabella_Des,
                 .Configurazione_Des = parametri_qualitativi.Configurazione_Des,
                 .Ordine = parametri_qualitativi.Ordine,
                 .Tabella_Key_Rif = parametri_qualitativi.Tabella_Key_Rif,
                 .ChkReferenza = parametri_qualitativi.ChkReferenza,
                 .ChkOmni_Invisibili = parametri_qualitativi.ChkOmni_Invisibili,
                 .ChkEtichetta = parametri_qualitativi.ChkEtichetta,
                 .ChkEdit = parametri_qualitativi.ChkEdit,
                 .ChkObbligatorio = parametri_qualitativi.ChkObbligatorio,
                 .Validita_Inizio = parametri_qualitativi.Validita_Inizio,
                 .Validita_Fine = parametri_qualitativi.Validita_Fine,
                 .Data_Creazione = parametri_qualitativi.Data_Creazione,
                 .Inviato = parametri_qualitativi.inviato,
                 .Username_Creazione = parametri_qualitativi.Username_Creazione
            }

            Dim result = ParamQual.Distinct() _
                .OrderBy(Function(x) x.Piva) _
                .ThenBy(Function(x) x.Id_Testata) _
                .ThenBy(Function(x) x.Ordine) _
                .ToList()

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            risposta = JsonConvert.SerializeObject(result, Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function

    '##############################################################################################
    Public Function LeggiParametriQualitativiXTestata(ByVal piva As String, Id_Testata As Integer,
                                              ByRef objParametri As AgronicaCoreParametri
                                              ) As List(Of OModuli_Referenze_Config_Dettagli)

        Const nomeRoutine = "AgronicaCoreContabDAL.OModuli_Referenze_Config_Dettagli_R.LeggiParametriQualitativiXTestata()"
        Dim result As List(Of OModuli_Referenze_Config_Dettagli)

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim ParamQual =
             From parametri_qualitativi In GiasContext.OModuli_Referenze_Config_Dettagli
             Join otab In GiasContext.OTabelle
                 On CStr(otab.Tabella_Cod) Equals parametri_qualitativi.Tabella_ID
             Group Join testate In GiasContext.OModuli_Referenze_Config_Testata
                 On parametri_qualitativi.Id_Testata Equals testate.Id_Testata
                 Into testate_parametri = Group From tp In testate_parametri.DefaultIfEmpty
             Where parametri_qualitativi.Piva.Equals(piva) And
                 parametri_qualitativi.Tipo = 1 And
                 parametri_qualitativi.Id_Testata = Id_Testata 'And Not (New String() {"C", "F"}).Contains(parametri_qualitativi.Tabella_ID) ' Tipo parametro di OTabelle = "Scelta da lista"
             Select parametri_qualitativi

            result = ParamQual.Distinct() _
                .OrderBy(Function(x) x.Piva) _
                .ThenBy(Function(x) x.Id_Testata) _
                .ThenBy(Function(x) x.Ordine) _
                .ToList()

        End Using

        Return result

    End Function

    '##############################################################################################
    Public Function LeggiParametriQualitativiFiltroSpecieVarieta(ByVal piva As String,
                                                                 ByVal modulo_generazione As Integer,
                                                                 ByVal veg_cod As Integer,
                                                                 ByVal cul_cod As Integer,
                                                                 ByRef objParametri As AgronicaCoreParametri
                                                                 ) As String

        Const nomeRoutine = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.LeggiParametriQualitativiFiltroSpecieVarieta()"
        Dim risposta As String = ""

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim ParamQual =
             From parametri_qualitativi In GiasContext.OModuli_Referenze_Config_Dettagli
             Join parametri_qualitativi_testata In GiasContext.OModuli_Referenze_Config_Testata
                 On parametri_qualitativi_testata.Piva Equals parametri_qualitativi.Piva And
                    parametri_qualitativi_testata.Id_Testata Equals parametri_qualitativi.Id_Testata
             Join otab In GiasContext.OTabelle
                 On CStr(otab.Tabella_Cod) Equals parametri_qualitativi.Tabella_ID
             Where
                  parametri_qualitativi_testata.Modulo_Generazione = modulo_generazione And
                  parametri_qualitativi.Piva.Equals(piva) And
                  (otab.Tipo = 1 Or otab.Tipo = 3 Or otab.Tipo = 4 Or otab.Tipo = 5) And
                  parametri_qualitativi.Tabella_ID <> "F" And
                  ((parametri_qualitativi_testata.OFiltro_Veg_Cod = "" OrElse parametri_qualitativi_testata.OFiltro_Veg_Cod = "0") OrElse
                    ((parametri_qualitativi_testata.OFiltro_Cul_Cod = "" OrElse parametri_qualitativi_testata.OFiltro_Cul_Cod = "0") AndAlso
                 (parametri_qualitativi_testata.OFiltro_Veg_Cod.Contains("|" & CStr(veg_cod) & "|") OrElse parametri_qualitativi_testata.OFiltro_Veg_Cod.Contains(" " & CStr(veg_cod) & "|") OrElse parametri_qualitativi_testata.OFiltro_Veg_Cod.Contains("|" & CStr(veg_cod) & " ") OrElse parametri_qualitativi_testata.OFiltro_Veg_Cod.StartsWith(CStr(veg_cod) & "|") OrElse parametri_qualitativi_testata.OFiltro_Veg_Cod.EndsWith("|" & CStr(veg_cod)))) OrElse
                    ((parametri_qualitativi_testata.OFiltro_Veg_Cod.Contains("|" & CStr(veg_cod) & "|") OrElse parametri_qualitativi_testata.OFiltro_Veg_Cod.Contains(" " & CStr(veg_cod) & "|") OrElse parametri_qualitativi_testata.OFiltro_Veg_Cod.Contains("|" & CStr(veg_cod) & " ") OrElse parametri_qualitativi_testata.OFiltro_Veg_Cod.StartsWith(CStr(veg_cod) & "|") OrElse parametri_qualitativi_testata.OFiltro_Veg_Cod.EndsWith("|" & CStr(veg_cod))) AndAlso
                 ((parametri_qualitativi_testata.OFiltro_Cul_Cod.Contains("|" & CStr(cul_cod) & "|") OrElse parametri_qualitativi_testata.OFiltro_Cul_Cod.Contains(" " & CStr(cul_cod) & "|") OrElse parametri_qualitativi_testata.OFiltro_Cul_Cod.Contains("|" & CStr(cul_cod) & " ") OrElse parametri_qualitativi_testata.OFiltro_Veg_Cod.StartsWith(CStr(cul_cod) & "|") OrElse parametri_qualitativi_testata.OFiltro_Veg_Cod.EndsWith("|" & CStr(cul_cod))))))
             Select New With {
                 .Tabella_ID = parametri_qualitativi.Tabella_ID,
                 .Tabella_Des = otab.Tabella_Des,
                 .Tabella_Cod_Des = otab.Tabella_Cod_Des.ToLower,
                 .Tipo = otab.Tipo,
                 .Ordine = parametri_qualitativi.Ordine,
                 .ChkObbligatorio = If(parametri_qualitativi.ChkObbligatorio, 0),
                 .Tabella_Key_Rif = If(parametri_qualitativi.Tabella_Key_Rif, ""),
                 .Valore_Minimo = otab.Valore_Minimo,
                 .Valore_Maximo = otab.Valore_Maximo,
                 .NumDecimali_Maximo = otab.NumDecimali_Maximo
             }

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            risposta = JsonConvert.SerializeObject(ParamQual.Distinct().ToList().OrderBy(Function(x) x.Ordine), Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function

    '##############################################################################################
    Public Function OTabelleDistinct(ByVal Piva As String,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreParametri
                                    ) As DataTable

        '----- Descrizione
        Const nomeRoutine As String = "AgronicaCoreContabDAL.OModuli_Referenze_Config_Dettagli.OTabelleDistinct()"

        '----- Variabili
        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT distinct  OTabelle.tabella_cod, OTabelle.Tipo, OTabelle.Tabella_Des, OTabelle.Tabella_Cod_Des  ")
            stb.AppendLine(" FROM OModuli_Referenze_Config_Testata  " & vbCrLf)
            stb.AppendLine(" INNER JOIN OModuli_Referenze_Config_Dettagli ON  OModuli_Referenze_Config_Dettagli.piva = OModuli_Referenze_Config_Testata.piva ")
            stb.AppendLine(" AND  OModuli_Referenze_Config_Dettagli.id_testata = OModuli_Referenze_Config_Testata.id_testata ")
            stb.AppendLine(" INNER JOIN   OTabelle ON CONVERT(varchar(50),OTabelle.Tabella_cod) = OModuli_Referenze_Config_Dettagli.Tabella_id ")
            stb.AppendLine(" AND OModuli_Referenze_Config_Testata.modulo_generazione = OTabelle.modulo_generazione ")
            stb.AppendLine(" WHERE  OModuli_Referenze_Config_Testata.piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            stb.AppendLine(" order by otabelle.tabella_cod ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function



    '##############################################################################################
    'query usata dal report F&F: esportazione excel conferimenti
    Public Function OTabelleParametriDistinct(ByVal Piva As String,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByRef objParametri As AgronicaCoreParametri
                                            ) As DataTable

        Const nomeRoutine As String = "AgronicaCoreContabDAL.OModuli_Referenze_Config_Dettagli.OTabelleParametriDistinct()"
        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0
            stb.AppendLine(" -- il DISTINCT è per avere le OTabelle (che vengono usate come nome di colonna) non ripetute per ogni OModuli_Referenze_Config_Dettagli")
            stb.AppendLine(" SELECT DISTINCT  OTabelle.tabella_cod, OTabelle.Tipo, OTabelle.Tabella_Des, OTabelle.Tabella_Cod_Des  ")
            stb.AppendLine(" , ISNULL(OTabelle_Parametri.Descrizione, '') AS Descrizione, ISNULL(OTabelle_Parametri.Sigla, '') AS Sigla, ISNULL(OTabelle_Parametri.Codice_Origine, '') AS Codice_Origine  ")
            stb.AppendLine(" -- se si aggiunge Tabella_Par_Cod poi ci possono essere più righe con la stessa descrizione di parametro, perchè ci possono essere più OModuli_Referenze_Config_Testata per specie diverse  ")
            stb.AppendLine(" , ISNULL(OTabelle_Parametri.Tabella_Par_Cod, 0) AS Tabella_Par_Cod   ")
            stb.AppendLine(" FROM OModuli_Referenze_Config_Testata  " & vbCrLf)
            stb.AppendLine(" INNER JOIN OModuli_Referenze_Config_Dettagli ON  OModuli_Referenze_Config_Dettagli.piva = OModuli_Referenze_Config_Testata.piva ")
            stb.AppendLine(" AND  OModuli_Referenze_Config_Dettagli.id_testata = OModuli_Referenze_Config_Testata.id_testata ")
            stb.AppendLine(" INNER JOIN   OTabelle ON CONVERT(varchar(50),OTabelle.Tabella_cod) = OModuli_Referenze_Config_Dettagli.Tabella_id ")
            stb.AppendLine(" AND OModuli_Referenze_Config_Testata.modulo_generazione = OTabelle.modulo_generazione ")
            stb.AppendLine(" LEFT OUTER JOIN OTabelle_Parametri ON OTabelle.Tabella_Cod= OTabelle_Parametri.Tabella_Cod AND OTabelle.Modulo_Generazione= OTabelle_Parametri.Modulo_Generazione")
            stb.AppendLine(" AND OModuli_Referenze_Config_Testata.modulo_generazione = OTabelle.modulo_generazione ")
            stb.AppendLine(" WHERE  OModuli_Referenze_Config_Testata.piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            stb.AppendLine(" ORDER BY otabelle.tabella_cod, descrizione ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    '##############################################################################################
    'query usata dal report F&F: esportazione excel conferimenti
    Public Function OTabelleParametri(ByVal Piva As String,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByRef objParametri As AgronicaCoreParametri
                                            ) As DataTable

        Const nomeRoutine As String = "AgronicaCoreContabDAL.OModuli_Referenze_Config_Dettagli.OTabelleParametri()"
        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0
            stb.AppendLine(" -- il DISTINCT è per avere le OTabelle (che vengono usate come nome di colonna) non ripetute per ogni OModuli_Referenze_Config_Dettagli")
            stb.AppendLine(" SELECT DISTINCT  OTabelle.tabella_cod, OTabelle.Tipo, OTabelle.Tabella_Des, OTabelle.Tabella_Cod_Des  ")
            stb.AppendLine(" , ISNULL(OTabelle_Parametri.Descrizione, '') AS Descrizione, ISNULL(OTabelle_Parametri.Sigla, '') AS Sigla, ISNULL(OTabelle_Parametri.Codice_Origine, '') AS Codice_Origine  ")
            stb.AppendLine(" -- se si aggiunge Tabella_Par_Cod poi ci possono essere più righe con la stessa descrizione di parametro, perchè ci possono essere più OModuli_Referenze_Config_Testata per specie diverse  ")
            stb.AppendLine(" , ISNULL(OTabelle_Parametri.Tabella_Par_Cod, 0) AS Tabella_Par_Cod   ")
            stb.AppendLine(" FROM OModuli_Referenze_Config_Testata  " & vbCrLf)
            stb.AppendLine(" INNER JOIN OModuli_Referenze_Config_Dettagli ON  OModuli_Referenze_Config_Dettagli.piva = OModuli_Referenze_Config_Testata.piva ")
            stb.AppendLine(" AND  OModuli_Referenze_Config_Dettagli.id_testata = OModuli_Referenze_Config_Testata.id_testata ")
            stb.AppendLine(" INNER JOIN   OTabelle ON CONVERT(varchar(50),OTabelle.Tabella_cod) = OModuli_Referenze_Config_Dettagli.Tabella_id ")
            stb.AppendLine(" AND OModuli_Referenze_Config_Testata.modulo_generazione = OTabelle.modulo_generazione ")
            stb.AppendLine(" LEFT OUTER JOIN OTabelle_Parametri ON OTabelle.Tabella_Cod= OTabelle_Parametri.Tabella_Cod AND OTabelle.Modulo_Generazione= OTabelle_Parametri.Modulo_Generazione")
            stb.AppendLine(" AND OModuli_Referenze_Config_Testata.modulo_generazione = OTabelle.modulo_generazione ")
            stb.AppendLine(" WHERE  OModuli_Referenze_Config_Testata.piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            stb.AppendLine(" ORDER BY otabelle.tabella_cod, descrizione ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Friend Function IsParametroReferenziatoDaAltri(piva As String, id_Testata As Integer, tabella_ID As String, objParametri As AgronicaCoreParametri) As List(Of String)
        Const nomeRoutine = "AgronicaCoreContabDAL.OModuli_Referenze_Config_Dettagli_R.LeggiParametriQualitativiXConfigurazione()"
        Dim risposta As List(Of String) = Nothing

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
            Dim parametro = GiasContext.OTabelle _
                .FirstOrDefault(Function(p) CStr(p.Tabella_Cod) = tabella_ID)

            Dim configurazione = GiasContext.OModuli_Referenze_Config_Dettagli _
                    .Where(Function(d) d.Piva = piva AndAlso
                        d.Id_Testata = id_Testata) _
                    .ToList()

            If Not IsNothing(configurazione) AndAlso configurazione.Any() Then
                risposta = configurazione _
                    .Where(Function(r) r.Tabella_Key_Rif.ToLower() = parametro.Tabella_Cod_Des.ToLower()) _
                    .Select(Function(r) r.Tabella_Key) _
                    .ToList()
            End If

        End Using

        Return risposta
    End Function
End Class

Public Class OModuli_Referenze_Config_Dettagli_W
    Inherits AgronicaCoreDataProvider.DataProvider

#Region "Costruttori"

    Public Sub New()
        Provider = System.Globalization.CultureInfo.InvariantCulture
        Format = "yyyyMMdd"
        ValiditaInizio = Date.ParseExact("19000101", Format, Provider)
        ValiditaFine = Date.ParseExact("21001231", Format, Provider)
    End Sub

#End Region

    Private _format As String
    Public Shadows Property Format() As String
        Get
            Return _format
        End Get
        Set
            _format = Value
        End Set
    End Property

    Private _provider As System.Globalization.CultureInfo
    Public Shadows Property Provider() As System.Globalization.CultureInfo
        Get
            Return _provider
        End Get
        Set
            _provider = Value
        End Set
    End Property

    Private _validitaInizio As Date
    Public Shadows Property ValiditaInizio() As Date
        Get
            Return _validitaInizio
        End Get
        Set
            _validitaInizio = Value
        End Set
    End Property

    Private _validitaFine As Date
    Public Shadows Property ValiditaFine() As Date
        Get
            Return _validitaFine
        End Get
        Set
            _validitaFine = Value
        End Set
    End Property

    Public Function AggiornaRecordModificati(ByVal piva As String,
            ByVal righeInserite As String,
            ByVal righeModificate As String,
            ByVal righeCancellate As String,
            ByVal tutteleRighe As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As String


        Dim Piva_SuperUser = objParametri.PivaSuperUser
        Dim esitoAggioramento As String = String.Empty
        Dim MessaggioErrore As String = String.Empty
        Dim NomeRoutine As String = "AgronicaCoreContabDAL.OModuli_Referenze_Config_Testata_W.AggiornaRecordModificati()"
        ' Controlla se ci sono periodi sovrapposti all'interno delle righe che si stanno gestendo
        Try
            Dim righeInseriteArray As JArray = JArray.Parse(righeInserite)
            Dim righeModificateArray As JArray = JArray.Parse(righeModificate)
            Dim righeCancellateArray As JArray = JArray.Parse(righeCancellate)
            Dim tutteleRigheArray As JArray = JArray.Parse(tutteleRighe)

            'Array che ti servono per la parte di scrittura
            Dim EFArrayToInsert As New List(Of OModuli_Referenze_Config_Dettagli)
            Dim EFArrayToUpdate As New List(Of OModuli_Referenze_Config_Dettagli)
            Dim EFArrayToDelete As New List(Of OModuli_Referenze_Config_Dettagli)

            Dim isValide As Boolean = VerificaConfigurazioniPerTestata(tutteleRigheArray, MessaggioErrore)
            isValide = isValide AndAlso ImpostaRigheInserire(righeInseriteArray, EFArrayToInsert, MessaggioErrore, objParametri)
            isValide = isValide AndAlso ImpostaRigheModificate(righeModificateArray, EFArrayToUpdate, MessaggioErrore, objParametri)
            isValide = isValide AndAlso ImpostaRigheCancellate(tutteleRigheArray, righeCancellateArray, EFArrayToDelete, MessaggioErrore, objParametri)

            If isValide Then
                'Parte Di scrittura
                esitoAggioramento = Scrivi(piva, EFArrayToInsert, EFArrayToUpdate, EFArrayToDelete, objParametri)
            Else
                Throw New Exception(MessaggioErrore)
            End If

        Catch ex As Exception
            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            esitoAggioramento = $"{ex.Message}<br />"
        Finally

        End Try

        Return esitoAggioramento
    End Function

    Private Function ImpostaRigheInserire(righeArray As JArray,
                                          EFArray As List(Of OModuli_Referenze_Config_Dettagli),
                                          ByRef messaggioErrore As String,
                                          objParametri As AgronicaCoreParametri) As Boolean
        Dim result As Boolean = False
        messaggioErrore = String.Empty

        For Each obj As JObject In righeArray
            messaggioErrore = VerificaRigaValida(obj, objParametri)

            If String.IsNullOrEmpty(messaggioErrore) Then
                Dim dettaglio As New OModuli_Referenze_Config_Dettagli
                ImpostaTabellaEF(obj, dettaglio, objParametri)
                dettaglio.Data_Creazione = CDate(FormatDateTime(Now, 2).ToString() & " 00:00:00")
                dettaglio.Username_Creazione = objParametri.UsernameOperazione
                dettaglio.Data_Modifica = CDate(FormatDateTime(Now, 2).ToString() & " 00:00:00")
                dettaglio.Username_Modifica = objParametri.UsernameOperazione
                dettaglio.inviato = 0
                EFArray.Add(dettaglio)
            Else
                Exit For
            End If
        Next
        result = String.IsNullOrEmpty(messaggioErrore)

        Return result

    End Function

    Private Function ImpostaRigheModificate(righeArray As JArray,
                                          EFArray As List(Of OModuli_Referenze_Config_Dettagli),
                                          ByRef messaggioErrore As String,
                                          objParametri As AgronicaCoreParametri) As Boolean
        Dim result As Boolean = False
        messaggioErrore = String.Empty

        For Each obj As JObject In righeArray
            messaggioErrore = VerificaRigaValida(obj, objParametri)

            If String.IsNullOrEmpty(messaggioErrore) Then
                Dim dettaglio As New OModuli_Referenze_Config_Dettagli
                ImpostaTabellaEF(obj, dettaglio, objParametri)
                If Not String.IsNullOrEmpty(obj("Data_Creazione")) Then
                    dettaglio.Data_Creazione = Date.ParseExact(obj("Data_Creazione").ToString, Format, Provider)
                End If
                dettaglio.Username_Creazione = obj("Username_Creazione").ToString
                dettaglio.Data_Modifica = CDate(FormatDateTime(Now, 2).ToString() & " 00:00:00")
                dettaglio.Username_Modifica = objParametri.UsernameOperazione
                dettaglio.inviato = obj("Inviato")
                EFArray.Add(dettaglio)
            Else
                Exit For
            End If
        Next
        result = String.IsNullOrEmpty(messaggioErrore)

        Return result

    End Function

    Private Function ImpostaRigheCancellate(tutteleRigheArray As JArray,
                                          righeArray As JArray,
                                          EFArray As List(Of OModuli_Referenze_Config_Dettagli),
                                          ByRef messaggioErrore As String,
                                          objParametri As AgronicaCoreParametri) As Boolean
        Dim result As Boolean = False
        messaggioErrore = String.Empty

        For Each obj As JObject In righeArray
            Dim dettaglio As New OModuli_Referenze_Config_Dettagli With {
                    .Piva = obj("Piva"),
                    .Id_Testata = obj("Id_Testata"),
                    .Tipo = obj("Tipo"),
                    .Tabella_ID = obj("Tabella_ID"),
                    .Tabella_Key = obj("Tabella_Key"),
                    .Tabella_Key_Rif = obj("Tabella_Key_Rif")
                }

            messaggioErrore = VerificaRigaCancellazioneValida(dettaglio, objParametri)

            If String.IsNullOrEmpty(messaggioErrore) Then
                EFArray.Add(dettaglio)
            Else
                Exit For
            End If
        Next
        result = String.IsNullOrEmpty(messaggioErrore)

        Return result

    End Function

    Private Sub ImpostaTabellaEF(obj As JObject, ByRef dettaglio As OModuli_Referenze_Config_Dettagli, objParametri As AgronicaCoreParametri)
        Dim inizio As DateTime = DateTime.ParseExact(obj("Validita_Inizio").ToString, "yyyyMMdd", Nothing)
        Dim fine As DateTime = DateTime.ParseExact(obj("Validita_Fine").ToString, "yyyyMMdd", Nothing)

        dettaglio.Piva = obj("Piva")
        dettaglio.Id_Testata = obj("Id_Testata")
        dettaglio.Tipo = obj("Tipo")
        dettaglio.Tabella_ID = obj("Tabella_ID")
        dettaglio.Tabella_Key = obj("Tabella_Key")
        dettaglio.Configurazione_Des = obj("Configurazione_Des")
        dettaglio.ChkReferenza = obj("ChkReferenza")
        dettaglio.Ordine = obj("Ordine")
        dettaglio.ChkOmni_Invisibili = obj("ChkOmni_Invisibili")
        dettaglio.ChkEtichetta = obj("ChkEtichetta")
        dettaglio.ChkEdit = obj("ChkEdit")
        dettaglio.ChkObbligatorio = obj("ChkObbligatorio")
        dettaglio.Tabella_Key_Rif = obj("Tabella_Key_Rif")

        If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
            dettaglio.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio").ToString, Format, Provider)
        End If
        If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
            dettaglio.Validita_Fine = Date.ParseExact(obj("Validita_Fine").ToString, Format, Provider)
        End If
    End Sub

    Private Function VerificaConfigurazioniPerTestata(tutteleRigheArray As JArray, ByRef MessaggioErrore As String) As Boolean
        Dim gruppoPerTestata = tutteleRigheArray _
                .GroupBy(Function(key) key("Id_Testata").ToString(),
                         Function(gr) gr,
                         Function(k, g) New With
                         {
                            .IdTestata = k,
                            .Id_Testata_Des = g.Where(Function(x) x("Id_Testata").ToString() = k) _
                                .Select(Function(x) x("Id_Testata_Des").ToString()) _
                                .FirstOrDefault(),
                            .Parametri = g.Select(Function(p) New With
                             {
                                .Tabella_Key = p("Tabella_Key").ToString(),
                                .Ordine = p("Ordine").ToString(),
                                .Tabella_Key_Rif = p("Tabella_Key_Rif").ToString()
                             }) _
                         .ToList()
                 }) _
                 .ToList()

        For Each testata In gruppoPerTestata
            Dim messaggio As String = String.Empty

            Dim parametriDoppi = testata.Parametri _
                .GroupBy(Function(key) key.Tabella_Key, Function(gr) gr,
                         Function(k, g) New With
                         {
                            .Tabella_Key = k,
                            .Count = g.Count()
                         }) _
                         .Where(Function(x) x.Count > 1) _
                         .Select(Function(x) x.Tabella_Key) _
                         .ToList()

            If parametriDoppi.Any() Then
                If Not String.IsNullOrEmpty(messaggio) Then
                    messaggio += "<br />"
                End If
                messaggio += $"I seguenti parametri qualitativi sono stati utilizzati più volte: {String.Join("<br />", parametriDoppi)}"
            End If

            Dim ordiniDoppi = testata.Parametri _
                .GroupBy(Function(key) key.Ordine, Function(gr) gr,
                         Function(k, g) New With
                         {
                            .Ordine = k,
                            .Count = g.Count()
                         }) _
                         .Where(Function(x) x.Count > 1) _
                         .ToList()

            If ordiniDoppi.Any() Then
                If Not String.IsNullOrEmpty(messaggio) Then
                    messaggio += "<br />"
                End If
                messaggio += $"L'ordine di alcuni parametri qualitativi sono uguali"
            End If

            If Not String.IsNullOrEmpty(messaggio) Then
                If Not String.IsNullOrEmpty(MessaggioErrore) Then
                    MessaggioErrore += "<br />"
                End If
                MessaggioErrore = $"Per la testata '{testata.Id_Testata_Des}' sono presenti le seguenti segnalazioni:<br />{messaggio}"
            End If
        Next

        Return String.IsNullOrEmpty(MessaggioErrore)
    End Function

    Private Function VerificaRigaValida(obj As JObject, ByRef objParametri As AgronicaCoreParametri) As String
        Dim result As String = String.Empty
        ' Verifica di non sovrapposizione del parametro di riferimento con se stesso
        If obj("Tabella_Key").ToString().Equals(obj("Tabella_Key_Rif").ToString(), StringComparison.OrdinalIgnoreCase) Then
            result = "Parametro qualitativo referenziato su se stesso"
        End If
        Return result
    End Function

    Private Function VerificaRigaCancellazioneValida(dettaglio As OModuli_Referenze_Config_Dettagli,
                                                     ByRef objParametri As AgronicaCoreParametri) As String
        Dim messaggio As String = String.Empty
        ' Verificare se il parametro è referenziato da altri parametri

        Dim leggiAnagrafeLog As New OModuli_Referenze_Config_Dettagli_R
        Dim parametroReferenziato As List(Of String) = leggiAnagrafeLog.IsParametroReferenziatoDaAltri(dettaglio.Piva, dettaglio.Id_Testata, dettaglio.Tabella_ID, objParametri)
        If Not IsNothing(parametroReferenziato) AndAlso parametroReferenziato.Any() Then
            messaggio += $"Il parametro qualitativo '{dettaglio.Tabella_Key}' non poò essere cancellato perchè referenziato dai parametri: {String.Join("<br />", parametroReferenziato)}"
        Else
            ' Verificare se il parametro è referenziato da Materie_Prime_Campionatura
            Dim lettura As New OTabelle_Parametri_R
            Dim conReferenza = lettura.VerificaParametroQualitativoUtilizzato(dettaglio.Tabella_ID, objParametri)

            If Not IsNothing(conReferenza) AndAlso conReferenza.Rows.Count > 0 Then
                messaggio += $"Il parametro qualitativo '{dettaglio.Tabella_Key}' non può essere cancellato perchè già utilizzata dal campionamento delle materie prime"
            End If
        End If

        Return messaggio
    End Function

    '#########################################################################
    Private Function Scrivi(ByVal piva As String,
                               ByVal EFArrayToInsert As List(Of OModuli_Referenze_Config_Dettagli),
                               ByVal EFArrayToUpdate As List(Of OModuli_Referenze_Config_Dettagli),
                               ByVal EFArrayToDelete As List(Of OModuli_Referenze_Config_Dettagli),
                               ByRef objParametri As AgronicaCoreParametri
                               ) As String

        Const nomeRoutine = "AgronicaCoreContabDAL.OModuli_Referenze_Config_Testata_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim gefutils As New Gias_EF_Utility
        Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Dim sequenza_tabelle As New AgronicaCoreDataProvider.Agro_Sequenze

        Try
            'Prova di scrittura

            'Scrittura in Entity Framework 
            Using GiasContext As New Gias_DeveloperServer_Entities(efConnString)

                Dim transaction As DbContextTransaction = Nothing
                ' Contiene anche i dettagli
                Try
                    transaction = GiasContext.Database.BeginTransaction()

                    For Each listProdotti As OModuli_Referenze_Config_Dettagli In EFArrayToInsert
                        GiasContext.OModuli_Referenze_Config_Dettagli.Add(listProdotti)
                    Next

                    For Each listProdotti As OModuli_Referenze_Config_Dettagli In EFArrayToUpdate
                        GiasContext.OModuli_Referenze_Config_Dettagli.Attach(listProdotti)
                        GiasContext.Entry(listProdotti).State = EntityState.Modified
                    Next

                    For Each listProdotti As OModuli_Referenze_Config_Dettagli In EFArrayToDelete
                        GiasContext.OModuli_Referenze_Config_Dettagli.Attach(listProdotti)
                        GiasContext.OModuli_Referenze_Config_Dettagli.Remove(listProdotti)
                    Next

                    GiasContext.SaveChanges()
                    'Gias Context.SaveChanges() è come se fosse una transazione se c'è un errore,
                    'nelle righe inserite,cancellate o modificate viene annullata tutta la scrittura
                    transaction.Commit()
                Catch ex As Exception
                    If IsNothing(transaction) Then
                        transaction.Rollback()
                    End If
                End Try
            End Using

            '---------------------------------------------

            '--------------------------------------------------------------------------
            'xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return messaggioErrore

    End Function

End Class