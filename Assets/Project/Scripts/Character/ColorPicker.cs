using UnityEngine;

public class ColorPicker : MonoBehaviour {
    [SerializeField] Color[] colors;
    [SerializeField] SpriteRenderer[] spriteRenderer;

    void Awake() {
        //spriteRenderer = GetComponentsInChildren<SpriteRenderer>();
        int randomIndex = Random.Range( 0, colors.Length );
        Color randomColor = colors[randomIndex];

        foreach (var spriteRenderer1 in spriteRenderer) {
            spriteRenderer1.color = randomColor;
        }
    }
}