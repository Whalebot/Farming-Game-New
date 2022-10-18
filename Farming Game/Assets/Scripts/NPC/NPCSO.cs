using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "CharacterSO", menuName = "ScriptableObjects/Character", order = 3)]
public class NPCSO : ScriptableObject
{
    [PreviewField] public Sprite defaultSprite;
    public Sprite happySprite;
    public Sprite blushSprite;
    public Sprite angrySprite;
    public Sprite smugSprite;

    public List<ItemSO> likedItems;
    public List<ItemSO> hatedItems;

}
