using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDC.Enums;

namespace GDC.Home
{
    public class Pool : MonoBehaviour
    {
        [SerializeField] List<GameObject> m_PooledObjects;
        public void Setup(GameObject objectToPool, int amountToPool, Transform parent)
        {
            m_PooledObjects = new List<GameObject>();
            for (int i = 0; i < amountToPool; i++)
            {
                GameObject obj = (GameObject)Instantiate(objectToPool, parent);
                obj.SetActive(false);
                m_PooledObjects.Add(obj);
            }
        }
        public void Setup(GameObject[] objectsToPool, int amountForEach, Transform[] parentForEach)
        {
            m_PooledObjects = new List<GameObject>();
            int parentIdx = 0;
            foreach (GameObject go in objectsToPool)
            {
                for (int i = 0; i < amountForEach; i++)
                {
                    GameObject obj = (GameObject)Instantiate(go, new Vector3(100, 0, 0), Quaternion.identity, parentForEach[parentIdx]);
                    obj.SetActive(false);
                    m_PooledObjects.Add(obj);
                }
                parentIdx++;
            }
        }
        public GameObject GetPooledObject()
        {
            // List<GameObject> temp = m_PooledObjects.Where(o => !o.activeInHierarchy).ToList();
            // return temp[Random.Range(0, temp.Count)];
            for (int i = 0; i < m_PooledObjects.Count; i++)
            {
                if (!m_PooledObjects[i].activeInHierarchy)
                {
                    return m_PooledObjects[i];
                }
            }
            return null;
        }
    }
}
