using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OTTER
{
    class Voce:Likovi
    {
        protected bool zivot;
        public bool Zivot { get => zivot; set => zivot = value; }

        

        
        public Voce(string slika, int x, int y, string ime, bool zivot):base(slika, x,y, ime)
        {
            
            if (zivot == true)
            {
                this.zivot = true;
            }
            else
            {
                this.zivot = false;
            }

        }
        

        
    }
}
