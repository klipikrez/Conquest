using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CreateTeamButtons : MonoBehaviour
{
    public List<TeamButton> teamButtons = new List<TeamButton>();



    // Start is called before the first frame update
    void Start()
    {


    }
    private bool IsImage(string fileName)
    {
        string extension = Path.GetExtension(fileName).ToLower();
        return extension == ".png" || extension == ".jpg" || extension == ".jpeg" || extension == ".bmp";
    }
    public void DeselectAll()
    {
        foreach (TeamButton but in teamButtons)
        {
            but.Deselelect();
        }
    }

    public void SelectNoUpdate(int team)
    {
        foreach (TeamButton but in teamButtons)
        {
            if (but.team == team) but.SelectNoUpdate();
        }
    }



}
