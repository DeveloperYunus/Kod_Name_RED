public interface IPoolable
{
    void SentObjectToPool();
}
public interface IKillable
{
    void Kill();
}


public interface IDamageTakeable<T>
{
    void TakeDamage(T damageAmount);
}
public interface IDamageGivable<T>
{
    void GiveDamage(T damageAmount);
}
