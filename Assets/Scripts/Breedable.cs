using System.CodeDom.Compiler;
using UnityEngine;

public interface Breedable
{
    public string dna
    {
        get;
        set;
    }
    public Breedable crossover(Breedable partner);
    public void mutate();
}
