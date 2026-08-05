using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.anagrafiche
{
    public class Bookmark
    {
        public int Bookmark_Cod { get; set; }

        public string Bookmark_Des { get; set; }

        public double Center_Lat { get; set; }

        public double Center_Lng { get; set; }

        public int Zoom { get; set; }

        public int Posizioni_Speciali { get; set; }

    }
}
