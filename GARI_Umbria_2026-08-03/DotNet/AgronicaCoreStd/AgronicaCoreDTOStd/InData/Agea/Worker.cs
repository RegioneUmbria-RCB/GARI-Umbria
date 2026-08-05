using AgronicaCoreDTOStd.InData.Demetra;
using AgronicaCoreModelsSTD.anagrafiche;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Transactions;

namespace AgronicaCoreDTOStd.InData.Agea
{
    public class Worker
    {
        public Worker() { }
        public Worker(RisorseUmane risorsaUmana, PersonType personType) 
        {
            switch (personType)
            {
                case PersonType.PersonaFisica:
                    fiscalCode = risorsaUmana.contatto.primaryKey.codice;
                    vatNumber = string.Empty;
                    workerType = WorkerType.Internal;
                    break;

                case PersonType.PersonaGiuridica:
                    fiscalCode = string.Empty;
                    vatNumber = risorsaUmana.contatto.primaryKey.codice;
                    workerType = WorkerType.External;
                    break;
                
                default:
                    break;
            }
        }

        public string fiscalCode { get; set; }

        public string vatNumber { get; set; }

        private string _workerType = WorkerType.Internal;

        public string workerType
        {
            get
            {
                return _workerType;
            }
            set
            {
                if (value == WorkerType.Internal || value == WorkerType.External)
                {
                    _workerType = value;
                }
                else
                {
                    throw new ArgumentException("Invalid value supplied");
                }
            }
        }
    }

    public class WorkerElement : Worker
    {
        public WorkerElement() : base() { }

        public WorkerElement(RisorseUmane risorsaUmana, PersonType personType, int anno) :base(risorsaUmana, personType)
        {
            switch (personType)
            {
                case PersonType.PersonaFisica:
                    workerName = risorsaUmana.contatto.nome;
                    workerSurname = risorsaUmana.contatto.cognome;
                    companyName = string.Empty;
                    break;

                case PersonType.PersonaGiuridica:
                    workerName = string.Empty;
                    workerSurname = string.Empty;
                    companyName = risorsaUmana.contatto.ragione_Sociale;
                    break;

                default:
                    break;
            }

            AgronicaCoreModelsSTD.documenti.Documento patentino = null;
            if(risorsaUmana.contatto.documenti != null )
            {
                // estraggo il patentino emesso nell'anno corrente ancora valido 
                patentino = risorsaUmana.contatto.documenti
                    .Where(d => d.Data_Rilascio.Year <= anno &&
                        d.Data_Scadenza.Year >= anno)
                    .OrderByDescending(d => d.Data_Rilascio)
                    .FirstOrDefault();

                if (patentino == null)
                    // estraggo il patentino emesso nell'anno successivo ancora valido 
                    patentino = risorsaUmana.contatto.documenti
                    .Where(d => d.Data_Rilascio.Year <= anno &&
                        d.Data_Scadenza.Year >= anno)
                    .OrderBy(d => d.Data_Rilascio)
                    .FirstOrDefault();
            }

            if (patentino != null && !string.IsNullOrEmpty(patentino.Numero))
            {
                licenseNumber = patentino.Numero;
                licenseReleaseDate = patentino.Data_Rilascio.ToString("yyyy-MM-dd");
                licenseExpirationDate = patentino.Data_Scadenza.ToString("yyyy-MM-dd");
                licenseReleaseDateToCheck = patentino.Data_Rilascio;
                licenseExpirationDateToCheck = patentino.Data_Scadenza;
            }
        }

        public string workerName { get; set; }

        public string workerSurname { get; set; }

        public string companyName { get; set; }

        public string licenseNumber { get; set; }

        public string licenseReleaseDate { get; set; }

        public string licenseExpirationDate { get; set; }

        /// <summary>
        /// Proprietà introdotta per i controlli da non esportare ad AGEA
        /// </summary>
        [JsonIgnore]
        public DateTime? licenseReleaseDateToCheck { get; set; }


        /// <summary>
        /// Proprietà introdotta per i controlli da non esportare ad AGEA
        /// </summary>
        [JsonIgnore]
        public DateTime? licenseExpirationDateToCheck { get; set; }

        public Worker ToWorker()
        {
            return new Worker()
            {
                fiscalCode = fiscalCode,
                vatNumber = vatNumber,
                workerType = workerType
            };
        }
    }


    public class WorkerType
    {
        public const string Internal = "INTERNAL";
        public const string External = "EXTERNAL";
    }

    public enum PersonType
    {
        PersonaFisica = 0,
        PersonaGiuridica = 1
    }
}
