
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class helperUI


    Public Shared Function DescrizioneOperazioniNonutilizzo(ByVal Dichiazioni_1_Operazioni_2 As Int16, ByVal elem_cod As Integer, ByVal d1 As Date, ByVal d2 As Date) As String
        Dim p1 As String = ""
        Dim p2 As String = ""

        If elem_cod = 3 Then
            p2 = "(Fertilizzazioni)"
        Else
            p2 = "(Trattamenti)"
        End If

        If Dichiazioni_1_Operazioni_2 = 1 Then
            p1 = "Ci sono delle dichiarazioni di non utilizzo " & p2
        Else
            p1 = "Ci sono delle operazioni di agenda " & p2
        End If

        Return p1 & " registrate per questa impresa nel periodo dal " & d1 & " al " & d2 & "."

    End Function



#Region "Verifiche su dichiarazioni non utilizzo"

    Public Shared Sub VerificaNonUtilizzo(ByVal Dichiazioni_1_Operazioni_2 As Int16, ByVal lav_cod As Integer, ByVal piva As String, ByRef lErroriSegnalazioni As Label, ByVal data As Date, ByVal objParametri_Server As AgronicaCoreParametri, ByVal objParametri_Utenti As AgronicaCoreParametri)

        Dim elem_cod As Integer = 3

        Dim opL As New AgronicaCoreMetaSchemaDAL.Operazioni_R
        Dim ogru As String = _
        opL.Gru_Op_from_LavorazioneCod( _
            lav_cod, _
            objParametri_Server _
        )

        If ogru = "3" Then
            elem_cod = 123
        End If


        Dim D1 As Date = data
        Dim D2 As Date = data

        Dim brval As Boolean = _
            verificaOperazioni(Dichiazioni_1_Operazioni_2, lav_cod, elem_cod, piva, D1, D2, objParametri_Server, objParametri_Utenti)

        If Not brval Then
            lErroriSegnalazioni.Text = DescrizioneOperazioniNonutilizzo(Dichiazioni_1_Operazioni_2, elem_cod, D1, D2)
        Else
            lErroriSegnalazioni.Text = ""
        End If

    End Sub

    
    Private Shared Function verificaOperazioni(ByVal Dichiazioni_1_Operazioni_2 As Int16, ByVal lav_Cod As Integer, ByVal elem_cod As Integer, ByVal piva As String, ByRef d1 As Date, ByRef d2 As Date, ByVal objParametri_server As AgronicaCoreParametri, ByVal objParametri_utenti As AgronicaCoreParametri) As Boolean

        Dim xVerifica As New AgronicaCoreContabDAL.Contabilita_R
        Dim xLetturaAnnataAgraria As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read




        xLetturaAnnataAgraria.AnnataAgraria(d1, d1, d2, objParametri_utenti)

        Dim xRval As Boolean = False
        Dim xDtRval As DataTable

        If Dichiazioni_1_Operazioni_2 = 2 Then
            xDtRval = _
            xVerifica.DocumentiContabili_NonUtilizzo_xVerifica( _
               piva, _
               d1, _
               d2, _
               lav_Cod, _
               objParametri_server _
           )
        Else



            xDtRval = xVerifica.DocumentiContabili_NonUtilizzo_EsportazioneSIGPA( _
                        piva, _
                        0, _
                        "", _
                        d1, _
                        d2, _
                        elem_cod, _
                        objParametri_server _
                    )
        End If


        If xDtRval.Rows.Count = 0 Then
            xRval = True
        End If
        Return xRval
    End Function
#End Region


End Class
