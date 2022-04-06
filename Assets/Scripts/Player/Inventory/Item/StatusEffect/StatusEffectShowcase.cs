using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectShowcase : MonoBehaviour
{
    [SerializeField] private SpriteRenderer iconRenderer;

    [SerializeField] private string statusName;
    [SerializeField] private Sprite icon;

    public bool showingIcon { get; private set; }

    public void setStatusName(string name) => statusName = name;
    public string getStatusName() => statusName;

    public void setShowingIcon(bool isShowing)
    {
        showingIcon = isShowing;
        transform.GetChild(0).gameObject.SetActive(isShowing);
    }

    private void Start()
    {
        iconRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }
    public void setStatusIcon(Sprite icon)
    {
        iconRenderer.sprite = icon ? icon : null;
    }

    public void setStatusEffect(StatusEffect effect)
    {
        if (effect)
        {
            setStatusName(effect.statusName);
            setStatusIcon(effect.icon);
        }

        setShowingIcon(effect);
    }
}
