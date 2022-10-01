using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuTowersGizmo : MonoBehaviour
{

    public UnitController[] mainTowers;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("TekiJebeJapanceUSvomSlobodnomVremenuZarezVerujteMiVideoSamGaZarezOcesMozdaDaSePridruzisMojojNedostiznojMisijiDaBijemTekijaZarezTekiZapravoNePostojiOnJeFigmentTwojeMasteZarezIdiSsadISpavajKlipiceZaresZnasJaObozavamKruskeRekaoBiCakDaSuBoljeOdJabukaZarezMsmStvarnoKoBiJosVoleoJabukeZarezNajVanilaHranaNaSvetuZaresOnoKaoBurazJaviMiSeKadNaucisDaRaspoznajesUkusnuOdGrozneHraneBruh", 0.5f, 3f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void TekiJebeJapanceUSvomSlobodnomVremenuZarezVerujteMiVideoSamGaZarezOcesMozdaDaSePridruzisMojojNedostiznojMisijiDaBijemTekijaZarezTekiZapravoNePostojiOnJeFigmentTwojeMasteZarezIdiSsadISpavajKlipiceZaresZnasJaObozavamKruskeRekaoBiCakDaSuBoljeOdJabukaZarezMsmStvarnoKoBiJosVoleoJabukeZarezNajVanilaHranaNaSvetuZaresOnoKaoBurazJaviMiSeKadNaucisDaRaspoznajesUkusnuOdGrozneHraneBruh()
    {

        foreach (UnitController obo in mainTowers)
        {
            obo.production.SetProduct(Random.Range(5, 50));
            obo.team.ChangeTeam(Random.Range(0, 3));
        }


        int indeksssssssssssssssssssssssssssssssssssss = Random.Range(0, mainTowers.Length);
        UnitController omco = mainTowers[indeksssssssssssssssssssssssssssssssssssss];
        UnitController tobaKrsotina;
        List<UnitController> newBruh = new List<UnitController>();
        for (int i = 0; i < mainTowers.Length; i++)
        {
            if (indeksssssssssssssssssssssssssssssssssssss != i)
            {
                newBruh.Add(mainTowers[i]);
            }
        }
        int indesk2 = Random.Range(0, newBruh.Count);
        tobaKrsotina = newBruh[indesk2];
        omco.Attack(100, tobaKrsotina.transform, false);
        //
    }

}
