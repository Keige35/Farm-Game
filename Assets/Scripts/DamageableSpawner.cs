using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableSpawner : MonoBehaviour
{
    [SerializeField] private int amount;
    [SerializeField] private DamageableType damageableType;

    private void Start()
    {
        for (int i = 0; i < amount; i++)
        {
            ItemSpawner.Instance.SpawnNewDamageableItem(damageableType, transform.position, transform.rotation);
        }
    }
}