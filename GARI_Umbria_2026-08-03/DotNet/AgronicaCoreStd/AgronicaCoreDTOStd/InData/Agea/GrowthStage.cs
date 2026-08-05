using System;
using Newtonsoft.Json;

namespace AgronicaCoreDTOStd.InData.Agea
{
    /// <summary>
    /// Corrisponde ai Rilievi Fasi Fenologiche
    /// </summary>
    public class GrowthStage : Plot_Key
    {

        /// <summary>
        /// Proprietà introdotta per i controlli da non esportare ad AGEA
        /// </summary>
        [JsonIgnore]
        public string bbch0DescriptionToCheck { get; set; }

        public string bbch0StartDate { get; set; } = new DateTime(1900,01,01).ToString("yyyy-MM-dd");

        /// <summary>
        /// Proprietà introdotta per i controlli da non esportare ad AGEA
        /// </summary>
        [JsonIgnore]
        public DateTime? bbch0StartDateToCheck { get; set; }

        /// <summary>
        /// Chiava composta PIVA_idAgenda
        /// </summary>
        public string bbch0StartDate_operationId { get; set; } = "";

        /// <summary>
        /// Proprietà introdotta per i controlli da non esportare ad AGEA
        /// </summary>
        [JsonIgnore]
        public string bbch0StartDate_operationDescriptionToCheck { get; set; }

        public string bbch0EndDate { get; set; } = new DateTime(1900, 01, 01).ToString("yyyy-MM-dd");

        [JsonIgnore]
        public DateTime? bbch0EndDateToCheck { get; set; }

        /// <summary>
        /// Chiava composta PIVA_idAgenda
        /// </summary>
        public string bbch0EndDate_operationId { get; set; } = "";

        /// <summary>
        /// Proprietà introdotta per i controlli da non esportare ad AGEA
        /// </summary>
        [JsonIgnore]
        public string bbch0EndDate_operationDescriptionToCheck { get; set; }

        /// <summary>
        /// Proprietà introdotta per i controlli da non esportare ad AGEA
        /// </summary>
        [JsonIgnore]
        public string bbch1DescriptionToCheck { get; set; }

        public string bbch1StartDate { get; set; } = new DateTime(1900, 01, 01).ToString("yyyy-MM-dd");

        [JsonIgnore]
        public DateTime? bbch1StartDateToCheck { get; set; }

        /// <summary>
        /// Chiava composta PIVA_idAgenda
        /// </summary>
        public string bbch1StartDate_operationId { get; set; } = "";

        /// <summary>
        /// Proprietà introdotta per i controlli da non esportare ad AGEA
        /// </summary>
        [JsonIgnore]
        public string bbch1StartDate_operationDescriptionToCheck { get; set; }

        public string bbch1EndDate { get; set; } = new DateTime(1900, 01, 01).ToString("yyyy-MM-dd");

        [JsonIgnore]
        public DateTime? bbch1EndDateToCheck { get; set; }

        /// <summary>
        /// Chiava composta PIVA_idAgenda
        /// </summary>
        public string bbch1EndDate_operationId { get; set; } = "";

        /// <summary>
        /// Proprietà introdotta per i controlli da non esportare ad AGEA
        /// </summary>
        [JsonIgnore]
        public string bbch1EndDate_operationDescriptionToCheck { get; set; }

        /// <summary>
        /// Proprietà introdotta per i controlli da non esportare ad AGEA
        /// </summary>
        [JsonIgnore]
        public string bbch2DescriptionToCheck { get; set; }

        public string bbch2StartDate { get; set; } = new DateTime(1900, 01, 01).ToString("yyyy-MM-dd");

        [JsonIgnore]
        public DateTime? bbch2StartDateToCheck { get; set; }

        /// <summary>
        /// Chiava composta PIVA_idAgenda
        /// </summary>
        public string bbch2StartDate_operationId { get; set; } = "";

        /// <summary>
        /// Proprietà introdotta per i controlli da non esportare ad AGEA
        /// </summary>
        [JsonIgnore]
        public string bbch2StartDate_operationDescriptionToCheck { get; set; }

        public string bbch2EndDate { get; set; } = new DateTime(1900, 01, 01).ToString("yyyy-MM-dd");

        [JsonIgnore]
        public DateTime? bbch2EndDateToCheck { get; set; }

        /// <summary>
        /// Chiava composta PIVA_idAgenda
        /// </summary>
        public string bbch2EndDate_operationId { get; set; } = "";

        /// <summary>
        /// Proprietà introdotta per i controlli da non esportare ad AGEA
        /// </summary>
        [JsonIgnore]
        public string bbch2EndDate_operationDescriptionToCheck { get; set; }

        /// <summary>
        /// Proprietà introdotta per i controlli da non esportare ad AGEA
        /// </summary>
        [JsonIgnore]
        public string bbch3DescriptionToCheck { get; set; }

        public string bbch3StartDate { get; set; } = new DateTime(1900, 01, 01).ToString("yyyy-MM-dd");

        [JsonIgnore]
        public DateTime? bbch3StartDateToCheck { get; set; }

        /// <summary>
        /// Chiava composta PIVA_idAgenda
        /// </summary>
        public string bbch3StartDate_operationId { get; set; } = "";

        /// <summary>
        /// Proprietà introdotta per i controlli da non esportare ad AGEA
        /// </summary>
        [JsonIgnore]
        public string bbch3StartDate_operationDescriptionToCheck { get; set; }

        public string bbch3EndDate { get; set; } = new DateTime(1900, 01, 01).ToString("yyyy-MM-dd");

        [JsonIgnore]
        public DateTime? bbch3EndDateToCheck { get; set; }

        /// <summary>
        /// Chiava composta PIVA_idAgenda
        /// </summary>
        public string bbch3EndDate_operationId { get; set; } = "";

        /// <summary>
        /// Proprietà introdotta per i controlli da non esportare ad AGEA
        /// </summary>
        [JsonIgnore]
        public string bbch3EndDate_operationDescriptionToCheck { get; set; }

        /// <summary>
        /// Proprietà introdotta per i controlli da non esportare ad AGEA
        /// </summary>
        [JsonIgnore]
        public string bbch4DescriptionToCheck { get; set; }

        public string bbch4StartDate { get; set; } = new DateTime(1900, 01, 01).ToString("yyyy-MM-dd");

        [JsonIgnore]
        public DateTime? bbch4StartDateToCheck { get; set; }

        /// <summary>
        /// Chiava composta PIVA_idAgenda
        /// </summary>
        public string bbch4StartDate_operationId { get; set; } = "";

        /// <summary>
        /// Proprietà introdotta per i controlli da non esportare ad AGEA
        /// </summary>
        [JsonIgnore]
        public string bbch4StartDate_operationDescriptionToCheck { get; set; }

        public string bbch4EndDate { get; set; } = new DateTime(1900, 01, 01).ToString("yyyy-MM-dd");

        [JsonIgnore]
        public DateTime? bbch4EndDateToCheck { get; set; }

        /// <summary>
        /// Chiava composta PIVA_idAgenda
        /// </summary>
        public string bbch4EndDate_operationId { get; set; } = "";

        /// <summary>
        /// Proprietà introdotta per i controlli da non esportare ad AGEA
        /// </summary>
        [JsonIgnore]
        public string bbch4EndDate_operationDescriptionToCheck { get; set; }

        /// <summary>
        /// Proprietà introdotta per i controlli da non esportare ad AGEA
        /// </summary>
        [JsonIgnore]
        public string bbch5DescriptionToCheck { get; set; }

        public string bbch5StartDate { get; set; } = new DateTime(1900, 01, 01).ToString("yyyy-MM-dd");

        [JsonIgnore]
        public DateTime? bbch5StartDateToCheck { get; set; }

        /// <summary>
        /// Chiava composta PIVA_idAgenda
        /// </summary>
        public string bbch5StartDate_operationId { get; set; } = "";

        /// <summary>
        /// Proprietà introdotta per i controlli da non esportare ad AGEA
        /// </summary>
        [JsonIgnore]
        public string bbch5StartDate_operationDescriptionToCheck { get; set; }

        public string bbch5EndDate { get; set; } = new DateTime(1900, 01, 01).ToString("yyyy-MM-dd");

        [JsonIgnore]
        public DateTime? bbch5EndDateToCheck { get; set; }

        /// <summary>
        /// Chiava composta PIVA_idAgenda
        /// </summary>
        public string bbch5EndDate_operationId { get; set; } = "";

        /// <summary>
        /// Proprietà introdotta per i controlli da non esportare ad AGEA
        /// </summary>
        [JsonIgnore]
        public string bbch5EndDate_operationDescriptionToCheck { get; set; }

        /// <summary>
        /// Proprietà introdotta per i controlli da non esportare ad AGEA
        /// </summary>
        [JsonIgnore]
        public string bbch6DescriptionToCheck { get; set; }

        public string bbch6StartDate { get; set; } = new DateTime(1900, 01, 01).ToString("yyyy-MM-dd");

        [JsonIgnore]
        public DateTime? bbch6StartDateToCheck { get; set; }

        /// <summary>
        /// Chiava composta PIVA_idAgenda
        /// </summary>
        public string bbch6StartDate_operationId { get; set; } = "";

        /// <summary>
        /// Proprietà introdotta per i controlli da non esportare ad AGEA
        /// </summary>
        [JsonIgnore]
        public string bbch6StartDate_operationDescriptionToCheck { get; set; }

        public string bbch6EndDate { get; set; } = new DateTime(1900, 01, 01).ToString("yyyy-MM-dd");

        [JsonIgnore]
        public DateTime? bbch6EndDateToCheck { get; set; }

        /// <summary>
        /// Chiava composta PIVA_idAgenda
        /// </summary>
        public string bbch6EndDate_operationId { get; set; } = "";

        /// <summary>
        /// Proprietà introdotta per i controlli da non esportare ad AGEA
        /// </summary>
        [JsonIgnore]
        public string bbch6EndDate_operationDescriptionToCheck { get; set; }

        /// <summary>
        /// Proprietà introdotta per i controlli da non esportare ad AGEA
        /// </summary>
        [JsonIgnore]
        public string bbch7DescriptionToCheck { get; set; }

        public string bbch7StartDate { get; set; } = new DateTime(1900, 01, 01).ToString("yyyy-MM-dd");

        [JsonIgnore]
        public DateTime? bbch7StartDateToCheck { get; set; }

        /// <summary>
        /// Chiava composta PIVA_idAgenda
        /// </summary>
        public string bbch7StartDate_operationId { get; set; } = "";

        /// <summary>
        /// Proprietà introdotta per i controlli da non esportare ad AGEA
        /// </summary>
        [JsonIgnore]
        public string bbch7StartDate_operationDescriptionToCheck { get; set; }

        public string bbch7EndDate { get; set; } = new DateTime(1900, 01, 01).ToString("yyyy-MM-dd");

        [JsonIgnore]
        public DateTime? bbch7EndDateToCheck { get; set; }

        /// <summary>
        /// Chiava composta PIVA_idAgenda
        /// </summary>
        public string bbch7EndDate_operationId { get; set; } = "";

        /// <summary>
        /// Proprietà introdotta per i controlli da non esportare ad AGEA
        /// </summary>
        [JsonIgnore]
        public string bbch7EndDate_operationDescriptionToCheck { get; set; }

        /// <summary>
        /// Proprietà introdotta per i controlli da non esportare ad AGEA
        /// </summary>
        [JsonIgnore]
        public string bbch8DescriptionToCheck { get; set; }

        public string bbch8StartDate { get; set; } = new DateTime(1900, 01, 01).ToString("yyyy-MM-dd");

        [JsonIgnore]
        public DateTime? bbch8StartDateToCheck { get; set; }

        /// <summary>
        /// Chiava composta PIVA_idAgenda
        /// </summary>
        public string bbch8StartDate_operationId { get; set; } = "";

        /// <summary>
        /// Proprietà introdotta per i controlli da non esportare ad AGEA
        /// </summary>
        [JsonIgnore]
        public string bbch8StartDate_operationDescriptionToCheck { get; set; }

        public string bbch8EndDate { get; set; } = new DateTime(1900, 01, 01).ToString("yyyy-MM-dd");

        [JsonIgnore]
        public DateTime? bbch8EndDateToCheck { get; set; }

        /// <summary>
        /// Chiava composta PIVA_idAgenda
        /// </summary>
        public string bbch8EndDate_operationId { get; set; } = "";

        /// <summary>
        /// Proprietà introdotta per i controlli da non esportare ad AGEA
        /// </summary>
        [JsonIgnore]
        public string bbch8EndDate_operationDescriptionToCheck { get; set; }

        /// <summary>
        /// Proprietà introdotta per i controlli da non esportare ad AGEA
        /// </summary>
        [JsonIgnore]
        public string bbch9DescriptionToCheck { get; set; }

        public string bbch9StartDate { get; set; } = new DateTime(1900, 01, 01).ToString("yyyy-MM-dd");

        [JsonIgnore]
        public DateTime? bbch9StartDateToCheck { get; set; }

        /// <summary>
        /// Chiava composta PIVA_idAgenda
        /// </summary>
        public string bbch9StartDate_operationId { get; set; } = "";

        /// <summary>
        /// Proprietà introdotta per i controlli da non esportare ad AGEA
        /// </summary>
        [JsonIgnore]
        public string bbch9StartDate_operationDescriptionToCheck { get; set; }

        public string bbch9EndDate { get; set; } = new DateTime(1900, 01, 01).ToString("yyyy-MM-dd");

        [JsonIgnore]
        public DateTime? bbch9EndDateToCheck { get; set; }

        /// <summary>
        /// Chiava composta PIVA_idAgenda
        /// </summary>
        public string bbch9EndDate_operationId { get; set; } = "";

        /// <summary>
        /// Proprietà introdotta per i controlli da non esportare ad AGEA
        /// </summary>
        [JsonIgnore]
        public string bbch9EndDate_operationDescriptionToCheck { get; set; }

        public GrowthStage(string _islandId, string _plotId, string _pcgId, string _plantationId,string _codiBarrScheVali) : base(_islandId, _plotId, _pcgId, _plantationId, _codiBarrScheVali)
        {
            islandId = _islandId;
            plotId = _plotId;
            pcgId = _pcgId;
            plantationId = _plantationId;
            codiBarrScheVali = _codiBarrScheVali;
        }
    }
}
