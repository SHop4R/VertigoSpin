using System;
using UnityEngine;

namespace VertigoSpin.Project.Scripts.UI
{
    [Serializable]
    public struct Panel
    {
        [field: SerializeField] public GameObject PanelObject{ get; private set; }
        [field: SerializeField] public PanelType PanelType{ get; private set; }

        public void Show() => PanelObject.SetActive(true);
        public void Hide() => PanelObject.SetActive(false);
    }
}