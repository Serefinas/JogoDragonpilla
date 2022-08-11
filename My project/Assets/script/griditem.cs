using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class griditem : MonoBehaviour
{
    public int x
    {
        get;
        private set;
    }
    public int y
    {
        get;
        private set;
    }
    
    public int id;
    
    public void MudarPosicao(int nX, int nY)
    {
        x = nX;
        y = nY;
        gameObject.name = string.Format("Sprite[{0}][{1}]", x, y);
    }
    void OnMouseDown ()
    {
        if(ClickMouseEvento != null )
        {
            ClickMouseEvento(this);
        }
    }
    public delegate void ClickMouse (griditem item);
    public static event ClickMouse ClickMouseEvento;

}
