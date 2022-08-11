using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InformacaoMatch
{
    public List<griditem> match;
    public int matchComecoX;
    public int matchFinalX;
    public int matchComecoY;
    public int matchFinalY;

    public bool MatchValido
    {
        get { return match != null; }
    }
}
