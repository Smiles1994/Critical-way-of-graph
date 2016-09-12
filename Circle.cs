using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;

namespace WindowsFormsApplication2
{
    [Serializable]
    class Circle
    {
        private int x;
        private int y;
        private int r;
        private int n;
        public Circle(int x, int y, int r, int n)
        {
            this.x = x;
            this.y = y;
            this.r = r;
            this.n = n;

        }
        public int X
        {
            get { return x; }
            set { x = value; }
        }
        public int Y
        {
            get { return y; }
            set { y = value; }
        }
        public int R
        {
            get { return r; }
            set { r = value; }
        }
        public int N
        {
            get { return n; }
            set { n = value; }
        }
    }
}
