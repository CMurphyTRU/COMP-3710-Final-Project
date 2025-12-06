using UnityEngine;

namespace Assets.Scripts.Creatures
{
    [System.Serializable]
    public class CreatureShapeDNA
    {
        public Vector2[] points;
        public float movementDelay;
        public int forceMagnitude;
        public Vector2 forceDirection;

        public static float MIN_DELAY = 0.5f;
        public static float MAX_DELAY = 3f;
        public static int MIN_MAGNITUDE = 100;
        public static int MAX_MAGNITUDE = 300;
        public static Vector2 MAX_DIR = Vector2.one;
        public CreatureShapeDNA(int size)
        {
            Randomize(size);
        }
        public CreatureShapeDNA(string dnaString)
        {
            string[] genes = dnaString.Split(':');
            string[] pointGenes = genes[0].Split(",");
            string[] movementGenes = genes[1].Split(",");

            points = new Vector2[pointGenes.Length / 2];

            for(int i = 0; i < points.Length; i++)
            {
                points[i] = new Vector2(float.Parse(pointGenes[i * 2]), float.Parse(pointGenes[i * 2 + 1]));
            }

            movementDelay = float.Parse(movementGenes[0]);
            forceMagnitude = int.Parse(movementGenes[1]);
            forceDirection = new Vector2(float.Parse(movementGenes[2]), float.Parse(movementGenes[3]));
        }

        private void Randomize(int size)
        {

            movementDelay = Random.Range(MIN_DELAY, MAX_DELAY);
            forceMagnitude = Random.Range(MIN_MAGNITUDE, MAX_MAGNITUDE);
            forceDirection = new Vector2(Random.Range(0.1f, MAX_DIR.x), Random.Range(0.1f, MAX_DIR.y));

            points = new Vector2[size];
            float xTotal = 0;
            float yTotal = 0;

            for (int i = 0; i < size; i++)
            {
                float x = Random.value * 2 - 1;
                float y = Random.value * 2 - 1;
                points[i].x = x;
                points[i].y = y;
                xTotal += x;
                yTotal += y;
            }
            Vector2 pathCenter = new Vector2(xTotal / size, yTotal / size);
            PolygonUtils.fixOrigin(points, pathCenter);
            points = PolygonUtils.sortClockwise(points, pathCenter);
        }
    public override string ToString()
        {
            string dnaString = "";

            for (int i = 0; i < points.Length; i++)
            {
                dnaString += string.Format("{0:0.00},{1:0.00}", points[i].x, points[i].y);
                if(i < points.Length - 1) dnaString += ",";
            }
            dnaString += string.Format(":{0:0.00},{1:000},{2:0.00},{3:0.00}", movementDelay, forceMagnitude, forceDirection.x, forceDirection.y);
            return dnaString;
        }
    }
}
