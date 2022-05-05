using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using NavMeshBuilder = UnityEngine.AI.NavMeshBuilder;


namespace Core.NavMeshGeneration{
	// Build and update a localized navmesh from the sources marked by NavMeshSourceTag

	[DefaultExecutionOrder(-102)]
	public class LocalNavMeshBuilder : MonoBehaviour
	{
	    // The center of the build
        private Transform m_Tracked;

	    // The size of the build bounds
        private Vector3 m_Size = new Vector3(80.0f, 20.0f, 80.0f);

        public Vector3 Size
        {
            set{
                m_Size = value;
            }
        }

        private NavMeshData m_NavMeshData;
        private AsyncOperation m_Operation;
        private NavMeshDataInstance m_Instance;
        private List<NavMeshBuildSource> m_Sources = new List<NavMeshBuildSource>();

        private NavMeshBuildSettings GetBuildSettings()
        {
            var buildSettings = NavMesh.GetSettingsByID(0);
            buildSettings.agentHeight = 0.4f;
            //logic change settings here
            return buildSettings;
        }

	    void OnEnable()
	    {
	        // Construct and add navmesh
	        m_NavMeshData = new NavMeshData();
	        m_Instance = NavMesh.AddNavMeshData(m_NavMeshData);
	        if (m_Tracked == null) m_Tracked = transform;
	    }

	    void OnDisable()
	    {
	        // Unload navmesh and clear handle
	        m_Instance.Remove();
	    }

        /// <summary>
        /// update navmesh by size
        /// </summary>
        /// <param name="asyncUpdate">If set to <c>true</c> async update.</param>
	    public void UpdateNavMesh(bool asyncUpdate = false)
	    {
	        NavMeshSourceTag.Collect(ref m_Sources);
            var defaultBuildSettings = GetBuildSettings();
	        var bounds = QuantizedBounds();

	        if (asyncUpdate)
                m_Operation = NavMeshBuilder.UpdateNavMeshDataAsync(m_NavMeshData, defaultBuildSettings, m_Sources, bounds);
	        else
                NavMeshBuilder.UpdateNavMeshData(m_NavMeshData, defaultBuildSettings, m_Sources, bounds);
	    }

        /// <summary>
        /// update all navmesh depend on source
        /// </summary>
        /// <param name="asyncUpdate">If set to <c>true</c> async update.</param>
        public void UpdateAllNavMesh(bool asyncUpdate = false)
        {
            NavMeshSourceTag.Collect(ref m_Sources);
            var defaultBuildSettings = GetBuildSettings();
            var bounds = CalculateWorldBounds(m_Sources);

            if (asyncUpdate)
                m_Operation = NavMeshBuilder.UpdateNavMeshDataAsync(m_NavMeshData, defaultBuildSettings, m_Sources, bounds);
            else
                NavMeshBuilder.UpdateNavMeshData(m_NavMeshData, defaultBuildSettings, m_Sources, bounds);
        }

	    static Vector3 Quantize(Vector3 v, Vector3 quant)
	    {
            float x = quant.x * Mathf.Floor(v.x / quant.x);
            float y = quant.y * Mathf.Floor(v.y / quant.y);
            float z = quant.z * Mathf.Floor(v.z / quant.z);
            return new Vector3(x, y, z);
	    }

	    Bounds QuantizedBounds()
	    {
	        // Quantize the bounds to update only when theres a 10% change in size
	        var center = m_Tracked ? m_Tracked.position : transform.position;
            var bounds = new Bounds(Quantize(center, 0.1f * m_Size), m_Size);
            bounds.Expand(0.1f);
            return bounds;
	    }

        Bounds CalculateWorldBounds(List<NavMeshBuildSource> sources)
        {
            Matrix4x4 worldToLocal = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            worldToLocal = worldToLocal.inverse;

            var result = new Bounds();
            foreach (var src in sources)
            {
                switch (src.shape)
                {
                    case NavMeshBuildSourceShape.Mesh:
                        {
                            var m = src.sourceObject as Mesh;
                            result.Encapsulate(GetWorldBounds(worldToLocal * src.transform, m.bounds));
                            break;
                        }
                    case NavMeshBuildSourceShape.Terrain:
                        {
                            // Terrain pivot is lower/left corner - shift bounds accordingly
                            var t = src.sourceObject as TerrainData;
                            result.Encapsulate(GetWorldBounds(worldToLocal * src.transform, new Bounds(0.5f * t.size, t.size)));
                            break;
                        }
                    case NavMeshBuildSourceShape.Box:
                    case NavMeshBuildSourceShape.Sphere:
                    case NavMeshBuildSourceShape.Capsule:
                    case NavMeshBuildSourceShape.ModifierBox:
                        result.Encapsulate(GetWorldBounds(worldToLocal * src.transform, new Bounds(Vector3.zero, src.size)));
                        break;
                }
            }
            result.Expand(0.1f);
            return result;
        }

        static Bounds GetWorldBounds(Matrix4x4 mat, Bounds bounds)
        {
            var absAxisX = Abs(mat.MultiplyVector(Vector3.right));
            var absAxisY = Abs(mat.MultiplyVector(Vector3.up));
            var absAxisZ = Abs(mat.MultiplyVector(Vector3.forward));
            var worldPosition = mat.MultiplyPoint(bounds.center);
            var worldSize = absAxisX * bounds.size.x + absAxisY * bounds.size.y + absAxisZ * bounds.size.z;
            return new Bounds(worldPosition, worldSize);
        }

        static Vector3 Abs(Vector3 v)
        {
            return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
        }

        void OnDrawGizmosSelected()
        {
            if (m_NavMeshData)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(m_NavMeshData.sourceBounds.center, m_NavMeshData.sourceBounds.size);
            }

            Gizmos.color = Color.yellow;
            var bounds = QuantizedBounds();
            Gizmos.DrawWireCube(bounds.center, bounds.size);

            Gizmos.color = Color.green;
            var center = m_Tracked ? m_Tracked.position : transform.position;
            Gizmos.DrawWireCube(center, m_Size);
        }
	}
}
