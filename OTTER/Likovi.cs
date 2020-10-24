using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OTTER
{
    abstract class Likovi:Sprite
    {
        protected string ime;

        public string Ime { get => ime; set => ime = value; }

        public Likovi(string slika, int x, int y, string ime):base(slika, x, y) {
            this.Ime = ime;
        }
        
        
    }
}
