using Assets.Scripts.Creatures;
using UnityEngine;


[RequireComponent(typeof(MeshRenderer))]
public class CreatureGenerator : MonoBehaviour
{
    public CreatureDNA dna;
    public GameObject[] shapes;
    private MeshRenderer meshRenderer;
    public PhysicsMaterial2D physicsMaterial;
    public Vector3 goalPosition;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void generateFromDNA(CreatureDNA dna)
    {
        this.dna = dna;
        shapes = new GameObject[dna.shapeCount];
        generateShapes();
        joinShapes();
    }

    /*
     * generateShapes() :
     * Converts the CreatureShapeDNA in dna into a GameObject
     */
    private void generateShapes()
    {
        meshRenderer.material.color = dna.colour;
        for (int i = 0; i < dna.shapes.Length; i++)
        {
            // Create GameObject for Shape
            CreatureShapeDNA shapeDNA = dna.shapes[i];
            GameObject shape = new GameObject($"CreaturePart{i}");
            shape.transform.parent = transform;
            shape.transform.position = transform.position;

            // Add Shape rendering
            MeshRenderer shapeRenderer = shape.AddComponent<MeshRenderer>();
            MeshFilter shapeFilter = shape.AddComponent<MeshFilter>();
            Mesh mesh = generateShapeMesh(shapeDNA);

            shapeRenderer.material = meshRenderer.material;
            shapeFilter.mesh = mesh;
            shapes[i] = shape;

            // Add Shape physics components
            PolygonCollider2D collider = shape.AddComponent<PolygonCollider2D>();
            Rigidbody2D rigidbody = shape.AddComponent<Rigidbody2D>();
            collider.SetPath(0, shapeDNA.points);
            shape.layer = gameObject.layer;
            collider.sharedMaterial = physicsMaterial;
        }
        CreatureEvaluator creatureEval = shapes[0].AddComponent<CreatureEvaluator>();
        creatureEval.finishPosition = goalPosition;
        
    }

    /*
     *  Rendering methods
     */
    private Mesh generateShapeMesh(CreatureShapeDNA shapeDNA)
    {
        Mesh mesh = new Mesh();
        Vector2[] pathList = shapeDNA.points;

        // Populating mesh with vertices and triangles
        mesh.vertices = createVertices(pathList);
        mesh.triangles = createTriangles(pathList);
        return mesh;
    }
    private int[] createTriangles(Vector2[] pathList)
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

    private Vector3[] createVertices(Vector2[] pathList)
    {
        int verticesLen = pathList.Length;
        Vector3[] vertices = new Vector3[verticesLen];
        for (int i = 0; i < verticesLen; i++)
        {
            vertices[i] = pathList[i];
        }
        return vertices;
    }

    /*
     *  Physics methods
     */

    private void joinShapes()
    {
        for(int i = 0; i < shapes.Length - 1; i++)
        {
            GameObject shape = shapes[i+1];
            GameObject sibling = shapes[dna.siblings[i]];
            Vector2[] shapePoints = shape.GetComponent<PolygonCollider2D>().GetPath(0);
            Vector2[] siblingPoints = sibling.GetComponent<PolygonCollider2D>().GetPath(0);

            int jointIndex = dna.joints[i * 2];
            int siblingJointIndex = dna.joints[i * 2 + 1];

            Vector2 anchorPoint = shapePoints[jointIndex];
            Vector2 siblingPoint = siblingPoints[siblingJointIndex];

            HingeJoint2D joint = shape.AddComponent<HingeJoint2D>();
            shape.transform.position = sibling.transform.position + vector2To3(siblingPoint) - vector2To3(anchorPoint);
            joint.anchor = anchorPoint;
            joint.connectedBody = sibling.GetComponent<Rigidbody2D>();

            CreatureShapeDNA shapeDNA = dna.shapes[i + 1];
            shape.AddComponent<ShapeMovement>().EnableMovement(shapeDNA.movementDelay, shapeDNA.movementMagnitude, shapeDNA.direction);
            
        }
    }

    /*
     *  Util methods
     */
    private Vector3 vector2To3(Vector2 vector)
    {
        return vector;
    }
}
