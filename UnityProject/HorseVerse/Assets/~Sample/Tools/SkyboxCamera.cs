using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SkyboxCamera : MonoBehaviour
{
    public Camera m_camera;
    public int Texture_size = 256;
    public string FileName = "Skybox";


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Capture(FileName);
        }
    }

    void Capture(string path)
    {
#if UNITY_EDITOR
        Cubemap cubemap = new Cubemap(Texture_size, TextureFormat.ARGB32, true);
        cubemap.name = path;
        m_camera.RenderToCubemap(cubemap);

        AssetDatabase.CreateAsset(
          cubemap,
          $"Assets/Textures/Skybox/{path}.cubemap"
        );
#endif
    }
}
