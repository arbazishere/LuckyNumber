using UnityEngine;

namespace ArbazAminKhan.Pool.Components
{
    public class ObjectPool : MonoBehaviour
    {
        ObjectPool_Data _objectPoolData = new();

        public ObjectPool Spawn()
        {
            var objPoolComp = _objectPoolData._despawnedData.Count > 0 ? _objectPoolData._despawnedData[default] : Instantiate(this, this.transform.parent);
            objPoolComp._objectPoolData._prefab = this;

            _objectPoolData._despawnedData.Remove(objPoolComp);
            _objectPoolData._spawnedData.Add(objPoolComp);

            objPoolComp.gameObject.SetActive(true);

            return objPoolComp;
        }

        public void DespawnAllInstance()
        {
            foreach (var pool in _objectPoolData._spawnedData) pool.gameObject.SetActive(false);
            _objectPoolData._despawnedData.AddRange(_objectPoolData._spawnedData);
            _objectPoolData._spawnedData.Clear();
        }

        public void Despawn() => _objectPoolData._prefab.Despawn(this);

        public void Despawn(ObjectPool pool)
        {
            _objectPoolData._spawnedData.Remove(pool);
            _objectPoolData._despawnedData.Add(pool);

            pool.gameObject.SetActive(false);
        }
    }

    [System.Serializable]
    public class ObjectPool_Data
    {
        public ObjectPool _prefab;

        [Space(8)]
        public System.Collections.Generic.List<ObjectPool> _spawnedData = new();
        public System.Collections.Generic.List<ObjectPool> _despawnedData = new();
    }
}