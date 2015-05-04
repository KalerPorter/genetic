using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genetic2
{
    class Program
    {

        static void Main(string[] args)
        {
            GenControl gen = new GenControl(BreedType.Tournament, CrossType.OnePoint);
            gen.defaultInitString(100, 128, 256);
            Console.WriteLine("Solution found");
            Console.ReadLine();
        }

        static void Run(int ind, int ind_max, int ind_inc, int total_it, int stringlenbegin, BreedType breed, CrossType cross, string filename)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(filename, true))
            {
                GenControl cont = new GenControl(breed,cross);
                cont.setFile(filename);

                float average;

                for (int j = ind; j <= ind_max; j += ind_inc)
                {
                    average = 0;

                    Console.WriteLine("String Length: " + j);
                    for (int i = 0; i < total_it; i++)
                    {
                        var gener = cont.defaultInitString(j, stringlenbegin, stringlenbegin*2);
                        average += gener;
                        Console.Write(gener + " ");
                    }
                    average /= ((float)total_it);
                    file.WriteLine(j + "," + average);
                    Console.WriteLine();
                    Console.WriteLine("Average: " + average);
                    Console.WriteLine();
                    Console.WriteLine();
                }

            }
        }
    }
}
