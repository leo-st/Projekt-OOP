using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Windows.Forms;

namespace OTTER
{
    class Cekic:Likovi
    {
        protected int zivot;

        public int Zivot { get => zivot; set {
                if (value < 0)
                {
                    throw new Exception();
                }
                else
                {
                    zivot = value;
                }
            } }

        public Cekic(string slika, int x, int y, string ime, int zivot):base(slika, x, y, ime)
        {
            this.Zivot = zivot;
        }
        
       
    }
}
