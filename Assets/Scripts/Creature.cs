using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Creature : Breedable
{
    public static Material defaultMaterial;

    [SerializeField] public MeshRenderer defaultMeshRenderer;
    [SerializeField] public Color colour = new Color(255, 0, 0);
    [SerializeField] public GameObject[] features;

    public string dna { 
        get {
            return "";
        } set
        {
            this.dna = value;
        }
    }


    public override void mutate()
    {
        throw new System.NotImplementedException();
    }

    private void Awake()
    {
        if (defaultMaterial == null) defaultMaterial = Resources.Load < Material >("Materials/DefaultCreatureMaterial");

        features = new GameObject[Settings.creatureFeatures];
        features[0] = generateFeature(0, false);

        for (int i = 1; i < features.Length; i++)
        {
            features[i] = generateFeature(i, true, features[i - 1].GetComponent<CreatureFeature>());
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private GameObject generateFeature(int id, bool hasJoint, CreatureFeature sibling = null)
    {
        GameObject featureObject = new GameObject("CreatureFeature" + id);
        CreatureFeature feature = featureObject.AddComponent<CreatureFeature>();
        feature.AddComponent<PolygonCollider2D>();
        feature.AddComponent<Rigidbody2D>();
        feature.colour = colour;

        if (hasJoint)
        {
            HingeJoint2D joint = featureObject.AddComponent<HingeJoint2D>();
            joint.connectedBody = sibling.GetComponent<Rigidbody2D>();

            feature.joint = joint;
        }

        featureObject.transform.position = new Vector3(id * 1, 0);
        return featureObject;
    }
}
