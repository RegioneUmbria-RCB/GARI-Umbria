Imports AgronicaCoreDataProvider
Imports AgronicaCoreStampeDAL
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports CrystalDecisions.CrystalReports.Engine
Imports System.Dynamic

Public Class Traduttore_DDT : Inherits Traduzione_Stampa_Base

    Private _objectToReportBinder As ExpandoObjectToReportBinder = Nothing
    Private _tipo_report_DDT As Enum_Tipo_Report_DDT



    Public Sub New(piva As String, enum_CodificaStampe As enum_CodificaStampe, Codice_Lingua As String, tipo_Stampa As Integer, ByRef objParametri As AgronicaCoreParametri)
        MyBase.New(piva, enum_CodificaStampe, Codice_Lingua, tipo_Stampa, objParametri)
    End Sub

    Public Sub New(piva As String,
                   enum_CodificaStampe As enum_CodificaStampe,
                   Codice_Lingua As String,
                   tipo_Stampa As Enum_Tipo_Report_DDT,
                   ByVal Report As ReportClass,
                   ByRef objParametri As AgronicaCoreParametri)
        MyBase.New(piva, enum_CodificaStampe, Codice_Lingua, tipo_Stampa, Report, objParametri)

        _tipo_report_DDT = tipo_Stampa

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

        Dim sectioni2 = New List(Of ParametroReportLocalizzabile) From
            {
                New ParametroReportLocalizzabile With
                {
                    .NomeReport = "", .NomeSezione = "Section2", .Sottoreport = False,
                    .NomeParametro = "DPCR", .ValoreParametro = "D.P.R. 472 del 14.08.1996 - D.P.R. 696 del 21.12.1996"
                },
                New ParametroReportLocalizzabile With
                {
                    .NomeReport = "", .NomeSezione = "Section2", .Sottoreport = False,
                    .NomeParametro = "NumDDT", .ValoreParametro = "Num DdT"
                },
                 New ParametroReportLocalizzabile With
                {
                    .NomeReport = "", .NomeSezione = "Section2", .Sottoreport = False,
                    .NomeParametro = "DataEmissione", .ValoreParametro = "Data Emissione"
                },
                 New ParametroReportLocalizzabile With
                {
                    .NomeReport = "", .NomeSezione = "Section2", .Sottoreport = False,
                    .NomeParametro = "Colli", .ValoreParametro = "Colli"
                },
                  New ParametroReportLocalizzabile With
                {
                    .NomeReport = "", .NomeSezione = "Section2", .Sottoreport = False,
                    .NomeParametro = "Litri", .ValoreParametro = "Litri"
                },
                  New ParametroReportLocalizzabile With
                {
                    .NomeReport = "", .NomeSezione = "Section2", .Sottoreport = False,
                    .NomeParametro = "Pagina", .ValoreParametro = "Pagina"
                },
                  New ParametroReportLocalizzabile With
                {
                    .NomeReport = "", .NomeSezione = "Section2", .Sottoreport = False,
                    .NomeParametro = "Destinazione", .ValoreParametro = "Destinazione"
                }
            }

        Dim pageHeaderSection5 = New List(Of ParametroReportLocalizzabile) From
            {
                New ParametroReportLocalizzabile With
                {
                    .NomeReport = "", .NomeSezione = "PageHeaderSection5", .Sottoreport = False,
                    .NomeParametro = "DPCR", .ValoreParametro = "D.P.R. 472 del 14.08.1996 - D.P.R. 696 del 21.12.1996"
                },
                 New ParametroReportLocalizzabile With
                {
                    .NomeReport = "", .NomeSezione = "PageHeaderSection5", .Sottoreport = False,
                    .NomeParametro = "Destinazione", .ValoreParametro = "Destinazione"
                },
                 New ParametroReportLocalizzabile With
                {
                    .NomeReport = "", .NomeSezione = "PageHeaderSection5", .Sottoreport = False,
                    .NomeParametro = "DataEmissione", .ValoreParametro = "Data Emissione"
                },
                  New ParametroReportLocalizzabile With
                {
                    .NomeReport = "", .NomeSezione = "PageHeaderSection5", .Sottoreport = False,
                    .NomeParametro = "NumDDT", .ValoreParametro = "Num DdT"
                },
                   New ParametroReportLocalizzabile With
                {
                    .NomeReport = "", .NomeSezione = "PageHeaderSection5", .Sottoreport = False,
                    .NomeParametro = "Colli", .ValoreParametro = "Colli"
                },
                   New ParametroReportLocalizzabile With
                {
                    .NomeReport = "", .NomeSezione = "PageHeaderSection5", .Sottoreport = False,
                    .NomeParametro = "PesoLordo", .ValoreParametro = "Peso Lordo"
                },
                    New ParametroReportLocalizzabile With
                {
                    .NomeReport = "", .NomeSezione = "PageHeaderSection5", .Sottoreport = False,
                    .NomeParametro = "Tara", .ValoreParametro = "Tara"
                },
                      New ParametroReportLocalizzabile With
                {
                    .NomeReport = "", .NomeSezione = "PageHeaderSection5", .Sottoreport = False,
                    .NomeParametro = "PesoNetto", .ValoreParametro = "Peso Netto"
                },
                        New ParametroReportLocalizzabile With
                {
                    .NomeReport = "", .NomeSezione = "PageHeaderSection5", .Sottoreport = False,
                    .NomeParametro = "Pagina", .ValoreParametro = "Pagina"
                }
            }

        Dim pageHeaderSection1 = New List(Of ParametroReportLocalizzabile) From
            {
                 New ParametroReportLocalizzabile With
                {
                    .NomeReport = "", .NomeSezione = "PageHeaderSection1", .Sottoreport = False,
                    .NomeParametro = "ModalitaPagamento", .ValoreParametro = "Modalità di Pagamento:"
                }
            }

        Dim section15 = New List(Of ParametroReportLocalizzabile) From
            {
                 New ParametroReportLocalizzabile With
                {
                    .NomeReport = "", .NomeSezione = "Section15", .Sottoreport = False,
                    .NomeParametro = "Note", .ValoreParametro = "Note:"
                }
            }

        Dim section6 = New List(Of ParametroReportLocalizzabile) From
            {
                 New ParametroReportLocalizzabile With
                {
                    .NomeReport = "", .NomeSezione = "Section6", .Sottoreport = False,
                    .NomeParametro = "Qta", .ValoreParametro = "Qtà"
                },
                 New ParametroReportLocalizzabile With
                {
                    .NomeReport = "", .NomeSezione = "Section6", .Sottoreport = False,
                    .NomeParametro = "UM", .ValoreParametro = "UM"
                },
                 New ParametroReportLocalizzabile With
                {
                    .NomeReport = "", .NomeSezione = "Section6", .Sottoreport = False,
                    .NomeParametro = "DescrizioneProdotto", .ValoreParametro = "Descrizione Prodotto"
                }
            }

        Dim section12 = New List(Of ParametroReportLocalizzabile) From
            {
                 New ParametroReportLocalizzabile With
                {
                    .NomeReport = "", .NomeSezione = "Section12", .Sottoreport = False,
                    .NomeParametro = "Qta", .ValoreParametro = "Qtà"
                },
                 New ParametroReportLocalizzabile With
                {
                    .NomeReport = "", .NomeSezione = "Section12", .Sottoreport = False,
                    .NomeParametro = "UM", .ValoreParametro = "UM"
                },
                 New ParametroReportLocalizzabile With
                {
                    .NomeReport = "", .NomeSezione = "Section12", .Sottoreport = False,
                    .NomeParametro = "DescrizioneProdotto", .ValoreParametro = "Descrizione Prodotto"
                },
                  New ParametroReportLocalizzabile With
                {
                    .NomeReport = "", .NomeSezione = "Section12", .Sottoreport = False,
                    .NomeParametro = "Prezzo", .ValoreParametro = "Prezzo"
                },
                   New ParametroReportLocalizzabile With
                {
                    .NomeReport = "", .NomeSezione = "Section12", .Sottoreport = False,
                    .NomeParametro = "Sconto", .ValoreParametro = "Sconto"
                }
            }


        Dim section7 = New List(Of ParametroReportLocalizzabile) From
            {
                 New ParametroReportLocalizzabile With
                {
                    .NomeReport = "", .NomeSezione = "Section7", .Sottoreport = False,
                    .NomeParametro = "Qta", .ValoreParametro = "Qtà"
                },
                 New ParametroReportLocalizzabile With
                {
                    .NomeReport = "", .NomeSezione = "Section7", .Sottoreport = False,
                    .NomeParametro = "UM", .ValoreParametro = "UM"
                },
                 New ParametroReportLocalizzabile With
                {
                    .NomeReport = "", .NomeSezione = "Section7", .Sottoreport = False,
                    .NomeParametro = "DescrizioneProdotto", .ValoreParametro = "Descrizione Prodotto"
                },
                 New ParametroReportLocalizzabile With
                {
                    .NomeReport = "", .NomeSezione = "Section7", .Sottoreport = False,
                    .NomeParametro = "PesoLordo", .ValoreParametro = "PesoLordo"
                },
                 New ParametroReportLocalizzabile With
                {
                    .NomeReport = "", .NomeSezione = "Section7", .Sottoreport = False,
                    .NomeParametro = "Tara", .ValoreParametro = "Tara"
                },
                 New ParametroReportLocalizzabile With
                {
                    .NomeReport = "", .NomeSezione = "Section7", .Sottoreport = False,
                    .NomeParametro = "PesoNetto", .ValoreParametro = "PesoNetto"
                },
                  New ParametroReportLocalizzabile With
                {
                    .NomeReport = "", .NomeSezione = "Section7", .Sottoreport = False,
                    .NomeParametro = "Prezzo", .ValoreParametro = "Prezzo"
                }
            }

        Dim section4 = New List(Of ParametroReportLocalizzabile) From
            {
                 New ParametroReportLocalizzabile With
                {
                    .NomeReport = "", .NomeSezione = "Section4", .Sottoreport = False,
                    .NomeParametro = "Articolo62", .ValoreParametro = "Assolve agli obblighi dell'Art. 3 del D.Lgs 198/2021 e s.m.i. Il contratto ha durata per la presente consegna."
                }
            }

        Dim section16 = New List(Of ParametroReportLocalizzabile) From
            {
                 New ParametroReportLocalizzabile With
                {
                    .NomeReport = "", .NomeSezione = "Section16", .Sottoreport = False,
                    .NomeParametro = "TrattamentoDati", .ValoreParametro = "Dichiariamo che i suoi dati verranno trattati In relazione alle esigenze contrattuali ed ai conseguenti adempimenti degli obblighi legali e fiscali, come previsto dall'art. 13 D.Lgs 196/2003 (Codice in Materia di Protezione dei Dati Personali)"
                }
            }

        Dim section17 = New List(Of ParametroReportLocalizzabile) From
            {
                 New ParametroReportLocalizzabile With
                {
                    .NomeReport = "", .NomeSezione = "Section17", .Sottoreport = False,
                    .NomeParametro = "TrattamentoDati", .ValoreParametro = "Trattiamo e tuteliamo i Vostri dati esclusivamente per fini amministrativi e contabili In conformità al GDPR - Regolamento UE 2016/679."
                }
            }

        Dim section5 = New List(Of ParametroReportLocalizzabile) From
            {
                 New ParametroReportLocalizzabile With
                {
                    .NomeReport = "", .NomeSezione = "Section5", .Sottoreport = False,
                    .NomeParametro = "AspettoBeni", .ValoreParametro = "Aspetto dei beni:"
                },
                  New ParametroReportLocalizzabile With
                {
                    .NomeReport = "", .NomeSezione = "Section5", .Sottoreport = False,
                    .NomeParametro = "Causale", .ValoreParametro = "Causale:"
                },
                   New ParametroReportLocalizzabile With
                {
                    .NomeReport = "", .NomeSezione = "Section5", .Sottoreport = False,
                    .NomeParametro = "TrasportoACura", .ValoreParametro = "Trasporto a cura del:"
                },
                New ParametroReportLocalizzabile With
                {
                    .NomeReport = "", .NomeSezione = "Section5", .Sottoreport = False,
                    .NomeParametro = "GestioneVettore", .ValoreParametro = "Gestione vettore:"
                },
                  New ParametroReportLocalizzabile With
                {
                    .NomeReport = "", .NomeSezione = "Section5", .Sottoreport = False,
                    .NomeParametro = "Vettore", .ValoreParametro = "Vettore:"
                },
                   New ParametroReportLocalizzabile With
                {
                    .NomeReport = "", .NomeSezione = "Section5", .Sottoreport = False,
                    .NomeParametro = "DataSpedizione", .ValoreParametro = "Data spedizione:"
                },
                New ParametroReportLocalizzabile With
                {
                    .NomeReport = "", .NomeSezione = "Section5", .Sottoreport = False,
                    .NomeParametro = "FirmaVettore", .ValoreParametro = "Firma Vettore/Conducente:"
                },
                 New ParametroReportLocalizzabile With
                {
                    .NomeReport = "", .NomeSezione = "Section5", .Sottoreport = False,
                    .NomeParametro = "DataConsegna", .ValoreParametro = "Data consegna:"
                },
                 New ParametroReportLocalizzabile With
                {
                    .NomeReport = "", .NomeSezione = "Section5", .Sottoreport = False,
                    .NomeParametro = "FirmaDestinatario", .ValoreParametro = "Firma Destinatario:"
                }
            }


        Dim bolla2016 = sectioni2.Concat(pageHeaderSection5).Concat(pageHeaderSection1) _
                    .Concat(section15).Concat(section6).Concat(section12).Concat(section7) _
                    .Concat(section4).Concat(section16).Concat(section17).Concat(section5) _
                    .ToList().ToExpando()

        Dim traduzioni As ExpandoObject = Nothing

        Select Case Tipo_Stampa
            Case Enum_Tipo_Report_DDT.CRBolla2016
                traduzioni = New Dictionary(Of String, Object) From
                {
                    {"CR_BOLLA2016", bolla2016}
                }.ToExpando
            Case Enum_Tipo_Report_DDT.CRBolla2016_LB
                traduzioni = New Dictionary(Of String, Object) From
               {
                   {"CRBolla2016_LB", bolla2016}
               }.ToExpando
            Case Enum_Tipo_Report_DDT.CRBolla
                traduzioni = New Dictionary(Of String, Object) From
              {
                  {"CR_BOLLA", bolla2016}
              }.ToExpando
        End Select

        Return traduzioni

    End Function



End Class

Public Enum Enum_Tipo_Report_DDT
    Undefined = 0
    CRBolla2016 = 1
    CRBolla2016_LB = 2
    CRBolla = 3
End Enum