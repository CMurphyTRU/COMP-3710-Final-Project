using System.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class CreatureFeature : MonoBehaviour, GeneHolder
{

    [SerializeField] private new PolygonCollider2D collider;
    [SerializeField] private Rigidbody2D rigidBody;
    [SerializeField] private Mesh mesh;
    [SerializeField] private Vector2[] pathList;
    [SerializeField] private Vector2 pathCenter;
    [SerializeField] public HingeJoint2D joint;
    [SerializeField] public Color colour;
    [SerializeField] private int hingeIndex = -1;
    public string genes { get; set;} = "";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        collider = gameObject.GetComponent<PolygonCollider2D>();
        rigidBody = gameObject.GetComponent<Rigidbody2D>();

        if (genes == "")
        {
            generatePath();
        } else
        {
            generatePathWithGene();
        }

        generateMesh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void generatePath()
    {
        int size = Settings.pathSize;
        pathList = new Vector2[size];
        float xTotal = 0;
        float yTotal = 0;

        for(int i = 0; i < size; i++)
        {
            float x = Random.value * 2 - 1;
            float y = Random.value * 2 - 1;
            pathList[i].x = x;
            pathList[i].y = y;
            xTotal += x;
            yTotal += y;

            genes += string.Format("{0:0.00},{0:0.00}", x + 1, y + 1);
            if(i < size - 1) genes += ",";

        }
        if (hingeIndex < 0) genes += ":-00";
        else genes += string.Format(":{0:000}", hingeIndex);
        Debug.Log(genes);
        pathCenter = new Vector2(xTotal / size, yTotal / size);
        pathList = PolygonUtils.sortClockwise(pathList, pathCenter);
        collider.SetPath(0, pathList);
    }

   private void generatePathWithGene()
    {
        string[] components = genes.Split(':');
        string[] coords = components[0].Split(',');
        int joint = int.Parse(components[1]);
        pathList = new Vector2[coords.Length / 2];

        for(int i = 0; i < coords.Length; i += 2)
        {
            float x = float.Parse(coords[i]) - 1;
            float y = float.Parse(coords[i + 1]) - 1;
            pathList[i / 2].x = x;
            pathList[i / 2].y = y;
        }
        collider.SetPath(0, pathList);
    }

    private void generateMesh()
    {
        MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
        MeshFilter filter = this.gameObject.AddComponent<MeshFilter>();

        Material material = Object.Instantiate(Creature.defaultMaterial);
        material.color = colour;
        renderer.material = material;
        mesh = new Mesh();

        filter.mesh = mesh;

        // Add the vertices and triangles to the mesh so it renders a polygon

        int verticesLen = pathList.Length;

        Vector3[] vertices = new Vector3[verticesLen];
        int[] triangles = new int[(verticesLen-2) * 3];

        for (int i = 0; i < verticesLen; i++)
        {
            vertices[i] = pathList[i];
            int triIndex = i * 3;
            if(triIndex < triangles.Length)
            {
                triangles[triIndex] = 0;
                triangles[triIndex + 1] = i + 1;
                triangles[triIndex + 2] = i + 2;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
    }
}
