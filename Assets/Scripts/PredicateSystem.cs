using UnityEngine;
using System.Collections.Generic;
using System.Linq;

interface IPred
{
    bool Check(Enemy e, in TargetCtx c);
}

static class PredChain
{
    public static Chain<TLeaf> Start<TLeaf>(TLeaf leaf) where TLeaf : struct, IPred => new(leaf);
}

//Builder
readonly struct Chain<TPred> where TPred : struct, IPred
{
    public readonly TPred p;

    public Chain(TPred p)
    {
        this.p = p;

    }

    public Chain<And<TPred, TNext>> And<TNext>(TNext n) where TNext : struct, IPred => new(new And<TPred, TNext>(p, n));
    public Chain<Or<TPred, TNext>> Or<TNext>(TNext n) where TNext : struct, IPred => new(new Or<TPred, TNext>(p, n));
    public Chain<Not<TPred>> Not() => new(new Not<TPred>(p));
    public TPred Build() => p;
}


// Combinators
readonly struct And<A, B> : IPred 
    where A : struct, IPred 
    where B : struct, IPred
{
    public readonly A a;
    public readonly B b;

    public And(A a, B b)
    {
        this.a = a; 
        this.b = b;
    }

    public readonly bool Check(Enemy e, in TargetCtx c) => a.Check(e, in c) && b.Check(e, in c);
}

readonly struct Or<A, B> : IPred
    where A : struct, IPred
    where B : struct, IPred
{
    public readonly A a;
    public readonly B b;

    public Or(A a, B b)
    {
        this.a = a;
        this.b = b;
    }

    public readonly bool Check(Enemy e, in TargetCtx c) => a.Check(e, in c) || b.Check(e, in c);
}

readonly struct Not<A> : IPred
    where A : struct, IPred
{
    public readonly A a;
    public Not(A a)
    {
        this.a = a;
    }

    public readonly bool Check(Enemy e, in TargetCtx c) => !a.Check(e, in c);
}


// Leaves
readonly struct InRange : IPred
{
    public readonly bool Check(Enemy e, in TargetCtx c) => (e.transform.position - c.pos).sqrMagnitude <= c.range * c.range;
}

readonly struct LineOfSight : IPred
{
    public readonly bool Check(Enemy e, in TargetCtx c)
    {
        if (Physics.Linecast(c.pos, e.transform.position, out RaycastHit hit, c.layerMask))
        {
            return hit.transform.TryGetComponent<Enemy>(out Enemy enemy) && enemy == e;
        }
        return true;
    }

    //public readonly bool Check(Enemy e, in TargetCtx c)
    //{
    //    return !Physics.Linecast(c.pos, e.transform.position, c.layerMask);
    //}
}
readonly struct IsAlive : IPred
{
    public readonly bool Check(Enemy e, in TargetCtx c) => e.health > 0;
}

readonly struct IsType : IPred
{
    public readonly bool Check(Enemy e, in TargetCtx c) => c.enemyTypes.Contains(e.enemyType);
}

readonly struct TargetCtx
{
    public readonly Vector3 pos;
    public readonly float range;
    public readonly LayerMask layerMask;
    public readonly IReadOnlyList<Enemy.EnemyType> enemyTypes;

    public TargetCtx(Vector3 pos, float range, LayerMask layerMask, List<Enemy.EnemyType> enemyTypes)
    {
        this.pos = pos;
        this.range = range;
        this.layerMask = layerMask;
        this.enemyTypes = enemyTypes;
    }
}
