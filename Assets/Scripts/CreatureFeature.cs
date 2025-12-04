using System.Collections;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class CreatureFeature : MonoBehaviour, GeneHolder
{

    // Core Features
    [SerializeField] private new PolygonCollider2D collider;
    [SerializeField] private Rigidbody2D rigidBody;

    // Rendering
    [SerializeField] private Mesh mesh;
    [SerializeField] private Vector2[] pathList;
    [SerializeField] private Vector2 pathCenter;
    [SerializeField] public Color colour;
    public string genes { get; set;} = "";

    // Joints

    [SerializeField] public HingeJoint2D joint;
    [SerializeField] private int anchorIndex = -1;
    [SerializeField] private int siblingAnchorIndex = -1;
    [SerializeField] public CreatureFeature sibling;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {

        collider = gameObject.GetComponent<PolygonCollider2D>();
        rigidBody = gameObject.GetComponent<Rigidbody2D>();

        if (genes == "")
        {
            generatePath();
        }
        else
        {
            generateUsingGenes();
        }
        collider.SetPath(0, pathList);

        generateMesh();
    }

    // Update is called once per frame
    private void Update()
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

        pathCenter = new Vector2(xTotal / size, yTotal / size);
        PolygonUtils.fixOrigin(pathList, pathCenter);
        pathList = PolygonUtils.sortClockwise(pathList, pathCenter);

        adjustJoints();

        addAnchorGene(anchorIndex);
        addAnchorGene(siblingAnchorIndex);
    }

   private void generateUsingGenes()
    {
        string[] elements = genes.Split(':'); // Split gene into specific components


        generatePathFromGenes(elements[0]); // Generates list of numbers where every 2 represents a point.

        int joint = int.Parse(elements[1]);
        int siblingJoint = int.Parse(elements[2]);
    }

    private void adjustJoints()
    {
        if (sibling == null) return;
        siblingAnchorIndex = sibling.getRandomPointIndex();
        anchorIndex = getRandomPointIndex();

        transform.localPosition = sibling.transform.localPosition + sibling.getPointAt(siblingAnchorIndex) - getPointAt(anchorIndex);

        joint.anchor = getPointAt(anchorIndex);
    }

    public int getRandomPointIndex()
    {
        return Random.Range(0, pathList.Length - 1);
    }
    public Vector3 getPointAt(int index)
    {
        return pathList[index];
    }

    private void generatePathFromGenes(string pathGene)
    {
        string[] coords = pathGene.Split(',');

        pathList = new Vector2[coords.Length / 2];

        for (int i = 0; i < coords.Length; i += 2)
        {
            float x = float.Parse(coords[i]) - 1;
            float y = float.Parse(coords[i + 1]) - 1;
            pathList[i / 2].x = x;
            pathList[i / 2].y = y;
        }
    }

    private void addAnchorGene(int index)
    {
        if (index < 0) genes += ":-00";
        else genes += string.Format(":{0:000}", index);
    }

    private void generateMesh()
    {
        // Renderer setup
        MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
        Material material = Object.Instantiate(Creature.defaultMaterial);
        material.color = colour;
        renderer.material = material;
        // Filter setup
        MeshFilter filter = this.gameObject.AddComponent<MeshFilter>();
        mesh = new Mesh();
        filter.mesh = mesh;

        // Populating mesh with vertices and triangles
        mesh.vertices = createVertices();
        mesh.triangles = createTriangles(); 
 
        
    }

    private int[] createTriangles()
    {
        int verticesLen = pathList.Length;
        int[] triangles = new int[(verticesLen - 2) * 3];

        for (int i = 0; i < verticesLen; i++)
        {
            int triIndex = i * 3;
            if (triIndex < triangles.Length)
            {
                triangles[triIndex] = 0;
                triangles[triIndex + 1] = i + 1;
                triangles[triIndex + 2] = i + 2;
            }
        }
        return triangles;
    }

    private Vector3[] createVertices()
    {
        int verticesLen = pathList.Length;
        Vector3[] vertices = new Vector3[verticesLen];
        for (int i = 0; i < verticesLen; i++)
        {
            vertices[i] = pathList[i];
        }
        return vertices;
    }
}
