using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubePreview : MonoBehaviour
{
    [SerializeField] int startIndex;
    [SerializeField] int currentIndex;
    [SerializeField] bool isIncomingCube;
    Cube cube;
    MeshRenderer meshRenderer;
    private void Awake() 
    { 
        cube = GetComponent<Cube>();
        meshRenderer= GetComponent<MeshRenderer>();
    }
    private void OnEnable() => GameManager.OnRoundChange += UpdateCubeAppearance;

    private void OnDisable() => GameManager.OnRoundChange -= UpdateCubeAppearance;

    void UpdateCubeAppearance(int round)
    {
        currentIndex = startIndex + round;
        Debug.Log(CubeManager.Instance.GetIncomingCube()[currentIndex]);
        cube.InitCubeType(CubeManager.Instance.GetIncomingCube()[currentIndex].Value);
        if (startIndex==0)
        {
             DoTweenMoving();
        }
        else
        {
            DoTweenScaling();
        }
            
    }
    void DoTweenMoving()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(Vector3.zero, 0.0f));
        sequence.Append(transform.DOMove(GridManager.Instance.GetIncomingNodePos().position + new Vector3(0, 0, -1f), 0f));
        sequence.Append(transform.DOScale(new Vector3(0.7f, 0.7f, 0.7f), 0f).SetEase(Ease.OutSine));
        sequence.Append(transform.DOMove(GridManager.Instance.GetCurrentNodePos().position + new Vector3(0, 0, -1f), 0.3f).SetEase(Ease.InOutBack));
        sequence.Join(transform.DOScale(Vector3.one, 0.3f));
    }
    void DoTweenScaling()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(Vector3.zero, 0f));
        sequence.Append(transform.DOScale(new Vector3(0.7f, 0.7f, 0.7f), 0.3f).SetEase(Ease.OutCirc));
    }
} 
