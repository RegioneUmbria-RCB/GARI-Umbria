Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.Utilityprovider
Imports AgronicaCorePianoConcimazioneBIZ

Public Class PUA_Fabbisogni

    Public Sub New()

    End Sub

    Public Function FabbisognixTipoPUA(ByVal FabbisognoInput As PUA_Fabbisogni_input,
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) _
                                            As PUA_Fabbisogni_output

        Dim Output As New PUA_Fabbisogni_output

        Dim N As Decimal = 0

        Dim Nr As Decimal 'Nr è l'azoto messo a disposizione dalla coltura precedente
        Dim Na As Decimal 'Na azoto atmosferico
        Dim Ns As Decimal 'Ns azoto mineralizzato da precedenti concimazioni organiche
        Dim Nm As Decimal 'Nm annuo mineralizzazione materia organica suolo
        Dim Ni As Decimal 'Ni da acqua irrigazione


        Try

            'Nr è l'azoto messo a disposizione dalla coltura precedente
            Dim Precessione_input As New PianoConcimazione_Precessione_input
            Precessione_input.Regolamento_Cod = FabbisognoInput.Regolamento_Cod
            Precessione_input.PUA_Tipo = FabbisognoInput.PUA_Tipo
            Dim Precessione_output As New PianoConcimazione_Precessione_output
            Dim ObjPrecessione As New PianoConcimazione_Precessione
            Precessione_output = ObjPrecessione.Precessione(Precessione_input, objParametri)

            'Nm azoto mineralizzato da precedenti concimazioni organiche
            Dim EffluentiXFrequenza_input As New PUA_EffluentiXFrequenza_input
            EffluentiXFrequenza_input.Regolamento_Cod = FabbisognoInput.Regolamento_Cod
            Dim EffluentiXFrequenza_output As New PUA_EffluentiXFrequenza_output
            Dim ObjEffluentiXFrequenza As New PUA_Effluenti
            EffluentiXFrequenza_output = ObjEffluentiXFrequenza.EffluentiXFrequenza(EffluentiXFrequenza_input, objParametri)

            'recupero i coeff di assorbimento e classe tessitura
            Dim DtAss As New DataTable
            Dim DrAss() As DataRow
            Dim DtTess As New DataTable
            Dim DrTess() As DataRow

            Dim Ubicazione_output As PianoConcimazione_Ubicazione_output
            Dim TipoAcqua_output As PianoConcimazione_TipoAcqua_output

            If FabbisognoInput.PUA_Tipo = enum_PUA_Tipo.Completo Then

                Dim objAss As New AgronicaCoreMetaSchemaDAL.GruppoFinalitaxSpeciexEpoca_R
                DtAss = objAss.Leggi(FabbisognoInput.Regolamento_Cod, 0, 0, 0, 0, "", "", objParametri)
                Dim objTess As New AgronicaCoreMetaSchemaDAL.PC_Tessiture_R
                DtTess = objTess.Leggi_IdGruppoTessitura_Da_SabbiaArgilla_DT(-99, -99, objParametri)

                'Na azoto atmosferico
                Dim Ubicazione_input As New PianoConcimazione_Ubicazione_input
                Ubicazione_input.Regolamento_Cod = FabbisognoInput.Regolamento_Cod
                Ubicazione_output = New PianoConcimazione_Ubicazione_output
                Dim ObjUbicazione As New PianoConcimazione_Ubicazione
                Ubicazione_output = ObjUbicazione.Ubicazione(Ubicazione_input, objParametri)

                'Ni è l'azoto messo a disposizione dall'acqua utilizzata per l'irrigazione (es zvn acqua sotterranea = 10)
                Dim TipoAcqua_input As New PianoConcimazione_TipoAcqua_input
                TipoAcqua_input.Regolamento_Cod = FabbisognoInput.Regolamento_Cod
                TipoAcqua_output = New PianoConcimazione_TipoAcqua_output
                Dim ObjTipoAcqua As New PianoConcimazione_TipoAcqua
                TipoAcqua_output = ObjTipoAcqua.TipoAcqua(TipoAcqua_input, objParametri)

            End If

            '(16/10/2019 fede) aggiunta parametrizzazione in base al regolamento
            Dim objPar As New AgronicaCoreMetaSchemaDAL.PUA_ParametrixRegolamenti_R
            Dim DtPar As DataTable = objPar.Leggi(0, FabbisognoInput.Regolamento_Cod, "", "", objParametri)
            Dim Considera_CoefficienteTempo_CalcoloFabbisognoN_Nm As Boolean = False ' = 8
            Dim Considera_CoefficienteTempo_CalcoloFabbisognoN_Na As Boolean = False '= 9
            Dim Considera_CoefficienteTempo_CalcoloFabbisognoN_Ns As Boolean = False '= 10
            Dim Leggi_Nm_DaTabella As Boolean = False '=11
            If Not DtPar Is Nothing AndAlso DtPar.Rows.Count > 0 Then
                Dim DrCTNm() As DataRow = DtPar.Select("parametro_cod=" & enum_PUAParametri.Considera_CoefficienteTempo_CalcoloFabbisognoN_Nm)
                If Not DrCTNm Is Nothing AndAlso DrCTNm.Length > 0 AndAlso Not IsDBNull(DrCTNm(0).Item("parametro_valore")) AndAlso IsNumeric(DrCTNm(0).Item("parametro_valore")) AndAlso CInt(DrCTNm(0).Item("parametro_valore")) = 1 Then
                    Considera_CoefficienteTempo_CalcoloFabbisognoN_Nm = True
                End If
                Dim DrCTNa() As DataRow = DtPar.Select("parametro_cod=" & enum_PUAParametri.Considera_CoefficienteTempo_CalcoloFabbisognoN_Na)
                If Not DrCTNa Is Nothing AndAlso DrCTNa.Length > 0 AndAlso Not IsDBNull(DrCTNa(0).Item("parametro_valore")) AndAlso IsNumeric(DrCTNa(0).Item("parametro_valore")) AndAlso CInt(DrCTNa(0).Item("parametro_valore")) = 1 Then
                    Considera_CoefficienteTempo_CalcoloFabbisognoN_Na = True
                End If
                Dim DrCTNs() As DataRow = DtPar.Select("parametro_cod=" & enum_PUAParametri.Considera_CoefficienteTempo_CalcoloFabbisognoN_Ns)
                If Not DrCTNs Is Nothing AndAlso DrCTNs.Length > 0 AndAlso Not IsDBNull(DrCTNs(0).Item("parametro_valore")) AndAlso IsNumeric(DrCTNs(0).Item("parametro_valore")) AndAlso CInt(DrCTNs(0).Item("parametro_valore")) = 1 Then
                    Considera_CoefficienteTempo_CalcoloFabbisognoN_Ns = True
                End If
                Dim DrNmTab() As DataRow = DtPar.Select("parametro_cod=" & enum_PUAParametri.Leggi_Nm_DaTabella)
                If Not DrNmTab Is Nothing AndAlso DrNmTab.Length > 0 AndAlso Not IsDBNull(DrNmTab(0).Item("parametro_valore")) AndAlso IsNumeric(DrNmTab(0).Item("parametro_valore")) AndAlso CInt(DrNmTab(0).Item("parametro_valore")) = 1 Then
                    Leggi_Nm_DaTabella = True
                End If
            End If


            For Each app As PUA_Appezzamento In FabbisognoInput.PUA_ListaAppezzamenti

                Nr = 0
                Na = 0
                Ni = 0
                Ns = 0
                Nm = 0

                If IsNothing(app.Grfi_Cod_Concimazione) Then
                    app.Grfi_Cod_Concimazione = 0
                End If
                If IsNothing(app.Grfi_Des_Concimazione) Then
                    app.Grfi_Des_Concimazione = ""
                End If
                If IsNothing(app.B_Perc) Then
                    app.B_Perc = 0
                End If
                If IsNothing(app.N_FertilizzazioniPrecedenti) Then
                    app.N_FertilizzazioniPrecedenti = 0
                End If

                'Nr è l'azoto messo a disposizione dalla coltura precedente
                If Not Precessione_output Is Nothing AndAlso Precessione_output.ListaPrecessione.Count > 0 Then
                    Dim Precessione As New Precessione
                    Precessione = Precessione_output.ListaPrecessione.Where(Function(x) x.Codice = app.PrecessioneCod)(0)
                    If Not Precessione Is Nothing Then
                        Nr = Precessione.N_Residuo
                    End If
                End If

                'Ns azoto mineralizzato da precedenti concimazioni organiche
                If app.N_FertilizzazioniPrecedenti > 0 Then
                    Ns = app.N_FertilizzazioniPrecedenti
                Else
                    If app.N_Distribuito > 0 Then
                        Dim N_Riduz As Decimal = 0
                        EffluentiXFrequenza_output = ObjEffluentiXFrequenza.EffluentiXFrequenza(EffluentiXFrequenza_input, objParametri)
                        If Not EffluentiXFrequenza_output Is Nothing AndAlso EffluentiXFrequenza_output.ListaEffluentiXFrequenza.Count > 0 Then
                            Dim EffluentiXFrequenza As New PUA_EffluentiXFrequenza
                            EffluentiXFrequenza = EffluentiXFrequenza_output.ListaEffluentiXFrequenza.Where(Function(x) x.Eff_Cod = app.FertOrganicoCod And x.Id_Fre = app.FrequenzaCod)(0)
                            If Not EffluentiXFrequenza Is Nothing Then
                                N_Riduz = EffluentiXFrequenza.N
                            End If
                        End If
                        If N_Riduz > 0 Then
                            Ns = N_Riduz * app.N_Distribuito
                        End If
                    End If
                End If

                If FabbisognoInput.PUA_Tipo = enum_PUA_Tipo.Completo Then

                    'Na azoto atmosferico
                    If Not Ubicazione_output Is Nothing AndAlso Ubicazione_output.ListaUbicazione.Count > 0 Then
                        Dim Ubicazione As New Ubicazione
                        Ubicazione = Ubicazione_output.ListaUbicazione.Where(Function(x) x.Codice = app.UbicazioneCod)(0)
                        If Not Ubicazione Is Nothing Then
                            Na = Ubicazione.Deposizione_Anno
                        End If
                    End If


                    'Ni è l'azoto messo a disposizione dall'acqua utilizzata per l'irrigazione (es zvn acqua sotterranea = 10)
                    If Not TipoAcqua_output Is Nothing AndAlso TipoAcqua_output.ListaTipiAcqua.Count > 0 Then
                        Dim TipoAcqua As New TipoAcqua
                        Dim TipoZona As String = "n"
                        Select Case app.ZVN
                            Case True
                                TipoZona = "v"
                        End Select
                        TipoAcqua = TipoAcqua_output.ListaTipiAcqua.Where(Function(x) x.Codice = app.TipoAcquaCod And x.TipoZona = TipoZona)(0)
                        If Not TipoAcqua Is Nothing Then
                            Ni = TipoAcqua.N
                        End If
                    End If

                    Dim coltura As New PUA_Coltura

                    If Not DtAss Is Nothing AndAlso DtAss.Rows.Count > 0 Then
                        If app.Grfi_Cod_Concimazione > 0 Then
                            DrAss = DtAss.Select("veg_cod=" & app.Veg_Cod & " and grfi_cod_pua=" & app.Grfi_Cod_Concimazione)
                        Else
                            DrAss = DtAss.Select("veg_cod=" & app.Veg_Cod & " and grfi_cod_gias=" & app.Grfi_Cod)
                        End If

                        If Not DrAss Is Nothing AndAlso DrAss.Length > 0 Then
                            If app.B_Perc > 0 Then
                                coltura.b_perc = app.B_Perc
                            Else
                                coltura.b_perc = DrAss(0).Item("b_perc")
                                app.B_Perc = DrAss(0).Item("b_perc")
                                app.Grfi_Cod_Concimazione = DrAss(0).Item("grfi_cod_pua")
                            End If
                            'coltura.b_perc = DrAss(0).Item("b_perc")
                            coltura.base = DrAss(0).Item("base")
                            coltura.bilancio_completo = DrAss(0).Item("bilancio_completo")
                            coltura.coeff_incr = DrAss(0).Item("coeff_incr")
                            coltura.coeff_temp = DrAss(0).Item("coeff_temp")
                        End If
                    End If

                    '(Y*b) coeff asportazione
                    app.Assorbimento = Assorbimento(app.Resa, coltura)

                    'Nm annuo mineralizzazione materia organica suolo

                    '(17/09/2019 fede)
                    'sostituita formula utilizzata nel foglio di carnevali pua 2008
                    'Nm = (app.So / 100) * peso20 * 1000 * (minimo / 100) * 0.05

                    If Not DtTess Is Nothing AndAlso DtTess.Rows.Count > 0 Then
                        DrTess = DtTess.Select("argilla=" & Agro_SQL_SaveNum(AgronicaCoreDataProvider.Agro_Math.ArrotondaVal_0(app.Argilla)) & " and sabbia=" & Agro_SQL_SaveNum(AgronicaCoreDataProvider.Agro_Math.ArrotondaVal_0(app.Sabbia)))
                        If Not DrTess Is Nothing AndAlso DrTess.Length > 0 Then
                            'reg umbria letto in tabella
                            If Leggi_Nm_DaTabella = True Then
                                Dim Id_GruppoTessitura As Integer = DrTess(0).Item("Id_GruppoTessitura")
                                Dim objPC_GrigliaSO As New AgronicaCoreMetaSchemaDAL.PC_GrigliaSO_R
                                Dim Id_Dotazione As Integer = objPC_GrigliaSO.Leggi_Dotazione(FabbisognoInput.Regolamento_Cod, app.So, Id_GruppoTessitura, "", "", objParametri)
                                Dim objPC_NMinerazizzatoSOSuolo As New AgronicaCoreMetaSchemaDAL.PC_NMinerazizzatoSOSuolo_R
                                Nm = objPC_NMinerazizzatoSOSuolo.Leggi_N(FabbisognoInput.Regolamento_Cod, Id_Dotazione, Id_GruppoTessitura, "", "", objParametri)
                            Else
                                'caso vecchio calcolo carnevali
                                Dim peso20 As Decimal = 0
                                Dim minimo As Decimal = 0
                                Dim Id_ClasseTessitura As Integer = DrTess(0).Item("Id_ClasseTessitura")
                                Dim objTess1 As New AgronicaCoreMetaSchemaDAL.ClassiTessituraB_R
                                Dim DtTess1 As DataTable
                                DtTess1 = objTess1.LeggiDistinct(Id_ClasseTessitura, FabbisognoInput.Regolamento_Cod, "", "", objParametri)
                                If Not DtTess1 Is Nothing AndAlso DtTess1.Rows.Count > 0 Then
                                    If Not IsDBNull(DtTess1.Rows(0).Item("peso20")) AndAlso IsNumeric(DtTess1.Rows(0).Item("peso20")) Then
                                        peso20 = DtTess1.Rows(0).Item("peso20")
                                    End If
                                    If Not IsDBNull(DtTess1.Rows(0).Item("minimo")) AndAlso IsNumeric(DtTess1.Rows(0).Item("minimo")) Then
                                        minimo = DtTess1.Rows(0).Item("minimo")
                                    End If
                                End If
                                Nm = (app.So / 100) * peso20 * 1000 * (minimo / 100) * 0.05
                            End If
                        End If
                    End If

                    N = PUA_FabbisognoAzoto(FabbisognoInput.Regolamento_Cod,
                                            coltura, app.Resa, app.Ciclo,
                                            Nr, Na, Ns, Nm, Ni,
                                            Considera_CoefficienteTempo_CalcoloFabbisognoN_Nm, Considera_CoefficienteTempo_CalcoloFabbisognoN_Na, Considera_CoefficienteTempo_CalcoloFabbisognoN_Ns)
                Else

                    Dim valoreMax As Boolean = True
                    Dim anno As Integer

                    If FabbisognoInput.Regolamento_Cod = 1 Then

                        'caso particolare pomodoro
                        If app.Veg_Cod = 52 Then
                            If app.ValiditaInizio > CDate("31/10/" + app.ValiditaInizio.Year.ToString) Then
                                anno = app.ValiditaInizio.Year + 1
                            Else
                                anno = app.ValiditaInizio.Year
                            End If

                            If app.ValiditaInizio > CDate("05/05/" + anno.ToString) Then
                                valoreMax = False
                            Else
                                valoreMax = True
                            End If
                        Else
                            valoreMax = True
                        End If

                    Else

                        valoreMax = Nothing

                    End If

                    Dim objLimitiAzotoxSpecie As New AgronicaCoreMetaSchemaDAL.LimitiAzotoxSpecie_R
                    Dim ResaRif As Decimal
                    Dim FattoreCorrettivo_N As Decimal

                    N = objLimitiAzotoxSpecie.RecuperaAzotoResaFromVegCod_Regolamento(app.Veg_Cod, app.Grfi_Cod_Concimazione, app.StatoImpiantoCod,
                                                                                      valoreMax,
                                                                                      FabbisognoInput.Regolamento_Cod,
                                                                                          ResaRif, FattoreCorrettivo_N,
                                                                                          objParametri)
                    If N <> -1 Then

                        'sottraggo l'eventuale N da precessione
                        N -= Nr

                        'sottraggo l'eventuale N residuo da fertilizzazione precedente
                        N -= Ns

                        'aggiunta considerazione dell'eventuale fattore correttivo per resa maggiore
                        If FattoreCorrettivo_N > 0 AndAlso app.Resa > ResaRif Then
                            N += (app.Resa - ResaRif) * FattoreCorrettivo_N
                        End If

                    End If

                End If

                app.N_Fabbisogno = N

                If N > 0 Then
                    app.N_FabbisognoComplessivo = N * app.Superficie
                End If

            Next

            Output.PUA_ListaAppezzamenti = FabbisognoInput.PUA_ListaAppezzamenti
            Output.N_Fabbisogno = N

        Catch ex As Exception
            'Fedeeeee!!!!!!!
            Output.MessaggioErrore = ex.Message
            Output.PUA_ListaAppezzamenti = FabbisognoInput.PUA_ListaAppezzamenti
        End Try

        Return Output

    End Function


    Public Function PUA_FabbisognoAzoto(ByVal Regolamento_Cod As Integer,
                                        ByVal coltura As PUA_Coltura,
                                            ByVal Resa As Decimal,
                                            ByVal Ciclo As enum_Appezzamento_Pua_Ciclo,
                                                ByVal Nr As Decimal, ByVal Na As Decimal, ByVal Ns As Decimal, ByVal Nm As Decimal, ByVal Ni As Decimal,
                                                    ByVal Considera_CoefficienteTempo_CalcoloFabbisognoN_Nm As Boolean, ByVal Considera_CoefficienteTempo_CalcoloFabbisognoN_Na As Boolean, ByVal Considera_CoefficienteTempo_CalcoloFabbisognoN_Ns As Boolean
                                                ) As Decimal

        Dim Risp As Decimal

        Try



            Risp = Fabbisogno_Netto(Resa, Ciclo, coltura, Nr, Na, Ns, Nm, Ni, Considera_CoefficienteTempo_CalcoloFabbisognoN_Nm, Considera_CoefficienteTempo_CalcoloFabbisognoN_Na, Considera_CoefficienteTempo_CalcoloFabbisognoN_Ns)

            If Risp < 0 Then
                Risp = 0
            End If

        Catch ex As Exception

            Risp = -1.0

        End Try

        Return Risp

    End Function

    Public Function Fabbisogno_Netto(ByVal resa As Decimal, ByVal ciclo As enum_Appezzamento_Pua_Ciclo, ByVal coltura As PUA_Coltura, ByVal Nr As Decimal, ByVal Na As Decimal, ByVal Ns As Decimal, ByVal Nm As Decimal, ByVal Ni As Decimal,
                                                                                         ByVal Considera_CoefficienteTempo_CalcoloFabbisognoN_Nm As Boolean, ByVal Considera_CoefficienteTempo_CalcoloFabbisognoN_Na As Boolean, ByVal Considera_CoefficienteTempo_CalcoloFabbisognoN_Ns As Boolean) As Decimal

        Return Assorbimento(resa, coltura) - (N_Utile_Da_Conteggiare(ciclo, coltura, Nm, Considera_CoefficienteTempo_CalcoloFabbisognoN_Nm) + N_Utile_Da_Conteggiare_atm(ciclo, coltura, Na, Considera_CoefficienteTempo_CalcoloFabbisognoN_Na) + N_utilexColtura(coltura, Ns, Considera_CoefficienteTempo_CalcoloFabbisognoN_Ns) + Nr + Ni)

    End Function

    Public Function Assorbimento(ByVal Resa As Decimal, ByVal coltura As PUA_Coltura) As Decimal

        Dim val As Decimal

        If Resa > 0.0 Then
            val = ((coltura.b_perc * 10 * Resa) + coltura.base) * coltura.coeff_incr
        Else
            val = 0.0
        End If

        Return val

    End Function

    Public Function N_Utile_Da_Conteggiare(ByVal ciclo As enum_Appezzamento_Pua_Ciclo, ByVal Coltura As PUA_Coltura, ByVal Nm As Decimal, ByVal Considera_CoefficienteTempo_CalcoloFabbisognoN_Nm As Boolean) As Decimal

        If ciclo = enum_Appezzamento_Pua_Ciclo.Principale Then
            Return N_Utile_Calcolato(Coltura, Nm, Considera_CoefficienteTempo_CalcoloFabbisognoN_Nm)
        Else
            Return 0
        End If
    End Function

    Public Function N_Utile_Da_Conteggiare_atm(ByVal ciclo As enum_Appezzamento_Pua_Ciclo, ByVal coltura As PUA_Coltura, ByVal Na As Decimal, ByVal Considera_CoefficienteTempo_CalcoloFabbisognoN_Na As Boolean) As Decimal

        If ciclo = enum_Appezzamento_Pua_Ciclo.Principale Then
            Return N_Utile_Calcolato_atm(coltura, Na, Considera_CoefficienteTempo_CalcoloFabbisognoN_Na)
        Else
            Return 0
        End If

    End Function

    Public Function N_Utile_Calcolato(ByVal coltura As PUA_Coltura, ByVal Nm As Decimal, ByVal Considera_CoefficienteTempo_CalcoloFabbisognoN_Nm As Boolean) As Decimal

        Select Case Considera_CoefficienteTempo_CalcoloFabbisognoN_Nm
            Case True
                Return Nm * coltura.coeff_temp * coltura.bilancio_completo
            Case Else
                Return Nm * coltura.bilancio_completo
        End Select


    End Function

    Public Function N_Utile_Calcolato_atm(ByVal coltura As PUA_Coltura, ByVal Na As Decimal, ByVal Considera_CoefficienteTempo_CalcoloFabbisognoN_Na As Boolean) As Decimal

        Select Case Considera_CoefficienteTempo_CalcoloFabbisognoN_Na
            Case True
                Return Na * coltura.coeff_temp * coltura.bilancio_completo
            Case Else
                Return Na * coltura.bilancio_completo
        End Select

    End Function

    Private Function N_utilexColtura(ByVal coltura As PUA_Coltura, ByVal Ns As Decimal, ByVal Considera_CoefficienteTempo_CalcoloFabbisognoN_Ns As Boolean) As Decimal

        Select Case Considera_CoefficienteTempo_CalcoloFabbisognoN_Ns
            Case True
                Return Ns * coltura.coeff_temp
            Case Else
                Return Ns
        End Select

    End Function

End Class


Public Class PUA_Fabbisogni_input

    Public Regolamento_Cod As Integer
    Public PUA_Tipo As enum_PUA_Tipo

    Public Url As String

    Public PUA_ListaAppezzamenti As List(Of PUA_Appezzamento)

    Sub New()
        Url = ""
        PUA_ListaAppezzamenti = New List(Of PUA_Appezzamento)
    End Sub

End Class

Public Class PUA_Fabbisogni_output

    Public PUA_ListaAppezzamenti As List(Of PUA_Appezzamento)
    Public N_Fabbisogno As Decimal
    Public MessaggioErrore As String

    Public Sub New()
        PUA_ListaAppezzamenti = New List(Of PUA_Appezzamento)
        N_Fabbisogno = 0
        MessaggioErrore = ""
    End Sub

End Class

Public Class PUA_Coltura

    Public b_perc As Decimal
    Public base As Decimal
    Public coeff_incr As Decimal
    Public coeff_temp As Decimal

    Public bilancio_completo As Integer

    'Public n_distribuito As Decimal

End Class



