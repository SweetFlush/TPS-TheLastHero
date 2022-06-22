using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public ThirdPersonShooterController player;
    public GameManager gameManager;

    public GameObject inGameUI;
    public GameObject resultUI;

    public GameObject purposeText;
    public Text bulletText;
    public GameObject reloadText;
    public Text healthText;
    public Text killText;
    public Text survText;
    public Text alertText;
    public GameObject alertDialog;

    public Text[] weaponTexts;

    public Text survivorLeftText;
    public Text resultText;

    private void Start()
    {
        reloadText.SetActive(false);
        resultUI.SetActive(false);
        alertDialog.gameObject.SetActive(false);

        Invoke("DisablePurposeText", 2f);
    }

    // Update is called once per frame
    void Update()
    {
        bulletText.text = player.currentBullet + " / " + player.maxBullet;
        healthText.text = player.health.ToString();
        killText.text = gameManager.zombieKilled.ToString();
        survText.text = gameManager.survivorRemained.ToString();

        if (player.currentBullet == 0)
            reloadText.SetActive(true);
        else
            reloadText.SetActive(false);

        switch(player.gunNumber)
        {
            case 0:
                ActivateWeaponText(weaponTexts[0]);
                DeactivateWeaponText(weaponTexts[1]);
                DeactivateWeaponText(weaponTexts[2]);
                DeactivateWeaponText(weaponTexts[3]);
                break;
            case 1:
                ActivateWeaponText(weaponTexts[1]);
                DeactivateWeaponText(weaponTexts[0]);
                DeactivateWeaponText(weaponTexts[2]);
                DeactivateWeaponText(weaponTexts[3]);
                break;
            case 2:
                ActivateWeaponText(weaponTexts[2]);
                DeactivateWeaponText(weaponTexts[0]);
                DeactivateWeaponText(weaponTexts[1]);
                DeactivateWeaponText(weaponTexts[3]);
                break;
            case 3:
                ActivateWeaponText(weaponTexts[3]);
                DeactivateWeaponText(weaponTexts[1]);
                DeactivateWeaponText(weaponTexts[2]);
                DeactivateWeaponText(weaponTexts[0]);
                break;
        }
    }

    public void Alert(int n)
    {
        alertDialog.SetActive(true);
        alertText.text = n + "ų �޼�! \n ���ο� ���Ⱑ �رݵǾ����ϴ�! \n ���� ������ ���� �ż����ϴ�!!";

        Invoke("DisableAlert", 2f);
    }

    private void DisableAlert()
    {
        alertDialog.SetActive(false);
    }

    public void TurnResultUIOn(bool gameFailed)
    {
        Cursor.lockState = CursorLockMode.Confined;
        inGameUI.SetActive(false);
        resultUI.SetActive(true);

        if (gameFailed)
            resultText.text = "��� �����ڰ� ����Ͽ� ������ ����Ǿ����ϴ�.";
        else
            resultText.text = "���������� �ùε��� ��ȣ�ϰ� \n Ż���Ű�� �� �����߽��ϴ�!";
        survivorLeftText.text = "��Ƴ��� �ù� �� : " + gameManager.survivorArrived.ToString();
    }

    private void DisablePurposeText()
    {
        purposeText.SetActive(false);
    }

    private void ActivateWeaponText(Text text)
    {
        text.color = Color.white;
    }

    private void DeactivateWeaponText(Text text)
    {
        text.color = new Color(223f / 255f, 223f / 255f, 223f / 255f);
    }

    public void unlockWeapon(int i)
    {
        weaponTexts[i].gameObject.SetActive(true);
    }
}
