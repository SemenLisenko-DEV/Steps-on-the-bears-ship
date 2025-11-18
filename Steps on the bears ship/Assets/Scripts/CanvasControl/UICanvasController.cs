using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICanvasController : MonoBehaviour
{
    static public bool isPause = false;
    static public UICanvasController instance;
    [Serializable]
    public struct ButtonBind
    {
        public string keyName;
        public string canvasName;
    }
    [SerializeField] public bool needPause = false;
    [SerializeField] public ButtonBind[] buttonBinds;
    [SerializeField] public GameObject[] Canvases;
    [SerializeField] public int curActiveCanvas = -1;
    
    private List<IEnumerator> binds = new List<IEnumerator>();
    public void Start()
    {
        instance = this;
        for (int i = 0; i < buttonBinds.Length; i++)
        {
            binds.Add(bindChecker(buttonBinds[i]));
            StartCoroutine(binds[i]);
        }
        if (needPause)
        {
            DisableCanvases();
        }
    }
    public IEnumerator bindChecker(ButtonBind buttonBind)
    {
        yield return new WaitUntil(() => Input.GetButtonDown(buttonBind.keyName));
        AudioSource buttonSound;
        if (TryGetComponent(out buttonSound) && FindActiveCanvas(buttonBind.canvasName) != curActiveCanvas)
        {
            buttonSound.Play();
            SetActiveCanvas(buttonBind.canvasName);
        }
        else
        {
            DisableCanvases();
        }
        yield return new WaitUntil(() => Input.GetButtonUp(buttonBind.keyName));
        StartCoroutine(bindChecker(buttonBind));
    }
    public void SetActiveCanvas(int ID)
    {
        if(curActiveCanvas == ID)
        {
            return;
        }
        DisableCanvases();
        Canvases[ID].SetActive(true);
        curActiveCanvas = ID;
        isPause = true;
        if (needPause)
        {
            Time.timeScale = 0f;
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void SetActiveCanvas(string name)
    {
        if (curActiveCanvas == FindActiveCanvas(name))
        {
            return;
        }
        DisableCanvases();
        int id = FindActiveCanvas(name);
        if (id == -1) 
        {
            return; 
        }
        Canvases[id].SetActive(true);
        curActiveCanvas = id;
        isPause = true;
        if (needPause)
        {
            Time.timeScale = 0f;
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public int FindActiveCanvas(string name)
    {
        int id = -1;
        for (int i = 0; i < Canvases.Length; i++)
        {
            if (Canvases[i].name == name)
            {
                id = i;
            }
        }
        return id;
    }
    public void DisableCanvases()
    {
        for (int i = 0; i < Canvases.Length; i++)
        {
            Canvases[i].SetActive(false);
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isPause = false;
        curActiveCanvas = -1;
        Time.timeScale = 1f;
    }
}
