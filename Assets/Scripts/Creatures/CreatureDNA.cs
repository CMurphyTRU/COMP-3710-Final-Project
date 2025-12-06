using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using ColorUtility = UnityEngine.ColorUtility;

namespace Assets.Scripts.Creatures
{
    [System.Serializable]
    public class CreatureDNA
    {
        public static int ShapesPerCreature = 5;
        public static int PointsPerShape = 5;
        public int shapeCount;
        public int pointsPerShape;
        public Color colour;

        public CreatureShapeDNA[] shapes;
        public int[] joints;
        public int[] siblings;

        public CreatureDNA(bool randomize = true)
        {
            if (randomize)
            {
                Randomize();
            }
        }

        public CreatureDNA(string dnaString)
        {
            string[] genes = dnaString.Split('&');

            ColorUtility.TryParseHtmlString(genes[0], out colour);
            shapeCount = int.Parse(genes[1]);
            pointsPerShape = int.Parse(genes[2]);

            shapes = new CreatureShapeDNA[shapeCount];

            for(int i = 0; i < shapeCount; i++)
            {
                shapes[i] = new CreatureShapeDNA(genes[i + 3]);
            }
            int jointOffset = 3 + shapeCount;
            joints = dnaStringToIntList(genes[jointOffset]);
            int siblingOffset = jointOffset + 1;
            siblings = dnaStringToIntList(genes[siblingOffset]);
        }

        private void Randomize()
        {
            shapeCount = ShapesPerCreature;
            pointsPerShape = PointsPerShape;
            colour = Random.ColorHSV();
            shapes = generateShapes(shapeCount);
            joints = generateJoints();
            siblings = generateSiblings();
        }

        private CreatureShapeDNA[] generateShapes(int length)
        {
            CreatureShapeDNA[] result = new CreatureShapeDNA[length];
           for(int shapeIndex = 0; shapeIndex < length; shapeIndex++)
            {
                result[shapeIndex] = new CreatureShapeDNA(pointsPerShape);
            }
            return result;
        }

        private int[] generateJoints()
        {
            int[] result = new int[(shapeCount - 1) * 2];
            for(int i = 0; i < result.Length; i++)
            {
                result[i] = Random.Range(0, pointsPerShape - 1);
            }
            return result;
        }

        private int[] generateSiblings()
        {
            int[] result = new int[shapeCount - 1];
            for(int i = 0; i < result.Length; i++)
            {
                result[i] = Random.Range(0, i);
            }
            return result;
        }

        private string intListToDNAString(int[] list)
        {
            string result = string.Empty;
            for (int i = 0; i < list.Length; i++)
            {
                result += string.Format("{0:000}", list[i]);
                if (i < list.Length - 1) result += ",";
            }
            return result;
        }
        
        private int[] dnaStringToIntList(string dnaString)
        {
            string[] genes = dnaString.Split(',');
            int[] result = new int[genes.Length];

            for (int i = 0; i < genes.Length; i++)
            {
                result[i] = int.Parse(genes[i]);
            }
            return result;
        }

        public override string ToString()
        {
            string dnaString = $"{colour.ToHexString()}&{shapeCount}&{pointsPerShape}&";

            foreach (CreatureShapeDNA shapeDNA in shapes) dnaString += shapeDNA.ToString() + "&";

            dnaString += $"{intListToDNAString(joints)}&{intListToDNAString(siblings)}";
            return dnaString;
        }
        
    }
}