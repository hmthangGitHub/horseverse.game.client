using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SkyboxCamera : MonoBehaviour
{
    const int TEXTURE_SIZE = 256;
    public Camera m_camera;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Capture();
        }
    }

    void Capture()
    {
#if UNITY_EDITOR
        Cubemap cubemap = new Cubemap(TEXTURE_SIZE, TextureFormat.ARGB32, true);
        cubemap.name = "Skybox";
        m_camera.RenderToCubemap(cubemap);

        AssetDatabase.CreateAsset(
          cubemap,
          "Assets/Textures/Skybox/Skybox.cubemap"
        );
#endif
    }
}
