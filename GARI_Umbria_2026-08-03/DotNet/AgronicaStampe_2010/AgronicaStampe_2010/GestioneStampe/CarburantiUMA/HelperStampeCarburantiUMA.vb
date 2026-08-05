Imports AgronicaCoreDataProvider
Imports AgronicaCoreUmaDal

Public Class HelperStampeCarburantiUMA

    Private objParametri_Server As New AgronicaCoreParametri
    Private objParametri_Utenti As New AgronicaCoreParametri

    Private colonnaRimBenz As String
    Private colonnaRimGas As String
    Private colonnaRimSer As String

    Private colonnaAccBenz As String
    Private colonnaAccGas As String
    Private colonnaAccSer As String

    Public Sub New(objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri)
        If objParametri_Server Is Nothing Then
            Throw New ArgumentNullException(NameOf(objParametri_Server))
        End If

        If objParametri_Utenti Is Nothing Then
            Throw New ArgumentNullException(NameOf(objParametri_Utenti))
        End If

        Me.objParametri_Server = objParametri_Server
        Me.objParametri_Utenti = objParametri_Utenti
    End Sub

    Private Function ValorizzaSR_Inutilizzati(ByRef drRimInu As DS_GestioneRimanenzeInutilizzo.DT_GestioneRimanenzeInutilizzoRow,
                                              ByRef drRichiestaCorrente As DataRow,
                                              ByVal piva As String,
                                              ByVal codRichiestaTestata As Integer,
                                              ByVal flagRichiestaProprio As Boolean,
                                              ByVal dataStampa As Date,
                                              ByRef out_zerocarbInutilizzato As Boolean,
                                              ByRef out_nascondiTrasferimenti As Boolean,
                                              ByRef out_nascondiRestituzioni As Boolean,
                                              ByRef out_nascondiAccise As Boolean,
                                              ByVal out_causaliInutilizzoReport As String) As List(Of RiepilogoStampeCarbUMA)

        Dim listaRiepiloghiCarb As New List(Of RiepilogoStampeCarbUMA)()

        Dim benzRiepiloghi = New RiepilogoStampeCarbUMA(3, codRichiestaTestata, 0, 0, 0)
        Dim gasRiepiloghi = New RiepilogoStampeCarbUMA(2, codRichiestaTestata, 0, 0, 0)
        Dim serRiepiloghi = New RiepilogoStampeCarbUMA(8, codRichiestaTestata, 0, 0, 0)

        Dim handleTrasferimenti As New UMA_Richieste_Trasferimenti_R()
        Dim handleRestituzioni As New UMA_Richieste_Restituzioni_R()
        Dim handleCausaliInutilizzo As New UMA_Causali_R()

        Dim dtTrasferimenti = handleTrasferimenti.Leggi(piva, codRichiestaTestata, objParametri_Server)
        Dim dtRestituzioni = handleRestituzioni.Leggi(piva, codRichiestaTestata, objParametri_Server)

        Dim carbNonUtilizzatoBenz = If(IsDBNull(drRichiestaCorrente("Rim_Dich_Benzina")), 0, drRichiestaCorrente.Field(Of Double)("Rim_Dich_Benzina"))
        Dim carbNonUtilizzatoGas = If(IsDBNull(drRichiestaCorrente("Rim_Dich_Gasolio")), 0, drRichiestaCorrente.Field(Of Double)("Rim_Dich_Gasolio"))
        Dim carbNonUtilizzatoSer = If(IsDBNull(drRichiestaCorrente("Rim_Dich_Gasolio_Serra")), 0, drRichiestaCorrente.Field(Of Double)("Rim_Dich_Gasolio_Serra"))

        If carbNonUtilizzatoBenz > 0 OrElse carbNonUtilizzatoGas > 0 OrElse carbNonUtilizzatoSer > 0 Then

            out_zerocarbInutilizzato = False

            Dim listaCausaliInutilizzo As New List(Of String)()

            Dim causaliInutilizzotestata = If(IsDBNull(drRichiestaCorrente("Causale_Non_Utilizzo")), "", drRichiestaCorrente.Field(Of String)("Causale_Non_Utilizzo"))

            If causaliInutilizzotestata <> "" Then
                Dim dtCausaliInutilizzo = handleCausaliInutilizzo.Leggi(objParametri_Server)

                For Each item As String In causaliInutilizzotestata.Split("|")
                    Dim rowcausaleDes = dtCausaliInutilizzo.Select("Causale_Cod = " & item).FirstOrDefault()
                    If rowcausaleDes IsNot Nothing Then
                        listaCausaliInutilizzo.Add(rowcausaleDes("Causale_Des"))
                    End If
                Next

                If listaCausaliInutilizzo.Count > 0 Then
                    out_causaliInutilizzoReport = " a causa di " & String.Join(", ", listaCausaliInutilizzo)
                End If
            End If

            If flagRichiestaProprio = True Then
                drRimInu.InutilizzatoProprioBenzina = carbNonUtilizzatoBenz
                drRimInu.InutilizzatoProprioGasolio = carbNonUtilizzatoGas
                drRimInu.InutilizzatoProprioGasolioSerra = carbNonUtilizzatoSer

                drRimInu.InutilizzatoTerziBenzina = 0
                drRimInu.InutilizzatoTerziGasolio = 0
                drRimInu.InutilizzatoTerziGasolioSerra = 0

            Else
                drRimInu.InutilizzatoTerziBenzina = carbNonUtilizzatoBenz
                drRimInu.InutilizzatoTerziGasolio = carbNonUtilizzatoGas
                drRimInu.InutilizzatoTerziGasolioSerra = carbNonUtilizzatoSer

                drRimInu.InutilizzatoProprioBenzina = 0
                drRimInu.InutilizzatoProprioGasolio = 0
                drRimInu.InutilizzatoProprioGasolioSerra = 0
            End If

            If dtTrasferimenti.Rows.Count > 0 Then

                out_nascondiTrasferimenti = False

                Dim carbTrasferitoBenz = dtTrasferimenti.AsEnumerable().Sum(
                        Function(row)
                            Return If(IsDBNull(row(colonnaRimBenz)), 0, row.Field(Of Double)(colonnaRimBenz))
                        End Function)

                Dim carbTrasferitoGas = dtTrasferimenti.AsEnumerable().Sum(
                        Function(row)
                            Return If(IsDBNull(row(colonnaRimGas)), 0, row.Field(Of Double)(colonnaRimGas))
                        End Function)

                Dim carbTrasferitoSer = dtTrasferimenti.AsEnumerable().Sum(
                        Function(row)
                            Return If(IsDBNull(row(colonnaRimSer)), 0, row.Field(Of Double)(colonnaRimSer))
                        End Function)

                benzRiepiloghi.Trasferimenti = carbTrasferitoBenz
                gasRiepiloghi.Trasferimenti = carbTrasferitoGas
                serRiepiloghi.Trasferimenti = carbTrasferitoSer

                If flagRichiestaProprio = True Then
                    drRimInu.TrasferitoProprioBenzina = carbTrasferitoBenz
                    drRimInu.TrasferitoProprioGasolio = carbTrasferitoGas
                    drRimInu.TrasferitoProprioGasolioSerra = carbTrasferitoSer

                    drRimInu.TrasferitoTerziBenzina = 0
                    drRimInu.TrasferitoTerziGasolio = 0
                    drRimInu.TrasferitoTerziGasolioSerra = 0

                Else
                    drRimInu.TrasferitoTerziBenzina = carbTrasferitoBenz
                    drRimInu.TrasferitoTerziGasolio = carbTrasferitoGas
                    drRimInu.TrasferitoTerziGasolioSerra = carbTrasferitoSer

                    drRimInu.TrasferitoProprioBenzina = 0
                    drRimInu.TrasferitoProprioGasolio = 0
                    drRimInu.TrasferitoProprioGasolioSerra = 0
                End If
            Else
                out_nascondiTrasferimenti = True
            End If

            If dtRestituzioni.Rows.Count > 0 Then
                out_nascondiRestituzioni = False

                'Sommo perché i record sono divisi per i fornitori beneficiari del reso
                Dim qtaRestituzioniBenzina = dtRestituzioni.AsEnumerable().Sum(
                    Function(row)
                        Return If(IsDBNull(row(colonnaRimBenz)), 0, row.Field(Of Double)(colonnaRimBenz))
                    End Function)

                Dim qtaRestituzioniGasolio = dtRestituzioni.AsEnumerable().Sum(
                    Function(row)
                        Return If(IsDBNull(row(colonnaRimGas)), 0, row.Field(Of Double)(colonnaRimGas))
                    End Function)

                Dim qtaRestituzioniGasolioSerra = dtRestituzioni.AsEnumerable().Sum(
                    Function(row)
                        Return If(IsDBNull(row(colonnaRimSer)), 0, row.Field(Of Double)(colonnaRimSer))
                    End Function)

                benzRiepiloghi.Restituzioni = qtaRestituzioniBenzina
                gasRiepiloghi.Restituzioni = qtaRestituzioniGasolio
                serRiepiloghi.Restituzioni = qtaRestituzioniGasolioSerra

                If flagRichiestaProprio = True Then
                    drRimInu.RestituitoProprioBenzina = qtaRestituzioniBenzina
                    drRimInu.RestituitoProprioGasolio = qtaRestituzioniGasolio
                    drRimInu.RestituitoProprioGasolioSerra = qtaRestituzioniGasolioSerra

                    drRimInu.RestituitoTerziBenzina = 0
                    drRimInu.RestituitoTerziGasolio = 0
                    drRimInu.RestituitoTerziGasolioSerra = 0

                Else
                    drRimInu.RestituitoTerziBenzina = qtaRestituzioniBenzina
                    drRimInu.RestituitoTerziGasolio = qtaRestituzioniGasolio
                    drRimInu.RestituitoTerziGasolioSerra = qtaRestituzioniGasolioSerra

                    drRimInu.RestituitoProprioBenzina = 0
                    drRimInu.RestituitoProprioGasolio = 0
                    drRimInu.RestituitoProprioGasolioSerra = 0
                End If

            Else
                out_nascondiRestituzioni = True
            End If

            Dim carbAcciseBenz = If(IsDBNull(drRichiestaCorrente(colonnaAccBenz)), 0, drRichiestaCorrente.Field(Of Double)(colonnaAccBenz))
            Dim carbAcciseGas = If(IsDBNull(drRichiestaCorrente(colonnaAccGas)), 0, drRichiestaCorrente.Field(Of Double)(colonnaAccGas))
            Dim carbAcciseSer = If(IsDBNull(drRichiestaCorrente(colonnaAccSer)), 0, drRichiestaCorrente.Field(Of Double)(colonnaAccSer))

            If carbAcciseBenz <> 0 OrElse carbAcciseGas <> 0 OrElse carbAcciseSer <> 0 Then
                out_nascondiAccise = False

                benzRiepiloghi.Accise = carbAcciseBenz
                gasRiepiloghi.Accise = carbAcciseGas
                serRiepiloghi.Accise = carbAcciseSer

                If flagRichiestaProprio = True Then
                    drRimInu.AcciseProprioBenzina = carbAcciseBenz
                    drRimInu.AcciseProprioGasolio = carbAcciseGas
                    drRimInu.AcciseProprioGasolioSerra = carbAcciseSer

                    drRimInu.AcciseTerziBenzina = 0
                    drRimInu.AcciseTerziGasolio = 0
                    drRimInu.AcciseTerziGasolioSerra = 0

                Else
                    drRimInu.AcciseTerziBenzina = carbAcciseBenz
                    drRimInu.AcciseTerziGasolio = carbAcciseGas
                    drRimInu.AcciseTerziGasolioSerra = carbAcciseSer

                    drRimInu.AcciseProprioBenzina = 0
                    drRimInu.AcciseProprioGasolio = 0
                    drRimInu.AcciseProprioGasolioSerra = 0
                End If
            Else
                out_nascondiAccise = True
            End If

        Else
            out_zerocarbInutilizzato = True
        End If

        listaRiepiloghiCarb.Add(benzRiepiloghi)
        listaRiepiloghiCarb.Add(gasRiepiloghi)
        listaRiepiloghiCarb.Add(serRiepiloghi)

        Return listaRiepiloghiCarb

    End Function

    Public Function ValorizzaSR_Inutilizzati_Richiesta(ByRef rptRichiestaCarb As Rpt_RichiestaCarbPrevisioneLav,
                                                       ByRef dsRimanenzeInutilizzo As DS_GestioneRimanenzeInutilizzo,
                                                       ByRef drRichiestaCorrente As DataRow,
                                                       ByVal piva As String,
                                                       ByVal codRichiestaTestata As Integer,
                                                       ByVal flagRichiestaProprio As Boolean,
                                                       ByVal dataStampa As Date) As List(Of RiepilogoStampeCarbUMA)

        colonnaRimBenz = "Benzina"
        colonnaRimGas = "Gasolio"
        colonnaRimSer = "Gasolio_Serra"

        colonnaAccBenz = "Rec_Acc_Dich_Benzina"
        colonnaAccGas = "Rec_Acc_Dich_Gasolio"
        colonnaAccSer = "Rec_Acc_Dich_Gasolio_Serra"

        Dim causaliInutilizzoReport = ""

        Dim drRimInu As DS_GestioneRimanenzeInutilizzo.DT_GestioneRimanenzeInutilizzoRow = dsRimanenzeInutilizzo.DT_GestioneRimanenzeInutilizzo.NewRow

        Dim out_zerocarbInutilizzato As Boolean
        Dim out_nascondiTrasferimenti As Boolean
        Dim out_nascondiRestituzioni As Boolean
        Dim out_nascondiAccise As Boolean

        Dim listaRiepiloghiCarb = ValorizzaSR_Inutilizzati(drRimInu, drRichiestaCorrente, piva, codRichiestaTestata, flagRichiestaProprio, dataStampa, out_zerocarbInutilizzato, out_nascondiTrasferimenti, out_nascondiRestituzioni, out_nascondiAccise, causaliInutilizzoReport)

        Dim srInutilizzoSections = rptRichiestaCarb.OpenSubreport("Rpt_GestioneRimanenzeInutilizzo.rpt").ReportDefinition.Sections

        Dim prefissoCarbInut = "- "

        If out_zerocarbInutilizzato = False Then
            drRimInu.DichiarCarbInutilizzato = String.Format("{0}Che alla presente data {1} la Ditta dispone delle seguenti rimanenze di carburante agricolo inutilizzato{2}:",
                                                                        prefissoCarbInut, dataStampa.ToShortDateString, causaliInutilizzoReport)

            srInutilizzoSections.Item("HeaderSectionCarbTrasferimenti").SectionFormat.EnableSuppress = out_nascondiTrasferimenti
            srInutilizzoSections.Item("HeaderSectionCarbRestituzioni").SectionFormat.EnableSuppress = out_nascondiRestituzioni
            srInutilizzoSections.Item("HeaderSectionCarbAccise").SectionFormat.EnableSuppress = out_nascondiAccise
        Else
            drRimInu.DichiarCarbInutilizzato = String.Format("{0}Che alla presente data {1} la Ditta non dispone di rimanenze di carburante agricolo inutilizzato",
                                                             prefissoCarbInut, dataStampa.ToShortDateString)

            srInutilizzoSections.Item("HeaderSectionCarbInutilizzato").SectionFormat.EnableSuppress = True
            srInutilizzoSections.Item("HeaderSectionCarbTrasferimenti").SectionFormat.EnableSuppress = True
            srInutilizzoSections.Item("HeaderSectionCarbRestituzioni").SectionFormat.EnableSuppress = True
            srInutilizzoSections.Item("HeaderSectionCarbAccise").SectionFormat.EnableSuppress = True
        End If

        drRimInu.DichiarCarbTrasferimenti = "- Di cui trasferite ad altre aziende:"
        drRimInu.DichiarCarbRestituzioni = "- Di cui restituite ai distributori:"
        drRimInu.DichiarCarbAccise = "- Di cui soggette a recupero accise:"

        dsRimanenzeInutilizzo.DT_GestioneRimanenzeInutilizzo.Rows.Add(drRimInu)

        Return listaRiepiloghiCarb

    End Function

    Public Function ValorizzaSR_Inutilizzati_IstruttoriaRich(ByRef rptIstruttoriaRichCarb As Rpt_VerbaleIstruttoriaRichCarb,
                                                             ByRef dsRimanenzeInutilizzo As DS_GestioneRimanenzeInutilizzo,
                                                             ByRef drRichiestaCorrente As DataRow,
                                                             ByVal piva As String,
                                                             ByVal codRichiestaTestata As Integer,
                                                             ByVal flagRichiestaProprio As Boolean,
                                                             ByVal dataStampa As Date) As List(Of RiepilogoStampeCarbUMA)

        colonnaRimBenz = "Confermato_Benzina"
        colonnaRimGas = "Confermato_Gasolio"
        colonnaRimSer = "Confermato_Gasolio_Serra"

        colonnaAccBenz = "Rec_Acc_Conf_Benzina"
        colonnaAccGas = "Rec_Acc_Conf_Gasolio"
        colonnaAccSer = "Rec_Acc_Conf_Gasolio_Serra"

        Dim causaliInutilizzoReport = ""

        Dim drRimInu As DS_GestioneRimanenzeInutilizzo.DT_GestioneRimanenzeInutilizzoRow = dsRimanenzeInutilizzo.DT_GestioneRimanenzeInutilizzo.NewRow

        Dim out_zerocarbInutilizzato As Boolean
        Dim out_nascondiTrasferimenti As Boolean
        Dim out_nascondiRestituzioni As Boolean
        Dim out_nascondiAccise As Boolean

        Dim listaRiepiloghiCarb = ValorizzaSR_Inutilizzati(drRimInu, drRichiestaCorrente, piva, codRichiestaTestata,
            flagRichiestaProprio, dataStampa, out_zerocarbInutilizzato, out_nascondiTrasferimenti,
            out_nascondiRestituzioni, out_nascondiAccise, causaliInutilizzoReport)

        Dim prefissoCarbInut = "6) "

        Dim srInutilizzoSections = rptIstruttoriaRichCarb.OpenSubreport("Rpt_GestioneRimanenzeInutilizzo.rpt").ReportDefinition.Sections

        If out_zerocarbInutilizzato = False Then
            drRimInu.DichiarCarbInutilizzato = String.Format("{0}La Ditta alla presente data {1} dispone delle seguenti rimanenze di carburante agricolo inutilizzato{2}:",
                                                                        prefissoCarbInut, dataStampa.ToShortDateString, causaliInutilizzoReport)

            srInutilizzoSections.Item("HeaderSectionCarbTrasferimenti").SectionFormat.EnableSuppress = out_nascondiTrasferimenti
            srInutilizzoSections.Item("HeaderSectionCarbRestituzioni").SectionFormat.EnableSuppress = out_nascondiRestituzioni
            srInutilizzoSections.Item("HeaderSectionCarbAccise").SectionFormat.EnableSuppress = out_nascondiAccise
        Else
            drRimInu.DichiarCarbInutilizzato = String.Format("{0}La Ditta alla presente data {1} non dispone di rimanenze di carburante agricolo inutilizzato",
                                                             prefissoCarbInut, dataStampa.ToShortDateString)

            srInutilizzoSections.Item("HeaderSectionCarbInutilizzato").SectionFormat.EnableSuppress = True
            srInutilizzoSections.Item("HeaderSectionCarbTrasferimenti").SectionFormat.EnableSuppress = True
            srInutilizzoSections.Item("HeaderSectionCarbRestituzioni").SectionFormat.EnableSuppress = True
            srInutilizzoSections.Item("HeaderSectionCarbAccise").SectionFormat.EnableSuppress = True
        End If

        drRimInu.DichiarCarbTrasferimenti = "- Di cui se ne conferma il trasferimento presso altre aziende per i seguenti quantitativi:"
        drRimInu.DichiarCarbRestituzioni = "- Di cui se ne conferma la restituzione presso i distributori per i seguenti quantitativi:"
        drRimInu.DichiarCarbAccise = "- Di cui soggette a recupero accise:"

        dsRimanenzeInutilizzo.DT_GestioneRimanenzeInutilizzo.Rows.Add(drRimInu)

        Return listaRiepiloghiCarb

    End Function

    Public Function OttieniDescrizioneFascicolo_o_PCG(ByVal fascicoloCod As Integer, ByVal fascicoloDes As String, ByVal richiestaCod As Integer, ByVal gruppoCod As String) As String

        Dim descr As String

        Select Case fascicoloCod
            Case 0
                descr = ""
            Case -1
                descr = "Coltura non legata a fascicolo"
            Case -2
                descr = "Anticipi"
            Case -3
                descr = "Trasferimenti"
            Case -4
                descr = "Piano Colturale"
                'Dal 2025 le richieste non saranno più collegate ad un fascicolo, di conseguenza il caso standard risulta la costante -4.
                'Come prima versione stampiamo questa dicitura fissa nelle stampe, nel caso per sviluppi futuri occorra mostrare 
                'gli effettivi impianti collegati alla richiesta/rendicontazione occorrerà probabilmente modificare anche il layout del report
                'perché si tratta di più informazioni rispetto alla descrizione semplice del fascicolo; nel caso, verificare se la query seguente
                'estrae già tutte le informazioni descrittive necessarie degli impianti.
                'Dim handleRichieste As New UMA_Richieste_R
                'If gruppoCod <> "" Then
                '    Dim dtPCG = handleRichieste.Leggi_Richiesta_X_Reg_Impianti("", richiestaCod, gruppoCod, 0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, Me.objParametri_Server)
                '    If dtPCG.Rows.Count > 0 Then
                '    End If
                'End If
            Case Else
                descr = fascicoloDes
        End Select

        Return descr

    End Function

End Class

Public Class RiepilogoStampeCarbUMA
    Public Property Tipo As Integer
    Public Property RichiestaCod As Integer
    Public Property Trasferimenti As Double
    Public Property Restituzioni As Double
    Public Property Accise As Double

    Public Sub New()
        Me.Tipo = 0
        Me.RichiestaCod = 0
        Me.Trasferimenti = 0
        Me.Restituzioni = 0
        Me.Accise = 0
    End Sub

    Public Sub New(tipo As Integer, richiestaCod As Integer, trasferimenti As Double, restituzioni As Double, accise As Double)
        Me.Tipo = tipo
        Me.RichiestaCod = richiestaCod
        Me.Trasferimenti = trasferimenti
        Me.Restituzioni = restituzioni
        Me.Accise = accise
    End Sub
End Class
