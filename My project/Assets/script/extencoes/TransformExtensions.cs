using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformEstensions
{
    public static IEnumerator Mova(this Transform t, Vector3 alvo, float tempo)
    {
        Vector3 VetorDiferenca = (alvo - t.position);
        float TamanhaDiferenca = VetorDiferenca.magnitude;
        VetorDiferenca.Normalize();
        float contador = 0;
        while(contador < tempo)
        {
            float Movimento = (Time.deltaTime * TamanhaDiferenca) / tempo;
            t.position += VetorDiferenca * Movimento;
            contador += Time.deltaTime;
            yield return null;
        }
        t.position = alvo;
    }
    public static IEnumerator escala(this Transform t, Vector3 alvo, float tempo)
    {
        Vector3 VetorDiferenca = (alvo - t.localScale);
        float TamanhaDiferenca = VetorDiferenca.magnitude;
        VetorDiferenca.Normalize();
        float contador = 0;
        while (contador < tempo)
        {
            float Movimento = (Time.deltaTime * TamanhaDiferenca) / tempo;
            t.localScale += VetorDiferenca * Movimento;
            contador += Time.deltaTime;
            yield return null;
        }
        t.localScale = alvo;
    }
}
