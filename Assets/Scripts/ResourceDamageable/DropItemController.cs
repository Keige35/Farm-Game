using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DropItemController : MonoBehaviour
{
    [SerializeField] private Ease ease;
    [SerializeField] private List<float> defaultDamagePercent = new List<float>();
    [SerializeField] private ItemType dropItem;
    [SerializeField] private Vector2 dropItemSqure;
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private Transform floorPosition;
    private List<float> damagePercent = new List<float>();
    private void Awake()
    { 
        DefaultNpcStateMachine.changedHealth += HealthUpdated;
    }
    private void OnEnable()
    {
        if(damagePercent.Count == 0)
        {
            foreach (float percent in defaultDamagePercent)
            {
                damagePercent.Add(percent);
            }
        }
    }
    protected void HealthUpdated(int currentHealth, int maxHealth, GameObject damageObject)
    {
        if (damageObject != this.gameObject) return;
        var currentPercent = (float)currentHealth / maxHealth;
        currentPercent *= 100f;
        UpdatePercent(currentPercent);
        if (currentHealth > 0)
        {
            return;
        }

    }
    private void UpdatePercent(float currentPercent)
    {
        for (int i = 0; i < damagePercent.Count; i++)
        {
            if (damagePercent[i] >= currentPercent)
            {
                SpawnItem(dropItem);
                damagePercent.Remove(damagePercent[i]);
                if (damagePercent.Count != 0)
                {
                    UpdatePercent(currentPercent);
                }

                break;
            }
        }
    }
    private void SpawnItem(ItemType dropItem)
    {
        var randomX = Random.Range(-dropItemSqure.x / 2f, dropItemSqure.x / 2f);
        var randomZ = Random.Range(-dropItemSqure.y / 2f, dropItemSqure.y / 2f);
        var newPosition = floorPosition.position + new Vector3(randomX, 0, randomZ);
        var newItem = ItemSpawner.Instance.GetItemByType(dropItem);
        newItem.transform.position = spawnPosition.position;
        newItem.transform.rotation = Random.rotation;
        var sequence = DOTween.Sequence();
        newItem.IsSelectable = false;
        sequence.Append(newItem.transform.DORotate(Vector3.zero, 0.9f));
        sequence.Join(newItem.transform.DOJump(newPosition, 1.8f, 1, 0.9f).SetEase(ease));
        sequence.Append(newItem.transform.DOShakeScale(0.4f, 0.3f));
        sequence.Append(DOVirtual.DelayedCall(0.2f, () => { newItem.IsSelectable = true; }));
    }
}
