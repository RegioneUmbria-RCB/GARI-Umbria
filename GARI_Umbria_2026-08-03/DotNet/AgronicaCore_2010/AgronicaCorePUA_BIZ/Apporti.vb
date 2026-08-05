Public Class PUA_Apporti_BIZ

    Public Function FertilizzanteUsatoAllaData(ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                           ByVal DataPartenza As Date,
                                           ByVal DataIntervento As Date,
                                           ByVal Fer_Cod As String) As Decimal

        Dim objRecuperaDati As New AgronicaCorePUA_DAL.PUA_Apporti_DAL

        Dim risp As Decimal = objRecuperaDati.ApportoDistribuitoxData(objParametri, DataPartenza, DataIntervento, Fer_Cod)

        Return risp

    End Function

    Public Function StoccaggioDisponibileAllaDataEffluente(ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                  ByVal PUA_Cod As Integer,
                                                  ByVal Regolamento_Cod As Integer,
                                                  ByVal Eff_Cod As String,
                                                  ByVal DataPartenza As Date,
                                                  ByVal DataIntervento As Date) As Decimal

        Dim risp As Decimal = 0

        If Eff_Cod <> 0 Then

            Dim Capacita As Decimal = EffluentiTotale(objParametri, PUA_Cod, Regolamento_Cod, "Carico", Eff_Cod)
            Dim CapacitaAlTerminePeriodoDivieto_Dichiarata As Decimal = EffluentiTotale(objParametri, PUA_Cod, Regolamento_Cod, "Riempimento", Eff_Cod)

            Dim CapacitaAlTerminePeriodoDivieto As Decimal = CapacitaAlTerminePeriodoDivieto_Dichiarata

            Dim RicaricaGiornaliera As Decimal = RicaricaGiornalieraCalcola(Capacita)

            Dim NrGiorni As Integer = NrGiorni_from_data(DataPartenza, DataIntervento)

            risp = NrGiorni * RicaricaGiornaliera + CapacitaAlTerminePeriodoDivieto

        End If

        Return risp

    End Function

    Public Function StoccaggioDisponibileAllaDataEffluente(ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                  ByVal PUA_Cod As Integer,
                                                  ByVal Regolamento_Cod As Integer,
                                                  ByVal Eff_Cod As String,
                                                  ByVal DataPartenza As Date, ByVal DataIntervento As Date,
                                                    ByRef Capacita As Decimal, ByRef CapacitaAlTerminePeriodoDivieto As Decimal,
                                                    ByRef RicaricaGiornaliera As Decimal, ByRef NrGiorni As Integer) As Decimal

        Dim risp As Decimal = 0

        If Eff_Cod <> 0 Then

            Capacita = EffluentiTotale(objParametri, PUA_Cod, Regolamento_Cod, "Carico", Eff_Cod)
            CapacitaAlTerminePeriodoDivieto = EffluentiTotale(objParametri, PUA_Cod, Regolamento_Cod, "Riempimento", Eff_Cod)

            RicaricaGiornaliera = RicaricaGiornalieraCalcola(Capacita)

            NrGiorni = NrGiorni_from_data(DataPartenza, DataIntervento)

            risp = NrGiorni * RicaricaGiornaliera + CapacitaAlTerminePeriodoDivieto

        End If

        Return risp

    End Function

    Public Function EffluentiTotale(ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                    ByVal PUA_Cod As Integer,
                                    ByVal Regolamento_Cod As Integer,
                                    ByVal nomeCod As String,
                                    ByVal sEff_Cod As String) As Decimal

        Dim risp As Decimal = -1.0
        Dim objRecupero As New AgronicaCorePUA_DAL.PUA_Apporti_DAL

        Try
            risp = objRecupero.RecuperaEffluentiTotali(objParametri, nomeCod, PUA_Cod, Regolamento_Cod, sEff_Cod)

        Catch ex As Exception
            risp = -1.0
        End Try

        Return risp

    End Function

    Public Function RicaricaGiornalieraCalcola(ByVal TotaleAnno As Decimal) As Decimal

        Return TotaleAnno / 365

    End Function

    Private Function NrGiorni_from_data(ByVal DataPartenza As Date, ByVal DataIntervento As Date) As Integer

        Return If(DataIntervento > DataPartenza, DataIntervento.Subtract(DataPartenza).Days, -1)

    End Function

    Public Function FabbisognoAzotoSoddisfatto(ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                               ByVal Piva As String, ByVal Sa_Cod As Integer, ByVal Appezza As Integer, ByVal Id_Reg As Integer, ByVal Progetto_Cod As Integer,
                                               ByVal Id_Fert_escluso As Integer) As Decimal


        Dim objRecuperaDati As New AgronicaCorePUA_DAL.PUA_Apporti_DAL

        Dim Risp As Double

        Try

            Risp = objRecuperaDati.NUtile_Distribuito(objParametri,
                                                Piva, Sa_Cod, Appezza, Id_Reg, Progetto_Cod,
                                                Id_Fert_escluso)

        Catch ex As Exception

            Risp = -1.0

        End Try

        Return Risp

    End Function


    Public Function CaricaDisponibilitaAttuale(ByVal Fer_Cod As Integer, ByVal Eff_Cod As Integer, ByVal PUA_Cod As Integer, ByVal Regolamento_Cod As Integer, ByVal DataIntervento As Date,
                                               ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Decimal

        Dim DataPartenza As Date = CDate("01/02/" + DataIntervento.Year.ToString)

        Dim objApporti As New AgronicaCorePUA_BIZ.PUA_Apporti_BIZ
        Dim FertilizzanteUsato As Decimal = objApporti.FertilizzanteUsatoAllaData(objParametri_Server, DataPartenza, DataIntervento, Fer_Cod)


        'If Qs_TipoOperazione = TipiEnumerativi.enum_TipoOperazioneDB.Modifica AndAlso FertilizzanteUsato > 0.0 Then
        '    If IsNumeric(lblDisp_apporti_xappezza.Text) Then
        '        FertilizzanteUsato -= CDbl(lblDisp_apporti_xappezza.Text)
        '    End If
        'End If

        Dim StoccaggioPrevistoInData As Decimal = objApporti.StoccaggioDisponibileAllaDataEffluente(
                                                objParametri_Server,
                                                PUA_Cod,
                                                Regolamento_Cod,
                                                Eff_Cod,
                                                DataPartenza,
                                                DataIntervento)

        Dim Disponibilita As Decimal = AgronicaCoreDataProvider.Agro_Math.ArrotondaVal_4(StoccaggioPrevistoInData - FertilizzanteUsato)

        Return Disponibilita

    End Function

End Class
