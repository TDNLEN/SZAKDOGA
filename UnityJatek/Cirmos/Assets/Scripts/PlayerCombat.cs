using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Transform swordHolder;  
    public GameObject equippedSword;

    public void PickUpItem(GameObject swordObject, Vector3 localPos, Quaternion localRot, Vector3 localScale)
    {
        if (swordHolder == null)
        {
            Debug.LogError("Nincs beállítva swordHolder! (Player/ItemHolder-t húzd be)");
            return;
        }       

        swordObject.transform.SetParent(swordHolder, false);
        swordObject.transform.localPosition = localPos;
        swordObject.transform.localRotation = localRot;
        swordObject.transform.localScale = localScale;

        var col = swordObject.GetComponent<Collider2D>();
        if (col) col.enabled = false;

        equippedSword = swordObject;
        Debug.Log("Kard felszedve ✔");
    }
}
