using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [field:SerializeField] public List<Enemy> enemies {  get; private set; } = new List<Enemy>();

    private void Awake()
    {
        instance = this;
    }

    public void EnemyKilled(Enemy enemy)
    {
        enemies.Remove(enemy);
    }
}
