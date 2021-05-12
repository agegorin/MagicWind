using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillButton : MonoBehaviour
{
    private PlayerAttacks type;
    private Vector3 lastPoint;
    private Vector3 targetPoint;
    private float startMoveTime;

    // 0 - arrow; 1 - strongarrow; 2 - whirpool; 3 - allside
    [SerializeField] private Sprite[] sprite;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        
        type = (PlayerAttacks)Random.Range(0, 6);
        SpriteRenderer actionImage = transform.GetChild(0).GetComponent<SpriteRenderer>();

        switch(type)
        {
            case PlayerAttacks.OneLeft:
                actionImage.sprite = sprite[0];
                actionImage.flipX = true;
                break;
            case PlayerAttacks.OneRight:
                actionImage.sprite = sprite[0];
                break;
            case PlayerAttacks.AllLeft:
                actionImage.sprite = sprite[1];
                actionImage.flipX = true;
                break;
            case PlayerAttacks.AllRight:
                actionImage.sprite = sprite[1];
                break;
            case PlayerAttacks.Whirpool:
                actionImage.sprite = sprite[2];
                break;
            case PlayerAttacks.WhirpoolRight:
                actionImage.sprite = sprite[2];
                actionImage.flipX = true;
                break;
            case PlayerAttacks.Cross:
                actionImage.sprite = sprite[3];
                break;
        }
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(lastPoint, targetPoint, (Time.time - startMoveTime) * 5f);
    }

    public void StartMove(Vector3 _targetPoint)
    {
        startMoveTime = Time.time;
        lastPoint = transform.position;
        targetPoint = _targetPoint;
    }

    private void OnMouseDown()
    {
        gameManager.SelectAttackType(type, gameObject.GetComponent<SkillButton>());
    }
}
