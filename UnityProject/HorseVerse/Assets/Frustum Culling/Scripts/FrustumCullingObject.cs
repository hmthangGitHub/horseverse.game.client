using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrustumCullingSpace
{
    public class FrustumCullingObject : MonoBehaviour
    {
        [HideInInspector] public GameObject mainObject;
        [HideInInspector] public float cameraLeftPad,
        cameraRightPad,
        cameraTopPad,
        cameraBottomPad;

        [HideInInspector] public bool distanceCull;
        [HideInInspector] public float distanceToCull;
        [HideInInspector] public bool prioritizeDistance;
        [HideInInspector] public bool distanceCullOnly;

        [HideInInspector] public bool autoCatchCamera;
        [HideInInspector] public Camera mainCam;
        [HideInInspector] public FrustumCulling mainScript;

        bool turnedOff;
        int frames = 0;
        int distanceFrames = 0;
        bool distanceOk = false;

        [SerializeField] private List<Renderer> renderers = new List<Renderer>(); 

        void Start()
        {
            if (autoCatchCamera)
            {
                if (CameraActiveHandler.mainCamera != default)
                    mainCam = CameraActiveHandler.mainCamera;
                else
                {
                    //mainCam = Camera.main;
                    StartCoroutine(doWaitForCamera());
                }
            }

            InitRenderes();
        }

        IEnumerator doWaitForCamera()
        {
            float time = 0;
            while (time < 50.0f)
            {
                if (CameraActiveHandler.mainCamera != default)
                {
                    mainCam = CameraActiveHandler.mainCamera;
                    break;
                }
                time += Time.deltaTime;
                yield return new WaitForSeconds(0.5f);
            }
        }

        void LateUpdate()
        {
            if (mainScript != null) mainScript.CopyProperties(this);
            else {
                Destroy(gameObject);
                return;
            }

            frames++;
            if (distanceCull) distanceFrames++;

            //run once every 4 frames
            if (frames >= 10 && !distanceCullOnly) {
                frames = 0;
                
                gameObject.transform.position = mainObject.transform.position;
                gameObject.transform.rotation = mainObject.transform.rotation;

                if (mainCam != null) {
                    //change object to view port position and check whether it's within the camera padding or not
                    //Vector3 screenPoint = mainCam.WorldToViewportPoint(transform.position);
                    //bool onScreen = screenPoint.z > 0f && screenPoint.x > cameraLeftPad && screenPoint.x < cameraRightPad && screenPoint.y > cameraTopPad && screenPoint.y < cameraBottomPad;

                    bool onScreen = IsVisibleInView(mainCam, renderers);

                    if (distanceCull) {
                        if (prioritizeDistance) {
                            if (onScreen && distanceOk) {
                                if (turnedOff) EnableObject();
                            }else{
                                if (!turnedOff) DisableObject();
                            }
                        }else{
                            if (onScreen) {
                                if (turnedOff && distanceOk) EnableObject();
                            }else{
                                if (!turnedOff) DisableObject();
                            }
                        }
                    }else{
                        if (onScreen) {
                            if (turnedOff) EnableObject();
                        }else{
                            if (!turnedOff) DisableObject();
                        }
                    }
                    
                }else{
                    Debug.LogWarning("No game camera set");
                }
            }

            //check distance every 7 frames
            if (distanceFrames >= 7) {
                distanceFrames = 0;
                float distance = (mainCam.transform.position - transform.position).sqrMagnitude;
                
                if (distance < distanceToCull * distanceToCull) {
                    distanceOk = true;
                }else{
                    distanceOk = false;
                }

                if (distanceCullOnly) {
                    if (distanceOk) EnableObject();
                    else DisableObject();
                }
            }
        }

        void DisableObject()
        {
            //mainObject.SetActive(false);
            for (int i = 0; i < renderers.Count; i++)
            {
                renderers[i].enabled = false;
            }
            turnedOff = true;
        }

        void EnableObject()
        {
            //mainObject.SetActive(true);
            for (int i = 0; i < renderers.Count; i++)
            {
                renderers[i].enabled = true;
            }
            turnedOff = false;
        }

        void InitRenderes()
        {
            renderers.Clear();
            renderers.AddRange(mainObject.GetComponents<Renderer>());
            renderers.AddRange(mainObject.GetComponentsInChildren<Renderer>());
        }

        bool IsVisibleInView(Camera cam, List<Renderer> _renderers)
        {
            

            var planes = GeometryUtility.CalculateFrustumPlanes(cam);
            for (int i = 0; i < _renderers.Count; i++)
            {
                if (GeometryUtility.TestPlanesAABB(planes, _renderers[i].bounds)) return true;
            }
            return false;
        }
    }
}