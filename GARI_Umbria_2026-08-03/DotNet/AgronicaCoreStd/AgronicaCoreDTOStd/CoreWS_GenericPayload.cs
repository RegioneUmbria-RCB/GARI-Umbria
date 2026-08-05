using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

public class CoreWS_GenericPayload
{
    public CoreWS_GenericObjP objP { get; set; }

    public CoreWS_GenericPayload(CoreWS_GenericObjP objP)
    {
        this.objP = objP;
    }

    public CoreWS_GenericPayload() { }

}