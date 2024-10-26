using System.Collections.Generic;
using UnityEngine;

namespace UISystem
{
    [ExecuteAlways]
    public class PanelManager : UIBehaviour
    {
        [SerializeField] private int panelIndex;
        [SerializeField] private List<Panel> panels;

        private void OnValidate()
        {
            ChangePanel(panelIndex);
        }

        public void ChangePanel(int index)
        {
            index = Mathf.Clamp(index, 0, panels.Count - 1);
            panelIndex = index;
            for (int i = 0; i < panels.Count; i++)
            {
                panels[i].Visible(i == index);
            }
        }
    }
}