Imports log4net
Imports Newtonsoft.Json
Imports System.Xml

Public Class ConfiguratoreLog4Net

    Private _configurazioneInizializzata As Boolean
    Public Property ConfigurazioneInizializzata() As Boolean
        Get
            Return _configurazioneInizializzata
        End Get
        Set(ByVal value As Boolean)
            _configurazioneInizializzata = value
        End Set
    End Property

    Public Sub ImpostaProprieta(parametriExtra As String)

        Const nomeRoutine As String = "ConfiguratoreLog4Net.ImpostaProprieta()"
        Const prefissoProprietaPercorsoLog As String = "PercorsoLog"

        Dim nomeProprietaPercorsoLog As String
        Dim valoreProprietaPercorsoLog As String

        Try

            Dim importatori = JsonConvert.DeserializeObject(Of Importatori)(parametriExtra)

            If Not IsNothing(importatori) AndAlso importatori.Configurazioni.Count > 0 Then

                For Each configurazione In importatori.Configurazioni

                    If Not String.IsNullOrEmpty(configurazione.PercorsoLog) Then

                        nomeProprietaPercorsoLog = String.Format("{0}_{1}",
                                                                 prefissoProprietaPercorsoLog,
                                                                 configurazione.IdServizio)

                        valoreProprietaPercorsoLog = configurazione.PercorsoLog

                        SeImpostaProprietaLog4Net(nomeProprietaPercorsoLog, valoreProprietaPercorsoLog)

                    End If

                Next

            End If

        Catch ex As Exception

            Dim messaggioEccezione = String.Format("[{0}] : {1} ({2})",
                                                   nomeRoutine,
                                                   "Errore in impostazione proprietà file configurazione log4net",
                                                   ex.Message)

            Throw New Exception(messaggioEccezione)

        End Try

    End Sub

    Public Sub ImpostaProprietaDefault(fileConfigLog4Net As String)

        Const nomeRoutine As String = "ConfiguratoreLog4Net.ImpostaProprietaDefault()"
        Const prefissoProperty As String = "%property{"
        Const percorsoLogDefault As String = "Log"

        Dim nomeProprietaPercorsoLog As String

        Try

            Dim documentoXML As New XmlDocument

            documentoXML.Load(fileConfigLog4Net)

            Dim listaNodiAppender = documentoXML.SelectNodes(".//appender")

            If Not listaNodiAppender Is Nothing AndAlso listaNodiAppender.Count > 0 Then

                For Each app As XmlNode In listaNodiAppender

                    Dim nodoFile = app.SelectSingleNode("file")

                    If Not IsNothing(nodoFile) Then

                        Dim valoreNodoFile = nodoFile.Attributes("value").Value

                        If valoreNodoFile.Contains(prefissoProperty) Then

                            nomeProprietaPercorsoLog = Replace(Split(valoreNodoFile, "}")(0),
                                                               prefissoProperty,
                                                               "")

                            SeImpostaProprietaLog4Net(nomeProprietaPercorsoLog, percorsoLogDefault)


                        End If

                    End If

                Next

            End If

        Catch ex As Exception

            Dim messaggioEccezione = String.Format("[{0}] : {1} ({2})",
                                                   nomeRoutine,
                                                   "Errore in impostazione proprietà file configurazione log4net",
                                                   ex.Message)

            Throw New Exception(messaggioEccezione)

        End Try

    End Sub

    Private Sub SeImpostaProprietaLog4Net(nomeProprietaPercorsoLog As String,
                                          valoreProprietaPercorsoLog As String)

        log4net.GlobalContext.Properties.Item(nomeProprietaPercorsoLog) = valoreProprietaPercorsoLog

    End Sub

End Class
