using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scroller : MonoBehaviour
{
    public List<Transform> transforms = new List<Transform>();

    private Transform origin;

    public float cardSize = 1f;

    public float cardSpace = 150f;

    private bool examineCard;

    public float slidingSpeed = 5f;

    public float rotationSpeed = 5f;

    void Start()
    {
        origin = transform;
    }

    void Update()
    {
        UpdateCards();
    }

    public void UpdateCards()
    {
        for(var i = 0; i < transforms.Count; i++)
        {
            Vector3 newPosition = origin.position + Vector3.right * (i - transforms.Count / 2) * cardSpace;
            newPosition += Vector3.forward * Mathf.Pow(Mathf.Abs(newPosition.x - transform.position.x)/ cardSpace, 2) * 100f;
            transforms[i].position = Vector3.Lerp(transforms[i].position, newPosition, slidingSpeed * Time.deltaTime);

            SpriteRenderer front = transforms[i].GetChild(0).GetComponent<SpriteRenderer>();
            SpriteRenderer back = transforms[i].GetChild(1).GetComponent<SpriteRenderer>();

            front.color = new Color(front.color.r, front.color.g, front.color.b, (400 - Mathf.Abs(transforms[i].position.z - transform.position.z))/400);
            back.color = new Color(back.color.r, back.color.g, back.color.b, (400 - Mathf.Abs(transforms[i].position.z - transform.position.z)) / 400);

            if (i != transforms.Count / 2 || !examineCard)
                transforms[i].rotation = Quaternion.Lerp(transforms[i].rotation, Quaternion.LookRotation(Vector3.forward), rotationSpeed * Time.deltaTime);
            else if (examineCard)
                transforms[i].rotation = Quaternion.Lerp(transforms[i].rotation, Quaternion.LookRotation(Vector3.back), rotationSpeed * Time.deltaTime);

            transforms[i].localScale = Vector3.one * (cardSize + front.color.a * cardSize);
            transforms[i].localScale = new Vector3(transforms[i].localScale.x, transforms[i].localScale.y, 1);

     
        }
    }

    public void ScrollLeft()
    {      
        var firstObject = transforms[0];
        transforms.RemoveAt(0);
        transforms.Add(firstObject);
        examineCard = false;
    }

    public void ScrollRight()
    {
        var lastObject = transforms[transforms.Count - 1];
        transforms.RemoveAt(transforms.Count - 1);
        transforms.Insert(0, lastObject);
        examineCard = false;
    }

    public void FlipCard()
    {
        examineCard = !examineCard;
    }

    public GameObject SelectedCard()
    {
        return transforms[transforms.Count / 2].gameObject;
    }
}
