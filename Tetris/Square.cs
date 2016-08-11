using System;
using System.Collections.Generic;
using System.Text;

namespace Tetris.Cherub
{
    class Square : Shape
    {
        public Square(int color, int direction)
        {
            this.color = color;
            blockX[0] = 0;
            blockY[0] = 0;
            blockX[1] = 1;
            blockY[1] = 0;
            blockX[2] = 0;
            blockY[2] = 1;
            blockX[3] = 1;
            blockY[3] = 1;
        }

        public override void Rotate(int rotation)
        {
            // Do nothing
        }
    }
}
