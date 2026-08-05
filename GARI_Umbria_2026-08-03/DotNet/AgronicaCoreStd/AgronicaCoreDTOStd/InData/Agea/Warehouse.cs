
using System.Collections.Generic;

namespace AgronicaCoreDTOStd.InData.Agea
{
    public class Warehouse: Warehouse_Key
    {
        public string description { get; set; }
        public List<WarehouseStockPesticide> pesticides { get; set; }
        public List<WarehouseStockChemicalFertilizer> chemicalFertilizers { get; set; }
        public List<WarehouseStockOrganicFertilizer> organicFertilizers { get; set; }

        public Warehouse(string address, string municipality, string foglio, string particella, string subalterno, string georeferencing, decimal capacity, string description): base( address,  municipality,  foglio,  particella,  subalterno,  georeferencing, capacity)
        {
            this.description = description;
            pesticides = new List<WarehouseStockPesticide>();
            chemicalFertilizers = new List<WarehouseStockChemicalFertilizer>();
            organicFertilizers = new List<WarehouseStockOrganicFertilizer>();
        }
    }

    public class Warehouse_Key
    {
        public string address { get; set; }
        public string municipality { get; set; }
        public string foglio { get; set; }
        public string particella { get; set; }
        public string subalterno { get; set; }
        public string georeferencing { get; set; }
        public decimal capacity { get; set; }

        public Warehouse_Key(string address, string municipality, string foglio, string particella, string subalterno, string georeferencing, decimal capacity)
        {
            this.address = address;
            this.municipality = municipality;
            this.foglio = foglio;
            this.particella = particella;
            this.subalterno = subalterno;
            this.georeferencing = georeferencing;
            this.capacity = capacity;
        }
    }
}
