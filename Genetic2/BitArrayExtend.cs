using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Genetic2
{
    public static class BitArrayExtend
    {
        public static string MakeString(this BitArray ba)
        {
            StringBuilder builder = new StringBuilder(1000);
            for (int i = 0; i < ba.Length; i++)
                builder.Append(ba[i]==true?1:0);
            return builder.ToString();

        }
    }
}
