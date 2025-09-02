using System.Collections.Generic;
using UnityEngine;

using CanTarget = And<And<And<InRange, IsAlive>, LineOfSight>, IsType>;
using Underground = Or<And<And<InRange, IsAlive>, LineOfSight>, IsType>;


public class Player : MonoBehaviour
{

    [SerializeField] float speed;
    float horizontal;
    float vertical;
    [field:SerializeField] public float range { private set; get; } = 5;
    [field: SerializeField] public LayerMask layerMask {  private set; get; }
    [field: SerializeField] public List<Enemy.EnemyType> enemyTypes { private set; get; }

    CanTarget canTarget;
    Underground underground;

    [SerializeField] bool useUndergroundAttack;

    private void Start()
    {
        canTarget = PredChain.Start(new InRange()).And(new IsAlive()).And(new LineOfSight()).And(new IsType()).Build();
        underground = PredChain.Start(new InRange()).And(new IsAlive()).And(new LineOfSight()).Or(new IsType()).Build();
    }

    // Update is called once per frame
    void Update()
    {
        vertical = Input.GetAxisRaw("Vertical");
        horizontal = Input.GetAxisRaw("Horizontal");

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
        var pred = PredChain.Start(new InRange()).And(new IsAlive()).And(new LineOfSight()).And(new IsType()).Build();

        for (int i = GameManager.instance.enemies.Count - 1; i >= 0; i--)
        {
            var enemy = GameManager.instance.enemies[i];
            if (pred.Check(enemy, in ctx))
                enemy.TakeDamage();
        }

        //if (useUndergroundAttack)
        //{
        //    ApplyAttack(underground, GameManager.instance.enemies, ctx);
        //}
        //else
        //{
        //    ApplyAttack(canTarget, GameManager.instance.enemies, ctx);
        //}
    }

    private void ApplyAttack<TPred>(in TPred pred, List<Enemy> enemies, in TargetCtx ctx) where TPred : IPred
    {
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            var enemy = enemies[i];
            if (pred.Check(enemy, in ctx))
                enemy.TakeDamage();
        }
    }
}
