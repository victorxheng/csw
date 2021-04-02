using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreamloLeaderboard : MonoBehaviour
{
    private string privateCode = "_R6rwl46WEuBbwsv4D_R_QvWxfv-JpTUCEdPJXvhrh_g";
    private string publicCode = "5d8f8708d1041303ecf75bcd";
    string dreamloWebserviceURL = "http://dreamlo.com/lb/";


    string highScore = "";

    public LeaderboardMenu lb;

    ////////////////////////////////////////////////////////////////////////////////////////////////

    // A player named Carmine got a score of 100. If the same name is added twice, we use the higher score.
    // http://dreamlo.com/lb/(your super secret very long code)/add/Carmine/100

    // A player named Carmine got a score of 1000 in 90 seconds.
    // http://dreamlo.com/lb/(your super secret very long code)/add/Carmine/1000/90

    // A player named Carmine got a score of 1000 in 90 seconds and is Awesome.
    // http://dreamlo.com/lb/(your super secret very long code)/add/Carmine/1000/90/Awesome

    ////////////////////////////////////////////////////////////////////////////////////////////////


    public struct Score
    {
        public string playerName;
        public float score;
    }


    void Start()
    {
        highScore = "";
    }


    public static double DateDiffInSeconds(System.DateTime now, System.DateTime olderdate)
    {
        var difference = now.Subtract(olderdate);
        return difference.TotalSeconds;
    }

    System.DateTime _lastRequest = System.DateTime.Now;
    int _requestTotal = 0;


    bool TooManyRequests()
    {
        var now = System.DateTime.Now;

        if (DateDiffInSeconds(now, _lastRequest) <= 2)
        {
            _lastRequest = now;
            _requestTotal++;
            if (_requestTotal > 3)
            {
                Debug.LogError("DREAMLO Too Many Requests. Am I inside an update loop?");
                return true;
            }

        }
        else
        {
            _lastRequest = now;
            _requestTotal = 0;
        }

        return false;
    }

    public void AddScore(string playerName)
    {
        if (TooManyRequests()) return;

        StartCoroutine(AddScoreWithPipe(playerName));

    }

    // This function saves a trip to the server. Adds the score and retrieves results in one trip.
    IEnumerator AddScoreWithPipe(string player)
    {
        player = Clean(player);
        float kills = PlayerPrefs.GetInt("kills", 0);
        float deaths = PlayerPrefs.GetInt("deaths", 0);
        float kdr;
        if (deaths != 0)
            kdr = kills / deaths;
        else
            kdr = kills / 1; // round hundreths
        kdr *= 100;
        WWW www = new WWW(dreamloWebserviceURL + privateCode + "/add-pipe/" + WWW.EscapeURL(player) + "/" + kdr.ToString("0"));
        yield return www;
        if (lb.uploadSuccessfulText)
            lb.outputText.text = "UPLOAD SUCCESSFUL";
        LoadScores(true);
    }

    IEnumerator GetScores(bool refresh)
    {
        highScore = "";
        WWW www = new WWW(dreamloWebserviceURL + publicCode + "/pipe");
        yield return www;
        

        highScore = www.text;

        lb.formatScores(refresh);
    }
    public void DeletePrevious(string name)
    {
        StartCoroutine(DeleteScore(name));
        lb.UploadScores();
    }
    IEnumerator DeleteScore(string name)
    {
        WWW www = new WWW(dreamloWebserviceURL + privateCode + "/delete/" + WWW.EscapeURL(name));
        yield return www;
    }
    /*
	IEnumerator GetSingleScore(string playerName)
	{
		highScores = "";
		WWW www = new WWW(dreamloWebserviceURL +  publicCode  + "/pipe-get/" + WWW.EscapeURL(playerName));
		yield return www;
		highScores = www.text;
	}
	*/
    public void LoadScores(bool refresh)
    {
        if (TooManyRequests()) return;
        StartCoroutine(GetScores(refresh));
    }


    public string[] ToStringArray()
    {
        if (this.highScore == null) return null;
        if (this.highScore == "") return null;

        string[] rows = this.highScore.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        return rows;


    }

    public List<Score> ToListLowToHigh()
    {
        Score[] scoreList = this.ToScoreArray();

        if (scoreList == null) return new List<Score>();

        List<Score> genericList = new List<Score>(scoreList);

        genericList.Sort((x, y) => x.score.CompareTo(y.score));

        return genericList;
    }

    public List<Score> ToListHighToLow()
    {
        Score[] scoreList = this.ToScoreArray();

        if (scoreList == null) return new List<Score>();

        List<Score> genericList = new List<Score>(scoreList);

        genericList.Sort((x, y) => y.score.CompareTo(x.score));

        return genericList;
    }
    public Score[] ToScoreArray()
    {
        string[] rows = ToStringArray();
        if (rows == null) return null;

        int rowcount = rows.Length;

        if (rowcount <= 0) return null;

        Score[] scoreList = new Score[rowcount];

        for (int i = 0; i < rowcount; i++)
        {
            string[] values = rows[i].Split(new char[] { '|' }, System.StringSplitOptions.None);

            Score current = new Score();
            current.playerName = values[0];
            current.score = 0;
            if (values.Length > 1) current.score = System.Convert.ToInt64(values[1]);
            scoreList[i] = current;
        }

        return scoreList;
    }
    

    // Keep pipe and slash out of names

    string Clean(string s)
    {
        s = s.Replace("/", "");
        s = s.Replace("|", "");
        s = s.Replace("+", " ");
        return s;

    }

    int CheckInt(string s)
    {
        int x = 0;

        int.TryParse(s, out x);
        return x;
    }
}
