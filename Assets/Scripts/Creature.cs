using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Creature : Breedable
{
    public static Material defaultMaterial;

    [SerializeField] public MeshRenderer defaultMeshRenderer;
    [SerializeField] public Color colour = new Color(255, 0, 0);
    [SerializeField] public GameObject[] features;

    public override void mutate()
    {
        throw new System.NotImplementedException();
    }

    private void Awake()
    {
        if (defaultMaterial == null) defaultMaterial = Resources.Load < Material >("Materials/DefaultCreatureMaterial");

        generateFeatureList();
        addMovementScripts();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Camera.main.transform.position = features[0].transform.position + Vector3.back;
    }

    private void generateFeatureList()
    {
        features = new GameObject[Settings.creatureFeatures];
        features[0] = generateFeature(0);

        for (int i = 1; i < features.Length; i++)
        {
            features[i] = generateFeature(i, true, features[Random.Range(0, i - 1)].GetComponent<CreatureFeature>());
        }
    }

    private GameObject generateFeature(int id, bool hasJoint = false, CreatureFeature sibling = null)
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
            feature.sibling = sibling;

            feature.joint = joint;
        }
        featureObject.transform.SetParent(this.transform);
        featureObject.transform.position = Vector3.zero;
        return featureObject;
    }

    private void addMovementScripts()
    {
        foreach(GameObject creatureObject in features)
        {
            creatureObject.AddComponent<CreatureMovement>();
        }
    }
}
