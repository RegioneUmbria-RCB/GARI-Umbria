Imports System.Text

Public Class Audit

    'Private _LogErrori As StringBuilder
    'Private _LogRiepilogo As StringBuilder
    'Private _LogImport As StringBuilder
    'Private _objOpzioni As Gias2Gias_LIB.clsOpzioni

    Public Sub ImportaAudit(ByRef Log_Import As StringBuilder,
                            ByRef Log_Errori As StringBuilder,
                            ByRef Log_Riepilogo As StringBuilder,
                            ByVal PivaOrigine As String,
                            ByVal PivaDestinazione As String,
                            ByVal objOpzioni As Gias2Gias_LIB.clsOpzioni)

        '_LogErrori = Log_Errori
        '_LogImport = Log_Import
        '_LogRiepilogo = Log_Riepilogo
        '_objOpzioni = objOpzioni

        Dim ValiditaInizio As DateTime = #1/1/1900#
        Dim ValiditaFine As DateTime = #12/31/2100#

        Try

            'di default imposto le date dell'intervallo coincidenti con l'annata agraria 
            'dal 1/11 al 31/10

            'Mese = Now.Month

            'Select Case Mese
            '    Case 11, 12
            '        'se sono nei mesi di novembre o dicembre..........
            '        'l'annata agraria va dal 1/11 di quest'anno al 31/10 del prossimo
            '        ValiditaInizio = CDate("01/11/" & Now.Year)

            '    Case Else
            '        'l'annata agraria va dal 1/11 dell'anno scorso al 31/10 di quest'anno
            '        ValiditaInizio = CDate("01/11/" & Now.Year - 1)

            'End Select

            'ValiditaInizio = Now.Today


            Dim IntervisteDecode As New List(Of Intervistekey)
            Dim AuditDecode As New List(Of AuditKey)


            '*************************************************************************************************************
            '***  RECORD AUDIT         ******************************************************************************
            '*************************************************************************************************************

            'lettura 
            Dim letturaAudit As New AgronicaCoreAuditDAL.Audit_R
            Dim audits As DataTable = letturaAudit.Leggi(0, 0, 0,
                                                         PivaOrigine,
                                                         ValiditaInizio, ValiditaFine,
                                                         "", "",
                                                         objOpzioni.objParametri_Server_GIAS_ORIGINE)

            'scrittura
            Dim ObjSequenze As New AgronicaCoreDataProvider.Agro_Sequenze

            Dim audit_Cod As Integer


            Dim auditScrivi As New AgronicaCoreAuditDAL.Audit_W
            'Dim auditLogScrivi As New AgronicaCoreAuditDAL.Audit_log_W
            For Each audit As DataRow In audits.Rows

                audit_Cod = ObjSequenze.NuovoId_Tabella("AUDIT",
                                                        objOpzioni.BaseCode_DESTINAZIONE,
                                                        objOpzioni.TopCode_DESTINAZIONE,
                                                        objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)


                'scrivi
                AuditDecode.Add( _
                    New AuditKey With { _
                        .oldkey_audit_cod = audit("audit_cod"), _
                        .oldkey_regolamento = audit("Regolamento_cod"), _
                        .oldkey_superuser = audit("Audit_SuperUser"), _
                        .oldkey_tipo = audit("Audit_Tipo"), _
                        .newkey_adit_cod = audit_Cod, _
                        .newkey_regolamento = audit("Regolamento_cod"), _
                        .newkey_superuser = audit("Audit_SuperUser"), _
                        .newkey_tipo = audit("Audit_Tipo") _
                })

                auditScrivi.scrivi(
                    audit_Cod,
                    audit("Audit_tipo"),
                    dbUtils.DBNullToNothing(audit("Audit_responsabile")),
                    dbUtils.DBNullToNothing(audit("Audit_stato")),
                    audit("Regolamento_cod"),
                    PivaDestinazione,
                    dbUtils.DBNullToNothing(audit("Note")),
                    audit("Campionato"),
                    audit("Validita_inizio"),
                    audit("Validita_fine"),
                    audit("Data_Creazione"),
                    audit("Data_Modifica"),
                    audit("Username_Creazione"),
                    audit("Username_Modifica"),
                    objOpzioni.objParametri_Server_GIAS_DESTINAZIONE,
                    audit("Rintracciabilita"),)



                '*************************************************************************************************************
                '***  RECORD AUDIT RISPOSTA         **************************************************************************
                '*************************************************************************************************************

                'lettura
                Dim letturaAuditRisposta As New AgronicaCoreAuditDAL.Audit_Risposte_R
                Dim auditRisposte As DataTable = letturaAuditRisposta.Leggi( _
                    audit("Audit_cod"), _
                    audit("Audit_tipo"), _
                    audit("Regolamento_Cod"), _
                    0, _
                    ValiditaInizio, _
                    ValiditaFine, _
                    "", _
                    "", _
                    objOpzioni.objParametri_Server_GIAS_ORIGINE _
                )

                Dim scriviAuditRisposta As New AgronicaCoreAuditDAL.Audit_Risposte_W
                Dim auditOldKey As New AuditKey
                Dim auditNewKey As Integer

                For Each auditRisposta As DataRow In auditRisposte.Rows

                    auditOldKey.oldkey_audit_cod = auditRisposta("Audit_cod")
                    auditOldKey.oldkey_regolamento = auditRisposta("regolamento_cod")
                    auditOldKey.oldkey_superuser = auditRisposta("audit_superUser")
                    auditOldKey.oldkey_tipo = auditRisposta("audit_tipo")

                    auditNewKey = ( _
                        From i In AuditDecode _
                        Where i.oldkey_audit_cod = auditOldKey.oldkey_audit_cod _
                        AndAlso i.oldkey_regolamento = auditOldKey.oldkey_regolamento _
                        AndAlso i.oldkey_superuser = auditOldKey.oldkey_superuser _
                        AndAlso i.oldkey_tipo = auditOldKey.oldkey_tipo _
                        Select i.newkey_adit_cod).FirstOrDefault


                    scriviAuditRisposta.scrivi( _
                        auditNewKey, _
                        auditRisposta("Audit_Tipo"), _
                        auditRisposta("Punto_Numero"), _
                        auditRisposta("Regolamento_cod"), _
                        auditRisposta("Disp_cod"), _
                        dbUtils.DBNullToNothing(auditRisposta("PropostaCorrettiva")), _
                        dbUtils.DBNullToNothing(auditRisposta("Valore")), _
                        auditRisposta("Validita_inizio"), _
                        auditRisposta("Validita_fine"), _
                        auditRisposta("Data_Creazione"), _
                        auditRisposta("Data_Modifica"), _
                        auditRisposta("Username_Creazione"), _
                        auditRisposta("Username_Modifica"), _
                        objOpzioni.objParametri_Server_GIAS_DESTINAZIONE _
                    )


                Next

            Next

            'loggo
            'For Each l As AuditKey In AuditDecode
            '    auditLogScrivi.scrivi( _
            '        l.oldkey_audit_cod, _
            '        l.oldkey_tipo, _
            '        l.oldkey_regolamento, _
            '        l.newkey_adit_cod, _
            '        l.newkey_tipo, _
            '        l.newkey_regolamento, _
            '        l.oldkey_superuser, _
            '        l.newkey_superuser, _
            '        objOpzioni.objParametri_Server_GIAS_DESTINAZIONE
            '    )
            'Next



            '*************************************************************************************************************
            '***  RECORD AUDIT INTERVISTA         ******************************************************************************
            '*************************************************************************************************************

            'lettura
            Dim objLetturaInterviste As New AgronicaCoreAuditDAL.Audit_Interviste_R
            Dim interviste As DataTable = objLetturaInterviste.Leggi( _
                0, _
                0, _
                0, _
                PivaOrigine, _
                ValiditaInizio, _
                ValiditaFine, _
                "", _
                "", _
                objOpzioni.objParametri_Server_GIAS_ORIGINE _
                )

            'scrittura
            Dim objAudit_Interviste As New AgronicaCoreAuditDAL.Audit_Interviste_W
            Dim cod_intervista As Integer

            For Each intervista In interviste.Rows


                cod_intervista = ObjSequenze.NuovoId_Tabella( _
                                       "AUDIT_INTERVISTE", _
                                        objOpzioni.BaseCode_DESTINAZIONE, _
                                        objOpzioni.TopCode_DESTINAZIONE, _
                                        objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)


                objAudit_Interviste.Scrivi( _
                    intervista("Audit_tipo"), _
                    intervista("Regolamento_Cod"), _
                    cod_intervista, _
                    PivaDestinazione, _
                    intervista("Validita_Inizio"), _
                    intervista("Validita_Fine"), _
                    objOpzioni.objParametri_Server_GIAS_DESTINAZIONE, _
                    intervista("Data_creazione"), _
                    intervista("Data_Modifica"), _
                    intervista("Username_Creazione"), _
                    intervista("Username_Modifica") _
                )

                IntervisteDecode.Add( _
                New Intervistekey With { _
                        .oldkey_intervista_cod = intervista("intervista_cod"), _
                        .oldkey_regolamento = intervista("Regolamento_cod"), _
                        .oldkey_superuser = intervista("Intervista_SuperUser"), _
                        .oldkey_tipo = intervista("Audit_Tipo"), _
                        .newkey_intervista_cod = cod_intervista, _
                        .newkey_regolamento = intervista("Regolamento_cod"), _
                        .newkey_superuser = intervista("Intervista_SuperUser"), _
                        .newkey_tipo = intervista("Audit_Tipo") _
                })


                '*************************************************************************************************************
                '***  RECORD RISPOSTE INTERVISTA         *********************************************************************
                '*************************************************************************************************************
                Dim Dt_Domande_Interviste As DataTable

                Dim objAudit_Domande_Interviste As New AgronicaCoreAuditDAL.Audit_Risposte_Interviste_R

                ' lettura
                Dt_Domande_Interviste = objAudit_Domande_Interviste.Leggi( _
                    intervista("Audit_Tipo"), _
                    intervista("Regolamento_cod"), _
                    intervista("intervista_cod"), _
                    0, _
                    "", _
                    "", _
                    objOpzioni.objParametri_Server_GIAS_ORIGINE)

                Dim objAuditRisposteInterviste As New AgronicaCoreAuditDAL.Audit_Risposte_Interviste_W
                Dim IntervistaoldKey As New Intervistekey
                Dim Intervistanewkey As Integer


                For Each domandaIntervista In Dt_Domande_Interviste.Rows

                    IntervistaoldKey.oldkey_intervista_cod = domandaIntervista("Intervista_cod")
                    IntervistaoldKey.oldkey_regolamento = domandaIntervista("regolamento_cod")
                    IntervistaoldKey.oldkey_superuser = domandaIntervista("intervista_superUser")
                    IntervistaoldKey.oldkey_tipo = domandaIntervista("audit_tipo")

                    Intervistanewkey = ( _
                        From i In IntervisteDecode _
                        Where i.oldkey_intervista_cod = IntervistaoldKey.oldkey_intervista_cod _
                        AndAlso i.oldkey_regolamento = IntervistaoldKey.oldkey_regolamento _
                        AndAlso i.oldkey_superuser = IntervistaoldKey.oldkey_superuser _
                        AndAlso i.oldkey_tipo = IntervistaoldKey.oldkey_tipo _
                        Select i.newkey_intervista_cod).FirstOrDefault

                    objAuditRisposteInterviste.Scrivi( _
                        domandaIntervista("Audit_Tipo"), _
                        domandaIntervista("Regolamento_Cod"), _
                        Intervistanewkey, _
                        domandaIntervista("Domanda_cod"), _
                        dbUtils.DBNullToNothing(domandaIntervista("valore")), _
                        domandaIntervista("Validita_Inizio"), _
                        domandaIntervista("Validita_Fine"), _
                        objOpzioni.objParametri_Server_GIAS_DESTINAZIONE, _
                        domandaIntervista("Data_creazione"), _
                        domandaIntervista("Data_Modifica"), _
                        domandaIntervista("Username_Creazione"), _
                        domandaIntervista("Username_Modifica") _
                    )

                Next
            Next


        Catch ex As Exception
            Log_Import.Append("AUDIT: Impresa: " & PivaOrigine & "Si è verificato un errore durante il salvataggio del profilo di condizionalità.")
            Throw ex
        End Try

    End Sub


End Class


Friend Class Intervistekey
    Property oldkey_intervista_cod As Integer
    Property oldkey_superuser As String
    Property oldkey_tipo As Integer
    Property oldkey_regolamento As Integer

    Property newkey_intervista_cod As Integer
    Property newkey_superuser As String
    Property newkey_tipo As Integer
    Property newkey_regolamento As Integer


End Class

Friend Class AuditKey

    Property oldkey_audit_cod As Integer
    Property oldkey_superuser As String
    Property oldkey_tipo As Integer
    Property oldkey_regolamento As Integer

    Property newkey_adit_cod As Integer
    Property newkey_superuser As String
    Property newkey_tipo As Integer
    Property newkey_regolamento As Integer

End Class
