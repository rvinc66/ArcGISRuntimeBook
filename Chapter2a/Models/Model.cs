using System;

namespace Chapter2a.Models
{
    public class Model
    {
        private string searchText = "Lancaster";
        private string basemapLayerUri = "http://services.arcgisonline.com/ArcGIS/rest/services/World_Topo_Map/MapServer";
        private string usaLayerUri = "http://sampleserver6.arcgisonline.com/arcgis/rest/services/USA/MapServer";

        public Model()  { }

        public string SearchText
        {
            get { return this.searchText; }
            set
            {
                if (value != this.searchText)
                {
                    this.searchText = value;
                }
            }
        }
        public string BasemapLayerUri
        {
            get {return this.basemapLayerUri;}
            set
            {
                if (value != this.basemapLayerUri)
                {
                    this.basemapLayerUri = value;
                }
            }
        }
        public string USALayerUri
        {
            get { return this.usaLayerUri; }
            set
            {
                if (value != this.usaLayerUri)
                {
                    this.usaLayerUri = value;
                }
            }
        }
    }
}
