using DamageNumbersPro;
using UnityEngine;
namespace Game {
    public class DamageUIManager : MonoBehaviour {
        public DamageNumber numberPrefab;
        public RectTransform rectParent;
        
        public void SpawnDamageNumber(int damageAmount, Vector2 position) {
            numberPrefab.SpawnGUI( rectParent, position, damageAmount );
        }
    }
}