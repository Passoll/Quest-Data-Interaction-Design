using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class OVRHM_MenuButton : MonoBehaviour, IOVRHandMenu
{
    public Vector3 offsetFromHand;
    protected CustomHandPlane m_plane;
    [SerializeField, Optional] private PlayableDirector menu_playable;

    public GameObject F_Menu;
    public GameObject Trans_Menu;
    public GameObject Sel_Menu;

    private void Awake()
    {
        gameObject.SetActive(true);
        F_Menu.SetActive(false);
        Trans_Menu.SetActive(false);
        Sel_Menu.SetActive(false);
    }

    public void SetPlane(CustomHandPlane plane)
    {
        m_plane = plane;
    }

    //All base viz
    public void SetVisibility(bool state)
    {
        if (!state)
        {
            F_Menu.SetActive(state);
            Trans_Menu.SetActive(state);
            Sel_Menu.SetActive(state);
        }
        else
        {
            F_Menu.SetActive(state);
            F_Menu.transform.localPosition = new Vector3(0,0 ,0);
            play_menuappear();
        }
    }

  
    public void ToggleVisibility()
    {
        F_Menu.SetActive(!F_Menu.activeSelf);
    }

    public void SetMenuViz_Transform(bool state)
    {
        play_menudisappear();
        if (state)
        {
            StartCoroutine(Setobjactive(F_Menu, Trans_Menu));
            //F_Menu.transform.position = new Vector3(100000,100000,100000);
        }
        
    }
    
    public void SetMenuViz_Select(bool state)
    {
        play_menudisappear();
        if (state)
        {
            StartCoroutine(Setobjactive(F_Menu, Sel_Menu));
            //F_Menu.transform.position = new Vector3(100000,100000,100000);
        }
    }
    
    
    public void UpdatePosHand()
    {
        if (gameObject.activeSelf)
        {
            transform.position = m_plane.startPos + m_plane.offsetVec * offsetFromHand.x 
                                                  + m_plane.alignVec * offsetFromHand.y 
                                                  + m_plane.handVec * offsetFromHand.z;
            transform.rotation = Quaternion.LookRotation(m_plane.handVec, m_plane.alignVec);
        }
    }
    
    private void play_menuappear()
    {
        if (menu_playable != null)
        {
            menu_playable.Play();
        }
    }
    private void play_menudisappear()
    {
        menu_playable.time = 0.25f;
        if (menu_playable != null)
        {
            StartCoroutine(PlayableRewind(menu_playable));
        }
    }
    
    private IEnumerator Setobjactive(GameObject falsemenu, GameObject truemenu)
    {
        yield return new WaitForSeconds(0.25f);  // waits for 10 milliseconds
        falsemenu.SetActive(false);
        truemenu.SetActive(true);
    }
    
    private IEnumerator PlayableRewind(PlayableDirector _playable)
    {
        yield return new WaitForSeconds(Time.deltaTime);
        _playable.time -= 1.0f * Time.deltaTime;  //1.0f是倒帶速度
        _playable.Evaluate();
        if (_playable.time < 0f)
        {
            _playable.time = 0f;
            _playable.Evaluate();
        }
        else
        {
            StartCoroutine(PlayableRewind(_playable));
        }
    }
    
    
    
    
}
