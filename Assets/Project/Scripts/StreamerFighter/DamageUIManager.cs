using DamageNumbersPro;
using UnityEngine;
namespace StreamerFighter {
    public class DamageUIManager : MonoBehaviour {
        public DamageNumberMesh regularDamage;
        public DamageNumberMesh poisonDamage;
        
        public void SpawnDamage(int damageAmount, Vector2 position) {
            regularDamage.Spawn( position, damageAmount );
        }
        
        public void SpawnPoisonDamage(int damageAmount, Vector2 position) {
            poisonDamage.Spawn( position, damageAmount );
        }
    }
}