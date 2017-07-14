using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

[RequireComponent(typeof(Rigidbody))]
public class Book : VRTK_InteractableObject {

    [Tooltip("To let the book stand know how to control the book")]
    public BookControl book;

}
