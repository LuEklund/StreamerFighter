using DamageNumbersPro;
using UnityEngine;
using UnityEngine.Serialization;
namespace Game {
    public class DamageUIManager : MonoBehaviour {
        [FormerlySerializedAs( "numberPrefab" )] 
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