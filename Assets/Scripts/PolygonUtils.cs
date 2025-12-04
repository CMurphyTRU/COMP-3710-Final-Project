using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

static public class PolygonUtils
{
    public static Vector2[] sortClockwise(Vector2[] path, Vector2 center)
    {
        Vector2[] result = new Vector2[path.Length];
        List<Vector2> nextToSort = new List<Vector2>(path);
        
        for(int i = 0; i < path.Length; i++)
        {
            int indexOfSmallest = getSmallestAngleFromPoint(center, nextToSort);
            result[i] = nextToSort.ElementAt(indexOfSmallest);
            nextToSort.RemoveAt(indexOfSmallest);
        }


        return result;
    }

    private static int getSmallestAngleFromPoint(Vector2 point, List<Vector2> vectorList)
    {
        float angle = 2*Mathf.PI; // Start at the highest possible angle
        int smallest = 0; // Assume index 0 is the smallest to start

        for(int i = 0; i < vectorList.Count; i++)
        {
            Vector2 currentVector = vectorList[i];
            float currAngle = Mathf.Atan2(currentVector.y - point.y, currentVector.x - point.x);

            if (currAngle < angle)
            {
                smallest = i;
                angle = currAngle;
            }
        }


        return smallest;
    }

}
