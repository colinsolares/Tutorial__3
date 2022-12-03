using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class RubyController : MonoBehaviour
{
     public float speed = 3.0f;
    public int maxHealth = 5;
    public float timeInvincible = 2.0f;
     public int health { get { return currentHealth; }}
    int currentHealth;
     bool isInvincible;
    float invincibleTimer;
     Rigidbody2D rigidbody2d;
    float horizontal; 
    float vertical;
    Animator animator;
    Vector2 lookDirection = new Vector2(1,0);
    // COG and Ammo
    public GameObject projectilePrefab;
    public int ammo { get { return currentAmmo; }}

    public AudioClip QuestComplete { get; private set; }

    public int currentAmmo = 6;
     // Ammo Text UI
    public TextMeshProUGUI ammoText;
    
    // sounds
    AudioSource audioSource;
     public AudioClip BackGroundMusic;
    public AudioClip throwSound;
    public AudioClip hitSound;
    public AudioClip WinClip;
    public AudioClip GameOver;
    public AudioClip Jambi;
    public AudioClip tickingSound;
    
    // Speed Boost Timer
    public float timeBoosting = 4.0f;
    float speedBoostTimer;
    bool isBoosting;

    
  
    // Health and Damage Particles
    public ParticleSystem healthEffect;
    public ParticleSystem damageEffect;

    // fixed robots
    public TextMeshProUGUI fixedText;
    private int scoreFixed = 0;


    // Win text and Lose Text and Restart bool
    public GameObject WinTextObject1;
    public GameObject  WinTextObject2;
    public GameObject LoseTextObject;
    public GameObject ammoTextObject;
    bool gameOver;
    bool winGame;

    //levels 
    public static int level = 1;

    // Variables for Timer
    [SerializeField] private TextMeshProUGUI timerUI; // Attach TMP object to this slot
    [SerializeField] private float mainTimer; // Change this value to your liking in Unity

    private float timer;
    private bool canCount = false;
    private bool doOnce = false;
    private bool hasPressedKey = false;
    private bool hasMoved = false;

    public GameObject TimerObject; // This is your Timer text in the Canvas

    
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();

        // animator
        animator = GetComponent<Animator>();
        //Health
        currentHealth = maxHealth;
        currentHealth = 5;
        //Audio
        audioSource= GetComponent<AudioSource>();
        audioSource.clip = BackGroundMusic;
        audioSource.Play();

        // Fixed Robot Text
        fixedText.text = "Fixed Robots: " + scoreFixed.ToString() + "/6";

    

        // Win Text and Lose text set to false, as well as restart bool
        WinTextObject1.SetActive(false);
        WinTextObject2.SetActive(false);
        LoseTextObject.SetActive(false);
        gameOver = false;
        winGame = false;
        

        
        // Timer
        timer = mainTimer;
        TimerObject.SetActive(false);
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    // Update is called once per frame
    void Update()
    {   
         // Close Game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

         // Restart game
        if (Input.GetKeyDown(KeyCode.R))
        {
            
            {
                if (gameOver == true)
            
                {

                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // this loads the currently active scene

                }
            }
            
        }
         if (Input.GetKeyDown(KeyCode.K))
        {
           
            
            SceneManager.LoadScene("Level 1"); 
            level = 1;
            
        } 

        // Speed Boost Timer
        if (isBoosting == true)
        {
            speedBoostTimer -= Time.deltaTime; // Once speed boost activates, it counts down
            speed = 8;
        
            if (speedBoostTimer < 0)
            {
                isBoosting = false;
                speed = 5; 
            }
        }
        
        // ruby animation
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        
        Vector2 move = new Vector2(horizontal, vertical);
        
        if(!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }
        
        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        // decrease health decay
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }

          if(Input.GetKeyDown(KeyCode.C))
        {
            
            if (currentAmmo > 0)
            {
                Launch();
                ChangeAmmo(-1);
                AmmoText();
            }
           
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                  PlaySound(Jambi);

                character.DisplayDialog();
                }  
            }
            

            if (scoreFixed >= 6)
            {
                SceneManager.LoadScene("Level 2");
                level = 2;
            }
        }
         if (Input.GetKeyDown(KeyCode.I))
         {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("Dog"));
            if (hit.collider != null)
            {
                
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                  PlaySound(Jambi);

                character.DisplayDialog();
                }  
            }
         }

          if (Input.GetKeyDown(KeyCode.P))
          {
             RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("Cat"));
            if (hit.collider != null)
            {
                
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                  PlaySound(Jambi);

                character.DisplayDialog();
                }  
            }
          }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            
            audioSource.clip = BackGroundMusic;
            audioSource.Stop();

             audioSource.clip = tickingSound;
            audioSource.Stop();

            audioSource.clip = WinClip;
            audioSource.Play();
        }


          // Timer Main Code
        // Press this key to activate timer
        if (hasMoved == false)
        {
            if (hasPressedKey == false)
            {
                if (Input.GetKeyDown(KeyCode.T))
                {
                    timer = mainTimer;
                    canCount = true;
                    doOnce = false;
                    TimerObject.SetActive(true);

                    // Ticking Sound Starts
                    PlaySound(tickingSound);
            
                    hasPressedKey = true;
                }
            }
        }

         // Timer functionality
        if (timer >= 0.0f && canCount)
        {
            timer -= Time.deltaTime;
            timerUI.text = "Time: " + timer.ToString("F");
        }

         else if (timer <= 0.0f && !doOnce)
        {
            canCount = false;
            doOnce = true;
            timerUI.text = "Time: " + timer.ToString("0.00");

            // Lose State
            LoseTextObject.SetActive(true);

            transform.position = new Vector3(-5f, 0f, -100f);
            speed = 0;
            Destroy(gameObject.GetComponent<SpriteRenderer>());

            gameOver = true;

            // BackgroundMusicManager is turned off
            audioSource.clip = BackGroundMusic;
            audioSource.Stop();

           

            // Ticking Sound Stops
            audioSource.clip = tickingSound;
            audioSource.Stop();

             // Calls sound script and plays lose sound
            audioSource.clip = GameOver;
            audioSource.Play();
        }

        
    }

    
     void FixedUpdate()
     {
       float horizontal = Input.GetAxis("Horizontal");
       float vertical = Input.GetAxis("Vertical");
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
     }

  

     public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            animator.SetTrigger("Hit");
            if (isInvincible)
            
                return;
             
            isInvincible = true;
            invincibleTimer = timeInvincible;

            PlaySound(hitSound);

             // Damage Particle effect
            damageEffect = Instantiate(damageEffect, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        }
        
        if (amount > 0)
        {
           healthEffect = Instantiate(healthEffect, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        }
           
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth); 

        // Ruby loses all health, lose text appears and restart becomes true
        if (currentHealth <= 0)
        {
            LoseTextObject.SetActive(true);
            if (level ==1 )
            {
                WinTextObject1.SetActive(false);
            }

            transform.position = new Vector3(-5f, 0f, -100f);
            speed = 0;
            Destroy(gameObject.GetComponent<SpriteRenderer>());

            gameOver = true;

            audioSource.clip = BackGroundMusic;
            audioSource.Stop();
            audioSource.clip = GameOver;
            audioSource.Play();
        }

        

        
    }

    void Launch()   
    {
    GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

    Projectile projectile = projectileObject.GetComponent<Projectile>();
    projectile.Launch(lookDirection, 300);

    animator.SetTrigger("Launch");

    PlaySound(throwSound);
    }

     //Fixed robots
     public void FixedRobots(int amount)
     {
        scoreFixed += amount;
        fixedText.text = "Fixed Robots: " + scoreFixed.ToString() + "/6";

        Debug.Log("Fixed Robots: " + scoreFixed);
        
         // Win Text1 Appears
        if (scoreFixed == 6 && level == 1)
        { 
            
            WinTextObject1.SetActive(true);
            canCount = false;
            doOnce = true;

           
            
        }   
           
           
        if (scoreFixed == 6 && level == 2)
        { 

             WinTextObject1.SetActive(true);
            winGame = true;
            Destroy(gameObject.GetComponent<SpriteRenderer>());
             
             // Disables time attack on level
            canCount = false;
            doOnce = true;

            // Ticking Sound Stops
           

            audioSource.clip = BackGroundMusic;
            audioSource.Stop();

            audioSource.clip = tickingSound;
            audioSource.Stop();

            audioSource.clip = WinClip;
            audioSource.Play();
                
            gameOver = true; 
        }
            
        
       
        
     }

      public void AmmoText()
    {
        ammoText.text = "Ammo: " + currentAmmo.ToString();
    }

    public void ChangeAmmo(int amount)
    {
        // Ammo math code
        currentAmmo = Mathf.Abs(currentAmmo + amount);
        Debug.Log("Ammo: " + currentAmmo);
    }


      public void SpeedBoost(int amount)
    {
        if (amount > 0)
        {
            speedBoostTimer = timeBoosting;
            isBoosting = true;
        }
    }

    

}
