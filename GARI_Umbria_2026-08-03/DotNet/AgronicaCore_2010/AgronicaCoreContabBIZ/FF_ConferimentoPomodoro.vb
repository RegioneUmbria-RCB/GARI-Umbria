Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports Newtonsoft.Json

Public Class FF_ConferimentoPomodoro
    Inherits AgronicaCoreDataProvider.LogProvider

    Public Class Contratto_Conferimento_Pomodoro

        Public Property Contratto_Cod As Integer
        Public Property Fase_Cod As Integer

        Public Property ChkPremio As Boolean
        Public Property Premio As Decimal
        Public Property Tetto As Decimal
        Public Property DataInizioPremio As Date
        Public Property DataFinePremio As Date

        Public Property ChkPremio2 As Boolean
        Public Property Premio2 As Decimal
        Public Property Tetto2 As Decimal
        Public Property DataInizioPremio2 As Date
        Public Property DataFinePremio2 As Date


        Public Property TotaleDMA_MAX As Decimal
        Public Property Franchigia As Decimal
        Public Property TotaleDMI_MAX As Decimal
        Public Property Coefficiente As Decimal
        Public Property Abbattimento As Decimal
        Public Property Maggiorazione As Decimal

        Public Property Listino_Cod As Integer
        Public Property Prezzo As Decimal

    End Class

    Public Class Parametri_Conferimento_Pomodoro

        Public Property Pomodoro_BIO As Boolean

        ' Difetti Maggiori
        Public Property Inerti As Decimal
        Public Property Inerti_MAX As Decimal = 4
        Public Property Verde As Decimal
        Public Property Verde_MAX As Decimal = 3.5
        Public Property Marcio As Decimal
        Public Property Marcio_MAX As Decimal = 5


        ' Difetti Minori
        Public Property ResiduoOttico As Decimal
        Public Property FruttiSchiacciati As Decimal
        Public Property FruttiSchiacciati_MAX As Decimal = 100
        Public Property FruttiImmaturi As Decimal
        Public Property FruttiImmaturi_MAX As Decimal = 100
        Public Property FruttiScottati As Decimal
        Public Property FruttiScottati_MAX As Decimal = 100
        Public Property FruttiLesionati As Decimal
        Public Property FruttiLesionati_MAX As Decimal = 100


    End Class

    Public Class Riepilogo_Conferimento_Pomodoro

        Public Property Inerti_KG As Decimal
        Public Property Verde_KG As Decimal
        Public Property Marcio_KG As Decimal
        Public Property IndicePrezzo As Decimal
        Public Property FruttiSchiacciati_KG As Decimal
        Public Property FruttiImmaturi_KG As Decimal
        Public Property FruttiScottati_KG As Decimal
        Public Property FruttiLesionati_KG As Decimal

        Public Property TotaleDMA As Decimal
        Public Property TotaleDMA_MAX As Decimal
        Public Property TotaleDMA_KG As Decimal

        Public Property TotaleDMI As Decimal
        Public Property TotaleDMI_MAX As Decimal
        Public Property TotaleDMI_KG As Decimal

        Public Property Totale_Difetti As Decimal
        Public Property Totale_Difetti_KG As Decimal

        Public Property Indice_Pomodoro As Decimal
        Public Property Riduzione As Decimal
        Public Property Variazione As Decimal
        Public Property Peso_Effettivo As Decimal

        Public Property Giorni As Integer
        Public Property Giorni2 As Integer
        Public Property Premio As Decimal
        Public Property Maggiorazione As Decimal
        Public Property Premio_Complessivo As Decimal
        Public Property Prezzo_Contratto As Decimal
        Public Property Prezzo_Finale As Decimal
        Public Property Prezzo_Netto As Decimal
        Public Property Prezzo_Totale As Decimal

        Public Dati_Necessari As Boolean = True
        Public Errori As String

    End Class

    Public Function CalcolaIndicePrezzo(ByVal Anno As Integer, ByVal ResiduoOttico As Decimal) As Decimal
        Dim IndicePrezzo As Decimal = 100
        Select Case Anno
            Case 2020
                Select Case ResiduoOttico
                    Case Is <= 4.2
                        IndicePrezzo = 82.5
                    Case Is >= 5.6
                        IndicePrezzo = 117.5
                    Case Else
                        IndicePrezzo = (ResiduoOttico - 4.9) * 25 + 100
                End Select
            Case 2021 To 2025
                Select Case ResiduoOttico
                    Case Is <= 4.15
                        IndicePrezzo = 82.5
                    Case Is >= 5.55
                        IndicePrezzo = 117.5
                    Case Else
                        IndicePrezzo = (ResiduoOttico - 4.85) * 25 + 100
                End Select
            Case 2026
                Select Case ResiduoOttico
                    Case Is <= 4.2
                        IndicePrezzo = 82.5
                    Case Is >= 5.6
                        IndicePrezzo = 117.5
                    Case Else
                        IndicePrezzo = (ResiduoOttico - 4.9) * 25 + 100
                End Select
            Case Else ' Replico i conteggi più recenti
                Select Case ResiduoOttico
                    Case Is <= 4.2
                        IndicePrezzo = 82.5
                    Case Is >= 5.6
                        IndicePrezzo = 117.5
                    Case Else
                        IndicePrezzo = (ResiduoOttico - 4.9) * 25 + 100
                End Select
        End Select
        Return IndicePrezzo
    End Function

    Public Function CalcolaVariazioneDifettiMaggiori(ByVal Anno As Integer, ByVal TotaleDMA As Decimal) As Decimal
        Dim variazione As Decimal = 0
        Select Case Anno
            Case 2020 To 2025
                If TotaleDMA < 3 Then
                    variazione = 3 - TotaleDMA
                ElseIf TotaleDMA > 4 Then
                    variazione = 4 - TotaleDMA
                Else
                    variazione = 0
                End If
            Case 2026
                If TotaleDMA < 2 Then
                    variazione = 2 - TotaleDMA
                ElseIf TotaleDMA > 3 Then
                    variazione = 3 - TotaleDMA
                Else
                    variazione = 0
                End If
            Case Else ' Replico i conteggi più recenti
                If TotaleDMA < 2 Then
                    variazione = 2 - TotaleDMA
                ElseIf TotaleDMA > 3 Then
                    variazione = 3 - TotaleDMA
                Else
                    variazione = 0
                End If
        End Select
        Return variazione
    End Function


    Public Function CalcolaPrezzoPomodoro(ByVal Peso_Netto As Decimal, ByVal Prezzo As Decimal, ByVal Data As Date, ByRef Parametri As Parametri_Conferimento_Pomodoro, ByRef Contratto As Contratto_Conferimento_Pomodoro) As Riepilogo_Conferimento_Pomodoro

        Dim Riepilogo As New Riepilogo_Conferimento_Pomodoro
        Dim Errori As New StringBuilder

        ' Imposto il prezzo base prendendolo dal contratto
        Riepilogo.Prezzo_Contratto = Contratto.Prezzo
        If Contratto.Prezzo > 0 Then
            Prezzo = Contratto.Prezzo
        End If

        '###################################################################################
        '############################## DIFETTI MAGGIORI ###################################
        '###################################################################################

        Riepilogo.Inerti_KG = (Peso_Netto * Parametri.Inerti) / 100
        Riepilogo.Verde_KG = (Peso_Netto * Parametri.Verde) / 100
        Riepilogo.Marcio_KG = (Peso_Netto * Parametri.Marcio) / 100
        Riepilogo.TotaleDMA = Parametri.Inerti + Parametri.Verde + Parametri.Marcio
        Riepilogo.TotaleDMA_KG = Peso_Netto * Riepilogo.TotaleDMA / 100

        '###################################################################################
        '############################### DIFETTI MINORI ####################################
        '###################################################################################

        If Parametri.Pomodoro_BIO Then
            Riepilogo.IndicePrezzo = 100
        Else
            Riepilogo.IndicePrezzo = CalcolaIndicePrezzo(Data.Year, Parametri.ResiduoOttico)
        End If

        Riepilogo.FruttiSchiacciati_KG = (Peso_Netto * Parametri.FruttiSchiacciati) / 100
        Riepilogo.FruttiImmaturi_KG = (Peso_Netto * Parametri.FruttiImmaturi) / 100
        Riepilogo.FruttiScottati_KG = (Peso_Netto * Parametri.FruttiScottati) / 100
        Riepilogo.FruttiLesionati_KG = (Peso_Netto * Parametri.FruttiLesionati) / 100

        Riepilogo.TotaleDMI = Parametri.FruttiSchiacciati + Parametri.FruttiImmaturi + Parametri.FruttiScottati + Parametri.FruttiLesionati
        Riepilogo.TotaleDMI_KG = Riepilogo.FruttiSchiacciati_KG + Riepilogo.FruttiImmaturi_KG + Riepilogo.FruttiScottati_KG + Riepilogo.FruttiLesionati_KG

        Riepilogo.Totale_Difetti = Riepilogo.TotaleDMA + Riepilogo.TotaleDMI
        Riepilogo.Totale_Difetti_KG = Riepilogo.TotaleDMA_KG + Riepilogo.TotaleDMI_KG

        '=======================================================================================================
        'Verifica Conformità
        '-------------------------------------------------------------------------------------------------------

        'Marcio
        If Parametri.Marcio > Parametri.Marcio_MAX Then
            Errori.AppendLine("Marcio %: superiore al limite massimo consentito<br>")
            Riepilogo.Dati_Necessari = False
        End If

        'Verde
        If Parametri.Verde > Parametri.Verde_MAX Then
            Errori.AppendLine("Verde %: superiore al limite massimo consentito<br>")
            Riepilogo.Dati_Necessari = False
        End If

        'Inerti
        If Parametri.Inerti > Parametri.Inerti_MAX Then
            Errori.AppendLine("Inerti %: superiore al limite massimo consentito<br>")
            Riepilogo.Dati_Necessari = False
        End If

        ' Dati Contratto
        Riepilogo.TotaleDMA_MAX = Contratto.TotaleDMA_MAX
        Riepilogo.TotaleDMI_MAX = Contratto.TotaleDMI_MAX

        'Totale_DMA
        If Riepilogo.TotaleDMA > Riepilogo.TotaleDMA_MAX Then
            Errori.AppendLine("Totale difetti maggiori superiore al limite massimo consentito.<br>")
            Riepilogo.Dati_Necessari = False
        End If

        '==================================================================================================
        'Calcolo Indice Base
        '--------------------------------------------------------------------------------------------------

        If Not Parametri.Pomodoro_BIO Then

            If Parametri.ResiduoOttico > 0 Then

                'Frutti_Schiacciati
                If Parametri.FruttiSchiacciati > Parametri.FruttiSchiacciati_MAX Then
                    Riepilogo.Dati_Necessari = False
                End If

                'Frutti_Immaturi
                If Parametri.FruttiImmaturi > Parametri.FruttiImmaturi_MAX Then
                    Riepilogo.Dati_Necessari = False
                End If

                'Frutti_Scottati
                If Parametri.FruttiScottati > Parametri.FruttiScottati_MAX Then
                    Riepilogo.Dati_Necessari = False
                End If

                'Frutti_Lesionati
                If Parametri.FruttiLesionati > Parametri.FruttiLesionati_MAX Then
                    Riepilogo.Dati_Necessari = False
                End If

                'Totale_DMI
                If Riepilogo.TotaleDMI > Riepilogo.TotaleDMI_MAX Then
                    Errori.AppendLine("Totale difetti minori superiore al limite massimo consentito.<br>")
                    Riepilogo.Dati_Necessari = False
                End If

                Riepilogo.Riduzione = Riepilogo.TotaleDMI * Contratto.Coefficiente

                If Riepilogo.Riduzione > Contratto.Abbattimento Then
                    Riepilogo.Dati_Necessari = False
                End If

                ' Calcolo riduzione (può variare di anno in anno)
                If Parametri.Pomodoro_BIO Then
                    Riepilogo.Variazione = 0
                Else
                    Riepilogo.Variazione = CalcolaVariazioneDifettiMaggiori(Data.Year, Riepilogo.TotaleDMA)
                End If

                Riepilogo.Indice_Pomodoro = Riepilogo.IndicePrezzo - Format(Riepilogo.Riduzione, "###.00") + Riepilogo.Variazione

            End If

        End If

        '==================================================================================================
        'Calcolo Premio Tardivo o Surgelato
        '--------------------------------------------------------------------------------------------------

        If Contratto.ChkPremio Then
            Riepilogo.Giorni = DateDiff("d", Contratto.DataInizioPremio, Data) + 1
            If Riepilogo.Giorni < 0 Then
                Riepilogo.Giorni = 0
            ElseIf Contratto.DataFinePremio <> AGRODATAFINE Then
                If DateDiff("d", Contratto.DataFinePremio, Data) > 0 Then
                    Riepilogo.Giorni = DateDiff("d", Contratto.DataInizioPremio, Contratto.DataFinePremio) + 1
                End If
            End If
        End If

        If Contratto.ChkPremio2 Then
            Riepilogo.Giorni2 = DateDiff("d", Contratto.DataInizioPremio2, Data) + 1
            If Riepilogo.Giorni2 < 0 Then
                Riepilogo.Giorni2 = 0
            ElseIf Contratto.DataFinePremio2 <> AGRODATAFINE Then
                If DateDiff("d", Contratto.DataFinePremio2, Data) > 0 Then
                    Riepilogo.Giorni2 = DateDiff("d", Contratto.DataInizioPremio2, Contratto.DataFinePremio2) + 1
                End If
            End If
        End If

        Riepilogo.Premio = Contratto.Premio * Riepilogo.Giorni + Contratto.Premio2 * Riepilogo.Giorni2
        If Riepilogo.Premio > Contratto.Tetto Then
            Riepilogo.Premio = Contratto.Tetto
        End If

        'Calcolo Peso Effettivo
        Riepilogo.Peso_Effettivo = Format(Peso_Netto - Riepilogo.TotaleDMA_KG, "##,###,##0")

        '==========================================================================================================
        'Premio Pomodoro Biologico
        '----------------------------------------------------------------------------------------------------------
        'Calcolo Maggiorazione
        Riepilogo.Maggiorazione = Format(Contratto.Maggiorazione * (Riepilogo.Peso_Effettivo / 1000), "###,###,##0.00")

        'Calcolo Premio Complessivo
        Riepilogo.Premio_Complessivo = Format(Riepilogo.Premio * (Riepilogo.Peso_Effettivo / 1000), "###,###,##0.00")
        '==========================================================================================================

        If Parametri.Pomodoro_BIO Then

            'Calcolo Prezzo da contratto + premio pomodoro tardivo
            If Riepilogo.Peso_Effettivo <> 0 Then
                Riepilogo.Prezzo_Finale = Format(Prezzo + (Riepilogo.Premio_Complessivo / Riepilogo.Peso_Effettivo), "###,###,##0.00###")  '5 decimali
            End If

        Else

            'Calcolo Prezzo Complessivo Unitario
            If Riepilogo.Peso_Effettivo <> 0 Then
                Riepilogo.Prezzo_Finale = Format((Prezzo * Riepilogo.Indice_Pomodoro / 100) + (Riepilogo.Premio_Complessivo / Riepilogo.Peso_Effettivo) + (Riepilogo.Maggiorazione / Riepilogo.Peso_Effettivo), "###,###,##0.00###") '5 decimali
            End If

        End If

        ' prezzo totale
        Riepilogo.Prezzo_Totale = Riepilogo.Prezzo_Finale * Riepilogo.Peso_Effettivo

        ' rettifico il prezzo rapportandolo al peso netto prodotto conferito
        Riepilogo.Prezzo_Netto = Riepilogo.Prezzo_Finale * (Riepilogo.Peso_Effettivo / Peso_Netto)

        Riepilogo.Errori = Errori.ToString()

        Return Riepilogo

    End Function

    Public Function RiepilogoDatiPomodoro(ByRef Parametri As Parametri_Conferimento_Pomodoro, ByRef Contratto As Contratto_Conferimento_Pomodoro, ByRef Riepilogo As Riepilogo_Conferimento_Pomodoro) As String

        'Dim Dati_Riepilogo = JsonConvert.SerializeObject(Riepilogo, Formatting.None)
        'Dati_Riepilogo = Replace(Replace(Dati_Riepilogo, "}", ""), "{", "")
        'Return Replace(Dati_Riepilogo, ",", "<br>")
        Dim Inerti_Alert As String = If(Parametri.Inerti > Parametri.Inerti_MAX, "danger", "")
        Dim Verde_Alert As String = If(Parametri.Verde > Parametri.Verde_MAX, "danger", "")
        Dim Marcio_Alert As String = If(Parametri.Marcio > Parametri.Marcio_MAX, "danger", "")
        Dim TotaleDMA_Alert As String = If(Riepilogo.TotaleDMA > Riepilogo.TotaleDMA_MAX, "danger", "")
        Dim TotaleDMI_Alert As String = If(Not Parametri.Pomodoro_BIO AndAlso Riepilogo.TotaleDMI > Riepilogo.TotaleDMI_MAX, "danger", "")

        Dim Dati_Riepilogo As New StringBuilder

        Dati_Riepilogo.AppendLine("<div class=""row""><div class=""col-lg-6 col-sm-12"">")
        Dati_Riepilogo.AppendLine("<table class=""table table-bordered"">")
        Dati_Riepilogo.AppendLine("<tr class=""active""><th colspan=""2"">RIDUZIONI SU QUANTITA'</th></tr>")
        Dati_Riepilogo.AppendLine("<tr class=""" & Inerti_Alert & """><td>% materiali inerti</td><td class=""text-right"">" & Format(Parametri.Inerti, "##0.00") & "</td>")
        Dati_Riepilogo.AppendLine("<tr class=""" & Verde_Alert & """><td>% pomodoro verde</td><td class=""text-right"">" & Format(Parametri.Verde, "##0.00") & "</td>")
        Dati_Riepilogo.AppendLine("<tr class=""" & Marcio_Alert & """><td>% pomodoro marcio</td><td class=""text-right"">" & Format(Parametri.Marcio, "##0.00") & "</td>")
        Dati_Riepilogo.AppendLine("<tr class=""" & TotaleDMA_Alert & """><td>Tasso riduzione %</td><td class=""text-right"">" & Format(Riepilogo.TotaleDMA, "##0.00") & "</td>")
        Dati_Riepilogo.AppendLine("<tr><td>Pari a uno scarto di Q.li</td><td class=""text-right"">" & Format(Riepilogo.TotaleDMA_KG / 100, "###,###,##0.00") & "</td>")
        Dati_Riepilogo.AppendLine("<tr><td>Peso netto a pagamento Q.li</td><td class=""text-right"">" & Format(Riepilogo.Peso_Effettivo / 100, "###,###,##0.00") & "</td>")
        Dati_Riepilogo.AppendLine("</table>")
        Dati_Riepilogo.AppendLine("<table class=""table table-bordered"">")
        Dati_Riepilogo.AppendLine("<tr><th class=""active"" colspan=""2"">IMPORTI</th></tr>")
        Dati_Riepilogo.AppendLine("<tr><td>Prezzo unitario da contratto €/t</td><td class=""text-right"">" & Format(Contratto.Prezzo * 1000, "###,###,##0.00") & "</td>")
        Dati_Riepilogo.AppendLine("<tr><td>Prezzo Unitario Finale €/t</td><td class=""text-right"">" & Format(Riepilogo.Prezzo_Finale * 1000, "###,###,##0.00") & "</td>")
        Dati_Riepilogo.AppendLine("<tr><td>Importo totale a pagamento €</td><td class=""text-right"">" & Format(Riepilogo.Prezzo_Totale, "###,###,##0.00") & "</td>")
        Dati_Riepilogo.AppendLine("</table>")
        Dati_Riepilogo.AppendLine("</div><div class=""col-lg-6 col-sm-12"">")
        Dati_Riepilogo.AppendLine("<table class=""table table-bordered"">")
        Dati_Riepilogo.AppendLine("<tr class=""active""><th colspan=""2"">VARIAZIONI SU PREZZO</th></tr>")
        Dati_Riepilogo.AppendLine("<tr><td>Residuo ottico grado Brix</td><td class=""text-right"">" & Format(Parametri.ResiduoOttico, "##0.00") & "</td>")
        Dati_Riepilogo.AppendLine("<tr><td>Indice prezzo grado Brix</td><td class=""text-right"">" & Format(Riepilogo.IndicePrezzo, "##0.00") & "</td>")
        Dati_Riepilogo.AppendLine("<tr><td>% frutti schiacciati/spaccati</td><td class=""text-right"">" & Format(Parametri.FruttiSchiacciati, "##0.00") & "</td>")
        Dati_Riepilogo.AppendLine("<tr><td>% frutti immaturi</td><td class=""text-right"">" & Format(Parametri.FruttiImmaturi, "##0.00") & "</td>")
        Dati_Riepilogo.AppendLine("<tr><td>% frutti con scottature solari</td><td class=""text-right"">" & Format(Parametri.FruttiScottati, "##0.00") & "</td>")
        Dati_Riepilogo.AppendLine("<tr><td>% frutti con lesioni cicatrizzate</td><td class=""text-right"">" & Format(Parametri.FruttiLesionati, "##0.00") & "</td>")
        Dati_Riepilogo.AppendLine("<tr class=""" & TotaleDMI_Alert & """><td>Totale percentuali</td><td class=""text-right"">" & Format(Riepilogo.TotaleDMI, "##0.00") & "</td>")
        Dati_Riepilogo.AppendLine("<tr><td>Pari a una riduzione %</td><td class=""text-right"">" & Format(-Riepilogo.Riduzione, "##0.00") & "</td>")
        Dati_Riepilogo.AppendLine("<tr><td>Maggiorazione/riduzione %</td><td class=""text-right"">" & Format(Riepilogo.Variazione, "##0.00") & "</td>")
        Dati_Riepilogo.AppendLine("<tr><td>Indice base 100 di variazione prezzo</td><td class=""text-right"">" & Format(Riepilogo.Indice_Pomodoro, "###,###,##0.00") & "</td>")
        Dati_Riepilogo.AppendLine("<tr><td>Premio pomodoro tardivo €</td><td class=""text-right"">" & Format(Riepilogo.Premio_Complessivo, "###,###,##0.00") & "</td>")
        Dati_Riepilogo.AppendLine("</table>")
        Dati_Riepilogo.AppendLine("</div></div>")

        'Dati_Riepilogo.AppendLine("<div class=""row""><div class=""col-lg-6 col-sm-12"">")
        'Dati_Riepilogo.AppendLine("<table class=""table table-bordered"">")
        'Dati_Riepilogo.AppendLine("<tr class=""active""><th colspan=""2"">Difetti Maggiori</th></tr>")
        'Dati_Riepilogo.AppendLine("<tr class=""" & Inerti_Alert & """><td>Inerti Kg</td><td class=""text-right"">" & Format(Riepilogo.Inerti_KG, "###,###,##0.00") & "</td>")
        'Dati_Riepilogo.AppendLine("<tr class=""" & Verde_Alert & """><td>Verde Kg</td><td class=""text-right"">" & Format(Riepilogo.Verde_KG, "###,###,##0.00") & "</td>")
        'Dati_Riepilogo.AppendLine("<tr class=""" & Marcio_Alert & """><td>Marcio Kg</td><td class=""text-right"">" & Format(Riepilogo.Marcio_KG, "###,###,##0.00") & "</td>")
        'Dati_Riepilogo.AppendLine("<tr class=""" & TotaleDMA_Alert & """><td>Totale Difetti % (Max " & Format(Riepilogo.TotaleDMA_MAX, "##0.00") & "%)</td><td class=""text-right"">" & Format(Riepilogo.TotaleDMA, "##0.00") & "</td>")
        'Dati_Riepilogo.AppendLine("<tr class=""" & TotaleDMA_Alert & """><td>Totale Difetti Kg</td><td class=""text-right"">" & Format(Riepilogo.TotaleDMA_KG, "###,###,##0") & "</td>")
        'Dati_Riepilogo.AppendLine("</table>")
        'Dati_Riepilogo.AppendLine("<table class=""table table-bordered"">")
        'Dati_Riepilogo.AppendLine("<tr class=""active""><th colspan=""2"">Difetti Minori</th></tr>")
        'Dati_Riepilogo.AppendLine("<tr><td>Indice Prezzo</td><td class=""text-right"">" & Format(Riepilogo.IndicePrezzo, "##0.00") & "</td>")
        'Dati_Riepilogo.AppendLine("<tr><td>Frutti Schiacciati Kg</td><td class=""text-right"">" & Format(Riepilogo.FruttiSchiacciati_KG, "###,###,##0.00") & "</td>")
        'Dati_Riepilogo.AppendLine("<tr><td>Frutti Immaturi Kg</td><td class=""text-right"">" & Format(Riepilogo.FruttiImmaturi_KG, "###,###,##0.00") & "</td>")
        'Dati_Riepilogo.AppendLine("<tr><td>Frutti Scottati Kg</td><td class=""text-right"">" & Format(Riepilogo.FruttiScottati_KG, "###,###,##0.00") & "</td>")
        'Dati_Riepilogo.AppendLine("<tr><td>Frutti Lesionati Kg</td><td class=""text-right"">" & Format(Riepilogo.FruttiLesionati_KG, "###,###,##0.00") & "</td>")
        'Dati_Riepilogo.AppendLine("<tr class=""" & TotaleDMI_Alert & """><td>Totale Difetti % (Max " & Format(Riepilogo.TotaleDMI_MAX, "##0.00") & "%)</td><td class=""text-right"">" & Format(Riepilogo.TotaleDMI, "##0.00") & "</td>")
        'Dati_Riepilogo.AppendLine("<tr class=""" & TotaleDMI_Alert & """><td>Totale Difetti Kg</td><td class=""text-right"">" & Format(Riepilogo.TotaleDMI_KG, "###,###,##0") & "</td>")
        'Dati_Riepilogo.AppendLine("</table>")
        'Dati_Riepilogo.AppendLine("</div><div class=""col-lg-6 col-sm-12"">")
        'Dati_Riepilogo.AppendLine("<table class=""table table-bordered"">")
        'Dati_Riepilogo.AppendLine("<tr><th class=""active"" colspan=""2"">Dati Riepilogativi</th></tr>")
        'Dati_Riepilogo.AppendLine("<tr><td>Totale Difetti % </td><td class=""text-right"">" & Format(Riepilogo.Totale_Difetti, "##0.00") & "</td>")
        'Dati_Riepilogo.AppendLine("<tr><td>Totale Difetti Kg</td><td class=""text-right"">" & Format(Riepilogo.Totale_Difetti_KG, "###,###,##0") & "</td>")
        'Dati_Riepilogo.AppendLine("<tr><td>Premio Applicato €/t</td><td class=""text-right"">" & Format(Contratto.Premio, "###,###,##0.00") & "</td>")
        'Dati_Riepilogo.AppendLine("<tr><td>Tetto €/t</td><td class=""text-right"">" & Format(Contratto.Tetto, "###,###,##0.00") & "</td>")
        'Dati_Riepilogo.AppendLine("<tr><td>Franchigia %</td><td class=""text-right"">" & Format(Contratto.Franchigia, "##0.00") & "</td>")
        'Dati_Riepilogo.AppendLine("<tr><td>Coefficiente %</td><td class=""text-right"">" & Format(Contratto.Coefficiente, "##0.00") & "</td>")
        'Dati_Riepilogo.AppendLine("<tr><td>Abbattimento %</td><td class=""text-right"">" & Format(Contratto.Abbattimento, "##0.00") & "</td>")
        'Dati_Riepilogo.AppendLine("<tr><td>Riduzione per difetti minori %</td><td class=""text-right"">" & Format(-Riepilogo.Riduzione, "##0.00") & "</td>")
        'Dati_Riepilogo.AppendLine("<tr><td>Aumento/riduzione per difetti maggiori %</td><td class=""text-right"">" & Format(Riepilogo.Variazione, "##0.00") & "</td>")
        'Dati_Riepilogo.AppendLine("<tr><td>Indice Variazione Prezzo</td><td class=""text-right"">" & Format(Riepilogo.Indice_Pomodoro, "###,###,##0.00") & "</td>")
        'Dati_Riepilogo.AppendLine("<tr><td>Giorni Premio</td><td class=""text-right"">" & Format(Riepilogo.Giorni, "##0") & "</td>")
        'Dati_Riepilogo.AppendLine("<tr><td>Premio/Ton €</td><td class=""text-right"">" & Format(Riepilogo.Premio, "###,###,##0.00") & "</td>")
        'Dati_Riepilogo.AppendLine("<tr><td>Maggiorazione €/Ton</td><td class=""text-right"">" & Format(Contratto.Maggiorazione, "###,###,##0.00") & "</td>")
        'Dati_Riepilogo.AppendLine("</table>")
        'Dati_Riepilogo.AppendLine("</div></div>")
        'Dati_Riepilogo.AppendLine("<div class=""row""><div class=""col-lg-12"">")
        'Dati_Riepilogo.AppendLine("<table class=""table table-bordered"">")
        'Dati_Riepilogo.AppendLine("<tr><th class=""active"" colspan=""2"">Importi</th></tr>")
        'Dati_Riepilogo.AppendLine("<tr><td>Prezzo Contratto €</td><td class=""text-right"">" & Format(Contratto.Prezzo, "###,###,##0.00###") & "</td>")
        'Dati_Riepilogo.AppendLine("<tr><td>Maggiorazione Totale €</td><td class=""text-right"">" & Format(Riepilogo.Maggiorazione, "###,###,##0.00") & "</td>")
        'Dati_Riepilogo.AppendLine("<tr><td>Premio Complessivo €</td><td class=""text-right"">" & Format(Riepilogo.Premio_Complessivo, "###,###,##0.00###") & "</td>")
        'Dati_Riepilogo.AppendLine("<tr><td>Prezzo Unitario Finale €</td><td class=""text-right"">" & Format(Riepilogo.Prezzo_Finale, "###,###,##0.00###") & "</td>")
        'Dati_Riepilogo.AppendLine("<tr><td>Peso a Pagamento Kg</td><td class=""text-right"">" & Format(Riepilogo.Peso_Effettivo, "###,###,##0") & "</td>")
        'Dati_Riepilogo.AppendLine("<tr><td>Prezzo Totale €</td><td class=""text-right"">" & Format(Riepilogo.Prezzo_Totale, "###,###,##0.00") & "</td>")
        'Dati_Riepilogo.AppendLine("</table>")
        'Dati_Riepilogo.AppendLine("</div></div>")

        Return Dati_Riepilogo.ToString

    End Function

End Class
