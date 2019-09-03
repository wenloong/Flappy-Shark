using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverWindow : MonoBehaviour
{
    private Text scoreText;

    private void Awake()
    {
        scoreText = transform.Find("scoreText").GetComponent<Text>();
        Hide();
    }

    private void Start()
    {
        Shark.GetInstance().OnDied += Shark_OnDied;
    }

    private void Shark_OnDied(object sender, System.EventArgs e) 
    {
        scoreText.text = Level.GetInstance().GetPipePassedCount().ToString();
        Show();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

}
