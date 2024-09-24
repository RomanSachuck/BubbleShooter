using System.Collections;
using DG.Tweening;
using UnityEngine;

public class ScaleAnimation : MonoBehaviour
{
    private void OnEnable() =>
        StartCoroutine(RunAnimation());

    private IEnumerator RunAnimation()
    {
        while (true)
        {
            DOTween.Sequence()
                .Append(transform.DOScale(1.5f, .75f))
                .Append(transform.DOScale(1, .75f)); 
            
            yield return new WaitForSeconds(1.5f);
        }
    }
}
