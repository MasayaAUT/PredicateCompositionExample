using System.Collections.Generic;
using UnityEngine;

using CanTarget = And<And<And<InRange, IsAlive>, LineOfSight>, IsType>;
//using CanTarget = And<And<InRange, IsAlive>, Or<LineOfSight, IsType>>;

public class Player : MonoBehaviour
{

    [SerializeField] float speed;
    float horizontal;
    float vertical;
    [field:SerializeField] public float range { private set; get; } = 5;
    [field: SerializeField] public LayerMask layerMask {  private set; get; }
    [field: SerializeField] public List<Enemy.EnemyType> enemyTypes { private set; get; }

    CanTarget pred;

    private void Start()
    {
        pred = PredChain.Start(new InRange()).And(new IsAlive()).And(new LineOfSight()).And(new IsType()).Build();
    }

    // Update is called once per frame
    void Update()
    {
        vertical = Input.GetAxis("Vertical");
        horizontal = Input.GetAxis("Horizontal");

        Move();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Attack();
        }
    }

    private void Move()
    {
        Vector3 moveDirection = ((vertical * Vector3.forward) + (horizontal * Vector3.right)).normalized;
        moveDirection *= Time.deltaTime * speed;
        transform.position += moveDirection;
    }

    private void Attack()
    {
        var ctx = new TargetCtx(transform.position, range, layerMask, enemyTypes);
        //var pred = PredChain.Start(new InRange()).And(new IsAlive()).And(new LineOfSight()).And(new IsType()).Build();

        Enemy[] enemyList = GameManager.instance.enemies.ToArray();
        if (enemyList.Length > 0)
        {
            for (int i = 0; i < enemyList.Length; i++)
            {
                if (pred.Check(enemyList[i], ctx)) enemyList[i].TakeDamage();
            }
        }
    }
}
