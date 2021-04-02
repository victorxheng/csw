using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FindMatch : MonoBehaviour {
    public Text text;
    public GameObject Menu;
    bool set = false;
	// Use this for initialization
	void Start () {
        Menu.SetActive(false);
        StartCoroutine(RandomLoadScreen());
        StartCoroutine(FindingMatchDots());
	}
	IEnumerator RandomLoadScreen()
    {
        yield return new WaitForSeconds(Random.Range(0.8f,8.5f));
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
            if(dotNumber == 1)
            {
                text.text = "FINDING MATCH.";
                dotNumber = 2;
            }
            else if(dotNumber == 2)
            {
                text.text = "FINDING MATCH..";
                dotNumber = 3;
            }
            else{

                text.text = "FINDING MATCH...";
                dotNumber = 1;
            }
            yield return new WaitForSeconds(0.25f);
        }
    }
}
