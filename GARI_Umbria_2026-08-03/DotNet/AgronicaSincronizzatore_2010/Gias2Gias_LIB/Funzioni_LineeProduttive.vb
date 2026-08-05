
Imports System.Xml
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

Imports AgronicaCoreDataProvider

Imports System.Text
Imports AgronicaCoreContabDAL
Imports AgronicaCoreEntityFramework_POCO


Partial Public Class Funzioni
    Public Sub Elabora_LineeProduttive_Salva( _
                                    ByVal objOpzioni As clsOpzioni, _
                                    ByRef Log_Import As StringBuilder, _
                                    ByRef Log_Errori As StringBuilder, _
                                    ByRef Log_Riepilogo As StringBuilder, _
                                    ByVal Piva_Origine As String, _
                                    ByVal Piva_Destinazione As String _
                            )

        Const nomeFunzione As String = "Elabora_LineeProduttive_Salva"


        Try

            'prima versione, copia "brutale", alla fine della copia occorre fare l'update del valore in sequenza tabella al più alto.
            Dim dtLinee_Classi_Produzioni As DataTable
            Dim dtLinee_Preparazioni As DataTable
            Dim dtLinee_Preparazioni_Dettagli As DataTable
            Dim dtLinee_Produzioni As DataTable
            Dim dtLinee_Produzioni_Mix As DataTable
            Dim dtLinee_Produzioni_Parametri As DataTable
            Dim dtLinee_Produzioni_Riferimenti As DataTable
            Dim dtLinee_ProduzionixPreparazioni As DataTable
            Dim dtLinee_Preparazioni_Report_Esclusi As DataTable
            Dim dtLinee_PreparazionixReport As DataTable


            Dim leggiLinee_Classi_Produzioni As New Linee_Classi_Produzioni_R
            Dim leggiLinee_Preparazioni As New Linee_Preparazioni_R
            Dim leggiLinee_Preparazioni_Dettagli As New Linee_Preparazioni_Dettagli_R
            Dim leggiLinee_Preparazioni_Report As New Linee_Preparazioni_Report_R
            Dim leggiLinee_Produzioni As New Linee_Produzioni_R
            Dim leggiLinee_Produzioni_Mix As New Linee_Produzioni_Mix_R
            Dim leggiLinee_Produzioni_Parametri As New Linee_Produzioni_Parametri_R
            Dim leggiLinee_Produzioni_Riferimenti As New Linee_Produzioni_Riferimenti_R
            Dim leggiLinee_ProduzionixPreparazioni As New Linee_ProduzionixPreparazioni_R
            Dim leggiLinee_Preparazioni_Report_Esclusi As New Linee_Preparazioni_Report_Esclusi_R
            Dim leggiLinee_PreparazionixReport As New Linee_Preparazionixreport_R


            Dim scriviLinee_Classi_Produzioni As New Linee_Classi_Produzioni_W
            Dim scriviLinee_Preparazioni As New Linee_Preparazioni_W
            Dim scriviLinee_Preparazioni_Dettagli As New Linee_Preparazioni_Dettagli_W
            Dim scriviLinee_Preparazioni_Report As New Linee_Preparazioni_Report_W
            Dim scriviLinee_Produzioni As New Linee_Produzioni_W
            Dim scriviLinee_Produzioni_Mix As New Linee_Produzioni_Mix_W
            Dim scriviLinee_Produzioni_Parametri As New Linee_Produzioni_Parametri_W
            Dim scriviLinee_Produzioni_Riferimenti As New Linee_Produzioni_Riferimenti_W
            Dim scriviLinee_ProduzionixPreparazioni As New Linee_ProduzionixPreparazioni_W
            Dim scriviLinee_Preparazioni_Report_Esclusi As New Linee_Preparazioni_Report_Esclusi_W
            Dim scriviLinee_PreparazionixReport As New Linee_Preparazionixreport_W


            dtLinee_Classi_Produzioni = leggiLinee_Classi_Produzioni.Leggi(Piva_Origine, 0, "", "", objOpzioni.objParametri_Server_GIAS_ORIGINE)
            dtLinee_Preparazioni = leggiLinee_Preparazioni.Leggi(Piva_Origine, 0, 0, 0, "", "", objOpzioni.objParametri_Server_GIAS_ORIGINE)
            dtLinee_Preparazioni_Dettagli = leggiLinee_Preparazioni_Dettagli.Leggi(Piva_Origine, "", "", objOpzioni.objParametri_Server_GIAS_ORIGINE)
            dtLinee_Produzioni = leggiLinee_Produzioni.LeggiLineeProduzione(Piva_Origine, "", "", objOpzioni.objParametri_Server_GIAS_ORIGINE)
            dtLinee_Produzioni_Mix = leggiLinee_Produzioni_Mix.Leggi(Piva_Origine, "", "", objOpzioni.objParametri_Server_GIAS_ORIGINE)
            dtLinee_Produzioni_Parametri = leggiLinee_Produzioni_Parametri.Leggi(Piva_Origine, "", "", objOpzioni.objParametri_Server_GIAS_ORIGINE)
            dtLinee_Produzioni_Riferimenti = leggiLinee_Produzioni_Riferimenti.Leggi(Piva_Origine, "", "", objOpzioni.objParametri_Server_GIAS_ORIGINE)
            dtLinee_ProduzionixPreparazioni = leggiLinee_ProduzionixPreparazioni.Leggi(Piva_Origine, "", "", objOpzioni.objParametri_Server_GIAS_ORIGINE)
            dtLinee_Preparazioni_Report_Esclusi = leggiLinee_Preparazioni_Report_Esclusi.Leggi(Piva_Origine, "", "", objOpzioni.objParametri_Server_GIAS_ORIGINE)
            dtLinee_PreparazionixReport = leggiLinee_PreparazionixReport.Leggi(Piva_Origine, "", "", objOpzioni.objParametri_Server_GIAS_ORIGINE)



            For Each iCurrdtLinee_Classi_Produzioni As DataRow In dtLinee_Classi_Produzioni.Rows
                scriviLinee_Classi_Produzioni.Scrivi( _
                      Piva_Destinazione _
                    , iCurrdtLinee_Classi_Produzioni("Linea_Classe_Cod") _
                    , iCurrdtLinee_Classi_Produzioni("Linea_Classe_Des") _
                    , iCurrdtLinee_Classi_Produzioni("Tipo_Produzione") _
                    , iCurrdtLinee_Classi_Produzioni("Linea_Classe_Padre_Cod") _
                    , iCurrdtLinee_Classi_Produzioni("Tipo_Classe") _
                    , iCurrdtLinee_Classi_Produzioni("Modulo_Generazione") _
                    , iCurrdtLinee_Classi_Produzioni("ChkUtility") _
                    , iCurrdtLinee_Classi_Produzioni("DirPicture") _
                    , objOpzioni.objParametri_Server_GIAS_DESTINAZIONE, _
                      iCurrdtLinee_Classi_Produzioni("Data_Creazione"), _
                      iCurrdtLinee_Classi_Produzioni("Data_Modifica"), _
                      iCurrdtLinee_Classi_Produzioni("Username_Creazione"), _
                      iCurrdtLinee_Classi_Produzioni("Username_Modifica") _
                )


            Next

            For Each iCurrdtLinee_Preparazioni As DataRow In dtLinee_Preparazioni.Rows
                scriviLinee_Preparazioni.Scrivi( _
                       Piva_Destinazione _
                    , iCurrdtLinee_Preparazioni("Preparazione_Cod") _
                    , iCurrdtLinee_Preparazioni("Preparazione_Des") _
                    , iCurrdtLinee_Preparazioni("Tipo_Produzione") _
                    , iCurrdtLinee_Preparazioni("Lav_Cod") _
                    , iCurrdtLinee_Preparazioni("Preparazione_Sigla") _
                    , iCurrdtLinee_Preparazioni("Tipo_Preparazione") _
                    , iCurrdtLinee_Preparazioni("Tipo_Durata") _
                    , iCurrdtLinee_Preparazioni("Durata") _
                    , iCurrdtLinee_Preparazioni("Udm_Cod") _
                    , iCurrdtLinee_Preparazioni("Qta") _
                    , iCurrdtLinee_Preparazioni("ChkMonitor") _
                    , iCurrdtLinee_Preparazioni("ChkAnalisi") _
                    , iCurrdtLinee_Preparazioni("Tipo_Preselezione") _
                    , iCurrdtLinee_Preparazioni("Tipo_Scarico") _
                    , iCurrdtLinee_Preparazioni("Tipo_Ripartizione") _
                    , iCurrdtLinee_Preparazioni("ChkData_Default") _
                    , iCurrdtLinee_Preparazioni("ChkCampo_Note") _
                    , iCurrdtLinee_Preparazioni("ChkOrdini") _
                    , iCurrdtLinee_Preparazioni("ChkBilancia") _
                    , iCurrdtLinee_Preparazioni("ChkScarto") _
                    , iCurrdtLinee_Preparazioni("ChkBilancia_LottoEdit") _
                    , iCurrdtLinee_Preparazioni("ChkBilancia_Abilitazione") _
                    , iCurrdtLinee_Preparazioni("Tipo_Insert") _
                    , iCurrdtLinee_Preparazioni("ChkTopDown") _
                    , iCurrdtLinee_Preparazioni("ChkDataScadenza") _
                    , iCurrdtLinee_Preparazioni("Giorni_Scadenza") _
                    , iCurrdtLinee_Preparazioni("Tipo_Save") _
                    , iCurrdtLinee_Preparazioni("Step_Save") _
                    , iCurrdtLinee_Preparazioni("ChkSalva_Continua") _
                    , iCurrdtLinee_Preparazioni("Udm_Cod_Extra") _
                    , iCurrdtLinee_Preparazioni("Cod_RisUm_Conferimento") _
                    , iCurrdtLinee_Preparazioni("ChkLotto_Unico") _
                    , iCurrdtLinee_Preparazioni("Lotto_Unico") _
                    , iCurrdtLinee_Preparazioni("ChkLotto_Componente") _
                    , iCurrdtLinee_Preparazioni("ChkLotto_Progetto") _
                    , iCurrdtLinee_Preparazioni("ChkLotto_Linea") _
                    , iCurrdtLinee_Preparazioni("ChkLotto_Ordine") _
                    , iCurrdtLinee_Preparazioni("Tipo_Lotto_Identificativo") _
                    , iCurrdtLinee_Preparazioni("Note") _
                    , iCurrdtLinee_Preparazioni("ChkDiscriminante_Lotto") _
                    , iCurrdtLinee_Preparazioni("ChkDes_Lib_Ora") _
                    , iCurrdtLinee_Preparazioni("ChkDes_Lib_Lotto") _
                    , iCurrdtLinee_Preparazioni("ChkDes_Lib_Contatto") _
                    , iCurrdtLinee_Preparazioni("Linea_Classe_Cod") _
                    , iCurrdtLinee_Preparazioni("ChkConfezionamento") _
                    , iCurrdtLinee_Preparazioni("ChkDes_Lib_Provenienza") _
                    , iCurrdtLinee_Preparazioni("ChkDes_Lib_Destinazione") _
                    , iCurrdtLinee_Preparazioni("Colore") _
                    , iCurrdtLinee_Preparazioni("ChkDocumenti") _
                    , iCurrdtLinee_Preparazioni("Tipo_Integrazione") _
                    , iCurrdtLinee_Preparazioni("Tipo_Default") _
                    , iCurrdtLinee_Preparazioni("ChkUtility") _
                    , iCurrdtLinee_Preparazioni("Tipo_Filtro_Preparati") _
                    , iCurrdtLinee_Preparazioni("Modulo_Generazione") _
                    , iCurrdtLinee_Preparazioni("Codice_Generazione") _
                    , iCurrdtLinee_Preparazioni("Tipo_Alterazione") _
                    , iCurrdtLinee_Preparazioni("ChkLink_Parametri") _
                    , iCurrdtLinee_Preparazioni("ChkLink_Utility") _
                    , iCurrdtLinee_Preparazioni("Domanda") _
                    , iCurrdtLinee_Preparazioni("ChkMix") _
                    , iCurrdtLinee_Preparazioni("ChkRisposta_Indefinita") _
                    , iCurrdtLinee_Preparazioni("validita_inizio") _
                    , iCurrdtLinee_Preparazioni("validita_fine") _
                    , objOpzioni.objParametri_Server_GIAS_DESTINAZIONE, _
                    iCurrdtLinee_Preparazioni("Data_Creazione"), _
                    iCurrdtLinee_Preparazioni("Data_Modifica"), _
                    iCurrdtLinee_Preparazioni("Username_Creazione"), _
                    iCurrdtLinee_Preparazioni("Username_Modifica") _
                )

            Next

            For Each iCurrdtLinee_Preparazioni_Dettagli In dtLinee_Preparazioni_Dettagli.Rows


                Dim lMatCod As Integer = iCurrdtLinee_Preparazioni_Dettagli("Mat_Cod")
                Dim lProCod As Integer = iCurrdtLinee_Preparazioni_Dettagli("Pro_Cod")

                Dim lNewMatCod As Integer = 0

                If lProCod = 0 Then
                    Dim lNewMaterie As G2G_Recode_MateriePrime = _
                        (From mm In _funzioniGLOBAL.Materie _
                          Where mm.From_Mat_Cod = lMatCod _
                          And mm.From_Piva = Piva_Origine _
                          And mm.To_Piva = Piva_Destinazione _
                          ).FirstOrDefault
                    If lNewMaterie Is Nothing Then
                        Throw New Exception("Mat cod errato!")
                    End If
                    lNewMatCod = lNewMaterie.To_Mat_Cod
                End If


                Dim lVecchioSaCodDestinazione As Integer = iCurrdtLinee_Preparazioni_Dettagli("Sa_Cod_Destinazione")
                Dim nuovoSaCodDestinazione As Integer = _
                    GetRicodificaSA_COD(Piva_Origine, lVecchioSaCodDestinazione,
                                        objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser,
                                        objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser)

                scriviLinee_Preparazioni_Dettagli.Scrivi( _
                  Piva_Destinazione _
                , iCurrdtLinee_Preparazioni_Dettagli("Preparazione_Cod") _
                , iCurrdtLinee_Preparazioni_Dettagli("Dettaglio_Cod") _
                , iCurrdtLinee_Preparazioni_Dettagli("Dettaglio_Des") _
                , iCurrdtLinee_Preparazioni_Dettagli("Cau_Mov") _
                , iCurrdtLinee_Preparazioni_Dettagli("Elem_Cod") _
                , lProCod _
                , lNewMatCod _
                , iCurrdtLinee_Preparazioni_Dettagli("Veg_Cod") _
                , iCurrdtLinee_Preparazioni_Dettagli("Cul_Cod") _
                , iCurrdtLinee_Preparazioni_Dettagli("Udm_Cod") _
                , iCurrdtLinee_Preparazioni_Dettagli("Mezzo") _
                , iCurrdtLinee_Preparazioni_Dettagli("Tipo_Destinazione") _
                , nuovoSaCodDestinazione _
                , iCurrdtLinee_Preparazioni_Dettagli("Id_Destinazione") _
                , iCurrdtLinee_Preparazioni_Dettagli("Qta") _
                , iCurrdtLinee_Preparazioni_Dettagli("Qta_Extra") _
                , iCurrdtLinee_Preparazioni_Dettagli("Qta_Min") _
                , iCurrdtLinee_Preparazioni_Dettagli("Qta_Max") _
                , iCurrdtLinee_Preparazioni_Dettagli("ChkBase") _
                , iCurrdtLinee_Preparazioni_Dettagli("ChkScarto") _
                , iCurrdtLinee_Preparazioni_Dettagli("ChkSomma") _
                , iCurrdtLinee_Preparazioni_Dettagli("ChkRipartizione") _
                , iCurrdtLinee_Preparazioni_Dettagli("ChkMassa_Volumica") _
                , iCurrdtLinee_Preparazioni_Dettagli("ChkIniziale") _
                , iCurrdtLinee_Preparazioni_Dettagli("ChkFinale") _
                , iCurrdtLinee_Preparazioni_Dettagli("ChkCampionatura") _
                , iCurrdtLinee_Preparazioni_Dettagli("ChkLotto_Edit") _
                , iCurrdtLinee_Preparazioni_Dettagli("ChkDisabilitazione") _
                , iCurrdtLinee_Preparazioni_Dettagli("Modulo_Generazione") _
                , iCurrdtLinee_Preparazioni_Dettagli("Codice_Generazione") _
                , iCurrdtLinee_Preparazioni_Dettagli("Filtro_Colori") _
                , iCurrdtLinee_Preparazioni_Dettagli("Filtro_Categorie") _
                , iCurrdtLinee_Preparazioni_Dettagli("Filtro_Classificazioni") _
                , iCurrdtLinee_Preparazioni_Dettagli("Filtro_Cultivar") _
                , iCurrdtLinee_Preparazioni_Dettagli("ChkIntegrazione") _
                , iCurrdtLinee_Preparazioni_Dettagli("Lotto_Default") _
                , iCurrdtLinee_Preparazioni_Dettagli("Livello_Dettaglio") _
                , iCurrdtLinee_Preparazioni_Dettagli("Filtro_Diciture") _
                , iCurrdtLinee_Preparazioni_Dettagli("validita_inizio") _
                , iCurrdtLinee_Preparazioni_Dettagli("validita_fine") _
                , objOpzioni.objParametri_Server_GIAS_DESTINAZIONE, _
                    iCurrdtLinee_Preparazioni_Dettagli("Data_Creazione"), _
                    iCurrdtLinee_Preparazioni_Dettagli("Data_Modifica"), _
                    iCurrdtLinee_Preparazioni_Dettagli("Username_Creazione"), _
                    iCurrdtLinee_Preparazioni_Dettagli("Username_Modifica") _
                )

            Next

            For Each iCurrdtLinee_Produzioni In dtLinee_Produzioni.Rows
                scriviLinee_Produzioni.Scrivi( _
                   Piva_Destinazione _
                , iCurrdtLinee_Produzioni("Linea_Cod") _
                , iCurrdtLinee_Produzioni("Linea_Cod_Des") _
                , iCurrdtLinee_Produzioni("Linea_Des") _
                , iCurrdtLinee_Produzioni("Linea_Classe_Cod") _
                , iCurrdtLinee_Produzioni("Colore") _
                , iCurrdtLinee_Produzioni("ChkVisualizzazione_Risorse") _
                , iCurrdtLinee_Produzioni("Resa") _
                , iCurrdtLinee_Produzioni("Linea_Classe_Cod_Preparazione") _
                , iCurrdtLinee_Produzioni("Colore_Default") _
                , iCurrdtLinee_Produzioni("Veg_Cod") _
                , iCurrdtLinee_Produzioni("Cul_Cod") _
                , iCurrdtLinee_Produzioni("ChkFine_Automatica") _
                , iCurrdtLinee_Produzioni("Modulo_Generazione") _
                , iCurrdtLinee_Produzioni("Denominazione") _
                , iCurrdtLinee_Produzioni("Tipo_Denominazione") _
                , iCurrdtLinee_Produzioni("Tipo_Lotto_Identificativo") _
                , iCurrdtLinee_Produzioni("Categoria_Gias_Cod") _
                , iCurrdtLinee_Produzioni("Classificazione_Gias_Cod") _
                , iCurrdtLinee_Produzioni("Linea_DPI_Cod") _
                , iCurrdtLinee_Produzioni("Linea_Modello_Cod") _
                , iCurrdtLinee_Produzioni("ChkParametri_Automatici") _
                , iCurrdtLinee_Produzioni("Reg_Cod") _
                , iCurrdtLinee_Produzioni("Grfi_Cod") _
                , iCurrdtLinee_Produzioni("Filtro_Invisibili") _
                , iCurrdtLinee_Produzioni("Cod_Contatto_Terzi") _
                , iCurrdtLinee_Produzioni("Dicitura_Gias_Cod") _
                , iCurrdtLinee_Produzioni("ChkAggiornamento") _
                , iCurrdtLinee_Produzioni("Deno_Gias_Cod") _
                , iCurrdtLinee_Produzioni("OFiltro_Denominazione") _
                , iCurrdtLinee_Produzioni("OFiltro_Colore") _
                , iCurrdtLinee_Produzioni("OFiltro_Categoria") _
                , iCurrdtLinee_Produzioni("OFiltro_Classificazione") _
                , iCurrdtLinee_Produzioni("OFiltro_Regolamento") _
                , iCurrdtLinee_Produzioni("OFiltro_Finalita") _
                , iCurrdtLinee_Produzioni("OFiltro_Dicitura") _
                , iCurrdtLinee_Produzioni("ChkControllo_Manuale") _
                , iCurrdtLinee_Produzioni("validita_inizio") _
                , iCurrdtLinee_Produzioni("validita_fine") _
                , objOpzioni.objParametri_Server_GIAS_DESTINAZIONE, _
                    iCurrdtLinee_Produzioni("Data_Creazione"), _
                    iCurrdtLinee_Produzioni("Data_Modifica"), _
                    iCurrdtLinee_Produzioni("Username_Creazione"), _
                    iCurrdtLinee_Produzioni("Username_Modifica") _
                )



            Next

            For Each iCurrdtLinee_Produzioni_Mix In dtLinee_Produzioni_Mix.Rows


                Dim lVecchioSaCod As Integer = iCurrdtLinee_Produzioni_Mix("Sa_Cod")
                Dim nuovoSaCod As Integer = GetRicodificaSA_COD(Piva_Origine, lVecchioSaCod,
                                                                objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser,
                                                                objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser)

                scriviLinee_Produzioni_Mix.Scrivi( _
                      objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser _
                    , Piva_Destinazione _
                    , nuovoSaCod _
                    , iCurrdtLinee_Produzioni_Mix("Linea_Cod") _
                    , iCurrdtLinee_Produzioni_Mix("Veg_Cod") _
                    , iCurrdtLinee_Produzioni_Mix("Cul_Cod") _
                    , iCurrdtLinee_Produzioni_Mix("Gen_Cod") _
                    , iCurrdtLinee_Produzioni_Mix("Spe_Cod") _
                    , iCurrdtLinee_Produzioni_Mix("Raz_Cod") _
                    , iCurrdtLinee_Produzioni_Mix("Livello") _
                    , objOpzioni.objParametri_Server_GIAS_DESTINAZIONE, _
                    iCurrdtLinee_Produzioni_Mix("Data_Creazione"), _
                    iCurrdtLinee_Produzioni_Mix("Data_Modifica"), _
                    iCurrdtLinee_Produzioni_Mix("Username_Creazione"), _
                    iCurrdtLinee_Produzioni_Mix("Username_Modifica") _
                    )

            Next

            For Each iCurrdtLinee_Produzioni_Parametri In dtLinee_Produzioni_Parametri.Rows


                Dim lMatCod As Integer = iCurrdtLinee_Produzioni_Parametri("Mat_Cod")
                Dim lProCod As Integer = iCurrdtLinee_Produzioni_Parametri("Pro_Cod")

                Dim lNewMatCod As Integer = 0

                If lProCod = 0 Then
                    Dim lNewMaterie As G2G_Recode_MateriePrime = _
                        (From mm In _funzioniGLOBAL.Materie _
                          Where mm.From_Mat_Cod = lMatCod _
                          And mm.From_Piva = Piva_Origine _
                          And mm.To_Piva = Piva_Destinazione _
                          ).FirstOrDefault
                    If lNewMaterie Is Nothing Then
                        Throw New Exception("Mat cod errato!")
                    End If
                    lNewMatCod = lNewMaterie.To_Mat_Cod
                End If



                scriviLinee_Produzioni_Parametri.Scrivi( _
                       objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser _
                    , Piva_Destinazione _
                    , iCurrdtLinee_Produzioni_Parametri("Linea_Cod") _
                    , iCurrdtLinee_Produzioni_Parametri("Linea_Cod_Par") _
                    , iCurrdtLinee_Produzioni_Parametri("Tipo") _
                    , iCurrdtLinee_Produzioni_Parametri("Tipo_Cod") _
                    , iCurrdtLinee_Produzioni_Parametri("Elem_Cod") _
                    , lProCod _
                    , lNewMatCod _
                    , iCurrdtLinee_Produzioni_Parametri("Udm_Cod") _
                    , iCurrdtLinee_Produzioni_Parametri("Valore_Min") _
                    , iCurrdtLinee_Produzioni_Parametri("Valore_Max") _
                    , iCurrdtLinee_Produzioni_Parametri("Note") _
                    , iCurrdtLinee_Produzioni_Parametri("Modulo_Generazione") _
                    , iCurrdtLinee_Produzioni_Parametri("Tipo_Generazione") _
                    , iCurrdtLinee_Produzioni_Parametri("Codice_Generazione") _
                    , iCurrdtLinee_Produzioni_Parametri("validita_inizio") _
                    , iCurrdtLinee_Produzioni_Parametri("validita_fine") _
                    , objOpzioni.objParametri_Server_GIAS_DESTINAZIONE, _
                    iCurrdtLinee_Produzioni_Parametri("Data_Creazione"), _
                    iCurrdtLinee_Produzioni_Parametri("Data_Modifica"), _
                    iCurrdtLinee_Produzioni_Parametri("Username_Creazione"), _
                    iCurrdtLinee_Produzioni_Parametri("Username_Modifica") _
                )


            Next


            For Each iCurrdtLinee_Produzioni_Riferimenti In dtLinee_Produzioni_Riferimenti.Rows

                Dim lSaCodVecchio As Integer = iCurrdtLinee_Produzioni_Riferimenti("Sa_Cod")
                Dim nuovoSaCod As Integer = GetRicodificaSA_COD(Piva_Origine, lSaCodVecchio,
                                                                objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser,
                                                                objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser)

                scriviLinee_Produzioni_Riferimenti.Scrivi( _
                   objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser _
                , Piva_Destinazione _
                , nuovoSaCod _
                , iCurrdtLinee_Produzioni_Riferimenti("Linea_Cod") _
                , iCurrdtLinee_Produzioni_Riferimenti("Linea_Cod_Rif") _
                , iCurrdtLinee_Produzioni_Riferimenti("Preparazione_Cod") _
                , iCurrdtLinee_Produzioni_Riferimenti("Mat_Cod") _
                , iCurrdtLinee_Produzioni_Riferimenti("Mat_Cod_Rif") _
                , iCurrdtLinee_Produzioni_Riferimenti("Tipo") _
                , iCurrdtLinee_Produzioni_Riferimenti("Codice_Generazione") _
                , iCurrdtLinee_Produzioni_Riferimenti("Codice_Generazione_Rif") _
                , iCurrdtLinee_Produzioni_Riferimenti("Preparazione_Cod_Modello") _
                , iCurrdtLinee_Produzioni_Riferimenti("validita_inizio") _
                , iCurrdtLinee_Produzioni_Riferimenti("validita_fine") _
                , objOpzioni.objParametri_Server_GIAS_DESTINAZIONE, _
                iCurrdtLinee_Produzioni_Riferimenti("Data_Creazione"), _
                iCurrdtLinee_Produzioni_Riferimenti("Data_Modifica"), _
                iCurrdtLinee_Produzioni_Riferimenti("Username_Creazione"), _
                iCurrdtLinee_Produzioni_Riferimenti("Username_Modifica") _
            )


            Next

            For Each iCurrdtLinee_ProduzionixPreparazioni In dtLinee_ProduzionixPreparazioni.Rows
                scriviLinee_ProduzionixPreparazioni.Scrivi( _
                       objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser _
                    , iCurrdtLinee_ProduzionixPreparazioni("Linea_Cod") _
                    , iCurrdtLinee_ProduzionixPreparazioni("Preparazione_Cod") _
                    , iCurrdtLinee_ProduzionixPreparazioni("Livello") _
                    , iCurrdtLinee_ProduzionixPreparazioni("ChkLoop_Start") _
                    , iCurrdtLinee_ProduzionixPreparazioni("ChkLoop_End") _
                    , iCurrdtLinee_ProduzionixPreparazioni("Preparazione_Cod_Vincolo") _
                    , iCurrdtLinee_ProduzionixPreparazioni("Opzionale") _
                    , iCurrdtLinee_ProduzionixPreparazioni("Invisibile") _
                    , iCurrdtLinee_ProduzionixPreparazioni("Linea_Modello_Cod") _
                    , iCurrdtLinee_ProduzionixPreparazioni("Qta_Piano") _
                    , iCurrdtLinee_ProduzionixPreparazioni("validita_inizio") _
                    , iCurrdtLinee_ProduzionixPreparazioni("validita_fine") _
                    , objOpzioni.objParametri_Server_GIAS_DESTINAZIONE, _
                    iCurrdtLinee_ProduzionixPreparazioni("Data_Creazione"), _
                    iCurrdtLinee_ProduzionixPreparazioni("Data_Modifica"), _
                    iCurrdtLinee_ProduzionixPreparazioni("Username_Creazione"), _
                    iCurrdtLinee_ProduzionixPreparazioni("Username_Modifica") _
                )

            Next

            For Each iCurrdtLinee_Preparazioni_Report_Esclusi In dtLinee_Preparazioni_Report_Esclusi.Rows
                scriviLinee_Preparazioni_Report_Esclusi.Scrivi( _
                                  Piva_Destinazione _
                                , iCurrdtLinee_Preparazioni_Report_Esclusi("Preparazione_Cod") _
                                , iCurrdtLinee_Preparazioni_Report_Esclusi("Dettaglio_Cod") _
                                , iCurrdtLinee_Preparazioni_Report_Esclusi("Id_Report") _
                                , iCurrdtLinee_Preparazioni_Report_Esclusi("validita_inizio") _
                                , iCurrdtLinee_Preparazioni_Report_Esclusi("validita_fine") _
                                , objOpzioni.objParametri_Server_GIAS_DESTINAZIONE, _
                                iCurrdtLinee_Preparazioni_Report_Esclusi("Data_Creazione"), _
                                iCurrdtLinee_Preparazioni_Report_Esclusi("Data_Modifica"), _
                                iCurrdtLinee_Preparazioni_Report_Esclusi("Username_Creazione"), _
                                iCurrdtLinee_Preparazioni_Report_Esclusi("Username_Modifica") _
                )


            Next

            For Each iCurrdtLinee_PreparazionixReport In dtLinee_PreparazionixReport.Rows
                scriviLinee_PreparazionixReport.Scrivi( _
                     Piva_Destinazione _
                    , iCurrdtLinee_PreparazionixReport("Preparazione_Cod") _
                    , iCurrdtLinee_PreparazionixReport("Id_Report") _
                    , iCurrdtLinee_PreparazionixReport("StrCampo_Registri") _
                    , iCurrdtLinee_PreparazionixReport("validita_inizio") _
                    , iCurrdtLinee_PreparazionixReport("validita_fine") _
                    , objOpzioni.objParametri_Server_GIAS_DESTINAZIONE _
                    , iCurrdtLinee_PreparazionixReport("Data_Creazione"), _
                    iCurrdtLinee_PreparazionixReport("Data_Modifica"), _
                    iCurrdtLinee_PreparazionixReport("Username_Creazione"), _
                    iCurrdtLinee_PreparazionixReport("Username_Modifica") _
                )



            Next


            'aggiustamento di sequenza_tabelle
            Dim stb As New StringBuilder
            Dim exer As New AgronicaCoreDataProvider.DataProvider

            stb.Length = 0
            stb.Append(" update Sequenza_tabelle  " & vbCrLf)
            stb.Append(" set Ultimo_Valore = coalesce((select max(linea_cod)+1 from Linee_Produzioni), 1) " & vbCrLf)
            stb.Append(" where nome_Tabella = 'lineaproduzione' " & vbCrLf)
            exer.EseguiQuery_Scrittura(objOpzioni.objParametri_Server_GIAS_DESTINAZIONE, stb.ToString, "")

            stb.Length = 0
            stb.Append(" update Sequenza_tabelle  " & vbCrLf)
            stb.Append(" set Ultimo_Valore = coalesce((select max(Linea_Classe_Cod)+1 from Linee_Classi_Produzioni), 1) " & vbCrLf)
            stb.Append(" where nome_Tabella = 'linea_classe_produzione' " & vbCrLf)
            exer.EseguiQuery_Scrittura(objOpzioni.objParametri_Server_GIAS_DESTINAZIONE, stb.ToString, "")



        Catch ex As Exception

            Dim msg As String
            msg = CStr(Date.Now) + " - " + nomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)

        End Try

    End Sub


End Class
