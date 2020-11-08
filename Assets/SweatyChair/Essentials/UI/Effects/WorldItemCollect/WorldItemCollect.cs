using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SweatyChair;

public class WorldItemCollect : Singleton<WorldItemCollect>
{
    private const float ANIMATION_DURATION = 1.4f;
    //ui slots
    public GameObject coinIconSlot;
    public GameObject gemIconSlot;

    public Transform coinEndPos;
    public Transform gemEndPos;

    public static void CollectCoin(Transform fromPrefab)
    {
        Collect(fromPrefab, instance.coinEndPos, instance.coinIconSlot);
    }

    public static void CollectGem(Transform fromPrefab)
    {
        Collect(fromPrefab, instance.gemEndPos, instance.gemIconSlot);
    }

    public static void Collect(Transform fromWorldItem, Transform toUIItem, GameObject displayPrefab)
    {
        Vector2 fromPos = RectTransformUtility.WorldToScreenPoint(Camera.main, fromWorldItem.position);

        GameObject item = Instantiate<GameObject>(displayPrefab, instance.transform) as GameObject;
        item.transform.position = fromPos;

        LeanTween.move(item, toUIItem.position, ANIMATION_DURATION)
            .setEaseInBack()
            .setOnComplete(()=> {
                Destroy(item);
            });
    }
}
