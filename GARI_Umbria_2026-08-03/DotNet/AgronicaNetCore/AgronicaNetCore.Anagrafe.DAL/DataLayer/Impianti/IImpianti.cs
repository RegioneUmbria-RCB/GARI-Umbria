using AgronicaCoreDTOStd.InData.Anagrafica;
using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.metaschema;
using AgronicaCoreModelsSTD.metaschema.utilizzi;
using AgronicaNetCore.Base.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.Impianti
{
    public interface IImpianti
    {
        Task<DataTable?> GetPianoColturalePerConfrontoCatastoAsync(string partitaIva, DateTime dataInizio, DateTime dataFine, AgronicaCoreParametriServer objParametriServer, bool origine = true, bool ShowCatasto = false, bool showVarieta = false);
        Task<DataSet> GetExistsContributiACAAsync(AgronicaCoreParametriServer objParametriServer);


        /// <summary>
        /// Legge i dati relativi all'impianto e i relativi esercizi associati.
        /// </summary>
        Task<DataTable?> LeggiAsync(AgronicaCoreParametriServer objParametriServer, string piva = "", int saCod = 0, int appezza = 0, int idReg = 0, int progCod = -1);

        /// <summary>
        /// Legge i dati relativi all'impianto e ai suoi codici.
        /// </summary>
        Task<DataTable?> LeggiConCodiciAsync(AgronicaCoreParametriServer objParametriServer, string piva = "", int saCod = 0, int appezza = 0, int idReg = 0, int culCod = 0, int progCod = -1, int idCod = 0, string valCod = "");
        /// <summary>
        /// Mette in left join le tabelle Reg_Impianti, Cultivar, SpecieVegetali e GruppoVegetale.
        /// </summary>
        Task<DataTable?> LeggiInfoVarietaAsync(string piva, int saCod, int appezza, int idReg, AgronicaCoreParametriServer objParametriServer);

        /// <summary>
        /// Mette in left join le tabelle Reg_Impianti, Cultivar, SpecieVegetali e GruppoVegetale.
        /// </summary>
        /// <param name="listaDiPiva"></param>
        /// <param name="listaDiSaCod"></param>
        /// <param name="listaDiAppezza"></param>
        /// <param name="listaDiIdReg"></param>
        /// <param name="objParametriServer"></param>
        /// <returns></returns>
        Task<DataTable> LeggiInfoVarietaAsync(List<(string, int, int, int)> chiaviImpianto, AgronicaCoreParametriServer objParametriServer);
        /// <summary>
        /// Restituisce le specie vegetali distinte (Veg_Cod, Veg_Des) presenti nei
        /// <c>Reg_Impianti</c> delle aziende fornite.
        /// </summary>
        Task<DataTable> LeggiColturexPivaAsync(IEnumerable<string> pivas, AgronicaCoreParametriServer objParametriServer, DateTime? dataInizio = null, bool soloAttiviOggi = false, bool modalitaDemetra = false);

        /// <summary>
        /// Legge i codici progetto associati agli impianti forniti, validi alla data fornita
        /// </summary>
        /// <param name="chiaviImpianto"></param>
        /// <param name="objParametriServer"></param>
        /// <returns></returns>
        Task<DataTable> LeggiCodiciProgettoAsync(List<(string, int, int, int, DateTime)> chiaviImpianto, AgronicaCoreParametriServer objParametriServer);
    }
}
