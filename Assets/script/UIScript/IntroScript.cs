using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroScript : MonoBehaviour
{
    public float Introtime = 53f;
    public float Skiptime = 5f;
    private bool canSkip = false;
    public GameObject UISkip;
    private void Start()
    {
        StartCoroutine(waitforIntro());
        StartCoroutine(waitforSkip());
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)&&canSkip == true)
        {
            SceneManager.LoadScene(1);
        }
    }
    IEnumerator waitforSkip()
    {
        yield return new WaitForSeconds(Skiptime);
        canSkip = true;
        UISkip.SetActive(true);

    }
    IEnumerator waitforIntro()
    {
        yield return new WaitForSeconds(Introtime);
        SceneManager.LoadScene(1);
    }
}
