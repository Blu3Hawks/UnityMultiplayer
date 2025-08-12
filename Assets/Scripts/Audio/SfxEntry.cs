using UnityEngine;

namespace Audio {
    [CreateAssetMenu(fileName = "Sfx Entry", menuName = "Sfx Entry", order = 0)]
    public class SfxEntry : ScriptableObject {
        public SfxId id; 
        public AudioClip clip; 
        public bool spatial3D;
    }
}