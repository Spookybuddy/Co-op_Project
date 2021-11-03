using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureFix : MonoBehaviour
{
    private Mesh mesh;
    private Material mat;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
        mesh = GetComponent<MeshFilter>().mesh;

        //Auto set tag to Ground
        gameObject.tag = "Ground";

        //Set texture size to scale to repeat properly
        mat.mainTextureScale = new Vector2(mesh.bounds.size.x * transform.localScale.x / 2, 1);
    }
}
