using System;

namespace Chapter6.Models
{
    public class Model
    {
        private string tilePackage = "";
        private string mapPackage = "";
        private string gdb = "";

        public Model()  { }

        public string TilePackage
        {
            get { return this.tilePackage; }
            set
            {
                if (value != this.tilePackage)
                {
                    this.tilePackage = value;
                }
            }
        }
        public string MapPackage
        {
            get {return this.mapPackage;}
            set
            {
                if (value != this.mapPackage)
                {
                    this.mapPackage = value;
                }
            }
        }
        public string GDB
        {
            get { return this.gdb; }
            set
            {
                if (value != this.gdb)
                {
                    this.gdb = value;
                }
            }
        }
    }
}
