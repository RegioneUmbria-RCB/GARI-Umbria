Imports System.Data.Entity
Imports System.Transactions

Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider

Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json.JsonConvert

Imports System.Linq
Imports System.Xml.Linq

Imports AgronicaConversioneCartografiaGias.FormatsConverter
Imports AgronicaConversioneCartografiaGias.Agronica
Imports AgronicaConversioneCartografiaGias
Imports AgronicaGIS2012.Commons
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports System.Data.Common

Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO


Public Class Programmazione_R
    Inherits AgronicaCoreDataProvider.DataProvider

#Region "CoreLettura"

    Public Function Pianificazione_Leggi(ByVal Programmazione_Cod As Integer,
                                          ByRef Programmazione_Des As String,
                                          ByRef Programmazione_Des_Long As String,
                                          ByRef Piva As String,
                                          ByRef Sa_Cod As Integer,
                                          ByRef Note As String,
                                          ByRef Validita_Inizio As Date,
                                          ByRef Validita_Fine As Date,
                                          ByRef Tipo_Pianificazione As Integer,
                                          ByRef Dt_Appezzamenti As DataTable,
                                          ByRef Dt_Particelle As DataTable,
                                          ByRef TuttiCentri As Boolean,
                                                ByRef objParametri As AgronicaCoreParametri,
                                         Optional ByVal VerificaSeRibaltati As Boolean = False,
                                         Optional ByVal VerificaSeMovimentati As Boolean = False,
                                         Optional ByVal VerificaSeProvenientiDaFascicolo As Boolean = False,
                                         Optional ByVal VerificaSeGIS As Boolean = False
                                          ) As Integer


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Programmazione_R.Pianificazione_Leggi()"

        Dim FlagConnessioneLocale As Boolean = False
        Dim xConnessione As DbConnection = Nothing
        Dim xConnectionState As ConnectionState = ConnectionState.Closed
        Dim xTransazione As DbTransaction

        Dim Dt_Testata As New DataTable
        Dim Dt_Entita As New DataTable
        Dim Dt_EntitaxParticelle As New DataTable
        Dim Programmazione_Entita_Cod As Integer
        Dim i, j As Integer
        Dim Matrice(0, 12) As Object
        Dim ArrayEntita(120) As Object
        Dim strCentri As String()
        Dim ErrMSG As String = ""
        Dim MessaggioErrore As String
        Dim DrPart As DataRow()

        TuttiCentri = True

        Dim Fonte_Cod As Integer = 0

        Dim Ribaltato As Integer
        Dim Movimentato As Integer

        Dim Allegati_Documenti_Numero As String = ""
        Dim Allegati_Documenti_Numero_E As String = ""

        Try

            '------------------------------
            'Verifico se e' stata impostata una connessione
            If IsNothing(objParametri.objConnessione) Then
                'Flag
                FlagConnessioneLocale = True
                'Creo la connessione localmente
                xConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
                xTransazione = Nothing
            Else
                'Utilizzo quella passata come parametro
                xConnessione = objParametri.objConnessione
                xConnectionState = objParametri.objConnessione.State
                xTransazione = objParametri.objTransazione
            End If



            '----------------------------------------------
            'Leggo i dati della Testata
            Dim objProgrammazione_Testata As New AgronicaCoreAnagrafeDAL.Programmazione_Testata_R

            Dt_Testata = objProgrammazione_Testata.Leggi(
                                       ErrMSG,
                                       Programmazione_Cod,
                                       "",
                                       "",
                                       1,
                                       Validita_Inizio,
                                       Validita_Fine,
                                       enum_TipoRicetta.Non_Filtrare,
                                       enumSelezioneVariabile.Selezione_TabellaCompleta,
                                       "",
                                       "",
                                       objParametri)

            If Dt_Testata.Rows.Count > 0 Then

                Programmazione_Des = Dt_Testata.Rows(0).Item("Programmazione_Des").ToString
                Programmazione_Des_Long = ""
                Piva = Dt_Testata.Rows(0).Item("Piva").ToString
                Note = Dt_Testata.Rows(0).Item("Note").ToString

                'Validita_Inizio = CDate(Dt_Testata.Rows(0).Item("Validita_Inizio"))
                'Validita_Fine = CDate(Dt_Testata.Rows(0).Item("Validita_Fine"))
                Tipo_Pianificazione = CInt(Dt_Testata.Rows(0).Item("Tipo_Pianificazione"))
                Fonte_Cod = CInt(IIf(IsDBNull(Dt_Testata.Rows(0).Item("Fonte_Cod")), 0, Dt_Testata.Rows(0).Item("Fonte_Cod")))


                'Leggo i dati delle Entita
                Dim objProgrammazione_Entita As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_R
                Dt_Entita = objProgrammazione_Entita.Programmazione_Entita_Leggi(
                                            Programmazione_Cod,
                                            ErrMSG,
                                            0,
                                            "",
                                            "",
                                            0,
                                            0,
                                            0,
                                            0,
                                            0,
                                            Validita_Inizio,
                                            Validita_Fine,
                                            1,
                                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                                            "",
                                            "",
                                            objParametri,
                                            Fonte_Cod)

                'Leggo se le entità sono già state ribaltate e/o movimentate
                Dim objERib As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Programmazioni_R
                Dim objReg_Impianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Programmazioni_R
                Dim objGis As New AgronicaCoreGisDAL.GIS_Entita_R
                Dim objERibDAL As New AgronicaCoreAnagrafeBIZ.Reg_Impianto_R
                Dim DtERib As New DataTable
                Dim DrERib As DataRow()
                Dim DtEMov As New DataTable
                Dim DrEMov As DataRow()
                Dim DtEMov_Ricette As New DataTable
                Dim DrEMov_Ricette As DataRow()
                Dim DtEMov_Ricette_P As New DataTable
                Dim DrEMov_Ricette_P As DataRow()
                Dim listEntita_Gis As New List(Of Integer)

                Dim Gis_su_Campo As Boolean = False

                Dim objEFasc As New AgronicaCoreAnagrafeDAL.Allegati_EntitaxDocumenti_R
                Dim DtEFasc As New DataTable
                Dim DrEFasc As DataRow()

                If VerificaSeRibaltati Then
                    DtERib = objERib.Leggi("", 0, 0, 0, 0, CInt(Programmazione_Cod), 0, "", "", objParametri)
                End If
                If VerificaSeMovimentati Then
                    DtEMov = objERib.Leggi_OperazioniImpiantiRibaltati_DaProgrammazione("", CInt(Programmazione_Cod), 0, "", "", objParametri)
                    DtEMov_Ricette = objERib.Leggi_RicetteImpiantiRibaltati_DaProgrammazione("", CInt(Programmazione_Cod), 0, "", "", objParametri)
                    DtEMov_Ricette_P = objERib.Leggi_RicetteProgrammazioniRibaltati_DaProgrammazione("", CInt(Programmazione_Cod), 0, "", "", objParametri)
                End If
                If VerificaSeGIS Then
                    'Controllo se c'è GIS sulla Programmazione
                    'For Each row In objGis.Leggi(objParametri.PivaSuperUser, 0, 0, Piva, Sa_Cod, 0, 0, 0, "", "", "0", 0, 0, "0", 0, 0, CInt(Programmazione_Cod), 0, "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri).Rows
                    '    listEntita_Gis.Add(row.item("Programmazione_Entita_Cod"))
                    'Next


                    'Controllo se c'è GIS sul CAMPO
                    'For Each row As DataRow In objERib.Leggi(Piva, 0, 0, 0, 0, CInt(Programmazione_Cod), 0, "", "", objParametri).Rows
                    '    Dim DT = objGis.Leggi("", 0, 0, row.Item("piva"), row.Item("sa_cod"), 0, row.Item("Campo_Cod"), 0, "", "", "", 0, 0, "", 0, 0, 0, 0, "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)
                    '    If DT.Select(" Appezza = 0 AND ID_Imp = 0 AND Programmazione_Entita_Cod = 0 ").CopyToDataTable.Rows.Count > 0 Then
                    '        Gis_su_Campo = True
                    '    End If
                    'Next

                    'Controllo se c'è GIS su APPEZZA o ID_REG
                    For Each row As DataRow In objERib.Leggi(Piva, 0, 0, 0, 0, CInt(Programmazione_Cod), 0, "", "", objParametri).Rows
                        For Each row_entita In objGis.LeggiDB(objParametri.PivaSuperUser, 0, 0, row.Item("piva"), row.Item("sa_cod"), row.Item("appezza"), 0, 0, "", "", "", 0, 0, "", 0, 0, 0, 0, "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri).Rows
                            If Not listEntita_Gis.Contains(row.Item("Programmazione_Entita_Cod")) Then
                                listEntita_Gis.Add(row.Item("Programmazione_Entita_Cod"))
                            End If
                        Next
                    Next
                End If
                If VerificaSeProvenientiDaFascicolo Then
                    DtEFasc = objEFasc.Leggi(0, Piva, 0, 0, 0, 0, 0, "", "", "", 0, 0, "", "", "", 0, CInt(Programmazione_Cod), 0, "", "", objParametri)
                    If DtEFasc IsNot Nothing AndAlso DtEFasc.Rows.Count > 0 Then
                        Allegati_Documenti_Numero = DtEFasc.Rows(0).Item("Allegati_Documenti_Numero")
                    End If
                End If


                Dim objDP As New AgronicaCoreDataProvider.DatatableUtility
                strCentri = objDP.SelectDistinct(Dt_Entita, "Sa_Cod")

                'If Not strCentri Is Nothing AndAlso strCentri.Length > 0 AndAlso strCentri.Length = 1 Then
                '    TuttiCentri = False
                'End If

                Dim objProgrammazione_Particelle As New AgronicaCoreAnagrafeDAL.Programmazione_Particelle_R
                Dt_EntitaxParticelle = objProgrammazione_Particelle.Programmazione_Particelle_Leggi(objParametri,
                                                                                  Piva,
                                                                                  ErrMSG,
                                                                                  Programmazione_Cod)

                For i = 0 To Dt_Entita.Rows.Count - 1

                    Dim Catasto As String = ""

                    Sa_Cod = Dt_Entita.Rows(i).Item("sa_cod")

                    Programmazione_Entita_Cod = CInt(Dt_Entita.Rows(i).Item("Programmazione_Entita_Cod"))

                    'Leggo le particelle associate all'Entita
                    'Dim objProgrammazione_Particelle As New AgronicaCoreAnagrafeDAL.Programmazione_Particelle_R
                    'Dt_EntitaxParticelle = objProgrammazione_Particelle.Programmazione_Particelle_Leggi(objParametri,
                    '                                                              Piva,
                    '                                                              ErrMSG,
                    '                                                              0, Programmazione_Entita_Cod)

                    DrPart = Dt_EntitaxParticelle.Select("Programmazione_Entita_Cod=" & Programmazione_Entita_Cod.ToString)

                    If DrPart IsNot Nothing AndAlso DrPart.Length > 0 Then

                        For j = 0 To DrPart.Length - 1


                            Dim SupCatastale As Decimal = 0
                            SupCatastale = Ettari_from_EttariAreCentiare(DrPart(j).Item("ETTARI"),
                                                                                                                  DrPart(j).Item("ARE"),
                                                                                                                  DrPart(j).Item("CENTIARE"))
                            DT_Intersezioni_Insert(Dt_Particelle,
                                                   Tipo_Pianificazione,
                                                   Piva,
                                                   Dt_Entita.Rows(i).Item("sa_cod"),
                                                   Dt_Entita.Rows(i).Item("campo_cod"),
                                                   Dt_Entita.Rows(i).Item("appezza"),
                                                   DrPart(j).Item("Prov"),
                                                   DrPart(j).Item("Com"),
                                                   DrPart(j).Item("Prov_Des"),
                                                   DrPart(j).Item("Com_Des"),
                                                   DrPart(j).Item("Sezione"),
                                                   DrPart(j).Item("Foglio"),
                                                   DrPart(j).Item("Numero"),
                                                   DrPart(j).Item("Subalterno"),
                                                   DrPart(j).Item("Sup_Totale"),
                                                   DrPart(j).Item("Superficie"),
                                                   SupCatastale,
                                                   DrPart(j).Item("Validita_Inizio"),
                                                   DrPart(j).Item("Validita_Fine"),
                                                   "Dal " & CDate(DrPart(j).Item("Validita_Inizio")).ToString("dd/MM/yyyy") & " al " & CDate(DrPart(j).Item("Validita_Fine")).ToString("dd/MM/yyyy"),
                                                   0,
                                                   DrPart(j).Item("TitoloPossesso"),
                                                   CStr(DrPart(j).Item("Proprietario")))

                            Catasto &= IIf(j <> 0, "<BR>", "") &
                                    DrPart(j).Item("Prov") & "_" &
                                    DrPart(j).Item("Com") & "_" &
                                    DrPart(j).Item("Sezione") & "_" &
                                    DrPart(j).Item("Foglio") & "_" &
                                    DrPart(j).Item("Numero") & "_" &
                                    DrPart(j).Item("Subalterno") &
                                    " (Sup." & CStr(DrPart(j).Item("Superficie")) & " Ha)"

                        Next

                    End If
                    Dim Operazione_Cod As Integer = 0
                    Dim Operazione_Des As String = ""
                    If Not IsDBNull(Dt_Entita.Rows(i).Item("Operazione_Cod")) Then
                        Operazione_Cod = Dt_Entita.Rows(i).Item("Operazione_Cod")
                        Operazione_Des = OperazioneDes_From_OperazioneCod(Dt_Entita.Rows(i).Item("Operazione_Cod"))
                    End If

                    Dim veg_des_cliente As String = ""
                    Dim cul_des_cliente As String = ""
                    If Not IsNothing(Dt_Entita.Rows(i).Item("VegCul_Des_Cliente")) AndAlso
                    Not IsDBNull(Dt_Entita.Rows(i).Item("VegCul_Des_Cliente")) AndAlso
                    InStr(CStr(Dt_Entita.Rows(i).Item("VegCul_Des_Cliente")), "|") > 0 Then
                        veg_des_cliente = CStr(Dt_Entita.Rows(i).Item("VegCul_Des_Cliente")).Split("|")(0)
                        cul_des_cliente = CStr(Dt_Entita.Rows(i).Item("VegCul_Des_Cliente")).Split("|")(1)
                    End If


                    Ribaltato = 0
                    Movimentato = 0

                    If VerificaSeRibaltati Then
                        DrERib = DtERib.Select("Programmazione_Entita_Cod=" & Programmazione_Entita_Cod.ToString)
                        If DrERib IsNot Nothing AndAlso DrERib.Length > 0 Then
                            Ribaltato = 1
                        End If
                    End If

                    If VerificaSeMovimentati Then
                        DrEMov = DtEMov.Select("Programmazione_Entita_Cod=" & Programmazione_Entita_Cod.ToString)
                        If DrEMov IsNot Nothing AndAlso DrEMov.Length > 0 Then
                            Movimentato = 1
                        End If
                        DrEMov_Ricette = DtEMov_Ricette.Select("Programmazione_Entita_Cod=" & Programmazione_Entita_Cod.ToString)
                        If DrEMov_Ricette IsNot Nothing AndAlso DrEMov_Ricette.Length > 0 Then
                            Movimentato = 1
                        End If
                        DrEMov_Ricette_P = DtEMov_Ricette.Select("Programmazione_Entita_Cod=" & Programmazione_Entita_Cod.ToString)
                        If DrEMov_Ricette_P IsNot Nothing AndAlso DrEMov_Ricette_P.Length > 0 Then
                            Movimentato = 1
                        End If
                    End If

                    If VerificaSeGIS Then
                        If Gis_su_Campo Then
                            Movimentato = 1
                        Else
                            If listEntita_Gis.Contains(Programmazione_Entita_Cod) Then
                                Movimentato = 1
                            End If
                        End If
                    End If

                    Allegati_Documenti_Numero_E = ""
                    If VerificaSeProvenientiDaFascicolo Then
                        DrEFasc = DtEFasc.Select("Programmazione_Entita_Cod=" & Programmazione_Entita_Cod.ToString)
                        If DrEFasc IsNot Nothing AndAlso DrEFasc.Length > 0 Then
                            Allegati_Documenti_Numero_E = Allegati_Documenti_Numero
                        End If
                    End If

                    Dim Stato_Ribaltamento As Integer = 0
                    If Not IsDBNull(Dt_Entita.Rows(i).Item("Stato_Ribaltamento")) Then
                        Stato_Ribaltamento = Dt_Entita.Rows(i).Item("Stato_Ribaltamento")
                    End If

                    Dim Veg_Cod_Agea = ""
                    If Not IsDBNull(Dt_Entita.Rows(i).Item("Veg_Cod_Agea")) Then
                        Veg_Cod_Agea = Dt_Entita.Rows(i).Item("Veg_Cod_Agea")
                    End If

                    Dim Cul_Cod_Agea = ""
                    If Not IsDBNull(Dt_Entita.Rows(i).Item("Cul_Cod_Agea")) Then
                        Cul_Cod_Agea = Dt_Entita.Rows(i).Item("Cul_Cod_Agea")
                    End If

                    Dim Uso_Cod_Agea = ""
                    If Not IsDBNull(Dt_Entita.Rows(i).Item("Uso_Cod_Agea")) Then
                        Uso_Cod_Agea = Dt_Entita.Rows(i).Item("Uso_Cod_Agea")
                    End If

                    Dim Occupazione_Cod_Agea = ""
                    If Not IsDBNull(Dt_Entita.Rows(i).Item("Occupazione_Cod_Agea")) Then
                        Occupazione_Cod_Agea = Dt_Entita.Rows(i).Item("Occupazione_Cod_Agea")
                    End If

                    Dim Destinazione_Cod_Agea = ""
                    If Not IsDBNull(Dt_Entita.Rows(i).Item("Destinazione_Cod_Agea")) Then
                        Destinazione_Cod_Agea = Dt_Entita.Rows(i).Item("Destinazione_Cod_Agea")
                    End If

                    Dim Qualita_Cod_Agea = ""
                    If Not IsDBNull(Dt_Entita.Rows(i).Item("Qualita_Cod_Agea")) Then
                        Qualita_Cod_Agea = Dt_Entita.Rows(i).Item("Qualita_Cod_Agea")
                    End If

                    Dim Limite_N = ""
                    If Not IsDBNull(Dt_Entita.Rows(i).Item("Limite_N")) Then
                        Limite_N = Dt_Entita.Rows(i).Item("Limite_N")
                    End If

                    Dim Limite_P = ""
                    If Not IsDBNull(Dt_Entita.Rows(i).Item("Limite_P")) Then
                        Limite_P = Dt_Entita.Rows(i).Item("Limite_P")
                    End If

                    Dim Limite_K = ""
                    If Not IsDBNull(Dt_Entita.Rows(i).Item("Limite_K")) Then
                        Limite_K = Dt_Entita.Rows(i).Item("Limite_K")
                    End If

                    Dim Data_Fioritura_Prevista = AGRODATAINIZIO
                    If Not IsDBNull(Dt_Entita.Rows(i).Item("Data_Fioritura_Prevista")) Then
                        Data_Fioritura_Prevista = Dt_Entita.Rows(i).Item("Data_Fioritura_Prevista")
                    End If

                    Dim Veg_Cod_Prec = "0"
                    If Not IsDBNull(Dt_Entita.Rows(i).Item("Veg_Cod_Prec")) Then
                        Veg_Cod_Prec = Dt_Entita.Rows(i).Item("Veg_Cod_Prec")
                    End If

                    Dim Veg_Cod_Prec2 = "0"
                    If Not IsDBNull(Dt_Entita.Rows(i).Item("Veg_Cod_Prec2")) Then
                        Veg_Cod_Prec2 = Dt_Entita.Rows(i).Item("Veg_Cod_Prec2")
                    End If

                    Dim Veg_Cod_Prec3 = "0"
                    If Not IsDBNull(Dt_Entita.Rows(i).Item("Veg_Cod_Prec3")) Then
                        Veg_Cod_Prec3 = Dt_Entita.Rows(i).Item("Veg_Cod_Prec3")
                    End If

                    Dim Piano_Semina = ""
                    If Not IsDBNull(Dt_Entita.Rows(i).Item("Piano_Semina")) Then
                        Piano_Semina = Dt_Entita.Rows(i).Item("Piano_Semina")
                    End If

                    Dim Codice_Contratto = ""
                    If Not IsDBNull(Dt_Entita.Rows(i).Item("Codice_Contratto")) Then
                        Codice_Contratto = Dt_Entita.Rows(i).Item("Codice_Contratto")
                    End If

                    'Dim provenienza_fascicolo As String = ""
                    'Dim fascicoloAllegatiLettura As New AgronicaCoreAnagrafeDAL.Allegati_EntitaxDocumenti_R
                    'Dim DT = fascicoloAllegatiLettura.Leggi(0, Piva, 0, 0, 0, 0, 0, "", "", "", 0, 0, "", "", "", 0, Programmazione_Cod, 0, "", "", objParametri)
                    'If DT IsNot Nothing AndAlso DT.Rows.Count > 0 Then
                    '    provenienza_fascicolo = DT.Rows(0).Item("Allegati_Documenti_Numero")
                    'End If

                    If IsDBNull(Dt_Entita.Rows(i).Item("IAF")) Then
                        Dt_Entita.Rows(i).Item("IAF") = ""
                    End If

                    If IsDBNull(Dt_Entita.Rows(i).Item("Regolamento_Concimazione_Cod")) Then
                        Dt_Entita.Rows(i).Item("Regolamento_Concimazione_Cod") = 0
                    End If

                    If IsDBNull(Dt_Entita.Rows(i).Item("Flag_PubblicoPrivato")) Then
                        Dt_Entita.Rows(i).Item("Flag_PubblicoPrivato") = 0
                    End If

                    If IsDBNull(Dt_Entita.Rows(i).Item("id_tr")) Then
                        Dt_Entita.Rows(i).Item("id_tr") = 0
                    End If

                    Dim DistBZ_CorpiIdrici As Double = 0
                    If Dt_Entita.Rows(i).Item("DistBZ_CorpiIdrici") IsNot Nothing AndAlso Not IsDBNull(Dt_Entita.Rows(i).Item("DistBZ_CorpiIdrici")) Then
                        DistBZ_CorpiIdrici = CDbl(Dt_Entita.Rows(i).Item("DistBZ_CorpiIdrici"))
                    End If


                    Dim DistBZ_AreeResPub As Double = 0
                    If Dt_Entita.Rows(i).Item("DistBZ_AreeResPub") IsNot Nothing AndAlso Not IsDBNull(Dt_Entita.Rows(i).Item("DistBZ_AreeResPub")) Then
                        DistBZ_AreeResPub = CDbl(Dt_Entita.Rows(i).Item("DistBZ_AreeResPub"))
                    End If

                    Dim DistBZ_Allevamenti As Double = 0
                    If Dt_Entita.Rows(i).Item("DistBZ_Allevamenti") IsNot Nothing AndAlso Not IsDBNull(Dt_Entita.Rows(i).Item("DistBZ_Allevamenti")) Then
                        DistBZ_Allevamenti = CDbl(Dt_Entita.Rows(i).Item("DistBZ_Allevamenti"))
                    End If

                    Dim DistBZ_VegNatNonColt As Double = 0
                    If Dt_Entita.Rows(i).Item("DistBZ_VegNatNonColt") IsNot Nothing AndAlso Not IsDBNull(Dt_Entita.Rows(i).Item("DistBZ_VegNatNonColt")) Then
                        DistBZ_VegNatNonColt = CDbl(Dt_Entita.Rows(i).Item("DistBZ_VegNatNonColt"))
                    End If

                    Dim SupBZ_Riduzione As Double = 0
                    If Dt_Entita.Rows(i).Item("SupBZ_Riduzione") IsNot Nothing AndAlso Not IsDBNull(Dt_Entita.Rows(i).Item("SupBZ_Riduzione")) Then
                        SupBZ_Riduzione = CDbl(Dt_Entita.Rows(i).Item("SupBZ_Riduzione"))
                    End If

                    Dim Isola As String = ""
                    If Dt_Entita.Rows(i).Item("Isola") IsNot Nothing AndAlso Not IsDBNull(Dt_Entita.Rows(i).Item("Isola")) Then
                        Isola = Dt_Entita.Rows(i).Item("Isola")
                    End If

                    Dim Riferimento_Alfanumerico_Appezzamento As String = ""
                    If Dt_Entita.Rows(i).Item("Riferimento_Alfanumerico_Appezzamento") IsNot Nothing AndAlso Not IsDBNull(Dt_Entita.Rows(i).Item("Riferimento_Alfanumerico_Appezzamento")) Then
                        Riferimento_Alfanumerico_Appezzamento = Dt_Entita.Rows(i).Item("Riferimento_Alfanumerico_Appezzamento")
                    End If

                    Dim CapitolatoPrivato As String = ""
                    If Dt_Entita.Rows(i).Item("CapitolatoPrivato") IsNot Nothing AndAlso Not IsDBNull(Dt_Entita.Rows(i).Item("CapitolatoPrivato")) Then
                        CapitolatoPrivato = Dt_Entita.Rows(i).Item("CapitolatoPrivato")
                    End If

                    Dim CapitolatoPrivato_Des As String = ""
                    If Dt_Entita.Rows(i).Item("CapitolatoPrivato_Des") IsNot Nothing AndAlso Not IsDBNull(Dt_Entita.Rows(i).Item("CapitolatoPrivato_Des")) Then
                        CapitolatoPrivato_Des = Dt_Entita.Rows(i).Item("CapitolatoPrivato_Des")
                    End If

                    Dim Finalita_Concimazione_Impianto As Integer = 0
                    If Dt_Entita.Rows(i).Item("Finalita_Concimazione_Impianto") IsNot Nothing AndAlso Not IsDBNull(Dt_Entita.Rows(i).Item("Finalita_Concimazione_Impianto")) Then
                        Finalita_Concimazione_Impianto = Dt_Entita.Rows(i).Item("Finalita_Concimazione_Impianto")
                    End If

                    Dim Cod_Indirizzo As Integer = 0
                    If Dt_Entita.Rows(i).Item("Cod_Indirizzo") IsNot Nothing AndAlso Not IsDBNull(Dt_Entita.Rows(i).Item("Cod_Indirizzo")) Then
                        Cod_Indirizzo = Dt_Entita.Rows(i).Item("Cod_Indirizzo")
                    End If

                    Dim KPIN As String = ""
                    If Dt_Entita.Rows(i).Item("KPIN") IsNot Nothing AndAlso Not IsDBNull(Dt_Entita.Rows(i).Item("KPIN")) Then
                        KPIN = Dt_Entita.Rows(i).Item("KPIN")
                    End If

                    Dim Block_Name As String = ""
                    If Dt_Entita.Rows(i).Item("Block_Name") IsNot Nothing AndAlso Not IsDBNull(Dt_Entita.Rows(i).Item("Block_Name")) Then
                        Block_Name = Dt_Entita.Rows(i).Item("Block_Name")
                    End If

                    Dim ZespriFase_Cod As Integer = 0
                    If Dt_Entita.Rows(i).Item("ZespriFase_Cod") IsNot Nothing AndAlso Not IsDBNull(Dt_Entita.Rows(i).Item("ZespriFase_Cod")) Then
                        ZespriFase_Cod = Dt_Entita.Rows(i).Item("ZespriFase_Cod")
                    End If

                    Dim ZespriFase_Des As String = ""
                    If Dt_Entita.Rows(i).Item("ZespriFase_Des") IsNot Nothing AndAlso Not IsDBNull(Dt_Entita.Rows(i).Item("ZespriFase_Des")) Then
                        ZespriFase_Des = Dt_Entita.Rows(i).Item("ZespriFase_Des")
                    End If

                    Dim ZespriTipo_Cod As Integer = 0
                    If Dt_Entita.Rows(i).Item("ZespriTipo_Cod") IsNot Nothing AndAlso Not IsDBNull(Dt_Entita.Rows(i).Item("ZespriTipo_Cod")) Then
                        ZespriTipo_Cod = Dt_Entita.Rows(i).Item("ZespriTipo_Cod")
                    End If

                    Dim ZespriTipo_Des As String = ""
                    If Dt_Entita.Rows(i).Item("ZespriTipo_Des") IsNot Nothing AndAlso Not IsDBNull(Dt_Entita.Rows(i).Item("ZespriTipo_Des")) Then
                        ZespriTipo_Des = Dt_Entita.Rows(i).Item("ZespriTipo_Des")
                    End If

                    Dim ZespriGrower_Cod As Integer = 0
                    If Dt_Entita.Rows(i).Item("ZespriGrower_Cod") IsNot Nothing AndAlso Not IsDBNull(Dt_Entita.Rows(i).Item("ZespriGrower_Cod")) Then
                        ZespriGrower_Cod = Dt_Entita.Rows(i).Item("ZespriGrower_Cod")
                    End If

                    Dim ZespriGrower_Des As String = ""
                    If Dt_Entita.Rows(i).Item("ZespriGrower_Des") IsNot Nothing AndAlso Not IsDBNull(Dt_Entita.Rows(i).Item("ZespriGrower_Des")) Then
                        ZespriGrower_Des = Dt_Entita.Rows(i).Item("ZespriGrower_Des")
                    End If


                    Dim TipologiaInnestoTrapianto_Cod As String = ""
                    If Dt_Entita.Rows(i).Item("TipologiaInnestoTrapianto_Cod") IsNot Nothing AndAlso Not IsDBNull(Dt_Entita.Rows(i).Item("TipologiaInnestoTrapianto_Cod")) Then
                        TipologiaInnestoTrapianto_Cod = Dt_Entita.Rows(i).Item("TipologiaInnestoTrapianto_Cod")
                    End If

                    Dim TipologiaInnestoTrapianto_Des As String = ""
                    If Dt_Entita.Rows(i).Item("TipologiaInnestoTrapianto_Des") IsNot Nothing AndAlso Not IsDBNull(Dt_Entita.Rows(i).Item("TipologiaInnestoTrapianto_Des")) Then
                        TipologiaInnestoTrapianto_Des = Dt_Entita.Rows(i).Item("TipologiaInnestoTrapianto_Des")
                    End If

                    Dim Mat_Cod As Integer = 0
                    If Dt_Entita.Rows(i).Item("Mat_Cod") IsNot Nothing AndAlso Not IsDBNull(Dt_Entita.Rows(i).Item("Mat_Cod")) Then
                        Mat_Cod = Dt_Entita.Rows(i).Item("Mat_Cod")
                    End If

                    Dim Mat_Des As String = ""
                    If Dt_Entita.Rows(i).Item("Mat_Des") IsNot Nothing AndAlso Not IsDBNull(Dt_Entita.Rows(i).Item("Mat_Des")) Then
                        Mat_Des = Dt_Entita.Rows(i).Item("Mat_Des")
                    End If

                    DT_Appezzamenti_Insert(Dt_Appezzamenti,
                       1,
                       Operazione_Cod,
                       Operazione_Des,
                       Piva,
                       Dt_Entita.Rows(i).Item("sa_cod"),
                       Dt_Entita.Rows(i).Item("campo_cod"),
                       Dt_Entita.Rows(i).Item("appezza"),
                       Dt_Entita.Rows(i).Item("id_reg"),
                       Dt_Entita.Rows(i).Item("progetto_cod"),
                       Dt_Entita.Rows(i).Item("progetto_des"),
                       Dt_Entita.Rows(i).Item("entita_des"),
                       Dt_Entita.Rows(i).Item("Superficie"),
                       Dt_Entita.Rows(i).Item("veg_cod"),
                       Dt_Entita.Rows(i).Item("veg_des"),
                       Dt_Entita.Rows(i).Item("cul_cod"),
                       Dt_Entita.Rows(i).Item("cul_des"),
                       Dt_Entita.Rows(i).Item("grva_cod"),
                       Dt_Entita.Rows(i).Item("grva_des"),
                       Dt_Entita.Rows(i).Item("grfi_cod"),
                       Dt_Entita.Rows(i).Item("grfi_des"),
                       Dt_Entita.Rows(i).Item("cop_cod"),
                       Dt_Entita.Rows(i).Item("cop_des"),
                       Dt_Entita.Rows(i).Item("resa"),
                       Dt_Entita.Rows(i).Item("id_cod"),
                       Dt_Entita.Rows(i).Item("destinazioneuso_des"),
                       Dt_Entita.Rows(i).Item("num_piante"),
                       Dt_Entita.Rows(i).Item("tra_fila"),
                       Dt_Entita.Rows(i).Item("su_fila"),
                       Dt_Entita.Rows(i).Item("validita_inizio_Impianto"),
                       Dt_Entita.Rows(i).Item("validita_inizio"),
                       Dt_Entita.Rows(i).Item("validita_fine"),
                       Dt_Entita.Rows(i).Item("Programmazione_Entita_Cod"),
                       Dt_Entita.Rows(i).Item("Data_Semina"),
                       Dt_Entita.Rows(i).Item("Data_Raccolta"),
                       Dt_Entita.Rows(i).Item("TipoZona"),
                       Dt_Entita.Rows(i).Item("MetodoProduzione_Cod"),
                       Dt_Entita.Rows(i).Item("MetodoProduzione_Des"),
                       Catasto,
                       Dt_Entita.Rows(i).Item("sa_nome"),
                       Dt_Entita.Rows(i).Item("campo_des"),
                       Dt_Entita.Rows(i).Item("veg_cod_cliente"),
                       Dt_Entita.Rows(i).Item("cul_cod_cliente"),
                       veg_des_cliente,
                       cul_des_cliente,
                       Dt_Entita.Rows(i).Item("macrouso_cod"),
                       Dt_Entita.Rows(i).Item("macrouso_des"),
                       Dt_Entita.Rows(i).Item("unita_vitata"),
                       objParametri,
                       ElementoGrafico_Cod:=Dt_Entita.Rows(i).Item("ElementoGrafico_Cod"),
                       ElementoGrafico_Des:=Dt_Entita.Rows(i).Item("ElementoGrafico_des"),
                       Ribaltato:=Ribaltato,
                       Movimentato:=Movimentato,
                        Veg_Cod_Agea:=Veg_Cod_Agea,
                        Cul_Cod_Agea:=Cul_Cod_Agea,
                        Uso_Cod_Agea:=Uso_Cod_Agea,
                        Occupazione_Cod_Agea:=Occupazione_Cod_Agea,
                        Destinazione_Cod_Agea:=Destinazione_Cod_Agea,
                        Qualita_Cod_Agea:=Qualita_Cod_Agea,
                        Limite_N:=Limite_N,
                        Limite_P:=Limite_P,
                        Limite_K:=Limite_K,
                        Data_Fioritura_Prevista:=Data_Fioritura_Prevista,
                        Veg_Cod_Prec:=Veg_Cod_Prec,
                        Veg_Cod_Prec2:=Veg_Cod_Prec2,
                        Veg_Cod_Prec3:=Veg_Cod_Prec3,
                        Piano_Semina:=Piano_Semina,
                        Codice_Contratto:=Codice_Contratto,
                        Disciplinare_Cod:=Dt_Entita.Rows(i).Item("Disciplinare_Cod"),
                        Regolamento_Cod:=Dt_Entita.Rows(i).Item("Regolamento_Cod"),
                        Stato_Cod:=Dt_Entita.Rows(i).Item("Stato_Cod"),
                        Stato_Ribaltamento:=Stato_Ribaltamento,
                        provenienza_fascicolo:=Allegati_Documenti_Numero_E,
                        Codice_Fiscale_Tecnico:=Dt_Entita.Rows(i).Item("Codice_Fiscale_Tecnico"),
                        datoGis:=Dt_Entita.Rows(i).Item("datoGis"),
                        IAF:=Dt_Entita.Rows(i).Item("IAF"),
                        Regolamento_Concimazione_Cod:=Dt_Entita.Rows(i).Item("Regolamento_Concimazione_Cod"),
                        Flag_PubblicoPrivato:=Dt_Entita.Rows(i).Item("Flag_PubblicoPrivato"),
                        id_tr:=Dt_Entita.Rows(i).Item("id_tr"),
                        DistBZ_CorpiIdrici:=DistBZ_CorpiIdrici,
                        DistBZ_AreeResPub:=DistBZ_AreeResPub,
                        DistBZ_Allevamenti:=DistBZ_Allevamenti,
                        DistBZ_VegNatNonColt:=DistBZ_VegNatNonColt,
                        SupBZ_Riduzione:=SupBZ_Riduzione,
                        Riferimento_Alfanumerico_Appezzamento:=Riferimento_Alfanumerico_Appezzamento,
                        Isola:=Isola,
                        CapitolatoPrivato:=CapitolatoPrivato,
                        CapitolatoPrivato_Des:=CapitolatoPrivato_Des,
                        Finalita_Concimazione_Impianto:=Finalita_Concimazione_Impianto,
                        Cod_Indirizzo:=Cod_Indirizzo,
                        ind_des:=Dt_Entita.Rows(i).Item("ind_des"),
                        frz_des:=Dt_Entita.Rows(i).Item("frz_des"),
                        CAP:=Dt_Entita.Rows(i).Item("CAP"),
                        com_des:=Dt_Entita.Rows(i).Item("com_des_indirizzo"),
                        pro_cod:=Dt_Entita.Rows(i).Item("pro_cod_indirizzo"),
                        stato:=Dt_Entita.Rows(i).Item("stato_indirizzo"),
                        stato_des:=Dt_Entita.Rows(i).Item("stato_indirizzo_des"),
                        note:=Dt_Entita.Rows(i).Item("note_indirizzo"),
                        pro_cod_istat:=Dt_Entita.Rows(i).Item("pro_cod_istat_indirizzo"),
                        com_cod_istat:=Dt_Entita.Rows(i).Item("com_cod_istat_indirizzo"),
                        KPIN:=KPIN,
                        Block_Name:=Block_Name,
                        Foral_Cod:=Dt_Entita.Rows(i).Item("Foral_Cod"),
                        Foral_Des:=Dt_Entita.Rows(i).Item("Foral_Des"),
                        Data_Inizio_Portinnesto:=Dt_Entita.Rows(i).Item("Data_Inizio_Portinnesto"),
                        Data_Creazione:=Dt_Entita.Rows(i).Item("Data_Creazione"),
                        Data_Modifica:=Dt_Entita.Rows(i).Item("Data_Modifica"),
                        ZespriFase_Cod:=ZespriFase_Cod,
                        ZespriFase_Des:=ZespriFase_Des,
                        ZespriGrower_Cod:=ZespriGrower_Cod,
                        ZespriGrower_Des:=ZespriGrower_Des,
                        ZespriTipo_Cod:=ZespriTipo_Cod,
                        ZespriTipo_Des:=ZespriTipo_Des,
                        Num_Piante_Femmine:=Dt_Entita.Rows(i).Item("Num_Piante_Femmine"),
                        Num_Piante_Maschi:=Dt_Entita.Rows(i).Item("Num_Piante_Maschi"),
                        Port_Cod:=Dt_Entita.Rows(i).Item("Port_Cod"),
                        Port_Des:=Dt_Entita.Rows(i).Item("Port_Des"),
                        TipologiaInnestoTrapianto_Cod:=TipologiaInnestoTrapianto_Cod,
                        TipologiaInnestoTrapianto_Des:=TipologiaInnestoTrapianto_Des,
                        Mat_Cod:=Mat_Cod,
                        Mat_Des:=Mat_Des)

                Next

            End If


        Catch ex As Exception

            Programmazione_Cod = 0
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        Finally

            If FlagConnessioneLocale Then
                If Not IsNothing(xConnessione) Then
                    xConnessione.Close()
                    xConnessione.Dispose()
                End If
            Else
                If xConnectionState = ConnectionState.Closed Then
                    If Not IsNothing(xConnessione) Then
                        xConnessione.Close()
                    End If
                End If
            End If

        End Try

        'Restituisco il risultato
        Return Programmazione_Cod

    End Function

    Public Function Pianificazione_Leggi_Old(ByVal Programmazione_Cod As Integer,
                                          ByRef Programmazione_Des As String,
                                          ByRef Programmazione_Des_Long As String,
                                          ByRef Piva As String,
                                          ByRef Sa_Cod As Integer,
                                          ByRef Note As String,
                                          ByRef Validita_Inizio As Date,
                                          ByRef Validita_Fine As Date,
                                          ByRef Tipo_Pianificazione As Integer,
                                          ByRef Dt_Appezzamenti As DataTable,
                                          ByRef Dt_Particelle As DataTable,
                                          ByRef TuttiCentri As Boolean,
                                                ByRef objParametri As AgronicaCoreParametri,
                                         Optional ByVal VerificaSeRibaltati As Boolean = False,
                                         Optional ByVal VerificaSeMovimentati As Boolean = False,
                                         Optional ByVal VerificaSeProvenientiDaFascicolo As Boolean = False,
                                         Optional ByVal VerificaSeGIS As Boolean = False
                                          ) As Integer


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Programmazione_R.Pianificazione_Leggi()"

        Dim FlagConnessioneLocale As Boolean = False
        Dim xConnessione As DbConnection = Nothing
        Dim xConnectionState As ConnectionState = ConnectionState.Closed
        Dim xTransazione As DbTransaction

        Dim Dt_Testata As New DataTable
        Dim Dt_Entita As New DataTable
        Dim Dt_EntitaxParticelle As New DataTable
        Dim Programmazione_Entita_Cod As Integer
        Dim i, j As Integer
        Dim Matrice(0, 12) As Object
        Dim ArrayEntita(120) As Object
        Dim strCentri As String()
        Dim ErrMSG As String = ""
        Dim MessaggioErrore As String
        Dim DrPart As DataRow()

        TuttiCentri = True

        Dim Fonte_Cod As Integer = 0

        Dim Ribaltato As Integer
        Dim Movimentato As Integer

        Dim Allegati_Documenti_Numero As String = ""
        Dim Allegati_Documenti_Numero_E As String = ""

        Try

            '------------------------------
            'Verifico se e' stata impostata una connessione
            If IsNothing(objParametri.objConnessione) Then
                'Flag
                FlagConnessioneLocale = True
                'Creo la connessione localmente
                xConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
                xTransazione = Nothing
            Else
                'Utilizzo quella passata come parametro
                xConnessione = objParametri.objConnessione
                xConnectionState = objParametri.objConnessione.State
                xTransazione = objParametri.objTransazione
            End If



            '----------------------------------------------
            'Leggo i dati della Testata
            Dim objProgrammazione_Testata As New AgronicaCoreAnagrafeDAL.Programmazione_Testata_R

            Dt_Testata = objProgrammazione_Testata.Leggi(
                                       ErrMSG,
                                       Programmazione_Cod,
                                       "",
                                       "",
                                       1,
                                       Validita_Inizio,
                                       Validita_Fine,
                                       enum_TipoRicetta.Non_Filtrare,
                                       enumSelezioneVariabile.Selezione_TabellaCompleta,
                                       "",
                                       "",
                                       objParametri)

            If Dt_Testata.Rows.Count > 0 Then

                Programmazione_Des = Dt_Testata.Rows(0).Item("Programmazione_Des").ToString
                Programmazione_Des_Long = ""
                Piva = Dt_Testata.Rows(0).Item("Piva").ToString
                Note = Dt_Testata.Rows(0).Item("Note").ToString

                'Validita_Inizio = CDate(Dt_Testata.Rows(0).Item("Validita_Inizio"))
                'Validita_Fine = CDate(Dt_Testata.Rows(0).Item("Validita_Fine"))
                Tipo_Pianificazione = CInt(Dt_Testata.Rows(0).Item("Tipo_Pianificazione"))
                Fonte_Cod = CInt(IIf(IsDBNull(Dt_Testata.Rows(0).Item("Fonte_Cod")), 0, Dt_Testata.Rows(0).Item("Fonte_Cod")))


                'Leggo i dati delle Entita
                Dim objProgrammazione_Entita As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_R
                Dt_Entita = objProgrammazione_Entita.Programmazione_Entita_Leggi(
                                            Programmazione_Cod,
                                            ErrMSG,
                                            0,
                                            "",
                                            "",
                                            0,
                                            0,
                                            0,
                                            0,
                                            0,
                                            Validita_Inizio,
                                            Validita_Fine,
                                            1,
                                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                                            "",
                                            "",
                                            objParametri,
                                            Fonte_Cod)

                'Leggo se le entità sono già state ribaltate e/o movimentate
                Dim objERib As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Programmazioni_R
                Dim objReg_Impianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Programmazioni_R
                Dim objGis As New AgronicaCoreGisDAL.GIS_Entita_R
                Dim objERibDAL As New AgronicaCoreAnagrafeBIZ.Reg_Impianto_R
                Dim DtERib As New DataTable
                Dim DrERib As DataRow()
                Dim DtEMov As New DataTable
                Dim DrEMov As DataRow()
                Dim listEntita_Gis As New List(Of Integer)

                Dim Gis_su_Campo As Boolean = False

                Dim objEFasc As New AgronicaCoreAnagrafeDAL.Allegati_EntitaxDocumenti_R
                Dim DtEFasc As New DataTable
                Dim DrEFasc As DataRow()

                If VerificaSeRibaltati Then
                    DtERib = objERib.Leggi("", 0, 0, 0, 0, CInt(Programmazione_Cod), 0, "", "", objParametri)
                End If
                If VerificaSeMovimentati Then
                    DtEMov = objERib.Leggi_OperazioniImpiantiRibaltati_DaProgrammazione("", CInt(Programmazione_Cod), 0, "", "", objParametri)
                End If
                If VerificaSeGIS Then
                    'Controllo se c'è GIS sulla Programmazione
                    'For Each row In objGis.Leggi(objParametri.PivaSuperUser, 0, 0, Piva, Sa_Cod, 0, 0, 0, "", "", "0", 0, 0, "0", 0, 0, CInt(Programmazione_Cod), 0, "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri).Rows
                    '    listEntita_Gis.Add(row.item("Programmazione_Entita_Cod"))
                    'Next


                    'Controllo se c'è GIS sul CAMPO
                    'For Each row As DataRow In objERib.Leggi(Piva, 0, 0, 0, 0, CInt(Programmazione_Cod), 0, "", "", objParametri).Rows
                    '    Dim DT = objGis.Leggi("", 0, 0, row.Item("piva"), row.Item("sa_cod"), 0, row.Item("Campo_Cod"), 0, "", "", "", 0, 0, "", 0, 0, 0, 0, "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)
                    '    If DT.Select(" Appezza = 0 AND ID_Imp = 0 AND Programmazione_Entita_Cod = 0 ").CopyToDataTable.Rows.Count > 0 Then
                    '        Gis_su_Campo = True
                    '    End If
                    'Next

                    'Controllo se c'è GIS su APPEZZA o ID_REG
                    For Each row As DataRow In objERib.Leggi(Piva, 0, 0, 0, 0, CInt(Programmazione_Cod), 0, "", "", objParametri).Rows
                        For Each row_entita In objGis.LeggiDB(objParametri.PivaSuperUser, 0, 0, row.Item("piva"), row.Item("sa_cod"), row.Item("appezza"), 0, 0, "", "", "", 0, 0, "", 0, 0, 0, 0, "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri).Rows
                            If Not listEntita_Gis.Contains(row.Item("Programmazione_Entita_Cod")) Then
                                listEntita_Gis.Add(row.Item("Programmazione_Entita_Cod"))
                            End If
                        Next
                    Next
                End If
                If VerificaSeProvenientiDaFascicolo Then
                    DtEFasc = objEFasc.Leggi(0, Piva, 0, 0, 0, 0, 0, "", "", "", 0, 0, "", "", "", 0, CInt(Programmazione_Cod), 0, "", "", objParametri)
                    If DtEFasc IsNot Nothing AndAlso DtEFasc.Rows.Count > 0 Then
                        Allegati_Documenti_Numero = DtEFasc.Rows(0).Item("Allegati_Documenti_Numero")
                    End If
                End If


                Dim objDP As New AgronicaCoreDataProvider.DatatableUtility
                strCentri = objDP.SelectDistinct(Dt_Entita, "Sa_Cod")

                'If Not strCentri Is Nothing AndAlso strCentri.Length > 0 AndAlso strCentri.Length = 1 Then
                '    TuttiCentri = False
                'End If

                Dim objProgrammazione_Particelle As New AgronicaCoreAnagrafeDAL.Programmazione_Particelle_R
                Dt_EntitaxParticelle = objProgrammazione_Particelle.Programmazione_Particelle_Leggi(objParametri,
                                                                                  Piva,
                                                                                  ErrMSG,
                                                                                  Programmazione_Cod)

                For i = 0 To Dt_Entita.Rows.Count - 1

                    Dim Catasto As String = ""

                    Sa_Cod = Dt_Entita.Rows(i).Item("sa_cod")

                    Programmazione_Entita_Cod = CInt(Dt_Entita.Rows(i).Item("Programmazione_Entita_Cod"))

                    'Leggo le particelle associate all'Entita
                    'Dim objProgrammazione_Particelle As New AgronicaCoreAnagrafeDAL.Programmazione_Particelle_R
                    'Dt_EntitaxParticelle = objProgrammazione_Particelle.Programmazione_Particelle_Leggi(objParametri,
                    '                                                              Piva,
                    '                                                              ErrMSG,
                    '                                                              0, Programmazione_Entita_Cod)

                    DrPart = Dt_EntitaxParticelle.Select("Programmazione_Entita_Cod=" & Programmazione_Entita_Cod.ToString)

                    If DrPart IsNot Nothing AndAlso DrPart.Length > 0 Then

                        For j = 0 To DrPart.Length - 1


                            Dim SupCatastale As Decimal = 0
                            SupCatastale = Ettari_from_EttariAreCentiare(Dt_EntitaxParticelle.Rows(j).Item("ETTARI"),
                                                                                                                  Dt_EntitaxParticelle.Rows(j).Item("ARE"),
                                                                                                                  Dt_EntitaxParticelle.Rows(j).Item("CENTIARE"))
                            DT_Intersezioni_Insert(Dt_Particelle,
                                                   Tipo_Pianificazione,
                                                   Piva,
                                                   Dt_Entita.Rows(i).Item("sa_cod"),
                                                   Dt_Entita.Rows(i).Item("campo_cod"),
                                                   Dt_Entita.Rows(i).Item("appezza"),
                                                   Dt_EntitaxParticelle.Rows(j).Item("Prov"),
                                                   Dt_EntitaxParticelle.Rows(j).Item("Com"),
                                                   Dt_EntitaxParticelle.Rows(j).Item("Prov_Des"),
                                                   Dt_EntitaxParticelle.Rows(j).Item("Com_Des"),
                                                   Dt_EntitaxParticelle.Rows(j).Item("Sezione"),
                                                   Dt_EntitaxParticelle.Rows(j).Item("Foglio"),
                                                   Dt_EntitaxParticelle.Rows(j).Item("Numero"),
                                                   Dt_EntitaxParticelle.Rows(j).Item("Subalterno"),
                                                   Dt_EntitaxParticelle.Rows(j).Item("Sup_Totale"),
                                                   Dt_EntitaxParticelle.Rows(j).Item("Superficie"),
                                                   SupCatastale,
                                                   Dt_EntitaxParticelle.Rows(j).Item("Validita_Inizio"),
                                                   Dt_EntitaxParticelle.Rows(j).Item("Validita_Fine"),
                                                   "Dal " & CDate(Dt_EntitaxParticelle.Rows(j).Item("Validita_Inizio")).ToString("dd/mm/yyyy") & " al " & CDate(Dt_EntitaxParticelle.Rows(j).Item("Validita_Fine")).ToString("dd/mm/yyyy"),
                                                   Dt_EntitaxParticelle.Rows(j).Item("TitoloPossesso"))

                            Catasto &= IIf(j <> 0, "<BR>", "") &
                                    Dt_EntitaxParticelle.Rows(j).Item("Prov") & "_" &
                                    Dt_EntitaxParticelle.Rows(j).Item("Com") & "_" &
                                    Dt_EntitaxParticelle.Rows(j).Item("Sezione") & "_" &
                                    Dt_EntitaxParticelle.Rows(j).Item("Foglio") & "_" &
                                    Dt_EntitaxParticelle.Rows(j).Item("Numero") & "_" &
                                    Dt_EntitaxParticelle.Rows(j).Item("Subalterno") &
                                    " (Sup." & CStr(Dt_EntitaxParticelle.Rows(j).Item("Superficie")) & " Ha)"

                        Next

                    End If
                    Dim Operazione_Cod As Integer = 0
                    Dim Operazione_Des As String = ""
                    If Not IsDBNull(Dt_Entita.Rows(i).Item("Operazione_Cod")) Then
                        Operazione_Cod = Dt_Entita.Rows(i).Item("Operazione_Cod")
                        Operazione_Des = OperazioneDes_From_OperazioneCod(Dt_Entita.Rows(i).Item("Operazione_Cod"))
                    End If

                    Dim veg_des_cliente As String = ""
                    Dim cul_des_cliente As String = ""
                    If Not IsNothing(Dt_Entita.Rows(i).Item("VegCul_Des_Cliente")) AndAlso
                    Not IsDBNull(Dt_Entita.Rows(i).Item("VegCul_Des_Cliente")) AndAlso
                    InStr(CStr(Dt_Entita.Rows(i).Item("VegCul_Des_Cliente")), "|") > 0 Then
                        veg_des_cliente = CStr(Dt_Entita.Rows(i).Item("VegCul_Des_Cliente")).Split("|")(0)
                        cul_des_cliente = CStr(Dt_Entita.Rows(i).Item("VegCul_Des_Cliente")).Split("|")(1)
                    End If


                    Ribaltato = 0
                    Movimentato = 0

                    If VerificaSeRibaltati Then
                        DrERib = DtERib.Select("Programmazione_Entita_Cod=" & Programmazione_Entita_Cod.ToString)
                        If DrERib IsNot Nothing AndAlso DrERib.Length > 0 Then
                            Ribaltato = 1
                        End If
                    End If

                    If VerificaSeMovimentati Then
                        DrEMov = DtEMov.Select("Programmazione_Entita_Cod=" & Programmazione_Entita_Cod.ToString)
                        If DrEMov IsNot Nothing AndAlso DrEMov.Length > 0 Then
                            Movimentato = 1
                        End If
                    End If

                    If VerificaSeGIS Then
                        If Gis_su_Campo Then
                            Movimentato = 1
                        Else
                            If listEntita_Gis.Contains(Programmazione_Entita_Cod) Then
                                Movimentato = 1
                            End If
                        End If
                    End If

                    Allegati_Documenti_Numero_E = ""
                    If VerificaSeProvenientiDaFascicolo Then
                        DrEFasc = DtEFasc.Select("Programmazione_Entita_Cod=" & Programmazione_Entita_Cod.ToString)
                        If DrEFasc IsNot Nothing AndAlso DrEFasc.Length > 0 Then
                            Allegati_Documenti_Numero_E = Allegati_Documenti_Numero
                        End If
                    End If

                    Dim Stato_Ribaltamento As Integer = 0
                    If Not IsDBNull(Dt_Entita.Rows(i).Item("Stato_Ribaltamento")) Then
                        Stato_Ribaltamento = Dt_Entita.Rows(i).Item("Stato_Ribaltamento")
                    End If

                    Dim Veg_Cod_Agea = ""
                    If Not IsDBNull(Dt_Entita.Rows(i).Item("Veg_Cod_Agea")) Then
                        Veg_Cod_Agea = Dt_Entita.Rows(i).Item("Veg_Cod_Agea")
                    End If

                    Dim Cul_Cod_Agea = ""
                    If Not IsDBNull(Dt_Entita.Rows(i).Item("Cul_Cod_Agea")) Then
                        Cul_Cod_Agea = Dt_Entita.Rows(i).Item("Cul_Cod_Agea")
                    End If

                    Dim Uso_Cod_Agea = ""
                    If Not IsDBNull(Dt_Entita.Rows(i).Item("Uso_Cod_Agea")) Then
                        Uso_Cod_Agea = Dt_Entita.Rows(i).Item("Uso_Cod_Agea")
                    End If

                    Dim Occupazione_Cod_Agea = ""
                    If Not IsDBNull(Dt_Entita.Rows(i).Item("Occupazione_Cod_Agea")) Then
                        Occupazione_Cod_Agea = Dt_Entita.Rows(i).Item("Occupazione_Cod_Agea")
                    End If

                    Dim Destinazione_Cod_Agea = ""
                    If Not IsDBNull(Dt_Entita.Rows(i).Item("Destinazione_Cod_Agea")) Then
                        Destinazione_Cod_Agea = Dt_Entita.Rows(i).Item("Destinazione_Cod_Agea")
                    End If

                    Dim Qualita_Cod_Agea = ""
                    If Not IsDBNull(Dt_Entita.Rows(i).Item("Qualita_Cod_Agea")) Then
                        Qualita_Cod_Agea = Dt_Entita.Rows(i).Item("Qualita_Cod_Agea")
                    End If

                    Dim Limite_N = ""
                    If Not IsDBNull(Dt_Entita.Rows(i).Item("Limite_N")) Then
                        Limite_N = Dt_Entita.Rows(i).Item("Limite_N")
                    End If

                    Dim Limite_P = ""
                    If Not IsDBNull(Dt_Entita.Rows(i).Item("Limite_P")) Then
                        Limite_P = Dt_Entita.Rows(i).Item("Limite_P")
                    End If

                    Dim Limite_K = ""
                    If Not IsDBNull(Dt_Entita.Rows(i).Item("Limite_K")) Then
                        Limite_K = Dt_Entita.Rows(i).Item("Limite_K")
                    End If

                    Dim Data_Fioritura_Prevista = AGRODATAINIZIO
                    If Not IsDBNull(Dt_Entita.Rows(i).Item("Data_Fioritura_Prevista")) Then
                        Data_Fioritura_Prevista = Dt_Entita.Rows(i).Item("Data_Fioritura_Prevista")
                    End If

                    Dim Veg_Cod_Prec = "0"
                    If Not IsDBNull(Dt_Entita.Rows(i).Item("Veg_Cod_Prec")) Then
                        Veg_Cod_Prec = Dt_Entita.Rows(i).Item("Veg_Cod_Prec")
                    End If

                    Dim Veg_Cod_Prec2 = "0"
                    If Not IsDBNull(Dt_Entita.Rows(i).Item("Veg_Cod_Prec2")) Then
                        Veg_Cod_Prec2 = Dt_Entita.Rows(i).Item("Veg_Cod_Prec2")
                    End If

                    Dim Veg_Cod_Prec3 = "0"
                    If Not IsDBNull(Dt_Entita.Rows(i).Item("Veg_Cod_Prec3")) Then
                        Veg_Cod_Prec3 = Dt_Entita.Rows(i).Item("Veg_Cod_Prec3")
                    End If

                    Dim Piano_Semina = ""
                    If Not IsDBNull(Dt_Entita.Rows(i).Item("Piano_Semina")) Then
                        Piano_Semina = Dt_Entita.Rows(i).Item("Piano_Semina")
                    End If

                    Dim Codice_Contratto = ""
                    If Not IsDBNull(Dt_Entita.Rows(i).Item("Codice_Contratto")) Then
                        Codice_Contratto = Dt_Entita.Rows(i).Item("Codice_Contratto")
                    End If

                    'Dim provenienza_fascicolo As String = ""
                    'Dim fascicoloAllegatiLettura As New AgronicaCoreAnagrafeDAL.Allegati_EntitaxDocumenti_R
                    'Dim DT = fascicoloAllegatiLettura.Leggi(0, Piva, 0, 0, 0, 0, 0, "", "", "", 0, 0, "", "", "", 0, Programmazione_Cod, 0, "", "", objParametri)
                    'If DT IsNot Nothing AndAlso DT.Rows.Count > 0 Then
                    '    provenienza_fascicolo = DT.Rows(0).Item("Allegati_Documenti_Numero")
                    'End If

                    If IsDBNull(Dt_Entita.Rows(i).Item("IAF")) Then
                        Dt_Entita.Rows(i).Item("IAF") = ""
                    End If

                    DT_Appezzamenti_Insert(Dt_Appezzamenti,
                       1,
                       Operazione_Cod,
                       Operazione_Des,
                       Piva,
                       Dt_Entita.Rows(i).Item("sa_cod"),
                       Dt_Entita.Rows(i).Item("campo_cod"),
                       Dt_Entita.Rows(i).Item("appezza"),
                       Dt_Entita.Rows(i).Item("id_reg"),
                       Dt_Entita.Rows(i).Item("progetto_cod"),
                       Dt_Entita.Rows(i).Item("progetto_des"),
                       Dt_Entita.Rows(i).Item("entita_des"),
                       Dt_Entita.Rows(i).Item("Superficie"),
                       Dt_Entita.Rows(i).Item("veg_cod"),
                       Dt_Entita.Rows(i).Item("veg_des"),
                       Dt_Entita.Rows(i).Item("cul_cod"),
                       Dt_Entita.Rows(i).Item("cul_des"),
                       Dt_Entita.Rows(i).Item("grva_cod"),
                       Dt_Entita.Rows(i).Item("grva_des"),
                       Dt_Entita.Rows(i).Item("grfi_cod"),
                       Dt_Entita.Rows(i).Item("grfi_des"),
                       Dt_Entita.Rows(i).Item("cop_cod"),
                       Dt_Entita.Rows(i).Item("cop_des"),
                       Dt_Entita.Rows(i).Item("resa"),
                       Dt_Entita.Rows(i).Item("id_cod"),
                       Dt_Entita.Rows(i).Item("destinazioneuso_des"),
                       Dt_Entita.Rows(i).Item("num_piante"),
                       Dt_Entita.Rows(i).Item("tra_fila"),
                       Dt_Entita.Rows(i).Item("su_fila"),
                       Dt_Entita.Rows(i).Item("validita_inizio_Impianto"),
                       Dt_Entita.Rows(i).Item("validita_inizio"),
                       Dt_Entita.Rows(i).Item("validita_fine"),
                       Dt_Entita.Rows(i).Item("Programmazione_Entita_Cod"),
                       Dt_Entita.Rows(i).Item("Data_Semina"),
                       Dt_Entita.Rows(i).Item("Data_Raccolta"),
                       Dt_Entita.Rows(i).Item("TipoZona"),
                       Dt_Entita.Rows(i).Item("MetodoProduzione_Cod"),
                       Dt_Entita.Rows(i).Item("MetodoProduzione_Des"),
                       Catasto,
                       Dt_Entita.Rows(i).Item("sa_nome"),
                       Dt_Entita.Rows(i).Item("campo_des"),
                       Dt_Entita.Rows(i).Item("veg_cod_cliente"),
                       Dt_Entita.Rows(i).Item("cul_cod_cliente"),
                       veg_des_cliente,
                       cul_des_cliente,
                       Dt_Entita.Rows(i).Item("macrouso_cod"),
                       Dt_Entita.Rows(i).Item("macrouso_des"),
                       Dt_Entita.Rows(i).Item("unita_vitata"),
                       objParametri,
                       ElementoGrafico_Cod:=Dt_Entita.Rows(i).Item("ElementoGrafico_Cod"),
                       ElementoGrafico_Des:=Dt_Entita.Rows(i).Item("ElementoGrafico_des"),
                       Ribaltato:=Ribaltato,
                       Movimentato:=Movimentato,
                        Veg_Cod_Agea:=Veg_Cod_Agea,
                        Cul_Cod_Agea:=Cul_Cod_Agea,
                        Uso_Cod_Agea:=Uso_Cod_Agea,
                        Occupazione_Cod_Agea:=Occupazione_Cod_Agea,
                        Destinazione_Cod_Agea:=Destinazione_Cod_Agea,
                        Qualita_Cod_Agea:=Qualita_Cod_Agea,
                        Limite_N:=Limite_N,
                        Limite_P:=Limite_P,
                        Limite_K:=Limite_K,
                        Data_Fioritura_Prevista:=Data_Fioritura_Prevista,
                        Veg_Cod_Prec:=Veg_Cod_Prec,
                        Veg_Cod_Prec2:=Veg_Cod_Prec2,
                        Veg_Cod_Prec3:=Veg_Cod_Prec3,
                        Piano_Semina:=Piano_Semina,
                        Codice_Contratto:=Codice_Contratto,
                        Disciplinare_Cod:=Dt_Entita.Rows(i).Item("Disciplinare_Cod"),
                        Regolamento_Cod:=Dt_Entita.Rows(i).Item("Regolamento_Cod"),
                        Stato_Cod:=Dt_Entita.Rows(i).Item("Stato_Cod"),
                        Stato_Ribaltamento:=Stato_Ribaltamento,
                        provenienza_fascicolo:=Allegati_Documenti_Numero_E,
                        Codice_Fiscale_Tecnico:=Dt_Entita.Rows(i).Item("Codice_Fiscale_Tecnico"),
                        datoGis:=Dt_Entita.Rows(i).Item("datoGis"),
                        IAF:=Dt_Entita.Rows(i).Item("IAF")
                       )

                Next

            End If


        Catch ex As Exception

            Programmazione_Cod = 0
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        Finally

            If FlagConnessioneLocale Then
                If Not IsNothing(xConnessione) Then
                    xConnessione.Close()
                    xConnessione.Dispose()
                End If
            Else
                If xConnectionState = ConnectionState.Closed Then
                    If Not IsNothing(xConnessione) Then
                        xConnessione.Close()
                    End If
                End If
            End If

        End Try

        'Restituisco il risultato
        Return Programmazione_Cod

    End Function

    '################################################################################
    'a differenza della precedente legge anche le entita eliminate
    Public Function Pianificazione_Leggi(ByVal Programmazione_Cod As Integer,
                                          ByRef Programmazione_Des As String,
                                          ByRef Programmazione_Des_Long As String,
                                          ByRef Piva As String,
                                          ByRef Sa_Cod As Integer,
                                          ByRef Note As String,
                                          ByRef Validita_Inizio As Date,
                                          ByRef Validita_Fine As Date,
                                          ByRef Tipo_Pianificazione As Integer,
                                          ByRef Dt_Appezzamenti As DataTable,
                                          ByRef Dt_Particelle As DataTable,
                                          ByRef Dt_Appezzamenti_Eliminati As DataTable,
                                          ByRef TuttiCentri As Boolean,
                                          ByRef NumeroDiValidazione As String,
                                          ByRef DataDiValidazione As Date,
                                          ByRef Allegati_Documenti_NomeFile As String,
                                          ByRef objParametri As AgronicaCoreParametri,
                                          Optional ByVal xFiltroAggiuntivo_Programmazione_entita As String = "",
                                          Optional ByRef Stato_SQNPI As Integer = 0,
                                          Optional ByRef Fonte_Cod As Integer = 0,
                                         Optional ByVal VerificaSeRibaltati As Boolean = False,
                                         Optional ByVal VerificaSeMovimentati As Boolean = False
                                          ) As Integer


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Programmazione_R.Pianificazione_Leggi()"

        Dim FlagConnessioneLocale As Boolean = False
        Dim xConnessione As DbConnection = Nothing
        Dim xConnectionState As ConnectionState = ConnectionState.Closed
        Dim xTransazione As DbTransaction

        Dim Dt_Testata As New DataTable
        Dim Dt_Entita As New DataTable
        Dim Dt_Entita_Eliminate As New DataTable
        Dim Dt_EntitaxParticelle As New DataTable
        Dim Programmazione_Entita_Cod As Integer
        Dim i, j As Integer
        Dim Matrice(0, 12) As Object
        Dim ArrayEntita(120) As Object
        Dim strCentri As String()
        Dim ErrMSG As String = ""
        Dim MessaggioErrore As String

        TuttiCentri = True

        Dim Ribaltato As Integer
        Dim Movimentato As Integer

        Try

            '------------------------------
            'Verifico se e' stata impostata una connessione
            If IsNothing(objParametri.objConnessione) Then
                'Flag
                FlagConnessioneLocale = True
                'Creo la connessione localmente
                xConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
                xTransazione = Nothing
            Else
                'Utilizzo quella passata come parametro
                xConnessione = objParametri.objConnessione
                xConnectionState = objParametri.objConnessione.State
                xTransazione = objParametri.objTransazione
            End If



            '----------------------------------------------
            'Leggo i dati della Testata
            Dim objProgrammazione_Testata As New AgronicaCoreAnagrafeDAL.Programmazione_Testata_R

            Dt_Testata = objProgrammazione_Testata.Leggi(
                                       ErrMSG,
                                       Programmazione_Cod,
                                       "",
                                       "",
                                       1,
                                       Estremo_Validita_Inizio,
                                       Estremo_Validita_Fine,
                                       enum_TipoRicetta.Non_Filtrare,
                                       enumSelezioneVariabile.Selezione_TabellaCompleta,
                                       "",
                                       "",
                                       objParametri)

            If Dt_Testata.Rows.Count > 0 Then

                Programmazione_Des = Dt_Testata.Rows(0).Item("Programmazione_Des").ToString
                Programmazione_Des_Long = ""
                Piva = Dt_Testata.Rows(0).Item("Piva").ToString
                Note = Dt_Testata.Rows(0).Item("Note").ToString
                Validita_Inizio = CDate(Dt_Testata.Rows(0).Item("Validita_Inizio"))
                Validita_Fine = CDate(Dt_Testata.Rows(0).Item("Validita_Fine"))
                Tipo_Pianificazione = CInt(Dt_Testata.Rows(0).Item("Tipo_Pianificazione"))
                NumeroDiValidazione = Dt_Testata.Rows(0).Item("Allegati_Documenti_Numero")
                DataDiValidazione = Dt_Testata.Rows(0).Item("Validazione_Data")
                Allegati_Documenti_NomeFile = Dt_Testata.Rows(0).Item("Allegati_Documenti_NomeFile")
                Stato_SQNPI = CInt(IIf(IsDBNull(Dt_Testata.Rows(0).Item("Stato_SQNPI")), 0, Dt_Testata.Rows(0).Item("Stato_SQNPI")))
                Fonte_Cod = CInt(IIf(IsDBNull(Dt_Testata.Rows(0).Item("Fonte_Cod")), 0, Dt_Testata.Rows(0).Item("Fonte_Cod")))

                'Leggo i dati delle Entita

                Dim objProgrammazione_Entita As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_R

                Dt_Entita = objProgrammazione_Entita.Programmazione_Entita_Leggi(
                                            Programmazione_Cod,
                                            ErrMSG,
                                            0,
                                            "",
                                            "",
                                            0,
                                            0,
                                            0,
                                            0,
                                            0,
                                            Estremo_Validita_Inizio,
                                            Estremo_Validita_Fine,
                                            1,
                                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                                            xFiltroAggiuntivo_Programmazione_entita,
                                            "",
                                            objParametri,
                                            Fonte_Cod)

                'Leggo se le entità sono già state ribaltate e/o movimentate
                Dim objERib As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Programmazioni_R
                Dim DtERib As New DataTable
                Dim DrERib As DataRow()
                Dim DtEMov As New DataTable
                Dim DrEMov As DataRow()
                Dim DrPart As DataRow()

                If VerificaSeRibaltati Then
                    DtERib = objERib.Leggi("", 0, 0, 0, 0, CInt(Programmazione_Cod), 0, "", "", objParametri)
                End If
                If VerificaSeMovimentati Then
                    DtEMov = objERib.Leggi_OperazioniImpiantiRibaltati_DaProgrammazione("", CInt(Programmazione_Cod), 0, "", "", objParametri)
                End If

                Dim objDP As New AgronicaCoreDataProvider.DatatableUtility
                strCentri = objDP.SelectDistinct(Dt_Entita, "Sa_Cod")

                'If Not strCentri Is Nothing AndAlso strCentri.Length > 0 AndAlso strCentri.Length = 1 Then
                '    TuttiCentri = False
                'End If

                'Leggo tutte le particelle associate alle Entita
                Dim objProgrammazione_Particelle As New AgronicaCoreAnagrafeDAL.Programmazione_Particelle_R
                Dt_EntitaxParticelle = objProgrammazione_Particelle.Programmazione_Particelle_Leggi(objParametri,
                                                                                  Piva,
                                                                                  ErrMSG,
                                                                                  Programmazione_Cod)

                For i = 0 To Dt_Entita.Rows.Count - 1

                    Dim Catasto As String = ""

                    Sa_Cod = Dt_Entita.Rows(i).Item("sa_cod")

                    Programmazione_Entita_Cod = CInt(Dt_Entita.Rows(i).Item("Programmazione_Entita_Cod"))

                    DrPart = Dt_EntitaxParticelle.Select("Programmazione_Entita_Cod=" & Programmazione_Entita_Cod.ToString)

                    If DrPart IsNot Nothing AndAlso DrPart.Length > 0 Then

                        For j = 0 To DrPart.Length - 1

                            DT_Intersezioni_Insert(Dt_Particelle,
                                                   Tipo_Pianificazione,
                                                   Piva,
                                                   Dt_Entita.Rows(i).Item("sa_cod"),
                                                   Dt_Entita.Rows(i).Item("campo_cod"),
                                                   Dt_Entita.Rows(i).Item("appezza"),
                                                   DrPart(j).Item("Prov"),
                                                   DrPart(j).Item("Com"),
                                                   DrPart(j).Item("Prov_Des"),
                                                   DrPart(j).Item("Com_Des"),
                                                   DrPart(j).Item("Sezione"),
                                                   DrPart(j).Item("Foglio"),
                                                   DrPart(j).Item("Numero"),
                                                   DrPart(j).Item("Subalterno"),
                                                   DrPart(j).Item("Sup_Totale"),
                                                   DrPart(j).Item("Superficie"))

                            Catasto &= IIf(j <> 0, "<BR>", "") &
                                    DrPart(j).Item("Prov") & "_" &
                                    DrPart(j).Item("Com") & "_" &
                                    DrPart(j).Item("Sezione") & "_" &
                                    DrPart(j).Item("Foglio") & "_" &
                                    DrPart(j).Item("Numero") & "_" &
                                    DrPart(j).Item("Subalterno") &
                                    " (Sup." & CStr(DrPart(j).Item("Superficie")) & " Ha)"

                        Next

                    End If

                    'For j = 0 To Dt_EntitaxParticelle.Rows.Count - 1

                    '    DT_Intersezioni_Insert(Dt_Particelle,
                    '                           Tipo_Pianificazione,
                    '                           Piva,
                    '                           Dt_Entita.Rows(i).Item("sa_cod"),
                    '                           Dt_Entita.Rows(i).Item("campo_cod"),
                    '                           Dt_Entita.Rows(i).Item("appezza"),
                    '                           Dt_EntitaxParticelle.Rows(j).Item("Prov"),
                    '                           Dt_EntitaxParticelle.Rows(j).Item("Com"),
                    '                           Dt_EntitaxParticelle.Rows(j).Item("Prov_Des"),
                    '                           Dt_EntitaxParticelle.Rows(j).Item("Com_Des"),
                    '                           Dt_EntitaxParticelle.Rows(j).Item("Sezione"),
                    '                           Dt_EntitaxParticelle.Rows(j).Item("Foglio"),
                    '                           Dt_EntitaxParticelle.Rows(j).Item("Numero"),
                    '                           Dt_EntitaxParticelle.Rows(j).Item("Subalterno"),
                    '                           Dt_EntitaxParticelle.Rows(j).Item("Sup_Totale"),
                    '                           Dt_EntitaxParticelle.Rows(j).Item("Superficie"))

                    '    Catasto &= IIf(j <> 0, "<BR>", "") &
                    '            Dt_EntitaxParticelle.Rows(j).Item("Prov") & "_" &
                    '            Dt_EntitaxParticelle.Rows(j).Item("Com") & "_" &
                    '            Dt_EntitaxParticelle.Rows(j).Item("Sezione") & "_" &
                    '            Dt_EntitaxParticelle.Rows(j).Item("Foglio") & "_" &
                    '            Dt_EntitaxParticelle.Rows(j).Item("Numero") & "_" &
                    '            Dt_EntitaxParticelle.Rows(j).Item("Subalterno") &
                    '            " (Sup." & CStr(Dt_EntitaxParticelle.Rows(j).Item("Superficie")) + " Ha)"

                    'Next

                    Dim Operazione_Cod As Integer = 0
                    Dim Operazione_Des As String = ""
                    If Not IsDBNull(Dt_Entita.Rows(i).Item("Operazione_Cod")) Then
                        Operazione_Cod = Dt_Entita.Rows(i).Item("Operazione_Cod")
                        Operazione_Des = OperazioneDes_From_OperazioneCod(Dt_Entita.Rows(i).Item("Operazione_Cod"))
                    End If

                    Dim veg_des_cliente As String = ""
                    Dim cul_des_cliente As String = ""
                    If Not IsNothing(Dt_Entita.Rows(i).Item("VegCul_Des_Cliente")) AndAlso
                        Not IsDBNull(Dt_Entita.Rows(i).Item("VegCul_Des_Cliente")) AndAlso
                        InStr(CStr(Dt_Entita.Rows(i).Item("VegCul_Des_Cliente")), "|") > 0 Then
                        veg_des_cliente = CStr(Dt_Entita.Rows(i).Item("VegCul_Des_Cliente")).Split("|")(0)
                        cul_des_cliente = CStr(Dt_Entita.Rows(i).Item("VegCul_Des_Cliente")).Split("|")(1)
                    End If

                    Ribaltato = 0
                    Movimentato = 0

                    If VerificaSeRibaltati Then
                        DrERib = DtERib.Select("Programmazione_Entita_Cod=" & Programmazione_Entita_Cod.ToString)
                        If DrERib IsNot Nothing AndAlso DrERib.Length > 0 Then
                            Ribaltato = 1
                        End If
                    End If

                    If VerificaSeMovimentati Then
                        DrEMov = DtEMov.Select("Programmazione_Entita_Cod=" & Programmazione_Entita_Cod.ToString)
                        If DrEMov IsNot Nothing AndAlso DrEMov.Length > 0 Then
                            Movimentato = 1
                        End If
                    End If

                    DT_Appezzamenti_Insert(Dt_Appezzamenti,
                       1,
                       Operazione_Cod,
                       Operazione_Des,
                       Piva,
                       Dt_Entita.Rows(i).Item("sa_cod"),
                       Dt_Entita.Rows(i).Item("campo_cod"),
                       Dt_Entita.Rows(i).Item("appezza"),
                       Dt_Entita.Rows(i).Item("id_reg"),
                       Dt_Entita.Rows(i).Item("progetto_cod"),
                       Dt_Entita.Rows(i).Item("progetto_des"),
                       Dt_Entita.Rows(i).Item("entita_des"),
                       Dt_Entita.Rows(i).Item("Superficie"),
                       Dt_Entita.Rows(i).Item("veg_cod"),
                       Dt_Entita.Rows(i).Item("veg_des"),
                       Dt_Entita.Rows(i).Item("cul_cod"),
                       Dt_Entita.Rows(i).Item("cul_des"),
                       Dt_Entita.Rows(i).Item("grva_cod"),
                       Dt_Entita.Rows(i).Item("grva_des"),
                       Dt_Entita.Rows(i).Item("grfi_cod"),
                       Dt_Entita.Rows(i).Item("grfi_des"),
                       Dt_Entita.Rows(i).Item("cop_cod"),
                       Dt_Entita.Rows(i).Item("cop_des"),
                       Dt_Entita.Rows(i).Item("resa"),
                       Dt_Entita.Rows(i).Item("id_cod"),
                       Dt_Entita.Rows(i).Item("destinazioneuso_des"),
                       Dt_Entita.Rows(i).Item("num_piante"),
                       Dt_Entita.Rows(i).Item("tra_fila"),
                       Dt_Entita.Rows(i).Item("su_fila"),
                       Dt_Entita.Rows(i).Item("validita_inizio_Impianto"),
                       Dt_Entita.Rows(i).Item("validita_inizio"),
                       Dt_Entita.Rows(i).Item("validita_fine"),
                       Dt_Entita.Rows(i).Item("Programmazione_Entita_Cod"),
                       Dt_Entita.Rows(i).Item("Data_Semina"),
                       Dt_Entita.Rows(i).Item("Data_Raccolta"),
                       Dt_Entita.Rows(i).Item("TipoZona"),
                       Dt_Entita.Rows(i).Item("MetodoProduzione_Cod"),
                       Dt_Entita.Rows(i).Item("MetodoProduzione_Des"),
                       Catasto,
                       Dt_Entita.Rows(i).Item("sa_nome"),
                       Dt_Entita.Rows(i).Item("campo_des"),
                       Dt_Entita.Rows(i).Item("veg_cod_cliente"),
                       Dt_Entita.Rows(i).Item("cul_cod_cliente"),
                       veg_des_cliente,
                       cul_des_cliente,
                       Dt_Entita.Rows(i).Item("Macrouso_Cod"),
                       Dt_Entita.Rows(i).Item("Macrouso_Des"),
                       Dt_Entita.Rows(i).Item("unita_vitata"),
                       objParametri,
                       Dt_Entita.Rows(i).Item("ElementoGrafico_Cod"),
                       Dt_Entita.Rows(i).Item("ElementoGrafico_des"),
                                           Ribaltato, Movimentato, PivaReale:=Dt_Entita.Rows(i).Item("partitaIvaReale")
                        )

                Next

                Dim objProgrammazione_Entita_Eliminate_R As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_Eliminate_R

                Dt_Entita_Eliminate = objProgrammazione_Entita_Eliminate_R.Leggi(
                                            Programmazione_Cod,
                                            ErrMSG,
                                            0,
                                            Estremo_Validita_Inizio,
                                            Estremo_Validita_Fine,
                                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                                            "",
                                            "",
                                            objParametri)

                If Dt_Entita_Eliminate.Rows.Count > 0 Then

                    Dim Sa_Nome As String
                    Dim Campo_Des As String
                    Dim App_nome As String
                    Dim Sup_App As String
                    Dim Veg_Des As String
                    Dim Cul_Des As String
                    Dim Grfi_Des As String
                    Dim Progetto_Nome As String
                    Dim DestinazioneUso_Des As String

                    '-----------------------------------
                    'recupero i dati degli impianti
                    Dim Pive As String()
                    Dim SaCod As Integer()
                    Dim Appezza As Integer()
                    Dim IdReg As Integer()
                    Dim N_Impianti As Integer = 0
                    For i = 0 To Dt_Entita_Eliminate.Rows.Count - 1
                        ReDim Preserve Pive(N_Impianti)
                        ReDim Preserve SaCod(N_Impianti)
                        ReDim Preserve Appezza(N_Impianti)
                        ReDim Preserve IdReg(N_Impianti)
                        Pive(N_Impianti) = Dt_Entita_Eliminate.Rows(i).Item("Piva")
                        SaCod(N_Impianti) = Dt_Entita_Eliminate.Rows(i).Item("Sa_Cod")
                        Appezza(N_Impianti) = Dt_Entita_Eliminate.Rows(i).Item("Appezza")
                        IdReg(N_Impianti) = Dt_Entita_Eliminate.Rows(i).Item("Id_Reg")
                        N_Impianti += 1
                    Next

                    Dim objImpianto As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                    Dim DtImpianti As DataTable
                    Dim DrImpianto As DataRow()
                    DtImpianti = objImpianto.Leggi_Dati_Impianti_Distinte(Pive, SaCod, Appezza, IdReg, "", " Imprese_Progetti.Validita_Fine desc ", objParametri)


                    '------------------------------------------------
                    'riempio il dt_entita_eliminate
                    For i = 0 To Dt_Entita_Eliminate.Rows.Count - 1

                        Sa_Nome = ""
                        Campo_Des = ""
                        App_nome = ""
                        Sup_App = ""
                        Veg_Des = ""
                        Cul_Des = ""
                        Grfi_Des = ""
                        Progetto_Nome = ""
                        DestinazioneUso_Des = ""

                        If DtImpianti IsNot Nothing AndAlso DtImpianti.Rows.Count > 0 Then
                            If Dt_Entita_Eliminate.Rows(i).Item("id_reg") <> 0 Then
                                DrImpianto = DtImpianti.Select("Piva='" & Dt_Entita_Eliminate.Rows(i).Item("Piva").ToString & "' " &
                                                               " AND sa_cod=" & Dt_Entita_Eliminate.Rows(i).Item("Sa_Cod").ToString &
                                                               " AND appezza=" & Dt_Entita_Eliminate.Rows(i).Item("appezza").ToString &
                                                               " AND id_reg=" & Dt_Entita_Eliminate.Rows(i).Item("id_reg").ToString)
                            Else
                                DrImpianto = DtImpianti.Select("Piva='" & Dt_Entita_Eliminate.Rows(i).Item("Piva").ToString & "' " &
                                                               " AND sa_cod=" & Dt_Entita_Eliminate.Rows(i).Item("Sa_Cod").ToString &
                                                               " AND appezza=" & Dt_Entita_Eliminate.Rows(i).Item("appezza").ToString)
                            End If
                            If DrImpianto IsNot Nothing AndAlso DrImpianto.Length > 0 Then
                                Sa_Nome = DrImpianto(0).Item("Sa_Nome")
                                Campo_Des = DrImpianto(0).Item("Campo_Des")
                                App_nome = DrImpianto(0).Item("App_nome")
                                Sup_App = DrImpianto(0).Item("Sup_Imp")
                                Veg_Des = DrImpianto(0).Item("Veg_Des")
                                Cul_Des = DrImpianto(0).Item("Cul_Des")
                                Grfi_Des = DrImpianto(0).Item("Grfi_Des")
                                Progetto_Nome = DrImpianto(0).Item("Progetto_Nome")
                                DestinazioneUso_Des = DrImpianto(0).Item("DestinazioneUso")
                            End If
                        End If

                        DT_Appezzamenti_Eliminati_Insert(Dt_Appezzamenti_Eliminati,
                                                        Dt_Entita_Eliminate.Rows(i).Item("Piva"),
                                                        Dt_Entita_Eliminate.Rows(i).Item("Sa_Cod"),
                                                        Dt_Entita_Eliminate.Rows(i).Item("Campo_Cod"),
                                                        Dt_Entita_Eliminate.Rows(i).Item("Appezza"),
                                                        Dt_Entita_Eliminate.Rows(i).Item("Id_reg"),
                                                        Dt_Entita_Eliminate.Rows(i).Item("Progetto_Cod"),
                                                        Dt_Entita_Eliminate.Rows(i).Item("Programmazione_Entita_Cod"),
                                                        Dt_Entita_Eliminate.Rows(i).Item("Operazione_Cod"),
                                                        Dt_Entita_Eliminate.Rows(i).Item("validita_inizio"),
                                                        "",
                                                        "",
                                                        OperazioneDes_From_OperazioneCod(Dt_Entita_Eliminate.Rows(i).Item("Operazione_Cod")),
                                                        Sa_Nome,
                                                        Campo_Des,
                                                        App_nome,
                                                        Sup_App,
                                                        Veg_Des,
                                                        Cul_Des,
                                                        Grfi_Des,
                                                        Progetto_Nome,
                                                        DestinazioneUso_Des)
                    Next

                End If

            End If


        Catch ex As Exception

            Programmazione_Cod = 0
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        Finally

            If FlagConnessioneLocale Then
                If Not IsNothing(xConnessione) Then
                    xConnessione.Close()
                    xConnessione.Dispose()
                End If
            Else
                If xConnectionState = ConnectionState.Closed Then
                    If Not IsNothing(xConnessione) Then
                        xConnessione.Close()
                    End if
                End If
            End If

        End Try

        'Restituisco il risultato
        Return Programmazione_Cod

    End Function

    Public Function PianificazioneParticelle_Leggi(ByVal Programmazione_Cod As Integer,
                                                   ByRef Programmazione_Des As String,
                                                   ByRef Programmazione_Des_Long As String,
                                                   ByRef Piva As String,
                                                   ByRef Sa_Cod As Integer,
                                                   ByRef Note As String,
                                                   ByRef Validita_Inizio_Testata As Date,
                                                   ByRef Validita_Fine_Testata As Date,
                                                   ByRef Tipo_Pianificazione As Integer,
                                                   ByRef Dt_Appezzamenti As DataTable,
                                                   ByRef TuttiCentri As Boolean,
                                                        ByRef objParametri As AgronicaCoreParametri
                                                   ) As Integer

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Programmazione_R.PianificazioneParticelle_Leggi()"

        Dim FlagConnessioneLocale As Boolean = False
        Dim xConnessione As DbConnection = Nothing
        Dim xConnectionState As ConnectionState = ConnectionState.Closed
        Dim xTransazione As DbTransaction

        Dim Dt_Testata As New DataTable
        Dim Dt_Entita As New DataTable
        Dim Dt_EntitaxParticelle As New DataTable
        Dim Dt_Appezza As New DataTable

        Dim i, j As Integer
        Dim Matrice(0, 12) As Object
        Dim ArrayEntita(120) As Object
        Dim strCentri As String()

        Dim ErrMSG As String = ""
        Dim MessaggioErrore As String

        TuttiCentri = True

        Dim objUtility As AgronicaCoreDataProvider.UtilityProvider
        Dim objEntita As AgronicaCoreAnagrafeDAL.Programmazione_Entita_R
        Dim objProgrammazione_Testata_R As AgronicaCoreAnagrafeDAL.Programmazione_Testata_R


        Try


            '------------------------------
            'Verifico se e' stata impostata una connessione
            If IsNothing(objParametri.objConnessione) Then
                'Flag
                FlagConnessioneLocale = True
                'Creo la connessione localmente
                xConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
                xTransazione = Nothing
            Else
                'Utilizzo quella passata come parametro
                xConnessione = objParametri.objConnessione
                xConnectionState = objParametri.objConnessione.State
                xTransazione = objParametri.objTransazione
            End If


            'Leggo i dati della Testata

            objProgrammazione_Testata_R = New AgronicaCoreAnagrafeDAL.Programmazione_Testata_R

            Dt_Testata = objProgrammazione_Testata_R.Leggi(
                                       ErrMSG,
                                       Programmazione_Cod,
                                       "",
                                       "",
                                       1,
                                       Validita_Inizio_Testata,
                                       Validita_Fine_Testata,
                                       enum_TipoRicetta.Non_Filtrare,
                                       enumSelezioneVariabile.Selezione_TabellaCompleta,
                                       "",
                                       "",
                                       objParametri)

            If Dt_Testata.Rows.Count > 0 Then

                Programmazione_Des = Dt_Testata.Rows(0).Item("Programmazione_Des").ToString
                Programmazione_Des_Long = ""
                Piva = Dt_Testata.Rows(0).Item("Piva").ToString
                Note = Dt_Testata.Rows(0).Item("Note").ToString
                Validita_Inizio_Testata = CDate(Dt_Testata.Rows(0).Item("Validita_Inizio"))
                Validita_Fine_Testata = CDate(Dt_Testata.Rows(0).Item("Validita_Fine"))
                Tipo_Pianificazione = CInt(Dt_Testata.Rows(0).Item("Tipo_Pianificazione"))

                objEntita = New AgronicaCoreAnagrafeDAL.Programmazione_Entita_R

                'Leggo i dati delle Entita
                Dt_Entita = objEntita.Anagrafica_ParticellexProgrammazioneEntita_Leggi(Piva,
                                                                                    Programmazione_Cod,
                                                                                    ErrMSG,
                                                                                    "",
                                                                                    "",
                                                                                    objParametri)

                objEntita = Nothing

                objUtility = New AgronicaCoreDataProvider.UtilityProvider
                strCentri = objUtility.SelectDistinct(Dt_Entita, "Sa_Cod")

                If strCentri IsNot Nothing AndAlso strCentri.Length > 0 AndAlso strCentri.Length = 1 Then
                    TuttiCentri = False
                End If

                Dim Entita As Integer()
                Dim Appezza As Integer()
                Dim Id_Reg As Integer()
                Dim Sup_App As Decimal()
                Dim Veg_Cod As String()
                Dim Veg_Des As String()
                Dim Cul_Cod As String()
                Dim Cul_Des As String()
                Dim Grfi_Cod As Integer()
                Dim Grfi_Des As String()
                Dim DestinazioneUso_Cod As Integer()
                Dim DestinazioneUso_Des As String()
                Dim Validita_Inizio As String()
                'Dim Particella_Fine As String()

                Dim indice As Integer = 0
                Dim CampoCod_Old As Integer = 0


                For i = 0 To Dt_Entita.Rows.Count - 1

                    If Dt_Entita.Rows(i).Item("Campo_Cod") = 0 And Dt_Entita.Rows(i).Item("appezza") <> 0 Then

                        indice = 0

                        ReDim Preserve Entita(indice)
                        ReDim Preserve Appezza(indice)
                        ReDim Preserve Id_Reg(indice)
                        ReDim Preserve Sup_App(indice)
                        ReDim Preserve Veg_Cod(indice)
                        ReDim Preserve Veg_Des(indice)
                        ReDim Preserve Cul_Cod(indice)
                        ReDim Preserve Cul_Des(indice)
                        ReDim Preserve Grfi_Cod(indice)
                        ReDim Preserve Grfi_Des(indice)
                        ReDim Preserve DestinazioneUso_Cod(indice)
                        ReDim Preserve DestinazioneUso_Des(indice)
                        ReDim Preserve Validita_Inizio(indice)
                        'ReDim Preserve Particella_Fine(indice)

                        Entita(indice) = 0
                        Appezza(indice) = Dt_Entita.Rows(i).Item("appezza")
                        Id_Reg(indice) = Dt_Entita.Rows(i).Item("id_reg")
                        Sup_App(indice) = Dt_Entita.Rows(i).Item("superficie")
                        Veg_Cod(indice) = Dt_Entita.Rows(i).Item("veg_cod")
                        Veg_Des(indice) = Dt_Entita.Rows(i).Item("veg_des")
                        Cul_Cod(indice) = Dt_Entita.Rows(i).Item("cul_cod")
                        Cul_Des(indice) = Dt_Entita.Rows(i).Item("cul_des")
                        Grfi_Cod(indice) = Dt_Entita.Rows(i).Item("grfi_cod")
                        Grfi_Des(indice) = Dt_Entita.Rows(i).Item("grfi_des")
                        DestinazioneUso_Cod(indice) = Dt_Entita.Rows(i).Item("destinazioneuso_cod")
                        DestinazioneUso_Des(indice) = Dt_Entita.Rows(i).Item("destinazioneuso_des")
                        Validita_Inizio(indice) = IIf(CDate(Dt_Entita.Rows(i).Item("validita_inizio")) <> #1/1/1900#, CDate(Dt_Entita.Rows(i).Item("validita_inizio")).ToShortDateString, "")
                        'Particella_Fine(indice) = IIf(CDate(Dt_Entita.Rows(i).Item("particella_fine")) <> #1/1/1900#, CDate(Dt_Entita.Rows(i).Item("particella_fine")).ToShortDateString, "")


                    ElseIf Dt_Entita.Rows(i).Item("Campo_Cod") <> 0 And Dt_Entita.Rows(i).Item("appezza") = 0 Then

                        indice = 0

                        objEntita = New AgronicaCoreAnagrafeDAL.Programmazione_Entita_R

                        Dt_Appezza = objEntita.Anagrafica_AppezzaCampo_Leggi(Programmazione_Cod,
                                                                          Dt_Entita.Rows(i).Item("Campo_Cod"),
                                                                          ErrMSG,
                                                                          "",
                                                                          "",
                                                                          objParametri)

                        For j = 0 To Dt_Appezza.Rows.Count - 1

                            ReDim Preserve Entita(indice)
                            ReDim Preserve Appezza(indice)
                            ReDim Preserve Id_Reg(indice)
                            ReDim Preserve Sup_App(indice)
                            ReDim Preserve Veg_Cod(indice)
                            ReDim Preserve Veg_Des(indice)
                            ReDim Preserve Cul_Cod(indice)
                            ReDim Preserve Cul_Des(indice)
                            ReDim Preserve Grfi_Cod(indice)
                            ReDim Preserve Grfi_Des(indice)
                            ReDim Preserve DestinazioneUso_Cod(indice)
                            ReDim Preserve DestinazioneUso_Des(indice)
                            ReDim Preserve Validita_Inizio(indice)
                            'ReDim Preserve Particella_Fine(indice)

                            Entita(indice) = Dt_Appezza.Rows(j).Item("programmazione_entita_cod")
                            Appezza(indice) = Dt_Appezza.Rows(j).Item("appezza")
                            Id_Reg(indice) = Dt_Appezza.Rows(j).Item("id_reg")
                            Sup_App(indice) = Dt_Appezza.Rows(j).Item("superficie")
                            Veg_Cod(indice) = Dt_Appezza.Rows(j).Item("veg_cod")
                            Veg_Des(indice) = Dt_Appezza.Rows(j).Item("veg_des")
                            Cul_Cod(indice) = Dt_Appezza.Rows(j).Item("cul_cod")
                            Cul_Des(indice) = Dt_Appezza.Rows(j).Item("cul_des")
                            Grfi_Cod(indice) = Dt_Appezza.Rows(j).Item("grfi_cod")
                            Grfi_Des(indice) = Dt_Appezza.Rows(j).Item("grfi_des")
                            DestinazioneUso_Cod(indice) = Dt_Appezza.Rows(j).Item("destinazioneuso_cod")
                            DestinazioneUso_Des(indice) = Dt_Appezza.Rows(j).Item("destinazioneuso_des")
                            Validita_Inizio(indice) = IIf(CDate(Dt_Appezza.Rows(j).Item("validita_inizio")) <> #1/1/1900#, CDate(Dt_Appezza.Rows(j).Item("validita_inizio")).ToShortDateString, "")
                            'Particella_Fine(indice) = IIf(CDate(Dt_Entita.Rows(i).Item("particella_fine")) <> #1/1/1900#, CDate(Dt_Entita.Rows(i).Item("particella_fine")).ToShortDateString, "")

                            indice += 1
                        Next

                    End If

                    Dim StrSuperficie As String
                    Dim SupCatastale As Decimal
                    Dim SupCondotta As Decimal
                    Dim SupSeminabile As Decimal
                    Dim SupUnar As Decimal

                    SupCatastale = Ettari_from_EttariAreCentiare(Dt_Entita.Rows(i).Item("Ettari"), Dt_Entita.Rows(i).Item("Are"), Dt_Entita.Rows(i).Item("Centiare"))

                    StrSuperficie = Dt_Entita.Rows(i).Item("Sup_Condotta")
                    SupCondotta = CDbl(StrSuperficie)

                    StrSuperficie = Dt_Entita.Rows(i).Item("Sup_Seminabile")
                    SupSeminabile = CDbl(StrSuperficie)

                    StrSuperficie = Dt_Entita.Rows(i).Item("Sup_Unar")
                    SupUnar = CDbl(StrSuperficie)

                    DT_Appezzamenti_3Colture_Insert(Dt_Appezzamenti,
                                                  Dt_Entita.Rows(i).Item("Programmazione_Entita_Cod"),
                                                  Dt_Entita.Rows(i).Item("Sa_Cod"),
                                                  Dt_Entita.Rows(i).Item("Sa_Nome"),
                                                  Dt_Entita.Rows(i).Item("Campo_Cod"),
                                                  Dt_Entita.Rows(i).Item("Entita_Des"),
                                                  SupCatastale,
                                                  SupCondotta,
                                                  SupSeminabile,
                                                  SupUnar,
                                                  Entita,
                                                  Appezza,
                                                  Id_Reg,
                                                  Sup_App,
                                                  Veg_Cod,
                                                  Veg_Des,
                                                  Cul_Cod,
                                                  Cul_Des,
                                                  Grfi_Cod,
                                                  Grfi_Des,
                                                  DestinazioneUso_Cod,
                                                  DestinazioneUso_Des,
                                                  Validita_Inizio,
                                                  Dt_Entita.Rows(i).Item("PROV"),
                                                  Dt_Entita.Rows(i).Item("COM"),
                                                  Dt_Entita.Rows(i).Item("PROV_DES"),
                                                  Dt_Entita.Rows(i).Item("COM_DES"),
                                                  Dt_Entita.Rows(i).Item("Sezione"),
                                                  Dt_Entita.Rows(i).Item("Foglio"),
                                                  Dt_Entita.Rows(i).Item("Numero"),
                                                  Dt_Entita.Rows(i).Item("Subalterno"),
                                                  Dt_Entita.Rows(i).Item("Note"),
                                                  Dt_Entita.Rows(i).Item("Modifica"),
                                                  Dt_Entita.Rows(i).Item("Particella_Fine")
                                                  )


                    ' riazzero tutto per il prossimo inserimento
                    Array.Clear(Entita, 0, Entita.Length)
                    Array.Clear(Appezza, 0, Appezza.Length)
                    Array.Clear(Id_Reg, 0, Id_Reg.Length)
                    Array.Clear(Sup_App, 0, Sup_App.Length)
                    Array.Clear(Veg_Cod, 0, Veg_Cod.Length)
                    Array.Clear(Veg_Des, 0, Veg_Des.Length)
                    Array.Clear(Cul_Cod, 0, Cul_Cod.Length)
                    Array.Clear(Cul_Des, 0, Cul_Des.Length)
                    Array.Clear(Grfi_Cod, 0, Grfi_Cod.Length)
                    Array.Clear(Grfi_Des, 0, Grfi_Des.Length)
                    Array.Clear(DestinazioneUso_Cod, 0, DestinazioneUso_Cod.Length)
                    Array.Clear(DestinazioneUso_Des, 0, DestinazioneUso_Des.Length)
                    Array.Clear(Validita_Inizio, 0, Validita_Inizio.Length)

                    indice = 0

                Next

            End If
        Catch ex As Exception

            Programmazione_Cod = 0
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        Finally

            If FlagConnessioneLocale Then
                If Not IsNothing(xConnessione) Then
                    xConnessione.Close()
                    xConnessione.Dispose()
                End If
            Else
                If xConnectionState = ConnectionState.Closed Then
                    If Not IsNothing(xConnessione) Then
                        xConnessione.Close()
                    End If
                End If
            End If

        End Try

        'Restituisco il risultato
        Return Programmazione_Cod


    End Function

    Public Function PianificazioneParticelle_Leggi_New(ByVal Piva As String,
                                                   ByVal Programmazione_Cod As Integer,
                                                   ByRef Programmazione_Des As String,
                                                   ByRef Programmazione_Des_Long As String,
                                                   ByRef Sa_Cod As Integer,
                                                   ByRef Note As String,
                                                   ByRef Validita_Inizio_Testata As Date,
                                                   ByRef Validita_Fine_Testata As Date,
                                                   ByRef Tipo_Pianificazione As Integer,
                                                   ByRef Dt_Appezzamenti As DataTable,
                                                   ByRef TuttiCentri As Boolean,
                                                        ByRef objParametri As AgronicaCoreParametri,
                                                            Optional ByVal strFiltro As String = ""
                                                       ) As Integer


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Programmazione_R.PianificazioneParticelle_Leggi_New()"

        Dim FlagConnessioneLocale As Boolean = False
        Dim xConnessione As DbConnection = Nothing
        Dim xConnectionState As ConnectionState = ConnectionState.Closed
        Dim xTransazione As DbTransaction

        Dim ErrMSG As String = ""
        Dim MessaggioErrore As String

        Dim Dt_Testata As New DataTable
        Dim Dt_Entita As New DataTable
        Dim Dt_EntitaxParticelle As New DataTable
        Dim Dt_Appezza As DataTable

        Dim i, j As Integer
        Dim Matrice(0, 12) As Object
        Dim ArrayEntita(120) As Object
        Dim strCentri As String()

        TuttiCentri = True

        Try

            '------------------------------
            'Verifico se è stata impostata una connessione
            If IsNothing(objParametri.objConnessione) Then
                'Flag
                FlagConnessioneLocale = True
                'Creo la connessione localmente
                xConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
                xTransazione = Nothing
            Else
                'Utilizzo quella passata come parametro
                xConnessione = objParametri.objConnessione
                xConnectionState = objParametri.objConnessione.State
                xTransazione = objParametri.objTransazione
            End If


            '----------------------------------------------
            'Leggo i dati della Testata
            Dim objProgrammazione_Testata As New AgronicaCoreAnagrafeDAL.Programmazione_Testata_R

            Dt_Testata = objProgrammazione_Testata.Leggi(
                                       ErrMSG,
                                       Programmazione_Cod,
                                       Piva,
                                       "",
                                       1,
                                       Estremo_Validita_Inizio,
                                       Estremo_Validita_Fine,
                                       enum_TipoRicetta.Non_Filtrare,
                                       enumSelezioneVariabile.Selezione_TabellaCompleta,
                                       "", "",
                                       objParametri)

            If Dt_Testata.Rows.Count > 0 Then

                Programmazione_Des = Dt_Testata.Rows(0).Item("Programmazione_Des").ToString
                Programmazione_Des_Long = ""
                'Piva = Dt_Testata.Rows(0).Item("Piva").ToString
                Note = Dt_Testata.Rows(0).Item("Note").ToString
                Validita_Inizio_Testata = CDate(Dt_Testata.Rows(0).Item("Validita_Inizio"))
                Validita_Fine_Testata = CDate(Dt_Testata.Rows(0).Item("Validita_Fine"))
                Tipo_Pianificazione = CInt(Dt_Testata.Rows(0).Item("Tipo_Pianificazione"))

                'Leggo i dati delle Entita
                Dim objPE As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_R
                Dt_Entita = objPE.Anagrafica_ParticellexProgrammazioneEntita_Leggi(Piva,
                                                                                   Programmazione_Cod,
                                                                                   ErrMSG,
                                                                                    "", "", objParametri)
                Dim objDP As New AgronicaCoreDataProvider.DatatableUtility
                strCentri = objDP.SelectDistinct(Dt_Entita, "Sa_Cod")

                If strCentri IsNot Nothing AndAlso strCentri.Length > 0 AndAlso strCentri.Length = 1 Then
                    TuttiCentri = False
                End If

                Dim Entita As Integer()
                Dim Appezza As Integer()
                Dim Id_Reg As Integer()
                Dim Sup_App As Decimal()
                Dim Veg_Cod As String()
                Dim Veg_Des As String()
                Dim Cul_Cod As String()
                Dim Cul_Des As String()
                Dim Grfi_Cod As Integer()
                Dim Grfi_Des As String()
                Dim DestinazioneUso_Cod As Integer()
                Dim DestinazioneUso_Des As String()
                Dim Validita_Inizio As String()
                Dim Validita_Fine As String()

                Dim indice As Integer = 0
                Dim CampoCod_Old As Integer = 0

                For i = 0 To Dt_Entita.Rows.Count - 1

                    If Dt_Entita.Rows(i).Item("Campo_Cod") = 0 And Dt_Entita.Rows(i).Item("appezza") <> 0 Then

                        indice = 0

                        ReDim Preserve Entita(indice)
                        ReDim Preserve Appezza(indice)
                        ReDim Preserve Id_Reg(indice)
                        ReDim Preserve Sup_App(indice)
                        ReDim Preserve Veg_Cod(indice)
                        ReDim Preserve Veg_Des(indice)
                        ReDim Preserve Cul_Cod(indice)
                        ReDim Preserve Cul_Des(indice)
                        ReDim Preserve Grfi_Cod(indice)
                        ReDim Preserve Grfi_Des(indice)
                        ReDim Preserve DestinazioneUso_Cod(indice)
                        ReDim Preserve DestinazioneUso_Des(indice)
                        ReDim Preserve Validita_Inizio(indice)
                        ReDim Preserve Validita_Fine(indice)

                        Entita(indice) = 0
                        Appezza(indice) = Dt_Entita.Rows(i).Item("appezza")
                        Id_Reg(indice) = Dt_Entita.Rows(i).Item("id_reg")
                        Sup_App(indice) = Dt_Entita.Rows(i).Item("superficie")
                        Veg_Cod(indice) = Dt_Entita.Rows(i).Item("veg_cod")
                        Veg_Des(indice) = Dt_Entita.Rows(i).Item("veg_des")
                        Cul_Cod(indice) = Dt_Entita.Rows(i).Item("cul_cod")
                        Cul_Des(indice) = Dt_Entita.Rows(i).Item("cul_des")
                        Grfi_Cod(indice) = Dt_Entita.Rows(i).Item("grfi_cod")
                        Grfi_Des(indice) = Dt_Entita.Rows(i).Item("grfi_des")
                        DestinazioneUso_Cod(indice) = Dt_Entita.Rows(i).Item("destinazioneuso_cod")
                        DestinazioneUso_Des(indice) = Dt_Entita.Rows(i).Item("destinazioneuso_des")
                        Validita_Inizio(indice) = If(CDate(Dt_Entita.Rows(i).Item("validita_inizio")) <> #1/1/1900#, CDate(Dt_Entita.Rows(i).Item("validita_inizio")).ToShortDateString, "")
                        Validita_Fine(indice) = If(CDate(Dt_Entita.Rows(i).Item("validita_fine")) <> #12/31/2100#, CDate(Dt_Entita.Rows(i).Item("validita_fine")).ToShortDateString, "")


                    ElseIf Dt_Entita.Rows(i).Item("Campo_Cod") <> 0 And Dt_Entita.Rows(i).Item("appezza") = 0 Then

                        indice = 0

                        Dt_Appezza = objPE.Anagrafica_AppezzaCampo_Leggi(Programmazione_Cod,
                                                                          Dt_Entita.Rows(i).Item("Campo_Cod"),
                                                                          ErrMSG,
                                                                          "", "", objParametri)

                        For j = 0 To Dt_Appezza.Rows.Count - 1

                            ReDim Preserve Entita(indice)
                            ReDim Preserve Appezza(indice)
                            ReDim Preserve Id_Reg(indice)
                            ReDim Preserve Sup_App(indice)
                            ReDim Preserve Veg_Cod(indice)
                            ReDim Preserve Veg_Des(indice)
                            ReDim Preserve Cul_Cod(indice)
                            ReDim Preserve Cul_Des(indice)
                            ReDim Preserve Grfi_Cod(indice)
                            ReDim Preserve Grfi_Des(indice)
                            ReDim Preserve DestinazioneUso_Cod(indice)
                            ReDim Preserve DestinazioneUso_Des(indice)
                            ReDim Preserve Validita_Inizio(indice)
                            ReDim Preserve Validita_Fine(indice)

                            Entita(indice) = Dt_Appezza.Rows(j).Item("programmazione_entita_cod")
                            Appezza(indice) = Dt_Appezza.Rows(j).Item("appezza")
                            Id_Reg(indice) = Dt_Appezza.Rows(j).Item("id_reg")
                            Sup_App(indice) = Dt_Appezza.Rows(j).Item("superficie")
                            Veg_Cod(indice) = Dt_Appezza.Rows(j).Item("veg_cod")
                            Veg_Des(indice) = Dt_Appezza.Rows(j).Item("veg_des")
                            Cul_Cod(indice) = Dt_Appezza.Rows(j).Item("cul_cod")
                            Cul_Des(indice) = Dt_Appezza.Rows(j).Item("cul_des")
                            Grfi_Cod(indice) = Dt_Appezza.Rows(j).Item("grfi_cod")
                            Grfi_Des(indice) = Dt_Appezza.Rows(j).Item("grfi_des")
                            DestinazioneUso_Cod(indice) = Dt_Appezza.Rows(j).Item("destinazioneuso_cod")
                            DestinazioneUso_Des(indice) = Dt_Appezza.Rows(j).Item("destinazioneuso_des")
                            Validita_Inizio(indice) = If(CDate(Dt_Appezza.Rows(j).Item("validita_inizio")) <> #1/1/1900#, CDate(Dt_Appezza.Rows(j).Item("validita_inizio")).ToShortDateString, "")
                            Validita_Fine(indice) = If(CDate(Dt_Appezza.Rows(j).Item("validita_fine")) <> #12/31/2100#, CDate(Dt_Appezza.Rows(j).Item("validita_fine")).ToShortDateString, "")

                            indice += 1

                        Next

                        'se ho solo una coltura aggiungo l'altra..
                        If Dt_Appezza.Rows.Count = 1 Then

                            ReDim Preserve Entita(indice)
                            ReDim Preserve Appezza(indice)
                            ReDim Preserve Id_Reg(indice)
                            ReDim Preserve Sup_App(indice)
                            ReDim Preserve Veg_Cod(indice)
                            ReDim Preserve Veg_Des(indice)
                            ReDim Preserve Cul_Cod(indice)
                            ReDim Preserve Cul_Des(indice)
                            ReDim Preserve Grfi_Cod(indice)
                            ReDim Preserve Grfi_Des(indice)
                            ReDim Preserve DestinazioneUso_Cod(indice)
                            ReDim Preserve DestinazioneUso_Des(indice)
                            ReDim Preserve Validita_Inizio(indice)
                            ReDim Preserve Validita_Fine(indice)

                            Entita(indice) = 0
                            Appezza(indice) = 0
                            Id_Reg(indice) = 0
                            Sup_App(indice) = 0
                            Veg_Cod(indice) = ""
                            Veg_Des(indice) = ""
                            Cul_Cod(indice) = ""
                            Cul_Des(indice) = ""
                            Grfi_Cod(indice) = 0
                            Grfi_Des(indice) = ""
                            DestinazioneUso_Cod(indice) = 0
                            DestinazioneUso_Des(indice) = ""
                            Validita_Inizio(indice) = ""
                            Validita_Fine(indice) = ""

                        End If

                    End If

                    Dim StrSuperficie As String
                    Dim SupCatastale As Decimal
                    Dim SupCondotta As Decimal
                    Dim SupSeminabile As Decimal
                    Dim SupUnar As Decimal

                    SupCatastale = Ettari_from_EttariAreCentiare(Dt_Entita.Rows(i).Item("Ettari"), Dt_Entita.Rows(i).Item("Are"), Dt_Entita.Rows(i).Item("Centiare"))

                    StrSuperficie = Dt_Entita.Rows(i).Item("Sup_Condotta")
                    SupCondotta = CDbl(StrSuperficie)

                    StrSuperficie = Dt_Entita.Rows(i).Item("Sup_Seminabile")
                    SupSeminabile = CDbl(StrSuperficie)

                    StrSuperficie = Dt_Entita.Rows(i).Item("Sup_Unar")
                    SupUnar = CDbl(StrSuperficie)

                    DT_Appezzamenti_2Colture_Insert(Dt_Appezzamenti,
                                                  Dt_Entita.Rows(i).Item("Programmazione_Entita_Cod"),
                                                  Dt_Entita.Rows(i).Item("Sa_Cod"),
                                                  Dt_Entita.Rows(i).Item("Sa_Nome"),
                                                  Dt_Entita.Rows(i).Item("Campo_Cod"),
                                                  Dt_Entita.Rows(i).Item("Entita_Des"),
                                                  Entita,
                                                  Appezza,
                                                  Id_Reg,
                                                  Sup_App,
                                                  Veg_Cod,
                                                  Veg_Des,
                                                  Cul_Cod,
                                                  Cul_Des,
                                                  Grfi_Cod,
                                                  Grfi_Des,
                                                  DestinazioneUso_Cod,
                                                  DestinazioneUso_Des,
                                                  Validita_Inizio,
                                                  Validita_Fine,
                                                  Dt_Entita.Rows(i).Item("Note"),
                                                  Dt_Entita.Rows(i).Item("Modifica"))


                    ' riazzero tutto per il prossimo inserimento
                    Array.Clear(Entita, 0, Entita.Length)
                    Array.Clear(Appezza, 0, Appezza.Length)
                    Array.Clear(Id_Reg, 0, Id_Reg.Length)
                    Array.Clear(Sup_App, 0, Sup_App.Length)
                    Array.Clear(Veg_Cod, 0, Veg_Cod.Length)
                    Array.Clear(Veg_Des, 0, Veg_Des.Length)
                    Array.Clear(Cul_Cod, 0, Cul_Cod.Length)
                    Array.Clear(Cul_Des, 0, Cul_Des.Length)
                    Array.Clear(Grfi_Cod, 0, Grfi_Cod.Length)
                    Array.Clear(Grfi_Des, 0, Grfi_Des.Length)
                    Array.Clear(DestinazioneUso_Cod, 0, DestinazioneUso_Cod.Length)
                    Array.Clear(DestinazioneUso_Des, 0, DestinazioneUso_Des.Length)
                    Array.Clear(Validita_Inizio, 0, Validita_Inizio.Length)

                    indice = 0

                Next

            End If


        Catch ex As Exception

            Programmazione_Cod = 0
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        Finally

            If FlagConnessioneLocale Then
                If Not IsNothing(xConnessione) Then
                    xConnessione.Close()
                    xConnessione.Dispose()
                End If
            Else
                If xConnectionState = ConnectionState.Closed Then
                    If Not IsNothing(xConnessione) Then
                        xConnessione.Close()
                    End If
                End If
            End If

        End Try

    End Function

    Public Function Programmazione_Specie_Leggi(ByVal Programmazione_Cod As Integer,
                                                ByRef objParametri As AgronicaCoreParametri
                                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Programmazione_R.Programmazione_Specie_Leggi()"

        Dim DT_Specie As New DataTable
        Dim DT_Entita As New DataTable

        Dim ErrMSG As String = ""
        Dim MessaggioErrore As String

        Dim objEntitaDAL As AgronicaCoreAnagrafeDAL.Programmazione_Entita_R

        '-------------------------------------------------------------
        Dim Dt As New DataTable
        Dim Dr As DataRow

        Dt.Columns.Add(New DataColumn("Veg_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Veg_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sup_1", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("Sup_2", GetType(Decimal)))

        '-------------------------------------------------------------
        Try

            If Programmazione_Cod <> 0 Then

                objEntitaDAL = New AgronicaCoreAnagrafeDAL.Programmazione_Entita_R

                'Recupero il datatable
                DT_Specie = objEntitaDAL.Programmazione_Specie_Leggi_2(Programmazione_Cod,
                                                                       ErrMSG,
                                                                       enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                       "", "", objParametri)

                DT_Entita = objEntitaDAL.Programmazione_Entita_Leggi(
                                                        Programmazione_Cod,
                                                        ErrMSG,
                                                        0, "", "", 0, 0, 0, 0, 0, #1/1/1900#, #12/31/2100#, 1,
                                                        enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                        "",
                                                        "",
                                                        objParametri)

                Dim i, j As Integer
                Dim Veg_Cod As String
                Dim Cul_Cod As String
                Dim Veg_Des As String
                Dim Cul_Des As String
                Dim DrS As DataRow()
                Dim Sup_1 As Decimal
                Dim Sup_2 As Decimal

                If DT_Specie IsNot Nothing AndAlso DT_Entita IsNot Nothing AndAlso
                   DT_Entita.Rows.Count > 0 AndAlso DT_Specie.Rows.Count > 0 Then

                    For i = 0 To DT_Specie.Rows.Count - 1

                        Sup_1 = 0
                        Sup_2 = 0

                        Veg_Cod = DT_Specie.Rows(i).Item("Veg_Cod").ToString
                        Veg_Des = DT_Specie.Rows(i).Item("Veg_Des").ToString

                        Cul_Cod = DT_Specie.Rows(i).Item("Cul_Cod").ToString
                        Cul_Des = DT_Specie.Rows(i).Item("Cul_Des").ToString

                        If Veg_Cod <> "" Then

                            If Cul_Cod <> "" Then

                                DrS = DT_Entita.Select("Veg_Cod_Cliente='" & Veg_Cod & "'" &
                                                       "AND Cul_Cod_Cliente='" & Cul_Cod & "'")

                            Else

                                DrS = DT_Entita.Select("Veg_Cod_Cliente='" & Veg_Cod & "'")

                            End If


                            If DrS IsNot Nothing AndAlso DrS.Length > 0 Then

                                Dr = Dt.NewRow

                                If Cul_Cod <> "" Then
                                    Dr.Item("Veg_Cod") = Cul_Cod
                                    Dr.Item("Veg_Des") = Veg_Des & " - " & Cul_Des
                                Else
                                    Dr.Item("Veg_Cod") = Veg_Cod
                                    Dr.Item("Veg_Des") = Veg_Des
                                End If

                                For j = 0 To DrS.Length - 1

                                    Select Case CInt(DrS(j).Item("appezza"))
                                        Case -1
                                            Sup_1 += CDbl(DrS(j).Item("Superficie"))
                                        Case -2
                                            Sup_2 += CDbl(DrS(j).Item("Superficie"))
                                    End Select
                                Next

                                Dr.Item("Sup_1") = Sup_1
                                Dr.Item("Sup_2") = Sup_2

                                Dt.Rows.Add(Dr)

                            End If

                        End If

                    Next

                End If

            End If

        Catch ex As Exception
            Programmazione_Cod = 0
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Dt

    End Function

#End Region


#Region "Appezzamenti"

    Public Sub DT_Appezzamenti_Crea(ByRef DT As DataTable)

        DT.Columns.Add(New DataColumn("UNID_APP", GetType(String)))

        DT.Columns.Add(New DataColumn("ChkSeleziona", GetType(Boolean)))
        DT.Columns.Add(New DataColumn("Operazione_Cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Operazione_Des", GetType(String)))

        DT.Columns.Add(New DataColumn("Programmazione_Entita_Cod", GetType(Integer)))

        DT.Columns.Add(New DataColumn("Piva", GetType(String)))
        DT.Columns.Add(New DataColumn("partitaIvaReale", GetType(String)))
        DT.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Campo_Cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Appezza", GetType(Integer)))
        DT.Columns.Add(New DataColumn("ID_Reg", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Progetto_Cod", GetType(Integer)))

        DT.Columns.Add(New DataColumn("Sa_Nome", GetType(String)))
        DT.Columns.Add(New DataColumn("Campo_Des", GetType(String)))
        DT.Columns.Add(New DataColumn("App_Nome", GetType(String)))
        DT.Columns.Add(New DataColumn("Sup_App", GetType(Decimal)))

        DT.Columns.Add(New DataColumn("Veg_Cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Veg_Des", GetType(String)))

        DT.Columns.Add(New DataColumn("Cul_Cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Cul_Des", GetType(String)))

        DT.Columns.Add(New DataColumn("Grva_Cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Grva_Des", GetType(String)))

        DT.Columns.Add(New DataColumn("Grfi_Cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Grfi_Des", GetType(String)))

        DT.Columns.Add(New DataColumn("Cop_Cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Cop_Des", GetType(String)))

        DT.Columns.Add(New DataColumn("Progetto_Nome", GetType(String)))

        DT.Columns.Add(New DataColumn("Resa", GetType(Decimal)))
        DT.Columns.Add(New DataColumn("DestinazioneUso", GetType(Integer)))
        DT.Columns.Add(New DataColumn("DestinazioneUso_Des", GetType(String)))

        DT.Columns.Add(New DataColumn("Num_Piante", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Tra_Fila", GetType(Decimal)))
        DT.Columns.Add(New DataColumn("Su_Fila", GetType(Decimal)))

        DT.Columns.Add(New DataColumn("Validita_Inizio_Impianto", GetType(String)))
        DT.Columns.Add(New DataColumn("Validita_Inizio", GetType(String)))
        DT.Columns.Add(New DataColumn("Validita_Fine", GetType(String)))

        DT.Columns.Add(New DataColumn("Data_Semina", GetType(String)))
        DT.Columns.Add(New DataColumn("Data_Raccolta", GetType(String)))

        DT.Columns.Add(New DataColumn("Catasto", GetType(String)))
        DT.Columns.Add(New DataColumn("TipoZona", GetType(String)))

        DT.Columns.Add(New DataColumn("MetodoProduzione_Cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("MetodoProduzione_Des", GetType(String)))

        '  Vanni, 06/06/2013 16:12:59: aggiunta gestione veg e cul cod del cliente
        DT.Columns.Add(New DataColumn("veg_cod_cliente", GetType(String)))
        DT.Columns.Add(New DataColumn("cul_cod_cliente", GetType(String)))

        ' Maga, 04/7/2013: aggiunta 
        DT.Columns.Add(New DataColumn("veg_des_cliente", GetType(String)))
        DT.Columns.Add(New DataColumn("cul_des_cliente", GetType(String)))

        ' Nico 25/09/2013: aggiunta
        DT.Columns.Add(New DataColumn("macrouso_cod", GetType(String)))
        DT.Columns.Add(New DataColumn("macrouso_des", GetType(String)))

        ' Fede 28/01/2014: aggiunta
        DT.Columns.Add(New DataColumn("unita_vitata", GetType(Integer)))

        DT.Columns.Add(New DataColumn("ElementoGrafico_Cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("ElementoGrafico_Des", GetType(String)))

        ' Fede 15/09/2017: aggiunta
        DT.Columns.Add(New DataColumn("ribaltato", GetType(Integer)))
        DT.Columns.Add(New DataColumn("movimentato", GetType(Integer)))

        ' Drudi 04/10/2017: aggiunta
        DT.Columns.Add(New DataColumn("Veg_Cod_Agea", GetType(String)))
        DT.Columns.Add(New DataColumn("Cul_Cod_Agea", GetType(String)))
        DT.Columns.Add(New DataColumn("Uso_Cod_Agea", GetType(String)))
        DT.Columns.Add(New DataColumn("Occupazione_Cod_Agea", GetType(String)))
        DT.Columns.Add(New DataColumn("Destinazione_Cod_Agea", GetType(String)))
        DT.Columns.Add(New DataColumn("Qualita_Cod_Agea", GetType(String)))

        ' Drudi 02/11/2017: aggiunta
        DT.Columns.Add(New DataColumn("Limite_N", GetType(String)))
        DT.Columns.Add(New DataColumn("Limite_P", GetType(String)))
        DT.Columns.Add(New DataColumn("Limite_K", GetType(String)))
        DT.Columns.Add(New DataColumn("Data_Fioritura_Prevista", GetType(String)))
        DT.Columns.Add(New DataColumn("Veg_Cod_Prec", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Veg_Cod_Prec2", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Veg_Cod_Prec3", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Veg_Cod_Prec4", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Piano_Semina", GetType(String)))
        DT.Columns.Add(New DataColumn("Codice_Contratto", GetType(String)))
        DT.Columns.Add(New DataColumn("Disciplinare_Cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Regolamento_Cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Stato_Cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("via_stringa", GetType(String)))
        DT.Columns.Add(New DataColumn("Codice_Fiscale_Tecnico", GetType(String)))

        DT.Columns.Add(New DataColumn("Stato_Ribaltamento", GetType(Integer)))
        DT.Columns.Add(New DataColumn("provenienza_fascicolo", GetType(String)))

        DT.Columns.Add(New DataColumn("datoGis", GetType(String)))
        DT.Columns.Add(New DataColumn("IAF", GetType(String)))

        DT.Columns.Add(New DataColumn("Pratica_Cod", GetType(String)))

        DT.Columns.Add(New DataColumn("Disciplinare", GetType(String)))
        DT.Columns.Add(New DataColumn("Regolamento_Concimazione_Cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Flag_PubblicoPrivato", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Id_tr", GetType(Integer)))

        DT.Columns.Add(New DataColumn("DistBZ_CorpiIdrici", GetType(Double)))
        DT.Columns.Add(New DataColumn("DistBZ_AreeResPub", GetType(Double)))
        DT.Columns.Add(New DataColumn("DistBZ_Allevamenti", GetType(Double)))
        DT.Columns.Add(New DataColumn("DistBZ_VegNatNonColt", GetType(Double)))

        DT.Columns.Add(New DataColumn("SupBZ_Riduzione", GetType(Double)))

        ' Vanni 08/04/2019: aggiunta
        DT.Columns.Add(New DataColumn("wkt", GetType(String)))
        DT.Columns.Add(New DataColumn("wkt_georiferimento_cod", GetType(String)))


        DT.Columns.Add(New DataColumn("Isola", GetType(String)))
        DT.Columns.Add(New DataColumn("Riferimento_Alfanumerico_Appezzamento", GetType(String)))

        DT.Columns.Add(New DataColumn("CapitolatoPrivato", GetType(String)))
        DT.Columns.Add(New DataColumn("CapitolatoPrivato_Des", GetType(String)))

        DT.Columns.Add(New DataColumn("Finalita_Concimazione_Impianto", GetType(String)))

        'INDIRIZZO
        DT.Columns.Add(New DataColumn("Cod_Indirizzo", GetType(Integer)))
        DT.Columns.Add(New DataColumn("ind_des", GetType(String)))
        DT.Columns.Add(New DataColumn("frz_des", GetType(String)))
        DT.Columns.Add(New DataColumn("CAP", GetType(String)))
        DT.Columns.Add(New DataColumn("com_des_indirizzo", GetType(String)))
        DT.Columns.Add(New DataColumn("pro_cod_indirizzo", GetType(String)))
        DT.Columns.Add(New DataColumn("stato_indirizzo", GetType(String)))
        DT.Columns.Add(New DataColumn("stato_indirizzo_des", GetType(String)))
        DT.Columns.Add(New DataColumn("note_indirizzo", GetType(String)))
        DT.Columns.Add(New DataColumn("pro_cod_istat_indirizzo", GetType(String)))
        DT.Columns.Add(New DataColumn("com_cod_istat_indirizzo", GetType(String)))

        DT.Columns.Add(New DataColumn("Pratiche_Cod", GetType(String)))
        DT.Columns.Add(New DataColumn("Pratiche_Des", GetType(String)))

        DT.Columns.Add(New DataColumn("KPIN", GetType(String)))
        DT.Columns.Add(New DataColumn("Block_Name", GetType(String)))

        DT.Columns.Add(New DataColumn("Foral_Cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Foral_Des", GetType(String)))

        DT.Columns.Add(New DataColumn("Data_Inizio_Portinnesto", GetType(String)))

        DT.Columns.Add(New DataColumn("Data_Creazione", GetType(Date)))
        DT.Columns.Add(New DataColumn("Data_Modifica", GetType(Date)))

        'ZESPRI
        DT.Columns.Add(New DataColumn("ZespriFase_Cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("ZespriFase_Des", GetType(String)))
        DT.Columns.Add(New DataColumn("ZespriTipo_Cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("ZespriTipo_Des", GetType(String)))
        DT.Columns.Add(New DataColumn("ZespriGrower_Cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("ZespriGrower_Des", GetType(String)))

        'NUM PIANTE MF
        DT.Columns.Add(New DataColumn("Num_Piante_Femmine", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Num_Piante_Maschi", GetType(Integer)))

        'PORTINNESTO
        DT.Columns.Add(New DataColumn("Port_Cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Port_Des", GetType(String)))


        'TIPOLOGIA DI INNESTO O TRAPIANTO
        DT.Columns.Add(New DataColumn("TipologiaInnestoTrapianto_Cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("TipologiaInnestoTrapianto_Des", GetType(String)))

        'PRODOTTO
        DT.Columns.Add(New DataColumn("Mat_Cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Mat_Des", GetType(String)))

    End Sub

    '#################################################
    'GESTIONE FILTRI - EDIT LUGLIO 2013
    'DT_Appezzamenti = Session("DT_Appezzamenti") -> è il dt corrente gestito nel planning
    'DT_AppezzamentiCompleto lo creo qui dentro come unione di DT_Appezzamenti e DT_AppezzaNascosti
    'DT_AppezzaNascosti lo creo qui, viene tenuto in session (il viewstate non funziona), si aggiungono record in caso di filtri successivi
    Public Sub DT_Appezzamenti_ApplicaFiltro(ByRef LogErrori As String,
                                                ByRef DT_Appezzamenti As DataTable,
                                                ByRef DT_AppezzaNascosti As DataTable,
                                                ByVal Flag_1_ApplicaFiltro_2_RimuoviFiltro As Integer,
                                                ByVal Veg_Cod As Integer,
                                                ByVal Cul_Cod As Integer,
                                                ByVal Id_Cod_Dest_Uso As Integer,
                                                ByVal Str_Vulnerabile As String,
                                                ByVal StrKey_Particella As String,
                                                ByVal Macrouso_Cod As String,
                                                ByVal Utilizzo_Cod As String,
                                                ByVal Sa_Cod As Integer,
                                                ByVal Campo_Cod As Integer
                                                )

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Programmazione_R.DT_Appezzamenti_ApplicaFiltro()"

        '  ByRef DT_AppezzamentiCompleto As DataTable, _

        'gestire questi casi:
        'CASO 1 -> non sono stati ancora applicati filtri
        'CASO 2 -> filtri successivi, quindi il dt appezzamenti è già filtrato

        'tolgo questo controllo, perché nel caso il filtro precedente non abbia restituito appezzamenti
        'e ora cambio filtro o lo rimuovo
        'devo poter recuperare gli appezza nascosti!
        ''se non ci sono appezzamenti esco
        'If IsNothing(DT_Appezzamenti) Then
        '    Exit Sub
        'Else
        '    If DT_Appezzamenti.Rows.Count = 0 Then
        '        Exit Sub
        '    End If
        'End If
        'non dovrebbe cmq essere mai nothing -> da provare
        If IsNothing(DT_Appezzamenti) Then
            Exit Sub
        End If

        Dim DT_AppezzamentiIniziale As DataTable 'dt utilizzato qui dentro solo per controllo
        Dim DT_AppezzaNascostiIniziale As DataTable 'dt utilizzato qui dentro solo per controllo
        Dim debug As Boolean
        Dim i As Integer

        'salvo una copia del dt appezzamenti iniziale
        'DT_AppezzamentiIniziale = DT_Appezzamenti
        DT_AppezzamentiIniziale = DT_Appezzamenti.Copy
        'controllo
        If DT_AppezzamentiIniziale.Rows.Count <> DT_Appezzamenti.Rows.Count Then
            Throw New Exception("Errore numero righe DT_AppezzamentiIniziale.")
        End If

        Try

            If IsNothing(DT_AppezzaNascosti) Then
                'primo filtro applicato
                'copio solo la struttura del DT_Appezzamenti
                DT_AppezzaNascosti = DT_Appezzamenti.Clone()
            Else

                If DT_Appezzamenti.Rows.Count = 0 AndAlso DT_AppezzaNascosti.Rows.Count = 0 Then
                    Throw New Exception("Anomalia, DT_Appezzamenti e DT_AppezzaNascosti entrambi vuoti.")
                End If

                'filtri successivi, il DT_AppezzaNascosti è già stato creato
                '(ma potrebbe non avere righe perché non c'erano appezza da escludere in base al filtro)
                Dim num_righe_appezza_prima As Integer = DT_Appezzamenti.Rows.Count
                Dim num_righe_appezza_dopo As Integer = 0
                Dim num_righe_appezzanascosti_prima As Integer = DT_AppezzaNascosti.Rows.Count
                Dim num_righe_appezzanascosti_dopo As Integer = 0

                Dim Nuovo_UNID_APP As String
                Nuovo_UNID_APP = Nuovo_UNID_APP_ByDt_Appezzamenti(DT_Appezzamenti)

                If Nuovo_UNID_APP = "" Then
                    Throw New Exception("Nuovo_UNID_APP vuoto")
                End If

                'aggiungo le righe del DT_AppezzaNascosti al dt appezzamenti
                'per ripartire con nuovi filtri
                For i = 0 To DT_AppezzaNascosti.Rows.Count - 1
                    DT_AppezzaNascosti.Rows(i).Item("UNID_APP") = Nuovo_UNID_APP
                    DT_Appezzamenti.ImportRow(DT_AppezzaNascosti.Rows(i))
                    Nuovo_UNID_APP = Nuovo_UNID_APP_byUnidApp(Nuovo_UNID_APP)
                Next

                DT_AppezzaNascosti.Rows.Clear()
                num_righe_appezza_dopo = DT_Appezzamenti.Rows.Count
                num_righe_appezzanascosti_dopo = DT_AppezzaNascosti.Rows.Count
                If (num_righe_appezza_prima + num_righe_appezzanascosti_prima) <> (num_righe_appezza_dopo + num_righe_appezzanascosti_dopo) Then
                    Throw New Exception("Errore nel travaso da DT_AppezzaNascosti a DT_Appezzamenti.")
                End If
            End If ' DT_AppezzaNascosti nothing

            'salvo una copia del dt appezza nascosti iniziale
            DT_AppezzaNascostiIniziale = DT_AppezzaNascosti
            ''controllo
            'If DT_AppezzaNascostiIniziale.Rows.Count <> DT_AppezzaNascosti.Rows.Count Then
            '    Throw New Exception("Errore numero righe DT_AppezzaNascostiIniziale.")
            'End If

            If Flag_1_ApplicaFiltro_2_RimuoviFiltro = 1 Then

                Try

                    Try

                        If Not IsNothing(DT_Appezzamenti) AndAlso DT_Appezzamenti.Rows.Count > 0 Then

                            'preparo il filtro sql da applicare al dt appezzamenti

                            Dim StrSelectDT As String = ""
                            If Veg_Cod <> 0 Then
                                StrSelectDT &= " Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod, False)
                            End If
                            If Cul_Cod <> 0 Then
                                If StrSelectDT <> "" Then
                                    StrSelectDT &= " AND "
                                End If
                                StrSelectDT &= " Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod, False)
                            End If
                            If Id_Cod_Dest_Uso <> 0 Then
                                If StrSelectDT <> "" Then
                                    StrSelectDT &= " AND "
                                End If
                                StrSelectDT &= " DestinazioneUso = " & Agro_SQL_SaveNum(Id_Cod_Dest_Uso, False)
                            End If
                            If Str_Vulnerabile <> "" Then
                                If StrSelectDT <> "" Then
                                    StrSelectDT &= " AND "
                                End If
                                StrSelectDT &= " TipoZona = '" & Agro_SQL_SaveText(Str_Vulnerabile, False) & "'"
                            End If
                            If StrKey_Particella <> "" Then
                                If StrSelectDT <> "" Then
                                    StrSelectDT &= " AND "
                                End If
                                StrSelectDT &= " Catasto LIKE '%" & Agro_SQL_SaveText(StrKey_Particella, False) & "%'"
                            End If
                            If Macrouso_Cod <> "" Then
                                If StrSelectDT <> "" Then
                                    StrSelectDT &= " AND "
                                End If
                                StrSelectDT &= " Macrouso_Cod = '" & Agro_SQL_SaveText(Macrouso_Cod, False) & "' "
                            End If
                            If Utilizzo_Cod <> "" Then
                                If StrSelectDT <> "" Then
                                    StrSelectDT &= " AND "
                                End If
                                StrSelectDT &= " Veg_Cod_Cliente = '" & Agro_SQL_SaveText(Utilizzo_Cod, False) & "' "
                            End If

                            If Sa_Cod <> 0 Then
                                If StrSelectDT <> "" Then
                                    StrSelectDT &= " AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod, False)
                                Else
                                    StrSelectDT &= " Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod, False)
                                End If

                                If Campo_Cod <> 0 Then
                                    If StrSelectDT <> "" Then
                                        StrSelectDT &= " AND "
                                    End If
                                    StrSelectDT &= " Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod, False)
                                End If
                            End If



                            'uso questo dt temporaneo per fare il filtro
                            'non posso usare DT_Appezzamenti perché devo svuotarlo e riempirlo solo con il risultato del filtro
                            'e sul vettore di datarow filtrate rimangono dei riferimenti al dt iniziale
                            '(se lo svuoto anche nelle datarow non rimane più niente)
                            Dim DTxSelect As DataTable
                            DTxSelect = DT_Appezzamenti.Copy

                            Dim Vet_RowFiltrate As DataRow()
                            Vet_RowFiltrate = DTxSelect.Select(StrSelectDT)

                            'svuoto il dt appezzamenti e lo riempio solo con le righe filtrate
                            DT_Appezzamenti.Rows.Clear()

                            'per ogni datarow che corrisponde al filtro selezionato
                            If Not IsNothing(Vet_RowFiltrate) AndAlso Vet_RowFiltrate.Length > 0 Then

                                Dim UNID_APP_selezionati As String
                                Dim StrSql_UNID_APP_selezionati As String = ""
                                'salvo tutte le chiavi degli appezzamenti
                                For i = 0 To Vet_RowFiltrate.Length - 1
                                    UNID_APP_selezionati = Vet_RowFiltrate(i).Item("UNID_APP")
                                    StrSql_UNID_APP_selezionati &= "'" & UNID_APP_selezionati & "'" & ","
                                    DT_Appezzamenti.ImportRow(Vet_RowFiltrate(i))
                                Next
                                'controllo
                                If Vet_RowFiltrate.Length <> DT_Appezzamenti.Rows.Count Then
                                    Throw New Exception("Errore numero righe DT_Appezzamenti.")
                                End If

                                UNID_APP_selezionati = ""

                                If StrSql_UNID_APP_selezionati <> "" Then

                                    'controllo
                                    If DT_AppezzaNascosti.Rows.Count <> 0 Then
                                        Throw New Exception("DT_AppezzaNascosti deve essere vuoto e non lo è!")
                                    End If

                                    StrSql_UNID_APP_selezionati = Mid(StrSql_UNID_APP_selezionati, 1, StrSql_UNID_APP_selezionati.Length - 1)

                                    Dim Vet_RowESCLUSI As DataRow()
                                    Dim StrSelectESCLUSI As String = " UNID_APP NOT IN ( " & StrSql_UNID_APP_selezionati & ")"
                                    Vet_RowESCLUSI = DTxSelect.Select(StrSelectESCLUSI)

                                    'riempio il dt con tutti gli appezzamenti nascosti (fuori dal filtro selezionato)
                                    If Not IsNothing(Vet_RowESCLUSI) AndAlso Vet_RowESCLUSI.Length > 0 Then
                                        'Dim UNID_APP_esclusi As String
                                        For i = 0 To Vet_RowESCLUSI.Length - 1
                                            DT_AppezzaNascosti.ImportRow(Vet_RowESCLUSI(i))
                                            'UNID_APP_esclusi = Vet_RowESCLUSI(i).Item("UNID_APP")
                                        Next
                                        'controllo
                                        If Vet_RowESCLUSI.Length <> DT_AppezzaNascosti.Rows.Count Then
                                            Throw New Exception("Errore numero righe DT_AppezzaNascosti.")
                                        End If
                                    Else
                                        'il filtro coincide esattamente con tutti gli appezzamenti,
                                        'non c'è niente da escludere
                                        debug = True
                                    End If 'Vet_RowESCLUSI
                                Else
                                    Throw New Exception("Errore nell'import datarow filtrate.")
                                End If 'errore nella composizione del filtro unid_app
                            Else
                                'non c'è niente che soddisfa il filtro
                                'il DT_AppezzaNascosti coincide con il dt appezza iniziale
                                DT_AppezzaNascosti = DT_AppezzamentiIniziale
                            End If 'Vet_RowFiltrate
                        Else
                            'dt appezza vuoto
                            debug = True
                        End If

                        If DT_AppezzamentiIniziale.Rows.Count <> (DT_Appezzamenti.Rows.Count + DT_AppezzaNascosti.Rows.Count) Then
                            Throw New Exception("La somma delle righe del dt filtrato e del dt appezza esclusi non coincide con il numero di righe del dt iniziale.")
                        End If

                    Catch ex As Exception
                        LogErrori = NomeRoutine & " Errore durante l'applicazione del filtro: " & ex.Message & vbCrLf
                        'in caso di errore
                        DT_Appezzamenti = DT_AppezzamentiIniziale
                        DT_AppezzaNascosti = DT_AppezzaNascostiIniziale
                    End Try

                Catch ex2 As Exception
                    LogErrori = NomeRoutine & " Errore nel salvataggio dt iniziali: " & ex2.Message & vbCrLf
                End Try
            Else
                'rimozione filtro
                debug = True
            End If 'Flag_1_ApplicaFiltro_2_RimuoviFiltro

        Catch ex3 As Exception
            LogErrori = NomeRoutine & ex3.Message & vbCrLf
            'in caso di errore
            DT_Appezzamenti = DT_AppezzamentiIniziale
        End Try

    End Sub

    '#################################################################
    Public Function Ultimo_Appezza_ByDt_Appezzamenti(ByVal DT_Appezzamenti As DataTable) As String

        Dim i As Integer
        Dim UNID_APP As String
        Dim Piva As String = ""
        Dim Sa_Cod As Integer = 0
        Dim Campo_Cod As Integer = 0
        Dim Appezza As Integer = 0
        Dim Id_Reg As Integer = 0
        Dim Progetto_Cod As Integer = 0
        Dim UltimoAppezza As Integer = 0
        Dim Nuovo_UNID_APP As String = ""

        If Not IsNothing(DT_Appezzamenti) Then

            For i = 0 To DT_Appezzamenti.Rows.Count - 1

                UNID_APP = DT_Appezzamenti.Rows(i).Item("UNID_APP")

                Key_Appezzamento_GET(UNID_APP,
                                        Piva,
                                        Sa_Cod,
                                        Campo_Cod,
                                        Appezza,
                                        Id_Reg,
                                        Progetto_Cod)

                Select Case Appezza
                    Case Is < 0
                        If Appezza < UltimoAppezza Then
                            UltimoAppezza = Appezza
                        End If

                    Case Is > 0
                        If Appezza > UltimoAppezza Then
                            UltimoAppezza = Appezza
                        End If

                    Case Is = 0
                        Dim debug As Boolean = True
                End Select

            Next

        End If

        Return UltimoAppezza

    End Function

    '#################################################################
    Public Function Nuovo_UNID_APP_ByDt_Appezzamenti(ByVal DT_Appezzamenti As DataTable) As String

        Dim NuovoAppezza As Integer = 0
        Dim i As Integer
        Dim UNID_APP As String
        Dim Piva As String = ""
        Dim Sa_Cod As Integer = 0
        Dim Campo_Cod As Integer = 0
        Dim Appezza As Integer = 0
        Dim Id_Reg As Integer = 0
        Dim Progetto_Cod As Integer = 0
        Dim UltimoAppezza As Integer = 0
        Dim Nuovo_UNID_APP As String = ""

        If Not IsNothing(DT_Appezzamenti) Then
            For i = 0 To DT_Appezzamenti.Rows.Count - 1

                UNID_APP = DT_Appezzamenti.Rows(i).Item("UNID_APP")

                Key_Appezzamento_GET(UNID_APP,
                                        Piva,
                                        Sa_Cod,
                                        Campo_Cod,
                                        Appezza,
                                        Id_Reg,
                                        Progetto_Cod)

                Select Case Appezza
                    Case Is < 0
                        If Appezza < UltimoAppezza Then
                            UltimoAppezza = Appezza
                        End If

                    Case Is > 0
                        If Appezza > UltimoAppezza Then
                            UltimoAppezza = Appezza
                        End If

                    Case Is = 0

                End Select
            Next

            If UltimoAppezza < 0 Then
                NuovoAppezza = UltimoAppezza - 1
            Else
                NuovoAppezza = UltimoAppezza + 1
            End If

            Key_Appezzamento_SET(Nuovo_UNID_APP,
                                 Piva,
                                 Sa_Cod,
                                 Campo_Cod,
                                 NuovoAppezza,
                                 Id_Reg,
                                 Progetto_Cod)

        End If

        Return Nuovo_UNID_APP

    End Function

    '#################################################################
    Public Function Nuovo_UNID_APP_byUnidApp(ByVal UNID_APP As String) As String

        Dim NuovoAppezza As Integer = 0
        Dim Piva As String = ""
        Dim Sa_Cod As Integer = 0
        Dim Campo_Cod As Integer = 0
        Dim Appezza As Integer = 0
        Dim Id_Reg As Integer = 0
        Dim Progetto_Cod As Integer = 0
        Dim UltimoAppezza As Integer = 0
        Dim Nuovo_UNID_APP As String = ""

        Key_Appezzamento_GET(UNID_APP,
                                Piva,
                                Sa_Cod,
                                Campo_Cod,
                                Appezza,
                                Id_Reg,
                                Progetto_Cod)

        Select Case Appezza
            Case Is < 0
                If Appezza < UltimoAppezza Then
                    UltimoAppezza = Appezza
                End If

            Case Is > 0
                If Appezza > UltimoAppezza Then
                    UltimoAppezza = Appezza
                End If

            Case Is = 0

        End Select

        If UltimoAppezza < 0 Then
            NuovoAppezza = UltimoAppezza - 1
        Else
            NuovoAppezza = UltimoAppezza + 1
        End If

        Key_Appezzamento_SET(Nuovo_UNID_APP,
                             Piva,
                             Sa_Cod,
                             Campo_Cod,
                             NuovoAppezza,
                             Id_Reg,
                             Progetto_Cod)


        Return Nuovo_UNID_APP

    End Function

    Public Sub DT_Appezzamenti_Inizializza(
                                ByRef DT_Appezzamenti As DataTable,
                                ByVal ElencoAppezzamenti As String,
                                ByVal Data_Inizio As Date,
                                ByVal Data_Fine As Date,
                                ByVal DT_Intersezioni As DataTable,
                                    ByRef objParametri As AgronicaCoreParametri)

        Dim KeyAppezzamenti As String()
        Dim Piva As String = ""
        Dim Sa_Cod As Integer
        Dim Campo_Cod As Integer
        Dim Appezza As Integer
        Dim Id_Reg As Integer
        Dim Progetto_Cod As Integer
        Dim i, j As Integer
        Dim DT As New DataTable
        Dim Operazione_Cod As Integer
        Dim Data_Semina As Date
        Dim Data_Raccolta As Date
        Dim Filtro As String = ""
        Dim Catasto As String = ""
        Dim Dr As DataRow()
        Dim objMS As New AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Agea_R
        Dim veg_des_cliente As String = ""
        Dim cul_des_cliente As String = ""
        Dim Unita_Vitata As Integer
        Dim Validita_Inizio_Impianto As Date
        Dim Validita_Inizio_Esercizio As Date
        Dim Validita_Fine_Esercizio As Date

        Operazione_Cod = 1    'Appezzamenti inalterati ...

        KeyAppezzamenti = Split(ElencoAppezzamenti, "|")

        'Se era stato selezionato qualche appezzamento da conservare ...
        If KeyAppezzamenti.Length > 0 Then

            Dim ErrMSG As String = ""

            For i = 0 To KeyAppezzamenti.Length - 1
                'Scompongo la chiave 
                Key_Appezzamento_GET(KeyAppezzamenti(i), Piva, Sa_Cod, Campo_Cod, Appezza, Id_Reg, Progetto_Cod)
                Filtro &= " (Appezzamento.PIVA = '" & Agro_SQL_SaveText(Piva, False) & "' AND Appezzamento.SA_COD =" & Agro_SQL_SaveNum(Sa_Cod.ToString, False) & " AND Appezzamento.APPEZZA =" & Agro_SQL_SaveNum(Appezza.ToString, False) & ") OR "
            Next

            If Filtro <> "" Then
                Filtro = "(" & Left(Filtro, Filtro.Length - 4) & ")"
            End If

            'Recupero i dati dell'appezzamento
            DT = Anagrafica_Appezzamenti_Leggi_2(Piva, 0, 0, Data_Inizio, Data_Fine, Filtro, ErrMSG, objParametri)

            For i = 0 To DT.Rows.Count - 1

                Catasto = ""
                If DT_Intersezioni IsNot Nothing AndAlso DT_Intersezioni.Rows.Count > 0 Then
                    Dr = DT_Intersezioni.Select("Piva='" & DT.Rows(i).Item("Piva").ToString & "' AND Sa_Cod=" & DT.Rows(i).Item("Sa_Cod").ToString & " AND appezza=" & DT.Rows(i).Item("Appezza").ToString)
                    If Dr IsNot Nothing Then
                        For j = 0 To Dr.Length - 1
                            Catasto &= If(j <> 0, "<BR>", "") &
                                       Dr(j).Item("Prov") & "_" &
                                       Dr(j).Item("Com") & "_" &
                                       Dr(j).Item("Sezione") & "_" &
                                       Dr(j).Item("Foglio").ToString & "_" &
                                       Dr(j).Item("Numero").ToString & "_" &
                                       Dr(j).Item("Subalterno") &
                                       " (Sup." & CStr(Dr(j).Item("SupIntersezione")) & " Ha)"
                        Next
                    End If
                End If

                If Not IsDBNull(DT.Rows(i).Item("Data_Semina")) AndAlso
                       DT.Rows(i).Item("Data_Semina") <> "" AndAlso
                       IsDate(DT.Rows(i).Item("Data_Semina")) Then
                    Data_Semina = DT.Rows(i).Item("Data_Semina")
                Else
                    Data_Semina = #1/1/1900#
                End If

                If Not IsDBNull(DT.Rows(i).Item("Data_Raccolta")) AndAlso
                        DT.Rows(i).Item("Data_Raccolta") <> "" AndAlso
                        IsDate(DT.Rows(i).Item("Data_Raccolta")) Then
                    Data_Raccolta = DT.Rows(i).Item("Data_Raccolta")
                Else
                    Data_Raccolta = #1/1/1900#
                End If

                'If DT.Rows(i).Item("veg_cod_cliente") <> "" Then

                '    veg_des_cliente = objMS.VegDesAgea_from_VegCodAgea( _
                '                         DT.Rows(i).Item("veg_cod_cliente"), _
                '                          "", _
                '                          "", _
                '                         "", _
                '                         "", _
                '                          objParametri)

                '    If DT.Rows(i).Item("cul_cod_cliente") <> "" Then

                '        veg_des_cliente = objMS.VegDesAgea_from_VegCodAgea( _
                '                            DT.Rows(i).Item("veg_cod_cliente"), _
                '                           DT.Rows(i).Item("cul_cod_cliente"), _
                '                          cul_des_cliente, _
                '                          "", _
                '                          "", _
                '                           objParametri)
                '    End If
                'End If

                If DT.Rows(i).Item("veg_cod_cliente") <> "" AndAlso DT.Rows(i).Item("cul_cod_cliente") <> "" Then

                    veg_des_cliente = objMS.VegDesAgea_from_VegCodAgea(
                                        DT.Rows(i).Item("veg_cod_cliente"),
                                       DT.Rows(i).Item("cul_cod_cliente"),
                                      cul_des_cliente,
                                      "",
                                      "",
                                       objParametri)
                End If


                If Not IsDBNull(DT.Rows(i).Item("unita_vitata")) Then
                    Unita_Vitata = DT.Rows(i).Item("unita_vitata")
                Else
                    Unita_Vitata = 0
                End If

                If Not IsDBNull(DT.Rows(i).Item("Validita_Inizio_Impianto")) Then
                    Validita_Inizio_Impianto = DT.Rows(i).Item("Validita_Inizio_Impianto")
                Else
                    Validita_Inizio_Impianto = Data_Inizio
                End If

                If Not IsDBNull(DT.Rows(i).Item("Validita_Inizio_Distinta")) Then
                    Validita_Inizio_Esercizio = DT.Rows(i).Item("Validita_Inizio_Distinta")
                Else
                    Validita_Inizio_Esercizio = Data_Inizio
                End If

                If Not IsDBNull(DT.Rows(i).Item("Validita_Fine_Distinta")) Then
                    Validita_Fine_Esercizio = DT.Rows(i).Item("Validita_Fine_Distinta")
                Else
                    Validita_Fine_Esercizio = Data_Fine
                End If


                Dim curChiave As String =
                   DT.Rows(i).Item("Piva") & "_" &
                   DT.Rows(i).Item("Sa_Cod") & "_" &
                   DT.Rows(i).Item("Campo_Cod") & "_" &
                   DT.Rows(i).Item("Appezza") & "_" &
                   DT.Rows(i).Item("ID_Reg") & "_" &
                   DT.Rows(i).Item("Progetto_Cod")

                If ElencoAppezzamenti.Contains(curChiave) Then

                    'Inserisco la riga nel DT
                    DT_Appezzamenti_Insert(
                                    DT_Appezzamenti,
                                    False,
                                    0,
                                    "",
                                    DT.Rows(i).Item("Piva"),
                                    DT.Rows(i).Item("Sa_Cod"),
                                    DT.Rows(i).Item("Campo_Cod"),
                                    DT.Rows(i).Item("Appezza"),
                                    DT.Rows(i).Item("ID_Reg"),
                                    DT.Rows(i).Item("Progetto_Cod"),
                                    DT.Rows(i).Item("Progetto_Nome"),
                                    DT.Rows(i).Item("App_Nome"),
                                    DT.Rows(i).Item("Sup_App"),
                                    DT.Rows(i).Item("Veg_Cod"),
                                    DT.Rows(i).Item("Veg_Des"),
                                    DT.Rows(i).Item("Cul_Cod"),
                                    DT.Rows(i).Item("Cul_Des"),
                                    DT.Rows(i).Item("Grva_Cod"),
                                    DT.Rows(i).Item("Grva_Des"),
                                    DT.Rows(i).Item("Grfi_Cod"),
                                    DT.Rows(i).Item("Grfi_Des"),
                                    DT.Rows(i).Item("Cop_Cod"),
                                    DT.Rows(i).Item("Cop_Des"),
                                    DT.Rows(i).Item("Resa"),
                                    DT.Rows(i).Item("DestinazioneUso"),
                                    DT.Rows(i).Item("DestinazioneUso_Des"),
                                    Math.Round(DT.Rows(i).Item("P_Ha") * DT.Rows(i).Item("Sup_Imp")),
                                    0, 0,
                                    Validita_Inizio_Impianto,
                                    Validita_Inizio_Esercizio,
                                    Validita_Fine_Esercizio,
                                    0,
                                    Data_Semina,
                                    Data_Raccolta,
                                    String.Empty,
                                    1,
                                    "Convenzionale",
                                    Catasto,
                                    DT.Rows(i).Item("Sa_Nome"),
                                    DT.Rows(i).Item("Campo_Des"),
                                    DT.Rows(i).Item("veg_cod_cliente"),
                                    DT.Rows(i).Item("cul_cod_cliente"),
                                    veg_des_cliente,
                                    cul_des_cliente,
                                    "",
                                    "",
                                    Unita_Vitata,
                                    objParametri, 0, "",
                                    0, 0
)

                End If

            Next

        End If

    End Sub

    Public Function OperazioneDes_From_OperazioneCod(ByVal OperazioneCod As Integer) As String

        Select Case OperazioneCod
            Case enum_TipoOperazioneProgrammazioneEntita.Nessuna
                Return ""
            Case enum_TipoOperazioneProgrammazioneEntita.Confermato
                Return "Confermato"
            Case enum_TipoOperazioneProgrammazioneEntita.Nuovo_Appezzamento
                Return "Nuovo Appezzamento"
            Case enum_TipoOperazioneProgrammazioneEntita.Nuovo_Impianto
                Return "Nuovo Impianto"
            Case enum_TipoOperazioneProgrammazioneEntita.Nuova_Distinta
                Return "Nuovo Esercizio"
            Case enum_TipoOperazioneProgrammazioneEntita.Modifica_Semplice
                Return "Modificato"
            Case enum_TipoOperazioneProgrammazioneEntita.Modifica_Superficie
                Return "Superficie Modificata"
            Case enum_TipoOperazioneProgrammazioneEntita.Unione
                Return "Unione"
            Case enum_TipoOperazioneProgrammazioneEntita.Frazionamento
                Return "Frazionamento"
            Case enum_TipoOperazioneProgrammazioneEntita.Chiudi_Appezzamento
                Return "Chiuso Appezzamento"
            Case Else
                Return ""

        End Select

    End Function

    Public Sub DT_Appezzamenti_Insert(
                                ByRef DT_Appezzamenti As DataTable,
                                ByVal ChkSeleziona As Boolean,
                                ByVal Operazione_Cod As Integer,
                                ByVal Operazione_Des As String,
                                ByVal Piva As String,
                                ByVal Sa_Cod As Integer,
                                ByVal Campo_Cod As Integer,
                                ByVal Appezza As Integer,
                                ByVal ID_Reg As Integer,
                                ByVal Progetto_Cod As Integer,
                                ByVal Progetto_Nome As String,
                                ByVal App_Nome As String,
                                ByVal Sup_App As Decimal,
                                ByVal Veg_Cod As Integer,
                                ByVal Veg_Des As String,
                                ByVal Cul_Cod As Integer,
                                ByVal Cul_Des As String,
                                ByVal Grva_Cod As Integer,
                                ByVal Grva_Des As String,
                                ByVal Grfi_Cod As Integer,
                                ByVal Grfi_Des As String,
                                ByVal Cop_Cod As Integer,
                                ByVal Cop_Des As String,
                                ByVal Resa As Decimal,
                                ByVal DestinazioneUso As Integer,
                                ByVal DestinazioneUso_Des As String,
                                ByVal Num_Piante As Integer,
                                ByVal Tra_Fila As Decimal,
                                ByVal Su_Fila As Decimal,
                                ByVal Validita_Inizio_Impianto As Date,
                                ByVal Validita_Inizio As Date,
                                ByVal Validita_Fine As Date,
                                ByVal Programmazione_Entita_Cod As Integer,
                                ByVal Data_Semina As Date,
                                ByVal Data_Raccolta As Date,
                                ByVal TipoZona As String,
                                ByVal MetodoProduzione_Cod As Integer,
                                ByVal MetodoProduzione_Des As String,
                                ByVal Catasto As String,
                                ByVal Sa_Nome As String,
                                ByVal Campo_Des As String,
                                ByVal veg_cod_cliente As String,
                                ByVal cul_cod_cliente As String,
                                ByVal veg_des_cliente As String,
                                ByVal cul_des_cliente As String,
                                ByVal macrouso_cod As String,
                                ByVal macrouso_des As String,
                                ByVal Unita_Vitata As Integer,
                                ByRef objParametri As AgronicaCoreParametri,
                                Optional ByRef ElementoGrafico_Cod As Integer = 0,
                                Optional ByRef ElementoGrafico_Des As String = Nothing,
                                Optional ByVal Ribaltato As Integer = 0,
                                Optional ByVal Movimentato As Integer = 0,
                                Optional ByVal Veg_Cod_Agea As String = "",
                                Optional ByVal Cul_Cod_Agea As String = "",
                                Optional ByVal Uso_Cod_Agea As String = "",
                                Optional ByVal Occupazione_Cod_Agea As String = "",
                                Optional ByVal Destinazione_Cod_Agea As String = "",
                                Optional ByVal Qualita_Cod_Agea As String = "",
                                Optional ByVal Limite_N As String = "",
                                Optional ByVal Limite_P As String = "",
                                Optional ByVal Limite_K As String = "",
                                Optional ByVal Data_Fioritura_Prevista As Date = AGRODATAINIZIO,
                                Optional ByVal Veg_Cod_Prec As Integer = 0,
                                Optional ByVal Veg_Cod_Prec2 As Integer = 0,
                                Optional ByVal Veg_Cod_Prec3 As Integer = 0,
                                Optional ByVal Veg_Cod_Prec4 As Integer = 0,
                                Optional ByVal Piano_Semina As String = "",
                                Optional ByVal Codice_Contratto As String = "",
                                Optional ByVal Disciplinare_Cod As Integer = 0,
                                Optional ByVal Regolamento_Cod As Integer = 0,
                                Optional ByVal Stato_Cod As Integer = 0,
                                Optional ByVal Codice_Fiscale_Tecnico As String = "",
                                Optional ByVal Stato_Ribaltamento As Integer = 0,
                                Optional ByVal provenienza_fascicolo As String = "",
                                Optional ByVal datoGis As String = "",
                                Optional ByVal IAF As String = "",
                                Optional ByVal Pratica_Cod As String = "",
                                Optional ByVal Regolamento_Concimazione_Cod As Integer = 0,
                                Optional ByVal Flag_PubblicoPrivato As Integer = 0,
                                Optional ByVal id_tr As Integer = 0,
                                Optional ByVal DistBZ_CorpiIdrici As Double = 0,
                                Optional ByVal DistBZ_AreeResPub As Double = 0,
                                Optional ByVal DistBZ_Allevamenti As Double = 0,
                                Optional ByVal DistBZ_VegNatNonColt As Double = 0,
                                Optional ByVal wkt As String = "",
                                Optional ByVal wkt_georiferimento_cod As String = "-1",
                                Optional ByVal SupBZ_Riduzione As Double = 0,
                                Optional ByVal Riferimento_Alfanumerico_Appezzamento As String = "",
                                Optional ByVal Isola As String = "",
                                Optional ByVal CapitolatoPrivato As String = "",
                                Optional ByVal CapitolatoPrivato_Des As String = "",
                                Optional ByVal Finalita_Concimazione_Impianto As Integer = 0,
                                Optional ByVal Cod_Indirizzo As Integer = 0,
                                Optional ByVal ind_des As String = "",
                                Optional ByVal frz_des As String = "",
                                Optional ByVal CAP As String = "",
                                Optional ByVal com_des As String = "",
                                Optional ByVal pro_cod As String = "",
                                Optional ByVal stato As String = "",
                                Optional ByVal stato_des As String = "",
                                Optional ByVal note As String = "",
                                Optional ByVal pro_cod_istat As String = "",
                                Optional ByVal com_cod_istat As String = "",
                                Optional ByVal KPIN As String = "",
                                Optional ByVal Block_Name As String = "",
                                Optional ByVal Foral_Cod As Integer = 0,
                                Optional ByVal Foral_Des As String = "",
                                Optional ByVal Data_Inizio_Portinnesto As String = "",
                                Optional ByVal Data_Creazione As Date = AGRODATAINIZIO,
                                Optional ByVal Data_Modifica As Date = AGRODATAINIZIO,
                                Optional ByVal ZespriFase_Cod As Integer = 0,
                                Optional ByVal ZespriFase_Des As String = "",
                                Optional ByVal ZespriTipo_Cod As Integer = 0,
                                Optional ByVal ZespriTipo_Des As String = "",
                                Optional ByVal ZespriGrower_Cod As Integer = 0,
                                Optional ByVal ZespriGrower_Des As String = "",
                                Optional ByVal Num_Piante_Femmine As Integer = 0,
                                Optional ByVal Num_Piante_Maschi As Integer = 0,
                                Optional ByVal Port_Cod As Integer = 0,
                                Optional ByVal Port_Des As String = "",
                                Optional ByVal TipologiaInnestoTrapianto_Cod As Integer = 0,
                                Optional ByVal TipologiaInnestoTrapianto_Des As String = "",
                                Optional ByVal Mat_Cod As Integer = 0,
                                Optional ByVal Mat_Des As String = "",
                                Optional ByVal PivaReale As String = ""
                                )

        Dim DR As DataRow
        Dim UNID_APP As String = ""

        Dim UNID_APP_OLD As String = ""

        Key_Appezzamento_SET(UNID_APP, Piva, Sa_Cod, Campo_Cod, Appezza, 0, 0)

        DR = DT_Appezzamenti.NewRow

        DR.Item("UNID_APP") = UNID_APP

        DR.Item("Operazione_Cod") = Operazione_Cod
        DR.Item("Operazione_Des") = Operazione_Des

        DR.Item("Programmazione_Entita_Cod") = Programmazione_Entita_Cod

        DR.Item("Piva") = Piva

        If PivaReale <> "" Then
            DR.Item("partitaIvaReale") = PivaReale
        End If

        DR.Item("Sa_Cod") = Sa_Cod
        DR.Item("Campo_Cod") = Campo_Cod
        DR.Item("Appezza") = Appezza
        DR.Item("ID_Reg") = ID_Reg
        DR.Item("Progetto_Cod") = Progetto_Cod
        DR.Item("Sa_Nome") = Sa_Nome
        DR.Item("Campo_Des") = Campo_Des
        DR.Item("App_Nome") = App_Nome
        DR.Item("Sup_App") = Sup_App
        DR.Item("Veg_Cod") = Veg_Cod
        DR.Item("Veg_Des") = Veg_Des
        DR.Item("Cul_Cod") = Cul_Cod
        DR.Item("Cul_Des") = Cul_Des
        DR.Item("Grva_Cod") = Grva_Cod
        DR.Item("Grva_Des") = Grva_Des
        DR.Item("Grfi_Cod") = Grfi_Cod
        DR.Item("Grfi_Des") = Grfi_Des
        DR.Item("Cop_Cod") = Cop_Cod
        DR.Item("Cop_Des") = Cop_Des
        DR.Item("Progetto_Nome") = Progetto_Nome
        DR.Item("Resa") = Resa
        DR.Item("DestinazioneUso") = DestinazioneUso
        DR.Item("DestinazioneUso_des") = DestinazioneUso_Des
        DR.Item("Num_Piante") = Num_Piante
        DR.Item("Tra_Fila") = Tra_Fila
        DR.Item("Su_Fila") = Su_Fila
        DR.Item("Validita_Inizio_Impianto") = Validita_Inizio_Impianto.ToShortDateString
        DR.Item("Validita_Inizio") = Validita_Inizio.ToShortDateString
        DR.Item("Validita_Fine") = Validita_Fine.ToShortDateString
        DR.Item("Data_Semina") = Data_Semina.ToShortDateString  'IIf((Data_Semina = #1/1/1900#), "-", Data_Semina.ToShortDateString)
        DR.Item("Data_Raccolta") = Data_Raccolta.ToShortDateString    'IIf((Data_Raccolta = #1/1/1900#), "-", Data_Raccolta.ToShortDateString)
        DR.Item("TipoZona") = TipoZona
        DR.Item("MetodoProduzione_Cod") = MetodoProduzione_Cod
        DR.Item("MetodoProduzione_Des") = MetodoProduzione_Des

        DR.Item("veg_Cod_cliente") = veg_cod_cliente
        DR.Item("cul_Cod_cliente") = cul_cod_cliente
        DR.Item("veg_des_cliente") = veg_des_cliente
        DR.Item("cul_des_cliente") = cul_des_cliente

        DR.Item("macrouso_cod") = macrouso_cod
        DR.Item("macrouso_des") = macrouso_des

        DR.Item("Catasto") = Catasto

        DR.Item("unita_vitata") = Unita_Vitata
        If ElementoGrafico_Cod <> 0 Then
            DR.Item("ElementoGrafico_Cod") = ElementoGrafico_Cod
        End If
        If ElementoGrafico_Des IsNot Nothing Then
            DR.Item("ElementoGrafico_des") = ElementoGrafico_Des
        End If

        DR.Item("ribaltato") = Ribaltato
        DR.Item("movimentato") = Movimentato

        DR.Item("Veg_Cod_Agea") = Veg_Cod_Agea
        DR.Item("Cul_Cod_Agea") = Cul_Cod_Agea
        DR.Item("Uso_Cod_Agea") = Uso_Cod_Agea
        DR.Item("Occupazione_Cod_Agea") = Occupazione_Cod_Agea
        DR.Item("Destinazione_Cod_Agea") = Destinazione_Cod_Agea
        DR.Item("Qualita_Cod_Agea") = Qualita_Cod_Agea

        DR.Item("Limite_N") = Limite_N
        DR.Item("Limite_P") = Limite_P
        DR.Item("Limite_K") = Limite_K

        DR.Item("Data_Fioritura_Prevista") = Data_Fioritura_Prevista.ToShortDateString

        DR.Item("Veg_Cod_Prec") = Veg_Cod_Prec
        DR.Item("Veg_Cod_Prec2") = Veg_Cod_Prec2
        DR.Item("Veg_Cod_Prec3") = Veg_Cod_Prec3
        DR.Item("Veg_Cod_Prec4") = Veg_Cod_Prec4

        DR.Item("Piano_Semina") = Piano_Semina
        DR.Item("Codice_Contratto") = Codice_Contratto

        DR.Item("Disciplinare_Cod") = Disciplinare_Cod
        DR.Item("Regolamento_Cod") = Regolamento_Cod
        DR.Item("Stato_Cod") = Stato_Cod
        DR.Item("Codice_Fiscale_Tecnico") = Codice_Fiscale_Tecnico

        DR.Item("Stato_Ribaltamento") = Stato_Ribaltamento
        DR.Item("provenienza_fascicolo") = provenienza_fascicolo

        DR.Item("datoGis") = datoGis
        DR.Item("IAF") = IAF
        DR.Item("Pratica_Cod") = Pratica_Cod

        DR.Item("Regolamento_Concimazione_Cod") = Regolamento_Concimazione_Cod
        DR.Item("Flag_PubblicoPrivato") = Flag_PubblicoPrivato
        DR.Item("id_tr") = id_tr

        DR.Item("DistBZ_CorpiIdrici") = DistBZ_CorpiIdrici
        DR.Item("DistBZ_AreeResPub") = DistBZ_AreeResPub
        DR.Item("DistBZ_Allevamenti") = DistBZ_Allevamenti
        DR.Item("DistBZ_VegNatNonColt") = DistBZ_VegNatNonColt

        DR.Item("SupBZ_Riduzione") = SupBZ_Riduzione

        DR.Item("wkt") = wkt
        DR.Item("wkt_georiferimento_cod") = wkt_georiferimento_cod

        DR.Item("Riferimento_Alfanumerico_Appezzamento") = Riferimento_Alfanumerico_Appezzamento
        DR.Item("Isola") = Isola

        DR.Item("CapitolatoPrivato") = CapitolatoPrivato
        DR.Item("CapitolatoPrivato_Des") = CapitolatoPrivato_Des

        DR.Item("Finalita_Concimazione_Impianto") = Finalita_Concimazione_Impianto

        'INDIRIZZO
        DR.Item("Cod_Indirizzo") = Cod_Indirizzo
        DR.Item("ind_des") = ind_des
        DR.Item("frz_des") = frz_des
        DR.Item("CAP") = CAP
        DR.Item("com_des_indirizzo") = com_des
        DR.Item("pro_cod_indirizzo") = pro_cod
        DR.Item("stato_indirizzo") = stato
        DR.Item("stato_indirizzo_des") = stato_des
        DR.Item("note_indirizzo") = note
        DR.Item("pro_cod_istat_indirizzo") = pro_cod_istat
        DR.Item("com_cod_istat_indirizzo") = com_cod_istat

        DR.Item("KPIN") = KPIN
        DR.Item("Block_Name") = Block_Name

        DR.Item("Foral_Cod") = Foral_Cod
        DR.Item("Foral_Des") = Foral_Des

        DR.Item("Data_Inizio_Portinnesto") = Data_Inizio_Portinnesto

        DR.Item("Data_Creazione") = Data_Creazione
        DR.Item("Data_Modifica") = Data_Modifica

        'ZESPRI
        DR.Item("ZespriFase_Cod") = ZespriFase_Cod
        DR.Item("ZespriFase_Des") = ZespriFase_Des
        DR.Item("ZespriTipo_Cod") = ZespriTipo_Cod
        DR.Item("ZespriTipo_Des") = ZespriTipo_Des
        DR.Item("ZespriGrower_Cod") = ZespriGrower_Cod
        DR.Item("ZespriGrower_Des") = ZespriGrower_Des

        'NUM PIANTE MF
        DR.Item("Num_Piante_Femmine") = Num_Piante_Femmine
        DR.Item("Num_Piante_Maschi") = Num_Piante_Maschi

        'PORTINNESTO
        DR.Item("Port_Cod") = Port_Cod
        DR.Item("Port_Des") = Port_Des

        'Tipologia di innesto o trapianto
        DR.Item("TipologiaInnestoTrapianto_Cod") = TipologiaInnestoTrapianto_Cod
        DR.Item("TipologiaInnestoTrapianto_Des") = TipologiaInnestoTrapianto_Des

        'Tipologia di innesto o trapianto
        DR.Item("Mat_Cod") = Mat_Cod
        DR.Item("Mat_Des") = Mat_Des

        DT_Appezzamenti.Rows.Add(DR)

    End Sub

    Public Sub Key_Appezzamento_SET(
                                ByRef UNID_APP As String,
                                ByVal Piva As String,
                                ByVal Sa_Cod As Integer,
                                ByVal Campo_Cod As Integer,
                                ByVal Appezza As Integer,
                                ByVal Id_Reg As Integer,
                                ByVal Progetto_Cod As Integer
                                )

        UNID_APP = Piva & "_" & Sa_Cod.ToString & "_" & Campo_Cod.ToString & "_" & Appezza.ToString & "_" & Id_Reg.ToString & "_" & Progetto_Cod.ToString

    End Sub

    Public Sub Key_Appezzamento_GET(
                                ByVal UNID_APP As String,
                                ByRef Piva As String,
                                ByRef Sa_Cod As Integer,
                                ByRef Campo_Cod As Integer,
                                ByRef Appezza As Integer,
                                ByRef Id_Reg As Integer,
                                ByRef Progetto_Cod As Integer
                                )

        Dim Testo As String()
        Testo = Split(UNID_APP, "_")
        Piva = Testo(0)
        Sa_Cod = CInt(Testo(1))
        Campo_Cod = CInt(Testo(2))
        Appezza = CInt(Testo(3))
        Id_Reg = CInt(Testo(4))
        Progetto_Cod = CInt(Testo(5))

    End Sub

    Public Sub DT_Appezzamenti_Delete(
                        ByRef DT_Appezzamenti As DataTable,
                        ByVal Piva As String,
                        ByVal Sa_Cod As Integer,
                        ByVal Campo_Cod As Integer,
                        ByVal Appezza As Integer,
                        ByVal Id_Reg As Integer,
                        ByVal Programmazione_Entita_Cod As Integer)

        Dim i As Integer

        For i = 0 To DT_Appezzamenti.Rows.Count - 1

            If (DT_Appezzamenti.Rows(i).Item("Piva") = Piva) And
               (DT_Appezzamenti.Rows(i).Item("Sa_Cod") = Sa_Cod) And
               (DT_Appezzamenti.Rows(i).Item("Appezza") = Appezza) And
               (DT_Appezzamenti.Rows(i).Item("ID_Reg") = Id_Reg) And
               (DT_Appezzamenti.Rows(i).Item("Programmazione_Entita_Cod") = Programmazione_Entita_Cod) Then

                DT_Appezzamenti.Rows(i).Delete()
                DT_Appezzamenti.AcceptChanges()
                Exit For
            End If

        Next

    End Sub

#End Region

#Region "Appezzamenti_Eliminati"

    Public Sub DT_Appezzamenti_Eliminati_Crea(ByRef DT As DataTable)

        DT.Columns.Add(New DataColumn("Piva", GetType(String)))
        DT.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Campo_Cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Appezza", GetType(Integer)))
        DT.Columns.Add(New DataColumn("ID_Reg", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Progetto_Cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Programmazione_Entita_Cod", GetType(String)))
        DT.Columns.Add(New DataColumn("Operazione_Cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Operazione", GetType(String)))
        DT.Columns.Add(New DataColumn("Sa_Nome", GetType(String)))
        DT.Columns.Add(New DataColumn("Campo_Des", GetType(String)))
        DT.Columns.Add(New DataColumn("App_nome", GetType(String)))
        DT.Columns.Add(New DataColumn("Sup_App", GetType(String)))
        DT.Columns.Add(New DataColumn("Veg_Des", GetType(String)))
        DT.Columns.Add(New DataColumn("Cul_Des", GetType(String)))
        DT.Columns.Add(New DataColumn("Grfi_Des", GetType(String)))
        DT.Columns.Add(New DataColumn("Progetto_Nome", GetType(String)))
        DT.Columns.Add(New DataColumn("DestinazioneUso_Des", GetType(String)))

        DT.Columns.Add(New DataColumn("Data_Chiusura", GetType(String)))

        DT.Columns.Add(New DataColumn("UNID_APP_NEW", GetType(String)))

    End Sub

    Public Sub Key_Appezzamento_Eliminato_SET(
                                ByRef UNID_APP As String,
                                ByVal Piva As String,
                                ByVal Sa_Cod As Integer,
                                ByVal Appezza As Integer,
                                ByVal Id_Reg As Integer)

        UNID_APP = Piva & "_" & Sa_Cod.ToString & "_" & Appezza & "_" & Id_Reg

    End Sub

    Public Sub Key_Appezzamento_Eliminato_GET(
                                ByVal UNID_APP As String,
                                ByRef Piva As String,
                                ByRef Sa_Cod As Integer,
                                ByRef Appezza As Integer,
                                ByRef Id_Reg As Integer)

        Dim Testo As String()
        Testo = Split(UNID_APP, "_")
        Piva = Testo(0)
        Sa_Cod = CInt(Testo(1))
        Appezza = CInt(Testo(2))
        Id_Reg = CInt(Testo(3))

    End Sub

    Public Sub DT_Appezzamenti_Eliminati_Insert(
                            ByRef DT_Appezzamenti_Eliminati As DataTable,
                            ByVal Piva As String,
                            ByVal Sa_Cod As Integer,
                            ByVal Campo_Cod As Integer,
                            ByVal Appezza As Integer,
                            ByVal ID_Reg As Integer,
                            ByVal Progetto_Cod As Integer,
                            ByVal Programmazione_Entita_Cod As Integer,
                            ByVal Operazione_Cod As Integer,
                            ByVal Data_Chiusura As String,
                            Optional ByVal Appezza_new As String = "",
                            Optional ByVal ID_Reg_new As String = "",
                            Optional ByVal Operazione As String = "",
                            Optional ByVal Sa_Nome As String = "",
                            Optional ByVal Campo_Des As String = "",
                            Optional ByVal App_nome As String = "",
                            Optional ByVal Sup_App As String = "",
                            Optional ByVal Veg_Des As String = "",
                            Optional ByVal Cul_Des As String = "",
                            Optional ByVal Grfi_Des As String = "",
                            Optional ByVal Progetto_Nome As String = "",
                            Optional ByVal DestinazioneUso_Des As String = ""
                            )

        Dim DR As DataRow
        Dim UNID_APP As String = ""
        Dim UNID_APP_NEW As String = ""

        Key_Appezzamento_SET(UNID_APP, Piva, Sa_Cod, Campo_Cod, Appezza, ID_Reg, Progetto_Cod)

        If Appezza_new <> String.Empty AndAlso ID_Reg_new <> String.Empty Then
            Key_Appezzamento_SET(UNID_APP_NEW, Piva, Sa_Cod, Campo_Cod, Appezza_new, ID_Reg_new, Progetto_Cod)
        End If

        DR = DT_Appezzamenti_Eliminati.NewRow

        DR.Item("Piva") = Piva
        DR.Item("Sa_Cod") = Sa_Cod
        DR.Item("Campo_Cod") = Campo_Cod
        DR.Item("Appezza") = Appezza
        DR.Item("ID_Reg") = ID_Reg
        DR.Item("Progetto_Cod") = Progetto_Cod
        DR.Item("Programmazione_Entita_Cod") = Programmazione_Entita_Cod

        DR.Item("Operazione_Cod") = Operazione_Cod
        DR.Item("Operazione") = Operazione
        DR.Item("Sa_Nome") = Sa_Nome
        DR.Item("Campo_Des") = Campo_Des
        DR.Item("App_nome") = App_nome
        DR.Item("Sup_App") = Sup_App
        DR.Item("Veg_Des") = Veg_Des
        DR.Item("Cul_Des") = Cul_Des
        DR.Item("Grfi_Des") = Grfi_Des
        DR.Item("Progetto_Nome") = Progetto_Nome
        DR.Item("DestinazioneUso_Des") = DestinazioneUso_Des

        DR.Item("Data_Chiusura") = Data_Chiusura

        DR.Item("UNID_APP_NEW") = UNID_APP_NEW

        DT_Appezzamenti_Eliminati.Rows.Add(DR)

    End Sub

    Public Sub DT_Appezzamenti_Eliminati_Update(
                            ByRef DT_Appezzamenti_Eliminati As DataTable,
                            ByVal Piva As String,
                            ByVal Sa_Cod As Integer,
                            ByVal Campo_Cod As Integer,
                            ByVal Appezza As Integer,
                            ByVal ID_Reg As Integer,
                            ByVal Progetto_Cod As Integer,
                            ByVal Programmazione_Entita_Cod As Integer,
                            Optional ByVal Appezza_new As String = "",
                            Optional ByVal ID_Reg_new As String = "")

        Dim i As Integer
        Dim UNID_APP_NEW As String = ""

        If Appezza_new <> String.Empty AndAlso ID_Reg_new <> String.Empty Then
            Key_Appezzamento_SET(UNID_APP_NEW, Piva, Sa_Cod, Campo_Cod, Appezza_new, ID_Reg_new, Progetto_Cod)
        End If

        'cerco la riga da modificare
        For i = 0 To DT_Appezzamenti_Eliminati.Rows.Count - 1

            If DT_Appezzamenti_Eliminati.Rows(i).Item("Piva") = Piva And
               DT_Appezzamenti_Eliminati.Rows(i).Item("Campo_Cod") = Campo_Cod And
               DT_Appezzamenti_Eliminati.Rows(i).Item("Sa_Cod") = Sa_Cod And
               DT_Appezzamenti_Eliminati.Rows(i).Item("Appezza") = Appezza And
               DT_Appezzamenti_Eliminati.Rows(i).Item("Id_Reg") = ID_Reg Then

                If Progetto_Cod <> 0 Then
                    DT_Appezzamenti_Eliminati.Rows(i).Item("Progetto_Cod") = Progetto_Cod
                End If

                If Programmazione_Entita_Cod <> 0 Then
                    DT_Appezzamenti_Eliminati.Rows(i).Item("Programmazione_Entita_Cod") = Programmazione_Entita_Cod
                End If

                If UNID_APP_NEW <> String.Empty Then
                    DT_Appezzamenti_Eliminati.Rows(i).Item("UNID_APP_NEW") = UNID_APP_NEW
                End If

                Exit For

            End If

        Next


    End Sub

    Public Sub DT_Appezzamenti_Eliminati_Delete(
                            ByRef DT_Appezzamenti_Eliminati As DataTable,
                            ByVal Piva As String,
                            ByVal Sa_Cod As Integer,
                            ByVal Campo_Cod As Integer,
                            ByVal Appezza As Integer,
                            ByVal Id_Reg As Integer)

        Dim i As Integer

        For i = 0 To DT_Appezzamenti_Eliminati.Rows.Count - 1

            If (DT_Appezzamenti_Eliminati.Rows(i).Item("Piva") = Piva) And
               (DT_Appezzamenti_Eliminati.Rows(i).Item("Sa_Cod") = Sa_Cod) And
               (DT_Appezzamenti_Eliminati.Rows(i).Item("Appezza") = Appezza) And
               (DT_Appezzamenti_Eliminati.Rows(i).Item("ID_Reg") = Id_Reg) Then

                DT_Appezzamenti_Eliminati.Rows(i).Delete()
                DT_Appezzamenti_Eliminati.AcceptChanges()
                Exit For
            End If

        Next

    End Sub

#End Region

#Region "Campi"

    Public Sub DT_Campi_Crea(ByRef DT As DataTable)

        Dim i As Integer

        DT.Columns.Add(New DataColumn("UNID_CAMPO", GetType(String)))

        DT.Columns.Add(New DataColumn("Operazione_Cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Operazione_Des", GetType(String)))

        DT.Columns.Add(New DataColumn("Piva", GetType(String)))
        DT.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Campo_Cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Campo_Des", GetType(String)))

        For i = 1 To 12
            DT.Columns.Add(New DataColumn("Appezza" & Right("00" + i, 2) & "1", GetType(Integer)))
            DT.Columns.Add(New DataColumn("ID_Reg" & Right("00" + i, 2) & "1", GetType(Integer)))
            DT.Columns.Add(New DataColumn("Progetto_Cod" & Right("00" + i, 2) & "1", GetType(Integer)))
            DT.Columns.Add(New DataColumn("Num_Piante" & Right("00" + i, 2) & "1", GetType(Integer)))
            DT.Columns.Add(New DataColumn("Sup_App" & Right("00" + i, 2) & "1", GetType(Decimal)))
            DT.Columns.Add(New DataColumn("Appezza" & Right("00" + i, 2) & "2", GetType(Integer)))
            DT.Columns.Add(New DataColumn("ID_Reg" & Right("00" + i, 2) & "2", GetType(Integer)))
            DT.Columns.Add(New DataColumn("Progetto_Cod" & Right("00" + i, 2) & "2", GetType(Integer)))
            DT.Columns.Add(New DataColumn("Num_Piante" & Right("00" + i, 2) & "2", GetType(Integer)))
            DT.Columns.Add(New DataColumn("Sup_App" & Right("00" + i, 2) & "2", GetType(Decimal)))
        Next

        DT.Columns.Add(New DataColumn("Sup_Tot", GetType(Decimal)))
        DT.Columns.Add(New DataColumn("Num_Piante_Tot", GetType(Integer)))

        DT.Columns.Add(New DataColumn("Veg_Cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Veg_Des", GetType(String)))

        DT.Columns.Add(New DataColumn("Cul_Cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Cul_Des", GetType(String)))

        DT.Columns.Add(New DataColumn("Grva_Cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Grva_Des", GetType(String)))

        DT.Columns.Add(New DataColumn("Grfi_Cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Grfi_Des", GetType(String)))

        DT.Columns.Add(New DataColumn("Cop_Cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Cop_Des", GetType(String)))

        DT.Columns.Add(New DataColumn("Resa", GetType(Decimal)))
        DT.Columns.Add(New DataColumn("DestinazioneUso", GetType(Integer)))
        DT.Columns.Add(New DataColumn("DestinazioneUso_Des", GetType(String)))

        DT.Columns.Add(New DataColumn("MetodoProduzione_Cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("MetodoProduzione_Des", GetType(String)))

    End Sub

    Public Sub Key_Campo_SET(
                        ByRef UNID_CAMPO As String,
                        ByVal Piva As String,
                        ByVal Sa_Cod As Integer,
                        ByVal Campo_Cod As Integer)

        UNID_CAMPO = Piva & "_" & Sa_Cod.ToString & "_" & Campo_Cod

    End Sub

    Public Sub Key_Campo_GET(
                        ByVal UNID_CAMPO As String,
                        ByRef Piva As String,
                        ByRef Sa_Cod As Integer,
                        ByRef Campo_Cod As Integer)

        Dim Testo As String()
        Testo = Split(UNID_CAMPO, "_")
        Piva = Testo(0)
        Sa_Cod = CInt(Testo(1))
        Campo_Cod = CInt(Testo(2))

    End Sub

    Public Sub DT_Campi_Insert(
                            ByRef DT_Campi As DataTable,
                            ByVal Operazione_Cod As Integer,
                            ByVal Operazione_Des As String,
                            ByVal Piva As String,
                            ByVal Sa_Cod As Integer,
                            ByVal Campo_Cod As Integer,
                            ByVal Campo_Des As String,
                            ByVal Sup_Tot As Decimal,
                            ByVal Num_Piante_Tot As Integer,
                            ByVal Array() As Object,
                            ByVal Veg_Cod As Integer,
                            ByVal Veg_Des As String,
                            ByVal Cul_Cod As Integer,
                            ByVal Cul_Des As String,
                            ByVal Grva_Cod As Integer,
                            ByVal Grva_Des As String,
                            ByVal Grfi_Cod As Integer,
                            ByVal Grfi_Des As String,
                            ByVal Cop_Cod As Integer,
                            ByVal Cop_Des As String,
                            ByVal Resa As Decimal,
                            ByVal DestinazioneUso As Integer,
                            ByVal DestinazioneUso_Des As String,
                            ByVal MetodoProduzione_Cod As Integer,
                            ByVal MetodoProduzione_Des As String)

        Dim DR As DataRow
        Dim UNID_CAMPO As String = ""
        Dim i As Integer
        Dim n As Integer = 0

        Key_Campo_SET(UNID_CAMPO, Piva, Sa_Cod, Campo_Cod)

        DR = DT_Campi.NewRow

        DR.Item("UNID_CAMPO") = UNID_CAMPO

        DR.Item("Operazione_Cod") = Operazione_Cod
        DR.Item("Operazione_Des") = Operazione_Des

        DR.Item("Piva") = Piva
        DR.Item("Sa_Cod") = Sa_Cod
        DR.Item("Campo_Cod") = Campo_Cod
        DR.Item("Campo_Des") = Campo_Des

        For i = 1 To 12

            DR.Item("Appezza" & Right("00" + i, 2) & "1") = Array(0 + n)
            DR.Item("ID_Reg" & Right("00" + i, 2) & "1") = Array(1 + n)
            DR.Item("Progetto_Cod" & Right("00" + i, 2) & "1") = Array(2 + n)
            DR.Item("Num_Piante" & Right("00" + i, 2) & "1") = Array(3 + n)
            DR.Item("Sup_App" & Right("00" + i, 2) & "1") = Array(4 + n)
            DR.Item("Appezza" & Right("00" + i, 2) & "2") = Array(5 + n)
            DR.Item("ID_Reg" & Right("00" + i, 2) & "2") = Array(6 + n)
            DR.Item("Progetto_Cod" & Right("00" + i, 2) & "2") = Array(7 + n)
            DR.Item("Num_Piante" & Right("00" + i, 2) & "2") = Array(8 + n)
            DR.Item("Sup_App" & Right("00" + i, 2) & "2") = Array(9 + n)

            n += 10

        Next

        DR.Item("Sup_Tot") = Sup_Tot
        DR.Item("Num_Piante_Tot") = Num_Piante_Tot

        DR.Item("Veg_Cod") = Veg_Cod
        DR.Item("Veg_Des") = Veg_Des
        DR.Item("Cul_Cod") = Cul_Cod
        DR.Item("Cul_Des") = Cul_Des
        DR.Item("Grva_Cod") = Grva_Cod
        DR.Item("Grva_Des") = Grva_Des
        DR.Item("Grfi_Cod") = Grfi_Cod
        DR.Item("Grfi_Des") = Grfi_Des
        DR.Item("Cop_Cod") = Cop_Cod
        DR.Item("Cop_Des") = Cop_Des
        DR.Item("Resa") = Resa
        DR.Item("DestinazioneUso") = DestinazioneUso
        DR.Item("DestinazioneUso_des") = DestinazioneUso_Des

        DR.Item("MetodoProduzione_Cod") = MetodoProduzione_Cod
        DR.Item("MetodoProduzione_Des") = MetodoProduzione_Des

        DT_Campi.Rows.Add(DR)

    End Sub

    Public Sub DT_Campi_Update(
                                ByRef DT_Campi As DataTable,
                                ByVal Piva As String,
                                ByVal Sa_Cod As Integer,
                                ByVal Campo_Cod As Integer,
                                ByVal Campo_Des As String,
                                ByVal Sup_Tot As Decimal,
                                ByVal Num_Piante_Tot As Integer,
                                ByVal Veg_Cod As Integer,
                                ByVal Veg_Des As String,
                                ByVal Cul_Cod As Integer,
                                ByVal Cul_Des As String,
                                ByVal Grva_Cod As Integer,
                                ByVal Grva_Des As String,
                                ByVal Grfi_Cod As Integer,
                                ByVal Grfi_Des As String,
                                ByVal Cop_Cod As Integer,
                                ByVal Cop_Des As String,
                                ByVal Resa As Decimal,
                                ByVal DestinazioneUso As Integer,
                                ByVal DestinazioneUso_Des As String,
                                ByVal MetodoProduzione_Cod As Integer,
                                ByVal MetodoProduzione_Des As String)

        Dim UNID_CAMPO As String = ""
        Dim i As Integer

        Key_Campo_SET(UNID_CAMPO, Piva, Sa_Cod, Campo_Cod)

        'cerco la riga da modificare
        For i = 0 To DT_Campi.Rows.Count - 1

            If DT_Campi.Rows(i).Item("UNID_CAMPO") = UNID_CAMPO Then

                DT_Campi.Rows(i).Item("Campo_Des") = Campo_Des
                DT_Campi.Rows(i).Item("Sup_Tot") = Sup_Tot
                DT_Campi.Rows(i).Item("Num_Piante_Tot") = Num_Piante_Tot
                DT_Campi.Rows(i).Item("Veg_Cod") = Veg_Cod
                DT_Campi.Rows(i).Item("Veg_Des") = Veg_Des
                DT_Campi.Rows(i).Item("Cul_Cod") = Cul_Cod
                DT_Campi.Rows(i).Item("Cul_Des") = Cul_Des
                DT_Campi.Rows(i).Item("Grva_Cod") = Grva_Cod
                DT_Campi.Rows(i).Item("Grva_Des") = Grva_Des
                DT_Campi.Rows(i).Item("Grfi_Cod") = Grfi_Cod
                DT_Campi.Rows(i).Item("Grfi_Des") = Grfi_Des
                DT_Campi.Rows(i).Item("Cop_Cod") = Cop_Cod
                DT_Campi.Rows(i).Item("Cop_Des") = Cop_Des
                DT_Campi.Rows(i).Item("Resa") = Resa
                DT_Campi.Rows(i).Item("DestinazioneUso") = DestinazioneUso
                DT_Campi.Rows(i).Item("DestinazioneUso_des") = DestinazioneUso_Des
                DT_Campi.Rows(i).Item("MetodoProduzione_Cod") = MetodoProduzione_Cod
                DT_Campi.Rows(i).Item("MetodoProduzione_Des") = MetodoProduzione_Des

                Exit For

            End If

        Next


    End Sub

    Public Sub DT_Campi_Delete(
                            ByRef DT_Campi As DataTable,
                            ByVal Piva As String,
                            ByVal Sa_Cod As Integer,
                            ByVal Campo_Cod As Integer)

        Dim i As Integer

        For i = 0 To DT_Campi.Rows.Count - 1

            If (DT_Campi.Rows(i).Item("Piva") = Piva) And
                       (DT_Campi.Rows(i).Item("Sa_Cod") = Sa_Cod) And
                       (DT_Campi.Rows(i).Item("Campo_Cod") = Campo_Cod) Then

                DT_Campi.Rows(i).Delete()
                DT_Campi.AcceptChanges()
                Exit For

            End If

        Next

    End Sub

    Public Sub DT_Campi_Da_Appezzamenti(ByVal DT_Campi As DataTable, ByRef DT_Appezzamenti As DataTable)

        Dim i, j, x As Integer

        Dim strCampi As String()
        Dim Dr_Campo As DataRow()
        Dim Dr_Appezzamento As DataRow()

        Dim Piva As String = ""
        Dim Sa_Cod As Integer
        Dim Campo_Cod As Integer
        Dim Id_Reg As Integer = 0
        Dim Progetto_Cod As Integer = 0
        Dim Progetto_Nome As String = ""
        Dim App_Nome As String = ""
        Dim Campo_Des As String = ""
        Dim ArrayApp As Object()

        Dim Operazione As Integer
        Dim OperazioneDes As String = ""
        Dim Sup_App As Decimal = 0
        Dim Num_Piante As Integer = 0
        Dim Veg_Cod As Integer = 0
        Dim Veg_Des As String = ""
        Dim Cul_Cod As Integer = 0
        Dim Cul_Des As String = ""
        Dim Grva_Cod As Integer = 0
        Dim Grva_Des As String = ""
        Dim Grfi_Cod As Integer = 0
        Dim Grfi_Des As String = ""
        Dim Cop_Cod As Integer = -1
        Dim Cop_Des As String = ""
        Dim Resa As Decimal = 0
        Dim DestinazioneUso As Integer = 0
        Dim DestinazioneUso_Des As String = ""

        Dim MetodoProduzione_Cod As Integer = 0
        Dim MetodoProduzione_Des As String = ""

        Dim Validita_inizio_Entita As Date
        Dim Anno As Integer

        Dim objDP As New AgronicaCoreDataProvider.DatatableUtility
        strCampi = objDP.SelectDistinct(DT_Appezzamenti, "Campo_Cod")

        If strCampi IsNot Nothing Then

            For i = 0 To strCampi.Length - 1

                Campo_Cod = strCampi(i)

                'inizializzo l'array degli appezzamenti

                'ricavo gli altri 23 APPEZZA
                ReDim Preserve ArrayApp(120)
                For x = 0 To 119
                    ArrayApp(x) = 0
                Next

                'ricavo il record relativo al CAMPO
                Dr_Campo = DT_Appezzamenti.Select("Campo_Cod=" & Campo_Cod & " And Appezza=0")

                Dr_Appezzamento = DT_Appezzamenti.Select("Campo_Cod=" & Campo_Cod & " And Appezza<>0", "Validita_Inizio ASC")

                If Dr_Campo IsNot Nothing Then

                    If Dr_Campo.Length > 0 Then

                        Operazione = Dr_Campo(0).Item("Operazione_Cod")
                        OperazioneDes = Dr_Campo(0).Item("Operazione_Des")

                        Piva = Dr_Campo(0).Item("piva")
                        Sa_Cod = Dr_Campo(0).Item("sa_cod")

                        Sup_App = Dr_Campo(0).Item("sup_app")
                        Num_Piante = Dr_Campo(0).Item("num_piante")

                        Veg_Cod = Dr_Campo(0).Item("veg_cod")
                        Veg_Des = Dr_Campo(0).Item("veg_des")
                        Cul_Cod = Dr_Campo(0).Item("cul_cod")
                        Cul_Des = Dr_Campo(0).Item("cul_des")
                        Grva_Cod = Dr_Campo(0).Item("grva_cod")
                        Grva_Des = Dr_Campo(0).Item("grva_des")
                        Grfi_Cod = Dr_Campo(0).Item("grfi_cod")
                        Grfi_Des = Dr_Campo(0).Item("grfi_des")
                        Cop_Cod = Dr_Campo(0).Item("cop_cod")
                        Cop_Des = Dr_Campo(0).Item("cop_des")
                        Resa = Dr_Campo(0).Item("resa")
                        DestinazioneUso = Dr_Campo(0).Item("DestinazioneUso")
                        DestinazioneUso_Des = Dr_Campo(0).Item("destinazioneuso_des")

                        MetodoProduzione_Cod = Dr_Campo(0).Item("MetodoProduzione_Cod")
                        MetodoProduzione_Des = Dr_Campo(0).Item("MetodoProduzione_Des")
                    End If

                    For j = 0 To Dr_Appezzamento.Length - 1

                        Validita_inizio_Entita = Dr_Appezzamento(j).Item("validita_inizio")
                        Anno = Validita_inizio_Entita.Year

                        Select Case Validita_inizio_Entita

                            Case CDate("01/01/" & Anno) To CDate("15/01/" & Anno)
                                ArrayApp(0) = Dr_Appezzamento(j).Item("Appezza")
                                ArrayApp(1) = Dr_Appezzamento(j).Item("Id_Reg")
                                ArrayApp(2) = Dr_Appezzamento(j).Item("Progetto_Cod")
                                ArrayApp(3) = Dr_Appezzamento(j).Item("Num_Piante")
                                ArrayApp(4) = Dr_Appezzamento(j).Item("Sup_App")

                            Case CDate("16/01/" & Anno) To CDate("31/01/" & Anno)
                                ArrayApp(5) = Dr_Appezzamento(j).Item("Appezza")
                                ArrayApp(6) = Dr_Appezzamento(j).Item("Id_Reg")
                                ArrayApp(7) = Dr_Appezzamento(j).Item("Progetto_Cod")
                                ArrayApp(8) = Dr_Appezzamento(j).Item("Num_Piante")
                                ArrayApp(9) = Dr_Appezzamento(j).Item("Sup_App")

                            Case CDate("01/02/" & Anno) To CDate("15/02/" & Anno)
                                ArrayApp(10) = Dr_Appezzamento(j).Item("Appezza")
                                ArrayApp(11) = Dr_Appezzamento(j).Item("Id_Reg")
                                ArrayApp(12) = Dr_Appezzamento(j).Item("Progetto_Cod")
                                ArrayApp(13) = Dr_Appezzamento(j).Item("Num_Piante")
                                ArrayApp(14) = Dr_Appezzamento(j).Item("Sup_App")

                            Case CDate("16/02/" & Anno) To CDate("28/02/" & Anno)
                                ArrayApp(15) = Dr_Appezzamento(j).Item("Appezza")
                                ArrayApp(16) = Dr_Appezzamento(j).Item("Id_Reg")
                                ArrayApp(17) = Dr_Appezzamento(j).Item("Progetto_Cod")
                                ArrayApp(18) = Dr_Appezzamento(j).Item("Num_Piante")
                                ArrayApp(19) = Dr_Appezzamento(j).Item("Sup_App")

                            Case CDate("01/03/" & Anno) To CDate("15/03/" & Anno)
                                ArrayApp(20) = Dr_Appezzamento(j).Item("Appezza")
                                ArrayApp(21) = Dr_Appezzamento(j).Item("Id_Reg")
                                ArrayApp(22) = Dr_Appezzamento(j).Item("Progetto_Cod")
                                ArrayApp(23) = Dr_Appezzamento(j).Item("Num_Piante")
                                ArrayApp(24) = Dr_Appezzamento(j).Item("Sup_App")

                            Case CDate("16/03/" & Anno) To CDate("31/03/" & Anno)
                                ArrayApp(25) = Dr_Appezzamento(j).Item("Appezza")
                                ArrayApp(26) = Dr_Appezzamento(j).Item("Id_Reg")
                                ArrayApp(27) = Dr_Appezzamento(j).Item("Progetto_Cod")
                                ArrayApp(28) = Dr_Appezzamento(j).Item("Num_Piante")
                                ArrayApp(29) = Dr_Appezzamento(j).Item("Sup_App")

                            Case CDate("01/04/" & Anno) To CDate("15/04/" & Anno)
                                ArrayApp(30) = Dr_Appezzamento(j).Item("Appezza")
                                ArrayApp(31) = Dr_Appezzamento(j).Item("Id_Reg")
                                ArrayApp(32) = Dr_Appezzamento(j).Item("Progetto_Cod")
                                ArrayApp(33) = Dr_Appezzamento(j).Item("Num_Piante")
                                ArrayApp(34) = Dr_Appezzamento(j).Item("Sup_App")

                            Case CDate("16/04/" & Anno) To CDate("30/04/" & Anno)
                                ArrayApp(35) = Dr_Appezzamento(j).Item("Appezza")
                                ArrayApp(36) = Dr_Appezzamento(j).Item("Id_Reg")
                                ArrayApp(37) = Dr_Appezzamento(j).Item("Progetto_Cod")
                                ArrayApp(38) = Dr_Appezzamento(j).Item("Num_Piante")
                                ArrayApp(39) = Dr_Appezzamento(j).Item("Sup_App")

                            Case CDate("01/05/" & Anno) To CDate("15/05/" & Anno)
                                ArrayApp(40) = Dr_Appezzamento(j).Item("Appezza")
                                ArrayApp(41) = Dr_Appezzamento(j).Item("Id_Reg")
                                ArrayApp(42) = Dr_Appezzamento(j).Item("Progetto_Cod")
                                ArrayApp(43) = Dr_Appezzamento(j).Item("Num_Piante")
                                ArrayApp(44) = Dr_Appezzamento(j).Item("Sup_App")

                            Case CDate("16/05/" & Anno) To CDate("31/05/" & Anno)
                                ArrayApp(45) = Dr_Appezzamento(j).Item("Appezza")
                                ArrayApp(46) = Dr_Appezzamento(j).Item("Id_Reg")
                                ArrayApp(47) = Dr_Appezzamento(j).Item("Progetto_Cod")
                                ArrayApp(48) = Dr_Appezzamento(j).Item("Num_Piante")
                                ArrayApp(49) = Dr_Appezzamento(j).Item("Sup_App")

                            Case CDate("01/06/" & Anno) To CDate("15/06/" & Anno)
                                ArrayApp(50) = Dr_Appezzamento(j).Item("Appezza")
                                ArrayApp(51) = Dr_Appezzamento(j).Item("Id_Reg")
                                ArrayApp(52) = Dr_Appezzamento(j).Item("Progetto_Cod")
                                ArrayApp(53) = Dr_Appezzamento(j).Item("Num_Piante")
                                ArrayApp(54) = Dr_Appezzamento(j).Item("Sup_App")

                            Case CDate("16/06/" & Anno) To CDate("30/06/" & Anno)
                                ArrayApp(55) = Dr_Appezzamento(j).Item("Appezza")
                                ArrayApp(56) = Dr_Appezzamento(j).Item("Id_Reg")
                                ArrayApp(57) = Dr_Appezzamento(j).Item("Progetto_Cod")
                                ArrayApp(58) = Dr_Appezzamento(j).Item("Num_Piante")
                                ArrayApp(59) = Dr_Appezzamento(j).Item("Sup_App")

                            Case CDate("01/07/" & Anno) To CDate("15/07/" & Anno)
                                ArrayApp(60) = Dr_Appezzamento(j).Item("Appezza")
                                ArrayApp(61) = Dr_Appezzamento(j).Item("Id_Reg")
                                ArrayApp(62) = Dr_Appezzamento(j).Item("Progetto_Cod")
                                ArrayApp(63) = Dr_Appezzamento(j).Item("Num_Piante")
                                ArrayApp(64) = Dr_Appezzamento(j).Item("Sup_App")

                            Case CDate("16/07/" & Anno) To CDate("31/07/" & Anno)
                                ArrayApp(65) = Dr_Appezzamento(j).Item("Appezza")
                                ArrayApp(66) = Dr_Appezzamento(j).Item("Id_Reg")
                                ArrayApp(67) = Dr_Appezzamento(j).Item("Progetto_Cod")
                                ArrayApp(68) = Dr_Appezzamento(j).Item("Num_Piante")
                                ArrayApp(69) = Dr_Appezzamento(j).Item("Sup_App")

                            Case CDate("01/08/" & Anno) To CDate("15/08/" & Anno)
                                ArrayApp(70) = Dr_Appezzamento(j).Item("Appezza")
                                ArrayApp(71) = Dr_Appezzamento(j).Item("Id_Reg")
                                ArrayApp(72) = Dr_Appezzamento(j).Item("Progetto_Cod")
                                ArrayApp(73) = Dr_Appezzamento(j).Item("Num_Piante")
                                ArrayApp(74) = Dr_Appezzamento(j).Item("Sup_App")

                            Case CDate("16/08/" & Anno) To CDate("31/08/" & Anno)
                                ArrayApp(75) = Dr_Appezzamento(j).Item("Appezza")
                                ArrayApp(76) = Dr_Appezzamento(j).Item("Id_Reg")
                                ArrayApp(77) = Dr_Appezzamento(j).Item("Progetto_Cod")
                                ArrayApp(78) = Dr_Appezzamento(j).Item("Num_Piante")
                                ArrayApp(79) = Dr_Appezzamento(j).Item("Sup_App")

                            Case CDate("01/09/" & Anno) To CDate("15/09/" & Anno)
                                ArrayApp(80) = Dr_Appezzamento(j).Item("Appezza")
                                ArrayApp(81) = Dr_Appezzamento(j).Item("Id_Reg")
                                ArrayApp(82) = Dr_Appezzamento(j).Item("Progetto_Cod")
                                ArrayApp(83) = Dr_Appezzamento(j).Item("Num_Piante")
                                ArrayApp(84) = Dr_Appezzamento(j).Item("Sup_App")

                            Case CDate("16/09/" & Anno) To CDate("30/09/" & Anno)
                                ArrayApp(85) = Dr_Appezzamento(j).Item("Appezza")
                                ArrayApp(86) = Dr_Appezzamento(j).Item("Id_Reg")
                                ArrayApp(87) = Dr_Appezzamento(j).Item("Progetto_Cod")
                                ArrayApp(88) = Dr_Appezzamento(j).Item("Num_Piante")
                                ArrayApp(89) = Dr_Appezzamento(j).Item("Sup_App")

                            Case CDate("01/10/" & Anno) To CDate("15/10/" & Anno)
                                ArrayApp(90) = Dr_Appezzamento(j).Item("Appezza")
                                ArrayApp(91) = Dr_Appezzamento(j).Item("Id_Reg")
                                ArrayApp(92) = Dr_Appezzamento(j).Item("Progetto_Cod")
                                ArrayApp(93) = Dr_Appezzamento(j).Item("Num_Piante")
                                ArrayApp(94) = Dr_Appezzamento(j).Item("Sup_App")

                            Case CDate("16/10/" & Anno) To CDate("31/10/" & Anno)
                                ArrayApp(95) = Dr_Appezzamento(j).Item("Appezza")
                                ArrayApp(96) = Dr_Appezzamento(j).Item("Id_Reg")
                                ArrayApp(97) = Dr_Appezzamento(j).Item("Progetto_Cod")
                                ArrayApp(98) = Dr_Appezzamento(j).Item("Num_Piante")
                                ArrayApp(99) = Dr_Appezzamento(j).Item("Sup_App")

                            Case CDate("01/11/" & Anno) To CDate("15/11/" & Anno)
                                ArrayApp(100) = Dr_Appezzamento(j).Item("Appezza")
                                ArrayApp(101) = Dr_Appezzamento(j).Item("Id_Reg")
                                ArrayApp(102) = Dr_Appezzamento(j).Item("Progetto_Cod")
                                ArrayApp(103) = Dr_Appezzamento(j).Item("Num_Piante")
                                ArrayApp(104) = Dr_Appezzamento(j).Item("Sup_App")

                            Case CDate("16/11/" & Anno) To CDate("30/11/" & Anno)
                                ArrayApp(105) = Dr_Appezzamento(j).Item("Appezza")
                                ArrayApp(106) = Dr_Appezzamento(j).Item("Id_Reg")
                                ArrayApp(107) = Dr_Appezzamento(j).Item("Progetto_Cod")
                                ArrayApp(108) = Dr_Appezzamento(j).Item("Num_Piante")
                                ArrayApp(109) = Dr_Appezzamento(j).Item("Sup_App")

                            Case CDate("01/12/" & Anno) To CDate("15/12/" & Anno)
                                ArrayApp(110) = Dr_Appezzamento(j).Item("Appezza")
                                ArrayApp(111) = Dr_Appezzamento(j).Item("Id_Reg")
                                ArrayApp(112) = Dr_Appezzamento(j).Item("Progetto_Cod")
                                ArrayApp(113) = Dr_Appezzamento(j).Item("Num_Piante")
                                ArrayApp(114) = Dr_Appezzamento(j).Item("Sup_App")

                            Case CDate("16/12/" & Anno) To CDate("31/12/" & Anno)
                                ArrayApp(115) = Dr_Appezzamento(j).Item("Appezza")
                                ArrayApp(116) = Dr_Appezzamento(j).Item("Id_Reg")
                                ArrayApp(117) = Dr_Appezzamento(j).Item("Progetto_Cod")
                                ArrayApp(118) = Dr_Appezzamento(j).Item("Num_Piante")
                                ArrayApp(119) = Dr_Appezzamento(j).Item("Sup_App")

                        End Select


                    Next


                End If

                DT_Campi_Insert(DT_Campi,
                               Operazione,
                               OperazioneDes,
                               Piva,
                               Sa_Cod,
                               Campo_Cod,
                               Campo_Des,
                               Sup_App,
                               Num_Piante,
                               ArrayApp,
                               Veg_Cod,
                               Veg_Des,
                               Cul_Cod,
                               Cul_Des,
                               Grva_Cod,
                               Grva_Des,
                               Grfi_Cod,
                               Grfi_Des,
                               Cop_Cod,
                               Cop_Des,
                               Resa,
                               DestinazioneUso,
                               DestinazioneUso_Des,
                               MetodoProduzione_Cod,
                               MetodoProduzione_Des)

            Next


        End If


    End Sub

#End Region

#Region "Particelle"

    Public Sub DT_Particelle_Crea(
                             ByRef DT As DataTable)

        DT.Columns.Add(New DataColumn("UNID_PART", GetType(String)))

        DT.Columns.Add(New DataColumn("Piva", GetType(String)))
        DT.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Sa_Nome", GetType(String)))

        DT.Columns.Add(New DataColumn("Prov", GetType(String)))
        DT.Columns.Add(New DataColumn("Com", GetType(String)))
        DT.Columns.Add(New DataColumn("Prov_Des", GetType(String)))
        DT.Columns.Add(New DataColumn("Com_Des", GetType(String)))

        DT.Columns.Add(New DataColumn("Sezione", GetType(String)))
        DT.Columns.Add(New DataColumn("Foglio", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Numero", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Subalterno", GetType(String)))

        DT.Columns.Add(New DataColumn("TitoloPossesso", GetType(Integer)))
        DT.Columns.Add(New DataColumn("TitoloPossesso_Des", GetType(String)))

        DT.Columns.Add(New DataColumn("Validita_Inizio", GetType(String)))
        DT.Columns.Add(New DataColumn("Validita_Fine", GetType(String)))

        DT.Columns.Add(New DataColumn("SupTotale", GetType(Decimal)))
        DT.Columns.Add(New DataColumn("SupCondotta", GetType(Decimal)))
        DT.Columns.Add(New DataColumn("SupResidua", GetType(Decimal)))
        DT.Columns.Add(New DataColumn("SupSpandibile", GetType(Decimal)))

        DT.Columns.Add(New DataColumn("Cod_Macrouso", GetType(String)))
        DT.Columns.Add(New DataColumn("Macrouso_Des", GetType(String)))
        DT.Columns.Add(New DataColumn("SupMacrouso", GetType(Decimal)))
        DT.Columns.Add(New DataColumn("SupMacrousoDisp", GetType(Decimal)))

        DT.Columns.Add(New DataColumn("Cod_Utilizzo", GetType(String)))
        DT.Columns.Add(New DataColumn("Veg_Des_Agea", GetType(String)))
        DT.Columns.Add(New DataColumn("SupUtilizzo", GetType(Decimal)))
        DT.Columns.Add(New DataColumn("SupUtilizzoDisp", GetType(Decimal)))


    End Sub

    Public Sub Key_Particella_SET(
                                ByRef UNID_PART As String,
                                ByVal Prov As String,
                                ByVal Com As String,
                                ByVal Sezione As String,
                                ByVal Foglio As Integer,
                                ByVal Numero As Integer,
                                ByVal Subalterno As String)

        UNID_PART = Prov & "_" &
                    Com & "_" &
                    Sezione & "_" &
                    Foglio.ToString & "_" &
                    Numero.ToString & "_" &
                    Subalterno

    End Sub

    Public Sub Key_Particella_GET(
                                   ByVal UNID_PART As String,
                                   ByRef Prov As String,
                                   ByRef Com As String,
                                   ByRef Sezione As String,
                                   ByRef Foglio As Integer,
                                   ByRef Numero As Integer,
                                   ByRef Subalterno As String)

        Dim Testo As String()
        Testo = Split(UNID_PART, "_")
        Prov = Testo(0)
        Com = Testo(1)
        Sezione = Testo(2)
        Foglio = CInt(Testo(3))
        Numero = CInt(Testo(4))
        Subalterno = Testo(5)

    End Sub

    Public Sub DT_Particelle_Insert(
                             ByRef DT_Particelle As DataTable,
                             ByVal Piva As String,
                             ByVal Sa_Cod As Integer,
                             ByVal Sa_Nome As String,
                             ByVal Prov As String,
                             ByVal Com As String,
                             ByVal Prov_Des As String,
                             ByVal Com_des As String,
                             ByVal Sezione As String,
                             ByVal Foglio As Integer,
                             ByVal Numero As Integer,
                             ByVal Subalterno As String,
                             ByVal SupTotale As Decimal,
                             ByVal SupResidua As Decimal,
                             ByVal SupSpandibile As Decimal,
                             ByVal Macrouso_Cod As String,
                             ByVal Macrouso_Des As String,
                             ByVal SupMacrouso As Decimal,
                             ByVal SupMacrousoDisp As Decimal,
                             ByVal Utilizzo_Cod As String,
                             ByVal Utilizzo_Des As String,
                             ByVal SupUtilizzo As Decimal,
                             ByVal SupUtilizzoDisp As Decimal,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal TitoloPossesso As Integer,
                             ByVal TitoloPossesso_Des As String)

        Dim DR As DataRow
        Dim UNID_APP As String = ""
        Dim UNID_PART As String = ""

        Key_Particella_SET(UNID_PART, Prov, Com, Sezione, Foglio, Numero, Subalterno)

        DR = DT_Particelle.NewRow

        DR.Item("UNID_PART") = UNID_PART

        DR.Item("Piva") = Piva
        DR.Item("Sa_Cod") = Sa_Cod
        DR.Item("Sa_Nome") = Sa_Nome

        DR.Item("Prov") = Prov
        DR.Item("Com") = Com
        DR.Item("Prov_Des") = Prov_Des
        DR.Item("Com_Des") = Com_des

        DR.Item("Sezione") = Sezione
        DR.Item("Foglio") = Foglio
        DR.Item("Numero") = Numero
        DR.Item("Subalterno") = Subalterno

        DR.Item("Validita_Inizio") = IIf(Validita_Inizio = #1/1/1900#, "", Validita_Inizio.ToShortDateString)
        DR.Item("Validita_Fine") = IIf(Validita_Fine = #12/31/2100#, "", Validita_Fine.ToShortDateString)
        DR.Item("TitoloPossesso") = TitoloPossesso
        DR.Item("TitoloPossesso_Des") = TitoloPossesso_Des

        DR.Item("SupTotale") = SupTotale
        DR.Item("SupResidua") = SupResidua
        DR.Item("SupSpandibile") = SupSpandibile

        DR.Item("Cod_Macrouso") = Macrouso_Cod
        DR.Item("Macrouso_Des") = Macrouso_Des
        DR.Item("SupMacrouso") = SupMacrouso
        DR.Item("SupMacrousoDisp") = SupMacrousoDisp

        DR.Item("Cod_Utilizzo") = Utilizzo_Cod
        DR.Item("Veg_Des_Agea") = Utilizzo_Des
        DR.Item("SupUtilizzo") = SupUtilizzo
        DR.Item("SupUtilizzoDisp") = SupUtilizzoDisp

        DT_Particelle.Rows.Add(DR)

    End Sub

#End Region

#Region "Intersezioni"

    Public Sub DT_Intersezioni_Crea(
                            ByRef DT As DataTable)

        DT.Columns.Add(New DataColumn("UNID_APP", GetType(String)))
        DT.Columns.Add(New DataColumn("UNID_CAMPO", GetType(String)))
        DT.Columns.Add(New DataColumn("UNID_PART", GetType(String)))

        DT.Columns.Add(New DataColumn("Programmazione_Entita_Cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Piva", GetType(String)))
        DT.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Campo_Cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Appezza", GetType(Integer)))

        DT.Columns.Add(New DataColumn("Prov", GetType(String)))
        DT.Columns.Add(New DataColumn("Com", GetType(String)))
        DT.Columns.Add(New DataColumn("Prov_Des", GetType(String)))
        DT.Columns.Add(New DataColumn("Com_Des", GetType(String)))

        DT.Columns.Add(New DataColumn("Sezione", GetType(String)))
        DT.Columns.Add(New DataColumn("Foglio", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Numero", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Subalterno", GetType(String)))

        DT.Columns.Add(New DataColumn("SupTotale", GetType(Decimal)))
        DT.Columns.Add(New DataColumn("SupIntersezione", GetType(Decimal)))

        ' Drudi 07/11/2017
        DT.Columns.Add(New DataColumn("SupCatastale", GetType(Decimal)))
        DT.Columns.Add(New DataColumn("Validita_Inizio", GetType(Date)))
        DT.Columns.Add(New DataColumn("Validita_Fine", GetType(Date)))
        DT.Columns.Add(New DataColumn("datepossesso", GetType(String)))
        DT.Columns.Add(New DataColumn("possesso", GetType(String)))
        DT.Columns.Add(New DataColumn("TitoloPossesso", GetType(Integer)))

        DT.Columns.Add(New DataColumn("Proprietario", GetType(String)))

    End Sub

    Public Sub DT_Intersezioni_Inizializza(ByRef DT_Intersezioni As DataTable,
                                            ByVal TipoPianificazione As Integer,
                                            ByVal ElencoAppezzamenti As String,
                                            ByVal Data_Inizio As Date,
                                            ByVal Data_Fine As Date,
                                            ByRef ErrMSG As String,
                                            ByRef objParametri As AgronicaCoreParametri)

        Dim KeyAppezzamenti As String()
        Dim Piva As String = ""
        Dim Sa_Cod As Integer
        Dim Campo_Cod As Integer
        Dim Appezza As Integer
        Dim IdReg As Integer
        Dim Progetto_Cod As Integer
        Dim i As Integer
        Dim DT As New DataTable
        Dim k As Integer
        Dim Filtro As String = ""


        Select Case TipoPianificazione

            Case enum_TipoPianificazione.Pianificazione_Annuale,
                            enum_TipoPianificazione.Pianificazione_DaNotificaBio

                KeyAppezzamenti = Split(ElencoAppezzamenti, "|")

                'Se era stato selezionato qualche appezzamento da conservare ...
                If KeyAppezzamenti.Length > 0 Then

                    For i = 0 To KeyAppezzamenti.Length - 1
                        'Scompongo la chiave 
                        Key_Appezzamento_GET(KeyAppezzamenti(i), Piva, Sa_Cod, Campo_Cod, Appezza, IdReg, Progetto_Cod)
                        Filtro &= " (AppezzamentiXParticelle.PIVA = '" & Agro_SQL_SaveText(Piva, False) & "' AND AppezzamentiXParticelle.SA_COD =" & Agro_SQL_SaveNum(Sa_Cod.ToString, False) & " AND AppezzamentiXParticelle.APPEZZA =" & Agro_SQL_SaveNum(Appezza.ToString, False) & ") OR "
                    Next

                    If Filtro <> "" Then
                        Filtro = "(" & Left(Filtro, Filtro.Length - 4) & ")"
                    End If

                    DT = Anagrafica_AppezzamentixParticelle_Leggi(Piva, 0, 0, Data_Inizio, Data_Fine, Filtro, ErrMSG, objParametri)

                    For k = 0 To DT.Rows.Count - 1

                        DT_Intersezioni_Insert(
                                            DT_Intersezioni,
                                            TipoPianificazione,
                                            DT.Rows(k).Item("Piva"),
                                            DT.Rows(k).Item("Sa_Cod"),
                                            0,
                                            DT.Rows(k).Item("Appezza"),
                                            DT.Rows(k).Item("Prov"),
                                            DT.Rows(k).Item("Com"),
                                            DT.Rows(k).Item("Prov_Des"),
                                            DT.Rows(k).Item("Com_Des"),
                                            DT.Rows(k).Item("Sezione"),
                                            DT.Rows(k).Item("Foglio"),
                                            DT.Rows(k).Item("Numero"),
                                            DT.Rows(k).Item("Subalterno"),
                                            DT.Rows(k).Item("SupTotale"),
                                            DT.Rows(k).Item("SupIntersezione"))

                    Next

                End If

            Case enum_TipoPianificazione.Pianificazione_Quindicinale

        End Select


    End Sub


    Public Sub DT_Intersezioni_Delete(
                            ByRef DT_Intersezioni As DataTable,
                            ByVal TipoPianificazione As Integer,
                            ByVal Piva As String,
                            ByVal Sa_Cod As Integer,
                            ByVal Campo_Cod As Integer,
                            ByVal Appezza As Integer)

        Dim i As Integer
        Dim ArrayDaEliminare As Integer()
        Dim n As Integer = 0

        For i = 0 To DT_Intersezioni.Rows.Count - 1

            Select Case TipoPianificazione

                Case enum_TipoPianificazione.Pianificazione_Annuale,
                            enum_TipoPianificazione.Pianificazione_DaNotificaBio
                    If (DT_Intersezioni.Rows(i).Item("Piva") = Piva) And
                       (DT_Intersezioni.Rows(i).Item("Sa_Cod") = Sa_Cod) And
                       (DT_Intersezioni.Rows(i).Item("Appezza") = Appezza) Then

                        ReDim Preserve ArrayDaEliminare(n)
                        ArrayDaEliminare(n) = i
                        n += 1

                    End If

                Case enum_TipoPianificazione.Pianificazione_Quindicinale
                    If (DT_Intersezioni.Rows(i).Item("Piva") = Piva) And
                       (DT_Intersezioni.Rows(i).Item("Sa_Cod") = Sa_Cod) And
                       (DT_Intersezioni.Rows(i).Item("Campo_Cod") = Campo_Cod) Then

                        ReDim Preserve ArrayDaEliminare(n)
                        ArrayDaEliminare(n) = i
                        n += 1

                    End If

            End Select

        Next

        If ArrayDaEliminare IsNot Nothing Then
            For i = ArrayDaEliminare.Length - 1 To 0 Step -1
                DT_Intersezioni.Rows(ArrayDaEliminare(i)).Delete()
            Next
        End If

        DT_Intersezioni.AcceptChanges()

    End Sub

    Public Sub DT_Intersezioni_Insert(
                            ByRef DT_Intersezioni As DataTable,
                            ByVal TipoPianificazione As Integer,
                            ByVal Piva As String,
                            ByVal Sa_Cod As Integer,
                            ByVal Campo_Cod As Integer,
                            ByVal Appezza As Integer,
                            ByVal Prov As String,
                            ByVal Com As String,
                            ByVal Prov_Des As String,
                            ByVal Com_des As String,
                            ByVal Sezione As String,
                            ByVal Foglio As Integer,
                            ByVal Numero As Integer,
                            ByVal Subalterno As String,
                            ByVal SupTotale As Decimal,
                            ByVal SupIntersezione As Decimal,
                            Optional ByVal SupCatastale As Decimal = 0,
                            Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                            Optional ByVal Validita_Fine As Date = AGRODATAFINE,
                            Optional ByVal datepossesso As String = "",
                            Optional ByVal possesso As String = "",
                            Optional ByVal TitoloPossesso As Integer = 0,
                            Optional ByVal Proprietario As String = ""
                            )

        Dim DR As DataRow
        Dim UNID_APP As String = ""
        Dim UNID_PART As String = ""
        Dim UNID_CAMPO As String = ""

        Select Case TipoPianificazione

            Case enum_TipoPianificazione.Pianificazione_Annuale,
                            enum_TipoPianificazione.Pianificazione_DaNotificaBio

                Key_Appezzamento_SET(UNID_APP, Piva, Sa_Cod, Campo_Cod, Appezza, 0, 0)

                ' prima di inserire il record controllo che non ci sia già
                Dim DrRigheDoppie As DataRow()
                DrRigheDoppie = DT_Intersezioni.Select("Sa_Cod = " & Sa_Cod & " AND Appezza = " & Appezza &
                                                       " AND Prov = '" & Prov & "' AND Com = '" & Com & "' AND Sezione = '" & Sezione & "'" &
                                                       " AND Foglio = " & Foglio & " AND Numero = " & Numero & " AND Subalterno = '" & Subalterno & "'")

                If DrRigheDoppie.Length > 0 Then
                    Exit Sub
                End If

            Case enum_TipoPianificazione.Pianificazione_Quindicinale

                Key_Campo_SET(UNID_CAMPO, Piva, Sa_Cod, Campo_Cod)

                ' prima di inserire il record controllo che non ci sia già
                Dim DrRigheDoppie As DataRow()
                DrRigheDoppie = DT_Intersezioni.Select("Sa_Cod = " & Sa_Cod & " AND campo_cod = " & Campo_Cod & " AND Appezza = " & Appezza &
                                                       " AND Prov = '" & Prov & "' AND Com = '" & Com & "' AND Sezione = '" & Sezione & "'" &
                                                       " AND Foglio = " & Foglio & " AND Numero = " & Numero & " AND Subalterno = '" & Subalterno & "'")

                If DrRigheDoppie.Length > 0 Then
                    Exit Sub
                End If

        End Select

        Key_Particella_SET(UNID_PART, Prov, Com, Sezione, Foglio, Numero, Subalterno)

        DR = DT_Intersezioni.NewRow

        DR.Item("UNID_APP") = UNID_APP
        DR.Item("UNID_PART") = UNID_PART
        DR.Item("UNID_CAMPO") = UNID_CAMPO

        DR.Item("Piva") = Piva
        DR.Item("Sa_Cod") = Sa_Cod
        DR.Item("Campo_Cod") = Campo_Cod
        DR.Item("Appezza") = Appezza

        DR.Item("Prov") = Prov
        DR.Item("Com") = Com
        DR.Item("Prov_Des") = Prov_Des
        DR.Item("Com_Des") = Com_des

        DR.Item("Sezione") = Sezione
        DR.Item("Foglio") = Foglio
        DR.Item("Numero") = Numero
        DR.Item("Subalterno") = Subalterno

        DR.Item("SupTotale") = SupTotale
        DR.Item("SupIntersezione") = SupIntersezione

        DR.Item("SupCatastale") = SupCatastale
        DR.Item("Validita_Inizio") = Validita_Inizio
        DR.Item("Validita_Fine") = Validita_Fine
        DR.Item("datepossesso") = datepossesso
        DR.Item("possesso") = possesso
        DR.Item("TitoloPossesso") = TitoloPossesso

        DR.Item("Proprietario") = Proprietario

        DT_Intersezioni.Rows.Add(DR)

    End Sub

    Public Sub DT_Intersezioni_Update(
                                ByRef DT_Intersezioni As DataTable,
                                ByVal TipoPianificazione As Integer,
                                ByVal Piva As String,
                                ByVal Sa_Cod As Integer,
                                ByVal Campo_Cod As Integer,
                                ByVal Appezza As Integer,
                                ByVal Id_Reg As Integer,
                                ByVal Prov As String,
                                ByVal Com As String,
                                ByVal Prov_Des As String,
                                ByVal Com_des As String,
                                ByVal Sezione As String,
                                ByVal Foglio As Integer,
                                ByVal Numero As Integer,
                                ByVal Subalterno As String,
                                ByVal SupTotale As Decimal,
                                ByVal SupIntersezione As Decimal)

        Dim DR As DataRow
        Dim UNID_APP As String = ""
        Dim UNID_PART As String = ""
        Dim UNID_CAMPO As String = ""

        Key_Particella_SET(UNID_PART, Prov, Com, Sezione, Foglio, Numero, Subalterno)

        Select Case TipoPianificazione

            Case enum_TipoPianificazione.Pianificazione_Annuale,
                            enum_TipoPianificazione.Pianificazione_DaNotificaBio
                Key_Appezzamento_SET(UNID_APP, Piva, Sa_Cod, Campo_Cod, Appezza, Id_Reg, 0)

            Case enum_TipoPianificazione.Pianificazione_Quindicinale
                Key_Campo_SET(UNID_CAMPO, Piva, Sa_Cod, Campo_Cod)

        End Select


        DR = DT_Intersezioni.NewRow

        DR.Item("UNID_APP") = UNID_APP
        DR.Item("UNID_PART") = UNID_PART
        DR.Item("UNID_CAMPO") = UNID_CAMPO

        DR.Item("Piva") = Piva
        DR.Item("Sa_Cod") = Sa_Cod
        DR.Item("Campo_Cod") = Campo_Cod
        DR.Item("Appezza") = Appezza

        DR.Item("Prov") = Prov
        DR.Item("Com") = Com
        DR.Item("Prov_Des") = Prov_Des
        DR.Item("Com_Des") = Com_des

        DR.Item("Sezione") = Sezione
        DR.Item("Foglio") = Foglio
        DR.Item("Numero") = Numero
        DR.Item("Subalterno") = Subalterno

        DR.Item("SupTotale") = SupTotale
        DR.Item("SupIntersezione") = SupIntersezione

        DT_Intersezioni.Rows.Add(DR)

    End Sub

#End Region

#Region "Appezzamenti_2Colture"

    Public Sub DT_Appezzamenti_2Colture_Crea(ByRef DT As DataTable)

        Dim i As Integer
        'DT.Columns.Add(New DataColumn("UNID_APP", GetType(String)))

        DT.Columns.Add(New DataColumn("Programmazione_Entita_Cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Sa_Nome", GetType(String)))
        DT.Columns.Add(New DataColumn("Campo_Cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Campo_Des", GetType(String)))

        For i = 1 To 2
            DT.Columns.Add(New DataColumn("Entita_" & i.ToString, GetType(Integer)))
            DT.Columns.Add(New DataColumn("Appezza_" & i.ToString, GetType(Integer)))
            DT.Columns.Add(New DataColumn("Id_Reg_" & i.ToString, GetType(Integer)))
            DT.Columns.Add(New DataColumn("Sup_App_" & i.ToString, GetType(String)))
            DT.Columns.Add(New DataColumn("Codice_" & i.ToString, GetType(String)))
            DT.Columns.Add(New DataColumn("Veg_Cod_" & i.ToString, GetType(String)))
            DT.Columns.Add(New DataColumn("Veg_Des_" & i.ToString, GetType(String)))
            DT.Columns.Add(New DataColumn("Cul_Cod_" & i.ToString, GetType(String)))
            DT.Columns.Add(New DataColumn("Cul_Des_" & i.ToString, GetType(String)))
            DT.Columns.Add(New DataColumn("Grfi_Cod_" & i.ToString, GetType(Integer)))
            DT.Columns.Add(New DataColumn("Grfi_Des_" & i.ToString, GetType(String)))
            DT.Columns.Add(New DataColumn("DestinazioneUso_Cod_" & i.ToString, GetType(Integer)))
            DT.Columns.Add(New DataColumn("DestinazioneUso_Des_" & i.ToString, GetType(String)))
            DT.Columns.Add(New DataColumn("Validita_Inizio_" & i.ToString, GetType(String)))
            DT.Columns.Add(New DataColumn("Validita_Fine_" & i.ToString, GetType(String)))
        Next

        DT.Columns.Add(New DataColumn("Note", GetType(String)))
        DT.Columns.Add(New DataColumn("Mod", GetType(String)))


    End Sub

    Public Sub DT_Appezzamenti_2Colture_Insert(
                                ByRef DT_Campi As DataTable,
                                ByVal Programmazione_Entita_Cod As Integer,
                                ByVal Sa_Cod As Integer,
                                ByVal Sa_Nome As String,
                                ByVal Campo_Cod As Integer,
                                ByVal Campo_Des As String,
                                ByVal Entita() As Integer,
                                ByVal Appezza() As Integer,
                                ByVal Id_Reg() As Integer,
                                ByVal Sup_App() As Decimal,
                                ByVal Veg_Cod() As String,
                                ByVal Veg_Des() As String,
                                ByVal Cul_Cod() As String,
                                ByVal Cul_Des() As String,
                                ByVal Grfi_Cod() As Integer,
                                ByVal Grfi_Des() As String,
                                ByVal DestinazioneUso_Cod() As Integer,
                                ByVal DestinazioneUso_Des() As String,
                                ByVal Validita_Inizio() As String,
                                ByVal Validita_Fine() As String,
                                ByVal Note As String,
                                ByVal Modifica As String)


        Dim DR As DataRow
        Dim UNID_APP As String = ""
        Dim i As Integer

        Dim IconaSI As String = "<img src='../App_Immagini/Icone16/cS.ico'>"
        Dim IconaNO As String = "<img src='../App_Immagini/Icone16/cN.ico'>"

        'Key_App3Colture_SET(UNID_APP, Programmazione_Entita_Cod, Prov, Com, Sezione, Foglio, Numero, Subalterno)

        DR = DT_Campi.NewRow

        'DR.Item("UNID_APP") = UNID_APP

        DR.Item("Programmazione_Entita_Cod") = Programmazione_Entita_Cod
        DR.Item("Sa_Cod") = Sa_Cod
        DR.Item("Sa_Nome") = Sa_Nome
        DR.Item("Campo_Cod") = Campo_Cod
        DR.Item("Campo_Des") = Campo_Des

        For i = 1 To Math.Min(Sup_App.Length, 2)

            DR.Item("Entita_" & i.ToString) = Entita(i - 1)
            DR.Item("Appezza_" & i.ToString) = Appezza(i - 1)
            DR.Item("Id_Reg_" & i.ToString) = Id_Reg(i - 1)
            DR.Item("Sup_App_" & i.ToString) = Sup_App(i - 1)
            DR.Item("Codice_" & i.ToString) = Cul_Cod(i - 1)    'Veg_Cod(i - 1) & Cul_Cod(i - 1)
            DR.Item("Veg_Cod_" & i.ToString) = Veg_Cod(i - 1)
            DR.Item("Veg_Des_" & i.ToString) = Veg_Des(i - 1)
            DR.Item("Cul_Cod_" & i.ToString) = Cul_Cod(i - 1)
            DR.Item("Cul_Des_" & i.ToString) = Cul_Des(i - 1)
            DR.Item("Grfi_Cod_" & i.ToString) = Grfi_Cod(i - 1)
            DR.Item("Grfi_Des_" & i.ToString) = Grfi_Des(i - 1)
            DR.Item("DestinazioneUso_Cod_" & i.ToString) = DestinazioneUso_Cod(i - 1)
            DR.Item("DestinazioneUso_Des_" & i.ToString) = DestinazioneUso_Des(i - 1)
            DR.Item("Validita_Inizio_" & i.ToString) = Validita_Inizio(i - 1)
            DR.Item("Validita_Fine_" & i.ToString) = Validita_Fine(i - 1)

        Next

        DR.Item("Note") = Note

        DR.Item("Mod") = IIf(Modifica = "0", "", "MOD")

        DT_Campi.Rows.Add(DR)

    End Sub

#End Region

#Region "Appezzamenti_3Colture"

    Public Sub Key_App3Colture_SET(
                            ByRef UNID_APPEZZA As String,
                            ByVal Programmazione_Entita_Cod As Integer,
                            ByVal Prov As String,
                            ByVal Com As String,
                            ByVal Sezione As String,
                            ByVal Foglio As Integer,
                            ByVal Numero As Integer,
                            ByVal Subalterno As String)

        UNID_APPEZZA = Programmazione_Entita_Cod.ToString & "_" &
                       Prov & "_" &
                       Com & "_" &
                       Sezione & "_" &
                       Foglio.ToString & "_" &
                       Numero.ToString & "_" &
                       Subalterno

    End Sub

    Public Sub Key_App3Colture_GET(
                                ByVal UNID_APPEZZA As String,
                                ByRef Programmazione_Entita_Cod As Integer,
                                ByRef Prov As String,
                                ByRef Com As String,
                                ByRef Sezione As String,
                                ByRef Foglio As Integer,
                                ByRef Numero As Integer,
                                ByRef Subalterno As String)

        Dim Testo As String()
        Testo = Split(UNID_APPEZZA, "_")
        Programmazione_Entita_Cod = Testo(0)
        Prov = CInt(Testo(1))
        Com = CInt(Testo(2))
        Sezione = CInt(Testo(3))
        Foglio = CInt(Testo(4))
        Numero = CInt(Testo(5))
        Subalterno = CInt(Testo(6))


    End Sub

    Public Sub DT_Appezzamenti_3Colture_Crea(ByRef DT As DataTable)

        Dim i As Integer

        DT.Columns.Add(New DataColumn("UNID_APP", GetType(String)))

        DT.Columns.Add(New DataColumn("Programmazione_Entita_Cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Sa_Nome", GetType(String)))
        DT.Columns.Add(New DataColumn("Campo_Cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Campo_Des", GetType(String)))
        'DT.Columns.Add(New DataColumn("SupParticella", GetType(String)))
        'DT.Columns.Add(New DataColumn("Appezza", GetType(Integer)))
        'DT.Columns.Add(New DataColumn("Id_Reg", GetType(Integer)))

        DT.Columns.Add(New DataColumn("SupCatastale", GetType(String)))
        DT.Columns.Add(New DataColumn("SupCondotta", GetType(String)))
        DT.Columns.Add(New DataColumn("SupSeminabile", GetType(String)))
        DT.Columns.Add(New DataColumn("SupUnar", GetType(String)))

        For i = 1 To 3
            DT.Columns.Add(New DataColumn("Entita_" & i.ToString, GetType(Integer)))
            DT.Columns.Add(New DataColumn("Appezza_" & i.ToString, GetType(Integer)))
            DT.Columns.Add(New DataColumn("Id_Reg_" & i.ToString, GetType(Integer)))
            DT.Columns.Add(New DataColumn("Sup_App_" & i.ToString, GetType(String)))
            DT.Columns.Add(New DataColumn("Codice_" & i.ToString, GetType(String)))
            DT.Columns.Add(New DataColumn("Veg_Cod_" & i.ToString, GetType(String)))
            DT.Columns.Add(New DataColumn("Veg_Des_" & i.ToString, GetType(String)))
            DT.Columns.Add(New DataColumn("Cul_Cod_" & i.ToString, GetType(String)))
            DT.Columns.Add(New DataColumn("Cul_Des_" & i.ToString, GetType(String)))
            DT.Columns.Add(New DataColumn("Grfi_Cod_" & i.ToString, GetType(Integer)))
            DT.Columns.Add(New DataColumn("Grfi_Des_" & i.ToString, GetType(String)))
            DT.Columns.Add(New DataColumn("DestinazioneUso_Cod_" & i.ToString, GetType(Integer)))
            DT.Columns.Add(New DataColumn("DestinazioneUso_Des_" & i.ToString, GetType(String)))
            DT.Columns.Add(New DataColumn("Validita_Inizio_" & i.ToString, GetType(String)))
        Next

        DT.Columns.Add(New DataColumn("Prov", GetType(String)))
        DT.Columns.Add(New DataColumn("Com", GetType(String)))
        DT.Columns.Add(New DataColumn("Prov_Des", GetType(String)))
        DT.Columns.Add(New DataColumn("Com_Des", GetType(String)))

        DT.Columns.Add(New DataColumn("Sezione", GetType(String)))
        DT.Columns.Add(New DataColumn("Foglio", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Numero", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Subalterno", GetType(String)))

        DT.Columns.Add(New DataColumn("Note", GetType(String)))
        DT.Columns.Add(New DataColumn("Modifica", GetType(String)))
        DT.Columns.Add(New DataColumn("Particella_Fine", GetType(String)))


    End Sub

    Public Sub DT_Appezzamenti_3Colture_Insert(
                                ByRef DT_Campi As DataTable,
                                ByVal Programmazione_Entita_Cod As Integer,
                                ByVal Sa_Cod As Integer,
                                ByVal Sa_Nome As String,
                                ByVal Campo_Cod As Integer,
                                ByVal Campo_Des As String,
                                ByVal SupCatastale As Decimal,
                                ByVal SupCondotta As Decimal,
                                ByVal SupSeminabile As Decimal,
                                ByVal SupUnar As Decimal,
                                ByVal Entita() As Integer,
                                ByVal Appezza() As Integer,
                                ByVal Id_Reg() As Integer,
                                ByVal Sup_App() As Decimal,
                                ByVal Veg_Cod() As String,
                                ByVal Veg_Des() As String,
                                ByVal Cul_Cod() As String,
                                ByVal Cul_Des() As String,
                                ByVal Grfi_Cod() As Integer,
                                ByVal Grfi_Des() As String,
                                ByVal DestinazioneUso_Cod() As Integer,
                                ByVal DestinazioneUso_Des() As String,
                                ByVal Validita_Inizio() As String,
                                ByVal Prov As String,
                                ByVal Com As String,
                                ByVal Prov_Des As String,
                                ByVal Com_des As String,
                                ByVal Sezione As String,
                                ByVal Foglio As Integer,
                                ByVal Numero As Integer,
                                ByVal Subalterno As String,
                                ByVal Note As String,
                                ByVal Modifica As String,
                                ByVal Particella_Fine As String)


        Dim DR As DataRow
        Dim UNID_APP As String = ""
        Dim i As Integer

        Dim IconaSI As String = "X"
        Dim IconaNO As String = ""

        Key_App3Colture_SET(UNID_APP, Programmazione_Entita_Cod, Prov, Com, Sezione, Foglio, Numero, Subalterno)

        ''If DT_Campi Is Nothing Then
        ''    DT_Campi = New DataTable
        ''End If

        DR = DT_Campi.NewRow

        DR.Item("UNID_APP") = UNID_APP

        DR.Item("Programmazione_Entita_Cod") = Programmazione_Entita_Cod
        DR.Item("Sa_Cod") = Sa_Cod
        DR.Item("Sa_Nome") = Sa_Nome
        DR.Item("Campo_Cod") = Campo_Cod
        DR.Item("Campo_Des") = Campo_Des
        ' DR.Item("SupParticella") = Format(SupParticella, "0.0000")

        DR.Item("SupCatastale") = Format(SupCatastale, "0.0000")
        DR.Item("SupCondotta") = Format(SupCondotta, "0.0000")
        DR.Item("SupSeminabile") = Format(SupSeminabile, "0.0000")

        DR.Item("SupUnar") = IIf(SupUnar > 0, IconaSI, IconaNO)



        ' DR.Item("Appezza") = Appezza
        ' DR.Item("Id_Reg") = Appezza

        For i = 1 To Math.Min(Sup_App.Length, 3)

            DR.Item("Entita_" & i.ToString) = Entita(i - 1)
            DR.Item("Appezza_" & i.ToString) = Appezza(i - 1)
            DR.Item("Id_Reg_" & i.ToString) = Id_Reg(i - 1)
            DR.Item("Sup_App_" & i.ToString) = Sup_App(i - 1)
            DR.Item("Codice_" & i.ToString) = Cul_Cod(i - 1)    'Veg_Cod(i - 1) & Cul_Cod(i - 1)
            DR.Item("Veg_Cod_" & i.ToString) = Veg_Cod(i - 1)
            DR.Item("Veg_Des_" & i.ToString) = Veg_Des(i - 1)
            DR.Item("Cul_Cod_" & i.ToString) = Cul_Cod(i - 1)
            DR.Item("Cul_Des_" & i.ToString) = Cul_Des(i - 1)
            DR.Item("Grfi_Cod_" & i.ToString) = Grfi_Cod(i - 1)
            DR.Item("Grfi_Des_" & i.ToString) = Grfi_Des(i - 1)
            DR.Item("DestinazioneUso_Cod_" & i.ToString) = DestinazioneUso_Cod(i - 1)
            DR.Item("DestinazioneUso_Des_" & i.ToString) = DestinazioneUso_Des(i - 1)
            DR.Item("Validita_Inizio_" & i.ToString) = Validita_Inizio(i - 1)

        Next

        DR.Item("Prov") = Prov
        DR.Item("Com") = Com
        DR.Item("Prov_Des") = Prov_Des
        DR.Item("Com_Des") = Com_des

        DR.Item("Sezione") = Sezione
        DR.Item("Foglio") = Foglio
        DR.Item("Numero") = Numero
        DR.Item("Subalterno") = Subalterno
        DR.Item("Note") = Note
        DR.Item("Modifica") = Modifica
        DR.Item("Particella_Fine") = Particella_Fine

        DT_Campi.Rows.Add(DR)

    End Sub

    Public Sub DT_Appezzamenti_3Colture_Delete(
                                ByRef DT_Appezzamenti As DataTable,
                                ByVal Prov As String,
                                ByVal Com As String,
                                ByVal Sezione As Integer,
                                ByVal Foglio As Integer,
                                ByVal Numero As Integer,
                                ByVal Subalterno As String,
                                ByVal Sa_Cod As Integer,
                                ByVal Campo_Cod As Integer,
                                ByVal Programmazione_Entita_Cod As Integer)


        Dim i As Integer

        For i = 0 To DT_Appezzamenti.Rows.Count - 1

            If (DT_Appezzamenti.Rows(i).Item("Prov") = Prov) And
               (DT_Appezzamenti.Rows(i).Item("Com") = Com) And
               (DT_Appezzamenti.Rows(i).Item("Sezione") = Sezione) And
               (DT_Appezzamenti.Rows(i).Item("Com") = Com) And
               (DT_Appezzamenti.Rows(i).Item("Foglio") = Foglio) And
               (DT_Appezzamenti.Rows(i).Item("Numero") = Numero) And
               (DT_Appezzamenti.Rows(i).Item("Subalterno") = Subalterno) And
               (DT_Appezzamenti.Rows(i).Item("Numero") = Numero) And
               (DT_Appezzamenti.Rows(i).Item("Sa_Cod") = Sa_Cod) And
               (DT_Appezzamenti.Rows(i).Item("Campo_Cod") = Campo_Cod) And
               (DT_Appezzamenti.Rows(i).Item("Programmazione_Entita_Cod") = Programmazione_Entita_Cod) Then

                DT_Appezzamenti.Rows(i).Delete()
                DT_Appezzamenti.AcceptChanges()

                Exit For
            End If

        Next

    End Sub

#End Region

#Region "Anagrafica"
    Private Function Anagrafica_Appezzamenti_Leggi(
                                    ByVal Piva As String,
                                    ByVal Optional_SaCod As Integer,
                                    ByVal Optional_Appezza As Integer,
                                    ByVal Data_Inizio As Date,
                                    ByVal Data_Fine As Date,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef ErrMSG As String,
                                    ByRef objParametri As AgronicaCoreParametri
                                    ) As DataTable

        '----- Descrizione
        Dim DescrizioneFunzione As String = "Anagrafica Appezzamenti : Lettura"

        Dim DT As New DataTable

        Dim stb As New Text.StringBuilder
        stb.Length = 0

        Try
            stb.Append(" SELECT   Appezzamento.PIVA, Appezzamento.SA_COD, Appezzamento.Campo_Cod, Appezzamento.APPEZZA, Reg_Impianti.ID_REG, Reg_Impianti.Unita_Vitata, ")

            stb.Append("         ISNULL(Centri_Aziendali.sa_nome,'') AS sa_nome, ")
            stb.Append("         ISNULL(Campi.Campo_Des,'') AS Campo_Des, ")

            stb.Append("ISNULL((SELECT TOP 1 Imprese_Progetti.Progetto_Nome FROM Imprese_Progetti ")
            stb.Append("        WHERE Reg_Impianti.Piva = Imprese_Progetti.PIVA ")
            stb.Append("	     AND Reg_Impianti.sa_cod = Imprese_Progetti.sa_cod ")
            stb.Append("	     AND Reg_Impianti.appezza = Imprese_Progetti.APPEZZA ")
            stb.Append("	     AND Reg_Impianti.id_reg = Imprese_Progetti.ID_REG ), '') AS Progetto_Nome, ")
            stb.Append("ISNULL((SELECT TOP 1 Imprese_Progetti.Progetto_Cod FROM Imprese_Progetti ")
            stb.Append("        WHERE Reg_Impianti.Piva = Imprese_Progetti.PIVA ")
            stb.Append("	     AND Reg_Impianti.sa_cod = Imprese_Progetti.sa_cod ")
            stb.Append("	     AND Reg_Impianti.appezza = Imprese_Progetti.APPEZZA ")
            stb.Append("	     AND Reg_Impianti.id_reg = Imprese_Progetti.ID_REG ), 0) AS Progetto_Cod, ")
            stb.Append("ISNULL((SELECT TOP 1 Imprese_Progetti.Produzione_Prevista FROM Imprese_Progetti ")
            stb.Append("        WHERE Reg_Impianti.Piva = Imprese_Progetti.PIVA ")
            stb.Append("	     AND Reg_Impianti.sa_cod = Imprese_Progetti.sa_cod ")
            stb.Append("	     AND Reg_Impianti.appezza = Imprese_Progetti.APPEZZA ")
            stb.Append("	     AND Reg_Impianti.id_reg = Imprese_Progetti.ID_REG ), 0) AS Resa, ")

            stb.Append("         ISNULL(Cultivar.Cul_Cod,-1) as Cul_Cod, ")
            stb.Append("         ISNULL(Cultivar.Cul_Des,'') as Cul_Des, ")
            stb.Append("         ISNULL(SpecieVegetali.Veg_Cod,-1) as Veg_Cod, ")
            stb.Append("         ISNULL(SpecieVegetali.Veg_Des,'') as Veg_Des, ")
            stb.Append("         ISNULL(GruppoVarietale.Grva_Cod,-1) AS Grva_Cod, ")
            stb.Append("         ISNULL(GruppoVarietale.Grva_Des,'') AS Grva_Des, ")
            stb.Append("         ISNULL(GruppoFinalita.Grfi_Cod,-1) as Grfi_Cod, ")
            stb.Append("         ISNULL(GruppoFinalita.Grfi_Des,'') as Grfi_Des, ")
            stb.Append("         ISNULL(Copertura.Cop_Cod,-1) as Cop_Cod, ")
            stb.Append("         ISNULL(Copertura.Cop_Des,'') as Cop_Des, ")

            stb.Append("         '' AS Data_Semina, ")
            stb.Append("         '' AS Data_Raccolta, ")

            stb.Append("         0 as DestinazioneUso,  ")
            stb.Append("         '' as DestinazioneUso_Des,  ")

            stb.Append("         Appezzamento.Validita_Inizio AS Validita_Inizio_Appezzamento, Appezzamento.Validita_Fine AS Validita_Fine_Appezzamento,  ")
            stb.Append("         Reg_Impianti.Validita_Inizio AS Validita_Inizio_Impianto, Reg_Impianti.Validita_Fine AS Validita_Fine_Impianto, ")
            stb.Append("         Appezzamento.SUP_APP, Appezzamento.APP_NOME, ")
            stb.Append("         ISNULL(Reg_Impianti.P_Ha,0) AS P_Ha, ISNULL(Reg_Impianti.Sup_Imp,0) AS Sup_Imp " & vbCrLf)

            'by vanni
            stb.Append("        , ISNULL (ccVeg.val_cod , '') as veg_cod_cliente " & vbCrLf)
            stb.Append("        , ISNULL (ccCul.val_cod , '') as cul_cod_cliente " & vbCrLf)

            ''by maga
            'stb.Append("         , ISNULL( (SELECT Veg_Cod_Agea + '|' + Veg_Des_Agea" & vbCrLf)
            'stb.Append("                     FROM Codifica_SpecieVegetali_Agea " & vbCrLf)
            'stb.Append("                    INNER JOIN Reg_Impianti_Codici ccVeg " & vbCrLf)
            'stb.Append("                    ON ccVeg.val_cod = Codifica_SpecieVegetali_Agea.Veg_Cod_Agea " & vbCrLf)
            'stb.Append("                    AND ccVeg.id_cod = " & Agro_SQL_SaveNum(enum_CodiciAnagrafe.Codice_Specie_Agea) & " " & vbCrLf)
            'stb.Append("                     WHERE ccVeg.PIVA = Reg_Impianti.PIVA  " & vbCrLf)
            'stb.Append("                       and ccVeg.sa_cod = Reg_Impianti.SA_COD  " & vbCrLf)
            'stb.Append("                       and ccVeg.appezza = Reg_Impianti.APPEZZA  " & vbCrLf)
            'stb.Append("                       and ccVeg.Id_Reg = Reg_Impianti.ID_REG  " & vbCrLf)
            'stb.Append("                       and (  " & vbCrLf)
            'stb.Append("                            ccVeg.Progetto_Cod = Imprese_Progetti.Progetto_Cod " & vbCrLf)
            'stb.Append("                                or Imprese_Progetti.Progetto_Cod is null  " & vbCrLf)
            'stb.Append("                        ) " & vbCrLf)
            'stb.Append("                     ), '|') AS VegCodDes_Agea " & vbCrLf)

            'stb.Append("         , ISNULL( (SELECT cul_Cod_Agea + '|' + cul_Des_Agea" & vbCrLf)
            'stb.Append("                     FROM Codifica_SpecieVegetali_Agea " & vbCrLf)
            'stb.Append("                    INNER JOIN Reg_Impianti_Codici ccVeg " & vbCrLf)
            'stb.Append("                    ON ccVeg.val_cod = Codifica_SpecieVegetali_Agea.Cul_Cod_Agea " & vbCrLf)
            'stb.Append("                    AND ccVeg.id_cod = " & Agro_SQL_SaveNum(enum_CodiciAnagrafe.Codice_Cultivar_Agea) & " " & vbCrLf)
            'stb.Append("                     WHERE ccVeg.PIVA = Reg_Impianti.PIVA  " & vbCrLf)
            'stb.Append("                       and ccVeg.sa_cod = Reg_Impianti.SA_COD  " & vbCrLf)
            'stb.Append("                       and ccVeg.appezza = Reg_Impianti.APPEZZA  " & vbCrLf)
            'stb.Append("                       and ccVeg.Id_Reg = Reg_Impianti.ID_REG  " & vbCrLf)
            'stb.Append("                       and (  " & vbCrLf)
            'stb.Append("                            ccVeg.Progetto_Cod = Imprese_Progetti.Progetto_Cod " & vbCrLf)
            'stb.Append("                                or Imprese_Progetti.Progetto_Cod is null  " & vbCrLf)
            'stb.Append("                        ) " & vbCrLf)
            'stb.Append("                     ), '|') AS culCodDes_Agea " & vbCrLf)

            'stb.Append("         " & vbCrLf)
            'stb.Append("         " & vbCrLf)
            'stb.Append("         " & vbCrLf)
            'stb.Append("         " & vbCrLf)
            'stb.Append("         " & vbCrLf)
            'stb.Append("         " & vbCrLf)
            'stb.Append("         " & vbCrLf)

            stb.Append(" FROM    Imprese_Progetti RIGHT OUTER JOIN ")
            stb.Append("         Reg_Impianti ON Imprese_Progetti.Piva = Reg_Impianti.PIVA AND Imprese_Progetti.Sa_Cod = Reg_Impianti.SA_COD AND  ")
            stb.Append("         Imprese_Progetti.Appezza = Reg_Impianti.APPEZZA AND Imprese_Progetti.Id_Reg = Reg_Impianti.ID_REG LEFT OUTER JOIN ")
            stb.Append("         Copertura ON Reg_Impianti.COP_COD = Copertura.Cop_Cod LEFT OUTER JOIN ")
            stb.Append("         GruppoVarietale ON Reg_Impianti.GRVA_Cod_VEG = GruppoVarietale.Grva_Cod LEFT OUTER JOIN ")
            stb.Append("         GruppoFinalita ON Reg_Impianti.GRFI_COD = GruppoFinalita.Grfi_Cod LEFT OUTER JOIN ")
            stb.Append("         Cultivar INNER JOIN ")
            stb.Append("         SpecieVegetali ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod RIGHT OUTER JOIN ")
            stb.Append("         Appezzamento ON Reg_Impianti.APPEZZA = Appezzamento.APPEZZA AND Reg_Impianti.SA_COD = Appezzamento.SA_COD AND  ")
            stb.Append("         Reg_Impianti.PIVA = Appezzamento.PIVA ")

            stb.Append(" LEFT OUTER JOIN ")
            stb.Append("           Campi ON Appezzamento.Campo_Cod = Campi.Campo_Cod AND Appezzamento.Piva = Campi.Piva AND  ")
            stb.Append("           Appezzamento.Sa_Cod = Campi.Sa_Cod LEFT OUTER JOIN ")
            stb.Append("           Centri_Aziendali ON Appezzamento.Piva = Centri_Aziendali.PIVA AND Appezzamento.Sa_Cod = Centri_Aziendali.sa_cod ")

            'BY VANNI
            stb.Append("    left join Reg_Impianti_Codici ccVeg " & vbCrLf)
            stb.Append("                       on ccVeg.PIVA = Reg_Impianti.PIVA  " & vbCrLf)
            stb.Append("                       and ccVeg.sa_cod = Reg_Impianti.SA_COD  " & vbCrLf)
            stb.Append("                       and ccVeg.appezza = Reg_Impianti.APPEZZA  " & vbCrLf)
            stb.Append("                       and ccVeg.Id_Reg = Reg_Impianti.ID_REG  " & vbCrLf)
            stb.Append("                       and ccVeg.id_cod = " & Agro_SQL_SaveNum(enum_CodiciAnagrafe.Codice_Specie_Agea) & " " & vbCrLf)
            stb.Append("                       and (  " & vbCrLf)
            stb.Append("             ccVeg.Progetto_Cod = Imprese_Progetti.Progetto_Cod " & vbCrLf)
            stb.Append("                        or Imprese_Progetti.Progetto_Cod is null  " & vbCrLf)
            stb.Append("                        ) " & vbCrLf)
            stb.Append("   left join Reg_Impianti_Codici ccCul " & vbCrLf)
            stb.Append("                       on ccCul.PIVA = Reg_Impianti.PIVA  " & vbCrLf)
            stb.Append("                       and ccCul.sa_cod = Reg_Impianti.SA_COD  " & vbCrLf)
            stb.Append("                       and ccCul.appezza = Reg_Impianti.APPEZZA  " & vbCrLf)
            stb.Append("                       and ccCul.Id_Reg = Reg_Impianti.ID_REG  " & vbCrLf)
            stb.Append("                       and ccCul.id_cod = " & Agro_SQL_SaveNum(enum_CodiciAnagrafe.Codice_Cultivar_Agea) & " " & vbCrLf)
            stb.Append("                       and (  " & vbCrLf)
            stb.Append("             ccCul.Progetto_Cod = Imprese_Progetti.Progetto_Cod " & vbCrLf)
            stb.Append("                        or Imprese_Progetti.Progetto_Cod is null  " & vbCrLf)
            stb.Append("                        ) " & vbCrLf)

            stb.Append(" WHERE   (Appezzamento.PIVA = '" & Agro_SQL_SaveText(Piva) & "')  ")

            If Optional_SaCod <> 0 Then
                stb.Append(" AND     (Appezzamento.SA_COD = " & Agro_SQL_SaveNum(Optional_SaCod.ToString) & ")      ")
            End If

            If Optional_Appezza <> 0 Then
                stb.Append(" AND     (Appezzamento.APPEZZA = " & Agro_SQL_SaveNum(Optional_Appezza.ToString) & ")    ")
            End If

            stb.Append(" AND     (Appezzamento.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Fine) & ") ")
            stb.Append(" AND     (Appezzamento.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Inizio) & ")   ")
            stb.Append(" AND     (Reg_Impianti.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Fine) & ")         ")
            stb.Append(" AND     (Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Inizio) & ")         ")
            'StrSQL.Append(" AND     (Imprese_Progetti.Validita_Inizio <= " & SQL_SaveDate(Data_Istantanea) & ")         ")
            'StrSQL.Append(" AND     (Imprese_Progetti.Validita_Fine >= " & SQL_SaveDate(Data_Istantanea) & ")         ")

            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            stb.Append(" ORDER BY Appezzamento.PIVA, Appezzamento.SA_COD, Appezzamento.APP_NOME ")

            '----------------------------------------------------
            '--- Recupero il datatable --------------------------
            '----------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, DescrizioneFunzione)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            ErrMSG = ex.Message
            Scrivi_LOG(objParametri, DescrizioneFunzione, ErrMSG)
            DT = Nothing
            Throw New Exception("[" & DescrizioneFunzione & "] : " & ErrMSG)
        End Try
        Return DT


    End Function

    Private Function Anagrafica_Appezzamenti_Leggi_2(
                                   ByVal Piva As String,
                                   ByVal Optional_SaCod As Integer,
                                   ByVal Optional_Appezza As Integer,
                                   ByVal Data_Inizio As Date,
                                   ByVal Data_Fine As Date,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByRef ErrMSG As String,
                                   ByRef objParametri As AgronicaCoreParametri
                                   ) As DataTable

        '----- Descrizione
        Dim DescrizioneFunzione As String = "Anagrafica Appezzamenti : Lettura"

        Dim DT As New DataTable

        Dim stb As New Text.StringBuilder
        stb.Length = 0

        Try
            stb.Append(" SELECT DISTINCT  Appezzamento.PIVA, Appezzamento.SA_COD, Appezzamento.Campo_Cod, Appezzamento.APPEZZA, ISNULL(Reg_Impianti.ID_REG,0) AS ID_REG, Reg_Impianti.Unita_Vitata, ")

            stb.Append("         ISNULL(Centri_Aziendali.sa_nome,'') AS sa_nome, ")
            stb.Append("         ISNULL(Campi.Campo_Des,'') AS Campo_Des, ")

            stb.Append("         ISNULL(Imprese_Progetti.Progetto_Cod,0) AS Progetto_Cod, ")
            stb.Append("         ISNULL(Imprese_Progetti.Progetto_Nome,'') AS Progetto_Nome, ")
            stb.Append("         ISNULL(Imprese_Progetti.Produzione_Prevista,0)  AS Resa, ")
            stb.Append("         Imprese_Progetti.Validita_Inizio AS Validita_Inizio_Distinta, ")
            stb.Append("         Imprese_Progetti.Validita_Fine AS Validita_Fine_Distinta, ")


            stb.Append("         ISNULL(Cultivar.Cul_Cod,-1) as Cul_Cod, ")
            stb.Append("         ISNULL(Cultivar.Cul_Des,'') as Cul_Des, ")
            stb.Append("         ISNULL(SpecieVegetali.Veg_Cod,-1) as Veg_Cod, ")
            stb.Append("         ISNULL(SpecieVegetali.Veg_Des,'') as Veg_Des, ")
            stb.Append("         ISNULL(GruppoVarietale.Grva_Cod,-1) AS Grva_Cod, ")
            stb.Append("         ISNULL(GruppoVarietale.Grva_Des,'') AS Grva_Des, ")
            stb.Append("         ISNULL(GruppoFinalita.Grfi_Cod,-1) as Grfi_Cod, ")
            stb.Append("         ISNULL(GruppoFinalita.Grfi_Des,'') as Grfi_Des, ")
            stb.Append("         ISNULL(Copertura.Cop_Cod,-1) as Cop_Cod, ")
            stb.Append("         ISNULL(Copertura.Cop_Des,'') as Cop_Des, ")

            stb.Append("         '' AS Data_Semina, ")
            stb.Append("         '' AS Data_Raccolta, ")

            stb.Append("         0 as DestinazioneUso,  ")
            stb.Append("         '' as DestinazioneUso_Des,  ")

            stb.Append("         Appezzamento.Validita_Inizio AS Validita_Inizio_Appezzamento, Appezzamento.Validita_Fine AS Validita_Fine_Appezzamento,  ")
            stb.Append("         Reg_Impianti.Validita_Inizio AS Validita_Inizio_Impianto, Reg_Impianti.Validita_Fine AS Validita_Fine_Impianto, ")
            stb.Append("         Appezzamento.SUP_APP, Appezzamento.APP_NOME, ")
            stb.Append("         ISNULL(Reg_Impianti.P_Ha,0) AS P_Ha, ISNULL(Reg_Impianti.Sup_Imp,0) AS Sup_Imp " & vbCrLf)

            'by vanni
            stb.Append("        , ISNULL (ccVeg.val_cod , '') as veg_cod_cliente " & vbCrLf)
            stb.Append("        , ISNULL (ccCul.val_cod , '') as cul_cod_cliente " & vbCrLf)

            stb.Append(" FROM    Imprese_Progetti RIGHT OUTER JOIN ")
            stb.Append("         Reg_Impianti ON Imprese_Progetti.Piva = Reg_Impianti.PIVA AND Imprese_Progetti.Sa_Cod = Reg_Impianti.SA_COD AND  ")
            stb.Append("         Imprese_Progetti.Appezza = Reg_Impianti.APPEZZA AND Imprese_Progetti.Id_Reg = Reg_Impianti.ID_REG LEFT OUTER JOIN ")
            stb.Append("         Copertura ON Reg_Impianti.COP_COD = Copertura.Cop_Cod LEFT OUTER JOIN ")
            stb.Append("         GruppoVarietale ON Reg_Impianti.GRVA_Cod_VEG = GruppoVarietale.Grva_Cod LEFT OUTER JOIN ")
            stb.Append("         GruppoFinalita ON Reg_Impianti.GRFI_COD = GruppoFinalita.Grfi_Cod LEFT OUTER JOIN ")
            stb.Append("         Cultivar INNER JOIN ")
            stb.Append("         SpecieVegetali ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod RIGHT OUTER JOIN ")
            stb.Append("         Appezzamento ON Reg_Impianti.APPEZZA = Appezzamento.APPEZZA AND Reg_Impianti.SA_COD = Appezzamento.SA_COD AND  ")
            stb.Append("         Reg_Impianti.PIVA = Appezzamento.PIVA ")

            stb.Append(" LEFT OUTER JOIN ")
            stb.Append("           Campi ON Appezzamento.Campo_Cod = Campi.Campo_Cod AND Appezzamento.Piva = Campi.Piva AND  ")
            stb.Append("           Appezzamento.Sa_Cod = Campi.Sa_Cod LEFT OUTER JOIN ")
            stb.Append("           Centri_Aziendali ON Appezzamento.Piva = Centri_Aziendali.PIVA AND Appezzamento.Sa_Cod = Centri_Aziendali.sa_cod ")

            'BY VANNI
            stb.Append("    left join Reg_Impianti_Codici ccVeg " & vbCrLf)
            stb.Append("                       on ccVeg.PIVA = Reg_Impianti.PIVA  " & vbCrLf)
            stb.Append("                       and ccVeg.sa_cod = Reg_Impianti.SA_COD  " & vbCrLf)
            stb.Append("                       and ccVeg.appezza = Reg_Impianti.APPEZZA  " & vbCrLf)
            stb.Append("                       and ccVeg.Id_Reg = Reg_Impianti.ID_REG  " & vbCrLf)
            stb.Append("                       and ccVeg.id_cod = " & Agro_SQL_SaveNum(enum_CodiciAnagrafe.Codice_Specie_Agea) & " " & vbCrLf)
            stb.Append("                       and (  " & vbCrLf)
            stb.Append("             ccVeg.Progetto_Cod = Imprese_Progetti.Progetto_Cod " & vbCrLf)
            stb.Append("                        or Imprese_Progetti.Progetto_Cod is null  " & vbCrLf)
            stb.Append("                        ) " & vbCrLf)
            stb.Append("   left join Reg_Impianti_Codici ccCul " & vbCrLf)
            stb.Append("                       on ccCul.PIVA = Reg_Impianti.PIVA  " & vbCrLf)
            stb.Append("                       and ccCul.sa_cod = Reg_Impianti.SA_COD  " & vbCrLf)
            stb.Append("                       and ccCul.appezza = Reg_Impianti.APPEZZA  " & vbCrLf)
            stb.Append("                       and ccCul.Id_Reg = Reg_Impianti.ID_REG  " & vbCrLf)
            stb.Append("                       and ccCul.id_cod = " & Agro_SQL_SaveNum(enum_CodiciAnagrafe.Codice_Cultivar_Agea) & " " & vbCrLf)
            stb.Append("                       and (  " & vbCrLf)
            stb.Append("             ccCul.Progetto_Cod = Imprese_Progetti.Progetto_Cod " & vbCrLf)
            stb.Append("                        or Imprese_Progetti.Progetto_Cod is null  " & vbCrLf)
            stb.Append("                        ) " & vbCrLf)

            stb.Append(" WHERE   (Appezzamento.PIVA = '" & Agro_SQL_SaveText(Piva) & "')  ")

            If Optional_SaCod <> 0 Then
                stb.Append(" AND     (Appezzamento.SA_COD = " & Agro_SQL_SaveNum(Optional_SaCod.ToString) & ")      ")
            End If

            If Optional_Appezza <> 0 Then
                stb.Append(" AND     (Appezzamento.APPEZZA = " & Agro_SQL_SaveNum(Optional_Appezza.ToString) & ")    ")
            End If

            stb.Append(" AND     (Appezzamento.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Fine) & ") ")
            stb.Append(" AND     (Appezzamento.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Inizio) & ")   ")
            'stb.Append(" AND     (Reg_Impianti.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Fine) & ")         ")
            'stb.Append(" AND     (Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Inizio) & ")         ")
            'StrSQL.Append(" AND     (Imprese_Progetti.Validita_Inizio <= " & SQL_SaveDate(Data_Istantanea) & ")         ")
            'StrSQL.Append(" AND     (Imprese_Progetti.Validita_Fine >= " & SQL_SaveDate(Data_Istantanea) & ")         ")

            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            stb.Append(" ORDER BY Appezzamento.PIVA, Appezzamento.SA_COD, Appezzamento.APP_NOME ")

            '----------------------------------------------------
            '--- Recupero il datatable --------------------------
            '----------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, DescrizioneFunzione)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            ErrMSG = ex.Message
            Scrivi_LOG(objParametri, DescrizioneFunzione, ErrMSG)
            DT = Nothing
            Throw New Exception("[" & DescrizioneFunzione & "] : " & ErrMSG)
        End Try
        Return DT


    End Function

    Private Function Anagrafica_AppezzamentixParticelle_Leggi(
                                    ByVal Piva As String,
                                    ByVal Sa_Cod As Integer,
                                    ByVal Appezza As Integer,
                                    ByVal Data_Inizio As Date,
                                    ByVal Data_Fine As Date,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef ErrMSG As String,
                                    ByRef objParametri As AgronicaCoreParametri) _
                                    As DataTable


        '----- Descrizione
        Dim DescrizioneFunzione As String = "AppezzamentixParticelle : Lettura"

        Dim DT As New DataTable

        Dim StrSQL As New Text.StringBuilder
        StrSQL.Length = 0

        Try



            StrSQL.Append(" SELECT  AppezzamentiXParticelle.PIVA, AppezzamentiXParticelle.SA_COD, AppezzamentiXParticelle.APPEZZA, AppezzamentiXParticelle.PROV,   ")
            StrSQL.Append("         AppezzamentiXParticelle.COM, AppezzamentiXParticelle.SEZIONE, AppezzamentiXParticelle.FOGLIO, AppezzamentiXParticelle.NUMERO,   ")
            StrSQL.Append("         AppezzamentiXParticelle.SUBALTERNO, AppezzamentiXParticelle.AREA AS SupIntersezione, ISTAT.LOCALITA AS Com_Des,   ")
            StrSQL.Append("         ISTAT.COMUNI_PROV AS Prov_Des, ")
            StrSQL.Append("         ImpresexParticelle.Sup_Condotta AS SupTotale,   ")
            StrSQL.Append("         AppezzamentiXParticelle.Validita_Inizio AS Validita_Inizio_Intersezione, AppezzamentiXParticelle.Validita_Fine AS Validita_Fine_Intersezione  ")

            StrSQL.Append(" FROM    AppezzamentiXParticelle INNER JOIN  ")
            StrSQL.Append("         ISTAT ON AppezzamentiXParticelle.PROV = ISTAT.PROV AND AppezzamentiXParticelle.COM = ISTAT.COM INNER JOIN  ")
            StrSQL.Append("         ImpresexParticelle ON AppezzamentiXParticelle.PROV = ImpresexParticelle.PROV AND AppezzamentiXParticelle.COM = ImpresexParticelle.COM AND   ")
            StrSQL.Append("         AppezzamentiXParticelle.SEZIONE = ImpresexParticelle.SEZIONE AND AppezzamentiXParticelle.FOGLIO = ImpresexParticelle.FOGLIO AND   ")
            StrSQL.Append("         AppezzamentiXParticelle.NUMERO = ImpresexParticelle.NUMERO AND   ")
            StrSQL.Append("         AppezzamentiXParticelle.SUBALTERNO = ImpresexParticelle.SUBALTERNO  AND  AppezzamentiXParticelle.PIVA = ImpresexParticelle.PIVA  ")

            StrSQL.Append(" WHERE   1=1 ")

            If Piva <> "" Then
                StrSQL.Append(" AND     (AppezzamentiXParticelle.PIVA = '" & Agro_SQL_SaveText(Piva) & "')   ")
            End If
            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND     (AppezzamentiXParticelle.SA_COD = " & Agro_SQL_SaveNum(Sa_Cod.ToString) & ")  ")
            End If
            If Appezza <> 0 Then
                StrSQL.Append(" AND     (AppezzamentiXParticelle.APPEZZA = " & Agro_SQL_SaveNum(Appezza.ToString) & ") ")
            End If

            StrSQL.Append(" AND     (AppezzamentiXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Fine) & ")   ")
            StrSQL.Append(" AND     (AppezzamentiXParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Inizio) & ")  ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            StrSQL.Append(" ORDER BY ISTAT.COMUNI_PROV , ISTAT.LOCALITA  ")

            '----------------------------------------------------
            '--- Recupero il datatable --------------------------
            '----------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, DescrizioneFunzione)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            ErrMSG = ex.Message
            Scrivi_LOG(objParametri, DescrizioneFunzione, ErrMSG)
            DT = Nothing
            Throw New Exception("[" & DescrizioneFunzione & "] : " & ErrMSG)
        End Try
        Return DT

    End Function

    Public Function Anagrafica_ElencoAppezzamenti_Leggi(
                                    ByVal Piva As String,
                                    ByVal Optional_SaCod As Integer,
                                    ByVal Data_Inizio As Date,
                                    ByVal Data_Fine As Date,
                                    ByRef ErrMSG As String,
                                    ByRef objParametri As AgronicaCoreParametri) _
                                    As DataTable

        '----- Descrizione
        Const descrizioneFunzione = "Anagrafica ElencoAppezzamenti : Lettura"

        Dim dt As New DataTable

        Dim StrSQL As New Text.StringBuilder
        StrSQL.Length = 0

        Try
            StrSQL.Append(" SELECT  Centri_Aziendali.PIVA, Centri_Aziendali.sa_cod, Centri_Aziendali.sa_nome, Appezzamento.APPEZZA, Appezzamento.APP_NOME, ")
            StrSQL.Append("         Appezzamento.SUP_APP, Reg_Impianti.ID_REG, ISNULL(SpecieVegetali.Veg_Cod,0) AS Veg_Cod, ISNULL(SpecieVegetali.Veg_Des,'') AS Veg_Des, ISNULL(Cultivar.Cul_Cod,0) AS Cul_Cod, ISNULL(Cultivar.Cul_Des,'') AS Cul_Des, ")
            StrSQL.Append("         Reg_Impianti.GRFI_COD, GruppoFinalita.Grfi_Des, ISNULL(Reg_Impianti.COP_COD, - 1) AS Cop_Cod, ISNULL(Copertura.Cop_Des, '') AS Cop_Des, ")
            StrSQL.Append("         Appezzamento.Validita_Inizio AS Validita_Inizio_Appezzamento, Appezzamento.Validita_Fine AS Validita_Fine_Appezzamento, ")
            StrSQL.Append("         Reg_Impianti.Validita_Inizio AS Validita_Inizio_Impianto, Reg_Impianti.Validita_Fine AS Validita_Fine_Impianto,  ")
            StrSQL.Append("         Appezzamento.Campo_Cod, ISNULL(Campi.Campo_Des,'') AS Campo_Des,  ")

            StrSQL.Append("ISNULL((SELECT TOP 1 Imprese_Progetti.Progetto_Nome FROM Imprese_Progetti ")
            StrSQL.Append("        WHERE Reg_Impianti.Piva = Imprese_Progetti.PIVA ")
            StrSQL.Append("	     AND Reg_Impianti.sa_cod = Imprese_Progetti.sa_cod ")
            StrSQL.Append("	     AND Reg_Impianti.appezza = Imprese_Progetti.APPEZZA ")
            StrSQL.Append("	     AND Reg_Impianti.id_reg = Imprese_Progetti.ID_REG ), '') AS Progetto_Nome, ")
            StrSQL.Append("ISNULL((SELECT TOP 1 Imprese_Progetti.Progetto_Cod FROM Imprese_Progetti ")
            StrSQL.Append("        WHERE Reg_Impianti.Piva = Imprese_Progetti.PIVA ")
            StrSQL.Append("	     AND Reg_Impianti.sa_cod = Imprese_Progetti.sa_cod ")
            StrSQL.Append("	     AND Reg_Impianti.appezza = Imprese_Progetti.APPEZZA ")
            StrSQL.Append("	     AND Reg_Impianti.id_reg = Imprese_Progetti.ID_REG ), 0) AS Progetto_Cod, ")
            StrSQL.Append("ISNULL((SELECT TOP 1 Imprese_Progetti.Produzione_Prevista FROM Imprese_Progetti ")
            StrSQL.Append("        WHERE Reg_Impianti.Piva = Imprese_Progetti.PIVA ")
            StrSQL.Append("	     AND Reg_Impianti.sa_cod = Imprese_Progetti.sa_cod ")
            StrSQL.Append("	     AND Reg_Impianti.appezza = Imprese_Progetti.APPEZZA ")
            StrSQL.Append("	     AND Reg_Impianti.id_reg = Imprese_Progetti.ID_REG ), 0) AS Resa, ")

            'StrSQL.Append("         Imprese_Progetti.Progetto_Cod, Imprese_Progetti.Progetto_Nome, Imprese_Progetti.Progetto_Des, Imprese_Progetti.Validita_Inizio AS Validita_Inizio_Distinta, ")
            'StrSQL.Append("         Imprese_Progetti.Validita_Fine AS Validita_Fine_Distinta,  ")

            StrSQL.Append("         ISTAT.COMUNI_PROV, ISTAT.LOCALITA, AppezzamentiXParticelle.PROV, AppezzamentiXParticelle.COM, AppezzamentiXParticelle.SEZIONE, AppezzamentiXParticelle.FOGLIO, AppezzamentiXParticelle.NUMERO, ")
            StrSQL.Append("         AppezzamentiXParticelle.SUBALTERNO, AppezzamentiXParticelle.AREA, ")
            'StrSQL.Append("         ParticelleCatastali.ETTARI, ParticelleCatastali.[ARE], ParticelleCatastali.CENTIARE, ")
            StrSQL.Append("         AppezzamentiXParticelle.Validita_Inizio AS Validita_Inizio_Intersezione, ")
            StrSQL.Append("         AppezzamentiXParticelle.Validita_Fine AS Validita_Fine_Intersezione, ")

            StrSQL.Append("         ParticelleCatastali.PROV + '_' + ")
            StrSQL.Append("         ParticelleCatastali.COM + '_' + ")
            StrSQL.Append("         ParticelleCatastali.SEZIONE  + '_' + ")
            StrSQL.Append("         CONVERT(varchar(10), ParticelleCatastali.FOGLIO) + '_' + ")
            StrSQL.Append("         CONVERT(varchar(10), ParticelleCatastali.NUMERO) + '_' + ")
            StrSQL.Append("         ParticelleCatastali.SUBALTERNO AS PART_UNID, ")

            'StrSQL.Append("         Imprese_Progetti.Produzione_Prevista as Resa,  ")

            StrSQL.Append("         0 as DestinazioneUso,  ")
            StrSQL.Append("         '' as DestinazioneUso_Des  ")

            StrSQL.Append(" FROM    Appezzamento INNER JOIN ")
            StrSQL.Append("         Centri_Aziendali ON Appezzamento.PIVA = Centri_Aziendali.PIVA AND Appezzamento.SA_COD = Centri_Aziendali.sa_cod INNER JOIN ")
            StrSQL.Append("         Reg_Impianti ON Appezzamento.PIVA = Reg_Impianti.PIVA AND Appezzamento.SA_COD = Reg_Impianti.SA_COD AND  ")
            StrSQL.Append("         Appezzamento.APPEZZA = Reg_Impianti.APPEZZA INNER JOIN ")
            'StrSQL.Append("         Imprese_Progetti ON Reg_Impianti.PIVA = Imprese_Progetti.Piva AND Reg_Impianti.SA_COD = Imprese_Progetti.Sa_Cod AND  ")
            'StrSQL.Append("         Reg_Impianti.APPEZZA = Imprese_Progetti.Appezza AND Reg_Impianti.ID_REG = Imprese_Progetti.Id_Reg INNER JOIN ")
            StrSQL.Append("         AppezzamentiXParticelle ON Appezzamento.PIVA = AppezzamentiXParticelle.PIVA AND  ")
            StrSQL.Append("         Appezzamento.SA_COD = AppezzamentiXParticelle.SA_COD AND Appezzamento.APPEZZA = AppezzamentiXParticelle.APPEZZA INNER JOIN ")
            StrSQL.Append("         ParticelleCatastali ON AppezzamentiXParticelle.PROV = ParticelleCatastali.PROV AND AppezzamentiXParticelle.COM = ParticelleCatastali.COM AND  ")
            StrSQL.Append("         AppezzamentiXParticelle.SEZIONE = ParticelleCatastali.SEZIONE AND AppezzamentiXParticelle.FOGLIO = ParticelleCatastali.FOGLIO AND  ")
            StrSQL.Append("         AppezzamentiXParticelle.NUMERO = ParticelleCatastali.NUMERO AND  ")
            StrSQL.Append("         AppezzamentiXParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO INNER JOIN ")
            StrSQL.Append("         ISTAT ON ParticelleCatastali.PROV = ISTAT.PROV AND ParticelleCatastali.COM = ISTAT.COM LEFT OUTER JOIN ")
            StrSQL.Append("         Campi ON Appezzamento.PIVA = Campi.Piva AND Appezzamento.SA_COD = Campi.Sa_Cod AND Appezzamento.Campo_Cod = Campi.Campo_Cod  ")
            StrSQL.Append("         LEFT OUTER JOIN GruppoFinalita ON Reg_Impianti.Grfi_Cod = GruppoFinalita.Grfi_Cod  ")
            StrSQL.Append("         LEFT OUTER JOIN Copertura ON Reg_Impianti.COP_COD = Copertura.Cop_Cod  ")
            StrSQL.Append("         LEFT OUTER JOIN SpecieVegetali INNER JOIN ")
            StrSQL.Append("         Cultivar ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod ")

            StrSQL.Append(" WHERE   (Appezzamento.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Fine) & ")  ")
            StrSQL.Append(" AND     (Appezzamento.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Inizio) & ")  ")
            StrSQL.Append(" AND     (Reg_Impianti.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Fine) & ")  ")
            StrSQL.Append(" AND     (Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Inizio) & ")  ")
            StrSQL.Append(" AND     (AppezzamentiXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Fine) & ")  ")
            StrSQL.Append(" AND     (AppezzamentiXParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Inizio) & ")  ")
            StrSQL.Append(" AND     (Centri_Aziendali.PIVA = '" & Agro_SQL_SaveText(Piva) & "')  ")

            If Optional_SaCod <> 0 Then
                StrSQL.Append(" AND     (Centri_Aziendali.sa_cod = " & Agro_SQL_SaveNum(Optional_SaCod.ToString) & ") ")
            End If

            StrSQL.Append(" ORDER BY Centri_Aziendali.PIVA ASC, Centri_Aziendali.sa_cod ASC, Appezzamento.APPEZZA ASC ,")
            StrSQL.Append("         AppezzamentiXParticelle.PROV, AppezzamentiXParticelle.COM, ")
            StrSQL.Append("         AppezzamentiXParticelle.SEZIONE, AppezzamentiXParticelle.FOGLIO, ")
            StrSQL.Append("         AppezzamentiXParticelle.NUMERO, AppezzamentiXParticelle.SUBALTERNO  ")

            '----------------------------------------------------
            '--- Recupero il datatable --------------------------
            '----------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, descrizioneFunzione)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            ErrMSG = ex.Message
            Scrivi_LOG(objParametri, descrizioneFunzione, ErrMSG)
            dt = Nothing
            Throw New Exception("[" & descrizioneFunzione & "] : " & ErrMSG)
        End Try
        Return dt


    End Function

    Public Function Anagrafica_ElencoAppezzamenti_Leggi_2(
                                  ByVal Piva As String,
                                  ByVal Optional_SaCod As Integer,
                                  ByVal Optional_VegCod As Integer,
                                  ByVal Data_Inizio As Date,
                                  ByVal Data_Fine As Date,
                                  ByRef ErrMSG As String,
                                  ByRef objParametri As AgronicaCoreParametri,
                                  Optional ByVal xFiltroAggiuntivo As String = "") _
                                  As DataTable

        '----- Descrizione
        Dim DescrizioneFunzione As String = "Anagrafica ElencoAppezzamenti : Lettura"

        Dim DT As New DataTable

        Dim StrSQL As New Text.StringBuilder
        StrSQL.Length = 0

        Try
            StrSQL.Append(" SELECT  Centri_Aziendali.PIVA, Centri_Aziendali.sa_cod, Centri_Aziendali.sa_nome, Appezzamento.APPEZZA, Appezzamento.APP_NOME, Appezzamento.SUP_APP, ")
            StrSQL.Append("         ISNULL(Reg_Impianti.ID_REG, 0 ) AS ID_REG,  ")
            StrSQL.Append("         ISNULL(SpecieVegetali.Veg_Cod,0) AS Veg_Cod, ISNULL(SpecieVegetali.Veg_Des,'') AS Veg_Des, ISNULL(Cultivar.Cul_Cod,0) AS Cul_Cod, ISNULL(Cultivar.Cul_Des,'') AS Cul_Des, ")
            StrSQL.Append("         ISNULL(Reg_Impianti.GRFI_COD,0) AS GRFI_COD, ISNULL(GruppoFinalita.Grfi_Des,'') AS Grfi_Des, ISNULL(Reg_Impianti.COP_COD, - 1) AS Cop_Cod, ISNULL(Copertura.Cop_Des, '') AS Cop_Des, ")
            StrSQL.Append("         Appezzamento.Validita_Inizio AS Validita_Inizio_Appezzamento, Appezzamento.Validita_Fine AS Validita_Fine_Appezzamento, ")
            StrSQL.Append("         Reg_Impianti.Validita_Inizio AS Validita_Inizio_Impianto, Reg_Impianti.Validita_Fine AS Validita_Fine_Impianto,  ")
            StrSQL.Append("         Appezzamento.Campo_Cod, ISNULL(Campi.Campo_Des,'') AS Campo_Des,  ")

            StrSQL.Append("         ISNULL(Imprese_Progetti.Progetto_Nome,'') AS Progetto_Nome, ")
            StrSQL.Append("         ISNULL(Imprese_Progetti.Progetto_Cod,0) AS Progetto_Cod,  ")
            StrSQL.Append("         ISNULL(Imprese_Progetti.Produzione_Prevista,0) AS Resa,  ")
            StrSQL.Append("         Imprese_Progetti.Validita_Inizio AS Validita_Inizio_Distinta, ")
            StrSQL.Append("         Imprese_Progetti.Validita_Fine AS Validita_Fine_Distinta, ")

            StrSQL.Append("         ISTAT.COMUNI_PROV, ISTAT.LOCALITA, AppezzamentiXParticelle.PROV, AppezzamentiXParticelle.COM, AppezzamentiXParticelle.SEZIONE, AppezzamentiXParticelle.FOGLIO, AppezzamentiXParticelle.NUMERO, ")
            StrSQL.Append("         AppezzamentiXParticelle.SUBALTERNO, AppezzamentiXParticelle.AREA, ")
            StrSQL.Append("         AppezzamentiXParticelle.Validita_Inizio AS Validita_Inizio_Intersezione, ")
            StrSQL.Append("         AppezzamentiXParticelle.Validita_Fine AS Validita_Fine_Intersezione, ")

            StrSQL.Append("         ParticelleCatastali.PROV + '_' + ")
            StrSQL.Append("         ParticelleCatastali.COM + '_' + ")
            StrSQL.Append("         ParticelleCatastali.SEZIONE  + '_' + ")
            StrSQL.Append("         CONVERT(varchar(10), ParticelleCatastali.FOGLIO) + '_' + ")
            StrSQL.Append("         CONVERT(varchar(10), ParticelleCatastali.NUMERO) + '_' + ")
            StrSQL.Append("         ParticelleCatastali.SUBALTERNO AS PART_UNID, ")

            StrSQL.Append("         0 as DestinazioneUso,  ")
            StrSQL.Append("         '' as DestinazioneUso_Des  ")

            StrSQL.Append(" FROM    Appezzamento INNER JOIN ")
            StrSQL.Append("         Centri_Aziendali ON Appezzamento.PIVA = Centri_Aziendali.PIVA AND Appezzamento.SA_COD = Centri_Aziendali.sa_cod LEFT OUTER JOIN ")

            StrSQL.Append(" ISTAT INNER JOIN ")
            StrSQL.Append(" ParticelleCatastali ON ISTAT.PROV = ParticelleCatastali.PROV AND ISTAT.COM = ParticelleCatastali.COM INNER JOIN ")
            StrSQL.Append(" AppezzamentiXParticelle ON ParticelleCatastali.PROV = AppezzamentiXParticelle.PROV AND ParticelleCatastali.COM = AppezzamentiXParticelle.COM AND  ")
            StrSQL.Append(" ParticelleCatastali.SEZIONE = AppezzamentiXParticelle.SEZIONE AND ParticelleCatastali.FOGLIO = AppezzamentiXParticelle.FOGLIO AND ParticelleCatastali.NUMERO = AppezzamentiXParticelle.NUMERO AND  ")
            StrSQL.Append(" ParticelleCatastali.SUBALTERNO = AppezzamentiXParticelle.SUBALTERNO ON Appezzamento.PIVA = AppezzamentiXParticelle.PIVA AND Appezzamento.SA_COD = AppezzamentiXParticelle.SA_COD AND  ")
            StrSQL.Append(" Appezzamento.APPEZZA = AppezzamentiXParticelle.APPEZZA ")

            StrSQL.Append("         LEFT OUTER JOIN Campi ON Appezzamento.PIVA = Campi.Piva AND Appezzamento.SA_COD = Campi.Sa_Cod AND Appezzamento.Campo_Cod = Campi.Campo_Cod  ")
            StrSQL.Append("         LEFT OUTER JOIN Reg_Impianti ON Appezzamento.PIVA = Reg_Impianti.PIVA AND Appezzamento.SA_COD = Reg_Impianti.SA_COD ")
            StrSQL.Append("         AND Appezzamento.APPEZZA = Reg_Impianti.APPEZZA ")
            StrSQL.Append("         LEFT OUTER JOIN Imprese_Progetti ON Reg_Impianti.piva = Imprese_Progetti.piva and Reg_Impianti.sa_cod = Imprese_Progetti.sa_cod ")
            StrSQL.Append("         AND Reg_Impianti.appezza = Imprese_Progetti.appezza and Reg_Impianti.id_reg = Imprese_Progetti.id_reg ")
            StrSQL.Append("         LEFT OUTER JOIN GruppoFinalita ON Reg_Impianti.Grfi_Cod = GruppoFinalita.Grfi_Cod  ")
            StrSQL.Append("         LEFT OUTER JOIN Copertura ON Reg_Impianti.COP_COD = Copertura.Cop_Cod  ")
            StrSQL.Append("         LEFT OUTER JOIN SpecieVegetali INNER JOIN ")
            StrSQL.Append("         Cultivar ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod ")

            StrSQL.Append(" WHERE   (Appezzamento.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Fine) & ")  ")
            StrSQL.Append(" AND     (Appezzamento.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Inizio) & ")  ")
            StrSQL.Append(" AND     (Centri_Aziendali.PIVA = '" & Agro_SQL_SaveText(Piva) & "')  ")

            If Optional_SaCod <> 0 Then
                StrSQL.Append(" AND     (Centri_Aziendali.sa_cod = " & Agro_SQL_SaveNum(Optional_SaCod.ToString) & ") ")
            End If

            If Optional_VegCod <> 0 Then
                StrSQL.Append(" AND     (SpecieVegetali.Veg_Cod = " & Agro_SQL_SaveNum(Optional_VegCod.ToString) & ") ")
            End If

            If xFiltroAggiuntivo <> "" Then
                If Left(LTrim(xFiltroAggiuntivo), 3).ToUpper = "AND" Then
                    StrSQL.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                Else
                    StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                End If
            End If

            StrSQL.Append(" ORDER BY Centri_Aziendali.PIVA ASC, Centri_Aziendali.sa_cod ASC, Appezzamento.Campo_Cod ASC, Appezzamento.APPEZZA ASC ,")
            StrSQL.Append("         AppezzamentiXParticelle.PROV, AppezzamentiXParticelle.COM, ")
            StrSQL.Append("         AppezzamentiXParticelle.SEZIONE, AppezzamentiXParticelle.FOGLIO, ")
            StrSQL.Append("         AppezzamentiXParticelle.NUMERO, AppezzamentiXParticelle.SUBALTERNO  ")

            '----------------------------------------------------
            '--- Recupero il datatable --------------------------
            '----------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, DescrizioneFunzione)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            ErrMSG = ex.Message
            Scrivi_LOG(objParametri, DescrizioneFunzione, ErrMSG)
            DT = Nothing
            Throw New Exception("[" & DescrizioneFunzione & "] : " & ErrMSG)
        End Try
        Return DT


    End Function

    Public Function Anagrafica_AppezzamentixRibaltamento_Leggi(ByVal Programmazione_Cod As Integer,
                                                           ByVal Piva_SuperUser As String,
                                                           ByRef ErrMSG As String,
                                                           ByRef objParametri As AgronicaCoreParametri) _
                                                           As DataTable


        '----- Descrizione
        Dim DescrizioneFunzione As String = "AppezzamentixRibaltamento : Lettura"

        Dim DT As New DataTable

        Dim StrSQL As New Text.StringBuilder
        StrSQL.Length = 0

        Try
            StrSQL.Append(" SELECT Programmazione_Entita.Piva_SuperUser, Programmazione_Entita.Programmazione_Cod, Programmazione_Entita.Programmazione_Entita_Cod, ISNULL(Programmazione_Entita.Operazione_Cod,0) AS Operazione_Cod, ")
            StrSQL.Append(" Programmazione_Entita.Entita_Des AS App_Nome, Programmazione_Entita.Piva, Programmazione_Entita.Sa_Cod, Programmazione_Entita.Campo_Cod,  ")
            StrSQL.Append(" Programmazione_Entita.Appezza, Programmazione_Entita.Id_Reg, Programmazione_Entita.Progetto_Cod, Programmazione_Entita.Progetto_Des AS Progetto_Nome,  ")
            StrSQL.Append(" Programmazione_Entita.Id_Cod, Programmazione_Entita.Veg_Cod, Programmazione_Entita.Cul_Cod, Programmazione_Entita.Grfi_Cod,  ")
            StrSQL.Append(" Programmazione_Entita.Cop_Cod, Programmazione_Entita.Superficie AS Sup_App, Programmazione_Entita.Resa, Programmazione_Entita.TipoZona,  ")
            StrSQL.Append(" Programmazione_Entita.Veg_Cod_Prec, Programmazione_Entita.Id_Mat_O, Programmazione_Entita.Id_Fre, Programmazione_Entita.N_distribuito,  ISNULL(Programmazione_Entita.Unita_Vitata,0) AS Unita_Vitata,")
            StrSQL.Append(" Programmazione_Entita.Validita_Inizio, Programmazione_Entita.Validita_Fine, Programmazione_Entita.Num_Piante, Programmazione_Entita.Veg_Cod_Cliente, Programmazione_Entita.Cul_Cod_Cliente, ")
            StrSQL.Append(" Programmazione_Entita.TRA_Fila, Programmazione_Entita.SU_Fila, Programmazione_Entita.MetodoProduzione_Cod, Programmazione_Entita.Disciplinare_Cod, Programmazione_Entita.Regolamento_Cod,")
            StrSQL.Append(" Programmazione_Entita.Stato_Cod, Programmazione_Entita.Limite_N, Programmazione_Entita.Limite_P, Programmazione_Entita.Limite_K,  ")
            StrSQL.Append(" Programmazione_Entita.Data_Semina, Programmazione_Entita.Data_Raccolta, Programmazione_Entita.Data_Fioritura_Prevista, ")
            StrSQL.Append(" Programmazione_Entita.Veg_Cod_Prec2, Programmazione_Entita.Veg_Cod_Prec3, Programmazione_Entita.Veg_Cod_Prec4, ")
            StrSQL.Append(" Programmazione_Entita.Piano_Semina, Programmazione_Entita.Codice_Contratto, ")
            StrSQL.Append(" praticacod.val_cod as Pratica_Cod, Programmazione_Entita.Regolamento_Concimazione_Cod, Programmazione_Entita.Flag_PubblicoPrivato, ")
            StrSQL.Append(" Programmazione_Entita.Id_tr,  ")
            StrSQL.Append(" Programmazione_Entita.DistBZ_CorpiIdrici, Programmazione_Entita.DistBZ_AreeResPub, Programmazione_Entita.DistBZ_Allevamenti, Programmazione_Entita.DistBZ_VegNatNonColt, Programmazione_Entita.SupBZ_Riduzione,  ")
            StrSQL.Append(" CONVERT(varchar, Programmazione_Entita.Validita_Inizio_Impianto, 103) AS Validita_Inizio_Impianto, ")
            StrSQL.Append(" ISNULL(Programmazione_Entita.Grva_Cod, 0) AS Grva_Cod, ISNULL(SpecieVegetali.Gru_Cod, 0) AS Gru_Cod, ISNULL(SpecieVegetali.Veg_Des, '') AS Veg_Des, ISNULL(Cultivar.Cul_Des, '') AS Cul_Des, ")
            StrSQL.Append(" ISNULL(GruppoVarietale.Grva_Des, '') AS Grva_Des, ISNULL(GruppoFinalita.Grfi_Des, '') AS Grfi_Des, ")
            StrSQL.Append(" ISNULL(Codici_Anagrafe.descrizione, '') AS DestinazioneUso_Des, ")
            StrSQL.Append(" ISNULL(Centri_Aziendali.sa_nome, '') AS sa_nome, ISNULL(Campi.Campo_Des, '') AS Campo_Des, ")
            StrSQL.Append(" ISNULL(PEE.Piva,'') AS Piva_old, ISNULL(PEE.Sa_Cod,0) AS Sa_Cod_old, ISNULL(PEE.Campo_Cod,0) AS Campo_Cod_old, ISNULL(PEE.Appezza,0) AS Appezza_old, ISNULL(PEE.Id_Reg,0) AS Id_Reg_old, ISNULL(PEE.progetto_cod,0) AS progetto_cod_old, Programmazione_Entita.N_fabbisogno ")
            StrSQL.Append(" , ISNULL(Codice_Fiscale_Tecnico, '') Codice_Fiscale_Tecnico, Programmazione_Entita.Foral_Cod, Programmazione_Entita.Imp_Cod, ISNULL(Programmazione_Entita.Stato_Ribaltamento,0) AS Stato_Ribaltamento, ISNULL(Programmazione_Entita.IAF,'') as IAF ")
            StrSQL.Append(" , Programmazione_Entita.Riferimento_Alfanumerico_Appezzamento, Programmazione_Entita.Isola, Programmazione_Entita.CapitolatoPrivato, Programmazione_Entita.Finalita_Concimazione_Impianto ")
            StrSQL.Append(" , Programmazione_Entita.Cod_Indirizzo, COALESCE(Programmazione_Entita.Mat_Cod, 0) as Mat_Cod ")

            'INDIRIZZO
            StrSQL.Append(" , ISNULL(Indirizzi.ind_des, '') as ind_des, ISNULL(Indirizzi.frz_des, '') as frz_des, ISNULL(Indirizzi.CAP, '') as CAP, ISNULL(indirizzi.stato, '') as stato_indirizzo ")
            StrSQL.Append(" , ISNULL(indirizzi.note, '') as note_indirizzo, ISNULL(indirizzi.pro_cod_istat, '') as pro_cod_istat_indirizzo, ISNULL(indirizzi.com_cod_istat, '') as com_cod_istat_indirizzo  ")
            StrSQL.Append(" , ISNULL(ISTAT.COMUNI_PROV, '') as pro_cod_indirizzo, ISNULL(ISTAT.LOCALITA, '') as com_des_indirizzo, ISNULL(ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166.Descrizione, '') as stato_indirizzo_des ")

            StrSQL.Append(" , KPIN.Val_Cod as KPIN, Block_Name.Val_Cod as Block_Name ")
            StrSQL.Append(" , ISNULL(Data_Inizio_Portinnesto.Val_Cod, '') as Data_Inizio_Portinnesto ")

            'StrSQL.Append(" ,(SELECT MAX(ImpreseXParticelle.Validita_Inizio)  ")
            'StrSQL.Append(" FROM Programmazione_Testata INNER JOIN ")
            'StrSQL.Append(" ImpreseXParticelle INNER JOIN ")
            'StrSQL.Append(" Programmazione_Particelle ON ImpreseXParticelle.PROV = Programmazione_Particelle.Prov AND ImpreseXParticelle.COM = Programmazione_Particelle.Com AND  ")
            'StrSQL.Append(" ImpreseXParticelle.SEZIONE = Programmazione_Particelle.Sezione AND ImpreseXParticelle.FOGLIO = Programmazione_Particelle.Foglio AND  ")
            'StrSQL.Append(" ImpreseXParticelle.NUMERO = Programmazione_Particelle.Numero AND ImpreseXParticelle.SUBALTERNO = Programmazione_Particelle.Subalterno INNER JOIN ")
            'StrSQL.Append(" Programmazione_Entita PE ON Programmazione_Particelle.Programmazione_Entita_Cod = PE.Programmazione_Entita_Cod ON  ")
            'StrSQL.Append(" Programmazione_Testata.Programmazione_Cod = PE.Programmazione_Cod And Programmazione_Testata.Piva = ImpreseXParticelle.PIVA ")
            'StrSQL.Append(" WHERE PE.Programmazione_Entita_Cod = Programmazione_Entita.Programmazione_Entita_Cod  ")
            'StrSQL.Append(" AND PE.Validita_Fine >= ImpreseXParticelle.Validita_Inizio AND PE.Validita_Inizio <= ImpreseXParticelle.Validita_Fine ")
            'StrSQL.Append(" ) AS Inizio_Possesso ")

            'StrSQL.Append(" ,(SELECT MIN(ImpreseXParticelle.Validita_Fine)  ")
            'StrSQL.Append(" FROM Programmazione_Testata INNER JOIN ")
            'StrSQL.Append(" ImpreseXParticelle INNER JOIN ")
            'StrSQL.Append(" Programmazione_Particelle ON ImpreseXParticelle.PROV = Programmazione_Particelle.Prov AND ImpreseXParticelle.COM = Programmazione_Particelle.Com AND  ")
            'StrSQL.Append(" ImpreseXParticelle.SEZIONE = Programmazione_Particelle.Sezione AND ImpreseXParticelle.FOGLIO = Programmazione_Particelle.Foglio AND  ")
            'StrSQL.Append(" ImpreseXParticelle.NUMERO = Programmazione_Particelle.Numero AND ImpreseXParticelle.SUBALTERNO = Programmazione_Particelle.Subalterno INNER JOIN ")
            'StrSQL.Append(" Programmazione_Entita PE ON Programmazione_Particelle.Programmazione_Entita_Cod = PE.Programmazione_Entita_Cod ON  ")
            'StrSQL.Append(" Programmazione_Testata.Programmazione_Cod = PE.Programmazione_Cod And Programmazione_Testata.Piva = ImpreseXParticelle.PIVA ")
            'StrSQL.Append(" WHERE PE.Programmazione_Entita_Cod = Programmazione_Entita.Programmazione_Entita_Cod ")
            'StrSQL.Append(" AND PE.Validita_Fine >= ImpreseXParticelle.Validita_Inizio AND PE.Validita_Inizio <= ImpreseXParticelle.Validita_Fine ")
            'StrSQL.Append("  ) AS Fine_Possesso ")

            StrSQL.Append(" FROM Programmazione_Entita  ")
            StrSQL.Append(" LEFT JOIN Programmazione_Entita_Codici praticacod ON Programmazione_Entita.Programmazione_Entita_Cod = praticacod.Programmazione_Entita_Cod AND praticacod.id_cod = " & CStr(enum_CodiciAnagrafe.Programmazione_Impianto_Pratica_Cod) & " ")
            StrSQL.Append(" LEFT OUTER JOIN Centri_Aziendali ON Programmazione_Entita.Sa_Cod = Centri_Aziendali.sa_cod AND Programmazione_Entita.Piva = Centri_Aziendali.PIVA ")
            StrSQL.Append(" LEFT OUTER JOIN Campi ON Programmazione_Entita.Sa_Cod = Campi.Sa_Cod AND Programmazione_Entita.Piva = Campi.Piva AND Programmazione_Entita.Campo_Cod = Campi.Campo_Cod ")
            StrSQL.Append(" LEFT OUTER JOIN Codici_Anagrafe ON Programmazione_Entita.Id_Cod = Codici_Anagrafe.codice ")
            StrSQL.Append(" LEFT OUTER JOIN GruppoVarietale ON Programmazione_Entita.Grva_Cod = GruppoVarietale.Grva_Cod ")
            StrSQL.Append(" LEFT OUTER JOIN GruppoFinalita ON Programmazione_Entita.Grfi_Cod = GruppoFinalita.Grfi_Cod ")
            StrSQL.Append(" LEFT OUTER JOIN SpecieVegetali ON Programmazione_Entita.Veg_Cod = SpecieVegetali.Veg_Cod ")
            StrSQL.Append(" LEFT OUTER JOIN Cultivar ON Programmazione_Entita.Cul_Cod = Cultivar.Cul_Cod ")

            StrSQL.Append(" LEFT OUTER JOIN Programmazione_Entita_Eliminate AS PEE ON Programmazione_Entita.Piva_SuperUser = PEE.Piva_SuperUser AND  ")
            StrSQL.Append(" Programmazione_Entita.Programmazione_Cod = PEE.Programmazione_Cod AND ")
            StrSQL.Append(" Programmazione_Entita.Programmazione_Entita_Cod = PEE.Programmazione_Entita_Cod ")

            'INDIRIZZO
            StrSQL.Append(" LEFT JOIN Indirizzi on INDIRIZZI.Cod_Indirizzo = Programmazione_Entita.Cod_Indirizzo ")
            StrSQL.Append(" LEFT JOIN ISTAT on INDIRIZZI.pro_cod_istat = ISTAT.PROV AND INDIRIZZI.com_cod_istat = ISTAT.COM ")
            StrSQL.Append(" LEFT JOIN ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166 on INDIRIZZI.stato = ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166.Codice ")

            StrSQL.Append(" LEFT JOIN Programmazione_Entita_Codici KPIN on KPIN.Programmazione_Entita_Cod = Programmazione_Entita.Programmazione_Entita_Cod AND KPIN.id_cod = " & enum_CodiciAnagrafe.Zespri_Codice_kPIN & " ")
            StrSQL.Append(" LEFT JOIN Programmazione_Entita_Codici Block_Name on Block_Name.Programmazione_Entita_Cod = Programmazione_Entita.Programmazione_Entita_Cod AND Block_Name.id_cod = " & enum_CodiciAnagrafe.Zespri_Block_Name & " ")
            StrSQL.Append(" LEFT JOIN Programmazione_Entita_Codici Data_Inizio_Portinnesto on Data_Inizio_Portinnesto.Programmazione_Entita_Cod = Programmazione_Entita.Programmazione_Entita_Cod AND Data_Inizio_Portinnesto.id_cod = " & enum_CodiciAnagrafe.Data_Inizio_Portinnesto & " ")

            StrSQL.Append(" WHERE   (Programmazione_Entita.Piva_SuperUser = '" & Agro_SQL_SaveText(Piva_SuperUser) & "')   ")
            StrSQL.Append(" AND     (Programmazione_Entita.Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod.ToString) & ")  ")

            '----------------------------------------------------
            '--- Recupero il datatable --------------------------
            '----------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, DescrizioneFunzione)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            ErrMSG = ex.Message
            Scrivi_LOG(objParametri, DescrizioneFunzione, ErrMSG)
            DT = Nothing
            Throw New Exception("[" & DescrizioneFunzione & "] : " & ErrMSG)
        End Try
        Return DT

    End Function

    Public Function Anagrafica_Impianto_Dettagli_Leggi(ByVal Piva As String,
                                                   ByVal SaCod As Integer,
                                                   ByVal Appezza As Integer,
                                                   ByVal IdReg As Integer,
                                                   ByRef ErrMSG As String,
                                                   ByRef objParametri As AgronicaCoreParametri) _
                                                   As DataTable

        '----- Descrizione
        Dim DescrizioneFunzione As String = "Appezzamento_Dettagli : Lettura"

        Dim DT As New DataTable

        Dim StrSQL As New Text.StringBuilder
        StrSQL.Length = 0

        Try
            StrSQL.Append(" SELECT  Appezzamento.APP_NOME, Appezzamento.SUP_APP, SpecieVegetali.Veg_Des, Cultivar.Cul_Des,  ")
            StrSQL.Append(" GruppoFinalita.Grfi_Des, Imprese_Progetti.Progetto_Nome ")
            StrSQL.Append(" FROM Reg_Impianti ")
            StrSQL.Append(" INNER JOIN Appezzamento ON Reg_Impianti.PIVA = Appezzamento.PIVA AND Reg_Impianti.SA_COD = Appezzamento.SA_COD AND  ")
            StrSQL.Append(" Reg_Impianti.APPEZZA = Appezzamento.APPEZZA ")
            StrSQL.Append(" INNER JOIN Imprese_Progetti ON Reg_Impianti.PIVA = Imprese_Progetti.Piva AND Reg_Impianti.SA_COD = Imprese_Progetti.Sa_Cod AND  ")
            StrSQL.Append(" Reg_Impianti.APPEZZA = Imprese_Progetti.Appezza AND Reg_Impianti.ID_REG = Imprese_Progetti.Id_Reg ")
            StrSQL.Append(" INNER JOIN Cultivar ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod ")
            StrSQL.Append(" INNER JOIN SpecieVegetali ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod ")
            StrSQL.Append(" INNER JOIN GruppoFinalita ON Reg_Impianti.GRFI_COD = GruppoFinalita.Grfi_Cod ")

            StrSQL.Append(" WHERE Reg_Impianti.PIVA  ='" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" AND Reg_Impianti.SA_COD  =" & Agro_SQL_SaveNum(SaCod.ToString) & " ")
            StrSQL.Append(" AND Reg_Impianti.APPEZZA =" & Agro_SQL_SaveNum(Appezza.ToString) & " ")
            StrSQL.Append(" AND Reg_Impianti.ID_REG  =" & Agro_SQL_SaveNum(IdReg.ToString) & " ")

            '----------------------------------------------------
            '--- Recupero il datatable --------------------------
            '----------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, DescrizioneFunzione)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            ErrMSG = ex.Message
            Scrivi_LOG(objParametri, DescrizioneFunzione, ErrMSG)
            DT = Nothing
            Throw New Exception("[" & DescrizioneFunzione & "] : " & ErrMSG)
        End Try
        Return DT

    End Function


    Public Function Leggi_Descrizione_e_colore_Stato_SQNPI(ByVal Stato_SQNPI As Integer,
                                                   ByRef objParametri As AgronicaCoreParametri) _
                                                   As DataTable

        '----- Descrizione
        Dim DescrizioneFunzione As String = "Leggi_Descrizione_Stato_SQNPI : Lettura"
        Dim WAnagraficaStati_Cod = Stato_SQNPI

        Dim ErrMSG As String
        Dim DT As DataTable
        Dim StrSQL As New Text.StringBuilder

        StrSQL.Length = 0

        Try
            StrSQL.Append(" SELECT TOP 1  WAnagraficaStati_Des,Colore")
            StrSQL.Append(" FROM WAnagraficaStati ")

            StrSQL.Append(" WHERE WAnagraficaStati.WAnagraficaStati_Cod  = " & Agro_SQL_SaveNum(WAnagraficaStati_Cod) & " ")

            '----------------------------------------------------
            '--- Recupero il datatable --------------------------
            '----------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, DescrizioneFunzione)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            ErrMSG = ex.Message
            Scrivi_LOG(objParametri, DescrizioneFunzione, ErrMSG)
            DT = Nothing
            Throw New Exception("[" & DescrizioneFunzione & "] : " & ErrMSG)
        End Try

        Return DT

    End Function

#End Region

#Region "LettureJSONxRibaltamento"
    Public Function LeggiAppezzamenti_xRibaltamento_JSON(Qs_Piva As String, ByVal Qs_ProgrammazioneCod As String,
                                                      Txt_ValiditaInizio As String, Txt_ValiditaFine As String,
                                                      Chk_ControlloDate As Boolean, Cmb_Regolamento As String,
                                                      objParametri_Server As AgronicaCoreParametri,
                                                         Optional ByVal ControllaSeMovimentati As Boolean = False) As RispostaStandard

        Dim r As New RispostaStandard

        Try

            Dim DT_Appezzamenti As New DataTable
            Dim strErr As String = ""

            Dim objP As New AgronicaCoreAnagrafeBIZ.Programmazione_R
            DT_Appezzamenti = objP.Anagrafica_AppezzamentixRibaltamento_Leggi(CInt(Qs_ProgrammazioneCod),
                                                                              objParametri_Server.PivaSuperUser,
                                                                              strErr,
                                                                              objParametri_Server)

            Dim DtPP As New DataTable
            Dim DrPP As DataRow()
            Dim objPP As New AgronicaCoreAnagrafeDAL.Programmazione_Particelle_R
            DtPP = objPP.LeggiParticelle_Da_Programmazione("", 0,
                                                           CInt(Qs_ProgrammazioneCod), 0,
                                                           enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                           "", "", objParametri_Server)

            ' aggiungo i campi
            DT_Appezzamenti.Columns.Add(New DataColumn("SupApp_Formattata", GetType(String)))
            DT_Appezzamenti.Columns.Add(New DataColumn("Resa_Formattata", GetType(String)))
            DT_Appezzamenti.Columns.Add(New DataColumn("Validita", GetType(String)))
            DT_Appezzamenti.Columns.Add(New DataColumn("Validita_Originale", GetType(String)))
            DT_Appezzamenti.Columns.Add(New DataColumn("Operazione", GetType(String)))

            DT_Appezzamenti.Columns.Add(New DataColumn("Ribaltato", GetType(Integer)))
            DT_Appezzamenti.Columns.Add(New DataColumn("Movimentato", GetType(Integer)))

            DT_Appezzamenti.Columns.Add(New DataColumn("DateModificate", GetType(Integer)))

            DT_Appezzamenti.Columns.Add(New DataColumn("Catasto", GetType(String)))

            DT_Appezzamenti.Columns.Add(New DataColumn("Fase_Ciclo_Colturale_ID", GetType(String)))
            DT_Appezzamenti.Columns.Add(New DataColumn("Fase_Ciclo_Colturale_Des", GetType(String)))
            DT_Appezzamenti.Columns.Add(New DataColumn("LimiteMaxN", GetType(String)))

            ' rendo le colonne modificabili
            DT_Appezzamenti.Columns("Grva_Cod").ReadOnly = False    ' non so perché, non dovrebbe dare problemi, ci accedo in lettura quando costruisco l'xml
            DT_Appezzamenti.Columns("Veg_Des").ReadOnly = False
            DT_Appezzamenti.Columns("Sup_App").ReadOnly = False
            DT_Appezzamenti.Columns("Veg_Des").ReadOnly = False
            DT_Appezzamenti.Columns("Cul_Des").ReadOnly = False
            DT_Appezzamenti.Columns("Grfi_Des").ReadOnly = False
            DT_Appezzamenti.Columns("Progetto_Nome").ReadOnly = False
            DT_Appezzamenti.Columns("DestinazioneUso_Des").ReadOnly = False

            'Leggo se le entità sono già state ribaltate
            Dim Programmazione_Entita_Cod As Integer
            Dim objERib As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Programmazioni_R
            Dim DtERib As New DataTable
            Dim DrERib As DataRow()
            DtERib = objERib.Leggi("", 0, 0, 0, 0, CInt(Qs_ProgrammazioneCod), 0, "", "", objParametri_Server)

            Dim DtEMov As New DataTable
            Dim DrEMov As DataRow()
            If ControllaSeMovimentati Then
                DtEMov = objERib.Leggi_OperazioniImpiantiRibaltati_DaProgrammazione("", CInt(Qs_ProgrammazioneCod), 0, "", "", objParametri_Server)
            End If


            Dim i, p, j As Integer

            Dim Piva_old As String = ""
            Dim SaCod_old As Integer = 0
            Dim Appezza_old As Integer = 0
            Dim IdReg_old As Integer = 0

            Dim strOperazione As String = ""
            ' Dim PossessoInizio As String
            ' Dim PossessoFine As String
            Dim Inizio As String
            Dim Fine As String

            Dim EsisteInizio As Boolean
            Dim EsisteFine As Boolean
            Dim Catasto As String

            ' ciclo sulle righe e cancello i doppioni (caso delle unioni)
            For i = 0 To DT_Appezzamenti.Rows.Count - 1

                '(8/11/2017 fede) ribalto solo ciò che avevo selezionato
                'If DT_Appezzamenti.Rows(i).Item("Stato_Ribaltamento") = enum_Programmazione_Entita_Stato_Ribaltamento.DaRibaltare Then

                Catasto = ""

                DT_Appezzamenti.Rows(i).Item("DateModificate") = 0
                Inizio = CDate(DT_Appezzamenti.Rows(i).Item("Validita_Inizio")).ToShortDateString
                Fine = CDate(DT_Appezzamenti.Rows(i).Item("Validita_Fine")).ToShortDateString

                'Inizio = Txt_ValiditaInizio
                'Fine = Txt_ValiditaFine

                DT_Appezzamenti.Rows(i).Item("Validita_Originale") = Inizio & " - " & Fine

                If DtPP.Rows.Count > 0 Then
                    DrPP = DtPP.Select("Programmazione_Entita_Cod=" & DT_Appezzamenti.Rows(i).Item("Programmazione_Entita_Cod"))

                    If DrPP IsNot Nothing Then
                        For p = 0 To DrPP.Length - 1

                            Catasto &= IIf(p <> 0, "<BR>", "") &
                           DrPP(p).Item("prov") & "_" &
                           DrPP(p).Item("com") & "_" &
                           DrPP(p).Item("sezione") & "_" &
                           DrPP(p).Item("foglio").ToString & "_" &
                           DrPP(p).Item("numero").ToString & "_" &
                           DrPP(p).Item("subalterno") &
                           " (Sup." & CStr(DrPP(p).Item("Superficie")) & " Ha)"

                            If Chk_ControlloDate Then
                                EsisteInizio = False
                                EsisteFine = False
                                Dim DtPartImp As New DataTable
                                Dim objImprxPart As New AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R
                                DtPartImp = objImprxPart.LeggixChiave(0,
                                                                      Qs_Piva,
                                                                      0,
                                                                      DrPP(p).Item("prov"),
                                                                      DrPP(p).Item("com"),
                                                                      DrPP(p).Item("sezione"),
                                                                      DrPP(p).Item("foglio"),
                                                                      DrPP(p).Item("numero"),
                                                                      DrPP(p).Item("subalterno"),
                                                                      enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                      "", "", objParametri_Server)
                                For j = 0 To DtPartImp.Rows.Count - 1
                                    If Inizio >= DtPartImp.Rows(j).Item("validita_inizio") And
                                       Inizio <= DtPartImp.Rows(j).Item("validita_fine") Then
                                        EsisteInizio = True
                                        Exit For
                                    End If
                                Next

                                'se l'inizio della particella non esiste alla data scelta
                                'prendo l'inizio del possesso successivo a tale data
                                If Not EsisteInizio Then
                                    Dim DrPartImp As DataRow() = DtPartImp.Select("validita_inizio>='" & Inizio & "'")
                                    If DrPartImp IsNot Nothing AndAlso DrPartImp.Length > 0 Then
                                        Inizio = DrPartImp(0).Item("validita_inizio")
                                        DT_Appezzamenti.Rows(i).Item("DateModificate") = 1
                                    End If
                                End If

                                For j = 0 To DtPartImp.Rows.Count - 1
                                    If Fine <= DtPartImp.Rows(j).Item("validita_fine") And
                                       Fine >= DtPartImp.Rows(j).Item("validita_inizio") Then
                                        EsisteFine = True
                                        Exit For
                                    End If
                                Next

                                'se la fine particella non esiste alla data scelta
                                'prendo la fine del possesso precedente a tale data
                                If Not EsisteFine Then
                                    Dim DrPartImp As DataRow() = DtPartImp.Select("validita_fine<='" & Fine & "'")
                                    If DrPartImp IsNot Nothing AndAlso DrPartImp.Length > 0 Then
                                        Fine = DrPartImp(0).Item("validita_fine")
                                        DT_Appezzamenti.Rows(i).Item("DateModificate") = 1
                                    End If
                                End If

                            End If


                        Next
                    End If
                End If
                'End If
                DT_Appezzamenti.Rows(i).Item("Catasto") = Catasto

                'If Chk_ControlloDate.Checked Then
                '    If Not IsDBNull(DT_Appezzamenti.Rows(i).Item("Inizio_Possesso")) Then
                '        If CDate(DT_Appezzamenti.Rows(i).Item("Inizio_Possesso")) > CDate(Inizio) Then
                '            Inizio = CDate(DT_Appezzamenti.Rows(i).Item("Inizio_Possesso")).ToShortDateString
                '            DT_Appezzamenti.Rows(i).Item("DateModificate") = 1
                '        End If
                '    End If
                '    If Not IsDBNull(DT_Appezzamenti.Rows(i).Item("Fine_Possesso")) Then
                '        If CDate(DT_Appezzamenti.Rows(i).Item("Fine_Possesso")) < CDate(Fine) Then
                '            Fine = CDate(DT_Appezzamenti.Rows(i).Item("Fine_Possesso")).ToShortDateString
                '            DT_Appezzamenti.Rows(i).Item("DateModificate") = 1
                '        End If
                '    End If
                'End If

                DT_Appezzamenti.Rows(i).Item("Validita") = Inizio & " - " & Fine

                If IsDate(Inizio) Then
                    DT_Appezzamenti.Rows(i).Item("Validita_Inizio") = Inizio
                End If

                If IsDate(Fine) Then
                    DT_Appezzamenti.Rows(i).Item("Validita_Fine") = Fine
                End If


                If DT_Appezzamenti.Rows(i).Item("Validita_Inizio_Impianto") <> "01/01/1900" Then
                    DT_Appezzamenti.Rows(i).Item("Validita_Inizio_Impianto") = CDate(DT_Appezzamenti.Rows(i).Item("Validita_Inizio_Impianto")).ToShortDateString
                Else
                    DT_Appezzamenti.Rows(i).Item("Validita_Inizio_Impianto") = ""
                End If


                strOperazione = ""
                If Not IsDBNull(DT_Appezzamenti.Rows(i).Item("operazione_cod")) Then
                    strOperazione = objP.OperazioneDes_From_OperazioneCod(DT_Appezzamenti.Rows(i).Item("operazione_cod"))
                End If
                DT_Appezzamenti.Rows(i).Item("Operazione") = strOperazione

                DT_Appezzamenti.Rows(i).Item("Ribaltato") = 0
                DT_Appezzamenti.Rows(i).Item("Movimentato") = 0

                If Piva_old = DT_Appezzamenti.Rows(i).Item("Piva") And
                   SaCod_old = DT_Appezzamenti.Rows(i).Item("Sa_Cod") And
                   Appezza_old = DT_Appezzamenti.Rows(i).Item("Appezza") And
                   IdReg_old = DT_Appezzamenti.Rows(i).Item("Id_Reg") Then

                    'unione
                    DT_Appezzamenti.Rows(i).Delete()

                    'DT_Appezzamenti.Rows(i).Item("App_nome") = ""
                    'DT_Appezzamenti.Rows(i).Item("Sup_App") = DBNull.Value
                    'DT_Appezzamenti.Rows(i).Item("Veg_Des") = ""
                    'DT_Appezzamenti.Rows(i).Item("Cul_Des") = ""
                    'DT_Appezzamenti.Rows(i).Item("Grfi_Des") = ""
                    'DT_Appezzamenti.Rows(i).Item("Progetto_Nome") = ""
                    'DT_Appezzamenti.Rows(i).Item("Validita") = ""

                Else

                    Piva_old = DT_Appezzamenti.Rows(i).Item("Piva")
                    SaCod_old = DT_Appezzamenti.Rows(i).Item("Sa_Cod")
                    Appezza_old = DT_Appezzamenti.Rows(i).Item("Appezza")
                    IdReg_old = DT_Appezzamenti.Rows(i).Item("Id_Reg")

                    DT_Appezzamenti.Rows(i).Item("SupApp_Formattata") = Format(DT_Appezzamenti.Rows(i).Item("Sup_App"), "0.0000")
                    DT_Appezzamenti.Rows(i).Item("Resa_Formattata") = Format(DT_Appezzamenti.Rows(i).Item("Resa"), "0.00")

                    'verifico se l'appezzamento è già stato ribaltato ed eventualmente l'impianto ribaltato anche movimentato ...
                    Programmazione_Entita_Cod = DT_Appezzamenti.Rows(i).Item("Programmazione_Entita_Cod")

                    If Programmazione_Entita_Cod <> 0 Then
                        DrERib = DtERib.Select("Programmazione_Entita_Cod=" & Programmazione_Entita_Cod.ToString)
                        If DrERib IsNot Nothing AndAlso DrERib.Length > 0 Then
                            DT_Appezzamenti.Rows(i).Item("Ribaltato") = 1
                        End If
                        If ControllaSeMovimentati Then
                            DrEMov = DtEMov.Select("Programmazione_Entita_Cod=" & Programmazione_Entita_Cod.ToString)
                            If DrEMov IsNot Nothing AndAlso DrEMov.Length > 0 Then
                                DT_Appezzamenti.Rows(i).Item("Movimentato") = 1
                            End If
                        End If
                    End If

                End If

                'End If

            Next

            DT_Appezzamenti.AcceptChanges()

            r.RispostaOK = True
            r.RispostaStringa = JSON_DatatableAppezzamenti_Tabella(DT_Appezzamenti)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function

    Public Function LeggiAppezzamentiEliminati_xRibaltamento_JSON(Qs_Piva As String, ByVal Qs_ProgrammazioneCod As String,
                                                                     Txt_ValiditaInizio As String, Txt_ValiditaFine As String,
                                                                     Chk_ControlloDate As Boolean, Cmb_Regolamento As String,
                                                              objParametri_Server As AgronicaCoreParametri) As RispostaStandard
        Dim r As New RispostaStandard

        Try
            Dim DT_Appezzamenti_Eliminati As New DataTable
            Dim Dt_Entita_Eliminate As New DataTable
            Dim i As Integer
            Dim strErr As String = ""

            Dim objP As New AgronicaCoreAnagrafeBIZ.Programmazione_R
            objP.DT_Appezzamenti_Eliminati_Crea(DT_Appezzamenti_Eliminati)

            Dim objProgrammazione_Entita_Eliminate_R As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_Eliminate_R

            Dt_Entita_Eliminate = objProgrammazione_Entita_Eliminate_R.Leggi(
                                        CInt(Qs_ProgrammazioneCod),
                                        strErr,
                                        0,
                                        Estremo_Validita_Inizio,
                                        Estremo_Validita_Fine,
                                        enumSelezioneVariabile.Selezione_TabellaCompleta,
                                        "",
                                        "",
                                        objParametri_Server)

            If Dt_Entita_Eliminate.Rows.Count > 0 Then

                'Dim Operazione As String
                Dim Sa_Nome As String
                Dim Campo_Des As String
                Dim App_nome As String
                Dim Sup_App As String
                Dim Veg_Des As String
                Dim Cul_Des As String
                Dim Grfi_Des As String
                Dim Progetto_Nome As String
                Dim DestinazioneUso_Des As String

                '-----------------------------------
                'recupero i dati degli impianti
                Dim Pive As String()
                Dim SaCod As Integer()
                Dim Appezza As Integer()
                Dim IdReg As Integer()
                Dim N_Impianti As Integer = 0
                For i = 0 To Dt_Entita_Eliminate.Rows.Count - 1
                    ReDim Preserve Pive(N_Impianti)
                    ReDim Preserve SaCod(N_Impianti)
                    ReDim Preserve Appezza(N_Impianti)
                    ReDim Preserve IdReg(N_Impianti)
                    Pive(N_Impianti) = Dt_Entita_Eliminate.Rows(i).Item("Piva")
                    SaCod(N_Impianti) = Dt_Entita_Eliminate.Rows(i).Item("Sa_Cod")
                    Appezza(N_Impianti) = Dt_Entita_Eliminate.Rows(i).Item("Appezza")
                    IdReg(N_Impianti) = Dt_Entita_Eliminate.Rows(i).Item("Id_Reg")
                    N_Impianti += 1
                Next

                Dim objImpianto As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                Dim DtImpianti As DataTable
                Dim DrImpianto As DataRow()
                DtImpianti = objImpianto.Leggi_Dati_Impianti_Distinte(Pive, SaCod, Appezza, IdReg, "", " Imprese_Progetti.Validita_Fine desc ", objParametri_Server)


                '------------------------------------------------
                'riempio il dt_entita_eliminate
                For i = 0 To Dt_Entita_Eliminate.Rows.Count - 1

                    'Select Case Dt_Entita_Eliminate.Rows(i).Item("Progetto_Cod")
                    '    Case Is <> 0
                    '        Operazione = "Esercizio chiuso"
                    '    Case Else
                    '        Select Case Dt_Entita_Eliminate.Rows(i).Item("Id_Reg")
                    '            Case Is <> 0
                    '                Operazione = "Impianto chiuso"
                    '            Case Else
                    '                Select Case Dt_Entita_Eliminate.Rows(i).Item("Appezza")
                    '                    Case Is <> 0
                    '                        Operazione = "Appezzamento chiuso"
                    '                End Select
                    '        End Select
                    'End Select

                    Sa_Nome = ""
                    Campo_Des = ""
                    App_nome = ""
                    Sup_App = ""
                    Veg_Des = ""
                    Cul_Des = ""
                    Grfi_Des = ""
                    Progetto_Nome = ""
                    DestinazioneUso_Des = ""

                    If DtImpianti IsNot Nothing AndAlso DtImpianti.Rows.Count > 0 Then
                        If Dt_Entita_Eliminate.Rows(i).Item("id_reg") <> 0 Then
                            DrImpianto = DtImpianti.Select("Piva='" & Dt_Entita_Eliminate.Rows(i).Item("Piva").ToString & "' " &
                                                           " AND sa_cod=" & Dt_Entita_Eliminate.Rows(i).Item("Sa_Cod").ToString &
                                                           " AND appezza=" & Dt_Entita_Eliminate.Rows(i).Item("appezza").ToString &
                                                           " AND id_reg=" & Dt_Entita_Eliminate.Rows(i).Item("id_reg").ToString)
                        Else
                            DrImpianto = DtImpianti.Select("Piva='" & Dt_Entita_Eliminate.Rows(i).Item("Piva").ToString & "' " &
                                                           " AND sa_cod=" & Dt_Entita_Eliminate.Rows(i).Item("Sa_Cod").ToString &
                                                           " AND appezza=" & Dt_Entita_Eliminate.Rows(i).Item("appezza").ToString)
                        End If
                        If DrImpianto IsNot Nothing AndAlso DrImpianto.Length > 0 Then
                            Sa_Nome = DrImpianto(0).Item("Sa_Nome")
                            Campo_Des = DrImpianto(0).Item("Campo_Des")
                            App_nome = DrImpianto(0).Item("App_nome")
                            Sup_App = DrImpianto(0).Item("Sup_Imp")
                            Veg_Des = DrImpianto(0).Item("Veg_Des")
                            Cul_Des = DrImpianto(0).Item("Cul_Des")
                            Grfi_Des = DrImpianto(0).Item("Grfi_Des")
                            Progetto_Nome = DrImpianto(0).Item("Progetto_Nome")
                            DestinazioneUso_Des = DrImpianto(0).Item("DestinazioneUso")
                        End If
                    End If

                    objP.DT_Appezzamenti_Eliminati_Insert(DT_Appezzamenti_Eliminati,
                                                    Dt_Entita_Eliminate.Rows(i).Item("Piva"),
                                                    Dt_Entita_Eliminate.Rows(i).Item("Sa_Cod"),
                                                    Dt_Entita_Eliminate.Rows(i).Item("Campo_Cod"),
                                                    Dt_Entita_Eliminate.Rows(i).Item("Appezza"),
                                                    Dt_Entita_Eliminate.Rows(i).Item("Id_reg"),
                                                    Dt_Entita_Eliminate.Rows(i).Item("Progetto_Cod"),
                                                    Dt_Entita_Eliminate.Rows(i).Item("Programmazione_Entita_Cod"),
                                                     Dt_Entita_Eliminate.Rows(i).Item("Operazione_Cod"),
                                                   Dt_Entita_Eliminate.Rows(i).Item("validita_inizio"),
                                                    "",
                                                    "",
                                                    objP.OperazioneDes_From_OperazioneCod(Dt_Entita_Eliminate.Rows(i).Item("Operazione_Cod")),
                                                    Sa_Nome,
                                                    Campo_Des,
                                                    App_nome,
                                                    Sup_App,
                                                    Veg_Des,
                                                    Cul_Des,
                                                    Grfi_Des,
                                                    Progetto_Nome,
                                                    DestinazioneUso_Des)
                Next

            End If

            r.RispostaOK = True
            r.RispostaStringa = JSON_DatatableAppezzamentiEliminati_Tabella(DT_Appezzamenti_Eliminati)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    Private Shared Function JSON_DatatableAppezzamenti_Tabella(ByRef DT_Appezzamenti As DataTable) As String

        Dim l As New List(Of ColonneNome)
        Dim c As ColonneNome

        c = New ColonneNome("Piva_SuperUser", "Piva_SuperUser", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Programmazione_Cod", "Programmazione_Cod", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Programmazione_Entita_Cod", "Programmazione_Entita_Cod", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Operazione_Cod", "Operazione_Cod", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Piva", "Piva", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Sa_Cod", "Sa_Cod", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Campo_Cod", "Campo_Cod", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Appezza", "Appezza", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Id_Reg", "Id_Reg", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Progetto_Cod", "Progetto_Cod", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Progetto_Nome", "Progetto_Nome", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Id_Cod", "Id_Cod", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Veg_Cod", "Veg_Cod", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Cul_Cod", "Cul_Cod", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Grfi_Cod", "Grfi_Cod", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Cop_Cod", "Cop_Cod", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Resa", "Resa", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("TipoZona", "TipoZona", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Veg_Cod_Prec", "Veg_Cod_Prec", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Id_Mat_O", "Id_Mat_O", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Id_Fre", "Id_Fre", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("N_Distribuito", "N_Distribuito", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Validita_Inizio", "Validita_Inizio", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Validita_Fine", "Validita_Fine", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Num_Piante", "Num_Piante", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Veg_Cod_Cliente", "Veg_Cod_Cliente", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Cul_Cod_Cliente", "Cul_Cod_Cliente", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Grva_Cod", "Grva_Cod", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Gru_Cod", "Gru_Cod", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Grva_Des", "Grva_Des", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Campo_Des", "Campo_Des", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Piva_old", "Piva_old", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Sa_Cod_old", "Sa_Cod_old", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Campo_Cod_old", "Campo_Cod_old", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Appezza_old", "Appezza_old", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Id_Reg_old", "Id_Reg_old", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Progetto_cod_old", "Progetto_cod_old", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("N_fabbisogno", "N_fabbisogno", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Codice_Fiscale_Tecnico", "Codice_Fiscale_Tecnico", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("SupApp_Formattata", "SupApp_Formattata", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Operazione", "Operazione", "string")
        'c._hidden = True
        c._width = "107.27px"
        l.Add(c)

        c = New ColonneNome("Sa_Nome", "Centro", "string")
        'c._hidden = True
        l.Add(c)

        c = New ColonneNome("App_Nome", "App.", "string")
        'c._hidden = True
        c._width = "66.36px"
        l.Add(c)

        c = New ColonneNome("Sup_App", "Sup. [Ha]", "string")
        'c._hidden = True
        c._width = "67.27px"
        l.Add(c)

        c = New ColonneNome("Veg_Des", "Specie", "string")
        'c._hidden = True
        l.Add(c)

        c = New ColonneNome("Cul_Des", "Varietà", "string")
        'c._hidden = True
        l.Add(c)

        c = New ColonneNome("Grfi_Des", "Finalità", "string")
        'c._hidden = True
        c._width = "162.73px"
        l.Add(c)

        c = New ColonneNome("DestinazioneUso_Des", "Destinazione D'Uso", "string")
        'c._hidden = True
        c._width = "130px"
        l.Add(c)

        c = New ColonneNome("Validita_Inizio_Impianto", "Inizio Impianto", "string")
        'c._hidden = True
        c._width = "94.55px"
        l.Add(c)

        c = New ColonneNome("Validita", "Validità Aggiornata con Controllo dei Possessi", "string")
        'c._hidden = True
        c._width = "157.25px"
        l.Add(c)

        c = New ColonneNome("Validita_Originale", "Validità Appezzamento (Piano Colturale)", "string")
        'c._hidden = True
        c._width = "142.73px"
        l.Add(c)

        c = New ColonneNome("LimiteMaxN", "Limite Max N", "string")
        'c._hidden = True
        c._Editabile = True
        c._width = "78.18px"
        l.Add(c)

        c = New ColonneNome("Resa_Formattata", "Resa Prevista", "string")
        'c._hidden = True
        c._width = "90px"
        l.Add(c)

        c = New ColonneNome("Fase_Ciclo_Colturale_ID", "Fase_Ciclo_Colturale_ID", "number")
        c._hidden = True
        c._Editabile = True
        l.Add(c)

        c = New ColonneNome("Fase_Ciclo_Colturale_Des", "Fase del Ciclo Colturale", "string")
        c._hidden = True
        c._Editabile = True
        l.Add(c)

        c = New ColonneNome("Unita_Vitata", "Unita Vitata", "string")
        'c._hidden = True
        c._width = "81px"
        l.Add(c)

        c = New ColonneNome("Catasto", "Catasto", "string")
        'c._hidden = True
        c._width = "156px"
        l.Add(c)

        c = New ColonneNome("Ribaltato", "Ribaltato", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("DateModificate", "DateModificate", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Foral_Cod", "Foral_Cod", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Imp_Cod", "Imp_Cod", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("TRA_Fila", "TRA_Fila", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("SU_Fila", "SU_Fila", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("MetodoProduzione_Cod", "MetodoProduzione_Cod", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Disciplinare_Cod", "Disciplinare_Cod", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Regolamento_Cod", "Regolamento_Cod", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Regolamento_Concimazione_Cod", "Regolamento_Concimazione_Cod", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Flag_PubblicoPrivato", "Flag_PubblicoPrivato", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("id_tr", "id_tr", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Stato_Cod", "Stato_Cod", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Limite_N", "Limite_N", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Limite_P", "Limite_P", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Limite_K", "Limite_K", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Data_Semina", "Data_Semina", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Data_Raccolta", "Data_Raccolta", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Data_Fioritura_Prevista", "Data_Fioritura_Prevista", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Veg_Cod_Prec2", "Veg_Cod_Prec2", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Veg_Cod_Prec3", "Veg_Cod_Prec3", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Veg_Cod_Prec4", "Veg_Cod_Prec4", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Piano_Semina", "Piano_Semina", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Codice_Contratto", "Codice_Contratto", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("stato_ribaltamento", "stato_ribaltamento", "number")
        'c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("IAF", "IAF", "string")
        'c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Pratica_Cod", "Pratica_Cod", "string")
        'c._Editabile = True
        c._hidden = True
        l.Add(c)

        'c = New ColonneNome("provenienza_fascicolo", "provenienza_fascicolo", "String")
        ''c._Editabile = True
        'c._hidden = True
        'l.Add(c)
        c = New ColonneNome("DistBZ_CorpiIdrici", "DistBZ_CorpiIdrici", "number")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("DistBZ_AreeResPub", "DistBZ_AreeResPub", "number")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("DistBZ_Allevamenti", "DistBZ_Allevamenti", "number")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("DistBZ_VegNatNonColt", "DistBZ_VegNatNonColt", "number")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("SupBZ_Riduzione", "SupBZ_Riduzione", "number")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Riferimento_Alfanumerico_Appezzamento", "Riferimento_Alfanumerico_Appezzamento", "string")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Isola", "Isola", "string")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("CapitolatoPrivato", "CapitolatoPrivato", "string")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Finalita_Concimazione_Impianto", "Finalita_Concimazione_Impianto", "number")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        'INDIRIZZO
        c = New ColonneNome("Cod_Indirizzo", "Cod_Indirizzo", "number") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("ind_des", "ind_des", "string") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("frz_des", "frz_des", "string") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("CAP", "CAP", "string") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("com_des_indirizzo", "com_des", "string") With {
            ._Editabile = True,
            ._hidden = True
        }

        l.Add(c)
        c = New ColonneNome("pro_cod_indirizzo", "pro_cod", "string") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("stato_indirizzo", "stato", "string") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("stato_indirizzo_des", "stato_des", "string") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("note_indirizzo", "note", "string") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("pro_cod_istat_indirizzo", "pro_cod_istat", "string") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("com_cod_istat_indirizzo", "com_cod_istat", "string") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("KPIN", "KPIN", "string") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("Block_Name", "Block_Name", "string") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("Data_Inizio_Portinnesto", "Data_Inizio_Portinnesto", "string") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("Mat_Cod", "Mat_Cod", "number") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        js.Editabile_Deafault = False
        Dim risp As String = js.JSON_DataTable_Kendo(DT_Appezzamenti, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto) 'True, True, TipoFiltroKendo_colonne.CasellaTesto) '

        Return risp

    End Function

    Private Shared Function JSON_DatatableAppezzamentiEliminati_Tabella(ByRef DT_Appezzamenti_Eliminati As DataTable) As String
        Dim l As New List(Of ColonneNome)
        Dim c As ColonneNome

        c = New ColonneNome("Piva", "Piva", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Sa_Cod", "Sa_Cod", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Campo_Cod", "Campo_Cod", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Appezza", "Appezza", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("ID_Reg", "ID_Reg", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Progetto_Cod", "Progetto_Cod", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Programmazione_Entita_Cod", "Programmazione_Entita_Cod", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Operazione_Cod", "Operazione_Cod", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Operazione", "Operazione", "string")
        'c._hidden = True
        c._width = "107.27px"
        l.Add(c)

        c = New ColonneNome("Sa_Nome", "Centro", "string")
        'c._hidden = True
        l.Add(c)

        c = New ColonneNome("Campo_Des", "Campo", "string")
        'c._hidden = True
        l.Add(c)

        c = New ColonneNome("App_Nome", "App.", "string")
        'c._hidden = True
        c._width = "68.18px"
        l.Add(c)

        c = New ColonneNome("Sup_App", "Sup.[ha]", "string")
        'c._hidden = True
        c._width = "67.27px"
        l.Add(c)

        c = New ColonneNome("Veg_Des", "Specie", "string")
        'c._hidden = True
        l.Add(c)

        c = New ColonneNome("Cul_Des", "Varietà", "string")
        'c._hidden = True
        l.Add(c)

        c = New ColonneNome("Grfi_Des", "Finalità", "string")
        'c._hidden = True
        l.Add(c)

        c = New ColonneNome("Progetto_Nome", "Lotto", "string")
        'c._hidden = True
        c._width = "131.82px"
        l.Add(c)

        c = New ColonneNome("DestinazioneUso_Des", "Destinazione d'Uso", "string")
        c._width = "130.91px"
        'c._hidden = True
        l.Add(c)

        c = New ColonneNome("Data_Chiusura", "Data", "string")
        'c._hidden = True
        l.Add(c)

        c = New ColonneNome("UNID_APP_NEW", "UNID_APP_NEW", "string")
        c._hidden = True
        l.Add(c)

        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        js.Editabile_Deafault = False
        Dim risp As String = js.JSON_DataTable_Kendo(DT_Appezzamenti_Eliminati, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto) 'True, True, TipoFiltroKendo_colonne.CasellaTesto) '
        Return risp
    End Function

#End Region

#Region "PrenotazionePiante"

    Public Function Leggi_PrenotazionePiante_Riepilogo(Piva As String,
                                                       Servizio_Cod As Integer,
                                                       Data As Date,
                                                       Filtro_Visibilita_Utente As Boolean,
                                                       Sintetico As Boolean,
                                                       objParametri_Server As AgronicaCoreParametri) As DataTable

        Dim DT As New DataTable

        Try

            Dim objProgrammazione_Entita As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_R

            DT = objProgrammazione_Entita.PrenotazionePiante_Riepilogo(0, 0, Piva, Data, Filtro_Visibilita_Utente, "", " Imprese.Rag_Soc ", Sintetico, objParametri_Server)

        Catch ex As Exception

            DT = Nothing

        End Try

        Return DT
    End Function

#End Region



End Class

Public Class Programmazione_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Friend Class CopiaPrenotazioneEsitoModel
        Public Property Programmazione_Cod As Integer
        Public Property Programmazione_Entita_Cod As Integer
    End Class

    'Public Function Programmazione_EF_Scrivi(ByVal Programmazione As AgronicaCoreEntityFramework.Programmazione_Testata, ByRef objParametri As AgronicaCoreParametri) As Integer



    '    Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility

    '    Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

    '    Dim campTestata As New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(EFConnString)


    '    campTestata.AddToProgrammazione_Testata(Programmazione)

    '    Dim rval As Integer
    '    rval = campTestata.SaveChanges()

    '    Return rval

    'End Function



    'Public Function Programmazione_EF_Scrivi(ByVal Programmazione As String, ByRef objParametri As AgronicaCoreParametri) As Integer

    '    Dim ef2 As New AgronicaCoreEntityFramework.Programmazione_Testata

    '    ef2 = AgronicaCoreUtility.AgroSerializer.Deserialize_Use_DataContractSerializer_XmlString(Of AgronicaCoreEntityFramework.Programmazione_Testata)(Programmazione)

    '    Dim rval As Integer
    '    rval = Programmazione_EF_Scrivi(ef2, objParametri)

    '    Return rval
    'End Function

    '################################################################################
    Public Function EvadiOrdine(Programmazione_cod As Integer, objParametri_Server As AgronicaCoreParametri) As RispostaStandard

        Dim rval As New RispostaStandard


        Dim messaggioErrore As String = ""

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False


        'TODO: Gestire la transazione
        Try

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Apro la connessione al DB
            ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                           FlagTransazioneLocale,
                                                           objParametri_Server)



            Dim objtestataR As New AgronicaCoreAnagrafeDAL.Programmazione_Testata_R
            Dim objtestataW As New AgronicaCoreAnagrafeDAL.Programmazione_Testata_W

            objtestataW.Modifica_Stato(Programmazione_cod, enum_PrenotazionePiante_Stato.o_evaso, "", objParametri_Server)

            Dim dtRichiestaAssociata As DataTable =
                objtestataR.LeggiPrenotazioneAssociataAdOrdine(Programmazione_cod, "", "", objParametri_Server)

            If dtRichiestaAssociata.Rows.Count > 0 Then

                Dim Programmazione_cod_Richiesta As Integer =
                    dtRichiestaAssociata(0)("Programmazione_Cod")

                objtestataW.Modifica_Stato(Programmazione_cod_Richiesta, enum_PrenotazionePiante_Stato.p_evaso, "", objParametri_Server)

            End If

            rval.RispostaOK = True
            rval.RispostaStringa = ""

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Chiudo la connessione al DB
            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        Catch ex As Exception

            'Faccio il rollback della transazione
            If objParametri_Server.objTransazione IsNot Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)

            End If

            messaggioErrore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)


            Dim Messaggio As String = ""
            If messaggioErrore <> "" Then


                Messaggio += "ERR: Sono stati rilevati i seguenti errori : " & vbCrLf
                Messaggio += "" & vbCrLf
                Messaggio += messaggioErrore
                Messaggio += "" & vbCrLf
                Messaggio += "Ritentare il salvataggio dopo la correzione ..."

            End If

            rval.RispostaOK = False
            rval.Errore = Messaggio

        Finally
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)
        End Try

        Return rval

    End Function

    Public Function ImpostaStatoOrdine(Programmazione_cod As Integer, Stato_Cod As Integer, objParametri_Server As AgronicaCoreParametri) As RispostaStandard

        Dim rval As New RispostaStandard


        Dim messaggioErrore As String = ""

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False


        'TODO: Gestire la transazione
        Try

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Apro la connessione al DB
            ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                           FlagTransazioneLocale,
                                                           objParametri_Server)



            Dim objtestataR As New AgronicaCoreAnagrafeDAL.Programmazione_Testata_R
            Dim objtestataW As New AgronicaCoreAnagrafeDAL.Programmazione_Testata_W

            objtestataW.Modifica_Stato(Programmazione_cod, Stato_Cod, "", objParametri_Server)

            rval.RispostaOK = True
            rval.RispostaStringa = ""

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Chiudo la connessione al DB
            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        Catch ex As Exception

            'Faccio il rollback della transazione
            If objParametri_Server.objTransazione IsNot Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)

            End If

            messaggioErrore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)


            Dim Messaggio As String = ""
            If messaggioErrore <> "" Then


                Messaggio += "ERR: Sono stati rilevati i seguenti errori : " & vbCrLf
                Messaggio += "" & vbCrLf
                Messaggio += messaggioErrore
                Messaggio += "" & vbCrLf
                Messaggio += "Ritentare il salvataggio dopo la correzione ..."

            End If

            rval.RispostaOK = False
            rval.Errore = Messaggio

        Finally
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)
        End Try

        Return rval

    End Function

    Private Shared Function LeggiCodiceAnagrafe(programmazione_entita_cod As Integer, objParametri_Server As AgronicaCoreParametri, id_Cod As enum_CodiciAnagrafe, leggiCodiciAnagrafe As AgronicaCoreAnagrafeDAL.Programmazione_Entita_Codici_R) As String
        Dim dtVal As DataTable = leggiCodiciAnagrafe.Leggi(programmazione_entita_cod, id_Cod, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
        Dim val_Cod As String = "0"
        If dtVal.Rows.Count > 0 Then
            val_Cod = dtVal(0)("val_cod")
        End If

        Return val_Cod
    End Function

    Public Function CopiaPrenotazione(Programmazione_cod As Integer, objParametri_Server As AgronicaCoreParametri) As RispostaStandard

        Dim r As New RispostaStandard




        Dim messaggioErrore As String = ""

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False


        'TODO: Gestire la transazione
        Try

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Apro la connessione al DB
            ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                           FlagTransazioneLocale,
                                                           objParametri_Server)


            Dim objEW As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_W
            Dim objTW As New AgronicaCoreAnagrafeDAL.Programmazione_Testata_W


            'modifico lo stato
            objTW.Modifica_Stato(Programmazione_cod, enum_PrenotazionePiante_Stato.p_copiato, "", objParametri_Server)

            'leggo Testata e entita
            Dim objTR As New AgronicaCoreAnagrafeDAL.Programmazione_Testata_R
            Dim objER As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_R
            Dim dt_t As DataTable = objTR.Leggi("", Programmazione_cod, "", "", 1, AGRODATAINIZIO, AGRODATAFINE, enum_TipoPianificazione.Pianificazione_PrenotazionePiante,
                                                enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

            Dim dt_e As DataTable = objER.Leggi_solo_Programmazione_entita(Programmazione_cod, 0, objParametri_Server)

            Dim old_Programmazione_Entita_Cod As Integer = dt_e(0)("Programmazione_Entita_Cod")




            'seleziono il max di quell'anno
            Dim objprogrammazioneTestatar As New AgronicaCoreAnagrafeDAL.Programmazione_Testata_R
            Dim max_ID_num_colture As Integer = objprogrammazioneTestatar.GetMax_NumeColture(enum_TipoPianificazione.Pianificazione_PrenotazionePiante, objParametri_Server) + 1
            'nuova Testata
            Dim new_programmazione_cod As Integer
            objTW.Scrivi(new_programmazione_cod, dt_t.Rows(0).Item("programmazione_des"), dt_t.Rows(0).Item("programmazione_des_long"),
                                             dt_t.Rows(0).Item("piva"), dt_t.Rows(0).Item("note"), enum_TipoPianificazione.Pianificazione_PrenotazionePiante,
                                             0, dt_t.Rows(0).Item("validita_inizio"), dt_t.Rows(0).Item("validita_fine"),
                                             objParametri_Server, , , , , , max_ID_num_colture,
                                             enum_PrenotazionePiante_Stato.p_copiato)


            'nuova Entità
            Dim new_Programmazione_Entita_Cod As Integer
            Dim data_ordine As Date = Date.Now
            objEW.Scrivi(
                Programmazione_Cod:=new_programmazione_cod,
                Programmazione_Entita_Cod:=new_Programmazione_Entita_Cod,
                Entita_Des:=dt_e.Rows(0).Item("Entita_Des"),
                Piva:=dt_e.Rows(0).Item("Piva"),
                Sa_Cod:=dt_e.Rows(0).Item("Sa_Cod"),
                Campo_Cod:=dt_e.Rows(0).Item("Campo_Cod"),
                Appezza:=dt_e.Rows(0).Item("Appezza"),
                Id_Reg:=dt_e.Rows(0).Item("Id_Reg"),
                Progetto_Cod:=0,
                Progetto_Des:=dt_e.Rows(0).Item("Progetto_Des"),
                Id_Cod:=dt_e.Rows(0).Item("Id_Cod"),
                Veg_Cod:=dt_e.Rows(0).Item("Veg_Cod"),
                Cul_Cod:=dt_e.Rows(0).Item("Cul_Cod"),
                Grva_Cod:=dt_e.Rows(0).Item("Grva_Cod"),
                Grfi_Cod:=dt_e.Rows(0).Item("Grfi_Cod"),
                Cop_Cod:=dt_e.Rows(0).Item("Cop_Cod"),
                Superficie:=dt_e.Rows(0).Item("Superficie"),
                Resa:=dt_e.Rows(0).Item("Resa"),
                TipoZona:=dt_e.Rows(0).Item("TipoZona"),
                Veg_Cod_Prec:=dt_e.Rows(0).Item("Veg_Cod_Prec"),
                Id_Mat_O:=dt_e.Rows(0).Item("Id_Mat_O"),
                Id_Fre:=dt_e.Rows(0).Item("Id_Fre"),
                N_distribuito:=dt_e.Rows(0).Item("N_distribuito"),
                Num_Piante:=dt_e.Rows(0).Item("Num_Piante"),
                Tra_Fila:=dt_e.Rows(0).Item("Tra_Fila"),
                Su_Fila:=dt_e.Rows(0).Item("Su_Fila"),
                Foral_Cod:=dt_e.Rows(0).Item("Foral_Cod"),
                Port_Cod:=dt_e.Rows(0).Item("Port_Cod"),
                Imp_Cod:=0,
                Regolamento_Cod:=dt_e.Rows(0).Item("Regolamento_Cod"),
                Disciplinare_Cod:=0,
                Stato_Cod:=0,
                Ciclo:=dt_e.Rows(0).Item("Ciclo"),
                Data_Semina:=dt_e.Rows(0).Item("Data_Semina"),
                Data_Raccolta:=dt_e.Rows(0).Item("Data_Raccolta"),
                Note:=dt_e.Rows(0).Item("Note"),
                Veg_Cod_Cliente:=dt_e.Rows(0).Item("Veg_Cod_Cliente"),
                Cul_Cod_Cliente:=dt_e.Rows(0).Item("Cul_Cod_Cliente"),
                Validita_Inizio:=dt_e.Rows(0).Item("Validita_Inizio"),
                Validita_Fine:=dt_e.Rows(0).Item("Validita_Fine"),
                objParametri:=objParametri_Server,
                Veg_Cod_Agea:=dt_e.Rows(0).Item("Veg_Cod_Agea"),
                Cul_Cod_Agea:=dt_e.Rows(0).Item("Cul_Cod_Agea"),
                Uso_Cod_Agea:=dt_e.Rows(0).Item("Uso_Cod_Agea"),
                Occupazione_Cod_Agea:=dt_e.Rows(0).Item("Occupazione_Cod_Agea"),
                Destinazione_Cod_Agea:=dt_e.Rows(0).Item("Destinazione_Cod_Agea"),
                Qualita_Cod_Agea:=dt_e.Rows(0).Item("Qualita_Cod_Agea"),
                Data_Fioritura_Prevista:=dt_e.Rows(0).Item("Data_Fioritura_Prevista"),
                Conversione_Data_Inizio:=dt_e.Rows(0).Item("Conversione_Data_Inizio"),
                Veg_Cod_Prec3:=dt_e.Rows(0).Item("Veg_Cod_Prec3"),
                Veg_Cod_Prec4:=dt_e.Rows(0).Item("Veg_Cod_Prec4"),
                Stato_Ribaltamento:=dt_e.Rows(0).Item("Stato_Ribaltamento"),
                Regolamento_Concimazione_Cod:=dt_e.Rows(0).Item("Regolamento_Concimazione_Cod"),
                isola:=dt_e.Rows(0).Item("isola"),
                CapitolatoPrivato:=dt_e.Rows(0).Item("CapitolatoPrivato"),
                Codice_Contratto:=dt_e.Rows(0).Item("Codice_Contratto"),
                id_budget:=dt_e.Rows(0).Item("Id_Budget"),
                germinabilita:=dt_e.Rows(0).Item("germinabilita"),
                Codice_Fiscale_Tecnico:=dt_e.Rows(0).Item("Codice_Fiscale_Tecnico"),
                Superficie_Futura:=dt_e.Rows(0).Item("Superficie_Futura"),
                SupBZ_Riduzione:=dt_e.Rows(0).Item("SupBZ_Riduzione"),
                DistBZ_VegNatNonColt:=dt_e.Rows(0).Item("DistBZ_VegNatNonColt"),
                Cod_Macrouso:=dt_e.Rows(0).Item("Macrouso_Cod"),
                MetodoProduzione_Cod:=dt_e.Rows(0).Item("MetodoProduzione_Cod"),
                descImpiantoBudget:=dt_e.Rows(0).Item("Desc_impianto_budget")
            )


            Dim leggiCodiciAnagrafe As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_Codici_R
            Dim scriviCodiciAnagrafe As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_Codici_W
            Dim prenotazione_guideAudits_Fase As String = LeggiCodiceAnagrafe(old_Programmazione_Entita_Cod, objParametri_Server, enum_CodiciAnagrafe.Zespri_Fasi_Fase, leggiCodiciAnagrafe)
            Dim prenotazione_guideAudits_Tipo As String = LeggiCodiceAnagrafe(old_Programmazione_Entita_Cod, objParametri_Server, enum_CodiciAnagrafe.Zespri_Fasi_Tipo, leggiCodiciAnagrafe)
            Dim prenotazione_guideAudits_Grower As String = LeggiCodiceAnagrafe(old_Programmazione_Entita_Cod, objParametri_Server, enum_CodiciAnagrafe.Zespri_Fasi_Grower, leggiCodiciAnagrafe)

            Dim scriviCodicePlanning As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_Codici_W
            scriviCodicePlanning.Scrivi(new_Programmazione_Entita_Cod, enum_CodiciAnagrafe.Zespri_Fasi_Fase, prenotazione_guideAudits_Fase, AGRODATAINIZIO, AGRODATAFINE, objParametri_Server)
            scriviCodicePlanning.Scrivi(new_Programmazione_Entita_Cod, enum_CodiciAnagrafe.Zespri_Fasi_Tipo, prenotazione_guideAudits_Fase, AGRODATAINIZIO, AGRODATAFINE, objParametri_Server)
            scriviCodicePlanning.Scrivi(new_Programmazione_Entita_Cod, enum_CodiciAnagrafe.Zespri_Fasi_Grower, prenotazione_guideAudits_Fase, AGRODATAINIZIO, AGRODATAFINE, objParametri_Server)


            Dim str_agg As String

            str_agg = "Copia avvenuta con successo"


            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Chiudo la connessione al DB
            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''



            r.RispostaOK = True
            r.RispostaStringa = str_agg
            r.ParametroDue_stringa = "{ ""Programmazione_Cod"": " & new_programmazione_cod & ", ""Programmazione_Entita_Cod"": " & new_Programmazione_Entita_Cod & " }"



        Catch ex As Exception

            'Faccio il rollback della transazione
            If objParametri_Server.objTransazione IsNot Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)

            End If

            messaggioErrore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)

            Throw ex

            Dim Messaggio As String = ""
            If messaggioErrore <> "" Then


                Messaggio += "ERR: Sono stati rilevati i seguenti errori : " & vbCrLf
                Messaggio += "" & vbCrLf
                Messaggio += messaggioErrore
                Messaggio += "" & vbCrLf
                Messaggio += "Ritentare il salvataggio dopo la correzione ..."

            End If

            r.RispostaOK = False
            r.Errore = Messaggio

        Finally
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)
        End Try

        Return r
    End Function
    Public Function CopiaPrenotazione_bis(Programmazione_cod As Integer, objParametri_Server As AgronicaCoreParametri) As RispostaStandard

        Dim r As New RispostaStandard




        Dim messaggioErrore As String = ""

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False


        'TODO: Gestire la transazione
        Try

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Apro la connessione al DB
            ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                           FlagTransazioneLocale,
                                                           objParametri_Server)



            Dim objEW As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_W
            Dim objTW As New AgronicaCoreAnagrafeDAL.Programmazione_Testata_W


            'modifico lo stato
            objTW.Modifica_Stato(Programmazione_cod, enum_PrenotazionePiante_Stato.p_copiato, "", objParametri_Server)

            'leggo Testata e entita
            Dim objTR As New AgronicaCoreAnagrafeDAL.Programmazione_Testata_R
            Dim objER As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_R
            Dim dt_t As DataTable = objTR.Leggi("", Programmazione_cod, "", "", 1, AGRODATAINIZIO, AGRODATAFINE, enum_TipoPianificazione.Pianificazione_PrenotazionePiante,
                                                enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

            Dim dt_e As DataTable = objER.Leggi_solo_Programmazione_entita(Programmazione_cod, 0, objParametri_Server)

            Dim old_Programmazione_Entita_Cod As Integer = dt_e(0)("Programmazione_Entita_Cod")




            'seleziono il max di quell'anno
            Dim objprogrammazioneTestatar As New AgronicaCoreAnagrafeDAL.Programmazione_Testata_R
            Dim max_ID_num_colture As Integer = objprogrammazioneTestatar.GetMax_NumeColture(enum_TipoPianificazione.Pianificazione_PrenotazionePiante, objParametri_Server) + 1
            'nuova Testata
            Dim new_programmazione_cod As Integer
            objTW.Scrivi(new_programmazione_cod, dt_t.Rows(0).Item("programmazione_des"), dt_t.Rows(0).Item("programmazione_des_long"),
                                             dt_t.Rows(0).Item("piva"), dt_t.Rows(0).Item("note"), enum_TipoPianificazione.Pianificazione_PrenotazionePiante,
                                             0, dt_t.Rows(0).Item("validita_inizio"), dt_t.Rows(0).Item("validita_fine"),
                                             objParametri_Server, , , , , , max_ID_num_colture,
                                             enum_PrenotazionePiante_Stato.p_copiato)


            'nuova Entità
            Dim new_Programmazione_Entita_Cod As Integer
            Dim data_ordine As Date = Date.Now
            objEW.Scrivi(
                Programmazione_Cod:=new_programmazione_cod,
                Programmazione_Entita_Cod:=new_Programmazione_Entita_Cod,
                Entita_Des:=dt_e.Rows(0).Item("Entita_Des"),
                Piva:=dt_e.Rows(0).Item("Piva"),
                Sa_Cod:=dt_e.Rows(0).Item("Sa_Cod"),
                Campo_Cod:=dt_e.Rows(0).Item("Campo_Cod"),
                Appezza:=dt_e.Rows(0).Item("Appezza"),
                Id_Reg:=dt_e.Rows(0).Item("Id_Reg"),
                Progetto_Cod:=0,
                Progetto_Des:=dt_e.Rows(0).Item("Progetto_Des"),
                Id_Cod:=dt_e.Rows(0).Item("Id_Cod"),
                Veg_Cod:=dt_e.Rows(0).Item("Veg_Cod"),
                Cul_Cod:=dt_e.Rows(0).Item("Cul_Cod"),
                Grva_Cod:=dt_e.Rows(0).Item("Grva_Cod"),
                Grfi_Cod:=dt_e.Rows(0).Item("Grfi_Cod"),
                Cop_Cod:=dt_e.Rows(0).Item("Cop_Cod"),
                Superficie:=dt_e.Rows(0).Item("Superficie"),
                Resa:=dt_e.Rows(0).Item("Resa"),
                TipoZona:=dt_e.Rows(0).Item("TipoZona"),
                Veg_Cod_Prec:=dt_e.Rows(0).Item("Veg_Cod_Prec"),
                Id_Mat_O:=dt_e.Rows(0).Item("Id_Mat_O"),
                Id_Fre:=dt_e.Rows(0).Item("Id_Fre"),
                N_distribuito:=dt_e.Rows(0).Item("N_distribuito"),
                Num_Piante:=dt_e.Rows(0).Item("Num_Piante"),
                Tra_Fila:=dt_e.Rows(0).Item("Tra_Fila"),
                Su_Fila:=dt_e.Rows(0).Item("Su_Fila"),
                Foral_Cod:=dt_e.Rows(0).Item("Foral_Cod"),
                Port_Cod:=dt_e.Rows(0).Item("Port_Cod"),
                Imp_Cod:=0,
                Regolamento_Cod:=dt_e.Rows(0).Item("Regolamento_Cod"),
                Disciplinare_Cod:=0,
                Stato_Cod:=0,
                Ciclo:=dt_e.Rows(0).Item("Ciclo"),
                Data_Semina:=dt_e.Rows(0).Item("Data_Semina"),
                Data_Raccolta:=dt_e.Rows(0).Item("Data_Raccolta"),
                Note:=dt_e.Rows(0).Item("Note"),
                Veg_Cod_Cliente:=dt_e.Rows(0).Item("Veg_Cod_Cliente"),
                Cul_Cod_Cliente:=dt_e.Rows(0).Item("Cul_Cod_Cliente"),
                Validita_Inizio:=dt_e.Rows(0).Item("Validita_Inizio"),
                Validita_Fine:=dt_e.Rows(0).Item("Validita_Fine"),
                objParametri:=objParametri_Server,
                Veg_Cod_Agea:=dt_e.Rows(0).Item("Veg_Cod_Agea"),
                Cul_Cod_Agea:=dt_e.Rows(0).Item("Cul_Cod_Agea"),
                Uso_Cod_Agea:=dt_e.Rows(0).Item("Uso_Cod_Agea"),
                Occupazione_Cod_Agea:=dt_e.Rows(0).Item("Occupazione_Cod_Agea"),
                Destinazione_Cod_Agea:=dt_e.Rows(0).Item("Destinazione_Cod_Agea"),
                Qualita_Cod_Agea:=dt_e.Rows(0).Item("Qualita_Cod_Agea"),
                Data_Fioritura_Prevista:=dt_e.Rows(0).Item("Data_Fioritura_Prevista"),
                Conversione_Data_Inizio:=dt_e.Rows(0).Item("Conversione_Data_Inizio"),
                Veg_Cod_Prec3:=dt_e.Rows(0).Item("Veg_Cod_Prec3"),
                Veg_Cod_Prec4:=dt_e.Rows(0).Item("Veg_Cod_Prec4"),
                Stato_Ribaltamento:=dt_e.Rows(0).Item("Stato_Ribaltamento"),
                Regolamento_Concimazione_Cod:=dt_e.Rows(0).Item("Regolamento_Concimazione_Cod"),
                isola:=dt_e.Rows(0).Item("isola"),
                CapitolatoPrivato:=dt_e.Rows(0).Item("CapitolatoPrivato"),
                Codice_Contratto:=dt_e.Rows(0).Item("Codice_Contratto"),
                id_budget:=dt_e.Rows(0).Item("Id_Budget"),
                germinabilita:=dt_e.Rows(0).Item("germinabilita"),
                Codice_Fiscale_Tecnico:=dt_e.Rows(0).Item("Codice_Fiscale_Tecnico"),
                Superficie_Futura:=dt_e.Rows(0).Item("Superficie_Futura"),
                SupBZ_Riduzione:=dt_e.Rows(0).Item("SupBZ_Riduzione"),
                DistBZ_VegNatNonColt:=dt_e.Rows(0).Item("DistBZ_VegNatNonColt"),
                Cod_Macrouso:=dt_e.Rows(0).Item("Macrouso_Cod"),
                MetodoProduzione_Cod:=dt_e.Rows(0).Item("MetodoProduzione_Cod"),
                descimpiantobudget:=dt_e.Rows(0).Item("Desc_impianto_budget"),
                dataconsegna:=dt_e.Rows(0).Item("Data_Consegna"),
                budget_piva:=dt_e.Rows(0).Item("budget_piva"),
                budget_sa_cod:=dt_e.Rows(0).Item("budget_sa_cod"),
                budget_id_reg:=dt_e.Rows(0).Item("budget_id_reg"),
                budget_appezza:=dt_e.Rows(0).Item("budget_appezza"),
                qta_seme_omaggio:=dt_e.Rows(0).Item("qta_seme_omaggio"),
                id_plateau:=dt_e.Rows(0).Item("id_plateau")
            )



            Dim str_agg As String

            str_agg = "Copia avvenuta con successo"


            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Chiudo la connessione al DB
            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''



            r.RispostaOK = True
            r.RispostaStringa = str_agg
            r.ParametroDue_stringa = "{ ""Programmazione_Cod"": " & new_programmazione_cod & ", ""Programmazione_Entita_Cod"": " & new_Programmazione_Entita_Cod & " }"



        Catch ex As Exception

            'Faccio il rollback della transazione
            If objParametri_Server.objTransazione IsNot Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)

            End If

            messaggioErrore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)

            Throw ex

            Dim Messaggio As String = ""
            If messaggioErrore <> "" Then


                Messaggio += "ERR: Sono stati rilevati i seguenti errori : " & vbCrLf
                Messaggio += "" & vbCrLf
                Messaggio += messaggioErrore
                Messaggio += "" & vbCrLf
                Messaggio += "Ritentare il salvataggio dopo la correzione ..."

            End If

            r.RispostaOK = False
            r.Errore = Messaggio

        Finally

            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

        End Try

        Return r
    End Function


    ''' <summary>
    ''' Dalla programmazione_cod della richiesta recupera, se esiste il codice ordine associato alla richiesta
    ''' </summary>
    ''' <param name="programmazione_entita_Cod"></param>
    ''' <returns></returns>
    Public Function PianteLeggiCodiceOrdineCollegatoARichiesta(ByVal programmazione_entita_Cod As Integer, objParametri_Server As AgronicaCoreParametri) As String

        Dim xLetturaRif As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_R
        Dim dtRif As DataTable =
            xLetturaRif.PianteLeggiCodiceOrdineCollegatoARichiesta(programmazione_entita_Cod, "", "", objParametri_Server)

        Dim rval As String = ""
        If dtRif.Rows.Count = 1 Then
            rval = dtRif(0)("CodiceOrdine")
        End If

        Return rval

    End Function


    Public Shared Function CodiceDatoAnno_NumeroProgressivo(Prefisso_O_R As String, ByVal anno As Integer, ByVal numero As Integer) As String
        Return Prefisso_O_R & Right(anno, 2) & "-" & numero.ToString.PadLeft(5, "0")
    End Function

    Public Function OrdineDaPrenotazione(Programmazione_cod As Integer, objParametri_Server As AgronicaCoreParametri) As RispostaStandard

        Dim rval As New RispostaStandard

        Dim messaggioErrore As String = ""

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False


        'TODO: Gestire la transazione
        Try

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Apro la connessione al DB
            ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                           FlagTransazioneLocale,
                                                           objParametri_Server)






            'copio la prenotazione su prenotazione, poi modifico quanto serve sulla nuova prenotazione e la trasformo in un ordine
            Dim rvalCopiaPrenotazione As RispostaStandard =
                CopiaPrenotazione(Programmazione_cod, objParametri_Server)

            Dim CopiaPrenotazioneEsito As CopiaPrenotazioneEsitoModel =
                DeserializeObject(Of CopiaPrenotazioneEsitoModel)(rvalCopiaPrenotazione.ParametroDue_stringa)

            'aggiusto i dati.
            Dim leggiTestata As New AgronicaCoreAnagrafeDAL.Programmazione_Testata_R
            Dim leggiEntita As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_R

            Dim scriviTestata As New AgronicaCoreAnagrafeDAL.Programmazione_Testata_W
            Dim scriviEntita As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_W

            'Testata (programmazione_Testata)
            Dim dtTestaProgrammazione As DataTable =
                leggiTestata.Leggi("", Programmazione_cod, "", "", 1, AGRODATAINIZIO, AGRODATAFINE, enum_TipoPianificazione.Pianificazione_PrenotazionePiante, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

            Dim dtEntitaProgrammazione As DataTable =
                leggiEntita.Leggi(Programmazione_cod, 0, "", "", 0, 0, 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE, "", "", "", objParametri_Server)

            'Ordine Piante + rag_soc_vivaio
            'Partita IVA vivaio copiare da Veg_Cod_Cliente in entita
            Dim pivaVivaio As String =
                dtEntitaProgrammazione.Rows(0)("Veg_Cod_Cliente")

            Dim leggiRagSocVivaio As New AgronicaCoreAnagrafeDAL.Risorse_Umane_R
            Dim dtRagSocVivaio As DataTable =
                leggiRagSocVivaio.Leggi4("", pivaVivaio, 0, enum_Rapporti_Contabili_Standard.Vivaio, "", False, False, False, False, False, False, False, "", "", objParametri_Server)

            'rag_soc_vivaio
            Dim ragSocVivaio As String = ""
            If dtRagSocVivaio.Rows.Count > 0 Then
                Dim leggiContatto As New AgronicaCoreAnagrafeDAL.Contatti_R
                Dim dtContatto As DataTable =
                    leggiContatto.LeggiContattoSpecifico(dtRagSocVivaio(0)("piva"), pivaVivaio, -1, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                ragSocVivaio = dtContatto(0)("Rag_Soc")
            End If


            'Vivaio Preferito (join su programmazione_Testata da Programmazione_Cod_Padre); il campo Piva del padre è la piva del vivaio

            'Tipo_Pianificazione
            scriviTestata.Modifica_PrenotazionePiante_Ordine_Small(
                CopiaPrenotazioneEsito.Programmazione_Cod,
                "Ordine Piante",
                "Ordine Piante " & ragSocVivaio,
                pivaVivaio,
                ragSocVivaio,
                0,
                enum_TipoPianificazione.Pianificazione_OrdinePiante,
                "",
                objParametri_Server
            )

            'Entita
            'ordine_vivaista: copiare da Veg_Cod_Cliente

            'Piante Disponibili (n_Marze o n_piante) --> impostare a Zero, si suppone che sia evaso, poiché viene ribaltato ed associato
            Dim TotalePrenotatoSuQuestoOrdine As Integer = dtEntitaProgrammazione(0)("Num_Piante")
            scriviEntita.Modifica_Residuo_Ordine(CopiaPrenotazioneEsito.Programmazione_Cod, 0, TotalePrenotatoSuQuestoOrdine, "", objParametri_Server)

            'riporto i residui sulla prenotazione originale
            scriviEntita.Modifica_Residuo_Ordine(Programmazione_cod, 0, TotalePrenotatoSuQuestoOrdine, "", objParametri_Server)

            'ordine_portinnesto --> Regolamento_Cod

            'solo prenotazione
            'prenotazione_ff
            'prenotazione_sf

            'richiesta --> Port_Cod (prenotazioni) su imp_cod (ordini)
            Dim ProgrammazioneEntitaCodRichista As String =
                dtEntitaProgrammazione.Rows(0)("Programmazione_Entita_Cod")

            Dim codiceOrdineCollegato As String =
                PianteLeggiCodiceOrdineCollegatoARichiesta(ProgrammazioneEntitaCodRichista, objParametri_Server)

            If codiceOrdineCollegato = "" Then

                Dim xSeqProg As New Sequenza_Progressivi_R
                Dim prenotazione_anno As Integer = CDate(dtTestaProgrammazione.Rows(0)("validita_inizio")).Year
                Dim p1 As Integer =
                        xSeqProg.Nuovo_Progressivo_UpdateImmediato("", prenotazione_anno, enum_SequenzaProgressiviTipi.PrenotazionePiante_Ordini, "", "", 0, objParametri_Server)

                codiceOrdineCollegato = CodiceDatoAnno_NumeroProgressivo("O", prenotazione_anno, p1)

            End If

            scriviEntita.Modifica_PrenotazionePiante_Ordine_Small(
                Programmazione_Entita_Cod:=CopiaPrenotazioneEsito.Programmazione_Entita_Cod,
                piva:=pivaVivaio,
                port_cod:=0,
                Tra_Fila:=dtEntitaProgrammazione(0)("Regolamento_Cod"),
                Su_Fila:=0,
                imp_cod:=dtEntitaProgrammazione(0)("Port_Cod"),
                riferimento_alfanumerico_appezzamento:=codiceOrdineCollegato,
                objParametri:=objParametri_Server
            )

            'legame fra ordine e prenotazione

            scriviTestata.Modifica_Stato_Programmazione_cod_padre(
                Programmazione_cod,
                CopiaPrenotazioneEsito.Programmazione_Cod,
                enum_PrenotazionePiante_Stato.p_abbinato,
                "",
                objParametri_Server
            )

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Chiudo la connessione al DB
            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            rval.RispostaOK = True
            rval.RispostaStringa = "Nuovo Ordine Memorizzato correttamente"


        Catch ex As Exception

            'Faccio il rollback della transazione
            If objParametri_Server.objTransazione IsNot Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)

            End If

            messaggioErrore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)


            Dim Messaggio As String = ""
            If messaggioErrore <> "" Then


                Messaggio += "ERR: Sono stati rilevati i seguenti errori : " & vbCrLf
                Messaggio += "" & vbCrLf
                Messaggio += messaggioErrore
                Messaggio += "" & vbCrLf
                Messaggio += "Ritentare il salvataggio dopo la correzione ..."

            End If

            rval.RispostaOK = False
            rval.Errore = Messaggio

        Finally

            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

        End Try

        'modifico il tipo da prenotazione ad ordine
        Return rval
    End Function


    Public Function OrdineDaPrenotazione_bis(Programmazione_cod As Integer, objParametri_Server As AgronicaCoreParametri) As RispostaStandard

        Dim rval As New RispostaStandard

        Dim messaggioErrore As String = ""

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False


        'TODO: Gestire la transazione
        Try

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Apro la connessione al DB
            ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                           FlagTransazioneLocale,
                                                           objParametri_Server)






            'copio la prenotazione su prenotazione, poi modifico quanto serve sulla nuova prenotazione e la trasformo in un ordine
            Dim rvalCopiaPrenotazione As RispostaStandard =
                CopiaPrenotazione_bis(Programmazione_cod, objParametri_Server)

            Dim CopiaPrenotazioneEsito As CopiaPrenotazioneEsitoModel =
                DeserializeObject(Of CopiaPrenotazioneEsitoModel)(rvalCopiaPrenotazione.ParametroDue_stringa)

            'aggiusto i dati.
            Dim leggiTestata As New AgronicaCoreAnagrafeDAL.Programmazione_Testata_R
            Dim leggiEntita As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_R

            Dim scriviTestata As New AgronicaCoreAnagrafeDAL.Programmazione_Testata_W
            Dim scriviEntita As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_W

            'Testata (programmazione_Testata)
            Dim dtTestaProgrammazione As DataTable =
                leggiTestata.Leggi("", Programmazione_cod, "", "", 1, AGRODATAINIZIO, AGRODATAFINE, enum_TipoPianificazione.Pianificazione_PrenotazionePiante, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

            Dim dtEntitaProgrammazione As DataTable =
                leggiEntita.Leggi(Programmazione_cod, 0, "", "", 0, 0, 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE, "", "", "", objParametri_Server)

            'Ordine Piante + rag_soc_vivaio
            'Partita IVA vivaio copiare da Veg_Cod_Cliente in entita
            Dim pivaVivaio As String =
                dtEntitaProgrammazione.Rows(index:=0)("Veg_Cod_Cliente")

            Dim leggiRagSocVivaio As New AgronicaCoreAnagrafeDAL.Risorse_Umane_R
            Dim dtRagSocVivaio As DataTable =
                leggiRagSocVivaio.Leggi4("", pivaVivaio, 0, enum_Rapporti_Contabili_Standard.Vivaio, "", False, False, False, False, False, False, False, "", "", objParametri_Server)

            'rag_soc_vivaio
            Dim ragSocVivaio As String = ""
            If dtRagSocVivaio.Rows.Count > 0 Then
                Dim leggiContatto As New AgronicaCoreAnagrafeDAL.Contatti_R
                Dim dtContatto As DataTable =
                    leggiContatto.LeggiContattoSpecifico(dtRagSocVivaio(0)("piva"), pivaVivaio, -1, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                ragSocVivaio = dtContatto(0)("Rag_Soc") & " " & dtContatto(0)("Nome") & " " & dtContatto(0)("Cognome")
            End If


            'Vivaio Preferito (join su programmazione_Testata da Programmazione_Cod_Padre); il campo Piva del padre è la piva del vivaio

            'Tipo_Pianificazione
            scriviTestata.Modifica_PrenotazionePiante_Ordine_Small(
                CopiaPrenotazioneEsito.Programmazione_Cod,
                "Ordine Piante e Semi",
                "Ordine Piante " & ragSocVivaio & " e Semi " & "ragSocDitta",
                pivaVivaio,
                ragSocVivaio,
                0,
                enum_TipoPianificazione.Pianificazione_OrdinePiante,
                "",
                objParametri_Server
            )

            'Entita
            'ordine_vivaista: copiare da Veg_Cod_Cliente

            'UHALID: Da Investigare

            'Piante Disponibili (n_Marze o n_piante) --> impostare a Zero, si suppone che sia evaso, poiché viene ribaltato ed associato
            Dim TotalePrenotatoSuQuestoOrdine As Integer = dtEntitaProgrammazione(0)("Num_Piante")
            scriviEntita.Modifica_Residuo_Ordine(CopiaPrenotazioneEsito.Programmazione_Cod, 0, TotalePrenotatoSuQuestoOrdine, "", objParametri_Server)

            'riporto i residui sulla prenotazione originale
            'scriviEntita.Modifica_Residuo_Ordine(Programmazione_cod, 0, TotalePrenotatoSuQuestoOrdine, "", objParametri_Server)

            'ordine_portinnesto --> Regolamento_Cod

            'solo prenotazione
            'prenotazione_ff
            'prenotazione_sf

            'richiesta --> Port_Cod (prenotazioni) su imp_cod (ordini)
            Dim ProgrammazioneEntitaCodRichista As String =
                dtEntitaProgrammazione.Rows(0)("Programmazione_Entita_Cod")

            Dim codiceOrdineCollegato As String =
                PianteLeggiCodiceOrdineCollegatoARichiesta(ProgrammazioneEntitaCodRichista, objParametri_Server)

            If codiceOrdineCollegato = "" Then

                Dim xSeqProg As New Sequenza_Progressivi_R
                Dim prenotazione_anno As Integer = CDate(dtTestaProgrammazione.Rows(0)("validita_inizio")).Year
                Dim p1 As Integer =
                        xSeqProg.Nuovo_Progressivo_UpdateImmediato("", prenotazione_anno, enum_SequenzaProgressiviTipi.PrenotazionePiante_Ordini, "", "", 0, objParametri_Server)

                codiceOrdineCollegato = CodiceDatoAnno_NumeroProgressivo("O", prenotazione_anno, p1)

            End If

            'scriviEntita.Modifica_PrenotazionePiante_Ordine_Small(
            '    Programmazione_Entita_Cod:=CopiaPrenotazioneEsito.Programmazione_Entita_Cod,
            '    piva:=pivaVivaio,
            '    port_cod:=0,
            '    Tra_Fila:=dtEntitaProgrammazione(0)("Tra_Fila"),
            '    Su_Fila:=dtEntitaProgrammazione(0)("Su_Fila"),
            '    imp_cod:=dtEntitaProgrammazione(0)("Port_Cod"),
            '    riferimento_alfanumerico_appezzamento:=codiceOrdineCollegato,
            '    objParametri:=objParametri_Server
            ')

            scriviEntita.Modifica_PrenotazionePiante_Ordine_Small(
                Programmazione_Entita_Cod:=CopiaPrenotazioneEsito.Programmazione_Entita_Cod,
                piva:=pivaVivaio,
                port_cod:=0,
                Tra_Fila:=dtEntitaProgrammazione(0)("Regolamento_Cod"),
                Su_Fila:=0,
                imp_cod:=dtEntitaProgrammazione(0)("Port_Cod"),
                riferimento_alfanumerico_appezzamento:=codiceOrdineCollegato,
                objParametri:=objParametri_Server
            )

            'legame fra ordine e prenotazione

            scriviTestata.Modifica_Stato_Programmazione_cod_padre(
                Programmazione_cod,
                CopiaPrenotazioneEsito.Programmazione_Cod,
                enum_PrenotazionePiante_Stato.p_abbinato,
                "",
                objParametri_Server
            )

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Chiudo la connessione al DB
            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            rval.RispostaOK = True
            rval.RispostaStringa = "Nuovo Ordine Memorizzato correttamente"


        Catch ex As Exception

            'Faccio il rollback della transazione
            If objParametri_Server.objTransazione IsNot Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)

            End If

            messaggioErrore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)


            Dim Messaggio As String = ""
            If messaggioErrore <> "" Then


                Messaggio += "ERR: Sono stati rilevati i seguenti errori : " & vbCrLf
                Messaggio += "" & vbCrLf
                Messaggio += messaggioErrore
                Messaggio += "" & vbCrLf
                Messaggio += "Ritentare il salvataggio dopo la correzione ..."

            End If

            rval.RispostaOK = False
            rval.Errore = Messaggio

        Finally

            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

        End Try

        'modifico il tipo da prenotazione ad ordine
        Return rval
    End Function

    Public Function Ribalta(
            ByVal dati As String,
            ByVal datiChiusi As String,
            ByVal Cmb_Regolamento_Value As String,
            ByVal Chk_Chiudi_Arboree As Boolean,
            ByVal Chk_Crea_Campo As Boolean,
            ByVal Campo_Des As String,
            ByVal ProgressivoGIAS As Integer,
            ByVal paramValidita_Fine As DateTime,
            ByVal objParametri_Server As AgronicaCoreParametri,
            ByVal objParametri_Utenti As AgronicaCoreParametri,
            Optional ByVal Blocca_Appezza As Boolean = False
        ) As RispostaStandard

        Dim r As New RispostaStandard


        'Dim Session = HttpContext.Current.Session

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try
            Dim obj_Dati As RibaltaAppezza()
            obj_Dati = JsonConvert.DeserializeObject(Of RibaltaAppezza())(dati)

            Dim obj_Dati_Chiusi As AppChiusi()
            obj_Dati_Chiusi = JsonConvert.DeserializeObject(Of AppChiusi())(datiChiusi)
            Dim nomi As String = ""
            Dim i As Integer
            Dim Operazione_Cod As Integer

            Dim Key_Piva As String
            Dim Key_SaCod As String
            Dim Key_CampoCod As Integer
            Dim Key_Appezza As Integer
            Dim Key_IdReg As Integer
            Dim Key_ProgettoCod As Integer

            Dim Key_Piva_Old As String
            Dim Key_SaCod_Old As String
            Dim Key_CampoCod_Old As Integer
            Dim Key_Appezza_Old As Integer
            Dim Key_IdReg_Old As Integer
            Dim Key_ProgettoCod_Old As Integer

            Dim Key_Entita As Integer
            Dim key_programmazione_cod As Integer
            Dim Validita_Inizio_Impianto As Date
            Dim Codice_fiscale_tecnico As String

            Dim Validita_inizio_Min? As Date = Nothing
            Dim Validita_Fine_Max? As Date = Nothing

            Dim Validita_Inizio As Date
            Dim Validita_Fine As Date
            Dim Validita_Fine_Appezzamento As Date
            Dim Data_Chiusura As Date
            Dim App_Nome As String
            Dim Progetto_Nome As String
            Dim Id_Cod As Integer
            Dim Veg_Cod As Integer
            Dim Gru_Cod As Integer
            Dim Cul_Cod As Integer
            Dim Grfi_Cod As Integer
            Dim Grva_Cod As Integer
            Dim Sup_App As Decimal
            Dim Imp_Cod As Integer
            Dim Foral_Cod As Integer
            Dim Ribaltato As Integer
            Dim Stato_Impianto As Integer
            Dim Resa As Decimal

            Dim Veg_Cod_Cliente As String = ""
            Dim Cul_Cod_Cliente As String = ""

            Dim Unita_Vitata As Integer = 0

            ' Drudi 02/11/2017
            Dim Cop_Cod As Integer = 0
            Dim Num_Piante As Integer = 0
            Dim TRA_Fila As Double = 0
            Dim SU_Fila As Double = 0
            Dim Metodo_Produzione_Cod As Integer = 0
            Dim Disciplinare_Cod As Integer = 0
            Dim Regolamento_Cod As Integer = 0
            Dim Flag_PubblicoPrivato As Integer = 0
            Dim Id_tr As Integer = 0
            Dim Limite_N As String = ""
            Dim Limite_P As String = ""
            Dim Limite_K As String = ""
            Dim Veg_Cod_Prec As String = ""
            Dim Veg_Cod_Prec2 As String = ""
            Dim Veg_Cod_Prec3 As String = ""
            Dim Veg_Cod_Prec4 As String = ""
            Dim Piano_Semina As String = ""
            Dim Codice_Contratto As String = ""
            Dim Data_Semina As Date
            Dim Data_Raccolta As Date
            Dim Data_Fioritura_Prevista As Date
            Dim IAF As String = ""
            Dim Pratica_Cod As String = ""

            Dim DistBZ_CorpiIdrici As Double
            Dim DistBZ_AreeResPub As Double
            Dim DistBZ_Allevamenti As Double
            Dim DistBZ_VegNatNonColt As Double
            Dim SupBZ_Riduzione As Double

            Dim Riferimento_Alfanumerico_Appezzamento As String = ""
            Dim Isola As String = ""

            Dim CapitolatoPrivato As String = ""

            Dim Finalita_Concimazione_Impianto As Integer = 0

            Dim Cod_Indirizzo As Integer = 0
            Dim ind_des As String = ""
            Dim frz_des As String = ""
            Dim CAP As String = ""
            Dim com_des_indirizzo As String = ""
            Dim pro_cod_indirizzo As String = ""
            Dim stato_indirizzo As String = ""
            Dim stato_indirizzo_des As String = ""
            Dim note_indirizzo As String = ""
            Dim pro_cod_istat_indirizzo As String = ""
            Dim com_cod_istat_indirizzo As String = ""

            Dim KPIN As String = ""
            Dim Block_Name As String = ""

            Dim Mat_Cod As Integer = 0

            Dim Data_Inizio_Portinnesto As Date = AGRODATAINIZIO

            Dim Salva_Limite_N As Boolean
            Dim Blk_Flag = 0

            Dim ZeroData As String = "0"
            Dim ZeroInt As Integer = 0
            Dim ZeroString As String = "0"
            Dim ZeroDecimal As Decimal = 0
            Dim NullString As String = ""
            Dim Null As String = "NULL"
            Dim PuntoString As String = "."

            Dim BaseCode As Integer
            Dim TopCode As Integer

            Dim OUTPUT_Piva As String
            Dim OUTPUT_Sa_Cod As Integer
            Dim OUTPUT_Campo_Cod As Integer
            Dim OUTPUT_Appezza As Integer
            Dim OUTPUT_Id_Reg As Integer
            Dim OUTPUT_Progetto_Cod As Integer
            Dim InseritoProgetto As Boolean = False
            Dim InseritoImpianto As Boolean = False
            Dim InseritoAppezzamento As Boolean = False
            Dim InseritoCampo As Boolean = False

            Dim StrCampo As String
            Dim strXMLAppezzamenti As String
            Dim StrAppezzamento As String = ""
            Dim StrProgetto As String = ""
            Dim StrRegImpianto As String = ""

            Dim strErr As String = ""

            Dim StrParticelle As String = String.Empty
            Dim Ettari As Decimal
            Dim Are As Decimal
            Dim Centiare As Decimal



            Dim objXML As New AgronicaCoreXML.AnagrafeXML
            Dim objXMLPA2 As New AgronicaCoreXML.XML_Privato_Anagrafe2

            '----- Calcolo i valori di BaseCode e TopCode
            UtilityProvider.Calcola_BaseCode_TopCode(BaseCode, TopCode, ProgressivoGIAS)

            Dim objProgetto_W As New AgronicaCoreAnagrafeBIZ.Progetto_W
            Dim objReg_Impianto_W As New AgronicaCoreAnagrafeBIZ.Reg_Impianto_W
            Dim objAppezzamento_W As New AgronicaCoreAnagrafeBIZ.Appezzamento_W
            Dim objCampo_W As New AgronicaCoreAnagrafeBIZ.Campo_W

            Dim objAppezzamento_Write As New AgronicaCoreAnagrafeDAL.Appezzamento_Write
            Dim objAppezzaxParticelle_W As New AgronicaCoreAnagrafeDAL.AppezzaxParticelle_W
            Dim objReg_Impianti_Write As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Write
            Dim objImpresa_Progetti_W As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_W
            Dim objReg_Impianti_Codici_W As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_W

            Dim objParticelle_R As New AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R

            Dim objZonexParticelle_R As New AgronicaCoreAnagrafeDAL.ZonexParticelle_R
            Dim objZonexParticelle_W As New AgronicaCoreAnagrafeDAL.ZonexParticelle_W
            Dim objSpecieVegetali As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
            Dim objIstat As New AgronicaCoreMetaSchemaDAL.Istat_R

            Dim XmlDocAppoggio As Xml.XmlDocument

            Dim XmlAppezzamento As Xml.XmlElement

            Dim XML_DatiParticelle As Xml.XmlElement

            Dim Regolamento_Concimazioni_Cod As Integer = CInt(Cmb_Regolamento_Value)



            Dim strErrore As String = ""

            '------------------------------
            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                           FlagTransazioneLocale,
                                                           objParametri_Server)



            'verifico (se si sceglie di ribaltare le operazioni)
            'se sono stati scelti tutti gli app coinvolti
            Dim strProgrammazione_Entita_Cod As String
            Dim ArrayProgrammazione_Entita_Cod As Integer()
            Dim a As Integer = 0

            Dim AlmenoUno As Boolean = False

            For i = 0 To obj_Dati.Length - 1
                AlmenoUno = True

                Key_Entita = CInt(obj_Dati(i).Programmazione_Entita_Cod)

                ReDim Preserve ArrayProgrammazione_Entita_Cod(a)
                ArrayProgrammazione_Entita_Cod(a) = Key_Entita
                a += 1

                If Validita_inizio_Min Is Nothing Then
                    Validita_inizio_Min = obj_Dati(i).Validita_Inizio
                End If

                If IsDate(obj_Dati(i).Validita_Inizio) AndAlso CDate(obj_Dati(i).Validita_Inizio) < Validita_inizio_Min Then
                    Validita_inizio_Min = obj_Dati(i).Validita_Inizio
                End If

                If IsDate(obj_Dati(i).Validita_Inizio_Impianto) AndAlso obj_Dati(i).Validita_Inizio_Impianto <> AGRODATAINIZIO AndAlso CDate(obj_Dati(i).Validita_Inizio_Impianto) < Validita_inizio_Min Then
                    Validita_inizio_Min = obj_Dati(i).Validita_Inizio_Impianto
                End If

                If Validita_Fine_Max Is Nothing Then
                    Validita_Fine_Max = obj_Dati(i).Validita_Fine
                End If

                If IsDate(obj_Dati(i).Validita_Fine) AndAlso CDate(obj_Dati(i).Validita_Fine) > Validita_Fine_Max Then
                    Validita_Fine_Max = obj_Dati(i).Validita_Fine
                End If

                strProgrammazione_Entita_Cod &= Key_Entita & ","


            Next

            If Not AlmenoUno Then
                r.RispostaOK = True
                r.RispostaStringa = "Selezionare almeno un appezzamento!"
                Return r
            End If

            Dim HashOperazioni As New Hashtable

            'estraggo le operazioni fatte sulle entita selezionate
            If strProgrammazione_Entita_Cod <> "" Then

                Dim objRicetteDestinazioni As New AgronicaCoreContabDAL.Ricette_Destinazioni_R
                Dim DtRicetteDest As DataTable

                DtRicetteDest = objRicetteDestinazioni.Leggi_Operazioni_Da_Programmazione_Entita_Cod(0, 0, 0, 0,
                                                                                                     0,
                                                                                                     AGRODATAINIZIO,
                                                                                                     AGRODATAFINE,
                                                                                                     " Ricette_Destinazioni.Programmazione_Entita_Cod IN (" & Left(strProgrammazione_Entita_Cod, strProgrammazione_Entita_Cod.Length - 1) & ") ",
                                                                                                     "",
                                                                                                     objParametri_Server)

                For j = 0 To DtRicetteDest.Rows.Count - 1
                    If Not HashOperazioni.ContainsKey(DtRicetteDest.Rows(j).Item("Ricetta_Operazione_Cod")) Then
                        HashOperazioni.Add(DtRicetteDest.Rows(j).Item("Ricetta_Operazione_Cod"), "")
                    End If
                Next

            End If

            Dim EntitaOK As Boolean = False

            'verifico che le operazioni non coinvolgano entita non selezionate
            For Each key In HashOperazioni.Keys

                Dim objRicetteDestinazioni As New AgronicaCoreContabDAL.Ricette_Destinazioni_R
                Dim DtRicetteDest As DataTable

                DtRicetteDest = objRicetteDestinazioni.Leggi(0, key, 0, 0, 0,
                                                             "", 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE,
                                                             enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                             " Ricette_Destinazioni.Programmazione_Entita_Cod<>0 ", "", objParametri_Server)
                For i = 0 To DtRicetteDest.Rows.Count - 1
                    EntitaOK = False
                    For a = 0 To ArrayProgrammazione_Entita_Cod.Length - 1
                        If ArrayProgrammazione_Entita_Cod(a) = DtRicetteDest.Rows(i).Item("Programmazione_Entita_Cod") Then
                            EntitaOK = True
                        End If
                    Next
                    If Not EntitaOK Then
                        r.RispostaOK = True
                        r.RispostaStringa = "ATTENZIONE!! Esistono operazioni registrate NON solo sugli appezzamenti selezionati! Occorre selezionare tutti quelli coinvolti!"
                        Return r
                    End If
                Next

            Next

            '----------------------------------------------------------
            ' ELIMINO GLI APPEZZAMENTI (reali) CHIUSI, FRAZIONATI E UNITI

            If obj_Dati_Chiusi IsNot Nothing Then
                For i = 0 To obj_Dati_Chiusi.Length - 1

                    Operazione_Cod = obj_Dati_Chiusi(i).Operazione_Cod

                    Key_Piva = obj_Dati_Chiusi(i).Piva
                    Key_SaCod = CInt(obj_Dati_Chiusi(i).Sa_Cod)
                    Key_CampoCod = CInt(obj_Dati_Chiusi(i).Campo_Cod)
                    Key_Appezza = CInt(obj_Dati_Chiusi(i).Appezza)
                    Key_IdReg = CInt(obj_Dati_Chiusi(i).ID_Reg)
                    Key_ProgettoCod = CInt(obj_Dati_Chiusi(i).Progetto_Cod)
                    Key_Entita = CInt(obj_Dati_Chiusi(i).Programmazione_Entita_Cod)

                    Dim DataChiusura As Date = CDate(paramValidita_Fine)

                    If IsDate(obj_Dati_Chiusi(i).Data_Chiusura) AndAlso CDate(obj_Dati_Chiusi(i).Data_Chiusura) <> AGRODATAINIZIO Then
                        Data_Chiusura = CDate(obj_Dati_Chiusi(i).Data_Chiusura)
                    End If

                    Select Case Operazione_Cod
                        Case enum_TipoOperazioneProgrammazioneEntita.Chiudi_Appezzamento,
                        enum_TipoOperazioneProgrammazioneEntita.Frazionamento,
                        enum_TipoOperazioneProgrammazioneEntita.Unione

                            '-----------------------------
                            ' CHIUDO IL VECCHIO

                            'chiudo la vecchia distinta
                            objImpresa_Progetti_W.AggiornaValiditaFine(Key_Piva, Key_SaCod, Key_Appezza, Key_IdReg, Key_ProgettoCod, 0,
                                                                    CDate(paramValidita_Fine), "", objParametri_Server)

                            'chiudo il vecchio impianto
                            objReg_Impianti_Write.AggiornaValiditaFine(Key_Piva, Key_SaCod, Key_Appezza, 0, Key_IdReg,
                                                                    CDate(paramValidita_Fine), objParametri_Server)

                            'chiudo il vecchio appezzamento
                            objAppezzamento_Write.AggiornaValiditaFine(Key_Piva, Key_SaCod, 0, Key_Appezza,
                                                                    CDate(paramValidita_Fine), "", objParametri_Server)

                            'chiudo le intersezioni appezzamento-particelle
                            objAppezzaxParticelle_W.AggiornaValiditaFine(Key_Piva, Key_SaCod, 0, Key_Appezza,
                                                                        "", "", "", 0, 0, "",
                                                                        CDate(paramValidita_Fine), "", objParametri_Server)

                    End Select


                Next

            End If

            Dim objProgrammazione As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_W

            Dim PrimoApp As Boolean = True
            Dim listaSaCodR As New List(Of String)
            If obj_Dati.Length <> 0 Then
                For Each Appezza_Ribalta In obj_Dati
                    If Appezza_Ribalta.stato_ribaltamento = 1 Then

                        Dim HashParticelle As New Hashtable

                        Operazione_Cod = CInt(Appezza_Ribalta.Operazione_Cod)

                        Key_Piva = Appezza_Ribalta.Piva
                        Key_SaCod = CInt(Appezza_Ribalta.Sa_Cod)
                        Key_CampoCod = CInt(Appezza_Ribalta.Campo_Cod)
                        Key_Appezza = CInt(Appezza_Ribalta.Appezza)
                        Key_IdReg = CInt(Appezza_Ribalta.Id_Reg)
                        Key_ProgettoCod = CInt(Appezza_Ribalta.Progetto_Cod)

                        Key_Piva_Old = Appezza_Ribalta.Piva_old

                        Key_SaCod_Old = CInt(Appezza_Ribalta.Sa_Cod_old)
                        Key_CampoCod_Old = CInt(Appezza_Ribalta.Campo_Cod_old)
                        Key_Appezza_Old = CInt(Appezza_Ribalta.Appezza_old)
                        Key_IdReg_Old = CInt(Appezza_Ribalta.Id_Reg_old)
                        Key_ProgettoCod_Old = CInt(Appezza_Ribalta.Progetto_cod_old)

                        key_programmazione_cod = CInt(Appezza_Ribalta.Programmazione_Cod)
                        Key_Entita = CInt(Appezza_Ribalta.Programmazione_Entita_Cod)

                        If IsNumeric(Appezza_Ribalta.Ribaltato) Then
                            Ribaltato = CInt(Appezza_Ribalta.Ribaltato)
                        Else
                            Ribaltato = 0
                        End If

                        Codice_fiscale_tecnico = CStr(Appezza_Ribalta.Codice_Fiscale_Tecnico)

                        Validita_Inizio = CDate(Appezza_Ribalta.Validita_Inizio)
                        Validita_Fine = CDate(Appezza_Ribalta.Validita_Fine)
                        Validita_Fine_Appezzamento = CDate(Appezza_Ribalta.Validita_Fine)

                        If IsDate(Appezza_Ribalta.Validita_Inizio_Impianto) AndAlso Appezza_Ribalta.Validita_Inizio_Impianto <> AGRODATAINIZIO Then
                            Validita_Inizio_Impianto = CDate(Appezza_Ribalta.Validita_Inizio_Impianto)
                        Else
                            Validita_Inizio_Impianto = Validita_Inizio
                        End If
                        'If Validita_Inizio_Impianto = AGRODATAINIZIO Then
                        '    Validita_Inizio_Impianto = Validita_Inizio
                        'End If

                        Gru_Cod = 0
                        If IsNumeric(Appezza_Ribalta.Gru_Cod) Then
                            Gru_Cod = CDbl(Appezza_Ribalta.Gru_Cod)
                        End If

                        If Gru_Cod = 1 AndAlso Not Chk_Chiudi_Arboree Then
                            Validita_Fine_Appezzamento = AGRODATAFINE
                        End If


                        Id_Cod = CInt(Appezza_Ribalta.Id_Cod)
                        Veg_Cod = CInt(Appezza_Ribalta.Veg_Cod)
                        Cul_Cod = CInt(Appezza_Ribalta.Cul_Cod)
                        Grfi_Cod = CInt(Appezza_Ribalta.Grfi_Cod)
                        Grva_Cod = CInt(Appezza_Ribalta.Grva_Cod)

                        Imp_Cod = CInt(Appezza_Ribalta.Imp_Cod)
                        Foral_Cod = CInt(Appezza_Ribalta.Foral_Cod)

                        Data_Chiusura = DateAdd(DateInterval.Day, -1, Validita_Inizio)

                        App_Nome = Appezza_Ribalta.App_Nome
                        Progetto_Nome = Appezza_Ribalta.Progetto_Nome

                        Veg_Cod_Cliente = Appezza_Ribalta.Veg_Cod_Cliente
                        Cul_Cod_Cliente = Appezza_Ribalta.Cul_Cod_Cliente

                        'VERIFICARE!
                        If Veg_Cod_Cliente = "&nbsp;" Then
                            Veg_Cod_Cliente = ""
                        End If

                        If Cul_Cod_Cliente = "&nbsp;" Then
                            Cul_Cod_Cliente = ""
                        End If

                        Sup_App = 0
                        If IsNumeric(Appezza_Ribalta.Sup_App) Then
                            Sup_App = CDbl(Appezza_Ribalta.Sup_App)
                        End If

                        Resa = 0
                        If IsNumeric(Appezza_Ribalta.Resa) Then
                            Resa = CDbl(Appezza_Ribalta.Resa)
                        End If

                        Unita_Vitata = 0
                        If IsNumeric(Appezza_Ribalta.Unita_Vitata) Then
                            Unita_Vitata = CDbl(Appezza_Ribalta.Unita_Vitata)
                        End If

                        Stato_Impianto = 102

                        'If Not IsNothing(Appezza_Ribalta.Fase_Ciclo_Colturale_ID) AndAlso CType(GridView_Appezzamenti.Rows(i).FindControl("Cmb_Fase"), WebControls.DropDownList).Items.Count > 0 Then
                        If Not IsNothing(Appezza_Ribalta.Fase_Ciclo_Colturale_ID) Then
                            Stato_Impianto = Appezza_Ribalta.Fase_Ciclo_Colturale_ID
                        End If

                        Dim ValLimiteN As String = String.Empty
                        ValLimiteN = Appezza_Ribalta.LimiteMaxN

                        Salva_Limite_N = False

                        If ValLimiteN <> String.Empty Then
                            ' Rbl_Limiti.SelectedValue = 1 -> ribalta i limiti per gli appezzamenti in ZVN
                            ' Rbl_Limiti.SelectedValue = 2 -> ribalta tutti i limiti
                            Dim limiti As String = ""
                            'limiti = Cmb_Limiti.SelectedValue
                            Select Case limiti
                                Case "2"
                                    Salva_Limite_N = True
                                Case "1"
                                    ' controllo se l'appezzamento è in zvn
                                    Dim InZvn As Boolean = False
                                    objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(Validita_Inizio, Validita_Inizio)
                                    InZvn = objZonexParticelle_R.AppezzamentoInZona(Key_Piva, Key_SaCod, Key_Appezza,
                                                                                    enum_Zone.ZVN,
                                                                                    "", objParametri_Server)
                                    objParametri_Server.ResettaFinestra()
                                    If InZvn Then
                                        Salva_Limite_N = True
                                    End If
                            End Select

                        End If

                        Cop_Cod = 0
                        If IsNumeric(Appezza_Ribalta.Cop_Cod) Then
                            Cop_Cod = Appezza_Ribalta.Cop_Cod
                        End If

                        Num_Piante = 0
                        If IsNumeric(Appezza_Ribalta.Num_Piante) Then
                            Num_Piante = Appezza_Ribalta.Num_Piante
                        End If

                        TRA_Fila = 0
                        If IsNumeric(Appezza_Ribalta.TRA_Fila) Then
                            TRA_Fila = Appezza_Ribalta.TRA_Fila
                        End If

                        SU_Fila = 0
                        If IsNumeric(Appezza_Ribalta.SU_Fila) Then
                            SU_Fila = Appezza_Ribalta.SU_Fila
                        End If

                        Metodo_Produzione_Cod = 0
                        If IsNumeric(Appezza_Ribalta.MetodoProduzione_Cod) Then
                            Metodo_Produzione_Cod = Appezza_Ribalta.MetodoProduzione_Cod
                        End If

                        Disciplinare_Cod = 0
                        If IsNumeric(Appezza_Ribalta.Disciplinare_Cod) Then
                            Disciplinare_Cod = Appezza_Ribalta.Disciplinare_Cod
                        End If

                        Regolamento_Concimazioni_Cod = CInt(Cmb_Regolamento_Value)
                        If Regolamento_Concimazioni_Cod = 0 Then
                            If IsNumeric(Appezza_Ribalta.Regolamento_Cod) Then
                                Regolamento_Concimazioni_Cod = Appezza_Ribalta.Regolamento_Concimazione_Cod
                            End If
                        End If

                        Regolamento_Cod = 0
                        If IsNumeric(Appezza_Ribalta.Regolamento_Cod) Then
                            Regolamento_Cod = Appezza_Ribalta.Regolamento_Cod
                        End If

                        If (Regolamento_Cod = 0) AndAlso Cul_Cod <> 0 Then
                            Regolamento_Cod = 1
                        End If

                        Flag_PubblicoPrivato = 0
                        If IsNumeric(Appezza_Ribalta.Flag_PubblicoPrivato) Then
                            Flag_PubblicoPrivato = Appezza_Ribalta.Flag_PubblicoPrivato
                        End If

                        Id_tr = 0
                        If IsNumeric(Appezza_Ribalta.Id_tr) Then
                            Id_tr = Appezza_Ribalta.Id_tr
                        End If

                        Limite_N = Appezza_Ribalta.Limite_N
                        Limite_P = Appezza_Ribalta.Limite_P
                        Limite_K = Appezza_Ribalta.Limite_K

                        Veg_Cod_Prec = ""
                        If IsNumeric(Appezza_Ribalta.Veg_Cod_Prec) AndAlso CInt(Appezza_Ribalta.Veg_Cod_Prec) <> 0 Then
                            Veg_Cod_Prec = Appezza_Ribalta.Veg_Cod_Prec & "|" & objSpecieVegetali.Leggi(Appezza_Ribalta.Veg_Cod_Prec, 0, "", "", 1, "", "", objParametri_Server).Rows(0).Item("Gru_Cod")
                        End If

                        Veg_Cod_Prec2 = ""
                        If IsNumeric(Appezza_Ribalta.Veg_Cod_Prec2) AndAlso CInt(Appezza_Ribalta.Veg_Cod_Prec2) <> 0 Then
                            Veg_Cod_Prec2 = Appezza_Ribalta.Veg_Cod_Prec2 & "|" & objSpecieVegetali.Leggi(Appezza_Ribalta.Veg_Cod_Prec2, 0, "", "", 1, "", "", objParametri_Server).Rows(0).Item("Gru_Cod")
                        End If

                        Veg_Cod_Prec3 = ""
                        If IsNumeric(Appezza_Ribalta.Veg_Cod_Prec3) AndAlso CInt(Appezza_Ribalta.Veg_Cod_Prec3) <> 0 Then
                            Veg_Cod_Prec3 = Appezza_Ribalta.Veg_Cod_Prec3 & "|" & objSpecieVegetali.Leggi(Appezza_Ribalta.Veg_Cod_Prec3, 0, "", "", 1, "", "", objParametri_Server).Rows(0).Item("Gru_Cod")
                        End If

                        Veg_Cod_Prec4 = ""
                        If IsNumeric(Appezza_Ribalta.Veg_Cod_Prec4) AndAlso CInt(Appezza_Ribalta.Veg_Cod_Prec4) <> 0 Then
                            Veg_Cod_Prec4 = Appezza_Ribalta.Veg_Cod_Prec4 & "|" & objSpecieVegetali.Leggi(Appezza_Ribalta.Veg_Cod_Prec4, 0, "", "", 1, "", "", objParametri_Server).Rows(0).Item("Gru_Cod")
                        End If

                        Piano_Semina = Appezza_Ribalta.Piano_Semina
                        Codice_Contratto = Appezza_Ribalta.Codice_Contratto

                        Data_Semina = AGRODATAINIZIO
                        If IsDate(Appezza_Ribalta.Data_Semina) AndAlso Appezza_Ribalta.Data_Semina <> AGRODATAINIZIO Then
                            Data_Semina = CDate(Appezza_Ribalta.Data_Semina)
                        End If

                        Data_Raccolta = AGRODATAFINE
                        If IsDate(Appezza_Ribalta.Data_Raccolta) AndAlso Appezza_Ribalta.Data_Raccolta <> AGRODATAFINE Then
                            Data_Raccolta = CDate(Appezza_Ribalta.Data_Raccolta)
                        End If

                        Data_Fioritura_Prevista = AGRODATAINIZIO
                        If IsDate(Appezza_Ribalta.Data_Fioritura_Prevista) AndAlso Appezza_Ribalta.Data_Fioritura_Prevista <> AGRODATAINIZIO Then
                            Data_Fioritura_Prevista = CDate(Appezza_Ribalta.Data_Fioritura_Prevista)
                        End If

                        If Blocca_Appezza Then
                            Blk_Flag = -1
                        End If

                        IAF = ""
                        If Appezza_Ribalta.IAF IsNot Nothing Then
                            IAF = Appezza_Ribalta.IAF
                        End If

                        Pratica_Cod = ""
                        If Appezza_Ribalta.Pratica_Cod IsNot Nothing Then
                            Pratica_Cod = CStr(Appezza_Ribalta.Pratica_Cod)
                        End If

                        DistBZ_CorpiIdrici = 0
                        If Appezza_Ribalta.DistBZ_CorpiIdrici IsNot Nothing Then
                            DistBZ_CorpiIdrici = CDbl(Appezza_Ribalta.DistBZ_CorpiIdrici)
                        End If

                        DistBZ_AreeResPub = 0
                        If Appezza_Ribalta.DistBZ_AreeResPub IsNot Nothing Then
                            DistBZ_AreeResPub = CDbl(Appezza_Ribalta.DistBZ_AreeResPub)
                        End If

                        DistBZ_Allevamenti = 0
                        If Appezza_Ribalta.DistBZ_Allevamenti IsNot Nothing Then
                            DistBZ_Allevamenti = CDbl(Appezza_Ribalta.DistBZ_Allevamenti)
                        End If

                        DistBZ_VegNatNonColt = 0
                        If Appezza_Ribalta.DistBZ_VegNatNonColt IsNot Nothing Then
                            DistBZ_VegNatNonColt = CDbl(Appezza_Ribalta.DistBZ_VegNatNonColt)
                        End If

                        SupBZ_Riduzione = 0
                        If Appezza_Ribalta.SupBZ_Riduzione IsNot Nothing Then
                            SupBZ_Riduzione = CDbl(Appezza_Ribalta.SupBZ_Riduzione)
                        End If

                        Riferimento_Alfanumerico_Appezzamento = ""
                        If Appezza_Ribalta.Riferimento_Alfanumerico_Appezzamento IsNot Nothing Then
                            Riferimento_Alfanumerico_Appezzamento = Appezza_Ribalta.Riferimento_Alfanumerico_Appezzamento
                        End If

                        Isola = ""
                        If Appezza_Ribalta.Isola IsNot Nothing Then
                            Isola = Appezza_Ribalta.Isola
                        End If

                        CapitolatoPrivato = ""
                        If Appezza_Ribalta.CapitolatoPrivato IsNot Nothing Then
                            CapitolatoPrivato = Appezza_Ribalta.CapitolatoPrivato
                        End If

                        Finalita_Concimazione_Impianto = 0
                        If Appezza_Ribalta.Finalita_Concimazione_Impianto IsNot Nothing Then
                            Finalita_Concimazione_Impianto = Appezza_Ribalta.Finalita_Concimazione_Impianto
                        End If

                        Cod_Indirizzo = 0
                        If Appezza_Ribalta.Cod_Indirizzo IsNot Nothing Then
                            Cod_Indirizzo = Appezza_Ribalta.Cod_Indirizzo
                        End If

                        ind_des = ""
                        If Appezza_Ribalta.ind_des IsNot Nothing Then
                            ind_des = Appezza_Ribalta.ind_des
                        End If

                        frz_des = ""
                        If Appezza_Ribalta.frz_des IsNot Nothing Then
                            frz_des = Appezza_Ribalta.frz_des
                        End If

                        CAP = ""
                        If Appezza_Ribalta.CAP IsNot Nothing Then
                            CAP = Appezza_Ribalta.CAP
                        End If

                        com_des_indirizzo = ""
                        If Appezza_Ribalta.com_des_indirizzo IsNot Nothing Then
                            com_des_indirizzo = Appezza_Ribalta.com_des_indirizzo
                        End If

                        pro_cod_indirizzo = ""
                        If Appezza_Ribalta.pro_cod_indirizzo IsNot Nothing Then
                            pro_cod_indirizzo = Appezza_Ribalta.pro_cod_indirizzo
                        End If

                        stato_indirizzo = ""
                        If Appezza_Ribalta.stato_indirizzo IsNot Nothing Then
                            stato_indirizzo = Appezza_Ribalta.stato_indirizzo
                        End If

                        stato_indirizzo_des = ""
                        If Appezza_Ribalta.stato_indirizzo_des IsNot Nothing Then
                            stato_indirizzo_des = Appezza_Ribalta.stato_indirizzo_des
                        End If

                        note_indirizzo = ""
                        If Appezza_Ribalta.note_indirizzo IsNot Nothing Then
                            note_indirizzo = Appezza_Ribalta.note_indirizzo
                        End If

                        pro_cod_istat_indirizzo = ""
                        If Appezza_Ribalta.pro_cod_istat_indirizzo IsNot Nothing Then
                            pro_cod_istat_indirizzo = Appezza_Ribalta.pro_cod_istat_indirizzo
                        End If

                        com_cod_istat_indirizzo = ""
                        If Appezza_Ribalta.com_cod_istat_indirizzo IsNot Nothing Then
                            com_cod_istat_indirizzo = Appezza_Ribalta.com_cod_istat_indirizzo
                        End If

                        KPIN = ""
                        If Appezza_Ribalta.KPIN IsNot Nothing AndAlso Appezza_Ribalta.KPIN <> "" Then
                            KPIN = Appezza_Ribalta.KPIN
                        End If

                        Block_Name = ""
                        If Appezza_Ribalta.Block_Name IsNot Nothing AndAlso Appezza_Ribalta.Block_Name <> "" Then
                            Block_Name = Appezza_Ribalta.Block_Name
                        End If

                        Data_Inizio_Portinnesto = AGRODATAINIZIO
                        If Appezza_Ribalta.Data_Inizio_Portinnesto IsNot Nothing AndAlso Appezza_Ribalta.Data_Inizio_Portinnesto <> "" AndAlso IsDate(Appezza_Ribalta.Data_Inizio_Portinnesto) Then
                            Data_Inizio_Portinnesto = CDate(Appezza_Ribalta.Data_Inizio_Portinnesto)
                        End If

                        Mat_Cod = 0
                        If IsNumeric(Appezza_Ribalta.Mat_Cod) Then
                            Mat_Cod = Appezza_Ribalta.Mat_Cod
                        End If

                        Select Case Operazione_Cod

                        '-------------------------------------------------------------
                        '--- NUOVA DISTINTA
                        '-------------------------------------------------------------

                            Case enum_TipoOperazioneProgrammazioneEntita.Nuova_Distinta

                                Data_Chiusura = DateAdd(DateInterval.Day, -1, Validita_Inizio)

                                'chiudo la vecchia
                                objImpresa_Progetti_W.AggiornaValiditaFine(Key_Piva_Old, Key_SaCod_Old, Key_Appezza_Old, Key_IdReg_Old, Key_ProgettoCod_Old, 0,
                                                                        Data_Chiusura, "", objParametri_Server)

                                'apro la nuova
                                StrProgetto = ""
                                objXML.Xml_ProgettoPerImpianto(enum_CodificaDecodifica.Codifica,
                                                               StrProgetto,
                                                               enum_TipoOperazioneDB.Scrittura,
                                                               Key_Piva,
                                                               Key_SaCod,
                                                               ZeroInt,
                                                               Progetto_Nome,
                                                               NullString,
                                                               enum_Agenda_Causali.Progetto_Produzione_Agricola,
                                                               ZeroInt,
                                                               ZeroInt,
                                                               Data_Semina,
                                                               Data_Raccolta,
                                                               NullString,
                                                               Key_Appezza,
                                                               Key_IdReg,
                                                               0,
                                                               0,
                                                               Stato_Impianto,
                                                               Regolamento_Cod,
                                                               Disciplinare_Cod,
                                                               Regolamento_Concimazioni_Cod,
                                                               ZeroInt,
                                                               ZeroDecimal,
                                                               Resa,
                                                               Validita_Inizio,
                                                               Validita_Fine,
                                                               BaseCode,
                                                               TopCode,
                                                               Data_Fioritura_Prevista:=Data_Fioritura_Prevista)

                                InseritoProgetto = objProgetto_W.Impresa_Progetto_Scrivi(
                                                                CStr(StrProgetto),
                                                                OUTPUT_Piva,
                                                                OUTPUT_Progetto_Cod,
                                                                objParametri_Server)

                                If InseritoProgetto Then

                                    If Salva_Limite_N Then
                                        objReg_Impianti_Codici_W.ScrivixProgetto(Key_Piva, Key_SaCod, Key_Appezza, Key_IdReg,
                                                                                 OUTPUT_Progetto_Cod,
                                                                                 enum_CodiciAnagrafe.Impianto_LimiteN,
                                                                                 ValLimiteN,
                                                                                 Validita_Inizio, Validita_Fine, objParametri_Server)
                                    End If


                                    '  Vanni, 07/06/2013 14:22:20: ripristinata da Ribalta_OLD() --> Genera_Stringone_XML
                                    If Veg_Cod_Cliente <> "" Then
                                        objReg_Impianti_Codici_W.ScrivixProgetto(Key_Piva, Key_SaCod, Key_Appezza, Key_IdReg, OUTPUT_Progetto_Cod,
                                                                         enum_CodiciAnagrafe.Codice_Specie_Agea,
                                                                         Veg_Cod_Cliente,
                                                                        Validita_Inizio, Validita_Fine, objParametri_Server)
                                    End If

                                    If Cul_Cod_Cliente <> "" Then
                                        objReg_Impianti_Codici_W.ScrivixProgetto(Key_Piva, Key_SaCod, Key_Appezza, Key_IdReg, OUTPUT_Progetto_Cod,
                                                                         enum_CodiciAnagrafe.Codice_Cultivar_Agea,
                                                                         Cul_Cod_Cliente,
                                                                        Validita_Inizio, Validita_Fine, objParametri_Server)
                                    End If

                                    Dim reg_impianti_programmazione_w As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Programmazioni_W
                                    reg_impianti_programmazione_w.Scrivi(
                                        Key_Piva, Key_SaCod, Key_Appezza, Key_IdReg, OUTPUT_Progetto_Cod,
                                        key_programmazione_cod, Key_Entita, Validita_Inizio, Validita_Fine,
                                        objParametri_Server
                                    )

                                End If



                            '-------------------------------------------------------------
                            '--- NUOVO IMPIANTO
                            '-------------------------------------------------------------

                            Case enum_TipoOperazioneProgrammazioneEntita.Nuovo_Impianto

                                Data_Chiusura = DateAdd(DateInterval.Day, -1, Validita_Inizio_Impianto)

                                'chiudo la vecchia distinta
                                objImpresa_Progetti_W.AggiornaValiditaFine(Key_Piva_Old, Key_SaCod_Old, Key_Appezza_Old, Key_IdReg_Old, Key_ProgettoCod_Old, 0,
                                                                        Data_Chiusura, "", objParametri_Server)

                                'chiudo il vecchio impianto
                                objReg_Impianti_Write.AggiornaValiditaFine(Key_Piva_Old, Key_SaCod_Old, Key_Appezza_Old, 0, Key_IdReg_Old,
                                                                        Data_Chiusura, objParametri_Server)

                                StrRegImpianto = String.Empty
                                objXML.XML_Impianto(enum_CodificaDecodifica.Codifica,
                                                    StrRegImpianto,
                                                    enum_TipoOperazioneDB.Scrittura,
                                                    Key_Piva,
                                                    Key_SaCod,
                                                    Key_CampoCod,
                                                    Key_Appezza,
                                                    ZeroInt,
                                                    Sup_App,
                                                    ZeroInt,
                                                    ZeroInt,
                                                    ZeroInt,
                                                    Validita_Inizio_Impianto,
                                                    Cul_Cod,
                                                    Grva_Cod,
                                                    Data_Raccolta,
                                                    Resa,
                                                    ZeroDecimal,
                                                    ZeroInt,
                                                    ZeroInt, ZeroString, NullString, NullString, NullString,
                                                    TRA_Fila,
                                                    SU_Fila,
                                                    ZeroDecimal,
                                                    ZeroString,
                                                    ZeroInt,
                                                    ZeroInt, ZeroInt, ZeroInt, ZeroInt, ZeroInt, ZeroInt, ZeroString, ZeroInt, ZeroInt, ZeroInt, ZeroInt, ZeroInt, Codice_fiscale_tecnico, ZeroString,
                                                    Grfi_Cod,
                                                    Imp_Cod,
                                                    CInt(1),
                                                    ZeroInt,
                                                    ZeroInt,
                                                    Cop_Cod,
                                                    Foral_Cod,
                                                    ZeroInt, ZeroInt,
                                                    Validita_Inizio_Impianto,
                                                    Validita_Fine_Appezzamento,
                                                    BaseCode,
                                                    TopCode,
                                                    Unita_Vitata)

                                StrRegImpianto = "<DatiReg_Impianti>" & StrRegImpianto & "</DatiReg_Impianti>"

                                InseritoImpianto = objReg_Impianto_W.Reg_Impianto_Scrivi(
                                                                CStr(StrRegImpianto),
                                                                OUTPUT_Piva,
                                                                OUTPUT_Sa_Cod,
                                                                OUTPUT_Appezza,
                                                                OUTPUT_Id_Reg,
                                                                "",
                                                                objParametri_Server)

                                If InseritoImpianto Then

                                    '  Vanni, 07/06/2013 16:00:43: creazione non legata a distinta?
                                    '---------------------------------
                                    'caso destinazioni d'uso
                                    Dim strId_Cod As String = ""
                                    If Id_Cod <> 0 Then
                                        objReg_Impianti_Codici_W.Scrivi(Key_Piva, Key_SaCod, Key_Appezza, OUTPUT_Id_Reg,
                                                                        Id_Cod,
                                                                        "",
                                                                        Validita_Inizio, Validita_Fine, objParametri_Server)
                                    End If

                                    If TRA_Fila <> 0 Then
                                        objReg_Impianti_Codici_W.Scrivi(Key_Piva, Key_SaCod, Key_Appezza, OUTPUT_Id_Reg,
                                                                        enum_CodiciAnagrafe.Impianto_TraFila_Maschio,
                                                                        TRA_Fila,
                                                                        Validita_Inizio, Validita_Fine, objParametri_Server)
                                    End If

                                    If SU_Fila <> 0 Then
                                        objReg_Impianti_Codici_W.Scrivi(Key_Piva, Key_SaCod, Key_Appezza, OUTPUT_Id_Reg,
                                                                        enum_CodiciAnagrafe.Impianto_SuFila_Maschio,
                                                                        SU_Fila,
                                                                        Validita_Inizio, Validita_Fine, objParametri_Server)
                                    End If

                                    If Pratica_Cod <> 0 Then
                                        'DRUDI INSERIRE PRATICA
                                        objReg_Impianti_Codici_W.Scrivi(Key_Piva, Key_SaCod, Key_Appezza, OUTPUT_Id_Reg,
                                                                        enum_CodiciAnagrafe.Programmazione_Impianto_Pratica_Cod,
                                                                        CStr(Pratica_Cod),
                                                                        Validita_Inizio, Validita_Fine, objParametri_Server)
                                    End If

                                    'CREO LA DISTINTA
                                    StrProgetto = ""
                                    objXML.Xml_ProgettoPerImpianto(enum_CodificaDecodifica.Codifica,
                                                               StrProgetto,
                                                               enum_TipoOperazioneDB.Scrittura,
                                                               Key_Piva,
                                                               Key_SaCod,
                                                               ZeroInt,
                                                               Progetto_Nome,
                                                               NullString,
                                                               enum_Agenda_Causali.Progetto_Produzione_Agricola,
                                                               ZeroInt,
                                                               ZeroInt,
                                                               Data_Semina,
                                                               Data_Raccolta,
                                                               NullString,
                                                               Key_Appezza,
                                                               OUTPUT_Id_Reg,
                                                               0,
                                                               0,
                                                               Stato_Impianto,
                                                               Regolamento_Cod,
                                                               Disciplinare_Cod,
                                                               Regolamento_Concimazioni_Cod,
                                                               ZeroInt,
                                                               ZeroDecimal,
                                                               Resa,
                                                               Validita_Inizio,
                                                               Validita_Fine,
                                                               BaseCode,
                                                               TopCode,
                                                               Data_Fioritura_Prevista:=Data_Fioritura_Prevista)

                                    '  Vanni, 07/06/2013 16:00:58: poi creo una distinta?
                                    InseritoProgetto = objProgetto_W.Impresa_Progetto_Scrivi(
                                                                    CStr(StrProgetto),
                                                                    OUTPUT_Piva,
                                                                    OUTPUT_Progetto_Cod,
                                                                    objParametri_Server)

                                    If InseritoProgetto Then

                                        If Salva_Limite_N Then
                                            objReg_Impianti_Codici_W.ScrivixProgetto(Key_Piva, Key_SaCod, Key_Appezza, OUTPUT_Id_Reg, OUTPUT_Progetto_Cod,
                                                                                     enum_CodiciAnagrafe.Impianto_LimiteN,
                                                                                     ValLimiteN,
                                                                                     Validita_Inizio, Validita_Fine, objParametri_Server)
                                        Else
                                            If Limite_N <> "" AndAlso Limite_N <> "0" Then
                                                objReg_Impianti_Codici_W.ScrivixProgetto(Key_Piva, Key_SaCod, Key_Appezza, OUTPUT_Id_Reg, OUTPUT_Progetto_Cod,
                                                                                     enum_CodiciAnagrafe.Impianto_LimiteN,
                                                                                     Limite_N,
                                                                                     Validita_Inizio, Validita_Fine, objParametri_Server)
                                            End If

                                            If Limite_P <> "" AndAlso Limite_P <> "0" Then
                                                objReg_Impianti_Codici_W.ScrivixProgetto(Key_Piva, Key_SaCod, Key_Appezza, OUTPUT_Id_Reg, OUTPUT_Progetto_Cod,
                                                                                     enum_CodiciAnagrafe.Impianto_LimiteP,
                                                                                     Limite_P,
                                                                                     Validita_Inizio, Validita_Fine, objParametri_Server)
                                            End If

                                            If Limite_K <> "" AndAlso Limite_K <> "0" Then
                                                objReg_Impianti_Codici_W.ScrivixProgetto(Key_Piva, Key_SaCod, Key_Appezza, OUTPUT_Id_Reg, OUTPUT_Progetto_Cod,
                                                                                     enum_CodiciAnagrafe.Impianto_LimiteK,
                                                                                     Limite_K,
                                                                                     Validita_Inizio, Validita_Fine, objParametri_Server)
                                            End If
                                        End If

                                        If Piano_Semina <> "" Then
                                            objReg_Impianti_Codici_W.ScrivixProgetto(Key_Piva, Key_SaCod, Key_Appezza, OUTPUT_Id_Reg, OUTPUT_Progetto_Cod,
                                                                                     enum_CodiciAnagrafe.Impianto_PianoSemina,
                                                                                     Piano_Semina,
                                                                                     Validita_Inizio, Validita_Fine, objParametri_Server)
                                        End If

                                        '  Vanni, 07/06/2013 14:22:20: ripristinata da Ribalta_OLD() --> Genera_Stringone_XML
                                        If Veg_Cod_Cliente <> "" Then
                                            objReg_Impianti_Codici_W.ScrivixProgetto(Key_Piva, Key_SaCod, Key_Appezza, OUTPUT_Id_Reg, OUTPUT_Progetto_Cod,
                                                                             enum_CodiciAnagrafe.Codice_Specie_Agea,
                                                                             Veg_Cod_Cliente,
                                                                            Validita_Inizio, Validita_Fine, objParametri_Server)
                                        End If

                                        If Cul_Cod_Cliente <> "" Then
                                            objReg_Impianti_Codici_W.ScrivixProgetto(Key_Piva, Key_SaCod, Key_Appezza, OUTPUT_Id_Reg, OUTPUT_Progetto_Cod,
                                                                             enum_CodiciAnagrafe.Codice_Cultivar_Agea,
                                                                             Cul_Cod_Cliente,
                                                                            Validita_Inizio, Validita_Fine, objParametri_Server)
                                        End If

                                        Dim reg_impianti_programmazione_w As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Programmazioni_W
                                        reg_impianti_programmazione_w.Scrivi(
                                            Key_Piva, Key_SaCod, Key_Appezza, OUTPUT_Id_Reg, OUTPUT_Progetto_Cod,
                                            key_programmazione_cod, Key_Entita, Validita_Inizio, Validita_Fine,
                                            objParametri_Server
                                        )


                                    End If

                                End If


                            '-------------------------------------------------------------
                            '--- MODIFICA SUP APPEZZAMENTO
                            '-------------------------------------------------------------

                            Case enum_TipoOperazioneProgrammazioneEntita.Modifica_Superficie

                                '-----------------------------
                                ' CHIUDO IL VECCHIO

                                'chiudo la vecchia distinta
                                objImpresa_Progetti_W.AggiornaValiditaFine(Key_Piva_Old, Key_SaCod_Old, Key_Appezza_Old, Key_IdReg_Old, Key_ProgettoCod_Old, 0,
                                                                        Data_Chiusura, "", objParametri_Server)

                                'chiudo il vecchio impianto
                                objReg_Impianti_Write.AggiornaValiditaFine(Key_Piva_Old, Key_SaCod_Old, Key_Appezza_Old, 0, Key_IdReg_Old,
                                                                        Data_Chiusura, objParametri_Server)

                                'chiudo il vecchio appezzamento
                                objAppezzamento_Write.AggiornaValiditaFine(Key_Piva_Old, Key_SaCod_Old, 0, Key_Appezza_Old,
                                                                        Data_Chiusura, "", objParametri_Server)

                                'chiudo le intersezioni appezzamento-particelle
                                objAppezzaxParticelle_W.AggiornaValiditaFine(Key_Piva_Old, Key_SaCod_Old, 0, Key_Appezza_Old,
                                                                            "", "", "", 0, 0, "",
                                                                            Data_Chiusura, "", objParametri_Server)

                                '-----------------------------
                                ' APRO IL NUOVO

                                StrAppezzamento = ""
                                objXML.XML_Appezzamento(enum_CodificaDecodifica.Codifica,
                                                        StrAppezzamento,
                                                        enum_TipoOperazioneDB.Scrittura,
                                                        Key_Piva,
                                                        Key_SaCod,
                                                        ZeroInt,
                                                        Sup_App,
                                                        ZeroData,
                                                        ZeroData,
                                                        ZeroInt,
                                                        ZeroInt,
                                                        ZeroInt,
                                                        PuntoString,
                                                        ZeroInt,
                                                        PuntoString,
                                                        ZeroInt,
                                                        NullString,
                                                        ZeroDecimal,
                                                        ZeroDecimal,
                                                        ZeroDecimal,
                                                        ZeroDecimal,
                                                        ZeroDecimal,
                                                        ZeroDecimal,
                                                        ZeroDecimal,
                                                        ZeroDecimal,
                                                        ZeroDecimal,
                                                        ZeroDecimal,
                                                        ZeroDecimal,
                                                        ZeroDecimal,
                                                        ZeroString,
                                                        ZeroInt,
                                                        ZeroDecimal,
                                                        ZeroData,
                                                        ZeroDecimal,
                                                        ZeroData,
                                                        ZeroData,
                                                        ZeroDecimal,
                                                        ZeroData,
                                                        NullString,
                                                        App_Nome,
                                                        ZeroInt,
                                                        ZeroDecimal,
                                                        NullString,
                                                        Key_CampoCod,
                                                        ZeroInt,
                                                        ZeroData,
                                                        ZeroData,
                                                        Validita_Inizio_Impianto,
                                                        Validita_Fine_Appezzamento,
                                                        BaseCode,
                                                        TopCode,
                                                        DistBZ_CorpiIdrici:=DistBZ_CorpiIdrici,
                                                        DistBZ_AreeResPub:=DistBZ_AreeResPub,
                                                        DistBZ_Allevamenti:=DistBZ_Allevamenti,
                                                        DistBZ_VegNatNonColt:=DistBZ_VegNatNonColt,
                                                        SupBZ_Riduzione:=SupBZ_Riduzione)


                                XmlDocAppoggio = New Xml.XmlDocument
                                XmlDocAppoggio.LoadXml(StrAppezzamento)

                                XmlAppezzamento = XmlDocAppoggio.SelectSingleNode("Appezzamento")

                                Dim Str_CodiceAppezzamento As String = ""
                                objXML.XML_Codice(enum_CodificaDecodifica.Codifica,
                                                  Str_CodiceAppezzamento,
                                                  enum_TipoOperazioneDB.Scrittura,
                                                  enum_CodiciAnagrafe.TitoloPossesso,
                                                  1,
                                                  Validita_Inizio,
                                                  Validita_Fine,
                                                  BaseCode,
                                                  TopCode, "Appezzamento")

                                Dim Str_AppezzamentoxIndirizzi As String = ""
                                Dim Cod_indirizzo_new As Integer = 0
                                If Cod_Indirizzo <> 0 Then

                                    Dim siglaProv = ""
                                    If stato_indirizzo = "IT" Then
                                        objIstat.Provincia_from_CodIstat(pro_cod_istat_indirizzo, siglaProv, objParametri_Server)
                                    End If

                                    Dim strIndirizzo As String = ""
                                    objXML.XML_Indirizzo(enum_CodificaDecodifica.Codifica,
                                                                            strIndirizzo,
                                                                            enum_TipoOperazioneDB.Scrittura,
                                                                            1,
                                                                            0,
                                                                            ind_des,
                                                                            frz_des,
                                                                            CAP,
                                                                            com_des_indirizzo,
                                                                            siglaProv,
                                                                            stato_indirizzo,
                                                                            note_indirizzo,
                                                                            pro_cod_istat_indirizzo,
                                                                            com_cod_istat_indirizzo,
                                                                            AGRODATAINIZIO,
                                                                            AGRODATAFINE,
                                                                            BaseCode,
                                                                            TopCode)

                                    Str_AppezzamentoxIndirizzi &= strIndirizzo

                                End If

                                If Str_AppezzamentoxIndirizzi <> "" Then
                                    XmlAppezzamento.InnerXml &= Str_AppezzamentoxIndirizzi
                                End If

                                Dim DT_Particelle As New DataTable
                                Dim objPP As New AgronicaCoreAnagrafeDAL.Programmazione_Particelle_R
                                DT_Particelle = objPP.Programmazione_Particelle_Leggi_4(objParametri_Server,
                                                                                      Key_Piva,
                                                                                      strErr,
                                                                                      Key_Entita)
                                StrParticelle = String.Empty


                                For j = 0 To DT_Particelle.Rows.Count - 1

                                    If Not HashParticelle.ContainsKey(DT_Particelle.Rows(j).Item("PROV") & "_" & DT_Particelle.Rows(j).Item("COM") & "_" & DT_Particelle.Rows(j).Item("Sezione") & "_" & DT_Particelle.Rows(j).Item("Foglio") & "_" & DT_Particelle.Rows(j).Item("Numero") & "_" & DT_Particelle.Rows(j).Item("Subalterno")) Then

                                        HashParticelle.Add(DT_Particelle.Rows(j).Item("PROV") & "_" & DT_Particelle.Rows(j).Item("COM") & "_" & DT_Particelle.Rows(j).Item("Sezione") & "_" & DT_Particelle.Rows(j).Item("Foglio") & "_" & DT_Particelle.Rows(j).Item("Numero") & "_" & DT_Particelle.Rows(j).Item("Subalterno"), "")

                                        Conversioni.EttariAreCentiare_from_Ettari(DT_Particelle.Rows(j).Item("Superficie"), Ettari, Are, Centiare)

                                        StrParticelle &= objXML.XML_AppezzamentoParticella(enum_TipoOperazioneDB.Scrittura,
                                                                                          Key_Piva,
                                                                                          Key_SaCod,
                                                                                          ZeroInt,
                                                                                          DT_Particelle.Rows(j).Item("PROV"),
                                                                                          DT_Particelle.Rows(j).Item("COM"),
                                                                                          DT_Particelle.Rows(j).Item("Sezione"),
                                                                                          DT_Particelle.Rows(j).Item("foglio"),
                                                                                          DT_Particelle.Rows(j).Item("numero"),
                                                                                          DT_Particelle.Rows(j).Item("subalterno"),
                                                                                          DT_Particelle.Rows(j).Item("Superficie"),
                                                                                          Ettari,
                                                                                          Are,
                                                                                          Centiare,
                                                                                          ZeroInt, ZeroInt, ZeroInt, ZeroInt, ZeroInt, ZeroInt,
                                                                                          Validita_Inizio_Impianto,
                                                                                          Validita_Fine_Appezzamento,
                                                                                          BaseCode,
                                                                                          TopCode)



                                    End If

                                Next

                                XmlAppezzamento.InnerXml &= Str_CodiceAppezzamento

                                If StrParticelle <> "" Then
                                    XML_DatiParticelle = XmlAppezzamento.OwnerDocument.CreateElement("DatiParticelle")
                                    XmlAppezzamento.AppendChild(XML_DatiParticelle)
                                    XML_DatiParticelle.InnerXml = StrParticelle
                                End If

                                strXMLAppezzamenti = "<DatiAppezzamenti>" & XmlAppezzamento.OuterXml & "</DatiAppezzamenti>"

                                InseritoAppezzamento = objAppezzamento_W.Appezzamento_Scrivi(strXMLAppezzamenti,
                                                                                            OUTPUT_Piva,
                                                                                            OUTPUT_Sa_Cod,
                                                                                            OUTPUT_Appezza,
                                                                                            objParametri_Server,
                                                                                            objParametri_Utenti)

                                If InseritoAppezzamento Then
                                    If Blocca_Appezza Then
                                        Dim objAppezza As New AgronicaCoreAnagrafeDAL.Appezzamento_Write
                                        objAppezza.Appezza_Blocca(OUTPUT_Piva, OUTPUT_Sa_Cod, OUTPUT_Appezza, "", objParametri_Server)
                                    End If

                                    'APPEZZAMENTO CODICI
                                    Dim objAppezzaCodici As New AgronicaCoreAnagrafeDAL.Appezzamento_Codici_W
                                    If Metodo_Produzione_Cod <> 0 Then
                                        objAppezzaCodici.Scrivi(Key_Piva, Key_SaCod, OUTPUT_Appezza, enum_CodiciAnagrafe.MetodoDiProduzione, Metodo_Produzione_Cod, Validita_Inizio_Impianto, Validita_Fine_Appezzamento, objParametri_Server)
                                    End If
                                    If Veg_Cod_Prec <> "" Then
                                        objAppezzaCodici.Scrivi(Key_Piva, Key_SaCod, OUTPUT_Appezza, enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_1, Veg_Cod_Prec, Validita_Inizio_Impianto, Validita_Fine_Appezzamento, objParametri_Server)
                                    End If
                                    If Veg_Cod_Prec2 <> "" Then
                                        objAppezzaCodici.Scrivi(Key_Piva, Key_SaCod, OUTPUT_Appezza, enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_2, Veg_Cod_Prec2, Validita_Inizio_Impianto, Validita_Fine_Appezzamento, objParametri_Server)
                                    End If
                                    If Veg_Cod_Prec3 <> "" Then
                                        objAppezzaCodici.Scrivi(Key_Piva, Key_SaCod, OUTPUT_Appezza, enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_3, Veg_Cod_Prec3, Validita_Inizio_Impianto, Validita_Fine_Appezzamento, objParametri_Server)
                                    End If
                                    If Veg_Cod_Prec4 <> "" Then
                                        objAppezzaCodici.Scrivi(Key_Piva, Key_SaCod, OUTPUT_Appezza, enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_4, Veg_Cod_Prec4, Validita_Inizio_Impianto, Validita_Fine_Appezzamento, objParametri_Server)
                                    End If
                                    If Codice_Contratto <> "" Then
                                        objAppezzaCodici.Scrivi(Key_Piva, Key_SaCod, OUTPUT_Appezza, enum_CodiciAnagrafe.Appezzamento_CodiceContratto, Codice_Contratto, Validita_Inizio_Impianto, Validita_Fine_Appezzamento, objParametri_Server)
                                    End If
                                End If

                                If InseritoAppezzamento Then

                                    StrRegImpianto = String.Empty
                                    objXML.XML_Impianto(enum_CodificaDecodifica.Codifica,
                                                    StrRegImpianto,
                                                    enum_TipoOperazioneDB.Scrittura,
                                                    Key_Piva,
                                                    Key_SaCod,
                                                    Key_CampoCod,
                                                    OUTPUT_Appezza,
                                                    ZeroInt,
                                                    Sup_App,
                                                    ZeroInt,
                                                    ZeroInt,
                                                    ZeroInt,
                                                    Validita_Inizio_Impianto,
                                                    Cul_Cod,
                                                    Grva_Cod,
                                                    Data_Raccolta,
                                                    Resa,
                                                    ZeroDecimal,
                                                    ZeroInt,
                                                    ZeroInt, ZeroString, NullString, NullString, NullString,
                                                    TRA_Fila,
                                                    SU_Fila,
                                                    ZeroDecimal,
                                                    ZeroString,
                                                    ZeroInt,
                                                    ZeroInt, ZeroInt, ZeroInt, ZeroInt, ZeroInt, ZeroInt, ZeroString, ZeroInt, ZeroInt, ZeroInt, ZeroInt, ZeroInt, Codice_fiscale_tecnico, ZeroString,
                                                    Grfi_Cod,
                                                    Imp_Cod,
                                                    CInt(1),
                                                    ZeroInt,
                                                    ZeroInt,
                                                    Cop_Cod,
                                                    Foral_Cod,
                                                    ZeroInt, ZeroInt,
                                                    Validita_Inizio_Impianto,
                                                    Validita_Fine_Appezzamento,
                                                    BaseCode,
                                                    TopCode,
                                                    Unita_Vitata)

                                    StrRegImpianto = "<DatiReg_Impianti>" & StrRegImpianto & "</DatiReg_Impianti>"

                                    InseritoImpianto = objReg_Impianto_W.Reg_Impianto_Scrivi(
                                                                    CStr(StrRegImpianto),
                                                                    OUTPUT_Piva,
                                                                    OUTPUT_Sa_Cod,
                                                                    OUTPUT_Appezza,
                                                                    OUTPUT_Id_Reg,
                                                                    "",
                                                                    objParametri_Server)

                                    If InseritoImpianto Then
                                        '---------------------------------
                                        'caso destinazioni d'uso
                                        Dim strId_Cod As String = ""
                                        If Id_Cod <> 0 Then
                                            objReg_Impianti_Codici_W.Scrivi(Key_Piva, Key_SaCod, OUTPUT_Appezza, OUTPUT_Id_Reg,
                                                                            Id_Cod,
                                                                            "",
                                                                            Validita_Inizio, Validita_Fine, objParametri_Server)
                                        End If

                                        If TRA_Fila <> 0 Then
                                            objReg_Impianti_Codici_W.Scrivi(Key_Piva, Key_SaCod, OUTPUT_Appezza, OUTPUT_Id_Reg,
                                                                        enum_CodiciAnagrafe.Impianto_TraFila_Maschio,
                                                                        TRA_Fila,
                                                                        Validita_Inizio, Validita_Fine, objParametri_Server)
                                        End If

                                        If SU_Fila <> 0 Then
                                            objReg_Impianti_Codici_W.Scrivi(Key_Piva, Key_SaCod, OUTPUT_Appezza, OUTPUT_Id_Reg,
                                                                        enum_CodiciAnagrafe.Impianto_SuFila_Maschio,
                                                                        SU_Fila,
                                                                        Validita_Inizio, Validita_Fine, objParametri_Server)
                                        End If

                                        If Pratica_Cod <> 0 Then
                                            'DRUDI INSERIRE PRATICA
                                            objReg_Impianti_Codici_W.Scrivi(Key_Piva, Key_SaCod, OUTPUT_Appezza, OUTPUT_Id_Reg,
                                                                        enum_CodiciAnagrafe.Programmazione_Impianto_Pratica_Cod,
                                                                        CStr(Pratica_Cod),
                                                                        Validita_Inizio, Validita_Fine, objParametri_Server)
                                        End If

                                        'CREO LA DISTINTA
                                        StrProgetto = ""
                                        objXML.Xml_ProgettoPerImpianto(enum_CodificaDecodifica.Codifica,
                                                               StrProgetto,
                                                               enum_TipoOperazioneDB.Scrittura,
                                                               Key_Piva,
                                                               Key_SaCod,
                                                               ZeroInt,
                                                               Progetto_Nome,
                                                               NullString,
                                                               enum_Agenda_Causali.Progetto_Produzione_Agricola,
                                                               ZeroInt,
                                                               ZeroInt,
                                                               Data_Semina,
                                                               Data_Raccolta,
                                                               NullString,
                                                               OUTPUT_Appezza,
                                                               OUTPUT_Id_Reg,
                                                               0,
                                                               0,
                                                               Stato_Impianto,
                                                               Regolamento_Cod,
                                                               Disciplinare_Cod,
                                                               Regolamento_Concimazioni_Cod,
                                                               ZeroInt,
                                                               ZeroDecimal,
                                                               Resa,
                                                               Validita_Inizio,
                                                               Validita_Fine,
                                                               BaseCode,
                                                               TopCode,
                                                               Data_Fioritura_Prevista:=Data_Fioritura_Prevista)

                                        InseritoProgetto = objProgetto_W.Impresa_Progetto_Scrivi(
                                                                        CStr(StrProgetto),
                                                                        OUTPUT_Piva,
                                                                        OUTPUT_Progetto_Cod,
                                                                        objParametri_Server)

                                        If InseritoProgetto Then

                                            If Salva_Limite_N Then
                                                objReg_Impianti_Codici_W.ScrivixProgetto(Key_Piva, Key_SaCod, OUTPUT_Appezza, OUTPUT_Id_Reg, OUTPUT_Progetto_Cod,
                                                                                     enum_CodiciAnagrafe.Impianto_LimiteN,
                                                                                     ValLimiteN,
                                                                                     Validita_Inizio, Validita_Fine, objParametri_Server)
                                            Else
                                                If Limite_N <> "" AndAlso Limite_N <> "0" Then
                                                    objReg_Impianti_Codici_W.ScrivixProgetto(Key_Piva, Key_SaCod, OUTPUT_Appezza, OUTPUT_Id_Reg, OUTPUT_Progetto_Cod,
                                                                                     enum_CodiciAnagrafe.Impianto_LimiteN,
                                                                                     Limite_N,
                                                                                     Validita_Inizio, Validita_Fine, objParametri_Server)
                                                End If

                                                If Limite_P <> "" AndAlso Limite_P <> "0" Then
                                                    objReg_Impianti_Codici_W.ScrivixProgetto(Key_Piva, Key_SaCod, OUTPUT_Appezza, OUTPUT_Id_Reg, OUTPUT_Progetto_Cod,
                                                                                     enum_CodiciAnagrafe.Impianto_LimiteP,
                                                                                     Limite_P,
                                                                                     Validita_Inizio, Validita_Fine, objParametri_Server)
                                                End If

                                                If Limite_K <> "" AndAlso Limite_K <> "0" Then
                                                    objReg_Impianti_Codici_W.ScrivixProgetto(Key_Piva, Key_SaCod, OUTPUT_Appezza, OUTPUT_Id_Reg, OUTPUT_Progetto_Cod,
                                                                                     enum_CodiciAnagrafe.Impianto_LimiteK,
                                                                                     Limite_K,
                                                                                     Validita_Inizio, Validita_Fine, objParametri_Server)
                                                End If
                                            End If

                                            If Piano_Semina <> "" Then
                                                objReg_Impianti_Codici_W.ScrivixProgetto(Key_Piva, Key_SaCod, OUTPUT_Appezza, OUTPUT_Id_Reg, OUTPUT_Progetto_Cod,
                                                                                     enum_CodiciAnagrafe.Impianto_PianoSemina,
                                                                                     Piano_Semina,
                                                                                     Validita_Inizio, Validita_Fine, objParametri_Server)
                                            End If

                                            '  Vanni, 07/06/2013 14:22:20: ripristinata da Ribalta_OLD() --> Genera_Stringone_XML
                                            If Veg_Cod_Cliente <> "" Then
                                                objReg_Impianti_Codici_W.ScrivixProgetto(Key_Piva, Key_SaCod, OUTPUT_Appezza, OUTPUT_Id_Reg, OUTPUT_Progetto_Cod,
                                                                                 enum_CodiciAnagrafe.Codice_Specie_Agea,
                                                                                 Veg_Cod_Cliente,
                                                                                Validita_Inizio, Validita_Fine, objParametri_Server)
                                            End If

                                            If Cul_Cod_Cliente <> "" Then
                                                objReg_Impianti_Codici_W.ScrivixProgetto(Key_Piva, Key_SaCod, OUTPUT_Appezza, OUTPUT_Id_Reg, OUTPUT_Progetto_Cod,
                                                                                 enum_CodiciAnagrafe.Codice_Cultivar_Agea,
                                                                                 Cul_Cod_Cliente,
                                                                                Validita_Inizio, Validita_Fine, objParametri_Server)
                                            End If

                                            Dim reg_impianti_programmazione_w As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Programmazioni_W
                                            reg_impianti_programmazione_w.Scrivi(
                                               Key_Piva, Key_SaCod, OUTPUT_Appezza, OUTPUT_Id_Reg, OUTPUT_Progetto_Cod,
                                                key_programmazione_cod, Key_Entita, Validita_Inizio, Validita_Fine,
                                                objParametri_Server
                                            )

                                        End If

                                    End If

                                End If


                            '-------------------------------------------------------------
                            '--- NUOVO APPEZZAMENTO
                            '-------------------------------------------------------------

                            Case enum_TipoOperazioneProgrammazioneEntita.Nuovo_Appezzamento,
                                enum_TipoOperazioneProgrammazioneEntita.Modifica_Semplice,
                                enum_TipoOperazioneProgrammazioneEntita.Frazionamento,
                                enum_TipoOperazioneProgrammazioneEntita.Unione

                                StrAppezzamento = ""
                                objXML.XML_Appezzamento(enum_CodificaDecodifica.Codifica,
                                                        StrAppezzamento,
                                                        enum_TipoOperazioneDB.Scrittura,
                                                        Key_Piva,
                                                        Key_SaCod,
                                                        ZeroInt,
                                                        Sup_App,
                                                        ZeroData,
                                                        ZeroData,
                                                        ZeroInt,
                                                        ZeroInt,
                                                        ZeroInt,
                                                        PuntoString,
                                                        ZeroInt,
                                                        PuntoString,
                                                        ZeroInt,
                                                        NullString,
                                                        ZeroDecimal,
                                                        ZeroDecimal,
                                                        ZeroDecimal,
                                                        ZeroDecimal,
                                                        ZeroDecimal,
                                                        ZeroDecimal,
                                                        ZeroDecimal,
                                                        ZeroDecimal,
                                                        ZeroDecimal,
                                                        ZeroDecimal,
                                                        ZeroDecimal,
                                                        ZeroDecimal,
                                                        ZeroString,
                                                        ZeroInt,
                                                        ZeroDecimal,
                                                        ZeroData,
                                                        ZeroDecimal,
                                                        ZeroData,
                                                        ZeroData,
                                                        ZeroDecimal,
                                                        ZeroData,
                                                        NullString,
                                                        App_Nome,
                                                        ZeroInt,
                                                        ZeroDecimal,
                                                        NullString,
                                                        Key_CampoCod,
                                                        ZeroInt,
                                                        ZeroData,
                                                        ZeroData,
                                                        Validita_Inizio_Impianto,
                                                        Validita_Fine_Appezzamento,
                                                        BaseCode,
                                                        TopCode,
                                                        DistBZ_CorpiIdrici:=DistBZ_CorpiIdrici,
                                                        DistBZ_AreeResPub:=DistBZ_AreeResPub,
                                                        DistBZ_Allevamenti:=DistBZ_Allevamenti,
                                                        DistBZ_VegNatNonColt:=DistBZ_VegNatNonColt,
                                                        SupBZ_Riduzione:=SupBZ_Riduzione)


                                XmlDocAppoggio = New Xml.XmlDocument
                                XmlDocAppoggio.LoadXml(StrAppezzamento)

                                XmlAppezzamento = XmlDocAppoggio.SelectSingleNode("Appezzamento")

                                Dim Str_CodiceAppezzamento As String = ""
                                objXML.XML_Codice(enum_CodificaDecodifica.Codifica,
                                                  Str_CodiceAppezzamento,
                                                  enum_TipoOperazioneDB.Scrittura,
                                                  enum_CodiciAnagrafe.TitoloPossesso,
                                                  1,
                                                  Validita_Inizio,
                                                  Validita_Fine,
                                                  BaseCode,
                                                  TopCode, "Appezzamento")



                                Dim DT_Particelle As New DataTable
                                Dim objPP As New AgronicaCoreAnagrafeDAL.Programmazione_Particelle_R
                                DT_Particelle = objPP.Programmazione_Particelle_Leggi_4(objParametri_Server,
                                                                                        Key_Piva,
                                                                                        strErr,
                                                                                        Key_Entita)
                                StrParticelle = String.Empty

                                For j = 0 To DT_Particelle.Rows.Count - 1

                                    If Not HashParticelle.ContainsKey(DT_Particelle.Rows(j).Item("PROV") & "_" & DT_Particelle.Rows(j).Item("COM") & "_" & DT_Particelle.Rows(j).Item("Sezione") & "_" & DT_Particelle.Rows(j).Item("Foglio") & "_" & DT_Particelle.Rows(j).Item("Numero") & "_" & DT_Particelle.Rows(j).Item("Subalterno")) Then

                                        HashParticelle.Add(DT_Particelle.Rows(j).Item("PROV") & "_" & DT_Particelle.Rows(j).Item("COM") & "_" & DT_Particelle.Rows(j).Item("Sezione") & "_" & DT_Particelle.Rows(j).Item("Foglio") & "_" & DT_Particelle.Rows(j).Item("Numero") & "_" & DT_Particelle.Rows(j).Item("Subalterno"), "")

                                        Conversioni.EttariAreCentiare_from_Ettari(DT_Particelle.Rows(j).Item("Superficie"), Ettari, Are, Centiare)

                                        StrParticelle &= objXML.XML_AppezzamentoParticella(enum_TipoOperazioneDB.Scrittura,
                                                                                          Key_Piva,
                                                                                          Key_SaCod,
                                                                                          ZeroInt,
                                                                                          DT_Particelle.Rows(j).Item("PROV"),
                                                                                          DT_Particelle.Rows(j).Item("COM"),
                                                                                          DT_Particelle.Rows(j).Item("Sezione"),
                                                                                          DT_Particelle.Rows(j).Item("foglio"),
                                                                                          DT_Particelle.Rows(j).Item("numero"),
                                                                                          DT_Particelle.Rows(j).Item("subalterno"),
                                                                                          DT_Particelle.Rows(j).Item("Superficie"),
                                                                                          Ettari,
                                                                                          Are,
                                                                                          Centiare,
                                                                                          ZeroInt, ZeroInt, ZeroInt, ZeroInt, ZeroInt, ZeroInt,
                                                                                          Validita_Inizio_Impianto,
                                                                                          Validita_Fine_Appezzamento,
                                                                                          BaseCode,
                                                                                          TopCode)
                                    End If

                                Next

                                XmlAppezzamento.InnerXml = Str_CodiceAppezzamento

                                If StrParticelle <> "" Then
                                    XML_DatiParticelle = XmlAppezzamento.OwnerDocument.CreateElement("DatiParticelle")
                                    XmlAppezzamento.AppendChild(XML_DatiParticelle)
                                    XML_DatiParticelle.InnerXml = StrParticelle
                                End If

                                Dim Str_AppezzamentoxIndirizzi As String = ""
                                Dim Cod_indirizzo_new As Integer = 0
                                If Cod_Indirizzo <> 0 Then

                                    Dim siglaProv = ""
                                    If stato_indirizzo = "IT" Then
                                        objIstat.Provincia_from_CodIstat(pro_cod_istat_indirizzo, siglaProv, objParametri_Server)
                                    End If

                                    Dim strIndirizzo As String = ""
                                    objXML.XML_Indirizzo(enum_CodificaDecodifica.Codifica,
                                                                            strIndirizzo,
                                                                            enum_TipoOperazioneDB.Scrittura,
                                                                            1,
                                                                            0,
                                                                            ind_des,
                                                                            frz_des,
                                                                            CAP,
                                                                            com_des_indirizzo,
                                                                            siglaProv,
                                                                            stato_indirizzo,
                                                                            note_indirizzo,
                                                                            pro_cod_istat_indirizzo,
                                                                            com_cod_istat_indirizzo,
                                                                            AGRODATAINIZIO,
                                                                            AGRODATAFINE,
                                                                            BaseCode,
                                                                            TopCode)

                                    Str_AppezzamentoxIndirizzi &= strIndirizzo

                                End If

                                If Str_AppezzamentoxIndirizzi <> "" Then
                                    XmlAppezzamento.InnerXml &= Str_AppezzamentoxIndirizzi
                                End If

                                strXMLAppezzamenti = "<DatiAppezzamenti>" & XmlAppezzamento.OuterXml & "</DatiAppezzamenti>"

                                InseritoAppezzamento = objAppezzamento_W.Appezzamento_Scrivi(strXMLAppezzamenti,
                                                                                            OUTPUT_Piva,
                                                                                            OUTPUT_Sa_Cod,
                                                                                            OUTPUT_Appezza,
                                                                                            objParametri_Server,
                                                                                            objParametri_Utenti)

                                If InseritoAppezzamento Then

                                    If Blocca_Appezza Then
                                        Dim objAppezza As New AgronicaCoreAnagrafeDAL.Appezzamento_Write
                                        objAppezza.Appezza_Blocca(OUTPUT_Piva, OUTPUT_Sa_Cod, OUTPUT_Appezza, "", objParametri_Server)
                                    End If


                                    'APPEZZAMENTO CODICI
                                    Dim objAppezzaCodici As New AgronicaCoreAnagrafeDAL.Appezzamento_Codici_W
                                    If Metodo_Produzione_Cod <> 0 Then
                                        objAppezzaCodici.Scrivi(Key_Piva, Key_SaCod, OUTPUT_Appezza, enum_CodiciAnagrafe.MetodoDiProduzione, Metodo_Produzione_Cod, Validita_Inizio_Impianto, Validita_Fine_Appezzamento, objParametri_Server)
                                    End If
                                    If Veg_Cod_Prec <> "" Then
                                        objAppezzaCodici.Scrivi(Key_Piva, Key_SaCod, OUTPUT_Appezza, enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_1, Veg_Cod_Prec, Validita_Inizio_Impianto, Validita_Fine_Appezzamento, objParametri_Server)
                                    End If
                                    If Veg_Cod_Prec2 <> "" Then
                                        objAppezzaCodici.Scrivi(Key_Piva, Key_SaCod, OUTPUT_Appezza, enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_2, Veg_Cod_Prec2, Validita_Inizio_Impianto, Validita_Fine_Appezzamento, objParametri_Server)
                                    End If
                                    If Veg_Cod_Prec3 <> "" Then
                                        objAppezzaCodici.Scrivi(Key_Piva, Key_SaCod, OUTPUT_Appezza, enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_3, Veg_Cod_Prec3, Validita_Inizio_Impianto, Validita_Fine_Appezzamento, objParametri_Server)
                                    End If
                                    If Veg_Cod_Prec4 <> "" Then
                                        objAppezzaCodici.Scrivi(Key_Piva, Key_SaCod, OUTPUT_Appezza, enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_4, Veg_Cod_Prec4, Validita_Inizio_Impianto, Validita_Fine_Appezzamento, objParametri_Server)
                                    End If
                                    If Codice_Contratto <> "" Then
                                        objAppezzaCodici.Scrivi(Key_Piva, Key_SaCod, OUTPUT_Appezza, enum_CodiciAnagrafe.Appezzamento_CodiceContratto, Codice_Contratto, Validita_Inizio_Impianto, Validita_Fine_Appezzamento, objParametri_Server)
                                    End If

                                End If



                                If InseritoAppezzamento Then

                                    StrRegImpianto = String.Empty
                                    objXML.XML_Impianto(enum_CodificaDecodifica.Codifica,
                                                    StrRegImpianto,
                                                    enum_TipoOperazioneDB.Scrittura,
                                                    Key_Piva,
                                                    Key_SaCod,
                                                    Key_CampoCod,
                                                    OUTPUT_Appezza,
                                                    ZeroInt,
                                                    Sup_App,
                                                    ZeroInt,
                                                    ZeroInt,
                                                    ZeroInt,
                                                    Validita_Inizio_Impianto,
                                                    Cul_Cod,
                                                    Grva_Cod,
                                                    Data_Raccolta,
                                                    Resa,
                                                    ZeroDecimal,
                                                    ZeroInt,
                                                    ZeroInt, ZeroString, NullString, NullString, NullString,
                                                    TRA_Fila,
                                                    SU_Fila,
                                                    ZeroDecimal,
                                                    ZeroString,
                                                    ZeroInt,
                                                    ZeroInt, ZeroInt, ZeroInt, ZeroInt, ZeroInt, ZeroInt, ZeroString, ZeroInt, ZeroInt, ZeroInt, ZeroInt, ZeroInt, Codice_fiscale_tecnico, ZeroString,
                                                    Grfi_Cod,
                                                    Imp_Cod,
                                                    CInt(1),
                                                    ZeroInt,
                                                    ZeroInt,
                                                    Cop_Cod,
                                                    Foral_Cod,
                                                    ZeroInt, ZeroInt,
                                                    Validita_Inizio_Impianto,
                                                    Validita_Fine_Appezzamento,
                                                    BaseCode,
                                                    TopCode,
                                                    Unita_Vitata)

                                    StrRegImpianto = "<DatiReg_Impianti>" & StrRegImpianto & "</DatiReg_Impianti>"

                                    'salvataggio!!!
                                    InseritoImpianto = objReg_Impianto_W.Reg_Impianto_Scrivi(
                                                                    CStr(StrRegImpianto),
                                                                    OUTPUT_Piva,
                                                                    OUTPUT_Sa_Cod,
                                                                    OUTPUT_Appezza,
                                                                    OUTPUT_Id_Reg,
                                                                    "",
                                                                    objParametri_Server)


                                    If InseritoImpianto Then
                                        '---------------------------------
                                        'caso destinazioni d'uso
                                        Dim strId_Cod As String = ""
                                        If Id_Cod <> 0 Then
                                            objReg_Impianti_Codici_W.ScrivixProgetto(Key_Piva, Key_SaCod, OUTPUT_Appezza, OUTPUT_Id_Reg, Key_ProgettoCod,
                                                                            Id_Cod,
                                                                            "",
                                                                            Validita_Inizio, Validita_Fine, objParametri_Server)
                                        End If

                                        If TRA_Fila <> 0 Then
                                            objReg_Impianti_Codici_W.Scrivi(Key_Piva, Key_SaCod, OUTPUT_Appezza, OUTPUT_Id_Reg,
                                                                        enum_CodiciAnagrafe.Impianto_TraFila_Maschio,
                                                                        TRA_Fila,
                                                                        Validita_Inizio, Validita_Fine, objParametri_Server)
                                        End If

                                        If SU_Fila <> 0 Then
                                            objReg_Impianti_Codici_W.Scrivi(Key_Piva, Key_SaCod, OUTPUT_Appezza, OUTPUT_Id_Reg,
                                                                        enum_CodiciAnagrafe.Impianto_SuFila_Maschio,
                                                                        SU_Fila,
                                                                        Validita_Inizio, Validita_Fine, objParametri_Server)
                                        End If

                                        If Pratica_Cod <> 0 Then
                                            'DRUDI INSERIRE PRATICA
                                            objReg_Impianti_Codici_W.Scrivi(Key_Piva, Key_SaCod, OUTPUT_Appezza, OUTPUT_Id_Reg,
                                                                        enum_CodiciAnagrafe.Programmazione_Impianto_Pratica_Cod,
                                                                        CStr(Pratica_Cod),
                                                                        Validita_Inizio, Validita_Fine, objParametri_Server)
                                        End If


                                        'CREO LA DISTINTA
                                        StrProgetto = ""
                                        objXML.Xml_ProgettoPerImpianto(enum_CodificaDecodifica.Codifica,
                                                               StrProgetto,
                                                               enum_TipoOperazioneDB.Scrittura,
                                                               Key_Piva,
                                                               Key_SaCod,
                                                               ZeroInt,
                                                               Progetto_Nome,
                                                               NullString,
                                                               enum_Agenda_Causali.Progetto_Produzione_Agricola,
                                                               ZeroInt,
                                                               ZeroInt,
                                                               Data_Semina,
                                                               Data_Raccolta,
                                                               NullString,
                                                               OUTPUT_Appezza,
                                                               OUTPUT_Id_Reg,
                                                               0,
                                                               0,
                                                               Stato_Impianto,
                                                               Regolamento_Cod,
                                                               Disciplinare_Cod,
                                                               Regolamento_Concimazioni_Cod,
                                                               ZeroInt,
                                                               ZeroDecimal,
                                                               Resa,
                                                               Validita_Inizio,
                                                               Validita_Fine,
                                                               BaseCode,
                                                               TopCode,
                                                               Data_Fioritura_Prevista:=Data_Fioritura_Prevista)

                                        InseritoProgetto = objProgetto_W.Impresa_Progetto_Scrivi(
                                                                        CStr(StrProgetto),
                                                                        OUTPUT_Piva,
                                                                        OUTPUT_Progetto_Cod,
                                                                        objParametri_Server)

                                        If InseritoProgetto Then

                                            If Salva_Limite_N Then
                                                objReg_Impianti_Codici_W.ScrivixProgetto(Key_Piva, Key_SaCod, OUTPUT_Appezza, OUTPUT_Id_Reg, OUTPUT_Progetto_Cod,
                                                                                     enum_CodiciAnagrafe.Impianto_LimiteN,
                                                                                     ValLimiteN,
                                                                                     Validita_Inizio, Validita_Fine, objParametri_Server)
                                            Else
                                                If Limite_N <> "" AndAlso Limite_N <> "0" Then
                                                    objReg_Impianti_Codici_W.ScrivixProgetto(Key_Piva, Key_SaCod, OUTPUT_Appezza, OUTPUT_Id_Reg, OUTPUT_Progetto_Cod,
                                                                                     enum_CodiciAnagrafe.Impianto_LimiteN,
                                                                                     Limite_N,
                                                                                     Validita_Inizio, Validita_Fine, objParametri_Server)
                                                End If

                                                If Limite_P <> "" AndAlso Limite_P <> "0" Then
                                                    objReg_Impianti_Codici_W.ScrivixProgetto(Key_Piva, Key_SaCod, OUTPUT_Appezza, OUTPUT_Id_Reg, OUTPUT_Progetto_Cod,
                                                                                     enum_CodiciAnagrafe.Impianto_LimiteP,
                                                                                     Limite_P,
                                                                                     Validita_Inizio, Validita_Fine, objParametri_Server)
                                                End If

                                                If Limite_K <> "" AndAlso Limite_K <> "0" Then
                                                    objReg_Impianti_Codici_W.ScrivixProgetto(Key_Piva, Key_SaCod, OUTPUT_Appezza, OUTPUT_Id_Reg, OUTPUT_Progetto_Cod,
                                                                                     enum_CodiciAnagrafe.Impianto_LimiteK,
                                                                                     Limite_K,
                                                                                     Validita_Inizio, Validita_Fine, objParametri_Server)
                                                End If
                                            End If

                                            If Piano_Semina <> "" Then
                                                objReg_Impianti_Codici_W.ScrivixProgetto(Key_Piva, Key_SaCod, OUTPUT_Appezza, OUTPUT_Id_Reg, OUTPUT_Progetto_Cod,
                                                                                     enum_CodiciAnagrafe.Impianto_PianoSemina,
                                                                                     Piano_Semina,
                                                                                     Validita_Inizio, Validita_Fine, objParametri_Server)
                                            End If

                                            '  Vanni, 07/06/2013 14:22:20: ripristinata da Ribalta_OLD() --> Genera_Stringone_XML
                                            If Veg_Cod_Cliente <> "" Then
                                                objReg_Impianti_Codici_W.ScrivixProgetto(Key_Piva, Key_SaCod, OUTPUT_Appezza, OUTPUT_Id_Reg, OUTPUT_Progetto_Cod,
                                                                                 enum_CodiciAnagrafe.Codice_Specie_Agea,
                                                                                 Veg_Cod_Cliente,
                                                                                Validita_Inizio, Validita_Fine, objParametri_Server)
                                            End If

                                            If Cul_Cod_Cliente <> "" Then
                                                objReg_Impianti_Codici_W.ScrivixProgetto(Key_Piva, Key_SaCod, OUTPUT_Appezza, OUTPUT_Id_Reg, OUTPUT_Progetto_Cod,
                                                                                 enum_CodiciAnagrafe.Codice_Cultivar_Agea,
                                                                                 Cul_Cod_Cliente,
                                                                                Validita_Inizio, Validita_Fine, objParametri_Server)
                                            End If

                                            Dim reg_impianti_programmazione_w As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Programmazioni_W
                                            reg_impianti_programmazione_w.Scrivi(
                                                Key_Piva, Key_SaCod, OUTPUT_Appezza, OUTPUT_Id_Reg, OUTPUT_Progetto_Cod,
                                                key_programmazione_cod, Key_Entita, Validita_Inizio, Validita_Fine,
                                                objParametri_Server
                                            )



                                        End If

                                    End If

                                End If

                            Case enum_TipoOperazioneProgrammazioneEntita.Nessuna



                                'creo il campo raccoglitore
                                If Not listaSaCodR.Contains(Key_SaCod) AndAlso Chk_Crea_Campo Then

                                    If Validita_inizio_Min Is Nothing Then
                                        Validita_inizio_Min = AGRODATAINIZIO
                                    End If

                                    If Validita_Fine_Max Is Nothing Then
                                        Validita_Fine_Max = AGRODATAFINE
                                    End If

                                    StrCampo = ""
                                    objXML.XML_Campo(enum_CodificaDecodifica.Codifica,
                                                            StrCampo,
                                                            enum_TipoOperazioneDB.Scrittura,
                                                            Key_Piva,
                                                            Key_SaCod,
                                                            ZeroInt,
                                                            ZeroInt,
                                                            Campo_Des,
                                                            AGRODATAINIZIO,
                                                            AGRODATAINIZIO,
                                                            ZeroInt,
                                                            ZeroInt,
                                                            ZeroDecimal,
                                                            ZeroDecimal,
                                                            NullString,
                                                            ZeroInt,
                                                            ZeroInt,
                                                            Validita_inizio_Min,
                                                            Validita_Fine_Max,
                                                            BaseCode,
                                                            TopCode)

                                    If StrCampo <> "" Then

                                        StrCampo = "<DatiCampi>" & StrCampo & "</DatiCampi>"

                                        InseritoCampo = objCampo_W.Campo_Scrivi(StrCampo,
                                                                                OUTPUT_Piva,
                                                                                OUTPUT_Sa_Cod,
                                                                                OUTPUT_Campo_Cod,
                                                                                False,
                                                                                objParametri_Server,
                                                                                objParametri_Utenti)

                                        listaSaCodR.Add(Key_SaCod)

                                    End If

                                End If



                                If Ribaltato = 0 Then

                                    Dim campo_cod As Integer = 0

                                    If Key_CampoCod = 0 AndAlso OUTPUT_Campo_Cod <> 0 Then
                                        campo_cod = OUTPUT_Campo_Cod
                                    Else
                                        campo_cod = Key_CampoCod
                                    End If

                                    StrAppezzamento = ""
                                    objXML.XML_Appezzamento(enum_CodificaDecodifica.Codifica,
                                                        StrAppezzamento,
                                                        enum_TipoOperazioneDB.Scrittura,
                                                        Key_Piva,
                                                        Key_SaCod,
                                                        ZeroInt,
                                                        Sup_App,
                                                        ZeroData,
                                                        ZeroData,
                                                        ZeroInt,
                                                        ZeroInt,
                                                        ZeroInt,
                                                        PuntoString,
                                                        ZeroInt,
                                                        PuntoString,
                                                        ZeroInt,
                                                        NullString,
                                                        ZeroDecimal,
                                                        ZeroDecimal,
                                                        ZeroDecimal,
                                                        ZeroDecimal,
                                                        ZeroDecimal,
                                                        ZeroDecimal,
                                                        ZeroDecimal,
                                                        ZeroDecimal,
                                                        ZeroDecimal,
                                                        ZeroDecimal,
                                                        ZeroDecimal,
                                                        ZeroDecimal,
                                                        ZeroString,
                                                        ZeroInt,
                                                        ZeroDecimal,
                                                        ZeroData,
                                                        ZeroDecimal,
                                                        ZeroData,
                                                        ZeroData,
                                                        ZeroDecimal,
                                                        ZeroData,
                                                        NullString,
                                                        App_Nome,
                                                        ZeroInt,
                                                        ZeroDecimal,
                                                        NullString,
                                                        campo_cod,
                                                        ZeroInt,
                                                        ZeroData,
                                                        ZeroData,
                                                        Validita_Inizio,
                                                        Validita_Fine_Appezzamento,
                                                        BaseCode,
                                                        TopCode,
                                                        DistBZ_CorpiIdrici:=DistBZ_CorpiIdrici,
                                                        DistBZ_AreeResPub:=DistBZ_AreeResPub,
                                                        DistBZ_Allevamenti:=DistBZ_Allevamenti,
                                                        DistBZ_VegNatNonColt:=DistBZ_VegNatNonColt,
                                                        SupBZ_Riduzione:=SupBZ_Riduzione)


                                    XmlDocAppoggio = New Xml.XmlDocument
                                    XmlDocAppoggio.LoadXml(StrAppezzamento)

                                    XmlAppezzamento = XmlDocAppoggio.SelectSingleNode("Appezzamento")

                                    Dim Str_CodiceAppezzamento As String = ""
                                    objXML.XML_Codice(enum_CodificaDecodifica.Codifica,
                                                      Str_CodiceAppezzamento,
                                                      enum_TipoOperazioneDB.Scrittura,
                                                      enum_CodiciAnagrafe.TitoloPossesso,
                                                      1,
                                                      Validita_Inizio,
                                                      Validita_Fine,
                                                      BaseCode,
                                                      TopCode, "Appezzamento")



                                    Dim DT_Particelle As New DataTable
                                    Dim objPP As New AgronicaCoreAnagrafeDAL.Programmazione_Particelle_R
                                    DT_Particelle = objPP.Programmazione_Particelle_Leggi_4(objParametri_Server,
                                                                                          Key_Piva,
                                                                                          strErr,
                                                                                          Key_Entita)
                                    StrParticelle = String.Empty

                                    For j = 0 To DT_Particelle.Rows.Count - 1

                                        If Not HashParticelle.ContainsKey(DT_Particelle.Rows(j).Item("PROV") & "_" & DT_Particelle.Rows(j).Item("COM") & "_" & DT_Particelle.Rows(j).Item("Sezione") & "_" & DT_Particelle.Rows(j).Item("Foglio") & "_" & DT_Particelle.Rows(j).Item("Numero") & "_" & DT_Particelle.Rows(j).Item("Subalterno")) Then

                                            HashParticelle.Add(DT_Particelle.Rows(j).Item("PROV") & "_" & DT_Particelle.Rows(j).Item("COM") & "_" & DT_Particelle.Rows(j).Item("Sezione") & "_" & DT_Particelle.Rows(j).Item("Foglio") & "_" & DT_Particelle.Rows(j).Item("Numero") & "_" & DT_Particelle.Rows(j).Item("Subalterno"), "")

                                            Conversioni.EttariAreCentiare_from_Ettari(DT_Particelle.Rows(j).Item("Superficie"), Ettari, Are, Centiare)

                                            StrParticelle &= objXML.XML_AppezzamentoParticella(enum_TipoOperazioneDB.Scrittura,
                                                                                              Key_Piva,
                                                                                              Key_SaCod,
                                                                                              ZeroInt,
                                                                                              DT_Particelle.Rows(j).Item("PROV"),
                                                                                              DT_Particelle.Rows(j).Item("COM"),
                                                                                              DT_Particelle.Rows(j).Item("Sezione"),
                                                                                              DT_Particelle.Rows(j).Item("foglio"),
                                                                                              DT_Particelle.Rows(j).Item("numero"),
                                                                                              DT_Particelle.Rows(j).Item("subalterno"),
                                                                                              DT_Particelle.Rows(j).Item("Superficie"),
                                                                                              Ettari,
                                                                                              Are,
                                                                                              Centiare,
                                                                                              ZeroInt, ZeroInt, ZeroInt, ZeroInt, ZeroInt, ZeroInt,
                                                                                              Validita_Inizio,
                                                                                              Validita_Fine_Appezzamento,
                                                                                              BaseCode,
                                                                                              TopCode)

                                        End If

                                    Next

                                    XmlAppezzamento.InnerXml = Str_CodiceAppezzamento

                                    If StrParticelle <> "" Then
                                        XML_DatiParticelle = XmlAppezzamento.OwnerDocument.CreateElement("DatiParticelle")
                                        XmlAppezzamento.AppendChild(XML_DatiParticelle)
                                        XML_DatiParticelle.InnerXml &= StrParticelle
                                    End If

                                    Dim Str_AppezzamentoxIndirizzi As String = ""
                                    Dim Cod_indirizzo_new As Integer = 0
                                    If Cod_Indirizzo <> 0 Then

                                        Dim siglaProv = ""
                                        If stato_indirizzo = "IT" Then
                                            objIstat.Provincia_from_CodIstat(pro_cod_istat_indirizzo, siglaProv, objParametri_Server)
                                        End If

                                        Dim strIndirizzo As String = ""
                                        objXML.XML_Indirizzo(enum_CodificaDecodifica.Codifica,
                                                                            strIndirizzo,
                                                                            enum_TipoOperazioneDB.Scrittura,
                                                                            1,
                                                                            0,
                                                                            ind_des,
                                                                            frz_des,
                                                                            CAP,
                                                                            com_des_indirizzo,
                                                                            siglaProv,
                                                                            stato_indirizzo,
                                                                            note_indirizzo,
                                                                            pro_cod_istat_indirizzo,
                                                                            com_cod_istat_indirizzo,
                                                                            AGRODATAINIZIO,
                                                                            AGRODATAFINE,
                                                                            BaseCode,
                                                                            TopCode)

                                        Str_AppezzamentoxIndirizzi &= strIndirizzo

                                    End If

                                    If Str_AppezzamentoxIndirizzi <> "" Then
                                        XmlAppezzamento.InnerXml &= Str_AppezzamentoxIndirizzi
                                    End If

                                    strXMLAppezzamenti = "<DatiAppezzamenti>" & XmlAppezzamento.OuterXml & "</DatiAppezzamenti>"

                                    InseritoAppezzamento = objAppezzamento_W.Appezzamento_Scrivi(strXMLAppezzamenti,
                                                                                                OUTPUT_Piva,
                                                                                                OUTPUT_Sa_Cod,
                                                                                                OUTPUT_Appezza,
                                                                                                objParametri_Server,
                                                                                                objParametri_Utenti)

                                    If InseritoAppezzamento Then

                                        If Blocca_Appezza Then
                                            Dim objAppezza As New AgronicaCoreAnagrafeDAL.Appezzamento_Write
                                            objAppezza.Appezza_Blocca(OUTPUT_Piva, OUTPUT_Sa_Cod, OUTPUT_Appezza, "", objParametri_Server)
                                        End If


                                        'APPEZZAMENTO CODICI
                                        Dim objAppezzaCodici As New AgronicaCoreAnagrafeDAL.Appezzamento_Codici_W
                                        If Metodo_Produzione_Cod <> 0 Then
                                            objAppezzaCodici.Scrivi(Key_Piva, Key_SaCod, OUTPUT_Appezza, enum_CodiciAnagrafe.MetodoDiProduzione, Metodo_Produzione_Cod, Validita_Inizio, Validita_Fine_Appezzamento, objParametri_Server)
                                        End If
                                        If Veg_Cod_Prec <> "" Then
                                            objAppezzaCodici.Scrivi(Key_Piva, Key_SaCod, OUTPUT_Appezza, enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_1, Veg_Cod_Prec, Validita_Inizio, Validita_Fine_Appezzamento, objParametri_Server)
                                        End If
                                        If Veg_Cod_Prec2 <> "" Then
                                            objAppezzaCodici.Scrivi(Key_Piva, Key_SaCod, OUTPUT_Appezza, enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_2, Veg_Cod_Prec2, Validita_Inizio, Validita_Fine_Appezzamento, objParametri_Server)
                                        End If
                                        If Veg_Cod_Prec3 <> "" Then
                                            objAppezzaCodici.Scrivi(Key_Piva, Key_SaCod, OUTPUT_Appezza, enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_3, Veg_Cod_Prec3, Validita_Inizio, Validita_Fine_Appezzamento, objParametri_Server)
                                        End If
                                        If Veg_Cod_Prec4 <> "" Then
                                            objAppezzaCodici.Scrivi(Key_Piva, Key_SaCod, OUTPUT_Appezza, enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_4, Veg_Cod_Prec4, Validita_Inizio, Validita_Fine_Appezzamento, objParametri_Server)
                                        End If
                                        If Codice_Contratto <> "" Then
                                            objAppezzaCodici.Scrivi(Key_Piva, Key_SaCod, OUTPUT_Appezza, enum_CodiciAnagrafe.Appezzamento_CodiceContratto, Codice_Contratto, Validita_Inizio, Validita_Fine_Appezzamento, objParametri_Server)
                                        End If

                                        If Riferimento_Alfanumerico_Appezzamento <> "" AndAlso Riferimento_Alfanumerico_Appezzamento <> "0" Then
                                            'Dim objAppezzamentoCodici As New AgronicaCoreAnagrafeDAL.Appezzamento_Codici_R
                                            'Dim filtro As String = "(Appezzamento_Codici.Sa_Cod <> " & Key_SaCod & " Or Appezzamento_Codici.Appezza <> " & OUTPUT_Appezza & ")"
                                            'Dim dtCodAppezzamento = objAppezzamentoCodici.Leggi(Key_Piva, 0, 0, enum_CodiciAnagrafe.Riferimento_Alfanumerico_Appezzamento, Riferimento_Alfanumerico_Appezzamento, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, filtro, "", objParametri_Server)
                                            'If dtCodAppezzamento.Rows.Count > 0 Then
                                            '    Throw New Exception("Codice Alfanumerico Appezzamento già utilizzato.")
                                            'End If
                                            objAppezzaCodici.Scrivi(Key_Piva, Key_SaCod, OUTPUT_Appezza, enum_CodiciAnagrafe.Riferimento_Alfanumerico_Appezzamento, Riferimento_Alfanumerico_Appezzamento, Validita_Inizio, Validita_Fine_Appezzamento, objParametri_Server)
                                        End If

                                        If Isola <> "" Then
                                            objAppezzaCodici.Scrivi(Key_Piva, Key_SaCod, OUTPUT_Appezza, enum_CodiciAnagrafe.Isola, Isola, Validita_Inizio, Validita_Fine_Appezzamento, objParametri_Server)
                                        End If

                                    End If

                                    If InseritoAppezzamento Then

                                        Dim XmlDoc = New System.Xml.XmlDocument
                                        Dim objXmlAnagrafe = New AgronicaCoreXML.XML_Anagrafe
                                        Dim xmlImpianto = objXmlAnagrafe.XML_Impianto_Impianto("",
                                         XmlDoc,
                                         BaseCode,
                                         TopCode,
                                         enum_TipoOperazioneDB.Scrittura,
                                         "",
                                         Key_Piva,
                                         Key_SaCod,
                                         OUTPUT_Appezza,
                                         ZeroInt,
                                         ZeroInt, 'valore obbligatio, messo zero perche' e' il valore di default della scrivi
                                         Sup_App,
                                         Cul_Cod,
                                         Grfi_Cod,
                                         Validita_Inizio,
                                         Validita_Fine_Appezzamento,
                                         Validita_Inizio_Impianto,
                                        Grva_Cod_Veg:=Grva_Cod,
                                        Data_Raccolta:=Data_Raccolta,
                                        ResaPrevista:=Resa,
                                        Tra_Fila:=TRA_Fila,
                                        Su_Fila:=SU_Fila,
                                        Codice_Ficale_Tecnico:=Codice_fiscale_tecnico,
                                        Id_Campo:=OUTPUT_Campo_Cod, Imp_Cod:=Imp_Cod, Cop_Cod:=Cop_Cod, Foral_Cod:=Foral_Cod, Unita_Vitata:=Unita_Vitata, Data_Inizio_Portinnesto:=Data_Inizio_Portinnesto, Data_Inizio_Impianto:=Validita_Inizio_Impianto)






                                        'Uhalid 17/05/24 Sostituito con funzione sopra perche' deprecata
                                        'StrRegImpianto = String.Empty
                                        'StrRegImpianto = XmlDoc.InnerXml
                                        'objXML.XML_Impianto(enum_CodificaDecodifica.Codifica,
                                        '            StrRegImpianto,
                                        '            enum_TipoOperazioneDB.Scrittura,
                                        '            Key_Piva,
                                        '            Key_SaCod,
                                        '            OUTPUT_Campo_Cod,
                                        '            OUTPUT_Appezza,
                                        '            ZeroInt,
                                        '            Sup_App,
                                        '            ZeroInt,
                                        '            ZeroInt,
                                        '            ZeroInt,
                                        '            Validita_Inizio_Impianto,
                                        '            Cul_Cod,
                                        '            Grva_Cod,
                                        '            Data_Raccolta,
                                        '            Resa,
                                        '            ZeroDecimal,
                                        '            ZeroInt,
                                        '            ZeroInt, ZeroString, NullString, NullString, NullString,
                                        '            TRA_Fila,
                                        '            SU_Fila,
                                        '            ZeroDecimal,
                                        '            ZeroString,
                                        '            ZeroInt,
                                        '            ZeroInt, ZeroInt, ZeroInt, ZeroInt, ZeroInt, ZeroInt, ZeroString, ZeroInt, ZeroInt, ZeroInt, ZeroInt, ZeroInt, Codice_fiscale_tecnico, ZeroString,
                                        '            Grfi_Cod,
                                        '            Imp_Cod,
                                        '            CInt(1),
                                        '            ZeroInt,
                                        '            ZeroInt,
                                        '            Cop_Cod,
                                        '            Foral_Cod,
                                        '            ZeroInt, ZeroInt,
                                        '            Validita_Inizio,
                                        '            Validita_Fine_Appezzamento,
                                        '            BaseCode,
                                        '            TopCode,
                                        '            Unita_Vitata,
                                        '            Data_Inizio_Portinnesto)

                                        StrRegImpianto = "<DatiReg_Impianti>" & xmlImpianto.OuterXml & "</DatiReg_Impianti>"

                                        'salvataggio!!!
                                        InseritoImpianto = objReg_Impianto_W.Reg_Impianto_Scrivi(
                                                                        CStr(StrRegImpianto),
                                                                        OUTPUT_Piva,
                                                                        OUTPUT_Sa_Cod,
                                                                        OUTPUT_Appezza,
                                                                        OUTPUT_Id_Reg,
                                                                        "",
                                                                        objParametri_Server)


                                        If InseritoImpianto Then

                                            '  Drudi, scopiazzato dal ribalta fatto da vanni 21/11/2017 15:58:43: se esiste, ribalto anche la cartografia.
                                            Dim leggiVerificaRibalta As New AgronicaCoreGisDAL.GIS_Entita_R
                                            Dim dtVerificaRibalta As DataTable =
                                            leggiVerificaRibalta.PlanningRibaltaFastLeggiEntita(Appezza_Ribalta.Programmazione_Entita_Cod, objParametri_Server)

                                            If dtVerificaRibalta.Rows.Count > 0 Then

                                                Dim Entita_Cod As Integer
                                                Dim ElementiGrafici_Cod As Integer
                                                Dim ObjSequenze As New Agro_Sequenze

                                                'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
                                                Entita_Cod = ObjSequenze.NuovoId_Tabella("gis_entita", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri_Server)
                                                ElementiGrafici_Cod = ObjSequenze.NuovoId_Tabella("gis_elementigrafici", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri_Server)

                                                'Entita_Cod = ObjSequenze.Agronica_SequenzaTabelle_NuovoID("gis_entita", objParametri_Server)
                                                'ElementiGrafici_Cod = ObjSequenze.Agronica_SequenzaTabelle_NuovoID("gis_elementigrafici", objParametri_Server)

                                                Dim ScriviEntita As New AgronicaCoreGisDAL.GIS_Entita_W
                                                ScriviEntita.PlanningRibaltaFast(Entita_Cod, Appezza_Ribalta.Programmazione_Entita_Cod, OUTPUT_Piva, OUTPUT_Sa_Cod, OUTPUT_Appezza, OUTPUT_Id_Reg, objParametri_Server)

                                                Dim scriviElementiGrafici As New AgronicaCoreGisDAL.GIS_ElementiGrafici_W
                                                scriviElementiGrafici.PlanningRibaltaFast(Entita_Cod, ElementiGrafici_Cod, Appezza_Ribalta.Programmazione_Entita_Cod, objParametri_Server)

                                            End If

                                            '---------------------------------
                                            'caso destinazioni d'uso
                                            Dim strId_Cod As String = ""
                                            If Id_Cod <> 0 Then
                                                objReg_Impianti_Codici_W.ScrivixProgetto(Key_Piva, Key_SaCod, OUTPUT_Appezza, OUTPUT_Id_Reg, Key_ProgettoCod,
                                                                                Id_Cod,
                                                                                "",
                                                                                Validita_Inizio, Validita_Fine, objParametri_Server)
                                            End If

                                            If TRA_Fila <> 0 Then
                                                objReg_Impianti_Codici_W.Scrivi(Key_Piva, Key_SaCod, OUTPUT_Appezza, OUTPUT_Id_Reg,
                                                                        enum_CodiciAnagrafe.Impianto_TraFila_Maschio,
                                                                        TRA_Fila,
                                                                        Validita_Inizio, Validita_Fine, objParametri_Server)
                                            End If

                                            If SU_Fila <> 0 Then
                                                objReg_Impianti_Codici_W.Scrivi(Key_Piva, Key_SaCod, OUTPUT_Appezza, OUTPUT_Id_Reg,
                                                                        enum_CodiciAnagrafe.Impianto_SuFila_Maschio,
                                                                        SU_Fila,
                                                                        Validita_Inizio, Validita_Fine, objParametri_Server)
                                            End If

                                            If Pratica_Cod <> "" Then
                                                'DRUDI INSERIRE PRATICA
                                                objReg_Impianti_Codici_W.Scrivi(Key_Piva, Key_SaCod, OUTPUT_Appezza, OUTPUT_Id_Reg,
                                                                        enum_CodiciAnagrafe.Programmazione_Impianto_Pratica_Cod,
                                                                        CStr(Pratica_Cod),
                                                                        Validita_Inizio, Validita_Fine, objParametri_Server)
                                            End If

                                            'CREO LA DISTINTA
                                            StrProgetto = ""


                                            objXML.Xml_ProgettoPerImpianto(enum_CodificaDecodifica.Codifica,
                                                               StrProgetto,
                                                               enum_TipoOperazioneDB.Scrittura,
                                                               Key_Piva,
                                                               Key_SaCod,
                                                               ZeroInt,
                                                               Progetto_Nome,
                                                               NullString,
                                                               enum_Agenda_Causali.Progetto_Produzione_Agricola,
                                                               ZeroInt,
                                                               ZeroInt,
                                                               Data_Semina,
                                                               Data_Raccolta,
                                                               NullString,
                                                               OUTPUT_Appezza,
                                                               OUTPUT_Id_Reg,
                                                               0,
                                                               0,
                                                               Stato_Impianto,
                                                               Regolamento_Cod,
                                                               Disciplinare_Cod,
                                                               Regolamento_Concimazioni_Cod,
                                                               ZeroInt,
                                                               ZeroDecimal,
                                                               Resa,
                                                               Validita_Inizio,
                                                               Validita_Fine,
                                                               BaseCode,
                                                               TopCode,
                                                               Disciplinare_PubblicoPrivato:=Flag_PubblicoPrivato,
                                                               Data_Fioritura_Prevista:=Data_Fioritura_Prevista,
                                                               Mat_Cod:=Mat_Cod)

                                            InseritoProgetto = objProgetto_W.Impresa_Progetto_Scrivi(
                                                                            CStr(StrProgetto),
                                                                            OUTPUT_Piva,
                                                                            OUTPUT_Progetto_Cod,
                                                                            objParametri_Server)

                                            If InseritoProgetto Then

                                                If Salva_Limite_N Then
                                                    objReg_Impianti_Codici_W.ScrivixProgetto(Key_Piva, Key_SaCod, OUTPUT_Appezza, OUTPUT_Id_Reg, OUTPUT_Progetto_Cod,
                                                                                     enum_CodiciAnagrafe.Impianto_LimiteN,
                                                                                     ValLimiteN,
                                                                                     Validita_Inizio, Validita_Fine, objParametri_Server)
                                                Else
                                                    If Limite_N <> "" AndAlso Limite_N <> "0" Then
                                                        objReg_Impianti_Codici_W.ScrivixProgetto(Key_Piva, Key_SaCod, OUTPUT_Appezza, OUTPUT_Id_Reg, OUTPUT_Progetto_Cod,
                                                                                     enum_CodiciAnagrafe.Impianto_LimiteN,
                                                                                     Limite_N,
                                                                                     Validita_Inizio, Validita_Fine, objParametri_Server)
                                                    End If

                                                    If Limite_P <> "" AndAlso Limite_P <> "0" Then
                                                        objReg_Impianti_Codici_W.ScrivixProgetto(Key_Piva, Key_SaCod, OUTPUT_Appezza, OUTPUT_Id_Reg, OUTPUT_Progetto_Cod,
                                                                                     enum_CodiciAnagrafe.Impianto_LimiteP,
                                                                                     Limite_P,
                                                                                     Validita_Inizio, Validita_Fine, objParametri_Server)
                                                    End If

                                                    If Limite_K <> "" AndAlso Limite_K <> "0" Then
                                                        objReg_Impianti_Codici_W.ScrivixProgetto(Key_Piva, Key_SaCod, OUTPUT_Appezza, OUTPUT_Id_Reg, OUTPUT_Progetto_Cod,
                                                                                     enum_CodiciAnagrafe.Impianto_LimiteK,
                                                                                     Limite_K,
                                                                                     Validita_Inizio, Validita_Fine, objParametri_Server)
                                                    End If
                                                End If

                                                If Piano_Semina <> "" Then
                                                    objReg_Impianti_Codici_W.ScrivixProgetto(Key_Piva, Key_SaCod, OUTPUT_Appezza, OUTPUT_Id_Reg, OUTPUT_Progetto_Cod,
                                                                                     enum_CodiciAnagrafe.Impianto_PianoSemina,
                                                                                     Piano_Semina,
                                                                                     Validita_Inizio, Validita_Fine, objParametri_Server)
                                                End If

                                                '  Vanni, 07/06/2013 14:22:20: ripristinata da Ribalta_OLD() --> Genera_Stringone_XML
                                                If Veg_Cod_Cliente <> "" Then
                                                    objReg_Impianti_Codici_W.ScrivixProgetto(Key_Piva, Key_SaCod, OUTPUT_Appezza, OUTPUT_Id_Reg, OUTPUT_Progetto_Cod,
                                                                                     enum_CodiciAnagrafe.Codice_Specie_Agea,
                                                                                     Veg_Cod_Cliente,
                                                                                    Validita_Inizio, Validita_Fine, objParametri_Server)
                                                End If

                                                If Cul_Cod_Cliente <> "" Then
                                                    objReg_Impianti_Codici_W.ScrivixProgetto(Key_Piva, Key_SaCod, OUTPUT_Appezza, OUTPUT_Id_Reg, OUTPUT_Progetto_Cod,
                                                                                     enum_CodiciAnagrafe.Codice_Cultivar_Agea,
                                                                                     Cul_Cod_Cliente,
                                                                                    Validita_Inizio, Validita_Fine, objParametri_Server)
                                                End If

                                                If IAF <> "" Then
                                                    objReg_Impianti_Codici_W.ScrivixProgetto(Key_Piva, Key_SaCod, OUTPUT_Appezza, OUTPUT_Id_Reg, OUTPUT_Progetto_Cod,
                                                                        enum_CodiciAnagrafe.Impianto_IAF_ImpegniAggiuntiviFacoltativi,
                                                                        IAF,
                                                                        Validita_Inizio, Validita_Fine, objParametri_Server)
                                                End If

                                                If CapitolatoPrivato <> "" Then
                                                    objReg_Impianti_Codici_W.ScrivixProgetto(Key_Piva, Key_SaCod, OUTPUT_Appezza, OUTPUT_Id_Reg, OUTPUT_Progetto_Cod,
                                                                        enum_CodiciAnagrafe.Capitolato_Privato,
                                                                        CapitolatoPrivato,
                                                                        Validita_Inizio, Validita_Fine, objParametri_Server)
                                                End If

                                                If Finalita_Concimazione_Impianto <> 0 Then
                                                    objReg_Impianti_Codici_W.ScrivixProgetto(Key_Piva, Key_SaCod, OUTPUT_Appezza, OUTPUT_Id_Reg, OUTPUT_Progetto_Cod,
                                                                        enum_CodiciAnagrafe.Finalita_Concimazione_Impianto,
                                                                        Finalita_Concimazione_Impianto,
                                                                        Validita_Inizio, Validita_Fine, objParametri_Server)
                                                End If

                                                If KPIN <> "" Then
                                                    objReg_Impianti_Codici_W.ScrivixProgetto(Key_Piva, Key_SaCod, OUTPUT_Appezza, OUTPUT_Id_Reg, OUTPUT_Progetto_Cod,
                                                                        enum_CodiciAnagrafe.Zespri_Codice_kPIN,
                                                                        CStr(KPIN),
                                                                        Validita_Inizio, Validita_Fine, objParametri_Server)
                                                End If

                                                If Block_Name <> "" Then
                                                    objReg_Impianti_Codici_W.ScrivixProgetto(Key_Piva, Key_SaCod, OUTPUT_Appezza, OUTPUT_Id_Reg, OUTPUT_Progetto_Cod,
                                                                        enum_CodiciAnagrafe.Zespri_Block_Name,
                                                                        CStr(Block_Name),
                                                                        Validita_Inizio, Validita_Fine, objParametri_Server)
                                                End If

                                                Dim reg_impianti_programmazione_w As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Programmazioni_W
                                                reg_impianti_programmazione_w.Scrivi(
                                                    Key_Piva, Key_SaCod, OUTPUT_Appezza, OUTPUT_Id_Reg, OUTPUT_Progetto_Cod,
                                                    key_programmazione_cod, Key_Entita, Validita_Inizio, Validita_Fine,
                                                    objParametri_Server
                                                )



                                            End If

                                        End If

                                    End If
                                End If
                        End Select

                        PrimoApp = False

                        objProgrammazione.Aggiorna_Stato_Ribaltamento(Appezza_Ribalta.Programmazione_Cod,
                                                                      Appezza_Ribalta.Programmazione_Entita_Cod,
                                                                      Appezza_Ribalta.Piva, 0, 0, 0, 0,
                                                                      enum_Programmazione_Entita_Stato_Ribaltamento.Ribaltato,
                                                                      objParametri_Server)

                    End If
                Next

                ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)

                r.RispostaOK = True
                r.RispostaStringa = "Ribaltamento Completato"

            Else
                r.RispostaOK = True
                r.Errore = "Selezionare almeno un appezzamento"
            End If
        Catch ex As Exception
            r.RispostaOK = False

            If objParametri_Server.objTransazione IsNot Nothing Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        Finally
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)
        End Try

        Return r
    End Function

    Public Sub ControlloRibaltamento_EntitaPratiche(obj_Dati As RibaltaAppezza(), ByVal objParametri_Server As AgronicaCoreParametri, ByVal objParametri_Utenti As AgronicaCoreParametri)

        Dim PossoRibaltare = True
        Dim objGruppi_Utenti As New AgronicaCoreUtentiDAL.Gruppi_Utente_R
        Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_R

        Dim DTGruppoUtente = objUtenti.Leggi(objParametri_Server.UtenteUsername, "", "", "", objParametri_Server)
        If DTGruppoUtente.Rows.Count > 0 Then
            Dim DTConfigurazioneGruppo = objGruppi_Utenti.Leggi(DTGruppoUtente.Rows(0)("Gruppi_Utente_cod"), "", "", objParametri_Server)
            If DTConfigurazioneGruppo.Rows.Count > 0 AndAlso Not IsDBNull(DTConfigurazioneGruppo.Rows(0)("ConfigurazioniAggiuntive")) Then
                Dim ConfigurazioneGruppoStr = CStr(DTConfigurazioneGruppo.Rows(0)("ConfigurazioniAggiuntive"))
                If ConfigurazioneGruppoStr <> "" Then
                    Try
                        Dim ConfigurazioneGruppo = JObject.Parse(ConfigurazioneGruppoStr)
                        If ConfigurazioneGruppo("ConfigurazioniRibaltamentoEntita") IsNot Nothing Then
                            Dim objConfigurazioniRibaltamentoEntita = JArray.Parse(CStr(ConfigurazioneGruppo("ConfigurazioniRibaltamentoEntita")))
                            Dim objProgrammazione = New AgronicaCoreAnagrafeDAL.Programmazione_Testata_R
                            Dim Tipo_Planning = -1
                            For Each rowConfigurazione In objConfigurazioniRibaltamentoEntita
                                Dim Tipo_Planning_Conf = CInt(rowConfigurazione("Tipo_Planning"))
                                Dim Servizio_Cod = rowConfigurazione("Servizio_Cod")
                                Dim stati = JArray.Parse(rowConfigurazione("Stati"))
                                Dim i = 0
                                For Each entita In obj_Dati
                                    If i = 0 Then
                                        Dim dtProgramma = objProgrammazione.Leggi("", entita.Programmazione_Cod, "", "", 1, AGRODATAINIZIO, AGRODATAFINE,
                                                                enum_TipoRicetta.Non_Filtrare, enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                "", "", objParametri_Server)
                                        If dtProgramma.Rows.Count > 0 Then
                                            Tipo_Planning = dtProgramma.Rows(0)("Tipo_Pianificazione")
                                        End If
                                    End If
                                    If Tipo_Planning <> Tipo_Planning_Conf Then
                                        Exit For
                                    End If
                                    If entita.stato_ribaltamento IsNot Nothing AndAlso entita.stato_ribaltamento = 1 Then

                                    End If
                                    i += 1
                                Next
                            Next
                        End If
                    Catch ex As Exception

                    End Try
                End If
            End If
        End If

    End Sub

    Public Sub ControlloScrittura_EntitaPratiche(obj_Dati As RibaltaAppezza(), ByVal objParametri_Server As AgronicaCoreParametri, ByVal objParametri_Utenti As AgronicaCoreParametri)

        Dim PossoRibaltare = True
        Dim objGruppi_Utenti As New AgronicaCoreUtentiDAL.Gruppi_Utente_R
        Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_R

        Dim DTGruppoUtente = objUtenti.Leggi(objParametri_Server.UtenteUsername, 0, "", "", objParametri_Utenti)
        If DTGruppoUtente.Rows.Count > 0 Then
            Dim DTConfigurazioneGruppo = objGruppi_Utenti.Leggi(DTGruppoUtente.Rows(0)("Gruppi_Utente_cod"), "", "", objParametri_Utenti)
            If DTConfigurazioneGruppo.Rows.Count > 0 AndAlso Not IsDBNull(DTConfigurazioneGruppo.Rows(0)("ConfigurazioniAggiuntive")) Then
                Dim ConfigurazioneGruppoStr = CStr(DTConfigurazioneGruppo.Rows(0)("ConfigurazioniAggiuntive"))
                If ConfigurazioneGruppoStr <> "" Then
                    Try
                        Dim ConfigurazioneGruppo = JObject.Parse(ConfigurazioneGruppoStr)
                        If ConfigurazioneGruppo("ConfigurazioniScritturaEntita") IsNot Nothing Then
                            Dim objConfigurazioniRibaltamentoEntita = JArray.Parse(ConfigurazioneGruppo("ConfigurazioniScritturaEntita").ToString)
                            Dim objProgrammazione = New AgronicaCoreAnagrafeDAL.Programmazione_Testata_R
                            Dim objPratiche As New AgronicaCoreProfilazioneDAL.Pratiche_R
                            Dim Tipo_Planning = -1
                            For Each rowConfigurazione In objConfigurazioniRibaltamentoEntita
                                Dim Tipo_Planning_Conf = CInt(rowConfigurazione("Tipo_Planning"))
                                Dim Servizio_Cod = rowConfigurazione("Servizio_Cod")
                                Dim i = 0
                                For Each entita In obj_Dati
                                    If i = 0 Then
                                        Dim dtProgramma = objProgrammazione.Leggi("", entita.Programmazione_Cod, "", "", 1, AGRODATAINIZIO, AGRODATAFINE,
                                                                enum_TipoRicetta.Non_Filtrare, enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                "", "", objParametri_Server)
                                        If dtProgramma.Rows.Count > 0 Then
                                            Tipo_Planning = dtProgramma.Rows(0)("Tipo_Pianificazione")
                                        End If
                                    End If
                                    If Tipo_Planning <> Tipo_Planning_Conf Then
                                        Exit For
                                    End If

                                    Dim dtPratiche = objPratiche.Leggi(0, "", entita.Piva, "", 0, 0, 0, Servizio_Cod, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server, 0, entita.Programmazione_Entita_Cod)
                                    If dtPratiche.Rows.Count = 0 Then
                                        'CREO LA PRATICA MANCANTE
                                        Crea_Pratica_da_Entita(entita, Servizio_Cod, objParametri_Server)
                                    End If

                                    i += 1
                                Next
                            Next
                        End If
                    Catch ex As Exception

                    End Try
                End If
            End If
        End If

    End Sub


    Public Sub ControlloScrittura_EntitaPratiche(Programmazione_Entita_Cod As Integer, Validita_Inizio As Date, Validita_Fine As Date, Piva As String, ByVal objParametri_Server As AgronicaCoreParametri, ByVal objParametri_Utenti As AgronicaCoreParametri)

        Dim PossoRibaltare = True
        Dim objGruppi_Utenti As New AgronicaCoreUtentiDAL.Gruppi_Utente_R
        Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_R

        Dim DTGruppoUtente = objUtenti.Leggi(objParametri_Server.UtenteUsername, 0, "", "", objParametri_Utenti)
        If DTGruppoUtente.Rows.Count > 0 Then
            Dim DTConfigurazioneGruppo = objGruppi_Utenti.Leggi(DTGruppoUtente.Rows(0)("Gruppi_Utente_cod"), "", "", objParametri_Utenti)
            If DTConfigurazioneGruppo.Rows.Count > 0 AndAlso Not IsDBNull(DTConfigurazioneGruppo.Rows(0)("ConfigurazioniAggiuntive")) Then
                Dim ConfigurazioneGruppoStr = CStr(DTConfigurazioneGruppo.Rows(0)("ConfigurazioniAggiuntive"))
                If ConfigurazioneGruppoStr <> "" Then
                    Try
                        Dim ConfigurazioneGruppo = JObject.Parse(ConfigurazioneGruppoStr)
                        If ConfigurazioneGruppo("ConfigurazioniScritturaEntita") IsNot Nothing Then
                            Dim objConfigurazioniRibaltamentoEntita = JArray.Parse(ConfigurazioneGruppo("ConfigurazioniScritturaEntita").ToString)
                            Dim objProgrammazione = New AgronicaCoreAnagrafeDAL.Programmazione_Testata_R
                            Dim objProgrammazione_Entita = New AgronicaCoreAnagrafeDAL.Programmazione_Entita_R
                            Dim dt_entita = objProgrammazione_Entita.Leggi(0, Programmazione_Entita_Cod, "", "", 0, 0, 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE, "", "", "", objParametri_Server)
                            If dt_entita.Rows.Count > 0 AndAlso Programmazione_Entita_Cod <> 0 Then
                                Dim Programmazione_Cod As Integer = dt_entita.Rows(0)("Programmazione_Cod")
                                Dim objPratiche As New AgronicaCoreProfilazioneDAL.Pratiche_R
                                Dim Tipo_Planning = -1
                                For Each rowConfigurazione In objConfigurazioniRibaltamentoEntita
                                    Dim Tipo_Planning_Conf = CInt(rowConfigurazione("Tipo_Planning"))
                                    Dim Servizio_Cod = rowConfigurazione("Servizio_Cod")
                                    Dim i = 0

                                    If i = 0 Then
                                        Dim dtProgramma = objProgrammazione.Leggi("", Programmazione_Cod, "", "", 1, AGRODATAINIZIO, AGRODATAFINE,
                                                                enum_TipoRicetta.Non_Filtrare, enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                "", "", objParametri_Server)
                                        If dtProgramma.Rows.Count > 0 Then
                                            Tipo_Planning = dtProgramma.Rows(0)("Tipo_Pianificazione")
                                        End If
                                    End If
                                    If Tipo_Planning <> Tipo_Planning_Conf Then
                                        Exit For
                                    End If

                                    Dim dtPratiche = objPratiche.Leggi(0, "", Piva, "", 0, 0, 0, Servizio_Cod, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server, 0, Programmazione_Entita_Cod)
                                    If dtPratiche.Rows.Count = 0 Then
                                        'CREO LA PRATICA MANCANTE
                                        Crea_Pratica_da_Entita(Piva, Programmazione_Cod, Programmazione_Entita_Cod, Validita_Inizio, Validita_Fine, Servizio_Cod, objParametri_Server)
                                    End If

                                    i += 1
                                Next
                            End If
                        End If
                    Catch ex As Exception

                    End Try
                End If
            End If
        End If

    End Sub


    Public Sub Crea_Pratica_da_Entita(entita As RibaltaAppezza, Servizio_Cod As Integer, objParametri_Server As AgronicaCoreParametri)
        Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
        Dim Cuaa = objImprese.Leggi_CUAA(entita.Piva, objParametri_Server)
        Dim Validita_Inizio = entita.Validita_Inizio
        Dim Validita_Fine = entita.Validita_Fine
        Dim objServizi As New AgronicaCoreMetaSchemaDAL.Servizi_R
        Dim Servizio_Des = objServizi.Leggi(Servizio_Cod, "", AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server).Rows(0)("Servizio_Des")

        Dim StatoIniziale_Cod As Integer = 0

        Dim objPratiche_R As New AgronicaCoreProfilazioneDAL.Pratiche_R
        Dim objPratica_W As New AgronicaCoreProfilazioneDAL.Pratiche_W
        Dim objPraticaStati_W As New AgronicaCoreProfilazioneDAL.Pratiche_Stati_W
        Dim objPraticaStati_Attuale_W As New AgronicaCoreProfilazioneDAL.Pratiche_Stati_Attuali_W

        Dim LeggiSequenze As New Agro_Sequenze
        Dim NuovoPassaggioDiStato_cod As Integer = 0
        NuovoPassaggioDiStato_cod = LeggiSequenze.NuovoId_Tabella(
                             "PassaggioDiStato_cod",
                             0,
                             2000000000,
                             objParametri_Server
                         )

        Dim objSequenza As New Agro_Sequenze
        Dim Pratica_Cod = objSequenza.NuovoId_Tabella("Pratiche", 0, 2000000000, objParametri_Server)
        Dim InseritaPratica = objPratica_W.Scrivi(Pratica_Cod,
                                              Servizio_Des,
                                              entita.Piva,
                                              Cuaa,
                                              0, 0, 0,
                                              Servizio_Cod,
                                              Validita_Inizio, Validita_Fine,
                                              objParametri_Server,
                                              Date.Now, Date.Now,
                                              objParametri_Server.UsernameOperazione,
                                              objParametri_Server.UsernameOperazione, 0, "", "", AGRODATAINIZIO, "", 0, entita.Programmazione_Entita_Cod)

        If InseritaPratica Then

            StatoIniziale_Cod = objPratiche_R.Leggi_StatoInizialeServizio(Servizio_Cod, "", objParametri_Server)
            ScrivoIlPassaggioDiStatoIniziale(StatoIniziale_Cod, NuovoPassaggioDiStato_cod, Servizio_Cod, Validita_Fine, Date.Now, Pratica_Cod, objPraticaStati_W, objPraticaStati_Attuale_W, objParametri_Server)

        End If

    End Sub

    Public Sub Crea_Pratica_da_Entita(Piva As String, Programmazione_Cod As Integer, Programmazione_Entita_Cod As Integer, Validita_Inizio As Date, Validita_Fine As Date, Servizio_Cod As Integer, objParametri_Server As AgronicaCoreParametri)
        Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
        Dim Cuaa = objImprese.Leggi_CUAA(Piva, objParametri_Server)
        Dim objServizi As New AgronicaCoreMetaSchemaDAL.Servizi_R
        Dim Servizio_Des = objServizi.Leggi(Servizio_Cod, "", AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server).Rows(0)("Servizio_Des")

        Dim StatoIniziale_Cod As Integer = 0

        Dim objPratiche_R As New AgronicaCoreProfilazioneDAL.Pratiche_R
        Dim objPratica_W As New AgronicaCoreProfilazioneDAL.Pratiche_W
        Dim objPraticaStati_W As New AgronicaCoreProfilazioneDAL.Pratiche_Stati_W
        Dim objPraticaStati_Attuale_W As New AgronicaCoreProfilazioneDAL.Pratiche_Stati_Attuali_W

        Dim LeggiSequenze As New Agro_Sequenze
        Dim NuovoPassaggioDiStato_cod As Integer = 0
        NuovoPassaggioDiStato_cod = LeggiSequenze.NuovoId_Tabella(
                             "PassaggioDiStato_cod",
                             0,
                             2000000000,
                             objParametri_Server
                         )

        Dim objSequenza As New Agro_Sequenze
        Dim Pratica_Cod = objSequenza.NuovoId_Tabella("Pratiche", 0, 2000000000, objParametri_Server)
        Dim InseritaPratica = objPratica_W.Scrivi(Pratica_Cod,
                                              Servizio_Des,
                                              Piva,
                                              Cuaa,
                                              0, 0, 0,
                                              Servizio_Cod,
                                              Validita_Inizio, Validita_Fine,
                                              objParametri_Server,
                                              Date.Now, Date.Now,
                                              objParametri_Server.UsernameOperazione,
                                              objParametri_Server.UsernameOperazione, 0, "", "", AGRODATAINIZIO, "", 0, Programmazione_Entita_Cod)

        If InseritaPratica Then

            StatoIniziale_Cod = objPratiche_R.Leggi_StatoInizialeServizio(Servizio_Cod, "", objParametri_Server)
            ScrivoIlPassaggioDiStatoIniziale(StatoIniziale_Cod, NuovoPassaggioDiStato_cod, Servizio_Cod, Validita_Fine, Date.Now, Pratica_Cod, objPraticaStati_W, objPraticaStati_Attuale_W, objParametri_Server)

        End If

    End Sub

    Private Shared Sub ScrivoIlPassaggioDiStatoIniziale(ByVal Stato_cod As enum_Servizi_Stati, ByVal NuovoPassaggioDiStato_cod As Integer, ByVal Servizio_Cod As Integer, ByVal DataFinePratica As Date, ByVal DataApertura As Date, ByVal Pratica_Cod As Integer, ByVal objPraticaStati_W As AgronicaCoreProfilazioneDAL.Pratiche_Stati_W, ByVal objPraticaStati_Attuale_W As AgronicaCoreProfilazioneDAL.Pratiche_Stati_Attuali_W, objParametri_Server As AgronicaCoreParametri)
        objPraticaStati_W.Scrivi(Pratica_Cod,
                    Stato_cod,
                    Servizio_Cod,
                    DataApertura, DataFinePratica,
                    "", 0, NuovoPassaggioDiStato_cod,
                    objParametri_Server,
                    Now, Now,
                    objParametri_Server.UsernameOperazione,
                    objParametri_Server.UsernameOperazione)
        objPraticaStati_Attuale_W.Scrivi(Pratica_Cod,
                    Stato_cod,
                    "",
                    DataApertura, DataFinePratica,
                    objParametri_Server,
                    Now, Now,
                    objParametri_Server.UsernameOperazione,
                    objParametri_Server.UsernameOperazione)
    End Sub

    Public Function Pianificazione_Scrivi_Testata_da_JSON(ByVal Tipo_Operazione As Integer,
                                      ByRef Programmazione_Cod_OUT As Integer,
                                      ByVal Programmazione_Des As String,
                                      ByVal Programmazione_Des_Long As String,
                                      ByVal Piva As String,
                                      ByVal Note As String,
                                      ByVal Utente As String,
                                      ByVal Validita_Inizio As Date,
                                      ByVal Validita_Fine As Date,
                                      ByVal Tipo_Pianificazione As Integer,
                                      ByVal Tipo_Fonte As Integer,
                                      ByVal strAppezzamenti As String,
                                        ByVal NumeroDiValidazione As String,
                                        ByVal DataDiValidazione As Date,
                                            ByRef objParametri As AgronicaCoreParametri,
                                            Optional ByVal Stato_SQNPI As Integer = 0,
                                            Optional ByVal SalvaAllegato As Boolean = True,
                                            Optional ByVal objParametri_Utenti As AgronicaCoreParametri = Nothing
                                      ) As RispostaStandard
        Dim rval As New RispostaStandard

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Programmazione_W.Pianificazione_Scrivi_da_JSON()"

        Dim MessaggioErrore As String = ""
        Dim ErrMSG As String = ""

        Dim Operazione As Integer = 0
        Dim strCatasto As String = ""

        Dim Veg_Cod As Integer = 0
        Dim Cul_Cod As Integer = 0
        Dim Grfi_Cod As Integer = 0
        Dim Id_Cod As Integer = 0
        Dim Grva_Cod As Integer = 0

        Dim Macrouso_Cod As String = ""

        Dim ribaltato As Integer = 0
        Dim movimentato As Integer = 0

        Dim Campo_Cod As String = NumeroDiValidazione

        Dim provenienza_fascicolo As String = ""

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = True

        Try

            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            'ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
            'FlagTransazioneLocale,
            'objParametri)

            Dim Dt_Appezzamenti As New DataTable
            Dim Dt_Particelle As New DataTable
            Dim Dt_Appezzamenti_Eliminati As New DataTable

            Dim objProgrammazione As New Programmazione_R

            objProgrammazione.DT_Appezzamenti_Crea(Dt_Appezzamenti)
            objProgrammazione.DT_Intersezioni_Crea(Dt_Particelle)
            objProgrammazione.DT_Appezzamenti_Eliminati_Crea(Dt_Appezzamenti_Eliminati)

            Programmazione_Cod_OUT = Pianificazione_Scrivi(enum_TipoOperazioneDB.Scrittura,
                                                                       Programmazione_Cod_OUT,
                                                                       Programmazione_Des, Programmazione_Des_Long,
                                                                       Piva, Note, Utente,
                                                                       Validita_Inizio, Validita_Fine,
                                                                       Tipo_Pianificazione, Tipo_Fonte,
                                                                       Dt_Appezzamenti,
                                                                       Dt_Particelle,
                                                                       Dt_Appezzamenti_Eliminati,
                                                                       NumeroDiValidazione, DataDiValidazione,
                                                                       objParametri, 0, SalvaAllegato, False, "")

            rval.RispostaOK = True


        Catch ex As Exception

            rval.RispostaOK = False

            If objParametri.objTransazione IsNot Nothing Then
                'ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            'uso questa funzione per ottenere il Messaggio..:
            rval.Errore = "Errore durante l'operazione: " & vbCrLf &
                Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        Finally
            'ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)
        End Try

        Return rval
    End Function


    Private Function ReadXmlFromString(ByVal stringaXml As String) As XDocument
        Return XDocument.Parse(stringaXml)
    End Function

    Private Function GetData_Creazione(ByVal dataCreazione As DateTime) As String
        Return AgronicaCoreUtility.DataOra.DataOraToDate_JSON_ISO8601(dataCreazione)
    End Function


    Private Sub ProgrammazioneEntitaScriviDatoCartografico(Programmazione_Cod As Integer, Piva As String, Programmazione_Entita_Cod As Integer, Sa_Cod As Integer, wkt As String, wkt_georiferimento_cod As String, ScriviElementiGrafici As AgronicaCoreGisBIZ.GIS_Entita_W, objParametri_Server As AgronicaCoreParametri)

        Dim ParametriCartografici As ParametriCoordinateConverter = Nothing

        If wkt_georiferimento_cod <> "-1" Then


            Dim leggiTrasformazione As New AgronicaCoreGisDAL.GIS_SistemiRiferimentoCartografia_R
            Dim dtLeggiTrasformazione As DataTable = leggiTrasformazione.Leggi(wkt_georiferimento_cod, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)


            ParametriCartografici = New ParametriCoordinateConverter With {
                .CSFromText = dtLeggiTrasformazione(0)("CSFrom"),
                .CStoText = dtLeggiTrasformazione(0)("CSTo"),
                .CStoGeoText = dtLeggiTrasformazione(0)("CStoGeo"),
                .AgronicaLatOffset = dtLeggiTrasformazione(0)("AgronicaLatOffset"),
                .AgronicaLonOffset = dtLeggiTrasformazione(0)("AgronicaLonOffset"),
                .LibreriaDaUsare = dtLeggiTrasformazione(0)("LibreriaDaUsare")
            }


        End If

        Dim sNodeDoc As XDocument

        Dim wktHelp As New WKT
        Dim wktToGeoML As New wkt_gml
        Dim cconverter As New Agronica.CoordinateConverter

        Dim FinalDoc As New XDocument

        Dim ElemFinalXdoc As XElement = <DatiEntita xmlns="http://www.agronica.it/grafica/"
                                            xmlns:gml="http://www.opengis.net/gml"></DatiEntita>
        FinalDoc.Add(ElemFinalXdoc)

        If ParametriCartografici Is Nothing Then

            Dim DatiCartograficiOriginali_WGS84 As List(Of xyz) =
                    wktHelp.CreaCoordinateDaWkt(wkt, False)

            sNodeDoc = ReadXmlFromString(
                wktToGeoML.Trasforma(
                    wktHelp.CreaPoligonoDaCoordinate(
                        DatiCartograficiOriginali_WGS84,
                        (DatiCartograficiOriginali_WGS84.Count = 1)
                    ),
                    True,
                    False,
                    True,
                    0)
            )
            sNodeDoc.Root.@Flag_GPS = 0
        Else

            sNodeDoc = ReadXmlFromString(
                wktToGeoML.Trasforma(
                    cconverter.WKTPolygonWGS84_from_WKTPolygonED50(wkt, True, ParametriCartografici),
                    False,
                    False,
                    True,
                    0)
            )

            sNodeDoc.Root.@Flag_GPS = 0

        End If

        Dim newEntitaElement = <Entita TipoOperazioneDB="1" recno="" deleted="" section="E" id="1" descr="" ecolor="256" eline="256" rad="15" text="" gps="0" lat="0" lon="0" pdop="0" validita_inizio="01/09/2009" validita_fine="31/08/2010" username_creazione="" username_modifica="">
                                   <layers>
                                       <layer tipologia_layer="1"><%= CInt(enum_Gis_LayerElementiGrafici_std.Impianti_Pianificati_Entita) %></layer>
                                   </layers>
                                   <EntitaGIAS>
                                       <DatoGias>
                                           <PivaSuperUser><%= objParametri_Server.PivaSuperUser %></PivaSuperUser>
                                           <Entita_Cod>0</Entita_Cod>
                                           <TipoEntita_Cod><%= CInt(enum_GIS2012_TipoEntita.PLANNING_IMPIANTI_PIANIFICATI) %></TipoEntita_Cod>
                                           <Piva><%= Piva %></Piva>
                                           <Sa_Cod><%= Sa_Cod %></Sa_Cod>
                                           <Appezza>0</Appezza>
                                           <Campo_Cod>0</Campo_Cod>
                                           <Id_Imp>0</Id_Imp>
                                           <PROV>0</PROV>
                                           <COM>0</COM>
                                           <SEZIONE>0</SEZIONE>
                                           <FOGLIO>0</FOGLIO>
                                           <NUMERO>0</NUMERO>
                                           <SUBALTERNO>0</SUBALTERNO>
                                           <Programmazione_Entita_Cod><%= Programmazione_Entita_Cod %></Programmazione_Entita_Cod>
                                           <Programmazione_Cod><%= Programmazione_Cod %></Programmazione_Cod>
                                           <Id_Agenda>0</Id_Agenda>
                                           <id_mov_det>0</id_mov_det>
                                           <Ricetta_Operazione_Cod>0</Ricetta_Operazione_Cod>
                                           <OLDGrafica_ID></OLDGrafica_ID>
                                           <analisi_campione_cod>0</analisi_campione_cod>
                                           <inviato>0</inviato>
                                           <Data_Creazione><%= GetData_Creazione(Now) %></Data_Creazione>
                                           <Data_Modifica><%= GetData_Creazione(Now) %></Data_Modifica>
                                           <Username_Creazione>agronica</Username_Creazione>
                                           <Username_Modifica>agronica</Username_Modifica>
                                           <Validita_Inizio>1900-01-01T00:00:00</Validita_Inizio>
                                           <Validita_Fine>2100-12-31T00:00:00</Validita_Fine>
                                       </DatoGias>
                                   </EntitaGIAS>
                               </Entita>

        newEntitaElement.Add(sNodeDoc.FirstNode)
        FinalDoc.Root.Add(newEntitaElement)

        Dim ns1 As XNamespace = "http://www.agronica.it/grafica/"

        Dim ListaNS As New List(Of String)
        ListaNS.Add("http://www.opengis.net/gml")

        Dim sFinalDoc1 As String = xmlHelper.RemoveNamespace(FinalDoc, ListaNS).ToString.Replace("xmlns=""""", "")
        Dim FinalDoc1 As XDocument = XDocument.Parse(sFinalDoc1)

        For Each elemento In (
            From a In FinalDoc1.Elements(ns1 + "DatiEntita").Elements(ns1 + "Entita")
            Select a).ToList()



            Try

                Dim idle As Integer
                ScriviElementiGrafici.scrivi(elemento.ToString, idle, objParametri_Server)


            Catch ex As Exception


                'My.Computer.FileSystem.WriteAllText(NonImportatiSQL, head & "<Entita>" & elemento.Elements.FirstOrDefault.ToString & "</Entita>" & tail, True)

            End Try


        Next



    End Sub


    ''' <summary>
    ''' Scrittura / Aggiornamento / Eliminazione di una pianificazione
    ''' </summary>
    ''' <param name="Tipo_Operazione"></param>
    ''' <param name="Programmazione_Cod"></param>
    ''' <param name="Programmazione_Des"></param>
    ''' <param name="Programmazione_Des_Long"></param>
    ''' <param name="Piva"></param>
    ''' <param name="Note"></param>
    ''' <param name="Utente"></param>
    ''' <param name="Validita_Inizio"></param>
    ''' <param name="Validita_Fine"></param>
    ''' <param name="Tipo_Pianificazione"></param>
    ''' <param name="Tipo_Fonte"></param>
    ''' <param name="AppezzamentiInseriti"></param>
    ''' <param name="AppezzamentiModificati"></param>
    ''' <param name="AppezzamentiCancellati"></param>
    ''' <param name="Particelle"></param>
    ''' <param name="Dt_Appezzamenti_Eliminati"></param>
    ''' <param name="NumeroDiValidazione"></param>
    ''' <param name="DataDiValidazione"></param>
    ''' <param name="objParametri"></param>
    ''' <param name="Stato_SQNPI"></param>
    ''' <returns></returns>
    Public Function Pianificazione_Scrivi_da_JSON(ByVal Tipo_Operazione As Integer,
                                      ByRef Programmazione_Cod_OUT As Integer,
                                      ByVal Programmazione_Des As String,
                                      ByVal Programmazione_Des_Long As String,
                                      ByVal Piva As String,
                                      ByVal Note As String,
                                      ByVal Utente As String,
                                      ByVal Validita_Inizio As Date,
                                      ByVal Validita_Fine As Date,
                                      ByVal Tipo_Pianificazione As Integer,
                                      ByVal Tipo_Fonte As Integer,
                                      ByVal strAppezzamenti As String,
                                      ByVal NumeroDiValidazione As String,
                                      ByVal DataDiValidazione As Date,
                                      ByRef objParametri As AgronicaCoreParametri,
                                      Optional ByVal Stato_SQNPI As Integer = 0,
                                      Optional ByVal SalvaAllegato As Boolean = True,
                                      Optional ByVal objParametri_Utenti As AgronicaCoreParametri = Nothing,
                                      Optional ByVal Pratica_Cod As String = "",
                                      Optional ByVal importatoAutomaticamente As Boolean = False
                                      ) As RispostaStandard

        Dim rval As New RispostaStandard

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Programmazione_W.Pianificazione_Scrivi_da_JSON()"

        Dim MessaggioErrore As String = ""
        Dim ErrMSG As String = ""

        Dim righeInseriteArray As JArray

        Dim Operazione As Integer = 0
        Dim strCatasto As String = ""

        Dim Validita_Inizio_App As Date
        Dim Validita_Fine_App As Date

        Dim Veg_Cod As Integer = 0
        Dim Cul_Cod As Integer = 0
        Dim Grfi_Cod As Integer = 0
        Dim Id_Cod As Integer = 0
        Dim Grva_Cod As Integer = 0

        Dim Veg_Cod_Agea As String
        Dim Cul_Cod_Agea As String

        Dim Uso_Cod_Agea As String
        Dim Occupazione_Cod_Agea As String
        Dim Destinazione_Cod_Agea As String
        Dim Qualita_Cod_Agea As String

        Dim Macrouso_Cod As String = ""

        Dim ribaltato As Integer = 0
        Dim movimentato As Integer = 0

        Dim Sa_Cod As Integer
        Dim Cop_Cod As Integer
        Dim Lotto As String
        Dim Resa As Double
        Dim Num_Piante As Integer
        Dim TRA_Fila As Double
        Dim SU_Fila As Double
        Dim Validita_Inizio_Impianto As Date
        Dim Metodo_Produzione_Cod As Integer
        Dim Unita_Vitata As Integer
        Dim Disciplinare_Cod As Integer
        Dim Regolamento_Cod As Integer
        Dim Regolamento_Concimazione_Cod As Integer
        Dim Flag_PubblicoPrivato As Integer
        Dim id_tr As Integer
        Dim Stato_Cod As Integer

        Dim Limite_N As String
        Dim Limite_P As String
        Dim limite_K As String
        Dim Data_Semina As Date
        Dim Data_Raccolta As Date
        Dim Data_Fioritura_Prevista As Date
        Dim Veg_Cod_Prec As Integer
        Dim Veg_Cod_Prec2 As Integer
        Dim Veg_Cod_Prec3 As Integer
        Dim Veg_Cod_Prec4 As Integer
        Dim Piano_Semina As String
        Dim Codice_Contratto As String
        Dim Codice_Fiscale_Tecnico As String
        Dim Campo_Cod As Integer
        Dim IAF As String

        Dim TipoZona As String

        Dim DistBZ_CorpiIdrici As Double
        Dim DistBZ_AreeResPub As Double
        Dim DistBZ_Allevamenti As Double
        Dim DistBZ_VegNatNonColt As Double
        Dim SupBZ_Riduzione As Double

        Dim Finalita_Concimazione_Impianto As Integer

        Dim cod_indirizzo As Integer
        Dim ind_des As String
        Dim frz_des As String
        Dim CAP As String
        Dim com_des_indirizzo, pro_cod_indirizzo, stato_indirizzo, stato_indirizzo_des, note_indirizzo, pro_cod_istat_indirizzo, com_cod_istat_indirizzo As String

        Dim KPIN, Block_Name As String

        Dim Foral_Cod As Integer

        Dim Data_Inizio_Portinnesto As String

        Dim wkt As String = ""
        Dim wkt_georiferimento_cod = "-1"

        Dim Isola As String = ""
        Dim Riferimento_Alfanumerico_Appezzamento As String = ""

        Dim CapitolatoPrivato As String = ""

        Dim ZespriFase As String = ""
        Dim ZespriTipo As String = ""
        Dim ZespriGrower As String = ""

        Dim Num_Piante_Femmine As Integer = 0
        Dim Num_Piante_Maschi As Integer = 0

        Dim Port_cod As Integer = 0
        Dim TipologiaInnestoTrapianto_cod As Integer = 0
        Dim Mat_Cod As Integer = 0

        Dim Appezza As Integer

        Dim Stato_Ribaltamento As enum_Programmazione_Entita_Stato_Ribaltamento
        Dim provenienza_fascicolo As String = ""

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try

            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                           FlagTransazioneLocale,
                                                           objParametri)


            Dim strEntitaDaMantenere As String = ""

            If strAppezzamenti <> "" Then

                righeInseriteArray = JArray.Parse(strAppezzamenti)
                AgronicaCoreUtility.DataOra.JarrayAggiustaDate(righeInseriteArray)

                If righeInseriteArray IsNot Nothing Then

                    For Each r In righeInseriteArray
                        If (Not String.IsNullOrEmpty(Trim(r("movimentato"))) AndAlso Trim(r("movimentato")) = "1") Then
                            'Or (Not String.IsNullOrEmpty(Trim(r("datoGis"))) AndAlso Trim(r("datoGis")) = "1") 
                            strEntitaDaMantenere &= r("programmazione_entita_cod").ToString & ","
                        End If
                        If (Not String.IsNullOrEmpty(Trim(r("ribaltato"))) AndAlso Trim(r("ribaltato")) = "1") Then
                            If r("Selected") Is Nothing Then
                                strEntitaDaMantenere &= r("programmazione_entita_cod").ToString & ","
                            ElseIf Trim(r("Selected")) = "" Then
                                strEntitaDaMantenere &= r("programmazione_entita_cod").ToString & ","
                            ElseIf Trim(r("Selected")) <> "" AndAlso Trim(r("Selected")) = False Then
                                strEntitaDaMantenere &= r("programmazione_entita_cod").ToString & ","
                            End If
                        End If
                        If (Not String.IsNullOrEmpty(Trim(r("ribaltato"))) AndAlso Trim(r("ribaltato")) = "2") Then
                            If r("Selected") Is Nothing Then
                                strEntitaDaMantenere &= r("programmazione_entita_cod").ToString & ","
                            ElseIf Trim(r("Selected")) = "" Then
                                strEntitaDaMantenere &= r("programmazione_entita_cod").ToString & ","
                            ElseIf Trim(r("Selected")) <> "" AndAlso Trim(r("Selected")) = False Then
                                strEntitaDaMantenere &= r("programmazione_entita_cod").ToString & ","
                            End If
                        End If
                    Next
                    If strEntitaDaMantenere <> "" Then
                        strEntitaDaMantenere = Left(strEntitaDaMantenere, strEntitaDaMantenere.Length - 1)
                    End If
                End If

            End If


            'Se sono in modifica prima elimino planning e reale poi riscrivo
            If Tipo_Operazione = enum_TipoOperazioneDB.Modifica Then

                Programmazione_Cod_OUT = Pianificazione_Scrivi(enum_TipoOperazioneDB.Cancellazione,
                                            Programmazione_Cod_OUT,
                                            Programmazione_Des, Programmazione_Des_Long,
                                            Piva, Note, Utente,
                                            Validita_Inizio, Validita_Fine,
                                            Tipo_Pianificazione, Tipo_Fonte,
                                            Nothing,
                                            Nothing,
                                            Nothing,
                                            NumeroDiValidazione, DataDiValidazione,
                                            objParametri, 0, SalvaAllegato,
                                                               True,
                                                               strEntitaDaMantenere, objParametri_Utenti)

            End If


            If strAppezzamenti <> "" AndAlso strAppezzamenti <> " [  ]" AndAlso strAppezzamenti <> "[]" Then

                righeInseriteArray = JArray.Parse(strAppezzamenti)
                AgronicaCoreUtility.DataOra.JarrayAggiustaDate(righeInseriteArray)

                Dim minAppezza As Integer = (
                    From ss In righeInseriteArray
                    Select ss("appezza")
                    ).Min()

                minAppezza -= 1

                Dim listEntitaDaMantenere = strEntitaDaMantenere.Split(",")

                If righeInseriteArray IsNot Nothing Then

                    Dim Dt_Appezzamenti As New DataTable
                    Dim Dt_Particelle As New DataTable
                    Dim Dt_Appezzamenti_Eliminati As New DataTable

                    Dim objProgrammazione As New Programmazione_R

                    objProgrammazione.DT_Appezzamenti_Crea(Dt_Appezzamenti)
                    objProgrammazione.DT_Intersezioni_Crea(Dt_Particelle)
                    objProgrammazione.DT_Appezzamenti_Eliminati_Crea(Dt_Appezzamenti_Eliminati)

                    For Each r In righeInseriteArray

                        '(8/11/2017 fede) scrivo tutta la tabella sul planning
                        'ribalterò in anagrafica solo le righe selezionate
                        Stato_Ribaltamento = enum_Programmazione_Entita_Stato_Ribaltamento.NonDefinito

                        If r("Selected") IsNot Nothing AndAlso CBool(r("Selected")) = True Then
                            Stato_Ribaltamento = enum_Programmazione_Entita_Stato_Ribaltamento.DaRibaltare
                        End If

                        If r("Selected") IsNot Nothing AndAlso CBool(r("Selected")) = False AndAlso (Not String.IsNullOrEmpty(Trim(r("ribaltato")))) AndAlso Trim(r("ribaltato")) = "1" Then
                            Continue For
                        End If

                        If listEntitaDaMantenere.Contains(CStr(r("programmazione_entita_cod"))) Then
                            Continue For
                        End If

                        'If r("Selected") IsNot Nothing AndAlso CBool(r("Selected")) = True Then

                        If Not String.IsNullOrEmpty(Trim(r("validita_inizio"))) Then
                            Validita_Inizio_App = AgronicaCoreUtility.DataOra.DataStringaJSON_to_ISO8601(r("validita_inizio"))
                        Else
                            Validita_Inizio_App = AGRODATAINIZIO
                        End If

                        If Not String.IsNullOrEmpty(Trim(r("validita_fine"))) Then
                            Validita_Fine_App = AgronicaCoreUtility.DataOra.DataStringaJSON_to_ISO8601(r("validita_fine"))
                        Else
                            Validita_Fine_App = AGRODATAFINE
                        End If

                        Campo_Cod = 0

                        Veg_Cod = 0
                        Cul_Cod = 0
                        Grfi_Cod = 0
                        Id_Cod = 0
                        Grva_Cod = 0


                        Cop_Cod = 0
                        Lotto = ""
                        Resa = 0
                        Num_Piante = 0
                        TRA_Fila = 0
                        SU_Fila = 0
                        Validita_Inizio_Impianto = AGRODATAINIZIO
                        Metodo_Produzione_Cod = 0
                        Unita_Vitata = 0
                        Disciplinare_Cod = 0
                        Regolamento_Cod = 0
                        Regolamento_Concimazione_Cod = 0
                        Flag_PubblicoPrivato = 0
                        id_tr = 0
                        Stato_Cod = 0
                        Limite_N = ""
                        Limite_P = ""
                        limite_K = ""
                        Data_Semina = AGRODATAINIZIO
                        Data_Raccolta = AGRODATAFINE
                        Data_Fioritura_Prevista = AGRODATAINIZIO
                        Veg_Cod_Prec = 0
                        Veg_Cod_Prec2 = 0
                        Veg_Cod_Prec3 = 0
                        Veg_Cod_Prec4 = 0
                        Piano_Semina = ""
                        Codice_Contratto = ""
                        Codice_Fiscale_Tecnico = ""
                        IAF = ""

                        DistBZ_CorpiIdrici = 0
                        DistBZ_AreeResPub = 0
                        DistBZ_Allevamenti = 0
                        DistBZ_VegNatNonColt = 0
                        SupBZ_Riduzione = 0

                        Finalita_Concimazione_Impianto = 0

                        cod_indirizzo = 0
                        ind_des = ""
                        frz_des = ""
                        CAP = ""
                        com_des_indirizzo = ""
                        pro_cod_indirizzo = ""
                        stato_indirizzo = ""
                        stato_indirizzo_des = ""
                        note_indirizzo = ""
                        pro_cod_istat_indirizzo = ""
                        com_cod_istat_indirizzo = ""

                        Isola = ""
                        Riferimento_Alfanumerico_Appezzamento = ""

                        CapitolatoPrivato = ""

                        KPIN = ""
                        Block_Name = ""

                        Foral_Cod = 0
                        Mat_Cod = 0

                        Data_Inizio_Portinnesto = ""

                        ZespriFase = "0"
                        ZespriTipo = "0"
                        ZespriGrower = "0"

                        Num_Piante_Femmine = 0
                        Num_Piante_Maschi = 0

                        Sa_Cod = 0

                        If IsNumeric(r("cul_cod")) Then
                            Cul_Cod = r("cul_cod")
                        End If
                        If IsNumeric(r("grva_cod")) Then
                            Grva_Cod = r("grva_cod")
                        End If
                        If IsNumeric(r("grfi_cod")) Then
                            Grfi_Cod = r("grfi_cod")
                        End If

                        If Not IsDBNull(r("sa_cod")) AndAlso IsNumeric(r("sa_cod")) Then
                            Sa_Cod = CInt(r("sa_cod"))
                        Else
                            Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
                            Sa_Cod = objCentri.Leggi(Piva, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri).Rows(0)("Sa_Cod")
                        End If

                        'al momento nel campo veg_cod arriva veg_cod|id_cod
                        If r("veg_cod") IsNot Nothing AndAlso r("veg_cod") <> "" Then

                            Veg_Cod = Split(r("veg_cod"), "|")(0)
                            Id_Cod = Split(r("veg_cod"), "|")(1)

                            If Veg_Cod <> 0 Then
                                If Cul_Cod = 0 Then
                                    Dim objCultivar As New AgronicaCoreMetaSchemaDAL.Cultivar_R
                                    Cul_Cod = objCultivar.VarietaAltre(Veg_Cod, objParametri)
                                End If
                            End If

                        End If

                        If Veg_Cod = 0 AndAlso Id_Cod = 0 Then
                            Dim a = 0
                        End If

                        'If IsNumeric(r("Id_Cod")) Then
                        '    Id_Cod = r("Id_Cod")
                        'End If

                        If r("wkt") IsNot Nothing Then
                            wkt = r("wkt")
                        End If

                        If r("wkt_georiferimento_cod") IsNot Nothing Then
                            wkt_georiferimento_cod = r("wkt_georiferimento_cod")
                        End If

                        Veg_Cod_Agea = r("veg_cod_agea")
                        Cul_Cod_Agea = r("cul_cod_agea")
                        If Veg_Cod_Agea <> "" AndAlso Cul_Cod_Agea = "" Then
                            Cul_Cod_Agea = "000"
                        End If

                        Uso_Cod_Agea = ""
                        Occupazione_Cod_Agea = ""
                        Destinazione_Cod_Agea = ""
                        Qualita_Cod_Agea = ""

                        Uso_Cod_Agea = r("Uso_Cod_Agea")
                        Occupazione_Cod_Agea = r("Occupazione_Cod_Agea")
                        Destinazione_Cod_Agea = r("Destinazione_Cod_Agea")
                        Qualita_Cod_Agea = r("Qualita_Cod_Agea")

                        ribaltato = 0
                        movimentato = 0

                        If IsNumeric(r("ribaltato")) Then
                            ribaltato = r("ribaltato")
                        End If
                        If IsNumeric(r("movimentato")) Then
                            movimentato = r("movimentato")
                        End If

                        'se non l'ho ancora fatto mappo specie e varieta
                        If Veg_Cod = 0 AndAlso Id_Cod = 0 Then

                            'Dim objUtilizzi As New AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Agea_R

                            'Dim LogCodificheMancantiSpecie As String = ""
                            'Dim LogCodificheMancantiVarieta As String = ""

                            'objUtilizzi.Specie_e_Varieta_Gias_Da_Agea(
                            '                                        LogCodificheMancantiSpecie,
                            '                                        LogCodificheMancantiVarieta,
                            '                                        Veg_Cod_Agea, Cul_Cod_Agea,
                            '                                        Veg_Cod, Cul_Cod, Grfi_Cod, Grva_Cod, Id_Cod,
                            '                                        "", "",
                            '                                        "", "",
                            '                                        Validita_Inizio_App,
                            '                                        Uso_Cod_Agea,
                            '                                        Occupazione_Cod_Agea,
                            '                                        Destinazione_Cod_Agea,
                            '                                        Qualita_Cod_Agea,
                            '                                        objParametri)

                        End If

                        If IsNumeric(r("Cop_Cod")) Then
                            Cop_Cod = r("Cop_Cod")
                        End If

                        Lotto = r("Lotto")

                        If IsNumeric(r("Resa")) Then
                            Resa = r("Resa")
                        End If

                        If IsNumeric(r("Num_Piante")) Then
                            Num_Piante = r("Num_Piante")
                        End If

                        If IsNumeric(r("TRA_Fila")) Then
                            r("TRA_Fila") = CStr(r("TRA_Fila")).Replace(",", ".")
                            TRA_Fila = r("TRA_Fila")
                        End If

                        If IsNumeric(r("SU_Fila")) Then
                            r("SU_Fila") = CStr(r("SU_Fila")).Replace(",", ".")
                            SU_Fila = r("SU_Fila")
                        End If

                        If Not String.IsNullOrEmpty(Trim(r("Validita_Inizio_Impianto"))) Then
                            Validita_Inizio_Impianto = AgronicaCoreUtility.DataOra.DataStringaJSON_to_ISO8601(r("Validita_Inizio_Impianto"))
                        Else
                            Validita_Inizio_Impianto = AGRODATAINIZIO
                        End If

                        If IsNumeric(r("MetodoProduzione_Cod")) Then
                            Metodo_Produzione_Cod = r("MetodoProduzione_Cod")
                        End If

                        If IsNumeric(r("Unita_Vitata")) Then
                            Unita_Vitata = r("Unita_Vitata")
                        End If

                        If IsNumeric(r("Dpi_Cod")) Then
                            Disciplinare_Cod = CInt(r("Dpi_Cod"))
                        End If

                        If IsNumeric(r("Reg_Cod")) Then
                            Regolamento_Cod = r("Reg_Cod")
                        End If

                        If IsNumeric(r("Regolamento_Concimazione_Cod")) Then
                            Regolamento_Concimazione_Cod = r("Regolamento_Concimazione_Cod")
                        End If

                        If IsNumeric(r("Flag_PubblicoPrivato")) Then
                            Flag_PubblicoPrivato = r("Flag_PubblicoPrivato")
                        End If

                        If IsNumeric(r("id_tr")) Then
                            id_tr = r("id_tr")
                        End If

                        'If IsNumeric(r("Regolamento_Concimazione_Cod")) Then
                        '    Regolamento_Concimazione_Cod = r("Regolamento_Concimazione_Cod")
                        'End If

                        If IsNumeric(r("StatoImpianto_Cod")) Then
                            Stato_Cod = r("StatoImpianto_Cod")
                        End If

                        If Veg_Cod <> 0 AndAlso Stato_Cod = 0 Then
                            Stato_Cod = 102
                        End If

                        Limite_N = r("N")
                        Limite_P = r("P")
                        limite_K = r("K")

                        If Not String.IsNullOrEmpty(Trim(r("Data_Semina"))) Then
                            Data_Semina = AgronicaCoreUtility.DataOra.DataStringaJSON_to_ISO8601(r("Data_Semina"))
                        Else
                            Data_Semina = AGRODATAINIZIO
                        End If

                        If Not String.IsNullOrEmpty(Trim(r("Data_Raccolta"))) Then
                            Data_Raccolta = AgronicaCoreUtility.DataOra.DataStringaJSON_to_ISO8601(r("Data_Raccolta"))
                        Else
                            Data_Raccolta = AGRODATAFINE
                        End If

                        If Not String.IsNullOrEmpty(Trim(r("Data_Fioritura"))) Then
                            Data_Fioritura_Prevista = AgronicaCoreUtility.DataOra.DataStringaJSON_to_ISO8601(r("Data_Fioritura"))
                        Else
                            Data_Fioritura_Prevista = AGRODATAINIZIO
                        End If

                        If IsNumeric(r("Coltura_Precedente")) Then
                            Veg_Cod_Prec = r("Coltura_Precedente")
                        End If

                        If IsNumeric(r("Coltura_Precedente2")) Then
                            Veg_Cod_Prec2 = r("Coltura_Precedente2")
                        End If

                        If IsNumeric(r("Coltura_Precedente3")) Then
                            Veg_Cod_Prec3 = r("Coltura_Precedente3")
                        End If

                        If IsNumeric(r("Coltura_Precedente4")) Then
                            Veg_Cod_Prec4 = r("Coltura_Precedente4")
                        End If

                        Piano_Semina = r("Piano_Semina")
                        Codice_Contratto = r("Codice_Contratto")

                        If r("appezza") = "0" Then
                            Appezza = minAppezza
                            minAppezza -= 1
                        Else
                            Appezza = r("appezza")
                        End If

                        If r("Codice_Fiscale_Tecnico") IsNot Nothing Then
                            Codice_Fiscale_Tecnico = r("Codice_Fiscale_Tecnico")
                        End If

                        If IsNumeric(r("DistBZ_CorpiIdrici")) Then
                            DistBZ_CorpiIdrici = CDbl(r("DistBZ_CorpiIdrici"))
                        End If

                        If IsNumeric(r("DistBZ_AreeResPub")) Then
                            DistBZ_AreeResPub = CDbl(r("DistBZ_AreeResPub"))
                        End If

                        If IsNumeric(r("DistBZ_Allevamenti")) Then
                            DistBZ_Allevamenti = CDbl(r("DistBZ_Allevamenti"))
                        End If

                        If IsNumeric(r("DistBZ_VegNatNonColt")) Then
                            DistBZ_VegNatNonColt = CDbl(r("DistBZ_VegNatNonColt"))
                        End If

                        If IsNumeric(r("SupBZ_Riduzione")) Then
                            SupBZ_Riduzione = CDbl(r("SupBZ_Riduzione"))
                        End If

                        If r("TipoZona") IsNot Nothing AndAlso r("TipoZona") <> "" Then
                            TipoZona = r("TipoZona")
                        Else
                            TipoZona = "n"
                        End If

                        If r("IAF_Cod") IsNot Nothing Then
                            Dim s = r("IAF_Cod").ToString
                            'Dim json As JObject = r("IAF_Cod")
                            Dim jArr As JArray = r("IAF_Cod")
                            Dim resultStrIAF = ""
                            Dim first = True
                            For Each jobj In jArr
                                'Dim jobjj As JObject = jobj
                                If first Then
                                    first = False
                                Else
                                    resultStrIAF &= "|"
                                End If

                                resultStrIAF &= CStr(jobj("value"))


                            Next
                            IAF = resultStrIAF
                        End If

                        If r("campo_cod") IsNot Nothing AndAlso IsNumeric(r("campo_cod")) Then
                            Campo_Cod = CInt(r("campo_cod"))
                        End If

                        If Not String.IsNullOrEmpty(r("Isola")) Then
                            Isola = CStr(r("Isola"))
                        End If

                        If Not String.IsNullOrEmpty(r("Riferimento_Alfanumerico_Appezzamento")) Then
                            Riferimento_Alfanumerico_Appezzamento = CStr(r("Riferimento_Alfanumerico_Appezzamento"))
                        End If

                        If Not String.IsNullOrEmpty(r("CapitolatoPrivato")) Then
                            CapitolatoPrivato = CStr(r("CapitolatoPrivato"))
                        End If

                        If Not String.IsNullOrEmpty(r("Finalita_Concimazione_Impianto")) AndAlso IsNumeric(r("Finalita_Concimazione_Impianto")) Then
                            Finalita_Concimazione_Impianto = CInt(r("Finalita_Concimazione_Impianto"))
                        End If

                        If Not String.IsNullOrEmpty(r("Cod_Indirizzo")) AndAlso IsNumeric(r("Cod_Indirizzo")) Then
                            cod_indirizzo = CInt(r("Cod_Indirizzo"))
                        End If

                        If Not String.IsNullOrEmpty(r("ind_des")) Then
                            ind_des = CStr(r("ind_des"))
                        End If

                        If Not String.IsNullOrEmpty(r("frz_des")) Then
                            frz_des = CStr(r("frz_des"))
                        End If

                        If Not String.IsNullOrEmpty(r("CAP")) Then
                            CAP = CStr(r("CAP"))
                        End If

                        If Not String.IsNullOrEmpty(r("com_des_indirizzo")) Then
                            com_des_indirizzo = CStr(r("com_des_indirizzo"))
                        End If

                        If Not String.IsNullOrEmpty(r("pro_cod_indirizzo")) Then
                            pro_cod_indirizzo = CStr(r("pro_cod_indirizzo"))
                        End If

                        If Not String.IsNullOrEmpty(r("stato_indirizzo")) Then
                            stato_indirizzo = CStr(r("stato_indirizzo"))
                        End If

                        If Not String.IsNullOrEmpty(r("stato_indirizzo_des")) Then
                            stato_indirizzo_des = CStr(r("stato_indirizzo_des"))
                        End If

                        If Not String.IsNullOrEmpty(r("note_indirizzo")) Then
                            note_indirizzo = CStr(r("note_indirizzo"))
                        End If

                        If Not String.IsNullOrEmpty(r("pro_cod_istat_indirizzo")) Then
                            pro_cod_istat_indirizzo = CStr(r("pro_cod_istat_indirizzo"))
                        End If

                        If Not String.IsNullOrEmpty(r("com_cod_istat_indirizzo")) Then
                            com_cod_istat_indirizzo = CStr(r("com_cod_istat_indirizzo"))
                        End If

                        If Not String.IsNullOrEmpty(r("KPIN")) Then
                            KPIN = CStr(r("KPIN"))
                        End If

                        If Not String.IsNullOrEmpty(r("Block_Name")) Then
                            Block_Name = CStr(r("Block_Name"))
                        End If

                        If Not String.IsNullOrEmpty(r("Foral_Cod")) AndAlso IsNumeric(r("Foral_Cod").ToString) Then
                            Foral_Cod = CInt(r("Foral_Cod").ToString)
                        End If

                        If Not String.IsNullOrEmpty(r("Data_Inizio_Portinnesto")) AndAlso IsDate(r("Data_Inizio_Portinnesto").ToString) Then
                            Data_Inizio_Portinnesto = CDate(r("Data_Inizio_Portinnesto").ToString).ToShortDateString
                        End If

                        If Not String.IsNullOrEmpty(r("ZespriFase_Cod")) AndAlso IsNumeric(r("ZespriFase_Cod").ToString) Then
                            ZespriFase = CStr(r("ZespriFase_Cod"))
                        End If

                        If Not String.IsNullOrEmpty(r("ZespriTipo_Cod")) AndAlso IsNumeric(r("ZespriTipo_Cod").ToString) Then
                            ZespriTipo = CStr(r("ZespriTipo_Cod"))
                        End If

                        If Not String.IsNullOrEmpty(r("ZespriGrower_Cod")) AndAlso IsNumeric(r("ZespriGrower_Cod").ToString) Then
                            ZespriGrower = CStr(r("ZespriGrower_Cod"))
                        End If

                        If Not String.IsNullOrEmpty(r("Num_Piante_Femmine")) AndAlso IsNumeric(r("Num_Piante_Femmine").ToString) Then
                            Num_Piante_Femmine = CStr(r("Num_Piante_Femmine"))
                        End If

                        If Not String.IsNullOrEmpty(r("Num_Piante_Maschi")) AndAlso IsNumeric(r("Num_Piante_Maschi").ToString) Then
                            Num_Piante_Maschi = CStr(r("Num_Piante_Maschi"))
                        End If

                        If Not String.IsNullOrEmpty(r("Port_Cod")) AndAlso IsNumeric(r("Port_Cod").ToString) Then
                            Port_cod = CStr(r("Port_Cod"))
                        End If

                        If Not String.IsNullOrEmpty(r("TipologiaInnestoTrapianto_Cod")) AndAlso IsNumeric(r("TipologiaInnestoTrapianto_Cod").ToString) Then
                            TipologiaInnestoTrapianto_cod = CStr(r("TipologiaInnestoTrapianto_Cod"))
                        End If

                        If Not String.IsNullOrEmpty(r("Mat_Cod")) AndAlso IsNumeric(r("Mat_Cod").ToString) Then
                            Mat_Cod = CStr(r("Mat_Cod"))
                        End If

                        'aggiungo un nuovo app se non movimentato
                        If Not (Not String.IsNullOrEmpty(Trim(r("movimentato"))) AndAlso Trim(r("movimentato")) = "1") Then

                            objProgrammazione.DT_Appezzamenti_Insert(Dt_Appezzamenti,
                                                False,
                                                Operazione,
                                                objProgrammazione.OperazioneDes_From_OperazioneCod(Operazione),
                                                Piva,
                                                Sa_Cod,
                                                Campo_Cod,
                                                Appezza,
                                                0,
                                                0,
                                                Lotto,
                                                r("app_nome"),
                                                r("utilizzo_sup"),
                                                Veg_Cod,
                                                "",
                                                Cul_Cod,
                                                "",
                                                Grva_Cod,
                                                "",
                                                Grfi_Cod,
                                                "",
                                                Cop_Cod,
                                                "",
                                                Resa,
                                                Id_Cod,
                                                "",
                                                Num_Piante,
                                                TRA_Fila,
                                                SU_Fila,
                                                Validita_Inizio_Impianto,
                                                Validita_Inizio_App,
                                                Validita_Fine_App,
                                                r("programmazione_entita_cod"),
                                                Data_Semina,
                                                Data_Raccolta,
                                                TipoZona,
                                                Metodo_Produzione_Cod,
                                                "",
                                                "",
                                                r("sa_nome"),
                                                "",
                                                Veg_Cod_Agea,
                                                Cul_Cod_Agea,
                                                "",
                                                "",
                                                r("macrouso_cod"), "",
                                                Unita_Vitata,
                                                objParametri,
                                                Ribaltato:=ribaltato,
                                                Movimentato:=movimentato,
                                                Veg_Cod_Agea:=Veg_Cod_Agea,
                                                Cul_Cod_Agea:=Cul_Cod_Agea,
                                                Uso_Cod_Agea:=Uso_Cod_Agea,
                                                Occupazione_Cod_Agea:=Occupazione_Cod_Agea,
                                                Destinazione_Cod_Agea:=Destinazione_Cod_Agea,
                                                Qualita_Cod_Agea:=Qualita_Cod_Agea,
                                                Limite_N:=Limite_N,
                                                Limite_P:=Limite_P,
                                                Limite_K:=limite_K,
                                                Data_Fioritura_Prevista:=Data_Fioritura_Prevista,
                                                Veg_Cod_Prec:=Veg_Cod_Prec,
                                                Veg_Cod_Prec2:=Veg_Cod_Prec2,
                                                Veg_Cod_Prec3:=Veg_Cod_Prec3,
                                                Veg_Cod_Prec4:=Veg_Cod_Prec4,
                                                Piano_Semina:=Piano_Semina,
                                                Codice_Contratto:=Codice_Contratto,
                                                Disciplinare_Cod:=Disciplinare_Cod,
                                                Regolamento_Cod:=Regolamento_Cod,
                                                Stato_Cod:=Stato_Cod,
                                                Codice_Fiscale_Tecnico:=Codice_Fiscale_Tecnico,
                                                Stato_Ribaltamento:=Stato_Ribaltamento,
                                                provenienza_fascicolo:=r("provenienza_fascicolo"),
                                                IAF:=IAF,
                                                Pratica_Cod:=Pratica_Cod,
                                                Regolamento_Concimazione_Cod:=Regolamento_Concimazione_Cod,
                                                Flag_PubblicoPrivato:=Flag_PubblicoPrivato,
                                                id_tr:=id_tr,
                                                DistBZ_CorpiIdrici:=DistBZ_CorpiIdrici,
                                                DistBZ_AreeResPub:=DistBZ_AreeResPub,
                                                DistBZ_Allevamenti:=DistBZ_Allevamenti,
                                                DistBZ_VegNatNonColt:=DistBZ_VegNatNonColt,
                                                wkt:=wkt,
                                                wkt_georiferimento_cod:=wkt_georiferimento_cod,
                                                SupBZ_Riduzione:=SupBZ_Riduzione,
                                                Riferimento_Alfanumerico_Appezzamento:=Riferimento_Alfanumerico_Appezzamento,
                                                Isola:=Isola,
                                                CapitolatoPrivato:=CapitolatoPrivato,
                                                Finalita_Concimazione_Impianto:=Finalita_Concimazione_Impianto,
                                                Cod_Indirizzo:=cod_indirizzo,
                                                ind_des:=ind_des,
                                                frz_des:=frz_des,
                                                CAP:=CAP,
                                                com_des:=com_des_indirizzo,
                                                pro_cod:=pro_cod_indirizzo,
                                                stato:=stato_indirizzo,
                                                stato_des:=stato_indirizzo_des,
                                                note:=note_indirizzo,
                                                pro_cod_istat:=pro_cod_istat_indirizzo,
                                                com_cod_istat:=com_cod_istat_indirizzo,
                                                KPIN:=KPIN,
                                                Block_Name:=Block_Name,
                                                Foral_Cod:=Foral_Cod,
                                                Data_Inizio_Portinnesto:=Data_Inizio_Portinnesto,
                                                ZespriFase_Cod:=ZespriFase,
                                                ZespriTipo_Cod:=ZespriTipo,
                                                ZespriGrower_Cod:=ZespriGrower,
                                                Num_Piante_Femmine:=Num_Piante_Femmine,
                                                Num_Piante_Maschi:=Num_Piante_Maschi,
                                                Port_Cod:=Port_cod,
                                                TipologiaInnestoTrapianto_Cod:=TipologiaInnestoTrapianto_cod, Mat_Cod:=Mat_Cod
                                            )

                            strCatasto = r("catasto")

                            If strCatasto <> "" Then
                                Dim ParticelleArray = JArray.Parse(strCatasto)
                                Dim sezione As String = ""
                                Dim subalterno As String = ""
                                Dim HashParticelle As New Hashtable
                                If ParticelleArray IsNot Nothing Then
                                    For Each row In ParticelleArray

                                        sezione = ""
                                        subalterno = ""
                                        If row("Sezione") = "" Then
                                            sezione = "0"
                                        Else
                                            sezione = row("Sezione")
                                        End If

                                        If row("Subalterno") = "" Then
                                            subalterno = "0"
                                        Else
                                            subalterno = row("Subalterno")
                                        End If

                                        Dim key = CStr(row("Istat_Prov")) & "_" & CStr(row("Istat_Com")) & "_" & sezione & "_" & CStr(row("Foglio")) & "_" & CStr(row("Numero")) & "_" & subalterno

                                        If Not HashParticelle.Contains(key) Then

                                            HashParticelle.Add(key, "")

                                            Dim utilizzo_sup As Double

                                            If row("Utilizzo_sup") IsNot Nothing AndAlso IsNumeric(row("Utilizzo_sup")) Then
                                                utilizzo_sup = row("Utilizzo_sup")
                                            ElseIf row("utilizzo_sup") IsNot Nothing AndAlso IsNumeric(row("utilizzo_sup")) Then
                                                utilizzo_sup = row("utilizzo_sup")
                                            Else
                                                utilizzo_sup = 0
                                            End If

                                            objProgrammazione.DT_Intersezioni_Insert(
                                            Dt_Particelle,
                                            enum_TipoPianificazione.Pianificazione_Annuale,
                                            Piva,
                                            r("sa_cod"),
                                            0,
                                            Appezza,
                                            row("Istat_Prov"),
                                            row("Istat_Com"),
                                            row("Prov"),
                                            row("Com"),
                                            sezione,
                                            row("Foglio"),
                                            row("Numero"),
                                            subalterno,
                                            utilizzo_sup,
                                            utilizzo_sup)

                                        Else

                                            Dim query = " PROV='" & CStr(row("Istat_Prov")) & "' AND COM = '" & CStr(row("Istat_Com")) & "' AND SEZIONE='" & sezione & "' AND FOGLIO = " & CStr(row("Foglio")) &
                                                            " AND NUMERO = " & CStr(row("Numero")) & "  AND SUBALTERNO = '" & subalterno & "' AND Appezza = " & CStr(Appezza) & " "

                                            Dim PartRow = Dt_Particelle.Select(query)
                                            If PartRow.Count > 0 Then
                                                PartRow(0)("SupIntersezione") = Decimal.Round(CDec(PartRow(0)("SupIntersezione")) + CDec(row("Utilizzo_sup")), 4)
                                            End If

                                        End If

                                    Next

                                End If
                            End If

                        End If
                        'End If
                    Next

                    Dim hashZvn As New List(Of String)
                    For Each AppezzaRow In Dt_Appezzamenti.Rows

                        If AppezzaRow("TipoZona") = "v" Then
                            Dim dtZvnAppezza = Dt_Particelle.Select(" Appezza=" & AppezzaRow("Appezza") & " ")
                            For Each rowPart In dtZvnAppezza
                                Dim partStr As String = rowPart("PROV") & "_" &
                                                        rowPart("COM") & "_" &
                                                        rowPart("Sezione") & "_" &
                                                        rowPart("Foglio") & "_" &
                                                        rowPart("Numero") & "_" &
                                                        rowPart("Subalterno")

                                If Not hashZvn.Contains(partStr) Then
                                    hashZvn.Add(partStr)
                                End If
                            Next

                        End If

                    Next

                    For Each particellaZvn In hashZvn

                        Dim ListAppezzaZvn = Dt_Particelle.Select(" UNID_PART='" & particellaZvn & "'").CopyToDataTable.DefaultView.ToTable(True, "appezza")

                        For Each AppezzaR In ListAppezzaZvn.Rows
                            Dim rs = Dt_Appezzamenti.Select(" Appezza=" & AppezzaR("Appezza"))
                            For Each rsApp In rs
                                If rsApp("TipoZona") = "n" Then
                                    rsApp("TipoZona") = "v"
                                End If
                            Next
                        Next

                    Next

                    Dim objParticelle_R As New AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R
                    Dim objZonexParticelle_R As New AgronicaCoreAnagrafeDAL.ZonexParticelle_R
                    Dim objZonexParticelle_w As New AgronicaCoreAnagrafeDAL.ZonexParticelle_W

                    For Each particelaZvn In hashZvn

                        Dim partArr = particelaZvn.Split("_")
                        Dim Prov = partArr(0)
                        Dim Com = partArr(1)
                        Dim Sezione = partArr(2)
                        Dim Foglio = partArr(3)
                        Dim Numero = partArr(4)
                        Dim Subalterno = partArr(5)

                        If objZonexParticelle_R.Leggi(enum_Zone.ZVN, Prov, Com, Sezione, Foglio, Numero, Subalterno, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri).Rows.Count = 0 Then
                            Dim dtPart = objParticelle_R.Leggi(0, Piva, 0, 0, Prov, Com, Sezione, Foglio, Numero, Subalterno, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri)
                            If dtPart.Rows.Count > 0 Then
                                Dim area = dtPart(0)("Sup_Condotta")
                                objZonexParticelle_w.Scrivi(enum_Zone.ZVN, Prov, Com, Sezione, Foglio, Numero, Subalterno, area, Validita_Inizio, AGRODATAFINE, objParametri)
                            End If
                        End If

                    Next


                    Programmazione_Cod_OUT = Pianificazione_Scrivi(enum_TipoOperazioneDB.Scrittura,
                                                                       Programmazione_Cod_OUT,
                                                                       Programmazione_Des, Programmazione_Des_Long,
                                                                       Piva, Note, Utente,
                                                                       Validita_Inizio, Validita_Fine,
                                                                       Tipo_Pianificazione, Tipo_Fonte,
                                                                       Dt_Appezzamenti,
                                                                       Dt_Particelle,
                                                                       Dt_Appezzamenti_Eliminati,
                                                                       NumeroDiValidazione, DataDiValidazione,
                                                                       objParametri, 0, SalvaAllegato, False,
                                                                       strEntitaDaMantenere,
                                                                       Pratica_Cod:=Pratica_Cod,
                                                                       importatoAutomaticamente:=importatoAutomaticamente)

                    rval.RispostaOK = True

                End If

            Else

                rval.RispostaOK = True

            End If



            'If Programmazione_Cod_OUT = 0 Then

            'If strAppezzamenti <> "" Then

            '    righeInseriteArray = JArray.Parse(strAppezzamenti)
            '    AgronicaCoreUtility.DataOra.JarrayAggiustaDate(righeInseriteArray)

            '    Dim minAppezza As Integer = (
            '        From ss In righeInseriteArray
            '        Select ss("appezza")
            '    ).Min()

            '    minAppezza -= 1

            '    If Not righeInseriteArray Is Nothing Then

            '        Dim Dt_Appezzamenti As New DataTable
            '        Dim Dt_Particelle As New DataTable
            '        Dim Dt_Appezzamenti_Eliminati As New DataTable

            '        Dim objProgrammazione As New Programmazione_R

            '        objProgrammazione.DT_Appezzamenti_Crea(Dt_Appezzamenti)
            '        objProgrammazione.DT_Intersezioni_Crea(Dt_Particelle)
            '        objProgrammazione.DT_Appezzamenti_Eliminati_Crea(Dt_Appezzamenti_Eliminati)

            '        For Each r In righeInseriteArray

            '            If Not String.IsNullOrEmpty(Trim(r("validita_inizio"))) Then
            '                Validita_Inizio_App = AgronicaCoreUtility.DataOra.DataStringaJSON_to_ISO8601(r("validita_inizio"))
            '            End If

            '            If Not String.IsNullOrEmpty(Trim(r("validita_fine"))) Then
            '                Validita_Fine_App = AgronicaCoreUtility.DataOra.DataStringaJSON_to_ISO8601(r("validita_fine"))
            '            End If

            '            Veg_Cod = 0
            '            Cul_Cod = 0
            '            Grfi_Cod = 0
            '            Id_Cod = 0
            '            Grva_Cod = 0

            '            If IsNumeric(r("cul_cod")) Then
            '                Cul_Cod = r("cul_cod")
            '            End If
            '            If IsNumeric(r("grva_cod")) Then
            '                Grva_Cod = r("grva_cod")
            '            End If
            '            If IsNumeric(r("grfi_cod")) Then
            '                Grfi_Cod = r("grfi_cod")
            '            End If

            '            'al momento nel campo veg_cod arriva veg_cod|id_cod
            '            If Not r("veg_cod") Is Nothing AndAlso r("veg_cod") <> "" Then

            '                Veg_Cod = Split(r("veg_cod"), "|")(0)
            '                Id_Cod = Split(r("veg_cod"), "|")(1)

            '                If Veg_Cod <> 0 Then
            '                    If Cul_Cod = 0 Then
            '                        Dim objCultivar As New AgronicaCoreMetaSchemaDAL.Cultivar_R
            '                        Cul_Cod = objCultivar.VarietaAltre(Veg_Cod,
            '                                                           objParametri)
            '                    End If
            '                End If

            '            End If


            '            'If IsNumeric(r("Id_Cod")) Then
            '            '    Id_Cod = r("Id_Cod")
            '            'End If

            '            Veg_Cod_Agea = r("veg_cod_agea")
            '            Cul_Cod_Agea = r("cul_cod_agea")
            '            If Veg_Cod_Agea <> "" And Cul_Cod_Agea = "" Then
            '                Cul_Cod_Agea = "000"
            '            End If


            '            ribaltato = 0
            '            movimentato = 0

            '            If IsNumeric(r("ribaltato")) Then
            '                ribaltato = r("ribaltato")
            '            End If
            '            If IsNumeric(r("movimentato")) Then
            '                movimentato = r("movimentato")
            '            End If

            '            'se non l'ho ancora fatto mappo specie e varieta
            '            If Veg_Cod = 0 And Id_Cod = 0 Then

            '                Dim objUtilizzi As New AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Agea_R

            '                Dim LogCodificheMancantiSpecie As String = ""
            '                Dim LogCodificheMancantiVarieta As String = ""

            '                objUtilizzi.Specie_e_Varieta_Gias_Da_Agea(
            '                                                            LogCodificheMancantiSpecie,
            '                                                            LogCodificheMancantiVarieta,
            '                                                            Veg_Cod_Agea, Cul_Cod_Agea,
            '                                                            Veg_Cod, Cul_Cod, Grfi_Cod, Grva_Cod, Id_Cod,
            '                                                            "", "",
            '                                                            "", "",
            '                                                            Validita_Inizio_App,
            '                                                            objParametri)

            '            End If

            '            If r("appezza") = "0" Then
            '                Appezza = minAppezza
            '                minAppezza -= 1
            '            Else
            '                Appezza = r("appezza")
            '            End If

            '            objProgrammazione.DT_Appezzamenti_Insert(Dt_Appezzamenti,
            '                                    False,
            '                                    Operazione,
            '                                    objProgrammazione.OperazioneDes_From_OperazioneCod(Operazione),
            '                                    Piva,
            '                                    r("sa_cod"),
            '                                    0,
            '                                    Appezza,
            '                                    0,
            '                                    0,
            '                                    "",
            '                                    r("app_nome"),
            '                                    r("utilizzo_sup"),
            '                                    Veg_Cod,
            '                                    "",
            '                                    Cul_Cod,
            '                                    "",
            '                                    Grva_Cod,
            '                                    "",
            '                                    Grfi_Cod,
            '                                    "",
            '                                    0,
            '                                    "",
            '                                    0,
            '                                    Id_Cod,
            '                                    "",
            '                                    0,
            '                                    0,
            '                                    0,
            '                                    Validita_Inizio_App,
            '                                    Validita_Inizio_App,
            '                                    Validita_Fine_App,
            '                                    r("programmazione_entita_cod"),
            '                                    AGRODATAINIZIO,
            '                                    AGRODATAFINE,
            '                                    "",
            '                                    0,
            '                                    "",
            '                                    "",
            '                                    r("sa_nome"),
            '                                    "",
            '                                    Veg_Cod_Agea,
            '                                    Cul_Cod_Agea,
            '                                    "",
            '                                    "",
            '                                    r("macrouso_cod"), "",
            '                                    0,
            '                                    objParametri,
            '                                            Ribaltato:=ribaltato,
            '                                            Movimentato:=movimentato)

            '            strCatasto = r("catasto_key")

            '            If strCatasto <> "" Then
            '                Dim ParticelleArray As String() = Split(strCatasto, "<BR>")
            '                Dim ParticellaArray As String()
            '                If Not ParticelleArray Is Nothing Then
            '                    For i = 0 To ParticelleArray.Length - 1
            '                        ParticellaArray = Split(ParticelleArray(i), "_")
            '                        objProgrammazione.DT_Intersezioni_Insert(
            '                                        Dt_Particelle,
            '                                        enum_TipoPianificazione.Pianificazione_Annuale,
            '                                        Piva,
            '                                        r("sa_cod"),
            '                                        0,
            '                                        r("appezza"),
            '                                        ParticellaArray(0),
            '                                        ParticellaArray(1),
            '                                        "",
            '                                        "",
            '                                        IIf(ParticellaArray(2) = "", "0", ParticellaArray(2)),
            '                                        ParticellaArray(3),
            '                                        ParticellaArray(4),
            '                                        IIf(ParticellaArray(5) = "", "0", ParticellaArray(5)),
            '                                        ParticellaArray(6),
            '                                        ParticellaArray(6))
            '                    Next

            '                End If
            '            End If

            '        Next

            '        Programmazione_Cod_OUT = Pianificazione_Scrivi(enum_TipoOperazioneDB.Scrittura,
            '                                                       Programmazione_Cod_OUT,
            '                                                       Programmazione_Des, Programmazione_Des_Long,
            '                                                       Piva, Note, Utente,
            '                                                       Validita_Inizio, Validita_Fine,
            '                                                       Tipo_Pianificazione, Tipo_Fonte,
            '                                                       Dt_Appezzamenti,
            '                                                       Dt_Particelle,
            '                                                       Dt_Appezzamenti_Eliminati,
            '                                                       NumeroDiValidazione, DataDiValidazione,
            '                                                       objParametri, 0, SalvaAllegato)

            '        rval.RispostaOK = True

            '    End If

            'End If

            'End If

        Catch ex As Exception

            rval.RispostaOK = False

            If objParametri.objTransazione IsNot Nothing Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            'uso questa funzione per ottenere il Messaggio..:
            rval.Errore = "Errore durante l'operazione: " & vbCrLf &
                Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        Finally
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)
        End Try

        Return rval

    End Function

    Public Function Pianificazione_Scrivi(ByVal Tipo_Operazione As Integer,
                                          ByVal Programmazione_Cod As Integer,
                                          ByVal Programmazione_Des As String,
                                          ByVal Programmazione_Des_Long As String,
                                          ByVal Piva As String,
                                          ByVal Note As String,
                                          ByVal Utente As String,
                                          ByVal Validita_Inizio As Date,
                                          ByVal Validita_Fine As Date,
                                          ByVal Tipo_Pianificazione As Integer,
                                          ByVal Tipo_Fonte As Integer,
                                          ByVal Dt_Appezzamenti As DataTable,
                                          ByVal Dt_Particelle As DataTable,
                                          ByVal Dt_Appezzamenti_Eliminati As DataTable,
                                          ByVal NumeroDiValidazione As String,
                                          ByVal DataDiValidazione As Date,
                                          ByRef objParametri As AgronicaCoreParametri,
                                          Optional ByVal Stato_SQNPI As Integer = 0,
                                          Optional ByVal SalvaAllegato As Boolean = True,
                                          Optional ByVal EliminaAppezzamentiRibaltati As Boolean = False,
                                          Optional ByVal strEntitaDaMantenere As String = "",
                                          Optional ByRef objParametri_Utenti As AgronicaCoreParametri = Nothing,
                                          Optional ByRef Pratica_Cod As String = "",
                                          Optional ByVal importatoAutomaticamente As Boolean = False) As Integer

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Programmazione_W.Pianificazione_Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim ErrMSG As String = ""

        Dim Codice As Integer = 0
        Dim RecordInteressati As Integer = 0
        Dim Programmazione_Entita_Cod As Integer = 0
        Dim i, j As Integer
        Dim Dr As DataRow()
        Dim DrAppEliminati As DataRow()
        Dim Dt As DataTable
        Dim BoolDummy As Boolean

        Dim ScriviElementiGrafici As New AgronicaCoreGisBIZ.GIS_Entita_W

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try

            '------------------------------
            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                           FlagTransazioneLocale,
                                                           objParametri)
            Dim objFascicoloEntita As New AgronicaCoreAnagrafeDAL.Allegati_EntitaxDocumenti_W

            Dim objParticelle_R As New AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R
            Dim objZonexParticelle_R As New AgronicaCoreAnagrafeDAL.ZonexParticelle_R
            Dim objZonexParticelle_w As New AgronicaCoreAnagrafeDAL.ZonexParticelle_W

            Select Case Tipo_Operazione

                '----------------------------------------------------------
                '----------------------------------------------------------
                '------    SCRITTURA    -----------------------------------
                '----------------------------------------------------------
                '----------------------------------------------------------

                Case enum_TipoOperazioneDB.Scrittura

                    If Programmazione_Cod <> 0 Then
                        Codice = Programmazione_Cod
                    End If

                    Dim allegati_Documenti_cod As Integer = 0

                    If strEntitaDaMantenere = "" Then

                        Dim objProgrammazione_Testata_W As New AgronicaCoreAnagrafeDAL.Programmazione_Testata_W

                        Dim importAutomatico = 0
                        If importatoAutomaticamente Then
                            importAutomatico = 1
                        End If

                        BoolDummy = objProgrammazione_Testata_W.Scrivi(Codice,
                                                                        Programmazione_Des,
                                                                        Programmazione_Des_Long,
                                                                        Piva,
                                                                        Note,
                                                                        Tipo_Pianificazione,
                                                                         Tipo_Fonte,
                                                                         Validita_Inizio,
                                                                         Validita_Fine,
                                                                        objParametri,
                                                                        Stato_SQNPI:=Stato_SQNPI,
                                                                        Pratica_Cod:=Pratica_Cod,
                                                                        importatoAutomaticamente:=importAutomatico)

                        If NumeroDiValidazione <> "" Then
                            '(05-09-2017 fede) rendo opzionale il salvataggio dell'allegato
                            If SalvaAllegato Then
                                ScriviAllegatoEdEntitaAllegatoPlann(Programmazione_Des, Piva, Dt_Appezzamenti, NumeroDiValidazione, DataDiValidazione, objParametri, Codice, i, objFascicoloEntita, allegati_Documenti_cod)
                            End If
                        End If

                    Else

                        Dim objProgrammazione_Testata_W As New AgronicaCoreAnagrafeDAL.Programmazione_Testata_W

                        objProgrammazione_Testata_W.Modifica_Pratica(Programmazione_Cod, Pratica_Cod, "", objParametri)

                    End If

                    Dim dt_appezzamenti_Contiene_GIS As Boolean =
                        Dt_Appezzamenti.Columns.Contains("wkt")

                    If Not IsNothing(Dt_Appezzamenti) Then

                        Dim objProgrammazione_Entita_W As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_W
                        Dim objProgrammazione_Entita_Codici_W As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_Codici_W
                        Dim objIndirizzi_W As New AgronicaCoreAnagrafeDAL.Indirizzi_Write
                        Dim objIstat As New AgronicaCoreMetaSchemaDAL.Istat_R

                        Dim ObjSequenze = New Agro_Sequenze


                        For i = 0 To Dt_Appezzamenti.Rows.Count - 1

                            Dim codFiscaleTecnico As String = ""
                            If Dt_Appezzamenti.Columns.Contains("Codice_Fiscale_Tecnico") Then
                                codFiscaleTecnico = Dt_Appezzamenti.Rows(i).Item("Codice_Fiscale_Tecnico")
                            End If

                            Dim Operazione_Cod As Integer = 0
                            If Dt_Appezzamenti.Columns.Contains("Operazione_Cod") Then
                                Operazione_Cod = Dt_Appezzamenti.Rows(i).Item("Operazione_Cod")
                            End If

                            Dim veg_cod_cliente, cul_cod_cliente As String
                            If IsDBNull(Dt_Appezzamenti.Rows(i).Item("veg_cod_cliente")) Then
                                veg_cod_cliente = ""
                            Else
                                veg_cod_cliente = Dt_Appezzamenti.Rows(i).Item("veg_cod_cliente")
                            End If

                            If IsDBNull(Dt_Appezzamenti.Rows(i).Item("cul_cod_cliente")) Then
                                cul_cod_cliente = ""
                            Else
                                cul_cod_cliente = Dt_Appezzamenti.Rows(i).Item("cul_cod_cliente")
                            End If


                            'Dim via_stringa As String = ""
                            'Try
                            '    via_stringa = Dt_Appezzamenti.Rows(i).Item("via_stringa")
                            'Catch ex As Exception
                            'End Try


                            Dim via_stringa As String = ""
                            Try
                                via_stringa = Dt_Appezzamenti.Rows(i).Item("via_stringa").ToString.Replace("'", "`")
                            Catch ex As Exception
                            End Try


                            Dim Unita_Vitata As Integer = 0
                            If Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("unita_vitata")) Then
                                Unita_Vitata = Dt_Appezzamenti.Rows(i).Item("unita_vitata")
                            Else
                                Unita_Vitata = 0
                            End If

                            Dim Validita_Inizio_Impianto As Date = #1/1/1900#
                            If Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("Validita_Inizio_Impianto")) Then
                                Validita_Inizio_Impianto = Dt_Appezzamenti.Rows(i).Item("Validita_Inizio_Impianto")
                            Else
                                Validita_Inizio_Impianto = Dt_Appezzamenti.Rows(i).Item("Validita_Inizio")
                            End If

                            Dim Regolamento_Cod As Integer = 0
                            If Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("Regolamento_Cod")) Then
                                Regolamento_Cod = Dt_Appezzamenti.Rows(i).Item("Regolamento_Cod")
                            End If

                            Dim Regolamento_Concimazione_Cod As Integer = 0
                            If Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("Regolamento_Concimazione_Cod")) Then
                                Regolamento_Concimazione_Cod = Dt_Appezzamenti.Rows(i).Item("Regolamento_Concimazione_Cod")
                            End If

                            Dim Flag_PubblicoPrivato As Integer = 0
                            If Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("Flag_PubblicoPrivato")) Then
                                Flag_PubblicoPrivato = Dt_Appezzamenti.Rows(i).Item("Flag_PubblicoPrivato")
                            End If

                            Dim id_tr As Integer = 0
                            If Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("id_tr")) Then
                                id_tr = Dt_Appezzamenti.Rows(i).Item("id_tr")
                            End If

                            Dim Disciplinare_Cod As Integer = 0
                            If Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("Disciplinare_Cod")) Then
                                Disciplinare_Cod = Dt_Appezzamenti.Rows(i).Item("Disciplinare_Cod")
                            End If

                            Dim Stato_Cod As Integer = 0
                            If Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("Stato_Cod")) Then
                                Stato_Cod = Dt_Appezzamenti.Rows(i).Item("Stato_Cod")
                            End If

                            Dim Veg_Cod_Prec As Integer = 0
                            If Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("Veg_Cod_Prec")) Then
                                Veg_Cod_Prec = Dt_Appezzamenti.Rows(i).Item("Veg_Cod_Prec")
                            End If

                            Dim Veg_Cod_Prec2 As Integer = 0
                            If Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("Veg_Cod_Prec2")) Then
                                Veg_Cod_Prec2 = Dt_Appezzamenti.Rows(i).Item("Veg_Cod_Prec2")
                            End If

                            Dim Veg_Cod_Prec3 As Integer = 0
                            If Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("Veg_Cod_Prec3")) Then
                                Veg_Cod_Prec3 = Dt_Appezzamenti.Rows(i).Item("Veg_Cod_Prec3")
                            End If

                            Dim Veg_Cod_Prec4 As Integer = 0
                            If Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("Veg_Cod_Prec4")) Then
                                Veg_Cod_Prec4 = Dt_Appezzamenti.Rows(i).Item("Veg_Cod_Prec4")
                            End If

                            'Dim Codice_Fiscale_Tecnico As String = ""
                            'If Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("Codice_Fiscale_Tecnico")) Then
                            '    Codice_Fiscale_Tecnico = Dt_Appezzamenti.Rows(i).Item("Codice_Fiscale_Tecnico")
                            'End If

                            Dim Stato_Ribaltamento As Integer = 0
                            If Dt_Appezzamenti.Columns.Contains("Stato_Ribaltamento") AndAlso Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("Stato_Ribaltamento")) Then
                                Stato_Ribaltamento = Dt_Appezzamenti.Rows(i).Item("Stato_Ribaltamento")
                            End If

                            Dim Limite_N As String = ""
                            If Dt_Appezzamenti.Columns.Contains("Limite_N") AndAlso Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("Limite_N")) Then
                                Limite_N = Dt_Appezzamenti.Rows(i).Item("Limite_N")
                            End If

                            Dim Limite_P As String = ""
                            If Dt_Appezzamenti.Columns.Contains("Limite_P") AndAlso Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("Limite_P")) Then
                                Limite_P = Dt_Appezzamenti.Rows(i).Item("Limite_P")
                            End If

                            Dim Limite_K As String = ""
                            If Dt_Appezzamenti.Columns.Contains("Limite_K") AndAlso Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("Limite_K")) Then
                                Limite_K = Dt_Appezzamenti.Rows(i).Item("Limite_K")
                            End If

                            Dim Progetto_Nome As String = ""
                            If Dt_Appezzamenti.Columns.Contains("Progetto_Nome") AndAlso Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("Progetto_Nome")) Then
                                Progetto_Nome = Dt_Appezzamenti.Rows(i).Item("Progetto_Nome")
                            End If

                            Dim Piano_Semina As String = ""
                            If Dt_Appezzamenti.Columns.Contains("Piano_Semina") AndAlso Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("Piano_Semina")) Then
                                Piano_Semina = Dt_Appezzamenti.Rows(i).Item("Piano_Semina")
                            End If

                            Dim Codice_Contratto As String = ""
                            If Dt_Appezzamenti.Columns.Contains("Codice_Contratto") AndAlso Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("Codice_Contratto")) Then
                                Codice_Contratto = Dt_Appezzamenti.Rows(i).Item("Codice_Contratto")
                            End If

                            Dim IAF As String = ""
                            If Dt_Appezzamenti.Columns.Contains("IAF") AndAlso Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("IAF")) Then
                                IAF = Dt_Appezzamenti.Rows(i).Item("IAF")
                            End If

                            Pratica_Cod = 0
                            If Dt_Appezzamenti.Columns.Contains("Pratica_Cod") AndAlso Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("Pratica_Cod")) Then
                                Pratica_Cod = Dt_Appezzamenti.Rows(i).Item("Pratica_Cod")
                            End If

                            Dim DistBZ_CorpiIdrici As Double = 0
                            If Dt_Appezzamenti.Columns.Contains("DistBZ_CorpiIdrici") AndAlso Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("DistBZ_CorpiIdrici")) Then
                                DistBZ_CorpiIdrici = Dt_Appezzamenti.Rows(i).Item("DistBZ_CorpiIdrici")
                            End If

                            Dim DistBZ_AreeResPub As Double = 0
                            If Dt_Appezzamenti.Columns.Contains("DistBZ_AreeResPub") AndAlso Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("DistBZ_AreeResPub")) Then
                                DistBZ_AreeResPub = Dt_Appezzamenti.Rows(i).Item("DistBZ_AreeResPub")
                            End If

                            Dim DistBZ_Allevamenti As Double = 0
                            If Dt_Appezzamenti.Columns.Contains("DistBZ_Allevamenti") AndAlso Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("DistBZ_Allevamenti")) Then
                                DistBZ_Allevamenti = Dt_Appezzamenti.Rows(i).Item("DistBZ_Allevamenti")
                            End If

                            Dim DistBZ_VegNatNonColt As Double = 0
                            If Dt_Appezzamenti.Columns.Contains("DistBZ_VegNatNonColt") AndAlso Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("DistBZ_VegNatNonColt")) Then
                                DistBZ_VegNatNonColt = Dt_Appezzamenti.Rows(i).Item("DistBZ_VegNatNonColt")
                            End If

                            Dim SupBZ_Riduzione As Double = 0
                            If Dt_Appezzamenti.Columns.Contains("SupBZ_Riduzione") AndAlso Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("SupBZ_Riduzione")) Then
                                SupBZ_Riduzione = Dt_Appezzamenti.Rows(i).Item("SupBZ_Riduzione")
                            End If

                            Dim Tipo_Operazione_Entita As enum_TipoOperazioneDB
                            Dim lProgrammazione_Entita_cod As Integer =
                                Dt_Appezzamenti.Rows(i).Item("Programmazione_Entita_Cod")


                            Dim Riferimento_Alfanumerico_Appezzamento As String = Dt_Appezzamenti.Rows(i).Item("Riferimento_Alfanumerico_Appezzamento")
                            Dim Isola As String = Dt_Appezzamenti.Rows(i).Item("Isola")

                            Dim CapitolatoPrivato As String = ""
                            If Dt_Appezzamenti.Columns.Contains("CapitolatoPrivato") AndAlso Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("CapitolatoPrivato")) Then
                                CapitolatoPrivato = Dt_Appezzamenti.Rows(i).Item("CapitolatoPrivato")
                            End If

                            Dim Finalita_Concimazione_Impianto As Integer = 0
                            If Dt_Appezzamenti.Columns.Contains("Finalita_Concimazione_Impianto") AndAlso Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("Finalita_Concimazione_Impianto")) Then
                                Finalita_Concimazione_Impianto = Dt_Appezzamenti.Rows(i).Item("Finalita_Concimazione_Impianto")
                            End If

                            'INDIRIZZI
                            Dim inserisci_indirizzo As Boolean = False
                            Dim cancella_indirizzo As Boolean = False

                            Dim cod_indirizzo As Integer = 0
                            If Dt_Appezzamenti.Columns.Contains("cod_indirizzo") AndAlso Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("cod_indirizzo")) Then
                                cod_indirizzo = Dt_Appezzamenti.Rows(i).Item("cod_indirizzo")
                            End If

                            Dim ind_des As String = ""
                            If Dt_Appezzamenti.Columns.Contains("ind_des") AndAlso Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("ind_des")) Then
                                ind_des = Dt_Appezzamenti.Rows(i).Item("ind_des")
                            End If

                            Dim frz_des As String = ""
                            If Dt_Appezzamenti.Columns.Contains("frz_des") AndAlso Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("frz_des")) Then
                                frz_des = Dt_Appezzamenti.Rows(i).Item("frz_des")
                            End If

                            Dim CAP As String = ""
                            If Dt_Appezzamenti.Columns.Contains("CAP") AndAlso Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("CAP")) Then
                                CAP = Dt_Appezzamenti.Rows(i).Item("CAP")
                            End If

                            Dim com_des_indirizzo As String = ""
                            If Dt_Appezzamenti.Columns.Contains("com_des_indirizzo") AndAlso Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("com_des_indirizzo")) Then
                                com_des_indirizzo = Dt_Appezzamenti.Rows(i).Item("com_des_indirizzo")
                            End If

                            Dim pro_cod_indirizzo As String = ""
                            If Dt_Appezzamenti.Columns.Contains("pro_cod_indirizzo") AndAlso Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("pro_cod_indirizzo")) Then
                                pro_cod_indirizzo = Dt_Appezzamenti.Rows(i).Item("pro_cod_indirizzo")
                            End If

                            Dim stato_indirizzo As String = ""
                            If Dt_Appezzamenti.Columns.Contains("stato_indirizzo") AndAlso Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("stato_indirizzo")) Then
                                stato_indirizzo = Dt_Appezzamenti.Rows(i).Item("stato_indirizzo")
                            End If

                            Dim stato_indirizzo_des As String = ""
                            If Dt_Appezzamenti.Columns.Contains("stato_indirizzo_des") AndAlso Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("stato_indirizzo_des")) Then
                                stato_indirizzo_des = Dt_Appezzamenti.Rows(i).Item("stato_indirizzo_des")
                            End If

                            Dim note_indirizzo As String = ""
                            If Dt_Appezzamenti.Columns.Contains("note_indirizzo") AndAlso Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("note_indirizzo")) Then
                                note_indirizzo = Dt_Appezzamenti.Rows(i).Item("note_indirizzo")
                            End If

                            Dim pro_cod_istat_indirizzo As String = ""
                            If Dt_Appezzamenti.Columns.Contains("pro_cod_istat_indirizzo") AndAlso Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("pro_cod_istat_indirizzo")) Then
                                pro_cod_istat_indirizzo = Dt_Appezzamenti.Rows(i).Item("pro_cod_istat_indirizzo")
                            End If

                            Dim com_cod_istat_indirizzo As String = ""
                            If Dt_Appezzamenti.Columns.Contains("com_cod_istat_indirizzo") AndAlso Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("com_cod_istat_indirizzo")) Then
                                com_cod_istat_indirizzo = Dt_Appezzamenti.Rows(i).Item("com_cod_istat_indirizzo")
                            End If

                            Dim siglaProv = ""
                            If stato_indirizzo = "IT" AndAlso pro_cod_istat_indirizzo <> "" Then
                                objIstat.Provincia_from_CodIstat(pro_cod_istat_indirizzo, siglaProv, objParametri)
                            End If

                            If lProgrammazione_Entita_cod = 0 Then
                                Tipo_Operazione_Entita = enum_TipoOperazioneDB.Scrittura
                            Else
                                Tipo_Operazione_Entita = enum_TipoOperazioneDB.Modifica
                            End If

                            If stato_indirizzo <> "" AndAlso Trim(ind_des) <> "" AndAlso cod_indirizzo <> 0 Then

                                inserisci_indirizzo = True

                            ElseIf stato_indirizzo <> "" AndAlso Trim(ind_des) <> "" AndAlso cod_indirizzo = 0 Then

                                inserisci_indirizzo = True
                                cod_indirizzo = ObjSequenze.NuovoId_Tabella("indirizzi", 0, 2000000000, objParametri)

                            ElseIf cod_indirizzo <> 0 Then

                                cancella_indirizzo = True
                                cod_indirizzo = 0

                            End If

                            Dim Foral_Cod As Integer = 0
                            If Dt_Appezzamenti.Columns.Contains("Foral_Cod") AndAlso Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("Foral_Cod")) Then
                                Foral_Cod = Dt_Appezzamenti.Rows(i).Item("Foral_Cod")
                            End If

                            Dim Mat_Cod As String = ""
                            If Dt_Appezzamenti.Columns.Contains("Mat_Cod") AndAlso Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("Mat_Cod")) Then
                                Mat_Cod = Dt_Appezzamenti.Rows(i).Item("Mat_Cod")
                            End If

                            'DRUDI 2017/10/04 Inserite codifiche agea
                            BoolDummy = objProgrammazione_Entita_W.Scrivi(Codice,
                                                                  lProgrammazione_Entita_cod,
                                                                  Dt_Appezzamenti.Rows(i).Item("App_Nome").ToString,
                                                                  Piva,
                                                                  Dt_Appezzamenti.Rows(i).Item("Sa_Cod"),
                                                                  Dt_Appezzamenti.Rows(i).Item("Campo_Cod"),
                                                                  Dt_Appezzamenti.Rows(i).Item("Appezza"),
                                                                  Dt_Appezzamenti.Rows(i).Item("ID_Reg"),
                                                                  Dt_Appezzamenti.Rows(i).Item("Progetto_Cod"),
                                                                  Progetto_Nome,
                                                                  Dt_Appezzamenti.Rows(i).Item("DestinazioneUso").ToString,
                                                                  Dt_Appezzamenti.Rows(i).Item("Veg_Cod"),
                                                                  Dt_Appezzamenti.Rows(i).Item("Cul_Cod"),
                                                                  Dt_Appezzamenti.Rows(i).Item("Grva_Cod"),
                                                                  Dt_Appezzamenti.Rows(i).Item("Grfi_Cod"),
                                                                  Dt_Appezzamenti.Rows(i).Item("Cop_Cod"),
                                                                  Dt_Appezzamenti.Rows(i).Item("Sup_App"),
                                                                  Dt_Appezzamenti.Rows(i).Item("Resa"),
                                                                  Dt_Appezzamenti.Rows(i).Item("TipoZona"),
                                                                  Dt_Appezzamenti.Rows(i).Item("Veg_Cod_Prec"),
                                                                  0,
                                                                  0,
                                                                  0,
                                                                  Dt_Appezzamenti.Rows(i).Item("Num_Piante"),
                                                                  Dt_Appezzamenti.Rows(i).Item("Tra_Fila"),
                                                                  Dt_Appezzamenti.Rows(i).Item("Su_Fila"),
                                                                  Foral_Cod,
                                                                  Dt_Appezzamenti.Rows(i).Item("Port_cod"),
                                                                  0,
                                                                  Regolamento_Cod,
                                                                  Disciplinare_Cod,
                                                                  Stato_Cod,
                                                                  0,
                                                                  Dt_Appezzamenti.Rows(i).Item("Data_Semina"),
                                                                  Dt_Appezzamenti.Rows(i).Item("Data_Raccolta"),
                                                                  "",
                                                                  veg_cod_cliente,
                                                                  cul_cod_cliente,
                                                                  Dt_Appezzamenti.Rows(i).Item("MetodoProduzione_Cod"),
                                                                  Dt_Appezzamenti.Rows(i).Item("Validita_Inizio"),
                                                                  Dt_Appezzamenti.Rows(i).Item("Validita_Fine"),
                                                                  objParametri,
                                                                  Codice_Fiscale_Tecnico:=codFiscaleTecnico,
                                                                  Operazione_Cod:=Operazione_Cod,
                                                                  Cod_Macrouso:=Dt_Appezzamenti.Rows(i).Item("macrouso_cod"),
                                                                  via_stringa:=via_stringa,
                                                                  Unita_Vitata:=Unita_Vitata,
                                                                  Validita_Inizio_Impianto:=Validita_Inizio_Impianto,
                                                                  Veg_Cod_Agea:=Dt_Appezzamenti.Rows(i).Item("Veg_Cod_Agea"),
                                                                  Cul_Cod_Agea:=Dt_Appezzamenti.Rows(i).Item("Cul_Cod_Agea"),
                                                                  Uso_Cod_Agea:=Dt_Appezzamenti.Rows(i).Item("Uso_Cod_Agea"),
                                                                  Occupazione_Cod_Agea:=Dt_Appezzamenti.Rows(i).Item("Occupazione_Cod_Agea"),
                                                                  Destinazione_Cod_Agea:=Dt_Appezzamenti.Rows(i).Item("Destinazione_Cod_Agea"),
                                                                  Qualita_Cod_Agea:=Dt_Appezzamenti.Rows(i).Item("Qualita_Cod_Agea"),
                                                                  Limite_N:=Limite_N,
                                                                  Limite_P:=Limite_P,
                                                                  Limite_K:=Limite_K,
                                                                  Data_Fioritura_Prevista:=Dt_Appezzamenti.Rows(i).Item("Data_Fioritura_Prevista"),
                                                                  Veg_Cod_Prec2:=Veg_Cod_Prec2,
                                                                  Veg_Cod_Prec3:=Veg_Cod_Prec3,
                                                                  Veg_Cod_Prec4:=Veg_Cod_Prec4,
                                                                  Piano_Semina:=Piano_Semina,
                                                                  Codice_Contratto:=Codice_Contratto,
                                                                  Stato_Ribaltamento:=Stato_Ribaltamento,
                                                                  IAF:=IAF,
                                                                  Flag_PubblicoPrivato:=Flag_PubblicoPrivato,
                                                                  id_tr:=id_tr,
                                                                  Regolamento_Concimazione_Cod:=Regolamento_Concimazione_Cod,
                                                                  DistBZ_CorpiIdrici:=DistBZ_CorpiIdrici,
                                                                  DistBZ_AreeResPub:=DistBZ_AreeResPub,
                                                                  DistBZ_Allevamenti:=DistBZ_Allevamenti,
                                                                  DistBZ_VegNatNonColt:=DistBZ_VegNatNonColt,
                                                                  SupBZ_Riduzione:=SupBZ_Riduzione,
                                                                  riferimento_alfanumerico_appezzamento:=Riferimento_Alfanumerico_Appezzamento,
                                                                  isola:=Isola,
                                                                  CapitolatoPrivato:=CapitolatoPrivato,
                                                                  Finalita_Concimazione_Impianto:=Finalita_Concimazione_Impianto,
                                                                  cod_indirizzo:=cod_indirizzo, Mat_Cod:=Mat_Cod)

                            If inserisci_indirizzo Then
                                objIndirizzi_W.Scrivi(cod_indirizzo,
                                                      ind_des, frz_des, CAP,
                                                      com_des_indirizzo, siglaProv,
                                                      stato_indirizzo, note_indirizzo,
                                                      pro_cod_istat_indirizzo, com_cod_istat_indirizzo,
                                                      AGRODATAINIZIO, AGRODATAFINE, objParametri)
                            End If

                            If cancella_indirizzo Then
                                If Not IsDBNull(Dt_Appezzamenti.Columns.Contains("cod_indirizzo")) AndAlso Dt_Appezzamenti.Columns.Contains("cod_indirizzo") <> 0 Then
                                    objIndirizzi_W.Cancella(Dt_Appezzamenti.Columns.Contains("cod_indirizzo"), "", objParametri)
                                End If
                            End If

                            Dim KPIN As String = ""
                            If Dt_Appezzamenti.Columns.Contains("KPIN") AndAlso Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("KPIN")) Then
                                KPIN = Dt_Appezzamenti.Rows(i).Item("KPIN")
                            End If

                            If KPIN <> "" Then
                                objProgrammazione_Entita_Codici_W.Scrivi(lProgrammazione_Entita_cod,
                                                                         enum_CodiciAnagrafe.Zespri_Codice_kPIN,
                                                                         CStr(KPIN),
                                                                         AGRODATAINIZIO,
                                                                         AGRODATAFINE,
                                                                         objParametri
                                                                         )
                            End If

                            Dim Block_Name As String = ""
                            If Dt_Appezzamenti.Columns.Contains("Block_Name") AndAlso Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("Block_Name")) Then
                                Block_Name = Dt_Appezzamenti.Rows(i).Item("Block_Name")
                            End If

                            If Block_Name <> "" Then
                                objProgrammazione_Entita_Codici_W.Scrivi(lProgrammazione_Entita_cod,
                                                                         enum_CodiciAnagrafe.Zespri_Block_Name,
                                                                         CStr(Block_Name),
                                                                         AGRODATAINIZIO,
                                                                         AGRODATAFINE,
                                                                         objParametri
                                                                         )
                            End If


                            Dim Data_Inizio_Portinnesto As String = ""
                            If Dt_Appezzamenti.Columns.Contains("Data_Inizio_Portinnesto") AndAlso Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("Data_Inizio_Portinnesto")) Then
                                Data_Inizio_Portinnesto = Dt_Appezzamenti.Rows(i).Item("Data_Inizio_Portinnesto")
                            End If

                            If Data_Inizio_Portinnesto <> "" Then
                                objProgrammazione_Entita_Codici_W.Scrivi(lProgrammazione_Entita_cod,
                                                                         enum_CodiciAnagrafe.Data_Inizio_Portinnesto,
                                                                         CStr(Data_Inizio_Portinnesto),
                                                                         AGRODATAINIZIO,
                                                                         AGRODATAFINE,
                                                                         objParametri
                                                                         )
                            End If

                            Dim ZespriFase As String = ""
                            If Dt_Appezzamenti.Columns.Contains("ZespriFase_Cod") AndAlso Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("ZespriFase_Cod")) Then
                                ZespriFase = Dt_Appezzamenti.Rows(i).Item("ZespriFase_Cod")
                            End If

                            If ZespriFase <> "" AndAlso ZespriFase <> "0" Then
                                objProgrammazione_Entita_Codici_W.Scrivi(lProgrammazione_Entita_cod,
                                                                         enum_CodiciAnagrafe.Zespri_Fasi_Fase,
                                                                         ZespriFase,
                                                                         AGRODATAINIZIO,
                                                                         AGRODATAFINE,
                                                                         objParametri
                                                                         )
                            End If

                            Dim ZespriTipo As String = ""
                            If Dt_Appezzamenti.Columns.Contains("ZespriTipo_Cod") AndAlso Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("ZespriTipo_Cod")) Then
                                ZespriTipo = Dt_Appezzamenti.Rows(i).Item("ZespriTipo_Cod")
                            End If

                            If ZespriTipo <> "" AndAlso ZespriTipo <> "0" Then
                                objProgrammazione_Entita_Codici_W.Scrivi(lProgrammazione_Entita_cod,
                                                                         enum_CodiciAnagrafe.Zespri_Fasi_Tipo,
                                                                         ZespriTipo,
                                                                         AGRODATAINIZIO,
                                                                         AGRODATAFINE,
                                                                         objParametri
                                                                         )
                            End If

                            Dim ZespriGrower As String = ""
                            If Dt_Appezzamenti.Columns.Contains("ZespriGrower_Cod") AndAlso Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("ZespriGrower_Cod")) Then
                                ZespriGrower = Dt_Appezzamenti.Rows(i).Item("ZespriGrower_Cod")
                            End If

                            If ZespriGrower <> "" AndAlso ZespriGrower <> "0" Then
                                objProgrammazione_Entita_Codici_W.Scrivi(lProgrammazione_Entita_cod,
                                                                         enum_CodiciAnagrafe.Zespri_Fasi_Grower,
                                                                         ZespriGrower,
                                                                         AGRODATAINIZIO,
                                                                         AGRODATAFINE,
                                                                         objParametri
                                                                         )
                            End If

                            Dim Num_Piante_Femmine As String = ""
                            If Dt_Appezzamenti.Columns.Contains("Num_Piante_Femmine") AndAlso Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("Num_Piante_Femmine")) Then
                                Num_Piante_Femmine = Dt_Appezzamenti.Rows(i).Item("Num_Piante_Femmine")
                            End If

                            If Num_Piante_Femmine <> "" AndAlso Num_Piante_Femmine <> "0" Then
                                objProgrammazione_Entita_Codici_W.Scrivi(lProgrammazione_Entita_cod,
                                                                         enum_CodiciAnagrafe.Num_Piante_Femmine,
                                                                         Num_Piante_Femmine,
                                                                         AGRODATAINIZIO,
                                                                         AGRODATAFINE,
                                                                         objParametri
                                                                         )
                            End If

                            Dim Num_Piante_Maschi As String = ""
                            If Dt_Appezzamenti.Columns.Contains("Num_Piante_Maschi") AndAlso Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("Num_Piante_Maschi")) Then
                                Num_Piante_Maschi = Dt_Appezzamenti.Rows(i).Item("Num_Piante_Maschi")
                            End If

                            If Num_Piante_Maschi <> "" AndAlso Num_Piante_Maschi <> "0" Then
                                objProgrammazione_Entita_Codici_W.Scrivi(lProgrammazione_Entita_cod,
                                                                         enum_CodiciAnagrafe.Num_Piante_Maschi,
                                                                         Num_Piante_Maschi,
                                                                         AGRODATAINIZIO,
                                                                         AGRODATAFINE,
                                                                         objParametri
                                                                         )
                            End If

                            Dim TipologiaInnestoTrapianto_cod As String = ""
                            If Dt_Appezzamenti.Columns.Contains("TipologiaInnestoTrapianto_cod") AndAlso Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("TipologiaInnestoTrapianto_cod")) Then
                                TipologiaInnestoTrapianto_cod = Dt_Appezzamenti.Rows(i).Item("TipologiaInnestoTrapianto_cod")
                            End If


                            If TipologiaInnestoTrapianto_cod <> "" AndAlso TipologiaInnestoTrapianto_cod <> "0" Then
                                objProgrammazione_Entita_Codici_W.Scrivi(lProgrammazione_Entita_cod,
                                                                         enum_CodiciAnagrafe.TipologiaDIInnestoTrapianto,
                                                                         TipologiaInnestoTrapianto_cod,
                                                                         AGRODATAINIZIO,
                                                                         AGRODATAFINE,
                                                                         objParametri
                                                                         )
                            End If


                            If dt_appezzamenti_Contiene_GIS Then

                                Dim Dt_Appezzamenti_WKT As String =
                                    Dt_Appezzamenti.Rows(i).Item("wkt").ToString()

                                Dim Dt_Appezzamenti_WKT_GeoRiferimentoCod As String =
                                    Dt_Appezzamenti.Rows(i).Item("wkt_georiferimento_cod").ToString()

                                If Not String.IsNullOrEmpty(Dt_Appezzamenti_WKT) AndAlso
                                   Tipo_Operazione_Entita = enum_TipoOperazioneDB.Scrittura Then

                                    ProgrammazioneEntitaScriviDatoCartografico(
                                        Programmazione_Cod, Piva, lProgrammazione_Entita_cod,
                                        Dt_Appezzamenti.Rows(i).Item("Sa_Cod"), Dt_Appezzamenti_WKT,
                                        Dt_Appezzamenti_WKT_GeoRiferimentoCod, ScriviElementiGrafici, objParametri)
                                End If

                            End If


                            If Pratica_Cod <> "" AndAlso Pratica_Cod <> "0" Then
                                objProgrammazione_Entita_Codici_W.Scrivi(lProgrammazione_Entita_cod,
                                                                         enum_CodiciAnagrafe.Programmazione_Impianto_Pratica_Cod,
                                                                         CStr(Pratica_Cod),
                                                                         AGRODATAINIZIO,
                                                                         AGRODATAFINE,
                                                                         objParametri
                                                                         )
                            End If


                            If NumeroDiValidazione <> "" Then
                                '(05-09-2017 fede) rendo opzionale il salvataggio dell'allegato
                                If SalvaAllegato Then
                                    If Dt_Appezzamenti.Rows(i).Item("provenienza_fascicolo") <> "" Then
                                        objFascicoloEntita.Scrivi(allegati_Documenti_cod,
                                                             0,
                                                             Piva,
                                                             0,
                                                             0,
                                                             0, 0,
                                                             0,
                                                             "", "", "", 0, 0, "", "", "",
                                                             0, Codice, lProgrammazione_Entita_cod,
                                                             Dt_Appezzamenti.Rows(i).Item("Validita_Inizio"),
                                                             Dt_Appezzamenti.Rows(i).Item("Validita_Fine"),
                                                             objParametri)
                                    End If
                                End If
                            End If


                            Select Case Dt_Appezzamenti.Rows(i).Item("Appezza")
                                Case 0
                                    Dr = Dt_Particelle.Select("Piva='" & Piva & "' AND Sa_Cod=" & Dt_Appezzamenti.Rows(i).Item("Sa_Cod") & " AND Campo_Cod=" & Dt_Appezzamenti.Rows(i).Item("Campo_Cod"))
                                Case Else
                                    Dr = Dt_Particelle.Select("Piva='" & Piva & "' AND Sa_Cod=" & Dt_Appezzamenti.Rows(i).Item("Sa_Cod") & " AND Appezza=" & Dt_Appezzamenti.Rows(i).Item("Appezza"))
                            End Select

                            If Dr IsNot Nothing AndAlso Dr.Length > 0 Then

                                Dim objProgrammazione_Particelle_W As New AgronicaCoreAnagrafeDAL.Programmazione_Particelle_W
                                Dim objProgrammazione_Particelle_R As New AgronicaCoreAnagrafeDAL.Programmazione_Particelle_R

                                For j = 0 To Dr.Length - 1

                                    Dim DT_Programmazione_particelle = objProgrammazione_Particelle_R.Programmazione_Particelle_Leggi_2(objParametri,
                                                                                                     Codice,
                                                                                                     "",
                                                                                                     lProgrammazione_Entita_cod,
                                                                                                     Dr(j).Item("Prov").ToString,
                                                                                                     Dr(j).Item("Com").ToString,
                                                                                                     Dr(j).Item("Sezione").ToString,
                                                                                                     Dr(j).Item("Foglio").ToString,
                                                                                                     Dr(j).Item("Numero").ToString,
                                                                                                     Dr(j).Item("Subalterno").ToString)

                                    If DT_Programmazione_particelle.Rows.Count > 0 Then

                                        objProgrammazione_Particelle_W.Cancella(Piva,
                                                                                lProgrammazione_Entita_cod,
                                                                                Dr(j).Item("Prov").ToString,
                                                                                Dr(j).Item("Com").ToString,
                                                                                Dr(j).Item("Sezione").ToString,
                                                                                Dr(j).Item("Foglio").ToString,
                                                                                Dr(j).Item("Numero").ToString,
                                                                                Dr(j).Item("Subalterno").ToString,
                                                                                "",
                                                                                objParametri)

                                    End If

                                    BoolDummy = objProgrammazione_Particelle_W.Scrivi(lProgrammazione_Entita_cod,
                                                                                          Dr(j).Item("Prov").ToString,
                                                                                          Dr(j).Item("Com").ToString,
                                                                                          Dr(j).Item("Sezione").ToString,
                                                                                          Dr(j).Item("Foglio").ToString,
                                                                                          Dr(j).Item("Numero").ToString,
                                                                                          Dr(j).Item("Subalterno").ToString,
                                                                                          Dr(j).Item("SupIntersezione").ToString,
                                                                                          Dt_Appezzamenti.Rows(i).Item("Validita_Inizio"),
                                                                                          Dt_Appezzamenti.Rows(i).Item("Validita_Fine"),
                                                                                          objParametri)


                                    If Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("TipoZona")) AndAlso Dt_Appezzamenti.Rows(i).Item("TipoZona").ToString.Trim = "v" Then

                                        If objZonexParticelle_R.Leggi(enum_Zone.ZVN,
                                                                      Dr(j).Item("Prov").ToString,
                                                                      Dr(j).Item("Com").ToString,
                                                                      Dr(j).Item("Sezione").ToString,
                                                                      Dr(j).Item("Foglio").ToString,
                                                                      Dr(j).Item("Numero").ToString,
                                                                      Dr(j).Item("Subalterno").ToString,
                                                                      enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                      "",
                                                                      "",
                                                                      objParametri).Rows.Count = 0 Then
                                            Dim dtPart = objParticelle_R.Leggi(0,
                                                                               Piva,
                                                                               0,
                                                                               0,
                                                                               Dr(j).Item("Prov").ToString,
                                                                               Dr(j).Item("Com").ToString,
                                                                               Dr(j).Item("Sezione").ToString,
                                                                               Dr(j).Item("Foglio").ToString,
                                                                               Dr(j).Item("Numero").ToString,
                                                                               Dr(j).Item("Subalterno").ToString,
                                                                               enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                               "",
                                                                               "",
                                                                               objParametri)
                                            If dtPart.Rows.Count > 0 Then
                                                Dim area = dtPart(0)("Sup_Condotta")
                                                objZonexParticelle_w.Scrivi(enum_Zone.ZVN,
                                                                            Dr(j).Item("Prov").ToString,
                                                                            Dr(j).Item("Com").ToString,
                                                                            Dr(j).Item("Sezione").ToString,
                                                                            Dr(j).Item("Foglio").ToString,
                                                                            Dr(j).Item("Numero").ToString,
                                                                            Dr(j).Item("Subalterno").ToString,
                                                                            area,
                                                                            AGRODATAINIZIO,
                                                                            AGRODATAFINE,
                                                                            objParametri)
                                            End If
                                        End If

                                    End If


                                Next

                            End If



                            ' AGGIORNAMENTO DEGLI APPEZZAMENTI ELIMINATI
                            DrAppEliminati = Dt_Appezzamenti_Eliminati.Select("UNID_APP_NEW = '" & Dt_Appezzamenti.Rows(i).Item("UNID_APP") & "'")
                            If DrAppEliminati IsNot Nothing AndAlso DrAppEliminati.Length > 0 Then

                                For j = 0 To DrAppEliminati.Length - 1
                                    ' vengono direttamente modificati i campi del datatable, il datarow punta allo stessa area di memoria del dt
                                    DrAppEliminati(j).Item("Programmazione_Entita_Cod") = lProgrammazione_Entita_cod

                                Next
                            End If

                        Next

                    End If

                    If Not IsNothing(Dt_Appezzamenti_Eliminati) Then

                        Dim objProgrammazione_Entita_Eliminate_W As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_Eliminate_W

                        For i = 0 To Dt_Appezzamenti_Eliminati.Rows.Count - 1

                            'metto in un try xkè quando fraziono un appezzamento proveniente dal reale
                            'da errore di inserimento chiave duplicata
                            Try
                                Dim DataChiusura As Date = Estremo_Validita_Inizio

                                If Not IsDBNull(Dt_Appezzamenti_Eliminati.Rows(i).Item("Data_Chiusura")) AndAlso IsDate(Dt_Appezzamenti_Eliminati.Rows(i).Item("Data_Chiusura")) Then
                                    DataChiusura = CDate(Dt_Appezzamenti_Eliminati.Rows(i).Item("Data_Chiusura"))
                                End If

                                BoolDummy = objProgrammazione_Entita_Eliminate_W.Scrivi(
                                                              Piva,
                                                              Dt_Appezzamenti_Eliminati.Rows(i).Item("Sa_Cod"),
                                                              Dt_Appezzamenti_Eliminati.Rows(i).Item("Appezza"),
                                                              Dt_Appezzamenti_Eliminati.Rows(i).Item("ID_Reg"),
                                                              Dt_Appezzamenti_Eliminati.Rows(i).Item("Campo_Cod"),
                                                              Codice,
                                                              Dt_Appezzamenti_Eliminati.Rows(i).Item("Programmazione_Entita_Cod"),
                                                              Dt_Appezzamenti_Eliminati.Rows(i).Item("Progetto_Cod"),
                                                              Dt_Appezzamenti_Eliminati.Rows(i).Item("Operazione_Cod"),
                                                              DataChiusura,
                                                              Estremo_Validita_Fine,
                                                              objParametri)

                            Catch ex As Exception
                                MessaggioErrore = ex.Message
                            End Try

                        Next

                    End If

                    'Return Codice


                    '----------------------------------------------------------
                    '----------------------------------------------------------
                    '------    CANCELLAZIONE    -------------------------------
                    '----------------------------------------------------------
                    '----------------------------------------------------------

                Case enum_TipoOperazioneDB.Cancellazione

                    Dim FiltroEntita As String = ""
                    If strEntitaDaMantenere <> "" Then
                        FiltroEntita = "Programmazione_Entita.Programmazione_Entita_Cod NOT IN (" & strEntitaDaMantenere & ")"
                    End If

                    '-------------------------------------------------------
                    'ELIMINO i record in Programmazione_Particelle 
                    'ELIMINO i record in Programmazione_Entita_Codici
                    '(devo ricavare i Programmazione_Entita_Cod x farlo)
                    Dt = New DataTable

                    Dim objProgrammazione_Entita As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_R

                    Dt = objProgrammazione_Entita.Programmazione_Entita_Leggi(
                                                Programmazione_Cod,
                                                ErrMSG,
                                                0,
                                                "",
                                                "",
                                                0,
                                                0,
                                                0,
                                                0,
                                                0,
                                                Estremo_Validita_Inizio,
                                                Estremo_Validita_Fine,
                                                1,
                                                enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                FiltroEntita,
                                                "",
                                                objParametri,
                                                0, True)

                    Dim objAllegati_Documenti As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_W
                    Dim objAllegati_Entita As New AgronicaCoreAnagrafeDAL.Allegati_EntitaxDocumenti_W
                    Dim objAllegati_Entita_R As New AgronicaCoreAnagrafeDAL.Allegati_EntitaxDocumenti_R
                    Dim Allegati_Documenti_Cod = 0

                    If ErrMSG = "" Then

                        Dim objProgrammazione_Particelle_W As New AgronicaCoreAnagrafeDAL.Programmazione_Particelle_W
                        Dim objProgrammazione_Entita_Codici_W As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_Codici_W
                        Dim objIndirizzi As New AgronicaCoreAnagrafeDAL.Indirizzi_Write

                        Dim Campo_Cod = 0
                        Dim Piva_Campo = ""
                        Dim Sa_Cod_Campo = 0


                        Dim dtAllegati = objAllegati_Entita_R.Leggi(0, Piva, 0, 0, 0, 0, 0, "", "", "", 0, 0, "", "", "", 0, Programmazione_Cod, 0, "", "", objParametri)
                        If dtAllegati.Rows.Count > 0 Then
                            Allegati_Documenti_Cod = dtAllegati.Rows(0)("Allegati_Documenti_Cod")
                        End If

                        For i = 0 To Dt.Rows.Count - 1

                            If Not IsDBNull(Dt.Rows(i).Item("cod_indirizzo")) AndAlso Dt.Rows(i).Item("cod_indirizzo") <> 0 Then

                                BoolDummy = objIndirizzi.Cancella(Dt.Rows(i).Item("cod_indirizzo"), "", objParametri)

                            End If

                            BoolDummy = objProgrammazione_Particelle_W.Cancella("",
                                                                                Dt.Rows(i).Item("Programmazione_Entita_Cod"),
                                                                                  "",
                                                                                  "",
                                                                                  "0",
                                                                                  0,
                                                                                  0,
                                                                                  "0",
                                                                                 "",
                                                                                 objParametri)

                            BoolDummy = objProgrammazione_Entita_Codici_W.Cancella(
                                                            Dt.Rows(i).Item("Programmazione_Entita_Cod"),
                                                            0,
                                                            "",
                                                            objParametri)

                            BoolDummy = objAllegati_Entita.Cancella_Programmazione_Entita_cod(Dt.Rows(i).Item("Programmazione_Entita_Cod"), "", objParametri)

                            If EliminaAppezzamentiRibaltati Then

                                If Not (Dt.Rows(i).Item("piva_ribaltato") = "" AndAlso Dt.Rows(i).Item("sa_cod_ribaltato") = 0 AndAlso Dt.Rows(i).Item("appezza_ribaltato") = 0) Then

                                    Dim objAppezzamentoR As New AgronicaCoreAnagrafeBIZ.Appezzamento_R
                                    Dim objAppezzamentoR_DAL As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
                                    Dim DT_Appezzamento = objAppezzamentoR_DAL.Leggi(Dt.Rows(i).Item("piva_ribaltato"), Dt.Rows(i).Item("sa_cod_ribaltato"), Dt.Rows(i).Item("appezza_ribaltato"), enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)
                                    If DT_Appezzamento IsNot Nothing AndAlso DT_Appezzamento.Rows.Count > 0 Then
                                        Campo_Cod = DT_Appezzamento.Rows(0).Item("Campo_Cod")
                                        Piva_Campo = DT_Appezzamento.Rows(0).Item("PIVA")
                                        Sa_Cod_Campo = DT_Appezzamento.Rows(0).Item("SA_COD")
                                    End If

                                    Dim StrXmlCancella As String = objAppezzamentoR.Appezzamento_Leggi(
                                                                                   Dt.Rows(i).Item("piva_ribaltato"),
                                                                                    Dt.Rows(i).Item("sa_cod_ribaltato"),
                                                                                    0, Dt.Rows(i).Item("appezza_ribaltato"),
                                                                                   True, False, False, False, objParametri)
                                    If StrXmlCancella <> "" Then
                                        Dim objAppezzamentoW As New AgronicaCoreAnagrafeBIZ.Appezzamento_W
                                        objAppezzamentoW.Appezzamento_Scrivi(
                                                           CStr(StrXmlCancella),
                                                       Dt.Rows(i).Item("piva_ribaltato"),
                                                       Dt.Rows(i).Item("sa_cod_ribaltato"),
                                                       Dt.Rows(i).Item("appezza_ribaltato"),
                                                           objParametri,
                                                           Nothing)
                                    End If

                                End If
                            End If

                        Next

                        ' Se è vuoto elimino il campo

                        If strEntitaDaMantenere = "" AndAlso EliminaAppezzamentiRibaltati Then
                            If Campo_Cod <> 0 Then
                                Dim objCampoR As New AgronicaCoreAnagrafeBIZ.Campo_R
                                Dim objCampoW As New AgronicaCoreAnagrafeBIZ.Campo_W
                                Dim StrXmlCancella As String = objCampoR.Campo_Leggi(Piva_Campo,
                                                                                     Sa_Cod_Campo,
                                                                                     Campo_Cod,
                                                                                     AGRODATAINIZIO,
                                                                                     AGRODATAFINE,
                                                                                     True, False, objParametri)

                                'objCampoW.Campo_Scrivi(StrXmlCancella, Piva_Campo, Sa_Cod_Campo, Campo_Cod, True, objParametri, objParametri_Utenti)

                            End If
                        End If

                        Dim FiltroCancellazione As String = ""
                        If strEntitaDaMantenere <> "" Then
                            FiltroCancellazione = "Programmazione_Entita_Cod NOT IN (" & strEntitaDaMantenere & ")"
                        End If

                        '-------------------------------------------------------
                        'ELIMINO i record in Programmazione_Entita_Eliminate
                        Dim objProgrammazione_Entita_Eliminate_W As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_Eliminate_W

                        BoolDummy = objProgrammazione_Entita_Eliminate_W.Cancella(Programmazione_Cod,
                                                          0,
                                                          FiltroCancellazione,
                                                          objParametri)


                        '-------------------------------------------------------
                        'ELIMINO i record in Programmazione_Entita
                        Dim objProgrammazione_Entita_W As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_W

                        BoolDummy = objProgrammazione_Entita_W.Cancella(Programmazione_Cod,
                                                                        0,
                                                                        False,
                                                                        False,
                                                                        0,
                                                                        FiltroCancellazione,
                                                                        objParametri)


                    End If

                    If strEntitaDaMantenere = "" Then

                        '-------------------------------------------------------
                        'ELIMINO i record in Programmazione_Testata
                        Dim objProgrammazione_Testata_W As New AgronicaCoreAnagrafeDAL.Programmazione_Testata_W

                        BoolDummy = objProgrammazione_Testata_W.Cancella(Programmazione_Cod,
                                                                         "",
                                                                         objParametri)

                        objAllegati_Entita.Cancella_Programmazione_cod(Programmazione_Cod, "", objParametri)


                        '  Vanni, 06/06/2013 10:27:35: cancello legame fra allegati e planning..
                        '-------------------------------------------------------
                        'ELIMINO i record in Allegati_EntitaxDocumenti
                        Dim objAllegati_EntitaxDocumenti As New AgronicaCoreAnagrafeDAL.Allegati_EntitaxDocumenti_W
                        objAllegati_EntitaxDocumenti.Cancella_Programmazione_cod(
                            Programmazione_Cod, "", objParametri)

                    End If

                    'If Allegati_Documenti_Cod <> 0 Then
                    '    If objAllegati_Entita_R.Leggi(Allegati_Documenti_Cod, "", 0, 0, 0, 0, 0, "", "", "", 0, 0, "", "", "", 0, 0, 0, "", "", objParametri).Rows.Count = 0 Then
                    '        objAllegati_Documenti.Cancella(Allegati_Documenti_Cod, "", objParametri)
                    '    End If
                    'End If


                    Codice = Programmazione_Cod


                    '----------------------------------------------------------
                    '----------------------------------------------------------
                    '------    MODIFICA    -------------------------------
                    '----------------------------------------------------------
                    '----------------------------------------------------------

                Case enum_TipoOperazioneDB.Modifica

                    Dim objProgrammazione_Entita_R As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_R
                    Dim objProgrammazione_Particelle_W As New AgronicaCoreAnagrafeDAL.Programmazione_Particelle_W
                    Dim objProgrammazione_Entita_Eliminate_W As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_Eliminate_W
                    Dim objProgrammazione_Entita_W As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_W
                    Dim objProgrammazione_Testata_W As New AgronicaCoreAnagrafeDAL.Programmazione_Testata_W

                    '------------------------------------
                    ' CANCELLAZIONE
                    '------------------------------------

                    '-------------------------------------------------------
                    'ELIMINO i record in Programmazione_Particelle 
                    'ELIMINO i record in Programmazione_Entita_Codici
                    '(devo ricavare i Programmazione_Entita_Cod x farlo)
                    Dt = New DataTable

                    Dim objProgrammazione_Entita As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_R

                    Dt = objProgrammazione_Entita.Programmazione_Entita_Leggi(
                                                Programmazione_Cod,
                                                ErrMSG,
                                                0,
                                                "",
                                                "",
                                                0,
                                                0,
                                                0,
                                                0,
                                                0,
                                                Estremo_Validita_Inizio,
                                                Estremo_Validita_Fine,
                                                1,
                                                enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                "",
                                                "",
                                                objParametri)

                    If ErrMSG = "" Then

                        Dim objProgrammazione_Entita_Codici_W As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_Codici_W

                        For i = 0 To Dt.Rows.Count - 1
                            BoolDummy = objProgrammazione_Particelle_W.Cancella("",
                                                                                Dt.Rows(i).Item("Programmazione_Entita_Cod"),
                                                                                  "",
                                                                                  "",
                                                                                  "0",
                                                                                  0,
                                                                                  0,
                                                                                  "0",
                                                                                 "",
                                                                                 objParametri)

                            BoolDummy = objProgrammazione_Entita_Codici_W.Cancella(
                                                            Dt.Rows(i).Item("Programmazione_Entita_Cod"),
                                                            0,
                                                            "",
                                                            objParametri)
                        Next

                        '-------------------------------------------------------
                        'ELIMINO i record in Programmazione_Entita_Eliminate
                        BoolDummy = objProgrammazione_Entita_Eliminate_W.Cancella(Programmazione_Cod,
                                                          0,
                                                          "",
                                                          objParametri)


                        '-------------------------------------------------------
                        'ELIMINO i record in Programmazione_Entita
                        BoolDummy = objProgrammazione_Entita_W.Cancella(Programmazione_Cod,
                                                                        0,
                                                                        False,
                                                                        False,
                                                                        0,
                                                                        "",
                                                                        objParametri)



                    End If




                    '-------------------------------------------------------
                    'ELIMINO i record in Programmazione_Testata
                    BoolDummy = objProgrammazione_Testata_W.Cancella(Programmazione_Cod,
                                                                     "",
                                                                     objParametri)



                    '------------------------------------
                    ' INSERIMENTO
                    '------------------------------------

                    BoolDummy = objProgrammazione_Testata_W.Scrivi(Programmazione_Cod,
                                                                    Programmazione_Des,
                                                                    Programmazione_Des_Long,
                                                                    Piva,
                                                                    Note,
                                                                    Tipo_Pianificazione,
                                                                    Tipo_Fonte,
                                                                    Validita_Inizio,
                                                                    Validita_Fine,
                                                                    objParametri,
                                                                    Stato_SQNPI:=Stato_SQNPI)


                    '  Vanni, 06/06/2013 11:45:21: creo o modifico il record per validazione_numero
                    Dim allegati_Documenti_cod As Integer = 0

                    If NumeroDiValidazione <> "" Then

                        '(05-09-2017 fede) rendo opzionale il salvataggio dell'allegato
                        If SalvaAllegato Then

                            Dim allegatiDocumentiLeggi As New AgronicaCoreAnagrafeDAL.Allegati_EntitaxDocumenti_R
                            Dim dtallegatiDocumentiLeggi As DataTable =
                                allegatiDocumentiLeggi.Leggi(
                                    0,
                                    Piva,
                                    0,
                                    0,
                                    0,
                                    0,
                                    0,
                                    "",
                                    "",
                                    "",
                                    0,
                                    0,
                                    "",
                                    "",
                                    "",
                                    0,
                                    0,
                                     0,
                                    "",
                                    "",
                                    objParametri
                                )


                            If dtallegatiDocumentiLeggi.Rows.Count > 0 Then
                                allegati_Documenti_cod = dtallegatiDocumentiLeggi.Rows(0)("allegati_Documenti_cod")
                                Dim allegati_Documenti_w As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_W
                                allegati_Documenti_w.Modifica_NumeroDataValidazione_DatoPlanning(
                                    Programmazione_Cod,
                                    NumeroDiValidazione,
                                    DataDiValidazione,
                                    objParametri
                                )
                            Else
                                Dim objFascicoloXEntita As New AgronicaCoreAnagrafeDAL.Allegati_EntitaxDocumenti_W
                                ScriviAllegatoEdEntitaAllegatoPlann(Programmazione_Des, Piva, Dt_Appezzamenti, NumeroDiValidazione, DataDiValidazione, objParametri, Programmazione_Cod, 0, objFascicoloEntita, allegati_Documenti_cod)

                            End If


                            '  Vanni, 06/06/2013 10:27:35: cancello legame fra allegati e planning..
                            '-------------------------------------------------------
                            'ELIMINO i record in Allegati_EntitaxDocumenti
                            Dim objAllegati_EntitaxDocumenti As New AgronicaCoreAnagrafeDAL.Allegati_EntitaxDocumenti_W
                            objAllegati_EntitaxDocumenti.Cancella_Programmazione_cod(
                                Programmazione_Cod, "", objParametri)

                        End If

                    End If



                    If Not IsNothing(Dt_Appezzamenti) Then

                        For i = 0 To Dt_Appezzamenti.Rows.Count - 1

                            Dim data_Creazione As Date = #2/1/1900#
                            Dim drow As DataRow() = Dt.Select(" Programmazione_Entita_Cod = " & Dt_Appezzamenti.Rows(i).Item("Programmazione_Entita_Cod").ToString)
                            If drow.Length > 0 Then
                                data_Creazione = drow(0)("data_creazione")
                            End If

                            Dim codice_Fiscale_tecnico As String = ""
                            If Dt_Appezzamenti.Columns.Contains("Codice_Fiscale_Tecnico") Then
                                codice_Fiscale_tecnico = Dt_Appezzamenti.Rows(i).Item("codice_fiscale_tecnico")
                            End If

                            Dim Operazione_Cod As Integer = 0
                            If Dt_Appezzamenti.Columns.Contains("Operazione_Cod") Then
                                Operazione_Cod = Dt_Appezzamenti.Rows(i).Item("Operazione_Cod")
                            End If

                            Dim veg_cod_cliente, cul_cod_cliente As String
                            If IsDBNull(Dt_Appezzamenti.Rows(i).Item("veg_cod_cliente")) Then
                                veg_cod_cliente = ""
                            Else
                                veg_cod_cliente = Dt_Appezzamenti.Rows(i).Item("veg_cod_cliente")
                            End If

                            If IsDBNull(Dt_Appezzamenti.Rows(i).Item("cul_cod_cliente")) Then
                                cul_cod_cliente = ""
                            Else
                                cul_cod_cliente = Dt_Appezzamenti.Rows(i).Item("cul_cod_cliente")
                            End If

                            Dim via_stringa As String = ""
                            If Dt_Appezzamenti.Columns.Contains("via_stringa") Then
                                via_stringa = Dt_Appezzamenti.Rows(i).Item("via_stringa").ToString.Replace("'", "`")
                            End If

                            Dim Unita_Vitata As Integer = 0
                            If Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("unita_vitata")) Then
                                Unita_Vitata = Dt_Appezzamenti.Rows(i).Item("unita_vitata")
                            Else
                                Unita_Vitata = 0
                            End If

                            Dim Validita_Inizio_Impianto As Date = #1/1/1900#
                            If Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("Validita_Inizio_Impianto")) Then
                                Validita_Inizio_Impianto = Dt_Appezzamenti.Rows(i).Item("Validita_Inizio_Impianto")
                            Else
                                Validita_Inizio_Impianto = Dt_Appezzamenti.Rows(i).Item("Validita_Inizio")
                            End If

                            Dim Stato_Ribaltamento As Integer = 0
                            If Dt_Appezzamenti.Columns.Contains("Stato_Ribaltamento") AndAlso Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("Stato_Ribaltamento")) Then
                                Stato_Ribaltamento = Dt_Appezzamenti.Rows(i).Item("Stato_Ribaltamento")
                            End If

                            Dim IAF As String = ""
                            If Dt_Appezzamenti.Columns.Contains("IAF") AndAlso Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("IAF")) Then
                                IAF = Dt_Appezzamenti.Rows(i).Item("IAF")
                            End If

                            Dim DistBZ_CorpiIdrici As Double = 0
                            If Dt_Appezzamenti.Columns.Contains("DistBZ_CorpiIdrici") AndAlso Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("DistBZ_CorpiIdrici")) Then
                                DistBZ_CorpiIdrici = Dt_Appezzamenti.Rows(i).Item("DistBZ_CorpiIdrici")
                            End If

                            Dim DistBZ_AreeResPub As Double = 0
                            If Dt_Appezzamenti.Columns.Contains("DistBZ_AreeResPub") AndAlso Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("DistBZ_AreeResPub")) Then
                                DistBZ_AreeResPub = Dt_Appezzamenti.Rows(i).Item("DistBZ_AreeResPub")
                            End If

                            Dim DistBZ_Allevamenti As Double = 0
                            If Dt_Appezzamenti.Columns.Contains("DistBZ_Allevamenti") AndAlso Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("DistBZ_Allevamenti")) Then
                                DistBZ_Allevamenti = Dt_Appezzamenti.Rows(i).Item("DistBZ_Allevamenti")
                            End If

                            Dim DistBZ_VegNatNonColt As Double = 0
                            If Dt_Appezzamenti.Columns.Contains("DistBZ_VegNatNonColt") AndAlso Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("DistBZ_VegNatNonColt")) Then
                                DistBZ_VegNatNonColt = Dt_Appezzamenti.Rows(i).Item("DistBZ_VegNatNonColt")
                            End If

                            'DRUDI 2017/10/04 Inserite codifiche agea
                            BoolDummy = objProgrammazione_Entita_W.Scrivi(Programmazione_Cod,
                                                                  Dt_Appezzamenti.Rows(i).Item("Programmazione_Entita_Cod"),
                                                                  Dt_Appezzamenti.Rows(i).Item("App_Nome").ToString,
                                                                  Piva,
                                                                  Dt_Appezzamenti.Rows(i).Item("Sa_Cod"),
                                                                  Dt_Appezzamenti.Rows(i).Item("Campo_Cod"),
                                                                  Dt_Appezzamenti.Rows(i).Item("Appezza"),
                                                                  Dt_Appezzamenti.Rows(i).Item("ID_Reg"),
                                                                  Dt_Appezzamenti.Rows(i).Item("Progetto_Cod"),
                                                                  Dt_Appezzamenti.Rows(i).Item("Progetto_Nome").ToString,
                                                                  Dt_Appezzamenti.Rows(i).Item("DestinazioneUso").ToString,
                                                                  Dt_Appezzamenti.Rows(i).Item("Veg_Cod"),
                                                                  Dt_Appezzamenti.Rows(i).Item("Cul_Cod"),
                                                                  Dt_Appezzamenti.Rows(i).Item("Grva_Cod"),
                                                                  Dt_Appezzamenti.Rows(i).Item("Grfi_Cod"),
                                                                  Dt_Appezzamenti.Rows(i).Item("Cop_Cod"),
                                                                  Dt_Appezzamenti.Rows(i).Item("Sup_App"),
                                                                  Dt_Appezzamenti.Rows(i).Item("Resa"),
                                                                  Dt_Appezzamenti.Rows(i).Item("TipoZona"),
                                                                  0,
                                                                  0,
                                                                  0,
                                                                  0,
                                                                  Dt_Appezzamenti.Rows(i).Item("Num_Piante"),
                                                                  Dt_Appezzamenti.Rows(i).Item("Tra_Fila"),
                                                                  Dt_Appezzamenti.Rows(i).Item("Su_Fila"),
                                                                  0,
                                                                  0,
                                                                  0,
                                                                  1,
                                                                  0,
                                                                  102,
                                                                  0,
                                                                  Dt_Appezzamenti.Rows(i).Item("Data_Semina"),
                                                                  Dt_Appezzamenti.Rows(i).Item("Data_Raccolta"),
                                                                  "",
                                                                  veg_cod_cliente,
                                                                  cul_cod_cliente,
                                                                  Dt_Appezzamenti.Rows(i).Item("MetodoProduzione_Cod"),
                                                                  Dt_Appezzamenti.Rows(i).Item("Validita_Inizio"),
                                                                  Dt_Appezzamenti.Rows(i).Item("Validita_Fine"),
                                                                  objParametri,
                                                                  Codice_Fiscale_Tecnico:=codice_Fiscale_tecnico,
                                                                  Data_creazione:=data_Creazione,
                                                                  Operazione_Cod:=Operazione_Cod,
                                                                  Cod_Macrouso:=Dt_Appezzamenti.Rows(i).Item("macrouso_cod"),
                                                                  via_stringa:=via_stringa,
                                                                  Unita_Vitata:=Unita_Vitata,
                                                                  Validita_Inizio_Impianto:=Validita_Inizio_Impianto,
                                                                  Cul_Cod_Agea:=Dt_Appezzamenti.Rows(i).Item("Cul_Cod_Agea"),
                                                                  Uso_Cod_Agea:=Dt_Appezzamenti.Rows(i).Item("Uso_Cod_Agea"),
                                                                  Occupazione_Cod_Agea:=Dt_Appezzamenti.Rows(i).Item("Occupazione_Cod_Agea"),
                                                                  Destinazione_Cod_Agea:=Dt_Appezzamenti.Rows(i).Item("Destinazione_Cod_Agea"),
                                                                  Qualita_Cod_Agea:=Dt_Appezzamenti.Rows(i).Item("Qualita_Cod_Agea"),
                                                                          Stato_Ribaltamento:=Stato_Ribaltamento, IAF:=IAF,
                                                                          DistBZ_CorpiIdrici:=DistBZ_CorpiIdrici,
                                                                  DistBZ_AreeResPub:=DistBZ_AreeResPub,
                                                                  DistBZ_Allevamenti:=DistBZ_Allevamenti,
                                                                  DistBZ_VegNatNonColt:=DistBZ_VegNatNonColt
                                                                  )

                            '  Vanni, 06/06/2013 11:45:21: creo i record per allegatiXEntita sulla riga di planning
                            If NumeroDiValidazione <> "" AndAlso allegati_Documenti_cod <> 0 Then
                                '(05-09-2017 fede) rendo opzionale il salvataggio dell'allegato
                                If SalvaAllegato Then
                                    objFascicoloEntita.Scrivi(allegati_Documenti_cod,
                                                             0,
                                                             Piva,
                                                             0,
                                                             0,
                                                             0, 0,
                                                             0,
                                                             "", "", "", 0, 0, "", "", "",
                                                             0, Programmazione_Cod, Dt_Appezzamenti.Rows(i).Item("Programmazione_Entita_Cod"),
                                                             Dt_Appezzamenti.Rows(i).Item("Validita_Inizio"),
                                                             Dt_Appezzamenti.Rows(i).Item("Validita_Fine"),
                                                             objParametri)

                                End If
                            End If


                            Select Case Dt_Appezzamenti.Rows(i).Item("Appezza")
                                Case 0
                                    Dr = Dt_Particelle.Select("Piva='" & Piva & "' AND Sa_Cod=" & Dt_Appezzamenti.Rows(i).Item("Sa_Cod") & " AND Campo_Cod=" & Dt_Appezzamenti.Rows(i).Item("Campo_Cod"))
                                Case Else
                                    Dr = Dt_Particelle.Select("Piva='" & Piva & "' AND Sa_Cod=" & Dt_Appezzamenti.Rows(i).Item("Sa_Cod") & " AND Appezza=" & Dt_Appezzamenti.Rows(i).Item("Appezza"))
                            End Select

                            If Dr IsNot Nothing AndAlso Dr.Length > 0 Then


                                For j = 0 To Dr.Length - 1


                                    BoolDummy = objProgrammazione_Particelle_W.Scrivi(Dt_Appezzamenti.Rows(i).Item("Programmazione_Entita_Cod"),
                                                                                      Dr(j).Item("Prov").ToString,
                                                                                      Dr(j).Item("Com").ToString,
                                                                                      Dr(j).Item("Sezione").ToString,
                                                                                      Dr(j).Item("Foglio").ToString,
                                                                                      Dr(j).Item("Numero").ToString,
                                                                                      Dr(j).Item("Subalterno").ToString,
                                                                                      Dr(j).Item("SupIntersezione").ToString,
                                                                                      Dt_Appezzamenti.Rows(i).Item("Validita_Inizio"),
                                                                                      Dt_Appezzamenti.Rows(i).Item("Validita_Fine"),
                                                                                      objParametri)


                                Next

                            End If

                            ' AGGIORNAMENTO DEGLI APPEZZAMENTI ELIMINATI
                            DrAppEliminati = Dt_Appezzamenti_Eliminati.Select("UNID_APP_NEW = '" & Dt_Appezzamenti.Rows(i).Item("UNID_APP") & "'")
                            If DrAppEliminati IsNot Nothing AndAlso DrAppEliminati.Length > 0 Then

                                For j = 0 To DrAppEliminati.Length - 1
                                    ' vengono direttamente modificati i campi del datatable, il datarow punta allo stessa area di memoria del dt
                                    DrAppEliminati(j).Item("Programmazione_Entita_Cod") = Dt_Appezzamenti.Rows(i).Item("Programmazione_Entita_Cod")

                                Next
                            End If

                        Next

                    End If

                    If Not IsNothing(Dt_Appezzamenti_Eliminati) Then

                        For i = 0 To Dt_Appezzamenti_Eliminati.Rows.Count - 1

                            'metto in un try xkè quando fraziono un appezzamento proveniente dal reale
                            'da errore di inserimento chiave duplicata
                            Try

                                BoolDummy = objProgrammazione_Entita_Eliminate_W.Scrivi(
                                                              Piva,
                                                              Dt_Appezzamenti_Eliminati.Rows(i).Item("Sa_Cod"),
                                                              Dt_Appezzamenti_Eliminati.Rows(i).Item("Appezza"),
                                                              Dt_Appezzamenti_Eliminati.Rows(i).Item("ID_Reg"),
                                                              Dt_Appezzamenti_Eliminati.Rows(i).Item("Campo_Cod"),
                                                              Programmazione_Cod,
                                                              Dt_Appezzamenti_Eliminati.Rows(i).Item("Programmazione_Entita_Cod"),
                                                              Dt_Appezzamenti_Eliminati.Rows(i).Item("Progetto_Cod"),
                                                              Dt_Appezzamenti_Eliminati.Rows(i).Item("Operazione_Cod"),
                                                              Estremo_Validita_Inizio,
                                                              Estremo_Validita_Fine,
                                                              objParametri)

                            Catch ex As Exception
                                MessaggioErrore = ex.Message
                            End Try
                        Next

                    End If

            End Select

            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)

        Catch ex As Exception

            ''Faccio il rollback della transazione
            'If Not objParametri.objTransazione Is Nothing Then
            '    ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            'End If
            ConnessioniTransazioni.RollBackTransazione(objParametri)

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
            '    Return Codice

        Finally
            'Chiudo la connessione se è stata aperta in questa routine
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)
        End Try

        Return Codice

    End Function

    Private Shared Sub ScriviAllegatoEdEntitaAllegatoPlann(ByVal Programmazione_Des As String, ByVal Piva As String, ByVal Dt_Appezzamenti As DataTable, ByVal NumeroDiValidazione As String, ByVal DataDiValidazione As Date, ByRef objParametri As AgronicaCoreParametri, ByVal Codice As Integer, ByVal i As Integer, ByVal objFascicoloEntita As AgronicaCoreAnagrafeDAL.Allegati_EntitaxDocumenti_W, ByRef allegati_Documenti_cod As Integer)
        Dim allegatiDocumentiScrivi As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_W
        Dim allegatiDocumentiLeggi As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_R
        Dim DT = allegatiDocumentiLeggi.Leggi(0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri, Piva, NumeroDiValidazione)
        If DT.Rows.Count = 0 Then
            allegatiDocumentiScrivi.Scrivi(
            Piva,
            Programmazione_Des,
            9,
            "",
            NumeroDiValidazione,
            0,
            "",
            AGRODATAINIZIO,
            AGRODATAFINE,
            allegati_Documenti_cod,
            objParametri,
            Validazione_Data:=DataDiValidazione
        )
        Else
            allegatiDocumentiScrivi.Modifica_Programmazione_Des_Dato_NumeroValidazione(NumeroDiValidazione, Piva, Programmazione_Des, objParametri)
            allegati_Documenti_cod = allegatiDocumentiLeggi.Leggi(0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri, Piva, NumeroDiValidazione).Rows(0).Item("Allegati_Documenti_Cod")
        End If

        objFascicoloEntita.Scrivi(allegati_Documenti_cod,
                0,
                Piva,
                0,
                0,
                0, 0,
                0,
                "", "", "", 0, 0, "", "", "",
                0, Codice, 0,
                Dt_Appezzamenti.Rows(i).Item("Validita_Inizio"),
                Dt_Appezzamenti.Rows(i).Item("Validita_Fine"),
                objParametri)

    End Sub

    Public Function Pianificazione_Copia(ByVal Programmazione_Cod_DaCopiare As Integer,
                                                ByRef objParametri As AgronicaCoreParametri,
                                          Optional ByVal Piva As String = "",
                                          Optional ByVal Programmazione_Cod As Integer = 0,
                                          Optional ByVal Programmazione_Des As String = "",
                                          Optional ByVal Programmazione_Des_Long As String = "",
                                          Optional ByVal Note As String = "",
                                          Optional ByVal Validita_Inizio As Date = #1/1/1900#,
                                          Optional ByVal Validita_Fine As Date = #12/31/2100#,
                                         Optional ByVal CopiaDateFineEntita As Boolean = False) As Integer

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Programmazione_W.Pianificazione_Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim ErrMSG As String = ""

        Dim Codice As Integer = 0
        Dim RecordInteressati As Integer = 0
        Dim Programmazione_Entita_Cod As Integer = 0
        Dim i, j As Integer
        Dim Dr As DataRow()
        Dim Programmazione_Cod_DaCopiare_App As Integer
        Dim BoolDummy As Boolean
        Dim MetodoProduzione_Cod As Integer

        Dim Dt_Testata_R As New DataTable
        Dim Dt_Entita_R As New DataTable
        Dim Dt_Entita_Eliminate_R As New DataTable
        Dim Dt_Particelle_R As New DataTable

        Dim Dt_ParticelleCondotte_R As New DataTable
        Dim DrCondotte As DataRow()

        Dim Tipo_Pianificazione As Integer = 0

        Dim Fonte_Cod As Integer = 0

        Dim InserisciEntita As Boolean
        Dim Appezza As Integer = 0
        Dim Campo_Cod As Integer = 0
        Dim Area_Cod As Integer = 0

        Dim strFiltro As String
        Dim Anno As Integer = Validita_Inizio.Year

        Programmazione_Cod_DaCopiare_App = Programmazione_Cod_DaCopiare

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try

            '------------------------------
            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                           FlagTransazioneLocale,
                                                           objParametri)

            '----------------------------------------------
            'leggo i dati della programmazione da copiare
            Dim objProgrammazione_Testata As New AgronicaCoreAnagrafeDAL.Programmazione_Testata_R

            Dt_Testata_R = objProgrammazione_Testata.Leggi(
                                       ErrMSG,
                                       Programmazione_Cod_DaCopiare,
                                       Piva,
                                       "",
                                       1,
                                       Estremo_Validita_Inizio,
                                       Estremo_Validita_Fine,
                                       enum_TipoRicetta.Non_Filtrare,
                                       enumSelezioneVariabile.Selezione_TabellaCompleta,
                                       "", "",
                                       objParametri)

            Programmazione_Cod_DaCopiare = Programmazione_Cod_DaCopiare_App

            'leggo le particelle condotte alle date nuove
            'potrebbero esserci particelle non piu in conduzione
            '(in tal caso non devo copiare nè le particelle nè le entità!!)
            strFiltro = " AND ImpreseXParticelle.Sup_Condotta<> 0 "
            Dim objParticelle As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_R
            Dt_ParticelleCondotte_R = objParticelle.Anagrafica_Particelle_Leggi(Piva,
                                                                         0,
                                                                         Validita_Inizio,
                                                                         Validita_Fine,
                                                                         objParametri,
                                                                         strFiltro)

            If Dt_Testata_R IsNot Nothing AndAlso Dt_Testata_R.Rows.Count > 0 Then

                If Programmazione_Des = "" Then
                    Programmazione_Des = Dt_Testata_R.Rows(0).Item("Programmazione_Des")
                End If
                If Programmazione_Des_Long = "" Then
                    Programmazione_Des_Long = Dt_Testata_R.Rows(0).Item("Programmazione_Des_Long")
                End If

                If Piva = "" Then
                    Piva = Dt_Testata_R.Rows(0).Item("Piva")
                End If
                If Note = "" Then
                    Note = Dt_Testata_R.Rows(0).Item("Note")
                End If

                If Validita_Inizio = #1/1/1900# Then
                    Validita_Inizio = Dt_Testata_R.Rows(0).Item("Validita_Inizio")
                End If
                If Validita_Fine = #12/31/2100# Then
                    Validita_Fine = Dt_Testata_R.Rows(0).Item("Validita_Fine")
                End If

                Tipo_Pianificazione = Dt_Testata_R.Rows(0).Item("Tipo_Pianificazione")

                Fonte_Cod = Dt_Testata_R.Rows(0).Item("Tipo_Pianificazione")

                If Not IsDBNull(Dt_Testata_R.Rows(0).Item("Fonte_Cod")) Then
                    Fonte_Cod = Dt_Testata_R.Rows(0).Item("Fonte_Cod")
                End If

                'CREO LA NUOVA TESTATA

                Dim objProgrammazione_Testata_W As New AgronicaCoreAnagrafeDAL.Programmazione_Testata_W

                BoolDummy = objProgrammazione_Testata_W.Scrivi(Codice,
                                                                Programmazione_Des,
                                                                Programmazione_Des_Long,
                                                                Piva,
                                                                Note,
                                                                Tipo_Pianificazione,
                                                                Fonte_Cod,
                                                                Validita_Inizio,
                                                                Validita_Fine,
                                                                objParametri)

                '----------------------------------------------------
                'Leggo le entità della programmazione da copiare

                Dim objProgrammazione_Entita_R As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_R

                Dt_Entita_R = objProgrammazione_Entita_R.Programmazione_Entita_Leggi_conAreeOmogenee(
                                            Programmazione_Cod_DaCopiare,
                                            ErrMSG,
                                            0,
                                            "",
                                            "",
                                            0,
                                            0,
                                            0,
                                            0,
                                            0,
                                            Estremo_Validita_Inizio,
                                            Estremo_Validita_Fine,
                                            2,
                                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                                            "",
                                            "",
                                            objParametri)

                '----------------------------------
                'leggo le entità da cancellare

                Dim objProgrammazione_Entita_Eliminate_R As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_Eliminate_R

                Dt_Entita_Eliminate_R = objProgrammazione_Entita_Eliminate_R.Leggi(
                                            Programmazione_Cod_DaCopiare,
                                            ErrMSG,
                                            0,
                                            Estremo_Validita_Inizio,
                                            Estremo_Validita_Fine,
                                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                                            "",
                                            "",
                                            objParametri)

                '----------------------------------
                'leggo le particelle associate alle varie entità
                Dim objPP As New AgronicaCoreAnagrafeDAL.Programmazione_Particelle_R
                Dt_Particelle_R = objPP.Programmazione_Particelle_Leggi_2(objParametri,
                                                                         Programmazione_Cod_DaCopiare,
                                                                         ErrMSG,
)

                If Dt_Entita_R IsNot Nothing AndAlso Dt_Entita_R.Rows.Count > 0 Then

                    For i = 0 To Dt_Entita_R.Rows.Count - 1

                        InserisciEntita = False

                        '----------------------------------------
                        'verifico se l'ENTITA' è associata ad una o piu PARTICELLE
                        'se SI --> verifico che la particella sia ancora in CONDUZIONE all'azienda
                        '          copio l'entità solo in questo caso
                        'se NO --> non copio l'entità
                        Dr = Dt_Particelle_R.Select("Programmazione_Entita_Cod=" & Dt_Entita_R.Rows(i).Item("Programmazione_Entita_Cod"))

                        If Dr IsNot Nothing AndAlso Dr.Length > 0 Then

                            For j = 0 To Dr.Length - 1

                                DrCondotte = Dt_ParticelleCondotte_R.Select("Prov='" & Dr(j).Item("prov") & "'" &
                                                                            " AND Com='" & Dr(j).Item("com") & "'" &
                                                                            " AND sezione='" & Dr(j).Item("sezione") & "'" &
                                                                            " AND foglio=" & Dr(j).Item("foglio") & "" &
                                                                            " AND numero=" & Dr(j).Item("numero") & "" &
                                                                            " AND subalterno='" & Dr(j).Item("subalterno") & "'")

                                If DrCondotte IsNot Nothing AndAlso DrCondotte.Length > 0 Then
                                    InserisciEntita = True
                                Else
                                    InserisciEntita = False
                                    Exit For
                                End If

                            Next
                        Else
                            InserisciEntita = True
                        End If

                        If InserisciEntita Then

                            Dim objProgrammazione_Entita_W As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_W

                            Dim Validita_Inizio_Entita As Date = Validita_Inizio
                            Dim Validita_Fine_Entita As Date = Validita_Fine

                            Programmazione_Entita_Cod = 0

                            Select Case Tipo_Pianificazione

                                Case enum_TipoPianificazione.Pianificazione_Annuale,
                                     enum_TipoPianificazione.Pianificazione_DaNotificaBio

                                    ''lascio l'anno d'impianto
                                    'Validita_Inizio = Dt_Entita_R.Rows(i).Item("validita_inizio")
                                    'strValidita_Inizio = CDate(Validita_Inizio).ToShortDateString()

                                    '(08/11/2018 fede) introdotto il parametro CopiaDateEntita
                                    'come da richiesta di campisi x copia pua (vuole mantenere le date fine della pianificazione)
                                    Validita_Inizio_Entita = Dt_Entita_R.Rows(i).Item("validita_inizio")

                                    If CopiaDateFineEntita Then
                                        Validita_Fine_Entita = Dt_Entita_R.Rows(i).Item("Validita_Fine")
                                    End If

                                Case enum_TipoPianificazione.Pianificazione_Quindicinale

                                    'devo x forza far combinare le date delle quindicine
                                    Dim strValidita_Inizio As String
                                    Validita_Inizio_Entita = Dt_Entita_R.Rows(i).Item("validita_inizio")
                                    strValidita_Inizio = CDate(Validita_Inizio_Entita).ToShortDateString()
                                    strValidita_Inizio = Left(strValidita_Inizio, 6) & Anno.ToString

                            End Select

                            MetodoProduzione_Cod = enum_MetodoProduzione.Integrato

                            Dim Veg_Cod_Agea = Dt_Entita_R.Rows(i).Item("Veg_Cod_Agea")
                            If IsDBNull(Veg_Cod_Agea) Then
                                Veg_Cod_Agea = ""
                            End If

                            Dim Cul_Cod_Agea = Dt_Entita_R.Rows(i).Item("Cul_Cod_Agea")
                            If IsDBNull(Cul_Cod_Agea) Then
                                Cul_Cod_Agea = ""
                            End If

                            Dim Uso_Cod_Agea = Dt_Entita_R.Rows(i).Item("Uso_Cod_Agea")
                            If IsDBNull(Uso_Cod_Agea) Then
                                Uso_Cod_Agea = ""
                            End If

                            Dim Occupazione_Cod_Agea = Dt_Entita_R.Rows(i).Item("Occupazione_Cod_Agea")
                            If IsDBNull(Occupazione_Cod_Agea) Then
                                Occupazione_Cod_Agea = ""
                            End If

                            Dim Destinazione_Cod_Agea = Dt_Entita_R.Rows(i).Item("Destinazione_Cod_Agea")
                            If IsDBNull(Destinazione_Cod_Agea) Then
                                Destinazione_Cod_Agea = ""
                            End If

                            Dim Qualita_Cod_Agea = Dt_Entita_R.Rows(i).Item("Qualita_Cod_Agea")
                            If IsDBNull(Qualita_Cod_Agea) Then
                                Qualita_Cod_Agea = ""
                            End If

                            'DRUDI 2017/10/04 Inserite codifiche agea
                            BoolDummy = objProgrammazione_Entita_W.Scrivi(Codice,
                                              Programmazione_Entita_Cod,
                                              Dt_Entita_R.Rows(i).Item("Entita_Des").ToString,
                                              Piva,
                                              Dt_Entita_R.Rows(i).Item("Sa_Cod"),
                                              Dt_Entita_R.Rows(i).Item("Campo_Cod"),
                                              Dt_Entita_R.Rows(i).Item("Appezza"),
                                              Dt_Entita_R.Rows(i).Item("ID_Reg"),
                                              Dt_Entita_R.Rows(i).Item("Progetto_Cod"),
                                              Dt_Entita_R.Rows(i).Item("Progetto_Des"),
                                              Dt_Entita_R.Rows(i).Item("Id_Cod"),
                                              Dt_Entita_R.Rows(i).Item("Veg_Cod"),
                                              Dt_Entita_R.Rows(i).Item("Cul_Cod"),
                                              Dt_Entita_R.Rows(i).Item("Grva_Cod"),
                                              Dt_Entita_R.Rows(i).Item("Grfi_Cod"),
                                              Dt_Entita_R.Rows(i).Item("Cop_Cod"),
                                              Dt_Entita_R.Rows(i).Item("Superficie"),
                                              Dt_Entita_R.Rows(i).Item("Resa"),
                                              Dt_Entita_R.Rows(i).Item("TipoZona"),
                                              0,
                                              0,
                                              0,
                                              0,
                                              Dt_Entita_R.Rows(i).Item("Num_Piante"),
                                              Dt_Entita_R.Rows(i).Item("Tra_Fila"),
                                              Dt_Entita_R.Rows(i).Item("Su_Fila"),
                                              Dt_Entita_R.Rows(i).Item("Foral_Cod"),
                                              Dt_Entita_R.Rows(i).Item("Port_Cod"),
                                              Dt_Entita_R.Rows(i).Item("Imp_Cod"),
                                              Dt_Entita_R.Rows(i).Item("Regolamento_Cod"),
                                              0,
                                              Dt_Entita_R.Rows(i).Item("Stato_Cod"),
                                              Dt_Entita_R.Rows(i).Item("Ciclo"),
                                              Dt_Entita_R.Rows(i).Item("Data_Semina"),
                                              Dt_Entita_R.Rows(i).Item("Data_Raccolta"),
                                              Dt_Entita_R.Rows(i).Item("Note"),
                                              Dt_Entita_R.Rows(i).Item("Veg_Cod_Cliente"),
                                              Dt_Entita_R.Rows(i).Item("Cul_Cod_Cliente"),
                                              MetodoProduzione_Cod,
                                              Validita_Inizio_Entita,
                                              Validita_Fine_Entita,
                                              objParametri,
                                                Veg_Cod_Agea:=Veg_Cod_Agea,
                                                Cul_Cod_Agea:=Cul_Cod_Agea,
                                                Uso_Cod_Agea:=Uso_Cod_Agea,
                                                Occupazione_Cod_Agea:=Occupazione_Cod_Agea,
                                                Destinazione_Cod_Agea:=Destinazione_Cod_Agea,
                                                Qualita_Cod_Agea:=Qualita_Cod_Agea)

                            '------------------------------------------
                            'Inserisco le Particelle legate all'entità

                            If Dr IsNot Nothing AndAlso Dr.Length > 0 Then

                                Dim objProgrammazione_Particelle_W As New AgronicaCoreAnagrafeDAL.Programmazione_Particelle_W

                                For j = 0 To Dr.Length - 1

                                    BoolDummy = objProgrammazione_Particelle_W.Scrivi(Programmazione_Entita_Cod,
                                                                                      Dr(j).Item("Prov").ToString,
                                                                                      Dr(j).Item("Com").ToString,
                                                                                      Dr(j).Item("Sezione").ToString,
                                                                                      Dr(j).Item("Foglio"),
                                                                                      Dr(j).Item("Numero"),
                                                                                      Dr(j).Item("Subalterno").ToString,
                                                                                      Dr(j).Item("Superficie"),
                                                                                      Validita_Inizio_Entita,
                                                                                      Validita_Fine_Entita,
                                                                                      objParametri)

                                Next

                            End If

                            '------------------------------------------------------
                            'Inserisco le eventuali Aree Omogenee legate all'entità

                            Area_Cod = Dt_Entita_R.Rows(i).Item("Area_Cod")

                            If Area_Cod <> 0 Then

                                Dim objAree_W As New AgronicaCoreAnagrafeDAL.Area_OmogeneaxEntita_W

                                objAree_W.Scrivi(Piva,
                                                 Area_Cod,
                                                 Programmazione_Entita_Cod,
                                                 Validita_Inizio_Entita,
                                                 Validita_Fine_Entita,
                                                 objParametri)

                            End If


                        End If


                    Next

                End If


                If Dt_Entita_Eliminate_R IsNot Nothing AndAlso Dt_Entita_Eliminate_R.Rows.Count > 0 Then

                    Dim objProgrammazione_Entita_Eliminate_W As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_Eliminate_W

                    For i = 0 To Dt_Entita_Eliminate_R.Rows.Count - 1

                        'metto in un try xkè quando fraziono un appezzamento proveniente dal reale
                        'da errore di inserimento chiave duplicata
                        Try

                            BoolDummy = objProgrammazione_Entita_Eliminate_W.Scrivi(
                                                          Piva,
                                                          Dt_Entita_Eliminate_R.Rows(i).Item("Sa_Cod"),
                                                          Dt_Entita_Eliminate_R.Rows(i).Item("Appezza"),
                                                          Dt_Entita_Eliminate_R.Rows(i).Item("ID_Reg"),
                                                          Dt_Entita_Eliminate_R.Rows(i).Item("Campo_Cod"),
                                                          Codice,
                                                          Dt_Entita_Eliminate_R.Rows(i).Item("Programmazione_Entita_Cod"),
                                                          Dt_Entita_Eliminate_R.Rows(i).Item("Progetto_Cod"),
                                                          Dt_Entita_Eliminate_R.Rows(i).Item("Operazione_Cod"),
                                                          Dt_Entita_Eliminate_R.Rows(i).Item("Validita_Inizio"),
                                                          Dt_Entita_Eliminate_R.Rows(i).Item("Validita_Fine"),
                                                          objParametri)

                        Catch ex As Exception

                        End Try
                    Next

                End If

            End If

            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)

        Catch ex As Exception

            'Faccio il rollback della transazione
            If objParametri.objTransazione IsNot Nothing Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
            Return Codice

        Finally

            'Chiudo la connessione se è stata aperta in questa routine
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)

        End Try

        Return Codice

    End Function

    Public Function Pianificazione_CopiaEF(ByVal Programmazione_Cod_DaCopiare As Integer,
                                           ByVal Data_Inizio As Date,
                                           ByVal Data_Fine As Date,
                                           ByVal Programmazione_Des As String,
                                           ByVal Programmazione_Des_Long As String,
                                           ByRef objParametri As AgronicaCoreParametri,
                                           ByVal CopiaDateFineEntita As Boolean,
                                           ByRef MsgRitorno As JObject) As Integer

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Programmazione_W.Pianificazione_CopiaEF()"

        Dim MessaggioErrore As String = ""
        Dim ErrMSG As String = ""

        Dim objSequenze = New Agro_Sequenze

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim transactionOptions As New TransactionOptions()
        transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
        transactionOptions.Timeout = TransactionManager.MaximumTimeout

        Dim data = DateTime.Now
        Dim username = objParametri.UsernameOperazione

        Dim Codice = 0
        Try

            Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    Dim programmazione_testata = (From pt In GiasContext.Programmazione_Testata Where pt.Programmazione_Cod = Programmazione_Cod_DaCopiare).FirstOrDefault

                    If programmazione_testata IsNot Nothing Then

                        'Richiedo un nuovo id sequenza
                        Dim idSeq As Integer = objSequenze.NuovoId_Tabella("Programmazione_Testata", 0, 2000000000, objParametri, UtilizzaTransazione:=False)
                        Codice = idSeq

                        Dim programmazione_testata_C = Gias_EF_Utility.CopyEntity(GiasContext, programmazione_testata, Nothing, username, data)

                        programmazione_testata_C.Programmazione_Cod = idSeq
                        programmazione_testata_C.Programmazione_Des = Programmazione_Des
                        programmazione_testata_C.Programmazione_Des_Long = Programmazione_Des_Long
                        programmazione_testata_C.Pratica_Cod = ""
                        programmazione_testata_C.Validita_Inizio = Data_Inizio
                        programmazione_testata_C.Validita_Fine = Data_Fine

                        GiasContext.Programmazione_Testata.Add(programmazione_testata_C)


                        'leggo le particelle condotte alle date nuove
                        'potrebbero esserci particelle non piu in conduzione
                        '(in tal caso non devo copiare nè le particelle nè le entità!!)
                        Dim strFiltro = " AND ImpreseXParticelle.Sup_Condotta<> 0 "
                        Dim objParticelle_R As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_R
                        Dim Dt_ParticelleCondotte_R = objParticelle_R.Anagrafica_Particelle_Leggi(programmazione_testata_C.Piva,
                                                                         0,
                                                                         Data_Inizio,
                                                                         Data_Fine,
                                                                         objParametri,
                                                                         strFiltro)

                        Dim listEntitaAdd As New List(Of Programmazione_Entita)
                        Dim listEntita_CodiciAdd As New List(Of Programmazione_Entita_Codici)
                        Dim listEntita_ParticelleAdd As New List(Of Programmazione_Particelle)
                        Dim listEntitaCancellateAdd As New List(Of Programmazione_Entita_Eliminate)

                        Dim listEntita = (From pe In GiasContext.Programmazione_Entita Where pe.Programmazione_Cod = Programmazione_Cod_DaCopiare).ToList

                        Dim msgEntita As New JArray()

                        For Each entita In listEntita
                            Dim inserisciEntita = True
                            Dim objEntita As New JObject()
                            Dim objParticelle As New JArray()
                            'Richiedo un nuovo id sequenza
                            Dim idSeqEntita As Integer = objSequenze.NuovoId_Tabella("Programmazione_Entita", 0, 2000000000, objParametri, UtilizzaTransazione:=False)

                            Dim listParticelleEntita = (From pep In GiasContext.Programmazione_Particelle Where pep.Programmazione_Entita_Cod = entita.Programmazione_Entita_Cod)
                            For Each entita_particella In listParticelleEntita

                                Dim entita_particella_C = Gias_EF_Utility.CopyEntity(GiasContext, entita_particella, Nothing, username, data)

                                Dim DrCondotte = Dt_ParticelleCondotte_R.Select("Prov='" & entita_particella_C.Prov & "'" &
                                                                            " AND Com='" & entita_particella_C.Com & "'" &
                                                                            " AND sezione='" & entita_particella_C.Sezione & "'" &
                                                                            " AND foglio=" & entita_particella_C.Foglio & "" &
                                                                            " AND numero=" & entita_particella_C.Numero & "" &
                                                                            " AND subalterno='" & entita_particella_C.Subalterno & "'")

                                If DrCondotte IsNot Nothing AndAlso DrCondotte.Length > 0 Then
                                    If inserisciEntita Then
                                        inserisciEntita = True
                                    End If
                                Else
                                    Dim objParticella As New JObject()
                                    objParticella("Prov") = CStr(entita_particella_C.Prov)
                                    objParticella("Com") = CStr(entita_particella_C.Com)
                                    objParticella("Sezione") = CStr(entita_particella_C.Sezione)
                                    objParticella("Foglio") = CStr(entita_particella_C.Foglio)
                                    objParticella("Numero") = CStr(entita_particella_C.Numero)
                                    objParticella("Subalterno") = CStr(entita_particella_C.Subalterno)
                                    objParticelle.Add(objParticella)
                                    inserisciEntita = False
                                End If

                                If inserisciEntita Then
                                    entita_particella_C.Programmazione_Entita_Cod = idSeqEntita

                                    listEntita_ParticelleAdd.Add(entita_particella_C)
                                End If

                            Next

                            If Not inserisciEntita Then
                                objEntita("Nome") = CStr(entita.Entita_Des)
                                objEntita("Particelle") = objParticelle
                                msgEntita.Add(objEntita)
                                Continue For
                            End If

                            Dim listEntitaCodici = (From pec In GiasContext.Programmazione_Entita_Codici Where pec.Programmazione_Entita_Cod = entita.Programmazione_Entita_Cod AndAlso pec.id_cod <> enum_CodiciAnagrafe.Programmazione_Impianto_Pratica_Cod)
                            For Each entita_codice In listEntitaCodici

                                Dim entita_codice_C = Gias_EF_Utility.CopyEntity(GiasContext, entita_codice, Nothing, username, data)

                                entita_codice_C.Programmazione_Entita_Cod = idSeqEntita

                                listEntita_CodiciAdd.Add(entita_codice_C)

                            Next


                            Dim entita_C = Gias_EF_Utility.CopyEntity(GiasContext, entita, Nothing, username, data)

                            Dim Initial_Data_inizio = entita_C.Validita_Inizio

                            entita_C.Programmazione_Cod = programmazione_testata_C.Programmazione_Cod
                            entita_C.Programmazione_Entita_Cod = idSeqEntita
                            entita_C.Stato_Ribaltamento = 0
                            entita_C.Validita_Inizio = Data_Inizio
                            entita_C.Validita_Fine = Data_Fine
                            If entita_C.Validita_Inizio_Impianto = Initial_Data_inizio Then
                                entita_C.Validita_Inizio_Impianto = Data_Inizio
                            End If

                            listEntitaAdd.Add(entita_C)

                        Next

                        GiasContext.Programmazione_Entita.AddRange(listEntitaAdd)
                        GiasContext.Programmazione_Entita_Codici.AddRange(listEntita_CodiciAdd)
                        GiasContext.Programmazione_Particelle.AddRange(listEntita_ParticelleAdd)
                        'GiasContext.Programmazione_Entita_Eliminate.AddRange(listEntitaCancellateAdd)

                        GiasContext.SaveChanges()

                        ' COMMIT Effettivo
                        scope.Complete()

                        MsgRitorno("Esito") = CBool(True)
                        MsgRitorno("msg") = CStr("Piano colturale copiato")
                        MsgRitorno("Entita") = msgEntita

                    End If

                End Using

            End Using

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)

            MsgRitorno("Esito") = CBool(False)
            MsgRitorno("msg") = CStr(ex.Message)

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return Codice

    End Function

    Public Function Pianificazione_Modifica_Entita_Da_Json(ByVal Programmazione_Cod As Integer,
                                                          ByVal Piva As String,
                                                              ByVal AppezzamentiInseriti As String,
                                                              ByVal AppezzamentiModificati As String,
                                                              ByVal AppezzamentiCancellati As String,
                                                                    ByRef objParametri As AgronicaCoreParametri
                                                              ) As Integer

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Programmazione_W.Pianificazione_Modifica_Entita_Da_Json()"

        Dim MessaggioErrore As String = ""
        Dim ErrMSG As String = ""

        Dim righeInseriteArray As JArray
        Dim righeModificateArray As JArray


        ' MODIFICA SINGOLE ENTITA PLANNING

        Dim objProgrammazione_Particelle_W As New AgronicaCoreAnagrafeDAL.Programmazione_Particelle_W
        Dim objProgrammazione_Entita_W As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_W

        Dim BoolDummy As Boolean

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False


        Dim strCatasto As String = ""

        Dim N_Appezza = -1

        Dim Veg_Cod As Integer = 0
        Dim Cul_Cod As Integer = 0
        Dim Id_Cod As Integer = 0
        Dim Grfi_Cod As Integer = 0
        Dim Grva_Cod As Integer = 0

        Try


            '------------------------------
            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                           FlagTransazioneLocale,
                                                           objParametri)


            ' MODIFICA SINGOLE ENTITA PLANNING

            If AppezzamentiModificati <> "" Then

                righeModificateArray = JArray.Parse(AppezzamentiModificati)

                For Each r In righeModificateArray

                    If Not IsNothing(r("programmazione_entita_cod")) AndAlso r("programmazione_entita_cod") <> "0" AndAlso r("programmazione_entita_cod") <> "" Then

                        'ELIMINO i record in Programmazione_Particelle
                        'ELIMINO i record in Programmazione_Entita

                        BoolDummy = objProgrammazione_Particelle_W.Cancella("",
                                                            r("programmazione_entita_cod"),
                                                              "",
                                                              "",
                                                              "0",
                                                              0,
                                                              0,
                                                              "0",
                                                             "",
                                                             objParametri)

                        BoolDummy = objProgrammazione_Entita_W.Cancella(Programmazione_Cod,
                                                                    r("programmazione_entita_cod"),
                                                                    True,
                                                                    False,
                                                                    0,
                                                                    "",
                                                                    objParametri)

                        'REINSERISCO L'ENTITA
                        'DRUDI 04/10/2017 inserisco codifiche agea
                        BoolDummy = objProgrammazione_Entita_W.Scrivi(Programmazione_Cod,
                                                                       r("programmazione_entita_cod"),
                                                                       r("app_nome"),
                                                                         Piva, r("sa_cod"), 0, r("appezza"), 0, 0,
                                                                         "",
                                                                         r("Id_Cod"), r("Veg_Cod"), r("Cul_Cod"), r("Grva_Cod"), r("Grfi_Cod"),
                                                                         0,
                                                                         r("utilizzo_sup"),
                                                                         0,
                                                                         "",
                                                                         0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 102, 0,
                                                                         AGRODATAINIZIO, AGRODATAFINE,
                                                                         "",
                                                                         r("Veg_Cod_Agea"), r("Cul_Cod_Agea"),
                                                                         0,
                                                                         r("validita_inizio"), r("validita_fine"),
                                                                         objParametri,
                                                                         Cod_Macrouso:=r("Macrouso_Cod"),
                                                                         Validita_Inizio_Impianto:=r("validita_inizio"),
                                                                         Veg_Cod_Agea:=r("Veg_Cod_Agea"),
                                                                         Cul_Cod_Agea:=r("Cul_Cod_Agea"),
                                                                         Uso_Cod_Agea:=r("Uso_Cod_Agea"),
                                                                         Occupazione_Cod_Agea:=r("Occupazione_Cod_Agea"),
                                                                         Destinazione_Cod_Agea:=r("Destinazione_Cod_Agea"),
                                                                         Qualita_Cod_Agea:=r("Qualita_Cod_Agea"))

                        If CInt(r("appezza")) < N_Appezza Then
                            N_Appezza = CInt(r("appezza")) - 1
                        End If

                        strCatasto = r("catasto_key")

                        Dim ParticelleArray As String() = Split(strCatasto, "<br>")
                        Dim ParticellaArray As String()
                        If ParticelleArray IsNot Nothing Then
                            For i = 0 To ParticelleArray.Length - 1
                                ParticellaArray = Split(ParticelleArray(i), "_")

                                BoolDummy = objProgrammazione_Particelle_W.Scrivi(r("programmazione_entita_cod"),
                                                                                  ParticellaArray(0),
                                                                                  ParticellaArray(1),
                                                                                  If(ParticellaArray(2) = "", "0", ParticellaArray(2)),
                                                                                  ParticellaArray(3),
                                                                                  ParticellaArray(4),
                                                                                  If(ParticellaArray(5) = "", "0", ParticellaArray(5)),
                                                                                  ParticellaArray(11),
                                                                                  r("validita_inizio"), r("validita_fine"),
                                                                                  objParametri)

                            Next

                        End If


                    End If

                Next

            End If



            ' INSERIMENTO SINGOLE ENTITA PLANNING

            If AppezzamentiInseriti <> "" Then

                righeInseriteArray = JArray.Parse(AppezzamentiInseriti)

                For Each r In righeInseriteArray

                    If Not (IsNumeric(r("Id_Cod")) AndAlso IsNumeric(r("Veg_Cod"))) Then

                        Dim objUtilizzi As New AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Agea_R

                        Veg_Cod = 0
                        Cul_Cod = 0
                        Grfi_Cod = 0
                        Id_Cod = 0
                        Grva_Cod = 0

                        Dim LogCodificheMancantiSpecie As String = ""
                        Dim LogCodificheMancantiVarieta As String = ""

                        objUtilizzi.Specie_e_Varieta_Gias_Da_Agea(
                                                                    LogCodificheMancantiSpecie,
                                                                    LogCodificheMancantiVarieta,
                                                                    r("Veg_Cod_Agea"), r("Cul_Cod_Agea"),
                                                                    Veg_Cod, Cul_Cod, Grfi_Cod, Grva_Cod, Id_Cod,
                                                                    "", "",
                                                                    "",
                                                                    "",
                                                                    r("validita_inizio"),
                                                                    r("Uso_Cod_Agea"), r("Occupazione_Cod_Agea"), r("Destinazione_Cod_Agea"), r("Qualita_Cod_Agea"),
                                                                    objParametri)
                    End If
                    'INSERISCO L'ENTITA
                    'DRUDI 2017/10/04 Inserite codifiche agea
                    BoolDummy = objProgrammazione_Entita_W.Scrivi(Programmazione_Cod,
                                                                  0,
                                                                  r("app_nome"),
                                                                  Piva, r("sa_cod"), 0, N_Appezza, 0, 0,
                                                                     "",
                                                                     Id_Cod, Veg_Cod, Cul_Cod, Grva_Cod, Grfi_Cod,
                                                                     0,
                                                                     r("utilizzo_sup"),
                                                                     0,
                                                                     "",
                                                                     0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 102, 0,
                                                                     AGRODATAINIZIO, AGRODATAFINE,
                                                                     "",
                                                                    r("Veg_Cod_Agea"), r("Cul_Cod_Agea"),
                                                                     0,
                                                                    r("validita_inizio"), r("validita_fine"),
                                                                     objParametri,
                                                                     Cod_Macrouso:=r("Macrouso_Cod"),
                                                                     Validita_Inizio_Impianto:=r("validita_inizio"),
                                                                        Veg_Cod_Agea:=r("Veg_Cod_Agea"),
                                                                         Cul_Cod_Agea:=r("Cul_Cod_Agea"),
                                                                         Uso_Cod_Agea:=r("Uso_Cod_Agea"),
                                                                         Occupazione_Cod_Agea:=r("Occupazione_Cod_Agea"),
                                                                         Destinazione_Cod_Agea:=r("Destinazione_Cod_Agea"),
                                                                         Qualita_Cod_Agea:=r("Qualita_Cod_Agea"))

                    N_Appezza = -1

                    If BoolDummy Then

                        strCatasto = r("catasto_key")

                        If strCatasto <> "" Then
                            Dim ParticelleArray As String() = Split(strCatasto, "<br>")
                            Dim ParticellaArray As String()
                            If ParticelleArray IsNot Nothing Then
                                For i = 0 To ParticelleArray.Length - 1
                                    ParticellaArray = Split(ParticelleArray(i), "_")

                                    BoolDummy = objProgrammazione_Particelle_W.Scrivi(r("programmazione_entita_cod"),
                                                                                          ParticellaArray(0),
                                                                                          ParticellaArray(1),
                                                                                            IIf(ParticellaArray(2) = "", "0", ParticellaArray(2)),
                                                                                            ParticellaArray(3),
                                                                                            ParticellaArray(4),
                                                                                            IIf(ParticellaArray(5) = "", "0", ParticellaArray(5)),
                                                                                          ParticellaArray(11),
                                                                                            r("validita_inizio"), r("validita_fine"),
                                                                                          objParametri)

                                Next

                            End If

                        End If

                    End If



                Next

            End If

            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)

        Catch ex As Exception

            ''Faccio il rollback della transazione
            'If Not objParametri.objTransazione Is Nothing Then
            '    ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            'End If
            ConnessioniTransazioni.RollBackTransazione(objParametri)

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        Finally
            'Chiudo la connessione se è stata aperta in questa routine
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)
        End Try

        Return Programmazione_Cod

    End Function

    'NON scrive tutta la PIANIFICAZIONE
    'ma SOLO i dati relativi ad UNA PARTICELLA!!
    'VARIE ENTITA + LA PARTICELLA AD ESSE ASSOCIATE
    Public Function PianificazioneParticella_Scrivi(ByVal Tipo_Operazione As Integer,
                                                    ByVal Programmazione_Cod As Integer,
                                                    ByVal Piva As String,
                                                    ByVal Prov As String,
                                                    ByVal Com As String,
                                                    ByVal Sezione As String,
                                                    ByVal Foglio As Integer,
                                                    ByVal Numero As Integer,
                                                    ByVal Subalterno As String,
                                                    ByVal Utente As String,
                                                    ByVal Validita_Inizio As Date,
                                                    ByVal Validita_Fine As Date,
                                                    ByVal Dt_Appezzamenti As DataTable,
                                                    ByVal Dt_Appezzamenti_Eliminati As DataTable,
                                                          ByRef objParametri As AgronicaCoreParametri
                                                    ) As Integer

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Programmazione_W.PianificazioneParticella_Scrivi()"

        Dim RecordInteressati As Integer = 0
        Dim Programmazione_Entita_Cod As Integer = 0
        Dim Campo_Cod As Integer = 0
        Dim i, j As Integer
        Dim MetodoProduzione_Cod As Integer
        Dim nCol As Integer

        Dim intRet As Integer = 0
        Dim BoolDummy As Boolean

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim ErrMSG As String = ""
        Dim MessaggioErrore As String



        Try

            '------------------------------
            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                           FlagTransazioneLocale,
                                                           objParametri)


            Select Case Tipo_Operazione

                '----------------------------------------------------------
                '----------------------------------------------------------
                '------    SCRITTURA    -----------------------------------
                '----------------------------------------------------------
                '----------------------------------------------------------

                Case enum_TipoOperazioneDB.Scrittura

                    Dim UltimoCampo = 0
                    Dim Key_Campo As Integer = 0
                    For i = 0 To Dt_Appezzamenti.Rows.Count - 1
                        Key_Campo = CInt(Dt_Appezzamenti.Rows(i).Item("Campo_Cod"))
                        If Key_Campo < UltimoCampo Then
                            UltimoCampo = Key_Campo
                        End If
                    Next
                    Key_Campo = UltimoCampo - 1
                    Dim CampoCod As Integer

                    Dim objProgrammazione_Entita_W As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_W
                    Dim objProgrammazione_Particelle_W As New AgronicaCoreAnagrafeDAL.Programmazione_Particelle_W

                    For i = 0 To Dt_Appezzamenti.Rows.Count - 1

                        If Dt_Appezzamenti.Rows(i).Item("Campo_Cod") = 0 Then
                            CampoCod = Key_Campo
                            Key_Campo -= 1
                        Else
                            CampoCod = Dt_Appezzamenti.Rows(i).Item("Campo_Cod")
                        End If

                        MetodoProduzione_Cod = enum_MetodoProduzione.Integrato

                        ' inserisco il record legato al campo
                        'DRUDI 2017/10/04 Inserite codifiche agea
                        BoolDummy = objProgrammazione_Entita_W.Scrivi(Programmazione_Cod,
                                              0,
                                              "",
                                              Piva,
                                              Dt_Appezzamenti.Rows(i).Item("Sa_Cod"),
                                              CampoCod,
                                              0,
                                              0,
                                              0,
                                              "",
                                              0,
                                              0,
                                              0,
                                              0,
                                              0,
                                              0,
                                              0,
                                              0,
                                              "n",
                                              0,
                                              0,
                                              0,
                                              0,
                                              0,
                                              0,
                                              0,
                                              0,
                                              0,
                                              0,
                                              1,
                                              0,
                                              102,
                                              0,
                                              #1/1/1900#,
                                              #1/1/1900#,
                                              Dt_Appezzamenti.Rows(i).Item("Note"),
                                              "000000",
                                              "000",
                                              MetodoProduzione_Cod,
                                              Validita_Inizio,
                                              Validita_Fine,
                                              objParametri)


                        ' inserisco la particella legata all'entità del campo
                        BoolDummy = objProgrammazione_Particelle_W.Scrivi(Programmazione_Entita_Cod,
                                                                            Prov,
                                                                            Com,
                                                                            Sezione,
                                                                            Foglio,
                                                                            Numero,
                                                                            Subalterno,
                                                                            Dt_Appezzamenti.Rows(i).Item("Sup_App_1").ToString,
                                                                            Validita_Inizio,
                                                                            Validita_Fine,
                                                                            objParametri)


                        '-------------------------
                        '-------------------------
                        ' SALVATAGGIO MODIFICHE
                        '-------------------------
                        '-------------------------
                        If Dt_Appezzamenti.Rows(i).Item("Mod") <> "" Then

                            Dim objModifica As New AgronicaCoreGestoreModificheBIZ.AnalizzaModifiche

                            Dim Nome_Tabella As String = DatiAnagrafici_PianoColturale

                            Dim objChiave As New AgronicaCoreGestoreModificheBIZ.ChiaveDT(objParametri.PivaSuperUser, Programmazione_Entita_Cod, Nome_Tabella)

                            Dim bRet As Boolean = True

                            'Preparo il dt x salvare le modifiche
                            objChiave.DT_Modifiche.Rows(0).Item(2) = "1"

                            bRet = objModifica.AggiornaStato(objChiave,
                                                      AGRODATAINIZIO,
                                                      AGRODATAFINE,
                                                      objParametri.objConnessione,
                                                      objParametri.objTransazione,
                                                      objParametri.StringaConnessione,
                                                      1,
                                                      objParametri.LogDirectory,
                                                      objParametri.LogFileName,
                                                      objParametri.UtenteCodFiscale,
                                                      ErrMSG,
                                                      objParametri)

                        End If

                        For nCol = 1 To 2


                            If (Not IsDBNull(Dt_Appezzamenti.Rows(i).Item("Veg_Cod_" & nCol)) AndAlso
                               (Dt_Appezzamenti.Rows(i).Item("Veg_Cod_" & nCol) <> "00000" And
                                Dt_Appezzamenti.Rows(i).Item("Veg_Cod_" & nCol) <> "0" And
                                Dt_Appezzamenti.Rows(i).Item("Veg_Cod_" & nCol) <> "")) Or
                                nCol = 1 Then

                                MetodoProduzione_Cod = enum_MetodoProduzione.Integrato

                                BoolDummy = objProgrammazione_Entita_W.Scrivi(Programmazione_Cod,
                                                      0,
                                                      "",
                                                      Piva,
                                                      Dt_Appezzamenti.Rows(i).Item("Sa_Cod"),
                                                      CampoCod,
                                                      Dt_Appezzamenti.Rows(i).Item("Appezza_" & nCol),
                                                      Dt_Appezzamenti.Rows(i).Item("Id_Reg_" & nCol),
                                                      0,
                                                      "",
                                                      0,
                                                      0,
                                                      0,
                                                      0,
                                                      0,
                                                      0,
                                                      Dt_Appezzamenti.Rows(i).Item("Sup_App_" & nCol),
                                                      0,
                                                      "n",
                                                      0,
                                                      0,
                                                      0,
                                                      0,
                                                      0,
                                                      0,
                                                      0,
                                                      0,
                                                      0,
                                                      0,
                                                      1,
                                                      0,
                                                      102,
                                                      0,
                                                      #1/1/1900#,
                                                      #1/1/1900#,
                                                      "",
                                                      Dt_Appezzamenti.Rows(i).Item("Veg_Cod_" & nCol),
                                                      Dt_Appezzamenti.Rows(i).Item("Cul_Cod_" & nCol),
                                                      MetodoProduzione_Cod,
                                                      Dt_Appezzamenti.Rows(i).Item("Validita_Inizio_" & nCol),
                                                      Dt_Appezzamenti.Rows(i).Item("Validita_Fine_" & nCol),
                                                      objParametri,
                                                    Veg_Cod_Agea:=Dt_Appezzamenti.Rows(i).Item("Veg_Cod_Agea"),
                                                    Cul_Cod_Agea:=Dt_Appezzamenti.Rows(i).Item("Cul_Cod_Agea"),
                                                    Uso_Cod_Agea:=Dt_Appezzamenti.Rows(i).Item("Uso_Cod_Agea"),
                                                    Occupazione_Cod_Agea:=Dt_Appezzamenti.Rows(i).Item("Occupazione_Cod_Agea"),
                                                    Destinazione_Cod_Agea:=Dt_Appezzamenti.Rows(i).Item("Destinazione_Cod_Agea"),
                                                    Qualita_Cod_Agea:=Dt_Appezzamenti.Rows(i).Item("Qualita_Cod_Agea"))


                                If Dt_Appezzamenti.Rows(i).Item("Mod") <> "" Then

                                    Dim objModifica As New AgronicaCoreGestoreModificheBIZ.AnalizzaModifiche

                                    Dim Nome_Tabella As String = DatiAnagrafici_PianoColturale

                                    Dim objChiave As New AgronicaCoreGestoreModificheBIZ.ChiaveDT(objParametri.PivaSuperUser, Programmazione_Entita_Cod, Nome_Tabella)

                                    Dim bRet As Boolean = True

                                    'Preparo il dt x salvare le modifiche
                                    objChiave.DT_Modifiche.Rows(0).Item(2) = "1"

                                    bRet = objModifica.AggiornaStato(objChiave,
                                                              AGRODATAINIZIO,
                                                              AGRODATAFINE,
                                                              objParametri.objConnessione,
                                                              objParametri.objTransazione,
                                                              objParametri.StringaConnessione,
                                                              1,
                                                              objParametri.LogDirectory,
                                                              objParametri.LogFileName,
                                                              objParametri.UtenteCodFiscale,
                                                              ErrMSG,
                                                              objParametri)

                                End If




                            End If

                        Next

                    Next

                    intRet = 1


                    '----------------------------------------------------------
                    '----------------------------------------------------------
                    '------    CANCELLAZIONE    -------------------------------
                    '----------------------------------------------------------
                    '----------------------------------------------------------


                Case enum_TipoOperazioneDB.Cancellazione


                    Dim Dt_EntitaxParticelle As DataTable

                    '----------------------------------------------------------------------------
                    'LEGGO le entità a cui era associata la particella
                    Dim objPP As New AgronicaCoreAnagrafeDAL.Programmazione_Particelle_R
                    Dt_EntitaxParticelle = objPP.Programmazione_Particelle_Leggi(objParametri,
                                                                                  Piva,
                                                                                  ErrMSG,
                                                                                  0, 0,
                                                                                  Prov,
                                                                                  Com,
                                                                                  Sezione,
                                                                                  Foglio,
                                                                                  Numero,
                                                                                  Subalterno,
                                                                                  , ,
                                                                                  True)




                    '----------------------------------------------------------------------------
                    'ELIMINO le entità a cui era associata la particella da Programmazione_Entita

                    Dim ArrayCampo(0) As Integer
                    Dim n As Integer = 0
                    Dim CampoPresente As Boolean

                    If Dt_EntitaxParticelle IsNot Nothing AndAlso Dt_EntitaxParticelle.Rows.Count > 0 Then

                        For i = 0 To Dt_EntitaxParticelle.Rows.Count - 1

                            Programmazione_Entita_Cod = Dt_EntitaxParticelle.Rows(i).Item("Programmazione_Entita_Cod")

                            Campo_Cod = Dt_EntitaxParticelle.Rows(i).Item("Campo_Cod")

                            CampoPresente = False

                            For j = 0 To UBound(ArrayCampo)
                                If ArrayCampo(j) = Campo_Cod Then
                                    CampoPresente = True
                                    Exit For
                                End If
                            Next

                            If Not CampoPresente Then

                                '----------------------------------------------------------------------------
                                'ELIMINO il campo e i suoi figli

                                Dim objProgrammazione_Entita_W As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_W

                                BoolDummy = objProgrammazione_Entita_W.Cancella(Programmazione_Cod,
                                                                                Programmazione_Entita_Cod,
                                                                                False,
                                                                                True,
                                                                                Campo_Cod,
                                                                                "",
                                                                                objParametri)

                                '----------------------------------------------------------------------------
                                'ELIMINO le particelle legate al campo da Programmazione_Particelle

                                Dim objProgrammazione_Particelle_W As New AgronicaCoreAnagrafeDAL.Programmazione_Particelle_W

                                BoolDummy = objProgrammazione_Particelle_W.Cancella("",
                                                                                    Programmazione_Entita_Cod,
                                                                                    Prov,
                                                                                    Com,
                                                                                    Sezione,
                                                                                    Foglio,
                                                                                    Numero,
                                                                                    Subalterno,
                                                                                    "",
                                                                                    objParametri)

                                ReDim Preserve ArrayCampo(n)
                                ArrayCampo(n) = Campo_Cod
                                n += 1

                            End If

                        Next

                    End If

                    intRet = 1

            End Select

            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)

        Catch ex As Exception

            'Faccio il rollback della transazione
            If objParametri.objTransazione IsNot Nothing Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        Finally
            'Chiudo la connessione se è stata aperta in questa routine
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)
        End Try

        Return intRet

    End Function

    Public Function Pianificazione_CopiaConRicette(ByVal Programmazione_Cod_DaCopiare As Integer,
                                          ByRef objParametri As AgronicaCoreParametri,
                                          Optional ByVal Piva As String = "",
                                          Optional ByVal Programmazione_Cod As Integer = 0,
                                          Optional ByVal Programmazione_Des As String = "",
                                          Optional ByVal Programmazione_Des_Long As String = "",
                                          Optional ByVal Note As String = "",
                                          Optional ByVal Validita_Inizio As Date = #1/1/1900#,
                                          Optional ByVal Validita_Fine As Date = #12/31/2100#,
                                                   Optional ByVal CopiaDateFineEntita As Boolean = False) As Integer

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Programmazione_W.Pianificazione_Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim ErrMSG As String = ""

        Dim Codice As Integer = 0
        Dim RecordInteressati As Integer = 0
        Dim Programmazione_Entita_Cod As Integer = 0
        Dim i, j As Integer
        Dim Dr As DataRow()
        Dim Programmazione_Cod_DaCopiare_App As Integer
        Dim BoolDummy As Boolean
        Dim MetodoProduzione_Cod As Integer

        Dim Dt_Testata_R As New DataTable
        Dim Dt_Entita_R As New DataTable
        Dim Dt_Entita_Eliminate_R As New DataTable
        Dim Dt_Particelle_R As New DataTable

        Dim Dt_ParticelleCondotte_R As New DataTable
        Dim DrCondotte As DataRow()

        Dim Tipo_Pianificazione As Integer = 0
        Dim Fonte_Cod As Integer = 0

        Dim InserisciEntita As Boolean
        Dim Appezza As Integer = 0
        Dim Campo_Cod As Integer = 0
        Dim Area_Cod As Integer = 0

        Dim strFiltro As String

        Dim Anno As Integer = Validita_Inizio.Year

        Programmazione_Cod_DaCopiare_App = Programmazione_Cod_DaCopiare

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        ' datatable per salvare il mapping tra vecchie e nuove entità
        Dim DtMapping As New DataTable
        DtMapping.Columns.Add(New DataColumn("Codice_Nuovo", GetType(Integer)))
        DtMapping.Columns.Add(New DataColumn("Codice_Vecchio", GetType(Integer)))


        Try

            '------------------------------
            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                           FlagTransazioneLocale,
                                                           objParametri)

            '----------------------------------------------
            'leggo i dati della programmazione da copiare
            Dim objProgrammazione_Testata As New AgronicaCoreAnagrafeDAL.Programmazione_Testata_R

            Dt_Testata_R = objProgrammazione_Testata.Leggi(
                                       ErrMSG,
                                       Programmazione_Cod_DaCopiare,
                                       Piva,
                                       "",
                                       1,
                                       Estremo_Validita_Inizio,
                                       Estremo_Validita_Fine,
                                       enum_TipoRicetta.Non_Filtrare,
                                       enumSelezioneVariabile.Selezione_TabellaCompleta,
                                       "", "",
                                       objParametri)

            Programmazione_Cod_DaCopiare = Programmazione_Cod_DaCopiare_App

            'leggo le particelle condotte alle date nuove
            'potrebbero esserci particelle non piu in conduzione
            '(in tal caso non devo copiare nè le particelle nè le entità!!)
            strFiltro = " AND ImpreseXParticelle.Sup_Condotta<> 0 "
            Dim objParticelle As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_R
            Dt_ParticelleCondotte_R = objParticelle.Anagrafica_Particelle_Leggi(Piva,
                                                                         0,
                                                                         Validita_Inizio,
                                                                         Validita_Fine,
                                                                         objParametri,
                                                                         strFiltro)

            If Dt_Testata_R IsNot Nothing AndAlso Dt_Testata_R.Rows.Count > 0 Then

                If Programmazione_Des = "" Then
                    Programmazione_Des = Dt_Testata_R.Rows(0).Item("Programmazione_Des")
                End If
                If Programmazione_Des_Long = "" Then
                    Programmazione_Des_Long = Dt_Testata_R.Rows(0).Item("Programmazione_Des_Long")
                End If

                If Piva = "" Then
                    Piva = Dt_Testata_R.Rows(0).Item("Piva")
                End If
                If Note = "" Then
                    Note = Dt_Testata_R.Rows(0).Item("Note")
                End If

                If Validita_Inizio = #1/1/1900# Then
                    Validita_Inizio = Dt_Testata_R.Rows(0).Item("Validita_Inizio")
                End If
                If Validita_Fine = #12/31/2100# Then
                    Validita_Fine = Dt_Testata_R.Rows(0).Item("Validita_Fine")
                End If

                Tipo_Pianificazione = Dt_Testata_R.Rows(0).Item("Tipo_Pianificazione")

                If Not IsDBNull(Dt_Testata_R.Rows(0).Item("Fonte_Cod")) Then
                    Fonte_Cod = Dt_Testata_R.Rows(0).Item("Fonte_Cod")
                End If

                'CREO LA NUOVA TESTATA

                Dim objProgrammazione_Testata_W As New AgronicaCoreAnagrafeDAL.Programmazione_Testata_W

                BoolDummy = objProgrammazione_Testata_W.Scrivi(Codice,
                                                                Programmazione_Des,
                                                                Programmazione_Des_Long,
                                                                Piva,
                                                                Note,
                                                                Tipo_Pianificazione,
                                                                Fonte_Cod,
                                                                Validita_Inizio,
                                                                Validita_Fine,
                                                                objParametri)

                '----------------------------------------------------
                'Leggo le entità della programmazione da copiare

                Dim objProgrammazione_Entita_R As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_R

                Dt_Entita_R = objProgrammazione_Entita_R.Programmazione_Entita_Leggi_conAreeOmogenee(
                                            Programmazione_Cod_DaCopiare,
                                            ErrMSG,
                                            0,
                                            "",
                                            "",
                                            0,
                                            0,
                                            0,
                                            0,
                                            0,
                                            Estremo_Validita_Inizio,
                                            Estremo_Validita_Fine,
                                            2,
                                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                                            "",
                                            "",
                                            objParametri)

                '----------------------------------
                'leggo le entità da cancellare

                Dim objProgrammazione_Entita_Eliminate_R As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_Eliminate_R

                Dt_Entita_Eliminate_R = objProgrammazione_Entita_Eliminate_R.Leggi(
                                            Programmazione_Cod_DaCopiare,
                                            ErrMSG,
                                            0,
                                            Estremo_Validita_Inizio,
                                            Estremo_Validita_Fine,
                                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                                            "",
                                            "",
                                            objParametri)

                '----------------------------------
                'leggo le particelle associate alle varie entità
                Dim objPP As New AgronicaCoreAnagrafeDAL.Programmazione_Particelle_R
                Dt_Particelle_R = objPP.Programmazione_Particelle_Leggi_2(objParametri,
                                                                         Programmazione_Cod_DaCopiare,
                                                                         ErrMSG,
)

                If Dt_Entita_R IsNot Nothing AndAlso Dt_Entita_R.Rows.Count > 0 Then

                    For i = 0 To Dt_Entita_R.Rows.Count - 1

                        InserisciEntita = False

                        '----------------------------------------
                        'verifico se l'ENTITA' è associata ad una o piu PARTICELLE
                        'se SI --> verifico che la particella sia ancora in CONDUZIONE all'azienda
                        '          copio l'entità solo in questo caso
                        'se NO --> non copio l'entità
                        Dr = Dt_Particelle_R.Select("Programmazione_Entita_Cod=" & Dt_Entita_R.Rows(i).Item("Programmazione_Entita_Cod"))

                        If Dr IsNot Nothing AndAlso Dr.Length > 0 Then

                            For j = 0 To Dr.Length - 1

                                DrCondotte = Dt_ParticelleCondotte_R.Select("Prov='" & Dr(j).Item("prov") & "'" &
                                                                            " AND Com='" & Dr(j).Item("com") & "'" &
                                                                            " AND sezione='" & Dr(j).Item("sezione") & "'" &
                                                                            " AND foglio=" & Dr(j).Item("foglio") & "" &
                                                                            " AND numero=" & Dr(j).Item("numero") & "" &
                                                                            " AND subalterno='" & Dr(j).Item("subalterno") & "'")

                                If DrCondotte IsNot Nothing AndAlso DrCondotte.Length > 0 Then
                                    InserisciEntita = True
                                Else
                                    InserisciEntita = False
                                    Exit For
                                End If

                            Next
                        Else
                            InserisciEntita = True
                        End If

                        If InserisciEntita Then

                            Dim objProgrammazione_Entita_W As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_W

                            Programmazione_Entita_Cod = 0

                            Dim Validita_Inizio_Entita As Date = Validita_Inizio
                            Dim Validita_Fine_Entita As Date = Validita_Fine

                            Select Case Tipo_Pianificazione


                                Case enum_TipoPianificazione.Pianificazione_Annuale,
                                     enum_TipoPianificazione.Pianificazione_DaNotificaBio

                                    ''lascio l'anno d'impianto
                                    'Validita_Inizio = Dt_Entita_R.Rows(i).Item("validita_inizio")
                                    'strValidita_Inizio = CDate(Validita_Inizio).ToShortDateString()

                                    '(08/11/2018 fede) introdotto il parametro CopiaDateEntita
                                    'come da richiesta di campisi x copia pua (vuole mantenere le date fine della pianificazione)
                                    Validita_Inizio_Entita = Dt_Entita_R.Rows(i).Item("validita_inizio")

                                    If CopiaDateFineEntita Then
                                        Validita_Fine_Entita = Dt_Entita_R.Rows(i).Item("Validita_Fine")
                                    End If

                                Case enum_TipoPianificazione.Pianificazione_Quindicinale

                                    'devo x forza far combinare le date delle quindicine
                                    Dim strValidita_Inizio As String
                                    Validita_Inizio_Entita = Dt_Entita_R.Rows(i).Item("validita_inizio")
                                    strValidita_Inizio = CDate(Validita_Inizio_Entita).ToShortDateString()
                                    strValidita_Inizio = Left(strValidita_Inizio, 6) & Anno.ToString
                                    Validita_Inizio_Entita = CDate(strValidita_Inizio)

                            End Select


                            MetodoProduzione_Cod = enum_MetodoProduzione.Integrato

                            BoolDummy = objProgrammazione_Entita_W.Scrivi(Codice,
                                              Programmazione_Entita_Cod,
                                              Dt_Entita_R.Rows(i).Item("Entita_Des").ToString,
                                              Piva,
                                              Dt_Entita_R.Rows(i).Item("Sa_Cod"),
                                              Dt_Entita_R.Rows(i).Item("Campo_Cod"),
                                              Dt_Entita_R.Rows(i).Item("Appezza"),
                                              Dt_Entita_R.Rows(i).Item("ID_Reg"),
                                              Dt_Entita_R.Rows(i).Item("Progetto_Cod"),
                                              Dt_Entita_R.Rows(i).Item("Progetto_Des"),
                                              Dt_Entita_R.Rows(i).Item("Id_Cod"),
                                              Dt_Entita_R.Rows(i).Item("Veg_Cod"),
                                              Dt_Entita_R.Rows(i).Item("Cul_Cod"),
                                              Dt_Entita_R.Rows(i).Item("Grva_Cod"),
                                              Dt_Entita_R.Rows(i).Item("Grfi_Cod"),
                                              Dt_Entita_R.Rows(i).Item("Cop_Cod"),
                                              Dt_Entita_R.Rows(i).Item("Superficie"),
                                              Dt_Entita_R.Rows(i).Item("Resa"),
                                              Dt_Entita_R.Rows(i).Item("TipoZona"),
                                              0,
                                              0,
                                              0,
                                              0,
                                              Dt_Entita_R.Rows(i).Item("Num_Piante"),
                                              Dt_Entita_R.Rows(i).Item("Tra_Fila"),
                                              Dt_Entita_R.Rows(i).Item("Su_Fila"),
                                              Dt_Entita_R.Rows(i).Item("Foral_Cod"),
                                              Dt_Entita_R.Rows(i).Item("Port_Cod"),
                                              Dt_Entita_R.Rows(i).Item("Imp_Cod"),
                                              Dt_Entita_R.Rows(i).Item("Regolamento_Cod"),
                                              0,
                                              Dt_Entita_R.Rows(i).Item("Stato_Cod"),
                                              Dt_Entita_R.Rows(i).Item("Ciclo"),
                                              Dt_Entita_R.Rows(i).Item("Data_Semina"),
                                              Dt_Entita_R.Rows(i).Item("Data_Raccolta"),
                                              Dt_Entita_R.Rows(i).Item("Note"),
                                              Dt_Entita_R.Rows(i).Item("Veg_Cod_Cliente"),
                                              Dt_Entita_R.Rows(i).Item("Cul_Cod_Cliente"),
                                              MetodoProduzione_Cod,
                                              Validita_Inizio_Entita,
                                              Validita_Fine_Entita,
                                              objParametri,
                                            Veg_Cod_Agea:=DBNullDefaultValueString(Dt_Entita_R.Rows(i).Item("Veg_Cod_Agea")),
                                            Cul_Cod_Agea:=DBNullDefaultValueString(Dt_Entita_R.Rows(i).Item("Cul_Cod_Agea")),
                                            Uso_Cod_Agea:=DBNullDefaultValueString(Dt_Entita_R.Rows(i).Item("Uso_Cod_Agea")),
                                            Occupazione_Cod_Agea:=DBNullDefaultValueString(Dt_Entita_R.Rows(i).Item("Occupazione_Cod_Agea")),
                                            Destinazione_Cod_Agea:=DBNullDefaultValueString(Dt_Entita_R.Rows(i).Item("Destinazione_Cod_Agea")),
                                            Qualita_Cod_Agea:=DBNullDefaultValueString(Dt_Entita_R.Rows(i).Item("Qualita_Cod_Agea")))

                            If BoolDummy Then
                                ' la chiave è il codice vecchio, il valore il codice nuovo
                                ' HashEntita.Add(Dt_Entita_R.Rows(i).Item("Programmazione_Entita_Cod"), Programmazione_Entita_Cod)

                                Dim DrEntita As DataRow
                                DrEntita = DtMapping.NewRow
                                DrEntita.Item("Codice_Nuovo") = Programmazione_Entita_Cod
                                DrEntita.Item("Codice_Vecchio") = Dt_Entita_R.Rows(i).Item("Programmazione_Entita_Cod")

                                DtMapping.Rows.Add(DrEntita)

                            End If

                            '------------------------------------------
                            'Inserisco le Particelle legate all'entità

                            If Dr IsNot Nothing AndAlso Dr.Length > 0 Then

                                Dim objProgrammazione_Particelle_W As New AgronicaCoreAnagrafeDAL.Programmazione_Particelle_W

                                For j = 0 To Dr.Length - 1

                                    BoolDummy = objProgrammazione_Particelle_W.Scrivi(Programmazione_Entita_Cod,
                                                                                      Dr(j).Item("Prov").ToString,
                                                                                      Dr(j).Item("Com").ToString,
                                                                                      Dr(j).Item("Sezione").ToString,
                                                                                      Dr(j).Item("Foglio"),
                                                                                      Dr(j).Item("Numero"),
                                                                                      Dr(j).Item("Subalterno").ToString,
                                                                                      Dr(j).Item("Superficie"),
                                                                                      Validita_Inizio_Entita,
                                                                                      Validita_Fine_Entita,
                                                                                      objParametri)

                                Next

                            End If

                            '------------------------------------------------------
                            'Inserisco le eventuali Aree Omogenee legate all'entità

                            Area_Cod = Dt_Entita_R.Rows(i).Item("Area_Cod")

                            If Area_Cod <> 0 Then

                                Dim objAree_W As New AgronicaCoreAnagrafeDAL.Area_OmogeneaxEntita_W

                                objAree_W.Scrivi(Piva,
                                                 Area_Cod,
                                                 Programmazione_Entita_Cod,
                                                 Validita_Inizio_Entita,
                                                 Validita_Fine_Entita,
                                                 objParametri)

                            End If


                        End If


                    Next

                End If


                If Dt_Entita_Eliminate_R IsNot Nothing AndAlso Dt_Entita_Eliminate_R.Rows.Count > 0 Then


                    Dim objProgrammazione_Entita_Eliminate_W As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_Eliminate_W

                    For i = 0 To Dt_Entita_Eliminate_R.Rows.Count - 1

                        'metto in un try xkè quando fraziono un appezzamento proveniente dal reale
                        'da errore di inserimento chiave duplicata
                        Try

                            BoolDummy = objProgrammazione_Entita_Eliminate_W.Scrivi(
                                                          Piva,
                                                          Dt_Entita_Eliminate_R.Rows(i).Item("Sa_Cod"),
                                                          Dt_Entita_Eliminate_R.Rows(i).Item("Appezza"),
                                                          Dt_Entita_Eliminate_R.Rows(i).Item("ID_Reg"),
                                                          Dt_Entita_Eliminate_R.Rows(i).Item("Campo_Cod"),
                                                          Codice,
                                                          Dt_Entita_Eliminate_R.Rows(i).Item("Programmazione_Entita_Cod"),
                                                          Dt_Entita_Eliminate_R.Rows(i).Item("Progetto_Cod"),
                                                          Dt_Entita_Eliminate_R.Rows(i).Item("Operazione_Cod"),
                                                          Dt_Entita_Eliminate_R.Rows(i).Item("Validita_Inizio"),
                                                          Dt_Entita_Eliminate_R.Rows(i).Item("Validita_Fine"),
                                                          objParametri)

                        Catch ex As Exception

                        End Try
                    Next

                End If

            End If

            Dim h As Integer
            For h = 0 To DtMapping.Rows.Count - 1

                Dim objRicetteDest As New AgronicaCoreContabDAL.Ricette_Destinazioni_R
                Dim objRicette As New AgronicaCoreContabBIZ.Ricette_R
                Dim objRicette_W As New AgronicaCoreContabBIZ.Ricette_W

                'Dim objSequenze As New Agro_Sequenze
                'objSequenze.Calcola_BaseCode(


                'Dim BaseCode As Integer = 0
                'Dim TopCode As Integer = 0
                'objSequenze.Calcola_BaseCode(objParametri., _
                '                             TopCode, BaseCode, String.Empty, String.Empty, objParametri.UsernameOperazione)

                Dim DtRicette As DataTable
                DtRicette = objRicetteDest.Leggi_Operazioni_Da_Programmazione_Entita_Cod(0, 0, 0, 0, DtMapping.Rows(h).Item("Codice_Vecchio"), AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri)

                Dim r As Integer
                For r = 0 To DtRicette.Rows.Count - 1

                    Dim strRicette As String = objRicette.Ricetta_Leggi(DtRicette.Rows(r).Item("Ricetta_Cod"), "", 0, 0, 0, False, objParametri)

                    ' sostituisco i codici
                    strRicette = strRicette.Replace("TipoOperazioneDB=""0""", "TipoOperazioneDB=""1"" basecode=""1"" topcode=""1""") ' da lettura a scrittura
                    strRicette = strRicette.Replace("programmazione_cod=""" & Programmazione_Cod_DaCopiare & """", "programmazione_cod=""" & Codice & """") ' codice programmazione
                    strRicette = strRicette.Replace("programmazione_entita_cod=""" & DtMapping.Rows(h).Item("Codice_Vecchio") & """", "programmazione_entita_cod=""" & DtMapping.Rows(h).Item("Codice_Nuovo") & """") ' codice entita

                    strRicette = strRicette.Replace("ricetta_cod=""" & DtRicette.Rows(r).Item("Ricetta_Cod") & """", "ricetta_cod=""0""")
                    strRicette = strRicette.Replace("ricetta_operazione_cod=""" & DtRicette.Rows(r).Item("Ricetta_Operazione_Cod") & """", "ricetta_operazione_cod=""0""")
                    strRicette = strRicette.Replace("ricetta_dettaglio_cod=""" & DtRicette.Rows(r).Item("Ricetta_Dettaglio_Cod") & """", "ricetta_dettaglio_cod=""0""")
                    strRicette = strRicette.Replace("ricetta_destinazione_cod=""" & DtRicette.Rows(r).Item("Ricetta_Destinazione_Cod") & """", "ricetta_destinazione_cod=""0""")


                    Dim objDettTecnici As New AgronicaCoreContabDAL.Ricette_Dett_Tecnico_R
                    Dim DtTecnici As DataTable
                    DtTecnici = objDettTecnici.Leggi(DtRicette.Rows(r).Item("Ricetta_Cod"),
                                                     DtRicette.Rows(r).Item("Ricetta_Operazione_Cod"),
                                                     DtRicette.Rows(r).Item("Ricetta_Dettaglio_Cod"),
                                                     0, 0, AGRODATAINIZIO, AGRODATAFINE, enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                     "", "",
                                                     objParametri)

                    Dim t As Integer
                    For t = 0 To DtTecnici.Rows.Count - 1
                        strRicette = strRicette.Replace("ricetta_tecnico_cod=""" & DtTecnici.Rows(t).Item("Ricetta_Tecnico_Cod") & """ ", "ricetta_tecnico_cod=""0"" ")
                    Next

                    Dim RicettaCodNuovo As Integer
                    objRicette_W.Ricetta_Scrivi(strRicette, RicettaCodNuovo, objParametri)

                Next


            Next



            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)

        Catch ex As Exception

            'Faccio il rollback della transazione
            If objParametri.objTransazione IsNot Nothing Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        Finally
            'Chiudo la connessione se è stata aperta in questa routine
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)
        End Try

        Return Codice

    End Function

    Function DBNullDefaultValueString(value As Object, Optional defaultValue As String = "") As String

        If value Is Nothing OrElse IsDBNull(value) Then
            Return defaultValue
        Else
            Return CStr(value)
        End If

    End Function


    Public Function Pianificazione_Modifica_Validita_Fine(ByVal Programmazione_Cod As Integer,
                                                    ByVal Validita_Fine As Date,
                                                    ByRef objParametri As AgronicaCoreParametri
                                          ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Programmazione_W.Pianificazione_Modifica_Validita_Fine()"
        Dim MessaggioErrore As String = ""

        Dim i As Integer
        Dim BoolDummy As Boolean

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try

            '------------------------------
            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                           FlagTransazioneLocale,
                                                           objParametri)


            Dim objProgrammazione_Testata_W As New AgronicaCoreAnagrafeDAL.Programmazione_Testata_W

            BoolDummy = objProgrammazione_Testata_W.Modifica_Validita_Fine(Programmazione_Cod, Validita_Fine, "", objParametri)

            Dim objProgrammazione_Entita_W As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_W

            BoolDummy = objProgrammazione_Entita_W.Modifica_Validita_Fine(Programmazione_Cod, 0, Validita_Fine, "", objParametri)

            '----------------------------------
            'leggo le particelle associate alle varie entità
            Dim objPP As New AgronicaCoreAnagrafeDAL.Programmazione_Particelle_R
            Dim objPP_W As New AgronicaCoreAnagrafeDAL.Programmazione_Particelle_W
            Dim Dt_Particelle_R As DataTable
            Dt_Particelle_R = objPP.LeggiParticelle_Da_Programmazione("", 0,
                                                                      Programmazione_Cod,
                                                                      0, enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                      "", "",
                                                                      objParametri)

            If Dt_Particelle_R IsNot Nothing AndAlso Dt_Particelle_R.Rows.Count > 0 Then

                For i = 0 To Dt_Particelle_R.Rows.Count - 1

                    BoolDummy = objPP_W.Modifica_Validita_Fine(Dt_Particelle_R.Rows(i).Item("Programmazione_Entita_Cod"), Validita_Fine, "", objParametri)

                Next

            End If

            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)

        Catch ex As Exception

            ''Faccio il rollback della transazione
            ConnessioniTransazioni.RollBackTransazione(objParametri)

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            'Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
            Return False

        Finally
            'Chiudo la connessione se è stata aperta in questa routine
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)
        End Try

        Return True

    End Function

    Public Function Pianificazione_Modifica_Validita(ByVal Programmazione_Cod As Integer,
                                                    ByVal Validita_Inizio As Date,
                                                    ByVal Validita_Fine As Date,
                                                    ByRef objParametri As AgronicaCoreParametri
                                          ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Programmazione_W.Pianificazione_Modifica_Validita()"
        Dim MessaggioErrore As String = ""

        Dim i As Integer
        Dim BoolDummy As Boolean

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False


        Try

            '------------------------------
            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                           FlagTransazioneLocale,
                                                           objParametri)


            Dim objProgrammazione_Testata_W As New AgronicaCoreAnagrafeDAL.Programmazione_Testata_W

            BoolDummy = objProgrammazione_Testata_W.Modifica_Validita(Programmazione_Cod, Validita_Inizio, Validita_Fine, "", objParametri)

            Dim objProgrammazione_Entita_W As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_W

            BoolDummy = objProgrammazione_Entita_W.Modifica_Validita(Programmazione_Cod, 0, Validita_Inizio, Validita_Fine, "", objParametri)

            '----------------------------------
            'leggo le particelle associate alle varie entità
            Dim objPP As New AgronicaCoreAnagrafeDAL.Programmazione_Particelle_R
            Dim objPP_W As New AgronicaCoreAnagrafeDAL.Programmazione_Particelle_W
            Dim Dt_Particelle_R As DataTable
            Dt_Particelle_R = objPP.LeggiParticelle_Da_Programmazione("", 0,
                                                                      Programmazione_Cod,
                                                                      0, enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                      "", "",
                                                                      objParametri)

            If Dt_Particelle_R IsNot Nothing AndAlso Dt_Particelle_R.Rows.Count > 0 Then

                For i = 0 To Dt_Particelle_R.Rows.Count - 1

                    BoolDummy = objPP_W.Modifica_Validita(Dt_Particelle_R.Rows(i).Item("Programmazione_Entita_Cod"), Validita_Inizio, Validita_Fine, "", objParametri)

                Next

            End If


            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)

        Catch ex As Exception

            ''Faccio il rollback della transazione
            ConnessioniTransazioni.RollBackTransazione(objParametri)

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            'Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
            Return False

        Finally
            'Chiudo la connessione se è stata aperta in questa routine
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)
        End Try

        Return True

    End Function

End Class
