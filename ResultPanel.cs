using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResultPanel : MonoBehaviour {

    public Text resultText;
    public GameObject panel;
    public Text text;
    public Text kdrText;
    bool set = false;
    float kills;
    float deaths;
    float kdr;

    public void onShow(string txt)
    {
        resultText.text = txt;
        kills = PlayerPrefs.GetInt("kills", 0);
        deaths = PlayerPrefs.GetInt("deaths", 0);
        if (deaths != 0)
            kdr = kills / deaths;
        else
            kdr = kills / 1; // round hundreths
        kdrText.text = "KILLS: " + kills + " | DEATHS: " + deaths + " | KDR: " + kdr.ToString("0.##");
        
    }
	public void onMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void onRestart()
    {
        panel.SetActive(false);
        StartCoroutine(RandomLoadScreen());
        StartCoroutine(FindingMatchDots());
    }
    IEnumerator RandomLoadScreen()
    {
        yield return new WaitForSeconds(Random.Range(0.8f, 8.5f));
        set = true;
        text.text = "CONNECTING...";
        yield return new WaitForSeconds(Random.Range(0.5f, 2.0f));
        text.text = "LOADING...";
        yield return new WaitForSeconds(Random.Range(0.2f, 0.9f));
        SceneManager.LoadScene(1);
    }
    IEnumerator FindingMatchDots()
    {
        int dotNumber = 1;
        while (!set)
        {
            if (dotNumber == 1)
            {
                text.text = "FINDING MATCH.";
                dotNumber = 2;
            }
            else if (dotNumber == 2)
            {
                text.text = "FINDING MATCH..";
                dotNumber = 3;
            }
            else
            {

                text.text = "FINDING MATCH...";
                dotNumber = 1;
            }
            yield return new WaitForSeconds(0.25f);
        }
    }
}
