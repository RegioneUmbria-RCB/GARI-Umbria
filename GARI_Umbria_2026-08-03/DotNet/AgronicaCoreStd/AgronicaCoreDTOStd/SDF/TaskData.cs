using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.SDF
{
    public class TaskData
    {
        public int id { get; set; }
        public double cO2 { get; set; }
        public double fuelConsumption { get; set; }
        public string shapefileCosUrl { get; set; }
        public TaskDataCompany company { get; set; }
        public TaskDataVehicle vehicle { get; set; }
        public DateTime taskDataEndDateTime { get; set; }
    }
}
