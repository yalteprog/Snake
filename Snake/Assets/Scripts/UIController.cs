using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    private int size;
    private string type;
    public GameObject panel1;
    public GameObject panel2;
    public GameObject panel3;
   
    public bool settingsReady;

    public void setSize(int size_) { size = size_; }
    public void setType(string type_) { type = type_; }
    public void showPanel(GameObject pan_op)
    {
        pan_op.gameObject.SetActive(true);
    }
    public void closePanel(GameObject pan_cl)
    {
        pan_cl.gameObject.SetActive(false);
    }
    public int getSize() { return size; }

    public string getType() { return type; }

    public IEnumerator showStartMenu()
    {
        settingsReady = false;
        panel1.gameObject.SetActive(true);
        panel2.gameObject.SetActive(false);
        panel3.gameObject.SetActive(false);
        yield return new WaitWhile(() => !settingsReady);
        yield return null;

    }
    public bool areSettingsReady() { return settingsReady; }
    public void setSettingsReady() { settingsReady = true; }
    public void setRestart() { settingsReady = false;

        size = 0;
        type = "";
        

    }
   public IEnumerator showDeathMenu()
    {
        settingsReady = false;
        panel1.gameObject.SetActive(false);
        panel2.gameObject.SetActive(false);
        panel3.gameObject.SetActive(true);
        yield return new WaitWhile(() => size>0);
        yield return StartCoroutine(showStartMenu());
        yield return null;
    }
    public void exit() { Application.Quit(); }
}
