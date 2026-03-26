using UnityEngine;

public class UIParticle : MonoBehaviour
{
    private RectTransform uiTarget; // 기준이 되는 UI 요소

    private void Awake()
    {
        uiTarget = transform.parent.GetComponent<RectTransform>();
    }

    void Update()
    {
        // 1. UI 위치 → 화면 좌표
        Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, uiTarget.position);

        // 2. 화면 좌표 → 월드 좌표
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        worldPos.z = 0; // 파티클이 카메라와 겹치지 않도록 Z 조정

        // 3. 위치 이동
        transform.position = worldPos;
    }
}
