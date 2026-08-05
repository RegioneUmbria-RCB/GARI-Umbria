Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreDataProvider

Namespace Agenda.Operazioni_Colturali

    Public Module Handler_Operazione


        Public Function Crea_Nuova_OperazioneColturale(Partita_Iva_Azienda As String, Data_Operazione As Date, Tipo_Operazione_db As TipiEnumerativi.enum_TipoOperazioneDB, ByVal ID_Agenda As String, ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Operazione_Colturale.I_Operazione_Colturale
            Dim Temp_Operazione_Colturale As Operazione_Colturale.I_Operazione_Colturale
            Try
                Temp_Operazione_Colturale = New Operazione_Colturale.Operazione_Colturale(Partita_Iva_Azienda, Data_Operazione, Tipo_Operazione_db, ID_Agenda, objParametri_Server)
            Catch
                Return Nothing
            End Try
            Return Temp_Operazione_Colturale
        End Function


        Public Function Leggi_Operazione_Da_Sessione() As Operazione_Colturale.I_Operazione_Colturale
            Dim Temp_Operazione_Colturale As Operazione_Colturale.I_Operazione_Colturale
            Try
                Temp_Operazione_Colturale = HttpContext.Current.Session("Operazione_Colturale_Generica")
            Catch
                Return Nothing
            End Try
            Return Temp_Operazione_Colturale
        End Function

        Public Function Salva_Operazione_In_Sessione(Temp_Operazione_Colturale As Operazione_Colturale.I_Operazione_Colturale) As Boolean
            Try
                HttpContext.Current.Session("Operazione_Colturale_Generica") = Temp_Operazione_Colturale
            Catch ex As Exception
                Return False
            End Try
            Return True
        End Function


        'Public Function Leggi_Operazione_Da_Agenda(ByRef Operazione_Colturale_Da_Leggere As Operazione_Colturale.I_Operazione_Colturale, ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        '    Dim Temp_Operazione_Colturale As Operazione_Colturale.I_Operazione_Colturale
        '    Try
        '        Dim IO_Operaz As New IO.OperazioneColturale_ToFrom_AgendaDB(Operazione_Colturale_Da_Leggere, objParametri_Server)
        '        Temp_Operazione_Colturale = IO_Operaz.Leggi_Operazione()
        '    Catch
        '        Return False
        '    End Try
        '    If Not IsNothing(Temp_Operazione_Colturale) Then
        '        Operazione_Colturale_Da_Leggere = Temp_Operazione_Colturale
        '        Return True
        '    Else
        '        Return False
        '    End If
        '    Return False
        'End Function


        'Function Salva_Operazione_Su_Agenda(ByRef Operazione_Colturale_Da_Salvare As Operazione_Colturale.I_Operazione_Colturale, ByRef objParametri_Server As AgronicaCoreParametri, ByRef messaggiErrore As AgronicaCoreUtility.Messages_Str) As Boolean
        '    Try
        '        Dim IO_Operaz As New IO.OperazioneColturale_ToFrom_AgendaDB(Operazione_Colturale_Da_Salvare, objParametri_Server)
        '        Return IO_Operaz.Salva_Operazione(messaggiErrore)
        '    Catch
        '        Return False
        '    End Try
        '    Return False
        'End Function


        Function Elimina_Operazione_Da_Sessione() As Boolean
            Try
                HttpContext.Current.Session.Remove("Operazione_Colturale_Generica")
            Catch ex As Exception
                Return False
            End Try

            Return True
        End Function


        ''la proprietà dell'handler legge e scrive inm maniera trasparente in sessione
        'Public Property Operazione_Colturale As Operazione_Colturale.I_Operazione_Colturale Implements I_Operazione_Colturale_Handler.Operazione_Colturale
        '    Get
        '        Dim Temp_Operazione_Colturale As Operazione_Colturale.I_Operazione_Colturale
        '        Temp_Operazione_Colturale = HttpContext.Current.Session("Operazione_Colturale_Generica")
        '        Return Temp_Operazione_Colturale
        '    End Get
        '    Set(value As Operazione_Colturale.I_Operazione_Colturale)
        '        Dim Temp_Operazione_Colturale As Operazione_Colturale.I_Operazione_Colturale
        '        Temp_Operazione_Colturale = value
        '        HttpContext.Current.Session("Operazione_Colturale_Generica") = Temp_Operazione_Colturale
        '    End Set
        'End Property


        'Public Function CreaNuovaOperazioneColturale(Partita_Iva_Azienda As String, Data_Operazione As Date, Tipo_Operazione_db As TipiEnumerativi.enum_TipoOperazioneDB, ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean Implements I_Operazione_Colturale_Handler.CreaNuovaOperazioneColturale
        '    Try
        '        Operazione_Colturale = New Operazione_Colturale.Operazione_Colturale(Partita_Iva_Azienda, Data_Operazione, Tipo_Operazione_db, objParametri_Server)
        '    Catch
        '        Return False
        '    End Try
        '    Return True
        'End Function

        'Public Sub EliminaOperazione() Implements I_Operazione_Colturale_Handler.EliminaOperazione
        '    Operazione_Colturale = Nothing
        'End Sub




    End Module

End Namespace
