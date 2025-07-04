using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectHelp : MonoBehaviour
{    public static bool IsOverlapping(RectTransform rectA, RectTransform rectB)
    {
        if (rectA == null || rectB == null)
        {
            return false;
        }

        Vector3[] cornersA = new Vector3[4];
        Vector3[] cornersB = new Vector3[4];

        rectA.GetWorldCorners(cornersA);
        rectB.GetWorldCorners(cornersB);

        Rect rect1 = new Rect(cornersA[0], cornersA[2] - cornersA[0]);
        Rect rect2 = new Rect(cornersB[0], cornersB[2] - cornersB[0]);

        return rect1.Overlaps(rect2);
    }
}
