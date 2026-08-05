Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class QueryHelper

    Private _objParams As ObjParametri

    Private Const ColonnaClasseSigla = Utility_Lavorazioni_Macchina_Costanti.ColonnaClasseSigla
    Private Const ColonnaClasseDescrizione = Utility_Lavorazioni_Macchina_Costanti.ColonnaClasseDescrizione

    Public Sub New(objParams As ObjParametri)
        _objParams = objParams
    End Sub

    Friend Function leggiLavorazioneLotto(piva As String, lotto As String) As DataTable
        Dim ObjMovDet As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
        ' Preparazione filtro aggiuntivo
        Dim filtriAggiuntivi As String = OttieniFiltroLottoLavAperte(lotto, LAVCOD_TRASFORMAZIONI)
        ' Esecuzione lettura lavorazione
        Return ObjMovDet.Leggi(piva, 0, 0, 0, 0, 0, 0, 0, CAU_CARICO, 0, 0, 0, 0, 0, 0,
                               enumSelezioneVariabile.Selezione_JoinCompleta,
                               filtriAggiuntivi, String.Empty, _objParams.Server)
    End Function

    Friend Function leggiOrdineLavoroLotto(piva As String, lotto As String) As DataTable
        Dim ObjMovDet As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
        ' Preparazione filtro aggiuntivo
        Dim filtriAggiuntivi As String = OttieniFiltroLottoLavAperte(lotto, LAVCOD_TESTATE_ORDINE_LAVORAZIONE)
        ' Esecuzione lettura ordine lavorazione
        Return ObjMovDet.Leggi(piva, 0, 0, 0, 0, 0, 0, 0, CAU_CARICO, 0, 0, 0, 0, 0, 0,
                               enumSelezioneVariabile.Selezione_JoinCompleta,
                               filtriAggiuntivi, String.Empty, _objParams.Server)
    End Function

    Private Function OttieniFiltroLottoLavAperte(lotto As String, lav_cod As String) As String
        Dim filtriAggiuntivi As String
        filtriAggiuntivi = " AGENDA.LAV_COD = {0} "
        filtriAggiuntivi += "AND MOVIMENTI_DETTAGLI.LOTTO = '{1}' "
        filtriAggiuntivi += "AND MOVIMENTI_DETTAGLI.JOLLY_INT = {2} "
        filtriAggiuntivi += "AND MOVIMENTI_DETTAGLI.EXTRA_INT > 0 "   ' Filtro necessario per ordini lavorazione
        filtriAggiuntivi += "AND MOVIMENTI_DETTAGLI.EXTRA_STR <> '' " ' Filtro necessario per ordini lavorazione
        filtriAggiuntivi += "AND AGENDA.ID_AGENDA IN ( "
        filtriAggiuntivi += "SELECT MOV2.ID_AGENDA FROM MOVIMENTI MOV2 "
        filtriAggiuntivi += "WHERE MOV2.ID_AGENDA = AGENDA.ID_AGENDA AND MOV2.CAU_MOV = {3} AND MOV2.EXTRA_INT = 0 "
        filtriAggiuntivi += ") "
        Dim filtriAggiuntiviFormat As String = String.Format(filtriAggiuntivi,
                                                             lav_cod,
                                                             lotto,
                                                             MagazzinoNONMovimentato,
                                                             CAU_LINEA_PRODUZIONE)
        Return filtriAggiuntiviFormat
    End Function

    Friend Function LeggiMatDes(piva As String, elemCod As String, matCod As String) As String
        Dim ObjMatPrime As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
        ' Esecuzione lettura materie prime
        Return ObjMatPrime.MatDes_from_MatCod(piva, elemCod, matCod, String.Empty, String.Empty, String.Empty, _objParams.Server)
    End Function

    Friend Function LeggiPreparazioneDes(piva As String, preparazioneCod As Integer) As String
        Dim ObjLinPrep As New AgronicaCoreContabDAL.Linee_Preparazioni_R
        ' Esecuzione lettura linee preparazione
        Return ObjLinPrep.PreparazioneDes_From_PreparazioneCod(piva, preparazioneCod, _objParams.Server)
    End Function

    Friend Function LeggiPreparazione(piva As String, preparazioneCod As Integer) As DataTable
        Dim ObjPreparazioni As New AgronicaCoreContabDAL.Linee_Preparazioni_R
        Return ObjPreparazioni.Leggi(piva,
                                     preparazioneCod,
                                     0, 0, String.Empty, String.Empty,
                                     _objParams.Server)
    End Function

    Friend Function LeggiMateriePrimeCampionature(progressivo As Integer) As DataTable
        Dim ObjMatPriCamp As New AgronicaCoreContabDAL.Materie_Prime_Campionature_R
        ' Esecuzione lettura materie prime campionature
        Return ObjMatPriCamp.Leggi(progressivo, String.Empty, 0, 0, 0, String.Empty, True,
                                   enumSelezioneVariabile.Selezione_TabellaCompleta, String.Empty, String.Empty,
                                   _objParams.Server)
    End Function

    Friend Sub LeggiMateriPrime(piva As String, matCod As Integer, ByRef vegCod As Integer, ByRef culCod As Integer)
        Dim leggi_Materie_Prime As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
        vegCod = 0
        culCod = 0
        Dim DtMateriePrime = leggi_Materie_Prime.Leggi2(piva, 0, matCod, "", 0, "", True, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", _objParams.Server)
        If DtMateriePrime.Rows.Count > 0 Then
            vegCod = DtMateriePrime.Rows(0).Item("Veg_Cod")
            culCod = DtMateriePrime.Rows(0).Item("Cul_Cod")
        End If
    End Sub

    Friend Function LeggiCalibroDaSigla(piva As String,
                                        vegCod As Integer,
                                        culCod As Integer,
                                        codiceClasse As String) As Integer

        Dim tipoCod As Integer

        tipoCod = LeggiParametroQualitativoDaClasse(piva,
                                                    vegCod,
                                                    culCod,
                                                    ParametriQualitativi_Calibro,
                                                    codiceClasse,
                                                    ColonnaClasseSigla)

        Return tipoCod

    End Function

    Friend Function LeggiCalibroDaDescrizione(piva As String,
                                              vegCod As Integer,
                                              culCod As Integer,
                                              codiceClasse As String) As Integer

        Dim tipoCod As Integer

        tipoCod = LeggiParametroQualitativoDaClasse(piva,
                                                    vegCod,
                                                    culCod,
                                                    ParametriQualitativi_Calibro,
                                                    codiceClasse,
                                                    ColonnaClasseDescrizione)

        Return tipoCod

    End Function

    Friend Function LeggiQualitaDaSigla(piva As String,
                                        vegCod As Integer,
                                        culCod As Integer,
                                        codiceClasse As String) As Integer

        Dim tipoCod As Integer

        tipoCod = LeggiParametroQualitativoDaClasse(piva,
                                                    vegCod,
                                                    culCod,
                                                    ParametriQualitativi_Qualita,
                                                    codiceClasse,
                                                    ColonnaClasseSigla)

        Return tipoCod

    End Function

    Friend Function LeggiQualitaDaDescrizione(piva As String,
                                              vegCod As Integer,
                                              culCod As Integer,
                                              codiceClasse As String) As Integer

        Dim tipoCod As Integer

        tipoCod = LeggiParametroQualitativoDaClasse(piva,
                                                    vegCod,
                                                    culCod,
                                                    ParametriQualitativi_Qualita,
                                                    codiceClasse,
                                                    ColonnaClasseDescrizione)

        Return tipoCod

    End Function

    Friend Function LeggiParametroQualitativoDaClasse(piva As String,
                                                      vegCod As Integer,
                                                      culCod As Integer,
                                                      tipoClasse As String,
                                                      codiceClasse As String,
                                                      colonnaClasse As String) As Integer
        Dim tipoCod As Integer

        Dim dtOTabelle As New DataTable
        Dim righeTrovate As Integer = 0
        ' Lettura per specie e varietà
        If vegCod <> 0 And culCod <> 0 Then
            dtOTabelle = LeggiOTabelle(piva, vegCod, culCod, tipoClasse, codiceClasse, colonnaClasse)
            righeTrovate = dtOTabelle.Rows.Count
        End If
        ' Lettura per specie
        If righeTrovate <> 1 AndAlso vegCod <> 0 Then
            dtOTabelle = LeggiOTabelle(piva, vegCod, 0, tipoClasse, codiceClasse, colonnaClasse)
            righeTrovate = dtOTabelle.Rows.Count
        End If
        ' Lettura generica
        If righeTrovate <> 1 Then
            dtOTabelle = LeggiOTabelle(piva, 0, 0, tipoClasse, codiceClasse, colonnaClasse)
            righeTrovate = dtOTabelle.Rows.Count
        End If
        Select Case True
            Case righeTrovate = 1
                tipoCod = dtOTabelle.Rows(0).Item("Tabella_Par_Cod")
                If tipoCod = 0 Then
                    Throw New Exception(String.Format("Classe non ammessa: {0} (TipoClasse={1})", codiceClasse, tipoClasse))
                End If
            Case righeTrovate = 0
                Throw New Exception(String.Format("Classe non trovata: {0} (TipoClasse={1})", codiceClasse, tipoClasse))
            Case righeTrovate > 1
                Throw New Exception(String.Format("Classe con più corrispondenze: {0} (TipoClasse={1})", codiceClasse, tipoClasse))
        End Select

        Return tipoCod
    End Function

    Private Function LeggiOTabelle(piva As String,
                                   vegCod As Integer,
                                   culCod As Integer,
                                   tipoClasse As String,
                                   codiceClasse As String,
                                   colonnaClasse As String) As DataTable

        Dim ObjLetturaOTabelle As New AgronicaCoreAnagrafeDAL.OTabelle_R
        Dim filtriAggiuntivi As String
        filtriAggiuntivi = " OTABELLE_PARAMETRI.{3} = '{0}' "
        If vegCod = 0 Then
            filtriAggiuntivi += "AND OTABELLE_PARAMETRI.OFILTRO_VEG_COD IN ('0','') "
        Else
            filtriAggiuntivi += "AND OTABELLE_PARAMETRI.OFILTRO_VEG_COD LIKE '%|" & vegCod.ToString & "|%' "
        End If
        If culCod = 0 Then
            filtriAggiuntivi += "AND OTABELLE_PARAMETRI.OFILTRO_CUL_COD IN ('0','') "
        Else
            filtriAggiuntivi += "AND OTABELLE_PARAMETRI.OFILTRO_CUL_COD LIKE '%|" & culCod.ToString & "|%' "
        End If
        filtriAggiuntivi += "AND OTABELLE_PARAMETRI.TABELLA_COD IN ("
        filtriAggiuntivi += "SELECT MODREFCFG.TABELLA_ID FROM OMODULI_REFERENZE_CONFIG_DETTAGLI MODREFCFG "
        filtriAggiuntivi += "WHERE MODREFCFG.PIVA = '{1}' AND MODREFCFG.TABELLA_KEY = '{2}' "
        filtriAggiuntivi += ") "
        Dim filtriAggiuntiviFormat As String = String.Format(filtriAggiuntivi,
                                                             codiceClasse,
                                                             piva,
                                                             tipoClasse.Substring(1),
                                                             colonnaClasse)
        Return ObjLetturaOTabelle.LeggiParametri(piva, 0, String.Empty, filtriAggiuntiviFormat, _objParams.Server)
    End Function

    Friend Function LeggiMatCodImballo(piva As String,
                                       vegCod As Integer,
                                       culCod As Integer,
                                       tabellaParCod As Integer) As Integer
        Dim matCodImballo As Integer
        Dim tipoParametro = ParametriQualitativi_Imballaggio

        Dim dtOTabelle As New DataTable
        Dim righeTrovate As Integer = 0
        ' Lettura per specie e varietà
        If vegCod <> 0 And culCod <> 0 Then
            dtOTabelle = LeggiOTabelleParCod(piva, vegCod, culCod, tipoParametro, tabellaParCod)
            righeTrovate = dtOTabelle.Rows.Count
        End If
        ' Lettura per specie
        If righeTrovate <> 1 AndAlso vegCod <> 0 Then
            dtOTabelle = LeggiOTabelleParCod(piva, vegCod, 0, tipoParametro, tabellaParCod)
            righeTrovate = dtOTabelle.Rows.Count
        End If
        ' Lettura generica
        If righeTrovate <> 1 Then
            dtOTabelle = LeggiOTabelleParCod(piva, 0, 0, tipoParametro, tabellaParCod)
            righeTrovate = dtOTabelle.Rows.Count
        End If
        Select Case True
            Case righeTrovate = 1
                matCodImballo = dtOTabelle.Rows(0).Item("Mat_Cod_Generazione_Link")
                If matCodImballo = 0 Then
                    Throw New Exception("Codice imballo non indicato: " & tabellaParCod.ToString())
                End If
            Case righeTrovate = 0
                Throw New Exception("Codice imballo non trovato: " & tabellaParCod.ToString())
            Case righeTrovate > 1
                Throw New Exception("Codice imballo con più corrispondenze: " & tabellaParCod.ToString())
        End Select

        Return matCodImballo
    End Function

    Private Function LeggiOTabelleParCod(piva As String,
                                           vegCod As Integer,
                                           culCod As Integer,
                                           tipoParametro As String,
                                           tabellaParCod As Integer) As DataTable

        Dim ObjLetturaOTabelle As New AgronicaCoreAnagrafeDAL.OTabelle_R
        Dim filtriAggiuntivi As String
        filtriAggiuntivi = " OTABELLE_PARAMETRI.TABELLA_PAR_COD = {0} "
        If vegCod = 0 Then
            filtriAggiuntivi += "AND OTABELLE_PARAMETRI.OFILTRO_VEG_COD IN ('0','') "
        Else
            filtriAggiuntivi += "AND OTABELLE_PARAMETRI.OFILTRO_VEG_COD LIKE '%|" & vegCod.ToString & "|%' "
        End If
        If culCod = 0 Then
            filtriAggiuntivi += "AND OTABELLE_PARAMETRI.OFILTRO_CUL_COD IN ('0','') "
        Else
            filtriAggiuntivi += "AND OTABELLE_PARAMETRI.OFILTRO_CUL_COD LIKE '%|" & culCod.ToString & "|%' "
        End If
        filtriAggiuntivi += "AND OTABELLE_PARAMETRI.TABELLA_COD IN ("
        filtriAggiuntivi += "SELECT MODREFCFG.TABELLA_ID FROM OMODULI_REFERENZE_CONFIG_DETTAGLI MODREFCFG "
        filtriAggiuntivi += "WHERE MODREFCFG.PIVA = '{1}' AND MODREFCFG.TABELLA_KEY = '{2}' "
        filtriAggiuntivi += ") "
        Dim filtriAggiuntiviFormat As String = String.Format(filtriAggiuntivi,
                                                             tabellaParCod,
                                                             piva,
                                                             tipoParametro.Substring(1))
        Return ObjLetturaOTabelle.LeggiParametri(piva, 0, String.Empty, filtriAggiuntiviFormat, _objParams.Server)
    End Function

    Friend Function LeggiPesoTeoricoDaConfigImballi(piva As String,
                                                    matCod As Integer,
                                                    vegCod As Integer,
                                                    culCod As Integer) As Decimal
        Dim pesoTeoricoImballo As Decimal = 0

        Dim dtCfgImballi As New DataTable
        Dim righeTrovate As Integer = 0
        ' Lettura per specie e varietà
        If vegCod <> 0 And culCod <> 0 Then
            dtCfgImballi = LeggiConfigImballi(piva, matCod, vegCod, culCod)
            righeTrovate = dtCfgImballi.Rows.Count
        End If
        ' Lettura per specie
        If righeTrovate <> 1 AndAlso vegCod <> 0 Then
            dtCfgImballi = LeggiConfigImballi(piva, matCod, vegCod, 0)
            righeTrovate = dtCfgImballi.Rows.Count
        End If
        ' Lettura generica
        If righeTrovate <> 1 Then
            dtCfgImballi = LeggiConfigImballi(piva, matCod, 0, 0)
            righeTrovate = dtCfgImballi.Rows.Count
        End If
        Select Case True
            Case righeTrovate = 1
                pesoTeoricoImballo = dtCfgImballi.Rows(0).Item("Valore")
            Case righeTrovate = 0
                Throw New Exception("Peso teorico imballo non trovato: " & matCod.ToString())
            Case righeTrovate > 1
                Throw New Exception("Imballo con più pesi teorici indicati: " & matCod.ToString())
        End Select

        Return pesoTeoricoImballo
    End Function

    Friend Function LeggiConfigImballi(piva As String,
                                       matCod As Integer,
                                       vegCod As Integer,
                                       culCod As Integer) As DataTable
        Dim ObjLetturaCfgImballi As New AgronicaCoreAnagrafeDAL.Configurazione_Imballaggi_R
        Return ObjLetturaCfgImballi.Leggi(0, piva, 0, matCod, vegCod, culCod,
                                          "", "", _objParams.Server)

    End Function

    Friend Function LeggiMacchinaLav(piva As String, idAgenda As Integer) As String

        Dim codMacchinaLav As String = String.Empty

        Dim dtMovimenti As New DataTable
        Dim objLetturaMovimenti As New AgronicaCoreContabDAL.Movimenti_R
        dtMovimenti = objLetturaMovimenti.Leggi(piva, Sa_Cod:=0, idAgenda, Id_Mov:=0, Cod_RisUm:=0,
                                                CAU_LINEA_PRODUZIONE, enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                xFiltroAggiuntivo:=String.Empty,
                                                xOrderBy:=String.Empty,
                                                _objParams.Server)
        If dtMovimenti.Rows.Count = 1 Then
            codMacchinaLav = dtMovimenti.Rows(0).Item("Cod_Macchina_Lav")
        End If

        Return codMacchinaLav

    End Function

    Friend Function LeggiMacchinaLavDaIdServizio(piva As String, idServizio As String) As String

        Dim codMacchinaLav As String = String.Empty

        Dim objLineeMacchLav As New AgronicaCoreContabDAL.Linee_Macchine_Lavorazione_R

        Dim contestoRicezione = [Enum].GetName(GetType(enum_Contesto_Integrazione_Macchine_Lavorazione), 100)
        Dim ricezioneIdServizio = contestoRicezione & "=" & Trim(idServizio)
        Dim filtriAggiuntivi = String.Format(" PARAMETRI_IMPORT LIKE '%{0}%' ", ricezioneIdServizio)

        Dim dtLineeMacchLav = objLineeMacchLav.Leggi(piva, "", "",
                                                     filtriAggiuntivi, "",
                                                     _objParams.Server)

        If dtLineeMacchLav.Rows.Count > 0 Then
            For Each macchina As DataRow In dtLineeMacchLav.Rows
                Dim parametriImport = macchina.Item("Parametri_Import")
                Dim elencoParametri = Split(parametriImport, ";")
                For Each parametro In elencoParametri
                    If parametro = ricezioneIdServizio Then
                        codMacchinaLav = macchina.Item("codice")
                        Exit For
                    End If
                Next
                If Not String.IsNullOrEmpty(codMacchinaLav) Then
                    Exit For
                End If
            Next
        End If

        Return codMacchinaLav

    End Function

    Friend Function LeggiIdServizioDaMacchinaLav(piva As String, codMacchinaLav As String) As String

        Dim idServizio As String = String.Empty

        Dim objLineeMacchLav As New AgronicaCoreContabDAL.Linee_Macchine_Lavorazione_R

        Dim contestoRicezione = [Enum].GetName(GetType(enum_Contesto_Integrazione_Macchine_Lavorazione), 100)

        Dim dtLineeMacchLav = objLineeMacchLav.Leggi(piva, codMacchinaLav, "",
                                                     "", "",
                                                     _objParams.Server)

        If dtLineeMacchLav.Rows.Count = 1 Then
            Dim parametriImport = dtLineeMacchLav.Rows(0).Item("Parametri_Import")
            Dim elencoParametri = Split(parametriImport, ";")
            For Each parametro In elencoParametri
                Dim elementiParametro = Split(parametro, "=")
                If elementiParametro.Count >= 2 AndAlso elementiParametro(0) = contestoRicezione Then
                    idServizio = elementiParametro(1)
                    Exit For
                End If
            Next
        End If

        Return idServizio

    End Function

End Class