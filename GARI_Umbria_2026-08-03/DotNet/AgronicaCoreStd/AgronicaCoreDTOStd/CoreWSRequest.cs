using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

public class CoreWSRequest<T>
{

    public T InData;

    public CoreWSRequest(T InData)
    {
        this.InData = InData;
    }

}

