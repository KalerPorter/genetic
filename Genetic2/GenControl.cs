using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Genetic2
{
    class GenControl
    {

        string currentFileName;


        static public byte[] targetString;
        BreedType breed;
        CrossType cross;
        public GenControl(BreedType _breed, CrossType _cross)
        {
            breed = _breed;
            cross = _cross;
            targetString = Encoding.ASCII.GetBytes("THE_STRING_MATCH");
        }

        public int bitCount(int INDIVIDUALS, int STRING_LENGTH, int MUTATE)
        {

            Genetic gen = new Genetic(FitnessFunctionCountOnes);
            gen.InitalizeGeneration(INDIVIDUALS, STRING_LENGTH, MUTATE);
            gen.SetBreedType(breed);
            gen.SetCrossType(cross);

            int? bestFitness = 0;


            int mileStoneCount = 0;

            List<Genome> generationData;

            do
            {
                generationData = gen.Breed();

                if (gen.currentGeneration[0].fitness > bestFitness )
                {
                    mileStoneCount += 1;
                    bestFitness = gen.currentGeneration[0].fitness;
                    //Console.WriteLine("Best Fitness " + gen.currentGeneration[0].fitness + " Current Generation: " + gen.generationCount);
                    //Console.WriteLine("Current String: " + gen.currentGeneration[0].genome.MakeString());
                    //Console.WriteLine();
                    //WriteResult("Tournament", "Single-Point", ITERATION, INDIVIDUALS, STRING_LENGTH, MUTATE, gen.generationCount, mileStoneCount);
                }

            } while (gen.currentGeneration[0].fitness < STRING_LENGTH);
            return gen.generationCount;
        }

        public int defaultInitString(int INDIVIDUALS, int STRING_LENGTH, int MUTATE)
        {
            Genetic gen = new Genetic(FitnessFunctionTargetString);
            gen.InitalizeGeneration(INDIVIDUALS, STRING_LENGTH, MUTATE);
            gen.SetBreedType(breed);
            gen.SetCrossType(cross);

            int? bestFitness = 0;

            var generationData = gen.Breed();
            string removeN = BitArrayToStr(gen.currentGeneration[0].genome);

            while (removeN.CompareTo(System.Text.Encoding.UTF8.GetString(targetString)) != 0)
            {
                generationData = gen.Breed();

                if (gen.currentGeneration[0].fitness > bestFitness)
                {
                    removeN = BitArrayToStr(gen.currentGeneration[0].genome);
                    char[] invalid = System.IO.Path.GetInvalidFileNameChars();

                    bestFitness = gen.currentGeneration[0].fitness;
                    Console.WriteLine("Best Fitness " + gen.currentGeneration[0].fitness + " Current Generation: " + gen.generationCount);
                    Console.WriteLine("Current String: " + new String(removeN.Where(c => !invalid.Contains(c)).ToArray()));
                    Console.WriteLine();
                }

            }
            return gen.generationCount;
        }

        static int FitnessFunctionCountOnes(BitArray array)
        {
            int count = 0;
            foreach (bool a in array)
            {
                if (a == true)
                    count += 1;
            }
            return count;
        }

        static int FitnessFunctionTargetString(BitArray array)
        {
            int count = 1;


            String tempString = BitArrayToStr(array);

            //var enumer = array.GetEnumerator();

            //enumer.Reset();

            //const int SKIP = 4;
            //int index = 0;
            //var tempRay = new BitArray(4);
            //for (int i = 0; i < SKIP; i++)
            //{
            //    tempRay[i] = array[index + i];
            //}

            for (int i = 0; i < targetString.Length; i++)
            {

                if (tempString[i] == targetString[i])
                    count += 1;
            }

            return count;

            //int count = 0;
            //foreach (bool a in array)
            //    if (a == true)
            //        ++count;
            //return count;
        }

        //BitArrayToStr found on stack overflow at
        //http://stackoverflow.com/questions/3917086/convert-bitarray-to-string
        public static String BitArrayToStr(BitArray ba)
        {
            byte[] strArr = new byte[ba.Length / 8];

            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

            for (int i = 0; i < ba.Length / 8; i++)
            {
                for (int index = i * 8, m = 1; index < i * 8 + 8; index++, m *= 2)
                {
                    strArr[i] += ba.Get(index) ? (byte)m : (byte)0;
                }
            }

            return encoding.GetString(strArr);
        }


        public void WriteResult(string breedType, string crossType,int ITERATION, int INDIVIDUALS, int STRING_LENGTH, int MUTATE,int generation, int mileStoneCount)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(currentFileName, true))
            {
                file.WriteLine(breedType+","+crossType+","+ITERATION+","+INDIVIDUALS+","+STRING_LENGTH+","+MUTATE+","+generation+","+mileStoneCount);
            }
        }



        public void setFile(string p)
        {
            currentFileName = p;
        }
    }
}
