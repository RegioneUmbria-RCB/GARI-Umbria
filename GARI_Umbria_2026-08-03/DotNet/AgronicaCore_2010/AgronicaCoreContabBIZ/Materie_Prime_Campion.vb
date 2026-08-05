Imports System.Text
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreContabDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri


Public Class Materie_Prime_Campionature_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi_Progressivo_Calibro(Progr As Integer, Descrizione As String,
                                               Peso_Campione As Integer, Prog_Orig As Integer,
                                               Piva_Superuser_Orig As String,
                                               Validita_Inizio As Date, Validita_Fine As Date,
                                               objParams_Server As AgronicaCoreParametri) As Boolean

        Dim Materie_Prime_Campionature As New AgronicaCoreContabDAL.Materie_Prime_Campion_W

        Return Materie_Prime_Campionature.Scrivi(Progr, "calibro", 12,
                                          0, 0, Descrizione,
                                          Peso_Campione, Prog_Orig, Piva_Superuser_Orig,
                                          Validita_Inizio, Validita_Fine,
                                          objParams_Server)

    End Function

    ''' <summary>
    ''' Richiamata da servizio GSB, aggiornamento parametri qualitativi di ISCC in base al valore di Compliance_ISCC dell'impresa fornitore 
    ''' </summary>
    ''' <param name="objParametriServer"></param>
    ''' <param name="dataRif">Aggiorna solo per conferimenti creati o Compliance_ISCC dei fornitori aggiornati dopo questa data</param>
    ''' <returns></returns>
    Public Function Aggiorna_Parametri_Qualitativi_ISCC(ByVal paramQualTipoName_ISCCCurrVal As String,
                                                        ByVal paramQualTipoName_ISCCPrecVal As String,
                                                        ByVal paramQualTipoName_ISCCCurrData As String,
                                                        ByVal paramQualTipoName_ISCCPrecData As String,
                                                        ByVal objParametriServer As AgronicaCoreParametri,
                                                        Optional dataRif As Date = Nothing
                                                        ) As Integer

        Dim nomeRoutine As String = "AgronicaCoreContabBIZ.Materie_Prime_Campionature_W.Aggiorna_ISCC_Imprese_Compliance"

        Dim xRisp As Integer = 0

        'I valori possbili di Tipo_Cod per parametro "oiscccorrente"
        Dim oTabellaParamCode_ISCCCorrente_NULL As Integer = 0
        Dim oTabellaParamCode_ISCCCorrente_Compliant As Integer = 0
        Dim oTabellaParamCode_ISCCCorrente_NotCompliant As Integer = 0

        'I valori possbili di Tipo_Cod per parametro "oisccprecedente"
        Dim oTabellaParamCode_ISCCPrecedente_NULL As Integer = 0
        Dim oTabellaParamCode_ISCCPrecedente_Compliant As Integer = 0
        Dim oTabellaParamCode_ISCCPrecedente_NotCompliant As Integer = 0

        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False

        Dim dtMatPrimeCamp As New DataTable

        Try

            Utility.VerificaApriTransazione(objParametriServer, flagConnessione, flagTransazione)

            LeggiISCCParametriTipoCodValues(oTabellaParamCode_ISCCCorrente_NULL, oTabellaParamCode_ISCCCorrente_Compliant, oTabellaParamCode_ISCCCorrente_NotCompliant,
                                            oTabellaParamCode_ISCCPrecedente_NULL, oTabellaParamCode_ISCCPrecedente_Compliant, oTabellaParamCode_ISCCPrecedente_NotCompliant,
                                            objParametriServer)

            Dim MatPrimeCampionatureFiltro As New StringBuilder
            MatPrimeCampionatureFiltro.Append(" ( ")
            MatPrimeCampionatureFiltro.AppendLine("    (Imprese.Compliance_ISCC = 1 AND camp1.Tipo_Cod != " & oTabellaParamCode_ISCCCorrente_Compliant & ") ")
            MatPrimeCampionatureFiltro.AppendLine("    OR (Imprese.Compliance_ISCC = 0 AND camp1.Tipo_Cod != " & oTabellaParamCode_ISCCCorrente_NotCompliant & ") ")
            MatPrimeCampionatureFiltro.AppendLine(" ) ")

            Dim objMatPrimeCampionatureR As New Materie_Prime_Campionature_R
            dtMatPrimeCamp = objMatPrimeCampionatureR.Leggi_MateriaPrima_Campionature_ISCC(paramQualTipoName_ISCCCurrVal, paramQualTipoName_ISCCPrecVal,
                                                                                           paramQualTipoName_ISCCCurrData, paramQualTipoName_ISCCPrecData,
                                                                                           MatPrimeCampionatureFiltro.ToString(), "", objParametriServer, dataRif)

            xRisp = dtMatPrimeCamp.Rows.Count

            If dtMatPrimeCamp.Rows.Count > 0 Then

                'Dim paramQualTipoName_ISCCCurrVal As String = "oiscccorrente"
                'Dim paramQualTipoName_ISCCPrecVal As String = "oisccprecedente"
                'Dim paramQualTipoName_ISCCCurrData As String = "oiscccorrentedate"
                'Dim paramQualTipoName_ISCCPrecData As String = "oisccprecedentedate"

                Dim Progressivo As Integer

                'Valori correnti a sistema per "oiscccorrente" e "oisccprecedente"
                Dim ISCCCorrente_TipoCod_old As Integer
                Dim ISCCPrecedente_TipoCod_old As Integer

                'Nuovi valori da impostare per per ciascun parametro
                Dim ISCCCorrente_TipoCod As Integer
                Dim ISCCPrecedente_TipoCod As Integer
                Dim ISCCCorrenteData_ValCod As String
                Dim ISCCPrecedenteData_ValCod As String

                Dim FornitoreImpresa_Compliance_ISCC As Boolean

                Dim matPrimeCamp_Descrizione As String
                Dim matPrimeCamp_PesoCampione As Integer
                Dim matPrimeCamp_ProgressivoOrigine As Integer
                Dim matPrimeCamp_PivaSuperuserOrigine As String
                Dim matPrimeCamp_ValiditaInizio As Date
                Dim matPrimeCamp_ValiditaFine As Date

                For Each row In dtMatPrimeCamp.Rows

                    Progressivo = row.Item("Progressivo")

                    FornitoreImpresa_Compliance_ISCC = row.Item("FornitoreImpresa_Compliance_ISCC")

                    ISCCCorrente_TipoCod_old = row.Item("ISCCCurr_TipoCod")
                    ISCCPrecedente_TipoCod_old = row.Item("ISCCPrec_TipoCod")

                    'NOTA: i valori (Tipo_Cod, numerici) possibili per i parametri relativi ad ISCC corrente ed ISCC precedente sono differenti sia fra loro che rispetto al campo Compliance_ISCC della tabella Imprese:
                    'va di conseguenza selezionato il valore corretto 

                    If FornitoreImpresa_Compliance_ISCC Then
                        ISCCCorrente_TipoCod = oTabellaParamCode_ISCCCorrente_Compliant
                    Else
                        ISCCCorrente_TipoCod = oTabellaParamCode_ISCCCorrente_NotCompliant
                    End If

                    If ISCCCorrente_TipoCod_old = oTabellaParamCode_ISCCCorrente_Compliant Then
                        ISCCPrecedente_TipoCod = oTabellaParamCode_ISCCPrecedente_Compliant
                    ElseIf ISCCCorrente_TipoCod_old = oTabellaParamCode_ISCCCorrente_NotCompliant Then
                        ISCCPrecedente_TipoCod = oTabellaParamCode_ISCCPrecedente_NotCompliant
                    ElseIf ISCCCorrente_TipoCod_old = oTabellaParamCode_ISCCCorrente_NULL Then
                        ISCCPrecedente_TipoCod = oTabellaParamCode_ISCCPrecedente_NULL
                    Else
                        ISCCPrecedente_TipoCod = 0
                    End If

                    ISCCPrecedenteData_ValCod = row.Item("ISCCCurrData_ValCod")
                    ISCCCorrenteData_ValCod = Date.Now.ToString("yyyy-MM-dd HH:mm:ss")

                    matPrimeCamp_Descrizione = row.Item("Descrizione")
                    matPrimeCamp_PesoCampione = row.Item("Peso_Campione")
                    matPrimeCamp_ProgressivoOrigine = row.Item("Progressivo_Origine")
                    matPrimeCamp_PivaSuperuserOrigine = row.Item("Piva_SuperUser_Origine")
                    matPrimeCamp_ValiditaInizio = row.Item("Validita_Inizio")
                    matPrimeCamp_ValiditaFine = row.Item("Validita_Fine")

                    Dim objMatPrimeCampionatureW As New Materie_Prime_Campion_W

                    'Non posso aggiornare, per i parametri per i parametri relativi ad ISCC corrente ed ISCC precedente, Tipo_Cod per i valori della tabella Materie_Prime_Campionature,
                    'occorre cancellare le righe e scrivere nuovi valori

                    objMatPrimeCampionatureW.Cancella(Progressivo, paramQualTipoName_ISCCCurrVal, ISCCCorrente_TipoCod_old, 0, False, "", objParametriServer)
                    objMatPrimeCampionatureW.Cancella(Progressivo, paramQualTipoName_ISCCPrecVal, ISCCPrecedente_TipoCod_old, 0, False, "", objParametriServer)

                    objMatPrimeCampionatureW.Scrivi(Progressivo, paramQualTipoName_ISCCCurrVal, ISCCCorrente_TipoCod, 0, 0,
                                                    matPrimeCamp_Descrizione, matPrimeCamp_PesoCampione, matPrimeCamp_ProgressivoOrigine, matPrimeCamp_PivaSuperuserOrigine,
                                                    matPrimeCamp_ValiditaInizio, matPrimeCamp_ValiditaFine,
                                                    objParametriServer)
                    objMatPrimeCampionatureW.Scrivi(Progressivo, paramQualTipoName_ISCCPrecVal, ISCCPrecedente_TipoCod, 0, 0,
                                                    matPrimeCamp_Descrizione, matPrimeCamp_PesoCampione, matPrimeCamp_ProgressivoOrigine, matPrimeCamp_PivaSuperuserOrigine,
                                                    matPrimeCamp_ValiditaInizio, matPrimeCamp_ValiditaFine,
                                                    objParametriServer)

                    'Per i parametri per i parametri relativi alle date di aggiornamento di ISCC corrente ed ISCC precedente, i valori di riferimento sono in Val_Cod, quindi si può fare l'update 

                    objMatPrimeCampionatureW.Modifica(Progressivo, paramQualTipoName_ISCCCurrData, 0, 0, objParametriServer, ISCCCorrenteData_ValCod)
                    objMatPrimeCampionatureW.Modifica(Progressivo, paramQualTipoName_ISCCPrecData, 0, 0, objParametriServer, ISCCPrecedenteData_ValCod)

                Next

            End If

            Utility.VerificaChiudiTransazione(objParametriServer, flagTransazione)

        Catch ex As Exception

            Utility.VerificaAnnullaTransazione(objParametriServer, flagTransazione)

            Scrivi_LOG(objParametriServer, nomeRoutine, ex.Message)

            dtMatPrimeCamp = Nothing
            xRisp = -1
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)

        Finally

            Utility.VerificaChiudiConnessione(objParametriServer, flagConnessione)

        End Try

        Return xRisp

    End Function

    Private Sub LeggiISCCParametriTipoCodValues(ByRef ISCCCorrente_NULL As Integer, ByRef ISCCCorrente_Compliant As Integer, ByRef ISCCCorrente_NotCompliant As Integer,
                                                ByRef ISCCPrecedente_NULL As Integer, ByRef ISCCPrecedente_Compliant As Integer, ByRef ISCCPrecedente_NotCompliant As Integer,
                                                ByRef objParametriServer As AgronicaCoreParametri)

        ISCCCorrente_NULL = 0
        ISCCCorrente_Compliant = 0
        ISCCCorrente_NotCompliant = 0
        ISCCPrecedente_NULL = 0
        ISCCPrecedente_Compliant = 0
        ISCCPrecedente_NotCompliant = 0

        Dim oTabelleObj As New OTabelle_R
        Dim params = oTabelleObj.LeggiJoinParametri(objParametriServer, "(Tabella_Cod_Des = 'ISCCcorrente' OR Tabella_Cod_Des = 'ISCCprecedente')", "").Rows

        Dim Tabella_Par_Cod As String
        Dim Tabella_Cod_Des As String
        Dim Sigla As String

        For Each param In params

            Tabella_Par_Cod = param.Item("Tabella_Par_Cod")
            Tabella_Cod_Des = param.Item("Tabella_Cod_Des")
            Sigla = param.Item("Sigla")

            If Tabella_Cod_Des = "ISCCcorrente" Then

                If Sigla = "" Then
                    ISCCCorrente_NULL = Tabella_Par_Cod
                ElseIf Sigla = "CO" Then
                    ISCCCorrente_Compliant = Tabella_Par_Cod
                ElseIf Sigla = "NC" Then
                    ISCCCorrente_NotCompliant = Tabella_Par_Cod
                End If

            ElseIf Tabella_Cod_Des = "ISCCprecedente" Then

                If Sigla = "" Then
                    ISCCPrecedente_NULL = Tabella_Par_Cod
                ElseIf Sigla = "CO" Then
                    ISCCPrecedente_Compliant = Tabella_Par_Cod
                ElseIf Sigla = "NC" Then
                    ISCCPrecedente_NotCompliant = Tabella_Par_Cod
                End If

            End If

        Next

        If ISCCCorrente_NULL = 0 Then
            Throw New Exception(String.Format(My.Resources.AgronicaCoreContabBIZ.ParametroQualitativoNoValore, "vuoto", "ISCCcorrente"))
        End If

        If ISCCCorrente_Compliant = 0 Then
            Throw New Exception(String.Format(My.Resources.AgronicaCoreContabBIZ.ParametroQualitativoNoValore, "Compilant", "ISCCcorrente"))
        End If

        If ISCCCorrente_NotCompliant = 0 Then
            Throw New Exception(String.Format(My.Resources.AgronicaCoreContabBIZ.ParametroQualitativoNoValore, "Not Compilant", "ISCCcorrente"))
        End If

        If ISCCPrecedente_NULL = 0 Then
            Throw New Exception(String.Format(My.Resources.AgronicaCoreContabBIZ.ParametroQualitativoNoValore, "vuoto", "ISCCprecedente"))
        End If

        If ISCCPrecedente_Compliant = 0 Then
            Throw New Exception(String.Format(My.Resources.AgronicaCoreContabBIZ.ParametroQualitativoNoValore, "Compilant", "ISCCprecedente"))
        End If

        If ISCCPrecedente_NotCompliant = 0 Then
            Throw New Exception(String.Format(My.Resources.AgronicaCoreContabBIZ.ParametroQualitativoNoValore, "Not Compilant", "ISCCprecedente"))
        End If

    End Sub

End Class
