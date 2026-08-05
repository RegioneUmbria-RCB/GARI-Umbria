Imports AgronicaCoreDataProvider
Imports AgronicaCoreStampeDAL
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports CrystalDecisions.CrystalReports.Engine
Imports System.Dynamic

Public Class TraduttoreSchedaCampagna : Inherits Traduzione_Stampa_Base

    Private _objectToReportBinder As ExpandoObjectToReportBinder = Nothing
    Private _tipo_report_Scheda_Campagna As Enum_Tipo_Report_Scheda_Campagna

    Public Sub New(piva As String, enum_CodificaStampe As enum_CodificaStampe, Codice_Lingua As String, tipo_Stampa As Integer, ByRef objParametri As AgronicaCoreParametri)
        MyBase.New(piva, enum_CodificaStampe, Codice_Lingua, tipo_Stampa, objParametri)
    End Sub

    Public Sub New(piva As String,
                   enum_CodificaStampe As enum_CodificaStampe,
                   Codice_Lingua As String,
                   tipo_Stampa As Enum_Tipo_Report_Scheda_Campagna,
                   ByVal Report As ReportClass,
                   ByRef objParametri As AgronicaCoreParametri)
        MyBase.New(piva, enum_CodificaStampe, Codice_Lingua, tipo_Stampa, Report, objParametri)

        _tipo_report_Scheda_Campagna = tipo_Stampa

    End Sub
    Public Overrides Sub Traduci(codice_Lingua As String)

    End Sub

    Public Overrides Sub Leggi(xFiltroAggiuntivo As String, xOrderBy As String)
        MyBase.Leggi(xFiltroAggiuntivo, xOrderBy)
    End Sub


    Public Overrides Sub Traduci(xFiltroAggiuntivo As String, xOrderBy As String)


        Dim traduzione = InizializzaOggettoTraduzioni()
        MyBase.Leggi(xFiltroAggiuntivo, xOrderBy)

        _objectToReportBinder = New ExpandoObjectToReportBinder(traduzione, Report, DtTraduzioni)
        _objectToReportBinder.FIllPropertiesFromDB()
        _objectToReportBinder.BindPropertiesToReport()

    End Sub

    Protected Overrides Function InizializzaOggettoTraduzioni() As ExpandoObject


        Dim sectioni1 = New List(Of ParametroReportLocalizzabile) From
            {
                 New ParametroReportLocalizzabile With
                {
                    .NomeReport = "", .NomeSezione = "Section1", .Sottoreport = False,
                    .NomeParametro = "TitoloReport", .ValoreParametro = "Scheda di Campagna per Autocertificazione GLOBALG.A.P."
                },
                New ParametroReportLocalizzabile With
                {
                    .NomeReport = "", .NomeSezione = "Section1", .Sottoreport = False,
                    .NomeParametro = "CultureErbacee", .ValoreParametro = "COLTURE ARBOREE"
                }
            }

        Dim report = sectioni1.ToExpando()
        Dim traduzioni As ExpandoObject = Nothing

        traduzioni = New Dictionary(Of String, Object) From
        {
            {"Rpt_SchedaCampagna_EUREP_GAP_Multicentro", report}
        }.ToExpando


        Return traduzioni

    End Function



End Class

Public Enum Enum_Tipo_Report_Scheda_Campagna

    'SchedaCampagna = 0  'Disattivata
    SchedaCampagna_Semplificata = 1
    'Eurep_Gap = 2   'Disattivata
    SchedaCampagna_ConserveItalia = 3
    Eurep_Gap_Semplificata = 4
    SchedaCampagna_Pizzoli = 5
    SchedaCampagna_Multi = 6    'Multi Specie Multi Centri
    RegistroTrattamenti_Veneto = 7
    SchedaCampagna_Multicentro = 8
    Eurep_Gap_Multicentro = 9
    Prov_Aut_Trento = 10
    RegistroTrattamenti_VenetoUnico = 11
End Enum
