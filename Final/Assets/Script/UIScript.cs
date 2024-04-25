using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{

    GameObject player;
    playerCupcake playerScript;
    GameObject speedLine;
    GameObject speedText;
    float maxVel = Mathf.Sqrt(3); //max velocity for velocity input
    public float numberDisplayTime = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        playerScript = player.GetComponent<playerCupcake>();
        speedLine = GameObject.Find("SpeedLine");
        speedText = GameObject.Find("SpeedText");
        StartCoroutine(numbersLessFast());
    }

    // Update is called once per frame
    //void Update()
    //{
    //    //float curSpeed = Mathf.Abs(playerScript.velocityInput.magnitude*30);
    //    //float transparency = curSpeed / maxVel;
    //    //speedLine.GetComponent<RawImage>().color = new Color(255, 255, 255, transparency);
    //    //speedText.GetComponent<TextMeshProUGUI>().text = "Speed: " 
    //    //        + (curSpeed * playerScript.playerSpeed * Time.deltaTime ).ToString();
    //    //speedText.GetComponent<TextMeshProUGUI>().color = new Color(255, 255, 255, transparency);
    //}

    IEnumerator numbersLessFast()
    {
        yield return new WaitForSeconds(numberDisplayTime);
        float curSpeed = Mathf.Abs(playerScript.velocityInput.magnitude * 30);
        float transparency = curSpeed / maxVel;
        speedLine.GetComponent<RawImage>().color = new Color(255, 255, 255, transparency);
        speedText.GetComponent<TextMeshProUGUI>().text = "Speed: "
                + (curSpeed * playerScript.playerSpeed * Time.deltaTime).ToString();
        speedText.GetComponent<TextMeshProUGUI>().color = new Color(255, 255, 255, transparency);
        StartCoroutine(numbersLessFast());
    }
}
