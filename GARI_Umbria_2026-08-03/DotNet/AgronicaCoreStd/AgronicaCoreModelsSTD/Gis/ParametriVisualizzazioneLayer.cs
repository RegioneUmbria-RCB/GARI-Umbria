using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.Gis
{
    public class ParametriVisualizzazioneLayer
    {
        public TypeVisualizzazioneEntita type {get;set;}
        public string baseUrl { get; set; }
        public string bucket { get; set; }
        public string obj { get; set; }
        public List<string> views { get; set; }
        public string gsUriFile { get; set; }

    }

    public enum TypeVisualizzazioneEntita
    {
        CLOUD_STORAGE = 1
    }

}


