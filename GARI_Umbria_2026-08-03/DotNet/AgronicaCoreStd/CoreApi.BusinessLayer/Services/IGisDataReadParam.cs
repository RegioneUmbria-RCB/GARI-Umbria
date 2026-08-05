using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgronicaCoreDTOStd.InData.Gis;
using AgronicaCoreModelloSTD;

namespace CoreApi.BusinessLayer.Services
{
    public interface IGisDataReadParam
    {
        Task<bool> IsValid(GisDataReadParam gisDataReadParam);
    }

    public class GisDataReadParamValidator : IGisDataReadParam
    {
        public Task<bool> IsValid(GisDataReadParam gisDataReadParam)
        {
             var rval = true;

            if (String.IsNullOrEmpty(gisDataReadParam.piva)
                && String.IsNullOrEmpty(gisDataReadParam.wktBoundaySTIntersects)
                && gisDataReadParam.cfgSementi == null)
            {
                rval = false;
            }

            return Task.FromResult(rval);
        }
    }



}
