using UnityEngine;
using System.Collections;
using System;

public class Enemy : MovingObject
{
	public int enemyStrikePower;
	public int hp = 20;

	private Animator animator;
	private Transform target;
	private bool skipMove;

    private int xHeading;
    private int yHeading;

    public AudioClip enemyAttack1;
    public AudioClip enemyAttack2;
    public AudioSource enemyMoves;
    public AudioClip enemyMove;

    private SpriteRenderer spriteRenderer;
    private bool closeToPlayer = false;

	protected override void Start()
    {
        enemyMoves.clip = enemyMove;

        GameManager.instance.AddEnemyToList (this);

		animator = GetComponent<Animator>();

        target = GameObject.FindGameObjectWithTag("Player").transform;

        spriteRenderer = GetComponent<SpriteRenderer> ();

		base.Start();
	}

	protected override bool AttemptMove <T> (int xDir, int yDir)
    {
        if (skipMove && !GameManager.instance.enemiesFaster)
        {
            skipMove = false;
            return false;
        }

        base.AttemptMove<T>(xDir, yDir);
        skipMove = true;
            return true;
	}


    public void MoveEnemy() {
        int xDir = 0;
        int yDir = 0;

        CheckDistanceToPlayer();

        if (GameManager.instance.enemiesSmarter)
        {
            bool moveOnX = false;

            if (Mathf.Abs(xHeading) >= Mathf.Abs(yHeading))
            {
                moveOnX = true;
            }

            for (int attempt = 0; attempt < 2; attempt++)
            {
                if (moveOnX == true && xHeading < 0)
                {
                    xDir = -1;
                    yDir = 0;
                }
                else if (moveOnX == true && xHeading > 0)
                {
                    xDir = 1;
                    yDir = 0;
                }
                else if (moveOnX == false && yHeading < 0)
                {
                    xDir = 0;
                    yDir = -1;
                }
                else if (moveOnX == false && yHeading > 0)
                {
                    xDir = 0;
                    yDir = 1;
                }

                Vector2 start = transform.position;
                Vector2 end = start + new Vector2(xDir, yDir);
                base.boxCollider.enabled = false;
                RaycastHit2D hit = Physics2D.Linecast(start, end, base.blockingLayer);
                base.boxCollider.enabled = true;

                if (hit.transform != null)
                {
                    if (hit.transform.gameObject.tag == "Wall" || hit.transform.gameObject.tag == "Chest")
                    {
                        if (moveOnX == true)
                        {
                            moveOnX = false;
                        }
                        else
                        {
                            moveOnX = true;
                        }
                    }
                    else
                    {
                        break;
                    }

                }
            }
        }
        else
        {
            if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
            {
                yDir = target.position.y > transform.position.y ? 1 : -1;
            }
            else
            {
                xDir = target.position.x > transform.position.x ? 1 : -1;
            }

        }
        
        AttemptMove<Player>(xDir, yDir);
    }

    private void CheckDistanceToPlayer()
    {
        xHeading = (int)target.position.x - (int)transform.position.x;
        yHeading = (int)target.position.y - (int)transform.position.y;

        int absX = Math.Abs(xHeading);
        int absY = Math.Abs(yHeading);

        if (absX < 6 && absY < 4)
        {
            //closeToPlayer = true;
            MakeSoundCloseToPlayer();
        }
        
    }

    private void MakeSoundCloseToPlayer()
    {
        enemyMoves.Play();
        closeToPlayer = false;
    }

    protected override void OnCantMove <T> (T component)
    {
        enemyStrikePower = GameManager.instance.enemyPower;
        Player hitPlayer = component as Player;
        hitPlayer.LoseHealth(enemyStrikePower);
        animator.SetTrigger("enemyAttack");
        SoundManager.instance.RandomizeSfx2(enemyAttack1, enemyAttack2);
    }

    public SpriteRenderer getSpriteRenderer()
    {
        return spriteRenderer;
    }

    public void DamageEnemy(int loss)
    {
        hp -= loss;

        if(hp <= 0)
        {
            GameManager.instance.RemoveEnemyFromList(this);
            Destroy(gameObject);
        }
    }

}
