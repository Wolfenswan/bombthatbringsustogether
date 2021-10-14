using UnityEngine;

[CreateAssetMenu(fileName="PlayerData", menuName="Data/Player Data")]
public class PlayerData : ScriptableObject
{
    public string Name;
    public Vector2 GroundCheckOffset = new Vector2(0.25f,0.5f);
    public float JumpPower = 200f;
    public float JumpWithBallMultipl = 0.5f;
    public float MoveSpeed = 0.5f;
    public float AirSteeringSpeed = 0.8f;
    public float MoveSpeedWithBall = 0.25f;
    public float ThrowForce = 10f;
    public float YankForce = 8f;
    public bool CanYank = true;
    public Vector2 BallAttachOffsets = new Vector2(0.5f,0.1f);

    public AudioData AudioData;
}