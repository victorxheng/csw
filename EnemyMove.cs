using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    public GameObject player;
    public Rigidbody2D playerRb;
    public Joystick playerJoy;
    public healthManager health;
    [SerializeField] private LayerMask _layerMask;

    public Rigidbody2D rb;
    
    private float _stoppingDistance = 15f;

    private Vector2 _destination;
    private EnemyState _currentState;


    float wanderRange;
    float aggressionRange;
    float randomAmount;
    float time;
    int chickenFactor;
    int fireRate;
    float delayStart;

    private void Awake()
    {
        delayStart = Random.Range(0.5f, 2.0f);
        wanderRange = Random.Range(35f, 70f);
        aggressionRange = Random.Range(25f, 30f);
        chickenFactor = Random.Range(0, 5);
        

        int kills = PlayerPrefs.GetInt("kills", 0);
        int deaths = PlayerPrefs.GetInt("deaths", 0);
        if (deaths == 0) deaths = 1;
        if(kills/deaths < 1)
        {
            fireRate = Random.Range(0, 3);
        }
        else
        {
            fireRate = Random.Range(-1, 2);
        }
        
        

       /* if (kills<15)
        {
            randomAmount = Random.Range(6.0f, 12.0f);
        }
        else if (kills < 40)
        {
            randomAmount = Random.Range(5.0f, 8.0f);
        }
        else if(kills < 80)
        {
            randomAmount = Random.Range(4.0f, 6.0f);
        }
        else if(kills < 100)
        {
            randomAmount = Random.Range(1.0f, 4.0f);
        }
        else
        {*/
            randomAmount = Random.Range(0.0f,1.0f);
      //  }
       // randomAmount = 5;

        StartCoroutine(timeElapsed());

    }
    private void Start()
    {
        StartCoroutine(autoFire());
    }
    IEnumerator timeElapsed()
    {
        while (player)
        {
            time += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
    }
    public enum EnemyState
    {
        Wander,
        Aggressive,
        Defensive,
        Run,
        Dead
    }
    bool run = false;
    void FixedUpdate()
    {
        switch (_currentState)
        {
            case EnemyState.Wander:
                {
                    if (NeedsDestination())
                    {
                        GetDestination();
                    }
                    move(_destination.x, _destination.y);

                    if (Mathf.Abs(player.transform.position.x - transform.position.x) < 40
                        && Mathf.Abs(player.transform.position.y - transform.position.y) < 20)
                    {
                        //add delay
                        if(!hm.death)_currentState = EnemyState.Aggressive;
                    }
                    else if (Mathf.Abs(player.transform.position.x - transform.position.x) < 50
                        && Mathf.Abs(player.transform.position.y - transform.position.y) < 25)
                        {
                        GetDestination();
                    }

                    if (hm.enemyHealth <= 0) _currentState = EnemyState.Run;
                    break;
                }
            case EnemyState.Aggressive:
                {
                    if (!hm.death)
                    {
                        if (health.enemyHealth < startFlee && health.enemyHealth < health.playerHealth)
                        {
                            GetOppositeDestination();
                            run = false;
                            _currentState = EnemyState.Run;
                        }
                        else if (chickenFactor == 2 && health.enemyHealth < startFlee)
                        {
                            GetOppositeDestination();
                            run = false;
                            _currentState = EnemyState.Run;
                        }
                        else if (chickenFactor >= 3 && health.enemyHealth < health.playerHealth - 2)
                        {
                            GetOppositeDestination();
                            run = false;
                            _currentState = EnemyState.Run;
                        }
                        else if (Mathf.Abs(player.transform.position.x - transform.position.x) < aggressionRange * 2
                            && Mathf.Abs(player.transform.position.y - transform.position.y) < aggressionRange)
                        {
                            move(player.transform.position.x + Mathf.Sin(time*10) * randomAmount + playerJoy.Horizontal * 2, player.transform.position.y + Mathf.Sin(time*10) * randomAmount + playerJoy.Vertical * 2);

                        }
                        else
                        {
                            GetDestination();
                            _currentState = EnemyState.Wander;
                        }
                    }
                    else
                    {
                        _currentState = EnemyState.Run;
                    }
                    
                    break;
                }
            case EnemyState.Run:
                {
                    if (NeedsDestination())
                    {
                        GetOppositeDestination();
                    }
                    if (!hm.death)
                    {
                        if (health.enemyHealth > health.playerHealth && chickenFactor != 2)
                        {
                            _currentState = EnemyState.Wander;
                        }
                        else if ((Mathf.Abs(player.transform.position.x - transform.position.x) < 20
                            && Mathf.Abs(player.transform.position.y - transform.position.y) < 10))
                        {
                            if (run)
                            {
                                move(player.transform.position.x + Mathf.Sin(time*10) * randomAmount * 0.8f + playerJoy.Horizontal * 2, player.transform.position.y + Mathf.Sin(time*10) * randomAmount * 0.8f + playerJoy.Vertical * 2);

                            }
                            else
                            {
                                move(_destination.x, _destination.y);
                            }
                        }
                        else
                        {
                            if (!run) run = true;
                            move(_destination.x, _destination.y);
                        }

                        if (health.enemyHealth > finishFlee)
                        {
                            //add delay
                            _currentState = EnemyState.Wander;
                        }
                    }
                    else
                    {
                        if (health.enemyHealth <= 0) _currentState = EnemyState.Dead;
                        move(_destination.x, _destination.y);
                    }                    

                    break;
                }
            case EnemyState.Dead:
                {
                    rb.mass = 10000000;
                    rb.velocity = Vector2.zero;
                    break;
                }
             

        }

        


        
    }
    int finishFlee;
    int startFlee;
    private bool NeedsDestination()
    {
        finishFlee = Random.Range(10, 20);
        startFlee = Random.Range(5, 10);
        if (_destination == Vector2.zero)
            return true;

        var distance = Vector2.Distance(transform.position, _destination);
        if (distance <= _stoppingDistance)
        {
            return true;
        }

        return false;
    }


    float Xmax = 100;
    float Xmin = -100;
    float Ymax = 75;
    float Ymin = -75;


    private void GetDestination()
    {
        Vector2 dest = Vector2.zero;
        int count = 0;
        while(dest == Vector2.zero)
        {
            Vector2 testDest = new Vector2(player.transform.position.x + Random.Range(-wanderRange, wanderRange),
                player.transform.position.y + Random.Range(-wanderRange, wanderRange));
            if(testDest.x < Xmax && testDest.x > Xmin && testDest.y > Ymin && testDest.y < Ymax)
            {
                dest = testDest;
                break;
            }
            count++;
            if (count > 20)
                break;

            
        }
        _destination = dest;
        //print(dest);
    }

    float runRange = 40;
    private void GetOppositeDestination()
    {
        Vector2 dest = Vector2.zero;
        int count = 0;
        while (dest == Vector2.zero)
        {
            Vector2 testDest = new Vector2(Random.Range(Xmin, Xmax),
                Random.Range(Ymin, Ymax));
            if (testDest.x < Xmax && testDest.x > Xmin && testDest.y > Ymin && testDest.y < Ymax)
            {
                if (testDest.x < player.transform.position.x + runRange &&
                    testDest.x > player.transform.position.x - runRange &&
                    testDest.y < player.transform.position.y + runRange &&
                    testDest.y > player.transform.position.x - runRange)
                {
                }
                else
                {
                    dest = testDest;
                    break;
                }
            }
            count++;
            if (count > 50)
                break;


        }
        _destination = dest;
    }








    public void move(float targetX, float targetY)
    {
        float y = targetY - transform.position.y; //rotation
        float x = targetX - transform.position.x;
        float n = 1 / Mathf.Sqrt(x * x + y * y);
        x *= n;
        y *= n;
        

        if (x > 0 && y > 0)
            rb.rotation = (float)(-Mathf.Atan(x / y) * (180 / 3.1416));
        else if (x > 0 && y < 0)
            rb.rotation = (float)(-Mathf.Atan(Mathf.Abs(y) / x) * (180 / 3.1416)) - 90;
        else if (x < 0 && y > 0)
            rb.rotation = (float)(Mathf.Atan(Mathf.Abs(x) / y) * (180 / 3.1416));
        else if (x < 0 && y < 0)
            rb.rotation = (float)(Mathf.Atan(Mathf.Abs(y) / Mathf.Abs(x)) * (180 / 3.1416)) + 90;

        int speed;
        if (time < delayStart && time < 2)
            speed = 2;
        else
            speed = 100;
        int maxspeed = 40;

        // print(rb.velocity.magnitude);

        var move = new Vector2(x, y);
        rb.AddForce(move * speed);
        rb.angularVelocity = 0;
        if (rb.velocity.magnitude > maxspeed)
        {
            float factor = 40 / rb.velocity.magnitude;

            rb.velocity = new Vector2(rb.velocity.x * factor, rb.velocity.y * factor);
        }


    }

    IEnumerator autoFire()
    {
        while (_currentState != EnemyState.Dead)
        {
            if (Mathf.Abs(player.transform.position.x - transform.position.x) < 40
                && Mathf.Abs(player.transform.position.y - transform.position.y) < 20)
            {
                onEnemyFire();
            }
                yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
            
        }
    }

    public GameObject bullet;
    public void onEnemyFire()
    {
        // GameObject audio = Instantiate(playAudio);
        //audio.GetComponent<AudioPlayer>().playSound = "shoot";

        var bulletObject = Instantiate(bullet, new Vector2(-Mathf.Sin(rb.rotation * (3.14f / 180f)) * 0.8f + transform.position.x, Mathf.Cos(rb.rotation * (3.14f / 180f)) * 0.8f + transform.position.y), Quaternion.Euler(new Vector3(0, 0, rb.rotation)));

        if (gameObject.active)
            StartCoroutine(bulletMove(bulletObject));
    }

    IEnumerator bulletMove(GameObject bulletObject)
    {
        Vector2 velocity;
        // if (PlayerPrefs.GetInt("gameMode", 0) == 0)
        velocity = new Vector2(-Mathf.Sin(rb.rotation * (3.14f / 180f)) * Time.deltaTime * 100f + rb.velocity.x * Time.deltaTime * 1f, Mathf.Cos(rb.rotation * (3.14f / 180f)) * Time.deltaTime * 100f + rb.velocity.y * Time.deltaTime * 1f);

        //else
        //     velocity = new Vector2(-Mathf.Sin(rb.rotation * (3.14f / 180f)) * Time.deltaTime * 50f + rb.velocity.x * Time.deltaTime * 1f, Mathf.Cos(rb.rotation * (3.14f / 180f)) * Time.deltaTime * 50f + rb.velocity.y * Time.deltaTime * 1f);

        float time2 = 0;
        while (bulletObject)
        {

            // if (PlayerPrefs.GetInt("gameMode", 0) == 0)
            //{
            if (time2 > 0.6f) Destroy(bulletObject);
            //}
            // else
            //{
            //     if (time2 > 0.35f) Destroy(bulletObject);
            // }
            bulletObject.transform.position += Vector3.up * velocity.y;
            bulletObject.transform.position += Vector3.right * velocity.x;
            time2 += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
    public healthManager hm;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "PlayerBullet")
            hm.enemyDamage();
    }
}
