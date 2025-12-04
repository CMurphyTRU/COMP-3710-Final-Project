using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime;

namespace Assets.Scripts.Crossover
{
    public class UniformCrossover : CrossoverFunction
    {
        private char[] separators = { ',', '#', '&', ':', '.' };
        public Breedable[] Perform(Breedable mother, Breedable father)
        {
            Breedable[] children = new Breedable[] { mother, father };
            if (mother.dna.Length != father.dna.Length) return children; // Currently to ensure that dna formatting is maintained
            int dnaLength = mother.dna.Length;

            char[] childDna1 = children[0].dna.ToCharArray();
            char[] childDna2 = children[1].dna.ToCharArray();

            for (int i = 0; i < dnaLength; i++)
            {
                char motherElem = childDna1[i];
                char fatherElem = childDna2[i];


                // Mother and Father should have the same formatting for DNA so only need to check one
                // Save time by only doing crossover on important elements (i.e. numbers)
                if (separators.Contains(motherElem)) continue;

                // A '-' signals that the gene for joints can be skipped as the feature is the first element of the creature.
                if (motherElem == '-' || fatherElem == '-')
                {
                    i += 2;
                    continue; // Skips to next gene
                }

                // Check odds of swapping dna character
                if (MathUtils.rollOdds(Settings.uniformCrossoverChance))
                {
                    // Swap dna elements
                    childDna1[i] = fatherElem;
                    childDna2[i] = motherElem;
                }
            }
            children[0].dna = childDna1.ToString();
            children[1].dna = childDna2.ToString();
            return children;
        }
    }
}
