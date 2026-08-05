using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.utente
{
    public interface IPermesso
    {
        /// <summary>
        /// Codice identificativo dell'attività gestita dal pemesso.
        /// aka. Id_Attivita, Attivita_Cod
        /// </summary>
        int Permesso_ID { get; set; }

        /// <summary>
        /// Operazioni concesse sull'attività.
        /// aka. Id_Operazione
        ///     LETTURA = 0,
        ///     DISABILITATO = 1,
        ///     LETTURA_SCRITTURA = 2,
        /// </summary>
        int Permesso_Tipo { get; set; }
    }

    public class Utente_Permesso: IPermesso
    {
        //Corrisponde al TipiEnumerativi.enum_Security_Attivita

        /// <summary>
        /// Codice identificativo dell'attività gestita dal pemesso.
        /// aka. Id_Attivita, Attivita_Cod
        /// </summary>
        public int Permesso_ID { get; set; }

        /// <summary>
        /// Operazioni concesse sull'attività.
        /// aka. Id_Operazione:
        ///     LETTURA = 0,
        ///     DISABILITATO = 1,
        ///     LETTURA_SCRITTURA = 2
        /// </summary>
        /// <seealso cref="enum_TipoPermesso"/>
        public int Permesso_Tipo { get; set; }

        public Utente_Permesso() { }

        public Utente_Permesso(int id, int tipo)
        {
            this.Permesso_ID = id;
            this.Permesso_Tipo = tipo;
        }
    }

    public class Utente_Permesso_Gerarchia : Utente_Permesso
    {
        public baseClass.BaseCodeDescr MenuPrimoLivello { get; set; }
        public baseClass.BaseCodeDescr MenuSecondoLivello { get; set; }

        /// <summary>
        /// Descrizione dell'autorizzazione.
        /// </summary>
        public string Attivita_Des { get; set; }
        /// <summary>
        /// Codice identificativo dell'attività gestita dal pemesso.
        /// aka. Permesso_ID, Id_Attivita
        /// </summary>
        public int Attivita_Cod
        {
            get { return Permesso_ID; }
            set { Permesso_ID = value; }
        }
        /// <summary>
        /// Operazioni concesse sull'attività.
        /// aka. Permesso_Tipo
        ///     LETTURA = 0,
        ///     DISABILITATO = 1,
        ///     LETTURA_SCRITTURA = 2,
        /// </summary>
        public int Id_Operazione
        {
            get { return Permesso_Tipo; }
            set { Permesso_Tipo = value; }
        }

        public string Funzioni_Lettura { get; set; }
        public string Note_Lettura { get; set; }
        public string Funzioni_Scrittura { get; set; }
        public string Note_Scrittura { get; set; }
        public string Ordinamento { get; set; }

        public Utente_Permesso_Gerarchia(): base() { }

        public Utente_Permesso_Gerarchia(int id, int tipo) : base(id, tipo) { }

        public Utente_Permesso_Gerarchia(int id, int tipo, string des) : base(id, tipo)
        {
            this.Attivita_Des = des;
        }

        public Utente_Permesso_Gerarchia(int id, string des)
        {
            this.Attivita_Des = des;
            this.Attivita_Cod = id;
        }

        public Utente_Permesso_Gerarchia CreateCopy()
        {
            Utente_Permesso_Gerarchia copy = new Utente_Permesso_Gerarchia(Attivita_Cod, Permesso_Tipo, Attivita_Des);
            copy.MenuPrimoLivello = this.MenuPrimoLivello;
            copy.MenuSecondoLivello = this.MenuSecondoLivello;
            copy.Funzioni_Lettura = this.Funzioni_Lettura;
            copy.Note_Lettura = this.Note_Lettura;
            copy.Funzioni_Scrittura = this.Funzioni_Scrittura;
            copy.Note_Scrittura = this.Note_Scrittura;
            copy.Ordinamento = this.Ordinamento;
            return copy;
        }
    }
}
