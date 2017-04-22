using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public class Blocks
    {
        public int[,] Block1 = new int[3, 2] {
                                    {1, 0} ,   
                                    {1, 0} ,   
                                    {1, 1}   
                                };
        public int[,] Block2 = new int[2, 3] {
                                    {1, 1, 1} ,   
                                    {0, 1, 0}   
                                };

        public int[,] Block3 = new int[2, 3] {
                                    {1, 1, 0} ,   
                                    {0, 1, 1}   
                                };
        public int[,] Block4 = new int[2, 2] {
                                    {1, 1},
                                    {1, 1}
                                };
        public int[,] Block5 = new int[3, 1] {
                                    {1},
                                    {1},
                                    {1}
                                };
        public int[,] Block6 = new int[3, 2] {
                                    {1, 1},
                                    {1, 0},
                                    {1,0}
                                };
        public int[,] Block7 = new int[2, 3]{
                                {1,0,1},
                                {1,1,1}
                                };
    }
}
