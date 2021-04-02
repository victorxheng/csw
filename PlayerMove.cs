using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private float height;
    private float width;

    public Joystick joystick;
    private GameObject joyObject;

    public Rigidbody2D rb;
    public GameObject Enemy;
    public healthManager hm;
    public Animator playerAnim;
    private void Awake()
    {
        int rightSpawn = Random.Range(0, 2);
        if(rightSpawn == 1)
        {
            transform.position = new Vector2(60 + Random.Range(-5.0f,5.0f), Random.Range(-50.0f, 50.0f));
            rb.rotation = 90;
            Enemy.transform.position = new Vector2(-60 + Random.Range(-5.0f, 5.0f), Random.Range(-50.0f, 50.0f));
            Enemy.GetComponent<Rigidbody2D>().rotation = -90;
        }
        else
        {
            transform.position = new Vector2(-60 + Random.Range(-5.0f, 5.0f), Random.Range(-50.0f, 50.0f));
            rb.rotation = -90;
            Enemy.transform.position = new Vector2(60 + Random.Range(-5.0f, 5.0f), Random.Range(-50.0f, 50.0f));
            Enemy.GetComponent<Rigidbody2D>().rotation = 90;
        }
        //transform.position += new Vector3(Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f));

        

        height = Camera.main.orthographicSize * 2;
        width = height * Screen.width / Screen.height;
    }

    void FixedUpdate()
    {
        //if (PlayerPrefs.GetInt("gameOver", 0) == 0)
        //{
        float y = joystick.Vertical; //rotation
        float x = joystick.Horizontal;


        if (x > 0 && y > 0)
            rb.rotation = (float)(-Mathf.Atan(x / y) * (180 / 3.1416));
        else if (x > 0 && y < 0)
            rb.rotation = (float)(-Mathf.Atan(Mathf.Abs(y) / x) * (180 / 3.1416)) - 90;
        else if (x < 0 && y > 0)
            rb.rotation = (float)(Mathf.Atan(Mathf.Abs(x) / y) * (180 / 3.1416));
        else if (x < 0 && y < 0)
            rb.rotation = (float)(Mathf.Atan(Mathf.Abs(y) / Mathf.Abs(x)) * (180 / 3.1416)) + 90;

        if (joystick.Vertical != 0 && joystick.Horizontal != 0)
            playerAnim.SetBool("JoystickDown", true);
        else
            playerAnim.SetBool("JoystickDown", false);

        int speed = 100;
        int maxspeed = 40;

        // print(rb.velocity.magnitude);
        if (hm.playerHealth <= 0 && hm.death)
        {
            rb.mass = 100000;
            rb.velocity = Vector2.zero;
        }
        else
        {
            var move = new Vector2(joystick.Horizontal, joystick.Vertical);
            rb.AddForce(move * speed);
            if (joystick.Horizontal + joystick.Vertical != 0 && rb.angularVelocity != 0)
            {
                rb.angularVelocity = rb.angularVelocity / 1.05f;
                if (rb.angularVelocity < 0.1f)
                {
                    rb.angularVelocity = 0;
                }
            }
            if (rb.velocity.magnitude > maxspeed)
            {
                float factor = 40 / rb.velocity.magnitude;

                rb.velocity = new Vector2(rb.velocity.x * factor, rb.velocity.y * factor);
            }
        }

        //}
        /*else if (PlayerPrefs.GetInt("gameOver", 0) == -1)
        {
            if (joystick.Vertical != 0 || joystick.Horizontal != 0)
            {
                //PlayerPrefs.SetInt("gameOver", 0);
            }
        }
        
         
         
            if(rb.velocity.magnitude > maxspeed)
            {
            Vector2 playerVelocity = new Vector2(rb.velocity.x * Mathf.Sqrt(maxspeed*maxspeed / (rb.velocity.x * rb.velocity.x + rb.velocity.y * rb.velocity.y)), rb.velocity.y * Mathf.Sqrt(maxspeed*maxspeed / (rb.velocity.x * rb.velocity.x + rb.velocity.y * rb.velocity.y)));

            Vector2 checkMove = move+playerVelocity; print(checkMove.magnitude);

            if (checkMove.magnitude > maxspeed)
                {
                move = new Vector2(0, 0);
                }
            }
            rb.velocity
            
         */
    }
    public GameObject bullet;
    public GameObject playAudio;
    public void onFire()
    {
         GameObject audio = Instantiate(playAudio);
        audio.GetComponent<audioPlayer>().playSound = "shoot";

        if (!hm.death && hm.playerHealth >0)
        {
            var bulletObject = Instantiate(bullet, new Vector2(-Mathf.Sin(rb.rotation * (3.14f / 180f)) * 0.8f + transform.position.x, Mathf.Cos(rb.rotation * (3.14f / 180f)) * 0.8f + transform.position.y), Quaternion.Euler(new Vector3(0, 0, rb.rotation)));

            StartCoroutine(bulletMove(bulletObject));

        }
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
}
