using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Refs")]
    public GameObject attackArea;           // ide húzod a hitboxot
    public PlayerCombat playerCombat;       // ide húzod a PlayerCombatot

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
        // kell fegyver
        if (playerCombat != null && !playerCombat.HasWeapon) return;

        // a kézben lévő fegyver:
        var weaponGO = playerCombat != null ? playerCombat.equippedSword : null;

        // ha pisztoly
        var gun = weaponGO ? weaponGO.GetComponent<GunWeapon>() : null;
        if (gun != null)
        {
            // irány: a player X skálájának előjele alapján (jobb/bal)
            float sign = Mathf.Sign(transform.localScale.x == 0 ? 1f : transform.localScale.x);
            var dir = Vector2.right * sign;

            gun.TryShoot(dir, (Vector2)transform.position);
            if (playerCombat != null) playerCombat.PlayWeaponAttackAnim();
            return;
        }

        // --- MELEE fallback (axe stb.) ---
        if (Time.time - lastAttackTime < attackCooldown) return;
        lastAttackTime = Time.time;

        attacking = true;
        timer = 0f;
        if (attackArea) attackArea.SetActive(true);
        if (playerCombat != null) playerCombat.PlayWeaponAttackAnim();
    }
}
