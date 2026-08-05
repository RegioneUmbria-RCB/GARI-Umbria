Imports System.Xml
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

Imports AgronicaCoreDataProvider

Imports System.Text

Partial Public Class Funzioni

#Region "BIO"

    Public Sub Elabora_BIO_Zeta_Salva( _
                                ByVal objOpzioni As clsOpzioni, _
                                ByRef Log_Import As StringBuilder, _
                                ByRef Log_Errori As StringBuilder, _
                                ByRef Log_Riepilogo As StringBuilder, _
                                ByVal Piva_Origine As String, _
                                ByVal Piva_Destinazione As String
                        )

        Const nomeFunzione As String = "Elabora_BIO_Salva"

        Dim leggiPAPZ As New AgronicaCoreBiologicoDAL.CDX_PAP_Zeta_R
        Dim leggiPAPZ_B As New AgronicaCoreBiologicoDAL.CDX_PAPZeta_B_R


        Dim scriviPAPZ As New AgronicaCoreBiologicoDAL.CDX_PAPZeta_W
        Dim scriviPAPZ_B As New AgronicaCoreBiologicoDAL.CDX_PAPZeta_B_W

        Dim risp As Boolean


        Try

            Dim dtPAPZ As DataTable = leggiPAPZ.Leggi( _
                Piva_Origine, _
                0, _
                "", _
                " ID_PAPzoo ", _
                objOpzioni.objParametri_Server_GIAS_ORIGINE _
                )


            Dim id_papZeta As Integer
            Dim ID_PAP_Bzoo As Integer

            Dim papZFatti As New List(Of Integer)

            Dim objSequenze As New AgronicaCoreDataProvider.Agro_Sequenze

            For Each drow In dtPAPZ.Rows



                id_papZeta = objSequenze.NuovoId_Tabella(CStr("CDX_PAPZeta"), _
                                                   objOpzioni.BaseCode_DESTINAZIONE, _
                                                  objOpzioni.TopCode_DESTINAZIONE, _
                                                    objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)

                'testata zoo
                scriviPAPZ.Scrivi( _
                    Piva_Destinazione, _
                    id_papZeta, _
                    drow("Anno"), _
                    drow("Organismo_Sigla"), _
                    drow("Sede_Regionale_Cod"), _
                    drow("Regione_Cod"), _
                    drow("Protocollo"), _
                    drow("Protocollo_Data"), _
                    drow("Flag_Prima_Variazione"), _
                    drow("CUAA"), _
                    drow("Codice_Operatore"), _
                    drow("Data_Firma_Doc"), _
                    drow("Validita_Inizio"), _
                    drow("Validita_Fine"), _
                    drow("Validazione"), _
                    drow("Data_Validazione"), _
                    drow("UserName_Validazione"), _
                    objOpzioni.objParametri_Server_GIAS_DESTINAZIONE, _
                    drow("Data_Creazione"), _
                    drow("Data_Modifica"), _
                    drow("Username_Creazione"), _
                    drow("Username_Modifica") _
                )


                Dim dtPAPZ_B As DataTable = leggiPAPZ_B.Leggi( _
                "", _
                drow("ID_PAPzoo"), _
                0, _
                "", _
                " ID_PAPzoo ", _
                objOpzioni.objParametri_Server_GIAS_ORIGINE _
                )

                For Each dRowDet In dtPAPZ_B.Rows

                    ID_PAP_Bzoo = objSequenze.NuovoId_Tabella(CStr("CDX_PAPZoo"), _
                                                   objOpzioni.BaseCode_DESTINAZIONE, _
                                                  objOpzioni.TopCode_DESTINAZIONE, _
                                                    objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)

                    scriviPAPZ_B.Scrivi( _
                        Piva_Destinazione, _
                        id_papZeta, _
                        ID_PAP_Bzoo, _
                        dRowDet("UnitaProduttiva_Numero"), _
                        dRowDet("Codice_Specie"), _
                        dRowDet("Codice_Razza"), _
                        dRowDet("Codice_Categoria"), _
                        dRowDet("GEN_COD"), _
                        dRowDet("SPE_COD"), _
                        dRowDet("RAZ_COD"), _
                        dRowDet("IPRO_COD"), _
                        dRowDet("CAT_COD"), _
                        dRowDet("SpecieAnimali_Des"), _
                        dRowDet("CategoriaAnimali_Des"), _
                        dRowDet("RazzaAnimali_Des"), _
                        dRowDet("NUM_CAPI"), _
                        dRowDet("Num_Cicli"), _
                        dRowDet("FLAG_BIOLOGICO"), _
                        dRowDet("REG_COD"), _
                        dRowDet("UDM_NUMERO"), _
                        dRowDet("PROD_DESCR"), _
                        dRowDet("PROD_UDM"), _
                        dRowDet("PROD_QTA"), _
                        dRowDet("PROD_LOTTO"), _
                        dRowDet("PROD_CAT"), _
                        dRowDet("note"), _
                        dRowDet("Validita_Inizio"), _
                        dRowDet("Validita_Fine"), _
                        dRowDet("Validazione"), _
                        dRowDet("Data_Validazione"), _
                        dRowDet("UserName_Validazione"), _
                        objOpzioni.objParametri_Server_GIAS_DESTINAZIONE, _
                        dRowDet("Data_Creazione"), _
                        dRowDet("Data_Modifica"), _
                        dRowDet("Username_Creazione"), _
                        dRowDet("Username_Modifica") _
                     )


                Next

            Next



        Catch ex As Exception

            Dim msg As String
            msg = CStr(Date.Now) + " - " + nomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try
    End Sub




    Public Sub Elabora_BIO_Salva( _
                                ByVal objOpzioni As clsOpzioni, _
                                ByRef Log_Import As StringBuilder, _
                                ByRef Log_Errori As StringBuilder, _
                                ByRef Log_Riepilogo As StringBuilder, _
                                ByVal Piva_Origine As String, _
                                ByVal Piva_Destinazione As String, _
                                ByVal Sa_cod_Origine As Integer, _
                                ByVal sa_cod_destinazione As Integer _
                        )

        Const nomeFunzione As String = "Elabora_BIO_Salva"


        Try

            Dim leggiPAP As New AgronicaCoreBiologicoDAL.CDX_PAP_R

            Dim scriviPAP As New AgronicaCoreBiologicoDAL.CDX_PAP_W
            Dim scriviPAP_B As New AgronicaCoreBiologicoDAL.CDX_PAP_B_W


            Dim risp As Boolean

            Dim dtLetti As DataTable = leggiPAP.Stampa_PAP(0, " CDX_PAP.Piva='" & Piva_Origine & "' AND CDX_PAP_B.SA_cod = " & Sa_cod_Origine, " CDX_PAP.ID_PAP ", objOpzioni.objParametri_Server_GIAS_ORIGINE)



            Dim ID_PAP As Integer
            Dim ID_PAP_B As Integer
            Dim objSequenze As New AgronicaCoreDataProvider.Agro_Sequenze

            Dim papFatti As New List(Of Integer)




            For Each drow In dtLetti.Rows

                If Not papFatti.Contains(CInt(drow("ID_PAP"))) Then

                    ID_PAP = objSequenze.NuovoId_Tabella(CStr("CDX_PAP"), _
                                                   objOpzioni.BaseCode_DESTINAZIONE, _
                                                  objOpzioni.TopCode_DESTINAZIONE, _
                                                    objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)


                    'testata
                    risp = scriviPAP.Scrivi( _
                                ID_PAP, _
                                drow("Anno"), _
                                drow("Organismo_Sigla"), _
                                drow("Sede_Regionale_Cod"), _
                                drow("Regione_Cod"), _
                                drow("Protocollo"), _
                                drow("Protocollo_Data"), _
                                drow("Flag_Prima_Variazione"), _
                                Piva_Destinazione, _
                                drow("CUAA"), _
                                drow("SupTotDoc_Ettari"), _
                                drow("SupTotDoc_Are"), _
                                drow("SupTotDoc_Centiare"), _
                                drow("NumTotAppezzamenti"), _
                                drow("Data_Firma_Doc"), _
                                drow("Validita_Inizio"), _
                                drow("Validita_Fine"), _
                                drow("Validazione"), _
                                drow("Data_Validazione"), _
                                drow("UserName_Validazione"), _
                                objOpzioni.objParametri_Server_GIAS_DESTINAZIONE, _
                                drow("Data_Creazione"), _
                                drow("Data_Modifica"), _
                                drow("Username_Creazione"), _
                                drow("Username_Modifica") _
                    )

                    papFatti.Add(CInt(drow("ID_PAP")))
                End If

                ID_PAP_B = objSequenze.NuovoId_Tabella(CStr("CDX_PAP"), _
                                                   objOpzioni.BaseCode_DESTINAZIONE, _
                                                  objOpzioni.TopCode_DESTINAZIONE, _
                                                    objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)


                Dim appezzaLetto As Integer = drow("Appezza")
                Dim AppezzaDecodificato As Integer?
                Dim CampoLetto As Integer = drow("Campo_cod")
                Dim CampoCodDecodificato As Integer?
                Dim id_Reg_Letto As Integer = 0
                Dim id_Reg_Decodificato As Integer?
                Dim progettoCodLetto As Integer = 0
                Dim progettoCodDecodificato As Integer?
                LeggiDecodifiche(Piva_Origine, Sa_cod_Origine, nomeFunzione, drow, AppezzaLetto, AppezzaDecodificato, campoCodDecodificato, Id_reg_letto, id_reg_Decodificato, progettoCodLetto, progettoCodDecodificato)


                If AppezzaDecodificato Is Nothing And CampoCodDecodificato Is Nothing Then
                    Throw New Exception("cdx_campo: campo cod o appezza devono essere impostati.")
                End If

                'dettagli
                scriviPAP_B.Scrivi( _
                    ID_PAP, _
                    ID_PAP_B, _
                    drow("Num_Appezzamento"), _
                    Piva_Destinazione, _
                    sa_cod_destinazione, _
                    CampoCodDecodificato, _
                    AppezzaDecodificato, _
                    drow("Specie_Cod"), _
                    drow("Varieta_Cod"), _
                    drow("Specie_Des"), _
                    drow("Varieta_Des"), _
                    drow("Veg_Cod"), _
                    drow("Cul_Cod"), _
                    drow("AppSup_Ettari"), _
                    drow("AppSup_Are"), _
                    drow("AppSup_Centiare"), _
                    drow("Tipo_Agricoltura"), _
                    drow("Consociazione"), _
                    drow("Successione"), _
                    drow("Anno_Impianto"), _
                    drow("Qta_Prevista"), _
                    drow("Forza_Lavoro"), _
                    drow("Num_Appezzamento_Str"), _
                    drow("Validita_Inizio"), _
                    drow("Validita_Fine"), _
                    drow("Validazione"), _
                    drow("Data_Validazione"), _
                    drow("UserName_Validazione"), _
                    objOpzioni.objParametri_Server_GIAS_DESTINAZIONE, _
                    drow("Data_Creazione"), _
                    drow("Data_Modifica"), _
                    drow("Username_Creazione"), _
                    drow("Username_Modifica") _
                )

            Next


        Catch ex As Exception

            Dim msg As String
            msg = CStr(Date.Now) + " - " + nomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try

    End Sub

#End Region

#Region "BIO-Notifica"

    Public Sub Elabora_BIO_Notifica_Salva( _
                                    ByVal objOpzioni As clsOpzioni, _
                                    ByRef Log_Import As StringBuilder, _
                                    ByRef Log_Errori As StringBuilder, _
                                    ByRef Log_Riepilogo As StringBuilder, _
                                    ByVal Piva_Origine As String, _
                                    ByVal Piva_Destinazione As String, _
                                    ByVal Sa_cod_Origine As Integer, _
                                    ByVal Sa_Cod_Destinazione As Integer _
                            )

        Const nomeFunzione As String = "Elabora_BIO_Notifica_Salva"

        Try

            Dim leggiBio_Notifica As New AgronicaCoreBiologicoDAL.BIO_Notifica_Frontespizio_R
            Dim dtLeggiBio_Notifica As DataTable

            Dim leggiBIO_Notifica_SezA_Informazioni_R As New AgronicaCoreBiologicoDAL.BIO_Notifica_SezA_Informazioni_R
            Dim dtBIO_Notifica_SezA_Informazioni_R As DataTable

            Dim leggiBIO_Notifica_SezB_Zootecnico_R As New AgronicaCoreBiologicoDAL.BIO_Notifica_SezB_Zootecnico_R
            Dim dtBIO_Notifica_SezB_Zootecnico_R As DataTable

            Dim leggiBIO_Notifica_SezC_PreparazioniAlimentari_R As New AgronicaCoreBiologicoDAL.BIO_Notifica_SezC_PreparazioniAlimentari_R
            Dim dtBIO_Notifica_SezC_PreparazioniAlimentari_R As DataTable

            Dim leggiBIO_Notifica_SezD_Importazione_R As New AgronicaCoreBiologicoDAL.BIO_Notifica_SezD_Importazione_R
            Dim dtBIO_Notifica_SezD_Importazione_R As DataTable

            Dim leggiBIO_Notifica_SezE_Particelle_R As New AgronicaCoreBiologicoDAL.BIO_Notifica_SezE_Particelle_R
            Dim dtBIO_Notifica_SezE_Particelle_R As DataTable

            Dim leggiBIO_Notifica_SezF_Appezzamenti_R As New AgronicaCoreBiologicoDAL.BIO_Notifica_SezF_Appezzamenti_R
            Dim dtBIO_Notifica_SezF_Appezzamenti_R As DataTable

            Dim leggiBIO_Notifica_SezG_Fabbricati_R As New AgronicaCoreBiologicoDAL.BIO_Notifica_SezG_Fabbricati_R
            Dim dtBIO_Notifica_SezG_Fabbricati_R As DataTable


            Dim ScriviBioNotifica As New AgronicaCoreBiologicoDAL.BIO_Notifica_Frontespizio_W
            Dim ScriviBIO_Notifica_SezA_Informazioni_W As New AgronicaCoreBiologicoDAL.BIO_Notifica_SezA_Informazioni_W
            Dim ScriviBIO_Notifica_SezB_Zootecnico_W As New AgronicaCoreBiologicoDAL.BIO_Notifica_SezB_Zootecnico_W
            Dim ScriviBIO_Notifica_SezC_PreparazioniAlimentari_W As New AgronicaCoreBiologicoDAL.BIO_Notifica_SezC_PreparazioniAlimentari_W
            Dim ScriviBIO_Notifica_SezD_Importazione_W As New AgronicaCoreBiologicoDAL.BIO_Notifica_SezD_Importazione_W
            Dim ScriviBIO_Notifica_SezE_Particelle_W As New AgronicaCoreBiologicoDAL.BIO_Notifica_SezE_Particelle_W
            Dim ScriviBIO_Notifica_SezF_Appezzamenti_W As New AgronicaCoreBiologicoDAL.BIO_Notifica_SezF_Appezzamenti_W
            Dim ScriviBIO_Notifica_SezG_Fabbricati_W As New AgronicaCoreBiologicoDAL.BIO_Notifica_SezG_Fabbricati_W


            dtLeggiBio_Notifica = leggiBio_Notifica.Leggi( _
                0, _
                CostantiPersonalizzate.AGRODATAINIZIO,
                Piva_Origine, _
                Sa_cod_Origine, _
                "", _
                CostantiPersonalizzate.AGRODATAINIZIO, _
                "", _
                "", _
                objOpzioni.objParametri_Server_GIAS_ORIGINE, _
                Sa_cod_Origine = 0 _
            )



            For Each rowLeggiBio_Notifica As DataRow In dtLeggiBio_Notifica.Rows

                Dim objSequenze As New Agro_Sequenze

                Dim Notifica_IDNew As Integer
                Dim Notifica_IDOld As Integer = _
                    CType(rowLeggiBio_Notifica("Notifica_ID"), Integer)

                Notifica_IDNew = objSequenze.NuovoId_Tabella(CStr("BIO_Notifica"), _
                                                        objOpzioni.BaseCode_DESTINAZIONE, _
                                                       objOpzioni.TopCode_DESTINAZIONE, _
                                                        objOpzioni.objParametri_Server_GIAS_DESTINAZIONE _
                                                    )

                Dim ProgrammazioneCOD_OLD As Integer?
                Dim ProgrammazioneCOD_NEW As Integer?

                ProgrammazioneCOD_OLD = _
                    CType(rowLeggiBio_Notifica("Programmazione_Cod"), Integer)

                ProgrammazioneCOD_NEW = ( _
                    From p In _funzioniGLOBAL.Planning _
                    Where p.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser _
                    And p.From_Programmazione_Cod = ProgrammazioneCOD_OLD _
                    And p.From_Programmazione_Entita_cod = -1 _
                    And p.From_Piva = Piva_Origine _
                    Select p.To_Programmazione_Cod _
                ).FirstOrDefault

                If ProgrammazioneCOD_NEW Is Nothing Then
                    ProgrammazioneCOD_NEW = 0
                End If

                'testata
                ScriviBioNotifica.Scrivi( _
                    Notifica_IDNew, _
                    Piva_Destinazione, _
                    Sa_Cod_Destinazione, _
                    CType(rowLeggiBio_Notifica("Notifica_Ricevimento_Protocollo"), String), _
                    CType(rowLeggiBio_Notifica("Notifica_Ricevimento_Data"), Date), _
                    CType(rowLeggiBio_Notifica("Notifica_Fotografia_Data"), Date), _
                    CType(rowLeggiBio_Notifica("Cod_Istat_Regione"), String), _
                    CType(rowLeggiBio_Notifica("Flag_Regione"), Integer), _
                    CType(rowLeggiBio_Notifica("Flag_Ministero_MiPAF"), Integer), _
                    CType(rowLeggiBio_Notifica("Flag_Categoria_Produttore"), Integer), _
                    CType(rowLeggiBio_Notifica("Flag_Categoria_Preparatore"), Integer), _
                    CType(rowLeggiBio_Notifica("Flag_Categoria_Importatore"), Integer), _
                    CType(rowLeggiBio_Notifica("Flag_PrimaNotifica1_Variazione2"), Integer), _
                    CType(rowLeggiBio_Notifica("Flag_Cause_ModificaDichiarante"), Integer), _
                    CType(rowLeggiBio_Notifica("Flag_Cause_ModificaUnitaProduttive"), Integer), _
                    CType(rowLeggiBio_Notifica("Flag_Cause_ModificaCategorie"), Integer), _
                    CType(rowLeggiBio_Notifica("Flag_Cause_CambioOrganismo"), Integer), _
                    CType(rowLeggiBio_Notifica("Flag_Cause_CambioOrganismo_Des"), String), _
                    CType(rowLeggiBio_Notifica("Flag_Cause_Altro"), Integer), _
                    CType(rowLeggiBio_Notifica("Flag_Cause_Altro_Des"), String), _
                    CType(rowLeggiBio_Notifica("Dichiarante_CodiceFiscale"), String), _
                    CType(rowLeggiBio_Notifica("Dichiarante_Piva"), String), _
                    CType(rowLeggiBio_Notifica("Dichiarante_Flag_AziendaIndividuale"), Integer), _
                    CType(rowLeggiBio_Notifica("Dichiarante_Flag_Societa"), Integer), _
                    CType(rowLeggiBio_Notifica("Dichiarante_Flag_Cooperativa"), Integer), _
                    CType(rowLeggiBio_Notifica("Dichiarante_Cognome_RagSoc"), String), _
                    CType(rowLeggiBio_Notifica("Dichiarante_Nome"), String), _
                    CType(rowLeggiBio_Notifica("Dichiarante_Nascita_Provincia_Sigla"), String), _
                    CType(rowLeggiBio_Notifica("Dichiarante_Nascita_Provincia_ISTAT"), String), _
                    CType(rowLeggiBio_Notifica("Dichiarante_Nascita_Comune"), String), _
                    CType(rowLeggiBio_Notifica("Dichiarante_Nascita_Comune_ISTAT"), String), _
                    CType(rowLeggiBio_Notifica("Dichiarante_Nascita_Data"), Date), _
                    CType(rowLeggiBio_Notifica("Dichiarante_Flag_Maschio1_Femmina2_Giuridica3"), Integer), _
                    CType(rowLeggiBio_Notifica("Dichiarante_Domicilio_Provincia_Sigla"), String), _
                    CType(rowLeggiBio_Notifica("Dichiarante_Domicilio_Provincia_ISTAT"), String), _
                    CType(rowLeggiBio_Notifica("Dichiarante_Domicilio_Comune"), String), _
                    CType(rowLeggiBio_Notifica("Dichiarante_Domicilio_Comune_ISTAT"), String), _
                    CType(rowLeggiBio_Notifica("Dichiarante_Domicilio_Via"), String), _
                    CType(rowLeggiBio_Notifica("Dichiarante_Domicilio_Numero"), String), _
                    CType(rowLeggiBio_Notifica("Dichiarante_Domicilio_CAP"), String), _
                    CType(rowLeggiBio_Notifica("Dichiarante_Domicilio_Telefono"), String), _
                    CType(rowLeggiBio_Notifica("Dichiarante_Domicilio_Fax"), String), _
                    CType(rowLeggiBio_Notifica("Dichiarante_Domicilio_Email"), String), _
                    CType(rowLeggiBio_Notifica("RappLegale_Cognome"), String), _
                    CType(rowLeggiBio_Notifica("RappLegale_Nome"), String), _
                    CType(rowLeggiBio_Notifica("RappLegale_CodiceFiscale"), String), _
                    CType(rowLeggiBio_Notifica("RappLegale_Nascita_Provincia_Sigla"), String), _
                    CType(rowLeggiBio_Notifica("RappLegale_Nascita_Provincia_ISTAT"), String), _
                    CType(rowLeggiBio_Notifica("RappLegale_Nascita_Comune"), String), _
                    CType(rowLeggiBio_Notifica("RappLegale_Nascita_Comune_ISTAT"), String), _
                    CType(rowLeggiBio_Notifica("RappLegale_Nascita_Data"), Date), _
                    CType(rowLeggiBio_Notifica("RappLegale_Flag_Maschio1_Femmina2"), Integer), _
                    CType(rowLeggiBio_Notifica("RappLegale_Domicilio_Provincia_Sigla"), String), _
                    CType(rowLeggiBio_Notifica("RappLegale_Domicilio_Provincia_ISTAT"), String), _
                    CType(rowLeggiBio_Notifica("RappLegale_Domicilio_Comune"), String), _
                    CType(rowLeggiBio_Notifica("RappLegale_Domicilio_Comune_ISTAT"), String), _
                    CType(rowLeggiBio_Notifica("RappLegale_Domicilio_Via"), String), _
                    CType(rowLeggiBio_Notifica("RappLegale_Domicilio_Numero"), String), _
                    CType(rowLeggiBio_Notifica("RappLegale_Domicilio_CAP"), String), _
                    CType(rowLeggiBio_Notifica("OrganismoControllo_1_Codice"), String), _
                    CType(rowLeggiBio_Notifica("OrganismoControllo_1_CodiceOperatore"), String), _
                    CType(rowLeggiBio_Notifica("Organismo1_Flag_Produttore"), Integer), _
                    CType(rowLeggiBio_Notifica("Organismo1_Flag_Preparatore"), Integer), _
                    CType(rowLeggiBio_Notifica("Organismo1_Flag_Importatore"), Integer), _
                    CType(rowLeggiBio_Notifica("OrganismoControllo_2_Codice"), String), _
                    CType(rowLeggiBio_Notifica("OrganismoControllo_2_CodiceOperatore"), String), _
                    CType(rowLeggiBio_Notifica("Organismo2_Flag_Produttore"), Integer), _
                    CType(rowLeggiBio_Notifica("Organismo2_Flag_Preparatore"), Integer), _
                    CType(rowLeggiBio_Notifica("Organismo2_Flag_Importatore"), Integer), _
                    CType(rowLeggiBio_Notifica("Flag_Notifica_Bloccata"), Integer), _
                    ProgrammazioneCOD_NEW, _
                    CType(rowLeggiBio_Notifica("Username_Creazione"), String), _
                    CType(rowLeggiBio_Notifica("Data_Creazione"), Date), _
                    CType(rowLeggiBio_Notifica("Validita_Inizio"), Date), _
                    CType(rowLeggiBio_Notifica("Validita_Fine"), Date), _
                    objOpzioni.objParametri_Server_GIAS_DESTINAZIONE, _
                    rowLeggiBio_Notifica("Data_Modifica"), _
                    rowLeggiBio_Notifica("Username_Modifica") _
                )


                dtBIO_Notifica_SezA_Informazioni_R = _
                    leggiBIO_Notifica_SezA_Informazioni_R.Leggi( _
                        Notifica_IDOld, _
                        "", _
                        "", _
                        objOpzioni.objParametri_Server_GIAS_ORIGINE _
                    )


                ' --- Sezione A---
                For Each rowBIO_Notifica_SezA_Informazioni_R In dtBIO_Notifica_SezA_Informazioni_R.Rows

                    ScriviBIO_Notifica_SezA_Informazioni_W.Scrivi( _
                        Notifica_IDNew, _
                        Piva_Destinazione, _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("UnitaProduttiva_RagSoc"), String), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("UnitaProduttiva_CodiceOperatore"), String), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("UnitaProduttiva_Via"), String), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("UnitaProduttiva_Numero"), String), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("UnitaProduttiva_CAP"), String), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("UnitaProduttiva_CodIstat_Provincia"), String), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("UnitaProduttiva_CodIstat_Comune"), String), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("UnitaProduttiva_Telefono"), String), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("UnitaProduttiva_Fax"), String), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("UnitaProduttiva_Email"), String), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("Sup_Catastale"), Double), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("Sup_Bosco"), Double), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("Sup_Tare"), Double), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("Sup_PratiPascoli"), Double), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("Sup_SAU_Totale"), Double), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("Sup_SAU_Convenzionale"), Double), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("Sup_SAU_Conversione"), Double), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("Sup_SAU_Biologico"), Double), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IP_Cerealicolo"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IP_Cerealicolo_Riso"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IP_Cerealicolo_GranoDuro"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IP_Cerealicolo_GranoTenero"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IP_Cerealicolo_Mais"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IP_Cerealicolo_AltriCereali"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IP_Orticolo"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IP_Orticolo_PienoCampo"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IP_Orticolo_ColturaProtetta"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IP_ColtureIndustriali"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IP_Frutticolo"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IP_Frutticolo_Pomacee"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IP_Frutticolo_Drupacee"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IP_Frutticolo_Agrumi"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IP_Frutticolo_FruttaSecca"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IP_Frutticolo_UvaTavola"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IP_Frutticolo_Altro"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IP_Frutticolo_Altro_Des"), String), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IP_Vitivinicolo"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IP_Vitivinicolo_DaTavola"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IP_Vitivinicolo_Denominazione"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IP_Olivicolo"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IP_Olivicolo_DaMensa"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IP_Olivicolo_DaOlio"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IP_Foraggero"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IP_VivaisticoSementiero"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IP_Altro"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IP_Altro_Des"), String), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("S_Magazzini_Aziendali"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("S_Magazzini_Esterni"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("S_Magazzini_xMezziTecnici"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("S_Magazzini_xAttrezzature"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("S_Magazzini_xVenditaProdotti"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("S_Magazzini_Altro"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("S_Magazzini_Altro_Des"), String), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("S_Sili_Aziendali"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("S_Sili_Esterni"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("S_Sili_StockGranaglie"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("S_Sili_StockColtureIndustriali"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("S_Sili_StockMangimi"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("S_Sili_PreparazioneInsilati"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("S_Sili_Altro"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("S_Sili_Altro_Des"), String), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("S_Celle_Aziendali"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("S_Celle_Esterne"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("S_Celle_ProdVegetali"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("S_Celle_ProdZootecniche"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("S_Celle_Altro"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("S_Celle_Altro_Des"), String), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("S_Impianti_Aziendali"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("S_Impianti_Esterni"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("S_Impianti_Altro"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("S_Impianti_Altro_Des"), String), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IR_CEREALI"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IR_Cereali_Granella"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IR_Cereali_Conservazione"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IR_Cereali_Sfarinati"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IR_Cereali_Pastificazione"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IR_Cereali_Panificazione"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IR_Cereali_ProdottiDaForno"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IR_Cereali_AltriProdotti"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IR_COLTUREIND"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IR_ColtureInd_Granella"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IR_ColtureInd_Sfarinati"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IR_ColtureIndi_EstrazioneOlio"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IR_ColtureInd_Conservazione"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IR_ColtureInd_Confezionamento"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IR_ColtureInd_AltriProdotti"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IR_ORTOFRUTTICOLI"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IR_Ortofrutticoli_Freschi"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IR_Ortofrutticoli_ConserveVegetali"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IR_Ortofrutticoli_Conservazione"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IR_Ortofrutticoli_Confezionamento"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IR_VITIVINICOLO"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IR_Vitivinicolo_Vinificazione"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IR_Vitivinicolo_Mostificazione"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IR_Vitivinicolo_Imbottigliamento"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IR_OLEICOLO"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IR_Oleicolo_ConserveVegetali"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IR_Oleicolo_EstrazioneOlio"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IR_Oleicolo_Imbottigliamento"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IR_VIVSEMENTIERO"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IR_VivSementiero_Semi"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IR_VivSementiero_OrticoleTrapianto"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IR_VivSementiero_Astoni"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IR_VivSementiero_Barbatelle"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IR_VivSementiero_Altro"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IR_PIANTE_OFFICINALI"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("IR_PRODOTTI_SPONTANEI"), Integer), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("Username_Creazione"), String), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("Data_Creazione"), Date), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("Validita_Inizio"), Date), _
                        CType(rowBIO_Notifica_SezA_Informazioni_R("Validita_Fine"), Date), _
                        objOpzioni.objParametri_Server_GIAS_DESTINAZIONE, _
                        rowBIO_Notifica_SezA_Informazioni_R("Data_Modifica"), _
                        rowBIO_Notifica_SezA_Informazioni_R("Username_Modifica") _
                    )

                Next

                dtBIO_Notifica_SezB_Zootecnico_R = _
                    leggiBIO_Notifica_SezB_Zootecnico_R.Leggi( _
                        Notifica_IDOld, _
                        "", _
                        0, _
                        "", _
                        "", _
                        objOpzioni.objParametri_Server_GIAS_ORIGINE _
                    )

                ' --- Sezione B---
                For Each rowBIO_Notifica_SezB_Zootecnico_R In dtBIO_Notifica_SezB_Zootecnico_R.Rows

                    Dim l_saCod_SezB_Zootecnico_R As Integer = CType(rowBIO_Notifica_SezB_Zootecnico_R("UnitaProduttiva_SaCod"), Integer)
                    Dim decode_Sa_cod As Integer = _
                        (From r In _funzioniGLOBAL.Imprese _
                         Where r.FROM_Piva = Piva_Origine _
                         And r.FROM_SaCod = l_saCod_SezB_Zootecnico_R _
                         Select r.TO_SaCod).FirstOrDefault


                    ScriviBIO_Notifica_SezB_Zootecnico_W.Scrivi( _
                            Notifica_IDNew, _
                             Piva_Destinazione, _
                            decode_Sa_cod, _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("UnitaProduttiva_RagSoc"), String), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("UnitaProduttiva_Via"), String), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("UnitaProduttiva_Numero"), String), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("UnitaProduttiva_CAP"), String), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("UnitaProduttiva_CodIstat_Provincia"), String), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("UnitaProduttiva_CodIstat_Comune"), String), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("UnitaProduttiva_Telefono"), String), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("UnitaProduttiva_Fax"), String), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("UnitaProduttiva_Email"), String), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("UnitaProduttiva_CodAUSL"), String), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("UnitaProduttiva_TotaleZootecniche"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA_TotaleUnitaProduttive"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA10_UBABiologico"), Double), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA10_UBAConvenzionale"), Double), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA10_IPCarne"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA10_IPLatte"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA10_IPRiproduzione"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA10_IPAltro"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA10_IPAltroDes"), String), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA20_UBABiologico"), Double), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA20_UBAConvenzionale"), Double), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA20_IPCarne"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA20_IPLatte"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA20_IPRiproduzione"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA20_IPAltro"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA20_IPAltroDes"), String), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA30_UBABiologico"), Double), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA30_UBAConvenzionale"), Double), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA30_IPCarne"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA30_IPLatte"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA30_IPRiproduzione"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA30_IPAltro"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA30_IPAltroDes"), String), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA40_UBABiologico"), Double), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA40_UBAConvenzionale"), Double), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA40_IPCarne"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA40_IPLatte"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA40_IPRiproduzione"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA40_IPAltro"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA40_IPAltroDes"), String), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA50_UBABiologico"), Double), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA50_UBAConvenzionale"), Double), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA50_IPCarne"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA50_IPRiproduzione"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA50_IPAltro"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA50_IPAltroDes"), String), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA60_UBABiologico"), Double), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA60_UBAConvenzionale"), Double), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA60_IPRiproduzione"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA60_IPAltro"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA60_IPAltroDes"), String), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA61_UBABiologico"), Double), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA61_UBAConvenzionale"), Double), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA61_IPCarne"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA70_UBABiologico"), Double), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA70_UBAConvenzionale"), Double), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA70_IPCarne"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA70_IPUova"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA70_IPRiproduzione"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA70_IPAltro"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA70_IPAltroDes"), String), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA80_UBABiologico"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA80_UBAConvenzionale"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA80_IPMiele"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA80_IPPReale"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA80_IPCera"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA80_IPAltro"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA80_IPAltroDes"), String), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA90_UBABiologico"), Double), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA90_UBAConvenzionale"), Double), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA90_AltroDes"), String), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA90_IPAltroDes"), String), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA_TotaleUBA"), Double), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA_TotaleFamiglie"), Double), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("AA_UBAxEttaroSAU"), Double), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("PZ_CARNE"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("PZ_Carne_CarneFresca"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("PZ_Carne_DerivatiCarne"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("PZ_Carne_Macellazione"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("PZ_Carne_Conservazione"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("PZ_Carne_Sezionamento"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("PZ_Carne_ProdottiSalumeria"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("PZ_Carne_Confezionamento"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("PZ_LATTE"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("PZ_Latte_LatteAlimentare"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("PZ_Latte_Caseificazione"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("PZ_Latte_Burro"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("PZ_Latte_Yogurt"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("PZ_Latte_AltriDerivatiLatte"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("PZ_Latte_Confezionamento"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("PZ_Latte_Altro"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("PZ_Latte_AltroDes"), String), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("PZ_UOVA"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("PZ_Uova_Confezionamento"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("PZ_Uova_Altro"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("PZ_Uova_AltroDes"), String), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("PZ_PRODOTTIAPICOLTURA"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("PZ_ProdottiApicoltura_Confezionamento"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("PZ_ALTRO"), Integer), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("PZ_Altro_AltroDes"), String), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("Username_Creazione"), String), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("Data_Creazione"), Date), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("Validita_Inizio"), Date), _
                            CType(rowBIO_Notifica_SezB_Zootecnico_R("Validita_Fine"), Date), _
                            objOpzioni.objParametri_Server_GIAS_DESTINAZIONE, _
                            rowBIO_Notifica_SezB_Zootecnico_R("Data_Modifica"), _
                            rowBIO_Notifica_SezB_Zootecnico_R("Username_Modifica") _
                            )

                Next


                dtBIO_Notifica_SezC_PreparazioniAlimentari_R = _
                    leggiBIO_Notifica_SezC_PreparazioniAlimentari_R.Leggi( _
                        Notifica_IDOld, _
                        0, _
                        "", _
                        0, _
                        "", _
                        "", _
                    objOpzioni.objParametri_Server_GIAS_ORIGINE _
                )

                '--- Sezione C---
                For Each rowsBIO_Notifica_SezC_PreparazioniAlimentari_R In dtBIO_Notifica_SezC_PreparazioniAlimentari_R.Rows


                    Dim lsaCod_SezC_PreparazioniAlimentari_R As Integer = CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("CentroPreparazione_SaCod"), Integer)
                    Dim decode_Sa_cod As Integer = _
                        (From r In _funzioniGLOBAL.Imprese _
                         Where r.FROM_Piva = Piva_Origine _
                         And r.FROM_SaCod = lsaCod_SezC_PreparazioniAlimentari_R _
                         Select r.TO_SaCod).FirstOrDefault



                    ScriviBIO_Notifica_SezC_PreparazioniAlimentari_W.Scrivi( _
                    Notifica_IDNew, _
                    Piva_Destinazione, _
                    decode_Sa_cod, _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("CentroPreparazionePiva_Flag_Nostampa_Stampa"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("CentroPreparazione_RagSoc"), String), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("CentroPreparazione_Via"), String), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("CentroPreparazione_Numero"), String), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("CentroPreparazione_CAP"), String), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("CentroPreparazione_CodIstat_Provincia"), String), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("CentroPreparazione_CodIstat_Comune"), String), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("CentroPreparazione_Telefono"), String), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("CentroPreparazione_Fax"), String), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("CentroPreparazione_Email"), String), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("CentroPreparazione_CodAUSL"), String), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("CentroPreparazione_Comune"), String), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("CentroPreparazione_Sigla_Provincia"), String), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("CentroPreparazione_Provincia"), String), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_VEGETALE_Cp"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_VEGETALE_Ct"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_VEGETALE_Cm"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Vegetale_Ortofrutticoli_Cp"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Vegetale_Ortofrutticoli_Ct"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Vegetale_Ortofrutticoli_Cm"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Vegetale_Molinatura_Cp"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Vegetale_Molinatura_Ct"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Vegetale_Molinatura_Cm"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Vegetale_Fioccatura_Cp"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Vegetale_Fioccatura_Ct"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Vegetale_Fioccatura_Cm"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Vegetale_Pastificazione_Cp"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Vegetale_Pastificazione_Ct"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Vegetale_Pastificazione_Cm"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Vegetale_Surgelati_Cp"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Vegetale_Surgelati_Ct"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Vegetale_Surgelati_Cm"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Vegetale_Conserve_Cp"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Vegetale_Conserve_Ct"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Vegetale_Conserve_Cm"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Vegetale_IntegratoriAlimentari_Cp"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Vegetale_IntegratoriiAlimentari_Ct"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Vegetale_IntegratoriAlimentari_Cm"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Vegetale_EstrazioneOlio_Cp"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Vegetale_EstrazioneOlio_Ct"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Vegetale_EstrazioneOlio_Cm"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Vegetale_Vinificazione_Cp"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Vegetale_Vinificazione_Ct"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Vegetale_Vinificazione_Cm"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Vegetale_Liquori_Cp"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Vegetale_Liquori_Ct"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Vegetale_Liquori_Cm"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Vegetale_Imbottigliamento_Cp"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Vegetale_Imbottigliamento_Ct"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Vegetale_Imbottigliamento_Cm"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Vegetale_ProdottiErboristici_Cp"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Vegetale_ProdottiErboristici_Ct"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Vegetale_ProdottiErboristici_Cm"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Vegetale_AltroDes_Cp"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Vegetale_AltroDes_Ct"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Vegetale_AltroDes_Cm"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Vegetale_AltroDes"), String), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_ANIMALE_Cp"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_ANIMALE_Ct"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_ANIMALE_Cm"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Animale_Porzionatura_Cp"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Animale_Porzionatura_Ct"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Animale_Porzionatura_Cm"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Animale_Macellazione_Cp"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Animale_Macellazione_Ct"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Animale_Macellazione_Cm"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Animale_Sezionamento_Cp"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Animale_Sezionamento_Ct"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Animale_Sezionamento_Cm"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Animale_DerivatiCarne_Cp"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Animale_DerivatiCarne_Ct"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Animale_DerivatiCarne_Cm"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Animale_ConserveAnimali_Cp"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Animale_ConserveAnimali_Ct"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Animale_ConserveAnimali_Cm"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Animale_ProdottiSalumeria_Cp"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Animale_ProdottiSalumeria_Ct"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Animale_ProdottiSalumeria_Cm"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Animale_LatteAlimentare_Cp"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Animale_LatteAlimentare_Ct"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Animale_LatteAlimentare_Cm"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Animale_Caseificazione_Cp"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Animale_Caseificazione_Ct"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Animale_Caseificazione_Cm"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Animale_Burro_Cp"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Animale_Burro_Ct"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Animale_Burro_Cm"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Animale_Yogurt_Cm"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Animale_Yogurt_Cp"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Animale_Yogurt_Ct"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Animale_Uova_Cp"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Animale_Uova_Ct"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Animale_Uova_Cm"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Animale_Altro_Cp"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Animale_Altro_Ct"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Animale_Altro_Cm"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Animale_AltroDes"), String), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_INDUSTRIADOLCIARIA_Cp"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_INDUSTRIADOLCIARIA_Ct"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_IndustriaDolciaria_DaForno_Cp"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_IndustriaDolciaria_DaForno_Ct"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_IndustriaDolciaria_DaForno_Cm"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_IndustriaDolciaria_ProdottiDolciari_Cp"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_IndustriaDolciaria_ProdottiDolciari_Ct"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_IndustriaDolciaria_AltriProdotti_Cp"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_IndustriaDolciaria_AltriProdotti_Ct"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_IndustriaDolciaria_AltriProdotti_Cm"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_MANGIMI_Cp"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_MANGIMI_Ct"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_MANGIMI_Cm"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Mangimi_AltroDes_Cp"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Mangimi_AltroDes_Ct"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Mangimi_AltroDes_Cm"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_Mangimi_AltroDes"), String), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_IMMAGAZZINAMENTO_Cp"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_IMMAGAZZINAMENTO_Ct"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_CONSERVAZIONE_Cp"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_CONSERVAZIONE_Ct"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_CONDIZIONAMENTO_Cp"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_CONDIZIONAMENTO_Ct"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_CONFEZIONAMENTO_Cp"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_CONFEZIONAMENTO_Ct"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_ETICHETTATURA_Cp"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_ETICHETTATURA_Ct"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_ALTRO_Cp"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_ALTRO_Ct"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("PA_ALTRODes"), String), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("CaratteristichePA_Numero"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("CaratteristichePA_TipoDes"), String), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("CaratteristichePA_Flag_Periodica_Continuativa"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("CaratteristichePA_CapacitaLavoro"), Double), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("CaratteristichePA_CapacitaStoccaggio"), Double), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("CaratteristichePA_CapacitaLavoro_UdmCod"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("CaratteristichePA_CapacitaLavoroTempo_UdmCod"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("CaratteristichePA_CapacitaStoccaggio_UdmCod"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("CaratteristichePA_Periodica_Bio"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("CaratteristichePA_Continuativa_Bio"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("CaratteristichePA_Periodica_Conv"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("CaratteristichePA_Continuativa_Conv"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("STRUTTURE_SILI_mc"), Double), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("STRUTTURE_SILI_StoccaggioCereali"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("STRUTTURE_SILI_StoccaggioProteoleaginose"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("STRUTTURE_SILI_Altro_Des"), String), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("STRUTTURE_SILI_Altro"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("STRUTTURE_CELLE_mc"), Double), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("STRUTTURE_CELLE_Vegetali"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("STRUTTURE_CELLE_Zootecnici"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("STRUTTURE_CELLE_Altro_Des"), String), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("STRUTTURE_CELLE_Altro"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("STRUTTURE_UTILIZZO_Dedicato"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("STRUTTURE_UTILIZZO_Misto"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("COMMERCIO_Ingrosso"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("COMMERCIO_Dettagliante"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("COMMERCIO_GDO"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("COMMERCIO_Amarchio"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("COMMERCIO_Altro"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("COMMERCIO_PERIODICA_Bio"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("COMMERCIO_PERIODICA_Conv"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("COMMERCIO_CONTINUATIVA_Bio"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("COMMERCIO_CONTINUATIVA_Conv"), Integer), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("COMMERCIO_AMARCHIO_Des"), String), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("Username_Creazione"), String), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("Data_Creazione"), Date), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("Validita_Inizio"), Date), _
                    CType(rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("Validita_Fine"), Date), _
                    objOpzioni.objParametri_Server_GIAS_DESTINAZIONE, _
                    rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("Data_Modifica"), _
                    rowsBIO_Notifica_SezC_PreparazioniAlimentari_R("Username_Modifica") _
                )


                Next

                dtBIO_Notifica_SezD_Importazione_R = leggiBIO_Notifica_SezD_Importazione_R.Leggi( _
                     Notifica_IDOld, _
                     "", _
                     0, _
                     "", _
                     "", _
                     objOpzioni.objParametri_Server_GIAS_ORIGINE _
                )


                For Each rowBIO_Notifica_SezD_Importazione_R In dtBIO_Notifica_SezD_Importazione_R.Rows

                    Dim l_saCod_SezD_Importazione_R As Integer = CType(rowBIO_Notifica_SezD_Importazione_R("CentroRicevimento_SaCod"), Integer)
                    Dim decode_Sa_cod As Integer = _
                        (From r In _funzioniGLOBAL.Imprese _
                         Where r.FROM_Piva = Piva_Origine _
                         And r.FROM_SaCod = l_saCod_SezD_Importazione_R _
                         Select r.TO_SaCod).FirstOrDefault



                    ScriviBIO_Notifica_SezD_Importazione_W.Scrivi( _
                        Notifica_IDNew, _
                        CType(rowBIO_Notifica_SezD_Importazione_R("NumeroOrdine"), Integer), _
                        Piva_Destinazione, _
                        decode_Sa_cod, _
                        CType(rowBIO_Notifica_SezD_Importazione_R("CentroRicevimento_RagSoc"), String), _
                        CType(rowBIO_Notifica_SezD_Importazione_R("CentroRicevimento_Via"), String), _
                        CType(rowBIO_Notifica_SezD_Importazione_R("CentroRicevimento_Numero"), String), _
                        CType(rowBIO_Notifica_SezD_Importazione_R("CentroRicevimento_CAP"), String), _
                        CType(rowBIO_Notifica_SezD_Importazione_R("CentroRicevimento_Sigla_Provincia"), String), _
                        CType(rowBIO_Notifica_SezD_Importazione_R("CentroRicevimento_CodIstat_Provincia"), String), _
                        CType(rowBIO_Notifica_SezD_Importazione_R("CentroRicevimento_Comune"), String), _
                        CType(rowBIO_Notifica_SezD_Importazione_R("CentroRicevimento_CodIstat_Comune"), String), _
                        CType(rowBIO_Notifica_SezD_Importazione_R("CentroRicevimento_Telefono"), String), _
                        CType(rowBIO_Notifica_SezD_Importazione_R("CentroRicevimento_Fax"), String), _
                        CType(rowBIO_Notifica_SezD_Importazione_R("CentroRicevimento_Email"), String), _
                        CType(rowBIO_Notifica_SezD_Importazione_R("CentroRicevimento_Flag_Proprieta_Terzi"), Integer), _
                        CType(rowBIO_Notifica_SezD_Importazione_R("TPI_PRODOTTIVEGETALI"), Integer), _
                        CType(rowBIO_Notifica_SezD_Importazione_R("TPI_ProdottiVegetali_SemiLavorati"), Integer), _
                        CType(rowBIO_Notifica_SezD_Importazione_R("TPI_ProdottiVegetali_Preparati"), Integer), _
                        CType(rowBIO_Notifica_SezD_Importazione_R("TPI_PRODOTTIANIMALI"), Integer), _
                        CType(rowBIO_Notifica_SezD_Importazione_R("TPI_ProdottiAnimali_SemiLavorati"), Integer), _
                        CType(rowBIO_Notifica_SezD_Importazione_R("TPI_ProdottiAnimali_Preparati"), Integer), _
                        CType(rowBIO_Notifica_SezD_Importazione_R("TPI_MezziTecnici"), Integer), _
                        CType(rowBIO_Notifica_SezD_Importazione_R("TPI_MatRiprodVeget"), Integer), _
                        CType(rowBIO_Notifica_SezD_Importazione_R("TPI_Altro"), Integer), _
                        CType(rowBIO_Notifica_SezD_Importazione_R("TPI_AltroDes"), String), _
                        CType(rowBIO_Notifica_SezD_Importazione_R("TSR_SILI"), Integer), _
                        CType(rowBIO_Notifica_SezD_Importazione_R("TSR_Sili_StoccaggioGranaglie"), Integer), _
                        CType(rowBIO_Notifica_SezD_Importazione_R("TSR_Sili_StoccaggioColtIndustriali"), Integer), _
                        CType(rowBIO_Notifica_SezD_Importazione_R("TSR_CELLEFRIGORIFERE"), Integer), _
                        CType(rowBIO_Notifica_SezD_Importazione_R("TSR_Celle_ProduzioniVegetali"), Integer), _
                        CType(rowBIO_Notifica_SezD_Importazione_R("TSR_Celle_ProduzioniZootecniche"), Integer), _
                        CType(rowBIO_Notifica_SezD_Importazione_R("TSR_IMPIANTIPREPARAZIONIALIM"), Integer), _
                        CType(rowBIO_Notifica_SezD_Importazione_R("TSR_ImpiantiPreparazioniAlim_Importatore"), Integer), _
                        CType(rowBIO_Notifica_SezD_Importazione_R("TSR_ImpiantiPreparazioniAlim_Esterni"), Integer), _
                        CType(rowBIO_Notifica_SezD_Importazione_R("TSR_ImpiantiPreparazioniAlim_Altro"), Integer), _
                        CType(rowBIO_Notifica_SezD_Importazione_R("TSR_ImpiantiPreparazioniAlim_AltroDes1"), String), _
                        CType(rowBIO_Notifica_SezD_Importazione_R("TSR_ImpiantiPreparazioniAlim_AltroDes2"), String), _
                        CType(rowBIO_Notifica_SezD_Importazione_R("Username_Creazione"), String), _
                        CType(rowBIO_Notifica_SezD_Importazione_R("Data_Creazione"), Date), _
                        CType(rowBIO_Notifica_SezD_Importazione_R("Validita_Inizio"), Date), _
                        CType(rowBIO_Notifica_SezD_Importazione_R("Validita_Fine"), Date), _
                        objOpzioni.objParametri_Server_GIAS_DESTINAZIONE, _
                        rowBIO_Notifica_SezD_Importazione_R("Data_Modifica"), _
                        rowBIO_Notifica_SezD_Importazione_R("Username_Modifica") _
                    )

                Next

                dtBIO_Notifica_SezE_Particelle_R = leggiBIO_Notifica_SezE_Particelle_R.Leggi( _
                    Notifica_IDOld, _
                    0, _
                    "", _
                    "", _
                    objOpzioni.objParametri_Server_GIAS_ORIGINE _
                )


                For Each rowBIO_Notifica_SezE_Particelle_R In dtBIO_Notifica_SezE_Particelle_R.Rows

                    ScriviBIO_Notifica_SezE_Particelle_W.Scrivi( _
                    Notifica_IDNew, _
                    CType(rowBIO_Notifica_SezE_Particelle_R("NumeroOrdine"), Integer), _
                    CType(rowBIO_Notifica_SezE_Particelle_R("CodIstat_Provincia"), String), _
                    CType(rowBIO_Notifica_SezE_Particelle_R("CodIstat_Comune"), String), _
                    CType(rowBIO_Notifica_SezE_Particelle_R("Sezione"), String), _
                    CType(rowBIO_Notifica_SezE_Particelle_R("Foglio"), Integer), _
                    CType(rowBIO_Notifica_SezE_Particelle_R("Numero"), Integer), _
                    CType(rowBIO_Notifica_SezE_Particelle_R("Subalterno"), String), _
                    CType(rowBIO_Notifica_SezE_Particelle_R("TitoloPossesso"), Integer), _
                    CType(rowBIO_Notifica_SezE_Particelle_R("Sup_Catastale"), Double), _
                    CType(rowBIO_Notifica_SezE_Particelle_R("Sup_Convenzionale"), Double), _
                    CType(rowBIO_Notifica_SezE_Particelle_R("Sup_Conversione"), Double), _
                    CType(rowBIO_Notifica_SezE_Particelle_R("Sup_Biologico"), Double), _
                    CType(rowBIO_Notifica_SezE_Particelle_R("Username_Creazione"), String), _
                    CType(rowBIO_Notifica_SezE_Particelle_R("Data_Creazione"), Date), _
                    CType(rowBIO_Notifica_SezE_Particelle_R("Validita_Inizio"), Date), _
                    CType(rowBIO_Notifica_SezE_Particelle_R("Validita_Fine"), Date), _
                    objOpzioni.objParametri_Server_GIAS_DESTINAZIONE, _
                    rowBIO_Notifica_SezE_Particelle_R("Data_Modifica"), _
                    rowBIO_Notifica_SezE_Particelle_R("Username_Modifica") _
                )

                Next

                dtBIO_Notifica_SezF_Appezzamenti_R = leggiBIO_Notifica_SezF_Appezzamenti_R.Leggi( _
                    Notifica_IDOld, _
                    0, _
                    0, _
                    "", _
                    "", _
                    objOpzioni.objParametri_Server_GIAS_ORIGINE, _
                    True _
                )

                For Each rowBIO_Notifica_SezF_Appezzamenti_R In dtBIO_Notifica_SezF_Appezzamenti_R.Rows

                    Dim lRowBIO_Notifica_SezF_Appezzamenti_R As Integer = CType(rowBIO_Notifica_SezF_Appezzamenti_R("Appezza"), Int32)
                    Dim AppezzaDecode As Integer = ( _
                        From a In _AppezzamentiMappati _
                        Where a.From_Piva = Piva_Origine _
                        And a.From_sa_cod = Sa_cod_Origine _
                        And a.From_appezza = lRowBIO_Notifica_SezF_Appezzamenti_R _
                        Select a.To_appezza _
                    ).FirstOrDefault

                    Dim lProgrammazione_Entita_Cod As Integer = CType(rowBIO_Notifica_SezF_Appezzamenti_R("Programmazione_Entita_Cod"), Int32)

                    Dim Entita_cod As Integer = ( _
                        From e In _funzioniGLOBAL.Planning _
                        Where e.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser _
                        And e.From_Piva = Piva_Origine _
                        And e.From_Programmazione_Entita_cod = lProgrammazione_Entita_Cod _
                        Select e.To_Programmazione_Entita_cod _
                    ).FirstOrDefault

                    Dim l_saCod_SezF_Appezzamenti_R As Integer = CType(rowBIO_Notifica_SezF_Appezzamenti_R("sa_cod"), Integer)
                    Dim decode_Sa_cod As Integer = _
                        (From r In _funzioniGLOBAL.Imprese _
                         Where r.FROM_Piva = Piva_Origine _
                         And r.FROM_SaCod = l_saCod_SezF_Appezzamenti_R _
                         Select r.TO_SaCod).FirstOrDefault


                    ScriviBIO_Notifica_SezF_Appezzamenti_W.Scrivi( _
                        Notifica_IDNew, _
                        CType(rowBIO_Notifica_SezF_Appezzamenti_R("Progressivo_Appezzamento"), Int32), _
                        CType(rowBIO_Notifica_SezF_Appezzamenti_R("Progressivo_Particelle"), Int32), _
                        CType(rowBIO_Notifica_SezF_Appezzamenti_R("Flag_Appezzamento1_Particella2"), Int32), _
                        Piva_Destinazione, _
                        decode_Sa_cod, _
                        AppezzaDecode, _
                        CType(rowBIO_Notifica_SezF_Appezzamenti_R("Progressivo_UnitaProduttiva"), Int32), _
                        CType(rowBIO_Notifica_SezF_Appezzamenti_R("Sup_Appezzamento"), Double), _
                        CType(rowBIO_Notifica_SezF_Appezzamenti_R("Data_FineImpiegoProdottiNonConformi"), Date), _
                        CType(rowBIO_Notifica_SezF_Appezzamenti_R("MetodoProduzione_Cod"), Int32), _
                        CType(rowBIO_Notifica_SezF_Appezzamenti_R("TipologiaColtura_Cod"), Int32), _
                        CType(rowBIO_Notifica_SezF_Appezzamenti_R("OrientamentoProduttivo_Cod_1"), Int32), _
                        CType(rowBIO_Notifica_SezF_Appezzamenti_R("OrientamentoProduttivo_Cod_2"), Int32), _
                        CType(rowBIO_Notifica_SezF_Appezzamenti_R("OrientamentoProduttivo_Cod_3"), Int32), _
                        CType(rowBIO_Notifica_SezF_Appezzamenti_R("OrientamentoProduttivo_Cod_4"), Int32), _
                        CType(rowBIO_Notifica_SezF_Appezzamenti_R("OrientamentoProduttivo_Cod_5"), Int32), _
                        CType(rowBIO_Notifica_SezF_Appezzamenti_R("CodIstat_Provincia"), String), _
                        CType(rowBIO_Notifica_SezF_Appezzamenti_R("CodIstat_Comune"), String), _
                        CType(rowBIO_Notifica_SezF_Appezzamenti_R("Sezione"), String), _
                        CType(rowBIO_Notifica_SezF_Appezzamenti_R("Foglio"), Int32), _
                        CType(rowBIO_Notifica_SezF_Appezzamenti_R("Numero"), Int32), _
                        CType(rowBIO_Notifica_SezF_Appezzamenti_R("Subalterno"), String), _
                        CType(rowBIO_Notifica_SezF_Appezzamenti_R("Sup_Intersezione"), Double), _
                        CType(rowBIO_Notifica_SezF_Appezzamenti_R("TitoloPossesso"), Int32), _
                        CType(rowBIO_Notifica_SezF_Appezzamenti_R("Num_Appezzamento_Str"), String), _
                        CType(rowBIO_Notifica_SezF_Appezzamenti_R("Veg_cod"), Int32), _
                        Entita_cod, _
                        CType(rowBIO_Notifica_SezF_Appezzamenti_R("Username_Creazione"), String), _
                        CType(rowBIO_Notifica_SezF_Appezzamenti_R("Data_Creazione"), Date), _
                        CType(rowBIO_Notifica_SezF_Appezzamenti_R("Validita_Inizio"), Date), _
                        CType(rowBIO_Notifica_SezF_Appezzamenti_R("Validita_Fine"), Date), _
                        objOpzioni.objParametri_Server_GIAS_DESTINAZIONE, _
                        rowBIO_Notifica_SezF_Appezzamenti_R("Data_Modifica"), _
                        rowBIO_Notifica_SezF_Appezzamenti_R("Username_Modifica") _
                    )

                Next


                dtBIO_Notifica_SezG_Fabbricati_R = leggiBIO_Notifica_SezG_Fabbricati_R.Leggi( _
                    Notifica_IDOld, _
                    0, _
                    "", _
                    "", _
                    objOpzioni.objParametri_Server_GIAS_ORIGINE _
                )

                For Each rowBIO_Notifica_SezG_Fabbricati_R In dtBIO_Notifica_SezG_Fabbricati_R.Rows

                    Dim lFabbricato_cod As Integer = CType(rowBIO_Notifica_SezG_Fabbricati_R("Fabbricato_Cod"), Integer)
                    Dim fabbricatoCod As Integer = _
                        (From f In _Fabbricati _
                         Where f.FromPiva = Piva_Origine _
                         And f.From_FabbricatoCod = lFabbricato_cod _
                         Select f.To_FabbricatoCod _
                    ).FirstOrDefault

                    Dim l_saCod_SezG_Fabbricati_R As Integer = CType(rowBIO_Notifica_SezG_Fabbricati_R("sa_cod"), Integer)
                    Dim decode_Sa_cod As Integer = _
                        (From r In _funzioniGLOBAL.Imprese _
                         Where r.FROM_Piva = Piva_Origine _
                         And r.FROM_SaCod = l_saCod_SezG_Fabbricati_R _
                         Select r.TO_SaCod).FirstOrDefault


                    ScriviBIO_Notifica_SezG_Fabbricati_W.Scrivi( _
                        Notifica_IDNew, _
                        CType(rowBIO_Notifica_SezG_Fabbricati_R("NumeroOrdine"), Integer), _
                        Piva_Destinazione, _
                        decode_Sa_cod, _
                        fabbricatoCod, _
                        CType(rowBIO_Notifica_SezG_Fabbricati_R("CodIstat_Provincia"), String), _
                        CType(rowBIO_Notifica_SezG_Fabbricati_R("CodIstat_Comune"), String), _
                        CType(rowBIO_Notifica_SezG_Fabbricati_R("Sezione"), String), _
                        CType(rowBIO_Notifica_SezG_Fabbricati_R("Foglio"), Integer), _
                        CType(rowBIO_Notifica_SezG_Fabbricati_R("Numero"), Integer), _
                        CType(rowBIO_Notifica_SezG_Fabbricati_R("Subalterno"), String), _
                        CType(rowBIO_Notifica_SezG_Fabbricati_R("TitoloPossesso"), Integer), _
                        CType(rowBIO_Notifica_SezG_Fabbricati_R("Volume_Convenzionale"), Double), _
                        CType(rowBIO_Notifica_SezG_Fabbricati_R("Volume_Conversione"), Double), _
                        CType(rowBIO_Notifica_SezG_Fabbricati_R("Volume_Biologico"), Double), _
                        CType(rowBIO_Notifica_SezG_Fabbricati_R("Indirizzo"), String), _
                        CType(rowBIO_Notifica_SezG_Fabbricati_R("Username_Creazione"), String), _
                        CType(rowBIO_Notifica_SezG_Fabbricati_R("Data_Creazione"), Date), _
                        CType(rowBIO_Notifica_SezG_Fabbricati_R("Validita_Inizio"), Date), _
                        CType(rowBIO_Notifica_SezG_Fabbricati_R("Validita_Fine"), Date), _
                        objOpzioni.objParametri_Server_GIAS_DESTINAZIONE, _
                        rowBIO_Notifica_SezG_Fabbricati_R("Data_Modifica"), _
                        rowBIO_Notifica_SezG_Fabbricati_R("Username_Modifica") _
                    )

                Next

            Next

        Catch ex As Exception

            Dim msg As String
            msg = CStr(Date.Now) + " - " + nomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)

        End Try

    End Sub


#End Region

End Class
