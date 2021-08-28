using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Пребразователь UV материалов для постоянного масштабирования (не зависящего от размеров куба)
public class UVConverter : MonoBehaviour
{
    private static int[] indexX = new int[8] { 16, 17, 18, 19, 20, 21, 22, 23 };
    private static int[] indexY = new int[8] { 4, 5, 8, 9, 12, 13, 14, 15 };
    private const float pixelsPerUnit = 409.6f; //пикселей на игровую единицу
    private static Vector3[] scaleParameter = new Vector3[24]
    {
        new Vector3(1, -1, 0),
        new Vector3(1, -1, 0),
        new Vector3(1, -1, 0),
        new Vector3(1, -1, 0),
        new Vector3(1, 0, 1),
        new Vector3(1, 0, 1),
        new Vector3(1, 1, 0),
        new Vector3(1, 1, 0),
        new Vector3(1, 0, 1),
        new Vector3(1, 0, 1),
        new Vector3(1, 1, 0),
        new Vector3(1, 1, 0),
        new Vector3(1, 0, -1),
        new Vector3(1, 0, -1),
        new Vector3(1, 0, -1),
        new Vector3(1, 0, -1),
        new Vector3(0, -1, 1),
        new Vector3(0, -1, 1),
        new Vector3(0, -1, 1),
        new Vector3(0, -1, 1),
        new Vector3(0, -1, -1),
        new Vector3(0, -1, -1),
        new Vector3(0, -1, -1),
        new Vector3(0, -1, -1)
    };

    public static Vector3 elementwiseMultiply(Vector3 v1, Vector3 v2)
    {
        return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
    }

    // Start is called before the first frame update
    void Start()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector2[] uvs = mesh.uv;
        int size = GetComponent<Renderer>().material.mainTexture.width;
        for (int i = 0; i < 24; i++)
        {
            Vector3 offset = elementwiseMultiply((transform.position + elementwiseMultiply(
                transform.localScale, scaleParameter[i]) / 2) * pixelsPerUnit / size - Vector3.one / 2, -scaleParameter[i]) * size / pixelsPerUnit;
            if (indexX.Contains(i))
                uvs[i] = new Vector2(uvs[i].x * transform.localScale.z + offset.z, uvs[i].y * transform.localScale.y + offset.y) * pixelsPerUnit / size;
            else if (indexY.Contains(i))
                uvs[i] = new Vector2(uvs[i].x * transform.localScale.x + offset.x, uvs[i].y * transform.localScale.z + offset.z) * pixelsPerUnit / size;
            else
                uvs[i] = new Vector2(uvs[i].x * transform.localScale.x + offset.x, uvs[i].y * transform.localScale.y + offset.y) * pixelsPerUnit / size;
        }

        mesh.uv = uvs;
    }
}
