using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaNetCore.Base.Interfaces
{
    public interface IDeepCloneObject<T>
    {
        T CreateDeepCopy<T>(T obj);

    }
}
