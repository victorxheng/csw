using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class healthManager : MonoBehaviour {

    private float startingHealth = 20;
    private float waitForHeal = 5.0f;
    private float waitBetweenIncrement = 1;
    private int healIncrement = 1;

    private bool healStatus = false;
    private bool enemyHealStatus = false;

    public float playerHealth;
    public float enemyHealth;

	// Use this for initialization
	void Awake () {
        playerHealth = startingHealth;
        enemyHealth = startingHealth;
	}
    public void heal(string type)
    {
        if (!death)
        {
            if (type == "player")
                StartCoroutine(healCyclePlayer(playerHealth, type));
            else
                StartCoroutine(healCyclePlayer(enemyHealth, type));
        }
    }
    IEnumerator healCyclePlayer(float inputHealth, string type)
    {
        yield return new WaitForSeconds(waitForHeal);
        
        if(type == "player")
        {
            if (inputHealth == playerHealth)
            {
                int increments = 0;
                while (playerHealth < startingHealth)
                {
                    if (inputHealth + increments * healIncrement != playerHealth)
                    {
                        break;
                    }
                    else if (death) break;
                    playerHealth += healIncrement;
                    increments++;

                    yield return new WaitForSeconds(waitBetweenIncrement);
                }
            }
        }
        else
        {
            if (inputHealth == enemyHealth)
            {
                int increments = 0;
                while (enemyHealth < startingHealth)
                {
                    if (inputHealth + increments * healIncrement != enemyHealth)
                    {
                        break;
                    }
                    else if (death) break;
                    enemyHealth += healIncrement;
                    increments++;

                    yield return new WaitForSeconds(waitBetweenIncrement);
                }
            }
        }     
    }


    public Text playerHealthText;
    public Text enemyHealthText;
    public bool death = false;
    public ResultPanel rp;
    public GameObject resultPanel;
    public GameObject playerHealthBar;
    public GameObject enemyHealthBar;
    public SpriteRenderer playerSprite;
    public SpriteRenderer enemySprite;
    public CapsuleCollider2D enemyCollider;

    public GameObject bloodExplosion;
    public GameObject enemy;
    
    
    private void Update()
    {
        if((playerHealth > 0 && enemyHealth > 0) && !death)
        {
            playerHealthBar.transform.localScale = new Vector2(playerHealth / startingHealth,1);
            enemyHealthBar.transform.localScale = new Vector2(enemyHealth / startingHealth, 1);
        }
        else
        {
            if(playerHealth<= 0 && !death)
            {
                playerHealthBar.transform.localScale = new Vector2(0, 1);
                death = true;
                resultPanel.SetActive(true);
                PlayerPrefs.SetInt("deaths", PlayerPrefs.GetInt("deaths") + 1);
                rp.onShow("YOU LOST");

                Instantiate(bloodExplosion, gameObject.transform.position, Quaternion.identity);
                playerSprite.enabled = false;

                GameObject audio = Instantiate(playAudio);
                audio.GetComponent<audioPlayer>().playSound = "rip";
            }
            else if (enemyHealth<=0&&!death)
            {
                enemyHealthBar.transform.localScale = new Vector2(0, 1);
                death = true;
                resultPanel.SetActive(true);
                PlayerPrefs.SetInt("kills", PlayerPrefs.GetInt("kills") + 1);
                rp.onShow("YOU WIN!");

                Instantiate(bloodExplosion, enemy.transform.position, Quaternion.identity);
                enemySprite.enabled = false;
                enemyCollider.enabled = false;
                GameObject audio = Instantiate(playAudio);
                audio.GetComponent<audioPlayer>().playSound = "reward";
            }
        }
    }
    public GameObject playAudio;
    public void playerDamage()
    {
        GameObject audio = Instantiate(playAudio);
        audio.GetComponent<audioPlayer>().playSound = "hit";
        playerHealth -= 2;
        heal("player");
    }
    public void enemyDamage()
    {
        GameObject audio = Instantiate(playAudio);
        audio.GetComponent<audioPlayer>().playSound = "hit";
        enemyHealth -= 2;
        heal("enemy");
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "EnemyBullet")
        playerDamage();
    }
}
