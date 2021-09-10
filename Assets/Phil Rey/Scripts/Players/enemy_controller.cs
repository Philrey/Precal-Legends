using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using DG.Tweening;

public class enemy_controller : MonoBehaviour,player_class
{
    private Vector3 startPosition;
    [SerializeField]
    private Transform attackPosition;
    [SerializeField]
    private GameObject target;
    private Animator anim;

    private bool isAttacking = false;
    private GameObject cmCamera;
    private GameObject cmFollowCam;
    private CinemachineBasicMultiChannelPerlin shake;

    //Camera Shaker
    float shakeTimer;
    float shakeTimerTotal;
    float startingIntensity;

    // Start is called before the first frame update
    private void Awake() {
        anim = gameObject.GetComponentInChildren<Animator>();
        cmCamera = GameObject.Find("cmMainCamera");
        cmFollowCam = GameObject.Find("cmFollowCamera");
        shake = cmFollowCam.GetComponent<CinemachineVirtualCamera>()
            .GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        cmFollowCam.GetComponent<CinemachineVirtualCamera>().Follow = gameObject.transform;

        startPosition = transform.position;
    }
    // Update is called once per frame
    void Update() {
        if (Input.GetKey(KeyCode.Space)) {
            //attack();
        }
        if (Input.GetKeyDown(KeyCode.E)) {
            takeDamage();
        }
        if (shakeTimer > 0) {
            shakeTimer -= Time.deltaTime;

            shake.m_AmplitudeGain = Mathf.Lerp(startingIntensity, 0f, (1 - (shakeTimer / shakeTimerTotal)));
        }
    }
    public void focusCamToThis() {
        cmFollowCam.GetComponent<CinemachineVirtualCamera>().Follow = gameObject.transform;
    }

    #region Animation Events
    public void hitTarget() {
        target.GetComponent<player_class>().takeDamage();
        playSfx("attack", 0.5f);
        shakeCamera(5f, 0.5f);
    }
    private void shakeCamera(float intensity, float time) {
        shakeTimer = time;
        shakeTimerTotal = time;
        startingIntensity = intensity;
    }
    #endregion
    #region IEnumerators
    IEnumerator attackSequence() {
        isAttacking = true;

        cmCamera.SetActive(false);
        //Go to attack positon
        transform.DOMove(attackPosition.position, 1.5f);

        anim.SetTrigger("Walk");
        anim.SetBool("isWalking", true);
        while (DOTween.IsTweening(transform)) {
            yield return null;
        }
        anim.SetBool("isWalking", false);
        //Play attack animation
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(1f);
        //Go to back to start and play animations
        transform.eulerAngles = new Vector3(0, 180f, 0);
        anim.SetTrigger("Walk");
        anim.SetBool("isWalking", true);

        transform.DOMove(startPosition, 1.5f);
        cmCamera.SetActive(true);
        while (DOTween.IsTweening(transform)) {
            yield return null;
        }
        transform.eulerAngles = Vector3.zero;
        anim.SetBool("isWalking", false);
        isAttacking = false;
        yield return null;
    }
    #endregion
    #region Sound Manager Controls
    public void log(string toDebug) {
        Debug.Log(toDebug);
    }
    private void playBgm(string name) {
        playSound(name, 0);
    }
    private void playUi(string name) {
        playSound(name, 1);
    }
    private void playSfx(string name,float pitch) {
        playSound(name, 2, pitch);
    }
    private void playSound(string name, int soundType, float pitch = 1) {
        switch (soundType) {
            case 0: {
                GameObject.FindObjectOfType<sound_manager>().playBgSound(name);
                break;
            }
            case 1: {
                GameObject.FindObjectOfType<sound_manager>().playUiSound(name);
                break;
            }
            case 2: {
                GameObject.FindObjectOfType<sound_manager>().playSfxSound(name,false,pitch);
                break;
            }
        }
    }
    #endregion
    #region player_class Interface implementation
    private int health;

    public void attack() {
        if (!isAttacking) {
            StartCoroutine(attackSequence());
        }
    }
    public void takeDamage() {
        health--;
        Debug.Log("Enemy HP: " + health);
        if(health <= 0) {
            die();
        } else {
            anim.SetTrigger("Hurt");
        }
    }
    public void die() {
        Debug.Log("Enemy Dead");
        anim.SetTrigger("Death");
    }
    public int getHp() {
        return health;
    }
    public void setStats(int hp) {
        Debug.Log("Enemy HP:" + hp);
        health = hp;
    }
    #endregion
}
