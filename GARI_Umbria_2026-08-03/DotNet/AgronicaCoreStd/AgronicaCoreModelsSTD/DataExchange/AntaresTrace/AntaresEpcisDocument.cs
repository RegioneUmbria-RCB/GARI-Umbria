using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace AgronicaCoreModelsSTD.DataExchange.AntaresTrace
{
    // Questa è la radice del documento XML. 
    // Definisce il namespace principale "epcis" (urn:epcglobal:epcis:xsd:2)
    [XmlRoot("EPCISDocument", Namespace = "urn:epcglobal:epcis:xsd:2")]
    public class AntaresEpcisDocument
    {
        // Versione dello schema EPCIS (fisso a 2.0)
        [XmlAttribute("schemaVersion")]
        public string SchemaVersion { get; set; } = "2.0";

        // Data creazione del file XML (es. 2025-09-12T07:12:00Z)
        [XmlAttribute("creationDate")]
        public string CreationDate { get; set; }

        // L'utilizzo di Namespace = "" fa sparire il prefisso "epcis:" da questo nodo in giù
        // L'intestazione del documento (contiene Mittente/Destinatario)
        [XmlElement("EPCISHeader", Namespace = "")]
        public EpcisHeader Header { get; set; }

        // Il corpo del documento (contiene la lista degli eventi: Semina, Raccolta, ecc.)
        [XmlElement("EPCISBody", Namespace = "")]
        public EpcisBody Body { get; set; }
    }

    public class EpcisHeader
    {
        // Header standard SBDH (Standard Business Document Header) richiesto da GS1
        [XmlElement("StandardBusinessDocumentHeader", Namespace = "http://www.unece.org/cefact/namespaces/StandardBusinessDocumentHeader")]
        public SbdhHeader Sbdh { get; set; }
    }

    public class SbdhHeader
    {
        [XmlElement("HeaderVersion", Namespace = "http://www.unece.org/cefact/namespaces/StandardBusinessDocumentHeader")]
        public string HeaderVersion { get; set; } = "1.0";

        // Chi invia il messaggio (es. Food Metaverse)
        [XmlElement("Sender", Namespace = "http://www.unece.org/cefact/namespaces/StandardBusinessDocumentHeader")]
        public SbdhPartner Sender { get; set; }

        // Chi riceve il messaggio (es. Food Metaverse)
        [XmlElement("Receiver", Namespace = "http://www.unece.org/cefact/namespaces/StandardBusinessDocumentHeader")]
        public SbdhPartner Receiver { get; set; }

        // Identificativi univoci del documento (ID, Tipo, Data)
        [XmlElement("DocumentIdentification", Namespace = "http://www.unece.org/cefact/namespaces/StandardBusinessDocumentHeader")]
        public SbdhDocId DocumentIdentification { get; set; }
    }
    // Partner (Mittente o Destinatario)
    public class SbdhPartner
    {
        [XmlElement("Identifier", Namespace = "http://www.unece.org/cefact/namespaces/StandardBusinessDocumentHeader")]
        public SbdhIdentifier Identifier { get; set; }
    }
    // Identificatore del Partner es P.IVA
    public class SbdhIdentifier
    {
        // Autorità che ha emesso il codice (di solito "GS1")
        [XmlAttribute("Authority")]
        public string Authority { get; set; } = "GS1";
        // Il valore vero e proprio (es. 02099550010)
        [XmlText]
        public string Value { get; set; }
    }
    // Dettagli identificativi del documento
    public class SbdhDocId
    {
        [XmlElement("Standard", Namespace = "http://www.unece.org/cefact/namespaces/StandardBusinessDocumentHeader")]
        public string Standard { get; set; } = "EPCglobal";

        [XmlElement("TypeVersion", Namespace = "http://www.unece.org/cefact/namespaces/StandardBusinessDocumentHeader")]
        public string TypeVersion { get; set; } = "1.0";

        // ID Univoco del messaggio (Guid)
        [XmlElement("InstanceIdentifier", Namespace = "http://www.unece.org/cefact/namespaces/StandardBusinessDocumentHeader")]
        public string InstanceIdentifier { get; set; }

        // Tipo di contenuto (sempre "Events")
        [XmlElement("Type", Namespace = "http://www.unece.org/cefact/namespaces/StandardBusinessDocumentHeader")]
        public string Type { get; set; } = "Events";

        [XmlElement("CreationDateAndTime", Namespace = "http://www.unece.org/cefact/namespaces/StandardBusinessDocumentHeader")]
        public string CreationDateAndTime { get; set; }
    }
    // Corpo del messaggio EPCIS
    public class EpcisBody
    {
        [XmlElement("EventList", Namespace = "")]
        public EventList EventList { get; set; }
    }
    // Lista degli eventi di tracciabilità
    public class EventList
    {
        [XmlElement("ObjectEvent", typeof(ObjectEvent), Namespace = "")]
        // Polimorfismo: La lista può contenere sia ObjectEvent che TransformationEvent
        [XmlElement("TransformationEvent", typeof(TransformationEvent), Namespace = "")]
        public List<object> Events { get; set; } = new List<object>();
    }

    //// Classe base con le proprietà comuni a tutti gli eventi (Object e Transformation)
    //// Serve per non ripetere codice
    //public class EpcisEventBase
    //{
    //    // Quando è avvenuto il fatto (es. Data Semina)
    //    public string eventTime { get; set; }
    //    // Fuso orario (+01:00)
    //    [XmlElement("eventTimeZoneOffset")]
    //    public string eventTimeZoneOffset { get; set; }
    //    [XmlIgnore]
    //    public bool eventTimeZoneOffsetSpecified => !string.IsNullOrEmpty(eventTimeZoneOffset);
    //    // Cosa sta succedendo (business step): urn:epcglobal:cbv:bizstep:commissioning (DDT), planting, harvesting
    //    public string bizStep { get; set; }
    //    // Stato della merce: available, active, in_progress
    //    public string disposition { get; set; }

    //    // Estensioni custom "avex" (Appezzamenti, CUAA, ecc.)
    //    [XmlElement("extension")]
    //    public AvexExtension Extension { get; set; }
    //}

    //// Evento "Oggetto": Usato quando muovi o osservi oggetti esistenti (es. DDT Acquisto, Semina)
    //public class ObjectEvent : EpcisEventBase
    //{
    //    // Azione: ADD (Aggiungo merce, es. DDT) o OBSERVE (Guardo merce, es. Semina)
    //    public string action { get; set; }
    //    // Lista dei prodotti coinvolti (es. 200 piante Lotto A, 300 piante Lotto B)
    //    [XmlArray("quantityList")]
    //    [XmlArrayItem("quantityElement")]
    //    public List<QuantityElement> QuantityList { get; set; }
    //}

    //// Evento "Trasformazione": Usato per la Raccolta (Input: Piante -> Output: Frutti)
    //public class TransformationEvent : EpcisEventBase
    //{
    //    // Nota: TransformationEvent non ha "action" perché è implicita (trasforma input in output)

    //    // Cosa entra (es. le piante nel campo)
    //    [XmlArray("inputQuantityList")]
    //    [XmlArrayItem("quantityElement")]
    //    public List<QuantityElement> InputQuantityList { get; set; }

    //    // Cosa esce (es. l'uva raccolta)
    //    [XmlArray("outputQuantityList")]
    //    [XmlArrayItem("quantityElement")]
    //    public List<QuantityElement> OutputQuantityList { get; set; }
    //}

    // --- FIX ORDINE NODI: CLASSE BASE ELIMINATA ---
    // spacchettato la classe base per definire l'ordine perfetto dei campi.

    public class ObjectEvent
    {
        [XmlElement("eventTime", Namespace = "")]
        public string eventTime { get; set; }

        // FIX 2: Aggiunto Offset obbligatorio
        [XmlElement("eventTimeZoneOffset", Namespace = "")]
        public string eventTimeZoneOffset { get; set; } = "+00:00";

        [XmlElement("action", Namespace = "")]
        public string action { get; set; }

        [XmlElement("bizStep", Namespace = "")]
        public string bizStep { get; set; }

        [XmlElement("disposition", Namespace = "")]
        public string disposition { get; set; }

        [XmlArray("quantityList", Namespace = "")]
        [XmlArrayItem("quantityElement", Namespace = "")]
        public List<QuantityElement> QuantityList { get; set; }

        // FIX 1: Extension è l'ultimissima proprietà della classe
        [XmlElement("extension", Namespace = "")]
        public AvexExtension Extension { get; set; }
    }

    public class TransformationEvent
    {
        [XmlElement("eventTime", Namespace = "")]
        public string eventTime { get; set; }

        // FIX 2: Aggiunto Offset obbligatorio
        [XmlElement("eventTimeZoneOffset", Namespace = "")]
        public string eventTimeZoneOffset { get; set; } = "+00:00";

        [XmlArray("inputQuantityList", Namespace = "")]
        [XmlArrayItem("quantityElement", Namespace = "")]
        public List<QuantityElement> InputQuantityList { get; set; }

        [XmlArray("outputQuantityList", Namespace = "")]
        [XmlArrayItem("quantityElement", Namespace = "")]
        public List<QuantityElement> OutputQuantityList { get; set; }

        [XmlElement("bizStep", Namespace = "")]
        public string bizStep { get; set; }

        [XmlElement("disposition", Namespace = "")]
        public string disposition { get; set; }

        // FIX 1: Extension è l'ultimissima proprietà della classe
        [XmlElement("extension", Namespace = "")]
        public AvexExtension Extension { get; set; }
    }

    // Elemento Quantità: descrive UN lotto di prodotto
    public class QuantityElement
    {
        // Codice EPC (es. urn:epc:class:lsku:10_CODICE.LOTTO)
        public string epcClass { get; set; }
        // Quantità numerica come stringa (per formattazione precisa 200.00)
        public string quantity { get; set; }
        // Unità di misura: EA (Pezzi), KGM (Chilogrammi)
        public string uom { get; set; }
    }

    // Estensione Custom "Avex": Contiene i dati extra richiesti da Antares
    public class AvexExtension
    {
        // P.IVA o CUAA dell'azienda agricola
        [XmlElement("companyInput", Namespace = "http://avex.org/ns")]
        public string CompanyInput { get; set; }

        // Lista degli appezzamenti (Plots) coinvolti nell'operazione
        [XmlArray("plots", Namespace = "http://avex.org/ns")]
        [XmlArrayItem("plot", Namespace = "http://avex.org/ns")]
        public List<AvexPlot> Plots { get; set; }

        // Nome leggibile dell'evento (es. "Raccolta")
        [XmlElement("eventTypeName", Namespace = "http://avex.org/ns")]
        public string EventTypeName { get; set; }
        // Descrizione dettagliata
        [XmlElement("eventTypeDescription", Namespace = "http://avex.org/ns")]
        public string EventTypeDescription { get; set; }
    }

    // Dettaglio del singolo Appezzamento (Plot)
    public class AvexPlot
    {
        // Chiave univoca appezzamento (formato Antares: CUAA_CODICE...)
        [XmlElement("plotKey", Namespace = "http://avex.org/ns")]
        public string PlotKey { get; set; }

        // Codice ISTAT Comune
        [XmlElement("plotIstat", Namespace = "http://avex.org/ns")]
        public string PlotIstat { get; set; }

        [XmlElement("plotAddress", Namespace = "http://avex.org/ns")]
        public string PlotAddress { get; set; }

        [XmlElement("plotProv", Namespace = "http://avex.org/ns")]
        public string PlotProv { get; set; }

        [XmlElement("plotComune", Namespace = "http://avex.org/ns")]
        public string PlotComune { get; set; }

        // Sistema di riferimento coordinate (sempre 4326 = WGS84)
        [XmlElement("plotPolygonSrs", Namespace = "http://avex.org/ns")]
        public string PlotPolygonSrs { get; set; }

        // Geometria del poligono in formato WKT (Well Known Text)
        [XmlElement("plotPolygonWkt", Namespace = "http://avex.org/ns")]
        public string PlotPolygonWkt { get; set; }
    }
}