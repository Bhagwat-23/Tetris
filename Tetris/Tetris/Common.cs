using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public struct Matrix
    {
        public int data;
        public bool isLanded;
    };
    public enum Command
    {
        LEFT=0,
        RIGHT,
        DOWN,
        ROTATE_RIGHT,
        ROTATE_LEFT,
        LAND
    };

    public class Common
    {
    }
}
