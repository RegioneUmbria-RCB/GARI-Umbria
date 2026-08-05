Imports AgronicaCoreDataProvider

Public Class SecurityHelper

    ''' <summary>
    ''' This function reads an encrypted field from the database and eventually decrypts it
    ''' This function makes the assumption that the field storing the information whether or not the field is encrypted has the same name as the field but ends with '_isEncrypted'
    ''' </summary>
    ''' <param name="encryptedFieldKey"></param>
    ''' <param name="objParametri_Server"></param>
    ''' <returns></returns>
    Public Function ReadEncryptedFieldFromDb(encryptedFieldKey As String, objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim configurazioneSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim flagIsEncryptedKey = $"{encryptedFieldKey}_isEncrypted"

        Dim filtroAggiuntivo = $"Chiave IN ('{encryptedFieldKey}', '{flagIsEncryptedKey}', 'cr')"
        Dim dt As DataTable = configurazioneSiti.LeggiNonCacheable("", filtroAggiuntivo, objParametri_Server)

        Dim keysAndValues = (From r In dt.AsEnumerable
                             Select New With {
                            .Key = CStr(r.Item("Chiave")),
                            .Value = CStr(r.Item("Valore"))
                            }).ToList()

        Dim encryptedFieldKeyAndValue = keysAndValues.FirstOrDefault(Function(c) c.Key = encryptedFieldKey)
        Dim flagIsEncryptedKeyAndValue = keysAndValues.FirstOrDefault(Function(c) c.Key = flagIsEncryptedKey)
        Dim crKeyAndValue = keysAndValues.FirstOrDefault(Function(c) c.Key = "cr")

        Dim password As String = ""

        If Not IsNothing(flagIsEncryptedKeyAndValue) AndAlso Not String.IsNullOrEmpty(flagIsEncryptedKeyAndValue.Value) Then

            If Not IsNothing(encryptedFieldKeyAndValue) AndAlso Not String.IsNullOrEmpty(encryptedFieldKeyAndValue.Value) Then

                If flagIsEncryptedKeyAndValue.Value = "0" Then

                    password = encryptedFieldKeyAndValue.Value
                    If (Not IsNothing(crKeyAndValue)) AndAlso Not String.IsNullOrEmpty(crKeyAndValue.Value) Then
                        Dim sicurezza As New Sicurezza
                        Dim encryptedField = sicurezza.EncryptString(password, crKeyAndValue.Value)
                        Dim configurazioneSitiW As New AgronicaCoreVarieDAL.Configurazione_Siti_W
                        Dim chiavi As String() = {encryptedFieldKey, flagIsEncryptedKey}
                        Dim valori As String() = {encryptedField, "1"}
                        configurazioneSitiW.AggiornaValori(chiavi, valori, objParametri_Server)
                    End If

                ElseIf flagIsEncryptedKeyAndValue.Value = "1" Then

                    If Not IsNothing(crKeyAndValue) AndAlso Not String.IsNullOrEmpty(crKeyAndValue.Value) Then
                        Dim sicurezza As New Sicurezza
                        password = sicurezza.DecryptString(encryptedFieldKeyAndValue.Value, crKeyAndValue.Value)
                    End If

                Else
                    Throw New NotImplementedException("Valore della chiave _isEncrypted non gestita")
                End If

            End If

        End If

        Return password

    End Function

End Class
