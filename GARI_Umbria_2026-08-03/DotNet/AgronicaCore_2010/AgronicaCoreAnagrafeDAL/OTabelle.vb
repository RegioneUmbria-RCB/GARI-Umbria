Imports AgronicaCoreDataProvider
Imports System.Text
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreEntityFramework_POCO
Imports Newtonsoft.Json.Linq
Imports System.Data.Entity.Core.Metadata.Edm
Imports AgronicaCoreEntityFramework
Imports System.Data.Entity
Imports AgronicaCoreDTOStd.Identity
Imports AgronicaCoreModelsSTD.metaschema
Imports AgronicaCoreModelsSTD.Zoo

Public Class OTabelle_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(
        ByVal piva As String,
        ByVal tabellaCod As Integer,
        ByVal moduloGenerazione As Integer,
        ByVal tabellaCodDes As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.OTabelle_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine("SELECT * FROM OTabelle WHERE 1=1 ")

            If piva <> "" Then
                strSql.AppendLine(" AND piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            If tabellaCod <> 0 Then
                strSql.AppendLine(" AND tabella_cod = " & Agro_SQL_SaveNum(tabellaCod) & " ")
            End If

            If moduloGenerazione <> 0 Then
                strSql.AppendLine(" AND modulo_generazione = " & Agro_SQL_SaveNum(moduloGenerazione) & " ")
            End If

            If tabellaCodDes <> "" Then
                strSql.AppendLine(" AND tabella_cod_des = '" & Agro_SQL_SaveText(tabellaCodDes) & "' ")
            End If

            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dt

    End Function

    Public Function LeggiXTabella_Cod_Des(ByVal Tabella_Cod_Des As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.OTabelle_R.LeggiXTabella_Cod_Des()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine("SELECT * FROM OTabelle WHERE Tabella_Cod_Des = '" & Agro_SQL_SaveText(Tabella_Cod_Des) & "' ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt
    End Function

    Public Function LeggiParametriQualitativi(
        ByVal piva As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
        Optional ByVal xFiltroAggiuntivo As String = Nothing,
        Optional toLower As Boolean = False
        ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.OTabelle_R.LeggiParametriQualitativi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine("SELECT Piva,")
            strSql.AppendLine(" Tabella_Cod,")
            strSql.AppendLine(" Modulo_Generazione,")
            strSql.AppendLine(" Tipo,")
            strSql.AppendLine(" Tabella_Des,")

            If toLower Then
                strSql.AppendLine(" LOWER(Tabella_Cod_Des) Tabella_Cod_Des,")
            Else
                strSql.AppendLine(" Tabella_Cod_Des,")
            End If

            strSql.AppendLine(" ChkEsclusione,")
            strSql.AppendLine(" inviato,")
            strSql.AppendLine(" datainvio,")
            strSql.AppendLine(" Data_Creazione,")
            strSql.AppendLine(" Data_Modifica,")
            strSql.AppendLine(" Username_Creazione,")
            strSql.AppendLine(" Username_Modifica,")
            strSql.AppendLine(" Validita_Inizio,")
            strSql.AppendLine(" Validita_Fine,")
            strSql.AppendLine(" Tipo_Generazione_Link,")
            strSql.AppendLine(" SottoTipo_Generazione_Link,")
            strSql.AppendLine(" Valore_Minimo,")
            strSql.AppendLine(" Valore_Maximo,")
            strSql.AppendLine(" NumDecimali_Maximo")
            strSql.AppendLine("FROM OTabelle")
            strSql.AppendLine("WHERE piva IN ('AAAAAAAAAAA', '" & Agro_SQL_SaveText(piva) & "')")

            If Not IsNothing(xFiltroAggiuntivo) Then
                strSql.AppendLine($"AND {xFiltroAggiuntivo}")
            End If

            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dt
    End Function

    Public Function VerificaParametriUtilizzati(tabellaCod As Integer, moduloGenerazione As Short,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.OTabelle_R.VerificaUtilizzoParametro()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine("SELECT DISTINCT t.Piva,")
            strSql.AppendLine(" t.Tabella_Cod,")
            strSql.AppendLine(" t.Modulo_Generazione,")
            strSql.AppendLine(" t.Tipo,")
            strSql.AppendLine(" t.Tabella_Des,")
            strSql.AppendLine(" t.Tabella_Cod_Des,")
            strSql.AppendLine(" CASE WHEN tp.Tabella_Cod Is NULL THEN 0 ELSE 1 END Refernza_Otabelle_Parametri,")
            strSql.AppendLine(" CASE WHEN mrcd.Tabella_ID Is NULL THEN 0 ELSE 1 END Refernza_OModuli_Referenze_Config_Dettagli")
            strSql.AppendLine("FROM OTabelle t")
            strSql.AppendLine("LEFT JOIN OTabelle_Parametri tp")
            strSql.AppendLine("ON t.Tabella_Cod = tp.Tabella_Cod")
            strSql.AppendLine("AND t.Modulo_Generazione = tp.Modulo_Generazione")
            strSql.AppendLine("LEFT JOIN OModuli_Referenze_Config_Dettagli mrcd")
            strSql.AppendLine("ON CAST(t.Tabella_Cod AS VARCHAR(255)) = mrcd.Tabella_ID")
            strSql.AppendLine($"WHERE t.Tabella_Cod = {tabellaCod}")
            strSql.AppendLine($"AND t.Modulo_Generazione = {moduloGenerazione}")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt
    End Function

    Public Function LeggiXDescrizione(ByVal Tabella_Cod As String, ByVal Tabella_Par_Cod As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.OTabelle_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine("SELECT * FROM OTabelle_Parametri WHERE Tabella_Cod = '" & Agro_SQL_SaveText(Tabella_Cod) & "' ")
            strSql.AppendLine("                       AND Tabella_Par_Cod = '" & Agro_SQL_SaveText(Tabella_Par_Cod) & "' ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt
    End Function

    Public Function LeggiParametri(ByVal Piva As String,
                                   ByVal Tabella_Cod As Integer,
                                   ByVal Modulo_GenerazioneStrIn As String,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.OTabelle_R.LeggiParametri()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine("SELECT * ")
            strSql.AppendLine("FROM OTabelle_Parametri ")
            strSql.AppendLine("WHERE (Piva = 'AAAAAAAAAAA' OR Piva = '" & Agro_SQL_SaveText(Piva) & "') ")

            If Not String.IsNullOrEmpty(Modulo_GenerazioneStrIn) Then
                strSql.AppendLine("And Modulo_Generazione IN (" & Agro_SQL_Save_Clausola_IN(Modulo_GenerazioneStrIn) & ")")
            End If


            If Tabella_Cod <> 0 Then
                strSql.AppendLine("AND Tabella_Cod = " & Agro_SQL_SaveNum(Tabella_Cod) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] :  " & messaggioErrore)
        End Try

        Return dt
    End Function

    Public Function LeggiJoinParametri(ByRef objParametri As AgronicaCoreParametri, ByVal xFiltroAggiuntivo As String, ByVal xOrderBy As String) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.OTabelle_R.LeggiJoinParametri()"

        Dim dt As New DataTable
        Dim strSql As New StringBuilder

        Try
            strSql.AppendLine("SELECT * ")
            strSql.AppendLine("FROM OTabelle ")
            strSql.AppendLine("LEFT JOIN OTabelle_Parametri ")
            strSql.AppendLine("ON OTabelle_Parametri.Tabella_Cod = OTabelle.Tabella_Cod ")
            strSql.AppendLine("WHERE 1 = 1 ")

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Dim messaggioErrore As String = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

End Class


Public Class OTabelle_W
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

    Public Function ScriviParametro(ByVal Piva As String,
                                    ByVal Tabella_Cod As Integer,
                                    ByVal Tabella_Par_Cod As Integer,
                                    ByVal Modulo_Generazione As Integer,
                                    ByVal Descrizione As String,
                                    ByVal Sigla As String,
                                    ByVal Codice_Origine As String,
                                    ByVal OFiltro_Veg_Cod As String,
                                    ByVal OFiltro_Cul_Cod As String,
                                    ByVal Mat_Cod_Generazione_Link As Integer,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.OTabelle_W.ScriviParametro()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine("INSERT INTO OTabelle_Parametri ( ")
            strSql.AppendLine(" Piva ")
            strSql.AppendLine(" , Tabella_Cod ")
            strSql.AppendLine(" , Tabella_Par_Cod ")
            strSql.AppendLine(" , Modulo_Generazione ")
            strSql.AppendLine(" , Descrizione ")
            strSql.AppendLine(" , Sigla ")
            strSql.AppendLine(" , Codice_Origine ")
            strSql.AppendLine(" , ChkInvisibile ")
            strSql.AppendLine(" , Valore_Min ")
            strSql.AppendLine(" , Valore_Max ")
            strSql.AppendLine(" , OFiltro_Veg_Cod ")
            strSql.AppendLine(" , OFiltro_Cul_Cod ")
            strSql.AppendLine(" , OFiltro_Colore ")
            strSql.AppendLine(" , OFiltro_Categoria ")
            strSql.AppendLine(" , OFiltro_Classificazione ")
            strSql.AppendLine(" , OFiltro_Dicitura ")
            strSql.AppendLine(" , OFiltro_Caratteristica ")
            strSql.AppendLine(" , OFiltro_Deno ")
            strSql.AppendLine(" , OFiltro_Finalita ")
            strSql.AppendLine(" , OFiltro_Regolamento ")
            strSql.AppendLine(" , inviato ")
            strSql.AppendLine(" , datainvio ")
            strSql.AppendLine(" , Data_Creazione ")
            strSql.AppendLine(" , Data_Modifica ")
            strSql.AppendLine(" , Username_Creazione ")
            strSql.AppendLine(" , Username_Modifica ")
            strSql.AppendLine(" , Validita_Inizio ")
            strSql.AppendLine(" , Validita_Fine ")
            strSql.AppendLine(" , Codice_Generazione_Link ")
            strSql.AppendLine(" , Mat_Cod_Generazione_Link ")
            strSql.AppendLine(") VALUES ( ")
            strSql.AppendLine(" '" & Agro_SQL_SaveText(Piva) & "' ")
            strSql.AppendLine(" , " & Tabella_Cod.ToString & " ")
            strSql.AppendLine(" , " & Tabella_Par_Cod.ToString & " ")
            strSql.AppendLine(" , " & Modulo_Generazione.ToString & " ")
            strSql.AppendLine(" , '" & Agro_SQL_SaveText(Descrizione) & "' ")
            strSql.AppendLine(" , '" & Agro_SQL_SaveText(Sigla) & "' ")
            strSql.AppendLine(" , '" & Agro_SQL_SaveText(Codice_Origine) & "' ")
            strSql.AppendLine(" , 0 --ChkInvisibile ")
            strSql.AppendLine(" , -2000000000 --Valore_Min ")
            strSql.AppendLine(" , 2000000000 --Valore_Max ")
            strSql.AppendLine(" , '" & Agro_SQL_SaveText(OFiltro_Veg_Cod) & "' ")
            strSql.AppendLine(" , '" & Agro_SQL_SaveText(OFiltro_Cul_Cod) & "'")
            strSql.AppendLine(" , '' --OFiltro_Colore ")
            strSql.AppendLine(" , '' --OFiltro_Categoria ")
            strSql.AppendLine(" , '' --OFiltro_Classificazione ")
            strSql.AppendLine(" , '' --OFiltro_Dicitura ")
            strSql.AppendLine(" , '' --OFiltro_Caratteristica ")
            strSql.AppendLine(" , '' --OFiltro_Deno ")
            strSql.AppendLine(" , '' --OFiltro_Finalita ")
            strSql.AppendLine(" , '' -- OFiltro_Regolamento ")
            strSql.AppendLine(" , 0 --inviato ")
            strSql.AppendLine(" , NULL --datainvio ")
            strSql.AppendLine(" , " & Agro_SQL_SaveDateTime(Date.Now) & " --Data_Creazione ")
            strSql.AppendLine(" , " & Agro_SQL_SaveDateTime(Date.Now) & " --Data_Modifica ")
            strSql.AppendLine(" , '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' --Username_Creazione ")
            strSql.AppendLine(" , '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' --Username_Modifica ")
            strSql.AppendLine(" , " & Agro_SQL_SaveDate(AGRODATAINIZIO) & " --Validita_Inizio ")
            strSql.AppendLine(" , " & Agro_SQL_SaveDate(AGRODATAFINE) & " --Validita_Fine ")
            strSql.AppendLine(" , 0 --Codice_Generazione_Link ")
            strSql.AppendLine(" , " & Agro_SQL_SaveNum(Mat_Cod_Generazione_Link) & " --Mat_Cod_Generazione_Link ")
            strSql.AppendLine(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp
    End Function

    Public Function ModificaParametro(ByVal Piva As String,
                                    ByVal Tabella_Cod As Integer,
                                    ByVal Tabella_Par_Cod As Integer,
                                    ByVal Modulo_Generazione As Integer,
                                    ByVal Descrizione As String,
                                    ByVal Sigla As String,
                                    ByVal Codice_Origine As String,
                                    ByVal OFiltro_Veg_Cod As String,
                                    ByVal OFiltro_Cul_Cod As String,
                                    ByVal Mat_Cod_Generazione_Link As Integer,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.OTabelle_W.ModificaParametro()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE OTabelle_Parametri SET ")
            strSql.AppendLine("   Piva              = '" & Agro_SQL_SaveText(Piva) & "'  ")
            strSql.AppendLine("   ,OFiltro_Veg_Cod          =  '" & Agro_SQL_SaveText(OFiltro_Veg_Cod) & "'  ")
            strSql.AppendLine("   ,OFiltro_Cul_Cod           = '" & Agro_SQL_SaveText(OFiltro_Cul_Cod) & "'  ")

            strSql.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Date.Now))
            strSql.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            If Descrizione <> "" Then
                strSql.AppendLine("   ,Descrizione = '" & Agro_SQL_SaveText(Descrizione) & "'")
            End If

            If Sigla <> "" Then
                strSql.AppendLine("   ,Sigla = '" & Agro_SQL_SaveText(Sigla) & "'")
            End If

            If Codice_Origine <> "" Then
                strSql.AppendLine("   ,Codice_Origine = '" & Agro_SQL_SaveText(Codice_Origine) & "'")
            End If

            strSql.AppendLine(" WHERE Piva      =  '" & Agro_SQL_SaveText(Piva) & "'  ")
            strSql.AppendLine(" AND   Tabella_Cod     =  " & Agro_SQL_SaveNum(Tabella_Cod) & "  ")
            strSql.AppendLine(" AND   Tabella_Par_Cod = " & Agro_SQL_SaveNum(Tabella_Par_Cod) & "  ")
            strSql.AppendLine(" AND   Modulo_Generazione = " & Agro_SQL_SaveNum(Modulo_Generazione) & "  ")
            strSql.AppendLine(" AND   Mat_Cod_Generazione_Link = " & Agro_SQL_SaveNum(Mat_Cod_Generazione_Link) & "  ")

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function CancellaParametro(ByVal Piva As String,
                                        ByVal Tabella_Cod As Integer,
                                        ByVal Tabella_Par_Cod As Integer,
                                        ByVal Modulo_Generazione As Integer,
                                        ByVal Mat_Cod_Generazione_Link As Integer,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.OTabelle_W.CancellaParametro()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            strSql.AppendLine(" DELETE ")
            strSql.AppendLine(" FROM  OTabelle_Parametri ")
            strSql.AppendLine(" WHERE Piva= '" & Agro_SQL_SaveText(Piva) & "' ")

            If Tabella_Cod <> 0 Then
                strSql.AppendLine(" AND Tabella_Cod = " & Agro_SQL_SaveNum(Tabella_Cod) & "   ")
            End If

            If Tabella_Par_Cod <> 0 Then
                strSql.AppendLine(" AND Tabella_Par_Cod = " & Agro_SQL_SaveNum(Tabella_Par_Cod) & "   ")
            End If

            If Modulo_Generazione <> 0 Then
                strSql.AppendLine(" AND Modulo_Generazione = " & Agro_SQL_SaveNum(Modulo_Generazione) & "   ")
            End If

            If Mat_Cod_Generazione_Link <> 0 Then
                strSql.AppendLine(" AND Mat_Cod_Generazione_Link = " & Agro_SQL_SaveNum(Mat_Cod_Generazione_Link) & "   ")
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp
    End Function

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
        Dim NomeRoutine As String = "AgronicaCoreContabDAL.OTabelle_W.AggiornaRecordModificati()"
        ' Controlla se ci sono periodi sovrapposti all'interno delle righe che si stanno gestendo
        Try

            Dim righeInseriteArray As JArray = JArray.Parse(righeInserite)
            Dim righeModificateArray As JArray = JArray.Parse(righeModificate)
            Dim righeCancellateArray As JArray = JArray.Parse(righeCancellate)
            Dim tutteleRigheArray As JArray = JArray.Parse(tutteleRighe)

            'Array che ti servono per la parte di scrittura
            Dim EFArrayToInsert As New List(Of OTabelle)
            Dim EFArrayToUpdate As New List(Of OTabelle)
            Dim EFArrayToDelete As New List(Of OTabelle)

            Dim isValide As Boolean = ImpostaRigheInserire("AAAAAAAAAAA", righeInseriteArray, EFArrayToInsert, MessaggioErrore, objParametri)
            isValide = isValide AndAlso ImpostaRigheModificate(righeModificateArray, EFArrayToUpdate, MessaggioErrore, objParametri)
            isValide = isValide AndAlso ImpostaRigheCancellate("AAAAAAAAAAA", righeCancellateArray, EFArrayToDelete, MessaggioErrore, objParametri)

            If isValide Then
                'Parte Di scrittura
                esitoAggioramento = Scrivi(piva, EFArrayToInsert, EFArrayToUpdate, EFArrayToDelete, objParametri)
            Else
                Throw New Exception(MessaggioErrore)
            End If

        Catch ex As Exception
            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            esitoAggioramento = ex.Message
        Finally

        End Try

        Return esitoAggioramento
    End Function

    Private Function ImpostaRigheInserire(piva As String,
                                          righeArray As JArray,
                                          EFArray As List(Of OTabelle),
                                          ByRef messaggioErrore As String,
                                          objParametri As AgronicaCoreParametri) As Boolean
        Dim result As Boolean = False
        messaggioErrore = String.Empty

        For Each obj As JObject In righeArray
            messaggioErrore = VerificaRigaValida(piva, obj, objParametri)

            If String.IsNullOrEmpty(messaggioErrore) Then
                Dim tabella As New OTabelle
                ImpostaTabellaEF(piva, obj, tabella, objParametri)
                tabella.Data_Creazione = CDate(FormatDateTime(Now, 2).ToString() & " 00:00:00")
                tabella.Username_Creazione = objParametri.UsernameOperazione
                tabella.Data_Modifica = CDate(FormatDateTime(Now, 2).ToString() & " 00:00:00")
                tabella.Username_Modifica = objParametri.UsernameOperazione
                tabella.inviato = 0
                EFArray.Add(tabella)
            Else
                Exit For
            End If
        Next
        result = String.IsNullOrEmpty(messaggioErrore)

        Return result

    End Function

    Private Function ImpostaRigheModificate(righeArray As JArray,
                                          EFArray As List(Of OTabelle),
                                          ByRef messaggioErrore As String,
                                          objParametri As AgronicaCoreParametri) As Boolean
        Dim result As Boolean = False
        messaggioErrore = String.Empty

        For Each obj As JObject In righeArray
            messaggioErrore = VerificaRigaValida(obj, objParametri)

            If String.IsNullOrEmpty(messaggioErrore) Then
                Dim tabella As New OTabelle
                ImpostaTabellaEF(obj, tabella, objParametri)
                tabella.Tabella_Cod = obj("Tabella_Cod")
                If Not String.IsNullOrEmpty(obj("Data_Creazione")) Then
                    tabella.Data_Creazione = Date.ParseExact(obj("Data_Creazione").ToString, Format, Provider)
                End If
                tabella.Username_Creazione = obj("Username_Creazione").ToString
                tabella.Data_Modifica = CDate(FormatDateTime(Now, 2).ToString() & " 00:00:00")
                tabella.Username_Modifica = objParametri.UsernameOperazione
                tabella.inviato = obj("inviato")
                EFArray.Add(tabella)
            Else
                Exit For
            End If
        Next
        result = String.IsNullOrEmpty(messaggioErrore)

        Return result

    End Function

    Private Function ImpostaRigheCancellate(piva As String,
                                          righeArray As JArray,
                                          EFArray As List(Of OTabelle),
                                          ByRef messaggioErrore As String,
                                          objParametri As AgronicaCoreParametri) As Boolean
        Dim result As Boolean = False
        messaggioErrore = String.Empty

        For Each obj As JObject In righeArray
            Dim tabella As New OTabelle With {
                    .Piva = piva,
                    .Tabella_Cod = obj("Tabella_Cod"),
                    .Modulo_Generazione = obj("Modulo_Cod"),
                    .Tabella_Cod_Des = obj("Tabella_Cod_Des")
                }
            messaggioErrore = VerificaRigaCancellazioneValida(tabella, objParametri)

            If String.IsNullOrEmpty(messaggioErrore) Then
                EFArray.Add(tabella)
            Else
                Exit For
            End If
        Next
        result = String.IsNullOrEmpty(messaggioErrore)

        Return result

    End Function

    Private Sub ImpostaTabellaEF(obj As JObject, ByRef tabella As OTabelle, objParametri As AgronicaCoreParametri)
        ImpostaTabellaEF(obj("Piva"), obj, tabella, objParametri)
    End Sub

    Private Sub ImpostaTabellaEF(piva As String, obj As JObject, ByRef tabella As OTabelle, objParametri As AgronicaCoreParametri)
        Dim inizio As DateTime = DateTime.ParseExact(obj("Validita_Inizio").ToString, "yyyyMMdd", Nothing)
        Dim fine As DateTime = DateTime.ParseExact(obj("Validita_Fine").ToString, "yyyyMMdd", Nothing)

        tabella.Piva = piva
        tabella.Tabella_Cod = 0
        tabella.Modulo_Generazione = obj("Modulo_Cod")
        tabella.Tipo = obj("Tipo_Cod")
        tabella.Tabella_Des = obj("Tabella_Des")
        tabella.Tabella_Cod_Des = obj("Tabella_Cod_Des")
        tabella.ChkEsclusione = obj("ChkEsclusione")
        tabella.Tipo_Generazione_Link = obj("Tipo_Generazione_Link")
        tabella.SottoTipo_Generazione_Link = obj("SottoTipo_Generazione_Link")
        tabella.Valore_Minimo = obj("Valore_Minimo")
        tabella.Valore_Maximo = obj("Valore_Maximo")
        tabella.NumDecimali_Maximo = obj("NumDecimali_Maximo")

        If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
            tabella.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio").ToString, Format, Provider)
        End If
        If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
            tabella.Validita_Fine = Date.ParseExact(obj("Validita_Fine").ToString, Format, Provider)
        End If
    End Sub

    Private Function VerificaRigaValida(obj As JObject, ByRef objParametri As AgronicaCoreParametri) As String
        Dim messaggio As String = String.Empty
        IsRangeMaxMinValido(obj("Tipo_Cod"), obj("Valore_Minimo"), obj("Valore_Maximo"), obj("NumDecimali_Maximo"), messaggio)

        If Not String.IsNullOrEmpty(messaggio) Then
            messaggio = $"Per il parametro '{obj("Tabella_Cod_Des")}' son state riscontrate le seguenti segnalazioni:<br />{messaggio}"
        End If

        Return messaggio
    End Function

    Private Function VerificaRigaValida(piva As String, obj As JObject, ByRef objParametri As AgronicaCoreParametri) As String
        Dim messaggio As String = String.Empty
        IsUnivoco(piva, obj("Tabella_Cod"), obj("Modulo_Cod"), obj("Tabella_Cod_Des"), objParametri, messaggio)
        IsRangeMaxMinValido(obj("Tipo_Cod"), obj("Valore_Minimo"), obj("Valore_Maximo"), obj("NumDecimali_Maximo"), messaggio)

        If Not String.IsNullOrEmpty(messaggio) Then
            messaggio = $"Per il parametro '{obj("Tabella_Cod_Des")}' son state riscontrate le seguenti segnalazioni:<br />{messaggio}"
        End If

        Return messaggio
    End Function

    Private Function VerificaRigaCancellazioneValida(tabella As OTabelle, ByRef objParametri As AgronicaCoreParametri) As String
        Dim result As String = String.Empty
        Dim leggiTabelle As New OTabelle_R
        Dim presenza As DataTable = leggiTabelle.VerificaParametriUtilizzati(tabella.Tabella_Cod, tabella.Modulo_Generazione, objParametri)

        If Not IsNothing(presenza) AndAlso presenza.Rows.Count > 0 AndAlso
            (Not presenza.Rows(0)("Refernza_Otabelle_Parametri") = 0) AndAlso
            (Not presenza.Rows(0)("Refernza_OModuli_Referenze_Config_Dettagli") = 0) Then
            result = $"Non è possibile cancellare il parametro '{tabella.Tabella_Cod_Des}' perchè referenziato su 'Parametri qualitativi per referenza', oppure su 'Elenco valori parametri qualitativi'"
        End If
        Return result
    End Function

    Private Function IsUnivoco(piva As String, tabellaCod As Integer, moduloCod As Integer, tabellaDes As String,
                               ByRef objParametri As AgronicaCoreParametri, ByRef messaggio As String) As Boolean
        Dim result As Boolean = False
        Dim leggiTabelle As New OTabelle_R
        Dim tabelle As DataTable = leggiTabelle.Leggi(piva, tabellaCod, moduloCod, tabellaDes, objParametri)
        If Not IsNothing(tabelle) AndAlso tabelle.Rows.Count > 0 Then
            If Not String.IsNullOrEmpty(messaggio) Then
                messaggio += "<br />"
            End If
            messaggio += $"Il parametro qualitativo '{tabellaDes}' risulta essere già presente"
        Else
            result = True
        End If
        Return result
    End Function

    Private Function Scrivi(ByVal piva As String,
                           ByVal EFArrayToInsert As List(Of OTabelle),
                           ByVal EFArrayToUpdate As List(Of OTabelle),
                           ByVal EFArrayToDelete As List(Of OTabelle),
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
                    For Each listProdotti As OTabelle In EFArrayToInsert
                        Dim tabellaCod As Integer = 0

                        Do
                            tabellaCod = sequenza_tabelle.NuovoId_Tabella_EF(GiasContext,
                                                                "oTabelle",
                                                                0,
                                                                AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode,
                                                                objParametri)
                        Loop While (tabellaCod < AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode) AndAlso
                            (GiasContext.OTabelle.Any(Function(x) x.Tabella_Cod = tabellaCod))

                        listProdotti.Tabella_Cod = tabellaCod
                        GiasContext.OTabelle.Add(listProdotti)
                    Next

                    For Each listProdotti As OTabelle In EFArrayToUpdate
                        GiasContext.OTabelle.Attach(listProdotti)
                        GiasContext.Entry(listProdotti).State = EntityState.Modified
                    Next

                    For Each listProdotti As OTabelle In EFArrayToDelete
                        GiasContext.OTabelle.Attach(listProdotti)
                        GiasContext.OTabelle.Remove(listProdotti)
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

    Private Function IsRangeMaxMinValido(tipoCod As Short, valoreMinimo As String, valoreMaximo As String,
                                         numDecimaliMaximo As Short?, ByRef messaggio As String) As Boolean
        Dim result As Boolean = True
        Select Case tipoCod
            Case 3 'tipo dato numerico => verifica valòri min/max
                If Not String.IsNullOrEmpty(valoreMinimo) AndAlso String.IsNullOrEmpty(valoreMaximo) Then
                    If Not IsNumeric(valoreMinimo) Then
                        If Not String.IsNullOrEmpty(messaggio) Then
                            messaggio += "<br />"
                        End If
                        messaggio += "Il valore mininmo deve essere un valore numerico"
                        result = False
                    End If

                ElseIf String.IsNullOrEmpty(valoreMinimo) AndAlso Not String.IsNullOrEmpty(valoreMaximo) Then

                    If Not IsNumeric(valoreMaximo) Then
                        If Not String.IsNullOrEmpty(messaggio) Then
                            messaggio += "<br />"
                        End If
                        messaggio += "Il valore massimo deve essere un valore numerico"
                        result = False
                    End If

                ElseIf Not String.IsNullOrEmpty(valoreMinimo) AndAlso Not String.IsNullOrEmpty(valoreMaximo) Then
                    Dim minimo As Double = 0
                    Dim massimo As Double = 0

                    If Not IsNumeric(valoreMinimo) Then
                        If Not String.IsNullOrEmpty(messaggio) Then
                            messaggio += "<br />"
                        End If
                        messaggio += "Il valore mininmo deve essere un valore numerico"
                        result = False
                    Else
                        minimo = CDbl(valoreMinimo)
                    End If

                    If Not IsNumeric(valoreMaximo) Then
                        If Not String.IsNullOrEmpty(messaggio) Then
                            messaggio += "<br />"
                        End If
                        messaggio += "Il valore massimo deve essere un valore numerico"
                        result = False
                    Else
                        massimo = CDbl(valoreMaximo)
                    End If

                    If result AndAlso (minimo > massimo) Then
                        If Not String.IsNullOrEmpty(messaggio) Then
                            messaggio += "<br />"
                        End If
                        messaggio += "Il valore minimo deve essere un valore numerico inferiore, oppure uguale, al valore massimo"
                        result = False
                    End If

                End If

                If numDecimaliMaximo.HasValue AndAlso numDecimaliMaximo.Value < 0 Then
                    If Not String.IsNullOrEmpty(messaggio) Then
                        messaggio += "<br />"
                    End If
                    messaggio += "Il numero massimo di decimali deve essere un valore numerico maggiore, oppure uguale a 0(Zero)"
                    result = False
                End If

                'Case 4 'tipo dato caratteri => verifica valòri min/max per ora non prevista
            Case Else
                If Not String.IsNullOrEmpty(valoreMinimo) Then
                    If Not String.IsNullOrEmpty(messaggio) Then
                        messaggio += "<br />"
                    End If
                    messaggio += "Per la tipologia di dato il valore mininmo deve essere vuoto"
                    result = False
                End If

                If Not String.IsNullOrEmpty(valoreMaximo) Then
                    If Not String.IsNullOrEmpty(messaggio) Then
                        messaggio += "<br />"
                    End If
                    messaggio += "Per la tipologia di dato il valore massimo deve essere vuoto"
                    result = False
                End If

                If Not IsNumeric(numDecimaliMaximo) OrElse
                    (Not numDecimaliMaximo = 0) Then
                    If Not String.IsNullOrEmpty(messaggio) Then
                        messaggio += "<br />"
                    End If
                    messaggio += "Per la tipologia di dato il numero massimo di decimali deve essere vuoto"
                    result = False
                End If
        End Select
        Return result
    End Function

End Class