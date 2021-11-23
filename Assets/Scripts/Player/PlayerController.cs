using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject _debugScreen;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
        {
            _debugScreen.SetActive(!_debugScreen.activeSelf);
        }
    }
}
