using PathCreation;
using UnityEngine;

public class TrainingPathBridge : MonoBehaviour
{
    [SerializeField] private Transform bridgeTrigger;
    
    [SerializeField] private PathCreator destinationPath;
    [SerializeField] private MeshPathContainer.PathType destinationPathType;
    [SerializeField] private PathCreator sourcePath;
    [SerializeField] private PathCreator bridge;
    
    
    public float middlePointHeight = 5.0f;
    public float lowerPointHeight = 1.0f;

    public PathCreator DestinationPath
    {
        get => destinationPath;
        set => destinationPath = value;
    }

    public PathCreator SourcePath
    {
        get => sourcePath;
        set => sourcePath = value;
    }

    public MeshPathContainer.PathType DestinationPathType
    {
        get => destinationPathType;
        set => destinationPathType = value;
    }

    public PathCreator Bridge
    {
        get => bridge;
        set => bridge = value;
    }

    [ContextMenu("CreateBridge")]
    public void CreateBridge()
    {
        var bridgePosition = transform.position;
        Bridge.InitializeEditorData(false);
        var sourcePoint = sourcePath.bezierPath[sourcePath.bezierPath.NumPoints - 1] + sourcePath.transform.position;
        Bridge.bezierPath.SetPoint(0, sourcePoint - bridgePosition);
        
        var destinationPoint = destinationPath.bezierPath[0] + destinationPath.transform.position;
        Bridge.bezierPath.SetPoint(Bridge.bezierPath.NumPoints - 1, destinationPoint - bridgePosition);

        var middlePoint = Vector3.Lerp(sourcePoint, destinationPoint, 0.5f);
        middlePoint = new Vector3(middlePoint.x, middlePointHeight + destinationPoint.y, middlePoint.z);
        Bridge.bezierPath.SetPoint(3, middlePoint - bridgePosition);

        var lowerPoint = Vector3.Lerp(middlePoint, destinationPoint, 0.5f);// - Vector3.up * lowerPointHeight;
        Bridge.bezierPath.SetPoint(6, lowerPoint - bridgePosition);
        
        var temp = Bridge.bezierPath.AutoControlLength;
        Bridge.bezierPath.AutoControlLength = temp * 1.1f;
        Bridge.bezierPath.AutoControlLength = temp;

        bridgeTrigger.position = sourcePoint;
    }
}
