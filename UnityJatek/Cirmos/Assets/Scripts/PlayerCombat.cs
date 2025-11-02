using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Equip / Holder")]
    public Transform swordHolder;        // Player/ItemHolder
    public GameObject equippedSword;     // pl. Axe

    private Animator weaponAnimator;

    [Header("Player animator (opcionális)")]
    public Animator animator;

    // ezt kérdezi le a PlayerAttack
    public bool HasWeapon => equippedSword != null;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        // induláskor nincs fegyver
        if (animator != null)
            animator.SetBool("HasAxe", false);
    }

    // EZT már nem kell:
    // void Update() { if (Input.GetKeyDown...) ... }

    // ezt hívja az ItemPickUp
    public void PickUpItem(GameObject weaponObject, Vector3 localPos, Quaternion localRot, Vector3 localScale)
    {
        if (swordHolder == null)
        {
            Debug.LogError("Nincs beállítva swordHolder! Húzd be a Player/ItemHolder-t.");
            return;
        }

        weaponObject.transform.SetParent(swordHolder, false);
        weaponObject.transform.localPosition = localPos;
        weaponObject.transform.localRotation = localRot;
        weaponObject.transform.localScale = localScale;

        // földről felvett collider off
        var pickupCol = weaponObject.GetComponent<Collider2D>();
        if (pickupCol) pickupCol.enabled = false;

        // mostantól ez a fegyver
        equippedSword = weaponObject;

        // fegyver animator
        weaponAnimator = weaponObject.GetComponent<Animator>();
        if (weaponAnimator == null)
            Debug.LogWarning("A felvett fegyveren nincs Animator!");

        // player animnak is jelezhetjük
        if (animator) animator.SetBool("HasAxe", true);
    }

    // ezt hívhatja a PlayerAttack, ha azt akarod, hogy a fegyver animálódjon is
    public void PlayWeaponAttackAnim()
    {
        if (animator != null)
            animator.SetTrigger("Attack");

        if (weaponAnimator != null)
            weaponAnimator.SetTrigger("Attack");
    }
}
