using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointToEnemy : MonoBehaviour {
    public GameObject Enemy;
    public GameObject Player;
    public Rigidbody2D rb;
	
	// Update is called once per frame
	void Update () {

        float y = Enemy.transform.position.y-Player.transform.position.y; //rotation
        float x = Enemy.transform.position.x - Player.transform.position.x;


        if (x > 0 && y > 0)
            rb.rotation = (float)(-Mathf.Atan(x / y) * (180 / 3.1416));
        else if (x > 0 && y < 0)
            rb.rotation = (float)(-Mathf.Atan(Mathf.Abs(y) / x) * (180 / 3.1416)) - 90;
        else if (x < 0 && y > 0)
            rb.rotation = (float)(Mathf.Atan(Mathf.Abs(x) / y) * (180 / 3.1416));
        else if (x < 0 && y < 0)
            rb.rotation = (float)(Mathf.Atan(Mathf.Abs(y) / Mathf.Abs(x)) * (180 / 3.1416)) + 90;

        transform.position = Player.transform.position;

    }
}
