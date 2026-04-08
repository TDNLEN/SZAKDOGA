using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Refs")]
    public GameObject attackArea;      
    public PlayerCombat playerCombat;  

    [Header("Timing")]
    public float attackDuration = 0.25f; 
    public float attackCooldown = 0.4f;  

    private bool attacking = false;
    private float timer = 0f;
    private float lastAttackTime = -999f;

    private void Start()
    {
        if (attackArea)
            attackArea.SetActive(false);

        if (playerCombat == null)
            playerCombat = GetComponent<PlayerCombat>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            TryAttack();

        if (attacking)
        {
            timer += Time.deltaTime;
            if (timer >= attackDuration)
            {
                attacking = false;
                if (attackArea) attackArea.SetActive(false);
            }
        }
    }

    private void TryAttack()
    {
        if (playerCombat == null || !playerCombat.HasWeapon)
            return;

        var weaponGO = playerCombat.equippedSword;

        GunWeapon gun = null;
        if (weaponGO != null)
        {
            gun = weaponGO.GetComponent<GunWeapon>();
            if (gun == null)
                gun = weaponGO.GetComponentInChildren<GunWeapon>();
        }

        if (gun != null)
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0f;

            bool fired = gun.TryShootTowards(mouseWorld);
            if (fired)
                playerCombat.PlayWeaponAttackAnim();

            return;
        }


        if (Time.time - lastAttackTime < attackCooldown)
            return;

        lastAttackTime = Time.time;

        attacking = true;
        timer = 0f;
        if (attackArea) attackArea.SetActive(true);

        playerCombat.PlayWeaponAttackAnim();
    }
}
    