using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlatformGeneratorTest : MonoBehaviour
{
   public GameObject platformPrefab;
   public GameObject lastPlatform;
   public HorseTrainingControllerV2 horseTrainingControllerV2;
   private readonly Queue<GameObject> platformQueue = new Queue<GameObject>();
   private bool isFirstJump = true;
   private MasterHorseTrainingProperty masterHorseTrainingProperty;

   public void SetMasterHorseTrainingProperty(MasterHorseTrainingProperty masterHorseTrainingProperty)
   {
      this.masterHorseTrainingProperty = masterHorseTrainingProperty;
      lastPlatform = platformPrefab;
      for (var i = 0; i < 4; i++)
      {
         Generate();
      }
   }
   
   private void Generate()
   {
      var relativePointToPlayer = PredictRelativePointToPlayer();
      var platformTest = lastPlatform.GetComponent<PlatformTest>();
      var lastEndPosition = platformTest.end.position;
      var platform = CreateNewPlatform(relativePointToPlayer, lastEndPosition);
      lastPlatform = platform;
      platformQueue.Enqueue(platform);
   }

   private Vector3 PredictRelativePointToPlayer()
   {
      var highestPoint = PredictHighestPoint();

      var timeToReach = Random.Range(horseTrainingControllerV2.AirTime.x, horseTrainingControllerV2.AirTime.y);
      var relativePointToPlayer = PredictDownPoint(Random.Range(0.0f, horseTrainingControllerV2.ForwardVelocity * timeToReach)) 
                                  + highestPoint + Vector3.right * (horseTrainingControllerV2.HorizontalVelocity * Random.Range(-0.4f, 0.4f) * timeToReach);
      return relativePointToPlayer;
   }

   private GameObject CreateNewPlatform(Vector3 relativePointToPlayer, Vector3 lastEndPosition)
   {
      var platform = Instantiate(platformPrefab, this.transform);
      var platformTest = platform.GetComponent<PlatformTest>();
      platformTest.GenerateBlocks(relativePointToPlayer,
         lastEndPosition,
         masterHorseTrainingProperty.BlockPadding,
         masterHorseTrainingProperty.BlockSpacing,
         masterHorseTrainingProperty.BlockNumbersMin,
         masterHorseTrainingProperty.BlockNumbersMax);
      platformTest.OnJump += OnJump;
      return platform;
   }

   private void OnJump()
   {
      if (isFirstJump)
      {
         isFirstJump = false;
      }
      else
      {
         Destroy(platformQueue.Dequeue());
      }
      Generate();
   }

   private void CreateDebugSphere(Vector3 position)
   {
#if UNITY_EDITOR
      var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
      sphere.transform.position = position;
      sphere.transform.localScale = Vector3.one * 0.1f;
      sphere.GetComponent<Collider>().enabled = false;
#endif
   }

   private Vector3 PredictDownPoint(float z)
   {
      var angle = 0;
      var y = (Mathf.Tan(angle)) * z + (horseTrainingControllerV2.LowJumpMultiplier * horseTrainingControllerV2.DefaultGravity)
         * z * z / (2 * horseTrainingControllerV2.ForwardVelocity * horseTrainingControllerV2.ForwardVelocity);
      return new Vector3(0, y, z);
   }

   private Vector3 PredictHighestPoint()
   {
      var jumpVel = new Vector3(0, horseTrainingControllerV2.JumpVelocity, horseTrainingControllerV2.ForwardVelocity); 
      var v0 = jumpVel.magnitude;

      var angle = Mathf.Deg2Rad * Vector3.Angle(jumpVel, Vector3.forward);
      var maxZ = v0 * v0 * Mathf.Sin(2 * angle) / (-horseTrainingControllerV2.DefaultGravity * 2); 
      var maxY = (horseTrainingControllerV2.JumpVelocity * horseTrainingControllerV2.JumpVelocity) / (2 * -horseTrainingControllerV2.DefaultGravity);
      return new Vector3(0, maxY, maxZ);
   }
}