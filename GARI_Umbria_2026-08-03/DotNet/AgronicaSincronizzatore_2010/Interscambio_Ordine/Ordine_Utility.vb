Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreXMLUniversale
Public Class Ordine_Utility

    Public Shared Function CreaOrderMessageType(ByVal Intestazione As StandardBusinessDocumentHeader, ByVal Ordini As List(Of OrderType)) As OrderMessageType

        Dim doc As New OrderMessageType With {
            .StandardBusinessDocumentHeader = Intestazione,
            .order = Ordini.ToArray()
        }
        Return doc

    End Function

    Public Shared Function CreaStandardBusinessDocument(ByVal Origine As String, ByVal Destinazione As String, ByVal Tipo As String) As StandardBusinessDocumentHeader

        Dim sender = New Partner With {.Identifier = New PartnerIdentification() With {.Value = Origine}}
        Dim receiver = New Partner With {.Identifier = New PartnerIdentification() With {.Value = Destinazione}}
        Dim Data = Date.Now
        Dim Id = Origine & Data

        Dim sbd = New StandardBusinessDocumentHeader() With {
            .DocumentIdentification = New DocumentIdentification() With {
                .InstanceIdentifier = Id,
                .Type = Tipo,
                .MultipleType = False,
                .CreationDateAndTime = Data
            },
            .Sender = New Partner() {sender},
            .Receiver = New Partner() {receiver}
        }

        Return sbd

    End Function

    Public Shared Function CreaOrderType(ByVal Cliente As String, ByVal Fornitore As String, ByVal Ricetta As Ricette_Operazioni, ByVal Dettagli As DataTable, ByRef objParametri_Interscambio As AgronicaCoreParametri, ByRef logErrori As StringBuilder) As OrderType

        Dim objInterscambioR As New Gias_Interscambio_R
        Dim orderLineItem As New List(Of OrderLineItemType)
        Dim orderId = Cliente & "|" & Ricetta.Ricetta_Cod & "|" & Ricetta.Ricetta_Operazione_Cod
        Dim riga As Integer = 0

        For Each dettaglio In Dettagli.Rows

            Dim qta = dettaglio.Item("Qta_Extra_Totale")
            Dim udm = dettaglio.Item("Udm_Sim")
            Dim elem = dettaglio.Item("Elem_Cod")
            Dim codice = If(dettaglio.Item("Pro_Cod") <> 0, dettaglio.Item("Pro_Cod"), dettaglio.Item("Mat_Cod"))
            Dim descrizione As String = ""

            If elem = CostantiPersonalizzate.FERTILIZZANTI Then
                descrizione = dettaglio.Item("Fer_Des")
            ElseIf elem = CostantiPersonalizzate.FORMULATI Then
                descrizione = dettaglio.Item("Fr_Des")
            Else
                descrizione = dettaglio.Item("Mat_Des")
            End If

            Dim dt = objInterscambioR.LeggiCodiciProdotti("", 0, "", "", elem, codice, "", "", objParametri_Interscambio)

            ' esporto solo prodotti mappati nel db di interscambio
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then

                riga += 1

                elem = dt.Rows(0).Item("Cod_Categoria_ALTRO")
                codice = dt.Rows(0).Item("Cod_Prodotto_ALTRO")
                descrizione = dt.Rows(0).Item("Descr_Prodotto")

                orderLineItem.Add(New OrderLineItemType With {
                                     .lineItemNumber = riga,
                                     .requestedQuantity = New QuantityType With {.measurementUnitCode = udm, .Value = qta},
                                     .additionalOrderLineInstruction = New Description200Type() {},
                                     .note = New Description500Type() With {.Value = descrizione, .languageCode = "IT"},
                                     .transactionalTradeItem = New TransactionalTradeItemType With {.gtin = elem & "|" & codice}
                                  })

            Else

                logErrori.AppendLine("Ordine " & orderId & ": Prodotto " & descrizione & " (" & elem & "|" & codice & ") non presente sul database di interscambio")

            End If

        Next

        If orderLineItem.Count > 0 Then

            Dim order As New OrderType With {
                .creationDateTime = Ricetta.Validita_Inizio,
                .documentStatusCode = DocumentStatusEnumerationType.ORIGINAL,
                .avpList = New EcomStringAttributeValuePairListType() {
                                                                          New EcomStringAttributeValuePairListType With {.attributeName = "STORNO", .Value = "false"}
                                                                      },
                .orderIdentification = New Ecom_EntityIdentificationType With {
                    .entityIdentification = orderId,
                    .contentOwner = New Ecom_PartyIdentificationType With {.gln = Cliente}
                },
                .buyer = New TransactionalPartyType With {.gln = Cliente},
                .seller = New TransactionalPartyType With {.gln = Fornitore},
                .orderLogisticalInformation = New OrderLogisticalInformationType With {
                    .shipFrom = New TransactionalPartyType With {.gln = Fornitore},
                    .shipTo = New TransactionalPartyType With {.gln = Cliente},
                    .orderLogisticalDateInformation = New OrderLogisticalDateInformationType With {
                        .requestedDeliveryDateTime = New DateOptionalTimeType With {.date = Ricetta.Validita_Inizio}
                    }
                },
                .orderLineItem = orderLineItem.ToArray
            }

            Return order

        End If

        Return Nothing

    End Function

    Public Shared Function CancellaOrderType(ByVal Cliente As String, ByVal Fornitore As String, ByVal Recode As G2G_Recode_Ricette_Operazioni) As OrderType

        Dim order As New OrderType With {
            .creationDateTime = Date.Now,
            .documentStatusCode = DocumentStatusEnumerationType.ORIGINAL,
            .avpList = New EcomStringAttributeValuePairListType() {
                                                                      New EcomStringAttributeValuePairListType With {.attributeName = "STORNO", .Value = "true"}
                                                                  },
            .orderIdentification = New Ecom_EntityIdentificationType With {
                .entityIdentification = Cliente & "|" & Recode.From_Ricetta_Cod & "|" & Recode.From_Ricetta_Operazione_Cod,
                .contentOwner = New Ecom_PartyIdentificationType With {.gln = Cliente}
            },
            .buyer = New TransactionalPartyType With {.gln = Cliente},
            .seller = New TransactionalPartyType With {.gln = Fornitore}
        }

        Return order

    End Function

End Class
