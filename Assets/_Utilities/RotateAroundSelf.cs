using UnityEngine;

namespace Enemy.Nav_AI
{
    public class RotateAroundSelf : MonoBehaviour
    {
        [SerializeField] private float rotateSpeed;
        [SerializeField] private Vector3 rotateDir;

        private Vector3 rotateVector;

        private void Start()
        {
            rotateVector = rotateSpeed * rotateDir;
        }

        void Update()
        {
            transform.Rotate(rotateVector*Time.deltaTime);    
        }
    }
}
