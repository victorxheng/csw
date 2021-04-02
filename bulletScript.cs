using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletScript : MonoBehaviour {
    public GameObject explosion;
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(gameObject.tag == "PlayerBullet")
        {
            if (collision.gameObject.tag == "Enemy")
            {

                Instantiate(explosion, gameObject.transform.position, Quaternion.identity);
                Destroy(gameObject);
                // Destroy(collision.gameObject);
            }
        }
        else
        {
            if (collision.gameObject.tag == "Player")
            {

                Instantiate(explosion, gameObject.transform.position, Quaternion.identity);
                Destroy(gameObject);
                // Destroy(collision.gameObject);
            }

        }
    }/*
    private void Update()
    {
        if (PlayerPrefs.GetInt("gameOver", 0) == 1)
            Destroy(gameObject);
    }*/

}
