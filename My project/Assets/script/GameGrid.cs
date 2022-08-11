using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGrid : MonoBehaviour
{
    public int TamanhoX, TamanhoY;
    public float LarguraPeca = 1f;
    public float AlturaPeca = 1f;
    private GameObject[] _pecas;
    private griditem[,] _itens;
    private griditem ItemSelecionado;
    public float Velocidade = 0.1f;

    // Start is called before the first frame update
    void Start()
    { 
        PegaPecas();
        FillGrid();
        LimpaGrid();
        griditem.ClickMouseEvento += ClickMouse;
    }

    void Desabilita()
    {
        griditem.ClickMouseEvento -= ClickMouse;
    }

    void FillGrid()
    {
        _itens = new griditem[TamanhoX, TamanhoY];
        for (int x = 0; x < TamanhoX; x++)
        {
            for (int y = 0; y < TamanhoY; y++)
            {
                _itens[x,y] = InstanciaPecas(x, y);
            }
        }
    }

    void LimpaGrid()
    {
        for (int x = 0;x < TamanhoX; x++)
        {
            for(int y = 0; y < TamanhoY; y++)
            {
                InformacaoMatch matchInfo = PegaInformacao(_itens[x,y]);
                if (matchInfo.MatchValido)
                {
                    Destroy(_itens[x,y].gameObject);
                    _itens [x,y] = InstanciaPecas(x, y);
                    y--;
                }
            }
        }
    }

    griditem InstanciaPecas(int x, int y)
    {
        GameObject PecaAleatoria = _pecas[Random.Range(0, _pecas.Length)];
        griditem NovaPeca = ((GameObject) Instantiate (PecaAleatoria, new Vector3(x * LarguraPeca, y * AlturaPeca), Quaternion.identity)).GetComponent<griditem>();
        NovaPeca.MudarPosicao(x,y);
        return NovaPeca;
    }

    void ClickMouse(griditem item)
    {
        if (ItemSelecionado == item)
        {
            return;
        }
        if (ItemSelecionado == null)
        {
            ItemSelecionado = item;
        }
        else
        {
            float xDiff = Mathf.Abs(item.x - ItemSelecionado.x);
            float yDiff = Mathf.Abs(item.y - ItemSelecionado.y);
            if (xDiff+yDiff == 1)
            {
                StartCoroutine(TentaMatch(ItemSelecionado, item));
            }
            else
            {
                Debug.LogError("Movimento invalido");
            }
            ItemSelecionado = null;
        }
    }

    IEnumerator TentaMatch(griditem a, griditem b)
    {
        yield return StartCoroutine(troca(a, b));
        InformacaoMatch MatchA = PegaInformacao(a);
        InformacaoMatch MatchB = PegaInformacao(b);
        if (!MatchA.MatchValido && !MatchB.MatchValido)
        {
            yield return StartCoroutine(troca(a,b));
            yield break;
        }
        if (MatchA.MatchValido)
        {
            yield return StartCoroutine(DestroiItem(MatchA.match));
            yield return new WaitForSeconds(Velocidade);
            yield return StartCoroutine(AtualizaGrid(MatchA));
        }
        else if (MatchB.MatchValido)
        {
            yield return StartCoroutine(DestroiItem(MatchB.match));
            yield return new WaitForSeconds(Velocidade);
            yield return StartCoroutine(AtualizaGrid(MatchB));
        }
    }

    IEnumerator AtualizaGrid(InformacaoMatch match)
    {
        if (match.matchComecoY == match.matchFinalY)
        {
            for (int x = match.matchComecoX;x <= match.matchFinalX;x++)
            {
                for (int y = match.matchComecoY;y < TamanhoY-1;y++)
                {
                    griditem IndiceAcima = _itens[x, y + 1];
                    griditem atual = _itens[x, y];
                    _itens[x, y] = IndiceAcima;
                    _itens[x, y + 1] = atual;
                    _itens[x,y].MudarPosicao (_itens[x,y].x, _itens[x,y ].y-1);
                }
                _itens[x,TamanhoY-1] = InstanciaPecas(x, TamanhoY - 1);
            }
        }
        else if (match.matchComecoX == match.matchFinalX)
        {
            int AlturaMatch = 1+ (match.matchFinalY - match.matchComecoY);
            for (int y = match.matchComecoY + AlturaMatch;y <= TamanhoY-1;y++ )
            {
                griditem IndiceAbaixo = _itens[match.matchComecoX, y - AlturaMatch];
                griditem atual = _itens[match.matchComecoX, y];
                _itens[match.matchComecoX, y - AlturaMatch] = atual;
                _itens[match.matchComecoX, y] = IndiceAbaixo;
            }
            for (int y = 0; y< TamanhoY-AlturaMatch;y++)
            {
                _itens[match.matchComecoX, y].MudarPosicao(match.matchComecoX, y);
            }
            for (int i = 0; i < match.match.Count;i++)
            {
                _itens[match.matchComecoX, (TamanhoY - 1) - i] = InstanciaPecas(match.matchComecoX, (TamanhoY - 1) - i);
            }
        }
        for (int x = 0; x < TamanhoX; x++)
        {
            for (int y = 0; y < TamanhoY; y++)
            {
                InformacaoMatch matchInfo = PegaInformacao(_itens[x, y]);
                if (matchInfo.MatchValido)
                {
                    yield return new WaitForSeconds(Velocidade);
                    yield return StartCoroutine(DestroiItem(matchInfo.match));
                    yield return new WaitForSeconds(Velocidade);
                    yield return StartCoroutine(AtualizaGrid(matchInfo));
                }
            }
        }
    }

    IEnumerator DestroiItem(List<griditem> itens)
    {
        foreach(griditem i in itens)
        {
            yield return StartCoroutine(i.transform.escala(Vector3.zero,Velocidade));
            Destroy(i.gameObject);
        }
    }

    IEnumerator troca (griditem a, griditem b)
    {
        MudaStatus(false);//desativa rigidbody
        Vector3 PosicaoA = a.transform.position;
        Vector3 PosicaoB = b.transform.position;
        StartCoroutine(a.transform.Mova(PosicaoB, Velocidade));
        StartCoroutine(b.transform.Mova(PosicaoA, Velocidade));
        trocaIndicie(a, b);
        yield return new WaitForSeconds(Velocidade);
        MudaStatus(true);//reativa
    }

    void trocaIndicie(griditem a, griditem b)
    {
        griditem Atemp = _itens[a.x, a.y];
        _itens[a.x, a.y] = b;
        _itens[b.x, b.y] = Atemp;
        int XantigoB = b.x; int YantigoB = b.y;
        b.MudarPosicao(a.x, a.y);
        a.MudarPosicao(b.x, b.y);
    }

    List<griditem> matchHorizantal(griditem item)
    {
        List<griditem> Hitens = new List<griditem> { item };
        int esq = item.x - 1;
        int dir = item.x + 1;
        while(esq>= 0 && _itens[esq,item.y].id == item.id)
        {
            Hitens.Add(_itens[esq,item.y]);
            esq--;
        }
        while(dir < TamanhoX && _itens[dir,item.y].id == item.id)
        {
            Hitens.Add(_itens[dir,item.y]);
            dir++;
        }
        return Hitens;
    }

    List<griditem> matchVertical(griditem item)
    {
        List<griditem> Vitens = new List<griditem> { item };
        int cima = item.y + 1;
        int baixo = item.y - 1;
        while (baixo >= 0 && _itens [item.x,baixo].id == item.id)
        {
            Vitens.Add(_itens[item.x, baixo]);
            baixo--;
        }
        while (cima < TamanhoY && _itens[item.x,cima].id == item.id)
        {
            Vitens.Add(_itens[item.x, cima]);
            cima++;
        }
        return Vitens;  
    }

    InformacaoMatch PegaInformacao(griditem item)
    {
        InformacaoMatch m = new InformacaoMatch();
        m.match = null;
        List<griditem> Hmatch = matchHorizantal(item);
        List<griditem> Vmatch = matchVertical(item);
        if (Hmatch.Count >= 3 && Hmatch.Count > Vmatch.Count) 
        {
            m.matchComecoX = MinimoX(Hmatch);
            m.matchFinalX = MaximoX(Hmatch);
            m.matchComecoY = m.matchFinalY = Hmatch[0].y;
            m.match = Hmatch;
        }
        else if(Vmatch.Count >= 3)
        {
            m.matchComecoY = MinimoY(Vmatch);
            m.matchFinalY = MaximoY(Vmatch);
            m.matchComecoX = m.matchFinalX = Vmatch[0].x;
            m.match = Vmatch;
        }

        return m;
    }

    int MaximoX (List<griditem> itens)
    {
        float[] indices = new float[itens.Count];
        for (int i = 0; i < indices.Length; i++)
        {
            indices[i] = itens[i].x;
        }
        return (int)Mathf.Max(indices);
    }

    int MinimoX(List<griditem> itens)
    {
        float[] indices = new float[itens.Count];
        for (int i = 0; i < indices.Length; i++)
        {
            indices[i] = itens[i].x;
        }
        return (int)Mathf.Min(indices);
    }

    int MaximoY(List<griditem> itens)
    {
        float[] indices = new float[itens.Count];
        for (int i = 0; i < indices.Length; i++)
        {
            indices[i] = itens[i].y;
        }
        return (int)Mathf.Max(indices);
    }

    int MinimoY(List<griditem> itens)
    {
        float[] indices = new float[itens.Count];
        for (int i = 0; i < indices.Length; i++)
        {
            indices[i] = itens[i].y;
        }
        return (int)Mathf.Min(indices);
    }

    void PegaPecas ()
    {
        _pecas = Resources.LoadAll <GameObject> ("Prefabs");
        for (int i = 0; i < _pecas.Length; i++)
        {
            _pecas[i].GetComponent<griditem>().id = i;
        }
        
    }
    void MudaStatus(bool status)
    {
        foreach (griditem g in _itens)
        {
            g.GetComponent<Rigidbody2D>().isKinematic = !status;
        }
    }
}
