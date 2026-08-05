using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace AgronicaCoreModelsSTD.IsAlive
{
    /// <summary>
    /// Classe per risposta funzione IsAlive
    /// </summary>
    [Serializable]
    public class CheckIsAliveOUT
    {
        /// <summary>
        /// Lista DB verificati
        /// </summary>
        public List<ReachableDBsOUT> reachableDBs { get; set; }
        /// <summary>
        /// Lista siti verificati
        /// </summary>
        public List<ReachableSiteOUT> reachableSites { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ReachableDBsOUT
    {
        /// <summary>
        /// 
        /// </summary>
        public string key { get; set; }
        ///// <summary>
        ///// Connection string per il DB
        ///// </summary>
        //public string connectionString { get; set; } 'Disabilitata, da valutare se riattivarla sotto showConnectionString
        /// <summary>
        /// Risposta per quel sito in caso di errore
        /// </summary>
        public string error { get; set; }
        /// <summary>
        /// Messaggio legato al warmup di EF (solo per db Server)
        /// </summary>
        [JsonIgnoreIfNull]
        public string errorEF { get; set; }
        /// <summary>
        /// DB raggiungibile o meno
        /// </summary>
        public bool reachable { get; set; }

        public ReachableDBsOUT() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="connectionString"></param>
        /// <param name="error"></param>
        /// <param name="errorEF"></param>
        /// <param name="reachable"></param>
        public ReachableDBsOUT(string key, string connectionString, string error, string errorEF, bool reachable)
        {
            this.key = key;
            //this.connectionString = connectionString;
            this.error = error;
            this.errorEF = errorEF;
            this.reachable = reachable;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ReachableSiteOUT
    {
        /// <summary>
        /// Chiave sito
        /// </summary>
        public string key { get; set; }
        /// <summary>
        /// Url del sito chiamato (se vuoto non viene serializzato)
        /// </summary>
        [JsonIgnoreIfNull]
        public string url { get; set; }
        /// <summary>
        /// Risposta per quel sito in caso di errore
        /// </summary>
        public string error { get; set; }
        /// <summary>
        /// Versione del sito (se vuoto non viene serializzato)
        /// </summary>
        [JsonIgnoreIfNull]
        public string version { get; set; }
        /// <summary>
        /// Sito raggiungibile o meno
        /// </summary>
        public bool reachable { get; set; }

        public ReachableSiteOUT() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="error"></param>
        /// <param name="version"></param>
        /// <param name="url"></param>
        /// <param name="reachable"></param>
        public ReachableSiteOUT(string key, string url, string error, string version, bool reachable)
        {
            this.key = key;
            this.url = url;
            this.error = error;
            this.version = version;
            this.reachable = reachable;
        }
    }
    
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class JsonIgnoreIfNullAttribute : Attribute
    {
        public JsonIgnoreIfNullAttribute()
        {
        }
    }

}
