using AgronicaCoreDTOStd.InData.Agea;
using System;
using System.Collections.Generic;

namespace AgronicaCoreDataSTD.InData.Agea
{ 

    public class OperationWithPlot : Plot_Key, IOperation
    {
        public string operationId { get; set; }

        public OperationWithPlot(string _operationId, string _islandId, string _plotId, string _pcgId, string _plantationId, string _codiBarrScheVali) : base( _islandId, _plotId, _pcgId, _plantationId, _codiBarrScheVali)
        {
            operationId = _operationId;
            islandId = _islandId;
            plotId = _plotId;
            pcgId = _pcgId;
            plantationId = _plantationId;
            codiBarrScheVali = _codiBarrScheVali;
        }
    }

    public class OperationWithWarehouse : Warehouse_Key, IOperation
    {

        public string operationId { get; set; }

        public string eventDate { get; set; }

        public string productRegistrationNumber { get; set; }

        public decimal quantity { get; set; }

        public string measureUnit { get; set; }

        public string adversityName { get; set; }

        public List<Worker> Workers { get; set; }

        public OperationWithWarehouse(string _operationId, string _address, string _municipality, string _foglio, string _particella, string _subalterno, string _georeferencing, decimal _capacity) : base(_address, _municipality, _foglio, _particella, _subalterno, _georeferencing, _capacity)
        {
            operationId = _operationId;
            address = _address;
            municipality = _municipality;
            foglio = _foglio;
            particella = _particella;
            subalterno = _subalterno;
            georeferencing = _georeferencing;
            capacity = _capacity;
            this.Workers = new List<Worker>();
        }
    }

    interface IOperation
    {
        /// <summary>
        /// Chiava composta PIVA_idAgenda
        /// </summary>
        string operationId { get; set; }
    }

}
