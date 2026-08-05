using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.Esercizi
{
    public interface IEsercizi
    {
        Task<DataTable?> LeggiAsync(AgronicaCoreParametriServer objParametriServer, string piva = "", int saCod = 0, int appezza = 0, int idReg = 0, int progCod = -1, DateTime? inizio = null, DateTime? fine = null);
        Task<DataTable> LeggiEsercizioXIntegrazioneAttivita(string piva, int sacod, int appezza, int idReg, DateTime dataAttivita, AgronicaCoreParametriServer objParametriServer);
        Task<DataTable> LeggiEserciziApertixPivasAsync(List<string> pivas, AgronicaCoreParametriServer objParametriServer, bool dataAttuale = false);
        Task<DataTable> LeggiEserciziDSSNutrizioneAsync(string piva, int? saCod, int annoSolare, AgronicaCoreParametriServer objParametriServer, bool filtroVisibilitaUtente = false);
    }
}
