


using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace AgronicaCoreModelsSTD.DataExchange.AntaresTrace
{
    [XmlRoot("itsXml")]
    public class ItsXmlMasterData
    {
        [XmlAttribute("interface")]
        public string Interface { get; set; } = "MaterialMaster";

        [XmlElement("header")]
        public Header Header { get; set; }

        [XmlElement("products")]
        public Products Products { get; set; }
    }

    public class Header
    {
        [XmlElement("transactionTimestamp")]
        public string TransactionTimestamp { get; set; }

        [XmlElement("transactionTimeZone")]
        public string TransactionTimeZone { get; set; }

        [XmlElement("interfaceRevision")]
        public string InterfaceRevision { get; set; }

        [XmlElement("messageId")]
        public string MessageId { get; set; }

        [XmlElement("siteCode")]
        public string SiteCode { get; set; }

        [XmlElement("additionalDetails")]
        public string AdditionalDetails { get; set; }
    }

    public class Products
    {
        [XmlAttribute("listMode")]
        public string ListMode { get; set; } = "single";

        [XmlElement("product")]
        public List<Product> ProductList { get; set; } = new List<Product>();
    }

    public class Product
    {
        [XmlAttribute("active")]
        public bool Active { get; set; }

        [XmlElement("id")]
        public string Id { get; set; }

        [XmlElement("IsSerialized")]
        public int IsSerialized { get; set; }

        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Description")]
        public string Description { get; set; }

        [XmlElement("measureUnit")]
        public string MeasureUnit { get; set; }

        [XmlElement("codingRule")]
        public string CodingRule { get; set; }

        [XmlElement("productCode")]
        public string ProductCode { get; set; }

        [XmlElement("companyPrefix")]
        public string CompanyPrefix { get; set; }

        [XmlElement("additionalDetails")]
        public AdditionalDetails AdditionalDetails { get; set; }
    }

    public class AdditionalDetails
    {
        [XmlElement("detail")]
        public List<Detail> Details { get; set; } = new List<Detail>();
    }

    public class Detail
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("AI")]
        public string AI { get; set; }

        [XmlAttribute("numeric")]
        public int Numeric { get; set; }

        [XmlAttribute("note")]
        public string Note { get; set; }

        [XmlAttribute("mandatory")]
        public int Mandatory { get; set; }

        [XmlAttribute("UOM")]
        public string UOM { get; set; }

        [XmlAttribute("minValue")]
        public string MinValue { get; set; }

        [XmlAttribute("maxValue")]
        public string MaxValue { get; set; }

        [XmlText]
        public string Value { get; set; }
    }
}