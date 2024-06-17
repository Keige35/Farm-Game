using DG.Tweening;
using UnityEngine;

public class Trash : InterplayObject
{
    [SerializeField] private Transform endPosition;
    [SerializeField] private Transform trashWaste;

    private SaveData saveData;
    private Wallet wallet;

    public override void IterplayObject(CharacterInvetory characterInvetory)
    {
        saveData ??= ServiceLocator.GetService<SaveData>();
        wallet ??= ServiceLocator.GetService<Wallet>();

        var itemLast = characterInvetory.GetLastItem();
        if (itemLast == null) return;
        itemLast.ItemSelected();

        trashWaste.DOKill();
        trashWaste.DOLocalRotate(new Vector3(-90, 0, 0), 0.3f).OnComplete(() =>
        {
            trashWaste.DOLocalRotate(new Vector3(0, 0, 0), 0.3f);
        });
        itemLast.transform.DOJump(endPosition.position, 1.4f, 1, 0.6f).OnComplete(() => { itemLast.ReturnToPool(); });
        wallet.AddMoney(10);
    }
}