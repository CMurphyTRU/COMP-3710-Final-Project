using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(PolygonCollider2D))]
public class TerrainGenerator : MonoBehaviour
{
    public TerrainDNA dna;                        // DNA controlling terrain shape
    private MeshFilter meshFilter;                // mesh component
    private MeshRenderer meshRenderer;            // renderer for the mesh
    private PolygonCollider2D collider2D;         // collider outlining the terrain

    [HideInInspector]
    public float pointSpacing;                    // spacing between each terrain point

    [SerializeField] private float bottomY = -10f; // bottom of terrain mesh/collider

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        collider2D = GetComponent<PolygonCollider2D>();
    }

    public void GenerateFromDNA(TerrainDNA dna)
    {
        this.dna = dna;

        // calculate spacing based on camera width
        Camera cam = Camera.main;
        float visibleWidth = cam.orthographicSize * 2f * cam.aspect;
        pointSpacing = visibleWidth / (dna.pointCount - 1);

        // generate top surface of terrain
        Vector2[] surfacePoints = GeneratePoints();

        // build mesh from points
        Mesh mesh = BuildMesh(surfacePoints);
        meshFilter.mesh = mesh;

        // build collider outline
        Vector2[] colliderPath = BuildColliderPath(surfacePoints);
        collider2D.pathCount = 1;
        collider2D.SetPath(0, colliderPath);

        // apply terrain color
        meshRenderer.material.color = dna.groundColor;
    }

    private Vector2[] GeneratePoints()
    {
        Vector2[] pts = new Vector2[dna.pointCount];

        float currentY = 0f;
        float halfWidth = (dna.pointCount - 1) * pointSpacing * 0.5f;

        // accumulate height changes from DNA to form terrain surface
        for (int i = 0; i < dna.pointCount; i++)
        {
            currentY += dna.relativeHeights[i];
            currentY = Mathf.Lerp(currentY, 0f, dna.smoothness * 0.01f);

            float x = i * pointSpacing - halfWidth;
            pts[i] = new Vector2(x, currentY);
        }

        return pts;
    }

    private Mesh BuildMesh(Vector2[] points)
    {
        Mesh mesh = new Mesh();

        int n = points.Length;

        // create top and bottom vertices
        Vector3[] verts = new Vector3[n * 2];
        int[] tris = new int[(n - 1) * 6];

        for (int i = 0; i < n; i++)
        {
            verts[i] = new Vector3(points[i].x, points[i].y, 0f);      // top vertex
            verts[n + i] = new Vector3(points[i].x, bottomY, 0f);     // bottom vertex
        }

        // build triangles between top and bottom vertices
        int t = 0;
        for (int i = 0; i < n - 1; i++)
        {
            int tl = i;
            int tr = i + 1;
            int bl = n + i;
            int br = n + i + 1;

            tris[t++] = tl;
            tris[t++] = tr;
            tris[t++] = bl;

            tris[t++] = tr;
            tris[t++] = br;
            tris[t++] = bl;
        }

        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }

    private Vector2[] BuildColliderPath(Vector2[] points)
    {
        int n = points.Length;
        Vector2[] path = new Vector2[n * 2];

        // top surface of collider
        for (int i = 0; i < n; i++)
        {
            path[i] = points[i];
        }

        // bottom edge going back in reverse, forming a closed shape
        for (int i = 0; i < n; i++)
        {
            int topIndex = n - 1 - i;
            path[n + i] = new Vector2(points[topIndex].x, bottomY);
        }

        return path;
    }

    public void SetAlpha(float alpha)
    {
        // adjust the transparency of the terrain material
        Color c = meshRenderer.material.color;
        c.a = alpha;
        meshRenderer.material.color = c;
    }
}
