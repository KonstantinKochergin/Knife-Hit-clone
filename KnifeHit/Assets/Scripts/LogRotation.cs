using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogRotation : MonoBehaviour {

    [SerializeField]
    private float rotationSpeed;

	
	void Start () {
		
	}
	
	
	void Update () {
        rotate();
        acceleration();
	}

    void rotate()
    {
        this.transform.Rotate(new Vector3(0, 0, rotationSpeed * Time.deltaTime));
    }

    IEnumerator acceleration()   // увеличение скорости вращения бревна со временем
    {
        while (true)
        {
            rotationSpeed += 1;
            yield return new WaitForSeconds(2f);
        }
    }
}
