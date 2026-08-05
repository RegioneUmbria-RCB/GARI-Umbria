
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.Sicurezza



Module PageWarp
    '###############################################################################
    'Ampliare gestendo tutte le pagine
    Public Function PaginaAspx_from_TipoEnumPagina(ByVal Pag_GiasOnline As enum_PagineGiasOnline, _
                                                    ByVal PathToRoot As String) As String

        Dim Url As String

        Select Case Pag_GiasOnline

            Case enum_PagineGiasOnline.MenuContab '0
                'Return "MenuContabilita.aspx"
                Url = PathToRoot + "GestioneContabilita/MenuContabilita.aspx"

            Case enum_PagineGiasOnline.FiltroMovContabili '1
                'Return "FiltroMovContabili.aspx"
                Url = PathToRoot + "GestioneContabilita/FiltroMovContabili.aspx"

            Case enum_PagineGiasOnline.PianoConti '2
                'Return "PianoConti.aspx"
                Url = PathToRoot + "GestioneContabilita/AnagrafeContabilita/PianoConti.aspx"

            Case enum_PagineGiasOnline.DocTrasporto '3
                'Return "Bolle_New.aspx"
                Url = PathToRoot + "GestioneContabilita/DocumentiTrasporto/Bolle_New.aspx"

            Case enum_PagineGiasOnline.Fattura '4
                'Return "Fattura.aspx"
                Url = PathToRoot + "GestioneContabilita/MovEconomici/Fattura.aspx"

            Case enum_PagineGiasOnline.AltriCostiRicavi '5
                'Return "Altri_Costi_Ricavi.aspx"
                Url = PathToRoot + "GestioneContabilita/MovEconomici/Altri_Costi_Ricavi.aspx"

            Case enum_PagineGiasOnline.LiquidazioneIVA '6
                'Return "LiquidazioneIVA.aspx"
                Url = PathToRoot + "GestioneContabilita/Elaborati/LiquidazioneIVA.aspx"

            Case enum_PagineGiasOnline.PrimaNota '7
                'Return "PrimaNota.aspx"
                Url = PathToRoot + "GestioneContabilita/Elaborati/PrimaNota.aspx"

            Case enum_PagineGiasOnline.FormProdotto '8
                'Return "FormProdotto.aspx"
                Url = PathToRoot + "GestioneMagazzini/FormProdotto.aspx"

            Case enum_PagineGiasOnline.MagazziniGiacenze_Info '9
                'Return "FormProdotto.aspx"
                Url = PathToRoot + "GestioneMagazzini/MagazziniGiacenze_Info.aspx"

            Case enum_PagineGiasOnline.MagazziniMovimenti_Info '10
                'Return "FormProdotto.aspx"
                Url = PathToRoot + "GestioneMagazzini/MagazziniMovimenti_Info.aspx"

            Case enum_PagineGiasOnline.StalleVariazioniConsistenze_Info '11
                Url = PathToRoot + "GestioneStalle/StalleVariazioniConsistenze_Info.aspx"

            Case enum_PagineGiasOnline.AlberoImprese '30
                Url = PathToRoot + "AlberoImprese/AlberoImprese.aspx"

            Case enum_PagineGiasOnline.MenuAgenda '31
                Url = PathToRoot + "Agenda/MenuAgenda/MenuAgenda.aspx"

            Case enum_PagineGiasOnline.MenuPrincipale '32
                Url = PathToRoot + "Menu/Menu.aspx"

            Case enum_PagineGiasOnline.MenuStampe '33
                Url = PathToRoot + "GestioneStampe/MenuStampe.aspx"

            Case enum_PagineGiasOnline.MenuArchivi '34
                Url = PathToRoot + "ManutenzioneArchivi/MenuArchivi.aspx"

            Case enum_PagineGiasOnline.MenuAnalisiCosti '55
                Url = PathToRoot + "giasonline/AnalisiCostiProduzione/MenuAnalisiCosti.aspx"

            Case Else
                Url = ""

        End Select

        Return Url


    End Function


End Module
