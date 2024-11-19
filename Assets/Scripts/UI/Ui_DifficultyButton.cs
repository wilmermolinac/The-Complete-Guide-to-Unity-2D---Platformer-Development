using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Ui_DifficultyButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI _difficultyInfoText;
    
    [TextArea]
    [SerializeField] private string _textDescription;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        _difficultyInfoText.text = _textDescription;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
       _difficultyInfoText.text = "";
    }
}
