Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreUtility

Public Class MailPDC

    Public Function TestaInvioMailPDC_LabSpecifico(ByVal Cod_Risum As Integer,
                                                   ByRef objParametri_Server As AgronicaCoreParametri,
                                                   ByRef msgError As String
                                                   ) As Boolean

        Dim risMail As Boolean = False

        'leggo i dati per l'invio
        Dim objLabOpzioni As New AgronicaCoreAnagrafeDAL.Laboratori_Opzioni_R
        Dim dtLab As DataTable = objLabOpzioni.Leggi(Cod_Risum, 0, objParametri_Server)

        Dim RispondiA As String = ""
        Dim MailA As String = ""
        Dim MailCC As String = ""

        For i As Integer = 0 To dtLab.Rows.Count - 1
            Select Case dtLab.Rows(i).Item("Tipo_Opzione")
                Case enum_Opzioni_Laboratori.Mail_Automatica_A
                    MailA = dtLab.Rows(i).Item("Valore")
                Case enum_Opzioni_Laboratori.Mail_Automatica_CC
                    MailCC = dtLab.Rows(i).Item("Valore")
                Case enum_Opzioni_Laboratori.Mail_Rispondi_A
                    RispondiA = dtLab.Rows(i).Item("Valore")
            End Select
        Next

        If RispondiA.Trim = "" Then
            Throw New Exception("RispondiA non Configurato")
        End If
        If MailA.Trim = "" Then
            Throw New Exception("MailA non Configurato")
        End If

        risMail = TestaInvioMailPDC(RispondiA, MailA, MailCC, objParametri_Server, msgError)

        Return risMail

    End Function

    Public Function TestaInvioMailPDC(ByVal mittente As String,
                                      ByVal destinatario As String,
                                      ByVal mailCC As String,
                                      ByRef objParametri_Server As AgronicaCoreParametri,
                                      ByRef msgError As String
                                      ) As Boolean

        Dim risMail As Boolean = False


        If String.IsNullOrEmpty(Trim(mittente)) Then
            Throw New Exception("Mittente non Configurato")
        End If

        If String.IsNullOrEmpty(Trim(destinatario)) Then
            Throw New Exception("Destinatario non Configurato")
        End If


        Dim Oggetto As String = "Test mail PDC - Analisi GIAS"
        Dim TestoMail As String = "Test Mail"
        Dim Firma As String = "Firma test"

        'aggiungo la firma 
        TestoMail &= "<br><br>" & Firma


        Dim all As New AgronicaCoreGestioneRichieste.Esporta
        Dim pathFileXls As String = all.CreaAllegatoFittizioPDC()

        Dim allegati As String() = Nothing

        If Not String.IsNullOrEmpty(pathFileXls) Then
            allegati = {pathFileXls}
        End If

        Dim handleMail As New Mail
        msgError = handleMail.invia(objParametri_Server, mittente, destinatario,
                                    mailCC, "", Oggetto, TestoMail, True, allegati)

        If msgError = "" Then
            risMail = True
            msgError = "Mail inviata correttamente"
        Else
            risMail = False
        End If

        My.Computer.FileSystem.DeleteFile(pathFileXls, FileIO.UIOption.AllDialogs, FileIO.RecycleOption.SendToRecycleBin)

        Return risMail

    End Function

End Class
