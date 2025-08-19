using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Using
using TMPro;
using UnityEngine.SceneManagement;


public class player : MonoBehaviour
{
    //Variables being used
    public float velocity = 1;
    private Rigidbody2D rb;
    public GameObject playerBody;
    public bool spinning;

    public bool doubleJumpActive;
    public int doublJumpUsed;
    public float doubleJumpTimer;

    public GameObject boomCyan;
    public GameObject boomGreen;
    public GameObject boomGold;

    public bool goldPower;
    public float goldPowerTimer;

    public bool gameOver;

    public TextMeshProUGUI TimeText;
    public float timer;

    public GameObject gameOverScreen;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerBody.GetComponent<SpriteRenderer>().color = Color.cyan;
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver == false)
        {
            timer += Time.deltaTime;
            DisplayTime(timer);

            //Player jumping
            if (doubleJumpActive)
            {
                if (Input.GetMouseButtonDown(0) && spinning == true || Input.GetMouseButtonDown(0) && doublJumpUsed == 1)
                {
                    rb.linearVelocity = Vector2.up * velocity;
                    doublJumpUsed++;
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0) && spinning == true)
                {
                    rb.linearVelocity = Vector2.up * velocity;
                }
            }

            //rotating block
            if (spinning == false)
            {
                playerBody.transform.Rotate(0, 0, -700 * Time.deltaTime);
            }
            else
            {
                playerBody.transform.Rotate(0, 0, -350 * Time.deltaTime);
            }
        }
        else
        {
            gameOverScreen.SetActive(true);
        }
    }

    //All Player triggers
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("block") && gameOver == false)
        {
            collision.GetComponent<SpriteRenderer>().color = playerBody.GetComponent<SpriteRenderer>().color;
        }
        if (collision.gameObject.CompareTag("ground"))
        {
            spinning = true;
            doublJumpUsed = 0;
        }
        //power ups
        if (collision.gameObject.CompareTag("doubleJumpPowerUp"))
        {
            playerBody.GetComponent<SpriteRenderer>().color = Color.green;
            doubleJumpActive = true;
            goldPower = false;
            StartCoroutine(powerGreen());
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.CompareTag("goldPowerUp"))
        {
            playerBody.GetComponent<SpriteRenderer>().color = Color.yellow;
            goldPower = true;
            doubleJumpActive = false;
            StartCoroutine(powerGold());
            Destroy(collision.gameObject);
        }
        //enemies
        if (collision.gameObject.CompareTag("spike") && goldPower == false || collision.gameObject.CompareTag("outOfBounds"))
        {
            if (doubleJumpActive == true)
            {
                Instantiate(boomGreen, transform.position, Quaternion.Euler(new Vector3(0, 0, 0)));
                gameOver = true;
            }
            else if (goldPower == true)
            {
                Instantiate(boomGold, transform.position, Quaternion.Euler(new Vector3(0, 0, 0)));
                gameOver = true;
            }
            else if (gameOver == false)
            {
                Instantiate(boomCyan, transform.position, Quaternion.Euler(new Vector3(0, 0, 0)));
                gameOver = true;
            }
            StartCoroutine(setGameOver());
            playerBody.SetActive(false);
        }
    }


    //when no longer on ground
    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("ground"))
        {
            spinning = false;
        }
    }

    //player color changing
    IEnumerator powerGold()
    {
        yield return new WaitForSeconds(15);
        playerBody.GetComponent<SpriteRenderer>().color = Color.cyan;
        goldPower = false;
        GetComponent<CircleCollider2D>().enabled = false;
        GetComponent<CircleCollider2D>().enabled = true;
    }
    IEnumerator powerGreen()
    {
        yield return new WaitForSeconds(15);
        playerBody.GetComponent<SpriteRenderer>().color = Color.cyan;
        doubleJumpActive = false;
    }

    //Game over set
    IEnumerator setGameOver()
    {
        yield return new WaitForSeconds(1);
        Time.timeScale = 0;
    }

    //Score counter
    private void DisplayTime(float timeToDisplay)
    {
        if (PlayerPrefs.GetFloat("highscore") < timeToDisplay)
        {
            PlayerPrefs.SetFloat("highscore", timeToDisplay);
        }
        var t0 = (int)timeToDisplay;
        var m = t0 / 60;
        var s = (t0 - m * 60);
        var ms = (int)((timeToDisplay - t0) * 100);

        TimeText.text = $"{m:00}:{s:00}:{ms:00}";
    }

    //Buttons
    public void playAgainB()
    {
        SceneManager.LoadScene(1);
    }
    public void mainMenuB()
    {
        SceneManager.LoadScene(0);
    }

}
