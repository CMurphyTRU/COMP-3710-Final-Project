using UnityEngine;

namespace Assets.Scripts.Creatures
{
    [System.Serializable]
    public class CreatureShapeDNA
    {
        public Vector2[] points;
        public float movementDelay;
        public float movementMagnitude;
        public float directionAngle;
        public Vector2 direction;

        public static float MinimumDelay = 0.5f;
        public static float MaximumDelay = 2f;
        public static float MinimumMagnitude = 2f;
        public static float MaximumMagnitude = 10f;

        public CreatureShapeDNA(int size, bool randomize = true)
        {
            if (randomize)
            {
                Randomize(size);
            }
        }
        public CreatureShapeDNA(string dnaString)
        {
            string[] genes = dnaString.Split(':');
            string[] pointGenes = genes[0].Split(",");
            string[] movementGenes = genes[1].Split(",");

            points = new Vector2[pointGenes.Length / 2];

            for (int i = 0; i < points.Length; i++)
            {
                points[i] = new Vector2(float.Parse(pointGenes[i * 2]), float.Parse(pointGenes[i * 2 + 1]));
            }

            movementDelay = float.Parse(movementGenes[0]);
            movementMagnitude = float.Parse(movementGenes[1]);
            directionAngle = float.Parse(movementGenes[2]);
            direction = new Vector2(float.Parse(movementGenes[3]), float.Parse(movementGenes[4]));
        }

        private void Randomize(int size)
        {

            movementDelay = Random.Range(MinimumDelay, MaximumDelay);
            movementMagnitude = Random.Range(MinimumMagnitude, MaximumMagnitude);
            directionAngle = Random.value * 2 * Mathf.PI;
            direction = generateDirection(directionAngle);

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
                if (i < points.Length - 1) dnaString += ",";
            }
            dnaString += string.Format(":{0:0.00},{1:0.00},{2:0.00},{3:0.00},{4:0.00}", movementDelay, movementMagnitude, directionAngle, direction.x, direction.y);
            return dnaString;
        }

        private Vector2 generateDirection(float theta)
        {
            Vector2 direction = Vector2.zero;

            theta %= 2 * Mathf.PI;

            direction.x = Mathf.Cos(theta);
            direction.y = Mathf.Sin(theta);
            return direction;
        }

        private void mutate()
        {
            float mutationRate = CreatureDNA.MutationRate;

            for (int i = 0; i < points.Length; i++)
            {
                if (MathUtils.rollOdds(mutationRate)) points[i].x = Mathf.Clamp(points[i].x + Random.Range(-0.1f, 0.1f), -1, 1);
                if (MathUtils.rollOdds(mutationRate)) points[i].y = Mathf.Clamp(points[i].y + Random.Range(-0.1f, 0.1f), -1, 1);
            }
            if (MathUtils.rollOdds(mutationRate)) movementDelay = Mathf.Clamp(movementDelay + Random.Range(-0.1f, 0.1f), MinimumDelay, MaximumDelay);
            if (MathUtils.rollOdds(mutationRate)) movementMagnitude = Mathf.Clamp(movementMagnitude + Random.Range(-0.1f, 0.1f), MinimumMagnitude, MaximumMagnitude);
            if (MathUtils.rollOdds(mutationRate))
            {
                directionAngle += Random.Range(-0.1f, 0.1f);
                direction = generateDirection(directionAngle);
            }

        }

        public static CreatureShapeDNA Crossover(CreatureShapeDNA mother, CreatureShapeDNA father)
        {
            int shapeSize = mother.points.Length;
            CreatureShapeDNA child = new CreatureShapeDNA(shapeSize, false);
            child.points = new Vector2[mother.points.Length];
            float crossoverBias = CreatureDNA.CrossoverBias;

            for (int i = 0; i < shapeSize; i++)
            {
                child.points[i] = MathUtils.rollOdds(crossoverBias) ? mother.points[i] : father.points[i];
            }

            child.movementDelay = MathUtils.rollOdds(crossoverBias) ? mother.movementDelay : father.movementDelay;
            child.movementMagnitude = MathUtils.rollOdds(crossoverBias) ? mother.movementMagnitude : father.movementMagnitude;
            child.direction = MathUtils.rollOdds(crossoverBias) ? mother.direction : father.direction;

            child.mutate();

            return child;
        }
    }
}
