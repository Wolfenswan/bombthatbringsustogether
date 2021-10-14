using UnityEngine;

[CreateAssetMenu(fileName="GameData", menuName="Data/Game Data")]
public class GameData : ScriptableObject
{
    public LayerMask GroundLayer;
    public float BombTimer = 25;
    public float DistanceToGround;
    public float PickupDistance = 15f;
    public float MaxDistanceBetweenPlayers = 60f;
    public NEINGames.Collections.RangeInt ThrowingAngle = new NEINGames.Collections.RangeInt(-80,80);
    public float DebugSphereSize = 0.1f;
}