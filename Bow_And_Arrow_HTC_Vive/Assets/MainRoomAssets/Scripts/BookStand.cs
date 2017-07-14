using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FusedVR {
    public class BookStand : MonoBehaviour {

        private bool firstTime = false;

        void OnTriggerEnter(Collider other) {
            Book b = other.GetComponent<Book>();
            if (b != null)
                StartCoroutine(LaunchGame(b));
            
        }

        IEnumerator LaunchGame(Book book) {
            if (!firstTime) {
                firstTime = true;
                book.book.Open_Book();
                yield return new WaitForSeconds(1.5f);
                (LobbyManager.Instance as LobbyManager).StartGame();
            }
        }
    }
}