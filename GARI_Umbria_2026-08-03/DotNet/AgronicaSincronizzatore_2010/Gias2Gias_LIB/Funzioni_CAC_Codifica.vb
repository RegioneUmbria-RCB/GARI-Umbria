Imports System.Xml
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports System.Text

Partial Public Class Funzioni

#Region "CAC_Codifica_Animali"

    Public Sub Elabora_CAC_Codifica_Animali_Salva( _
                                ByVal objOpzioni As clsOpzioni, _
                                ByRef Log_Import As StringBuilder, _
                                ByRef Log_Errori As StringBuilder, _
                                ByRef Log_Riepilogo As StringBuilder)

        Const nomeFunzione As String = "CAC_Codifica_Zone"


        Try

            'TODO

        Catch ex As Exception
            Dim msg As String
            msg = CStr(Date.Now) + " - " + nomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try


    End Sub
#End Region


#Region "CAC_Codifica_Zone"

    Public Sub Elabora_CAC_Codifica_Zone_Salva( _
                                ByVal objOpzioni As clsOpzioni, _
                                ByRef Log_Import As StringBuilder, _
                                ByRef Log_Errori As StringBuilder, _
                                ByRef Log_Riepilogo As StringBuilder)

        Const nomeFunzione As String = "CAC_Codifica_Zone"


        Try

            Dim obj_R As New AgronicaCoreAnagrafeDAL.CAC_Codifica_Zone
            Dim obj_W As New AgronicaCoreAnagrafeDAL.CAC_Codifica_Zone_W

            Dim DT As DataTable = _
                obj_R.Leggi( _
                        "", _                        
                        objOpzioni.objParametri_Server_GIAS_ORIGINE _
                )

            If Not DT Is Nothing Then

                For Each dRow In DT.Rows

                    If obj_R.Leggi(dRow("Zona_Cod_Cliente"), objOpzioni.objParametri_Server_GIAS_DESTINAZIONE).Rows.Count = 0 Then
                        obj_W.scrivi( _
                            dRow("Zona_Cod_Cliente"), _
                            DBNullToNothing(dRow("Descrizione")), _
                            dRow("Zona_Cod_Gias"), _
                            objOpzioni.objParametri_Server_GIAS_DESTINAZIONE, _
                            dRow("Data_Modifica") _
                        )
                    End If

                Next

            End If

        Catch ex As Exception
            Dim msg As String
            msg = CStr(Date.Now) + " - " + nomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try


    End Sub
#End Region


#Region "CAC_Codifica_Cultivar"

    Public Sub Elabora_CAC_Codifica_Cultivar_Salva( _
                                ByVal objOpzioni As clsOpzioni, _
                                ByRef Log_Import As StringBuilder, _
                                ByRef Log_Errori As StringBuilder, _
                                ByRef Log_Riepilogo As StringBuilder)

        Const nomeFunzione As String = "CAC_Codifica_Cultivar"


        Try

            Dim obj_R As New AgronicaCoreAnagrafeDAL.CAC_Codifica_Cultivar_R
            Dim obj_W As New AgronicaCoreAnagrafeDAL.CAC_Codifica_Cultivar_W

            Dim DT As DataTable = _
                obj_R.Leggi("", _
                            0, _
                             0, _
                            #1/1/1900#, _
                             0, 0, 0, 0, _
                             "", "", "", _
                                objOpzioni.objParametri_Server_GIAS_ORIGINE _
                                )

            If Not DT Is Nothing Then

                For Each dRow In DT.Rows

                    If obj_R.Leggi(dRow("cultivar_coltiva"), dRow("cultivar_Gias"), dRow("veg_Cod_gias"), AGRODATAINIZIO, 0, 0, 0, 0, 0, "", "", objOpzioni.objParametri_Server_GIAS_DESTINAZIONE).Rows.Count = 0 Then

                        obj_W.Scrivi(dRow("Cultivar_Coltiva") _
                                    , dRow("Cultivar_Gias") _
                                     , dRow("Veg_Cod_Gias") _
                                    , DBNullToNothing(dRow("Grfi_Cod")) _
                                    , DBNullToNothing(dRow("Reg_Cod")) _
                                    , DBNullToNothing(dRow("Grva_Cod")) _
                                     , DBNullToNothing(dRow("Metodo_Produzione")) _
                                    , DBNullToNothing(dRow("Descrizione")) _
                                    , dRow("Cultivar_Coltiva_2") _
                                    , DBNullToNothing(dRow("Data_Modifica")) _
                                    , objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)

                    End If

                Next

            End If

        Catch ex As Exception
            Dim msg As String
            msg = CStr(Date.Now) + " - " + nomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try


    End Sub


#End Region

#Region "CAC_Codifica_FormeAllevamento"

    Public Sub Elabora_CAC_Codifica_FormeAllevamento_Salva( _
                                ByVal objOpzioni As clsOpzioni, _
                                ByRef Log_Import As StringBuilder, _
                                ByRef Log_Errori As StringBuilder, _
                                ByRef Log_Riepilogo As StringBuilder)

        Const nomeFunzione As String = "CAC_Codifica_FormeAllevamento"


        Try

            Dim obj_R As New AgronicaCoreAnagrafeDAL.CAC_Codifica_FormeAllevamento_R
            Dim obj_W As New AgronicaCoreAnagrafeDAL.CAC_Codifica_FormeAllevamento_W

            Dim DT As DataTable = _
                obj_R.Leggi("", _
                            0, _
                            "", "", _
                             objOpzioni.objParametri_Server_GIAS_ORIGINE)

            If Not DT Is Nothing Then

                For Each dRow In DT.Rows

                    If obj_R.Leggi(dRow("foral_cod_cliente"), dRow("foral_cod_gias"), "", "", objOpzioni.objParametri_Server_GIAS_DESTINAZIONE).Rows.Count = 0 Then
                        obj_W.Scrivi(DBNullToNothing(dRow("Descrizione")) _
                                    , dRow("Foral_Cod_Cliente") _
                                    , dRow("Foral_Cod_Gias") _
                                    , objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser _
                                    , dRow("Data_Modifica") _
                                    , objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)
                    End If

                Next

            End If

        Catch ex As Exception
            Dim msg As String
            msg = CStr(Date.Now) + " - " + nomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try


    End Sub


#End Region

#Region "CAC_Codifica_InfoAggiuntive"

    Public Sub Elabora_CAC_Codifica_InfoAggiuntive_Salva( _
                                ByVal objOpzioni As clsOpzioni, _
                                ByRef Log_Import As StringBuilder, _
                                ByRef Log_Errori As StringBuilder, _
                                ByRef Log_Riepilogo As StringBuilder)

        Const nomeFunzione As String = "CAC_Codifica_InfoAggiuntive"


        Try

            Dim obj_R As New AgronicaCoreAnagrafeDAL.CAC_Codifica_InfoAggiuntive_R
            Dim obj_W As New AgronicaCoreAnagrafeDAL.CAC_Codifica_InfoAggiuntive_W

            Dim DT As DataTable = _
                obj_R.Leggi(0, _
                            "", _
                            0, _
                             0, _
                              enumSelezioneVariabile.Selezione_TabellaCompleta, _
                               "", "", _
                             objOpzioni.objParametri_Server_GIAS_ORIGINE)

            If Not DT Is Nothing Then

                For Each dRow In DT.Rows

                    If obj_R.Leggi(dRow("Argomento_Cod"), dRow("InfoAgg_Cod"), 0, 1, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objOpzioni.objParametri_Server_GIAS_DESTINAZIONE).Rows.Count = 0 Then
                        obj_W.Scrivi( _
                                      dRow("InfoAgg_Cod") _
                                    , DBNullToNothing(dRow("InfoAgg_Des")) _
                                    , dRow("Argomento_Cod") _
                                    , dRow("Tipo_Codifica") _
                                    , DBNullToNothing(dRow("Argomento_Des")) _
                                    , DBNullToNothing(dRow("CodiceAux_1")) _
                                    , DBNullToNothing(dRow("CodiceAux_2")) _
                                    , DBNullToNothing(dRow("CodiceAux_3")) _
                                    , DBNullToNothing(dRow("TestoAux_1")) _
                                    , DBNullToNothing(dRow("TestoAux_2")) _
                                    , DBNullToNothing(dRow("TestoAux_3")) _
                                    , DBNullToNothing(dRow("Validita_Inizio")) _
                                    , DBNullToNothing(dRow("Validita_Fine")) _
                                    , objOpzioni.objParametri_Server_GIAS_DESTINAZIONE _
                                    , DBNullToNothing(dRow("Data_Creazione")) _
                                    , DBNullToNothing(dRow("Data_Modifica")) _
                                    , DBNullToNothing(dRow("Username_Creazione")) _
                                    , DBNullToNothing(dRow("Username_Modifica")) _
                                )


                    End If

                Next

            End If

        Catch ex As Exception
            Dim msg As String
            msg = CStr(Date.Now) + " - " + nomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try


    End Sub


#End Region

#Region "CAC_Codifica_Portinnesti"

    Public Sub Elabora_CAC_Codifica_Portinnesti_Salva( _
                                ByVal objOpzioni As clsOpzioni, _
                                ByRef Log_Import As StringBuilder, _
                                ByRef Log_Errori As StringBuilder, _
                                ByRef Log_Riepilogo As StringBuilder)

        Const nomeFunzione As String = "CAC_Codifica_Portinnesti"


        Try

            Dim obj_R As New AgronicaCoreAnagrafeDAL.CAC_Codifica_Portinnesti_R
            Dim obj_W As New AgronicaCoreAnagrafeDAL.CAC_Codifica_Portinnesti_W

            Dim DT As DataTable = _
                obj_R.Leggi("", _
                            0, _
                            "", _
                            "", _
                             objOpzioni.objParametri_Server_GIAS_ORIGINE)

            If Not DT Is Nothing Then

                For Each dRow In DT.Rows

                    If obj_R.Leggi(dRow("port_cod_cliente"), dRow("port_cod_gias"), "", "", objOpzioni.objParametri_Server_GIAS_DESTINAZIONE).Rows.Count = 0 Then

                        obj_W.Scrivi(DBNullToNothing(dRow("Descrizione")) _
                                    , objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser _
                                    , dRow("Port_Cod_Cliente") _
                                    , dRow("Port_Cod_Gias") _
                                    , dRow("Data_modifica") _
                                    , objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)
                    End If

                Next

            End If

        Catch ex As Exception
            Dim msg As String
            msg = CStr(Date.Now) + " - " + nomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try


    End Sub


#End Region

#Region "CAC_Codifica_Veg_Cod"

    Public Sub Elabora_CAC_Codifica_Veg_Cod_Salva( _
                                ByVal objOpzioni As clsOpzioni, _
                                ByRef Log_Import As StringBuilder, _
                                ByRef Log_Errori As StringBuilder, _
                                ByRef Log_Riepilogo As StringBuilder)

        Const nomeFunzione As String = "CAC_Codifica_Veg_Cod"


        Try

            Dim obj_R As New AgronicaCoreAnagrafeDAL.CAC_Codifica_Veg_Cod_R
            Dim obj_W As New AgronicaCoreAnagrafeDAL.CAC_Codifica_Veg_Cod_W

            Dim DT As DataTable = _
                obj_R.Leggi("", _
                            0, _
                             "", _
                             AGRODATAINIZIO, _
                              0, 0, 0, 0, 0, 0, _
                              "", _
                               enumSelezioneVariabile.Selezione_TabellaCompleta, _
                               "", "", _
                             objOpzioni.objParametri_Server_GIAS_ORIGINE)

            If Not DT Is Nothing Then

                For Each dRow In DT.Rows

                    If obj_R.Leggi(dRow("veg_cod_coltiva"), _
                             dRow("veg_cod_gias"), _
                             "", _
                             AGRODATAINIZIO, _
                              0, 0, 0, 0, 0, 0, _
                              "", _
                               enumSelezioneVariabile.Selezione_TabellaCompleta, _
                               "", "", _
                             objOpzioni.objParametri_Server_GIAS_DESTINAZIONE).Rows.Count = 0 Then


                        obj_W.Scrivi(DBNullToNothing(dRow("Descrizione")) _
                                    , DBNullToNothing(dRow("Grfi_Cod")) _
                                    , DBNullToNothing(dRow("Grva_Cod")) _
                                    , DBNullToNothing(dRow("Id_Cod")) _
                                    , dRow("Metodo_Produzione") _
                                    , DBNullToNothing(dRow("Raggruppamento_Varietale_Cod")) _
                                    , DBNullToNothing(dRow("Reg_Cod")) _
                                    , dRow("Veg_Cod_Coltiva") _
                                    , DBNullToNothing(dRow("Veg_Cod_Coltiva_2")) _
                                    , dRow("Veg_Cod_Gias") _
                                    , dRow("Data_Modifica") _
                                    , objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)
                    End If

                Next

            End If

        Catch ex As Exception
            Dim msg As String
            msg = CStr(Date.Now) + " - " + nomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try


    End Sub


#End Region

End Class
