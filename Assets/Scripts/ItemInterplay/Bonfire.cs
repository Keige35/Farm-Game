using UnityEngine;
using DG.Tweening;
using System.Collections;
public class Bonfire : InterplayObject
{
    [SerializeField] private Ease ease;
    [SerializeField] private GameObject bonfire;
    [SerializeField] private Transform endPosition;
    [SerializeField] private ParticleSystem particleSystemBonfire;
    [SerializeField] private float dropItemSqure;
    public override void IterplayObject(CharacterInvetory characterInvetory)
    {
        var item = characterInvetory.GetItemByType(ItemType.Meat);
        if (item)
        {
            item.IsSelectable = false;
            item.transform.position = bonfire.transform.position+Vector3.up;
            item.transform.rotation = bonfire.transform.rotation;
            item.transform.DOKill();
            item.transform.DOShakeScale(0.4f, 0.3f);
            StartCoroutine(SpawnCookedObject(item));
            
            return;
        }
        var handItem = characterInvetory.GetHandItemByType(HandItemType.FullBucket);
        if (handItem)
        {
            handItem.ReturnToPool();
            var newBucket = ItemSpawner.Instance.GetHandItemByType(HandItemType.Bucket);
            characterInvetory.AddNewHandItem(newBucket);
            ChokeFire();
        }
        else 
        {
            LightFire();
        }
    }
    private IEnumerator SpawnCookedObject(Item item)
    {
        yield return new WaitForSeconds(3f);
        item.ReturnToPool();
        var newObject = ItemSpawner.Instance.GetItemByType(ItemType.CookedMeat);
        var randomX = Random.Range(-dropItemSqure / 2f, dropItemSqure / 2f);
        var randomZ = Random.Range(-dropItemSqure / 2f, dropItemSqure / 2f);
        var newPosition = bonfire.transform.position + new Vector3(randomX, 0, randomZ);
        newObject.transform.position = newPosition;
        newObject.transform.rotation = Random.rotation;
        var sequence = DOTween.Sequence();
        newObject.IsSelectable = false;
        sequence.Append(newObject.transform.DORotate(Vector3.zero, 0.9f));
        sequence.Join(newObject.transform.DOJump(newPosition, 1.8f, 1, 0.9f).SetEase(ease));
        sequence.Append(newObject.transform.DOShakeScale(0.4f, 0.3f));
        sequence.Append(DOVirtual.DelayedCall(0.2f, () => { newObject.IsSelectable = true; }));
    }
    void LightFire()
    {
        particleSystemBonfire.gameObject.SetActive(true);
    }

    void ChokeFire()
    {
        particleSystemBonfire.gameObject.SetActive(false);
    }
}
