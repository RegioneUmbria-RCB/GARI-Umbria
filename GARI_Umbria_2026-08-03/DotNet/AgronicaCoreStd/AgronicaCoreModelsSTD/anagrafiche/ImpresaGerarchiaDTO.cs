using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.anagrafiche
{
    public interface IImpresaDto
    {
        string piva { get; set; }
        string rag_soc { get; set; }
        int Sa_Cod { get; set; }
        string Sa_Nome { get; set; }
        string partitaIvaReale { get; set; }
    }

    public class ImpresaDto: IImpresaDto
    {
        public string piva { get; set; }
        public string rag_soc { get; set; }
        public int Sa_Cod { get; set; }
        public string Sa_Nome { get; set; }
        public string partitaIvaReale { get; set; }
        public ImpresaDto() { }

        public ImpresaDto(string Piva)
        {
            piva = Piva;
        }

        public override bool Equals(object obj)
        {
            return obj is ImpresaDto dto &&
                   piva == dto.piva &&
                   Sa_Cod == dto.Sa_Cod;
        }

        public override int GetHashCode()
        {
            return (this.piva + this.Sa_Cod.ToString()).GetHashCode();
        }
    }

    public class ImpresaGerarchiaBaseDto : ImpresaDto
    {
        public string Padre { get; set; }
        public bool IsFoglia { get; set; }
        /// <summary>
        /// Indica il tipo di impresa a cui si fa riferiemnto.
        /// Impresa = 1, Cooperativa = 2, Consorzio = 3, OrganizzazioneProduttore = 4
        /// </summary>
        public int TipoImpresaGerarchia { get; set; }
    }

    public class ImpresaGerarchiaCuaaDto : ImpresaGerarchiaBaseDto
    {
        public string Cuaa { get; set; }
    }

    //public class ImpresaGerarchiaDto : ImpresaDto
    //{
    //    public ImpresaGerarchiaDto Padre { get; set; }
    //    public List<ImpresaGerarchiaDto> Figlie { get; set; } = new List<ImpresaGerarchiaDto>();

    //    public bool isFoglia
    //    {
    //        get { return this.Figlie.Count > 0; }
    //    }

    //    public bool isRadice
    //    {
    //        get { return this.Padre == null; }
    //    }

    //    /// <summary>
    //    /// Aggiunge l'impresa specificata alle figlie e crea il collegamento inverso (assegna padre).
    //    /// </summary>
    //    /// <param name="figlia"></param>
    //    /// <returns></returns>
    //    public bool addFiglia(ImpresaGerarchiaDto figlia)
    //    {
    //        if (this.Figlie.Find(i => i.piva == figlia.piva) != null)
    //        {
    //            return false;
    //        }
    //        this.Figlie.Add(figlia);
    //        if (figlia.Padre == null)
    //        {
    //            figlia.Padre = this;
    //        }
    //        return true;
    //    }

    //}
}
