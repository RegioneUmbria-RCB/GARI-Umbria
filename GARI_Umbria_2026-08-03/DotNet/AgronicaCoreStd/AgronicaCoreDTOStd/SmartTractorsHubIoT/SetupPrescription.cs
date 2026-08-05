using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.SmartTractors_HubIoT
{
    public class SetupPrescription
    {
        public DateTime workOrderDate { get; set; }
        public string operationType { get; set; }
        public string rateName { get; set; }
        public string rateUnit { get; set; }
        public Client client { get; set; }
        public Farm farm { get; set; }
        public Field field { get; set; }
        public Boundary boundary { get; set; }
        public GuidanceLine guidanceLine { get; set; }
        public Fertilizer fertilizer { get; set; }
        public Chemical chemical { get; set; }
        public TankMix tankMix { get; set; }
        public Variety variety { get; set; }
    }

    public class Client : EntityBase
    {

    }

    public class Farm : EntityBase
    {
        private string _clientRef = "";
        public string clientId
        {
            get
            {
                return _clientRef;
            }

        }

        public Farm(Client cli)
        {
            _clientRef = cli.id;
        }
    }

    public class Field : EntityBase
    {
        private string _clientRef = "";
        public string clientId
        {
            get
            {
                return _clientRef;
            }

        }

        private string _farmRef = "";
        public string farmId
        {
            get
            {
                return _farmRef;
            }
        }

        public Field(Client cli, Farm frm)
        {
            _clientRef = cli.id;
            _farmRef = frm.id;
        }
    }

    public class Boundary : EntityBase
    {
        public string sourceType { get; set; } = "External";
        public bool active { get; set; } = true;
        public bool archived { get; set; } = false;
        public bool irrigated { get; set; } = false;
        public List<Polygon> multipolygons { get; set; } = null;

        private string _fieldRef = "";
        public string fieldId
        {
            get
            {
                return _fieldRef;
            }
        }

        public Boundary(Field fld)
        {
            _fieldRef = fld.id;
        }
    }

    public class GuidanceLine : EntityBase
    {
        private string _fieldRef = "";
        public string FieldID
        {
            get
            {
                return _fieldRef;
            }
        }

        public Point APoint { get; set; }
        public Point BPoint { get; set; }
        public Measure northShift { get; set; }
        public Measure eastShift { get; set; }
        public decimal tramOffset { get; set; }
        public decimal tramSpacing { get; set; }
    }

    public class Fertilizer : EntityProductBase
    {

    }
    public class Chemical : EntityProductBase
    {

    }
    public class TankMix : EntityBaseClassification
    {

    }
    public class Variety : EntityBase
    {
        public string cropName { get; set; } = "";
        public string companyName { get; set; } = "";
    }



    public class EntityBase
    {
        public string id { get; set; } = "";
        public string name { get; set; } = "";

    }

    public class EntityBaseClassification : EntityBase
    {
        public string materialClassification { get; set; } = "";
    }

    public class EntityProductBase : EntityBaseClassification
    {
        public string companyName { get; set; } = "";
        public string type { get; set; } = "";
        public bool carrier { get; set; } = false;
    }

    public class Polygon
    {
        public List<Ring> rings { get; set; } = new List<Ring>();
    }

    public class Ring
    {
        public List<Point> points { get; set; } = new List<Point>();
        public string type { get; set; } = "exterior";
        public bool passable { get; set; } = true;
    }

    public class Point
    {
        public decimal lat { get; set; }
        public decimal lon { get; set; }
    }

    public class Measure
    {
        public decimal value { get; set; } 
        public string unit { get; set; }
    }

}
