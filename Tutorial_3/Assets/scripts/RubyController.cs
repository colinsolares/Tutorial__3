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

    //levels 
    public static int level;

    
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
        level = 1;
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
           
            
            SceneManager.LoadScene("MainScene"); 
            
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
                character.DisplayDialog();
                }  
            }

            if (scoreFixed >= 6)
            {
                SceneManager.LoadScene("Stage_2");
                level = 2;
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            
            audioSource.clip = BackGroundMusic;
            audioSource.Stop();

            audioSource.clip = WinClip;
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
        if (level == 1)
        { 
            if (scoreFixed >= 6)
            {
            WinTextObject1.SetActive(true);
            
            Destroy(gameObject.GetComponent<SpriteRenderer>());

            audioSource.clip = BackGroundMusic;
            audioSource.Stop();

            audioSource.clip = WinClip;
            audioSource.Play();
                
            gameOver = true; 
            }
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


}
