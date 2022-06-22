using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject canvas;
    public GameObject player;
    private ThirdPersonShooterController playerScript;
    private UIManager uiManager;
    public ZombieManager zombieManager;

    public GameObject[] civillianPrefab;

    public Transform civillianSpawnPoint;

    public GameObject introCamera;
    public GameObject introCamera2;
    public Camera outroCamera;

    public Animator trainAnimator;

    public int currentZombies = 0;
    public int maxZombies = 50;

    public int zombieKilled = 0;
    public int survivorRemained = 20;
    public int survivorArrived = 0;

    public int playerLv = 0;

    public int mgUnlock = 20;
    public int launcherUnlock = 50;
    public int sniperUnlock = 100;

    public bool gameFailed = false;

    // Start is called before the first frame update
    void Start()
    {
        uiManager = canvas.GetComponent<UIManager>();
        playerScript = player.GetComponentInChildren<ThirdPersonShooterController>();
        zombieManager.enabled = false;
        StartCoroutine(StartGame());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void KilledZombie()
    {
        currentZombies -= 1;
        zombieKilled += 1;

        if(zombieKilled >= mgUnlock && playerLv == 0)
        {
            playerScript.unlocked += 1;
            playerLv += 1;
            zombieManager.level = playerLv;
            maxZombies += 100;
            uiManager.Alert(mgUnlock);
            uiManager.unlockWeapon(1);
        }
        if (zombieKilled >= launcherUnlock && playerLv == 1)
        {
            playerScript.unlocked += 1;
            playerLv += 1;
            zombieManager.level = playerLv;
            maxZombies += 100;
            uiManager.Alert(launcherUnlock);
            uiManager.unlockWeapon(2);
        }
        if (zombieKilled >= sniperUnlock && playerLv == 2)
        {
            playerScript.unlocked += 1;
            playerLv += 1;
            zombieManager.level = playerLv;
            maxZombies += 100;
            uiManager.Alert(sniperUnlock);
            uiManager.unlockWeapon(3);
        }
    }

    public void SurvivorDead()
    {
        survivorRemained -= 1;
        if (survivorRemained == 0 && survivorArrived == 0)
        {
            gameFailed = true;
            FinishGame();
        }
    }

    public void SurvivorArrived()
    {
        survivorArrived += 1;
        if (survivorArrived == survivorRemained)
            StartCoroutine(FinishGame());
    }

    public IEnumerator StartGame()
    {
        //버스 카메라 3초동안 켜져있고
        introCamera.SetActive(true);
        player.SetActive(false);
        canvas.SetActive(false);

        yield return new WaitForSeconds(4f);

        //시민들 뛰쳐나오고
        introCamera.SetActive(false);
        introCamera2.SetActive(true);
        for (int i = 0; i < 20; i++)
        {
            Instantiate(civillianPrefab[i % 4], civillianSpawnPoint.position, Quaternion.identity);
        }

        yield return new WaitForSeconds(2f);

        //플레이어 시점 시작
        introCamera.SetActive(false);
        introCamera2.SetActive(false);
        player.SetActive(true);
        canvas.SetActive(true);
        zombieManager.enabled = true;
    }

    public IEnumerator FinishGame()
    {
        player.GetComponentInChildren<ThirdPersonShooterController>().enabled = false;
        player.GetComponentInChildren<Camera>().enabled = false;

        outroCamera.gameObject.SetActive(true);
        trainAnimator.enabled = true;

        yield return new WaitForSeconds(3f);
        uiManager.TurnResultUIOn(gameFailed);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Restart() {
        SceneManager.LoadScene("StartScene");
    }
}
