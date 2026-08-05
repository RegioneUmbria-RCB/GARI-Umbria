

Imports AgronicaCoreDataProvider
Imports AgronicaCoreModelsSTD.metaschema

Public Class InsulationMappingParameters

    ''' <summary>
    ''' Retrieves a list of all distinct Species Classes available on the server.
    ''' </summary>
    ''' <returns>
    ''' Returns a generic List of anonymous objects containing:
    ''' <list type="bullet">
    ''' <item><description>codice (Integer): The ID of the species.</description></item>
    ''' <item><description>descrizione (String): The description of the species class.</description></item>
    ''' </list>
    ''' </returns>
    ''' <remarks>
    ''' If the list is empty, hence the table is not valorized, the "Parametri Mappatura Isolamenti" section 
    ''' should not be enabled in the user settings page.
    ''' </remarks>
    Public Function GetSpeciesClassesList(objParametri_Server As AgronicaCoreParametri)
        Dim classReaderDAL As New AgronicaCoreSementieriDAL.ClassiDiSpecie_R
        Return classReaderDAL.distinct_sementieri_classidispecievegetali_des(objParametri_Server).
            AsEnumerable.Select(Function(x) New With {
                .codice = x.Field(Of Integer)("id_specie"),
                .descrizione = x.Field(Of String)("sementieri_classidispecievegetali_des")
            }).ToList
    End Function

    ''' <summary>
    ''' Retrieves the Species Classes associated with each user, according to the table Mappatura_Specie_x_Utente.
    ''' </summary>
    ''' <returns>
    ''' Returns a IEnumerable with the results grouped by user
    ''' </returns>
    Public Function GetUsersSpeciesClasses(objParametri_Utenti As AgronicaCoreParametri, objParametri_Server As AgronicaCoreParametri) As IEnumerable(Of ClassiSpeciePerUtente)
        Dim classReaderDAL As New AgronicaCoreSementieriDAL.ClassiDiSpecie_R
        Return classReaderDAL.GetClasseSpecieXUtente(objParametri_Utenti, objParametri_Server).AsEnumerable().
            GroupBy(Function(x) x.Field(Of String)("UserName")).
            Select(Function(x) New ClassiSpeciePerUtente With {
                .UserName = x.First.Field(Of String)("UserName"),
                .Nome = x.First.Field(Of String)("Nome"),
                .Cognome = x.First.Field(Of String)("Cognome"),
                .id_specie = x.Select(Function(row) row.Field(Of Integer)("id_specie")).ToList()
            })
    End Function

    Public Function SaveUserSpeciesClasses(user As ClassiSpeciePerUtente, objParametri_Utenti As AgronicaCoreParametri, objParametri_Server As AgronicaCoreParametri)
        Dim classReaderDAL As New AgronicaCoreSementieriDAL.ClassiDiSpecie_R
        Dim classWriterDAL As New AgronicaCoreSementieriDAL.ClassiDiSpecie_W
        Dim filtro = $" UserName = '{user.UserName}' "
        Dim species = classReaderDAL.GetClasseSpecieXUtente(objParametri_Utenti, objParametri_Server).
            Select(filtro).
            Select(Function(row) row.Field(Of Integer)("id_specie")).ToList()

        Dim removed = species.Except(user.id_specie).ToList()
        Dim added = user.id_specie.Except(species).ToList()
        If removed.Any Then
            classWriterDAL.RemoveClasseSpecieXUtente({user.UserName}, removed, objParametri_Server)
        End If
        If added.Any Then
            classWriterDAL.AddClasseSpecieXUtente(user.UserName, added, objParametri_Server)
        End If
    End Function

End Class
