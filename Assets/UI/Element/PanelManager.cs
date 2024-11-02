using System.Collections.Generic;
using UnityEngine;

namespace Syncodech.UI
{
    [ExecuteAlways]
    public class PanelManager : UIBehaviour
    {
        [SerializeField] private int panelIndex;
        [SerializeField] private List<Panel> panels;

        private void OnValidate()
        {
            Change(panelIndex);
        }

        public Panel Get(int index)
        {
            if (index < 0 || index >= panels.Count) return null;
            return panels[index];
        }
        
        public Panel Change(int index)
        {
            index = Mathf.Clamp(index, 0, panels.Count - 1);
            panelIndex = index;
            for (int i = 0; i < panels.Count; i++)
            {
                panels[i].Visible(i == index);
            }
            return panels[panelIndex];
        }
    }
}