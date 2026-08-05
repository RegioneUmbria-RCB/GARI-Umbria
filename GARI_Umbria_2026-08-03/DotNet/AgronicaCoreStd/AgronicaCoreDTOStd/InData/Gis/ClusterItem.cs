using System;
using System.Collections.Generic;
using System.Text;

namespace InData.Gis
{
    // Viene usato dal BIZ per generare l’oggetto di output che l’algoritmo di Clustering deve restituire.
    public class ClusterItem
    {


        // L'ID dell'entità (Entita_Cod) del punto che è stato scelto come centroide per il cluster item.
        public int Id { get; set; }

        // La rappresentazione WKT del punto del centroid.
        public string Wkt_Centroid { get; set; }

        // Riportato il numero dei punti associati all’id del cluster.
        public int ElementCount { get; set; }

        public ClusterItem()
        {
            Wkt_Centroid = string.Empty;
        }
    }

}

