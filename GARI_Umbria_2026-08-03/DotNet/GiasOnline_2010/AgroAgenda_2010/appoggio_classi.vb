'Public Class appoggio_classi
'  Private Function ControllaDose(ByRef Dose As Double, ByRef Dose_Tot As Double)

'        Txt_Dose_HL.Text = Txt_Dose_HL.Text.Replace(".", ",")
'        Txt_Dose_HA.Text = Txt_Dose_HA.Text.Replace(".", ",")




'        'controlli generici sulla dose
'        If Not IsNumeric(Me.Txt_Dose_HA.Text) Then
'            Messaggi.AgroMsgBox("Inserire un valore numerico per indicare la <b>Dose</b> di formulato.", Page, , updateDoseInserisci)
'            Return False
'        Else
'            If CDbl(Me.Txt_Dose_HA.Text) <= 0 Then
'                Messaggi.AgroMsgBox("Non e' possibile inserire una dose nulla o negativa.", Page, , updateDoseInserisci)
'                Return False
'            End If
'            Dose = Replace(Txt_Dose_HA.Text, ".", ",")
'        End If

'        'controlli generici sulla dose totale 











'        If Not IsNumeric(Txt_DoseTot_HA.Text) Then
'            Messaggi.AgroMsgBox("Inserire un valore numerico per indicare la <b>Dose Totale</b> di formulato.", Page, , updateDoseInserisci)
'            Return False
'        Else
'            If CDbl(Me.Txt_DoseTot_HA.Text) <= 0 Then
'                Messaggi.AgroMsgBox("Non e' possibile inserire una <b>Dose Totale</b> nulla o negativa.", Page, , updateDoseInserisci)
'                Return False
'            End If
'            Dose_Tot = Replace(Txt_DoseTot_HA.Text, ".", ",")
'        End If

'        'controllo sulla dose max

'        Dim DoseMax As Double = Session("DoseMax")
'        Dim Udm_Cod_Max As Integer = Session("Udm_Cod_Max")

'        Dim UDM_Radice, perHa_Hl As Integer






'        If Not Session("DoseMax") Is Nothing AndAlso DoseMax <> 0 Then

'            ScomponiUdm(UDM_Radice, perHa_Hl, Udm_Cod_Max)





'            If UDM_Radice <> -1 Then

'                Dim Moltiplicatore As Double
'                Dim doseIndicata As Double
'                Dim doseEtichetta As Double

'                'converto in kg o l
'                ConvertiToKG_L(UDM_Radice, Moltiplicatore)
'                doseEtichetta = DoseMax * Moltiplicatore

'                ConvertiToKG_L(Cmb_UdM.SelectedValue, Moltiplicatore)








'                Select Case perHa_Hl
'                    Case 2121
'                        'HL
'                        doseIndicata = Txt_Dose_HL.Text * Moltiplicatore
'                    Case 0
'                        Return True
'                    Case Else
'                        'HA
'                        doseIndicata = Txt_Dose_HA.Text * Moltiplicatore

'                End Select



'                If doseIndicata > doseEtichetta Then
'                    Messaggi.AgroMsgBox("ATTENZIONE!!! <br>La <b>Dose</b> selezionata non può superare quella <b>d'etichetta</b>", Page, , updateDoseInserisci)
'                    Return False
'                End If




'            End If

























'        End If


'        Return True

'    End Function


'End Class
