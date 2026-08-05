using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Gis
{
    /// <summary>
    /// Parametri di filtro in lettura della lista dei layer e dei tiles
    /// </summary>
    public class AttributoLayer_In
    {
        /// <summary>
        /// l’ID del layer selezionato
        /// </summary>
        /// <example>123</example>
        public string IdLayer { get; set; }

        /// <summary>
        /// Nuovo nome dell'Attributo (INSERT ed UPDATE)
        /// </summary>
        /// <example>Catasto</example>
        public string Nome { get; set; }

        /// <summary>
        /// Il tipo del dato dell'attributo
        /// </summary>
        /// <example>string</example>
        public string TipoDato { get; set; }

        /// <summary>
        /// Progressivo Data Struct. OPZIONALE, solo per operazioni di UPDATE e DELETE
        /// </summary>
        /// <example>123</example>
        public string ProgressivoDataStruct { get; set; }

        /// <summary>
        /// Operazione da eseguire sull'input (da Enum OperazioneAttributo)
        /// </summary>
        /// <example>1</example>
        public OperazioneAttributo IdOperazione { get; set; }

        /// <summary>
        /// ENUM delle operazioni CRUD disponibili:
        ///     INSERT = 1,
        ///     UPDATE = 2,
        ///     DELETE = 3
        /// </summary>
        public enum OperazioneAttributo
        {
            INSERT = 1,
            UPDATE = 2,
            DELETE = 3
        }

    }
}
