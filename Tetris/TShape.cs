using System;
using System.Collections.Generic;
using System.Text;

namespace Tetris.Cherub
{
    class TShape : Shape
    {
        public TShape(int color, int direction)
        {
            this.color = color;
            this.direction = direction;
            setCoordinate();
            
        }

        private void setCoordinate()
        {
            if (direction == 0)
            {
                blockX[0] = 0;
                blockY[0] = 1;
                blockX[1] = 0;
                blockY[1] = 2;
                blockX[2] = -1;
                blockY[2] = 2;
                blockX[3] = 1;
                blockY[3] = 2;
            }
            else if (direction == 1)
            {
                blockX[0] = 0;
                blockY[0] = 0;
                blockX[1] = 0;
                blockY[1] = 1;
                blockX[2] = 0;
                blockY[2] = 2;
                blockX[3] = 1;
                blockY[3] = 1;
            }
            else if (direction == 2)
            {
                blockX[0] = 0;
                blockY[0] = 1;
                blockX[1] = -1;
                blockY[1] = 1;
                blockX[2] = 1;
                blockY[2] = 1;
                blockX[3] = 0;
                blockY[3] = 2;
            }
            else if (direction == 3)
            {
                blockX[0] = 0;
                blockY[0] = 0;
                blockX[1] = 0;
                blockY[1] = 1;
                blockX[2] = 0;
                blockY[2] = 2;
                blockX[3] = -1;
                blockY[3] = 1;
            }
        }

        public override void Rotate(int rotation)
        {
            direction = (direction + rotation + 40) % 4;
            setCoordinate();
        }
    }
}
