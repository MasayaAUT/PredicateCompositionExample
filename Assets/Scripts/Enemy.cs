using UnityEngine;

public class Enemy : MonoBehaviour
{
    [field:SerializeField] public float health { get; private set; }
    [field:SerializeField] public EnemyType enemyType { get; private set; }

    public enum EnemyType
    {
        Ground,
        Flying,
        Underground
    }

    public void TakeDamage()
    {
        health -= 1;
        if(health <= 0)
        {
            GameManager.instance.EnemyKilled(this);
            Destroy(gameObject);
        }
    }
}
