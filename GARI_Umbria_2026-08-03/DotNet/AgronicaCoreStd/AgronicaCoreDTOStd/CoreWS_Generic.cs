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

public class CoreWS_Generic<T> : CoreWS_GenericPayload
{
    public T InData { get; set; }

    public CoreWS_Generic(CoreWS_GenericObjP objP, T inData) : base(objP)
    {
        this.InData = inData;
    }

    public CoreWS_Generic() { }

}