using UnityEngine;

[CreateAssetMenu(fileName = "EnemyConfig", menuName = "Combat/Enemy/EnemyConfig")]
public class EnemyConfig : ScriptableObject
{
    [Header("Enemy Stats")]
    public float health;
    public float speed;
    public int damage;
    public int currencyReward;
}
