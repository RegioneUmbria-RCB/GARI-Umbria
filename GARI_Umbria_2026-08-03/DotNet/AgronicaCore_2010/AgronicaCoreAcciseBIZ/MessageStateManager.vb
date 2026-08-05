Imports AgronicaCoreDataProvider.TipiEnumerativi

Public NotInheritable Class MessageStateManager

    Public Shared Function Recognized(ByVal tipoMessaggio As String) As Boolean

        Select Case tipoMessaggio.ToUpper()
            Case "IE818", "IE813", "IE815", "IE810", "IE801", "ALCOA"
                Return True
            Case Else
                Return False
        End Select

    End Function

    Public Shared Function GetState(ByVal tipoMessaggio As String,
                                    ByVal currentStep As DAA_Step_Enum,
                                    ByVal objectState As Object_State,
                                    ByVal stepSucces As Boolean,
                                    ByVal currentObjectState As enum_WWorflow_WAnagraficaStati) As enum_WWorflow_WAnagraficaStati


        Select Case currentStep
            Case DAA_Step_Enum.Caricamento
                If objectState = Object_State.Messaggio Then
                    Return If(stepSucces, enum_WWorflow_WAnagraficaStati.DAA_Telematico_File_Firmato_Inviato, currentObjectState)
                End If
                If objectState = Object_State.Testata Then
                    Select Case tipoMessaggio.ToUpper()
                        Case "IE815"
                            Return If(stepSucces, enum_WWorflow_WAnagraficaStati.DAA_Telematico_DAA_proposto_IE815_inviato_a_sistema, currentObjectState)
                        Case "IE810"
                            Return If(stepSucces, enum_WWorflow_WAnagraficaStati.DAA_Telematico_Annullamento_del_DAA, currentObjectState)
                        Case "IE813"
                            Return If(stepSucces, enum_WWorflow_WAnagraficaStati.DAA_Telematico_Cambio_destinazione_IE813_inviato_a_sistema, currentObjectState)
                        Case "IE818"
                            Return If(stepSucces, enum_WWorflow_WAnagraficaStati.DAA_Telematico_Nota_di_Ricevimento_IE818_Inviato_a_sistema, currentObjectState)
                        Case "ALCOA"
                            Return If(stepSucces, enum_WWorflow_WAnagraficaStati.DAA_Telematico_Riepilogo_Caricato_a_sistema, currentObjectState)
                        Case Else
                            Throw New NotImplementedException(String.Format("Tipo messaggio {0} non gestito", tipoMessaggio))
                    End Select
                End If
            Case DAA_Step_Enum.RispostaRicevuta
                If objectState = Object_State.Messaggio Then
                    Return If(stepSucces, enum_WWorflow_WAnagraficaStati.DAA_Telematico_In_fase_di_verifica, enum_WWorflow_WAnagraficaStati.DAA_Telematico_Risposta_Negativa_Ricevuta)
                End If
                If objectState = Object_State.Testata Then
                    Select Case tipoMessaggio.ToUpper()
                        Case "IE815"
                            Return enum_WWorflow_WAnagraficaStati.DAA_Telematico_DAA_proposto_IE815_contenente_errori
                        Case "IE810"
                            Return enum_WWorflow_WAnagraficaStati.DAA_Telematico_DAA_Annullato_IE819_contenente_errori
                        Case "IE813"
                            Return enum_WWorflow_WAnagraficaStati.DAA_Telematico_Cambio_destinazione_IE813_contenente_errori
                        Case "IE818"
                            Return enum_WWorflow_WAnagraficaStati.DAA_Telematico_Nota_di_Ricevimento_IE818_contenente_errori
                        Case "ALCOA"
                            Return enum_WWorflow_WAnagraficaStati.DAA_Telematico_Riepilogo_contenente_errori
                        Case Else
                            Throw New NotImplementedException(String.Format("Tipo messaggio {0} non gestito", tipoMessaggio))
                    End Select
                End If
            Case DAA_Step_Enum.EsitoRicevuto
                If objectState = Object_State.Messaggio Then
                    Return If(stepSucces, enum_WWorflow_WAnagraficaStati.DAA_Telematico_Risposta_Positiva_Ricevuta, enum_WWorflow_WAnagraficaStati.DAA_Telematico_Risposta_Negativa_Ricevuta)
                ElseIf objectState = Object_State.Testata Then
                    Select Case tipoMessaggio.ToUpper()
                        Case "IE815"
                            Return If(stepSucces, enum_WWorflow_WAnagraficaStati.DAA_Telematico_DAA_proposto_IE815_vagliato_correttamente, enum_WWorflow_WAnagraficaStati.DAA_Telematico_DAA_proposto_IE815_contenente_errori)
                        Case "IE810"
                            Return If(stepSucces, enum_WWorflow_WAnagraficaStati.DAA_Telematico_DAA_Annullato_IE819_ricevuto, enum_WWorflow_WAnagraficaStati.DAA_Telematico_DAA_Annullato_IE819_contenente_errori)
                        Case "IE813"
                            Return If(stepSucces, enum_WWorflow_WAnagraficaStati.DAA_Telematico_Cambio_destinazione_IE813_Vagliato_Correttamente, enum_WWorflow_WAnagraficaStati.DAA_Telematico_Cambio_destinazione_IE813_contenente_errori)
                        Case "IE818"
                            Return If(stepSucces, enum_WWorflow_WAnagraficaStati.DAA_Telematico_Nota_di_Ricevimento_IE818_Vagliato_correttamente, enum_WWorflow_WAnagraficaStati.DAA_Telematico_Nota_di_Ricevimento_IE818_contenente_errori)
                        Case "ALCOA"
                            Return If(stepSucces, enum_WWorflow_WAnagraficaStati.DAA_Telematico_Riepilogo_vagliato_correttamente, enum_WWorflow_WAnagraficaStati.DAA_Telematico_Riepilogo_contenente_errori)
                        Case Else
                            Throw New NotImplementedException(String.Format("Tipo messaggio {0} non gestito", tipoMessaggio))
                    End Select
                End If
        End Select

    End Function

End Class

Public Enum DAA_Step_Enum
    Caricamento
    RispostaRicevuta
    EsitoRicevuto
End Enum

Public Enum Object_State
    Messaggio
    Testata
End Enum