using UnityEngine;

public class HeadScrpit : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.CompareTag("Platform") && !GameManager.Instance.isDie) {
            GameManager.Instance.PlaySound("crack");
            GameManager.Instance.StartGameOver();
        }
    }
}