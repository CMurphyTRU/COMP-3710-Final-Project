using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Crossover
{
    public interface CrossoverFunction
    {
        public Breedable[] Perform(Breedable mother, Breedable father);
    }
}
