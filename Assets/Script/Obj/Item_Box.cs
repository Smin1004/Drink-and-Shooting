using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Box : Enemy_Base, IItme
{
    [Header("Item_Box")]
    [SerializeField] Sprite open;
    [SerializeField] Sprite destroy;
    [SerializeField] List<GameObject> weapon = new List<GameObject>();
    [SerializeField] Item_Gun item;

    [SerializeField] Vector2 boxSize;

    [SerializeField] bool isOpen;
    
    public void Use(){
        if(isOpen) return;

        isOpen = true;
        int ran = Random.Range(0, weapon.Count);

        var temp = Instantiate(item, transform.position, Quaternion.identity).GetComponent<Item_Gun>();
        temp.me = weapon[ran];
        GetComponent<SpriteRenderer>().sprite = open;
    }

    protected override void DieDestroy()
    {
        GetComponent<SpriteRenderer>().sprite = destroy;
        Destroy(GetComponent<BoxCollider2D>());
        Destroy(GetComponent<Rigidbody2D>());
    }
}
