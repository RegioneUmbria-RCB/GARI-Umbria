using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Text;
using AgronicaCoreDTOStd.InData.Zoo;

namespace InData.Zoo
{
    public class LeggiGiacenzeZooFirstSommDto : LeggiGiacenzeZooDto
    {
        [Required]
        [DisplayName("Id della nuova Prescrizione GIAS")]
        public int Id_Pres { get; set; }
    }
}
