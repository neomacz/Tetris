using System;
using System.Collections.Generic;
using System.Text;

namespace Tetris.Cherub
{
    public abstract class Shape : Object
    {
        public int direction;
        public int color;
        public int[] blockX = new int[4];
        public int[] blockY = new int[4];

        public abstract void Rotate(int rotation);
    }
}
