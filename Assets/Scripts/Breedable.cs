using Assets.Scripts.Crossover;
using System.CodeDom.Compiler;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Breedable : MonoBehaviour
{
    public string dna
    {
        get;
        set;
    } = "";
    public Breedable[] crossover(Breedable partner, CrossoverFunction crossoverFunction)
    {
        return crossoverFunction.Perform(this, partner);
    }
    public abstract void mutate();
}
