using UnityEditor.ShortcutManagement;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(PolygonCollider2D))]
public class TerrainGenerator : MonoBehaviour
{
    public TerrainDNA dna;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private PolygonCollider2D collider2D;

    [HideInInspector]
    public float pointSpacing;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        collider2D = GetComponent<PolygonCollider2D>();
    }

    public void GenerateFromDNA(TerrainDNA dna)
    {
        this.dna = dna;
        
        Camera cam = Camera.main;
        float visibleWidth = cam.orthographicSize * 2f * cam.aspect;
        pointSpacing = visibleWidth / (dna.pointCount - 1);

        

        Vector2[] points = GeneratePoints();
        Mesh mesh = BuildMesh(points);

        meshFilter.mesh = mesh;

        collider2D.pathCount = 1;
        collider2D.SetPath(0, BuildColliderPath(points));

        meshRenderer.material.color = dna.groundColor;

        
    }

    private Vector2[] GeneratePoints()
    {
    Vector2[] pts = new Vector2[dna.pointCount];

    float currentY = 0f;
    float halfWidth = (dna.pointCount - 1) * pointSpacing * 0.5f;

    for (int i = 0; i < dna.pointCount; i++)
    {
        currentY += dna.relativeHeights[i];
        currentY = Mathf.Lerp(currentY, 0, dna.smoothness * 0.01f);

        float x = i * pointSpacing - halfWidth;

        pts[i] = new Vector2(x, currentY);
    }

    return pts;
    }


    private Mesh BuildMesh(Vector2[] points)
    {
        Mesh mesh = new Mesh();

        int vertCount = points.Length + 2;

        Vector3[] verts = new Vector3[vertCount];
        int[] tris = new int[(points.Length - 1) * 6];

        for (int i = 0; i < points.Length; i++)
        {
            verts[i] = points[i];
        }

        verts[points.Length] = new Vector3(points[0].x, -10, 0);
        verts[points.Length + 1] = new Vector3(points[points.Length - 1].x, -10, 0);

        int t = 0;
        for (int i = 0; i < points.Length - 1; i++)
        {
            tris[t++] = i;
            tris[t++] = i + 1;
            tris[t++] = points.Length;

            tris[t++] = i + 1;
            tris[t++] = points.Length + 1;
            tris[t++] = points.Length;
        }

        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;

    }

    private Vector2[] BuildColliderPath(Vector2[] points)
    {
        Vector2[] path = new Vector2[points.Length + 2];

        for (int i = 0; i < points.Length; i++)
        {
            path[i] = points[i];
        }

        path[points.Length] = new Vector2(points[points.Length - 1].x, -10);
        path[points.Length + 1] = new Vector2(points[0].x, -10);

        return path;
    }

    public void SetAlpha(float alpha)
    {
        Color c = meshRenderer.material.color;
        c.a = alpha;
        meshRenderer.material.color = c;
    }

    
}
