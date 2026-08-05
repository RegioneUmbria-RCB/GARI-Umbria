using InData.Agea;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Agea
{
    public class Supply
    {
        public string farmDescription { get; set; }

        public string countryNotebookName { get; set; }

        public string countryNotebookDesc { get; set; }

        public int campaingYear { get; set; }

        public List<FarmingEvent> farmingEvents { get; set; }

        public List<PhytochemicalTreatment> phytochemicalTreatments { get; set; }

        public List<GrowthStage> growthStages { get; set; }

        public List<Irrigation> irrigations { get; set; }

        public List<ChemicalFertilization> chemicalFertilizations { get; set; }

        public List<OrganicFertilization> organicFertilizations { get; set; }

        public List<Plot> plotDescriptions { get; set; }

        public List<Worker> workers { get; set; }

        public List<EquipmentElement> equipmentList { get; set; }

        public List<Warehouse> warehouses { get; set; }

        public List<ProductTreatment> productTreatments { get; set; }

        public List<SeedTreatment> seedTreatments { get; set; }

        public Supply()
        {
            this.farmingEvents = new List<FarmingEvent>();
            this.phytochemicalTreatments = new List<PhytochemicalTreatment>();
            this.growthStages = new List<GrowthStage>();
            this.irrigations = new List<Irrigation>();
            this.chemicalFertilizations = new List<ChemicalFertilization> ();
            this.organicFertilizations = new List<OrganicFertilization> ();
            this.plotDescriptions = new List<Plot>();
            this.workers = new List<Worker>();
            this.equipmentList = new List<EquipmentElement>();
            this.warehouses = new List<Warehouse>();
            this.productTreatments = new List<ProductTreatment>();
            this.seedTreatments = new List<SeedTreatment>();
        }
    }
}
