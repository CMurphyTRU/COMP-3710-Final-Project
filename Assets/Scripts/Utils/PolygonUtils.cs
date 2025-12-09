using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

static public class PolygonUtils
{

    /*
     * Uses atan2 to sort a list of points around the center based on angles
     * in the clockwise direction.
     */
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

    /*
     * Uses atan2 to determine the smallest angle from the list relative to point
     */

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

    public static Vector3[] vector2ListToVector3List(Vector2[] vector2List, float offset)
    {
        Vector3[] vector3List = new Vector3[vector2List.Length];
        for (int i = 0; i < vector2List.Length; i++)
        {
            Vector2 currentElem = vector2List[i];
            vector3List[i] = new Vector3(currentElem.x + offset, currentElem.y + offset);
        }
        return vector3List;
    }

    // Offsets every point in a vector2 array by the center so that it is centered around it.
    public static void fixOrigin(Vector2[] pathList, Vector2 center)
    {
        for(int i = 0; i < pathList.Length; i++)
        {
            pathList[i] = pathList[i] - center;
        }
    }
}
