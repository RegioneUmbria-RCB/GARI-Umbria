using AgronicaNetCore.Base.Models;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace AgronicaNetCore.Base.Utility.ObjectExtension
{
    public static class ObjectExtension
    {
        public static TChild PropertyCopier<TParent, TChild>(this TParent parent, TChild child, string[]? key_properties_to_exclude = null)
        {

            // Questo metodo è in grado di copiare anche le proprietà a loro volta oggetto (ex. Appezzamento.Reg_Impianti)
            // NB: IMPORTANTE CHE key_properties_to_exclude SIANO IN MINUSCOLO! IL CONFRONTO E' CASE SENSITIVE!!!!

            var parentProperties = parent!.GetType().GetProperties();
            var childProperties = child!.GetType().GetProperties();

            foreach (var parentProperty in parentProperties)
            {
                if (key_properties_to_exclude != null && key_properties_to_exclude.Contains(parentProperty.Name.ToLower()))
                    continue;

                foreach (var childProperty in childProperties)
                {
                    if (key_properties_to_exclude != null && key_properties_to_exclude.Contains(childProperty.Name.ToLower()))
                        continue;

                    if (parentProperty.Name.ToLower() == childProperty.Name.ToLower() && parentProperty.PropertyType == childProperty.PropertyType)
                    {
                        childProperty.SetValue(child, parentProperty.GetValue(parent));
                        break;
                    }
                }
            }

            return child;
        }
    }
}
