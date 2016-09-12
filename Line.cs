using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsFormsApplication2
{
    [Serializable]
    class Line
    {
        Circle p1;
        Circle p2;
        public int d;
        public Line(Circle p1, Circle p2,int d)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.d = d;
        }
        public Circle P1
        {
            get {return p1;}
            set{p1 = value;}
        }
        public Circle P2
        {
            get { return p2; }
            set { p2 = value; }
        }
        public int D
        {
            get { return d; }
            set { d = value; }
        }
    }
}
