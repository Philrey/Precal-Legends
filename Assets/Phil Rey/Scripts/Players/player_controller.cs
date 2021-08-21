using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class player_controller : MonoBehaviour,player_class
{
    private Vector3 startPosition;
    [SerializeField]
    private Transform attackPosition;
    private Animator anim;

    private bool isAttacking = false;

    // Start is called before the first frame update
    private void Awake() {
        anim = gameObject.GetComponent<Animator>();
        startPosition = transform.position;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space)) {
            attack();
        }
        if (Input.GetKey(KeyCode.E)) {
            takeDamage();
        }
    }
    #region IEnumerators
    IEnumerator attackSequence() {
        isAttacking = true;
        //Go to attack positon
        transform.DOMove(attackPosition.position, 1f);

        anim.SetBool("doneRollingMove", false);
        anim.SetTrigger("Roll");
        while (DOTween.IsTweening(transform)) {
            yield return null;
        }
        anim.SetBool("doneRollingMove", true);
        //Play attack animation
        anim.SetTrigger("Attack"+(int)Random.Range(1,3));
        yield return new WaitForSeconds(1f);
        //Go to back to start and play animations
        transform.eulerAngles = new Vector3(0, 180f, 0);
        anim.SetBool("doneRollingMove", false);
        anim.SetTrigger("Roll");

        transform.DOMove(startPosition, 1f);
        while (DOTween.IsTweening(transform)) {
            yield return null;
        }
        transform.eulerAngles = Vector3.zero;
        anim.SetBool("doneRollingMove", true);
        isAttacking = false;
        yield return null;
    }
    #endregion
    #region player_class Interface implementation
    public void attack() {
        if (!isAttacking) {
            StartCoroutine(attackSequence());
        }
    }
    public void takeDamage() {
        anim.SetTrigger("Hurt");
    }
    public void die() {

    }
    public void getHp() {

    }
    #endregion
}
