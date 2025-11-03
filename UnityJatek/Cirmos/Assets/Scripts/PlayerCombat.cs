using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Equip / Holder")]
    public Transform swordHolder;        // Player/ItemHolder
    public GameObject equippedSword;     // pl. Axe

    private Animator weaponAnimator;

    [Header("Player animator (opcionális)")]
    public Animator animator;

    public bool HasWeapon => equippedSword != null;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (animator != null)
            animator.SetBool("HasAxe", false);
    }

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

        var pickupCol = weaponObject.GetComponent<Collider2D>();
        if (pickupCol) pickupCol.enabled = false;

        equippedSword = weaponObject;

        weaponAnimator = weaponObject.GetComponent<Animator>();
        if (weaponAnimator == null)
            Debug.LogWarning("A felvett fegyveren nincs Animator!");

        if (animator) animator.SetBool("HasAxe", true);
    }

    public void PlayWeaponAttackAnim()
    {
        if (animator != null)
            animator.SetTrigger("Attack");

        if (weaponAnimator != null)
            weaponAnimator.SetTrigger("Attack");
    }
}
