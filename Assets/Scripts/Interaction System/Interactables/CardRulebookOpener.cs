
using UnityEngine;

public class CardRulebookOpener : MonoBehaviour, IRulebookOpener
{
    public IRulebookEntry RulebookEntry { get => _card; }
    [SerializeField] private ACard _card;
}
