using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using static OVRSkeleton;

public class FingersTracker : MonoBehaviour
{
    public event Action<IList<OVRBone>> OnLeftHandFingersFound, OnRightHandFingersFound;
    [Header("Hands")]
    [SerializeField] private OVRHand _leftHand;
    [SerializeField] private OVRHand _rightHand;

    private OVRSkeleton _leftSkeleton;
    private OVRSkeleton _rightSkeleton;
    
    private Coroutine _waitLeftSkeleton, _waitRightSkeleton;
    private void Start()
    {
        TryFindHands();
        
        _waitLeftSkeleton = StartCoroutine(WaitForSkeletonUpdated(_leftSkeleton,
            skeleton => OnLeftHandFingersFound?.Invoke(UpdateFingerTips(skeleton))));
        _waitRightSkeleton = StartCoroutine(WaitForSkeletonUpdated(_rightSkeleton, 
            skeleton => OnRightHandFingersFound?.Invoke(UpdateFingerTips(skeleton))));
    }
    private void OnDestroy()
    {
        StopCoroutine(_waitLeftSkeleton);
        StopCoroutine(_waitRightSkeleton);
    }
    private void TryFindHands()
    {
        if (!_leftHand)
            _leftHand = GameObject.Find("LeftHandAnchor")?.GetComponentInChildren<OVRHand>();
        if (!_rightHand)
            _rightHand = GameObject.Find("RightHandAnchor")?.GetComponentInChildren<OVRHand>();

        _leftSkeleton = _leftHand?.GetComponent<OVRSkeleton>();
        _rightSkeleton = _rightHand?.GetComponent<OVRSkeleton>();
    }
    private static IEnumerator WaitForSkeletonUpdated(OVRSkeleton skeleton, Action<OVRSkeleton> onSkeletonUpdated)
    {
        yield return new WaitUntil(() => skeleton && skeleton.IsDataValid && skeleton.IsDataHighConfidence);
        onSkeletonUpdated?.Invoke(skeleton);
    }
    private static IList<OVRBone> UpdateFingerTips(OVRSkeleton skeleton)
    {
        if (!skeleton.IsDataValid || skeleton.Bones == null)
            return null;

        return skeleton.Bones;
    }
}
