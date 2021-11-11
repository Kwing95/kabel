using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeManager : MonoBehaviour
{

    // Draw a single line between two points
    public static GameObject DrawLine(Vector2 shotOrigin, Vector2 hitPoint, Material material = null, Transform parent = null, float width = 0.07f)
    {
        GameObject shotObject = new GameObject();
        if (parent != null)
            shotObject.transform.parent = parent;

        LineRenderer shotLine = shotObject.AddComponent<LineRenderer>();
        shotLine.SetPositions(new Vector3[] { (Vector3)shotOrigin - Globals.EPSILON * Vector3.forward, (Vector3)hitPoint - Globals.EPSILON * Vector3.forward });

        shotLine.startWidth = shotLine.endWidth = width;
        shotLine.sortingLayerName = "Effects";
        shotLine.material = (material == null ? Globals.BRIGHT_WHITE : material);
        // shotLine.startWidth = shotLine.endWidth = 0.07f;

        return shotObject;
    }

    // Draw a circle given a center and radius
    public static GameObject DrawCircle(Vector2 center, float radius, Material material, int segments = 50, float width = 0.07f)
    {
        GameObject circleObject = new GameObject();
        LineRenderer line = circleObject.AddComponent<LineRenderer>();
        line.material = material;
        line.startWidth = line.endWidth = width;
        line.positionCount = segments + 1;
        line.sortingLayerName = "Effects";

        float angle = 20f;

        for (int i = 0; i < (segments + 1); i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius; // xRadius for ellipse
            float y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

            line.SetPosition(i, new Vector3(center.x + x, center.y + y, -Globals.EPSILON));
            angle += (360f / segments);
        }

        return circleObject;
    }

}
