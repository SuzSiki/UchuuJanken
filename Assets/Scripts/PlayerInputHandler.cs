using UnityEngine;

public class PlayerInputHandler:IJankenInputHandler
{
    JankenPlayer master;
    SelectionButtonPanel selectionButton;

    public PlayerInputHandler(JankenPlayer master)
    {
        this.master = master;
        selectionButton = SelectionButtonPanel.GetPanel(SelectionPanelName.HandSelection);
        selectionButton.OnSelect += Select;
    }

    
    public void OnHandSelect()
    {
        selectionButton.Show();
        selectionButton.DisableButton((int)master.lastHand);
    }

    void Select(int id)
    {
        //idが0-3だがQuantumHandの0はnone
        id+=1;
        master.SetHand((QuantumHand)id);
        selectionButton.Hide(id);
    }
}