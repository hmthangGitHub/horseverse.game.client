using System;

[Serializable]
public class HorseTrainingAttribute
{
    public float originalSpeed = 10;
    public float offset = 2;
    public float regularJumpTime = 0.4f;
    public float jumpHeight = 2.0f;
    public float moveTime = 0.2f;
    public float bridgeTime = 2.0f;
    public float flyingAngle = 30.0f;
    public float angularSpeed = 0.2f;
}