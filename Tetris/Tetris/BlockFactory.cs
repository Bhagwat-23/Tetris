using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Tetris
{
    public static class BlockFactory
    {
        public static int[,] GetBlock(int blockNumber)
        {
            Blocks block = new Blocks();
            int[,] Block = new int[3, 3];

            switch (blockNumber)
            {
                case 1:
                    Block = block.Block1.Clone() as int[,];//Copy Matrix.

                    break;
                case 2:
                    Block = block.Block2.Clone() as int[,];//Copy Matrix.

                    break;
                case 3:
                    Block = block.Block3.Clone() as int[,];//Copy Matrix.
                    break;
                case 4:
                    Block = block.Block4.Clone() as int[,];//Copy Matrix.
                    break;
                case 5:
                    Block = block.Block5.Clone() as int[,];//Copy Matrix.
                    break;
                case 6:
                    Block = block.Block6.Clone() as int[,];//Copy Matrix.
                    break;
                case 7:
                    Block = block.Block7.Clone() as int[,];//Copy Matrix.
                    break;
            }
            return Block;
        }

    }
}
