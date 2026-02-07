using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GorillaLevelEditor.Core
{
    internal class CoroutineManager : MonoBehaviour
    {
        public static CoroutineManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }
    }
}
