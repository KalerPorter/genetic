using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;


namespace Genetic2
{

    enum BreedType { RandomTop, ProportionalRating, ProportionalRanking, Tournament };
    enum CrossType { OnePoint, TwoPoint, Uniform };

    class Genetic
    {

        public static Random rand = new Random();
        public List<Genome> currentGeneration;

        static public Func<BitArray, int> fitnessFunc
        {
            set;
            get;
        }

        public Func<List<Genome>> breedFunc
        {
            set;
            get;
        }

        public static int crossPoints;
        public static int mutate;
        public static int bitLength;
        public static int individuals;
        public int generationCount;


        public Genetic(Func<BitArray, int> fitness)
        {
            fitnessFunc = fitness;
        }

        public void SetBreedType(BreedType breed)
        {
            switch (breed)
            {
                case BreedType.RandomTop:
                    breedFunc = Breed_RandomTop;
                    break;
                case BreedType.ProportionalRating:
                    breedFunc = Breed_ProportionalRating;
                    break;
                case BreedType.ProportionalRanking:
                    breedFunc = Breed_ProportionalRanking;
                    break;
                case BreedType.Tournament:
                    breedFunc = Breed_Tournament;
                    break;
            }
        }

        public void SetCrossType(CrossType cross)
        {
            switch (cross)
            {
                case CrossType.OnePoint:
                    crossPoints = 1;
                    break;
                case CrossType.TwoPoint:
                    crossPoints = 2;
                    break;
                case CrossType.Uniform:
                    crossPoints = 5;
                    break;
            }
        }


        public void InitalizeGeneration(int individuals, int bitLength, int mutateRate, bool Random = true)
        {
            Genetic.bitLength = bitLength;
            Genetic.individuals = individuals;
            Genetic.mutate = mutateRate;
            this.generationCount = 1;
            currentGeneration = new List<Genome>();
            for (int i = 0; i < individuals; i++)
            {
                currentGeneration.Add(Genome.randomGenome(bitLength));
            }
            RankGeneration(currentGeneration);
            currentGeneration.Sort((x, y) => y._relativeFitness.CompareTo(x._relativeFitness));
            RankAgain(currentGeneration);
        }

        public void RankGeneration(List<Genome> inGeneration)
        {
            int? total = 0;
            foreach (var child in inGeneration)
                total += child.fitness;

            foreach (var child in inGeneration)
                child._relativeFitness = ((double)child.fitness / ((double)total));

        }

        public void RankAgain(List<Genome> inGeneration)
        {
            int count = 1;
            inGeneration.Reverse();
            foreach (var child in inGeneration)
            {
                child._rank = count;
                count++;
            }
            inGeneration.Reverse();
        }

        public List<Genome> Breed()
        {
            ++generationCount;
            currentGeneration = breedFunc();
            foreach (var child in currentGeneration)
            {
                child.Mutate(mutate);
            }
            RankGeneration(currentGeneration);
            currentGeneration.Sort((x, y) => y._relativeFitness.CompareTo(x._relativeFitness));
            RankAgain(currentGeneration);
            return currentGeneration;
        }

        private List<Genome> Breed_RandomTop()
        {
            const int TOP_N = 10;
            var newGeneration = new List<Genome>();

            //random breeding of most successful
            var breeders = currentGeneration.Take(TOP_N).ToArray();
            Genome newChild;
            while (newGeneration.Count < individuals)
            {
                newChild = new Genome(breeders[rand.Next(0, TOP_N)].Cross_N(breeders[rand.Next(0, TOP_N)], crossPoints));
                newGeneration.Add(newChild);
            }
            return newGeneration;
        }

        private List<Genome> Breed_ProportionalRating()
        {
            var newGeneration = new List<Genome>();
            while (newGeneration.Count < individuals) {
                var first = PropRatingSelectHelp(currentGeneration);
                var second = PropRatingSelectHelp(currentGeneration);
                var newChild = first.Cross_N(second, crossPoints);
                newGeneration.Add(newChild);
            }
            return newGeneration;
        }

        private List<Genome> Breed_ProportionalRanking()
        {
            var newGeneration = new List<Genome>();

            while (newGeneration.Count < individuals) {
                var first = PropRankingSelectHelp(currentGeneration);
                var second = PropRankingSelectHelp(currentGeneration);
                var newChild = first.Cross_N(second, crossPoints);
                newGeneration.Add(newChild);
            }

            return newGeneration;
        }

        private List<Genome> Breed_Tournament()
        {
            var newGeneration = new List<Genome>();

            while (newGeneration.Count < individuals)
            {
                var firstContest = TourneyHelp(currentGeneration[rand.Next(currentGeneration.Count)], currentGeneration[rand.Next(currentGeneration.Count)]);
                var secondContest = TourneyHelp(currentGeneration[rand.Next(currentGeneration.Count)], currentGeneration[rand.Next(currentGeneration.Count)]);
                newGeneration.Add(firstContest.Cross_N(secondContest, crossPoints));
            }

            return newGeneration;
        }

        private Genome TourneyHelp(Genome left, Genome right)
        {
            return (left._relativeFitness > right._relativeFitness) ? left : right;
        }

        private Genome PropRatingSelectHelp(List<Genome> generation)
        {
                double accum = 0;
                double chance = rand.NextDouble();

                foreach (var potentialParent in currentGeneration)
                {
                    accum += potentialParent._relativeFitness;
                    if (accum > chance)
                        return potentialParent;
                }
                return currentGeneration[currentGeneration.Count - 1];
        }

        private Genome PropRankingSelectHelp(List<Genome> generation)
        {
                int accum = 0;
                int chance = rand.Next(generation.Count*(generation.Count+1)/2) ;

                foreach (var potentialParent in currentGeneration)
                {
                    accum += potentialParent._rank;
                    if (accum > chance)
                        return potentialParent;
                }
                return currentGeneration[currentGeneration.Count - 1];
        }


        public void reset(Func<BitArray, int> fitness, int pIndividuals, int pBitLength, int pMutateRate)
        {
            currentGeneration = null;
            InitalizeGeneration(pIndividuals, pBitLength, pMutateRate);
        }

    }

    class Genome
    {
        public BitArray genome;
        int? _fitness;
        public int? fitness
        {
            get
            {
                if (_fitness == null)
                    _fitness = Genetic.fitnessFunc(genome);
                return _fitness;
            }
        }

        public double _relativeFitness;
        public int _rank;

        public Genome(BitArray inArray)
        {
            genome = inArray;
            _fitness = null;
            _relativeFitness = 0.0;
        }

        public Genome(Genome nome)
        {
            genome = new BitArray(nome.genome);
            _fitness = null;
            _relativeFitness = 0.0;
        }

        public Genome MakeChild(Genome other, BitArray mask)
        {
            BitArray leftCopy = new BitArray(this.genome);
            BitArray rightCopy = new BitArray(other.genome);
            BitArray maskCopy = new BitArray(mask);

            leftCopy.And(maskCopy);
            rightCopy.And(maskCopy.Not());

            //BitArray temp2 = new BitArray(right.And(mask.Not()));
            return new Genome(leftCopy.Or(rightCopy));
        }

        public Genome Cross_N(Genome other, int points)
        {
            BitArray bitMask = new BitArray(this.genome.Length);
            bitMask.SetAll(false);
            for (int i = 0; i < points; i++)
                bitMask[Genetic.rand.Next(Genetic.bitLength)] = true;
            Extend_Bits(ref bitMask);
            //Console.WriteLine(bitMask.Print());
            return MakeChild( other, bitMask);
        }

        public void Mutate(int mutateRate)
        {
            for (int i = 0; i < Genetic.bitLength; i++)
            {
                if (Genetic.rand.Next(mutateRate) == 0)
                {
                    genome[i] = !genome[i];
                }
            }

            ////Console.WriteLine("ORIG " + genome.MakeString());
            //var times = Genetic.rand.Next(0, (Genetic.bitLength / mutateRate)*2+1 );
            ////Console.WriteLine(times + " mutations");
            //for (int i = 0; i < times; ++i)
            //    genome[Genetic.rand.Next(0, Genetic.bitLength)] = Genetic.rand.Next(1, 3) % 2 == 0;
            ////Console.WriteLine("MUTE " + genome.MakeString());
            ////Console.WriteLine();
        }

        public static Genome randomGenome(int size)
        {
            return new Genome(randomBitArray(size));
        }

        private static BitArray randomBitArray(int size)
        {
            var bits = new BitArray(size);
            for (int i = 0; i < bits.Length; i++)
                bits[i] = Genetic.rand.Next(2) == 0;
            return bits;
        }

        private static void Extend_Bits(ref BitArray inArray, bool random_start = true)
        {
            bool start_with_true = Genetic.rand.Next(2) == 0;
            for (int i = 0; i < inArray.Length; i++)
            {
                if (inArray[i] == true)
                    start_with_true = !start_with_true;
                inArray[i] = start_with_true;
            }
        }

    }
}
