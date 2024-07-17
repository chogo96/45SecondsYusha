using UnityEngine;
using System.Collections;

public interface ICharacter : IIdentifiable
{
    void Die();
}

public interface IIdentifiable
{
    int ID { get; }
}
