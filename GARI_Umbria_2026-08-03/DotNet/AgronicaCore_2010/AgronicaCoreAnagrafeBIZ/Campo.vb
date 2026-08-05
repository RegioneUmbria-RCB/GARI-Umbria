Imports System.Data.Common
Imports System.Linq
Imports System.Transactions
Imports System.Xml
Imports System.Xml.Linq
Imports Newtonsoft.Json
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreVarieBIZ
Imports System.Text
Imports AgronicaCoreModelsSTD.exceptions

Public Class Campo_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi_Campi_APP(piva As String, data As Date, ByRef objParametri_Server As AgronicaCoreParametri, Optional ByVal leggiSoloAttivi As Boolean = True) As List(Of AgronicaCoreModelsSTD.anagrafiche.Campo)

        Dim objCampi As New Campi_R

        Dim xFiltroAggiuntivo As New StringBuilder
        If leggiSoloAttivi Then
            xFiltroAggiuntivo.AppendLine(" ( Validita_inizio <= " & Agro_SQL_SaveDate(data) & " ")
            xFiltroAggiuntivo.AppendLine(" AND Validita_Fine >= " & Agro_SQL_SaveDate(data) & " ) ")
        Else
            xFiltroAggiuntivo.AppendLine(" Validita_Fine >= " & Agro_SQL_SaveDate(data) & " ")
        End If
        Dim filtro = xFiltroAggiuntivo.ToString

        Dim campi As New List(Of AgronicaCoreModelsSTD.anagrafiche.Campo)
        Dim dtCampi = objCampi.Leggi(piva, 0, 0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, filtro, "", objParametri_Server)

        For Each row In dtCampi.Rows

            Dim campo = New AgronicaCoreModelsSTD.anagrafiche.Campo() With {
                .primaryKey = New AgronicaCoreModelsSTD.anagrafiche.Campo.PK() With {
                    .centroAziendalePK = New CentroAziendale.PK(row("sa_cod"), row("piva")), .codice = row("campo_cod")
                },
                .descrizione = row("campo_des")
            }

            campi.Add(campo)

        Next

        Return campi

    End Function


    Public Function Campo_Leggi(ByVal PIVA As String,
                                ByVal Sa_Cod As Long,
                                ByVal Campo_Cod As Long,
                                ByVal Validita_Inizio As Date,
                                ByVal Validita_Fine As Date,
                                ByVal ForDelete As Boolean,
                                ByVal AllAttributes As Boolean,
                                ByRef objParametri As AgronicaCoreParametri,
                                Optional ByVal TipoG2G As Integer = 0
                                ) As String

        Const nomeRoutine = "AnagrafeBIZ.Campo_R.Campo_Leggi()"

        Dim MessaggioErrore As String = ""
        Dim FlagConnessioneLocale As Boolean = False
        Dim xConnessione As DbConnection = Nothing
        Dim xConnectionState As ConnectionState = ConnectionState.Closed

        Dim xTransazione As DbTransaction
        Dim RisultatoFunzione As String = String.Empty


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

            '------------------------------
            'Mi procuro un elenco dei Campi associati alla Struttura Aziendale
            'all'interno della finestra temporale selezionata

            Dim ObjUtentixCampi As New AgronicaCoreAnagrafeDAL.UtentixCampi_Read
            Dim DtCampi As DataTable

            'Mi procuro il recordset richiesto
            DtCampi = ObjUtentixCampi.Leggi(CStr(PIVA),
                                            CLng(Sa_Cod),
                                            CLng(Campo_Cod),
                                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                                            "",
                                            "",
                                            objParametri,
                                            TipoG2G)


            If DtCampi.Rows.Count <> 0 Then

                Dim XmlDoc As New XmlDocument
                Dim XmlDatiCampi As XmlElement
                Dim XmlCampo As XmlElement
                Dim XmlCodice As XmlElement
                Dim XmlPart As XmlElement
                Dim XmlDatiCampixParticelle As XmlElement

                XmlDatiCampi = XmlDoc.CreateElement("DatiCampi")

                Dim iCam As Integer
                For iCam = 0 To DtCampi.Rows.Count - 1

                    '----- < CAMPO > -----
                    XmlCampo = XmlDoc.CreateElement("Campo")

                    With XmlCampo
                        .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                        .SetAttribute("piva", Agro_SQL_Load(DtCampi.Rows(iCam).Item("PIVA")))
                        .SetAttribute("sa_cod", Agro_SQL_Load(DtCampi.Rows(iCam).Item("Sa_Cod")))
                        .SetAttribute("campo_cod", Agro_SQL_Load(DtCampi.Rows(iCam).Item("Campo_Cod")))
                        .SetAttribute("campo_des", Agro_SQL_Load(DtCampi.Rows(iCam).Item("Campo_Des")))
                        .SetAttribute("sau_totale", Agro_SQL_Load(DtCampi.Rows(iCam).Item("Sau_Totale")))
                        .SetAttribute("gru_cod", Agro_SQL_Load(DtCampi.Rows(iCam).Item("Gru_Cod")))
                        .SetAttribute("veg_cod", Agro_SQL_Load(DtCampi.Rows(iCam).Item("Veg_Cod")))
                        .SetAttribute("campo_tipo", Agro_SQL_Load(DtCampi.Rows(iCam).Item("Campo_Tipo")))
                        .SetAttribute("inviato", Agro_SQL_Load(DtCampi.Rows(iCam).Item("Inviato")))
                        .SetAttribute("datainvio", Agro_SQL_Load(DtCampi.Rows(iCam).Item("DataInvio")))
                        .SetAttribute("data_creazione", Agro_SQL_Load(DtCampi.Rows(iCam).Item("Data_Creazione")))
                        .SetAttribute("data_modifica", Agro_SQL_Load(DtCampi.Rows(iCam).Item("Data_Modifica")))
                        .SetAttribute("username_creazione", Agro_SQL_Load(DtCampi.Rows(iCam).Item("Username_Creazione")))
                        .SetAttribute("username_modifica", Agro_SQL_Load(DtCampi.Rows(iCam).Item("Username_Modifica")))
                        .SetAttribute("validita_inizio", Agro_SQL_Load(DtCampi.Rows(iCam).Item("Validita_Inizio")))
                        .SetAttribute("validita_fine", Agro_SQL_Load(DtCampi.Rows(iCam).Item("Validita_Fine")))
                    End With


                    If AllAttributes Then

                        With XmlCampo
                            .SetAttribute("sau_biologico", Agro_SQL_Load(DtCampi.Rows(iCam).Item("Sau_Biologico")))
                            .SetAttribute("sau_conversione", Agro_SQL_Load(DtCampi.Rows(iCam).Item("Sau_Conversione")))
                            .SetAttribute("sau_convenzionale", Agro_SQL_Load(DtCampi.Rows(iCam).Item("Sau_Convenzionale")))
                            .SetAttribute("conversione_inizio", Agro_SQL_Load(DtCampi.Rows(iCam).Item("Conversione_Inizio")))
                            .SetAttribute("conversione_fine", Agro_SQL_Load(DtCampi.Rows(iCam).Item("Conversione_Fine")))
                            .SetAttribute("confinirischio", Agro_SQL_Load(DtCampi.Rows(iCam).Item("ConfiniRischio")))
                        End With

                    End If


                    '#################################
                    '##########  CODICI  #############
                    '#################################

                    Dim ObjCampixCodici As New AgronicaCoreAnagrafeDAL.Campi_Codici_Read
                    Dim DtCampixCodici As DataTable

                    DtCampixCodici = ObjCampixCodici.Leggi(CStr(Agro_SQL_Load(DtCampi.Rows(iCam).Item("PIVA"))),
                                                           CLng(Agro_SQL_Load(DtCampi.Rows(iCam).Item("Sa_Cod"))),
                                                           CLng(Agro_SQL_Load(DtCampi.Rows(iCam).Item("Campo_Cod"))),
                                                           0, "",
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                           "",
                                                           "",
                                                           objParametri)

                    If DtCampixCodici.Rows.Count <> 0 Then

                        Dim iCxC As Integer
                        For iCxC = 0 To DtCampixCodici.Rows.Count - 1

                            '----- < CODICE > -----
                            XmlCodice = XmlDoc.CreateElement("Codice")

                            With XmlCodice
                                .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                                .SetAttribute("id_cod", Agro_SQL_Load(DtCampixCodici.Rows(iCxC).Item("id_cod")))
                                .SetAttribute("val_cod", Agro_SQL_Load(DtCampixCodici.Rows(iCxC).Item("val_cod")))
                                .SetAttribute("descrizione", Agro_SQL_Load(DtCampixCodici.Rows(iCxC).Item("descrizione")))
                                .SetAttribute("validita_inizio", Agro_SQL_Load(DtCampixCodici.Rows(iCxC).Item("validita_inizio")))
                                .SetAttribute("validita_fine", Agro_SQL_Load(DtCampixCodici.Rows(iCxC).Item("validita_fine")))
                            End With

                            XmlCampo.AppendChild(XmlCodice)
                            '----- < / CODICE > -----

                        Next iCxC

                    End If

                    XmlCodice = Nothing
                    DtCampixCodici = Nothing
                    ObjCampixCodici = Nothing


                    '#############################################
                    '##########  CAMPI X PARTICELLE  #############
                    '#############################################

                    Dim ObjCampixPart As New AgronicaCoreAnagrafeDAL.CampixParticelle_R
                    Dim DtCampixPart As DataTable

                    DtCampixPart = ObjCampixPart.Leggi(CStr(Agro_SQL_Load(DtCampi.Rows(iCam).Item("PIVA"))),
                                                       CLng(Agro_SQL_Load(DtCampi.Rows(iCam).Item("Sa_Cod"))),
                                                       CLng(Agro_SQL_Load(DtCampi.Rows(iCam).Item("Campo_Cod"))),
                                                       "",
                                                       "",
                                                       "",
                                                       0,
                                                       0,
                                                       "",
                                                       enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                       "",
                                                       "",
                                                       objParametri)

                    If DtCampixPart.Rows.Count > 0 Then

                        XmlDatiCampixParticelle = XmlDoc.CreateElement("DatiParticelle")

                        For j = 0 To DtCampixPart.Rows.Count - 1

                            XmlPart = XmlDoc.CreateElement("Particella")

                            With XmlPart
                                .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                                .SetAttribute("piva", Agro_SQL_Load(DtCampixPart.Rows(j).Item("Piva")))
                                .SetAttribute("sa_cod", Agro_SQL_Load(DtCampixPart.Rows(j).Item("Sa_Cod")))
                                .SetAttribute("campo_cod", Agro_SQL_Load(DtCampixPart.Rows(j).Item("Campo_Cod")))
                                .SetAttribute("part_cod", Agro_SQL_Load(DtCampixPart.Rows(j).Item("Part_Cod")))
                                .SetAttribute("prov", Agro_SQL_Load(DtCampixPart.Rows(j).Item("Prov")))
                                .SetAttribute("com", Agro_SQL_Load(DtCampixPart.Rows(j).Item("Com")))
                                .SetAttribute("sezione", Agro_SQL_Load(DtCampixPart.Rows(j).Item("Sezione")))
                                .SetAttribute("foglio", Agro_SQL_Load(DtCampixPart.Rows(j).Item("Foglio")))
                                .SetAttribute("numero", Agro_SQL_Load(DtCampixPart.Rows(j).Item("Numero")))
                                .SetAttribute("subalterno", Agro_SQL_Load(DtCampixPart.Rows(j).Item("Subalterno")))
                                .SetAttribute("area", Agro_SQL_Load(DtCampixPart.Rows(j).Item("Area")))
                                .SetAttribute("sau_convenz_ettari", Agro_SQL_Load(DtCampixPart.Rows(j).Item("SAU_Convenz_Ettari")))
                                .SetAttribute("sau_convenz_are", Agro_SQL_Load(DtCampixPart.Rows(j).Item("SAU_Convenz_Are")))
                                .SetAttribute("sau_convenz_centiare", Agro_SQL_Load(DtCampixPart.Rows(j).Item("SAU_Convenz_Centiare")))
                                .SetAttribute("sau_convers_ettari", Agro_SQL_Load(DtCampixPart.Rows(j).Item("SAU_Convers_Ettari")))
                                .SetAttribute("sau_convers_are", Agro_SQL_Load(DtCampixPart.Rows(j).Item("SAU_Convers_Are")))
                                .SetAttribute("sau_convers_centiare", Agro_SQL_Load(DtCampixPart.Rows(j).Item("SAU_Convers_Centiare")))
                                .SetAttribute("sau_bio_ettari", Agro_SQL_Load(DtCampixPart.Rows(j).Item("SAU_Bio_Ettari")))
                                .SetAttribute("sau_bio_are", Agro_SQL_Load(DtCampixPart.Rows(j).Item("SAU_Bio_Are")))
                                .SetAttribute("sau_bio_centiare", Agro_SQL_Load(DtCampixPart.Rows(j).Item("SAU_Bio_Centiare")))
                                .SetAttribute("inviato", Agro_SQL_Load(DtCampixPart.Rows(j).Item("Inviato")))
                                .SetAttribute("datainvio", Agro_SQL_Load(DtCampixPart.Rows(j).Item("DataInvio")))
                                .SetAttribute("data_creazione", Agro_SQL_Load(DtCampixPart.Rows(j).Item("Data_Creazione")))
                                .SetAttribute("data_modifica", Agro_SQL_Load(DtCampixPart.Rows(j).Item("Data_Modifica")))
                                .SetAttribute("username_creazione", Agro_SQL_Load(DtCampixPart.Rows(j).Item("Username_Creazione")))
                                .SetAttribute("username_modifica", Agro_SQL_Load(DtCampixPart.Rows(j).Item("Username_Modifica")))
                                .SetAttribute("validita_inizio", Agro_SQL_Load(DtCampixPart.Rows(j).Item("xValidita_Inizio")))
                                .SetAttribute("validita_fine", Agro_SQL_Load(DtCampixPart.Rows(j).Item("xValidita_Fine")))
                            End With

                            XmlDatiCampixParticelle.AppendChild(XmlPart)

                        Next

                        XmlCampo.AppendChild(XmlDatiCampixParticelle)

                    End If

                    XmlPart = Nothing
                    DtCampixPart = Nothing
                    ObjCampixPart = Nothing


                    XmlDatiCampi.AppendChild(XmlCampo)
                    '----- < / CAMPI > -----


                Next iCam


                XmlDoc.AppendChild(XmlDatiCampi)

                RisultatoFunzione = XmlDoc.OuterXml
                '----- < / Documento XML > -----


                XmlCampo = Nothing
                XmlDatiCampi = Nothing
                XmlDoc = Nothing

            Else

                'Altrimenti, se non risulta selezionato nessun centro aziendale ...
                RisultatoFunzione = ""

            End If

            'Elimino gli oggetti che ho creato
            DtCampi = Nothing
            ObjUtentixCampi = Nothing

            '------------------------------

            'Assegno il risultato
            Campo_Leggi = RisultatoFunzione

        Catch ex As Exception
            RisultatoFunzione = ""
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, MessaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
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

    '#############################################################################
    '################### Funzioni per lettura dati Angular #######################
    '#############################################################################

    Public Function Leggi_Campo(Piva As String,
                                   Sa_Cod As Integer,
                                   Campo_Cod As Integer,
                                   ByRef objParametri As AgronicaCoreParametri) As Campo

        Dim c As New Campo

        Dim objCampi_Dal As New AgronicaCoreAnagrafeDAL.Campi_R
        Dim DT = objCampi_Dal.Leggi(Piva, Sa_Cod, Campo_Cod,
                                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                                    "", "",
                                    objParametri)

        Dim objCodici_Dal As New AgronicaCoreAnagrafeDAL.Campi_Codici_Read
        Dim codici = objCodici_Dal.Leggi(Piva,
                                         Sa_Cod,
                                         Campo_Cod,
                                         0,
                                         "",
                                         AGRODATAINIZIO,
                                         AGRODATAFINE,
                                         enumSelezioneVariabile.Selezione_TabellaCompleta,
                                         "",
                                         "",
                                         objParametri) 'tolto: " id_cod <> " & enum_CodiciAnagrafe.Riferimento_Alfanumerico_Campo & " ",

        Dim objAppezzamentoR As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
        Dim appezzamenti = objAppezzamentoR.Recupera_Appezzamenti_Colture_del_Campo(
                                                         Piva,
                                                         Sa_Cod,
                                                         Campo_Cod,
                                                         AGRODATAINIZIO,
                                                         AGRODATAFINE,
                                                         False,
                                                         enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                         "",
                                                         "",
                                                         objParametri)

        Dim objParticelleCatastali As New AgronicaCoreAnagrafeDAL.CampixParticelle_R
        Dim particelle = objParticelleCatastali.Leggi(Piva,
                                                         CInt(Sa_Cod),
                                                         CInt(Campo_Cod),
                                                         "",
                                                         "",
                                                         "",
                                                         0,
                                                         0,
                                                         "",
                                                         enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                         "",
                                                         "",
                                                         objParametri)

        If DT.Rows.Count = 0 Then
            Throw New Exception("Campo non trovato")
        End If

        c.primaryKey = New Campo.PK()
        c.primaryKey.centroAziendalePK = New CentroAziendale.PK()
        c.primaryKey.codice = DT.Rows(0)("Campo_Cod")
        c.primaryKey.centroAziendalePK.partitaIva = DT.Rows(0)("Piva")
        c.primaryKey.centroAziendalePK.codice = DT.Rows(0)("Sa_Cod")
        c.descrizione = If(IsDBNull(DT.Rows(0)("Campo_Des")), "", DT.Rows(0)("Campo_Des"))
        c.specie = New AgronicaCoreModelsSTD.metaschema.utilizzi.Specie()
        c.serra = If(DT.Rows(0)("Campo_Tipo") = 1, True, False)
        c.specie.codice = DT.Rows(0)("Veg_Cod")
        c.orientamento_Colturale = DT.Rows(0)("Gru_Cod")
        c.appezzamentoCampo = New List(Of AppezzamentoCampo)
        c.catastoCampo = New List(Of CatastoCampo)


        For Each row In appezzamenti.Rows
            Dim appezzamentoCampo As New AppezzamentoCampo()

            appezzamentoCampo.piva = row("PIVA")
            appezzamentoCampo.sa_cod = row("SA_COD")
            appezzamentoCampo.appezza = row("APPEZZA")
            appezzamentoCampo.campo_cod = row("Campo_Cod")
            appezzamentoCampo.sup_app = row("SUP_APP")
            appezzamentoCampo.app_nome = If(IsDBNull(row("APP_NOME")), "", row("APP_NOME"))
            appezzamentoCampo.validita = New IntervalloTemporale(
                If(IsDBNull(DT.Rows(0)("Validita_Inizio")), AGRODATAINIZIO, DT.Rows(0)("Validita_Inizio")),
                If(IsDBNull(DT.Rows(0)("Validita_Fine")), AGRODATAFINE, DT.Rows(0)("Validita_Fine")))
            appezzamentoCampo.id_reg = row("ID_REG")
            appezzamentoCampo.validita_impianto = New IntervalloTemporale(
                If(IsDBNull(DT.Rows(0)("Validita_Inizio")), AGRODATAINIZIO, DT.Rows(0)("Validita_Inizio")),
                If(IsDBNull(DT.Rows(0)("Validita_Fine")), AGRODATAFINE, DT.Rows(0)("Validita_Fine")))
            appezzamentoCampo.cul_cod = If(IsDBNull(row("CUL_COD")), 0, row("CUL_COD"))
            appezzamentoCampo.cul_des = If(IsDBNull(row("Cul_Des")), "", row("Cul_Des"))
            appezzamentoCampo.veg_des = If(IsDBNull(row("Veg_Des")), "", row("Veg_Des"))


            c.appezzamentoCampo.Add(appezzamentoCampo)
        Next

        For Each row In particelle.Rows
            Dim catastoCampo As New CatastoCampo()
            catastoCampo.particella = New ParticelleCatastali()
            catastoCampo.particella.primaryKey = New ParticelleCatastali.PK()
            catastoCampo.particella.primaryKey.Com = row("COM")
            catastoCampo.particella.primaryKey.Foglio = row("FOGLIO")
            catastoCampo.particella.primaryKey.Numero = row("NUMERO")
            catastoCampo.particella.primaryKey.Prov = row("PROV")
            catastoCampo.particella.primaryKey.Sezione = row("SEZIONE")
            catastoCampo.particella.primaryKey.Subalterno = row("SUBALTERNO")

            catastoCampo.particella.Area = row("AREA")
            catastoCampo.area = row("AREA")

            c.catastoCampo.Add(catastoCampo)
        Next

        c.validita = New IntervalloTemporale(
            If(IsDBNull(DT.Rows(0)("Validita_Inizio")), AGRODATAINIZIO, DT.Rows(0)("Validita_Inizio")),
             If(IsDBNull(DT.Rows(0)("Validita_Fine")), AGRODATAFINE, DT.Rows(0)("Validita_Fine")))

        c.codici = New List(Of CodiciAnagrafeValori)

        For Each row In codici.Rows
            If row("id_cod") = enum_CodiciAnagrafe.Riferimento_Alfanumerico_Campo Then
                c.campo_Codice = row("val_cod")
            ElseIf (row("id_cod") < 2000 Or row("id_cod") > 3000) Then
                Dim codiceAnagrafeValori As New CodiciAnagrafeValori()
                codiceAnagrafeValori.validita = New IntervalloTemporale(row("Validita_Inizio"), row("Validita_Fine"))
                codiceAnagrafeValori.valore = row("val_cod")
                codiceAnagrafeValori.codiceAnagrafe = New CodiceAnagrafe(row("id_cod")) With {
                        .descrizione = row("descrizione")
                    }
                c.codici.Add(codiceAnagrafeValori)
            End If
        Next

        Return c
    End Function

    '#############################################################################
    '############### Fine Funzioni per lettura dati Angular ######################
    '#############################################################################

End Class


'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

Public Class Campo_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Private ZeroData As String = "0"
    Private ZeroInt As Integer = 0
    Private ZeroString As String = "0"
    Private ZeroDecimal As Decimal = 0
    Private NullString As String = ""
    Private Null As String = "NULL"
    Private PuntoString As String = "."

    ''' <summary>
    ''' Scrive un nuovo campo con le particelle catastali passate ed associa la lista di appezzamenti al campo appena creato
    ''' </summary>
    ''' <param name="Campo_Des"></param>
    ''' <param name="Entita_Cod"></param>
    ''' <param name="ListaChiaviAppezzamentiPerAssociazioneCampo"></param>
    ''' <param name="ListaParticelleCatastaliPerAssociazioneCampo"></param>
    ''' <returns></returns>
    Public Function Campo_ScriviConAssociazioneDaRiparto(ByVal piva As String,
                                                        ByVal sa_cod As Integer,
                                                        ByVal Campo_Des As String,
                                                        ByVal Entita_Cod As Integer,
                                                        ByVal ListaChiaviAppezzamentiPerAssociazioneCampo As List(Of String),
                                                        ByVal ListaParticelleCatastaliPerAssociazioneCampo As List(Of String),
                                                        ByVal objParametri_server As AgronicaCoreParametri,
                                                        ByVal objParametri_utenti As AgronicaCoreParametri) As RispostaStandard

        Dim rval As New RispostaStandard


        Dim messaggioErrore As String = ""

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False


        'TODO: Gestire la transazione
        Try

            'Lavez - 05/11/2024 - gestione sequence inizializzazione se seq mancante
            If AgronicaCoreDataProvider.Agro_Sequenze.CheckAllowAppSettingsFlagUseSequence() Then
                If Not AgronicaCoreDataProvider.Agro_Sequenze.InizializzaSequence("Gis_Entita", 0, 2000000, objParametri_server) Then
                    Throw New Exception("Errore in inizializzazione sequence per Gis_Entita")
                End If
                If Not AgronicaCoreDataProvider.Agro_Sequenze.InizializzaSequence("Gis_ElementiGrafici", 0, 2000000, objParametri_server) Then
                    Throw New Exception("Errore in inizializzazione sequence per Gis_ElementiGrafici")
                End If
            End If

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Apro la connessione al DB
            ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale,
                                                           objParametri_server)


            'scrittura del nuovo campo

            Dim xmlCampo As String = Campo_ScriviConAssociazioneDaRiparto_xml(Campo_Des, piva, sa_cod, ListaParticelleCatastaliPerAssociazioneCampo, objParametri_server, objParametri_utenti)

            xmlCampo = "<DatiCampi>" & xmlCampo & "</DatiCampi>"

            Dim OUTPUT_Piva As String = Nothing
            Dim OUTPUT_Sa_Cod As Integer = Nothing
            Dim OUTPUT_Campo_Cod As Integer = Nothing

            Dim InseritoCampo As Boolean = Campo_Scrivi(
                xmlCampo,
                OUTPUT_Piva,
                OUTPUT_Sa_Cod,
                OUTPUT_Campo_Cod,
                False,
                objParametri_server,
                objParametri_utenti)

            'catasto associato al campo

            'associazione di appezzamenti al campo
            Dim scriviApp As New AgronicaCoreAnagrafeDAL.Appezzamento_Write
            For Each appezzamento In ListaChiaviAppezzamentiPerAssociazioneCampo
                Dim appezza As Integer =
                    appezzamento.Split("-")(2)
                scriviApp.ModificaSingolo_CampoNumerico(piva, sa_cod, appezza, "Campo_Cod", OUTPUT_Campo_Cod, "", objParametri_server)
            Next


            'copia delle entità grafiche dal planning al campo creato
            Dim entitaW As New AgronicaCoreGisDAL.GIS_Entita_W
            entitaW.RibaltaGraficaPlanningCampo(
                objParametri_server.PivaSuperUser,
                Entita_Cod,
                piva,
                sa_cod,
                OUTPUT_Campo_Cod,
                "",
                "",
                objParametri_server
            )


            rval.RispostaOK = True
            rval.RispostaStringa = "Campo creato correttamente"

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Chiudo la connessione al DB
            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_server)
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        Catch ex As Exception

            'Faccio il rollback della transazione
            If objParametri_server.objTransazione IsNot Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri_server)
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
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_server)
        End Try

        Return rval

    End Function

    Private Sub Campo_ScriviConAssociazioneDaRiparto_PivaSaCod(Str1 As String, ByRef piva As String, ByRef sa_cod As Integer)
        Dim a As String() =
            Str1.Split("-")

        piva = a(0)
        sa_cod = a(1)
    End Sub

    Private Function Campo_ScriviConAssociazioneDaRiparto_xml(campo_des As String,
                                                              piva As String,
                                                              sa_cod As Integer,
                                                              ByVal ListaParticelleCatastaliPerAssociazioneCampo As List(Of String),
                                                              ByRef objParametri As AgronicaCoreParametri,
                                                              ByRef objParametri_Utenti As AgronicaCoreParametri) As String

        Dim objXML As New AgronicaCoreXML.AnagrafeXML
        Dim StrCampo As String = ""

        Dim BaseCode As Integer
        Dim TopCode As Integer


        Dim objCodGias As New AgronicaCoreUtentiDAL.Utenti_CodiciGiasPRO_R
        Dim dtProg = objCodGias.Leggi("", "", objParametri_Utenti)
        Dim progressivo As Integer = dtProg.Rows(0).Item("progressivogias")
        Dim objAgroSeq As New Agro_Sequenze
        objAgroSeq.Calcola_BaseCode(progressivo, TopCode, BaseCode, objParametri_Utenti)


        objXML.XML_Campo(
            enum_CodificaDecodifica.Codifica,
            StrCampo,
            enum_TipoOperazioneDB.Scrittura,
            piva,
            sa_cod,
            ZeroInt,
            ZeroInt,
            campo_des,
            AGRODATAINIZIO,
            AGRODATAINIZIO,
            ZeroDecimal,
            ZeroDecimal,
            ZeroDecimal,
            ZeroDecimal,
            NullString,
            ZeroInt,
            ZeroInt,
            AGRODATAINIZIO,
            AGRODATAFINE,
            BaseCode,
            TopCode)

        Dim xDocCampo As XDocument = XDocument.Parse(StrCampo)

        Dim nodoParticelle = <DatiCampixParticelle></DatiCampixParticelle>

        Dim prov As String = ""
        Dim com As String = ""
        Dim sezione As String = ""
        Dim foglio As String = ""
        Dim numero As String = ""
        Dim subalterno As String = ""
        Dim Area As Decimal

        For Each particella In ListaParticelleCatastaliPerAssociazioneCampo

            Campo_ScriviConAssociazioneDaRiparto_SmontaParticella(particella, prov, com, sezione, foglio, numero, subalterno, Area)

            Dim nodoParticella =
                <CampoxParticella TipoOperazioneDB="1"
                    prov=<%= prov %>
                    com=<%= com %>
                    ezione=<%= sezione %>
                    foglio=<%= foglio %>
                    numero=<%= numero %>
                    subalterno=<%= subalterno %>
                    sau_convenz_ettari="0"
                    sau_convenz_are="0"
                    sau_convenz_centiare="0"
                    sau_convers_ettari="0"
                    sau_convers_are="0"
                    sau_convers_centiare="0"
                    sau_bio_ettari="0"
                    sau_bio_are="0"
                    sau_bio_centiare="0"
                    area="0"
                    validita_inizio="1900-01-01T00:00:00"
                    validita_fine="2100-12-31T00:00:00"
                    ></CampoxParticella>

            nodoParticelle.Add(nodoParticella)

        Next

        Return StrCampo

    End Function

    Private Sub Campo_ScriviConAssociazioneDaRiparto_SmontaParticella(ParticellaConDescrizione As String,
        ByRef Prov As String, ByRef com As String, ByRef sezione As String, ByRef foglio As String, ByRef numero As String, ByRef Subalterno As String, ByRef area As Decimal)

        '	RE (035) - NOVELLARA (028) - 0 - 63 - 94 - 0
        '   035-028-0-63-94-0

        Dim vApp As String() = ParticellaConDescrizione.Split("-")
        Prov = vApp(0)
        com = vApp(1)
        sezione = vApp(2)
        foglio = vApp(3)
        numero = vApp(4)
        Subalterno = vApp(5)

        area = 0

    End Sub

    '============================================================================
    Public Function Campo_Scrivi(ByVal DatiCampo As String,
                                 ByRef OUTPUT_Piva As String,
                                 ByRef OUTPUT_Sa_Cod As Integer,
                                 ByRef OUTPUT_Campo_Cod As Integer,
                                 ByVal Cancello_Solo_Campo As Boolean,
                                 ByRef objParametri As AgronicaCoreParametri,
                                 ByRef objParametri_Utenti As AgronicaCoreParametri,
                                 Optional ByVal TipoG2G As Integer = 0
                                 ) As Boolean

        Const MetodoNome = "AgronicaCoreAnagrafeBIZ : Campo_W . Campo_Scrivi"

        Dim XmlDoc As XmlDocument

        Dim MessaggioErrore As String = ""

        Dim objSequenze As New Agro_Sequenze
        Dim objCampi As New AgronicaCoreAnagrafeDAL.Campi_W
        Dim objUtentixCampi As New AgronicaCoreAnagrafeDAL.UtentixCampi_Write
        Dim objCampixParticelle As New AgronicaCoreAnagrafeDAL.CampixParticelle_W
        '   Dim objDettaglio              As object

        Dim objCampixCodici As New AgronicaCoreAnagrafeDAL.Campi_Codici_Write


        Dim objAppezzamenti_W As New AgronicaCoreAnagrafeDAL.Appezzamento_Write
        Dim objAppezzamenti_R As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
        Dim objUtentixAppezzamenti As New AgronicaCoreAnagrafeDAL.UtentixAppezzamenti_W
        Dim objAppezzaxParticelle As New AgronicaCoreAnagrafeDAL.AppezzaxParticelle_W
        Dim objReg_Impianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Write


        Dim objAppezzamentiLeggi As New AgronicaCoreAnagrafeBIZ.Appezzamento_R
        Dim objAppezzamentiScrivi As New AgronicaCoreAnagrafeBIZ.Appezzamento_W

        Dim objGrafica As New AgronicaCoreGraficaBIZ.Grafica_Write
        Dim objGrafica_AD As New AgronicaCoreGraficaDAL.Grafica_Write


        Dim GraphicKey As String

        Dim XmlAppezzamenti As String

        Dim Dummy As Integer
        Dim Cod_Campo As Integer
        Dim Esito As Boolean

        Dim BaseCode As Integer


        Dim xDatiCampi As XmlNodeList
        Dim xDatiCampo As XmlElement
        Dim xCampi As XmlNodeList
        Dim xCampo As XmlElement

        Dim xCodici As XmlNodeList
        Dim xCodice As XmlElement
        Dim xDatiCampixParticelle As XmlNodeList
        Dim xDatiCampoxParticella As XmlElement
        Dim xCampixParticelle As XmlNodeList
        Dim xCampoxParticella As XmlElement
        Dim xEntitaGrafiche As XmlNodeList

        Dim i_DatiParticella As Integer
        Dim i_DatiParticelle As Integer

        Dim i_DatiCampo As Integer
        Dim i_Campo As Integer
        '   Dim i_Dettaglio               As Integer
        Dim i_Codice As Integer

        Dim OpeDB_Campo As String
        '   Dim OpeDB_Dettaglio           As String
        Dim OpeDB_Codice As String
        Dim OpeDB_CampoxParticella As String


        '------------------------------
        Dim app_inizio As Date
        Dim app_fine As Date

        Dim FlagConnessioneLocale As Boolean
        Dim FlagTransazioneLocale As Boolean

        '------------------------------

        Try

            '------------------------------
            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale,
                                                           objParametri)
            '------------------------------

            XmlDoc = New XmlDocument
            XmlDoc.LoadXml(DatiCampo)

            '------------------------------


            xDatiCampi = XmlDoc.GetElementsByTagName("DatiCampi")

            i_DatiCampo = 0

            Do While i_DatiCampo < xDatiCampi.Count

                'Prelevo l'i-esimo blocco di DatiCampi (in realtà ne esiste uno solo)
                xDatiCampo = xDatiCampi.Item(i_DatiCampo)

                '------------------------------

                xCampi = xDatiCampo.GetElementsByTagName("Campo")

                i_Campo = 0

                Do While i_Campo < xCampi.Count

                    'Prelevo l' i-esimo Campo
                    xCampo = xCampi.Item(i_Campo)

                    'Prelevo gli attributi del campo selezionato
                    OpeDB_Campo = xCampo.GetAttribute("TipoOperazioneDB")

                    'Inizializzo Preventivamente il Cod_Campo
                    Cod_Campo = CInt(xCampo.GetAttribute("campo_cod"))

                    app_inizio = objParametri.FinestraTemporaleInizio
                    app_fine = objParametri.FinestraTemporaleFine
                    objParametri.FinestraTemporaleInizio = CDate(xCampo.GetAttribute("validita_inizio"))
                    objParametri.FinestraTemporaleFine = CDate(xCampo.GetAttribute("validita_fine"))


                    If Not IsNothing(xCampo.GetAttribute("basecode")) AndAlso xCampo.GetAttribute("basecode") <> "" Then
                        BaseCode = xCampo.GetAttribute("basecode")
                    Else
                        BaseCode = 0
                    End If



                    'Verifico l'operazione richiesta
                    Select Case OpeDB_Campo

                        Case "0"    'LEGGI -------------------------------------------------------

                            OUTPUT_Piva = CStr(xCampo.GetAttribute("piva"))
                            OUTPUT_Sa_Cod = CInt(xCampo.GetAttribute("sa_cod"))
                            OUTPUT_Campo_Cod = CInt(xCampo.GetAttribute("campo_cod"))

                        Case "1"    'SALVA -------------------------------------------------------

                            If Cod_Campo <= 0 Then

                                Cod_Campo = objSequenze.NuovoId_Campi(CStr(xCampo.GetAttribute("piva")),
                                                                      CInt(xCampo.GetAttribute("sa_cod")),
                                                                      CInt(xCampo.GetAttribute("basecode")),
                                                                      CInt(xCampo.GetAttribute("topcode")),
                                                                      objParametri)

                                objSequenze = Nothing

                                OUTPUT_Piva = CStr(xCampo.GetAttribute("piva"))
                                OUTPUT_Sa_Cod = CInt(xCampo.GetAttribute("sa_cod"))
                                OUTPUT_Campo_Cod = CInt(Cod_Campo)

                            Else

                                'Esportazione del campo in locale
                                OUTPUT_Piva = CStr(xCampo.GetAttribute("piva"))
                                OUTPUT_Sa_Cod = CInt(xCampo.GetAttribute("sa_cod"))
                                OUTPUT_Campo_Cod = CInt(xCampo.GetAttribute("campo_cod"))

                            End If

                            Dummy = objCampi.Scrivi(CStr(xCampo.GetAttribute("piva")),
                                                    CInt(xCampo.GetAttribute("sa_cod")),
                                                    Cod_Campo,
                                                    IIf(CStr(xCampo.GetAttribute("campo_des")) <> "", CStr(xCampo.GetAttribute("campo_des")), "Campo " & Format((Cod_Campo - BaseCode) Mod 100, "000")),
                                                    Agro_XML_GetInteger(xCampo, "gru_cod", 0),
                                                    Agro_XML_GetInteger(xCampo, "veg_cod", 0),
                                                    CDbl(xCampo.GetAttribute("sau_totale")),
                                                    Agro_XML_GetDecimal(xCampo, "sau_biologico", 0),
                                                    Agro_XML_GetDecimal(xCampo, "sau_conversione", 0),
                                                    Agro_XML_GetDecimal(xCampo, "sau_convenzionale", 0),
                                                    Agro_XML_GetDate(xCampo, "conversione_inizio", AGRODATAINIZIO),
                                                    Agro_XML_GetDate(xCampo, "conversione_fine", AGRODATAFINE),
                                                    Agro_XML_GetString(xCampo, "confinirischio", ""),
                                                    Agro_XML_GetInteger(xCampo, "campo_tipo", 0),
                                                    CDate(xCampo.GetAttribute("validita_inizio")),
                                                    CDate(xCampo.GetAttribute("validita_fine")),
                                                    objParametri,
                                                    Data_creazione:=Agro_XML_GetDate(xCampo, "data_creazione", #2/1/1900#),
                                                    Data_modifica:=Agro_XML_GetDate(xCampo, "data_modifica", #2/1/1900#),
                                                    username_creazione:=Agro_XML_GetString(xCampo, "username_creazione", ""),
                                                    username_modifica:=Agro_XML_GetString(xCampo, "username_modifica", "")
                                                    )

                            Dummy = objUtentixCampi.Scrivi(CStr(xCampo.GetAttribute("piva")),
                                                           CInt(xCampo.GetAttribute("sa_cod")),
                                                           Cod_Campo,
                                                           CDate(xCampo.GetAttribute("validita_inizio")),
                                                           CDate(xCampo.GetAttribute("validita_fine")),
                                                           objParametri)

                        Case "2"    'MODIFICA -------------------------------------------------------

                            OUTPUT_Piva = CStr(xCampo.GetAttribute("piva"))
                            OUTPUT_Sa_Cod = CInt(xCampo.GetAttribute("sa_cod"))
                            OUTPUT_Campo_Cod = CInt(xCampo.GetAttribute("campo_cod"))


                            objCampi.Modifica(CStr(xCampo.GetAttribute("piva")),
                                              CInt(xCampo.GetAttribute("sa_cod")),
                                              CInt(xCampo.GetAttribute("campo_cod")),
                                              IIf(CStr(xCampo.GetAttribute("campo_des")) <> "", CStr(xCampo.GetAttribute("campo_des")), "Campo " & Format((Cod_Campo - BaseCode) Mod 100, "000")),
                                              Agro_XML_GetInteger(xCampo, "gru_cod", 0),
                                              Agro_XML_GetInteger(xCampo, "veg_cod", 0),
                                              CDbl(xCampo.GetAttribute("sau_totale")),
                                              Agro_XML_GetDecimal(xCampo, "sau_biologico", 0),
                                              Agro_XML_GetDecimal(xCampo, "sau_conversione", 0),
                                              Agro_XML_GetDecimal(xCampo, "sau_convenzionale", 0),
                                              Agro_XML_GetDate(xCampo, "conversione_inizio", AGRODATAINIZIO),
                                              Agro_XML_GetDate(xCampo, "conversione_fine", AGRODATAFINE),
                                              Agro_XML_GetString(xCampo, "confinirischio", ""),
                                              Agro_XML_GetInteger(xCampo, "campo_tipo", 0),
                                              CDate(xCampo.GetAttribute("validita_inizio")),
                                              CDate(xCampo.GetAttribute("validita_fine")),
                                              "",
                                              objParametri,
                                              Data_modifica:=Agro_XML_GetDate(xCampo, "data_modifica", #2/1/1900#),
                                              username_modifica:=Agro_XML_GetString(xCampo, "username_modifica", "")
                                              )


                            'Aggiorno le Validità inizio e fine

                            'Aggiorno le validita delle relazioni CampixParticelle
                            objCampixParticelle.AggiornaValidita(CStr(xCampo.GetAttribute("piva")),
                                                                 CInt(xCampo.GetAttribute("sa_cod")),
                                                                 CInt(xCampo.GetAttribute("campo_cod")),
                                                                 CDate(xCampo.GetAttribute("validita_inizio")),
                                                                 CDate(xCampo.GetAttribute("validita_fine")),
                                                                 "",
                                                                 objParametri)

                            objUtentixCampi.AggiornaValiditaInizio(CStr(xCampo.GetAttribute("piva")),
                                                                   CInt(xCampo.GetAttribute("sa_cod")),
                                                                   CInt(xCampo.GetAttribute("campo_cod")),
                                                                   CDate(xCampo.GetAttribute("validita_inizio")),
                                                                   "",
                                                                   objParametri)

                            objUtentixCampi.AggiornaValiditaFine(CStr(xCampo.GetAttribute("piva")),
                                                                 CInt(xCampo.GetAttribute("sa_cod")),
                                                                 CInt(xCampo.GetAttribute("campo_cod")),
                                                                 CDate(xCampo.GetAttribute("validita_fine")),
                                                                 "",
                                                                 objParametri)

                    End Select



                    '-------------------------------------------------------------
                    ' DETTAGLI
                    '-------------------------------------------------------------

                    'Prelevo l'elenco dei Dettagli del Campo
                    'Set xDettagli = xCampo.getElementsByTagName("Dettaglio")

                    'i_Dettaglio = 0

                    'Do While i_Dettaglio < xDettagli.length

                    'Prelevo l'i-esimo Dettaglio_Campo
                    '   Set xDettaglio = xDettagli.Item(i_Dettaglio)

                    'Prelevo gli attributi del codice selezionato
                    '   OpeDB_Dettaglio = xDettaglio.getAttribute("TipoOperazioneDB")

                    'Creo l'oggetto COM
                    '   Set objDettaglio = CreateObject("Agro_Anagrafe_AD.Campi_Dettagli_Write")
                    '   Set objSequenze = CreateObject("Agro_Anagrafe_AD.Agro_Sequenze")

                    'Verifico l'operazione richiesta
                    '   Select Case OpeDB_Dettaglio
                    '
                    '      Case "0"    'LEGGI -------------------------------------------------------
                    '
                    '      Case "1"    'SALVA -------------------------------------------------------

                    '
                    'Richiedo un nuovo codice Dettaglio_Campo
                    'Cod_Dettaglio = objSequenze.NuovoId_Campo_Dettaglio(CStr(xCampo.getAttribute("piva")), _
                    '         CInt(xCampo.getAttribute("sa_cod")), _
                    '         Cod_Campo, _
                    '         CStr(Utente), _
                    '         objCnManager, _
                    '         CInt(xDettaglio.getAttribute("basecode")), _
                    '         CInt(xDettaglio.getAttribute("topcode")))

                    'Salvataggio

                    'Dummy = objDettaglio.Scrivi(CStr(xCampo.getAttribute("piva")), _
                    '            CInt(xCampo.getAttribute("sa_cod")), _
                    '            Cod_Campo, _
                    '            Cod_Dettaglio, _
                    '            CDbl(xDettaglio.getAttribute("area")), _
                    '            CStr(Utente), _
                    '            CDate(xDettaglio.getAttribute("validita_inizio")), _
                    '            CDate(xDettaglio.getAttribute("validita_fine")), _
                    '            objCnManager)
                    '
                    '      Case "2"    'MODIFICA -------------------------------------------------------
                    '
                    'objDettaglio.Modifica _
                    '            CStr(xCampo.getAttribute("piva")), _
                    '            CInt(xCampo.getAttribute("sa_cod")), _
                    '            CInt(xCampo.getAttribute("campo_cod")), _
                    '            CInt(xDettaglio.getAttribute("dettaglio_cod")), _
                    '            CDbl(xDettaglio.getAttribute("area")), _
                    '            CDate(xDettaglio.getAttribute("validita_inizio")), _
                    '            CDate(xDettaglio.getAttribute("validita_fine")), _
                    '            CStr(Utente), _
                    '            objCnManager


                    '
                    '      Case "3"    'ELIMINA -------------------------------------------------------
                    '
                    ' objDettaglio.Cancella _
                    '            CStr(xCampo.getAttribute("piva")), _
                    '            CInt(xCampo.getAttribute("sa_cod")), _
                    '            CInt(xCampo.getAttribute("campo_cod")), _
                    '            CInt(xDettaglio.getAttribute("dettaglio_cod")), _
                    '            CStr(Utente), _
                    '            objCnManager


                    '    End Select

                    'Incremento l'indice
                    '    i_Dettaglio = i_Dettaglio + 1

                    ' Loop


                    '-------------------------------------------------------------
                    ' ENTITA GRAFICHE
                    '-------------------------------------------------------------

                    xEntitaGrafiche = xCampo.GetElementsByTagName("DatiEntita")
                    If xEntitaGrafiche.Count = 1 Then
                        objGrafica.Grafica_Scrivi2005(xEntitaGrafiche.Item(0).OuterXml,
                                                      CStr(xCampo.GetAttribute("piva")),
                                                      CInt(xCampo.GetAttribute("sa_cod")),
                                                      Cod_Campo, True,
                                                      objParametri)
                    End If


                    '-------------------------------------------------------------
                    ' CODICI
                    '-------------------------------------------------------------

                    ' se G2G cancello eventuali codici presenti
                    If TipoG2G <> 0 AndAlso OpeDB_Campo = 2 Then
                        objCampixCodici.Cancella(CStr(xCampo.GetAttribute("piva")),
                                                 CInt(xCampo.GetAttribute("sa_cod")),
                                                 CInt(xCampo.GetAttribute("campo_cod")),
                                                 0, "", objParametri)
                    End If

                    'Prelevo l'elenco dei codici
                    xCodici = xCampo.GetElementsByTagName("Codice")

                    i_Codice = 0

                    Do While i_Codice < xCodici.Count

                        'Prelevo l'i-esimo codice
                        xCodice = xCodici.Item(i_Codice)

                        'Prelevo gli attributi del codice selezionato (se G2G forzo inserimento)
                        OpeDB_Codice = If(TipoG2G = 0, xCodice.GetAttribute("TipoOperazioneDB"), "1")

                        'Verifico l'operazione richiesta
                        Select Case OpeDB_Codice

                            Case "0"    'LEGGI -------------------------------------------------------

                            Case "1"    'SALVA -------------------------------------------------------

                                'Se entro in modifica e inserisco un nuovo codice ->
                                'il campo_cod si trova nella stringa Xml
                                If OpeDB_Campo = 2 Then
                                    Cod_Campo = CInt(xCampo.GetAttribute("campo_cod"))
                                End If


                                Dummy = objCampixCodici.Scrivi(CStr(xCampo.GetAttribute("piva")),
                                                               CInt(xCampo.GetAttribute("sa_cod")),
                                                               Cod_Campo,
                                                               CInt(xCodice.GetAttribute("id_cod")),
                                                               CStr(xCodice.GetAttribute("val_cod")),
                                                               CDate(xCodice.GetAttribute("validita_inizio")),
                                                               CDate(xCodice.GetAttribute("validita_fine")),
                                                               objParametri)

                            Case "2"    'MODIFICA -------------------------------------------------------

                                objCampixCodici.Modifica(CStr(xCampo.GetAttribute("piva")),
                                                         CInt(xCampo.GetAttribute("sa_cod")),
                                                         CInt(xCampo.GetAttribute("campo_cod")),
                                                         CInt(xCodice.GetAttribute("id_cod")),
                                                         CStr(xCodice.GetAttribute("val_cod")),
                                                         CDate(xCodice.GetAttribute("validita_inizio")),
                                                         CDate(xCodice.GetAttribute("validita_fine")),
                                                         "",
                                                         objParametri)


                            Case "3"    'ELIMINA -------------------------------------------------------

                                objCampixCodici.Cancella(CStr(xCampo.GetAttribute("piva")),
                                                         CInt(xCampo.GetAttribute("sa_cod")),
                                                         CInt(xCampo.GetAttribute("campo_cod")),
                                                         CInt(xCodice.GetAttribute("id_cod")),
                                                         "",
                                                         objParametri)

                        End Select

                        'Incremento l'indice
                        i_Codice += 1

                    Loop


                    '-------------------------------------------------------------
                    ' CAMPIXPARTICELLE
                    '-------------------------------------------------------------

                    xDatiCampixParticelle = xCampo.GetElementsByTagName("DatiCampixParticelle")

                    i_DatiParticelle = 0

                    Do While i_DatiParticelle < xDatiCampixParticelle.Count

                        'Prelevo l'i-esimo blocco di DatiParticelle (in realtà ne esiste uno solo)
                        xDatiCampoxParticella = xDatiCampixParticelle.Item(i_DatiParticelle)

                        '------------------------------

                        xCampixParticelle = xDatiCampoxParticella.GetElementsByTagName("CampoxParticella")

                        i_DatiParticella = 0

                        Do While i_DatiParticella < xCampixParticelle.Count

                            xCampoxParticella = xCampixParticelle.Item(i_DatiParticella)

                            'Prelevo gli attributi dell'indirizzo selezionato
                            OpeDB_CampoxParticella = xCampoxParticella.GetAttribute("TipoOperazioneDB")

                            'Verifico l'operazione richiesta
                            Select Case OpeDB_CampoxParticella

                                Case "0"    'LEGGI  ----------------------

                                Case "1"    'SALVA  ----------------------

                                    Dummy = objCampixParticelle.Scrivi(CStr(xCampoxParticella.GetAttribute("piva")),
                                                                       CInt(xCampoxParticella.GetAttribute("sa_cod")),
                                                                       Cod_Campo,
                                                                       CStr(xCampoxParticella.GetAttribute("prov")),
                                                                       CStr(xCampoxParticella.GetAttribute("com")),
                                                                       CStr(xCampoxParticella.GetAttribute("sezione")),
                                                                       CInt(xCampoxParticella.GetAttribute("foglio")),
                                                                       CInt(xCampoxParticella.GetAttribute("numero")),
                                                                       CStr(xCampoxParticella.GetAttribute("subalterno")),
                                                                       CDbl(xCampoxParticella.GetAttribute("sau_convenz_ettari")),
                                                                       CInt(xCampoxParticella.GetAttribute("sau_convenz_are")),
                                                                       CInt(xCampoxParticella.GetAttribute("sau_convenz_centiare")),
                                                                       CDbl(xCampoxParticella.GetAttribute("sau_convers_ettari")),
                                                                       CInt(xCampoxParticella.GetAttribute("sau_convers_are")),
                                                                       CInt(xCampoxParticella.GetAttribute("sau_convers_centiare")),
                                                                       CDbl(xCampoxParticella.GetAttribute("sau_bio_ettari")),
                                                                       CInt(xCampoxParticella.GetAttribute("sau_bio_are")),
                                                                       CInt(xCampoxParticella.GetAttribute("sau_bio_centiare")),
                                                                       Replace(xCampoxParticella.GetAttribute("area"), ".", ","),
                                                                       CDate(xCampoxParticella.GetAttribute("validita_inizio")),
                                                                       CDate(xCampoxParticella.GetAttribute("validita_fine")),
                                                                       objParametri)

                                Case "2"   'MODIFICA ----------------------

                                    objCampixParticelle.Modifica(CStr(xCampoxParticella.GetAttribute("piva")),
                                                                 CInt(xCampoxParticella.GetAttribute("sa_cod")),
                                                                 CInt(xCampoxParticella.GetAttribute("campo_cod")),
                                                                 CStr(xCampoxParticella.GetAttribute("prov")),
                                                                 CStr(xCampoxParticella.GetAttribute("com")),
                                                                 CStr(xCampoxParticella.GetAttribute("sezione")),
                                                                 CInt(xCampoxParticella.GetAttribute("foglio")),
                                                                 CInt(xCampoxParticella.GetAttribute("numero")),
                                                                 CStr(xCampoxParticella.GetAttribute("subalterno")),
                                                                 Replace(xCampoxParticella.GetAttribute("area"), ".", ","),
                                                                 CDbl(xCampoxParticella.GetAttribute("sau_convenz_ettari")),
                                                                 CInt(xCampoxParticella.GetAttribute("sau_convenz_are")),
                                                                 CInt(xCampoxParticella.GetAttribute("sau_convenz_centiare")),
                                                                 CDbl(xCampoxParticella.GetAttribute("sau_convers_ettari")),
                                                                 CInt(xCampoxParticella.GetAttribute("sau_convers_are")),
                                                                 CInt(xCampoxParticella.GetAttribute("sau_convers_centiare")),
                                                                 CDbl(xCampoxParticella.GetAttribute("sau_bio_ettari")),
                                                                 CInt(xCampoxParticella.GetAttribute("sau_bio_are")),
                                                                 CInt(xCampoxParticella.GetAttribute("sau_bio_centiare")),
                                                                 CDate(xCampoxParticella.GetAttribute("validita_inizio")),
                                                                 CDate(xCampoxParticella.GetAttribute("validita_fine")),
                                                                 "",
                                                                 objParametri)

                                Case "3"   'CANCELLA --------------------------

                                    objCampixParticelle.Cancella_Associazione(CStr(xCampoxParticella.GetAttribute("piva")),
                                                                              CInt(xCampoxParticella.GetAttribute("sa_cod")),
                                                                              CInt(xCampoxParticella.GetAttribute("campo_cod")),
                                                                              CStr(xCampoxParticella.GetAttribute("prov")),
                                                                              CStr(xCampoxParticella.GetAttribute("com")),
                                                                              CStr(xCampoxParticella.GetAttribute("sezione")),
                                                                              CInt(xCampoxParticella.GetAttribute("foglio")),
                                                                              CInt(xCampoxParticella.GetAttribute("numero")),
                                                                              CStr(xCampoxParticella.GetAttribute("subalterno")),
                                                                              "",
                                                                              objParametri)

                            End Select

                            i_DatiParticella += 1

                        Loop

                        i_DatiParticelle += 1

                    Loop

                    '</CAMPIXPARTICELLE>
                    '------------------------------------------------------------------------------------------


                    Select Case OpeDB_Campo

                        Case "2"

                            '-------------------------------------------------
                            'Modifica delle finestre temporali dei figli
                            Dim dtAppezza = objAppezzamenti_R.LeggiconCampo(CStr(xCampo.GetAttribute("piva")),
                                                                   CInt(xCampo.GetAttribute("sa_cod")),
                                                                   CInt(xCampo.GetAttribute("campo_cod")),
                                                                   enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                   "", "", objParametri)

                            Dim objAgenda_R As New AgronicaCoreContabDAL.Mov_Destinazioni_R

                            Dim Validita_Inizio_Orig = objParametri.FinestraTemporaleInizio
                            Dim Validita_Fine_Orig = objParametri.FinestraTemporaleFine

                            For Each RowAppezza In dtAppezza.Rows
                                Dim validita_inizio_appezza As Date = RowAppezza("Validita_Inizio")
                                Dim validita_fine_appezza As Date = RowAppezza("Validita_Fine")

                                If CDate(xCampo.GetAttribute("validita_inizio")) > validita_inizio_appezza Then
                                    objParametri.FinestraTemporaleInizio = validita_inizio_appezza
                                    objParametri.FinestraTemporaleFine = CDate(xCampo.GetAttribute("validita_inizio"))
                                    Dim dt = objAgenda_R.Leggi_conImpianti(CStr(xCampo.GetAttribute("piva")), CInt(xCampo.GetAttribute("sa_cod")), 0, 0, 0, RowAppezza("Appezza"), 0, "", "", objParametri)
                                    If dt.Rows.Count > 0 Then
                                        objParametri.FinestraTemporaleInizio = Validita_Inizio_Orig
                                        objParametri.FinestraTemporaleFine = Validita_Fine_Orig
                                        Throw New Exception("Sono presenti operazioni di agenda precedenti alla chiusura del campo per l'appezzamento " & CStr(RowAppezza("App_Nome")))
                                    End If
                                End If

                                If CDate(xCampo.GetAttribute("validita_fine")) < validita_fine_appezza Then
                                    objParametri.FinestraTemporaleInizio = CDate(xCampo.GetAttribute("validita_fine"))
                                    objParametri.FinestraTemporaleFine = validita_fine_appezza
                                    Dim dt = objAgenda_R.Leggi_conImpianti(CStr(xCampo.GetAttribute("piva")), CInt(xCampo.GetAttribute("sa_cod")), 0, 0, 0, RowAppezza("Appezza"), 0, "", "", objParametri)
                                    If dt.Rows.Count > 0 Then
                                        objParametri.FinestraTemporaleInizio = Validita_Inizio_Orig
                                        objParametri.FinestraTemporaleFine = Validita_Fine_Orig
                                        Throw New Exception("Sono presenti operazioni di agenda successive alla chiusura del campo per l'appezzamento " & CStr(RowAppezza("App_Nome")))
                                    End If
                                End If

                            Next

                            objAppezzamenti_W.AggiornaValiditaInizio(CStr(xCampo.GetAttribute("piva")),
                                                                   CInt(xCampo.GetAttribute("sa_cod")),
                                                                   CInt(xCampo.GetAttribute("campo_cod")),
                                                                   0,
                                                                   CDate(xCampo.GetAttribute("validita_inizio")),
                                                                   "",
                                                                   objParametri)
                            objAppezzamenti_W.AggiornaValiditaFine(CStr(xCampo.GetAttribute("piva")),
                                                                 CInt(xCampo.GetAttribute("sa_cod")),
                                                                 CInt(xCampo.GetAttribute("campo_cod")),
                                                                 0,
                                                                 CDate(xCampo.GetAttribute("validita_fine")),
                                                                 "",
                                                                 objParametri)
                            objAppezzamenti_W = Nothing

                            objUtentixAppezzamenti.AggiornaValiditaInizio(CStr(xCampo.GetAttribute("piva")),
                                                                          CInt(xCampo.GetAttribute("sa_cod")),
                                                                          CInt(xCampo.GetAttribute("campo_cod")),
                                                                          0,
                                                                          CDate(xCampo.GetAttribute("validita_inizio")),
                                                                          "",
                                                                          objParametri)
                            objUtentixAppezzamenti.AggiornaValiditaFine(CStr(xCampo.GetAttribute("piva")),
                                                                        CInt(xCampo.GetAttribute("sa_cod")),
                                                                        CInt(xCampo.GetAttribute("campo_cod")),
                                                                        0,
                                                                        CDate(xCampo.GetAttribute("validita_fine")),
                                                                        "",
                                                                        objParametri)
                            objUtentixAppezzamenti = Nothing


                            objAppezzaxParticelle.AggiornaValiditaInizio(CStr(xCampo.GetAttribute("piva")),
                                                                         CInt(xCampo.GetAttribute("sa_cod")),
                                                                         CInt(xCampo.GetAttribute("campo_cod")),
                                                                         0,
                                                                         "",
                                                                         "",
                                                                         "",
                                                                         0, 0, "", CDate(xCampo.GetAttribute("validita_inizio")),
                                                                         "",
                                                                         objParametri)

                            objAppezzaxParticelle.AggiornaValiditaFine(CStr(xCampo.GetAttribute("piva")),
                                                                       CInt(xCampo.GetAttribute("sa_cod")),
                                                                       CInt(xCampo.GetAttribute("campo_cod")),
                                                                       0,
                                                                       "", "", "", 0, 0, "", CDate(xCampo.GetAttribute("validita_fine")),
                                                                       "",
                                                                       objParametri)

                            objAppezzaxParticelle = Nothing

                            objReg_Impianti.AggiornaValiditaInizio(CStr(xCampo.GetAttribute("piva")),
                                                                   CInt(xCampo.GetAttribute("sa_cod")),
                                                                   0,
                                                                   CInt(xCampo.GetAttribute("campo_cod")),
                                                                   0,
                                                                   CDate(xCampo.GetAttribute("validita_inizio")),
                                                                   objParametri)
                            objReg_Impianti.AggiornaValiditaFine(CStr(xCampo.GetAttribute("piva")),
                                                                 CInt(xCampo.GetAttribute("sa_cod")),
                                                                 0,
                                                                 CInt(xCampo.GetAttribute("campo_cod")),
                                                                 0,
                                                                 CDate(xCampo.GetAttribute("validita_fine")),
                                                                 objParametri)
                            objReg_Impianti = Nothing

                            'Aggiorno le date della grafica
                            GraphicKey = "M" & Right(New String("0", 8) & Hex(CInt(xCampo.GetAttribute("campo_cod"))), 8)

                            objGrafica_AD.AggiornaValiditaInizio(CStr(xCampo.GetAttribute("piva")),
                                                                 CInt(xCampo.GetAttribute("sa_cod")),
                                                                 GraphicKey,
                                                                 CDate(xCampo.GetAttribute("validita_inizio")),
                                                                 "",
                                                                 objParametri)
                            objGrafica_AD.AggiornaValiditaFine(CStr(xCampo.GetAttribute("piva")),
                                                               CInt(xCampo.GetAttribute("sa_cod")),
                                                               GraphicKey,
                                                               CDate(xCampo.GetAttribute("validita_fine")),
                                                               "",
                                                               objParametri)

                        Case "3" 'CANCELLAZIONE CAMPO


                            'Cancello i figli Maggiori del Campo

                            'Cancella_Solo_Campo mi dice se devo cancellare il campo con tutti i figli o solo il campo.
                            'Questo perché un appezzamento può essere figlio di un centro o di un campo e senza
                            'questo accorgimento si verrebbe a creare una situazione anomala, cioè si tenterebbe di
                            'leggere degli appezzamenti che precedentemente erano stati cancellati senza però ancora
                            'eseguire il commit.

                            If Not Cancello_Solo_Campo Then

                                'Cancello TUTTI gli APPEZZAMENTI APPARTENENTI AL CAMPO
                                XmlAppezzamenti = objAppezzamentiLeggi.Appezzamento_Leggi(CStr(xCampo.GetAttribute("piva")),
                                                                                          CInt(xCampo.GetAttribute("sa_cod")),
                                                                                          CInt(xCampo.GetAttribute("campo_cod")),
                                                                                          0,
                                                                                          True, False, False, False,
                                                                                          objParametri)
                                objAppezzamentiLeggi = Nothing

                                If XmlAppezzamenti <> "" Then
                                    objAppezzamentiScrivi.Appezzamento_Scrivi(XmlAppezzamenti,
                                                                              "",
                                                                              0,
                                                                              0,
                                                                              objParametri,
                                                                              objParametri_Utenti)

                                    objAppezzamentiScrivi = Nothing
                                End If

                            End If
                            '------------------------------------------------------

                            objCampixCodici.Cancella(CStr(xCampo.GetAttribute("piva")),
                                                     CInt(xCampo.GetAttribute("sa_cod")),
                                                     CInt(xCampo.GetAttribute("campo_cod")),
                                                     0,
                                                     "",
                                                     objParametri)

                            objCampixParticelle.Cancella(CStr(xCampo.GetAttribute("piva")),
                                                         CInt(xCampo.GetAttribute("sa_cod")),
                                                         CInt(xCampo.GetAttribute("campo_cod")),
                                                         "",
                                                         "",
                                                         "",
                                                         0,
                                                         0,
                                                         "",
                                                         "",
                                                         objParametri)


                            objUtentixCampi.Cancella(CStr(xCampo.GetAttribute("piva")),
                                                     CInt(xCampo.GetAttribute("sa_cod")),
                                                     CInt(xCampo.GetAttribute("campo_cod")),
                                                     "",
                                                     objParametri)

                            objCampi.Cancella(CStr(xCampo.GetAttribute("piva")),
                                              CInt(xCampo.GetAttribute("sa_cod")),
                                              CInt(xCampo.GetAttribute("campo_cod")),
                                              "",
                                              objParametri)


                            'Cancella i dati grafici
                            GraphicKey = "M" & Right(New String("0", 8) & Hex(CInt(xCampo.GetAttribute("campo_cod"))), 8)

                            objGrafica_AD.Cancella(CStr(xCampo.GetAttribute("piva")),
                                            CInt(xCampo.GetAttribute("sa_cod")),
                                            "",
                                            GraphicKey,
                                            "",
                                             "",
                                             objParametri)

                            GraphicKey = "N" & Right(New String("0", 8) & Hex(CInt(xCampo.GetAttribute("campo_cod"))), 8)

                            objGrafica_AD.Cancella(CStr(xCampo.GetAttribute("piva")),
                                                   CInt(xCampo.GetAttribute("sa_cod")),
                                                   "",
                                                   GraphicKey,
                                                   "",
                                                   "",
                                                   objParametri)

                            '03/05/2018 Grilli: in accordo con Fabrizio, in caso di cancellazione di un campo,
                            'se nel Planning (siano essi già ribaltati o no), 
                            'ci sono dei riferimenti ai campi, vengono tolti impostando campo_cod a zero.
                            Dim objPE As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_W
                            objPE.Azzera_Campo_Cod(CStr(xCampo.GetAttribute("piva")),
                                                    CInt(xCampo.GetAttribute("sa_cod")),
                                                    CInt(xCampo.GetAttribute("campo_cod")),
                                                    "", objParametri)

                    End Select

                    '-------------------------------------------------------------

                    'Incremento l'indice
                    i_Campo += 1

                Loop

                '------------------------------

                'Incremento l'indice
                i_DatiCampo += 1

            Loop

            '------------------------------

            'Elimino tutti gli oggetti utilizzati

            'Set objDettaglio = Nothing

            'Set xDettaglio = Nothing
            'Set xDettagli = Nothing
            xCodice = Nothing
            xCodici = Nothing
            xCampo = Nothing
            xDatiCampo = Nothing
            XmlDoc = Nothing
            'Elimino l'oggetto
            objCampi = Nothing
            objUtentixCampi = Nothing
            objCampixParticelle = Nothing
            objCampixCodici = Nothing
            objGrafica_AD = Nothing

            '------------------------------

            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)

            '------------------------------

            'Restituisco un valore Dummy
            '  Campo_Scrivi = Cod_Campo

            objParametri.FinestraTemporaleInizio = app_inizio
            objParametri.FinestraTemporaleFine = app_fine

            Esito = True

        Catch ex As Exception

            Esito = False

            objParametri.FinestraTemporaleInizio = app_inizio
            objParametri.FinestraTemporaleFine = app_fine

            'Faccio il rollback della transazione
            If objParametri.objTransazione IsNot Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            '//////////////////////////////////////////////////////////////////////
            MessaggioErrore = "(Piva=" & OUTPUT_Piva & ")" &
                              "(Sa_Cod=" & CStr(OUTPUT_Sa_Cod) & ")" &
                              "(Campo_Cod=" & CStr(OUTPUT_Campo_Cod) & ")" &
                              " : " & ex.Message
            '//////////////////////////////////////////////////////////////////////


            Scrivi_LOG(objParametri, MetodoNome, MessaggioErrore)
            Throw New Exception("[" & MetodoNome & "] : " & MessaggioErrore)

        Finally

            ''Chiudo la connessione se è stata aperta in questa routine
            'If (FlagConnessioneLocale = True) AndAlso (Not objParametri.objConnessione Is Nothing) Then
            '    objParametri.objConnessione.Close()
            'End If
            'Chiudo la connessione se è stata aperta in questa routine
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)

        End Try

        Return Esito

    End Function

    Public Function Scrivi_Campo_Anagrafica(ByRef objCampo As AgronicaCoreModelsSTD.anagrafiche.Campo,
                                            ByVal tipoOperazione As enum_TipoOperazioneDB,
                                            ByRef objParametri_Server As AgronicaCoreParametri,
                                            ByRef objParametri_Utenti As AgronicaCoreParametri,
                                            Optional NoteLog As String = NOTELOG_ANAGRAFE_NG,
                                            Optional AppezzaDisaggregaSoloInCancellazione As Boolean = False) As String

        Dim Piva_SuperUser = objParametri_Server.PivaSuperUser
        Dim dal As New AgronicaCoreAnagrafeDAL.EFCampi()

        Dim ObjSequenze = New Agro_Sequenze
        Dim success As Boolean = True

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Campo_W.Scrivi_Campo_Anagrafica()"
        Dim MessaggioErrore As String = ""

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)

        Dim stWa As New Stopwatch
        stWa.Start()
        Dim scopeOption As New TransactionScopeOption
        Dim transactionOptions As New TransactionOptions
        transactionOptions.IsolationLevel = IsolationLevel.ReadUncommitted

        Dim EntitaCodxImg As New List(Of Integer)

        Using scope As New TransactionScope(scopeOption, transactionOptions)
            Using GiasContext As Gias_DeveloperServer_Entities = Gias_EF_Utility.CreateGiasContextConnection(objParametri_Server.StringaConnessione)

                Try
                    'dal.ContextOptions.UseLegacyPreserveChangesBehavior = False
                    'Open the contextObject connection state explicitly
                    GiasContext.Database.Connection.Open()

                    'disabilitaMergeOptions(dal)

                    GiasContext.Database.ExecuteSqlCommand("SET ARITHABORT ON;")



                    If tipoOperazione = enum_TipoOperazioneDB.Modifica Then
                        Verifica_ValiditaInizioFine(objCampo, objParametri_Server, objParametri_Utenti)
                    End If

                    If (tipoOperazione = enum_TipoOperazioneDB.Scrittura) Then
                        'Verifica_ValiditaInizioFine(objCampo, objParametri_Server)
                        Dim Campo_POCO = dal.Campo_Scrivi_EF(objCampo,
                                                             objParametri_Server,
                                                             objParametri_Utenti,
                                                             objParametri_Server.UsernameOperazione,
                                                             GiasContext,
                                                             False,
                                                             NoteLog:=NoteLog)

                        'Dim mess = Check_PianoConcimazione_EntitaxTestata(objCampo, Campo_POCO, tipoOperazione, GiasContext, objParametri_Server, objParametri_Utenti)
                        'If mess <> "" Then
                        '    Throw New GiasException(mess)
                        'End If


                        UtentiXCampi_Scrivi_Campo(Campo_POCO,
                                                  tipoOperazione,
                                                  objParametri_Server,
                                                  objParametri_Utenti)

                        CampiXCodici_ScriviModificaCancella_Campo_EF(objCampo,
                                                                     tipoOperazione,
                                                                     objParametri_Server,
                                                                     objParametri_Utenti,
                                                                     GiasContext,
                                                                     False,
                                                                     Campo_POCO)

                        Appezzamenti_ScriviModificaCancella_Campo(objCampo,
                                                                  tipoOperazione,
                                                                  objParametri_Server,
                                                                  objParametri_Utenti,
                                                                  Campo_POCO)

                        CampiXParticelle_ScriviModificaCancella_Campo(objCampo,
                                                                      tipoOperazione,
                                                                      objParametri_Server,
                                                                      objParametri_Utenti,
                                                                      Campo_POCO)

                        If objCampo.cartografia IsNot Nothing AndAlso objCampo.cartografia <> "" Then
                            EntitaGrafiche_ScriviModifica_Campo_EF(
                            objCampo,
                            objParametri_Server,
                            objParametri_Server.UsernameOperazione,
                            EntitaCodxImg,
                            tipoOperazione,
                            GiasContext,
                            False,
                            Campo_POCO
                        )
                        End If

                        objCampo.primaryKey.codice = Campo_POCO.Campo_Cod

                    ElseIf (tipoOperazione = enum_TipoOperazioneDB.Modifica) Then
                        '--------------------------------------------------------------------------------------
                        ' aggiorno le validita solo se è date di inizio o fine del campo sono variate
                        Dim campo_read As New AgronicaCoreAnagrafeDAL.Campi_R
                        Dim campo_old = campo_read.Leggi(objCampo.primaryKey.centroAziendalePK.partitaIva,
                                                            objCampo.primaryKey.centroAziendalePK.codice,
                                                            objCampo.primaryKey.codice,
                                                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                            "", "",
                                                            objParametri_Server)

                        Dim Campo_POCO = EFCampi.Campo_Modifica_EF(objCampo, objParametri_Server, objParametri_Server.UsernameOperazione, objParametri_Utenti, GiasContext, False, NoteLog:=NoteLog)

                        'Dim mess = Check_PianoConcimazione_EntitaxTestata(objCampo, Campo_POCO, tipoOperazione, GiasContext, objParametri_Server, objParametri_Utenti)
                        'If mess <> "" Then
                        '    Throw New GiasException(mess)
                        'End If

                        If objCampo.validita.inizio <> campo_old.Rows(0)("Validita_Inizio") Or objCampo.validita.fine <> campo_old.Rows(0)("Validita_Fine") Then

                            Validita_UtentiXCampi_Modifica_Campo(objCampo,
                                                             tipoOperazione,
                                                             objParametri_Server,
                                                             objParametri_Utenti)

                            Validita_CampiXParticelle_Modifica_Campo(objCampo,
                                                                 tipoOperazione,
                                                                 objParametri_Server,
                                                                 objParametri_Utenti)

                            'Validita_Appezzamenti_Modifica_Campo(objCampo,
                            '                                     tipoOperazione,
                            '                                     objParametri_Server,
                            '                                     objParametri_Utenti)
                        End If
                        '--------------------------------------------------------------------------------------

                        Appezzamenti_ScriviModificaCancella_Campo(objCampo,
                                                              tipoOperazione,
                                                              objParametri_Server,
                                                              objParametri_Utenti)

                        CampiXParticelle_ScriviModificaCancella_Campo(objCampo,
                                                                  tipoOperazione,
                                                                  objParametri_Server,
                                                                  objParametri_Utenti)

                        CampiXCodici_ScriviModificaCancella_Campo_EF(objCampo,
                                                                 tipoOperazione,
                                                                 objParametri_Server,
                                                                 objParametri_Utenti,
                                                                 GiasContext,
                                                                 False,
                                                                 Campo_POCO)

                        If objCampo.cartografia IsNot Nothing AndAlso objCampo.cartografia <> "" Then
                            EntitaGrafiche_ScriviModifica_Campo_EF(
                            objCampo,
                            objParametri_Server,
                            objParametri_Server.UsernameOperazione,
                            EntitaCodxImg,
                            tipoOperazione,
                            GiasContext,
                            False,
                            Campo_POCO
                            )
                        End If

                    ElseIf (tipoOperazione = enum_TipoOperazioneDB.Cancellazione) Then

                        'Cancella Appezzamenti
                        If Not AppezzaDisaggregaSoloInCancellazione Then
                            Dim objAppezza_DAL As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
                            Dim objAppezza_BIZ_W As New AgronicaCoreAnagrafeBIZ.Appezzamento_W
                            Dim objAppezza_BIZ_R As New AgronicaCoreAnagrafeBIZ.Appezzamento_R
                            Dim dtAppezza = objAppezza_DAL.LeggiconCampo(objCampo.primaryKey.centroAziendalePK.partitaIva,
                                                     objCampo.primaryKey.centroAziendalePK.codice,
                                                     objCampo.primaryKey.codice,
                                                     enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                     "", "", objParametri_Server)
                            Dim i = 0
                            For Each rowAppezza In dtAppezza.Rows
                                Dim Piva As String = rowAppezza("Piva")
                                Dim Sa_Cod As Long = rowAppezza("Sa_Cod")
                                Dim Appezza As Long = rowAppezza("Appezza")

                                Dim appezzamento As New Appezzamento() With {
                                    .primaryKey = New Appezzamento.PK() With {
                                        .centroAziendalePK = New CentroAziendale.PK() With {
                                            .partitaIva = Piva,
                                            .codice = Sa_Cod
                                        },
                                    .codice = Appezza
                                    },
                                    .flag_cancellazione = True
                                }
                                ''Prima di poter eliminare controllo CdG e Movimenti
                                'Dim objControlloAgenda As New AgronicaCoreAnagrafeBIZ.Reg_Impianto_W
                                'Dim objControlloCdG As New AgronicaCoreAnagrafeBIZ.Progetto_W
                                'Dim controlloCdG = objControlloCdG.controllo_CdGxEliminazione(Nothing, Piva, Sa_Cod, Appezza, 0, 0, objParametri_Server)
                                'If objControlloAgenda.controllo_MovimentiRicettexEliminazione(Nothing, Piva, Sa_Cod, Appezza, 0, objParametri_Server) Then

                                '    Dim MessaggioErroreAgenda As String = ("Non è possibile eliminare il Campo " & objCampo.descrizione & ", poiché esistono Operazioni registrate su impianti ad esso associati!")
                                '    Throw New GiasException(MessaggioErroreAgenda)

                                'ElseIf controlloCdG.errore = True Then
                                '    Dim MessaggioErroreCdG As String = ("Non è possibile eliminare il Campo " & objCampo.descrizione & ", poiché esistono Costi di Gestione su esercizi ad esso associati!")
                                '    Throw New GiasException(MessaggioErroreCdG)

                                'End If

                                'Dim appezzamento = objAppezza_BIZ_R.Leggi_Appezzamento_Anagrafica(Piva,
                                '                                               Sa_Cod,
                                '                                               Appezza,
                                '                                               0,
                                '                                               True,
                                '                                               True,
                                '                                               True,
                                '                                               AGRODATAINIZIO,
                                '                                               False,
                                '                                               True,
                                '                                               False,
                                '                                               objParametri_Server,
                                '                                               objParametri_Server,
                                '                                               objParametri_Utenti)


                                objAppezza_BIZ_W.Appezzamento_ScriviModifica(appezzamento, objParametri_Server, objParametri_Utenti)

                                i = i + 1
                            Next

                        Else
                            'In cancellazione, la funzione fa solo la disaggregazione degli appezzamenti collegati al campo
                            Appezzamenti_ScriviModificaCancella_Campo(objCampo,
                                          tipoOperazione,
                                          objParametri_Server,
                                          objParametri_Utenti)
                        End If


                        CampiXParticelle_ScriviModificaCancella_Campo(objCampo,
                                                                      tipoOperazione,
                                                                      objParametri_Server,
                                                                      objParametri_Utenti)

                        UtentiXCampi_Cancella_Campo(objCampo,
                                                    tipoOperazione,
                                                    objParametri_Server,
                                                    objParametri_Utenti)

                        ProgrammazioneEntita_Cancella_Campo(objCampo,
                                                            tipoOperazione,
                                                            objParametri_Server,
                                                            objParametri_Utenti)

                        CampiXCodici_ScriviModificaCancella_Campo_EF(objCampo,
                                                                     tipoOperazione,
                                                                     objParametri_Server,
                                                                     objParametri_Utenti,
                                                                     GiasContext,
                                                                     False)

                        EntitaGrafiche_ScriviModifica_Campo_EF(
                        objCampo,
                        objParametri_Server,
                        objParametri_Server.UsernameOperazione,
                        EntitaCodxImg,
                        tipoOperazione,
                        GiasContext,
                        False)

                        EFCampi.Campo_Cancella_EF(objCampo, objParametri_Server, objParametri_Utenti, GiasContext, False, NoteLog:=NoteLog)

                    End If

                    scope.Complete()

                Catch ex As GiasException
                    If scope IsNot Nothing Then
                        scope.Dispose()
                    End If
                    MessaggioErrore = ex.Message
                    Throw ex
                Catch ex As Exception
                    scope.Dispose()
                    MessaggioErrore = ex.Message
                    Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)

                Finally

                    If GiasContext IsNot Nothing AndAlso GiasContext.Database.Connection.State = ConnectionState.Open Then
                        GiasContext.Database.Connection.Close()
                    End If

                End Try

            End Using
        End Using

        stWa.Stop()
        Dim totalTime As TimeSpan = stWa.Elapsed

        Return MessaggioErrore

    End Function

    Private Function UtentiXCampi_Scrivi_Campo(
            ByRef Campo As AgronicaCoreEntityFramework_POCO.Campi,
            ByVal tipoOperazione As enum_TipoOperazioneDB,
            ByRef objParametri_Server As AgronicaCoreParametri,
            ByRef objParametri_Utenti As AgronicaCoreParametri) As String

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.EFCampi.Funzione_Accessoria_Scrivi_Campo()"
        Dim MessaggioErrore As String = String.Empty

        Dim objUtentixCampi As New AgronicaCoreAnagrafeDAL.UtentixCampi_Write

        Try
            objUtentixCampi.Scrivi(Campo.Piva,
                                   Campo.Sa_Cod,
                                   Campo.Campo_Cod,
                                   Campo.Validita_Inizio,
                                   Campo.Validita_Fine,
                                   objParametri_Server)
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)
        End Try

        Return MessaggioErrore

    End Function

    Private Function Validita_UtentiXCampi_Modifica_Campo(
            ByRef objCampo As AgronicaCoreModelsSTD.anagrafiche.Campo,
            ByVal tipoOperazione As enum_TipoOperazioneDB,
            ByRef objParametri_Server As AgronicaCoreParametri,
            ByRef objParametri_Utenti As AgronicaCoreParametri) As String

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.EFCampi.Funzione_Accessoria_Scrivi_Campo()"
        Dim MessaggioErrore As String = String.Empty

        Dim objUtentixCampi As New AgronicaCoreAnagrafeDAL.UtentixCampi_Write

        Try
            objUtentixCampi.AggiornaValiditaInizio(objCampo.primaryKey.centroAziendalePK.partitaIva,
                                                   objCampo.primaryKey.centroAziendalePK.codice,
                                                   objCampo.primaryKey.codice,
                                                   objCampo.validita.inizio,
                                                   "",
                                                   objParametri_Server)

            objUtentixCampi.AggiornaValiditaFine(objCampo.primaryKey.centroAziendalePK.partitaIva,
                                                   objCampo.primaryKey.centroAziendalePK.codice,
                                                   objCampo.primaryKey.codice,
                                                   objCampo.validita.fine,
                                                   "",
                                                   objParametri_Server)
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)
        End Try

        Return MessaggioErrore

    End Function

    Private Function Check_PianoConcimazione_EntitaxTestata(ByRef objCampo As Campo,
                                                            ByRef Campo As AgronicaCoreEntityFramework_POCO.Campi,
                                                            Tipo_Operazione As enum_TipoOperazioneDB,
                                                            ByRef GiasContext As Gias_DeveloperServer_Entities,
                                                            ByRef objParametri_Server As AgronicaCoreParametri,
                                                            ByRef objParametri_Utenti As AgronicaCoreParametri) As String
        Dim messaggio As String = ""

        Dim listaAppErrori_Aggiunti As New List(Of String)
        Dim listaAppErrori_Rimossi As New List(Of String)


        Dim dic_AppOld As New Dictionary(Of (String, Integer, Integer), String)
        Dim dic_Inseriti As New Dictionary(Of (String, Integer, Integer), String)
        Dim dic_AppRimossi As New Dictionary(Of (String, Integer, Integer), String)

        Dim campo_cod As Integer = If(Tipo_Operazione = enum_TipoOperazioneDB.Scrittura, Campo.Campo_Cod, objCampo.primaryKey.codice)

        'PESCO TUTTI GLI APP COLLEGATI AL CAMPO PRIMA DELLA MODIFICA
        Dim objAppezzamentoR As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
        Dim appezzamenti_old = objAppezzamentoR.Recupera_Appezzamenti_Colture_del_Campo(objCampo.primaryKey.centroAziendalePK.partitaIva,
                                                                                        objCampo.primaryKey.centroAziendalePK.codice,
                                                                                        campo_cod,
                                                                                        AGRODATAINIZIO, AGRODATAFINE,
                                                                                        False, enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                                                        "", "",
                                                                                        objParametri_Server)

        'Mi salvo in un Dic tutti gli appezzamenti collegati prima della modifica
        For Each app_old In appezzamenti_old.Rows
            Dim old_piva As String = app_old.item("piva")
            Dim old_sa_cod As Integer = app_old.item("sa_cod")
            Dim old_appezza As Integer = app_old.item("appezza")
            Dim old_app_nome As String = app_old.item("app_nome")

            If Not (dic_AppOld.ContainsKey((old_piva, old_sa_cod, old_appezza))) Then
                dic_AppOld.Add((old_piva, old_sa_cod, old_appezza), old_app_nome)
            End If
        Next

        If objCampo.appezzamentoCampo IsNot Nothing Then
            'Mi salvo tutti gli appezzamenti appena scelti
            If objCampo.appezzamentoCampo IsNot Nothing Then
                For Each app In objCampo.appezzamentoCampo
                    Dim piva As String = app.piva
                    Dim sa_cod As Integer = app.sa_cod
                    Dim appezza As Integer = app.appezza
                    Dim app_nome As String = app.app_nome

                    If Not dic_Inseriti.ContainsKey((piva, sa_cod, appezza)) Then
                        dic_Inseriti.Add((piva, sa_cod, appezza), app_nome)
                    End If
                Next
            End If
        End If
        'Se sono stati rimossi tutti i collegamenti, aggiungo tutti gli app vecchi alla lista degli app rimossi
        If dic_Inseriti.Count = 0 AndAlso dic_AppOld.Count > 0 Then
            dic_AppRimossi = dic_AppOld
        Else
            'Confronto i vecchi con i nuovi, e mi salvo eventuali elementi mancanti
            For Each app_old In dic_AppOld
                For Each app In dic_Inseriti
                    If Not dic_Inseriti.ContainsKey((app_old.Key.Item1, app_old.Key.Item2, app_old.Key.Item3)) AndAlso
                            Not dic_AppRimossi.ContainsKey((app_old.Key.Item1, app_old.Key.Item2, app_old.Key.Item3)) Then
                        dic_AppRimossi.Add((app_old.Key.Item1, app_old.Key.Item2, app_old.Key.Item3), app_old.Value)
                    End If
                Next
            Next
        End If


        'CICLO SUGLI APPEZZAMENTI COLLEGATI APPENA INSERITI PER VEDERE SE ESISTONO DEI COLLEGAMENTI INCONGRUENTI CON I PIANI DI CONCIMAZIONE
        Dim PivaCampo = objCampo.primaryKey.centroAziendalePK.partitaIva
        Dim Sa_CodCampo = objCampo.primaryKey.centroAziendalePK.codice
        Dim PianoConcimazione_EntitaxTestata_t = (From pc In GiasContext.PianoConcimazione_EntitaxTestata Where pc.Piva = PivaCampo AndAlso pc.Sa_Cod = Sa_CodCampo).ToList()
        For Each app In dic_Inseriti
            Dim piva As String = app.Key.Item1
            Dim sa_cod As Integer = app.Key.Item2
            Dim appezza As Integer = app.Key.Item3
            Dim app_nome As String = app.Value

            'CERCO SE ESISTE UN PIANO CONCIMAZIONE ASSOCIATO
            Dim PianoConcimazione_EntitaxTestata = (From pc In PianoConcimazione_EntitaxTestata_t Where pc.Appezza = appezza).ToList()
            For Each piano In PianoConcimazione_EntitaxTestata
                'SE SUL PIANO CONCIMAZIONE L'APP E' COLLEGATO AD UN CAMPO_COD DIVERSO/NON E' COLLEGATO AD UN CAMPO, DO ERRORE
                If piano.Campo_Cod <> campo_cod AndAlso Not (listaAppErrori_Aggiunti.Contains(app_nome)) Then
                    listaAppErrori_Aggiunti.Add(app_nome)
                End If
            Next
        Next

        'CERCO SE ESISTONO PIANI DI CONCIMAZIONE ASSOCIATI AGLI APPEZZAMENTI RIMOSSI
        Dim PianoConcimazione_EntitaxTestata_t1 = (From pc In GiasContext.PianoConcimazione_EntitaxTestata Where pc.Piva = PivaCampo AndAlso pc.Sa_Cod = Sa_CodCampo).ToList()
        For Each app In dic_AppRimossi
            Dim piva As String = app.Key.Item1
            Dim sa_cod As Integer = app.Key.Item2
            Dim appezza As Integer = app.Key.Item3
            Dim app_nome As String = app.Value

            'SE NEL PIANO CONCIMAZIONE è SALVATO IL CAMPO_COD, IMPEDISCO LA MODIFICA
            Dim PianoConcimazione_EntitaxTestata = (From pc In PianoConcimazione_EntitaxTestata_t1 Where pc.Piva = piva AndAlso pc.Sa_Cod = sa_cod AndAlso pc.Appezza = appezza).ToList()
            For Each piano In PianoConcimazione_EntitaxTestata
                If piano.Campo_Cod = campo_cod AndAlso Not (listaAppErrori_Rimossi.Contains(app_nome)) Then
                    listaAppErrori_Rimossi.Add(app_nome)
                End If
            Next
        Next

        If listaAppErrori_Aggiunti.Count > 0 Then
            messaggio += (Gias.ImpossibileAggiungereSeguentiAppezzamentiCampoRisultanoAssociatiPianoConcimazione & ": " & String.Join("<br>- ", listaAppErrori_Aggiunti))
            messaggio += "<br>"
        End If

        If listaAppErrori_Rimossi.Count > 0 Then
            messaggio += (Gias.ImpossibileRimuovereSeguentiAppezzamentiCampoRisultanoAssociatiPianoConcimazione & ": " & String.Join("<br>- ", listaAppErrori_Rimossi))
        End If
        Return messaggio

    End Function

    Private Function Validita_CampiXParticelle_Modifica_Campo(
            ByRef objCampo As AgronicaCoreModelsSTD.anagrafiche.Campo,
            ByVal tipoOperazione As enum_TipoOperazioneDB,
            ByRef objParametri_Server As AgronicaCoreParametri,
            ByRef objParametri_Utenti As AgronicaCoreParametri) As String

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.EFCampi.Funzione_Accessoria_Scrivi_Campo()"
        Dim MessaggioErrore As String = String.Empty

        Dim objCampiXparticelle As New AgronicaCoreAnagrafeDAL.CampixParticelle_W

        Try
            objCampiXparticelle.AggiornaValidita(objCampo.primaryKey.centroAziendalePK.partitaIva,
                                             objCampo.primaryKey.centroAziendalePK.codice,
                                             objCampo.primaryKey.codice,
                                             objCampo.validita.inizio,
                                             objCampo.validita.fine,
                                             "",
                                             objParametri_Server)
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)
        End Try

        Return MessaggioErrore

    End Function

    Private Function CampiXCodici_ScriviModificaCancella_Campo_EF(
            ByRef objCampo As AgronicaCoreModelsSTD.anagrafiche.Campo,
            ByVal tipoOperazioneCampo As enum_TipoOperazioneDB,
            ByRef objParametri_Server As AgronicaCoreParametri,
            ByRef objParametri_Utenti As AgronicaCoreParametri,
            Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
            Optional ByVal NewTransaction As Boolean = True,
            Optional ByRef Campo As AgronicaCoreEntityFramework_POCO.Campi = Nothing) As String

        Dim Piva_SuperUser = objParametri_Server.PivaSuperUser

        Dim ObjSequenze = New Agro_Sequenze

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.EFCampi.Funzione_Accessoria_Scrivi_Campo()"
        Dim MessaggioErrore As String = String.Empty

        Dim bCloseContext As Boolean = False
        Dim scope As TransactionScope = Nothing

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri_Server.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If

        Dim objCampiXCodici As New AgronicaCoreAnagrafeDAL.Campi_Codici_Write
        Dim objCodici As New AgronicaCoreAnagrafeBIZ.Codici_W

        Try
            'TODO Salvo: CONTROLLARE CHE EFFETTIVAMENTE QUESTA FUNZIONE CANCELLI TUTTI I CODICI
            objCampiXCodici.Cancella(objCampo.primaryKey.centroAziendalePK.partitaIva,
                                     objCampo.primaryKey.centroAziendalePK.codice,
                                     If(tipoOperazioneCampo = enum_TipoOperazioneDB.Scrittura, Campo.Campo_Cod, objCampo.primaryKey.codice),
                                     0,
                                     "(id_cod < 2000 OR id_cod >= 3000)",
                                     objParametri_Server)

            'TODO Salvo: devo riguardare cosa fa la funzione qui sotto
            If tipoOperazioneCampo <> enum_TipoOperazioneDB.Cancellazione Then
                If IsNothing(Campo) Then
                    Throw New Exception("Non è stato possibile scrivere/modificare i codici campo perché Campo_POCO è Nothing")
                ElseIf objCampo.codici IsNot Nothing Then

                    For Each codice In objCampo.codici
                        objCodici.Scrivi_Codici_Campo_NoEF(
                            codice,
                            GiasContext,
                            Campo,
                            objParametri_Server,
                            objParametri_Utenti,
                            False)
                    Next
                End If

                If IsNothing(Campo) Then
                    Throw New Exception("Non è stato possibile scrivere/modificare i codici campo perché Campo_POCO è Nothing")
                Else

                    If objCampo.campo_Codice IsNot Nothing Then
                        Dim campo_codice As New CodiciAnagrafeValori()
                        campo_codice.valore = objCampo.campo_Codice
                        campo_codice.codiceAnagrafe = New CodiceAnagrafe()
                        campo_codice.codiceAnagrafe.codice = 1279 'questo è id_cod fisso per campo_Codice

                        objCodici.Scrivi_Codici_Campo_NoEF(
                            campo_codice,
                            GiasContext,
                            Campo,
                            objParametri_Server,
                            objParametri_Utenti,
                            False)
                    End If

                End If
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)
        End Try

        Return MessaggioErrore

    End Function

    Private Function CampiXParticelle_Campo(
            ByRef objCampo As AgronicaCoreModelsSTD.anagrafiche.Campo,
            ByVal tipoOperazioneCampo As enum_TipoOperazioneDB,
            ByRef objParametri_Server As AgronicaCoreParametri,
            ByRef objParametri_Utenti As AgronicaCoreParametri) As String

        Dim Piva_SuperUser = objParametri_Server.PivaSuperUser

        Dim ObjSequenze = New Agro_Sequenze

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.EFCampi.Funzione_Accessoria_Scrivi_Campo()"
        Dim MessaggioErrore As String = String.Empty

        Dim objCampiXParticelle As New AgronicaCoreAnagrafeDAL.CampixParticelle_W

        Try
            ' TODO Salvo: NON SO COME CANCELLARE AGILMENTE TUTTE LE ASSOCIAZIONI
            For Each particella In objCampo.catastoCampo
                objCampiXParticelle.Cancella_Associazione(objCampo.primaryKey.centroAziendalePK.partitaIva,
                        objCampo.primaryKey.centroAziendalePK.codice,
                        objCampo.primaryKey.codice,
                        particella.particella.primaryKey.Prov,
                        particella.particella.primaryKey.Com,
                        particella.particella.primaryKey.Sezione,
                        particella.particella.primaryKey.Foglio,
                        particella.particella.primaryKey.Numero,
                        particella.particella.primaryKey.Subalterno,
                        "",
                        objParametri_Server)
            Next

            If tipoOperazioneCampo <> enum_TipoOperazioneDB.Cancellazione Then
                For Each particella In objCampo.catastoCampo
                    objCampiXParticelle.Scrivi(objCampo.primaryKey.centroAziendalePK.partitaIva,
                            objCampo.primaryKey.centroAziendalePK.codice,
                            objCampo.primaryKey.codice,
                            particella.particella.primaryKey.Prov,
                            particella.particella.primaryKey.Com,
                            particella.particella.primaryKey.Sezione,
                            particella.particella.primaryKey.Foglio,
                            particella.particella.primaryKey.Numero,
                            particella.particella.primaryKey.Subalterno,
                            0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                            objCampo.validita.inizio,
                            objCampo.validita.fine,
                            objParametri_Server)
                Next
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)
        End Try

        Return MessaggioErrore

    End Function

    Private Function Validita_Appezzamenti_Modifica_Campo(
            ByRef objCampo As AgronicaCoreModelsSTD.anagrafiche.Campo,
            ByVal tipoOperazioneCampo As enum_TipoOperazioneDB,
            ByRef objParametri_Server As AgronicaCoreParametri,
            ByRef objParametri_Utenti As AgronicaCoreParametri) As String

        Dim Piva_SuperUser = objParametri_Server.PivaSuperUser

        Dim ObjSequenze = New Agro_Sequenze

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Campo_W.Validita_Appezzamenti_Modifica_Campo()"
        Dim MessaggioErrore As String = String.Empty

        Dim objAppezzamenti_W As New AgronicaCoreAnagrafeDAL.Appezzamento_Write
        Dim objAppezzamenti_R As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
        Dim objAgenda_R As New AgronicaCoreContabDAL.Mov_Destinazioni_R

        Try

            Dim dtAppezza = objAppezzamenti_R.LeggiconCampo(objCampo.primaryKey.centroAziendalePK.partitaIva,
                                                            objCampo.primaryKey.centroAziendalePK.codice,
                                                            objCampo.primaryKey.codice,
                                                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                            "", "", objParametri_Server)

            'Dim Validita_Inizio_Orig = objParametri_Server.FinestraTemporaleInizio
            'Dim Validita_Fine_Orig = objParametri_Server.FinestraTemporaleFine

            Dim Validita_Inizio = objCampo.validita.inizio
            Dim Validita_Fine = objCampo.validita.fine

            For Each RowAppezza In dtAppezza.Rows
                Dim piva = RowAppezza("piva")
                Dim sa_cod = RowAppezza("sa_cod")
                Dim appezza = RowAppezza("appezza")

                Dim validita_inizio_appezza As Date = RowAppezza("Validita_Inizio")
                Dim validita_fine_appezza As Date = RowAppezza("Validita_Fine")

                Dim Validita_InizioNew = Validita_Inizio
                Dim Validita_FineNew = Validita_Fine

                If validita_inizio_appezza > Validita_InizioNew Then
                    Validita_InizioNew = validita_inizio_appezza
                End If
                If Validita_FineNew > validita_fine_appezza Then
                    Validita_FineNew = validita_fine_appezza
                End If

                Dim objAppsR As New AgronicaCoreAnagrafeBIZ.Appezzamento_R
                Dim appR = objAppsR.Leggi_Appezzamento_Anagrafica(Piva:=piva,
                                                                  Sa_Cod:=sa_cod,
                                                                  Appezza:=appezza,
                                                                  0,
                                                                  Leggi_Impianti:=True,
                                                                  Leggi_Indirizzi:=True,
                                                                  Leggi_Catasto:=True,
                                                                  data:=AGRODATAINIZIO,
                                                                  filtroData:=False,
                                                                  Leggi_Distinte:=True,
                                                                  Leggi_Cartografia:=False,
                                                                  objParametri_Server,
                                                                  objParametri_Server,
                                                                  objParametri_Utenti
                                                                  )

                appR.validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(Validita_InizioNew, Validita_FineNew)

                Dim objAppsW As New AgronicaCoreAnagrafeBIZ.Appezzamento_W
                Dim appW = objAppsW.Appezzamento_ScriviModifica(appR, objParametri_Server, objParametri_Utenti)

                'If objCampo.validita.inizio > validita_inizio_appezza Then
                '    objParametri_Server.FinestraTemporaleInizio = validita_inizio_appezza
                '    objParametri_Server.FinestraTemporaleFine = objCampo.validita.inizio
                '    Dim dt = objAgenda_R.Leggi_conImpianti(objCampo.primaryKey.centroAziendalePK.partitaIva,
                '                                            objCampo.primaryKey.centroAziendalePK.codice,
                '                                            0, 0, 0, RowAppezza("Appezza"),
                '                                            0, "", "", objParametri_Server)
                '    If dt.Rows.Count > 0 Then
                '        objParametri_Server.FinestraTemporaleInizio = Validita_Inizio_Orig
                '        objParametri_Server.FinestraTemporaleFine = Validita_Fine_Orig
                '        Throw New Exception("Sono presenti operazioni di agenda precedenti alla chiusura del campo per l'appezzamento " & CStr(RowAppezza("App_Nome")))
                '    End If
                'End If

                'If objCampo.validita.fine < validita_fine_appezza Then
                '    objParametri_Server.FinestraTemporaleInizio = objCampo.validita.fine
                '    objParametri_Server.FinestraTemporaleFine = validita_fine_appezza
                '    Dim dt = objAgenda_R.Leggi_conImpianti(objCampo.primaryKey.centroAziendalePK.partitaIva,
                '                                           objCampo.primaryKey.centroAziendalePK.codice,
                '                                           0, 0, 0, RowAppezza("Appezza"),
                '                                           0, "", "", objParametri_Server)
                '    If dt.Rows.Count > 0 Then
                '        objParametri_Server.FinestraTemporaleInizio = Validita_Inizio_Orig
                '        objParametri_Server.FinestraTemporaleFine = Validita_Fine_Orig
                '        Throw New Exception("Sono presenti operazioni di agenda successive alla chiusura del campo per l'appezzamento " & CStr(RowAppezza("App_Nome")))
                '    End If
                'End If

            Next

            For Each app In objCampo.appezzamentoCampo
                app.validita.inizio = objCampo.validita.inizio
                app.validita.fine = objCampo.validita.fine
            Next

            Aggiorna_ValiditaAppezzamenti_Campo(objCampo,
                                                tipoOperazioneCampo,
                                                objParametri_Server,
                                                objParametri_Utenti)
            Aggiorna_ValiditaUtentiXAppezzamenti_Campo(objCampo,
                                                       tipoOperazioneCampo,
                                                       objParametri_Server,
                                                       objParametri_Utenti)
            Aggiorna_ValiditaAppezzamentiXParticelle_Campo(objCampo,
                                                           tipoOperazioneCampo,
                                                           objParametri_Server,
                                                           objParametri_Utenti)
            Aggiorna_ValiditaReg_Impianti_Campo(objCampo,
                                                tipoOperazioneCampo,
                                                objParametri_Server,
                                                objParametri_Utenti)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)
        End Try

        Return MessaggioErrore

    End Function

    Private Function Aggiorna_ValiditaAppezzamenti_Campo(
            ByRef objCampo As AgronicaCoreModelsSTD.anagrafiche.Campo,
            ByVal tipoOperazioneCampo As enum_TipoOperazioneDB,
            ByRef objParametri_Server As AgronicaCoreParametri,
            ByRef objParametri_Utenti As AgronicaCoreParametri) As String

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.EFCampi.Funzione_Accessoria_Scrivi_Campo()"
        Dim MessaggioErrore As String = String.Empty

        Dim objAppezzamenti_W As New AgronicaCoreAnagrafeDAL.Appezzamento_Write

        Try

            objAppezzamenti_W.AggiornaValiditaInizio(objCampo.primaryKey.centroAziendalePK.partitaIva,
                                                     objCampo.primaryKey.centroAziendalePK.codice,
                                                     objCampo.primaryKey.codice,
                                                     0,
                                                     objCampo.validita.inizio,
                                                     "",
                                                     objParametri_Server)

            objAppezzamenti_W.AggiornaValiditaFine(objCampo.primaryKey.centroAziendalePK.partitaIva,
                                                     objCampo.primaryKey.centroAziendalePK.codice,
                                                     objCampo.primaryKey.codice,
                                                     0,
                                                     objCampo.validita.fine,
                                                     "",
                                                     objParametri_Server)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)
        End Try

        Return MessaggioErrore

    End Function

    Private Function Aggiorna_ValiditaUtentiXAppezzamenti_Campo(
            ByVal objCampo As AgronicaCoreModelsSTD.anagrafiche.Campo,
            ByVal tipoOperazioneCampo As enum_TipoOperazioneDB,
            ByRef objParametri_Server As AgronicaCoreParametri,
            ByRef objParametri_Utenti As AgronicaCoreParametri) As String

        Dim Piva_SuperUser = objParametri_Server.PivaSuperUser

        Dim ObjSequenze = New Agro_Sequenze

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.EFCampi.Funzione_Accessoria_Scrivi_Campo()"
        Dim MessaggioErrore As String = String.Empty

        Dim objUtentixAppezzamenti As New AgronicaCoreAnagrafeDAL.UtentixAppezzamenti_W

        Try

            objUtentixAppezzamenti.AggiornaValiditaInizio(objCampo.primaryKey.centroAziendalePK.partitaIva,
                                                          objCampo.primaryKey.centroAziendalePK.codice,
                                                          objCampo.primaryKey.codice,
                                                          0,
                                                          objCampo.validita.inizio,
                                                          "",
                                                          objParametri_Server)

            objUtentixAppezzamenti.AggiornaValiditaFine(objCampo.primaryKey.centroAziendalePK.partitaIva,
                                                        objCampo.primaryKey.centroAziendalePK.codice,
                                                        objCampo.primaryKey.codice,
                                                        0,
                                                        objCampo.validita.fine,
                                                        "",
                                                        objParametri_Server)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)
        End Try

        Return MessaggioErrore

    End Function

    Private Function Aggiorna_ValiditaAppezzamentiXParticelle_Campo(
        ByVal objCampo As AgronicaCoreModelsSTD.anagrafiche.Campo,
        ByVal tipoOperazioneCampo As enum_TipoOperazioneDB,
        ByRef objParametri_Server As AgronicaCoreParametri,
        ByRef objParametri_Utenti As AgronicaCoreParametri) As String

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.EFCampi.Funzione_Accessoria_Scrivi_Campo()"
        Dim MessaggioErrore As String = String.Empty

        Dim objAppezzaxParticelle As New AgronicaCoreAnagrafeDAL.AppezzaxParticelle_W

        Try

            objAppezzaxParticelle.AggiornaValiditaInizio(objCampo.primaryKey.centroAziendalePK.partitaIva,
                                                         objCampo.primaryKey.centroAziendalePK.codice,
                                                         objCampo.primaryKey.codice,
                                                         0,
                                                         "",
                                                         "",
                                                         "",
                                                         0, 0, "", objCampo.validita.inizio,
                                                         "",
                                                         objParametri_Server)

            objAppezzaxParticelle.AggiornaValiditaFine(objCampo.primaryKey.centroAziendalePK.partitaIva,
                                                        objCampo.primaryKey.centroAziendalePK.codice,
                                                        objCampo.primaryKey.codice,
                                                        0,
                                                        "", "", "", 0, 0, "", objCampo.validita.fine,
                                                        "",
                                                        objParametri_Server)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)
        End Try

        Return MessaggioErrore

    End Function

    Private Function Aggiorna_ValiditaReg_Impianti_Campo(
            ByVal objCampo As AgronicaCoreModelsSTD.anagrafiche.Campo,
            ByVal tipoOperazioneCampo As enum_TipoOperazioneDB,
            ByRef objParametri_Server As AgronicaCoreParametri,
            ByRef objParametri_Utenti As AgronicaCoreParametri) As String

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.EFCampi.Funzione_Accessoria_Scrivi_Campo()"
        Dim MessaggioErrore As String = String.Empty

        Dim objReg_Impianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Write

        Try

            objReg_Impianti.AggiornaValiditaInizio(objCampo.primaryKey.centroAziendalePK.partitaIva,
                                                   objCampo.primaryKey.centroAziendalePK.codice,
                                                   0,
                                                   objCampo.primaryKey.codice,
                                                   0,
                                                   objCampo.validita.inizio,
                                                   objParametri_Server)

            objReg_Impianti.AggiornaValiditaFine(objCampo.primaryKey.centroAziendalePK.partitaIva,
                                                 objCampo.primaryKey.centroAziendalePK.codice,
                                                 0,
                                                 objCampo.primaryKey.codice,
                                                 0,
                                                 objCampo.validita.fine,
                                                 objParametri_Server)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)
        End Try

        Return MessaggioErrore

    End Function

    Private Function Appezzamenti_Cancella_Campo(
            ByRef objCampo As AgronicaCoreModelsSTD.anagrafiche.Campo,
            ByVal tipoOperazioneCampo As enum_TipoOperazioneDB,
            ByRef objParametri_Server As AgronicaCoreParametri,
            ByRef objParametri_Utenti As AgronicaCoreParametri) As String

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.EFCampi.Funzione_Accessoria_Scrivi_Campo()"
        Dim MessaggioErrore As String = String.Empty

        Try

            'TODO Salvo:
            '########################################################################################################
            '#################### DA RICHIAMARE LA FUNZIONE DI CANCELLAZIONE PER GLI APPEZZAMENTI ###################
            '########################################################################################################

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)
        End Try

        Return MessaggioErrore

    End Function

    Private Function CampiXParticelle_ScriviModificaCancella_Campo(
            ByRef objCampo As AgronicaCoreModelsSTD.anagrafiche.Campo,
            ByVal tipoOperazioneCampo As enum_TipoOperazioneDB,
            ByRef objParametri_Server As AgronicaCoreParametri,
            ByRef objParametri_Utenti As AgronicaCoreParametri,
            Optional ByRef Campo As AgronicaCoreEntityFramework_POCO.Campi = Nothing) As String

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.EFCampi.Funzione_Accessoria_Scrivi_Campo()"
        Dim MessaggioErrore As String = String.Empty

        Dim objCampixParticelle As New AgronicaCoreAnagrafeDAL.CampixParticelle_W

        Try

            objCampixParticelle.Cancella(objCampo.primaryKey.centroAziendalePK.partitaIva,
                                         objCampo.primaryKey.centroAziendalePK.codice,
                                         If(tipoOperazioneCampo = enum_TipoOperazioneDB.Scrittura, Campo.Campo_Cod, objCampo.primaryKey.codice),
                                         "",
                                         "",
                                         "",
                                         0,
                                         0,
                                         "",
                                         "",
                                         objParametri_Server)

            If tipoOperazioneCampo <> enum_TipoOperazioneDB.Cancellazione AndAlso objCampo.catastoCampo IsNot Nothing Then
                'TODO Salvo: sistemare scrittura ettari, are, centiare
                For Each part In objCampo.catastoCampo
                    Dim ettari = 0
                    Dim are = 0
                    Dim centiare = 0
                    Conversioni.EttariAreCentiare_from_Ettari(part.area, ettari, are, centiare)
                    objCampixParticelle.Scrivi(objCampo.primaryKey.centroAziendalePK.partitaIva,
                                               objCampo.primaryKey.centroAziendalePK.codice,
                                               If(tipoOperazioneCampo = enum_TipoOperazioneDB.Scrittura, Campo.Campo_Cod, objCampo.primaryKey.codice),
                                               part.particella.primaryKey.Prov,
                                               part.particella.primaryKey.Com,
                                               part.particella.primaryKey.Sezione,
                                               part.particella.primaryKey.Foglio,
                                               part.particella.primaryKey.Numero,
                                               part.particella.primaryKey.Subalterno,
                                               ettari,
                                               are,
                                               centiare,
                                               CDbl(0),
                                               CInt(0),
                                               CInt(0),
                                               CDbl(0),
                                               CInt(0),
                                               CInt(0),
                                               part.area, 'superficie intersezione
                                               objCampo.validita.inizio,
                                               objCampo.validita.fine,
                                               objParametri_Server)
                Next
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)
        End Try

        Return MessaggioErrore

    End Function

    Private Function UtentiXCampi_Cancella_Campo(
            ByRef objCampo As AgronicaCoreModelsSTD.anagrafiche.Campo,
            ByVal tipoOperazioneCampo As enum_TipoOperazioneDB,
            ByRef objParametri_Server As AgronicaCoreParametri,
            ByRef objParametri_Utenti As AgronicaCoreParametri) As String

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.EFCampi.Funzione_Accessoria_Scrivi_Campo()"
        Dim MessaggioErrore As String = String.Empty

        Dim objUtentixCampi As New AgronicaCoreAnagrafeDAL.UtentixCampi_Write

        Try

            objUtentixCampi.Cancella(objCampo.primaryKey.centroAziendalePK.partitaIva,
                                     objCampo.primaryKey.centroAziendalePK.codice,
                                     objCampo.primaryKey.codice,
                                     "",
                                     objParametri_Server)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)
        End Try

        Return MessaggioErrore

    End Function

    Private Function ProgrammazioneEntita_Cancella_Campo(
            ByRef objCampo As AgronicaCoreModelsSTD.anagrafiche.Campo,
            ByVal tipoOperazioneCampo As enum_TipoOperazioneDB,
            ByRef objParametri_Server As AgronicaCoreParametri,
            ByRef objParametri_Utenti As AgronicaCoreParametri) As String

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Campo_W.ProgrammazioneEntita_Cancella_Campo()"
        Dim MessaggioErrore As String = String.Empty

        Dim objPE As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_W

        Try
            objPE.Azzera_Campo_Cod(objCampo.primaryKey.centroAziendalePK.partitaIva,
                                   objCampo.primaryKey.centroAziendalePK.codice,
                                   objCampo.primaryKey.codice,
                                   "",
                                   objParametri_Server)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)
        End Try

        Return MessaggioErrore

    End Function

    Private Function Appezzamenti_ScriviModificaCancella_Campo(
            ByRef objCampo As AgronicaCoreModelsSTD.anagrafiche.Campo,
            ByVal tipoOperazioneCampo As enum_TipoOperazioneDB,
            ByRef objParametri_Server As AgronicaCoreParametri,
            ByRef objParametri_Utenti As AgronicaCoreParametri,
            Optional ByRef Campo As AgronicaCoreEntityFramework_POCO.Campi = Nothing) As String

        Dim Piva_SuperUser = objParametri_Server.PivaSuperUser

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Campo_W.Appezzamenti_ScriviModificaCancella_Campo()"
        Dim MessaggioErrore As String = String.Empty

        Try
            'TODO Salvo:
            '########################################################################################################
            '#################### DA RICHIAMARE LA FUNZIONE DI CANCELLAZIONE PER GLI APPEZZAMENTI ###################
            '########################################################################################################

            Dim appezzamenti_new As List(Of AppezzamentoCampo)
            Dim objAppezzamentoR As New AgronicaCoreAnagrafeDAL.Appezzamento_Read

            Dim objCampiW As New AgronicaCoreAnagrafeDAL.Campi_W

            Dim appezzamenti_old = objAppezzamentoR.Recupera_Appezzamenti_Colture_del_Campo(
                    objCampo.primaryKey.centroAziendalePK.partitaIva,
                    objCampo.primaryKey.centroAziendalePK.codice,
                    If(tipoOperazioneCampo = enum_TipoOperazioneDB.Scrittura, Campo.Campo_Cod, objCampo.primaryKey.codice),
                    AGRODATAINIZIO,
                    AGRODATAFINE,
                    False,
                    enumSelezioneVariabile.Selezione_JoinDescrizioni,
                    "",
                    "",
                    objParametri_Server)

            Dim PivaCampo = objCampo.primaryKey.centroAziendalePK.partitaIva
            Dim Sa_CodCampo = objCampo.primaryKey.centroAziendalePK.codice

            Dim appezzamenti_old_list As List(Of String) = (From a In appezzamenti_old.Rows Select CStr(a("Piva") & "_" & a("Sa_Cod") & "_" & a("Appezza"))).ToList()

            Dim appezzamenti_new_list As List(Of String) = New List(Of String)
            If objCampo.appezzamentoCampo IsNot Nothing Then
                appezzamenti_new_list = (From a In objCampo.appezzamentoCampo Select CStr(PivaCampo & "_" &
                                                                                                     Sa_CodCampo & "_" &
                                                                                                     a.appezza)).ToList()
            End If

            If tipoOperazioneCampo = enum_TipoOperazioneDB.Cancellazione Then
                For i = 0 To appezzamenti_old.Rows.Count - 1
                    objCampiW.Disaggrega(objCampo.primaryKey.centroAziendalePK.partitaIva,
                                         objCampo.primaryKey.centroAziendalePK.codice,
                                         If(tipoOperazioneCampo = enum_TipoOperazioneDB.Scrittura, Campo.Campo_Cod, objCampo.primaryKey.codice),
                                         appezzamenti_old.Rows(i).Item("Appezza"),
                                         appezzamenti_old.Rows(i).Item("Validita_Inizio"),
                                         appezzamenti_old.Rows(i).Item("Validita_Fine"),
                                         objParametri_Server)
                Next
            End If

            If tipoOperazioneCampo <> enum_TipoOperazioneDB.Cancellazione AndAlso objCampo.appezzamentoCampo IsNot Nothing Then

                Dim appezzamenti_Disaggrega = (From el In appezzamenti_old_list Where Not appezzamenti_new_list.Contains(el)).ToList

                Dim appezzamenti_Aggrega = (From el In appezzamenti_new_list Where Not appezzamenti_old_list.Contains(el)).ToList


                For i = 0 To appezzamenti_Disaggrega.Count - 1
                    Dim appezza = appezzamenti_Disaggrega(i).Split("_")(2)
                    objCampiW.Disaggrega(objCampo.primaryKey.centroAziendalePK.partitaIva,
                                         objCampo.primaryKey.centroAziendalePK.codice,
                                         If(tipoOperazioneCampo = enum_TipoOperazioneDB.Scrittura, Campo.Campo_Cod, objCampo.primaryKey.codice),
                                         appezza,
                                         objCampo.validita.inizio,
                                         objCampo.validita.fine,
                                         objParametri_Server)
                Next

                appezzamenti_new = objCampo.appezzamentoCampo
                For j = 0 To appezzamenti_Aggrega.Count - 1
                    Dim appezza = appezzamenti_Aggrega(j).Split("_")(2)
                    objCampiW.Aggrega(objCampo.primaryKey.centroAziendalePK.partitaIva,
                                          objCampo.primaryKey.centroAziendalePK.codice,
                                          If(tipoOperazioneCampo = enum_TipoOperazioneDB.Scrittura, Campo.Campo_Cod, objCampo.primaryKey.codice),
                                          appezza,
                                          objCampo.validita.inizio,
                                          objCampo.validita.fine,
                                          objParametri_Server)
                Next

            End If

        Catch ex As GiasException
            MessaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)
        End Try

        Return MessaggioErrore

    End Function


    Private Function Appezzamenti_ScriviModificaCancella_Campo2(
            ByRef objCampo As AgronicaCoreModelsSTD.anagrafiche.Campo,
            ByVal tipoOperazioneCampo As enum_TipoOperazioneDB,
            ByRef objParametri_Server As AgronicaCoreParametri,
            ByRef objParametri_Utenti As AgronicaCoreParametri,
            Optional ByRef Campo As AgronicaCoreEntityFramework_POCO.Campi = Nothing) As String

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.EFCampi.Funzione_Accessoria_Scrivi_Campo()"
        Dim MessaggioErrore As String = String.Empty

        Try
            'TODO Salvo:
            '########################################################################################################
            '#################### DA RICHIAMARE LA FUNZIONE DI CANCELLAZIONE PER GLI APPEZZAMENTI ###################
            '########################################################################################################

            Dim appezzamenti_new As List(Of AppezzamentoCampo)
            Dim objAppezzamentoR As New AgronicaCoreAnagrafeDAL.Appezzamento_Read

            Dim objCampiW As New AgronicaCoreAnagrafeDAL.Campi_W

            Dim appezzamenti_old = objAppezzamentoR.Recupera_Appezzamenti_Colture_del_Campo(
                    objCampo.primaryKey.centroAziendalePK.partitaIva,
                    objCampo.primaryKey.centroAziendalePK.codice,
                    If(tipoOperazioneCampo = enum_TipoOperazioneDB.Scrittura, Campo.Campo_Cod, objCampo.primaryKey.codice),
                    AGRODATAINIZIO,
                    AGRODATAFINE,
                    False,
                    enumSelezioneVariabile.Selezione_JoinDescrizioni,
                    "",
                    "",
                    objParametri_Server)

            For i = 0 To appezzamenti_old.Rows.Count - 1
                objCampiW.Disaggrega(objCampo.primaryKey.centroAziendalePK.partitaIva,
                                     objCampo.primaryKey.centroAziendalePK.codice,
                                     If(tipoOperazioneCampo = enum_TipoOperazioneDB.Scrittura, Campo.Campo_Cod, objCampo.primaryKey.codice),
                                     appezzamenti_old.Rows(i).Item("Appezza"),
                                     appezzamenti_old.Rows(i).Item("Validita_Inizio"),
                                     appezzamenti_old.Rows(i).Item("Validita_Fine"),
                                     objParametri_Server)
            Next

            If tipoOperazioneCampo <> enum_TipoOperazioneDB.Cancellazione AndAlso objCampo.appezzamentoCampo IsNot Nothing Then
                appezzamenti_new = objCampo.appezzamentoCampo
                For Each app In appezzamenti_new

                    'If app.validita.inizio < objCampo.validita.inizio Then

                    '    Throw New GiasException("La Validità Inizio Non Comprende Alcuni Appezzamenti Selezionati")

                    'End If

                    'If app.validita.fine > objCampo.validita.fine Then

                    '    Throw New GiasException("La Validità Finale Non Comprende Alcuni Appezzamenti Selezionati")

                    'End If

                    objCampiW.Aggrega(objCampo.primaryKey.centroAziendalePK.partitaIva,
                                      objCampo.primaryKey.centroAziendalePK.codice,
                                      If(tipoOperazioneCampo = enum_TipoOperazioneDB.Scrittura, Campo.Campo_Cod, objCampo.primaryKey.codice),
                                      app.appezza,
                                      app.validita.inizio,
                                      app.validita.fine,
                                      objParametri_Server)
                Next
            End If

        Catch ex As GiasException
            MessaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)
        End Try

        Return MessaggioErrore

    End Function


    Private Function Verifica_ValiditaInizioFine(ByRef campo As AgronicaCoreModelsSTD.anagrafiche.Campo,
                                                 ByRef objParametri_Server As AgronicaCoreParametri,
                                                 ByRef objParametri_Utenti As AgronicaCoreParametri)

        Dim MessaggioErrore As String = ""

        Dim Validita_Fine = campo.validita.fine
        Dim Validita_Inizio = campo.validita.inizio

        Dim sa_cod = campo.primaryKey.centroAziendalePK.codice
        Dim piva = campo.primaryKey.centroAziendalePK.partitaIva
        Dim campo_cod = campo.primaryKey.codice

        Dim objCentriR As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
        Dim centro = objCentriR.Leggi(piva, sa_cod, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

        Dim objCampoR As New AgronicaCoreAnagrafeDAL.Campi_R
        Dim camp = objCampoR.Leggi(piva, sa_cod, campo_cod, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

        Dim objAppR As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
        Dim app = objAppR.LeggiconCampo(piva, sa_cod, campo_cod, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

        Try

            If camp.Rows.Count > 0 Then

                'Se l'intervallo delle validità viene modificato, controllo e aggiorno, se necessario, l'Appezzamento
                If Validita_Inizio <> camp.Rows(0)("Validita_Inizio") Or Validita_Fine <> camp.Rows(0)("Validita_Fine") Then

                    If app.Rows.Count > 0 Then
                        For Each a In app.Rows
                            Dim appezza = a("appezza")

                            'Ripristino le date dopo ogni iterazione
                            Dim Validita_Inizio_AppNew = Validita_Inizio
                            Dim Validita_Fine_AppNew = Validita_Fine

                            'Controllo se le date dell'Appezzamento rientrano nell'intervallo temporale del Centro, in questo caso rimangono invariate
                            Dim dateAppezzaModificate As Boolean = True
                            If Validita_Inizio_AppNew < a("Validita_Inizio") Then
                                Validita_Inizio_AppNew = a("Validita_Inizio")
                                dateAppezzaModificate = False
                            Else
                                dateAppezzaModificate = True
                            End If
                            If Validita_Fine_AppNew > a("Validita_Fine") Then
                                Validita_Fine_AppNew = a("Validita_Fine")
                                dateAppezzaModificate = False
                            Else
                                dateAppezzaModificate = True
                            End If

                            If dateAppezzaModificate Then
                                Dim objAppsR As New AgronicaCoreAnagrafeBIZ.Appezzamento_R
                                Dim appR = objAppsR.Leggi_Appezzamento_Anagrafica(Piva:=piva,
                                                                              Sa_Cod:=sa_cod,
                                                                              Appezza:=appezza,
                                                                              0,
                                                                              Leggi_Impianti:=True,
                                                                              Leggi_Indirizzi:=True,
                                                                              Leggi_Catasto:=True,
                                                                              data:=AGRODATAINIZIO,
                                                                              filtroData:=False,
                                                                              Leggi_Distinte:=True,
                                                                              Leggi_Cartografia:=False,
                                                                              objParametri_Server,
                                                                              objParametri_Server,
                                                                              objParametri_Utenti
                                                                              )

                                appR.validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(Validita_Inizio_AppNew, Validita_Fine_AppNew)

                                '------------------------
                                'CONTROLLO DATE IMPIANTI
                                '------------------------
                                Dim impiantiDaEliminare As New List(Of AgronicaCoreModelsSTD.anagrafiche.Impianto)
                                Dim eserciziDaEliminare As New List(Of AgronicaCoreModelsSTD.anagrafiche.Esercizio)

                                For Each impianto In appR.impianti

                                    'Ripristino le date dopo ogni iterazione
                                    Dim Validita_Inizio_Impianto = Validita_Inizio_AppNew
                                    Dim Validita_Fine_Impianto = Validita_Fine_AppNew

                                    'se la data di inizio dell'impianto è SUCCESSIVA alla FINE dell'Appezzamento, elimino l'Impianto
                                    'se la data di fine dell'impianto è PRECEDENTE all'INIZIO dell'Appezzamento, elimino l'Impianto
                                    If impianto.validita.inizio > Validita_Fine_AppNew OrElse
                                       impianto.validita.fine < Validita_Inizio_AppNew Then
                                        impiantiDaEliminare.Add(impianto)

                                        'Controllo se le date dell'Impianto rientrano nell'intervallo temporale dell'Appezzamento, in questo caso rimangono invariate
                                    ElseIf impianto.validita.inizio < Validita_Inizio_AppNew AndAlso impianto.validita.fine <= Validita_Fine_AppNew Then
                                        If impianto.validita.inizio > Validita_Inizio_AppNew Then
                                            Validita_Inizio_Impianto = impianto.validita.inizio
                                        End If
                                        If impianto.validita.fine < Validita_Fine_AppNew Then
                                            Validita_Fine_Impianto = impianto.validita.fine
                                        End If
                                    End If

                                    'aggiorno le validità solo se l'Impianto non è stato cancellato
                                    If Not impiantiDaEliminare.Contains(impianto) Then
                                        impianto.validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(Validita_Inizio_Impianto, Validita_Fine_Impianto)
                                    End If


                                    '------------------------
                                    'CONTROLLO DATE ESERCIZI
                                    '------------------------
                                    For Each esercizio In impianto.esercizi

                                        'Ripristino le date dopo ogni iterazione
                                        Dim Validita_Inizio_Esercizio = Validita_Inizio_Impianto
                                        Dim Validita_Fine_Esercizio = Validita_Fine_Impianto

                                        'se la data di inizio dell'Esercizio è SUCCESSIVA alla FINE dell'Impianto, elimino l'Esercizio
                                        'se la data di fine dell'Esercizio è PRECEDENTE all'INIZIO dell'Impianto, elimino l'Esercizio
                                        If esercizio.validita.inizio > Validita_Fine_Impianto OrElse
                                           esercizio.validita.fine < Validita_Inizio_Impianto Then
                                            eserciziDaEliminare.Add(esercizio)

                                            'Controllo se le date dell'Esercizio rientrano nell'intervallo temporale dell'Impianto, in questo caso rimangono invariate
                                        ElseIf esercizio.validita.inizio >= Validita_Inizio_Impianto AndAlso esercizio.validita.fine <= Validita_Fine_Impianto Then
                                            If esercizio.validita.inizio > Validita_Inizio_Impianto Then
                                                Validita_Inizio_Esercizio = esercizio.validita.inizio
                                            End If
                                            If esercizio.validita.fine < Validita_Fine_Impianto Then
                                                Validita_Fine_Esercizio = esercizio.validita.fine
                                            End If
                                        End If

                                        'aggiorno le validità solo se l'Esercizio non è stato cancellato
                                        If Not eserciziDaEliminare.Contains(esercizio) Then
                                            esercizio.validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(Validita_Inizio_Esercizio, Validita_Fine_Esercizio)
                                        End If
                                    Next

                                    For Each esercizio In eserciziDaEliminare
                                        impianto.esercizi.Remove(esercizio)
                                    Next
                                Next

                                For Each impianto In impiantiDaEliminare
                                    appR.impianti.Remove(impianto)
                                Next

                                Dim objAppsW As New AgronicaCoreAnagrafeBIZ.Appezzamento_W
                                Dim appW = objAppsW.Appezzamento_ScriviModifica(appR,
                                                                                objParametri_Server,
                                                                                objParametri_Utenti,
                                                                                AggiornaSoloValidita:=True)
                            End If
                        Next
                    End If

                    'Controlli con  Centro Aziendale
                    If Validita_Inizio < CDate(centro.Rows(0)("Validita_Inizio")) Then
                        MessaggioErrore += ("L'inizio  non può precedere la creazione del Centro Aziendale.") & " (" & CDate(centro.Rows(0)("Validita_Inizio")).ToShortDateString & ")."
                        Throw New GiasException(MessaggioErrore)
                    End If

                    If Validita_Fine > CDate(centro.Rows(0)("Validita_Fine")) Then
                        MessaggioErrore += ("La fine del Campo non può seguire la cessazione del Centro Aziendale") & " (" & CDate(centro.Rows(0)("Validita_Fine")).ToShortDateString & ")."
                        Throw New GiasException(MessaggioErrore)
                    End If
                End If
            End If

            If Validita_Fine < Validita_Inizio Then
                MessaggioErrore += ("La fine del Campo non può precedere la sua data di inizio.")
                Throw New GiasException(MessaggioErrore)
            End If

        Catch ex As GiasException
            MessaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception(MessaggioErrore)
        End Try

        Return MessaggioErrore
    End Function

    Private Sub EntitaGrafiche_ScriviModifica_Campo_EF(
                     dati_campo As AgronicaCoreModelsSTD.anagrafiche.Campo,
                     ByRef objParametri As AgronicaCoreParametri,
                     ByVal username As String,
                     ByRef EntitaCodXImg As List(Of Integer),
                     ByVal tipoOperazioneCampo As enum_TipoOperazioneDB,
                     Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                     Optional ByVal NewTransaction As Boolean = True,
                     Optional ByVal Campo As AgronicaCoreEntityFramework_POCO.Campi = Nothing
                     )

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Campo_W.EntitaGrafiche_ScriviModifica_Campo_EF()"
        Dim messaggioErrore As String = ""

        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If
        Try
            Dim modelEntita = New AgronicaCoreGisDAL.Entita
            Dim modelElementoGrafico = New AgronicaCoreGisDAL.ElementiGrafici

            Select Case tipoOperazioneCampo
                Case enum_TipoOperazioneDB.Scrittura, enum_TipoOperazioneDB.Modifica

                    Dim entitaCampoList = From entCampo In GiasContext.GIS_Entita
                                          Where entCampo.Piva = dati_campo.primaryKey.centroAziendalePK.partitaIva AndAlso
                                              entCampo.Sa_Cod = dati_campo.primaryKey.centroAziendalePK.codice AndAlso
                                              entCampo.Appezza = 0 AndAlso
                                              entCampo.Id_Imp = 0 AndAlso
                                              entCampo.Campo_Cod = If(tipoOperazioneCampo = enum_TipoOperazioneDB.Scrittura, Campo.Campo_Cod, dati_campo.primaryKey.codice) AndAlso
                                              entCampo.Validita_Inizio <= DateTime.Now AndAlso
                                              entCampo.Validita_Fine >= DateTime.Now
                                          Select entCampo

                    Dim entitaCampo = entitaCampoList.FirstOrDefault
                    If entitaCampo Is Nothing Then
                        'creazione
                        modelEntita.Piva = dati_campo.primaryKey.centroAziendalePK.partitaIva
                        modelEntita.Sa_Cod = dati_campo.primaryKey.centroAziendalePK.codice
                        modelEntita.Campo_cod = Campo.Campo_Cod
                        modelEntita.TipoEntita = enum_GIS2012_TipoEntita.CAMPI

                        Dim entita = AgronicaCoreGisDAL.EF_GIS_Entita.GIS_Entita_Scrivi_EF(modelEntita, objParametri, username, GiasContext, False)
                        modelElementoGrafico.EntitaCod = entita.Entita_Cod
                        modelElementoGrafico.Layer = enum_Gis_LayerElementiGrafici_std.CAMPI
                        modelElementoGrafico.Cartography = If(dati_campo.cartografia Is Nothing, "", dati_campo.cartografia)
                        modelElementoGrafico.Flag_GPS = dati_campo.flag_gps

                        Dim elementoGrafico = AgronicaCoreGisDAL.EF_GIS_ElementiGrafici.GIS_ElementoGrafico_Scrivi_EF(modelElementoGrafico, objParametri, username, GiasContext, False)
                        If (modelElementoGrafico.Cartography <> "") Then
                            EntitaCodXImg.Add(modelElementoGrafico.EntitaCod)
                        End If
                    Else
                        modelEntita.PivaSuperUser = entitaCampo.PivaSuperUser
                        modelEntita.EntitaCod = entitaCampo.Entita_Cod
                        modelEntita.Piva = entitaCampo.Piva
                        modelEntita.Sa_Cod = entitaCampo.Sa_Cod
                        modelEntita.Campo_cod = entitaCampo.Campo_Cod

                        Dim entita = AgronicaCoreGisDAL.EF_GIS_Entita.GIS_Entita_Modifica_EF(modelEntita, objParametri, username, GiasContext, False)

                        Dim elementiGraficiList = From elem In GiasContext.GIS_ElementiGrafici
                                                  Where elem.PivaSuperUser = modelEntita.PivaSuperUser AndAlso
                                             elem.Entita_Cod = modelEntita.EntitaCod
                                                  Select elem

                        Dim elementoGrafico = elementiGraficiList.FirstOrDefault
                        If elementoGrafico Is Nothing Then
                            modelElementoGrafico.EntitaCod = entita.Entita_Cod
                            modelElementoGrafico.Layer = enum_Gis_LayerElementiGrafici_std.CAMPI
                            modelElementoGrafico.Cartography = If(dati_campo.cartografia Is Nothing, "", dati_campo.cartografia)
                            modelElementoGrafico.Flag_GPS = dati_campo.flag_gps
                            Dim ele = AgronicaCoreGisDAL.EF_GIS_ElementiGrafici.GIS_ElementoGrafico_Scrivi_EF(modelElementoGrafico, objParametri, username, GiasContext, False)
                            If (modelElementoGrafico.Cartography <> "") Then
                                EntitaCodXImg.Add(modelElementoGrafico.EntitaCod)
                            End If
                        Else
                            modelElementoGrafico.PivaSuperUser = elementoGrafico.PivaSuperUser
                            modelElementoGrafico.ElementoGraficoCod = elementoGrafico.ElementoGrafico_Cod
                            modelElementoGrafico.Cartography = If(dati_campo.cartografia Is Nothing, "", dati_campo.cartografia)
                            modelElementoGrafico.Flag_GPS = dati_campo.flag_gps

                            Dim ele = AgronicaCoreGisDAL.EF_GIS_ElementiGrafici.GIS_ElementoGrafico_Modifica_EF(modelElementoGrafico, objParametri, username, GiasContext, False)
                            If (modelElementoGrafico.Cartography <> "") Then
                                EntitaCodXImg.Add(elementoGrafico.Entita_Cod)
                            End If
                        End If
                    End If

                Case enum_TipoOperazioneDB.Cancellazione
                    Dim entitaCampoList = From entCampo In GiasContext.GIS_Entita
                                          Where entCampo.Piva = dati_campo.primaryKey.centroAziendalePK.partitaIva AndAlso
                                              entCampo.Sa_Cod = dati_campo.primaryKey.centroAziendalePK.codice AndAlso
                                              entCampo.Appezza = 0 AndAlso
                                              entCampo.Id_Imp = 0 AndAlso
                                              entCampo.Campo_Cod = dati_campo.primaryKey.codice AndAlso
                                              entCampo.Validita_Inizio <= DateTime.Now AndAlso
                                              entCampo.Validita_Fine >= DateTime.Now
                                          Select entCampo

                    If entitaCampoList IsNot Nothing AndAlso entitaCampoList.Count > 0 Then
                        For Each entita In entitaCampoList
                            modelEntita.PivaSuperUser = entita.PivaSuperUser
                            modelEntita.EntitaCod = entita.Entita_Cod

                            Dim elementiGraficiList = From elem In GiasContext.GIS_ElementiGrafici
                                                      Where elem.PivaSuperUser = modelEntita.PivaSuperUser AndAlso
                                                          elem.Entita_Cod = modelEntita.EntitaCod
                                                      Select elem

                            If elementiGraficiList IsNot Nothing AndAlso elementiGraficiList.Count > 0 Then
                                For Each elemento In elementiGraficiList
                                    modelElementoGrafico.PivaSuperUser = elemento.PivaSuperUser
                                    modelElementoGrafico.ElementoGraficoCod = elemento.ElementoGrafico_Cod

                                    AgronicaCoreGisDAL.EF_GIS_ElementiGrafici.GIS_ElementoGrafico_Cancella_EF(modelElementoGrafico, objParametri, username, GiasContext, False)
                                Next
                            End If

                            AgronicaCoreGisDAL.EF_GIS_Entita.GIS_Entita_Cancella_EF(modelEntita, objParametri, username, GiasContext, False)

                        Next
                    End If
            End Select

            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If

        Catch ex As GiasException
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If
    End Sub
End Class


