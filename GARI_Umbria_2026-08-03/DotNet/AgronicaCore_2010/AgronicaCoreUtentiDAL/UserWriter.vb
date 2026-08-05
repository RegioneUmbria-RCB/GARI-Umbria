Imports System.Text
Imports System.Text.RegularExpressions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModelsSTD.profilazione
Imports Microsoft.VisualBasic.ApplicationServices

Public Interface IUserWriter
    Function CreateFromSSOData(userID As String, nome As String, cognome As String, email As String, profilo As Integer, params As ObjParams, Optional passwordHashEnabled As Boolean = False) As Boolean
    Function CreateFromEmailNameSurname(email As String, nome As String, cognome As String, profilo As Integer, params As ObjParams, Optional passwordHashEnabled As Boolean = False) As Boolean

    ' Add more functions in the future :)
End Interface

Public Class UserWriter
    Inherits AgronicaCoreDataProvider.DataProvider
    Implements IUserWriter

    Private Const USER_TYPE_BUSINESS = 1
    Private Const USER_TYPE_PERSON = 2

    Public Overridable Function CreateFromEmailNameSurname(
        email As String,
        nome As String,
        cognome As String,
        profilo As Integer,
        params As ObjParams,
        Optional passwordHashEnabled As Boolean = False
    ) As Boolean Implements IUserWriter.CreateFromEmailNameSurname
        Dim userCreated As Boolean = False
        If IsNothing(params.ObjParametri_Utenti) OrElse IsNothing(params.ObjParametri_Server) Then
            Throw New InvalidOperationException()
        End If
        Try
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, params.ObjParametri_Utenti)
            Dim username = email
            Dim password = generatePassword(passwordHashEnabled)
            Dim cf = GetNextSequentialCF(params.ObjParametri_Server)
            Dim groups As IEnumerable(Of Integer) = New List(Of Integer)
            userCreated = CreateNewUser(
                username, nome, cognome, password, email,
                cf, String.Empty, String.Empty, profilo,
                groups, params.ObjParametri_Utenti, passwordHashEnabled
            )
            If userCreated Then
                AssignFullVisibility(username, params.ObjParametri_Utenti)
            End If
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(1, params.ObjParametri_Utenti)
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(params.ObjParametri_Utenti)
        Catch ex As Exception
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, params.ObjParametri_Utenti)
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(params.ObjParametri_Utenti)
            Throw
        End Try
        Return userCreated
    End Function

    Public Overridable Function CreateFromSSOData(
        userID As String,
        nome As String,
        cognome As String,
        email As String,
        profilo As Integer,
        params As ObjParams,
        Optional passwordHashEnabled As Boolean = False
    ) As Boolean Implements IUserWriter.CreateFromSSOData
        Dim userCreated As Boolean = False
        If IsNothing(params.ObjParametri_Utenti) OrElse IsNothing(params.ObjParametri_Server) Then
            Throw New InvalidOperationException()
        End If
        Try
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, params.ObjParametri_Utenti)
            'Dim username = nome & "." & cognome
            Dim password = generatePassword(passwordHashEnabled)
            Dim cf = GetNextSequentialCF(params.ObjParametri_Server)
            Dim groups As IEnumerable(Of Integer) = New List(Of Integer)
            userCreated = CreateNewUser(
                userID, nome, cognome, password, email,
                cf, String.Empty, String.Empty, profilo,
                groups, params.ObjParametri_Utenti, passwordHashEnabled
            )
            If userCreated Then
                'Same username generation as in the abstract class
                'Dim username = nome & "." & cognome
                AssignFullVisibility(userID, params.ObjParametri_Utenti)
            End If
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(1, params.ObjParametri_Utenti)
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(params.ObjParametri_Utenti)
        Catch ex As Exception
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, params.ObjParametri_Utenti)
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(params.ObjParametri_Utenti)
            Throw
        End Try
        Return userCreated
    End Function

    Public Overridable Function CreateFromImportNewAgriData(
        userID As String,
        nome As String,
        cognome As String,
        cf As String,
        email As String,
        profilo As Integer,
        params As ObjParams,
        Optional passwordHashEnabled As Boolean = False,
        Optional assignVisibility As Boolean = False,
        Optional setFlagAccessoSpid As Boolean = False,
        Optional gruppoUtenti As Integer = 0
    ) As Boolean
        Dim userCreated As Boolean = False
        If IsNothing(params.ObjParametri_Utenti) OrElse IsNothing(params.ObjParametri_Server) Then
            Throw New InvalidOperationException()
        End If

        Try
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, params.ObjParametri_Utenti)
            'Dim username = nome & "." & cognome
            Dim password = generatePassword(passwordHashEnabled)
            'Dim cf = GetNextSequentialCF(params.ObjParametri_Server)
            Dim groups As IEnumerable(Of Integer) = New List(Of Integer)
            If gruppoUtenti <> 0 Then
                CType(groups, List(Of Integer)).Add(gruppoUtenti)
            End If
            userCreated = CreateNewUser(
                userID, nome, cognome, password, email,
                cf, String.Empty, String.Empty, profilo,
                groups, params.ObjParametri_Utenti, passwordHashEnabled
            )
            If userCreated And assignVisibility Then
                'Same username generation as in the abstract class
                'Dim username = nome & "." & cognome
                AssignFullVisibility(userID, params.ObjParametri_Utenti)
            End If
            If userCreated And setFlagAccessoSpid Then
                Dim objUtentiW As New AgronicaCoreUtentiDAL.Utenti_Write
                objUtentiW.ModificaFlagSPID(userID, True, params.ObjParametri_Utenti)
            End If
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(1, params.ObjParametri_Utenti)
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(params.ObjParametri_Utenti)
        Catch ex As Exception
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, params.ObjParametri_Utenti)
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(params.ObjParametri_Utenti)
            Throw
        End Try
        Return userCreated
    End Function

    ''' <see cref="AgronicaCoreUtentiBIZ.Utenti.GeneraPasswordRequisiti"></see>
    Private Function generatePassword(passwordHashEnabled As Boolean) As String
        Dim minLength = 16
        Dim minOccurrence = 1
        Dim lower As New Regex("[a-z]", RegexOptions.None, TimeSpan.FromSeconds(3))
        Dim upper As New Regex("[A-Z]", RegexOptions.None, TimeSpan.FromSeconds(3))
        Dim number As New Regex("[0-9]", RegexOptions.None, TimeSpan.FromSeconds(3))
        Dim special As New Regex("[^a-zA-Z0-9]", RegexOptions.None, TimeSpan.FromSeconds(3))

        Dim nuovaPassword As String
        Dim rand As New Random()
        Dim validate = Function(pwd As String)
                           If Len(pwd) < minLength Then Return False
                           If lower.Matches(pwd).Count < minOccurrence Then Return False
                           If upper.Matches(pwd).Count < minOccurrence Then Return False
                           If number.Matches(pwd).Count < minOccurrence Then Return False
                           If special.Matches(pwd).Count < minOccurrence Then Return False
                           If Not passwordHashEnabled AndAlso Not Regex.IsMatch(pwd, EXPREG_PASSWORD, RegexOptions.None, TimeSpan.FromSeconds(3)) Then Return False
                           Return True
                       End Function
        Do
            nuovaPassword = ""
            Do
                Dim charCode = rand.Next(33, 127)
                Dim newChar = Chr(charCode)
                If Regex.IsMatch(newChar, EXPREG_PASSWORD, RegexOptions.None, TimeSpan.FromSeconds(3)) Then 'Ammetto solo un sotto-insieme di questi caratteri
                    nuovaPassword &= newChar
                End If
            Loop While nuovaPassword.Length < minLength
            'Ciclo esterno perché devo capire se la password generata rispetta i requisiti nella sua interezza
        Loop While validate(nuovaPassword) = False
        Return nuovaPassword
    End Function

    ''' <summary>
    ''' Retrieves a random Codice Fiscale (Italian tax code) using sequential generation method.
    ''' </summary>
    ''' <param name="objP_Server">The server parameters required.</param>
    ''' <returns>A String representing the generated Codice Fiscale.</returns>
    Public Function GetNextSequentialCF(objP_Server As AgronicaCoreParametri) As String
        Dim objNuovoId As New AgronicaCoreDataProvider.Agro_Sequenze
        Dim upper = 2000000000
        Dim nextSewNum = objNuovoId.NuovoId_Tabella("cf_fittizio_login_sso", 0, upper, objP_Server)
        Return "CF" & nextSewNum.ToString().PadLeft(14, "0"c)
    End Function

    Private Function CreateNewUser(
        username As String, name As String, surname As String,
        password As String, email As String, cf As String, piva As String,
        commercialUsername As String, permissionProfile As Integer,
        groups As IEnumerable(Of Integer),
        objP_Utenti As AgronicaCoreParametri,
        Optional passwordHashEnabled As Boolean = False,
        Optional shouldApplyProfileSettings As Boolean = False
    ) As Boolean
        Dim baseRecordsCreated = AddPersonBaseRecords(
            username, name, surname, password, email, cf, piva,
            commercialUsername, permissionProfile, objP_Utenti, passwordHashEnabled
        )
        If baseRecordsCreated Then
            ApplyProfilePermissions(username, permissionProfile, objP_Utenti)
            If shouldApplyProfileSettings Then
                ApplyProfileSettings(username, permissionProfile, objP_Utenti)
            End If
            If groups.Any Then
                AssignUserGroups(username, groups, objP_Utenti)
            End If
            Return True
        End If
        Return False
    End Function

    Private Sub AssignUserGroups(username As String, groups As IEnumerable(Of Integer), objP_Utenti As AgronicaCoreParametri)
        Dim objGruppiUt As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_W
        Dim validityStart As Date = #1/1/1900#
        Dim validityEnd As Date = #12/31/2100#
        Dim groupsEnumerator = groups.Where(Function(x) x <> 0).GetEnumerator()
        While groupsEnumerator.MoveNext
            objGruppiUt.Scrivi(
                groupsEnumerator.Current, username,
                validityStart, validityEnd,
                objP_Utenti
            )
        End While
    End Sub

    Private Sub ApplyProfilePermissions(username As String, permissionProfile As Integer, objP_Utenti As AgronicaCoreParametri)
        Dim objTipologiexPermessi_W As New AgronicaCoreUtentiDAL.Utenti_TipologiexPermessi_W
        Dim validityStart As Date = #1/1/1900#
        Dim validityEnd As Date = #12/31/2100#
        objTipologiexPermessi_W.GeneraPermessiUtenteDaTipologia(
            permissionProfile, username,
            validityStart, validityEnd,
            objP_Utenti
        )
    End Sub

    Private Sub ApplyProfileSettings(username As String, permissionProfile As Integer, objP_Utenti As AgronicaCoreParametri)
        Dim objImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_W
        Dim objImpostazioniFiltroMono As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_FiltroMono_W
        objImpostazioni.ApplicaProfilo(permissionProfile, username, objP_Utenti)
        objImpostazioniFiltroMono.ApplicaProfilo(permissionProfile, username, objP_Utenti)
    End Sub

    ''' <param name="commercialUsername">usernameCommerciale (user notes)</param>
    ''' <param name="permissionProfile">Tipologia_Cod of the user's profile</param>
    ''' <returns>A boolean describing if the base records were correctly created</returns>
    Private Function AddPersonBaseRecords(
        username As String, name As String, surname As String,
        password As String, email As String, cf As String, piva As String,
        commercialUsername As String, permissionProfile As Integer,
        objP_Utenti As AgronicaCoreParametri,
        Optional passwordHashEnabled As Boolean = False
    ) As Boolean
        Dim objUtentiWrite As New AgronicaCoreUtentiDAL.Utenti_Write
        Dim objDettagli As New AgronicaCoreUtentiDAL.Utenti_Dettagli_W
        Dim baseRecordCreated = objUtentiWrite.Scrivi(
            username, password, 0,
            passwordHashEnabled, False,
            objP_Utenti, permissionProfile
        )
        If baseRecordCreated Then
            objDettagli.Scrivi(
                username, surname, name,
                String.Empty, email, piva, cf.ToUpper, String.Empty,
                USER_TYPE_PERSON, commercialUsername, objP_Utenti
            )
            objDettagli.Modifica_PivaSuperUserUtente(username, objP_Utenti)
            Return True
        End If
        Return False
    End Function

    Private Sub AssignFullVisibility(username As String, objP_Utenti As AgronicaCoreParametri)
        Dim objProfilo As New AgronicaCoreUtentiDAL.Utenti_Profili_Write
        Dim fittizia As IEnumerable(Of String) = New List(Of String)

        objProfilo.Scrivi(
            username, CInt(enum_Id_Servizio.GiasOnline),
            CreaFiltroXMLPermessi(fittizia),
            CreaFiltroSQLPermessi(fittizia),
            Codice_1:=0, Codice_2:=0,
            AGRODATAINIZIO,
            AGRODATAFINE,
            objP_Utenti
        )
    End Sub

    Private Function CreaFiltroXMLPermessi(piveImprese As List(Of String)) As String
        Dim xmlPermessi As New StringBuilder
        xmlPermessi.Append("<DatiFiltri><Filtro><DatiGerarchiaImprese>")
        For Each piva In piveImprese
            If piva <> "" Then
                xmlPermessi.Append("<GerarchiaImprese padre=""" & piva & """/>")
            End If
        Next
        xmlPermessi.Append(
            "</DatiGerarchiaImprese><DatiPive/><Impresa><Struttura><Appezzamento><Impianto><Agenda><Contatto/>
            </Agenda></Impianto></Appezzamento></Struttura></Impresa></Filtro></DatiFiltri>"
        )
        If piveImprese.Count = 0 Then
            xmlPermessi.Clear()
        End If
        Return xmlPermessi.ToString()
    End Function

    Private Function CreaFiltroSQLPermessi(piveImprese As List(Of String)) As String
        Dim sqlPermessi As New StringBuilder
        For Each piva In piveImprese
            If piva <> "" Then
                sqlPermessi.Append(
                    " ((GerarchiaImprese.Padre = '" & piva &
                    "' and GerarchiaImprese.Foglia=1 ) OR Imprese.Piva = '" & piva & "') OR"
                )
            End If
        Next
        If sqlPermessi.ToString() <> "" Then
            Return "AND (" & Left(sqlPermessi.ToString, sqlPermessi.Length - 2) & ")"
        End If
        Return sqlPermessi.ToString()
    End Function


End Class
