using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    [Header("--- Airport ---")]
    [SerializeField] private RectTransform airportImage;
    [SerializeField] private Transform airportTransform;
    private Transform selectedJet;

    [Header("--- Enemies ---")]
    public List<Transform> Enemies;


    void Start()
    { 
        selectedJet = WhichJetSelected.SelectedJet.transform;
    }

    private void FixedUpdate()
    {
        if (!selectedJet)
            return;

        CalculateAirportImagePos();
    }

    private void CalculateAirportImagePos()
    {
        Vector3 distance = airportTransform.position - selectedJet.position;
        distance.y = 0;

        Vector3 selectedJetForward = selectedJet.forward;
        selectedJetForward.y = 0;

        float angle = Vector3.SignedAngle(distance.normalized, selectedJetForward, Vector3.up);

        if (angle < -90 || angle > 90)
        {
            airportImage.gameObject.SetActive(false);
        }
        else
        {
            airportImage.DOKill();
            airportImage.gameObject.SetActive(true);
            airportImage.DOLocalMoveX(-angle * 2.7f, 0);
        }
    }
}
