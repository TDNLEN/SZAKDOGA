using UnityEngine;

public sealed class PlayerInputReader : MonoBehaviour
{

    [SerializeField] public Animator animator;
    public Vector2 MoveAxis { get; private set; }
    private int facingDirection = 1;


    void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y,transform.localScale.z);
    }

    private void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        if(x >0 && transform.localScale.x <0 ||
            x < 0 && transform.localScale.x > 0)
        {
            Flip();
        }

        //bool flipped = MoveAxis.x > 0;
        //this.transform.rotation = Quaternion.Euler(new Vector3(0f, flipped ? 180f : 0f, 0f));


        if (x != 0 || y != 0)
        {
            animator.SetBool("IsMoving", true);
        }
        else animator.SetBool("IsMoving", false);


        var v = new Vector2(x, y);

        MoveAxis=v.sqrMagnitude > 1f ? v.normalized : v;
    }
}
