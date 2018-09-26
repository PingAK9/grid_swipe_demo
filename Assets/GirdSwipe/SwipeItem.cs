using UnityEngine;

public class SwipeItem : MonoBehaviour {

	void Start () {
        rect = gameObject.GetComponent<RectTransform>();
    }
    
    float position = -321;
    bool isFront = false;
    RectTransform rect;
    float minScale;
    float spacing;
    public virtual void SetPosition(float value, float scale, float spacing)
    {
        if (position == value && this.minScale == scale && this.spacing == spacing)
        {
            return;
        }
        this.minScale = scale;
        this.spacing = spacing;
        position = value;
        float _abs = Mathf.Abs(value);
        if (isFront == false && _abs < 0.5f)
        {
            isFront = true;
            transform.SetAsLastSibling();
        }
        if (isFront == true && _abs > 0.5f)
        {
            isFront = false;
        }
        // pos
        Vector3 _pos = new Vector3(value, 0, 0);
        rect.localPosition = Vector3.zero - (_pos * spacing);
        // scale
        float _scale = 1 - (1 - minScale) * _abs;
        rect.localScale = Vector3.one * _scale;
    }
}
