using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private const string FILE_DIR = "/DATA/";
    private const string DATA_FILE = "highScores.txt";
    private string FILE_FULL_PATH;
    
    private int score;

    public int Score
    {
        get
        {
            return score;
        }
        set
        {
            score = value;
        }
    }

    private string highScoresString = "";

    private List<int> highScores;
    private List<string> names;

    public List<int> HighScores
    {
        get
        {
            if (highScores == null && File.Exists(FILE_FULL_PATH))
            {
                Debug.Log("Pulling highscores from file...");
                
                highScores = new List<int>();
                names = new List<string>();
                
                // Reading data from the file
                highScoresString = File.ReadAllText(FILE_FULL_PATH);

                // Cut off the last "\n" at the end of the file
                highScoresString = highScoresString.Trim();
                
                // Make a temporary array to store the split-ed high scores
                // Each number before the \n will be hold in a slot
                string[] highScoreArray = highScoresString.Split("\n");
                
                for (int i = 0; i < highScoreArray.Length; i++)
                {
                    string[] nameAndScore = highScoreArray[i].Split(",");

                    string currentPlayerName = nameAndScore[0];
                    names.Add(currentPlayerName);
                    
                    int currentHighScore = Int32.Parse(nameAndScore[1]);
                    highScores.Add(currentHighScore);
                }
            }
            else if (highScores == null)
            {
                Debug.Log("No highscore found");
                
                highScores = new List<int>();
                names = new List<string>();
                
                // Add some data to the list as starting point
                highScores.Add(0);
                highScores.Insert(0, 3);
                highScores.Insert(1, 2);
                highScores.Insert(2, 1);
                highScores.Insert(3, 0);
                
                names.Insert(0, "Anonymous");
                names.Insert(1, "Anonymous");
                names.Insert(2, "Anonymous");
                names.Insert(3, "Anonymous");
                names.Insert(4, "Anonymous");
            }

            return highScores;
        }
        /*set
        {
            
        }*/
    }

    public TextMeshProUGUI display;
    public TextMeshProUGUI inputName;
    public TextMeshProUGUI nameSlotText;
    
    // Submit button
    public Button submit;
    
    // For storing the name
    public String playerName = "Anonymous";

    private float timer = 0;
    public int masTime = 5;

    private bool isInGame = true;
    private bool isStarted = false;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        FILE_FULL_PATH = Application.dataPath + FILE_DIR + DATA_FILE;
        
        submit.GetComponent<Button>().onClick.AddListener(NameSubmitted);
    }

    // Update is called once per frame
    void Update()
    {
        if (isInGame && isStarted)
        {
            display.text = "Score: " + score + "\n"
                           + "Time: " + (masTime - (int)timer);
        }
        else if (!isStarted)
        {
            display.text = "What's your name?";
            
            /*// See the name input staff
            name.transform.parent.transform.parent.GetComponent<Image>().enabled = true;
            nameSlotText.text = "Enter Name...";
            
            submit.GetComponent<Image>().enabled = true;
            submit.GetComponentInChildren<TextMeshProUGUI>().text = "Submit";*/
            
        }
        else
        {
            display.text = "GAME OVER" + "\n"
                + "FINAL SCORE: " + score + "\n"
                + "High Scores:\n" + highScoresString;
        }

        // Timer increases
        if (isStarted)
        {
            timer += Time.deltaTime;
        }
        
        if (timer >= masTime && isInGame)
        {
            isInGame = false;
            SceneManager.LoadScene("End");
            SetHighScore();
        }
        
        // For level debugging
        if (SceneManager.GetActiveScene().name == "Level1")
        {
            Debug.Log("Hide player input");
            NameSubmitted();
        }
    }

    bool isHighScore(int score)
    {
        for (int i = 0; i < HighScores.Count; i++)
        {
            if (highScores[i] < score)
            {
                return true;
            }
        }

        return false;
    }

    void NameSubmitted()
    {
        if (SceneManager.GetActiveScene().name == "Begin")
        {
            SceneManager.LoadScene("Level1");

            playerName = inputName.text;
            inputName.enabled = false;
            
            Debug.Log(playerName);
        }
        
        //Debug.Log("Submitting...");
        isStarted = true;
        
        // Invisible the name input staff
        inputName.transform.parent.transform.parent.GetComponent<Image>().enabled = false;
        nameSlotText.text = "";

        submit.GetComponent<Image>().enabled = false;
        submit.GetComponentInChildren<TextMeshProUGUI>().text = "";
    }

    void SetHighScore()
    {
        // Check if there's a highscore generated
        if (isHighScore(score))
        {
            int highScoreSlot = -1;
                
            // Locating the position of the slot
            for (int i = 0; i < HighScores.Count; i++)
            {
                if (score > highScores[i])
                {
                    highScoreSlot = i;
                    
                    // Cut the loop
                    break;
                }
            }
                
            // Insert the new highscore to this slot
            highScores.Insert(highScoreSlot, score);
            names.Insert(highScoreSlot, playerName);

            // Just take the first 5 high scores
            highScores = highScores.GetRange(0, 5);
            names = names.GetRange(0, 5);

            string scoreBoardText = "";

            // Go through every slot in the highScores list
            /*foreach (var highScore in highScores)
            {
                scoreBoardText += playerName + "," + highScore + "\n";
            }*/
            for (int i = 0; i < highScores.Count; i++)
            {
                scoreBoardText += names[i] + "," + highScores[i] + "\n";
            }

            highScoresString = scoreBoardText;
                
            File.WriteAllText(FILE_FULL_PATH, highScoresString);
        }
    }
}
