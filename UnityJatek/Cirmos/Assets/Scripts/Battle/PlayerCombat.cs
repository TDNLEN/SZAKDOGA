using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Equip / Holder")]
    public Transform swordHolder;
    public GameObject equippedSword;

    private Animator weaponAnimator;
    public Animator animator;

    public bool HasWeapon => equippedSword != null;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (animator != null)
            animator.SetBool("HasAxe", false);
    }

    public void PickUpItem(GameObject weaponObject,
                           Vector3 localPos,
                           Quaternion localRot,
                           Vector3 localScale)
    {
        if (swordHolder == null)
        {
            Debug.LogError("Nincs beállítva swordHolder!");
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

        if (animator) animator.SetBool("HasAxe", true);

        // ha gun, szóljunk neki, hogy mostantól kézben van
        var gun = weaponObject.GetComponent<GunWeapon>();
        if (gun != null)
            gun.OnEquip();

        UpdateCursorForCurrentWeapon();

    }

    // 🔹 Ezt hívd MINDIG, ha fegyvert veszel le a kézről
    public void UnequipCurrent()
    {
        if (equippedSword != null)
        {
            // ha gun, szóljunk neki
            var gun = equippedSword.GetComponent<GunWeapon>();
            if (gun != null)
                gun.OnUnequip();

            // 🔹 ANIMATOR RESET, hogy ne maradjon fél-attack állapotban
            if (weaponAnimator != null)
            {
                // minden triggert / állapotot visszaállít
                weaponAnimator.Rebind();
                weaponAnimator.Update(0f);   // azonnal érvényesítve
            }

            var go = equippedSword;
            go.transform.SetParent(null);
            go.SetActive(false);

            equippedSword = null;
            weaponAnimator = null;
        }

        if (animator) animator.SetBool("HasAxe", false);
        UpdateCursorForCurrentWeapon();
    }





    public void PlayWeaponAttackAnim()
    {
        if (animator != null)
            animator.SetTrigger("Attack");

        if (weaponAnimator != null)
            weaponAnimator.SetTrigger("Attack");
    }

    // csak PlayerCombat használja
    void UpdateCursorForCurrentWeapon()
    {
        if (equippedSword != null && equippedSword.TryGetComponent<GunWeapon>(out _))
            CursorManager.UseCrosshair();
        else
            CursorManager.UseDefault();
    }

}
