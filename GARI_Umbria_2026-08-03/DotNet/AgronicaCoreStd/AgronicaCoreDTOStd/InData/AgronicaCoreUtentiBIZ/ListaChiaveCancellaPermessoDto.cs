using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.AgronicaCoreUtentiBIZ
{
    public class ListaChiaveCancellaPermessoDto
    {
        public List<ChiaveCancellaPermessoDto> permessi { get; set; }
    }
    public class ChiaveCancellaPermessoDto
    {
        public int Gruppi_Utente_cod { get; set; }
        public int Id_Gruppo_Merce { get; set; }
        public string Piva { get; set; }
    }
}
