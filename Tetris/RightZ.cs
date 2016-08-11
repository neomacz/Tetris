using System;
using System.Collections.Generic;
using System.Text;

namespace Tetris.Cherub
{
    class RightZ : Shape
    {
        public RightZ(int color, int direction)
        {
            this.color = color;
            this.direction = direction;
            setCoordinate();

        }

        private void setCoordinate()
        {
            if (direction == 0)
            {
                blockX[0] = -1;
                blockY[0] = 0;
                blockX[1] = 0;
                blockY[1] = 0;
                blockX[2] = 0;
                blockY[2] = 1;
                blockX[3] = 1;
                blockY[3] = 1;
            }
            else if (direction == 1)
            {
                blockX[0] = 0;
                blockY[0] = 0;
                blockX[1] = 0;
                blockY[1] = 1;
                blockX[2] = -1;
                blockY[2] = 1;
                blockX[3] = -1;
                blockY[3] = 2;
            }
        }
        public override void Rotate(int rotation)
        {
            direction = 1 - direction;
            setCoordinate();
        }
    }
}
