using System.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class CreatureFeature : MonoBehaviour
{

    [SerializeField] private PolygonCollider2D collider;
    [SerializeField] private Rigidbody2D rigidBody;
    [SerializeField] private Mesh mesh;
    [SerializeField] private Material material;
    [SerializeField] private Vector2[] pathList;
    [SerializeField] private Vector2 pathCenter;
    [SerializeField] public string gene { get; set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        collider = this.gameObject.AddComponent<PolygonCollider2D>();
        rigidBody = this.gameObject.AddComponent<Rigidbody2D>();

        generatePath();
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

        }
        pathCenter = new Vector2(xTotal / size, yTotal / size);
        pathList = PolygonUtils.sortClockwise(pathList, pathCenter);
        collider.SetPath(0, pathList);
    }

    private void generateMesh()
    {
        MeshRenderer renderer = this.gameObject.AddComponent<MeshRenderer>();
        MeshFilter filter = this.gameObject.AddComponent<MeshFilter>();

        mesh = new Mesh();
        material.color = Color.red;

        filter.mesh = mesh;
        renderer.material = material;

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
