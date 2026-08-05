Imports System.Runtime.CompilerServices

Public Module ObjectExtension

    <Extension()>
    Public Function DeepCloneObject(Of T)(ByVal source As T) As T
        Const nomeRoutine = "ObjectExtension.DeepCloneObject"

        Try
            Dim obj As New Runtime.Serialization.DataContractSerializer(GetType(T))

            Using stream As New IO.MemoryStream()
                obj.WriteObject(stream, source)
                stream.Seek(0, IO.SeekOrigin.Begin)
                Return CType(obj.ReadObject(stream), T)
            End Using

        Catch ex As Exception
            Throw New Exception(nomeRoutine & ": " & ex.Message)
        End Try
    End Function

    <Extension()>
    Public Function PropertyCopier(Of TParent, TChild)(parent As TParent, child As TChild,
                                                       Optional key_properties_to_exclude As String() = Nothing) As TChild

        'Questo metodo è in grado di copiare anche le proprietà a loro volta oggetto (ex. Appezzamento.Reg_Impianti)
        'NB: IMPORTANTE CHE key_properties_to_exclude SIANO IN MINUSCOLO! IL CONFRONTO E' CASE SENSITIVE!!!!

        Dim parentProperties = parent.[GetType]().GetProperties()
        Dim childProperties = child.[GetType]().GetProperties()

        For Each parentProperty In parentProperties

            If key_properties_to_exclude IsNot Nothing AndAlso
                key_properties_to_exclude.Contains(parentProperty.Name.ToLower()) Then
                Continue For
            End If

            For Each childProperty In childProperties

                If key_properties_to_exclude IsNot Nothing AndAlso
                key_properties_to_exclude.Contains(childProperty.Name.ToLower()) Then
                    Continue For
                End If

                If parentProperty.Name.ToLower() = childProperty.Name.ToLower() AndAlso parentProperty.PropertyType = childProperty.PropertyType Then
                    childProperty.SetValue(child, parentProperty.GetValue(parent))
                    Exit For
                End If

            Next
        Next

        Return child
    End Function

    <Extension()>
    Public Function FieldToPropertyCopier(Of TParent, TChild)(parent As TParent, child As TChild) As TChild

        'Questo metodo è in grado di copiare anche le proprietà a loro volta oggetto (ex. Appezzamento.Reg_Impianti)

        Dim parentFields = parent.GetType.GetFields()

        'Dim parentProperties = parent.[GetType]().GetProperties()
        Dim childProperties = child.[GetType]().GetProperties()

        For Each parentField In parentFields
            For Each childProperty In childProperties

                If parentField.Name.ToLower() = childProperty.Name.ToLower() AndAlso parentField.FieldType = childProperty.PropertyType Then
                    childProperty.SetValue(child, parentField.GetValue(parent))
                    Exit For
                End If

            Next
        Next

        Return child
    End Function

    <Extension()>
    Public Sub RiempiCampi_StandardGias(Of POCO)(objPOCO As POCO, username As String,
                                                 Optional edit_creazione As Boolean = True)

        'IMPORTANTE LEGGERE
        'Reflection on .NET by default is case sensitive for the class member.
        'To make it case insensitive you need to pass BindingFlags.IgnoreCase .

        'PROBLEM
        'I’ve passed the ignorecase flag and now it doesn’t return anything!!!

        'SOLUTION
        'Basically, If you pass one flag then the other flags will be overwritten by default which means all the default flags are disappeared.
        'So to make it case insensitive then you need to pass other binding flags

        'EXAMPLE
        'Dim pi As PropertyInfo = Me.[GetType]().GetProperty(fieldname, BindingFlags.IgnoreCase Or BindingFlags.[Public] Or BindingFlags.Instance)

        If edit_creazione Then
            Dim p_Data_Creazione = objPOCO.[GetType]().GetProperty("Data_Creazione", Reflection.BindingFlags.IgnoreCase Or Reflection.BindingFlags.[Public] Or Reflection.BindingFlags.Instance)
            p_Data_Creazione.SetValue(objPOCO, DateTime.Now)

            Dim p_Username_Creazione = objPOCO.[GetType]().GetProperty("Username_Creazione", Reflection.BindingFlags.IgnoreCase Or Reflection.BindingFlags.[Public] Or Reflection.BindingFlags.Instance)
            p_Username_Creazione.SetValue(objPOCO, username)
        End If

        Dim p_Data_Modifica = objPOCO.[GetType]().GetProperty("Data_Modifica", Reflection.BindingFlags.IgnoreCase Or Reflection.BindingFlags.[Public] Or Reflection.BindingFlags.Instance)
        p_Data_Modifica.SetValue(objPOCO, DateTime.Now)

        Dim p_Username_Modifica = objPOCO.[GetType]().GetProperty("Username_Modifica", Reflection.BindingFlags.IgnoreCase Or Reflection.BindingFlags.[Public] Or Reflection.BindingFlags.Instance)
        p_Username_Modifica.SetValue(objPOCO, username)

        'Provo a riempire inviato con int16, poi int32
        Dim p_Inviato = objPOCO.[GetType]().GetProperty("Inviato", Reflection.BindingFlags.IgnoreCase Or Reflection.BindingFlags.[Public] Or Reflection.BindingFlags.Instance)
        Try
            p_Inviato.SetValue(objPOCO, New Int16)
            Exit Sub
        Catch ex As Exception
        End Try
        Try
            p_Inviato.SetValue(objPOCO, New Int32)
        Catch ex As Exception
        End Try
    End Sub
End Module
