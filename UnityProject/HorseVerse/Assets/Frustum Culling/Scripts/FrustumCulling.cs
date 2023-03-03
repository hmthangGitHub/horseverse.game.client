using UnityEngine;
using FrustumCullingSpace;

[DisallowMultipleComponent]
public class FrustumCulling : MonoBehaviour
{
    public bool autoCatchCamera = true;                                    //set whether camera should be caught at run-time (decreases performance if used by many objects)
    public Camera mainCam;                                                 //active scene camera

    public float cameraLeftPad = -0.3f;                                    //object must surpass padding to the left to deactivate
    public float cameraRightPad = 1.3f;                                    //...................................right......
    public float cameraTopPad = -0.3f;                                     //...................................top........           
    public float cameraBottomPad = 1.3f;                                   //...................................bottom.....
    
    public bool autoBuildObjects = true;                                   //automatically cache the objects on script awake
    
    public bool distanceCull = false;                                      //flag whether culling should also take distance into consideration
    public float distanceToCull = 0f;                                      //the distance if exceeded will cull the object
    public bool prioritizeDistance = false;                                //flag whether distance should be top priority in distance culling
    public bool distanceCullOnly = false;                                  //flag whether only distance culling should be enabled
    
    GameObject go;                                                         //the dynamically-built game object that will replace the object

    void Awake()
    {
        //get active camera on script awake if set to do so
        if (autoCatchCamera) mainCam = Camera.main;
        if (autoBuildObjects) BuildObjects();
    }

    //builds the objects' positions
    public void BuildObjects()
    {
        GameObject parentGO = GameObject.Find("FrustumCullingParent");
        if (parentGO == null) {
            GameObject parentTemp = new GameObject();
            parentTemp.name = "FrustumCullingParent";
            parentGO = parentTemp;
        }

        go = new GameObject();
        go.name = "FrustumCullingObject";
        go.transform.parent = parentGO.transform;

        go.transform.position = transform.position;
        if (go.GetComponent<FrustumCullingObject>() == null) go.AddComponent<FrustumCullingObject>();
        
        FrustumCullingObject GoScript = go.GetComponent<FrustumCullingObject>();
        CopyProperties(GoScript);
    }

    //copy properties to secondary script
    public void CopyProperties(FrustumCullingObject fco)
    {
        fco.mainObject = transform.gameObject;
        fco.mainScript = this;

        fco.cameraLeftPad = cameraLeftPad;
        fco.cameraRightPad = cameraRightPad;
        fco.cameraTopPad = cameraTopPad;
        fco.cameraBottomPad = cameraBottomPad;

        fco.distanceCull = distanceCull;
        fco.prioritizeDistance = prioritizeDistance;
        fco.distanceToCull = distanceToCull;
        fco.distanceCullOnly = distanceCullOnly;

        fco.autoCatchCamera = autoCatchCamera;
        if (!autoCatchCamera) fco.mainCam = mainCam;
    }
}
