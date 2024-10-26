using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace UISystem
{
    public enum BarType : byte
    {
        Number = 0,
        Percent = 1
    }
    
    [ExecuteAlways]
    public class Bar : UIBehaviour
    {
        [SerializeField] private BarType type;
        [SerializeField] private bool wholeNumbers;
        [SerializeField] private float value;
        [SerializeField] private float min;
        [SerializeField] private float max;
        [SerializeField] private Direction direction;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Fill fill;

        public UnityEvent<float> onChange;
        
        public void Update()
        {
            Set(value);
        }

        public void Set(float value, bool callback = true)
        {
            value = Mathf.Clamp(value, min, max);

            float tmpValue = value;
            
            value = (value - min) / (max - min);
            
            fill.Set(value,direction);

            switch (type)
            {
                case BarType.Number:
                    if (text != null) text.SetText(wholeNumbers ? $"{tmpValue:0}" : $"{tmpValue:0.0}");
                    if(!callback) return; 
                    onChange.Invoke(tmpValue);
                    break;
                case BarType.Percent:
                    if (text != null) text.SetText($"{value * 100:0}%");
                    if(!callback) return; 
                    onChange.Invoke(value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}