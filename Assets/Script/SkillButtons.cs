using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillButtons : MonoBehaviour
{
    [SerializeField] SkillButton buttonPrefab;

    int selected = -1;

    Vector3 pos0 = new Vector3(-2.1f, -5.9f, -7f);
    Vector3 pos1 = new Vector3(-0.7f, -5.9f, -7f);
    Vector3 pos2 = new Vector3(0.7f, -5.9f, -7f);
    Vector3 pos3 = new Vector3(2.1f, -5.9f, -7f);
    Vector3 posSpawn = new Vector3(5f, -5.9f, -7f);
    Vector3 selectMod = new Vector3(0, 0.2f, 0);

    List<SkillButton> buttons = new List<SkillButton>();

    // Start is called before the first frame update
    void Start()
    {
        while(buttons.Count < 4)
        {
            SpawnButton();
        }
        buttons[0].transform.position = pos0;
        buttons[1].transform.position = pos1;
        buttons[2].transform.position = pos2;
        buttons[3].transform.position = pos3;
        RelocateButtons();
    }

    void RelocateButtons()
    {
        if (buttons[0] != null)
        {
            buttons[0].StartMove(pos0 + (selected == 0 ? selectMod : Vector3.zero));
        }
        if (buttons[1] != null)
        {
            buttons[1].StartMove(pos1 + (selected == 1 ? selectMod : Vector3.zero));
        }
        if (buttons[2] != null)
        {
            buttons[2].StartMove(pos2 + (selected == 2 ? selectMod : Vector3.zero));
        }
        if (buttons[3] != null)
        {
            buttons[3].StartMove(pos3 + (selected == 3 ? selectMod : Vector3.zero));
        }
    }

    void SpawnButton()
    {
        SkillButton button = Instantiate(buttonPrefab, posSpawn, buttonPrefab.transform.rotation, transform);
        button.StartMove(posSpawn);
        buttons.Add(button);
    }

    public void SelectButton(SkillButton selectedButton)
    {
        selected = -1;
        for(int i = 0; i < buttons.Count; i++)
        {
            if( buttons[i] == selectedButton)
            {
                selected = i;
            }
        }
        RelocateButtons();
    }

    public void DeselectButtons()
    {
        selected = -1;
        RelocateButtons();
    }

    public void DestroySelected()
    {
        GameObject toDelete = buttons[selected].gameObject;
        buttons.RemoveAt(selected);
        Destroy(toDelete);
        selected = -1;
        
        SpawnButton();
        RelocateButtons();        
    }
}
