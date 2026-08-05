using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security;
using System.Text;

namespace AgronicaCoreModelsSTD.exceptions
{
    [Serializable]
    public class AgroEccezioni_ParticellaConLegami_Exception: GiasException
    {
        public AgroEccezioni_ParticellaConLegami_Exception() { }

        public AgroEccezioni_ParticellaConLegami_Exception(string message) : base(message) { }

        public AgroEccezioni_ParticellaConLegami_Exception(string message, System.Exception innerException) : base(message, innerException) { }

        [SecuritySafeCritical]
        public AgroEccezioni_ParticellaConLegami_Exception(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
