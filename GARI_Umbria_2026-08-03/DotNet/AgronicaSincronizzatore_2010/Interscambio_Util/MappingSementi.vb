Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class MappingSementi
    Public Property SottoCategoria As String
    Public Property CatAltro As String
    Public Property CatGias As Integer?
    Public Property UdmAltro As enum_UnitaMisura?
    Public Property UdmGias As enum_UnitaMisura?
End Class

Public Class ListMappingSementi

    Public Property Lista As List(Of MappingSementi)

    Public Sub New(ByRef objSettings As ParametriExtra)
        'TODO: DEVO ELIMINARE LA UDM ALTRO FISSA, PERCHè COSì VALE SOLO PER ABOCA, PER GLI ALTRI LA DEVO LEGGERE/PASSARE SEMPRE COME VARIABILE!!!

        Lista = New List(Of MappingSementi) From {
            New MappingSementi With {
                .SottoCategoria = Util.ID_SOTTOCAT_SEMENTI,
                .CatAltro = objSettings.SottoCat_Sementi, .CatGias = SEMENTI,
                .UdmAltro = enum_UnitaMisura.KG, .UdmGias = enum_UnitaMisura.KG
            },
            New MappingSementi With {
                .SottoCategoria = Util.ID_SOTTOCAT_SEMENTI,
                .CatAltro = objSettings.SottoCat_Sementi, .CatGias = SEMENTI,
                .UdmAltro = enum_UnitaMisura.Numero, .UdmGias = enum_UnitaMisura.Unita_Seme
            },
            New MappingSementi With {
                .SottoCategoria = Util.ID_SOTTOCAT_PIANTINE,
                .CatAltro = objSettings.SottoCat_Piantine, .CatGias = SEMENTI,
                .UdmAltro = enum_UnitaMisura.Numero, .UdmGias = enum_UnitaMisura.Num_Piante
            },
            New MappingSementi With {
                .SottoCategoria = Util.ID_SOTTOCAT_PIANTINE,
                .CatAltro = objSettings.SottoCat_Piantine, .CatGias = SEMENTI,
                .UdmAltro = enum_UnitaMisura.KG, .UdmGias = enum_UnitaMisura.KG
            }
        }
    End Sub

    Public Function GetUdmGias(ByRef objSettings As ParametriExtra,
                               ByRef sottoCategoria As String,
                               ByVal udmAltro As enum_UnitaMisura,
                               Optional ByVal catAltro As String = Nothing
                               ) As Integer

        Const nomeRoutine = "MappingSementi.GetUdmGias"
        Try
            Dim element As MappingSementi
            If sottoCategoria IsNot Nothing AndAlso sottoCategoria <> "" Then
                Dim sottoCatLoc As String = sottoCategoria
                element = Lista.FirstOrDefault(Function(x) x.UdmAltro = udmAltro AndAlso x.SottoCategoria = sottoCatLoc)
            ElseIf catAltro IsNot Nothing Then
                element = Lista.FirstOrDefault(Function(x) x.UdmAltro = udmAltro AndAlso x.CatAltro = catAltro)
            Else
                Throw New Exception("Necessario parametro sottoCategoria o catAltro")
            End If

            If element Is Nothing Then
                Throw New Exception(String.Format("L'udm {0} [{1}] non è mappata su GIAS in associazione con la categoria [{2}]",
                                                  objSettings.SuffissoAltroGestionale, udmAltro.ToString, If(catAltro, sottoCategoria)))
            End If

            sottoCategoria = element.SottoCategoria
            Return CInt(element.UdmGias)

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try
    End Function

    Public Function GetUdmAltro(ByRef objSettings As ParametriExtra,
                                ByRef sottoCategoria As String,
                                ByVal udmGias As enum_UnitaMisura,
                                ByVal catGias As Integer
                                ) As enum_UnitaMisura

        Const nomeRoutine = "MappingSementi.GetUdmAltro"
        Try
            Dim element As MappingSementi = Lista.FirstOrDefault(Function(x) x.CatGias = catGias AndAlso x.UdmGias = udmGias)
            If element Is Nothing Then
                Throw New Exception(String.Format("L'udm GIAS [{0}] non è mappata su {1} in associazione con la categoria [{2}]",
                                                  udmGias, objSettings.SuffissoAltroGestionale, catGias))
            End If
            sottoCategoria = element.SottoCategoria
            Return element.UdmAltro
        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try
    End Function

    Public Function GetCatGias(ByRef objSettings As ParametriExtra,
                               ByRef sottoCategoria As String,
                               ByVal catAltro As String,
                               ByVal udmAltro As String
                               ) As Integer

        Const nomeRoutine = "MappingSementi.GetCatGias"
        Try
            Dim element As MappingSementi 
            If udmAltro IsNot Nothing Then
                element = Lista.FirstOrDefault(Function(x) x.CatAltro = catAltro AndAlso x.UdmAltro = udmAltro)
            Else
                element = Lista.FirstOrDefault(Function(x) x.CatAltro = catAltro)
            End If

            If element Is Nothing Then
                Throw New Exception(String.Format("La categoria {0} [{1}] non è mappata su GIAS in associazione con l'udm [{2}]",
                                                  objSettings.SuffissoAltroGestionale, catAltro, If(udmAltro, "")))
            End If
            sottoCategoria = element.SottoCategoria
            Return element.CatGias
        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try
    End Function

    Public Function GetCatAltro(ByRef objSettings As ParametriExtra,
                                ByRef sottoCategoria As String,
                                ByVal catGias As Integer,
                                ByVal udmGias As enum_UnitaMisura
                                ) As String

        Const nomeRoutine = "MappingSementi.GetCatAltro"
        Try
            Dim element As MappingSementi = Lista.FirstOrDefault(Function(x) x.CatGias = catGias AndAlso x.UdmGias = udmGias)
            If element Is Nothing Then
                Throw New Exception(String.Format("La categoria GIAS [{0}] non è mappata su {1} in associazione con l'udm [{2}]",
                                                  catGias, objSettings.SuffissoAltroGestionale, udmGias))
            End If
            sottoCategoria = element.SottoCategoria
            Return element.CatAltro
        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try
    End Function
End Class
