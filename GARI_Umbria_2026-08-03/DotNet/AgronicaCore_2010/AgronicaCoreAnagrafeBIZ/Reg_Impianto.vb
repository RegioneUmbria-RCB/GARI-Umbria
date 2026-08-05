Imports System.Data.Entity
Imports System.Linq
Imports System.Threading.Tasks
Imports System.Transactions
Imports System.Web.UI.WebControls
Imports System.Xml
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDTOStd.Identity
Imports AgronicaCoreDTOStd.OutData.Gis
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreGisBIZ
Imports AgronicaCoreModelsSTD
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.costanti
Imports AgronicaCoreModelsSTD.exceptions
Imports AgronicaCoreModelsSTD.metaschema.utilizzi
Imports AgronicaCoreModelsSTD.utente
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class Reg_Impianto_R
    Inherits AgronicaCoreDataProvider.LogProvider

    '============================================================================
    Public Function Reg_Impianto_Leggi(ByVal Piva As String,
                                       ByVal Sa_Cod As Integer,
                                       ByVal appezza As Integer,
                                       ByVal id_reg As Integer,
                                       ByVal ForDelete As Boolean,
                                       ByVal AllAttributes As Boolean,
                                       ByRef objParametri As AgronicaCoreParametri,
                                       Optional ByVal TipoG2G As Integer = 0
                                       ) As String

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutti gli impianti dell'utente
        '   Sa_Cod = 0                  =>  si leggono tutti gli impianti dell'impresa
        '   appezza = 0                 =>  si leggono tutti gli impianti del centro
        '   id_reg = 0                  =>  si leggono tutti gli impianti dell'appezzamento
        '====================================================================================


        Const nomeRoutine = "AnagrafeBIZ.Reg_Impianto_R.Reg_Impianto_Leggi()"

        Dim messaggioErrore As String = ""

        Dim FlagConnessioneLocale As Boolean = False


        Dim i, j As Integer

        Dim XmlDoc As XmlDocument

        Dim XmlDatiReg_Impianti As XmlElement
        Dim XmlReg_Impianto As XmlElement

        Dim XmlDatiCodici As XmlElement

        Dim XmlCodice As XmlElement

        Dim objReg_Impianto As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        Dim objCodici As AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R

        Dim DtReg_Impianti As DataTable
        Dim DtCodici As DataTable


        Dim RowCountReg_Impianti As Integer
        Dim RowCountCodici As Integer

        Dim RisultatoFunzione As String

        '------------------------------

        Try

            '------------------------------
            'Verifico se è stata impostata una connessione
            If IsNothing(objParametri.objConnessione) Then
                FlagConnessioneLocale = True
                objParametri.objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
            End If
            '------------------------------

            DtReg_Impianti = objReg_Impianto.Leggi(CStr(Piva),
                                                   CInt(Sa_Cod),
                                                   CInt(appezza),
                                                   CInt(id_reg),
                                                   enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                   "",
                                                   "",
                                                   objParametri,
                                                   TipoG2G:=TipoG2G)

            '-----------------------------

            'Se ottengo almeno un risultato, creo la struttura XML
            If Not IsNothing(DtReg_Impianti) Then

                '----- < Documento XML > -----
                XmlDoc = New XmlDocument

                XmlDatiReg_Impianti = XmlDoc.CreateElement("DatiReg_Impianti")

                RowCountReg_Impianti = 0
                'Effettuo un ciclo sugli Impianti
                Do While RowCountReg_Impianti <= DtReg_Impianti.Rows.Count - 1

                    '----- < REG_IMPIANTO > -----
                    XmlReg_Impianto = XmlDoc.CreateElement("Reg_Impianto")

                    i = RowCountReg_Impianti   'Alias

                    With XmlReg_Impianto
                        .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                        .SetAttribute("piva", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("PIVA")))
                        .SetAttribute("sa_cod", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Sa_Cod")))
                        .SetAttribute("appezza", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Appezza")))
                        .SetAttribute("id_reg", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Id_Reg")))
                        .SetAttribute("cul_cod", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Cul_Cod")))
                        .SetAttribute("grfi_cod", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Grfi_Cod")))
                        .SetAttribute("resa_prevista", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Resa_Prevista")))
                        .SetAttribute("resa_effettiva", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Resa_Effettiva")))
                        .SetAttribute("cop_di", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Cop_Di")))
                        .SetAttribute("cop_df", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Cop_Df")))
                        .SetAttribute("tra_fila", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Tra_Fila")))
                        .SetAttribute("su_fila", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Su_Fila")))
                        .SetAttribute("p_ha", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("P_Ha")))
                        '- - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -
                        .SetAttribute("Data_Inizio_Portinnesto", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Data_Inizio_Portinnesto")))
                        .SetAttribute("Data_Inizio_Innesto", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Data_Inizio_Innesto")))
                        .SetAttribute("Data_Inizio_Produzione", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Data_Inizio_Produzione")))
                        .SetAttribute("Piante_Maschi_InSesto", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Piante_Maschi_InSesto")))
                        '- - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -
                        .SetAttribute("foral_cod", Agro_SQL_Load_ConDefault(DtReg_Impianti.Rows(i).Item("Foral_Cod"), GetType(Integer)))
                        .SetAttribute("setup_cod", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Setup_Cod")))
                        .SetAttribute("port_cod", Agro_SQL_Load_ConDefault(DtReg_Impianti.Rows(i).Item("Port_Cod"), GetType(Integer)))
                        .SetAttribute("imp_cod", Agro_SQL_Load_ConDefault(DtReg_Impianti.Rows(i).Item("Imp_Cod"), GetType(Integer)))
                        .SetAttribute("stru_prot", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Stru_Prot")))
                        .SetAttribute("tecn_cod", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Tecn_Cod")))
                        .SetAttribute("su_cod", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Su_Cod")))
                        .SetAttribute("cop_cod", Agro_SQL_Load_ConDefault(DtReg_Impianti.Rows(i).Item("Cop_Cod"), GetType(Integer)))
                        .SetAttribute("cover", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Cover")))
                        .SetAttribute("monitorato", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Monitorato")))
                        .SetAttribute("user", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("User")))
                        .SetAttribute("regolamento", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Regolamento")))
                        .SetAttribute("finanziamento", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Finanziamento")))
                        .SetAttribute("sup_imp", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Sup_Imp")))
                        .SetAttribute("id_consociazione", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Id_Consociazione")))
                        .SetAttribute("inviato", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Inviato")))
                        .SetAttribute("datainvio", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("DataInvio")))
                        .SetAttribute("data_creazione", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Data_Creazione")))
                        .SetAttribute("data_modifica", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Data_Modifica")))
                        .SetAttribute("username_creazione", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Username_Creazione")))
                        .SetAttribute("username_modifica", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Username_Modifica")))
                        .SetAttribute("validita_inizio", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Validita_Inizio")))
                        .SetAttribute("validita_fine", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Validita_Fine")))
                        .SetAttribute("gru_cod", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("gru_cod")))
                    End With

                    'Se voglio tutti gli attributi
                    If AllAttributes Then

                        With XmlReg_Impianto
                            .SetAttribute("data_agg", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Data_Agg")))
                            .SetAttribute("cod_resp", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Cod_Resp")))
                            .SetAttribute("cod_ente", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Cod_Ente")))
                            .SetAttribute("campo_spia", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Campo_Spia")))
                            .SetAttribute("data", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Data")))
                            .SetAttribute("data_raccolta", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Data_Raccolta")))
                            .SetAttribute("scarto", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Scarto")))
                            .SetAttribute("ind_mat_cod", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Ind_Mat_Cod")))
                            .SetAttribute("ind_mat_ril", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Ind_Mat_Ril")))
                            .SetAttribute("sta_ter", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Sta_Ter")))
                            .SetAttribute("pro_pag", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Pro_Pag")))
                            .SetAttribute("seme_q", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Seme_Q")))
                            .SetAttribute("seme_t", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Seme_T")))
                            .SetAttribute("seme_p", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Seme_P")))
                            .SetAttribute("seme_d", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Seme_D")))
                            .SetAttribute("stato_residui", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Stato_Residui")))
                            .SetAttribute("denitrificazione", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Denitrificazione")))
                            .SetAttribute("volatilizzazione", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Volatilizzazione")))
                            .SetAttribute("grva_cod_veg", Agro_SQL_Load_ConDefault(DtReg_Impianti.Rows(i).Item("GrVa_Cod_Veg"), GetType(Integer)))
                            .SetAttribute("profonditalav", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("ProfonditaLav")))
                            .SetAttribute("id_campo", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Id_Campo")))
                            .SetAttribute("data_conversione", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Data_Conversione")))
                            .SetAttribute("codice_fiscale_tecnico", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Codice_Fiscale_Tecnico")))
                            .SetAttribute("provenienzaseme", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("ProvenienzaSeme")))

                            .SetAttribute("produzione", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("PRODUZIONE")))
                            .SetAttribute("sovrainnesto_cod", Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Sovrainnesto_Cod")))
                        End With

                    End If


                    '#################################
                    '##########  CODICI  #############
                    '#################################

                    'Mi procuro un elenco dei codici dell'impianto

                    objCodici = New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R

                    XmlDatiCodici = XmlDoc.CreateElement("DatiCodici")

                    DtCodici = objCodici.Leggi(CStr(Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("PIVA"))),
                                               CInt(Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Sa_Cod"))),
                                               CInt(Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Appezza"))),
                                               CInt(Agro_SQL_Load(DtReg_Impianti.Rows(i).Item("Id_Reg"))),
                                               "",
                                               0,
                                               "",
                                               enumSelezioneVariabile.Selezione_TabellaCompleta,
                                               "",
                                               "",
                                               objParametri)

                    'Se ottengo almeno un risultato, creo la struttura XML
                    If Not IsNothing(DtCodici) Then

                        RowCountCodici = 0
                        'Effettuo un ciclo sugli indirizzi
                        Do While RowCountCodici <= DtCodici.Rows.Count - 1

                            '----- < CODICE > -----
                            XmlCodice = XmlDoc.CreateElement("CodiceImpianto")

                            j = RowCountCodici 'Alias

                            With XmlCodice
                                .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                                .SetAttribute("id_cod", Agro_SQL_Load(DtCodici.Rows(j).Item("id_cod")))
                                .SetAttribute("val_cod", Agro_SQL_Load(DtCodici.Rows(j).Item("val_cod")))
                                .SetAttribute("progetto_cod", 0) 'Indico che il codice è associato all'impianto
                                .SetAttribute("descrizione", Agro_SQL_Load(DtCodici.Rows(j).Item("descrizione")))
                                .SetAttribute("validita_inizio", Agro_SQL_Load(DtCodici.Rows(j).Item("validita_inizio")))
                                .SetAttribute("validita_fine", Agro_SQL_Load(DtCodici.Rows(j).Item("validita_fine")))
                                .SetAttribute("data_creazione", Agro_SQL_Load(DtCodici.Rows(j).Item("data_creazione")))
                                .SetAttribute("data_modifica", Agro_SQL_Load(DtCodici.Rows(j).Item("data_modifica")))
                                .SetAttribute("username_creazione", Agro_SQL_Load(DtCodici.Rows(j).Item("username_creazione")))
                                .SetAttribute("username_modifica", Agro_SQL_Load(DtCodici.Rows(j).Item("username_modifica")))
                            End With

                            XmlDatiCodici.AppendChild(XmlCodice)
                            XmlCodice = Nothing

                            '----- < / CODICE > -----
                            RowCountCodici += 1

                        Loop

                        DtCodici.Dispose()

                    End If

                    DtCodici = Nothing
                    objCodici = Nothing


                    '#################################
                    '#################################
                    '#################################

                    XmlReg_Impianto.AppendChild(XmlDatiCodici)
                    XmlDatiReg_Impianti.AppendChild(XmlReg_Impianto)
                    XmlDatiCodici = Nothing
                    XmlReg_Impianto = Nothing

                    RowCountReg_Impianti += 1

                Loop

                DtReg_Impianti.Dispose()
                DtReg_Impianti = Nothing
                objReg_Impianto = Nothing

                '#################################
                '#################################
                '#################################




                '----- < / Reg_Impianti > -----

                XmlDoc.AppendChild(XmlDatiReg_Impianti)

                RisultatoFunzione = XmlDoc.OuterXml

                '----- < / Documento XML > -----
                XmlDatiReg_Impianti = Nothing
                XmlDoc = Nothing

            Else

                'Altrimenti, se non risulta selezionato nessun impianto ...
                RisultatoFunzione = ""

            End If


        Catch ex As Exception

            RisultatoFunzione = ""
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        Finally

            objReg_Impianto = Nothing
            objCodici = Nothing

            If FlagConnessioneLocale Then
                objParametri.objConnessione.Close()
                objParametri.objConnessione.Dispose()
            End If

        End Try

        'Restituisco il risultato
        Return RisultatoFunzione

    End Function

    Public Function Esistono_ImpiantiAttivi_SenzaID_Cod(Piva As String, Sa_Cod As Integer, id_cod As Integer, Validita_Inizio As Date, Validita_Fine As Date, ByRef objParametri As AgronicaCoreParametri) As Boolean
        Const nomeRoutine = "AnagrafeBIZ.Reg_Impianto_R.Esistono_ImpiantiAttivi_SenzaID_Cod()"
        Dim response As Boolean = False
        Dim messaggioErrore As String = ""

        Try

            Dim objAppezza As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
            Dim objAppezza_Codici As New AgronicaCoreAnagrafeDAL.Appezzamento_Codici_R
            Dim objImpianto As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
            Dim objImpianto_Codici As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R
            Dim objImprese_Progetti As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R

            Dim Finetra_Temporale_Inizio_BK = objParametri.FinestraTemporaleInizio
            Dim Finetra_Temporale_Fine_BK = objParametri.FinestraTemporaleFine

            objParametri.FinestraTemporaleInizio = Validita_Inizio
            objParametri.FinestraTemporaleFine = Validita_Fine

            Dim DTAppezza = objAppezza.Leggi(Piva, Sa_Cod, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)
            For Each rowAppezza In DTAppezza.Rows
                Dim DTCodice = objAppezza_Codici.Leggi(rowAppezza("piva"),
                                                       rowAppezza("sa_cod"),
                                                       rowAppezza("appezza"),
                                                       id_cod, "",
                                                       enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                       "", "", objParametri)

                If DTCodice.Rows.Count = 0 Then
                    Return True
                End If
            Next

            Dim DTImpianto = objImpianto.Leggi(Piva, Sa_Cod, 0, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)
            For Each rowImpianto In DTImpianto.Rows
                Dim DTCodice = objImpianto_Codici.Leggi(rowImpianto("piva"),
                                                       rowImpianto("sa_cod"),
                                                       rowImpianto("appezza"),
                                                       rowImpianto("id_reg"),
                                                       "", id_cod, "",
                                                       enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)

                If DTCodice.Rows.Count = 0 Then
                    Return True
                End If
            Next

            'Dim DTProgetti = objImprese_Progetti.Leggi(Piva, 0, "", 0, Sa_Cod, 0, 0, 0, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)
            'For Each rowImpianto In DTProgetti.Rows
            '    Dim DTCodice = objImpianto_Codici.Leggi(rowImpianto("piva"),
            '                                           rowImpianto("sa_cod"),
            '                                           rowImpianto("appezza"),
            '                                           rowImpianto("id_reg"),
            '                                           "", id_cod, "",
            '                                           enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)

            '    If DTCodice.Rows.Count = 0 Then
            '        Return True
            '    End If
            'Next


            objParametri.FinestraTemporaleInizio = Finetra_Temporale_Inizio_BK
            objParametri.FinestraTemporaleFine = Finetra_Temporale_Fine_BK

        Catch ex As Exception

            response = True
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try


        Return response
    End Function

    Public Function Esistono_ImpiantiAttivi_ConID_Cod(Piva As String, Sa_Cod As Integer, id_cod As Integer, Validita_Inizio As Date, Validita_Fine As Date, ByRef objParametri As AgronicaCoreParametri) As Boolean
        Const nomeRoutine = "AnagrafeBIZ.Reg_Impianto_R.Esistono_ImpiantiAttivi_ConID_Cod()"
        Dim response As Boolean = False
        Dim messaggioErrore As String = ""

        Try

            Dim objAppezza As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
            Dim objAppezza_Codici As New AgronicaCoreAnagrafeDAL.Appezzamento_Codici_R
            Dim objImpianto As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
            Dim objImpianto_Codici As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R
            Dim objImprese_Progetti As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R

            Dim Finetra_Temporale_Inizio_BK = objParametri.FinestraTemporaleInizio
            Dim Finetra_Temporale_Fine_BK = objParametri.FinestraTemporaleFine

            objParametri.FinestraTemporaleInizio = Validita_Inizio
            objParametri.FinestraTemporaleFine = Validita_Fine

            Dim DTAppezza = objAppezza.Leggi(Piva, Sa_Cod, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)
            For Each rowAppezza In DTAppezza.Rows
                Dim DTCodice = objAppezza_Codici.Leggi(rowAppezza("piva"),
                                                       rowAppezza("sa_cod"),
                                                       rowAppezza("appezza"),
                                                       id_cod, "",
                                                       enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                       "", "", objParametri)

                If DTCodice.Rows.Count > 0 Then
                    Return True
                End If
            Next

            Dim DTImpianto = objImpianto.Leggi(Piva, Sa_Cod, 0, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)
            For Each rowImpianto In DTImpianto.Rows
                Dim DTCodice = objImpianto_Codici.Leggi(rowImpianto("piva"),
                                                       rowImpianto("sa_cod"),
                                                       rowImpianto("appezza"),
                                                       rowImpianto("id_reg"),
                                                       "", id_cod, "",
                                                       enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)

                If DTCodice.Rows.Count > 0 Then
                    Return True
                End If
            Next

            Dim DTProgetti = objImprese_Progetti.Leggi(Piva, 0, "", 0, Sa_Cod, 0, 0, 0, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)
            For Each rowImpianto In DTProgetti.Rows
                Dim DTCodice = objImpianto_Codici.Leggi(rowImpianto("piva"),
                                                       rowImpianto("sa_cod"),
                                                       rowImpianto("appezza"),
                                                       rowImpianto("id_reg"),
                                                       "", id_cod, "",
                                                       enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)

                If DTCodice.Rows.Count > 0 Then
                    Return True
                End If
            Next


            objParametri.FinestraTemporaleInizio = Finetra_Temporale_Inizio_BK
            objParametri.FinestraTemporaleFine = Finetra_Temporale_Fine_BK

        Catch ex As Exception

            response = True
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try


        Return response
    End Function

    Public Function Leggi_Impianti_Anagrafica(ByVal Piva As String,
                                              ByVal Sa_Cod As Integer,
                                              ByVal Appezza As Integer,
                                              ByVal IdReg As Integer,
                                              ByVal Leggi_Distinte As Boolean,
                                              ByVal data As Date,
                                              ByVal filtroData As Boolean,
                                              ByVal Leggi_Cartografia As Boolean,
                                              ByRef objParametri_Super_Server As AgronicaCoreParametri,
                                              ByRef objParametri_Server As AgronicaCoreParametri,
                                              ByRef objParametri_Utenti As AgronicaCoreParametri
                                              ) As List(Of AgronicaCoreModelsSTD.anagrafiche.Impianto)

        Dim impianti As New List(Of AgronicaCoreModelsSTD.anagrafiche.Impianto)

        If Piva <> "" AndAlso Sa_Cod <> 0 AndAlso Appezza <> 0 Then
            Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
            Dim appInizio As Date = objParametri_Server.FinestraTemporaleInizio
            Dim appFine As Date = objParametri_Server.FinestraTemporaleFine

            If filtroData Then
                objParametri_Server.FinestraTemporaleInizio = data
                objParametri_Server.FinestraTemporaleFine = data
            Else
                objParametri_Server.FinestraTemporaleInizio = CDate(AGRODATAINIZIO)
                objParametri_Server.FinestraTemporaleFine = CDate(AGRODATAFINE)
            End If


            Dim DT_Impianti = objImpianti.Leggi(
                Piva,
                Sa_Cod,
                Appezza,
                IdReg,
                enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                "",
                " Reg_Impianti.Validita_Inizio ASC ",
                objParametri_Server
                )

            objParametri_Server.FinestraTemporaleInizio = CDate(appInizio)
            objParametri_Server.FinestraTemporaleFine = CDate(appFine)

            Dim leggiStaticMap As Boolean = False
            Dim LetturaCFG As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim jSonStaticMapCFG As String = LetturaCFG.Leggi_Valore(0, "GIS_StaticMapCFG", "", "", objParametri_Server)

            If jSonStaticMapCFG <> "" Then
                Dim StaticMapCFG As GeneraMappaStaticaInData = JsonConvert.DeserializeObject(Of GeneraMappaStaticaInData)(jSonStaticMapCFG)
                If StaticMapCFG.StaticMapAttive Then
                    leggiStaticMap = True
                End If
            End If

            For Each row In DT_Impianti.Rows
                Dim imp = Leggi_Impianto_Anagrafica(
                    row("piva"),
                    row("sa_cod"),
                    row("appezza"),
                    row("Id_Reg"),
                    Leggi_Distinte,
                    leggiStaticMap,
                    data,
                    filtroData,
                    Leggi_Cartografia,
                    objParametri_Super_Server,
                    objParametri_Server,
                    objParametri_Utenti
                    )

                ' Verifica se l'impianto è stato trovato
                If imp.validita IsNot Nothing Then
                    impianti.Add(imp)
                End If
            Next

        End If

        Return impianti
    End Function

    Public Function Leggi_Impianto_Anagrafica(ByVal Piva As String,
                                              ByVal Sa_Cod As Integer,
                                              ByVal Appezza As Integer,
                                              ByVal Id_Reg As Integer,
                                              ByVal Leggi_Distinte As Boolean,
                                              ByVal LeggiStaticMap As Boolean,
                                              ByVal data As Date,
                                              ByVal filtroData As Boolean,
                                              ByVal Leggi_Cartografia As Boolean,
                                              ByRef objParametri_Super_Server As AgronicaCoreParametri,
                                              ByRef objParametri_Server As AgronicaCoreParametri,
                                              ByRef objParametri_Utenti As AgronicaCoreParametri
                                              ) As AgronicaCoreModelsSTD.anagrafiche.Impianto

        Dim impianto As New AgronicaCoreModelsSTD.anagrafiche.Impianto(
            New AgronicaCoreModelsSTD.anagrafiche.Impianto.PK(
            Id_Reg,
            New AgronicaCoreModelsSTD.anagrafiche.Appezzamento.PK(
            Appezza,
            New AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK(Sa_Cod, Piva)
            )
            )
            )

        If Piva <> "" AndAlso Sa_Cod <> 0 AndAlso Appezza <> 0 AndAlso Id_Reg <> 0 Then
            Dim obj_imp_cod As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R

            Dim dtcodice = obj_imp_cod.Leggi(Piva, Sa_Cod, Appezza, Id_Reg, "", enum_CodiciAnagrafe.Codice_Impianto, "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
            If dtcodice.Rows.Count > 0 Then
                impianto.codiceImpianto = dtcodice.Rows(0).Item("val_cod")
            End If

            Dim objRegImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
            Dim DtRegImpianti As DataTable

            Dim objAppCodici As New AgronicaCoreAnagrafeDAL.Appezzamento_Codici_R
            Dim objCodici As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R

            If Leggi_Cartografia Then
                LeggiStaticMap = True
            End If

            DtRegImpianti = objRegImpianti.Leggi(
                Piva,
                Sa_Cod,
                Appezza,
                Id_Reg,
                enumSelezioneVariabile.Selezione_JoinCompleta,
                "",
                "",
                objParametri_Server,
                LeggiStaticMap:=LeggiStaticMap
                )

            If DtRegImpianti IsNot Nothing AndAlso DtRegImpianti.Rows.Count > 0 Then

                ''' TODO CONSOCIAZIONE
                If CInt(DtRegImpianti.Rows(0).Item("Id_Consociazione")) > 0 Then
                    impianto.consociazionePK = New AgronicaCoreModelsSTD.anagrafiche.Impianto.PK(CInt(DtRegImpianti.Rows(0).Item("Id_Consociazione")),
                                                                           New AgronicaCoreModelsSTD.anagrafiche.Appezzamento.PK() With {
                                                                                .codice = 0,
                                                                                .centroAziendalePK = New AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK() With {
                                                                                        .codice = 0,
                                                                                        .partitaIva = ""
                                                                                    }
                                                                                }
                                                                           )
                    impianto.impiantoConsociato = True
                Else
                    impianto.impiantoConsociato = False
                End If

                impianto.superficie = DtRegImpianti.Rows(0).Item("sup_imp")
                Dim unitaMisuraAlternativa = 0
                Dim unitaMisuraAlternativa_Des = ""
                Dim tasso_conv As Double = 0
                impianto.superficieAlternativa = If(IsDBNull(DtRegImpianti.Rows(0).Item("SUP_ALT")), 0, DtRegImpianti.Rows(0).Item("SUP_ALT"))

                If Not IsDBNull(DtRegImpianti.Rows(0).Item("UDM_COD_ALT")) Then
                    unitaMisuraAlternativa = DtRegImpianti.Rows(0).Item("UDM_COD_ALT")
                    unitaMisuraAlternativa_Des = DtRegImpianti.Rows(0).Item("UDM_DES_ALT")
                    tasso_conv = DtRegImpianti.Rows(0).Item("tasso_conv")
                End If
                impianto.unitaMisuraAlternativa = New UnitaDiMisura_Alternativa(unitaMisuraAlternativa) With {
                    .descrizione = unitaMisuraAlternativa_Des,
                    .tassoConversione = tasso_conv
                }

                If Not IsNothing(DtRegImpianti.Rows(0).Item("UDM_DES_ALT")) AndAlso Not IsDBNull(DtRegImpianti.Rows(0).Item("UDM_DES_ALT")) Then
                    impianto.unitaMisuraAlternativa.descrizione = DtRegImpianti.Rows(0).Item("UDM_DES_ALT")
                End If

                If Not IsNothing(DtRegImpianti.Rows(0).Item("UDM_SIM_ALT")) AndAlso Not IsDBNull(DtRegImpianti.Rows(0).Item("UDM_SIM_ALT")) Then
                    impianto.unitaMisuraAlternativa.simbolo = DtRegImpianti.Rows(0).Item("UDM_SIM_ALT")
                End If

                impianto.validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(DtRegImpianti.Rows(0).Item("Validita_Inizio"), DtRegImpianti.Rows(0).Item("Validita_Fine"))

                impianto.data_Creazione = AGRODATAINIZIO
                If Not IsDBNull(DtRegImpianti.Rows(0).Item("Data_Creazione")) Then
                    impianto.data_Creazione = DtRegImpianti.Rows(0).Item("Data_Creazione")
                End If

                impianto.data_Inizio_Produzione = AGRODATAINIZIO
                If Not IsDBNull(DtRegImpianti.Rows(0).Item("Data_Inizio_Produzione")) Then
                    impianto.data_Inizio_Produzione = DtRegImpianti.Rows(0).Item("Data_Inizio_Produzione")
                End If

                impianto.data_Inizio_Impianto = AGRODATAINIZIO
                If Not IsDBNull(DtRegImpianti.Rows(0).Item("Data_Inizio_Impianto")) Then
                    impianto.data_Inizio_Impianto = DtRegImpianti.Rows(0).Item("Data_Inizio_Impianto")
                End If

                impianto.data_Innesto_Varieta = AGRODATAINIZIO
                If Not IsDBNull(DtRegImpianti.Rows(0).Item("Data_Inizio_Innesto")) Then
                    impianto.data_Innesto_Varieta = DtRegImpianti.Rows(0).Item("Data_Inizio_Innesto")
                End If

                impianto.data_Inizio_Portinnesto = AGRODATAINIZIO
                If Not IsDBNull(DtRegImpianti.Rows(0).Item("data_Inizio_Portinnesto")) Then
                    impianto.data_Inizio_Portinnesto = DtRegImpianti.Rows(0).Item("data_Inizio_Portinnesto")
                End If

                If Not IsDBNull(DtRegImpianti.Rows(0).Item("Piante_Maschi_InSesto")) Then
                    impianto.maschi_in_Sesto = DtRegImpianti.Rows(0).Item("Piante_Maschi_InSesto")
                End If

                If LeggiStaticMap Then
                    impianto.immagineBase64 = DtRegImpianti.Rows(0)("StaticMapBase64String")
                End If

                If Leggi_Cartografia AndAlso DtRegImpianti.Columns.Contains("cartografia") Then
                    impianto.cartografia = DtRegImpianti.Rows(0)("cartografia")
                End If

                If Leggi_Cartografia AndAlso DtRegImpianti.Columns.Contains("SuperficieGis") Then
                    impianto.superficieGis = DtRegImpianti.Rows(0)("SuperficieGis")
                End If

                impianto.algoritmoCodifica = Replica_GIAS.LeggiAlgoritmoCodifica(Piva, objParametri_Server)

                If DtRegImpianti.Rows(0).Item("Cul_cod") <> 0 Then
                    impianto.utilizzoTerreno = New AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta(DtRegImpianti.Rows(0).Item("Cul_cod")) With {
                        .descrizione = DtRegImpianti.Rows(0).Item("Cul_Des"),
                        .specie = New AgronicaCoreModelsSTD.metaschema.utilizzi.Specie(DtRegImpianti.Rows(0).Item("Veg_Cod")) With {
                            .descrizione = DtRegImpianti.Rows(0).Item("Veg_Des")
                        }
                    }
                End If

                If DtRegImpianti.Rows(0).Item("cover") = 0 Then
                    impianto.cover_Crops = False
                Else
                    impianto.cover_Crops = True
                End If

                If DtRegImpianti.Rows(0).Item("monitorato") = 0 Then
                    impianto.monitorato = False
                Else
                    impianto.monitorato = True
                End If

                impianto.copertura = New AgronicaCoreModelsSTD.metaschema.Copertura(0) With {.descrizione = ""}
                If Not IsDBNull(DtRegImpianti.Rows(0).Item("cop_cod")) Then
                    impianto.copertura = New AgronicaCoreModelsSTD.metaschema.Copertura(DtRegImpianti.Rows(0).Item("cop_cod")) With {.descrizione = DtRegImpianti.Rows(0).Item("cop_des")}
                End If

                impianto.cop_Data_Inizio = AGRODATAINIZIO
                If Not IsDBNull(DtRegImpianti.Rows(0).Item("cop_di")) Then
                    impianto.cop_Data_Inizio = DtRegImpianti.Rows(0).Item("cop_di")
                End If

                impianto.cop_Data_Fine = AGRODATAFINE
                If Not IsDBNull(DtRegImpianti.Rows(0).Item("cop_df")) Then
                    impianto.cop_Data_Fine = DtRegImpianti.Rows(0).Item("cop_df")
                End If

                impianto.seminaTrapianto = New AgronicaCoreModelsSTD.metaschema.SeminaTrapianto("-1") With {.descrizione = ""}
                If Not IsDBNull(DtRegImpianti.Rows(0).Item("setup_cod")) Then
                    Select Case CStr(DtRegImpianti.Rows(0).Item("setup_cod"))
                        Case "Trapiantato"
                            impianto.seminaTrapianto = New AgronicaCoreModelsSTD.metaschema.SeminaTrapianto("Trapiantato") With {.descrizione = "Trapiantato"}
                        Case "Seminato"
                            impianto.seminaTrapianto = New AgronicaCoreModelsSTD.metaschema.SeminaTrapianto("Seminato") With {.descrizione = "Seminato"}
                        Case Else
                            impianto.seminaTrapianto = New AgronicaCoreModelsSTD.metaschema.SeminaTrapianto("") With {.descrizione = ""}
                    End Select
                End If

                impianto.tecnicaConduzioneTraFila = New AgronicaCoreModelsSTD.metaschema.DensitaImpianto.TecnicaConduzioneTraFila(0) With {.descrizione = ""}
                If Not IsDBNull(DtRegImpianti.Rows(0).Item("tecn_cod")) Then
                    Dim objConduzioneTraFila As New AgronicaCoreMetaSchemaDAL.ConduzioneTraFila_R
                    Dim dt = objConduzioneTraFila.Leggi(DtRegImpianti.Rows(0).Item("tecn_cod"), 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                    impianto.tecnicaConduzioneTraFila = New AgronicaCoreModelsSTD.metaschema.DensitaImpianto.TecnicaConduzioneTraFila(dt.Rows(0).Item("tecn_cod")) With {.descrizione = dt.Rows(0).Item("tecn_des")}
                End If

                impianto.tecnicaConduzioneSuFila = New AgronicaCoreModelsSTD.metaschema.DensitaImpianto.TecnicaConduzioneSuFila(0) With {.descrizione = ""}
                If Not IsDBNull(DtRegImpianti.Rows(0).Item("su_cod")) Then
                    Dim objConduzioneSuFila As New AgronicaCoreMetaSchemaDAL.ConduzioneSuFila_R
                    Dim dt = objConduzioneSuFila.Leggi(DtRegImpianti.Rows(0).Item("su_cod"), 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                    impianto.tecnicaConduzioneSuFila = New AgronicaCoreModelsSTD.metaschema.DensitaImpianto.TecnicaConduzioneSuFila(dt.Rows(0).Item("tecn_cod")) With {.descrizione = dt.Rows(0).Item("tecn_des")}
                End If

                impianto.gruppoFinalita = New AgronicaCoreModelsSTD.metaschema.utilizzi.GruppoFinalita(0) With {.descrizione = ""}
                If Not IsDBNull(DtRegImpianti.Rows(0).Item("Grfi_cod")) Then
                    impianto.gruppoFinalita = New AgronicaCoreModelsSTD.metaschema.utilizzi.GruppoFinalita(DtRegImpianti.Rows(0).Item("Grfi_cod")) With {.descrizione = DtRegImpianti.Rows(0).Item("Grfi_des")}
                    If DtRegImpianti.Rows(0).Item("Cul_cod") <> 0 Then
                        impianto.gruppoFinalita.specieCod = CType(impianto.utilizzoTerreno, Varieta).specie.codice
                    End If
                End If

                'Imp_Des
                impianto.irrigazione = New AgronicaCoreModelsSTD.metaschema.Irrigazione(0) With {.descrizione = ""}
                If Not IsDBNull(DtRegImpianti.Rows(0).Item("Imp_Cod")) Then
                    impianto.irrigazione = New AgronicaCoreModelsSTD.metaschema.Irrigazione(DtRegImpianti.Rows(0).Item("Imp_Cod")) With {.descrizione = DtRegImpianti.Rows(0).Item("Imp_Des")}
                End If

                ' macchina irrigazione impianto
                impianto.macchineIrrigazione = New List(Of ParcoMacchine)
                If Not IsDBNull(DtRegImpianti.Rows(0).Item("flagImpiantoIsMacchina")) AndAlso CInt(DtRegImpianti.Rows(0).Item("flagImpiantoIsMacchina")) = 1 Then
                    Dim objImpiantiMacchine As New AgronicaCoreAnagrafeDAL.Reg_ImpiantiXParcoMacchine_R
                    Dim dtImpiantiMacchine = objImpiantiMacchine.ReadJoinDescriptions(objParametri_Server, Piva, Sa_Cod, Appezza, Id_Reg)
                    If dtImpiantiMacchine IsNot Nothing AndAlso dtImpiantiMacchine.Rows.Count > 0 Then
                        impianto.macchineIrrigazione.Add(
                            New ParcoMacchine With {
                                .codice = dtImpiantiMacchine.Rows(0).Item("Mac_Cod"),
                                .descrizione = dtImpiantiMacchine.Rows(0).Item("Mac_Des")
                            }
                        )
                    End If
                End If

                impianto.gruppoVarietale = New AgronicaCoreModelsSTD.metaschema.utilizzi.GruppoVarietale(DtRegImpianti.Rows(0).Item("grva_cod_veg"), DtRegImpianti.Rows(0).Item("Grva_Des"))

                impianto.portinnesto = New AgronicaCoreModelsSTD.metaschema.DensitaImpianto.Portinnesto(0) With {.descrizione = ""}
                If Not IsDBNull(DtRegImpianti.Rows(0).Item("port_cod")) AndAlso CInt(DtRegImpianti.Rows(0).Item("port_cod")) > 0 Then
                    Dim objPortinnesti As New AgronicaCoreMetaSchemaDAL.Portinnesti
                    Dim dt = objPortinnesti.Leggi(DtRegImpianti.Rows(0).Item("port_cod"), "", "", objParametri_Server)
                    impianto.portinnesto = New AgronicaCoreModelsSTD.metaschema.DensitaImpianto.Portinnesto(DtRegImpianti.Rows(0).Item("port_cod")) With {.descrizione = dt.Rows(0).Item("port_des")}
                ElseIf Not IsDBNull(DtRegImpianti.Rows(0).Item("port_cod")) AndAlso CInt(DtRegImpianti.Rows(0).Item("port_cod")) < 1 Then
                    impianto.portinnesto = New AgronicaCoreModelsSTD.metaschema.DensitaImpianto.Portinnesto(DtRegImpianti.Rows(0).Item("port_cod")) With {.descrizione = ""}
                End If

                impianto.formaAllevamento = New AgronicaCoreModelsSTD.metaschema.DensitaImpianto.FormaAllevamento(0) With {.descrizione = ""}
                If Not IsDBNull(DtRegImpianti.Rows(0).Item("foral_cod")) AndAlso CInt(DtRegImpianti.Rows(0).Item("foral_cod")) > 0 Then
                    Dim objFormaAllevamento As New AgronicaCoreMetaSchemaDAL.FormeAllevamento
                    Dim dt = objFormaAllevamento.Leggi(DtRegImpianti.Rows(0).Item("foral_cod"), "", "", objParametri_Server)
                    impianto.formaAllevamento = New AgronicaCoreModelsSTD.metaschema.DensitaImpianto.FormaAllevamento(dt.Rows(0).Item("foral_cod")) With {.descrizione = dt.Rows(0).Item("foral_des")}
                End If

                Dim provenienzaSeme = New AgronicaCoreModelsSTD.metaschema.ProvenienzaSeme(0) With {.descrizione = ""}
                Select Case DtRegImpianti.Rows(0).Item("ProvenienzaSeme")
                    Case 0
                        impianto.provenienzaSeme = New AgronicaCoreModelsSTD.metaschema.ProvenienzaSeme(DtRegImpianti.Rows(0).Item("ProvenienzaSeme")) With {
                            .descrizione = ""
                        }
                    Case 1
                        impianto.provenienzaSeme = New AgronicaCoreModelsSTD.metaschema.ProvenienzaSeme(DtRegImpianti.Rows(0).Item("ProvenienzaSeme")) With {
                            .descrizione = "Biologica"
                        }
                    Case 2
                        impianto.provenienzaSeme = New AgronicaCoreModelsSTD.metaschema.ProvenienzaSeme(DtRegImpianti.Rows(0).Item("ProvenienzaSeme")) With {
                            .descrizione = "Integrato"
                        }
                    Case 3
                        impianto.provenienzaSeme = New AgronicaCoreModelsSTD.metaschema.ProvenienzaSeme(DtRegImpianti.Rows(0).Item("ProvenienzaSeme")) With {
                            .descrizione = "Deroga"
                        }
                End Select

                impianto.unita_Vitata = 0
                If Not IsNothing(DtRegImpianti.Rows(0).Item("Unita_Vitata")) Then
                    impianto.unita_Vitata = DtRegImpianti.Rows(0).Item("Unita_Vitata")
                End If


                'lavez - 21/03/2024 - chiavi nuovo tracciato agea
                If Not IsDBNull(DtRegImpianti.Rows(0).Item("Agea_idColt")) Then
                    impianto.Agea_idColt = DtRegImpianti.Rows(0).Item("Agea_idColt")
                End If


                Dim objcodAn As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R
                Dim StrCodiciImpianto = objcodAn.Filtro_Codici_Anagrafe(4, 3, 2, objParametri_Server)
                'Elimino il codice Titolo Possesso, Metodo Produzione, Magazzino Conferimento,
                'Organismo Referente, Capitolato Privato e Dettaglio Specie Personalizzato 
                'perché già presenti
                StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " & CStr(enum_CodiciAnagrafe.TitoloPossesso), "")
                StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " & CStr(enum_CodiciAnagrafe.TitoloPossesso) & " Or ", "")
                StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " & CStr(enum_CodiciAnagrafe.MetodoDiProduzione), "")
                StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " & CStr(enum_CodiciAnagrafe.MetodoDiProduzione) & " Or ", "")
                StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " & CStr(enum_CodiciAnagrafe.Riferimento_Trasferimento_Dati), "")
                StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " & CStr(enum_CodiciAnagrafe.Riferimento_Trasferimento_Dati) & " Or ", "")
                StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " & CStr(enum_CodiciAnagrafe.Organismo_Referente), "")
                StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " & CStr(enum_CodiciAnagrafe.Organismo_Referente) & " Or ", "")
                StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " & CStr(enum_CodiciAnagrafe.Capitolato_Privato), "")
                StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " & CStr(enum_CodiciAnagrafe.Capitolato_Privato) & " Or ", "")
                StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " & CStr(enum_CodiciAnagrafe.Zespri_Fasi_Fase), "")
                StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " & CStr(enum_CodiciAnagrafe.Zespri_Fasi_Fase) & " Or ", "")
                StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " & CStr(enum_CodiciAnagrafe.Dettaglio_Specie_Personalizzato), "")
                StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " & CStr(enum_CodiciAnagrafe.Dettaglio_Specie_Personalizzato) & " Or ", "")
                StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " & CStr(enum_CodiciAnagrafe.Magazzino_Conferimento), "")
                StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " & CStr(enum_CodiciAnagrafe.Magazzino_Conferimento) & " Or ", "")
                StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " & CStr(enum_CodiciAnagrafe.Data_Inizio_Portinnesto), "")
                StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " & CStr(enum_CodiciAnagrafe.Data_Inizio_Portinnesto) & " Or ", "")
                StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " & CStr(enum_CodiciAnagrafe.Impianto_Nr_domanda_ACA), "")
                StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " & CStr(enum_CodiciAnagrafe.Impianto_Nr_domanda_ACA) & " Or ", "")

                Dim FiltroLetturaCodici As String = ""

                FiltroLetturaCodici += " ( " &
                " Reg_Impianti_Codici.Id_Cod NOT IN ( " &
                CStr(enum_CodiciAnagrafe.Codice_Specie_Agea) & ", " &
                CStr(enum_CodiciAnagrafe.Codice_Cultivar_Agea) & ", " &
                CStr(enum_CodiciAnagrafe.Coltura_Precedente_1) & ", " &
                CStr(enum_CodiciAnagrafe.Coltura_Precedente_2) & ", " &
                CStr(enum_CodiciAnagrafe.Coltura_Precedente_3) & ", " &
                CStr(enum_CodiciAnagrafe.Coltura_Precedente_4) & " " &
                "   ) " &
                " )"

                FiltroLetturaCodici &= " AND Reg_Impianti_Codici.progetto_cod = 0  " &
                                " AND (Reg_Impianti_Codici.id_cod < 2000 OR Reg_Impianti_Codici.id_cod >= 3000 ) "


                Dim DtCodici = objCodici.Leggi(Piva, Sa_Cod, Appezza, Id_Reg,
                                               "", 0, "",
                                               enumSelezioneVariabile.Selezione_TabellaCompleta,
                                               FiltroLetturaCodici, "",
                                               objParametri_Server)

                Dim StrCodici2 As String = ""

                Dim Codici As New List(Of AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori)

                impianto.impianto_Ibrido = False
                impianto.codBMBDBT_M = ""
                impianto.codBMBDBT_F = ""
                impianto.genetica_M = ""
                impianto.genetica_F = ""
                impianto.offType_M = ""
                impianto.offType_F = ""
                impianto.tra_Fila_M = 0
                impianto.distanzaTraFila_F = 0
                impianto.su_Fila_M = 0
                impianto.distanzaSuFila_F = 0
                impianto.interbina = 0
                impianto.germinabilita = 0
                impianto.codiceZona = New baseClass.BaseCodeDescrStr("", "")
                impianto.tagliatoIntero = New AgronicaCoreModelsSTD.metaschema.TagliatoIntero("") With {.descrizione = ""}
                impianto.partiTuberi = 0
                impianto.dettaglio_varieta_personalizzato = New AgronicaCoreModelsSTD.metaschema.DettaglioVarietaPersonalizzato(0) With {.descrizione = ""}

                If DtCodici IsNot Nothing AndAlso DtCodici.Rows.Count > 0 Then

                    For i = 0 To DtCodici.Rows.Count - 1

                        'If (Not IsDBNull(Dr(i).Item("val_cod"))) Then
                        If (Not IsDBNull(DtCodici.Rows(i).Item("val_cod"))) Then

                            Dim Id_Cod = DtCodici.Rows(i).Item("id_cod")
                            Dim Val_Cod = DtCodici.Rows(i).Item("val_cod")
                            Dim CodiceAnagrafeDes = DtCodici.Rows(i).Item("descrizione")

                            Select Case Id_Cod

                                Case enum_CodiciAnagrafe.Impianto_Ibrido
                                    impianto.impianto_Ibrido = False
                                    If DtRegImpianti.Rows(0).Item("grva_cod_veg") < 0 Then
                                        impianto.impianto_Ibrido = True
                                    End If
                                Case enum_CodiciAnagrafe.Impianto_CodiceB_Maschio
                                    impianto.codBMBDBT_M = Val_Cod

                                Case enum_CodiciAnagrafe.Impianto_CodiceB_Femmina
                                    impianto.codBMBDBT_F = Val_Cod

                                Case enum_CodiciAnagrafe.Impianto_Genetica_Maschio
                                    impianto.genetica_M = Val_Cod

                                Case enum_CodiciAnagrafe.Impianto_Genetica_Femmina
                                    impianto.genetica_F = Val_Cod

                                Case enum_CodiciAnagrafe.Impianto_OffType_Maschio
                                    impianto.offType_M = Val_Cod

                                Case enum_CodiciAnagrafe.Impianto_OffType_Femmina
                                    impianto.offType_F = Val_Cod

                                Case enum_CodiciAnagrafe.Impianto_TraFila_Maschio
                                    If Val_Cod <> "" Then
                                        impianto.tra_Fila_M = Val_Cod
                                    End If

                                Case enum_CodiciAnagrafe.Impianto_TraFila_Femmina
                                    If Val_Cod <> "" Then
                                        impianto.distanzaTraFila_F = Val_Cod
                                    End If

                                Case enum_CodiciAnagrafe.Impianto_SuFila_Maschio
                                    If Val_Cod <> "" Then
                                        impianto.su_Fila_M = Val_Cod
                                    End If

                                Case enum_CodiciAnagrafe.Impianto_SuFila_Femmina
                                    If Val_Cod <> "" Then
                                        impianto.distanzaSuFila_F = Val_Cod
                                    End If

                                Case enum_CodiciAnagrafe.Impianto_Interbina
                                    If Val_Cod <> "0" And Val_Cod <> "" Then
                                        impianto.interbina = Val_Cod
                                    End If

                                Case enum_CodiciAnagrafe.Impianto_Germinabilita
                                    impianto.germinabilita = Val_Cod

                                Case enum_CodiciAnagrafe.CodiceZona
                                    If IsNumeric(Val_Cod) Then

                                        Dim objMateriePrime As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
                                        Dim dtProdotto = objMateriePrime.Leggi(Piva, 0, 0, Val_Cod, "", 0, 0, 0, 0, 0, 0, 0, "", 0, "", True, False, "", enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
                                        If dtProdotto.Rows.Count > 0 Then
                                            impianto.codiceZona = New baseClass.BaseCodeDescrStr(Val_Cod, dtProdotto(0)("Mat_Des"))
                                        End If

                                    End If

                                Case 3000 To 3999

                                    If impianto.utilizzoTerreno IsNot Nothing Then
                                        Throw New Exception("Impianto con Utilizzo Terreno " & CStr(Id_Cod) & " - " & CodiceAnagrafeDes &
                                                        " e Cultivar " & CStr(impianto.utilizzoTerreno.descrizione))
                                    End If

                                    impianto.utilizzoTerreno = New AgronicaCoreModelsSTD.metaschema.utilizzi.DestinazioneUso(Id_Cod) With {
                                    .descrizione = CodiceAnagrafeDes
                                }
                                    impianto.utilizzoTerreno.classType = ClassType.DestinazioneUso

                                Case enum_CodiciAnagrafe.Impianto_Taglio_Tuberi_Patate

                                    impianto.tagliatoIntero.codice = Val_Cod

                                Case enum_CodiciAnagrafe.Impianto_Parti_Tuberi_Patate

                                    impianto.partiTuberi = Val_Cod

                                Case enum_CodiciAnagrafe.Dettaglio_Specie_Personalizzato

                                    If Val_Cod IsNot Nothing AndAlso Val_Cod <> "" Then
                                        Dim objCAC As New AgronicaCoreAnagrafeDAL.CAC_Codifica_InfoAggiuntive_R

                                        Dim Dt = objCAC.Leggi(2, Val_Cod, 0, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

                                        If Dt.Rows.Count > 0 Then
                                            impianto.dettaglio_varieta_personalizzato = New AgronicaCoreModelsSTD.metaschema.DettaglioVarietaPersonalizzato(Val_Cod) With {
                                            .descrizione = Dt.Rows(0)("InfoAgg_Des")
                                        }
                                        End If
                                    End If

                                Case Else

                                    'se nella stringa contenente i codici restituita dal componente è presente..
                                    If InStr(StrCodiciImpianto, Id_Cod.ToString) <> 0 Then

                                        Dim Codice As New AnagrafeNG.Codici

                                        Codice.Id_Cod = Id_Cod
                                        Codice.Descrizione = CodiceAnagrafeDes
                                        Codice.Val_Cod = Val_Cod
                                        Codice.Validita_Inizio = DtCodici.Rows(i).Item("Validita_Inizio")
                                        Codice.Validita_Fine = DtCodici.Rows(i).Item("Validita_Fine")

                                        Codici.Add(New AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori() With {
                                                    .valore = Val_Cod,
                                                    .validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(
                                                                DtCodici.Rows(i).Item("Validita_Inizio"),
                                                                DtCodici.Rows(i).Item("Validita_Fine")),
                                                    .codiceAnagrafe = New AgronicaCoreModelsSTD.anagrafiche.CodiceAnagrafe(Id_Cod) With {
                                                        .descrizione = CodiceAnagrafeDes
                                                    }
                                                })
                                    End If
                            End Select
                        End If
                    Next

                    impianto.codici = Codici

                End If

                If impianto.utilizzoTerreno Is Nothing Then
                    impianto.utilizzoTerreno = New AgronicaCoreModelsSTD.metaschema.utilizzi.DestinazioneUso(-1) With {
                                    .descrizione = Str_TerrenoNudo
                                }
                End If

                If Leggi_Distinte Then
                    Dim objProgetti_R As New Progetto_R
                    Dim specie = New Specie(0)

                    If (impianto.utilizzoTerreno IsNot Nothing) Then
                        If (impianto.utilizzoTerreno.classType = ClassType.Varieta) Then
                            specie = CType(impianto.utilizzoTerreno, Varieta).specie
                        End If
                    End If

                    impianto.esercizi = objProgetti_R.Leggi_Esercizi_Anagrafica(
                        Piva,
                        Sa_Cod,
                        Appezza,
                        Id_Reg,
                        specie,
                        data,
                        filtroData,
                        objParametri_Super_Server,
                        objParametri_Server,
                        objParametri_Utenti
                        )
                End If

            End If
        End If

        Return impianto
    End Function

    Public Sub LeggiImpianti_QdC(ByRef Dt As DataTable,
                                   ByVal Sa_Cod As Integer,
                                   ByVal Veg_Cod As Integer,
                                   ByVal Piva As String,
                                   ByVal Data As Date,
                                   ByVal consideraTerrenoNudo As Boolean,
                                   ByVal dettagliTerrenoNudo As Boolean,
                                   ByVal Dest_Cod As Integer,
                                   ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                   ByVal Dpi_Cod As Integer,
                                   ByVal Grfi_Cod As Integer,
                                   ByVal Flag_Protetto As Integer,
                                   ByVal Flag_PubblicoPrivato As Integer,
                                   ByVal Lav_Cod_List As List(Of Integer),
                                   ByVal TipoRicetta As Integer,
                                   ByVal Id_Agenda_List As List(Of Integer),
                                   ByVal Ricetta_Operazione_Cod_List As List(Of Integer),
                                   ByVal Campo_Cod As Integer,
                                   ByVal TipoAttivita As Integer,
                                   ByVal Stato As Integer,
                                   ByVal objParametri_Server As AgronicaCoreParametri,
                                   ByVal objParametri_Utenti As AgronicaCoreParametri,
                                   ByVal objParametri_Super_Server As AgronicaCoreParametri)

        Dim leggiLingua As New Lingue_Read
        Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod,
                                                                  enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                  "", "", objParametri_Server)

        Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
        Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

        Dim j As Integer = 0
        Dim ColturaProtetta As String = ""

        '----- Recupero l'elenco degli impianti

        ''------------------------------------------------------------------------------
        Dim Flag_Disciplinare As Boolean

        Dim visualizza_Kpin_BlockName As Boolean = False

        Dim visualizza_codici_imp_app_prj As Boolean = False

        Dim leggiAncheBloccati As Boolean = False

        Dim objParametriIngresso As New AgronicaCoreMetaSchemaBIZ.FasiFenologiche_input

        Dim objImpost As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

        Dim impostazioni As New List(Of Integer) From {
            enum_Impostazioni_Utenti.SuperUser_KPIN_BlockName,
            enum_Impostazioni_Utenti.SuperUser_Visualizza_Codici_Anagrafici,
            enum_Impostazioni_Utenti.SUPERUSER_COD_PERMETTI_OPERAZIONI_SU_IMPIANTI_BLOCCATI,
            enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_FASI_FENOLOGICHE
        }

        Dim utenti_impostazioni = objImpost.LeggiScalare(objParametri_Utenti.UtenteUsername, impostazioni, objParametri_Utenti)

        If utenti_impostazioni.Any() Then

            Dim utente_impostazione As Utente_Impostazioni

            utente_impostazione = utenti_impostazioni.FirstOrDefault(Function(x) x.Impostazione_Cod = enum_Impostazioni_Utenti.SuperUser_KPIN_BlockName)

            If (utente_impostazione IsNot Nothing AndAlso utente_impostazione.Valore = "1") Then
                visualizza_Kpin_BlockName = True
            End If

            utente_impostazione = utenti_impostazioni.FirstOrDefault(Function(x) x.Impostazione_Cod = enum_Impostazioni_Utenti.SuperUser_Visualizza_Codici_Anagrafici)

            If (utente_impostazione IsNot Nothing AndAlso utente_impostazione.Valore = "1") Then
                visualizza_codici_imp_app_prj = True
            End If

            utente_impostazione = utenti_impostazioni.FirstOrDefault(Function(x) x.Impostazione_Cod = enum_Impostazioni_Utenti.SUPERUSER_COD_PERMETTI_OPERAZIONI_SU_IMPIANTI_BLOCCATI)

            If (utente_impostazione IsNot Nothing AndAlso utente_impostazione.Valore = "1") Then
                leggiAncheBloccati = True
            End If

            utente_impostazione = utenti_impostazioni.FirstOrDefault(Function(x) x.Impostazione_Cod = enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_FASI_FENOLOGICHE)

            If (utente_impostazione IsNot Nothing AndAlso utente_impostazione.Valore = "1") Then
                objParametriIngresso.Personalizzate = True
            End If

        End If

        If Grfi_Cod <> 0 OrElse Flag_Protetto <> 0 OrElse Flag_PubblicoPrivato <> 0 Then
            Flag_Disciplinare = True
        Else
            Flag_Disciplinare = False
        End If

        'per ora viene messo fisso -1 metto Flag_Protetto = -1 x non filtrarlo nel caricamento degli impianti
        If Lav_Cod_List.Exists(Function(lav_cod) {LAVCOD_FERTIRRIGAZIONE, LAVCOD_CONCIMAZIONE_FOGLIARE,
                                                            LAVCOD_DISTRIBUZIONE_CONCIME, LAVCOD_DISTRIBUZIONE_AMMENDANTI,
                                                            LAVCOD_SARCHIATURA_CONCIMAZIONE, LAVCOD_TRATTAMENTO_ANTIBUTTERATURA}.Contains(lav_cod)) Then


            Flag_Protetto = -1
        End If

        Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read

        'filtro in scrittura se ho impostato il filtro dal menu agenda
        Dim filtro As String = "|"
        'TODO DA CHIEDERE COME GESTIRE LA SESSIONE
        If TipoOperazioneDB = enum_TipoOperazioneDB.Scrittura Then
            'If Not IsNothing(Session("Filtro")) AndAlso Session("Filtro") <> "" Then
            '    'No un filtro

            '    filtro = Session("Filtro")

            'End If
        End If

        'TODO DA CHIEDERE COME GESTIRE
        Dim id_cod As Integer = 0
        'If Not IsNothing(ComboSpecie) AndAlso
        '    DETTAGLITERRENONUDO AndAlso
        '    ComboSpecie.Valore_Combo.Split("/").Length = 2 AndAlso
        '    ComboSpecie.Valore_Combo.Split("/")(0) = "0" Then
        '    'terreno nudo con indicata la destinazione
        '    id_cod = CInt(ComboSpecie.Valore_Combo.Split("/")(1))
        'End If

        If dettagliTerrenoNudo AndAlso Veg_Cod = 0 Then
            'terreno nudo con indicata la destinazione
            id_cod = Dest_Cod
        End If

        Dim specie As Integer

        'Dim specie As String = objParametriAgenda.Veg_Cod.Split("/")(0)
        'If Not IsNothing(ComboSpecie) AndAlso CONSIDERATERRENONUDO Then
        '    specie = ComboSpecie.Valore_Combo.Split("/")(0)
        'Else
        '    If objParametriAgenda.Veg_Cod.Split("/")(0) = "0" Then
        '        specie = "-999"
        '    Else
        '        specie = objParametriAgenda.Veg_Cod.Split("/")(0)
        '    End If
        'End If

        If consideraTerrenoNudo Then
            specie = Veg_Cod
        Else
            If Veg_Cod = 0 Then
                specie = -999
            Else
                specie = Veg_Cod
            End If
        End If

        '----------------------------------
        'TODO DA CHIEDERE COME GESTIRE
        'If Not IsNothing(Session("PERMETTI_OPERAZIONI_SU_IMPIANTI_BLOCCATI")) Then
        '    If Session("PERMETTI_OPERAZIONI_SU_IMPIANTI_BLOCCATI") = True Then
        '        leggiAncheBloccati = True
        '    End If
        'End If

        If TipoOperazioneDB = enum_TipoOperazioneDB.Lettura Then
            leggiAncheBloccati = True
        End If

        Dim FiltroAggiuntivo As String = ""

        Dim filtroFinale As String = filtro.Split("|")(1)
        If FiltroAggiuntivo <> "" Then
            FiltroAggiuntivo = "(" & FiltroAggiuntivo & ")"
            If filtroFinale <> "" Then
                filtroFinale = filtroFinale & " AND " & FiltroAggiuntivo
            Else
                filtroFinale = FiltroAggiuntivo
            End If
        End If

        '(12/11/2018 fede) aggiunta indicazione fase fenologica corrente
        Dim objParametriUscitaFasiNew As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output
        Dim objParametriUscitaFasiOld As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output

        Dim FF_Cod_Fioritura_Old As Integer = 0
        Dim FF_Cod_Fioritura_New As Integer = 0

        Dim strFFCod As String = ""

        Dim leggiStaticMap As Boolean = False

        Dim classTess_input As New AgronicaCorePianoConcimazioneBIZ.PUA_ClassiTessitura_input

        'Recupero l'url delle FasiFenologiche da Configurazione_Siti

        Dim xLeggiConfigurazioneSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

        Dim DTConfigurazioneSiti_Super_Server As DataTable = xLeggiConfigurazioneSiti.Leggi(0, "", "", "", objParametri_Super_Server)


        If Not IsNothing(DTConfigurazioneSiti_Super_Server) AndAlso DTConfigurazioneSiti_Super_Server.Rows.Count > 0 Then

            Dim DRGiasOnline_WS_SpecieVegetali = DTConfigurazioneSiti_Super_Server.Select("Chiave = 'GiasOnline_WS_SpecieVegetali_IAgroAPI_SpecieVegetali'")

            If Not IsNothing(DRGiasOnline_WS_SpecieVegetali) AndAlso DRGiasOnline_WS_SpecieVegetali.Length = 1 Then
                If DRGiasOnline_WS_SpecieVegetali(0)("Valore") <> "" Then
                    objParametriIngresso.Url = DRGiasOnline_WS_SpecieVegetali(0)("Valore") & "/FasiFenologiche"
                End If
            End If

            Dim DRGiasOnline_WS_PianoConcimazione = DTConfigurazioneSiti_Super_Server.Select("Chiave = 'GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione'")

            If Not IsNothing(DRGiasOnline_WS_PianoConcimazione) AndAlso DRGiasOnline_WS_PianoConcimazione.Length = 1 Then
                If DRGiasOnline_WS_PianoConcimazione(0)("Valore") <> "" Then
                    classTess_input.Url = DRGiasOnline_WS_PianoConcimazione(0)("Valore")
                End If
            End If

        End If

        Dim DTConfigurazioneSiti_Server As DataTable = xLeggiConfigurazioneSiti.Leggi(0, "", "", "", objParametri_Server)

        If Not IsNothing(DTConfigurazioneSiti_Server) AndAlso DTConfigurazioneSiti_Server.Rows.Count > 0 Then

            Dim DRStaticMapCFG = DTConfigurazioneSiti_Server.Select("Chiave = 'GIS_StaticMapCFG'")

            If Not IsNothing(DRStaticMapCFG) AndAlso DRStaticMapCFG.Length = 1 Then
                Dim jSonStaticMapCFG As String = DRStaticMapCFG(0)("Valore")

                If jSonStaticMapCFG <> "" Then
                    Dim StaticMapCFG As GeneraMappaStaticaInData = JsonConvert.DeserializeObject(Of GeneraMappaStaticaInData)(jSonStaticMapCFG)
                    If StaticMapCFG.StaticMapAttive Then
                        leggiStaticMap = True
                    End If
                End If
            End If

        End If

        If IsNumeric(specie) AndAlso CInt(specie) > 0 Then

            objParametriIngresso.Veg_Cod = CInt(specie)
            objParametriIngresso.Lingua_Cod = objParametri_Server.Lingua_Cod

            Dim objFasi_WS As New AgronicaCoreWebService.FasiFenologiche_WS

            objParametriUscitaFasiNew = objFasi_WS.FasiFenologiche(objParametriIngresso)
            objParametriUscitaFasiOld = objFasi_WS.FasiFenologiche_OLD(objParametriIngresso)

            'fasi bbch x fioritura
            For f = 0 To objParametriUscitaFasiNew.ListaFasiFenologiche.Count - 1
                If objParametriUscitaFasiNew.ListaFasiFenologiche(f).Fioritura Then
                    FF_Cod_Fioritura_New = objParametriUscitaFasiNew.ListaFasiFenologiche(f).Cod_SS
                    FF_Cod_Fioritura_Old = objParametriUscitaFasiNew.ListaFasiFenologiche(f).FF_Cod
                    Exit For
                End If
            Next

        End If

        If FF_Cod_Fioritura_New <> 0 AndAlso FF_Cod_Fioritura_Old <> 0 Then
            strFFCod = " (" & FF_Cod_Fioritura_New & "," & FF_Cod_Fioritura_Old & ")"
        ElseIf FF_Cod_Fioritura_New <> 0 AndAlso FF_Cod_Fioritura_Old = 0 Then
            strFFCod = " (" & FF_Cod_Fioritura_New & ")"
        ElseIf FF_Cod_Fioritura_New = 0 AndAlso FF_Cod_Fioritura_Old <> 0 Then
            strFFCod = " (" & FF_Cod_Fioritura_Old & ")"
        Else
            strFFCod = " (-1)"
        End If

        'Fare la query per controllare se ci sono delle Dichiarazioni di Non Utilizzo se ho scelto dei Trattamenti o delle Fertilizzazioni
        Dim Controlla_Dichiarazione_Non_Utilizzo As Boolean = False

        If Lav_Cod_List.FindIndex(Function(Lav_Cod) {LAVCOD_DISTRIBUZIONE_CONCIME, LAVCOD_SARCHIATURA_CONCIMAZIONE, LAVCOD_DISTRIBUZIONE_AMMENDANTI, LAVCOD_CONCIMAZIONE_FOGLIARE, LAVCOD_FERTIRRIGAZIONE, LAVCOD_TRATTAMENTO_ANTIBUTTERATURA}.Contains(Lav_Cod) OrElse
                                                    {LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_DISERBO, LAVCOD_DISSECCAMENTO, LAVCOD_GEODISINFESTAZIONE, LAVCOD_TRATTAMENTO_FITOREGOLATORE}.Contains(Lav_Cod)) > -1 Then

            Controlla_Dichiarazione_Non_Utilizzo = True

        End If

        'lettura impianti
        Dt = objImpianti.Leggi_Impianti_xAgenda3(Flag_Disciplinare,
                                                 Piva, Sa_Cod, Campo_Cod,
                                                 specie, 0,
                                                 Data, Grfi_Cod,
                                                 Flag_Protetto, True,
                                                 id_cod,
                                                 filtroFinale, "  App_Nome, Cul_Des, Progetto ",
                                                 objParametri_Server, leggiAncheBloccati,
                                                 visualizza_Kpin_BlockName, visualizza_codici_imp_app_prj,
                                                 strFFCod, leggiStaticMap:=leggiStaticMap, Controlla_Dichiarazione_Non_Utilizzo:=Controlla_Dichiarazione_Non_Utilizzo)
        '----------------------------------
        'lettura particelle impianti
        'Gestione precedente per la lettura della classe tessitura
        'Dim includiClasseTessitura As Boolean = TipoRicetta = enum_TipoRicetta.PianoDistribuzionePua AndAlso LAVCOD_DISTRIBUZIONE_AMMENDANTI

        Dim includiClasseTessitura As Boolean = Lav_Cod_List.Exists(Function(lav_cod) {LAVCOD_DISTRIBUZIONE_AMMENDANTI}.Contains(lav_cod))

        Dim DtParticelle As DataTable = objImpianti.Leggi_ParticelleImpianti_xAgenda2(Flag_Disciplinare,
                                                                                     Piva,
                                                                                     Sa_Cod,
                                                                                     specie,
                                                                                     0,
                                                                                     Data,
                                                                                     Grfi_Cod,
                                                                                     Flag_Protetto,
                                                                                     id_cod,
                                                                                     filtro.Split("|")(1), " App_Nome, Cul_Des, Progetto ",
                                                                                     objParametri_Server, leggiAncheBloccati, includiClasseTessitura:=includiClasseTessitura)

        '----------------------------------
        'lettura zone vulnerabili
        Dim objPV As New AgronicaCoreAnagrafeDAL.ZonexParticelle_R
        Dim DtPV As DataTable = objPV.Leggi(-17,
                                            "", "", "", 0, 0, "",
                                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                                            "",
                                            "", objParametri_Server)

        '----------------------------------------
        'lettura fasce (la metto in un try catch in caso non esista la tabella nel DB PianoConcimazione_Pua e di conseguenza la vista)
        Dim DtPVF As DataTable = Nothing
        '20/01/23 Commentata questa lettura perché l'anagrafica questo dato non lo gestisce e potrebbe causare problemi
        'Try
        '    Dim objPVF As New AgronicaCoreMetaSchemaDAL.ParticelleCatastali_Vulnerabili_R
        '    DtPVF = objPVF.Leggi("", "", "", 0, 0, "", 0,
        '                         " Fascia_Cod <>0 ",
        '                         "", objParametri_Server)
        'Catch ex As Exception

        'End Try

        '----------------------------------------
        'lettura delle analisi
        Dim classTess_output As New AgronicaCorePianoConcimazioneBIZ.PUA_ClassiTessitura_output
        If Lav_Cod_List.Exists(Function(lav_cod) {LAVCOD_DISTRIBUZIONE_AMMENDANTI}.Contains(lav_cod)) Then

            Dim pcws As New AgronicaCoreWebService.PianoConcimazione_WS
            classTess_input.Regolamento_Cod = Dpi_Cod

            classTess_output = pcws.ClassiTessitura(classTess_input)
        End If

        Dt.Columns.Add(New DataColumn("App_Nome_Anagrafica", GetType(String)))

        'aggiungo la colonna catasto
        Dt.Columns.Add(New DataColumn("catasto", GetType(String)))
        Dt.Columns.Add(New DataColumn("kendoKey", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sup_Imp_help", GetType(Double)))
        Dt.Columns.Add(New DataColumn("Descrizione_Unica", GetType(String)))

        '(31/07/2018 fede) aggiunte colonne x dati gis
        Dt.Columns.Add(New DataColumn("GisWkt", GetType(String)))
        Dt.Columns.Add(New DataColumn("GisWktGps", GetType(String)))
        Dt.Columns.Add(New DataColumn("GisWktSistemaRiferimento", GetType(String)))
        Dt.Columns.Add(New DataColumn("GisTipoEntita_cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("GisLayerCod", GetType(Integer)))

        '(12/11/2018 fede) aggiunta indicazione fase fenologica corrente
        Dt.Columns.Add(New DataColumn("Fase_Corrente", GetType(String)))

        Dt.Columns.Add(New DataColumn("Sup_Riduzione_BufferZone", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("Perc_Riduzione_Deriva", GetType(Decimal)))

        '(12/06/2019 Marco G) aggiunta classi di Tessitura
        Dt.Columns.Add(New DataColumn("Id_ClasseTessitura", GetType(String)))
        Dt.Columns.Add(New DataColumn("Str_ClasseTessitura", GetType(String)))
        Dt.Columns.Add(New DataColumn("ListaClassiTessitura", GetType(List(Of AgronicaCoreModelsSTD.anagrafiche.ClasseTessitura))))

        Dt.Columns.Add(New DataColumn("N_Max", GetType(String)))
        Dt.Columns.Add(New DataColumn("P_Max", GetType(String)))
        Dt.Columns.Add(New DataColumn("K_Max", GetType(String)))
        Dt.Columns.Add(New DataColumn("Mg_Max", GetType(String)))
        Dt.Columns.Add(New DataColumn("CU_Max", GetType(String)))

        Dt.Columns.Add(New DataColumn("N_Max_Decimal", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("P_Max_Decimal", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("K_Max_Decimal", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("Mg_Max_Decimal", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("CU_Max_Decimal", GetType(Decimal)))

        Dt.Columns.Add(New DataColumn("N_Residuo_Decimal", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("P_Residuo_Decimal", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("K_Residuo_Decimal", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("Mg_Residuo_Decimal", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("CU_Residuo_Decimal", GetType(Decimal)))

        Dt.Columns.Add(New DataColumn("N_Residuo_Percentuale", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("P_Residuo_Percentuale", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("K_Residuo_Percentuale", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("Mg_Residuo_Percentuale", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("CU_Residuo_Percentuale", GetType(Decimal)))

        Dt.Columns.Add(New DataColumn("Obj_Disciplinare", GetType(AgronicaCoreModelsSTD.metaschema.Disciplinare)))

        Dt.Columns.Add(New DataColumn("Flag_Protetto", GetType(Integer)))

        Dt.Columns.Add(New DataColumn("ZVN", GetType(String)))

        Dt.Columns.Add(New DataColumn("Disciplinare_Des", GetType(String)))

        Dt.Columns.Add(New DataColumn("CarenzaStr", GetType(String)) With {.DefaultValue = String.Empty})
        Dt.Columns.Add(New DataColumn("DataCarenza", GetType(Date)) With {.DefaultValue = AGRODATAINIZIO})
        Dt.Columns.Add(New DataColumn("Validita_Inizio_Date", GetType(Date)) With {.DefaultValue = AGRODATAINIZIO})
        Dt.Columns.Add(New DataColumn("Validita_Fine_Date", GetType(Date)) With {.DefaultValue = AGRODATAFINE})
        Dt.Columns.Add(New DataColumn("Data_Semina_Date", GetType(Date)) With {.DefaultValue = AGRODATAINIZIO})
        Dt.Columns.Add(New DataColumn("Data_Fioritura_Date", GetType(Date)) With {.DefaultValue = AGRODATAINIZIO})
        Dt.Columns.Add(New DataColumn("Data_Fioritura_Prevista_Date", GetType(Date)) With {.DefaultValue = AGRODATAINIZIO})
        Dt.Columns.Add(New DataColumn("Data_Raccolta_Prevista_Date", GetType(Date)) With {.DefaultValue = AGRODATAINIZIO})
        Dt.Columns.Add(New DataColumn("Data_Raccolta_Date", GetType(Date)) With {.DefaultValue = AGRODATAINIZIO})
        Dt.Columns.Add(New DataColumn("Data_Semina_Prevista_Date", GetType(Date)) With {.DefaultValue = AGRODATAINIZIO})
        Dt.Columns.Add(New DataColumn("Validita_Inizio_Distinta_Date", GetType(Date)) With {.DefaultValue = AGRODATAINIZIO})
        Dt.Columns.Add(New DataColumn("Validita_Fine_Distinta_Date", GetType(Date)) With {.DefaultValue = AGRODATAFINE})
        Dt.Columns.Add(New DataColumn("Validita_inizio_Appezzamento_Date", GetType(Date)) With {.DefaultValue = AGRODATAINIZIO})
        Dt.Columns.Add(New DataColumn("Validita_fine_Appezzamento_Date", GetType(Date)) With {.DefaultValue = AGRODATAFINE})

        Dt.Columns.Add(New DataColumn("IrrigazioneUtilizzata_Codice", GetType(String)) With {.DefaultValue = "0_0"})
        Dt.Columns.Add(New DataColumn("IrrigazioneUtilizzata_Descrizione", GetType(String)) With {.DefaultValue = $"--- {Gias.ImpiantoIrrigazione} --- {Gias.Nessuno}"})
        Dt.Columns.Add(New DataColumn("Consiglio_Codice", GetType(Integer)) With {.DefaultValue = 0})
        Dt.Columns.Add(New DataColumn("Consiglio_Descrizione", GetType(String)) With {.DefaultValue = Gias.Nessuno})
        Dt.Columns.Add(New DataColumn("DataConsiglio", GetType(Date)) With {.DefaultValue = AGRODATAINIZIO})
        Dt.Columns.Add(New DataColumn("DoseAcquaConsiglio", GetType(Decimal)) With {.DefaultValue = 0})
        Dt.Columns.Add(New DataColumn("UdmConsiglio", GetType(UnitaMisura)) With {.DefaultValue = Nothing})
        Dt.Columns.Add(New DataColumn("UdmDose", GetType(String)) With {.DefaultValue = "m3/Ha"})
        Dt.Columns.Add(New DataColumn("DoseAcquaGiornaliera", GetType(Decimal)) With {.DefaultValue = 0})
        Dt.Columns.Add(New DataColumn("OreIrrigazione", GetType(Integer)) With {.DefaultValue = 0})
        Dt.Columns.Add(New DataColumn("Portata", GetType(Decimal)) With {.DefaultValue = 0})
        Dt.Columns.Add(New DataColumn("Efficienza", GetType(Decimal)) With {.DefaultValue = 100})
        Dt.Columns.Add(New DataColumn("QtaTotaleAcquaGiornaliera", GetType(Decimal)) With {.DefaultValue = 0})
        Dt.Columns.Add(New DataColumn("QtaTotaleAcquaAssorbitaGiornaliera", GetType(Decimal)) With {.DefaultValue = 0})
        Dt.Columns.Add(New DataColumn("DataInizioIrrigazione", GetType(Date)) With {.DefaultValue = Data})
        Dt.Columns.Add(New DataColumn("DataFineIrrigazione", GetType(Date)) With {.DefaultValue = Data})
        Dt.Columns.Add(New DataColumn("FrequenzaIrrigazioneMedia", GetType(Decimal)) With {.DefaultValue = 1})
        Dt.Columns.Add(New DataColumn("QtaTotaleAcquaUtilizzataPeriodo", GetType(Decimal)) With {.DefaultValue = 0})
        Dt.Columns.Add(New DataColumn("QtaTotaleAcquaAssorbitaPeriodo", GetType(Decimal)) With {.DefaultValue = 0})
        Dt.Columns.Add(New DataColumn("Frequenza", GetType(Decimal)) With {.DefaultValue = 0})
        Dt.Columns.Add(New DataColumn("tipoIrrigazione", GetType(AgronicaCoreModelsSTD.metaschema.TipoIrrigazione)) With {.DefaultValue = Nothing})
        Dt.Columns.Add(New DataColumn("macchinaIrrigazione", GetType(AgronicaCoreModelsSTD.anagrafiche.ParcoMacchine)) With {.DefaultValue = Nothing})

        If Dt IsNot Nothing AndAlso Dt.Rows.Count > 0 Then

            For j = 0 To Dt.Rows.Count - 1

                Dt.Rows(j).Item("App_Nome_Anagrafica") = Dt.Rows(j).Item("APP_NOME")

                Dim kk As String =
                    Dt.Rows(j).Item("Piva") & "-" &
                    Dt.Rows(j).Item("Sa_Cod").ToString & "-" &
                    Dt.Rows(j).Item("Appezza").ToString & "-" &
                    Dt.Rows(j).Item("id_reg").ToString


                Dt.Rows(j).Item("kendoKey") = kk
                Dt.Rows(j).Item("Sup_Imp_help") = 0


                '--------------------------
                '02/09/2014 esclusa dalla copertura cop_cod=1 (protezione grandine)
                Select Case Dt.Rows(j).Item("Cop_Cod")
                    Case 0, 1, 3, 4, 5, 6 'nessuna copertura
                        ColturaProtetta = Gias.No
                        Flag_Protetto = 0
                    Case Else
                        ColturaProtetta = Gias.Si
                        Flag_Protetto = 1
                End Select

                Dt.Rows(j).Item("id_rcdpi") = 0


                Dt.Rows(j).Item("Obj_Disciplinare") = New AgronicaCoreModelsSTD.metaschema.Disciplinare(Dt.Rows(j).Item("Disciplinare_Cod").ToString()) With {
                    .descrizione = Dt.Rows(j).Item("Disciplinare").ToString(),
                    .gruppoFinalita = New AgronicaCoreModelsSTD.metaschema.utilizzi.GruppoFinalita(If(IsDBNull(Dt.Rows(j).Item("GRFI_COD")), 0, CInt(Dt.Rows(j).Item("GRFI_COD")))) With {
                        .descrizione = Dt.Rows(j).Item("Grfi_Des").ToString()
                    },
                    .flagProtetto = Flag_Protetto
                }

                Dt.Rows(j).Item("Flag_Protetto") = Flag_Protetto

                '--------------------------
                'Formatto le date
                If IsDate(Dt.Rows(j).Item("Validita_Inizio")) Then

                    Dt.Rows(j).Item("Validita_Inizio_Date") = CDate(Dt.Rows(j).Item("Validita_Inizio"))

                End If
                If IsDate(Dt.Rows(j).Item("Validita_Fine")) Then

                    Dt.Rows(j).Item("Validita_Fine_Date") = CDate(Dt.Rows(j).Item("Validita_Fine"))

                End If
                If IsDate(Dt.Rows(j).Item("Data_Semina")) Then

                    Dt.Rows(j).Item("Data_Semina_Date") = CDate(Dt.Rows(j).Item("Data_Semina"))

                End If
                If IsDate(Dt.Rows(j).Item("Data_Semina_Prevista")) Then

                    Dt.Rows(j).Item("Data_Semina_Prevista_Date") = CDate(Dt.Rows(j).Item("Data_Semina_Prevista"))

                End If
                If IsDate(Dt.Rows(j).Item("Data_Raccolta")) Then

                    Dt.Rows(j).Item("Data_Raccolta_Date") = CDate(Dt.Rows(j).Item("Data_Raccolta"))

                End If
                If IsDate(Dt.Rows(j).Item("Data_Raccolta_Prevista")) Then

                    Dt.Rows(j).Item("Data_Raccolta_Prevista_Date") = CDate(Dt.Rows(j).Item("Data_Raccolta_Prevista"))

                End If
                If IsDate(Dt.Rows(j).Item("Data_Fioritura")) Then

                    Dt.Rows(j).Item("Data_Fioritura_Date") = CDate(Dt.Rows(j).Item("Data_Fioritura"))

                End If
                If IsDate(Dt.Rows(j).Item("Data_Fioritura_Prevista")) Then

                    Dt.Rows(j).Item("Data_Fioritura_Prevista_Date") = CDate(Dt.Rows(j).Item("Data_Fioritura_Prevista"))

                End If
                If IsDate(Dt.Rows(j).Item("Validita_Inizio_Distinta")) Then

                    Dt.Rows(j).Item("Validita_Inizio_Distinta_Date") = CDate(Dt.Rows(j).Item("Validita_Inizio_Distinta"))

                End If
                If IsDate(Dt.Rows(j).Item("Validita_Fine_Distinta")) Then

                    Dt.Rows(j).Item("Validita_Fine_Distinta_Date") = CDate(Dt.Rows(j).Item("Validita_Fine_Distinta"))

                End If
                If IsDate(Dt.Rows(j).Item("Validita_inizio_Appezzamento")) Then

                    Dt.Rows(j).Item("Validita_inizio_Appezzamento_Date") = CDate(Dt.Rows(j).Item("Validita_inizio_Appezzamento"))

                End If
                If IsDate(Dt.Rows(j).Item("Validita_fine_Appezzamento")) Then

                    Dt.Rows(j).Item("Validita_fine_Appezzamento_Date") = CDate(Dt.Rows(j).Item("Validita_fine_Appezzamento"))

                End If
                '========= PermessoDPI fine

                '-----------------------------------------
                'aggiunta indicazione catasto (19/09/2012)
                Dim strCatasto As String = ""
                Dim listaClassiTessituraModel As New List(Of AgronicaCoreModelsSTD.anagrafiche.ClasseTessitura)()
                Dim strId_ClasseTessitura As String = ""
                Dim strStr_ClasseTessitura As String = ""
                Dim DrParticelle As DataRow()
                Dim DrPV As DataRow()
                Dim DrPVFA As DataRow()
                Dim DrPVFB As DataRow()
                Dim p As Integer

                Dim strParticella As String
                Dim strVulnerabile As String

                Dim Flag_ZVN As Boolean = False

                If DtParticelle IsNot Nothing AndAlso DtParticelle.Rows.Count > 0 Then
                    DrParticelle = DtParticelle.Select("piva='" & Dt.Rows(j).Item("piva").ToString & "' and sa_cod=" & Dt.Rows(j).Item("sa_cod").ToString & " and appezza=" & Dt.Rows(j).Item("appezza").ToString & " and id_reg=" & Dt.Rows(j).Item("id_reg").ToString)
                    If DrParticelle IsNot Nothing AndAlso DrParticelle.Length > 0 Then
                        For p = 0 To DrParticelle.Length - 1
                            strParticella = DrParticelle(p).Item("prov") & "_" & DrParticelle(p).Item("com") & "_" & DrParticelle(p).Item("sezione") & "_" & DrParticelle(p).Item("foglio") & "_" & DrParticelle(p).Item("numero") & "_" & DrParticelle(p).Item("subalterno")
                            strVulnerabile = ""
                            If DtPV IsNot Nothing AndAlso DtPV.Rows.Count > 0 Then
                                DrPV = DtPV.Select("prov='" & DrParticelle(p).Item("prov").ToString & "' AND Com='" & DrParticelle(p).Item("com").ToString & "' AND Sezione='" & DrParticelle(p).Item("sezione").ToString & "' AND Foglio=" & DrParticelle(p).Item("foglio").ToString & " AND Numero=" & DrParticelle(p).Item("numero").ToString & " AND subalterno='" & DrParticelle(p).Item("subalterno").ToString & "'")
                                If DrPV IsNot Nothing AndAlso DrPV.Length > 0 Then
                                    strVulnerabile = " <b>(V)</b>"
                                    Flag_ZVN = True
                                End If
                            End If
                            If DtPVF IsNot Nothing AndAlso DtPVF.Rows.Count > 0 Then
                                DrPVFA = DtPVF.Select("prov='" & DrParticelle(p).Item("prov").ToString & "' AND Com='" & DrParticelle(p).Item("com").ToString & "' AND Foglio=" & DrParticelle(p).Item("foglio").ToString & " AND Fascia_Cod=1")
                                If DrPVFA IsNot Nothing AndAlso DrPVFA.Length > 0 Then
                                    strVulnerabile = " <b>(V - " & My.Resources.AgronicaCoreAnagrafeBIZ.FasciaA & ")</b>"
                                End If
                                DrPVFB = DtPVF.Select("prov='" & DrParticelle(p).Item("prov").ToString & "' AND Com='" & DrParticelle(p).Item("com").ToString & "' AND Foglio=" & DrParticelle(p).Item("foglio").ToString & " AND Fascia_Cod=2")
                                If DrPVFB IsNot Nothing AndAlso DrPVFB.Length > 0 Then
                                    strVulnerabile = " <b>(V - " & My.Resources.AgronicaCoreAnagrafeBIZ.FasciaB & ")</b>"
                                End If
                            End If
                            strCatasto &= strParticella & strVulnerabile & "<br>"
                        Next

                        Dim arrId_ClassiTessitura As Integer() = (From drP As DataRow In DrParticelle Where CInt(drP.Item("Id_ClasseTessitura")) > 0 Select CInt(drP.Item("Id_ClasseTessitura"))).Distinct().ToArray()
                        Dim listaStrClassiTessitura As New List(Of String)
                        For Each el As Integer In arrId_ClassiTessitura
                            Dim strClassiTessitura As String = (From ct As AgronicaCorePianoConcimazioneBIZ.PUA_ClasseTessitura In classTess_output.ListaClassiTessitura Where el = ct.Tessitura_Cod Select CStr(ct.Tessitura_Des)).ToArray().FirstOrDefault()
                            If Not IsNothing(strClassiTessitura) Then

                                listaStrClassiTessitura.Add(strClassiTessitura.Trim())

                                listaClassiTessituraModel.Add(New AgronicaCoreModelsSTD.anagrafiche.ClasseTessitura(el) With {
                                    .descrizione = strClassiTessitura
                                })

                            End If
                        Next
                        strId_ClasseTessitura = String.Join(",", arrId_ClassiTessitura)
                        strStr_ClasseTessitura = String.Join(", ", listaStrClassiTessitura)
                    End If
                End If
                If strCatasto <> "" Then
                    strCatasto = Left(strCatasto, strCatasto.Length - 4)
                End If
                Dt.Rows(j).Item("Id_ClasseTessitura") = strId_ClasseTessitura
                Dt.Rows(j).Item("Str_ClasseTessitura") = strStr_ClasseTessitura
                Dt.Rows(j).Item("ListaClassiTessitura") = listaClassiTessituraModel

                Dt.Rows(j).Item("catasto") = strCatasto

                Dt.Rows(j).Item("ZVN") = If(Flag_ZVN, Gias.Si, Gias.No)

                Dt.Rows(j).Item("N_Max") = Dt.Rows(j).Item("N_Massimo")

                If IsNumeric(Dt.Rows(j).Item("N_Max")) AndAlso CDec(Dt.Rows(j).Item("N_Max")) > 0 Then
                    Dt.Rows(j).Item("N_Max_Decimal") = Math.Round(CDec(Dt.Rows(j).Item("N_Max")), 3)
                Else
                    Dt.Rows(j).Item("N_Max_Decimal") = 0
                End If

                Dt.Rows(j).Item("P_Max") = Dt.Rows(j).Item("P_Massimo")

                If IsNumeric(Dt.Rows(j).Item("P_Max")) AndAlso CDec(Dt.Rows(j).Item("P_Max")) > 0 Then
                    Dt.Rows(j).Item("P_Max_Decimal") = Math.Round(CDec(Dt.Rows(j).Item("P_Max")), 3)
                Else
                    Dt.Rows(j).Item("P_Max_Decimal") = 0
                End If

                Dt.Rows(j).Item("K_Max") = Dt.Rows(j).Item("K_Massimo")

                If IsNumeric(Dt.Rows(j).Item("K_Max")) AndAlso CDec(Dt.Rows(j).Item("K_Max")) > 0 Then
                    Dt.Rows(j).Item("K_Max_Decimal") = Math.Round(CDec(Dt.Rows(j).Item("K_Max")), 3)
                Else
                    Dt.Rows(j).Item("K_Max_Decimal") = 0
                End If

                Dt.Rows(j).Item("Mg_Max") = Dt.Rows(j).Item("Mg_Massimo")

                If IsNumeric(Dt.Rows(j).Item("Mg_Max")) AndAlso CDec(Dt.Rows(j).Item("Mg_Max")) > 0 Then
                    Dt.Rows(j).Item("Mg_Max_Decimal") = Math.Round(CDec(Dt.Rows(j).Item("Mg_Max")), 3)
                Else
                    Dt.Rows(j).Item("Mg_Max_Decimal") = 0
                End If

                If Data >= #1/1/2019# Then
                    Dt.Rows(j).Item("CU_Max") = 4
                Else
                    Dt.Rows(j).Item("CU_Max") = 6
                End If

                Dt.Rows(j).Item("CU_Max_Decimal") = Dt.Rows(j).Item("CU_Max")

                If Dt.Rows(j).Item("Mac_Cod") > 0 Then

                    'dati di default
                    Dim macchina = New ParcoMacchine
                    macchina.flag_cancellazione = False
                    macchina.partitaIva = "" 'non necessaria
                    macchina.centroPK = New CentroAziendale.PK(0, "")
                    macchina.contatto = New Contatto
                    macchina.contatto.data_Nascita = AGRODATAINIZIO
                    macchina.alimentazione = New metaschema.Carburante
                    macchina.codice_stringa = ""
                    macchina.costi = New List(Of CostoUnitario)
                    macchina.CUAA_Proprietario = ""
                    macchina.data_Immatricolazione = AGRODATAINIZIO
                    macchina.data_Rilascio_Autorizzazione = DateTime.Now
                    macchina.data_Ultima_Revisione = DateTime.Now
                    macchina.data_Ultima_Manutenzione = DateTime.Now
                    macchina.Data_Carico = AGRODATAINIZIO
                    macchina.Data_Scarico = AGRODATAFINE
                    macchina.data_Ultima_Taratura = AGRODATAINIZIO
                    macchina.scadenza_Taratura = AGRODATAFINE
                    macchina.Data_Inizio_Installazione = AGRODATAINIZIO
                    macchina.Data_Fine_Installazione = AGRODATAFINE

                    Dim efficienza = 0.01
                    If Dt.Rows(j).Item("EfficienzaMacchinaIrrigazione") IsNot Nothing AndAlso Dt.Rows(j).Item("EfficienzaMacchinaIrrigazione") > 0 Then
                        efficienza = Dt.Rows(j).Item("EfficienzaMacchinaIrrigazione")
                    End If
                    macchina.codice = Dt.Rows(j).Item("Mac_Cod")
                    macchina.portata = Dt.Rows(j).Item("PortataMacchinaIrrigazione")
                    macchina.efficienza = efficienza
                    macchina.codice_impianto = Dt.Rows(j).Item("ImpCodMacchina")
                    Dt.Rows(j).Item("macchinaIrrigazione") = macchina

                    Dt.Rows(j).Item("IrrigazioneUtilizzata_Codice") = "0_" & Dt.Rows(j).Item("Mac_Cod")
                    Dt.Rows(j).Item("IrrigazioneUtilizzata_Descrizione") = $"--- {Gias.MacchinePerIrrigazione} --- {Dt.Rows(j).Item("Mac_Des")}"
                    Dt.Rows(j).Item("Portata") = Dt.Rows(j).Item("PortataMacchinaIrrigazione")
                    Dt.Rows(j).Item("Efficienza") = efficienza

                ElseIf Dt.Rows(j).Item("Imp_Cod") > 0 Then

                    Dim tipoIrrigazione = New AgronicaCoreModelsSTD.metaschema.TipoIrrigazione
                    tipoIrrigazione.codice = Dt.Rows(j).Item("Imp_Cod")
                    tipoIrrigazione.descrizione = $"--- {Gias.ImpiantoIrrigazione} --- {Dt.Rows(j).Item("Imp_Des")}"
                    Dt.Rows(j).Item("tipoIrrigazione") = tipoIrrigazione

                    Dt.Rows(j).Item("IrrigazioneUtilizzata_Codice") = Dt.Rows(j).Item("Imp_Cod") & "_0"
                    Dt.Rows(j).Item("IrrigazioneUtilizzata_Descrizione") = $"--- {Gias.ImpiantoIrrigazione} --- {Dt.Rows(j).Item("Imp_Des")}"

                End If

                'NPK MarcoG 13/06/2019
                Dim strN As String = "<b>" & Gias.Massimo & ": </b><span class='qdc_n_masssimo'>" & If(CStr(Dt.Rows(j).Item("N_Max")) = "", NEWLINE & Gias.NonDefinito, CStr(Dt.Rows(j).Item("N_Max"))) & "</span><br>"
                '"<b>Distribuito: </b><span class='qdc_n_distribuito'>" & CStr(Dt.Rows(j).Item("N_Distribuito")) & "</span><br>" &
                '"<b>Residuo: </b><span class='qdc_n_residuo'>" & CStr(Dt.Rows(j).Item("N_Residuo")) & "</span>"

                Dim strP As String = "<b>" & Gias.Massimo & ": </b><span class='qdc_p_masssimo'>" & If(CStr(Dt.Rows(j).Item("P_Max")) = "", NEWLINE & Gias.NonDefinito, CStr(Dt.Rows(j).Item("P_Max"))) & "</span><br>"
                '"<b>Distribuito: </b><span class='qdc_p_distribuito'>" & CStr(Dt.Rows(j).Item("P_Distribuito")) & "</span><br>" &
                '"<b>Residuo: </b><span class='qdc_p_residuo'>" & CStr(Dt.Rows(j).Item("P_Residuo")) & "</span>"

                Dim strK As String = "<b>" & Gias.Massimo & ": </b><span class='qdc_k_masssimo'>" & If(CStr(Dt.Rows(j).Item("K_Max")) = "", NEWLINE & Gias.NonDefinito, CStr(Dt.Rows(j).Item("K_Max"))) & "</span><br>"
                '"<b>Distribuito: </b><span class='qdc_k_distribuito'>" & CStr(Dt.Rows(j).Item("K_Distribuito")) & "</span><br>" &
                '"<b>Residuo: </b><span class='qdc_k_residuo'>" & CStr(Dt.Rows(j).Item("K_Residuo")) & "</span>"

                Dim strMg As String = "<b>" & Gias.Massimo & ": </b><span class='qdc_mg_masssimo'>" & If(CStr(Dt.Rows(j).Item("Mg_Max")) = "", NEWLINE & Gias.NonDefinito, CStr(Dt.Rows(j).Item("Mg_Max"))) & "</span><br>" '&
                '"<b>Distribuito: </b><span class='qdc_mg_distribuito'>" & CStr(Dt.Rows(j).Item("Mg_Distribuito")) & "</span><br>" &
                '"<b>Residuo: </b><span class='qdc_mg_residuo'>" & CStr(Dt.Rows(j).Item("Mg_Residuo")) & "</span>"

                Dim strCU As String = "<b>" & Gias.Massimo & ": </b><span class='qdc_cu_masssimo'>" & If(CStr(Dt.Rows(j).Item("CU_Max")) = "", NEWLINE & Gias.NonDefinito, CStr(Dt.Rows(j).Item("CU_Max"))) & "</span><br>" '&




                Dt.Rows(j).Item("Descrizione_Unica") = Dt.Rows(j).Item("App_Nome") &
                                                        If(String.IsNullOrEmpty(Dt.Rows(j).Item("RifNumerico")), "", " (" & Dt.Rows(j).Item("RifNumerico") & ")") &
                                                        " - <i>" & Gias.Varieta & ": " & Dt.Rows(j).Item("Cul_Des") & "</i> - " & Gias.SuperficieAbbr2 & ": " & Dt.Rows(j).Item("Sup_Imp") & " Ha"


                '----------------------------------------------------------------

                Dim objFert As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
                Dim objDettRic As New AgronicaCoreContabDAL.Ricette_Dettagli_R
                Dim N_Distribuito As Decimal = 0
                Dim P_Distribuito As Decimal = 0
                Dim K_Distribuito As Decimal = 0
                Dim Mg_Distribuito As Decimal = 0
                Dim Cu_Distribuito As Decimal = 0
                Dim N_Distribuito_Ricetta As Decimal = 0
                Dim P_Distribuito_Ricetta As Decimal = 0
                Dim K_Distribuito_Ricetta As Decimal = 0
                Dim Mg_Distribuito_Ricetta As Decimal = 0
                Dim Cu_Distribuito_Ricetta As Decimal = 0
                Dim N_Residuo As Decimal = 0
                Dim P_Residuo As Decimal = 0
                Dim K_Residuo As Decimal = 0
                Dim Mg_Residuo As Decimal = 0
                Dim CU_Residuo As Decimal = 0

                If Lav_Cod_List.Exists(Function(lav_cod) {LAVCOD_FERTIRRIGAZIONE,
                                                              LAVCOD_CONCIMAZIONE_FOGLIARE,
                                                              LAVCOD_DISTRIBUZIONE_CONCIME,
                                                              LAVCOD_DISTRIBUZIONE_AMMENDANTI,
                                                              LAVCOD_SARCHIATURA_CONCIMAZIONE,
                                                              LAVCOD_TRATTAMENTO_ANTIBUTTERATURA}.Contains(lav_cod)) Then
                    'se almeno un valore massimo è stato impostato sull'impianto
                    If Dt.Rows(j).Item("N_Max") <> "" OrElse
                        Dt.Rows(j).Item("P_Max") <> "" OrElse
                        Dt.Rows(j).Item("K_Max") <> "" OrElse
                        Dt.Rows(j).Item("Mg_Max") <> "" OrElse
                        Dt.Rows(j).Item("CU_Max") <> "" Then

                        objFert.Leggi_Macroelementi_Distribuiti_List_Id_Agenda_Esclusi(N_Distribuito,
                                                                P_Distribuito,
                                                                K_Distribuito,
                                                                Mg_Distribuito,
                                                                Cu_Distribuito,
                                                                0, 0, 0, 0, 0,
                                                                CStr(Dt.Rows(j).Item("Piva")),
                                                                CInt(Dt.Rows(j).Item("Sa_Cod")),
                                                                CInt(Dt.Rows(j).Item("Appezza")),
                                                                CInt(Dt.Rows(j).Item("Id_Reg")),
                                                                CInt(Dt.Rows(j).Item("Progetto_Cod")),
                                                                CDate(Dt.Rows(j).Item("Validita_Inizio_Distinta")),
                                                                CDate(Dt.Rows(j).Item("Validita_Fine_Distinta")),
                                                                Id_Agenda_List,
                                                                objParametri_Server)

                        If TipoAttivita = AgronicaCoreModelsSTD.attivita.Attivita.Tipo_Attivita.Ricetta AndAlso Stato = AgronicaCoreModelsSTD.attivita.Attivita.Stati.Da_Eseguire Then
                            objDettRic.Leggi_Macroelementi_Distribuiti_List_Ricetta_operazione_Cod_Esclusi(N_Distribuito_Ricetta,
                                                                                                            P_Distribuito_Ricetta,
                                                                                                            K_Distribuito_Ricetta,
                                                                                                            Mg_Distribuito_Ricetta,
                                                                                                            Cu_Distribuito_Ricetta,
                                                                                                            0, 0, 0, 0, 0,
                                                                                                            CStr(Dt.Rows(j).Item("Piva")),
                                                                                                            CInt(Dt.Rows(j).Item("Sa_Cod")),
                                                                                                            CInt(Dt.Rows(j).Item("Appezza")),
                                                                                                            CInt(Dt.Rows(j).Item("Id_Reg")),
                                                                                                            CInt(Dt.Rows(j).Item("Progetto_Cod")),
                                                                                                            CDate(Dt.Rows(j).Item("Validita_Inizio_Distinta")),
                                                                                                            CDate(Dt.Rows(j).Item("Validita_Fine_Distinta")),
                                                                                                            Ricetta_Operazione_Cod_List,
                                                                                                            objParametri_Server)
                        End If



                        If IsNumeric(Dt.Rows(j).Item("N_Max")) Then

                            'Arrotondo gli N a 3 decimali dopo la virgola mentre le percentuali a 2

                            'Mostro l'indicazione del Distribuito con la Percentuale
                            Dim N_Distribuito_Percentuale As Decimal = Calcola_Percentuale_N_P_K_Cu_Mg(Dt.Rows(j).Item("N_Max_Decimal"), N_Distribuito)
                            Dt.Rows(j).Item("N_Distribuito") = String.Format("{0:N3}", N_Distribuito)
                            strN &= "<b>" & My.Resources.AgronicaCoreAnagrafeBIZ.GiaDistribuito & ": </b><span class='qdc_n_distribuito'>" & Dt.Rows(j).Item("N_Distribuito") & " (" & N_Distribuito_Percentuale & "%) </span><br>"

                            'Mostro l'indicazione del Preventivato con la Percentuale
                            If TipoAttivita = AgronicaCoreModelsSTD.attivita.Attivita.Tipo_Attivita.Ricetta AndAlso Stato = AgronicaCoreModelsSTD.attivita.Attivita.Stati.Da_Eseguire Then
                                Dim N_Preventivato_Percentuale As Decimal = Calcola_Percentuale_N_P_K_Cu_Mg(Dt.Rows(j).Item("N_Max_Decimal"), N_Distribuito_Ricetta)
                                strN &= "<b>" & My.Resources.AgronicaCoreAnagrafeBIZ.Preventivato & ": </b><span class='qdc_n_preventivato'>" & String.Format("{0:N3}", N_Distribuito_Ricetta) & " (" & N_Preventivato_Percentuale & "%)</span><br>"
                            End If


                            'Mostro l'indicazione del Residuo (Massimale - (Distribuito + Preventivato)) con la Percentuale
                            Dim N_Residuo_Decimal As Decimal = Dt.Rows(j).Item("N_Max_Decimal") - N_Distribuito - N_Distribuito_Ricetta
                            Dim N_Residuo_Percentuale As Decimal = Calcola_Percentuale_N_P_K_Cu_Mg(Dt.Rows(j).Item("N_Max_Decimal"), N_Residuo_Decimal)
                            Dt.Rows(j).Item("N_Residuo_Decimal") = Math.Round(N_Residuo_Decimal, 3)
                            Dt.Rows(j).Item("N_Residuo_Percentuale") = N_Residuo_Percentuale
                            Dt.Rows(j).Item("N_Residuo") = String.Format("{0:N3}", N_Residuo_Decimal)
                            strN &= "<b>" & My.Resources.AgronicaCoreAnagrafeBIZ.Residuo & ": </b><span class='qdc_n_residuo'>" & Dt.Rows(j).Item("N_Residuo") & " (" & Dt.Rows(j).Item("N_Residuo_Percentuale") & "%)</span>"

                        End If
                        If IsNumeric(Dt.Rows(j).Item("P_Max")) Then

                            'Arrotondo i P a 3 decimali dopo la virgola mentre le percentuali a 2

                            'Mostro l'indicazione del Distribuito con la Percentuale
                            Dim P_Distribuito_Percentuale As Decimal = Calcola_Percentuale_N_P_K_Cu_Mg(Dt.Rows(j).Item("P_Max_Decimal"), P_Distribuito)
                            Dt.Rows(j).Item("P_Distribuito") = String.Format("{0:N3}", P_Distribuito)
                            strP &= "<b>" & My.Resources.AgronicaCoreAnagrafeBIZ.GiaDistribuito & ": </b><span class='qdc_p_distribuito'>" & Dt.Rows(j).Item("P_Distribuito") & " (" & P_Distribuito_Percentuale & "%) </span><br>"

                            'Mostro l'indicazione del Preventivato con la Percentuale
                            If TipoAttivita = AgronicaCoreModelsSTD.attivita.Attivita.Tipo_Attivita.Ricetta AndAlso Stato = AgronicaCoreModelsSTD.attivita.Attivita.Stati.Da_Eseguire Then
                                Dim P_Preventivato_Percentuale As Decimal = Calcola_Percentuale_N_P_K_Cu_Mg(Dt.Rows(j).Item("P_Max_Decimal"), P_Distribuito_Ricetta)
                                strP &= "<b>" & My.Resources.AgronicaCoreAnagrafeBIZ.Preventivato & ": </b><span class='qdc_p_preventivato'>" & String.Format("{0:N3}", P_Distribuito_Ricetta) & " (" & P_Preventivato_Percentuale & "%)</span><br>"
                            End If


                            'Mostro l'indicazione del Residuo (Massimale - (Distribuito + Preventivato)) con la Percentuale
                            Dim P_Residuo_Decimal As Decimal = Dt.Rows(j).Item("P_Max_Decimal") - P_Distribuito - P_Distribuito_Ricetta
                            Dim P_Residuo_Percentuale As Decimal = Calcola_Percentuale_N_P_K_Cu_Mg(Dt.Rows(j).Item("P_Max_Decimal"), P_Residuo_Decimal)
                            Dt.Rows(j).Item("P_Residuo_Decimal") = Math.Round(P_Residuo_Decimal, 3)
                            Dt.Rows(j).Item("P_Residuo_Percentuale") = P_Residuo_Percentuale
                            Dt.Rows(j).Item("P_Residuo") = String.Format("{0:N3}", P_Residuo_Decimal)
                            strP &= "<b>" & My.Resources.AgronicaCoreAnagrafeBIZ.Residuo & ": </b><span class='qdc_p_residuo'>" & Dt.Rows(j).Item("P_Residuo") & " (" & Dt.Rows(j).Item("P_Residuo_Percentuale") & "%)</span>"

                        End If
                        If IsNumeric(Dt.Rows(j).Item("K_Max")) Then

                            'Arrotondo i K a 3 decimali dopo la virgola mentre le percentuali a 2

                            'Mostro l'indicazione del Distribuito con la Percentuale
                            Dim K_Distribuito_Percentuale As Decimal = Calcola_Percentuale_N_P_K_Cu_Mg(Dt.Rows(j).Item("K_Max_Decimal"), K_Distribuito)
                            Dt.Rows(j).Item("K_Distribuito") = String.Format("{0:N3}", K_Distribuito)
                            strK &= "<b>" & My.Resources.AgronicaCoreAnagrafeBIZ.GiaDistribuito & ": </b><span class='qdc_k_distribuito'>" & Dt.Rows(j).Item("K_Distribuito") & " (" & K_Distribuito_Percentuale & "%) </span><br>"

                            'Mostro l'indicazione del Preventivato con la Percentuale
                            If TipoAttivita = AgronicaCoreModelsSTD.attivita.Attivita.Tipo_Attivita.Ricetta AndAlso Stato = AgronicaCoreModelsSTD.attivita.Attivita.Stati.Da_Eseguire Then
                                Dim K_Preventivato_Percentuale As Decimal = Calcola_Percentuale_N_P_K_Cu_Mg(Dt.Rows(j).Item("K_Max_Decimal"), K_Distribuito_Ricetta)
                                strK &= "<b>" & My.Resources.AgronicaCoreAnagrafeBIZ.Preventivato & ": </b><span class='qdc_k_preventivato'>" & String.Format("{0:N3}", K_Distribuito_Ricetta) & " (" & K_Preventivato_Percentuale & "%)</span><br>"
                            End If


                            'Mostro l'indicazione del Residuo (Massimale - (Distribuito + Preventivato)) con la Percentuale
                            Dim K_Residuo_Decimal As Decimal = Dt.Rows(j).Item("K_Max_Decimal") - K_Distribuito - K_Distribuito_Ricetta
                            Dim K_Residuo_Percentuale As Decimal = Calcola_Percentuale_N_P_K_Cu_Mg(Dt.Rows(j).Item("K_Max_Decimal"), K_Residuo_Decimal)
                            Dt.Rows(j).Item("K_Residuo_Decimal") = Math.Round(K_Residuo_Decimal, 3)
                            Dt.Rows(j).Item("K_Residuo_Percentuale") = K_Residuo_Percentuale
                            Dt.Rows(j).Item("K_Residuo") = String.Format("{0:N3}", K_Residuo_Decimal)
                            strK &= "<b>" & My.Resources.AgronicaCoreAnagrafeBIZ.Residuo & ": </b><span class='qdc_k_residuo'>" & Dt.Rows(j).Item("K_Residuo") & " (" & Dt.Rows(j).Item("K_Residuo_Percentuale") & "%)</span>"
                        End If
                        If IsNumeric(Dt.Rows(j).Item("Mg_Max")) Then

                            'Arrotondo gli Mg a 3 decimali dopo la virgola mentre le percentuali a 2

                            'Mostro l'indicazione del Distribuito con la Percentuale
                            Dim Mg_Distribuito_Percentuale As Decimal = Calcola_Percentuale_N_P_K_Cu_Mg(Dt.Rows(j).Item("Mg_Max_Decimal"), Mg_Distribuito)
                            Dt.Rows(j).Item("Mg_Distribuito") = String.Format("{0:N3}", Mg_Distribuito)
                            strMg &= "<b>" & My.Resources.AgronicaCoreAnagrafeBIZ.GiaDistribuito & ": </b><span class='qdc_mg_distribuito'>" & Dt.Rows(j).Item("Mg_Distribuito") & " (" & Mg_Distribuito_Percentuale & "%) </span><br>"

                            'Mostro l'indicazione del Preventivato con la Percentuale
                            If TipoAttivita = AgronicaCoreModelsSTD.attivita.Attivita.Tipo_Attivita.Ricetta AndAlso Stato = AgronicaCoreModelsSTD.attivita.Attivita.Stati.Da_Eseguire Then
                                Dim Mg_Preventivato_Percentuale As Decimal = Calcola_Percentuale_N_P_K_Cu_Mg(Dt.Rows(j).Item("Mg_Max_Decimal"), Mg_Distribuito_Ricetta)
                                strMg &= "<b>" & My.Resources.AgronicaCoreAnagrafeBIZ.Preventivato & ": </b><span class='qdc_mg_preventivato'>" & String.Format("{0:N3}", Mg_Distribuito_Ricetta) & " (" & Mg_Preventivato_Percentuale & "%)</span><br>"
                            End If


                            'Mostro l'indicazione del Residuo (Massimale - (Distribuito + Preventivato)) con la Percentuale
                            Dim Mg_Residuo_Decimal As Decimal = Dt.Rows(j).Item("Mg_Max_Decimal") - Mg_Distribuito - Mg_Distribuito_Ricetta
                            Dim Mg_Residuo_Percentuale As Decimal = Calcola_Percentuale_N_P_K_Cu_Mg(Dt.Rows(j).Item("Mg_Max_Decimal"), Mg_Residuo_Decimal)
                            Dt.Rows(j).Item("Mg_Residuo_Decimal") = Math.Round(Mg_Residuo_Decimal, 3)
                            Dt.Rows(j).Item("Mg_Residuo_Percentuale") = Mg_Residuo_Percentuale
                            Dt.Rows(j).Item("Mg_Residuo") = String.Format("{0:N3}", Mg_Residuo_Decimal)
                            strMg &= "<b>" & My.Resources.AgronicaCoreAnagrafeBIZ.Residuo & ": </b><span class='qdc_mg_residuo'>" & Dt.Rows(j).Item("Mg_Residuo") & " (" & Dt.Rows(j).Item("Mg_Residuo_Percentuale") & "%)</span>"

                        End If
                        If IsNumeric(Dt.Rows(j).Item("CU_Max")) Then

                            'Arrotondo i Cu a 3 decimali dopo la virgola mentre le percentuali a 2

                            'Mostro l'indicazione del Distribuito con la Percentuale
                            Dim CU_Distribuito_Percentuale As Decimal = Calcola_Percentuale_N_P_K_Cu_Mg(Dt.Rows(j).Item("CU_Max_Decimal"), Cu_Distribuito)
                            Dt.Rows(j).Item("CU_Distribuito") = String.Format("{0:N3}", Cu_Distribuito)
                            strCU &= "<b>" & My.Resources.AgronicaCoreAnagrafeBIZ.GiaDistribuito & ": </b><span class='qdc_cu_distribuito'>" & Dt.Rows(j).Item("CU_Distribuito") & " (" & CU_Distribuito_Percentuale & "%) </span><br>"

                            'Mostro l'indicazione del Preventivato con la Percentuale
                            If TipoAttivita = AgronicaCoreModelsSTD.attivita.Attivita.Tipo_Attivita.Ricetta AndAlso Stato = AgronicaCoreModelsSTD.attivita.Attivita.Stati.Da_Eseguire Then
                                Dim CU_Preventivato_Percentuale As Decimal = Calcola_Percentuale_N_P_K_Cu_Mg(Dt.Rows(j).Item("CU_Max_Decimal"), Cu_Distribuito_Ricetta)
                                strCU &= "<b>" & My.Resources.AgronicaCoreAnagrafeBIZ.Preventivato & ": </b><span class='qdc_cu_preventivato'>" & String.Format("{0:N3}", Cu_Distribuito_Ricetta) & " (" & CU_Preventivato_Percentuale & "%)</span><br>"
                            End If


                            'Mostro l'indicazione del Residuo (Massimale - (Distribuito + Preventivato)) con la Percentuale
                            Dim CU_Residuo_Decimal As Decimal = Dt.Rows(j).Item("CU_Max_Decimal") - Cu_Distribuito - Cu_Distribuito_Ricetta
                            Dim CU_Residuo_Percentuale As Decimal = Calcola_Percentuale_N_P_K_Cu_Mg(Dt.Rows(j).Item("CU_Max_Decimal"), CU_Residuo_Decimal)
                            Dt.Rows(j).Item("CU_Residuo_Decimal") = Math.Round(CU_Residuo_Decimal, 3)
                            Dt.Rows(j).Item("CU_Residuo_Percentuale") = CU_Residuo_Percentuale
                            Dt.Rows(j).Item("CU_Residuo") = String.Format("{0:N3}", CU_Residuo_Decimal)
                            strCU &= "<b>" & My.Resources.AgronicaCoreAnagrafeBIZ.Residuo & ": </b><span class='qdc_cu_residuo'>" & Dt.Rows(j).Item("CU_Residuo") & " (" & Dt.Rows(j).Item("CU_Residuo_Percentuale") & "%)</span>"
                        End If
                    End If
                End If

                Dt.Rows(j).Item("N_Massimo") = strN
                Dt.Rows(j).Item("P_Massimo") = strP
                Dt.Rows(j).Item("K_Massimo") = strK
                Dt.Rows(j).Item("Mg_Massimo") = strMg
                Dt.Rows(j).Item("CU_Massimo") = strCU



                Dim lpiva As String = Dt.Rows(j)("piva")
                Dim lSa_cod As Integer = Dt.Rows(j)("sa_cod")
                Dim lAppezza As Integer = Dt.Rows(j)("appezza")
                Dim lId_reg As Integer = Dt.Rows(j)("id_reg")

                'TODO DA CHIEDERE COME GESTIRE IN FUTURO CON NUOVO MODELLO ANGULAR
                'Dim impianto1 As Impianto = (
                '    From ii In objParametriAgenda.Impianti
                '    Where ii.Piva = lpiva AndAlso
                '          ii.Sa_Cod = lSa_cod AndAlso
                '          ii.Appezza = lAppezza AndAlso
                '          ii.ID_Reg = lId_reg).FirstOrDefault

                'If impianto1 IsNot Nothing AndAlso impianto1.GisWkt <> "" Then
                '    Dt.Rows(j).Item("GisWkt") = impianto1.GisWkt
                '    Dt.Rows(j).Item("GisWktGps") = impianto1.GisWktGps
                '    Dt.Rows(j).Item("GisWktSistemaRiferimento") = impianto1.GisWktSistemaRiferimento
                '    Dt.Rows(j).Item("GisTipoEntita_cod") = impianto1.GisTipoEntita_cod
                '    Dt.Rows(j).Item("GisLayerCod") = impianto1.GisLayerCod
                'Else
                '    Dt.Rows(j).Item("GisWkt") = ""
                '    Dt.Rows(j).Item("GisWktGps") = ""
                '    Dt.Rows(j).Item("GisWktSistemaRiferimento") = ""
                '    Dt.Rows(j).Item("GisTipoEntita_cod") = 0
                '    Dt.Rows(j).Item("GisLayerCod") = 0
                'End If


                'If impianto1 IsNot Nothing Then
                '    Dt.Rows(j).Item("Sup_Riduzione_BufferZone") = impianto1.Sup_Riduzione_BufferZone
                '    Dt.Rows(j).Item("Perc_Riduzione_Deriva") = impianto1.Perc_Riduzione_Deriva
                'Else
                '    Dt.Rows(j).Item("Sup_Riduzione_BufferZone") = 0
                '    Dt.Rows(j).Item("Perc_Riduzione_Deriva") = 0
                'End If

                Dt.Rows(j).Item("Sup_Riduzione_BufferZone") = 0
                Dt.Rows(j).Item("Perc_Riduzione_Deriva") = 0


                Dim Fase_Cod_Corrente As Integer = 0
                Dim Fase_Des_Corrente As String = ""
                Dim Data_Fase_Corrente As String = ""

                '(12/11/2018 fede)
                If Not IsDBNull(Dt.Rows(j).Item("Fase_Cod_Corrente")) AndAlso
                    IsNumeric(Dt.Rows(j).Item("Fase_Cod_Corrente")) AndAlso
                    CInt(Dt.Rows(j).Item("Fase_Cod_Corrente")) > 0 Then

                    Fase_Cod_Corrente = CInt(Dt.Rows(j).Item("Fase_Cod_Corrente"))

                    Select Case Fase_Cod_Corrente

                        Case < 1000 'caso vecchio av_cod = ff_cod
                            Fase_Des_Corrente = (From aa In objParametriUscitaFasiOld.ListaFasiFenologiche
                                                 Where aa.FF_Cod = Fase_Cod_Corrente
                                                 Select aa.Descrizione
                                        ).FirstOrDefault

                        Case Else ' caso nuovo av_cod= cod_css
                            Fase_Des_Corrente = (From aa In objParametriUscitaFasiNew.ListaFasiFenologiche
                                                 Where aa.Cod_SS = Fase_Cod_Corrente
                                                 Select aa.Descrizione & " - BBCH " & aa.Stadio
                                        ).FirstOrDefault
                    End Select

                    If Not IsDBNull(Dt.Rows(j).Item("Data_Fase_Corrente")) AndAlso
                                IsDate(Dt.Rows(j).Item("Data_Fase_Corrente")) Then
                        Data_Fase_Corrente = CDate(Dt.Rows(j).Item("Data_Fase_Corrente")).ToShortDateString
                    End If

                End If

                If Data_Fase_Corrente <> "" Then
                    Fase_Des_Corrente &= " (" & Data_Fase_Corrente & ")"
                End If

                Dt.Rows(j).Item("Fase_Corrente") = Fase_Des_Corrente

                '(17/12/2018 fede)
                Dim strBuffer As String = ""

                If Not (Dt.Rows(j).Item("DistBZ_CorpiIdrici") = 0 And
                     Dt.Rows(j).Item("DistBZ_AreeResPub") = 0 And
                    Dt.Rows(j).Item("DistBZ_Allevamenti") = 0 And
                    Dt.Rows(j).Item("DistBZ_VegNatNonColt") = 0) Then

                    If Dt.Rows(j).Item("DistBZ_CorpiIdrici") <> 0 Then
                        strBuffer &= My.Resources.AgronicaCoreAnagrafeBIZ.CorpiIdriciPer & " " & Dt.Rows(j).Item("DistBZ_CorpiIdrici") & " m" & ","
                    End If
                    If Dt.Rows(j).Item("DistBZ_AreeResPub") <> 0 Then
                        strBuffer &= My.Resources.AgronicaCoreAnagrafeBIZ.AreeResidenzialiPubblichePer & " " & Dt.Rows(j).Item("DistBZ_AreeResPub") & " m" & ","
                    End If
                    If Dt.Rows(j).Item("DistBZ_Allevamenti") <> 0 Then
                        strBuffer &= My.Resources.AgronicaCoreAnagrafeBIZ.AllevamentiPer & " " & Dt.Rows(j).Item("DistBZ_Allevamenti") & " m" & ","
                    End If
                    If Dt.Rows(j).Item("DistBZ_VegNatNonColt") <> 0 Then
                        strBuffer &= My.Resources.AgronicaCoreAnagrafeBIZ.VegetazioneNaturaleNonColtivataPer & " " & Dt.Rows(j).Item("DistBZ_VegNatNonColt") & " m" & ","
                    End If

                    If strBuffer <> "" Then
                        strBuffer = " - <b>" & My.Resources.AgronicaCoreAnagrafeBIZ.ContiguoA & " " & Left(strBuffer, strBuffer.Length - 1) & "</b>"
                    End If

                    If Dt.Rows(j).Item("SupBZ_Riduzione") <> 0 Then
                        strBuffer &= " - <b>" & My.Resources.AgronicaCoreAnagrafeBIZ.OffsetCapezzagnadi & " " & Dt.Rows(j).Item("SupBZ_Riduzione") & " m</b>"
                    End If


                End If

                If strBuffer <> "" Then
                    Dt.Rows(j).Item("app_nome") &= strBuffer
                End If


                'Unione colonne Regolamento e Disciplinare
                Dim Regolamento_Cod = Dt.Rows(j).Item("Regolamento")
                Dim Disciplinare_Cod = Dt.Rows(j).Item("Disciplinare_Cod")
                Dim Disciplinare_Des As String = ""

                If Regolamento_Cod = 4 Then
                    Disciplinare_Des = "BIO"
                Else
                    If Disciplinare_Cod > 0 Then
                        Disciplinare_Des = Dt.Rows(j).Item("Disciplinare")
                    Else
                        Disciplinare_Des = Gias.Nessuno
                    End If
                End If

                Dt.Rows(j).Item("Disciplinare_Des") = Disciplinare_Des

            Next

        End If

        'TODO DA CHIEDERE COME GESTIRE IN FUTURO CON NUOVO MODELLO ANGULAR
        '(31/07/2018 fede) aggiunte colonne x dati gis
        'Dim objGis As New AgronicaControlliGIS.V_M
        'Dim r As New RispostaStandard
        'r = objGis.AgendaLetturaWKT_SuListaImpianti(Lav_Cod, Dt, False, objParametri_Server)

    End Sub

    Public Function Leggi_SpecieVegetali_Attive_Impianti(ByVal Piva As String,
                                                            ByVal Sa_Cod As Integer,
                                                            ByVal Lav_Cod As Integer,
                                                            ByVal Data As Date,
                                                            ByVal FiltraImpostazioniUtente As Boolean,
                                                            ByVal consideraTerrenoNudo As Boolean,
                                                            ByVal dettagliTerrenoNudo As Boolean,
                                                            ByRef objParametri_Server As AgronicaCoreParametri,
                                                            ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                            Optional ByVal Filtra_Validita_Esercizi As Boolean = False
                                                            ) As List(Of AgronicaCoreModelsSTD.metaschema.utilizzi.UtilizzoTerreno)

        Dim UtilizzoTerrenoList As New List(Of AgronicaCoreModelsSTD.metaschema.utilizzi.UtilizzoTerreno)

        Dim leggiAncheImpiantiBloccati As Boolean = False

        If FiltraImpostazioniUtente Then

            Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim Dt_Impostazioni_Super As DataTable = ObjUtenti.Leggi(0,
                                                    2,
                                                    AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                    "",
                                                    "",
                                                    objParametri_Utenti)

            For i = 0 To Dt_Impostazioni_Super.Rows.Count - 1
                Select Case CInt(Dt_Impostazioni_Super.Rows(i).Item("Impostazione_Cod"))
                    Case enum_Impostazioni_Utenti.SUPERUSER_COD_PERMETTI_OPERAZIONI_SU_IMPIANTI_BLOCCATI
                        Select Case Dt_Impostazioni_Super.Rows(i).Item("Impostazione_Valore_1")
                            Case "0"
                                leggiAncheImpiantiBloccati = False
                            Case "1"
                                leggiAncheImpiantiBloccati = True
                            Case Else
                                leggiAncheImpiantiBloccati = False
                        End Select
                End Select
            Next

        End If

        Dim ddl_Specie As New DropDownList

        Dim clc = New AgronicaCoreUtility.CaricaListControl

        clc.SpecieVegetale_Coltivate_Optimize(ddl_Specie,
                                              Piva,
                                             Sa_Cod,
                                             Data,
                                             consideraTerrenoNudo,
                                            False,
                                            "",
                                            0,
                                            0,
                                                "",
                                            True,
                                            0,
                                                0, 0, dettagliTerrenoNudo,
                                                "",
                                                "",
                                                objParametri_Server, objParametri_Utenti,
                                                leggiAncheImpiantiBloccati,
                                                0, "", Filtra_Validita_Esercizi:=Filtra_Validita_Esercizi)

        'N.B:
        'Lo SpecieVegetale_Coltivate_Optimize restituisce solo il Veg_Cod quando carica una Specie,
        'mentre per le Destinazioni D'Uso viene restituito 0/codice destinazione

        For Each item In ddl_Specie.Items

            Dim val = CStr(item.Value).Split("/")

            If Not IsNothing(val) AndAlso val.Length > 0 Then

                If val.Length = 1 Then

                    Dim codice As Integer = val(0)


                    If codice = 0 Then

                        'Caso Terreno Nudo
                        Dim terrenoNudo As New AgronicaCoreModelsSTD.metaschema.utilizzi.DestinazioneUso

                        terrenoNudo.codice = codice

                        terrenoNudo.descrizione = item.Text

                        UtilizzoTerrenoList.Add(terrenoNudo)

                    Else

                        Dim varieta As New AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta(0)

                        Dim newspecie As New AgronicaCoreModelsSTD.metaschema.utilizzi.Specie(codice)

                        newspecie.descrizione = item.Text

                        varieta.specie = newspecie

                        UtilizzoTerrenoList.Add(varieta)

                    End If



                ElseIf val.Length > 1 Then

                    Dim newdestinazioneuso As New AgronicaCoreModelsSTD.metaschema.utilizzi.DestinazioneUso

                    newdestinazioneuso.codice = CInt(val(1))

                    newdestinazioneuso.descrizione = item.Text

                    UtilizzoTerrenoList.Add(newdestinazioneuso)

                End If

            End If

        Next


        Return UtilizzoTerrenoList

    End Function

    Public Function Leggi_SpecieVegetali_Impianti_NoFiltroData(ByVal Piva As String,
                                                               ByVal Sa_Cod As Integer,
                                                               ByVal FiltraImpostazioniUtente As Boolean,
                                                               ByVal consideraTerrenoNudo As Boolean,
                                                               ByVal dettagliTerrenoNudo As Boolean,
                                                               ByRef objParametri_Server As AgronicaCoreParametri,
                                                               ByRef objParametri_Utenti As AgronicaCoreParametri
                                                               ) As List(Of AgronicaCoreModelsSTD.metaschema.utilizzi.UtilizzoTerreno)

        Dim UtilizzoTerrenoList As New List(Of AgronicaCoreModelsSTD.metaschema.utilizzi.UtilizzoTerreno)

        Dim leggiAncheImpiantiBloccati As Boolean = False

        If FiltraImpostazioniUtente Then

            Dim ObjUtenti As New Utenti_Impostazioni_Read
            Dim valoreImpostazione = ObjUtenti.LeggiValoreImpostazioneScalare(enum_Impostazioni_Utenti.SUPERUSER_COD_PERMETTI_OPERAZIONI_SU_IMPIANTI_BLOCCATI, objParametri_Utenti.UtenteUsername, objParametri_Utenti)

            Select Case valoreImpostazione
                Case "0"
                    leggiAncheImpiantiBloccati = False
                Case "1"
                    leggiAncheImpiantiBloccati = True
                Case Else
                    leggiAncheImpiantiBloccati = False
            End Select

        End If

        Dim ddl_Specie As New DropDownList

        Dim clc = New AgronicaCoreUtility.CaricaListControl

        clc.TutteSpecieColtivate_3(ddl_Specie,
                                   False,
                                   "",
                                   0,
                                   Piva,
                                   Sa_Cod,
                                   AGRODATAINIZIO,
                                   AGRODATAFINE,
                                   consideraTerrenoNudo,
                                   "",
                                   "",
                                   objParametri_Server,
                                   leggiAncheImpiantiBloccati)

        'N.B:
        'Lo SpecieVegetale_Coltivate_Optimize restituisce solo il Veg_Cod quando carica una Specie,
        'mentre per le Destinazioni D'Uso viene restituito 0/codice destinazione

        For Each item In ddl_Specie.Items

            Dim val = CStr(item.Value).Split("/")

            If Not IsNothing(val) AndAlso val.Length > 0 Then

                If val.Length = 1 Then

                    Dim codice As Integer = val(0)


                    If codice = 0 Then

                        'Caso Terreno Nudo
                        Dim terrenoNudo As New AgronicaCoreModelsSTD.metaschema.utilizzi.DestinazioneUso

                        terrenoNudo.codice = codice

                        terrenoNudo.descrizione = item.Text

                        UtilizzoTerrenoList.Add(terrenoNudo)

                    Else

                        Dim varieta As New AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta(0)

                        Dim newspecie As New AgronicaCoreModelsSTD.metaschema.utilizzi.Specie(codice)

                        newspecie.descrizione = item.Text

                        varieta.specie = newspecie

                        UtilizzoTerrenoList.Add(varieta)

                    End If



                ElseIf val.Length > 1 Then

                    Dim newdestinazioneuso As New AgronicaCoreModelsSTD.metaschema.utilizzi.DestinazioneUso

                    newdestinazioneuso.codice = CInt(val(1))

                    newdestinazioneuso.descrizione = item.Text

                    UtilizzoTerrenoList.Add(newdestinazioneuso)

                End If

            End If

        Next


        Return UtilizzoTerrenoList

    End Function

    Public Function Leggi_Esercizi_Anagrafica(ByVal Piva As String,
                                              ByVal Sa_Cod As Long,
                                              ByVal Appezza As Long,
                                              ByVal Id_Reg As Long,
                                              ByVal Campo_Cod As Long,
                                              ByVal Data_Filtro As Date,
                                              ByRef objParametri_Server As AgronicaCoreParametri,
                                              ByRef objParametri_Utenti As AgronicaCoreParametri) As DataTable

        Dim objImprese_Progetti As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
        Dim FinestraTemporaleInizio = objParametri_Server.FinestraTemporaleInizio
        Dim FinestraTemporaleFine = objParametri_Server.FinestraTemporaleFine

        If Data_Filtro <> AGRODATAINIZIO Then
            objParametri_Server.FinestraTemporaleInizio = Data_Filtro
            objParametri_Server.FinestraTemporaleFine = Data_Filtro
        End If

        Dim leggiStaticMap As Boolean = False
        Dim LetturaCFG As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim jSonStaticMapCFG As String = LetturaCFG.Leggi_Valore(0, "GIS_StaticMapCFG", "", "", objParametri_Server)

        If jSonStaticMapCFG <> "" Then
            Dim StaticMapCFG As GeneraMappaStaticaInData = JsonConvert.DeserializeObject(Of GeneraMappaStaticaInData)(jSonStaticMapCFG)
            If StaticMapCFG.StaticMapAttive Then
                leggiStaticMap = True
            End If
        End If

        Dim escludiFiltroCFT As String = LetturaCFG.Leggi_Valore(0, "GIS_EscludiFiltroCodiceFiscaleTecnico", "", "", objParametri_Server)
        Dim Codice_Fiscale_Tecnico As String = ""

        If escludiFiltroCFT <> "" AndAlso Not Boolean.Parse(escludiFiltroCFT) Then
            Dim xUtenteCorrente As String = CType(objParametri_Server, AgronicaCoreDataProvider.AgronicaCoreParametri).UtenteUsername
            Dim objUtentiDAL As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_R

            Codice_Fiscale_Tecnico = objUtentiDAL.Leggi_IdentificativoGruppoUtenti_Singolo(xUtenteCorrente, objParametri_Utenti)
        End If

        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim leggiDatiRibaltamento As Boolean = objPermessi.Controlla_Permessi_Utente(
            objParametri_Utenti.UtenteUsername,
            enum_Id_Servizio.GiasOnline,
            enum_Security_Attivita.Budget_Ribaltamento_Su_Reale,
            enum_Security_Operazione.Lettura,
            Date.Now,
            "",
            objParametri_Utenti
            )

        Dim readLinkedMachines As Boolean = objPermessi.Controlla_Permessi_Utente(
            objParametri_Utenti.UtenteUsername,
            enum_Id_Servizio.GiasOnline,
            enum_Security_Attivita.GestioneAssociazioneAppezzamentiXParcoMacchine,
            enum_Security_Operazione.Lettura,
            Date.Now,
            "",
            objParametri_Utenti
            )

        Dim dt = objImprese_Progetti.Leggi_x_anagraficaNG(
            Piva,
            Sa_Cod,
            Appezza,
            Id_Reg,
            Campo_Cod,
            Codice_Fiscale_Tecnico,
            "",
            "",
            objParametri_Server,
            Date.Now,
            leggiStaticMap,
            leggiDatiRibaltamento,
            readLinkedMachines:=readLinkedMachines
            )

        Dim centres = dt.AsEnumerable().Select(Of KeyValuePair(Of String, Int32))(
            Function(r) New KeyValuePair(Of String, Int32)(r.Item("PIVA"), r.Item("SA_COD"))
            )

        Dim centresSet As New HashSet(Of KeyValuePair(Of String, Int32))(centres)

        For Each c In centresSet.AsEnumerable
            Dim exPiva = c.Key
            Dim exSaCod = c.Value

            Dim replicaPiva As String = Replica_GIAS.VerificaConfigurazione(exPiva, exSaCod, objParametri_Server)
            Dim replicaRagioneSociale As String = String.Empty

            If replicaPiva <> "" Then
                Dim objanag As New AgronicaCoreAnagrafeDAL.Imprese_Read
                replicaRagioneSociale = objanag.RagSoc_from_Piva(replicaPiva, objParametri_Server)
            End If

            For Each r In dt.Rows
                If r.Item("PIVA") = exPiva AndAlso r.Item("SA_COD") = exSaCod Then
                    r.Item("replicaGiasPiva") = replicaPiva
                    r.Item("replicaGiasRagioneSociale") = replicaRagioneSociale
                End If
            Next
        Next

        If Data_Filtro <> AGRODATAINIZIO Then
            objParametri_Server.FinestraTemporaleInizio = FinestraTemporaleInizio
            objParametri_Server.FinestraTemporaleFine = FinestraTemporaleFine
        End If

        Return dt
    End Function

    ''' <summary>
    ''' Returns True if the user is allowed to read also blocked implants, depending on User Settings, False otherwise
    ''' </summary>
    ''' <param name="objParametriUtenti"></param>
    ''' <returns></returns>
    Public Function BlockedImplantsReadSetting(ByVal objParametriUtenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Dim LeggiAncheBloccati As Boolean = False
        Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim Dt_Impostazioni_Super As DataTable = ObjUtenti.Leggi(
            0,
            2,
            AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
            "",
            "",
            objParametriUtenti
            )

        For i = 0 To Dt_Impostazioni_Super.Rows.Count - 1
            Select Case CInt(Dt_Impostazioni_Super.Rows(i).Item("Impostazione_Cod"))
                Case enum_Impostazioni_Utenti.SUPERUSER_COD_PERMETTI_OPERAZIONI_SU_IMPIANTI_BLOCCATI
                    Select Case Dt_Impostazioni_Super.Rows(i).Item("Impostazione_Valore_1")
                        Case "0"
                            LeggiAncheBloccati = False
                        Case "1"
                            LeggiAncheBloccati = True
                        Case Else
                            LeggiAncheBloccati = False
                    End Select
            End Select
        Next

        Return LeggiAncheBloccati
    End Function

    Private Function Calcola_Percentuale_N_P_K_Cu_Mg(ByVal Massimale As Decimal, ByVal GiaDistribuito As Decimal)

        'Arrotondo la percentuale a 2 decimali

        Dim Percentuale_Gia_Distribuita As Decimal = 0

        If Massimale > 0 Then
            Percentuale_Gia_Distribuita = Math.Round(GiaDistribuito / Massimale * 100, 2)
        End If

        Return Percentuale_Gia_Distribuita
    End Function


End Class


'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§


Public Class Reg_Impianto_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Cancella(
        ByVal xpiva As String,
        ByVal xsa_Cod As Integer,
        ByVal xappezza As Integer,
        ByVal xId_Imp As Integer,
        ByVal Utente_Username As String,
        ByVal idServizio As Integer,
        ByVal objParametri_Server As AgronicaCoreParametri,
        ByVal objParametri_Utenti As AgronicaCoreParametri,
        Optional NoteLog As String = ""
     ) As RispostaStandard

        Dim result As New RispostaStandard


        If Not (xpiva <> "" AndAlso xsa_Cod <> 0 AndAlso xappezza <> 0 AndAlso xId_Imp <> 0) Then
            result.RispostaOK = False
            result.Errore = "La funzione di cancellazione è stata chiamata con parametri non validi."
            Return result
        End If

        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim UtenteAbilitato As Boolean = objPermessi.Controlla_Permessi_Utente(
                                    Utente_Username,
                                    idServizio,
                                    enum_Security_Attivita.Anagrafica_Impianto,
                                    enum_Security_Operazione.Modifica,
                                    Date.Now,
                                    "",
                                    objParametri_Utenti)


        If Not UtenteAbilitato Then
            result.RispostaOK = False
            result.Errore = "Non si dispone dei permessi per cancellare l'impianto."
            Return result
        End If

        'controllo che non ci siano registrazioni di agenda riferite all'impianto

        Dim ObjAgenda As New AgronicaCoreContabDAL.Mov_Destinazioni_R
        Dim ObjRicette As New AgronicaCoreContabDAL.Ricette_Destinazioni_R
        Dim DtAgenda As DataTable
        Dim DtRicette As DataTable

        DtAgenda = ObjAgenda.Leggi(CStr(xpiva),
                                 CInt(xsa_Cod),
                                 0,
                                 0,
                                 0,
                                 CInt(xappezza),
                                 xId_Imp,
                                 0,
                                 enumSelezioneVariabile.Selezione_TabellaCompleta,
                                 "",
                                 "",
                                 objParametri_Server)

        DtRicette = ObjRicette.Leggi(0, 0, 0, 0, 0,
                                             CStr(xpiva),
                                             CInt(xsa_Cod),
                                             CInt(xappezza),
                                             xId_Imp,
                                             AGRODATAINIZIO,
                                             AGRODATAFINE,
                                             enumSelezioneVariabile.Selezione_TabellaCompleta,
                                             "",
                                             "",
                                             objParametri_Server)


        If DtAgenda.Rows.Count <> 0 OrElse DtRicette.Rows.Count <> 0 Then
            'Throw New Exception("Impossibile eliminare l'impianto poiché esistono delle registrazioni ad esso associate!" & Chr(13) & _
            '            " Per poter procedere con l'eliminazione occorre cancellare i dati tramite il modulo 'Agenda'!")

            result.RispostaOK = False
            result.Errore = "Impossibile eliminare l'impianto, perché esistono delle registrazioni ad esso associate!" & Chr(13) &
                        " Per poter procedere con l'eliminazione occorre cancellare i dati tramite il modulo 'Agenda'!"
            Return result
        Else


            'controllo se l'appezzamento è bloccato
            Dim objAppezza_R As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
            Dim DT_Appezza As DataTable
            DT_Appezza = objAppezza_R.Leggi(xpiva, xsa_Cod, xappezza, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)


            If DT_Appezza.Rows(0).Item("blk_Flag") = -1 Then
                Throw New Exception("Impossibile eliminare l'impianto poichè è stato bloccato da smart!")
            End If
            'If DT_Appezza.Rows(0).Item("blk_Flag") = 1 And Me.SI_NO.Value = 0 Then
            '    Dim Testo As String
            '    Testo = "E' stata fatta una misura tramite Palmare, si desidera comunque cancellare?"
            '    Testo = Server.UrlEncode(Testo)
            '    Dim Stringa As String = "<script language='javascript'> " & _
            '                        " a = window.showModalDialog(" & Chr(34) & "../AA_Script/Controlli/AgroSiNo/AgroSiNo.aspx?des=" & Testo & Chr(34) & "," & Chr(34) & Chr(34) & "," & Chr(34) & "dialogWidth:260px;dialogHeight:350px;status:no; center:yes;edge:raised;" & Chr(34) & ") " & _
            '                        vbCrLf & " document.all(" & Chr(34) & "SI_NO" & Chr(34) & ").value = a" & _
            '                        vbCrLf & " rispostaSiNo();" & _
            '                        "</script>"
            '    Me.FindControl("Form1").Controls.Add(New LiteralControl(Stringa))
            '    Exit Function
            'End If

            'SI_NO.Value = 0

            ' controllo che non ci siano movimenti CdG collegati all'impianto
            Dim objCDG As New AgronicaCoreContabDAL.CDG_DAL_R
            If objCDG.Verifica_CDG_Impianti(CStr(xpiva), CInt(xsa_Cod), CInt(xappezza), CInt(xId_Imp), Nothing, objParametri_Server) Then
                result.RispostaOK = False
                result.Errore = "Impossibile eliminare l'impianto, perché esistono dei Costi di Gestione associati ad un esercizio!"
                Return result
            End If


            'CANCELLIAMO I POLIGONI IN Reg_Impianto_Scrivi()
            'controllo se l'Impianto è collegato ad un Entità GIS
            'Dim objGIS As New AgronicaCoreGisBIZ.GIS_Entita_R
            'If (objGIS.esisteGisEntita_cancellazioneElementoAnagrafico(xpiva, xsa_Cod, xappezza, 0, xId_Imp, enum_GIS2012_TipoEntita.IMPIANTI_ORTICOLA, objParametri_Server)) Then
            '    result.RispostaOK = False
            '    result.Errore = Gias.ImpossibileCancellareImpiantoCollegatoPoligono
            '    Return result
            'End If


            Dim objImpiantoR As New AgronicaCoreAnagrafeBIZ.Reg_Impianto_R

            'Leggo la stringa XML dell'oggetto
            Dim dati As String = objImpiantoR.Reg_Impianto_Leggi(
                                    CStr(xpiva),
                                    CInt(xsa_Cod),
                                    CInt(xappezza),
                                    CInt(xId_Imp),
                                    CBool(True),
                                    CBool(True),
                                    objParametri_Server)

            objImpiantoR = Nothing

            If dati <> "" Then
                'Cancello l'elemento
                Dim rvalImpiantoScrivi As Boolean = Reg_Impianto_Scrivi(
                                        CStr(dati),
                                        Nothing,
                                        Nothing,
                                        Nothing,
                                        Nothing,
                                        "",
                                        objParametri_Server,
                                        NoteLog:=NoteLog)

                If Not rvalImpiantoScrivi Then
                    result.RispostaOK = False
                    result.RispostaStringa = "Si è verificato un errore in fase di scrittura. Ripetere l'operazione"
                    Return result
                End If
            End If

        End If


        result.RispostaOK = True
        result.RispostaStringa = "Impianto correttamente cancellato"


        Return result

    End Function

    Public Function Reg_Impianto_Scrivi(ByVal DatiReg_Impianto As String,
                                        ByRef OUTPUT_Piva As String,
                                        ByRef OUTPUT_Sa_Cod As Integer,
                                        ByRef OUTPUT_Appezza As Integer,
                                        ByRef OUTPUT_Id_Reg As Integer,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByRef objParametri As AgronicaCoreParametri,
                                        Optional ByVal idServizio As enum_Id_Servizio = enum_Id_Servizio.GiasOnline,
                                        Optional ByVal TipoG2G As Integer = 0,
                                        Optional ByVal Date_Modifiche_Progetti_Verificate As Boolean = False,
                                        Optional NoteLog As String = "") As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeBIZ.Reg_Impianto_W.Reg_Impianto_Scrivi()"

        Dim messaggioErrore As String = ""
        Dim xRisp As Boolean = True

        Dim flagTransazioneLocale As Boolean = False
        Dim flagConnessioneLocale As Boolean = False
        '------------------------------
        Dim objSequenze As New Agro_Sequenze
        Dim objRegImpiantiW As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Write
        Dim objCodiciR As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_W
        Dim objDestR As New AgronicaCoreContabDAL.Mov_Destinazioni_R
        Dim ObjRicette As New AgronicaCoreContabDAL.Ricette_Destinazioni_R
        Dim objPianoConcimazione_R As New AgronicaCoreAnagrafeDAL.PianoConcimazione_EntitaxTestata_R
        Dim objPianoConcimazione_W As New AgronicaCoreAnagrafeDAL.PianoConcimazione_EntitaxTestata_W
        Dim objProgettoW As New AgronicaCoreAnagrafeBIZ.Progetto_W
        Dim objAgronicaLogAnagrafeW As New AgronicaCoreAnagrafeDAL.AgronicaLogAnagrafe_W

        Dim objGraficaBizW As New AgronicaCoreGraficaBIZ.Grafica_Write
        Dim objGraficaDalW As New AgronicaCoreGraficaDAL.Grafica_Write

        Dim chiaveGrafica As String = ""
        Dim dtAgenda As New DataTable
        Dim dtRicette As New DataTable
        Dim dtPianoConcimazione As New DataTable
        Dim dummy As Long
        Dim codRegImpianto As Long
        Dim resLog As Boolean = False

        Dim xmlDoc As XmlDocument

        Dim xDatiReg_Impianti As XmlNodeList
        Dim xDatiReg_Impianto As XmlElement
        Dim xReg_Impianti As XmlNodeList
        Dim xReg_Impianto As XmlElement

        Dim xDatiCodiciImpianto As XmlElement
        Dim xCodici As XmlNodeList
        Dim xCodice As XmlElement
        Dim xDatiImprese_Progetti As XmlNodeList
        Dim xImprese_Progetti As XmlElement

        Dim xEntitaGrafiche As XmlNodeList

        Dim i_DatiReg_Impianto As Integer
        Dim i_Reg_Impianto As Integer

        Dim i_Codice As Integer

        Dim i_DatiImprese_Progetti As Integer

        Dim OpeDB_Reg_Impianto As String
        Dim OpeDB_Codice As String

        Dim graphicKey As String = ""
        Dim baseCode As Long

        Try

            '------------------------------
            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            ConnessioniTransazioni.ApriConnessioneXCoreBiz(flagConnessioneLocale,
                                                           flagTransazioneLocale,
                                                           objParametri)

            '------------------------------

            xmlDoc = New XmlDocument
            xmlDoc.LoadXml(DatiReg_Impianto)

            '------------------------------

            xDatiReg_Impianti = xmlDoc.GetElementsByTagName("DatiReg_Impianti")

            i_DatiReg_Impianto = 0

            Do While i_DatiReg_Impianto < xDatiReg_Impianti.Count

                'Prelevo l'i-esimo blocco di Datireg_impianti (in realtà ne esiste uno solo)
                xDatiReg_Impianto = xDatiReg_Impianti.Item(i_DatiReg_Impianto)

                '------------------------------

                xReg_Impianti = xDatiReg_Impianto.GetElementsByTagName("Reg_Impianto")

                i_Reg_Impianto = 0

                Do While i_Reg_Impianto < xReg_Impianti.Count

                    'Prelevo l' i-esimo reg_impianto
                    xReg_Impianto = xReg_Impianti.Item(i_Reg_Impianto)

                    'Prelevo gli attributi del reg_impianto selezionato
                    OpeDB_Reg_Impianto = xReg_Impianto.GetAttribute("TipoOperazioneDB")

                    'Inizializzo Preventivamente il Cod_reg_impianto
                    codRegImpianto = CInt(xReg_Impianto.GetAttribute("id_reg"))

                    'validita_inizio_impianto = CDate(xReg_Impianto.getAttribute("validita_inizio"))

                    'Verifico l'operazione richiesta
                    Select Case OpeDB_Reg_Impianto

                        Case "0"    'LEGGI -------------------------------------------------------

                            OUTPUT_Piva = CStr(xReg_Impianto.GetAttribute("piva"))
                            OUTPUT_Sa_Cod = CInt(xReg_Impianto.GetAttribute("sa_cod"))
                            OUTPUT_Appezza = CInt(xReg_Impianto.GetAttribute("appezza"))
                            OUTPUT_Id_Reg = CInt(xReg_Impianto.GetAttribute("id_reg"))

                        Case "1"    'SALVA -------------------------------------------------------

                            If codRegImpianto <= 0 Then

                                codRegImpianto = objSequenze.NuovoId_Reg_Impianti(CStr(xReg_Impianto.GetAttribute("piva")),
                                                                                  CInt(xReg_Impianto.GetAttribute("sa_cod")),
                                                                                  CInt(xReg_Impianto.GetAttribute("appezza")),
                                                                                  CInt(xReg_Impianto.GetAttribute("basecode")),
                                                                                  CInt(xReg_Impianto.GetAttribute("topcode")),
                                                                                  objParametri)

                                OUTPUT_Piva = CStr(xReg_Impianto.GetAttribute("piva"))
                                OUTPUT_Sa_Cod = CInt(xReg_Impianto.GetAttribute("sa_cod"))
                                OUTPUT_Appezza = CInt(xReg_Impianto.GetAttribute("appezza"))
                                OUTPUT_Id_Reg = codRegImpianto

                            Else

                                'Esportazione dell'impianto in Locale

                                OUTPUT_Piva = CStr(xReg_Impianto.GetAttribute("piva"))
                                OUTPUT_Sa_Cod = CInt(xReg_Impianto.GetAttribute("sa_cod"))
                                OUTPUT_Appezza = CInt(xReg_Impianto.GetAttribute("appezza"))
                                OUTPUT_Id_Reg = CInt(xReg_Impianto.GetAttribute("id_reg"))

                            End If

                            dummy = objRegImpiantiW.Scrivi(CStr(xReg_Impianto.GetAttribute("piva")),
                                                           CInt(xReg_Impianto.GetAttribute("sa_cod")),
                                                           CInt(xReg_Impianto.GetAttribute("appezza")),
                                                           codRegImpianto,
                                                           Agro_XML_GetInteger(xReg_Impianto, "cod_resp", 0),
                                                           Agro_XML_GetInteger(xReg_Impianto, "cod_ente", 0),
                                                           Agro_XML_GetInteger(xReg_Impianto, "campo_spia", 0),
                                                           Agro_XML_GetDate(xReg_Impianto, "data", AGRODATAINIZIO),
                                                           CInt(xReg_Impianto.GetAttribute("cul_cod")),
                                                           Agro_XML_GetInteger(xReg_Impianto, "grfi_cod", -1),
                                                           Agro_XML_GetDateString(xReg_Impianto, "data_raccolta", New Date),
                                                           CDbl(xReg_Impianto.GetAttribute("resa_prevista")),
                                                           CDbl(xReg_Impianto.GetAttribute("resa_effettiva")),
                                                           Agro_XML_GetInteger(xReg_Impianto, "scarto", 0),
                                                           Agro_XML_GetInteger(xReg_Impianto, "ind_mat_cod", 0),
                                                           Agro_XML_GetInteger(xReg_Impianto, "ind_mat_ril", 0),
                                                           Agro_XML_GetString(xReg_Impianto, "sta_ter", ""),
                                                           Agro_XML_GetDateString(xReg_Impianto, "cop_di", New Date),
                                                           Agro_XML_GetDateString(xReg_Impianto, "cop_df", New Date),
                                                           CDbl(xReg_Impianto.GetAttribute("tra_fila")),
                                                           CDbl(xReg_Impianto.GetAttribute("su_fila")),
                                                           Agro_XML_GetDateString(xReg_Impianto, "Data_Inizio_Portinnesto", New Date),' - - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -
                                                           Agro_XML_GetDateString(xReg_Impianto, "Data_Inizio_Innesto", New Date),
                                                           Agro_XML_GetDateString(xReg_Impianto, "Data_Inizio_Produzione", New Date),
                                                           Agro_XML_GetInteger(xReg_Impianto, "Piante_Maschi_InSesto", 0), ' - - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -
                                                           Agro_XML_GetInteger(xReg_Impianto, "foral_cod", -1),
                                                           CStr(xReg_Impianto.GetAttribute("setup_cod")),
                                                           CInt(xReg_Impianto.GetAttribute("port_cod")),
                                                           Agro_XML_GetInteger(xReg_Impianto, "imp_cod", -1),
                                                           CInt(xReg_Impianto.GetAttribute("stru_prot")),
                                                           Agro_XML_GetInteger(xReg_Impianto, "pro_pag", 0),
                                                           Agro_XML_GetInteger(xReg_Impianto, "seme_q", 0),
                                                           Agro_XML_GetInteger(xReg_Impianto, "seme_t", 0),
                                                           Agro_XML_GetInteger(xReg_Impianto, "seme_p", 0),
                                                           Agro_XML_GetInteger(xReg_Impianto, "seme_d", 0),
                                                           Agro_XML_GetString(xReg_Impianto, "stato_residui", ""),
                                                           Agro_XML_GetInteger(xReg_Impianto, "tecn_cod", -1),
                                                           Agro_XML_GetInteger(xReg_Impianto, "denitrificazione", 0),
                                                           Agro_XML_GetInteger(xReg_Impianto, "volatilizzazione", 0),
                                                           Agro_XML_GetInteger(xReg_Impianto, "grva_cod_veg", 0),
                                                           Agro_XML_GetInteger(xReg_Impianto, "profonditalav", 0),
                                                           Agro_XML_GetInteger(xReg_Impianto, "id_campo", 0),
                                                           Agro_XML_GetInteger(xReg_Impianto, "su_cod", -1),
                                                           Agro_XML_GetInteger(xReg_Impianto, "cop_cod", -1),
                                                           CInt(xReg_Impianto.GetAttribute("cover")),
                                                           CInt(xReg_Impianto.GetAttribute("monitorato")),
                                                           Agro_XML_GetString(xReg_Impianto, "codice_fiscale_tecnico", ""),
                                                           Agro_XML_GetInteger(xReg_Impianto, "regolamento", 1),
                                                           Agro_XML_GetInteger(xReg_Impianto, "finanziamento", 1),
                                                           Agro_XML_GetDateString(xReg_Impianto, "data_conversione", New Date),
                                                           Agro_XML_GetInteger(xReg_Impianto, "provenienzaseme", 0),
                                                           Agro_XML_GetDecimal(xReg_Impianto, "sup_imp", 0),
                                                           Agro_XML_GetInteger(xReg_Impianto, "id_consociazione", 0),
                                                           CDate(xReg_Impianto.GetAttribute("validita_inizio")),
                                                           CDate(xReg_Impianto.GetAttribute("validita_fine")),
                                                           objParametri,
                                                           Data_creazione:=Agro_XML_GetDate(xReg_Impianto, "data_creazione", #2/1/1900#),
                                                           Data_modifica:=Agro_XML_GetDate(xReg_Impianto, "data_modifica", #2/1/1900#),
                                                           username_creazione:=Agro_XML_GetString(xReg_Impianto, "username_creazione", ""),
                                                           username_modifica:=Agro_XML_GetString(xReg_Impianto, "username_modifica", ""),
                                                           Unita_Vitata:=Agro_XML_GetInteger(xReg_Impianto, "unita_vitata", 0),
                                                           PRODUZIONE:=Agro_XML_GetInteger(xReg_Impianto, "produzione", 0),
                                                           Sovrainnesto_Cod:=Agro_XML_GetInteger(xReg_Impianto, "sovrainnesto_cod", 0),
                                                           Data_Inizio_Impianto:=Agro_XML_GetDate(xReg_Impianto, "Data_Inizio_Impianto", AGRODATAINIZIO)
                                                           )

                            '************************************************
                            '*********** INIZIO LOGGING *******************
                            '************************************************

                            'MEMORIZZO L'INSERIMENTO DELL'OPERAZIONE NELLA TABELLA DI LOG (AGRONICA LOG ANAGRAFE)
                            'è necessario avere Migra >= 505

                            resLog = objAgronicaLogAnagrafeW.Scrivi(CInt(OpeDB_Reg_Impianto),
                                                                    enum_TipoEntita_Des.Impianti,
                                                                    CStr(xReg_Impianto.GetAttribute("piva")),
                                                                    CStr(xReg_Impianto.GetAttribute("sa_cod")),
                                                                    CStr(xReg_Impianto.GetAttribute("appezza")),
                                                                    CStr(codRegImpianto),
                                                                    Nothing,
                                                                    Nothing,
                                                                    NoteLog, CInt(idServizio), objParametri)

                            '************************************************
                            '*********** FINE LOGGING *********************
                            '************************************************

                        Case "2"    'MODIFICA -------------------------------------------------------

                            OUTPUT_Piva = CStr(xReg_Impianto.GetAttribute("piva"))
                            OUTPUT_Sa_Cod = CInt(xReg_Impianto.GetAttribute("sa_cod"))
                            OUTPUT_Appezza = CInt(xReg_Impianto.GetAttribute("appezza"))
                            OUTPUT_Id_Reg = CInt(xReg_Impianto.GetAttribute("id_reg"))

                            Dim Cul_Cod As Integer = CInt(xReg_Impianto.GetAttribute("cul_cod"))
                            Dim Sup_Imp As Decimal = Agro_XML_GetDecimal(xReg_Impianto, "sup_imp", 0)
                            Dim Grfi_Cod As Decimal = Agro_XML_GetInteger(xReg_Impianto, "grfi_cod", -1)

                            '-----------------------------------------------------------
                            'CONTROLLO Operazioni d'agenda
                            'se cambia la sup --> update del campo flag nella tabella agenda

                            Dim a As Integer

                            dtAgenda = objDestR.Leggi(CStr(OUTPUT_Piva),
                                                     CInt(OUTPUT_Sa_Cod),
                                                     0,
                                                     0,
                                                     0,
                                                     CInt(OUTPUT_Appezza),
                                                     CInt(OUTPUT_Id_Reg),
                                                     0,
                                                     enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                     "",
                                                     "",
                                                     objParametri)

                            If Not IsNothing(dtAgenda) AndAlso dtAgenda.Rows.Count > 0 Then

                                Dim Reg_Impianti_Read As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                                Dim dtreg As DataTable = Reg_Impianti_Read.Leggi_DescrizioniImpianti(CStr(OUTPUT_Piva),
                                                                                                    CInt(OUTPUT_Sa_Cod),
                                                                                                    CInt(OUTPUT_Appezza),
                                                                                                    CInt(OUTPUT_Id_Reg),
                                                                                                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                                    "", "",
                                                                                                    objParametri)



                                Dim Veg_Cod_Old As Integer = CInt(dtreg.Rows(0).Item("Veg_Cod"))
                                Dim Cul_Cod_Old As Integer = CInt(dtreg.Rows(0).Item("Cul_Cod"))
                                Dim Grfi_Cod_Old As Integer = CInt(dtreg.Rows(0).Item("Grfi_Cod"))
                                Dim Sup_Imp_Old As Decimal = CDbl(dtreg.Rows(0).Item("Sup_Imp"))

                                'se cambia la sup --> update del campo flag nella tabella agenda
                                '2017/01/12 modifica per poter modificare da terreno nudo a una specie con operaz. associate
                                'se cambia la specie o la finalità partendo da terreno nudo --> update del campo flag nella tabella agenda

                                '
                                ' NB: Il controllo per le operazioni d'agenda di aver usato solo l'appezzamento coinvolto nel
                                '     cambio specie deve essere fatto prima di arrivare ai Core
                                '     SERVE perché le operazioni possono essere fatte su PIù appezz. ma SOLO UNA specie
                                '

                                If Sup_Imp <> Sup_Imp_Old OrElse ((Cul_Cod_Old <> Cul_Cod OrElse Grfi_Cod_Old <> Grfi_Cod) AndAlso Cul_Cod_Old = 0) Then
                                    Dim objAgenda As New AgronicaCoreContabDAL.Agenda_W
                                    For a = 0 To dtAgenda.Rows.Count - 1
                                        objAgenda.Agenda_SettaFlag_OpDaRiconfermare(CStr(OUTPUT_Piva),
                                                                                    dtAgenda.Rows(a).Item("id_agenda"),
                                                                                    objParametri.UtenteUsername,
                                                                                    Date.Now,
                                                                                    "",
                                                                                    objParametri)
                                    Next
                                End If

                                ''verifica che non si stia cambiando la specie
                                'If Cul_Cod <> Cul_Cod_Old Then
                                '    Dim objCul As New AgronicaCoreMetaSchemaDAL.Cultivar_R
                                '    Dim Veg_Cod As Integer = objCul.VegCod_from_CulCod(Cul_Cod, objParametri)
                                'End If
                            End If



                            '-----------------------------------------------------------

                            dummy = objRegImpiantiW.Modifica(CStr(xReg_Impianto.GetAttribute("piva")),
                                                             CInt(xReg_Impianto.GetAttribute("sa_cod")),
                                                             CInt(xReg_Impianto.GetAttribute("appezza")),
                                                             CInt(xReg_Impianto.GetAttribute("id_reg")),
                                                             Agro_XML_GetInteger(xReg_Impianto, "cod_resp", 0),
                                                             Agro_XML_GetInteger(xReg_Impianto, "cod_ente", 0),
                                                             Agro_XML_GetInteger(xReg_Impianto, "campo_spia", 0),
                                                             Agro_XML_GetDate(xReg_Impianto, "data", AGRODATAINIZIO),
                                                             Cul_Cod,
                                                             Agro_XML_GetInteger(xReg_Impianto, "grfi_cod", -1),
                                                             Agro_XML_GetDateString(xReg_Impianto, "data_raccolta", New Date),
                                                             CDbl(xReg_Impianto.GetAttribute("resa_prevista")),
                                                             CDbl(xReg_Impianto.GetAttribute("resa_effettiva")),
                                                             Agro_XML_GetInteger(xReg_Impianto, "scarto", 0),
                                                             Agro_XML_GetInteger(xReg_Impianto, "ind_mat_cod", 0),
                                                             Agro_XML_GetInteger(xReg_Impianto, "ind_mat_ril", 0),
                                                             Agro_XML_GetString(xReg_Impianto, "sta_ter", ""),
                                                             Agro_XML_GetDateString(xReg_Impianto, "cop_di", New Date),
                                                             Agro_XML_GetDateString(xReg_Impianto, "cop_df", New Date),
                                                             CDbl(xReg_Impianto.GetAttribute("tra_fila")),
                                                             CDbl(xReg_Impianto.GetAttribute("su_fila")),
                                                             Agro_XML_GetDateString(xReg_Impianto, "Data_Inizio_Portinnesto", New Date),' - - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -
                                                             Agro_XML_GetDateString(xReg_Impianto, "Data_Inizio_Innesto", New Date),
                                                             Agro_XML_GetDateString(xReg_Impianto, "Data_Inizio_Produzione", New Date),
                                                             Agro_XML_GetInteger(xReg_Impianto, "Piante_Maschi_InSesto", 0), ' - - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -
                                                             Agro_XML_GetInteger(xReg_Impianto, "foral_cod", -1),
                                                             CStr(xReg_Impianto.GetAttribute("setup_cod")),
                                                             CInt(xReg_Impianto.GetAttribute("port_cod")),
                                                             Agro_XML_GetInteger(xReg_Impianto, "imp_cod", -1),
                                                             CInt(xReg_Impianto.GetAttribute("stru_prot")),
                                                             Agro_XML_GetInteger(xReg_Impianto, "pro_pag", 0),
                                                             Agro_XML_GetInteger(xReg_Impianto, "seme_q", 0),
                                                             Agro_XML_GetInteger(xReg_Impianto, "seme_t", 0),
                                                             Agro_XML_GetInteger(xReg_Impianto, "seme_p", 0),
                                                             Agro_XML_GetInteger(xReg_Impianto, "seme_d", 0),
                                                             Agro_XML_GetString(xReg_Impianto, "stato_residui", ""),
                                                             Agro_XML_GetInteger(xReg_Impianto, "tecn_cod", -1),
                                                             Agro_XML_GetInteger(xReg_Impianto, "denitrificazione", 0),
                                                             Agro_XML_GetInteger(xReg_Impianto, "volatilizzazione", 0),
                                                             Agro_XML_GetInteger(xReg_Impianto, "grva_cod_veg", 0),
                                                             Agro_XML_GetInteger(xReg_Impianto, "profonditalav", 0),
                                                             Agro_XML_GetInteger(xReg_Impianto, "id_campo", 0),
                                                             Agro_XML_GetInteger(xReg_Impianto, "su_cod", -1),
                                                             Agro_XML_GetInteger(xReg_Impianto, "cop_cod", -1),
                                                             CInt(xReg_Impianto.GetAttribute("cover")),
                                                             CInt(xReg_Impianto.GetAttribute("monitorato")),
                                                             Agro_XML_GetString(xReg_Impianto, "codice_fiscale_tecnico", ""),
                                                             Agro_XML_GetInteger(xReg_Impianto, "regolamento", 1),
                                                             Agro_XML_GetInteger(xReg_Impianto, "finanziamento", 1),
                                                             Agro_XML_GetDateString(xReg_Impianto, "data_conversione", New Date),
                                                             Agro_XML_GetInteger(xReg_Impianto, "provenienzaseme", 0),
                                                             Sup_Imp,
                                                             Agro_XML_GetInteger(xReg_Impianto, "id_consociazione", 0),
                                                             CDate(xReg_Impianto.GetAttribute("validita_inizio")),
                                                             CDate(xReg_Impianto.GetAttribute("validita_fine")),
                                                             "",
                                                             objParametri,
                                                             Unita_Vitata:=Agro_XML_GetInteger(xReg_Impianto, "unita_vitata", 0),
                                                             PRODUZIONE:=If(Not xReg_Impianto.HasAttribute("produzione"), Nothing, CInt(xReg_Impianto.GetAttribute("produzione"))),
                                                             Sovrainnesto_Cod:=If(Not xReg_Impianto.HasAttribute("sovrainnesto_cod"), Nothing, CInt(xReg_Impianto.GetAttribute("sovrainnesto_cod")))
                                                             )

                            '************************************************
                            '*********** INIZIO LOGGING *******************
                            '************************************************

                            'MEMORIZZO L'INSERIMENTO DELL'OPERAZIONE NELLA TABELLA DI LOG (AGRONICA LOG ANAGRAFE)
                            'è necessario avere Migra >= 505

                            resLog = objAgronicaLogAnagrafeW.Scrivi(CInt(OpeDB_Reg_Impianto),
                                                                    enum_TipoEntita_Des.Impianti,
                                                                    CStr(xReg_Impianto.GetAttribute("piva")),
                                                                    CStr(xReg_Impianto.GetAttribute("sa_cod")),
                                                                    CStr(xReg_Impianto.GetAttribute("appezza")),
                                                                    CStr(xReg_Impianto.GetAttribute("id_reg")),
                                                                    Nothing,
                                                                    Nothing,
                                                                    NoteLog, CInt(idServizio), objParametri)

                            '************************************************
                            '*********** FINE LOGGING *********************
                            '************************************************

                    End Select

                    '-------------------------------------------------------------
                    ' ENTITA GRAFICHE
                    '-------------------------------------------------------------
                    xEntitaGrafiche = xReg_Impianto.GetElementsByTagName("DatiEntita")
                    If xEntitaGrafiche.Count = 1 Then

                        objSequenze.AppezzaIdReg_Comprimi(chiaveGrafica,
                                                          "0",
                                                          CInt(xReg_Impianto.GetAttribute("appezza")),
                                                          codRegImpianto,
                                                          CInt(xReg_Impianto.GetAttribute("basecode")),
                                                          objParametri)

                        If chiaveGrafica <> "-1" Then

                            objGraficaBizW.Grafica_Scrivi2005(xEntitaGrafiche.Item(0).OuterXml,
                                                              CStr(xReg_Impianto.GetAttribute("piva")),
                                                              CInt(xReg_Impianto.GetAttribute("sa_cod")),
                                                              CInt(Val("&H" & chiaveGrafica)),
                                                              True,
                                                              objParametri)

                        End If

                    End If

                    '-------------------------------------------------------------
                    ' CODICI
                    '-------------------------------------------------------------

                    ' se G2G cancello eventuali codici presenti
                    If TipoG2G <> 0 AndAlso OpeDB_Reg_Impianto = 2 Then
                        objCodiciR.Cancella(CStr(xReg_Impianto.GetAttribute("piva")),
                                            CInt(xReg_Impianto.GetAttribute("sa_cod")),
                                            CInt(xReg_Impianto.GetAttribute("appezza")),
                                            codRegImpianto,
                                            0,
                                            "Progetto_Cod=0",
                                            objParametri)
                    End If

                    'UTILIZZATO SOLO DA GIAS LAN

                    '================================================================================================
                    'Codice Destinazione Uso Terreno Nudo
                    '------------------------------------------------------------------------------------------------

                    If Agro_SQL_SaveNum(xReg_Impianto.GetAttribute("destinazioneuso_old"), False) <> Agro_SQL_SaveNum(xReg_Impianto.GetAttribute("destinazioneuso"), False) Then

                        If Agro_SQL_SaveNum(xReg_Impianto.GetAttribute("destinazioneuso_old"), False) > 0 Then

                            objCodiciR.Cancella(CStr(xReg_Impianto.GetAttribute("piva")),
                                                CInt(xReg_Impianto.GetAttribute("sa_cod")),
                                                CInt(xReg_Impianto.GetAttribute("appezza")),
                                                codRegImpianto,
                                                CInt(xReg_Impianto.GetAttribute("destinazioneuso_old")),
                                                "",
                                                objParametri)

                        End If


                        If Agro_SQL_SaveNum(xReg_Impianto.GetAttribute("destinazioneuso"), False) > 0 Then

                            dummy = objCodiciR.Scrivi(CStr(xReg_Impianto.GetAttribute("piva")),
                                                      CInt(xReg_Impianto.GetAttribute("sa_cod")),
                                                      CInt(xReg_Impianto.GetAttribute("appezza")),
                                                      codRegImpianto,
                                                      CInt(xReg_Impianto.GetAttribute("destinazioneuso")),
                                                      "",
                                                      CDate(xReg_Impianto.GetAttribute("validita_inizio")),
                                                      CDate(xReg_Impianto.GetAttribute("validita_fine")),
                                                      objParametri)

                        End If

                    End If


                    '================================================================================================
                    'Altri CODICI
                    '------------------------------------------------------------------------------------------------

                    'Non va bene, perché vengono presi anche i nodi CodiceImpianto
                    'che si trovano sotto alla distinta!!!
                    'Devo prendere invece solo i figli di Reg_Impianto
                    'xCodici = xReg_Impianto.GetElementsByTagName("CodiceImpianto")

                    'prende il nodo DatiCodici figlio del nodo corrente (Reg_Impianto)
                    xDatiCodiciImpianto = xReg_Impianto.SelectSingleNode("child::DatiCodici")

                    If Not IsNothing(xDatiCodiciImpianto) AndAlso xDatiCodiciImpianto.HasChildNodes Then

                        'prende i nodi CodiceImpianto figli del nodo corrente (DatiCodici)
                        xCodici = xDatiCodiciImpianto.SelectNodes("child::CodiceImpianto")

                        i_Codice = 0

                        If Not IsNothing(xCodici) Then

                            Do While i_Codice < xCodici.Count

                                'Prelevo l' i-esimo Codice
                                xCodice = xCodici.Item(i_Codice)

                                'Nota Marco Attenzione: Al fine di evitare scritture errate di codici associati
                                'al progetto (vedi importatori vari) verifico che tale valore sia = 0
                                If Agro_SQL_SaveNum(xCodice.GetAttribute("progetto_cod"), False) = 0 Then

                                    'Prelevo gli attributi del codice selezionato (se G2G forzo inserimento)
                                    OpeDB_Codice = If(TipoG2G = 0, xCodice.GetAttribute("TipoOperazioneDB"), "1")

                                    'Verifico l'operazione richiesta
                                    Select Case OpeDB_Codice

                                        Case "0"    'LEGGI -------------------------------------------------------

                                        Case "1"    'SALVA -------------------------------------------------------

                                            'Se entro in modifica e inserisco un nuovo codice ->
                                            'l'Id_Reg si trova nella stringa Xml
                                            If OpeDB_Reg_Impianto = 2 Then
                                                codRegImpianto = CInt(xReg_Impianto.GetAttribute("id_reg"))
                                            End If

                                            dummy = objCodiciR.Scrivi(CStr(xReg_Impianto.GetAttribute("piva")),
                                                                      CInt(xReg_Impianto.GetAttribute("sa_cod")),
                                                                      CInt(xReg_Impianto.GetAttribute("appezza")),
                                                                      codRegImpianto,
                                                                      CInt(xCodice.GetAttribute("id_cod")),
                                                                      CStr(xCodice.GetAttribute("val_cod")),
                                                                      CDate(xCodice.GetAttribute("validita_inizio")),
                                                                      CDate(xCodice.GetAttribute("validita_fine")),
                                                                      objParametri,
                                                                      Data_creazione:=Agro_XML_GetDate(xCodice, "data_creazione", #2/1/1900#),
                                                                      Data_modifica:=Agro_XML_GetDate(xCodice, "data_modifica", #2/1/1900#),
                                                                      username_creazione:=Agro_XML_GetString(xCodice, "username_creazione", ""),
                                                                      username_modifica:=Agro_XML_GetString(xCodice, "username_modifica", "")
                                                                      )

                                        Case "2"    'MODIFICA -------------------------------------------------------

                                            objCodiciR.Modifica(CStr(xReg_Impianto.GetAttribute("piva")),
                                                                CInt(xReg_Impianto.GetAttribute("sa_cod")),
                                                                CInt(xReg_Impianto.GetAttribute("appezza")),
                                                                codRegImpianto,
                                                                CInt(xCodice.GetAttribute("id_cod")),
                                                                CStr(xCodice.GetAttribute("val_cod")),
                                                                CDate(xCodice.GetAttribute("validita_inizio")),
                                                                CDate(xCodice.GetAttribute("validita_fine")),
                                                                xFiltroAggiuntivo,
                                                                objParametri)


                                        Case "3"    'ELIMINA -------------------------------------------------------

                                            objCodiciR.Cancella(CStr(xReg_Impianto.GetAttribute("piva")),
                                                                CInt(xReg_Impianto.GetAttribute("sa_cod")),
                                                                CInt(xReg_Impianto.GetAttribute("appezza")),
                                                                codRegImpianto,
                                                                CInt(xCodice.GetAttribute("id_cod")),
                                                                "",
                                                                objParametri)

                                    End Select

                                End If

                                'Incremento l'indice
                                i_Codice += 1

                            Loop

                        End If 'codiceimpianto

                        '             i_DatiCodici += 1
                        '
                        '           Loop

                    End If 'dati codici

                    '-------------------------------------------------------------


                    xDatiImprese_Progetti = xReg_Impianto.GetElementsByTagName("DatiProgetto")

                    i_DatiImprese_Progetti = 0

                    'Dovrebbe esserci un solo nodo
                    Do While i_DatiImprese_Progetti < xDatiImprese_Progetti.Count

                        xImprese_Progetti = xDatiImprese_Progetti(i_DatiImprese_Progetti)

                        'Invio al componente di gestione appezzamenti

                        Dim sImprese_Progetti As String = xImprese_Progetti.OuterXml

                        'If Cod_Struttura <> 0 Then

                        'Rimpiazzo i codici 0 con i nuovi codici
                        sImprese_Progetti = Replace(sImprese_Progetti,
                                                    "sa_cod=" & Chr(34) & "0" & Chr(34),
                                                    "sa_cod=" & Chr(34) & CInt(xReg_Impianto.GetAttribute("sa_cod")) & Chr(34))
                        'End If

                        sImprese_Progetti = Replace(sImprese_Progetti,
                                                    "appezza=" & Chr(34) & "0" & Chr(34),
                                                    "appezza=" & Chr(34) & CStr(CInt(xReg_Impianto.GetAttribute("appezza"))) & Chr(34))

                        sImprese_Progetti = Replace(sImprese_Progetti,
                                                    "id_reg=" & Chr(34) & "0" & Chr(34),
                                                    "id_reg=" & Chr(34) & CStr(codRegImpianto) & Chr(34))


                        objProgettoW.Impresa_Progetto_Scrivi(sImprese_Progetti,
                                                             Nothing,
                                                             Nothing,
                                                             objParametri, NoteLog:=NoteLog)

                        xImprese_Progetti = Nothing

                        i_DatiImprese_Progetti += 1

                    Loop

                    xDatiImprese_Progetti = Nothing

                    '-------------------------------------------------------------

                    Select Case OpeDB_Reg_Impianto

                        Case "2"

                            'Aggiorno le date della grafica
                            graphicKey = ""
                            objSequenze.AppezzaIdReg_Comprimi(graphicKey, "I",
                                                              CInt(xReg_Impianto.GetAttribute("appezza")),
                                                              CInt(xReg_Impianto.GetAttribute("id_reg")),
                                                              CInt(xReg_Impianto.GetAttribute("basecode")),
                                                              objParametri)

                            objGraficaDalW.AggiornaValiditaImpianto(CStr(xReg_Impianto.GetAttribute("piva")),
                                                                    CInt(xReg_Impianto.GetAttribute("sa_cod")),
                                                                    graphicKey,
                                                                    "",
                                                                    CDate(xReg_Impianto.GetAttribute("validita_inizio")),
                                                                    CDate(xReg_Impianto.GetAttribute("validita_fine")),
                                                                    "", objParametri)

                            '-------------------------------------------------
                            '-------------INIZIO MODIFICHE PER CHIUSURA DISTINTE-----------
                            '-------------------------------------------------

                            objParametri.ImpostaFinestre_con_SalvataggioTemporale(AGRODATAINIZIO, AGRODATAFINE)

                            '-------------------------------------------------
                            'Modifica delle finestre temporali progetti
                            'chiudo la fine del progetto 
                            Dim objImpreseProgR As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
                            Dim dt As DataTable = objImpreseProgR.LeggiDistinta(CStr(xReg_Impianto.GetAttribute("piva")),
                                                                                CInt(xReg_Impianto.GetAttribute("sa_cod")),
                                                                                CInt(xReg_Impianto.GetAttribute("appezza")),
                                                                                CInt(xReg_Impianto.GetAttribute("id_reg")),
                                                                                "", enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                                "", " Imprese_Progetti.Validita_Inizio desc  ", objParametri)

                            If dt.Rows.Count > 0 Then
                                If Date.Compare(CDate(dt.Rows(0).Item("Validita_Inizio")), CDate(xReg_Impianto.GetAttribute("validita_fine"))) > 0 Then

                                    'c'è una distinta aperta in una data successiva a quella di chiusura impianto, genero errore
                                    Throw New Exception("<br><br> ATTENZIONE! <br> C'è una distinta aperta in una data successiva a quella di chiusura impianto, verificare le distinte!")
                                Else
                                    'modifico la data distinta, se non ci sono operazioni successive registrate
                                    ''''''''''''''''''

                                    If Not Date_Modifiche_Progetti_Verificate Then
                                        'devo controllare anche che sia solo una distinta successiva..
                                        Dim dt2 As DataTable = objImpreseProgR.LeggiDistinta(CStr(xReg_Impianto.GetAttribute("piva")),
                                                                                             CInt(xReg_Impianto.GetAttribute("sa_cod")),
                                                                                             CInt(xReg_Impianto.GetAttribute("appezza")),
                                                                                             CInt(xReg_Impianto.GetAttribute("id_reg")),
                                                                                             "", enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                                             " Imprese_Progetti.Validita_Fine > " & Agro_SQL_SaveDate(CDate(xReg_Impianto.GetAttribute("validita_fine")), False), "", objParametri)
                                        If dt2.Rows.Count > 1 Then
                                            Throw New Exception("<br><br> ATTENZIONE! <br> Ci sono " & dt2.Rows.Count & " distinte aperta in una data successiva a quella di chiusura impianto, verificare le distinte!")

                                        End If
                                    End If

                                    If Not Date_Modifiche_Progetti_Verificate Then
                                        'devo controllare anche che non ci siano distinte precedenti
                                        Dim dt2 As DataTable = objImpreseProgR.LeggiDistinta(CStr(xReg_Impianto.GetAttribute("piva")),
                                                                                             CInt(xReg_Impianto.GetAttribute("sa_cod")),
                                                                                             CInt(xReg_Impianto.GetAttribute("appezza")),
                                                                                             CInt(xReg_Impianto.GetAttribute("id_reg")),
                                                                                             "", enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                                             " Imprese_Progetti.Validita_Inizio < " & Agro_SQL_SaveDate(CDate(xReg_Impianto.GetAttribute("validita_inizio")), False), "", objParametri)
                                        If dt2.Rows.Count > 0 Then
                                            Throw New Exception("<br><br> ATTENZIONE! <br> Ci sono " & dt2.Rows.Count & " distinte aperta in una data precedente a quella di apertura impianto, verificare le distinte!")

                                        End If
                                    End If

                                    '--------------------------------------------------------------------------------------------------------------------------------------------
                                    'Controllo, se sono già state registrate Operazioni d'Agenda
                                    'che le date delle operazioni nn siano esterne alle date scelte x l'impianto 
                                    '--------------------------------------------------------------------------------------------------------------------------------------------

                                    Dim DTA As DataTable

                                    'ricavo il recordset dei movimenti di produzione associati all'impianto
                                    DTA = objDestR.LeggiCronologiaMovimenti(CStr(xReg_Impianto.GetAttribute("piva")),
                                                                            CInt(xReg_Impianto.GetAttribute("sa_cod")),
                                                                            CInt(xReg_Impianto.GetAttribute("appezza")),
                                                                            CInt(xReg_Impianto.GetAttribute("id_reg")),
                                                                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                            " Data_Movimento > " & Agro_SQL_SaveDate(CDate(xReg_Impianto.GetAttribute("validita_fine")), False) & " ",
                                                                            "  Data_Movimento desc ",
                                                                            objParametri)


                                    If DTA.Rows.Count > 0 Then
                                        'ci sono operazioni fatte successivamente alla chiusura
                                        Throw New Exception("<br><br> ATTENZIONE! <br> Ci sono una o più operazioni di agenda create successivamente alla data di chiusura impianto, verificare le operazioni sull'impianto!")
                                    End If


                                    ''''''''''''''''''

                                    Dim objImpreseProg As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_W

                                    objImpreseProg.AggiornaValiditaFine(CStr(xReg_Impianto.GetAttribute("piva")),
                                                                        CInt(xReg_Impianto.GetAttribute("sa_cod")),
                                                                        CInt(xReg_Impianto.GetAttribute("appezza")),
                                                                        dt.Rows(0).Item("id_reg"),
                                                                        dt.Rows(0).Item("progetto_cod"),
                                                                        0,
                                                                        CDate(xReg_Impianto.GetAttribute("validita_fine")),
                                                                        "", objParametri)

                                End If
                            End If

                            objParametri.ResettaFinestra()

                            '-------------------------------------------------
                            '-------------FINE MODIFICHE---------------
                            '-------------------------------------------------


                        Case "3" 'CANCELLAZIONE reg_impianto

                            'Cancellazione di TUTTE le OPERAZIONI DI AGENDA associate all'impianto

                            dtAgenda = objDestR.LeggiCronologiaMovimenti(CStr(xReg_Impianto.GetAttribute("piva")),
                                                                         CInt(xReg_Impianto.GetAttribute("sa_cod")),
                                                                         CInt(xReg_Impianto.GetAttribute("appezza")),
                                                                         CInt(xReg_Impianto.GetAttribute("id_reg")),
                                                                         enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                         "",
                                                                         "",
                                                                         objParametri)

                            dtRicette = ObjRicette.Leggi(0, 0, 0, 0, 0,
                                                         CStr(xReg_Impianto.GetAttribute("piva")),
                                                         CInt(xReg_Impianto.GetAttribute("sa_cod")),
                                                         CInt(xReg_Impianto.GetAttribute("appezza")),
                                                         xReg_Impianto.GetAttribute("id_reg"),
                                                         AGRODATAINIZIO,
                                                         AGRODATAFINE,
                                                         enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                         "",
                                                         "",
                                                         objParametri)

                            '17/01/2019 DRUDI 
                            ' non elimino più le operazioni di agenda ma lancio una eccezione
                            If dtAgenda.Rows.Count > 0 OrElse dtRicette.Rows.Count > 0 Then

                                Throw New Exception(" Non è possibile eliminare l'impianto Piva:" & CStr(xReg_Impianto.GetAttribute("piva")) &
                                                    " Sa_Cod: " & CInt(xReg_Impianto.GetAttribute("sa_cod")) &
                                                    " Appezza: " & CInt(xReg_Impianto.GetAttribute("appezza")) &
                                                    " id_reg: " & CInt(xReg_Impianto.GetAttribute("id_reg")) &
                                                    " perché ci sono registrazioni associate ")

                            End If

                            'controllo se l'Appezzamento è collegato ad un Entità  GIS
                            'Dim objGIS As New AgronicaCoreGisBIZ.GIS_Entita_R
                            'If (objGIS.esisteGisEntita_cancellazioneElementoAnagrafico(CStr(xReg_Impianto.GetAttribute("piva")), CInt(xReg_Impianto.GetAttribute("sa_cod")), CInt(xReg_Impianto.GetAttribute("appezza")), 0, CInt(xReg_Impianto.GetAttribute("id_reg")), enum_GIS2012_TipoEntita.IMPIANTI_ORTICOLA, objParametri)) Then
                            '    Throw New Exception(Gias.ImpossibileCancellareImpiantoCollegatoPoligono)
                            'End If
                            'Dim i As Integer
                            'If Not IsNothing(dtAgenda) AndAlso dtAgenda.Rows.Count <> 0 Then

                            '    Dim objAgendaLeggi As New AgronicaCoreContabBIZ.Agenda_R
                            '    Dim objAgendaScrivi As New AgronicaCoreContabBIZ.Agenda_W

                            '    For i = 0 To dtAgenda.Rows.Count - 1

                            '        datiAgenda = objAgendaLeggi.Agenda_Leggi(CStr(xReg_Impianto.GetAttribute("piva")),
                            '                                                 CInt(xReg_Impianto.GetAttribute("sa_cod")),
                            '                                                 CInt(dtAgenda.Rows(i).Item("Id_Agenda")),
                            '                                                 0, True,
                            '                                                 objParametri)

                            '        objAgendaScrivi.Agenda_Scrivi(datiAgenda,
                            '                                      Nothing,
                            '                                      0,
                            '                                      CInt(idServizio),
                            '                                      0,
                            '                                      "",
                            '                                      objParametri)

                            '    Next


                            '    objAgendaLeggi = Nothing
                            '    objAgendaScrivi = Nothing

                            'End If

                            dtAgenda.Dispose()
                            dtAgenda = Nothing

                            Dim objPUALog As New AgronicaCorePUA_DAL.AgronicaLogPua_W

                            'DRUDI 2019-10-22 Cancellazione PUA_LetamazioniPrecedenti e Anagrafe_VincoliAgronomici
                            Dim objAnagrafe_VincoliAgronomici_R As New AgronicaCoreAnagrafeDAL.Anagrafe_VincoliAgronomici_R
                            Dim objAnagrafe_VincoliAgronomici_W As New AgronicaCoreAnagrafeDAL.Anagrafe_VincoliAgronomici_W
                            Dim dtAnagrafe_Vincoli = objAnagrafe_VincoliAgronomici_R.Leggi(0,
                                                                                          CStr(xReg_Impianto.GetAttribute("piva")),
                                                                                          CInt(xReg_Impianto.GetAttribute("sa_cod")),
                                                                                          CInt(xReg_Impianto.GetAttribute("appezza")),
                                                                                          CInt(xReg_Impianto.GetAttribute("id_reg")),
                                                                                          0, 0, 0, 0, "", "", objParametri)

                            For Each vincolo In dtAnagrafe_Vincoli.Rows
                                If vincolo("Progetto_Cod") = 0 Then
                                    objAnagrafe_VincoliAgronomici_W.CancellaById(vincolo("ID"), "", objParametri)
                                    objPUALog.Scrivi(enum_TipoOperazioneDB.Cancellazione, "Anagrafe_VincoliAgronomici", vincolo("pua_Cod"), vincolo("regolamento_cod"), vincolo("id"), vincolo("piva"), vincolo("sa_cod"), vincolo("appezza"), vincolo("id_reg"), vincolo("progetto_cod"), Nothing, Nothing, "", objParametri)
                                End If
                            Next


                            Dim objPUA_LetamazioniPrecedenti_R As New AgronicaCorePUA_DAL.PUA_LetamazioniPrecedenti_R
                            Dim objPUA_LetamazioniPrecedenti_W As New AgronicaCorePUA_DAL.PUA_LetamazioniPrecedenti_W


                            Dim dtPUA_Letamazioni = objPUA_LetamazioniPrecedenti_R.Leggi(0,
                                                                                         0,
                                                                                         CStr(xReg_Impianto.GetAttribute("piva")),
                                                                                         CInt(xReg_Impianto.GetAttribute("sa_cod")),
                                                                                         CInt(xReg_Impianto.GetAttribute("appezza")),
                                                                                         CInt(xReg_Impianto.GetAttribute("id_reg")),
                                                                                         0, "", "", objParametri)

                            For Each pualet In dtPUA_Letamazioni.Rows
                                If pualet("Progetto_Cod") = 0 Then
                                    objPUA_LetamazioniPrecedenti_W.CancellaByID(pualet("ID"), "", objParametri)
                                    objPUALog.Scrivi(enum_TipoOperazioneDB.Cancellazione, "PUA_LetamazioniPrecedenti", pualet("pua_Cod"), pualet("regolamento_cod"), pualet("id"), pualet("piva"), pualet("sa_cod"), pualet("appezza"), pualet("id_reg"), pualet("progetto_cod"), pualet("eff_cod"), pualet("id_fre"), "", objParametri)
                                End If
                            Next


                            '--------------------------------------------------------------------------------------------------
                            'Cancellazione Poligoni
                            Dim objGISEntita_R As New AgronicaCoreGisDAL.GIS_Entita_R
                            Dim objGISEntita_W As New AgronicaCoreGisDAL.GIS_Entita_W
                            Dim objGISElementiGrafici_R As New AgronicaCoreGisDAL.GIS_ElementiGrafici_R
                            Dim objGISElementiGrafici_W As New AgronicaCoreGisDAL.GIS_ElementiGrafici_W

                            Dim DT_GISEntita As DataTable
                            Dim DT_GISElementiGrafici As DataTable

                            DT_GISEntita = objGISEntita_R.Leggi("", 0,
                                                                enum_GIS2012_TipoEntita.IMPIANTI_ORTICOLA,
                                                                CStr(xReg_Impianto.GetAttribute("piva")),
                                                                CInt(xReg_Impianto.GetAttribute("sa_cod")),
                                                                CInt(xReg_Impianto.GetAttribute("appezza")),
                                                                0,
                                                                CInt(xReg_Impianto.GetAttribute("id_reg")),
                                                                AGRODATAINIZIO, AGRODATAFINE,
                                                                "", "", objParametri)

                            For Each entita In DT_GISEntita.Rows
                                DT_GISElementiGrafici = objGISElementiGrafici_R.Leggi(entita("PivaSuperUser"), 0, entita("Entita_Cod"),
                                                                                      0, 0,
                                                                                      enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                                      "", "", objParametri)

                                For Each elemento In DT_GISElementiGrafici.Rows
                                    objGISElementiGrafici_W.Cancella(entita("PivaSuperUser"), elemento("ElementoGrafico_Cod"), "", objParametri)
                                Next

                                objGISEntita_W.Cancella(entita("PivaSuperUser"), entita("Entita_Cod"), "", objParametri)
                            Next


                            '--------------------------------------------------------------------------------------------------
                            ' Cancellazione dei dati grafici
                            baseCode = (CInt(xReg_Impianto.GetAttribute("sa_cod")) \ (2 ^ 17)) * (2 ^ 17)

                            objSequenze.AppezzaIdReg_Comprimi(graphicKey, "I", CInt(xReg_Impianto.GetAttribute("appezza")),
                                                              xReg_Impianto.GetAttribute("id_reg"), CInt(baseCode),
                                                              objParametri)

                            objGraficaDalW.Cancella(CStr(xReg_Impianto.GetAttribute("piva")), CInt(xReg_Impianto.GetAttribute("sa_cod")),
                                                    "", graphicKey, "",
                                                    "", objParametri)

                            objSequenze.AppezzaIdReg_Comprimi(graphicKey, "F", CInt(xReg_Impianto.GetAttribute("appezza")),
                                                              xReg_Impianto.GetAttribute("id_reg"), CInt(baseCode),
                                                              objParametri)

                            objGraficaDalW.Cancella(CStr(xReg_Impianto.GetAttribute("piva")), CInt(xReg_Impianto.GetAttribute("sa_cod")),
                                                    "", graphicKey, "",
                                                    "", objParametri)

                            '------------------------------------------------------
                            'Cancellazione della Distinta di Produzione Agricola Aziendale
                            Dim objDistintaR As New AgronicaCoreAnagrafeBIZ.Progetto_R
                            Dim datiDelete As String
                            Dim dummyProgetto As Boolean

                            'Lettura stringa Xml soggetta a cancellazione
                            datiDelete = objDistintaR.Impresa_Progetti_Leggi(CStr(xReg_Impianto.GetAttribute("piva")),
                                                                             0,
                                                                             "",
                                                                             0,
                                                                             CInt(xReg_Impianto.GetAttribute("sa_cod")),
                                                                             CInt(xReg_Impianto.GetAttribute("appezza")),
                                                                             CInt(xReg_Impianto.GetAttribute("id_reg")),
                                                                             0, 0,
                                                                             True,
                                                                             objParametri)

                            If datiDelete <> "" Then
                                'Cancellazione Distinta
                                dummyProgetto = objProgettoW.Impresa_Progetto_Scrivi(CStr(datiDelete),
                                                                                     Nothing,
                                                                                     Nothing,
                                                                                     objParametri,
                                                                                     NoteLog:=NoteLog)
                            End If
                            '============================================================================================================================

                            objRegImpiantiW.Cancella(CStr(xReg_Impianto.GetAttribute("piva")),
                                                     CInt(xReg_Impianto.GetAttribute("sa_cod")),
                                                     CInt(xReg_Impianto.GetAttribute("appezza")),
                                                     CInt(xReg_Impianto.GetAttribute("id_reg")),
                                                     "",
                                                     objParametri)


                            '************************************************
                            '*********** INIZIO LOGGING *******************
                            '************************************************

                            'MEMORIZZO L'INSERIMENTO DELL'OPERAZIONE NELLA TABELLA DI LOG (AGRONICA LOG ANAGRAFE)
                            'è necessario avere Migra >= 505

                            resLog = objAgronicaLogAnagrafeW.Scrivi(CInt(OpeDB_Reg_Impianto),
                                                                    enum_TipoEntita_Des.Impianti,
                                                                    CStr(xReg_Impianto.GetAttribute("piva")),
                                                                    CStr(xReg_Impianto.GetAttribute("sa_cod")),
                                                                    CStr(xReg_Impianto.GetAttribute("appezza")),
                                                                    CStr(xReg_Impianto.GetAttribute("id_reg")),
                                                                    Nothing,
                                                                    Nothing,
                                                                    NoteLog, CInt(idServizio), objParametri)

                            '************************************************
                            '*********** FINE LOGGING *********************
                            '************************************************


                            dtPianoConcimazione = objPianoConcimazione_R.Leggi(0,
                                                                               0,
                                                                               CStr(xReg_Impianto.GetAttribute("piva")),
                                                                               CStr(xReg_Impianto.GetAttribute("sa_cod")),
                                                                               0,
                                                                               CStr(xReg_Impianto.GetAttribute("appezza")),
                                                                               CStr(xReg_Impianto.GetAttribute("id_reg")),
                                                                               0,
                                                                               "",
                                                                               "",
                                                                               "",
                                                                               0,
                                                                               0,
                                                                               "",
                                                                               0,
                                                                               "",
                                                                               enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                               "",
                                                                               "",
                                                                               objParametri)


                            For Each rowPianoConcimazione In dtPianoConcimazione.Rows

                                objPianoConcimazione_W.Cancella(rowPianoConcimazione("PC_Testata_Cod"),
                                                                rowPianoConcimazione("PC_Entita_Cod"),
                                                                rowPianoConcimazione("Piva"),
                                                                rowPianoConcimazione("Sa_Cod"),
                                                                rowPianoConcimazione("Appezza"),
                                                                rowPianoConcimazione("Id_Imp"),
                                                                rowPianoConcimazione("Fabbricato_Cod"),
                                                                rowPianoConcimazione("Campo_Cod"),
                                                                rowPianoConcimazione("Prov"),
                                                                rowPianoConcimazione("Com"),
                                                                rowPianoConcimazione("Sezione"),
                                                                rowPianoConcimazione("Foglio"),
                                                                rowPianoConcimazione("Numero"),
                                                                rowPianoConcimazione("Subalterno"),
                                                                0,
                                                                "",
                                                                "",
                                                                objParametri)
                            Next


                    End Select

                    '-------------------------------------------------------------

                    i_Reg_Impianto += 1 'Incremento l'indice

                Loop

                '------------------------------

                i_DatiReg_Impianto += 1 'Incremento l'indice

            Loop

            '------------------------------

            'Elimino tutti gli oggetti utilizzati

            xReg_Impianto = Nothing
            xDatiReg_Impianto = Nothing
            xmlDoc = Nothing

            objSequenze = Nothing
            objRegImpiantiW = Nothing
            objAgronicaLogAnagrafeW = Nothing
            objCodiciR = Nothing
            objProgettoW = Nothing
            objDestR = Nothing
            objGraficaBizW = Nothing
            objGraficaDalW = Nothing

            'Restituisco un valore Dummy
            xRisp = True

            'Faccio il commit
            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(flagTransazioneLocale, objParametri)


        Catch ex As Exception

            'Restituisco un valore Dummy
            xRisp = False

            'Faccio il rollback della transazione
            If objParametri.objTransazione IsNot Nothing Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            '//////////////////////////////////////////////////////////////////////
            messaggioErrore = "(Piva=" & OUTPUT_Piva & ")" &
                              "(Sa_Cod=" & CStr(OUTPUT_Sa_Cod) & ")" &
                              "(Appezza=" & CStr(OUTPUT_Appezza) & ")" &
                              "(Id_Reg=" & CStr(OUTPUT_Id_Reg) & ")" &
                              " : " & ex.Message
            '//////////////////////////////////////////////////////////////////////

            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False

            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        Finally

            'Chiudo la connessione
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(flagConnessioneLocale, objParametri)

        End Try

        Return xRisp

    End Function

#Region "Entity Framework"

    Public Function Impianto_ScriviModifica(ByVal dati_impianto As String,
                                            ByRef OUTPUT_Piva As String,
                                            ByRef OUTPUT_Sa_Cod As Integer,
                                            ByRef OUTPUT_Appezza As Integer,
                                            ByRef OUTPUT_Id_Reg As Integer,
                                            ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                            Optional ByVal default_regolamento As Integer = 1,
                                            Optional NoteLog As String = "") As RispostaStandard

        Dim nomeRoutine As String = "AnagrafeBIZ.Reg_Impianto_W.Impianto_ScriviModifica"
        Dim messaggioErrore As String = ""

        Dim r As New RispostaStandard

        Dim objImp_Dal As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Write
        Dim objImp_cod_Dal As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_W
        Dim objPrj_part As New AgronicaCoreAnagrafeDAL.ProgettixParticelle_W
        Dim objPrj_Dal As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_W
        Dim gefutils As New Gias_EF_Utility

        Dim objImpreseCodici As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
        Dim algoritmo_codifica = objImpreseCodici.Leggi_Codice_from_Imprese_Codici(OUTPUT_Piva, enum_CodiciAnagrafe.Algoritmo_Codifica, objParametriServer)

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametriServer.StringaConnessione)

        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

        Using scope As New TransactionScope()

            Try
                Dim obj_Impianto = Newtonsoft.Json.JsonConvert.DeserializeObject(dati_impianto)
                Dim scriviImp As Boolean = False

                Dim piva As String = obj_Impianto.GetValue("piva").ToString()
                Dim sa_cod As Integer = CInt(obj_Impianto.GetValue("sa_cod"))
                Dim appezza As Integer = CInt(obj_Impianto.GetValue("appezza"))

                Dim id_reg As Integer = CInt(obj_Impianto.GetValue("id_reg"))
                If id_reg = 0 Then
                    scriviImp = True
                    Dim agroDP As New AgronicaCoreDataProvider.Agro_Sequenze
                    id_reg = agroDP.NuovoId_Reg_Impianti(piva, sa_cod, appezza, 0, 200000000, objParametriServer)
                End If

                Dim campo_cod As Integer = CInt(obj_Impianto.GetValue("campo_cod"))

                Dim impiantoElem = From impianto In GiasContext.Reg_Impianti
                                   Where impianto.PIVA = piva AndAlso
                                         impianto.SA_COD = sa_cod AndAlso
                                         impianto.APPEZZA = appezza AndAlso
                                         impianto.ID_REG = id_reg
                                   Select impianto

                'Salvo l'impianto
                If scriviImp Then
                    Impianto_Scrivi(dati_impianto, piva, sa_cod, appezza, id_reg, campo_cod, objParametriServer, GiasContext, NoteLog:=NoteLog)
                Else
                    If impiantoElem.Count > 0 Then
                        'Se è stata modificata la superficie marco tutte le operazioni dei agenda
                        If impiantoElem.FirstOrDefault.Sup_Imp <> CDbl(obj_Impianto.GetValue("sup_imp")) Then

                            Dim id_agendas = (From a In GiasContext.Mov_Destinazioni
                                              Where a.Piva = piva AndAlso
                                                    a.Sa_Cod = sa_cod AndAlso
                                                    a.Appezza = appezza AndAlso
                                                    a.Id_Destinazione = id_reg AndAlso
                                                    a.Tipo_Destinazione = 0
                                              Select a.Id_Agenda).Distinct.ToList

                            For Each id_agenda In id_agendas
                                Dim agenda = (From a In GiasContext.Agenda Where a.Id_Agenda = id_agenda).FirstOrDefault

                                If agenda IsNot Nothing Then
                                    agenda.Blocco_Flag = 2
                                    agenda.Blocco_Username = objParametriServer.UtenteUsername
                                    agenda.Blocco_Data = DateTime.Now
                                    GiasContext.Entry(agenda).State = EntityState.Modified
                                    GiasContext.SaveChanges()
                                End If

                            Next

                        End If
                        Impianto_Modifica(dati_impianto, piva, sa_cod, appezza, id_reg, campo_cod, objParametriServer, GiasContext, NoteLog:=NoteLog)
                    Else
                        Impianto_Scrivi(dati_impianto, piva, sa_cod, appezza, id_reg, campo_cod, objParametriServer, GiasContext, NoteLog:=NoteLog)
                    End If
                End If

                Dim grva_cod = 0
                If IsNumeric(obj_Impianto.GetValue("grva_cod")) Then
                    grva_cod = obj_Impianto.GetValue("grva_cod")
                End If

                Dim interbina = obj_Impianto.GetValue("interbina")
                Dim germinabilita = obj_Impianto.GetValue("germinabilita")
                Dim codiceZona = obj_Impianto.GetValue("codiceZona")
                Dim dettaglio_varieta_personalizzato = obj_Impianto.GetValue("dettaglio_varieta_personalizzato")
                Dim impianto_ibrido = obj_Impianto.GetValue("impianto_ibrido")

                Dim unita_vitata = obj_Impianto.GetValue("unita_vitata")
                Dim CodBMBDBT_M = obj_Impianto.GetValue("CodBMBDBT_M")
                Dim CodBMBDBT_F = obj_Impianto.GetValue("CodBMBDBT_F")
                Dim Genetica_M = obj_Impianto.GetValue("Genetica_M")
                Dim Genetica_F = obj_Impianto.GetValue("Genetica_F")
                Dim OffType_M = obj_Impianto.GetValue("OffType_M")
                Dim OffType_F = obj_Impianto.GetValue("OffType_F")
                Dim DistanzaSuFila_F = obj_Impianto.GetValue("DistanzaSuFila_F")
                Dim DistanzaTraFila_F = obj_Impianto.GetValue("DistanzaTraFila_F")
                Dim tra_fila_m = obj_Impianto.GetValue("tra_fila_m")
                Dim su_fila_m = obj_Impianto.GetValue("su_fila_m")
                Dim tagliato_tuberi = obj_Impianto.GetValue("tagliato_tuberi")
                Dim partiTuberi = obj_Impianto.GetValue("partiTuberi")
                Dim chkConsociazione_checked = obj_Impianto.GetValue("chkConsociazione_checked")
                Dim Id_Consociazione = obj_Impianto.GetValue("id_consociazione")
                Dim codice_impianto = obj_Impianto.GetValue("codice_impianto")

                Dim val_interbina As Integer = 0
                If interbina.ToString <> "" Then
                    val_interbina = 1
                End If

                'Salvo i codici dell'impianto
                If val_interbina <> 0 Then
                    objImp_cod_Dal.ScriviModificaEliminaxImpianto(piva, sa_cod, appezza, id_reg,
                                                                  enum_CodiciAnagrafe.Impianto_Ibrido, val_interbina, True,
                                                                  objParametriServer, GiasContext)
                End If

                objImp_cod_Dal.ScriviModificaEliminaxImpianto(piva, sa_cod, appezza, id_reg,
                                                              enum_CodiciAnagrafe.Impianto_CodiceB_Maschio, CodBMBDBT_M.ToString, True,
                                                              objParametriServer, GiasContext)

                objImp_cod_Dal.ScriviModificaEliminaxImpianto(piva, sa_cod, appezza, id_reg,
                                                              enum_CodiciAnagrafe.Impianto_CodiceB_Femmina, CodBMBDBT_F.ToString, True,
                                                              objParametriServer, GiasContext)

                objImp_cod_Dal.ScriviModificaEliminaxImpianto(piva, sa_cod, appezza, id_reg,
                                                              enum_CodiciAnagrafe.Impianto_Genetica_Maschio, Genetica_M.ToString, True,
                                                              objParametriServer, GiasContext)

                objImp_cod_Dal.ScriviModificaEliminaxImpianto(piva, sa_cod, appezza, id_reg,
                                                              enum_CodiciAnagrafe.Impianto_Genetica_Femmina, Genetica_F.ToString, True,
                                                              objParametriServer, GiasContext)

                objImp_cod_Dal.ScriviModificaEliminaxImpianto(piva, sa_cod, appezza, id_reg,
                                                              enum_CodiciAnagrafe.Impianto_OffType_Maschio, OffType_M.ToString, True,
                                                              objParametriServer, GiasContext)

                objImp_cod_Dal.ScriviModificaEliminaxImpianto(piva, sa_cod, appezza, id_reg,
                                                              enum_CodiciAnagrafe.Impianto_OffType_Femmina, OffType_F.ToString, True,
                                                              objParametriServer, GiasContext)

                objImp_cod_Dal.ScriviModificaEliminaxImpianto(piva, sa_cod, appezza, id_reg,
                                                              enum_CodiciAnagrafe.Impianto_TraFila_Maschio, tra_fila_m.ToString, True,
                                                              objParametriServer, GiasContext)

                objImp_cod_Dal.ScriviModificaEliminaxImpianto(piva, sa_cod, appezza, id_reg,
                                                              enum_CodiciAnagrafe.Impianto_TraFila_Femmina, DistanzaTraFila_F.ToString, True,
                                                              objParametriServer, GiasContext)

                objImp_cod_Dal.ScriviModificaEliminaxImpianto(piva, sa_cod, appezza, id_reg,
                                                              enum_CodiciAnagrafe.Impianto_SuFila_Maschio, su_fila_m.ToString, True,
                                                              objParametriServer, GiasContext)

                objImp_cod_Dal.ScriviModificaEliminaxImpianto(piva, sa_cod, appezza, id_reg,
                                                              enum_CodiciAnagrafe.Impianto_SuFila_Femmina, DistanzaSuFila_F.ToString, True,
                                                              objParametriServer, GiasContext)

                objImp_cod_Dal.ScriviModificaEliminaxImpianto(piva, sa_cod, appezza, id_reg,
                                                              enum_CodiciAnagrafe.Impianto_Interbina, interbina.ToString, True,
                                                              objParametriServer, GiasContext)

                objImp_cod_Dal.ScriviModificaEliminaxImpianto(piva, sa_cod, appezza, id_reg,
                                                              enum_CodiciAnagrafe.Impianto_Germinabilita, germinabilita.ToString, True,
                                                              objParametriServer, GiasContext)
                objImp_cod_Dal.ScriviModificaEliminaxImpianto(piva, sa_cod, appezza, id_reg,
                                                              enum_CodiciAnagrafe.CodiceZona, codiceZona.ToString, True,
                                                              objParametriServer, GiasContext)

                objImp_cod_Dal.ScriviModificaEliminaxImpianto(piva, sa_cod, appezza, id_reg,
                                                              enum_CodiciAnagrafe.Impianto_Taglio_Tuberi_Patate, tagliato_tuberi.ToString, True,
                                                              objParametriServer, GiasContext)

                objImp_cod_Dal.ScriviModificaEliminaxImpianto(piva, sa_cod, appezza, id_reg,
                                                              enum_CodiciAnagrafe.Impianto_Parti_Tuberi_Patate, partiTuberi.ToString, True,
                                                              objParametriServer, GiasContext)

                objImp_cod_Dal.ScriviModificaEliminaxImpianto(piva, sa_cod, appezza, id_reg,
                                                              enum_CodiciAnagrafe.Dettaglio_Specie_Personalizzato, dettaglio_varieta_personalizzato.ToString, True,
                                                              objParametriServer, GiasContext)

                objImp_cod_Dal.ScriviModificaEliminaxImpianto(piva, sa_cod, appezza, id_reg,
                                                              enum_CodiciAnagrafe.Codice_Impianto, codice_impianto.ToString, True,
                                                              objParametriServer, GiasContext)

                'Gestione id_Cod_Terreno, cancello e risalvo sempre
                Dim lid_cod = From ric In GiasContext.Reg_Impianti_Codici
                              Where ric.PIVA = piva AndAlso
                                    ric.sa_cod = sa_cod AndAlso
                                    ric.appezza = appezza AndAlso
                                    ric.Id_Reg = id_reg AndAlso
                                    ric.Progetto_Cod = 0 AndAlso
                                    ric.id_cod >= 3000 AndAlso
                                    ric.id_cod < 4000

                For Each cod_terreno In lid_cod
                    GiasContext.Reg_Impianti_Codici.Remove(cod_terreno)
                    GiasContext.SaveChanges()
                Next

                Dim id_cod_terreno As Integer = 0
                If IsNumeric(obj_Impianto.GetValue("id_cod_terreno")) Then
                    id_cod_terreno = CInt(obj_Impianto.GetValue("id_cod_terreno"))
                    objImp_cod_Dal.ScriviModificaEliminaxImpianto(piva, sa_cod, appezza, id_reg, id_cod_terreno, " ", True,
                                                                  objParametriServer, GiasContext)
                End If

                Dim listaProgetto_Cod As New List(Of Integer)
                Dim dati_distintekendo = obj_Impianto.GetValue("dati_Distinte")

                'Gestione salvataggio distinte
                Dim objDist As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_W

                If dati_distintekendo IsNot Nothing Then

                    Dim aa = JObject.Parse(dati_distintekendo.ToString)
                    Dim kr = aa.GetValue("kendo_rows")
                    Dim dati_distinte = JArray.Parse(kr.ToString)

                    'creo esercizio default se non indicato
                    If dati_distinte.Count = 0 Then

                        Dim appezza_des = (From a In GiasContext.Appezzamento Where a.PIVA = piva AndAlso a.SA_COD = sa_cod AndAlso a.APPEZZA = appezza Select a.APP_NOME).FirstOrDefault
                        Dim progetto_des As String = "Esercizio: " & appezza_des
                        Dim cul_cod = CInt(obj_Impianto.GetValue("cul_cod"))
                        Dim veg_cod = CInt(obj_Impianto.GetValue("veg_cod"))
                        If cul_cod = 0 Then
                            If id_cod_terreno = 0 Then
                                progetto_des &= " - Terreno Nudo"
                            Else
                                Dim terreno = (From a In GiasContext.Codici_Anagrafe Where a.codice = id_cod_terreno Select a.descrizione).FirstOrDefault
                                progetto_des &= If(terreno Is Nothing, "", " - " & terreno)
                            End If
                        Else
                            Dim specie = (From a In GiasContext.Cultivar Join b In GiasContext.SpecieVegetali On a.Veg_Cod Equals b.Veg_Cod Where a.Cul_Cod = cul_cod Select b.Veg_Des & " - " & a.Cul_Des).FirstOrDefault
                            progetto_des &= If(specie Is Nothing, "", " - " & specie)
                        End If

                        Dim obj_Distinta As New JObject
                        obj_Distinta.Add(New JProperty("Progetto_Cod", 0))
                        obj_Distinta.Add(New JProperty("Progetto_Nome", ""))
                        obj_Distinta.Add(New JProperty("Progetto_Des", progetto_des))
                        obj_Distinta.Add(New JProperty("Regolamento_Cod", default_regolamento))
                        obj_Distinta.Add(New JProperty("Validita_Inizio", obj_Impianto.GetValue("impianto_data_inizio")))
                        obj_Distinta.Add(New JProperty("Validita_Fine", obj_Impianto.GetValue("impianto_data_fine")))
                        obj_Distinta.Add(New JProperty("Data_Inizio_Portinnesto", obj_Impianto.GetValue("Data_Inizio_Portinnesto")))
                        obj_Distinta.Add(New JProperty("Data_Inizio_Innesto", obj_Impianto.GetValue("Data_Inizio_Innesto")))
                        obj_Distinta.Add(New JProperty("Data_Inizio_Produzione", obj_Impianto.GetValue("Data_Inizio_Produzione")))
                        obj_Distinta.Add(New JProperty("Piante_Maschi_InSesto", obj_Impianto.GetValue("Piante_Maschi_InSesto")))
                        dati_distinte.Add(obj_Distinta)

                    End If

                    For Each distinta1 In dati_distinte
                        Dim distinta = JObject.Parse(distinta1.ToString)
                        Dim progetto_cod = CInt(distinta.GetValue("Progetto_Cod"))

                        'CONTROLLI SUI CdG SOLO SU DISTINTE MODIFICATE
                        If progetto_cod <> 0 Then
                            Dim validita_inizio = CStr(distinta.GetValue("Validita_Inizio"))
                            Dim validita_fine = CStr(distinta.GetValue("Validita_Fine"))
                            Dim progetto_nome = CStr(distinta.GetValue("Progetto_Nome"))
                            Dim descrizione_distinta As String = ""

                            If progetto_nome <> "" Then
                                descrizione_distinta = progetto_nome
                            Else
                                descrizione_distinta = "con validità dal " & validita_inizio & " al " & validita_fine
                            End If

                            Dim objEsercizioR As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
                            Dim ese = objEsercizioR.LeggiDistinta3(piva, sa_cod, appezza, id_reg, " Imprese_Progetti.Progetto_Cod = " & progetto_cod, "", objParametriServer)

                            If ese.Rows.Count > 0 Then
                                For Each esercizio_db In ese.Rows
                                    'Esegui i controlli solo se le date sono state effettivamente modificate
                                    If validita_inizio <> esercizio_db("Validita_Inizio") Or validita_fine <> esercizio_db("Validita_Fine") Then

                                        Dim objControllo As New AgronicaCoreAnagrafeBIZ.Progetto_W
                                        Dim controllo = objControllo.controllo_CdG(Nothing, piva, sa_cod, appezza, id_reg, progetto_cod, validita_inizio, validita_fine, objParametriServer)

                                        If controllo.errore Then
                                            Dim MessaggioErroreCdG As String = ""
                                            If Not controllo.messaggioSpecifico Then
                                                MessaggioErroreCdG &= (" Impossibile modificare la data di " & controllo.inizio_fine & " dell'esercizio " & progetto_nome & ", perché esistono Costi di Gestione associati in data " & controllo.dataCdG.ToShortDateString())
                                            Else
                                                MessaggioErroreCdG &= (" Impossibile modificare la data di " & controllo.inizio_fine & " dell'esercizio " & progetto_nome & ", perché esistono Costi di Gestione associati")
                                            End If

                                            Throw New GiasException(MessaggioErroreCdG)
                                        End If
                                    End If
                                Next
                            End If
                        End If

                        ' generazione automatica codice per distinta
                        If algoritmo_codifica <> "" AndAlso String.IsNullOrEmpty(distinta.GetValue("Progetto_Nome")) Then
                            Dim progetto_anno = CDate(distinta.GetValue("Validita_Inizio").ToString).Year
                            Dim progetto_codice = Replica_GIAS.LeggiCodiceProgressivo(piva, enum_SequenzaProgressiviTipi.CodiciOPAgriZoo, progetto_anno, objParametriServer)
                            distinta("Progetto_Nome") = progetto_codice
                        End If

                        Dim Finalita_Concimazione_Cod = ""
                        If distinta.GetValue("Finalita_Concimazione_Cod") IsNot Nothing Then
                            Finalita_Concimazione_Cod = distinta.GetValue("Finalita_Concimazione_Cod")
                        End If

                        Dim TxtN = ""
                        If distinta.GetValue("TxtN") IsNot Nothing Then
                            TxtN = distinta.GetValue("TxtN")
                        End If

                        Dim TxtP2O5 = ""
                        If distinta.GetValue("TxtP2O5") IsNot Nothing Then
                            TxtP2O5 = distinta.GetValue("TxtP2O5")
                        End If

                        Dim TxtK2O = ""
                        If distinta.GetValue("TxtK2O") IsNot Nothing Then
                            TxtK2O = distinta.GetValue("TxtK2O")
                        End If

                        Dim TxtMgO = ""
                        If distinta.GetValue("TxtMgO") IsNot Nothing Then
                            TxtMgO = distinta.GetValue("TxtMgO")
                        End If

                        Dim Organismo_Referente = ""
                        If distinta.GetValue("Organismo_Referente") IsNot Nothing Then
                            Organismo_Referente = distinta.GetValue("Organismo_Referente")
                        End If

                        Dim Riferimento_Trasferimento_Dati = ""
                        If distinta.GetValue("Riferimento_Trasferimento_Dati") IsNot Nothing Then
                            Riferimento_Trasferimento_Dati = distinta.GetValue("Riferimento_Trasferimento_Dati")
                        End If

                        Dim Magazzino_Conferimento = ""
                        If distinta.GetValue("Magazzino_Conferimento") IsNot Nothing Then
                            Magazzino_Conferimento = distinta.GetValue("Magazzino_Conferimento")
                        End If

                        Dim Capitolato_Privato = ""
                        If distinta.GetValue("Capitolato_Privato") IsNot Nothing Then
                            Capitolato_Privato = distinta.GetValue("Capitolato_Privato")
                        End If

                        Dim Licenza_Coltivazione = ""
                        If distinta.GetValue("Licenza_Coltivazione") IsNot Nothing Then
                            Licenza_Coltivazione = distinta.GetValue("Licenza_Coltivazione")
                        End If

                        Dim Impianto_IAF_ImpegniAggiuntiviFacoltativi = ""
                        If distinta.GetValue("Impianto_IAF_ImpegniAggiuntiviFacoltativi") IsNot Nothing AndAlso distinta.GetValue("Impianto_IAF_ImpegniAggiuntiviFacoltativi").ToString <> "" Then
                            Dim arr = JArray.Parse(distinta.GetValue("Impianto_IAF_ImpegniAggiuntiviFacoltativi").ToString)
                            For i = 0 To arr.Count - 1
                                Impianto_IAF_ImpegniAggiuntiviFacoltativi &= arr(i).ToString
                                If i <> arr.Count - 1 Then
                                    Impianto_IAF_ImpegniAggiuntiviFacoltativi &= "|"
                                End If
                            Next
                        End If

                        Dim Impianto_PianoSemina = ""
                        If distinta.GetValue("Impianto_PianoSemina") IsNot Nothing Then
                            Impianto_PianoSemina = distinta.GetValue("Impianto_PianoSemina")
                        End If

                        Dim data_semina_prevista = ""
                        If distinta.GetValue("data_semina_prevista") IsNot Nothing Then
                            data_semina_prevista = distinta.GetValue("data_semina_prevista")
                        End If

                        Dim data_raccolta_prevista = ""
                        If distinta.GetValue("data_raccolta_prevista") IsNot Nothing Then
                            data_raccolta_prevista = distinta.GetValue("data_raccolta_prevista")
                        End If

                        Dim data_fioritura_prevista = ""
                        If distinta.GetValue("data_fioritura_prevista") IsNot Nothing Then
                            data_fioritura_prevista = distinta.GetValue("data_fioritura_prevista")
                        End If

                        Dim id_tr = ""
                        If distinta.GetValue("id_tr") IsNot Nothing Then
                            id_tr = distinta.GetValue("id_tr")
                        End If

                        Dim produzione_prevista = ""
                        If distinta.GetValue("produzione_prevista") IsNot Nothing Then
                            produzione_prevista = distinta.GetValue("produzione_prevista")
                        End If

                        Dim distinta_chiusa = ""
                        If distinta.GetValue("distinta_chiusa") IsNot Nothing Then
                            distinta_chiusa = distinta.GetValue("distinta_chiusa")
                        End If

                        Dim distinta_replica = ""
                        If distinta.GetValue("distinta_replica") IsNot Nothing Then
                            distinta_replica = distinta.GetValue("distinta_replica")
                        End If

                        Dim obj_Codici_Distinta = distinta.GetValue("obj_Codici_Distinta")
                        Dim Altri_Codici = distinta.GetValue("Altri_Codici")
                        Dim obj_Particelle_Distinta = distinta.GetValue("obj_Particelle_Distinta")

                        'Salvo/Modifico Distinta
                        If progetto_cod = 0 Then
                            objDist.ScriviEFxAnagrafica(distinta.ToString, piva, sa_cod, appezza, id_reg, progetto_cod,
                                                        objParametriServer, GiasContext, NoteLog:=NoteLog)
                        Else
                            objDist.ModificaEFxAnagrafica(distinta.ToString, piva, sa_cod, appezza, id_reg, progetto_cod,
                                                          objParametriServer, GiasContext, NoteLog:=NoteLog)
                        End If

                        listaProgetto_Cod.Add(progetto_cod)

                        'Salvo i codici della distinta
                        Dim listCodici = New List(Of Integer)

                        objImp_cod_Dal.ScriviModificaEliminaxProgetto(piva, sa_cod, appezza, id_reg, progetto_cod,
                                                                      enum_CodiciAnagrafe.Finalita_Concimazione_Impianto, Finalita_Concimazione_Cod, True,
                                                                      objParametriServer, GiasContext)
                        listCodici.Add(enum_CodiciAnagrafe.Finalita_Concimazione_Impianto)

                        objImp_cod_Dal.ScriviModificaEliminaxProgetto(piva, sa_cod, appezza, id_reg, progetto_cod,
                                                                      enum_CodiciAnagrafe.Impianto_LimiteN, TxtN, True,
                                                                      objParametriServer, GiasContext)
                        listCodici.Add(enum_CodiciAnagrafe.Impianto_LimiteN)

                        objImp_cod_Dal.ScriviModificaEliminaxProgetto(piva, sa_cod, appezza, id_reg, progetto_cod,
                                                                      enum_CodiciAnagrafe.Impianto_LimiteP, TxtP2O5, True,
                                                                      objParametriServer, GiasContext)
                        listCodici.Add(enum_CodiciAnagrafe.Impianto_LimiteP)

                        objImp_cod_Dal.ScriviModificaEliminaxProgetto(piva, sa_cod, appezza, id_reg, progetto_cod,
                                                                      enum_CodiciAnagrafe.Impianto_LimiteK, TxtK2O, True,
                                                                      objParametriServer, GiasContext)
                        listCodici.Add(enum_CodiciAnagrafe.Impianto_LimiteK)

                        objImp_cod_Dal.ScriviModificaEliminaxProgetto(piva, sa_cod, appezza, id_reg, progetto_cod,
                                                                      enum_CodiciAnagrafe.Impianto_LimiteMg, TxtMgO, True,
                                                                      objParametriServer, GiasContext)
                        listCodici.Add(enum_CodiciAnagrafe.Impianto_LimiteMg)

                        objImp_cod_Dal.ScriviModificaEliminaxProgetto(piva, sa_cod, appezza, id_reg, progetto_cod,
                                                                      enum_CodiciAnagrafe.Organismo_Referente, Organismo_Referente, True,
                                                                      objParametriServer, GiasContext)
                        listCodici.Add(enum_CodiciAnagrafe.Organismo_Referente)

                        objImp_cod_Dal.ScriviModificaEliminaxProgetto(piva, sa_cod, appezza, id_reg, progetto_cod,
                                                                      enum_CodiciAnagrafe.Riferimento_Trasferimento_Dati, Riferimento_Trasferimento_Dati, True,
                                                                      objParametriServer, GiasContext)
                        listCodici.Add(enum_CodiciAnagrafe.Riferimento_Trasferimento_Dati)

                        objImp_cod_Dal.ScriviModificaEliminaxProgetto(piva, sa_cod, appezza, id_reg, progetto_cod,
                                                                      enum_CodiciAnagrafe.Magazzino_Conferimento, Magazzino_Conferimento, True,
                                                                      objParametriServer, GiasContext)
                        listCodici.Add(enum_CodiciAnagrafe.Magazzino_Conferimento)

                        objImp_cod_Dal.ScriviModificaEliminaxProgetto(piva, sa_cod, appezza, id_reg, progetto_cod,
                                                                      enum_CodiciAnagrafe.Capitolato_Privato, Capitolato_Privato, True,
                                                                      objParametriServer, GiasContext)
                        listCodici.Add(enum_CodiciAnagrafe.Capitolato_Privato)

                        objImp_cod_Dal.ScriviModificaEliminaxProgetto(piva, sa_cod, appezza, id_reg, progetto_cod,
                                                                      enum_CodiciAnagrafe.Zespri_Fasi_Fase, Licenza_Coltivazione, True,
                                                                      objParametriServer, GiasContext)
                        listCodici.Add(enum_CodiciAnagrafe.Zespri_Fasi_Fase)

                        objImp_cod_Dal.ScriviModificaEliminaxProgetto(piva, sa_cod, appezza, id_reg, progetto_cod,
                                                                      enum_CodiciAnagrafe.Impianto_PianoSemina, Impianto_PianoSemina, True,
                                                                      objParametriServer, GiasContext)
                        listCodici.Add(enum_CodiciAnagrafe.Impianto_PianoSemina)

                        objImp_cod_Dal.ScriviModificaEliminaxProgetto(piva, sa_cod, appezza, id_reg, progetto_cod,
                                                                      enum_CodiciAnagrafe.Distinta_Chiusa, distinta_chiusa, True,
                                                                      objParametriServer, GiasContext)
                        listCodici.Add(enum_CodiciAnagrafe.Distinta_Chiusa)

                        objImp_cod_Dal.ScriviModificaEliminaxProgetto(piva, sa_cod, appezza, id_reg, progetto_cod,
                                                                      enum_CodiciAnagrafe.Impianto_IAF_ImpegniAggiuntiviFacoltativi, Impianto_IAF_ImpegniAggiuntiviFacoltativi, True,
                                                                      objParametriServer, GiasContext)
                        listCodici.Add(enum_CodiciAnagrafe.Impianto_IAF_ImpegniAggiuntiviFacoltativi)

                        ' aggiungo alla lista codici per evitare che venga cancellato
                        listCodici.Add(enum_CodiciAnagrafe.Codice_Impianto_Ribaltato)

                        ' ribaltamento dati su chiusura distinta, in caso di errori lancio eccezione
                        If distinta_chiusa = "1" OrElse distinta_replica = "1" Then
                            Dim objReplica As New AgronicaCoreAnagrafeBIZ.Replica_GIAS
                            Dim errReplica = objReplica.Replica_Impianto(piva, sa_cod, appezza, id_reg, campo_cod, progetto_cod, objParametriServer)
                            If Not String.IsNullOrEmpty(errReplica) Then
                                Throw New Exception(errReplica)
                            End If
                        End If

                        If distinta.GetValue("obj_Codici_Distinta") IsNot Nothing Then
                            'Dim aaa = JObject.Parse(distinta.GetValue("obj_Codici_Distinta").ToString)
                            'Dim kra = aaa.GetValue("kendo_rows")
                            Dim codici_distinta = JArray.Parse(distinta.GetValue("obj_Codici_Distinta").ToString)

                            For Each codice1 In codici_distinta
                                Dim codice = JObject.Parse(codice1.ToString)
                                Dim id_cod = codice.GetValue("id_cod")
                                Dim val_cod = codice.GetValue("val_cod")
                                objImp_cod_Dal.ScriviModificaEliminaxProgetto(piva, sa_cod, appezza, id_reg, progetto_cod, id_cod, val_cod, True,
                                                                              objParametriServer, GiasContext)
                                listCodici.Add(id_cod)
                            Next

                        End If

                        'Gestione Particelle distinta
                        Dim prog_partl = From pp In GiasContext.ProgettixParticelle
                                         Where pp.PIVA = piva AndAlso
                                               pp.sa_cod = sa_cod AndAlso
                                               pp.appezza = appezza AndAlso
                                               pp.Id_Reg = id_reg AndAlso
                                               pp.Progetto_Cod = progetto_cod
                                         Select pp

                        If prog_partl.Count > 0 Then
                            For Each prog_part In prog_partl
                                GiasContext.ProgettixParticelle.Remove(prog_part)
                            Next
                        End If
                        GiasContext.SaveChanges()

                        If distinta.GetValue("obj_Particelle_Distinta") IsNot Nothing Then

                            Dim particelle_distinta = JArray.Parse(distinta.GetValue("obj_Particelle_Distinta").ToString)
                            For Each part1 In particelle_distinta
                                Dim particella = JObject.Parse(part1.ToString)
                                Dim val_cod = particella.GetValue("val_cod").ToString
                                Dim prov = particella.GetValue("prov").ToString
                                Dim com = particella.GetValue("com").ToString

                                Dim sezione = "0"
                                If particella.GetValue("sezione").ToString IsNot Nothing AndAlso particella.GetValue("sezione").ToString <> "" Then
                                    sezione = particella.GetValue("sezione").ToString
                                End If

                                Dim foglio = particella.GetValue("foglio").ToString
                                Dim numero = particella.GetValue("numero").ToString

                                Dim subalterno = "0"
                                If particella.GetValue("subalterno").ToString IsNot Nothing AndAlso particella.GetValue("subalterno").ToString <> "" Then
                                    subalterno = particella.GetValue("subalterno")
                                End If

                                Dim id_cod = particella.GetValue("id_cod").ToString

                                objPrj_part.ScriviModificaElimina(piva, sa_cod, appezza, id_reg, progetto_cod,
                                                                  prov, com, sezione, foglio, numero, subalterno, id_cod, val_cod,
                                                                  objParametriServer, GiasContext)

                            Next

                        End If

                        'Cancello gli eventuali codici della distinta non impostati
                        Dim imp_codl = From ic In GiasContext.Reg_Impianti_Codici
                                       Where ic.PIVA = piva AndAlso
                                             ic.sa_cod = sa_cod AndAlso
                                             ic.appezza = appezza AndAlso
                                             ic.Id_Reg = id_reg AndAlso
                                             ic.Progetto_Cod = progetto_cod AndAlso
                                             Not listCodici.Contains(ic.id_cod)
                                       Select ic

                        If imp_codl.Count > 0 Then
                            For Each imp_cod In imp_codl
                                GiasContext.Entry(imp_cod).State = EntityState.Deleted
                                GiasContext.SaveChanges()
                            Next
                        End If

                    Next

                End If

                'ELIMINO LE DISTINTE CHE NON TROVO
                Dim imp_prol = From ic In GiasContext.Imprese_Progetti
                               Where ic.Piva = piva And
                                     ic.Sa_Cod = sa_cod And
                                     ic.Appezza = appezza And
                                     ic.Id_Reg = id_reg And
                                     Not listaProgetto_Cod.Contains(ic.Progetto_Cod)
                               Select ic

                If imp_prol.Count > 0 Then
                    For Each imp_pro In imp_prol
                        objDist.EliminaEFxAnagrafica(imp_pro, objParametriServer, GiasContext, NoteLog:=NoteLog)
                    Next
                End If

                Dim imp_codl1 = From ic In GiasContext.Reg_Impianti_Codici
                                Where ic.PIVA = piva AndAlso
                                      ic.sa_cod = sa_cod AndAlso
                                      ic.appezza = appezza AndAlso
                                      ic.Id_Reg = id_reg AndAlso
                                      Not listaProgetto_Cod.Contains(ic.Progetto_Cod) AndAlso
                                      ic.Progetto_Cod <> 0
                                Select ic

                If imp_codl1.Count > 0 Then
                    For Each imp_cod In imp_codl1
                        'GiasContext.Entry(imp_cod).State = EntityState.Deleted
                        GiasContext.Reg_Impianti_Codici.Remove(imp_cod)
                    Next
                End If

                GiasContext.SaveChanges()

                scope.Complete()
                scope.Dispose()

                r.RispostaOK = True
                r.RispostaStringa = "Salvataggio effettuato con successo"

            Catch ex As Exception

                r.RispostaOK = False

                messaggioErrore = ex.Message
                Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)

                r.Errore = "[" & nomeRoutine & "] : " & messaggioErrore
                scope.Dispose()

            Finally

                GiasContext.Dispose()

            End Try

        End Using

        Return r

    End Function

    Private Function Impianto_Scrivi(ByVal dati_impianto As String,
                                    ByRef OUTPUT_Piva As String,
                                    ByRef OUTPUT_Sa_Cod As Integer,
                                    ByRef OUTPUT_Appezza As Integer,
                                    ByRef OUTPUT_Id_Reg As Integer,
                                    ByRef OUTPUT_Campo_Cod As Integer,
                                    ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                    ByRef GiasContext As Gias_DeveloperServer_Entities,
                                    Optional NoteLog As String = ""
                                    ) As Boolean

        Dim nomeRoutine As String = "AnagrafeBIZ.Reg_Impianto_W.Impianto_Scrivi"
        Dim messaggioErrore As String = ""

        Dim xRisp As Boolean = False

        Try

            Dim obj_Impianto = Newtonsoft.Json.JsonConvert.DeserializeObject(dati_impianto)

            Dim validita_inizio = CDate(obj_Impianto.GetValue("impianto_data_inizio"))
            Dim validita_fine = CDate(obj_Impianto.GetValue("impianto_data_fine"))

            Dim cul_cod = CInt(obj_Impianto.GetValue("cul_cod"))
            Dim veg_cod = CInt(obj_Impianto.GetValue("veg_cod"))
            Dim gru_cod = 0
            If IsNumeric(obj_Impianto.GetValue("gru_cod")) Then
                gru_cod = obj_Impianto.GetValue("gru_cod")
            End If

            Dim port_cod = 0
            If IsNumeric(obj_Impianto.GetValue("port_cod")) Then
                port_cod = obj_Impianto.GetValue("port_cod")
            End If
            Dim foral_cod = 0
            If IsNumeric(obj_Impianto.GetValue("foral_cod")) Then
                foral_cod = obj_Impianto.GetValue("foral_cod")
            End If
            Dim imp_cod2 = 0
            If IsNumeric(obj_Impianto.GetValue("imp_cod2")) Then
                imp_cod2 = obj_Impianto.GetValue("imp_cod2")
            End If
            Dim provenienza_seme As Integer = 0
            If IsNumeric(obj_Impianto.GetValue("provenienzaseme")) Then
                provenienza_seme = CInt(obj_Impianto.GetValue("provenienzaseme"))
            End If
            Dim cop_cod As Integer = 0
            If IsNumeric(obj_Impianto.GetValue("cop_cod")) Then
                cop_cod = CInt(obj_Impianto.GetValue("cop_cod"))
            End If
            Dim sup_imp = CDbl(obj_Impianto.GetValue("sup_imp"))

            Dim grfi_cod As Integer = 0
            If IsNumeric(obj_Impianto.GetValue("grfi_cod")) Then
                grfi_cod = CInt(obj_Impianto.GetValue("grfi_cod"))
            End If

            Dim id_cod_terreno = CInt(obj_Impianto.GetValue("id_cod_terreno"))
            Dim grva_cod = 0
            If IsNumeric(obj_Impianto.GetValue("grva_cod")) Then
                grva_cod = obj_Impianto.GetValue("grva_cod")
            End If
            Dim setup_cod = obj_Impianto.GetValue("setup_cod")

            Dim coverCrops = obj_Impianto.GetValue("ChkCoverCrops")
            Dim coverInt As Int16
            If coverCrops Then
                coverInt = 1
            Else
                coverInt = 0
            End If

            Dim monitorato = obj_Impianto.GetValue("ChkMonitorato")
            Dim monitoratoInt As Int16
            If monitorato Then
                monitoratoInt = 1
            Else
                monitoratoInt = 0
            End If

            Dim resa_prevista = obj_Impianto.GetValue("Resa_Prevista")
            If resa_prevista Is Nothing Then
                resa_prevista = 0
            End If

            Dim resa_effettiva = obj_Impianto.GetValue("Resa_Effettiva")
            If resa_effettiva Is Nothing Then
                resa_effettiva = 0
            End If

            Dim cop_data_inizio = obj_Impianto.GetValue("cop_data_inizio").ToString
            If IsDate(cop_data_inizio) Then
                cop_data_inizio = CDate(cop_data_inizio)
                If cop_data_inizio < AGRODATAINIZIO Then
                    cop_data_inizio = AGRODATAINIZIO
                End If
            Else
                cop_data_inizio = AGRODATAINIZIO
            End If

            Dim cop_data_fine = obj_Impianto.GetValue("cop_data_fine").ToString
            If IsDate(cop_data_fine) Then
                cop_data_fine = CDate(cop_data_fine)
                If cop_data_fine < AGRODATAINIZIO Then
                    cop_data_fine = AGRODATAFINE
                End If
            Else
                cop_data_fine = AGRODATAFINE
            End If

            Dim tra_fila_m = obj_Impianto.GetValue("tra_fila_m")
            Dim su_fila_m = obj_Impianto.GetValue("su_fila_m")
            Dim tecn_cod = obj_Impianto.GetValue("tecn_cod")
            If Not IsNumeric(tecn_cod) Then
                tecn_cod = 0
            End If
            Dim su_cod = obj_Impianto.GetValue("su_cod")
            If Not IsNumeric(su_cod) Then
                su_cod = 0
            End If

            Dim interbina = obj_Impianto.GetValue("interbina")
            Dim germinabilita = obj_Impianto.GetValue("germinabilita")
            Dim dettaglio_varieta_personalizzato = obj_Impianto.GetValue("dettaglio_varieta_personalizzato")
            Dim impianto_ibrido = obj_Impianto.GetValue("impianto_ibrido")

            Dim unita_vitataInt As Integer
            If IsNumeric(obj_Impianto.GetValue("unita_vitata")) Then
                unita_vitataInt = CInt(obj_Impianto.GetValue("unita_vitata"))
            Else
                unita_vitataInt = 0
            End If

            Dim Data_Inizio_Portinnesto = obj_Impianto.GetValue("Data_Inizio_Portinnesto").ToString
            If IsDate(Data_Inizio_Portinnesto) Then
                Data_Inizio_Portinnesto = CDate(Data_Inizio_Portinnesto)
                If Data_Inizio_Portinnesto < AGRODATAINIZIO Then
                    Data_Inizio_Portinnesto = AGRODATAFINE
                End If
            Else
                Data_Inizio_Portinnesto = AGRODATAFINE
            End If

            Dim Data_Inizio_Innesto = obj_Impianto.GetValue("Data_Inizio_Innesto").ToString
            If IsDate(Data_Inizio_Innesto) Then
                Data_Inizio_Innesto = CDate(Data_Inizio_Innesto)
                If Data_Inizio_Innesto < AGRODATAINIZIO Then
                    Data_Inizio_Innesto = AGRODATAFINE
                End If
            Else
                Data_Inizio_Innesto = AGRODATAFINE
            End If

            Dim Data_Inizio_Produzione = obj_Impianto.GetValue("Data_Inizio_Produzione").ToString
            If IsDate(Data_Inizio_Produzione) Then
                Data_Inizio_Produzione = CDate(Data_Inizio_Produzione)
                If Data_Inizio_Produzione < AGRODATAINIZIO Then
                    Data_Inizio_Produzione = AGRODATAFINE
                End If
            Else
                Data_Inizio_Produzione = AGRODATAFINE
            End If

            Dim Piante_Maschi_InSesto = obj_Impianto.GetValue("ChkMaschiSesto")
            Dim Piante_Maschi_InSestoInt As Int16
            If Piante_Maschi_InSesto Then
                Piante_Maschi_InSestoInt = 1
            Else
                Piante_Maschi_InSestoInt = 0
            End If

            Dim CodBMBDBT_M = obj_Impianto.GetValue("CodBMBDBT_M")
            Dim CodBMBDBT_F = obj_Impianto.GetValue("CodBMBDBT_F")
            Dim Genetica_M = obj_Impianto.GetValue("Genetica_M")
            Dim Genetica_F = obj_Impianto.GetValue("Genetica_F")
            Dim OffType_M = obj_Impianto.GetValue("OffType_M")
            Dim OffType_F = obj_Impianto.GetValue("OffType_F")
            Dim DistanzaSuFila_F = obj_Impianto.GetValue("DistanzaSuFila_F")
            Dim DistanzaTraFila_F = obj_Impianto.GetValue("DistanzaTraFila_F")

            Dim chkConsociazione_checked = obj_Impianto.GetValue("chkConsociazione_checked")
            Dim Id_Consociazione = obj_Impianto.GetValue("id_consociazione")



            Dim imp As New AgronicaCoreEntityFramework_POCO.Reg_Impianti

            imp.PIVA = OUTPUT_Piva
            imp.SA_COD = OUTPUT_Sa_Cod
            imp.APPEZZA = OUTPUT_Appezza
            imp.ID_REG = OUTPUT_Id_Reg
            imp.DATA_AGG = Now.Date
            imp.COD_RESP = 0
            imp.COD_ENTE = 0
            imp.CAMPO_SPIA = 0
            imp.DATA = Now.Date
            imp.CUL_COD = cul_cod
            imp.GRFI_COD = grfi_cod

            imp.PRODUZIONE = 0
            imp.Resa_Prevista = 0
            imp.Resa_Effettiva = 0
            imp.SCARTO = 0
            imp.IND_MAT_COD = 0
            imp.IND_MAT_RIL = 0
            imp.STA_TER = ""
            imp.COP_DI = cop_data_inizio
            imp.COP_DF = cop_data_fine
            imp.TRA_FILA = 0
            imp.SU_FILA = 0
            imp.P_HA = 0
            imp.Piante_Maschi_InSesto = Piante_Maschi_InSestoInt
            imp.Data_Inizio_Innesto = Data_Inizio_Innesto
            imp.Data_Inizio_Produzione = Data_Inizio_Produzione
            imp.Data_Inizio_Portinnesto = Data_Inizio_Portinnesto
            imp.FORAL_COD = foral_cod
            imp.SETUP_COD = setup_cod
            imp.PORT_COD = port_cod
            imp.IMP_COD = imp_cod2
            imp.STRU_PROT = 0
            imp.PRO_PAG = 0
            imp.SEME_Q = 0
            imp.SEME_T = 0
            imp.SEME_P = 0
            imp.SEME_D = 0
            imp.STATO_RESIDUI = ""
            imp.TECN_COD = tecn_cod
            imp.DENITRIFICAZIONE = 0
            imp.VOLATILIZZAZIONE = 0
            imp.PROFONDITALAV = 0
            imp.ID_CAMPO = OUTPUT_Campo_Cod
            imp.SU_COD = su_cod
            imp.COP_COD = cop_cod
            imp.COVER = coverInt
            imp.MONITORATO = monitoratoInt
            imp.CODICE_FISCALE_TECNICO = ""
            imp.USER = objParametriServer.PivaSuperUser
            imp.REGOLAMENTO = 1
            imp.FINANZIAMENTO = 0
            imp.ProvenienzaSeme = provenienza_seme
            imp.inviato = 0
            imp.Data_Creazione = DateTime.Now
            imp.Data_Modifica = DateTime.Now
            imp.Username_Creazione = objParametriServer.UsernameOperazione
            imp.Username_Modifica = objParametriServer.UsernameOperazione
            imp.Validita_Inizio = validita_inizio
            imp.Validita_Fine = validita_fine
            imp.Validazione = 0
            imp.UserName_Validazione = ""
            imp.Sup_Imp = sup_imp
            imp.GRVA_Cod_VEG = grva_cod
            imp.Id_Consociazione = Id_Consociazione
            imp.Unita_Vitata = unita_vitataInt
            imp.Sovrainnesto_Cod = 0

            GiasContext.Reg_Impianti.Add(imp)
            GiasContext.SaveChanges()

            'Scrittura tabella Agronica_Log_Anagrafe
            Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
            Dim log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.Impianti,
                                                                                 imp.PIVA, CStr(imp.SA_COD),
                                                                                 CStr(imp.APPEZZA), CStr(imp.ID_REG),
                                                                                 Nothing, Nothing,
                                                                                 enum_TipoOperazioneDB.Scrittura,
                                                                                 objParametriServer, enum_Id_Servizio.GiasOnline,
                                                                                 note:=NoteLog)

            GiasContext.Agronica_Log_Anagrafe.Add(log)
            GiasContext.SaveChanges()

            xRisp = True

        Catch ex As Exception
            xRisp = False
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Private Function Impianto_Modifica(ByVal dati_impianto As String,
                                      ByVal OUTPUT_Piva As String,
                                      ByVal OUTPUT_Sa_Cod As Integer,
                                      ByVal OUTPUT_Appezza As Integer,
                                      ByVal OUTPUT_Id_Reg As Integer,
                                      ByVal OUTPUT_Campo_Cod As Integer,
                                      ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                      ByRef GiasContext As Gias_DeveloperServer_Entities,
                                      Optional NoteLog As String = ""
                                      ) As Boolean

        Dim nomeRoutine As String = "AnagrafeBIZ.Reg_Impianto_W.Impianto_Modifica"
        Dim messaggioErrore As String = ""

        Dim xRisp As Boolean = False

        Try

            Dim obj_Impianto = Newtonsoft.Json.JsonConvert.DeserializeObject(dati_impianto)

            Dim validita_inizio = AGRODATAINIZIO
            If IsDate(obj_Impianto.GetValue("impianto_data_inizio").ToString) Then
                validita_inizio = CDate(obj_Impianto.GetValue("impianto_data_inizio"))
            End If

            Dim validita_fine = AGRODATAFINE
            If IsDate(obj_Impianto.GetValue("impianto_data_fine").ToString) Then
                validita_fine = CDate(obj_Impianto.GetValue("impianto_data_fine"))
            End If

            Dim cul_cod = CInt(obj_Impianto.GetValue("cul_cod"))
            Dim veg_cod = CInt(obj_Impianto.GetValue("veg_cod"))
            Dim gru_cod = 0
            If IsNumeric(obj_Impianto.GetValue("gru_cod")) Then
                gru_cod = obj_Impianto.GetValue("gru_cod")
            End If

            Dim port_cod = 0
            If IsNumeric(obj_Impianto.GetValue("port_cod")) Then
                port_cod = obj_Impianto.GetValue("port_cod")
            End If
            Dim foral_cod = 0
            If IsNumeric(obj_Impianto.GetValue("foral_cod")) Then
                foral_cod = obj_Impianto.GetValue("foral_cod")
            End If
            Dim imp_cod2 = 0
            If IsNumeric(obj_Impianto.GetValue("imp_cod2")) Then
                imp_cod2 = obj_Impianto.GetValue("imp_cod2")
            End If

            Dim provenienza_seme = 0
            If IsNumeric(obj_Impianto.GetValue("provenienzaseme")) Then
                provenienza_seme = CInt(obj_Impianto.GetValue("provenienzaseme"))
            End If

            Dim cop_cod = 0
            If IsNumeric(obj_Impianto.GetValue("cop_cod")) Then
                cop_cod = CInt(obj_Impianto.GetValue("cop_cod"))
            End If
            Dim sup_imp = CDbl(obj_Impianto.GetValue("sup_imp"))

            Dim grfi_cod As Integer = 0
            If IsNumeric(obj_Impianto.GetValue("grfi_cod")) Then
                grfi_cod = CInt(obj_Impianto.GetValue("grfi_cod"))
            End If

            Dim id_cod_terreno = CInt(obj_Impianto.GetValue("id_cod_terreno"))
            Dim grva_cod = 0
            If IsNumeric(obj_Impianto.GetValue("grva_cod")) Then
                grva_cod = obj_Impianto.GetValue("grva_cod")
            End If
            Dim setup_cod = obj_Impianto.GetValue("setup_cod")

            Dim coverCrops = obj_Impianto.GetValue("ChkCoverCrops")
            If coverCrops Then
                coverCrops = 1
            Else
                coverCrops = 0
            End If

            Dim monitorato = obj_Impianto.GetValue("ChkMonitorato")
            If monitorato Then
                monitorato = 1
            Else
                monitorato = 0
            End If

            Dim resa_prevista = obj_Impianto.GetValue("Resa_Prevista")
            If Not IsNumeric(resa_prevista) Then
                resa_prevista = 0
            End If

            Dim resa_effettiva = obj_Impianto.GetValue("Resa_Effettiva")
            If Not IsNumeric(resa_effettiva) Then
                resa_effettiva = 0
            End If

            Dim cop_data_inizio = AGRODATAINIZIO
            If IsDate(obj_Impianto.GetValue("cop_data_inizio").ToString) Then
                cop_data_inizio = CDate(obj_Impianto.GetValue("cop_data_inizio").ToString)
                If cop_data_inizio < AGRODATAINIZIO Then
                    cop_data_inizio = AGRODATAINIZIO
                End If
            End If

            Dim cop_data_fine = AGRODATAFINE
            If IsDate(obj_Impianto.GetValue("cop_data_fine").ToString) Then
                cop_data_fine = CDate(obj_Impianto.GetValue("cop_data_fine").ToString)
                If cop_data_fine < AGRODATAINIZIO Then
                    cop_data_fine = AGRODATAFINE
                End If
            End If

            Dim tra_fila_m = obj_Impianto.GetValue("tra_fila_m")
            Dim su_fila_m = obj_Impianto.GetValue("su_fila_m")

            Dim tecn_cod = obj_Impianto.GetValue("tecn_cod")
            If Not IsNumeric(tecn_cod) Then
                tecn_cod = 0
            End If

            Dim su_cod = obj_Impianto.GetValue("su_cod")
            If Not IsNumeric(su_cod) Then
                su_cod = 0
            End If

            Dim codice_zona = obj_Impianto.GetValue("codice_zona")
            Dim interbina = obj_Impianto.GetValue("interbina")
            Dim germinabilita = obj_Impianto.GetValue("germinabilita")
            Dim dettaglio_varieta_personalizzato = obj_Impianto.GetValue("dettaglio_varieta_personalizzato")
            Dim impianto_ibrido = obj_Impianto.GetValue("impianto_ibrido")

            Dim unita_vitata = obj_Impianto.GetValue("unita_vitata")
            If Not IsNumeric(unita_vitata) Then
                unita_vitata = 0
            End If

            ' - - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -
            Dim Data_Inizio_Portinnesto = AGRODATAFINE
            If IsDate(obj_Impianto.GetValue("Data_Inizio_Portinnesto").ToString) Then
                Data_Inizio_Portinnesto = CDate(obj_Impianto.GetValue("Data_Inizio_Portinnesto").ToString)
                If Data_Inizio_Portinnesto < AGRODATAINIZIO Then
                    Data_Inizio_Portinnesto = AGRODATAFINE
                End If
            End If

            Dim Data_Inizio_Innesto = AGRODATAFINE
            If IsDate(obj_Impianto.GetValue("Data_Inizio_Innesto").ToString) Then
                Data_Inizio_Innesto = CDate(obj_Impianto.GetValue("Data_Inizio_Innesto").ToString)
                If Data_Inizio_Innesto < AGRODATAINIZIO Then
                    Data_Inizio_Innesto = AGRODATAFINE
                End If
            End If


            Dim Data_Inizio_Produzione = AGRODATAFINE
            If IsDate(obj_Impianto.GetValue("Data_Inizio_Produzione").ToString) Then
                Data_Inizio_Produzione = CDate(obj_Impianto.GetValue("Data_Inizio_Produzione").ToString)
                If Data_Inizio_Produzione < AGRODATAINIZIO Then
                    Data_Inizio_Produzione = AGRODATAFINE
                End If
            End If

            Dim Piante_Maschi_InSesto = obj_Impianto.GetValue("ChkMaschiSesto")
            If Piante_Maschi_InSesto Then
                Piante_Maschi_InSesto = 1
            Else
                Piante_Maschi_InSesto = 0
            End If
            ' - - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -

            Dim CodBMBDBT_M = obj_Impianto.GetValue("CodBMBDBT_M")
            Dim CodBMBDBT_F = obj_Impianto.GetValue("CodBMBDBT_F")
            Dim Genetica_M = obj_Impianto.GetValue("Genetica_M")
            Dim Genetica_F = obj_Impianto.GetValue("Genetica_F")
            Dim OffType_M = obj_Impianto.GetValue("OffType_M")
            Dim OffType_F = obj_Impianto.GetValue("OffType_F")
            Dim DistanzaSuFila_F = obj_Impianto.GetValue("DistanzaSuFila_F")
            Dim DistanzaTraFila_F = obj_Impianto.GetValue("DistanzaTraFila_F")

            Dim chkConsociazione_checked = obj_Impianto.GetValue("chkConsociazione_checked")
            Dim Id_Consociazione = obj_Impianto.GetValue("id_consociazione")

            Dim reg_impianti = From impianto In GiasContext.Reg_Impianti
                               Where impianto.PIVA = OUTPUT_Piva AndAlso
                                     impianto.SA_COD = OUTPUT_Sa_Cod AndAlso
                                     impianto.APPEZZA = OUTPUT_Appezza AndAlso
                                     impianto.ID_REG = OUTPUT_Id_Reg
                               Select impianto

            Dim imp = reg_impianti.FirstOrDefault

            imp.Validita_Inizio = validita_inizio
            imp.Validita_Fine = validita_fine
            imp.ID_CAMPO = OUTPUT_Campo_Cod
            imp.CUL_COD = cul_cod
            imp.PORT_COD = port_cod
            imp.FORAL_COD = foral_cod
            imp.IMP_COD = imp_cod2
            imp.ProvenienzaSeme = provenienza_seme
            imp.COP_COD = cop_cod
            imp.Sup_Imp = sup_imp
            imp.GRFI_COD = grfi_cod
            imp.GRVA_Cod_VEG = grva_cod
            imp.SETUP_COD = setup_cod
            imp.COVER = CInt(coverCrops)
            imp.MONITORATO = CInt(monitorato)
            imp.Resa_Effettiva = CDbl(resa_effettiva)
            imp.Resa_Prevista = CDbl(resa_prevista)
            imp.COP_DI = cop_data_inizio
            imp.COP_DF = cop_data_fine
            imp.TRA_FILA = 0
            imp.SU_FILA = 0
            imp.TECN_COD = CInt(tecn_cod)
            imp.SU_COD = CInt(su_cod)
            imp.DATA_AGG = Now.Date
            imp.Data_Modifica = DateTime.Now
            imp.Username_Modifica = objParametriServer.UsernameOperazione
            imp.Id_Consociazione = CInt(Id_Consociazione)
            imp.Unita_Vitata = CInt(unita_vitata)

            ' - - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -
            imp.Data_Inizio_Portinnesto = Data_Inizio_Portinnesto
            imp.Data_Inizio_Innesto = Data_Inizio_Innesto
            imp.Data_Inizio_Produzione = Data_Inizio_Produzione
            imp.Piante_Maschi_InSesto = CInt(Piante_Maschi_InSesto)
            ' - - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -

            GiasContext.Entry(imp).State = EntityState.Modified

            GiasContext.SaveChanges()

            'Scrittura tabella Agronica_Log_Anagrafe
            Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
            Dim log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.Impianti,
                                                                                 imp.PIVA, CStr(imp.SA_COD),
                                                                                 CStr(imp.APPEZZA), CStr(imp.ID_REG),
                                                                                 Nothing, Nothing,
                                                                                 enum_TipoOperazioneDB.Modifica,
                                                                                 objParametriServer, enum_Id_Servizio.GiasOnline,
                                                                                 note:=NoteLog)

            GiasContext.Agronica_Log_Anagrafe.Add(log)
            GiasContext.SaveChanges()

            xRisp = True

        Catch ex As Exception
            xRisp = False
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

#End Region

    ''' <summary>
    ''' Apre un nuovo impianti di terreno nudo sull'appezzamento le cui chiavi vengono passate alla funzione
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="Appezza"></param>
    ''' <param name="OUTPUT_Id_Reg"></param>
    ''' <param name="BaseCode"></param>
    ''' <param name="TopCode"></param>
    ''' <param name="Errore"></param>
    ''' <returns></returns>
    Public Function Apertura_Impianto(
            ByVal Piva As String,
            ByVal Sa_Cod As Integer,
            ByVal Appezza As Integer,
            ByRef OUTPUT_Id_Reg As Integer,
            ByVal BaseCode As Integer,
            ByVal TopCode As Integer,
            ByRef Errore As String,
            ByVal objparametri_server As AgronicaCoreParametri,
            Optional NoteLog As String = "") As Boolean


        Dim DesChiaveImpianto As String = ""
        Dim objImpRead As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        Dim objRegCodiciRead As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R
        Dim objXMLUt As New AgronicaCoreXML.XML_Utility
        Dim DT As DataTable
        Dim flag_insert = True
        Dim Validita_fine_App As Date

        Dim data_inizio_prec As Date
        Dim data_fine_prec As Date
        Dim sup_imp As Double


        'nuovi dati distinta
        Dim data_inizio As Date
        Dim data_fine As Date
        Dim progetto_nome As String
        Dim progetto_des As String

        'leggo i dati della distinta più vecchia (l'ultima)
        DT = objImpRead.Leggi(Piva, Sa_Cod, Appezza,
                              OUTPUT_Id_Reg,
                              enumSelezioneVariabile.Selezione_JoinDescrizioni,
                              "", " Reg_Impianti.Validita_Inizio DESC",
                              objparametri_server)

        If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

            With DT.Rows(0)
                data_inizio_prec = .Item("validita_inizio")
                data_fine_prec = .Item("validita_fine")
                Validita_fine_App = .Item("Validita_fine_Appezzamento")
                sup_imp = .Item("sup_imp")
            End With

            'credo le nuove date 

            'imposto la data inizio come la data fine della distinta precedente piu un giorno
            data_inizio = DateAdd(DateInterval.Day, 1, data_fine_prec)

            If data_inizio > Validita_fine_App Then
                'errore!!!! non dovrebbe mai finire qui
                Errore &= DesChiaveImpianto & "errore, la nuova data di fine inizio supera la data di fine appezzamento!" & vbCrLf
                Return False
            End If

            'la edit impianto fa: imposto la data fine come la data fine della distinta precedente + 1 anno
            'che è uguale a fare la data inizio + 1 anno - 1 giorno
            data_fine = DateAdd(DateInterval.Year, 1, data_fine_prec)

            If data_fine > Validita_fine_App Then
                'la data fine della distinta non può superare la data fine dell'impianto
                'quindi la imposto uguale alla data fine impianto
                data_fine = Validita_fine_App
            End If


            'lotto
            If data_inizio.Year <> data_fine.Year Then
                progetto_nome = "Lotto " & CStr(data_inizio.Year) & "/" & CStr(data_fine.Year)
            Else
                progetto_nome = "Lotto " & CStr(data_inizio.Year)
            End If

            progetto_des = progetto_nome

            Dim esitoScrittura As Boolean = Apertura_Impianto_Scrivi(
                OUTPUT_Id_Reg,
                Errore,
                BaseCode,
                TopCode,
                Piva, Sa_Cod, Appezza,
                sup_imp,
                data_inizio,
                data_fine,
                progetto_des,
                DesChiaveImpianto,
                objparametri_server,
                NoteLog:=NoteLog
                )


        Else
            Errore &= DesChiaveImpianto & "errore durante la lettura dei dati dell\'impianto attualmente attivo." & vbCrLf
            Return False
        End If


        If Errore <> "" Then
            Return False
        End If


        Return True

    End Function

    Public Function Apertura_Impianto_Scrivi(
            ByRef OUTPUT_Id_Reg As Integer,
            ByRef Errore As String,
            ByVal BaseCode As Integer,
            ByVal TopCode As Integer,
            ByVal piva As String,
            ByVal sa_cod As Integer,
            ByVal appezza As Integer,
            ByVal sup_imp As Double,
            ByVal data_inizio As Date,
            ByVal data_fine As Date,
            ByVal progetto_Des As String,
            ByVal DesChiaveImpianto As String,
            ByVal objParametri_Server As AgronicaCoreParametri,
            Optional NoteLog As String = ""
    ) As Boolean

        Dim objXML As New AgronicaCoreXML.XML_Anagrafe

        Dim XmlDoc As XmlDocument
        XmlDoc = New XmlDocument

        Dim StrImpianto As String

        'Creo il nodo "DatiReg_Impianti"
        Dim XmlDatiReg_Impianti As XmlElement
        XmlDatiReg_Impianti = XmlDoc.CreateElement("DatiReg_Impianti")



        StrImpianto = objXML.XML_Impianto(
            Errore,
            XmlDoc,
            BaseCode,
            TopCode,
            Nothing,
            Nothing,
            enum_TipoOperazioneDB.Scrittura,
            enum_TipoOperazioneDB.Scrittura,
            objParametri_Server.PivaSuperUser,
            piva,
            sa_cod,
            appezza,
            0,
            0,
            sup_imp,
            0,
            0,
            data_inizio,
            data_fine,
            data_inizio,
            0,
            progetto_Des,
            progetto_Des,
            CAU_PROGETTO_PRODUZIONE,
            data_inizio,
            data_fine).OuterXml

        XmlDatiReg_Impianti.InnerXml = StrImpianto

        If Errore <> "" Then
            Return False
        End If

        Dim StringaXmlCreazione As String = XmlDatiReg_Impianti.OuterXml()

        If StringaXmlCreazione <> "" Then


            Try

                '-----------------------------------------------------
                '--------------- SCRITTURA  --------------------

                Dim OUT_Piva As String = "" 'non usato

                Dim flag_insert As Boolean = Reg_Impianto_Scrivi(StringaXmlCreazione,
                                                        OUT_Piva, 0, 0, OUTPUT_Id_Reg,
                                                         "", objParametri_Server, NoteLog:=NoteLog)
                '-----------------------------------------------------

                Return flag_insert

            Catch ex As Exception
                Errore &= ex.Message & vbCrLf
                Return False
            End Try

        Else
            Errore &= DesChiaveImpianto & "errore durante la creazione dei dati del nuovo impianto." & vbCrLf
            Return False
        End If

    End Function

    Public Sub Modifica_Finalita(ByRef impiantoRif As RifImpianto, grfiCod As Integer)

    End Sub

    Public Shared Function Esistono_Impianti_Su_Appezzamenti(
                                           ByVal Piva As String,
                                           ByVal Sa_Cod As Integer,
                                           ByVal Appezza As Integer,
                                           ByVal Id_Reg As Integer,
                                               ByRef objParametri As AgronicaCoreParametri
                                               ) As Boolean

        Dim DTImpianto As DataTable

        Dim objImpianto As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read

        DTImpianto = objImpianto.Leggi(CStr(Piva), CInt(Sa_Cod), CInt(Appezza), 0,
                                       enumSelezioneVariabile.Selezione_TabellaCompleta,
                                       "ID_REG <> " & Id_Reg,
                                       "", objParametri)

        objImpianto = Nothing

        If DTImpianto.Rows.Count > 0 Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Function controllo_MovimentiRicettePua(DatiImpianto As AgronicaCoreModelsSTD.anagrafiche.Impianto,
                                                  piva As String,
                                                  sa_cod As Integer,
                                                  appezza As Integer,
                                                  id_reg As Integer,
                                                  Validita_Inizio As Date,
                                                  Validita_Fine As Date,
                                                  ByRef objParametri As AgronicaCoreParametri)

        Dim MessaggioErrore As String = ""
        Dim nomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Reg_Impianto.controllo_MovimentiRicettePua()"
        Dim dtAgenda As New DataTable
        Dim dtRicette As New DataTable

        Dim objDestR As New AgronicaCoreContabDAL.Mov_Destinazioni_R
        Dim ObjRicette As New AgronicaCoreContabDAL.Ricette_Destinazioni_R

        Try
            dtAgenda = objDestR.LeggiCronologiaMovimenti(piva, sa_cod, appezza, id_reg,
                                                     enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                     " ( Data_Movimento > " & Agro_SQL_SaveDate(Validita_Fine, False) & " OR Data_Movimento < " & Agro_SQL_SaveDate(Validita_Inizio, False) & " ) ",
                                                     "",
                                                     objParametri, joinDescrizioneImpianto:=True)

            dtRicette = ObjRicette.Leggi(0, 0, 0, 0, 0,
                                        piva, sa_cod, appezza, id_reg,
                                        AGRODATAINIZIO, AGRODATAFINE,
                                        enumSelezioneVariabile.Selezione_TabellaCompleta,
                                        " ( Ricette_Operazioni.Validita_inizio > " & Agro_SQL_SaveDate(Validita_Fine, False) & " OR Ricette_Operazioni.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Inizio, False) & " ) ",
                                        "",
                                        objParametri, joinRicette:=True, joinDescrizioneImpianto:=True)

            If dtAgenda.Rows.Count > 0 OrElse dtRicette.Rows.Count > 0 Then
                Dim objProgettoBIZ As New AgronicaCoreAnagrafeBIZ.Progetto_W
                Dim descrizioneImpianto = objProgettoBIZ.getCurrentDescrizione(If(dtAgenda.Rows.Count > 0, dtAgenda.Rows(0), dtRicette.Rows(0)),
                                                                               "",
                                                                               If(dtAgenda.Rows.Count > 0, dtAgenda.Rows(0).Item("Validita_Inizio_Impianto"), dtRicette.Rows(0).Item("Validita_Inizio_Impianto")),
                                                                               If(dtAgenda.Rows.Count > 0, dtAgenda.Rows(0).Item("Validita_Fine_Impianto"), dtRicette.Rows(0).Item("Validita_Fine_Impianto")))

                MessaggioErrore &= descrizioneImpianto & My.Resources.AgronicaCoreAnagrafeBIZ.ImpossibileModificareDataImpiantoRegistrazioniAssociate

                Throw New GiasException(MessaggioErrore)
            End If



            dtAgenda.Dispose()
            dtAgenda = Nothing

            'Dim objPUALog As New AgronicaCorePUA_DAL.AgronicaLogPua_W

            ''DRUDI 2019-10-22 Cancellazione PUA_LetamazioniPrecedenti e Anagrafe_VincoliAgronomici
            'Dim objAnagrafe_VincoliAgronomici_R As New AgronicaCoreAnagrafeDAL.Anagrafe_VincoliAgronomici_R
            'Dim objAnagrafe_VincoliAgronomici_W As New AgronicaCoreAnagrafeDAL.Anagrafe_VincoliAgronomici_W
            'Dim dtAnagrafe_Vincoli = objAnagrafe_VincoliAgronomici_R.Leggi(0, piva, sa_cod, appezza, id_reg,
            '                                                               0, 0, 0, 0, "", "", objParametri)

            'For Each vincolo In dtAnagrafe_Vincoli.Rows
            '    If vincolo("Progetto_Cod") = 0 Then
            '        objAnagrafe_VincoliAgronomici_W.CancellaById(vincolo("ID"), "", objParametri)
            '        objPUALog.Scrivi(enum_TipoOperazioneDB.Cancellazione, "Anagrafe_VincoliAgronomici", vincolo("pua_Cod"), vincolo("regolamento_cod"), vincolo("id"), vincolo("piva"), vincolo("sa_cod"), vincolo("appezza"), vincolo("id_reg"), vincolo("progetto_cod"), Nothing, Nothing, "", objParametri)
            '    End If
            'Next


            'Dim objPUA_LetamazioniPrecedenti_R As New AgronicaCorePUA_DAL.PUA_LetamazioniPrecedenti_R
            'Dim objPUA_LetamazioniPrecedenti_W As New AgronicaCorePUA_DAL.PUA_LetamazioniPrecedenti_W

            'Dim dtPUA_Letamazioni = objPUA_LetamazioniPrecedenti_R.Leggi(0, 0,
            '                                                             piva, sa_cod, appezza, id_reg,
            '                                                             0, "", "", objParametri)

            'For Each pualet In dtPUA_Letamazioni.Rows
            '    If pualet("Progetto_Cod") = 0 Then
            '        objPUA_LetamazioniPrecedenti_W.CancellaByID(pualet("ID"), "", objParametri)
            '        objPUALog.Scrivi(enum_TipoOperazioneDB.Cancellazione, "PUA_LetamazioniPrecedenti", pualet("pua_Cod"), pualet("regolamento_cod"), pualet("id"), pualet("piva"), pualet("sa_cod"), pualet("appezza"), pualet("id_reg"), pualet("progetto_cod"), pualet("eff_cod"), pualet("id_fre"), "", objParametri)
            '    End If
            'Next

        Catch ex As GiasException
            MessaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return MessaggioErrore
    End Function

    Public Function controllo_MovimentiRicettexEliminazione(DatiImpianto As AgronicaCoreModelsSTD.anagrafiche.Impianto,
                                                            piva As String,
                                                            sa_cod As Integer,
                                                            appezza As Integer,
                                                            id_reg As Integer,
                                                            ByRef objParametri As AgronicaCoreParametri)

        Dim MessaggioErrore As String = ""
        Dim nomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Reg_Impianto.controllo_MovimentiRicettexEliminazione()"
        Dim dtAgenda As New DataTable
        Dim dtRicette As New DataTable

        Dim objDestR As New AgronicaCoreContabDAL.Mov_Destinazioni_R
        Dim ObjRicette As New AgronicaCoreContabDAL.Ricette_Destinazioni_R

        Try
            dtAgenda = objDestR.LeggiCronologiaMovimenti(piva, sa_cod, appezza, id_reg,
                                                     enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                     "",
                                                     "",
                                                     objParametri)

            dtRicette = ObjRicette.Leggi(0, 0, 0, 0, 0,
                                        piva, sa_cod, appezza, id_reg,
                                        AGRODATAINIZIO, AGRODATAFINE,
                                        enumSelezioneVariabile.Selezione_TabellaCompleta,
                                        "", "",
                                        objParametri)

            If dtAgenda.Rows.Count > 0 OrElse dtRicette.Rows.Count > 0 Then
                Return True
            End If

            Return False

            dtAgenda.Dispose()
            dtAgenda = Nothing

        Catch ex As GiasException
            MessaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return MessaggioErrore
    End Function

    Public Function controllo_CancellazioneVincoliLetamazioniPUAxEliminazione(DatiImpianto As AgronicaCoreModelsSTD.anagrafiche.Impianto,
                                                               piva As String,
                                                               sa_cod As Integer,
                                                               appezza As Integer,
                                                               id_reg As Integer,
                                                               ByRef objParametri As AgronicaCoreParametri)

        Dim MessaggioErrore As String = ""
        Dim nomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Reg_Impianto.controllo_CancellazioneVincoliLetamazioniPUAxEliminazione()"

        Try

            Dim objPUALog As New AgronicaCorePUA_DAL.AgronicaLogPua_W

            'DRUDI 2019-10-22 Cancellazione PUA_LetamazioniPrecedenti e Anagrafe_VincoliAgronomici
            Dim objAnagrafe_VincoliAgronomici_R As New AgronicaCoreAnagrafeDAL.Anagrafe_VincoliAgronomici_R
            Dim objAnagrafe_VincoliAgronomici_W As New AgronicaCoreAnagrafeDAL.Anagrafe_VincoliAgronomici_W
            Dim dtAnagrafe_Vincoli = objAnagrafe_VincoliAgronomici_R.Leggi(0, piva, sa_cod, appezza, id_reg,
                                                                           0, 0, 0, 0, "", "", objParametri)

            For Each vincolo In dtAnagrafe_Vincoli.Rows
                If vincolo("Progetto_Cod") = 0 Then
                    objAnagrafe_VincoliAgronomici_W.CancellaById(vincolo("ID"), "", objParametri)
                    objPUALog.Scrivi(enum_TipoOperazioneDB.Cancellazione, "Anagrafe_VincoliAgronomici", vincolo("pua_Cod"), vincolo("regolamento_cod"), vincolo("id"), vincolo("piva"), vincolo("sa_cod"), vincolo("appezza"), vincolo("id_reg"), vincolo("progetto_cod"), Nothing, Nothing, "", objParametri)
                End If
            Next


            Dim objPUA_LetamazioniPrecedenti_R As New AgronicaCorePUA_DAL.PUA_LetamazioniPrecedenti_R
            Dim objPUA_LetamazioniPrecedenti_W As New AgronicaCorePUA_DAL.PUA_LetamazioniPrecedenti_W

            Dim dtPUA_Letamazioni = objPUA_LetamazioniPrecedenti_R.Leggi(0, 0,
                                                                         piva, sa_cod, appezza, id_reg,
                                                                         0, "", "", objParametri)

            For Each pualet In dtPUA_Letamazioni.Rows
                If pualet("Progetto_Cod") = 0 Then
                    objPUA_LetamazioniPrecedenti_W.CancellaByID(pualet("ID"), "", objParametri)
                    objPUALog.Scrivi(enum_TipoOperazioneDB.Cancellazione, "PUA_LetamazioniPrecedenti", pualet("pua_Cod"), pualet("regolamento_cod"), pualet("id"), pualet("piva"), pualet("sa_cod"), pualet("appezza"), pualet("id_reg"), pualet("progetto_cod"), pualet("eff_cod"), pualet("id_fre"), "", objParametri)
                End If
            Next

        Catch ex As GiasException
            MessaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return MessaggioErrore
    End Function

    Public Sub ClosePlant(piva As String,
                          saCod As Integer,
                          appezza As Integer,
                          idReg As Integer,
                          progettoCod As Integer,
                          distintaChiusa As Boolean,
                          distintaReplica As Boolean,
                          objParametriServer As AgronicaCoreParametri,
                          objParametriUtente As AgronicaCoreParametri)

        Dim routineName As String = "AgronicaCoreAnagrafeBIZ.Reg_Impianto.ClosePlant()"

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametriServer.StringaConnessione)
        Dim giasContext As New Gias_DeveloperServer_Entities(EFConnString)

        Using scope As New TransactionScope()
            Try
                Dim objAppezza As New AgronicaCoreAnagrafeBIZ.Appezzamento_R
                Dim objReplica As New AgronicaCoreAnagrafeBIZ.Replica_GIAS
                Dim objImp_cod_Dal As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_W

                Dim campoCod = objAppezza.ReadAppezzamentoCampoCod(piva, saCod, appezza, objParametriServer)

                If distintaChiusa Then
                    objImp_cod_Dal.ScriviModificaEliminaxProgetto(
                        piva,
                        saCod,
                        appezza,
                        idReg,
                        progettoCod,
                        enum_CodiciAnagrafe.Distinta_Chiusa,
                        If(distintaChiusa, "1", "0"),
                        True,
                        objParametriServer,
                        giasContext
                        )
                End If

                If distintaChiusa OrElse distintaReplica Then
                    Dim errReplica = objReplica.Replica_Impianto(piva, saCod, appezza, idReg, campoCod, progettoCod, objParametriServer, objParametriUtente)
                    If Not String.IsNullOrEmpty(errReplica) Then
                        Throw New Exception(errReplica)
                    End If
                End If

                giasContext.SaveChanges()

                scope.Complete()
                scope.Dispose()

            Catch ex As Exception
                Throw New Exception("[" & routineName & "] : " & ex.Message)
            Finally
                giasContext.Dispose()
            End Try
        End Using
    End Sub

    Public Function controllo_UMA_RichiestexEliminazione(piva As String,
                                                     sa_cod As Integer,
                                                     appezza As Integer,
                                                     id_reg As Integer,
                                                     ByRef objParametri As AgronicaCoreParametri)

        Dim MessaggioErrore As String = ""
        Dim nomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Reg_Impianto.controllo_UMA_RichiestexEliminazione()"
        Dim dtRichieste As New DataTable

        Dim objUMA As New AgronicaCoreUmaDal.UMA_RichiesteXReg_Impianti_R

        Try
            dtRichieste = objUMA.Leggi_Richieste_x_Impianto_xControlliAnagrafica(piva, sa_cod, appezza, id_reg, objParametri)


            If dtRichieste.Rows.Count > 0 Then
                Return True
            End If

            Return False

            dtRichieste.Dispose()
            dtRichieste = Nothing

        Catch ex As GiasException
            MessaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return MessaggioErrore
    End Function

#Region "Controlli Massivi per Modifica Multipla Piano Colturale"
    Public Function controllo_MovimentiRicettePua_Massivo(listChiavi As List(Of (String, Integer, Integer, Integer)),
                                                          profonditaJoin As Enum_EntitaModificaMultiplaPianoColturale,
                                                          Validita_Inizio As Date,
                                                          Validita_Fine As Date,
                                                          ByRef objParametri As AgronicaCoreParametri
                                                          ) As List(Of (String, Integer, Integer, Integer, Integer))

        Dim MessaggioErrore As String = ""
        Dim nomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Reg_Impianto.controllo_MovimentiRicettePua_Massivo()"
        Dim dtAgenda As New DataTable
        Dim dtRicette As New DataTable

        Dim objDestR As New AgronicaCoreContabDAL.Mov_Destinazioni_R
        Dim ObjRicette As New AgronicaCoreContabDAL.Ricette_Destinazioni_R

        Dim listaBloccati As New List(Of (String, Integer, Integer, Integer, Integer))

        Try
            dtAgenda = objDestR.LeggiCronologiaMovimenti_Massivo(listChiavi,
                                                                 profonditaJoin,
                                                                 " ( Data_Movimento > " & Agro_SQL_SaveDate(Validita_Fine, False) & " OR Data_Movimento < " & Agro_SQL_SaveDate(Validita_Inizio, False) & " ) ",
                                                                 objParametri)

            dtRicette = ObjRicette.LeggiCronologiaRicette_Massivo(listChiavi,
                                                                  profonditaJoin,
                                                                  " ( Ricette_Destinazioni.Validita_inizio > " & Agro_SQL_SaveDate(Validita_Fine, False) & " OR Ricette_Destinazioni.Validita_fine < " & Agro_SQL_SaveDate(Validita_Inizio, False) & " ) ",
                                                                  objParametri)

            If dtAgenda.Rows.Count > 0 Then
                For Each row In dtAgenda.Rows
                    Dim piva As String = row("piva")
                    Dim sa_cod As String = row("sa_cod")
                    Dim appezza As String = row("appezza")
                    Dim id_reg As String = row("id_destinazione")

                    listaBloccati.Add((piva, sa_cod, appezza, id_reg, 0))
                Next
            End If

            If dtRicette.Rows.Count > 0 Then
                For Each row In dtAgenda.Rows
                    Dim piva As String = row("piva")
                    Dim sa_cod As String = row("sa_cod")
                    Dim appezza As String = row("appezza")
                    Dim id_reg As String = row("id_Reg")

                    listaBloccati.Add((piva, sa_cod, appezza, id_reg, 0))
                Next
            End If

            If listaBloccati.Count > 0 Then
                listaBloccati = listaBloccati.Distinct().ToList()
            End If

            dtAgenda.Dispose()
            dtAgenda = Nothing
            dtRicette.Dispose()
            dtRicette = Nothing


        Catch ex As GiasException
            MessaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return listaBloccati
    End Function

#End Region
#Region "Subs di Verifica"
    Public Sub Verifica_ControlliImpianto(ByRef impianto As AgronicaCoreModelsSTD.anagrafiche.Impianto,
                                       ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                       ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                       ByRef GiasContext As Gias_DeveloperServer_Entities,
                                       ByRef NewTransaction As Boolean,
                                       Optional ByRef isFromOperazioneAgenda As Boolean = False)

        Verifica_ValiditaInizioFine(impianto, objParametri_Server, objParametri_Utenti)
        Verifica_Specie(impianto, objParametri_Server)
        Verifica_Superficie(impianto, objParametri_Server, GiasContext, NewTransaction)
        If Not isFromOperazioneAgenda Then
            If (impianto.utilizzoTerreno IsNot Nothing) Then
                If (impianto.utilizzoTerreno.classType = ClassType.Varieta) Then
                    Verifica_Varieta(impianto, objParametri_Server)
                    Verifica_Utilizzo(impianto, objParametri_Server)
                    Verifica_Finalita(impianto, objParametri_Server)
                ElseIf (impianto.utilizzoTerreno.classType = ClassType.DestinazioneUso) Then
                    Verifica_DestinazioneUso(impianto, objParametri_Server)
                End If
            End If
        End If

    End Sub

    Public Sub Verifica_Utilizzo(ByRef impianto As AgronicaCoreModelsSTD.anagrafiche.Impianto,
                                 ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim MessaggioErrore As String = ""
        Dim nomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Reg_Impianto.Verifica_Utilizzo()"

        Dim sa_cod = impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice
        Dim piva = impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva
        Dim appezza = impianto.primaryKey.appezzamentoPK.codice
        Dim id_Reg = impianto.primaryKey.codice

        If id_Reg <> 0 Then
            'Faccio questo controllo solo su impianti già esistenti

            Dim Veg_Cod As Integer = 0
            Dim id_cod As Integer = 0
            Select Case impianto.utilizzoTerreno.classType
                Case costanti.ClassType.Varieta
                    Dim objCultivar As New AgronicaCoreMetaSchemaDAL.Cultivar_R
                    Veg_Cod = objCultivar.VegCod_from_CulCod(impianto.utilizzoTerreno.codice, objParametri_Server)
                Case costanti.ClassType.DestinazioneUso
                    id_cod = impianto.utilizzoTerreno.codice
            End Select

            Dim objImpiantoR As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
            Dim imp = objImpiantoR.Leggi(piva, sa_cod, appezza, id_Reg, enumSelezioneVariabile.Selezione_JoinDescrizioni, "", "", objParametri_Server)

            Dim ObjAgenda As New AgronicaCoreContabDAL.Mov_Destinazioni_R
            Dim DTAgenda = ObjAgenda.LeggiCronologiaMovimenti(piva,
                                                            sa_cod,
                                                            appezza,
                                                            id_Reg,
                                                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                            "",
                                                            "",
                                                            objParametri_Server)

            Try
                If imp.Rows.Count > 0 Then
                    If Veg_Cod <> imp.Rows(0)("Veg_Cod") OrElse id_cod <> imp.Rows(0)("id_cod") Then
                        If DTAgenda.Rows.Count > 0 Then
                            'Se esistono delle Agende collegate al mio impianto, controllo che non sia coinvolto con altri impianti
                            Dim piuApp As Boolean = False
                            Dim objMovDes As New AgronicaCoreContabDAL.Mov_Destinazioni_R
                            For ag As Integer = 0 To DTAgenda.Rows.Count - 1
                                Dim id_agenda As Integer = DTAgenda.Rows(ag).Item("Id_Agenda")
                                Dim DT_dest As DataTable = objMovDes.Leggi_Dettagli_Impianti(piva, id_agenda, "", "", objParametri_Server)
                                For Each dest As DataRow In DT_dest.Rows
                                    'Se trovo appezzamenti diversi, do errore
                                    If CInt(dest.Item("appezza")) <> appezza Then
                                        piuApp = True
                                        Exit For
                                    End If
                                Next
                                If piuApp Then
                                    Exit For
                                End If
                            Next
                            If piuApp Then
                                MessaggioErrore += (Gias.AttenzioneImpossibileVariareSpeciePercheAssociatiMovimentiAgendaCoinvolgentiAltriAppezzamenti)
                                Throw New GiasException(MessaggioErrore)
                            End If
                        End If
                    End If
                End If
            Catch ex As GiasException
                MessaggioErrore = ex.Message
                Throw ex
            Catch ex As Exception
                MessaggioErrore = ex.Message
                Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
            End Try
        End If

    End Sub

    Private Sub Verifica_Finalita(ByRef impianto As AgronicaCoreModelsSTD.anagrafiche.Impianto,
                                    ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim MessaggioErrore As String
        Try
            'Superficie nulla ...
            If impianto.gruppoFinalita.codice = 0 Then
                MessaggioErrore = My.Resources.AgronicaCoreAnagrafeBIZ.CampoFinalitaDaSelezionare
                Throw New GiasException(MessaggioErrore)
            End If

        Catch ex As GiasException
            MessaggioErrore = ex.Message
            Throw ex
        End Try

    End Sub

    Private Sub Verifica_Varieta(ByRef impianto As AgronicaCoreModelsSTD.anagrafiche.Impianto,
                                    ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Try
            If (impianto.utilizzoTerreno.classType = ClassType.Varieta) Then
                If impianto.utilizzoTerreno.codice = 0 Then
                    Dim MessaggioErrore = My.Resources.AgronicaCoreAnagrafeBIZ.CampoVarietaDaSelezionare
                    Throw New GiasException(MessaggioErrore)
                End If
            End If

        Catch ex As GiasException
            Throw ex
        End Try

    End Sub

    Private Sub Verifica_DestinazioneUso(ByRef impianto As AgronicaCoreModelsSTD.anagrafiche.Impianto,
                                    ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Try
            If (impianto.utilizzoTerreno.classType = ClassType.DestinazioneUso) Then
                If impianto.utilizzoTerreno.codice = 0 Then
                    Dim MessaggioErrore = My.Resources.AgronicaCoreAnagrafeBIZ.CampoDestinazioneDUsoDaSelezionare
                    Throw New GiasException(MessaggioErrore)
                End If
            End If

        Catch ex As GiasException
            Throw ex
        End Try

    End Sub

    Private Sub Verifica_Specie(ByRef impianto As AgronicaCoreModelsSTD.anagrafiche.Impianto,
                                    ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim MessaggioErrore As String
        Try
            'lavez - 18/09/2024 il controllo sulla specie vegetale è valido solo se il ClassType è Varietà (su Destinazione d'uso crasha)
            If (impianto.utilizzoTerreno.classType = ClassType.Varieta) Then
                If CType(impianto.utilizzoTerreno, Object).specie.codice = 0 Then
                    MessaggioErrore = My.Resources.AgronicaCoreAnagrafeBIZ.CampoSpecieDaSelezionare
                    Throw New GiasException(MessaggioErrore)
                End If
            End If

        Catch ex As GiasException
            MessaggioErrore = ex.Message
            Throw ex
        End Try

    End Sub

    Private Sub Verifica_Superficie(ByRef impianto As AgronicaCoreModelsSTD.anagrafiche.Impianto,
                                   ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                   ByRef GiasContext As Gias_DeveloperServer_Entities,
                                   ByRef NewTransaction As Boolean)

        Dim MessaggioErrore As String = ""

        Dim Validita_Fine = impianto.validita.fine
        Dim Validita_Inizio = impianto.validita.inizio

        Dim sa_cod = impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice
        Dim piva = impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva
        Dim appezza = impianto.primaryKey.appezzamentoPK.codice
        Dim id_Reg = impianto.primaryKey.codice
        Dim sup_imp = impianto.superficie

        Try
            If sup_imp <= 0 Then
                MessaggioErrore = My.Resources.AgronicaCoreAnagrafeBIZ.LaSUPERFICIENonPuoEssereNulla
                Throw New GiasException(MessaggioErrore)
            End If

            If id_Reg <> 0 Then
                'Faccio qieto controllo solo su impianti già esistenti
                Dim objImpiantoR As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                Dim imp = objImpiantoR.Leggi(piva, sa_cod, appezza, id_Reg, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

                Dim ObjAgenda As New AgronicaCoreContabDAL.Mov_Destinazioni_R
                Dim DTAgenda = ObjAgenda.LeggiCronologiaMovimenti(piva,
                                                                  sa_cod,
                                                                  appezza,
                                                                  id_Reg,
                                                                  enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                  "",
                                                                  "",
                                                                  objParametri_Server)

                If imp.Rows.Count > 0 Then
                    'Lavez - 23/06/2025 - aggiungo anche il flagCessata ="1" per marcare le operazioni di agenda come bloccate
                    'If sup_imp <> imp.Rows(0).Item("sup_imp") And DTAgenda.Rows.Count > 0 Then
                    If ((sup_imp <> imp.Rows(0).Item("sup_imp")) OrElse (impianto.flagCessata IsNot Nothing AndAlso impianto.flagCessata = "1")) AndAlso DTAgenda.Rows.Count > 0 Then

                        Dim id_agendas = (From a In GiasContext.Mov_Destinazioni
                                          Where a.Piva = piva AndAlso
                                            a.Sa_Cod = sa_cod AndAlso
                                            a.Appezza = appezza AndAlso
                                            a.Id_Destinazione = id_Reg AndAlso
                                            a.Tipo_Destinazione = 0
                                          Select a.Id_Agenda).Distinct.ToList

                        For Each id_agenda In id_agendas
                            Dim agenda = (From a In GiasContext.Agenda Where a.Id_Agenda = id_agenda).FirstOrDefault

                            If agenda IsNot Nothing Then
                                agenda.Blocco_Flag = 2
                                agenda.Blocco_Username = objParametri_Server.UtenteUsername
                                agenda.Blocco_Data = DateTime.Now
                                GiasContext.Entry(agenda).State = EntityState.Modified
                            End If

                        Next
                        GiasContext.SaveChanges()
                    End If
                End If
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message.ToString()
            Throw New Exception(MessaggioErrore)
        End Try

    End Sub

    Private Sub Verifica_ValiditaInizioFine(ByRef impianto As AgronicaCoreModelsSTD.anagrafiche.Impianto,
                                            ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                            ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim MessaggioErrore As String = ""
        Dim nomeRoutine As String = "Reg_Impianto.vb\Verifica_ValiditaInizioFine()"
        Dim Validita_Fine = impianto.validita.fine
        Dim Validita_Inizio = impianto.validita.inizio

        Dim sa_cod = impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice
        Dim piva = impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva
        Dim appezza = impianto.primaryKey.appezzamentoPK.codice
        Dim id_Reg = impianto.primaryKey.codice
        Dim sup_imp = impianto.superficie

        Dim objAppezzamentoR As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
        Dim appezzamento = objAppezzamentoR.Leggi(piva, sa_cod, appezza, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

        Dim objImpiantoR As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        Dim imp = objImpiantoR.LeggiImpianto_ControlliAnagrafica(piva, sa_cod, appezza, id_Reg, objParametri_Server)

        Dim objEsercizioR As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
        Dim ese = objEsercizioR.LeggiDistinta_ControlliAnagrafica(piva, sa_cod, appezza, id_Reg, 0, objParametri_Server)

        Dim nomeCurrent As String = ""
        If imp.Rows.Count = 1 Then
            Dim objProgettoBIZ As New AgronicaCoreAnagrafeBIZ.Progetto_W
            nomeCurrent = objProgettoBIZ.getCurrentDescrizione(imp.Rows(0), "", imp.Rows(0).Item("Validita_Inizio"), imp.Rows(0).Item("Validita_Fine"))
        End If

        Try

            If Validita_Fine < Validita_Inizio Then
                MessaggioErrore &= nomeCurrent & My.Resources.AgronicaCoreAnagrafeBIZ.DataFineImpiantoSuperioreDataInizioImpianto
                Throw New GiasException(MessaggioErrore)
            End If

            If imp.Rows.Count > 0 Then

                'Se l'intervallo delle validità viene modificato, controllo e aggiorno, se necessario, l'Esercizio
                If Validita_Inizio <> imp.Rows(0)("Validita_Inizio") Or Validita_Fine <> imp.Rows(0)("Validita_Fine") Then
                    If Validita_Inizio < CDate(appezzamento.Rows(0)("Validita_Inizio")) Then
                        MessaggioErrore &= nomeCurrent & My.Resources.AgronicaCoreAnagrafeBIZ.DataInizioImpiantoInferioreDataInizioAppezzamento_
                        Throw New GiasException(MessaggioErrore)
                    End If

                    If Validita_Fine > CDate(appezzamento.Rows(0)("Validita_Fine")) Then
                        MessaggioErrore &= nomeCurrent & My.Resources.AgronicaCoreAnagrafeBIZ.DataFineImpiantoSuperioreDataFineAppezzamento_
                        Throw New GiasException(MessaggioErrore)
                    End If
                End If
            End If

        Catch ex As GiasException
            MessaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

    End Sub
#End Region

    Public Sub GeneraStaticMapDaSincroAPP(ByRef dtImpianti As DataTable, StaticMapCFG As GeneraMappaStaticaInData, ByRef objParametri_Server As AgronicaCoreParametri)

        Try
            'Troviamo tutti gli impianti che non hanno la StaticMap e la generiamo
            'da dtImpianti trova tutte le righe che hanno la colonna coordinate dbNUll o stringa vuota
            Dim rowsWithNoImage = dtImpianti.AsEnumerable().Where(Function(row) (IsDBNull(row("StaticMapBase64String")) OrElse String.IsNullOrEmpty(row("StaticMapBase64String").ToString())) AndAlso Not IsDBNull(row("entita_cod")) AndAlso CInt(row("entita_cod")) <> 0).ToList()
            Dim impiantiDaAggiornare = rowsWithNoImage _
        .Where(Function(row) (Not IsDBNull(row("cartografia")) AndAlso Not String.IsNullOrEmpty(row("cartografia").ToString()))) _
        .GroupBy(Function(row) (CStr(row("piva")), CInt(row("sa_cod")), CInt(row("appezza")))) _
        .ToDictionary(Function(g) g.Key, Function(g) g.First()("entita_cod"))

            If impiantiDaAggiornare.Count > 0 Then
                Dim gestioneStaticMaps As New GoogleStaticMaps
                For Each impianto In impiantiDaAggiornare
                    Dim entitaCod As Integer = impianto.Value
                    StaticMapCFG.EntitaCod = entitaCod
                    StaticMapCFG.objParametri_Server = objParametri_Server
                    Dim bytesMappa As Byte() = Nothing
                    gestioneStaticMaps.AggiornaElementoGraficoConMappaStatica(StaticMapCFG, bytesMappa)

                    'se bytesMappa <> null, allora andiamo a  ' Convertila in Base64
                    If bytesMappa IsNot Nothing AndAlso bytesMappa.Length > 0 Then
                        Dim image As String = Convert.ToBase64String(bytesMappa)
                        'aggiorniamo la colonna nel datatable, in corrispondenza delle chiavi dell'impianto
                        Dim rowsToUpdate = dtImpianti.AsEnumerable().Where(Function(row) (CStr(row("piva")) = impianto.Key.Item1 AndAlso CInt(row("sa_cod")) = impianto.Key.Item2 AndAlso CInt(row("appezza")) = impianto.Key.Item3)).ToList()
                        For Each rowToUpdate In rowsToUpdate
                            rowToUpdate("StaticMapBase64String") = image
                        Next
                    Else
                        Scrivi_LOG(objParametri_Server, "Reg_Impianto.vb\GeneraStaticMapDaSincroAPP()", $"Anomalia nella generazione della staticMap per Entita_Cod {entitaCod}", CustomLOGParams:=New CustomLOGParams() With {.LogDirectory = objParametri_Server.LogDirectory, .LogDescrizioneUtente = objParametri_Server.LogDescrizioneUtente, .LogFileName = $"ReadByteArrayFromUrlGet_{Guid.NewGuid()}.txt"})
                    End If
                Next
            End If

        Catch ex As Exception
            'TryCatch silenzioso, l'errore nella generazione della mappa non deve alterare il funzionamento della sincro web2app
            Scrivi_LOG(objParametri_Server, "Reg_Impianto.vb\GeneraStaticMapDaSincroAPP()", ex.Message, , CustomLOGParams:=New CustomLOGParams() With {.LogDirectory = objParametri_Server.LogDirectory, .LogDescrizioneUtente = objParametri_Server.LogDescrizioneUtente, .LogFileName = $"ReadByteArrayFromUrlGet_{Guid.NewGuid()}.txt"})
        End Try
    End Sub
End Class
