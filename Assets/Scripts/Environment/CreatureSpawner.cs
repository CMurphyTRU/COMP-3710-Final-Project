using UnityEngine;
using System.Collections;

public class CreatureSpawner : MonoBehaviour
{
    public GameObject creaturePrefab;

    void Start()
    {
        StartCoroutine(SpawnAfterDelay());
    }

    IEnumerator SpawnAfterDelay()
    {
        
        yield return new WaitForSeconds(0.1f);

        TerrainGenerator[] terrains = FindObjectsOfType<TerrainGenerator>();
        Debug.Log("Terrains found: " + terrains.Length);

        foreach (TerrainGenerator terrain in terrains)
        {
            SpawnCreatureForTerrain(terrain);
        }
    }

    void SpawnCreatureForTerrain(TerrainGenerator terrain)
    {
        PolygonCollider2D poly = terrain.GetComponent<PolygonCollider2D>();

        
        float actualLeft = GetLeftSurfaceX(terrain);

        
        float intendedLeft = terrain.transform.position.x;

        
        float offset = actualLeft - intendedLeft;

        
        float spawnX = intendedLeft - offset - 40f;
        float spawnY = poly.bounds.max.y + 5f;

        Vector3 spawnPos = new Vector3(spawnX, spawnY, 0);

        Debug.Log($"Terrain {terrain.name} - actualLeft={actualLeft}, intendedLeft={intendedLeft}, offset={offset}, spawnX={spawnX}");

        
        GameObject creature = Instantiate(creaturePrefab);

        
        creature.transform.position = spawnPos;
    }

   
    float GetLeftSurfaceX(TerrainGenerator terrain)
    {
        PolygonCollider2D poly = terrain.GetComponent<PolygonCollider2D>();
        int count = poly.points.Length;

        float minX = float.MaxValue;

        
        int half = count / 2;

        for (int i = 0; i < half; i++)
        {
            Vector3 world = terrain.transform.TransformPoint(poly.points[i]);

            if (world.x < minX)
                minX = world.x;
        }

        return minX;
    }
}
