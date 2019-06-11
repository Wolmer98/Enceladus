using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardScroller : MonoBehaviour
{
    public List<Character_DisplayCard> cards;
    public Transform cardParent;

    private Transform origin;

    public float cardSize = 1f;

    public float cardSpace = 150f;

    private bool examineCard;

    public float slidingSpeed = 5f;

    public float rotationSpeed = 5f;

    public void InitCardScroller()
    {
        origin = transform;
        UpdateCardHierarchy();
        ScrollRight();
    }

    void Update()
    {
        UpdateCards();
    }

    public void UpdateCards()
    {
        for (var i = 0; i < cards.Count; i++)
        {
            bool isCenterCard = (cards[i] == cards[cards.Count / 2]);

            Vector3 newPosition = origin.localPosition + Vector3.right * (i - cards.Count / 2) * cardSpace;
            newPosition += Vector3.forward * Mathf.Pow(Mathf.Abs(newPosition.x - transform.localPosition.x) / cardSpace, 2) * 100f;
            cards[i].RectTransform.anchoredPosition = Vector3.Lerp(cards[i].RectTransform.anchoredPosition, newPosition, slidingSpeed * Time.deltaTime);
            cards[i].transform.localPosition = new Vector3(cards[i].RectTransform.anchoredPosition.x, cards[i].RectTransform.anchoredPosition.y, Mathf.Lerp(cards[i].transform.localPosition.z, newPosition.z, Time.deltaTime * slidingSpeed));

            CanvasGroup card = cards[i].cardGroup;
            CanvasGroup front = cards[i].frontGroup;
            CanvasGroup back = cards[i].backGroup;

            card.alpha = ((400 - Mathf.Abs(cards[i].transform.localPosition.z - transform.position.z)) / 400);

            Vector3 newScale = Vector3.one * (card.alpha * cardSize);
            if (i != cards.Count / 2 || !examineCard)
            {
                //cards[i].RectTransform.rotation = Quaternion.Lerp(cards[i].RectTransform.rotation, Quaternion.LookRotation(Vector3.forward), rotationSpeed * Time.deltaTime);
                //cards[i].RectTransform.localScale = newScale;
                cards[i].RectTransform.localScale = new Vector3(Mathf.Lerp(cards[i].RectTransform.localScale.x, newScale.x, Time.deltaTime * rotationSpeed), newScale.y, newScale.z);

                if (Mathf.Abs(cards[i].RectTransform.localScale.x) < 0.1f * (rotationSpeed / 5))
                {
                    front.alpha = 1;
                    back.alpha = 0;
                }
            }               
            else if (examineCard)
            {
                //cards[i].RectTransform.rotation = Quaternion.Lerp(cards[i].RectTransform.rotation, Quaternion.LookRotation(Vector3.back), rotationSpeed * Time.deltaTime);              
                cards[i].RectTransform.localScale = new Vector3(Mathf.Lerp(cards[i].RectTransform.localScale.x, -newScale.x, Time.deltaTime * rotationSpeed), newScale.y, newScale.z);

                if (Mathf.Abs(cards[i].RectTransform.localScale.x) < 0.1f * (rotationSpeed / 5))
                {
                    front.alpha = 0;
                    back.alpha = 1;
                }
            }

            cards[i].RectTransform.localScale = new Vector3(cards[i].RectTransform.localScale.x, cards[i].RectTransform.localScale.y, 1);
        }
    }

    public void ScrollLeft()
    {
        var firstObject = cards[0];
        cards.RemoveAt(0);
        cards.Add(firstObject);
        firstObject.transform.SetAsFirstSibling();
        examineCard = false;

        UpdateCardHierarchy();
    }

    public void ScrollRight()
    {
        var lastObject = cards[cards.Count - 1];
        cards.RemoveAt(cards.Count - 1);
        cards.Insert(0, lastObject);
        lastObject.transform.SetAsFirstSibling();
        examineCard = false;

        UpdateCardHierarchy();
    }

    private void UpdateCardHierarchy()
    {
        cards[cards.Count / 2].transform.SetAsLastSibling();
    }

    public void FlipCard()
    {
        examineCard = !examineCard;
    }

    public Character_DisplayCard SelectedCard()
    {
        return cards[cards.Count / 2];
    }
}
