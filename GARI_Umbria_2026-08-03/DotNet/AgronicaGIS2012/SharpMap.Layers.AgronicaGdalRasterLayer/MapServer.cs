using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SharpMap.Layers;

namespace SharpMap.Layers.AgronicaGdalRasterLayer
{
    public class MapServer
    {

        public List<Layers.Layer> GetMap(                
              string fullFileNameWithPath
            , double pTop
            , double pLeft
            , double pBottom
            , double pRight
            , double pPixelWidth
            , double pPixelHeight
        )
        {

            List<Layers.Layer> rval = new List<Layer>();

            Layers.AgroGdalRasterLayer Layer = new Layers.AgroGdalRasterLayer("tmp", fullFileNameWithPath);

            rval.Add(Layer);

            return rval;

        }



    }
}
