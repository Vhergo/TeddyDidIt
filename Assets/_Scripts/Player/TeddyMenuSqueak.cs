using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TeddyMenuSqueak : MonoBehaviour
{
    [SerializeField] private AudioClip squeak;
    [SerializeField] private LayerMask teddyLayer;
    [SerializeField] private Transform teddy;
    [SerializeField] private float squeakScale;
    [SerializeField] private float squeakCooldown;
    [SerializeField] private float squeakTime;
    [SerializeField] private string ignoreUIElementTag;
    private float originalScale;
    private MeshCollider meshCollider;
    private bool canSqueak = true;
    private float cooldownTimer;

    private void Start()
    {
        originalScale = teddy.localScale.x;
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && canSqueak && !IsPointerOverUI()) {
            Debug.Log("SQUEAK");
            PlaySqueakyNoise();
        }
    }

    private void PlaySqueakyNoise()
    {
        RaycastHit raycastHit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out raycastHit, 1000f, teddyLayer)) {
            SoundManager.Instance.PlaySound(squeak, true);
            StartCoroutine(StartSqueakCooldown());
        } else {
            Debug.Log("No hit detected.");
        }
    }

    private IEnumerator StartSqueakCooldown()
    {
        canSqueak = false;
        teddy.localScale = new Vector3(squeakScale, squeakScale, squeakScale);
        yield return new WaitForSeconds(squeakTime);
        teddy.localScale = new Vector3(originalScale, originalScale, originalScale);
        yield return new WaitForSeconds(squeakCooldown - squeakTime);
        canSqueak = true;
    }

    private bool IsPointerOverUI()
    {
        if (EventSystem.current.IsPointerOverGameObject()) {
            PointerEventData eventData = new PointerEventData(EventSystem.current) {
                position = Input.mousePosition
            };
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            foreach (RaycastResult result in results) {
                if (result.gameObject.CompareTag(ignoreUIElementTag)) {
                    Debug.Log(result.gameObject.name);
                    return false;
                }else {
                    Debug.Log("CHECK");
                    return true;
                }
            }
        }
        return false;
    }
}
